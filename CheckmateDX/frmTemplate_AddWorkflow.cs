using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectCommon;
using ProjectLibrary;

namespace CheckmateDX
{
    public partial class frmTemplate_AddWorkflow : CheckmateDX.frmParent
    {
        private Workflow _editWorkflow;

        private Workflow _Workflow;
        public frmTemplate_AddWorkflow(Guid parentGuid)
        {
            InitializeComponent();
            PopulateFormElements(parentGuid);
        }

        public frmTemplate_AddWorkflow(Workflow editWorkflow)
        {
            InitializeComponent();
            this.Text = "Edit Workflow";
            btnOk.Text = "Accept";
            PopulateFormElements(editWorkflow);
            _editWorkflow = editWorkflow;
        }

        /// <summary>
        /// Populate form element for adding
        /// </summary>
        private void PopulateFormElements(Guid parentGuid)
        {
            if (parentGuid != Guid.Empty)
                Common.PopulateCmbWorkflow(cmbParentWorkflow, null, parentGuid);
            else
                Common.PopulateCmbWorkflow(cmbParentWorkflow, null, null);
        }

        /// <summary>
        /// Populate form element for editing
        /// </summary>
        /// <param name="selectedParent">Parent workflow guid</param>
        private void PopulateFormElements(Workflow editWorkflow)
        {
            Common.PopulateCmbWorkflow(cmbParentWorkflow, editWorkflow.GUID, editWorkflow.workflowParentGuid);
            txtName.Text = editWorkflow.workflowName;
            txtDescription.Text = editWorkflow.workflowDescription;
        }

        public Workflow GetWorkflow()
        {
            return _Workflow;
        }

        #region Validation
        private bool ValidateFormElements()
        {
            if (txtName.Text.Trim() == string.Empty)
            {
                Common.Warn("Workflow cannot be empty");
                txtName.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for duplicate Workflow
        /// </summary>
        private bool ValidateWorkflow()
        {
            using (AdapterWORKFLOW_MAIN daWorkflow = new AdapterWORKFLOW_MAIN())
            {
                if (daWorkflow.GetBy(txtName.Text.Trim()) != null)
                {
                    return false;
                }
            }

            return true;
        }


        private bool ValidateEditWorkflow(Guid editWorkflowGuid)
        {
            using (AdapterWORKFLOW_MAIN daWorkflow = new AdapterWORKFLOW_MAIN())
            {
                dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow = daWorkflow.GetBy(txtName.Text.Trim());

                if (drWorkflow != null)
                {
                    if (drWorkflow.GUID != editWorkflowGuid)
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
                if (_editWorkflow != null || ValidateWorkflow())
                {
                    if (_editWorkflow != null)
                    {
                        if (ValidateEditWorkflow(_editWorkflow.GUID))
                        {
                            _editWorkflow.workflowName = txtName.Text.Trim();
                            _editWorkflow.workflowDescription = txtDescription.Text.Trim();
                            _editWorkflow.workflowParentGuid = (Guid)((ValuePair)cmbParentWorkflow.SelectedItem).Value;
                            _editWorkflow.workflowParentName = ((ValuePair)cmbParentWorkflow.SelectedItem).Label;
                            _Workflow = _editWorkflow;
                        }
                        else
                        {
                            Common.Prompt("The name specified has already been used by other workflow\n\nPlease type in a different name");
                            return;
                        }
                    }
                    else
                    {
                        Workflow newWorkflow = new Workflow(Guid.NewGuid())
                        {
                            workflowName = txtName.Text.Trim(),
                            workflowDescription = txtDescription.Text.Trim(),
                            workflowParentGuid = (Guid)((ValuePair)cmbParentWorkflow.SelectedItem).Value,
                            workflowParentName = ((ValuePair)cmbParentWorkflow.SelectedItem).Label
                        };

                        _Workflow = newWorkflow;
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                    Common.Warn("Workflow already exists"); ;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion
    }
}
