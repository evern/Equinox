using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using System.Linq;

namespace CheckmateDX
{
    public partial class frmTool_User : CheckmateDX.frmTool_Main
    {
        AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN();
        dsUSER_MAIN _dsUser = new dsUSER_MAIN();

        List<User> _allUser = new List<User>();

        public frmTool_User()
        {
            InitializeComponent();
            userBindingSource.DataSource = _allUser;
            RefreshUsers();
        }

        #region Form Population
        /// <summary>
        /// Refresh all users from database
        /// </summary>
        private void RefreshUsers()
        {
            _allUser.Clear();
            dsUSER_MAIN.USER_MAINDataTable dtUser;

            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
                dtUser = _daUser.Get();
            else
                dtUser = _daUser.GetAuthUsers();

            if(dtUser != null)
            {
                foreach (dsUSER_MAIN.USER_MAINRow drUser in dtUser.Rows)
                {
                    _allUser.Add(new User(drUser.GUID)
                    {
                        userFirstName = drUser.FIRSTNAME,
                        userLastName = drUser.LASTNAME,
                        userQANumber = drUser.QANUMBER,
                        userPassword = drUser.PASSWORD,
                        userRole = new ValuePair(Common.ConvertRoleGuidToName(drUser.ROLE), (Guid)drUser.ROLE),
                        userEmail = drUser.EMAIL,
                        userCompany = drUser.COMPANY,
                        userProject = new ValuePair(Common.ConvertProjectGuidToName(drUser.DPROJECT, (Guid)drUser.ROLE), (Guid)drUser.DPROJECT),
                        userDiscipline = Common.ConvertDBDisciplineForDisplay(drUser.DDISCIPLINE, (Guid)drUser.ROLE),
                        userInfo = drUser.IsINFONull() ? string.Empty : drUser.INFO,
                        CreatedDate = drUser.CREATED,
                        CreatedBy = drUser.CREATEDBY
                    }
                    );
                }

                gridControl.RefreshDataSource();
            }
        }
        #endregion

        #region Button Overrides
        protected override void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmTool_User_Add frmUserAdd = new frmTool_User_Add();
            if (frmUserAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                User newUser = frmUserAdd.GetUser();
                dsUSER_MAIN.USER_MAINRow drUser = _dsUser.USER_MAIN.NewUSER_MAINRow();
                drUser.GUID = Guid.NewGuid();
                AssignUserDetails(drUser, newUser);
                drUser.DPROJECT = (Guid)newUser.userProject.Value;
                drUser.DDISCIPLINE = Common.ConvertDisplayDisciplineForDB(newUser.userDiscipline);
                drUser.PASSWORD = Common.Encrypt(Variables.defaultPassword, true);
                drUser.CREATED = DateTime.Now;
                drUser.CREATEDBY = System_Environment.GetUser().GUID;
                _dsUser.USER_MAIN.AddUSER_MAINRow(drUser);
                _daUser.Save(drUser);
                addUserDisciplines(drUser, newUser.allDisciplinePermission);
                RefreshUsers();
                //Common.Prompt("User successfully added");
            }

            base.btnAdd_ItemClick(sender, e);
        }

        protected override void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridView1.SelectedRowsCount == 0)
            {
                Common.Warn("Please select User to edit");
                return;
            }

            frmTool_User_Add frmUserAdd = new frmTool_User_Add((User)gridView1.GetFocusedRow());
            if (frmUserAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                User editUser = frmUserAdd.GetUser();

                dsUSER_MAIN.USER_MAINRow drUser = _daUser.GetBy(editUser.GUID);
                if(drUser != null)
                {
                    AssignUserDetails(drUser, editUser);
                    drUser.DDISCIPLINE = Common.ConvertDisplayDisciplineForDB(editUser.userDiscipline);
                    drUser.PASSWORD = editUser.userPassword;
                    drUser.UPDATED = DateTime.Now;
                    drUser.UPDATEDBY = System_Environment.GetUser().GUID;
                    _daUser.Save(drUser);
                    addUserDisciplines(drUser, editUser.allDisciplinePermission);
                    RefreshUsers();
                    //Common.Prompt("User successfully updated");
                }
            }
            base.btnEdit_ItemClick(sender, e);
        }

        protected override void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridView1.SelectedRowsCount == 0)
            {
                Common.Warn("Please select user(s) to delete");
                return;
            }

            //frmTool_User_ChangePassword frmVerifyPassword = new frmTool_User_ChangePassword((User)gridView1.GetFocusedRow());
            //if (frmVerifyPassword.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            //    return;

            if (!Common.Confirmation("Are you sure you want to delete selected user(s)?", "Confirmation"))
                return;

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable(); //what project to delete
            int[] selectedRowIndexes = gridView1.GetSelectedRows();

            foreach (int selectedRowIndex in selectedRowIndexes)
            {
                User selectedUser = (User)gridView1.GetRow(selectedRowIndex);
                dsUSER_MAIN.USER_MAINRow drUser = dtUser.NewUSER_MAINRow();
                //only guid is needed but other details are posted for information purpose
                drUser.GUID = selectedUser.GUID;
                drUser.QANUMBER = selectedUser.userQANumber;
                drUser.FIRSTNAME = selectedUser.userFirstName;
                drUser.LASTNAME = selectedUser.userLastName;
                drUser.PASSWORD = selectedUser.userPassword;
                drUser.ROLE = (Guid)((ValuePair)selectedUser.userRole).Value;
                drUser.COMPANY = selectedUser.userCompany;
                drUser.EMAIL = selectedUser.userEmail;
                drUser.DPROJECT = (Guid)((ValuePair)selectedUser.userProject).Value;
                drUser.DDISCIPLINE = selectedUser.userDiscipline;
                drUser.CREATED = selectedUser.CreatedDate;
                drUser.CREATEDBY = selectedUser.CreatedBy;
                dtUser.AddUSER_MAINRow(drUser);
            }

            rptDeletion f = new rptDeletion(dtUser);
            f.ShowReport();

            RefreshUsers();
            gridView1.ClearSelection();
            base.btnDelete_ItemClick(sender, e);
        }

        private void addUserDisciplines(dsUSER_MAIN.USER_MAINRow drUser, bool addAllDiscipline)
        {
            using (AdapterUSER_MAIN daUser = new AdapterUSER_MAIN())
            {
                List<string> allDisciplines = Enum.GetNames(typeof(Discipline)).ToList();
                List<string> userAuthorisedDisciplines = new List<string>();
                dsUSER_MAIN.USER_DISCDataTable dtUserDisc = daUser.GetUserDisciplines(drUser.GUID);

                if (dtUserDisc != null)
                {
                    foreach (dsUSER_MAIN.USER_DISCRow drUserDisc in dtUserDisc.Rows)
                    {
                        userAuthorisedDisciplines.Add(drUserDisc.DISCIPLINE);
                    }
                }

                //add user discipline
                if (addAllDiscipline)
                {
                    foreach (string discipline in allDisciplines)
                    {
                        if (!userAuthorisedDisciplines.Contains(discipline))
                        {
                            daUser.SetUserDiscipline(drUser.GUID, discipline);
                        }
                    }
                }
                else
                {
                    if (!drUser.IsDDISCIPLINENull() || drUser.DDISCIPLINE != string.Empty)
                    {
                        if (!userAuthorisedDisciplines.Contains(drUser.DDISCIPLINE))
                        {
                            daUser.SetUserDiscipline(drUser.GUID, drUser.DDISCIPLINE);
                        }
                    }
                }
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Assigns common user details to data row
        /// </summary>
        /// <param name="drUserMaster">datarow to be assigned</param>
        /// <param name="user">user details</param>
        private void AssignUserDetails(dsUSER_MAIN.USER_MAINRow drUser, User user)
        {
            drUser.FIRSTNAME = user.userFirstName;
            drUser.LASTNAME = user.userLastName;
            drUser.QANUMBER = user.userQANumber;
            drUser.ROLE = (Guid)((ValuePair)user.userRole).Value;
            drUser.EMAIL = user.userEmail;
            drUser.COMPANY = user.userCompany;
            drUser.INFO = user.userInfo;
            drUser.DDISCIPLINE = user.userDiscipline;
        }
        #endregion
    }
}
