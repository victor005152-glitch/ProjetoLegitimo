using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace ProjetoIntegrador
{
    /// <summary>
    /// Interação lógica para Home.xaml
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

            ResetarBotoes();

            BotaoEstoque.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7C3AED"));

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
            try
            {
                string verificar =
                    "SELECT COUNT(*) FROM produtos WHERE codigo_barras = @codigo";

                MySqlCommand verificarCmd =
                    new MySqlCommand(verificar, ConectBd.Conexao);

                verificarCmd.Parameters.AddWithValue("@codigo", Codigo1.Text);

                int existe = Convert.ToInt32(verificarCmd.ExecuteScalar());

                if (existe > 0)
                {
                    MessageBox.Show("Esse código de barras já está cadastrado!");
                    return;
                }

                string sql =
                    "INSERT INTO Produtos (nome, codigo_barras, categoria, marca, valor_custo, valor_venda, quantidade_estoque) " +
                    "VALUES (@nome, @codigo_barras, @categoria, @marca, @valor_custo, @valor_venda, @quantidade_estoque)";

                MySqlCommand comando =
                    new MySqlCommand(sql, ConectBd.Conexao);

                comando.Parameters.AddWithValue("@codigo_barras", Codigo1.Text);
                comando.Parameters.AddWithValue("@nome", Nome1.Text);
                comando.Parameters.AddWithValue("@categoria", Categoria1.Text);
                comando.Parameters.AddWithValue("@marca", Marca1.Text);
                comando.Parameters.AddWithValue("@quantidade_estoque", Quantidade1.Text);
                comando.Parameters.AddWithValue("@valor_custo", Custo1.Text);
                comando.Parameters.AddWithValue("@valor_venda", Venda1.Text);

                comando.ExecuteNonQuery();

                Mostrar_DataGrid();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Erro: {ex.Message}");
            }
            NavigationService.Navigate(new Home());
        }

        private void Mostrar_DataGrid()
        {
            DGestoque.Visibility = Visibility.Visible;

            string sql = "SELECT * FROM produtos";

            MySqlCommand comando =
                new MySqlCommand(sql, ConectBd.Conexao);

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
                QTD.Content = produtos.Sum(p => p.QuantidadeEstoque).ToString();
                DGestoque.ItemsSource = produtos;
            }
        }

        private void Exc_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < DGestoque.SelectedItems.Count; i++)
            {
                if (DGestoque.SelectedItems[i] is Produto prd)
                {
                    string sql =
                        "DELETE FROM produtos WHERE codigo_barras = @cod";

                    MySqlCommand comando =
                        new MySqlCommand(sql, ConectBd.Conexao);

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

        private void Pesquisa1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Pesquisa1.Text == "Pesquisar...")
            {
                return;
            }

            string sql = @"
                SELECT * FROM produtos
                WHERE
                    nome LIKE @pesquisa OR
                    categoria LIKE @pesquisa OR
                    marca LIKE @pesquisa OR
                    codigo_barras LIKE @pesquisa";

            MySqlCommand comando =
                new MySqlCommand(sql, ConectBd.Conexao);

            comando.Parameters.AddWithValue(
                "@pesquisa",
                "%" + Pesquisa1.Text + "%");

            List<Produto> produtos = new List<Produto>();

            using (MySqlDataReader leitor = comando.ExecuteReader())
            {
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
            }

            DGestoque.ItemsSource = produtos;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaLogin());
        }

        private void Cad_Click1(object sender, RoutedEventArgs e)
        {
            Cadastro.Visibility = Visibility.Visible;
        }

        private void Quantidade_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Cad_Click(sender, e);
            }
        }

        private void Fechar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }

        private void ResetarBotoes()
        {
            BotaoEstoque.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F2937"));

            BotaoHistorico.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F2937"));

            BotaoFinanceiro.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F2937"));

            BotaoVendas.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F2937"));
        }

        private void ATL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Codigo.Text))
                {
                    MessageBox.Show("Informe o código do produto.");
                    return;
                }

                if (!int.TryParse(QTD1.Text, out int qtd))
                {
                    MessageBox.Show("Informe uma quantidade válida.");
                    return;
                }

                string sql = @"
            UPDATE produtos
            SET quantidade_estoque = quantidade_estoque + @qtd
            WHERE codigo_barras = @codigo";

                MySqlCommand cmd =
                    new MySqlCommand(sql, ConectBd.Conexao);

                cmd.Parameters.AddWithValue("@qtd", qtd);
                cmd.Parameters.AddWithValue("@codigo", Codigo.Text);

                int linhas = cmd.ExecuteNonQuery();

                if (linhas == 0)
                {
                    MessageBox.Show("Produto não encontrado.");
                    return;
                }

                MessageBox.Show("Estoque atualizado com sucesso!");

                QTD1.Clear();
                Codigo.Clear();

                AtualizarEstoque.Visibility = Visibility.Collapsed;

                Mostrar_DataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private void ATL_Click1(object sender, RoutedEventArgs e)
        {
            AtualizarEstoque.Visibility = Visibility.Visible;
        }
        private void QTD1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Atualizar.RaiseEvent(
                    new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void BotaoVendas_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaVendas());
        }
    }
}
