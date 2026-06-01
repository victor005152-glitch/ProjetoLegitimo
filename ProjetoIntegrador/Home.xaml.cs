using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
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

        public class Produto
        {
            public string CodigoBarras { get; set; }
            public string Nome { get; set; }
            public string Categoria { get; set; }
            public string Marca { get; set; }
            public int QuantidadeEstoque { get; set; }
            public decimal ValorCusto { get; set; }
            public decimal ValorVenda { get; set; }
        }



        public Home()
        {
            InitializeComponent();

            Cadastro.Visibility = Visibility.Collapsed;
            CollumCusto.Visibility = Visibility.Collapsed;

            Mostrar_DataGrid();
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

            //CollumCusto.Visibility = Visibility.Visible;//
            try
            {
                string sql = "INSERT INTO Produtos (nome, codigo_barras, categoria, marca, valor_custo, valor_venda, quantidade_estoque) VALUES (@nome, @codigo_barras, @categoria, @marca, @valor_custo, @valor_venda, @quantidade_estoque)";

                MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao);
                comando.Parameters.AddWithValue("@codigo_barras", Codigo1.Text);
                comando.Parameters.AddWithValue("@nome", Nome1.Text);
                comando.Parameters.AddWithValue("@categoria", Categoria1.Text);
                comando.Parameters.AddWithValue("@marca", Marca1.Text);
                comando.Parameters.AddWithValue("@quantidade_estoque", Quantidade1.Text);
                comando.Parameters.AddWithValue("valor_custo", Custo1.Text);
                comando.Parameters.AddWithValue("@valor_venda", Venda1.Text);

                comando.ExecuteNonQuery();
                Cadastro.Visibility = Visibility.Collapsed;


                Mostrar_DataGrid();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Erro: {ex.Message}");
            }
        }
        private void Mostrar_DataGrid()
        {
            DGestoque.Visibility = Visibility.Visible;
            string sql = "SELECT * FROM produtos";
            MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao);
            using (MySqlDataReader leitor = comando.ExecuteReader())
            {
                List<Produto> produtos = new List<Produto>();
                while (leitor.Read())
                {
                    Produto produto = new Produto
                    {
                        CodigoBarras = leitor["codigo_barras"].ToString(),
                        Nome = leitor["nome"].ToString(),
                        Categoria = leitor["categoria"].ToString(),
                        Marca = leitor["marca"].ToString(),
                        QuantidadeEstoque = Convert.ToInt32(leitor["quantidade_estoque"]),
                        ValorCusto = Convert.ToDecimal(leitor["valor_custo"]),
                        ValorVenda = Convert.ToDecimal(leitor["valor_venda"])
                    };
                    produtos.Add(produto);
                }
                DGestoque.ItemsSource = produtos;
                leitor.Close();
            }
        }

        private void Exc_Click(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < DGestoque.SelectedItems.Count; i++)
            {
                if (DGestoque.SelectedItems[i] is Produto prd)
                {
                    string sql = $"DELETE FROM produtos WHERE codigo_barras = @cod";
                    MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao);
                    comando.Parameters.AddWithValue("@cod", prd.CodigoBarras);
                    comando.ExecuteNonQuery();
                }
            }
            Mostrar_DataGrid();
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

        private void Cad_Click1(object sender, RoutedEventArgs e)
        {
            Cadastro.Visibility = Visibility.Visible;
        }
    }
}
