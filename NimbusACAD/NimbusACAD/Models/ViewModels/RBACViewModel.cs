using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NimbusACAD.Models.ViewModels
{
    public class CriarPerfilRBACViewModel
    {
        [Required]
        [Display(Name = "Nome do perfil")]
        public string PerfilNm { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
    }

    public class CriarPermissaoRBACViewModel
    {
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
        public virtual ICollection<int> PermissaoID { get; set; }
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
}