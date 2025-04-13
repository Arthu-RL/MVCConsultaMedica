using System.ComponentModel.DataAnnotations;

namespace TrabalhoMVC.Models
{
    public enum StatusConsulta
    {
        [Display(Name = "Agendada")]
        Agendada = 0,

        [Display(Name = "Confirmada")]
        Confirmada = 1,

        [Display(Name = "Realizada")]
        Realizada = 2,

        [Display(Name = "Cancelada")]
        Cancelada = 3
    }
}