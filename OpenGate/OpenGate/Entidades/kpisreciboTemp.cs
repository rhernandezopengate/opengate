using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class kpisreciboTemp
    {
    }

    public partial class kpisrecibo
    {
        public int Total { get; internal set; }
        public int TotalOks { get; internal set; }
        public int TotalDesv { get; internal set; }
        public decimal procentaje { get; internal set; }
        public string fechahoradescargastring { get; internal set; }
        public string fechahoraingresosistemastring { get; internal set; }
    }

}