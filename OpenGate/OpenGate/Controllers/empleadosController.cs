using Microsoft.AspNet.Identity;
using OpenGate.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenGate.Controllers
{
    public class empleadosController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: empleados
        public ActionResult Index()
        {
            return View(db.empleado.ToList());
        }

        public ActionResult Details(int id)
        {
            return View(db.empleado.Where(x => x.id==id).FirstOrDefault());
        }

        public ActionResult Delete(int id)
        {
            return View(db.empleado.Where(x => x.id == id).FirstOrDefault());
        }

        public ActionResult Edit(int id)
        {
            empleado empleado = db.empleado.Where(x => x.id == id).FirstOrDefault();
            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email", empleado.AspNetUsers_Id);
            ViewBag.Area_Id = new SelectList(db.area, "id", "descripcion", empleado.Area_Id);
            ViewBag.Puesto_Id = new SelectList(db.puesto, "id", "descripcion", empleado.Puesto_Id);
            return View(empleado);
        }

        public ActionResult Create()
        {
            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.AreaId = new SelectList(db.area, "id", "descripcion");
            ViewBag.PuestoId = new SelectList(db.puesto, "id", "descripcion");
            return View();
        }

        [HttpPost]
        public ActionResult Create(string nombre, string ap, string am, DateTime fhIngreso, string direccion, int PuestoId, int AreaId, HttpPostedFileBase postedFile, string AspNetUsers_Id, string celular, string telefono)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }

            empleado empleado = new empleado();
            empleado.NameFile = Path.GetFileName(postedFile.FileName);
            empleado.ExtFile = postedFile.ContentType;
            empleado.Nombre = nombre;
            empleado.ApellidoPaterno = ap;
            empleado.ApellidoMaterno = am;
            empleado.FechaIngreso = fhIngreso;
            empleado.Direccion = direccion;
            empleado.Puesto_Id = PuestoId;
            empleado.Area_Id = AreaId;
            empleado.foto = bytes;
            empleado.AspNetUsers_Id = AspNetUsers_Id;
            empleado.Celular = celular;
            empleado.Telefono = telefono;

            db.empleado.Add(empleado);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(string Nombre, string ApellidoPaterno, string ApellidoMaterno, DateTime FechaIngreso, string Direccion, int Puesto_Id, int Area_Id, HttpPostedFileBase postedFile, string AspNetUsers_Id, int id, string Celular, string Telefono)
        {
            empleado empleado = db.empleado.Where(x => x.id == id).FirstOrDefault();
            
            empleado.Nombre = Nombre;
            empleado.ApellidoPaterno = ApellidoPaterno;
            empleado.ApellidoMaterno = ApellidoMaterno;
            empleado.FechaIngreso = FechaIngreso;
            empleado.Direccion = Direccion;
            empleado.Puesto_Id = Puesto_Id;
            empleado.Area_Id = Area_Id;
            empleado.Telefono = Telefono;
            empleado.Celular = Celular;            
            empleado.AspNetUsers_Id = AspNetUsers_Id;

            if (postedFile != null)
            {
                byte[] bytes;
                using (BinaryReader br = new BinaryReader(postedFile.InputStream))
                {
                    bytes = br.ReadBytes(postedFile.ContentLength);
                }
                empleado.foto = bytes;
                empleado.NameFile = Path.GetFileName(postedFile.FileName);
                empleado.ExtFile = postedFile.ContentType;
            }                 
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            empleado empleado = db.empleado.Where(x => x.id == id).FirstOrDefault();
            db.empleado.Remove(empleado);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public FileResult DownloadFile(int? fileId)
        {
            byte[] bytes;
            string fileName, contentType;           

            empleado empleado = db.empleado.Where(x => x.id == fileId).FirstOrDefault();

            bytes = empleado.foto;
            contentType = empleado.ExtFile;
            fileName = empleado.NameFile;

            return File(bytes, contentType, fileName);
        }

        public ActionResult ImagenEmpleado(string id)
        {
            empleado empleado = db.empleado.Where(x => x.AspNetUsers_Id == id).FirstOrDefault();
            byte[] imageData = empleado.foto;
            if (imageData != null && imageData.Length > 0)
            {
                return new FileStreamResult(new System.IO.MemoryStream(imageData), empleado.ExtFile);
            }
            else
            {
                return null;
            } 
        }

        public ActionResult Imagen()
        {
            string user = User.Identity.GetUserId();
            empleado empleado = db.empleado.Where(x => x.AspNetUsers_Id == user).FirstOrDefault();
            byte[] imageData = empleado.foto;
            if (imageData != null && imageData.Length > 0)
            {
                return new FileStreamResult(new System.IO.MemoryStream(imageData), empleado.ExtFile);
            }
            else
            {
                return null;
            }
        }
    }
}