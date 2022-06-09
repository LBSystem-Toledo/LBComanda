using Xamarin.Forms;

namespace LBComandaPrism.Views
{
    public partial class MapaMesaPage : ContentPage
    {
        public MapaMesaPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
