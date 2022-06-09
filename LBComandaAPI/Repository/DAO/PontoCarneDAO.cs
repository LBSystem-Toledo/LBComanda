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
    public class PontoCarneDAO : IPontoCarne
    {
        readonly IConfiguration _config;
        public PontoCarneDAO(IConfiguration config) { _config = config; }

        public async Task<IEnumerable<PontoCarne>> GetAsync(string token, string Cd_produto)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select a.DS_Ponto");
                sql.AppendLine("from TB_RES_PontoCarne a");
                sql.AppendLine("where exists(select 1 from TB_EST_GrupoProduto x");
                sql.AppendLine("				inner join TB_EST_Produto y");
                sql.AppendLine("				on x.CD_Grupo = y.CD_Grupo");
                sql.AppendLine("				and isnull(x.PontoCarne, 0) = 1");
                sql.AppendLine("				and y.CD_Produto = '" + Cd_produto.Trim() + "')");

                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        var ret = await conexao._conexao.QueryAsync<PontoCarne>(sql.ToString());
                        return ret;
                    }
                    else return null;
                }
            }
            catch { return null; }
        }
    }
}
