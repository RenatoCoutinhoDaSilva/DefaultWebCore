using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Entity.Models
{
    public class TokenSenha : EntidadeBase
    {
        public TokenSenha() { }

        /// <summary>
        /// Cria o token sem fazer referência ao usuário, ele só trará os dados do usuário ao ser buscado no banco.
        /// </summary>
        /// <param name="usuarioEmail">Email utilizado no login do usuário.</param>
        /// <param name="validadeToken">Validade do token em dias, será contado a partir da data de criação.</param>
        public TokenSenha(string usuarioEmail, int validadeToken) {
            DataCadastro = DateTime.Now;

            UsuarioEmail = usuarioEmail.ToLower();
            Token = Guid.NewGuid().ToString().ToLower();
            Ativo = true;
            Validade = DataCadastro.AddDays(validadeToken);
        }

        public string UsuarioEmail { get; set; }
        public string Token { get; set; }
        public bool Ativo { get; set; }
        public DateTime Validade { get; set; }

        public bool EstaValido() {
            return Ativo && Validade > DateTime.Now;
        }
    }
}
