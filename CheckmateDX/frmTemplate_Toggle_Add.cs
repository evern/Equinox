using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectCommon;
using ProjectLibrary;

namespace CheckmateDX
{
    public partial class frmTemplate_Toggle_Add : DevExpress.XtraEditors.XtraForm
    {
        private Template_Toggle _editTemplate_Toggle;
        private Template_Toggle _toggle;
        private string _discipline;
        CustomUserGroupListService customUserGroup = new CustomUserGroupListService();

        public frmTemplate_Toggle_Add(string discipline)
        {
            InitializeComponent();
            _discipline = discipline;
        }

        public frmTemplate_Toggle_Add(Template_Toggle editTemplate_Toggle)
        {
            InitializeComponent();
            Text = "Edit Template Toggle";
            btnOk.Text = "Accept";

            PopulateFormElements(editTemplate_Toggle);
            _editTemplate_Toggle = editTemplate_Toggle;
            _discipline = editTemplate_Toggle.toggleDiscipline;
        }

        /// <summary>
        /// Populate for element for editing
        /// </summary>
        private void PopulateFormElements(Template_Toggle editTemplate_Toggle)
        {
            txtName.Text = editTemplate_Toggle.toggleName;    
        }

        public Template_Toggle GetTemplate_Toggle()
        {
            return _toggle;
        }

        /// <summary>
        /// Checks whether form is properly filled
        /// </summary>
        private bool ValidateFormElements()
        {
            List<string> ReservedWord = customUserGroup.GetUserGroupList();
            if (ReservedWord.Any(obj => obj == txtName.Text.Trim().ToUpper()))
            {
                Common.Warn("Name is reserved for application purpose");
                txtName.Focus();
                return false;
            }

            if (txtName.Text.Trim() == string.Empty)
            {
                Common.Warn("Name cannot be empty");
                txtName.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for duplicate toggle
        /// </summary>
        private bool ValidateTemplate_Toggle()
        {
            using(AdapterTEMPLATE_TOGGLE daTemplate_Toggle = new AdapterTEMPLATE_TOGGLE())
            {
                if (daTemplate_Toggle.GetBy_DisciplineAndName(_discipline, txtName.Text.Trim()) != null)
                {
                    return false;
                }
            }

            return true;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ValidateFormElements())
            {
                if (ValidateTemplate_Toggle())
                {
                    if (_editTemplate_Toggle != null)
                    {
                        _editTemplate_Toggle.toggleName = txtName.Text.Trim();
                        _editTemplate_Toggle.toggleDescription = txtDescription.Text.Trim();
                        _toggle = _editTemplate_Toggle;
                    }
                    else
                    {
                        var newTemplate_Toggle = new Template_Toggle(Guid.NewGuid())
                        {
                            toggleName = txtName.Text.Trim(),
                            toggleDescription = txtDescription.Text.Trim()
                        };

                        _toggle = newTemplate_Toggle;
                    }

                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    Common.Warn("Toggle already exist");
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}