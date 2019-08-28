using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace OpenGate.Controllers
{
    [Authorize]
    public class guiasController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: guias
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult ObtenerGuias()
        {
            try
            {
                var Draw = Request.Form.GetValues("draw").FirstOrDefault();
                var Start = Request.Form.GetValues("start").FirstOrDefault();
                var Length = Request.Form.GetValues("length").FirstOrDefault();
                var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
                var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                var guia = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                var orden = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

                int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
                int Skip = Start != null ? Convert.ToInt32(Start) : 0;
                int TotalRecords = 0;

                List<guias> listaGuias = new List<guias>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec SP_Guias_ParametrosOpcionales @orden, @guia";
                    var query = new SqlCommand(sql, con);

                    if (guia != "")
                    {                        
                        query.Parameters.AddWithValue("@guia", guia);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@guia", DBNull.Value);
                    }

                    if (orden != "")
                    {
                        query.Parameters.AddWithValue("@orden", orden);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@orden", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var guias = new guias();

                            guias.id = Convert.ToInt32(dr["id"]);
                            guias.Guia = dr["Guia"].ToString();
                            guias.Orden = dr["Orden"].ToString();

                            listaGuias.Add(guias);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listaGuias = listaGuias.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listaGuias.ToList().Count();
                var NewItems = listaGuias.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Importar()
        {
            return View();
        }

        [HttpPost]
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

                    DataTable dt = new DataTable();

                    dt.Columns.AddRange(new DataColumn[2] {
                        //1
                        new DataColumn("orden", typeof(string)),
                        //2                        
                        new DataColumn("guia", typeof(string)),
                    });

                    string csvData = System.IO.File.ReadAllText(filePath);

                    foreach (string row in csvData.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {
                            dt.Rows.Add();
                            int i = 0;

                            //Execute a loop over the columns.
                            foreach (string cell in row.Split(','))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell;                                

                                i++;
                            }
                        }
                    }

                    dt.AcceptChanges();

                    DataTable dtCaptura = new DataTable();

                    dtCaptura.Columns.AddRange(new DataColumn[2] {
                        //1
                        new DataColumn("Ordenes_Id", typeof(int)),
                        //2                        
                        new DataColumn("Guia", typeof(string)),
                    });

                    foreach (DataRow orow in dt.Select())
                    {
                        DataRow rowCaptura = dtCaptura.NewRow();
                        string guia = orow["guia"].ToString();
                        string orden = orow["orden"].ToString();

                        var ordenes = db.ordenes.Where(x => x.Orden.Equals(orden)).FirstOrDefault();

                        if (ordenes != null)
                        {
                            rowCaptura["Ordenes_Id"] = ordenes.id;
                            rowCaptura["Guia"] = guia;
                            dtCaptura.Rows.Add(rowCaptura);
                        }
                    }

                    string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "dbo.guias";                           
                            sqlBulkCopy.ColumnMappings.Add("Guia", "Guia");
                            sqlBulkCopy.ColumnMappings.Add("Ordenes_Id", "Ordenes_Id");                     

                            con.Open();

                            sqlBulkCopy.WriteToServer(dtCaptura);
                            con.Close();
                        }
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

        // GET: guias/Details/5
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            guias guias = db.guias.Find(id);
            if (guias == null)
            {
                return HttpNotFound();
            }
            return View(guias);
        }

        // GET: guias/Create
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Create()
        {
            ViewBag.Ordenes_Id = new SelectList(db.ordenes, "id", "Orden");
            return View();
        }

        // POST: guias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Create([Bind(Include = "id,Guia,Ordenes_Id")] guias guias)
        {
            if (ModelState.IsValid)
            {
                db.guias.Add(guias);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Ordenes_Id = new SelectList(db.ordenes, "id", "Orden", guias.Ordenes_Id);
            return View(guias);
        }

        // GET: guias/Edit/5
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            guias guias = db.guias.Find(id);
            if (guias == null)
            {
                return HttpNotFound();
            }
            ViewBag.Ordenes_Id = new SelectList(db.ordenes, "id", "Orden", guias.Ordenes_Id);
            return View(guias);
        }

        // POST: guias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Guia,Ordenes_Id")] guias guias)
        {
            if (ModelState.IsValid)
            {
                db.Entry(guias).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Ordenes_Id = new SelectList(db.ordenes, "id", "Orden", guias.Ordenes_Id);
            return View(guias);
        }

        // GET: guias/Delete/5
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            guias guias = db.guias.Find(id);
            if (guias == null)
            {
                return HttpNotFound();
            }
            return View(guias);
        }

        // POST: guias/Delete/5
        [Authorize(Roles = "admin, homedeliveryoperaciones")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            guias guias = db.guias.Find(id);
            db.guias.Remove(guias);
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
