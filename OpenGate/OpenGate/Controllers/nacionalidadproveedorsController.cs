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
    public class nacionalidadproveedorsController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: nacionalidadproveedors
        public ActionResult Index()
        {
            return View(db.nacionalidadproveedor.ToList());
        }

        // GET: nacionalidadproveedors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nacionalidadproveedor nacionalidadproveedor = db.nacionalidadproveedor.Find(id);
            if (nacionalidadproveedor == null)
            {
                return HttpNotFound();
            }
            return View(nacionalidadproveedor);
        }

        // GET: nacionalidadproveedors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: nacionalidadproveedors/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] nacionalidadproveedor nacionalidadproveedor)
        {
            if (ModelState.IsValid)
            {
                db.nacionalidadproveedor.Add(nacionalidadproveedor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nacionalidadproveedor);
        }

        // GET: nacionalidadproveedors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nacionalidadproveedor nacionalidadproveedor = db.nacionalidadproveedor.Find(id);
            if (nacionalidadproveedor == null)
            {
                return HttpNotFound();
            }
            return View(nacionalidadproveedor);
        }

        // POST: nacionalidadproveedors/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] nacionalidadproveedor nacionalidadproveedor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nacionalidadproveedor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nacionalidadproveedor);
        }

        // GET: nacionalidadproveedors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nacionalidadproveedor nacionalidadproveedor = db.nacionalidadproveedor.Find(id);
            if (nacionalidadproveedor == null)
            {
                return HttpNotFound();
            }
            return View(nacionalidadproveedor);
        }

        // POST: nacionalidadproveedors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            nacionalidadproveedor nacionalidadproveedor = db.nacionalidadproveedor.Find(id);
            db.nacionalidadproveedor.Remove(nacionalidadproveedor);
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
