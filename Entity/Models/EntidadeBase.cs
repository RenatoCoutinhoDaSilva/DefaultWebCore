using System;

namespace Entity.Models
{
    public class EntidadeBase
    {
        public long Id { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Excluido { get; set; }
     }
}
