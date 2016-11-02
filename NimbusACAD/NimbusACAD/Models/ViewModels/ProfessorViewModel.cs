using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NimbusACAD.Models.DB;

namespace NimbusACAD.Models.ViewModels
{
    //Frequencia --Automaticamente adicionar o valor de aulas a "Tot_Aulas_dadas" em Negocio_Disciplina
    public class FrequenciaViewModel
    {
        [Required]
        [Display(Name = "Disciplina")]
        public int DisciplinaID { get; set; }

        [Required]
        [Display(Name = "Professor")]
        public int ProfessorID { get; set; }

        [Required]
        [Display(Name = "Dia")]
        [DataType(DataType.Date)]
        public DateTime DtAula { get; set; }

        [Required]
        [Display(Name = "Quantidade de aulas")]
        public int QtdeAulas { get; set; }

        [Required]
        [Display(Name = "Aula Ministrada")]
        public string AulaMinistrada { get; set; }

        [Display(Name = "Alunos Presentes")]
        public virtual ICollection<ListaAlunosViewModel> Matriculas { get; set; }
    }

    public class ListaAlunosViewModel
    {
        public int MatriculaID { get; set; }

        [Required]
        [Display(Name = "Nome")]
        public string NomeAluno { get; set; }
    }

    //Notas
    public class ListaLancarNotaViewModel
    {
        public int DisciplinaID { get; set; }

        [Required]
        [Display(Name = "Disciplina")]
        public string DisciplinaNm { get; set; }

        public ICollection<LancarNotaViewModel> notas { get; set; }
    }

    public class LancarNotaViewModel
    {
        public int MatriculaID { get; set; }
        public int VinculoID { get; set; }

        [Required]
        [Display(Name = "Aluno")]
        public string AlunoNm { get; set; }

        [Display(Name = "1ª Avalição")]
        public double Nota1 { get; set; }

        [Display(Name = "2ª Avaliação")]
        public double Nota2 { get; set; }
    }

    //Disciplinas
    public class VerDisciplinasViewModel //DIARIO
    {
        [Required]
        [Display(Name = "Disciplina")]
        public string Disciplina { get; set; }

        [Required]
        [Display(Name = "Total de aulas dadas")]
        public int TotAulasDadas { get; set; }

        [Required]
        [Display(Name = "Alunos")]
        public virtual ICollection<ListaAlunosViewModel> Alunos { get; set; }

        [Required]
        [Display(Name = "Horário")]
        public virtual ICollection<ListaHorarioViewModel> Horarios { get; set; }
    }

    //Curso
    public class VerCursoProfessorViewModel
    {
        public int CursoID { get; set; }

        [Required]
        [Display(Name = "Curso")]
        public string CursoNm { get; set; }

        [Required]
        [Display(Name = "Período")]
        public string Periodo { get; set; }

        [Required]
        [Display(Name = "Modulo")]
        public virtual ICollection<int> ModuloID { get; set; }

        [Required]
        [Display(Name = "Disciplinas")]
        public virtual ICollection<Negocio_Disciplina> DisciplinaNm { get; set; }
    }

    public class DisciplinasProfessorVIewModel
    {
        [Required]
        [Display(Name = "Disciplinas")]
        public virtual ICollection<ListaDisciplinaViewModel> disciplinas { get; set; }
    }
}