using Prism.Mvvm;

namespace LBComandaPrism.Models
{
    public class Sabor: BindableBase
    {
        private string _ds_sabor = string.Empty;
        public string Ds_sabor { get { return _ds_sabor; } set { SetProperty(ref _ds_sabor, value); } }
        private bool _incluido = false;
        public bool Incluido { get { return _incluido; } set { SetProperty(ref _incluido, value); } }
    }
}
