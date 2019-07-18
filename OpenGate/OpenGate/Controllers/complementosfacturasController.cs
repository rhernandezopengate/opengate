using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity;

namespace OpenGate.Controllers
{
    [Authorize]
    public class complementosfacturasController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        public ActionResult VistaGeneral()
        {
            var complementosfacturas = db.complementosfacturas.Include(c => c.asigancion).Include(c => c.bancos).Include(c => c.cuentaordenante).Include(c => c.factura).Include(c => c.statuscomplemento);
            return View(complementosfacturas.ToList());
        }

        // GET: complementosfacturas
        public ActionResult Index()
        {
            var complementosfacturas = db.complementosfacturas.Include(c => c.asigancion).Include(c => c.bancos).Include(c => c.cuentaordenante).Include(c => c.factura).Include(c => c.statuscomplemento);
            return View(complementosfacturas.ToList());
        }

        [HttpPost]
        public JsonResult StatusComplementos()
        {
            List<SelectListItem> liststatus = new List<SelectListItem>();

            foreach (var item in db.statuscomplemento.ToList())
            {
                liststatus.Add(new SelectListItem
                {
                    Value = item.id.ToString(),
                    Text = item.descripcion
                });
            }

            return Json(liststatus);
        }

        public ActionResult ConsultaIndex()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var Numero = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var FechaPago = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var FechaFactura = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var RazonSocial = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var Estado = Request.Form.GetValues("columns[14][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<complementosfacturas> listaComplementos = new List<complementosfacturas>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_COMPLEMENTOSFACTURAS_PARAMETROSOPCIONALES] @fechaPago, @fechaFactura, @idproveedor, @idStatusComplemento, @numero, @idcategoria";
                    var query = new SqlCommand(sql, con);

                    query.Parameters.AddWithValue("@idcategoria", DBNull.Value);

                    if (Numero != "")
                    {
                        query.Parameters.AddWithValue("@numero", Numero);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@numero", DBNull.Value);
                    }

                    if (FechaPago != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaPago);
                        query.Parameters.AddWithValue("@fechaPago", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaPago", DBNull.Value);
                    }

                    if (FechaFactura != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaFactura);
                        query.Parameters.AddWithValue("@fechaFactura", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaFactura", DBNull.Value);
                    }

                    if (RazonSocial == "" || RazonSocial == "0")
                    {
                        query.Parameters.AddWithValue("@idproveedor", DBNull.Value);
                    }
                    else
                    {
                        var querys = db.proveedor.FirstOrDefault(x => x.RazonSocial == RazonSocial);

                        if (querys != null)
                        {
                            query.Parameters.AddWithValue("@idproveedor", querys.id);
                        }
                        else
                        {
                            query.Parameters.AddWithValue("@idproveedor", RazonSocial);
                        }
                    }

                    if (Estado == "" || Estado == "0")
                    {
                        query.Parameters.AddWithValue("@idStatusComplemento", DBNull.Value);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@idStatusComplemento", Estado);
                    }                                  

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            complementosfacturas complementosfacturas = new complementosfacturas();
                            bancos bancos = new bancos();
                            complementosfacturas.id = int.Parse(dr["id"].ToString());
                            complementosfacturas.Autorizacion = dr["Autorizacion"].ToString();
                            complementosfacturas.NumeroFactura1 = dr["Numero"].ToString();
                            complementosfacturas.FechaPago1 = Convert.ToDateTime(dr["FechaPago"].ToString());
                            complementosfacturas.FechaFactura1 = Convert.ToDateTime(dr["FechaFactura"].ToString());
                            complementosfacturas.Concepto1 = dr["Concepto"].ToString();
                            decimal? subtotal = decimal.Parse(dr["Subtotal"].ToString());
                            decimal? iva = decimal.Parse(dr["Iva"].ToString());
                            decimal? retencion = decimal.Parse(dr["Retencion"].ToString());
                            decimal? descuento = decimal.Parse(dr["Descuento"].ToString());
                            decimal? total = decimal.Parse(dr["Total"].ToString());
                            complementosfacturas.SubtotalString1 = String.Format("{0:C}", subtotal);
                            complementosfacturas.IvaString1 = String.Format("{0:C}", iva);
                            complementosfacturas.DescuentoString1 = String.Format("{0:C}", descuento);
                            complementosfacturas.RetencionString1 = String.Format("{0:C}", retencion);                            
                            complementosfacturas.TotalString1 = String.Format("{0:C}", total);                            
                            complementosfacturas.BancoString1 = dr["nombre"].ToString() + " RFC: " + dr["rfc"].ToString();
                            complementosfacturas.CuentaString1 = dr["descripcion"].ToString();
                            complementosfacturas.RazonString1 = dr["RazonSocial"].ToString();
                            complementosfacturas.StatusComplemento1 = dr["Expr1"].ToString();

                            listaComplementos.Add(complementosfacturas);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listaComplementos = listaComplementos.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listaComplementos.ToList().Count();
                var NewItems = listaComplementos.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }            
        }

        public ActionResult ConsultaTrafico()
        {
            return View();
        }

        public ActionResult ConsultaTraficoData()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var Numero = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var FechaPago = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var FechaFactura = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var RazonSocial = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var Estado = Request.Form.GetValues("columns[14][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<complementosfacturas> listaComplementos = new List<complementosfacturas>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_COMPLEMENTOSFACTURAS_PARAMETROSOPCIONALES] @fechaPago, @fechaFactura, @idproveedor, @idStatusComplemento, @numero, @idcategoria";
                    var query = new SqlCommand(sql, con);

                    query.Parameters.AddWithValue("@idcategoria", 1);

                    if (Numero != "")
                    {
                        query.Parameters.AddWithValue("@numero", Numero);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@numero", DBNull.Value);
                    }

                    if (FechaPago != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaPago);
                        query.Parameters.AddWithValue("@fechaPago", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaPago", DBNull.Value);
                    }

                    if (FechaFactura != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaFactura);
                        query.Parameters.AddWithValue("@fechaFactura", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaFactura", DBNull.Value);
                    }

                    if (RazonSocial == "" || RazonSocial == "0")
                    {
                        query.Parameters.AddWithValue("@idproveedor", DBNull.Value);
                    }
                    else
                    {
                        var querys = db.proveedor.FirstOrDefault(x => x.RazonSocial == RazonSocial);

                        if (querys != null)
                        {
                            query.Parameters.AddWithValue("@idproveedor", querys.id);
                        }
                        else
                        {
                            query.Parameters.AddWithValue("@idproveedor", RazonSocial);
                        }
                    }

                    if (Estado == "" || Estado == "0")
                    {
                        query.Parameters.AddWithValue("@idStatusComplemento", DBNull.Value);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@idStatusComplemento", Estado);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            complementosfacturas complementosfacturas = new complementosfacturas();
                            bancos bancos = new bancos();
                            complementosfacturas.id = int.Parse(dr["id"].ToString());
                            complementosfacturas.Autorizacion = dr["Autorizacion"].ToString();
                            complementosfacturas.NumeroFactura1 = dr["Numero"].ToString();
                            complementosfacturas.FechaPago1 = Convert.ToDateTime(dr["FechaPago"].ToString());
                            complementosfacturas.FechaFactura1 = Convert.ToDateTime(dr["FechaFactura"].ToString());
                            complementosfacturas.Concepto1 = dr["Concepto"].ToString();
                            decimal? subtotal = decimal.Parse(dr["Subtotal"].ToString());
                            decimal? iva = decimal.Parse(dr["Iva"].ToString());
                            decimal? retencion = decimal.Parse(dr["Retencion"].ToString());
                            decimal? descuento = decimal.Parse(dr["Descuento"].ToString());
                            decimal? total = decimal.Parse(dr["Total"].ToString());
                            complementosfacturas.SubtotalString1 = String.Format("{0:C}", subtotal);
                            complementosfacturas.IvaString1 = String.Format("{0:C}", iva);
                            complementosfacturas.DescuentoString1 = String.Format("{0:C}", descuento);
                            complementosfacturas.RetencionString1 = String.Format("{0:C}", retencion);
                            complementosfacturas.TotalString1 = String.Format("{0:C}", total);
                            complementosfacturas.BancoString1 = dr["nombre"].ToString() + " RFC: " + dr["rfc"].ToString();
                            complementosfacturas.CuentaString1 = dr["descripcion"].ToString();
                            complementosfacturas.RazonString1 = dr["RazonSocial"].ToString();
                            complementosfacturas.StatusComplemento1 = dr["Expr1"].ToString();

                            listaComplementos.Add(complementosfacturas);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listaComplementos = listaComplementos.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listaComplementos.ToList().Count();
                var NewItems = listaComplementos.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        //Borrar ctrl y vista
        public ActionResult AsignarBancos()
        {
            return View();
        }       

        public ActionResult GenerarComplemento(string FechaPago, string Proveedor_Id, string Bancos_Id, string CuentaOrdenante_Id, string Autorizacion)
        {
            try
            {
                int proveedorid = int.Parse(Proveedor_Id);
                int bancosid = int.Parse(Bancos_Id);
                int cuentaid = int.Parse(CuentaOrdenante_Id);
                DateTime fechapago = Convert.ToDateTime(FechaPago);

                var facturas = from f in db.factura
                              where f.Proveedor_Id == proveedorid && f.FechaPago == fechapago
                              select new { f.id };

                asigancion asigancion = new asigancion();
                asigancion.FechaRegistro = DateTime.Now;
                asigancion.AspNetUsers_Id = User.Identity.GetUserId();                
                int folio = db.asigancion.Count() + 1;
                asigancion.Folio = folio.ToString();
                db.asigancion.Add(asigancion);                

                foreach (var item in facturas)
                {
                    complementosfacturas complemento = new complementosfacturas();

                    int validacion = (from c in db.complementosfacturas
                                      where c.Factura_Id == item.id
                                     select new { c.id }).Count();

                    if (validacion > 0)
                    {
                        return Json("ErrorFactura", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        complemento.Factura_Id = item.id;
                        complemento.Asigancion_Id = asigancion.id;
                        complemento.Bancos_Id = bancosid;
                        complemento.CuentaOrdenante_Id = cuentaid;
                        complemento.Autorizacion = Autorizacion;
                        complemento.StatusComplemento_Id = 2;
                        db.complementosfacturas.Add(complemento);
                    }            
                }

                db.SaveChanges();

                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CrearAsignacionComplementos()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var RazonSocial = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var FechaPago = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<factura> listFacturas = new List<factura>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                { 
                    con.Open();

                    string sql = "exec [SP_CONTABILIDAD_COMPLEMENTO] @fechaPago, @idproveedor";
                    var query = new SqlCommand(sql, con);                                                  

                    if (FechaPago != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaPago);
                        query.Parameters.AddWithValue("@fechaPago", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaPago", DBNull.Value);
                    }
                    
                    if (RazonSocial == "" || RazonSocial == "0")
                    {
                        query.Parameters.AddWithValue("@idproveedor", DBNull.Value);
                    }
                    else
                    {
                        var querys = db.proveedor.FirstOrDefault(x => x.RazonSocial == RazonSocial);

                        if (querys != null)
                        {
                            query.Parameters.AddWithValue("@idproveedor", querys.id);
                        }
                        else
                        {
                            query.Parameters.AddWithValue("@idproveedor", RazonSocial);
                        }
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var facturas = new factura();
                                                        
                            facturas.id = Convert.ToInt32(dr["id"]);
                            facturas.Numero = dr["Numero"].ToString();
                            facturas.FechaFactura = Convert.ToDateTime(dr["FechaFactura"]);
                            facturas.FechaFacturaFin = Convert.ToDateTime(dr["FechaFactura"]);
                            facturas.FechaPago = Convert.ToDateTime(dr["FechaPago"]);
                            facturas.FechaPagoFin = Convert.ToDateTime(dr["FechaPago"]);
                            facturas.RazonSocial = dr["razonsocial"].ToString();
                            facturas.Concepto = dr["Concepto"].ToString();
                            facturas.Estado = dr["descripcion"].ToString();
                            decimal? subtotal = Convert.ToDecimal(dr["Subtotal"]);
                            decimal? iva = Convert.ToDecimal(dr["Iva"]);
                            decimal? descuento = Convert.ToDecimal(dr["Descuento"]);
                            decimal? retencion = Convert.ToDecimal(dr["Retencion"]);
                            decimal? total = Convert.ToDecimal(dr["Total"]);
                            facturas.SubTotalString = String.Format("{0:C}", subtotal);
                            facturas.IvaString = String.Format("{0:C}", iva);
                            facturas.DescuentoString = String.Format("{0:C}", descuento);
                            facturas.RetencionString = String.Format("{0:C}", retencion);
                            facturas.TotalString = String.Format("{0:C}", total);
                            facturas.Estado = dr["descripcion"].ToString();
                            facturas.IsComplemento = Convert.ToInt32(dr["iscomplemento"]); 

                            listFacturas.Add(facturas);
                        }
                    }

                    if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                    {
                        listFacturas = listFacturas.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                    }

                    TotalRecords = listFacturas.ToList().Count();
                    var NewItems = listFacturas.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                    return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }   
        }

        // GET: complementosfacturas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            complementosfacturas complementosfacturas = db.complementosfacturas.Find(id);
            if (complementosfacturas == null)
            {
                return HttpNotFound();
            }
            return View(complementosfacturas);
        }

        // GET: complementosfacturas/Create
        public ActionResult Create()
        {
            ViewBag.Asigancion_Id = new SelectList(db.asigancion, "id", "Folio");
            ViewBag.Bancos_Id = new SelectList(db.bancos, "id", "nombre");
            ViewBag.CuentaOrdenante_Id = new SelectList(db.cuentaordenante, "id", "descripcion");
            ViewBag.Factura_Id = new SelectList(db.factura, "id", "Numero");
            ViewBag.StatusComplemento_Id = new SelectList(db.statuscomplemento, "id", "descripcion");
            return View();
        }

        // POST: complementosfacturas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Bancos_Id,CuentaOrdenante_Id,Factura_Id,Autorizacion,StatusComplemento_Id,Asigancion_Id")] complementosfacturas complementosfacturas)
        {
            if (ModelState.IsValid)
            {
                db.complementosfacturas.Add(complementosfacturas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Asigancion_Id = new SelectList(db.asigancion, "id", "Folio", complementosfacturas.Asigancion_Id);
            ViewBag.Bancos_Id = new SelectList(db.bancos, "id", "nombre", complementosfacturas.Bancos_Id);
            ViewBag.CuentaOrdenante_Id = new SelectList(db.cuentaordenante, "id", "descripcion", complementosfacturas.CuentaOrdenante_Id);
            ViewBag.Factura_Id = new SelectList(db.factura, "id", "Numero", complementosfacturas.Factura_Id);
            ViewBag.StatusComplemento_Id = new SelectList(db.statuscomplemento, "id", "descripcion", complementosfacturas.StatusComplemento_Id);
            return View(complementosfacturas);
        }

        // GET: complementosfacturas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            complementosfacturas complementosfacturas = db.complementosfacturas.Find(id);
            if (complementosfacturas == null)
            {
                return HttpNotFound();
            }
            ViewBag.Asigancion_Id = new SelectList(db.asigancion, "id", "Folio", complementosfacturas.Asigancion_Id);
            ViewBag.Bancos_Id = new SelectList(db.bancos, "id", "nombre", complementosfacturas.Bancos_Id);
            ViewBag.CuentaOrdenante_Id = new SelectList(db.cuentaordenante, "id", "descripcion", complementosfacturas.CuentaOrdenante_Id);
            ViewBag.Factura_Id = new SelectList(db.factura, "id", "Numero", complementosfacturas.Factura_Id);
            ViewBag.StatusComplemento_Id = new SelectList(db.statuscomplemento, "id", "descripcion", complementosfacturas.StatusComplemento_Id);
            return PartialView("Edit", complementosfacturas);
        }

        // POST: complementosfacturas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Bancos_Id,CuentaOrdenante_Id,Factura_Id,Autorizacion,StatusComplemento_Id,Asigancion_Id")] complementosfacturas complementosfacturas)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(complementosfacturas).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Editado Correctamente." });
                }
                ViewBag.Asigancion_Id = new SelectList(db.asigancion, "id", "Folio", complementosfacturas.Asigancion_Id);
                ViewBag.Bancos_Id = new SelectList(db.bancos, "id", "nombre", complementosfacturas.Bancos_Id);
                ViewBag.CuentaOrdenante_Id = new SelectList(db.cuentaordenante, "id", "descripcion", complementosfacturas.CuentaOrdenante_Id);
                ViewBag.Factura_Id = new SelectList(db.factura, "id", "Numero", complementosfacturas.Factura_Id);
                ViewBag.StatusComplemento_Id = new SelectList(db.statuscomplemento, "id", "descripcion", complementosfacturas.StatusComplemento_Id);
                return PartialView("Edit", complementosfacturas);
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        // GET: complementosfacturas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            complementosfacturas complementosfacturas = db.complementosfacturas.Find(id);
            if (complementosfacturas == null)
            {
                return HttpNotFound();
            }
            return View(complementosfacturas);
        }

        // POST: complementosfacturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            complementosfacturas complementosfacturas = db.complementosfacturas.Find(id);
            db.complementosfacturas.Remove(complementosfacturas);
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
