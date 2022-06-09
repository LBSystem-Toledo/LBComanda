using Acr.UserDialogs;
using LBComandaPrism.Models;
using LBComandaPrism.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace LBComandaPrism.ViewModels
{
    public class ExtratoPageViewModel : ViewModelBase
    {
        private Mesa _mesa;
        public Mesa Mesa { get { return _mesa; } set { SetProperty(ref _mesa, value); } }
        private bool _recvenda = false;
        public bool RecVenda { get { return _recvenda; } set { SetProperty(ref _recvenda, value); } }
        private bool _vermesa = false;
        public bool VerMesa { get { return _vermesa; } set { SetProperty(ref _vermesa, value); } }
        private bool _vercartao = false;
        public bool VerCartao { get { return _vercartao; } set { SetProperty(ref _vercartao, value); } }
        private string _nr_cartao = string.Empty;
        public string Nr_cartao { get { return _nr_cartao; } set { SetProperty(ref _nr_cartao, value); } }
        private decimal _total = decimal.Zero;
        public decimal Total { get { return _total; } set { SetProperty(ref _total, value); } }

        public ObservableCollection<ItemVenda> ItensVenda { get; set; } = new ObservableCollection<ItemVenda>();

        public DelegateCommand VoltarCommand { get; }
        public DelegateCommand CreditoCommand { get; }
        public DelegateCommand DebitoCommand { get; }

        private readonly IPageDialogService dialogService;
        public ExtratoPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            :base(navigationService)
        {
            dialogService = pageDialogService;

            RecVenda = !string.IsNullOrWhiteSpace(App.Garcom.Stone_id);
            VoltarCommand = new DelegateCommand(async () => await navigationService.GoBackAsync());
            CreditoCommand = new DelegateCommand(async () =>
            {
                if (Total > decimal.Zero)
                {
                    NavigationParameters p = new NavigationParameters();
                    p.Add("D_C", "CREDITO");
                    p.Add("VALOR", Total);
                    if (!string.IsNullOrWhiteSpace(Nr_cartao))
                        p.Add("CARTAO", Nr_cartao);
                    else p.Add("MESA", Mesa);
                    await navigationService.NavigateAsync("StonePage", p, useModalNavigation: true);
                }
                else await dialogService.DisplayAlertAsync("Mensagem", "Não existe valor a receber.", "OK");
            });
            DebitoCommand = new DelegateCommand(async () =>
            {
                if (Total > decimal.Zero)
                {
                    NavigationParameters p = new NavigationParameters();
                    p.Add("D_C", "DEBITO");
                    p.Add("VALOR", Total);
                    if (!string.IsNullOrWhiteSpace(Nr_cartao))
                        p.Add("CARTAO", Nr_cartao);
                    else p.Add("MESA", Mesa);
                    await navigationService.NavigateAsync("StonePage", p, useModalNavigation: true);
                }
                else await dialogService.DisplayAlertAsync("Mensagem", "Não existe valor a receber.", "OK");
            });
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters != null)
            {
                if (parameters.ContainsKey("mesa"))
                {
                    using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Clear))
                    {
                        Mesa = (Mesa)parameters["mesa"];
                        List<ItemVenda> itens = await DataService.GetExtratoMesaAsync(((int)Mesa.Id_local).ToString(), ((int)Mesa.Id_mesa).ToString());
                        itens.ForEach(p => ItensVenda.Add(p));
                        Total = itens.Sum(p => p.ValorVenda);
                        VerMesa = true;
                        VerCartao = false;
                    }
                }
                else if(parameters.ContainsKey("ST_CARTAO"))
                {
                    if (App.Garcom.LerQRCodeAPP)
                    {
                        //Comanda Cartão
                        var scanner = DependencyService.Get<IQrCodeScanningService>();
                        var result = await scanner.ScanAsync();
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            Nr_cartao = parameters["cartao"].ToString();
                            List<ItemVenda> itens = await DataService.GetExtratoCartaoAsync(Nr_cartao);
                            itens.ForEach(p => ItensVenda.Add(p));
                            Total = itens.Sum(p => p.ValorVenda);
                            VerMesa = false;
                            VerCartao = true;
                        }
                        else await NavigationService.NavigateAsync("NumeroCartaoPage");
                    }
                    else await NavigationService.NavigateAsync("NumeroCartaoPage");
                    
                }
                else if(parameters.ContainsKey("NR_CARTAO"))
                {
                    Nr_cartao = parameters["NR_CARTAO"].ToString();
                    List<ItemVenda> itens = await DataService.GetExtratoCartaoAsync(Nr_cartao);
                    itens.ForEach(p => ItensVenda.Add(p));
                    Total = itens.Sum(p => p.ValorVenda);
                    VerMesa = false;
                    VerCartao = true;
                }
            }
        }
    }
}
