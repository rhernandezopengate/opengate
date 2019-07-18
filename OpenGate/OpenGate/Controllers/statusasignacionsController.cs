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
    public class statusasignacionsController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: statusasignacions
        public ActionResult Index()
        {
            return View(db.statusasignacion.ToList());
        }

        // GET: statusasignacions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statusasignacion statusasignacion = db.statusasignacion.Find(id);
            if (statusasignacion == null)
            {
                return HttpNotFound();
            }
            return View(statusasignacion);
        }

        // GET: statusasignacions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: statusasignacions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] statusasignacion statusasignacion)
        {
            if (ModelState.IsValid)
            {
                db.statusasignacion.Add(statusasignacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statusasignacion);
        }

        // GET: statusasignacions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statusasignacion statusasignacion = db.statusasignacion.Find(id);
            if (statusasignacion == null)
            {
                return HttpNotFound();
            }
            return View(statusasignacion);
        }

        // POST: statusasignacions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] statusasignacion statusasignacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statusasignacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statusasignacion);
        }

        // GET: statusasignacions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statusasignacion statusasignacion = db.statusasignacion.Find(id);
            if (statusasignacion == null)
            {
                return HttpNotFound();
            }
            return View(statusasignacion);
        }

        // POST: statusasignacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            statusasignacion statusasignacion = db.statusasignacion.Find(id);
            db.statusasignacion.Remove(statusasignacion);
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
