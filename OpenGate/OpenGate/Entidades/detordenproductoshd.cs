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
    
    public partial class detordenproductoshd
    {
        public int id { get; set; }
        public int Ordenes_Id { get; set; }
        public int Skus_Id { get; set; }
        public Nullable<int> cantidad { get; set; }
        public int Cantidad { get; internal set; }
        public virtual ordenes ordenes { get; set; }
        public virtual skus skus { get; set; }
        public string FechaString { get; internal set; }
        public string OrdenString { get; internal set; }
        public string OracleID { get; internal set; }
        public string SKUDescripcion { get; internal set; }
        public string SKU { get; internal set; }
    }
}
