namespace CheckmateDX
{
    partial class frmTemplate_Add
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTemplate_Add));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.flpContent = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlName = new System.Windows.Forms.Panel();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.lblName = new DevExpress.XtraEditors.LabelControl();
            this.pnlRevision = new System.Windows.Forms.Panel();
            this.txtRevision = new DevExpress.XtraEditors.TextEdit();
            this.lblRevision = new DevExpress.XtraEditors.LabelControl();
            this.pnlDescription = new System.Windows.Forms.Panel();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.pnlWorkflow = new System.Windows.Forms.Panel();
            this.cmbWorkflow = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblWorkflow = new DevExpress.XtraEditors.LabelControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkEditQR = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.panel3 = new System.Windows.Forms.Panel();
            this.checkEditSkipApproved = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmbCopyFrom = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.flpContent.SuspendLayout();
            this.pnlName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            this.pnlRevision.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRevision.Properties)).BeginInit();
            this.pnlDescription.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            this.pnlWorkflow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbWorkflow.Properties)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditQR.Properties)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditSkipApproved.Properties)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbCopyFrom.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.panelControl1);
            this.pnlMain.Controls.Add(this.flpContent);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(578, 806);
            this.pnlMain.TabIndex = 3;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Controls.Add(this.btnOk);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 738);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(75, 0, 75, 0);
            this.panelControl1.Size = new System.Drawing.Size(578, 68);
            this.panelControl1.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(351, 2);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(150, 64);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnOk.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.ImageOptions.Image")));
            this.btnOk.Location = new System.Drawing.Point(77, 2);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(150, 64);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&Add";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // flpContent
            // 
            this.flpContent.Controls.Add(this.pnlName);
            this.flpContent.Controls.Add(this.pnlRevision);
            this.flpContent.Controls.Add(this.pnlDescription);
            this.flpContent.Controls.Add(this.pnlWorkflow);
            this.flpContent.Controls.Add(this.panel1);
            this.flpContent.Controls.Add(this.panel3);
            this.flpContent.Controls.Add(this.panel2);
            this.flpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpContent.Location = new System.Drawing.Point(0, 0);
            this.flpContent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flpContent.Name = "flpContent";
            this.flpContent.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.flpContent.Size = new System.Drawing.Size(578, 806);
            this.flpContent.TabIndex = 1;
            // 
            // pnlName
            // 
            this.pnlName.Controls.Add(this.txtName);
            this.pnlName.Controls.Add(this.lblName);
            this.pnlName.Location = new System.Drawing.Point(4, 19);
            this.pnlName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlName.Name = "pnlName";
            this.pnlName.Padding = new System.Windows.Forms.Padding(15, 15, 75, 15);
            this.pnlName.Size = new System.Drawing.Size(567, 60);
            this.pnlName.TabIndex = 1;
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(165, 15);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(327, 26);
            this.txtName.TabIndex = 1;
            // 
            // lblName
            // 
            this.lblName.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Appearance.Options.UseFont = true;
            this.lblName.Appearance.Options.UseTextOptions = true;
            this.lblName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblName.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblName.Location = new System.Drawing.Point(15, 15);
            this.lblName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblName.Name = "lblName";
            this.lblName.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.lblName.Size = new System.Drawing.Size(150, 30);
            this.lblName.TabIndex = 5;
            this.lblName.Text = "Name";
            // 
            // pnlRevision
            // 
            this.pnlRevision.Controls.Add(this.txtRevision);
            this.pnlRevision.Controls.Add(this.lblRevision);
            this.pnlRevision.Location = new System.Drawing.Point(4, 87);
            this.pnlRevision.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlRevision.Name = "pnlRevision";
            this.pnlRevision.Padding = new System.Windows.Forms.Padding(15, 15, 75, 15);
            this.pnlRevision.Size = new System.Drawing.Size(567, 60);
            this.pnlRevision.TabIndex = 2;
            // 
            // txtRevision
            // 
            this.txtRevision.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRevision.Location = new System.Drawing.Point(165, 15);
            this.txtRevision.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtRevision.Name = "txtRevision";
            this.txtRevision.Size = new System.Drawing.Size(327, 26);
            this.txtRevision.TabIndex = 1;
            // 
            // lblRevision
            // 
            this.lblRevision.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRevision.Appearance.Options.UseFont = true;
            this.lblRevision.Appearance.Options.UseTextOptions = true;
            this.lblRevision.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblRevision.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblRevision.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblRevision.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblRevision.Location = new System.Drawing.Point(15, 15);
            this.lblRevision.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblRevision.Name = "lblRevision";
            this.lblRevision.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.lblRevision.Size = new System.Drawing.Size(150, 30);
            this.lblRevision.TabIndex = 5;
            this.lblRevision.Text = "Revision";
            // 
            // pnlDescription
            // 
            this.pnlDescription.Controls.Add(this.txtDescription);
            this.pnlDescription.Controls.Add(this.lblDescription);
            this.pnlDescription.Location = new System.Drawing.Point(4, 155);
            this.pnlDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlDescription.Name = "pnlDescription";
            this.pnlDescription.Padding = new System.Windows.Forms.Padding(15, 15, 75, 15);
            this.pnlDescription.Size = new System.Drawing.Size(567, 60);
            this.pnlDescription.TabIndex = 3;
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(165, 15);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(327, 26);
            this.txtDescription.TabIndex = 1;
            // 
            // lblDescription
            // 
            this.lblDescription.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Appearance.Options.UseFont = true;
            this.lblDescription.Appearance.Options.UseTextOptions = true;
            this.lblDescription.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblDescription.Location = new System.Drawing.Point(15, 15);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.lblDescription.Size = new System.Drawing.Size(150, 30);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description";
            // 
            // pnlWorkflow
            // 
            this.pnlWorkflow.Controls.Add(this.cmbWorkflow);
            this.pnlWorkflow.Controls.Add(this.lblWorkflow);
            this.pnlWorkflow.Location = new System.Drawing.Point(4, 223);
            this.pnlWorkflow.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlWorkflow.Name = "pnlWorkflow";
            this.pnlWorkflow.Padding = new System.Windows.Forms.Padding(15, 15, 75, 15);
            this.pnlWorkflow.Size = new System.Drawing.Size(567, 60);
            this.pnlWorkflow.TabIndex = 4;
            // 
            // cmbWorkflow
            // 
            this.cmbWorkflow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbWorkflow.Location = new System.Drawing.Point(165, 15);
            this.cmbWorkflow.Margin = new System.Windows.Forms.Padding(0);
            this.cmbWorkflow.Name = "cmbWorkflow";
            this.cmbWorkflow.Properties.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbWorkflow.Properties.Appearance.Options.UseFont = true;
            this.cmbWorkflow.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbWorkflow.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbWorkflow.Size = new System.Drawing.Size(327, 30);
            this.cmbWorkflow.TabIndex = 8;
            // 
            // lblWorkflow
            // 
            this.lblWorkflow.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWorkflow.Appearance.Options.UseFont = true;
            this.lblWorkflow.Appearance.Options.UseTextOptions = true;
            this.lblWorkflow.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblWorkflow.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblWorkflow.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblWorkflow.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblWorkflow.Location = new System.Drawing.Point(15, 15);
            this.lblWorkflow.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblWorkflow.Name = "lblWorkflow";
            this.lblWorkflow.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.lblWorkflow.Size = new System.Drawing.Size(150, 30);
            this.lblWorkflow.TabIndex = 5;
            this.lblWorkflow.Text = "Workflow";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkEditQR);
            this.panel1.Controls.Add(this.labelControl1);
            this.panel1.Location = new System.Drawing.Point(4, 291);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(15, 15, 75, 15);
            this.panel1.Size = new System.Drawing.Size(567, 60);
            this.panel1.TabIndex = 5;
            // 
            // checkEditQR
            // 
            this.checkEditQR.Location = new System.Drawing.Point(165, 15);
            this.checkEditQR.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkEditQR.Name = "checkEditQR";
            this.checkEditQR.Properties.Caption = "";
            this.checkEditQR.Size = new System.Drawing.Size(112, 19);
            this.checkEditQR.TabIndex = 6;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Appearance.Options.UseTextOptions = true;
            this.labelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.labelControl1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelControl1.Location = new System.Drawing.Point(15, 15);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.labelControl1.Size = new System.Drawing.Size(150, 30);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "QR Support";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.checkEditSkipApproved);
            this.panel3.Controls.Add(this.labelControl3);
            this.panel3.Location = new System.Drawing.Point(4, 359);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(15, 15, 75, 15);
            this.panel3.Size = new System.Drawing.Size(567, 60);
            this.panel3.TabIndex = 7;
            // 
            // checkEditSkipApproved
            // 
            this.checkEditSkipApproved.Location = new System.Drawing.Point(165, 15);
            this.checkEditSkipApproved.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkEditSkipApproved.Name = "checkEditSkipApproved";
            this.checkEditSkipApproved.Properties.Caption = "";
            this.checkEditSkipApproved.Size = new System.Drawing.Size(112, 19);
            this.checkEditSkipApproved.TabIndex = 6;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Appearance.Options.UseTextOptions = true;
            this.labelControl3.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.labelControl3.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.labelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl3.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelControl3.Location = new System.Drawing.Point(15, 15);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.labelControl3.Size = new System.Drawing.Size(150, 30);
            this.labelControl3.TabIndex = 5;
            this.labelControl3.Text = "Skip Approved";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cmbCopyFrom);
            this.panel2.Controls.Add(this.labelControl2);
            this.panel2.Location = new System.Drawing.Point(4, 427);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(15, 15, 75, 15);
            this.panel2.Size = new System.Drawing.Size(567, 60);
            this.panel2.TabIndex = 6;
            // 
            // cmbCopyFrom
            // 
            this.cmbCopyFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbCopyFrom.Location = new System.Drawing.Point(165, 15);
            this.cmbCopyFrom.Margin = new System.Windows.Forms.Padding(0);
            this.cmbCopyFrom.Name = "cmbCopyFrom";
            this.cmbCopyFrom.Properties.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCopyFrom.Properties.Appearance.Options.UseFont = true;
            this.cmbCopyFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbCopyFrom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbCopyFrom.Size = new System.Drawing.Size(327, 30);
            this.cmbCopyFrom.TabIndex = 8;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Appearance.Options.UseTextOptions = true;
            this.labelControl2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.labelControl2.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl2.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelControl2.Location = new System.Drawing.Point(15, 15);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.labelControl2.Size = new System.Drawing.Size(150, 30);
            this.labelControl2.TabIndex = 5;
            this.labelControl2.Text = "Copy From";
            // 
            // frmTemplate_Add
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.ClientSize = new System.Drawing.Size(578, 806);
            this.Controls.Add(this.pnlMain);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "frmTemplate_Add";
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.flpContent.ResumeLayout(false);
            this.pnlName.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            this.pnlRevision.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtRevision.Properties)).EndInit();
            this.pnlDescription.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            this.pnlWorkflow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbWorkflow.Properties)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkEditQR.Properties)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkEditSkipApproved.Properties)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbCopyFrom.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpContent;
        private System.Windows.Forms.Panel pnlName;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.LabelControl lblName;
        private System.Windows.Forms.Panel pnlMain;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private System.Windows.Forms.Panel pnlRevision;
        private DevExpress.XtraEditors.TextEdit txtRevision;
        private DevExpress.XtraEditors.LabelControl lblRevision;
        private System.Windows.Forms.Panel pnlDescription;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraEditors.LabelControl lblDescription;
        private System.Windows.Forms.Panel pnlWorkflow;
        private DevExpress.XtraEditors.LabelControl lblWorkflow;
        private DevExpress.XtraEditors.ComboBoxEdit cmbWorkflow;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.CheckEdit checkEditQR;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraEditors.ComboBoxEdit cmbCopyFrom;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private System.Windows.Forms.Panel panel3;
        private DevExpress.XtraEditors.CheckEdit checkEditSkipApproved;
        private DevExpress.XtraEditors.LabelControl labelControl3;
    }
}
