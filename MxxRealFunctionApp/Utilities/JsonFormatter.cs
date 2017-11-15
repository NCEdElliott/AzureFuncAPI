using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MxxRealFunctionApp.Utilities
{
	public class JsonFormatter
	{
		public static string JsonPrettify(string json)
		{
			using (var stringReader = new StringReader(json))
			using (var stringWriter = new StringWriter())
			{
				var jsonReader = new JsonTextReader(stringReader);
				var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };

				jsonWriter.WriteToken(jsonReader);

				return stringWriter.ToString();
			}
		}
	}
}
