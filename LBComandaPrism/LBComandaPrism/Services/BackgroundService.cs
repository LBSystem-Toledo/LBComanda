using LBComandaPrism.Models;
using LBComandaPrism.Utils;
using Matcha.BackgroundService;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LBComandaPrism.Services
{
    public class BackgroundService : IPeriodicTask
    {
        public TimeSpan Interval { get; set; }

        public BackgroundService(int segundos)
        { Interval = TimeSpan.FromSeconds(segundos); }
        public async Task<bool> StartJob()
        {
            //Buscar entregas entregue
            List<Entrega> lista = await App.Database.GetEntregasAsync(Entregue: true);
            if (lista != null)
                foreach(var l in lista)
                {
                    string url = App.url_api.Substring(0, App.url_api.LastIndexOf(':'));
                    string porta = App.url_api.Substring(App.url_api.LastIndexOf(':') + 1, App.url_api.Length - App.url_api.LastIndexOf(':') - 1);
                    bool ret = await CrossConnectivity.Current.IsRemoteReachable(url, int.Parse(porta.SoNumero()));
                    //Verificar se url tem conexão
                    if (ret)
                        try
                        {
                            //Sincronizar com api
                            if (await DataService.ConcluirEntregaAsync(l))
                                await App.Database.ExcluirEntregaAsync(l);
                        }
                        catch { }
                }
            return true;
        }
    }
}
