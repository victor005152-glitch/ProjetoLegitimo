using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjetoIntegrador
{
    /// <summary>
    /// Interação lógica para TelaRecuperacao.xaml
    /// </summary>
    public partial class TelaRecuperacao : Page
    {
        private string emailRecuperacao = "";

        // CONFIGURAÇÃO DO E-MAIL QUE VAI ENVIAR OS CÓDIGOS
        private const string EMAIL_SISTEMA = "wrvrecuperacao@gmail.com";
        private const string SENHA_APP = "Nba2k23.";

        public TelaRecuperacao()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaLogin());
        }

        private string GerarCodigo()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string email = EmailDigitado.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Digite o e-mail cadastrado.");
                return;
            }

            string codigo = GerarCodigo();

            try
            {
                if (ConectBd.Conexao.State != ConnectionState.Open)
                {
                    ConectBd.Conexao.Open();
                }

                string sqlVerificar = "SELECT COUNT(*) FROM Usuario WHERE Email = @Email";

                using (MySqlCommand comando = new MySqlCommand(sqlVerificar, ConectBd.Conexao))
                {
                    comando.Parameters.AddWithValue("@Email", email);

                    int existe = Convert.ToInt32(comando.ExecuteScalar());

                    if (existe == 0)
                    {
                        MessageBox.Show("Email não encontrado.");
                        return;
                    }
                }

                string sqlSalvar = @"
                    UPDATE Usuario
                    SET codigo_recuperacao = @Codigo,
                        codigo_expira = DATE_ADD(NOW(), INTERVAL 10 MINUTE)
                    WHERE Email = @Email";

                using (MySqlCommand comando = new MySqlCommand(sqlSalvar, ConectBd.Conexao))
                {
                    comando.Parameters.AddWithValue("@Codigo", codigo);
                    comando.Parameters.AddWithValue("@Email", email);
                    comando.ExecuteNonQuery();
                }

                await EnviarEmailCodigo(email, codigo);

                emailRecuperacao = email;

                MessageBox.Show("Código enviado para o e-mail cadastrado.");

                painelCodigo.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao enviar código: " + ex.Message);
            }
        }

        private void btnValidarCodigo_Click(object sender, RoutedEventArgs e)
        {
            string codigoDigitado = CodigoDigitado.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigoDigitado))
            {
                MessageBox.Show("Digite o código recebido.");
                return;
            }

            try
            {
                if (ConectBd.Conexao.State != ConnectionState.Open)
                {
                    ConectBd.Conexao.Open();
                }

                string sql = @"
                    SELECT COUNT(*)
                    FROM Usuario
                    WHERE Email = @Email
                      AND codigo_recuperacao = @Codigo
                      AND codigo_expira > NOW()";

                using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
                {
                    comando.Parameters.AddWithValue("@Email", emailRecuperacao);
                    comando.Parameters.AddWithValue("@Codigo", codigoDigitado);

                    int valido = Convert.ToInt32(comando.ExecuteScalar());

                    if (valido > 0)
                    {
                        MessageBox.Show("Código confirmado. Agora digite a nova senha.");
                        painelNovaSenha.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show("Código inválido ou expirado.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao validar código: " + ex.Message);
            }
        }

        private void btnAlterarSenha_Click(object sender, RoutedEventArgs e)
        {
            string novaSenha = NovaSenhaDigitada.Password;
            string confirmarSenha = ConfirmarSenhaDigitada.Password;

            if (string.IsNullOrWhiteSpace(novaSenha) || string.IsNullOrWhiteSpace(confirmarSenha))
            {
                MessageBox.Show("Digite e confirme a nova senha.");
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

                string sql = @"
                    UPDATE Usuario
                    SET Senha = @Senha,
                        codigo_recuperacao = NULL,
                        codigo_expira = NULL
                    WHERE Email = @Email";

                using (MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao))
                {
                    comando.Parameters.AddWithValue("@Senha", novaSenha);
                    comando.Parameters.AddWithValue("@Email", emailRecuperacao);
                    comando.ExecuteNonQuery();
                }

                MessageBox.Show("Senha alterada com sucesso.");
                NavigationService.Navigate(new TelaLogin());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao alterar senha: " + ex.Message);
            }
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_1(sender, e);
            }
        }

        private async Task EnviarEmailCodigo(string emailDestino, string codigo)
        {
            MailMessage mensagem = new MailMessage();

            mensagem.From = new MailAddress(EMAIL_SISTEMA);
            mensagem.To.Add(emailDestino);
            mensagem.Subject = "Código de recuperação de senha";
            mensagem.Body =
                "Olá!\n\n" +
                "Seu código de recuperação de senha é: " + codigo + "\n\n" +
                "Esse código expira em 10 minutos.\n\n" +
                "Se você não solicitou essa recuperação, ignore este e-mail.";

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(EMAIL_SISTEMA, SENHA_APP);

                await smtp.SendMailAsync(mensagem);
            }
        }
    }
}