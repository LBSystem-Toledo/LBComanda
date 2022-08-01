using Prism;
using Prism.Ioc;
using LBComandaPrism.ViewModels;
using LBComandaPrism.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using LBComandaPrism.Models;
using LBComandaPrism.DataBase;
using Matcha.BackgroundService;
using LBComandaPrism.Services;
using Xamarin.Forms;

namespace LBComandaPrism
{
    public partial class App
    {
        static AcessoDB database;
        public static AcessoDB Database
        {
            get
            {
                if (database == null)
                    database = new AcessoDB(Xamarin.Essentials.FileSystem.AppDataDirectory);
                return database;
            }
        }

        public static Garcom Garcom { get; set; } = new Garcom();

        #if (DEBUG)
            public const string url_api = "http://192.168.1.106:45455";
            //public const string url_api = "http://177.107.125.182:33209/reshomolog";
        #else
            //public const string url_api = "http://177.107.125.182:33209/restaurante";
            //public const string url_api = "http://177.107.125.182:33209/reshomolog";
            public const string url_api = "http://192.168.0.160/comanda";
        #endif

        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            DevExpress.XamarinForms.CollectionView.Initializer.Init();
            DevExpress.XamarinForms.Editors.Initializer.Init();
            InitializeComponent();
            await NavigationService.NavigateAsync("LoginPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MenuPage, MenuPageViewModel>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<CardapioPage, CardapioPageViewModel>();
            containerRegistry.RegisterForNavigation<DetalheItemPage, DetalheItemPageViewModel>();
            containerRegistry.RegisterForNavigation<CarrinhoPage, CarrinhoPageViewModel>();
            containerRegistry.RegisterForNavigation<MesaPage, MesaPageViewModel>();
            containerRegistry.RegisterForNavigation<ExtratoPage, ExtratoPageViewModel>();
            containerRegistry.RegisterForNavigation<MapaMesaPage, MapaMesaPageViewModel>();
            containerRegistry.RegisterForNavigation<AguardandoTokenPage, AguardandoTokenPageViewModel>();
            containerRegistry.RegisterForNavigation<EntregasPage, EntregasPageViewModel>();

            containerRegistry.RegisterForNavigation<AbrirFecharCartaoPage, AbrirFecharCartaoPageViewModel>();
            containerRegistry.RegisterForNavigation<StonePage, StonePageViewModel>();
            containerRegistry.RegisterForNavigation<NumeroCartaoPage, NumeroCartaoPageViewModel>();
        }

        protected override void OnStart()
        {
            //Registra
            BackgroundAggregatorService.Add(() => new BackgroundService(60));
            //Inicia
            BackgroundAggregatorService.StartBackgroundService();
        }

        protected override void OnSleep()
        {
            //Para Servico
            BackgroundAggregatorService.StopBackgroundService();
        }

        protected override void OnResume()
        {
            //Inicia novamente
            BackgroundAggregatorService.StartBackgroundService();
        }
    }
}
