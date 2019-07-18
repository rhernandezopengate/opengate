using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenGate.Controllers
{
    public class ErroresController : Controller
    {
        // GET: Errores
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error404()
        {
            return View();
        }

        public ActionResult Error500(string error)
        {
            ViewBag.Error = error;
            return View();
        }
    }
}