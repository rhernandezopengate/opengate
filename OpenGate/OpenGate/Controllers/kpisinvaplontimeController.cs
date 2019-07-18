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
    public class kpisinvaplontimeController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: kpisinvaplontime
        public ActionResult Index()
        {
            return View();
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
                List<kpisinventariosaplontime> customersModels = new List<kpisinventariosaplontime>();
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
                    dt.Columns.AddRange(new DataColumn[17] {
                        new DataColumn("Mes", typeof(string)),
                        new DataColumn("FechaReciboFisico", typeof(DateTime)),
                        new DataColumn("FechaTerminoValidacion", typeof(DateTime)),
                        new DataColumn("DiasValidacion", typeof(int)),
                        new DataColumn("FechaCreacionOracle", typeof(DateTime)),
                        new DataColumn("FechaReciboOracle",typeof(DateTime)),
                        new DataColumn("DiasOracle", typeof(int)),
                        new DataColumn("FolioFacturacion", typeof(string)),
                        new DataColumn("Observaciones", typeof(string)),
                        new DataColumn("CV", typeof(string)),
                        new DataColumn("Number", typeof(string)),
                        new DataColumn("ordernumber", typeof(string)),
                        new DataColumn("piezassistema", typeof(string)),
                        new DataColumn("piezasfisico", typeof(string)),
                        new DataColumn("palletsrecibidos", typeof(string)),
                        new DataColumn("montofacturado", typeof(string)),
                        new DataColumn("ingresado", typeof(string))
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
                            sqlBulkCopy.DestinationTableName = "dbo.kpisinventariosaplontime";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table                            
                            sqlBulkCopy.ColumnMappings.Add("Mes", "Mes");
                            sqlBulkCopy.ColumnMappings.Add("FechaReciboFisico", "FechaReciboFisico");
                            sqlBulkCopy.ColumnMappings.Add("FechaTerminoValidacion", "FechaTerminoValidacion");
                            sqlBulkCopy.ColumnMappings.Add("DiasValidacion", "DiasValidacion");
                            sqlBulkCopy.ColumnMappings.Add("FechaCreacionOracle", "FechaCreacionOracle");
                            sqlBulkCopy.ColumnMappings.Add("FechaReciboOracle", "FechaReciboOracle");
                            sqlBulkCopy.ColumnMappings.Add("DiasOracle", "DiasOracle");
                            sqlBulkCopy.ColumnMappings.Add("FolioFacturacion", "FolioFacturacion");
                            sqlBulkCopy.ColumnMappings.Add("Observaciones", "Observaciones");
                            sqlBulkCopy.ColumnMappings.Add("CV", "CV");
                            sqlBulkCopy.ColumnMappings.Add("Number", "Number");
                            sqlBulkCopy.ColumnMappings.Add("ordernumber", "ordernumber");
                            sqlBulkCopy.ColumnMappings.Add("piezassistema", "piezassistema");
                            sqlBulkCopy.ColumnMappings.Add("piezasfisico", "piezasfisico");
                            sqlBulkCopy.ColumnMappings.Add("palletsrecibidos", "palletsrecibidos");
                            sqlBulkCopy.ColumnMappings.Add("montofacturado", "montofacturado");
                            sqlBulkCopy.ColumnMappings.Add("ingresado", "ingresado");

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
                List<kpisinventariosaplontime> listaKPIS = new List<kpisinventariosaplontime>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [dbo].[SP_KPISINVENTARIOSAPLONTIME_PARAMETROSOPCIONALES] @mes";

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
                            var kpis = new kpisinventariosaplontime();

                            kpis.id = Convert.ToInt32(dr["id"]);
                            kpis.Mes = dr["Mes"].ToString();
                            kpis.FechaReciboFisico = Convert.ToDateTime(dr["FechaReciboFisico"]);
                            kpis.FechaTerminoValidacion = Convert.ToDateTime(dr["FechaTerminoValidacion"]);
                            kpis.DiasValidacion = Convert.ToInt32(dr["DiasValidacion"]);
                            kpis.FechaCreacionOracle = Convert.ToDateTime(dr["FechaCreacionOracle"]);
                            kpis.FechaReciboOracle = Convert.ToDateTime(dr["FechaReciboOracle"]);
                            kpis.DiasOracle = Convert.ToInt32(dr["DiasOracle"]);
                            kpis.FolioFacturacion = dr["FolioFacturacion"].ToString();
                            kpis.Observaciones = dr["Observaciones"].ToString();
                            kpis.CV = dr["CV"].ToString();
                            kpis.Number = dr["Number"].ToString();
                            kpis.ordernumber = dr["ordernumber"].ToString();
                            kpis.piezassistema = Convert.ToInt32(dr["piezassistema"]);
                            kpis.piezasfisico = Convert.ToInt32(dr["piezasfisico"]);
                            kpis.palletsrecibidos = Convert.ToInt32(dr["palletsrecibidos"]);
                            kpis.montofacturado = Convert.ToInt32(dr["montofacturado"]);
                            kpis.ingresado = dr["ingresado"].ToString();

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
            var kpi = from k in db.kpisinventariosaplontime                      
                      select new { k.id, dia = k.DiasOracle, mes = k.Mes };

            var listakpis = new List<kpisinventariosaplontime>();

            foreach (var item in kpi)
            {
                kpisinventariosaplontime kpis = new kpisinventariosaplontime();
                kpis.id = item.id;
                kpis.Mes = item.mes;
                kpis.DiasOracle = item.dia;
                listakpis.Add(kpis);
            }            

            var kpisAgrupados = from k in listakpis
                                group k by k.Mes into g
                                select new GourpMeses<string, kpisinventariosaplontime>
                                {
                                    Key = g.Key,
                                    Values = g,
                                    TotalRecords = g.Count(x => x.DiasOracle > 0),
                                    TotalOks = g.Count(x => x.DiasOracle <= 7 && x.DiasOracle > 0),
                                    TotalDesv = g.Count(x => x.DiasOracle > 7)
                                };

            List<kpisinventariosaplontime> lista = new List<kpisinventariosaplontime>();

            foreach (var item in kpisAgrupados)
            {
                kpisinventariosaplontime kpisplaneacion = new kpisinventariosaplontime();
                kpisplaneacion.Mes = item.Key;
                kpisplaneacion.Total = (int)item.TotalRecords;
                kpisplaneacion.TotalOks = (int)item.TotalOks;
                kpisplaneacion.TotalDesv = (int)item.TotalDesv;

                if (item.TotalOks > 0)
                {
                    var porcentajeOk = ((int)item.TotalOks / (decimal)(int)item.TotalRecords) * 100;
                    var porcentajeDesvRounded = Math.Round(porcentajeOk, 2);
                    kpisplaneacion.procentaje = porcentajeDesvRounded;
                }
                else
                {
                    kpisplaneacion.procentaje = 0;
                }

                lista.Add(kpisplaneacion);
            }

            Totales();

            return PartialView(lista.ToList());
        }

        public ActionResult Graficas(string mes)
        {
            try
            {
                var ultimomes = db.kpisinventariosaplontime.OrderByDescending(x => x.id).First().Mes;
                if (mes == "1" || mes == "2" || mes == "3" || mes == "4" || mes == "5" || mes == "6" || mes == "7" || mes == "8" || mes == "9" || mes == "10" || mes == "11" || mes == "12")
                {
                    ViewBag.DataPointsAplOnTime = JsonConvert.SerializeObject(Datos(ultimomes));
                }
                else
                {
                    string meses = mes.Remove(0, 17).ToString();

                    meses.Trim();
                    if (meses.ToString() == "Enero\n\n            ")
                    {
                        meses = "Enero";
                    }                    
                    else if (meses.ToString() == "Febrero\n\n            ")
                    {
                        meses = "Febrero";
                    }
                    else if (meses.ToString() == "Marzo\n\n            ")
                    {
                        meses = "Marzo";
                    }
                    else if (meses.ToString() == "Abril\n\n            ")
                    {
                        meses = "Abril";
                    }
                    else if (meses.ToString() == "Mayo\n\n            ")
                    {
                        meses = "Mayo";
                    }
                    else if (meses.ToString() == "Junio\n\n            ")
                    {
                        meses = "Junio";
                    }
                    else if (meses.ToString() == "Julio\n\n            ")
                    {
                        meses = "Julio";
                    }
                    else if (meses.ToString() == "Agosto\n\n            ")
                    {
                        meses = "Agosto";
                    }
                    else if (meses.ToString() == "Septiembre\n\n            ")
                    {
                        meses = "Septiembre";
                    }
                    else if (meses.ToString() == "Octubre\n\n            ")
                    {
                        meses = "Octubre";
                    }
                    else if (meses.ToString() == "Noviembre\n\n            ")
                    {
                        meses = "Noviembre";
                    }
                    else if (meses.ToString() == "Diciembre\n\n            ")
                    {
                        meses = "Diciembre";
                    }
                    else if (meses.ToString() == "Total\n            ")
                    {
                        meses = "Total";
                    }

                    ViewBag.Mes = meses;

                    ViewBag.DataPointsAplOnTime = JsonConvert.SerializeObject(Datos(meses));
                }                
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                throw;
            }
            return PartialView();
        }

        private List<DataPoint> Datos(string mes)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            var registros = from k in db.kpisinventariosaplontime                            
                            select new { k.id, mes = k.Mes };

            var registrosOk = from k in db.kpisinventariosaplontime
                              where k.DiasOracle <= 7 
                              select new { k.id, mes = k.Mes };

            var registrosDesv = from k in db.kpisinventariosaplontime
                                where k.DiasOracle > 7
                                select new { k.id, mes = k.Mes };

            if (mes.Equals("Total"))
            {
                var totalRegistros = (decimal)registros.Where(x => x.id > 12).Select(x => x.id).Count();

                var totalOks = (int)registrosOk.Where(x => x.id > 12).Select(x => x.id).Count();

                var totalDesv = (int)registrosDesv.Where(x => x.id > 12).Select(x => x.id).Count();

                var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;

                var porcentajedesv = (totalDesv / totalRegistros) * 100;

                var porcentajeokRounded = Math.Round(porcentajeok, 2);

                var porcentajeDesvRounded = Math.Round(porcentajedesv, 2);

                double dataOk = (double)Math.Round(porcentajeokRounded, 2);
                double dataDesv = (double)Math.Round(porcentajeDesvRounded, 2);

                dataPoints.Add(new DataPoint("Ok", dataOk));
                dataPoints.Add(new DataPoint("Desv", dataDesv));
            }
            else
            {

                var totalRegistros = (decimal)registros.Where(x => x.mes.Contains(mes) && x.id > 12).Select(x => x.id).Count();

                if (totalRegistros > 0)
                {
                    var totalOks = (int)registrosOk.Where(x => x.mes.Contains(mes) && x.id > 12).Select(x => x.id).Count();

                    var totalDesv = (int)registrosDesv.Where(x => x.mes.Contains(mes) && x.id > 12).Select(x => x.id).Count();

                    var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;

                    var porcentajedesv = (totalDesv / totalRegistros) * 100;

                    var porcentajeokRounded = Math.Round(porcentajeok, 2);

                    var porcentajeDesvRounded = Math.Round(porcentajedesv, 2);

                    double dataOk = (double)Math.Round(porcentajeokRounded, 2);
                    double dataDesv = (double)Math.Round(porcentajeDesvRounded, 2);

                    dataPoints.Add(new DataPoint("Ok", dataOk));
                    dataPoints.Add(new DataPoint("Desv", dataDesv));
                }
                else
                {
                    double dataOk = 0;
                    double dataDesv = 0;

                    dataPoints.Add(new DataPoint("Ok", dataOk));
                    dataPoints.Add(new DataPoint("Desv", dataDesv));
                }
            }

            return dataPoints;
        }

        private void Totales()
        {
            var registros = from k in db.kpisinventariosaplontime
                            select new { k.id, mes = k.Mes };

            var registrosOk = from k in db.kpisinventariosaplontime
                              where k.DiasOracle <= 7 
                              select new { k.id, mes = k.Mes };

            var registrosDesv = from k in db.kpisinventariosaplontime
                                where k.DiasOracle > 7 
                                select new { k.id, mes = k.Mes };

            var totalRegistros = (decimal)registros.Where(x => x.id > 12).Select(x => x.id).Count();

            var totalOks = (int)registrosOk.Where(x => x.id > 12).Select(x => x.id).Count();

            var totalDesv = (int)registrosDesv.Where(x => x.id > 12).Select(x => x.id).Count();

            var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;

            var porcentajeokRounded = Math.Round(porcentajeok, 2);

            double dataOk = (double)Math.Round(porcentajeokRounded, 2);

            ViewBag.TotalAp = totalRegistros;
            ViewBag.TotalOkAp = totalOks;
            ViewBag.TotalDesvAp = totalDesv;
            ViewBag.PorcentajeOkAp = dataOk;
        }
    }
}