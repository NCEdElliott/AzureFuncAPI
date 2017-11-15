using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using static System.Net.WebRequestMethods;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using MxxRealFunctionApp.Utilities;
using System.Threading.Tasks;

namespace MxxRealFunctionApp
{
    public static class GetWeatherFunction
    {
        [FunctionName("GetWeatherFunction")]
		public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, 
																	   "get", "post", 
																	   Route = "GetWeather/CityState/{cityState}")]HttpRequestMessage req, string cityState, TraceWriter log)
        {
            log.Info($"C# HTTP trigger function processed a weather request for {cityState}.");

			string query = $"select * from weather.forecast where woeid in (select woeid from geo.places(1) where text=\"{cityState}\")";
			
			string url = "https://query.yahooapis.com/v1/public/yql?q=" + query + "&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";

			HttpClient client = new HttpClient();

			string jsonResult = JsonFormatter.JsonPrettify(await client.GetStringAsync(url));
			
			// Fetching the name from the path parameter in the request URL
			return new HttpResponseMessage(HttpStatusCode.OK)
				   {
						Content = new StringContent(jsonResult, Encoding.UTF8, "application/json")
				   };
		}
    }
}
