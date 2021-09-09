using FlightTelemetry.Shared;
using FlightTelemetry.Shared.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlightTelemetry.Microservices.DataArchival
{
	public static class DataArchivalFunction
	{
		private static readonly Dictionary<string, DateTime> _timestamps =
			new Dictionary<string, DateTime>();

		private static readonly object _threadLock =
			new object();

		private static readonly CloudBlobContainer _blobContainer;

		static DataArchivalFunction()
        {
			var storageAccount = new CloudStorageAccount(
				new StorageCredentials(
					Environment.GetEnvironmentVariable("StorageAccountName"),
					Environment.GetEnvironmentVariable("StorageAccountKey")),
				useHttps: true);

			var blobClient = storageAccount.CreateCloudBlobClient();

			_blobContainer = blobClient.GetContainerReference(Environment.GetEnvironmentVariable("BlobContainerName"));
		}

		[FunctionName("DataArchival")]
		public static async Task DataArchival(
			[CosmosDBTrigger(
				databaseName: Constants.DatabaseName,
				collectionName: Constants.LocationContainerName,
				ConnectionStringSetting = "CosmosDbConnectionString",
				LeaseCollectionName = "lease",
				LeaseCollectionPrefix = "DataArchival-"
			)]
			IReadOnlyList<Document> documents,
			ILogger logger)
		{
			foreach (var document in documents)
			{
				try
				{
					await ArchiveFlightLocation(document, logger);
				}
				catch (Exception ex)
				{
					logger.LogError($"Error processing document id {document.Id}: {ex.Message}");
				}
			}
		}

		private static async Task ArchiveFlightLocation(Document document, ILogger logger)
		{
			var locationEvent = JsonConvert.DeserializeObject<LocationEvent>(document.ToString());
			if (ShouldSkip(locationEvent))
			{
				return;
			}

			var blobName = $"{DateTime.UtcNow:yyyyMMdd-HHmmss}-{locationEvent.FlightNumber}-{locationEvent.Id}.json";
			var blob = _blobContainer.GetBlockBlobReference(blobName);
			var bytes = Encoding.ASCII.GetBytes(document.ToString());
			await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

			logger.LogWarning($"Archived '{blobName}' to blob storage");
		}

		private static bool ShouldSkip(LocationEvent locationEvent)
		{
			// Make sure not to miss the last archive for completed flights
			if (locationEvent.IsComplete)
			{
				return false;
			}

			// Throttle continuous processing by delaying between archivals of the same flight to blob storage
			lock (_threadLock)
			{
				if (_timestamps.ContainsKey(locationEvent.FlightNumber))
				{
					if (DateTime.Now.Subtract(_timestamps[locationEvent.FlightNumber]).TotalSeconds < 15)
					{
						return true;
					}
					_timestamps[locationEvent.FlightNumber] = DateTime.Now;
				}
				else
				{
					_timestamps.Add(locationEvent.FlightNumber, DateTime.Now);
				}
			}

			return false;
		}

	}
}
