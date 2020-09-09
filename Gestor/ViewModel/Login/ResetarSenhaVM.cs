using Gestor.Filters;
using System.ComponentModel.DataAnnotations;

namespace Gestor.ViewModel.Login
{
    public class ResetarSenhaVM
    {
        public string token { get; set; }

        [Required(ErrorMessage = "Informação obrigatória.")]
        [SenhaValidaAttributeFilter(true)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Informação obrigatória.")]
        [Compare("Senha", ErrorMessage = "Senha e confirmação não correspondem.")]
        public string ConfirmaSenha { get; set; }
    }
}
