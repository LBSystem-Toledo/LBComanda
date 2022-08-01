using Acr.UserDialogs;
using LBComandaPrism.Models;
using LBComandaPrism.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms.Internals;

namespace LBComandaPrism.ViewModels
{
    public class DetalheItemPageViewModel : ViewModelBase
    {
        private Produto _produto = null;
        public Produto Produto { get { return _produto; } set { SetProperty(ref _produto, value); } }

        private ObservableCollection<Ingredientes> _ingredientes;
        public ObservableCollection<Ingredientes> Ingredientes { get { return _ingredientes; } set { SetProperty(ref _ingredientes, value); } }
        private ObservableCollection<ItemExcluir> _itensexcluir;
        public ObservableCollection<ItemExcluir> ItensExcluir { get { return _itensexcluir; } set { SetProperty(ref _itensexcluir, value); } }
        private ObservableCollection<Adicional> _adicionais;
        public ObservableCollection<Adicional> Adicionais { get { return _adicionais; } set { SetProperty(ref _adicionais, value); } }
        private ObservableCollection<PontoCarne> _pontocarne;
        public ObservableCollection<PontoCarne> PontoCarne { get { return _pontocarne; } set { SetProperty(ref _pontocarne, value); } }
        private ObservableCollection<Sabor> _sabores;
        public ObservableCollection<Sabor> Sabores { get { return _sabores; } set { SetProperty(ref _sabores, value); } }
        private ObservableCollection<Observacoes> _observacoes;
        public ObservableCollection<Observacoes> Observacoes { get { return _observacoes; } set { SetProperty(ref _observacoes, value); } }
        private PontoCarne _pontoselecionado;
        public PontoCarne PontoSelecionado { get { return _pontoselecionado; } set { SetProperty(ref _pontoselecionado, value); } }
        public string ObsItem { get; set; } = string.Empty;
        private bool _existeingredientes = false;
        public bool ExisteIngredientes { get { return _existeingredientes; } set { SetProperty(ref _existeingredientes, value); } }
        private bool _existeitensexcluir = false;
        public bool ExisteItensExcluir { get { return _existeitensexcluir; } set { SetProperty(ref _existeitensexcluir, value); } }
        private bool _existeadicionais = false;
        public bool ExisteAdicionais
        {
            get { return _existeadicionais; }
            set { SetProperty(ref _existeadicionais, value); }
        }
        private bool _existepontocarne = false;
        public bool ExistePontoCarne
        {
            get { return _existepontocarne; }
            set { SetProperty(ref _existepontocarne, value); }
        }
        private bool _existesabores = false;
        public bool ExisteSabores { get { return _existesabores; } set { SetProperty(ref _existesabores, value); } }
        private bool _existeobs = false;
        public bool ExisteObs { get { return _existeobs; } set { SetProperty(ref _existeobs, value); } }

        public DelegateCommand ConfirmarCommand { get; }
        public DelegateCommand CancelarCommand { get; }
        public DelegateCommand LimparPontoCarneCommand { get; }

        public readonly IPageDialogService dialogService;
        public DetalheItemPageViewModel(INavigationService navigationService, IPageDialogService _dialogService)
            : base(navigationService)
        {
            dialogService = _dialogService;
            ConfirmarCommand = new DelegateCommand(async() =>
            {
                ItemVenda it = new ItemVenda();
                it.Cd_produto = Produto.Cd_produto;
                it.Ds_produto = Produto.Ds_produto;
                it.Quantidade = Produto.Quantidade;
                it.PrecoVenda = Produto.PrecoVenda;
                it.Obs = ObsItem;
                Ingredientes?.ForEach(p =>
                {
                    if (p.Incluido)
                        it.Ingredientes.Add(p);
                    else it.IngredientesDel.Add(p);
                });
                ItensExcluir?.ForEach(p =>
                {
                    if (p.Selecionado)
                        it.ItensExcluir.Add(p);
                });
                Adicionais?.ForEach(p =>
                {
                    if (p.Selecionado)
                        it.Adicionais.Add(p);
                });
                Sabores?.ForEach(p =>
                {
                    if (p.Incluido)
                        it.Sabores.Add(p);
                });
                Observacoes?.ForEach(p =>
                {
                    if (p.Marcar)
                        it.Obs += (string.IsNullOrWhiteSpace(it.Obs) ? string.Empty : "\r\n") + p.Obs.Trim();
                });
                if (PontoSelecionado != null)
                    it.PontoCarne = PontoSelecionado.Ds_ponto;
                if(Produto.PontoCarne && string.IsNullOrWhiteSpace(it.PontoCarne))
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Produto obriga informar ponto carne.", "OK");
                    return;
                }
                if(Produto.Sabor && it.Sabores.Count.Equals(0))
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Produto obriga informar sabor.", "OK");
                    return;
                }
                var param = new NavigationParameters();
                param.Add("item", it);
                await navigationService.GoBackAsync(param);
            });
            CancelarCommand = new DelegateCommand(() => navigationService.GoBackAsync());
            LimparPontoCarneCommand = new DelegateCommand(() => { PontoSelecionado = null; });
        }
        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
            {
                Produto = (Produto)parameters["produto"];
                List<Ingredientes> ingredientes = await DataService.GetIngredientesAsync(Produto.Cd_produto);
                if (ingredientes.Count > 0)
                {
                    Ingredientes = new ObservableCollection<Ingredientes>(ingredientes);
                    ExisteIngredientes = true;
                }
                List<ItemExcluir> itensexcluir = await DataService.GetItensExcluirAsync(Produto.Cd_grupo);
                if(itensexcluir.Count > 0)
                {
                    ItensExcluir = new ObservableCollection<ItemExcluir>(itensexcluir);
                    ExisteItensExcluir = true;
                }
                List<Adicional> adicionais = await DataService.GetAdicionaisAsync(Produto.Cd_produto);
                if (adicionais.Count > 0)
                {
                    Adicionais = new ObservableCollection<Adicional>(adicionais);
                    ExisteAdicionais = true;
                }
                List<PontoCarne> pc = await DataService.GetPontoCarnesAsync(Produto.Cd_produto);
                if (pc.Count > 0)
                {
                    PontoCarne = new ObservableCollection<PontoCarne>(pc);
                    ExistePontoCarne = true;
                }
                List<Sabor> sb = await DataService.GetSaboresAsync(Produto.Cd_produto);
                if (sb?.Count > 0)
                {
                    Sabores = new ObservableCollection<Sabor>(sb);
                    ExisteSabores = true;
                }
                List<Observacoes> obs = await DataService.GetObservacoesAsync(Produto.Cd_produto);
                if (obs?.Count > 0)
                {
                    Observacoes = new ObservableCollection<Observacoes>(obs);
                    ExisteObs = true;
                }
            }
        }
    }
}
