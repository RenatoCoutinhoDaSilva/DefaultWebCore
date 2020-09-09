using System;
using System.ComponentModel.DataAnnotations;

namespace Gestor.Filters
{
    /// <summary>
    /// Se o valor da propriedade for preenchido, esse se torna obrigatório.
    /// </summary>
    public class ObrigatorioSeAttributeFilter : ValidationAttribute
    {
        private string nomeDependencia { get; set; }

        /// <param name="_nomeDependencia">É o nome do parâmetro ao qual esse depende</param>
        public ObrigatorioSeAttributeFilter(string _nomeDependencia) {
            nomeDependencia = _nomeDependencia;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context) {
            object instancia = context.ObjectInstance;
            Type type = instancia.GetType();
            object valorDependente = type.GetProperty(nomeDependencia).GetValue(instancia, null);
            if (valorDependente != null && value == null) {
                return new ValidationResult("Informação obrigatória.");
            }
            return ValidationResult.Success;
        }
    }
}
