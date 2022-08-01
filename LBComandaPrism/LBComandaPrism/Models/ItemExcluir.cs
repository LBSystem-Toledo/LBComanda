using Prism.Mvvm;

namespace LBComandaPrism.Models
{
    public class ItemExcluir: BindableBase
    {
        string _ds_item = string.Empty;
        public string Ds_item { get { return _ds_item; } set { SetProperty(ref _ds_item, value); } }
        private bool _selecionado = false;
        public bool Selecionado { get { return _selecionado; } set { SetProperty(ref _selecionado, value); } }
    }
}
