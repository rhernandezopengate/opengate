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
using System.Linq.Dynamic;

namespace OpenGate.Controllers
{
    public class csrsController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        // GET: csrs
        public ActionResult Index()
        {
            return View();
        }

        // GET: csrs/Upload
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

                    dt.Columns.AddRange(new DataColumn[39] {
                        //1
                        new DataColumn("NumeroCuenta", typeof(string)),
                        //2
                        new DataColumn("Guia", typeof(string)),
                        //3
                        new DataColumn("PieceID", typeof(string)),
                        //4
                        new DataColumn("Referencia", typeof(string)),
                        //5
                        new DataColumn("IATAOrigen", typeof(string)),
                        //6
                        new DataColumn("CiudadOrigen", typeof(string)),
                        //7
                        new DataColumn("IATADestino", typeof(string)),
                        //8
                        new DataColumn("CiudadDestino", typeof(string)),
                        //9
                        new DataColumn("SVCSubIATA", typeof(string)),
                        //10
                        new DataColumn("Ruta", typeof(string)),
                        //11
                        new DataColumn("Piezas", typeof(int)),
                        //12
                        new DataColumn("Peso", typeof(decimal)),
                        //13
                        new DataColumn("FechaRecoleccion", typeof(string)),
                        //14
                        new DataColumn("FechaPrimerCheckpointTerminal", typeof(string)),
                        //15
                        new DataColumn("HoraPrimerCheckpointTerminal", typeof(string)),
                        //16
                        new DataColumn("PrimerCheckpointTerminal", typeof(string)),
                        //17
                        new DataColumn("DescripcionPrimerCheckTerminal", typeof(string)),
                        //18
                        new DataColumn("DetallesEntregaComentarios", typeof(string)),
                        //19
                        new DataColumn("TiempoTransitoEstimado", typeof(string)),
                        //20
                        new DataColumn("TiempoTransitoRealizado", typeof(string)),
                        //21
                        new DataColumn("IntentosEntrega", typeof(string)),
                        //22
                        new DataColumn("CausaDemora", typeof(string)),
                        //23
                        new DataColumn("FechaIngresoCC", typeof(DateTime)),
                        //24
                        new DataColumn("DiasCC", typeof(int)),
                        //25
                        new DataColumn("Producto", typeof(string)),
                        //26
                        new DataColumn("ValorSeguro", typeof(decimal)),
                        //27
                        new DataColumn("NombreRemitente", typeof(string)),
                        //28
                        new DataColumn("ContactoRemitente", typeof(string)),
                        //29
                        new DataColumn("DireccionRemitente", typeof(string)),
                        //30
                        new DataColumn("CPRemitente", typeof(string)),
                        //31
                        new DataColumn("NombreDestinatario", typeof(string)),
                        //32
                        new DataColumn("ContactoDestinatario", typeof(string)),
                        //33
                        new DataColumn("DireccionDestinatario", typeof(string)),
                        //34
                        new DataColumn("CPDestinatario", typeof(string)),
                        //35
                        new DataColumn("UltimoCheckpoint", typeof(string)), 
                        //36
                        new DataColumn("FechaUltimoCheckpoint", typeof(DateTime)),
                        //37
                        new DataColumn("HoraUltimoCheckpoint", typeof(string)),
                        //38
                        new DataColumn("detalleultimocheckpoint", typeof(string)),
                        //39
                        new DataColumn("FechaCarga", typeof(DateTime)),
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
                                        dt.Rows[dt.Rows.Count - 1][0] = "NA";
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
                                        dt.Rows[dt.Rows.Count - 1][10] = int.Parse("0");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][11].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][11] = decimal.Parse("0");
                                    }
                                    if (dt.Rows[dt.Rows.Count - 1][12].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][12] = DateTime.Parse("01/01/1900");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][13].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][13] = DateTime.Parse("01/01/1900");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][14].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][14] = "00:00:00";
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
                                        dt.Rows[dt.Rows.Count - 1][17] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][18].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][18] = int.Parse("0");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][19].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][19] = int.Parse("0");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][20].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][20] = int.Parse("0");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][21].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][21] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][22].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][22] = DateTime.Parse("01/01/1900");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][23].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][23] = int.Parse("0");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][24].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][24] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][25].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][25] = decimal.Parse("0");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][26].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][26] = int.Parse("0");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][27].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][27] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][28].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][28] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][29].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][29] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][30].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][30] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][31].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][31] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][32].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][32] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][33].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][33] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][34].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][34] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][35].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][35] = DateTime.Parse("01/01/1900");
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][36].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][36] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][37].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][37] = "NA";
                                    }

                                    if (dt.Rows[dt.Rows.Count - 1][38].ToString() == "")
                                    {
                                        dt.Rows[dt.Rows.Count - 1][38] = DateTime.Parse("01/01/1900");
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

                    DataTable dtValidado = ListaCarga(dt);                                                          

                    string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "dbo.csr";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table                            
                            sqlBulkCopy.ColumnMappings.Add("NumeroCuenta", "NumeroCuenta");
                            sqlBulkCopy.ColumnMappings.Add("Guia", "Guia");
                            sqlBulkCopy.ColumnMappings.Add("PieceID", "PieceID");
                            sqlBulkCopy.ColumnMappings.Add("Referencia", "Referencia");
                            sqlBulkCopy.ColumnMappings.Add("IATAOrigen", "IATAOrigen");
                            sqlBulkCopy.ColumnMappings.Add("CiudadOrigen", "CiudadOrigen");
                            sqlBulkCopy.ColumnMappings.Add("IATADestino", "IATADestino");
                            sqlBulkCopy.ColumnMappings.Add("CiudadDestino", "CiudadDestino");
                            sqlBulkCopy.ColumnMappings.Add("SVCSubIATA", "SVCSubIATA");
                            sqlBulkCopy.ColumnMappings.Add("Ruta", "Ruta");
                            sqlBulkCopy.ColumnMappings.Add("Piezas", "Piezas");
                            sqlBulkCopy.ColumnMappings.Add("Peso", "Peso");
                            sqlBulkCopy.ColumnMappings.Add("FechaRecoleccion", "FechaRecoleccion");
                            sqlBulkCopy.ColumnMappings.Add("FechaPrimerCheckpointTerminal", "FechaPrimerCheckpointTerminal");
                            sqlBulkCopy.ColumnMappings.Add("HoraPrimerCheckpointTerminal", "HoraPrimerCheckpointTerminal");
                            sqlBulkCopy.ColumnMappings.Add("PrimerCheckpointTerminal", "PrimerCheckpointTerminal");
                            sqlBulkCopy.ColumnMappings.Add("DescripcionPrimerCheckTerminal", "DescripcionPrimerCheckTerminal");
                            sqlBulkCopy.ColumnMappings.Add("DetallesEntregaComentarios", "DetallesEntregaComentarios");
                            sqlBulkCopy.ColumnMappings.Add("TiempoTransitoEstimado", "TiempoTransitoEstimado");
                            sqlBulkCopy.ColumnMappings.Add("TiempoTransitoRealizado", "TiempoTransitoRealizado");
                            sqlBulkCopy.ColumnMappings.Add("IntentosEntrega", "IntentosEntrega");
                            sqlBulkCopy.ColumnMappings.Add("CausaDemora", "CausaDemora");
                            sqlBulkCopy.ColumnMappings.Add("FechaIngresoCC", "FechaIngresoCC");
                            sqlBulkCopy.ColumnMappings.Add("DiasCC", "DiasCC");
                            sqlBulkCopy.ColumnMappings.Add("Producto", "Producto");
                            sqlBulkCopy.ColumnMappings.Add("ValorSeguro", "ValorSeguro");
                            sqlBulkCopy.ColumnMappings.Add("NombreRemitente", "NombreRemitente");
                            sqlBulkCopy.ColumnMappings.Add("ContactoRemitente", "ContactoRemitente");
                            sqlBulkCopy.ColumnMappings.Add("DireccionRemitente", "DireccionRemitente");
                            sqlBulkCopy.ColumnMappings.Add("CPRemitente", "CPRemitente");
                            sqlBulkCopy.ColumnMappings.Add("NombreDestinatario", "NombreDestinatario");
                            sqlBulkCopy.ColumnMappings.Add("ContactoDestinatario", "ContactoDestinatario");
                            sqlBulkCopy.ColumnMappings.Add("DireccionDestinatario", "DireccionDestinatario");
                            sqlBulkCopy.ColumnMappings.Add("CPDestinatario", "CPDestinatario");
                            sqlBulkCopy.ColumnMappings.Add("UltimoCheckpoint", "UltimoCheckpoint");
                            sqlBulkCopy.ColumnMappings.Add("FechaUltimoCheckpoint", "FechaUltimoCheckpoint");
                            sqlBulkCopy.ColumnMappings.Add("HoraUltimoCheckpoint", "HoraUltimoCheckpoint");
                            sqlBulkCopy.ColumnMappings.Add("detalleultimocheckpoint", "detalleultimocheckpoint");
                            sqlBulkCopy.ColumnMappings.Add("FechaCarga", "FechaCarga");

                            con.Open();

                            sqlBulkCopy.WriteToServer(dt);
                            con.Close();
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            catch (HttpException _ex)
            {
                string error = _ex.Message.ToString();
                return RedirectToAction("Error500", "Errores", new { error = error });
            }
            catch (Exception _ex)
            {
                string error = _ex.Message.ToString();
                return RedirectToAction("Error500", "Errores", new { error =  error});
            }           
        }

        public DataTable ListaCarga(DataTable dt)
        {
            try
            {
                int filas = dt.Rows.Count;

                foreach (DataRow orow in dt.Select())
                {
                    string referencia = orow["Referencia"].ToString();
                    string origen = orow["IATAOrigen"].ToString();
                    string ultimock = orow["UltimoCheckpoint"].ToString();
                    string numerocuenta = orow["NumeroCuenta"].ToString();

                    if (numerocuenta.Contains("980550757"))
                    {
                        if (referencia.Contains("2ZD") || referencia.Contains("2T") || referencia.Contains("50H") && origen.Contains("QRO"))
                        {
                            var csrTemp = (from csr in db.csr
                                           where csr.Referencia == referencia.Trim()
                                           select csr).FirstOrDefault();

                            if (csrTemp != null)
                            {
                                string query = "UPDATE csr SET [UltimoCheckpoint] = @UltimoCheckpoINT WHERE ([Referencia] LIKE '%' + @Referencia + '%') AND ([UltimoCheckpoint] != 'OK')";
                                db.Database.ExecuteSqlCommand(query, new SqlParameter("@Referencia", referencia), new SqlParameter("@UltimoCheckpoINT", ultimock));
                                dt.Rows.Remove(orow);
                            }
                        }
                        else
                        {
                            dt.Rows.Remove(orow);
                        }
                    }
                    else
                    {
                        dt.Rows.Remove(orow);
                    }                  
                }

                int filasrestantes = dt.Rows.Count;

                dt.AcceptChanges();
                
                return dt;
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }            
        }

        [HttpPost]
        public ActionResult ObtenerCSR()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var guia = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            //var orden = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();            
            //var direcciondestinatario = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            //var nombredestinatario = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            //var cp = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<csr> lista = new List<csr>();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_CSR_PARAMETROSOPCIONALES] @guia";
                    var query = new SqlCommand(sql, con);

                    if (guia != "")
                    {
                        query.Parameters.AddWithValue("@guia", guia);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@guia", DBNull.Value);
                    }

                    //if (orden != "")
                    //{
                    //    query.Parameters.AddWithValue("@orden", orden);
                    //}
                    //else
                    //{
                    //    query.Parameters.AddWithValue("@orden", DBNull.Value);
                    //}



                    //if (direcciondestinatario != "")
                    //{
                    //    query.Parameters.AddWithValue("@direccion", direcciondestinatario);
                    //}
                    //else
                    //{
                    //    query.Parameters.AddWithValue("@direccion", DBNull.Value);
                    //}

                    //if (nombredestinatario != "")
                    //{
                    //    query.Parameters.AddWithValue("@nombre", nombredestinatario);
                    //}
                    //else
                    //{
                    //    query.Parameters.AddWithValue("@nombre", DBNull.Value);
                    //}

                    //if (cp != "")
                    //{
                    //    query.Parameters.AddWithValue("@cp", cp);
                    //}
                    //else
                    //{
                    //    query.Parameters.AddWithValue("@cp", DBNull.Value);
                    //}

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var csr = new csr();
                            csr.id = Convert.ToInt32(dr["id"]);
                            csr.NumeroCuenta = dr["NumeroCuenta"].ToString();
                            csr.Guia = dr["Guia"].ToString();
                            csr.Referencia = dr["Referencia"].ToString();
                            csr.PieceID = dr["PieceID"].ToString();
                            csr.IATAOrigen = dr["IATAOrigen"].ToString();
                            csr.CiudadOrigen = dr["CiudadOrigen"].ToString();
                            csr.IATADestino = dr["IATADestino"].ToString();
                            csr.CiudadDestino = dr["CiudadDestino"].ToString();
                            csr.SVCSubIATA = dr["SVCSubIATA"].ToString();
                            csr.Ruta = dr["Ruta"].ToString();
                            csr.Piezas = Convert.ToInt32(dr["Piezas"]);
                            csr.Peso = Convert.ToDecimal(dr["Peso"]);
                            csr.FechaRecoleccion = Convert.ToDateTime(dr["FechaRecoleccion"]);
                            csr.FechaUltimoCheckpoint = Convert.ToDateTime(dr["FechaUltimoCheckpoint"]);
                            csr.HoraPrimerCheckpointTerminal = dr["HoraPrimerCheckpointTerminal"].ToString();
                            csr.PrimerCheckpointTerminal = dr["PrimerCheckpointTerminal"].ToString();
                            csr.DescripcionPrimerCheckTerminal = dr["DescripcionPrimerCheckTerminal"].ToString();
                            csr.DetallesEntregaComentarios = dr["DetallesEntregaComentarios"].ToString();
                            csr.TiempoTransitoEstimado = Convert.ToInt32(dr["TiempoTransitoEstimado"]);
                            csr.TiempoTransitoRealizado = Convert.ToInt32(dr["TiempoTransitoRealizado"]);
                            csr.IntentosEntrega = Convert.ToInt32(dr["IntentosEntrega"]);
                            csr.CausaDemora = dr["CausaDemora"].ToString();
                            csr.FechaIngresoCC = Convert.ToDateTime(dr["FechaIngresoCC"]);
                            csr.DiasCC = Convert.ToInt32(dr["DiasCC"]);
                            csr.Producto = dr["Producto"].ToString();
                            csr.ValorSeguro = Convert.ToDecimal(dr["ValorSeguro"]);
                            csr.NombreRemitente = dr["NombreRemitente"].ToString();
                            csr.ContactoRemitente = dr["ContactoRemitente"].ToString();
                            csr.DireccionRemitente = dr["DireccionRemitente"].ToString();
                            csr.CPRemitente = dr["CPRemitente"].ToString();
                            csr.NombreDestinatario = dr["NombreDestinatario"].ToString();
                            csr.ContactoDestinatario = dr["ContactoDestinatario"].ToString();
                            csr.DireccionDestinatario = dr["DireccionDestinatario"].ToString();
                            csr.CPDestinatario = dr["CPDestinatario"].ToString();
                            csr.UltimoCheckpoint = dr["UltimoCheckpoint"].ToString();
                            csr.FechaUltimoCheckpoint = Convert.ToDateTime(dr["FechaUltimoCheckpoint"]);
                            csr.HoraUltimoCheckpoint = dr["HoraUltimoCheckpoint"].ToString();
                            csr.detalleultimocheckpoint = dr["detalleultimocheckpoint"].ToString();

                            lista.Add(csr);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    lista = lista.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = lista.ToList().Count();
                var NewItems = lista.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        // GET: csrs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            csr csr = db.csr.Find(id);
            if (csr == null)
            {
                return HttpNotFound();
            }
            return View(csr);
        }

        // GET: csrs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: csrs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,NumeroCuenta,Guia,PieceID,Referencia,IATAOrigen,CiudadOrigen,IATADestino,CiudadDestino,SVCSubIATA,Ruta,Piezas,Peso,FechaRecoleccion,FechaPrimerCheckpointTerminal,HoraPrimerCheckpointTerminal,PrimerCheckpointTerminal,DescripcionPrimerCheckTerminal,DetallesEntregaComentarios,TiempoTransitoEstimado,TiempoTransitoRealizado,IntentosEntrega,CausaDemora,FechaIngresoCC,DiasCC,Producto,ValorSeguro,NombreRemitente,ContactoRemitente,DireccionRemitente,CPRemitente,NombreDestinatario,ContactoDestinatario,DireccionDestinatario,CPDestinatario,UltimoCheckpoint,FechaUltimoCheckpoint,HoraUltimoCheckpoint,detalleultimocheckpoint")] csr csr)
        {
            if (ModelState.IsValid)
            {
                db.csr.Add(csr);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(csr);
        }

        // GET: csrs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            csr csr = db.csr.Find(id);
            if (csr == null)
            {
                return HttpNotFound();
            }
            return View(csr);
        }

        // POST: csrs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,NumeroCuenta,Guia,PieceID,Referencia,IATAOrigen,CiudadOrigen,IATADestino,CiudadDestino,SVCSubIATA,Ruta,Piezas,Peso,FechaRecoleccion,FechaPrimerCheckpointTerminal,HoraPrimerCheckpointTerminal,PrimerCheckpointTerminal,DescripcionPrimerCheckTerminal,DetallesEntregaComentarios,TiempoTransitoEstimado,TiempoTransitoRealizado,IntentosEntrega,CausaDemora,FechaIngresoCC,DiasCC,Producto,ValorSeguro,NombreRemitente,ContactoRemitente,DireccionRemitente,CPRemitente,NombreDestinatario,ContactoDestinatario,DireccionDestinatario,CPDestinatario,UltimoCheckpoint,FechaUltimoCheckpoint,HoraUltimoCheckpoint,detalleultimocheckpoint")] csr csr)
        {
            if (ModelState.IsValid)
            {
                db.Entry(csr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(csr);
        }

        // GET: csrs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            csr csr = db.csr.Find(id);
            if (csr == null)
            {
                return HttpNotFound();
            }
            return View(csr);
        }

        // POST: csrs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            csr csr = db.csr.Find(id);
            db.csr.Remove(csr);
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
