using LBComandaPrism.ViewModels;
using System.Linq;
using Xamarin.Forms;

namespace LBComandaPrism.Views
{
    public partial class DetalheItemPage : ContentPage
    {
        public DetalheItemPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void CheckEdit_CheckedChanged(object sender, System.EventArgs e)
        {
            if((sender as DevExpress.XamarinForms.Editors.CheckEdit).IsChecked.Value)
            {
                if ((BindingContext as DetalheItemPageViewModel).Sabores?.Count > 0)
                    if ((BindingContext as DetalheItemPageViewModel).Sabores.ToList().Count(p => p.Incluido) >
                        (BindingContext as DetalheItemPageViewModel).Produto.Max_sabor)
                    {
                        (BindingContext as DetalheItemPageViewModel).dialogService.DisplayAlertAsync("Mensagem",
                            "Produto aceita no maximo <" + (BindingContext as DetalheItemPageViewModel).Produto.Max_sabor.ToString() +
                            "> sabores.", "OK");
                        (sender as DevExpress.XamarinForms.Editors.CheckEdit).IsChecked = false;
                    }
            }
        }
    }
}
