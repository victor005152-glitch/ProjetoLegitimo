using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjetoIntegrador
{
    /// <summary>
    /// Interação lógica para Home.xam
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void BotaoEstoque_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaHistorico());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaFinanceiro());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cad_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Exc_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Pesquisa_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Pesquisa1.Text == "Pesquisar...")
            {
                Pesquisa1.Text = "";
            }
        }

        private void Pesquisa_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Pesquisa1.Text))
            {
                Pesquisa1.Text = "Pesquisar...";
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaLogin());
        }
    }
}
