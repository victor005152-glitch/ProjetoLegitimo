using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using static ProjetoIntegrador.ChartViewModel;
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



            Ind.DataContext = new ChartViewModel();
            PZ1.DataContext = new ViewModel();
            PZ2.DataContext = new ViewModel();
        }


        private void BotaoHistorico_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaHistorico());
        }

        private void BotaoEstoque_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }

<<<<<<< HEAD
        private void Ind_Loaded(object sender, RoutedEventArgs e)
        {

=======
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaLogin());
>>>>>>> 52fa3ef6f7fa336d93bafb61afbabc256b9eaaf0
        }
    }

    //======================================
    //GRAFICO DE INDICADORES
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
    //GRAFICO DE PIZZA1
    //======================================

    public partial class ViewModel : ObservableObject
    {

        public IEnumerable<ISeries> Series { get; set; } =
   new[] { 2, 4, 1, 4, 3 }.AsPieSeries();
        public IEnumerable<ISeries> Series1 { get; set; } =
            new[]

            {
            new PieSeries<int> { Values = new[]{ 2 } },
            new PieSeries<int> { Values = new[]{ 4 } },
            new PieSeries<int> { Values = new[]{ 1 } },
            new PieSeries<int> { Values = new[]{ 4 } },
            new PieSeries<int> { Values = new[]{ 3 } },
            };


        public LabelVisual Title1 { get; set; } =
            new LabelVisual
            {
                Text = "My chart title",
                TextSize = 16,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

    }


    //======================================
    //GRAFICO DE PIZZA2
    //======================================

    public partial class ViewModel : ObservableObject
    {

        public IEnumerable<ISeries> Series2 { get; set; } =
   new[] { 2, 4, 1, 4, 3 }.AsPieSeries();
        public IEnumerable<ISeries> Series3 { get; set; } =
            new[]
            {
            new PieSeries<int> { Values = new[]{ 2 } },
            new PieSeries<int> { Values = new[]{ 4 } },
            new PieSeries<int> { Values = new[]{ 1 } },
            new PieSeries<int> { Values = new[]{ 4 } },
            new PieSeries<int> { Values = new[]{ 3 } },
            };

        public LabelVisual Title2 { get; set; } =
            new LabelVisual
            {
                Text = "META MENSAL",
                TextSize = 16,
                Padding = new LiveChartsCore.Drawing.Padding(20),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };
    }
}
