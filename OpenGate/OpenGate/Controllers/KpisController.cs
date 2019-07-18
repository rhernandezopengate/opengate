using Newtonsoft.Json;
using OpenGate.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenGate.Controllers
{
    [Authorize]
    public class KpisController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: Kpis
        public ActionResult Index()
        {
            try
            {
                var ultimasemana = db.kpisplaneacion.Max(x => x.wk);
                var ultimasemanadespacho = db.kpisdespacho.Max(x => x.wk);
                var mes = db.kpisinvetntarios.OrderByDescending(x => x.id).First().mes;
                var mesaplontime = db.kpisinventariosaplontime.OrderByDescending(x => x.id).First().Mes;
                var mesxactitud = db.kpisinventariosexactitud.OrderByDescending(x => x.id).First().mes;
                var wkRecibo = db.kpisrecibo.OrderByDescending(x => x.id).First().wk;
                ViewBag.mesAplOntime = mesaplontime;
                ViewBag.mesCedis = mes;
                ViewBag.semana = ultimasemana;
                ViewBag.semanadespacho = ultimasemanadespacho;
                ViewBag.mesExactitud = mesxactitud;
                ViewBag.WkRecibo = wkRecibo;
            }
            catch (Exception)
            {
                ViewBag.semana = 0;                
            }     

            return View();
        }

        public ActionResult Trafico()
        {
            ViewBag.MesOnTime = db.kpistrafico.OrderByDescending(x => x.id).First().mescarga;
            ViewBag.MesEvalOcupacion = db.kpistrafico.OrderByDescending(x => x.id).First().mescarga;
            return View();
        }
    }
}