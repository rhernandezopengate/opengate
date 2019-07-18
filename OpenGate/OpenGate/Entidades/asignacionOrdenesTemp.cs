using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class asignacionOrdenesTemp
    {
    }

    public partial class asignacionordenes
    {
        public int CSRID { get; set; }

        public string Guia { get; set; }

        public string Orden { get; set; }

        public string Referencia { get; set; }

        public string DSIDCliente { get; set; }

        public string StatusDHL { get; set; }

        public string StatusManual { get; set; }

        public DateTime FechaRecoleccion { get; set; }

        public DateTime FechaProbableEntrega { get; set; }

        public int IsVencido { get; set; }
    }
}