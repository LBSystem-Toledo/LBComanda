using LBComanda_API.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace LBComanda_API.Controllers
{
    public class LBComandaController : ApiController
    {
        //[HttpGet]
        //[ActionName("ValidarGarcom")]
        //public Garcom ValidarGarcom(string Login, string Senha)
        //{
        //    try
        //    {
        //        StringBuilder sql = new StringBuilder();
        //        sql.AppendLine("select a.CD_Clifor, a.NM_Clifor");
        //        sql.AppendLine("from VTB_FIN_CLIFOR a");
        //        sql.AppendLine("where ISNULL(a.ST_Registro, 'A') <> 'C'");
        //        sql.AppendLine("and ISNULL(a.st_funcativo, 'S') = 'S'");
        //        sql.AppendLine("and a.LoginGarcomApp = '" + Login.Trim() + "'");
        //        sql.AppendLine("and a.SenhaGarcomApp = '" + Senha.Trim() + "'");

        //        SqlCommand comando = new SqlCommand()
        //        {
        //            CommandType = System.Data.CommandType.Text,
        //            CommandText = sql.ToString()
        //        };
        //        if (TConexao.Ativar(true))
        //        {
        //            comando.Connection = TConexao.Conexao;
        //            SqlDataReader reader = comando.ExecuteReader();
        //            List<Garcom> lista = new List<Garcom>();
        //            while (reader.Read())
        //            {
        //                Garcom t = new Garcom();
        //                if (!reader.IsDBNull(reader.GetOrdinal("CD_Clifor")))
        //                    t.Cd_garcom = reader.GetString(reader.GetOrdinal("CD_Clifor"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("NM_Clifor")))
        //                    t.Nm_garcom = reader.GetString(reader.GetOrdinal("NM_Clifor"));
        //                lista.Add(t);
        //            }
        //            return lista?.FirstOrDefault();
        //        }
        //        else return null;
        //    }
        //    catch { return null; }
        //}

        //[HttpGet]
        //[ActionName("GetMesa")]
        //public List<Mesa> GetMesa()
        //{
        //    try
        //    {
        //        StringBuilder sql = new StringBuilder();
        //        sql.AppendLine("select a.ID_Mesa, a.NR_Mesa, a.ID_Local, b.DS_Local, ");
        //        sql.AppendLine("PossuiVenda = isnull((select top 1 'S' ");
        //        sql.AppendLine("			            from TB_RES_PreVenda x ");
        //        sql.AppendLine("			            inner join TB_RES_Cartao y ");
        //        sql.AppendLine("			            on x.CD_Empresa = y.CD_Empresa ");
        //        sql.AppendLine("			            and x.ID_Cartao = y.ID_Cartao ");
        //        sql.AppendLine("			            where y.ID_Local = a.ID_Local ");
        //        sql.AppendLine("			            and y.ID_Mesa = a.ID_Mesa ");
        //        sql.AppendLine("			            and ISNULL(y.ST_Registro, 'A') = 'A'), 'N')");
        //        sql.AppendLine("from TB_RES_Mesa a ");
        //        sql.AppendLine("inner join TB_RES_Local b ");
        //        sql.AppendLine("on a.ID_Local = b.ID_Local ");
        //        sql.AppendLine("and a.Cancelado = 0 ");
        //        sql.AppendLine("and b.Cancelado = 0");

        //        SqlCommand comando = new SqlCommand()
        //        {
        //            CommandType = System.Data.CommandType.Text,
        //            CommandText = sql.ToString()
        //        };
        //        if (TConexao.Ativar(true))
        //        {
        //            comando.Connection = TConexao.Conexao;
        //            SqlDataReader reader = comando.ExecuteReader();
        //            List<Mesa> lista = new List<Mesa>();
        //            while (reader.Read())
        //            {
        //                Mesa t = new Mesa();
        //                if (!reader.IsDBNull(reader.GetOrdinal("id_mesa")))
        //                    t.Id_mesa = reader.GetDecimal(reader.GetOrdinal("id_mesa"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("nr_mesa")))
        //                    t.Nr_mesa = reader.GetString(reader.GetOrdinal("nr_mesa"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("id_local")))
        //                    t.Id_local = reader.GetDecimal(reader.GetOrdinal("id_local"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("ds_local")))
        //                    t.Ds_local = reader.GetString(reader.GetOrdinal("ds_local"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("PossuiVenda")))
        //                    t.PossuiVenda = reader.GetString(reader.GetOrdinal("PossuiVenda")).Trim().ToUpper().Equals("S");
        //                lista.Add(t);
        //            }
        //            return lista;
        //        }
        //        else return null;
        //    }
        //    catch { return null; }
        //}

        //[HttpGet]
        //[ActionName("GetProdutos")]
        //public List<Produto> GetProdutos()
        //{
        //    try
        //    {
        //        StringBuilder sql = new StringBuilder();
        //        sql.AppendLine("select a.CD_Produto, a.DS_Produto, a.CD_Grupo, ");
        //        sql.AppendLine("b.DS_Grupo, b.CD_Grupo_Pai, c.DS_Grupo as DS_Grupo_Pai, b.PontoCarne,");
        //        sql.AppendLine("PrecoVenda = dbo.F_PRECO_VENDA(cfg.CD_Empresa, a.CD_Produto, cfg.CD_TabelaPreco)");
        //        sql.AppendLine("from TB_EST_Produto a");
        //        sql.AppendLine("inner join TB_EST_GrupoProduto b");
        //        sql.AppendLine("on a.CD_Grupo = b.CD_Grupo");
        //        sql.AppendLine("and isnull(a.st_registro, 'A') <> 'C'");
        //        sql.AppendLine("inner join TB_EST_GrupoProduto c");
        //        sql.AppendLine("on b.CD_Grupo_Pai = c.CD_Grupo");
        //        sql.AppendLine("inner join TB_EST_TpProduto d ");
        //        sql.AppendLine("on a.TP_Produto = d.TP_Produto ");
        //        sql.AppendLine("and ISNULL(d.ST_MPrima, 'N') <> 'S' ");
        //        sql.AppendLine("outer apply");
        //        sql.AppendLine("(");
        //        sql.AppendLine("	select top 1 x.CD_Empresa, x.CD_TabelaPreco");
        //        sql.AppendLine("	from TB_RES_Config x");
        //        sql.AppendLine(") as cfg");

        //        SqlCommand comando = new SqlCommand()
        //        {
        //            CommandType = System.Data.CommandType.Text,
        //            CommandText = sql.ToString()
        //        };
        //        if (TConexao.Ativar(true))
        //        {
        //            comando.Connection = TConexao.Conexao;
        //            SqlDataReader reader = comando.ExecuteReader();
        //            List<Produto> lista = new List<Produto>();
        //            while (reader.Read())
        //            {
        //                Produto t = new Produto();
        //                if (!reader.IsDBNull(reader.GetOrdinal("CD_Produto")))
        //                    t.Cd_produto = reader.GetString(reader.GetOrdinal("CD_Produto"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("DS_Produto")))
        //                    t.Ds_produto = reader.GetString(reader.GetOrdinal("DS_Produto"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("CD_Grupo")))
        //                    t.Cd_grupo = reader.GetString(reader.GetOrdinal("CD_Grupo"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("DS_Grupo")))
        //                    t.Ds_grupo = reader.GetString(reader.GetOrdinal("DS_Grupo"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("CD_Grupo_Pai")))
        //                    t.Cd_grupo_pai = reader.GetString(reader.GetOrdinal("CD_Grupo_Pai"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("DS_Grupo_Pai")))
        //                    t.Ds_grupo_pai = reader.GetString(reader.GetOrdinal("DS_Grupo_Pai"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("PontoCarne")))
        //                    t.PontoCarne = reader.GetBoolean(reader.GetOrdinal("PontoCarne"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("PrecoVenda")))
        //                    t.PrecoVenda = reader.GetDecimal(reader.GetOrdinal("PrecoVenda"));
        //                lista.Add(t);
        //            }
        //            return lista;
        //        }
        //        else return null;
        //    }
        //    catch { return null; }
        //}

        //[HttpGet]
        //[ActionName("GetPontoCarne")]
        //public List<PontoCarne> GetPontoCarne(string Cd_produto)
        //{
        //    try
        //    {
        //        StringBuilder sql = new StringBuilder();
        //        sql.AppendLine("select a.DS_Ponto");
        //        sql.AppendLine("from TB_RES_PontoCarne a");
        //        sql.AppendLine("where exists(select 1 from TB_EST_GrupoProduto x");
        //        sql.AppendLine("				inner join TB_EST_Produto y");
        //        sql.AppendLine("				on x.CD_Grupo = y.CD_Grupo");
        //        sql.AppendLine("				and isnull(x.PontoCarne, 0) = 1");
        //        sql.AppendLine("				and y.CD_Produto = '" + Cd_produto.Trim() + "')");

        //        SqlCommand comando = new SqlCommand()
        //        {
        //            CommandType = System.Data.CommandType.Text,
        //            CommandText = sql.ToString()
        //        };
        //        if (TConexao.Ativar(true))
        //        {
        //            comando.Connection = TConexao.Conexao;
        //            SqlDataReader reader = comando.ExecuteReader();
        //            List<PontoCarne> lista = new List<PontoCarne>();
        //            while (reader.Read())
        //            {
        //                PontoCarne t = new PontoCarne();
        //                if (!reader.IsDBNull(reader.GetOrdinal("ds_ponto")))
        //                    t.Ds_ponto = reader.GetString(reader.GetOrdinal("ds_ponto"));
        //                lista.Add(t);
        //            }
        //            return lista;
        //        }
        //        else return null;
        //    }
        //    catch { return null; }
        //}

        //[HttpGet]
        //[ActionName("GetAdicionais")]
        //public List<Adicional> GetAdicionais(string Cd_produto)
        //{
        //    try
        //    {
        //        StringBuilder sql = new StringBuilder();
        //        sql.AppendLine("select ISNULL(a.CD_ProdutoAdicional, c.CD_Produto) as CD_Produto,");
        //        sql.AppendLine("ISNULL(d.DS_Produto, c.DS_Produto) as DS_Produto,");
        //        sql.AppendLine("Vl_venda = dbo.F_PRECO_VENDA(cfg.cd_empresa, ISNULL(a.CD_ProdutoAdicional, c.CD_Produto), cfg.CD_TabelaPreco)");
        //        sql.AppendLine("from TB_RES_Adicionais a");
        //        sql.AppendLine("inner join TB_EST_Produto b");
        //        sql.AppendLine("on a.CD_GrupoProduto = b.CD_Grupo");
        //        sql.AppendLine("and isnull(b.st_registro, 'A') <> 'C'");
        //        sql.AppendLine("left outer join TB_EST_Produto c");
        //        sql.AppendLine("on a.CD_GrupoAdicional = c.CD_Grupo");
        //        sql.AppendLine("and isnull(c.st_registro, 'A') <> 'C'");
        //        sql.AppendLine("left outer join TB_EST_Produto d");
        //        sql.AppendLine("on a.CD_ProdutoAdicional = d.CD_Produto");
        //        sql.AppendLine("and isnull(d.st_registro, 'A') <> 'C'");
        //        sql.AppendLine("outer apply");
        //        sql.AppendLine("(");
        //        sql.AppendLine("	select top 1 x.CD_Empresa, x.CD_TabelaPreco");
        //        sql.AppendLine("	from TB_RES_Config x");
        //        sql.AppendLine(") as cfg");
        //        sql.AppendLine("where b.CD_Produto = '" + Cd_produto.Trim() + "'");

        //        SqlCommand comando = new SqlCommand()
        //        {
        //            CommandType = System.Data.CommandType.Text,
        //            CommandText = sql.ToString()
        //        };
        //        if (TConexao.Ativar(true))
        //        {
        //            comando.Connection = TConexao.Conexao;
        //            SqlDataReader reader = comando.ExecuteReader();
        //            List<Adicional> lista = new List<Adicional>();
        //            while (reader.Read())
        //            {
        //                Adicional t = new Adicional();
        //                if (!reader.IsDBNull(reader.GetOrdinal("CD_Produto")))
        //                    t.Cd_produto = reader.GetString(reader.GetOrdinal("CD_Produto"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("DS_Produto")))
        //                    t.Ds_produto = reader.GetString(reader.GetOrdinal("DS_Produto"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("Vl_venda")))
        //                    t.PrecoVenda = reader.GetDecimal(reader.GetOrdinal("Vl_venda"));
        //                lista.Add(t);
        //            }
        //            return lista;
        //        }
        //        else return null;
        //    }
        //    catch { return null; }
        //}

        //[HttpGet]
        //[ActionName("GetIngredientes")]
        //public List<Ingredientes> GetIngredientes(string Cd_produto)
        //{
        //    try
        //    {
        //        StringBuilder sql = new StringBuilder();
        //        sql.AppendLine("select a.CD_Produto, a.CD_Item,");
        //        sql.AppendLine("b.DS_Produto as DS_Item, a.Obrigatorio");
        //        sql.AppendLine("from TB_EST_FichaTecProduto a");
        //        sql.AppendLine("inner join TB_EST_Produto b");
        //        sql.AppendLine("on a.CD_Item = b.CD_Produto");
        //        sql.AppendLine("where a.CD_Produto = '" + Cd_produto.Trim() + "'");

        //        SqlCommand comando = new SqlCommand()
        //        {
        //            CommandType = System.Data.CommandType.Text,
        //            CommandText = sql.ToString()
        //        };
        //        if (TConexao.Ativar(true))
        //        {
        //            comando.Connection = TConexao.Conexao;
        //            SqlDataReader reader = comando.ExecuteReader();
        //            List<Ingredientes> lista = new List<Ingredientes>();
        //            while (reader.Read())
        //            {
        //                Ingredientes t = new Ingredientes();
        //                if (!reader.IsDBNull(reader.GetOrdinal("CD_Produto")))
        //                    t.Cd_produto = reader.GetString(reader.GetOrdinal("CD_Produto"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("CD_Item")))
        //                    t.Cd_item = reader.GetString(reader.GetOrdinal("CD_Item"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("DS_Item")))
        //                    t.Ds_item = reader.GetString(reader.GetOrdinal("DS_Item"));
        //                if (!reader.IsDBNull(reader.GetOrdinal("Obrigatorio")))
        //                    t.Obrigatorio = reader.GetBoolean(reader.GetOrdinal("Obrigatorio"));
        //                lista.Add(t);
        //            }
        //            return lista;
        //        }
        //        else return null;
        //    }
        //    catch { return null; }
        //}


        //[HttpGet]
        //[ActionName("GetSabores")]
        //public List<Sabor> GetSabores(string Cd_produto)
        //{
        //    try
        //    {
        //        StringBuilder sql = new StringBuilder();
        //        sql.AppendLine("select a.DS_Sabor ");
        //        sql.AppendLine("from TB_RES_Sabores a");
        //        sql.AppendLine("where exists(select 1 from tb_est_produto x ");
        //        sql.AppendLine("                where x.cd_grupo = a.cd_grupo ");
        //        sql.AppendLine("                and x.CD_Produto = '" + Cd_produto.Trim() + "')");

        //        SqlCommand comando = new SqlCommand()
        //        {
        //            CommandType = System.Data.CommandType.Text,
        //            CommandText = sql.ToString()
        //        };
        //        if (TConexao.Ativar(true))
        //        {
        //            comando.Connection = TConexao.Conexao;
        //            SqlDataReader reader = comando.ExecuteReader();
        //            List<Sabor> lista = new List<Sabor>();
        //            while (reader.Read())
        //            {
        //                Sabor t = new Sabor();
        //                if (!reader.IsDBNull(reader.GetOrdinal("DS_Sabor")))
        //                    t.Ds_sabor = reader.GetString(reader.GetOrdinal("DS_Sabor"));
        //                lista.Add(t);
        //            }
        //            return lista;
        //        }
        //        else return null;
        //    }
        //    catch { return null; }
        //}

        [HttpGet]
        [ActionName("GetExtrato")]
        public List<ItemVenda> GetExtrato(string Id_local,
                                          string Id_mesa)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("select a.cd_produto, b.DS_Produto, a.Vl_Unitario,");
                sql.AppendLine("a.Quantidade - a.qtd_faturada as Quantidade");
                sql.AppendLine("from vtb_res_itensprevenda a ");
                sql.AppendLine("inner join TB_EST_Produto b");
                sql.AppendLine("on a.CD_Produto = b.CD_Produto");
                sql.AppendLine("inner join TB_RES_PreVenda c");
                sql.AppendLine("on a.cd_empresa = c.CD_Empresa");
                sql.AppendLine("and a.ID_PreVenda = c.ID_PreVenda");
                sql.AppendLine("inner join TB_RES_Cartao d");
                sql.AppendLine("on c.CD_Empresa = d.CD_Empresa");
                sql.AppendLine("and c.ID_Cartao = d.ID_Cartao");
                sql.AppendLine("and ISNULL(d.ST_Registro, 'A') = 'A'");
                sql.AppendLine("where d.ID_Local = " + Id_local);
                sql.AppendLine("and d.ID_Mesa = " + Id_mesa);

                SqlCommand comando = new SqlCommand()
                {
                    CommandType = System.Data.CommandType.Text,
                    CommandText = sql.ToString()
                };
                if (TConexao.Ativar(true))
                {
                    comando.Connection = TConexao.Conexao;
                    SqlDataReader reader = comando.ExecuteReader();
                    List<ItemVenda> lista = new List<ItemVenda>();
                    while (reader.Read())
                    {
                        ItemVenda t = new ItemVenda();
                        if (!reader.IsDBNull(reader.GetOrdinal("cd_produto")))
                            t.Cd_produto = reader.GetString(reader.GetOrdinal("cd_produto"));
                        if (!reader.IsDBNull(reader.GetOrdinal("DS_Produto")))
                            t.Ds_produto = reader.GetString(reader.GetOrdinal("DS_Produto"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Vl_Unitario")))
                            t.PrecoVenda = reader.GetDecimal(reader.GetOrdinal("Vl_Unitario"));
                        if (!reader.IsDBNull(reader.GetOrdinal("Quantidade")))
                            t.Quantidade = reader.GetDecimal(reader.GetOrdinal("Quantidade"));
                        lista.Add(t);
                    }
                    return lista;
                }
                else return null;
            }
            catch { return null; }
        }

        [HttpPost]
        [ActionName("GravarItens")]
        public bool GravarItens(string Id_local,
                                string Id_mesa,
                                [FromBody] List<ItemVenda> items)
        {
            SqlTransaction transaction = null;
            try
            {
                if (TConexao.Ativar(true))
                {
                    transaction = TConexao.Conexao.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    SqlCommand comando = new SqlCommand();
                    comando.Connection = TConexao.Conexao;
                    comando.Transaction = transaction;
                    string Cd_empresa = string.Empty;
                    string Cd_clifor = string.Empty;
                    string Id_prevenda = string.Empty;
                    //Buscar config
                    comando.CommandType = System.Data.CommandType.Text;
                    comando.CommandText = "select top 1 cd_empresa, cd_clifor from TB_RES_Config";
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Cd_empresa = reader.GetString(0);
                            Cd_clifor = reader.GetString(1);
                        }
                    }
                    //Buscar venda aberta
                    comando.CommandText = "select a.cd_empresa, a.id_prevenda " +
                                          "from VTB_RES_PREVENDA a " +
                                          "inner join TB_RES_Cartao b " +
                                          "on a.cd_empresa = b.cd_empresa " +
                                          "and a.id_cartao = b.id_cartao " +
                                          "where b.id_local = " + Id_local + " " +
                                          "and b.id_mesa = " + Id_mesa + " " +
                                          "and isnull(b.st_registro, 'A') = 'A'";
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Cd_empresa = reader.GetString(0);
                            Id_prevenda = reader.GetDecimal(1).ToString();
                        }
                    }
                    if (string.IsNullOrWhiteSpace(Id_prevenda))
                    {
                        //Novo Cartao
                        comando.CommandType = System.Data.CommandType.StoredProcedure;
                        comando.CommandText = "IA_RES_CARTAO";
                        comando.Parameters.AddWithValue("@P_CD_EMPRESA", Cd_empresa);
                        SqlParameter pCartao = new SqlParameter();
                        pCartao.SqlDbType = System.Data.SqlDbType.Decimal;
                        pCartao.Direction = System.Data.ParameterDirection.InputOutput;
                        pCartao.ParameterName = "@P_ID_CARTAO";
                        pCartao.Value = DBNull.Value;
                        comando.Parameters.Add(pCartao);
                        comando.Parameters.AddWithValue("@P_CD_CLIFOR", Cd_clifor);
                        comando.Parameters.AddWithValue("@P_ID_MESA", Id_mesa);
                        comando.Parameters.AddWithValue("@P_ID_LOCAL", Id_local);
                        comando.Parameters.AddWithValue("@P_NR_CARTAO", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_DT_ABERTURA", DateTime.Now);
                        comando.Parameters.AddWithValue("@P_DT_FECHAMENTO", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_ST_MENORIDADE", "N");
                        comando.Parameters.AddWithValue("@P_VL_LIMITECARTAO", decimal.Zero);
                        comando.Parameters.AddWithValue("@P_ST_REGISTRO", "A");
                        comando.ExecuteNonQuery();
                        //Nova Venda
                        comando.CommandText = "IA_RES_PREVENDA";
                        comando.Parameters.Clear();
                        comando.Parameters.AddWithValue("@P_CD_EMPRESA", Cd_empresa);
                        SqlParameter pId_venda = new SqlParameter();
                        pId_venda.SqlDbType = System.Data.SqlDbType.Decimal;
                        pId_venda.Direction = System.Data.ParameterDirection.InputOutput;
                        pId_venda.ParameterName = "@P_ID_PREVENDA";
                        pId_venda.Value = DBNull.Value;
                        comando.Parameters.Add(pId_venda);
                        comando.Parameters.AddWithValue("@P_ID_CARTAO", pCartao.Value.ToString());
                        comando.Parameters.AddWithValue("@P_CD_ENTREGADOR", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_LOGINCANC", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_LOGINVENDA", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_ID_CAIXA", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_DT_VENDA", DateTime.Now);
                        comando.Parameters.AddWithValue("@P_ST_DELIVERY", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_NR_SENHAFASTFOOD", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_DT_ENTREGADELIVERY", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_ST_LEVARMAQCARTAO", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_OBSFECHARDELIVERY", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_VL_TROCOPARA", decimal.Zero);
                        comando.Parameters.AddWithValue("@P_ST_CLIENTERETIRA", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_HR_CLIENTERETIRA", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_VL_TAXAENTREGA", decimal.Zero);
                        comando.Parameters.AddWithValue("@P_MOTIVOCANC", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_ST_REGISTRO", "A");
                        comando.ExecuteNonQuery();
                        Id_prevenda = pId_venda.Value.ToString();
                    }
                    //Gravar Pedido API
                    comando.CommandText = "IA_RES_PEDIDOAPI";
                    comando.CommandType = System.Data.CommandType.StoredProcedure;
                    comando.Parameters.Clear();
                    SqlParameter pId = new SqlParameter();
                    pId.SqlDbType = System.Data.SqlDbType.Decimal;
                    pId.Direction = System.Data.ParameterDirection.InputOutput;
                    pId.ParameterName = "@P_ID";
                    pId.Value = DBNull.Value;
                    comando.Parameters.Add(pId);
                    comando.Parameters.AddWithValue("@P_CD_EMPRESA", Cd_empresa);
                    comando.Parameters.AddWithValue("@P_ID_PREVENDA", Id_prevenda);
                    comando.Parameters.AddWithValue("@P_IMPRESSO", false);
                    comando.ExecuteNonQuery();
                    items.ForEach(p =>
                    {
                        //Gravar Item
                        comando.CommandText = "IA_RES_ITENSPREVENDA";
                        comando.CommandType = System.Data.CommandType.StoredProcedure;
                        comando.Parameters.Clear();
                        comando.Parameters.AddWithValue("@P_CD_EMPRESA", Cd_empresa);
                        comando.Parameters.AddWithValue("@P_ID_PREVENDA", Id_prevenda);
                        SqlParameter pId_item = new SqlParameter();
                        pId_item.SqlDbType = System.Data.SqlDbType.Decimal;
                        pId_item.Direction = System.Data.ParameterDirection.InputOutput;
                        pId_item.ParameterName = "@P_ID_ITEM";
                        pId_item.Value = DBNull.Value;
                        comando.Parameters.Add(pId_item);
                        comando.Parameters.AddWithValue("@P_CD_PRODUTO", p.Cd_produto);
                        comando.Parameters.AddWithValue("@P_ID_ITEMPAIADIC", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_LOGINCANC", DBNull.Value);
                        comando.Parameters.AddWithValue("@P_CD_GARCON", p.Cd_garcom);
                        comando.Parameters.AddWithValue("@P_QUANTIDADE", p.Quantidade);
                        comando.Parameters.AddWithValue("@P_VL_UNITARIO", p.PrecoVenda);
                        comando.Parameters.AddWithValue("@P_VL_DESCONTO", decimal.Zero);
                        comando.Parameters.AddWithValue("@P_VL_ACRESCIMO", decimal.Zero);
                        comando.Parameters.AddWithValue("@P_OBSITEM", p.Obs);
                        comando.Parameters.AddWithValue("@P_IMPRESSO", false);
                        comando.Parameters.AddWithValue("@P_PONTOCARNE", p.PontoCarne);
                        comando.Parameters.AddWithValue("@P_ST_REGISTRO", "A");
                        comando.Parameters.AddWithValue("@P_MOTIVOCANC", DBNull.Value);
                        comando.ExecuteNonQuery();
                        p.Cd_empresa = Cd_empresa;
                        p.Id_prevenda = decimal.Parse(Id_prevenda);
                        p.Id_item = decimal.Parse(pId_item.Value.ToString());
                        //Adicional
                        p.Adicionais
                        .ForEach(v =>
                        {
                            comando.CommandText = "IA_RES_ITENSPREVENDA";
                            comando.Parameters.Clear();
                            comando.Parameters.AddWithValue("@P_CD_EMPRESA", Cd_empresa);
                            comando.Parameters.AddWithValue("@P_ID_PREVENDA", Id_prevenda);
                            SqlParameter pId_adic = new SqlParameter();
                            pId_adic.SqlDbType = System.Data.SqlDbType.Decimal;
                            pId_adic.Direction = System.Data.ParameterDirection.InputOutput;
                            pId_adic.ParameterName = "@P_ID_ITEM";
                            pId_adic.Value = DBNull.Value;
                            comando.Parameters.Add(pId_adic);
                            comando.Parameters.AddWithValue("@P_CD_PRODUTO", v.Cd_adicional);
                            comando.Parameters.AddWithValue("@P_ID_ITEMPAIADIC", pId_item.Value);
                            comando.Parameters.AddWithValue("@P_LOGINCANC", DBNull.Value);
                            comando.Parameters.AddWithValue("@P_CD_GARCON", p.Cd_garcom);
                            comando.Parameters.AddWithValue("@P_QUANTIDADE", 1);
                            comando.Parameters.AddWithValue("@P_VL_UNITARIO", v.PrecoVenda);
                            comando.Parameters.AddWithValue("@P_VL_DESCONTO", decimal.Zero);
                            comando.Parameters.AddWithValue("@P_VL_ACRESCIMO", decimal.Zero);
                            comando.Parameters.AddWithValue("@P_OBSITEM", DBNull.Value);
                            comando.Parameters.AddWithValue("@P_IMPRESSO", false);
                            comando.Parameters.AddWithValue("@P_PONTOCARNE", DBNull.Value);
                            comando.Parameters.AddWithValue("@P_ST_REGISTRO", "A");
                            comando.Parameters.AddWithValue("@P_MOTIVOCANC", DBNull.Value);
                            comando.ExecuteNonQuery();
                            p.Cd_empresa = Cd_empresa;
                            p.Id_prevenda = decimal.Parse(Id_prevenda);
                            v.Id_adicional = int.Parse(pId_adic.Value.ToString());
                        });
                        //Ingredientes
                        p.Ingredientes
                        .ForEach(v =>
                        {
                            comando.CommandText = "IA_RES_ITENSPREVENDA_INGREDIENTES";
                            comando.Parameters.Clear();
                            comando.Parameters.AddWithValue("@P_CD_EMPRESA", Cd_empresa);
                            comando.Parameters.AddWithValue("@P_ID_PREVENDA", Id_prevenda);
                            comando.Parameters.AddWithValue("@P_ID_ITEM", pId_item.Value);
                            comando.Parameters.AddWithValue("@P_ID_INGREDIENTE", DBNull.Value);
                            comando.Parameters.AddWithValue("@P_DS_INGREDIENTE", v.Ds_item);
                            comando.ExecuteNonQuery();
                        });
                        //Ingredientes Excluir
                        p.IngredientesDel
                        .ForEach(v =>
                        {
                            comando.CommandText = "IA_RES_ITENSPREVENDA_ITEMEXCLUIR";
                            comando.Parameters.Clear();
                            comando.Parameters.AddWithValue("@P_CD_EMPRESA", Cd_empresa);
                            comando.Parameters.AddWithValue("@P_ID_PREVENDA", Id_prevenda);
                            comando.Parameters.AddWithValue("@P_ID_ITEM", pId_item.Value);
                            comando.Parameters.AddWithValue("@P_ID_EXCLUIR", DBNull.Value);
                            comando.Parameters.AddWithValue("@P_DS_ITEMEXCLUIR", v.Ds_item);
                            comando.ExecuteNonQuery();
                        });
                        //Sabores
                        p.Sabores
                        .ForEach(v =>
                        {
                            comando.CommandText = "IA_RES_SABORESITENS";
                            comando.Parameters.Clear();
                            comando.Parameters.AddWithValue("@P_CD_EMPRESA", Cd_empresa);
                            comando.Parameters.AddWithValue("@P_ID_PREVENDA", Id_prevenda);
                            comando.Parameters.AddWithValue("@P_ID_ITEM", pId_item.Value);
                            comando.Parameters.AddWithValue("@P_ID_SABOR", DBNull.Value);
                            comando.Parameters.AddWithValue("@P_DS_SABOR", v.Ds_sabor);
                            comando.ExecuteNonQuery();
                        });
                    });
                    transaction.Commit();
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                return false;
            }
        }
    }
}
