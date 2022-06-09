using Prism.Mvvm;
using System.Drawing;

namespace LBComandaPrism.Models
{
    public class Mesa: BindableBase
    {
        private decimal _id_mesa;
        public decimal Id_mesa { get { return _id_mesa; } set { SetProperty(ref _id_mesa, value); } }
        private decimal _id_local;
        public decimal Id_local { get { return _id_local; } set { SetProperty(ref _id_local, value); } }
        private string _ds_local = string.Empty;
        public string Ds_local { get { return _ds_local; } set { SetProperty(ref _ds_local, value); } }
        private string _nr_mesa = string.Empty;
        public string Nr_mesa { get { return _nr_mesa; } set { SetProperty(ref _nr_mesa, value); } }
        private bool _possuivenda = false;
        public bool PossuiVenda { get { return _possuivenda; } set { SetProperty(ref _possuivenda, value); } }
        public Color CorBotao => PossuiVenda ? Color.Red : Color.FromArgb(240, 139, 41);
    }
}
