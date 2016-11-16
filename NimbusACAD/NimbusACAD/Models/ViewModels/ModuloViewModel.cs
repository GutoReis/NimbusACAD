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
        [Display(Name = "Nome Módulo")]
        public string ModuloNome { get; set; }

        [Required]
        [Display(Name = "Máximo de alunos")]
        public int MaxAlunos { get; set; }

        [Required]
        [Display(Name = "Carga horária")]
        public int CargaHoraria { get; set; }
    }

    public class VerModuloViewModel
    {
        [Key]
        public int ModuloID { get; set; }

        public int CursoID { get; set}

        [Required]
        [Display(Name = "Curso")]
        public string CursoNM { get; set; }

        [Required]
        [Display(Name = "Nome Módulo")]
        public string ModuloNome { get; set; }

        [Required]
        [Display(Name = "Máximo de alunos")]
        public int MaxAlunos { get; set; }

        [Required]
        [Display(Name = "Total de alunos inscritos")]
        public int TotAlunos { get; set; }

        [Required]
        [Display(Name = "Carga horária")]
        public int CargaHoraria { get; set; }

        [Required]
        [Display(Name = "Disciplinas")]
        public virtual ICollection<ListaDisciplinaViewModel> disciplinas { get; set; }
    }

    public class ListaModulosViewModel
    {
        public int ModuloID { get; set; }
        public string ModuloNome { get; set; }
    }
}