using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectCommon;
using ProjectLibrary;

namespace CheckmateDX
{
    public partial class frmITR_Select_Discipline : CheckmateDX.frmParent
    {
        string _discipline = string.Empty;

        public frmITR_Select_Discipline()
        {
            InitializeComponent();
            PopulateFormElements();
        }

        /// <summary>
        /// Populate form element for adding
        /// </summary>
        private void PopulateFormElements()
        {
            Common.PopulateCmbAuthDiscipline(cmbDiscipline);
        }

        /// <summary>
        /// For other interface to retrieve user selected discipline
        /// </summary>
        public string GetDiscipline()
        {
            return _discipline;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _discipline = cmbDiscipline.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
