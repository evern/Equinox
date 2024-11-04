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
    public partial class frmPrefill_Add : CheckmateDX.frmParent
    {
        private Prefill _editPrefill;
        private Prefill _prefill;

        public frmPrefill_Add()
        {
            InitializeComponent();
        }

        public frmPrefill_Add(Prefill editPrefill)
        {
            InitializeComponent();
            this.Text = "Edit Prefill";
            btnOk.Text = "Accept";
            PopulateFormElements(editPrefill);
            _editPrefill = editPrefill;
        }

        /// <summary>
        /// Populate form element for editing
        /// </summary>
        private void PopulateFormElements(Prefill editPrefill)
        {
            //pnlName.Visible = false;
            txtName.Text = editPrefill.prefillName;
            txtCategory.Text = editPrefill.prefillCategory;
        }

        public Prefill GetPrefill()
        {
            return _prefill;
        }

        #region Validation
        /// <summary>
        /// Checks whether form is properly filled
        /// </summary>
        private bool ValidateFormElements()
        {
            if (txtName.Text.Trim() == string.Empty)
            {
                Common.Warn("Name cannot be empty");
                txtName.Focus();
                return false;
            }

            if (txtCategory.Text.Trim() == string.Empty)
            {
                Common.Warn("Category cannot be empty");
                txtCategory.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for duplicate prefill
        /// </summary>
        private bool ValidatePrefill()
        {
            //System prefill name
            if (txtName.Text.Trim() == Variables.prefillTagNumber || txtName.Text.Trim() == Variables.prefillTagDescription)
                return false;

            if (txtName.Text.Trim() == Variables.prefillProjNumber || txtName.Text.Trim() == Variables.prefillProjName || txtName.Text.Trim() == Variables.prefillProjClient)
                return false;

            if (txtName.Text.Trim() == Variables.prefillDate || txtName.Text.Trim() == Variables.prefillDateTime || txtName.Text.Trim() == Variables.prefillChild)
                return false;

            using (AdapterPREFILL_MAIN daPrefill = new AdapterPREFILL_MAIN())
            {
                if (daPrefill.GetBy(txtName.Text.Trim()) != null)
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
                if (_editPrefill != null || ValidatePrefill())
                {
                    if (_editPrefill != null)
                    {
                        _editPrefill.prefillName = txtName.Text.Trim();
                        _editPrefill.prefillCategory = txtCategory.Text.Trim();
                        _prefill = _editPrefill;
                    }
                    else
                    {
                        Prefill newPrefill = new Prefill(Guid.NewGuid())
                        {
                            prefillName = txtName.Text.Trim(),
                            prefillCategory = txtCategory.Text.Trim()
                        };

                        _prefill = newPrefill;
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                    Common.Warn("Another prefill with the same name already exists, please use a different name"); ;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion
    }
}
