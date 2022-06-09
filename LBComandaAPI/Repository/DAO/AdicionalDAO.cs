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
    public class AdicionalDAO : IAdicional
    {
        readonly IConfiguration _config;
        public AdicionalDAO(IConfiguration config) { _config = config; }

        public async Task<IEnumerable<Adicional>> GetAsync(string token, string Cd_produto)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select ISNULL(a.CD_ProdutoAdicional, c.CD_Produto) as CD_Adicional,");
                sql.AppendLine("ISNULL(d.DS_Produto, c.DS_Produto) as Ds_adicional,");
                sql.AppendLine("PrecoVenda = dbo.F_PRECO_VENDA(cfg.cd_empresa, ISNULL(a.CD_ProdutoAdicional, c.CD_Produto), cfg.CD_TabelaPreco)");
                sql.AppendLine("from TB_RES_Adicionais a");
                sql.AppendLine("inner join TB_EST_Produto b");
                sql.AppendLine("on a.CD_GrupoProduto = b.CD_Grupo");
                sql.AppendLine("and isnull(b.st_registro, 'A') <> 'C'");
                sql.AppendLine("left outer join TB_EST_Produto c");
                sql.AppendLine("on a.CD_GrupoAdicional = c.CD_Grupo");
                sql.AppendLine("and isnull(c.st_registro, 'A') <> 'C'");
                sql.AppendLine("left outer join TB_EST_Produto d");
                sql.AppendLine("on a.CD_ProdutoAdicional = d.CD_Produto");
                sql.AppendLine("outer apply");
                sql.AppendLine("(");
                sql.AppendLine("	select top 1 x.CD_Empresa, x.CD_TabelaPreco");
                sql.AppendLine("	from TB_RES_Config x");
                sql.AppendLine(") as cfg");
                sql.AppendLine("where b.CD_Produto = '" + Cd_produto.Trim() + "'");

                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        var ret = await conexao._conexao.QueryAsync<Adicional>(sql.ToString());
                        return ret;
                    }
                    else return null;
                }
            }
            catch { return null; }
        }
    }
}
