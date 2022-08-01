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
    public class ItemExcluirDAO : IItemExcluir
    {
        readonly IConfiguration _config;
        public ItemExcluirDAO(IConfiguration config) { _config = config; }

        public async Task<IEnumerable<ItemExcluir>> GetAsync(string token, string Cd_grupo)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select a.DS_Item");
                sql.AppendLine("from TB_RES_ItensExcluir a");
                sql.AppendLine("where a.CD_GrupoProduto = '" + Cd_grupo.Trim() + "'");

                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        var ret = await conexao._conexao.QueryAsync<ItemExcluir>(sql.ToString());
                        return ret;
                    }
                    else return null;
                }
            }
            catch { return null; }
        }
    }
}
