namespace CheckmateDX
{
    partial class frmInteractable_DatetimePicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInteractable_DatetimePicker));
            this.timePicker1 = new System.Windows.Forms.DateTimePicker();
            this.labelControlTime = new DevExpress.XtraEditors.LabelControl();
            this.datePicker1 = new System.Windows.Forms.DateTimePicker();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.labelControlDate = new DevExpress.XtraEditors.LabelControl();
            this.panelControlDate = new DevExpress.XtraEditors.PanelControl();
            this.panelControlTime = new DevExpress.XtraEditors.PanelControl();
            this.panelControlButtons = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlDate)).BeginInit();
            this.panelControlDate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlTime)).BeginInit();
            this.panelControlTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlButtons)).BeginInit();
            this.panelControlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // timePicker1
            // 
            this.timePicker1.CustomFormat = "HH:mm";
            this.timePicker1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timePicker1.Font = new System.Drawing.Font("Open Sans", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.timePicker1.Location = new System.Drawing.Point(92, 2);
            this.timePicker1.Name = "timePicker1";
            this.timePicker1.ShowUpDown = true;
            this.timePicker1.Size = new System.Drawing.Size(390, 85);
            this.timePicker1.TabIndex = 1;
            // 
            // labelControlTime
            // 
            this.labelControlTime.Appearance.Font = new System.Drawing.Font("Open Sans", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControlTime.Appearance.Options.UseFont = true;
            this.labelControlTime.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControlTime.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelControlTime.Location = new System.Drawing.Point(2, 2);
            this.labelControlTime.Name = "labelControlTime";
            this.labelControlTime.Size = new System.Drawing.Size(90, 84);
            this.labelControlTime.TabIndex = 1;
            this.labelControlTime.Text = "Time";
            // 
            // datePicker1
            // 
            this.datePicker1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.datePicker1.Font = new System.Drawing.Font("Open Sans", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.datePicker1.Location = new System.Drawing.Point(92, 2);
            this.datePicker1.Name = "datePicker1";
            this.datePicker1.Size = new System.Drawing.Size(390, 33);
            this.datePicker1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(379, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 41);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.ImageOptions.Image")));
            this.btnOk.Location = new System.Drawing.Point(273, 5);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 41);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "&Ok";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // labelControlDate
            // 
            this.labelControlDate.Appearance.Font = new System.Drawing.Font("Open Sans", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControlDate.Appearance.Options.UseFont = true;
            this.labelControlDate.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControlDate.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelControlDate.Location = new System.Drawing.Point(2, 2);
            this.labelControlDate.Name = "labelControlDate";
            this.labelControlDate.Size = new System.Drawing.Size(90, 33);
            this.labelControlDate.TabIndex = 0;
            this.labelControlDate.Text = "Date";
            // 
            // panelControlDate
            // 
            this.panelControlDate.Controls.Add(this.datePicker1);
            this.panelControlDate.Controls.Add(this.labelControlDate);
            this.panelControlDate.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControlDate.Location = new System.Drawing.Point(0, 0);
            this.panelControlDate.Name = "panelControlDate";
            this.panelControlDate.Size = new System.Drawing.Size(484, 37);
            this.panelControlDate.TabIndex = 1;
            // 
            // panelControlTime
            // 
            this.panelControlTime.Controls.Add(this.timePicker1);
            this.panelControlTime.Controls.Add(this.labelControlTime);
            this.panelControlTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControlTime.Location = new System.Drawing.Point(0, 37);
            this.panelControlTime.Name = "panelControlTime";
            this.panelControlTime.Size = new System.Drawing.Size(484, 88);
            this.panelControlTime.TabIndex = 2;
            // 
            // panelControlButtons
            // 
            this.panelControlButtons.Controls.Add(this.btnCancel);
            this.panelControlButtons.Controls.Add(this.btnOk);
            this.panelControlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlButtons.Location = new System.Drawing.Point(0, 125);
            this.panelControlButtons.MaximumSize = new System.Drawing.Size(0, 51);
            this.panelControlButtons.Name = "panelControlButtons";
            this.panelControlButtons.Size = new System.Drawing.Size(484, 51);
            this.panelControlButtons.TabIndex = 3;
            // 
            // frmInteractable_DatetimePicker
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(484, 176);
            this.Controls.Add(this.panelControlButtons);
            this.Controls.Add(this.panelControlTime);
            this.Controls.Add(this.panelControlDate);
            this.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmInteractable_DatetimePicker";
            ((System.ComponentModel.ISupportInitialize)(this.panelControlDate)).EndInit();
            this.panelControlDate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControlTime)).EndInit();
            this.panelControlTime.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControlButtons)).EndInit();
            this.panelControlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker timePicker1;
        private DevExpress.XtraEditors.LabelControl labelControlTime;
        private System.Windows.Forms.DateTimePicker datePicker1;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.LabelControl labelControlDate;
        private DevExpress.XtraEditors.PanelControl panelControlDate;
        private DevExpress.XtraEditors.PanelControl panelControlTime;
        private DevExpress.XtraEditors.PanelControl panelControlButtons;

    }
}
