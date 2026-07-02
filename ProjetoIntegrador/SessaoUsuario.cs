using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoIntegrador
{
    public static class SessaoUsuario
    {
        public static int UsuarioId { get; set; }
        public static string Nome { get; set; } = "";
        public static string Email { get; set; } = "";
        public static string TipoUsuario { get; set; } = "";

        public static bool EstaLogado
        {
            get { return UsuarioId > 0; }
        }

        public static void Limpar()
        {
            UsuarioId = 0;
            Nome = "";
            Email = "";
            TipoUsuario = "";
        }
    }
}
