using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ProjetoIntegrador
{
    public partial class TelaClientes : Page
    {
        private ObservableCollection<ClienteCadastro> clientes = new ObservableCollection<ClienteCadastro>();
        private int clienteSelecionadoId = 0;

        public TelaClientes()
        {
            InitializeComponent();

            Clientes1.ItemsSource = clientes;

            ResetarBotoes();

            BotaoClientes.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7C3AED"));

            CarregarClientes();
        }

        private void AbrirConexao()
        {
            if (ConectBd.Conexao.State != ConnectionState.Open)
            {
                ConectBd.Conexao.Open();
            }
        }

        private void ResetarBotoes()
        {
            SolidColorBrush corPadrao =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1F2937"));

            BotaoEstoque.Background = corPadrao;
            BotaoVendas.Background = corPadrao;
            BotaoClientes.Background = corPadrao;
            BotaoHistorico.Background = corPadrao;
            BotaoFinanceiro.Background = corPadrao;
            BotaoEmpresa.Background = corPadrao;
        }

        private void CarregarClientes()
        {
            try
            {
                clientes.Clear();
                AbrirConexao();

                string busca = TxtPesquisa.Text.Trim();

                string sql = @"
                    SELECT
                        id,
                        nome,
                        cpf,
                        telefone,
                        email,
                        endereco,
                        numero,
                        bairro,
                        cidade,
                        estado,
                        cep,
                        observacao,
                        ativo,
                        data_cadastro
                    FROM clientes
                    WHERE ativo = 1";

                if (!string.IsNullOrWhiteSpace(busca))
                {
                    sql += @"
                        AND (
                            nome LIKE @busca
                            OR cpf LIKE @busca
                            OR telefone LIKE @busca
                            OR email LIKE @busca
                        )";
                }

                sql += " ORDER BY nome ASC";

                using (MySqlCommand cmd = new MySqlCommand(sql, ConectBd.Conexao))
                {
                    if (!string.IsNullOrWhiteSpace(busca))
                    {
                        cmd.Parameters.AddWithValue("@busca", "%" + busca + "%");
                    }

                    using (MySqlDataReader leitor = cmd.ExecuteReader())
                    {
                        while (leitor.Read())
                        {
                            clientes.Add(new ClienteCadastro
                            {
                                Id = Convert.ToInt32(leitor["id"]),
                                Nome = leitor["nome"].ToString(),
                                Cpf = leitor["cpf"] == DBNull.Value ? "" : leitor["cpf"].ToString(),
                                Telefone = leitor["telefone"] == DBNull.Value ? "" : leitor["telefone"].ToString(),
                                Email = leitor["email"] == DBNull.Value ? "" : leitor["email"].ToString(),
                                Endereco = leitor["endereco"] == DBNull.Value ? "" : leitor["endereco"].ToString(),
                                Numero = leitor["numero"] == DBNull.Value ? "" : leitor["numero"].ToString(),
                                Bairro = leitor["bairro"] == DBNull.Value ? "" : leitor["bairro"].ToString(),
                                Cidade = leitor["cidade"] == DBNull.Value ? "" : leitor["cidade"].ToString(),
                                Estado = leitor["estado"] == DBNull.Value ? "" : leitor["estado"].ToString(),
                                Cep = leitor["cep"] == DBNull.Value ? "" : leitor["cep"].ToString(),
                                Observacao = leitor["observacao"] == DBNull.Value ? "" : leitor["observacao"].ToString(),
                                Ativo = Convert.ToInt32(leitor["ativo"]),
                                DataCadastro = leitor["data_cadastro"] == DBNull.Value
                                    ? DateTime.Now
                                    : Convert.ToDateTime(leitor["data_cadastro"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar clientes: " + ex.Message);
            }
        }

        private void BotaoSalvar_Click(object sender, RoutedEventArgs e)
        {
            string nome = TxtNome.Text.Trim();

            if (string.IsNullOrWhiteSpace(nome))
            {
                MessageBox.Show("Digite o nome do cliente.");
                TxtNome.Focus();
                return;
            }

            try
            {
                AbrirConexao();

                if (clienteSelecionadoId == 0)
                {
                    string sql = @"
                        INSERT INTO clientes
                        (
                            nome,
                            cpf,
                            telefone,
                            email,
                            endereco,
                            numero,
                            bairro,
                            cidade,
                            estado,
                            cep,
                            observacao,
                            ativo
                        )
                        VALUES
                        (
                            @nome,
                            @cpf,
                            @telefone,
                            @email,
                            @endereco,
                            @numero,
                            @bairro,
                            @cidade,
                            @estado,
                            @cep,
                            @observacao,
                            1
                        )";

                    using (MySqlCommand cmd = new MySqlCommand(sql, ConectBd.Conexao))
                    {
                        PreencherParametros(cmd);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Cliente cadastrado com sucesso.");
                }
                else
                {
                    string sql = @"
                        UPDATE clientes
                        SET
                            nome = @nome,
                            cpf = @cpf,
                            telefone = @telefone,
                            email = @email,
                            endereco = @endereco,
                            numero = @numero,
                            bairro = @bairro,
                            cidade = @cidade,
                            estado = @estado,
                            cep = @cep,
                            observacao = @observacao
                        WHERE id = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, ConectBd.Conexao))
                    {
                        PreencherParametros(cmd);
                        cmd.Parameters.AddWithValue("@id", clienteSelecionadoId);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Cliente atualizado com sucesso.");
                }

                LimparCampos();
                CarregarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar cliente: " + ex.Message);
            }
        }

        private void PreencherParametros(MySqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@nome", TxtNome.Text.Trim());
            cmd.Parameters.AddWithValue("@cpf", TxtCpf.Text.Trim());
            cmd.Parameters.AddWithValue("@telefone", TxtTelefone.Text.Trim());
            cmd.Parameters.AddWithValue("@email", TxtEmail.Text.Trim());
            cmd.Parameters.AddWithValue("@endereco", TxtEndereco.Text.Trim());
            cmd.Parameters.AddWithValue("@numero", TxtNumero.Text.Trim());
            cmd.Parameters.AddWithValue("@bairro", TxtBairro.Text.Trim());
            cmd.Parameters.AddWithValue("@cidade", TxtCidade.Text.Trim());
            cmd.Parameters.AddWithValue("@estado", TxtEstado.Text.Trim().ToUpper());
            cmd.Parameters.AddWithValue("@cep", TxtCep.Text.Trim());
            cmd.Parameters.AddWithValue("@observacao", TxtObservacao.Text.Trim());
        }

        private void Clientes1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClienteCadastro cliente = Clientes1.SelectedItem as ClienteCadastro;

            if (cliente == null)
            {
                return;
            }

            clienteSelecionadoId = cliente.Id;

            TxtNome.Text = cliente.Nome;
            TxtCpf.Text = cliente.Cpf;
            TxtTelefone.Text = cliente.Telefone;
            TxtEmail.Text = cliente.Email;
            TxtEndereco.Text = cliente.Endereco;
            TxtNumero.Text = cliente.Numero;
            TxtBairro.Text = cliente.Bairro;
            TxtCidade.Text = cliente.Cidade;
            TxtEstado.Text = cliente.Estado;
            TxtCep.Text = cliente.Cep;
            TxtObservacao.Text = cliente.Observacao;

            LabelIdCliente.Content = "Cliente selecionado: ID " + cliente.Id;
        }

        private void BotaoNovo_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void LimparCampos()
        {
            clienteSelecionadoId = 0;

            TxtNome.Clear();
            TxtCpf.Clear();
            TxtTelefone.Clear();
            TxtEmail.Clear();
            TxtEndereco.Clear();
            TxtNumero.Clear();
            TxtBairro.Clear();
            TxtCidade.Clear();
            TxtEstado.Clear();
            TxtCep.Clear();
            TxtObservacao.Clear();

            Clientes1.SelectedItem = null;
            LabelIdCliente.Content = "Nenhum cliente selecionado";

            TxtNome.Focus();
        }

        private void BotaoExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (clienteSelecionadoId == 0)
            {
                MessageBox.Show("Selecione um cliente para excluir.");
                return;
            }

            MessageBoxResult resposta = MessageBox.Show(
                "Deseja realmente excluir este cliente?",
                "Confirmar exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resposta != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                AbrirConexao();

                string sql = @"
                    UPDATE clientes
                    SET ativo = 0
                    WHERE id = @id";

                using (MySqlCommand cmd = new MySqlCommand(sql, ConectBd.Conexao))
                {
                    cmd.Parameters.AddWithValue("@id", clienteSelecionadoId);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Cliente excluído com sucesso.");

                LimparCampos();
                CarregarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir cliente: " + ex.Message);
            }
        }

        private void BotaoPesquisar_Click(object sender, RoutedEventArgs e)
        {
            CarregarClientes();
        }

        private void TxtPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CarregarClientes();
            }
        }

        private void BotaoClientes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IrEstoque_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
        }

        private void IrVendas_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaVendas());
        }

        private void IrHistorico_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaHistorico());
        }

        private void IrFinanceiro_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaFinanceiro());
        }

        private void IrEmpresa_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaEmpresa());
        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            SessaoUsuario.Limpar();
            NavigationService.Navigate(new TelaLogin());
        }
    }

    public class ClienteCadastro
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Cpf { get; set; } = "";
        public string Telefone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Endereco { get; set; } = "";
        public string Numero { get; set; } = "";
        public string Bairro { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Estado { get; set; } = "";
        public string Cep { get; set; } = "";
        public string Observacao { get; set; } = "";
        public int Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}