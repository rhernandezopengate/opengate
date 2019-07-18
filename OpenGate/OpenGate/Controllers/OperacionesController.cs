using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace OpenGate.Controllers
{
    public class OperacionesController : Controller
    {
        // GET: Operaciones
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Fletes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string[] DynamicTextBox)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ViewBag.Values = serializer.Serialize(DynamicTextBox);

            foreach (var textboxValue in DynamicTextBox)
            {

            }

            ViewBag.Message = DynamicTextBox.Count() + "records add";

            return View();
        }
    }
}