using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using OpenGate.Entidades;
using System.Linq.Dynamic;
using System.Data.Entity;
using System.IO;

namespace OpenGate.Controllers
{
    public class asignacionordenesController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: asignacionordenes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AsignarOrdenes()
        {
            var ids = from c in db.asignacionordenes
                      select c.Concentrado_Id;


            var query2 = from item in db.concentrado
                         join c in db.csr on item.CSR_Id equals c.id
                         where !ids.Contains(item.id) && c.UltimoCheckpoint != "OK"
                         select item;


            var query = from c in db.concentrado.Where(x => x.csr.UltimoCheckpoint != "OK")
                        where !(from a in db.asignacionordenes select a.Concentrado_Id).Contains(c.id)
                        select c;

            ViewBag.ConteoSinAsignar = query2.Count() <= 0 ? 0 : query.Count();

            return View();
        }

        [HttpPost]
        public ActionResult AsignarOrdenes(string Cantidad, string ddlUsuarios)
        {
            try
            {
                //Cantidad de Ordenes a Asignar
                int cantidadOrdenes = int.Parse(Cantidad);
                //Se utiliza en el ciclo para determinar si la cantiad de registros es igual a la cantidad de ordenes a asignar
                int contador = 0;
                //Cuenta los registros del concentrado los cuales tengan un Estado de DHL diferente a entregado
                var concentrado = from c in db.concentrado.Where(x => x.csr.UltimoCheckpoint != "OK")
                                  where !(from a in db.asignacionordenes select a.Concentrado_Id).Contains(c.id)
                                  select c;

                int conteo = concentrado.Count();

                //Se genera una lista para agregar los registros de la consulta del concentrado
                List<concentrado> lista = new List<concentrado>();

                //Se recorre la lista de concentrados
                foreach (var item in concentrado)
                {
                    //En cada ciclo se busca el id del concentrado en la asignacion
                    var asignacionTemp = db.asignacionordenes.Where(x => x.Concentrado_Id == item.id).FirstOrDefault();

                    //Si el Id del concentrado no esta en la asignacion se agrega en la lista para ingresar
                    if (asignacionTemp == null)
                    {
                        concentrado concentradoTemp = new concentrado();
                        concentradoTemp.id = item.id;
                        concentradoTemp.GuiasImpresas_Id = item.GuiasImpresas_Id;
                        //Se valida si el registro del CSR es nulo esto para buscar la frecuencia y el estado de DHL
                        //Si es nulo se colocaran los valores 
                        int? csrID = item.CSR_Id;

                        if (csrID != null)
                        {
                            var csr = db.csr.Where(x => x.id == csrID).FirstOrDefault();
                            concentradoTemp.CSR_Id = csr.id;
                        }
                        else
                        {
                            concentradoTemp.CSR_Id = 0;
                        }
                        contador++;
                        lista.Add(concentradoTemp);
                    }

                    if (cantidadOrdenes <= contador)
                    {
                        break;
                    }
                }

                //Se recorre la lista para asignar los valores para insertar a la BD
                for (int i = 0; i < lista.Count; i++)
                {
                    //Comienzo la asignacion de valor 
                    asignacionordenes asignacionordenes = new asignacionordenes();
                    asignacionordenes.Concentrado_Id = lista[i].id;
                    asignacionordenes.StatusAsignacion_Id = 2;
                    asignacionordenes.FechaAlta = DateTime.Now;
                    asignacionordenes.AspNetUsers_Id = ddlUsuarios;

                    var idCSR = lista[i].CSR_Id;
                    if (idCSR != 0)
                    {
                        string codigocsr = db.csr.Where(x => x.id == idCSR).FirstOrDefault().UltimoCheckpoint;
                        var idNomenclatura = db.nomenclaturadhl.Where(x => x.codigo == codigocsr).FirstOrDefault();
                        string codigopostal = db.csr.Where(x => x.id == idCSR).FirstOrDefault().CPDestinatario;
                        var idFrecuencia = db.fecuenciadhl.Where(x => x.postalcode == codigopostal).FirstOrDefault();

                        //Se validan ya que puede que los valores pueden estar en el CSR pero no en los catalogos Ejm. Codigo Postal
                        if (idNomenclatura != null)
                        {
                            asignacionordenes.NomenclaturaDHL_Id = idNomenclatura.id;
                        }
                        else
                        {
                            asignacionordenes.NomenclaturaDHL_Id = 9;
                        }

                        if (idFrecuencia != null)
                        {
                            asignacionordenes.FecuenciaDHL_Id = idFrecuencia.id;
                        }
                        else
                        {
                            asignacionordenes.FecuenciaDHL_Id = 29910;
                        }
                    }
                    else
                    {
                        asignacionordenes.NomenclaturaDHL_Id = 9;
                        asignacionordenes.FecuenciaDHL_Id = 29910;
                    }
                    //Se guarda enla base de datos
                    db.asignacionordenes.Add(asignacionordenes);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ObtenerAsignaciones()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var guia = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var observaciones = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var usuario = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var fechaprobableentrega = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var orden = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            int IdError = 0;

            try
            {
                List<asignacionordenes> listAsignacion = new List<asignacionordenes>();
                
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_ASIGNACIONORDEN_PARAMETROSOPCIONALES] @guia, @orden, @observaciones, @usuario";
                    var query = new SqlCommand(sql, con);

                    if (guia != "")
                    {
                        query.Parameters.AddWithValue("@guia", guia);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@guia", DBNull.Value);
                    }

                    if (orden != "")
                    {
                        query.Parameters.AddWithValue("@orden", orden);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@orden", DBNull.Value);
                    }

                    if (observaciones != "" && observaciones != "0")
                    {
                        query.Parameters.AddWithValue("@observaciones", observaciones);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@observaciones", DBNull.Value);
                    }

                    if (usuario != "" && usuario != "0")
                    {
                        query.Parameters.AddWithValue("@usuario", usuario);
                    }
                    else
                    {                        
                        query.Parameters.AddWithValue("@usuario", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var asignacionTemp = new asignacionordenes();

                            IdError = Convert.ToInt32(dr["Expr1"]);

                            asignacionTemp.id = Convert.ToInt32(dr["id"]);                            
                            asignacionTemp.Guia = dr["numero"].ToString();
                            asignacionTemp.Orden = dr["orden"].ToString();
                            asignacionTemp.Referencia = dr["Referencia"].ToString();
                            asignacionTemp.StatusManual = dr["descripcion"].ToString();
                            asignacionTemp.StatusDHL = dr["observaciones"].ToString();
                            asignacionTemp.Comentarios = dr["Comentarios"].ToString();

                            if (dr["CPDestinatario"].ToString().Equals(string.Empty))
                            {
                                asignacionTemp.FechaProbableEntrega = DateTime.MinValue;
                            }
                            else
                            {
                                string cp = dr["CPDestinatario"].ToString();
                                string frecuencia = dr["frequency"].ToString();

                                if (dr["FechaRecoleccion"].ToString().Equals(string.Empty))
                                {
                                    asignacionTemp.FechaRecoleccion = DateTime.MinValue;
                                    asignacionTemp.FechaProbableEntrega = DateTime.MinValue;
                                }
                                else
                                {
                                    DateTime fechaRecoleccion = Convert.ToDateTime(dr["FechaRecoleccion"]);
                                    double dias = double.Parse(frecuencia);
                                    int contador = 0;
                                    int contadorFinSemana = 0;

                                    while (contador < dias)
                                    {
                                        DateTime fechaAux = fechaRecoleccion.AddDays(contador);

                                        if ((int)fechaAux.DayOfWeek == 5 || (int)fechaAux.DayOfWeek == 6)
                                        {
                                            contadorFinSemana = contadorFinSemana + 1;
                                        }
                                        contador++;
                                    }

                                    if (contadorFinSemana > 0)
                                    {
                                        double diasTotal = contadorFinSemana + dias;
                                        DateTime fechaTemp = fechaRecoleccion.AddDays(diasTotal);
                                        string dia = fechaTemp.DayOfWeek.ToString();
                                        DateTime fechaAux;
                                        if (dia == "Saturday")
                                        {
                                            double diaDomingo = diasTotal + 2;
                                            fechaAux = fechaRecoleccion.AddDays(diaDomingo);
                                            asignacionTemp.FechaProbableEntrega = fechaAux;
                                        }
                                        else if (dia == "Sunday")
                                        {
                                            double diaDomingo = diasTotal + 1;
                                            fechaAux = fechaRecoleccion.AddDays(diaDomingo);
                                            asignacionTemp.FechaProbableEntrega = fechaAux;
                                        }
                                        else
                                        {
                                            fechaAux = fechaRecoleccion.AddDays(diasTotal);
                                            asignacionTemp.FechaProbableEntrega = fechaAux;
                                        }
                                    }
                                    else
                                    {
                                        DateTime fechaTemp = fechaRecoleccion.AddDays(dias);
                                        asignacionTemp.FechaProbableEntrega = fechaTemp;
                                    }
                                    asignacionTemp.FechaRecoleccion = Convert.ToDateTime(dr["FechaRecoleccion"]);
                                }                                
                            }

                            if (dr["CSRID"].ToString().Equals(string.Empty))
                            {
                                asignacionTemp.CSRID = 0;
                                asignacionTemp.DSIDCliente = "";
                            }
                            else
                            {
                                asignacionTemp.CSRID = Convert.ToInt32(dr["CSRID"]);
                                asignacionTemp.DSIDCliente = dr["DSIdCliente"].ToString();
                            }                            

                            if (DateTime.Now > asignacionTemp.FechaProbableEntrega)
                            {
                                int dia = (DateTime.Now - asignacionTemp.FechaProbableEntrega).Days;
                                if (dia <= 0)
                                {
                                    asignacionTemp.IsVencido = 1;
                                }
                                else if (dia == 1)
                                {
                                    asignacionTemp.IsVencido = 2;
                                }
                                else if (dia > 1)
                                {
                                    asignacionTemp.IsVencido = 3;
                                }
                            }
                            else
                            {
                                asignacionTemp.IsVencido = 1;
                            }

                            if (fechaprobableentrega != "")
                            {
                                DateTime fecha = Convert.ToDateTime(fechaprobableentrega);
                                if (asignacionTemp.FechaProbableEntrega <= fecha)
                                {
                                    listAsignacion.Add(asignacionTemp);
                                }
                            }
                            else
                            {
                                listAsignacion.Add(asignacionTemp);
                            }                            
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listAsignacion = listAsignacion.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listAsignacion.ToList().Count();
                var NewItems = listAsignacion.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                ViewBag.Error = IdError;
                return null;
            }
        }

        public ActionResult Edit(int id)
        {
            asignacionordenes asignacionordenes = db.asignacionordenes.Where(x => x.id == id).FirstOrDefault();

            ViewBag.StatusAsignacion_Id = new SelectList(db.statusasignacion, "id", "descripcion", asignacionordenes.StatusAsignacion_Id);           

            return View(asignacionordenes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(asignacionordenes asignacionordenes)
        {
            asignacionordenes asignacionTemp = db.asignacionordenes.Where(x => x.id == asignacionordenes.id).FirstOrDefault();
            asignacionTemp.Comentarios = asignacionordenes.Comentarios;
            asignacionTemp.StatusAsignacion_Id = asignacionordenes.StatusAsignacion_Id;

            if (asignacionTemp.StatusAsignacion_Id != 2)
            {
                var csr = asignacionTemp.concentrado.CSR_Id.ToString();

                if (csr != string.Empty)
                {
                    int a = int.Parse(csr);
                    csr csrTemp = db.csr.Where(x => x.id == a).FirstOrDefault();
                    csrTemp.UltimoCheckpoint = "OK";
                    db.SaveChanges();
                }
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Editado Correctamente." });            
        }

        [HttpPost]
        public JsonResult ListaUsuarios()
        {
            List<SelectListItem> listausuarios = new List<SelectListItem>();

            foreach (var item in db.empleado.Where(x => x.Puesto_Id == 9).ToList())
            {
                listausuarios.Add(new SelectListItem
                {
                    Value = item.AspNetUsers_Id.ToString(),
                    Text = item.Nombre
                });
            }

            return Json(listausuarios);
        }

        [HttpPost]
        public JsonResult ListaStatus()
        {
            List<SelectListItem> listastatus = new List<SelectListItem>();

            foreach (var item in db.nomenclaturadhl.GroupBy(x=> x.observaciones).ToList())
            {
                listastatus.Add(new SelectListItem
                {
                    Value = item.Key,
                    Text = item.Key
                });
            }

            return Json(listastatus);
        }

        [HttpPost]
        public ActionResult CerrarOrdenes(string[] dato)
        {
            try
            {
                foreach (var item in dato)
                {                    
                    foreach (string row in item.Split(','))
                    {
                        string reemplazo = row.Replace("r", "").Replace("\\", "").Replace("[", "").Replace("]", "").Remove(0, 1).Remove(10, 1);

                        int id = db.guiasimpresas.Where(x => x.numero.Contains(reemplazo)).FirstOrDefault().id;

                        var asignacionor = (from ao in db.asignacionordenes
                                                join c in db.concentrado on ao.Concentrado_Id equals c.id
                                                where c.GuiasImpresas_Id == id
                                                select ao).FirstOrDefault();

                        asignacionor.StatusAsignacion_Id = 3;
                        db.SaveChanges();
                    }                    
                }

                return Json("Correcto", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }            
        }

        public ActionResult DividirRegistros()
        {            
            return View();
        }

        public List<area> dividirlista(List<area> list, int parts)
        {
            double partLength = list.Count() / (double)parts;

            int i = 0;
            var splits = from name in list
                         group name by Math.Floor((double)(i++ / partLength)) into part
                         select part;

            List<area> lista = new List<area>();
            foreach (var item in splits)
            {
                area area = new area();
                area.id = (int)item.Key;
                lista.Add(area);
            } 

            return lista;
        }

        public static List<T>[] Partition<T>(List<T> list, int totalPartitions)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (totalPartitions < 1)
                throw new ArgumentOutOfRangeException("totalPartitions");

            List<T>[] partitions = new List<T>[totalPartitions];

            int maxSize = (int)Math.Ceiling(list.Count / (double)totalPartitions);
            int k = 0;

            for (int i = 0; i < partitions.Length; i++)
            {
                partitions[i] = new List<T>();
                for (int j = k; j < k + maxSize; j++)
                {
                    if (j >= list.Count)
                        break;
                    partitions[i].Add(list[j]);
                }
                k += maxSize;
            }

            return partitions;
        }
    }
}