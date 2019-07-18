using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class kpisinventariosexactitudTemp
    {
    }

    public partial class kpisinventariosexactitud
    {
        private string porcentaje;

        public string Porcentaje { get => porcentaje; set => porcentaje = value; }

        public string absolutoString { get; internal set; }

        public string valorString { get; internal set; }
    }

}