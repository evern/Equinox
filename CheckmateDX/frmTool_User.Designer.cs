namespace CheckmateDX
{
    partial class frmTool_User
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
            this.userBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colGuid = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserFirstName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserLastName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserQANumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserPassword = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserRole = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserEmail = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserCompany = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserProject = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserDiscipline = new DevExpress.XtraGrid.Columns.GridColumn();
            this.coluserInfo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colselectionMarker = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.userBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // userBindingSource
            // 
            this.userBindingSource.DataSource = typeof(ProjectLibrary.User);
            // 
            // gridControl
            // 
            this.gridControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControl.DataSource = this.userBindingSource;
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.gridControl.Location = new System.Drawing.Point(0, 58);
            this.gridControl.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.gridControl.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControl.MainView = this.gridView1;
            this.gridControl.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.gridControl.MenuManager = this.barManager1;
            this.gridControl.Name = "gridControl";
            this.gridControl.Size = new System.Drawing.Size(2031, 1022);
            this.gridControl.TabIndex = 4;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colGuid,
            this.coluserFirstName,
            this.coluserLastName,
            this.coluserQANumber,
            this.coluserPassword,
            this.coluserRole,
            this.coluserEmail,
            this.coluserCompany,
            this.coluserProject,
            this.coluserDiscipline,
            this.coluserInfo,
            this.colCreatedDate,
            this.colselectionMarker});
            this.gridView1.DetailHeight = 525;
            this.gridView1.FixedLineWidth = 3;
            this.gridView1.GridControl = this.gridControl;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsSelection.MultiSelect = true;
            // 
            // colGuid
            // 
            this.colGuid.FieldName = "GUID";
            this.colGuid.MinWidth = 30;
            this.colGuid.Name = "colGuid";
            this.colGuid.OptionsColumn.AllowShowHide = false;
            this.colGuid.Width = 112;
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
            this.coluserQANumber.Caption = "Username";
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
            this.coluserPassword.OptionsColumn.AllowShowHide = false;
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
            this.coluserEmail.Caption = "Email";
            this.coluserEmail.FieldName = "userEmail";
            this.coluserEmail.MinWidth = 30;
            this.coluserEmail.Name = "coluserEmail";
            this.coluserEmail.OptionsColumn.AllowEdit = false;
            this.coluserEmail.Visible = true;
            this.coluserEmail.VisibleIndex = 7;
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
            this.coluserCompany.VisibleIndex = 6;
            this.coluserCompany.Width = 112;
            // 
            // coluserProject
            // 
            this.coluserProject.Caption = "Project";
            this.coluserProject.FieldName = "userProject";
            this.coluserProject.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            this.coluserProject.MinWidth = 30;
            this.coluserProject.Name = "coluserProject";
            this.coluserProject.OptionsColumn.AllowEdit = false;
            this.coluserProject.Visible = true;
            this.coluserProject.VisibleIndex = 4;
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
            this.coluserDiscipline.VisibleIndex = 5;
            this.coluserDiscipline.Width = 112;
            // 
            // coluserInfo
            // 
            this.coluserInfo.Caption = "Info";
            this.coluserInfo.FieldName = "userInfo";
            this.coluserInfo.MinWidth = 30;
            this.coluserInfo.Name = "coluserInfo";
            this.coluserInfo.Visible = true;
            this.coluserInfo.VisibleIndex = 8;
            this.coluserInfo.Width = 112;
            // 
            // colCreatedDate
            // 
            this.colCreatedDate.Caption = "Created";
            this.colCreatedDate.FieldName = "CreatedDate";
            this.colCreatedDate.MinWidth = 30;
            this.colCreatedDate.Name = "colCreatedDate";
            this.colCreatedDate.OptionsColumn.AllowEdit = false;
            this.colCreatedDate.Visible = true;
            this.colCreatedDate.VisibleIndex = 9;
            this.colCreatedDate.Width = 112;
            // 
            // colselectionMarker
            // 
            this.colselectionMarker.FieldName = "selectionMarker";
            this.colselectionMarker.MinWidth = 30;
            this.colselectionMarker.Name = "colselectionMarker";
            this.colselectionMarker.OptionsColumn.AllowShowHide = false;
            this.colselectionMarker.Width = 112;
            // 
            // frmTool_User
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.ClientSize = new System.Drawing.Size(2031, 1100);
            this.Controls.Add(this.gridControl);
            this.Margin = new System.Windows.Forms.Padding(843, 183, 843, 183);
            this.Name = "frmTool_User";
            this.Text = "Users";
            this.Controls.SetChildIndex(this.gridControl, 0);
            ((System.ComponentModel.ISupportInitialize)(this.userBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource userBindingSource;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colGuid;
        private DevExpress.XtraGrid.Columns.GridColumn coluserFirstName;
        private DevExpress.XtraGrid.Columns.GridColumn coluserLastName;
        private DevExpress.XtraGrid.Columns.GridColumn coluserQANumber;
        private DevExpress.XtraGrid.Columns.GridColumn coluserPassword;
        private DevExpress.XtraGrid.Columns.GridColumn coluserRole;
        private DevExpress.XtraGrid.Columns.GridColumn coluserEmail;
        private DevExpress.XtraGrid.Columns.GridColumn coluserCompany;
        private DevExpress.XtraGrid.Columns.GridColumn coluserProject;
        private DevExpress.XtraGrid.Columns.GridColumn coluserDiscipline;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate;
        private DevExpress.XtraGrid.Columns.GridColumn colselectionMarker;
        private DevExpress.XtraGrid.Columns.GridColumn coluserInfo;

    }
}
