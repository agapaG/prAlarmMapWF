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
            this.AlarmMap = new GMap.NET.WindowsForms.GMapControl();
            this.label1 = new System.Windows.Forms.Label();
            this.tLan = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tLng = new System.Windows.Forms.TextBox();
            this.bSize = new System.Windows.Forms.Button();
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
            this.AlarmMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AlarmMap_MouseClick);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 425);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Lan";
            // 
            // tLan
            // 
            this.tLan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tLan.Location = new System.Drawing.Point(47, 425);
            this.tLan.Name = "tLan";
            this.tLan.Size = new System.Drawing.Size(100, 22);
            this.tLan.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(153, 425);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Lng";
            // 
            // tLng
            // 
            this.tLng.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tLng.Location = new System.Drawing.Point(188, 425);
            this.tLng.Name = "tLng";
            this.tLng.Size = new System.Drawing.Size(100, 22);
            this.tLng.TabIndex = 4;
            // 
            // bSize
            // 
            this.bSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bSize.Location = new System.Drawing.Point(303, 425);
            this.bSize.Name = "bSize";
            this.bSize.Size = new System.Drawing.Size(75, 23);
            this.bSize.TabIndex = 9;
            this.bSize.Text = "Size";
            this.bSize.UseVisualStyleBackColor = true;
            this.bSize.Click += new System.EventHandler(this.bSize_Click);
            // 
            // Map
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.bSize);
            this.Controls.Add(this.tLng);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tLan);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AlarmMap);
            this.Name = "Map";
            this.Text = "AlarmMap";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Map_FormClosing);
            this.Load += new System.EventHandler(this.Map_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GMap.NET.WindowsForms.GMapControl AlarmMap;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tLan;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tLng;
        private System.Windows.Forms.Button bSize;
    }
}

