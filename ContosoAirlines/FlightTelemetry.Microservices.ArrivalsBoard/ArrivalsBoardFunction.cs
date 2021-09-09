using FlightTelemetry.Shared;
using FlightTelemetry.Shared.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightTelemetry.Microservices.ArrivalsBoard
{
    public static class ArrivalsBoardFunction
    {
        private static readonly CosmosClient _client =
            new CosmosClient(Environment.GetEnvironmentVariable("CosmosDbConnectionString"));

        private static readonly Dictionary<string, AirportArrivals> _board =
            new Dictionary<string, AirportArrivals>();

        private static DateTime _timestamp;

        private static readonly object _threadLock =
            new object();

        [FunctionName("ArrivalsBoard")]
		public static async Task ArrivalsBoard(
			[CosmosDBTrigger(
				databaseName: Constants.DatabaseName,
				collectionName: Constants.LocationContainerName,
				ConnectionStringSetting = "CosmosDbConnectionString",
				LeaseCollectionName = "lease",
				LeaseCollectionPrefix = "ArrivalsBoard-"
			)]
			IReadOnlyList<Document> documents,
			ILogger logger)
		{
            var includesCompletedFlights = false;
            foreach (var document in documents)
            {
                try
                {
                    var locationEvent = JsonConvert.DeserializeObject<LocationEvent>(document.ToString());
                    includesCompletedFlights = includesCompletedFlights || locationEvent.IsComplete;
                    UpdateBoard(locationEvent);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }

            if (ShouldSkip(includesCompletedFlights))
            {
                return;
            }

            var arrivalsBoardContainer = _client.GetContainer(Constants.DatabaseName, Constants.ArrivalsBoardContainerName);
            foreach (var airport in _board.Keys)
            {
                var arrivals = _board[airport];
                try
                {
                    var result = await arrivalsBoardContainer.UpsertItemAsync
                        (arrivals, new Microsoft.Azure.Cosmos.PartitionKey("arrival"));

                    logger.LogWarning($"Upserted materialized view for airport {airport} ({result.RequestCharge} RUs)");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }
        }

        private static bool ShouldSkip(bool includesCompletedFlights)
        {
            // Make sure not to miss the last upsert for completed flights
            if (includesCompletedFlights)
            {
                return false;
            }

            // Throttle continuous processing by delaying between updates to the arrivalsBoard container
            lock (_threadLock)
            {
                var sinceLastUpdate = DateTime.Now.Subtract(_timestamp);
                if (sinceLastUpdate.TotalSeconds < 5)
                {
                    return true;
                }
                _timestamp = DateTime.Now;
            }
            return false;
        }

        private static void UpdateBoard(LocationEvent locationEvent)
        {
            var airport = locationEvent.ArrivalAirport;

            lock (_threadLock)
            {
                if (!_board.ContainsKey(airport))
                {
                    var arrivals = new AirportArrivals
                    {
                        Id = airport,
                        Type = "arrival",
                        ArrivalAirport = airport,
                        Flights = new List<AirportArrivals.ArrivingFlight>()
                    };
                    _board.Add(airport, arrivals);
                }

                var flights = _board[airport].Flights;

                var flight = flights.FirstOrDefault(f => f.FlightNumber == locationEvent.FlightNumber);
                if (flight == null)
                {
                    flight = new AirportArrivals.ArrivingFlight
                    {
                        FlightNumber = locationEvent.FlightNumber,
                        DepartureAirport = locationEvent.DepartureAirport,
                        RemainingMinutes = locationEvent.RemainingMinutes,
                    };
                    flights.Add(flight);
                }
                else
                {
                    flight.RemainingMinutes = locationEvent.RemainingMinutes;
                }
            }

        }

    }
}
