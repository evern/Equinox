using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectCommon;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;

namespace CheckmateDX
{
    public partial class frmSync_Manage_Superseded : CheckmateDX.frmParent
    {
        AdapterSYNC_PAIR _daSyncPair = new AdapterSYNC_PAIR();
        AdapterSYNC_TABLE _daSyncTable = new AdapterSYNC_TABLE();
        List<Client> _SyncClient = new List<Client>();

        public frmSync_Manage_Superseded()
        {
            InitializeComponent();
            clientBindingSource.DataSource = _SyncClient;
            Refresh_Client();
        }

        #region Initialisation
        /// <summary>
        /// Loads all the available client from database to datagrid
        /// </summary>
        public void Refresh_Client()
        {
            _SyncClient.Clear();
            dsSYNC_PAIR.SYNC_PAIRDataTable dtSyncPair = _daSyncPair.GetAll_ExcludeDeleted();
            if(dtSyncPair != null)
            {
                foreach(dsSYNC_PAIR.SYNC_PAIRRow drSyncPair in dtSyncPair.Rows)
                {
                    _SyncClient.Add(new Client(drSyncPair.GUID)
                        {
                            HWID = drSyncPair.HWID,
                            IPAddress = drSyncPair.IP_ADDRESS,
                            Approved = drSyncPair.APPROVED,
                            Description = drSyncPair.DESCRIPTION,
                            CreatedBy = drSyncPair.CREATEDBY,
                            CreatedDate = drSyncPair.CREATED,
                            CreatedName = Common.ConvertUserGuidToName(drSyncPair.CREATEDBY)
                        });
                }
            }

            gridControlClient.RefreshDataSource();
        }
        #endregion
        /// <summary>
        /// Determines what action to take when user clicks on the datagridview
        /// </summary>
        private void gridViewClient_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            GridView view = (GridView)sender;
            if (CanToggle(view, e.Location))
            {
                Client selectedClient = (Client)gridViewClient.GetFocusedRow();
                if (selectedClient.Approved)
                {
                    LoadToggleState(selectedClient.GUID);
                    panelControlToggle.Enabled = true;
                }
            }
            else
                panelControlToggle.Enabled = false;

            if (!CanCheck(view, e.Location)) return;
            DoCheck(view);
            ((DXMouseEventArgs)e).Handled = true;
        }

        /// <summary>
        /// Checks whether the user is selecting a row in the datagridview
        /// </summary>
        private bool CanToggle(GridView view, Point location)
        {
            GridHitInfo hitInfo = view.CalcHitInfo(location);
            //return view.SelectedRowsCount == 1 && hitInfo.InRowCell && view.IsRowSelected(hitInfo.RowHandle);
            return view.SelectedRowsCount == 1 && hitInfo.InRowCell;
        }

        /// <summary>
        /// Checks whether the user is selecting the checkbox in a particular datagridviewrow
        /// </summary>
        private bool CanCheck(GridView view, Point location)
        {
            GridHitInfo hitInfo = view.CalcHitInfo(location);
            return view.SelectedRowsCount == 1 && hitInfo.InRowCell && hitInfo.Column == colApproved &&
                view.IsRowSelected(hitInfo.RowHandle);
        }

        /// <summary>
        /// Toggle the enabled value on selected row
        /// </summary>
        private void DoCheck(GridView view)
        {
            bool value = false;
            for (int i = 0; i < view.RowCount; i++)
            if (view.IsRowSelected(i))
            {
                if (!value) 
                {
                    value = !(bool)view.GetRowCellValue(i, colApproved); //gets the reversed value for toggle
                    if(!value)
                    {
                        if (!Common.Confirmation("Are you sure you want to disable all sync activities for this machine?", "Disabling Sync"))
                            return;
                    }
                    view.SetRowCellValue(i, colApproved, value);
                    Client selectedClient = (Client)gridViewClient.GetRow(i);
                    ToggleClient(selectedClient.GUID, value);
                    panelControlToggle.Enabled = value;
                }
            }
        }

        /// <summary>
        /// Update the toggled value into database
        /// </summary>
        private void ToggleClient(Guid clientGuid, bool value)
        {
            dsSYNC_PAIR.SYNC_PAIRRow drClient = _daSyncPair.GetBy(clientGuid);
            if(drClient != null)
            {
                drClient.UPDATED = DateTime.Now;
                drClient.UPDATEDBY = System_Environment.GetUser().GUID;
                drClient.APPROVED = value;
                _daSyncPair.Save(drClient);
            }
        }

        /// <summary>
        /// Loads the toggle state for selected client on the datagridview
        /// </summary>
        private void LoadToggleState(Guid clientGuid)
        {
            switchITR.Toggled -= switchITR_Toggled;
            switchPunchlist.Toggled -= switchPunchlist_Toggled;
            switchUser.Toggled -= switchUser_Toggled;
            switchTemplate.Toggled -= switchTemplate_Toggled;
            switchProject.Toggled -= switchProject_Toggled;

            switchITR.EditValue = false;
            switchPunchlist.EditValue = false;
            switchTemplate.EditValue = false;
            switchProject.EditValue = false;
            switchUser.EditValue = false;
            switchPunchlist.EditValue = false;

            dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNC_TABLE = _daSyncTable.GetBy(clientGuid);
            if (dtSYNC_TABLE != null)
            {
                foreach(dsSYNC_TABLE.SYNC_TABLERow drSYNC_TABLE in dtSYNC_TABLE)
                {
                    if(drSYNC_TABLE.TYPE == Sync_Type_Superseded.ITR.ToString())
                    {
                        if (drSYNC_TABLE.SYNC_MODE == Sync_Mode.Both.ToString())
                            switchITR.EditValue = true;

                        checkITRSync.Checked = drSYNC_TABLE.ONETIME;
                    }

                    if(drSYNC_TABLE.TYPE == Sync_Type_Superseded.Punchlist.ToString())
                    {
                        if (drSYNC_TABLE.SYNC_MODE == Sync_Mode.Both.ToString())
                            switchPunchlist.EditValue = true;

                        checkPunchlistSync.Checked = drSYNC_TABLE.ONETIME;
                    }

                    if (drSYNC_TABLE.TYPE == Sync_Type_Superseded.User.ToString())
                    {
                        if (drSYNC_TABLE.SYNC_MODE != Sync_Mode.None.ToString())
                        {
                            switchUser.EditValue = true;
                            radioGroupUser.EditValue = drSYNC_TABLE.SYNC_MODE;
                        }

                        checkUserSync.Checked = drSYNC_TABLE.ONETIME;
                        checkUserSyncDelete.Checked = drSYNC_TABLE.DELETES;
                    }

                    if (drSYNC_TABLE.TYPE == Sync_Type_Superseded.Template.ToString())
                    {
                        if (drSYNC_TABLE.SYNC_MODE != Sync_Mode.None.ToString())
                        {
                            switchTemplate.EditValue = true;
                            radioGroupTemplate.EditValue = drSYNC_TABLE.SYNC_MODE;
                        }

                        checkTemplateSync.Checked = drSYNC_TABLE.ONETIME;
                        checkTemplateSyncDelete.Checked = drSYNC_TABLE.DELETES;
                    }

                    if (drSYNC_TABLE.TYPE == Sync_Type_Superseded.Project.ToString())
                    {
                        if (drSYNC_TABLE.SYNC_MODE != Sync_Mode.None.ToString())
                        {
                            switchProject.EditValue = true;
                            radioGroupProject.EditValue = drSYNC_TABLE.SYNC_MODE;
                        }

                        checkProjectSync.Checked = drSYNC_TABLE.ONETIME;
                        checkProjectSyncDelete.Checked = drSYNC_TABLE.DELETES;
                    }
                }
            }

            switchITR.Toggled += switchITR_Toggled;
            switchPunchlist.Toggled += switchPunchlist_Toggled;
            switchUser.Toggled += switchUser_Toggled;
            switchTemplate.Toggled += switchTemplate_Toggled;
            switchProject.Toggled += switchProject_Toggled;
        }

        /// <summary>
        /// Update the toggle state of ITR in the database
        /// </summary>
        private void switchITR_Toggled(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            checkITRSync.Enabled = (bool)switchITR.EditValue;

            if((bool)switchITR.EditValue)
                Update_Toggle(selectedClient.GUID, Sync_Type_Superseded.ITR, Sync_Mode.Both);
            else
                Update_Toggle(selectedClient.GUID, Sync_Type_Superseded.ITR, Sync_Mode.None);
        }

        /// <summary>
        /// Update the toggle state of punchlist sync in the database
        /// </summary>
        private void switchPunchlist_Toggled(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            checkPunchlistSync.Enabled = (bool)switchPunchlist.EditValue;

            if ((bool)switchPunchlist.EditValue)
                Update_Toggle(selectedClient.GUID, Sync_Type_Superseded.Punchlist, Sync_Mode.Both);
            else
                Update_Toggle(selectedClient.GUID, Sync_Type_Superseded.Punchlist, Sync_Mode.None);
        }

        /// <summary>
        /// Update the toggle state of user sync in the database
        /// </summary>
        private void switchUser_Toggled(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            checkUserSync.Enabled = (bool)switchUser.EditValue;
            radioGroupUser.Enabled = (bool)switchUser.EditValue;
            checkUserSyncDelete.Enabled = (bool)switchUser.EditValue;

            if ((bool)switchUser.EditValue)
                Update_Toggle(selectedClient.GUID, Sync_Type_Superseded.User, (Sync_Mode)Enum.Parse(typeof(Sync_Mode), radioGroupUser.EditValue.ToString()));
            else
                Update_Toggle(selectedClient.GUID, Sync_Type_Superseded.User, Sync_Mode.None);
        }

        /// <summary>
        /// Update the toggle state of template sync in database
        /// </summary>
        private void switchTemplate_Toggled(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            checkTemplateSync.Enabled = (bool)switchTemplate.EditValue;
            radioGroupTemplate.Enabled = (bool)switchTemplate.EditValue;
            checkTemplateSyncDelete.Enabled = (bool)switchTemplate.EditValue;

            if ((bool)switchTemplate.EditValue)
                Update_Toggle(selectedClient.GUID, Sync_Type_Superseded.Template, (Sync_Mode)Enum.Parse(typeof(Sync_Mode), radioGroupTemplate.EditValue.ToString()));
            else
                Update_Toggle(selectedClient.GUID, Sync_Type_Superseded.Template, Sync_Mode.None);
        }

        /// <summary>
        /// Update the toggle state of project sync in database
        /// </summary>
        private void switchProject_Toggled(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            checkProjectSync.Enabled = (bool)switchProject.EditValue;
            radioGroupProject.Enabled = (bool)switchProject.EditValue;
            checkProjectSyncDelete.Enabled = (bool)switchProject.EditValue;

            if ((bool)switchProject.EditValue)
                Update_Toggle(selectedClient.GUID, Sync_Type_Superseded.Project, (Sync_Mode)Enum.Parse(typeof(Sync_Mode), radioGroupProject.EditValue.ToString()));
            else
                Update_Toggle(selectedClient.GUID, Sync_Type_Superseded.Project, Sync_Mode.None);
        }

        /// <summary>
        /// Helper to write the type and mode to Sync_Table
        /// </summary>
        private void Update_Toggle(Guid clientGuid, Sync_Type_Superseded type, Sync_Mode mode)
        {
            dsSYNC_TABLE.SYNC_TABLERow drSyncTable = _daSyncTable.GetByType(clientGuid, type);
            if (drSyncTable != null)
            {
                drSyncTable.SYNC_MODE = mode.ToString();
                drSyncTable.UPDATED = DateTime.Now;
                drSyncTable.UPDATEDBY = System_Environment.GetUser().GUID;
                _daSyncTable.Save(drSyncTable);
            }
            else
            {
                dsSYNC_TABLE dsSYNCTABLE = new dsSYNC_TABLE();
                dsSYNC_TABLE.SYNC_TABLERow drNewSyncTable = dsSYNCTABLE.SYNC_TABLE.NewSYNC_TABLERow();

                drNewSyncTable.GUID = Guid.NewGuid();
                drNewSyncTable.SYNC_PAIR_GUID = clientGuid;
                drNewSyncTable.TYPE = type.ToString();
                drNewSyncTable.SYNC_MODE = mode.ToString();
                drNewSyncTable.ONETIME = false;
                drNewSyncTable.DELETES = false;
                drNewSyncTable.CREATED = DateTime.Now;
                drNewSyncTable.CREATEDBY = System_Environment.GetUser().GUID;

                dsSYNCTABLE.SYNC_TABLE.AddSYNC_TABLERow(drNewSyncTable);
                _daSyncTable.Save(drNewSyncTable);
            }
        }

        private void Update_Check(Guid clientGuid, Sync_Type_Superseded type, bool checkState, bool deletes)
        {
            dsSYNC_TABLE.SYNC_TABLERow drSyncTable = _daSyncTable.GetByType(clientGuid, type);
            if(drSyncTable != null)
            {
                if (deletes)
                    drSyncTable.DELETES = checkState;
                else
                    drSyncTable.ONETIME = checkState;

                drSyncTable.UPDATED = DateTime.Now;
                drSyncTable.UPDATEDBY = System_Environment.GetUser().GUID;
                _daSyncTable.Save(drSyncTable);
            }
        }

        private void checkITRSync_CheckedChanged(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            Update_Check(selectedClient.GUID, Sync_Type_Superseded.ITR, checkITRSync.Checked, false);
        }

        private void checkPunchlistSync_CheckedChanged(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            Update_Check(selectedClient.GUID, Sync_Type_Superseded.Punchlist, checkPunchlistSync.Checked, false);
        }

        private void checkUserSync_CheckedChanged(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            Update_Check(selectedClient.GUID, Sync_Type_Superseded.User, checkUserSync.Checked, false);
        }

        private void checkTemplateSync_CheckedChanged(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            Update_Check(selectedClient.GUID, Sync_Type_Superseded.Template, checkTemplateSync.Checked, false);
        }

        private void checkProjectSync_CheckedChanged(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            Update_Check(selectedClient.GUID, Sync_Type_Superseded.Project, checkProjectSync.Checked, false);
        }

        private void checkUserSyncDelete_CheckedChanged(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            Update_Check(selectedClient.GUID, Sync_Type_Superseded.User, checkUserSyncDelete.Checked, true);
        }

        private void checkTemplateSyncDelete_CheckedChanged(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            Update_Check(selectedClient.GUID, Sync_Type_Superseded.Template, checkTemplateSyncDelete.Checked, true);
        }

        private void checkProjectSyncDelete_CheckedChanged(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            Update_Check(selectedClient.GUID, Sync_Type_Superseded.Project, checkProjectSyncDelete.Checked, true);
        }

        private void switchITR_EnabledChanged(object sender, EventArgs e)
        {
            checkITRSync.Enabled = (switchITR.Enabled && (bool)switchITR.EditValue);
        }

        private void switchPunchlist_EnabledChanged(object sender, EventArgs e)
        {
            checkPunchlistSync.Enabled = (switchPunchlist.Enabled && (bool)switchPunchlist.EditValue);
        }

        private void switchUser_EnabledChanged(object sender, EventArgs e)
        {
            checkUserSync.Enabled = (switchUser.Enabled && (bool)switchUser.EditValue);
            checkUserSyncDelete.Enabled = (switchUser.Enabled && (bool)switchUser.EditValue);
            radioGroupUser.Enabled = (switchUser.Enabled && (bool)switchUser.EditValue);
        }

        private void switchTemplate_EnabledChanged(object sender, EventArgs e)
        {
            checkTemplateSync.Enabled = (switchTemplate.Enabled && (bool)switchTemplate.EditValue);
            checkTemplateSyncDelete.Enabled = (switchTemplate.Enabled && (bool)switchTemplate.EditValue);
            radioGroupTemplate.Enabled = (switchTemplate.Enabled && (bool)switchTemplate.EditValue);
        }

        private void switchProject_EnabledChanged(object sender, EventArgs e)
        {
            checkProjectSync.Enabled = (switchProject.Enabled && (bool)switchProject.EditValue);
            checkProjectSyncDelete.Enabled = (switchProject.Enabled && (bool)switchProject.EditValue);
            radioGroupProject.Enabled = (switchProject.Enabled && (bool)switchProject.EditValue);
        }

        private void radioGroupUser_EditValueChanged(object sender, EventArgs e)
        {
            switchUser_Toggled(null, null);
        }

        private void radioGroupTemplate_EditValueChanged(object sender, EventArgs e)
        {
            switchTemplate_Toggled(null, null);
        }

        private void radioGroupProject_EditValueChanged(object sender, EventArgs e)
        {
            switchProject_Toggled(null, null);
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            Client selectedClient = (Client)gridViewClient.GetFocusedRow();
            frmSync_Status_Superseded f = new frmSync_Status_Superseded(selectedClient.HWID, selectedClient.Description);
            f.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridViewClient.SelectedRowsCount == 0)
            {
                Common.Prompt("Please select a row to delete");
                return;
            }

            if (!Common.Confirmation("Are you sure you want to delete selected client?", "Confirmation"))
                return;

            _daSyncPair.RemoveBy(((Client)gridViewClient.GetFocusedRow()).GUID);
            Refresh_Client();
        }
    }
}
