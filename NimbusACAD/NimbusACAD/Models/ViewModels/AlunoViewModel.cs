using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NimbusACAD.Models.ViewModels
{
    public class AlunoViewModel
    {
        public class RegistrarAlunoViewModel
        {
            [Required]
            [Display(Name = "Aluno")]
            public int AlunoID { get; set; }

            [Required]
            [Display(Name = "Identificador do Curso")]
            public int Curso { get; set; }

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
        
        //Fazer vinculo-dsiciplina
        public class CriarVinculoModuloViewModel
        {
            [Required]
            [Display(Name = "Modulo")]
            public int ModuloID { get; set; }

            [Required]
            [Display(Name = "Aluno")]
            public int MatriculaID { get; set; }

            //Se está: e, curso c/ dependencia, c/ matricula trancada,
            //em curso, reprovado, aprovado
            [Required]
            [Display(Name = "Status do aluno")]
            public string StatusVinculo { get; set; }
        }

        //Ver vinculo-Modulo
        public class VerVinculoModuloViewModel
        {
            [Required]
            [Display(Name = "Modulo")]
            public int ModuloID { get; set; }

            [Required]
            [Display(Name = "Status no modulo")]
            public string StatusVinculo { get; set; }
        }

        //Criar vinculo-disciplina
        public class CriarVinculoDisciplinaViewModel
        {
            [Required]
            [Display(Name = "Disciplina")]
            public int DisciplinaID { get; set; }

            [Required]
            [Display(Name = "Aluno")]
            public int MatriculaID { get; set; }

            [Required]
            [Display(Name = "Numero de chamada")]
            public int NumChamada { get; set; }
        }

        //Ver vinculo-Disciplina
        public class VerVinculoDisciplinaViewModel
        {
            [Required]
            [Display(Name = "Disciplina")]
            public string DisciplinaNm { get; set; }

            [Required]
            [Display(Name = "Aluno")]
            public string NmAluno { get; set; }

            [Required]
            [Display(Name = "Numero de chamada")]
            public int NumChamado { get; set; }

            [Required]
            [Display(Name = "1ª Avalição")]
            public float Nota1 { get; set; }

            [Required]
            [Display(Name = "2ª Avalição")]
            public float Nota2 { get; set; }

            [Required]
            [Display(Name = "Media final")]
            public float MediaFinal { get; set; }

            [Required]
            [Display(Name = "Frequencia")]
            public int Frequencia { get; set; }

            [Required]
            [Display(Name = "Horario")]
            public string DiaHorarioDisc { get; set; }
        }

        //Notas de todas as disciplinas + frequencia por disciplina
        public class NotasViewModel
        {
            [Required]
            [Display(Name = "Disciplina")]
            public string DisciplinaNm { get; set; }

            [Display(Name = "1º Avaliação")]
            public float Nota1 { get; set; }

            [Display(Name = "2º Avaliação")]
            public float Nota2 { get; set; }

            [Required]
            [Display(Name = "Media final")]
            public float MediaFinal { get; set; }
        }
    }
}