namespace CheckmateDX
{
    partial class frmPrefill_Add
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrefill_Add));
            this.pnlName = new System.Windows.Forms.Panel();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.lblName = new DevExpress.XtraEditors.LabelControl();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.flpContent = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlCategory = new System.Windows.Forms.Panel();
            this.txtCategory = new DevExpress.XtraEditors.TextEdit();
            this.lblCategory = new DevExpress.XtraEditors.LabelControl();
            this.pnlName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.flpContent.SuspendLayout();
            this.pnlCategory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCategory.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlName
            // 
            this.pnlName.Controls.Add(this.txtName);
            this.pnlName.Controls.Add(this.lblName);
            this.pnlName.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlName.Location = new System.Drawing.Point(5, 22);
            this.pnlName.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.pnlName.Name = "pnlName";
            this.pnlName.Padding = new System.Windows.Forms.Padding(17, 17, 87, 17);
            this.pnlName.Size = new System.Drawing.Size(661, 70);
            this.pnlName.TabIndex = 0;
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(192, 17);
            this.txtName.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtName.Name = "txtName";
            this.txtName.Properties.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Properties.Appearance.Options.UseFont = true;
            this.txtName.Size = new System.Drawing.Size(382, 42);
            this.txtName.TabIndex = 1;
            // 
            // lblName
            // 
            this.lblName.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Appearance.Options.UseFont = true;
            this.lblName.Appearance.Options.UseTextOptions = true;
            this.lblName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblName.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblName.Location = new System.Drawing.Point(17, 17);
            this.lblName.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.lblName.Name = "lblName";
            this.lblName.Padding = new System.Windows.Forms.Padding(0, 0, 17, 0);
            this.lblName.Size = new System.Drawing.Size(175, 36);
            this.lblName.TabIndex = 5;
            this.lblName.Text = "Name";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.panelControl1);
            this.pnlMain.Controls.Add(this.flpContent);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(672, 282);
            this.pnlMain.TabIndex = 2;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Controls.Add(this.btnOk);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 203);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(87, 0, 87, 0);
            this.panelControl1.Size = new System.Drawing.Size(672, 79);
            this.panelControl1.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(407, 3);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(175, 73);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnOk.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.ImageOptions.Image")));
            this.btnOk.Location = new System.Drawing.Point(90, 3);
            this.btnOk.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(175, 73);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&Add";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // flpContent
            // 
            this.flpContent.Controls.Add(this.pnlName);
            this.flpContent.Controls.Add(this.pnlCategory);
            this.flpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpContent.Location = new System.Drawing.Point(0, 0);
            this.flpContent.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.flpContent.Name = "flpContent";
            this.flpContent.Padding = new System.Windows.Forms.Padding(0, 17, 0, 0);
            this.flpContent.Size = new System.Drawing.Size(672, 282);
            this.flpContent.TabIndex = 1;
            // 
            // pnlCategory
            // 
            this.pnlCategory.Controls.Add(this.txtCategory);
            this.pnlCategory.Controls.Add(this.lblCategory);
            this.pnlCategory.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCategory.Location = new System.Drawing.Point(5, 102);
            this.pnlCategory.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.pnlCategory.Name = "pnlCategory";
            this.pnlCategory.Padding = new System.Windows.Forms.Padding(17, 17, 87, 17);
            this.pnlCategory.Size = new System.Drawing.Size(661, 70);
            this.pnlCategory.TabIndex = 1;
            // 
            // txtCategory
            // 
            this.txtCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCategory.Location = new System.Drawing.Point(192, 17);
            this.txtCategory.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.txtCategory.Name = "txtCategory";
            this.txtCategory.Properties.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCategory.Properties.Appearance.Options.UseFont = true;
            this.txtCategory.Size = new System.Drawing.Size(382, 42);
            this.txtCategory.TabIndex = 1;
            // 
            // lblCategory
            // 
            this.lblCategory.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCategory.Appearance.Options.UseFont = true;
            this.lblCategory.Appearance.Options.UseTextOptions = true;
            this.lblCategory.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblCategory.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblCategory.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCategory.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblCategory.Location = new System.Drawing.Point(17, 17);
            this.lblCategory.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Padding = new System.Windows.Forms.Padding(0, 0, 17, 0);
            this.lblCategory.Size = new System.Drawing.Size(175, 36);
            this.lblCategory.TabIndex = 5;
            this.lblCategory.Text = "Category";
            // 
            // frmPrefill_Add
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
            this.ClientSize = new System.Drawing.Size(672, 282);
            this.Controls.Add(this.pnlMain);
            this.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.Name = "frmPrefill_Add";
            this.pnlName.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.flpContent.ResumeLayout(false);
            this.pnlCategory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCategory.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlName;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.LabelControl lblName;
        private System.Windows.Forms.Panel pnlMain;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private System.Windows.Forms.FlowLayoutPanel flpContent;
        private System.Windows.Forms.Panel pnlCategory;
        private DevExpress.XtraEditors.TextEdit txtCategory;
        private DevExpress.XtraEditors.LabelControl lblCategory;
    }
}
