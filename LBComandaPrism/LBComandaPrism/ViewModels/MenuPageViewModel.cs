using Acr.UserDialogs;
using LBComandaPrism.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace LBComandaPrism.ViewModels
{
    public class MenuPageViewModel : ViewModelBase
    {
        private bool _perfilentregador = false;
        public bool Perfilentregador { get { return _perfilentregador; } set { SetProperty(ref _perfilentregador, value); } }
        private bool _perfilmesa = false;
        public bool Perfilmesa { get { return _perfilmesa; } set { SetProperty(ref _perfilmesa, value); } }
        private bool _perfilcartao = false;
        public bool Perfilcartao { get { return _perfilcartao; } set { SetProperty(ref _perfilcartao, value); } }

        private string _login = string.Empty;
        public string Login { get { return _login.ToUpper(); } set { SetProperty(ref _login, value); } }

        public DelegateCommand AbrirFecharCommand { get; }
        public DelegateCommand CardapioCommand { get; }
        public DelegateCommand AtendimentoMesaCommand { get; }
        public DelegateCommand ExtratoCartaoCommand { get; }
        public DelegateCommand ApontarEntregasCommand { get; }

        private readonly IPageDialogService dialogService;
        public MenuPageViewModel(INavigationService navigationService, IPageDialogService _dialogService)
            :base(navigationService)
        {
            dialogService = _dialogService;
            if (App.Garcom != null)
            {
                Login = App.Garcom.Login;
                Perfilentregador = App.Garcom.ST_Entregador;
                Perfilmesa = !App.Garcom.ST_Entregador && App.Garcom.Tp_cartao.Trim().Equals("1");
                Perfilcartao = !App.Garcom.ST_Entregador && App.Garcom.Tp_cartao.Trim().Equals("0");
            }

            AbrirFecharCommand = new DelegateCommand(async () => await navigationService.NavigateAsync("/MenuPage/NavigationPage/AbrirFecharCartaoPage"));
            CardapioCommand = new DelegateCommand(async () => await navigationService.NavigateAsync("/MenuPage/NavigationPage/CardapioPage"));
            AtendimentoMesaCommand = new DelegateCommand(async () => await navigationService.NavigateAsync("/MenuPage/NavigationPage/MapaMesaPage"));
            ApontarEntregasCommand = new DelegateCommand(async () => await navigationService.NavigateAsync("/MenuPage/NavigationPage/EntregasPage"));
            ExtratoCartaoCommand = new DelegateCommand(async () =>
            {
                NavigationParameters p = new NavigationParameters();
                p.Add("ST_CARTAO", true);
                await navigationService.NavigateAsync("ExtratoPage", p, useModalNavigation: true);
            });
        }
    }
}
