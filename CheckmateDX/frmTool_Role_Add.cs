using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectDatabase.DataAdapters;
using ProjectCommon;
using ProjectLibrary;

namespace CheckmateDX
{
    public partial class frmTool_Role_Add : CheckmateDX.frmParent
    {
         private Role _editRole;

        private Role _Role;
        public frmTool_Role_Add(Guid parentGuid)
        {
            InitializeComponent();
            PopulateFormElements(parentGuid);
        }

        public frmTool_Role_Add(Role editRole)
        {
            InitializeComponent();
            this.Text = "Edit Role";
            btnOk.Text = "Accept";
            PopulateFormElements(editRole);
            _editRole = editRole;
        }
        
        /// <summary>
        /// Populate form element for adding
        /// </summary>
        private void PopulateFormElements(Guid parentGuid)
        {
            if(parentGuid != Guid.Empty)
                Common.PopulateCmbAuthRole(cmbParentRole, null, parentGuid, true);
            else
                Common.PopulateCmbAuthRole(cmbParentRole, null, null, true);
        }

        /// <summary>
        /// Populate form element for editing
        /// </summary>
        /// <param name="selectedParent">Parent role guid</param>
        private void PopulateFormElements(Role editRole)
        {
            Common.PopulateCmbAuthRole(cmbParentRole, editRole.GUID, editRole.roleParentGuid, true);
            txtName.Text = editRole.roleName;
        }

        public Role GetRole()
        {
            return _Role;
        }

        #region Validation
        private bool ValidateFormElements()
        {
            if (txtName.Text.Trim() == string.Empty)
            {
                Common.Warn("Role cannot be empty");
                txtName.Focus();
                return false;
            }

            return true;
        } 

        /// <summary>
        /// Checks for duplicate Role
        /// </summary>
        private bool ValidateRole()
        {
            using (AdapterROLE_MAIN daRole = new AdapterROLE_MAIN())
            {
                if (daRole.GetBy(txtName.Text.Trim()) != null)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Events
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ValidateFormElements())
            {
                if (_editRole != null || ValidateRole())
                {
                    if (_editRole != null)
                    {
                        _editRole.roleName = txtName.Text.Trim();
                        _editRole.roleParentGuid = (Guid)((ValuePair)cmbParentRole.SelectedItem).Value;
                        _editRole.roleParentName = ((ValuePair)cmbParentRole.SelectedItem).Label;
                        _Role = _editRole;
                    }
                    else
                    {
                        Role newRole = new Role(Guid.NewGuid())
                        {
                            roleName = txtName.Text.Trim(),
                            roleParentGuid = (Guid)((ValuePair)cmbParentRole.SelectedItem).Value,
                            roleParentName = ((ValuePair)cmbParentRole.SelectedItem).Label
                        };

                        _Role = newRole;
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                    Common.Warn("Role already exists"); ;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion
    }
}
