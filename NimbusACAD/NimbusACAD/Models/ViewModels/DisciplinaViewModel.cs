using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NimbusACAD.Models.ViewModels
{
    //Criar disciplina
    public class DisciplinaViewModel
    {
        [Required]
        [Display(Name = "Modulo")]
        public int ModuloID { get; set; }

        [Required]
        [Display(Name = "Nome da Disciplina")]
        public string DisciplinaNM { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Professor")]
        public int ProfessorID { get; set; }

        [Required]
        [Display(Name = "Carga Horária")]
        public int CargaHoraria { get; set; }

        //Horario
        [Required]
        [Display(Name = "Dia")]
        public string DiaSemana { get; set; }

        HorarioViewModel HVM = new HorarioViewModel();
    }

    public class HorarioViewModel
    {
        [Required]
        [Display(Name = "Disciplina")]
        public int DisciplinaID { get; set; }

        [Required]
        [Display(Name = "Dia da semana")]
        public string DiaSemana { get; set; }

        [Required]
        [Display(Name = "Hora de Início")]
        public DateTime HoraInicio { get; set; }

        [Required]
        [Display(Name = "Hora de Fim")]
        public DateTime HoraFim { get; set; }
    }

}