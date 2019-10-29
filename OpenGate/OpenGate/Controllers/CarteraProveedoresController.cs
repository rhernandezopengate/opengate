using OpenGate.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace OpenGate.Controllers
{
    [Authorize]
    public class CarteraProveedoresController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        public ActionResult IndexView()
        {
            return View();
        }

        public ActionResult EditView(int? id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult CreateView()
        {
            return View();
        }

        // GET: CarteraProveedores
        public ActionResult Index()
        {
            return PartialView(db.proveedor.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.Mensaje = "Sin Error";            
            ViewBag.NacionalidadProveedor_Id = new SelectList(db.nacionalidadproveedor, "id", "descripcion");
            ViewBag.CategoriaProveedor_Id = new SelectList(db.categoriaproveedor, "id", "descripcion");
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProveedoresViewModel proveedoresView)
        {
            try
            {
                proveedor proveedores = new proveedor();
                informacionbancaria informacionbancaria = new informacionbancaria();

                if (proveedoresView.Informacionbancaria.Clabe.Length != 18)
                {
                    ViewBag.Mensaje = "La clabe interbancaria debe tener 18 digitos.";
                    ViewBag.NacionalidadProveedor_Id = new SelectList(db.nacionalidadproveedor, "id", "descripcion");
                    ViewBag.categoriaproveedor_id = new SelectList(db.categoriaproveedor, "id", "descripcion");
                    return View(proveedoresView);
                }

                if (proveedoresView.DynamicTextBox == null)
                {
                    ViewBag.Mensaje = "Debe ingresar por lo menos un contacto";
                    ViewBag.NacionalidadProveedor_Id = new SelectList(db.nacionalidadproveedor, "id", "descripcion");
                    ViewBag.categoriaproveedor_id = new SelectList(db.categoriaproveedor, "id", "descripcion");
                    return View(proveedoresView);
                }

                proveedores.Colonia = proveedoresView.Proveedores.Colonia;
                proveedores.RazonSocial = proveedoresView.Proveedores.RazonSocial;
                proveedores.RepresentanteLegal = proveedoresView.Proveedores.RepresentanteLegal;
                proveedores.NombreComercial = proveedoresView.Proveedores.NombreComercial;
                proveedores.RFC = proveedoresView.Proveedores.RFC;
                proveedores.CodigoPostal = proveedoresView.Proveedores.CodigoPostal;
                proveedores.Calle = proveedoresView.Proveedores.Calle;
                proveedores.Municipio = proveedoresView.Proveedores.Municipio;
                proveedores.Estado = proveedoresView.Proveedores.Estado;
                proveedores.Pais = proveedoresView.Proveedores.Pais;
                proveedores.ModenaFacturacion = proveedoresView.Proveedores.ModenaFacturacion;
                proveedores.DiasCredito = proveedoresView.Proveedores.DiasCredito;
                proveedores.ActividadEmpresarial = proveedoresView.Proveedores.ActividadEmpresarial;
                proveedores.StatusProveedorVisible_Id = 1;
                proveedores.StatusProveedor_Id = 1;
                proveedores.CategoriaProveedor_Id = proveedoresView.CategoriaProveedor_Id;
                proveedores.NacionalidadProveedor_Id = proveedoresView.NacionalidadProveedor_Id;

                db.proveedor.Add(proveedores);
                db.SaveChanges();

                if (proveedores.id > 0)
                {
                    informacionbancaria.NombreBanco = proveedoresView.Informacionbancaria.NombreBanco;
                    informacionbancaria.CuentaBancaria = proveedoresView.Informacionbancaria.CuentaBancaria;
                    informacionbancaria.Clabe = proveedoresView.Informacionbancaria.Clabe;
                    informacionbancaria.Proveedores_Id = proveedores.id;
                    db.informacionbancaria.Add(informacionbancaria);
                    db.SaveChanges();
                }

                ////Loop through the dynamic TextBox values.
                foreach (string textboxValue in proveedoresView.DynamicTextBox)
                {
                    contactoproveedor contactoproveedor = new contactoproveedor();

                    //Insert the dynamic TextBox values to Database Table.
                    contactoproveedor.Nombre = textboxValue;

                    foreach (string textboxValue1 in proveedoresView.DynamicTextTelefono)
                    {
                        //Insert the dynamic TextBox values to Database Table.
                        contactoproveedor.Telefono = textboxValue1;
                    }

                    foreach (string textboxValue2 in proveedoresView.DynamicTextMail)
                    {
                        //Insert the dynamic TextBox values to Database Table.
                        contactoproveedor.Mail = textboxValue2;
                    }

                    foreach (string textboxValue3 in proveedoresView.DynamicTextPuesto)
                    {
                        //Insert the dynamic TextBox values to Database Table.
                        contactoproveedor.Puesto = textboxValue3;
                    }

                    contactoproveedor.Proveedores_Id = proveedores.id;

                    db.contactoproveedor.Add(contactoproveedor);
                    db.SaveChanges();

                }                
                return RedirectToAction("IndexView");
            }
            catch (Exception _ex)
            {
                ViewBag.Mensaje = _ex.Message.ToString();
                return View(proveedoresView);
            }        
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            proveedor proveedores = db.proveedor.Find(id);
            if (proveedores == null)
            {
                return HttpNotFound();
            }
            ViewBag.NacionalidadProveedor_Id = new SelectList(db.nacionalidadproveedor, "id", "descripcion", proveedores.NacionalidadProveedor_Id);
            ViewBag.CategoriaProveedor_Id = new SelectList(db.categoriaproveedor, "id", "descripcion", proveedores.CategoriaProveedor_Id);
            ViewBag.StatusProveedor_Id = new SelectList(db.statusproveedor, "id", "descripcion", proveedores.StatusProveedor_Id);
            ViewBag.StatusProveedorVisible_Id = new SelectList(db.statusproveedorvisible, "id", "descripcion", proveedores.StatusProveedorVisible_Id);

            return View(proveedores);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(proveedor proveedor)
        {
            if (ModelState.IsValid)
            {
                if (proveedor.StatusProveedor_Id == 2)
                {
                    var facturas = (from f in db.factura
                                    where f.Proveedor_Id == proveedor.id &&
                                    f.StatusFactura_Id == 1
                                    select f);

                    foreach (var item in facturas)
                    {
                        item.StatusFactura_Id = 4;
                        db.factura.AddOrUpdate(item);
                    }
                }
                else
                {
                    var facturas = (from f in db.factura
                                    where f.Proveedor_Id == proveedor.id &&
                                    f.StatusFactura_Id == 4
                                    select f);

                    foreach (var item in facturas)
                    {
                        item.StatusFactura_Id = 1;
                        db.factura.AddOrUpdate(item);
                    }
                }

                db.Entry(proveedor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexView");
            }
            ViewBag.categoriaproveedor_id = new SelectList(db.categoriaproveedor, "id", "descripcion", proveedor.CategoriaProveedor_Id);
            ViewBag.nacionalidadproveedor_id = new SelectList(db.nacionalidadproveedor, "id", "descripcion", proveedor.NacionalidadProveedor_Id);
            ViewBag.statusproveedor_id = new SelectList(db.statusproveedor, "id", "descripcion", proveedor.StatusProveedor_Id);
            ViewBag.StatusProveedorVisible_Id = new SelectList(db.statusproveedorvisible, "id", "descripcion", proveedor.StatusProveedorVisible_Id);
            return View(proveedor);
        }

        public ActionResult InformacionBancaria(int idProveedor)
        {
            var lista = db.informacionbancaria.Where(x => x.Proveedores_Id == idProveedor).ToList();
            return PartialView("_InformacionBancaria", lista);
        }

        public ActionResult ContactosProveedor(int idProveedor)
        {
            var lista = db.contactoproveedor.Where(x => x.Proveedores_Id == idProveedor).ToList();
            return PartialView("_ContactosProveedor", lista);
        }

        public ActionResult EditarContactosProveedores(int id)
        {
            return PartialView("_EditarContactosProveedor", db.contactoproveedor.Find(id));
        }

        [HttpPost]
        public ActionResult EditarContactosProveedor(int id, string nombreContacto, string telefonoContacto, string mailContacto, string puestoContacto)
        {
            contactoproveedor contactoproveedores = db.contactoproveedor.Find(id);
            contactoproveedores.Nombre = nombreContacto;
            contactoproveedores.Telefono = telefonoContacto;
            contactoproveedores.Mail = mailContacto;
            contactoproveedores.Puesto = puestoContacto;

            db.SaveChanges();

            return Json(new { success = true, message = "Editado Correctamente." + id });
        }

        public ActionResult DetalleProveedor(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var rslt = from f in db.contactoproveedor
                           where f.id == id
                           select new
                           { f.id, f.Nombre, f.Telefono, f.Mail, f.Puesto };

                if (rslt == null)
                {
                    return HttpNotFound();
                }

                return Json(rslt, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult CreateContactoProveedor()
        {
            return PartialView("_CreateContactoProveedor");
        }

        [HttpPost]
        public ActionResult CreateContactoProveedor(int id, string nombre, string telefono, string mail, string puesto)
        {
            contactoproveedor contactoproveedor = new contactoproveedor();

            contactoproveedor.Nombre = nombre;
            contactoproveedor.Telefono = telefono;
            contactoproveedor.Mail = mail;
            contactoproveedor.Puesto = puesto;
            contactoproveedor.Proveedores_Id = id;

            db.contactoproveedor.Add(contactoproveedor);
            db.SaveChanges();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteContactoProveedor(int? id)
        {
            contactoproveedor contactoproveedor = db.contactoproveedor.Find(id);
            db.contactoproveedor.Remove(contactoproveedor);
            db.SaveChanges();            
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditarInformacionBancaria(int id)
        {
            informacionbancaria info = db.informacionbancaria.Find(id);
            return PartialView("_EditarInformacionBancaria", info);
        }

        public ActionResult EditInformacionBancaria(int id, string NombreBanco, string CuentaBancaria, string Clabe)
        {
            informacionbancaria info = db.informacionbancaria.Find(id);
            info.NombreBanco = NombreBanco;
            info.CuentaBancaria = CuentaBancaria;
            info.Clabe = Clabe;

            db.SaveChanges();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReloadInformacionBancaria(int id)
        {
            List<informacionbancaria> lista = db.informacionbancaria.Where(x => x.Proveedores_Id == id).ToList();
            return PartialView("_InformacionBancaria", lista);
        }

        public ActionResult Reload(int id)
        {
            List<contactoproveedor> lista = db.contactoproveedor.Where(x => x.Proveedores_Id == id).ToList();
            return PartialView(lista);
        }
    }
}