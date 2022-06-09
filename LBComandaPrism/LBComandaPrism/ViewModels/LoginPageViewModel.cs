using Acr.UserDialogs;
using LBComandaPrism.Interface;
using LBComandaPrism.Models;
using LBComandaPrism.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using Xamarin.Forms;

namespace LBComandaPrism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private string _login = string.Empty;
        public string Login { get { return _login; } set { SetProperty(ref _login, value); } }
        private string _senha = string.Empty;
        public string Senha { get { return _senha; } set { SetProperty(ref _senha, value); } }
        public string _cnpj = string.Empty;
        public string Cnpj { get { return _cnpj; } set { SetProperty(ref _cnpj, value); } }

        public DelegateCommand LoginCommand { get; }
        public DelegateCommand SairCommand { get; }

        private readonly IPageDialogService dialogService;
        public LoginPageViewModel(INavigationService navigationService, IPageDialogService _dialogService)
            : base(navigationService)
        {
            dialogService = _dialogService;
            LoginCommand = new DelegateCommand(async () =>
            {
                if (string.IsNullOrWhiteSpace(Login))
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Obrigatório informar LOGIN.", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Senha))
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Obrigatório informar SENHA.", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Cnpj))
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Obrigatório informar CNPJ.", "OK");
                    return;
                }
                using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Clear))
                {
                    Garcom garcom = await DataService.ValidarLoginAsync(Login, Senha, Cnpj);
                    if (garcom == null)
                    {
                        await dialogService.DisplayAlertAsync("Mensagem", "Não foi possivel localizar GARÇOM para LOGIN e SENHA informado.", "OK");
                        return;
                    }
                    await PCLHelper.WriteTextAllAsync("LoginGarcom", Login + ":" + Senha + ":" + Cnpj);
                    App.Garcom = garcom;
                    App.Garcom.Cnpj = Cnpj;
                    App.Garcom.Login = Login;
                    if (garcom.ExigirTokenApp && garcom.Token == null)
                        await navigationService.NavigateAsync("AguardandoTokenPage");
                    else if(App.Garcom.ST_Entregador)
                        await navigationService.NavigateAsync(new Uri("/MenuPage/NavigationPage/EntregasPage", System.UriKind.Relative));
                    else await NavigationService.NavigateAsync(new Uri("/MenuPage/NavigationPage/CardapioPage", System.UriKind.Relative));
                }
            });
            SairCommand = new DelegateCommand(() => DependencyService.Get<ICloseApplication>()?.closeApplication());
        }
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (await PCLHelper.ArquivoExisteAsync("LoginGarcom"))
            {
                string aux = await PCLHelper.ReadAllTextAsync("LoginGarcom");
                string[] vetor = aux.Split(new char[] { ':' });
                if (vetor.Length > 0)
                {
                    Login = aux.Split(new char[] { ':' })[0];
                    if(vetor.Length > 1)
                        Senha = aux.Split(new char[] { ':' })[1];
                    if(vetor.Length > 2)
                        Cnpj = aux.Split(new char[] { ':' })[2];
                }
            }
        }
    }
}
