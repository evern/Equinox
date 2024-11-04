namespace CheckmateDX
{
    partial class frmSchedule_AddWBS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSchedule_AddWBS));
            this.flpContent = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlName = new System.Windows.Forms.Panel();
            this.txtName = new DevExpress.XtraEditors.TextEdit();
            this.lblName = new DevExpress.XtraEditors.LabelControl();
            this.pnlDescription = new System.Windows.Forms.Panel();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.pnlWBS = new System.Windows.Forms.Panel();
            this.treeListLookUpWBS = new DevExpress.XtraEditors.TreeListLookUpEdit();
            this.wbsTagDisplayBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.treeListLookUpEdit1TreeList = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.lblWBS = new DevExpress.XtraEditors.LabelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.flpContent.SuspendLayout();
            this.pnlName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).BeginInit();
            this.pnlDescription.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            this.pnlWBS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListLookUpWBS.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wbsTagDisplayBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeListLookUpEdit1TreeList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpContent
            // 
            this.flpContent.Controls.Add(this.pnlName);
            this.flpContent.Controls.Add(this.pnlDescription);
            this.flpContent.Controls.Add(this.pnlWBS);
            this.flpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpContent.Location = new System.Drawing.Point(0, 0);
            this.flpContent.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flpContent.Name = "flpContent";
            this.flpContent.Padding = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.flpContent.Size = new System.Drawing.Size(480, 265);
            this.flpContent.TabIndex = 1;
            // 
            // pnlName
            // 
            this.pnlName.Controls.Add(this.txtName);
            this.pnlName.Controls.Add(this.lblName);
            this.pnlName.Location = new System.Drawing.Point(4, 16);
            this.pnlName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlName.Name = "pnlName";
            this.pnlName.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.pnlName.Size = new System.Drawing.Size(472, 50);
            this.pnlName.TabIndex = 1;
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(137, 12);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(273, 22);
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
            this.lblName.Location = new System.Drawing.Point(12, 12);
            this.lblName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.pnlDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlDescription.Name = "pnlDescription";
            this.pnlDescription.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.pnlDescription.Size = new System.Drawing.Size(472, 50);
            this.pnlDescription.TabIndex = 3;
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(137, 12);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(273, 22);
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
            this.lblDescription.Location = new System.Drawing.Point(12, 12);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblDescription.Size = new System.Drawing.Size(125, 26);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description";
            // 
            // pnlWBS
            // 
            this.pnlWBS.Controls.Add(this.treeListLookUpWBS);
            this.pnlWBS.Controls.Add(this.lblWBS);
            this.pnlWBS.Location = new System.Drawing.Point(4, 132);
            this.pnlWBS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlWBS.Name = "pnlWBS";
            this.pnlWBS.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.pnlWBS.Size = new System.Drawing.Size(472, 50);
            this.pnlWBS.TabIndex = 6;
            // 
            // treeListLookUpWBS
            // 
            this.treeListLookUpWBS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListLookUpWBS.EditValue = "TEST";
            this.treeListLookUpWBS.Location = new System.Drawing.Point(137, 12);
            this.treeListLookUpWBS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.treeListLookUpWBS.Name = "treeListLookUpWBS";
            this.treeListLookUpWBS.Properties.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeListLookUpWBS.Properties.Appearance.Options.UseFont = true;
            this.treeListLookUpWBS.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.treeListLookUpWBS.Properties.DataSource = this.wbsTagDisplayBindingSource;
            this.treeListLookUpWBS.Properties.DisplayMember = "wbsTagDisplayName";
            this.treeListLookUpWBS.Properties.NullText = "Select WBS...";
            this.treeListLookUpWBS.Properties.TreeList = this.treeListLookUpEdit1TreeList;
            this.treeListLookUpWBS.Properties.ValueMember = "wbsTagDisplayGuid";
            this.treeListLookUpWBS.Size = new System.Drawing.Size(273, 26);
            this.treeListLookUpWBS.TabIndex = 6;
            // 
            // wbsTagDisplayBindingSource
            // 
            this.wbsTagDisplayBindingSource.DataSource = typeof(ProjectLibrary.wbsTagDisplay);
            // 
            // treeListLookUpEdit1TreeList
            // 
            this.treeListLookUpEdit1TreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1,
            this.treeListColumn2});
            this.treeListLookUpEdit1TreeList.DataSource = this.wbsTagDisplayBindingSource;
            this.treeListLookUpEdit1TreeList.KeyFieldName = "wbsTagDisplayGuid";
            this.treeListLookUpEdit1TreeList.Location = new System.Drawing.Point(-8, 6);
            this.treeListLookUpEdit1TreeList.Name = "treeListLookUpEdit1TreeList";
            this.treeListLookUpEdit1TreeList.OptionsView.ShowIndentAsRowStyle = true;
            this.treeListLookUpEdit1TreeList.ParentFieldName = "wbsTagDisplayParentGuid";
            this.treeListLookUpEdit1TreeList.Size = new System.Drawing.Size(400, 200);
            this.treeListLookUpEdit1TreeList.TabIndex = 0;
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "Name";
            this.treeListColumn1.FieldName = "wbsTagDisplayName";
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.OptionsColumn.AllowEdit = false;
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            this.treeListColumn1.Width = 120;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.Caption = "Description";
            this.treeListColumn2.FieldName = "wbsTagDisplayDescription";
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.OptionsColumn.AllowEdit = false;
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 1;
            this.treeListColumn2.Width = 150;
            // 
            // lblWBS
            // 
            this.lblWBS.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWBS.Appearance.Options.UseFont = true;
            this.lblWBS.Appearance.Options.UseTextOptions = true;
            this.lblWBS.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblWBS.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblWBS.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblWBS.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblWBS.Location = new System.Drawing.Point(12, 12);
            this.lblWBS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lblWBS.Name = "lblWBS";
            this.lblWBS.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblWBS.Size = new System.Drawing.Size(125, 26);
            this.lblWBS.TabIndex = 5;
            this.lblWBS.Text = "WBS";
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(291, 2);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(125, 52);
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
            this.btnOk.Location = new System.Drawing.Point(64, 2);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(125, 52);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&Add";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Controls.Add(this.btnOk);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 209);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(62, 0, 62, 0);
            this.panelControl1.Size = new System.Drawing.Size(480, 56);
            this.panelControl1.TabIndex = 3;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.panelControl1);
            this.pnlMain.Controls.Add(this.flpContent);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(480, 265);
            this.pnlMain.TabIndex = 5;
            // 
            // frmSchedule_AddWBS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.ClientSize = new System.Drawing.Size(480, 265);
            this.Controls.Add(this.pnlMain);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "frmSchedule_AddWBS";
            this.Text = "Add WBS";
            this.flpContent.ResumeLayout(false);
            this.pnlName.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtName.Properties)).EndInit();
            this.pnlDescription.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            this.pnlWBS.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListLookUpWBS.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wbsTagDisplayBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeListLookUpEdit1TreeList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpContent;
        private System.Windows.Forms.Panel pnlName;
        private DevExpress.XtraEditors.TextEdit txtName;
        private DevExpress.XtraEditors.LabelControl lblName;
        private System.Windows.Forms.Panel pnlDescription;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraEditors.LabelControl lblDescription;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlWBS;
        private DevExpress.XtraEditors.TreeListLookUpEdit treeListLookUpWBS;
        private DevExpress.XtraTreeList.TreeList treeListLookUpEdit1TreeList;
        private DevExpress.XtraEditors.LabelControl lblWBS;
        private System.Windows.Forms.BindingSource wbsTagDisplayBindingSource;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
    }
}
