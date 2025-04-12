using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrabalhoMVC.Models
{
    public abstract class Pessoa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        [DisplayName("Nome Completo")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(14, ErrorMessage = "O CPF deve ter 14 caracteres")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "CPF inválido. Use o formato: 000.000.000-00")]
        [DisplayName("CPF")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "O telefone deve ter entre 10 e 20 caracteres")]
        [DisplayName("Telefone")]
        [DataType(DataType.PhoneNumber)]
        public string Telefone { get; set; }
    }
}