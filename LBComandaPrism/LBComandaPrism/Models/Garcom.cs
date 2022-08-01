using Prism.Mvvm;

namespace LBComandaPrism.Models
{
    public class Garcom: BindableBase
    {
        public string Cd_empresa { get; set; } = string.Empty;
        public string Nm_empresa { get; set; } = string.Empty;
        public string Endereco_empresa { get; set; } = string.Empty;
        private string _cd_garcom = string.Empty;
        public string Cd_garcom { get { return _cd_garcom; } set { SetProperty(ref _cd_garcom, value); } }
        private string _nm_garcom = string.Empty;
        public string Nm_garcom { get { return _nm_garcom; } set { SetProperty(ref _nm_garcom, value); } }
        public string Cnpj { get; set; } = string.Empty;
        public bool ExigirTokenApp { get; set; } = false;
        public bool ST_Entregador { get; set; } = false;
        public bool LerQRCodeAPP { get; set; } = false;
        public bool St_mesacartao { get; set; } = false;
        public Token Token { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Tp_cartao { get; set; } = string.Empty;
        public int Nr_cartaorotini { get; set; }
        public int Nr_cartaorotfin { get; set; }
        public string Stone_id { get; set; } = string.Empty;
        public string Impressora { get; set; } = string.Empty;
    }
}
