//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proyecto_Hotel_California.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TB_EMPLEADO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_EMPLEADO()
        {
            this.TB_USUARIO = new HashSet<TB_USUARIO>();
        }
    
        public string COD_EMP { get; set; }
        public string NOM_EMP { get; set; }
        public string APE_EMP { get; set; }
        public int COD_TIPO_DOC { get; set; }
        public string NUM_DOC { get; set; }
        public int COD_TIPO_EMP { get; set; }
        public Nullable<System.DateTime> FEC_NAC { get; set; }
        public Nullable<System.DateTime> FEC_CONTRATO { get; set; }
        public Nullable<System.DateTime> FEC_CESE { get; set; }
        public string TELEFONO { get; set; }
        public string DIRECCION { get; set; }
        public int COD_SEDE { get; set; }
        public int COD_EST_EMP { get; set; }
    
        public virtual TB_ESTADO_EMP TB_ESTADO_EMP { get; set; }
        public virtual TB_SEDE TB_SEDE { get; set; }
        public virtual TB_TIPO_DOC TB_TIPO_DOC { get; set; }
        public virtual TB_TIPO_EMP TB_TIPO_EMP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_USUARIO> TB_USUARIO { get; set; }
    }
}
