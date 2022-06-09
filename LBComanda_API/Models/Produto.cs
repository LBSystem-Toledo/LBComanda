namespace LBComanda_API.Models
{
    public class Produto
    {
        public string Cd_produto { get; set; } = string.Empty;
        public string Ds_produto { get; set; } = string.Empty;
        public string Cd_grupo { get; set; } = string.Empty;
        public string Ds_grupo { get; set; } = string.Empty;
        public string Cd_grupo_pai { get; set; } = string.Empty;
        public string Ds_grupo_pai { get; set; } = string.Empty;
        public bool PontoCarne { get; set; } = false;
        public decimal PrecoVenda { get; set; } = decimal.Zero;
    }
}
