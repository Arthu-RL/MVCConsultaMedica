using System.Collections.Generic;

namespace TrabalhoMVC.Models
{
    public class Sintoma
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        // Navegação para o relacionamento muitos para muitos
        public virtual ICollection<ConsultaMedica> Consultas { get; set; }
    }
}