using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjetoIntegrador
{
    /// <summary>
    /// Interação lógica para TelaCadastro.xam
    /// </summary>
    public partial class TelaCadastro : Page
    {
        public TelaCadastro()
        {
            InitializeComponent();
        }

        private void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            string nome = txbUser.Text;
            string senha = txbSenha.Password;

            try
            {
                string sql = @"INSERT INTO Usuario (Nome, Senha) VALUES (@nome, @senha)";

                using (MySqlCommand cmd = new MySqlCommand(sql, ConectBd.Conexao))
                {
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@senha", senha);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Usuário cadastrado com sucesso!");

                txbUser.Clear();
                txbSenha.Clear();

                NavigationService.Navigate(new TelaLogin());
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


        }
    }
}
