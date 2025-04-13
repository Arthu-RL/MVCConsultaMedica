using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TrabalhoMVC.Models
{
    public class ConsultaMedica
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O paciente é obrigatório")]
        [DisplayName("Paciente")]
        public int PacienteId { get; set; }

        [Required(ErrorMessage = "O médico é obrigatório")]
        [DisplayName("Médico")]
        public int MedicoId { get; set; }

        [Required(ErrorMessage = "A data da consulta é obrigatória")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [DisplayName("Data da Consulta")]
        public DateTime DataConsulta { get; set; }

        [Required(ErrorMessage = "O status da consulta é obrigatório")]
        [DisplayName("Status")]
        public StatusConsulta Status { get; set; }

        [Required(ErrorMessage = "O valor da consulta é obrigatório")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        [DisplayName("Valor")]
        public decimal Valor { get; set; }

        [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
        [DisplayName("Observações")]
        public string Observacoes { get; set; }

        // Propriedades de navegação
        public virtual Paciente? Paciente { get; set; }
        public virtual Medico? Medico { get; set; }
    }
}