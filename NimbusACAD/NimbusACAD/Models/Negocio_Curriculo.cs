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
    
    public partial class Negocio_Curriculo
    {
        public int Curriculo_ID { get; set; }
        public int Pessoa_ID { get; set; }
        public int Documento_ID { get; set; }
        public string Orgao_Emissor { get; set; }
        public Nullable<System.DateTime> Dt_Emissao { get; set; }
        public string Cidade_Emissao { get; set; }
        public string Estado_Emissao { get; set; }
        public string Pais_Emissao { get; set; }
    
        public virtual Negocio_Documento Negocio_Documento { get; set; }
        public virtual Negocio_Pessoa Negocio_Pessoa { get; set; }
    }
}
