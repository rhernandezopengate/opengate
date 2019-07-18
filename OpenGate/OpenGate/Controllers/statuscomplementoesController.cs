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
    public class statuscomplementoesController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();


        // GET: statuscomplementoes
        public ActionResult Index()
        {
            return View(db.statuscomplemento.ToList());
        }

        // GET: statuscomplementoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statuscomplemento statuscomplemento = db.statuscomplemento.Find(id);
            if (statuscomplemento == null)
            {
                return HttpNotFound();
            }
            return View(statuscomplemento);
        }

        // GET: statuscomplementoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: statuscomplementoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] statuscomplemento statuscomplemento)
        {
            if (ModelState.IsValid)
            {
                db.statuscomplemento.Add(statuscomplemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(statuscomplemento);
        }

        // GET: statuscomplementoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statuscomplemento statuscomplemento = db.statuscomplemento.Find(id);
            if (statuscomplemento == null)
            {
                return HttpNotFound();
            }
            return View(statuscomplemento);
        }

        // POST: statuscomplementoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] statuscomplemento statuscomplemento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statuscomplemento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(statuscomplemento);
        }

        // GET: statuscomplementoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            statuscomplemento statuscomplemento = db.statuscomplemento.Find(id);
            if (statuscomplemento == null)
            {
                return HttpNotFound();
            }
            return View(statuscomplemento);
        }

        // POST: statuscomplementoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            statuscomplemento statuscomplemento = db.statuscomplemento.Find(id);
            db.statuscomplemento.Remove(statuscomplemento);
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
