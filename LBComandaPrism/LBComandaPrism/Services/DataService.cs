using LBComandaPrism.Models;
using LBComandaPrism.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LBComandaPrism.Services
{
    public static class DataService
    {
        public static TokenStone TokenStone { get; set; }

        public static async Task<Garcom> ValidarLoginAsync(string Login, string Senha, string Cnpj)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Garcom/ValidarGarcomAsync", Method.Get);
                    request.AddHeader("login", Login)
                        .AddHeader("senha", Senha)
                        .AddHeader("cnpj", Cnpj);
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<Garcom>(response.Content);
                    else return null;
                }
            }
            catch{ return null; }
        }
        public static async Task<Token> ValidarTokenAsync()
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Garcom/ValidarTokenAsync", Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())))
                        .AddHeader("garcom", App.Garcom.Cd_garcom);
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<Token>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<bool> SolicitarTokenAsync()
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Garcom/SolicitarTokenAsync", Method.Post);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())))
                        .AddHeader("garcom", App.Garcom.Cd_garcom);
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<bool>(response.Content);
                    else return false;
                }
            }
            catch { return false; }
        }
        public static async Task<List<Mesa>> GetMesasAsync()
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Mesa/GetMesaAsync", Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<List<Mesa>>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<List<Local>> GetLocalsAsync()
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Mesa/GetLocalMesaAsync", Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<List<Local>>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<List<GrupoProduto>> GetGruposAsync(string ds_produto)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Produto/GetGrupoAsync?ds_produto=" + ds_produto, Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<List<GrupoProduto>>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<List<Produto>> GetProdutosAsync()
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Produto/GetProdutoAsync", Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<List<Produto>>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<List<Adicional>> GetAdicionaisAsync(string Cd_produto)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Adicional/GetAdicionalAsync?Cd_produto=" + Cd_produto.Trim(), Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<List<Adicional>>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<List<ItemExcluir>> GetItensExcluirAsync(string Cd_grupo)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/ItemExcluir/GetItemExcluirAsync?Cd_grupo=" + Cd_grupo.Trim(), Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<List<ItemExcluir>>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<List<Ingredientes>> GetIngredientesAsync(string Cd_produto)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Ingrediente/GetIngredienteAsync?Cd_produto=" + Cd_produto.Trim(), Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<List<Ingredientes>>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<List<PontoCarne>> GetPontoCarnesAsync(string Cd_produto)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/PontoCarne/GetPontoCarneAsync?Cd_produto=" + Cd_produto.Trim(), Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<List<PontoCarne>>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<List<Sabor>> GetSaboresAsync(string Cd_produto)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Sabor/GetSaborAsync?Cd_produto=" + Cd_produto.Trim(), Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<List<Sabor>>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<List<Observacoes>> GetObservacoesAsync(string Cd_produto)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Observacoes/GetObsAsync?Cd_produto=" + Cd_produto.Trim(), Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<List<Observacoes>>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<List<ItemVenda>> GetExtratoMesaAsync(string Id_local, string Id_mesa)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/ItemVenda/GetExtratoMesaAsync?Id_local=" + Id_local.Trim() + "&Id_mesa=" + Id_mesa.Trim(), Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<List<ItemVenda>>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<List<ItemVenda>> GetExtratoCartaoAsync(string Nr_cartao)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/ItemVenda/GetExtratoCartaoAsync?Nr_cartao=" + Nr_cartao, Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<List<ItemVenda>>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<bool> GravarComandaMesaAsync(string Id_local, 
                                                              string Id_mesa,
                                                              List<ItemVenda> items)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/ItemVenda/GravarComandaMesaAsync?Id_local=" + Id_local + "&Id_mesa=" + Id_mesa, Method.Post);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())))
                        .AddJsonBody<List<ItemVenda>>(items);
                    RestResponse response = await client.ExecuteAsync(request);
                    return response.IsSuccessful;
                }
            }
            catch { return false; }
        }
        public static async Task<bool> GravarComandaCartaoAsync(string Nr_cartao,
                                                                List<ItemVenda> items)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/ItemVenda/GravarComandaCartaoAsync?Nr_cartao=" + Nr_cartao, Method.Post);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())))
                        .AddJsonBody<List<ItemVenda>>(items);
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return true;
                    else throw new Exception(JsonConvert.DeserializeObject<string>(response.Content));
                }
            }
            catch(Exception ex) { throw new Exception(ex.Message.Trim()); }
        }
        public static async Task<bool> GravarComandaBalcaoAsync(string ClienteBalcao,
                                                                List<ItemVenda> items)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/ItemVenda/GravarComandaBalcaoAsync?ClienteBalcao=" + ClienteBalcao, Method.Post);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())))
                        .AddJsonBody<List<ItemVenda>>(items);
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<bool>(response.Content);
                    else throw new Exception(JsonConvert.DeserializeObject<string>(response.Content));
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message.Trim()); }
        }
        public static async Task<bool> GravarComandaValeFestaAsync(List<ItemVenda> items)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/ItemVenda/GravarComandaValeFestaAsync", Method.Post);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())))
                        .AddJsonBody<List<ItemVenda>>(items);
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<bool>(response.Content);
                    else throw new Exception(JsonConvert.DeserializeObject<string>(response.Content));
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message.Trim()); }
        }
        public static async Task<Entrega> ApontarEntregaAsync(string id_prevenda)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Entrega/ApontarEntregaAsync?Id_prevenda=" + id_prevenda + "&Cd_entregador=" + App.Garcom.Cd_garcom, Method.Post);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<Entrega>(response.Content);
                    else return null;
                }
            }
            catch { return null; }
        }
        public static async Task<bool> ConcluirEntregaAsync(Entrega entrega)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Entrega/ConcluirEntregaAsync", Method.Post);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())))
                        .AddJsonBody<Entrega>(entrega);
                    RestResponse response = await client.ExecuteAsync(request);
                    return response.IsSuccessful;
                }
            }
            catch { return false; }
        }
        public static async Task<bool> ConsultarCartaoAbertoAsync(int Nr_cartao)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Cartao/ConsultarCartaoAbertoAsync?Nr_cartao=" + Nr_cartao, Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<bool>(response.Content);
                    else throw new Exception(response.Content);
                }
            }
            catch(Exception ex) { throw new Exception(ex.Message); }
        }
        public static async Task<bool> AbrirCartaoAsync(int Nr_cartao,
                                                        string Celular,
                                                        string Nome,
                                                        bool MenorIdade)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Cartao/AbrirCartaoAsync?Nr_cartao=" + Nr_cartao +
                                                  "&Celular=" + Celular + "&Nome=" + Nome + "&MenorIdade=" + MenorIdade, Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<bool>(response.Content);
                    else throw new Exception(response.Content);
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
        }
        public static async Task<string> ConsultaClienteAsync(string Celular)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/Cartao/ConsultaClienteAsync?Celular=" + Celular, Method.Get);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())));
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<string>(response.Content);
                    else return string.Empty;
                }
            }
            catch { return string.Empty; }
        }
        public static async Task<bool> ReceberVendaAsync(RecVenda rec)
        {
            try
            {
                using (var client = new RestClient(App.url_api))
                {
                    var request = new RestRequest("/api/RecVenda/ReceberVendaAsync", Method.Post);
                    request.AddHeader("token", Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero())))
                        .AddJsonBody<RecVenda>(rec);
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        return JsonConvert.DeserializeObject<bool>(response.Content);
                    else throw new Exception(JsonConvert.DeserializeObject<string>(response.Content));
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message.Trim()); }
        }
        #region Stone
        public static async Task<bool> GerarTokenStoneAsync(string chave)
        {
            try
            {
                using (var client = new RestClient("http://cloud.lbsystemsoftware.com.br:8080/stone"))
                {
                    var request = new RestRequest("/api/LbStone/AutenticarAcessoAsync", Method.Get);
                    request.AddHeader("Token", chave);
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                    {
                        TokenStone = JsonConvert.DeserializeObject<TokenStone>(response.Content);
                        return true;
                    }
                    else return false;

                }
            }
            catch { return false; }
        }
        public static async Task<RetornoStone> CriarPreTransacaoAsync(PreTransacao preTransacao)
        {
            RetornoStone ret = new RetornoStone();
            try
            {
                using (var client = new RestClient("http://cloud.lbsystemsoftware.com.br:8080/stone"))
                {
                    var request = new RestRequest("/api/LbStone/CriarPreTransacaoAsync", Method.Post);
                    request.AddHeader("Authorization", "Bearer " + TokenStone.token)
                        .AddJsonBody<PreTransacao>(preTransacao);
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                    {
                        ret = JsonConvert.DeserializeObject<RetornoStone>(response.Content);
                        ret.status = 1;
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        ret.status = -1;
                    else ret.status = 0;
                }
            }
            catch { ret.status = 0; }
            return ret;
        }
        public static async Task<RetornoStone> ConsultaUltimoStatusPreTransacaoAsync(PreTransacao preTransacao)
        {
            RetornoStone ret = new RetornoStone();
            try
            {
                using (var client = new RestClient("http://cloud.lbsystemsoftware.com.br:8080/stone"))
                {
                    var request = new RestRequest("/api/LbStone/ConsultaUltimoStatusPreTransacaoAsync", Method.Post);
                    request.AddHeader("Authorization", "Bearer " + TokenStone.token)
                        .AddJsonBody<PreTransacao>(preTransacao);
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                    {
                        ret.msg = JsonConvert.DeserializeObject<string>(response.Content);
                        ret.status = 1;
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        ret.status = -1;
                    else ret.status = 0;
                }
            }
            catch { ret.status = 0; }
            return ret;
        }
        public static async Task<RetornoStone> DesativaPreTransacaoAsync(PreTransacao preTransacao)
        {
            RetornoStone ret = new RetornoStone();
            try
            {
                using (var client = new RestClient("http://cloud.lbsystemsoftware.com.br:8080/stone"))
                {
                    var request = new RestRequest("/api/LbStone/DesativaPreTransacaoAsync", Method.Post);
                    request.AddHeader("Authorization", "Bearer " + TokenStone.token)
                        .AddJsonBody<PreTransacao>(preTransacao);
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                        ret.status = 1;
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        ret.status = -1;
                    else ret.status = 0;
                }
            }
            catch { ret.status = 0; }
            return ret;
        }
        public static async Task<RetornoStone> GetTransacaoAsync(PreTransacao preTransacao)
        {
            RetornoStone ret = new RetornoStone();
            try
            {
                using (var client = new RestClient("http://cloud.lbsystemsoftware.com.br:8080/stone"))
                {
                    var request = new RestRequest("/api/LbStone/GetTransacaoAsync", Method.Post);
                    request.AddHeader("Authorization", "Bearer " + TokenStone.token)
                        .AddJsonBody<PreTransacao>(preTransacao);
                    RestResponse response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                    {
                        ret.transacao = JsonConvert.DeserializeObject<Transacao>(response.Content);
                        ret.status = 1;
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        ret.status = -1;
                    else ret.status = 0;
                }
            }
            catch { ret.status = 0; }
            return ret;
        }
        #endregion
    }
}
