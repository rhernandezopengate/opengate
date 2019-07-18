using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;

namespace OpenGate.Controllers
{
    [Authorize]
    public class proveedorsController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {            
            var proveedores = (from proveedor in db.proveedor
                             where proveedor.RazonSocial.Contains(prefix)
                             orderby proveedor.RazonSocial ascending
                             select new
                             {
                                 label = proveedor.RazonSocial,
                                 val = proveedor.id
                             }).ToList();

            return Json(proveedores);
        }

        // GET: proveedors
        public ActionResult Index()
        {
            var proveedor = db.proveedor.Include(p => p.categoriaproveedor).Include(p => p.nacionalidadproveedor).Include(p => p.statusproveedor);
            return View(proveedor.ToList());
        }

        // GET: proveedors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            proveedor proveedor = db.proveedor.Find(id);
            if (proveedor == null)
            {
                return HttpNotFound();
            }
            return View(proveedor);
        }

        // GET: proveedors/Create
        public ActionResult Create()
        {
            ViewBag.categoriaproveedor_id = new SelectList(db.categoriaproveedor, "id", "descripcion");
            ViewBag.nacionalidadproveedor_id = new SelectList(db.nacionalidadproveedor, "id", "descripcion");
            ViewBag.statusproveedor_id = new SelectList(db.statusproveedor, "id", "descripcion");
            return View();
        }

        // POST: proveedors/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,razonsocial,rfc,nacionalidadproveedor_id,categoriaproveedor_id,statusproveedor_id")] proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                db.proveedor.Add(proveedor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.categoriaproveedor_id = new SelectList(db.categoriaproveedor, "id", "descripcion", proveedor.CategoriaProveedor_Id);
            ViewBag.nacionalidadproveedor_id = new SelectList(db.nacionalidadproveedor, "id", "descripcion", proveedor.NacionalidadProveedor_Id);
            ViewBag.statusproveedor_id = new SelectList(db.statusproveedor, "id", "descripcion", proveedor.StatusProveedor_Id);
            return View(proveedor);
        }

        // GET: proveedors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            proveedor proveedor = db.proveedor.Find(id);
            if (proveedor == null)
            {
                return HttpNotFound();
            }
            ViewBag.categoriaproveedor_id = new SelectList(db.categoriaproveedor, "id", "descripcion", proveedor.CategoriaProveedor_Id);
            ViewBag.nacionalidadproveedor_id = new SelectList(db.nacionalidadproveedor, "id", "descripcion", proveedor.NacionalidadProveedor_Id);
            ViewBag.statusproveedor_id = new SelectList(db.statusproveedor, "id", "descripcion", proveedor.StatusProveedor_Id);
            return View(proveedor);
        }

        // POST: proveedors/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,razonsocial,rfc,nacionalidadproveedor_id,categoriaproveedor_id,statusproveedor_id")] proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                if (proveedor.StatusProveedor_Id == 2)
                {
                    var facturas = (from f in db.factura
                                   where f.Proveedor_Id == proveedor.id &&
                                   f.StatusFactura_Id == 1
                                   select f);

                    foreach (var item in facturas)
                    {
                        item.StatusFactura_Id = 4;
                        db.factura.AddOrUpdate(item);                        
                    }                    
                }
                else
                {
                    var facturas = (from f in db.factura
                                    where f.Proveedor_Id == proveedor.id &&
                                    f.StatusFactura_Id == 4
                                    select f);

                    foreach (var item in facturas)
                    {
                        item.StatusFactura_Id = 1;
                        db.factura.AddOrUpdate(item);
                    }                    
                }
                
                db.Entry(proveedor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.categoriaproveedor_id = new SelectList(db.categoriaproveedor, "id", "descripcion", proveedor.CategoriaProveedor_Id);
            ViewBag.nacionalidadproveedor_id = new SelectList(db.nacionalidadproveedor, "id", "descripcion", proveedor.NacionalidadProveedor_Id);
            ViewBag.statusproveedor_id = new SelectList(db.statusproveedor, "id", "descripcion", proveedor.StatusProveedor_Id);
            return View(proveedor);
        }

        // GET: proveedors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            proveedor proveedor = db.proveedor.Find(id);
            if (proveedor == null)
            {
                return HttpNotFound();
            }
            return View(proveedor);
        }

        // POST: proveedors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            proveedor proveedor = db.proveedor.Find(id);
            db.proveedor.Remove(proveedor);
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
