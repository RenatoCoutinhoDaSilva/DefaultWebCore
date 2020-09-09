using System.ComponentModel.DataAnnotations;

namespace Gestor.ViewModel.Login
{
    public class EsqueciSenhaVM
    {
        [Required(ErrorMessage = "Informação obrigatória!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Por favor, digite um e-mail válido!")]
        public string Email { get; set; }
    }
}
