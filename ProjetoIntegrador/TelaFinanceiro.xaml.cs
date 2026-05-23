using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
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
    /// Interação lógica para TelaFinanceiro.xam
    /// </summary>
    public partial class TelaFinanceiro : Page
    {
        public ISeries[] Series { get; set; }
        public TelaFinanceiro()
        {

            InitializeComponent();

            Series = new ISeries[]
            {
            new ColumnSeries<double>
        {
            Values = new double[]
            {
                12, 25, 40, 18, 50, 70
            }
        }
            };

            DataContext = this;
        }

        private void BotaoHistorico_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaHistorico());
        }

        private void BotaoEstoque_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }
    }
}