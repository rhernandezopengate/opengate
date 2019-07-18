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
    public class centrocostosController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: centrocostos
        public ActionResult Index()
        {
            return View(db.centrocostos.ToList());
        }

        // GET: centrocostos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            centrocostos centrocostos = db.centrocostos.Find(id);
            if (centrocostos == null)
            {
                return HttpNotFound();
            }
            return View(centrocostos);
        }

        // GET: centrocostos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: centrocostos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] centrocostos centrocostos)
        {
            if (ModelState.IsValid)
            {
                db.centrocostos.Add(centrocostos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(centrocostos);
        }

        // GET: centrocostos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            centrocostos centrocostos = db.centrocostos.Find(id);
            if (centrocostos == null)
            {
                return HttpNotFound();
            }
            return View(centrocostos);
        }

        // POST: centrocostos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] centrocostos centrocostos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(centrocostos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(centrocostos);
        }

        // GET: centrocostos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            centrocostos centrocostos = db.centrocostos.Find(id);
            if (centrocostos == null)
            {
                return HttpNotFound();
            }
            return View(centrocostos);
        }

        // POST: centrocostos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            centrocostos centrocostos = db.centrocostos.Find(id);
            db.centrocostos.Remove(centrocostos);
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
