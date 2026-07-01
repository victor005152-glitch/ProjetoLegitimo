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
    /// Interação lógica para TelaHistorico.xam
    /// </summary>
    public partial class TelaHistorico : Page
    {
        public TelaHistorico()
        {
            InitializeComponent();

            ResetarBotoes();

            BotaoHistorico.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7C3AED"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaFinanceiro());
        }

        private void BotaoEstoque_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }

        private void BotaoVendas_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaVendas());
        }

        private void Pesquisa_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Pesquisa.Text == "Pesquisar...")
            {
                Pesquisa.Text = "";
            }
        }

        private void Pesquisa_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Pesquisa.Text))
            {
                Pesquisa.Text = "Pesquisar...";
            }
        }

        private void BotaoEmpresa_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaLogin());
        }

private void ResetarBotoes()
        {
            BotaoEstoque.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F2937"));

            BotaoVendas.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F2937"));

            BotaoHistorico.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F2937"));

            BotaoFinanceiro.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F2937"));

            BotaoEmpresa.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F2937"));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaEmpresa());
        }

        private void BotaoSair_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaLogin());
        }
    }

}
