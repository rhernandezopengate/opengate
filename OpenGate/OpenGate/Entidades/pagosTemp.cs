using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class pagosTemp
    {
        [Display(Name = "Status")]
        public string statusString { get; set; }
        [Display(Name = "Total")]
        public string TotalString { get; set; }
    }

    [MetadataType(typeof(pagosTemp))]

    public partial class pago
    {
        public string statusString { get; set; }
        public string TotalString { get; set; }
    }
}