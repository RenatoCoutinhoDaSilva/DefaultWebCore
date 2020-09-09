using System.ComponentModel.DataAnnotations;

namespace Gestor.ViewModel.Login
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Informação obrigatória!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Por favor, digite um e-mail válido!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informação obrigatória!")]
        public string Senha { get; set; }

        public bool LembrarSe { get; set; }
    }
}
