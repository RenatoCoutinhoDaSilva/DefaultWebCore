using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Gestor.Filters
{
    public class SenhaValidaAttributeFilter : ValidationAttribute
    {
        private bool required;

        public SenhaValidaAttributeFilter(bool _required) {
            required = _required;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            if (value == null && !required) return ValidationResult.Success;
            if (value == null && required)
                return new ValidationResult("A senha precisa ter pelo menos uma letra minúscula, uma letra maiúscula, um número e ter entre 8 e 16 caracteres.");

            var password = value.ToString();
            if (Regex.Match(password, "^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%&/_*]+)(?=.*\\d)[a-zA-Z\\d!@#$%&/_*]{8,16}$").Success)
                return ValidationResult.Success;
            else
                return new ValidationResult("A senha precisa ter pelo menos uma letra minúscula, uma letra maiúscula, um número e ter entre 8 e 16 caracteres.");
        }
    }
}
