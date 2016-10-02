using System.ComponentModel.DataAnnotations;

namespace NimbusACAD.Models.ViewModels
{
    public class RegistrarFuncionarioViewModel
    {
        [Required]
        [Display(Name = "Funcionário")]
        public int PessoaID { get; set; }

        [Required]
        [Display(Name = "Cargo")]
        public int CargoID { get; set; }
    }

    public class TipoFuncionarioViewModel
    {
        [Required]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
    }
}