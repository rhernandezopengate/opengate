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
    public class statuspagoesController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: statuspagoes
        public ActionResult Index()
        {
            return View(db.statuspago.ToList());
        }

        // GET: statuspagoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statuspago statuspago = db.statuspago.Find(id);
            if (statuspago == null)
            {
                return HttpNotFound();
            }
            return View(statuspago);
        }

        // GET: statuspagoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: statuspagoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] statuspago statuspago)
        {
            if (ModelState.IsValid)
            {
                db.statuspago.Add(statuspago);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statuspago);
        }

        // GET: statuspagoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statuspago statuspago = db.statuspago.Find(id);
            if (statuspago == null)
            {
                return HttpNotFound();
            }
            return View(statuspago);
        }

        // POST: statuspagoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] statuspago statuspago)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statuspago).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statuspago);
        }

        // GET: statuspagoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statuspago statuspago = db.statuspago.Find(id);
            if (statuspago == null)
            {
                return HttpNotFound();
            }
            return View(statuspago);
        }

        // POST: statuspagoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            statuspago statuspago = db.statuspago.Find(id);
            db.statuspago.Remove(statuspago);
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
