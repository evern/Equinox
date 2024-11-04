using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace AutoUpdate
{
    public partial class Authentication : DevExpress.XtraEditors.XtraForm
    {
        public Authentication()
        {
            InitializeComponent();
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Return)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        public string GetPassword()
        {
            return txtPassword.Text.Trim();
        }
    }
}