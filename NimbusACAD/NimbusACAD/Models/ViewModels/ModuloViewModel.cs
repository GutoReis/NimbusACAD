using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NimbusACAD.Models.ViewModels
{
    public class CriarModuloViewModel
    {
        [Required]
        [Display(Name = "Curso")]
        public int CursoID { get; set; }

        [Required]
        [Display(Name = "Máximo de alunos")]
        public int MaxAlunos { get; set; }

        [Required]
        [Display(Name = "Carga horária")]
        public int CargaHoraria { get; set; }
    }

    public class VerModuloViewModel
    {
        [Required]
        [Display(Name = "Curso")]
        public int CursoID { get; set; }

        [Required]
        [Display(Name = "Máximo de alunos")]
        public int MaxAlunos { get; set; }

        [Required]
        [Display(Name = "Total de alunos inscritos")]
        public int TotAlunos { get; set; }

        [Required]
        [Display(Name = "Carga horária")]
        public int CargaHoraria { get; set; }
    }
}