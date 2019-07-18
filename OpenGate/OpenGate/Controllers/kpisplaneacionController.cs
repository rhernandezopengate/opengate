using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OpenGate.Entidades;
using System.Linq.Dynamic;

namespace OpenGate.Controllers
{
    [Authorize]
    public class kpisplaneacionController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
                
        // GET: kpisplaneacion
        public ActionResult Resumen()
        {
            try
            {
                var kpi = from k in db.kpisplaneacion
                          select new { k.id, status = k.status, mes = k.mes };

                var listakpis = new List<kpisplaneacion>();

                foreach (var item in kpi)
                {
                    kpisplaneacion kpisplaneacion = new kpisplaneacion();
                    kpisplaneacion.id = item.id;
                    kpisplaneacion.mes = item.mes;
                    kpisplaneacion.status = item.status;
                    listakpis.Add(kpisplaneacion);
                }

                var kpisAgrupados = from k in listakpis
                                    group k by k.mes into g
                                    select new GourpMeses<string, kpisplaneacion>
                                    {
                                        Key = g.Key,
                                        Values = g,
                                        TotalRecords = g.Count(x => x.status.Contains("OK") || x.status.Contains("Desv")),
                                        TotalOks = g.Count(x => x.status.Contains("OK")),
                                        TotalDesv = g.Count(x => x.status.Contains("Desv"))
                                    };

                List<kpisplaneacion> lista = new List<kpisplaneacion>();

                foreach (var item in kpisAgrupados)
                {
                    kpisplaneacion kpisplaneacion = new kpisplaneacion();
                    kpisplaneacion.mes = item.Key;
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
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        private List<DataPoint> Datos(string mes)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            var registros = from k in db.kpisplaneacion
                            where k.status.Contains("Ok") || k.status.Contains("Desv")
                            select new { k.id, mes = k.mes };

            var registrosOk = from k in db.kpisplaneacion
                              where k.status.Contains("OK")
                              select new { k.id, mes = k.mes };

            var registrosDesv = from k in db.kpisplaneacion
                                where k.status.Contains("Desv")
                                select new { k.id, mes = k.mes };

            ViewBag.mes = mes;

            if (mes.Equals("Total"))
            {
                var totalRegistros = (decimal)registros.Select(x => x.id).Count();

                var totalOks = (int)registrosOk.Select(x => x.id).Count();

                var totalDesv = (int)registrosDesv.Select(x => x.id).Count();

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

                var totalRegistros = (decimal)registros.Where(x => x.mes.Contains(mes)).Select(x => x.id).Count();

                if (totalRegistros > 0)
                {
                    var totalOks = (int)registrosOk.Where(x => x.mes.Contains(mes)).Select(x => x.id).Count();

                    var totalDesv = (int)registrosDesv.Where(x => x.mes.Contains(mes)).Select(x => x.id).Count();

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
            var registros = from k in db.kpisplaneacion
                            where k.status.Contains("Ok") || k.status.Contains("Desv")
                            select new { k.id, mes = k.mes };

            var registrosOk = from k in db.kpisplaneacion
                              where k.status.Contains("OK")
                              select new { k.id, mes = k.mes };

            var registrosDesv = from k in db.kpisplaneacion
                                where k.status.Contains("Desv")
                                select new { k.id, mes = k.mes };

            var totalRegistros = (decimal)registros.Select(x => x.id).Count();

            var totalOks = (int)registrosOk.Select(x => x.id).Count();

            var totalDesv = (int)registrosDesv.Select(x => x.id).Count();

            var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;

            var porcentajeokRounded = Math.Round(porcentajeok, 2);

            double dataOk = (double)Math.Round(porcentajeokRounded, 2);

            ViewBag.TotalAplicaciones = totalRegistros;
            ViewBag.TotalOk = totalOks;
            ViewBag.TotalDesv = totalDesv;
            ViewBag.PorcentajeOk = dataOk;
        }

        public ActionResult Graficas(string mes)
        {
            try
            {
                var ultimomes = db.kpisplaneacion.OrderByDescending(x => x.id).First().mes;
                if (mes == "1" || mes == "2" || mes == "3"|| mes == "4" || mes == "5" || mes == "6" || mes == "7" || mes == "8" || mes == "9" || mes == "10" || mes == "11" || mes == "12")
                {
                    ViewBag.DataPoints = JsonConvert.SerializeObject(Datos(ultimomes));
                }
                else
                {
                    string meses = mes.Remove(0, 21).ToString();

                    meses.Trim();
                    if (meses.ToString() == "Enero\n                ")
                    {
                        meses = "Enero";
                    }
                    else if (meses.ToString() == "Febrero\n                ")
                    {
                        meses = "Febrero";
                    }
                    else if (meses.ToString() == "Marzo\n                ")
                    {
                        meses = "Marzo";
                    }
                    else if (meses.ToString() == "Abril\n                ")
                    {
                        meses = "Abril";
                    }
                    else if (meses.ToString() == "Mayo\n                ")
                    {
                        meses = "Mayo";
                    }
                    else if (meses.ToString() == "Junio\n                ")
                    {
                        meses = "Junio";
                    }
                    else if (meses.ToString() == "Julio\n                ")
                    {
                        meses = "Julio";
                    }
                    else if (meses.ToString() == "Agosto\n                ")
                    {
                        meses = "Agosto";
                    }
                    else if (meses.ToString() == "Septiembre\n                ")
                    {
                        meses = "Septiembre";
                    }
                    else if (meses.ToString() == "Octubre\n                ")
                    {
                        meses = "Octubre";
                    }
                    else if (meses.ToString() == "Noviembre\n                ")
                    {
                        meses = "Noviembre";
                    }
                    else if (meses.ToString() == "Diciembre\n                ")
                    {
                        meses = "Diciembre";
                    }
                    else if (meses.ToString() == "l\n            ")
                    {
                        meses = "Total";
                    }
                    ViewBag.DataPoints = JsonConvert.SerializeObject(Datos(meses));
                }                          
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                throw;
            }
            return PartialView();
        }

        public ActionResult Kpisplaneacion()
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

            var Mes = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var WK = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();            
            var Requisicion = Request.Form.GetValues("columns[8][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<kpisplaneacion> listaKPIS = new List<kpisplaneacion>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [dbo].[SP_KPISPLANEACIONES_PARAMETROSOPCIONALES] @mes, @wk, @requisicion";

                    var query = new SqlCommand(sql, con);

                    if (Mes != "")
                    {
                        query.Parameters.AddWithValue("@mes", Mes);
                    }                    
                    else
                    {
                        query.Parameters.AddWithValue("@mes", DBNull.Value);
                    }
                    
                    if (Requisicion != "")
                    {
                        query.Parameters.AddWithValue("@requisicion", Requisicion);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@requisicion", DBNull.Value);
                    }

                    if (WK != "")
                    {
                        query.Parameters.AddWithValue("@wk", WK);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@wk", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var kpis = new kpisplaneacion();

                            kpis.id = Convert.ToInt32(dr["id"]);                            
                            kpis.mes = dr["mes"].ToString();
                            kpis.ano = dr["ano"].ToString();
                            kpis.fechaembarque = Convert.ToDateTime(dr["fechaembarque"]);
                            kpis.wk = Convert.ToInt32(dr["wk"]);
                            kpis.cv = dr["cv"].ToString();
                            kpis.mo = dr["mo"].ToString();
                            kpis.subinventario = dr["subinventario"].ToString();
                            kpis.requisicion = dr["requisicion"].ToString();
                            kpis.orden = dr["orden"].ToString();
                            kpis.aplicacion = dr["aplicacion"].ToString();
                            kpis.fechageneracion = Convert.ToDateTime(dr["fechageneracion"]);
                            kpis.fechaentrega = Convert.ToDateTime(dr["fechaentrega"]);
                            kpis.tipoembarque = dr["tipoembarque"].ToString();                            
                            kpis.status = dr["status"].ToString();

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
            catch (Exception)
            {

                throw;
            }
        }

        [Authorize(Roles = "admin, coordinadorplaneacion")]
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [Authorize(Roles = "admin, coordinadorplaneacion")]
        public ActionResult Import(HttpPostedFileBase postedFileBase)
        {
            try
            {
                List<kpisplaneacion> customersModels = new List<kpisplaneacion>();
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
                    dt.Columns.AddRange(new DataColumn[14] {
                                new DataColumn("Mes", typeof(string)),
                                new DataColumn("Ano", typeof(string)),
                                new DataColumn("FechaEmbarque", typeof(DateTime)),
                                new DataColumn("WK", typeof(int)),
                                new DataColumn("CV", typeof(string)),
                                new DataColumn("MO", typeof(string)),
                                new DataColumn("SubInventario", typeof(string)),
                                new DataColumn("Requisicion", typeof(string)),
                                new DataColumn("Orden", typeof(string)),
                                new DataColumn("Aplicacion", typeof(string)),
                                new DataColumn("FechaGeneracion", typeof(DateTime)),
                                new DataColumn("FechaEntrega", typeof(DateTime)),
                                new DataColumn("TipoEmbarque", typeof(string)),                                
                                new DataColumn("Status",typeof(string)) });

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
                            sqlBulkCopy.DestinationTableName = "dbo.KpisPlaneacion";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table                            
                            sqlBulkCopy.ColumnMappings.Add("Mes", "mes");
                            sqlBulkCopy.ColumnMappings.Add("Ano", "ano");
                            sqlBulkCopy.ColumnMappings.Add("FechaEmbarque", "fechaembarque");
                            sqlBulkCopy.ColumnMappings.Add("WK", "wk");
                            sqlBulkCopy.ColumnMappings.Add("CV", "cv");
                            sqlBulkCopy.ColumnMappings.Add("MO", "mo");
                            sqlBulkCopy.ColumnMappings.Add("SubInventario", "subinventario");
                            sqlBulkCopy.ColumnMappings.Add("Requisicion", "requisicion");
                            sqlBulkCopy.ColumnMappings.Add("Orden", "orden");
                            sqlBulkCopy.ColumnMappings.Add("Aplicacion", "aplicacion");
                            sqlBulkCopy.ColumnMappings.Add("FechaGeneracion", "fechageneracion");
                            sqlBulkCopy.ColumnMappings.Add("FechaEntrega", "fechaentrega");
                            sqlBulkCopy.ColumnMappings.Add("TipoEmbarque", "tipoembarque");                            
                            sqlBulkCopy.ColumnMappings.Add("Status", "status");

                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            con.Close();
                        }
                    }
                }

                return RedirectToAction("Kpisplaneacion");
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                throw;
            }
        }

        [Authorize(Roles = "admin, coordinadorplaneacion")]
        public ActionResult Delete()
        {
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin, coordinadorplaneacion")]
        public ActionResult Delete(int semana)
        {
            List<kpisplaneacion> lista = db.kpisplaneacion.Where(x => x.wk == semana).ToList();

            foreach (var item in lista)
            {

                kpisplaneacion kpi = db.kpisplaneacion.Find(item.id);
                db.kpisplaneacion.Remove(kpi);
                db.SaveChanges();
            }
            return RedirectToAction("Kpisplaneacion");
        }
    }
}