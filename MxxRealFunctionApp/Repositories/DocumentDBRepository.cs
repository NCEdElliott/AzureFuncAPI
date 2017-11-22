using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace MxxRealFunctionApp.Repositories
{
	public static class DocumentDBRepository<T> where T : class
	{
		private static readonly string _databaseId = "ToDoList";
		private static readonly string _collectionId = "Quotes";
		private static readonly string _authKey = "NEXW8ofWLsCQLoxen9FVGqLtJChxjV4XHiNiovS7jb6k1nrFgTHQFsDCWLDSlQTPjMQHUWnqpT0wWvHzK7ocBg==";
		private static readonly string _endpoint = "https://mxxreal.documents.azure.com:443/";
		private static readonly string _graphEndpoint = "https://mxxreal-graph.documents.azure.com:443/";

		private static DocumentClient client;

		public static async Task<T> GetItemAsync(string id)
		{
			try
			{
				Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
				return (T)(dynamic)document;
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					return null;
				}
				else
				{
					throw;
				}
			}
		}

		public static async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
		{
			IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
				UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
				new FeedOptions { MaxItemCount = -1 })
				.Where(predicate)
				.AsDocumentQuery();

			List<T> results = new List<T>();
			while (query.HasMoreResults)
			{
				results.AddRange(await query.ExecuteNextAsync<T>());
			}

			return results;
		}

		public static async Task<Document> CreateItemAsync(T item)
		{
			return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), item);
		}

		public static async Task<Document> UpdateItemAsync(string id, T item)
		{
			return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id), item);
		}

		public static async Task DeleteItemAsync(string id)
		{
			await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
		}

		public static void Initialize()
		{
			client = new DocumentClient(new Uri(_endpoint), _authKey, new ConnectionPolicy { EnableEndpointDiscovery = false });
			CreateDatabaseIfNotExistsAsync().Wait();
			CreateCollectionIfNotExistsAsync().Wait();
		}

		private static async Task CreateDatabaseIfNotExistsAsync()
		{
			try
			{
				await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseId));
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					await client.CreateDatabaseAsync(new Database { Id = _databaseId });
				}
				else
				{
					throw;
				}
			}
		}

		private static async Task CreateCollectionIfNotExistsAsync()
		{
			try
			{
				await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId));
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					await client.CreateDocumentCollectionAsync(
						UriFactory.CreateDatabaseUri(_databaseId),
						new DocumentCollection { Id = _collectionId },
						new RequestOptions { OfferThroughput = 1000 });
				}
				else
				{
					throw;
				}
			}
		}
	}
}