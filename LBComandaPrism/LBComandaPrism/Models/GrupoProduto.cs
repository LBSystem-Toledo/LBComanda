using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace LBComandaPrism.Models
{
    public class GrupoProduto: BindableBase
    {
        private string _cd_grupo = string.Empty;
        public string Cd_grupo { get { return _cd_grupo; } set { SetProperty(ref _cd_grupo, value); } }
        private string _ds_grupo = string.Empty;
        public string Ds_grupo { get { return _ds_grupo; } set { SetProperty(ref _ds_grupo, value); } }
        private ObservableCollection<Produto> _produtos;
        public ObservableCollection<Produto> Produtos { get { return _produtos; } set { SetProperty(ref _produtos, value); } }
    }
}
