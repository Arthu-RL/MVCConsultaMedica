using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrabalhoMVC.Models
{
    public class Medico : Pessoa
    {
        [Required(ErrorMessage = "O CRM é obrigatório")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "O CRM deve ter entre 5 e 20 caracteres")]
        [DisplayName("CRM")]
        public string CRM { get; set; }

        [Required(ErrorMessage = "A especialidade é obrigatória")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "A especialidade deve ter entre 3 e 50 caracteres")]
        [DisplayName("Especialidade")]
        public string Especialidade { get; set; }

        [DisplayName("Médico Ativo")]
        public bool Ativo { get; set; } = true;

        // Propriedade de navegação para o relacionamento
        public virtual ICollection<ConsultaMedica>? Consultas { get; set; }

        // Propriedade calculada para exibir o nome completo com título e CRM
        [DisplayName("Nome Completo")]
        public string NomeCompletoComCRM
        {
            get
            {
                return $"{Nome} ({CRM})";
            }
        }
    }
}