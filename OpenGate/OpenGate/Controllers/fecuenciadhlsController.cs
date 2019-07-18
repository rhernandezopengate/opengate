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
    public class fecuenciadhlsController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: fecuenciadhls
        public ActionResult Index()
        {
            return View(db.fecuenciadhl.ToList());
        }

        // GET: fecuenciadhls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fecuenciadhl fecuenciadhl = db.fecuenciadhl.Find(id);
            if (fecuenciadhl == null)
            {
                return HttpNotFound();
            }
            return View(fecuenciadhl);
        }

        // GET: fecuenciadhls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: fecuenciadhls/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,carrier,postalcode,freightcode,frequency,wharehouse,statusfq,descripcion")] fecuenciadhl fecuenciadhl)
        {
            if (ModelState.IsValid)
            {
                db.fecuenciadhl.Add(fecuenciadhl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(fecuenciadhl);
        }

        // GET: fecuenciadhls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fecuenciadhl fecuenciadhl = db.fecuenciadhl.Find(id);
            if (fecuenciadhl == null)
            {
                return HttpNotFound();
            }
            return View(fecuenciadhl);
        }

        // POST: fecuenciadhls/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,carrier,postalcode,freightcode,frequency,wharehouse,statusfq,descripcion")] fecuenciadhl fecuenciadhl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fecuenciadhl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fecuenciadhl);
        }

        // GET: fecuenciadhls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fecuenciadhl fecuenciadhl = db.fecuenciadhl.Find(id);
            if (fecuenciadhl == null)
            {
                return HttpNotFound();
            }
            return View(fecuenciadhl);
        }

        // POST: fecuenciadhls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            fecuenciadhl fecuenciadhl = db.fecuenciadhl.Find(id);
            db.fecuenciadhl.Remove(fecuenciadhl);
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
