namespace LBComandaAPI.Models
{
    public class RecVenda
    {
        public string Nr_cartao { get; set; } = string.Empty;
        public int Id_local { get; set; }
        public int Id_mesa { get; set; }
        public string Cd_empresa { get; set; } = string.Empty;
        public decimal Id_cartao { get; set; } = decimal.Zero;
        public decimal Valor { get; set; } = decimal.Zero;
        public string Bandeira { get; set; } = string.Empty;
        public string Nsu { get; set; } = string.Empty;
        public string Cd_garcom { get; set; } = string.Empty;
        public string D_C { get; set; } = string.Empty;
    }
}
