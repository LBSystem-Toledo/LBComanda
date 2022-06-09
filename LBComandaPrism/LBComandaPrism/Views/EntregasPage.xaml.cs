using Xamarin.Forms;

namespace LBComandaPrism.Views
{
    public partial class EntregasPage : ContentPage
    {
        public EntregasPage()
        {
            InitializeComponent();
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
