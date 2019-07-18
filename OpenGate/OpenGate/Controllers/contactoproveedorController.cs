using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;

namespace OpenGate.Controllers
{
    public class contactoproveedorController : Controller
    {
        // GET: contactoproveedor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return PartialView("_Create");
        }

        [HttpPost]
        public ActionResult Create(contactoproveedor contactoproveedor)
        {
            return Json("Error");
        }
    }
}