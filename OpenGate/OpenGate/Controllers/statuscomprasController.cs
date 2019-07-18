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
    public class statuscomprasController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: statuscompras
        public ActionResult Index()
        {
            return View(db.statuscompras.ToList());
        }

        // GET: statuscompras/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statuscompras statuscompras = db.statuscompras.Find(id);
            if (statuscompras == null)
            {
                return HttpNotFound();
            }
            return View(statuscompras);
        }

        // GET: statuscompras/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: statuscompras/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] statuscompras statuscompras)
        {
            if (ModelState.IsValid)
            {
                db.statuscompras.Add(statuscompras);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statuscompras);
        }

        // GET: statuscompras/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statuscompras statuscompras = db.statuscompras.Find(id);
            if (statuscompras == null)
            {
                return HttpNotFound();
            }
            return View(statuscompras);
        }

        // POST: statuscompras/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] statuscompras statuscompras)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statuscompras).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statuscompras);
        }

        // GET: statuscompras/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statuscompras statuscompras = db.statuscompras.Find(id);
            if (statuscompras == null)
            {
                return HttpNotFound();
            }
            return View(statuscompras);
        }

        // POST: statuscompras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            statuscompras statuscompras = db.statuscompras.Find(id);
            db.statuscompras.Remove(statuscompras);
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
