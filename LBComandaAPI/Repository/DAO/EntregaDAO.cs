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
    public class EntregaDAO: IEntrega
    {
        readonly IConfiguration _config;
        public EntregaDAO(IConfiguration config) { _config = config; }

        public async Task<Entrega> ApontarEntregaAsync(string token, string id_prevenda, string cd_entregador)
        {
            try
            {
                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("select a.Cd_empresa, a.ID_PreVenda, a.ST_LevarMaqCartao,")
                            .AppendLine("a.ObsFecharDelivery, a.Vl_TrocoPara, c.NM_Clifor,")
                            .AppendLine("c.DS_Endereco, c.Numero, c.Bairro, c.DS_Complemento,")
                            .AppendLine("c.Proximo, c.DS_Cidade, c.DS_Observacao")
                            .AppendLine("from VTB_RES_PREVENDA a")
                            .AppendLine("inner join TB_RES_Cartao b")
                            .AppendLine("on a.CD_Empresa = b.cd_empresa")
                            .AppendLine("and a.ID_Cartao = b.id_cartao")
                            .AppendLine("inner join VTB_FIN_CLIFOR c")
                            .AppendLine("on b.CD_Clifor = c.CD_Clifor")
                            .AppendLine("inner join VTB_DIV_EMPRESA d")
                            .AppendLine("on a.CD_Empresa = d.CD_Empresa")
                            .AppendLine("where a.ST_Delivery = 'A'")
                            .AppendLine("and dbo.FVALIDA_NUMEROS(d.NR_CGC) = '" + Encoding.UTF8.GetString(Convert.FromBase64String(token)) + "'")
                            .AppendLine("and a.id_prevenda = " + id_prevenda);
                        Entrega ret = await conexao._conexao.QueryFirstOrDefaultAsync<Entrega>(sql.ToString());
                        if (ret != null)
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@CD_ENTREGADOR", cd_entregador);
                            param.Add("@CD_EMPRESA", ret.Cd_empresa);
                            param.Add("@ID_PREVENDA", ret.Id_prevenda);
                            await conexao._conexao.ExecuteAsync("update TB_RES_PreVenda set ST_Delivery = 'F', DT_SaiuEntrega = getdate(), " +
                                                                "CD_Entregador = @CD_ENTREGADOR, DT_Alt = getdate() " +
                                                                "where cd_empresa = @CD_EMPRESA and id_prevenda = @ID_PREVENDA", param);
                        }
                        return ret;
                    }
                    else return null;
                }
            }
            catch { return null; }
        }

        public async Task<bool> ConcluirEntregaAsync(string token, Entrega entrega)
        {
            try
            {
                using (TConexao conexao = new TConexao(_config.GetConnectionString(Encoding.UTF8.GetString(Convert.FromBase64String(token)))))
                {
                    if (await conexao.OpenConnectionAsync())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@CD_EMPRESA", entrega.Cd_empresa);
                        param.Add("@ID_PREVENDA", entrega.Id_prevenda);
                        param.Add("@DT_ENTREGADELIVERY", entrega.Dt_entregadelivery.Value);
                        int ret =await conexao._conexao.ExecuteAsync("update TB_RES_PreVenda set ST_Delivery = 'E', DT_EntregaDelivery = @DT_ENTREGADELIVERY, DT_Alt = getdate() " +
                                                                     "where cd_empresa = @CD_EMPRESA and id_prevenda = @ID_PREVENDA", param);
                        return ret > 0;
                    }
                    else return false;
                }
            }
            catch { return false; }
        }
    }
}
