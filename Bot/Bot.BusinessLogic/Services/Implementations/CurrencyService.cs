using System;
using System.Text;
using Bot.BusinessLogic.Services.Interfaces;
using HtmlAgilityPack;

namespace Bot.BusinessLogic.Services.Implementations
{
	public class CurrencyService: ICurrencyService
    {
        public decimal Get(string type)
        {
            var currency = Currency();

            if(type == "$")
                return currency[0];
            return currency[1];
        }
        
        private List<decimal> Currency()
        {
            List<decimal> currency = new List<decimal>();
            HtmlWeb web = new HtmlWeb();
            web.OverrideEncoding = Encoding.UTF8;
            HtmlDocument document = web.Load("https://myfin.by/currency/minsk");
            try
            {
                int number = 0;
                foreach (HtmlNode volume in document.DocumentNode.SelectNodes("//tr[contains(@class, 'tr-tb acc-link_11 not_h')]//td"))
                {

                    if (number == 2)
                        currency.Add(Convert.ToDecimal(volume.InnerText));
                    if (number == 4)
                    {
                        currency.Add(Convert.ToDecimal(volume.InnerText));
                        break;
                    }
                    number++;
                }
                return currency;
            }
            catch (Exception ex) { Console.WriteLine("Ошибка " + ex); return null; }
        }
	}
}

