using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WebStore.CfpLibraryHost
{
	class Program
	{
		private static CosmosClient Client { get; set; }

		static void Main(string[] args)
		{
			Task.Run(async () =>
			{
				Client = new CosmosClient("AccountEndpoint=https://cdb-sql.documents.azure.com:443/;AccountKey=ESHck0v4H2CscTGpDZB4PFbzrUhIY21cw7qk3kc05QRI7AlFSiU0fHLENKtopuRVyewchDafQeFEzoBV5gvVNA==;");

				var database = Client.GetDatabase("acme-webstore");
				await database.CreateContainerIfNotExistsAsync("lease", "/id");
				var monitoredContainer = database.GetContainer("cart");
				var leaseContainer = database.GetContainer("lease");

				var cfp = monitoredContainer.GetChangeFeedProcessorBuilder<dynamic>("ChangeFeedDemoProcessor", ProcessChangesAsync)
					.WithInstanceName("Change Feed Processor Library demo")
					.WithStartTime(DateTime.MinValue.ToUniversalTime())
					.WithLeaseContainer(leaseContainer)
					.Build();

				await cfp.StartAsync();
				Console.WriteLine("Started change feed processor - press any key to stop");
				Console.ReadKey(true);
				await cfp.StopAsync();
			}).Wait();
		}

		static async Task ProcessChangesAsync(IReadOnlyCollection<dynamic> documents, CancellationToken cancellationToken)
		{
			foreach (var document in documents)
			{
				Console.WriteLine($"Document {document.id} has changed!");
				Console.WriteLine(document.ToString());
				// ...do what you need with each change... the sky's the limit!
			}
		}

	}
}
