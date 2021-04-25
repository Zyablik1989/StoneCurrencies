using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneCurrencies.Entities
{
    /// <summary>
    /// Валюта
    /// </summary>
    internal class Currency
    {
        public string Name { get; set; }
        public string EngName { get; set; }
        public string CharCode { get; set; }
        public int? NumCode { get; set; }
    }
}
