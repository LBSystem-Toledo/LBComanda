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
    public class ObservacoesDAO: IObservacoes
    {
        readonly IConfiguration _config;
        public ObservacoesDAO(IConfiguration config) { _config = config; }

        public async Task<IEnumerable<Observacoes>> GetAsync(string token, string Cd_produto)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select a.Obs");
                sql.AppendLine("from TB_RES_Observacoes a");
                sql.AppendLine("inner join TB_RES_Obs_X_Grupo b");
                sql.AppendLine("on a.Id_obs = b.id_obs");
                sql.AppendLine("inner join TB_EST_Produto c");
                sql.AppendLine("on b.CD_Grupo = c.CD_Grupo");
                sql.AppendLine("where c.CD_Produto = '" + Cd_produto.Trim() + "'");

                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        var ret = await conexao._conexao.QueryAsync<Observacoes>(sql.ToString());
                        return ret;
                    }
                    else return null;
                }
            }
            catch { return null; }
        }
    }
}
