using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenGate.Entidades
{
    public class GroupDespacho<K, T>
    {
        public K Key;
        public IEnumerable<T> Values;
        public decimal? QTYPallets;
        public int? SumaDias;
        public int? wkasd;
    }
}