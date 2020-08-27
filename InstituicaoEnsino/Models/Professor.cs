using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InstituicaoEnsino.Models
{
    public partial class Professor
    {
        public Professor()
        {
            Aluno = new HashSet<Aluno>();
        }

        public int IdProfessor { get; set; }

        [Required]
        [StringLength(254, ErrorMessage = "Nome não pode exceder 254 caracteres")]
        [RegularExpression(@"^[a-zA-Z\u00c0-\u017e'\-]+(\s[a-zA-Z\u00c0-\u017e'\-]+)+$", ErrorMessage = "Formato inválido")]
        public string Nome { get; set; }
        public DateTime? DataUltimaImportacao { get; set; }

        public virtual ICollection<Aluno> Aluno { get; set; }
    }
}
