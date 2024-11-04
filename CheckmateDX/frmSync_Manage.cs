using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ProjectCommon;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectLibrary;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using System.Data.SqlClient;

namespace CheckmateDX
{
    public partial class frmSync_Manage : frmParent
    {
        private List<SyncOption> _SyncOptions = new List<SyncOption>();
        private List<Client> _Clients = new List<Client>();
        private AdapterSYNC_PAIR _daSYNCPAIR = new AdapterSYNC_PAIR(Variables.ConnStr);
        private AdapterSYNC_TABLE _daSYNCTABLE = new AdapterSYNC_TABLE(Variables.ConnStr);

        public frmSync_Manage()
        {
            InitializeComponent();
            gridControlClient.DataSource = _Clients;
            treeListOptions.DataSource = _SyncOptions;
            Refresh_Clients();
        }

        #region List Population
        /// <summary>
        /// Refreshes the Paired Clients
        /// </summary>
        private void Refresh_Clients()
        {
            _Clients.Clear();

            dsSYNC_PAIR.SYNC_PAIRDataTable dtSYNCPAIR = _daSYNCPAIR.GetAll_ExcludeDeleted();
            if(dtSYNCPAIR != null)
            {
                foreach(dsSYNC_PAIR.SYNC_PAIRRow drSYNCPAIR in dtSYNCPAIR.Rows)
                {
                    _Clients.Add(new Client(drSYNCPAIR.GUID)
                    {
                        HWID = drSYNCPAIR.HWID,
                        IPAddress = drSYNCPAIR.IP_ADDRESS,
                        Approved = drSYNCPAIR.APPROVED,
                        Description = drSYNCPAIR.DESCRIPTION,
                        CreatedBy = drSYNCPAIR.CREATEDBY,
                        CreatedDate = drSYNCPAIR.CREATED,
                        CreatedName = Common.ConvertUserGuidToName(drSYNCPAIR.CREATEDBY)
                    });
                }
            }

            gridViewClient.RefreshData();
            gridViewClient.CancelSelection();
        }

        /// <summary>
        /// Refreshes the Client Sync Options Depending on GridViewClient Selections
        /// </summary>
        private void Refresh_Client_Options()
        {
            _SyncOptions.Clear();

            if(gridViewClient.GetFocusedRow() != null)
            {
                Client SelectedClient = (Client)gridViewClient.GetFocusedRow();

                Common.Populate_Database_Options(_SyncOptions);
                if (SelectedClient.Approved)
                {
                    treeListOptions.Enabled = true;
                    using (AdapterSYNC_TABLE daSYNCTABLE = new AdapterSYNC_TABLE(Variables.ConnStr))
                    {
                        dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNCTABLE = daSYNCTABLE.GetBy(SelectedClient.GUID);
                        if (dtSYNCTABLE != null)
                        {
                            Update_SyncOptions_CheckState(_SyncOptions, dtSYNCTABLE);
                        }
                    }
                }
                else
                    treeListOptions.Enabled = false;
            }

            SyncOptions_RefreshDataSource();
        }

        /// <summary>
        /// Update the Sync Option Item Enabled and OneTime Statuses
        /// </summary>
        /// <returns>Whether any Changes has been made</returns>
        private bool Update_SyncOptions_CheckState(List<SyncOption> Sync_Options, dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNCTABLE)
        {
            bool ChangesMade = false;
            foreach(SyncOption Sync_Option in Sync_Options)
            {
                dsSYNC_TABLE.SYNC_TABLERow drSYNCTABLE = dtSYNCTABLE.FirstOrDefault(obj => obj.TYPE == Common.ReplaceSpacesWith_(Sync_Option.OptionName));
                if (drSYNCTABLE != null)
                {
                    ChangesMade = true;
                    Sync_Option.OptionOneTime = drSYNCTABLE.ONETIME;
                    Sync_Option.OptionEnabled = (drSYNCTABLE.SYNC_MODE == Sync_Mode.Both.ToString());
                }
                //else it was initialized as false enabled and onetime by default
            }

            return ChangesMade;
        }
        #endregion

        #region Events

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridViewClient.SelectedRowsCount == 0)
            {
                Common.Prompt("Please select a row to delete");
                return;
            }

            if (!Common.Confirmation("Are you sure you want to delete selected client?", "Confirmation"))
                return;

            _daSYNCPAIR.RemoveBy(((Client)gridViewClient.GetFocusedRow()).GUID);
            Refresh_Clients();
        }

        /// <summary>
        /// OneTime CheckValue Handling
        /// </summary>
        private void repositoryItemCheckEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            SyncOption SelectedSyncOption = (SyncOption)treeListOptions.GetDataRecordByNode(treeListOptions.FocusedNode);
            if (SelectedSyncOption != null)
            {
                if (!SelectedSyncOption.OptionEnabled)
                {
                    Common.Warn("Cannot enable this option because item is disabled");
                    e.NewValue = e.OldValue; //nullify changes because enabled is false
                }
            }
            else
                e.NewValue = e.OldValue; //nullify changes
        }

        /// <summary>
        /// Binding Node CheckState with OptionEnabled
        /// </summary>
        private void treeListOptions_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            //because current checked member is a parent, update recursively
            if (e.Node.HasChildren)
            {
                foreach (TreeListNode childNode in e.Node.Nodes)
                {
                    //SetValue won't invoke treeListOptions_CellValueChanged
                    childNode.SetValue(colOptionEnabled, e.Node.Checked); //node update will be reflected in the binded list
                }
            }
            //because current checked member is a child, update directly
            else
                //SetValue won't invoke treeListOptions_CellValueChanged
                e.Node.SetValue(colOptionEnabled, e.Node.Checked); //node update will be reflected in the binded list

            SyncOptions_UpdateOneTime();
            Save_Client_SyncOptions();
        }

        /// <summary>
        /// Save the entire sync options for the selected client
        /// </summary>
        private void Save_Client_SyncOptions()
        {
            Client SelectedClient = (Client)gridViewClient.GetFocusedRow();
            if (SelectedClient == null)
                return;

            //save the sync options into database
            dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNCTABLE = _daSYNCTABLE.GetBy(SelectedClient.GUID);
            if (dtSYNCTABLE == null)
                dtSYNCTABLE = new dsSYNC_TABLE.SYNC_TABLEDataTable();

            foreach (SyncOption Option in _SyncOptions.Where(obj => obj.OptionParentID != 0))
            {
                dsSYNC_TABLE.SYNC_TABLERow drSYNCITEM = dtSYNCTABLE.FirstOrDefault(obj => obj.TYPE == Common.ReplaceSpacesWith_(Option.OptionName));
                if (drSYNCITEM != null)
                {
                    if (Option.OptionEnabled)
                        drSYNCITEM.SYNC_MODE = Sync_Mode.Both.ToString();
                    else
                        drSYNCITEM.SYNC_MODE = Sync_Mode.None.ToString();

                    drSYNCITEM.ONETIME = Option.OptionOneTime;
                    drSYNCITEM.UPDATED = DateTime.Now;
                    drSYNCITEM.UPDATEDBY = System_Environment.GetUser().GUID;
                }
                else if (Option.OptionEnabled)
                {
                    dsSYNC_TABLE.SYNC_TABLERow drNEWSYNCITEM = dtSYNCTABLE.NewSYNC_TABLERow();
                    drNEWSYNCITEM.GUID = Guid.NewGuid();
                    drNEWSYNCITEM.SYNC_PAIR_GUID = SelectedClient.GUID;
                    drNEWSYNCITEM.TYPE = Common.ReplaceSpacesWith_(Option.OptionName);
                    drNEWSYNCITEM.SYNC_MODE = Sync_Mode.Both.ToString();
                    drNEWSYNCITEM.ONETIME = Option.OptionOneTime;
                    drNEWSYNCITEM.DELETES = true;
                    drNEWSYNCITEM.CREATED = DateTime.Now;
                    drNEWSYNCITEM.CREATEDBY = System_Environment.GetUser().GUID;
                    dtSYNCTABLE.AddSYNC_TABLERow(drNEWSYNCITEM);
                }
            }

            _daSYNCTABLE.Save(dtSYNCTABLE);
        }

        private void gridViewClient_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            GridView view = (GridView)sender;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
            if (!hitInfo.InRow)
            {
                _SyncOptions.Clear();
                treeListOptions.RefreshDataSource();
            }
            else
            {
                if(hitInfo.Column == colApproved)
                {
                    Client SelectedClient = (Client)gridViewClient.GetFocusedRow();
                    view.SetRowCellValue(view.FocusedRowHandle, colApproved, !SelectedClient.Approved);
                }

                Refresh_Client_Options();
            }
        }

        private void gridViewClient_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if(gridViewClient.GetFocusedRow() != null)
            {
                Client SelectedClient = (Client)gridViewClient.GetFocusedRow();
                dsSYNC_PAIR.SYNC_PAIRRow drSYNCPAIR = _daSYNCPAIR.GetBy(SelectedClient.GUID);
                if(drSYNCPAIR != null)
                {
                    drSYNCPAIR.APPROVED = SelectedClient.Approved;
                    drSYNCPAIR.DESCRIPTION = SelectedClient.Description;
                    _daSYNCPAIR.Save(drSYNCPAIR);

                    treeListOptions.Enabled = drSYNCPAIR.APPROVED;
                }
            }
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            if (gridViewClient.GetFocusedRow() != null)
            {
                Client SelectedClient = (Client)gridViewClient.GetFocusedRow();
                using (frmSync_Status f = new frmSync_Status(SelectedClient.HWID, SelectedClient.Description))
                {
                    f.ShowDialog();
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _daSYNCPAIR.Dispose();
            _daSYNCTABLE.Dispose();
            base.OnFormClosing(e);
        }
        #endregion

        #region View Helpers
        //Custom Datasource Refresh to Link Checked with SyncOption.OptionEnabled
        private void SyncOptions_RefreshDataSource()
        {
            treeListOptions.RefreshDataSource();

            bool AllChecked = true;
            bool NoneChecked = true;
            foreach(TreeListNode node in treeListOptions.Nodes)
            {
                foreach(TreeListNode childNode in node.Nodes) //only recurse childnode
                {
                    SyncOption GetSyncOption = (SyncOption)treeListOptions.GetDataRecordByNode(childNode);
                    if (GetSyncOption.OptionEnabled)
                    {
                        childNode.Checked = true;
                        NoneChecked = false;
                    }
                    else
                    {
                        childNode.Checked = false;
                        AllChecked = false;
                    }
                }

                if (NoneChecked)
                    node.CheckState = CheckState.Unchecked;
                else if (AllChecked)
                    node.CheckState = CheckState.Checked;
                else
                    node.CheckState = CheckState.Indeterminate;

                AllChecked = true;
                NoneChecked = true;
            }

            treeListOptions.Refresh();
            treeListOptions.ExpandAll();
        }

        /// <summary>
        /// Bind OneTime checked to node checked with
        /// </summary>
        private void SyncOptions_UpdateOneTime()
        {
            foreach (TreeListNode node in treeListOptions.Nodes) //handling categories
            {
                if (!node.Checked && (bool)node.GetValue(colOptionOneTime))
                {
                    node.SetValue(colOptionOneTime, false);
                }

                foreach (TreeListNode childNodes in node.Nodes)
                {
                    if (!childNodes.Checked && (bool)childNodes.GetValue(colOptionOneTime))
                    {
                        childNodes.SetValue(colOptionOneTime, false);
                    }
                }
            }
        }
        #endregion
    }
}