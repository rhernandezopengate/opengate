using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;

namespace OpenGate.Controllers
{
    [Authorize]
    public class statusfacturasController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: statusfacturas
        public ActionResult Index()
        {
            return View(db.statusfactura.ToList());
        }

        // GET: statusfacturas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statusfactura statusfactura = db.statusfactura.Find(id);
            if (statusfactura == null)
            {
                return HttpNotFound();
            }
            return View(statusfactura);
        }

        // GET: statusfacturas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: statusfacturas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] statusfactura statusfactura)
        {
            if (ModelState.IsValid)
            {
                db.statusfactura.Add(statusfactura);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statusfactura);
        }

        // GET: statusfacturas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statusfactura statusfactura = db.statusfactura.Find(id);
            if (statusfactura == null)
            {
                return HttpNotFound();
            }
            return View(statusfactura);
        }

        // POST: statusfacturas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] statusfactura statusfactura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statusfactura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statusfactura);
        }

        // GET: statusfacturas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statusfactura statusfactura = db.statusfactura.Find(id);
            if (statusfactura == null)
            {
                return HttpNotFound();
            }
            return View(statusfactura);
        }

        // POST: statusfacturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            statusfactura statusfactura = db.statusfactura.Find(id);
            db.statusfactura.Remove(statusfactura);
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
