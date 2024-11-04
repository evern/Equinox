using DevExpress.XtraRichEdit;

namespace CheckmateDX
{
    partial class frmITR_Main
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
            DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager2 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, null, true, true);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmITR_Main));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipTitleItem toolTipTitleItem1 = new DevExpress.Utils.ToolTipTitleItem();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            this.pnlMain = new DevExpress.XtraEditors.PanelControl();
            this.pnlITR = new DevExpress.XtraEditors.PanelControl();
            this.customRichEdit1 = new CheckmateDX.CustomRichEdit();
            this.pnlButtons = new DevExpress.XtraEditors.PanelControl();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnReject = new DevExpress.XtraEditors.SimpleButton();
            this.btnProgress = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnPunchlist = new DevExpress.XtraEditors.SimpleButton();
            this.btnBrowsePunchlist = new DevExpress.XtraEditors.SimpleButton();
            this.btnExportToPDF = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnReplacePDF = new DevExpress.XtraEditors.SimpleButton();
            this.btnAppendPDF = new DevExpress.XtraEditors.SimpleButton();
            this.btnAttachImage = new DevExpress.XtraEditors.SimpleButton();
            this.btnAddSignature = new DevExpress.XtraEditors.SimpleButton();
            this.btnInsertSignature = new DevExpress.XtraEditors.SimpleButton();
            this.btnInsertEquipment = new DevExpress.XtraEditors.SimpleButton();
            this.btnInsertAcceptance = new DevExpress.XtraEditors.SimpleButton();
            this.btnInsertEditableSup = new DevExpress.XtraEditors.SimpleButton();
            this.btnInsertEditable = new DevExpress.XtraEditors.SimpleButton();
            this.btnToggleField = new DevExpress.XtraEditors.SimpleButton();
            this.btnUnprotect = new DevExpress.XtraEditors.SimpleButton();
            this.btnCleanUpQR = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlITR)).BeginInit();
            this.pnlITR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // splashScreenManager2
            // 
            splashScreenManager2.ClosingDelay = 500;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.pnlITR);
            this.pnlMain.Controls.Add(this.pnlButtons);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(5);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(3077, 1293);
            this.pnlMain.TabIndex = 0;
            // 
            // pnlITR
            // 
            this.pnlITR.Controls.Add(this.customRichEdit1);
            this.pnlITR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlITR.Location = new System.Drawing.Point(3, 3);
            this.pnlITR.Margin = new System.Windows.Forms.Padding(5);
            this.pnlITR.Name = "pnlITR";
            this.pnlITR.Size = new System.Drawing.Size(3071, 1093);
            this.pnlITR.TabIndex = 1;
            // 
            // customRichEdit1
            // 
            this.customRichEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customRichEdit1.Location = new System.Drawing.Point(3, 3);
            this.customRichEdit1.Margin = new System.Windows.Forms.Padding(5);
            this.customRichEdit1.Name = "customRichEdit1";
            this.customRichEdit1.Options.AutoCorrect.DetectUrls = false;
            this.customRichEdit1.Options.AutoCorrect.ReplaceTextAsYouType = false;
            this.customRichEdit1.Options.Behavior.Touch = DevExpress.XtraRichEdit.DocumentCapability.Enabled;
            this.customRichEdit1.Options.DocumentCapabilities.Bookmarks = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
            this.customRichEdit1.Options.DocumentCapabilities.Comments = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
            this.customRichEdit1.Options.DocumentCapabilities.EndNotes = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
            this.customRichEdit1.Options.DocumentCapabilities.FootNotes = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
            this.customRichEdit1.Options.DocumentCapabilities.Hyperlinks = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
            this.customRichEdit1.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            this.customRichEdit1.Options.Printing.PrintPreviewFormKind = DevExpress.XtraRichEdit.PrintPreviewFormKind.Bars;
            this.customRichEdit1.Options.RangePermissions.Visibility = DevExpress.XtraRichEdit.RichEditRangePermissionVisibility.Visible;
            this.customRichEdit1.Options.SpellChecker.AutoDetectDocumentCulture = false;
            this.customRichEdit1.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            this.customRichEdit1.Size = new System.Drawing.Size(3065, 1087);
            this.customRichEdit1.TabIndex = 0;
            this.customRichEdit1.DocumentLoaded += new System.EventHandler(this.customRichEdit1_DocumentLoaded);
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.flpButtons);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(3, 1096);
            this.pnlButtons.Margin = new System.Windows.Forms.Padding(5);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(3071, 194);
            this.pnlButtons.TabIndex = 0;
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnClose);
            this.flpButtons.Controls.Add(this.btnDelete);
            this.flpButtons.Controls.Add(this.btnReject);
            this.flpButtons.Controls.Add(this.btnProgress);
            this.flpButtons.Controls.Add(this.btnSave);
            this.flpButtons.Controls.Add(this.btnPunchlist);
            this.flpButtons.Controls.Add(this.btnBrowsePunchlist);
            this.flpButtons.Controls.Add(this.btnExportToPDF);
            this.flpButtons.Controls.Add(this.btnPrint);
            this.flpButtons.Controls.Add(this.btnReplacePDF);
            this.flpButtons.Controls.Add(this.btnAppendPDF);
            this.flpButtons.Controls.Add(this.btnAttachImage);
            this.flpButtons.Controls.Add(this.btnAddSignature);
            this.flpButtons.Controls.Add(this.btnInsertSignature);
            this.flpButtons.Controls.Add(this.btnInsertEquipment);
            this.flpButtons.Controls.Add(this.btnInsertAcceptance);
            this.flpButtons.Controls.Add(this.btnInsertEditableSup);
            this.flpButtons.Controls.Add(this.btnInsertEditable);
            this.flpButtons.Controls.Add(this.btnToggleField);
            this.flpButtons.Controls.Add(this.btnUnprotect);
            this.flpButtons.Controls.Add(this.btnCleanUpQR);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpButtons.Location = new System.Drawing.Point(3, 3);
            this.flpButtons.Margin = new System.Windows.Forms.Padding(5);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(3065, 188);
            this.flpButtons.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.ImageOptions.Image")));
            this.btnClose.Location = new System.Drawing.Point(2860, 5);
            this.btnClose.Margin = new System.Windows.Forms.Padding(5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(200, 83);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "&Exit";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.Image")));
            this.btnDelete.Location = new System.Drawing.Point(2650, 5);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(5);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(200, 83);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnReject
            // 
            this.btnReject.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReject.Appearance.Options.UseFont = true;
            this.btnReject.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnReject.ImageOptions.Image")));
            this.btnReject.Location = new System.Drawing.Point(2440, 5);
            this.btnReject.Margin = new System.Windows.Forms.Padding(5);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(200, 83);
            this.btnReject.TabIndex = 9;
            this.btnReject.Text = "&Reject";
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnProgress
            // 
            this.btnProgress.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProgress.Appearance.Options.UseFont = true;
            this.btnProgress.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnProgress.ImageOptions.Image")));
            this.btnProgress.Location = new System.Drawing.Point(2230, 5);
            this.btnProgress.Margin = new System.Windows.Forms.Padding(5);
            this.btnProgress.Name = "btnProgress";
            this.btnProgress.Size = new System.Drawing.Size(200, 83);
            this.btnProgress.TabIndex = 6;
            this.btnProgress.Text = "&Progress";
            this.btnProgress.Click += new System.EventHandler(this.btnProgress_Click);
            // 
            // btnSave
            // 
            this.btnSave.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.ImageOptions.Image")));
            this.btnSave.Location = new System.Drawing.Point(2020, 5);
            this.btnSave.Margin = new System.Windows.Forms.Padding(5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(200, 83);
            toolTipTitleItem1.Text = "Save (Ctrl+S)";
            toolTipItem1.LeftIndent = 6;
            toolTipItem1.Text = "Save the document.";
            superToolTip1.Items.Add(toolTipTitleItem1);
            superToolTip1.Items.Add(toolTipItem1);
            this.btnSave.SuperTip = superToolTip1;
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "&Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnPunchlist
            // 
            this.btnPunchlist.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPunchlist.Appearance.Options.UseFont = true;
            this.btnPunchlist.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPunchlist.ImageOptions.Image")));
            this.btnPunchlist.Location = new System.Drawing.Point(1810, 5);
            this.btnPunchlist.Margin = new System.Windows.Forms.Padding(5);
            this.btnPunchlist.Name = "btnPunchlist";
            this.btnPunchlist.Size = new System.Drawing.Size(200, 83);
            this.btnPunchlist.TabIndex = 11;
            this.btnPunchlist.Text = "Punch&list";
            this.btnPunchlist.Click += new System.EventHandler(this.btnPunchlist_Click);
            // 
            // btnBrowsePunchlist
            // 
            this.btnBrowsePunchlist.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowsePunchlist.Appearance.Options.UseFont = true;
            this.btnBrowsePunchlist.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnBrowsePunchlist.ImageOptions.Image")));
            this.btnBrowsePunchlist.Location = new System.Drawing.Point(1520, 5);
            this.btnBrowsePunchlist.Margin = new System.Windows.Forms.Padding(5);
            this.btnBrowsePunchlist.Name = "btnBrowsePunchlist";
            this.btnBrowsePunchlist.Size = new System.Drawing.Size(280, 83);
            this.btnBrowsePunchlist.TabIndex = 12;
            this.btnBrowsePunchlist.Text = "&Browse Punchlist";
            this.btnBrowsePunchlist.Click += new System.EventHandler(this.btnBrowsePunchlist_Click);
            // 
            // btnExportToPDF
            // 
            this.btnExportToPDF.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportToPDF.Appearance.Options.UseFont = true;
            this.btnExportToPDF.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnExportToPDF.ImageOptions.Image")));
            this.btnExportToPDF.Location = new System.Drawing.Point(1310, 5);
            this.btnExportToPDF.Margin = new System.Windows.Forms.Padding(5);
            this.btnExportToPDF.Name = "btnExportToPDF";
            this.btnExportToPDF.Size = new System.Drawing.Size(200, 83);
            this.btnExportToPDF.TabIndex = 10;
            this.btnExportToPDF.Text = "&Export";
            this.btnExportToPDF.Click += new System.EventHandler(this.btnExportToPDF_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Appearance.Options.UseFont = true;
            this.btnPrint.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnPrint.ImageOptions.Image")));
            this.btnPrint.Location = new System.Drawing.Point(1100, 5);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(5);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(200, 83);
            this.btnPrint.TabIndex = 7;
            this.btnPrint.Text = "&Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnReplacePDF
            // 
            this.btnReplacePDF.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReplacePDF.Appearance.Options.UseFont = true;
            this.btnReplacePDF.ImageOptions.ImageUri.Uri = "ExportToPDF;Colored";
            this.btnReplacePDF.Location = new System.Drawing.Point(882, 5);
            this.btnReplacePDF.Margin = new System.Windows.Forms.Padding(5);
            this.btnReplacePDF.Name = "btnReplacePDF";
            this.btnReplacePDF.Size = new System.Drawing.Size(208, 83);
            this.btnReplacePDF.TabIndex = 24;
            this.btnReplacePDF.Text = "&Replace";
            this.btnReplacePDF.Click += new System.EventHandler(this.btnReplacePDF_Click);
            // 
            // btnAppendPDF
            // 
            this.btnAppendPDF.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAppendPDF.Appearance.Options.UseFont = true;
            this.btnAppendPDF.ImageOptions.ImageUri.Uri = "ExportToPDF;Colored";
            this.btnAppendPDF.Location = new System.Drawing.Point(664, 5);
            this.btnAppendPDF.Margin = new System.Windows.Forms.Padding(5);
            this.btnAppendPDF.Name = "btnAppendPDF";
            this.btnAppendPDF.Size = new System.Drawing.Size(208, 83);
            this.btnAppendPDF.TabIndex = 23;
            this.btnAppendPDF.Text = "&Attach PDF";
            this.btnAppendPDF.Click += new System.EventHandler(this.btnAppendPDF_Click);
            // 
            // btnAttachImage
            // 
            this.btnAttachImage.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAttachImage.Appearance.Options.UseFont = true;
            this.btnAttachImage.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAttachImage.ImageOptions.Image")));
            this.btnAttachImage.Location = new System.Drawing.Point(446, 5);
            this.btnAttachImage.Margin = new System.Windows.Forms.Padding(5);
            this.btnAttachImage.Name = "btnAttachImage";
            this.btnAttachImage.Size = new System.Drawing.Size(208, 83);
            this.btnAttachImage.TabIndex = 22;
            this.btnAttachImage.Text = "&Attach";
            this.btnAttachImage.Click += new System.EventHandler(this.btnAttachImage_Click);
            // 
            // btnAddSignature
            // 
            this.btnAddSignature.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddSignature.Appearance.Options.UseFont = true;
            this.btnAddSignature.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnAddSignature.ImageOptions.Image")));
            this.btnAddSignature.Location = new System.Drawing.Point(216, 5);
            this.btnAddSignature.Margin = new System.Windows.Forms.Padding(5);
            this.btnAddSignature.Name = "btnAddSignature";
            this.btnAddSignature.Size = new System.Drawing.Size(220, 83);
            this.btnAddSignature.TabIndex = 20;
            this.btnAddSignature.Text = "Add Signature";
            this.btnAddSignature.Visible = false;
            this.btnAddSignature.Click += new System.EventHandler(this.btnAddSignature_Click);
            // 
            // btnInsertSignature
            // 
            this.btnInsertSignature.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertSignature.Appearance.Options.UseFont = true;
            this.btnInsertSignature.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnInsertSignature.ImageOptions.Image")));
            this.btnInsertSignature.Location = new System.Drawing.Point(2852, 98);
            this.btnInsertSignature.Margin = new System.Windows.Forms.Padding(5);
            this.btnInsertSignature.Name = "btnInsertSignature";
            this.btnInsertSignature.Size = new System.Drawing.Size(208, 83);
            this.btnInsertSignature.TabIndex = 13;
            this.btnInsertSignature.Text = "Signature";
            this.btnInsertSignature.Visible = false;
            this.btnInsertSignature.Click += new System.EventHandler(this.btnInsertSignature_Click);
            // 
            // btnInsertEquipment
            // 
            this.btnInsertEquipment.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertEquipment.Appearance.Options.UseFont = true;
            this.btnInsertEquipment.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnInsertEquipment.ImageOptions.Image")));
            this.btnInsertEquipment.Location = new System.Drawing.Point(2634, 98);
            this.btnInsertEquipment.Margin = new System.Windows.Forms.Padding(5);
            this.btnInsertEquipment.Name = "btnInsertEquipment";
            this.btnInsertEquipment.Size = new System.Drawing.Size(208, 83);
            this.btnInsertEquipment.TabIndex = 18;
            this.btnInsertEquipment.Text = "Equipment";
            this.btnInsertEquipment.Visible = false;
            this.btnInsertEquipment.Click += new System.EventHandler(this.btnInsertEquipment_Click);
            // 
            // btnInsertAcceptance
            // 
            this.btnInsertAcceptance.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertAcceptance.Appearance.Options.UseFont = true;
            this.btnInsertAcceptance.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnInsertAcceptance.ImageOptions.Image")));
            this.btnInsertAcceptance.Location = new System.Drawing.Point(2416, 98);
            this.btnInsertAcceptance.Margin = new System.Windows.Forms.Padding(5);
            this.btnInsertAcceptance.Name = "btnInsertAcceptance";
            this.btnInsertAcceptance.Size = new System.Drawing.Size(208, 83);
            this.btnInsertAcceptance.TabIndex = 17;
            this.btnInsertAcceptance.Text = "Acceptance";
            this.btnInsertAcceptance.Visible = false;
            this.btnInsertAcceptance.Click += new System.EventHandler(this.btnInsertAcceptance_Click);
            // 
            // btnInsertEditableSup
            // 
            this.btnInsertEditableSup.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertEditableSup.Appearance.Options.UseFont = true;
            this.btnInsertEditableSup.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnInsertEditableSup.ImageOptions.Image")));
            this.btnInsertEditableSup.Location = new System.Drawing.Point(2198, 98);
            this.btnInsertEditableSup.Margin = new System.Windows.Forms.Padding(5);
            this.btnInsertEditableSup.Name = "btnInsertEditableSup";
            this.btnInsertEditableSup.Size = new System.Drawing.Size(208, 83);
            this.btnInsertEditableSup.TabIndex = 19;
            this.btnInsertEditableSup.Text = "Editable Sup.";
            this.btnInsertEditableSup.Visible = false;
            this.btnInsertEditableSup.Click += new System.EventHandler(this.btnInsertEditableSup_Click);
            // 
            // btnInsertEditable
            // 
            this.btnInsertEditable.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInsertEditable.Appearance.Options.UseFont = true;
            this.btnInsertEditable.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnInsertEditable.ImageOptions.Image")));
            this.btnInsertEditable.Location = new System.Drawing.Point(1980, 98);
            this.btnInsertEditable.Margin = new System.Windows.Forms.Padding(5);
            this.btnInsertEditable.Name = "btnInsertEditable";
            this.btnInsertEditable.Size = new System.Drawing.Size(208, 83);
            this.btnInsertEditable.TabIndex = 16;
            this.btnInsertEditable.Text = "Editable Insp.";
            this.btnInsertEditable.Visible = false;
            this.btnInsertEditable.Click += new System.EventHandler(this.btnInsertEditable_Click);
            // 
            // btnToggleField
            // 
            this.btnToggleField.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToggleField.Appearance.Options.UseFont = true;
            this.btnToggleField.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnToggleField.ImageOptions.Image")));
            this.btnToggleField.Location = new System.Drawing.Point(1762, 98);
            this.btnToggleField.Margin = new System.Windows.Forms.Padding(5);
            this.btnToggleField.Name = "btnToggleField";
            this.btnToggleField.Size = new System.Drawing.Size(208, 83);
            this.btnToggleField.TabIndex = 14;
            this.btnToggleField.Text = "&Show Fields";
            this.btnToggleField.Visible = false;
            this.btnToggleField.Click += new System.EventHandler(this.btnToggleField_Click);
            // 
            // btnUnprotect
            // 
            this.btnUnprotect.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnprotect.Appearance.Options.UseFont = true;
            this.btnUnprotect.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnUnprotect.ImageOptions.Image")));
            this.btnUnprotect.Location = new System.Drawing.Point(1544, 98);
            this.btnUnprotect.Margin = new System.Windows.Forms.Padding(5);
            this.btnUnprotect.Name = "btnUnprotect";
            this.btnUnprotect.Size = new System.Drawing.Size(208, 83);
            this.btnUnprotect.TabIndex = 15;
            this.btnUnprotect.Text = "&Unprotect";
            this.btnUnprotect.Visible = false;
            this.btnUnprotect.Click += new System.EventHandler(this.btnUnprotect_Click);
            // 
            // btnCleanUpQR
            // 
            this.btnCleanUpQR.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCleanUpQR.Appearance.Options.UseFont = true;
            this.btnCleanUpQR.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCleanUpQR.ImageOptions.Image")));
            this.btnCleanUpQR.Location = new System.Drawing.Point(1326, 98);
            this.btnCleanUpQR.Margin = new System.Windows.Forms.Padding(5);
            this.btnCleanUpQR.Name = "btnCleanUpQR";
            this.btnCleanUpQR.Size = new System.Drawing.Size(208, 83);
            this.btnCleanUpQR.TabIndex = 21;
            this.btnCleanUpQR.Text = "&Cleanup QR";
            this.btnCleanUpQR.Visible = false;
            this.btnCleanUpQR.Click += new System.EventHandler(this.btnCleanUpQR_Click);
            // 
            // frmITR_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.ClientSize = new System.Drawing.Size(3077, 1293);
            this.Controls.Add(this.pnlMain);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Glow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "frmITR_Main";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlITR)).EndInit();
            this.pnlITR.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.flpButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlMain;
        private DevExpress.XtraEditors.PanelControl pnlButtons;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private DevExpress.XtraEditors.PanelControl pnlITR;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraEditors.SimpleButton btnProgress;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.SimpleButton btnReject;
        private DevExpress.XtraEditors.SimpleButton btnExportToPDF;
        private DevExpress.XtraEditors.SimpleButton btnPunchlist;
        private DevExpress.XtraEditors.SimpleButton btnBrowsePunchlist;
        private DevExpress.XtraEditors.SimpleButton btnInsertSignature;
        private CustomRichEdit customRichEdit1;
        private DevExpress.XtraEditors.SimpleButton btnToggleField;
        private DevExpress.XtraEditors.SimpleButton btnUnprotect;
        private DevExpress.XtraEditors.SimpleButton btnInsertEditable;
        private DevExpress.XtraEditors.SimpleButton btnInsertEquipment;
        private DevExpress.XtraEditors.SimpleButton btnInsertAcceptance;
        private DevExpress.XtraEditors.SimpleButton btnInsertEditableSup;
        private DevExpress.XtraEditors.SimpleButton btnAddSignature;
        private DevExpress.XtraEditors.SimpleButton btnCleanUpQR;
        private DevExpress.XtraEditors.SimpleButton btnAttachImage;
        private DevExpress.XtraEditors.SimpleButton btnAppendPDF;
        private DevExpress.XtraEditors.SimpleButton btnReplacePDF;
    }
}
