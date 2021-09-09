using System;
using System.Text;

namespace FlightTelemetry.Shared
{
	public static class Extensions
	{
		public static string GetExceptionMessage(this Exception ex)
		{
			var sb = new StringBuilder();
			if (ex is AggregateException aex)
			{
				var errCtr = 0;
				foreach (var iex in aex.Flatten().InnerExceptions)
				{
					sb.Append($"[{++errCtr}] {iex.Message}. ");
				}
			}
			else
			{
				sb.Append(ex.Message);
			}
			return sb.ToString();
		}
	}
}
