using ProjectCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckmateDX
{
    public partial class frmCertificateRoleSelection : Form
    {
        public frmCertificateRoleSelection()
        {
            InitializeComponent();
            Common.PopulateCmbCVCRoleEnum(cmbRole);
        }

        public string GetSelectedCVCRole()
        {
            if (cmbRole.EditValue == null)
                return string.Empty;

            return cmbRole.EditValue.ToString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
