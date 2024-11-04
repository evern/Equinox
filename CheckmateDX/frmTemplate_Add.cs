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
using DevExpress.XtraEditors.Controls;

namespace CheckmateDX
{
    public partial class frmTemplate_Add : CheckmateDX.frmParent
    {
         private Template _editTemplate;

        private Template _Template;

        /// <summary>
        /// Constructor for adding
        /// </summary>
        public frmTemplate_Add()
        {
            InitializeComponent();
        }

        public frmTemplate_Add(List<Template> copyTemplates)
        {
            InitializeComponent();
            PopulateFormElements(copyTemplates);
        }

        /// <summary>
        /// Constructor for editing
        /// </summary>
        /// <param name="editTemplate">Template to edit</param>
        public frmTemplate_Add(Template editTemplate, List<Template> copyTemplates)
        {
            InitializeComponent();
            this.Text = "Edit Template";
            btnOk.Text = "Accept";
            PopulateFormElements(editTemplate, copyTemplates);
            _editTemplate = editTemplate;
        }

        /// <summary>
        /// Populate form element for adding
        /// </summary>
        private void PopulateFormElements(List<Template> copyTemplates)
        {
            Common.PopulateCmbWorkflow(cmbWorkflow, null, null, false);
            ComboBoxItemCollection coll = cmbCopyFrom.Properties.Items;
            coll.BeginUpdate();
            foreach (Template copyTemplate in copyTemplates)
            {
                coll.Add(copyTemplate.templateName);
            }
            coll.EndUpdate();
        }

        /// <summary>
        /// Populate form element for editing
        /// </summary>
        /// <param name="selectedParent">Parent template guid</param>
        private void PopulateFormElements(Template editTemplate, List<Template> copyTemplates)
        {
            Common.PopulateCmbWorkflow(cmbWorkflow, null, (Guid)editTemplate.templateWorkFlow.Value, false);
            txtName.Text = editTemplate.templateName;
            txtRevision.Text = editTemplate.templateRevision;
            txtDescription.Text = editTemplate.templateDescription;
            checkEditQR.Checked = editTemplate.templateQRSupport;
            checkEditSkipApproved.Checked = editTemplate.templateSkipApproved;
            ComboBoxItemCollection coll = cmbCopyFrom.Properties.Items;
            coll.BeginUpdate();
            foreach(Template copyTemplate in copyTemplates)
            {
                coll.Add(copyTemplate);
            }
            coll.EndUpdate();
        }

        public Template GetTemplate()
        {
            return _Template;
        }

        public string GetCopyFromTemplate()
        {
            if (cmbCopyFrom.EditValue != null)
                return cmbCopyFrom.EditValue.ToString();
            else
                return string.Empty;
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

            if (txtRevision.Text.Trim() == string.Empty)
            {
                Common.Warn("Revision cannot be empty");
                txtRevision.Focus();
                return false;
            }

            return true;
        } 

        /// <summary>
        /// Checks for duplicate Template
        /// </summary>
        private bool ValidateTemplate()
        {
            using (AdapterTEMPLATE_MAIN daTemplate = new AdapterTEMPLATE_MAIN())
            {
                if (daTemplate.GetBy(txtName.Text.Trim()) != null)
                    return false;

                if (txtName.Text.Trim() == Variables.punchlistTemplateName)
                    return false;
            }

            return true;
        }

        private bool ValidateEditTemplate(Guid editTemplateGuid)
        {
            using (AdapterTEMPLATE_MAIN daTemplate = new AdapterTEMPLATE_MAIN())
            {
                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = daTemplate.GetBy(txtName.Text.Trim());

                if (drTemplate != null)
                {
                    if (drTemplate.GUID != editTemplateGuid)
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
                if (_editTemplate != null || ValidateTemplate())
                {
                    if (_editTemplate != null)
                    {
                        if (ValidateEditTemplate(_editTemplate.GUID))
                        {
                            _editTemplate.templateName = txtName.Text.Trim();
                            _editTemplate.templateRevision = txtRevision.Text.Trim();
                            _editTemplate.templateDescription = txtDescription.Text.Trim();
                            _editTemplate.templateQRSupport = checkEditQR.Checked;
                            _editTemplate.templateSkipApproved = checkEditSkipApproved.Checked;
                            _editTemplate.templateWorkFlow = (ValuePair)cmbWorkflow.SelectedItem;
                            _Template = _editTemplate;
                        }
                        else
                        {
                            Common.Prompt("The name specified has already been used by other template\n\nPlease type in a different name");
                            return;
                        }
                            
                    }
                    else
                    {
                        Template newTemplate = new Template(Guid.NewGuid())
                        {
                            templateName = txtName.Text.Trim(),
                            templateRevision = txtRevision.Text.Trim(),
                            templateDescription = txtDescription.Text.Trim(),
                            templateQRSupport = checkEditQR.Checked,
                            templateSkipApproved = checkEditSkipApproved.Checked,
                            templateWorkFlow = (ValuePair)cmbWorkflow.SelectedItem
                        };

                        _Template = newTemplate;
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                    Common.Warn("Name already exists"); ;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion
    }
}
