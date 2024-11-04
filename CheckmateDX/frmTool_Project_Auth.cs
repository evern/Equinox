using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using System.Linq;

namespace CheckmateDX
{
    public partial class frmTool_Project_Auth : CheckmateDX.frmParent
    {
        //drag and drop
        GridHitInfo _downHitInfo = null;
        //database
        AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN();
        dsUSER_MAIN _dsUser = new dsUSER_MAIN();
        AdapterPROJECT _daProject = new AdapterPROJECT();
        dsPROJECT _dsProject = new dsPROJECT();

        //class lists
        List<User> _includedUsers = new List<User>();
        List<User> _excludedUsers = new List<User>();
        List<Project> _allProject = new List<Project>();
        public frmTool_Project_Auth()
        {
            InitializeComponent();
            projectBindingSource.DataSource = _allProject;
            userBindingAuth.DataSource = _includedUsers;
            userBindingUnauth.DataSource = _excludedUsers;
            RefreshProjects();
            RefreshUsers();
        }

        #region Form Population
        /// <summary>
        /// Refresh all project from database
        /// </summary>
        private void RefreshProjects()
        {
            _allProject.Clear();
            dsPROJECT.PROJECTDataTable dtProject = _daProject.Get();
            foreach (dsPROJECT.PROJECTRow drProject in dtProject.Rows)
            {
                _allProject.Add(new Project(drProject.GUID)
                {
                    projectNumber = drProject.NUMBER,
                    projectName = drProject.NAME,
                    projectClient = drProject.CLIENT
                });
            }

            gridProject.RefreshDataSource();
        }
        /// <summary>
        /// Refresh all users from database
        /// </summary>
        private void RefreshUsers()
        {
            _includedUsers.Clear();
            _excludedUsers.Clear();

            Project selectedProject = GetSelectedProject();
            if (selectedProject == null)
                return;

            dsUSER_MAIN.USER_MAINDataTable dtUserProject = _daUser.GetByProject(selectedProject.GUID);
            //dsUSER_MAIN.USER_MAINDataTable dtUserAll = _daUser.GetAuthUsersWithProjectRestrictedRole();
            dsUSER_MAIN.USER_MAINDataTable dtUserAll = _daUser.GetAuthUsers();

            if (dtUserProject != null)
            {
                foreach (dsUSER_MAIN.USER_MAINRow drUser in dtUserProject.Rows)
                {
                    //only add the user if user is part of the authorised list
                    if(dtUserAll != null && dtUserAll.Any(obj => obj.GUID == drUser.GUID))
                        AddUserToList(_includedUsers, drUser);
                }
            }

            if(dtUserAll != null)
            {
                foreach (dsUSER_MAIN.USER_MAINRow drUser in dtUserAll.Rows)
                {
                    if (!_includedUsers.Any(obj => obj.GUID == drUser.GUID))
                    {
                        AddUserToList(_excludedUsers, drUser);
                    }
                }
            }

            gridAuth.RefreshDataSource();
            gridUnauth.RefreshDataSource();
        } 
        #endregion

        #region Drag'n'Drop
        /// <summary>
        /// Action handler to validate area which user starts dragging
        /// </summary>
        private void grid_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            _downHitInfo = null;

            GridHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
            if (Control.ModifierKeys != Keys.None) return;
            if (e.Button == MouseButtons.Left && hitInfo.InRow && hitInfo.HitTest != GridHitTest.RowIndicator)
                _downHitInfo = hitInfo;
        }

        /// <summary>
        /// Action handler for when user draging starts on a valid area
        /// </summary>
        private void grid_MouseMove(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Button == MouseButtons.Left && _downHitInfo != null)
            {
                Size dragSize = SystemInformation.DragSize;
                Rectangle dragRect = new Rectangle(new Point(_downHitInfo.HitPoint.X - dragSize.Width / 2,
                    _downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                if (!dragRect.Contains(new Point(e.X, e.Y)))
                {
                    view.GridControl.DoDragDrop(GetDragData(view), DragDropEffects.All);
                    _downHitInfo = null;
                }
            }
        }

        /// <summary>
        /// Action handler for when user starts dropping
        /// </summary>
        private void grid_DragDrop(object sender, DragEventArgs e)
        {
            GridControl grid = (GridControl)sender;
            BindingSource binding = (BindingSource)grid.DataSource;
            List<User> users = (List<User>)binding.DataSource;
            DragData data = (DragData)e.Data.GetData(typeof(DragData));

            List<User> userFrom;
            List<User> userTo;
            if (grid.Name == gridUnauth.Name)
            {
                userFrom = _includedUsers;
                userTo = _excludedUsers;
            }
            else
            {
                userFrom = _excludedUsers;
                userTo = _includedUsers;
            }

            if (data != null && binding != null)
            {
                for (int i = 0; i < data.dataRowIndexes.GetLength(0); i++)
                {
                    userTo.Add(userFrom[data.dataRowIndexes[i]]);
                }

                foreach (User user in userTo)
                {
                    User findIncluded = userFrom.FirstOrDefault(obj => obj.GUID == user.GUID);
                    if (findIncluded != null)
                    {
                        userFrom.Remove(findIncluded);

                        //database operation
                        Project selectedProject = GetSelectedProject();
                        if(selectedProject != null)
                        {
                            if(grid.Name == gridAuth.Name)
                            {
                                _daUser.SetUserProject(findIncluded.GUID, selectedProject.GUID);
                            }
                            else
                            {
                                _daUser.RemoveUserProject(findIncluded.GUID, selectedProject.GUID);
                            }

                            _daUser.UpdateUser(findIncluded.GUID);
                        }
                    }
                }
            }

            gridAuth.RefreshDataSource();
            gridUnauth.RefreshDataSource();
        }

        /// <summary>
        /// Dragging UI representation
        /// </summary>
        private void grid_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragData)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Retrive selected indexes when user start draging the data
        /// </summary>
        private DragData GetDragData(GridView view)
        {
            int[] selection = view.GetSelectedRows();
            if (selection == null)
                return null;

            int count = selection.Length;
            DragData result = new DragData() { sourceData = (BindingSource)view.GridControl.DataSource, dataRowIndexes = new int[count] };

            for (int i = 0; i < count; i++)
            {
                result.dataRowIndexes[i] = view.GetDataSourceRowIndex(selection[i]);
            }

            return result;
        } 
        #endregion

        #region Helper
        /// <summary>
        /// Adds datarow user into list
        /// </summary>
        private void AddUserToList(List<User> userList, dsUSER_MAIN.USER_MAINRow drUser)
        {
            userList.Add(new User(drUser.GUID)
            {
                //data is posted for information only, what is shown on the list determine which is necessary
                userFirstName = drUser.FIRSTNAME,
                userLastName = drUser.LASTNAME,
                userQANumber = drUser.QANUMBER,
                userPassword = drUser.PASSWORD,
                userRole = new ValuePair(Common.ConvertRoleGuidToName(drUser.ROLE), (Guid)drUser.ROLE),
                userEmail = drUser.EMAIL,
                userCompany = drUser.COMPANY,
                userProject = new ValuePair(Common.ConvertProjectGuidToName(drUser.DPROJECT), (Guid)drUser.DPROJECT),
                userDiscipline = Common.Replace_WithSpaces(drUser.DDISCIPLINE)
            }
            );
        }

        /// <summary>
        /// Get user selected project from gridview as class object
        /// </summary>
        private Project GetSelectedProject()
        {
            if (gridView1.SelectedRowsCount == 0)
                return null;

            return (Project)gridView1.GetFocusedRow();
        }
        #endregion

        #region Events
        private void gridProject_Click(object sender, EventArgs e)
        {
            RefreshUsers();
        }
        #endregion
    }
}
