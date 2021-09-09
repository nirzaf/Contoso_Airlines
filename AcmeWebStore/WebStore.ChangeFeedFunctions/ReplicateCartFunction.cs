using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebStore.ChangeFeedFunctions
{
	class ReplicateCartFunction
	{
		private static readonly CosmosClient _client;

		static ReplicateCartFunction()
		{
			var connStr = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
			_client = new CosmosClient(connStr);
		}

		[FunctionName("ReplicateCart")]
		public static async Task ReplicateCart(
			[CosmosDBTrigger(
				databaseName: "acme-webstore",
				collectionName: "cart",
				ConnectionStringSetting = "CosmosDbConnectionString",
				LeaseCollectionName = "lease",
				LeaseCollectionPrefix = "ReplicateCart"
			)]
			IReadOnlyList<Document> documents,
			ILogger logger)
		{
			var container = _client.GetContainer("acme-webstore", "cartByItem");
			foreach (var document in documents)
			{
				try
				{
					if (document.TimeToLive == null)
					{
						await container.UpsertItemAsync(document);
						logger.LogWarning($"Upserted document id {document.Id} in replica container");
					}
					else
					{
						var item = document.GetPropertyValue<string>("item");
						await container.DeleteItemAsync<Document>(document.Id, new Microsoft.Azure.Cosmos.PartitionKey(item));
						logger.LogWarning($"Deleted document id {document.Id} in replica container");
					}
				}
				catch (Exception ex)
				{
					logger.LogError($"Error processing change for document id {document.Id}: {ex.Message}");
				}
			}
		}

	}
}
