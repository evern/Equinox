using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProjectCommon;
using ProjectLibrary;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using DevExpress.XtraTreeList.Nodes;
using System.Data.SqlClient;

namespace CheckmateDX
{
    public partial class frmSync_Cleanup : frmParent
    {
        private List<SyncOption> _SyncOptions = new List<SyncOption>();

        public frmSync_Cleanup()
        {
            InitializeComponent();
            Populate_Form_Elements();
        }

        #region Form Population
        Guid _projectGuid = Guid.Empty;
        string _discipline = string.Empty;
        private void Populate_Form_Elements()
        {
            Common.Populate_Database_Options(_SyncOptions, true);
            bindingSource1.DataSource = _SyncOptions;
            treeListDatabase.ExpandAll();

            //need to use timer so that splashscreen is already hidden
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            assignEnvironmentParameter();
        }
        /// <summary>
        /// Used when user is superadmin, which doesn't have default project and discipline
        /// </summary>
        private void assignEnvironmentParameter()
        {
            //get user parameter
            _projectGuid = (Guid)System_Environment.GetUser().userProject.Value;
            _discipline = System_Environment.GetUser().userDiscipline;

            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                frmTool_Project frmSelectProject = new frmTool_Project();
                frmSelectProject.ShowAsDialog();
                if (frmSelectProject.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _projectGuid = frmSelectProject.GetSelectedProject().GUID;
                }
                else
                {
                    this.Close();
                    return;
                }


                frmITR_Select_Discipline frmSelectDiscipline = new frmITR_Select_Discipline();
                if (frmSelectDiscipline.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _discipline = frmSelectDiscipline.GetDiscipline();
                }
                else
                {
                    this.Close();
                    return;
                }
            }
        }
        #endregion

        #region Events
        private void btnPurge_Click(object sender, EventArgs e)
        {
            using(AdapterTOMBSTONE daTOMBSTONE = new AdapterTOMBSTONE())
            {
                dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE = daTOMBSTONE.Get();
                if (dtTOMBSTONE.Rows.Count == 0)
                {
                    Common.Prompt("Nothing to purge, please run trim first");
                    return;
                }
            }

            if (!Common.WarningConfirmation("This operation is system wide\nIf tombstone is deleted before any outstanding device is synced\nDeleted/unwanted record will get inserted back into the database\nAre you sure you want to continue?", "Warning"))
                return;

            using(AdapterTOMBSTONE daTOMBSTONE = new AdapterTOMBSTONE())
            {
                int ClearedRows = daTOMBSTONE.Clear();
                Common.Prompt(ClearedRows.ToString() + " records cleared");
            }
        }

        private void btnTrim_Click(object sender, EventArgs e)
        {
            if (!Common.WarningConfirmation("This process is not reversible, are you sure you want to proceed?", "Warning"))
                return;

            splashScreenManager1.ShowWaitForm();
            List<SyncOption> EnabledSyncOption = _SyncOptions.Where(obj => obj.OptionEnabled == true).ToList();
            int trimCount = 0;

            if(EnabledSyncOption.Count > 0)
            {
                using (AdapterTOMBSTONE daTOMBSTONE = new AdapterTOMBSTONE())
                {
                    dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE = new dsTOMBSTONE.TOMBSTONEDataTable();
                    #region Mundane Database Purging Routine
                    //Scope Project
                    if (EnabledSyncOption.Any(obj => obj.OptionName == Sync_Item.Equipment.ToString()))
                    {
                        using (AdapterGENERAL_EQUIPMENT daEQUIPMENT = new AdapterGENERAL_EQUIPMENT())
                        {
                            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtEQUIPMENT = daEQUIPMENT.Get_Deleted(_projectGuid);
                            if (dtEQUIPMENT != null)
                            {
                                foreach (dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drEQUIPMENT in dtEQUIPMENT.Rows)
                                {
                                    AddTombstone(drEQUIPMENT.GUID, Sync_Type.GENERAL_EQUIPMENT, dtTOMBSTONE);
                                    drEQUIPMENT.Delete();
                                }

                                daEQUIPMENT.Save(dtEQUIPMENT);
                            }
                        }
                    }

                    //Scope: Discipline
                    if (EnabledSyncOption.Any(obj => obj.OptionName == Sync_Item.Template_Components.ToString()))
                    {
                        using (AdapterPREFILL_MAIN daPREFILL = new AdapterPREFILL_MAIN())
                        {
                            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL = daPREFILL.Get_Deleted(_discipline);
                            if (dtPREFILL != null)
                            {
                                foreach (dsPREFILL_MAIN.PREFILL_MAINRow drPREFILL in dtPREFILL.Rows)
                                {
                                    AddTombstone(drPREFILL.GUID, Sync_Type.PREFILL_MAIN, dtTOMBSTONE);
                                    drPREFILL.Delete();
                                }

                                daPREFILL.Save(dtPREFILL);
                            }
                        }

                        using (AdapterTEMPLATE_TOGGLE daTEMPLATE_TOGGLE = new AdapterTEMPLATE_TOGGLE())
                        {
                            dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE = daTEMPLATE_TOGGLE.Get_Deleted(_discipline);
                            if (dtTEMPLATE_TOGGLE != null)
                            {
                                foreach (dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow drTEMPLATE_TOGGLE in dtTEMPLATE_TOGGLE.Rows)
                                {
                                    AddTombstone(drTEMPLATE_TOGGLE.GUID, Sync_Type.TEMPLATE_TOGGLE, dtTOMBSTONE);
                                    drTEMPLATE_TOGGLE.Delete();
                                }

                                daTEMPLATE_TOGGLE.Save(dtTEMPLATE_TOGGLE);
                            }
                        }

                        using (AdapterMATRIX_TYPE daMATRIX_TYPE = new AdapterMATRIX_TYPE())
                        {
                            dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMATRIX_TYPE = daMATRIX_TYPE.Get_Deleted(_discipline);
                            if (dtMATRIX_TYPE != null)
                            {
                                foreach (dsMATRIX_TYPE.MATRIX_TYPERow drMATRIX_TYPE in dtMATRIX_TYPE.Rows)
                                {
                                    AddTombstone(drMATRIX_TYPE.GUID, Sync_Type.MATRIX_TYPE, dtTOMBSTONE);
                                    drMATRIX_TYPE.Delete();
                                }

                                daMATRIX_TYPE.Save(dtMATRIX_TYPE);
                            }
                        }
                    }

                    //Scope: Global
                    if (EnabledSyncOption.Any(obj => obj.OptionName == Sync_Item.Template.ToString()))
                    {
                        using (AdapterWORKFLOW_MAIN daWORKFLOW_MAIN = new AdapterWORKFLOW_MAIN())
                        {
                            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWORKFLOW_MAIN = daWORKFLOW_MAIN.Get_Deleted();
                            if (dtWORKFLOW_MAIN != null)
                            {
                                foreach (dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWORKFLOW_MAIN in dtWORKFLOW_MAIN.Rows)
                                {
                                    AddTombstone(drWORKFLOW_MAIN.GUID, Sync_Type.WORKFLOW_MAIN, dtTOMBSTONE);
                                    drWORKFLOW_MAIN.Delete();
                                }

                                daWORKFLOW_MAIN.Save(dtWORKFLOW_MAIN);
                            }
                        }

                        using (AdapterTEMPLATE_MAIN daTEMPLATE_MAIN = new AdapterTEMPLATE_MAIN())
                        {
                            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = daTEMPLATE_MAIN.Get_Deleted();
                            if (dtTEMPLATE_MAIN != null)
                            {
                                foreach (dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTEMPLATE_MAIN in dtTEMPLATE_MAIN.Rows)
                                {
                                    AddTombstone(drTEMPLATE_MAIN.GUID, Sync_Type.TEMPLATE_MAIN, dtTOMBSTONE);
                                    drTEMPLATE_MAIN.Delete();
                                }

                                daTEMPLATE_MAIN.Save(dtTEMPLATE_MAIN);
                            }
                        }
                    }

                    //Scope: Global
                    if (EnabledSyncOption.Any(obj => obj.OptionName == Sync_Item.Role.ToString()))
                    {
                        using (AdapterROLE_MAIN daROLE_MAIN = new AdapterROLE_MAIN())
                        {
                            dsROLE_MAIN.ROLE_MAINDataTable dtROLE_MAIN = daROLE_MAIN.Get_Deleted();
                            if (dtROLE_MAIN != null)
                            {
                                foreach (dsROLE_MAIN.ROLE_MAINRow drROLE_MAIN in dtROLE_MAIN.Rows)
                                {
                                    AddTombstone(drROLE_MAIN.GUID, Sync_Type.ROLE_MAIN, dtTOMBSTONE);
                                    drROLE_MAIN.Delete();
                                }

                                daROLE_MAIN.Save(dtROLE_MAIN);
                            }

                            dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtROLE_PRIVILEGE = daROLE_MAIN.Get_Deleted_Privilege();
                            if (dtROLE_PRIVILEGE != null)
                            {
                                foreach (dsROLE_MAIN.ROLE_PRIVILEGERow drROLE_PRIVILEGE in dtROLE_PRIVILEGE.Rows)
                                {
                                    AddTombstone(drROLE_PRIVILEGE.GUID, Sync_Type.ROLE_PRIVILEGE, dtTOMBSTONE);
                                    drROLE_PRIVILEGE.Delete();
                                }

                                daROLE_MAIN.Save(dtROLE_PRIVILEGE);
                            }
                        }
                    }

                    //Scope: Global
                    if (EnabledSyncOption.Any(obj => obj.OptionName == Sync_Item.User.ToString()))
                    {
                        using (AdapterUSER_MAIN daUSER_MAIN = new AdapterUSER_MAIN())
                        {
                            dsUSER_MAIN.USER_MAINDataTable dtUSER_MAIN = daUSER_MAIN.Get_Deleted();
                            if (dtUSER_MAIN != null)
                            {
                                foreach (dsUSER_MAIN.USER_MAINRow drUSER_MAIN in dtUSER_MAIN.Rows)
                                {
                                    AddTombstone(drUSER_MAIN.GUID, Sync_Type.USER_MAIN, dtTOMBSTONE);
                                    drUSER_MAIN.Delete();
                                }

                                daUSER_MAIN.Save(dtUSER_MAIN);
                            }

                            dsUSER_MAIN.USER_PROJECTDataTable dtUSER_PROJECT = daUSER_MAIN.Get_Deleted_UserProjects();
                            if (dtUSER_PROJECT != null)
                            {
                                foreach (dsUSER_MAIN.USER_PROJECTRow drUSER_PROJECT in dtUSER_PROJECT.Rows)
                                {
                                    AddTombstone(drUSER_PROJECT.GUID, Sync_Type.USER_PROJECT, dtTOMBSTONE);
                                    drUSER_PROJECT.Delete();
                                }

                                daUSER_MAIN.Save(dtUSER_PROJECT);
                            }

                            dsUSER_MAIN.USER_DISCDataTable dtUSER_DISC = daUSER_MAIN.Get_Deleted_UserDisciplines();
                            if (dtUSER_DISC != null)
                            {
                                foreach (dsUSER_MAIN.USER_DISCRow drUSER_DISC in dtUSER_DISC.Rows)
                                {
                                    AddTombstone(drUSER_DISC.GUID, Sync_Type.USER_DISC, dtTOMBSTONE);
                                    drUSER_DISC.Delete();
                                }

                                daUSER_MAIN.Save(dtUSER_DISC);
                            }
                        }
                    }

                    //Scope: Project
                    if (EnabledSyncOption.Any(obj => obj.OptionName == Sync_Item.Schedule.ToString()))
                    {
                        using (AdapterSCHEDULE daSCHEDULE = new AdapterSCHEDULE())
                        {
                            dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE = daSCHEDULE.Get_Deleted(_projectGuid);
                            if (dtSCHEDULE != null)
                            {
                                foreach (dsSCHEDULE.SCHEDULERow drSCHEDULE in dtSCHEDULE.Rows)
                                {
                                    AddTombstone(drSCHEDULE.GUID, Sync_Type.SCHEDULE, dtTOMBSTONE);
                                    drSCHEDULE.Delete();
                                }

                                daSCHEDULE.Save(dtSCHEDULE);
                            }
                        }

                        using (AdapterMATRIX_ASSIGNMENT daMATRIX_ASSIGNMENT = new AdapterMATRIX_ASSIGNMENT())
                        {
                            dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMATRIX_ASSIGNMENT = daMATRIX_ASSIGNMENT.Get_Deleted(_projectGuid);
                            if (dtMATRIX_ASSIGNMENT != null)
                            {
                                foreach (dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow drMATRIX_ASSIGNMENT in dtMATRIX_ASSIGNMENT.Rows)
                                {
                                    AddTombstone(drMATRIX_ASSIGNMENT.GUID, Sync_Type.MATRIX_ASSIGNMENT, dtTOMBSTONE);
                                    drMATRIX_ASSIGNMENT.Delete();
                                }

                                daMATRIX_ASSIGNMENT.Save(dtMATRIX_ASSIGNMENT);
                            }
                        }

                        using (AdapterTAG daTAG = new AdapterTAG())
                        {
                            dsTAG.TAGDataTable dtTAG = daTAG.Get_Deleted(_projectGuid);
                            if (dtTAG != null)
                            {
                                foreach (dsTAG.TAGRow drTAG in dtTAG.Rows)
                                {
                                    AddTombstone(drTAG.GUID, Sync_Type.TAG, dtTOMBSTONE);
                                    drTAG.Delete();
                                }

                                daTAG.Save(dtTAG);
                            }
                        }

                        using (AdapterWBS daWBS = new AdapterWBS())
                        {
                            dsWBS.WBSDataTable dtWBS = daWBS.Get_Deleted(_projectGuid);
                            if (dtWBS != null)
                            {
                                foreach (dsWBS.WBSRow drWBS in dtWBS.Rows)
                                {
                                    AddTombstone(drWBS.GUID, Sync_Type.WBS, dtTOMBSTONE);
                                    drWBS.Delete();
                                }

                                daWBS.Save(dtWBS);
                            }
                        }

                        using (AdapterTEMPLATE_REGISTER daTEMPLATE_REGISTER = new AdapterTEMPLATE_REGISTER())
                        {
                            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER = daTEMPLATE_REGISTER.Get_Deleted(_projectGuid);
                            if (dtTEMPLATE_REGISTER != null)
                            {
                                foreach (dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTEMPLATE_REGISTER in dtTEMPLATE_REGISTER.Rows)
                                {
                                    AddTombstone(drTEMPLATE_REGISTER.GUID, Sync_Type.TEMPLATE_REGISTER, dtTOMBSTONE);
                                    drTEMPLATE_REGISTER.Delete();
                                }

                                daTEMPLATE_REGISTER.Save(dtTEMPLATE_REGISTER);
                            }
                        }
                    }

                    //Scope: Project
                    if (EnabledSyncOption.Any(obj => obj.OptionName == Common.Replace_WithSpaces(Sync_Item.Header_Data.ToString())))
                    {
                        using (AdapterPREFILL_REGISTER daPREFILL_REGISTER = new AdapterPREFILL_REGISTER())
                        {
                            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPREFILL_REGISTER = daPREFILL_REGISTER.Get_Deleted(_projectGuid);
                            if (dtPREFILL_REGISTER != null)
                            {
                                List<dsPREFILL_REGISTER.PREFILL_REGISTERRow> deleteRows = new List<dsPREFILL_REGISTER.PREFILL_REGISTERRow>();
                                foreach (dsPREFILL_REGISTER.PREFILL_REGISTERRow drPREFILL_REGISTER in dtPREFILL_REGISTER.Rows)
                                {
                                    AddTombstone(drPREFILL_REGISTER.GUID, Sync_Type.PREFILL_REGISTER, dtTOMBSTONE);
                                    drPREFILL_REGISTER.Delete();
                                }

                                daPREFILL_REGISTER.Save(dtPREFILL_REGISTER);
                            }
                        }
                    }

                    //Scope: Project
                    if (EnabledSyncOption.Any(obj => obj.OptionName == Sync_Item.ITR.ToString()))
                    {
                        using (AdapterITR_MAIN daITR_MAIN = new AdapterITR_MAIN())
                        {
                            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = daITR_MAIN.Get_Deleted(_projectGuid);
                            if (dtITR_MAIN != null)
                            {
                                foreach (dsITR_MAIN.ITR_MAINRow drITR_MAIN in dtITR_MAIN.Rows)
                                {
                                    AddTombstone(drITR_MAIN.GUID, Sync_Type.ITR_MAIN, dtTOMBSTONE);
                                    drITR_MAIN.Delete();
                                }

                                daITR_MAIN.Save(dtITR_MAIN);
                            }
                        }

                        using (AdapterITR_STATUS daITR_STATUS = new AdapterITR_STATUS())
                        {
                            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = daITR_STATUS.Get_Deleted(_projectGuid);
                            if (dtITR_STATUS != null)
                            {
                                foreach (dsITR_STATUS.ITR_STATUSRow drITR_STATUS in dtITR_STATUS.Rows)
                                {
                                    AddTombstone(drITR_STATUS.GUID, Sync_Type.ITR_STATUS, dtTOMBSTONE);
                                    drITR_STATUS.Delete();
                                }

                                daITR_STATUS.Save(dtITR_STATUS);
                            }
                        }
                    }

                    //Scope: Project
                    if (EnabledSyncOption.Any(obj => obj.OptionName == Sync_Item.Punchlist.ToString()))
                    {
                        using (AdapterPUNCHLIST_MAIN daPUNCHLIST_MAIN = new AdapterPUNCHLIST_MAIN())
                        {
                            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_MAIN = daPUNCHLIST_MAIN.Get_Deleted(_projectGuid);
                            if (dtPUNCHLIST_MAIN != null)
                            {
                                foreach (dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPUNCHLIST_MAIN in dtPUNCHLIST_MAIN.Rows)
                                {
                                    AddTombstone(drPUNCHLIST_MAIN.GUID, Sync_Type.PUNCHLIST_MAIN, dtTOMBSTONE);
                                    drPUNCHLIST_MAIN.Delete();
                                }

                                daPUNCHLIST_MAIN.Save(dtPUNCHLIST_MAIN);
                            }
                        }

                        using (AdapterPUNCHLIST_STATUS daPUNCHLIST_STATUS = new AdapterPUNCHLIST_STATUS())
                        {
                            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPUNCHLIST_STATUS = daPUNCHLIST_STATUS.Get_Deleted(_projectGuid);
                            if (dtPUNCHLIST_STATUS != null)
                            {
                                foreach (dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow drPUNCHLIST_STATUS in dtPUNCHLIST_STATUS.Rows)
                                {
                                    AddTombstone(drPUNCHLIST_STATUS.GUID, Sync_Type.PUNCHLIST_STATUS, dtTOMBSTONE);
                                    drPUNCHLIST_STATUS.Delete();
                                }

                                daPUNCHLIST_STATUS.Save(dtPUNCHLIST_STATUS);
                            }
                        }
                    }
                    #endregion

                    trimCount = dtTOMBSTONE.Rows.Count;
                    if(dtTOMBSTONE.Rows.Count > 0)
                    {
                        daTOMBSTONE.Save(dtTOMBSTONE);
                    }
                }
            }

            splashScreenManager1.CloseWaitForm();
            if (trimCount == 0)
                Common.Prompt("No records trimmed");
            else
                Common.Prompt(trimCount.ToString() + " records trimmed\nPlease ensure that all devices are synced before purging");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Add a new row to tombstone
        /// </summary>
        /// <param name="DeletedItemGuid">Guid of deleted item</param>
        /// <param name="dtTOMBSTONE">Datatable to add the tombstone row to</param>
        private void AddTombstone(Guid DeletedItemGuid, Sync_Type SyncType , dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE)
        {
            dsTOMBSTONE.TOMBSTONERow drTOMBSTONE = dtTOMBSTONE.NewTOMBSTONERow();
            drTOMBSTONE.GUID = Guid.NewGuid();
            drTOMBSTONE.TABLENAME = SyncType.ToString();
            drTOMBSTONE.TOMBSTONE_GUID = DeletedItemGuid;
            drTOMBSTONE.CREATED = DateTime.Now;
            drTOMBSTONE.CREATEDBY = System_Environment.GetUser().GUID;
            dtTOMBSTONE.AddTOMBSTONERow(drTOMBSTONE);
        }
        #endregion

        #region Background Events
        /// <summary>
        /// Bind node checkstate to optionenabled member
        /// </summary>
        private void treeListDatabase_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
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
        }
        #endregion
    }
}
