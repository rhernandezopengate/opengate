using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class Group<K, T>
    {
        public K Key;
        public IEnumerable<T> Values;
        public decimal? PalletsEmbarcadosTemp;
        public int? CajasEmbarcadasTemp;
        public int? PiezasEmbarcadasTemp;
        public decimal? CapacidadPalletsUnidadTemp;
        public decimal? PesoInventarioTemp;
        public decimal? CapasidadUnidadTemp;
        public decimal? TotalPagado;
        public decimal? FacturacionFlete;
        public decimal? FacturacionManiobras;
        public string Mes;
        public string EvaluacionPosicionamiento;
        public int? TotalRecords;
        public int? TotalOks;
        public int? TotalDesv;

    }
}