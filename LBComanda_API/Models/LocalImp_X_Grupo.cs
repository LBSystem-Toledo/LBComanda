using System.Collections.Generic;

namespace LBComanda_API.Models
{
    public class LocalImp_X_Grupo
    {
        public decimal? Id_localimp { get; set; } = null;
        public string Ds_localimp { get; set; } = string.Empty;
        public string Porta_imp { get; set; } = string.Empty;
        public string Tp_impressora { get; set; } = string.Empty;
        public List<ItemVenda> ItensImprimir { get; set; } = new List<ItemVenda>();
    }
}
