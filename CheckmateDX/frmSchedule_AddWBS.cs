using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectCommon;
using ProjectLibrary;

namespace CheckmateDX
{
    public partial class frmSchedule_AddWBS : CheckmateDX.frmParent
    {
        private WBS _editWBS;

        private WBS _WBS;

        List<wbsTagDisplay> _allWBSTagDisplay = new List<wbsTagDisplay>();
        Guid _projectGuid;
        /// <summary>
        /// Constructor for adding
        /// </summary>
        public frmSchedule_AddWBS(Guid projectGuid, List<wbsTagDisplay> wbsTagDisplay, Guid selectedWBSParentGuid)
        {
            InitializeComponent();
            _projectGuid = projectGuid;

            PopulateFormElements(wbsTagDisplay, selectedWBSParentGuid);
        }

        /// <summary>
        /// Constructor for editing
        /// </summary>
        /// <param name="editWBS">WBS to edit</param>
        public frmSchedule_AddWBS(Guid projectGuid, List<wbsTagDisplay> wbsTagDisplay, WBS editWBS)
        {
            InitializeComponent();
            _projectGuid = projectGuid;
            this.Text = "Edit WBS";
            btnOk.Text = "Accept";
            PopulateFormElements(wbsTagDisplay, editWBS);
            _editWBS = editWBS;
        }

        /// <summary>
        /// Populate form element for editing
        /// </summary>
        /// <param name="selectedParent">Parent wbs guid</param>
        private void PopulateFormElements(List<wbsTagDisplay> wbsTagDisplay, WBS editWBS)
        {
            PopulateFormElements(wbsTagDisplay, editWBS.wbsParentGuid, editWBS.GUID);

            txtName.Text = editWBS.wbsName;
            txtDescription.Text = editWBS.wbsDescription;
        }

        /// <summary>
        /// Populate form element for editing
        /// </summary>
        /// <param name="wbsTagDisplay">selectable wbs members</param>
        private void PopulateFormElements(List<wbsTagDisplay> wbsTagDisplay, Guid selectedWBSParentGuid, Guid? excludedGuid = null)
        {
            _allWBSTagDisplay = Common.ProcessWBSTagTreeList(wbsTagDisplay, excludedGuid, true);
            wbsTagDisplayBindingSource.DataSource = _allWBSTagDisplay;

            wbsTagDisplay selectedWBSTag = wbsTagDisplay.FirstOrDefault(obj => obj.wbsTagDisplayGuid == selectedWBSParentGuid);

            if (selectedWBSTag != null)
                treeListLookUpWBS.EditValue = _allWBSTagDisplay[_allWBSTagDisplay.IndexOf(selectedWBSTag)].wbsTagDisplayGuid;
            else
                treeListLookUpWBS.EditValue = _allWBSTagDisplay[0].wbsTagDisplayGuid;
        }

        public WBS GetWBS()
        {
            return _WBS;
        }

        #region Validation
        private bool ValidateFormElements()
        {
            if (txtName.Text.Trim() == string.Empty)
            {
                Common.Warn("Name cannot be empty");
                txtName.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for duplicate WBS
        /// </summary>
        private bool ValidateWBS(Guid projectGuid)
        {
            using (AdapterWBS daWBS = new AdapterWBS())
            {
                if (daWBS.GetBy(txtName.Text.Trim(), projectGuid) != null)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateEditWBS(WBS editWBS, Guid projectGuid)
        {
            using (AdapterWBS daWBS = new AdapterWBS())
            {
                dsWBS.WBSRow drWBS = daWBS.GetBy(txtName.Text.Trim(), projectGuid);

                if (drWBS != null)
                {
                    if (drWBS.GUID != editWBS.GUID)
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
                if (_editWBS != null || ValidateWBS(_projectGuid))
                {
                    if (_editWBS != null)
                    {
                        if (ValidateEditWBS(_editWBS, _projectGuid))
                        {
                            _editWBS.wbsName = txtName.Text.Trim();
                            _editWBS.wbsDescription = txtDescription.Text.Trim();
                            _editWBS.wbsParentGuid = (Guid)treeListLookUpWBS.EditValue;
                            _WBS = _editWBS;
                        }
                        else
                        {
                            Common.Prompt("The name specified has already been used in this project\n\nPlease type in a different name");
                            return;
                        }
                    }
                    else
                    {
                        WBS newWBS = new WBS(Guid.NewGuid())
                        {
                            wbsName = txtName.Text.Trim(),
                            wbsDescription = txtDescription.Text.Trim(),
                            wbsParentGuid = (Guid)treeListLookUpWBS.EditValue
                        };

                        _WBS = newWBS;
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                    Common.Warn("Name already exists in project"); ;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion
    }
}
