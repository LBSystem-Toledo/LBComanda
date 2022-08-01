using Dapper;
using LBComandaAPI.DataBase;
using LBComandaAPI.Models;
using LBComandaAPI.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace LBComandaAPI.Repository.DAO
{
    public class ComandaDAO : IComanda
    {
        readonly IConfiguration _config;
        public ComandaDAO(IConfiguration config) { _config = config; }
        public async Task<IEnumerable<Comanda>> GetAsync(string token)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select g.TP_Impressora, g.Porta_Imp, c.id_cartao, i.NR_Mesa, h.DS_Local,")
                    .AppendLine("j.NM_Clifor as NM_Garcom, d.Quantidade, e.DS_Produto, d.PontoCarne, d.ObsItem,")
                    .AppendLine("d.CD_Empresa, d.ID_PreVenda, d.ID_Item, c.NR_Cartao, d.NR_MesaCartao,")
                    .AppendLine("a.ST_ClienteRetira, c.nm_clifor as ClienteBalcao,")
                    .AppendLine("case when d.ID_ItemPaiAdic is null then 0 else 1 end as Adicional")
                    .AppendLine("from TB_RES_PreVenda a")
                    .AppendLine("inner join VTB_RES_Cartao c")
                    .AppendLine("on a.CD_Empresa = c.CD_Empresa")
                    .AppendLine("and a.ID_Cartao = c.ID_Cartao")
                    .AppendLine("and ISNULL(c.ST_Registro, 'A') = 'A'")
                    .AppendLine("and ISNULL(a.ST_Registro, 'A') <> 'C'")
                    .AppendLine("inner join TB_RES_ItensPreVenda d")
                    .AppendLine("on a.CD_Empresa = d.CD_Empresa")
                    .AppendLine("and a.ID_PreVenda = d.ID_PreVenda")
                    .AppendLine("and ISNULL(d.ST_Registro, 'A') <> 'C'")
                    .AppendLine("and ISNULL(d.Impresso, 0) = 0")
                    .AppendLine("and ISNULL(d.PedidoAPP, 0) = 1")
                    .AppendLine("inner join TB_EST_Produto e")
                    .AppendLine("on d.CD_Produto = e.CD_Produto")
                    .AppendLine("inner join TB_RES_LocalImp_X_Grupo f")
                    .AppendLine("on e.CD_Grupo = f.CD_Grupo")
                    .AppendLine("inner join TB_RES_LocalImp g")
                    .AppendLine("on f.ID_LocalImp = g.ID_LocalImp")
                    .AppendLine("left join TB_RES_Local h")
                    .AppendLine("on c.ID_Local = h.ID_Local")
                    .AppendLine("left join TB_RES_Mesa i")
                    .AppendLine("on c.ID_Local = i.ID_Local")
                    .AppendLine("and c.ID_Mesa = i.ID_Mesa")
                    .AppendLine("left join TB_FIN_Clifor j")
                    .AppendLine("on d.CD_Garcon = j.CD_Clifor")
                    .AppendLine("inner join VTB_DIV_EMPRESA k")
                    .AppendLine("on a.CD_Empresa = k.CD_Empresa")
                    .AppendLine("where dbo.FVALIDA_NUMEROS(k.NR_CGC) = '" + Encoding.UTF8.GetString(Convert.FromBase64String(token)) + "'");

                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        var ret = await conexao._conexao.QueryAsync<Comanda>(sql.ToString());
                        foreach(var c in ret)
                        {
                            //Ingredientes
                            sql = new StringBuilder();
                            sql.AppendLine("select ds_ingrediente as DS_Item")
                                .AppendLine("from TB_RES_ItensPreVenda_Ingredientes ")
                                .AppendLine("where cd_empresa = '" + c.Cd_empresa.Trim() + "'")
                                .AppendLine("and id_prevenda = " + c.Id_prevenda.ToString())
                                .AppendLine("and id_item = " + c.Id_item.ToString());
                            c.Ingredientes = await conexao._conexao.QueryAsync<Ingredientes>(sql.ToString());
                            //Itens Excluir
                            sql = new StringBuilder();
                            sql.AppendLine("select DS_ItemExcluir as Ds_item")
                                .AppendLine("from TB_RES_ItensPreVenda_ItemExcluir")
                                .AppendLine("where CD_Empresa = '" + c.Cd_empresa.Trim() + "'")
                                .AppendLine("and ID_PreVenda = " + c.Id_prevenda.ToString())
                                .AppendLine("and ID_Item = " + c.Id_item.ToString());
                            c.ItensExcluir = await conexao._conexao.QueryAsync<ItemExcluir>(sql.ToString());
                            //Sabor
                            sql = new StringBuilder();
                            sql.AppendLine("select DS_Sabor")
                                .AppendLine("from TB_RES_SaboresItens")
                                .AppendLine("where CD_Empresa = '" + c.Cd_empresa.Trim() + "'")
                                .AppendLine("and ID_PreVenda = " + c.Id_prevenda.ToString())
                                .AppendLine("and ID_Item = " + c.Id_item.ToString());
                            c.Sabores = await conexao._conexao.QueryAsync<Sabor>(sql.ToString());
                            //Adicional
                            sql = new StringBuilder();
                            sql.AppendLine("select b.DS_Produto as DS_adicional")
                                .AppendLine("from TB_RES_ItensPreVenda a")
                                .AppendLine("inner join TB_EST_Produto b")
                                .AppendLine("on a.CD_Produto = b.CD_Produto")
                                .AppendLine("where a.CD_Empresa = '" + c.Cd_empresa.Trim() + "'")
                                .AppendLine("and a.ID_PreVenda = " + c.Id_prevenda.ToString())
                                .AppendLine("and a.ID_ItemPaiAdic = " + c.Id_item.ToString())
                                .AppendLine("and ISNULL(a.ST_Registro, 'A') <> 'C'");
                            c.Adicionais = await conexao._conexao.QueryAsync<Adicional>(sql.ToString());
                        }
                        return ret;
                    }
                    else return null;
                }
            }
            catch { return null; }
        }
        public async Task GravarImpressoAsync(string token, List<Comanda> Itens)
        {
            try
            {
                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        foreach (var p in Itens)
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@IMPRESSO", true);
                            param.Add("@CD_EMPRESA", p.Cd_empresa);
                            param.Add("@ID_PREVENDA", p.Id_prevenda);
                            param.Add("@ID_ITEM", p.Id_item);
                            await conexao._conexao.ExecuteAsync("update TB_RES_ItensPreVenda set Impresso = @IMPRESSO, DT_Alt = getdate() " +
                                                                "where cd_empresa = @CD_EMPRESA and id_prevenda = @ID_PREVENDA " +
                                                                "and id_item = @ID_ITEM", param: param, commandType: CommandType.Text);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
