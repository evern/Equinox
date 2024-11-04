using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace CheckmateDX
{
    public partial class frmSyncTestCount : DevExpress.XtraEditors.XtraForm
    {
        public frmSyncTestCount()
        {
            InitializeComponent();
        }

        public decimal GetTestCount()
        {
            return spinEdit1.Value;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}