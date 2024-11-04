namespace CheckmateDX
{
    partial class frmDatabase_Config
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDatabase_Config));
            this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.WaitForm1), true, true);
            this.pnlMain = new System.Windows.Forms.Panel();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnTest = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.flpContent = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlServer = new System.Windows.Forms.Panel();
            this.txtServer = new DevExpress.XtraEditors.TextEdit();
            this.lblServer = new DevExpress.XtraEditors.LabelControl();
            this.pnlDatabase = new System.Windows.Forms.Panel();
            this.txtDatabase = new DevExpress.XtraEditors.TextEdit();
            this.lblDatabase = new DevExpress.XtraEditors.LabelControl();
            this.pnlUsername = new System.Windows.Forms.Panel();
            this.txtUsername = new DevExpress.XtraEditors.TextEdit();
            this.lblUsername = new DevExpress.XtraEditors.LabelControl();
            this.pnlPassword = new System.Windows.Forms.Panel();
            this.txtPassword = new DevExpress.XtraEditors.TextEdit();
            this.lblPassword = new DevExpress.XtraEditors.LabelControl();
            this.panelHWID = new System.Windows.Forms.Panel();
            this.lblCurrentHWID = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.flpContent.SuspendLayout();
            this.pnlServer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtServer.Properties)).BeginInit();
            this.pnlDatabase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabase.Properties)).BeginInit();
            this.pnlUsername.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).BeginInit();
            this.pnlPassword.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            this.panelHWID.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.panelControl1);
            this.pnlMain.Controls.Add(this.flpContent);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(6);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(768, 644);
            this.pnlMain.TabIndex = 1;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnTest);
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Controls.Add(this.btnOk);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelControl1.Location = new System.Drawing.Point(0, 554);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(6);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(40, 0, 40, 0);
            this.panelControl1.Size = new System.Drawing.Size(768, 90);
            this.panelControl1.TabIndex = 8;
            // 
            // btnTest
            // 
            this.btnTest.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTest.Appearance.Options.UseFont = true;
            this.btnTest.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnTest.ImageOptions.Image")));
            this.btnTest.Location = new System.Drawing.Point(286, 2);
            this.btnTest.Margin = new System.Windows.Forms.Padding(6);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(200, 82);
            this.btnTest.TabIndex = 3;
            this.btnTest.Text = "&Test";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(526, 2);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(200, 86);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnOk.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.ImageOptions.Image")));
            this.btnOk.Location = new System.Drawing.Point(42, 2);
            this.btnOk.Margin = new System.Windows.Forms.Padding(6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(200, 86);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&Save";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // flpContent
            // 
            this.flpContent.Controls.Add(this.pnlServer);
            this.flpContent.Controls.Add(this.pnlDatabase);
            this.flpContent.Controls.Add(this.pnlUsername);
            this.flpContent.Controls.Add(this.pnlPassword);
            this.flpContent.Controls.Add(this.panelHWID);
            this.flpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpContent.Location = new System.Drawing.Point(0, 0);
            this.flpContent.Margin = new System.Windows.Forms.Padding(6);
            this.flpContent.Name = "flpContent";
            this.flpContent.Padding = new System.Windows.Forms.Padding(0, 18, 0, 0);
            this.flpContent.Size = new System.Drawing.Size(768, 644);
            this.flpContent.TabIndex = 1;
            // 
            // pnlServer
            // 
            this.pnlServer.Controls.Add(this.txtServer);
            this.pnlServer.Controls.Add(this.lblServer);
            this.pnlServer.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlServer.Location = new System.Drawing.Point(6, 24);
            this.pnlServer.Margin = new System.Windows.Forms.Padding(6);
            this.pnlServer.Name = "pnlServer";
            this.pnlServer.Padding = new System.Windows.Forms.Padding(18, 18, 98, 18);
            this.pnlServer.Size = new System.Drawing.Size(754, 80);
            this.pnlServer.TabIndex = 0;
            // 
            // txtServer
            // 
            this.txtServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtServer.Location = new System.Drawing.Point(218, 18);
            this.txtServer.Margin = new System.Windows.Forms.Padding(6);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(438, 26);
            this.txtServer.TabIndex = 6;
            // 
            // lblServer
            // 
            this.lblServer.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServer.Appearance.Options.UseFont = true;
            this.lblServer.Appearance.Options.UseTextOptions = true;
            this.lblServer.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblServer.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblServer.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblServer.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblServer.Location = new System.Drawing.Point(18, 18);
            this.lblServer.Margin = new System.Windows.Forms.Padding(6);
            this.lblServer.Name = "lblServer";
            this.lblServer.Padding = new System.Windows.Forms.Padding(0, 0, 18, 0);
            this.lblServer.Size = new System.Drawing.Size(200, 44);
            this.lblServer.TabIndex = 5;
            this.lblServer.Text = "Server";
            // 
            // pnlDatabase
            // 
            this.pnlDatabase.Controls.Add(this.txtDatabase);
            this.pnlDatabase.Controls.Add(this.lblDatabase);
            this.pnlDatabase.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDatabase.Location = new System.Drawing.Point(6, 116);
            this.pnlDatabase.Margin = new System.Windows.Forms.Padding(6);
            this.pnlDatabase.Name = "pnlDatabase";
            this.pnlDatabase.Padding = new System.Windows.Forms.Padding(18, 18, 98, 18);
            this.pnlDatabase.Size = new System.Drawing.Size(754, 80);
            this.pnlDatabase.TabIndex = 1;
            // 
            // txtDatabase
            // 
            this.txtDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDatabase.Location = new System.Drawing.Point(218, 18);
            this.txtDatabase.Margin = new System.Windows.Forms.Padding(6);
            this.txtDatabase.Name = "txtDatabase";
            this.txtDatabase.Size = new System.Drawing.Size(438, 26);
            this.txtDatabase.TabIndex = 6;
            // 
            // lblDatabase
            // 
            this.lblDatabase.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDatabase.Appearance.Options.UseFont = true;
            this.lblDatabase.Appearance.Options.UseTextOptions = true;
            this.lblDatabase.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblDatabase.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblDatabase.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDatabase.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblDatabase.Location = new System.Drawing.Point(18, 18);
            this.lblDatabase.Margin = new System.Windows.Forms.Padding(6);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Padding = new System.Windows.Forms.Padding(0, 0, 18, 0);
            this.lblDatabase.Size = new System.Drawing.Size(200, 44);
            this.lblDatabase.TabIndex = 5;
            this.lblDatabase.Text = "Database";
            // 
            // pnlUsername
            // 
            this.pnlUsername.Controls.Add(this.txtUsername);
            this.pnlUsername.Controls.Add(this.lblUsername);
            this.pnlUsername.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlUsername.Location = new System.Drawing.Point(6, 208);
            this.pnlUsername.Margin = new System.Windows.Forms.Padding(6);
            this.pnlUsername.Name = "pnlUsername";
            this.pnlUsername.Padding = new System.Windows.Forms.Padding(18, 18, 98, 18);
            this.pnlUsername.Size = new System.Drawing.Size(754, 80);
            this.pnlUsername.TabIndex = 2;
            // 
            // txtUsername
            // 
            this.txtUsername.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUsername.Location = new System.Drawing.Point(218, 18);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(6);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(438, 26);
            this.txtUsername.TabIndex = 6;
            // 
            // lblUsername
            // 
            this.lblUsername.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsername.Appearance.Options.UseFont = true;
            this.lblUsername.Appearance.Options.UseTextOptions = true;
            this.lblUsername.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblUsername.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblUsername.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblUsername.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblUsername.Location = new System.Drawing.Point(18, 18);
            this.lblUsername.Margin = new System.Windows.Forms.Padding(6);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Padding = new System.Windows.Forms.Padding(0, 0, 18, 0);
            this.lblUsername.Size = new System.Drawing.Size(200, 44);
            this.lblUsername.TabIndex = 5;
            this.lblUsername.Text = "Username";
            // 
            // pnlPassword
            // 
            this.pnlPassword.Controls.Add(this.txtPassword);
            this.pnlPassword.Controls.Add(this.lblPassword);
            this.pnlPassword.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPassword.Location = new System.Drawing.Point(6, 300);
            this.pnlPassword.Margin = new System.Windows.Forms.Padding(6);
            this.pnlPassword.Name = "pnlPassword";
            this.pnlPassword.Padding = new System.Windows.Forms.Padding(18, 18, 98, 18);
            this.pnlPassword.Size = new System.Drawing.Size(754, 80);
            this.pnlPassword.TabIndex = 3;
            // 
            // txtPassword
            // 
            this.txtPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPassword.Location = new System.Drawing.Point(218, 18);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(6);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Properties.Appearance.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Properties.Appearance.Options.UseFont = true;
            this.txtPassword.Properties.UseSystemPasswordChar = true;
            this.txtPassword.Size = new System.Drawing.Size(438, 30);
            this.txtPassword.TabIndex = 6;
            // 
            // lblPassword
            // 
            this.lblPassword.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.Appearance.Options.UseFont = true;
            this.lblPassword.Appearance.Options.UseTextOptions = true;
            this.lblPassword.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblPassword.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblPassword.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblPassword.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblPassword.Location = new System.Drawing.Point(18, 18);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(6);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Padding = new System.Windows.Forms.Padding(0, 0, 18, 0);
            this.lblPassword.Size = new System.Drawing.Size(200, 44);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Password";
            // 
            // panelHWID
            // 
            this.panelHWID.Controls.Add(this.lblCurrentHWID);
            this.panelHWID.Controls.Add(this.labelControl1);
            this.panelHWID.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHWID.Location = new System.Drawing.Point(6, 392);
            this.panelHWID.Margin = new System.Windows.Forms.Padding(6);
            this.panelHWID.Name = "panelHWID";
            this.panelHWID.Padding = new System.Windows.Forms.Padding(18, 18, 98, 18);
            this.panelHWID.Size = new System.Drawing.Size(754, 80);
            this.panelHWID.TabIndex = 4;
            // 
            // lblCurrentHWID
            // 
            this.lblCurrentHWID.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentHWID.Appearance.Options.UseFont = true;
            this.lblCurrentHWID.Appearance.Options.UseTextOptions = true;
            this.lblCurrentHWID.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblCurrentHWID.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblCurrentHWID.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCurrentHWID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentHWID.Location = new System.Drawing.Point(218, 18);
            this.lblCurrentHWID.Margin = new System.Windows.Forms.Padding(6);
            this.lblCurrentHWID.Name = "lblCurrentHWID";
            this.lblCurrentHWID.Padding = new System.Windows.Forms.Padding(0, 0, 18, 0);
            this.lblCurrentHWID.Size = new System.Drawing.Size(438, 44);
            this.lblCurrentHWID.TabIndex = 6;
            this.lblCurrentHWID.Text = "Current HWID";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Candara", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Appearance.Options.UseTextOptions = true;
            this.labelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.labelControl1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl1.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelControl1.Location = new System.Drawing.Point(18, 18);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(6);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Padding = new System.Windows.Forms.Padding(0, 0, 18, 0);
            this.labelControl1.Size = new System.Drawing.Size(200, 44);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "Current HWID";
            // 
            // frmDatabase_Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.ClientSize = new System.Drawing.Size(768, 644);
            this.Controls.Add(this.pnlMain);
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "frmDatabase_Config";
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.flpContent.ResumeLayout(false);
            this.pnlServer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtServer.Properties)).EndInit();
            this.pnlDatabase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDatabase.Properties)).EndInit();
            this.pnlUsername.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).EndInit();
            this.pnlPassword.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            this.panelHWID.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpContent;
        private System.Windows.Forms.Panel pnlServer;
        private DevExpress.XtraEditors.TextEdit txtServer;
        private DevExpress.XtraEditors.LabelControl lblServer;
        private System.Windows.Forms.Panel pnlDatabase;
        private DevExpress.XtraEditors.TextEdit txtDatabase;
        private DevExpress.XtraEditors.LabelControl lblDatabase;
        private System.Windows.Forms.Panel pnlUsername;
        private DevExpress.XtraEditors.TextEdit txtUsername;
        private DevExpress.XtraEditors.LabelControl lblUsername;
        private System.Windows.Forms.Panel pnlPassword;
        private DevExpress.XtraEditors.LabelControl lblPassword;
        private System.Windows.Forms.Panel pnlMain;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnTest;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        private System.Windows.Forms.Panel panelHWID;
        private DevExpress.XtraEditors.LabelControl lblCurrentHWID;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}
