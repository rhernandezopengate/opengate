using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenGate.ViewModels
{
    public partial class DetalleOrdenProductosViewModel
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public bool Retirar { get; set; }
        public decimal Monto()
        {
            return Cantidad * PrecioUnitario;
        }
    }
}