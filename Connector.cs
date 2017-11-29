using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RandomNumberApp
{
    class Connector
    {
        private const string J_API_KEY = "9fd9da59-8553-4b86-9c95-746dcaf2bd22";
        private const int J_N = 0;
        private const int J_DecimalPlaces = 20;
        private const int J_ID = 0;
        private const string J_JRPC_VERSION = "2.0";
        private const string J_METHOD = "generateDecimalFractions";

        private readonly Uri _baseUrl = new Uri(@"https://api.random.org");
        //private readonly Uri RelativeUrl = new Uri("https://api.random.org/json-rpc/1/invoke"); //TODO uncomment this
        private readonly Uri _relativeUrl = new Uri("http://torisels.w.staszic.waw.pl/~torisels/apitest/"); //DEV

        private const string mediaType = "application/json";

        private HttpClient _client;
        private HttpRequestMessage _request;

      

        private decimal[] Decimals { get; set; }

        public decimal[] GetDecimals()
        {
            return Decimals ?? null;
        }

        public Connector()
        {
            SetUpConnection();
        }

        private void SetUpConnection()
        {
            _client = new HttpClient {BaseAddress = _baseUrl};
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            _request = new HttpRequestMessage(HttpMethod.Post, _relativeUrl)
            {
                Content = new StringContent(createRequestJsonString(), Encoding.UTF8, mediaType)
            };
        }

        public async Task SendRequest()
        {
            var response = await _client.SendAsync(_request);
            var content = await response.Content.ReadAsStringAsync();
            Decimals = ParseDataToDecimals(content);
        }

        private string createRequestJsonString()
        {
            JObject nesOb = new JObject(
                new JProperty("apiKey", J_API_KEY),
                new JProperty("n", J_N),
                new JProperty("decimalPlaces", J_DecimalPlaces),
                new JProperty("replacement", false));

            JObject o = new JObject(
                new JProperty("jsonrpc",J_JRPC_VERSION),
                new JProperty("method",J_METHOD),
                new JProperty("params", nesOb),
                new JProperty("id", J_ID)
                );

            return JsonConvert.SerializeObject(o);
        }
        /**
         * This method parses the JSON data to List of Decimals.
         * 
         * **/
        private decimal[] ParseDataToDecimals(string jsonString)
        {
            dynamic json =  JsonConvert.DeserializeObject(jsonString);

            var decimals = new List<decimal>();

            foreach (string s in json.result.random.data)
            {
                decimals.Add(decimal.Parse(s));
            }
            return decimals.ToArray();
        }
    }
}
