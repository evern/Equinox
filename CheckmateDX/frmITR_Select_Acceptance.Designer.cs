namespace CheckmateDX
{
    partial class frmITR_Select_Acceptance
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmITR_Select_Acceptance));
            this.btnAcceptable = new DevExpress.XtraEditors.SimpleButton();
            this.btnNotApplicable = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnPunchlist = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAcceptable
            // 
            this.btnAcceptable.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAcceptable.Appearance.Options.UseFont = true;
            this.btnAcceptable.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAcceptable.Image = ((System.Drawing.Image)(resources.GetObject("btnAcceptable.Image")));
            this.btnAcceptable.Location = new System.Drawing.Point(2, 2);
            this.btnAcceptable.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnAcceptable.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnAcceptable.Name = "btnAcceptable";
            this.btnAcceptable.Padding = new System.Windows.Forms.Padding(3);
            this.btnAcceptable.Size = new System.Drawing.Size(166, 60);
            this.btnAcceptable.TabIndex = 3;
            this.btnAcceptable.Text = "Acceptable";
            this.btnAcceptable.Click += new System.EventHandler(this.btnAcceptable_Click);
            // 
            // btnNotApplicable
            // 
            this.btnNotApplicable.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNotApplicable.Appearance.Options.UseFont = true;
            this.btnNotApplicable.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNotApplicable.Image = ((System.Drawing.Image)(resources.GetObject("btnNotApplicable.Image")));
            this.btnNotApplicable.Location = new System.Drawing.Point(2, 62);
            this.btnNotApplicable.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnNotApplicable.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnNotApplicable.Name = "btnNotApplicable";
            this.btnNotApplicable.Padding = new System.Windows.Forms.Padding(3);
            this.btnNotApplicable.Size = new System.Drawing.Size(166, 60);
            this.btnNotApplicable.TabIndex = 4;
            this.btnNotApplicable.Text = "Not Applicable";
            this.btnNotApplicable.Click += new System.EventHandler(this.btnNotApplicable_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnPunchlist);
            this.panelControl1.Controls.Add(this.btnNotApplicable);
            this.panelControl1.Controls.Add(this.btnAcceptable);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(170, 185);
            this.panelControl1.TabIndex = 5;
            // 
            // btnPunchlist
            // 
            this.btnPunchlist.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPunchlist.Appearance.Options.UseFont = true;
            this.btnPunchlist.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPunchlist.Image = ((System.Drawing.Image)(resources.GetObject("btnPunchlist.Image")));
            this.btnPunchlist.Location = new System.Drawing.Point(2, 122);
            this.btnPunchlist.LookAndFeel.SkinName = "Office 2019 Dark Gray";
            this.btnPunchlist.LookAndFeel.UseDefaultLookAndFeel = false;
            this.btnPunchlist.Name = "btnPunchlist";
            this.btnPunchlist.Padding = new System.Windows.Forms.Padding(3);
            this.btnPunchlist.Size = new System.Drawing.Size(166, 60);
            this.btnPunchlist.TabIndex = 5;
            this.btnPunchlist.Text = "Punchlist";
            this.btnPunchlist.Click += new System.EventHandler(this.btnPunchlist_Click);
            // 
            // frmITR_Select_Acceptance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(170, 185);
            this.Controls.Add(this.panelControl1);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Glow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmITR_Select_Acceptance";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmITR_Select_Acceptance";
            this.Deactivate += new System.EventHandler(this.frmITR_Select_Acceptance_Deactivate);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnAcceptable;
        private DevExpress.XtraEditors.SimpleButton btnNotApplicable;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnPunchlist;


    }
}