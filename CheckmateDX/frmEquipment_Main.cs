using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;

namespace CheckmateDX
{
    public partial class frmEquipment_Main : CheckmateDX.frmTool_Main
    {
        AdapterGENERAL_EQUIPMENT _daEquipment = new AdapterGENERAL_EQUIPMENT();
        dsGENERAL_EQUIPMENT _dsEquipment = new dsGENERAL_EQUIPMENT();

        List<Equipment> _allEquipment = new List<Equipment>();

        //variables
        string _discipline = string.Empty;
        Guid _projectGuid = Guid.Empty;
        public frmEquipment_Main()
        {
            InitializeComponent();
            _discipline = System_Environment.GetUser().userDiscipline;
            _projectGuid = (Guid)System_Environment.GetUser().userProject.Value;
            EquipmentBindingSource.DataSource = _allEquipment;
            timer1.Enabled = true;
        }

        #region Public
        /// <summary>
        /// Do not allow user to add/edit/delete if this form is shown as a dialog
        /// </summary>
        public void ShowAsDialog()
        {
            barMenu.Visible = false;
            colEquipmentAssetNumber.Visible = false;
            colCreatedDate.Visible = false;
            this.Width = 700;
            this.Height = 900;
            this.Text = "Select Test Equipment";
            gridView1.RowHeight = 50;
        }
        #endregion

        #region Form Population
        /// <summary>
        /// Refresh all Equipments from database
        /// </summary>
        private void RefreshEquipments()
        {
            _allEquipment.Clear();
            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtEquipment = _daEquipment.GetByDisciplineProject(_discipline, _projectGuid);

            if (dtEquipment != null)
            {
                foreach (dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drEquipment in dtEquipment.Rows)
                {
                    _allEquipment.Add(new Equipment(drEquipment.GUID)
                    {
                        EquipmentDiscipline = drEquipment.DISCIPLINE,
                        EquipmentAssetNumber = drEquipment.ASSET_NUMBER,
                        EquipmentMake = drEquipment.MAKE,
                        EquipmentExpiry = drEquipment.EXPIRY,
                        EquipmentModel = drEquipment.MODEL,
                        EquipmentSerial = drEquipment.SERIAL,
                        EquipmentType = drEquipment.TYPE,
                        CreatedDate = drEquipment.CREATED,
                        CreatedBy = drEquipment.CREATEDBY
                    }
                    );
                }
            }

            gridControl.RefreshDataSource();
        }
        #endregion

        #region Events
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            SetupEquipmentParameters();
        }

        private void SetupEquipmentParameters()
        {
            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                frmTool_Project frmSelectProject = new frmTool_Project();
                frmSelectProject.ShowAsDialog();
                if (frmSelectProject.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _projectGuid = frmSelectProject.GetSelectedProject().GUID;
                }
                else
                {
                    this.Close();
                    return;
                }

                frmITR_Select_Discipline frmSelectDiscipline = new frmITR_Select_Discipline();
                if (frmSelectDiscipline.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _discipline = frmSelectDiscipline.GetDiscipline();
                }
                else
                {
                    this.Close();
                    return;
                }
            }

            RefreshEquipments();
        }
        /// <summary>
        /// Allows user to select equipment
        /// </summary>
        private void gridControl_Click(object sender, EventArgs e)
        {
            if(this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.FixedDialog)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        public Equipment GetSelectedEq()
        {
            return (Equipment)gridView1.GetFocusedRow();
        }
        #endregion

        #region Button Overrides
        protected override void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmEquipment_Add frmEquipmentAdd = new frmEquipment_Add();
            if (frmEquipmentAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Equipment newEquipment = frmEquipmentAdd.GetEquipment();
                dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drEquipment = _dsEquipment.GENERAL_EQUIPMENT.NewGENERAL_EQUIPMENTRow();
                drEquipment.GUID = Guid.NewGuid();
                drEquipment.DISCIPLINE = _discipline;
                drEquipment.PROJECTGUID = _projectGuid;
                AssignEquipmentDetails(drEquipment, newEquipment);
                drEquipment.CREATED = DateTime.Now;
                drEquipment.CREATEDBY = System_Environment.GetUser().GUID;
                _dsEquipment.GENERAL_EQUIPMENT.AddGENERAL_EQUIPMENTRow(drEquipment);
                _daEquipment.Save(drEquipment);

                RefreshEquipments();
                //Common.Prompt("Equipment successfully added");
            }

            base.btnAdd_ItemClick(sender, e);
        }
        

        protected override void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridView1.SelectedRowsCount == 0)
            {
                Common.Warn("Please select Equipment to edit");
                return;
            }

            frmEquipment_Add frmEquipmentAdd = new frmEquipment_Add((Equipment)gridView1.GetFocusedRow());
            if (frmEquipmentAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Equipment editEquipment = frmEquipmentAdd.GetEquipment();

                dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drEquipment = _daEquipment.GetBy(editEquipment.GUID);
                AssignEquipmentDetails(drEquipment, editEquipment);
                drEquipment.UPDATED = DateTime.Now;
                drEquipment.UPDATEDBY = System_Environment.GetUser().GUID;
                _daEquipment.Save(drEquipment);

                RefreshEquipments();
                //Common.Prompt("Equipment successfully updated");
            }

            base.btnEdit_ItemClick(sender, e);
        }

        protected override void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridView1.SelectedRowsCount == 0)
            {
                Common.Warn("Please select equipment(s) to delete");
                return;
            }

            if (!Common.Confirmation("Are you sure you want to delete selected equipment(s)?", "Confirmation"))
                return;

            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtEquipment = new dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable(); //what project to delete
            int[] selectedRowIndexes = gridView1.GetSelectedRows();

            foreach (int selectedRowIndex in selectedRowIndexes)
            {
                Equipment selectedEquipment = (Equipment)gridView1.GetRow(selectedRowIndex);
                _daEquipment.RemoveBy(selectedEquipment.GUID);
            }

            RefreshEquipments();

            base.btnDelete_ItemClick(sender, e);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Assigns common Equipment details to data row
        /// </summary>
        /// <param name="drEquipmentMaster">datarow to be assigned</param>
        /// <param name="Equipment">Equipment details</param>
        private void AssignEquipmentDetails(dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drEquipment, Equipment Equipment)
        {
            drEquipment.ASSET_NUMBER = Equipment.EquipmentAssetNumber;
            drEquipment.MAKE = Equipment.EquipmentMake;
            drEquipment.EXPIRY = Equipment.EquipmentExpiry;
            drEquipment.MODEL = Equipment.EquipmentModel;
            drEquipment.SERIAL = Equipment.EquipmentSerial;
            drEquipment.TYPE = Equipment.EquipmentType;
        }
        #endregion
    }
}
