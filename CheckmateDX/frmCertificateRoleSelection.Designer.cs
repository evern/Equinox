
namespace CheckmateDX
{
    partial class frmCertificateRoleSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCertificateRoleSelection));
            this.pnlParentRole = new System.Windows.Forms.Panel();
            this.cmbRole = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.panelDialogButtons = new DevExpress.XtraEditors.PanelControl();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.pnlParentRole.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRole.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelDialogButtons)).BeginInit();
            this.panelDialogButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlParentRole
            // 
            this.pnlParentRole.Controls.Add(this.cmbRole);
            this.pnlParentRole.Controls.Add(this.labelControl1);
            this.pnlParentRole.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlParentRole.Location = new System.Drawing.Point(0, 0);
            this.pnlParentRole.Margin = new System.Windows.Forms.Padding(4);
            this.pnlParentRole.Name = "pnlParentRole";
            this.pnlParentRole.Padding = new System.Windows.Forms.Padding(15, 15, 75, 15);
            this.pnlParentRole.Size = new System.Drawing.Size(601, 60);
            this.pnlParentRole.TabIndex = 3;
            // 
            // cmbRole
            // 
            this.cmbRole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbRole.Location = new System.Drawing.Point(165, 15);
            this.cmbRole.Margin = new System.Windows.Forms.Padding(0);
            this.cmbRole.Name = "cmbRole";
            this.cmbRole.Properties.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbRole.Properties.Appearance.Options.UseFont = true;
            this.cmbRole.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmbRole.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmbRole.Size = new System.Drawing.Size(361, 30);
            this.cmbRole.TabIndex = 7;
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
            this.labelControl1.Location = new System.Drawing.Point(15, 15);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(4);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.labelControl1.Size = new System.Drawing.Size(150, 30);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "Role";
            // 
            // panelDialogButtons
            // 
            this.panelDialogButtons.Controls.Add(this.btnOk);
            this.panelDialogButtons.Controls.Add(this.btnCancel);
            this.panelDialogButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelDialogButtons.Location = new System.Drawing.Point(0, 58);
            this.panelDialogButtons.Margin = new System.Windows.Forms.Padding(4);
            this.panelDialogButtons.Name = "panelDialogButtons";
            this.panelDialogButtons.Size = new System.Drawing.Size(601, 84);
            this.panelDialogButtons.TabIndex = 6;
            // 
            // btnOk
            // 
            this.btnOk.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnOk.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.ImageOptions.Image")));
            this.btnOk.Location = new System.Drawing.Point(2, 2);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(150, 80);
            this.btnOk.TabIndex = 12;
            this.btnOk.Text = "&Ok";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
            this.btnCancel.Location = new System.Drawing.Point(449, 2);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(150, 80);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmCertificateRoleSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 142);
            this.Controls.Add(this.panelDialogButtons);
            this.Controls.Add(this.pnlParentRole);
            this.Name = "frmCertificateRoleSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Certificate Role Selection";
            this.pnlParentRole.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cmbRole.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelDialogButtons)).EndInit();
            this.panelDialogButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlParentRole;
        private DevExpress.XtraEditors.ComboBoxEdit cmbRole;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.PanelControl panelDialogButtons;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
    }
}