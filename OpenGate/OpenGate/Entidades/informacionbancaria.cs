//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenGate.Entidades
{
    using System;
    using System.Collections.Generic;
    
    public partial class informacionbancaria
    {
        public int id { get; set; }
        public string NombreBanco { get; set; }
        public string CuentaBancaria { get; set; }
        public string Clabe { get; set; }
        public int Proveedores_Id { get; set; }
    
        public virtual proveedor proveedor { get; set; }
    }
}
