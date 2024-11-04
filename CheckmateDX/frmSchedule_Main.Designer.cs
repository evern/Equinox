namespace CheckmateDX
{
    partial class frmSchedule_Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSchedule_Main));
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.WaitForm1), true, true);
            this.splashScreenManager2 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), true, true);
            this.barMenu = new DevExpress.XtraBars.Bar();
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.barStatus = new DevExpress.XtraBars.Bar();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnlTagMain = new DevExpress.XtraEditors.PanelControl();
            this.treeListWBSTag = new DevExpress.XtraTreeList.TreeList();
            this.colwbsTagDisplayName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colwbsTagDisplayDescription = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colwbsTagDisplayType1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colwbsTagDisplayType2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colwbsTagDisplayType3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colwbsTagDisplayAttachTag = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colwbsTagDisplayAttachWBS = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.wbsTagDisplayBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.flowLayoutPanelTag = new System.Windows.Forms.FlowLayoutPanel();
            this.dropDownButtonAdd = new DevExpress.XtraEditors.DropDownButton();
            this.popupMenu1 = new DevExpress.XtraBars.PopupMenu(this.components);
            this.bBtnAddTag = new DevExpress.XtraBars.BarButtonItem();
            this.bBtnAddWBS = new DevExpress.XtraBars.BarButtonItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.barEditItem1 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.barEditItem2 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemPopupContainerEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPopupContainerEdit();
            this.barEditItem3 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemTextEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            this.barEditItem4 = new DevExpress.XtraBars.BarEditItem();
            this.repositoryItemSpreadsheetCellStyleEdit1 = new DevExpress.XtraSpreadsheet.Design.RepositoryItemSpreadsheetCellStyleEdit();
            this.btnEditTagWBS = new DevExpress.XtraEditors.SimpleButton();
            this.bBtnDeleteTagWBS = new DevExpress.XtraEditors.SimpleButton();
            this.btnDeleteTag = new DevExpress.XtraEditors.SimpleButton();
            this.bBtnAssignTemplate = new DevExpress.XtraEditors.SimpleButton();
            this.bBtnBulkEdit = new DevExpress.XtraEditors.SimpleButton();
            this.bBtnMatrixAssign = new DevExpress.XtraEditors.SimpleButton();
            this.pnlScheduleMain = new DevExpress.XtraEditors.PanelControl();
            this.gridControlSchedule = new DevExpress.XtraGrid.GridControl();
            this.scheduleBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridViewSchedule = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colscheduleProjectGuid = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colscheduleName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colscheduleDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colscheduleDiscipline = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGUID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.cmbDiscipline = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.groupBoxAll = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.simpleButton8 = new DevExpress.XtraEditors.SimpleButton();
            this.btnStatusOnly = new DevExpress.XtraEditors.SimpleButton();
            this.btnMasterReport = new DevExpress.XtraEditors.SimpleButton();
            this.btnITRStatusBreakdownReport = new DevExpress.XtraEditors.SimpleButton();
            this.btnDisciplineMasterReport = new DevExpress.XtraEditors.SimpleButton();
            this.btnTrimWBS = new DevExpress.XtraEditors.SimpleButton();
            this.groupBoxIndividual = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.simpleButton9 = new DevExpress.XtraEditors.SimpleButton();
            this.btnImportMasterSheet = new DevExpress.XtraEditors.SimpleButton();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton3 = new DevExpress.XtraEditors.SimpleButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlTagMain)).BeginInit();
            this.pnlTagMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListWBSTag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wbsTagDisplayBindingSource)).BeginInit();
            this.flowLayoutPanelTag.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPopupContainerEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpreadsheetCellStyleEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlScheduleMain)).BeginInit();
            this.pnlScheduleMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSchedule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scheduleBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSchedule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbDiscipline.Properties)).BeginInit();
            this.groupBoxAll.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.groupBoxIndividual.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // barMenu
            // 
            this.barMenu.BarName = "Tools";
            this.barMenu.DockCol = 0;
            this.barMenu.DockRow = 0;
            this.barMenu.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.barMenu.Text = "Tools";
            // 
            // bar2
            // 
            this.bar2.BarName = "Status bar";
            this.bar2.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar2.OptionsBar.AllowQuickCustomization = false;
            this.bar2.OptionsBar.DrawDragBorder = false;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Status bar";
            // 
            // bar3
            // 
            this.bar3.BarName = "Tools";
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Left;
            this.bar3.FloatLocation = new System.Drawing.Point(386, 249);
            this.bar3.Text = "Tools";
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
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 2;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.02143F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.97857F));
            this.tableLayoutPanelMain.Controls.Add(this.pnlTagMain, 1, 0);
            this.tableLayoutPanelMain.Controls.Add(this.pnlScheduleMain, 0, 0);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(7);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(3665, 1269);
            this.tableLayoutPanelMain.TabIndex = 0;
            // 
            // pnlTagMain
            // 
            this.pnlTagMain.Controls.Add(this.treeListWBSTag);
            this.pnlTagMain.Controls.Add(this.flowLayoutPanelTag);
            this.pnlTagMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTagMain.Location = new System.Drawing.Point(1693, 7);
            this.pnlTagMain.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.pnlTagMain.LookAndFeel.UseDefaultLookAndFeel = false;
            this.pnlTagMain.Margin = new System.Windows.Forms.Padding(7);
            this.pnlTagMain.Name = "pnlTagMain";
            this.pnlTagMain.Size = new System.Drawing.Size(1965, 1215);
            this.pnlTagMain.TabIndex = 4;
            // 
            // treeListWBSTag
            // 
            this.treeListWBSTag.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colwbsTagDisplayName,
            this.colwbsTagDisplayDescription,
            this.colwbsTagDisplayType1,
            this.colwbsTagDisplayType2,
            this.colwbsTagDisplayType3,
            this.colwbsTagDisplayAttachTag,
            this.colwbsTagDisplayAttachWBS});
            this.treeListWBSTag.Cursor = System.Windows.Forms.Cursors.Default;
            this.treeListWBSTag.DataSource = this.wbsTagDisplayBindingSource;
            this.treeListWBSTag.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListWBSTag.FixedLineWidth = 3;
            this.treeListWBSTag.HorzScrollStep = 7;
            this.treeListWBSTag.ImageIndexFieldName = "wbsTagDisplayImageIndex";
            this.treeListWBSTag.KeyFieldName = "wbsTagDisplayGuid";
            this.treeListWBSTag.Location = new System.Drawing.Point(3, 102);
            this.treeListWBSTag.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.treeListWBSTag.LookAndFeel.UseDefaultLookAndFeel = false;
            this.treeListWBSTag.Margin = new System.Windows.Forms.Padding(7);
            this.treeListWBSTag.MinWidth = 40;
            this.treeListWBSTag.Name = "treeListWBSTag";
            this.treeListWBSTag.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.treeListWBSTag.OptionsSelection.MultiSelect = true;
            this.treeListWBSTag.OptionsView.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.Dark;
            this.treeListWBSTag.ParentFieldName = "wbsTagDisplayParentGuid";
            this.treeListWBSTag.SelectImageList = this.imageList1;
            this.treeListWBSTag.Size = new System.Drawing.Size(1959, 1110);
            this.treeListWBSTag.TabIndex = 13;
            this.treeListWBSTag.TreeLevelWidth = 35;
            // 
            // colwbsTagDisplayName
            // 
            this.colwbsTagDisplayName.Caption = "Name";
            this.colwbsTagDisplayName.FieldName = "wbsTagDisplayName";
            this.colwbsTagDisplayName.MinWidth = 67;
            this.colwbsTagDisplayName.Name = "colwbsTagDisplayName";
            this.colwbsTagDisplayName.OptionsColumn.AllowEdit = false;
            this.colwbsTagDisplayName.Visible = true;
            this.colwbsTagDisplayName.VisibleIndex = 0;
            this.colwbsTagDisplayName.Width = 160;
            // 
            // colwbsTagDisplayDescription
            // 
            this.colwbsTagDisplayDescription.Caption = "Description";
            this.colwbsTagDisplayDescription.FieldName = "wbsTagDisplayDescription";
            this.colwbsTagDisplayDescription.MinWidth = 40;
            this.colwbsTagDisplayDescription.Name = "colwbsTagDisplayDescription";
            this.colwbsTagDisplayDescription.OptionsColumn.AllowEdit = false;
            this.colwbsTagDisplayDescription.Visible = true;
            this.colwbsTagDisplayDescription.VisibleIndex = 1;
            this.colwbsTagDisplayDescription.Width = 856;
            // 
            // colwbsTagDisplayType1
            // 
            this.colwbsTagDisplayType1.Caption = "Type 1";
            this.colwbsTagDisplayType1.FieldName = "wbsTagDisplayType1";
            this.colwbsTagDisplayType1.MinWidth = 40;
            this.colwbsTagDisplayType1.Name = "colwbsTagDisplayType1";
            this.colwbsTagDisplayType1.OptionsColumn.AllowEdit = false;
            this.colwbsTagDisplayType1.Visible = true;
            this.colwbsTagDisplayType1.VisibleIndex = 2;
            this.colwbsTagDisplayType1.Width = 151;
            // 
            // colwbsTagDisplayType2
            // 
            this.colwbsTagDisplayType2.Caption = "Type 2";
            this.colwbsTagDisplayType2.FieldName = "wbsTagDisplayType2";
            this.colwbsTagDisplayType2.MinWidth = 40;
            this.colwbsTagDisplayType2.Name = "colwbsTagDisplayType2";
            this.colwbsTagDisplayType2.OptionsColumn.AllowEdit = false;
            this.colwbsTagDisplayType2.Visible = true;
            this.colwbsTagDisplayType2.VisibleIndex = 3;
            this.colwbsTagDisplayType2.Width = 151;
            // 
            // colwbsTagDisplayType3
            // 
            this.colwbsTagDisplayType3.Caption = "Type 3";
            this.colwbsTagDisplayType3.FieldName = "wbsTagDisplayType3";
            this.colwbsTagDisplayType3.MinWidth = 40;
            this.colwbsTagDisplayType3.Name = "colwbsTagDisplayType3";
            this.colwbsTagDisplayType3.OptionsColumn.AllowEdit = false;
            this.colwbsTagDisplayType3.Visible = true;
            this.colwbsTagDisplayType3.VisibleIndex = 4;
            this.colwbsTagDisplayType3.Width = 151;
            // 
            // colwbsTagDisplayAttachTag
            // 
            this.colwbsTagDisplayAttachTag.FieldName = "wbsTagDisplayAttachTag";
            this.colwbsTagDisplayAttachTag.MinWidth = 40;
            this.colwbsTagDisplayAttachTag.Name = "colwbsTagDisplayAttachTag";
            this.colwbsTagDisplayAttachTag.Width = 160;
            // 
            // colwbsTagDisplayAttachWBS
            // 
            this.colwbsTagDisplayAttachWBS.FieldName = "wbsTagDisplayAttachWBS";
            this.colwbsTagDisplayAttachWBS.MinWidth = 40;
            this.colwbsTagDisplayAttachWBS.Name = "colwbsTagDisplayAttachWBS";
            this.colwbsTagDisplayAttachWBS.Width = 160;
            // 
            // wbsTagDisplayBindingSource
            // 
            this.wbsTagDisplayBindingSource.DataSource = typeof(ProjectLibrary.wbsTagDisplay);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "tag_folder-32x32.png");
            this.imageList1.Images.SetKeyName(1, "tag_green-32x32.png");
            // 
            // flowLayoutPanelTag
            // 
            this.flowLayoutPanelTag.Controls.Add(this.dropDownButtonAdd);
            this.flowLayoutPanelTag.Controls.Add(this.btnEditTagWBS);
            this.flowLayoutPanelTag.Controls.Add(this.bBtnDeleteTagWBS);
            this.flowLayoutPanelTag.Controls.Add(this.btnDeleteTag);
            this.flowLayoutPanelTag.Controls.Add(this.bBtnAssignTemplate);
            this.flowLayoutPanelTag.Controls.Add(this.bBtnBulkEdit);
            this.flowLayoutPanelTag.Controls.Add(this.bBtnMatrixAssign);
            this.flowLayoutPanelTag.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanelTag.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flowLayoutPanelTag.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanelTag.Margin = new System.Windows.Forms.Padding(7);
            this.flowLayoutPanelTag.Name = "flowLayoutPanelTag";
            this.flowLayoutPanelTag.Padding = new System.Windows.Forms.Padding(7);
            this.flowLayoutPanelTag.Size = new System.Drawing.Size(1959, 99);
            this.flowLayoutPanelTag.TabIndex = 0;
            // 
            // dropDownButtonAdd
            // 
            this.dropDownButtonAdd.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dropDownButtonAdd.Appearance.Options.UseFont = true;
            this.dropDownButtonAdd.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Show;
            this.dropDownButtonAdd.DropDownControl = this.popupMenu1;
            this.dropDownButtonAdd.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("dropDownButtonAdd.ImageOptions.Image")));
            this.dropDownButtonAdd.Location = new System.Drawing.Point(14, 14);
            this.dropDownButtonAdd.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.dropDownButtonAdd.LookAndFeel.UseDefaultLookAndFeel = false;
            this.dropDownButtonAdd.Margin = new System.Windows.Forms.Padding(7);
            this.dropDownButtonAdd.Name = "dropDownButtonAdd";
            this.dropDownButtonAdd.Size = new System.Drawing.Size(200, 80);
            this.dropDownButtonAdd.TabIndex = 0;
            this.dropDownButtonAdd.Text = "Add";
            this.dropDownButtonAdd.ToolTip = "Add a new Tag/WBS";
            // 
            // popupMenu1
            // 
            this.popupMenu1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bBtnAddTag),
            new DevExpress.XtraBars.LinkPersistInfo(this.bBtnAddWBS)});
            this.popupMenu1.Manager = this.barManager1;
            this.popupMenu1.MenuDrawMode = DevExpress.XtraBars.MenuDrawMode.LargeImagesText;
            this.popupMenu1.Name = "popupMenu1";
            // 
            // bBtnAddTag
            // 
            this.bBtnAddTag.Caption = "&Tag";
            this.bBtnAddTag.Id = 0;
            this.bBtnAddTag.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("bBtnAddTag.ImageOptions.Image")));
            this.bBtnAddTag.ItemAppearance.Normal.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnAddTag.ItemAppearance.Normal.Options.UseFont = true;
            this.bBtnAddTag.ItemAppearance.Pressed.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnAddTag.ItemAppearance.Pressed.Options.UseFont = true;
            this.bBtnAddTag.Name = "bBtnAddTag";
            this.bBtnAddTag.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAddTag_ItemClick);
            // 
            // bBtnAddWBS
            // 
            this.bBtnAddWBS.Caption = "&WBS";
            this.bBtnAddWBS.Id = 1;
            this.bBtnAddWBS.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("bBtnAddWBS.ImageOptions.Image")));
            this.bBtnAddWBS.ItemAppearance.Pressed.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnAddWBS.ItemAppearance.Pressed.Options.UseFont = true;
            this.bBtnAddWBS.Name = "bBtnAddWBS";
            this.bBtnAddWBS.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAddWBS_ItemClick);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bBtnAddTag,
            this.bBtnAddWBS,
            this.barEditItem1,
            this.barEditItem2,
            this.barEditItem3,
            this.barEditItem4});
            this.barManager1.MaxItemId = 6;
            this.barManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTextEdit1,
            this.repositoryItemPopupContainerEdit1,
            this.repositoryItemTextEdit2,
            this.repositoryItemSpreadsheetCellStyleEdit1});
            this.barManager1.StatusBar = this.bar1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Custom 2";
            this.bar1.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar1.OptionsBar.AllowQuickCustomization = false;
            this.bar1.OptionsBar.DrawDragBorder = false;
            this.bar1.OptionsBar.UseWholeRow = true;
            this.bar1.Text = "Custom 2";
            this.bar1.Visible = false;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(7);
            this.barDockControlTop.Size = new System.Drawing.Size(3665, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 1269);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(7);
            this.barDockControlBottom.Size = new System.Drawing.Size(3665, 26);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(7);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 1269);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(3665, 0);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(7);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 1269);
            // 
            // barEditItem1
            // 
            this.barEditItem1.Caption = "barEditItem1";
            this.barEditItem1.Edit = this.repositoryItemTextEdit1;
            this.barEditItem1.Id = 2;
            this.barEditItem1.Name = "barEditItem1";
            // 
            // repositoryItemTextEdit1
            // 
            this.repositoryItemTextEdit1.AutoHeight = false;
            this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // barEditItem2
            // 
            this.barEditItem2.Caption = "barEditItem2";
            this.barEditItem2.Edit = this.repositoryItemPopupContainerEdit1;
            this.barEditItem2.Id = 3;
            this.barEditItem2.Name = "barEditItem2";
            // 
            // repositoryItemPopupContainerEdit1
            // 
            this.repositoryItemPopupContainerEdit1.AutoHeight = false;
            this.repositoryItemPopupContainerEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemPopupContainerEdit1.Name = "repositoryItemPopupContainerEdit1";
            // 
            // barEditItem3
            // 
            this.barEditItem3.Caption = "barEditItem3";
            this.barEditItem3.Edit = this.repositoryItemTextEdit2;
            this.barEditItem3.Id = 4;
            this.barEditItem3.Name = "barEditItem3";
            // 
            // repositoryItemTextEdit2
            // 
            this.repositoryItemTextEdit2.AutoHeight = false;
            this.repositoryItemTextEdit2.Name = "repositoryItemTextEdit2";
            // 
            // barEditItem4
            // 
            this.barEditItem4.Caption = "barEditItem4";
            this.barEditItem4.Edit = this.repositoryItemSpreadsheetCellStyleEdit1;
            this.barEditItem4.Id = 5;
            this.barEditItem4.Name = "barEditItem4";
            // 
            // repositoryItemSpreadsheetCellStyleEdit1
            // 
            this.repositoryItemSpreadsheetCellStyleEdit1.AutoHeight = false;
            this.repositoryItemSpreadsheetCellStyleEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemSpreadsheetCellStyleEdit1.Control = null;
            this.repositoryItemSpreadsheetCellStyleEdit1.Name = "repositoryItemSpreadsheetCellStyleEdit1";
            // 
            // btnEditTagWBS
            // 
            this.btnEditTagWBS.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditTagWBS.Appearance.Options.UseFont = true;
            this.btnEditTagWBS.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEditTagWBS.ImageOptions.Image")));
            this.btnEditTagWBS.Location = new System.Drawing.Point(228, 14);
            this.btnEditTagWBS.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.btnEditTagWBS.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnEditTagWBS.Margin = new System.Windows.Forms.Padding(7);
            this.btnEditTagWBS.Name = "btnEditTagWBS";
            this.btnEditTagWBS.Padding = new System.Windows.Forms.Padding(7);
            this.btnEditTagWBS.Size = new System.Drawing.Size(200, 80);
            this.btnEditTagWBS.TabIndex = 2;
            this.btnEditTagWBS.Text = "Edit";
            this.btnEditTagWBS.ToolTip = "Edit a selected Tag/WBS";
            this.btnEditTagWBS.Click += new System.EventHandler(this.btnEditTagWBS_Click);
            // 
            // bBtnDeleteTagWBS
            // 
            this.bBtnDeleteTagWBS.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnDeleteTagWBS.Appearance.Options.UseFont = true;
            this.bBtnDeleteTagWBS.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("bBtnDeleteTagWBS.ImageOptions.Image")));
            this.bBtnDeleteTagWBS.Location = new System.Drawing.Point(442, 14);
            this.bBtnDeleteTagWBS.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.bBtnDeleteTagWBS.LookAndFeel.UseDefaultLookAndFeel = false;
            this.bBtnDeleteTagWBS.Margin = new System.Windows.Forms.Padding(7);
            this.bBtnDeleteTagWBS.Name = "bBtnDeleteTagWBS";
            this.bBtnDeleteTagWBS.Padding = new System.Windows.Forms.Padding(7);
            this.bBtnDeleteTagWBS.Size = new System.Drawing.Size(200, 80);
            this.bBtnDeleteTagWBS.TabIndex = 4;
            this.bBtnDeleteTagWBS.Text = "Delete";
            this.bBtnDeleteTagWBS.ToolTip = "Delete a selected Tag/WBS";
            this.bBtnDeleteTagWBS.Click += new System.EventHandler(this.bBtnDeleteTagWBS_Click);
            // 
            // btnDeleteTag
            // 
            this.btnDeleteTag.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteTag.Appearance.Options.UseFont = true;
            this.btnDeleteTag.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteTag.ImageOptions.Image")));
            this.btnDeleteTag.Location = new System.Drawing.Point(656, 14);
            this.btnDeleteTag.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.btnDeleteTag.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnDeleteTag.Margin = new System.Windows.Forms.Padding(7);
            this.btnDeleteTag.Name = "btnDeleteTag";
            this.btnDeleteTag.Padding = new System.Windows.Forms.Padding(7);
            this.btnDeleteTag.Size = new System.Drawing.Size(280, 80);
            this.btnDeleteTag.TabIndex = 8;
            this.btnDeleteTag.Text = "Delete Tag Only";
            this.btnDeleteTag.ToolTip = "Delete children Tags for selected WBS";
            this.btnDeleteTag.Click += new System.EventHandler(this.btnDeleteTag_Click);
            // 
            // bBtnAssignTemplate
            // 
            this.bBtnAssignTemplate.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnAssignTemplate.Appearance.Options.UseFont = true;
            this.bBtnAssignTemplate.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("bBtnAssignTemplate.ImageOptions.Image")));
            this.bBtnAssignTemplate.Location = new System.Drawing.Point(950, 14);
            this.bBtnAssignTemplate.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.bBtnAssignTemplate.LookAndFeel.UseDefaultLookAndFeel = false;
            this.bBtnAssignTemplate.Margin = new System.Windows.Forms.Padding(7);
            this.bBtnAssignTemplate.Name = "bBtnAssignTemplate";
            this.bBtnAssignTemplate.Padding = new System.Windows.Forms.Padding(7);
            this.bBtnAssignTemplate.Size = new System.Drawing.Size(280, 80);
            this.bBtnAssignTemplate.TabIndex = 5;
            this.bBtnAssignTemplate.Text = "Assign Template";
            this.bBtnAssignTemplate.ToolTip = "Assign template to Tag/WBS";
            this.bBtnAssignTemplate.Click += new System.EventHandler(this.bBtnAssignTemplate_Click);
            // 
            // bBtnBulkEdit
            // 
            this.bBtnBulkEdit.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnBulkEdit.Appearance.Options.UseFont = true;
            this.bBtnBulkEdit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("bBtnBulkEdit.ImageOptions.Image")));
            this.bBtnBulkEdit.Location = new System.Drawing.Point(1244, 14);
            this.bBtnBulkEdit.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.bBtnBulkEdit.LookAndFeel.UseDefaultLookAndFeel = false;
            this.bBtnBulkEdit.Margin = new System.Windows.Forms.Padding(7);
            this.bBtnBulkEdit.Name = "bBtnBulkEdit";
            this.bBtnBulkEdit.Padding = new System.Windows.Forms.Padding(7);
            this.bBtnBulkEdit.Size = new System.Drawing.Size(240, 80);
            this.bBtnBulkEdit.TabIndex = 6;
            this.bBtnBulkEdit.Text = "Bulk Add/Edit";
            this.bBtnBulkEdit.Visible = false;
            this.bBtnBulkEdit.Click += new System.EventHandler(this.bBtnBulkEdit_Click);
            // 
            // bBtnMatrixAssign
            // 
            this.bBtnMatrixAssign.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnMatrixAssign.Appearance.Options.UseFont = true;
            this.bBtnMatrixAssign.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("bBtnMatrixAssign.ImageOptions.Image")));
            this.bBtnMatrixAssign.Location = new System.Drawing.Point(1498, 14);
            this.bBtnMatrixAssign.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.bBtnMatrixAssign.LookAndFeel.UseDefaultLookAndFeel = false;
            this.bBtnMatrixAssign.Margin = new System.Windows.Forms.Padding(7);
            this.bBtnMatrixAssign.Name = "bBtnMatrixAssign";
            this.bBtnMatrixAssign.Padding = new System.Windows.Forms.Padding(7);
            this.bBtnMatrixAssign.Size = new System.Drawing.Size(240, 80);
            this.bBtnMatrixAssign.TabIndex = 7;
            this.bBtnMatrixAssign.Text = "Matrix Assign";
            this.bBtnMatrixAssign.ToolTip = "Assign template by type defined in Matrix. Matrix can be maintained in Template M" +
    "enu -> Matrix Menu";
            this.bBtnMatrixAssign.Click += new System.EventHandler(this.bBtnMatrixAssign_Click);
            // 
            // pnlScheduleMain
            // 
            this.pnlScheduleMain.Controls.Add(this.gridControlSchedule);
            this.pnlScheduleMain.Controls.Add(this.panelControl1);
            this.pnlScheduleMain.Controls.Add(this.groupBoxAll);
            this.pnlScheduleMain.Controls.Add(this.groupBoxIndividual);
            this.pnlScheduleMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlScheduleMain.Location = new System.Drawing.Point(7, 7);
            this.pnlScheduleMain.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.pnlScheduleMain.LookAndFeel.UseDefaultLookAndFeel = false;
            this.pnlScheduleMain.Margin = new System.Windows.Forms.Padding(7);
            this.pnlScheduleMain.Name = "pnlScheduleMain";
            this.pnlScheduleMain.Size = new System.Drawing.Size(1672, 1215);
            this.pnlScheduleMain.TabIndex = 3;
            // 
            // gridControlSchedule
            // 
            this.gridControlSchedule.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControlSchedule.DataSource = this.scheduleBindingSource;
            this.gridControlSchedule.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlSchedule.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(81, 39, 81, 39);
            this.gridControlSchedule.Location = new System.Drawing.Point(3, 515);
            this.gridControlSchedule.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.gridControlSchedule.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlSchedule.MainView = this.gridViewSchedule;
            this.gridControlSchedule.Margin = new System.Windows.Forms.Padding(81, 39, 81, 39);
            this.gridControlSchedule.Name = "gridControlSchedule";
            this.gridControlSchedule.Size = new System.Drawing.Size(1666, 697);
            this.gridControlSchedule.TabIndex = 13;
            this.gridControlSchedule.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSchedule});
            this.gridControlSchedule.Click += new System.EventHandler(this.gridControlSchedule_Click);
            // 
            // scheduleBindingSource
            // 
            this.scheduleBindingSource.DataSource = typeof(ProjectLibrary.Schedule);
            // 
            // gridViewSchedule
            // 
            this.gridViewSchedule.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colscheduleProjectGuid,
            this.colscheduleName,
            this.colscheduleDescription,
            this.colscheduleDiscipline,
            this.colGUID,
            this.colCreatedDate,
            this.colCreatedBy});
            this.gridViewSchedule.DetailHeight = 699;
            this.gridViewSchedule.FixedLineWidth = 3;
            this.gridViewSchedule.GridControl = this.gridControlSchedule;
            this.gridViewSchedule.Name = "gridViewSchedule";
            this.gridViewSchedule.OptionsSelection.EnableAppearanceFocusedCell = false;
            // 
            // colscheduleProjectGuid
            // 
            this.colscheduleProjectGuid.FieldName = "scheduleProjectGuid";
            this.colscheduleProjectGuid.MinWidth = 40;
            this.colscheduleProjectGuid.Name = "colscheduleProjectGuid";
            this.colscheduleProjectGuid.Width = 151;
            // 
            // colscheduleName
            // 
            this.colscheduleName.Caption = "Name";
            this.colscheduleName.FieldName = "scheduleName";
            this.colscheduleName.MinWidth = 40;
            this.colscheduleName.Name = "colscheduleName";
            this.colscheduleName.OptionsColumn.AllowEdit = false;
            this.colscheduleName.Visible = true;
            this.colscheduleName.VisibleIndex = 0;
            this.colscheduleName.Width = 241;
            // 
            // colscheduleDescription
            // 
            this.colscheduleDescription.Caption = "Description";
            this.colscheduleDescription.FieldName = "scheduleDescription";
            this.colscheduleDescription.MinWidth = 40;
            this.colscheduleDescription.Name = "colscheduleDescription";
            this.colscheduleDescription.OptionsColumn.AllowEdit = false;
            this.colscheduleDescription.Visible = true;
            this.colscheduleDescription.VisibleIndex = 1;
            this.colscheduleDescription.Width = 327;
            // 
            // colscheduleDiscipline
            // 
            this.colscheduleDiscipline.FieldName = "scheduleDiscipline";
            this.colscheduleDiscipline.MinWidth = 40;
            this.colscheduleDiscipline.Name = "colscheduleDiscipline";
            this.colscheduleDiscipline.Width = 151;
            // 
            // colGUID
            // 
            this.colGUID.FieldName = "GUID";
            this.colGUID.MinWidth = 40;
            this.colGUID.Name = "colGUID";
            this.colGUID.Width = 151;
            // 
            // colCreatedDate
            // 
            this.colCreatedDate.FieldName = "CreatedDate";
            this.colCreatedDate.MinWidth = 40;
            this.colCreatedDate.Name = "colCreatedDate";
            this.colCreatedDate.OptionsColumn.AllowEdit = false;
            this.colCreatedDate.Visible = true;
            this.colCreatedDate.VisibleIndex = 2;
            this.colCreatedDate.Width = 161;
            // 
            // colCreatedBy
            // 
            this.colCreatedBy.FieldName = "CreatedBy";
            this.colCreatedBy.MinWidth = 40;
            this.colCreatedBy.Name = "colCreatedBy";
            this.colCreatedBy.Width = 151;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.cmbDiscipline);
            this.panelControl1.Controls.Add(this.labelControl5);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(3, 460);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(7);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1666, 55);
            this.panelControl1.TabIndex = 12;
            // 
            // cmbDiscipline
            // 
            this.cmbDiscipline.Location = new System.Drawing.Point(204, 4);
            this.cmbDiscipline.Margin = new System.Windows.Forms.Padding(0);
            this.cmbDiscipline.Name = "cmbDiscipline";
            this.cmbDiscipline.Properties.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbDiscipline.Properties.Appearance.Options.UseFont = true;
            this.cmbDiscipline.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbDiscipline.Properties.DropDownRows = 20;
            this.cmbDiscipline.Properties.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.cmbDiscipline.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.cmbDiscipline.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbDiscipline.Size = new System.Drawing.Size(1004, 46);
            this.cmbDiscipline.TabIndex = 10;
            this.cmbDiscipline.EditValueChanged += new System.EventHandler(this.cmbDiscipline_EditValueChanged);
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl5.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelControl5.Location = new System.Drawing.Point(3, 3);
            this.labelControl5.Margin = new System.Windows.Forms.Padding(7);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(200, 49);
            this.labelControl5.TabIndex = 9;
            this.labelControl5.Text = "Select Discipline";
            // 
            // groupBoxAll
            // 
            this.groupBoxAll.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxAll.Controls.Add(this.flowLayoutPanel3);
            this.groupBoxAll.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxAll.Location = new System.Drawing.Point(3, 238);
            this.groupBoxAll.Name = "groupBoxAll";
            this.groupBoxAll.Size = new System.Drawing.Size(1666, 222);
            this.groupBoxAll.TabIndex = 16;
            this.groupBoxAll.TabStop = false;
            this.groupBoxAll.Text = "Buttons for All Schedules";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.simpleButton8);
            this.flowLayoutPanel3.Controls.Add(this.btnStatusOnly);
            this.flowLayoutPanel3.Controls.Add(this.btnMasterReport);
            this.flowLayoutPanel3.Controls.Add(this.btnITRStatusBreakdownReport);
            this.flowLayoutPanel3.Controls.Add(this.btnDisciplineMasterReport);
            this.flowLayoutPanel3.Controls.Add(this.btnTrimWBS);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 29);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(1660, 190);
            this.flowLayoutPanel3.TabIndex = 0;
            // 
            // simpleButton8
            // 
            this.simpleButton8.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.simpleButton8.Appearance.Options.UseFont = true;
            this.simpleButton8.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton8.ImageOptions.Image")));
            this.simpleButton8.Location = new System.Drawing.Point(7, 7);
            this.simpleButton8.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.simpleButton8.LookAndFeel.UseDefaultLookAndFeel = false;
            this.simpleButton8.Margin = new System.Windows.Forms.Padding(7);
            this.simpleButton8.Name = "simpleButton8";
            this.simpleButton8.Padding = new System.Windows.Forms.Padding(7);
            this.simpleButton8.Size = new System.Drawing.Size(200, 80);
            this.simpleButton8.TabIndex = 4;
            this.simpleButton8.Text = "Master";
            this.simpleButton8.ToolTip = "Maintain all schedules in a single sheet";
            this.simpleButton8.Click += new System.EventHandler(this.btnMasterSheet_Click);
            // 
            // btnStatusOnly
            // 
            this.btnStatusOnly.Appearance.Font = new System.Drawing.Font("Candara", 9.75F);
            this.btnStatusOnly.Appearance.Options.UseFont = true;
            this.btnStatusOnly.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnStatusOnly.ImageOptions.Image")));
            this.btnStatusOnly.Location = new System.Drawing.Point(221, 7);
            this.btnStatusOnly.Margin = new System.Windows.Forms.Padding(7);
            this.btnStatusOnly.Name = "btnStatusOnly";
            this.btnStatusOnly.Size = new System.Drawing.Size(275, 80);
            this.btnStatusOnly.TabIndex = 9;
            this.btnStatusOnly.Text = "All Schedule Status";
            this.btnStatusOnly.ToolTip = "Show status of all ITRs transposed";
            this.btnStatusOnly.Click += new System.EventHandler(this.btnStatusOnly_Click);
            // 
            // btnMasterReport
            // 
            this.btnMasterReport.Appearance.Font = new System.Drawing.Font("Candara", 9.75F);
            this.btnMasterReport.Appearance.Options.UseFont = true;
            this.btnMasterReport.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnMasterReport.ImageOptions.Image")));
            this.btnMasterReport.Location = new System.Drawing.Point(510, 7);
            this.btnMasterReport.Margin = new System.Windows.Forms.Padding(7);
            this.btnMasterReport.Name = "btnMasterReport";
            this.btnMasterReport.Size = new System.Drawing.Size(227, 80);
            this.btnMasterReport.TabIndex = 10;
            this.btnMasterReport.Text = "Master Report";
            this.btnMasterReport.ToolTip = "Show status of ITRs in all schedules";
            this.btnMasterReport.Click += new System.EventHandler(this.btnMasterReport_Click);
            // 
            // btnITRStatusBreakdownReport
            // 
            this.btnITRStatusBreakdownReport.Appearance.Font = new System.Drawing.Font("Candara", 9.75F);
            this.btnITRStatusBreakdownReport.Appearance.Options.UseFont = true;
            this.btnITRStatusBreakdownReport.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnITRStatusBreakdownReport.ImageOptions.Image")));
            this.btnITRStatusBreakdownReport.Location = new System.Drawing.Point(751, 7);
            this.btnITRStatusBreakdownReport.Margin = new System.Windows.Forms.Padding(7);
            this.btnITRStatusBreakdownReport.Name = "btnITRStatusBreakdownReport";
            this.btnITRStatusBreakdownReport.Size = new System.Drawing.Size(350, 80);
            this.btnITRStatusBreakdownReport.TabIndex = 11;
            this.btnITRStatusBreakdownReport.Text = "Master ITR Status Report";
            this.btnITRStatusBreakdownReport.ToolTip = "Show status of ITRs in all schedules";
            this.btnITRStatusBreakdownReport.Click += new System.EventHandler(this.btnITRStatusBreakdownReport_Click);
            // 
            // btnDisciplineMasterReport
            // 
            this.btnDisciplineMasterReport.Appearance.Font = new System.Drawing.Font("Candara", 9.75F);
            this.btnDisciplineMasterReport.Appearance.Options.UseFont = true;
            this.btnDisciplineMasterReport.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDisciplineMasterReport.ImageOptions.Image")));
            this.btnDisciplineMasterReport.Location = new System.Drawing.Point(1115, 7);
            this.btnDisciplineMasterReport.Margin = new System.Windows.Forms.Padding(7);
            this.btnDisciplineMasterReport.Name = "btnDisciplineMasterReport";
            this.btnDisciplineMasterReport.Size = new System.Drawing.Size(350, 80);
            this.btnDisciplineMasterReport.TabIndex = 12;
            this.btnDisciplineMasterReport.Text = "Discipline Master Report";
            this.btnDisciplineMasterReport.ToolTip = "Shows associated assets for each subsystems";
            this.btnDisciplineMasterReport.Click += new System.EventHandler(this.btnDisciplineMasterReport_Click);
            // 
            // btnTrimWBS
            // 
            this.btnTrimWBS.Appearance.Font = new System.Drawing.Font("Candara", 9.75F);
            this.btnTrimWBS.Appearance.Options.UseFont = true;
            this.btnTrimWBS.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTrimWBS.ImageOptions.Image")));
            this.btnTrimWBS.Location = new System.Drawing.Point(7, 101);
            this.btnTrimWBS.Margin = new System.Windows.Forms.Padding(7);
            this.btnTrimWBS.Name = "btnTrimWBS";
            this.btnTrimWBS.Size = new System.Drawing.Size(271, 80);
            this.btnTrimWBS.TabIndex = 7;
            this.btnTrimWBS.Text = "Trim Unused WBS";
            this.btnTrimWBS.ToolTip = "Delete WBS that doesn\'t have Tag, ITRs or Punchlists";
            this.btnTrimWBS.Click += new System.EventHandler(this.btnTrimWBS_Click);
            // 
            // groupBoxIndividual
            // 
            this.groupBoxIndividual.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxIndividual.Controls.Add(this.flowLayoutPanel2);
            this.groupBoxIndividual.Controls.Add(this.flowLayoutPanel1);
            this.groupBoxIndividual.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxIndividual.Location = new System.Drawing.Point(3, 3);
            this.groupBoxIndividual.Name = "groupBoxIndividual";
            this.groupBoxIndividual.Size = new System.Drawing.Size(1666, 235);
            this.groupBoxIndividual.TabIndex = 15;
            this.groupBoxIndividual.TabStop = false;
            this.groupBoxIndividual.Text = "Buttons for Individual Schedules";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.simpleButton9);
            this.flowLayoutPanel2.Controls.Add(this.btnImportMasterSheet);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel2.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 128);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(7);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(7);
            this.flowLayoutPanel2.Size = new System.Drawing.Size(1660, 101);
            this.flowLayoutPanel2.TabIndex = 10;
            // 
            // simpleButton9
            // 
            this.simpleButton9.Appearance.Font = new System.Drawing.Font("Candara", 9.75F);
            this.simpleButton9.Appearance.Options.UseFont = true;
            this.simpleButton9.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton9.ImageOptions.Image")));
            this.simpleButton9.Location = new System.Drawing.Point(14, 14);
            this.simpleButton9.Margin = new System.Windows.Forms.Padding(7);
            this.simpleButton9.Name = "simpleButton9";
            this.simpleButton9.Size = new System.Drawing.Size(200, 80);
            this.simpleButton9.TabIndex = 6;
            this.simpleButton9.Text = "Schedule";
            this.simpleButton9.ToolTip = "Maintain only selected schedule";
            this.simpleButton9.Click += new System.EventHandler(this.btnScheduleSheet_Click);
            // 
            // btnImportMasterSheet
            // 
            this.btnImportMasterSheet.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportMasterSheet.Appearance.Options.UseFont = true;
            this.btnImportMasterSheet.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImportMasterSheet.ImageOptions.Image")));
            this.btnImportMasterSheet.Location = new System.Drawing.Point(228, 14);
            this.btnImportMasterSheet.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.btnImportMasterSheet.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnImportMasterSheet.Margin = new System.Windows.Forms.Padding(7);
            this.btnImportMasterSheet.Name = "btnImportMasterSheet";
            this.btnImportMasterSheet.Padding = new System.Windows.Forms.Padding(7);
            this.btnImportMasterSheet.Size = new System.Drawing.Size(413, 80);
            this.btnImportMasterSheet.TabIndex = 8;
            this.btnImportMasterSheet.Text = "Import To Selected Schedule";
            this.btnImportMasterSheet.ToolTip = "Import from excel to selected schedle";
            this.btnImportMasterSheet.Click += new System.EventHandler(this.btnImportMasterSheet_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.simpleButton1);
            this.flowLayoutPanel1.Controls.Add(this.simpleButton2);
            this.flowLayoutPanel1.Controls.Add(this.simpleButton3);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 29);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(7);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(7);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1660, 99);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // simpleButton1
            // 
            this.simpleButton1.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.simpleButton1.Appearance.Options.UseFont = true;
            this.simpleButton1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton1.ImageOptions.Image")));
            this.simpleButton1.Location = new System.Drawing.Point(14, 14);
            this.simpleButton1.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.simpleButton1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.simpleButton1.Margin = new System.Windows.Forms.Padding(7);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(200, 80);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "Add";
            this.simpleButton1.ToolTip = "Add a new schedule";
            this.simpleButton1.Click += new System.EventHandler(this.btnAddSchedule_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.simpleButton2.Appearance.Options.UseFont = true;
            this.simpleButton2.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton2.ImageOptions.Image")));
            this.simpleButton2.Location = new System.Drawing.Point(228, 14);
            this.simpleButton2.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.simpleButton2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.simpleButton2.Margin = new System.Windows.Forms.Padding(7);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Padding = new System.Windows.Forms.Padding(7);
            this.simpleButton2.Size = new System.Drawing.Size(200, 80);
            this.simpleButton2.TabIndex = 1;
            this.simpleButton2.Text = "Edit";
            this.simpleButton2.ToolTip = "Edit a selected schedule";
            this.simpleButton2.Click += new System.EventHandler(this.btnEditSchedule_Click);
            // 
            // simpleButton3
            // 
            this.simpleButton3.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.simpleButton3.Appearance.Options.UseFont = true;
            this.simpleButton3.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("simpleButton3.ImageOptions.Image")));
            this.simpleButton3.Location = new System.Drawing.Point(442, 14);
            this.simpleButton3.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.simpleButton3.LookAndFeel.UseDefaultLookAndFeel = false;
            this.simpleButton3.Margin = new System.Windows.Forms.Padding(7);
            this.simpleButton3.Name = "simpleButton3";
            this.simpleButton3.Padding = new System.Windows.Forms.Padding(7);
            this.simpleButton3.Size = new System.Drawing.Size(200, 80);
            this.simpleButton3.TabIndex = 2;
            this.simpleButton3.Text = "Delete";
            this.simpleButton3.ToolTip = "Delete a selected schedule";
            this.simpleButton3.Click += new System.EventHandler(this.btnDeleteSchedule_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmSchedule_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.ClientSize = new System.Drawing.Size(3665, 1295);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "frmSchedule_Main";
            this.Text = "Schedule";
            this.tableLayoutPanelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlTagMain)).EndInit();
            this.pnlTagMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListWBSTag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wbsTagDisplayBindingSource)).EndInit();
            this.flowLayoutPanelTag.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPopupContainerEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpreadsheetCellStyleEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlScheduleMain)).EndInit();
            this.pnlScheduleMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlSchedule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scheduleBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewSchedule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbDiscipline.Properties)).EndInit();
            this.groupBoxAll.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.groupBoxIndividual.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private DevExpress.XtraEditors.PanelControl pnlScheduleMain;
        private DevExpress.XtraEditors.PanelControl pnlTagMain;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelTag;
        private DevExpress.XtraEditors.DropDownButton dropDownButtonAdd;
        private DevExpress.XtraBars.PopupMenu popupMenu1;
        private DevExpress.XtraBars.BarButtonItem bBtnAddTag;
        private DevExpress.XtraBars.BarButtonItem bBtnAddWBS;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.SimpleButton btnEditTagWBS;
        private DevExpress.XtraEditors.SimpleButton bBtnDeleteTagWBS;
        private DevExpress.XtraBars.BarEditItem barEditItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
        private DevExpress.XtraBars.Bar bar1;
        private System.Windows.Forms.BindingSource scheduleBindingSource;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.ComboBoxEdit cmbDiscipline;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraGrid.GridControl gridControlSchedule;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewSchedule;
        private DevExpress.XtraGrid.Columns.GridColumn colscheduleProjectGuid;
        private DevExpress.XtraGrid.Columns.GridColumn colscheduleName;
        private DevExpress.XtraGrid.Columns.GridColumn colscheduleDescription;
        private DevExpress.XtraGrid.Columns.GridColumn colscheduleDiscipline;
        private DevExpress.XtraGrid.Columns.GridColumn colGUID;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedBy;
        private System.Windows.Forms.BindingSource wbsTagDisplayBindingSource;
        private DevExpress.XtraTreeList.TreeList treeListWBSTag;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colwbsTagDisplayName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colwbsTagDisplayDescription;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colwbsTagDisplayAttachTag;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colwbsTagDisplayAttachWBS;
        private System.Windows.Forms.ImageList imageList1;
        private DevExpress.XtraEditors.SimpleButton bBtnAssignTemplate;
        private DevExpress.XtraEditors.SimpleButton bBtnBulkEdit;
        private DevExpress.XtraBars.BarEditItem barEditItem4;
        private DevExpress.XtraSpreadsheet.Design.RepositoryItemSpreadsheetCellStyleEdit repositoryItemSpreadsheetCellStyleEdit1;
        private DevExpress.XtraBars.BarEditItem barEditItem2;
        private DevExpress.XtraEditors.Repository.RepositoryItemPopupContainerEdit repositoryItemPopupContainerEdit1;
        private DevExpress.XtraBars.BarEditItem barEditItem3;
        private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit2;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colwbsTagDisplayType1;
        private DevExpress.XtraEditors.SimpleButton bBtnMatrixAssign;
        private DevExpress.XtraBars.Bar barMenu;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.Bar bar3;
        private DevExpress.XtraBars.Bar barStatus;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colwbsTagDisplayType2;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colwbsTagDisplayType3;
        private DevExpress.XtraEditors.SimpleButton btnImportMasterSheet;
        private DevExpress.XtraEditors.SimpleButton btnDeleteTag;
        private DevExpress.XtraEditors.SimpleButton btnStatusOnly;
        private DevExpress.XtraEditors.SimpleButton simpleButton8;
        private DevExpress.XtraEditors.SimpleButton simpleButton9;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton simpleButton3;
        private DevExpress.XtraEditors.SimpleButton btnMasterReport;
        private DevExpress.XtraEditors.SimpleButton btnTrimWBS;
        private System.Windows.Forms.GroupBox groupBoxAll;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBoxIndividual;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private DevExpress.XtraEditors.SimpleButton btnITRStatusBreakdownReport;
        private DevExpress.XtraEditors.SimpleButton btnDisciplineMasterReport;
    }
}
