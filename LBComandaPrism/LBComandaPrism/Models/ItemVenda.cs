using Prism.Mvvm;
using System.Collections.Generic;

namespace LBComandaPrism.Models
{
    public class ItemVenda: BindableBase
    {
        private string _cd_garcom = string.Empty;
        public string Cd_garcom { get { return _cd_garcom; } set { SetProperty(ref _cd_garcom, value); } }
        private string _cd_produto = string.Empty;
        public string Cd_produto { get { return _cd_produto; } set { SetProperty(ref _cd_produto, value); } }
        private string _ds_produto = string.Empty;
        public string Ds_produto { get { return _ds_produto; } set { SetProperty(ref _ds_produto, value); } }
        private decimal _quantidade = decimal.Zero;
        public decimal Quantidade { get { return _quantidade; } set { SetProperty(ref _quantidade, value); } }
        private decimal _precovenda = decimal.Zero;
        public decimal PrecoVenda { get { return _precovenda; } set { SetProperty(ref _precovenda, value); } }
        public decimal ValorVenda => decimal.Multiply(Quantidade, PrecoVenda);
        private string _obs = string.Empty;
        public string Obs { get { return _obs; } set { SetProperty(ref _obs, value); } }
        private string _pontocarne = string.Empty;
        public string PontoCarne { get { return _pontocarne; } set { SetProperty(ref _pontocarne, value); } }
        private string _nr_mesacartao = string.Empty;
        public string Nr_mesacartao { get { return _nr_mesacartao; } set { SetProperty(ref _nr_mesacartao, value); } }
        public bool ExistePontoCarne => !string.IsNullOrWhiteSpace(PontoCarne);
        public bool ExisteObs => !string.IsNullOrWhiteSpace(Obs);
        public bool ExisteIngredientes => Ingredientes.Count > 0;
        public string IngredientesStr
        {
            get
            {
                if (Ingredientes.Count > 0)
                {
                    string s = string.Empty;
                    Ingredientes.ForEach(p => s += (string.IsNullOrWhiteSpace(s) ? string.Empty : ",") + p.Ds_item.Trim());
                    return s;
                }
                else return string.Empty;
            }
        }
        public bool ExisteItensExcluir => ItensExcluir.Count > 0;
        public string ItensExcluirStr
        {
            get
            {
                if (ItensExcluir.Count > 0)
                {
                    string s = string.Empty;
                    ItensExcluir.ForEach(p => s += (string.IsNullOrWhiteSpace(s) ? string.Empty : ",") + p.Ds_item.Trim());
                    return s;
                }
                else return string.Empty;
            }
        }
        public bool ExisteAdicional => Adicionais.Count > 0;
        public string AdicionaisStr
        {
            get
            {
                if (Adicionais.Count > 0)
                {
                    string s = string.Empty;
                    Adicionais.ForEach(p => s += (string.IsNullOrWhiteSpace(s) ? string.Empty : ",") + p.Ds_adicional.Trim());
                    return s;
                }
                else return string.Empty;
            }
        }
        public bool ExisteSabores => Sabores.Count > 0;
        public string SaboresStr
        {
            get
            {
                if (Sabores.Count > 0)
                {
                    string s = string.Empty;
                    Sabores.ForEach(p => s += (string.IsNullOrWhiteSpace(s) ? string.Empty : ",") + p.Ds_sabor.Trim());
                    return s;
                }
                else return string.Empty;
            }
        }
        private List<Adicional> _adicionais = new List<Adicional>();
        public List<Adicional> Adicionais { get { return _adicionais; } set { SetProperty(ref _adicionais, value); } }
        private List<Ingredientes> _ingredientes = new List<Ingredientes>();
        public List<Ingredientes> Ingredientes { get { return _ingredientes; } set { SetProperty(ref _ingredientes, value); } }
        private List<Ingredientes> _ingredientesdel = new List<Ingredientes>();
        public List<Ingredientes> IngredientesDel { get { return _ingredientesdel; } set { SetProperty(ref _ingredientesdel, value); } }
        private List<ItemExcluir> _itensexcluir = new List<ItemExcluir>();
        public List<ItemExcluir> ItensExcluir { get { return _itensexcluir; } set { SetProperty(ref _itensexcluir, value); } }
        private List<Sabor> _sabores = new List<Sabor>();
        public List<Sabor> Sabores { get { return _sabores; } set { SetProperty(ref _sabores, value); } }
    }
}
