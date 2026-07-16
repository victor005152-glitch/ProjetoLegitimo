using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjetoIntegrador
{
    public partial class TelaHistorico : Page
    {
        private ObservableCollection<HistoricoVenda> vendasHistorico =
            new ObservableCollection<HistoricoVenda>();

        private const string PlaceholderPesquisa = "Pesquisar venda...";

        public TelaHistorico()
        {
            InitializeComponent();

            Historico1.ItemsSource = vendasHistorico;

            ResetarBotoes();

            BotaoHistorico.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7C3AED"));

            Pesquisa.Text = PlaceholderPesquisa;

            CarregarHistorico();
        }

        public class HistoricoVenda
        {
            public int VendaId { get; set; }
            public string CodigoVenda { get; set; } = "";
            public DateTime DataVenda { get; set; }
            public string Cliente { get; set; } = "";
            public string Cpf { get; set; } = "";
            public string FormaPagamento { get; set; } = "";
            public decimal Total { get; set; }
            public string Operador { get; set; } = "";
            public int QuantidadeItens { get; set; }
            public string Produtos { get; set; } = "";

            public string DataFormatada
            {
                get { return DataVenda.ToString("dd/MM/yyyy HH:mm"); }
            }

            public string TotalFormatado
            {
                get { return "R$ " + Total.ToString("N2"); }
            }
        }

        private void AbrirConexao()
        {
            if (ConectBd.Conexao.State != ConnectionState.Open)
            {
                ConectBd.Conexao.Open();
            }
        }

        private void CarregarHistorico(string pesquisa = "")
        {
            vendasHistorico.Clear();

            try
            {
                AbrirConexao();

                pesquisa = pesquisa.Trim();

                if (pesquisa == PlaceholderPesquisa)
                {
                    pesquisa = "";
                }

                string sql = @"
                    SELECT
                        v.id,
                        v.data_venda,
                        COALESCE(v.cliente_nome, '') AS cliente_nome,
                        COALESCE(v.cliente_cpf, '') AS cliente_cpf,
                        COALESCE(v.forma_pagamento, '') AS forma_pagamento,
                        COALESCE(v.total, 0) AS total,
                        COALESCE(v.operador_nome, '') AS operador_nome,
                        COALESCE(SUM(vi.quantidade), 0) AS quantidade_itens,
                        COALESCE(GROUP_CONCAT(CONCAT(vi.quantidade, 'x ', vi.nome_produto) SEPARATOR ', '), '') AS produtos
                    FROM vendas v
                    LEFT JOIN venda_itens vi ON vi.venda_id = v.id
                    WHERE
                        @pesquisa = ''
                        OR CAST(v.id AS CHAR) LIKE @like
                        OR LPAD(v.id, 6, '0') LIKE @like
                        OR DATE_FORMAT(v.data_venda, '%d/%m/%Y') LIKE @like
                        OR DATE_FORMAT(v.data_venda, '%d/%m/%Y %H:%i') LIKE @like
                        OR v.cliente_nome LIKE @like
                        OR v.cliente_cpf LIKE @like
                        OR v.forma_pagamento LIKE @like
                        OR v.operador_nome LIKE @like
                        OR vi.nome_produto LIKE @like
                        OR vi.codigo_barras LIKE @like
                    GROUP BY
                        v.id,
                        v.data_venda,
                        v.cliente_nome,
                        v.cliente_cpf,
                        v.forma_pagamento,
                        v.total,
                        v.operador_nome
                    ORDER BY v.data_venda DESC";

                using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
                {
                    comando.Parameters.AddWithValue("@pesquisa", pesquisa);
                    comando.Parameters.AddWithValue("@like", "%" + pesquisa + "%");

                    using (MySqlDataReader leitor = comando.ExecuteReader())
                    {
                        while (leitor.Read())
                        {
                            int vendaId = Convert.ToInt32(leitor["id"]);

                            string cliente = leitor["cliente_nome"].ToString();

                            if (string.IsNullOrWhiteSpace(cliente))
                            {
                                cliente = "Consumidor";
                            }

                            string operador = leitor["operador_nome"].ToString();

                            if (string.IsNullOrWhiteSpace(operador))
                            {
                                operador = "Operador";
                            }

                            HistoricoVenda venda = new HistoricoVenda
                            {
                                VendaId = vendaId,
                                CodigoVenda = vendaId.ToString("D6"),
                                DataVenda = Convert.ToDateTime(leitor["data_venda"]),
                                Cliente = cliente,
                                Cpf = leitor["cliente_cpf"].ToString(),
                                FormaPagamento = leitor["forma_pagamento"].ToString(),
                                Total = Convert.ToDecimal(leitor["total"]),
                                Operador = operador,
                                QuantidadeItens = Convert.ToInt32(leitor["quantidade_itens"]),
                                Produtos = leitor["produtos"].ToString()
                            };

                            vendasHistorico.Add(venda);
                        }
                    }
                }

                AtualizarCards();
                LimparDetalhes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar histórico: " + ex.Message);
            }
        }

        private void AtualizarCards()
        {
            TotalVendas.Content = vendasHistorico.Count.ToString();

            decimal valorTotal = vendasHistorico.Sum(v => v.Total);
            ValorTotal.Content = "R$ " + valorTotal.ToString("N2");

            int quantidadeItens = vendasHistorico.Sum(v => v.QuantidadeItens);
            TotalItens.Content = quantidadeItens.ToString();
        }

        private void Historico1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HistoricoVenda venda = Historico1.SelectedItem as HistoricoVenda;

            if (venda == null)
            {
                return;
            }

            DetalheVenda.Content = "Venda Nº " + venda.CodigoVenda;

            string produtos = string.IsNullOrWhiteSpace(venda.Produtos)
                ? "-"
                : venda.Produtos;

            DetalheProduto.Text = produtos;

            string cliente = venda.Cliente;

            if (!string.IsNullOrWhiteSpace(venda.Cpf))
            {
                cliente += " - CPF: " + venda.Cpf;
            }

            cliente += " | Operador: " + venda.Operador;

            DetalheCliente.Content = cliente;
            DetalheValor.Content = venda.TotalFormatado;
            DetalheData.Content = venda.DataFormatada;
            DetalhePagamento.Content = venda.FormaPagamento;
        }

        private void LimparDetalhes()
        {
            DetalheVenda.Content = "Selecione uma venda";
            DetalheProduto.Text = "-";
            DetalheCliente.Content = "-";
            DetalheValor.Content = "R$ 0,00";
            DetalheData.Content = "-";
            DetalhePagamento.Content = "-";
        }

        private void BtnComprovante_Click(object sender, RoutedEventArgs e)
        {
            HistoricoVenda venda = Historico1.SelectedItem as HistoricoVenda;

            if (venda == null)
            {
                MessageBox.Show("Selecione uma venda para abrir o comprovante.");
                return;
            }

            NavigationService.Navigate(new TelaComprovante(venda.VendaId));
        }

        private void BotaoAtualizar_Click(object sender, RoutedEventArgs e)
        {
            Pesquisa.Text = PlaceholderPesquisa;
            CarregarHistorico();
        }

        private void Pesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Pesquisa == null)
            {
                return;
            }

            string texto = Pesquisa.Text.Trim();

            if (texto == PlaceholderPesquisa)
            {
                return;
            }

            CarregarHistorico(texto);
        }

        private void Pesquisa_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Pesquisa.Text == PlaceholderPesquisa)
            {
                Pesquisa.Text = "";
            }
        }

        private void Pesquisa_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Pesquisa.Text))
            {
                Pesquisa.Text = PlaceholderPesquisa;
                CarregarHistorico();
            }
        }

        private void ResetarBotoes()
        {
            SolidColorBrush corPadrao =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));

            BotaoEstoque.Background = corPadrao;
            BotaoVendas.Background = corPadrao;
            BotaoHistorico.Background = corPadrao;
            BotaoFinanceiro.Background = corPadrao;
            BotaoEmpresa.Background = corPadrao;
            BotaoClientes.Background = corPadrao;
        }

        private void BotaoEstoque_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }

        private void BotaoVendas_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaVendas());
        }

        private void BotaoFinanceiro_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaFinanceiro());
        }

        private void BotaoEmpresa_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaEmpresa());
        }

        private void BotaoClientes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaClientes());
        }

        private void BotaoSair_Click(object sender, RoutedEventArgs e)
        {
            SessaoUsuario.Limpar();
            NavigationService.Navigate(new TelaLogin());
        }
    }
}