using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;

namespace OpenGate.Controllers
{
    public class DashboardOrdenesController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: DashboardOrdenes
        public ActionResult Index()
        {
            DateTime fechaHoy = DateTime.Now.Date;
            ViewBag.ConteoOrdenesCerradas = db.ordenes.Where(x => x.StatusOrdenImpresa_Id == 3 && x.FechaAlta > fechaHoy).Count();
            ViewBag.ConteoOrdenes = db.ordenes.Where(x => x.FechaAlta > fechaHoy).Count();

            var query = db.ordenes.Where(x => x.StatusOrdenImpresa_Id == 3).GroupBy(x => x.Picker);
            List<ordenes> lista = new List<ordenes>();
            
            foreach (var item in query)
            {
                if (item.Key != null)
                {
                    ordenes ordenes = new ordenes();
                    ordenes.Picker = item.Key;
                    ordenes.Cantidad = item.Count();

                    lista.Add(ordenes);
                }                
            }

            ViewBag.ListaPicker = lista;

            //ViewBag.ConteoOrdenesCerradas = .Count();
            return View();
        }
    }
}