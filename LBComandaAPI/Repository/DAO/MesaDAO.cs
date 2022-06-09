using Dapper;
using LBComandaAPI.DataBase;
using LBComandaAPI.Models;
using LBComandaAPI.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.DAO
{
    public class MesaDAO : IMesa
    {
        readonly IConfiguration _config;
        public MesaDAO(IConfiguration config) { _config = config; }

        public async Task<IEnumerable<Mesa>> GetAsync(string token)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select a.ID_Mesa, a.NR_Mesa, a.ID_Local, b.DS_Local, ");
                sql.AppendLine("PossuiVenda = isnull((select top 1 1 ");
                sql.AppendLine("			            from TB_RES_PreVenda x ");
                sql.AppendLine("			            inner join TB_RES_Cartao y ");
                sql.AppendLine("			            on x.CD_Empresa = y.CD_Empresa ");
                sql.AppendLine("			            and x.ID_Cartao = y.ID_Cartao ");
                sql.AppendLine("			            where y.ID_Local = a.ID_Local ");
                sql.AppendLine("			            and y.ID_Mesa = a.ID_Mesa ");
                sql.AppendLine("			            and ISNULL(y.ST_Registro, 'A') = 'A'), 0)");
                sql.AppendLine("from TB_RES_Mesa a ");
                sql.AppendLine("inner join TB_RES_Local b ");
                sql.AppendLine("on a.ID_Local = b.ID_Local ");
                sql.AppendLine("and a.Cancelado = 0 ");
                sql.AppendLine("and b.Cancelado = 0");

                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                        return await conexao._conexao.QueryAsync<Mesa>(sql.ToString());
                    else return null;
                }
            }
            catch { return null; }
        }
        public async Task<IEnumerable<Local>> GetLocalAsync(string token)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select a.ID_Local, a.DS_Local");
                sql.AppendLine("from TB_RES_Local a ");
                sql.AppendLine("and a.Cancelado = 0 ");

                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                        return await conexao._conexao.QueryAsync<Local>(sql.ToString());
                    else return null;
                }
            }
            catch { return null; }
        }
    }
}
