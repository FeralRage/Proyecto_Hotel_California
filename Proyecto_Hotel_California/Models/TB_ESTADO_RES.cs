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
    
    public partial class TB_ESTADO_RES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_ESTADO_RES()
        {
            this.TB_RESERVA = new HashSet<TB_RESERVA>();
        }
    
        public int COD_EST_RES { get; set; }
        public string DESC_EST { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_RESERVA> TB_RESERVA { get; set; }
    }
}
