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
    public partial class frmTool_Discipline_Auth : CheckmateDX.frmParent
    {
        //drag and drop
        GridHitInfo _downHitInfo = null;

        //database
        AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN();
        dsUSER_MAIN _dsUser = new dsUSER_MAIN();

        //class lists
        List<User> _includedUsers = new List<User>();
        List<User> _excludedUsers = new List<User>();
        List<Simple> _disciplines = new List<Simple>();

        public frmTool_Discipline_Auth()
        {
            InitializeComponent();
            disciplineBindingSource.DataSource = _disciplines;
            userBindingAuth.DataSource = _includedUsers;
            userBindingUnauth.DataSource = _excludedUsers;
            RefreshDiscipline();
        }

        #region Form Population
        /// <summary>
        /// Refresh all discipline from enum
        /// </summary>
        private void RefreshDiscipline()
        {
            //Admin superuser gets all discipline
            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                List<string> disciplines = Enum.GetNames(typeof(Discipline)).ToList();
                foreach (string discipline in disciplines)
                {
                    _disciplines.Add(new Simple(discipline));
                }
            }

            using (AdapterUSER_MAIN daUser = new AdapterUSER_MAIN())
            {
                dsUSER_MAIN.USER_DISCDataTable dtUserDisc = daUser.GetAuthDisciplines();

                if (dtUserDisc != null)
                {
                    foreach (dsUSER_MAIN.USER_DISCRow drUserDisc in dtUserDisc.Rows)
                    {
                        string discipline = Common.ConvertDBDisciplineForDisplay(drUserDisc.DISCIPLINE);
                        if (!_disciplines.Any(x => x.simpleName == discipline))
                            _disciplines.Add(new Simple(discipline));
                    }
                }
            }

            gridDiscipline.RefreshDataSource();
        }

        /// <summary>
        /// Refresh all users from database
        /// </summary>
        private void RefreshUsers()
        {
            _includedUsers.Clear();
            _excludedUsers.Clear();

            Simple selectedDiscipline = GetSelectedDiscipline();
            if (selectedDiscipline == null)
                return;

            dsUSER_MAIN.USER_MAINDataTable dtUserDiscipline = _daUser.GetByDiscipline(selectedDiscipline.simpleName);
            dsUSER_MAIN.USER_MAINDataTable dtUserAll = _daUser.GetAuthUsers();
            if (dtUserDiscipline != null)
            {
                foreach (dsUSER_MAIN.USER_MAINRow drUser in dtUserDiscipline.Rows)
                {
                    //only add the user if user is part of the authorised list
                    if (dtUserAll != null && dtUserAll.Any(obj => obj.GUID == drUser.GUID))
                        AddUserToList(_includedUsers, drUser);
                }
            }

            if (dtUserAll != null)
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
                        Simple selectedDiscipline = GetSelectedDiscipline();
                        if (selectedDiscipline != null)
                        {
                            if (grid.Name == gridAuth.Name)
                            {
                                _daUser.SetUserDiscipline(findIncluded.GUID, selectedDiscipline.simpleName);
                            }
                            else
                            {
                                _daUser.RemoveUserDiscipline(findIncluded.GUID, selectedDiscipline.simpleName);
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
                userDiscipline = Common.Replace_WithSpaces(drUser.DDISCIPLINE),
                CreatedDate = drUser.CREATED
            }
            );
        }

        /// <summary>
        /// Get user selected discipine from gridview as class object
        /// </summary>
        private Simple GetSelectedDiscipline()
        {
            if (gridView1.SelectedRowsCount == 0)
                return null;

            return (Simple)gridView1.GetFocusedRow();
        }
        #endregion

        #region Event
        private void gridDiscipline_MouseClick(object sender, MouseEventArgs e)
        {
            RefreshUsers();
        }
        #endregion
    }
}
