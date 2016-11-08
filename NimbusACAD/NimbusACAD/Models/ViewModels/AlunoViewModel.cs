using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NimbusACAD.Models.ViewModels
{
    public class RegistrarAlunoViewModel
    {
        [Key][Column(Order = 0)]
        [Display(Name = "Aluno")]
        public int PessoaID { get; set; }

        [Key][Column(Order = 1)]
        [Display(Name = "Curso")]
        public int CursoID { get; set; }

        [Required]
        [Display(Name = "Modulo")]
        public int ModuloID { get; set; }

        [Required]
        [Display(Name = "Ano")]
        public int Ano { get; set; }

        [Required]
        [Display(Name = "Deve Documentação")]
        public bool DeveDocumento { get; set; }
    }

    public class DocumentosDeventesViewModel
    {
        [Required]
        [Display(Name = "Matricula")]
        public int Matricula { get; set; }

        [Required]
        [Display(Name = "Documento")]
        public int DocumentoID { get; set; }
    }
        
    public class CriarVinculoModuloViewModel
    {
        [Required]
        [Display(Name = "Modulo")]
        public int ModuloID { get; set; }

        [Required]
        [Display(Name = "Aluno")]
        public int MatriculaID { get; set; }

        //Se está: Dependencia, Matricula Trancada,
        //Em Curso, Reprovado, Aprovado
        [Required]
        [Display(Name = "Status do aluno")]
        public string StatusVinculo { get; set; }
    }

    public class VerVinculoModuloViewModel
    {
        [Key]
        public int VinculoID { get; set; }

        [Required]
        [Display(Name = "Modulo")]
        public string ModuloNM { get; set; }

        [Required]
        [Display(Name = "Status no modulo")]
        public string StatusVinculo { get; set; }

        [Required]
        [Display(Name = "Disciplinas")]
        public virtual ICollection<ListaDisciplinasViewModel> disciplinas { get; set; }
    }

    public class CriarVinculoDisciplinaViewModel
    {
        [Key][Column(Order = 0)]
        [Display(Name = "Disciplina")]
        public int DisciplinaID { get; set; }

        [Key][Column(Order = 1)]
        [Display(Name = "Aluno")]
        public int MatriculaID { get; set; }

        [Required]
        [Display(Name = "Numero de chamada")]
        public int NumChamada { get; set; }
    }

    public class VerVinculoDisciplinaViewModel //DIARIO
    {
        [Key]
        [Display(Name = "Disciplina")]
        public string DisciplinaNm { get; set; }

        [Required]
        [Display(Name = "Professor")]
        public string Professor { get; set; }

        [Required]
        [Display(Name = "Aluno")]
        public string NmAluno { get; set; }

        [Required]
        [Display(Name = "Numero de chamada")]
        public int NumChamada { get; set; }

        [Required]
        [Display(Name = "1ª Avalição")]
        public double Nota1 { get; set; }

        [Required]
        [Display(Name = "2ª Avalição")]
        public double Nota2 { get; set; }

        [Required]
        [Display(Name = "Media final")]
        public double MediaFinal { get; set; }

        [Required]
        [Display(Name = "Total de aulas dadas")]
        public int TotAulasDadas { get; set; }

        [Required]
        [Display(Name = "Frequencia")]
        public int Frequencia { get; set; }

        [Required]
        [Display(Name = "Horarios")]
        public virtual ICollection<ListaHorarioViewModel> horarios { get; set; }
    }
    
    public class ListarNotasAlunoViewModel
    {
        [Key]
        public int MatriculaID { get; set; }

        public virtual ICollection<NotasAlunoViewModel> notas { get; set; }
    }

    public class NotasAlunoViewModel //DIARIO
    {
        [Key]
        public int VinculoID { get; set; }

        [Required]
        [Display(Name = "Modulo")]
        public string ModuloNm { get; set; }

        [Required]
        [Display(Name = "Disciplina")]
        public string DisciplinaNm { get; set; }

        [Required]
        [Display(Name = "Professor")]
        public string Professor { get; set; }

        [Display(Name = "1º Avaliação")]
        public double Nota1 { get; set; }

        [Display(Name = "2º Avaliação")]
        public double Nota2 { get; set; }

        [Required]
        [Display(Name = "Media final")]
        public double MediaFinal { get; set; }

        [Required]
        [Display(Name = "Frequencia")]
        public int Frequencia { get; set; }
    }

    public class ListaNotasDisciplinaViewModel
    {
        [Key]
        public int DisciplinaID { get; set; }

        [Required]
        [Display(Name = "Disciplina")]
        public string DisciplinaNm { get; set; }

        public virtual ICollection<NotasDisciplinaViewModel> notas { get; set; }
    }

    public class NotasDisciplinaViewModel
    {
        [Key]
        public int VinculoID { get; set; }

        [Required]
        [Display(Name = "Aluno")]
        public string AlunoNm { get; set; }

        [Display(Name = "1º Avaliação")]
        public double Nota1 { get; set; }

        [Display(Name = "2º Avaliação")]
        public double Nota2 { get; set; }

        [Required]
        [Display(Name = "Media final")]
        public double MediaFinal { get; set; }

        [Required]
        [Display(Name = "Frequencia")]
        public int Frequencia { get; set; }
    }

    public class VerAlunoViewModel
    {
        //pID
        public int pID { get; set; }
        //matID
        [Key]
        public int matID { get; set; }
        //Nome
        [Required]
        [Display(Name = "Nome")]
        public string Nome { get; set; }
        //Email
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        //Ativo
        [Required]
        [Display(Name = "Ativo")]
        public string Ativo { get; set; }
        //DeveDocumento
        [Required]
        [Display(Name = "Deve Documento")]
        public string DeveDocumento { get; set; }
        //Documentos deventes
        [Required]
        [Display(Name = "Documentos Deventes")]
        public virtual ICollection<VerDocsDeventesViewModel> documentos { get; set; }
        //CursoID
        public int cID { get; set; }
        //Curso
        [Required]
        [Display(Name = "Curso")]
        public string Curso { get; set; }
        //Modulos <List>
        [Required]
        [Display(Name = "Modulos")]
        public virtual ICollection<ListaVinculoModuloViewModel> modulos { get; set; }
    }

    public class VerDocsDeventesViewModel
    {
        [Key]
        public int DocumentoID { get; set; }

        [Required]
        [Display(Name = "Documento")]
        public string DocumentoNM { get; set; }
    }

    public class ListaVinculoModuloViewModel
    {
        [Key]
        public int VinculoID { get; set; }

        [Required]
        [Display(Name = "Modulo")]
        public string ModuloNM { get; set; }

        [Required]
        [Display(Name = "Status no modulo")]
        public string StatusVinculo { get; set; }
    }

    public class ListaDisciplinasViewModel
    {
        [Key]
        public int DisciplinaID { get; set; }
        
        [Required]
        [Display(Name = "Disciplina")]
        public string DisciplinaNM { get; set; }
    }
}