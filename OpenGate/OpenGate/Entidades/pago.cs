//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenGate.Entidades
{
    using System;
    using System.Collections.Generic;
    
    public partial class pago
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public pago()
        {
            this.pagofacturas = new HashSet<pagofacturas>();
        }
    
        public int id { get; set; }
        public Nullable<System.DateTime> FechaRegistro { get; set; }
        public Nullable<int> Numero { get; set; }
        public Nullable<System.DateTime> FechaPago { get; set; }
        public Nullable<decimal> MontoTotal { get; set; }
        public string AspNetUsers_Id { get; set; }
        public int StatusPago_Id { get; set; }
    
        public virtual AspNetUsers AspNetUsers { get; set; }
        public virtual statuspago statuspago { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pagofacturas> pagofacturas { get; set; }
    }
}
