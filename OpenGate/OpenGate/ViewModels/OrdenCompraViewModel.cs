using OpenGate.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenGate.ViewModels
{
    public class OrdenCompraViewModel
    {
        #region Cabecera
        public string Cliente { get; set; }
        public int CabeceraProductoId { get; set; }
        public string CabeceraProductoNombre { get; set; }
        public int CabeceraProductoCantidad { get; set; }
        public decimal CabeceraProductoPrecio { get; set; }
        #endregion

        #region Contenido
        public List<DetalleOrdenProductosViewModel> ComprobanteDetalle { get; set; }
        #endregion

        #region Pie
        public decimal Total()
        {
            return ComprobanteDetalle.Sum(x => x.Monto());
        }
        public DateTime Creado { get; set; }
        #endregion

        public OrdenCompraViewModel()
        {
            ComprobanteDetalle = new List<DetalleOrdenProductosViewModel>();
            Refrescar();
        }

        public void Refrescar()
        {
            CabeceraProductoId = 0;
            CabeceraProductoNombre = null;
            CabeceraProductoCantidad = 1;
            CabeceraProductoPrecio = 0;
        }

        public bool SeAgregoUnProductoValido()
        {
            return !(CabeceraProductoId == 0 || string.IsNullOrEmpty(CabeceraProductoNombre) || CabeceraProductoCantidad == 0 || CabeceraProductoPrecio == 0);
        }

        public bool ExisteEnDetalle(int ProductoId)
        {
            return ComprobanteDetalle.Any(x => x.ProductoId == ProductoId);
        }

        public void RetirarItemDeDetalle()
        {
            if (ComprobanteDetalle.Count > 0)
            {
                var detalleARetirar = ComprobanteDetalle.Where(x => x.Retirar)
                                                        .SingleOrDefault();

                ComprobanteDetalle.Remove(detalleARetirar);
            }
        }

        public void AgregarItemADetalle()
        {
            ComprobanteDetalle.Add(new DetalleOrdenProductosViewModel
            {
                ProductoId = CabeceraProductoId,
                ProductoNombre = CabeceraProductoNombre,
                PrecioUnitario = CabeceraProductoPrecio,
                Cantidad = CabeceraProductoCantidad,
            });

            Refrescar();
        }

        public ordencompra ToModel()
        {
            var comprobante = new ordencompra();
            comprobante.NombreProyecto = this.Cliente;
            comprobante.Fecha = DateTime.Now;
            comprobante.Total = this.Total();

            foreach (var d in ComprobanteDetalle)
            {
                comprobante.detalleordenproductos.Add(new detalleordenproductos
                {
                    Productos_Id = d.ProductoId,
                    monto = d.Monto(),
                    preciounitario = d.PrecioUnitario,
                    cantidad = d.Cantidad
                });
            }

            return comprobante;
        }

    }
}