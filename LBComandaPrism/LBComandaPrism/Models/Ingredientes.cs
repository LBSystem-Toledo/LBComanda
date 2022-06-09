using Prism.Mvvm;

namespace LBComandaPrism.Models
{
    public class Ingredientes: BindableBase
    {
        private string _cd_produto = string.Empty;
        public string Cd_produto { get { return _cd_produto; } set { SetProperty(ref _cd_produto, value); } }
        private string _cd_item = string.Empty;
        public string Cd_item { get { return _cd_item; } set { SetProperty(ref _cd_item, value); } }
        private string _ds_item = string.Empty;
        public string Ds_item { get { return _ds_item; } set { SetProperty(ref _ds_item, value); } }
        private bool _obrigatorio = false;
        public bool Obrigatorio { get { return _obrigatorio; } set { SetProperty(ref _obrigatorio, value); } }

        public bool Habilitar => !Obrigatorio;
        private bool _incluido = true;
        public bool Incluido { get { return _incluido; } set { SetProperty(ref _incluido, value); } }
    }
}
