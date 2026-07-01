using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjetoIntegrador
{
    public partial class TelaComprovante : Page
    {
        private int vendaId;

        public TelaComprovante(int idVenda)
        {
            InitializeComponent();

            vendaId = idVenda;

            CarregarComprovante();
        }

        private void CarregarComprovante()
        {
            try
            {
                if (ConectBd.Conexao.State != ConnectionState.Open)
                {
                    ConectBd.Conexao.Open();
                }

                CarregarEmpresa();
                CarregarVenda();
                CarregarItens();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar comprovante: " + ex.Message);
            }
        }

        private void CarregarEmpresa()
        {
            string sql = @"
                SELECT *
                FROM empresa_config
                ORDER BY id
                LIMIT 1";

            using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
            using (MySqlDataReader leitor = comando.ExecuteReader())
            {
                if (leitor.Read())
                {
                    string nomeFantasia = leitor["nome_fantasia"].ToString();
                    string razaoSocial = leitor["razao_social"].ToString();
                    string cnpj = leitor["cnpj"].ToString();
                    string telefone = leitor["telefone"].ToString();
                    string email = leitor["email"].ToString();
                    string endereco = leitor["endereco"].ToString();
                    string numero = leitor["numero"].ToString();
                    string bairro = leitor["bairro"].ToString();
                    string cidade = leitor["cidade"].ToString();
                    string estado = leitor["estado"].ToString();
                    string cep = leitor["cep"].ToString();
                    string mensagemRodape = leitor["mensagem_rodape"].ToString();

                    TxtEmpresa.Text = nomeFantasia;
                    TxtRazaoSocial.Text = razaoSocial;
                    TxtCnpj.Text = "CNPJ: " + cnpj;

                    TxtEndereco.Text =
                        endereco + ", " + numero + " - " + bairro + "\n" +
                        cidade + "/" + estado + " - CEP: " + cep;

                    TxtContato.Text = "Telefone: " + telefone + " | E-mail: " + email;

                    if (!string.IsNullOrWhiteSpace(mensagemRodape))
                    {
                        TxtMensagemRodape.Text = mensagemRodape;
                    }
                }
                else
                {
                    TxtEmpresa.Text = "EMPRESA NÃO CADASTRADA";
                    TxtRazaoSocial.Text = "";
                    TxtCnpj.Text = "";
                    TxtEndereco.Text = "";
                    TxtContato.Text = "";
                    TxtMensagemRodape.Text = "Obrigado pela preferência!";
                }
            }
        }

        private void CarregarVenda()
        {
            string sql = @"
                SELECT *
                FROM vendas
                WHERE id = @id";

            using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
            {
                comando.Parameters.AddWithValue("@id", vendaId);

                using (MySqlDataReader leitor = comando.ExecuteReader())
                {
                    if (leitor.Read())
                    {
                        DateTime dataVenda = Convert.ToDateTime(leitor["data_venda"]);
                        decimal subtotal = Convert.ToDecimal(leitor["subtotal"]);
                        decimal descontoValor = Convert.ToDecimal(leitor["desconto_valor"]);
                        decimal total = Convert.ToDecimal(leitor["total"]);

                        string formaPagamento = leitor["forma_pagamento"].ToString();
                        string operador = leitor["operador_nome"].ToString();
                        string clienteNome = leitor["cliente_nome"].ToString();
                        string clienteCpf = leitor["cliente_cpf"].ToString();

                        decimal valorRecebido = 0;
                        decimal troco = 0;

                        if (leitor["valor_recebido"] != DBNull.Value)
                        {
                            valorRecebido = Convert.ToDecimal(leitor["valor_recebido"]);
                        }

                        if (leitor["troco"] != DBNull.Value)
                        {
                            troco = Convert.ToDecimal(leitor["troco"]);
                        }

                        TxtNumeroVenda.Text = "Venda Nº " + vendaId.ToString("D6");
                        TxtDataVenda.Text = dataVenda.ToString("dd/MM/yyyy HH:mm");
                        TxtOperador.Text = string.IsNullOrWhiteSpace(operador) ? "Operador" : operador;
                        TxtPagamento.Text = formaPagamento;

                        if (!string.IsNullOrWhiteSpace(clienteNome))
                        {
                            TxtCliente.Text = "Cliente: " + clienteNome;

                            if (!string.IsNullOrWhiteSpace(clienteCpf))
                            {
                                TxtCliente.Text += " | CPF: " + clienteCpf;
                            }
                        }
                        else
                        {
                            TxtCliente.Text = "";
                        }

                        TxtSubtotal.Text = "R$ " + subtotal.ToString("N2");
                        TxtDesconto.Text = "R$ " + descontoValor.ToString("N2");
                        TxtTotal.Text = "R$ " + total.ToString("N2");

                        if (formaPagamento == "Dinheiro")
                        {
                            TxtRecebido.Text = "R$ " + valorRecebido.ToString("N2");
                            TxtTroco.Text = "R$ " + troco.ToString("N2");
                        }
                        else
                        {
                            TxtRecebido.Text = "-";
                            TxtTroco.Text = "-";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Venda não encontrada.");
                    }
                }
            }
        }

        private void CarregarItens()
        {
            ListaItens.Items.Clear();

            string sql = @"
                SELECT 
                    nome_produto,
                    quantidade,
                    valor_unitario,
                    subtotal
                FROM venda_itens
                WHERE venda_id = @venda_id
                ORDER BY id";

            using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
            {
                comando.Parameters.AddWithValue("@venda_id", vendaId);

                using (MySqlDataReader leitor = comando.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        string nomeProduto = leitor["nome_produto"].ToString();
                        int quantidade = Convert.ToInt32(leitor["quantidade"]);
                        decimal valorUnitario = Convert.ToDecimal(leitor["valor_unitario"]);
                        decimal subtotal = Convert.ToDecimal(leitor["subtotal"]);

                        Grid linha = new Grid
                        {
                            Margin = new Thickness(0, 0, 0, 8)
                        };

                        linha.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        linha.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(55) });
                        linha.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(95) });

                        TextBlock txtProduto = new TextBlock
                        {
                            Text = nomeProduto + "\nR$ " + valorUnitario.ToString("N2") + " un.",
                            FontSize = 13,
                            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF111827")),
                            TextWrapping = TextWrapping.Wrap
                        };

                        TextBlock txtQtd = new TextBlock
                        {
                            Text = quantidade.ToString(),
                            FontSize = 13,
                            TextAlignment = TextAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        TextBlock txtSubtotal = new TextBlock
                        {
                            Text = "R$ " + subtotal.ToString("N2"),
                            FontSize = 13,
                            FontWeight = FontWeights.SemiBold,
                            TextAlignment = TextAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        Grid.SetColumn(txtProduto, 0);
                        Grid.SetColumn(txtQtd, 1);
                        Grid.SetColumn(txtSubtotal, 2);

                        linha.Children.Add(txtProduto);
                        linha.Children.Add(txtQtd);
                        linha.Children.Add(txtSubtotal);

                        ListaItens.Items.Add(linha);
                    }
                }
            }
        }

        private void BotaoImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(AreaComprovante, "Comprovante de Venda");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao imprimir: " + ex.Message);
            }
        }

        private void BotaoVoltar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaVendas());
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

        private void BotaoFinanceiro_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaFinanceiro());
        }

        private void BotaoEmpresa_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaEmpresa());
        }

        private void BotaoSair_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaLogin());
        }
    }
}