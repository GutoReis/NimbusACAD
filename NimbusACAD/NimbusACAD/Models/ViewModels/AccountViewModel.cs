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
        [DataType(DataType.Date)]
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
        public int Numero { get; set; }

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
        public int PessoaID { get; set; }

        [Required]
        [Display(Name = "Documento")]
        public int DocumentoID { get; set; }

        [Display(Name = "Orgão Emissor")]
        public string OrgaoEmissor { get; set; }

        [Display(Name = "Data de Emissão")]
        [DataType(DataType.Date)]
        public DateTime DtEmissao { get; set; }

        [Display(Name = "Cidade de Emissão")]
        public string Cidade { get; set; }

        [Display(Name = "Estado de Emissão")]
        public string Estado { get; set; }

        [Display(Name = "País de Emissão")]
        public string Pais { get; set; }
    }

    public class EsqueceuSenhaViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class AlterarSenhaViewModel
    {
        [Required]
        [Display(Name = "Nova Senha")]
        public string Senha { get; set; }

        [Required]
        [Display(Name = "Confirmar Senha")]
        public string ConfirmarSenha { get; set; }
    }

    public class PerfilDeUsuarioViewModel
    {
        //Infos de Negocio_Pessoa
        //Preenchido pelo sistema, não será alterado pelo usuário
        public int PessoaID { get; set; } //Vale tanto para Negocio_Pessoa como para RBAC_Usuario

        [Required(ErrorMessage = "*")]
        [Display(Name = "Primerio Nome")]
        public string PrimeiroNome { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Sobrenome")]
        public string Sobrenome { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "CPF")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "RG")]
        public string RG { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Sexo")]
        public string Sexo { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Data de nascimento")]
        [DataType(DataType.Date)]
        public DateTime DtNascimento { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Telefone principal")]
        public string TelPrincipal { get; set; }

        //Pode ser Nulo
        [Display(Name = "Telefone secundário")]
        public string TelSecundario { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Email")]
        public string Email { get; set; } //Vale tanto para Negocio_Pessoa como para RBAC_Usuario


        //Preenchido pelo sistema, não será alterado pelo usuário
        [Display(Name = "Email confrimado")]
        public bool EmailConfirmado { get; set; }

        //Preenchido pelo sistema, não será alterado pelo usuário
        [Display(Name = "Total de notificações não lidas")]
        public int TotNotifNL { get; set; }

        //Infos de Negocio_Endereço
        //Preenchido pelo sistema, terá a opção alterar endereço
        [Display(Name = "Endereço completo")]
        public string EndCompleto { get; set; }


        //Infos de RBAC_Usuario
        //Preenchido pelo sistema, não será alterado pelo usuário
        public int UsuarioID { get; set; }

        //Deverá ser trocada apenas na área "Alterar senha"
        [Display(Name = "Senha criptografada")]
        public string SenhaHash { get; set; }

        //Deverá ser trocada apenas na área "Alterar senha"
        [Display(Name = "Salt")]
        public string Salt { get; set; }

        //Preenchido pelo sistema, não será alterado pelo usuário
        [Display(Name = "Data de criação")]
        [DataType(DataType.Date)]
        public DateTime DtCriacao { get; set; }

        //Preenchido pelo sistema, será alterado pelo sisteman na atualização dos dados
        [Display(Name = "Data da ultima modificação")]
        [DataType(DataType.Date)]
        public DateTime DtModif { get; set; }

        //Preenchido pelo sistema, não será alterado pelo usuário
        [Display(Name = "Bloqueado")]
        public string Bloqueado { get; set; }

        //Info de RBAC_Perfil
        //Preenchido pelo sistema, não será alterado pelo usuário
        [Display(Name = "Perfil")]
        public string Perfil { get; set; }
    }

    public class ListaPerfisViewModel
    {
        [Display(Name = "Nome completo")]
        public string NmCompleto { get; set; }

        [Display(Name = "CPF")]
        public string CPF { get; set; }

        [Display(Name = "RG")]
        public string RG { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Telefone principal")]
        public string TelPrincipal { get; set; }

        //Fazer conversão de bool -> string
        [Display(Name = "Bloqueado")]
        public string Bloqueado { get; set; }
    }

    public class AlterarEnderecoViewModel
    {
        //Preenchido pelo sistema, não será alterado pelo usuário
        public int EnderecoID { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "CEP")]
        public string CEP { get; set; }

        [Display(Name = "Complemento")]
        public string Complemento { get; set; }

        [Display(Name = "Número")]
        public int Numero { get; set; }

        //Preenchido pelo sistema, não será alterado pelo usuário
        public bool Ativo { get; set; }

        //Preenchido pelo sistema, não será alterado pelo usuário
        public int PessoaID { get; set; }

        //Preenchido pelo sistema se o CEP já estiver cadastrado
        [Required(ErrorMessage = "*")]
        [Display(Name = "Logradouro")]
        public string Logradouro { get; set; }

        //Preenchido pelo sistema se o CEP já estiver cadastrado
        [Required(ErrorMessage = "*")]
        [Display(Name = "Bairro")]
        public string Bairro { get; set; }

        //Preenchido pelo sistema se o CEP já estiver cadastrado
        [Required(ErrorMessage = "*")]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; }

        //Preenchido pelo sistema se o CEP já estiver cadastrado
        [Required(ErrorMessage = "*")]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        //Preenchido pelo sistema se o CEP já estiver cadastrado
        [Required(ErrorMessage = "*")]
        [Display(Name = "País")]
        public string Pais { get; set; }
    }
}