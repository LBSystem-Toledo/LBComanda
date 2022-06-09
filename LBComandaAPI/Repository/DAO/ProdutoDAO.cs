using Dapper;
using LBComandaAPI.DataBase;
using LBComandaAPI.Models;
using LBComandaAPI.Repository.Interfaces;
using LBComandaAPI.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.DAO
{
    public class ProdutoDAO : IProduto
    {
        readonly IConfiguration _config;
        public ProdutoDAO(IConfiguration config) { _config = config; }

        public async Task<IEnumerable<Produto>> GetAsync(string token)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select a.CD_Produto, a.DS_Produto, a.CD_Grupo, a.QT_CombSabores as Max_sabor,")
                    .AppendLine("b.DS_Grupo, b.CD_Grupo_Pai, c.DS_Grupo as DS_Grupo_Pai, b.PontoCarne,")
                    .AppendLine("PrecoVenda = dbo.F_PRECO_VENDA(cfg.CD_Empresa, a.CD_Produto, cfg.CD_TabelaPreco),")
                    .AppendLine("Bloqueado = isnull((select top 1 1")
                    .AppendLine("from tb_res_produtobloqueado x ")
                    .AppendLine("inner join vtb_div_empresa y ")
                    .AppendLine("on x.cd_empresa = y.cd_empresa ")
                    .AppendLine("where x.cd_produto = a.cd_produto ")
                    .AppendLine("and x.bloqueado = 1 ")
                    .AppendLine("and dbo.FVALIDA_NUMEROS(y.nr_cgc) = '" + Encoding.UTF8.GetString(Convert.FromBase64String(token)).SoNumero() + "'), 0),")
                    .AppendLine("Adicional = isnull((select top 1 1")
                    .AppendLine("from TB_RES_Adicionais x")
                    .AppendLine("inner join TB_EST_Produto y")
                    .AppendLine("on x.CD_GrupoProduto = y.CD_Grupo")
                    .AppendLine("and isnull(y.st_registro, 'A') <> 'C'")
                    .AppendLine("left outer join TB_EST_Produto z")
                    .AppendLine("on x.CD_GrupoAdicional = z.CD_Grupo")
                    .AppendLine("and isnull(z.st_registro, 'A') <> 'C'")
                    .AppendLine("left outer join TB_EST_Produto w")
                    .AppendLine("on x.CD_ProdutoAdicional = w.CD_Produto")
                    .AppendLine("where y.CD_Produto = a.CD_Produto), 0),")
                    .AppendLine("Ingrediente = isnull((select top 1 1")
                    .AppendLine("from TB_EST_FichaTecProduto x")
                    .AppendLine("inner join TB_EST_Produto y")
                    .AppendLine("on x.CD_Item = y.CD_Produto")
                    .AppendLine("where x.CD_Produto = a.cd_produto), 0),")
                    .AppendLine("Sabor = isnull((select top 1 1")
                    .AppendLine("from TB_RES_Sabores x")
                    .AppendLine("inner join TB_EST_Produto y")
                    .AppendLine("on x.CD_Grupo = y.CD_Grupo")
                    .AppendLine("where y.CD_Produto = a.CD_Produto), 0),")
                    .AppendLine("Obs = isnull((select top 1 1")
                    .AppendLine("from TB_RES_Obs_X_Grupo x")
                    .AppendLine("inner join TB_EST_Produto y")
                    .AppendLine("on x.CD_Grupo = y.CD_Grupo")
                    .AppendLine("where y.CD_Produto = a.CD_Produto), 0)")
                    .AppendLine("from TB_EST_Produto a")
                    .AppendLine("inner join TB_EST_GrupoProduto b")
                    .AppendLine("on a.CD_Grupo = b.CD_Grupo")
                    .AppendLine("and isnull(a.st_registro, 'A') <> 'C'")
                    .AppendLine("and ISNULL(b.Mobile, 0) = 1")
                    .AppendLine("inner join TB_EST_GrupoProduto c")
                    .AppendLine("on b.CD_Grupo_Pai = c.CD_Grupo")
                    .AppendLine("inner join TB_EST_TpProduto d ")
                    .AppendLine("on a.TP_Produto = d.TP_Produto ")
                    .AppendLine("and ISNULL(d.ST_MPrima, 'N') <> 'S' ")
                    .AppendLine("outer apply")
                    .AppendLine("(")
                    .AppendLine("	select top 1 x.CD_Empresa, x.CD_TabelaPreco")
                    .AppendLine("	from TB_RES_Config x")
                    .AppendLine(") as cfg");

                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        var ret = await conexao._conexao.QueryAsync<Produto>(sql.ToString());
                        return ret;
                    }
                    else return null;
                }
            }
            catch { return null; }
        }

        public async Task<IEnumerable<Grupo>> GetGruposAsync(string token, string ds_produto)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select distinct a.CD_Grupo, b.DS_Grupo")
                    .AppendLine("from TB_EST_Produto a")
                    .AppendLine("inner join TB_EST_GrupoProduto b")
                    .AppendLine("on a.CD_Grupo = b.CD_Grupo")
                    .AppendLine("and isnull(a.st_registro, 'A') <> 'C'")
                    .AppendLine("and ISNULL(b.Mobile, 0) = 1")
                    .AppendLine("inner join TB_EST_GrupoProduto c")
                    .AppendLine("on b.CD_Grupo_Pai = c.CD_Grupo")
                    .AppendLine("inner join TB_EST_TpProduto d ")
                    .AppendLine("on a.TP_Produto = d.TP_Produto ")
                    .AppendLine("and ISNULL(d.ST_MPrima, 'N') <> 'S' ");
                if (!string.IsNullOrWhiteSpace(ds_produto))
                    sql.AppendLine("and a.ds_produto like '%" + ds_produto.Trim() + "%'");
                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        var ret = await conexao._conexao.QueryAsync<Grupo>(sql.ToString());
                        foreach(var p in ret)
                        {
                            sql = new StringBuilder();
                            sql.AppendLine("select a.CD_Produto, a.DS_Produto, a.CD_Grupo, a.QT_CombSabores as Max_sabor,")
                            .AppendLine("b.DS_Grupo, b.CD_Grupo_Pai, c.DS_Grupo as DS_Grupo_Pai, b.PontoCarne,")
                            .AppendLine("PrecoVenda = dbo.F_PRECO_VENDA(cfg.CD_Empresa, a.CD_Produto, cfg.CD_TabelaPreco),")
                            .AppendLine("Bloqueado = isnull((select top 1 1")
                            .AppendLine("from tb_res_produtobloqueado x ")
                            .AppendLine("inner join vtb_div_empresa y ")
                            .AppendLine("on x.cd_empresa = y.cd_empresa ")
                            .AppendLine("where x.cd_produto = a.cd_produto ")
                            .AppendLine("and x.bloqueado = 1 ")
                            .AppendLine("and dbo.FVALIDA_NUMEROS(y.nr_cgc) = '" + Encoding.UTF8.GetString(Convert.FromBase64String(token)).SoNumero() + "'), 0),")
                            .AppendLine("Adicional = isnull((select top 1 1")
                            .AppendLine("from TB_RES_Adicionais x")
                            .AppendLine("inner join TB_EST_Produto y")
                            .AppendLine("on x.CD_GrupoProduto = y.CD_Grupo")
                            .AppendLine("and isnull(y.st_registro, 'A') <> 'C'")
                            .AppendLine("left outer join TB_EST_Produto z")
                            .AppendLine("on x.CD_GrupoAdicional = z.CD_Grupo")
                            .AppendLine("and isnull(z.st_registro, 'A') <> 'C'")
                            .AppendLine("left outer join TB_EST_Produto w")
                            .AppendLine("on x.CD_ProdutoAdicional = w.CD_Produto")
                            .AppendLine("where y.CD_Produto = a.CD_Produto), 0),")
                            .AppendLine("Ingrediente = isnull((select top 1 1")
                            .AppendLine("from TB_EST_FichaTecProduto x")
                            .AppendLine("inner join TB_EST_Produto y")
                            .AppendLine("on x.CD_Item = y.CD_Produto")
                            .AppendLine("where x.CD_Produto = a.cd_produto), 0),")
                            .AppendLine("Sabor = isnull((select top 1 1")
                            .AppendLine("from TB_RES_Sabores x")
                            .AppendLine("inner join TB_EST_Produto y")
                            .AppendLine("on x.CD_Grupo = y.CD_Grupo")
                            .AppendLine("where y.CD_Produto = a.CD_Produto), 0),")
                            .AppendLine("Obs = isnull((select top 1 1")
                            .AppendLine("from TB_RES_Obs_X_Grupo x")
                            .AppendLine("inner join TB_EST_Produto y")
                            .AppendLine("on x.CD_Grupo = y.CD_Grupo")
                            .AppendLine("where y.CD_Produto = a.CD_Produto), 0)")
                            .AppendLine("from TB_EST_Produto a")
                            .AppendLine("inner join TB_EST_GrupoProduto b")
                            .AppendLine("on a.CD_Grupo = b.CD_Grupo")
                            .AppendLine("and isnull(a.st_registro, 'A') <> 'C'")
                            .AppendLine("inner join TB_EST_GrupoProduto c")
                            .AppendLine("on b.CD_Grupo_Pai = c.CD_Grupo")
                            .AppendLine("inner join TB_EST_TpProduto d ")
                            .AppendLine("on a.TP_Produto = d.TP_Produto ")
                            .AppendLine("and ISNULL(d.ST_MPrima, 'N') <> 'S' ")
                            .AppendLine("outer apply")
                            .AppendLine("(")
                            .AppendLine("	select top 1 x.CD_Empresa, x.CD_TabelaPreco")
                            .AppendLine("	from TB_RES_Config x")
                            .AppendLine("	inner join VTB_DIV_EMPRESA y")
                            .AppendLine("	on x.CD_Empresa = y.CD_Empresa")
                            .AppendLine("	and dbo.FVALIDA_NUMEROS(y.nr_cgc) = '" + Encoding.UTF8.GetString(Convert.FromBase64String(token)).SoNumero() + "'")
                            .AppendLine(") as cfg")
                            .AppendLine("where a.cd_grupo = '" + p.Cd_grupo.Trim() + "'");
                            if (!string.IsNullOrWhiteSpace(ds_produto))
                                sql.AppendLine("and a.ds_produto like '%" + ds_produto.Trim() + "%'");
                            p.Produtos = await conexao._conexao.QueryAsync<Produto>(sql.ToString());
                        }
                        return ret;
                    }
                    else return null;
                }
            }
            catch { return null; }
        }
    }
}
