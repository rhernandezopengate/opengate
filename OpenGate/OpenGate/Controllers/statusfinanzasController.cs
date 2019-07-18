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
    public class statusfinanzasController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: statusfinanzas
        public ActionResult Index()
        {
            return View(db.statusfinanzas.ToList());
        }

        // GET: statusfinanzas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statusfinanzas statusfinanzas = db.statusfinanzas.Find(id);
            if (statusfinanzas == null)
            {
                return HttpNotFound();
            }
            return View(statusfinanzas);
        }

        // GET: statusfinanzas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: statusfinanzas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] statusfinanzas statusfinanzas)
        {
            if (ModelState.IsValid)
            {
                db.statusfinanzas.Add(statusfinanzas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statusfinanzas);
        }

        // GET: statusfinanzas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statusfinanzas statusfinanzas = db.statusfinanzas.Find(id);
            if (statusfinanzas == null)
            {
                return HttpNotFound();
            }
            return View(statusfinanzas);
        }

        // POST: statusfinanzas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] statusfinanzas statusfinanzas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statusfinanzas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statusfinanzas);
        }

        // GET: statusfinanzas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statusfinanzas statusfinanzas = db.statusfinanzas.Find(id);
            if (statusfinanzas == null)
            {
                return HttpNotFound();
            }
            return View(statusfinanzas);
        }

        // POST: statusfinanzas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            statusfinanzas statusfinanzas = db.statusfinanzas.Find(id);
            db.statusfinanzas.Remove(statusfinanzas);
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
