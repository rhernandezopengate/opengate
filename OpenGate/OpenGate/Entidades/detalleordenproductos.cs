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
    
    public partial class detalleordenproductos
    {
        public int id { get; set; }
        public int OrdenCompra_Id { get; set; }
        public int Productos_Id { get; set; }
        public Nullable<int> cantidad { get; set; }
        public Nullable<decimal> preciounitario { get; set; }
        public Nullable<decimal> monto { get; set; }
    
        public virtual productos productos { get; set; }
        public virtual ordencompra ordencompra { get; set; }
    }
}
