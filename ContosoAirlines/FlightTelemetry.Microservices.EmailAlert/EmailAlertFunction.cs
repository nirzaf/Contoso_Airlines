using FlightTelemetry.Shared;
using FlightTelemetry.Shared.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightTelemetry.Microservices.EmailAlert
{
    public static class EmailAlertFunction
	{
        private static readonly FlightSpatial _spatial =
            new FlightSpatial();

        private static readonly SendGridClient _sendGridClient =
            new SendGridClient(Environment.GetEnvironmentVariable("SendGridApiKey"));

        private static readonly string _fromEmailAddress =
            Environment.GetEnvironmentVariable("FromEmailAddress");

        private static readonly string _fromName =
            Environment.GetEnvironmentVariable("FromName");

        private static readonly string _toEmailAddress =
            Environment.GetEnvironmentVariable("ToEmailAddress");

        private static readonly Dictionary<string, DateTime> _timestamps =
            new Dictionary<string, DateTime>();

        private static readonly object _threadLock =
            new object();

        [FunctionName("EmailAlert")]
		public static async Task EmailAlert(
			[CosmosDBTrigger(
				databaseName: Constants.DatabaseName,
				collectionName: Constants.LocationContainerName,
				ConnectionStringSetting = "CosmosDbConnectionString",
				LeaseCollectionName = "lease",
				LeaseCollectionPrefix = "EmailAlert-"
			)]
			IReadOnlyList<Document> documents,
			ILogger logger)
		{
            foreach (var document in documents)
            {
                try
                {
                    await CheckFlightLocation(document, logger);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error processing document id {document.Id}: {ex.Message}");
                }
            }
        }

        private static async Task CheckFlightLocation(Document document, ILogger logger)
        {
            var locationEvent = JsonConvert.DeserializeObject<LocationEvent>(document.ToString());
            if (!_spatial.IsInNoFlyZone(locationEvent.Latitude, locationEvent.Longitude))
            {
                return;
            }
            lock (_threadLock)
            {
                if (_timestamps.TryGetValue(locationEvent.FlightNumber, out DateTime lastEmailSent))
                {
                    if (DateTime.Now.Subtract(lastEmailSent).TotalMinutes < 1)
                    {
                        return;
                    }
                }
                if (!_timestamps.ContainsKey(locationEvent.FlightNumber))
                {
                    _timestamps.Add(locationEvent.FlightNumber, DateTime.Now);
                }
                else
                {
                    _timestamps[locationEvent.FlightNumber] = DateTime.Now;
                }
            }
            await SendEmail(locationEvent, logger);
        }

        private static async Task SendEmail(LocationEvent locationEvent, ILogger logger)
        {
            var from = new EmailAddress(_fromEmailAddress, _fromName);
            var to = new EmailAddress(_toEmailAddress);
            var subject = $"Flight {locationEvent.FlightNumber} has entered a no-fly zone";
            var details = JsonConvert.SerializeObject(locationEvent, Formatting.Indented).Split(Environment.NewLine)
                .Where(d => d.Trim() != "{" && d.Trim() != "}")
                .Select(d => FormatDetailRowHtml(d));

            var detailsHtml = "<table>" + string.Join(null, details) + "</table>";

            var body =
                $"You are receiving this notice because flight <b>{locationEvent.FlightNumber}</b> has entered a no-fly zone<br /><br />" +
                detailsHtml;

            var message = MailHelper.CreateSingleEmail(from, to, subject, body, body);

            var response = await _sendGridClient.SendEmailAsync(message);

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                logger.LogWarning(subject);
                return;
            }

            throw new Exception($"Error sending email to '{_toEmailAddress}'");
        }

        private static string FormatDetailRowHtml(string rawDetailLine)
        {
            rawDetailLine = rawDetailLine.Trim();
            var delim = rawDetailLine.IndexOf(@""": ");
            var label = rawDetailLine.Substring(1, delim - 1);
            var value = rawDetailLine.Substring(delim + 2).Trim();
            if (value.EndsWith(","))
            {
                value = value.Substring(0, value.Length - 1);
            }
            if (value.StartsWith('"') && value.EndsWith('"'))
            {
                value = value.Substring(1, value.Length - 2);
            }
            label = Regex.Replace(label, "([a-z])([A-Z])", "$1 $2");
            label = char.ToUpper(label[0]) + label.Substring(1);
            var html = $"<tr><td valign=top style='text-align: right;'>{label}</td><td valign=top><b>{value}</b></td></tr>";
            return html;
        }

    }
}
