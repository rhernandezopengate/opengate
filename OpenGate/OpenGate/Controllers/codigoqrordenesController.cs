using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Configuration;

namespace OpenGate.Controllers
{
    [Authorize]
    public class codigoqrordenesController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        // GET: codigoqrordenes
        public ActionResult Index()
        {            
            return View();
        }

        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        [HttpPost]
        public ActionResult ObtenerQR()
        {
            try
            {
                var Draw = Request.Form.GetValues("draw").FirstOrDefault();
                var Start = Request.Form.GetValues("start").FirstOrDefault();
                var Length = Request.Form.GetValues("length").FirstOrDefault();
                var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
                var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                var fechaRegistro = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

                int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
                int Skip = Start != null ? Convert.ToInt32(Start) : 0;
                int TotalRecords = 0;

                List<codigoqrordenes> listaQR = new List<codigoqrordenes>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec SP_CodigoQrOrdenes_ConsultaParametrosOpcionales @fechaOrdenes";
                    var query = new SqlCommand(sql, con);

                    if (fechaRegistro != "")
                    {
                        DateTime date = Convert.ToDateTime(fechaRegistro);
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
                            var qrcode = new codigoqrordenes();

                            qrcode.id = Convert.ToInt32(dr["id"]);
                            qrcode.CodigoQR = dr["CodigoQR"].ToString();
                            qrcode.OrdenConsulta = dr["Orden"].ToString();
                            qrcode.FechaAltaOrden = Convert.ToDateTime(dr["FechaAlta"].ToString());
                                                       
                            listaQR.Add(qrcode);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listaQR = listaQR.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listaQR.ToList().Count();
                var NewItems = listaQR.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        // GET: codigoqrordenes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            codigoqrordenes codigoqrordenes = db.codigoqrordenes.Find(id);
            if (codigoqrordenes == null)
            {
                return HttpNotFound();
            }
            return View(codigoqrordenes);
        }

        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        // GET: codigoqrordenes/Create
        public ActionResult Create()
        {
            ViewBag.Ordenes_Id = new SelectList(db.ordenes, "id", "Orden");
            return View();
        }

        // POST: codigoqrordenes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,CodigoQR,Ordenes_Id")] codigoqrordenes codigoqrordenes)
        {
            if (ModelState.IsValid)
            {
                db.codigoqrordenes.Add(codigoqrordenes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Ordenes_Id = new SelectList(db.ordenes, "id", "Orden", codigoqrordenes.Ordenes_Id);
            return View(codigoqrordenes);
        }

        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        // GET: codigoqrordenes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            codigoqrordenes codigoqrordenes = db.codigoqrordenes.Find(id);
            if (codigoqrordenes == null)
            {
                return HttpNotFound();
            }
            ViewBag.Ordenes_Id = new SelectList(db.ordenes, "id", "Orden", codigoqrordenes.Ordenes_Id);
            return View(codigoqrordenes);
        }

        // POST: codigoqrordenes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,CodigoQR,Ordenes_Id")] codigoqrordenes codigoqrordenes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(codigoqrordenes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Ordenes_Id = new SelectList(db.ordenes, "id", "Orden", codigoqrordenes.Ordenes_Id);
            return View(codigoqrordenes);
        }

        // GET: codigoqrordenes/Delete/5
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            codigoqrordenes codigoqrordenes = db.codigoqrordenes.Find(id);
            if (codigoqrordenes == null)
            {
                return HttpNotFound();
            }
            return View(codigoqrordenes);
        }

        // POST: codigoqrordenes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult DeleteConfirmed(int id)
        {
            codigoqrordenes codigoqrordenes = db.codigoqrordenes.Find(id);
            db.codigoqrordenes.Remove(codigoqrordenes);
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
