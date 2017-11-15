using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using MxxRealFunctionApp.Utilities;
using System.Text;

namespace MxxRealFunctionApp
{
    public static class GetExchangeRateFunction
    {
        [FunctionName("GetExchangeRateFunction")]
		public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function,
														   "get",
														   Route = "GetExchangeRate/date/{date}/base/{baseCurrency}")]HttpRequestMessage req, string date, string baseCurrency, TraceWriter log)
		{
			log.Info($"C# HTTP trigger function processed an Exchange Rate request for {date} / {baseCurrency}.");

			string url = $"https://api.fixer.io/{date}?base={baseCurrency}";

			HttpClient client = new HttpClient();

			string jsonResult = JsonFormatter.JsonPrettify(await client.GetStringAsync(url));

			if (string.IsNullOrWhiteSpace(jsonResult))
			{
				return new HttpResponseMessage(HttpStatusCode.NotFound)
				{
					Content = new StringContent("{\"Error\": \"Exchange Rates not found\"}", Encoding.UTF8, "application/json")
				};
			}
			else
			{
				return new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent(jsonResult, Encoding.UTF8, "application/json")
				};
			}
		}
	}
}
