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

namespace LBComandaPrism.ViewModels
{
    public class MapaMesaPageViewModel : ViewModelBase
    {
        private List<ItemVenda> Carrinho { get; set; }
        private List<Mesa> MesaOrigem { get; set; } = new List<Mesa>();

        private Local _localrrente;
        public Local Localcorrente
        {
            get { return _localrrente; }
            set
            {
                SetProperty(ref _localrrente, value);
                if (value != null)
                    Mesas = new ObservableCollection<Mesa>(MesaOrigem.FindAll(p => p.Id_local == value.Id_local));
            }
        }
        private ObservableCollection<Local> _locais;
        public ObservableCollection<Local> Locais { get { return _locais; } set { SetProperty(ref _locais, value); } }
        private ObservableCollection<Mesa> _mesas;
        public ObservableCollection<Mesa> Mesas { get { return _mesas; } set { SetProperty(ref _mesas, value); } }

        public DelegateCommand<Mesa> MesaCommand { get; }
        public DelegateCommand CancelarCommand { get; }
        

        private readonly IPageDialogService dialogService;
        public MapaMesaPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
            dialogService = pageDialogService;
            
            MesaCommand = new DelegateCommand<Mesa>(async (Mesa m) => 
            {
                if (Carrinho != null)
                {
                    var ret = await dialogService.DisplayAlertAsync("Pergunta", "Confirma seleção da mesa Nº" + m.Nr_mesa + " Local " + m.Ds_local.Trim().ToUpper() + " para gravar comanda?", "SIM", "NÃO");
                    if (ret)
                        try
                        {
                            using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
                            {
                                var retorno = await DataService.GravarComandaMesaAsync(((int)m.Id_local).ToString(), ((int)m.Id_mesa).ToString(), Carrinho);
                                if (retorno)
                                {
                                    await dialogService.DisplayAlertAsync("Mensagem", "Comanda gravada com sucesso.", "OK");
                                    await navigationService.NavigateAsync("/MenuPage/NavigationPage/CardapioPage");
                                }
                                else await dialogService.DisplayAlertAsync("Mensagem", "Erro gravar comanda.", "OK");
                            }
                        }
                        catch { }
                }
                else if(m.PossuiVenda)
                {
                    NavigationParameters param = new NavigationParameters();
                    param.Add("mesa", m);
                    await navigationService.NavigateAsync("ExtratoPage", param, useModalNavigation: true);
                }
            });

            CancelarCommand = new DelegateCommand(async () => await navigationService.GoBackAsync());
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                using (UserDialogs.Instance.Loading(title: string.Empty, maskType: MaskType.Black))
                {
                    if (MesaOrigem.Count.Equals(0))
                    {
                        MesaOrigem = await DataService.GetMesasAsync();
                        if (MesaOrigem.Count > 0)
                        {
                            var lLocais =
                            MesaOrigem.Select(p => new { p.Id_local, p.Ds_local })
                                .Distinct()
                                .OrderBy(p => p.Id_local)
                                .ToList();
                            Locais = new ObservableCollection<Local>();
                            lLocais.ForEach(p => Locais.Add(new Local { Id_local = p.Id_local, Ds_local = p.Ds_local }));
                            Localcorrente = Locais.First();
                            Mesas = new ObservableCollection<Mesa>(MesaOrigem.FindAll(p=> p.Id_local == Localcorrente.Id_local));
                        }
                    }
                }
                if (parameters.ContainsKey("CARRINHO"))
                    Carrinho = parameters["CARRINHO"] as List<ItemVenda>;
            }
            catch (Exception ex) { await dialogService.DisplayAlertAsync("Erro", ex.Message.Trim(), "OK"); }
        }
    }
}
