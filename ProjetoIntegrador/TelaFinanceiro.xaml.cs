using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace ProjetoIntegrador
{
    /// <summary>
    /// Interação lógica para TelaFinanceiro.xam
    /// </summary>
    public partial class TelaFinanceiro : Page
    {
        public ChartViewModel[] Series { get; set; }

        public TelaFinanceiro()
        {
            InitializeComponent();

            // DataContext dos gráficos
            Ind.DataContext = new ChartViewModel();
            PZ1.DataContext = new ViewModelPizza1();
            PZ2.DataContext = new ViewModelPizza2();

            ResetarBotoes();

            BotaoFinanceiro.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7C3AED"));
        }

        private void BotaoEstoque_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }

        private void BotaoVendas_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaVendas());
        }

        private void BotaoHistorico_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaHistorico());
        }

        private void BotaoSair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

        private void ButtonEmpresa_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaEmpresa());
        }

       
    }

    //======================================
    // GRÁFICO DE INDICADORES (BARRAS)
    //======================================
    public class ChartViewModel
    {
        public ISeries[] Series { get; set; }

        public Axis[] XAxes { get; set; }

        public LabelVisual Title { get; set; } =
            new LabelVisual
            {
                Text = "LUCROS MENSAIS",
                TextSize = 14,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

        public ChartViewModel()
        {
            Series = new ISeries[]
            {
                new RowSeries<double>
                {
                    Values = new double[] { 9623 },
                    Name = "Verstappen",
                    Fill = new SolidColorPaint(SKColors.Red),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 14,
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Middle,
                    DataLabelsFormatter = point => $"Verstappen {point.Coordinate.PrimaryValue}"
                },
                new RowSeries<double>
                {
                    Values = new double[] { 94860 },
                    Name = "Sainz",
                    Fill = new SolidColorPaint(SKColors.Green),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 14,
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Middle,
                    DataLabelsFormatter = point => $"Sainz {point.Coordinate.PrimaryValue}"
                },
                new RowSeries<double>
                {
                    Values = new double[] { 9366 },
                    Name = "Hamilton",
                    Fill = new SolidColorPaint(SKColors.DodgerBlue),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 14,
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Middle,
                    DataLabelsFormatter = point => $"Hamilton {point.Coordinate.PrimaryValue}"
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    MinLimit = 8800,
                    MaxLimit = 10000
                }
            };
        }
    }

    //======================================
    // GRÁFICO DE PIZZA 1
    //======================================
    public class ViewModelPizza1
    {
        public IEnumerable<ISeries> Series { get; set; }

        public LabelVisual Title1 { get; set; } =
            new LabelVisual
            {
                Text = "DISTRIBUIÇÃO DE VENDAS",
                TextSize = 16,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

        public ViewModelPizza1()
        {
            Series = new[] { 2, 4, 1, 4, 3 }.AsPieSeries();
        }
    }

    //======================================
    // GRÁFICO DE PIZZA 2
    //======================================
    public class ViewModelPizza2
    {
        public IEnumerable<ISeries> Series { get; set; }

        public LabelVisual Title2 { get; set; } =
            new LabelVisual
            {
                Text = "META MENSAL",
                TextSize = 16,
                Padding = new LiveChartsCore.Drawing.Padding(20),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

        public ViewModelPizza2()
        {
            Series = new[] { 2, 4, 1, 4, 3 }.AsPieSeries();
        }

    }

}