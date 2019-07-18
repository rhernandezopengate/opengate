using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class complementosfacturasTemp
    {
    }

    [MetadataType(typeof(complementosfacturasTemp))]
    public partial class complementosfacturas
    {
        public string  NumeroFactura1 { get; set; }
        public DateTime FechaFactura1 { get; set; }
        public DateTime FechaPago1 { get; set; }
        public string Concepto1 { get; set; }              
        public string SubtotalString1 { get; set; }
        public string IvaString1 { get; set; }
        public string RetencionString1 { get; set; }
        public string DescuentoString1 { get; set; }
        public string TotalString1 { get; set; }
        public string BancoString1 { get; set; }
        public string CuentaString1 { get; set; }
        public string RazonString1 { get; set; }
        public string StatusComplemento1 { get; set; }
    }
}