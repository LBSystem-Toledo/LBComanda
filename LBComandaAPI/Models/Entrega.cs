using System;

namespace LBComandaAPI.Models
{
    public class Entrega
    {
        public string Cd_empresa { get; set; } = string.Empty;
        public decimal Id_prevenda { get; set; }
        public bool ST_LevarMaqCartao { get; set; } = false;
        public string ObsFecharDelivery { get; set; } = string.Empty;
        public DateTime? Dt_entregadelivery { get; set; } = null;
        public decimal Vl_TrocoPara { get; set; } = decimal.Zero;
        public string NM_Clifor { get; set; } = string.Empty;
        public string DS_Endereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string DS_Complemento { get; set; } = string.Empty;
        public string Proximo { get; set; } = string.Empty;
        public string DS_Cidade { get; set; } = string.Empty;
        public string DS_Observacao { get; set; } = string.Empty;
    }
}
