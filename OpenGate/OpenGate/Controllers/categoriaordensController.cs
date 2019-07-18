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
    public class categoriaordensController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: categoriaordens
        public ActionResult Index()
        {
            return View(db.categoriaorden.ToList());
        }

        // GET: categoriaordens/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            categoriaorden categoriaorden = db.categoriaorden.Find(id);
            if (categoriaorden == null)
            {
                return HttpNotFound();
            }
            return View(categoriaorden);
        }

        // GET: categoriaordens/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: categoriaordens/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] categoriaorden categoriaorden)
        {
            if (ModelState.IsValid)
            {
                db.categoriaorden.Add(categoriaorden);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categoriaorden);
        }

        // GET: categoriaordens/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            categoriaorden categoriaorden = db.categoriaorden.Find(id);
            if (categoriaorden == null)
            {
                return HttpNotFound();
            }
            return View(categoriaorden);
        }

        // POST: categoriaordens/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] categoriaorden categoriaorden)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categoriaorden).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categoriaorden);
        }

        // GET: categoriaordens/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            categoriaorden categoriaorden = db.categoriaorden.Find(id);
            if (categoriaorden == null)
            {
                return HttpNotFound();
            }
            return View(categoriaorden);
        }

        // POST: categoriaordens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            categoriaorden categoriaorden = db.categoriaorden.Find(id);
            db.categoriaorden.Remove(categoriaorden);
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
