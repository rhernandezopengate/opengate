using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;
using System.Linq.Dynamic;
using System.Globalization;

namespace OpenGate.Controllers
{
    [Authorize]
    public class ordenesController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: ordenes
        [Authorize(Roles = "admin, homedeliveryoperaciones, analistainventarios")]
        public ActionResult Index()
        {
            return View(db.ordenes.ToList());
        }

        [Authorize(Roles = "admin, homedeliveryoperaciones, analistainventarios")]
        public ActionResult ObtenerOrdenes()
        {
            try
            {
                var Draw = Request.Form.GetValues("draw").FirstOrDefault();
                var Start = Request.Form.GetValues("start").FirstOrDefault();
                var Length = Request.Form.GetValues("length").FirstOrDefault();
                var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
                var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                var orden = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                var fechaRegistro = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
                var status = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

                int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
                int Skip = Start != null ? Convert.ToInt32(Start) : 0;
                int TotalRecords = 0;

                List<ordenes> listaOrdenes = new List<ordenes>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec SP_ConsultaOrdenes_ParametrosOpcionales @orden, @fechaOrdenes, @idstatus";
                    var query = new SqlCommand(sql, con);
                 
                    if (orden != "")
                    {
                        query.Parameters.AddWithValue("@orden", orden);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@orden", DBNull.Value);
                    }

                    if (fechaRegistro != "")
                    {
                        DateTime date = Convert.ToDateTime(fechaRegistro);
                        query.Parameters.AddWithValue("@fechaOrdenes", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaOrdenes", DBNull.Value);
                    }

                    if (status == "" || status == "0")
                    {
                        query.Parameters.AddWithValue("@idstatus", DBNull.Value);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@idstatus", status);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var ordenes = new ordenes();

                            ordenes.id = Convert.ToInt32(dr["id"]);

                            ordenes.Orden = dr["Orden"].ToString();
                            ordenes.Picker = dr["Picker"].ToString();
                            ordenes.Auditor = dr["nombre"].ToString().ToUpper();
                            ordenes.StatusString = dr["descripcion"].ToString().ToUpper();
                            ordenes.FechaAltaString =  dr["FechaAlta"].ToString();

                            listaOrdenes.Add(ordenes);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listaOrdenes = listaOrdenes.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listaOrdenes.ToList().Count();
                var NewItems = listaOrdenes.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        public ActionResult EliminarOrden(string error)
        {
            ViewBag.Error = error;
            return View();
        }

        public ActionResult EliminarCorte(string error)
        {
            ViewBag.Error = error;
            ViewBag.UltimoCorte = db.ordenes.OrderByDescending(x => x.id).FirstOrDefault().FechaAlta;

            return View();
        }

        public ActionResult ConfirmarEliminarCorte()
        {
            try
            {
                var orden = db.ordenes.OrderByDescending(x => x.id).FirstOrDefault();             
                
                var student = db.ordenes
                 .SqlQuery("Select * from ordenes where FechaAlta=@fecha", new SqlParameter("@fecha", orden.FechaAlta))
                 .ToList();

                foreach (var item in student)
                {
                    var detalle = db.detordenproductoshd.Where(x => x.Ordenes_Id == item.id).ToList();
                    var guias = db.guias.Where(x => x.Ordenes_Id == item.id).ToList();
                    var detususario = db.detusuariosordenes.Where(x => x.Ordenes_Id == item.id).FirstOrDefault();
                    var codigoqr = db.codigoqrordenes.Where(x => x.Ordenes_Id == item.id).ToList();
                    var ordenDelete = db.ordenes.Where(x => x.Orden == item.Orden).FirstOrDefault();
                    
                    if (detalle != null)
                    {
                        db.detordenproductoshd.RemoveRange(detalle);
                    }

                    if (guias != null)
                    {
                        db.guias.RemoveRange(guias);
                    }

                    if (detususario != null)
                    {
                        db.detusuariosordenes.Remove(detususario);
                    }

                    if (codigoqr != null)
                    {
                        db.codigoqrordenes.RemoveRange(codigoqr);
                    }

                    db.ordenes.Remove(ordenDelete);                    
                }

                db.SaveChanges();

                return Json("Correcto", JsonRequestBehavior.AllowGet);                
            }
            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }            
        }
        
        public ActionResult ConfirmarEliminacionOrdenes(string orden)
        {
            try
            {
                var ordenTemp = db.ordenes.Where(x => x.Orden.Equals(orden)).FirstOrDefault();

                if (ordenTemp != null)
                {
                    var detalle = db.detordenproductoshd.Where(x => x.Ordenes_Id == ordenTemp.id).ToList();
                    var guias = db.guias.Where(x => x.Ordenes_Id == ordenTemp.id).ToList();
                    var detususario = db.detusuariosordenes.Where(x => x.Ordenes_Id == ordenTemp.id).FirstOrDefault();
                    var codigoqr = db.codigoqrordenes.Where(x => x.Ordenes_Id == ordenTemp.id).ToList();
                    var ordenDelete = db.ordenes.Where(x => x.Orden == orden).FirstOrDefault();

                    if (detalle != null)
                    {
                        db.detordenproductoshd.RemoveRange(detalle);
                    }

                    if (guias != null)
                    {
                        db.guias.RemoveRange(guias);
                    }

                    if (detususario != null)
                    {
                        db.detusuariosordenes.Remove(detususario);
                    }

                    if (codigoqr != null)
                    {
                        db.codigoqrordenes.RemoveRange(codigoqr);
                    }
                                
                    db.ordenes.Remove(ordenDelete);

                    db.SaveChanges();

                    return RedirectToAction("EliminarOrden", new { error = "Correcto" });
                }
                else
                {
                    return RedirectToAction("EliminarOrden", new { error = "Error" });
                }
            }
            catch (Exception)
            {
                return RedirectToAction("EliminarOrden", new { error = "Cath" });
            }
        }
        
        [HttpPost]
        [Authorize(Roles = "admin, homedeliveryoperaciones, analistainventarios")]
        public JsonResult ListaStatus()
        {
            List<SelectListItem> liststatus = new List<SelectListItem>();

            foreach (var item in db.statusordenimpresa.ToList())
            {
                liststatus.Add(new SelectListItem
                {
                    Value = item.id.ToString(),
                    Text = item.descripcion
                });
            }

            return Json(liststatus);
        }

        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Importar()
        {
            return View();
        }

        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Cargar(HttpPostedFileBase postedFileBase)
        {
            try
            {
                string filePath = string.Empty;

                if (postedFileBase != null)
                {
                    string path = Server.MapPath("~/Uploads/");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(postedFileBase.FileName);
                    string extension = Path.GetExtension(postedFileBase.FileName);
                    postedFileBase.SaveAs(filePath);

                    DataTable dtCharge = new DataTable();

                    dtCharge.Columns.AddRange(new DataColumn[5] {
                        //1
                        new DataColumn("item", typeof(string)),
                        //2                        
                        new DataColumn("qty", typeof(string)),
                        //3
                        new DataColumn("ea", typeof(string)),
                        //4
                        new DataColumn("order", typeof(string)),
                        //5
                        new DataColumn("createdby", typeof(string)),
                    });

                    string csvData = System.IO.File.ReadAllText(filePath);

                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            dtCharge.Rows.Add();
                            int i = 0;

                            //Execute a loop over the columns.
                            foreach (string cell in row.Split(','))
                            {
                                dtCharge.Rows[dtCharge.Rows.Count - 1][i] = cell;

                                i++;
                            }
                        }
                    }

                    List<ordenes> listaValidacion = db.ordenes.ToList();
                    List<ordenes> lista = new List<ordenes>();

                    foreach (DataRow row in dtCharge.Rows)
                    {
                        ordenes ordenes = new ordenes();
                        ordenes.FechaAlta = DateTime.Now;
                        ordenes.Orden = row[3].ToString();
                        ordenes.User = row[4].ToString();
                        ordenes.StatusOrdenImpresa_Id = 1;
                        
                        var orden = listaValidacion.Where(x => x.Orden.Equals(ordenes.Orden.Trim())).FirstOrDefault();
                        if (orden == null)
                        {
                            lista.Add(ordenes);
                        }
                        else
                        {
                            Console.WriteLine("Orden Duplicada");
                        }
                    }

                    if (lista.Count > 0)
                    {
                        var grouping = lista.GroupBy(x => x.Orden).ToList();
                        DataTable dtOrden = new DataTable();

                        dtOrden.Columns.AddRange(new DataColumn[4] {
                            //1
                            new DataColumn("FechaAlta", typeof(DateTime)),
                            //2                        
                            new DataColumn("StatusOrdenImpresa_Id", typeof(int)),
                            //3
                            new DataColumn("Orden", typeof(string)),
                            //4
                            new DataColumn("User", typeof(string)),                        
                        });

                        foreach (var item in grouping)
                        {
                            var orden = lista.Where(x => x.Orden == item.Key).FirstOrDefault();

                            if (orden != null)
                            {
                                dtOrden.Rows.Add(new object[] {
                                    DateTime.Now,
                                    orden.StatusOrdenImpresa_Id,
                                    orden.Orden,
                                    orden.User
                                });
                            }
                            else
                            {
                                Console.Write("La orden ya existe");
                            }
                        }

                        string connectionString = @"Data Source=SQL7001.site4now.net;Initial Catalog=DB_A3F19C_OG;User Id=DB_A3F19C_OG_admin;Password=xQ9znAhU;";

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
                        {
                            bulkCopy.DestinationTableName = "dbo.ordenes";

                            bulkCopy.ColumnMappings.Add("FechaAlta", "FechaAlta");
                            bulkCopy.ColumnMappings.Add("Orden", "Orden");
                            bulkCopy.ColumnMappings.Add("User", "User");
                            bulkCopy.ColumnMappings.Add("StatusOrdenImpresa_Id", "StatusOrdenImpresa_Id");

                            try
                            {
                                bulkCopy.WriteToServer(dtOrden);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }

                        DataTable dtDetalles = new DataTable();

                        dtDetalles.Columns.AddRange(new DataColumn[3] {
                            //1
                            new DataColumn("Ordenes_Id", typeof(int)),
                            //2                        
                            new DataColumn("Skus_Id", typeof(int)),
                            //3
                            new DataColumn("cantidad", typeof(string)),                            
                        });

                        List<ordenes> listaOrdenes = db.ordenes.ToList();
                        List<skus> listaSKU = db.skus.ToList();

                        foreach (DataRow row in dtCharge.Rows)
                        {
                            string orden = row[3].ToString();
                            string sku = row[0].ToString();

                            var ordenTemp = lista.Where(x => x.Orden.Contains(orden)).FirstOrDefault();

                            if (ordenTemp != null)
                            {
                                int idOrden = listaOrdenes.Where(x => x.Orden.Contains(orden)).FirstOrDefault().id;
                                int idSKU = listaSKU.Where(x => x.Sku.Contains(sku)).FirstOrDefault().id;
                                dtDetalles.Rows.Add(new object[] {
                                    idOrden,
                                    idSKU,
                                    int.Parse(row[1].ToString())
                                });
                            }
                        }

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString))
                        {
                            bulkCopy.DestinationTableName = "dbo.detordenproductoshd";

                            bulkCopy.ColumnMappings.Add("Ordenes_Id", "Ordenes_Id");
                            bulkCopy.ColumnMappings.Add("Skus_Id", "Skus_Id");
                            bulkCopy.ColumnMappings.Add("cantidad", "cantidad");

                            try
                            {
                                bulkCopy.WriteToServer(dtDetalles);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("NO SE HAN ENCONTRADO REGISTROS NUEVOS");
                    }

                }

                return RedirectToAction("Index");
            }
            catch (Exception _ex)
            {
                string error = _ex.Message.ToString();
                return RedirectToAction("Error500", "Errores", new { error = error });
            }            
        }

        // GET: ordenes/Details/5
        [Authorize(Roles = "admin, homedeliveryoperaciones, analistainventarios")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ordenes ordenes = db.ordenes.Find(id);
            if (ordenes == null)
            {
                return HttpNotFound();
            }

            var detuser = db.detusuariosordenes.Where(x => x.Ordenes_Id == id).FirstOrDefault();
            if (detuser != null)
            {
                ViewBag.Auditor = detuser.usuarios.nombre.ToUpper();
            }

            var detalleOrden = db.detordenproductoshd.Where(x => x.Ordenes_Id == id).ToList();
            List<detordenproductoshd> lista = new List<detordenproductoshd>();
            foreach (var item in detalleOrden)
            {
                detordenproductoshd detordenproductoshd = new detordenproductoshd();
                detordenproductoshd.cantidad = item.cantidad;
                detordenproductoshd.SKU = item.skus.Sku;
                detordenproductoshd.SKUDescripcion = item.skus.Descripcion;
                lista.Add(detordenproductoshd);
            }

            ViewBag.Detalle = lista;
            
            return View(ordenes);
        }

        // GET: ordenes/Create
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ordenes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Create([Bind(Include = "id,FechaAlta,Orden,User,StatusOrdenImpresa_Id,Picker")] ordenes ordenes)
        {
            if (ModelState.IsValid)
            {
                db.ordenes.Add(ordenes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ordenes);
        }

        // GET: ordenes/Edit/5
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ordenes ordenes = db.ordenes.Find(id);
            ViewBag.StatusOrdenImpresa_Id = new SelectList(db.statusordenimpresa, "id", "descripcion", ordenes.StatusOrdenImpresa_Id);

            if (ordenes == null)
            {
                return HttpNotFound();
            }
            
            return View(ordenes);
        }

        // POST: ordenes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Edit(ordenes ordenes)
        {
            ordenes ordenes1 = db.ordenes.Find(ordenes.id);

            if (ordenes1 != null)
            {
                ordenes1.StatusOrdenImpresa_Id = ordenes.StatusOrdenImpresa_Id;
                ordenes1.FechaAlta = ordenes.FechaAlta;
                ordenes1.Orden = ordenes.Orden;
                //Oracle ID
                ordenes1.User = ordenes.User;
                ordenes1.Picker = ordenes.Picker;

                db.SaveChanges();
                return Json("Correcto", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return View(ordenes);
            }
        }

        // GET: ordenes/Delete/5
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ordenes ordenes = db.ordenes.Find(id);
            if (ordenes == null)
            {
                return HttpNotFound();
            }
            return View(ordenes);
        }

        // POST: ordenes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult DeleteConfirmed(int id)
        {
            ordenes ordenes = db.ordenes.Find(id);
            db.ordenes.Remove(ordenes);
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
