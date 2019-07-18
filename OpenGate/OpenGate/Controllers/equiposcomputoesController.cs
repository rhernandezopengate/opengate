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
    public class equiposcomputoesController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: equiposcomputoes
        public ActionResult Index()
        {
            var equiposcomputo = db.equiposcomputo.Include(e => e.empleado).Include(e => e.equipomantenimiento);            
            return View(equiposcomputo.ToList());
        }

        // GET: equiposcomputoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            equiposcomputo equiposcomputo = db.equiposcomputo.Find(id);
            if (equiposcomputo == null)
            {
                return HttpNotFound();
            }
            return View(equiposcomputo);
        }

        // GET: equiposcomputoes/Create
        public ActionResult Create()
        {
            ViewBag.Empleado_Id = new SelectList(db.empleado, "id", "Nombre");
            ViewBag.EquipoMantenimiento_Id = new SelectList(db.equipomantenimiento, "id", "descripcion");
            return View();
        }

        // POST: equiposcomputoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,CPU,RAM,SistemaOperativo,NombreEquipo,Modelo,Marca,TipoEquipo,Ubicacion,ServiceTag,ExpressServiceCode,FechaEntrada,FechaBaja,Empleado_Id,EquipoMantenimiento_Id,HDD")] equiposcomputo equiposcomputo)
        {
            if (ModelState.IsValid)
            {
                db.equiposcomputo.Add(equiposcomputo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Empleado_Id = new SelectList(db.empleado, "id", "Nombre", equiposcomputo.Empleado_Id);
            ViewBag.EquipoMantenimiento_Id = new SelectList(db.equipomantenimiento, "id", "descripcion", equiposcomputo.EquipoMantenimiento_Id);
            return View(equiposcomputo);
        }

        // GET: equiposcomputoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            equiposcomputo equiposcomputo = db.equiposcomputo.Find(id);
            if (equiposcomputo == null)
            {
                return HttpNotFound();
            }
            ViewBag.Empleado_Id = new SelectList(db.empleado, "id", "Nombre", equiposcomputo.Empleado_Id);
            ViewBag.EquipoMantenimiento_Id = new SelectList(db.equipomantenimiento, "id", "descripcion", equiposcomputo.EquipoMantenimiento_Id);
            return View(equiposcomputo);
        }

        // POST: equiposcomputoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,CPU,RAM,SistemaOperativo,NombreEquipo,Modelo,Marca,TipoEquipo,Ubicacion,ServiceTag,ExpressServiceCode,FechaEntrada,FechaBaja,Empleado_Id,EquipoMantenimiento_Id,HDD")] equiposcomputo equiposcomputo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equiposcomputo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Empleado_Id = new SelectList(db.empleado, "id", "Nombre", equiposcomputo.Empleado_Id);
            ViewBag.EquipoMantenimiento_Id = new SelectList(db.equipomantenimiento, "id", "descripcion", equiposcomputo.EquipoMantenimiento_Id);
            return View(equiposcomputo);
        }

        // GET: equiposcomputoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            equiposcomputo equiposcomputo = db.equiposcomputo.Find(id);
            if (equiposcomputo == null)
            {
                return HttpNotFound();
            }
            return View(equiposcomputo);
        }

        // POST: equiposcomputoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            equiposcomputo equiposcomputo = db.equiposcomputo.Find(id);
            db.equiposcomputo.Remove(equiposcomputo);
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
