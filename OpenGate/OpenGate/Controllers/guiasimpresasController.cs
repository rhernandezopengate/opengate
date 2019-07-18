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

namespace OpenGate.Controllers
{
    public class guiasimpresasController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: guiasimpresas
        public ActionResult Index()
        {
            return View(db.guiasimpresas.ToList());
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase postedFileBase)
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

                    dt.Columns.AddRange(new DataColumn[3] {
                        //1
                        new DataColumn("fecha", typeof(DateTime)),
                        //2
                        new DataColumn("orden", typeof(string)),
                        //3                        
                        new DataColumn("numero", typeof(string)),
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
                                if (cell.Equals(string.Empty))
                                {
                                    if (dt.Rows[dt.Rows.Count - 1][0].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][0] = DateTime.Parse("01/01/1900");
                                    }
                                    if (dt.Rows[dt.Rows.Count - 1][1].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][1] = "NA";
                                    }
                                    if (dt.Rows[dt.Rows.Count - 1][2].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][2] = "NA";
                                    }                                    
                                }
                                else
                                {
                                    dt.Rows[dt.Rows.Count - 1][i] = cell;
                                }
                                i++;
                            }
                        }
                    }
                                        
                    foreach (DataRow orow in dt.Select())
                    {
                        string orden = orow["orden"].ToString();
                        guiasimpresas guiasimpresas = db.guiasimpresas.Where(x => x.orden == orden).FirstOrDefault();
                        if (guiasimpresas != null)
                        {
                            dt.Rows.Remove(orow);
                        }                        
                    }

                    dt.AcceptChanges();

                    string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "dbo.guiasimpresas";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table                            
                            //1
                            sqlBulkCopy.ColumnMappings.Add("fecha", "fecha");
                            //2
                            sqlBulkCopy.ColumnMappings.Add("numero", "numero");
                            //3
                            sqlBulkCopy.ColumnMappings.Add("orden", "orden");

                            con.Open();

                            sqlBulkCopy.WriteToServer(dt);
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

        // GET: guiasimpresas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            guiasimpresas guiasimpresas = db.guiasimpresas.Find(id);
            if (guiasimpresas == null)
            {
                return HttpNotFound();
            }
            return View(guiasimpresas);
        }

        // GET: guiasimpresas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: guiasimpresas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,fecha,numero,orden")] guiasimpresas guiasimpresas)
        {
            if (ModelState.IsValid)
            {
                db.guiasimpresas.Add(guiasimpresas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(guiasimpresas);
        }

        // GET: guiasimpresas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            guiasimpresas guiasimpresas = db.guiasimpresas.Find(id);
            if (guiasimpresas == null)
            {
                return HttpNotFound();
            }
            return View(guiasimpresas);
        }

        // POST: guiasimpresas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,fecha,numero,orden")] guiasimpresas guiasimpresas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(guiasimpresas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(guiasimpresas);
        }

        // GET: guiasimpresas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            guiasimpresas guiasimpresas = db.guiasimpresas.Find(id);
            if (guiasimpresas == null)
            {
                return HttpNotFound();
            }
            return View(guiasimpresas);
        }

        // POST: guiasimpresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            guiasimpresas guiasimpresas = db.guiasimpresas.Find(id);
            db.guiasimpresas.Remove(guiasimpresas);
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
