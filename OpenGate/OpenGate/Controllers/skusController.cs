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
    public class skusController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: skus
        public ActionResult Index()
        {
            return View(db.skus.ToList());
        }

        // GET: skus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            skus skus = db.skus.Find(id);
            if (skus == null)
            {
                return HttpNotFound();
            }
            return View(skus);
        }

        // GET: skus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: skus/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Sku,Descripcion,uom,codigobarras,codigobidimensional,qtymanual,kit")] skus skus)
        {
            if (ModelState.IsValid)
            {
                db.skus.Add(skus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(skus);
        }

        // GET: skus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            skus skus = db.skus.Find(id);
            if (skus == null)
            {
                return HttpNotFound();
            }
            return View(skus);
        }

        // POST: skus/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Sku,Descripcion,uom,codigobarras,codigobidimensional,qtymanual,kit")] skus skus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(skus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(skus);
        }

        // GET: skus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            skus skus = db.skus.Find(id);
            if (skus == null)
            {
                return HttpNotFound();
            }
            return View(skus);
        }

        // POST: skus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            skus skus = db.skus.Find(id);
            db.skus.Remove(skus);
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
