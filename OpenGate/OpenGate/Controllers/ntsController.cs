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
    public class ntsController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: nts
        public ActionResult Index()
        {
            return View(db.nts.ToList());
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

                    dt.Columns.AddRange(new DataColumn[18] {
                        //1
                        new DataColumn("NTSDate", typeof(DateTime)),
                        //2
                        new DataColumn("Order", typeof(string)),
                        //3
                        new DataColumn("DSIdCliente", typeof(string)),
                        //4
                        new DataColumn("DSNombreCliente", typeof(string)),
                        //5
                        new DataColumn("OSACalleNumero", typeof(string)),
                        //6
                        new DataColumn("OSANull1", typeof(string)),
                        //7
                        new DataColumn("OSAColonia", typeof(string)),
                        //8
                        new DataColumn("OSADestinatario", typeof(string)),
                        //9
                        new DataColumn("OSACiudad", typeof(string)),
                        //10
                        new DataColumn("OSANull2", typeof(string)),
                        //11
                        new DataColumn("OSAEstado", typeof(string)),
                        //12
                        new DataColumn("OSACodigoPostal", typeof(string)),
                        //13
                        new DataColumn("OSI", typeof(string)),
                        //14
                        new DataColumn("osp", typeof(string)),
                        //15
                        new DataColumn("osh", typeof(string)),
                        //16
                        new DataColumn("ofcodigopais", typeof(string)),
                        //17
                        new DataColumn("oforigen", typeof(string)),
                        //18
                        new DataColumn("fecharegistro", typeof(DateTime)),                        
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
                                    if (dt.Rows[dt.Rows.Count - 1][3].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][3] = "NA";
                                    }
                                    if (dt.Rows[dt.Rows.Count - 1][4].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][4] = "NA";
                                    }
                                    if (dt.Rows[dt.Rows.Count - 1][5].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][5] = "NA";
                                    }
                                    if (dt.Rows[dt.Rows.Count - 1][6].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][6] = "NA";
                                    }
                                    if (dt.Rows[dt.Rows.Count - 1][7].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][7] = "NA";
                                    }
                                    if (dt.Rows[dt.Rows.Count - 1][8].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][8] = "NA";
                                    }
                                    if (dt.Rows[dt.Rows.Count - 1][9].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][9] = "NA";
                                    }
                                    if (dt.Rows[dt.Rows.Count - 1][10].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][10] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][11].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][11] = "NA";
                                    }
                                    if (dt.Rows[dt.Rows.Count - 1][12].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][12] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][13].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][13] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][14].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][14] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][15].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][15] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][16].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][16] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][17].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][17] = DateTime.Parse("01/01/1900");
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
                        string order = orow["Order"].ToString();
                        nts nts = db.nts.Where(x => x.Order == order).FirstOrDefault();
                        if (nts != null)
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
                            sqlBulkCopy.DestinationTableName = "dbo.nts";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table                            
                            //1
                            sqlBulkCopy.ColumnMappings.Add("NTSDate", "NTSDate");
                            //2
                            sqlBulkCopy.ColumnMappings.Add("Order", "Order");
                            //3
                            sqlBulkCopy.ColumnMappings.Add("DSIdCliente", "DSIdCliente");
                            //4
                            sqlBulkCopy.ColumnMappings.Add("DSNombreCliente", "DSNombreCliente");
                            //5
                            sqlBulkCopy.ColumnMappings.Add("OSACalleNumero", "OSACalleNumero");
                            //6
                            sqlBulkCopy.ColumnMappings.Add("OSANull1", "OSANull1");
                            //7
                            sqlBulkCopy.ColumnMappings.Add("OSAColonia", "OSAColonia");
                            //8
                            sqlBulkCopy.ColumnMappings.Add("OSADestinatario", "OSADestinatario");
                            //9
                            sqlBulkCopy.ColumnMappings.Add("OSACiudad", "OSACiudad");
                            //10
                            sqlBulkCopy.ColumnMappings.Add("OSANull2", "OSANull2");
                            //11
                            sqlBulkCopy.ColumnMappings.Add("OSAEstado", "OSAEstado");
                            //12
                            sqlBulkCopy.ColumnMappings.Add("OSACodigoPostal", "OSACodigoPostal");
                            //13
                            sqlBulkCopy.ColumnMappings.Add("OSI", "OSI");
                            //14
                            sqlBulkCopy.ColumnMappings.Add("osp", "osp");
                            //15
                            sqlBulkCopy.ColumnMappings.Add("osh", "osh");
                            //16
                            sqlBulkCopy.ColumnMappings.Add("ofcodigopais", "ofcodigopais");
                            //17
                            sqlBulkCopy.ColumnMappings.Add("oforigen", "oforigen");
                            //18
                            sqlBulkCopy.ColumnMappings.Add("fecharegistro", "fecharegistro");
                            
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

        // GET: nts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nts nts = db.nts.Find(id);
            if (nts == null)
            {
                return HttpNotFound();
            }
            return View(nts);
        }

        // GET: nts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: nts/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,NTSDate,Order,DSIdCliente,DSNombreCliente,OSACalleNumero,OSANull1,OSAColonia,OSADestinatario,OSACiudad,OSANull2,OSAEstado,OSACodigoPostal,OSI,osp,osh,ofcodigopais,oforigen,fecharegistro")] nts nts)
        {
            if (ModelState.IsValid)
            {
                db.nts.Add(nts);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nts);
        }

        // GET: nts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nts nts = db.nts.Find(id);
            if (nts == null)
            {
                return HttpNotFound();
            }
            return View(nts);
        }

        // POST: nts/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,NTSDate,Order,DSIdCliente,DSNombreCliente,OSACalleNumero,OSANull1,OSAColonia,OSADestinatario,OSACiudad,OSANull2,OSAEstado,OSACodigoPostal,OSI,osp,osh,ofcodigopais,oforigen,fecharegistro")] nts nts)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nts);
        }

        // GET: nts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            nts nts = db.nts.Find(id);
            if (nts == null)
            {
                return HttpNotFound();
            }
            return View(nts);
        }

        // POST: nts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            nts nts = db.nts.Find(id);
            db.nts.Remove(nts);
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
