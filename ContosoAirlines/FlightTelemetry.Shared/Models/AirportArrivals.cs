using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlightTelemetry.Shared.Models
{
	public class AirportArrivals
	{
		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("type")]
		public string Type { get; set; }
		[JsonProperty("arrivalAirport")]
		public string ArrivalAirport { get; set; }
		[JsonProperty("flights")]
		public List<ArrivingFlight> Flights { get; set; }

		public class ArrivingFlight
		{
			[JsonProperty("flightNumber")]
			public string FlightNumber { get; set; }
			[JsonProperty("departureAirport")]
			public string DepartureAirport { get; set; }
			[JsonProperty("remainingMinutes")]
			public double RemainingMinutes { get; set; }
		}
	}
}
