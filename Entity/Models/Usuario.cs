using Entity.Enums;
using Microsoft.AspNetCore.Identity;
using System;

namespace Entity.Models
{
    public class Usuario : IdentityUser<long>
    {
        public static string CaminhoArquivo { get { return "/Conteudo/Usuarios"; } }

        public string Nome { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
        public string Documento { get; set; }
        public string Telefone { get; set; }
        public string Foto { get; set; }
        public PerfilUsuario Perfil { get; set; }

    }
}
