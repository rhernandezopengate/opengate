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
    public class kpisinventariosexactitudController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: kpisinventariosexactitud
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Resumen()
        {
            var query = db.kpisinventariosexactitud.ToList().OrderBy(x => x.id).GroupBy(x => new { x.mes })
               .Select(y => new kpisinventariosexactitud()
               {
                   mes = y.Key.mes,
                   absoluto = y.Sum(x => x.absoluto),
                   valorinventario = y.Sum(x => x.valorinventario)
               });

            List<kpisinventariosexactitud> lista2 = new List<kpisinventariosexactitud>();

            foreach (var item in query)
            {
                kpisinventariosexactitud kpis = new kpisinventariosexactitud();
                kpis.mes = item.mes;
                kpis.absoluto = item.absoluto;
                kpis.valorinventario = item.valorinventario;

                decimal? value = kpis.absoluto;
                kpis.absolutoString = String.Format("{0:C}", value);

                decimal? value2 = kpis.valorinventario;
                kpis.valorString = String.Format("{0:C}", value2);

                if (item.valorinventario > 0)
                {
                    var resultado = ((decimal)item.valorinventario - (decimal)(int)item.absoluto) / (decimal)(int)item.valorinventario;
                    var porcentajeRounded = resultado * 100;
                    double p = Convert.ToDouble(porcentajeRounded);
                    kpis.Porcentaje = String.Format("{0:0.0000}", p);
                }
                else
                {
                    kpis.Porcentaje = "0";
                }
                Totales();
                lista2.Add(kpis);
            } 
            return PartialView(lista2.ToList());
        }

        public ActionResult Graficas(string mes)
        {
            try
            {
                var ultimomes = db.kpisinventariosexactitud.OrderByDescending(x => x.id).First().mes;
                if (mes == "1" || mes == "2" || mes == "3" || mes == "4" || mes == "5" || mes == "6" || mes == "7" || mes == "8" || mes == "9" || mes == "10" || mes == "11" || mes == "12")
                {
                    ViewBag.DataPointsExactitud = JsonConvert.SerializeObject(DatosGrafica(ultimomes));
                    ViewBag.MesExactitud = ultimomes;
                }
                else if (mes == "Total")
                {
                    ViewBag.DataPointsExactitud = JsonConvert.SerializeObject(DatosGrafica(mes));
                    ViewBag.MesExactitud = mes;
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
                    
                    ViewBag.DataPointsExactitud = JsonConvert.SerializeObject(DatosGrafica(meses));
                    ViewBag.MesExactitud = meses;
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                throw;
            }
            return PartialView();
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
            var Area = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<kpisinventariosexactitud> listaKPIS = new List<kpisinventariosexactitud>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [dbo].[SP_KPISINVENTARIOSEXACTITUD_PARAMETROSOPCIONALES] @mes, @area";

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
                            var kpis = new kpisinventariosexactitud();

                            kpis.id = Convert.ToInt32(dr["id"]);                            
                            kpis.mes = dr["mes"].ToString();
                            kpis.subinventario = dr["subinventario"].ToString();
                            kpis.absoluto =  Convert.ToDecimal(dr["absoluto"]);
                            kpis.valorinventario = Convert.ToDecimal(dr["valorinventario"]);

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

        public double DatosGrafica(string mes)
        {            

            var registros = from k in db.kpisinventariosexactitud
                            where k.id > 12
                            select new { k.id, absoluto = k.absoluto, valor = k.valorinventario, mes=k.mes };

            if (mes.Equals("Total"))
            {
                var totalRegistros = registros.Where(x => x.id > 12).ToList();

                if (totalRegistros != null)
                {
                    decimal? absolutos = 0;
                    decimal? valor = 0;
                    foreach (var item in totalRegistros)
                    {
                        absolutos += item.absoluto;
                        valor += item.valor;
                    }

                    double a = Convert.ToDouble(absolutos);
                    double v = Convert.ToDouble(valor);

                    var porcentaje = (v - a) / v;
                    var porcentajeok = porcentaje * 100;
                    var porcentajedesv = 100 - porcentajeok;

                    double ok = double.Parse(String.Format("{0:0.0000}", porcentajeok));
                    double desv = double.Parse(String.Format("{0:0.0000}", porcentajedesv));

                    return ok;
                }
                else
                {
                    return 0;                    
                }
            }
            else
            {
                var totalRegistros = registros.Where(x => x.mes.Contains(mes)).ToList();

                if (totalRegistros != null)
                {
                    decimal? absolutos = 0;
                    decimal? valor = 0;
                    foreach (var item in totalRegistros)
                    {
                        absolutos += item.absoluto;
                        valor += item.valor;                        
                    }

                    double a = Convert.ToDouble(absolutos);
                    double v = Convert.ToDouble(valor);

                    var porcentaje = (v - a) / v;
                    var porcentajeok = porcentaje * 100;
                    var porcentajedesv = 100 - porcentajeok;

                    double ok = double.Parse(String.Format("{0:0.0000}", porcentajeok));
                    double desv = double.Parse(String.Format("{0:0.0000}", porcentajedesv));

                    return ok;
                }
                else
                {
                    return 0;
                }
            }            
        }

        public void Totales()
        {
            var registros = from k in db.kpisinventariosexactitud
                            where k.id > 12
                            select new { k.id, absoluto = k.absoluto, valor = k.valorinventario, mes = k.mes };

            decimal? absolutos = 0;
            decimal? valor = 0;

            foreach (var item in registros)
            {
                absolutos += item.absoluto;
                valor += item.valor;
            }

            double a = Convert.ToDouble(absolutos);
            double v = Convert.ToDouble(valor);

            var porcentaje = (v - a) / v;
            var porcentajeok = porcentaje * 100;
            var porcentajedesv = 100 - porcentajeok;

            double ok = double.Parse(String.Format("{0:0.0000}", porcentajeok));

            ViewBag.Absoluto = String.Format("{0:C}", a);
            ViewBag.ValorInventario = String.Format("{0:C}", v);
            ViewBag.PorcentajeExactitud = ok;
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
                List<kpisinventariosexactitud> customersModels = new List<kpisinventariosexactitud>();
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
                    dt.Columns.AddRange(new DataColumn[4] {
                        new DataColumn("SubInventario", typeof(string)),
                        new DataColumn("Mes", typeof(string)),
                        new DataColumn("Absoluto", typeof(decimal)),
                        new DataColumn("ValorInventario", typeof(decimal)),
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
                            sqlBulkCopy.DestinationTableName = "dbo.[kpisinventariosexactitud]";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table                            
                            sqlBulkCopy.ColumnMappings.Add("SubInventario", "subinventario");
                            sqlBulkCopy.ColumnMappings.Add("Mes", "mes");
                            sqlBulkCopy.ColumnMappings.Add("Absoluto", "absoluto");
                            sqlBulkCopy.ColumnMappings.Add("ValorInventario", "valorinventario");

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

        [Authorize(Roles = "admin, coordinadorinventarios")]
        public ActionResult Delete()
        {
            return View();
        }
    }
}