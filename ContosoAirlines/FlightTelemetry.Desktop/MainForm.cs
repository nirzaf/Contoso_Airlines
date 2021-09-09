using FlightTelemetry.Shared;
using FlightTelemetry.Shared.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FlightTelemetry.Desktop
{
    public partial class MainForm : Form
    {
        #region "Private fields"

        private readonly FlightRepo _repo = new FlightRepo();
        private readonly FlightSpatial _spatial = new FlightSpatial();
        private readonly object _textToLogLock = new object();

        private Airport[] _airports;
        private Flight[] _flights;
        private bool _airportsVisible;
        private bool _washingtonDcVisible;
        private string _textToLog = string.Empty;
        private bool _suppressEvent = false;
        private DateTime _logStartedAt = DateTime.Now;
        private double _loggedRuCharges;

        #endregion

        #region "Properties"

        private Map BingMaps => this.mapUserControl1.Map;

        #endregion

        #region "Setup"

        public MainForm()
        {
            InitializeComponent();

            var config = ConfigurationManager.AppSettings;
            Cosmos.SetAuth(config["CosmosEndpoint"], config["CosmosMasterKey"]);

            this._repo = new FlightRepo();

            this.BingMaps.CredentialsProvider = new ApplicationIdCredentialsProvider(ConfigurationManager.AppSettings["BingMapsKey"]);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.LoadMetadata();
        }

        private void LoadMetadata()
        {
            this.EnableDisableUI(false);
            try
            {
                Task.Run(async () =>
                {
                    this._airports = await this._repo.GetAirports();
                    this._flights = await this._repo.GetFlights();
                }).Wait();

                this.LoadLocationMap();
                this.LoadArrivalsBoard();
            }
            catch (Exception ex)
            {
                var message = ex.GetExceptionMessage();
                this.LogOutput(message);
                System.Windows.Forms.MessageBox.Show(message, "Error loading metadata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.EnableDisableUI(true);
        }

        #endregion

        #region "Location map"

        private void LoadLocationMap()
        {
            this.BingMaps.Mode = new AerialMode(true);
            this.BingMaps.SetView(new Location(39.8283, -98.5795), zoomLevel: 4);

            this.LocationMapTreeView.Nodes.Clear();
            var rootNode = this.LocationMapTreeView.Nodes.Add("All flights");

            foreach (var flight in this._flights)
            {
                var node = new TreeNode
                {
                    Text = $"{flight.FlightNumber} {flight.DepartureAirport} > {flight.ArrivalAirport}",
                    Tag = flight,
                };
                rootNode.Nodes.Add(node);
            }

            this.LocationMapTreeView.ExpandAll();
        }

        private void LocationMapTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (this._suppressEvent)
            {
                return;
            }

            this.CheckUncheckChildNodes(e.Node);
            this.RefreshLocationMap();
        }

        private void AirportsCheckbox_CheckedChanged(object sender, EventArgs e) => this.RefreshLocationMap();

        private void FlightsCheckbox_CheckedChanged(object sender, EventArgs e) => this.RefreshLocationMap();

        private void FlightLabelCheckBox_CheckedChanged(object sender, EventArgs e) => this.RefreshLocationMap();

        private void TrailingCheckBox_CheckedChanged(object sender, EventArgs e) => this.RefreshLocationMap();

        private void LeadingCheckBox_CheckedChanged(object sender, EventArgs e) => this.RefreshLocationMap();

        private void WashDcCheckBox_CheckedChanged(object sender, EventArgs e) => this.RefreshLocationMap();

        private void MaterializedViewCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.ClearLog();
        }

        private void LocationMapTimer_Tick(object sender, EventArgs e) => this.RefreshLocationMap();

        private void RefreshLocationMap()
        {
            System.Windows.Forms.Application.DoEvents();
            var locations = default(Dictionary<string, LocationEvent>);
            Task.Run(async () =>
            {
                locations = await this.GetFlightLocations();
            }).Wait();

            this.RefreshWashingtonDc();
            this.RefreshAirports();
            this.RefreshFlightLocations(locations);
        }

        private void RefreshWashingtonDc()
        {
            if (this.WashDcCheckBox.Checked)
            {
                if (!this._washingtonDcVisible)
                {
                    this.ShowWashingtonDc();
                }
            }
            else
            {
                if (this._washingtonDcVisible)
                {
                    this.HideWashingtonDc();
                }
            }
        }

        private void ShowWashingtonDc()
        {
            const double DcLatitude = 38.9072;
            const double DcLongitude = -77.0369;
            const int AllowableRadius = 150;

            var center = new Location(DcLatitude, DcLongitude);
            var circle = this.CreateCircle(center, AllowableRadius);

            var poly = new MapPolyline
            {
                Locations = circle,
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 2,
                Tag = "dc",
                ToolTip = "Washington DC No-Fly Zone",
            };
            this.BingMaps.Children.Add(poly);
            this._washingtonDcVisible = true;
        }

        public LocationCollection CreateCircle(Location center, double radiusMiles)
        {
            var points = this._spatial.CreateCirclePoints(center.Latitude, center.Longitude, radiusMiles);
            var circle = new LocationCollection();
            foreach (var point in points)
            {
                circle.Add(new Location(point.Item1, point.Item2));
            }

            return circle;
        }

        private void HideWashingtonDc()
        {
            this.HideMapObjects("dc");
            this._washingtonDcVisible = false;
        }

        private void RefreshAirports()
        {
            if (this.AirportsCheckBox.Checked)
            {
                if (!this._airportsVisible)
                {
                    this.ShowAirports();
                }
            }
            else
            {
                if (this._airportsVisible)
                {
                    this.HideAirports();
                }
            }
        }

        private void ShowAirports()
        {
            foreach (var airport in this._airports)
            {
                var pin = new Pushpin
                {
                    Location = new Location(airport.Latitude, airport.Longitude),
                    Tag = "airport",
                    ToolTip = $"{airport.Name}\r\n({airport.Code})",
                };
                this.BingMaps.Children.Add(pin);
            }
            this._airportsVisible = true;
        }

        private void HideAirports()
        {
            this.HideMapObjects("airport");
            this._airportsVisible = false;
        }

        private async Task<Dictionary<string, LocationEvent>> GetFlightLocations()
        {
            if (this.MaterializedViewCheckBox.Checked)
            {
                return await GetFlightLocationsFromMaterializedView();
            }

            var dict = new Dictionary<string, LocationEvent>();

            foreach (TreeNode node in this.LocationMapTreeView.Nodes[0].Nodes)
            {
                System.Windows.Forms.Application.DoEvents();
                var flight = (Flight)node.Tag;
                if (this.FlightsCheckbox.Checked && node.Checked)
                {
                    var queryOperation = await this._repo.QueryLocation(flight.FlightNumber);
                    this.LogOutput($"'{queryOperation.Sql}' - Cost: {queryOperation.Cost} RUs", queryOperation.Cost);
                    dict.Add(flight.FlightNumber, queryOperation.LocationEvent);
                }
                else
                {
                    dict.Add(flight.FlightNumber, null);
                }
            }
            return dict;
        }

        private async Task<Dictionary<string, LocationEvent>> GetFlightLocationsFromMaterializedView()
        {
            var dict = new Dictionary<string, LocationEvent>();
            var flights = new List<Flight>();
            foreach (TreeNode node in this.LocationMapTreeView.Nodes[0].Nodes)
            {
                System.Windows.Forms.Application.DoEvents();
                var flight = (Flight)node.Tag;
                dict.Add(flight.FlightNumber, null);
                if (this.FlightsCheckbox.Checked && node.Checked)
                {
                    flights.Add(flight);
                }
            }

            if (flights.Count <= 3)
            {
                foreach (var flight in flights)
                {
                    var flightNumber = flight.FlightNumber;
                    var readOperation = await this._repo.ReadCurrentLocation(flightNumber);
                    this.LogOutput($"'Point read for flight {flightNumber}' - Cost: {readOperation.Cost} RUs", readOperation.Cost);
                    dict[flightNumber] = readOperation.LocationEvent;
                }
            }
            else if (flights.Count > 3)
            {
                var flightNumbers = flights.Select(f => f.FlightNumber).ToArray();
                var queryOperation = await this._repo.QueryCurrentLocations(flightNumbers);
                foreach (var locationEvent in queryOperation.LocationEvents)
                {
                    dict[locationEvent.FlightNumber] = locationEvent;
                }
                this.LogOutput($"'{queryOperation.Sql}' - Cost: {queryOperation.Cost} RUs", queryOperation.Cost);
            }

            return dict;
        }

        private void RefreshFlightLocations(Dictionary<string, LocationEvent> locations)
        {
            foreach (var flightNumber in locations.Keys)
            {
                HideFlightLocation(flightNumber);
                var location = locations[flightNumber];
                if (location != null && location.RemainingMiles > 50)
                {
                    ShowFlightLocation(location);
                }
            }
        }

        private void ShowFlightLocation(LocationEvent location)
        {
            if (this.TrailingCheckBox.Checked)
            {
                var departureAirport = this._airports.First(a => a.Code == location.DepartureAirport);
                var line = new MapPolyline
                {
                    Locations = new LocationCollection
                    {
                        new Location(departureAirport.Latitude, departureAirport.Longitude),
                        new Location(location.Latitude, location.Longitude)
                    },
                    Stroke = new SolidColorBrush(Colors.Yellow),
                    StrokeThickness = 2,
                    Tag = $"flight.{location.FlightNumber}",
                };
                this.BingMaps.Children.Add(line);
            }

            if (this.LeadingCheckBox.Checked)
            {
                var arrivalAirport = this._airports.First(a => a.Code == location.ArrivalAirport);
                var line = new MapPolyline
                {
                    Locations = new LocationCollection
                    {
                        new Location(arrivalAirport.Latitude, arrivalAirport.Longitude),
                        new Location(location.Latitude, location.Longitude)
                    },
                    Stroke = new SolidColorBrush(Colors.Yellow),
                    StrokeThickness = 2,
                    StrokeDashArray = new DoubleCollection(new double[] { 1 }),
                    Tag = $"flight.{location.FlightNumber}",
                };
                this.BingMaps.Children.Add(line);
            }

            var flight = this._flights.First(f => f.FlightNumber == location.FlightNumber);

            var planeIcon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri(@".\Images\airplane-2-48.png", UriKind.Relative)),
                ToolTip = $"Flight: {location.FlightNumber} ({location.DepartureAirport} > {location.ArrivalAirport})\r\nSpeed: {location.Speed} mph\r\nAltitude: {location.Altitude} ft\r\nLocation: {location.Latitude}, {location.Longitude}",
                Width = 48,
                Height = 48,
                LayoutTransform = new RotateTransform { Angle = flight.IconRotation },
            };
            var planeLayer = new MapLayer
            {
                Tag = $"flight.{location.FlightNumber}"
            };
            planeLayer.AddChild(planeIcon, new Location(location.Latitude, location.Longitude), PositionOrigin.Center);

            if (this.FlightLabelCheckBox.Checked)
            {
                var planeLabel = new System.Windows.Controls.Label
                {
                    Content = location.FlightNumber,
                    FontWeight = FontWeights.Bold,
                    Background = new SolidColorBrush(Colors.Black) { Opacity = .5 },
                    Foreground = System.Windows.Media.Brushes.Yellow,
                };
                planeLayer.AddChild(planeLabel, new Location(location.Latitude, location.Longitude), new System.Windows.Point(-16, 24));
            }

            this.BingMaps.Children.Add(planeLayer);
        }

        private void HideFlightLocation(string flightNumber) =>
            this.HideMapObjects($"flight.{flightNumber}");

        private void HideMapObjects(string tag)
        {
            for (var i = this.BingMaps.Children.Count - 1; i >= 0; i--)
            {
                var child = this.BingMaps.Children[i];
                if (child is FrameworkElement item && item.Tag.ToString() == tag)
                {
                    this.BingMaps.Children.RemoveAt(i);
                }
            }
        }

        #endregion

        #region "Arrivals board"

        private void LoadArrivalsBoard()
        {
            this.ArrivalsBoardTreeView.Nodes.Clear();
            var rootNode = this.ArrivalsBoardTreeView.Nodes.Add("All airports");

            foreach (var airport in this._airports)
            {
                var node = new TreeNode
                {
                    Text = airport.Code,
                    Tag = airport,
                };
                rootNode.Nodes.Add(node);
            }

            this.ArrivalsBoardTreeView.ExpandAll();

            this.ArrivalsBoardDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Consolas", 16);
            this.ArrivalsBoardDataGridView.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.ArrivalsBoardDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Blue;

            this.ArrivalsBoardDataGridView.DefaultCellStyle.Font = new Font("Consolas", 16);
            this.ArrivalsBoardDataGridView.DefaultCellStyle.BackColor = System.Drawing.Color.Blue;
            this.ArrivalsBoardDataGridView.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
        }

        private void ArrivalsBoardTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (this._suppressEvent)
            {
                return;
            }

            this.CheckUncheckChildNodes(e.Node);
            this.RefreshArrivalsBoard();
        }

        private void ArrivalsBoardTimer_Tick(object sender, EventArgs e) =>
            this.RefreshArrivalsBoard();

        private void RefreshArrivalsBoard()
        {
            System.Windows.Forms.Application.DoEvents();
            var arrivals = default(Dictionary<string, AirportArrivals>);
            Task.Run(async () =>
            {
                arrivals = await this.GetArrivalsBoard();
            }).Wait();

            var dt = new DataTable();
            dt.Columns.AddRange(new[]
            {
                new DataColumn("Flight"),
                new DataColumn("From"),
                new DataColumn("To"),
                new DataColumn("Status"),
            });
            foreach (var airportCode in arrivals.Keys)
            {
                var airportArrivals = arrivals[airportCode];
                if (airportArrivals != null)
                {
                    foreach (var flight in airportArrivals.Flights)
                    {
                        var arrivingAt = DateTime.Now.AddMinutes(flight.RemainingMinutes);
                        var status = flight.RemainingMinutes == 0 ? "ARRIVED" : arrivingAt.ToString("hh:mm tt");
                        dt.Rows.Add(new object[] { flight.FlightNumber, flight.DepartureAirport, airportCode, status });
                    }
                }
            }
            this.ArrivalsBoardDataGridView.DataSource = dt;
            this.ArrivalsBoardDataGridView.DefaultCellStyle.SelectionBackColor = this.ArrivalsBoardDataGridView.DefaultCellStyle.BackColor;
            this.ArrivalsBoardDataGridView.DefaultCellStyle.SelectionForeColor = this.ArrivalsBoardDataGridView.DefaultCellStyle.ForeColor;
            this.ArrivalsBoardDataGridView.Refresh();
            System.Windows.Forms.Application.DoEvents();
        }

        private async Task<Dictionary<string, AirportArrivals>> GetArrivalsBoard()
        {
            var dict = new Dictionary<string, AirportArrivals>();
            foreach (TreeNode node in this.ArrivalsBoardTreeView.Nodes[0].Nodes)
            {
                System.Windows.Forms.Application.DoEvents();
                var airport = (Airport)node.Tag;
                if (node.Checked)
                {
                    // With the materialized view, this can be done with a point read on the currentLocation container
                    try
                    {
                        var readOperation = await this._repo.ReadAirportArrivals(airport.Code);
                        this.LogOutput($"'Arrivals board point read by pk/id {airport.Code}' - Cost: {readOperation.Cost} RUs", readOperation.Cost);
                        dict.Add(airport.Code, readOperation.AirportArrivals);
                    }
                    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                    }
                }
                else
                {
                    dict.Add(airport.Code, null);
                }
            }
            return dict;
        }

        #endregion

        #region "Common"

        private void EnableDisableUI(bool enabled)
        {
            this.LocationMapTopPanel.Enabled = enabled;
            this.LocationMapTreeView.Enabled = enabled;
            this.LocationMapTimer.Enabled = enabled;

            this.ArrivalsBoardTreeView.Enabled = enabled;
            this.ArrivalsBoardTimer.Enabled = enabled;
        }

        private void CheckUncheckChildNodes(TreeNode checkedNode)
        {
            this.ClearLog();
            if (checkedNode.Parent == null)
            {
                this._suppressEvent = true;
                foreach (TreeNode node in checkedNode.Nodes)
                {
                    node.Checked = checkedNode.Checked;
                }
                this._suppressEvent = false;
            }
        }

        private void LogOutput(string output, double ruCharge = 0)
        {
            System.Diagnostics.Debug.WriteLine(output);
            lock (this._textToLogLock)
            {
                this._loggedRuCharges += ruCharge;
                this._textToLog += DateTime.Now.ToString() + " " + output + Environment.NewLine;
            }
        }

        private void LogTimer_Tick(object sender, EventArgs e)
        {
            lock (this._textToLogLock)
            {
                if (this._textToLog.Length > 0)
                {
                    this.LogTextBox.AppendText(this._textToLog);
                    this._textToLog = string.Empty;
                }
            }
            this.TotalRUsToolStripLabel.Text = $"{this._loggedRuCharges:0.##}";
            var elapsed = DateTime.Now.Subtract(this._logStartedAt);
            this.ElapsedToolStripLabel.Text = $"{elapsed.Hours}h {elapsed.Minutes}m {elapsed.Seconds}s";

            var average = (int)(this._loggedRuCharges / elapsed.TotalSeconds);
            this.RURateToolStripLabel.Text = $"{average} RU/sec";
        }

        private void ClearLogToolStripButton_Click(object sender, EventArgs e) =>
            this.ClearLog();

        private void ClearLog()
        {
            System.Windows.Forms.Application.DoEvents();
            this.LogTimer.Enabled = false;
            System.Windows.Forms.Application.DoEvents();
            this.LogTextBox.Clear();
            System.Windows.Forms.Application.DoEvents();
            this._loggedRuCharges = 0;
            this._logStartedAt = DateTime.Now;
            this.LogTimer.Enabled = true;
        }

        #endregion

    }
}
