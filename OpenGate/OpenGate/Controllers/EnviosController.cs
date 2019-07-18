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

namespace OpenGate.Controllers
{
    [Authorize]
    public class EnviosController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: Envios
        [Authorize(Roles = "admin, gerentegeneral, homedeliveryherbalife, homedeliveryqro")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin, gerentegeneral, homedeliveryqro, coordinadorinventarios")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult AsignacionAplicacion()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult ObtenerEnviosAplicacion()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var Requerimiento = Request.Form.GetValues("columns[14][search][value]").FirstOrDefault();
            var CV = Request.Form.GetValues("columns[9][search][value]").FirstOrDefault();
            var Fecha = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<envioshomedelivery> listEnvios = new List<envioshomedelivery>();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "EXEC [dbo].[SP_ENVIOS_CONSULTAAPLICACION] @Requerimiento, @CV, @fecha, @Aplicacion";
                    var query = new SqlCommand(sql, con);

                    query.Parameters.AddWithValue("@Aplicacion", DBNull.Value);

                    if (Requerimiento != "")
                    {
                        query.Parameters.AddWithValue("@Requerimiento", Requerimiento);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@Requerimiento", DBNull.Value);
                    }

                    if (CV != "")
                    {
                        query.Parameters.AddWithValue("@CV", CV);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@CV", DBNull.Value);
                    }

                    if (Fecha != "")
                    {
                        DateTime date = Convert.ToDateTime(Fecha);
                        query.Parameters.AddWithValue("@fecha", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fecha", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var envios = new envioshomedelivery();

                            envios.id = Convert.ToInt32(dr["id"]);
                            envios.tipoenvio = dr["tipoenvio"].ToString();
                            envios.solicitante = dr["solicitante"].ToString();
                            envios.mes = dr["mes"].ToString();
                            envios.fecharequerimiento = Convert.ToDateTime(dr["fecharequerimiento"]);
                            envios.sku = dr["sku"].ToString();
                            envios.descripcion = dr["descripcion"].ToString();
                            envios.cantidad = Convert.ToInt32(dr["cantidad"]);
                            envios.cv = dr["cv"].ToString();
                            envios.destino = dr["destino"].ToString();
                            envios.cajas = Convert.ToInt32(dr["cajas"]);
                            envios.paqueteria = dr["paqueteria"].ToString();
                            envios.referenciaguia = dr["referenciaguia"].ToString();
                            envios.numeroguia = dr["numeroguia"].ToString();
                            envios.numerorequerimiento = dr["numerorequerimiento"].ToString();
                            envios.tiposalida = dr["tiposalida"].ToString();
                            envios.aplicacion = dr["aplicacion"].ToString();
                            envios.observaciones = dr["observaciones"].ToString();

                            listEnvios.Add(envios);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listEnvios = listEnvios.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listEnvios.ToList().Count();
                var NewItems = listEnvios.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(string requerimiento, string cv, DateTime fecha, string aplicacion)
        {
            try
            {
                List<envioshomedelivery> listEnvios = new List<envioshomedelivery>();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "EXEC [dbo].[SP_ENVIOS_CONSULTAAPLICACION] @Requerimiento, @CV, @fecha";
                    var query = new SqlCommand(sql, con);

                    query.Parameters.AddWithValue("@Aplicacion", DBNull.Value);

                    if (requerimiento != "")
                    {
                        query.Parameters.AddWithValue("@Requerimiento", requerimiento);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@Requerimiento", DBNull.Value);
                    }

                    if (cv != "")
                    {
                        query.Parameters.AddWithValue("@CV", cv);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@CV", DBNull.Value);
                    }

                    if (fecha != null)
                    {
                        DateTime date = Convert.ToDateTime(fecha);
                        query.Parameters.AddWithValue("@fecha", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fecha", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var enviosTemp = new envioshomedelivery();

                            enviosTemp.id = Convert.ToInt32(dr["id"]);                            

                            listEnvios.Add(enviosTemp);
                        }
                    }
                }               

                foreach (var item in listEnvios)
                {
                    envioshomedelivery envio = db.envioshomedelivery.Where(x => x.id == item.id).FirstOrDefault();
                    envio.aplicacion = aplicacion;
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Ok" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "No" });
            }           
        }

        [HttpPost]
        [Authorize(Roles = "admin, gerentegeneral, homedeliveryherbalife, homedeliveryqro")]
        public ActionResult ObtenerEnvios()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var TipoEnvio = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var Solicitante = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var Mes = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var FechaRequerimiento = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var Guia = Request.Form.GetValues("columns[13][search][value]").FirstOrDefault();
            var Aplicacion = Request.Form.GetValues("columns[16][search][value]").FirstOrDefault();


            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            int valorerror = 0;

            try
            {
                List<envioshomedelivery> listEnvios = new List<envioshomedelivery>();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "EXEC [dbo].[SP_ENVIOS_PARAMETROS_OPCIONALES] @Mes, @TipoEnvio, @Solicitante, @Guia, @Aplicacion, @FHRequerimiento";
                    var query = new SqlCommand(sql, con);

                    if (TipoEnvio != "")
                    {
                        query.Parameters.AddWithValue("@TipoEnvio", TipoEnvio);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@TipoEnvio", DBNull.Value);
                    }

                    if (Solicitante != "")
                    {
                        query.Parameters.AddWithValue("@Solicitante", Solicitante);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@Solicitante", DBNull.Value);
                    }

                    if (Mes != "")
                    {
                        query.Parameters.AddWithValue("@Mes", Mes);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@Mes", DBNull.Value);
                    }

                    if (Guia != "")
                    {
                        query.Parameters.AddWithValue("@Guia", Guia);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@Guia", DBNull.Value);
                    }

                    if (Aplicacion != "")
                    {
                        query.Parameters.AddWithValue("@Aplicacion", Aplicacion);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@Aplicacion", DBNull.Value);
                    }

                    if (FechaRequerimiento != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaRequerimiento);
                        query.Parameters.AddWithValue("@FHRequerimiento", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@FHRequerimiento", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var envios = new envioshomedelivery();

                            valorerror = Convert.ToInt32(dr["id"]);
                            envios.id = Convert.ToInt32(dr["id"]);
                            envios.tipoenvio = dr["tipoenvio"].ToString();
                            envios.solicitante = dr["solicitante"].ToString();
                            envios.mes = dr["mes"].ToString();
                            envios.fecharequerimiento = Convert.ToDateTime(dr["fecharequerimiento"]);
                            envios.sku = dr["sku"].ToString();
                            envios.descripcion = dr["descripcion"].ToString();
                            envios.cantidad = Convert.ToInt32(dr["cantidad"]);
                            envios.cv = dr["cv"].ToString();
                            envios.destino = dr["destino"].ToString();
                            envios.cajas = Convert.ToInt32(dr["cajas"]);
                            envios.paqueteria = dr["paqueteria"].ToString();
                            envios.referenciaguia = dr["referenciaguia"].ToString();
                            envios.numeroguia = dr["numeroguia"].ToString();
                            envios.numerorequerimiento = dr["numerorequerimiento"].ToString();
                            envios.tiposalida = dr["tiposalida"].ToString();
                            envios.aplicacion = dr["aplicacion"].ToString();
                            envios.observaciones = dr["observaciones"].ToString();

                            listEnvios.Add(envios);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listEnvios = listEnvios.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listEnvios.ToList().Count();
                var NewItems = listEnvios.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                string error = _ex.Message.ToString();
                return RedirectToAction("Error500", "Errores", new { error = error + valorerror });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin, gerentegeneral, homedeliveryqro, coordinadorinventarios")]
        public ActionResult Import(HttpPostedFileBase postedFileBase)
        {
            try
            {
                List<envioshomedelivery> lista = new List<envioshomedelivery>();
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
                        new DataColumn("tipoenvio", typeof(string)),
                        new DataColumn("solicitante", typeof(string)),
                        new DataColumn("mes", typeof(string)),
                        new DataColumn("fecharequerimiento", typeof(DateTime)),
                        new DataColumn("sku", typeof(string)),
                        new DataColumn("descripcion", typeof(string)),
                        new DataColumn("cantidad", typeof(string)),
                        new DataColumn("cv", typeof(string)),
                        new DataColumn("destino", typeof(string)),
                        new DataColumn("cajas", typeof(string)),
                        new DataColumn("paqueteria", typeof(string)),
                        new DataColumn("referenciaguia", typeof(string)),
                        new DataColumn("numeroguia", typeof(string)),
                        new DataColumn("numerorequerimiento", typeof(string)),
                        new DataColumn("tiposalida", typeof(string)),
                        new DataColumn("aplicacion", typeof(string)),
                        new DataColumn("observaciones", typeof(string))
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
                                    if (dt.Rows[dt.Rows.Count - 1][6].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][6] = int.Parse("0");
                                    }
                                    if (dt.Rows[dt.Rows.Count - 1][9].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][9] = int.Parse("0");
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

                    string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "dbo.envioshomedelivery";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table                            
                            sqlBulkCopy.ColumnMappings.Add("tipoenvio", "tipoenvio");
                            sqlBulkCopy.ColumnMappings.Add("solicitante", "solicitante");
                            sqlBulkCopy.ColumnMappings.Add("mes", "mes");
                            sqlBulkCopy.ColumnMappings.Add("fecharequerimiento", "fecharequerimiento");
                            sqlBulkCopy.ColumnMappings.Add("sku", "sku");
                            sqlBulkCopy.ColumnMappings.Add("descripcion", "descripcion");
                            sqlBulkCopy.ColumnMappings.Add("cantidad", "cantidad");
                            sqlBulkCopy.ColumnMappings.Add("cv", "cv");
                            sqlBulkCopy.ColumnMappings.Add("destino", "destino");
                            sqlBulkCopy.ColumnMappings.Add("cajas", "cajas");
                            sqlBulkCopy.ColumnMappings.Add("paqueteria", "paqueteria");
                            sqlBulkCopy.ColumnMappings.Add("referenciaguia", "referenciaguia");
                            sqlBulkCopy.ColumnMappings.Add("numeroguia", "numeroguia");
                            sqlBulkCopy.ColumnMappings.Add("numerorequerimiento", "numerorequerimiento");
                            sqlBulkCopy.ColumnMappings.Add("tiposalida", "tiposalida");
                            sqlBulkCopy.ColumnMappings.Add("aplicacion", "aplicacion");
                            sqlBulkCopy.ColumnMappings.Add("observaciones", "observaciones");

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
                return RedirectToAction("Error500", "Errores", new { error =  error} );
            }
        }
        
    }
}