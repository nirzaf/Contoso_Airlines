using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebStore.ChangeFeedFunctions
{
	class DenormalizeProductFunction
	{
		private static readonly CosmosClient _client;

		static DenormalizeProductFunction()
		{
			var connStr = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
			_client = new CosmosClient(connStr);
		}

		[FunctionName("DenormalizeProduct")]
		public static async Task DenormalizeProduct(
			[CosmosDBTrigger(
				databaseName: "acme-webstore",
				collectionName: "productMeta",
				ConnectionStringSetting = "CosmosDbConnectionString",
				LeaseCollectionName = "lease",
				LeaseCollectionPrefix = "DenormalizeProduct"
			)]
			IReadOnlyList<Document> documents,
			ILogger logger)
		{
			foreach (var document in documents)
			{
				try
				{
					await ProcessChange(document, logger);
				}
				catch (Exception ex)
				{
					logger.LogError($"Error processing change for document {document.Id}: {ex.Message}");
				}
			}
		}

		private static async Task ProcessChange(Document document, ILogger logger)
		{
			var item = JsonConvert.DeserializeObject<dynamic>(document.ToString());
			string type = item.type;

			switch (type)
			{
				case "category":
					string categoryId = item.id;
					string categoryName = item.name;
					await UpdateProductCategoryName(categoryId, categoryName, logger);
					break;

				case "tag":
					string tagId = item.id;
					string tagName = item.name;
					await UpdateProductTagName(tagId, tagName, logger);
					break;
			}
		}

		private static async Task UpdateProductCategoryName(string categoryId, string categoryName, ILogger logger)
		{
			var sql = $"SELECT * FROM c WHERE c.categoryId = '{categoryId}'";
			var productContainer = _client.GetContainer("acme-webstore", "product");
			var options = new QueryRequestOptions { PartitionKey = new Microsoft.Azure.Cosmos.PartitionKey(categoryId) };
			var iterator = productContainer.GetItemQueryIterator<dynamic>(sql, requestOptions: options);
			var ctr = 0;
			while (iterator.HasMoreResults)
			{
				var page = await iterator.ReadNextAsync();
				foreach (var productDocument in page)
				{
					logger.LogWarning($" Updating product ID {productDocument.id} with new category name '{categoryName}'");
					productDocument.categoryName = categoryName;
					await productContainer.ReplaceItemAsync(productDocument, productDocument.id.ToString());
					ctr++;
				}
			}

			logger.LogWarning($"Propagated new category name '{categoryName}' (id '{categoryId}') to {ctr} product document(s)");
		}

		private static async Task UpdateProductTagName(string tagId, string tagName, ILogger logger)
		{
			var sql = $"SELECT * FROM c WHERE ARRAY_CONTAINS(c.tags, {{'id': '{tagId}'}}, true)";
			var productContainer = _client.GetContainer("acme-webstore", "product");
			var options = new QueryRequestOptions { MaxConcurrency = -1 };
			var iterator = productContainer.GetItemQueryIterator<dynamic>(sql, requestOptions: options);
			var ctr = 0;
			while (iterator.HasMoreResults)
			{
				var page = await iterator.ReadNextAsync();
				foreach (var productDocument in page)
				{
					logger.LogWarning($" Updating product ID {productDocument.id} with new tag name '{tagName}'");
					var tag = ((IEnumerable<dynamic>)productDocument.tags).First(t => t.id == tagId);
					tag.name = tagName;
					await productContainer.ReplaceItemAsync(productDocument, productDocument.id.ToString());
					ctr++;
				}
			}

			logger.LogWarning($"Propagated new tag name '{tagName}' (id '{tagId}') to {ctr} product document(s)");
		}

	}
}
