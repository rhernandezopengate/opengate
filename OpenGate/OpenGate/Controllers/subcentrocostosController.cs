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
    public class subcentrocostosController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: subcentrocostos
        public ActionResult Index()
        {
            return View(db.subcentrocostos.ToList());
        }

        // GET: subcentrocostos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            subcentrocostos subcentrocostos = db.subcentrocostos.Find(id);
            if (subcentrocostos == null)
            {
                return HttpNotFound();
            }
            return View(subcentrocostos);
        }

        // GET: subcentrocostos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: subcentrocostos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] subcentrocostos subcentrocostos)
        {
            if (ModelState.IsValid)
            {
                db.subcentrocostos.Add(subcentrocostos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(subcentrocostos);
        }

        // GET: subcentrocostos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            subcentrocostos subcentrocostos = db.subcentrocostos.Find(id);
            if (subcentrocostos == null)
            {
                return HttpNotFound();
            }
            return View(subcentrocostos);
        }

        // POST: subcentrocostos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] subcentrocostos subcentrocostos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subcentrocostos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subcentrocostos);
        }

        // GET: subcentrocostos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            subcentrocostos subcentrocostos = db.subcentrocostos.Find(id);
            if (subcentrocostos == null)
            {
                return HttpNotFound();
            }
            return View(subcentrocostos);
        }

        // POST: subcentrocostos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            subcentrocostos subcentrocostos = db.subcentrocostos.Find(id);
            db.subcentrocostos.Remove(subcentrocostos);
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
