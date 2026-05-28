using MySql.Data.MySqlClient;
using ProjetoIntegrador;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ProjetoIntegrador
{
    /// <summary>
    /// Interação lógica para TelaLogin.xam
    /// </summary>
    public partial class TelaLogin : Page
    {
        public TelaLogin()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaCadastro());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT Nome, Senha FROM Usuario WHERE Nome = @nome AND Senha= @senha";

            MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao);
            comando.Parameters.AddWithValue("@nome", User1.Text);
            comando.Parameters.AddWithValue("@senha", Senha1.Password);

            using (MySqlDataReader leitor = comando.ExecuteReader())
            {
                if (leitor.Read())
                {
                    string nome = leitor["Nome"].ToString();
                    leitor.Close();
                    NavigationService.Navigate(new Home());
                }
                else
                {
                    MessageBox.Show("Usuário ou senha inválidos.");
                }
                leitor.Close();
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
