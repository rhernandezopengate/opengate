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
                               join a in db.detusuariosordenes on o.id equals a.Ordenes_Id
                               where o.Picker == picker && o.FechaAlta > fechaHoy
                               select new { erroreso, a });

                List<tipoerror> lista = new List<tipoerror>();

                foreach (var item in errores)
                {
                    tipoerror erroresordenes = new tipoerror();
                    erroresordenes.descripcion = item.erroreso.tipoerror.descripcion;
                    erroresordenes.OrdenString = item.erroreso.ordenes.Orden;
                    erroresordenes.Auditor = item.a.usuarios.nombre;
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


        public ActionResult ConteoMensual(int month)
        {
            DateTime fechaHoy = Convert.ToDateTime("2019/10/01");

            //Cambiar mes
            var toppickers = db.ordenes.Where(x => (x.StatusOrdenImpresa_Id > 1 || x.StatusOrdenImpresa_Id < 5) && (x.FechaAlta.Value.Month == month)).GroupBy(x => x.Picker);
            //Cambiar mes
            var topauditores = db.detusuariosordenes.Where(x => (x.ordenes.StatusOrdenImpresa_Id > 1) && (x.ordenes.FechaAlta.Value.Month == month)).GroupBy(x => x.usuarios.nombre);
            //Cambiar mes
            var piezas = db.detordenproductoshd.Where(x => x.ordenes.FechaAlta.Value.Month == month);
            //Cambiar mes
            var errores = db.erroresordenes.Where(x => x.ordenes.FechaAlta.Value.Month == month);

            List<ordenes> listaPickers = new List<ordenes>();
            List<ordenes> listaAuditores = new List<ordenes>();
            List<DataPoint> dataPoints = new List<DataPoint>();

            foreach (var item in toppickers)
            {
                if (item.Key != null)
                {
                    ordenes ordenes = new ordenes();

                    var p = piezas.Where(x => x.ordenes.Picker.Contains(item.Key.ToUpper())).Sum(x => x.cantidad);

                    if (p != null)
                    {                        
                        var porcentajeRounded = Math.Round((decimal)p, 2);
                        ordenes.CantidadPiezas = (int)porcentajeRounded;
                    }
                    else
                    {
                        ordenes.CantidadPiezas = 0;
                    }

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

                    ordenes.CantidadPiezas = (int)detalleCantidad.Sum(x => x.cantidad);

                    listaAuditores.Add(ordenes);
                }
            }

            foreach (var item in toppickers)
            {
                if (item.Key != null)
                {
                    var p = piezas.Where(x => x.ordenes.Picker.Contains(item.Key.ToUpper())).Sum(x => x.cantidad);

                    if (p != null)
                    {
                        dataPoints.Add(new DataPoint(item.Key, (int)piezas.Where(x => x.ordenes.Picker.Contains(item.Key.ToUpper())).Sum(x => x.cantidad) * -1));
                    }
                    else
                    {
                        dataPoints.Add(new DataPoint(item.Key, 0));
                    }                    
                }
            }

            ViewBag.ListaPicker = listaPickers.OrderBy(x => x.CantidadPiezas);
            ViewBag.ListaAuditores = listaAuditores.OrderBy(x => x.CantidadPiezas);
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints.OrderByDescending(x => x.Y));

            return View();
        }

        public ActionResult DetalleErroresMensual(string picker)
        {
            try
            {
                DateTime fechaHoy = Convert.ToDateTime("2019/09/01");

                var errores = (from erroreso in db.erroresordenes
                               join o in db.ordenes on erroreso.Ordenes_Id equals o.id
                               join a in db.detusuariosordenes on o.id equals a.Ordenes_Id
                               where o.Picker == picker && o.FechaAlta.Value.Month == 9
                               select new { erroreso, a });

                List<tipoerror> lista = new List<tipoerror>();

                foreach (var item in errores)
                {
                    tipoerror erroresordenes = new tipoerror();
                    erroresordenes.descripcion = item.erroreso.tipoerror.descripcion;
                    erroresordenes.OrdenString = item.erroreso.ordenes.Orden;
                    erroresordenes.Auditor = item.a.usuarios.nombre;
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