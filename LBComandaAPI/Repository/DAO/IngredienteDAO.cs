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
    public class IngredienteDAO : IIngrediente
    {
        readonly IConfiguration _config;
        public IngredienteDAO(IConfiguration config) { _config = config; }

        public async Task<IEnumerable<Ingredientes>> GetAsync(string token, string Cd_produto)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select a.CD_Produto, a.CD_Item,");
                sql.AppendLine("b.DS_Produto as DS_Item, a.Obrigatorio");
                sql.AppendLine("from TB_EST_FichaTecProduto a");
                sql.AppendLine("inner join TB_EST_Produto b");
                sql.AppendLine("on a.CD_Item = b.CD_Produto");
                sql.AppendLine("where a.CD_Produto = '" + Cd_produto.Trim() + "'");

                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        var ret = await conexao._conexao.QueryAsync<Ingredientes>(sql.ToString());
                        return ret;
                    }
                    else return null;
                }
            }
            catch { return null; }
        }
    }
}
