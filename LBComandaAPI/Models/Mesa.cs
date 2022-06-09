namespace LBComandaAPI.Models
{
    public class Mesa
    {
        public decimal Id_mesa { get; set; }
        public decimal Id_local { get; set; }
        public string Ds_local { get; set; }
        public string Nr_mesa { get; set; }
        public bool PossuiVenda { get; set; } = false;
    }
}
