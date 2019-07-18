using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;
using System.Linq.Dynamic;

namespace OpenGate.Controllers
{
    [Authorize]
    public class kpisinventariosController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: kpisinventarios
        public ActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        public ActionResult ListaKpis()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var Mes = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
            var Area = Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<kpisinvetntarios> listaKPIS = new List<kpisinvetntarios>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [dbo].[SP_KPISINVENTARIOS_PARAMETROSOPCIONALES] @mes, @area";

                    var query = new SqlCommand(sql, con);

                                      

                    if (Mes != "")
                    {
                        query.Parameters.AddWithValue("@mes", Mes);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@mes", DBNull.Value);
                    }

                    if (Area != "")
                    {
                        query.Parameters.AddWithValue("@area", Area);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@area", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var kpis = new kpisinvetntarios();

                            kpis.id = Convert.ToInt32(dr["id"]);
                            kpis.skuvariance = dr["skuvariance"].ToString();
                            kpis.monto = Convert.ToInt32(dr["monto"]);
                            kpis.porcentajemonto = Convert.ToDecimal(dr["porcentajemonto"]);
                            kpis.piezas = Convert.ToInt32(dr["piezas"]);
                            kpis.porcentajepiezas = Convert.ToDecimal(dr["porcentajepiezas"]);
                            kpis.mes = dr["mes"].ToString();
                            kpis.area = dr["area"].ToString();

                            listaKPIS.Add(kpis);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listaKPIS = listaKPIS.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listaKPIS.ToList().Count();
                var NewItems = listaKPIS.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                ViewBag.Error = "Ha ocurrido un error. Contacte al administrador del sistema";
                return RedirectToAction("Error500", "Errores");
            }
        }

        public ActionResult Resumen()
        {
            var mes = db.kpisinvetntarios.OrderByDescending(x => x.id).First().mes;
            ViewBag.mesCedis = mes;
            ViewData["CEDIS"] = db.kpisinvetntarios.Where(x => x.mes.Contains(mes) && x.area.Contains("CEDIS")).ToList();
            ViewData["HomeDelivery"] = db.kpisinvetntarios.Where(x => x.mes.Contains(mes) && x.area.Contains("Home Delivery")).ToList();

            return PartialView();
        }

        [Authorize(Roles = "admin, coordinadorinventarios")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, coordinadorinventarios")]
        public ActionResult Import(HttpPostedFileBase postedFileBase)
        {
            try
            {
                List<kpisinvetntarios> customersModels = new List<kpisinvetntarios>();
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
                    dt.Columns.AddRange(new DataColumn[7] {
                        new DataColumn("SKUVariance", typeof(string)),
                        new DataColumn("Monto", typeof(int)),
                        new DataColumn("PorcentajeMonto", typeof(decimal)),
                        new DataColumn("Piezas",typeof(int)),
                        new DataColumn("PorcentajePiezas", typeof(decimal)),
                        new DataColumn("Mes", typeof(string)),
                        new DataColumn("Area", typeof(string))                        
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

                    string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "dbo.kpisinvetntarios";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table                            
                            sqlBulkCopy.ColumnMappings.Add("SKUVariance", "skuvariance");
                            sqlBulkCopy.ColumnMappings.Add("Monto", "monto");
                            sqlBulkCopy.ColumnMappings.Add("PorcentajeMonto", "porcentajemonto");
                            sqlBulkCopy.ColumnMappings.Add("Piezas", "piezas");                            
                            sqlBulkCopy.ColumnMappings.Add("PorcentajePiezas", "porcentajepiezas");
                            sqlBulkCopy.ColumnMappings.Add("Mes", "mes");
                            sqlBulkCopy.ColumnMappings.Add("Area", "area");

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
                ViewBag.Error = _ex.Message.ToString();
                return RedirectToAction("Error500", "Errores");
            }
        }

    }
}