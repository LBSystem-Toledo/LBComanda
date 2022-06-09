using LBComandaPrism.Models;
using LBComandaPrism.Services;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LBComandaPrism.ViewModels
{
    public class InternoPageViewModel : ViewModelBase
    {
        public ObservableCollection<Mesa> MesasLocal { get; set; } = new ObservableCollection<Mesa>();

        private readonly IPageDialogService dialogService;
        public InternoPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            :base(navigationService)
        {
            dialogService = pageDialogService;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            List<Mesa> retorno = await new DataService().GetMesasAsync();
            retorno?.Where(p => p.Ds_local.Trim().ToUpper().Equals("INTERNO"))
                .ToList()
                .ForEach(p => MesasLocal.Add(p));
        }
    }
}
