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
using ProjectDatabase.Dataset;

namespace CheckmateDX
{
    public partial class frmSchedule_Matrix_Add : CheckmateDX.frmParent
    {
        private MatrixType _editMatrixType;
        private MatrixType _matrixType;

        public frmSchedule_Matrix_Add()
        {
            InitializeComponent();
            Common.PopulateCmbAuthDiscipline(cmbDiscipline);
        }

        public frmSchedule_Matrix_Add(MatrixType editMatrixType)
        {
            InitializeComponent();
            this.Text = "Edit Equipment Type";
            btnOk.Text = "Accept";
            PopulateFormElements(editMatrixType);
            Common.PopulateCmbAuthDiscipline(cmbDiscipline);
            _editMatrixType = editMatrixType;
        }

        /// <summary>
        /// Populate form element for editing
        /// </summary>
        private void PopulateFormElements(MatrixType editMatrixType)
        {
            pnlName.Visible = false;
            txtName.Text = editMatrixType.typeName;
            txtDescription.Text = editMatrixType.typeDescription;
            txtCategory.Text = editMatrixType.typeCategory;
        }

        public MatrixType GetMatrixType()
        {
            return _matrixType;
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

            if (cmbDiscipline.Text.Trim() == string.Empty) 
            {
                Common.Warn("Please select a discipline");
                cmbDiscipline.Focus();
                return false;
            }
             
            return true;
        }

        /// <summary>
        /// Checks for duplicate prefill
        /// </summary>
        private bool ValidateMatrixType()
        {
            //System prefill name
            if (txtName.Text.Trim() == Variables.prefillTagNumber || txtName.Text.Trim() == Variables.prefillTagDescription)
                return false;

            if (txtName.Text.Trim() == Variables.prefillProjNumber || txtName.Text.Trim() == Variables.prefillProjName || txtName.Text.Trim() == Variables.prefillProjClient)
                return false;

            if (txtName.Text.Trim() == Variables.prefillDate || txtName.Text.Trim() == Variables.prefillDateTime)
                return false;

            using (AdapterMATRIX_TYPE daMatrixType = new AdapterMATRIX_TYPE())
            {
                if (daMatrixType.GetBy(txtName.Text.Trim()) != null)
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
                if (_editMatrixType != null || ValidateMatrixType())
                {
                    if (_editMatrixType != null)
                    {
                        _editMatrixType.typeDescription = txtDescription.Text.Trim();
                        _editMatrixType.typeCategory = txtCategory.Text.Trim();
                        _editMatrixType.typeDiscipline = cmbDiscipline.Text;
                        _matrixType = _editMatrixType;
                    }
                    else
                    {
                        MatrixType newMatrixType = new MatrixType(Guid.NewGuid())
                        {
                            typeName = txtName.Text.Trim(),
                            typeDescription = txtDescription.Text.Trim(),
                            typeCategory = txtCategory.Text.Trim(),
                            typeDiscipline = cmbDiscipline.Text.Trim()
                        };

                        _matrixType = newMatrixType;
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                    Common.Warn("Another equipment type with the same name already exists, please use a different name"); ;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion
    }
}
