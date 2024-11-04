namespace CheckmateDX
{
    partial class frmSync_Manage_Conflict
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSync_Manage_Conflict));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.gridControlData = new DevExpress.XtraGrid.GridControl();
            this.gridViewData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repositoryItemRichTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemRichTextEdit();
            this.repositoryItemImageComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
            this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
            this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
            this.lblData = new DevExpress.XtraEditors.LabelControl();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.gridControlTable = new DevExpress.XtraGrid.GridControl();
            this.conflictTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridViewTable = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colTableName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colConflictCount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblType = new DevExpress.XtraEditors.LabelControl();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnResolve = new DevExpress.XtraEditors.SimpleButton();
            this.btnExpandAll = new DevExpress.XtraEditors.SimpleButton();
            this.btnCollapseAll = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichTextEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageComboBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.conflictTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTable)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panelControl2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelControl3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(915, 647);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.gridControlData);
            this.panelControl2.Controls.Add(this.lblData);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(303, 3);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(609, 591);
            this.panelControl2.TabIndex = 4;
            // 
            // gridControlData
            // 
            this.gridControlData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlData.Location = new System.Drawing.Point(2, 23);
            this.gridControlData.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.gridControlData.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlData.MainView = this.gridViewData;
            this.gridControlData.Name = "gridControlData";
            this.gridControlData.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemRichTextEdit1,
            this.repositoryItemImageComboBox1,
            this.repositoryItemPictureEdit1});
            this.gridControlData.Size = new System.Drawing.Size(605, 566);
            this.gridControlData.TabIndex = 4;
            this.gridControlData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewData});
            // 
            // gridViewData
            // 
            this.gridViewData.GridControl = this.gridControlData;
            this.gridViewData.Name = "gridViewData";
            this.gridViewData.OptionsBehavior.EditingMode = DevExpress.XtraGrid.Views.Grid.GridEditingMode.EditFormInplace;
            this.gridViewData.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.Click;
            this.gridViewData.OptionsBehavior.ReadOnly = true;
            this.gridViewData.OptionsEditForm.ActionOnModifiedRowChange = DevExpress.XtraGrid.Views.Grid.EditFormModifiedAction.Nothing;
            this.gridViewData.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gridViewData_CustomRowCellEditForEditing);
            // 
            // repositoryItemRichTextEdit1
            // 
            this.repositoryItemRichTextEdit1.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("repositoryItemRichTextEdit1.Appearance.Image")));
            this.repositoryItemRichTextEdit1.Appearance.Options.UseImage = true;
            this.repositoryItemRichTextEdit1.DocumentFormat = DevExpress.XtraRichEdit.DocumentFormat.OpenXml;
            this.repositoryItemRichTextEdit1.Name = "repositoryItemRichTextEdit1";
            this.repositoryItemRichTextEdit1.OptionsExport.PlainText.ExportFinalParagraphMark = DevExpress.XtraRichEdit.Export.PlainText.ExportFinalParagraphMark.Never;
            this.repositoryItemRichTextEdit1.ReadOnly = true;
            this.repositoryItemRichTextEdit1.ShowCaretInReadOnly = false;
            // 
            // repositoryItemImageComboBox1
            // 
            this.repositoryItemImageComboBox1.AutoHeight = false;
            this.repositoryItemImageComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemImageComboBox1.LargeImages = this.imageCollection1;
            this.repositoryItemImageComboBox1.Name = "repositoryItemImageComboBox1";
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            this.imageCollection1.InsertGalleryImage("exporttodoc_16x16.png", "images/export/exporttodoc_16x16.png", DevExpress.Images.ImageResourceCache.Default.GetImage("images/export/exporttodoc_16x16.png"), 0);
            this.imageCollection1.Images.SetKeyName(0, "exporttodoc_16x16.png");
            // 
            // repositoryItemPictureEdit1
            // 
            this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
            this.repositoryItemPictureEdit1.ReadOnly = true;
            // 
            // lblData
            // 
            this.lblData.Appearance.Font = new System.Drawing.Font("Candara", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblData.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblData.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblData.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblData.Location = new System.Drawing.Point(2, 2);
            this.lblData.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.lblData.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(605, 21);
            this.lblData.TabIndex = 2;
            this.lblData.Text = "Data";
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.gridControlTable);
            this.panelControl3.Controls.Add(this.lblType);
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl3.Location = new System.Drawing.Point(3, 3);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(294, 591);
            this.panelControl3.TabIndex = 5;
            // 
            // gridControlTable
            // 
            this.gridControlTable.DataSource = this.conflictTableBindingSource;
            this.gridControlTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControlTable.Location = new System.Drawing.Point(2, 23);
            this.gridControlTable.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.gridControlTable.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControlTable.MainView = this.gridViewTable;
            this.gridControlTable.Name = "gridControlTable";
            this.gridControlTable.Size = new System.Drawing.Size(290, 566);
            this.gridControlTable.TabIndex = 3;
            this.gridControlTable.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewTable});
            // 
            // conflictTableBindingSource
            // 
            this.conflictTableBindingSource.DataSource = typeof(ProjectLibrary.Conflict_Table);
            // 
            // gridViewTable
            // 
            this.gridViewTable.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colTableName,
            this.colConflictCount});
            this.gridViewTable.GridControl = this.gridControlTable;
            this.gridViewTable.Name = "gridViewTable";
            this.gridViewTable.OptionsView.ShowGroupPanel = false;
            this.gridViewTable.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridViewTable_MouseUp);
            // 
            // colTableName
            // 
            this.colTableName.FieldName = "TableName";
            this.colTableName.Name = "colTableName";
            this.colTableName.OptionsColumn.AllowEdit = false;
            this.colTableName.Visible = true;
            this.colTableName.VisibleIndex = 0;
            // 
            // colConflictCount
            // 
            this.colConflictCount.FieldName = "ConflictCount";
            this.colConflictCount.Name = "colConflictCount";
            this.colConflictCount.OptionsColumn.AllowEdit = false;
            this.colConflictCount.Visible = true;
            this.colConflictCount.VisibleIndex = 1;
            // 
            // lblType
            // 
            this.lblType.Appearance.Font = new System.Drawing.Font("Candara", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblType.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblType.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblType.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblType.Location = new System.Drawing.Point(2, 2);
            this.lblType.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.lblType.LookAndFeel.UseDefaultLookAndFeel = false;
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(290, 21);
            this.lblType.TabIndex = 2;
            this.lblType.Text = "Table";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnResolve);
            this.flowLayoutPanel1.Controls.Add(this.btnExpandAll);
            this.flowLayoutPanel1.Controls.Add(this.btnCollapseAll);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(303, 600);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(609, 44);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // btnResolve
            // 
            this.btnResolve.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResolve.Appearance.Options.UseFont = true;
            this.btnResolve.Image = ((System.Drawing.Image)(resources.GetObject("btnResolve.Image")));
            this.btnResolve.Location = new System.Drawing.Point(506, 3);
            this.btnResolve.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnResolve.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnResolve.Name = "btnResolve";
            this.btnResolve.Size = new System.Drawing.Size(100, 41);
            this.btnResolve.TabIndex = 7;
            this.btnResolve.Text = "&Resolve";
            this.btnResolve.Click += new System.EventHandler(this.btnResolve_Click);
            // 
            // btnExpandAll
            // 
            this.btnExpandAll.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExpandAll.Appearance.Options.UseFont = true;
            this.btnExpandAll.Image = ((System.Drawing.Image)(resources.GetObject("btnExpandAll.Image")));
            this.btnExpandAll.Location = new System.Drawing.Point(400, 3);
            this.btnExpandAll.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnExpandAll.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnExpandAll.Name = "btnExpandAll";
            this.btnExpandAll.Size = new System.Drawing.Size(100, 41);
            this.btnExpandAll.TabIndex = 8;
            this.btnExpandAll.Text = "&Expand";
            this.btnExpandAll.Click += new System.EventHandler(this.btnExpandAll_Click);
            // 
            // btnCollapseAll
            // 
            this.btnCollapseAll.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCollapseAll.Appearance.Options.UseFont = true;
            this.btnCollapseAll.Image = ((System.Drawing.Image)(resources.GetObject("btnCollapseAll.Image")));
            this.btnCollapseAll.Location = new System.Drawing.Point(294, 3);
            this.btnCollapseAll.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnCollapseAll.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnCollapseAll.Name = "btnCollapseAll";
            this.btnCollapseAll.Size = new System.Drawing.Size(100, 41);
            this.btnCollapseAll.TabIndex = 9;
            this.btnCollapseAll.Text = "&Collapse";
            this.btnCollapseAll.Click += new System.EventHandler(this.btnCollapseAll_Click);
            // 
            // frmSync_Manage_Conflict
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 647);
            this.Controls.Add(this.tableLayoutPanel1);
            this.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.LookAndFeel.UseDefaultLookAndFeel = false;
            this.Name = "frmSync_Manage_Conflict";
            this.Text = "Manage Conflict";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemRichTextEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemImageComboBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemPictureEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControlTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.conflictTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTable)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.LabelControl lblData;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraEditors.LabelControl lblType;
        private DevExpress.XtraGrid.GridControl gridControlTable;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewTable;
        private DevExpress.XtraGrid.GridControl gridControlData;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewData;
        private System.Windows.Forms.BindingSource conflictTableBindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn colTableName;
        private DevExpress.XtraGrid.Columns.GridColumn colConflictCount;
        private DevExpress.XtraEditors.Repository.RepositoryItemRichTextEdit repositoryItemRichTextEdit1;
        private DevExpress.Utils.ImageCollection imageCollection1;
        private DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox repositoryItemImageComboBox1;
        private DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit repositoryItemPictureEdit1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraEditors.SimpleButton btnResolve;
        private DevExpress.XtraEditors.SimpleButton btnExpandAll;
        private DevExpress.XtraEditors.SimpleButton btnCollapseAll;
    }
}