using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class GourpMeses<K, T>
    {
        public K Key;
        public IEnumerable<T> Values;
        public int? TotalRecords;
        public int? TotalOks;
        public int? TotalDesv;
        public double? porcentaje;        
    }
}