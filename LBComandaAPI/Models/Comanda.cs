using System.Collections.Generic;

namespace LBComandaAPI.Models
{
    public class Comanda
    {
        public int Id_prevenda { get; set; }
        public string Cd_empresa { get; set; } = string.Empty;
        public int Id_item { get; set; }
        public string Tp_impressora { get; set; } = string.Empty;
        public string Porta_Imp { get; set; } = string.Empty;
        public int Id_cartao { get; set; }
        public string Nr_mesa { get; set; } = string.Empty;
        public string Ds_local { get; set; } = string.Empty;
        public string Nm_garcom { get; set; } = string.Empty;
        public string Ds_produto { get; set; } = string.Empty;
        public decimal Quantidade { get; set; } = decimal.Zero;
        public string Pontocarne { get; set; } = string.Empty;
        public string Obsitem { get; set; } = string.Empty;
        public IEnumerable<Ingredientes> Ingredientes { get; set; }
        public IEnumerable<Sabor> Sabores { get; set; }
        public IEnumerable<Adicional> Adicionais { get; set; }
    }
}
