namespace CheckmateDX
{
    partial class frmSync_Status
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSync_Status));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlStatus = new DevExpress.XtraGrid.GridControl();
            this.syncStatusBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridViewStatus = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colSyncStatus_Type = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSyncStatus_Status = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemImageComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
            this.colSyncStatus_Delete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSyncStatus_Upload = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSyncStatus_Download = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSyncStatus_Same = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSyncStatus_Conflict = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSyncStatus_Resolve = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSyncStatus_LastSync = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSyncStatus_Created = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSyncStatus_CreatedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSyncStatus_CurrentPercentage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemProgressBar2 = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
            this.colSyncStatus_OverallPercentage = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemProgressBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
            this.colSyncStatus_CreatedStr = new DevExpress.XtraGrid.Columns.GridColumn();
            this.tSync_Start = new System.Windows.Forms.Timer(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), true, true);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.syncStatusBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.btnPrint, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.gridControlStatus, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 561);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnPrint
            // 
            this.btnPrint.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Appearance.Options.UseFont = true;
            this.btnPrint.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnPrint.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPrint.ImageOptions.Image")));
            this.btnPrint.Location = new System.Drawing.Point(681, 514);
            this.btnPrint.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.btnPrint.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(100, 44);
            this.btnPrint.TabIndex = 6;
            this.btnPrint.Text = "&Print";
            // 
            // gridControlStatus
            // 
            this.gridControlStatus.DataSource = this.syncStatusBindingSource;
            this.gridControlStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlStatus.Location = new System.Drawing.Point(3, 3);
            this.gridControlStatus.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.gridControlStatus.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlStatus.MainView = this.gridViewStatus;
            this.gridControlStatus.Name = "gridControlStatus";
            this.gridControlStatus.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemProgressBar1,
            this.repositoryItemImageComboBox1,
            this.repositoryItemProgressBar2});
            this.gridControlStatus.Size = new System.Drawing.Size(778, 505);
            this.gridControlStatus.TabIndex = 0;
            this.gridControlStatus.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewStatus});
            // 
            // syncStatusBindingSource
            // 
            this.syncStatusBindingSource.DataSource = typeof(ProjectLibrary.Sync_Status);
            // 
            // gridViewStatus
            // 
            this.gridViewStatus.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colSyncStatus_Type,
            this.colSyncStatus_Status,
            this.colSyncStatus_Delete,
            this.colSyncStatus_Upload,
            this.colSyncStatus_Download,
            this.colSyncStatus_Same,
            this.colSyncStatus_Conflict,
            this.colSyncStatus_Resolve,
            this.colSyncStatus_LastSync,
            this.colSyncStatus_Created,
            this.colSyncStatus_CreatedBy,
            this.colSyncStatus_CurrentPercentage,
            this.colSyncStatus_OverallPercentage,
            this.colSyncStatus_CreatedStr});
            this.gridViewStatus.GridControl = this.gridControlStatus;
            this.gridViewStatus.Name = "gridViewStatus";
            this.gridViewStatus.OptionsView.ShowGroupPanel = false;
            // 
            // colSyncStatus_Type
            // 
            this.colSyncStatus_Type.Caption = "Type";
            this.colSyncStatus_Type.FieldName = "SyncStatus_Type";
            this.colSyncStatus_Type.Name = "colSyncStatus_Type";
            this.colSyncStatus_Type.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_Type.Visible = true;
            this.colSyncStatus_Type.VisibleIndex = 0;
            // 
            // colSyncStatus_Status
            // 
            this.colSyncStatus_Status.Caption = "Status";
            this.colSyncStatus_Status.ColumnEdit = this.repositoryItemImageComboBox1;
            this.colSyncStatus_Status.FieldName = "SyncStatus_Status";
            this.colSyncStatus_Status.Name = "colSyncStatus_Status";
            this.colSyncStatus_Status.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_Status.Visible = true;
            this.colSyncStatus_Status.VisibleIndex = 1;
            // 
            // repositoryItemImageComboBox1
            // 
            this.repositoryItemImageComboBox1.AutoHeight = false;
            this.repositoryItemImageComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemImageComboBox1.LargeImages = this.imageCollection1;
            this.repositoryItemImageComboBox1.Name = "repositoryItemImageComboBox1";
            this.repositoryItemImageComboBox1.SmallImages = this.imageCollection1;
            this.repositoryItemImageComboBox1.Sorted = true;
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            this.imageCollection1.InsertGalleryImage("time_16x16.png", "images/scheduling/time_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/scheduling/time_16x16.png"), 0);
            this.imageCollection1.Images.SetKeyName(0, "time_16x16.png");
            this.imageCollection1.InsertGalleryImage("find_16x16.png", "images/find/find_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/find/find_16x16.png"), 1);
            this.imageCollection1.Images.SetKeyName(1, "find_16x16.png");
            this.imageCollection1.InsertGalleryImage("remove_16x16.png", "images/actions/remove_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/remove_16x16.png"), 2);
            this.imageCollection1.Images.SetKeyName(2, "remove_16x16.png");
            this.imageCollection1.InsertGalleryImage("issue_16x16.png", "images/support/issue_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/support/issue_16x16.png"), 3);
            this.imageCollection1.Images.SetKeyName(3, "issue_16x16.png");
            this.imageCollection1.InsertGalleryImage("next_16x16.png", "images/navigation/next_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/navigation/next_16x16.png"), 4);
            this.imageCollection1.Images.SetKeyName(4, "next_16x16.png");
            this.imageCollection1.InsertGalleryImage("previous_16x16.png", "images/navigation/previous_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/navigation/previous_16x16.png"), 5);
            this.imageCollection1.Images.SetKeyName(5, "previous_16x16.png");
            this.imageCollection1.InsertGalleryImage("save_16x16.png", "images/save/save_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/save/save_16x16.png"), 6);
            this.imageCollection1.Images.SetKeyName(6, "save_16x16.png");
            // 
            // colSyncStatus_Delete
            // 
            this.colSyncStatus_Delete.Caption = "Delete";
            this.colSyncStatus_Delete.FieldName = "SyncStatus_Delete";
            this.colSyncStatus_Delete.Name = "colSyncStatus_Delete";
            this.colSyncStatus_Delete.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_Delete.Visible = true;
            this.colSyncStatus_Delete.VisibleIndex = 2;
            // 
            // colSyncStatus_Upload
            // 
            this.colSyncStatus_Upload.Caption = "Upload";
            this.colSyncStatus_Upload.FieldName = "SyncStatus_Upload";
            this.colSyncStatus_Upload.Name = "colSyncStatus_Upload";
            this.colSyncStatus_Upload.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_Upload.Visible = true;
            this.colSyncStatus_Upload.VisibleIndex = 3;
            // 
            // colSyncStatus_Download
            // 
            this.colSyncStatus_Download.Caption = "Download";
            this.colSyncStatus_Download.FieldName = "SyncStatus_Download";
            this.colSyncStatus_Download.Name = "colSyncStatus_Download";
            this.colSyncStatus_Download.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_Download.Visible = true;
            this.colSyncStatus_Download.VisibleIndex = 4;
            // 
            // colSyncStatus_Same
            // 
            this.colSyncStatus_Same.Caption = "Same";
            this.colSyncStatus_Same.FieldName = "SyncStatus_Same";
            this.colSyncStatus_Same.Name = "colSyncStatus_Same";
            this.colSyncStatus_Same.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_Same.Visible = true;
            this.colSyncStatus_Same.VisibleIndex = 5;
            // 
            // colSyncStatus_Conflict
            // 
            this.colSyncStatus_Conflict.Caption = "Conflict";
            this.colSyncStatus_Conflict.FieldName = "SyncStatus_Conflict";
            this.colSyncStatus_Conflict.Name = "colSyncStatus_Conflict";
            this.colSyncStatus_Conflict.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_Conflict.Visible = true;
            this.colSyncStatus_Conflict.VisibleIndex = 6;
            // 
            // colSyncStatus_Resolve
            // 
            this.colSyncStatus_Resolve.Caption = "Resolve";
            this.colSyncStatus_Resolve.FieldName = "SyncStatus_Resolve";
            this.colSyncStatus_Resolve.Name = "colSyncStatus_Resolve";
            this.colSyncStatus_Resolve.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_Resolve.Visible = true;
            this.colSyncStatus_Resolve.VisibleIndex = 7;
            // 
            // colSyncStatus_LastSync
            // 
            this.colSyncStatus_LastSync.Caption = "Sync From Date";
            this.colSyncStatus_LastSync.FieldName = "SyncStatus_LastSynced";
            this.colSyncStatus_LastSync.Name = "colSyncStatus_LastSync";
            this.colSyncStatus_LastSync.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_LastSync.Visible = true;
            this.colSyncStatus_LastSync.VisibleIndex = 8;
            // 
            // colSyncStatus_Created
            // 
            this.colSyncStatus_Created.FieldName = "SyncStatus_Created";
            this.colSyncStatus_Created.Name = "colSyncStatus_Created";
            this.colSyncStatus_Created.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_Created.OptionsColumn.AllowShowHide = false;
            // 
            // colSyncStatus_CreatedBy
            // 
            this.colSyncStatus_CreatedBy.Caption = "User";
            this.colSyncStatus_CreatedBy.FieldName = "SyncStatus_CreatedBy";
            this.colSyncStatus_CreatedBy.Name = "colSyncStatus_CreatedBy";
            this.colSyncStatus_CreatedBy.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_CreatedBy.Visible = true;
            this.colSyncStatus_CreatedBy.VisibleIndex = 9;
            // 
            // colSyncStatus_CurrentPercentage
            // 
            this.colSyncStatus_CurrentPercentage.Caption = "Current Progress";
            this.colSyncStatus_CurrentPercentage.ColumnEdit = this.repositoryItemProgressBar2;
            this.colSyncStatus_CurrentPercentage.FieldName = "SyncStatus_CurrentPercentage";
            this.colSyncStatus_CurrentPercentage.Name = "colSyncStatus_CurrentPercentage";
            this.colSyncStatus_CurrentPercentage.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_CurrentPercentage.Visible = true;
            this.colSyncStatus_CurrentPercentage.VisibleIndex = 10;
            // 
            // repositoryItemProgressBar2
            // 
            this.repositoryItemProgressBar2.Name = "repositoryItemProgressBar2";
            this.repositoryItemProgressBar2.ShowTitle = true;
            this.repositoryItemProgressBar2.Step = 1;
            // 
            // colSyncStatus_OverallPercentage
            // 
            this.colSyncStatus_OverallPercentage.Caption = "Overall Progress";
            this.colSyncStatus_OverallPercentage.ColumnEdit = this.repositoryItemProgressBar1;
            this.colSyncStatus_OverallPercentage.FieldName = "SyncStatus_OverallPercentage";
            this.colSyncStatus_OverallPercentage.Name = "colSyncStatus_OverallPercentage";
            this.colSyncStatus_OverallPercentage.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_OverallPercentage.Visible = true;
            this.colSyncStatus_OverallPercentage.VisibleIndex = 11;
            // 
            // repositoryItemProgressBar1
            // 
            this.repositoryItemProgressBar1.Name = "repositoryItemProgressBar1";
            this.repositoryItemProgressBar1.ShowTitle = true;
            this.repositoryItemProgressBar1.Step = 1;
            // 
            // colSyncStatus_CreatedStr
            // 
            this.colSyncStatus_CreatedStr.Caption = "Completed";
            this.colSyncStatus_CreatedStr.FieldName = "SyncStatus_CreatedStr";
            this.colSyncStatus_CreatedStr.Name = "colSyncStatus_CreatedStr";
            this.colSyncStatus_CreatedStr.OptionsColumn.AllowEdit = false;
            this.colSyncStatus_CreatedStr.OptionsColumn.ReadOnly = true;
            this.colSyncStatus_CreatedStr.Visible = true;
            this.colSyncStatus_CreatedStr.VisibleIndex = 12;
            // 
            // tSync_Start
            // 
            this.tSync_Start.Tick += new System.EventHandler(this.tSync_Start_Tick);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmSync_Status
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.tableLayoutPanel1);
            this.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmSync_Status";
            this.Text = "Sync";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.syncStatusBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraGrid.GridControl gridControlStatus;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewStatus;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.Repository.RepositoryItemProgressBar repositoryItemProgressBar1;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repositoryItemImageComboBox1;
        private DevExpress.Utils.ImageCollection imageCollection1;
        private System.Windows.Forms.Timer tSync_Start;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_Type;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_Status;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_Upload;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_Same;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_Conflict;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_Download;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_Created;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_CreatedBy;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_OverallPercentage;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_CreatedStr;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_Resolve;
        private System.Windows.Forms.BindingSource syncStatusBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_CurrentPercentage;
        private DevExpress.XtraEditors.Repository.RepositoryItemProgressBar repositoryItemProgressBar2;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_Delete;
        private DevExpress.XtraGrid.Columns.GridColumn colSyncStatus_LastSync;
    }
}