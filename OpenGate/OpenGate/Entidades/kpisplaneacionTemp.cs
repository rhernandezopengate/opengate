using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class kpisplaneacionTemp
    {
    }

    [MetadataType(typeof(kpisplaneacionTemp))]
    public partial class kpisplaneacion
    {
        public int Total { get; set; }
        public int TotalOks { get; set; }
        public int TotalDesv { get; set; }
        public decimal procentaje { get; set; }
        public int sumaTotal { get; set; }
    }
}