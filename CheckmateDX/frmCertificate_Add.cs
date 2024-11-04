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
    public partial class frmCertificate_Add : CheckmateDX.frmParent
    {
        private ViewModel_Certificate _editViewModelCertificate;
        private ViewModel_Certificate _viewModelCertificate;

        public frmCertificate_Add(string certificateName)
        {
            InitializeComponent();
            this.Text = "Add " + certificateName;
            this.btnOk.Text = "Add";
        }

        /// <summary>
        /// This is used for editing
        /// </summary>
        public frmCertificate_Add(ViewModel_Certificate viewModelCertificate)
        {
            InitializeComponent(); 
            
            if (viewModelCertificate.GetType() == typeof(ViewModel_CVC))
            {
                this.Text = "Edit Construction Verification Certificate";
            }
            else if (viewModelCertificate.GetType() == typeof(ViewModel_NOE))
            {
                this.Text = "Edit Notice of Energisation";
            }
            else if (viewModelCertificate.GetType() == typeof(ViewModel_PunchlistWalkdown))
            {
                this.Text = "Edit Punchlist Walkdown";
            }

            this.btnOk.Text = "Edit";
            _editViewModelCertificate = viewModelCertificate;
            txtDescription.Text = _editViewModelCertificate.Description;
        }

        public ViewModel_Certificate GetCertificate()
        {
            return _viewModelCertificate;
        }

        #region Validation
        /// <summary>
        /// Checks whether form is properly filled
        /// </summary>
        private bool ValidateFormElements()
        {
            if (txtDescription.Text.Trim() == string.Empty)
            {
                Common.Warn("Name cannot be empty");
                txtDescription.Focus();
                return false;
            }

            return true;
        }
        #endregion

        #region Events
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ValidateFormElements())
            {
                if (_editViewModelCertificate != null)
                {
                    _editViewModelCertificate.Description = txtDescription.Text.Trim();
                    _viewModelCertificate = _editViewModelCertificate;
                }
                else
                {
                    ViewModel_Certificate newViewModelCertificate = new ViewModel_Certificate()
                    {
                        Description = txtDescription.Text.Trim(),
                    };

                    _viewModelCertificate = newViewModelCertificate;
                }

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion
    }
}
