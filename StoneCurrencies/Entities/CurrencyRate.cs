using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneCurrencies.Entities
{
    /// <summary>
    /// Курс валюты
    /// </summary>
    internal class CurrencyRate
    {
        public int? NumCode { get; set; }
        public decimal? Rate { get; set; }
    }
}
