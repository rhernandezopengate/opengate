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
    public class equipomantenimientoesController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: equipomantenimientoes
        public ActionResult Index()
        {
            return View(db.equipomantenimiento.ToList());
        }

        // GET: equipomantenimientoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            equipomantenimiento equipomantenimiento = db.equipomantenimiento.Find(id);
            if (equipomantenimiento == null)
            {
                return HttpNotFound();
            }
            return View(equipomantenimiento);
        }

        // GET: equipomantenimientoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: equipomantenimientoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] equipomantenimiento equipomantenimiento)
        {
            if (ModelState.IsValid)
            {
                db.equipomantenimiento.Add(equipomantenimiento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(equipomantenimiento);
        }

        // GET: equipomantenimientoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            equipomantenimiento equipomantenimiento = db.equipomantenimiento.Find(id);
            if (equipomantenimiento == null)
            {
                return HttpNotFound();
            }
            return View(equipomantenimiento);
        }

        // POST: equipomantenimientoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] equipomantenimiento equipomantenimiento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipomantenimiento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(equipomantenimiento);
        }

        // GET: equipomantenimientoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            equipomantenimiento equipomantenimiento = db.equipomantenimiento.Find(id);
            if (equipomantenimiento == null)
            {
                return HttpNotFound();
            }
            return View(equipomantenimiento);
        }

        // POST: equipomantenimientoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            equipomantenimiento equipomantenimiento = db.equipomantenimiento.Find(id);
            db.equipomantenimiento.Remove(equipomantenimiento);
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
