using LBComandaPrism.Models;
using LBComandaPrism.Services;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms.Internals;

namespace LBComandaPrism.ViewModels
{
    public class MesaPageViewModel : ViewModelBase
    {
        private bool Extrato { get; set; } = false;
        private Local _localcurrent = null;
        public Local LocalCurrent 
        { 
            get { return _localcurrent; } 
            set 
            {
                if (_localcurrent != value)
                {
                    SetProperty(ref _localcurrent, value);
                    Mesas.FindAll(p => p.Id_local == value.Id_local)
                        .ForEach(p => MesasLocal.Add(p));
                }
            } 
        }
        public ObservableCollection<Local> Locais { get; set; } = new ObservableCollection<Local>();
        public ObservableCollection<Mesa> MesasLocal { get; set; } = new ObservableCollection<Mesa>();
        public List<Mesa> Mesas { get; set; } = new List<Mesa>();
        public List<ItemVenda> Carrinho { get; set; } = new List<ItemVenda>();

        public DelegateCommand<Mesa> MesaCommand { get; }
        public DelegateCommand CancelarCommand { get; }

        private readonly IPageDialogService dialogService;
        public MesaPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            :base(navigationService)
        {
            dialogService = pageDialogService;

            MesaCommand = new DelegateCommand<Mesa>(async (Mesa m) =>
            {
                if (m != null)
                {
                    if (Extrato)
                    {
                        var param = new NavigationParameters();
                        param.Add("id_local", m.Id_local);
                        param.Add("local", m.Ds_local);
                        param.Add("id_mesa", m.Id_mesa);
                        param.Add("mesa", m.Nr_mesa);
                        await navigationService.NavigateAsync("ExtratoPage", param);
                    }
                    else
                    {
                        var retorno = await dialogService.DisplayAlertAsync("Pergunta", "Confirma seleção da MESA Nº" + m.Nr_mesa, "OK", "CANCEL");
                        if (retorno)
                        {
                            Carrinho.ForEach(p => p.Cd_garcom = App.Garcom.Cd_garcom);
                            retorno = await DataService.GravarComandaMesaAsync(((int)m.Id_local).ToString(), ((int)m.Id_mesa).ToString(), Carrinho);
                            if (retorno)
                            {
                                await dialogService.DisplayAlertAsync("Mensagem", "Comanda gravada com sucesso.", "OK");
                                var param = new NavigationParameters();
                                param.Add("limparcarrinho", "OK");
                                await navigationService.NavigateAsync("CardapioPage");
                            }
                            else await dialogService.DisplayAlertAsync("Mensagem", "Erro gravar comanda.", "OK");
                        }
                    }
                }
            });

            CancelarCommand = new DelegateCommand(async () => await navigationService.GoBackAsync());
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            List<Mesa> retorno = await DataService.GetMesasAsync();
            retorno.ForEach(p => Mesas.Add(p));
            Mesas.Select(p => new { p.Id_local, p.Ds_local })
                .Distinct()
                .ToList()
                .ForEach(p => Locais.Add(new Local { Id_local = p.Id_local, Ds_local = p.Ds_local }));
            if (parameters != null)
            {
                if (parameters.ContainsKey("carrinho"))
                    ((ObservableCollection<ItemVenda>)parameters["carrinho"]).ForEach(p => Carrinho.Add(p));
                if (parameters.ContainsKey("extrato"))
                    Extrato = true;
            }
        }
    }
}
