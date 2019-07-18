using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class serviciosTemp
    {
        [Display(Name = "Aplicaciones")]
        public bool respaldoaplicaciones { get; set; }
        [Display(Name = "CCTV")]
        public bool cctv { get; set; }
        [Display(Name = "Telefonia")]
        public bool telefonia { get; set; }
        [Display(Name = "Alarmas")]
        public bool alarmas { get; set; }
        [Display(Name = "Correo")]
        public bool correo { get; set; }
        [Display(Name = "Antivirus")]
        public bool antivirus { get; set; }
        [Display(Name = "Usuarios Wifi")]        
        public bool usuarioswifi { get; set; }
        [Display(Name = "Internet Carga")]
        public string internetcarga { get; set; }
        [Display(Name = "Internet Descarga")]
        public string internetdescarga { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha Registro")]
        public Nullable<System.DateTime> fecharegistro { get; set; }
    }

    [MetadataType(typeof(serviciosTemp))]
    public partial class servicios
    {
        public Nullable<System.DateTime> fecharegistro { get; set; }
        public bool cctv { get; set; }
        public bool telefonia { get; set; }
        public bool alarmas { get; set; }
        public bool correo { get; set; }
        public bool antivirus { get; set; }
        public bool respaldoaplicaciones { get; set; }
        public bool usuarioswifi { get; set; }
    }
}