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
    public class formapagoesController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: formapagoes
        public ActionResult Index()
        {
            return View(db.formapago.ToList());
        }

        // GET: formapagoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            formapago formapago = db.formapago.Find(id);
            if (formapago == null)
            {
                return HttpNotFound();
            }
            return View(formapago);
        }

        // GET: formapagoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: formapagoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] formapago formapago)
        {
            if (ModelState.IsValid)
            {
                db.formapago.Add(formapago);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(formapago);
        }

        // GET: formapagoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            formapago formapago = db.formapago.Find(id);
            if (formapago == null)
            {
                return HttpNotFound();
            }
            return View(formapago);
        }

        // POST: formapagoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] formapago formapago)
        {
            if (ModelState.IsValid)
            {
                db.Entry(formapago).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(formapago);
        }

        // GET: formapagoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            formapago formapago = db.formapago.Find(id);
            if (formapago == null)
            {
                return HttpNotFound();
            }
            return View(formapago);
        }

        // POST: formapagoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            formapago formapago = db.formapago.Find(id);
            db.formapago.Remove(formapago);
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
