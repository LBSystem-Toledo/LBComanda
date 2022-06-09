using System.Collections.Generic;

namespace LBComandaAPI.Models
{
    public class Grupo
    {
        public string Cd_grupo { get; set; } = string.Empty;
        public string Ds_grupo { get; set; } = string.Empty;
        public IEnumerable<Produto> Produtos { get; set; }
    }
}
