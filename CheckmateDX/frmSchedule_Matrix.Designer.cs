namespace CheckmateDX
{
    partial class frmSchedule_Matrix
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSchedule_Matrix));
            DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager2 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, null, true, true);
            this.tblLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnlTypeMain = new DevExpress.XtraEditors.PanelControl();
            this.gridControlType = new DevExpress.XtraGrid.GridControl();
            this.matrixTypeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridViewType = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.coltypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coltypeDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coltypeCategory = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coltypeDiscipline = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGUID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.flowLayoutPanelSchedule = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddType = new DevExpress.XtraEditors.SimpleButton();
            this.btnEditType = new DevExpress.XtraEditors.SimpleButton();
            this.btnDeleteType = new DevExpress.XtraEditors.SimpleButton();
            this.btnReport = new DevExpress.XtraEditors.SimpleButton();
            this.pnlTemplate = new DevExpress.XtraEditors.PanelControl();
            this.gridControlTemplate = new DevExpress.XtraGrid.GridControl();
            this.templateCheckBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridViewTemplate = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.coltemplateSelected = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.coltemplateWorkFlow = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coltemplateName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coltemplateRevision = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coltemplateDiscipline = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coltemplateDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTemplateQRSupport = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGUID1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedBy1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.btnTrimDuplicates = new DevExpress.XtraEditors.SimpleButton();
            this.btnCopyFrom = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.cmbProject = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.WaitForm1), true, true);
            this.tblLayoutMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlTypeMain)).BeginInit();
            this.pnlTypeMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixTypeBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewType)).BeginInit();
            this.flowLayoutPanelSchedule.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlTemplate)).BeginInit();
            this.pnlTemplate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.templateCheckBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbProject.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tblLayoutMain
            // 
            this.tblLayoutMain.ColumnCount = 2;
            this.tblLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayoutMain.Controls.Add(this.pnlTypeMain, 0, 0);
            this.tblLayoutMain.Controls.Add(this.pnlTemplate, 1, 0);
            this.tblLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLayoutMain.Location = new System.Drawing.Point(0, 0);
            this.tblLayoutMain.Margin = new System.Windows.Forms.Padding(4);
            this.tblLayoutMain.Name = "tblLayoutMain";
            this.tblLayoutMain.RowCount = 1;
            this.tblLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tblLayoutMain.Size = new System.Drawing.Size(1372, 970);
            this.tblLayoutMain.TabIndex = 0;
            // 
            // pnlTypeMain
            // 
            this.pnlTypeMain.Controls.Add(this.gridControlType);
            this.pnlTypeMain.Controls.Add(this.flowLayoutPanelSchedule);
            this.pnlTypeMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTypeMain.Location = new System.Drawing.Point(4, 4);
            this.pnlTypeMain.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.pnlTypeMain.LookAndFeel.UseDefaultLookAndFeel = false;
            this.pnlTypeMain.Margin = new System.Windows.Forms.Padding(4);
            this.pnlTypeMain.Name = "pnlTypeMain";
            this.pnlTypeMain.Size = new System.Drawing.Size(678, 962);
            this.pnlTypeMain.TabIndex = 4;
            // 
            // gridControlType
            // 
            this.gridControlType.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControlType.DataSource = this.matrixTypeBindingSource;
            this.gridControlType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlType.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.gridControlType.Location = new System.Drawing.Point(2, 77);
            this.gridControlType.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.gridControlType.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlType.MainView = this.gridViewType;
            this.gridControlType.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.gridControlType.Name = "gridControlType";
            this.gridControlType.Size = new System.Drawing.Size(674, 883);
            this.gridControlType.TabIndex = 13;
            this.gridControlType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewType});
            // 
            // matrixTypeBindingSource
            // 
            this.matrixTypeBindingSource.DataSource = typeof(ProjectLibrary.MatrixType);
            // 
            // gridViewType
            // 
            this.gridViewType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.coltypeName,
            this.coltypeDescription,
            this.coltypeCategory,
            this.coltypeDiscipline,
            this.colGUID,
            this.colCreatedDate,
            this.colCreatedBy});
            this.gridViewType.DetailHeight = 525;
            this.gridViewType.FixedLineWidth = 3;
            this.gridViewType.GridControl = this.gridControlType;
            this.gridViewType.Name = "gridViewType";
            this.gridViewType.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridViewType.OptionsSelection.MultiSelect = true;
            this.gridViewType.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridViewType_MouseUp);
            // 
            // coltypeName
            // 
            this.coltypeName.Caption = "Name";
            this.coltypeName.FieldName = "typeName";
            this.coltypeName.MinWidth = 30;
            this.coltypeName.Name = "coltypeName";
            this.coltypeName.OptionsColumn.AllowEdit = false;
            this.coltypeName.Visible = true;
            this.coltypeName.VisibleIndex = 2;
            this.coltypeName.Width = 112;
            // 
            // coltypeDescription
            // 
            this.coltypeDescription.Caption = "Description";
            this.coltypeDescription.FieldName = "typeDescription";
            this.coltypeDescription.MinWidth = 30;
            this.coltypeDescription.Name = "coltypeDescription";
            this.coltypeDescription.OptionsColumn.AllowEdit = false;
            this.coltypeDescription.Visible = true;
            this.coltypeDescription.VisibleIndex = 3;
            this.coltypeDescription.Width = 112;
            // 
            // coltypeCategory
            // 
            this.coltypeCategory.Caption = "Category";
            this.coltypeCategory.FieldName = "typeCategory";
            this.coltypeCategory.MinWidth = 30;
            this.coltypeCategory.Name = "coltypeCategory";
            this.coltypeCategory.OptionsColumn.AllowEdit = false;
            this.coltypeCategory.Visible = true;
            this.coltypeCategory.VisibleIndex = 0;
            this.coltypeCategory.Width = 112;
            // 
            // coltypeDiscipline
            // 
            this.coltypeDiscipline.Caption = "Discipline";
            this.coltypeDiscipline.FieldName = "typeDiscipline";
            this.coltypeDiscipline.MinWidth = 30;
            this.coltypeDiscipline.Name = "coltypeDiscipline";
            this.coltypeDiscipline.OptionsColumn.AllowEdit = false;
            this.coltypeDiscipline.Visible = true;
            this.coltypeDiscipline.VisibleIndex = 1;
            this.coltypeDiscipline.Width = 112;
            // 
            // colGUID
            // 
            this.colGUID.FieldName = "GUID";
            this.colGUID.MinWidth = 30;
            this.colGUID.Name = "colGUID";
            this.colGUID.OptionsColumn.AllowEdit = false;
            this.colGUID.Width = 112;
            // 
            // colCreatedDate
            // 
            this.colCreatedDate.FieldName = "CreatedDate";
            this.colCreatedDate.MinWidth = 30;
            this.colCreatedDate.Name = "colCreatedDate";
            this.colCreatedDate.OptionsColumn.AllowEdit = false;
            this.colCreatedDate.Width = 112;
            // 
            // colCreatedBy
            // 
            this.colCreatedBy.FieldName = "CreatedBy";
            this.colCreatedBy.MinWidth = 30;
            this.colCreatedBy.Name = "colCreatedBy";
            this.colCreatedBy.OptionsColumn.AllowEdit = false;
            this.colCreatedBy.Width = 112;
            // 
            // flowLayoutPanelSchedule
            // 
            this.flowLayoutPanelSchedule.Controls.Add(this.btnAddType);
            this.flowLayoutPanelSchedule.Controls.Add(this.btnEditType);
            this.flowLayoutPanelSchedule.Controls.Add(this.btnDeleteType);
            this.flowLayoutPanelSchedule.Controls.Add(this.btnReport);
            this.flowLayoutPanelSchedule.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanelSchedule.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flowLayoutPanelSchedule.Location = new System.Drawing.Point(2, 2);
            this.flowLayoutPanelSchedule.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanelSchedule.Name = "flowLayoutPanelSchedule";
            this.flowLayoutPanelSchedule.Padding = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanelSchedule.Size = new System.Drawing.Size(674, 75);
            this.flowLayoutPanelSchedule.TabIndex = 2;
            // 
            // btnAddType
            // 
            this.btnAddType.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddType.Appearance.Options.UseFont = true;
            this.btnAddType.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAddType.ImageOptions.Image")));
            this.btnAddType.Location = new System.Drawing.Point(8, 8);
            this.btnAddType.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnAddType.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnAddType.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddType.Name = "btnAddType";
            this.btnAddType.Size = new System.Drawing.Size(150, 60);
            this.btnAddType.TabIndex = 0;
            this.btnAddType.Text = "Add";
            this.btnAddType.Click += new System.EventHandler(this.btnAddType_Click);
            // 
            // btnEditType
            // 
            this.btnEditType.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditType.Appearance.Options.UseFont = true;
            this.btnEditType.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditType.ImageOptions.Image")));
            this.btnEditType.Location = new System.Drawing.Point(166, 8);
            this.btnEditType.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnEditType.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnEditType.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditType.Name = "btnEditType";
            this.btnEditType.Padding = new System.Windows.Forms.Padding(4);
            this.btnEditType.Size = new System.Drawing.Size(150, 60);
            this.btnEditType.TabIndex = 1;
            this.btnEditType.Text = "Edit";
            this.btnEditType.Click += new System.EventHandler(this.btnEditType_Click);
            // 
            // btnDeleteType
            // 
            this.btnDeleteType.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteType.Appearance.Options.UseFont = true;
            this.btnDeleteType.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteType.ImageOptions.Image")));
            this.btnDeleteType.Location = new System.Drawing.Point(324, 8);
            this.btnDeleteType.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnDeleteType.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnDeleteType.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteType.Name = "btnDeleteType";
            this.btnDeleteType.Padding = new System.Windows.Forms.Padding(4);
            this.btnDeleteType.Size = new System.Drawing.Size(150, 60);
            this.btnDeleteType.TabIndex = 2;
            this.btnDeleteType.Text = "Delete";
            this.btnDeleteType.Click += new System.EventHandler(this.btnDeleteType_Click);
            // 
            // btnReport
            // 
            this.btnReport.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReport.Appearance.Options.UseFont = true;
            this.btnReport.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnReport.ImageOptions.Image")));
            this.btnReport.Location = new System.Drawing.Point(482, 8);
            this.btnReport.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnReport.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnReport.Name = "btnReport";
            this.btnReport.Padding = new System.Windows.Forms.Padding(4);
            this.btnReport.Size = new System.Drawing.Size(150, 60);
            this.btnReport.TabIndex = 3;
            this.btnReport.Text = "Report";
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // pnlTemplate
            // 
            this.pnlTemplate.Controls.Add(this.gridControlTemplate);
            this.pnlTemplate.Controls.Add(this.panelControl2);
            this.pnlTemplate.Controls.Add(this.panelControl1);
            this.pnlTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTemplate.Location = new System.Drawing.Point(690, 4);
            this.pnlTemplate.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.pnlTemplate.LookAndFeel.UseDefaultLookAndFeel = false;
            this.pnlTemplate.Margin = new System.Windows.Forms.Padding(4);
            this.pnlTemplate.Name = "pnlTemplate";
            this.pnlTemplate.Size = new System.Drawing.Size(678, 962);
            this.pnlTemplate.TabIndex = 5;
            // 
            // gridControlTemplate
            // 
            this.gridControlTemplate.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControlTemplate.DataSource = this.templateCheckBindingSource;
            this.gridControlTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlTemplate.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.gridControlTemplate.Location = new System.Drawing.Point(2, 82);
            this.gridControlTemplate.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.gridControlTemplate.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlTemplate.MainView = this.gridViewTemplate;
            this.gridControlTemplate.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.gridControlTemplate.Name = "gridControlTemplate";
            this.gridControlTemplate.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gridControlTemplate.Size = new System.Drawing.Size(674, 878);
            this.gridControlTemplate.TabIndex = 14;
            this.gridControlTemplate.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewTemplate});
            // 
            // templateCheckBindingSource
            // 
            this.templateCheckBindingSource.DataSource = typeof(ProjectLibrary.Template_Check);
            // 
            // gridViewTemplate
            // 
            this.gridViewTemplate.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.coltemplateSelected,
            this.coltemplateWorkFlow,
            this.coltemplateName,
            this.coltemplateRevision,
            this.coltemplateDiscipline,
            this.coltemplateDescription,
            this.colTemplateQRSupport,
            this.colGUID1,
            this.colCreatedDate1,
            this.colCreatedBy1});
            this.gridViewTemplate.DetailHeight = 525;
            this.gridViewTemplate.FixedLineWidth = 3;
            this.gridViewTemplate.GridControl = this.gridControlTemplate;
            this.gridViewTemplate.Name = "gridViewTemplate";
            this.gridViewTemplate.OptionsFind.AlwaysVisible = true;
            this.gridViewTemplate.OptionsSelection.EnableAppearanceFocusedCell = false;
            // 
            // coltemplateSelected
            // 
            this.coltemplateSelected.Caption = "Assign";
            this.coltemplateSelected.ColumnEdit = this.repositoryItemCheckEdit1;
            this.coltemplateSelected.FieldName = "templateSelected";
            this.coltemplateSelected.MinWidth = 30;
            this.coltemplateSelected.Name = "coltemplateSelected";
            this.coltemplateSelected.Visible = true;
            this.coltemplateSelected.VisibleIndex = 5;
            this.coltemplateSelected.Width = 112;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.repositoryItemCheckEdit1_EditValueChanging);
            // 
            // coltemplateWorkFlow
            // 
            this.coltemplateWorkFlow.FieldName = "templateWorkFlow";
            this.coltemplateWorkFlow.MinWidth = 30;
            this.coltemplateWorkFlow.Name = "coltemplateWorkFlow";
            this.coltemplateWorkFlow.Width = 112;
            // 
            // coltemplateName
            // 
            this.coltemplateName.Caption = "Name";
            this.coltemplateName.FieldName = "templateName";
            this.coltemplateName.MinWidth = 30;
            this.coltemplateName.Name = "coltemplateName";
            this.coltemplateName.OptionsColumn.AllowEdit = false;
            this.coltemplateName.Visible = true;
            this.coltemplateName.VisibleIndex = 0;
            this.coltemplateName.Width = 112;
            // 
            // coltemplateRevision
            // 
            this.coltemplateRevision.Caption = "Revision";
            this.coltemplateRevision.FieldName = "templateRevision";
            this.coltemplateRevision.MinWidth = 30;
            this.coltemplateRevision.Name = "coltemplateRevision";
            this.coltemplateRevision.OptionsColumn.AllowEdit = false;
            this.coltemplateRevision.Visible = true;
            this.coltemplateRevision.VisibleIndex = 2;
            this.coltemplateRevision.Width = 112;
            // 
            // coltemplateDiscipline
            // 
            this.coltemplateDiscipline.Caption = "Discipline";
            this.coltemplateDiscipline.FieldName = "templateDiscipline";
            this.coltemplateDiscipline.MinWidth = 30;
            this.coltemplateDiscipline.Name = "coltemplateDiscipline";
            this.coltemplateDiscipline.OptionsColumn.AllowEdit = false;
            this.coltemplateDiscipline.Visible = true;
            this.coltemplateDiscipline.VisibleIndex = 3;
            this.coltemplateDiscipline.Width = 112;
            // 
            // coltemplateDescription
            // 
            this.coltemplateDescription.Caption = "Description";
            this.coltemplateDescription.FieldName = "templateDescription";
            this.coltemplateDescription.MinWidth = 30;
            this.coltemplateDescription.Name = "coltemplateDescription";
            this.coltemplateDescription.OptionsColumn.AllowEdit = false;
            this.coltemplateDescription.Visible = true;
            this.coltemplateDescription.VisibleIndex = 1;
            this.coltemplateDescription.Width = 112;
            // 
            // colTemplateQRSupport
            // 
            this.colTemplateQRSupport.Caption = "QR Support";
            this.colTemplateQRSupport.FieldName = "templateQRSupport";
            this.colTemplateQRSupport.MinWidth = 30;
            this.colTemplateQRSupport.Name = "colTemplateQRSupport";
            this.colTemplateQRSupport.OptionsColumn.AllowEdit = false;
            this.colTemplateQRSupport.Visible = true;
            this.colTemplateQRSupport.VisibleIndex = 4;
            this.colTemplateQRSupport.Width = 112;
            // 
            // colGUID1
            // 
            this.colGUID1.FieldName = "GUID";
            this.colGUID1.MinWidth = 30;
            this.colGUID1.Name = "colGUID1";
            this.colGUID1.Width = 112;
            // 
            // colCreatedDate1
            // 
            this.colCreatedDate1.FieldName = "CreatedDate";
            this.colCreatedDate1.MinWidth = 30;
            this.colCreatedDate1.Name = "colCreatedDate1";
            this.colCreatedDate1.Width = 112;
            // 
            // colCreatedBy1
            // 
            this.colCreatedBy1.FieldName = "CreatedBy";
            this.colCreatedBy1.MinWidth = 30;
            this.colCreatedBy1.Name = "colCreatedBy1";
            this.colCreatedBy1.Width = 112;
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.btnTrimDuplicates);
            this.panelControl2.Controls.Add(this.btnCopyFrom);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl2.Location = new System.Drawing.Point(2, 42);
            this.panelControl2.Margin = new System.Windows.Forms.Padding(4);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(674, 40);
            this.panelControl2.TabIndex = 16;
            // 
            // btnTrimDuplicates
            // 
            this.btnTrimDuplicates.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTrimDuplicates.Appearance.Options.UseFont = true;
            this.btnTrimDuplicates.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnTrimDuplicates.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTrimDuplicates.ImageOptions.Image")));
            this.btnTrimDuplicates.Location = new System.Drawing.Point(252, 2);
            this.btnTrimDuplicates.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnTrimDuplicates.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnTrimDuplicates.Margin = new System.Windows.Forms.Padding(4);
            this.btnTrimDuplicates.Name = "btnTrimDuplicates";
            this.btnTrimDuplicates.Padding = new System.Windows.Forms.Padding(4);
            this.btnTrimDuplicates.Size = new System.Drawing.Size(210, 36);
            this.btnTrimDuplicates.TabIndex = 4;
            this.btnTrimDuplicates.Text = "Trim Duplicates";
            this.btnTrimDuplicates.Click += new System.EventHandler(this.btnTrimDuplicates_Click);
            // 
            // btnCopyFrom
            // 
            this.btnCopyFrom.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopyFrom.Appearance.Options.UseFont = true;
            this.btnCopyFrom.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCopyFrom.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyFrom.ImageOptions.Image")));
            this.btnCopyFrom.Location = new System.Drawing.Point(462, 2);
            this.btnCopyFrom.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnCopyFrom.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCopyFrom.Margin = new System.Windows.Forms.Padding(4);
            this.btnCopyFrom.Name = "btnCopyFrom";
            this.btnCopyFrom.Padding = new System.Windows.Forms.Padding(4);
            this.btnCopyFrom.Size = new System.Drawing.Size(210, 36);
            this.btnCopyFrom.TabIndex = 3;
            this.btnCopyFrom.Text = "Copy From";
            this.btnCopyFrom.Click += new System.EventHandler(this.btnCopyFrom_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.cmbProject);
            this.panelControl1.Controls.Add(this.labelControl5);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(2, 2);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(674, 40);
            this.panelControl1.TabIndex = 15;
            // 
            // cmbProject
            // 
            this.cmbProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbProject.Location = new System.Drawing.Point(152, 2);
            this.cmbProject.Margin = new System.Windows.Forms.Padding(0);
            this.cmbProject.Name = "cmbProject";
            this.cmbProject.Properties.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbProject.Properties.Appearance.Options.UseFont = true;
            this.cmbProject.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbProject.Properties.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.cmbProject.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.cmbProject.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbProject.Size = new System.Drawing.Size(520, 30);
            this.cmbProject.TabIndex = 10;
            this.cmbProject.EditValueChanged += new System.EventHandler(this.cmbProject_EditValueChanged);
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl5.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelControl5.Location = new System.Drawing.Point(2, 2);
            this.labelControl5.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(150, 36);
            this.labelControl5.TabIndex = 9;
            this.labelControl5.Text = "Select Project";
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // splashScreenManager2
            // 
            splashScreenManager2.ClosingDelay = 500;
            // 
            // frmSchedule_Matrix
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.ClientSize = new System.Drawing.Size(1372, 970);
            this.Controls.Add(this.tblLayoutMain);
            this.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmSchedule_Matrix";
            this.Text = "Matrix";
            this.tblLayoutMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlTypeMain)).EndInit();
            this.pnlTypeMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixTypeBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewType)).EndInit();
            this.flowLayoutPanelSchedule.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlTemplate)).EndInit();
            this.pnlTemplate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.templateCheckBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbProject.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblLayoutMain;
        private DevExpress.XtraEditors.PanelControl pnlTypeMain;
        private DevExpress.XtraGrid.GridControl gridControlType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewType;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelSchedule;
        private DevExpress.XtraEditors.SimpleButton btnAddType;
        private DevExpress.XtraEditors.SimpleButton btnEditType;
        private DevExpress.XtraEditors.SimpleButton btnDeleteType;
        private DevExpress.XtraEditors.PanelControl pnlTemplate;
        private System.Windows.Forms.BindingSource matrixTypeBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn coltypeName;
        private DevExpress.XtraGrid.Columns.GridColumn coltypeDescription;
        private DevExpress.XtraGrid.Columns.GridColumn coltypeCategory;
        private DevExpress.XtraGrid.Columns.GridColumn coltypeDiscipline;
        private DevExpress.XtraGrid.Columns.GridColumn colGUID;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedBy;
        private DevExpress.XtraGrid.GridControl gridControlTemplate;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTemplate;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.ComboBoxEdit cmbProject;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private System.Windows.Forms.BindingSource templateCheckBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn coltemplateSelected;
        private DevExpress.XtraGrid.Columns.GridColumn coltemplateWorkFlow;
        private DevExpress.XtraGrid.Columns.GridColumn coltemplateName;
        private DevExpress.XtraGrid.Columns.GridColumn coltemplateRevision;
        private DevExpress.XtraGrid.Columns.GridColumn coltemplateDiscipline;
        private DevExpress.XtraGrid.Columns.GridColumn coltemplateDescription;
        private DevExpress.XtraGrid.Columns.GridColumn colGUID1;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate1;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedBy1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.SimpleButton btnCopyFrom;
        private DevExpress.XtraGrid.Columns.GridColumn colTemplateQRSupport;
        private DevExpress.XtraEditors.SimpleButton btnTrimDuplicates;
        private DevExpress.XtraEditors.SimpleButton btnReport;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
    }
}
