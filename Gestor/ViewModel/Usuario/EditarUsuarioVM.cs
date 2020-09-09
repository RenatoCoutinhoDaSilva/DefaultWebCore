using Gestor.Filters;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Gestor.ViewModel.Usuario
{
    public class EditarUsuarioVM
    {
        public EditarUsuarioVM() { }
        public EditarUsuarioVM(Entity.Models.Usuario model) {
            Id = model.Id;
            Email = model.Email;
            Nome = model.Nome;
            Documento = model.Documento;
            Telefone = model.Telefone;
            Ativo = model.Ativo;

            if (!string.IsNullOrEmpty(model.Foto))
                FotoAtual = $"{Entity.Models.Usuario.CaminhoArquivo}/{model.Foto}";
        }

        public long Id { get; set; }
        /// <summary>
        /// Campo apenas informativo, não é possível mudar o e-mail do usuário para não ferir a integridade do DB, pois ele é utilizado como username.
        /// </summary>
        public string Email { get; set; }
        public bool Ativo { get; set; }

        [Required(ErrorMessage = "Informação obrigatória!")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informação obrigatória!")]
        [ValidaCPFAttributeFilter(ErrorMessage = "Digite um CPF válido.")]
        public string Documento { get; set; }
        public string Telefone { get; set; }
        public string FotoAtual { get; set; }
        public IFormFile Foto { get; set; }

        [SenhaValidaAttributeFilter(false)]
        public string Senha { get; set; }

        [Compare("Senha", ErrorMessage = "Confirmação não é igual a senha.")]
        public string ConfirmaSenha { get; set; }
    }
}
