using Dapper;
using LBComandaAPI.DataBase;
using LBComandaAPI.Models;
using LBComandaAPI.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.DAO
{
    public class SaborDAO : ISabor
    {
        readonly IConfiguration _config;
        public SaborDAO(IConfiguration config) { _config = config; }

        public async Task<IEnumerable<Sabor>> GetAsync(string token, string Cd_produto)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select a.DS_Sabor ");
                sql.AppendLine("from TB_RES_Sabores a");
                sql.AppendLine("where exists(select 1 from tb_est_produto x ");
                sql.AppendLine("                where x.cd_grupo = a.cd_grupo ");
                sql.AppendLine("                and x.CD_Produto = '" + Cd_produto.Trim() + "')");

                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        var ret = await conexao._conexao.QueryAsync<Sabor>(sql.ToString());
                        return ret;
                    }
                    else return null;
                }
            }
            catch { return null; }
        }
    }
}
