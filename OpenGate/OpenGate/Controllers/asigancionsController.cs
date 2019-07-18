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
    public class asigancionsController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: asigancions
        public ActionResult Index()
        {
            var asigancion = db.asigancion.Include(a => a.AspNetUsers);
            return View(asigancion.ToList());
        }

        // GET: asigancions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asigancion asigancion = db.asigancion.Find(id);
            if (asigancion == null)
            {
                return HttpNotFound();
            }
            return View(asigancion);
        }

        // GET: asigancions/Create
        public ActionResult Create()
        {
            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: asigancions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Folio,FechaRegistro,AspNetUsers_Id")] asigancion asigancion)
        {
            if (ModelState.IsValid)
            {
                db.asigancion.Add(asigancion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email", asigancion.AspNetUsers_Id);
            return View(asigancion);
        }

        // GET: asigancions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asigancion asigancion = db.asigancion.Find(id);
            if (asigancion == null)
            {
                return HttpNotFound();
            }
            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email", asigancion.AspNetUsers_Id);
            return View(asigancion);
        }

        // POST: asigancions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Folio,FechaRegistro,AspNetUsers_Id")] asigancion asigancion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asigancion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email", asigancion.AspNetUsers_Id);
            return View(asigancion);
        }

        // GET: asigancions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            asigancion asigancion = db.asigancion.Find(id);
            if (asigancion == null)
            {
                return HttpNotFound();
            }
            return View(asigancion);
        }

        // POST: asigancions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            asigancion asigancion = db.asigancion.Find(id);
            db.asigancion.Remove(asigancion);
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
