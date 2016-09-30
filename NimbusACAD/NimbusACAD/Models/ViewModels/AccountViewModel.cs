using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NimbusACAD.Models;
using System.Linq;
using System.Web;

namespace NimbusACAD.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usuário")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Senha")]
        public string Password { get; set; }
    }

    public class RegistrarComumViewModel
    {
        //Infos do usuário
        [Required]
        [Display(Name = "Nome")]
        public string PrimeiroNome { get; set; }

        [Required]
        [Display(Name = "Sobrenome")]
        public string Sobrenome { get; set; }

        [Required]
        [Display(Name = "CPF")]
        public string CPF { get; set; }

        [Required]
        [Display(Name = "RG")]
        public string RG { get; set; }

        [Required]
        [Display(Name = "Sexo")]
        public string Sexo { get; set; }

        [Required]
        [Display(Name = "Data de Nascimento")]
        public DateTime DtNascimento { get; set; }

        [Required]
        [Display(Name = "Telefone (Obrigatório)")]
        public string TelPrincipal { get; set; }

        [Display(Name = "Telefone (Opcional)")]
        public string TelOpcional { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //infos de endereço
        [Required]
        [Display(Name = "CEP")]
        public string CEP { get; set; }

        [Required]
        [Display(Name = "Logradouro")]
        public string Logradouro { get; set; }

        [Display(Name = "Complemento")]
        public string Complemento { get; set; }

        [Display(Name = "Numero")]
        public string Numero { get; set; }

        [Required]
        [Display(Name = "Bairro")]
        public string Bairro { get; set; }

        [Required]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Required]
        [Display(Name = "Pais")]
        public string Pais { get; set; }
    }

    public class CurriculoViewModel
    {
        [Required]
        [Display(Name = "Usuário")]
        public string Pessoa_ID { get; set; }

        [Required]
        [Display(Name = "Documento")]
        public string Documento_ID { get; set; }

        [Display(Name = "Orgão Emissor")]
        public string Orgao_Emissor { get; set; }

        [Display(Name = "Data de Emissão")]
        public DateTime Dt_Emissao { get; set; }

        [Display(Name = "Cidade de Emissão")]
        public string Cidade { get; set; }

        [Display(Name = "Estado de Emissão")]
        public string Estado { get; set; }

        [Display(Name = "País de Emissão")]
        public string Pais { get; set; }
    }

}