namespace CheckmateDX
{
    partial class frmTool_User_ChangeSignature
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTool_User_ChangeSignature));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlContent = new DevExpress.XtraEditors.PanelControl();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bBtnAccept = new DevExpress.XtraBars.BarButtonItem();
            this.bBtnClear = new DevExpress.XtraBars.BarButtonItem();
            this.bBtnCancel = new DevExpress.XtraBars.BarButtonItem();
            this.btnBrowse = new DevExpress.XtraBars.BarButtonItem();
            this.btnExport = new DevExpress.XtraBars.BarButtonItem();
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.bBtnBrowse = new DevExpress.XtraBars.BarButtonItem();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlContent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.pnlContent);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 44);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(533, 402);
            this.pnlMain.TabIndex = 2;
            // 
            // pnlContent
            // 
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 0);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(533, 402);
            this.pnlContent.TabIndex = 9;
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1,
            this.bar3});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barStaticItem1,
            this.bBtnAccept,
            this.bBtnClear,
            this.bBtnCancel,
            this.btnBrowse,
            this.btnExport});
            this.barManager1.MaxItemId = 9;
            this.barManager1.StatusBar = this.bar3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.FloatLocation = new System.Drawing.Point(445, 169);
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bBtnAccept),
            new DevExpress.XtraBars.LinkPersistInfo(this.bBtnClear),
            new DevExpress.XtraBars.LinkPersistInfo(this.bBtnCancel),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnBrowse),
            new DevExpress.XtraBars.LinkPersistInfo(this.btnExport)});
            this.bar1.Text = "Tools";
            // 
            // bBtnAccept
            // 
            this.bBtnAccept.Caption = "&Accept";
            this.bBtnAccept.Id = 1;
            this.bBtnAccept.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("bBtnAccept.ImageOptions.Image")));
            this.bBtnAccept.ItemAppearance.Normal.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnAccept.ItemAppearance.Normal.Options.UseFont = true;
            this.bBtnAccept.ItemAppearance.Pressed.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnAccept.ItemAppearance.Pressed.Options.UseFont = true;
            this.bBtnAccept.Name = "bBtnAccept";
            this.bBtnAccept.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.bBtnAccept.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bBtnAccept_ItemClick);
            // 
            // bBtnClear
            // 
            this.bBtnClear.Caption = "C&lear";
            this.bBtnClear.Id = 2;
            this.bBtnClear.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("bBtnClear.ImageOptions.Image")));
            this.bBtnClear.ItemAppearance.Normal.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnClear.ItemAppearance.Normal.Options.UseFont = true;
            this.bBtnClear.ItemAppearance.Pressed.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnClear.ItemAppearance.Pressed.Options.UseFont = true;
            this.bBtnClear.Name = "bBtnClear";
            this.bBtnClear.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.bBtnClear.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bBtnClear_ItemClick);
            // 
            // bBtnCancel
            // 
            this.bBtnCancel.Caption = "&Cancel";
            this.bBtnCancel.Id = 3;
            this.bBtnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("bBtnCancel.ImageOptions.Image")));
            this.bBtnCancel.ItemAppearance.Normal.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnCancel.ItemAppearance.Normal.Options.UseFont = true;
            this.bBtnCancel.ItemAppearance.Pressed.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBtnCancel.ItemAppearance.Pressed.Options.UseFont = true;
            this.bBtnCancel.Name = "bBtnCancel";
            this.bBtnCancel.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.bBtnCancel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bBtnCancel_ItemClick);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Caption = "Browse";
            this.btnBrowse.Id = 6;
            this.btnBrowse.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowse.ImageOptions.Image")));
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnBrowse.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bBtnBrowse_ItemClick);
            // 
            // btnExport
            // 
            this.btnExport.Caption = "Export";
            this.btnExport.Id = 8;
            this.btnExport.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnExport.ImageOptions.Image")));
            this.btnExport.Name = "btnExport";
            this.btnExport.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.btnExport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExport_ItemClick);
            // 
            // bar3
            // 
            this.bar3.BarName = "Status bar";
            this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar3.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem1)});
            this.bar3.OptionsBar.AllowQuickCustomization = false;
            this.bar3.OptionsBar.DrawDragBorder = false;
            this.bar3.OptionsBar.UseWholeRow = true;
            this.bar3.Text = "Status bar";
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Caption = "Please Sign in the Empty Spaces Above";
            this.barStaticItem1.Id = 0;
            this.barStaticItem1.Name = "barStaticItem1";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Manager = this.barManager1;
            this.barDockControlTop.Size = new System.Drawing.Size(533, 44);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 446);
            this.barDockControlBottom.Manager = this.barManager1;
            this.barDockControlBottom.Size = new System.Drawing.Size(533, 26);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 44);
            this.barDockControlLeft.Manager = this.barManager1;
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 402);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(533, 44);
            this.barDockControlRight.Manager = this.barManager1;
            this.barDockControlRight.Size = new System.Drawing.Size(0, 402);
            // 
            // bBtnBrowse
            // 
            this.bBtnBrowse.Caption = "&Browse";
            this.bBtnBrowse.Id = 4;
            this.bBtnBrowse.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("bBtnBrowse.ImageOptions.Image")));
            this.bBtnBrowse.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("bBtnBrowse.ImageOptions.LargeImage")));
            this.bBtnBrowse.Name = "bBtnBrowse";
            this.bBtnBrowse.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            this.bBtnBrowse.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.bBtnBrowse.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bBtnBrowse_ItemClick);
            // 
            // frmTool_User_ChangeSignature
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(533, 472);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmTool_User_ChangeSignature";
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlContent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private DevExpress.XtraEditors.PanelControl pnlContent;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.Bar bar3;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bBtnAccept;
        private DevExpress.XtraBars.BarButtonItem bBtnClear;
        private DevExpress.XtraBars.BarButtonItem bBtnCancel;
        private DevExpress.XtraBars.BarButtonItem btnBrowse;
        private DevExpress.XtraBars.BarButtonItem bBtnBrowse;
        private DevExpress.XtraBars.BarButtonItem btnExport;
    }
}
