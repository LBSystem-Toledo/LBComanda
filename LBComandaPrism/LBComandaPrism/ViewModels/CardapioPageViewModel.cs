using Acr.UserDialogs;
using LBComandaPrism.Models;
using LBComandaPrism.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace LBComandaPrism.ViewModels
{
    public class CardapioPageViewModel : ViewModelBase
    {
        private static bool loop = true;

        private string _filtroproduto = string.Empty;
        public string Filtroproduto { get { return _filtroproduto; } set { SetProperty(ref _filtroproduto, value); } }
        private bool _solicitartoken = false;
        public bool Solicitartoken { get { return _solicitartoken; } set { SetProperty(ref _solicitartoken, value); } }
        private bool _aguardandoliberacao = false;
        public bool Aguardandoliberacao { get { return _aguardandoliberacao; } set { SetProperty(ref _aguardandoliberacao, value); } }
        private bool _cardapio = true;
        public bool Cardapio { get { return _cardapio; } set { SetProperty(ref _cardapio, value); } }

        private List<Produto> ProdutosOrigem { get; set; } = new List<Produto>();
        
        private GrupoProduto _grupocorrente;
        public GrupoProduto Grupocorrente 
        { 
            get { return _grupocorrente; } 
            set 
            { 
                SetProperty(ref _grupocorrente, value); 
                if(value != null)
                {
                    if (string.IsNullOrWhiteSpace(value.Cd_grupo) &&
                        string.IsNullOrWhiteSpace(Filtroproduto))
                        ProdutosOC = new ObservableCollection<Produto>(ProdutosOrigem);
                    else if (string.IsNullOrWhiteSpace(value.Cd_grupo) &&
                        !string.IsNullOrWhiteSpace(Filtroproduto))
                        ProdutosOC = new ObservableCollection<Produto>(ProdutosOrigem.FindAll(p => p.Ds_produto.Contains(Filtroproduto.ToUpper())));
                    else if (!string.IsNullOrWhiteSpace(value.Cd_grupo) &&
                        string.IsNullOrWhiteSpace(Filtroproduto))
                        ProdutosOC = new ObservableCollection<Produto>(ProdutosOrigem.FindAll(p => p.Cd_grupo == value.Cd_grupo));
                    else ProdutosOC = new ObservableCollection<Produto>(ProdutosOrigem.FindAll(p => p.Cd_grupo == value.Cd_grupo &&
                                                                                                    p.Ds_produto.Contains(Filtroproduto.ToUpper())));
                }
            } 
        }
        private ObservableCollection<GrupoProduto> _grupos;
        public ObservableCollection<GrupoProduto> Grupos { get { return _grupos; } set { SetProperty(ref _grupos, value); } }
        private ObservableCollection<Produto> _produtos;
        public ObservableCollection<Produto> ProdutosOC { get { return _produtos; } set { SetProperty(ref _produtos, value); } }
        public ObservableCollection<ItemVenda> Sacola { get; set; } = new ObservableCollection<ItemVenda>();
        private bool _existesacola = false;
        public bool ExisteSacola
        {
            get { return _existesacola; }
            set { SetProperty(ref _existesacola, value); }
        }
        private int _itenssacola = 0;
        public int ItensSacola
        {
            get { return _itenssacola; }
            set { SetProperty(ref _itenssacola, value); }
        }
        private decimal _valorsacola = decimal.Zero;
        public decimal ValorSacola
        {
            get { return _valorsacola; }
            set { SetProperty(ref _valorsacola, value); }
        }

        public DelegateCommand<Produto> ExcluirItemCommand { get; }
        public DelegateCommand<Produto> DetalheItemCommand { get; }
        public DelegateCommand VisualizarCarrinhoCommand { get; }
        public DelegateCommand VoltarCommand { get; }
        public DelegateCommand BuscarCommand { get; }
        public DelegateCommand<Produto> AddItemCompra { get; }
        public DelegateCommand<Produto> DelItemCompra { get; }
        public DelegateCommand SolicitarTokenCommand { get; }

        private readonly IPageDialogService dialogService;
        public CardapioPageViewModel(INavigationService navigationService, IPageDialogService _dialogService)
            : base(navigationService)
        {
            dialogService = _dialogService;

            ExcluirItemCommand = new DelegateCommand<Produto>(async (Produto prod) =>
            {
                if(prod != null)
                {
                    var retorno = await dialogService.DisplayAlertAsync("Pergunta", "Confirma exclusão do item <" + prod.Ds_produto.Trim() + ">?", "SIM", "NÃO");
                    if(retorno)
                    {
                        Sacola.ToList().FindAll(p => p.Cd_produto.Trim().Equals(prod.Cd_produto.Trim()))
                        .ForEach(p => Sacola.Remove(p));
                        ExisteSacola = Sacola.Count > 0;
                        ItensSacola = (int)Sacola.Sum(p => p.Quantidade);
                        ValorSacola = Sacola.Sum(p => p.ValorVenda + p.Adicionais.Sum(v => v.PrecoVenda));
                        prod.ItemVendido = false;
                        prod.Quantidade = 1;
                    }
                }
            });

            DetalheItemCommand = new DelegateCommand<Produto>(async (Produto prod) =>
            {
                if (prod != null)
                {
                    if(prod.Bloqueado)
                    {
                        await dialogService.DisplayAlertAsync("Mensagem", "Produto BLOQUEADO para venda.", "OK");
                        return;
                    }
                    bool lancar = true;
                    if (Sacola.ToList().Exists(x => x.Cd_produto == prod.Cd_produto))
                    {
                        var ret = await dialogService.DisplayAlertAsync("Pergunta",
                            "Produto já existe no carrinho, quantidade<" + Sacola.ToList().FindAll(x => x.Cd_produto == prod.Cd_produto).Sum(p=> p.Quantidade) + ">.\r\n" +
                            "Deseja incluir novo item?", "SIM", "NÃO");
                        lancar = ret;
                    }
                    if(lancar)
                    {
                        if (prod.PontoCarne ||
                            prod.Adicional ||
                            prod.Ingrediente ||
                            prod.Sabor ||
                            prod.Obs)
                        {
                            var parametro = new NavigationParameters();
                            parametro.Add("produto", prod);
                            await navigationService.NavigateAsync("DetalheItemPage", parametro, useModalNavigation: true);
                        }
                        else
                        {
                            ItemVenda it = new ItemVenda();
                            it.Cd_produto = prod.Cd_produto;
                            it.Ds_produto = prod.Ds_produto;
                            it.Quantidade = prod.Quantidade;
                            it.PrecoVenda = prod.PrecoVenda;
                            Sacola.Add(it);
                            prod.ItemVendido = true;
                        }
                        ExisteSacola = Sacola.Count > 0;
                        ItensSacola = (int)Sacola.Sum(p => p.Quantidade);
                        ValorSacola = Sacola.Sum(p => p.ValorVenda + p.Adicionais.Sum(v => v.PrecoVenda));
                    }
                }
            });

            VisualizarCarrinhoCommand = new DelegateCommand(async () =>
            {
                var param = new NavigationParameters();
                param.Add("carrinho", Sacola);
                await navigationService.NavigateAsync("CarrinhoPage", param, useModalNavigation: true);
            });

            VoltarCommand = new DelegateCommand(async () => 
            {
                await navigationService.GoBackAsync();
            });
            
            BuscarCommand = new DelegateCommand(() =>
            {
                if(ProdutosOrigem.Count > 0 && Grupocorrente != null)
                {
                    if (string.IsNullOrWhiteSpace(Grupocorrente.Cd_grupo) &&
                        string.IsNullOrWhiteSpace(Filtroproduto))
                        ProdutosOC = new ObservableCollection<Produto>(ProdutosOrigem);
                    else if (string.IsNullOrWhiteSpace(Grupocorrente.Cd_grupo) &&
                        !string.IsNullOrWhiteSpace(Filtroproduto))
                        ProdutosOC = new ObservableCollection<Produto>(ProdutosOrigem.FindAll(p => p.Ds_produto.Contains(Filtroproduto.ToUpper())));
                    else if (!string.IsNullOrWhiteSpace(Grupocorrente.Cd_grupo) &&
                        string.IsNullOrWhiteSpace(Filtroproduto))
                        ProdutosOC = new ObservableCollection<Produto>(ProdutosOrigem.FindAll(p => p.Cd_grupo == Grupocorrente.Cd_grupo));
                    else ProdutosOC = new ObservableCollection<Produto>(ProdutosOrigem.FindAll(p => p.Cd_grupo == Grupocorrente.Cd_grupo &&
                                                                                                    p.Ds_produto.Contains(Filtroproduto.ToUpper())));
                }
            });

            AddItemCompra = new DelegateCommand<Produto>((Produto p) =>
            {
                if (p != null)
                    p.Quantidade += 1;
            });

            DelItemCompra = new DelegateCommand<Produto>((Produto p) =>
            {
                if (p == null ? false : p.Quantidade > 1)
                    p.Quantidade -= 1;
            });

            SolicitarTokenCommand = new DelegateCommand(async () =>
            {
                if (await DataService.SolicitarTokenAsync())
                {
                    Aguardandoliberacao = true;
                    Solicitartoken = false;
                    Cardapio = false;
                }
                else await dialogService.DisplayAlertAsync("Mensagem", "Erro solicitar Token.", "OK");
            });
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            loop = true;
            try
            {
                using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
                {
                    if (ProdutosOrigem.Count.Equals(0))
                    {
                        ProdutosOrigem = await DataService.GetProdutosAsync();
                        if (ProdutosOrigem.Count > 0)
                        {
                            List<Produto> lista = new List<Produto>();
                            var lgrupos =
                            ProdutosOrigem.Select(p => new { p.Cd_grupo, p.Ds_grupo })
                                .Distinct()
                                .OrderBy(p => p.Cd_grupo)
                                .ToList();
                            Grupos = new ObservableCollection<GrupoProduto>();
                            Grupos.Add(new GrupoProduto { Cd_grupo = string.Empty, Ds_grupo = "TODOS" });
                            lgrupos.ForEach(p => Grupos.Add(new GrupoProduto { Cd_grupo = p.Cd_grupo, Ds_grupo = p.Ds_grupo }));
                            Grupocorrente = Grupos.First();
                            ProdutosOC = new ObservableCollection<Produto>(ProdutosOrigem);
                        }
                    }
                    if (parameters != null)
                        if (parameters.Count > 0)
                        {
                            if (parameters.ContainsKey("item"))
                            {
                                Sacola.Add((ItemVenda)parameters["item"]);
                                ExisteSacola = true;
                                ItensSacola = (int)Sacola.Sum(p => p.Quantidade);
                                ValorSacola = Sacola.Sum(p => p.ValorVenda + p.Adicionais.Sum(v => v.PrecoVenda));
                                if (ProdutosOC.ToList().FirstOrDefault(v => v.Cd_produto == (parameters["item"] as ItemVenda).Cd_produto) != null)
                                    ProdutosOC.ToList().FirstOrDefault(v => v.Cd_produto == (parameters["item"] as ItemVenda).Cd_produto).ItemVendido = true;
                            }
                            if (parameters.ContainsKey("carrinho"))
                            {
                                Sacola.Clear();
                                ((ObservableCollection<ItemVenda>)parameters["carrinho"]).ForEach(p => Sacola.Add(p));
                                ExisteSacola = Sacola.Count > 0;
                                ItensSacola = (int)Sacola.Sum(p => p.Quantidade);
                                ValorSacola = Sacola.Sum(p => p.ValorVenda + p.Adicionais.Sum(v => v.PrecoVenda));
                                ProdutosOC
                                    .ToList()
                                    .ForEach(v => v.ItemVendido = Sacola.ToList().Exists(p => v.Cd_produto == p.Cd_produto));
                            }
                        }
                }
                await Task.Run(async () =>
                {
                    while (loop)
                    {
                        try
                        {
                            if (App.Garcom.ExigirTokenApp &&
                            App.Garcom.Token.Dt_token.AddMinutes(App.Garcom.Token.Temp_validade) < System.DateTime.Now &&
                            !Aguardandoliberacao)
                            {
                                Cardapio = false;
                                Solicitartoken = true;
                                Aguardandoliberacao = false;
                            }
                            else if (Aguardandoliberacao)
                            {
                                var tk = await DataService.ValidarTokenAsync();
                                if (tk != null)
                                {
                                    App.Garcom.Token = tk;
                                    Aguardandoliberacao = false;
                                    Solicitartoken = false;
                                    Cardapio = true;
                                }
                            }
                        }
                        catch { }
                        finally
                        { await Task.Delay(15000); }
                    }
                });
            }
            catch(Exception ex) { await dialogService.DisplayAlertAsync("Erro", ex.Message.Trim(), "OK"); }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            loop = false;
        }
    }
}
