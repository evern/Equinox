namespace CheckmateDX
{
    partial class frmSync_Status_Online
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSync_Status_Online));
            this.lblSyncServer = new DevExpress.XtraEditors.LabelControl();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.syncStatusBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.colCreatedBy = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colCreatedDate = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colDeletedBy = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colDeletedDateStr = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colDeletedDate = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colUpdatedBy = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.colUpdatedDateStr = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colSyncBy = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colSyncDate = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colstatus = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colsyncItemName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.coladditionalInfo = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.coltype = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.pnlSyncStatus = new DevExpress.XtraEditors.PanelControl();
            this.treeListSyncStatus = new DevExpress.XtraTreeList.TreeList();
            this.colUpdatedDate = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.syncStatusBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSyncStatus)).BeginInit();
            this.pnlSyncStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListSyncStatus)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSyncServer
            // 
            this.lblSyncServer.Appearance.Font = new System.Drawing.Font("Open Sans", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSyncServer.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblSyncServer.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSyncServer.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSyncServer.Location = new System.Drawing.Point(2, 2);
            this.lblSyncServer.Name = "lblSyncServer";
            this.lblSyncServer.Size = new System.Drawing.Size(594, 36);
            this.lblSyncServer.TabIndex = 2;
            this.lblSyncServer.Text = "SYNC";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "stock_down-32x32.png");
            this.imageList1.Images.SetKeyName(1, "stock_up-32x32.png");
            this.imageList1.Images.SetKeyName(2, "Pixelmixer-Basic-Tick.ico");
            this.imageList1.Images.SetKeyName(3, "warning-icon.png");
            // 
            // syncStatusBindingSource
            // 
            this.syncStatusBindingSource.DataSource = typeof(ProjectLibrary.SyncStatus_Superseded);
            // 
            // colCreatedBy
            // 
            this.colCreatedBy.FieldName = "CreatedBy";
            this.colCreatedBy.Name = "colCreatedBy";
            this.colCreatedBy.Width = 64;
            // 
            // colCreatedDate
            // 
            this.colCreatedDate.Caption = "Created";
            this.colCreatedDate.FieldName = "CreatedDate";
            this.colCreatedDate.Format.FormatString = "g";
            this.colCreatedDate.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colCreatedDate.Name = "colCreatedDate";
            this.colCreatedDate.OptionsColumn.AllowEdit = false;
            this.colCreatedDate.Visible = true;
            this.colCreatedDate.VisibleIndex = 5;
            this.colCreatedDate.Width = 96;
            // 
            // colDeletedBy
            // 
            this.colDeletedBy.FieldName = "DeletedBy";
            this.colDeletedBy.Name = "colDeletedBy";
            this.colDeletedBy.Width = 64;
            // 
            // colDeletedDateStr
            // 
            this.colDeletedDateStr.Caption = "Deleted";
            this.colDeletedDateStr.FieldName = "DeletedDateStr";
            this.colDeletedDateStr.Format.FormatString = "g";
            this.colDeletedDateStr.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colDeletedDateStr.Name = "colDeletedDateStr";
            this.colDeletedDateStr.OptionsColumn.AllowEdit = false;
            this.colDeletedDateStr.OptionsColumn.ReadOnly = true;
            this.colDeletedDateStr.Visible = true;
            this.colDeletedDateStr.VisibleIndex = 7;
            this.colDeletedDateStr.Width = 94;
            // 
            // colDeletedDate
            // 
            this.colDeletedDate.FieldName = "DeletedDate";
            this.colDeletedDate.Name = "colDeletedDate";
            this.colDeletedDate.Width = 64;
            // 
            // colUpdatedBy
            // 
            this.colUpdatedBy.FieldName = "UpdatedBy";
            this.colUpdatedBy.Name = "colUpdatedBy";
            this.colUpdatedBy.Width = 63;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // colUpdatedDateStr
            // 
            this.colUpdatedDateStr.Caption = "Updated";
            this.colUpdatedDateStr.FieldName = "UpdatedDateStr";
            this.colUpdatedDateStr.Format.FormatString = "g";
            this.colUpdatedDateStr.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colUpdatedDateStr.Name = "colUpdatedDateStr";
            this.colUpdatedDateStr.OptionsColumn.AllowEdit = false;
            this.colUpdatedDateStr.OptionsColumn.ReadOnly = true;
            this.colUpdatedDateStr.Visible = true;
            this.colUpdatedDateStr.VisibleIndex = 6;
            this.colUpdatedDateStr.Width = 94;
            // 
            // colSyncBy
            // 
            this.colSyncBy.FieldName = "SyncBy";
            this.colSyncBy.Name = "colSyncBy";
            this.colSyncBy.Width = 63;
            // 
            // colSyncDate
            // 
            this.colSyncDate.Caption = "Synced";
            this.colSyncDate.FieldName = "SyncDate";
            this.colSyncDate.Format.FormatString = "g";
            this.colSyncDate.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colSyncDate.Name = "colSyncDate";
            this.colSyncDate.OptionsColumn.AllowEdit = false;
            this.colSyncDate.Visible = true;
            this.colSyncDate.VisibleIndex = 4;
            this.colSyncDate.Width = 95;
            // 
            // colstatus
            // 
            this.colstatus.Caption = "Status";
            this.colstatus.FieldName = "status";
            this.colstatus.Name = "colstatus";
            this.colstatus.OptionsColumn.AllowEdit = false;
            this.colstatus.Visible = true;
            this.colstatus.VisibleIndex = 3;
            this.colstatus.Width = 95;
            // 
            // colsyncItemName
            // 
            this.colsyncItemName.Caption = "Sync Item";
            this.colsyncItemName.FieldName = "syncItemName";
            this.colsyncItemName.Name = "colsyncItemName";
            this.colsyncItemName.OptionsColumn.AllowEdit = false;
            this.colsyncItemName.Visible = true;
            this.colsyncItemName.VisibleIndex = 1;
            this.colsyncItemName.Width = 139;
            // 
            // coladditionalInfo
            // 
            this.coladditionalInfo.Caption = "Info";
            this.coladditionalInfo.FieldName = "additionalInfo";
            this.coladditionalInfo.Name = "coladditionalInfo";
            this.coladditionalInfo.OptionsColumn.AllowEdit = false;
            this.coladditionalInfo.Visible = true;
            this.coladditionalInfo.VisibleIndex = 2;
            this.coladditionalInfo.Width = 132;
            // 
            // coltype
            // 
            this.coltype.Caption = "Type";
            this.coltype.FieldName = "type";
            this.coltype.MinWidth = 33;
            this.coltype.Name = "coltype";
            this.coltype.OptionsColumn.AllowEdit = false;
            this.coltype.Visible = true;
            this.coltype.VisibleIndex = 0;
            this.coltype.Width = 140;
            // 
            // btnPrint
            // 
            this.btnPrint.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Appearance.Options.UseFont = true;
            this.btnPrint.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnPrint.Image = ((System.Drawing.Image)(resources.GetObject("btnPrint.Image")));
            this.btnPrint.Location = new System.Drawing.Point(501, 411);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(100, 44);
            this.btnPrint.TabIndex = 5;
            this.btnPrint.Text = "&Print";
            // 
            // pnlSyncStatus
            // 
            this.pnlSyncStatus.Controls.Add(this.treeListSyncStatus);
            this.pnlSyncStatus.Controls.Add(this.lblSyncServer);
            this.pnlSyncStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSyncStatus.Location = new System.Drawing.Point(3, 3);
            this.pnlSyncStatus.Name = "pnlSyncStatus";
            this.pnlSyncStatus.Size = new System.Drawing.Size(598, 402);
            this.pnlSyncStatus.TabIndex = 1;
            // 
            // treeListSyncStatus
            // 
            this.treeListSyncStatus.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.coltype,
            this.coladditionalInfo,
            this.colsyncItemName,
            this.colstatus,
            this.colSyncDate,
            this.colSyncBy,
            this.colUpdatedDate,
            this.colUpdatedDateStr,
            this.colUpdatedBy,
            this.colDeletedDate,
            this.colDeletedDateStr,
            this.colDeletedBy,
            this.colCreatedDate,
            this.colCreatedBy});
            this.treeListSyncStatus.DataSource = this.syncStatusBindingSource;
            this.treeListSyncStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListSyncStatus.ImageIndexFieldName = "imageIndex";
            this.treeListSyncStatus.KeyFieldName = "GUID";
            this.treeListSyncStatus.Location = new System.Drawing.Point(2, 38);
            this.treeListSyncStatus.Name = "treeListSyncStatus";
            this.treeListSyncStatus.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.treeListSyncStatus.OptionsSelection.MultiSelect = true;
            this.treeListSyncStatus.ParentFieldName = "parentGuid";
            this.treeListSyncStatus.SelectImageList = this.imageList1;
            this.treeListSyncStatus.Size = new System.Drawing.Size(594, 362);
            this.treeListSyncStatus.TabIndex = 15;
            this.treeListSyncStatus.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.Dark;
            // 
            // colUpdatedDate
            // 
            this.colUpdatedDate.FieldName = "UpdatedDate";
            this.colUpdatedDate.Name = "colUpdatedDate";
            this.colUpdatedDate.Width = 63;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.btnPrint, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pnlSyncStatus, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(604, 458);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // frmSync_Status_Online
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 458);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "frmSync_Status_Online";
            this.Text = "frmSync_Status_Online";
            ((System.ComponentModel.ISupportInitialize)(this.syncStatusBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlSyncStatus)).EndInit();
            this.pnlSyncStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListSyncStatus)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblSyncServer;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.BindingSource syncStatusBindingSource;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCreatedBy;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCreatedDate;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colDeletedBy;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colDeletedDateStr;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colDeletedDate;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colUpdatedBy;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colUpdatedDateStr;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colSyncBy;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colSyncDate;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colstatus;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colsyncItemName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn coladditionalInfo;
        private DevExpress.XtraTreeList.Columns.TreeListColumn coltype;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.PanelControl pnlSyncStatus;
        private DevExpress.XtraTreeList.TreeList treeListSyncStatus;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colUpdatedDate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}