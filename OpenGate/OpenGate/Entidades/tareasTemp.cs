using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenGate.Entidades
{
    public class tareasTemp
    {
        [AllowHtml]
        public string Notas { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> Fecha { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FechaEntrega { get; set; }
    }

    [MetadataType(typeof(tareasTemp))]
    public partial class tareas
    {
        public int folio { set; get; }
    }
}