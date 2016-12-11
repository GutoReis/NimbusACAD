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
    }

    public class VerDisciplinaViewModel
    {
        [Key]
        public int DisciplinaID { get; set; }

        public int ModuloID { get; set; }

        [Required]
        [Display(Name = "Modulo")]
        public string ModuloNM { get; set; }

        [Required]
        [Display(Name = "Nome da Disciplina")]
        public string DisciplinaNM { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [Display(Name = "Professor")]
        public string ProfessorNM { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Carga Horária")]
        public int CargaHoraria { get; set; }

        [Required]
        [Display(Name = "Horários da disciplina")]
        public virtual ICollection<ListaHorarioViewModel> horariosAula { get; set; }
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
        [DataType(DataType.Time)]
        public DateTime HoraInicio { get; set; }

        [Required]
        [Display(Name = "Hora de Fim")]
        [DataType(DataType.Time)]
        public DateTime HoraFim { get; set; }
    }

    public class ListaHorarioViewModel
    {
        [Key]
        public int horarioID { get; set; }

        [Required]
        [Display(Name = "Dia da semana")]
        public string DiaSemana { get; set; }

        [Required]
        [Display(Name = "Hora de Início")]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        [Display(Name = "Hora de Fim")]
        public TimeSpan HoraFim { get; set; }
    }

    public class ListaDisciplinaViewModel
    {
        [Key]
        [Display(Name = "ID")]
        public int DisciplinaID { get; set; }

        [Required]
        [Display(Name = "Disciplina")]
        public string DisciplinaNM { get; set; }
    }
}