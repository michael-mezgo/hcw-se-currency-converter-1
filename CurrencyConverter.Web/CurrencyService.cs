using System.Globalization;

namespace CurrencyConverter.Web
{
    using System.Xml.Linq;

    public class CurrencyService : ICurrencyService
    {
        private readonly string _ecbUrl;
        private readonly string _validKey;

        public CurrencyService()
        {
            _ecbUrl = Environment.GetEnvironmentVariable("ECB_URL")
                ?? throw new InvalidOperationException("ECB_URL is not set");
            _validKey = Environment.GetEnvironmentVariable("VALID_KEY")
                ?? throw new InvalidOperationException("VALID_KEY is not set");
        }

        public double Convert(string fromCurrency, string toCurrency, double amount, string apiKey)
        {
            if (apiKey != _validKey)
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
            var xml = XDocument.Load(_ecbUrl);

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
