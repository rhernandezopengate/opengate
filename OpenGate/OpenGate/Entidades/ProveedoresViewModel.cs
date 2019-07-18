using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class ProveedoresViewModel
    {
        public proveedor Proveedores { get; set; }
        public informacionbancaria Informacionbancaria { get; set; }
        public string[] DynamicTextBox { get; set; }
        public string[] DynamicTextTelefono { get; set; }
        public string[] DynamicTextMail { get; set; }
        public string[] DynamicTextPuesto { get; set; }

        [StringLength(18, ErrorMessage = "El titulo no debe ser mayor a 50 caracteres")]
        public string ClabeInterbancaria { get; set; }

        public int NacionalidadProveedor_Id { get; set; }

        public int CategoriaProveedor_Id { get; set; }
    }
}