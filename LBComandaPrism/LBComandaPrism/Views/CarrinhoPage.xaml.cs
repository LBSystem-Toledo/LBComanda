using LBComandaPrism.ViewModels;
using Xamarin.Forms;

namespace LBComandaPrism.Views
{
    public partial class CarrinhoPage : ContentPage
    {
        public CarrinhoPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
