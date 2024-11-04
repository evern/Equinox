namespace CheckmateDX
{
    partial class frmSync_Manage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSync_Manage));
            this.repositoryItemToggleSwitch1 = new DevExpress.XtraEditors.Repository.RepositoryItemToggleSwitch();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.gridControlClient = new DevExpress.XtraGrid.GridControl();
            this.clientBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridViewClient = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colHWID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIPAddress = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colApproved = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGUID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnHistory = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.treeListOptions = new DevExpress.XtraTreeList.TreeList();
            this.colOptionName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colScope = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colOptionEnabled = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colOptionOneTime = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.syncOptionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemToggleSwitch1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlClient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.clientBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewClient)).BeginInit();
            this.flpButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListOptions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.syncOptionBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // repositoryItemToggleSwitch1
            // 
            this.repositoryItemToggleSwitch1.AutoHeight = false;
            this.repositoryItemToggleSwitch1.Name = "repositoryItemToggleSwitch1";
            this.repositoryItemToggleSwitch1.OffText = "Off";
            this.repositoryItemToggleSwitch1.OnText = "On";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelControl1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(915, 647);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.gridControlClient, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.flpButtons, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(634, 641);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // gridControlClient
            // 
            this.gridControlClient.DataSource = this.clientBindingSource;
            this.gridControlClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlClient.Location = new System.Drawing.Point(3, 3);
            this.gridControlClient.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.gridControlClient.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlClient.MainView = this.gridViewClient;
            this.gridControlClient.Name = "gridControlClient";
            this.gridControlClient.Size = new System.Drawing.Size(628, 585);
            this.gridControlClient.TabIndex = 0;
            this.gridControlClient.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewClient});
            // 
            // clientBindingSource
            // 
            this.clientBindingSource.DataSource = typeof(ProjectLibrary.Client);
            // 
            // gridViewClient
            // 
            this.gridViewClient.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colHWID,
            this.colIPAddress,
            this.colDescription,
            this.colApproved,
            this.colCreatedName,
            this.colGUID,
            this.colCreatedDate,
            this.colCreatedBy});
            this.gridViewClient.GridControl = this.gridControlClient;
            this.gridViewClient.Name = "gridViewClient";
            this.gridViewClient.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.EditFormInplaceHideCurrentRow;
            this.gridViewClient.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
            this.gridViewClient.OptionsEditForm.ActionOnModifiedRowChange = DevExpress.XtraGrid.Views.Grid.EditFormModifiedAction.Nothing;
            this.gridViewClient.OptionsView.ShowGroupPanel = false;
            this.gridViewClient.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewClient_CellValueChanged);
            this.gridViewClient.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridViewClient_MouseUp);
            // 
            // colHWID
            // 
            this.colHWID.FieldName = "HWID";
            this.colHWID.Name = "colHWID";
            this.colHWID.OptionsColumn.AllowEdit = false;
            this.colHWID.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
            this.colHWID.Visible = true;
            this.colHWID.VisibleIndex = 0;
            // 
            // colIPAddress
            // 
            this.colIPAddress.FieldName = "IPAddress";
            this.colIPAddress.Name = "colIPAddress";
            this.colIPAddress.OptionsColumn.AllowEdit = false;
            // 
            // colDescription
            // 
            this.colDescription.FieldName = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 1;
            // 
            // colApproved
            // 
            this.colApproved.FieldName = "Approved";
            this.colApproved.Name = "colApproved";
            this.colApproved.Visible = true;
            this.colApproved.VisibleIndex = 2;
            // 
            // colCreatedName
            // 
            this.colCreatedName.FieldName = "CreatedName";
            this.colCreatedName.Name = "colCreatedName";
            this.colCreatedName.OptionsColumn.AllowEdit = false;
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
            // 
            // colCreatedBy
            // 
            this.colCreatedBy.FieldName = "CreatedBy";
            this.colCreatedBy.Name = "colCreatedBy";
            this.colCreatedBy.OptionsColumn.AllowEdit = false;
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnDelete);
            this.flpButtons.Controls.Add(this.btnHistory);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpButtons.Location = new System.Drawing.Point(3, 594);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flpButtons.Size = new System.Drawing.Size(628, 44);
            this.flpButtons.TabIndex = 1;
            // 
            // btnDelete
            // 
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.Image")));
            this.btnDelete.Location = new System.Drawing.Point(525, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 41);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnHistory
            // 
            this.btnHistory.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHistory.Appearance.Options.UseFont = true;
            this.btnHistory.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnHistory.ImageOptions.Image")));
            this.btnHistory.Location = new System.Drawing.Point(419, 3);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(100, 41);
            this.btnHistory.TabIndex = 5;
            this.btnHistory.Text = "&History";
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.treeListOptions);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(643, 3);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(269, 641);
            this.panelControl1.TabIndex = 1;
            // 
            // treeListOptions
            // 
            this.treeListOptions.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colOptionName,
            this.colScope,
            this.colOptionEnabled,
            this.colOptionOneTime});
            this.treeListOptions.DataSource = this.syncOptionBindingSource;
            this.treeListOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListOptions.KeyFieldName = "OptionID";
            this.treeListOptions.Location = new System.Drawing.Point(2, 2);
            this.treeListOptions.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.treeListOptions.LookAndFeel.UseDefaultLookAndFeel = false;
            this.treeListOptions.Name = "treeListOptions";
            this.treeListOptions.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.treeListOptions.OptionsView.CheckBoxStyle = DevExpress.XtraTreeList.DefaultNodeCheckBoxStyle.Check;
            this.treeListOptions.ParentFieldName = "OptionParentID";
            this.treeListOptions.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.treeListOptions.Size = new System.Drawing.Size(265, 637);
            this.treeListOptions.TabIndex = 0;
            this.treeListOptions.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.treeListOptions_AfterCheckNode);
            // 
            // colOptionName
            // 
            this.colOptionName.Caption = "Name";
            this.colOptionName.FieldName = "OptionName";
            this.colOptionName.MinWidth = 32;
            this.colOptionName.Name = "colOptionName";
            this.colOptionName.OptionsColumn.AllowEdit = false;
            this.colOptionName.Visible = true;
            this.colOptionName.VisibleIndex = 0;
            this.colOptionName.Width = 49;
            // 
            // colScope
            // 
            this.colScope.Caption = "Scope";
            this.colScope.FieldName = "OptionScope";
            this.colScope.Name = "colScope";
            this.colScope.OptionsColumn.AllowEdit = false;
            this.colScope.Visible = true;
            this.colScope.VisibleIndex = 2;
            // 
            // colOptionEnabled
            // 
            this.colOptionEnabled.Caption = "Enabled";
            this.colOptionEnabled.ColumnEdit = this.repositoryItemToggleSwitch1;
            this.colOptionEnabled.FieldName = "OptionEnabled";
            this.colOptionEnabled.Name = "colOptionEnabled";
            this.colOptionEnabled.Width = 49;
            // 
            // colOptionOneTime
            // 
            this.colOptionOneTime.Caption = "Once";
            this.colOptionOneTime.ColumnEdit = this.repositoryItemCheckEdit1;
            this.colOptionOneTime.FieldName = "OptionOneTime";
            this.colOptionOneTime.Name = "colOptionOneTime";
            this.colOptionOneTime.Visible = true;
            this.colOptionOneTime.VisibleIndex = 1;
            this.colOptionOneTime.Width = 50;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.repositoryItemCheckEdit1_EditValueChanging);
            // 
            // syncOptionBindingSource
            // 
            this.syncOptionBindingSource.DataSource = typeof(ProjectLibrary.SyncOption);
            // 
            // frmSync_Manage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 647);
            this.Controls.Add(this.tableLayoutPanel1);
            this.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmSync_Manage";
            this.Text = "Sync Manager";
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemToggleSwitch1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlClient)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.clientBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewClient)).EndInit();
            this.flpButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListOptions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.syncOptionBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private DevExpress.XtraGrid.GridControl gridControlClient;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewClient;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraEditors.SimpleButton btnHistory;
        private DevExpress.XtraTreeList.TreeList treeListOptions;
        private System.Windows.Forms.BindingSource clientBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn colHWID;
        private DevExpress.XtraGrid.Columns.GridColumn colIPAddress;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private DevExpress.XtraGrid.Columns.GridColumn colApproved;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedName;
        private DevExpress.XtraGrid.Columns.GridColumn colGUID;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedBy;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colOptionName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colOptionEnabled;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colOptionOneTime;
        private System.Windows.Forms.BindingSource syncOptionBindingSource;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemToggleSwitch repositoryItemToggleSwitch1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colScope;
    }
}