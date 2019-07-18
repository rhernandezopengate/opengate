using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class kpistraficoTemp
    {
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public Nullable<int> numerocajas { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public Nullable<int> numeropiezas { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public Nullable<decimal> palletsembarcados { get; set; }
    }

    [MetadataType(typeof(kpistraficoTemp))]
    public partial class kpistrafico
    {
        public string CostoFleteString { get; set; }
        public string CostoManiobrasString { get; set; }
        public string Estado { get; set; }
        public string EstadoPeso { get; set; }
        public string EstadoTotal { get; set; }
        public int Total { get; internal set; }
        public int TotalOks { get; internal set; }
        public int TotalDesv { get; internal set; }
        public decimal procentaje { get; internal set; }        
        public string fhrequeridaposicionamientoString { get; internal set; }
        public string fhrealpsocicionamientoString { get; internal set; }
        public string fhcitaString { get; internal set; }
        public string fharriborealString { get; internal set; }
        public string FacturacionFleteString { get; internal set; }
        public string FacturacionManiobrasString { get; internal set; }
        public decimal Porcentaje { get; internal set; }
        public string NumeroCajasString { get; internal set; }
    }
}