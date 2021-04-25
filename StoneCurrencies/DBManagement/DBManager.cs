using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoneCurrencies.Model;

namespace StoneCurrencies.DBManagement
{
    internal class DBManager
    {
        /// <summary>
        /// На основании списка валют из API загружаем в базу те валюты, которых там нет.
        /// </summary>
        /// <param name="Currencies"></param>
        internal void UploadCurrenciesIntoDBViaLinqToDB(List<Entities.Currency> Currencies)
        {
            using (var db = new Model.CurrenciesDBDataContext(Properties.Settings.Default.StoneCurrenciesConnectionString))
            {
                var Changes = 0;

                foreach (var currency in Currencies)
                {
                    var dbCurrency = new Model.Currency
                    {
                        Name = currency.Name,
                        EngName = currency.EngName,
                        NumCode = currency.NumCode,
                        CharCode = currency.CharCode
                    };
                    if(!db.Currencies.Any(_=>_.CharCode==dbCurrency.CharCode && _.NumCode==dbCurrency.NumCode))
                        db.Currencies.InsertOnSubmit(dbCurrency);
                }

                Changes = db.GetChangeSet().Deletes.Count + db.GetChangeSet().Inserts.Count + db.GetChangeSet().Updates.Count;
                if (Changes > 0)
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                Console.WriteLine($"В базу загружены новые валюты в количестве {Changes}");
            };
        }

        /// <summary>
        /// На основании списка курсов валют из API загружаем в базу те курсы валют, которых там нет.
        /// </summary>
        /// <param name="currencyRates"></param>
        internal void UploadCurrencyRatesIntoDBViaLinqToDB(List<Entities.CurrencyRate> currencyRates, DateTime date)
        {
            using (var db = new Model.CurrenciesDBDataContext(Properties.Settings.Default.StoneCurrenciesConnectionString))
            {
                var Changes = 0;
                //Отфильтровываем курсы тех валют, которых нет в справочнике валют
                var RatesWithRegisteredCurrencies = currencyRates
                    .Where(_ =>
                        db.Currencies.Select(c => c.NumCode)
                            .Contains(_.NumCode));

                //Курсы валют не могут с пустыми полезными полями, отфильтровываем.
                var RatesWithoutNullValues = RatesWithRegisteredCurrencies
                    .Where(_ => !(_.NumCode == null || _.Rate == null));

                foreach (var rate in RatesWithoutNullValues)
                {
                    var dbRate = new Model.CurrencyRate
                    {
                        DateTime = date,
                        NumCode = rate.NumCode ?? 0,
                        Rate = rate.Rate ?? 0
                    };

                    if (!db.CurrencyRates.Any(_ => _.NumCode == dbRate.NumCode && _.DateTime == dbRate.DateTime))
                        db.CurrencyRates.InsertOnSubmit(dbRate);
                }

                Changes = db.GetChangeSet().Deletes.Count + db.GetChangeSet().Inserts.Count + db.GetChangeSet().Updates.Count;
                if (Changes > 0)
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                Console.WriteLine($"В базу загружены новые курсы валют на дату {date.ToString("dd.MM.yyyy")} в количестве {Changes}");
            };

        }

        /// <summary>
        /// Получить из базы перечень Валют
        /// </summary>
        /// <returns></returns>
        internal List<Entities.Currency> DownloadCurrenciesFromDBViaLinqToDB()
        {
            using (var db = new Model.CurrenciesDBDataContext(Properties.Settings.Default.StoneCurrenciesConnectionString))
            {
                return db.Currencies.Select(_ => new Entities.Currency()
                {
                    Name = _.Name,
                    EngName = _.EngName,
                    CharCode = _.CharCode,
                    NumCode = _.NumCode
                }).ToList();
            };
        }

        /// <summary>
        /// Получить из базы перечень курсов валют
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        internal List<Entities.CurrencyRate> DownloadCurrencyRatesFromDBViaLinqToDB(DateTime date)
        {
            using (var db = new Model.CurrenciesDBDataContext(Properties.Settings.Default.StoneCurrenciesConnectionString))
            {
                return db.CurrencyRates
                    .Where(_ => _.DateTime.Date == date.Date)
                    .Select(_ => new Entities.CurrencyRate()
                {
                     NumCode = _.NumCode,
                     Rate = _.Rate
                }).ToList();
            };
        }
    }
}
