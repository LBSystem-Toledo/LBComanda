using System.Collections.Generic;

namespace LBComanda_API.Models
{
    public class GrupoProduto
    {
        public string Cd_grupo { get; set; } = string.Empty;
        public string Ds_grupo { get; set; } = string.Empty;
        public List<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
