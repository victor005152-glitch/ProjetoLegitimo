using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static ProjetoIntegrador.Home;

namespace ProjetoIntegrador
{
    public partial class TelaVendas : Page
    {
        // Lista de itens da venda
        private ObservableCollection<ItemVenda> itensVenda = new ObservableCollection<ItemVenda>();
        private string formaPagamentoSelecionada = "Dinheiro";
        private decimal subtotal = 0;
        private decimal descontoGeral = 0;
        private decimal total = 0;

        public TelaVendas()
        {
            InitializeComponent();
            DGVendas.ItemsSource = itensVenda;
            AtualizarResumo();
            ResetarBotoes();

            BotaoVendas.Background =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#FF7C3AED"));


            AtualizarResumo();

        }

        // Classe para representar um item na venda
        public class ItemVenda
        {
            public string CodigoBarras { get; set; }
            public string Nome { get; set; }
            public int Quantidade { get; set; }
            public decimal ValorUnitario { get; set; }
            public decimal Desconto { get; set; }
            public decimal Subtotal { get { return (ValorUnitario * Quantidade) - Desconto; } }
        }

        // Eventos dos botões do menu
        private void BotaoEstoque_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Home());
        }

        private void BotaoHistorico_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new TelaHistorico());
        }

        private void BotaoFinanceiro_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new TelaFinanceiro());
        }

        private void BotaoSair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Pesquisa de produto
        private void PesquisaProduto_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PesquisaProduto.Text == "Código do produto...")
            {
                PesquisaProduto.Text = "";
            }
        }

        private void PesquisaProduto_LostFocus(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(PesquisaProduto.Text))
            {
                PesquisaProduto.Text = "Código do produto...";
            }
        }

        private void PesquisaProduto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdicionarProduto();
            }
        }

        // Adicionar produto à venda
        private void BtnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            AdicionarProduto();
        }

        private void AdicionarProduto()
        {
            if (string.IsNullOrWhiteSpace(PesquisaProduto.Text) ||
                PesquisaProduto.Text == "Código ou nome do produto...")
                return;

            int quantidade = 1;

            if (!int.TryParse(QuantidadeVenda.Text, out quantidade))
                quantidade = 1;

            string sql = @"
        SELECT *
        FROM produtos
        WHERE  codigo_barras LIKE @pesquisa
        OR nome LIKE @nome
        LIMIT 1";

            MySqlCommand cmd =
                new MySqlCommand(sql, ConectBd.Conexao);

            cmd.Parameters.AddWithValue("@pesquisa", PesquisaProduto.Text);
            cmd.Parameters.AddWithValue("@nome", "%" + PesquisaProduto.Text + "%");

            using (MySqlDataReader leitor = cmd.ExecuteReader())
            {
                if (leitor.Read())
                {
                    string codigo = leitor["codigo_barras"].ToString();
                    string nome = leitor["nome"].ToString();
                    decimal valorVenda =
                        Convert.ToDecimal(leitor["valor_venda"]);

                    var itemExistente =
                        itensVenda.FirstOrDefault(i => i.CodigoBarras == codigo);

                    if (itemExistente != null)
                    {
                        itemExistente.Quantidade += quantidade;

                        DGVendas.Items.Refresh();
                    }
                    else
                    {
                        itensVenda.Add(new ItemVenda
                        {
                            CodigoBarras = codigo,
                            Nome = nome,
                            Quantidade = quantidade,
                            ValorUnitario = valorVenda,
                            Desconto = 0
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Produto não encontrado.");
                    return;
                }
            }

            PesquisaProduto.Clear();
            QuantidadeVenda.Text = "1";

            AtualizarResumo();
        }
        // Remover item selecionado
        private void BtnRemoverItem_Click(object sender, RoutedEventArgs e)
        {
            if (DGVendas.SelectedItem != null)
            {
                itensVenda.Remove((ItemVenda)DGVendas.SelectedItem);
                AtualizarResumo();
            }
            else
            {
                MessageBox.Show("Selecione um item para remover.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Limpar toda a venda
        private void BtnLimparVenda_Click(object sender, RoutedEventArgs e)
        {
            if (itensVenda.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Deseja realmente limpar todos os itens?",
                    "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    itensVenda.Clear();
                    DescontoGeral.Text = "0,00";
                    ValorRecebido.Text = "0,00";
                    AtualizarResumo();
                }
            }
        }

        // Seleção da forma de pagamento
        private void BtnDinheiro_Click(object sender, RoutedEventArgs e)
        {
            formaPagamentoSelecionada = "Dinheiro";
            LabelPagamento.Content = "Pagamento: Dinheiro";
            ResetarBotoesPagamento();
            BtnDinheiro.Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF3B82F6"));
        }

        private void BtnCartao_Click(object sender, RoutedEventArgs e)
        {
            formaPagamentoSelecionada = "Cartão";
            LabelPagamento.Content = "Pagamento: Cartão";
            ResetarBotoesPagamento();
            BtnCartao.Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF3B82F6"));
        }

        private void BtnPix_Click(object sender, RoutedEventArgs e)
        {
            formaPagamentoSelecionada = "Pix";
            LabelPagamento.Content = "Pagamento: Pix";
            ResetarBotoesPagamento();
            BtnPix.Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF3B82F6"));
        }

        private void BtnBoleto_Click(object sender, RoutedEventArgs e)
        {
            formaPagamentoSelecionada = "Boleto";
            LabelPagamento.Content = "Pagamento: Boleto";
            ResetarBotoesPagamento();
            BtnBoleto.Background = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF3B82F6"));
        }

        private void ResetarBotoesPagamento()
        {
            var corPadrao = new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF1F2937"));

            BtnDinheiro.Background = corPadrao;
            BtnCartao.Background = corPadrao;
            BtnPix.Background = corPadrao;
            BtnBoleto.Background = corPadrao;
        }

        // Atualizar desconto
        private void DescontoGeral_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (DescontoGeral.Text == "0,00")
            {

                DescontoGeral.Clear();
            }
            if (decimal.TryParse(DescontoGeral.Text, out decimal desconto))
            {
                descontoGeral = desconto;
                AtualizarResumo();
            }

        }

        // Atualizar valor recebido e calcular troco
        private void ValorRecebido_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LabelTroco == null)
                return;

            if (decimal.TryParse(ValorRecebido.Text, out decimal recebido))
            {
                decimal troco = recebido - total;

                if (troco < 0)
                    troco = 0;

                LabelTroco.Content = $"R$ {troco:N2}";
            }
        }

        // Atualizar resumo da venda
        private void AtualizarResumo()
        {
            subtotal = itensVenda.Sum(i => i.Subtotal);

            // descontoGeral agora é porcentagem
            decimal valorDesconto = subtotal * (descontoGeral / 100);

            total = subtotal - valorDesconto;

            if (LabelSubtotal != null)
                LabelSubtotal.Content = $"R$ {subtotal:N2}";

            if (LabelTotal != null)
                LabelTotal.Content = $"R$ {total:N2}";

            if (TotalItens != null)
                TotalItens.Content = itensVenda.Sum(i => i.Quantidade).ToString();

            if (ValorTotalVenda != null)
                ValorTotalVenda.Content = $"R$ {total:N2}";

            if (LabelTroco != null &&
                ValorRecebido != null &&
                decimal.TryParse(ValorRecebido.Text, out decimal recebido))
            {
                decimal troco = recebido - total;

                if (troco < 0)
                    troco = 0;

                LabelTroco.Content = $"R$ {troco:N2}";
            }
        }
        // Finalizar venda
        private void BtnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (itensVenda.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um produto à venda.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (decimal.TryParse(ValorRecebido.Text, out decimal recebido) && recebido < total)
            {
                MessageBox.Show("Valor recebido é menor que o total da venda.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Confirmar venda?\n\nTotal: R$ {total:N2}\nPagamento: {formaPagamentoSelecionada}",
                "Finalizar Venda",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Aqui você deve salvar a venda no banco de dados
                // RegistrarVenda();

                MessageBox.Show("Venda finalizada com sucesso!", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                try
                {

                    string sql = @"
            UPDATE produtos
            SET quantidade_estoque = quantidade_estoque - @qtd
            WHERE codigo_barras = @codigo";


                    for (int i = 0; i < DGVendas.SelectedItems.Count; i++)
                    {
                        if (DGVendas.SelectedItems[i] is ItemVenda prd)
                        {

                            MySqlCommand cmd =
                                new MySqlCommand(sql, ConectBd.Conexao);

                            cmd.Parameters.AddWithValue("@qtd", prd.Quantidade);
                            cmd.Parameters.AddWithValue("@codigo", prd.CodigoBarras);

                            int linhas = cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    if (ex.Number == 1062)
                    {
                        MessageBox.Show("Este usuário já existe.");
                    }
                    else
                    {
                        MessageBox.Show($"Erro: {ex.Message}");
                    }
                }
                // Limpar para próxima venda
                itensVenda.Clear();
                DescontoGeral.Text = "0,00";
                ValorRecebido.Text = "0,00";

                AtualizarResumo();
            }
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

        private void PesquisaProduto_TextChanged(object sender, TextChangedEventArgs e)
        {


        }
    }
}