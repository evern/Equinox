namespace CheckmateDX
{
    partial class frmTool_User_ChangePassword
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTool_User_ChangePassword));
            this.txtNewPassword2 = new DevExpress.XtraEditors.TextEdit();
            this.lblNewPassword2 = new DevExpress.XtraEditors.LabelControl();
            this.pnlCurrentPassword = new System.Windows.Forms.Panel();
            this.txtCurrentPassword = new DevExpress.XtraEditors.TextEdit();
            this.lblCurrentPassword = new DevExpress.XtraEditors.LabelControl();
            this.txtNewPassword1 = new DevExpress.XtraEditors.TextEdit();
            this.lblNewPassword1 = new DevExpress.XtraEditors.LabelControl();
            this.pnlNewPassword1 = new System.Windows.Forms.Panel();
            this.flpContent = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlNewPassword2 = new System.Windows.Forms.Panel();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.pnlMain = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.txtNewPassword2.Properties)).BeginInit();
            this.pnlCurrentPassword.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNewPassword1.Properties)).BeginInit();
            this.pnlNewPassword1.SuspendLayout();
            this.flpContent.SuspendLayout();
            this.pnlNewPassword2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtNewPassword2
            // 
            this.txtNewPassword2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNewPassword2.Location = new System.Drawing.Point(130, 10);
            this.txtNewPassword2.Name = "txtNewPassword2";
            this.txtNewPassword2.Properties.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNewPassword2.Properties.Appearance.Options.UseFont = true;
            this.txtNewPassword2.Properties.UseSystemPasswordChar = true;
            this.txtNewPassword2.Size = new System.Drawing.Size(198, 38);
            this.txtNewPassword2.TabIndex = 6;
            this.txtNewPassword2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewPassword2_KeyDown);
            // 
            // lblNewPassword2
            // 
            this.lblNewPassword2.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewPassword2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblNewPassword2.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblNewPassword2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNewPassword2.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblNewPassword2.Location = new System.Drawing.Point(10, 10);
            this.lblNewPassword2.Name = "lblNewPassword2";
            this.lblNewPassword2.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.lblNewPassword2.Size = new System.Drawing.Size(120, 20);
            this.lblNewPassword2.TabIndex = 5;
            this.lblNewPassword2.Text = "Repeat Password";
            // 
            // pnlCurrentPassword
            // 
            this.pnlCurrentPassword.Controls.Add(this.txtCurrentPassword);
            this.pnlCurrentPassword.Controls.Add(this.lblCurrentPassword);
            this.pnlCurrentPassword.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCurrentPassword.Location = new System.Drawing.Point(3, 13);
            this.pnlCurrentPassword.Name = "pnlCurrentPassword";
            this.pnlCurrentPassword.Padding = new System.Windows.Forms.Padding(10, 10, 50, 10);
            this.pnlCurrentPassword.Size = new System.Drawing.Size(378, 40);
            this.pnlCurrentPassword.TabIndex = 0;
            // 
            // txtCurrentPassword
            // 
            this.txtCurrentPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCurrentPassword.Location = new System.Drawing.Point(130, 10);
            this.txtCurrentPassword.Name = "txtCurrentPassword";
            this.txtCurrentPassword.Properties.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCurrentPassword.Properties.Appearance.Options.UseFont = true;
            this.txtCurrentPassword.Properties.UseSystemPasswordChar = true;
            this.txtCurrentPassword.Size = new System.Drawing.Size(198, 38);
            this.txtCurrentPassword.TabIndex = 6;
            this.txtCurrentPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCurrentPassword_KeyDown);
            // 
            // lblCurrentPassword
            // 
            this.lblCurrentPassword.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentPassword.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblCurrentPassword.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblCurrentPassword.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCurrentPassword.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblCurrentPassword.Location = new System.Drawing.Point(10, 10);
            this.lblCurrentPassword.Name = "lblCurrentPassword";
            this.lblCurrentPassword.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.lblCurrentPassword.Size = new System.Drawing.Size(120, 20);
            this.lblCurrentPassword.TabIndex = 5;
            this.lblCurrentPassword.Text = "Current Password";
            // 
            // txtNewPassword1
            // 
            this.txtNewPassword1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNewPassword1.Location = new System.Drawing.Point(130, 10);
            this.txtNewPassword1.Name = "txtNewPassword1";
            this.txtNewPassword1.Properties.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNewPassword1.Properties.Appearance.Options.UseFont = true;
            this.txtNewPassword1.Properties.UseSystemPasswordChar = true;
            this.txtNewPassword1.Size = new System.Drawing.Size(198, 38);
            this.txtNewPassword1.TabIndex = 6;
            this.txtNewPassword1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewPassword1_KeyDown);
            // 
            // lblNewPassword1
            // 
            this.lblNewPassword1.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewPassword1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblNewPassword1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblNewPassword1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblNewPassword1.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblNewPassword1.Location = new System.Drawing.Point(10, 10);
            this.lblNewPassword1.Name = "lblNewPassword1";
            this.lblNewPassword1.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.lblNewPassword1.Size = new System.Drawing.Size(120, 20);
            this.lblNewPassword1.TabIndex = 5;
            this.lblNewPassword1.Text = "New Password";
            // 
            // pnlNewPassword1
            // 
            this.pnlNewPassword1.Controls.Add(this.txtNewPassword1);
            this.pnlNewPassword1.Controls.Add(this.lblNewPassword1);
            this.pnlNewPassword1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlNewPassword1.Location = new System.Drawing.Point(3, 59);
            this.pnlNewPassword1.Name = "pnlNewPassword1";
            this.pnlNewPassword1.Padding = new System.Windows.Forms.Padding(10, 10, 50, 10);
            this.pnlNewPassword1.Size = new System.Drawing.Size(378, 40);
            this.pnlNewPassword1.TabIndex = 1;
            // 
            // flpContent
            // 
            this.flpContent.Controls.Add(this.pnlCurrentPassword);
            this.flpContent.Controls.Add(this.pnlNewPassword1);
            this.flpContent.Controls.Add(this.pnlNewPassword2);
            this.flpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpContent.Location = new System.Drawing.Point(0, 0);
            this.flpContent.Name = "flpContent";
            this.flpContent.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.flpContent.Size = new System.Drawing.Size(384, 211);
            this.flpContent.TabIndex = 1;
            // 
            // pnlNewPassword2
            // 
            this.pnlNewPassword2.Controls.Add(this.txtNewPassword2);
            this.pnlNewPassword2.Controls.Add(this.lblNewPassword2);
            this.pnlNewPassword2.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlNewPassword2.Location = new System.Drawing.Point(3, 105);
            this.pnlNewPassword2.Name = "pnlNewPassword2";
            this.pnlNewPassword2.Padding = new System.Windows.Forms.Padding(10, 10, 50, 10);
            this.pnlNewPassword2.Size = new System.Drawing.Size(378, 40);
            this.pnlNewPassword2.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(232, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 41);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.Location = new System.Drawing.Point(52, 2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 41);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&Accept";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Controls.Add(this.btnOk);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 166);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(50, 0, 50, 0);
            this.panelControl1.Size = new System.Drawing.Size(384, 45);
            this.panelControl1.TabIndex = 8;
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.panelControl1);
            this.pnlMain.Controls.Add(this.flpContent);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(384, 211);
            this.pnlMain.TabIndex = 1;
            // 
            // frmTool_User_ChangePassword
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(384, 211);
            this.Controls.Add(this.pnlMain);
            this.Name = "frmTool_User_ChangePassword";
            ((System.ComponentModel.ISupportInitialize)(this.txtNewPassword2.Properties)).EndInit();
            this.pnlCurrentPassword.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtCurrentPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNewPassword1.Properties)).EndInit();
            this.pnlNewPassword1.ResumeLayout(false);
            this.flpContent.ResumeLayout(false);
            this.pnlNewPassword2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtNewPassword2;
        private DevExpress.XtraEditors.LabelControl lblNewPassword2;
        private System.Windows.Forms.Panel pnlCurrentPassword;
        private DevExpress.XtraEditors.TextEdit txtCurrentPassword;
        private DevExpress.XtraEditors.LabelControl lblCurrentPassword;
        private DevExpress.XtraEditors.TextEdit txtNewPassword1;
        private DevExpress.XtraEditors.LabelControl lblNewPassword1;
        private System.Windows.Forms.Panel pnlNewPassword1;
        private System.Windows.Forms.FlowLayoutPanel flpContent;
        private System.Windows.Forms.Panel pnlNewPassword2;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.Panel pnlMain;
    }
}
