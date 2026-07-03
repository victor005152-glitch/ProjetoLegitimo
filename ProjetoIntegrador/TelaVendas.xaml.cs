using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjetoIntegrador
{
    public partial class TelaVendas : Page
    {
        private ObservableCollection<ItemVenda> itensVenda = new ObservableCollection<ItemVenda>();
        private ObservableCollection<ProdutoEstoque> produtosEstoque = new ObservableCollection<ProdutoEstoque>();
        private ObservableCollection<ProdutoEstoque> produtosFiltrados = new ObservableCollection<ProdutoEstoque>();

        private string formaPagamentoSelecionada = "Dinheiro";
        private decimal subtotal = 0;
        private decimal descontoGeral = 0;
        private decimal total = 0;

        private int clienteSelecionadoId = 0;
        private string clienteSelecionadoNome = "";
        private string clienteSelecionadoCpf = "";

        private const string PlaceholderPesquisa = "Código do produto...";

        public TelaVendas()
        {
            InitializeComponent();

            DGVendas.ItemsSource = itensVenda;
            DGEstoque.ItemsSource = produtosFiltrados;

            CarregarEstoque();
            CarregarClientesVenda();

            AtualizarResumo();
            ResetarBotoes();

            BotaoVendas.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7C3AED"));
        }

        public class ItemVenda
        {
            public int ProdutoId { get; set; }
            public string CodigoBarras { get; set; } = "";
            public string Nome { get; set; } = "";
            public int Quantidade { get; set; }
            public decimal ValorUnitario { get; set; }
            public decimal CustoUnitario { get; set; }
            public decimal Desconto { get; set; }

            public decimal Subtotal
            {
                get { return (ValorUnitario * Quantidade) - Desconto; }
            }

            public decimal Lucro
            {
                get { return Subtotal - (CustoUnitario * Quantidade); }
            }
        }

        public class ProdutoEstoque
        {
            public int Id { get; set; }
            public string CodigoBarras { get; set; } = "";
            public string Nome { get; set; } = "";
            public string Categoria { get; set; } = "";
            public int QuantidadeEstoque { get; set; }
            public decimal ValorVenda { get; set; }
            public decimal ValorCusto { get; set; }
        }

        private void AbrirConexao()
        {
            if (ConectBd.Conexao.State != ConnectionState.Open)
            {
                ConectBd.Conexao.Open();
            }
        }

        private void CarregarEstoque()
        {
            produtosEstoque.Clear();
            produtosFiltrados.Clear();

            try
            {
                AbrirConexao();

                string sql = @"
                    SELECT 
                        id,
                        codigo_barras,
                        nome,
                        categoria,
                        quantidade_estoque,
                        valor_venda,
                        valor_custo
                    FROM produtos
                    ORDER BY nome";

                using (MySqlCommand cmd = new MySqlCommand(sql, ConectBd.Conexao))
                using (MySqlDataReader leitor = cmd.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        ProdutoEstoque produto = new ProdutoEstoque
                        {
                            Id = Convert.ToInt32(leitor["id"]),
                            CodigoBarras = leitor["codigo_barras"].ToString(),
                            Nome = leitor["nome"].ToString(),
                            Categoria = leitor["categoria"].ToString(),
                            QuantidadeEstoque = Convert.ToInt32(leitor["quantidade_estoque"]),
                            ValorVenda = Convert.ToDecimal(leitor["valor_venda"]),
                            ValorCusto = Convert.ToDecimal(leitor["valor_custo"])
                        };

                        produtosEstoque.Add(produto);
                        produtosFiltrados.Add(produto);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar estoque: " + ex.Message);
            }
        }

        private void CarregarClientesVenda()
        {
            try
            {
                AbrirConexao();

                ComboClientes.Items.Clear();

                ComboClientes.Items.Add(new ClienteVenda
                {
                    Id = 0,
                    Nome = "Consumidor não informado",
                    Cpf = ""
                });

                string sql = @"
                    SELECT id, nome, cpf
                    FROM clientes
                    WHERE ativo = 1
                    ORDER BY nome ASC";

                using (MySqlCommand cmd = new MySqlCommand(sql, ConectBd.Conexao))
                using (MySqlDataReader leitor = cmd.ExecuteReader())
                {
                    while (leitor.Read())
                    {
                        ComboClientes.Items.Add(new ClienteVenda
                        {
                            Id = Convert.ToInt32(leitor["id"]),
                            Nome = leitor["nome"].ToString(),
                            Cpf = leitor["cpf"] == DBNull.Value ? "" : leitor["cpf"].ToString()
                        });
                    }
                }

                ComboClientes.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar clientes na venda: " + ex.Message);
            }
        }

        private void ComboClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClienteVenda cliente = ComboClientes.SelectedItem as ClienteVenda;

            if (cliente == null)
            {
                clienteSelecionadoId = 0;
                clienteSelecionadoNome = "";
                clienteSelecionadoCpf = "";
                return;
            }

            clienteSelecionadoId = cliente.Id;
            clienteSelecionadoNome = cliente.Id == 0 ? "" : cliente.Nome;
            clienteSelecionadoCpf = cliente.Cpf;
        }

        private void AtualizarFiltroEstoque()
        {
            if (produtosFiltrados == null || PesquisaProduto == null)
            {
                return;
            }

            produtosFiltrados.Clear();

            string pesquisa = PesquisaProduto.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(pesquisa) ||
                pesquisa == PlaceholderPesquisa.ToLower() ||
                pesquisa == "código ou nome do produto..." ||
                pesquisa == "código do produto.")
            {
                foreach (ProdutoEstoque produto in produtosEstoque)
                {
                    produtosFiltrados.Add(produto);
                }

                return;
            }

            foreach (ProdutoEstoque produto in produtosEstoque)
            {
                bool encontrou =
                    produto.Nome.ToLower().Contains(pesquisa) ||
                    produto.CodigoBarras.ToLower().Contains(pesquisa) ||
                    produto.Categoria.ToLower().Contains(pesquisa);

                if (encontrou)
                {
                    produtosFiltrados.Add(produto);
                }
            }
        }

        private void AdicionarProduto()
        {
            if (DGEstoque.SelectedItem != null)
            {
                AdicionarProdutoSelecionado();
                return;
            }

            if (string.IsNullOrWhiteSpace(PesquisaProduto.Text) ||
                PesquisaProduto.Text == PlaceholderPesquisa ||
                PesquisaProduto.Text == "Código do produto." ||
                PesquisaProduto.Text == "Código ou nome do produto...")
            {
                return;
            }

            string pesquisa = PesquisaProduto.Text.Trim().ToLower();

            ProdutoEstoque produtoEncontrado = produtosEstoque.FirstOrDefault(p =>
                p.CodigoBarras.ToLower() == pesquisa ||
                p.Nome.ToLower().Contains(pesquisa));

            if (produtoEncontrado == null)
            {
                MessageBox.Show("Produto não encontrado.");
                return;
            }

            DGEstoque.SelectedItem = produtoEncontrado;
            AdicionarProdutoSelecionado();

            PesquisaProduto.Clear();
            AtualizarFiltroEstoque();
        }

        private void AdicionarProdutoSelecionado()
        {
            ProdutoEstoque produto = DGEstoque.SelectedItem as ProdutoEstoque;

            if (produto == null)
            {
                MessageBox.Show("Selecione um produto no estoque.");
                return;
            }

            int quantidade = 1;

            if (!int.TryParse(QuantidadeVenda.Text, out quantidade))
            {
                quantidade = 1;
            }

            if (quantidade <= 0)
            {
                MessageBox.Show("Quantidade inválida.");
                return;
            }

            ItemVenda itemExistente =
                itensVenda.FirstOrDefault(i => i.CodigoBarras == produto.CodigoBarras);

            int quantidadeAtualNoCarrinho = itemExistente == null ? 0 : itemExistente.Quantidade;
            int quantidadeFinal = quantidadeAtualNoCarrinho + quantidade;

            if (quantidadeFinal > produto.QuantidadeEstoque)
            {
                MessageBox.Show("Quantidade maior que o estoque disponível.");
                return;
            }

            if (itemExistente != null)
            {
                itemExistente.Quantidade += quantidade;
                DGVendas.Items.Refresh();
            }
            else
            {
                itensVenda.Add(new ItemVenda
                {
                    ProdutoId = produto.Id,
                    CodigoBarras = produto.CodigoBarras,
                    Nome = produto.Nome,
                    Quantidade = quantidade,
                    ValorUnitario = produto.ValorVenda,
                    CustoUnitario = produto.ValorCusto,
                    Desconto = 0
                });
            }

            QuantidadeVenda.Text = "1";
            AtualizarResumo();
        }

        private int SalvarVendaEBaixarEstoque()
        {
            AbrirConexao();

            MySqlTransaction transacao = ConectBd.Conexao.BeginTransaction();

            try
            {
                decimal valorDesconto = subtotal * (descontoGeral / 100);

                decimal? valorRecebido = null;
                decimal? troco = null;

                if (formaPagamentoSelecionada == "Dinheiro")
                {
                    if (decimal.TryParse(ValorRecebido.Text, out decimal recebido))
                    {
                        valorRecebido = recebido;
                        troco = recebido - total;

                        if (troco < 0)
                        {
                            troco = 0;
                        }
                    }
                }

                string sqlVenda = @"
                    INSERT INTO vendas
                    (
                        data_venda,
                        operador_nome,
                        cliente_id,
                        cliente_nome,
                        cliente_cpf,
                        subtotal,
                        desconto_percentual,
                        desconto_valor,
                        total,
                        forma_pagamento,
                        valor_recebido,
                        troco,
                        status_venda
                    )
                    VALUES
                    (
                        NOW(),
                        @operador_nome,
                        @cliente_id,
                        @cliente_nome,
                        @cliente_cpf,
                        @subtotal,
                        @desconto_percentual,
                        @desconto_valor,
                        @total,
                        @forma_pagamento,
                        @valor_recebido,
                        @troco,
                        'Finalizada'
                    )";

                int vendaId = 0;

                using (MySqlCommand cmdVenda = new MySqlCommand(sqlVenda, ConectBd.Conexao, transacao))
                {
                    cmdVenda.Parameters.AddWithValue("@operador_nome",
                        string.IsNullOrWhiteSpace(SessaoUsuario.Nome) ? "Operador" : SessaoUsuario.Nome);

                    cmdVenda.Parameters.AddWithValue("@cliente_id",
                        clienteSelecionadoId > 0 ? (object)clienteSelecionadoId : DBNull.Value);

                    cmdVenda.Parameters.AddWithValue("@cliente_nome",
                        string.IsNullOrWhiteSpace(clienteSelecionadoNome) ? (object)DBNull.Value : clienteSelecionadoNome);

                    cmdVenda.Parameters.AddWithValue("@cliente_cpf",
                        string.IsNullOrWhiteSpace(clienteSelecionadoCpf) ? (object)DBNull.Value : clienteSelecionadoCpf);

                    cmdVenda.Parameters.AddWithValue("@subtotal", subtotal);
                    cmdVenda.Parameters.AddWithValue("@desconto_percentual", descontoGeral);
                    cmdVenda.Parameters.AddWithValue("@desconto_valor", valorDesconto);
                    cmdVenda.Parameters.AddWithValue("@total", total);
                    cmdVenda.Parameters.AddWithValue("@forma_pagamento", formaPagamentoSelecionada);

                    cmdVenda.Parameters.AddWithValue("@valor_recebido",
                        valorRecebido.HasValue ? (object)valorRecebido.Value : DBNull.Value);

                    cmdVenda.Parameters.AddWithValue("@troco",
                        troco.HasValue ? (object)troco.Value : DBNull.Value);

                    cmdVenda.ExecuteNonQuery();

                    vendaId = Convert.ToInt32(cmdVenda.LastInsertedId);
                }

                foreach (ItemVenda item in itensVenda)
                {
                    string sqlItem = @"
                        INSERT INTO venda_itens
                        (
                            venda_id,
                            produto_id,
                            codigo_barras,
                            nome_produto,
                            quantidade,
                            valor_unitario,
                            custo_unitario,
                            desconto,
                            subtotal,
                            lucro
                        )
                        VALUES
                        (
                            @venda_id,
                            @produto_id,
                            @codigo_barras,
                            @nome_produto,
                            @quantidade,
                            @valor_unitario,
                            @custo_unitario,
                            @desconto,
                            @subtotal,
                            @lucro
                        )";

                    using (MySqlCommand cmdItem = new MySqlCommand(sqlItem, ConectBd.Conexao, transacao))
                    {
                        cmdItem.Parameters.AddWithValue("@venda_id", vendaId);
                        cmdItem.Parameters.AddWithValue("@produto_id", item.ProdutoId);
                        cmdItem.Parameters.AddWithValue("@codigo_barras", item.CodigoBarras);
                        cmdItem.Parameters.AddWithValue("@nome_produto", item.Nome);
                        cmdItem.Parameters.AddWithValue("@quantidade", item.Quantidade);
                        cmdItem.Parameters.AddWithValue("@valor_unitario", item.ValorUnitario);
                        cmdItem.Parameters.AddWithValue("@custo_unitario", item.CustoUnitario);
                        cmdItem.Parameters.AddWithValue("@desconto", item.Desconto);
                        cmdItem.Parameters.AddWithValue("@subtotal", item.Subtotal);
                        cmdItem.Parameters.AddWithValue("@lucro", item.Lucro);

                        cmdItem.ExecuteNonQuery();
                    }

                    string sqlEstoque = @"
                        UPDATE produtos
                        SET quantidade_estoque = quantidade_estoque - @quantidade
                        WHERE id = @produto_id
                          AND quantidade_estoque >= @quantidade";

                    using (MySqlCommand cmdEstoque = new MySqlCommand(sqlEstoque, ConectBd.Conexao, transacao))
                    {
                        cmdEstoque.Parameters.AddWithValue("@quantidade", item.Quantidade);
                        cmdEstoque.Parameters.AddWithValue("@produto_id", item.ProdutoId);

                        int linhasAfetadas = cmdEstoque.ExecuteNonQuery();

                        if (linhasAfetadas == 0)
                        {
                            throw new Exception("Estoque insuficiente para o produto: " + item.Nome);
                        }
                    }
                }

                transacao.Commit();

                return vendaId;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
        }

        private void LimparVenda()
        {
            itensVenda.Clear();

            DescontoGeral.Text = "0,00";
            ValorRecebido.Text = "0,00";
            QuantidadeVenda.Text = "1";
            PesquisaProduto.Text = PlaceholderPesquisa;

            if (ComboClientes.Items.Count > 0)
            {
                ComboClientes.SelectedIndex = 0;
            }

            CarregarEstoque();
            AtualizarResumo();
        }

        private void AtualizarResumo()
        {
            subtotal = itensVenda.Sum(i => i.Subtotal);

            decimal valorDesconto = subtotal * (descontoGeral / 100);
            total = subtotal - valorDesconto;

            if (LabelSubtotal != null)
            {
                LabelSubtotal.Content = $"R$ {subtotal:N2}";
            }

            if (LabelTotal != null)
            {
                LabelTotal.Content = $"R$ {total:N2}";
            }

            if (TotalItens != null)
            {
                TotalItens.Content = itensVenda.Sum(i => i.Quantidade).ToString();
            }

            if (ValorTotalVenda != null)
            {
                ValorTotalVenda.Content = $"R$ {total:N2}";
            }

            if (LabelTroco != null &&
                ValorRecebido != null &&
                decimal.TryParse(ValorRecebido.Text, out decimal recebido))
            {
                decimal troco = recebido - total;

                if (troco < 0)
                {
                    troco = 0;
                }

                LabelTroco.Content = $"R$ {troco:N2}";
            }
        }

        private void BotaoEstoque_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Home());
        }

        private void BotaoVendas_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BotaoHistorico_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new TelaHistorico());
        }

        private void BotaoFinanceiro_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new TelaFinanceiro());
        }

        private void BotaoClientes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new TelaClientes());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new TelaEmpresa());
        }

        private void BotaoSair_Click(object sender, RoutedEventArgs e)
        {
            SessaoUsuario.Limpar();
            NavigationService?.Navigate(new TelaLogin());
        }

        private void PesquisaProduto_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PesquisaProduto.Text == PlaceholderPesquisa ||
                PesquisaProduto.Text == "Código do produto.")
            {
                PesquisaProduto.Text = "";
            }
        }

        private void PesquisaProduto_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PesquisaProduto.Text))
            {
                PesquisaProduto.Text = PlaceholderPesquisa;
                AtualizarFiltroEstoque();
            }
        }

        private void PesquisaProduto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AdicionarProduto();
            }
        }

        private void PesquisaProduto_TextChanged(object sender, TextChangedEventArgs e)
        {
            AtualizarFiltroEstoque();
        }

        private void BtnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            AdicionarProduto();
        }

        private void DGEstoque_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AdicionarProdutoSelecionado();
        }

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
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void BtnLimparVenda_Click(object sender, RoutedEventArgs e)
        {
            if (itensVenda.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Deseja realmente limpar todos os itens?",
                    "Confirmar",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    itensVenda.Clear();
                    DescontoGeral.Text = "0,00";
                    ValorRecebido.Text = "0,00";
                    AtualizarResumo();
                }
            }
        }

        private void BtnDinheiro_Click(object sender, RoutedEventArgs e)
        {
            formaPagamentoSelecionada = "Dinheiro";
            LabelPagamento.Content = "Pagamento: Dinheiro";
            ResetarBotoesPagamento();

            BtnDinheiro.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B82F6"));
        }

        private void BtnCartao_Click(object sender, RoutedEventArgs e)
        {
            formaPagamentoSelecionada = "Cartão";
            LabelPagamento.Content = "Pagamento: Cartão";
            ResetarBotoesPagamento();

            BtnCartao.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B82F6"));
        }

        private void BtnPix_Click(object sender, RoutedEventArgs e)
        {
            formaPagamentoSelecionada = "Pix";
            LabelPagamento.Content = "Pagamento: Pix";
            ResetarBotoesPagamento();

            BtnPix.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B82F6"));
        }

        private void BtnBoleto_Click(object sender, RoutedEventArgs e)
        {
            formaPagamentoSelecionada = "Boleto";
            LabelPagamento.Content = "Pagamento: Boleto";
            ResetarBotoesPagamento();

            BtnBoleto.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3B82F6"));
        }

        private void ResetarBotoesPagamento()
        {
            SolidColorBrush corPadrao =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F2937"));

            BtnDinheiro.Background = corPadrao;
            BtnCartao.Background = corPadrao;
            BtnPix.Background = corPadrao;
            BtnBoleto.Background = corPadrao;
        }

        private void DescontoGeral_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DescontoGeral == null)
            {
                return;
            }

            if (decimal.TryParse(DescontoGeral.Text, out decimal desconto))
            {
                descontoGeral = desconto;
            }
            else
            {
                descontoGeral = 0;
            }

            AtualizarResumo();
        }

        private void ValorRecebido_TextChanged(object sender, TextChangedEventArgs e)
        {
            AtualizarResumo();
        }

        private void BtnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (itensVenda.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um produto à venda.", "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (formaPagamentoSelecionada == "Dinheiro")
            {
                if (!decimal.TryParse(ValorRecebido.Text, out decimal recebido))
                {
                    MessageBox.Show("Informe o valor recebido.", "Aviso",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (recebido < total)
                {
                    MessageBox.Show("Valor recebido é menor que o total da venda.", "Aviso",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
            }

            string clienteTexto = clienteSelecionadoId > 0
                ? clienteSelecionadoNome
                : "Consumidor não informado";

            MessageBoxResult result = MessageBox.Show(
                $"Confirmar venda?\n\nCliente: {clienteTexto}\nTotal: R$ {total:N2}\nPagamento: {formaPagamentoSelecionada}",
                "Finalizar Venda",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                int vendaId = SalvarVendaEBaixarEstoque();

                MessageBox.Show(
                    $"Venda finalizada com sucesso!\n\nNúmero da venda: {vendaId}",
                    "Sucesso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                LimparVenda();

                NavigationService?.Navigate(new TelaComprovante(vendaId));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao finalizar venda: " + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ResetarBotoes()
        {
            SolidColorBrush corPadrao =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F2937"));

            BotaoEstoque.Background = corPadrao;
            BotaoVendas.Background = corPadrao;
            BotaoHistorico.Background = corPadrao;
            BotaoFinanceiro.Background = corPadrao;
            BotaoEmpresa.Background = corPadrao;
            BotaoClientes.Background = corPadrao;
        }
    }

    public class ClienteVenda
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Cpf { get; set; } = "";

        public override string ToString()
        {
            if (Id == 0)
            {
                return Nome;
            }

            if (string.IsNullOrWhiteSpace(Cpf))
            {
                return Nome;
            }

            return Nome + " - " + Cpf;
        }
    }
}