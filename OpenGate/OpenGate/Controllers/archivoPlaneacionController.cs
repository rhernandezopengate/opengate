using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using OpenGate.Entidades;

namespace OpenGate.Controllers
{
    public class archivoPlaneacionController : Controller
    {
        dbOpenGateLogisticsEntities db = new dbOpenGateLogisticsEntities();
        // GET: archivoPlaneacion
        public ActionResult Index()
        {
            List<lineasmo> lista = new List<lineasmo>();

            List<String> supplierNames = new List<String>();
            supplierNames.Add("A-0");
            supplierNames.Add("B-0");
            supplierNames.Add("C-0");
            supplierNames.Add("D-0");
            supplierNames.Add("E-0");
            supplierNames.Add("F-0");
            supplierNames.Add("G-0");
            supplierNames.Add("H-0");
            supplierNames.Add("I-0");
            supplierNames.Add("J-0");
            supplierNames.Add("K-0");
            supplierNames.Add("L-0");
            supplierNames.Add("M-0");
            supplierNames.Add("N-0");
            supplierNames.Add("O-0");
            supplierNames.Add("P-0");
            supplierNames.Add("Q-0");
            supplierNames.Add("C4-0");

            //Se agregan todos los resultados a la variable SKU
            string skuBusqueda = "";

            foreach (var item in db.mo.ToList())
            {
                skuBusqueda += item.Column_1;                
            }

            //Valor de separacion de registros en SQL Column_2 " Funciona como Pattern
            char valor = '"';

            String pattern = @"" + valor.ToString();

            String[] elements = Regex.Split(skuBusqueda, pattern);

            //Indica si en el numero de linea se deben borrar 1 Caracter o dos
            int linea = 1;

            string tranfer = "";

            foreach (var element in elements)
            {
                lineasmo mo = new lineasmo();
                
                //Transfer Order
                int indexTranfer = element.Replace(" ", "").IndexOf("TransferOrder");

                if (indexTranfer > 0)
                {
                    tranfer = element.Replace(" ", "").Substring(indexTranfer, 24).Replace("TransferOrder:", "");
                    mo.tranfer = tranfer;
                }
                else
                {
                    tranfer = element.Replace(" ", "").Substring(0, 12);
                    
                    if (tranfer == "\f\fMX1QUERETA")
                    {
                        tranfer = element.Replace(" ", "").Substring(0, 372).Substring(362, 10);
                    }

                    if (tranfer == "\fMX1QUERETAR")
                    {
                        tranfer = element.Replace(" ", "").Substring(0, 372).Substring(361, 10);
                    }

                    mo.tranfer = tranfer;

                }   

                int indexLotNumber = element.Replace(" ", "").IndexOf("LotNumber");
                int indexLotQuantity = element.Replace(" ", "").IndexOf("LotQuantity");
                int indexExpDate = element.Replace(" ", "").IndexOf("Exp.Date");
                int indexWH = element.Replace(" ", "").IndexOf("ANAPATRICIAH-");
                int indexQty = element.Replace(" ", "").IndexOf("EA");                
                int indexEtiquetado = element.Replace(" ", "").IndexOf("V");
                int indexTranferOrder = element.Replace(" ", "").IndexOf("1167427414Allocated");
                
                int valorLinea = linea++;

                string lineaMO = "";
                string sku = "";
                string ubicacion = "";
                string numeroLote = "";
                string cantidadLote = "";
                string fechaExpiracion = "";
                string wharehouse = "";
                string cantidad = "";

                //Linea 
                if (indexTranferOrder > 0)
                {
                    
                    if (valorLinea < 10)
                    {
                        lineaMO = element.Replace(" ", "").Substring(indexTranferOrder, 29).Replace("1167427414Allocated", "").Substring(0, 1);
                    }
                    else
                    {
                        lineaMO = element.Replace(" ", "").Substring(indexTranferOrder, 29).Replace("1167427414Allocated", "").Substring(0, 2);
                    }
                    
                    mo.linea = lineaMO;
                }

                //SKU 
                if (indexTranferOrder > 0)
                {
                    sku = element.Replace(" ", "").Substring(indexTranferOrder, 29).Replace("1167427414Allocated"+lineaMO, "").Substring(0,6);
                    mo.sku = sku;
                }

                //Numero Lote 
                if (indexLotNumber > 0)
                {
                    numeroLote = element.Replace(" ", "").Substring(indexLotNumber, 19);
                    mo.lotNumber = numeroLote.Replace("LotNumber:", "").Replace("Ex", "");
                }

                //Cantidad por lote
                if (indexLotQuantity > 0)
                {
                    cantidadLote = element.Replace(" ", "") + "   ";
                    mo.lotQty = cantidadLote.Substring(indexLotQuantity, 18).Replace("LotQuantity:", "");
                }

                if (indexExpDate > 0)
                {
                    fechaExpiracion = element.Replace(" ", "") + "   ";
                    mo.ExpDate = fechaExpiracion.Substring(indexExpDate, 18).Replace("Exp.Date:", "");
                }

                //Wharehouse
                if (indexWH > 0)
                {
                    wharehouse = element.Replace(" ", "") + "   ";
                    mo.wh = wharehouse.Substring(indexWH, 25).Replace("ANAPATRICIAH-", "").Substring(0,2);
                }

                //Cantidad
                if (indexQty > 0)
                {
                    cantidad = element.Replace(" ", "") + "   ";
                    mo.qty = cantidad.Substring(indexQty, 8).Replace("EA", "").Replace("_", "");
                }

                //Ubicacion
                if (indexEtiquetado > 0)
                {
                    ubicacion = element.Replace(" ", "").Substring(indexEtiquetado, 30);

                    foreach (var item in supplierNames)
                    {
                        int indiceDelimiter = ubicacion.IndexOf(item.ToString());

                        if (indiceDelimiter > 0)
                        {
                            mo.ubicacion = ubicacion.Substring(indiceDelimiter, 8);
                        }
                    }                   
                }

                lista.Add(mo);
            }               

            return View(lista);
        }

        public ActionResult Create()
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
                    dt.Columns.AddRange(new DataColumn[2] {
                        new DataColumn("column_1", typeof(string)),
                        new DataColumn("column_2", typeof(string))
                    });


                    string csvData = System.IO.File.ReadAllText(filePath);

                    char cadena = Convert.ToChar("-");

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
                            sqlBulkCopy.DestinationTableName = "[dbo].[mo]";

                            //[OPTIONAL]: Map the DataTable columns with that of the database table                            
                            sqlBulkCopy.ColumnMappings.Add("column_1", "Column_1");
                            sqlBulkCopy.ColumnMappings.Add("column_2", "column_2");                            

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
    }
}