namespace CheckmateDX
{
    partial class frmEquipment_Main
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
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.EquipmentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colEquipmentProjectGuid = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEquipmentDiscipline = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEquipmentAssetNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEquipmentMake = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEquipmentType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEquipmentModel = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEquipmentSerial = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colEquipmentExpiry = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGUID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCreatedBy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EquipmentBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl
            // 
            this.gridControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControl.DataSource = this.EquipmentBindingSource;
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(41, 19, 41, 19);
            this.gridControl.Location = new System.Drawing.Point(0, 44);
            this.gridControl.LookAndFeel.SkinName = "Visual Studio 2013 Light";
            this.gridControl.LookAndFeel.UseDefaultLookAndFeel = false;
            this.gridControl.MainView = this.gridView1;
            this.gridControl.Margin = new System.Windows.Forms.Padding(41, 19, 41, 19);
            this.gridControl.MenuManager = this.barManager1;
            this.gridControl.Name = "gridControl";
            this.gridControl.Size = new System.Drawing.Size(1354, 667);
            this.gridControl.TabIndex = 5;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.gridControl.Click += new System.EventHandler(this.gridControl_Click);
            // 
            // EquipmentBindingSource
            // 
            this.EquipmentBindingSource.DataSource = typeof(ProjectLibrary.Equipment);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colEquipmentProjectGuid,
            this.colEquipmentDiscipline,
            this.colEquipmentAssetNumber,
            this.colEquipmentMake,
            this.colEquipmentType,
            this.colEquipmentModel,
            this.colEquipmentSerial,
            this.colEquipmentExpiry,
            this.colGUID,
            this.colCreatedDate,
            this.colCreatedBy});
            this.gridView1.GridControl = this.gridControl;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsSelection.MultiSelect = true;
            // 
            // colEquipmentProjectGuid
            // 
            this.colEquipmentProjectGuid.FieldName = "EquipmentProjectGuid";
            this.colEquipmentProjectGuid.Name = "colEquipmentProjectGuid";
            // 
            // colEquipmentDiscipline
            // 
            this.colEquipmentDiscipline.FieldName = "EquipmentDiscipline";
            this.colEquipmentDiscipline.Name = "colEquipmentDiscipline";
            // 
            // colEquipmentAssetNumber
            // 
            this.colEquipmentAssetNumber.Caption = "Asset Number";
            this.colEquipmentAssetNumber.FieldName = "EquipmentAssetNumber";
            this.colEquipmentAssetNumber.Name = "colEquipmentAssetNumber";
            this.colEquipmentAssetNumber.OptionsColumn.AllowEdit = false;
            this.colEquipmentAssetNumber.Visible = true;
            this.colEquipmentAssetNumber.VisibleIndex = 0;
            // 
            // colEquipmentMake
            // 
            this.colEquipmentMake.Caption = "Make";
            this.colEquipmentMake.FieldName = "EquipmentMake";
            this.colEquipmentMake.Name = "colEquipmentMake";
            this.colEquipmentMake.OptionsColumn.AllowEdit = false;
            this.colEquipmentMake.Visible = true;
            this.colEquipmentMake.VisibleIndex = 1;
            // 
            // colEquipmentType
            // 
            this.colEquipmentType.Caption = "Type";
            this.colEquipmentType.FieldName = "EquipmentType";
            this.colEquipmentType.Name = "colEquipmentType";
            this.colEquipmentType.OptionsColumn.AllowEdit = false;
            this.colEquipmentType.Visible = true;
            this.colEquipmentType.VisibleIndex = 2;
            // 
            // colEquipmentModel
            // 
            this.colEquipmentModel.Caption = "Model";
            this.colEquipmentModel.FieldName = "EquipmentModel";
            this.colEquipmentModel.Name = "colEquipmentModel";
            this.colEquipmentModel.OptionsColumn.AllowEdit = false;
            this.colEquipmentModel.Visible = true;
            this.colEquipmentModel.VisibleIndex = 3;
            // 
            // colEquipmentSerial
            // 
            this.colEquipmentSerial.Caption = "Serial";
            this.colEquipmentSerial.FieldName = "EquipmentSerial";
            this.colEquipmentSerial.Name = "colEquipmentSerial";
            this.colEquipmentSerial.OptionsColumn.AllowEdit = false;
            this.colEquipmentSerial.Visible = true;
            this.colEquipmentSerial.VisibleIndex = 4;
            // 
            // colEquipmentExpiry
            // 
            this.colEquipmentExpiry.Caption = "Expiry";
            this.colEquipmentExpiry.FieldName = "EquipmentExpiry";
            this.colEquipmentExpiry.Name = "colEquipmentExpiry";
            this.colEquipmentExpiry.OptionsColumn.AllowEdit = false;
            this.colEquipmentExpiry.Visible = true;
            this.colEquipmentExpiry.VisibleIndex = 5;
            // 
            // colGUID
            // 
            this.colGUID.FieldName = "GUID";
            this.colGUID.Name = "colGUID";
            // 
            // colCreatedDate
            // 
            this.colCreatedDate.FieldName = "CreatedDate";
            this.colCreatedDate.Name = "colCreatedDate";
            this.colCreatedDate.OptionsColumn.AllowEdit = false;
            this.colCreatedDate.Visible = true;
            this.colCreatedDate.VisibleIndex = 6;
            // 
            // colCreatedBy
            // 
            this.colCreatedBy.FieldName = "CreatedBy";
            this.colCreatedBy.Name = "colCreatedBy";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmEquipment_Main
            // 
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.ClientSize = new System.Drawing.Size(1354, 733);
            this.Controls.Add(this.gridControl);
            this.Name = "frmEquipment_Main";
            this.Text = "Equipment";
            this.Controls.SetChildIndex(this.gridControl, 0);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EquipmentBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private System.Windows.Forms.BindingSource EquipmentBindingSource;
        private System.Windows.Forms.Timer timer1;
        private DevExpress.XtraGrid.Columns.GridColumn colEquipmentProjectGuid;
        private DevExpress.XtraGrid.Columns.GridColumn colEquipmentDiscipline;
        private DevExpress.XtraGrid.Columns.GridColumn colEquipmentAssetNumber;
        private DevExpress.XtraGrid.Columns.GridColumn colEquipmentMake;
        private DevExpress.XtraGrid.Columns.GridColumn colEquipmentType;
        private DevExpress.XtraGrid.Columns.GridColumn colEquipmentModel;
        private DevExpress.XtraGrid.Columns.GridColumn colEquipmentSerial;
        private DevExpress.XtraGrid.Columns.GridColumn colEquipmentExpiry;
        private DevExpress.XtraGrid.Columns.GridColumn colGUID;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedDate;
        private DevExpress.XtraGrid.Columns.GridColumn colCreatedBy;

    }
}
