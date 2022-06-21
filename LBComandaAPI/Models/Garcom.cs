namespace LBComandaAPI.Models
{
    public class Garcom
    {
        public string Cd_empresa { get; set; } = string.Empty;
        public string Cd_garcom { get; set; } = string.Empty;
        public string Nm_garcom { get; set; } = string.Empty;
        public bool ExigirTokenApp { get; set; } = false;
        public bool ST_Entregador { get; set; } = false;
        public bool LerQRCodeAPP { get; set; } = false;
        public bool St_mesacartao { get; set; } = false;
        public Token Token { get; set; }
        public string Tp_cartao { get; set; } = string.Empty;
        public int Nr_cartaorotini { get; set; }
        public int Nr_cartaorotfin { get; set; }
        public string Stone_id { get; set; } = string.Empty;
    }
}
