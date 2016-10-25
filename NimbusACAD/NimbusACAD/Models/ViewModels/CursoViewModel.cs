using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NimbusACAD.Models.ViewModels
{
    public class CriarCursoViewModel
    {
        [Required]
        [Display(Name = "Nome do Curso")]
        public string CursoNm { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Perído")]
        public string Periodo { get; set; }

        [Required]
        [Display(Name = "Coordenador")]
        public int CoordenadorID { get; set; }

        [Required]
        [Display(Name = "Carga Horária")]
        public int CargaHoraria { get; set; }
    }

    public class VerCursoViewModel
    {
        public int CursoID { get; set; }

        [Required]
        [Display(Name = "Nome do Curso")]
        public string CursoNm { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Perído")]
        public string Periodo { get; set; }

        [Required]
        [Display(Name = "Coordenador")]
        public string CoordenadorNM { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string CoordenadorEmail { get; set; }

        [Required]
        [Display(Name = "Carga Horária")]
        public int CargaHoraria { get; set; }

        [Required]
        [Display(Name = "Módulos")]
        public virtual ICollection<ListaModulosViewModel> Modulos { get; set; }
    }
}