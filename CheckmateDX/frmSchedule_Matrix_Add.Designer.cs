namespace CheckmateDX
{
    partial class frmSchedule_Matrix_Add
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSchedule_Matrix_Add));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.flpContent = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlName = new System.Windows.Forms.Panel();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.lblName = new DevExpress.XtraEditors.LabelControl();
            this.pnlDescription = new System.Windows.Forms.Panel();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.pnlCategory = new System.Windows.Forms.Panel();
            this.txtCategory = new DevExpress.XtraEditors.TextEdit();
            this.lblCategory = new DevExpress.XtraEditors.LabelControl();
            this.pnlDiscipline = new System.Windows.Forms.Panel();
            this.cmbDiscipline = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lblDiscipline = new DevExpress.XtraEditors.LabelControl();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.flpContent.SuspendLayout();
            this.pnlName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            this.pnlDescription.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            this.pnlCategory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCategory.Properties)).BeginInit();
            this.pnlDiscipline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDiscipline.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.panelControl1);
            this.pnlMain.Controls.Add(this.flpContent);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(4);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(480, 311);
            this.pnlMain.TabIndex = 5;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Controls.Add(this.btnOk);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 255);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(62, 0, 62, 0);
            this.panelControl1.Size = new System.Drawing.Size(480, 56);
            this.panelControl1.TabIndex = 5;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(291, 2);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(125, 52);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancel";
            // 
            // btnOk
            // 
            this.btnOk.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnOk.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.ImageOptions.Image")));
            this.btnOk.Location = new System.Drawing.Point(64, 2);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(125, 52);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "&Add";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // flpContent
            // 
            this.flpContent.Controls.Add(this.pnlName);
            this.flpContent.Controls.Add(this.pnlDescription);
            this.flpContent.Controls.Add(this.pnlCategory);
            this.flpContent.Controls.Add(this.pnlDiscipline);
            this.flpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpContent.Location = new System.Drawing.Point(0, 0);
            this.flpContent.Margin = new System.Windows.Forms.Padding(4);
            this.flpContent.Name = "flpContent";
            this.flpContent.Padding = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.flpContent.Size = new System.Drawing.Size(480, 311);
            this.flpContent.TabIndex = 1;
            // 
            // pnlName
            // 
            this.pnlName.Controls.Add(this.txtName);
            this.pnlName.Controls.Add(this.lblName);
            this.pnlName.Location = new System.Drawing.Point(4, 16);
            this.pnlName.Margin = new System.Windows.Forms.Padding(4);
            this.pnlName.Name = "pnlName";
            this.pnlName.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.pnlName.Size = new System.Drawing.Size(472, 50);
            this.pnlName.TabIndex = 1;
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(137, 12);
            this.txtName.Margin = new System.Windows.Forms.Padding(4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(273, 22);
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
            this.lblName.Location = new System.Drawing.Point(12, 12);
            this.lblName.Margin = new System.Windows.Forms.Padding(4);
            this.lblName.Name = "lblName";
            this.lblName.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblName.Size = new System.Drawing.Size(125, 26);
            this.lblName.TabIndex = 5;
            this.lblName.Text = "Name";
            // 
            // pnlDescription
            // 
            this.pnlDescription.Controls.Add(this.txtDescription);
            this.pnlDescription.Controls.Add(this.lblDescription);
            this.pnlDescription.Location = new System.Drawing.Point(4, 74);
            this.pnlDescription.Margin = new System.Windows.Forms.Padding(4);
            this.pnlDescription.Name = "pnlDescription";
            this.pnlDescription.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.pnlDescription.Size = new System.Drawing.Size(472, 50);
            this.pnlDescription.TabIndex = 2;
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(137, 12);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(4);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(273, 22);
            this.txtDescription.TabIndex = 1;
            // 
            // lblDescription
            // 
            this.lblDescription.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Appearance.Options.UseFont = true;
            this.lblDescription.Appearance.Options.UseTextOptions = true;
            this.lblDescription.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblDescription.Location = new System.Drawing.Point(12, 12);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(4);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblDescription.Size = new System.Drawing.Size(125, 26);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description";
            // 
            // pnlCategory
            // 
            this.pnlCategory.Controls.Add(this.txtCategory);
            this.pnlCategory.Controls.Add(this.lblCategory);
            this.pnlCategory.Location = new System.Drawing.Point(4, 132);
            this.pnlCategory.Margin = new System.Windows.Forms.Padding(4);
            this.pnlCategory.Name = "pnlCategory";
            this.pnlCategory.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.pnlCategory.Size = new System.Drawing.Size(472, 50);
            this.pnlCategory.TabIndex = 3;
            // 
            // txtCategory
            // 
            this.txtCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCategory.Location = new System.Drawing.Point(137, 12);
            this.txtCategory.Margin = new System.Windows.Forms.Padding(4);
            this.txtCategory.Name = "txtCategory";
            this.txtCategory.Size = new System.Drawing.Size(273, 22);
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
            this.lblCategory.Location = new System.Drawing.Point(12, 12);
            this.lblCategory.Margin = new System.Windows.Forms.Padding(4);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblCategory.Size = new System.Drawing.Size(125, 26);
            this.lblCategory.TabIndex = 5;
            this.lblCategory.Text = "Category";
            // 
            // pnlDiscipline
            // 
            this.pnlDiscipline.Controls.Add(this.cmbDiscipline);
            this.pnlDiscipline.Controls.Add(this.lblDiscipline);
            this.pnlDiscipline.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDiscipline.Location = new System.Drawing.Point(4, 190);
            this.pnlDiscipline.Margin = new System.Windows.Forms.Padding(4);
            this.pnlDiscipline.Name = "pnlDiscipline";
            this.pnlDiscipline.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.pnlDiscipline.Size = new System.Drawing.Size(472, 50);
            this.pnlDiscipline.TabIndex = 4;
            // 
            // cmbDiscipline
            // 
            this.cmbDiscipline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbDiscipline.Location = new System.Drawing.Point(137, 12);
            this.cmbDiscipline.Margin = new System.Windows.Forms.Padding(0);
            this.cmbDiscipline.Name = "cmbDiscipline";
            this.cmbDiscipline.Properties.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbDiscipline.Properties.Appearance.Options.UseFont = true;
            this.cmbDiscipline.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbDiscipline.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbDiscipline.Size = new System.Drawing.Size(273, 26);
            this.cmbDiscipline.TabIndex = 6;
            // 
            // lblDiscipline
            // 
            this.lblDiscipline.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDiscipline.Appearance.Options.UseFont = true;
            this.lblDiscipline.Appearance.Options.UseTextOptions = true;
            this.lblDiscipline.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDiscipline.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblDiscipline.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDiscipline.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblDiscipline.Location = new System.Drawing.Point(12, 12);
            this.lblDiscipline.Margin = new System.Windows.Forms.Padding(4);
            this.lblDiscipline.Name = "lblDiscipline";
            this.lblDiscipline.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblDiscipline.Size = new System.Drawing.Size(125, 26);
            this.lblDiscipline.TabIndex = 5;
            this.lblDiscipline.Text = "Discipline";
            // 
            // frmSchedule_Matrix_Add
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.ClientSize = new System.Drawing.Size(480, 311);
            this.Controls.Add(this.pnlMain);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "frmSchedule_Matrix_Add";
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.flpContent.ResumeLayout(false);
            this.pnlName.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            this.pnlDescription.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            this.pnlCategory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCategory.Properties)).EndInit();
            this.pnlDiscipline.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbDiscipline.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlDescription;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraEditors.LabelControl lblDescription;
        private System.Windows.Forms.Panel pnlName;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.LabelControl lblName;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.FlowLayoutPanel flpContent;
        private System.Windows.Forms.Panel pnlDiscipline;
        private DevExpress.XtraEditors.ComboBoxEdit cmbDiscipline;
        private DevExpress.XtraEditors.LabelControl lblDiscipline;
        private System.Windows.Forms.Panel pnlCategory;
        private DevExpress.XtraEditors.TextEdit txtCategory;
        private DevExpress.XtraEditors.LabelControl lblCategory;
    }
}
