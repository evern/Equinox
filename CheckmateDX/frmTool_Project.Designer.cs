namespace CheckmateDX
{
    partial class frmTool_Project
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTool_Project));
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.projectBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colGuid = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprojectNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprojectName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colprojectClient = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelDialogButtons = new DevExpress.XtraEditors.PanelControl();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelDialogButtons)).BeginInit();
            this.panelDialogButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridControl
            // 
            this.gridControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControl.DataSource = this.projectBindingSource;
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.gridControl.Location = new System.Drawing.Point(0, 58);
            this.gridControl.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.gridControl.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControl.MainView = this.gridView1;
            this.gridControl.Margin = new System.Windows.Forms.Padding(62, 28, 62, 28);
            this.gridControl.MenuManager = this.barManager1;
            this.gridControl.Name = "gridControl";
            this.gridControl.Size = new System.Drawing.Size(2031, 938);
            this.gridControl.TabIndex = 4;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // projectBindingSource
            // 
            this.projectBindingSource.DataSource = typeof(ProjectLibrary.Project);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colGuid,
            this.colprojectNumber,
            this.colprojectName,
            this.colprojectClient,
            this.colCreatedDate});
            this.gridView1.DetailHeight = 525;
            this.gridView1.FixedLineWidth = 3;
            this.gridView1.GridControl = this.gridControl;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsSelection.MultiSelect = true;
            // 
            // colGuid
            // 
            this.colGuid.FieldName = "Guid";
            this.colGuid.MinWidth = 30;
            this.colGuid.Name = "colGuid";
            this.colGuid.Width = 112;
            // 
            // colprojectNumber
            // 
            this.colprojectNumber.Caption = "Number";
            this.colprojectNumber.FieldName = "projectNumber";
            this.colprojectNumber.MinWidth = 30;
            this.colprojectNumber.Name = "colprojectNumber";
            this.colprojectNumber.OptionsColumn.AllowEdit = false;
            this.colprojectNumber.OptionsColumn.ReadOnly = true;
            this.colprojectNumber.Visible = true;
            this.colprojectNumber.VisibleIndex = 0;
            this.colprojectNumber.Width = 225;
            // 
            // colprojectName
            // 
            this.colprojectName.Caption = "Name";
            this.colprojectName.FieldName = "projectName";
            this.colprojectName.MinWidth = 30;
            this.colprojectName.Name = "colprojectName";
            this.colprojectName.OptionsColumn.AllowEdit = false;
            this.colprojectName.OptionsColumn.ReadOnly = true;
            this.colprojectName.Visible = true;
            this.colprojectName.VisibleIndex = 1;
            this.colprojectName.Width = 375;
            // 
            // colprojectClient
            // 
            this.colprojectClient.Caption = "Client";
            this.colprojectClient.FieldName = "projectClient";
            this.colprojectClient.MinWidth = 30;
            this.colprojectClient.Name = "colprojectClient";
            this.colprojectClient.OptionsColumn.AllowEdit = false;
            this.colprojectClient.OptionsColumn.ReadOnly = true;
            this.colprojectClient.Visible = true;
            this.colprojectClient.VisibleIndex = 2;
            this.colprojectClient.Width = 300;
            // 
            // colCreatedDate
            // 
            this.colCreatedDate.Caption = "Created";
            this.colCreatedDate.FieldName = "CreatedDate";
            this.colCreatedDate.MinWidth = 30;
            this.colCreatedDate.Name = "colCreatedDate";
            this.colCreatedDate.OptionsColumn.AllowEdit = false;
            this.colCreatedDate.OptionsColumn.ReadOnly = true;
            this.colCreatedDate.Visible = true;
            this.colCreatedDate.VisibleIndex = 3;
            this.colCreatedDate.Width = 150;
            // 
            // panelDialogButtons
            // 
            this.panelDialogButtons.Controls.Add(this.btnOk);
            this.panelDialogButtons.Controls.Add(this.btnCancel);
            this.panelDialogButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDialogButtons.Location = new System.Drawing.Point(0, 996);
            this.panelDialogButtons.Margin = new System.Windows.Forms.Padding(4);
            this.panelDialogButtons.Name = "panelDialogButtons";
            this.panelDialogButtons.Size = new System.Drawing.Size(2031, 84);
            this.panelDialogButtons.TabIndex = 5;
            this.panelDialogButtons.Visible = false;
            // 
            // btnOk
            // 
            this.btnOk.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnOk.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.ImageOptions.Image")));
            this.btnOk.Location = new System.Drawing.Point(2, 2);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(150, 80);
            this.btnOk.TabIndex = 12;
            this.btnOk.Text = "&Ok";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(1879, 2);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(150, 80);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmTool_Project
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.ClientSize = new System.Drawing.Size(2031, 1100);
            this.Controls.Add(this.gridControl);
            this.Controls.Add(this.panelDialogButtons);
            this.Margin = new System.Windows.Forms.Padding(843, 183, 843, 183);
            this.Name = "frmTool_Project";
            this.Text = "Projects";
            this.Controls.SetChildIndex(this.panelDialogButtons, 0);
            this.Controls.SetChildIndex(this.gridControl, 0);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelDialogButtons)).EndInit();
            this.panelDialogButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.BindingSource projectBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn colGuid;
        private DevExpress.XtraGrid.Columns.GridColumn colprojectNumber;
        private DevExpress.XtraGrid.Columns.GridColumn colprojectName;
        private DevExpress.XtraGrid.Columns.GridColumn colprojectClient;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate;
        private DevExpress.XtraEditors.PanelControl panelDialogButtons;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
    }
}
