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
    
    public partial class Negocio_Documento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Negocio_Documento()
        {
            this.Negocio_Curriculo = new HashSet<Negocio_Curriculo>();
            this.Negocio_Matricula_Aluno = new HashSet<Negocio_Matricula_Aluno>();
        }
    
        public int Documento_ID { get; set; }
        public string Documento_Nome { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Negocio_Curriculo> Negocio_Curriculo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Negocio_Matricula_Aluno> Negocio_Matricula_Aluno { get; set; }
    }
}
