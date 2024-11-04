using System;
using System.Collections.Generic;
using ProjectDatabase.DataAdapters;
using ProjectCommon;
using ProjectLibrary;
using DevExpress.XtraEditors.Controls;

namespace CheckmateDX
{
    public partial class frmTool_User_Add : CheckmateDX.frmParent
    {
        private User _editUser;

        private User _user;
        public frmTool_User_Add()
        {
            InitializeComponent();
            Common.PopulateCmbAuthRole(cmbRole, null, null);
            PopulateFormElements();
            txtCompany.Text = Variables.defaultUserCompany;
            cmbRole.SelectedIndexChanged += cmbRole_SelectedIndexChanged;
        }

        public frmTool_User_Add(User editUser)
        {
            InitializeComponent();
            Text = "Edit User";
            btnOk.Text = "Accept";

            Common.PopulateCmbAuthRole(cmbRole, null, (Guid)editUser.userRole.Value);
            PopulateFormElements(editUser);
            _editUser = editUser;

            pnlQANumber.Visible = false;
            pnlProject.Visible = false;
            pnlDiscipline.Visible = true;

            //if (editUser.userDiscipline == Discipline.Electrical.ToString())
            //    pnlEWID.Visible = true;
            //else
            //    pnlEWID.Visible = false;

            if(System_Environment.HasPrivilege(PrivilegeTypeID.ResetPassword))
                btnReset.Visible = true;
            cmbRole.SelectedIndexChanged += cmbRole_SelectedIndexChanged;
        }

        /// <summary>
        /// Populate form element for adding
        /// </summary>
        private void PopulateFormElements()
        {
            if(cmbRole.Text != Variables.AdminSuperuser)
            {
                //cmbDiscipline.SelectedValueChanged -= cmbDiscipline_SelectedValueChanged;
                Common.PopulateCmbAuthDiscipline(cmbDiscipline);
                cmbDiscipline.SelectedValueChanged += cmbDiscipline_SelectedValueChanged;
                Common.PopulateCmbAuthProject(cmbProject, true);

                if(_editUser == null)
                {
                    pnlDiscipline.Visible = true;
                    pnlProject.Visible = true;
                }
            }
            else
            {
                ComboBoxItemCollection coll = cmbDiscipline.Properties.Items;
                coll.BeginUpdate();
                coll.Clear();
                coll.Add("");
                coll.EndUpdate();
                cmbDiscipline.SelectedIndex = 0;

                ComboBoxItemCollection coll1 = cmbProject.Properties.Items;
                coll1.BeginUpdate();
                coll1.Clear();
                coll1.Add(new ValuePair("All Projects", Guid.Empty));
                coll1.EndUpdate();
                cmbProject.SelectedIndex = 0;

                pnlDiscipline.Visible = false;
                pnlProject.Visible = false;
            }
        }

        /// <summary>
        /// Populate for element for editing
        /// </summary>
        private void PopulateFormElements(User editUser)
        {
            PopulateFormElements();
            txtQANumber.Text = editUser.userQANumber;
            txtFirstName.Text = editUser.userFirstName;
            txtLastName.Text = editUser.userLastName;
            txtEmail.Text = editUser.userEmail;
            txtCompany.Text = editUser.userCompany;
            txtEWID.Text = editUser.userInfo;

            ComboBoxItemCollection coll = cmbDiscipline.Properties.Items;
            if (editUser.userDiscipline != string.Empty)
                cmbDiscipline.SelectedIndex = coll.IndexOf(editUser.userDiscipline);
            else
                cmbDiscipline.SelectedIndex = 0;
        }

        public User GetUser()
        {
            return _user;
        }

        /// <summary>
        /// Checks whether form is properly filled
        /// </summary>
        private bool ValidateFormElements()
        {
            if (txtQANumber.Text.Trim() == string.Empty)
            {
                Common.Warn("Username cannot be empty");
                txtQANumber.Focus();
                return false;
            }

            if (txtFirstName.Text.Trim() == string.Empty)
            {
                Common.Warn("First name cannot be empty");
                txtFirstName.Focus();
                return false;
            }

            if (txtLastName.Text.Trim() == string.Empty)
            {
                Common.Warn("Last name cannot be empty");
                txtLastName.Focus();
                return false;
            }

            if (cmbRole.Text.Trim() == string.Empty)
            {
                Common.Warn("Role cannot be empty");
                cmbRole.Focus();
                return false;
            }

            if (cmbRole.Text != Variables.AdminSuperuser && cmbDiscipline.Text.Trim() == string.Empty)
            {
                Common.Warn("Discipline cannot be empty");
                cmbDiscipline.Focus();
                return false;
            }

            if (txtEmail.Text.Trim() != string.Empty && !txtEmail.Text.Contains("@"))
            {
                Common.Warn("Invalid email address");
                txtEmail.Focus();
                return false;
            }

            if (txtCompany.Text.Trim() == string.Empty)
            {
                Common.Warn("Company cannot be empty");
                txtCompany.Focus();
                return false;
            }

            if (_editUser == null && cmbDiscipline.Text.Trim() == Discipline.Electrical.ToString() && txtEWID.Text.Trim() == string.Empty)
            {
                Common.Warn("EW cannot be empty");
                txtEWID.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for duplicate user
        /// </summary>
        private bool ValidateUser()
        {
            using(AdapterUSER_MAIN daUser = new AdapterUSER_MAIN())
            {
                if (daUser.GetBy(txtQANumber.Text.Trim()) != null)
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
                if (_editUser != null || ValidateUser())
                {
                    if (_editUser != null)
                    {
                        _editUser.userFirstName = txtFirstName.Text.Trim();
                        _editUser.userLastName = txtLastName.Text.Trim();
                        _editUser.userRole = (ValuePair)cmbRole.SelectedItem;
                        _editUser.userEmail = txtEmail.Text.Trim();
                        _editUser.userCompany = txtCompany.Text.Trim();
                        _editUser.userInfo = txtEWID.Text.Trim();
                        _editUser.userDiscipline = Common.ReplaceSpacesWith_(cmbDiscipline.Text);
                        _editUser.allDisciplinePermission = checkEditAllDiscipline.Checked;
                        _user = _editUser;
                    }
                    else
                    {
                        var newUser = new User(Guid.NewGuid())
                        {
                            userFirstName = txtFirstName.Text.Trim(),
                            userLastName = txtLastName.Text.Trim(),
                            userQANumber = txtQANumber.Text.Trim(),
                            userRole = (ValuePair)cmbRole.SelectedItem,
                            userEmail = txtEmail.Text.Trim(),
                            userCompany = txtCompany.Text.Trim(),
                            userProject = (ValuePair)cmbProject.SelectedItem,
                            userDiscipline = Common.ReplaceSpacesWith_(cmbDiscipline.Text),
                            userInfo = Common.ReplaceSpacesWith_(txtEWID.Text),
                            allDisciplinePermission = checkEditAllDiscipline.Checked
                        };

                        _user = newUser;
                    }

                    DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    Common.Warn("User already exists");
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if(_editUser != null && Common.Confirmation("Are you sure you want to reset the user password to\n\n" + Variables.defaultPassword + "?", "Reset Password"))
            {
                _editUser.userPassword = Common.Encrypt(Variables.defaultPassword, true);
                btnOk.Focus();
                Common.Prompt("Password changed\n\nPress 'Accept' to save it");
            }
        }

        private void cmbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateFormElements();
        }

        private void cmbDiscipline_SelectedValueChanged(object sender, EventArgs e)
        {
            if(cmbDiscipline.Text == Common.Replace_WithSpaces(Discipline.Electrical.ToString()))
                pnlEWID.Visible = true;
            else
            {
                pnlEWID.Visible = false;
                txtEWID.Text = string.Empty;
            }
        }
    }
}
