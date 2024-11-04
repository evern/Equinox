namespace CheckmateDX
{
    partial class frmSchedule_AddTag
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSchedule_AddTag));
            this.treeListColumn1 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.flpContent = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlNumber = new System.Windows.Forms.Panel();
            this.txtNumber = new DevExpress.XtraEditors.TextEdit();
            this.lblNumber = new DevExpress.XtraEditors.LabelControl();
            this.pnlDescription = new System.Windows.Forms.Panel();
            this.txtDescription = new DevExpress.XtraEditors.TextEdit();
            this.lblDescription = new DevExpress.XtraEditors.LabelControl();
            this.pnlWBS = new System.Windows.Forms.Panel();
            this.treeListLookUpWBS = new DevExpress.XtraEditors.TreeListLookUpEdit();
            this.wbsTagDisplayBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.treeListLookUpEdit1TreeList = new DevExpress.XtraTreeList.TreeList();
            this.treeListColumn2 = new DevExpress.XtraTreeList.Columns.TreeListColumn();
            this.btnClearWBS = new DevExpress.XtraEditors.SimpleButton();
            this.lblWBS = new DevExpress.XtraEditors.LabelControl();
            this.pnlType = new System.Windows.Forms.Panel();
            this.searchLookUpType1 = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.matrixTypeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.searchLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.lblType = new DevExpress.XtraEditors.LabelControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.searchLookUpType2 = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.searchLookUpType3 = new DevExpress.XtraEditors.SearchLookUpEdit();
            this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.flpContent.SuspendLayout();
            this.pnlNumber.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumber.Properties)).BeginInit();
            this.pnlDescription.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).BeginInit();
            this.pnlWBS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeListLookUpWBS.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wbsTagDisplayBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeListLookUpEdit1TreeList)).BeginInit();
            this.pnlType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpType1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixTypeBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpType2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpType3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "Name";
            this.treeListColumn1.FieldName = "wbsTagDisplayName";
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.OptionsColumn.AllowEdit = false;
            this.treeListColumn1.Visible = true;
            this.treeListColumn1.VisibleIndex = 0;
            this.treeListColumn1.Width = 120;
            // 
            // flpContent
            // 
            this.flpContent.Controls.Add(this.pnlNumber);
            this.flpContent.Controls.Add(this.pnlDescription);
            this.flpContent.Controls.Add(this.pnlWBS);
            this.flpContent.Controls.Add(this.pnlType);
            this.flpContent.Controls.Add(this.panel1);
            this.flpContent.Controls.Add(this.panel2);
            this.flpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpContent.Location = new System.Drawing.Point(0, 0);
            this.flpContent.Margin = new System.Windows.Forms.Padding(4);
            this.flpContent.Name = "flpContent";
            this.flpContent.Padding = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.flpContent.Size = new System.Drawing.Size(480, 423);
            this.flpContent.TabIndex = 1;
            // 
            // pnlNumber
            // 
            this.pnlNumber.Controls.Add(this.txtNumber);
            this.pnlNumber.Controls.Add(this.lblNumber);
            this.pnlNumber.Location = new System.Drawing.Point(4, 16);
            this.pnlNumber.Margin = new System.Windows.Forms.Padding(4);
            this.pnlNumber.Name = "pnlNumber";
            this.pnlNumber.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.pnlNumber.Size = new System.Drawing.Size(472, 50);
            this.pnlNumber.TabIndex = 1;
            // 
            // txtNumber
            // 
            this.txtNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNumber.Location = new System.Drawing.Point(137, 12);
            this.txtNumber.Margin = new System.Windows.Forms.Padding(4);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.Size = new System.Drawing.Size(273, 22);
            this.txtNumber.TabIndex = 1;
            // 
            // lblNumber
            // 
            this.lblNumber.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumber.Appearance.Options.UseFont = true;
            this.lblNumber.Appearance.Options.UseTextOptions = true;
            this.lblNumber.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblNumber.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblNumber.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNumber.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblNumber.Location = new System.Drawing.Point(12, 12);
            this.lblNumber.Margin = new System.Windows.Forms.Padding(4);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblNumber.Size = new System.Drawing.Size(125, 26);
            this.lblNumber.TabIndex = 5;
            this.lblNumber.Text = "Number";
            // 
            // pnlDescription
            // 
            this.pnlDescription.Controls.Add(this.txtDescription);
            this.pnlDescription.Controls.Add(this.lblDescription);
            this.pnlDescription.Location = new System.Drawing.Point(4, 74);
            this.pnlDescription.Margin = new System.Windows.Forms.Padding(4);
            this.pnlDescription.Name = "pnlDescription";
            this.pnlDescription.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.pnlDescription.Size = new System.Drawing.Size(472, 50);
            this.pnlDescription.TabIndex = 2;
            // 
            // txtDescription
            // 
            this.txtDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDescription.Location = new System.Drawing.Point(137, 12);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(4);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(273, 22);
            this.txtDescription.TabIndex = 1;
            // 
            // lblDescription
            // 
            this.lblDescription.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Appearance.Options.UseFont = true;
            this.lblDescription.Appearance.Options.UseTextOptions = true;
            this.lblDescription.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblDescription.Location = new System.Drawing.Point(12, 12);
            this.lblDescription.Margin = new System.Windows.Forms.Padding(4);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblDescription.Size = new System.Drawing.Size(125, 26);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description";
            // 
            // pnlWBS
            // 
            this.pnlWBS.Controls.Add(this.treeListLookUpWBS);
            this.pnlWBS.Controls.Add(this.btnClearWBS);
            this.pnlWBS.Controls.Add(this.lblWBS);
            this.pnlWBS.Location = new System.Drawing.Point(4, 132);
            this.pnlWBS.Margin = new System.Windows.Forms.Padding(4);
            this.pnlWBS.Name = "pnlWBS";
            this.pnlWBS.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.pnlWBS.Size = new System.Drawing.Size(472, 50);
            this.pnlWBS.TabIndex = 3;
            // 
            // treeListLookUpWBS
            // 
            this.treeListLookUpWBS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeListLookUpWBS.EditValue = "TEST";
            this.treeListLookUpWBS.Location = new System.Drawing.Point(137, 12);
            this.treeListLookUpWBS.Margin = new System.Windows.Forms.Padding(4);
            this.treeListLookUpWBS.Name = "treeListLookUpWBS";
            this.treeListLookUpWBS.Properties.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeListLookUpWBS.Properties.Appearance.Options.UseFont = true;
            this.treeListLookUpWBS.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.treeListLookUpWBS.Properties.DataSource = this.wbsTagDisplayBindingSource;
            this.treeListLookUpWBS.Properties.DisplayMember = "wbsTagDisplayName";
            this.treeListLookUpWBS.Properties.NullText = "Select WBS...";
            this.treeListLookUpWBS.Properties.TreeList = this.treeListLookUpEdit1TreeList;
            this.treeListLookUpWBS.Properties.ValueMember = "wbsTagDisplayGuid";
            this.treeListLookUpWBS.Size = new System.Drawing.Size(243, 26);
            this.treeListLookUpWBS.TabIndex = 6;
            // 
            // wbsTagDisplayBindingSource
            // 
            this.wbsTagDisplayBindingSource.DataSource = typeof(ProjectLibrary.wbsTagDisplay);
            // 
            // treeListLookUpEdit1TreeList
            // 
            this.treeListLookUpEdit1TreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1,
            this.treeListColumn2});
            this.treeListLookUpEdit1TreeList.DataSource = this.wbsTagDisplayBindingSource;
            this.treeListLookUpEdit1TreeList.KeyFieldName = "wbsTagDisplayGuid";
            this.treeListLookUpEdit1TreeList.Location = new System.Drawing.Point(-8, 6);
            this.treeListLookUpEdit1TreeList.Name = "treeListLookUpEdit1TreeList";
            this.treeListLookUpEdit1TreeList.OptionsView.ShowIndentAsRowStyle = true;
            this.treeListLookUpEdit1TreeList.ParentFieldName = "wbsTagDisplayParentGuid";
            this.treeListLookUpEdit1TreeList.Size = new System.Drawing.Size(400, 200);
            this.treeListLookUpEdit1TreeList.TabIndex = 0;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.Caption = "Description";
            this.treeListColumn2.FieldName = "wbsTagDisplayDescription";
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.OptionsColumn.AllowEdit = false;
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 1;
            this.treeListColumn2.Width = 150;
            // 
            // btnClearWBS
            // 
            this.btnClearWBS.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearWBS.Appearance.Options.UseFont = true;
            this.btnClearWBS.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClearWBS.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnClearWBS.ImageOptions.Image")));
            this.btnClearWBS.Location = new System.Drawing.Point(380, 12);
            this.btnClearWBS.Margin = new System.Windows.Forms.Padding(4);
            this.btnClearWBS.Name = "btnClearWBS";
            this.btnClearWBS.Size = new System.Drawing.Size(30, 26);
            this.btnClearWBS.TabIndex = 7;
            this.btnClearWBS.Click += new System.EventHandler(this.btnClearWBS_Click);
            // 
            // lblWBS
            // 
            this.lblWBS.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWBS.Appearance.Options.UseFont = true;
            this.lblWBS.Appearance.Options.UseTextOptions = true;
            this.lblWBS.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblWBS.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblWBS.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblWBS.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblWBS.Location = new System.Drawing.Point(12, 12);
            this.lblWBS.Margin = new System.Windows.Forms.Padding(4);
            this.lblWBS.Name = "lblWBS";
            this.lblWBS.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblWBS.Size = new System.Drawing.Size(125, 26);
            this.lblWBS.TabIndex = 5;
            this.lblWBS.Text = "WBS";
            // 
            // pnlType
            // 
            this.pnlType.Controls.Add(this.searchLookUpType1);
            this.pnlType.Controls.Add(this.lblType);
            this.pnlType.Location = new System.Drawing.Point(4, 190);
            this.pnlType.Margin = new System.Windows.Forms.Padding(4);
            this.pnlType.Name = "pnlType";
            this.pnlType.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.pnlType.Size = new System.Drawing.Size(472, 50);
            this.pnlType.TabIndex = 4;
            // 
            // searchLookUpType1
            // 
            this.searchLookUpType1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchLookUpType1.EditValue = "";
            this.searchLookUpType1.Location = new System.Drawing.Point(137, 12);
            this.searchLookUpType1.Margin = new System.Windows.Forms.Padding(4);
            this.searchLookUpType1.Name = "searchLookUpType1";
            this.searchLookUpType1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpType1.Properties.DataSource = this.matrixTypeBindingSource;
            this.searchLookUpType1.Properties.NullText = "";
            this.searchLookUpType1.Properties.View = this.searchLookUpEdit1View;
            this.searchLookUpType1.Size = new System.Drawing.Size(273, 22);
            this.searchLookUpType1.TabIndex = 6;
            // 
            // matrixTypeBindingSource
            // 
            this.matrixTypeBindingSource.DataSource = typeof(ProjectLibrary.MatrixType);
            // 
            // searchLookUpEdit1View
            // 
            this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
            this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // lblType
            // 
            this.lblType.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblType.Appearance.Options.UseFont = true;
            this.lblType.Appearance.Options.UseTextOptions = true;
            this.lblType.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblType.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblType.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblType.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblType.Location = new System.Drawing.Point(12, 12);
            this.lblType.Margin = new System.Windows.Forms.Padding(4);
            this.lblType.Name = "lblType";
            this.lblType.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblType.Size = new System.Drawing.Size(125, 26);
            this.lblType.TabIndex = 5;
            this.lblType.Text = "Type 1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.searchLookUpType2);
            this.panel1.Controls.Add(this.labelControl1);
            this.panel1.Location = new System.Drawing.Point(4, 248);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.panel1.Size = new System.Drawing.Size(472, 50);
            this.panel1.TabIndex = 5;
            // 
            // searchLookUpType2
            // 
            this.searchLookUpType2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchLookUpType2.EditValue = "";
            this.searchLookUpType2.Location = new System.Drawing.Point(137, 12);
            this.searchLookUpType2.Margin = new System.Windows.Forms.Padding(4);
            this.searchLookUpType2.Name = "searchLookUpType2";
            this.searchLookUpType2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpType2.Properties.DataSource = this.matrixTypeBindingSource;
            this.searchLookUpType2.Properties.NullText = "";
            this.searchLookUpType2.Properties.View = this.gridView1;
            this.searchLookUpType2.Size = new System.Drawing.Size(273, 22);
            this.searchLookUpType2.TabIndex = 6;
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Appearance.Options.UseTextOptions = true;
            this.labelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.labelControl1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelControl1.Location = new System.Drawing.Point(12, 12);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.labelControl1.Size = new System.Drawing.Size(125, 26);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "Type 2";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.searchLookUpType3);
            this.panel2.Controls.Add(this.labelControl2);
            this.panel2.Location = new System.Drawing.Point(4, 306);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(12, 12, 62, 12);
            this.panel2.Size = new System.Drawing.Size(472, 50);
            this.panel2.TabIndex = 6;
            // 
            // searchLookUpType3
            // 
            this.searchLookUpType3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchLookUpType3.EditValue = "";
            this.searchLookUpType3.Location = new System.Drawing.Point(137, 12);
            this.searchLookUpType3.Margin = new System.Windows.Forms.Padding(4);
            this.searchLookUpType3.Name = "searchLookUpType3";
            this.searchLookUpType3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.searchLookUpType3.Properties.DataSource = this.matrixTypeBindingSource;
            this.searchLookUpType3.Properties.NullText = "";
            this.searchLookUpType3.Properties.View = this.gridView2;
            this.searchLookUpType3.Size = new System.Drawing.Size(273, 22);
            this.searchLookUpType3.TabIndex = 6;
            // 
            // gridView2
            // 
            this.gridView2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView2.Name = "gridView2";
            this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Appearance.Options.UseTextOptions = true;
            this.labelControl2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.labelControl2.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl2.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelControl2.Location = new System.Drawing.Point(12, 12);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.labelControl2.Size = new System.Drawing.Size(125, 26);
            this.labelControl2.TabIndex = 5;
            this.labelControl2.Text = "Type 3";
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(291, 2);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(125, 52);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnOk.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.ImageOptions.Image")));
            this.btnOk.Location = new System.Drawing.Point(64, 2);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(125, 52);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&Add";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Controls.Add(this.btnOk);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 367);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(4);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(62, 0, 62, 0);
            this.panelControl1.Size = new System.Drawing.Size(480, 56);
            this.panelControl1.TabIndex = 5;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.panelControl1);
            this.pnlMain.Controls.Add(this.flpContent);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(4);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(480, 423);
            this.pnlMain.TabIndex = 6;
            // 
            // frmSchedule_AddTag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.ClientSize = new System.Drawing.Size(480, 423);
            this.Controls.Add(this.pnlMain);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "frmSchedule_AddTag";
            this.Text = "Add Tag";
            this.flpContent.ResumeLayout(false);
            this.pnlNumber.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtNumber.Properties)).EndInit();
            this.pnlDescription.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription.Properties)).EndInit();
            this.pnlWBS.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeListLookUpWBS.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wbsTagDisplayBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.treeListLookUpEdit1TreeList)).EndInit();
            this.pnlType.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpType1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.matrixTypeBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpEdit1View)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpType2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.searchLookUpType3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpContent;
        private System.Windows.Forms.Panel pnlNumber;
        private DevExpress.XtraEditors.TextEdit txtNumber;
        private DevExpress.XtraEditors.LabelControl lblNumber;
        private System.Windows.Forms.Panel pnlDescription;
        private DevExpress.XtraEditors.TextEdit txtDescription;
        private DevExpress.XtraEditors.LabelControl lblDescription;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.BindingSource wbsTagDisplayBindingSource;
        private System.Windows.Forms.Panel pnlWBS;
        private DevExpress.XtraEditors.TreeListLookUpEdit treeListLookUpWBS;
        private DevExpress.XtraTreeList.TreeList treeListLookUpEdit1TreeList;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn1;
        private DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn2;
        private DevExpress.XtraEditors.LabelControl lblWBS;
        private System.Windows.Forms.Panel pnlType;
        private DevExpress.XtraEditors.LabelControl lblType;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpType1;
        private DevExpress.XtraGrid.Views.Grid.GridView searchLookUpEdit1View;
        private System.Windows.Forms.BindingSource matrixTypeBindingSource;
        private DevExpress.XtraEditors.SimpleButton btnClearWBS;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpType2;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraEditors.SearchLookUpEdit searchLookUpType3;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView2;
        private DevExpress.XtraEditors.LabelControl labelControl2;
    }
}
