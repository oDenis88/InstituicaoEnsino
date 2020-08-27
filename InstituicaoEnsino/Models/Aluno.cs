using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstituicaoEnsino.Models
{
    public partial class Aluno
    {
        public int IdAluno { get; set; }
        public int IdProfessor { get; set; }

        [Required]
        public string Nome { get; set; }

        [DisplayFormat(DataFormatString = "{0:R$ 0.00}")]
        public decimal Mensalidade { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataVencimento { get; set; }

        public virtual Professor IdProfessorNavigation { get; set; }
    }
}
