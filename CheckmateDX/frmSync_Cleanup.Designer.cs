namespace CheckmateDX
{
    partial class frmSync_Cleanup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSync_Cleanup));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.treeListDatabase = new DevExpress.XtraTreeList.TreeList();
            this.colOptionName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colOptionEnabled = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colOptionOneTime = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colSyncScope = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.repositoryItemProgressBar1 = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnExit = new DevExpress.XtraEditors.SimpleButton();
            this.btnPurge = new DevExpress.XtraEditors.SimpleButton();
            this.btnTrim = new DevExpress.XtraEditors.SimpleButton();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.WaitForm1), true, true);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListDatabase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.treeListDatabase, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(915, 647);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // treeListDatabase
            // 
            this.treeListDatabase.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.colOptionName,
            this.colOptionEnabled,
            this.colOptionOneTime,
            this.colSyncScope});
            this.treeListDatabase.DataSource = this.bindingSource1;
            this.treeListDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListDatabase.KeyFieldName = "OptionID";
            this.treeListDatabase.Location = new System.Drawing.Point(3, 3);
            this.treeListDatabase.Name = "treeListDatabase";
            this.treeListDatabase.OptionsBehavior.AllowRecursiveNodeChecking = true;
            this.treeListDatabase.OptionsView.CheckBoxStyle = DevExpress.XtraTreeList.DefaultNodeCheckBoxStyle.Check;
            this.treeListDatabase.ParentFieldName = "OptionParentID";
            this.treeListDatabase.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemProgressBar1});
            this.treeListDatabase.Size = new System.Drawing.Size(909, 591);
            this.treeListDatabase.TabIndex = 0;
            this.treeListDatabase.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.treeListDatabase_AfterCheckNode);
            // 
            // colOptionName
            // 
            this.colOptionName.Caption = "Database Type";
            this.colOptionName.FieldName = "OptionName";
            this.colOptionName.MinWidth = 32;
            this.colOptionName.Name = "colOptionName";
            this.colOptionName.OptionsColumn.AllowEdit = false;
            this.colOptionName.Visible = true;
            this.colOptionName.VisibleIndex = 0;
            this.colOptionName.Width = 687;
            // 
            // colOptionEnabled
            // 
            this.colOptionEnabled.Caption = "Trim";
            this.colOptionEnabled.FieldName = "OptionEnabled";
            this.colOptionEnabled.Name = "colOptionEnabled";
            this.colOptionEnabled.OptionsColumn.AllowEdit = false;
            this.colOptionEnabled.Width = 108;
            // 
            // colOptionOneTime
            // 
            this.colOptionOneTime.FieldName = "OptionOneTime";
            this.colOptionOneTime.Name = "colOptionOneTime";
            this.colOptionOneTime.OptionsColumn.AllowEdit = false;
            this.colOptionOneTime.Width = 296;
            // 
            // colSyncScope
            // 
            this.colSyncScope.Caption = "Scope";
            this.colSyncScope.FieldName = "OptionScope";
            this.colSyncScope.Name = "colSyncScope";
            this.colSyncScope.OptionsColumn.AllowEdit = false;
            this.colSyncScope.Visible = true;
            this.colSyncScope.VisibleIndex = 1;
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(ProjectLibrary.SyncOption);
            // 
            // repositoryItemProgressBar1
            // 
            this.repositoryItemProgressBar1.Name = "repositoryItemProgressBar1";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnExit);
            this.flowLayoutPanel1.Controls.Add(this.btnPurge);
            this.flowLayoutPanel1.Controls.Add(this.btnTrim);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 600);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(909, 44);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnExit
            // 
            this.btnExit.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Appearance.Options.UseFont = true;
            this.btnExit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.ImageOptions.Image")));
            this.btnExit.Location = new System.Drawing.Point(806, 3);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(100, 41);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "&Close";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnPurge
            // 
            this.btnPurge.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPurge.Appearance.Options.UseFont = true;
            this.btnPurge.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPurge.ImageOptions.Image")));
            this.btnPurge.Location = new System.Drawing.Point(700, 3);
            this.btnPurge.Name = "btnPurge";
            this.btnPurge.Size = new System.Drawing.Size(100, 41);
            this.btnPurge.TabIndex = 7;
            this.btnPurge.Text = "&Purge";
            this.btnPurge.Click += new System.EventHandler(this.btnPurge_Click);
            // 
            // btnTrim
            // 
            this.btnTrim.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTrim.Appearance.Options.UseFont = true;
            this.btnTrim.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTrim.ImageOptions.Image")));
            this.btnTrim.Location = new System.Drawing.Point(594, 3);
            this.btnTrim.Name = "btnTrim";
            this.btnTrim.Size = new System.Drawing.Size(100, 41);
            this.btnTrim.TabIndex = 6;
            this.btnTrim.Text = "&Trim";
            this.btnTrim.Click += new System.EventHandler(this.btnTrim_Click);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // timer1
            // 
            this.timer1.Interval = 1;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmSync_Cleanup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 647);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "frmSync_Cleanup";
            this.Text = "Trim Database";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListDatabase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemProgressBar1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraTreeList.TreeList treeListDatabase;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraEditors.SimpleButton btnExit;
        private DevExpress.XtraEditors.SimpleButton btnTrim;
        private DevExpress.XtraEditors.SimpleButton btnPurge;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colOptionName;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colOptionEnabled;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colOptionOneTime;
        private System.Windows.Forms.BindingSource bindingSource1;
        private DevExpress.XtraEditors.Repository.RepositoryItemProgressBar repositoryItemProgressBar1;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colSyncScope;
        private System.Windows.Forms.Timer timer1;
    }
}