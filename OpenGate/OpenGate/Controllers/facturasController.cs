using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OpenGate.Entidades;
using System.Linq.Dynamic;
using System.Web.Security;
using Newtonsoft.Json;

namespace OpenGate.Controllers
{
    [Authorize]
    public class facturasController : Controller
    {
        private dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();

        [Authorize(Roles = "admin, tesoreria, finanzas, contabilidad")]
        public ActionResult Default()
        {
            var cantidaddebe = from f in db.factura
                               where f.StatusFactura_Id == 1
                               select new { f.Total };

            var cantidaddetenido = from f in db.factura
                               where f.StatusFactura_Id == 4
                               select new { f.Total };

            var cantidadvencido = from f in db.factura
                                   where f.StatusFactura_Id == 1
                                   select new { f.FechaVencimiento, f.id, f.Total };

            decimal? vencido = 0;
            DateTime? hoy = DateTime.Now;
            foreach (var item in cantidadvencido)
            {
                TimeSpan span = hoy.Value.Subtract((DateTime)item.FechaVencimiento);

                if(span.Days > 0)
                {
                    vencido += item.Total;                                    
                }
            }

            decimal? debe = 0;
            foreach (var item in cantidaddebe)
            {
                debe += item.Total;
            }

            decimal? detenido = 0;
            foreach (var item in cantidaddetenido)
            {
                detenido += item.Total;
            }

            ViewBag.Vencido = String.Format("{0:C}", vencido);
            ViewBag.Debe = String.Format("{0:C}", debe);
            ViewBag.Detenido = String.Format("{0:C}", detenido);
                             
            return View();
        }


        public ActionResult ReporteFacturacionClientes(string mes) 
        {
            try
            {
                List<DataPoint> dataPoints = new List<DataPoint>();
                
                int mesActual = int.Parse(mes);

                var consulta = (from f in db.factura
                                where f.FechaFactura.Value.Month.Equals(12) && f.FechaFactura.Value.Year.Equals(2019)
                                select new { cliente = f.clientes.razonsocial, f.Total }).ToList();

                foreach (var item in consulta.GroupBy(x => x.cliente))
                {
                    dataPoints.Add(new DataPoint(item.Key, (double)item.Sum(x => x.Total)));
                }



                var consultatabla = (from f in db.factura                                     
                                     group f by new { f.clientes.razonsocial, f.FechaFactura.Value.Month } into g
                                     select new Group<string, factura> {  });




                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

                return PartialView();
            }
            catch (Exception)
            {
                return PartialView();
            }        
        }


        #region Vista Reporte Desglozado

        [Authorize(Roles = "admin, tesoreria, finanzas, contabilidad")]
        public ActionResult ReporteDesglozado()
        {
            return View();
        }

        [Authorize(Roles = "admin, tesoreria, finanzas, contabilidad")]
        public ActionResult VistaContabilidad()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var Numero = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var FechaFactura = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var FechaFacturaFin = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var FechaPago = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var FechaPagoFin = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var RazonSocial = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
            var Concepto = Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();
            var StatusFactura = Request.Form.GetValues("columns[8][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<factura> listFacturas = new List<factura>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_FACTURAS_PARAMETROSOPCIONALES_CONTABILIDAD] @fechaFInicio, @fechaFFin, @numero, @fechaPInicio, @fechaPFin, @idproveedor, @concepto, @idstatusfactura";
                    var query = new SqlCommand(sql, con);

                    if (FechaFactura != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaFactura);
                        query.Parameters.AddWithValue("@fechaFInicio", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaFInicio", DBNull.Value);
                    }

                    if (FechaFacturaFin != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaFacturaFin);
                        query.Parameters.AddWithValue("@fechaFFin", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaFFin", DBNull.Value);
                    }

                    if (FechaPago != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaPago);
                        query.Parameters.AddWithValue("@fechaPInicio", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaPInicio", DBNull.Value);
                    }

                    if (FechaPagoFin != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaPagoFin);
                        query.Parameters.AddWithValue("@fechaPFin", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaPFin", DBNull.Value);
                    }

                    if (RazonSocial == "" || RazonSocial == "0")
                    {
                        query.Parameters.AddWithValue("@idproveedor", DBNull.Value);
                    }
                    else
                    {
                        var querys = db.proveedor.FirstOrDefault(x => x.RazonSocial == RazonSocial);

                        if (querys != null)
                        {
                            query.Parameters.AddWithValue("@idproveedor", querys.id);
                        }
                        else
                        {
                            query.Parameters.AddWithValue("@idproveedor", RazonSocial);
                        }
                    }

                    if (Numero != "")
                    {
                        query.Parameters.AddWithValue("@numero", Numero);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@numero", DBNull.Value);
                    }


                    if (Concepto != "")
                    {
                        query.Parameters.AddWithValue("@concepto", Concepto);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@concepto", DBNull.Value);
                    }

                    if (StatusFactura == "" || StatusFactura == "0")
                    {
                        query.Parameters.AddWithValue("@idstatusfactura", DBNull.Value);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@idstatusfactura", StatusFactura);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var facturas = new factura();

                            facturas.id = Convert.ToInt32(dr["id"]);
                            facturas.Numero = dr["Numero"].ToString();
                            facturas.FechaFactura = Convert.ToDateTime(dr["FechaFactura"]);
                            facturas.FechaFacturaFin = Convert.ToDateTime(dr["FechaFactura"]);

                            if (dr["FechaPago"] != DBNull.Value)
                            {
                                facturas.FechaPagoString = Convert.ToDateTime(dr["FechaPago"].ToString()).Date.ToShortDateString();
                                facturas.FechaPagoFinString = Convert.ToDateTime(dr["FechaPago"].ToString()).Date.ToShortDateString();
                            }
                            else
                            {
                                facturas.FechaPagoString = "";
                                facturas.FechaPagoFinString = "";
                            }

                            //facturas.FechaPagoFin = Convert.ToDateTime(dr["FechaPago"]);
                            facturas.RazonSocial = dr["razonsocial"].ToString();
                            facturas.Concepto = dr["Concepto"].ToString();
                            facturas.Estado = dr["descripcion"].ToString();
                            decimal? subtotal = Convert.ToDecimal(dr["Subtotal"]);
                            decimal? iva = Convert.ToDecimal(dr["Iva"]);
                            decimal? descuento = Convert.ToDecimal(dr["Descuento"]);
                            decimal? retencion = Convert.ToDecimal(dr["Retencion"]);
                            decimal? total = Convert.ToDecimal(dr["Total"]);
                            facturas.SubTotalString = String.Format("{0:C}", subtotal);
                            facturas.IvaString = String.Format("{0:C}", iva);
                            facturas.DescuentoString = String.Format("{0:C}", descuento);
                            facturas.RetencionString = String.Format("{0:C}", retencion);
                            facturas.TotalString = String.Format("{0:C}", total);

                            facturas.Estado = dr["descripcion"].ToString();

                            listFacturas.Add(facturas);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listFacturas = listFacturas.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listFacturas.ToList().Count();
                var NewItems = listFacturas.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        #endregion

        #region Vista Index
        [Authorize(Roles = "admin, tesoreria, finanzas, contabilidad")]
        public ActionResult Index()
        {
            return View(db.factura.ToList());
        }
        [Authorize(Roles = "admin, tesoreria, finanzas, contabilidad")]
        [HttpPost]
        public ActionResult getFacturas()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var DV = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var FechaFactura = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var FechaFacturaFin = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var FechaVencimiento = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var FechaVencimientoFin = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var Numero = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
            var RazonSocial = Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();
            var FechaPago = Request.Form.GetValues("columns[8][search][value]").FirstOrDefault();
            var FechaPagoFin = Request.Form.GetValues("columns[9][search][value]").FirstOrDefault();
            var Concepto = Request.Form.GetValues("columns[10][search][value]").FirstOrDefault();
            var Total = Request.Form.GetValues("columns[11][search][value]").FirstOrDefault();
            var Observaciones = Request.Form.GetValues("columns[12][search][value]").FirstOrDefault();
            var Estado = Request.Form.GetValues("columns[13][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<factura> listFacturas = new List<factura>();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_FACTURAS_PARAMETROSOPCIONALES] @fechaFInicio, @fechaFFin, @numero, @fechaPInicio, @fechaPFin, @fechaVInicio, @fechaVFin, @idproveedor, @idstatus, @concepto, @observaciones";
                    var query = new SqlCommand(sql, con);

                    if (FechaFactura != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaFactura);
                        query.Parameters.AddWithValue("@fechaFInicio", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaFInicio", DBNull.Value);
                    }

                    if (FechaFacturaFin != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaFacturaFin);
                        query.Parameters.AddWithValue("@fechaFFin", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaFFin", DBNull.Value);
                    }

                    if (FechaVencimiento != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaVencimiento);
                        query.Parameters.AddWithValue("@fechaVInicio", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaVInicio", DBNull.Value);
                    }

                    if (FechaVencimientoFin != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaVencimientoFin);
                        query.Parameters.AddWithValue("@fechaVFin", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaVFin", DBNull.Value);
                    }

                    if (FechaPago != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaPago);
                        query.Parameters.AddWithValue("@fechaPInicio", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaPInicio", DBNull.Value);
                    }

                    if (FechaPagoFin != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaPagoFin);
                        query.Parameters.AddWithValue("@fechaPFin", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaPFin", DBNull.Value);
                    }

                    if (RazonSocial == "" || RazonSocial == "0")
                    {
                        query.Parameters.AddWithValue("@idproveedor", DBNull.Value);
                    }
                    else
                    {
                        var querys = db.proveedor.FirstOrDefault(x => x.RazonSocial == RazonSocial);

                        if (querys != null)
                        {
                            query.Parameters.AddWithValue("@idproveedor", querys.id);
                        }
                        else
                        {
                            query.Parameters.AddWithValue("@idproveedor", RazonSocial);
                        }
                    }

                    if (Numero != "")
                    {
                        query.Parameters.AddWithValue("@numero", Numero);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@numero", DBNull.Value);
                    }

                    if (Estado == "" || Estado == "0")
                    {
                        query.Parameters.AddWithValue("@idstatus", DBNull.Value);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@idstatus", Estado);
                    }

                    if (Concepto != "")
                    {
                        query.Parameters.AddWithValue("@concepto", Concepto);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@concepto", DBNull.Value);
                    }

                    if (Observaciones != "")
                    {
                        query.Parameters.AddWithValue("@observaciones", Observaciones);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@observaciones", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var facturas = new factura();

                            facturas.id = Convert.ToInt32(dr["id"]);

                            if (dr["FechaPago"] != DBNull.Value)
                            {
                                facturas.FechaPagoString = Convert.ToDateTime(dr["FechaPago"].ToString()).Date.ToShortDateString();
                                facturas.FechaPagoFinString = Convert.ToDateTime(dr["FechaPago"].ToString()).Date.ToShortDateString();
                            }
                            else
                            {
                                facturas.FechaPagoString = "";
                                facturas.FechaPagoFinString = "";
                            }
                            facturas.FechaFactura = Convert.ToDateTime(dr["FechaFactura"]);
                            facturas.FechaFacturaFin = Convert.ToDateTime(dr["FechaFactura"]);
                            facturas.FechaVencimiento = Convert.ToDateTime(dr["FechaVencimiento"]);
                            facturas.FechaVencimientoFin = Convert.ToDateTime(dr["FechaVencimiento"]);
                            facturas.Numero = dr["Numero"].ToString();
                            facturas.Concepto = dr["Concepto"].ToString();
                            facturas.Total = Convert.ToDecimal(dr["Total"]);
                            facturas.Observaciones = dr["Observaciones"].ToString();
                            facturas.RazonSocial = dr["razonsocial"].ToString();
                            decimal? value = facturas.Total;
                            facturas.TotalString = String.Format("{0:C}", value);

                            if (dr["descripcion"].ToString() == "Pagada")
                            {
                                facturas.dv = 0;
                            }
                            else
                            {
                                facturas.dv = Convert.ToInt32(dr["Duration"]);
                            }

                            facturas.Estado = dr["descripcion"].ToString();

                            listFacturas.Add(facturas);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listFacturas = listFacturas.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listFacturas.ToList().Count();
                var NewItems = listFacturas.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public JsonResult ListaStatus()
        {
            List<SelectListItem> liststatus = new List<SelectListItem>();

            foreach (var item in db.statusfactura.ToList())
            {
                liststatus.Add(new SelectListItem
                {
                    Value = item.id.ToString(),
                    Text = item.descripcion
                });
            }

            return Json(liststatus);
        }
        #endregion

        #region Pago Individual de Facturas

        [HttpPost]
        [Authorize(Roles = "admin, tesoreria, finanzas, contabilidad")]
        public ActionResult PagarFactura(int? id)
        {
            try
            {
                factura factura = db.factura.FirstOrDefault(x => x.id == id);
                if (factura != null)
                {
                    if (factura.StatusFactura_Id != 2)
                    {
                        var numeroActual = db.pago.ToList().Count();
                        int numeroPago = int.Parse(numeroActual.ToString()) + 1;

                        factura.StatusFactura_Id = 2;
                        factura.FechaPago = DateTime.Now;

                        pago pagos = new pago();
                        pagos.Numero = numeroPago;
                        pagos.FechaPago = DateTime.Now;
                        pagos.FechaRegistro = DateTime.Now;
                        pagos.AspNetUsers_Id = User.Identity.GetUserId();
                        pagos.MontoTotal = factura.Total;
                        pagos.StatusPago_Id = 1;
                        db.pago.Add(pagos);
                        db.SaveChanges();

                        pagofacturas pagosFacturas = new pagofacturas();
                        pagosFacturas.Factura_Id = (int)id;
                        pagosFacturas.Pago_Id = pagos.id;
                        db.pagofacturas.Add(pagosFacturas);
                        db.SaveChanges();

                        return Json("Success", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Error", JsonRequestBehavior.AllowGet);
                    }
                }
                return null;
            }
            catch (Exception _ex)
            {
                Console.Write(_ex.Message.ToString());
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Vista de Trafico

        [Authorize(Roles = "admin, tesoreria, finanzas, contabilidad, trafico, gerentegeneral")]
        public ActionResult VistaTrafico()
        {
            return View();
        }

        [Authorize(Roles = "admin, tesoreria, finanzas, contabilidad, trafico, gerentegeneral")]
        public ActionResult VistaTraficoData()
        {
            var Draw = Request.Form.GetValues("draw").FirstOrDefault();
            var Start = Request.Form.GetValues("start").FirstOrDefault();
            var Length = Request.Form.GetValues("length").FirstOrDefault();
            var SortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
            var SortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            var Numero = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            var FechaFactura = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            var FechaFacturaFin = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
            var FechaPago = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            var FechaPagoFin = Request.Form.GetValues("columns[5][search][value]").FirstOrDefault();
            var RazonSocial = Request.Form.GetValues("columns[6][search][value]").FirstOrDefault();
            var Concepto = Request.Form.GetValues("columns[7][search][value]").FirstOrDefault();

            int PageSize = Length != null ? Convert.ToInt32(Length) : 0;
            int Skip = Start != null ? Convert.ToInt32(Start) : 0;
            int TotalRecords = 0;

            try
            {
                List<factura> listFacturas = new List<factura>();

                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();

                    string sql = "exec [SP_FACTURAS_PARAMETROSOPCIONALES_TRAFICO] @fechaFInicio, @fechaFFin, @numero, @fechaPInicio, @fechaPFin, @idproveedor, @concepto";
                    var query = new SqlCommand(sql, con);

                    if (FechaFactura != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaFactura);
                        query.Parameters.AddWithValue("@fechaFInicio", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaFInicio", DBNull.Value);
                    }

                    if (FechaFacturaFin != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaFacturaFin);
                        query.Parameters.AddWithValue("@fechaFFin", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaFFin", DBNull.Value);
                    }

                    if (FechaPago != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaPago);
                        query.Parameters.AddWithValue("@fechaPInicio", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaPInicio", DBNull.Value);
                    }

                    if (FechaPagoFin != "")
                    {
                        DateTime date = Convert.ToDateTime(FechaPagoFin);
                        query.Parameters.AddWithValue("@fechaPFin", date);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@fechaPFin", DBNull.Value);
                    }

                    if (RazonSocial == "" || RazonSocial == "0")
                    {
                        query.Parameters.AddWithValue("@idproveedor", DBNull.Value);
                    }
                    else
                    {
                        var querys = db.proveedor.FirstOrDefault(x => x.RazonSocial == RazonSocial);

                        if (querys != null)
                        {
                            query.Parameters.AddWithValue("@idproveedor", querys.id);
                        }
                        else
                        {
                            query.Parameters.AddWithValue("@idproveedor", RazonSocial);
                        }
                    }

                    if (Numero != "")
                    {
                        query.Parameters.AddWithValue("@numero", Numero);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@numero", DBNull.Value);
                    }

                    if (Concepto != "")
                    {
                        query.Parameters.AddWithValue("@concepto", Concepto);
                    }
                    else
                    {
                        query.Parameters.AddWithValue("@concepto", DBNull.Value);
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // facturas
                            var facturas = new factura();

                            facturas.id = Convert.ToInt32(dr["id"]);
                            facturas.Numero = dr["Numero"].ToString();
                            facturas.FechaFactura = Convert.ToDateTime(dr["FechaFactura"]);
                            facturas.FechaFacturaFin = Convert.ToDateTime(dr["FechaFactura"]);
                            facturas.FechaPago = Convert.ToDateTime(dr["FechaPago"]);
                            facturas.FechaPagoFin = Convert.ToDateTime(dr["FechaPago"]);
                            facturas.RazonSocial = dr["razonsocial"].ToString();
                            facturas.Concepto = dr["Concepto"].ToString();
                            facturas.Estado = dr["descripcion"].ToString();
                            decimal? subtotal = Convert.ToDecimal(dr["Subtotal"]);
                            decimal? iva = Convert.ToDecimal(dr["Iva"]);
                            decimal? descuento = Convert.ToDecimal(dr["Descuento"]);
                            decimal? retencion = Convert.ToDecimal(dr["Retencion"]);
                            decimal? total = Convert.ToDecimal(dr["Total"]);
                            facturas.SubTotalString = String.Format("{0:C}", subtotal);
                            facturas.IvaString = String.Format("{0:C}", iva);
                            facturas.DescuentoString = String.Format("{0:C}", descuento);
                            facturas.RetencionString = String.Format("{0:C}", retencion);
                            facturas.TotalString = String.Format("{0:C}", total);

                            facturas.Estado = dr["descripcion"].ToString();

                            listFacturas.Add(facturas);
                        }
                    }
                }

                if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDir)))
                {
                    listFacturas = listFacturas.OrderBy(SortColumn + " " + SortColumnDir).ToList();
                }

                TotalRecords = listFacturas.ToList().Count();
                var NewItems = listFacturas.Skip(Skip).Take(PageSize == -1 ? TotalRecords : PageSize).ToList();

                return Json(new { draw = Draw, recordsFiltered = TotalRecords, recordsTotal = TotalRecords, data = NewItems }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        #endregion

        #region CRUD

        [Authorize(Roles = "admin")]
        public ActionResult VistaGeneral()
        {
            return View(db.factura.ToList());
        }


        // GET: facturas/Details/5
        [Authorize(Roles = "admin, tesoreria, finanzas, contabilidad")]
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var rslt = from f in db.factura
                           where f.id == id
                           select new
                           {
                               f.id,
                               f.Numero,
                               f.Total,
                               f.Retencion,
                               f.Concepto,
                               f.Iva,
                               f.Descuento,
                               f.Subtotal,
                               estado = f.statusfactura.descripcion,
                               razon = f.proveedor.RazonSocial,
                               f.FechaFactura,
                               f.FechaSello,
                               f.FechaVencimiento,
                               f.Observaciones
                           };

                if (rslt == null)
                {
                    return HttpNotFound();
                }

                return Json(rslt, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // GET: facturas/Create
        [Authorize(Roles = "admin, tesoreria")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: facturas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "admin, tesoreria")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,FechaRegistro,Numero,FechaFactura,FechaSello,FechaVencimiento,FechaPago,Concepto,Subtotal,Iva,Descuento,Retencion,Total,Observaciones,Proveedor_Id,StatusFactura_Id,AspNetUsers_Id,Clientes_Id")] factura factura)
        {
            if (ModelState.IsValid)
            {
                factura.FechaRegistro = DateTime.Now;
                factura.FechaPago = null;
                factura.AspNetUsers_Id = User.Identity.GetUserId();


                var querys = db.proveedor.FirstOrDefault(x => x.id == factura.Proveedor_Id);

                if (querys.StatusProveedor_Id == 2)
                {
                    factura.StatusFactura_Id = 4;
                }
                else
                {
                    factura.StatusFactura_Id = 1;
                }

                db.factura.Add(factura);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Proveedor_Id = new SelectList(db.proveedor, "id", "razonsocial", factura.Proveedor_Id);
            ViewBag.StatusFactura_Id = new SelectList(db.statusfactura, "id", "descripcion", factura.StatusFactura_Id);

            return View(factura);
        }

        // GET: facturas/Edit/5
        [Authorize(Roles = "admin, tesoreria, finanzas, contabilidad")]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                factura factura = db.factura.Find(id);
                if (factura == null)
                {
                    return HttpNotFound();
                }

                ViewBag.Proveedor_Id = new SelectList(db.proveedor, "id", "razonsocial", factura.Proveedor_Id);
                if (factura.StatusFactura_Id == 2)
                {
                    ViewBag.StatusFactura_Id = new SelectList(db.statusfactura.Where(x => x.id == 2), "id", "descripcion", factura.StatusFactura_Id);
                    ViewBag.Deshabilitado = "1";
                }
                else
                {
                    ViewBag.StatusFactura_Id = new SelectList(db.statusfactura.Where(x => x.id != 2), "id", "descripcion", factura.StatusFactura_Id);
                }
                ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email", factura.AspNetUsers_Id);
                ViewBag.Clientes_Id = new SelectList(db.clientes, "id", "razonsocial", factura.Clientes_Id);
                factura.proveedorString = db.proveedor.Find(factura.Proveedor_Id).RazonSocial;

                return PartialView("_EditEmp", factura);
            }
            catch (Exception _ex)
            {
                Console.WriteLine(_ex.Message.ToString());
                return null;
            }
        }

        // POST: facturas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "admin, tesoreria, contabilidad")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,FechaRegistro,Numero,FechaFactura,FechaSello,FechaVencimiento,FechaPago,Concepto,Subtotal,Iva,Descuento,Retencion,Total,Observaciones,Proveedor_Id,StatusFactura_Id,AspNetUsers_Id,Clientes_Id")] factura factura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(factura).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Editado Correctamente." });
            }

            ViewBag.Proveedor_Id = new SelectList(db.proveedor, "id", "razonsocial", factura.Proveedor_Id);
            ViewBag.Clientes_Id = new SelectList(db.clientes, "id", "razonsocial", factura.Clientes_Id);
            ViewBag.StatusFactura_Id = new SelectList(db.statusfactura, "id", "descripcion", factura.StatusFactura_Id);
            ViewBag.AspNetUsers_Id = new SelectList(db.AspNetUsers, "Id", "Email", factura.AspNetUsers_Id);

            return PartialView("_EditEmp", factura);
        }

        // GET: facturas/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            factura factura = db.factura.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // POST: facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            factura factura = db.factura.Find(id);
            db.factura.Remove(factura);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public string ValidarProveedorFactura(string numero, string razon)
        {
            if (razon == "")
            {
                List<factura> facturas = db.factura.Where(x => x.Numero == numero).ToList();
                if (facturas.Count != 0)
                {
                    string proveedor = "";

                    foreach (var item in facturas)
                    {
                        proveedor += "<ul class='center'> <li> <h2>" + item.proveedor.RazonSocial + "</h2> </li> </ul>";
                    }

                    return proveedor;
                }
                else
                {
                    string sinfactura = "Sin facturas";
                    return sinfactura;
                }
            }
            else
            {
                List<factura> facturas = db.factura.Where(x => x.Numero == numero && x.proveedor.RazonSocial == razon).ToList();
                if (facturas.Count != 0)
                {
                    string proveedor = "";

                    foreach (var item in facturas)
                    {
                        proveedor += "<ul class='center'> <li> <h2>" + item.proveedor.RazonSocial + "</h2> </li> </ul>";
                    }

                    return proveedor;
                }
                else
                {
                    string sinfactura = "Sin facturas";
                    return sinfactura;
                }
            }
        }

        #endregion

        public JsonResult CalcularFechaVencimiento(string fechaSello)
        {
            DateTime fecha = Convert.ToDateTime(fechaSello, System.Globalization.CultureInfo.GetCultureInfo("es-MX").DateTimeFormat);
            fecha = fecha.AddDays(30);
            return Json(fecha.ToShortDateString());
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
