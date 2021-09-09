using FlightTelemetry.Shared;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace FlightTelementry.Generator
{
	public static class Program
	{
		private static readonly FlightRepo _repo = new FlightRepo();
		private static readonly TelemetryGenerator _generator = new TelemetryGenerator();

		public static async Task Main(string[] args)
		{
			Console.WriteLine("Cosmos DB Flight Telemetry Generator");
			Console.WriteLine();

			var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			Cosmos.SetAuth(config["CosmosEndpoint"], config["CosmosMasterKey"]);

			if (args.Length > 0)
			{
				await RunOperation(args);
			}
			else
			{
				await RunInteractive();
			}
		}

		private static async Task RunInteractive()
		{
			ShowUsage();
			while (true)
			{
				Console.Write("Flights> ");
				var input = Console.ReadLine();
				if (!string.IsNullOrWhiteSpace(input))
				{
					if ("quit".StartsWith(input.ToLower()))
					{
						break;
					}
					var args = input.Split(' ');
					await RunOperation(args);
				}
			}
		}

		private static async Task RunOperation(string[] args)
		{
			try
			{
				var operation = args[0].ToLower();
				var arg1 = args.Length > 1 ? args[1].ToLower() : null;

				if (operation.Matches("initialize"))
				{
					await _generator.InitializeDatabase();
				}
				else if (operation.Matches("delete"))
				{
					await _generator.DeleteDatabase(confirm: true);
				}
				else if (operation.Matches("generate"))
				{
					var continuous = (arg1 == "-c");
					await _generator.GenerateData(continuous);
				}
				else if (operation.Matches("airports"))
				{
					await ShowAirports();
				}
				else if (operation.Matches("flights"))
				{
					await ShowFlights();
				}
				else if (operation.Matches("reset"))
				{
					await _generator.ResetFlights();
				}
				else if (operation.Matches("help") || operation == "?")
				{
					ShowUsage();
				}
				else
				{
					throw new Exception("Unrecognized command");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
				ShowUsage();
			}
		}

		private static bool Matches(this string operation, string match) =>
			match.StartsWith(operation);

		private static void ShowUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("  initialize    initialize database");
			Console.WriteLine("  delete        delete database");
			Console.WriteLine("  reset         reset flights");
			Console.WriteLine("  generate -c   generate data (-c for continuous)");
			Console.WriteLine("  airports      show airports");
			Console.WriteLine("  flights       show flights");
			Console.WriteLine("  help (or ?)   show usage");
			Console.WriteLine("  quit          exit utility");
			Console.WriteLine();
		}

		private static async Task ShowFlights()
		{
			var flights = await _repo.GetFlights();
			var ctr = 0;
			foreach (var flight in flights)
			{
				ctr++;
				Console.WriteLine($"[{ctr,2}] {flight.FlightNumber,-8}{flight.DepartureAirport} > {flight.ArrivalAirport}  {flight.DistanceMiles} miles");
			}
		}

		private static async Task ShowAirports()
		{
			var airports = await _repo.GetAirports();
			var ctr = 0;
			foreach (var airport in airports)
			{
				ctr++;
				Console.WriteLine($"[{ctr,2}] {airport.Code} - {airport.Name} ({airport.Latitude}, {airport.Longitude})");
			}
		}

	}
}
