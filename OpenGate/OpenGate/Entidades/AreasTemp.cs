using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class AreasTemp
    {
        [Display(Name = "Area")]
        public string descripcion { get; set; }
    }

    [MetadataType(typeof(AreasTemp))]
    public partial class area
    {
    }

}