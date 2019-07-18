using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class puestosTemp
    {
        [Display(Name ="Puesto")]
        public string descripcion { get; set; }
    }

    [MetadataType(typeof(puestosTemp))]
    public partial class puesto
    {
    }

}