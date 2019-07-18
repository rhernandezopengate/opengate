using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class EmpleadosTemp
    {
        [Display(Name = "Nombre(s)")]
        public string Nombre { get; set; }
        [Display(Name = "A. Paterno")]
        public string ApellidoPaterno { get; set; }
        [Display(Name = "A. Materno")]
        public string ApellidoMaterno { get; set; }
        [Display(Name = "Fecha Ingreso")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaIngreso { get; set; }
        public string Direccion { get; set; }
        public int Puesto_Id { get; set; }
        public int Area_Id { get; set; }
    }

    [MetadataType(typeof(EmpleadosTemp))]
    public partial class empleado
    {

    }
}