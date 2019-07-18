using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;

namespace OpenGate.Controllers
{
    public class ordencompraController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: ordencompra
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.CentroCostos_Id = new SelectList(db.centrocostos, "Id", "Descripcion");
            ViewBag.SubCentroCostos_Id = new SelectList(db.subcentrocostos, "Id", "Descripcion");
            ViewBag.Proveedores_Id = new SelectList(db.proveedor, "Id", "RazonSocial");
            ViewBag.CategoriaOrden_Id = new SelectList(db.categoriaorden, "Id", "Descripcion");
            ViewBag.TipoCompra_Id = new SelectList(db.tipocompra, "Id", "Descripcion");
            ViewBag.Clientes_Id = new SelectList(db.clientes, "Id", "RazonSocial");
            ViewBag.FormaPago_Id = new SelectList(db.formapago, "Id", "Descripcion");

            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }
    }
}