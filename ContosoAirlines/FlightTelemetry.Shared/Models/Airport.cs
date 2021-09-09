using Newtonsoft.Json;

namespace FlightTelemetry.Shared.Models
{
	public class Airport
	{
		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("type")]
		public string Type { get; set; }
		[JsonProperty("code")]
		public string Code { get; set; }
		[JsonProperty("latitude")]
		public double Latitude { get; set; }
		[JsonProperty("longitude")]
		public double Longitude { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
	}
}
