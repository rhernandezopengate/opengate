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
    public class solicitantesController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: solicitantes
        public ActionResult Index()
        {
            var solicitante = db.solicitante.Include(s => s.AspNetUsers).Include(s => s.supervisor);
            return View(solicitante.ToList());
        }

        // GET: solicitantes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            solicitante solicitante = db.solicitante.Find(id);
            if (solicitante == null)
            {
                return HttpNotFound();
            }
            return View(solicitante);
        }

        // GET: solicitantes/Create
        public ActionResult Create()
        {
            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers.OrderBy(x => x.Email), "Id", "Email");
            ViewBag.Supervisor_Id = new SelectList(db.supervisor, "id", "Nombre");
            return View();
        }

        // POST: solicitantes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Nombre,AspNetUsers_Id,Supervisor_Id")] solicitante solicitante)
        {
            if (ModelState.IsValid)
            {
                db.solicitante.Add(solicitante);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email", solicitante.AspNetUsers_Id);
            ViewBag.Supervisor_Id = new SelectList(db.supervisor, "id", "Nombre", solicitante.Supervisor_Id);
            return View(solicitante);
        }

        // GET: solicitantes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            solicitante solicitante = db.solicitante.Find(id);
            if (solicitante == null)
            {
                return HttpNotFound();
            }
            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email", solicitante.AspNetUsers_Id);
            ViewBag.Supervisor_Id = new SelectList(db.supervisor, "id", "Nombre", solicitante.Supervisor_Id);
            return View(solicitante);
        }

        // POST: solicitantes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Nombre,AspNetUsers_Id,Supervisor_Id")] solicitante solicitante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(solicitante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email", solicitante.AspNetUsers_Id);
            ViewBag.Supervisor_Id = new SelectList(db.supervisor, "id", "Nombre", solicitante.Supervisor_Id);
            return View(solicitante);
        }

        // GET: solicitantes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            solicitante solicitante = db.solicitante.Find(id);
            if (solicitante == null)
            {
                return HttpNotFound();
            }
            return View(solicitante);
        }

        // POST: solicitantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            solicitante solicitante = db.solicitante.Find(id);
            db.solicitante.Remove(solicitante);
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
