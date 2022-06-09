using Xamarin.Forms;

namespace LBComandaPrism.Views
{
    public partial class CardapioPage : ContentPage
    {
        public CardapioPage()
        {
            InitializeComponent();
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
