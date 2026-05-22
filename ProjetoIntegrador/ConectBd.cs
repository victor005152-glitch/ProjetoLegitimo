using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ProjetoIntegrador
{
    class ConectBd
    {
        public static MySqlConnection? Conexao { get; private set; }

        public static void AbrirConexao(string banco)
        {
            try
            {
                if (Conexao == null)
                {
                    Conexao = new MySqlConnection(banco);
                    Conexao.Open();
                }
            }
            catch (Exception ex)
            {
                Conexao = null;
                MessageBox.Show(ex.ToString());
            }
        }

        public static void FecharConexao()
        {
            if (Conexao != null && Conexao.State == System.Data.ConnectionState.Open)
                Conexao.Close();
        }
    }
}
