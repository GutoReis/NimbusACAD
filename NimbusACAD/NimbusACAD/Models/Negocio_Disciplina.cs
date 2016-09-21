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
    
    public partial class Negocio_Disciplina
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Negocio_Disciplina()
        {
            this.Negocio_Frequencia = new HashSet<Negocio_Frequencia>();
            this.Negocio_Quadro_Horario = new HashSet<Negocio_Quadro_Horario>();
            this.Negocio_Vinculo_Disciplina = new HashSet<Negocio_Vinculo_Disciplina>();
        }
    
        public int Disciplina_ID { get; set; }
        public int Modulo_ID { get; set; }
        public string Disciplina_Nome { get; set; }
        public string Descricao { get; set; }
        public int Professor_ID { get; set; }
        public Nullable<int> Tot_Aulas_Dadas { get; set; }
        public Nullable<int> Carga_Horaria { get; set; }
    
        public virtual Negocio_Funcionario Negocio_Funcionario { get; set; }
        public virtual Negocio_Modulo Negocio_Modulo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Negocio_Frequencia> Negocio_Frequencia { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Negocio_Quadro_Horario> Negocio_Quadro_Horario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Negocio_Vinculo_Disciplina> Negocio_Vinculo_Disciplina { get; set; }
    }
}
