using Dapper;
using LBComandaAPI.DataBase;
using LBComandaAPI.Models;
using LBComandaAPI.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using LBComandaAPI.Utils;
using System.Linq;

namespace LBComandaAPI.Repository.DAO
{
    public class ItemVendaDAO : IItemVenda
    {
        readonly IConfiguration _config;
        public ItemVendaDAO(IConfiguration config) { _config = config; }

        public async Task<IEnumerable<ItemVenda>> GetAsync(string token, string Id_local, string Id_mesa, string Nr_cartao)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select a.cd_produto, b.DS_Produto, a.Vl_Unitario as PrecoVenda,");
                sql.AppendLine("a.Quantidade - a.qtd_faturada as Quantidade");
                sql.AppendLine("from vtb_res_itensprevenda a ");
                sql.AppendLine("inner join TB_EST_Produto b");
                sql.AppendLine("on a.CD_Produto = b.CD_Produto");
                sql.AppendLine("and isnull(a.st_registro, 'A') <> 'C'");
                sql.AppendLine("inner join TB_RES_PreVenda c");
                sql.AppendLine("on a.cd_empresa = c.CD_Empresa");
                sql.AppendLine("and a.ID_PreVenda = c.ID_PreVenda");
                sql.AppendLine("inner join TB_RES_Cartao d");
                sql.AppendLine("on c.CD_Empresa = d.CD_Empresa");
                sql.AppendLine("and c.ID_Cartao = d.ID_Cartao");
                sql.AppendLine("and ISNULL(d.ST_Registro, 'A') = 'A'");
                if (!string.IsNullOrWhiteSpace(Nr_cartao.SoNumero()))
                    sql.AppendLine("where d.nr_cartao = '" + Nr_cartao.Trim() + "'");
                else
                {
                    sql.AppendLine("where d.ID_Local = " + Id_local);
                    sql.AppendLine("and d.ID_Mesa = " + Id_mesa);
                }

                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        var ret = await conexao._conexao.QueryAsync<ItemVenda>(sql.ToString());
                        return ret;
                    }
                    else return null;
                }
            }
            catch { return null; }
        }
        
        public async Task<bool> GravarItensAsync(string token, string Id_local, string Id_mesa, List<ItemVenda> items)
        {
            SqlTransaction t = null;
            try
            {
                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        t = conexao._conexao.BeginTransaction(IsolationLevel.ReadCommitted);
                        //Buscar config
                        Config cfg = await conexao._conexao.QueryFirstAsync<Config>("select a.cd_empresa, a.cd_clifor from TB_RES_Config a " +
                                                                                    "inner join VTB_DIV_Empresa b " +
                                                                                    "on a.cd_empresa = b.cd_empresa " +
                                                                                    "and dbo.FVALIDA_NUMEROS(b.NR_CGC) = '" + Encoding.UTF8.GetString(Convert.FromBase64String(token)) + "'", transaction: t);
                        //Buscar venda aberta
                        decimal id_prevenda = 
                        await conexao._conexao.ExecuteScalarAsync<decimal>("select a.id_prevenda " +
                                                                           "from VTB_RES_PREVENDA a " +
                                                                           "inner join TB_RES_Cartao b " +
                                                                           "on a.cd_empresa = b.cd_empresa " +
                                                                           "and a.id_cartao = b.id_cartao " +
                                                                           "where b.id_local = " + Id_local + " " +
                                                                           "and b.id_mesa = " + Id_mesa + " " +
                                                                           "and isnull(b.st_registro, 'A') = 'A'", transaction: t);
                        DynamicParameters param;
                        if (id_prevenda <= 0)
                        {
                            //Novo Cartão
                            param = new DynamicParameters();
                            param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                            param.Add("@P_ID_CARTAO", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                            param.Add("@P_CD_CLIFOR", cfg.Cd_clifor);
                            param.Add("@P_NM_CLIFOR", null);
                            param.Add("@P_ID_MESA", Id_mesa);
                            param.Add("@P_ID_LOCAL", Id_local);
                            param.Add("@P_NR_CARTAO", null);
                            param.Add("@P_DT_ABERTURA", DateTime.Now);
                            param.Add("@P_DT_FECHAMENTO", null);
                            param.Add("@P_ST_MENORIDADE", "N");
                            param.Add("@P_VL_LIMITECARTAO", decimal.Zero);
                            param.Add("@P_VALEEVENTO", false);
                            param.Add("@P_ST_REGISTRO", "A");
                            int ret = await conexao._conexao.ExecuteAsync("IA_RES_CARTAO", param, transaction: t, commandType: CommandType.StoredProcedure);
                            if (ret > 0)
                            {
                                //Pegar ID Cartão
                                decimal id_cartao = param.Get<decimal>("@P_ID_CARTAO");
                                //Nova Venda
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                param.Add("@P_ID_CARTAO", id_cartao);
                                param.Add("@P_CD_ENTREGADOR", null);
                                param.Add("@P_LOGINCANC", null);
                                param.Add("@P_LOGINVENDA", null);
                                param.Add("@P_ID_CAIXA", null);
                                param.Add("@P_DT_VENDA", DateTime.Now);
                                param.Add("@P_ST_DELIVERY", null);
                                param.Add("@P_NR_SENHAFASTFOOD", null);
                                param.Add("@P_DT_ENTREGADELIVERY", null);
                                param.Add("@P_ST_LEVARMAQCARTAO", null);
                                param.Add("@P_OBSFECHARDELIVERY", null);
                                param.Add("@P_VL_TROCOPARA", decimal.Zero);
                                param.Add("@P_ST_CLIENTERETIRA", null);
                                param.Add("@P_HR_CLIENTERETIRA", null);
                                param.Add("@P_MOTIVOCANC", null);
                                param.Add("@P_IDIFOOD", null);
                                param.Add("@P_DT_SAIUENTREGA", null);
                                param.Add("@P_RECEBERRETIRADA", null);
                                param.Add("@P_TROCABALCAOMESA", null);
                                param.Add("@P_ST_REGISTRO", "A");
                                ret = await conexao._conexao.ExecuteAsync("IA_RES_PREVENDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                                if (ret > 0)
                                    id_prevenda = param.Get<decimal>("@P_ID_PREVENDA");
                            }
                            else return false;
                        }
                        foreach(var p in items)
                        {
                            //Gravar Item
                            param = new DynamicParameters();
                            param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                            param.Add("@P_ID_PREVENDA", id_prevenda);
                            param.Add("@P_ID_ITEM", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                            param.Add("@P_CD_PRODUTO", p.Cd_produto);
                            param.Add("@P_ID_ITEMPAIADIC", null);
                            param.Add("@P_LOGINCANC", null);
                            param.Add("@P_CD_GARCON", p.Cd_garcom);
                            param.Add("@P_QUANTIDADE", p.Quantidade);
                            param.Add("@P_VL_UNITARIO", p.PrecoVenda);
                            param.Add("@P_VL_DESCONTO", decimal.Zero);
                            param.Add("@P_VL_ACRESCIMO", decimal.Zero);
                            param.Add("@P_VL_FRETE", decimal.Zero);
                            param.Add("@P_OBSITEM", p.Obs);
                            param.Add("@P_IMPRESSO", false);
                            param.Add("@P_PONTOCARNE", p.PontoCarne);
                            param.Add("@P_PEDIDOAPP", true);
                            param.Add("@P_NR_MESACARTAO", null);
                            param.Add("@P_ST_REGISTRO", "A");
                            param.Add("@P_MOTIVOCANC", null);
                            await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                            p.Cd_empresa = cfg.Cd_empresa;
                            p.Id_prevenda = id_prevenda;
                            p.Id_item = param.Get<decimal>("@P_ID_ITEM");
                            //Adicional
                            foreach(var v in p.Adicionais)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                param.Add("@P_CD_PRODUTO", v.Cd_adicional);
                                param.Add("@P_ID_ITEMPAIADIC", p.Id_item);
                                param.Add("@P_LOGINCANC", null);
                                param.Add("@P_CD_GARCON", p.Cd_garcom);
                                param.Add("@P_QUANTIDADE", 1);
                                param.Add("@P_VL_UNITARIO", v.PrecoVenda);
                                param.Add("@P_VL_DESCONTO", decimal.Zero);
                                param.Add("@P_VL_ACRESCIMO", decimal.Zero);
                                param.Add("@P_VL_FRETE", decimal.Zero);
                                param.Add("@P_OBSITEM", null);
                                param.Add("@P_IMPRESSO", false);
                                param.Add("@P_PONTOCARNE", null);
                                param.Add("@P_ST_REGISTRO", "A");
                                param.Add("@P_MOTIVOCANC", null);
                                param.Add("@P_PEDIDOAPP", true);
                                param.Add("@P_NR_MESACARTAO", null);
                                await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                            //Ingredientes
                            foreach(var v in p.Ingredientes)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", p.Id_item);
                                param.Add("@P_ID_INGREDIENTE", null);
                                param.Add("@P_DS_INGREDIENTE", v.Ds_item);
                                await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_INGREDIENTES", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                            //Ingredientes Excluir
                            foreach(var v in p.IngredientesDel)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", p.Id_item);
                                param.Add("@P_ID_EXCLUIR", null);
                                param.Add("@P_DS_ITEMEXCLUIR", v.Ds_item);
                                await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_ITEMEXCLUIR", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                            //Itens Excluir
                            foreach(var v in p.ItensExcluir)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", p.Id_item);
                                param.Add("@P_ID_EXCLUIR", null);
                                param.Add("@P_DS_ITEMEXCLUIR", v.Ds_item);
                                await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_ITEMEXCLUIR", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                            //Sabores
                            foreach(var v in p.Sabores)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", p.Id_item);
                                param.Add("@P_ID_SABOR", null);
                                param.Add("@P_DS_SABOR", v.Ds_sabor);
                                await conexao._conexao.ExecuteAsync("IA_RES_SABORESITENS", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                        }
                        t.Commit();
                        return true;
                    }
                    else return false;
                }
            }
            catch
            {
                if (t != null)
                    t.Dispose();
                return false;
            }
        }
        public async Task<bool> GravarItensAsync(string token, string Nr_cartao, List<ItemVenda> items)
        {
            SqlTransaction t = null;
            try
            {
                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        t = conexao._conexao.BeginTransaction(IsolationLevel.ReadCommitted);
                        //Buscar config
                        Config cfg = await conexao._conexao.QueryFirstAsync<Config>("select a.cd_empresa, a.cd_clifor, a.st_abrircartao " +
                                                                                    "from TB_RES_Config a " +
                                                                                    "inner join VTB_DIV_Empresa b " +
                                                                                    "on a.cd_empresa = b.cd_empresa " +
                                                                                    "and dbo.FVALIDA_NUMEROS(b.NR_CGC) = '" + Encoding.UTF8.GetString(Convert.FromBase64String(token)) + "'", transaction: t);
                        //Buscar venda aberta
                        decimal id_prevenda =
                        await conexao._conexao.ExecuteScalarAsync<decimal>("select a.id_prevenda " +
                                                                           "from VTB_RES_PREVENDA a " +
                                                                           "inner join TB_RES_Cartao b " +
                                                                           "on a.cd_empresa = b.cd_empresa " +
                                                                           "and a.id_cartao = b.id_cartao " +
                                                                           "where b.nr_cartao = " + Nr_cartao + " " +
                                                                           "and isnull(b.st_registro, 'A') = 'A'", transaction: t);
                        DynamicParameters param;
                        if (id_prevenda <= 0)
                        {
                            if (!cfg.St_abrircartao)
                                throw new Exception("Não é permitido fazer pedido para cartão FECHADO.");
                            //Novo Cartão
                            param = new DynamicParameters();
                            param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                            param.Add("@P_ID_CARTAO", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                            param.Add("@P_CD_CLIFOR", cfg.Cd_clifor);
                            param.Add("@P_NM_CLIFOR", null);
                            param.Add("@P_ID_MESA", null);
                            param.Add("@P_ID_LOCAL", null);
                            param.Add("@P_NR_CARTAO", Nr_cartao);
                            param.Add("@P_DT_ABERTURA", DateTime.Now);
                            param.Add("@P_DT_FECHAMENTO", null);
                            param.Add("@P_ST_MENORIDADE", "N");
                            param.Add("@P_VL_LIMITECARTAO", decimal.Zero);
                            param.Add("@P_VALEEVENTO", false);
                            param.Add("@P_ST_REGISTRO", "A");
                            int ret = await conexao._conexao.ExecuteAsync("IA_RES_CARTAO", param, transaction: t, commandType: CommandType.StoredProcedure);
                            if (ret > 0)
                            {
                                //Pegar ID Cartão
                                decimal id_cartao = param.Get<decimal>("@P_ID_CARTAO");
                                //Nova Venda
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                param.Add("@P_ID_CARTAO", id_cartao);
                                param.Add("@P_CD_ENTREGADOR", null);
                                param.Add("@P_LOGINCANC", null);
                                param.Add("@P_LOGINVENDA", null);
                                param.Add("@P_ID_CAIXA", null);
                                param.Add("@P_DT_VENDA", DateTime.Now);
                                param.Add("@P_ST_DELIVERY", null);
                                param.Add("@P_NR_SENHAFASTFOOD", null);
                                param.Add("@P_DT_ENTREGADELIVERY", null);
                                param.Add("@P_ST_LEVARMAQCARTAO", null);
                                param.Add("@P_OBSFECHARDELIVERY", null);
                                param.Add("@P_VL_TROCOPARA", decimal.Zero);
                                param.Add("@P_ST_CLIENTERETIRA", null);
                                param.Add("@P_HR_CLIENTERETIRA", null);
                                param.Add("@P_MOTIVOCANC", null);
                                param.Add("@P_IDIFOOD", null);
                                param.Add("@P_DT_SAIUENTREGA", null);
                                param.Add("@P_RECEBERRETIRADA", null);
                                param.Add("@P_TROCABALCAOMESA", null);
                                param.Add("@P_ST_REGISTRO", "A");
                                ret = await conexao._conexao.ExecuteAsync("IA_RES_PREVENDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                                if (ret > 0)
                                    id_prevenda = param.Get<decimal>("@P_ID_PREVENDA");
                            }
                            else return false;
                        }
                        foreach (var p in items)
                        {
                            //Gravar Item
                            param = new DynamicParameters();
                            param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                            param.Add("@P_ID_PREVENDA", id_prevenda);
                            param.Add("@P_ID_ITEM", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                            param.Add("@P_CD_PRODUTO", p.Cd_produto);
                            param.Add("@P_ID_ITEMPAIADIC", null);
                            param.Add("@P_LOGINCANC", null);
                            param.Add("@P_CD_GARCON", p.Cd_garcom);
                            param.Add("@P_QUANTIDADE", p.Quantidade);
                            param.Add("@P_VL_UNITARIO", p.PrecoVenda);
                            param.Add("@P_VL_DESCONTO", decimal.Zero);
                            param.Add("@P_VL_ACRESCIMO", decimal.Zero);
                            param.Add("@P_VL_FRETE", decimal.Zero);
                            param.Add("@P_OBSITEM", p.Obs);
                            param.Add("@P_IMPRESSO", false);
                            param.Add("@P_PONTOCARNE", p.PontoCarne);
                            param.Add("@P_PEDIDOAPP", true);
                            param.Add("@P_NR_MESACARTAO", p.Nr_mesacartao);
                            param.Add("@P_ST_REGISTRO", "A");
                            param.Add("@P_MOTIVOCANC", null);
                            await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                            p.Cd_empresa = cfg.Cd_empresa;
                            p.Id_prevenda = id_prevenda;
                            p.Id_item = param.Get<decimal>("@P_ID_ITEM");
                            //Adicional
                            foreach (var v in p.Adicionais)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                param.Add("@P_CD_PRODUTO", v.Cd_adicional);
                                param.Add("@P_ID_ITEMPAIADIC", p.Id_item);
                                param.Add("@P_LOGINCANC", null);
                                param.Add("@P_CD_GARCON", p.Cd_garcom);
                                param.Add("@P_QUANTIDADE", 1);
                                param.Add("@P_VL_UNITARIO", v.PrecoVenda);
                                param.Add("@P_VL_DESCONTO", decimal.Zero);
                                param.Add("@P_VL_ACRESCIMO", decimal.Zero);
                                param.Add("@P_VL_FRETE", decimal.Zero);
                                param.Add("@P_OBSITEM", null);
                                param.Add("@P_IMPRESSO", false);
                                param.Add("@P_PONTOCARNE", null);
                                param.Add("@P_ST_REGISTRO", "A");
                                param.Add("@P_MOTIVOCANC", null);
                                param.Add("@P_PEDIDOAPP", true);
                                await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                            //Ingredientes
                            foreach (var v in p.Ingredientes)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", p.Id_item);
                                param.Add("@P_ID_INGREDIENTE", null);
                                param.Add("@P_DS_INGREDIENTE", v.Ds_item);
                                await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_INGREDIENTES", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                            //Ingredientes Excluir
                            foreach (var v in p.IngredientesDel)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", p.Id_item);
                                param.Add("@P_ID_EXCLUIR", null);
                                param.Add("@P_DS_ITEMEXCLUIR", v.Ds_item);
                                await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_ITEMEXCLUIR", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                            //Sabores
                            foreach (var v in p.Sabores)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", p.Id_item);
                                param.Add("@P_ID_SABOR", null);
                                param.Add("@P_DS_SABOR", v.Ds_sabor);
                                await conexao._conexao.ExecuteAsync("IA_RES_SABORESITENS", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                        }
                        t.Commit();
                        return true;
                    }
                    else return false;
                }
            }
            catch(Exception ex)
            {
                if (t != null)
                    t.Dispose();
                throw new Exception(ex.Message.Trim());
            }
        }
        public async Task<bool> GravarItensAsync(string token, List<ItemVenda> items)
        {
            SqlTransaction t = null;
            try
            {
                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        t = conexao._conexao.BeginTransaction(IsolationLevel.ReadCommitted);
                        //Buscar caixa aberto
                        decimal id_caixa = await conexao._conexao.ExecuteScalarAsync<decimal>("select top 1 a.ID_Caixa " +
                                                                                              "from TB_PDV_Caixa a " +
                                                                                              "inner join VTB_DIV_EMPRESA b " +
                                                                                              "on a.CD_Empresa = b.CD_Empresa " +
                                                                                              "where ISNULL(a.ST_Registro, 'A') = 'A' " +
                                                                                              "and dbo.FVALIDA_NUMEROS(b.NR_CGC) = '" + Encoding.UTF8.GetString(Convert.FromBase64String(token)) + "' " +
                                                                                              "order by a.DT_Abertura desc", transaction: t, commandType: CommandType.Text);
                        if (id_caixa > decimal.Zero)
                        {
                            //Buscar config
                            Config cfg = await conexao._conexao.QueryFirstAsync<Config>("select a.cd_empresa, a.cd_clifor, a.CD_TabelaPreco, " +
                                                                                        "a.cd_portadordinheiro as cd_portador, a.CD_Local, " +
                                                                                        "c.CD_ContaOperacional, c.cd_historico " +
                                                                                        "from TB_RES_Config a " +
                                                                                        "inner join VTB_DIV_Empresa b " +
                                                                                        "on a.cd_empresa = b.cd_empresa " +
                                                                                        "left join TB_PDV_CFGCupomFiscal c " +
                                                                                        "on a.cd_empresa = c.cd_empresa " +
                                                                                        "and dbo.FVALIDA_NUMEROS(b.NR_CGC) = '" + Encoding.UTF8.GetString(Convert.FromBase64String(token)) + "'", transaction: t);
                            DynamicParameters param;
                            //Novo Cartão
                            param = new DynamicParameters();
                            param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                            param.Add("@P_ID_CARTAO", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                            param.Add("@P_CD_CLIFOR", cfg.Cd_clifor);
                            param.Add("@P_NM_CLIFOR", string.Empty);
                            param.Add("@P_ID_MESA", null);
                            param.Add("@P_ID_LOCAL", null);
                            param.Add("@P_NR_CARTAO", null);
                            param.Add("@P_DT_ABERTURA", DateTime.Now);
                            param.Add("@P_DT_FECHAMENTO", null);
                            param.Add("@P_ST_MENORIDADE", "N");
                            param.Add("@P_VL_LIMITECARTAO", decimal.Zero);
                            param.Add("@P_VALEEVENTO", true);
                            param.Add("@P_ST_REGISTRO", "A");
                            int ret = await conexao._conexao.ExecuteAsync("IA_RES_CARTAO", param, transaction: t, commandType: CommandType.StoredProcedure);
                            decimal id_prevenda = 0;
                            if (ret > 0)
                            {
                                //Pegar ID Cartão
                                decimal id_cartao = param.Get<decimal>("@P_ID_CARTAO");
                                //Nova Venda
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                param.Add("@P_ID_CARTAO", id_cartao);
                                param.Add("@P_CD_ENTREGADOR", null);
                                param.Add("@P_LOGINCANC", null);
                                param.Add("@P_LOGINVENDA", null);
                                param.Add("@P_ID_CAIXA", id_caixa);
                                param.Add("@P_DT_VENDA", DateTime.Now);
                                param.Add("@P_ST_DELIVERY", null);
                                param.Add("@P_NR_SENHAFASTFOOD", null);
                                param.Add("@P_DT_ENTREGADELIVERY", null);
                                param.Add("@P_ST_LEVARMAQCARTAO", null);
                                param.Add("@P_OBSFECHARDELIVERY", null);
                                param.Add("@P_VL_TROCOPARA", decimal.Zero);
                                param.Add("@P_ST_CLIENTERETIRA", "A");
                                param.Add("@P_HR_CLIENTERETIRA", null);
                                param.Add("@P_MOTIVOCANC", null);
                                param.Add("@P_IDIFOOD", null);
                                param.Add("@P_DT_SAIUENTREGA", null);
                                param.Add("@P_RECEBERRETIRADA", 1);
                                param.Add("@P_TROCABALCAOMESA", null);
                                param.Add("@P_ST_REGISTRO", "A");
                                ret = await conexao._conexao.ExecuteAsync("IA_RES_PREVENDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                                if (ret > 0)
                                    id_prevenda = param.Get<decimal>("@P_ID_PREVENDA");
                                //Gravar Venda Rapida
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_VENDARAPIDA", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                param.Add("@P_ID_PDV", null);
                                param.Add("@P_CD_CLIFOR", cfg.Cd_clifor);
                                param.Add("@P_CD_ENDERECO", null);
                                param.Add("@P_NR_CGC_CPF", null);
                                param.Add("@P_NM_CLIFOR", null);
                                param.Add("@P_DS_ENDERECO", null);
                                param.Add("@P_LOGINDESCONTO", null);
                                param.Add("@P_CD_TABELAPRECO", cfg.CD_TabelaPreco);
                                param.Add("@P_CD_CLIFORIND", null);
                                param.Add("@P_ID_PESSOA", null);
                                param.Add("@P_LOGINCANC", null);
                                param.Add("@P_CD_REPRESENTANTE", null);
                                param.Add("@P_CD_VENDEDOR", null);
                                param.Add("@P_DT_EMISSAO", DateTime.Now);
                                param.Add("@P_DS_OBSERVACAO", "VENDA APP  VALE FESTA");
                                param.Add("@P_PLACA", null);
                                param.Add("@P_NR_REQUISICAO", null);
                                param.Add("@P_MOTIVOCANC", null);
                                param.Add("@P_ST_REGISTRO", "A");
                                ret = await conexao._conexao.ExecuteAsync("IA_PDV_VENDARAPIDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                                decimal id_vendarapida = param.Get<decimal>("@P_ID_VENDARAPIDA");
                                //Gravar Caixa Recebimento
                                param = new DynamicParameters();
                                param.Add("@p_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@p_CD_CONTAGER", cfg.CD_ContaOperacional);
                                param.Add("@P_DT_LANCTO", DateTime.Now);
                                param.Add("@P_CD_HISTORICO", cfg.Cd_historico);
                                param.Add("@P_TP_MOV", "R");
                                param.Add("@P_COMPLHISTORICO", "VENDA APP VALE FESTA");
                                param.Add("@P_NM_CLIFOR", null);
                                param.Add("@P_VALOR", items.Sum(x => x.Quantidade * x.PrecoVenda + x.Adicionais.Sum(y => y.Quantidade * y.PrecoVenda)));
                                param.Add("@P_ST_TITULO", "N");
                                param.Add("@P_ST_ESTORNO", "N");
                                param.Add("@P_CD_LANCTOCAIXA", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                param.Add("@P_LOGIN", null);
                                param.Add("@P_ST_AVULSO", "N");
                                param.Add("@P_LOGINAUDITAVULSO", null);
                                param.Add("@P_DT_AUDITAVULSO", null);
                                param.Add("@P_ANEXO", null, dbType: DbType.Binary);
                                param.Add("@P_EXT_ANEXO", null);
                                await conexao._conexao.ExecuteAsync("IA_FIN_CAIXA", param, transaction: t, commandType: CommandType.StoredProcedure);
                                decimal cd_lanctocaixa = param.Get<decimal>("@P_CD_LANCTOCAIXA");
                                //Caixa x Venda rapida
                                param = new DynamicParameters();
                                param.Add("@P_ID_MOVIMENTO", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                param.Add("@P_ID_CUPOM", id_vendarapida);
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_CD_CONTAGER", cfg.CD_ContaOperacional);
                                param.Add("@P_CD_LANCTOCAIXA", cd_lanctocaixa);
                                param.Add("@P_CD_PORTADOR", cfg.Cd_portador);
                                param.Add("@P_ID_CAIXA", id_caixa);
                                param.Add("@P_CD_LANCTOCAIXA_TROCO", null);
                                param.Add("@P_ID_ADTO", null);
                                param.Add("@P_ID_CARTAFRETE", null);
                                await conexao._conexao.ExecuteAsync("IA_PDV_CUPOM_X_MOVCAIXA", param, transaction: t, commandType: CommandType.StoredProcedure);

                                foreach (var p in items)
                                {
                                    //Gravar Item
                                    param = new DynamicParameters();
                                    param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                    param.Add("@P_ID_PREVENDA", id_prevenda);
                                    param.Add("@P_ID_ITEM", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                    param.Add("@P_CD_PRODUTO", p.Cd_produto);
                                    param.Add("@P_ID_ITEMPAIADIC", null);
                                    param.Add("@P_LOGINCANC", null);
                                    param.Add("@P_CD_GARCON", p.Cd_garcom);
                                    param.Add("@P_QUANTIDADE", p.Quantidade);
                                    param.Add("@P_VL_UNITARIO", p.PrecoVenda);
                                    param.Add("@P_VL_DESCONTO", decimal.Zero);
                                    param.Add("@P_VL_ACRESCIMO", decimal.Zero);
                                    param.Add("@P_VL_FRETE", decimal.Zero);
                                    param.Add("@P_OBSITEM", p.Obs);
                                    param.Add("@P_IMPRESSO", false);
                                    param.Add("@P_PONTOCARNE", p.PontoCarne);
                                    param.Add("@P_PEDIDOAPP", true);
                                    param.Add("@P_NR_MESACARTAO", null);
                                    param.Add("@P_ST_REGISTRO", "A");
                                    param.Add("@P_MOTIVOCANC", null);
                                    await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                                    p.Cd_empresa = cfg.Cd_empresa;
                                    p.Id_prevenda = id_prevenda;
                                    p.Id_item = param.Get<decimal>("@P_ID_ITEM");
                                    //Gravar Item Venda Rapida
                                    param = new DynamicParameters();
                                    param.Add("@P_ID_VENDARAPIDA", id_vendarapida);
                                    param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                    param.Add("@P_ID_LANCTOVENDA", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                    param.Add("@P_CD_PRODUTO", p.Cd_produto);
                                    param.Add("@P_CD_LOCAL", cfg.CD_Local);
                                    param.Add("@P_CD_VENDEDOR", null);
                                    param.Add("@P_QUANTIDADE", p.Quantidade);
                                    param.Add("@P_VL_UNITARIO", p.PrecoVenda);
                                    param.Add("@P_VL_DESCONTO", decimal.Zero);
                                    param.Add("@P_VL_ACRESCIMO", decimal.Zero);
                                    param.Add("@P_VL_JURO_FIN", decimal.Zero);
                                    param.Add("@P_VL_FRETE", decimal.Zero);
                                    param.Add("@P_VL_SUBTOTAL", p.Quantidade * p.PrecoVenda);
                                    param.Add("@P_ST_REGISTRO", "A");
                                    ret = await conexao._conexao.ExecuteAsync("IA_PDV_VENDARAPIDA_ITEM", param, transaction: t, commandType: CommandType.StoredProcedure);
                                    decimal id_lanctovenda = param.Get<decimal>("@P_ID_LANCTOVENDA");
                                    //Baixar estoque
                                    param = new DynamicParameters();
                                    param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                    param.Add("@P_CD_PRODUTO", p.Cd_produto);
                                    param.Add("@P_ID_LANCTOESTOQUE", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                    param.Add("@P_CD_LOCAL", cfg.CD_Local);
                                    param.Add("@P_ID_VARIEDADE", null);
                                    param.Add("@P_DT_LANCTO", DateTime.Now.ToString("yyyyMMdd"));
                                    param.Add("@P_TP_MOVIMENTO", "S");
                                    param.Add("@P_QTD_ENTRADA", decimal.Zero);
                                    param.Add("@P_QTD_SAIDA", p.Quantidade);
                                    param.Add("@P_VL_UNITARIO", decimal.Zero);
                                    param.Add("@P_VL_SUBTOTAL", decimal.Zero);
                                    param.Add("@P_TP_LANCTO", "N");
                                    param.Add("@P_ST_REGISTRO", "A");
                                    param.Add("@P_DS_OBSERVACAO", "VENDA APP VALE FESTA");
                                    ret = await conexao._conexao.ExecuteAsync("INCLUI_EST_ESTOQUE", param, transaction: t, commandType: CommandType.StoredProcedure);
                                    decimal id_estoque = param.Get<decimal>("@P_ID_LANCTOESTOQUE");
                                    //Amarrar estoque com item venda rapida
                                    param = new DynamicParameters();
                                    param.Add("@P_ID_CUPOM", id_vendarapida);
                                    param.Add("@P_ID_LANCTO", id_lanctovenda);
                                    param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                    param.Add("@P_CD_PRODUTO", p.Cd_produto);
                                    param.Add("@P_ID_LANCTOESTOQUE", id_estoque);
                                    await conexao._conexao.ExecuteAsync("IA_PDV_CUPOMFISCAL_ITEM_X_ESTOQUE", param, transaction: t, commandType: CommandType.StoredProcedure);
                                    //Amarrar item venda rapida x item venda restaurante
                                    param = new DynamicParameters();
                                    param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                    param.Add("@P_ID_PREVENDA", id_prevenda);
                                    param.Add("@P_ID_ITEM", p.Id_item);
                                    param.Add("@P_ID_REGISTRO", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                    param.Add("@P_ID_LANCTOVENDA", id_lanctovenda);
                                    param.Add("@P_ID_VENDARAPIDA", id_vendarapida);
                                    param.Add("@P_ID_NFCE", null);
                                    param.Add("@P_ID_LANCTONFCE", null);
                                    await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_X_ITENSCUPOM", param, transaction: t, commandType: CommandType.StoredProcedure);
                                    //Adicional
                                    foreach (var v in p.Adicionais)
                                    {
                                        param = new DynamicParameters();
                                        param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                        param.Add("@P_ID_PREVENDA", id_prevenda);
                                        param.Add("@P_ID_ITEM", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                        param.Add("@P_CD_PRODUTO", v.Cd_adicional);
                                        param.Add("@P_ID_ITEMPAIADIC", p.Id_item);
                                        param.Add("@P_LOGINCANC", null);
                                        param.Add("@P_CD_GARCON", p.Cd_garcom);
                                        param.Add("@P_QUANTIDADE", 1);
                                        param.Add("@P_VL_UNITARIO", v.PrecoVenda);
                                        param.Add("@P_VL_DESCONTO", decimal.Zero);
                                        param.Add("@P_VL_ACRESCIMO", decimal.Zero);
                                        param.Add("@P_VL_FRETE", decimal.Zero);
                                        param.Add("@P_OBSITEM", null);
                                        param.Add("@P_IMPRESSO", false);
                                        param.Add("@P_PONTOCARNE", null);
                                        param.Add("@P_ST_REGISTRO", "A");
                                        param.Add("@P_MOTIVOCANC", null);
                                        param.Add("@P_PEDIDOAPP", true);
                                        param.Add("@P_NR_MESACARTAO", null);
                                        await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                                        decimal id_item = param.Get<decimal>("@P_ID_ITEM");

                                        //Gravar Item Venda Rapida
                                        param = new DynamicParameters();
                                        param.Add("@P_ID_VENDARAPIDA", id_vendarapida);
                                        param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                        param.Add("@P_ID_LANCTOVENDA", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                        param.Add("@P_CD_PRODUTO", p.Cd_produto);
                                        param.Add("@P_CD_LOCAL", cfg.CD_Local);
                                        param.Add("@P_CD_VENDEDOR", null);
                                        param.Add("@P_QUANTIDADE", p.Quantidade);
                                        param.Add("@P_VL_UNITARIO", p.PrecoVenda);
                                        param.Add("@P_VL_DESCONTO", decimal.Zero);
                                        param.Add("@P_VL_ACRESCIMO", decimal.Zero);
                                        param.Add("@P_VL_JURO_FIN", decimal.Zero);
                                        param.Add("@P_VL_FRETE", decimal.Zero);
                                        param.Add("@P_VL_SUBTOTAL", p.Quantidade * p.PrecoVenda);
                                        param.Add("@P_ST_REGISTRO", "A");
                                        ret = await conexao._conexao.ExecuteAsync("IA_PDV_VENDARAPIDA_ITEM", param, transaction: t, commandType: CommandType.StoredProcedure);
                                        id_lanctovenda = param.Get<decimal>("@P_ID_LANCTOVENDA");
                                        //Baixar estoque
                                        param = new DynamicParameters();
                                        param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                        param.Add("@P_CD_PRODUTO", p.Cd_produto);
                                        param.Add("@P_ID_LANCTOESTOQUE", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                        param.Add("@P_CD_LOCAL", cfg.CD_Local);
                                        param.Add("@P_ID_VARIEDADE", null);
                                        param.Add("@P_DT_LANCTO", DateTime.Now);
                                        param.Add("@P_TP_MOVIMENTO", "S");
                                        param.Add("@P_QTD_ENTRADA", decimal.Zero);
                                        param.Add("@P_QTD_SAIDA", p.Quantidade);
                                        param.Add("@P_VL_UNITARIO", decimal.Zero);
                                        param.Add("@P_VL_SUBTOTAL", decimal.Zero);
                                        param.Add("@P_TP_LANCTO", "N");
                                        param.Add("@P_ST_REGISTRO", "A");
                                        param.Add("@P_DS_OBSERVACAO", "VENDA APP VALE FESTA");
                                        ret = await conexao._conexao.ExecuteAsync("INCLUI_EST_ESTOQUE", param, transaction: t, commandType: CommandType.StoredProcedure);
                                        id_estoque = param.Get<decimal>("@P_ID_LANCTOESTOQUE");
                                        //Amarrar estoque com item venda rapida
                                        param = new DynamicParameters();
                                        param.Add("@P_ID_CUPOM", id_vendarapida);
                                        param.Add("@P_ID_LANCTO", id_lanctovenda);
                                        param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                        param.Add("@P_CD_PRODUTO", p.Cd_produto);
                                        param.Add("@P_ID_LANCTOESTOQUE", id_estoque);
                                        await conexao._conexao.ExecuteAsync("IA_PDV_CUPOMFISCAL_ITEM_X_ESTOQUE", param, transaction: t, commandType: CommandType.StoredProcedure);
                                        //Amarrar item venda rapida x item venda restaurante
                                        param = new DynamicParameters();
                                        param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                        param.Add("@P_ID_PREVENDA", id_prevenda);
                                        param.Add("@P_ID_ITEM", id_item);
                                        param.Add("@P_ID_REGISTRO", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                        param.Add("@P_ID_LANCTOVENDA", id_lanctovenda);
                                        param.Add("@P_ID_VENDARAPIDA", id_vendarapida);
                                        param.Add("@P_ID_NFCE", null);
                                        param.Add("@P_ID_LANCTONFCE", null);
                                        await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_X_ITENSCUPOM", param, transaction: t, commandType: CommandType.StoredProcedure);
                                    }
                                    //Ingredientes
                                    foreach (var v in p.Ingredientes)
                                    {
                                        param = new DynamicParameters();
                                        param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                        param.Add("@P_ID_PREVENDA", id_prevenda);
                                        param.Add("@P_ID_ITEM", p.Id_item);
                                        param.Add("@P_ID_INGREDIENTE", null);
                                        param.Add("@P_DS_INGREDIENTE", v.Ds_item);
                                        await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_INGREDIENTES", param, transaction: t, commandType: CommandType.StoredProcedure);
                                    }
                                    //Ingredientes Excluir
                                    foreach (var v in p.IngredientesDel)
                                    {
                                        param = new DynamicParameters();
                                        param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                        param.Add("@P_ID_PREVENDA", id_prevenda);
                                        param.Add("@P_ID_ITEM", p.Id_item);
                                        param.Add("@P_ID_EXCLUIR", null);
                                        param.Add("@P_DS_ITEMEXCLUIR", v.Ds_item);
                                        await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_ITEMEXCLUIR", param, transaction: t, commandType: CommandType.StoredProcedure);
                                    }
                                    //Itens Excluir
                                    foreach (var v in p.ItensExcluir)
                                    {
                                        param = new DynamicParameters();
                                        param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                        param.Add("@P_ID_PREVENDA", id_prevenda);
                                        param.Add("@P_ID_ITEM", p.Id_item);
                                        param.Add("@P_ID_EXCLUIR", null);
                                        param.Add("@P_DS_ITEMEXCLUIR", v.Ds_item);
                                        await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_ITEMEXCLUIR", param, transaction: t, commandType: CommandType.StoredProcedure);
                                    }
                                    //Sabores
                                    foreach (var v in p.Sabores)
                                    {
                                        param = new DynamicParameters();
                                        param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                        param.Add("@P_ID_PREVENDA", id_prevenda);
                                        param.Add("@P_ID_ITEM", p.Id_item);
                                        param.Add("@P_ID_SABOR", null);
                                        param.Add("@P_DS_SABOR", v.Ds_sabor);
                                        await conexao._conexao.ExecuteAsync("IA_RES_SABORESITENS", param, transaction: t, commandType: CommandType.StoredProcedure);
                                    }
                                }
                                //Gravar venda
                                t.Commit();
                                return true;
                            }
                            else return false;
                        }
                        else return false;
                    }
                    else return false;
                }
            }
            catch
            {
                if (t != null)
                    t.Dispose();
                return false;
            }
        }
        public async Task<bool> GravarItensBalcaoAsync(string token, string ClienteBalcao, List<ItemVenda> items)
        {
            SqlTransaction t = null;
            try
            {
                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        t = conexao._conexao.BeginTransaction(IsolationLevel.ReadCommitted);
                        //Buscar config
                        Config cfg = await conexao._conexao.QueryFirstAsync<Config>("select a.cd_empresa, a.cd_clifor from TB_RES_Config a " +
                                                                                    "inner join VTB_DIV_Empresa b " +
                                                                                    "on a.cd_empresa = b.cd_empresa " +
                                                                                    "and dbo.FVALIDA_NUMEROS(b.NR_CGC) = '" + Encoding.UTF8.GetString(Convert.FromBase64String(token)) + "'", transaction: t);
                        DynamicParameters param;
                        //Novo Cartão
                        param = new DynamicParameters();
                        param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                        param.Add("@P_ID_CARTAO", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                        param.Add("@P_CD_CLIFOR", cfg.Cd_clifor);
                        param.Add("@P_NM_CLIFOR", ClienteBalcao);
                        param.Add("@P_ID_MESA", null);
                        param.Add("@P_ID_LOCAL", null);
                        param.Add("@P_NR_CARTAO", null);
                        param.Add("@P_DT_ABERTURA", DateTime.Now);
                        param.Add("@P_DT_FECHAMENTO", null);
                        param.Add("@P_ST_MENORIDADE", "N");
                        param.Add("@P_VL_LIMITECARTAO", decimal.Zero);
                        param.Add("@P_VALEEVENTO", false);
                        param.Add("@P_ST_REGISTRO", "A");
                        int ret = await conexao._conexao.ExecuteAsync("IA_RES_CARTAO", param, transaction: t, commandType: CommandType.StoredProcedure);
                        decimal id_prevenda = 0;
                        if (ret > 0)
                        {
                            //Pegar ID Cartão
                            decimal id_cartao = param.Get<decimal>("@P_ID_CARTAO");
                            //Nova Venda
                            param = new DynamicParameters();
                            param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                            param.Add("@P_ID_PREVENDA", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                            param.Add("@P_ID_CARTAO", id_cartao);
                            param.Add("@P_CD_ENTREGADOR", null);
                            param.Add("@P_LOGINCANC", null);
                            param.Add("@P_LOGINVENDA", null);
                            param.Add("@P_ID_CAIXA", null);
                            param.Add("@P_DT_VENDA", DateTime.Now);
                            param.Add("@P_ST_DELIVERY", null);
                            param.Add("@P_NR_SENHAFASTFOOD", null);
                            param.Add("@P_DT_ENTREGADELIVERY", null);
                            param.Add("@P_ST_LEVARMAQCARTAO", null);
                            param.Add("@P_OBSFECHARDELIVERY", null);
                            param.Add("@P_VL_TROCOPARA", decimal.Zero);
                            param.Add("@P_ST_CLIENTERETIRA", "A");
                            param.Add("@P_HR_CLIENTERETIRA", null);
                            param.Add("@P_MOTIVOCANC", null);
                            param.Add("@P_IDIFOOD", null);
                            param.Add("@P_DT_SAIUENTREGA", null);
                            param.Add("@P_RECEBERRETIRADA", 1);
                            param.Add("@P_TROCABALCAOMESA", null);
                            param.Add("@P_ST_REGISTRO", "A");
                            ret = await conexao._conexao.ExecuteAsync("IA_RES_PREVENDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                            if (ret > 0)
                                id_prevenda = param.Get<decimal>("@P_ID_PREVENDA");
                        }
                        foreach (var p in items)
                        {
                            //Gravar Item
                            param = new DynamicParameters();
                            param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                            param.Add("@P_ID_PREVENDA", id_prevenda);
                            param.Add("@P_ID_ITEM", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                            param.Add("@P_CD_PRODUTO", p.Cd_produto);
                            param.Add("@P_ID_ITEMPAIADIC", null);
                            param.Add("@P_LOGINCANC", null);
                            param.Add("@P_CD_GARCON", p.Cd_garcom);
                            param.Add("@P_QUANTIDADE", p.Quantidade);
                            param.Add("@P_VL_UNITARIO", p.PrecoVenda);
                            param.Add("@P_VL_DESCONTO", decimal.Zero);
                            param.Add("@P_VL_ACRESCIMO", decimal.Zero);
                            param.Add("@P_VL_FRETE", decimal.Zero);
                            param.Add("@P_OBSITEM", p.Obs);
                            param.Add("@P_IMPRESSO", false);
                            param.Add("@P_PONTOCARNE", p.PontoCarne);
                            param.Add("@P_PEDIDOAPP", true);
                            param.Add("@P_NR_MESACARTAO", null);
                            param.Add("@P_ST_REGISTRO", "A");
                            param.Add("@P_MOTIVOCANC", null);
                            await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                            p.Cd_empresa = cfg.Cd_empresa;
                            p.Id_prevenda = id_prevenda;
                            p.Id_item = param.Get<decimal>("@P_ID_ITEM");
                            //Adicional
                            foreach (var v in p.Adicionais)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", dbType: DbType.Decimal, direction: ParameterDirection.Output);
                                param.Add("@P_CD_PRODUTO", v.Cd_adicional);
                                param.Add("@P_ID_ITEMPAIADIC", p.Id_item);
                                param.Add("@P_LOGINCANC", null);
                                param.Add("@P_CD_GARCON", p.Cd_garcom);
                                param.Add("@P_QUANTIDADE", 1);
                                param.Add("@P_VL_UNITARIO", v.PrecoVenda);
                                param.Add("@P_VL_DESCONTO", decimal.Zero);
                                param.Add("@P_VL_ACRESCIMO", decimal.Zero);
                                param.Add("@P_VL_FRETE", decimal.Zero);
                                param.Add("@P_OBSITEM", null);
                                param.Add("@P_IMPRESSO", false);
                                param.Add("@P_PONTOCARNE", null);
                                param.Add("@P_ST_REGISTRO", "A");
                                param.Add("@P_MOTIVOCANC", null);
                                param.Add("@P_PEDIDOAPP", true);
                                param.Add("@P_NR_MESACARTAO", null);
                                await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                            //Ingredientes
                            foreach (var v in p.Ingredientes)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", p.Id_item);
                                param.Add("@P_ID_INGREDIENTE", null);
                                param.Add("@P_DS_INGREDIENTE", v.Ds_item);
                                await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_INGREDIENTES", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                            //Ingredientes Excluir
                            foreach (var v in p.IngredientesDel)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", p.Id_item);
                                param.Add("@P_ID_EXCLUIR", null);
                                param.Add("@P_DS_ITEMEXCLUIR", v.Ds_item);
                                await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_ITEMEXCLUIR", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                            //Itens Excluir
                            foreach (var v in p.ItensExcluir)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", p.Id_item);
                                param.Add("@P_ID_EXCLUIR", null);
                                param.Add("@P_DS_ITEMEXCLUIR", v.Ds_item);
                                await conexao._conexao.ExecuteAsync("IA_RES_ITENSPREVENDA_ITEMEXCLUIR", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                            //Sabores
                            foreach (var v in p.Sabores)
                            {
                                param = new DynamicParameters();
                                param.Add("@P_CD_EMPRESA", cfg.Cd_empresa);
                                param.Add("@P_ID_PREVENDA", id_prevenda);
                                param.Add("@P_ID_ITEM", p.Id_item);
                                param.Add("@P_ID_SABOR", null);
                                param.Add("@P_DS_SABOR", v.Ds_sabor);
                                await conexao._conexao.ExecuteAsync("IA_RES_SABORESITENS", param, transaction: t, commandType: CommandType.StoredProcedure);
                            }
                        }
                        t.Commit();
                        return true;
                    }
                    else return false;
                }
            }
            catch
            {
                if (t != null)
                    t.Dispose();
                return false;
            }
        }
    }
}
