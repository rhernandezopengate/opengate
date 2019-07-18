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
    public class cuentaordenantesController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        [HttpPost]
        public JsonResult ddlCuentaOrdenante()
        {
            List<SelectListItem> listacuenta = new List<SelectListItem>();

            foreach (var item in db.cuentaordenante.ToList())
            {
                listacuenta.Add(new SelectListItem
                {
                    Value = item.id.ToString(),
                    Text = "Cuenta: " + item.descripcion
                });
            }

            return Json(listacuenta);
        }

        // GET: cuentaordenantes
        public ActionResult Index()
        {
            return View(db.cuentaordenante.ToList());
        }

        // GET: cuentaordenantes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cuentaordenante cuentaordenante = db.cuentaordenante.Find(id);
            if (cuentaordenante == null)
            {
                return HttpNotFound();
            }
            return View(cuentaordenante);
        }

        // GET: cuentaordenantes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: cuentaordenantes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,descripcion")] cuentaordenante cuentaordenante)
        {
            if (ModelState.IsValid)
            {
                db.cuentaordenante.Add(cuentaordenante);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cuentaordenante);
        }

        // GET: cuentaordenantes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cuentaordenante cuentaordenante = db.cuentaordenante.Find(id);
            if (cuentaordenante == null)
            {
                return HttpNotFound();
            }
            return View(cuentaordenante);
        }

        // POST: cuentaordenantes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,descripcion")] cuentaordenante cuentaordenante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuentaordenante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cuentaordenante);
        }

        // GET: cuentaordenantes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cuentaordenante cuentaordenante = db.cuentaordenante.Find(id);
            if (cuentaordenante == null)
            {
                return HttpNotFound();
            }
            return View(cuentaordenante);
        }

        // POST: cuentaordenantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            cuentaordenante cuentaordenante = db.cuentaordenante.Find(id);
            db.cuentaordenante.Remove(cuentaordenante);
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
