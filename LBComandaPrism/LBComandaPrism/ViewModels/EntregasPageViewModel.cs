using Acr.UserDialogs;
using LBComandaPrism.Models;
using LBComandaPrism.Services;
using LBComandaPrism.Utils;
using Plugin.Connectivity;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace LBComandaPrism.ViewModels
{
    public class EntregasPageViewModel : ViewModelBase
    {
        private ObservableCollection<Entrega> _entregas;
        public ObservableCollection<Entrega> Entregas { get { return _entregas; } set { SetProperty(ref _entregas, value); } }

        public DelegateCommand LerQRCodeCommand { get; }
        public DelegateCommand<Entrega> ConfirmarEntregaCommand { get; }

        private readonly IPageDialogService dialogService;
        public EntregasPageViewModel(INavigationService navigationService, IPageDialogService _dialogService)
            :base(navigationService)
        {
            dialogService = _dialogService;

            LerQRCodeCommand = new DelegateCommand(async() =>
            {
                using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
                {
                    string url = App.url_api.Substring(0, App.url_api.LastIndexOf(':'));
                    string porta = App.url_api.Substring(App.url_api.LastIndexOf(':') + 1, App.url_api.Length - App.url_api.LastIndexOf(':') - 1);
                    bool ret = await CrossConnectivity.Current.IsRemoteReachable(url, int.Parse(porta.SoNumero()));
                    if (ret)
                    {
                        var scanner = DependencyService.Get<IQrCodeScanningService>();
                        var result = await scanner.ScanAsync();
                        if (!string.IsNullOrWhiteSpace(result))
                            try
                            {
                                Entrega entrega = await DataService.ApontarEntregaAsync(result);
                                if (entrega != null)
                                {
                                    await App.Database.GravarEntregaAsync(entrega);
                                    await dialogService.DisplayAlertAsync("Mensagem", "Pedido incluido com sucesso.", "OK");
                                    Entregas = new ObservableCollection<Entrega>(await App.Database.GetEntregasAsync());
                                }
                                else await dialogService.DisplayAlertAsync("Mensagem", "Pedido não localizado.", "OK");
                            }
                            catch { }
                    }
                    else await dialogService.DisplayAlertAsync("Mensagem", "Sem conexão com o serviço.", "OK");
                }
            });
            ConfirmarEntregaCommand = new DelegateCommand<Entrega>(async (Entrega e) =>
            {
                if (e != null)
                    using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
                    {
                        e.Dt_entregadelivery = DateTime.Now;
                        e.Entregue = true;
                        await App.Database.GravarEntregaAsync(e);
                        await dialogService.DisplayAlertAsync("Mensagem", "Entrega confirmada com sucesso.", "OK");
                        Entregas = new ObservableCollection<Entrega>(await App.Database.GetEntregasAsync());
                    }
            });
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
            {
                Entregas = new ObservableCollection<Entrega>(await App.Database.GetEntregasAsync());
            }
        }
    }
}
