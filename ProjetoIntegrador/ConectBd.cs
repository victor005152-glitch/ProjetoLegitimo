using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoIntegrador
{
    internal class ConectBd
    {
        public string StrConex;
        MySqlConnection conextion = new MySqlConnection();

        public ConectBd()
        {
            StrConex = "Server=localhost;Database=loja_de_roupas;Uid=root;Password=123456789;";
        }

        public void Conectar()
        {
            conextion.ConnectionString = StrConex;
            conextion.Open();
        }
        public void Insert(string sql)
        {
            MySqlCommand cmd = new MySqlCommand(sql, conextion);
            MySqlDataReader reader = cmd.ExecuteReader();
        }
    }
}
