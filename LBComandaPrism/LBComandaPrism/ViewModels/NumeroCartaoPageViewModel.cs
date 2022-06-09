using Acr.UserDialogs;
using LBComandaPrism.Models;
using LBComandaPrism.Services;
using LBComandaPrism.Utils;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LBComandaPrism.ViewModels
{
    public class NumeroCartaoPageViewModel : ViewModelBase
    {
        List<ItemVenda> ItensVenda { get; set; }

        string _nr_cartao = string.Empty;
        public string Nr_cartao { get { return _nr_cartao; } set { SetProperty(ref _nr_cartao, value); } }

        public DelegateCommand ConfirmarCommand { get; }

        private readonly IPageDialogService dialogService;
        public NumeroCartaoPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            :base(navigationService)
        {
            dialogService = pageDialogService;

            ConfirmarCommand = new DelegateCommand(async () =>
            {
                if(string.IsNullOrWhiteSpace(Nr_cartao.SoNumero()))
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Obrigatório informar numero cartão.", "OK");
                    return;
                }
                if(App.Garcom.Nr_cartaorotini > 0 &&
                int.Parse(Nr_cartao.SoNumero()) < App.Garcom.Nr_cartaorotini)
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Cartão informado deve ser maior que <" + App.Garcom.Nr_cartaorotini.ToString() + ">.", "OK");
                    Nr_cartao = string.Empty;
                    return;
                }
                if (App.Garcom.Nr_cartaorotfin > 0 &&
                int.Parse(Nr_cartao.SoNumero()) > App.Garcom.Nr_cartaorotfin)
                {
                    await dialogService.DisplayAlertAsync("Mensagem", "Cartão informado deve ser menor que <" + App.Garcom.Nr_cartaorotfin.ToString() + ">.", "OK");
                    Nr_cartao = string.Empty;
                    return;
                }
                NavigationParameters p = new NavigationParameters();
                p.Add("NR_CARTAO", Nr_cartao);
                await navigationService.GoBackAsync(p);
            });
        }
    }
}
