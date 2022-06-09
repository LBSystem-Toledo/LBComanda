using Prism.Mvvm;
using System.Drawing;

namespace LBComandaPrism.Models
{
    public class Produto: BindableBase
    {
        private string _cd_produto = string.Empty;
        public string Cd_produto { get { return _cd_produto; } set { SetProperty(ref _cd_produto, value); } }
        private string _ds_produto = string.Empty;
        public string Ds_produto { get { return _ds_produto; } set { SetProperty(ref _ds_produto, value); } }
        private string _cd_grupo = string.Empty;
        public string Cd_grupo { get { return _cd_grupo; } set { SetProperty(ref _cd_grupo, value); } }
        private string _ds_grupo = string.Empty;
        public string Ds_grupo { get { return _ds_grupo; } set { SetProperty(ref _ds_grupo, value); } }
        private string _cd_grupo_pai = string.Empty;
        public string Cd_grupo_pai { get { return _cd_grupo_pai; } set { SetProperty(ref _cd_grupo_pai, value); } }
        private string _ds_grupo_pai = string.Empty;
        public string Ds_grupo_pai { get { return _ds_grupo_pai; } set { SetProperty(ref _ds_grupo_pai, value); } }
        private bool _pontocarne = false;
        public bool PontoCarne { get { return _pontocarne; } set { SetProperty(ref _pontocarne, value); } }
        private decimal _precovenda = decimal.Zero;
        public decimal PrecoVenda { get { return _precovenda; } set { SetProperty(ref _precovenda, value); } }
        private int _quantidade = 1;
        public int Quantidade { get { return _quantidade; } set { SetProperty(ref _quantidade, value); } }
        private bool _bloqueado = false;
        public bool Bloqueado
        {
            get { return _bloqueado; }
            set
            {
                SetProperty(ref _bloqueado, value);
                if (value)
                    Cor = Color.Red;
                else Cor = Color.FromArgb(255, 255, 255);
            }
        }
        private bool _itemvendido = false;
        public bool ItemVendido
        {
            get { return _itemvendido; }
            set
            {
                SetProperty(ref _itemvendido, value);
                //if (value)
                //    Cor = Color.FromArgb(206,136,69);
                //else Cor = Color.FromArgb(255,255,255);
            }
        }
        private Color _cor = Color.FromArgb(255,255,255);
        public Color Cor
        {
            get { return _cor; }
            set { SetProperty(ref _cor, value); }
        }
        private int _max_sabor;
        public int Max_sabor { get { return _max_sabor; } set { SetProperty(ref _max_sabor, value); } }
        private bool _adicional = false;
        public bool Adicional { get { return _adicional; } set { SetProperty(ref _adicional, value); } }
        private bool _ingrediente = false;
        public bool Ingrediente { get { return _ingrediente; } set { SetProperty(ref _ingrediente, value); } }
        private bool _sabor = false;
        public bool Sabor { get { return _sabor; } set { SetProperty(ref _sabor, value); } }
        private bool _obs = false;
        public bool Obs { get { return _obs; } set { SetProperty(ref _obs, value); } }
    }
}
