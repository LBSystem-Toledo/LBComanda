using LBComandaPrism.ViewModels;
using Xamarin.Forms;

namespace LBComandaPrism.Views
{
    public partial class AbrirFecharCartaoPage : ContentPage
    {
        public AbrirFecharCartaoPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var viewModel = BindingContext as AbrirFecharCartaoPageViewModel;
            if (viewModel.ConsultaClienteCommand.CanExecute())
                viewModel.ConsultaClienteCommand.Execute();
        }
    }
}
