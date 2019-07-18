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
    public class kpisreciboController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: kpisrecibo
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

            var WK = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var Mes = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();            
            var Orden = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<kpisrecibo> listaKPIS = new List<kpisrecibo>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [dbo].[SP_KPISRECIBO_PARAMETROSOPCIONALES] @mes, @wk, @orden";

                    var query = new SqlCommand(sql, con);

                    if (Mes != "")
                    {
                        query.Parameters.AddWithValue("@mes", Mes);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@mes", DBNull.Value);
                    }

                    if (WK != "")
                    {
                        query.Parameters.AddWithValue("@wk", WK);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@wk", DBNull.Value);
                    }

                    if (Orden != "")
                    {
                        query.Parameters.AddWithValue("@orden", Orden);
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
                            var kpis = new kpisrecibo();

                            kpis.id = Convert.ToInt32(dr["id"]);
                            kpis.fecha = Convert.ToDateTime(dr["fecha"]);
                            kpis.wk = Convert.ToInt32(dr["wk"]);
                            kpis.mes = dr["mes"].ToString();
                            kpis.proveedor = dr["proveedor"].ToString();
                            kpis.ordenguia = dr["ordenguia"].ToString();
                            kpis.descarga = dr["descarga"].ToString();
                            kpis.fechahoradescargastring = dr["fechahoradescarga"].ToString();
                            kpis.fechahoraingresosistemastring = dr["fechahoraingresosistema"].ToString();                            
                            kpis.tiempoaplicacionoracle = dr["tiempoaplicacionoracle"].ToString();
                            kpis.evaluacioningreso = dr["evaluacioningreso"].ToString();

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

        public ActionResult Resumen()
        {
            try
            {
                var kpi = from k in db.kpisrecibo
                          select new { k.id, evaluacion = k.evaluacioningreso, mes = k.mes };

                var listakpis = new List<kpisrecibo>();

                foreach (var item in kpi)
                {
                    kpisrecibo kpisplaneacion = new kpisrecibo();
                    kpisplaneacion.id = item.id;
                    kpisplaneacion.mes = item.mes;
                    kpisplaneacion.evaluacioningreso = item.evaluacion;
                    listakpis.Add(kpisplaneacion);
                }

                var kpisAgrupados = from k in listakpis
                                    group k by k.mes into g
                                    select new GourpMeses<string, kpisrecibo>
                                    {
                                        Key = g.Key,
                                        Values = g,
                                        TotalRecords = g.Count(x => x.evaluacioningreso.Contains("A Tiempo") || x.evaluacioningreso.Contains("Tarde")),
                                        TotalOks = g.Count(x => x.evaluacioningreso.Contains("A Tiempo")),
                                        TotalDesv = g.Count(x => x.evaluacioningreso.Contains("Tarde"))
                                    };

                List<kpisrecibo> lista = new List<kpisrecibo>();

                foreach (var item in kpisAgrupados)
                {
                    kpisrecibo kpisplaneacion = new kpisrecibo();
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

                    Totales();

                    lista.Add(kpisplaneacion);
                }

                return PartialView(lista.ToList());
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public ActionResult Graficas(string mes)
        {
            try
            {
                var ultimomes = db.kpisrecibo.OrderByDescending(x => x.id).First().mes;
                if (mes == "1" || mes == "2" || mes == "3" || mes == "4" || mes == "5" || mes == "6" || mes == "7" || mes == "8" || mes == "9" || mes == "10" || mes == "11" || mes == "12")
                {
                    ViewBag.DataPointsRecibo = JsonConvert.SerializeObject(Datos(ultimomes));
                }
                else if (mes == "Total")
                {
                    ViewBag.DataPointsRecibo = JsonConvert.SerializeObject(Datos(ultimomes));
                }
                else
                {
                    string meses = mes.Remove(0, 17).ToString();

                    meses.Trim();
                    if (meses.ToString() == "Enero\n            ")
                    {
                        meses = "Enero";
                    }
                    else if (meses.ToString() == "Febrero\n            ")
                    {
                        meses = "Febrero";
                    }
                    else if (meses.ToString() == "Marzo\n            ")
                    {
                        meses = "Marzo";
                    }
                    else if (meses.ToString() == "Abril\n            ")
                    {
                        meses = "Abril";
                    }
                    else if (meses.ToString() == "Mayo\n            ")
                    {
                        meses = "Mayo";
                    }
                    else if (meses.ToString() == "Junio\n            ")
                    {
                        meses = "Junio";
                    }
                    else if (meses.ToString() == "Julio\n            ")
                    {
                        meses = "Julio";
                    }
                    else if (meses.ToString() == "Agosto\n            ")
                    {
                        meses = "Agosto";
                    }
                    else if (meses.ToString() == "Septiembre\n            ")
                    {
                        meses = "Septiembre";
                    }
                    else if (meses.ToString() == "Octubre\n            ")
                    {
                        meses = "Octubre";
                    }
                    else if (meses.ToString() == "Noviembre\n            ")
                    {
                        meses = "Noviembre";
                    }
                    else if (meses.ToString() == "Diciembre\n            ")
                    {
                        meses = "Diciembre";
                    }                   

                    ViewBag.Mes = meses;

                    ViewBag.DataPointsRecibo = JsonConvert.SerializeObject(Datos(meses));
                }                
            }
            catch (Exception)
            {
                ViewBag.Error = "Ha ocurrido un error. Contacte al administrador del sistema.";
                return RedirectToAction("Error500", "Errores");
            }
            return PartialView();
        }

        private List<DataPoint> Datos(string mes)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            var registros = from k in db.kpisrecibo
                            where k.evaluacioningreso.Contains("A Tiempo") || k.evaluacioningreso.Contains("Tarde")
                            select new { k.id, mes = k.mes };

            var registrosOk = from k in db.kpisrecibo
                              where k.evaluacioningreso.Contains("A Tiempo")
                              select new { k.id, mes = k.mes };

            var registrosDesv = from k in db.kpisrecibo
                                where k.evaluacioningreso.Contains("Tarde")
                                select new { k.id, mes = k.mes };

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

                dataPoints.Add(new DataPoint("A Tiempo", dataOk));
                dataPoints.Add(new DataPoint("Tarde", dataDesv));
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

                    dataPoints.Add(new DataPoint("A Tiempo", dataOk));
                    dataPoints.Add(new DataPoint("Tarde", dataDesv));
                }
                else
                {
                    double dataOk = 0;
                    double dataDesv = 0;

                    dataPoints.Add(new DataPoint("A Tiempo", dataOk));
                    dataPoints.Add(new DataPoint("Tarde", dataDesv));
                }
            }

            return dataPoints;
        }

        private void Totales()
        {
            var registros = from k in db.kpisrecibo
                            where k.evaluacioningreso.Contains("A Tiempo") || k.evaluacioningreso.Contains("Tarde")
                            select new { k.id, mes = k.mes };

            var registrosOk = from k in db.kpisrecibo
                              where k.evaluacioningreso.Contains("A Tiempo")
                              select new { k.id, mes = k.mes };

            var registrosDesv = from k in db.kpisrecibo
                                where k.evaluacioningreso.Contains("Tarde")
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

        [Authorize(Roles = "admin, coordinadoroperaciones")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin, coordinadoroperaciones")]
        public ActionResult Delete()
        {
            return View();
        }

        [Authorize(Roles = "admin, coordinadoroperaciones")]
        public ActionResult Import(HttpPostedFileBase postedFileBase)
        {
            try
            {
                List<kpisrecibo> listarecibos = new List<kpisrecibo>();
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
                    dt.Columns.AddRange(new DataColumn[10] {
                        new DataColumn("Fecha", typeof(DateTime)),
                        new DataColumn("WK", typeof(int)),
                        new DataColumn("Mes", typeof(string)),
                        new DataColumn("Proveedor", typeof(string)),
                        new DataColumn("Guia", typeof(string)),
                        new DataColumn("Descarga", typeof(string)),
                        new DataColumn("FechaHoraDescarga", typeof(DateTime)),
                        new DataColumn("FechaHoraIngreso", typeof(DateTime)),
                        new DataColumn("TiempoAplicacion", typeof(string)),
                        new DataColumn("Evaluacion", typeof(string))
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
                            sqlBulkCopy.DestinationTableName = "dbo.kpisrecibo";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table                            
                            sqlBulkCopy.ColumnMappings.Add("Fecha", "fecha");
                            sqlBulkCopy.ColumnMappings.Add("WK", "wk");
                            sqlBulkCopy.ColumnMappings.Add("Mes", "mes");
                            sqlBulkCopy.ColumnMappings.Add("Proveedor", "proveedor");
                            sqlBulkCopy.ColumnMappings.Add("Guia", "ordenguia");
                            sqlBulkCopy.ColumnMappings.Add("Descarga", "descarga");
                            sqlBulkCopy.ColumnMappings.Add("FechaHoraDescarga", "fechahoradescarga");
                            sqlBulkCopy.ColumnMappings.Add("FechaHoraIngreso", "fechahoraingresosistema");
                            sqlBulkCopy.ColumnMappings.Add("TiempoAplicacion", "tiempoaplicacionoracle");
                            sqlBulkCopy.ColumnMappings.Add("Evaluacion", "evaluacioningreso");

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

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin, coordinadoroperaciones")]
        public ActionResult Delete(int semana)
        {
            List<kpisrecibo> lista = db.kpisrecibo.Where(x => x.wk == semana).ToList();

            foreach (var item in lista)
            {
                kpisrecibo kpi = db.kpisrecibo.Find(item.id);
                db.kpisrecibo.Remove(kpi);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}