using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class kpisinventariosaplontimeTemp
    {
    }

    public partial class kpisinventariosaplontime
    {
        public int Total { get; internal set; }
        public int TotalOks { get; internal set; }
        public int TotalDesv { get; internal set; }
        public decimal procentaje { get; internal set; }
    }
}