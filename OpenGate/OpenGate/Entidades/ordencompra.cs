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
    
    public partial class ordencompra
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ordencompra()
        {
            this.detalleordenproductos = new HashSet<detalleordenproductos>();
        }
    
        public int id { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public string NombreProyecto { get; set; }
        public string Locacion { get; set; }
        public string LugarEntrega { get; set; }
        public Nullable<int> TiempoRespuesta { get; set; }
        public Nullable<System.DateTime> FechaEntrega { get; set; }
        public string Justificacion { get; set; }
        public string Politicas { get; set; }
        public int CentroCostos_Id { get; set; }
        public int SubCentroCostos_Id { get; set; }
        public int Proveedores_Id { get; set; }
        public int CategoriaOrden_Id { get; set; }
        public int TipoCompra_Id { get; set; }
        public int StatusFinanzas_Id { get; set; }
        public int StatusCompras_Id { get; set; }
        public int StatusDireccion_Id { get; set; }
        public int Solicitante_Id { get; set; }
        public int Clientes_Id { get; set; }
        public Nullable<int> FormaPago_Id { get; set; }
    
        public virtual categoriaorden categoriaorden { get; set; }
        public virtual centrocostos centrocostos { get; set; }
        public virtual clientes clientes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<detalleordenproductos> detalleordenproductos { get; set; }
        public virtual formapago formapago { get; set; }
        public virtual proveedor proveedor { get; set; }
        public virtual solicitante solicitante { get; set; }
        public virtual statuscompras statuscompras { get; set; }
        public virtual statusdireccion statusdireccion { get; set; }
        public virtual statusfinanzas statusfinanzas { get; set; }
        public virtual subcentrocostos subcentrocostos { get; set; }
        public virtual tipocompra tipocompra { get; set; }
        public decimal Total { get; internal set; }
    }
}
