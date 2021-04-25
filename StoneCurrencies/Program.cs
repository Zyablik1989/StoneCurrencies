using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StoneCurrencies.APIManagement;
using StoneCurrencies.DBManagement;

namespace StoneCurrencies
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine(
@"[D]-Закачать валюты и курсы валют в базу
[C]-Выбрать валюту и дату, чтобы увидеть курс на этот день.
[Любая клавиша]-Показать курс валют (USD) за сегодняшнюю дату"
                );
                var key = Console.ReadKey(true);

                //Получаем символ, который ввёл пользователь
                var keyPressed = key.Key.ToString().ToUpper();
                switch (keyPressed)
                {
                    case "D":
                        //Начинаем загрузку данных с Api в Базу данных
                        UploadCurrenciesRatesFromApiToDb();
                        break;
                    case "C":
                        //Начинаем ввод условий для демонстрации конкретного курса
                        ShowSpecifiedCurrencyRate();
                        break;
                    default:
                        //Начинаем демонстрацию сегодняшнего курса с валютой, с которой мы якобы работаем.
                        ShowCurrencyRates();
                        break;
                }
            }
        }

        /// <summary>
        /// Получаем от дату и валюту от пользователя.
        /// </summary>
        private static void ShowSpecifiedCurrencyRate()
        {
            var date = DateTime.Now;
            var input = string.Empty;
                //Добиваемся от пользователя корректной даты
            do
            {
                Console.WriteLine("Введите дату (например в формате: 24.11.2001), пустое значение - сегодняшняя дата");
                input = Console.ReadLine();
            } while (!string.IsNullOrEmpty(input) && !DateTime.TryParse(input, out date));

            //Добиваемся от пользователя корректной валюты (3 заглавных латинских буквы)
            do
            {
                Console.WriteLine("Введите валюту в коротком формате (Например USD или EUR)");
                input = Console.ReadLine();
            } while (input == null || !new Regex(@"^[A-Z]{3}$").IsMatch(input));
            
            //Отображаем курс на основе введённых данных
            ShowCurrencyRates(date, input);
        }

        /// <summary>
        /// Показать курс валюты
        /// </summary>
        /// <param name="date"></param>
        /// <param name="charCode"></param>
        private static void ShowCurrencyRates(DateTime date = default, string charCode = "USD")
        {
            //Если дата не указана, то ставим сегодняшнюю
            if (date == default) date = DateTime.Now;

            var Db = new DBManager();
            var currencies = Db.DownloadCurrenciesFromDBViaLinqToDB();
            var currencyRates = Db.DownloadCurrencyRatesFromDBViaLinqToDB(date);

            //Объединяем валюты и их курсы в единое множество
            var currencyInfos = currencies.Join(currencyRates, c => c.NumCode, cr => cr.NumCode,
                (c, cr) => new {c.Name, c.CharCode, c.NumCode, cr.Rate});

            var result = new StringBuilder()
                .AppendLine($"На дату {date.ToString("dd MMMM yyyy")} из базы получены следующие курсы валют:");

                //Если хоть что-то по курсу валют нашлось, то выводим на экран
            foreach (var info in currencyInfos.Where(_ =>
                _.CharCode.ToUpper() == charCode))
                result.AppendLine($"— 1 {info.CharCode}({info.Name}) стоит {info.Rate}р.");

            Console.WriteLine(result.ToString());
        }

        /// <summary>
        /// Загружаем данные из API в БД
        /// </summary>
        private static void UploadCurrenciesRatesFromApiToDb()
        {
            var date = DateTime.Now;
            var input = string.Empty;
            //Добиваемся от пользователя корректной даты
            do
            {
                Console.WriteLine("Введите дату (например в формате: 24.11.2001), пустое значение - сегодняшняя дата");
                input = Console.ReadLine();
            } while (!string.IsNullOrEmpty(input) && !DateTime.TryParse(input, out date));

            var Api = new ApiManager();
            var Db = new DBManager();
            //Валют
            Db.UploadCurrenciesIntoDBViaLinqToDB(Api.RetrieveCurrencies());
            //Курсы валют
            Db.UploadCurrencyRatesIntoDBViaLinqToDB(Api.RetrieveCurrencyRates(date), date);
        }
    }
}