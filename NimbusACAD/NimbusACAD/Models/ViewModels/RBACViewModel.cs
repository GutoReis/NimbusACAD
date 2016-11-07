using NimbusACAD.Models.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NimbusACAD.Models.ViewModels
{
    public class CriarPerfilRBACViewModel
    {
        public int PerfilID { get; set; }

        [Required]
        [Display(Name = "Nome do perfil")]
        public string PerfilNm { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
    }

    public class CriarPermissaoRBACViewModel
    {
        public int PermissaoID { get; set; }

        [Required]
        [Display(Name = "Nome da permissão")]
        public string PermissaoNm { get; set; }
    }

    public class VinculoPerfilPermissaoViewModel
    {
        [Required]
        [Display(Name = "Perfil")]
        public int PerfilID { get; set; }

        [Required]
        [Display(Name = "Permissão")]
        public int PermissaoID { get; set; }
    }

    public class VinculoPerfilUsuarioViewModel
    {
        [Required]
        [Display(Name = "Usuário")]
        public int UsuarioID { get; set; }

        [Required]
        [Display(Name = "Perfil")]
        public int PerfilID { get; set; }
    }

    public class ListaUsuariosPerfilViewModel
    {
        public int usuarioID { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string UsuarioNome { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ListaPermissoesPerfilViewModel
    {
        public int permisssaoID { get; set; }

        [Required]
        [Display(Name = "Permissao")]
        public string PermissaoNome { get; set; }
    }

    public class ListaPerfisPermissaoViewModel
    {
        public int perfilID { get; set; }

        [Required]
        [Display(Name = "Perfil")]
        public string PerfilNm { get; set; }
    }

    public class BloquearUsuarioViewModel
    {
        [Key]
        public int usuarioID { get; set; }

        [Required]
        [Display(Name = "Bloqueado")]
        public bool Bloqueado { get; set; }
    }

    public class VerPerfilViewModel
    {
        [Required]
        [Display(Name = "ID")]
        public int PerfilID { get; set; }

        [Required]
        [Display(Name = "Perfil")]
        public string PerfilNome { get; set; }

        [Required]
        [Display(Name = "Permissões")]
        public virtual ICollection<RBAC_Permissao> Permissoes { get; set; }
    }
}