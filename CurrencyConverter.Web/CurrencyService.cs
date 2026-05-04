using System.Globalization;

namespace CurrencyConverter.Web
{
    using System.Xml.Linq;

    public class CurrencyService : ICurrencyService
    {
        private const string ECB_URL = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
        private const string VALID_KEY = "secret123";

        public double Convert(string fromCurrency, string toCurrency, double amount, string apiKey)
        {
            if (apiKey != VALID_KEY)
            {
                throw new Exception("Authentication failed");
            }

            var rates = LoadRates();

            double fromRate = fromCurrency == "EUR" ? 1 : rates[fromCurrency];
            double toRate = toCurrency == "EUR" ? 1 : rates[toCurrency];

            double eurAmount = amount / fromRate;
            
            return eurAmount * toRate;
        }

        public List<string> GetSupportedCurrencies()
        {
            var rates = LoadRates();
            var supportedCurrencies = rates.Keys.ToList();
            supportedCurrencies.Add("EUR");
            supportedCurrencies.Sort();
            return supportedCurrencies;
        }

        private Dictionary<string, double> LoadRates()
        {
            var xml = XDocument.Load(ECB_URL);

            var rates = xml.Descendants()
                .Where(x => x.Name.LocalName == "Cube" && x.Attribute("currency") != null)
                .ToDictionary(
                    x => x.Attribute("currency").Value,
                    x => double.Parse(x.Attribute("rate").Value, CultureInfo.InvariantCulture)
                );

            return rates;
        }
    }
}
