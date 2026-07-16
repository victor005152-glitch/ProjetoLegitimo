using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjetoIntegrador
{
    public partial class TelaEmpresa : Page
    {
        private int empresaId = 0;

        public TelaEmpresa()
        {
            InitializeComponent();

            ResetarBotoes();

            BotaoEmpresa.Background =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7C3AED"));

            CarregarDadosEmpresa();
        }

        private void CarregarDadosEmpresa()
        {
            try
            {
                if (ConectBd.Conexao.State != ConnectionState.Open)
                {
                    ConectBd.Conexao.Open();
                }

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
                        empresaId = Convert.ToInt32(leitor["id"]);

                        TxtNomeFantasia.Text = leitor["nome_fantasia"].ToString();
                        TxtRazaoSocial.Text = leitor["razao_social"].ToString();
                        TxtCnpj.Text = leitor["cnpj"].ToString();
                        TxtTelefone.Text = leitor["telefone"].ToString();
                        TxtEmail.Text = leitor["email"].ToString();
                        TxtEndereco.Text = leitor["endereco"].ToString();
                        TxtNumero.Text = leitor["numero"].ToString();
                        TxtBairro.Text = leitor["bairro"].ToString();
                        TxtCidade.Text = leitor["cidade"].ToString();
                        TxtEstado.Text = leitor["estado"].ToString();
                        TxtCep.Text = leitor["cep"].ToString();
                        TxtMensagemRodape.Text = leitor["mensagem_rodape"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados da empresa: " + ex.Message);
            }
        }

        private void BotaoSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCamposEmpresa())
            {
                return;
            }

            try
            {
                if (ConectBd.Conexao.State != ConnectionState.Open)
                {
                    ConectBd.Conexao.Open();
                }

                if (empresaId > 0)
                {
                    AtualizarEmpresa();
                }
                else
                {
                    InserirEmpresa();
                }

                MessageBox.Show(
                    "Dados da empresa salvos com sucesso!",
                    "Sucesso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao salvar dados da empresa: " + ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
        private bool ValidarCamposEmpresa()
        {
            if (string.IsNullOrWhiteSpace(TxtNomeFantasia.Text))
            {
                MessageBox.Show("Informe o nome fantasia da empresa.");
                TxtNomeFantasia.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtCnpj.Text))
            {
                MessageBox.Show("Informe o CNPJ da empresa.");
                TxtCnpj.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtTelefone.Text))
            {
                MessageBox.Show("Informe o telefone da empresa.");
                TxtTelefone.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtEndereco.Text))
            {
                MessageBox.Show("Informe o endereço da empresa.");
                TxtEndereco.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtCidade.Text))
            {
                MessageBox.Show("Informe a cidade da empresa.");
                TxtCidade.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtEstado.Text))
            {
                MessageBox.Show("Informe o estado/UF da empresa.");
                TxtEstado.Focus();
                return false;
            }

            if (TxtEstado.Text.Trim().Length != 2)
            {
                MessageBox.Show("A UF deve ter 2 letras. Exemplo: RS, SP, SC.");
                TxtEstado.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(TxtEmail.Text) && !TxtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Informe um e-mail válido ou deixe o campo vazio.");
                TxtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtMensagemRodape.Text))
            {
                TxtMensagemRodape.Text = "Obrigado pela preferência!";
            }

            return true;
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


        private void AtualizarEmpresa()
        {
            string sql = @"
                UPDATE empresa_config
                SET nome_fantasia = @nome_fantasia,
                    razao_social = @razao_social,
                    cnpj = @cnpj,
                    telefone = @telefone,
                    email = @email,
                    endereco = @endereco,
                    numero = @numero,
                    bairro = @bairro,
                    cidade = @cidade,
                    estado = @estado,
                    cep = @cep,
                    mensagem_rodape = @mensagem_rodape
                WHERE id = @id";

            using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
            {
                comando.Parameters.AddWithValue("@id", empresaId);
                AdicionarParametrosEmpresa(comando);
                comando.ExecuteNonQuery();
            }
        }

        private void InserirEmpresa()
        {
            string sql = @"
                INSERT INTO empresa_config
                (nome_fantasia, razao_social, cnpj, telefone, email, endereco, numero, bairro, cidade, estado, cep, mensagem_rodape)
                VALUES
                (@nome_fantasia, @razao_social, @cnpj, @telefone, @email, @endereco, @numero, @bairro, @cidade, @estado, @cep, @mensagem_rodape)";

            using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
            {
                AdicionarParametrosEmpresa(comando);
                comando.ExecuteNonQuery();

                empresaId = Convert.ToInt32(comando.LastInsertedId);
            }
        }

        private void AdicionarParametrosEmpresa(MySqlCommand comando)
        {
            comando.Parameters.AddWithValue("@nome_fantasia", TxtNomeFantasia.Text.Trim());
            comando.Parameters.AddWithValue("@razao_social", TxtRazaoSocial.Text.Trim());
            comando.Parameters.AddWithValue("@cnpj", TxtCnpj.Text.Trim());
            comando.Parameters.AddWithValue("@telefone", TxtTelefone.Text.Trim());
            comando.Parameters.AddWithValue("@email", TxtEmail.Text.Trim());
            comando.Parameters.AddWithValue("@endereco", TxtEndereco.Text.Trim());
            comando.Parameters.AddWithValue("@numero", TxtNumero.Text.Trim());
            comando.Parameters.AddWithValue("@bairro", TxtBairro.Text.Trim());
            comando.Parameters.AddWithValue("@cidade", TxtCidade.Text.Trim());
            comando.Parameters.AddWithValue("@estado", TxtEstado.Text.Trim().ToUpper());
            comando.Parameters.AddWithValue("@cep", TxtCep.Text.Trim());
            comando.Parameters.AddWithValue("@mensagem_rodape", TxtMensagemRodape.Text.Trim());
        }

        private void BotaoVoltar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Home());
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

        private void BotaoSair_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaLogin());
        }
        private void BotaoClientes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaClientes());
        }

        private void BotaoEmpresa_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaEmpresa());
        }
    }

}