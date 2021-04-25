using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using StoneCurrencies.Entities;

namespace StoneCurrencies.APIManagement
{
    internal class ApiManager
    {
        //Получить валюты из API
        internal List<Currency> RetrieveCurrencies()
        {
            //Справочник валют, если что-то пойдёт не так, вернём пустым
            var RetrievedCurrencies = new List<Currency>();

            var tempInt = 0;
            decimal tempDec = 0;
            try
            {
                //Получаем XML
                var xdoc = XDocument.Load("http://www.cbr.ru/scripts/XML_valFull.asp");
                
                //На основе элементов создаём перечень валют
                var currencies = xdoc.Descendants("Item")
                    .Select(_ => new Currency
                    {
                        Name = _.Element("Name")?.Value,
                        CharCode = _.Element("ISO_Char_Code")?.Value,
                        EngName = _.Element("EngName")?.Value,
                        NumCode = int.TryParse(_.Element("ISO_Num_Code")?.Value, out tempInt) ? tempInt : 0
                    })
                    .ToList();

                if (currencies.Count > 0)
                    RetrievedCurrencies = currencies;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            Console.WriteLine($"С сайта ЦБ получены валюты в количестве {RetrievedCurrencies.Count}");
            return RetrievedCurrencies;
        }

        internal List<CurrencyRate> RetrieveCurrencyRates(DateTime date)
        {
            //Справочник курсов валют, если что-то пойдёт не так, вернём пустым
            var RetrievedCurrencyRates = new List<CurrencyRate>();

            var tempInt = 0;
            decimal tempDec = 0;
            try
            {
                var dateString = date.ToString("dd.MM.yyyy");
                //Получаем XML
                var xdoc = XDocument.Load("http://www.cbr.ru/scripts/XML_daily.asp?date_req="+dateString );

                //На основе элементов создаём перечень курсов валют
                var currencyRates = xdoc.Descendants("Valute")
                    .Select(_ => new CurrencyRate
                    {
                        NumCode = int.TryParse(_.Element("NumCode")?.Value, out tempInt) ? tempInt : 0,
                        Rate =
                            decimal.TryParse(_.Element("Value")?.Value, out tempDec)
                                ? (decimal?) (tempDec / (int.TryParse(_.Element("Nominal")?.Value, out tempInt)
                                                  ? tempInt
                                                  : 1))
                                : null
                    })
                    .ToList();
                if (currencyRates.Count > 0)
                    RetrievedCurrencyRates = currencyRates;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            Console.WriteLine($"С сайта ЦБ получены курсы валют в количестве {RetrievedCurrencyRates.Count}");
            return RetrievedCurrencyRates;
        }
    }
}