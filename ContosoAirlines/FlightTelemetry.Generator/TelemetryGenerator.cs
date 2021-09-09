using Bogus;
using FlightTelemetry.Shared;
using FlightTelemetry.Shared.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightTelementry.Generator
{
	public class TelemetryGenerator
	{
		#region "Constants"

		private const int PayloadIntervalMs = 10;

		#endregion

		#region "Private fields"

		private readonly Airport[] _airports = new[]
		{
			new Airport { Code = "JFK", Longitude = -73.7781  , Latitude = 40.6413, Name = "New York, NY: John F. Kennedy International" },
			new Airport { Code = "LAX", Longitude = -118.4085 , Latitude = 33.9416, Name = "Los Angeles, CA: Los Angeles International" },
			new Airport { Code = "SEA", Longitude = -122.2961 , Latitude = 47.4436, Name = "Seattle, WA: Seattle/Tacoma International" },
			new Airport { Code = "MCO", Longitude = -81.3081  , Latitude = 28.4312, Name = "Orlando, FL: Orlando International" },
			new Airport { Code = "ORD", Longitude = -87.9073  , Latitude = 41.9742, Name = "Chicago, IL: Chicago O'Hare International" },
			new Airport { Code = "DEN", Longitude = -104.6737 , Latitude = 39.8561, Name = "Denver, CO: Denver International" },
		};

		private static readonly IDictionary<string, SpeedAltitudeEntry> _speedAndAltitude = new Dictionary<string, SpeedAltitudeEntry>();
		private static readonly object _threadLock = new object();

		private Flight[] _flights = new[]
		{
			new Flight { FlightNumber = "CA1001", DepartureAirport = "JFK", ArrivalAirport = "LAX", IconRotation = 260 },
			new Flight { FlightNumber = "CA1002", DepartureAirport = "DEN", ArrivalAirport = "LAX", IconRotation = 235 },
			new Flight { FlightNumber = "CA1003", DepartureAirport = "ORD", ArrivalAirport = "LAX", IconRotation = 245 },
			new Flight { FlightNumber = "CA1004", DepartureAirport = "JFK", ArrivalAirport = "SEA", IconRotation = 285 },
			new Flight { FlightNumber = "CA1005", DepartureAirport = "MCO", ArrivalAirport = "LAX", IconRotation = 280 },
			new Flight { FlightNumber = "CA1006", DepartureAirport = "MCO", ArrivalAirport = "JFK", IconRotation = 30 },
			new Flight { FlightNumber = "CA1007", DepartureAirport = "LAX", ArrivalAirport = "MCO", IconRotation = 100 },
			new Flight { FlightNumber = "CA1008", DepartureAirport = "ORD", ArrivalAirport = "MCO", IconRotation = 155 },
			new Flight { FlightNumber = "CA1009", DepartureAirport = "JFK", ArrivalAirport = "MCO", IconRotation = 205},
			new Flight { FlightNumber = "CA1010", DepartureAirport = "SEA", ArrivalAirport = "LAX", IconRotation = 165},
			new Flight { FlightNumber = "CA1011", DepartureAirport = "LAX", ArrivalAirport = "SEA", IconRotation = 350 },
			new Flight { FlightNumber = "CA1012", DepartureAirport = "SEA", ArrivalAirport = "JFK", IconRotation = 105 },
		};

		private readonly FlightRepo _flightRepo = new FlightRepo();
		private readonly FlightSpatial _spatial = new FlightSpatial();

		private readonly Randomizer _random = new Randomizer();

		private int _telemetryCount;
		private int _noFlyZoneCount;
		private List<string> _completedFlights;
		private int _errorCount;

		#endregion

		#region "Properties"

		private (string ContainerName, string PartitionKey, int Throughput, bool AutoScale, int? TimeToLive)[] Containers => new (string, string, int, bool, int?)[]
		{
			(Constants.LeaseContainerName, "/id", 400, false, null),				// for CFP Library
			(Constants.MetadataContainerName, "/type", 400, false, null),			// for small lookup lists; pk=airport/flight; id=airport ID/flight ID (e.g., airport, flight)
			(Constants.LocationContainerName, "/id", 1000, true, null),				// for ingestion; pk/id = GUID
			(Constants.CurrentLocationContainerName, "/type", 400, false, null),	// for currentLocation microservice; pk=location; id=flight ID
			(Constants.ArrivalsBoardContainerName, "/type", 400, false, null),		// for arrivalsBoard microservice; pk=arrival; id=arrival airport ID
		};

		#endregion

		#region "Initialize"

		public async Task InitializeDatabase()
		{
			if (!this.Confirm($"This will create the {Constants.DatabaseName} database. It will be deleted if it already exists."))
			{
				return;
			}

			await this.CreateDatabase();
			await this.CreateContainers();
			await this.CreateAirports();
			await this.CreateFlights();
		}

		private async Task CreateDatabase()
		{
			Console.WriteLine($"Creating database {Constants.DatabaseName}");
			await this.DeleteDatabase(confirm: false);
			await Cosmos.Client.CreateDatabaseAsync(Constants.DatabaseName);
			Console.WriteLine($"Created database {Constants.DatabaseName}");
		}

		private async Task CreateContainers()
		{
			Console.WriteLine("Creating containers");
			foreach (var container in this.Containers)
			{
				await this.CreateContainer(
					container.ContainerName,
					container.PartitionKey,
					container.Throughput,
					container.AutoScale,
					container.TimeToLive);
			}
			Console.WriteLine($"Created containers");
		}

		protected async Task CreateContainer(string containerName, string partitionKey, int throughput, bool autoScale, int? timeToLive)
		{
			var throughputProperties = default(ThroughputProperties);
			if (autoScale)
			{
				throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(throughput * 10);
				Console.WriteLine($"Creating container '{containerName}', partitionKey = {partitionKey}, throughput = {throughput} - {throughput * 10} RUs");
			}
			else
			{
				throughputProperties = ThroughputProperties.CreateManualThroughput(throughput);
				Console.WriteLine($"Creating container '{containerName}', partitionKey = {partitionKey}, throughput = {throughput} RUs");
			}
			var containerProperties = new ContainerProperties
			{
				Id = containerName,
				PartitionKeyPath = partitionKey,
				DefaultTimeToLive = timeToLive,
			};

			await Cosmos.Client.GetDatabase(Constants.DatabaseName).CreateContainerAsync(containerProperties, throughputProperties);
		}

		public async Task DeleteDatabase(bool confirm)
		{
			if (confirm && !this.Confirm($"This will delete the {Constants.DatabaseName} database, if it already exists."))
			{
				return;
			}

			var deleted = false;
			try
			{
				await Cosmos.Client.GetDatabase(Constants.DatabaseName).DeleteAsync();
				deleted = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Could not delete database {Constants.DatabaseName}: {ex.Message}");
			}
			if (deleted)
			{
				Console.WriteLine($"Deleted database {Constants.DatabaseName}");
			}
		}

		private async Task CreateAirports()
		{
			var container = Cosmos.Client.GetContainer(Constants.DatabaseName, Constants.MetadataContainerName);
			foreach (var airport in this._airports)
			{
				airport.Id = airport.Code;
				airport.Type = "airport";
				await container.CreateItemAsync(airport, new PartitionKey(airport.Type));
			}
			Console.WriteLine($"Created {this._airports.Length} airports");
		}

		private async Task CreateFlights()
		{
			var container = Cosmos.Client.GetContainer(Constants.DatabaseName, Constants.MetadataContainerName);
			foreach (var flight in this._flights)
			{
				var departureAirport = this._airports.First(a => a.Code == flight.DepartureAirport);
				var arrivalAirport = this._airports.First(a => a.Code == flight.ArrivalAirport);
				var distance = this._spatial.CalculateDistance(departureAirport.Latitude, departureAirport.Longitude, arrivalAirport.Latitude, arrivalAirport.Longitude);
				var duration = (distance / 500) * 60;  // assume 500 mph
				var arrivalTime = DateTime.UtcNow.AddMinutes(duration);

				flight.Id = flight.FlightNumber;
				flight.Type = "flight";
				flight.TailNumber = this._random.Number(2, 9) * 97 + this._random.AlphaNumeric(2) + this._random.Number(1, 10);
				flight.Latitude = departureAirport.Latitude;
				flight.Longitude = departureAirport.Longitude;
				flight.DistanceMiles = Convert.ToInt32(distance);
				flight.ArrivalTime = arrivalTime;
				flight.DurationMinutes = Math.Round(duration, 2);

				await container.CreateItemAsync(flight, new PartitionKey(flight.Type));
			}

			Console.WriteLine($"Created {this._flights.Length} flights");
		}

		public async Task ResetFlights()
		{
			var airports = await this._flightRepo.GetAirports();
			var flights = await this._flightRepo.GetFlights();

			var container = Cosmos.Client.GetContainer(Constants.DatabaseName, Constants.CurrentLocationContainerName);
			var ctr = 0;
			foreach (var flight in flights)
			{
				try
				{
					await container.DeleteItemAsync<object>(flight.FlightNumber, new PartitionKey(flight.FlightNumber));
					ctr++;
				}
				catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
				}
			}
			Console.WriteLine($"Deleted {ctr} current locations");

			container = Cosmos.Client.GetContainer(Constants.DatabaseName, Constants.ArrivalsBoardContainerName);
			ctr = 0;
			foreach (var airport in airports)
			{
				try
				{
					await container.DeleteItemAsync<object>(airport.Code, new PartitionKey(airport.Code));
					ctr++;
				}
				catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
				}
			}
			Console.WriteLine($"Deleted {ctr} arrivals");

			container = Cosmos.Client.GetContainer(Constants.DatabaseName, Constants.LocationContainerName);
			ctr = 0;
			foreach (var flight in flights)
			{
				var result = await this._flightRepo.QueryLocation(flight.FlightNumber);
				var location = result.LocationEvent;
				if (location != null)
				{
					var airport = airports.First(a => a.Code == location.DepartureAirport);
					location.Latitude = airport.Latitude;
					location.Longitude = airport.Longitude;
					location.RemainingMiles = 0;
					location.RemainingMinutes = 0;
					await container.ReplaceItemAsync(location, location.Id, new PartitionKey(location.Id));
					ctr++;
				}
			}
			Console.WriteLine($"Cleared {ctr} flight locations");

			//container = Cosmos.Client.GetContainer(Constants.DatabaseName, Constants.LeaseContainerName);
			//try
			//{
			//	await container.DeleteContainerAsync();
			//	Console.WriteLine($"Deleted {Constants.LeaseContainerName} container");
			//}
			//catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
			//{
			//}

		}

		#endregion

		#region "Generate"

		public async Task GenerateData(bool continuous)
		{
			Console.Write($"Press any key to start generating telemetry {(continuous ? "continuously" : "through flight completion")}, or ESC to cancel... ");
			var key = Console.ReadKey(true);
			if (key.KeyChar == (char)27)
			{
				Console.WriteLine("cancel");
				return;
			}

			Console.Clear();
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine($"Flight  From > To  Arrives  Distance  Duration  Remaining           Speed    Altitude   Location");
			Console.WriteLine($"------- ---------- -------  --------  --------  ------------------  -------- ---------  -------------------");

			this._flights = await this._flightRepo.GetFlights();

			var currentLine = Console.CursorTop;
			Console.SetCursorPosition(0, 5 + this._flights.Length);
			Console.Write($"Press space bar to pause, ESC to stop... ");
			Console.SetCursorPosition(0, currentLine);

			this._telemetryCount = 0;
			this._noFlyZoneCount = 0;
			this._errorCount = 0;
			this._completedFlights = new List<string>();
			var allFlightsComplete = false;
			var container = Cosmos.Client.GetContainer(Constants.DatabaseName, Constants.LocationContainerName);
			var started = DateTime.UtcNow;
			while (true)
			{
				while (!Console.KeyAvailable)
				{
					var elapsed = DateTime.UtcNow.Subtract(started);
					var telemetry = this.GenerateTelemetry(continuous);
					if (telemetry.Count == 0)
					{
						allFlightsComplete = true;
						break;
					}
					else
					{
						var errorCount = await this.WriteTelemetry(container, telemetry);
						var rate = Math.Round(this._telemetryCount / elapsed.TotalSeconds, 0);
						this._errorCount += errorCount;
						Console.SetCursorPosition(0, 0);
//						Console.WriteLine($"Flights={this._flights.Length}; Telemetry={this._telemetryCount} ({rate}/sec); NoFlyZone={this._noFlyZoneCount}; Errors={this._errorCount}; Elapsed={elapsed}");
						Console.WriteLine($"Flights={this._flights.Length}; Count={this._telemetryCount} ({rate}/sec); Errors={this._errorCount}; Elapsed={elapsed}");
						await Task.Delay(PayloadIntervalMs);
					}
				}
				if (allFlightsComplete)
				{
					break;
				}
				key = Console.ReadKey(true);
				if (key.KeyChar == ' ')
				{
					Console.SetCursorPosition(0, 5 + this._flights.Length);
					Console.Write($"Paused; press any key to resume...       ");
					while (!Console.KeyAvailable) { }
					Console.ReadKey(true);
					Console.SetCursorPosition(0, 5 + this._flights.Length);
					Console.Write($"Press space bar to pause, ESC to stop... ");
				}
				else if (key.KeyChar == (char)27)
				{
					break;
				}
			}

			Console.SetCursorPosition(0, 5 + this._flights.Length);
			Console.WriteLine($"Stopped after generating {this._telemetryCount} telemetry items for {this._flights.Length} flights with {this._errorCount} errors");
		}

		private List<LocationEvent> GenerateTelemetry(bool continuous)
		{
			Console.SetCursorPosition(0, 4); 
			var list = new List<LocationEvent>();
			var ctr = 0;
			foreach (var flight in this._flights)
			{
				if (this._completedFlights.Contains(flight.FlightNumber))
				{
					Console.WriteLine();
					continue;
				}
				ctr++;
				var arrivalAirport = this._airports.First(a => a.Code == flight.ArrivalAirport);
				var remainingMiles = (int)this._spatial.CalculateDistance(flight.Latitude, flight.Longitude, arrivalAirport.Latitude, arrivalAirport.Longitude);
				var remainingMinutes = remainingMiles / 500.0 * 60;  // assume 500 mph

				var speedAndAltitude = this.GenerateSpeedAndAltitude(flight.FlightNumber);

				var item = new LocationEvent
				{
					Id = Guid.NewGuid().ToString(),
					Type = "location",
					FlightNumber = flight.FlightNumber,
					TailNumber = flight.TailNumber,
					DepartureAirport = flight.DepartureAirport,
					ArrivalAirport = flight.ArrivalAirport,
					DurationMinutes = flight.DurationMinutes,
					RemainingMinutes = remainingMinutes,
					DistanceMiles = flight.DistanceMiles,
					RemainingMiles = remainingMiles,
					ArrivalTime = flight.ArrivalTime,
					Latitude = flight.Latitude,
					Longitude = flight.Longitude,
					//IsInNoFlyZone = this._spatial.IsInNoFlyZone(flight.Latitude, flight.Longitude),
					Altitude = speedAndAltitude.Altitude,
					Speed = speedAndAltitude.Speed,
					Ttl = 30 * 60 * 60 * 24,
				};

				if ((remainingMiles < 20) && !continuous)
				{
					item.Speed = 0;
					item.Altitude = 0;
					item.RemainingMiles = 0;
					item.RemainingMinutes = 0;
					item.IsComplete = true;
				}

				Console.WriteLine($"{item.FlightNumber,6}  {item.DepartureAirport} > {item.ArrivalAirport}    {flight.ArrivalTime:HH:mm}   {flight.DistanceMiles,4} mi   {Math.Round(flight.DurationMinutes / 60, 2),-4} hr    {remainingMiles,4} mi, {Math.Round(remainingMinutes / 60, 2),-4:0.00} hr   {item.Speed} mph  {item.Altitude} ft {item.Latitude,9:##00.0000}, {item.Longitude,9:##00.0000}   ");
				list.Add(item);
				this._telemetryCount++;

				if (item.IsComplete)
				{
					this._completedFlights.Add(flight.FlightNumber);
					var line = $"{flight.FlightNumber,6}  {flight.DepartureAirport} > {flight.ArrivalAirport}  ARRIVED";
					line += new string(' ', Console.WindowWidth - line.Length - 1);
					Console.SetCursorPosition(0, Console.CursorTop - 1);
					Console.WriteLine(line);
					continue;
				}

				this.NudgeFlight(flight, continuous);
			}

			return list;
		}

		private SpeedAltitudeEntry GenerateSpeedAndAltitude(string flightNumber)
		{
			lock (_threadLock)
			{
				if (_speedAndAltitude.TryGetValue(flightNumber, out SpeedAltitudeEntry speedAndAltitude))
				{
					if (DateTime.Now.Subtract(speedAndAltitude.Timestamp).TotalSeconds < 1)
					{
						return speedAndAltitude;
					}
				}

				speedAndAltitude = new SpeedAltitudeEntry
				{
					Speed = this._random.Number(490, 510),
					Altitude = this._random.Number(37000, 38000),
					Timestamp = DateTime.Now,
				};

				if (!_speedAndAltitude.ContainsKey(flightNumber))
				{
					_speedAndAltitude.Add(flightNumber, speedAndAltitude);
				}
				else
				{
					_speedAndAltitude[flightNumber] = speedAndAltitude;
				}

				return speedAndAltitude;
			}
		}

		private void NudgeFlight(Flight flight, bool continuous)
		{
			while (true)
			{
				var subtractLat = this._random.Number(0, 1) == 0;
				var subtractLon = this._random.Number(0, 1) == 0;
				var valueLat = this._random.Number(50, 60) / 1500.0;
				var valueLon = this._random.Number(50, 60) / 1500.0;

				var lat = subtractLat ? flight.Latitude - valueLat : flight.Latitude + valueLat;
				var lon = subtractLon ? flight.Longitude - valueLon : flight.Longitude + valueLon;

				// Get the previous distance to the arrival airport
				var arrivalAirport = this._airports.First(a => a.Code == flight.ArrivalAirport);
				var prevRemainingDistance = this._spatial.CalculateDistance(lat, lon, arrivalAirport.Latitude, arrivalAirport.Longitude);

				if ((prevRemainingDistance < 10) && !continuous)
				{
					// Complete the flight if we're within 10 mi of our arrival airport
					break;
				}

				// Get the new distance to the arrival airport
				var newRemainingDistance = this._spatial.CalculateDistance(flight.Latitude, flight.Longitude, arrivalAirport.Latitude, arrivalAirport.Longitude);

				if ((newRemainingDistance < 20) && continuous)
				{
					// Restart the flight if we're within 10 mi of our arrival airport
					var departureAirport = this._airports.First(a => a.Code == flight.DepartureAirport);
					flight.Latitude = departureAirport.Latitude;
					flight.Longitude = departureAirport.Longitude;
					break;
				}

				// Accept the nudge if the new distance is less than the previous
				if (prevRemainingDistance < newRemainingDistance)
				{
					flight.Latitude = Math.Round(lat, 4);
					flight.Longitude = Math.Round(lon, 4);
					if (this._spatial.IsInNoFlyZone(lat, lon))
					{
						this._noFlyZoneCount++;
						break;
					}
					break;
				}
			}
		}

		protected async Task<int> WriteTelemetry(Container container, List<LocationEvent> docs)
		{
			var tasks = new Task[docs.Count];
			var taskCtr = 0;
			var errorCtr = 0;
			foreach (var item in docs)
			{
				tasks[taskCtr] = container
					.CreateItemAsync(item, new PartitionKey(item.Id))
					.ContinueWith(task => errorCtr = this.CatchInsertDocumentError(task, errorCtr));

				taskCtr++;
			}

			await Task.WhenAll(tasks);

			return errorCtr;
		}

		private int CatchInsertDocumentError(Task<ItemResponse<LocationEvent>> task, int errorCtr)
		{
			if (!task.IsCompletedSuccessfully)
			{
				var currentLineNumber = Console.CursorTop;
				Console.SetCursorPosition(0, 5 + this._flights.Length);
				errorCtr++;
				if (task.Exception is AggregateException ae)
				{
					var ctr = 0;
					foreach (var ex in ae.Flatten().InnerExceptions)
					{
						Console.WriteLine($"[{++ctr}] {ex.Message}");
					}
				}
				else
				{
					Console.WriteLine($"Task failed for unknown reasons");
				}
				Console.SetCursorPosition(0, currentLineNumber);
			}

			return errorCtr;
		}

		#endregion

		#region "Helpers"

		private bool Confirm(string message)
		{
			Console.WriteLine(message);
			while (true)
			{
				Console.Write("Are you sure (Y/N)? ");
				var input = Console.ReadLine();
				if (input.ToLower().StartsWith("y"))
				{
					return true;
				}
				if (input.ToLower().StartsWith("n"))
				{
					return false;
				}
			}
		}

		#endregion

		public class SpeedAltitudeEntry
		{
			public int Speed { get; set; }
			public int Altitude { get; set; }
			public DateTime Timestamp { get; set; }
		}

	}
}
