using Newtonsoft.Json;

namespace Services
{
    public class ExchangeRatesWebService
    {
        //Fields to hold standard URL parts
        private const string baseUrl = "https://openexchangerates.org/api/";
        private const string apiKey = "5a4f0e3c427046f99109ca62e66ff65b";

        /// <summary>
        /// Gets the current value of DKK in relation to the USD
        /// </summary>
        /// <returns>A Double with the DKK/USD value</returns>
        public double GetUSDInDKK()
        {
            //Build the url
            string url = $"{baseUrl}latest.json?app_id={apiKey}&symbols=DKK";

            //The variable to hold the HTTP (text, that is) response from the server
            string json;

            //Make a client object that will call the server
            using (HttpClient client = new())
            {
                //Call the server and save the response in the json variable
                json = client.GetStringAsync(url).Result;
            }

            //Convert the json to C# object
            RootCurrency root = JsonConvert.DeserializeObject<RootCurrency>(json);

            //Get the exchange rate, by "dotting" in the Root object
            return root.rates.DKK;
        }
    }
    public class Rates
    {
        public double DKK { get; set; }
    }

    public class RootCurrency
    {
        public string disclaimer { get; set; }
        public string license { get; set; }
        public int timestamp { get; set; }
        public string @base { get; set; }
        public Rates rates { get; set; }
    }

}