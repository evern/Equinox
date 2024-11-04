namespace CheckmateDX
{
    partial class frmTool_Role
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (_daRole != null)
                {
                    _daRole.Dispose();
                    _daRole = null;
                }
                if (_dsRole != null)
                {
                    _dsRole.Dispose();
                    _dsRole = null;
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTool_Role));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SuperToolTip superToolTip3 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem3 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem3 = new DevExpress.Utils.ToolTipItem();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.barMenu = new DevExpress.XtraBars.Bar();
            this.btnAdd = new DevExpress.XtraBars.BarButtonItem();
            this.btnEdit = new DevExpress.XtraBars.BarButtonItem();
            this.btnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.barStatus = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.tableLayoutRoles = new System.Windows.Forms.TableLayoutPanel();
            this.treeListRole = new DevExpress.XtraTreeList.TreeList();
            this.colroleParentName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colroleName1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colCreatedDate3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colCreatedBy3 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.roleBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.pnlMain = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutUsers = new System.Windows.Forms.TableLayoutPanel();
            this.gridDisabledPrivilege = new DevExpress.XtraGrid.GridControl();
            this.privilegeBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.gridView3 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colprivTypeID2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprivName3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprivCategory1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprivLocked1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGUID2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedBy2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.gridEnabledPrivilege = new DevExpress.XtraGrid.GridControl();
            this.privilegeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colprivTypeID1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprivName1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprivCategory = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprivLocked = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGUID1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedBy1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprivTypeID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprivName2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprivCategory2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGUID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprivName = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.tableLayoutRoles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListRole)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.roleBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.tableLayoutMain.SuspendLayout();
            this.tableLayoutUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDisabledPrivilege)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.privilegeBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridEnabledPrivilege)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.privilegeBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
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
            this.btnEdit,
            this.btnDelete});
            this.barManager1.MaxItemId = 3;
            this.barManager1.StatusBar = this.barStatus;
            // 
            // barMenu
            // 
            this.barMenu.BarName = "Tools";
            this.barMenu.DockCol = 0;
            this.barMenu.DockRow = 0;
            this.barMenu.DockStyle = DevExpress.XtraBars.BarDockStyle.Left;
            this.barMenu.FloatLocation = new System.Drawing.Point(232, 184);
            this.barMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.btnAdd),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnEdit),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnDelete)});
            this.barMenu.Text = "Tools";
            // 
            // btnAdd
            // 
            this.btnAdd.Caption = "Add";
            this.btnAdd.Id = 0;
            this.btnAdd.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.ImageOptions.Image")));
            this.btnAdd.ItemAppearance.Normal.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ItemAppearance.Normal.Options.UseFont = true;
            this.btnAdd.ItemAppearance.Pressed.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ItemAppearance.Pressed.Options.UseFont = true;
            this.btnAdd.Name = "btnAdd";
            toolTipTitleItem1.Text = "Record";
            toolTipItem1.LeftIndent = 6;
            toolTipItem1.Text = "Add Role";
            superToolTip1.Items.Add(toolTipTitleItem1);
            superToolTip1.Items.Add(toolTipItem1);
            this.btnAdd.SuperTip = superToolTip1;
            this.btnAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAdd_ItemClick);
            // 
            // btnEdit
            // 
            this.btnEdit.Caption = "Edit";
            this.btnEdit.Id = 1;
            this.btnEdit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.ImageOptions.Image")));
            this.btnEdit.ItemAppearance.Normal.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.ItemAppearance.Normal.Options.UseFont = true;
            this.btnEdit.ItemAppearance.Pressed.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.ItemAppearance.Pressed.Options.UseFont = true;
            this.btnEdit.Name = "btnEdit";
            toolTipTitleItem2.Text = "Record";
            toolTipItem2.LeftIndent = 6;
            toolTipItem2.Text = "Edit Role";
            superToolTip2.Items.Add(toolTipTitleItem2);
            superToolTip2.Items.Add(toolTipItem2);
            this.btnEdit.SuperTip = superToolTip2;
            this.btnEdit.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnEdit_ItemClick);
            // 
            // btnDelete
            // 
            this.btnDelete.Caption = "Delete";
            this.btnDelete.Id = 2;
            this.btnDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.Image")));
            this.btnDelete.ItemAppearance.Normal.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ItemAppearance.Normal.Options.UseFont = true;
            this.btnDelete.ItemAppearance.Pressed.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.ItemAppearance.Pressed.Options.UseFont = true;
            this.btnDelete.Name = "btnDelete";
            toolTipTitleItem3.Text = "Record";
            toolTipItem3.LeftIndent = 6;
            toolTipItem3.Text = "Delete Role";
            superToolTip3.Items.Add(toolTipTitleItem3);
            superToolTip3.Items.Add(toolTipItem3);
            this.btnDelete.SuperTip = superToolTip3;
            this.btnDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnDelete_ItemClick);
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
            this.barDockControlTop.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.barDockControlTop.Size = new System.Drawing.Size(1176, 0);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 818);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.barDockControlBottom.Size = new System.Drawing.Size(1176, 24);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.barDockControlLeft.Size = new System.Drawing.Size(68, 818);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1176, 0);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 818);
            // 
            // tableLayoutRoles
            // 
            this.tableLayoutRoles.ColumnCount = 1;
            this.tableLayoutRoles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutRoles.Controls.Add(this.treeListRole, 0, 1);
            this.tableLayoutRoles.Controls.Add(this.labelControl4, 0, 0);
            this.tableLayoutRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutRoles.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutRoles.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutRoles.Name = "tableLayoutRoles";
            this.tableLayoutRoles.RowCount = 2;
            this.tableLayoutRoles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutRoles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 95F));
            this.tableLayoutRoles.Size = new System.Drawing.Size(544, 806);
            this.tableLayoutRoles.TabIndex = 3;
            // 
            // treeListRole
            // 
            this.treeListRole.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colroleParentName,
            this.colroleName1,
            this.colCreatedDate3,
            this.colCreatedBy3});
            this.treeListRole.DataSource = this.roleBindingSource;
            this.treeListRole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListRole.FixedLineWidth = 3;
            this.treeListRole.HorzScrollStep = 4;
            this.treeListRole.KeyFieldName = "GUID";
            this.treeListRole.Location = new System.Drawing.Point(4, 44);
            this.treeListRole.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.treeListRole.LookAndFeel.UseDefaultLookAndFeel = false;
            this.treeListRole.Margin = new System.Windows.Forms.Padding(4);
            this.treeListRole.MinWidth = 30;
            this.treeListRole.Name = "treeListRole";
            this.treeListRole.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.treeListRole.OptionsSelection.MultiSelect = true;
            this.treeListRole.ParentFieldName = "roleParentGuid";
            this.treeListRole.Size = new System.Drawing.Size(536, 758);
            this.treeListRole.TabIndex = 0;
            this.treeListRole.TreeLevelWidth = 27;
            this.treeListRole.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeListRoles_FocusedNodeChanged);
            // 
            // colroleParentName
            // 
            this.colroleParentName.Caption = "Parent";
            this.colroleParentName.FieldName = "roleParentName";
            this.colroleParentName.MinWidth = 30;
            this.colroleParentName.Name = "colroleParentName";
            this.colroleParentName.OptionsColumn.AllowEdit = false;
            this.colroleParentName.Width = 120;
            // 
            // colroleName1
            // 
            this.colroleName1.Caption = "Name";
            this.colroleName1.FieldName = "roleName";
            this.colroleName1.MinWidth = 30;
            this.colroleName1.Name = "colroleName1";
            this.colroleName1.OptionsColumn.AllowEdit = false;
            this.colroleName1.Visible = true;
            this.colroleName1.VisibleIndex = 0;
            this.colroleName1.Width = 342;
            // 
            // colCreatedDate3
            // 
            this.colCreatedDate3.FieldName = "CreatedDate";
            this.colCreatedDate3.MinWidth = 30;
            this.colCreatedDate3.Name = "colCreatedDate3";
            this.colCreatedDate3.Width = 88;
            // 
            // colCreatedBy3
            // 
            this.colCreatedBy3.FieldName = "CreatedBy";
            this.colCreatedBy3.MinWidth = 30;
            this.colCreatedBy3.Name = "colCreatedBy3";
            this.colCreatedBy3.Width = 88;
            // 
            // roleBindingSource
            // 
            this.roleBindingSource.DataSource = typeof(ProjectLibrary.Role);
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControl4.Location = new System.Drawing.Point(4, 4);
            this.labelControl4.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.labelControl4.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl4.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(536, 32);
            this.labelControl4.TabIndex = 8;
            this.labelControl4.Text = "Select Role to Display Privilege";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.tableLayoutMain);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(68, 0);
            this.pnlMain.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.pnlMain.LookAndFeel.UseDefaultLookAndFeel = false;
            this.pnlMain.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1108, 818);
            this.pnlMain.TabIndex = 4;
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 2;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutMain.Controls.Add(this.tableLayoutUsers, 1, 0);
            this.tableLayoutMain.Controls.Add(this.tableLayoutRoles, 0, 0);
            this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMain.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutMain.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 1;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutMain.Size = new System.Drawing.Size(1104, 814);
            this.tableLayoutMain.TabIndex = 0;
            // 
            // tableLayoutUsers
            // 
            this.tableLayoutUsers.ColumnCount = 1;
            this.tableLayoutUsers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutUsers.Controls.Add(this.gridDisabledPrivilege, 0, 4);
            this.tableLayoutUsers.Controls.Add(this.labelControl3, 0, 3);
            this.tableLayoutUsers.Controls.Add(this.labelControl2, 0, 1);
            this.tableLayoutUsers.Controls.Add(this.labelControl1, 0, 0);
            this.tableLayoutUsers.Controls.Add(this.gridEnabledPrivilege, 0, 2);
            this.tableLayoutUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutUsers.Location = new System.Drawing.Point(556, 4);
            this.tableLayoutUsers.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutUsers.Name = "tableLayoutUsers";
            this.tableLayoutUsers.RowCount = 5;
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.5F));
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.5F));
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutUsers.Size = new System.Drawing.Size(544, 806);
            this.tableLayoutUsers.TabIndex = 2;
            // 
            // gridDisabledPrivilege
            // 
            this.gridDisabledPrivilege.AllowDrop = true;
            this.gridDisabledPrivilege.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridDisabledPrivilege.DataSource = this.privilegeBindingSource1;
            this.gridDisabledPrivilege.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDisabledPrivilege.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            this.gridDisabledPrivilege.Location = new System.Drawing.Point(4, 466);
            this.gridDisabledPrivilege.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.gridDisabledPrivilege.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridDisabledPrivilege.MainView = this.gridView3;
            this.gridDisabledPrivilege.Margin = new System.Windows.Forms.Padding(4);
            this.gridDisabledPrivilege.MenuManager = this.barManager1;
            this.gridDisabledPrivilege.Name = "gridDisabledPrivilege";
            this.gridDisabledPrivilege.Size = new System.Drawing.Size(536, 336);
            this.gridDisabledPrivilege.TabIndex = 11;
            this.gridDisabledPrivilege.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView3});
            this.gridDisabledPrivilege.DragDrop += new System.Windows.Forms.DragEventHandler(this.grid_DragDrop);
            this.gridDisabledPrivilege.DragOver += new System.Windows.Forms.DragEventHandler(this.grid_DragOver);
            // 
            // privilegeBindingSource1
            // 
            this.privilegeBindingSource1.DataSource = typeof(ProjectLibrary.Privilege);
            // 
            // gridView3
            // 
            this.gridView3.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colprivTypeID2,
            this.colprivName3,
            this.colprivCategory1,
            this.colprivLocked1,
            this.colGUID2,
            this.colCreatedDate2,
            this.colCreatedBy2});
            this.gridView3.DetailHeight = 525;
            this.gridView3.FixedLineWidth = 3;
            this.gridView3.GridControl = this.gridDisabledPrivilege;
            this.gridView3.GroupCount = 1;
            this.gridView3.Name = "gridView3";
            this.gridView3.OptionsSelection.MultiSelect = true;
            this.gridView3.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colprivCategory1, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.gridView3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grid_MouseDown);
            this.gridView3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.grid_MouseMove);
            // 
            // colprivTypeID2
            // 
            this.colprivTypeID2.FieldName = "privTypeID";
            this.colprivTypeID2.MinWidth = 30;
            this.colprivTypeID2.Name = "colprivTypeID2";
            this.colprivTypeID2.Width = 112;
            // 
            // colprivName3
            // 
            this.colprivName3.Caption = "Name";
            this.colprivName3.FieldName = "privName";
            this.colprivName3.MinWidth = 30;
            this.colprivName3.Name = "colprivName3";
            this.colprivName3.OptionsColumn.AllowEdit = false;
            this.colprivName3.Visible = true;
            this.colprivName3.VisibleIndex = 0;
            this.colprivName3.Width = 243;
            // 
            // colprivCategory1
            // 
            this.colprivCategory1.Caption = "Category";
            this.colprivCategory1.FieldName = "privCategory";
            this.colprivCategory1.MinWidth = 30;
            this.colprivCategory1.Name = "colprivCategory1";
            this.colprivCategory1.OptionsColumn.AllowEdit = false;
            this.colprivCategory1.Visible = true;
            this.colprivCategory1.VisibleIndex = 0;
            this.colprivCategory1.Width = 168;
            // 
            // colprivLocked1
            // 
            this.colprivLocked1.Caption = "Locked";
            this.colprivLocked1.FieldName = "privLocked";
            this.colprivLocked1.MinWidth = 30;
            this.colprivLocked1.Name = "colprivLocked1";
            // 
            // colGUID2
            // 
            this.colGUID2.FieldName = "GUID";
            this.colGUID2.MinWidth = 30;
            this.colGUID2.Name = "colGUID2";
            this.colGUID2.Width = 112;
            // 
            // colCreatedDate2
            // 
            this.colCreatedDate2.FieldName = "CreatedDate";
            this.colCreatedDate2.MinWidth = 30;
            this.colCreatedDate2.Name = "colCreatedDate2";
            this.colCreatedDate2.Width = 112;
            // 
            // colCreatedBy2
            // 
            this.colCreatedBy2.FieldName = "CreatedBy";
            this.colCreatedBy2.MinWidth = 30;
            this.colCreatedBy2.Name = "colCreatedBy2";
            this.colCreatedBy2.Width = 112;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Appearance.Options.UseTextOptions = true;
            this.labelControl3.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.labelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControl3.Location = new System.Drawing.Point(4, 426);
            this.labelControl3.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.labelControl3.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl3.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(536, 32);
            this.labelControl3.TabIndex = 9;
            this.labelControl3.Text = "Disabled Privilege";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Appearance.Options.UseTextOptions = true;
            this.labelControl2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControl2.Location = new System.Drawing.Point(4, 44);
            this.labelControl2.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.labelControl2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl2.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(536, 32);
            this.labelControl2.TabIndex = 8;
            this.labelControl2.Text = "Enabled Privilege";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControl1.Location = new System.Drawing.Point(4, 4);
            this.labelControl1.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.labelControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl1.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(536, 32);
            this.labelControl1.TabIndex = 7;
            this.labelControl1.Text = "Drag \'N Drop Privilege(s) Between List";
            // 
            // gridEnabledPrivilege
            // 
            this.gridEnabledPrivilege.AllowDrop = true;
            this.gridEnabledPrivilege.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridEnabledPrivilege.DataSource = this.privilegeBindingSource;
            this.gridEnabledPrivilege.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridEnabledPrivilege.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
            this.gridEnabledPrivilege.Location = new System.Drawing.Point(4, 84);
            this.gridEnabledPrivilege.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.gridEnabledPrivilege.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridEnabledPrivilege.MainView = this.gridView1;
            this.gridEnabledPrivilege.Margin = new System.Windows.Forms.Padding(4);
            this.gridEnabledPrivilege.MenuManager = this.barManager1;
            this.gridEnabledPrivilege.Name = "gridEnabledPrivilege";
            this.gridEnabledPrivilege.Size = new System.Drawing.Size(536, 334);
            this.gridEnabledPrivilege.TabIndex = 12;
            this.gridEnabledPrivilege.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.gridEnabledPrivilege.DragDrop += new System.Windows.Forms.DragEventHandler(this.grid_DragDrop);
            this.gridEnabledPrivilege.DragOver += new System.Windows.Forms.DragEventHandler(this.grid_DragOver);
            // 
            // privilegeBindingSource
            // 
            this.privilegeBindingSource.DataSource = typeof(ProjectLibrary.Privilege);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colprivTypeID1,
            this.colprivName1,
            this.colprivCategory,
            this.colprivLocked,
            this.colGUID1,
            this.colCreatedDate1,
            this.colCreatedBy1});
            this.gridView1.DetailHeight = 525;
            this.gridView1.FixedLineWidth = 3;
            this.gridView1.GridControl = this.gridEnabledPrivilege;
            this.gridView1.GroupCount = 1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.MultiSelect = true;
            this.gridView1.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colprivCategory, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
            this.gridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grid_MouseDown);
            this.gridView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.grid_MouseMove);
            // 
            // colprivTypeID1
            // 
            this.colprivTypeID1.FieldName = "privTypeID";
            this.colprivTypeID1.MinWidth = 30;
            this.colprivTypeID1.Name = "colprivTypeID1";
            this.colprivTypeID1.Width = 126;
            // 
            // colprivName1
            // 
            this.colprivName1.Caption = "Name";
            this.colprivName1.FieldName = "privName";
            this.colprivName1.MinWidth = 30;
            this.colprivName1.Name = "colprivName1";
            this.colprivName1.OptionsColumn.AllowEdit = false;
            this.colprivName1.Visible = true;
            this.colprivName1.VisibleIndex = 0;
            this.colprivName1.Width = 229;
            // 
            // colprivCategory
            // 
            this.colprivCategory.Caption = "Category";
            this.colprivCategory.FieldName = "privCategory";
            this.colprivCategory.MinWidth = 30;
            this.colprivCategory.Name = "colprivCategory";
            this.colprivCategory.OptionsColumn.AllowEdit = false;
            this.colprivCategory.Visible = true;
            this.colprivCategory.VisibleIndex = 0;
            this.colprivCategory.Width = 229;
            // 
            // colprivLocked
            // 
            this.colprivLocked.Caption = "Locked";
            this.colprivLocked.FieldName = "privLocked";
            this.colprivLocked.MinWidth = 30;
            this.colprivLocked.Name = "colprivLocked";
            this.colprivLocked.Visible = true;
            this.colprivLocked.VisibleIndex = 1;
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
            // colprivTypeID
            // 
            this.colprivTypeID.FieldName = "privTypeID";
            this.colprivTypeID.Name = "colprivTypeID";
            // 
            // colprivName2
            // 
            this.colprivName2.FieldName = "privName";
            this.colprivName2.Name = "colprivName2";
            // 
            // colprivCategory2
            // 
            this.colprivCategory2.FieldName = "privCategory";
            this.colprivCategory2.Name = "colprivCategory2";
            // 
            // colGUID
            // 
            this.colGUID.FieldName = "GUID";
            this.colGUID.Name = "colGUID";
            // 
            // colCreatedDate
            // 
            this.colCreatedDate.FieldName = "CreatedDate";
            this.colCreatedDate.Name = "colCreatedDate";
            // 
            // colCreatedBy
            // 
            this.colCreatedBy.FieldName = "CreatedBy";
            this.colCreatedBy.Name = "colCreatedBy";
            // 
            // colprivName
            // 
            this.colprivName.Caption = "Privilege";
            this.colprivName.FieldName = "privName";
            this.colprivName.Name = "colprivName";
            this.colprivName.OptionsColumn.AllowEdit = false;
            this.colprivName.Visible = true;
            this.colprivName.VisibleIndex = 1;
            // 
            // frmTool_Role
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.ClientSize = new System.Drawing.Size(1176, 842);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmTool_Role";
            this.Text = "Role";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.tableLayoutRoles.ResumeLayout(false);
            this.tableLayoutRoles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListRole)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.roleBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.tableLayoutMain.ResumeLayout(false);
            this.tableLayoutUsers.ResumeLayout(false);
            this.tableLayoutUsers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDisabledPrivilege)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.privilegeBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridEnabledPrivilege)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.privilegeBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected DevExpress.XtraBars.BarManager barManager1;
        protected DevExpress.XtraBars.Bar barMenu;
        protected DevExpress.XtraBars.BarButtonItem btnAdd;
        protected DevExpress.XtraBars.BarButtonItem btnEdit;
        protected DevExpress.XtraBars.BarButtonItem btnDelete;
        protected DevExpress.XtraBars.Bar barStatus;
        protected DevExpress.XtraBars.BarDockControl barDockControlTop;
        protected DevExpress.XtraBars.BarDockControl barDockControlBottom;
        protected DevExpress.XtraBars.BarDockControl barDockControlLeft;
        protected DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.PanelControl pnlMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutRoles;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutUsers;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.BindingSource roleBindingSource;
        private DevExpress.XtraTreeList.TreeList treeListRole;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colroleParentName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colroleName1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCreatedDate3;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCreatedBy3;
        private DevExpress.XtraGrid.Columns.GridColumn colprivTypeID;
        private DevExpress.XtraGrid.Columns.GridColumn colprivName2;
        private DevExpress.XtraGrid.Columns.GridColumn colprivCategory2;
        private DevExpress.XtraGrid.Columns.GridColumn colGUID;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedBy;
        private DevExpress.XtraGrid.Columns.GridColumn colprivName;
        private DevExpress.XtraGrid.GridControl gridEnabledPrivilege;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.GridControl gridDisabledPrivilege;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView3;
        private System.Windows.Forms.BindingSource privilegeBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn colprivTypeID1;
        private DevExpress.XtraGrid.Columns.GridColumn colprivName1;
        private DevExpress.XtraGrid.Columns.GridColumn colprivCategory;
        private DevExpress.XtraGrid.Columns.GridColumn colprivLocked;
        private DevExpress.XtraGrid.Columns.GridColumn colGUID1;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate1;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedBy1;
        private System.Windows.Forms.BindingSource privilegeBindingSource1;
        private DevExpress.XtraGrid.Columns.GridColumn colprivTypeID2;
        private DevExpress.XtraGrid.Columns.GridColumn colprivName3;
        private DevExpress.XtraGrid.Columns.GridColumn colprivCategory1;
        private DevExpress.XtraGrid.Columns.GridColumn colprivLocked1;
        private DevExpress.XtraGrid.Columns.GridColumn colGUID2;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate2;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedBy2;
    }
}
