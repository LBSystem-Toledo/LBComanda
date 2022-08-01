using Dapper;
using LBComandaAPI.DataBase;
using LBComandaAPI.Models;
using LBComandaAPI.Repository.Interfaces;
using LBComandaAPI.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.DAO
{
    public class GarcomDAO: IGarcom
    {
        readonly IConfiguration _config;
        public GarcomDAO(IConfiguration config) { _config = config; }

        public async Task<Garcom> ValidarGarcomAsync(string Login, string Senha, string Cnpj)
        {
            try
            {
                //Verificar se cnpj esta habilitado para utilizar mobile
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select 1")
                    .AppendLine("from TB_CRM_CLIENTE")
                    .AppendLine("where ISNULL(Mobile, 0) = 1")
                    .AppendLine("and ISNULL(ST_REGISTRO, 'A') <> 'I'")
                    .AppendLine("and dbo.FVALIDA_NUMEROS(CNPJ) = '" + Cnpj.SoNumero() + "'");
                bool cnpj_habilitado = false;
                using (TConexao conexao = new TConexao(_config.GetConnectionString("conexaoHelp")))
                {
                    if (await conexao.OpenConnectionAsync())
                        cnpj_habilitado = await conexao._conexao.ExecuteScalarAsync<bool>(sql.ToString());
                }
                if (cnpj_habilitado)
                {
                    sql = new StringBuilder();
                    sql.AppendLine("select a.CD_Empresa, b.NM_Empresa, a.CD_Clifor as Cd_Garcom, c.LerQRCodeAPP,")
                        .AppendLine("a.NM_Clifor as NM_Garcom, a.ExigirTokenApp, a.ST_Entregador,")
                        .AppendLine("c.tp_cartao, c.nr_cartaorotini, c.nr_cartaorotfin, c.st_mesacartao, d.Stone_id,")
                        .AppendLine("b.DS_Endereco + ', ' + b.Numero + ' - ' + b.Bairro + ' - ' + b.ds_cidade + ' - ' + b.UF as Endereco_empresa")
                        .AppendLine("from VTB_FIN_CLIFOR a")
                        .AppendLine("inner join VTB_DIV_Empresa b ")
                        .AppendLine("on a.cd_empresa = b.cd_empresa ")
                        .AppendLine("inner join TB_RES_Config c ")
                        .AppendLine("on a.cd_empresa = c.cd_empresa ")
                        .AppendLine("left join TB_DIV_CFGEmpresa d")
                        .AppendLine("on a.cd_empresa = d.cd_empresa")
                        .AppendLine("where ISNULL(a.ST_Registro, 'A') <> 'C'")
                        .AppendLine("and ISNULL(a.st_funcativo, 'S') = 'S'")
                        .AppendLine("and a.LoginGarcomApp = '" + Login.Trim() + "'")
                        .AppendLine("and a.SenhaGarcomApp = '" + Senha.Trim() + "'")
                        .AppendLine("and dbo.FVALIDA_NUMEROS(b.NR_CGC) = '" + Cnpj.SoNumero() + "'");
                    using (TConexao conexao = new TConexao(_config.GetConnectionString(Cnpj.SoNumero())))
                    {
                        if (await conexao.OpenConnectionAsync())
                        {
                            Garcom ret = await conexao._conexao.QueryFirstAsync<Garcom>(sql.ToString());
                            if(ret == null ? false : ret.ExigirTokenApp)
                            {
                                //Verificar se existe token valido
                                sql = new StringBuilder();
                                sql.AppendLine("select a.cd_empresa, a.id_token, a.dt_token, a.temp_validade")
                                    .AppendLine("from tb_res_tokenapp a ")
                                    .AppendLine("inner join vtb_div_empresa b ")
                                    .AppendLine("on a.cd_empresa = b.cd_empresa ")
                                    .AppendLine("where a.cd_garcom = '" + ret.Cd_garcom.Trim() + "'")
                                    .AppendLine("and dbo.FVALIDA_NUMEROS(b.NR_CGC) = '" + Cnpj.SoNumero() + "'")
                                    .AppendLine("and DATEADD(MINUTE, a.temp_validade, a.dt_token) > GETDATE()");
                                Token tk = await conexao._conexao.QueryFirstOrDefaultAsync<Token>(sql.ToString());
                                if (tk == null)
                                {
                                    //Verificar se não existe token aguardando liberação
                                    sql = new StringBuilder();
                                    sql.AppendLine("select 1 from tb_res_tokenapp a ")
                                        .AppendLine("where a.cd_empresa = '" + ret.Cd_empresa.Trim() + "'")
                                        .AppendLine("and a.cd_garcom = '" + ret.Cd_garcom.Trim() + "'")
                                        .AppendLine("and a.dt_token is null");
                                    if (!await conexao._conexao.ExecuteScalarAsync<bool>(sql.ToString()))
                                    {
                                        //Solicitar novo token
                                        DynamicParameters param = new DynamicParameters();
                                        param.Add("@P_CD_EMPRESA", ret.Cd_empresa);
                                        param.Add("@P_ID_TOKEN", dbType: DbType.Int32, direction: ParameterDirection.Output);
                                        param.Add("@P_CD_GARCOM", ret.Cd_garcom);
                                        param.Add("@P_LOGIN", null);
                                        param.Add("@P_DT_TOKEN", null);
                                        param.Add("@P_TEMP_VALIDADE", null);
                                        await conexao._conexao.ExecuteAsync("IA_RES_TOKENAPP", param, commandType: CommandType.StoredProcedure);
                                    }
                                }
                                else ret.Token = tk;
                            }
                            return ret;
                        }
                        else return null;
                    }
                }
                else return null;
            }
            catch { return null; }
        }
        public async Task<Token> ValidarTokenAsync(string token, string garcom)
        {
            try
            {
                string cnpj = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                //Verificar se existe token valido
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select a.cd_empresa, a.id_token, a.dt_token, a.temp_validade")
                    .AppendLine("from tb_res_tokenapp a ")
                    .AppendLine("inner join vtb_div_empresa b ")
                    .AppendLine("on a.cd_empresa = b.cd_empresa ")
                    .AppendLine("where a.cd_garcom = '" + garcom.Trim() + "'")
                    .AppendLine("and dbo.FVALIDA_NUMEROS(b.NR_CGC) = '" + cnpj.SoNumero() + "'")
                    .AppendLine("and DATEADD(MINUTE, a.temp_validade, a.dt_token) > GETDATE()");
                using (TConexao conexao = new TConexao(_config.GetConnectionString(cnpj)))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        var ret = await conexao._conexao.QueryFirstAsync<Token>(sql.ToString());
                        return ret;
                    }
                    else return null;
                }
            }
            catch { return null; }
        }
        public async Task<bool> SolicitarToken(string token, string garcom)
        {
            try
            {
                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        //Buscar codigo empresa
                        string cd_empresa = await conexao._conexao.ExecuteScalarAsync<string>("select cd_empresa from vtb_div_empresa where dbo.FVALIDA_NUMEROS(NR_CGC) = '" + Encoding.UTF8.GetString(Convert.FromBase64String(token)) + "'");
                        //Solicitar novo token
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_CD_EMPRESA", cd_empresa);
                        param.Add("@P_ID_TOKEN", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        param.Add("@P_CD_GARCOM", garcom);
                        param.Add("@P_LOGIN", null);
                        param.Add("@P_DT_TOKEN", null);
                        param.Add("@P_TEMP_VALIDADE", null);
                        int i = await conexao._conexao.ExecuteAsync("IA_RES_TOKENAPP", param, commandType: CommandType.StoredProcedure);
                        return i > 0;
                    }
                    else return false;
                }
            }
            catch { return false; }
        }
    }
}
