using Dapper;
using LBComandaAPI.DataBase;
using LBComandaAPI.Models;
using LBComandaAPI.Repository.Interfaces;
using LBComandaAPI.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.DAO
{
    public class RecVendaDAO : IRecVenda
    {
        readonly IConfiguration _config;
        public RecVendaDAO(IConfiguration config) { _config = config; }

        public async Task<bool> ReceberVendaAsync(string token, RecVenda rec)
        {
            SqlTransaction t = null;
            try
            {
                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        t = conexao._conexao.BeginTransaction(IsolationLevel.ReadCommitted);
                        //Buscar venda aberta
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("select a.cd_empresa, a.id_cartao")
                            .AppendLine("from tb_res_cartao a")
                            .AppendLine("where isnull(a.st_registro, 'A') = 'A'");
                        if (!string.IsNullOrWhiteSpace(rec.Nr_cartao.SoNumero()))
                            sql.AppendLine("and a.nr_cartao = '" + rec.Nr_cartao.SoNumero() + "'");
                        else
                            sql.AppendLine("and a.id_local = " + rec.Id_local.ToString())
                                .AppendLine("and a.id_mesa = " + rec.Id_mesa.ToString());
                        RecVenda aux = await conexao._conexao.QueryFirstOrDefaultAsync<RecVenda>(sql.ToString(), transaction: t);
                        if (aux == null)
                            throw new Exception("Comanda já se encontra FECHADA.");
                        //Gravar recebimento
                        DynamicParameters p = new DynamicParameters();
                        p.Add("@P_ID_RECEBIMENTO", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        p.Add("@P_CD_EMPRESA", aux.Cd_empresa);
                        p.Add("@P_ID_CARTAO", aux.Id_cartao);
                        p.Add("@P_CD_GARCOM", rec.Cd_garcom);
                        p.Add("@P_VALOR", rec.Valor);
                        p.Add("@P_DATA", DateTime.Now);
                        p.Add("@P_MOTIVOCANC", null);
                        p.Add("@P_D_C", rec.D_C);
                        p.Add("@P_BANDEIRA", rec.Bandeira);
                        p.Add("@P_NSU", rec.Nsu);
                        p.Add("@P_ST_REGISTRO", "A");
                        await conexao._conexao.ExecuteAsync("IA_RES_RECVENDA", p, transaction: t, commandType: CommandType.StoredProcedure);
                        //Alterar status do cartão para FECHADO
                        await conexao._conexao.ExecuteAsync("update tb_res_cartao set st_registro = 'F', dt_alt = getdate() where cd_empresa = '" + aux.Cd_empresa.Trim() + "' and id_cartao = " + aux.Id_cartao.ToString(), transaction: t);
                        t.Commit();
                        return true;
                    }
                    else return false;
                }
            }
            catch(Exception ex) { throw new Exception(ex.Message.Trim()); }
        }
    }
}
