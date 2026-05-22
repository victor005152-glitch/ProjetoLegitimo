using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;


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
    }
}
