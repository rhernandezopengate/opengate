using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OpenGate.Entidades;
using System.Linq.Dynamic;
using System.Threading;
using System.Net.Mail;
using System.Text;
using System.Net.Mime;

namespace OpenGate.Controllers
{
    [Authorize]
    public class pagoesController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        
        public ActionResult VistaPagos()
        {
            try
            {                
                List<pago> listaPagos = db.pago.OrderByDescending(x => x.id).ToList();
                return View(listaPagos);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult Detalle(int idPago)
        {
            try
            {
                var consultapagofacturas = db.pagofacturas.Where(x => x.Pago_Id == idPago).ToList();
                var listapagofacturas = new List<pagofacturas>();

                foreach (var item in consultapagofacturas)
                {
                    pagofacturas pagofacturas = new pagofacturas();
                    pagofacturas.factura = item.factura;
                    pagofacturas.pago = item.pago;
                    listapagofacturas.Add(pagofacturas);
                }

                var facturasagrupadas = from pf in listapagofacturas
                                        group pf by pf.factura.proveedor.RazonSocial into g
                                        select new Group<string, pagofacturas> { Key = g.Key, Values = g, TotalPagado = g.Sum(x => x.factura.Total)};
                      
                return PartialView(facturasagrupadas.ToList());
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult EnviarCorreo()
        {
            try
            {
                //Mensaje HTML
                string mailmensaje = "<table border='0' cellpadding='0' cellspacing='0' width='100%'>";
                mailmensaje += "<tr>";
                mailmensaje += "<td style='padding: 10px 0 30px 0;'>";
                mailmensaje += "<table align='center' border='0' cellpadding='0' cellspacing='0' width='600' style='border: 1px solid #cccccc; border-collapse: collapse;'>";
                mailmensaje += "<tr>";
                mailmensaje += "<td align='center' style='padding: 40px 0 30px 0; color: #153643; font-size: 28px; font-weight: bold; font-family: Arial, sans-serif;'>";
                mailmensaje += "OPEN GATE LOGISTICS";
                mailmensaje += "</td>";
                mailmensaje += "</tr>";
                mailmensaje += "<tr>";
                mailmensaje += "<td bgcolor='#ffffff' style='padding: 40px 30px 40px 30px;'>";
                mailmensaje += "<table border='0' cellpadding='0' cellspacing='0' width='100%'>";
                mailmensaje += "<tr>";
                mailmensaje += "<td style='color: #153643; font-family: Arial, sans-serif; font-size: 24px;'>";
                mailmensaje += "<b>Pago a proveedores</b>";
                mailmensaje += "</td>";
                mailmensaje += "</tr>";
                mailmensaje += "<tr>";
                mailmensaje += "<td style='padding: 20px 0 30px 0; color: #153643; font-family: Arial, sans-serif; font-size: 16px; line-height: 20px;'>";
                mailmensaje += "Se ha generado un nuevo pago a proveedores por favor ingrese a la aplicacion para gestionar la informacion.";
                mailmensaje += "<br /><br />";
                mailmensaje += "<a href='http://rhernandezog-001-site2.etempurl.com/'>Ingrese aqui</a>";
                mailmensaje += "</td>";
                mailmensaje += "</tr>";
                mailmensaje += "</table>";
                mailmensaje += "</td>";
                mailmensaje += "</tr>";
                mailmensaje += "<tr>";
                mailmensaje += "<td bgcolor='#BB3619' style='padding: 30px 30px 30px 30px;'>";
                mailmensaje += "<table border='0' cellpadding='0' cellspacing='0' width='100%'>";
                mailmensaje += "<tr>";
                mailmensaje += "<td style='color: #ffffff; font-family: Arial, sans-serif; font-size: 14px;' width='75%'>";
                mailmensaje += "&reg; Open Gate Logistics, 2019<br/>	";
                mailmensaje += "</td>	";
                mailmensaje += "</tr>";
                mailmensaje += "</table>";
                mailmensaje += "</td>";
                mailmensaje += "</tr>";
                mailmensaje += "</table>";
                mailmensaje += "</td>";
                mailmensaje += "</tr>";
                mailmensaje += "</table>";

                //Crear el mensaje
                MailMessage mail = new MailMessage();

                //Establecer las direcciones 
                mail.From = new MailAddress("rhernandez@opengatelogistics.com"); //IMPORTANTE: Establecer la direccion que envia los correos.
                //Direcciones con copia 
                mail.To.Add("rviale@opengatelogistics.com");
                mail.CC.Add("auxtrafico@opengatelogistics.com");
                mail.CC.Add("ajuarez@opengatelogistics.com");
                mail.CC.Add("rmedina@opengatelogistics.com");
                mail.CC.Add("gjasso@opengatelogistics.com");
                mail.CC.Add("crodriguez@opengatelogistics.com");                

                //Establecer el contenido del mensaje
                mail.Subject = "Pago a proveedores";
                mail.Body = mailmensaje;
                mail.IsBodyHtml = true;
                //Enviar el mensaje
                SmtpClient smtp = new SmtpClient("smtp.office365.com");
                smtp.UseDefaultCredentials = false;
                //IMPORANTE: Esta es la configuracion SMTP. Se debe loguear la direccion de donde se envia el correo 
                NetworkCredential Credentials = new NetworkCredential("rhernandez@opengatelogistics.com", "L3Qq$J]r3");
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = Credentials;
                smtp.Send(mail);

                return View();
            }
            catch(Exception _ex)
            {
                Console.Write(_ex.Message.ToString());
                return null;
            }            
        }
        
        // GET: pagoes
        public ActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerPagos()
        {
            List<pago> listaPagos = new List<pago>();

            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var numero = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            var pagos = from p in db.pago
                        where p.Numero.ToString().Contains(numero)
                        select new { p.id, p.Numero, status = p.statuspago.descripcion, p.FechaPago, p.MontoTotal };

            foreach (var item in pagos)
            {
                pago pago = new pago();
                pago.id = item.id;
                pago.Numero = item.Numero;
                pago.statusString = item.status;
                pago.FechaPago = item.FechaPago;               
                decimal? value = item.MontoTotal;
                pago.TotalString = String.Format("{0:C}", value);

                listaPagos.Add(pago);
            }

            if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
            {
                listaPagos = listaPagos.OrderBy(SortColumn + " " + SortColumnDir).ToList();
            }

            TotalRecords = listaPagos.ToList().Count();
            var NewItems = listaPagos.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

            return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VistaDetalles(int? id)
        {
            try
            {
                pago pagos = db.pago.Find(id);
                ViewBag.numero = pagos.Numero;
                ViewBag.fechapago = pagos.FechaPago.Value.ToShortDateString();
                ViewBag.montototal = pagos.MontoTotal;
                ViewBag.descripcion = pagos.statuspago.descripcion;

                List<pagofacturas> lista = (from fp in db.pagofacturas
                            where fp.Pago_Id == id
                            join f in db.factura on fp.Factura_Id equals f.id
                            select fp).ToList();

                return PartialView("_VistaDetalles", lista);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }            
        }

        public ActionResult PagoFacturas()
        {
            var facturas = db.factura.Where(x => x.StatusFactura_Id == 1).ToList<factura>();

            foreach (var item in facturas)
            {
                DateTime fechaInicio = item.FechaVencimiento.Value.Date;
                DateTime fechaFinal = DateTime.Now.Date;
                TimeSpan tSpan = fechaFinal - fechaInicio;

                item.dv = tSpan.Days;
            }

            facturasCE cE = new facturasCE();

            cE.FacturasPorPagar = facturas.ToList();

            return View(cE);
        }

        [HttpPost]
        [Authorize(Roles = "admin, finanzas, contabilidad")]
        public ActionResult PagoFacturas(facturasCE model, string fechaPago, string totalSumar)
        {
            try
            {
                var facturasSeleccionadas = model.FacturasPorPagar.Where(x => x.IsChecked == true).ToList<factura>();
                var numeroActual = db.pago.ToList().Count();
                int numeroPago = int.Parse(numeroActual.ToString()) + 1;

                pago pagos = new pago();
                pagos.StatusPago_Id = 1;
                pagos.FechaRegistro = DateTime.Now;
                pagos.Numero = numeroPago;
                pagos.FechaPago = DateTime.Parse(fechaPago);
                pagos.MontoTotal = decimal.Parse(totalSumar);
                pagos.AspNetUsers_Id = User.Identity.GetUserId();
                db.pago.Add(pagos);

                foreach (var item in facturasSeleccionadas)
                {
                    factura facturas = db.factura.Find(item.id);
                    facturas.StatusFactura_Id = 2;
                    facturas.FechaPago = DateTime.Parse(fechaPago);

                    pagofacturas pagosFacturas = new pagofacturas();
                    pagosFacturas.Factura_Id = facturas.id;
                    pagosFacturas.Pago_Id = pagos.id;
                    db.pagofacturas.Add(pagosFacturas);
                }
                EnviarCorreo();
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }            
        }

        // GET: pagoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }            
            
            var pago = from p in db.pago
                       where p.id == id
                       select new { p.Numero, p.FechaPago, p.MontoTotal, estado = p.statuspago.descripcion };            

            if (pago == null)
            {
                return HttpNotFound();
            }
            return Json(pago, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "admin, finanzas, tesoreria")]
        public ActionResult CancelarPago(int? id)
        {
            var cancelacion = from pf in db.pagofacturas 
                              join p in db.pago on pf.Pago_Id equals p.id
                              join f in db.factura on pf.Factura_Id equals f.id
                              where p.id == id
                              select new { f.id, idpago = p.id };

            foreach (var item in cancelacion)
            {
                factura factura = db.factura.Find(item.id);
                factura.FechaPago = null;
                factura.StatusFactura_Id = 1;                
            }

            pago pago = db.pago.Find(id);
            
            if (pago.StatusPago_Id == 1)
            {
                pago.StatusPago_Id = 2;
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }     
        }

        // GET: pagoes/Create
        public ActionResult Create()
        {
            ViewBag.aspnetusers_id = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: pagoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,fecharegistro,numero,fechapago,montototal,aspnetusers_id")] pago pago)
        {
            if (ModelState.IsValid)
            {
                db.pago.Add(pago);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.aspnetusers_id = new SelectList(db.AspNetUsers, "Id", "Email", pago.AspNetUsers_Id);
            return View(pago);
        }

        // GET: pagoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pago pago = db.pago.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            ViewBag.aspnetusers_id = new SelectList(db.AspNetUsers, "Id", "Email", pago.AspNetUsers_Id);
            return View(pago);
        }

        // POST: pagoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,fecharegistro,numero,fechapago,montototal,aspnetusers_id")] pago pago)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pago).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.aspnetusers_id = new SelectList(db.AspNetUsers, "Id", "Email", pago.AspNetUsers_Id);
            return View(pago);
        }

        // GET: pagoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            pago pago = db.pago.Find(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            return View(pago);
        }

        // POST: pagoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            pago pago = db.pago.Find(id);
            db.pago.Remove(pago);
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
