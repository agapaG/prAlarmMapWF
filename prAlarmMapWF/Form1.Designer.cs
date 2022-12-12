namespace prAlarmMapWF
{
    partial class Map
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Map));
            this.AlarmMap = new GMap.NET.WindowsForms.GMapControl();
            this.SuspendLayout();
            // 
            // AlarmMap
            // 
            this.AlarmMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AlarmMap.Bearing = 0F;
            this.AlarmMap.CanDragMap = true;
            this.AlarmMap.EmptyTileColor = System.Drawing.Color.Navy;
            this.AlarmMap.GrayScaleMode = false;
            this.AlarmMap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.AlarmMap.LevelsKeepInMemmory = 5;
            this.AlarmMap.Location = new System.Drawing.Point(7, -12);
            this.AlarmMap.MarkersEnabled = true;
            this.AlarmMap.MaxZoom = 2;
            this.AlarmMap.MinZoom = 2;
            this.AlarmMap.MouseWheelZoomEnabled = true;
            this.AlarmMap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.AlarmMap.Name = "AlarmMap";
            this.AlarmMap.NegativeMode = false;
            this.AlarmMap.PolygonsEnabled = true;
            this.AlarmMap.RetryLoadTile = 0;
            this.AlarmMap.RoutesEnabled = true;
            this.AlarmMap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.AlarmMap.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.AlarmMap.ShowTileGridLines = false;
            this.AlarmMap.Size = new System.Drawing.Size(814, 459);
            this.AlarmMap.TabIndex = 0;
            this.AlarmMap.Zoom = 0D;
            this.AlarmMap.Load += new System.EventHandler(this.AlarmMap_Load);
            this.AlarmMap.Scroll += new System.Windows.Forms.ScrollEventHandler(this.AlarmMap_Scroll);
            // 
            // Map
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.AlarmMap);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Map";
            this.Text = "AlarmMap";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Map_FormClosing);
            this.Load += new System.EventHandler(this.Map_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private GMap.NET.WindowsForms.GMapControl AlarmMap;
    }
}

