using Dapper;
using LBComandaAPI.DataBase;
using LBComandaAPI.Repository.Interfaces;
using LBComandaAPI.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.DAO
{
    public class CartaoDAO: ICartao
    {
        readonly IConfiguration _config;
        public CartaoDAO(IConfiguration config) { _config = config; }

        public async Task<bool> AbrirCartaoAsync(string token, int Nr_cartao, string Celular, string Nome, bool MenorIdade)
        {
            SqlTransaction t = null;
            try
            {
                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)).SoNumero())))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        t = conexao._conexao.BeginTransaction(IsolationLevel.ReadUncommitted);
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("select a.cd_empresa ")
                            .AppendLine("from VTB_DIV_Empresa a ")
                            .AppendLine("where dbo.FVALIDA_NUMEROS(a.NR_CGC) = '" + Encoding.UTF8.GetString(Convert.FromBase64String(token)).SoNumero() + "'");
                        string cd_empresa = await conexao._conexao.ExecuteScalarAsync<string>(sql.ToString(), transaction: t);
                        sql.Clear();
                        sql.AppendLine("select 1")
                            .AppendLine("from TB_RES_Cartao a")
                            .AppendLine("where a.nr_cartao = " + Nr_cartao.ToString())
                            .AppendLine("and a.st_registro = 'A'")
                            .AppendLine("and a.cd_empresa = '" + cd_empresa + "'");
                        var ret = await conexao._conexao.ExecuteScalarAsync<int>(sql.ToString(), transaction: t);
                        if (ret > 0)
                            return false;//Cartão ja esta aberto
                        else
                        {
                            DynamicParameters param;
                            //Verificar se cliente cadastrado
                            sql.Clear();
                            sql.AppendLine("select a.CD_Clifor")
                                .AppendLine("from TB_FIN_Clifor a")
                                .AppendLine("where dbo.FVALIDA_NUMEROS(a.celular) like '%" + Celular.SoNumero() + "'");
                            string cd_clifor = await conexao._conexao.ExecuteScalarAsync<string>(sql.ToString(), transaction: t);
                            if(string.IsNullOrWhiteSpace(cd_clifor))
                            {
                                //Cadastrar cliente
                                cd_clifor = await conexao._conexao.ExecuteScalarAsync<string>("select max(cd_clifor) from tb_fin_clifor", transaction: t);
                                cd_clifor = (int.Parse(cd_clifor) + 1).ToString().PadLeft(cd_clifor.Length, '0');
                                sql.Clear();
                                sql.AppendLine("select a.Cd_CondFiscal_Clifor")
                                    .AppendLine("from TB_RES_Config a")
                                    .AppendLine("where a.cd_empresa = '" + cd_empresa + "'");
                                string cond = await conexao._conexao.ExecuteScalarAsync<string>(sql.ToString(), transaction: t);
                                param = new DynamicParameters();
                                param.Add("@cd_clifor", cd_clifor);
                                param.Add("@nm_clifor", Nome);
                                param.Add("@condfiscal", cond);
                                param.Add("@celular", Celular);
                                sql.Clear();
                                sql.AppendLine("insert into tb_fin_clifor(cd_clifor, nm_clifor, tp_pessoa, cd_condfiscal_clifor, celular, st_registro, dt_cad, dt_alt)")
                                    .AppendLine("values(@cd_clifor, @nm_clifor, 'F', @condfiscal, @celular, 'A', getdate(), getdate())");
                                await conexao._conexao.ExecuteAsync(sql.ToString(), param, transaction: t);
                                //Gravar na PF
                                sql.Clear();
                                sql.AppendLine("insert into tb_fin_clifor_pf(cd_clifor, dt_cad, dt_alt)values('" + cd_clifor + "', getdate(), getdate())");
                                await conexao._conexao.ExecuteAsync(sql.ToString(), transaction: t);
                            }
                            //Abrir cartão
                            param = new DynamicParameters();
                            param.Add("@P_CD_EMPRESA", cd_empresa);
                            param.Add("@P_ID_CARTAO", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                            param.Add("@P_CD_CLIFOR", cd_clifor);
                            param.Add("@P_ID_MESA", null);
                            param.Add("@P_ID_LOCAL", null);
                            param.Add("@P_NR_CARTAO", Nr_cartao);
                            param.Add("@P_DT_ABERTURA", DateTime.Now);
                            param.Add("@P_DT_FECHAMENTO", null);
                            param.Add("@P_ST_MENORIDADE", MenorIdade ? "S" : "N");
                            param.Add("@P_VL_LIMITECARTAO", 0);
                            param.Add("@P_ST_REGISTRO", "A");
                            await conexao._conexao.ExecuteAsync("IA_RES_CARTAO", param, commandType: CommandType.StoredProcedure, transaction: t);
                            decimal id_cartao = param.Get<decimal>("@P_ID_CARTAO");
                            //Criar pre venda
                            param = new DynamicParameters();
                            param.Add("@P_CD_EMPRESA", cd_empresa);
                            param.Add("@P_ID_CARTAO", id_cartao);
                            param.Add("@P_ID_PREVENDA", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                            param.Add("@P_DT_VENDA", DateTime.Now);
                            param.Add("@P_ST_DELIVERY", null);
                            param.Add("@P_DT_SAIUENTREGA", null);
                            param.Add("@P_ST_REGISTRO", "A");
                            param.Add("@P_OBSFECHARDELIVERY", null);
                            param.Add("@P_NR_SENHAFASTFOOD", null);
                            param.Add("@P_ST_LEVARMAQCARTAO", null);
                            param.Add("@P_VL_TROCOPARA", null);
                            param.Add("@P_CD_ENTREGADOR", null);
                            param.Add("@P_ST_CLIENTERETIRA", null);
                            param.Add("@P_DT_ENTREGADELIVERY", null);
                            param.Add("@P_LOGINVENDA", null);
                            param.Add("@P_LOGINCANC", null);
                            param.Add("@P_MOTIVOCANC", null);
                            param.Add("@P_HR_CLIENTERETIRA", null);
                            param.Add("@P_RECEBERRETIRADA", null);
                            param.Add("@P_VL_TAXAENTREGA", null);
                            param.Add("@P_ID_CAIXA", null);
                            param.Add("@P_IDIFOOD", null);
                            await conexao._conexao.ExecuteAsync("IA_RES_PREVENDA", param, commandType: CommandType.StoredProcedure, transaction: t);
                            t.Commit();
                            return true;
                        }
                    }
                    else throw new Exception("Erro abrir conexão banco dados.");
                }
            }
            catch(Exception ex) 
            {
                if (t != null)
                    t.Dispose();
                throw new Exception(ex.Message.Trim()); 
            }
        }

        public async Task<bool> ConsultarCartaoAbertoAsync(string token, int Nr_cartao)
        {
            try
            {
                string cnpj = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select 1")
                    .AppendLine("from TB_RES_Cartao a")
                    .AppendLine("inner join VTB_DIV_Empresa b")
                    .AppendLine("on a.cd_empresa = b.CD_Empresa")
                    .AppendLine("where a.nr_cartao = " + Nr_cartao.ToString())
                    .AppendLine("and a.st_registro = 'A'")
                    .AppendLine("and dbo.FVALIDA_NUMEROS(b.NR_CGC) = '" + cnpj.SoNumero() + "'");
                using (TConexao conexao = new TConexao(_config.GetConnectionString(cnpj.SoNumero())))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        var ret = await conexao._conexao.ExecuteScalarAsync<int>(sql.ToString());
                        if (ret > 0)
                        {
                            //Verificar se cartão não foi movimentado
                            sql.Clear();
                            sql.AppendLine("select 1")
                                .AppendLine("from VTB_RES_Cartao a")
                                .AppendLine("inner join VTB_DIV_Empresa b")
                                .AppendLine("on a.cd_empresa = b.CD_Empresa")
                                .AppendLine("where a.nr_cartao = " + Nr_cartao.ToString())
                                .AppendLine("and a.st_registro = 'A'")
                                .AppendLine("and isnull(a.valor_cartao, 0) > 0")
                                .AppendLine("and dbo.FVALIDA_NUMEROS(b.NR_CGC) = '" + cnpj.SoNumero() + "'");
                            ret = await conexao._conexao.ExecuteScalarAsync<int>(sql.ToString());
                            if (ret > 0)
                                return false;
                            else
                            {
                                //Fechar cartão sem movimento
                                sql.Clear();
                                sql.AppendLine("update TB_RES_Cartao set ")
                                    .AppendLine("ST_Registro = 'F', DT_Fechamento = GETDATE(), DT_Alt = GETDATE()")
                                    .AppendLine("from TB_RES_Cartao a")
                                    .AppendLine("inner join VTB_DIV_Empresa b")
                                    .AppendLine("on a.cd_empresa = b.CD_Empresa")
                                    .AppendLine("where a.nr_cartao = " + Nr_cartao.ToString())
                                    .AppendLine("and a.st_registro = 'A'")
                                    .AppendLine("and dbo.FVALIDA_NUMEROS(b.NR_CGC) = '" + cnpj.SoNumero() + "'");
                                await conexao._conexao.ExecuteAsync(sql.ToString());
                                return true;//Cartão liberado
                            }
                        }
                        else return true;//Cartão liberado
                    }
                    else throw new Exception("Erro abrir conexão banco dados.");
                }
            }
            catch(Exception ex) { throw new Exception(ex.Message.Trim()); }
        }

        public async Task<string> ConsultaClienteAsync(string token, string Celular)
        {
            try
            {
                string cnpj = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select a.NM_Clifor")
                    .AppendLine("from TB_FIN_Clifor a")
                    .AppendLine("where dbo.FVALIDA_NUMEROS(a.celular) like '%" + Celular.SoNumero() + "'");
                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)).SoNumero())))
                {
                    if (await conexao.OpenConnectionAsync())
                        return await conexao._conexao.ExecuteScalarAsync<string>(sql.ToString());
                    else return string.Empty;
                }
            }
            catch { return string.Empty; }
        }
    }
}
