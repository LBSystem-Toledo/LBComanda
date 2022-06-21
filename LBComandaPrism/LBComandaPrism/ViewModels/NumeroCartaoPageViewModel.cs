using Acr.UserDialogs;
using LBComandaPrism.Models;
using LBComandaPrism.Services;
using LBComandaPrism.Utils;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LBComandaPrism.ViewModels
{
    public class NumeroCartaoPageViewModel : ViewModelBase
    {
        bool FecharVenda { get; set; } = false;
        List<ItemVenda> ItensVenda { get; set; }

        string _nr_cartao = string.Empty;
        public string Nr_cartao { get { return _nr_cartao; } set { SetProperty(ref _nr_cartao, value); } }
        bool _enablecartao = true;
        public bool EnableCartao { get { return _enablecartao; } set { SetProperty(ref _enablecartao, value); } }
        string _nr_mesa = string.Empty;
        public string Nr_mesa { get { return _nr_mesa; } set { SetProperty(ref _nr_mesa, value); } }
        bool _visiblemesa = false;
        public bool VisibleMesa { get { return _visiblemesa; } set { SetProperty(ref _visiblemesa, value); } }

        public DelegateCommand ConfirmarCommand { get; }

        private readonly IPageDialogService dialogService;
        public NumeroCartaoPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            :base(navigationService)
        {
            dialogService = pageDialogService;

            ConfirmarCommand = new DelegateCommand(async () =>
            {
                if(string.IsNullOrWhiteSpace(Nr_cartao.SoNumero()))
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Obrigatório informar numero cartão.", "OK");
                    return;
                }
                if(App.Garcom.Nr_cartaorotini > 0 &&
                int.Parse(Nr_cartao.SoNumero()) < App.Garcom.Nr_cartaorotini)
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Cartão informado deve ser maior que <" + App.Garcom.Nr_cartaorotini.ToString() + ">.", "OK");
                    Nr_cartao = string.Empty;
                    return;
                }
                if (App.Garcom.Nr_cartaorotfin > 0 &&
                int.Parse(Nr_cartao.SoNumero()) > App.Garcom.Nr_cartaorotfin)
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Cartão informado deve ser menor que <" + App.Garcom.Nr_cartaorotfin.ToString() + ">.", "OK");
                    Nr_cartao = string.Empty;
                    return;
                }
                if (FecharVenda)
                {
                    try
                    {
                        using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
                        {
                            ItensVenda.ForEach(p => p.Nr_mesacartao = Nr_mesa);
                            var retorno = await DataService.GravarComandaCartaoAsync(Nr_cartao, ItensVenda);
                            if (retorno)
                            {
                                await dialogService.DisplayAlertAsync("Mensagem", "Comanda gravada com sucesso.", "OK");
                                await navigationService.NavigateAsync("/MenuPage/NavigationPage/CardapioPage");
                            }
                            else await dialogService.DisplayAlertAsync("Mensagem", "Erro gravar comanda.", "OK");
                        }
                    }
                    catch { }
                }
                else
                {
                    NavigationParameters p = new NavigationParameters();
                    p.Add("NR_CARTAO", Nr_cartao);
                    if (!string.IsNullOrWhiteSpace(Nr_mesa))
                        p.Add("NR_MESA", Nr_mesa);
                    await navigationService.GoBackAsync(p);
                }
            });
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("CARRINHO"))
            {
                VisibleMesa = App.Garcom.St_mesacartao;
                FecharVenda = true;
                ItensVenda = parameters["CARRINHO"] as List<ItemVenda>;
                if (App.Garcom.LerQRCodeAPP)
                {
                    //Comanda Cartão
                    var scanner = DependencyService.Get<IQrCodeScanningService>();
                    var result = await scanner.ScanAsync();
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        Nr_cartao = result;
                        EnableCartao = false;
                    }
                }
            }
        }
    }
}
