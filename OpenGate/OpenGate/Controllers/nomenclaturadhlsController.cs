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
    public class nomenclaturadhlsController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: nomenclaturadhls
        public ActionResult Index()
        {
            return View(db.nomenclaturadhl.ToList());
        }

        // GET: nomenclaturadhls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nomenclaturadhl nomenclaturadhl = db.nomenclaturadhl.Find(id);
            if (nomenclaturadhl == null)
            {
                return HttpNotFound();
            }
            return View(nomenclaturadhl);
        }

        // GET: nomenclaturadhls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: nomenclaturadhls/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,tipocheckpoint,codigo,descripcion,statusherbalife,observaciones")] nomenclaturadhl nomenclaturadhl)
        {
            if (ModelState.IsValid)
            {
                db.nomenclaturadhl.Add(nomenclaturadhl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nomenclaturadhl);
        }

        // GET: nomenclaturadhls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nomenclaturadhl nomenclaturadhl = db.nomenclaturadhl.Find(id);
            if (nomenclaturadhl == null)
            {
                return HttpNotFound();
            }
            return View(nomenclaturadhl);
        }

        // POST: nomenclaturadhls/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,tipocheckpoint,codigo,descripcion,statusherbalife,observaciones")] nomenclaturadhl nomenclaturadhl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nomenclaturadhl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nomenclaturadhl);
        }

        // GET: nomenclaturadhls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nomenclaturadhl nomenclaturadhl = db.nomenclaturadhl.Find(id);
            if (nomenclaturadhl == null)
            {
                return HttpNotFound();
            }
            return View(nomenclaturadhl);
        }

        // POST: nomenclaturadhls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            nomenclaturadhl nomenclaturadhl = db.nomenclaturadhl.Find(id);
            db.nomenclaturadhl.Remove(nomenclaturadhl);
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
