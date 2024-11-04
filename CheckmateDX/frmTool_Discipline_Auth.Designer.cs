namespace CheckmateDX
{
    partial class frmTool_Discipline_Auth
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
            this.gridView3 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colGuid1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserFirstName1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserLastName1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserQANumber1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserPassword1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserRole1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserEmail1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserCompany1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserProject1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserDiscipline1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridUnauth = new DevExpress.XtraGrid.GridControl();
            this.userBindingUnauth = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutProjects = new System.Windows.Forms.TableLayoutPanel();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.gridDiscipline = new DevExpress.XtraGrid.GridControl();
            this.disciplineBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colsimpleName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.pnlMain = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutUsers = new System.Windows.Forms.TableLayoutPanel();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.gridAuth = new DevExpress.XtraGrid.GridControl();
            this.userBindingAuth = new System.Windows.Forms.BindingSource(this.components);
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.coluserFirstName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserLastName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserQANumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserPassword = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserRole = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserEmail = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserCompany = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserProject = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserDiscipline = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserEnabled = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.coluserGuid = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridUnauth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userBindingUnauth)).BeginInit();
            this.tableLayoutProjects.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDiscipline)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.disciplineBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.tableLayoutMain.SuspendLayout();
            this.tableLayoutUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAuth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userBindingAuth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // gridView3
            // 
            this.gridView3.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colGuid1,
            this.coluserFirstName1,
            this.coluserLastName1,
            this.coluserQANumber1,
            this.coluserPassword1,
            this.coluserRole1,
            this.coluserEmail1,
            this.coluserCompany1,
            this.coluserProject1,
            this.coluserDiscipline1,
            this.colCreatedDate1});
            this.gridView3.DetailHeight = 525;
            this.gridView3.FixedLineWidth = 3;
            this.gridView3.GridControl = this.gridUnauth;
            this.gridView3.Name = "gridView3";
            this.gridView3.OptionsSelection.MultiSelect = true;
            this.gridView3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grid_MouseDown);
            this.gridView3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.grid_MouseMove);
            // 
            // colGuid1
            // 
            this.colGuid1.FieldName = "userGuid";
            this.colGuid1.MinWidth = 30;
            this.colGuid1.Name = "colGuid1";
            this.colGuid1.Width = 112;
            // 
            // coluserFirstName1
            // 
            this.coluserFirstName1.Caption = "First Name";
            this.coluserFirstName1.FieldName = "userFirstName";
            this.coluserFirstName1.MinWidth = 30;
            this.coluserFirstName1.Name = "coluserFirstName1";
            this.coluserFirstName1.OptionsColumn.AllowEdit = false;
            this.coluserFirstName1.Visible = true;
            this.coluserFirstName1.VisibleIndex = 1;
            this.coluserFirstName1.Width = 112;
            // 
            // coluserLastName1
            // 
            this.coluserLastName1.Caption = "Last Name";
            this.coluserLastName1.FieldName = "userLastName";
            this.coluserLastName1.MinWidth = 30;
            this.coluserLastName1.Name = "coluserLastName1";
            this.coluserLastName1.OptionsColumn.AllowEdit = false;
            this.coluserLastName1.Visible = true;
            this.coluserLastName1.VisibleIndex = 2;
            this.coluserLastName1.Width = 112;
            // 
            // coluserQANumber1
            // 
            this.coluserQANumber1.Caption = "QA Number";
            this.coluserQANumber1.FieldName = "userQANumber";
            this.coluserQANumber1.MinWidth = 30;
            this.coluserQANumber1.Name = "coluserQANumber1";
            this.coluserQANumber1.OptionsColumn.AllowEdit = false;
            this.coluserQANumber1.Visible = true;
            this.coluserQANumber1.VisibleIndex = 0;
            this.coluserQANumber1.Width = 112;
            // 
            // coluserPassword1
            // 
            this.coluserPassword1.FieldName = "userPassword";
            this.coluserPassword1.MinWidth = 30;
            this.coluserPassword1.Name = "coluserPassword1";
            this.coluserPassword1.Width = 112;
            // 
            // coluserRole1
            // 
            this.coluserRole1.Caption = "Role";
            this.coluserRole1.FieldName = "userRole";
            this.coluserRole1.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            this.coluserRole1.MinWidth = 30;
            this.coluserRole1.Name = "coluserRole1";
            this.coluserRole1.OptionsColumn.AllowEdit = false;
            this.coluserRole1.Visible = true;
            this.coluserRole1.VisibleIndex = 3;
            this.coluserRole1.Width = 112;
            // 
            // coluserEmail1
            // 
            this.coluserEmail1.FieldName = "userEmail";
            this.coluserEmail1.MinWidth = 30;
            this.coluserEmail1.Name = "coluserEmail1";
            this.coluserEmail1.Width = 112;
            // 
            // coluserCompany1
            // 
            this.coluserCompany1.Caption = "Company";
            this.coluserCompany1.FieldName = "userCompany";
            this.coluserCompany1.MinWidth = 30;
            this.coluserCompany1.Name = "coluserCompany1";
            this.coluserCompany1.OptionsColumn.AllowEdit = false;
            this.coluserCompany1.Visible = true;
            this.coluserCompany1.VisibleIndex = 5;
            this.coluserCompany1.Width = 112;
            // 
            // coluserProject1
            // 
            this.coluserProject1.FieldName = "userProject";
            this.coluserProject1.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            this.coluserProject1.MinWidth = 30;
            this.coluserProject1.Name = "coluserProject1";
            this.coluserProject1.Width = 112;
            // 
            // coluserDiscipline1
            // 
            this.coluserDiscipline1.Caption = "Discipline";
            this.coluserDiscipline1.FieldName = "userDiscipline";
            this.coluserDiscipline1.MinWidth = 30;
            this.coluserDiscipline1.Name = "coluserDiscipline1";
            this.coluserDiscipline1.OptionsColumn.AllowEdit = false;
            this.coluserDiscipline1.Visible = true;
            this.coluserDiscipline1.VisibleIndex = 4;
            this.coluserDiscipline1.Width = 112;
            // 
            // colCreatedDate1
            // 
            this.colCreatedDate1.FieldName = "CreatedDate";
            this.colCreatedDate1.MinWidth = 30;
            this.colCreatedDate1.Name = "colCreatedDate1";
            this.colCreatedDate1.Width = 112;
            // 
            // gridUnauth
            // 
            this.gridUnauth.AllowDrop = true;
            this.gridUnauth.DataSource = this.userBindingUnauth;
            this.gridUnauth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridUnauth.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.gridUnauth.Location = new System.Drawing.Point(4, 611);
            this.gridUnauth.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.gridUnauth.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridUnauth.MainView = this.gridView3;
            this.gridUnauth.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gridUnauth.Name = "gridUnauth";
            this.gridUnauth.Size = new System.Drawing.Size(998, 473);
            this.gridUnauth.TabIndex = 5;
            this.gridUnauth.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView3});
            this.gridUnauth.DragDrop += new System.Windows.Forms.DragEventHandler(this.grid_DragDrop);
            this.gridUnauth.DragOver += new System.Windows.Forms.DragEventHandler(this.grid_DragOver);
            // 
            // userBindingUnauth
            // 
            this.userBindingUnauth.DataSource = typeof(ProjectLibrary.User);
            // 
            // tableLayoutProjects
            // 
            this.tableLayoutProjects.ColumnCount = 1;
            this.tableLayoutProjects.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutProjects.Controls.Add(this.labelControl4, 0, 0);
            this.tableLayoutProjects.Controls.Add(this.gridDiscipline, 0, 1);
            this.tableLayoutProjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutProjects.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutProjects.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutProjects.Name = "tableLayoutProjects";
            this.tableLayoutProjects.RowCount = 2;
            this.tableLayoutProjects.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tableLayoutProjects.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 96F));
            this.tableLayoutProjects.Size = new System.Drawing.Size(1005, 1088);
            this.tableLayoutProjects.TabIndex = 3;
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl4.Location = new System.Drawing.Point(4, 4);
            this.labelControl4.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.labelControl4.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(994, 33);
            this.labelControl4.TabIndex = 8;
            this.labelControl4.Text = "Select Discipline to Display Users";
            // 
            // gridDiscipline
            // 
            this.gridDiscipline.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridDiscipline.DataSource = this.disciplineBindingSource;
            this.gridDiscipline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDiscipline.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.gridDiscipline.Location = new System.Drawing.Point(4, 47);
            this.gridDiscipline.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.gridDiscipline.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridDiscipline.MainView = this.gridView1;
            this.gridDiscipline.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gridDiscipline.Name = "gridDiscipline";
            this.gridDiscipline.Size = new System.Drawing.Size(997, 1037);
            this.gridDiscipline.TabIndex = 9;
            this.gridDiscipline.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.gridDiscipline.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gridDiscipline_MouseClick);
            // 
            // disciplineBindingSource
            // 
            this.disciplineBindingSource.DataSource = typeof(ProjectLibrary.Simple);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colsimpleName});
            this.gridView1.DetailHeight = 525;
            this.gridView1.FixedLineWidth = 3;
            this.gridView1.GridControl = this.gridDiscipline;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            // 
            // colsimpleName
            // 
            this.colsimpleName.Caption = "Discipline";
            this.colsimpleName.FieldName = "simpleName";
            this.colsimpleName.MinWidth = 30;
            this.colsimpleName.Name = "colsimpleName";
            this.colsimpleName.OptionsColumn.AllowEdit = false;
            this.colsimpleName.Visible = true;
            this.colsimpleName.VisibleIndex = 0;
            this.colsimpleName.Width = 112;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.tableLayoutMain);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.pnlMain.LookAndFeel.UseDefaultLookAndFeel = false;
            this.pnlMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(2031, 1100);
            this.pnlMain.TabIndex = 1;
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 2;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutMain.Controls.Add(this.tableLayoutUsers, 1, 0);
            this.tableLayoutMain.Controls.Add(this.tableLayoutProjects, 0, 0);
            this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMain.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 1;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutMain.Size = new System.Drawing.Size(2027, 1096);
            this.tableLayoutMain.TabIndex = 0;
            // 
            // tableLayoutUsers
            // 
            this.tableLayoutUsers.ColumnCount = 1;
            this.tableLayoutUsers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutUsers.Controls.Add(this.labelControl3, 0, 3);
            this.tableLayoutUsers.Controls.Add(this.labelControl2, 0, 1);
            this.tableLayoutUsers.Controls.Add(this.gridAuth, 0, 2);
            this.tableLayoutUsers.Controls.Add(this.labelControl1, 0, 0);
            this.tableLayoutUsers.Controls.Add(this.gridUnauth, 0, 4);
            this.tableLayoutUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutUsers.Location = new System.Drawing.Point(1017, 4);
            this.tableLayoutUsers.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutUsers.Name = "tableLayoutUsers";
            this.tableLayoutUsers.RowCount = 5;
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 44F));
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 44F));
            this.tableLayoutUsers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 192F));
            this.tableLayoutUsers.Size = new System.Drawing.Size(1006, 1088);
            this.tableLayoutUsers.TabIndex = 2;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Appearance.Options.UseTextOptions = true;
            this.labelControl3.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.labelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControl3.Location = new System.Drawing.Point(4, 568);
            this.labelControl3.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.labelControl3.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(998, 35);
            this.labelControl3.TabIndex = 9;
            this.labelControl3.Text = "Unauthorised Users";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Appearance.Options.UseTextOptions = true;
            this.labelControl2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelControl2.Location = new System.Drawing.Point(4, 47);
            this.labelControl2.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.labelControl2.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(998, 35);
            this.labelControl2.TabIndex = 8;
            this.labelControl2.Text = "Authorised Users";
            // 
            // gridAuth
            // 
            this.gridAuth.AllowDrop = true;
            this.gridAuth.DataSource = this.userBindingAuth;
            this.gridAuth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAuth.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.gridAuth.Location = new System.Drawing.Point(4, 90);
            this.gridAuth.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.gridAuth.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridAuth.MainView = this.gridView2;
            this.gridAuth.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gridAuth.Name = "gridAuth";
            this.gridAuth.Size = new System.Drawing.Size(998, 470);
            this.gridAuth.TabIndex = 6;
            this.gridAuth.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView2});
            this.gridAuth.DragDrop += new System.Windows.Forms.DragEventHandler(this.grid_DragDrop);
            this.gridAuth.DragOver += new System.Windows.Forms.DragEventHandler(this.grid_DragOver);
            // 
            // userBindingAuth
            // 
            this.userBindingAuth.DataSource = typeof(ProjectLibrary.User);
            // 
            // gridView2
            // 
            this.gridView2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.coluserFirstName,
            this.coluserLastName,
            this.coluserQANumber,
            this.coluserPassword,
            this.coluserRole,
            this.coluserEmail,
            this.coluserCompany,
            this.coluserProject,
            this.coluserDiscipline,
            this.coluserEnabled,
            this.colCreatedDate});
            this.gridView2.DetailHeight = 525;
            this.gridView2.FixedLineWidth = 3;
            this.gridView2.GridControl = this.gridAuth;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsSelection.MultiSelect = true;
            this.gridView2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grid_MouseDown);
            this.gridView2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.grid_MouseMove);
            // 
            // coluserFirstName
            // 
            this.coluserFirstName.Caption = "First Name";
            this.coluserFirstName.FieldName = "userFirstName";
            this.coluserFirstName.MinWidth = 30;
            this.coluserFirstName.Name = "coluserFirstName";
            this.coluserFirstName.OptionsColumn.AllowEdit = false;
            this.coluserFirstName.Visible = true;
            this.coluserFirstName.VisibleIndex = 1;
            this.coluserFirstName.Width = 112;
            // 
            // coluserLastName
            // 
            this.coluserLastName.Caption = "Last Name";
            this.coluserLastName.FieldName = "userLastName";
            this.coluserLastName.MinWidth = 30;
            this.coluserLastName.Name = "coluserLastName";
            this.coluserLastName.OptionsColumn.AllowEdit = false;
            this.coluserLastName.Visible = true;
            this.coluserLastName.VisibleIndex = 2;
            this.coluserLastName.Width = 112;
            // 
            // coluserQANumber
            // 
            this.coluserQANumber.Caption = "QA Number";
            this.coluserQANumber.FieldName = "userQANumber";
            this.coluserQANumber.MinWidth = 30;
            this.coluserQANumber.Name = "coluserQANumber";
            this.coluserQANumber.OptionsColumn.AllowEdit = false;
            this.coluserQANumber.Visible = true;
            this.coluserQANumber.VisibleIndex = 0;
            this.coluserQANumber.Width = 112;
            // 
            // coluserPassword
            // 
            this.coluserPassword.FieldName = "userPassword";
            this.coluserPassword.MinWidth = 30;
            this.coluserPassword.Name = "coluserPassword";
            this.coluserPassword.OptionsColumn.AllowEdit = false;
            this.coluserPassword.Width = 112;
            // 
            // coluserRole
            // 
            this.coluserRole.Caption = "Role";
            this.coluserRole.FieldName = "userRole";
            this.coluserRole.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            this.coluserRole.MinWidth = 30;
            this.coluserRole.Name = "coluserRole";
            this.coluserRole.OptionsColumn.AllowEdit = false;
            this.coluserRole.Visible = true;
            this.coluserRole.VisibleIndex = 3;
            this.coluserRole.Width = 112;
            // 
            // coluserEmail
            // 
            this.coluserEmail.FieldName = "userEmail";
            this.coluserEmail.MinWidth = 30;
            this.coluserEmail.Name = "coluserEmail";
            this.coluserEmail.OptionsColumn.AllowEdit = false;
            this.coluserEmail.Width = 112;
            // 
            // coluserCompany
            // 
            this.coluserCompany.Caption = "Company";
            this.coluserCompany.FieldName = "userCompany";
            this.coluserCompany.MinWidth = 30;
            this.coluserCompany.Name = "coluserCompany";
            this.coluserCompany.OptionsColumn.AllowEdit = false;
            this.coluserCompany.Visible = true;
            this.coluserCompany.VisibleIndex = 5;
            this.coluserCompany.Width = 112;
            // 
            // coluserProject
            // 
            this.coluserProject.FieldName = "userProject";
            this.coluserProject.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            this.coluserProject.MinWidth = 30;
            this.coluserProject.Name = "coluserProject";
            this.coluserProject.OptionsColumn.AllowEdit = false;
            this.coluserProject.Width = 112;
            // 
            // coluserDiscipline
            // 
            this.coluserDiscipline.Caption = "Discipline";
            this.coluserDiscipline.FieldName = "userDiscipline";
            this.coluserDiscipline.MinWidth = 30;
            this.coluserDiscipline.Name = "coluserDiscipline";
            this.coluserDiscipline.OptionsColumn.AllowEdit = false;
            this.coluserDiscipline.Visible = true;
            this.coluserDiscipline.VisibleIndex = 4;
            this.coluserDiscipline.Width = 112;
            // 
            // coluserEnabled
            // 
            this.coluserEnabled.FieldName = "userEnabled";
            this.coluserEnabled.MinWidth = 30;
            this.coluserEnabled.Name = "coluserEnabled";
            this.coluserEnabled.OptionsColumn.AllowEdit = false;
            this.coluserEnabled.Width = 112;
            // 
            // colCreatedDate
            // 
            this.colCreatedDate.FieldName = "CreatedDate";
            this.colCreatedDate.MinWidth = 30;
            this.colCreatedDate.Name = "colCreatedDate";
            this.colCreatedDate.OptionsColumn.AllowEdit = false;
            this.colCreatedDate.Width = 112;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Location = new System.Drawing.Point(4, 4);
            this.labelControl1.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.labelControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.labelControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(994, 33);
            this.labelControl1.TabIndex = 7;
            this.labelControl1.Text = "Drag \'N Drop User(s) Between List";
            // 
            // coluserGuid
            // 
            this.coluserGuid.FieldName = "userGuid";
            this.coluserGuid.Name = "coluserGuid";
            this.coluserGuid.OptionsColumn.AllowEdit = false;
            // 
            // frmTool_Discipline_Auth
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.ClientSize = new System.Drawing.Size(2031, 1100);
            this.Controls.Add(this.pnlMain);
            this.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.Name = "frmTool_Discipline_Auth";
            this.Text = "Discipline Authorisation";
            ((System.ComponentModel.ISupportInitialize)(this.gridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridUnauth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userBindingUnauth)).EndInit();
            this.tableLayoutProjects.ResumeLayout(false);
            this.tableLayoutProjects.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDiscipline)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.disciplineBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.tableLayoutMain.ResumeLayout(false);
            this.tableLayoutUsers.ResumeLayout(false);
            this.tableLayoutUsers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAuth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userBindingAuth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.Views.Grid.GridView gridView3;
        private DevExpress.XtraGrid.Columns.GridColumn colGuid1;
        private DevExpress.XtraGrid.Columns.GridColumn coluserFirstName1;
        private DevExpress.XtraGrid.Columns.GridColumn coluserLastName1;
        private DevExpress.XtraGrid.Columns.GridColumn coluserQANumber1;
        private DevExpress.XtraGrid.Columns.GridColumn coluserPassword1;
        private DevExpress.XtraGrid.Columns.GridColumn coluserRole1;
        private DevExpress.XtraGrid.Columns.GridColumn coluserEmail1;
        private DevExpress.XtraGrid.Columns.GridColumn coluserCompany1;
        private DevExpress.XtraGrid.Columns.GridColumn coluserProject1;
        private DevExpress.XtraGrid.Columns.GridColumn coluserDiscipline1;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate1;
        private DevExpress.XtraGrid.GridControl gridUnauth;
        private System.Windows.Forms.BindingSource userBindingUnauth;
        private System.Windows.Forms.TableLayoutPanel tableLayoutProjects;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.PanelControl pnlMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutUsers;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraGrid.GridControl gridAuth;
        private System.Windows.Forms.BindingSource userBindingAuth;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraGrid.Columns.GridColumn coluserFirstName;
        private DevExpress.XtraGrid.Columns.GridColumn coluserLastName;
        private DevExpress.XtraGrid.Columns.GridColumn coluserQANumber;
        private DevExpress.XtraGrid.Columns.GridColumn coluserPassword;
        private DevExpress.XtraGrid.Columns.GridColumn coluserRole;
        private DevExpress.XtraGrid.Columns.GridColumn coluserEmail;
        private DevExpress.XtraGrid.Columns.GridColumn coluserCompany;
        private DevExpress.XtraGrid.Columns.GridColumn coluserProject;
        private DevExpress.XtraGrid.Columns.GridColumn coluserDiscipline;
        private DevExpress.XtraGrid.Columns.GridColumn coluserEnabled;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.BindingSource disciplineBindingSource;
        private DevExpress.XtraGrid.GridControl gridDiscipline;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colsimpleName;
        private DevExpress.XtraGrid.Columns.GridColumn coluserGuid;
    }
}
