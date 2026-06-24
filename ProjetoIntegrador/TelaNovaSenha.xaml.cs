using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjetoIntegrador
{

    public partial class TelaNovaSenha : Page
    {

        private string emailRecuperacao = "";

        public TelaNovaSenha(string email)
        {
            InitializeComponent();
            emailRecuperacao = email;
        }

        private void btnAlterarSenha_Click(object sender, RoutedEventArgs e)
        {
            AlterarSenha();
        }

        private void ConfirmarSenhaDigitada_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AlterarSenha();
            }
        }

        private void AlterarSenha()
        {
            string novaSenha = NovaSenhaDigitada.Password;
            string confirmarSenha = ConfirmarSenhaDigitada.Password;

            if (string.IsNullOrWhiteSpace(novaSenha) || string.IsNullOrWhiteSpace(confirmarSenha))
            {
                MessageBox.Show("Digite e confirme a nova senha.");
                return;
            }

            if (novaSenha.Length < 4)
            {
                MessageBox.Show("A senha precisa ter pelo menos 4 caracteres.");
                return;
            }

            if (novaSenha != confirmarSenha)
            {
                MessageBox.Show("As senhas não conferem.");
                return;
            }

            try
            {
                if (ConectBd.Conexao.State != ConnectionState.Open)
                {
                    ConectBd.Conexao.Open();
                }

                // BUSCA A SENHA ANTIGA DO USUÁRIO
                string sqlBuscarSenha = @"
            SELECT Senha
            FROM Usuario
            WHERE Email = @Email";

                string senhaAntiga = "";

                using (MySqlCommand comandoBuscar = new MySqlCommand(sqlBuscarSenha, ConectBd.Conexao))
                {
                    comandoBuscar.Parameters.AddWithValue("@Email", emailRecuperacao);

                    object resultado = comandoBuscar.ExecuteScalar();

                    if (resultado == null)
                    {
                        MessageBox.Show("Usuário não encontrado.");
                        return;
                    }

                    senhaAntiga = resultado.ToString();
                }

                // VERIFICA SE A NOVA SENHA É IGUAL À ANTIGA
                if (novaSenha == senhaAntiga)
                {
                    MessageBox.Show("A nova senha não pode ser igual à senha antiga.");
                    return;
                }

                // ALTERA A SENHA
                string sqlAlterar = @"
            UPDATE Usuario
            SET Senha = @Senha,
                codigo_recuperacao = NULL,
                codigo_expira = NULL
            WHERE Email = @Email";

                using (MySqlCommand comando = new MySqlCommand(sqlAlterar, ConectBd.Conexao))
                {
                    comando.Parameters.AddWithValue("@Senha", novaSenha);
                    comando.Parameters.AddWithValue("@Email", emailRecuperacao);

                    int linhasAfetadas = comando.ExecuteNonQuery();

                    if (linhasAfetadas > 0)
                    {
                        MessageBox.Show("Senha alterada com sucesso.");
                        NavigationService.Navigate(new TelaLogin());
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível alterar a senha.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao alterar senha: " + ex.Message);
            }
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaLogin());
        }
    }
}