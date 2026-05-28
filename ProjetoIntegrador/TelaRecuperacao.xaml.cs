using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
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
    /// Interação lógica para TelaRecuperacao.xam
    /// </summary>
    public partial class TelaRecuperacao : Page
    {
        public TelaRecuperacao()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TelaLogin());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            string sql = "SELECT Nome, Senha FROM Usuario WHERE Email = @Email";
            MySqlCommand comando = new MySqlCommand(sql, ConectBd.Conexao);
            comando.Parameters.AddWithValue("@Email", EmailDigitado.Text);




            using (MySqlDataReader leitor = comando.ExecuteReader())
            {

                if (leitor.Read())
                {
                    string nome = leitor["Nome"].ToString();
                    string senha = leitor["Senha"].ToString();
                    MessageBox.Show($"Usuário encontrado: {nome}\nSenha: {senha}");
                    NavigationService.Navigate(new TelaLogin());
                }
                else
                {
                    MessageBox.Show("Email não encontrado.");
                }
                leitor.Close();

             


            }

           
        }
        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_1(sender, e);
            }
        }

    }
}
