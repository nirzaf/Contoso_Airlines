namespace FlightTelemetry.Desktop
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.LocationMapSplitContainer = new System.Windows.Forms.SplitContainer();
			this.LocationMapTreeView = new System.Windows.Forms.TreeView();
			this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
			this.mapUserControl1 = new FlightTelemetry.Desktop.MapUserControl();
			this.LocationMapTopPanel = new System.Windows.Forms.Panel();
			this.FlightLabelCheckBox = new System.Windows.Forms.CheckBox();
			this.MaterializedViewCheckBox = new System.Windows.Forms.CheckBox();
			this.WashDcCheckBox = new System.Windows.Forms.CheckBox();
			this.LeadingCheckBox = new System.Windows.Forms.CheckBox();
			this.TrailingCheckBox = new System.Windows.Forms.CheckBox();
			this.FlightsCheckbox = new System.Windows.Forms.CheckBox();
			this.AirportsCheckBox = new System.Windows.Forms.CheckBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.ArrivalsBoardSplitContainer = new System.Windows.Forms.SplitContainer();
			this.ArrivalsBoardTreeView = new System.Windows.Forms.TreeView();
			this.ArrivalsBoardDataGridView = new System.Windows.Forms.DataGridView();
			this.label2 = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.LogTextBox = new System.Windows.Forms.TextBox();
			this.toolStrip2 = new System.Windows.Forms.ToolStrip();
			this.ClearLogToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.TotalRUsToolStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
			this.ElapsedToolStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this.RURateToolStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.LocationMapTimer = new System.Windows.Forms.Timer(this.components);
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
			this.LogTimer = new System.Windows.Forms.Timer(this.components);
			this.ArrivalsBoardTimer = new System.Windows.Forms.Timer(this.components);
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.LocationMapSplitContainer)).BeginInit();
			this.LocationMapSplitContainer.Panel1.SuspendLayout();
			this.LocationMapSplitContainer.Panel2.SuspendLayout();
			this.LocationMapSplitContainer.SuspendLayout();
			this.LocationMapTopPanel.SuspendLayout();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ArrivalsBoardSplitContainer)).BeginInit();
			this.ArrivalsBoardSplitContainer.Panel1.SuspendLayout();
			this.ArrivalsBoardSplitContainer.Panel2.SuspendLayout();
			this.ArrivalsBoardSplitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ArrivalsBoardDataGridView)).BeginInit();
			this.tabPage3.SuspendLayout();
			this.toolStrip2.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(4, 4);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(936, 473);
			this.tabControl1.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.Color.DarkGray;
			this.tabPage1.Controls.Add(this.LocationMapSplitContainer);
			this.tabPage1.Controls.Add(this.LocationMapTopPanel);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
			this.tabPage1.Size = new System.Drawing.Size(928, 447);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Location Map";
			// 
			// LocationMapSplitContainer
			// 
			this.LocationMapSplitContainer.BackColor = System.Drawing.Color.Silver;
			this.LocationMapSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LocationMapSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.LocationMapSplitContainer.Location = new System.Drawing.Point(2, 33);
			this.LocationMapSplitContainer.Margin = new System.Windows.Forms.Padding(2);
			this.LocationMapSplitContainer.Name = "LocationMapSplitContainer";
			// 
			// LocationMapSplitContainer.Panel1
			// 
			this.LocationMapSplitContainer.Panel1.Controls.Add(this.LocationMapTreeView);
			// 
			// LocationMapSplitContainer.Panel2
			// 
			this.LocationMapSplitContainer.Panel2.Controls.Add(this.elementHost1);
			this.LocationMapSplitContainer.Size = new System.Drawing.Size(924, 412);
			this.LocationMapSplitContainer.SplitterDistance = 172;
			this.LocationMapSplitContainer.SplitterWidth = 3;
			this.LocationMapSplitContainer.TabIndex = 2;
			// 
			// LocationMapTreeView
			// 
			this.LocationMapTreeView.BackColor = System.Drawing.Color.Gray;
			this.LocationMapTreeView.CheckBoxes = true;
			this.LocationMapTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LocationMapTreeView.ForeColor = System.Drawing.Color.White;
			this.LocationMapTreeView.Location = new System.Drawing.Point(0, 0);
			this.LocationMapTreeView.Margin = new System.Windows.Forms.Padding(2);
			this.LocationMapTreeView.Name = "LocationMapTreeView";
			this.LocationMapTreeView.ShowLines = false;
			this.LocationMapTreeView.ShowPlusMinus = false;
			this.LocationMapTreeView.ShowRootLines = false;
			this.LocationMapTreeView.Size = new System.Drawing.Size(172, 412);
			this.LocationMapTreeView.TabIndex = 0;
			this.LocationMapTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.LocationMapTreeView_AfterCheck);
			// 
			// elementHost1
			// 
			this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.elementHost1.Location = new System.Drawing.Point(0, 0);
			this.elementHost1.Margin = new System.Windows.Forms.Padding(2);
			this.elementHost1.Name = "elementHost1";
			this.elementHost1.Size = new System.Drawing.Size(749, 412);
			this.elementHost1.TabIndex = 0;
			this.elementHost1.Text = "elementHost1";
			this.elementHost1.Child = this.mapUserControl1;
			// 
			// LocationMapTopPanel
			// 
			this.LocationMapTopPanel.BackColor = System.Drawing.Color.Gray;
			this.LocationMapTopPanel.Controls.Add(this.FlightLabelCheckBox);
			this.LocationMapTopPanel.Controls.Add(this.MaterializedViewCheckBox);
			this.LocationMapTopPanel.Controls.Add(this.WashDcCheckBox);
			this.LocationMapTopPanel.Controls.Add(this.LeadingCheckBox);
			this.LocationMapTopPanel.Controls.Add(this.TrailingCheckBox);
			this.LocationMapTopPanel.Controls.Add(this.FlightsCheckbox);
			this.LocationMapTopPanel.Controls.Add(this.AirportsCheckBox);
			this.LocationMapTopPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.LocationMapTopPanel.Location = new System.Drawing.Point(2, 2);
			this.LocationMapTopPanel.Margin = new System.Windows.Forms.Padding(2);
			this.LocationMapTopPanel.Name = "LocationMapTopPanel";
			this.LocationMapTopPanel.Size = new System.Drawing.Size(924, 31);
			this.LocationMapTopPanel.TabIndex = 1;
			// 
			// FlightLabelCheckBox
			// 
			this.FlightLabelCheckBox.AutoSize = true;
			this.FlightLabelCheckBox.Checked = true;
			this.FlightLabelCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.FlightLabelCheckBox.ForeColor = System.Drawing.Color.White;
			this.FlightLabelCheckBox.Location = new System.Drawing.Point(156, 8);
			this.FlightLabelCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.FlightLabelCheckBox.Name = "FlightLabelCheckBox";
			this.FlightLabelCheckBox.Size = new System.Drawing.Size(86, 17);
			this.FlightLabelCheckBox.TabIndex = 7;
			this.FlightLabelCheckBox.Text = "Flight Label";
			this.FlightLabelCheckBox.UseVisualStyleBackColor = true;
			this.FlightLabelCheckBox.CheckedChanged += new System.EventHandler(this.FlightLabelCheckBox_CheckedChanged);
			// 
			// MaterializedViewCheckBox
			// 
			this.MaterializedViewCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MaterializedViewCheckBox.AutoSize = true;
			this.MaterializedViewCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.MaterializedViewCheckBox.ForeColor = System.Drawing.Color.White;
			this.MaterializedViewCheckBox.Location = new System.Drawing.Point(778, 8);
			this.MaterializedViewCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.MaterializedViewCheckBox.Name = "MaterializedViewCheckBox";
			this.MaterializedViewCheckBox.Size = new System.Drawing.Size(136, 17);
			this.MaterializedViewCheckBox.TabIndex = 6;
			this.MaterializedViewCheckBox.Text = "Use materialized view";
			this.MaterializedViewCheckBox.UseVisualStyleBackColor = true;
			this.MaterializedViewCheckBox.CheckedChanged += new System.EventHandler(this.MaterializedViewCheckBox_CheckedChanged);
			// 
			// WashDcCheckBox
			// 
			this.WashDcCheckBox.AutoSize = true;
			this.WashDcCheckBox.ForeColor = System.Drawing.Color.White;
			this.WashDcCheckBox.Location = new System.Drawing.Point(424, 8);
			this.WashDcCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.WashDcCheckBox.Name = "WashDcCheckBox";
			this.WashDcCheckBox.Size = new System.Drawing.Size(111, 17);
			this.WashDcCheckBox.TabIndex = 5;
			this.WashDcCheckBox.Text = "Washington, DC";
			this.WashDcCheckBox.UseVisualStyleBackColor = true;
			this.WashDcCheckBox.CheckedChanged += new System.EventHandler(this.WashDcCheckBox_CheckedChanged);
			// 
			// LeadingCheckBox
			// 
			this.LeadingCheckBox.AutoSize = true;
			this.LeadingCheckBox.Checked = true;
			this.LeadingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.LeadingCheckBox.ForeColor = System.Drawing.Color.White;
			this.LeadingCheckBox.Location = new System.Drawing.Point(336, 8);
			this.LeadingCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.LeadingCheckBox.Name = "LeadingCheckBox";
			this.LeadingCheckBox.Size = new System.Drawing.Size(77, 17);
			this.LeadingCheckBox.TabIndex = 4;
			this.LeadingCheckBox.Text = "Lead path";
			this.LeadingCheckBox.UseVisualStyleBackColor = true;
			this.LeadingCheckBox.CheckedChanged += new System.EventHandler(this.LeadingCheckBox_CheckedChanged);
			// 
			// TrailingCheckBox
			// 
			this.TrailingCheckBox.AutoSize = true;
			this.TrailingCheckBox.Checked = true;
			this.TrailingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TrailingCheckBox.ForeColor = System.Drawing.Color.White;
			this.TrailingCheckBox.Location = new System.Drawing.Point(252, 8);
			this.TrailingCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.TrailingCheckBox.Name = "TrailingCheckBox";
			this.TrailingCheckBox.Size = new System.Drawing.Size(74, 17);
			this.TrailingCheckBox.TabIndex = 3;
			this.TrailingCheckBox.Text = "Trail path";
			this.TrailingCheckBox.UseVisualStyleBackColor = true;
			this.TrailingCheckBox.CheckedChanged += new System.EventHandler(this.TrailingCheckBox_CheckedChanged);
			// 
			// FlightsCheckbox
			// 
			this.FlightsCheckbox.AutoSize = true;
			this.FlightsCheckbox.Checked = true;
			this.FlightsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.FlightsCheckbox.ForeColor = System.Drawing.Color.White;
			this.FlightsCheckbox.Location = new System.Drawing.Point(84, 8);
			this.FlightsCheckbox.Margin = new System.Windows.Forms.Padding(2);
			this.FlightsCheckbox.Name = "FlightsCheckbox";
			this.FlightsCheckbox.Size = new System.Drawing.Size(61, 17);
			this.FlightsCheckbox.TabIndex = 1;
			this.FlightsCheckbox.Text = "Flights";
			this.FlightsCheckbox.UseVisualStyleBackColor = true;
			this.FlightsCheckbox.CheckedChanged += new System.EventHandler(this.FlightsCheckbox_CheckedChanged);
			// 
			// AirportsCheckBox
			// 
			this.AirportsCheckBox.AutoSize = true;
			this.AirportsCheckBox.Checked = true;
			this.AirportsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.AirportsCheckBox.ForeColor = System.Drawing.Color.White;
			this.AirportsCheckBox.Location = new System.Drawing.Point(8, 8);
			this.AirportsCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.AirportsCheckBox.Name = "AirportsCheckBox";
			this.AirportsCheckBox.Size = new System.Drawing.Size(67, 17);
			this.AirportsCheckBox.TabIndex = 0;
			this.AirportsCheckBox.Text = "Airports";
			this.AirportsCheckBox.UseVisualStyleBackColor = true;
			this.AirportsCheckBox.CheckedChanged += new System.EventHandler(this.AirportsCheckbox_CheckedChanged);
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.Color.DarkGray;
			this.tabPage2.Controls.Add(this.ArrivalsBoardSplitContainer);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
			this.tabPage2.Size = new System.Drawing.Size(968, 446);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Arrivals Board";
			// 
			// ArrivalsBoardSplitContainer
			// 
			this.ArrivalsBoardSplitContainer.BackColor = System.Drawing.Color.Silver;
			this.ArrivalsBoardSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ArrivalsBoardSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.ArrivalsBoardSplitContainer.Location = new System.Drawing.Point(2, 2);
			this.ArrivalsBoardSplitContainer.Margin = new System.Windows.Forms.Padding(2);
			this.ArrivalsBoardSplitContainer.Name = "ArrivalsBoardSplitContainer";
			// 
			// ArrivalsBoardSplitContainer.Panel1
			// 
			this.ArrivalsBoardSplitContainer.Panel1.Controls.Add(this.ArrivalsBoardTreeView);
			// 
			// ArrivalsBoardSplitContainer.Panel2
			// 
			this.ArrivalsBoardSplitContainer.Panel2.BackColor = System.Drawing.Color.DimGray;
			this.ArrivalsBoardSplitContainer.Panel2.Controls.Add(this.ArrivalsBoardDataGridView);
			this.ArrivalsBoardSplitContainer.Panel2.Controls.Add(this.label2);
			this.ArrivalsBoardSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(16);
			this.ArrivalsBoardSplitContainer.Size = new System.Drawing.Size(964, 442);
			this.ArrivalsBoardSplitContainer.SplitterDistance = 172;
			this.ArrivalsBoardSplitContainer.SplitterWidth = 3;
			this.ArrivalsBoardSplitContainer.TabIndex = 6;
			// 
			// ArrivalsBoardTreeView
			// 
			this.ArrivalsBoardTreeView.BackColor = System.Drawing.Color.Gray;
			this.ArrivalsBoardTreeView.CheckBoxes = true;
			this.ArrivalsBoardTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ArrivalsBoardTreeView.ForeColor = System.Drawing.Color.White;
			this.ArrivalsBoardTreeView.Location = new System.Drawing.Point(0, 0);
			this.ArrivalsBoardTreeView.Margin = new System.Windows.Forms.Padding(2);
			this.ArrivalsBoardTreeView.Name = "ArrivalsBoardTreeView";
			this.ArrivalsBoardTreeView.ShowLines = false;
			this.ArrivalsBoardTreeView.ShowPlusMinus = false;
			this.ArrivalsBoardTreeView.ShowRootLines = false;
			this.ArrivalsBoardTreeView.Size = new System.Drawing.Size(172, 442);
			this.ArrivalsBoardTreeView.TabIndex = 0;
			this.ArrivalsBoardTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.ArrivalsBoardTreeView_AfterCheck);
			// 
			// ArrivalsBoardDataGridView
			// 
			this.ArrivalsBoardDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.ArrivalsBoardDataGridView.BackgroundColor = System.Drawing.Color.Blue;
			this.ArrivalsBoardDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.ArrivalsBoardDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
			this.ArrivalsBoardDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.Color.DimGray;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DimGray;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.ArrivalsBoardDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.ArrivalsBoardDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.ArrivalsBoardDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ArrivalsBoardDataGridView.EnableHeadersVisualStyles = false;
			this.ArrivalsBoardDataGridView.Location = new System.Drawing.Point(16, 66);
			this.ArrivalsBoardDataGridView.Name = "ArrivalsBoardDataGridView";
			this.ArrivalsBoardDataGridView.ReadOnly = true;
			this.ArrivalsBoardDataGridView.RowHeadersVisible = false;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ArrivalsBoardDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
			this.ArrivalsBoardDataGridView.Size = new System.Drawing.Size(757, 360);
			this.ArrivalsBoardDataGridView.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Dock = System.Windows.Forms.DockStyle.Top;
			this.label2.Font = new System.Drawing.Font("Segoe UI Light", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.White;
			this.label2.Location = new System.Drawing.Point(16, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(757, 50);
			this.label2.TabIndex = 4;
			this.label2.Text = "ARRIVALS";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.LogTextBox);
			this.tabPage3.Controls.Add(this.toolStrip2);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(968, 446);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Logs";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// LogTextBox
			// 
			this.LogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LogTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LogTextBox.Location = new System.Drawing.Point(3, 28);
			this.LogTextBox.Multiline = true;
			this.LogTextBox.Name = "LogTextBox";
			this.LogTextBox.ReadOnly = true;
			this.LogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.LogTextBox.Size = new System.Drawing.Size(962, 415);
			this.LogTextBox.TabIndex = 0;
			// 
			// toolStrip2
			// 
			this.toolStrip2.BackColor = System.Drawing.Color.Transparent;
			this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ClearLogToolStripButton,
            this.toolStripLabel1,
            this.TotalRUsToolStripLabel,
            this.toolStripSeparator1,
            this.toolStripLabel3,
            this.ElapsedToolStripLabel,
            this.toolStripSeparator2,
            this.toolStripLabel2,
            this.RURateToolStripLabel});
			this.toolStrip2.Location = new System.Drawing.Point(3, 3);
			this.toolStrip2.Name = "toolStrip2";
			this.toolStrip2.Size = new System.Drawing.Size(962, 25);
			this.toolStrip2.TabIndex = 3;
			this.toolStrip2.Text = "toolStrip2";
			// 
			// ClearLogToolStripButton
			// 
			this.ClearLogToolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.ClearLogToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.ClearLogToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("ClearLogToolStripButton.Image")));
			this.ClearLogToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ClearLogToolStripButton.Name = "ClearLogToolStripButton";
			this.ClearLogToolStripButton.Size = new System.Drawing.Size(38, 22);
			this.ClearLogToolStripButton.Text = "Clear";
			this.ClearLogToolStripButton.Click += new System.EventHandler(this.ClearLogToolStripButton_Click);
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(55, 22);
			this.toolStripLabel1.Text = "Total RUs";
			// 
			// TotalRUsToolStripLabel
			// 
			this.TotalRUsToolStripLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.TotalRUsToolStripLabel.Name = "TotalRUsToolStripLabel";
			this.TotalRUsToolStripLabel.Size = new System.Drawing.Size(14, 22);
			this.TotalRUsToolStripLabel.Text = "0";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripLabel3
			// 
			this.toolStripLabel3.Name = "toolStripLabel3";
			this.toolStripLabel3.Size = new System.Drawing.Size(47, 22);
			this.toolStripLabel3.Text = "Elapsed";
			// 
			// ElapsedToolStripLabel
			// 
			this.ElapsedToolStripLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.ElapsedToolStripLabel.Name = "ElapsedToolStripLabel";
			this.ElapsedToolStripLabel.Size = new System.Drawing.Size(55, 22);
			this.ElapsedToolStripLabel.Text = "00:00:00";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripLabel2
			// 
			this.toolStripLabel2.Name = "toolStripLabel2";
			this.toolStripLabel2.Size = new System.Drawing.Size(50, 22);
			this.toolStripLabel2.Text = "Average";
			// 
			// RURateToolStripLabel
			// 
			this.RURateToolStripLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.RURateToolStripLabel.Name = "RURateToolStripLabel";
			this.RURateToolStripLabel.Size = new System.Drawing.Size(57, 22);
			this.RURateToolStripLabel.Text = "0 RU/sec";
			// 
			// LocationMapTimer
			// 
			this.LocationMapTimer.Enabled = true;
			this.LocationMapTimer.Interval = 3000;
			this.LocationMapTimer.Tick += new System.EventHandler(this.LocationMapTimer_Tick);
			// 
			// toolStrip1
			// 
			this.toolStrip1.BackColor = System.Drawing.Color.Black;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel4});
			this.toolStrip1.Location = new System.Drawing.Point(4, 4);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(837, 33);
			this.toolStrip1.TabIndex = 2;
			this.toolStrip1.Text = "toolStrip1";
			this.toolStrip1.Visible = false;
			// 
			// toolStripLabel4
			// 
			this.toolStripLabel4.Font = new System.Drawing.Font("Segoe UI Semilight", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toolStripLabel4.ForeColor = System.Drawing.Color.White;
			this.toolStripLabel4.Name = "toolStripLabel4";
			this.toolStripLabel4.Size = new System.Drawing.Size(517, 30);
			this.toolStripLabel4.Text = "Cosmos DB realtime event sourcing with microservices";
			// 
			// LogTimer
			// 
			this.LogTimer.Enabled = true;
			this.LogTimer.Interval = 1000;
			this.LogTimer.Tick += new System.EventHandler(this.LogTimer_Tick);
			// 
			// ArrivalsBoardTimer
			// 
			this.ArrivalsBoardTimer.Enabled = true;
			this.ArrivalsBoardTimer.Interval = 5000;
			this.ArrivalsBoardTimer.Tick += new System.EventHandler(this.ArrivalsBoardTimer_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Silver;
			this.ClientSize = new System.Drawing.Size(944, 481);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.toolStrip1);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "MainForm";
			this.Padding = new System.Windows.Forms.Padding(4);
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Contoso Airlines Demo";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.LocationMapSplitContainer.Panel1.ResumeLayout(false);
			this.LocationMapSplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.LocationMapSplitContainer)).EndInit();
			this.LocationMapSplitContainer.ResumeLayout(false);
			this.LocationMapTopPanel.ResumeLayout(false);
			this.LocationMapTopPanel.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.ArrivalsBoardSplitContainer.Panel1.ResumeLayout(false);
			this.ArrivalsBoardSplitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ArrivalsBoardSplitContainer)).EndInit();
			this.ArrivalsBoardSplitContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ArrivalsBoardDataGridView)).EndInit();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.toolStrip2.ResumeLayout(false);
			this.toolStrip2.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private MapUserControl mapUserControl1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel LocationMapTopPanel;
        private System.Windows.Forms.CheckBox AirportsCheckBox;
        private System.Windows.Forms.CheckBox FlightsCheckbox;
        private System.Windows.Forms.Timer LocationMapTimer;
        private System.Windows.Forms.SplitContainer LocationMapSplitContainer;
        private System.Windows.Forms.TreeView LocationMapTreeView;
        private System.Windows.Forms.CheckBox LeadingCheckBox;
        private System.Windows.Forms.CheckBox TrailingCheckBox;
        private System.Windows.Forms.CheckBox WashDcCheckBox;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TextBox LogTextBox;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Timer LogTimer;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton ClearLogToolStripButton;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel TotalRUsToolStripLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripLabel ElapsedToolStripLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripLabel RURateToolStripLabel;
        private System.Windows.Forms.CheckBox MaterializedViewCheckBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.Timer ArrivalsBoardTimer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView ArrivalsBoardDataGridView;
        private System.Windows.Forms.SplitContainer ArrivalsBoardSplitContainer;
        private System.Windows.Forms.TreeView ArrivalsBoardTreeView;
        private System.Windows.Forms.CheckBox FlightLabelCheckBox;
    }
}

