using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Configuration;

namespace OpenGate.Controllers
{
    public class concentradoguiasController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: concentradoguias
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult ObtenerConcentrado()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var guia = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<concentrado> listaConcentrado = new List<concentrado>();       

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_CONCENTRADO_PARAMETROSOPCIONALES] @order";
                    var query = new SqlCommand(sql, con);

                    if (guia != "")
                    {
                        query.Parameters.AddWithValue("@order", guia);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@order", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var concentrado = new concentrado();

                            concentrado.IdGuia = Convert.ToInt32(dr["id"]);
                            concentrado.Guia = dr["Guia"].ToString();
                            concentrado.ChekPoint = dr["UltimoCheckpoint"].ToString();

                            if (dr["NTSDate"].ToString() == "")
                            {
                                concentrado.NTSDate = DateTime.MinValue;
                                concentrado.NTS = string.Empty;
                            }
                            else
                            {
                                concentrado.NTSDate = Convert.ToDateTime(dr["NTSDate"]);
                                concentrado.NTS = dr["Order"].ToString();
                            }

                            if (dr["Referencia"].ToString() == "")
                            {
                                concentrado.ReferenciaCSR = "NA";
                                concentrado.Concatenado = "NA";
                            }
                            else
                            {
                                concentrado.ReferenciaCSR = dr["Referencia"].ToString();

                                if (dr["CPDestinatario"].ToString().Length < 4)
                                {
                                    concentrado.Concatenado = dr["ContactoDestinatario"].ToString() + "/ " + dr["DireccionDestinatario"].ToString() + "/ " + "0" + dr["CPDestinatario"].ToString();
                                }
                                else
                                {
                                    concentrado.Concatenado = dr["ContactoDestinatario"].ToString() + "/ " + dr["DireccionDestinatario"].ToString() + "/ " + dr["CPDestinatario"].ToString();
                                }
                            }
                            listaConcentrado.Add(concentrado);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listaConcentrado = listaConcentrado.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listaConcentrado.ToList().Count();
                var NewItems = listaConcentrado.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;                
            }            
        }

        public ActionResult CargarNTS()
        {            
            try
            {
                //Se validan las guias que no existen en el concentrado
                var query = from guias in db.guiasimpresas  where !(from con in db.concentrado select con.id ).Contains(guias.id) select guias;
                
                foreach (var item in query)
                {
                    concentrado concentrados = new concentrado();
                    concentrados.GuiasImpresas_Id = item.id;
                    var csrs = db.csr.Where(x => x.Referencia.Contains(item.orden)).FirstOrDefault();

                    if (csrs != null)
                    {
                        concentrados.CSR_Id = csrs.id;
                    }
                    else
                    {
                        concentrados.csr = null;
                    }

                    var nts = db.nts.Where(x => x.Order.Contains(item.orden)).FirstOrDefault();

                    if (nts != null)
                    {
                        concentrados.NTS_Id = nts.id;
                    }
                    else
                    {
                        concentrados.NTS_Id = null;
                    }

                    var validacion = db.concentrado.Where(x => x.GuiasImpresas_Id == item.id).FirstOrDefault();                    

                    if (validacion == null)
                    {
                        db.concentrado.Add(concentrados);                        
                    }                    
                }

                db.SaveChanges();

                return ActualizarConcentrado() == true ? Json("Success", JsonRequestBehavior.AllowGet) : Json("Error", JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }            
        }

        public bool ActualizarConcentrado()
        {
            try
            {
                var concentradoactualizar = from c in db.concentrado
                                            join csrst in db.csr 
                                            on c.CSR_Id equals csrst.id into cGroup
                                            from d in cGroup.DefaultIfEmpty()
                                            select new
                                            {
                                                conentradoId = c.id,
                                                referenciaCsr = d == null ? "NA" : d.Referencia,
                                                statusCsr = d == null ? "NA" : d.UltimoCheckpoint
                                            };

                int cantidad = concentradoactualizar.Count();                
                List<concentrado> listaTemp = new List<concentrado>();
                int contador = 0;
                foreach (var item in concentradoactualizar)
                {
                    if (item.statusCsr != "OK" || item.statusCsr == "NA")
                    {                        
                        concentrado concentrado = db.concentrado.Where(x => x.id == item.conentradoId).FirstOrDefault();
                        concentrado.ReferenciaCSR = item.referenciaCsr;
                        contador++;
                        listaTemp.Add(concentrado);
                    }             
                }

                int conteo = contador;
                foreach (var item in listaTemp)
                {
                    concentrado concentrado = db.concentrado.Where(x => x.id == item.id).FirstOrDefault();
                    string orden = concentrado.guiasimpresas.orden.ToString();

                    var csr = db.csr.Where(x => x.Referencia.Contains(orden)).FirstOrDefault();
                    var nts = db.nts.Where(x => x.Order.Contains(orden)).FirstOrDefault();

                    if (concentrado.csr == null)
                    {
                        if (csr != null)
                        {
                            concentrado.CSR_Id = csr.id;
                        }
                    }

                    if (concentrado.nts == null)
                    {
                        if (nts != null)
                        {
                            concentrado.NTS_Id = nts.id;
                        }
                    }

                    db.SaveChanges();
                }               
                
                return true;
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return false;
            }
        }
    }
}