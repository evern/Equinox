namespace CheckmateDX
{
    partial class frmITR_Comment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmITR_Comment));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelControlButtons = new DevExpress.XtraEditors.PanelControl();
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.txtComments = new DevExpress.XtraEditors.TextEdit();
            this.lblName = new DevExpress.XtraEditors.LabelControl();
            this.panelControlMain = new DevExpress.XtraEditors.PanelControl();
            this.treeListComments = new DevExpress.XtraTreeList.TreeList();
            this.coliTRCommInfo = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.coliTRCommCreator = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colCreatedDate = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.colCreatedBy = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.iTRCommentsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlButtons)).BeginInit();
            this.panelControlButtons.SuspendLayout();
            this.flowLayoutPanelButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtComments.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlMain)).BeginInit();
            this.panelControlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListComments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.iTRCommentsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panelControlButtons, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panelControlMain, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1176, 543);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panelControlButtons
            // 
            this.panelControlButtons.Controls.Add(this.flowLayoutPanelButtons);
            this.panelControlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlButtons.Location = new System.Drawing.Point(4, 457);
            this.panelControlButtons.Margin = new System.Windows.Forms.Padding(4);
            this.panelControlButtons.Name = "panelControlButtons";
            this.panelControlButtons.Size = new System.Drawing.Size(1168, 82);
            this.panelControlButtons.TabIndex = 0;
            // 
            // flowLayoutPanelButtons
            // 
            this.flowLayoutPanelButtons.Controls.Add(this.btnCancel);
            this.flowLayoutPanelButtons.Controls.Add(this.btnOk);
            this.flowLayoutPanelButtons.Controls.Add(this.txtComments);
            this.flowLayoutPanelButtons.Controls.Add(this.lblName);
            this.flowLayoutPanelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanelButtons.Location = new System.Drawing.Point(2, 2);
            this.flowLayoutPanelButtons.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            this.flowLayoutPanelButtons.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.flowLayoutPanelButtons.Size = new System.Drawing.Size(1164, 78);
            this.flowLayoutPanelButtons.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(980, 8);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(180, 62);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.ImageOptions.Image")));
            this.btnOk.Location = new System.Drawing.Point(792, 8);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(180, 62);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&Reject";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(259, 24);
            this.txtComments.Margin = new System.Windows.Forms.Padding(4, 20, 4, 4);
            this.txtComments.Name = "txtComments";
            this.txtComments.Properties.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtComments.Properties.Appearance.Options.UseFont = true;
            this.txtComments.Size = new System.Drawing.Size(525, 30);
            this.txtComments.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Appearance.Options.UseFont = true;
            this.lblName.Appearance.Options.UseTextOptions = true;
            this.lblName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblName.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lblName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblName.Location = new System.Drawing.Point(101, 8);
            this.lblName.Margin = new System.Windows.Forms.Padding(4);
            this.lblName.Name = "lblName";
            this.lblName.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.lblName.Size = new System.Drawing.Size(150, 62);
            this.lblName.TabIndex = 6;
            this.lblName.Text = "Any Comments?";
            // 
            // panelControlMain
            // 
            this.panelControlMain.Controls.Add(this.treeListComments);
            this.panelControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControlMain.Location = new System.Drawing.Point(4, 4);
            this.panelControlMain.Margin = new System.Windows.Forms.Padding(4);
            this.panelControlMain.Name = "panelControlMain";
            this.panelControlMain.Size = new System.Drawing.Size(1168, 445);
            this.panelControlMain.TabIndex = 1;
            // 
            // treeListComments
            // 
            this.treeListComments.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.coliTRCommInfo,
            this.coliTRCommCreator,
            this.colCreatedDate,
            this.colCreatedBy});
            this.treeListComments.DataSource = this.iTRCommentsBindingSource;
            this.treeListComments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListComments.FixedLineWidth = 3;
            this.treeListComments.HorzScrollStep = 4;
            this.treeListComments.ImageIndexFieldName = "iTRCommImageIndex";
            this.treeListComments.KeyFieldName = "GUID";
            this.treeListComments.Location = new System.Drawing.Point(2, 2);
            this.treeListComments.Margin = new System.Windows.Forms.Padding(4);
            this.treeListComments.MinWidth = 30;
            this.treeListComments.Name = "treeListComments";
            this.treeListComments.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.treeListComments.OptionsView.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.Dark;
            this.treeListComments.ParentFieldName = "iTRCommParentGuid";
            this.treeListComments.SelectImageList = this.imageList1;
            this.treeListComments.Size = new System.Drawing.Size(1164, 441);
            this.treeListComments.TabIndex = 0;
            this.treeListComments.TreeLevelWidth = 27;
            // 
            // coliTRCommInfo
            // 
            this.coliTRCommInfo.Caption = "Information";
            this.coliTRCommInfo.FieldName = "iTRCommInfo";
            this.coliTRCommInfo.MinWidth = 30;
            this.coliTRCommInfo.Name = "coliTRCommInfo";
            this.coliTRCommInfo.OptionsColumn.AllowEdit = false;
            this.coliTRCommInfo.Visible = true;
            this.coliTRCommInfo.VisibleIndex = 1;
            this.coliTRCommInfo.Width = 600;
            // 
            // coliTRCommCreator
            // 
            this.coliTRCommCreator.Caption = "Creator";
            this.coliTRCommCreator.FieldName = "iTRCommCreator";
            this.coliTRCommCreator.MinWidth = 49;
            this.coliTRCommCreator.Name = "coliTRCommCreator";
            this.coliTRCommCreator.OptionsColumn.AllowEdit = false;
            this.coliTRCommCreator.Visible = true;
            this.coliTRCommCreator.VisibleIndex = 0;
            this.coliTRCommCreator.Width = 225;
            // 
            // colCreatedDate
            // 
            this.colCreatedDate.FieldName = "CreatedDate";
            this.colCreatedDate.Format.FormatString = "d";
            this.colCreatedDate.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colCreatedDate.MinWidth = 30;
            this.colCreatedDate.Name = "colCreatedDate";
            this.colCreatedDate.OptionsColumn.AllowEdit = false;
            this.colCreatedDate.Visible = true;
            this.colCreatedDate.VisibleIndex = 2;
            this.colCreatedDate.Width = 375;
            // 
            // colCreatedBy
            // 
            this.colCreatedBy.FieldName = "CreatedBy";
            this.colCreatedBy.MinWidth = 30;
            this.colCreatedBy.Name = "colCreatedBy";
            this.colCreatedBy.Width = 208;
            // 
            // iTRCommentsBindingSource
            // 
            this.iTRCommentsBindingSource.DataSource = typeof(ProjectLibrary.iTRComments);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "new-32x32.png");
            this.imageList1.Images.SetKeyName(1, "stock_down-32x32.png");
            this.imageList1.Images.SetKeyName(2, "stock_up-32x32.png");
            this.imageList1.Images.SetKeyName(3, "comment-32x32.png");
            this.imageList1.Images.SetKeyName(4, "exclamation-32x32.png");
            // 
            // frmITR_Comment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.ClientSize = new System.Drawing.Size(1176, 543);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmITR_Comment";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControlButtons)).EndInit();
            this.panelControlButtons.ResumeLayout(false);
            this.flowLayoutPanelButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtComments.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlMain)).EndInit();
            this.panelControlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListComments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.iTRCommentsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.PanelControl panelControlMain;
        private DevExpress.XtraEditors.PanelControl panelControlButtons;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.TextEdit txtComments;
        private DevExpress.XtraEditors.LabelControl lblName;
        private System.Windows.Forms.BindingSource iTRCommentsBindingSource;
        private System.Windows.Forms.ImageList imageList1;
        private DevExpress.XtraTreeList.TreeList treeListComments;
        private DevExpress.XtraTreeList.Columns.TreeListColumn coliTRCommInfo;
        private DevExpress.XtraTreeList.Columns.TreeListColumn coliTRCommCreator;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCreatedDate;
        private DevExpress.XtraTreeList.Columns.TreeListColumn colCreatedBy;

    }
}
