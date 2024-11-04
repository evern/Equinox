
namespace CheckmateDX
{
    partial class frmCertificate_Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCertificate_Main));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlPunchlistWalkdown = new System.Windows.Forms.Panel();
            this.customRichEdit1 = new CheckmateDX.CustomRichEdit();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnReject = new DevExpress.XtraEditors.SimpleButton();
            this.btnProgress = new DevExpress.XtraEditors.SimpleButton();
            this.btnExportToPDF = new DevExpress.XtraEditors.SimpleButton();
            this.btnImportPDF = new DevExpress.XtraEditors.SimpleButton();
            this.btnReplacePDF = new DevExpress.XtraEditors.SimpleButton();
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.WaitForm1), true, true);
            this.pnlMain.SuspendLayout();
            this.pnlPunchlistWalkdown.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.pnlPunchlistWalkdown);
            this.pnlMain.Controls.Add(this.pnlButtons);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1918, 999);
            this.pnlMain.TabIndex = 0;
            // 
            // pnlPunchlistWalkdown
            // 
            this.pnlPunchlistWalkdown.Controls.Add(this.customRichEdit1);
            this.pnlPunchlistWalkdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPunchlistWalkdown.Location = new System.Drawing.Point(0, 0);
            this.pnlPunchlistWalkdown.Name = "pnlPunchlistWalkdown";
            this.pnlPunchlistWalkdown.Size = new System.Drawing.Size(1918, 927);
            this.pnlPunchlistWalkdown.TabIndex = 0;
            // 
            // customRichEdit1
            // 
            this.customRichEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customRichEdit1.Location = new System.Drawing.Point(0, 0);
            this.customRichEdit1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.customRichEdit1.Size = new System.Drawing.Size(1918, 927);
            this.customRichEdit1.TabIndex = 1;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.flpButtons);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 927);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(1918, 72);
            this.pnlButtons.TabIndex = 1;
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.btnClose);
            this.flpButtons.Controls.Add(this.btnSave);
            this.flpButtons.Controls.Add(this.btnReject);
            this.flpButtons.Controls.Add(this.btnProgress);
            this.flpButtons.Controls.Add(this.btnExportToPDF);
            this.flpButtons.Controls.Add(this.btnImportPDF);
            this.flpButtons.Controls.Add(this.btnReplacePDF);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpButtons.Location = new System.Drawing.Point(0, 0);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(1918, 72);
            this.flpButtons.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.ImageOptions.Image")));
            this.btnClose.Location = new System.Drawing.Point(1764, 4);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(150, 61);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "&Exit";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.ImageOptions.Image")));
            this.btnSave.Location = new System.Drawing.Point(1606, 4);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(150, 61);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "&Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnReject
            // 
            this.btnReject.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReject.Appearance.Options.UseFont = true;
            this.btnReject.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnReject.ImageOptions.Image")));
            this.btnReject.Location = new System.Drawing.Point(1448, 4);
            this.btnReject.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(150, 61);
            this.btnReject.TabIndex = 16;
            this.btnReject.Text = "&Reject";
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnProgress
            // 
            this.btnProgress.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProgress.Appearance.Options.UseFont = true;
            this.btnProgress.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnProgress.ImageOptions.Image")));
            this.btnProgress.Location = new System.Drawing.Point(1290, 4);
            this.btnProgress.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnProgress.Name = "btnProgress";
            this.btnProgress.Size = new System.Drawing.Size(150, 61);
            this.btnProgress.TabIndex = 15;
            this.btnProgress.Text = "&Approve";
            this.btnProgress.Click += new System.EventHandler(this.btnProgress_Click);
            // 
            // btnExportToPDF
            // 
            this.btnExportToPDF.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportToPDF.Appearance.Options.UseFont = true;
            this.btnExportToPDF.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnExportToPDF.ImageOptions.Image")));
            this.btnExportToPDF.Location = new System.Drawing.Point(1134, 1);
            this.btnExportToPDF.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnExportToPDF.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnExportToPDF.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.btnExportToPDF.Name = "btnExportToPDF";
            this.btnExportToPDF.Size = new System.Drawing.Size(150, 61);
            this.btnExportToPDF.TabIndex = 12;
            this.btnExportToPDF.Text = "&Export PDF";
            this.btnExportToPDF.Click += new System.EventHandler(this.btnExportToPDF_Click);
            // 
            // btnImportPDF
            // 
            this.btnImportPDF.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportPDF.Appearance.Options.UseFont = true;
            this.btnImportPDF.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnImportPDF.ImageOptions.Image")));
            this.btnImportPDF.Location = new System.Drawing.Point(980, 1);
            this.btnImportPDF.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnImportPDF.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnImportPDF.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.btnImportPDF.Name = "btnImportPDF";
            this.btnImportPDF.Size = new System.Drawing.Size(150, 61);
            this.btnImportPDF.TabIndex = 13;
            this.btnImportPDF.Text = "&Import PDF";
            this.btnImportPDF.Click += new System.EventHandler(this.btnImportPDF_Click);
            // 
            // btnReplacePDF
            // 
            this.btnReplacePDF.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReplacePDF.Appearance.Options.UseFont = true;
            this.btnReplacePDF.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnReplacePDF.ImageOptions.Image")));
            this.btnReplacePDF.Location = new System.Drawing.Point(756, 1);
            this.btnReplacePDF.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnReplacePDF.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnReplacePDF.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.btnReplacePDF.Name = "btnReplacePDF";
            this.btnReplacePDF.Size = new System.Drawing.Size(220, 61);
            this.btnReplacePDF.TabIndex = 17;
            this.btnReplacePDF.Text = "&Replace with PDF";
            this.btnReplacePDF.Click += new System.EventHandler(this.btnReplacePDF_Click);
            // 
            // splashScreenManager1
            // 
            this.splashScreenManager1.ClosingDelay = 500;
            // 
            // frmCertificate_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1918, 999);
            this.Controls.Add(this.pnlMain);
            this.Name = "frmCertificate_Main";
            this.Text = "Certificate";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnlMain.ResumeLayout(false);
            this.pnlPunchlistWalkdown.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.flpButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlPunchlistWalkdown;
        private System.Windows.Forms.Panel pnlButtons;
        private CustomRichEdit customRichEdit1;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private DevExpress.XtraEditors.SimpleButton btnExportToPDF;
        private DevExpress.XtraEditors.SimpleButton btnImportPDF;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnProgress;
        private DevExpress.XtraEditors.SimpleButton btnReject;
        private DevExpress.XtraEditors.SimpleButton btnReplacePDF;
    }
}