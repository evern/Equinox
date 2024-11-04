using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectCommon;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;

namespace CheckmateDX
{
    public partial class frmTool_User_ChangePassword : CheckmateDX.frmParent
    {
        AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN();
        User _editUser;
        public frmTool_User_ChangePassword()
        {
            InitializeComponent();
        }

        public frmTool_User_ChangePassword(User editUser)
        {
            InitializeComponent();
            pnlNewPassword1.Visible = false;
            pnlNewPassword2.Visible = false;
            lblCurrentPassword.Text = "User's Password";
            btnOk.Text = "Delete";
            _editUser = editUser;
        }

        #region Validation
        /// <summary>
        /// Checks whether form is properly filled
        /// </summary>
        public bool ValidateFormElements()
        {
            if (txtCurrentPassword.Text.Trim() == string.Empty)
            {
                Common.Warn("Current password must be entered");
                txtCurrentPassword.Focus();
                return false;
            }

            if (txtNewPassword1.Text.Trim() == string.Empty)
            {
                Common.Warn("Password must be entered");
                txtNewPassword1.Focus();
                return false;
            }

            if (txtNewPassword2.Text.Trim() == string.Empty)
            {
                Common.Warn("Please repeat your password");
                txtNewPassword2.Focus();
                return false;
            }

            if (txtNewPassword1.Text.Trim() != txtNewPassword2.Text.Trim())
            {
                Common.Warn("Incorrect repeated password");
                txtNewPassword2.Focus();
                return false;
            }

            return true;
        }
        /// <summary>
        /// Checks for correct current password
        /// </summary>
        private dsUSER_MAIN.USER_MAINRow ValidatePassword()
        {
            dsUSER_MAIN.USER_MAINRow currentUser = _daUser.VerifyLogin(System_Environment.GetUser().userQANumber, Common.Encrypt(txtCurrentPassword.Text.Trim(), true));
            if (currentUser != null)
                return currentUser;

            return null;
        }

        #endregion

        #region Events
        private void btnOk_Click(object sender, EventArgs e)
        {
            if(_editUser != null)
            {
                if (Common.Encrypt(txtCurrentPassword.Text.Trim(), true) != _editUser.userPassword)
                    Common.Prompt("Incorrect Password");
                else
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else if (ValidateFormElements())
            {
                dsUSER_MAIN.USER_MAINRow drUser = ValidatePassword();
                if (drUser != null)
                {
                    drUser.PASSWORD = Common.Encrypt(txtNewPassword1.Text.Trim(), true);
                    drUser.UPDATED = DateTime.Now;
                    drUser.UPDATEDBY = drUser.GUID;
                    _daUser.Save(drUser);
                    //Common.Prompt("Password successfully changed");
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                    Common.Warn("Incorrect current password"); ;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void txtCurrentPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnOk_Click(null, null);
        }

        private void txtNewPassword1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnOk_Click(null, null);
        }

        private void txtNewPassword2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnOk_Click(null, null);
        }
        #endregion
    }
}
