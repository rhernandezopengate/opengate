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
    public class statustareasController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: statustareas
        public ActionResult Index()
        {
            return View(db.statustarea.ToList());
        }

        // GET: statustareas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statustarea statustarea = db.statustarea.Find(id);
            if (statustarea == null)
            {
                return HttpNotFound();
            }
            return View(statustarea);
        }

        // GET: statustareas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: statustareas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] statustarea statustarea)
        {
            if (ModelState.IsValid)
            {
                db.statustarea.Add(statustarea);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statustarea);
        }

        // GET: statustareas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statustarea statustarea = db.statustarea.Find(id);
            if (statustarea == null)
            {
                return HttpNotFound();
            }
            return View(statustarea);
        }

        // POST: statustareas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] statustarea statustarea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statustarea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statustarea);
        }

        // GET: statustareas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statustarea statustarea = db.statustarea.Find(id);
            if (statustarea == null)
            {
                return HttpNotFound();
            }
            return View(statustarea);
        }

        // POST: statustareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            statustarea statustarea = db.statustarea.Find(id);
            db.statustarea.Remove(statustarea);
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
