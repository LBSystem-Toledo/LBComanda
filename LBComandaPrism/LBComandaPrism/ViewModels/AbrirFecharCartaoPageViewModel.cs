using Acr.UserDialogs;
using LBComandaPrism.Services;
using LBComandaPrism.Utils;
using Plugin.Connectivity;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using Xamarin.Forms;

namespace LBComandaPrism.ViewModels
{
    public class AbrirFecharCartaoPageViewModel : ViewModelBase
    {
        private string _nr_cartao = string.Empty;
        public string Nr_cartao { get { return _nr_cartao; } set { SetProperty(ref _nr_cartao, value); } }
        private string _celular = string.Empty;
        public string Celular { get { return _celular; } set { SetProperty(ref _celular, value); } }
        private string _nome = string.Empty;
        public string Nome { get { return _nome; } set { SetProperty(ref _nome, value); } }
        private bool _menoridade = false;
        public bool Menoridade { get { return _menoridade; } set { SetProperty(ref _menoridade, value); } }

        public DelegateCommand LerQRCodeCommand { get; }
        public DelegateCommand SalvarCommand { get; }
        public DelegateCommand ConsultarCartaoCommand { get; }
        public DelegateCommand ConsultaClienteCommand { get; }

        private readonly IPageDialogService dialogService;
        public AbrirFecharCartaoPageViewModel(INavigationService navigationService, IPageDialogService _dialogService)
            :base(navigationService)
        {
            dialogService = _dialogService;

            LerQRCodeCommand = new DelegateCommand(async () =>
            {
                using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
                {
                    var scanner = DependencyService.Get<IQrCodeScanningService>();
                    var result = await scanner.ScanAsync();
                    if (!string.IsNullOrWhiteSpace(result))
                        Nr_cartao = result;
                }
            });

            SalvarCommand = new DelegateCommand(async () =>
            {
                if(string.IsNullOrWhiteSpace(Nr_cartao.SoNumero()))
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Obrigatório informar numero valido para cartão", "OK");
                    return;
                }
                if(App.Garcom.Nr_cartaorotini > 0 && App.Garcom.Nr_cartaorotini > int.Parse(Nr_cartao.SoNumero()))
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Nº cartão <" + App.Garcom.Nr_cartaorotini + "> é o minimo da faixa de cartão rotativo.", "OK");
                    return;
                }
                if (App.Garcom.Nr_cartaorotfin > 0 && App.Garcom.Nr_cartaorotfin < int.Parse(Nr_cartao.SoNumero()))
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Nº cartão <" + App.Garcom.Nr_cartaorotfin + "> é o máximo da faixa de cartão rotativo.", "OK");
                    return;
                }
                try
                {
                    using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
                    {
                        if (await DataService.AbrirCartaoAsync(int.Parse(Nr_cartao), Celular, Nome, Menoridade))
                        {
                            await dialogService.DisplayAlertAsync("Mensagem", "Cartão ABERTO com SUCESSO.", "OK");
                            Nr_cartao = string.Empty;
                            Celular = string.Empty;
                            Nome = string.Empty;
                            Menoridade = false;
                        }
                        else await dialogService.DisplayAlertAsync("Mensagem", "Cartão <" + Nr_cartao + "> já se encontra ABERTO.", "OK");
                    }
                }
                catch (Exception ex) { await dialogService.DisplayAlertAsync("Erro", ex.Message.Trim(), "OK"); }
            });

            ConsultarCartaoCommand = new DelegateCommand(async () =>
            {
                using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
                {
                    var scanner = DependencyService.Get<IQrCodeScanningService>();
                    var result = await scanner.ScanAsync();
                    if (!string.IsNullOrWhiteSpace(result))
                        try
                        {
                            if (await DataService.ConsultarCartaoAbertoAsync(int.Parse(result)))
                                await dialogService.DisplayAlertAsync("Mensagem", "Cartão Nº" + result + " está LIBERADO.", "OK");
                            else await dialogService.DisplayAlertAsync("Mensagem", "Cartão Nº" + result + " está ABERTO.\r\n" +
                                                                        "Favor dirigir-se ao caixa.", "OK");
                        }
                        catch(Exception ex) { await dialogService.DisplayAlertAsync("Erro", ex.Message.Trim(), "OK"); }
                }
            });

            ConsultaClienteCommand = new DelegateCommand(async () =>
            {
                if(Celular.SoNumero().Length >= 8)
                    using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
                    {
                        try
                        {
                            Nome = await DataService.ConsultaClienteAsync(Celular);
                        }
                        catch { }
                    }
            });
        }
    }
}
