namespace CheckmateDX
{
    partial class frmTemplate_Toggle
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTemplate_Toggle));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.gridControlSchedule = new DevExpress.XtraGrid.GridControl();
            this.templateToggleBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridViewToggle = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.coltoggleTemplateGuid = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coltoggleName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coltoggleDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUpdatedDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUpdatedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDeletedDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDeletedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGUID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnImport = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSchedule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.templateToggleBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewToggle)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.gridControlSchedule);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 50);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(784, 496);
            this.panelControl1.TabIndex = 0;
            // 
            // gridControlSchedule
            // 
            this.gridControlSchedule.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControlSchedule.DataSource = this.templateToggleBindingSource;
            this.gridControlSchedule.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlSchedule.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(41, 19, 41, 19);
            this.gridControlSchedule.Location = new System.Drawing.Point(2, 2);
            this.gridControlSchedule.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.gridControlSchedule.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlSchedule.MainView = this.gridViewToggle;
            this.gridControlSchedule.Margin = new System.Windows.Forms.Padding(41, 19, 41, 19);
            this.gridControlSchedule.Name = "gridControlSchedule";
            this.gridControlSchedule.Size = new System.Drawing.Size(780, 492);
            this.gridControlSchedule.TabIndex = 14;
            this.gridControlSchedule.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewToggle});
            // 
            // templateToggleBindingSource
            // 
            this.templateToggleBindingSource.DataSource = typeof(ProjectLibrary.Template_Toggle);
            // 
            // gridViewToggle
            // 
            this.gridViewToggle.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.coltoggleTemplateGuid,
            this.coltoggleName,
            this.coltoggleDescription,
            this.colUpdatedDate,
            this.colUpdatedBy,
            this.colDeletedDate,
            this.colDeletedBy,
            this.colGUID,
            this.colCreatedDate,
            this.colCreatedBy});
            this.gridViewToggle.GridControl = this.gridControlSchedule;
            this.gridViewToggle.Name = "gridViewToggle";
            this.gridViewToggle.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewToggle.OptionsView.ShowGroupPanel = false;
            // 
            // coltoggleTemplateGuid
            // 
            this.coltoggleTemplateGuid.FieldName = "toggleTemplateGuid";
            this.coltoggleTemplateGuid.Name = "coltoggleTemplateGuid";
            this.coltoggleTemplateGuid.OptionsColumn.AllowEdit = false;
            // 
            // coltoggleName
            // 
            this.coltoggleName.Caption = "Name";
            this.coltoggleName.FieldName = "toggleName";
            this.coltoggleName.Name = "coltoggleName";
            this.coltoggleName.OptionsColumn.AllowEdit = false;
            this.coltoggleName.Visible = true;
            this.coltoggleName.VisibleIndex = 0;
            // 
            // coltoggleDescription
            // 
            this.coltoggleDescription.Caption = "Description";
            this.coltoggleDescription.FieldName = "toggleDescription";
            this.coltoggleDescription.Name = "coltoggleDescription";
            this.coltoggleDescription.OptionsColumn.AllowEdit = false;
            this.coltoggleDescription.Visible = true;
            this.coltoggleDescription.VisibleIndex = 1;
            // 
            // colUpdatedDate
            // 
            this.colUpdatedDate.FieldName = "UpdatedDate";
            this.colUpdatedDate.Name = "colUpdatedDate";
            this.colUpdatedDate.OptionsColumn.AllowEdit = false;
            // 
            // colUpdatedBy
            // 
            this.colUpdatedBy.FieldName = "UpdatedBy";
            this.colUpdatedBy.Name = "colUpdatedBy";
            this.colUpdatedBy.OptionsColumn.AllowEdit = false;
            // 
            // colDeletedDate
            // 
            this.colDeletedDate.FieldName = "DeletedDate";
            this.colDeletedDate.Name = "colDeletedDate";
            this.colDeletedDate.OptionsColumn.AllowEdit = false;
            // 
            // colDeletedBy
            // 
            this.colDeletedBy.FieldName = "DeletedBy";
            this.colDeletedBy.Name = "colDeletedBy";
            this.colDeletedBy.OptionsColumn.AllowEdit = false;
            // 
            // colGUID
            // 
            this.colGUID.FieldName = "GUID";
            this.colGUID.Name = "colGUID";
            this.colGUID.OptionsColumn.AllowEdit = false;
            // 
            // colCreatedDate
            // 
            this.colCreatedDate.FieldName = "CreatedDate";
            this.colCreatedDate.Name = "colCreatedDate";
            this.colCreatedDate.OptionsColumn.AllowEdit = false;
            this.colCreatedDate.Visible = true;
            this.colCreatedDate.VisibleIndex = 2;
            // 
            // colCreatedBy
            // 
            this.colCreatedBy.FieldName = "CreatedBy";
            this.colCreatedBy.Name = "colCreatedBy";
            this.colCreatedBy.OptionsColumn.AllowEdit = false;
            // 
            // btnImport
            // 
            this.btnImport.Caption = "Import";
            this.btnImport.Glyph = ((System.Drawing.Image)(resources.GetObject("btnImport.Glyph")));
            this.btnImport.Id = 3;
            this.btnImport.Name = "btnImport";
            this.btnImport.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "barButtonItem1";
            this.barButtonItem1.Id = 3;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmTemplate_Toggle
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.panelControl1);
            this.Name = "frmTemplate_Toggle";
            this.Text = "Template Toggle";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Controls.SetChildIndex(this.panelControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSchedule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.templateToggleBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewToggle)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl gridControlSchedule;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewToggle;
        private System.Windows.Forms.BindingSource templateToggleBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn coltoggleTemplateGuid;
        private DevExpress.XtraGrid.Columns.GridColumn coltoggleName;
        private DevExpress.XtraGrid.Columns.GridColumn coltoggleDescription;
        private DevExpress.XtraGrid.Columns.GridColumn colUpdatedDate;
        private DevExpress.XtraGrid.Columns.GridColumn colUpdatedBy;
        private DevExpress.XtraGrid.Columns.GridColumn colDeletedDate;
        private DevExpress.XtraGrid.Columns.GridColumn colDeletedBy;
        private DevExpress.XtraGrid.Columns.GridColumn colGUID;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedBy;
        private DevExpress.XtraBars.BarButtonItem btnImport;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private System.Windows.Forms.Timer timer1;
    }
}