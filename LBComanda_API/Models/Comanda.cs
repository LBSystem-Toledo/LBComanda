using System.Collections.Generic;

namespace LBComanda_API.Models
{
    public class Comanda
    {
        public string Id_local { get; set; } = string.Empty;
        public string Id_mesa { get; set; } = string.Empty;
        public List<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();
    }
}
