using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjetoIntegrador
{
    public partial class TelaLogin : Page
    {
        public TelaLogin()
        {
            InitializeComponent();

            // Limpa sessão ao voltar para a tela de login
            SessaoUsuario.Limpar();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaCadastro());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string nomeUsuario = User1.Text.Trim();
            string senha = Senha1.Password.Trim();

            if (string.IsNullOrWhiteSpace(nomeUsuario))
            {
                MessageBox.Show("Digite o usuário.");
                User1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(senha))
            {
                MessageBox.Show("Digite a senha.");
                Senha1.Focus();
                return;
            }

            bool loginValido = false;

            int usuarioId = 0;
            string nome = "";
            string email = "";
            string tipoUsuario = "Operador";

            try
            {
                if (ConectBd.Conexao.State != ConnectionState.Open)
                {
                    ConectBd.Conexao.Open();
                }

                string sql = @"
            SELECT 
                Id,
                Nome,
                Email,
                tipo_usuario
            FROM usuario
            WHERE Nome = @nome
              AND Senha = @senha
              AND ativo = 1
            LIMIT 1";

                using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
                {
                    comando.Parameters.AddWithValue("@nome", nomeUsuario);
                    comando.Parameters.AddWithValue("@senha", senha);

                    using (MySqlDataReader leitor = comando.ExecuteReader())
                    {
                        if (leitor.Read())
                        {
                            loginValido = true;

                            usuarioId = Convert.ToInt32(leitor["Id"]);
                            nome = leitor["Nome"].ToString();
                            email = leitor["Email"] == DBNull.Value ? "" : leitor["Email"].ToString();
                            tipoUsuario = leitor["tipo_usuario"] == DBNull.Value ? "Operador" : leitor["tipo_usuario"].ToString();
                        }
                    }
                }

                if (loginValido)
                {
                    SessaoUsuario.UsuarioId = usuarioId;
                    SessaoUsuario.Nome = nome;
                    SessaoUsuario.Email = email;
                    SessaoUsuario.TipoUsuario = tipoUsuario;

                    NavigationService.Navigate(new Home());
                }
                else
                {
                    MessageBox.Show("Usuário ou senha inválidos.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao fazer login: " + ex.Message);
            }
        }

        private void Senha1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_1(sender, e);
            }
        }

        private void Recuperar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaRecuperacao());
        }
    }
}