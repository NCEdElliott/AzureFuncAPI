using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using MxxRealFunctionApp.Utilities;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using MxxRealFunctionApp.Repositories;
using MxxRealFunctionApp.Models;

namespace MxxRealFunctionApp
{
    public static class GetStockFunction
    {
		[FunctionName("GetStockFunction")]
		public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, 
														   "get",  
														   Route = "GetStock/symbol/{symbol}")]HttpRequestMessage req, string symbol, TraceWriter log)
        {
			DocumentDBRepository<Quote>.Initialize();

			log.Info($"C# HTTP trigger function processed a stock request for {symbol}.");

			string url = $"https://api.iextrading.com/1.0/stock/{symbol}/quote";

			HttpClient client = new HttpClient();

			string jsonResult = JsonFormatter.JsonPrettify(await client.GetStringAsync(url));

			var quote = Quote.FromJson(jsonResult);

			if (quote != null)
			{
				var foundQuotes = await DocumentDBRepository<Quote>.GetItemsAsync(x => x.CompanyName == quote.CompanyName &&
																					  x.Close == quote.Close &&
																					  x.CloseTime == quote.CloseTime);

				if (foundQuotes == null ||
					foundQuotes.Count() == 0)
				{
					var createdDoc = await DocumentDBRepository<Quote>.CreateItemAsync(quote);
				} 
			}

			if (string.IsNullOrWhiteSpace(jsonResult))
			{
				return new HttpResponseMessage(HttpStatusCode.NotFound)
				{
					Content = new StringContent("{\"Error\": \"Stock not found\"}", Encoding.UTF8, "application/json")
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
