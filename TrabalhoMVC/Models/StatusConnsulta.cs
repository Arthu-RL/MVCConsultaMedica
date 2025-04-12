using System.ComponentModel.DataAnnotations;

namespace TrabalhoMVC.Models
{
    public enum StatusConsulta
    {
        [Display(Name = "Agendada")]
        Agendada,

        [Display(Name = "Confirmada")]
        Confirmada,

        [Display(Name = "Realizada")]
        Realizada,

        [Display(Name = "Cancelada")]
        Cancelada
    }
}
