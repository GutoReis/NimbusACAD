//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NimbusACAD.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Negocio_Quadro_Horario
    {
        public int Quadro_Horario_ID { get; set; }
        public int Disciplina_ID { get; set; }
        public string Dia_Semana { get; set; }
        public Nullable<System.TimeSpan> Hora_Inicio { get; set; }
        public Nullable<System.TimeSpan> Hora_Fim { get; set; }
    
        public virtual Negocio_Disciplina Negocio_Disciplina { get; set; }
    }
}