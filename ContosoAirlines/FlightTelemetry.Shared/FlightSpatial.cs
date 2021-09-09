using System;
using System.Collections.Generic;

namespace FlightTelemetry.Shared
{
	public class FlightSpatial
	{
		public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
		{
			var distanceInRadians =
				Math.Sin(this.DegreesToRadians(lat1)) * Math.Sin(this.DegreesToRadians(lat2)) +
				Math.Cos(this.DegreesToRadians(lat1)) * Math.Cos(this.DegreesToRadians(lat2)) * Math.Cos(this.DegreesToRadians(lon1 - lon2));

			var distanceInDegrees = this.RadiansToDegrees(Math.Acos(distanceInRadians));
			var distanceInMiles = distanceInDegrees * 60 * 1.1515;

			return distanceInMiles;
		}

		public IEnumerable<(double, double)> CreateCirclePoints(double centerLatitude, double centerLongitude, double radiusMiles)
		{
			const double EarthRadiusInMiles = 3956.0;
			var lat = this.DegreesToRadians(centerLatitude);
			var lon = this.DegreesToRadians(centerLongitude);
			var d = radiusMiles / EarthRadiusInMiles; // d = angular distance covered on earth's surface
			var points = new List<(double, double)>();

			for (var x = 0; x <= 360; x++)
			{
				var brng = this.DegreesToRadians(x);
				var latRadians = Math.Asin(Math.Sin(lat) * Math.Cos(d) + Math.Cos(lat) * Math.Sin(d) * Math.Cos(brng));
				var lonRadians = lon + Math.Atan2(Math.Sin(brng) * Math.Sin(d) * Math.Cos(lat), Math.Cos(d) - Math.Sin(lat) * Math.Sin(latRadians));

				points.Add((this.RadiansToDegrees(latRadians), this.RadiansToDegrees(lonRadians)));
			}

			return points;
		}

		public bool IsInNoFlyZone(double latitude, double longitude)
		{
			const double DcLatitude = 38.9072;
			const double DcLongitude = -77.0369;

			const int AllowableRadius = 150;

			var distance = this.CalculateDistance(
				latitude, longitude,
				DcLatitude, DcLongitude);

			var isInNoFlyZone = distance < AllowableRadius;

			return isInNoFlyZone;
		}

		private double DegreesToRadians(double degrees) =>
			degrees * Math.PI / 180.0;

		private double RadiansToDegrees(double radians) =>
			radians / Math.PI * 180.0;

	}
}
