namespace CheckmateDX
{
    partial class frmPunchlist_Print
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
            this.customRichEdit1 = new CheckmateDX.CustomRichEdit();
            this.SuspendLayout();
            // 
            // customRichEdit1
            // 
            this.customRichEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customRichEdit1.Location = new System.Drawing.Point(0, 0);
            this.customRichEdit1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.customRichEdit1.Name = "customRichEdit1";
            this.customRichEdit1.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            this.customRichEdit1.Options.Printing.PrintPreviewFormKind = DevExpress.XtraRichEdit.PrintPreviewFormKind.Bars;
            this.customRichEdit1.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
            this.customRichEdit1.Size = new System.Drawing.Size(1372, 970);
            this.customRichEdit1.TabIndex = 1;
            // 
            // frmPunchlist_Print
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.ClientSize = new System.Drawing.Size(1372, 970);
            this.Controls.Add(this.customRichEdit1);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "frmPunchlist_Print";
            this.ResumeLayout(false);

        }

        #endregion

        private CustomRichEdit customRichEdit1;


    }
}
