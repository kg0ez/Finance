using System;
using System.Globalization;
using System.Text;
using Bot.BusinessLogic.Services.Interfaces;
using HtmlAgilityPack;

namespace Bot.BusinessLogic.Services.Implementations
{
    public class CurrencyService : ICurrencyService
    {
        public decimal Get(string type)
        {
            var currency = Currency();
            if (type == "$")
                return currency[0];
            return currency[1];
        }

        private List<decimal> Currency()
        {
            List<decimal> currency = new List<decimal>();
            HtmlWeb web = new HtmlWeb();
            web.OverrideEncoding = Encoding.UTF8;
            HtmlDocument document = web.Load("https://myfin.by/currency/minsk");
            decimal dollar = default;
            decimal euro = default;
            try
            {
                var dollarNodeText = document.DocumentNode
                    .SelectNodes(".//div[@class='c-best-rates']//table/tbody")[0].ChildNodes[0].ChildNodes[3].InnerText;
                var euroNodeText = document.DocumentNode
                .SelectNodes(".//div[@class='c-best-rates']//table/tbody")[0].ChildNodes[1].ChildNodes[3].InnerText;

                var IsDollarParsed = decimal.TryParse(dollarNodeText,NumberStyles.AllowDecimalPoint, new NumberFormatInfo { NumberDecimalSeparator = "."}, out dollar) ;
                var IsEuroParsed = decimal.TryParse(euroNodeText,NumberStyles.AllowDecimalPoint, new NumberFormatInfo { NumberDecimalSeparator = "."}, out euro) ;
                if (IsDollarParsed && IsEuroParsed)
                {
                    currency.Add(dollar);
                    currency.Add(euro);
                    return currency;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex) { Console.WriteLine("Ошибка " + ex); return null; }
        }
    }
}

