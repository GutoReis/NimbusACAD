//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NimbusACAD.Models.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Negocio_Modulo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Negocio_Modulo()
        {
            this.Negocio_Disciplina = new HashSet<Negocio_Disciplina>();
            this.Negocio_Vinculo_Modulo = new HashSet<Negocio_Vinculo_Modulo>();
        }
    
        public int Modulo_ID { get; set; }
        public string Modulo_Nome { get; set; }
        public int Curso_ID { get; set; }
        public Nullable<int> Max_Alunos { get; set; }
        public Nullable<int> Tot_Inscritos { get; set; }
        public Nullable<int> Carga_Horaria { get; set; }
    
        public virtual Negocio_Curso Negocio_Curso { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Negocio_Disciplina> Negocio_Disciplina { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Negocio_Vinculo_Modulo> Negocio_Vinculo_Modulo { get; set; }
    }
}
