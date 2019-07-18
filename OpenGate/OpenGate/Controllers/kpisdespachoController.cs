using OpenGate.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using Newtonsoft.Json;

namespace OpenGate.Controllers
{
    [Authorize]
    public class kpisdespachoController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: kpisdespacho
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin, coordinadoroperaciones")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin, coordinadoroperaciones")]
        public ActionResult Import(HttpPostedFileBase postedFileBase)
        {
            try
            {
                List<kpisdespacho> customersModels = new List<kpisdespacho>();
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
                    dt.Columns.AddRange(new DataColumn[11] {
                        new DataColumn("Mes", typeof(string)),
                        new DataColumn("WK", typeof(int)),
                        new DataColumn("DiaSemana", typeof(int)),
                        new DataColumn("ValorDia",typeof(decimal)),
                        new DataColumn("CV", typeof(string)),
                        new DataColumn("MO", typeof(string)),
                        new DataColumn("TipoEmbarque", typeof(string)),
                        new DataColumn("FechaEmbarque", typeof(DateTime)),
                        new DataColumn("QTYPiezas", typeof(int)),
                        new DataColumn("QTYCajas", typeof(int)),
                        new DataColumn("QTYPallets",typeof(decimal))                        
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
                            sqlBulkCopy.DestinationTableName = "dbo.kpisdespacho";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table                            
                            sqlBulkCopy.ColumnMappings.Add("Mes", "mes");
                            sqlBulkCopy.ColumnMappings.Add("WK", "wk");
                            sqlBulkCopy.ColumnMappings.Add("CV", "cv");
                            sqlBulkCopy.ColumnMappings.Add("DiaSemana", "diasemana");
                            sqlBulkCopy.ColumnMappings.Add("ValorDia", "valordia");
                            sqlBulkCopy.ColumnMappings.Add("MO", "mo");
                            sqlBulkCopy.ColumnMappings.Add("TipoEmbarque", "tipoembarque");
                            sqlBulkCopy.ColumnMappings.Add("FechaEmbarque", "fechaembarque");
                            sqlBulkCopy.ColumnMappings.Add("QTYPiezas", "qtypiezas");
                            sqlBulkCopy.ColumnMappings.Add("QTYCajas", "qtycajas");
                            sqlBulkCopy.ColumnMappings.Add("QTYPallets", "qtypallets");                            

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

        [HttpPost]
        public ActionResult ListaKpis()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var Mes = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<kpisdespacho> listaKPIS = new List<kpisdespacho>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [dbo].[SP_KPISDESPACHO_PARAMETROSOPCIONALES] @mes";

                    var query = new SqlCommand(sql, con);

                    if (Mes != "")
                    {
                        query.Parameters.AddWithValue("@mes", Mes);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@mes", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var kpis = new kpisdespacho();

                            kpis.id = Convert.ToInt32(dr["id"]);
                            kpis.mes = dr["mes"].ToString();
                            kpis.wk = Convert.ToInt32(dr["wk"]);
                            kpis.diasemana = Convert.ToInt32(dr["diasemana"]);
                            kpis.cv = dr["cv"].ToString();
                            kpis.mo = dr["mo"].ToString();
                            kpis.tipoembarque = dr["tipoembarque"].ToString();
                            kpis.fechaembarque = Convert.ToDateTime(dr["fechaembarque"]);
                            kpis.qtypiezas = Convert.ToInt32(dr["qtypiezas"]);
                            kpis.qtycajas = Convert.ToInt32(dr["qtycajas"]);
                            kpis.qtypallets = Convert.ToDecimal(dr["qtypallets"]);

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
                Console.WriteLine(_ex.Message.ToString());
                ViewBag.Error = "Ha ocurrido un error. Contacte al administrador del sistema";
                return RedirectToAction("Error500", "Errores");
            }
        }

        public ActionResult Resumen()
        {
            try
            {
                Totales();
                return PartialView(DatosGrafica());
            }
            catch (Exception)
            {
                ViewBag.Error = "Ha ocurrido un error. Contacte al administrador del sistema.";
                return RedirectToAction("Error500", "Errores");               
            }            
        }

        public List<kpisdespacho> DatosGrafica()
        {
            var query = db.kpisdespacho.ToList().OrderBy(x => x.wk).GroupBy(x => new { x.mes, x.wk, x.diasemana, x.valordia })
                .Select(y => new kpisdespacho()
                {
                    mes = y.Key.mes,
                    diasemana = y.Key.diasemana,
                    wk = y.Key.wk,
                    valordia = y.Key.valordia,
                    qtypallets = y.Sum(x => x.qtypallets),
                });

            List<kpisdespacho> lista2 = new List<kpisdespacho>();

            foreach (var item in query)
            {
                kpisdespacho kpisdespacho = new kpisdespacho();
                kpisdespacho.mes = item.mes;
                kpisdespacho.diasemana = item.diasemana;
                kpisdespacho.wk = item.wk;
                kpisdespacho.valordia = item.valordia;
                kpisdespacho.qtypallets = item.qtypallets;

                lista2.Add(kpisdespacho);
            }

            List<kpisdespacho> lista3 = new List<kpisdespacho>();

            foreach (var item in lista2.GroupBy(x => x.mes))
            {
                kpisdespacho kpisdespacho = new kpisdespacho();

                kpisdespacho.mes = item.Key;
                kpisdespacho.qtypallets = item.Sum(x => x.qtypallets);
                kpisdespacho.valordia = item.Sum(x => x.valordia);

                if (item.Sum(x => x.qtypallets) > 0)
                {
                    var resultado = ((decimal)item.Sum(x => x.qtypallets) / (decimal)(int)item.Sum(x => x.valordia));
                    var porcentajeRounded = Math.Round(resultado, 2);
                    kpisdespacho.sumadias = porcentajeRounded;
                }
                else
                {
                    kpisdespacho.sumadias = 0;
                }

                lista3.Add(kpisdespacho);
            }
            return lista3.ToList();
        }

        public ActionResult Graficas(string mes)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            foreach (var item in DatosGrafica())
            {
                dataPoints.Add(new DataPoint( item.mes, (double)item.qtypallets));
            }
            ViewBag.DataPointss = JsonConvert.SerializeObject(dataPoints);
                        
            return PartialView();
        }

        public void Totales()
        {
            var query = db.kpisdespacho.ToList().OrderBy(x => x.wk).GroupBy(x => new { x.mes, x.wk, x.diasemana, x.valordia })
                .Select(y => new kpisdespacho()
                {
                    mes = y.Key.mes,
                    diasemana = y.Key.diasemana,
                    wk = y.Key.wk,
                    valordia = y.Key.valordia,
                    qtypallets = y.Sum(x => x.qtypallets),
                });

            decimal? valorPallets = 0;
            int? valorHoras = 0;
            foreach (var item in query)
            {
                valorPallets += item.qtypallets;
                valorHoras += item.valordia;
            }

            ViewBag.SumaPallets = valorPallets;
            ViewBag.SumaHoras = valorHoras;
            var resultado = ((decimal)valorPallets / (decimal)(int)valorHoras);
            var porcentajeRounded = Math.Round(resultado, 2);
            ViewBag.TotalDivision = porcentajeRounded;            
        }

        [Authorize(Roles = "admin, coordinadoroperaciones")]
        public ActionResult Delete()
        {
            return View();
        }

        [Authorize(Roles = "admin, coordinadoroperaciones")]
        [HttpPost, ActionName("Delete")]
        public ActionResult Delete(int semana)
        {
            List<kpisdespacho> lista = db.kpisdespacho.Where(x => x.wk == semana).ToList();

            foreach (var item in lista)
            {

                kpisdespacho kpi = db.kpisdespacho.Find(item.id);
                db.kpisdespacho.Remove(kpi);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}