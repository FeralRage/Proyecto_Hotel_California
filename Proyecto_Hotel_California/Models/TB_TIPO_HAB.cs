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
    
    public partial class TB_TIPO_HAB
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TB_TIPO_HAB()
        {
            this.TB_HABITACION = new HashSet<TB_HABITACION>();
        }
    
        public int COD_TIPO_HAB { get; set; }
        public string DESC_TIPO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TB_HABITACION> TB_HABITACION { get; set; }
    }
}
