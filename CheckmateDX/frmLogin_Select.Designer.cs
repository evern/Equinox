namespace CheckmateDX
{
    partial class frmLogin_Select
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin_Select));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlContent = new DevExpress.XtraEditors.PanelControl();
            this.pnlNumber = new System.Windows.Forms.Panel();
            this.cmbDiscipline = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblNumber = new DevExpress.XtraEditors.LabelControl();
            this.pnlName = new System.Windows.Forms.Panel();
            this.cmbProject = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblName = new DevExpress.XtraEditors.LabelControl();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlContent)).BeginInit();
            this.pnlContent.SuspendLayout();
            this.pnlNumber.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDiscipline.Properties)).BeginInit();
            this.pnlName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbProject.Properties)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.pnlContent);
            this.pnlMain.Controls.Add(this.pnlButtons);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(5);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(576, 241);
            this.pnlMain.TabIndex = 0;
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.pnlNumber);
            this.pnlContent.Controls.Add(this.pnlName);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 0);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(5);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(0, 14, 0, 0);
            this.pnlContent.Size = new System.Drawing.Size(576, 167);
            this.pnlContent.TabIndex = 1;
            // 
            // pnlNumber
            // 
            this.pnlNumber.Controls.Add(this.cmbDiscipline);
            this.pnlNumber.Controls.Add(this.lblNumber);
            this.pnlNumber.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlNumber.Location = new System.Drawing.Point(2, 76);
            this.pnlNumber.Margin = new System.Windows.Forms.Padding(5);
            this.pnlNumber.Name = "pnlNumber";
            this.pnlNumber.Padding = new System.Windows.Forms.Padding(14, 14, 74, 14);
            this.pnlNumber.Size = new System.Drawing.Size(572, 60);
            this.pnlNumber.TabIndex = 2;
            // 
            // cmbDiscipline
            // 
            this.cmbDiscipline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbDiscipline.Location = new System.Drawing.Point(164, 14);
            this.cmbDiscipline.Margin = new System.Windows.Forms.Padding(0);
            this.cmbDiscipline.Name = "cmbDiscipline";
            this.cmbDiscipline.Properties.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbDiscipline.Properties.Appearance.Options.UseFont = true;
            this.cmbDiscipline.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbDiscipline.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbDiscipline.Size = new System.Drawing.Size(334, 30);
            this.cmbDiscipline.TabIndex = 1;
            // 
            // lblNumber
            // 
            this.lblNumber.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumber.Appearance.Options.UseFont = true;
            this.lblNumber.Appearance.Options.UseTextOptions = true;
            this.lblNumber.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblNumber.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblNumber.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNumber.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblNumber.Location = new System.Drawing.Point(14, 14);
            this.lblNumber.Margin = new System.Windows.Forms.Padding(5);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Padding = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.lblNumber.Size = new System.Drawing.Size(150, 32);
            this.lblNumber.TabIndex = 5;
            this.lblNumber.Text = "Discipline";
            // 
            // pnlName
            // 
            this.pnlName.Controls.Add(this.cmbProject);
            this.pnlName.Controls.Add(this.lblName);
            this.pnlName.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlName.Location = new System.Drawing.Point(2, 16);
            this.pnlName.Margin = new System.Windows.Forms.Padding(5);
            this.pnlName.Name = "pnlName";
            this.pnlName.Padding = new System.Windows.Forms.Padding(14, 14, 74, 14);
            this.pnlName.Size = new System.Drawing.Size(572, 60);
            this.pnlName.TabIndex = 1;
            // 
            // cmbProject
            // 
            this.cmbProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbProject.Location = new System.Drawing.Point(164, 14);
            this.cmbProject.Margin = new System.Windows.Forms.Padding(0);
            this.cmbProject.Name = "cmbProject";
            this.cmbProject.Properties.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbProject.Properties.Appearance.Options.UseFont = true;
            this.cmbProject.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbProject.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbProject.Size = new System.Drawing.Size(334, 30);
            this.cmbProject.TabIndex = 1;
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
            this.lblName.Location = new System.Drawing.Point(14, 14);
            this.lblName.Margin = new System.Windows.Forms.Padding(5);
            this.lblName.Name = "lblName";
            this.lblName.Padding = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.lblName.Size = new System.Drawing.Size(150, 32);
            this.lblName.TabIndex = 5;
            this.lblName.Text = "Project";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnOk);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 167);
            this.pnlButtons.Margin = new System.Windows.Forms.Padding(5);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(74, 0, 74, 0);
            this.pnlButtons.Size = new System.Drawing.Size(576, 74);
            this.pnlButtons.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(352, 0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(150, 74);
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
            this.btnOk.Location = new System.Drawing.Point(74, 0);
            this.btnOk.Margin = new System.Windows.Forms.Padding(5);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(150, 74);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&Accept";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // frmLogin_Select
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.ClientSize = new System.Drawing.Size(576, 241);
            this.Controls.Add(this.pnlMain);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmLogin_Select";
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlContent)).EndInit();
            this.pnlContent.ResumeLayout(false);
            this.pnlNumber.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbDiscipline.Properties)).EndInit();
            this.pnlName.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbProject.Properties)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private DevExpress.XtraEditors.PanelControl pnlContent;
        private System.Windows.Forms.Panel pnlButtons;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private System.Windows.Forms.Panel pnlNumber;
        private DevExpress.XtraEditors.LabelControl lblNumber;
        private System.Windows.Forms.Panel pnlName;
        private DevExpress.XtraEditors.LabelControl lblName;
        private DevExpress.XtraEditors.ComboBoxEdit cmbDiscipline;
        private DevExpress.XtraEditors.ComboBoxEdit cmbProject;
    }
}
