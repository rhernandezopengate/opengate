using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class facturasTemp
    {
        [Display(Name="DV")]
        public int dv { get; set; }
        [Display(Name = "Fecha Factura Fin")]
        public DateTime FechaFacturaFin { get; set; }
        [Display(Name = "Fecha Pago Fin")]
        public DateTime FechaPagoFin { get; set; }
        [Display(Name = "Proveedor")]
        public string RazonSocial { get; set; }
        [Display(Name = "Subtotal")]
        public string SubTotalString { get; set; }
        [Display(Name = "Iva")]
        public string IvaString { get; set; }
        [Display(Name = "Descuento")]
        public string DescuentoString { get; set; }
        [Display(Name = "Retencion")]
        public string RetencionString { get; set; }
        [Display(Name = "Total")]
        public string TotalString { get; set; }
        [Display(Name = "Estado")]
        public string Estado { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FechaFactura { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FechaPago { get; set; }
    }

    [MetadataType(typeof(facturasTemp))]
    public partial class factura
    {
        public int dv { get; set; }
        public DateTime FechaFacturaFin { get; set; }
        public DateTime FechaPagoFin { get; set; }
        public string RazonSocial { get; set; }
        public string SubTotalString { get; set; }
        public string Estado { get; set; }
        public string IvaString { get; set; }
        public string DescuentoString { get; set; }
        public string RetencionString { get; set; }
        public string TotalString { get; set; }
        public string FechaPagoString { get; set; }
        public string FechaPagoFinString { get; set; }
        public DateTime FechaVencimientoFin { get; set; }
        public bool IsChecked { get; set; }
        public int IsComplemento { get; set; }
        public string proveedorString { get; set; }        
    }
}