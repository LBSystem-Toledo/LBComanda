using LBComandaPrism.Services;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Threading.Tasks;

namespace LBComandaPrism.ViewModels
{
    public class AguardandoTokenPageViewModel : ViewModelBase
	{
        private bool _aguardandoliberacao = false;
        public bool Aguardandoliberacao { get { return _aguardandoliberacao; } set { SetProperty(ref _aguardandoliberacao, value); } }
        private bool _tokenliberado = false;
        public bool Tokenliberado { get { return _tokenliberado; } set { SetProperty(ref _tokenliberado, value); } }
        
        private static bool loop = true;

        public DelegateCommand AcessarMapaMesasCommand { get; }

        public AguardandoTokenPageViewModel(INavigationService navigationService)
            :base(navigationService)
        {
            AcessarMapaMesasCommand = new DelegateCommand(async () =>
            {
                await navigationService.NavigateAsync(new Uri("/MenuPage/NavigationPage/CardapioPage", System.UriKind.Relative));
            });
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            Aguardandoliberacao = true;
            loop = true;
            await Task.Run(async () =>
            {
                while (loop)
                {
                    try
                    {
                       var tk = await DataService.ValidarTokenAsync();
                        if (tk != null)
                        {
                            App.Garcom.Token = tk;
                            Aguardandoliberacao = false;
                            Tokenliberado = true;
                        }
                    }
                    catch { }
                    finally
                    { await Task.Delay(15000); }
                }
            });
        }
        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            loop = false;
        }
    }
}
