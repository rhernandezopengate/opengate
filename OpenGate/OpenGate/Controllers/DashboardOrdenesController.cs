using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OpenGate.Entidades;

namespace OpenGate.Controllers
{
    public class DashboardOrdenesController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: DashboardOrdenes
        [Authorize]
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Index()
        {
            DateTime fechaHoy = DateTime.Now.Date;
            ViewBag.ConteoOrdenesAbiertas = db.ordenes.Where(x => x.StatusOrdenImpresa_Id ==1 && x.FechaAlta > fechaHoy).Count();
            ViewBag.ConteoOrdenesCerradas = db.ordenes.Where(x => x.StatusOrdenImpresa_Id > 1 && x.StatusOrdenImpresa_Id < 4 && x.FechaAlta > fechaHoy).Count();
            ViewBag.ConteoOrdenesBackOrder = db.ordenes.Where(x => x.StatusOrdenImpresa_Id == 4 && x.FechaAlta > fechaHoy).Count();
            ViewBag.ConteoOrdenes = db.ordenes.Where(x => x.FechaAlta > fechaHoy).Count();

            var toppickers = db.ordenes.Where(x => x.StatusOrdenImpresa_Id > 1 && x.FechaAlta > fechaHoy).GroupBy(x => x.Picker);
            var topauditores = db.detusuariosordenes.Where(x => x.ordenes.StatusOrdenImpresa_Id > 1 && x.ordenes.FechaAlta > fechaHoy).GroupBy(x => x.usuarios.nombre);
            

            var piezas = db.detordenproductoshd.Where(x => x.ordenes.FechaAlta > fechaHoy);
            var errores = db.erroresordenes.Where(x => x.ordenes.FechaAlta > fechaHoy);
            List<ordenes> listaPickers = new List<ordenes>();
            List<ordenes> listaAuditores = new List<ordenes>();

            foreach (var item in toppickers)
            {
                if (item.Key != null)
                {
                    ordenes ordenes = new ordenes();
                    
                    decimal qty = (int)piezas.Where(x => x.ordenes.Picker.Contains(item.Key.ToUpper())).Sum(x => x.cantidad);
                    var porcentajeRounded = Math.Round(qty, 2);
                    ordenes.CantidadPiezas = (int)porcentajeRounded;

                    if (errores != null)
                    {
                        ordenes.CantidadErrores = (int)errores.Where(x => x.ordenes.Picker.Contains(item.Key.ToUpper())).Count();
                    }
                    else
                    {
                        ordenes.CantidadErrores = 0;
                    }

                    ordenes.Picker = item.Key.ToUpper();
                    ordenes.Cantidad = item.Count(); 

                    listaPickers.Add(ordenes);
                }                
            }

            foreach (var item in topauditores)
            {
                if (item.Key != null)
                {
                    ordenes ordenes = new ordenes();
                    ordenes.Auditor = item.Key.ToUpper();
                    ordenes.Cantidad = item.Count();

                    var ordenesTemp = from dt in db.detusuariosordenes
                                      where dt.usuarios.nombre.Equals(item.Key) && dt.ordenes.FechaAlta > fechaHoy
                                      select dt.Ordenes_Id;

                    var detalleCantidad = from dt in db.detordenproductoshd
                                          where ordenesTemp.Contains(dt.Ordenes_Id)
                                          select dt;

                    ordenes.CantidadPiezas = (int)detalleCantidad.Sum(x=> x.cantidad);
                    
                    listaAuditores.Add(ordenes);
                }
            }

            ViewBag.ListaAuditores = listaAuditores.OrderBy(x => x.Cantidad);

            ViewBag.ListaPicker = listaPickers.OrderBy(x => x.CantidadPiezas);

            List<DataPoint> dataPoints = new List<DataPoint>();

            foreach (var item in toppickers)
            {
                if (item.Key != null)
                {
                    dataPoints.Add(new DataPoint(item.Key, (int)piezas.Where(x => x.ordenes.Picker.Contains(item.Key.ToUpper())).Sum(x => x.cantidad) * -1));
                }
            }

            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
         
            return View();
        }

        public ActionResult DetalleErrores(string picker)
        {
            try
            {
                DateTime fechaHoy = DateTime.Now.Date;
                
                var errores = (from erroreso in db.erroresordenes
                               join o in db.ordenes on erroreso.Ordenes_Id equals o.id
                               where o.Picker == picker && o.FechaAlta > fechaHoy
                               select erroreso).ToList();

                List<tipoerror> lista = new List<tipoerror>();

                foreach (var item in errores)
                {
                    tipoerror erroresordenes = new tipoerror();
                    erroresordenes.descripcion = item.tipoerror.descripcion;
                    erroresordenes.OrdenString = item.ordenes.Orden;
                    lista.Add(erroresordenes);
                }

                ViewBag.Picker = picker;
                ViewBag.Errores = lista;

                return View();
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}