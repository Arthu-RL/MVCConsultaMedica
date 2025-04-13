using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrabalhoMVC.Models
{
    public class Paciente : Pessoa
    {
        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Data de Nascimento")]
        public DateTime DataNascimento { get; set; }

        // Propriedade de navegação para o relacionamento
        public virtual ICollection<ConsultaMedica> Consultas { get; set; }

        // Propriedade calculada para mostrar a idade
        [DisplayName("Idade")]
        public int Idade
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DataNascimento.Year;
                if (DataNascimento.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}