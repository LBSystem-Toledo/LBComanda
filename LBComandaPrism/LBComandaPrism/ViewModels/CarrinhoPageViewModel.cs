using Acr.UserDialogs;
using LBComandaPrism.Models;
using LBComandaPrism.Services;
using LBComandaPrism.Utils;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace LBComandaPrism.ViewModels
{
    public class CarrinhoPageViewModel : ViewModelBase
    {
        private ItemVenda _itemvenda = null;
        public ItemVenda ItemSelecionado { get { return _itemvenda; } set { SetProperty(ref _itemvenda, value); } }
        private decimal _totalcarrinho = decimal.Zero;
        public decimal TotalCarrinho { get { return _totalcarrinho; } set { SetProperty(ref _totalcarrinho, value); } }
        public ObservableCollection<ItemVenda> Carrinho { get; set; } = new ObservableCollection<ItemVenda>();

        public DelegateCommand<ItemVenda> removeItemCommand { get; }
        public DelegateCommand voltarCommand { get; }
        public DelegateCommand fecharCommand { get; }
        public DelegateCommand<ItemVenda> AddItemCompra { get; }
        public DelegateCommand<ItemVenda> DelItemCompra { get; }

        private readonly IPageDialogService dialogService;
        public CarrinhoPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            :base(navigationService)
        {
            dialogService = pageDialogService;

            AddItemCompra = new DelegateCommand<ItemVenda>((ItemVenda p) =>
            {
                if (p != null)
                {
                    p.Quantidade += 1;
                    TotalCarrinho = Carrinho.Sum(x => x.ValorVenda);
                    Carrinho.ForEach(x => TotalCarrinho += x.Adicionais.Sum(v => v.PrecoVenda));
                }
            });

            DelItemCompra = new DelegateCommand<ItemVenda>((ItemVenda p) =>
            {
                if (p == null ? false : p.Quantidade > 1)
                {
                    p.Quantidade -= 1;
                    TotalCarrinho = Carrinho.Sum(x => x.ValorVenda);
                    Carrinho.ForEach(x => TotalCarrinho += x.Adicionais.Sum(v => v.PrecoVenda));
                }
            });

            voltarCommand = new DelegateCommand(async () =>
            {
                var param = new NavigationParameters();
                param.Add("carrinho", Carrinho);
                await navigationService.GoBackAsync(param);
            });

            removeItemCommand = new DelegateCommand<ItemVenda>((ItemVenda item) =>
            {
                if (item != null)
                {
                    Carrinho.Remove(item);
                    TotalCarrinho = Carrinho.Sum(p => p.ValorVenda);
                    Carrinho.ForEach(p => TotalCarrinho += p.Adicionais.Sum(v => v.PrecoVenda));
                }
            });

            fecharCommand = new DelegateCommand(async () =>
            {
                using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
                {
                    if (Carrinho.Count.Equals(0))
                    {
                        await dialogService.DisplayAlertAsync("Mensagem", "Não é permitido fechar comanda sem ITENS.", "OK");
                        await navigationService.GoBackAsync();
                    }
                    else
                    {
                        Carrinho.ForEach(p => p.Cd_garcom = App.Garcom.Cd_garcom);
                        if (App.Garcom.Tp_cartao.Trim().Equals("0"))
                        {
                            NavigationParameters param = new NavigationParameters();
                            param.Add("CARRINHO", Carrinho.ToList());
                            await navigationService.NavigateAsync("NumeroCartaoPage", param);
                        }
                        else
                        {
                            NavigationParameters param = new NavigationParameters();
                            param.Add("CARRINHO", Carrinho.ToList());
                            await navigationService.NavigateAsync("MapaMesaPage", param, useModalNavigation: true);
                        }
                    }
                }
            });
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.Count > 0)
            {
                if (parameters.ContainsKey("carrinho"))
                {
                    ((ObservableCollection<ItemVenda>)parameters["carrinho"]).ForEach(p => Carrinho.Add(p));
                    TotalCarrinho = Carrinho.Sum(p => p.ValorVenda);
                    Carrinho.ForEach(p => TotalCarrinho += p.Adicionais.Sum(v => v.PrecoVenda));
                }
                if(parameters.ContainsKey("NR_CARTAO"))
                    using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
                    {
                        try
                        {
                            var retorno = await DataService.GravarComandaCartaoAsync(parameters["NR_CARTAO"].ToString(), Carrinho.ToList());
                            if (retorno)
                            {
                                await dialogService.DisplayAlertAsync("Mensagem", "Comanda gravada com sucesso.", "OK");
                                await NavigationService.NavigateAsync("/MenuPage/NavigationPage/CardapioPage");
                            }
                            else await dialogService.DisplayAlertAsync("Mensagem", "Erro gravar comanda.", "OK");
                        }
                        catch (Exception ex) { await dialogService.DisplayAlertAsync("Erro", ex.Message.Trim(), "OK"); }
                    }
            }
        }
    }
}
