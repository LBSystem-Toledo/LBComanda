using LBComandaPrism.Models;
using LBComandaPrism.Services;
using LBComandaPrism.Utils;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LBComandaPrism.ViewModels
{
    public class StonePageViewModel : ViewModelBase
    {
        static bool loop = true;
        bool Processando { get; set; } = false;
        PreTransacao preTransacao { get; set; }
        static Mesa Mesa { get; set; }
        string Nr_cartao { get; set; }

        private string _d_c;
        public string D_C { get { return _d_c; } set { SetProperty(ref _d_c, value); } }
        private decimal _vl_receber = decimal.Zero;
        public decimal Vl_receber { get { return _vl_receber; } set { SetProperty(ref _vl_receber, value); } }
        private string _mensagem = string.Empty;
        public string Mensagem { get { return _mensagem; } set { SetProperty(ref _mensagem, value); } }

        public DelegateCommand CancelarCommand { get; }

        private readonly IPageDialogService dialogService;
        public StonePageViewModel(INavigationService navigationService, IPageDialogService _dialogService)
            : base(navigationService)
        {
            dialogService = _dialogService;
            CancelarCommand = new DelegateCommand(async () =>
            {
                if (preTransacao != null)
                {
                    try
                    {
                        RetornoStone ret = await DataService.DesativaPreTransacaoAsync(preTransacao);
                        if (ret.status == -1)
                            await DataService.GerarTokenStoneAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero() + ":" + App.Garcom.Stone_id)));
                        else if (ret.status == 1)
                        {
                            Mensagem = "TRANSAÇÃO CANCELADA COM SUCESSO.";
                            await Task.Delay(3000);
                            Device.BeginInvokeOnMainThread(async () => await NavigationService.GoBackAsync());
                        }
                        else Mensagem = "ERRO REGISTRAR ESTORNO TRANSAÇÃO.";
                    }
                    catch { Mensagem = "ERRO REGISTRAR ESTORNO TRANSAÇÃO."; }
                }
            });
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            D_C = parameters["D_C"].ToString();
            Vl_receber = decimal.Parse(parameters["VALOR"].ToString());
            Mensagem = "GERANDO PRÉ TRANSAÇÃO";
            if (parameters.ContainsKey("MESA"))
                Mesa = (Mesa)parameters["MESA"];
            if (parameters.ContainsKey("CARTAO"))
                Nr_cartao = parameters["CARTAO"].ToString();
            loop = true;
            await Task.Run(async () =>
            {
                while(loop)
                {
                    try
                    {
                        if (DataService.TokenStone is null)
                            await DataService.GerarTokenStoneAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(App.Garcom.Cnpj.SoNumero() + ":" + App.Garcom.Stone_id)));
                        if (preTransacao is null)
                        {
                            PreTransacao p = new PreTransacao();
                            p.amount = int.Parse(Vl_receber.ToString("N2", new System.Globalization.CultureInfo("pt-BR", true)).SoNumero());
                            p.establishment_id = App.Garcom.Stone_id;
                            Pagamento pag = new Pagamento();
                            pag.type = D_C.Trim().ToUpper().Equals("DEBITO") ? 1 : 2;
                            p.payment = pag;
                            RetornoStone ret = await DataService.CriarPreTransacaoAsync(p);
                            if (ret.status == 1)
                            {
                                Mensagem = "TOQUE NO LOGO OU APERTE A TECLA VERDE PARA BUSCAR";
                                preTransacao = ret.pre_transaction;
                            }
                        }
                        else if (Processando)
                        {
                            RetornoStone ret = await DataService.GetTransacaoAsync(preTransacao);
                            if (ret.transacao != null)
                                if (ret.transacao.status.Trim().ToUpper().Equals("PROCESSED"))
                                    try
                                    {
                                        await DataService.ReceberVendaAsync(
                                            new RecVenda
                                            {
                                                Cd_garcom = App.Garcom.Cd_garcom,
                                                D_C = D_C.Trim().ToUpper().Equals("DEBITO") ? "D" : "C",
                                                Bandeira = ret.transacao.card_brand.Trim().ToUpper(),
                                                Nsu = ret.transacao.stone_transaction_id,
                                                Valor = Vl_receber,
                                                Nr_cartao = Nr_cartao,
                                                Id_local = Mesa is null ? 0 : (int)Mesa.Id_local,
                                                Id_mesa = Mesa is null ? 0 : (int)Mesa.Id_mesa
                                            });
                                    }
                                    catch (Exception ex)
                                    {
                                        await dialogService.DisplayAlertAsync("Erro", "Erro ao gravar recebimento no ERP.\r\n" +
                                                                              "Favor dirigir ao caixa e proceder o recebimento no ERP de forma manual.\r\n" +
                                                                              "Erro: " + ex.Message.Trim(), "OK");
                                    }
                                    finally
                                    {
                                        Mensagem = "TRANSAÇÃO APROVADA COM SUCESSO";
                                        await Task.Delay(3000);
                                        Device.BeginInvokeOnMainThread(async()=> await NavigationService.NavigateAsync(new Uri("/MenuPage/NavigationPage/CardapioPage", System.UriKind.Relative)));
                                    }
                                else if (ret.transacao.status.Trim().ToUpper().Equals("CANCELED"))
                                {
                                    Mensagem = "TRANSAÇÃO CANCELADA";
                                    await Task.Delay(3000);
                                    Device.BeginInvokeOnMainThread(async () => await NavigationService.GoBackAsync());
                                }
                                else
                                {
                                    ret = await DataService.ConsultaUltimoStatusPreTransacaoAsync(preTransacao);
                                    if (ret.status == 1)
                                    {
                                        if (ret.msg.Trim().ToUpper().Equals("PROCESSING"))
                                        {
                                            Mensagem = "APROXIME, INSIRA OU PASSE O CARTÃO";
                                            Processando = true;
                                        }
                                        else if (ret.msg.Trim().ToUpper().Equals("UNDONE"))
                                        {
                                            Mensagem = "TRANSAÇÃO CANCELADA PELO USUARIO";
                                            await Task.Delay(3000);
                                            Device.BeginInvokeOnMainThread(async () => await NavigationService.GoBackAsync());
                                        }
                                    }
                                }
                            else
                            {
                                ret = await DataService.ConsultaUltimoStatusPreTransacaoAsync(preTransacao);
                                if (ret.status == 1)
                                {
                                    if (ret.msg.Trim().ToUpper().Equals("PROCESSING"))
                                    {
                                        Mensagem = "APROXIME, INSIRA OU PASSE O CARTÃO";
                                        Processando = true;
                                    }
                                    else if (ret.msg.Trim().ToUpper().Equals("UNDONE"))
                                    {
                                        Mensagem = "TRANSAÇÃO CANCELADA PELO USUARIO";
                                        await Task.Delay(3000);
                                        Device.BeginInvokeOnMainThread(async () => await NavigationService.GoBackAsync());
                                    }
                                }
                            }
                        }
                        else
                        {
                            RetornoStone ret = await DataService.ConsultaUltimoStatusPreTransacaoAsync(preTransacao);
                            if (ret.status == 1)
                            {
                                if (ret.msg.Trim().ToUpper().Equals("PROCESSING"))
                                {
                                    Mensagem = "APROXIME, INSIRA OU PASSE O CARTÃO";
                                    Processando = true;
                                }
                                else if (ret.msg.Trim().ToUpper().Equals("UNDONE"))
                                {
                                    Mensagem = "TRANSAÇÃO CANCELADA PELO USUARIO";
                                    await Task.Delay(3000);
                                    Device.BeginInvokeOnMainThread(async () => await NavigationService.GoBackAsync());
                                }
                            }
                        }
                    }
                    catch { }
                    finally { await Task.Delay(5000); }
                }
            });
        }
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            loop = false;
        }
    }
}
