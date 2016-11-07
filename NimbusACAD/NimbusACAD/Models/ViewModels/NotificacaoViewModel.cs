using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NimbusACAD.Models.ViewModels
{
    public class EscreverNotifacaoViewModel
    {
        [Key]
        public int NotificacaoID { get; set; }

        [Required]
        [Display(Name = "De")]
        public int EmissorID { get; set; }

        [Required]
        [Display(Name = "Para")]
        public int ReceptorID { get; set; }

        [Required]
        [Display(Name = "Assunto")]
        public string Assunto { get; set; }

        [Required]
        [Display(Name = "Menssagem")]
        public string Corpo { get; set; }
    }

    public class LerNotifacaoViewModel
    {
        [Required]
        [Display(Name = "De")]
        public int EmissorID { get; set; }

        [Required]
        [Display(Name = "Assunto")]
        public string Assunto { get; set; }

        [Required]
        [Display(Name = "Menssagem")]
        public string Corpo { get; set; }
    }

    public class VerInfosNotifacaoViewModel
    {
        [Required]
        [Display(Name = "De")]
        public int EmissorID { get; set; }

        [Required]
        [Display(Name = "Para")]
        public int ReceptorID { get; set; }

        [Required]
        [Display(Name = "Assunto")]
        public string Assunto { get; set; }

        [Required]
        [Display(Name = "Menssagem")]
        public string Corpo { get; set; }

        [Required]
        [Display(Name = "Lida")]
        public bool Lida { get; set; }
    }
}