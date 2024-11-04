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
using DevExpress.XtraTreeList.Nodes;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using System.Linq;


namespace CheckmateDX
{
    public partial class frmTool_Role : CheckmateDX.frmParent
    {
        //drag and drop
        GridHitInfo _downHitInfo = null;
        //database
        AdapterROLE_MAIN _daRole = new AdapterROLE_MAIN();
        dsROLE_MAIN _dsRole = new dsROLE_MAIN();

        //class lists
        List<Role> _allRole = new List<Role>();
        List<Privilege> _enabledPrivileges = new List<Privilege>();
        List<Privilege> _disabledPrivileges = new List<Privilege>();

        public frmTool_Role()
        {
            InitializeComponent();
            roleBindingSource.DataSource = _allRole;
            privilegeBindingSource.DataSource = _enabledPrivileges;
            privilegeBindingSource1.DataSource = _disabledPrivileges;
            
            RefreshRoles();
        }
        #region Form Population
        /// <summary>
        /// Refresh all role from database
        /// </summary>
        private void RefreshRoles()
        {
            _allRole.Clear();
            dsROLE_MAIN.ROLE_MAINDataTable dtRole = _daRole.GetAuthRole();
            if (dtRole != null)
            {
                foreach (dsROLE_MAIN.ROLE_MAINRow drRole in dtRole.Rows)
                {
                    _allRole.Add(new Role(drRole.GUID)
                    {
                        roleName = drRole.NAME,
                        roleParentGuid = drRole.PARENTGUID,
                        roleParentName = Common.ConvertRoleGuidToName(drRole.PARENTGUID)
                    });
                }
            }

            treeListRole.RefreshDataSource();
            treeListRole.ExpandAll();
        }

        private void RefreshPrivilege()
        {
            _enabledPrivileges.Clear();
            _disabledPrivileges.Clear();

            Role selectedRole = (Role)treeListRole.GetDataRecordByNode(treeListRole.FocusedNode);
            if (selectedRole == null)
                return;

            dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtPrivilege = _daRole.GetPrivilegeBy(selectedRole.GUID);
            if (dtPrivilege != null)
            {
                foreach (dsROLE_MAIN.ROLE_PRIVILEGERow drPrivilege in dtPrivilege.Rows)
                {
                    AddPrivilegeToList(_enabledPrivileges, drPrivilege);
                }
            }

            List<Privilege> getPrivilege;
            //admin superuser gets to assign all privilege in the system
            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                getPrivilege = System_Privilege.GetList();
            }
            else //other users only gets to assign privileges according to what's authorised for their role
            {
                dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtUserPrivileges = _daRole.GetPrivilegeBy((Guid)System_Environment.GetUser().userRole.Value);
                getPrivilege = new List<Privilege>();
                if(dtUserPrivileges != null)
                {
                    foreach (dsROLE_MAIN.ROLE_PRIVILEGERow drUserPrivilege in dtUserPrivileges.Rows)
                    {
                        if (!drUserPrivilege.LOCKED) //when this privilege is locked to that particular role it cannot be assigned to it's children
                        {
                            getPrivilege.Add(new Privilege(drUserPrivilege.GUID)
                            {
                                privTypeID = drUserPrivilege.TYPEID,
                                privName = drUserPrivilege.NAME,
                                privCategory = Common.GetPrivilegeCategory(drUserPrivilege.TYPEID),
                                privLocked = drUserPrivilege.LOCKED
                            });
                        }
                    }
                }
            }

            foreach (Privilege privilege in getPrivilege)
            {
                if (!_enabledPrivileges.Any(obj => obj.privTypeID == privilege.privTypeID))
                {
                    _disabledPrivileges.Add(privilege);
                }
            }

            gridEnabledPrivilege.RefreshDataSource();
            gridView1.ExpandAllGroups();
            gridDisabledPrivilege.RefreshDataSource();
            gridView3.ExpandAllGroups();
        } 
        #endregion

        #region Role Maintenance

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Role selectedRole = (Role)treeListRole.GetDataRecordByNode(treeListRole.FocusedNode);
            frmTool_Role_Add frmRoleAdd;
            if (selectedRole == null)
                frmRoleAdd = new frmTool_Role_Add(Guid.Empty);
            else
                frmRoleAdd = new frmTool_Role_Add(selectedRole.GUID);
           

            if (frmRoleAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Role newRole = frmRoleAdd.GetRole();
                dsROLE_MAIN.ROLE_MAINRow drRole = _dsRole.ROLE_MAIN.NewROLE_MAINRow();
                drRole.GUID = Guid.NewGuid();
                drRole.PARENTGUID = newRole.roleParentGuid;
                drRole.NAME = newRole.roleName;
                drRole.CREATED = DateTime.Now;
                drRole.CREATEDBY = System_Environment.GetUser().GUID;
                _dsRole.ROLE_MAIN.AddROLE_MAINRow(drRole);
                _daRole.Save(drRole);
                RefreshRoles();
                //Common.Prompt("Role successfully added");
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(treeListRole.GetDataRecordByNode(treeListRole.FocusedNode) == null)
            {
                Common.Warn("Please select role to edit");
                return;
            }

            Role selectedRole = (Role)treeListRole.GetDataRecordByNode(treeListRole.FocusedNode);
            frmTool_Role_Add frmRoleAdd = new frmTool_Role_Add(selectedRole);
            if (frmRoleAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Role editRole = frmRoleAdd.GetRole();

                dsROLE_MAIN.ROLE_MAINRow drRole = _daRole.GetBy(editRole.GUID);
                drRole.NAME = editRole.roleName;
                drRole.PARENTGUID = editRole.roleParentGuid;
                drRole.UPDATED = DateTime.Now;
                drRole.UPDATEDBY = System_Environment.GetUser().GUID;
                _daRole.Save(drRole);
                //Common.Prompt("Role successfully updated");
                RefreshRoles();
            }
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (treeListRole.GetDataRecordByNode(treeListRole.FocusedNode) == null)
            {
                Common.Warn("Please select role(s) to delete");
                return;
            }

            if (!Common.Confirmation("Are you sure you want to delete selected Role(s)?", "Confirmation"))
                return;

            dsROLE_MAIN.ROLE_MAINDataTable dtRole = new dsROLE_MAIN.ROLE_MAINDataTable(); //what Role to delete    
            foreach(TreeListNode node in treeListRole.Selection)
            {
                Role selectedRole = (Role)treeListRole.GetDataRecordByNode(node);
                //necessary values depends on what is displayed in the report
                dsROLE_MAIN.ROLE_MAINRow drRole = dtRole.NewROLE_MAINRow();
                drRole.GUID = selectedRole.GUID;
                drRole.NAME = selectedRole.roleName;
                drRole.PARENTGUID = selectedRole.roleParentGuid;
                drRole.CREATED = selectedRole.CreatedDate;
                drRole.CREATEDBY = selectedRole.CreatedBy;
                dtRole.AddROLE_MAINRow(drRole);
            }

            rptDeletion f = new rptDeletion(dtRole);
            f.ShowReport();

            RefreshRoles();
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
            List<Privilege> privileges = (List<Privilege>)binding.DataSource;
            DragData data = (DragData)e.Data.GetData(typeof(DragData));

            List<Privilege> privilegeFrom;
            List<Privilege> privilegeTo;
            if (grid.Name == gridDisabledPrivilege.Name)
            {
                privilegeFrom = _enabledPrivileges;
                privilegeTo = _disabledPrivileges;
            }
            else
            {
                privilegeFrom = _disabledPrivileges;
                privilegeTo = _enabledPrivileges;
            }

            if (data != null && binding != null)
            {
                for (int i = 0; i < data.dataRowIndexes.GetLength(0); i++)
                {
                    Privilege movingPrivilege = privilegeFrom[data.dataRowIndexes[i]];
                    movingPrivilege.privLocked = false; //reset the locked status when we're moving it
                    privilegeTo.Add(movingPrivilege);
                }

                foreach (Privilege privilege in privilegeTo)
                {
                    Privilege findEnabled = privilegeFrom.FirstOrDefault(obj => obj.privName == privilege.privName);
                    if (findEnabled != null)
                    {
                        privilegeFrom.Remove(findEnabled);

                        //database operation
                        Role selectedRole = (Role)treeListRole.GetDataRecordByNode(treeListRole.FocusedNode);
                        if (selectedRole != null)
                        {
                            if (grid.Name == gridEnabledPrivilege.Name)
                            {
                                _daRole.SetRolePrivilege(selectedRole.GUID, privilege);
                            }
                            else
                            {
                                _daRole.RemoveRolePrivilege(selectedRole.GUID, privilege.privTypeID);
                            }

                            _daRole.RoleUpdate(selectedRole.GUID);
                        }
                    }
                }
            }

            gridEnabledPrivilege.RefreshDataSource();
            gridDisabledPrivilege.RefreshDataSource();
            gridView1.ExpandAllGroups();
            gridView3.ExpandAllGroups();
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
        /// Adds datarow privilege into list
        /// </summary>
        private void AddPrivilegeToList(List<Privilege> privilegeList, dsROLE_MAIN.ROLE_PRIVILEGERow drPrivilege)
        {
            privilegeList.Add(new Privilege(drPrivilege.GUID)
            {
                //data is posted for information only, what is shown on the list determine which is necessary
                privName = drPrivilege.NAME,
                privTypeID = drPrivilege.TYPEID,
                privCategory = Common.GetPrivilegeCategory(drPrivilege.TYPEID),
                privLocked = drPrivilege.LOCKED
            }
            );
        }
        #endregion

        #region Events
        private void treeListRoles_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            RefreshPrivilege();
        }

        /// <summary>
        /// Handles Checkbox Value Change
        /// </summary>
        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Name == colprivLocked.Name)
            {
                Role selectedRole = (Role)treeListRole.GetDataRecordByNode(treeListRole.FocusedNode);
                if (selectedRole == null)
                    return;

                Privilege selectedPrivilege = _enabledPrivileges[e.RowHandle];
                dsROLE_MAIN.ROLE_PRIVILEGERow drRole = _daRole.GetPrivilegeBy(selectedRole.GUID, selectedPrivilege);
                if(drRole != null)
                {
                    drRole.LOCKED = bool.Parse(e.Value.ToString());
                    _daRole.Save(drRole);
                }
            }
        }
        #endregion

        /// <summary>
        /// Dispose local adapters upon closing
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            _daRole.Dispose();
            base.OnClosed(e);
        }
    }
}
