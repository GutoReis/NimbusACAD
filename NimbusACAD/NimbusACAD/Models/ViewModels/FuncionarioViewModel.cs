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

    public class VerFuncionarioViewModel
    {
        public int funcionarioID { get; set; }
        public int pessoaID { get; set; }
        public int cargoID { get; set; }

        [Required]
        [Display(Name = "Funcionário")]
        public string FuncionarioNM { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Telefone Principal")]
        public string TelefonePrincipal { get; set; }

        [Required]
        [Display(Name = "Telefone Secundário")]
        public string TelefoneSecundario { get; set; }

        [Required]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }
        
        [Required]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; }
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