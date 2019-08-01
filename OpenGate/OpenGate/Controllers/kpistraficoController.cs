using Newtonsoft.Json;
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
using System.Globalization;
using System.Net.Mime;
using System.Windows.Forms.Design.Behavior;

namespace OpenGate.Controllers
{
    [Authorize]
    public class kpistraficoController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: kpistrafico
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

            var Remision = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var Destino = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
            var TipoRequerimiento = Request.Form.GetValues("columns[9][search][value]").FirstOrDefault();            

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<kpistrafico> listaKPIS = new List<kpistrafico>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [dbo].[SP_KPISTRAFICO_PARAMETROSOPCIONALES] @remision, @destino, @tiporequerimiento";

                    var query = new SqlCommand(sql, con);

                    if (Remision != "")
                    {
                        query.Parameters.AddWithValue("@remision", Remision);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@remision", DBNull.Value);
                    }

                    if (Destino != "")
                    {
                        query.Parameters.AddWithValue("@destino", Destino);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@destino", DBNull.Value);
                    }

                    if (TipoRequerimiento == "" || TipoRequerimiento == "0")
                    {
                        query.Parameters.AddWithValue("@tiporequerimiento", DBNull.Value);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@tiporequerimiento", TipoRequerimiento);                        
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var kpis = new kpistrafico();

                            kpis.id = Convert.ToInt32(dr["id"]);
                            kpis.remision = dr["remision"].ToString();
                            kpis.consecutivoflete = dr["consecutivoflete"].ToString();
                            kpis.respnsableflete = dr["respnsableflete"].ToString();
                            kpis.mescarga = dr["mescarga"].ToString();
                            kpis.origen = dr["origen"].ToString();
                            kpis.destino = dr["destino"].ToString();
                            kpis.wh = dr["wh"].ToString();
                            kpis.ruta = dr["ruta"].ToString();
                            kpis.tiporequerimiento = dr["tiporequerimiento"].ToString();
                            kpis.wk = Convert.ToInt32(dr["wk"]);
                            kpis.fechacarga = Convert.ToDateTime(dr["fechacarga"]);
                            kpis.fechaentrega = Convert.ToDateTime(dr["fechaentrega"]);
                            kpis.ro = dr["ro"].ToString();
                            kpis.tipoenvio = dr["tipoenvio"].ToString();
                            kpis.tipounidad = dr["tipounidad"].ToString();
                            kpis.capasidadpalletsunidad = decimal.Parse(dr["capasidadpalletsunidad"].ToString());
                            kpis.palletsembarcados = decimal.Parse(dr["palletsembarcados"].ToString());
                            kpis.porcentajeocupacionunidad = (decimal.Parse(dr["porcentajeocupacionunidad"].ToString()) * 100);
                            kpis.capasidadunidad = decimal.Parse(dr["capasidadunidad"].ToString());
                            kpis.pesoinventario = decimal.Parse(dr["pesoinventario"].ToString());
                            kpis.porcentajepesounidad = (decimal.Parse(dr["porcentajepesounidad"].ToString()) * 100);
                            kpis.evaluacionocupacionunidad = dr["evaluacionocupacionunidad"].ToString();
                            kpis.numerocajas = int.Parse(dr["numerocajas"].ToString());
                            kpis.numeropiezas = int.Parse(dr["numeropiezas"].ToString());
                            kpis.FacturacionFleteString = String.Format("{0:C}", decimal.Parse(dr["costoflete"].ToString()));
                            kpis.FacturacionManiobrasString = String.Format("{0:C}", decimal.Parse(dr["costomaniobras"].ToString()));
                            kpis.fhrequeridaposicionamientoString = Convert.ToDateTime(dr["fhrequeridaposicionamiento"]).ToString();
                            kpis.fhrealpsocicionamientoString = Convert.ToDateTime(dr["fhrealpsocicionamiento"]).ToString();
                            kpis.evaluacionposicionamiento = dr["evaluacionposicionamiento"].ToString();
                            kpis.fhcitaString = Convert.ToDateTime(dr["fhcita"]).ToString();
                            kpis.fharriborealString = Convert.ToDateTime(dr["fharriboreal"]).ToString();
                            kpis.evaluacionentrega = dr["evaluacionentrega"].ToString();
                            kpis.maniobristasrequeridos = int.Parse(dr["maniobristasrequeridos"].ToString());
                            kpis.maniobristasatiempo = int.Parse(dr["maniobristasatiempo"].ToString());
                            kpis.evaluacionmaniobras = dr["evaluacionmaniobras"].ToString();
                            kpis.observaciones = dr["observaciones"].ToString();

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

                return new JsonResult()
                {                    
                    Data = new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems },                    
                    MaxJsonLength = Int32.MaxValue
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize(Roles = "admin, gerentegeneral, trafico")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin, gerentegeneral, trafico")]
        public ActionResult Delete()
        {
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin, gerentegeneral, trafico")]
        public ActionResult Delete(int semana)
        {
            List<kpistrafico> lista = db.kpistrafico.Where(x => x.wk == semana).ToList();

            foreach (var item in lista)
            {

                kpistrafico kpi = db.kpistrafico.Find(item.id);
                db.kpistrafico.Remove(kpi);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "admin, gerentegeneral, trafico")]
        public ActionResult Import(HttpPostedFileBase postedFileBase)
        {
            try
            {
                List<kpisdespacho> listaKpis = new List<kpisdespacho>();
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
                    dt.Columns.AddRange(new DataColumn[36] {
                        new DataColumn("Remision", typeof(string)),
                        new DataColumn("ConsecutivoFlete", typeof(string)),
                        new DataColumn("RespnsableFlete", typeof(string)),
                        new DataColumn("Mes", typeof(string)),
                        new DataColumn("Origen",typeof(string)),
                        new DataColumn("Destino", typeof(string)),
                        new DataColumn("WH", typeof(string)),
                        new DataColumn("Ruta", typeof(string)),
                        new DataColumn("TipoRequerimiento", typeof(string)),
                        new DataColumn("WK", typeof(int)),
                        new DataColumn("FechaCarga", typeof(DateTime)),
                        new DataColumn("FechaEntrega", typeof(DateTime)),
                        new DataColumn("RO", typeof(string)),
                        new DataColumn("TipoEnvio",typeof(string)),
                        new DataColumn("TipoUnidad", typeof(string)),
                        new DataColumn("CapasidadPalletsUnidad", typeof(decimal)),
                        new DataColumn("PalletsEmbarcados", typeof(decimal)),
                        new DataColumn("PorcentajeOcupacionUnidad", typeof(decimal)),
                        new DataColumn("CapasidadUnidad", typeof(decimal)),
                        new DataColumn("PesoInventario", typeof(decimal)),
                        new DataColumn("PorcentajePesoUnidad", typeof(decimal)),
                        new DataColumn("EvaluacionOcupacionUnidad", typeof(string)),
                        new DataColumn("NumeroCajas",typeof(int)), //Cambiar a entero
                        new DataColumn("NumeroPiezas", typeof(int)),
                        new DataColumn("CostoFlete", typeof(decimal)),
                        new DataColumn("CostoManiobras", typeof(decimal)),

                        new DataColumn("FHRequeridaPosicionamiento", typeof(DateTime)),
                        new DataColumn("FHRealpPocicionamiento", typeof(DateTime)),
                        new DataColumn("EvaluacionPosicionamiento", typeof(string)),

                        new DataColumn("FHCita", typeof(DateTime)),
                        new DataColumn("FHArriboReal", typeof(DateTime)),
                        new DataColumn("EvaluacionEntrega", typeof(string)),

                        new DataColumn("ManiobristasRequeridos", typeof(int)),
                        new DataColumn("ManiobristasATiempo", typeof(int)),
                        new DataColumn("EvaluacionManiobras", typeof(string)),
                        new DataColumn("Observaciones",typeof(string))
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
                            sqlBulkCopy.DestinationTableName = "dbo.kpistrafico";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table                            
                            sqlBulkCopy.ColumnMappings.Add("Remision", "remision");
                            sqlBulkCopy.ColumnMappings.Add("ConsecutivoFlete", "consecutivoflete");
                            sqlBulkCopy.ColumnMappings.Add("RespnsableFlete", "respnsableflete");
                            sqlBulkCopy.ColumnMappings.Add("Mes", "mescarga");
                            sqlBulkCopy.ColumnMappings.Add("Origen", "origen");
                            sqlBulkCopy.ColumnMappings.Add("Destino", "destino");
                            sqlBulkCopy.ColumnMappings.Add("WH", "wh");
                            sqlBulkCopy.ColumnMappings.Add("Ruta", "ruta");
                            sqlBulkCopy.ColumnMappings.Add("TipoRequerimiento", "tiporequerimiento");
                            sqlBulkCopy.ColumnMappings.Add("WK", "wk");
                            sqlBulkCopy.ColumnMappings.Add("FechaCarga", "fechacarga");
                            sqlBulkCopy.ColumnMappings.Add("FechaEntrega", "fechaentrega");
                            sqlBulkCopy.ColumnMappings.Add("RO", "ro");
                            sqlBulkCopy.ColumnMappings.Add("TipoEnvio", "tipoenvio");
                            sqlBulkCopy.ColumnMappings.Add("TipoUnidad", "tipounidad");
                            sqlBulkCopy.ColumnMappings.Add("CapasidadPalletsUnidad", "capasidadpalletsunidad");
                            sqlBulkCopy.ColumnMappings.Add("PalletsEmbarcados", "palletsembarcados");
                            sqlBulkCopy.ColumnMappings.Add("PorcentajeOcupacionUnidad", "porcentajeocupacionunidad");
                            sqlBulkCopy.ColumnMappings.Add("CapasidadUnidad", "capasidadunidad");
                            sqlBulkCopy.ColumnMappings.Add("PesoInventario", "pesoinventario");
                            sqlBulkCopy.ColumnMappings.Add("PorcentajePesoUnidad", "porcentajepesounidad");
                            sqlBulkCopy.ColumnMappings.Add("EvaluacionOcupacionUnidad", "evaluacionocupacionunidad");
                            sqlBulkCopy.ColumnMappings.Add("NumeroCajas", "numerocajas");
                            sqlBulkCopy.ColumnMappings.Add("NumeroPiezas", "numeropiezas");
                            sqlBulkCopy.ColumnMappings.Add("CostoFlete", "costoflete");
                            sqlBulkCopy.ColumnMappings.Add("CostoManiobras", "costomaniobras");
                            sqlBulkCopy.ColumnMappings.Add("FHRequeridaPosicionamiento", "fhrequeridaposicionamiento");
                            sqlBulkCopy.ColumnMappings.Add("FHRealpPocicionamiento", "fhrealpsocicionamiento");
                            sqlBulkCopy.ColumnMappings.Add("EvaluacionPosicionamiento", "evaluacionposicionamiento");
                            sqlBulkCopy.ColumnMappings.Add("FHCita", "fhcita");
                            sqlBulkCopy.ColumnMappings.Add("FHArriboReal", "fharriboreal");
                            sqlBulkCopy.ColumnMappings.Add("EvaluacionEntrega", "evaluacionentrega");
                            sqlBulkCopy.ColumnMappings.Add("ManiobristasRequeridos", "maniobristasrequeridos");
                            sqlBulkCopy.ColumnMappings.Add("ManiobristasATiempo", "maniobristasatiempo");
                            sqlBulkCopy.ColumnMappings.Add("EvaluacionManiobras", "evaluacionmaniobras");
                            sqlBulkCopy.ColumnMappings.Add("Observaciones", "observaciones");

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
        public JsonResult ListaTipoRequerimiento()
        {
            List<SelectListItem> listrequerimiento = new List<SelectListItem>();
            var query = db.kpistrafico.Where(x => x.id > 12).GroupBy(x => x.tiporequerimiento).ToList();

            foreach (var item in query.OrderBy(x => x.Key))
            {
                listrequerimiento.Add(new SelectListItem
                {
                    Value = item.Key,
                    Text = item.Key
                });
            }

            return Json(listrequerimiento);
        }

        #region On Time Entregas

        public ActionResult ResumenOnTimeEntregas()
        {
            try
            {
                var kpi = from k in db.kpistrafico
                          select new { k.id, status = k.evaluacionentrega, mes = k.mescarga };

                var listakpis = new List<kpistrafico>();

                foreach (var item in kpi)
                {
                    kpistrafico kpisTemp = new kpistrafico();
                    kpisTemp.id = item.id;
                    kpisTemp.mescarga = item.mes;
                    kpisTemp.evaluacionentrega = item.status;
                    listakpis.Add(kpisTemp);
                }

                var kpisAgrupados = from k in listakpis
                                    group k by k.mescarga into g
                                    select new GourpMeses<string, kpistrafico>
                                    {
                                        Key = g.Key,
                                        Values = g,
                                        TotalRecords = g.Count(x => x.evaluacionentrega.Contains("Ok") || x.evaluacionentrega.Contains("No")),
                                        TotalOks = g.Count(x => x.evaluacionentrega.Contains("Ok")),
                                        TotalDesv = g.Count(x => x.evaluacionentrega.Contains("No"))
                                    };


                List<kpistrafico> lista = new List<kpistrafico>();

                foreach (var item in kpisAgrupados)
                {
                    kpistrafico kpis = new kpistrafico();
                    kpis.mescarga = item.Key;
                    kpis.Total = (int)item.TotalRecords;
                    kpis.TotalOks = (int)item.TotalOks;
                    kpis.TotalDesv = (int)item.TotalDesv;

                    if (item.TotalOks > 0)
                    {
                        var porcentajeOk = ((int)item.TotalOks / (decimal)(int)item.TotalRecords) * 100;
                        var porcentajeDesvRounded = Math.Round(porcentajeOk, 2);
                        kpis.procentaje = porcentajeDesvRounded;
                    }
                    else
                    {
                        kpis.procentaje = 0;
                    }

                    lista.Add(kpis);
                }

                TotalesOnTime();

                return PartialView(lista.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult GraficaOnTimeEntregas()
        {
            ViewBag.MesOnTimeEntregas = db.kpistrafico.OrderByDescending(x => x.id).First().mescarga;
            ViewBag.DataPoints = JsonConvert.SerializeObject(DatosOnTime());

            return PartialView();
        }

        private List<DataPoint> DatosOnTime()
        {
            var query = db.kpistrafico.ToList().OrderBy(x => x.wk).GroupBy(x => x.mescarga)
                .Select(y => new kpistrafico()
                {
                    mescarga = y.Key,                                                            
                    numerocajas = y.Count(x => x.evaluacionentrega.Contains("Ok")),
                    numeropiezas = y.Count(x => x.evaluacionentrega.Contains("Ok") || x.evaluacionentrega.Contains("No"))
                });

            List<kpistrafico> lista = new List<kpistrafico>();
            List<DataPoint> dataPoints = new List<DataPoint>();

            foreach (var item in query)
            {
                kpistrafico kpis = new kpistrafico();

                kpis.mescarga = item.mescarga;
                double porcentajeRounded = 0;
                if (item.numeropiezas > 0)
                {
                    var resultado = ((decimal)item.numerocajas / (decimal)(int)item.numeropiezas) * 100;
                    porcentajeRounded = (double)Math.Round(resultado, 2);                    
                }                
                dataPoints.Add(new DataPoint(item.mescarga, (double)porcentajeRounded));
            }                

            return dataPoints;
        }

        private void TotalesOnTime()
        {
            var registros = from k in db.kpistrafico
                            where k.evaluacionentrega.Contains("Ok") || k.evaluacionentrega.Contains("No")
                            select new { k.id, mes = k.mescarga };

            var registrosOk = from k in db.kpistrafico
                              where k.evaluacionentrega.Contains("Ok")
                              select new { k.id, mes = k.mescarga };

            var registrosDesv = from k in db.kpistrafico
                                where k.evaluacionentrega.Contains("No")
                                select new { k.id, mes = k.mescarga };

            var totalRegistros = (decimal)registros.Select(x => x.id).Count();

            var totalOks = (int)registrosOk.Select(x => x.id).Count();

            var totalDesv = (int)registrosDesv.Select(x => x.id).Count();

            var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;

            var porcentajeokRounded = Math.Round(porcentajeok, 2);

            double dataOk = (double)Math.Round(porcentajeokRounded, 2);

            ViewBag.TotalOnTime = totalRegistros;
            ViewBag.TotalOkOnTime = totalOks;
            ViewBag.TotalDesvOnTime = totalDesv;
            ViewBag.PorcentajeOkOnTime = dataOk;
        }

        #endregion

        #region Evaluacion Maniobras

        public ActionResult ResumenEvaluacionManiobras()
        {
            try
            {
                var kpi = from k in db.kpistrafico
                          select new { k.id, status = k.evaluacionmaniobras, mes = k.mescarga };

                var listakpis = new List<kpistrafico>();

                foreach (var item in kpi)
                {
                    kpistrafico kpisTemp = new kpistrafico();
                    kpisTemp.id = item.id;
                    kpisTemp.mescarga = item.mes;
                    kpisTemp.evaluacionmaniobras = item.status;
                    listakpis.Add(kpisTemp);
                }

                var kpisAgrupados = from k in listakpis
                                    group k by k.mescarga into g
                                    select new GourpMeses<string, kpistrafico>
                                    {
                                        Key = g.Key,
                                        Values = g,
                                        TotalRecords = g.Count(x => x.evaluacionmaniobras.Contains("Ok") || x.evaluacionmaniobras.Contains("No")),
                                        TotalOks = g.Count(x => x.evaluacionmaniobras.Contains("Ok")),
                                        TotalDesv = g.Count(x => x.evaluacionmaniobras.Contains("No"))
                                    };


                List<kpistrafico> lista = new List<kpistrafico>();

                foreach (var item in kpisAgrupados)
                {
                    kpistrafico kpis = new kpistrafico();
                    kpis.mescarga = item.Key;
                    kpis.Total = (int)item.TotalRecords;
                    kpis.TotalOks = (int)item.TotalOks;
                    kpis.TotalDesv = (int)item.TotalDesv;

                    if (item.TotalOks > 0)
                    {
                        var porcentajeOk = ((int)item.TotalOks / (decimal)(int)item.TotalRecords) * 100;
                        var porcentajeDesvRounded = Math.Round(porcentajeOk, 2);
                        kpis.procentaje = porcentajeDesvRounded;
                    }
                    else
                    {
                        kpis.procentaje = 0;
                    }

                    TotalesEvalManiobras();

                    lista.Add(kpis);
                }

                return PartialView(lista.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void TotalesEvalManiobras()
        {
            var registros = from k in db.kpistrafico
                            where k.evaluacionmaniobras.Contains("Ok") || k.evaluacionmaniobras.Contains("No")
                            select new { k.id, mes = k.mescarga };

            var registrosOk = from k in db.kpistrafico
                              where k.evaluacionmaniobras.Contains("Ok")
                              select new { k.id, mes = k.mescarga };

            var registrosDesv = from k in db.kpistrafico
                                where k.evaluacionmaniobras.Contains("No")
                                select new { k.id, mes = k.mescarga };

            var totalRegistros = (decimal)registros.Select(x => x.id).Count();

            var totalOks = (int)registrosOk.Select(x => x.id).Count();

            var totalDesv = (int)registrosDesv.Select(x => x.id).Count();

            var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;

            var porcentajeokRounded = Math.Round(porcentajeok, 2);

            double dataOk = (double)Math.Round(porcentajeokRounded, 2);

            ViewBag.TotalEvaluacionManiobras = totalRegistros;
            ViewBag.TotalOkEvaluacionManiobras = totalOks;
            ViewBag.TotalDesvEvaluacionManiobras = totalDesv;
            ViewBag.PorcentajeOkEvaluacionManiobras = dataOk;
        }

        private List<DataPoint> DatosEvalManiobras(string mes)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            var registros = from k in db.kpistrafico
                            where k.evaluacionmaniobras.Contains("Ok") || k.evaluacionmaniobras.Contains("No")
                            select new { k.id, mes = k.mescarga };

            var registrosOk = from k in db.kpistrafico
                              where k.evaluacionmaniobras.Contains("Ok")
                              select new { k.id, mes = k.mescarga };

            var registrosDesv = from k in db.kpistrafico
                                where k.evaluacionmaniobras.Contains("No")
                                select new { k.id, mes = k.mescarga };

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
                dataPoints.Add(new DataPoint("No", dataDesv));
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
                    dataPoints.Add(new DataPoint("No", dataDesv));
                }
                else
                {
                    double dataOk = 0;
                    double dataDesv = 0;

                    dataPoints.Add(new DataPoint("Ok", dataOk));
                    dataPoints.Add(new DataPoint("No", dataDesv));
                }
            }

            return dataPoints;
        }

        public ActionResult GraficaEvaluacionManiobras(string mes)
        {
            try
            {
                var ultimomes = db.kpistrafico.OrderByDescending(x => x.id).First().mescarga;
                if (mes == "1" || mes == "2" || mes == "3" || mes == "4" || mes == "5" || mes == "6" || mes == "7" || mes == "8" || mes == "9" || mes == "10" || mes == "11" || mes == "12")
                {
                    ViewBag.MesEvalManiobras = ultimomes;
                    ViewBag.DataPointsManiobras = JsonConvert.SerializeObject(DatosEvalManiobras(ultimomes));
                }
                else
                {                    
                    ViewBag.DataPointsManiobras = JsonConvert.SerializeObject(DatosEvalManiobras(mes));
                    ViewBag.MesEvalManiobras = mes;
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                throw;
            }
            return PartialView();
        }

        #endregion

        #region Evaluacion ocupacion 

        public List<kpistrafico> ResultadosOcupacion()
        {
            try
            {
                var kpi = from k in db.kpistrafico
                          select new { k.id, k.mescarga, k.consecutivoflete, mes = k.mescarga, k.capasidadpalletsunidad, k.palletsembarcados, k.pesoinventario, k.capasidadunidad };

                var listakpis = new List<kpistrafico>();

                foreach (var item in kpi)
                {
                    kpistrafico kpisTemp = new kpistrafico();
                    kpisTemp.id = item.id;
                    kpisTemp.consecutivoflete = item.consecutivoflete;
                    kpisTemp.mescarga = item.mes;
                    kpisTemp.capasidadpalletsunidad = item.capasidadpalletsunidad;
                    kpisTemp.palletsembarcados = item.palletsembarcados;
                    kpisTemp.pesoinventario = item.pesoinventario;
                    kpisTemp.capasidadunidad = item.capasidadunidad;
                    kpisTemp.mescarga = item.mes;

                    listakpis.Add(kpisTemp);
                }

                var kpisAgrupados = from k in listakpis
                                    group k by new { k.consecutivoflete, k.mescarga } into g
                                    select new Group<string, kpistrafico>
                                    {
                                        Key = g.Key.consecutivoflete,
                                        Values = g,
                                        CapacidadPalletsUnidadTemp = g.Sum(x => x.capasidadpalletsunidad),
                                        PalletsEmbarcadosTemp = g.Sum(x => x.palletsembarcados),
                                        PesoInventarioTemp = g.Sum(x => x.pesoinventario),
                                        CapasidadUnidadTemp = g.Sum(x => x.capasidadunidad),
                                        Mes = g.Key.mescarga
                                    };

                List<kpistrafico> lista = new List<kpistrafico>();

                foreach (var item in kpisAgrupados)
                {
                    kpistrafico kpis = new kpistrafico();

                    var pallet = (decimal)item.PalletsEmbarcadosTemp;
                    var capacidadpalletunidad = (decimal)item.CapacidadPalletsUnidadTemp;
                    var peso = (decimal)item.PesoInventarioTemp;
                    var capacidadunidad = (decimal)item.CapasidadUnidadTemp;

                    kpis.mescarga = item.Mes;
                    kpis.consecutivoflete = item.Key;
                    kpis.palletsembarcados = pallet;
                    kpis.pesoinventario = peso;
                    kpis.capasidadunidad = capacidadunidad;
                    kpis.capasidadpalletsunidad = capacidadpalletunidad;

                    if (pallet != 0 && capacidadpalletunidad != 0)
                    {
                        var porcentajePallets = ((decimal)item.PalletsEmbarcadosTemp / (decimal)item.CapacidadPalletsUnidadTemp);
                        double dataOk = (double)Math.Round(porcentajePallets, 2);
                        double valor = double.Parse("0.9");
                        kpis.porcentajeocupacionunidad = (decimal)dataOk;

                        if (dataOk >= valor)
                        {
                            kpis.Estado = "OK";
                        }
                        else
                        {
                            kpis.Estado = "NO";
                        }
                    }                    
                    else
                    {
                        kpis.Estado = "NA";
                    }

                    if (peso != 0 && capacidadunidad != 0)
                    {
                        var porcentajeOcupacion = ((decimal)item.PesoInventarioTemp / (decimal)item.CapasidadUnidadTemp);
                        double dataOk = (double)Math.Round(porcentajeOcupacion, 2);
                        double valor = double.Parse("0.9");
                        kpis.porcentajepesounidad = (decimal)dataOk;

                        if (dataOk >= valor)
                        {
                            kpis.EstadoPeso = "OK";
                        }
                        else
                        {
                            kpis.EstadoPeso = "NO";
                        }
                    }                    
                    else
                    {
                        kpis.EstadoPeso = "NA";
                    }

                    if (kpis.EstadoPeso == "OK" || kpis.Estado == "OK")
                    {
                        kpis.EstadoTotal = "OK";
                    }
                    else if (kpis.EstadoPeso == "NA" && kpis.Estado == "NA")
                    {
                        kpis.EstadoTotal = "NA";
                    }
                    else
                    {
                        kpis.EstadoTotal = "NO";
                    }

                    lista.Add(kpis);
                }

                var kpisAgrupados1 = from k in lista
                                     group k by k.mescarga into g                                     
                                     select new Group<string, kpistrafico>
                                     {
                                         Key = g.Key,
                                         Values = g,
                                         TotalOks = g.Count(x => x.EstadoTotal.Contains("OK")),
                                         TotalDesv = g.Count(x => x.EstadoTotal.Contains("NO")),
                                         TotalRecords = g.Count(x => x.EstadoTotal.Contains("OK") || x.EstadoTotal.Contains("NO"))
                                     };

                List<kpistrafico> lista2 = new List<kpistrafico>();

                foreach (var item in kpisAgrupados1)
                {
                    kpistrafico kpis = new kpistrafico();
                    kpis.mescarga = item.Key;
                    kpis.Total = (int)item.TotalRecords;
                    kpis.TotalOks = (int)item.TotalOks;
                    kpis.TotalDesv = (int)item.TotalDesv;
                    if ((int)item.TotalRecords == 0)
                    {
                        kpis.Porcentaje = 0;
                    }
                    else
                    {
                        var porcentajeok = ((int)item.TotalOks / (decimal)(int)item.TotalRecords) * 100;
                        var porcentajeokRounded = Math.Round(porcentajeok, 2);
                        double dataOk = (double)Math.Round(porcentajeokRounded, 2);
                        kpis.Porcentaje = (decimal)dataOk;
                    }

                    lista2.Add(kpis);
                }

                return lista2;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult ResumenPrueba()
        {
            return PartialView(ResultadosOcupacion().ToList());
        }

        public ActionResult ResumenEvaluacionOcupacion()
        {
            TotalesAnuales();
            return PartialView(ResultadosOcupacion().ToList());                     
        }

        public void TotalesAnuales()
        {
            int totalRegistros = 0;
            int totalOks = 0;
            int totalDesv = 0;

            foreach (var item in ResultadosOcupacion().ToList())
            {
                totalRegistros += (int)item.Total;
                totalOks += (int)item.TotalOks;
                totalDesv += (int)item.TotalDesv;
            }

            var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;
            var porcentajeokRounded = Math.Round(porcentajeok, 2);

            ViewBag.PorcentajeOcupacion = porcentajeokRounded;
            ViewBag.TotalOcupacion = totalRegistros;
            ViewBag.TotalOkOcupacion = totalOks;
            ViewBag.TotalNoOcupacion = totalDesv;
        }

        public ActionResult GraficaEvaluacionOcupacion(string mes)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            int totalRegistros = 0;
            int totalOks = 0;
            int totalDesv = 0;
                       
            if (mes == "Total")
            {
                foreach (var item in ResultadosOcupacion().ToList())
                {
                    totalRegistros += (int)item.Total;
                    totalOks += (int)item.TotalOks;
                    totalDesv += (int)item.TotalDesv;
                }

                var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;

                var porcentajedesv = ((int)totalDesv / (decimal)(int)totalRegistros) * 100;

                var porcentajeokRounded = Math.Round(porcentajeok, 2);

                var porcentajeDesvRounded = Math.Round(porcentajedesv, 2);

                double dataOk = (double)Math.Round(porcentajeokRounded, 2);
                double dataDesv = (double)Math.Round(porcentajeDesvRounded, 2);

                dataPoints.Add(new DataPoint("Ok", dataOk));
                dataPoints.Add(new DataPoint("No", dataDesv));

                ViewBag.MesOcupacion = mes;
            }
            else
            {
                if (mes == "1" || mes == "2" || mes == "3" || mes == "4" || mes == "5" || mes == "6" || mes == "7" || mes == "8" || mes == "9" || mes == "10" || mes == "11" || mes == "12")
                {
                    var ultimomes = db.kpistrafico.OrderByDescending(x => x.id).First().mescarga;
                    ViewBag.MesOcupacion = ultimomes;
                    foreach (var item in ResultadosOcupacion().Where(x => x.mescarga == ultimomes).ToList())
                    {
                        totalRegistros += (int)item.Total;
                        totalOks += (int)item.TotalOks;
                        totalDesv += (int)item.TotalDesv;
                    }

                    var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;

                    var porcentajedesv = ((int)totalDesv / (decimal)(int)totalRegistros) * 100;

                    var porcentajeokRounded = Math.Round(porcentajeok, 2);

                    var porcentajeDesvRounded = Math.Round(porcentajedesv, 2);

                    double dataOk = (double)Math.Round(porcentajeokRounded, 2);
                    double dataDesv = (double)Math.Round(porcentajeDesvRounded, 2);

                    dataPoints.Add(new DataPoint("Ok", dataOk));
                    dataPoints.Add(new DataPoint("No", dataDesv));                    
                }
                else
                {
                    ViewBag.MesOcupacion = mes;

                    foreach (var item in ResultadosOcupacion().Where(x => x.mescarga == mes).ToList())
                    {
                        totalRegistros += (int)item.Total;
                        totalOks += (int)item.TotalOks;
                        totalDesv += (int)item.TotalDesv;
                    }

                    var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;

                    var porcentajedesv = ((int)totalDesv / (decimal)(int)totalRegistros) * 100;

                    var porcentajeokRounded = Math.Round(porcentajeok, 2);

                    var porcentajeDesvRounded = Math.Round(porcentajedesv, 2);

                    double dataOk = (double)Math.Round(porcentajeokRounded, 2);
                    double dataDesv = (double)Math.Round(porcentajeDesvRounded, 2);

                    dataPoints.Add(new DataPoint("Ok", (double)dataOk));
                    dataPoints.Add(new DataPoint("No", dataDesv));
                }
            }
            
            ViewBag.DataPointsEvalOcupacion = JsonConvert.SerializeObject(dataPoints);

            return PartialView();
        }

        #endregion

        #region On Time Posicionamiento

        public ActionResult ResumenOnTimePosicionamiento()
        {
            TotalesEvalPosicionamiento();
            return PartialView(DatosPosicionamiento());
        }

        public List<kpistrafico> DatosPosicionamiento()
        {
            try
            {
                var kpi = from k in db.kpistrafico
                          select new { k.id, status = k.evaluacionposicionamiento, mes = k.mescarga, k.consecutivoflete };

                var listakpis = new List<kpistrafico>();

                foreach (var item in kpi)
                {
                    kpistrafico kpisTemp = new kpistrafico();
                    kpisTemp.id = item.id;
                    kpisTemp.mescarga = item.mes;
                    kpisTemp.evaluacionposicionamiento = item.status;
                    kpisTemp.consecutivoflete = item.consecutivoflete;
                    listakpis.Add(kpisTemp);
                }

                var kpisAgrupados = from k in listakpis
                                    group k by new { k.consecutivoflete, k.mescarga, k.evaluacionposicionamiento } into g
                                    select new Group<string, kpistrafico>
                                    {
                                        Key = g.Key.consecutivoflete,
                                        Values = g,
                                        Mes = g.Key.mescarga,
                                        EvaluacionPosicionamiento = g.Key.evaluacionposicionamiento,
                                    };


                List<kpistrafico> lista = new List<kpistrafico>();

                foreach (var item in kpisAgrupados)
                {
                    kpistrafico kpis = new kpistrafico();
                    kpis.consecutivoflete = item.Key;
                    kpis.mescarga = item.Mes;
                    kpis.evaluacionposicionamiento = item.EvaluacionPosicionamiento;

                    lista.Add(kpis);
                }

                var kpisAgrupados1 = from k in lista
                                     group k by k.mescarga into g
                                     select new Group<string, kpistrafico>
                                     {
                                         Key = g.Key,
                                         Values = g,
                                         TotalOks = g.Count(x => x.evaluacionposicionamiento.Contains("Ok")),
                                         TotalDesv = g.Count(x => x.evaluacionposicionamiento.Contains("No")),
                                         TotalRecords = g.Count(x => x.evaluacionposicionamiento.Contains("Ok") || x.evaluacionposicionamiento.Contains("No"))
                                     };

                List<kpistrafico> lista1 = new List<kpistrafico>();

                foreach (var item in kpisAgrupados1)
                {
                    kpistrafico kpis = new kpistrafico();
                    kpis.mescarga = item.Key;
                    kpis.TotalOks = (int)item.TotalOks;
                    kpis.TotalDesv = (int)item.TotalDesv;
                    kpis.Total = (int)item.TotalRecords;

                    if (kpis.TotalOks > 0)
                    {
                        var porcentajeOk = ((int)item.TotalOks / (decimal)(int)item.TotalRecords) * 100;
                        var porcentajeDesvRounded = Math.Round(porcentajeOk, 2);
                        kpis.Porcentaje = (decimal)porcentajeDesvRounded;
                    }
                    else
                    {
                        kpis.Porcentaje = 0;
                    }

                    lista1.Add(kpis);
                }                
                return lista1.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void TotalesEvalPosicionamiento()
        {
            int totalRegistros = 0;
            int totalOks = 0;
            int totalDesv = 0;

            foreach (var item in DatosPosicionamiento().ToList())
            {
                totalRegistros += (int)item.Total;
                totalOks += (int)item.TotalOks;
                totalDesv += (int)item.TotalDesv;
            }

            var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;
            var porcentajeokRounded = Math.Round(porcentajeok, 2);

            ViewBag.TotalEvaluacionPosicionamiento = totalRegistros;
            ViewBag.TotalOkEvaluacionPosicionamiento = totalOks;
            ViewBag.TotalDesvEvaluacionPosicionamiento = totalDesv;
            ViewBag.PorcentajeOkEvaluacionPosicionamiento = porcentajeokRounded;
        }

        public ActionResult GraficaEvaluacionPosicionamiento(string mes)
        {
            try
            {
                List<DataPoint> dataPoints = new List<DataPoint>();
                int totalRegistros = 0;
                int totalOks = 0;
                int totalDesv = 0;

                if (mes=="Total")
                {
                    foreach (var item in DatosPosicionamiento().ToList())
                    {
                        totalRegistros += (int)item.Total;
                        totalOks += (int)item.TotalOks;
                        totalDesv += (int)item.TotalDesv;
                    }

                    var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;

                    var porcentajedesv = ((int)totalDesv / (decimal)(int)totalRegistros) * 100;

                    var porcentajeokRounded = Math.Round(porcentajeok, 2);

                    var porcentajeDesvRounded = Math.Round(porcentajedesv, 2);

                    double dataOk = (double)Math.Round(porcentajeokRounded, 2);
                    double dataDesv = (double)Math.Round(porcentajeDesvRounded, 2);

                    dataPoints.Add(new DataPoint("Ok", dataOk));
                    dataPoints.Add(new DataPoint("Desv", dataDesv));

                    ViewBag.DataPointPosicionamiento = JsonConvert.SerializeObject(dataPoints);

                    ViewBag.MesEvalOcupacion = mes;
                }
                else
                {
                    var ultimomes = db.kpistrafico.OrderByDescending(x => x.id).First().mescarga;
                    if (mes == "1" || mes == "2" || mes == "3" || mes == "4" || mes == "5" || mes == "6" || mes == "7" || mes == "8" || mes == "9" || mes == "10" || mes == "11" || mes == "12")
                    {
                        ViewBag.MesEvalPosicionamiento = ultimomes;
                        foreach (var item in DatosPosicionamiento().Where(x => x.mescarga == ultimomes).ToList())
                        {
                            totalRegistros += (int)item.Total;
                            totalOks += (int)item.TotalOks;
                            totalDesv += (int)item.TotalDesv;
                        }

                        var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;

                        var porcentajedesv = ((int)totalDesv / (decimal)(int)totalRegistros) * 100;

                        var porcentajeokRounded = Math.Round(porcentajeok, 2);

                        var porcentajeDesvRounded = Math.Round(porcentajedesv, 2);

                        double dataOk = (double)Math.Round(porcentajeokRounded, 2);
                        double dataDesv = (double)Math.Round(porcentajeDesvRounded, 2);

                        dataPoints.Add(new DataPoint("Ok", dataOk));
                        dataPoints.Add(new DataPoint("Desv", dataDesv));
                        ViewBag.DataPointPosicionamiento = JsonConvert.SerializeObject(dataPoints);
                        ViewBag.MesEvalOcupacion = ultimomes;
                    }
                    else
                    {                        
                        foreach (var item in DatosPosicionamiento().Where(x => x.mescarga == mes).ToList())
                        {
                            totalRegistros += (int)item.Total;
                            totalOks += (int)item.TotalOks;
                            totalDesv += (int)item.TotalDesv;
                        }

                        var porcentajeok = ((int)totalOks / (decimal)(int)totalRegistros) * 100;

                        var porcentajedesv = ((int)totalDesv / (decimal)(int)totalRegistros) * 100;

                        var porcentajeokRounded = Math.Round(porcentajeok, 2);

                        var porcentajeDesvRounded = Math.Round(porcentajedesv, 2);

                        double dataOk = (double)Math.Round(porcentajeokRounded, 2);
                        double dataDesv = (double)Math.Round(porcentajeDesvRounded, 2);

                        dataPoints.Add(new DataPoint("Ok", dataOk));
                        dataPoints.Add(new DataPoint("Desv", dataDesv));
                        ViewBag.DataPointPosicionamiento = JsonConvert.SerializeObject(dataPoints);
                        ViewBag.MesEvalOcupacion = mes;
                    }
                }                
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                throw;
            }
            return PartialView();
        }

        #endregion

        #region Facturacion X Tipo Envio

        public ActionResult ResumenFacturacionRequirimiento()
        {
            var kpi = from k in db.kpistrafico
                      where k.id > 12
                      select new { k.id, k.tiporequerimiento, k.costoflete, k.costomaniobras };

            var listakpis = new List<kpistrafico>();

            foreach (var item in kpi)
            {
                kpistrafico kpisTemp = new kpistrafico();
                kpisTemp.id = item.id;
                kpisTemp.tiporequerimiento = item.tiporequerimiento;
                kpisTemp.costoflete = item.costoflete;
                kpisTemp.costomaniobras = item.costomaniobras;                

                listakpis.Add(kpisTemp);
            }

            var kpisAgrupados = from k in listakpis
                                group k by k.tiporequerimiento into g
                                select new Group<string, kpistrafico>
                                {
                                    Key = g.Key,
                                    Values = g,
                                    FacturacionFlete = g.Sum(x => x.costoflete),
                                    FacturacionManiobras = g.Sum(x => x.costomaniobras)                                    
                                };

            List<kpistrafico> lista = new List<kpistrafico>();

            foreach (var item in kpisAgrupados)
            {
                kpistrafico kpis = new kpistrafico();
                                
                kpis.tiporequerimiento = item.Key;
                kpis.FacturacionFleteString = String.Format("{0:C}", item.FacturacionFlete);
                kpis.FacturacionManiobrasString = String.Format("{0:C}", item.FacturacionManiobras);                         

                lista.Add(kpis);
            }
            return PartialView(lista.ToList());
        }

        #endregion

        #region Suma de Movimientos

        public ActionResult ResumenSumaMovimientos()
        {
            var kpi = from k in db.kpistrafico
                      where k.id > 12
                      select new { k.id, k.tiporequerimiento, k.palletsembarcados, k.numeropiezas, k.numerocajas };

            var listakpis = new List<kpistrafico>();

            foreach (var item in kpi)
            {
                kpistrafico kpisTemp = new kpistrafico();
                kpisTemp.id = item.id;
                kpisTemp.tiporequerimiento = item.tiporequerimiento;
                kpisTemp.palletsembarcados = item.palletsembarcados;
                kpisTemp.numerocajas = item.numerocajas;
                kpisTemp.numeropiezas = item.numeropiezas;

                listakpis.Add(kpisTemp);
            }

            var kpisAgrupados = from k in listakpis
                                group k by k.tiporequerimiento into g
                                select new Group<string, kpistrafico>
                                {
                                    Key = g.Key,
                                    Values = g,
                                    PalletsEmbarcadosTemp = g.Sum(x => x.palletsembarcados),
                                    CajasEmbarcadasTemp = g.Sum(x => x.numerocajas),
                                    PiezasEmbarcadasTemp = g.Sum(x => x.numeropiezas)
                                };

            List<kpistrafico> lista = new List<kpistrafico>();

            foreach (var item in kpisAgrupados)
            {
                kpistrafico kpis = new kpistrafico();
                                
                var cajas = (double)Math.Round((double)item.PalletsEmbarcadosTemp, 0);                
                kpis.tiporequerimiento = item.Key;
                kpis.palletsembarcados = (decimal)cajas;
                kpis.numerocajas = item.CajasEmbarcadasTemp;
                kpis.numeropiezas = item.PiezasEmbarcadasTemp;

                lista.Add(kpis);
            }
            return PartialView(lista.ToList());
        }

        #endregion
    }
}