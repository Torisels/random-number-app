using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
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

        private readonly Uri BaseUrl = new Uri(@"https://api.random.org");
        //private readonly Uri RelativeUrl = new Uri("https://api.random.org/json-rpc/1/invoke"); //TODO uncomment this
        private readonly Uri RelativeUrl = new Uri("http://torisels.w.staszic.waw.pl/~torisels/apitest/"); //DEV

        private const string mediaType = "application/json";

        private HttpClient client;
        private HttpRequestMessage request;

      

        private decimal[] Decimals { get; set; }

        public decimal[] GetDecimals()
        {
            return Decimals ?? null;
        }

        private const string devtest = @"{
              ""jsonrpc"": ""2.0"",
              ""result"": {
                ""random"": {
                  ""data"": [
                    0.528603664155,
                    0.10317942408449,
                    0.9692723586187,
                    0.9856829677046,
                    0.14391010617136,
                    0.28573503037159,
                    0.64049629998182,
                    0.04481570717269,
                    0.63144861445594,
                    0.4119882205557,
                    0.76425400681331,
                    0.5425395852179,
                    0.6770498138576,
                    0.92375606406131,
                    0.89409554338424,
                    0.30871961303879,
                    0.81877002777878,
                    0.066600326656812,
                    0.19553387620782,
                    0.6400705932094
                  ],
                  ""completionTime"": ""2017-11-16 20:44:05Z""
                },
                ""bitsUsed"": 1329,
                ""bitsLeft"": 248671,
                ""requestsLeft"": 999,
                ""advisoryDelay"": 0
              },
              ""id"": ""0""
            }";


        public Connector()
        {
            setUpConnection();
            //sendRequest();
        }

        private void setUpConnection()
        {
            client = new HttpClient {BaseAddress = BaseUrl};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            request = new HttpRequestMessage(HttpMethod.Post, RelativeUrl)
            {
                Content = new StringContent(createRequestJsonString(), Encoding.UTF8, mediaType)
            };
        }

        public async Task sendRequest()
        {
            await Task.Delay(1500);
            var response = await client.SendAsync(request);
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

        private decimal[] ParseDataToDecimals(string jsonString = devtest)
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
