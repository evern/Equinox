using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectCommon;
using ProjectLibrary;
using ProjectDatabase.DataAdapters;

namespace CheckmateDX
{
    public partial class frmEquipment_Add : CheckmateDX.frmParent
    {
        private Equipment _editEquipment;

        private Equipment _Equipment;
        public frmEquipment_Add()
        {
            InitializeComponent();
        }

        public frmEquipment_Add(Equipment editEquipment)
        {
            InitializeComponent();
            Text = "Edit Equipment";
            btnOk.Text = "Accept";

            PopulateFormElements(editEquipment);
            _editEquipment = editEquipment;
        }

        /// <summary>
        /// Populate for element for editing
        /// </summary>
        private void PopulateFormElements(Equipment editEquipment)
        {
            txtAssetNumber.Text = editEquipment.EquipmentAssetNumber;
            txtMake.Text = editEquipment.EquipmentMake;
            txtModel.Text = editEquipment.EquipmentModel;
            txtSerial.Text = editEquipment.EquipmentSerial;
            txtType.Text = editEquipment.EquipmentType;
            dateEditExpiry.EditValue = editEquipment.EquipmentExpiry;
        }

        public Equipment GetEquipment()
        {
            return _Equipment;
        }

        /// <summary>
        /// Checks whether form is properly filled
        /// </summary>
        private bool ValidateFormElements()
        {
            if (txtAssetNumber.Text.Trim() == string.Empty)
            {
                Common.Warn("Asset number cannot be empty");
                txtAssetNumber.Focus();
                return false;
            }

            if (txtMake.Text.Trim() == string.Empty)
            {
                Common.Warn("Make cannot be empty");
                txtMake.Focus();
                return false;
            }

            if (txtModel.Text.Trim() == string.Empty)
            {
                Common.Warn("Model cannot be empty");
                txtModel.Focus();
                return false;
            }

            if (txtSerial.Text.Trim() == string.Empty)
            {
                Common.Warn("Serial cannot be empty");
                txtSerial.Focus();
                return false;
            }

            if (txtType.Text.Trim() == string.Empty)
            {
                Common.Warn("Type cannot be empty");
                txtType.Focus();
                return false;
            }

            if (dateEditExpiry.EditValue == null)
            {
                Common.Warn("Please select expiry");
                dateEditExpiry.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for duplicate user
        /// </summary>
        private bool ValidateEquipment()
        {
            //using(AdapterGENERAL_EQUIPMENT daEquipment = new AdapterGENERAL_EQUIPMENT())
            //{
            //    if (daEquipment.GetBy(txtSerial.Text.Trim()) != null)
            //    {
            //        return false;
            //    }
            //}

            return true;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ValidateFormElements())
            {
                if (_editEquipment != null || ValidateEquipment())
                {
                    if (_editEquipment != null)
                    {
                        _editEquipment.EquipmentAssetNumber = txtAssetNumber.Text.Trim();
                        _editEquipment.EquipmentMake = txtMake.Text.Trim();
                        _editEquipment.EquipmentModel = txtModel.Text.Trim();
                        _editEquipment.EquipmentSerial = txtSerial.Text.Trim();
                        _editEquipment.EquipmentType = txtType.Text.Trim();
                        _editEquipment.EquipmentExpiry = (DateTime)dateEditExpiry.EditValue;
                        _Equipment = _editEquipment;
                    }
                    else
                    {
                        var newEquipment = new Equipment(Guid.NewGuid())
                        {
                            EquipmentAssetNumber = txtAssetNumber.Text.Trim(),
                            EquipmentMake = txtMake.Text.Trim(),
                            EquipmentModel = txtModel.Text.Trim(),
                            EquipmentSerial = txtSerial.Text.Trim(),
                            EquipmentType = txtType.Text.Trim(),
                            EquipmentExpiry = (DateTime)dateEditExpiry.EditValue
                        };

                        _Equipment = newEquipment;
                    }

                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    Common.Warn("Equipment already exists");
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
