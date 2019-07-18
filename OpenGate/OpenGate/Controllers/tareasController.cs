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
    public class tareasController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();


        // GET: tareas
        [Authorize(Roles = "admin, finanzas")]
        public ActionResult Index()
        {
            ViewBag.Controller = "Tareas";
            var tareas = db.tareas.Include(t => t.AspNetUsers).Include(t => t.statustarea);

            int contador = 1;
            foreach (var item in tareas)
            {
                item.folio = contador++;
            }

            return View(tareas.ToList());
        }

        // GET: tareas/Details/5
        [Authorize(Roles = "admin, finanzas")]
        public ActionResult Details(int? id)
        {
            ViewBag.Controller = "Tareas";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tareas tareas = db.tareas.Find(id);
            if (tareas == null)
            {
                return HttpNotFound();
            }
            return View(tareas);
        }

        // GET: tareas/Create
        [Authorize(Roles = "admin, finanzas")]
        public ActionResult Create()
        {
            ViewBag.Controller = "Tareas";
            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.StatusTarea_Id = new SelectList(db.statustarea, "id", "descripcion");
            return View();
        }

        // POST: tareas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, finanzas")]
        public ActionResult Create([Bind(Include = "id,Actividad,HoraInicio,Solicita,Fecha,FechaEntrega,HoraFin,PorcentajeCumplido,Observaciones,Notas,FechaRegistro,StatusTarea_Id,AspNetUsers_Id")] tareas tareas)
        {
            if (ModelState.IsValid)
            {
                tareas.FechaRegistro = DateTime.Now;
                db.tareas.Add(tareas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email", tareas.AspNetUsers_Id);
            ViewBag.StatusTarea_Id = new SelectList(db.statustarea, "id", "descripcion", tareas.StatusTarea_Id);
            return View(tareas);
        }

        // GET: tareas/Edit/5
        [Authorize(Roles = "admin, finanzas")]
        public ActionResult Edit(int? id)
        {
            ViewBag.Controller = "Tareas";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tareas tareas = db.tareas.Find(id);
            if (tareas == null)
            {
                return HttpNotFound();
            }
            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email", tareas.AspNetUsers_Id);
            ViewBag.StatusTarea_Id = new SelectList(db.statustarea, "id", "descripcion", tareas.StatusTarea_Id);
            return View(tareas);
        }

        // POST: tareas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, finanzas")]
        public ActionResult Edit([Bind(Include = "id,Actividad,HoraInicio,Solicita,Fecha,FechaEntrega,HoraFin,PorcentajeCumplido,Observaciones,Notas,FechaRegistro,StatusTarea_Id,AspNetUsers_Id")] tareas tareas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tareas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email", tareas.AspNetUsers_Id);
            ViewBag.StatusTarea_Id = new SelectList(db.statustarea, "id", "descripcion", tareas.StatusTarea_Id);
            return View(tareas);
        }

        // GET: tareas/Delete/5
        [Authorize(Roles = "admin, finanzas")]
        public ActionResult Delete(int? id)
        {
            ViewBag.Controller = "Tareas";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tareas tareas = db.tareas.Find(id);
            if (tareas == null)
            {
                return HttpNotFound();
            }
            return View(tareas);
        }

        // POST: tareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, finanzas")]
        public ActionResult DeleteConfirmed(int id)
        {
            tareas tareas = db.tareas.Find(id);
            db.tareas.Remove(tareas);
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
