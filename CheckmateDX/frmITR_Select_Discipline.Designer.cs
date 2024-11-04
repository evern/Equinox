namespace CheckmateDX
{
    partial class frmITR_Select_Discipline
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmITR_Select_Discipline));
            this.cmbDiscipline = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblDiscipline = new DevExpress.XtraEditors.LabelControl();
            this.pnlDiscipline = new System.Windows.Forms.Panel();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.flpContent = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDiscipline.Properties)).BeginInit();
            this.pnlDiscipline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.flpContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbDiscipline
            // 
            this.cmbDiscipline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbDiscipline.Location = new System.Drawing.Point(110, 10);
            this.cmbDiscipline.Margin = new System.Windows.Forms.Padding(0);
            this.cmbDiscipline.Name = "cmbDiscipline";
            this.cmbDiscipline.Properties.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbDiscipline.Properties.Appearance.Options.UseFont = true;
            this.cmbDiscipline.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbDiscipline.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbDiscipline.Size = new System.Drawing.Size(218, 22);
            this.cmbDiscipline.TabIndex = 6;
            // 
            // lblDiscipline
            // 
            this.lblDiscipline.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiscipline.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDiscipline.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblDiscipline.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDiscipline.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblDiscipline.Location = new System.Drawing.Point(10, 10);
            this.lblDiscipline.Name = "lblDiscipline";
            this.lblDiscipline.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.lblDiscipline.Size = new System.Drawing.Size(100, 20);
            this.lblDiscipline.TabIndex = 5;
            this.lblDiscipline.Text = "Discipline";
            // 
            // pnlDiscipline
            // 
            this.pnlDiscipline.Controls.Add(this.cmbDiscipline);
            this.pnlDiscipline.Controls.Add(this.lblDiscipline);
            this.pnlDiscipline.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDiscipline.Location = new System.Drawing.Point(3, 13);
            this.pnlDiscipline.Name = "pnlDiscipline";
            this.pnlDiscipline.Padding = new System.Windows.Forms.Padding(10, 10, 50, 10);
            this.pnlDiscipline.Size = new System.Drawing.Size(378, 40);
            this.pnlDiscipline.TabIndex = 4;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(232, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 41);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.Location = new System.Drawing.Point(52, 2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 41);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&Ok";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Controls.Add(this.btnOk);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 67);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(50, 0, 50, 0);
            this.panelControl1.Size = new System.Drawing.Size(384, 45);
            this.panelControl1.TabIndex = 8;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.panelControl1);
            this.pnlMain.Controls.Add(this.flpContent);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(384, 112);
            this.pnlMain.TabIndex = 1;
            // 
            // flpContent
            // 
            this.flpContent.Controls.Add(this.pnlDiscipline);
            this.flpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpContent.Location = new System.Drawing.Point(0, 0);
            this.flpContent.Name = "flpContent";
            this.flpContent.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.flpContent.Size = new System.Drawing.Size(384, 112);
            this.flpContent.TabIndex = 1;
            // 
            // frmITR_Select_Discipline
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(384, 112);
            this.Controls.Add(this.pnlMain);
            this.Name = "frmITR_Select_Discipline";
            ((System.ComponentModel.ISupportInitialize)(this.cmbDiscipline.Properties)).EndInit();
            this.pnlDiscipline.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.flpContent.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.ComboBoxEdit cmbDiscipline;
        private DevExpress.XtraEditors.LabelControl lblDiscipline;
        private System.Windows.Forms.Panel pnlDiscipline;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.FlowLayoutPanel flpContent;
    }
}
