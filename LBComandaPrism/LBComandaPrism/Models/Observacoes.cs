using Prism.Mvvm;

namespace LBComandaPrism.Models
{
    public class Observacoes: BindableBase
    {
        private string _obs = string.Empty;
        public string Obs { get { return _obs; } set { SetProperty(ref _obs, value); } }
        private bool _marcar = false;
        public bool Marcar { get { return _marcar; } set { SetProperty(ref _marcar, value); } }
    }
}
