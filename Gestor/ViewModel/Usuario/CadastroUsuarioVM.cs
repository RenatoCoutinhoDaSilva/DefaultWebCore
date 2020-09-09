using Gestor.Filters;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Gestor.ViewModel.Usuario
{
    public class CadastroUsuarioVM
    {
        [Required(ErrorMessage = "Informação obrigatória!")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informação obrigatória!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Por favor, digite um e-mail válido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informação obrigatória!")]
        [ValidaCPFAttributeFilter(ErrorMessage = "Digite um CPF válido.")]
        public string Documento { get; set; }
        public string Telefone { get; set; }
        public IFormFile Foto { get; set; }
        public bool Ativo { get; set; }

        [SenhaValidaAttributeFilter(true)]
        public string Senha { get; set; }

        [Compare("Senha", ErrorMessage = "Confirmação não é igual a senha.")]
        public string ConfirmaSenha { get; set; }

        public void LimpaSenha() {
            Senha = "";
            ConfirmaSenha = "";
        }
    }
}
