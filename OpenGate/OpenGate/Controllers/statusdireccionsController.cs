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
    public class statusdireccionsController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: statusdireccions
        public ActionResult Index()
        {
            return View(db.statusdireccion.ToList());
        }

        // GET: statusdireccions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statusdireccion statusdireccion = db.statusdireccion.Find(id);
            if (statusdireccion == null)
            {
                return HttpNotFound();
            }
            return View(statusdireccion);
        }

        // GET: statusdireccions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: statusdireccions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] statusdireccion statusdireccion)
        {
            if (ModelState.IsValid)
            {
                db.statusdireccion.Add(statusdireccion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statusdireccion);
        }

        // GET: statusdireccions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statusdireccion statusdireccion = db.statusdireccion.Find(id);
            if (statusdireccion == null)
            {
                return HttpNotFound();
            }
            return View(statusdireccion);
        }

        // POST: statusdireccions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] statusdireccion statusdireccion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statusdireccion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statusdireccion);
        }

        // GET: statusdireccions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statusdireccion statusdireccion = db.statusdireccion.Find(id);
            if (statusdireccion == null)
            {
                return HttpNotFound();
            }
            return View(statusdireccion);
        }

        // POST: statusdireccions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            statusdireccion statusdireccion = db.statusdireccion.Find(id);
            db.statusdireccion.Remove(statusdireccion);
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
