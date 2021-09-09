using Microsoft.Azure.Cosmos;

namespace FlightTelemetry.Shared
{
	public static class Cosmos
	{
		private static string _endpoint;
		private static string _masterKey;
		private static string _connectionString;
		private static CosmosClient _client;
		private static CosmosClientOptions _options;

		public static void SetAuth(string endpoint, string masterKey, CosmosClientOptions options = null)
		{
			_endpoint = endpoint;
			_masterKey = masterKey;
			_options = options;
		}

		public static void SetAuth(string connectionString)
		{
			_connectionString = connectionString;
		}

		public static CosmosClient Client
		{
			get
			{
				if (_client == null)
				{
					if (_connectionString == null)
					{
						_client = new CosmosClient(_endpoint, _masterKey, _options);
					}
					else
					{
						_client = new CosmosClient(_connectionString, _options);
					}
				}
				return _client;
			}
		}

	}
}
