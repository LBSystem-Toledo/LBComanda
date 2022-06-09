using Prism.Mvvm;

namespace LBComandaPrism.Models
{
    public class Adicional: BindableBase
    {
        private string _cd_adicional = string.Empty;
        public string Cd_adicional { get { return _cd_adicional; } set { SetProperty(ref _cd_adicional, value); } }
        private string _ds_adicional = string.Empty;
        public string Ds_adicional { get { return _ds_adicional; } set { SetProperty(ref _ds_adicional, value); } }
        private decimal _precovenda = decimal.Zero;
        public decimal PrecoVenda { get { return _precovenda; } set { SetProperty(ref _precovenda, value); } }
        private bool _selecionado = false;
        public bool Selecionado { get { return _selecionado; } set { SetProperty(ref _selecionado, value); } }
    }
}
