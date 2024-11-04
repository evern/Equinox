
namespace CheckmateDX
{
    partial class frmCertificate_PunchlistWalkdown_Browse
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCertificate_PunchlistWalkdown_Browse));
            this.colSubsystems = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDisciplnes = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.punchlistWalkdownBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.barMenu = new DevExpress.XtraBars.Bar();
            this.btnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btnChangeDescription = new DevExpress.XtraBars.BarButtonItem();
            this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.barEdit = new DevExpress.XtraBars.BarButtonItem();
            this.btnHistory = new DevExpress.XtraBars.BarButtonItem();
            this.btnReport = new DevExpress.XtraBars.BarButtonItem();
            this.barStatus = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barSubItemApprovals = new DevExpress.XtraBars.BarSubItem();
            this.barButtonItemPending = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItemApproved = new DevExpress.XtraBars.BarButtonItem();
            this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.WaitForm1), true, true);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.flyoutPanel = new DevExpress.Utils.FlyoutPanel();
            this.flyoutPanelControl = new DevExpress.Utils.FlyoutPanelControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.treeListComments = new DevExpress.XtraTreeList.TreeList();
            this.colCertificateCommInfo = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colCertificateCommCreator = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colCreatedDate = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colCreatedBy = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.CommentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            this.imageList3 = new System.Windows.Forms.ImageList(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnComment = new DevExpress.XtraEditors.SimpleButton();
            this.txtComments = new DevExpress.XtraEditors.TextEdit();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.punchlistWalkdownBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.flyoutPanel)).BeginInit();
            this.flyoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.flyoutPanelControl)).BeginInit();
            this.flyoutPanelControl.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListComments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CommentBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtComments.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // colSubsystems
            // 
            this.colSubsystems.Caption = "Subsystems";
            this.colSubsystems.FieldName = "DelimitedSubsystems";
            this.colSubsystems.MinWidth = 30;
            this.colSubsystems.Name = "colSubsystems";
            this.colSubsystems.OptionsColumn.AllowEdit = false;
            this.colSubsystems.Visible = true;
            this.colSubsystems.VisibleIndex = 4;
            this.colSubsystems.Width = 637;
            // 
            // colDisciplnes
            // 
            this.colDisciplnes.Caption = "Disciplines";
            this.colDisciplnes.FieldName = "DelimitedDisciplines";
            this.colDisciplnes.MinWidth = 30;
            this.colDisciplnes.Name = "colDisciplnes";
            this.colDisciplnes.OptionsColumn.AllowEdit = false;
            this.colDisciplnes.Visible = true;
            this.colDisciplnes.VisibleIndex = 3;
            this.colDisciplnes.Width = 200;
            // 
            // colNumber
            // 
            this.colNumber.Caption = "Number";
            this.colNumber.FieldName = "Number";
            this.colNumber.MinWidth = 30;
            this.colNumber.Name = "colNumber";
            this.colNumber.OptionsColumn.AllowEdit = false;
            this.colNumber.Visible = true;
            this.colNumber.VisibleIndex = 0;
            this.colNumber.Width = 150;
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colNumber,
            this.colDisciplnes,
            this.colStatus,
            this.colDescription,
            this.colSubsystems});
            this.gridView1.DetailHeight = 553;
            this.gridView1.FixedLineWidth = 3;
            this.gridView1.GridControl = this.gridControl;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsSelection.MultiSelect = true;
            this.gridView1.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gridView1_CustomDrawCell);
            this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // colStatus
            // 
            this.colStatus.Caption = "Status";
            this.colStatus.FieldName = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.OptionsColumn.AllowEdit = false;
            this.colStatus.Visible = true;
            this.colStatus.VisibleIndex = 2;
            // 
            // colDescription
            // 
            this.colDescription.Caption = "Description";
            this.colDescription.FieldName = "Description";
            this.colDescription.MinWidth = 30;
            this.colDescription.Name = "colDescription";
            this.colDescription.OptionsColumn.AllowEdit = false;
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 1;
            this.colDescription.Width = 500;
            // 
            // gridControl
            // 
            this.gridControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControl.DataSource = this.punchlistWalkdownBindingSource;
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(62, 29, 62, 29);
            this.gridControl.Location = new System.Drawing.Point(0, 58);
            this.gridControl.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.gridControl.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControl.MainView = this.gridView1;
            this.gridControl.Margin = new System.Windows.Forms.Padding(62, 29, 62, 29);
            this.gridControl.MenuManager = this.barManager1;
            this.gridControl.Name = "gridControl";
            this.gridControl.Size = new System.Drawing.Size(1514, 783);
            this.gridControl.TabIndex = 6;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.gridControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gridControl_MouseClick);
            // 
            // punchlistWalkdownBindingSource
            // 
            this.punchlistWalkdownBindingSource.DataSource = typeof(ProjectLibrary.ViewModel_PunchlistWalkdown);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.barMenu,
            this.barStatus});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.btnAdd,
            this.btnChangeDescription,
            this.btnDelete,
            this.barEdit,
            this.barSubItemApprovals,
            this.barButtonItemPending,
            this.barButtonItemApproved,
            this.btnHistory,
            this.btnReport});
            this.barManager1.MaxItemId = 17;
            this.barManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemComboBox1});
            this.barManager1.StatusBar = this.barStatus;
            // 
            // barMenu
            // 
            this.barMenu.BarName = "Tools";
            this.barMenu.DockCol = 0;
            this.barMenu.DockRow = 0;
            this.barMenu.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAdd),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnChangeDescription),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnDelete),
            new DevExpress.XtraBars.LinkPersistInfo(this.barEdit),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnHistory),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnReport)});
            this.barMenu.Text = "Tools";
            // 
            // btnAdd
            // 
            this.btnAdd.Border = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.btnAdd.Caption = "&Add";
            this.btnAdd.Id = 0;
            this.btnAdd.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.ImageOptions.Image")));
            this.btnAdd.ItemAppearance.Normal.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ItemAppearance.Normal.Options.UseFont = true;
            this.btnAdd.ItemAppearance.Pressed.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ItemAppearance.Pressed.Options.UseFont = true;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAdd_ItemClick);
            // 
            // btnChangeDescription
            // 
            this.btnChangeDescription.Border = DevExpress.XtraEditors.Controls.BorderStyles.Default;
            this.btnChangeDescription.Caption = "&Change Description";
            this.btnChangeDescription.Id = 1;
            this.btnChangeDescription.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnChangeDescription.ImageOptions.Image")));
            this.btnChangeDescription.ItemAppearance.Normal.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChangeDescription.ItemAppearance.Normal.Options.UseFont = true;
            this.btnChangeDescription.ItemAppearance.Pressed.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChangeDescription.ItemAppearance.Pressed.Options.UseFont = true;
            this.btnChangeDescription.Name = "btnChangeDescription";
            this.btnChangeDescription.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnChangeDescription.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnChangeDescription_ItemClick);
            // 
            // btnDelete
            // 
            this.btnDelete.Caption = "&Delete";
            this.btnDelete.Id = 2;
            this.btnDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.Image")));
            this.btnDelete.ItemAppearance.Normal.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ItemAppearance.Normal.Options.UseFont = true;
            this.btnDelete.ItemAppearance.Pressed.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ItemAppearance.Pressed.Options.UseFont = true;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnDelete_ItemClick);
            // 
            // barEdit
            // 
            this.barEdit.Caption = "Edit";
            this.barEdit.Id = 11;
            this.barEdit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barEdit.ImageOptions.Image")));
            this.barEdit.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barEdit.ImageOptions.LargeImage")));
            this.barEdit.Name = "barEdit";
            this.barEdit.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.barEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonEdit_ItemClick);
            // 
            // btnHistory
            // 
            this.btnHistory.Caption = "History";
            this.btnHistory.Id = 15;
            this.btnHistory.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnHistory.ImageOptions.Image")));
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnHistory.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnHistory_ItemClick);
            // 
            // btnReport
            // 
            this.btnReport.Caption = "Master Report";
            this.btnReport.Id = 16;
            this.btnReport.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnReport.ImageOptions.Image")));
            this.btnReport.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("btnReport.ImageOptions.LargeImage")));
            this.btnReport.Name = "btnReport";
            this.btnReport.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnReport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnReport_ItemClick);
            // 
            // barStatus
            // 
            this.barStatus.BarName = "Status bar";
            this.barStatus.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.barStatus.DockCol = 0;
            this.barStatus.DockRow = 0;
            this.barStatus.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.barStatus.OptionsBar.AllowQuickCustomization = false;
            this.barStatus.OptionsBar.DrawDragBorder = false;
            this.barStatus.OptionsBar.UseWholeRow = true;
            this.barStatus.Text = "Status bar";
            this.barStatus.Visible = false;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(62, 29, 62, 29);
            this.barDockControlTop.Size = new System.Drawing.Size(1514, 58);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 841);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(62, 29, 62, 29);
            this.barDockControlBottom.Size = new System.Drawing.Size(1514, 20);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 58);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(62, 29, 62, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 783);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1514, 58);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(62, 29, 62, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 783);
            // 
            // barSubItemApprovals
            // 
            this.barSubItemApprovals.Caption = "Approvals";
            this.barSubItemApprovals.Id = 12;
            this.barSubItemApprovals.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barSubItemApprovals.ImageOptions.Image")));
            this.barSubItemApprovals.Name = "barSubItemApprovals";
            this.barSubItemApprovals.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            // 
            // barButtonItemPending
            // 
            this.barButtonItemPending.Caption = "Pending";
            this.barButtonItemPending.Id = 13;
            this.barButtonItemPending.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItemPending.ImageOptions.Image")));
            this.barButtonItemPending.Name = "barButtonItemPending";
            // 
            // barButtonItemApproved
            // 
            this.barButtonItemApproved.Caption = "Approved";
            this.barButtonItemApproved.Id = 14;
            this.barButtonItemApproved.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItemApproved.ImageOptions.Image")));
            this.barButtonItemApproved.Name = "barButtonItemApproved";
            // 
            // repositoryItemComboBox1
            // 
            this.repositoryItemComboBox1.AutoHeight = false;
            this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
            this.repositoryItemComboBox1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // flyoutPanel
            // 
            this.flyoutPanel.Controls.Add(this.flyoutPanelControl);
            this.flyoutPanel.Location = new System.Drawing.Point(148, 181);
            this.flyoutPanel.Margin = new System.Windows.Forms.Padding(2);
            this.flyoutPanel.Name = "flyoutPanel";
            this.flyoutPanel.Options.AnchorType = DevExpress.Utils.Win.PopupToolWindowAnchor.TopRight;
            this.flyoutPanel.OptionsButtonPanel.ButtonPanelHeight = 46;
            this.flyoutPanel.OwnerControl = this.gridControl;
            this.flyoutPanel.Size = new System.Drawing.Size(600, 659);
            this.flyoutPanel.TabIndex = 21;
            // 
            // flyoutPanelControl
            // 
            this.flyoutPanelControl.Controls.Add(this.tableLayoutPanel2);
            this.flyoutPanelControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flyoutPanelControl.FlyoutPanel = this.flyoutPanel;
            this.flyoutPanelControl.Location = new System.Drawing.Point(0, 0);
            this.flyoutPanelControl.Margin = new System.Windows.Forms.Padding(2);
            this.flyoutPanelControl.Name = "flyoutPanelControl";
            this.flyoutPanelControl.Size = new System.Drawing.Size(600, 659);
            this.flyoutPanelControl.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.treeListComments, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(596, 655);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // treeListComments
            // 
            this.treeListComments.Appearance.Row.Options.UseTextOptions = true;
            this.treeListComments.Appearance.Row.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.treeListComments.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colCertificateCommInfo,
            this.colCertificateCommCreator,
            this.colCreatedDate,
            this.colCreatedBy});
            this.treeListComments.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListComments.DataSource = this.CommentBindingSource;
            this.treeListComments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListComments.HorzScrollStep = 5;
            this.treeListComments.ImageIndexFieldName = "CertificateCommImageIndex";
            this.treeListComments.KeyFieldName = "GUID";
            this.treeListComments.Location = new System.Drawing.Point(2, 2);
            this.treeListComments.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.treeListComments.LookAndFeel.UseDefaultLookAndFeel = false;
            this.treeListComments.Margin = new System.Windows.Forms.Padding(2);
            this.treeListComments.MinWidth = 30;
            this.treeListComments.Name = "treeListComments";
            this.treeListComments.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.treeListComments.OptionsView.AutoCalcPreviewLineCount = true;
            this.treeListComments.OptionsView.ShowPreview = true;
            this.treeListComments.OptionsView.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.Dark;
            this.treeListComments.ParentFieldName = "CertificateCommParentGuid";
            this.treeListComments.PreviewFieldName = "CertificateCommInfo";
            this.treeListComments.PreviewLineCount = 5;
            this.treeListComments.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemMemoEdit1});
            this.treeListComments.RowHeight = 46;
            this.treeListComments.SelectImageList = this.imageList3;
            this.treeListComments.Size = new System.Drawing.Size(592, 636);
            this.treeListComments.TabIndex = 16;
            this.treeListComments.TreeLevelWidth = 26;
            // 
            // colCertificateCommInfo
            // 
            this.colCertificateCommInfo.AppearanceCell.Options.UseTextOptions = true;
            this.colCertificateCommInfo.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
            this.colCertificateCommInfo.Caption = "Information";
            this.colCertificateCommInfo.FieldName = "CertificateCommInfo";
            this.colCertificateCommInfo.MinWidth = 30;
            this.colCertificateCommInfo.Name = "colCertificateCommInfo";
            this.colCertificateCommInfo.OptionsColumn.AllowEdit = false;
            this.colCertificateCommInfo.Width = 559;
            // 
            // colCertificateCommCreator
            // 
            this.colCertificateCommCreator.Caption = "Creator";
            this.colCertificateCommCreator.FieldName = "CertificateCommCreator";
            this.colCertificateCommCreator.MinWidth = 50;
            this.colCertificateCommCreator.Name = "colCertificateCommCreator";
            this.colCertificateCommCreator.OptionsColumn.AllowEdit = false;
            this.colCertificateCommCreator.Visible = true;
            this.colCertificateCommCreator.VisibleIndex = 0;
            this.colCertificateCommCreator.Width = 314;
            // 
            // colCreatedDate
            // 
            this.colCreatedDate.FieldName = "CreatedDate";
            this.colCreatedDate.Format.FormatString = "g";
            this.colCreatedDate.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colCreatedDate.MinWidth = 30;
            this.colCreatedDate.Name = "colCreatedDate";
            this.colCreatedDate.OptionsColumn.AllowEdit = false;
            this.colCreatedDate.Visible = true;
            this.colCreatedDate.VisibleIndex = 1;
            this.colCreatedDate.Width = 224;
            // 
            // colCreatedBy
            // 
            this.colCreatedBy.FieldName = "CreatedBy";
            this.colCreatedBy.MinWidth = 30;
            this.colCreatedBy.Name = "colCreatedBy";
            this.colCreatedBy.Width = 209;
            // 
            // CommentBindingSource
            // 
            this.CommentBindingSource.DataSource = typeof(ProjectLibrary.CertificateComments);
            // 
            // repositoryItemMemoEdit1
            // 
            this.repositoryItemMemoEdit1.LinesCount = 5;
            this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
            // 
            // imageList3
            // 
            this.imageList3.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList3.ImageStream")));
            this.imageList3.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList3.Images.SetKeyName(0, "new-32x32.png");
            this.imageList3.Images.SetKeyName(1, "stock_down-32x32.png");
            this.imageList3.Images.SetKeyName(2, "stock_up-32x32.png");
            this.imageList3.Images.SetKeyName(3, "comment-32x32.png");
            this.imageList3.Images.SetKeyName(4, "exclamation-32x32.png");
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnComment);
            this.flowLayoutPanel1.Controls.Add(this.txtComments);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(10, 651);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(10, 11, 10, 11);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(576, 1);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.Visible = false;
            // 
            // btnComment
            // 
            this.btnComment.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnComment.Appearance.Options.UseFont = true;
            this.btnComment.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnComment.ImageOptions.Image")));
            this.btnComment.Location = new System.Drawing.Point(424, 2);
            this.btnComment.Margin = new System.Windows.Forms.Padding(2);
            this.btnComment.Name = "btnComment";
            this.btnComment.Size = new System.Drawing.Size(150, 53);
            this.btnComment.TabIndex = 5;
            this.btnComment.Text = "&Reply";
            // 
            // txtComments
            // 
            this.txtComments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtComments.Location = new System.Drawing.Point(64, 72);
            this.txtComments.Margin = new System.Windows.Forms.Padding(2, 15, 2, 2);
            this.txtComments.Name = "txtComments";
            this.txtComments.Properties.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtComments.Properties.Appearance.Options.UseFont = true;
            this.txtComments.Size = new System.Drawing.Size(510, 30);
            this.txtComments.TabIndex = 6;
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "multiroute_32x32.png");
            this.imageList2.Images.SetKeyName(1, "Task_32x32.png");
            this.imageList2.Images.SetKeyName(2, "Task_32x32.png");
            this.imageList2.Images.SetKeyName(3, "new-32x32.png");
            this.imageList2.Images.SetKeyName(4, "stock_up-32x32.png");
            this.imageList2.Images.SetKeyName(5, "stock_down-32x32.png");
            this.imageList2.Images.SetKeyName(6, "comment-32x32.png");
            this.imageList2.Images.SetKeyName(7, "exclamation-32x32.png");
            // 
            // frmCertificate_PunchlistWalkdown_Browse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1514, 861);
            this.Controls.Add(this.flyoutPanel);
            this.Controls.Add(this.gridControl);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmCertificate_PunchlistWalkdown_Browse";
            this.Text = "Punchlist Walkdown";
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.punchlistWalkdownBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.flyoutPanel)).EndInit();
            this.flyoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.flyoutPanelControl)).EndInit();
            this.flyoutPanelControl.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListComments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CommentBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemoEdit1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtComments.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraGrid.Columns.GridColumn colSubsystems;
        private DevExpress.XtraGrid.Columns.GridColumn colDisciplnes;
        private DevExpress.XtraGrid.Columns.GridColumn colNumber;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.GridControl gridControl;
        private System.Windows.Forms.BindingSource punchlistWalkdownBindingSource;
        protected DevExpress.XtraBars.BarManager barManager1;
        protected DevExpress.XtraBars.Bar barMenu;
        private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemComboBox1;
        protected DevExpress.XtraBars.BarButtonItem btnAdd;
        protected DevExpress.XtraBars.BarButtonItem btnChangeDescription;
        protected DevExpress.XtraBars.BarButtonItem btnDelete;
        protected DevExpress.XtraBars.Bar barStatus;
        protected DevExpress.XtraBars.BarDockControl barDockControlTop;
        protected DevExpress.XtraBars.BarDockControl barDockControlBottom;
        protected DevExpress.XtraBars.BarDockControl barDockControlLeft;
        protected DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private DevExpress.XtraBars.BarButtonItem barEdit;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraBars.BarSubItem barSubItemApprovals;
        private DevExpress.XtraBars.BarButtonItem barButtonItemPending;
        private DevExpress.XtraBars.BarButtonItem barButtonItemApproved;
        private DevExpress.XtraGrid.Columns.GridColumn colStatus;
        private DevExpress.Utils.FlyoutPanel flyoutPanel;
        private DevExpress.Utils.FlyoutPanelControl flyoutPanelControl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private DevExpress.XtraTreeList.TreeList treeListComments;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCertificateCommInfo;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCertificateCommCreator;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCreatedDate;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCreatedBy;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit repositoryItemMemoEdit1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraEditors.SimpleButton btnComment;
        private DevExpress.XtraEditors.TextEdit txtComments;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.BindingSource CommentBindingSource;
        private System.Windows.Forms.ImageList imageList3;
        private DevExpress.XtraBars.BarButtonItem btnHistory;
        private DevExpress.XtraBars.BarButtonItem btnReport;
    }
}