namespace PDFToDocConverter
{
    partial class PDFConverter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PDFConverter));
            this.btnConvert = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // btnConvert
            // 
            this.btnConvert.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConvert.Appearance.Options.UseFont = true;
            this.btnConvert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnConvert.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnConvert.ImageOptions.Image")));
            this.btnConvert.Location = new System.Drawing.Point(0, 0);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(284, 39);
            this.btnConvert.TabIndex = 11;
            this.btnConvert.Text = "&Convert PDF to Doc";
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // PDFConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 39);
            this.Controls.Add(this.btnConvert);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PDFConverter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PDF To Doc Converter";
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnConvert;
    }
}

