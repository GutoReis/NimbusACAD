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
    
    public partial class RBAC_Perfil
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RBAC_Perfil()
        {
            this.RBAC_Link_Perfil_Permissao = new HashSet<RBAC_Link_Perfil_Permissao>();
            this.RBAC_Link_Usuario_Perfil = new HashSet<RBAC_Link_Usuario_Perfil>();
        }
    
        public int Perfil_ID { get; set; }
        public string Perfil_Nome { get; set; }
        public string Descricao { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RBAC_Link_Perfil_Permissao> RBAC_Link_Perfil_Permissao { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RBAC_Link_Usuario_Perfil> RBAC_Link_Usuario_Perfil { get; set; }
    }
}