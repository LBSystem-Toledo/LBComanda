using System.Collections.Generic;

namespace LBComanda_API.Models
{
    public class ItemVenda
    {
        public string Cd_empresa { get; set; } = string.Empty;
        public decimal? Id_prevenda { get; set; } = null;
        public decimal? Id_item { get; set; } = null;
        public string Cd_garcom { get; set; } = string.Empty;
        public string Cd_produto { get; set; } = string.Empty;
        public string Ds_produto { get; set; } = string.Empty;
        public decimal Quantidade { get; set; } = decimal.Zero;
        public decimal PrecoVenda { get; set; } = decimal.Zero;
        public string Obs { get; set; } = string.Empty;
        public string PontoCarne { get; set; } = string.Empty;
        public List<Adicional> Adicionais { get; set; } = new List<Adicional>();
        public List<Ingredientes> Ingredientes { get; set; } = new List<Ingredientes>();
        public List<Ingredientes> IngredientesDel { get; set; } = new List<Ingredientes>();
        public List<Sabor> Sabores { get; set; } = new List<Sabor>();
    }
}
