namespace RenamePDFToQRCode
{
    partial class frmSplitPDFByQRCode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSplitPDFByQRCode));
            this.btnBulkAttach = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // btnBulkAttach
            // 
            this.btnBulkAttach.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBulkAttach.Appearance.Options.UseFont = true;
            this.btnBulkAttach.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnBulkAttach.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnBulkAttach.ImageOptions.Image")));
            this.btnBulkAttach.Location = new System.Drawing.Point(0, 0);
            this.btnBulkAttach.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnBulkAttach.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnBulkAttach.Margin = new System.Windows.Forms.Padding(2);
            this.btnBulkAttach.Name = "btnBulkAttach";
            this.btnBulkAttach.Size = new System.Drawing.Size(234, 61);
            this.btnBulkAttach.TabIndex = 12;
            this.btnBulkAttach.Text = "&Split PDF By QRCode";
            this.btnBulkAttach.Click += new System.EventHandler(this.btnSplit_Click);
            // 
            // frmSplitPDFByQRCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 60);
            this.Controls.Add(this.btnBulkAttach);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSplitPDFByQRCode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PDF Splitter";
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnBulkAttach;
    }
}