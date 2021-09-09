using Newtonsoft.Json;
using System;

namespace FlightTelemetry.Shared.Models
{
	public class Flight
	{
		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("type")]
		public string Type { get; set; }
		[JsonProperty("flightNumber")]
		public string FlightNumber { get; set; }
		[JsonProperty("tailNumber")]
		public string TailNumber { get; set; }
		[JsonProperty("departureAirport")]
		public string DepartureAirport { get; set; }
		[JsonProperty("arrivalAirport")]
		public string ArrivalAirport { get; set; }
		[JsonProperty("durationMinutes")]
		public double DurationMinutes { get; set; }
		[JsonProperty("distanceMiles")]
		public int DistanceMiles { get; set; }
		[JsonProperty("arrivalTime")]
		public DateTime ArrivalTime { get; set; }
		[JsonProperty("iconRotation")]
		public double IconRotation { get; set; }
		[JsonProperty("latitude")]
		public double Latitude { get; set; }
		[JsonProperty("longitude")]
		public double Longitude { get; set; }
	}
}
