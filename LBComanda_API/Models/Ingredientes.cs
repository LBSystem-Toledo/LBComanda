namespace LBComanda_API.Models
{
    public class Ingredientes
    {
        public string Cd_produto { get; set; } = string.Empty;
        public string Cd_item { get; set; } = string.Empty;
        public string Ds_item { get; set; } = string.Empty;
        public bool Obrigatorio { get; set; } = false;
    }
}
