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
    public class bancosController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        [HttpPost]
        public JsonResult ddlBancos()
        {
            List<SelectListItem> ListaBancos = new List<SelectListItem>();

            foreach (var item in db.bancos.ToList())
            {
                ListaBancos.Add(new SelectListItem
                {
                    Value = item.id.ToString(),
                    Text = item.nombre + " " +item.rfc
                });
            }

            return Json(ListaBancos);
        }

        // GET: bancos
        public ActionResult Index()
        {
            return View(db.bancos.ToList());
        }

        // GET: bancos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bancos bancos = db.bancos.Find(id);
            if (bancos == null)
            {
                return HttpNotFound();
            }
            return View(bancos);
        }

        // GET: bancos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: bancos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,nombre,rfc")] bancos bancos)
        {
            if (ModelState.IsValid)
            {
                db.bancos.Add(bancos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bancos);
        }

        // GET: bancos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bancos bancos = db.bancos.Find(id);
            if (bancos == null)
            {
                return HttpNotFound();
            }
            return View(bancos);
        }

        // POST: bancos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nombre,rfc")] bancos bancos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bancos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bancos);
        }

        // GET: bancos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bancos bancos = db.bancos.Find(id);
            if (bancos == null)
            {
                return HttpNotFound();
            }
            return View(bancos);
        }

        // POST: bancos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            bancos bancos = db.bancos.Find(id);
            db.bancos.Remove(bancos);
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
