using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;
using System.Linq.Dynamic;

namespace OpenGate.Controllers
{
    [Authorize]
    public class detalleordenproductosController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: detalleordenproductos
        public ActionResult Index()
        {
            var detalleordenproductos = db.detalleordenproductos.Include(d => d.productos).Include(d => d.ordencompra);


            return View(detalleordenproductos.ToList());
        }

        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult ReportesSKUS()
        {
            return View();
        }

        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult ObtenerOrdenes()
        {
            try
            {
                var Draw = Request.Form.GetValues("draw").FirstOrDefault();
                var Start = Request.Form.GetValues("start").FirstOrDefault();
                var Length = Request.Form.GetValues("length").FirstOrDefault();
                var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
                var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                var fechaInicio = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                var fechaFin = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

                int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
                int Skip = Start != null ? Convert.ToInt32(Start) : 0;
                int TotalRecords = 0;

                List<detordenproductoshd> Inventario = new List<detordenproductoshd>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec SP_Inventario_ConsultaParametrosOpcionales @fechaOrdenes";
                    var query = new SqlCommand(sql, con);

                    if (fechaInicio != "")
                    {
                        DateTime date = Convert.ToDateTime(fechaInicio);
                        query.Parameters.AddWithValue("@fechaOrdenes", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaOrdenes", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var detalle = new detordenproductoshd();

                            detalle.FechaString = dr["FechaAlta"].ToString();
                            detalle.OrdenString = dr["Orden"].ToString();
                            detalle.Cantidad = Convert.ToInt32(dr["cantidad"]);
                            detalle.SKUDescripcion = dr["Sku"].ToString();

                            Inventario.Add(detalle);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    Inventario = Inventario.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = Inventario.ToList().Count();
                var NewItems = Inventario.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        // GET: detalleordenproductos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detalleordenproductos detalleordenproductos = db.detalleordenproductos.Find(id);
            if (detalleordenproductos == null)
            {
                return HttpNotFound();
            }
            return View(detalleordenproductos);
        }

        // GET: detalleordenproductos/Create
        public ActionResult Create()
        {
            ViewBag.Productos_Id = new SelectList(db.productos, "id", "Codigo");
            ViewBag.OrdenCompra_Id = new SelectList(db.ordencompra, "id", "NombreProyecto");
            return View();
        }

        // POST: detalleordenproductos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,OrdenCompra_Id,Productos_Id,cantidad,preciounitario,monto")] detalleordenproductos detalleordenproductos)
        {
            if (ModelState.IsValid)
            {
                db.detalleordenproductos.Add(detalleordenproductos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Productos_Id = new SelectList(db.productos, "id", "Codigo", detalleordenproductos.Productos_Id);
            ViewBag.OrdenCompra_Id = new SelectList(db.ordencompra, "id", "NombreProyecto", detalleordenproductos.OrdenCompra_Id);
            return View(detalleordenproductos);
        }

        // GET: detalleordenproductos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detalleordenproductos detalleordenproductos = db.detalleordenproductos.Find(id);
            if (detalleordenproductos == null)
            {
                return HttpNotFound();
            }
            ViewBag.Productos_Id = new SelectList(db.productos, "id", "Codigo", detalleordenproductos.Productos_Id);
            ViewBag.OrdenCompra_Id = new SelectList(db.ordencompra, "id", "NombreProyecto", detalleordenproductos.OrdenCompra_Id);
            return View(detalleordenproductos);
        }

        // POST: detalleordenproductos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,OrdenCompra_Id,Productos_Id,cantidad,preciounitario,monto")] detalleordenproductos detalleordenproductos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detalleordenproductos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Productos_Id = new SelectList(db.productos, "id", "Codigo", detalleordenproductos.Productos_Id);
            ViewBag.OrdenCompra_Id = new SelectList(db.ordencompra, "id", "NombreProyecto", detalleordenproductos.OrdenCompra_Id);
            return View(detalleordenproductos);
        }

        // GET: detalleordenproductos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            detalleordenproductos detalleordenproductos = db.detalleordenproductos.Find(id);
            if (detalleordenproductos == null)
            {
                return HttpNotFound();
            }
            return View(detalleordenproductos);
        }

        // POST: detalleordenproductos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            detalleordenproductos detalleordenproductos = db.detalleordenproductos.Find(id);
            db.detalleordenproductos.Remove(detalleordenproductos);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
