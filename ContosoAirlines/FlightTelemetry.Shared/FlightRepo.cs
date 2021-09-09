using FlightTelemetry.Shared.Models;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FlightTelemetry.Shared
{
	public class FlightRepo
	{
		public async Task<Airport[]> GetAirports() =>
			await this.GetList<Airport>("airport");

		public async Task<Flight[]> GetFlights() =>
			await this.GetList<Flight>("flight");

		public async Task<T[]> GetList<T>(string type)
		{
			var list = new List<T>();
			var container = Cosmos.Client.GetContainer(Constants.DatabaseName, Constants.MetadataContainerName);
			var sql = $"SELECT * FROM c WHERE c.type = '{type}'";
			var options = new QueryRequestOptions { PartitionKey = new PartitionKey(type) };
			var iterator = container.GetItemQueryIterator<T>(sql, requestOptions: options);
			while (iterator.HasMoreResults)
			{
				var page = await iterator.ReadNextAsync();
				list.AddRange(page);
			}
			return list.ToArray();
		}

		public async Task<(LocationEvent LocationEvent, string Sql, double Cost)> QueryLocation(string flightNumber)
		{
			var container = Cosmos.Client.GetContainer(Constants.DatabaseName, Constants.LocationContainerName);
			var query = $"SELECT TOP 1 * FROM c WHERE c.flightNumber = '{flightNumber}' ORDER BY c._ts DESC";
			var options = new QueryRequestOptions { MaxConcurrency = -1 };
			var iterator = container.GetItemQueryIterator<LocationEvent>(query, requestOptions: options);
			var page = await iterator.ReadNextAsync();
			var cost = page.RequestCharge;
			var location = page.FirstOrDefault();

			return (location, query, cost);
		}

		public async Task<(LocationEvent LocationEvent, double Cost)> ReadCurrentLocation(string flightNumber)
		{
			try
			{
				var container = Cosmos.Client.GetContainer(Constants.DatabaseName, Constants.CurrentLocationContainerName);
				var result = await container.ReadItemAsync<LocationEvent>(flightNumber, new PartitionKey("location"));
				var cost = result.RequestCharge;    // should never be more than 1 RU for documents under 1K
				var location = result.Resource;

				return (location, cost);
			}
			catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				return (null, 0);
			}
		}

		public async Task<(LocationEvent[] LocationEvents, string Sql, double Cost)> QueryCurrentLocations(string[] flightNumbers)
		{
			var container = Cosmos.Client.GetContainer(Constants.DatabaseName, Constants.CurrentLocationContainerName);
			var flightNumbersCsv = string.Join("','", flightNumbers);
			var query = $"SELECT * FROM c WHERE c.type = 'location' AND c.flightNumber IN('{flightNumbersCsv}')";
			var options = new QueryRequestOptions { PartitionKey = new PartitionKey("location") };
			var iterator = container.GetItemQueryIterator<LocationEvent>(query, requestOptions: options);

			var locations = new List<LocationEvent>();
			var cost = 0d;
			while (iterator.HasMoreResults)
			{
				var page = await iterator.ReadNextAsync();
				cost += page.RequestCharge;
				locations.AddRange(page);
			}

			return (locations.ToArray(), query, cost);
		}

		public async Task<(AirportArrivals AirportArrivals, double Cost)> ReadAirportArrivals(string airportCode)
        {
			try
			{
				var container = Cosmos.Client.GetContainer(Constants.DatabaseName, Constants.ArrivalsBoardContainerName);
				var result = await container.ReadItemAsync<AirportArrivals>(airportCode, new PartitionKey("arrival"));
				var cost = result.RequestCharge;    // should never be more than 1 RU for documents under 1K
				var arrivals = result.Resource;

				return (arrivals, cost);
			}
			catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
			{
				return (null, 0);
			}
		}

	}
}
