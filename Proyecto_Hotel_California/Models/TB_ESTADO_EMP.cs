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
    
    public partial class TB_ESTADO_EMP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_ESTADO_EMP()
        {
            this.TB_EMPLEADO = new HashSet<TB_EMPLEADO>();
        }
    
        public int COD_EST_EMP { get; set; }
        public string DESC_EST { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_EMPLEADO> TB_EMPLEADO { get; set; }
    }
}
