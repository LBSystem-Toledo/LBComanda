using Prism.Mvvm;

namespace LBComandaPrism.Models
{
    public class PontoCarne: BindableBase
    {
        private string _ds_ponto = string.Empty;
        public string Ds_ponto { get { return _ds_ponto; } set { SetProperty(ref _ds_ponto, value); } }
    }
}
