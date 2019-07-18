using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class concentradoTemp
    {
    }

    [MetadataType(typeof(concentradoTemp))]
    public partial class concentrado
    {
        public int IdGuia { get; set; }
        public string NTS { get; set; }
        public DateTime? NTSDate { get; set; }
        public string Concatenado { get; set; }
        public string Guia { get; set; }
        public string ReferenciaCSR { get; set; }
        public string ChekPoint { get; set; }

        public string CPDestinatario { get; set; }
    }
}