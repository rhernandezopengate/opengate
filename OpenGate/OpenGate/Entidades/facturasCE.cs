using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class facturasCE
    {
        public List<factura> FacturasPorPagar { get; set; }
    }
}