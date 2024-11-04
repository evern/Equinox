using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Linq;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectCommon;
using ProjectLibrary;
using System.IO;
using System.Xml.Linq;
using DevExpress.XtraPrinting;

namespace CheckmateDX
{
    public enum SyncStatusDisplay { same, upload, download, warning }
    public partial class frmSync_Status_Superseded : CheckmateDX.frmParent
    {
        //Environment Variables
        Guid _projectGuid;
        //Changes 7-May-2015 : Sync tag and wbs by entire project
        //string _discipline;

        //Locals
        AdapterTAG _daTagLocal = new AdapterTAG();
        AdapterWBS _daWBSLocal = new AdapterWBS();
        AdapterITR_MAIN _daITRLocal = new AdapterITR_MAIN();
        AdapterITR_STATUS _daITRStatusLocal = new AdapterITR_STATUS();
        AdapterITR_STATUS_ISSUE _daITRIssueLocal = new AdapterITR_STATUS_ISSUE();
        AdapterPUNCHLIST_MAIN _daPunchlistLocal = new AdapterPUNCHLIST_MAIN();
        AdapterPUNCHLIST_STATUS _daPunchlistStatusLocal = new AdapterPUNCHLIST_STATUS();
        AdapterPUNCHLIST_STATUS_ISSUE _daPunchlistIssueLocal = new AdapterPUNCHLIST_STATUS_ISSUE();
        AdapterUSER_MAIN _daUserLocal = new AdapterUSER_MAIN();
        AdapterROLE_MAIN _daRoleLocal = new AdapterROLE_MAIN();
        AdapterTEMPLATE_MAIN _daTemplateLocal = new AdapterTEMPLATE_MAIN();
        AdapterTEMPLATE_REGISTER _daRegisterLocal = new AdapterTEMPLATE_REGISTER();
        AdapterPREFILL_MAIN _daPrefillLocal = new AdapterPREFILL_MAIN();
        AdapterPREFILL_REGISTER _daPrefillRegLocal = new AdapterPREFILL_REGISTER();
        AdapterWORKFLOW_MAIN _daWorkflowLocal = new AdapterWORKFLOW_MAIN();
        AdapterPROJECT _daProjectLocal = new AdapterPROJECT();
        AdapterSCHEDULE _daScheduleLocal = new AdapterSCHEDULE();
        AdapterSYNC_HISTORY _daSyncHistoryLocal = new AdapterSYNC_HISTORY();
        AdapterGENERAL_EQUIPMENT _daEquipmentLocal = new AdapterGENERAL_EQUIPMENT();

        //Remotes
        AdapterTAG _daTagRemote;
        AdapterWBS _daWBSRemote;
        AdapterITR_MAIN _daITRRemote;
        AdapterITR_STATUS _daITRStatusRemote;
        AdapterITR_STATUS_ISSUE _daITRIssueRemote;
        AdapterPUNCHLIST_MAIN _daPunchlistRemote;
        AdapterPUNCHLIST_STATUS _daPunchlistStatusRemote;
        AdapterPUNCHLIST_STATUS_ISSUE _daPunchlistIssueRemote;
        AdapterSYNC_PAIR _daSyncPairRemote;
        AdapterSYNC_TABLE _daSyncTableRemote;
        AdapterUSER_MAIN _daUserRemote;
        AdapterROLE_MAIN _daRoleRemote;
        AdapterTEMPLATE_MAIN _daTemplateRemote;
        AdapterTEMPLATE_REGISTER _daRegisterRemote;
        AdapterPREFILL_MAIN _daPrefillRemote;
        AdapterPREFILL_REGISTER _daPrefillRegRemote;
        AdapterWORKFLOW_MAIN _daWorkflowRemote;
        AdapterPROJECT _daProjectRemote;
        AdapterSCHEDULE _daScheduleRemote;
        AdapterSYNC_HISTORY _daSyncHistoryRemote;
        AdapterGENERAL_EQUIPMENT _daEquipmentRemote;

        List<SyncStatus_Superseded> _SyncItems = new List<SyncStatus_Superseded>();

        //String variables
        string _HWID = Common.GetHWID();
        Guid _localSyncGuid = Guid.Empty; //will only be populated if sync is approved

        public frmSync_Status_Superseded()
        {
            InitializeComponent();
            string serverName;
            string remoteConnStr = GetConnStrFromXML(out serverName);
            lblSyncServer.Text = serverName;
            RemoteAdapterInitialisation(remoteConnStr);
            syncStatusBindingSource.DataSource = _SyncItems;

            //get user parameter
            _projectGuid = (Guid)System_Environment.GetUser().userProject.Value;

            //Changes 7-May-2015 : Sync tag and wbs by entire project
            //_discipline = System_Environment.GetUser().userDiscipline;

            timer1.Enabled = true;
        }

        //Displays sync history by hwid
        public frmSync_Status_Superseded(string hwid, string machineName)
        {
            InitializeComponent();
            lblSyncServer.Text = machineName;
            AdapterSYNC_HISTORY daSyncHistory = new AdapterSYNC_HISTORY();
            dsSYNC_HISTORY.SYNC_HISTORYDataTable dtSyncHistory = daSyncHistory.GetBy(hwid);

            if (dtSyncHistory == null)
                return;

            foreach(dsSYNC_HISTORY.SYNC_HISTORYRow drSyncHistory in dtSyncHistory.Rows)
            {
                _SyncItems.Add(new SyncStatus_Superseded(drSyncHistory.SYNC_ITEM_GUID, drSyncHistory.SYNC_PARENT_GUID)
                {
                    status = drSyncHistory.STATUS,
                    imageIndex = (int)drSyncHistory.SYNC_DIRECTION,
                    additionalInfo = drSyncHistory.IsSYNC_DESCNull() ? string.Empty : drSyncHistory.SYNC_DESC,
                    syncItemName = drSyncHistory.IsSYNC_ITEMNull() ? string.Empty : drSyncHistory.SYNC_ITEM,
                    type = drSyncHistory.TYPE,
                    SyncDate = drSyncHistory.SYNCED,
                    SyncBy = drSyncHistory.SYNCED_BY,
                    CreatedDate = drSyncHistory.CREATED,
                    CreatedBy = drSyncHistory.CREATED_BY,
                    UpdatedDate = drSyncHistory.IsUPDATEDNull() ? DateTime.MinValue : drSyncHistory.UPDATED,
                    UpdatedBy = drSyncHistory.IsUPDATED_BYNull() ? Guid.Empty : drSyncHistory.UPDATED_BY,
                    DeletedDate = drSyncHistory.IsDELETEDNull() ? DateTime.MinValue : drSyncHistory.DELETED,
                    DeletedBy = drSyncHistory.IsDELETED_BYNull() ? Guid.Empty : drSyncHistory.DELETED_BY
                });
            }

            syncStatusBindingSource.DataSource = _SyncItems;
            treeListSyncStatus.ExpandAll();
            treeListSyncStatus.BestFitColumns();
        }

        #region Display Process
        /// <summary>
        /// Updates the displayed status
        /// </summary>
        private void UpdateSyncItem(Guid syncGuid, string additionalInfo, string syncItemName, string status, SyncStatusDisplay statustype)
        {
            SyncStatus_Superseded findSync = _SyncItems.FirstOrDefault(obj => obj.GUID == syncGuid);
            if(findSync != null)
            {
                findSync.additionalInfo = additionalInfo.Equals(string.Empty) ? findSync.additionalInfo : additionalInfo;
                findSync.syncItemName = syncItemName.Equals(string.Empty) ? findSync.syncItemName : syncItemName;
                findSync.status = status.Equals(string.Empty) ? findSync.status : status;

                switch(statustype)
                {
                    case SyncStatusDisplay.download:
                        findSync.imageIndex = 0;
                        break;
                    case SyncStatusDisplay.upload:
                        findSync.imageIndex = 1;
                        break;
                    case SyncStatusDisplay.same:
                        findSync.imageIndex = 2;
                        break;
                    case SyncStatusDisplay.warning:
                        findSync.imageIndex = 3;
                        break;
                }
            }
        }

        /// <summary>
        /// Update sync date used for ITR and Punchlist because those variables aren't known at creation time
        /// </summary>
        public void UpdateSyncDate(Guid syncGuid, DateTime created, Guid createdby, DateTime updated, Guid updatedby, DateTime deleted, Guid deletedBy)
        {
            SyncStatus_Superseded findSync = _SyncItems.FirstOrDefault(obj => obj.GUID == syncGuid);
            if(findSync != null)
            {
                findSync.CreatedDate = created.Equals(DateTime.MinValue) ? findSync.CreatedDate : created;
                findSync.CreatedBy = createdby.Equals(Guid.Empty) ? findSync.CreatedBy : createdby;
                findSync.UpdatedDate = updated.Equals(DateTime.MinValue) ? findSync.UpdatedDate : updated;
                findSync.UpdatedBy = updatedby.Equals(Guid.Empty) ? findSync.UpdatedBy : updatedby;
                findSync.DeletedDate = deleted.Equals(DateTime.MinValue) ? findSync.DeletedDate : deleted;
                findSync.DeletedBy = deletedBy.Equals(Guid.Empty) ? findSync.DeletedBy : deletedBy;
            }
        }
        #endregion

        #region Sync Process
        /// <summary>
        /// Perform the ITR Sync
        /// </summary>
        //Changes 7-May-2015 : Sync tag and wbs by entire project
        //private void Sync_ITR(Guid projectGuid, string discipline)
        private void Sync_ITR(Guid projectGuid)
        {
            byte[] localTemplate;
            byte[] remoteTemplate;
            byte[] localHash;
            byte[] remoteHash;

            dsITR_MAIN dsITR = new dsITR_MAIN();
            //Changes 7-May-2015 : Sync tag and wbs by entire project
            //List<SyncITR> iTRs = EstablishSyncITRs(projectGuid, discipline);
            List<SyncITR> iTRs = EstablishSyncITRs(projectGuid);
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("Syncing ITRs ... ");
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetLargeFormat, 500);
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, iTRs.Count);

            foreach (SyncITR iTR in iTRs)
            {
                int localITRScore = RateLocalITR(iTR);
                int remoteITRScore = RateRemoteITR(iTR);

                Guid SyncTagWBSGuid = Guid.NewGuid();
                _SyncItems.Add(new SyncStatus_Superseded(SyncTagWBSGuid, projectGuid)
                    {
                        syncItemName = iTR.templateName,
                        additionalInfo = iTR.attachmentName,
                        type = Sync_Type_Superseded.ITR.ToString(),
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID
                    });

                dsITR_MAIN.ITR_MAINRow drITRLocal;
                dsITR_MAIN.ITR_MAINRow drITRRemote;

                if (iTR.isWBS)
                {
                    drITRLocal = _daITRLocal.GetByWBSName(iTR.attachmentName, iTR.templateName, projectGuid);
                    drITRRemote = _daITRRemote.GetByWBSName(iTR.attachmentName, iTR.templateName, projectGuid);
                }
                else
                {
                    drITRLocal = _daITRLocal.GetByTagName(iTR.attachmentName, iTR.templateName, projectGuid);
                    drITRRemote = _daITRRemote.GetByTagName(iTR.attachmentName, iTR.templateName, projectGuid);
                }

                if(localITRScore != remoteITRScore)
                {
                    dsITR_MAIN.ITR_MAINRow drITRNew = dsITR.ITR_MAIN.NewITR_MAINRow();
                    Guid attachmentGuid = Guid.Empty;
                    //when either is null
                    if(drITRLocal == null)
                    {
                        if (!drITRRemote.IsTAG_GUIDNull())
                            attachmentGuid = isTagWBSExist(_daTagLocal, null, iTR.attachmentName, projectGuid);
                        else if(!drITRRemote.IsWBS_GUIDNull())
                            attachmentGuid = isTagWBSExist(null, _daWBSLocal, iTR.attachmentName, projectGuid);

                        if(attachmentGuid != Guid.Empty) //Check if WBSTag exists
                        {
                            drITRNew.ItemArray = drITRRemote.ItemArray; //even GUID is copied over, because if deleted locally it'll remain deleted

                            dsITR_MAIN.ITR_MAINRow drITRDeleted = _daITRLocal.GetDeletedITR(drITRRemote.GUID);
                            if(drITRDeleted == null)
                            {
                                //WBSTag guid correction
                                if (!drITRRemote.IsTAG_GUIDNull())
                                    drITRNew.TAG_GUID = attachmentGuid;
                                else
                                    drITRNew.WBS_GUID = attachmentGuid;

                                dsITR.ITR_MAIN.AddITR_MAINRow(drITRNew);
                                _daITRLocal.Save(drITRNew);
                                CopyITRStatusAndIssue(drITRRemote.GUID, drITRNew.GUID, _daITRStatusRemote, _daITRStatusLocal, _daITRIssueRemote, _daITRIssueLocal);
                                UpdateSyncDate(SyncTagWBSGuid, drITRNew.CREATED, drITRNew.CREATEDBY, drITRNew.IsUPDATEDNull() ? DateTime.MinValue : drITRNew.UPDATED, drITRNew.IsUPDATEDBYNull() ? Guid.Empty : drITRNew.UPDATEDBY, drITRNew.IsDELETEDNull() ? DateTime.MinValue : drITRNew.DELETED, drITRNew.IsDELETEDBYNull() ? Guid.Empty : drITRNew.DELETEDBY);
                                UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Created", SyncStatusDisplay.download);
                            }
                            else
                            {
                                //Change 12-05-2015: Delete remote ITR if it was deleted locally
                                drITRRemote.DELETED = drITRDeleted.DELETED;
                                drITRRemote.DELETEDBY = drITRDeleted.DELETEDBY;
                                _daITRRemote.Save(drITRRemote);
                                UpdateSyncDate(SyncTagWBSGuid, drITRDeleted.CREATED, drITRDeleted.CREATEDBY, drITRDeleted.IsUPDATEDNull() ? DateTime.MinValue : drITRDeleted.UPDATED, drITRDeleted.IsUPDATEDBYNull() ? Guid.Empty : drITRDeleted.UPDATEDBY, drITRDeleted.IsDELETEDNull() ? DateTime.MinValue : drITRDeleted.DELETED, drITRDeleted.IsDELETEDBYNull() ? Guid.Empty : drITRDeleted.DELETEDBY);
                                UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Deleted", SyncStatusDisplay.upload);
                            }
                        }
                        else
                        {
                            UpdateSyncDate(SyncTagWBSGuid, drITRRemote.CREATED, drITRRemote.CREATEDBY, drITRRemote.IsUPDATEDNull() ? DateTime.MinValue : drITRRemote.UPDATED, drITRRemote.IsUPDATEDBYNull() ? Guid.Empty : drITRRemote.UPDATEDBY, drITRRemote.IsDELETEDNull() ? DateTime.MinValue : drITRRemote.DELETED, drITRRemote.IsDELETEDBYNull() ? Guid.Empty : drITRRemote.DELETEDBY);
                            UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Attachment Not Found", SyncStatusDisplay.same);
                        }
                    }
                    else if(drITRRemote == null)
                    {
                        if (!drITRLocal.IsTAG_GUIDNull())
                            attachmentGuid = isTagWBSExist(_daTagRemote, null, iTR.attachmentName, projectGuid);
                        else if (!drITRLocal.IsWBS_GUIDNull())
                            attachmentGuid = isTagWBSExist(null, _daWBSRemote, iTR.attachmentName, projectGuid);

                        if (attachmentGuid != Guid.Empty)
                        {
                            drITRNew.ItemArray = drITRLocal.ItemArray;

                            dsITR_MAIN.ITR_MAINRow drITRDeleted = _daITRRemote.GetDeletedITR(drITRLocal.GUID);
                            if (drITRDeleted == null)
                            {
                                //WBSTag guid correction
                                if (!drITRLocal.IsTAG_GUIDNull())
                                    drITRNew.TAG_GUID = attachmentGuid;
                                else
                                    drITRNew.WBS_GUID = attachmentGuid;

                                dsITR.ITR_MAIN.AddITR_MAINRow(drITRNew);
                                _daITRRemote.Save(drITRNew);
                                CopyITRStatusAndIssue(drITRLocal.GUID, drITRNew.GUID, _daITRStatusLocal, _daITRStatusRemote, _daITRIssueLocal, _daITRIssueRemote);
                                UpdateSyncDate(SyncTagWBSGuid, drITRNew.CREATED, drITRNew.CREATEDBY, drITRNew.IsUPDATEDNull() ? DateTime.MinValue : drITRNew.UPDATED, drITRNew.IsUPDATEDBYNull() ? Guid.Empty : drITRNew.UPDATEDBY, drITRNew.IsDELETEDNull() ? DateTime.MinValue : drITRNew.DELETED, drITRNew.IsDELETEDBYNull() ? Guid.Empty : drITRNew.DELETEDBY);
                                UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Created", SyncStatusDisplay.upload);
                            }
                            else
                            {
                                //Change 12-05-2015: Delete local ITR if it was deleted remotely
                                drITRLocal.DELETED = drITRDeleted.DELETED;
                                drITRLocal.DELETEDBY = drITRDeleted.DELETEDBY;
                                _daITRLocal.Save(drITRLocal);
                                UpdateSyncDate(SyncTagWBSGuid, drITRDeleted.CREATED, drITRDeleted.CREATEDBY, drITRDeleted.IsUPDATEDNull() ? DateTime.MinValue : drITRDeleted.UPDATED, drITRDeleted.IsUPDATEDBYNull() ? Guid.Empty : drITRDeleted.UPDATEDBY, drITRDeleted.IsDELETEDNull() ? DateTime.MinValue : drITRDeleted.DELETED, drITRDeleted.IsDELETEDBYNull() ? Guid.Empty : drITRDeleted.DELETEDBY);
                                UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Deleted", SyncStatusDisplay.download);
                            } 
                        }
                        else
                        {
                            UpdateSyncDate(SyncTagWBSGuid, drITRLocal.CREATED, drITRLocal.CREATEDBY, drITRLocal.IsUPDATEDNull() ? DateTime.MinValue : drITRLocal.UPDATED, drITRLocal.IsUPDATEDBYNull() ? Guid.Empty : drITRLocal.UPDATEDBY, drITRLocal.IsDELETEDNull() ? DateTime.MinValue : drITRLocal.DELETED, drITRLocal.IsDELETEDBYNull() ? Guid.Empty : drITRLocal.DELETEDBY);
                            UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Attachment Not Found", SyncStatusDisplay.same);
                        }   
                    }
                    //don't need to validate both is null from here one because they would've have the same score
                    //when both is not null
                    else
                    {
                        localTemplate = drITRLocal.ITR;
                        remoteTemplate = drITRRemote.ITR;

                        localHash = new MD5CryptoServiceProvider().ComputeHash(localTemplate);
                        remoteHash = new MD5CryptoServiceProvider().ComputeHash(remoteTemplate);

                        if(SameHash(localHash, remoteHash)) //copy status only if same hash
                        {
                            if(localITRScore > remoteITRScore)
                            {
                                CopyITRStatusAndIssue(drITRLocal.GUID, drITRRemote.GUID, _daITRStatusLocal, _daITRStatusRemote, _daITRIssueLocal, _daITRIssueRemote);
                                UpdateSyncDate(SyncTagWBSGuid, drITRLocal.CREATED, drITRLocal.CREATEDBY, drITRLocal.IsUPDATEDNull() ? DateTime.MinValue : drITRLocal.UPDATED, drITRLocal.IsUPDATEDBYNull() ? Guid.Empty : drITRLocal.UPDATEDBY, drITRLocal.IsDELETEDNull() ? DateTime.MinValue : drITRNew.DELETED, drITRLocal.IsDELETEDBYNull() ? Guid.Empty : drITRLocal.DELETEDBY);
                                UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Status Update", SyncStatusDisplay.upload);
                            }
                            else if(remoteITRScore > localITRScore)
                            {
                                CopyITRStatusAndIssue(drITRRemote.GUID, drITRLocal.GUID, _daITRStatusRemote, _daITRStatusLocal, _daITRIssueRemote, _daITRIssueLocal);
                                UpdateSyncDate(SyncTagWBSGuid, drITRRemote.CREATED, drITRRemote.CREATEDBY, drITRRemote.IsUPDATEDNull() ? DateTime.MinValue : drITRRemote.UPDATED, drITRRemote.IsUPDATEDBYNull() ? Guid.Empty : drITRRemote.UPDATEDBY, drITRRemote.IsDELETEDNull() ? DateTime.MinValue : drITRRemote.DELETED, drITRRemote.IsDELETEDBYNull() ? Guid.Empty : drITRRemote.DELETEDBY);
                                UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Status Update", SyncStatusDisplay.download);
                            }
                        }
                        else //create new sequence if different hash
                        {
                            dsITR_MAIN.ITR_MAINRow drNewITR = dsITR.ITR_MAIN.NewITR_MAINRow();
                            if (localITRScore > remoteITRScore)
                            {
                                drNewITR.ItemArray = drITRLocal.ItemArray;
                                drNewITR.GUID = Guid.NewGuid();
                                //WBSTag guid correction because it was retrieved by name earlier
                                if(!drITRRemote.IsTAG_GUIDNull())
                                    drNewITR.TAG_GUID = drITRRemote.TAG_GUID;
                                else if(!drITRRemote.IsWBS_GUIDNull())
                                    drNewITR.WBS_GUID = drITRRemote.WBS_GUID;

                                drNewITR.SEQUENCE_NUMBER = drITRRemote.SEQUENCE_NUMBER + 1;
                                dsITR.ITR_MAIN.AddITR_MAINRow(drNewITR);
                                _daITRRemote.RemoveBy(drITRRemote.GUID);
                                _daITRRemote.Save(drNewITR);
                                CopyITRStatusAndIssue(drITRLocal.GUID, drNewITR.GUID, _daITRStatusLocal, _daITRStatusRemote, _daITRIssueLocal, _daITRIssueRemote);

                                UpdateSyncDate(SyncTagWBSGuid, drNewITR.CREATED, drNewITR.CREATEDBY, drNewITR.IsUPDATEDNull() ? DateTime.MinValue : drNewITR.UPDATED, drNewITR.IsUPDATEDBYNull() ? Guid.Empty : drNewITR.UPDATEDBY, drNewITR.IsDELETEDNull() ? DateTime.MinValue : drNewITR.DELETED, drNewITR.IsDELETEDBYNull() ? Guid.Empty : drNewITR.DELETEDBY);
                                UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Revised", SyncStatusDisplay.upload);
                            }
                            else if (remoteITRScore > localITRScore)
                            {
                                drNewITR.ItemArray = drITRRemote.ItemArray;
                                drNewITR.GUID = Guid.NewGuid(); //the reason new guid is used is for version control
                                //WBSTag guid correction because it was retrieved by name and project earlier
                                if (!drITRLocal.IsTAG_GUIDNull())
                                    drNewITR.TAG_GUID = drITRLocal.TAG_GUID;
                                else if (!drITRLocal.IsWBS_GUIDNull())
                                    drNewITR.WBS_GUID = drITRLocal.WBS_GUID;

                                drNewITR.SEQUENCE_NUMBER = drITRLocal.SEQUENCE_NUMBER + 1;
                                dsITR.ITR_MAIN.AddITR_MAINRow(drNewITR);
                                _daITRLocal.RemoveBy(drITRLocal.GUID);
                                _daITRLocal.Save(drNewITR);
                                CopyITRStatusAndIssue(drITRRemote.GUID, drNewITR.GUID, _daITRStatusRemote, _daITRStatusLocal, _daITRIssueRemote, _daITRIssueLocal);
                                
                                UpdateSyncDate(SyncTagWBSGuid, drNewITR.CREATED, drNewITR.CREATEDBY, drNewITR.IsUPDATEDNull() ? DateTime.MinValue : drNewITR.UPDATED, drNewITR.IsUPDATEDBYNull() ? Guid.Empty : drNewITR.UPDATEDBY, drNewITR.IsDELETEDNull() ? DateTime.MinValue : drNewITR.DELETED, drNewITR.IsDELETEDBYNull() ? Guid.Empty : drNewITR.DELETEDBY);
                                UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Revised", SyncStatusDisplay.download);
                            }
                            else
                            {
                                if(drITRLocal != null && drITRRemote != null)
                                {
                                    UpdateSyncDate(SyncTagWBSGuid, drITRLocal.CREATED, drITRLocal.CREATEDBY, drITRLocal.IsUPDATEDNull() ? DateTime.MinValue : drITRLocal.UPDATED, drITRLocal.IsUPDATEDBYNull() ? Guid.Empty : drITRLocal.UPDATEDBY, drITRLocal.IsDELETEDNull() ? DateTime.MinValue : drITRLocal.DELETED, drITRLocal.IsDELETEDBYNull() ? Guid.Empty : drITRLocal.DELETEDBY);
                                    UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Conflicting Content", SyncStatusDisplay.warning);
                                }
                            }
                        }
                    }
                }
                else //if both have the same score
                {
                    if(drITRLocal != null && drITRRemote != null)
                    {
                        localTemplate = drITRLocal.ITR;
                        remoteTemplate = drITRRemote.ITR;
                        localHash = new MD5CryptoServiceProvider().ComputeHash(localTemplate);
                        remoteHash = new MD5CryptoServiceProvider().ComputeHash(remoteTemplate);

                        UpdateSyncDate(SyncTagWBSGuid, drITRLocal.CREATED, drITRLocal.CREATEDBY, drITRLocal.IsUPDATEDNull() ? DateTime.MinValue : drITRLocal.UPDATED, drITRLocal.IsUPDATEDBYNull() ? Guid.Empty : drITRLocal.UPDATEDBY, drITRLocal.IsDELETEDNull() ? DateTime.MinValue : drITRLocal.DELETED, drITRLocal.IsDELETEDBYNull() ? Guid.Empty : drITRLocal.DELETEDBY);
                        if(SameHash(localHash, remoteHash))
                            UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        else
                            UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Conflicting Content", SyncStatusDisplay.warning);
                    }
                    //else (this won't happen because if both have the same score then it must be both null or both not null which is handled above)
                }

                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            splashScreenManager1.CloseWaitForm();
        }

        private void Sync_Punchlist_New(Guid projectGuid)
        {
            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_LOCAL = _daPunchlistLocal.GetAllTagPunchlist_ByProject(projectGuid);
            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_REMOTE = _daPunchlistRemote.GetAllTagPunchlist_ByProject(projectGuid);
            dsPUNCHLIST_MAIN dsPUNCHLIST = new dsPUNCHLIST_MAIN();

            //if local punchlist database is not empty by this project
            if (dtPUNCHLIST_LOCAL != null)
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Uploading Punchlist ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtPUNCHLIST_LOCAL.Rows.Count);

                foreach (dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPUNCHLIST_LOCAL in dtPUNCHLIST_LOCAL.Rows)
                {
                    _SyncItems.Add(new SyncStatus_Superseded(drPUNCHLIST_LOCAL.GUID, projectGuid)
                        {
                            additionalInfo = string.Empty,
                            syncItemName = string.Empty,
                            type = Sync_Type_Superseded.Punchlist.ToString(),
                            SyncDate = DateTime.Now,
                            SyncBy = System_Environment.GetUser().GUID
                        });

                    dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drFINDPUNCHLIST_REMOTE = dtPUNCHLIST_REMOTE.FirstOrDefault(obj => obj.GUID == drPUNCHLIST_LOCAL.GUID);

                    //remote punchlist not found - Add
                    if (drFINDPUNCHLIST_REMOTE == null)
                    {
                        //validate the tag number
                        dsTAG.TAGRow drTAG_REMOTE = _daTagRemote.GetIncludeDeletedBy(drPUNCHLIST_LOCAL.TAG_GUID);
                        string LocalPunchlist_ItemName = drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM == string.Empty ? "Ad-Hoc" : drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM;

                        //tag not found
                        if (drTAG_REMOTE == null)
                        {
                            dsTAG.TAGRow drTAG_LOCAL = _daTagLocal.GetIncludeDeletedBy(drPUNCHLIST_LOCAL.TAG_GUID);
                            if (drTAG_LOCAL != null)
                                UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_LOCAL.NUMBER, LocalPunchlist_ItemName, "Remote Tag Not Found", SyncStatusDisplay.warning);
                            else //unlikely to happen because of foreign key constraint
                                UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_LOCAL.NUMBER, LocalPunchlist_ItemName, "Local and Remote Tag Not Found", SyncStatusDisplay.warning);
                        }
                        //tag is found
                        else
                        {
                            dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drNEWPUNCHLIST_REMOTE = dsPUNCHLIST.PUNCHLIST_MAIN.NewPUNCHLIST_MAINRow();
                            drNEWPUNCHLIST_REMOTE.ItemArray = drPUNCHLIST_LOCAL.ItemArray;

                            //dont have to validate ITR number if it's ad-hoc
                            if (drPUNCHLIST_LOCAL.ITR_GUID == Guid.Empty)
                            {
                                UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_REMOTE.NUMBER, LocalPunchlist_ItemName, "Created | Ad-Hoc", SyncStatusDisplay.upload);
                            }
                            //adjust the ITR GUID if it's not ad-hoc
                            else
                            {
                                bool isDeleted = false;
                                Guid? toITR_GUID = QueryPunchlistITRGuid(drPUNCHLIST_LOCAL.ITR_GUID, drPUNCHLIST_LOCAL.TAG_GUID, _daITRLocal, _daITRRemote, out isDeleted);
                                //validate the ITR number
                                if (toITR_GUID != null)
                                {
                                    drNEWPUNCHLIST_REMOTE.ITR_GUID = (Guid)toITR_GUID;
                                    if(isDeleted)
                                        UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_REMOTE.NUMBER, LocalPunchlist_ItemName, "Created as Delete | ITR Adjusted", SyncStatusDisplay.upload);
                                    else
                                        UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_REMOTE.NUMBER, LocalPunchlist_ItemName, "Created | ITR Adjusted", SyncStatusDisplay.upload);
                                }
                                else
                                {
                                    UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_REMOTE.NUMBER, LocalPunchlist_ItemName, "Created | ITR Not Adjusted", SyncStatusDisplay.upload);
                                }
                            }

                            dsPUNCHLIST.PUNCHLIST_MAIN.AddPUNCHLIST_MAINRow(drNEWPUNCHLIST_REMOTE);
                            _daPunchlistRemote.Save(drNEWPUNCHLIST_REMOTE);
                            CopyPunchlistStatusAndIssue(drPUNCHLIST_LOCAL.GUID, drNEWPUNCHLIST_REMOTE.GUID, _daPunchlistStatusLocal, _daPunchlistStatusRemote, _daPunchlistIssueLocal, _daPunchlistIssueRemote);
                        }
                    }
                    //remote punchlist found by GUID
                    else
                    {
                        //validate created date
                        if (drPUNCHLIST_LOCAL.CREATED < drFINDPUNCHLIST_REMOTE.CREATED 
                            || (drPUNCHLIST_LOCAL.IsUPDATEDNull() && !drFINDPUNCHLIST_REMOTE.IsUPDATEDNull()) 
                            || ((!drPUNCHLIST_LOCAL.IsUPDATEDNull() && !drFINDPUNCHLIST_REMOTE.IsUPDATEDNull()) && drPUNCHLIST_LOCAL.UPDATED < drFINDPUNCHLIST_REMOTE.UPDATED))
                        {
                            drPUNCHLIST_LOCAL.ItemArray = drFINDPUNCHLIST_REMOTE.ItemArray;
                            dsTAG.TAGRow drTAG_LOCAL = _daTagLocal.GetBy(drPUNCHLIST_LOCAL.TAG_GUID); //Tag won't be null because it's foreign key constrained
                            if (drFINDPUNCHLIST_REMOTE.ITR_GUID == Guid.Empty)
                            {
                                UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_LOCAL.NUMBER, drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM, "Updated | Ad-Hoc", SyncStatusDisplay.download);
                            }
                            else
                            {
                                bool isDeleted = false;
                                Guid? toITR_GUID = QueryPunchlistITRGuid(drFINDPUNCHLIST_REMOTE.ITR_GUID, drFINDPUNCHLIST_REMOTE.TAG_GUID, _daITRRemote, _daITRLocal, out isDeleted);
                                if (toITR_GUID != null)
                                {
                                    if (toITR_GUID == drPUNCHLIST_LOCAL.ITR_GUID)
                                        UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_LOCAL.NUMBER, drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM, "Updated | ITR Known", SyncStatusDisplay.download);
                                    else
                                    {
                                        drPUNCHLIST_LOCAL.ITR_GUID = (Guid)toITR_GUID;
                                        UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_LOCAL.NUMBER, drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM, "Updated | ITR Adjusted", SyncStatusDisplay.download);
                                    }
                                }
                            }

                            _daPunchlistLocal.Save(drPUNCHLIST_LOCAL);
                            CopyPunchlistStatusAndIssue(drFINDPUNCHLIST_REMOTE.GUID, drPUNCHLIST_LOCAL.GUID, _daPunchlistStatusRemote, _daPunchlistStatusLocal, _daPunchlistIssueRemote, _daPunchlistIssueLocal);
                        }
                        else if (drPUNCHLIST_LOCAL.CREATED > drFINDPUNCHLIST_REMOTE.CREATED
                            || (!drPUNCHLIST_LOCAL.IsUPDATEDNull() && drFINDPUNCHLIST_REMOTE.IsUPDATEDNull())
                            || ((!drPUNCHLIST_LOCAL.IsUPDATEDNull() && !drFINDPUNCHLIST_REMOTE.IsUPDATEDNull()) && drPUNCHLIST_LOCAL.UPDATED > drFINDPUNCHLIST_REMOTE.UPDATED))
                        {
                            drFINDPUNCHLIST_REMOTE.ItemArray = drPUNCHLIST_LOCAL.ItemArray;
                            dsTAG.TAGRow drTAG_REMOTE = _daTagRemote.GetBy(drFINDPUNCHLIST_REMOTE.TAG_GUID);
                            if (drPUNCHLIST_LOCAL.ITR_GUID == Guid.Empty)
                            {
                                UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_REMOTE.NUMBER, drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM, "Updated | Ad-Hoc", SyncStatusDisplay.upload);
                            }
                            else
                            {
                                bool isDeleted = false;
                                Guid? toITR_GUID = QueryPunchlistITRGuid(drPUNCHLIST_LOCAL.ITR_GUID, drPUNCHLIST_LOCAL.TAG_GUID, _daITRLocal, _daITRRemote, out isDeleted);
                                if (toITR_GUID != null)
                                {
                                    if (toITR_GUID == drPUNCHLIST_LOCAL.ITR_GUID)
                                        UpdateSyncItem(drFINDPUNCHLIST_REMOTE.GUID, drTAG_REMOTE.NUMBER, drFINDPUNCHLIST_REMOTE.ITR_PUNCHLIST_ITEM, "Updated | ITR Known", SyncStatusDisplay.upload);
                                    else
                                    {
                                        drFINDPUNCHLIST_REMOTE.ITR_GUID = (Guid)toITR_GUID;
                                        UpdateSyncItem(drFINDPUNCHLIST_REMOTE.GUID, drTAG_REMOTE.NUMBER, drFINDPUNCHLIST_REMOTE.ITR_PUNCHLIST_ITEM, "Updated | ITR Adjusted", SyncStatusDisplay.upload);
                                    }
                                }

                                _daPunchlistRemote.Save(drFINDPUNCHLIST_REMOTE);
                                CopyPunchlistStatusAndIssue(drPUNCHLIST_LOCAL.GUID, drFINDPUNCHLIST_REMOTE.GUID, _daPunchlistStatusLocal, _daPunchlistStatusRemote, _daPunchlistIssueLocal, _daPunchlistIssueRemote);
                            }
                        }
                        else
                        {
                            bool isDeleted = false;
                            //check whether server and local knowns about this ITR
                            Guid? toITR_GUID = QueryPunchlistITRGuid(drFINDPUNCHLIST_REMOTE.ITR_GUID, drFINDPUNCHLIST_REMOTE.TAG_GUID, _daITRRemote, _daITRLocal, out isDeleted);
                            Guid? fromITR_GUID = QueryPunchlistITRGuid(drPUNCHLIST_LOCAL.ITR_GUID, drPUNCHLIST_LOCAL.TAG_GUID, _daITRLocal, _daITRRemote, out isDeleted);

                            dsTAG.TAGRow drTAG_REMOTE = _daTagRemote.GetIncludeDeletedBy(drFINDPUNCHLIST_REMOTE.TAG_GUID);
                            if(isDeleted)
                            {
                                int deleteCount = 0;
                                if(drPUNCHLIST_LOCAL.IsDELETEDNull())
                                {
                                    drPUNCHLIST_LOCAL.DELETED = DateTime.Now;
                                    drPUNCHLIST_LOCAL.DELETEDBY = Guid.Empty;
                                    deleteCount += 1;
                                    _daPunchlistLocal.Save(drPUNCHLIST_LOCAL);
                                    UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_REMOTE.NUMBER, drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM, "Delete Locally", SyncStatusDisplay.download);
                                }

                                if(drFINDPUNCHLIST_REMOTE.IsDELETEDNull())
                                {
                                    drFINDPUNCHLIST_REMOTE.DELETED = DateTime.Now;
                                    drFINDPUNCHLIST_REMOTE.DELETEDBY = Guid.Empty;
                                    deleteCount += 1;
                                    _daPunchlistRemote.Save(drFINDPUNCHLIST_REMOTE);
                                    UpdateSyncItem(drFINDPUNCHLIST_REMOTE.GUID, drTAG_REMOTE.NUMBER, drFINDPUNCHLIST_REMOTE.ITR_PUNCHLIST_ITEM, "Delete Remotely", SyncStatusDisplay.upload);
                                }

                                if(!drFINDPUNCHLIST_REMOTE.IsDELETEDNull() && !drPUNCHLIST_LOCAL.IsDELETEDNull())
                                    UpdateSyncItem(drFINDPUNCHLIST_REMOTE.GUID, drTAG_REMOTE.NUMBER, drFINDPUNCHLIST_REMOTE.ITR_PUNCHLIST_ITEM, "No Change", SyncStatusDisplay.same);
                                
                                if(deleteCount == 2)
                                    UpdateSyncItem(drFINDPUNCHLIST_REMOTE.GUID, drTAG_REMOTE.NUMBER, drFINDPUNCHLIST_REMOTE.ITR_PUNCHLIST_ITEM, "Delete Both", SyncStatusDisplay.same);

                            }
                            else if(!drPUNCHLIST_LOCAL.IsDELETEDNull() && drFINDPUNCHLIST_REMOTE.IsDELETEDNull())
                            {
                                drFINDPUNCHLIST_REMOTE.DELETED = drPUNCHLIST_LOCAL.DELETED;
                                drFINDPUNCHLIST_REMOTE.DELETEDBY = drPUNCHLIST_LOCAL.DELETEDBY;
                                UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_REMOTE.NUMBER, drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM, "Delete", SyncStatusDisplay.upload);

                                _daPunchlistRemote.Save(drFINDPUNCHLIST_REMOTE);
                            }
                            else if(!drFINDPUNCHLIST_REMOTE.IsDELETEDNull() && drPUNCHLIST_LOCAL.IsDELETEDNull())
                            {
                                drPUNCHLIST_LOCAL.DELETED = drFINDPUNCHLIST_REMOTE.DELETED;
                                drPUNCHLIST_LOCAL.DELETEDBY = drFINDPUNCHLIST_REMOTE.DELETEDBY;
                                UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_REMOTE.NUMBER, drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM, "Delete", SyncStatusDisplay.download);

                                _daPunchlistLocal.Save(drPUNCHLIST_LOCAL);
                            }
                            else if (toITR_GUID != null && fromITR_GUID == null)
                            {
                                drPUNCHLIST_LOCAL.ITR_GUID = (Guid)toITR_GUID;
                                UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_REMOTE.NUMBER, drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM, "Update Only ITR Guid", SyncStatusDisplay.download);
                                _daPunchlistLocal.Save(drPUNCHLIST_LOCAL);
                            }
                            else if (fromITR_GUID != null && toITR_GUID == null)
                            {
                                drFINDPUNCHLIST_REMOTE.ITR_GUID = (Guid)fromITR_GUID;
                                UpdateSyncItem(drFINDPUNCHLIST_REMOTE.GUID, drTAG_REMOTE.NUMBER, drFINDPUNCHLIST_REMOTE.ITR_PUNCHLIST_ITEM, "Update Only ITR Guid", SyncStatusDisplay.upload);
                                _daPunchlistRemote.Save(drFINDPUNCHLIST_REMOTE);
                            }
                            else if (fromITR_GUID != null && toITR_GUID != null)
                            {
                                UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_REMOTE.NUMBER, drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM, "ITR Guid Matches Both Ways", SyncStatusDisplay.same);
                            }
                            else if (fromITR_GUID == null && toITR_GUID == null)
                            {
                                UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_REMOTE.NUMBER, drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM, "ITR Guid Not Found", SyncStatusDisplay.warning);
                            }
                            else
                                UpdateSyncItem(drPUNCHLIST_LOCAL.GUID, drTAG_REMOTE.NUMBER, drPUNCHLIST_LOCAL.ITR_PUNCHLIST_ITEM, "No Change", SyncStatusDisplay.same);

                        }
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();

                //check for any remote punchlist to add
                if (dtPUNCHLIST_REMOTE != null)
                {
                    splashScreenManager1.ShowWaitForm();
                    splashScreenManager1.SetWaitFormCaption("Downloading Punchlist ... ");
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtPUNCHLIST_REMOTE.Rows.Count);

                    foreach (dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPUNCHLIST_REMOTE in dtPUNCHLIST_REMOTE.Rows)
                    {
                        dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drFINDPUNCHLIST_LOCAL = dtPUNCHLIST_LOCAL.FirstOrDefault(obj => obj.GUID == drPUNCHLIST_REMOTE.GUID);

                        //local punchlist not found - Add
                        if (drFINDPUNCHLIST_LOCAL == null)
                        {
                            _SyncItems.Add(new SyncStatus_Superseded(drPUNCHLIST_REMOTE.GUID, projectGuid)
                            {
                                additionalInfo = string.Empty,
                                syncItemName = string.Empty,
                                type = Sync_Type_Superseded.Punchlist.ToString(),
                                SyncDate = DateTime.Now,
                                SyncBy = System_Environment.GetUser().GUID,
                            });

                            //validate the tag number
                            dsTAG.TAGRow drTAG_LOCAL = _daTagRemote.GetBy(drPUNCHLIST_REMOTE.TAG_GUID);

                            string RemotePunchlist_ItemName = drPUNCHLIST_REMOTE.ITR_PUNCHLIST_ITEM == string.Empty ? "Ad-Hoc" : drPUNCHLIST_REMOTE.ITR_PUNCHLIST_ITEM;

                            //tag not found
                            if (drTAG_LOCAL == null)
                            {
                                dsTAG.TAGRow drTAG_REMOTE = _daTagRemote.GetBy(drPUNCHLIST_REMOTE.TAG_GUID);
                                if (drTAG_REMOTE != null)
                                    UpdateSyncItem(drPUNCHLIST_REMOTE.GUID, drTAG_LOCAL.NUMBER, RemotePunchlist_ItemName, "Remote Tag Not Found", SyncStatusDisplay.warning);
                                else //unlikely to happen because of foreign key constraint
                                    UpdateSyncItem(drPUNCHLIST_REMOTE.GUID, drTAG_LOCAL.NUMBER, RemotePunchlist_ItemName, "Local and Remote Tag Not Found", SyncStatusDisplay.warning);
                            }
                            //tag is found
                            else
                            {
                                dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drNEWPUNCHLIST_LOCAL = dsPUNCHLIST.PUNCHLIST_MAIN.NewPUNCHLIST_MAINRow();
                                drNEWPUNCHLIST_LOCAL.ItemArray = drPUNCHLIST_REMOTE.ItemArray;

                                //dont have to validate ITR number if it's ad-hoc
                                if (drPUNCHLIST_REMOTE.ITR_GUID == Guid.Empty)
                                {
                                    UpdateSyncItem(drPUNCHLIST_REMOTE.GUID, drTAG_LOCAL.NUMBER, RemotePunchlist_ItemName, "Created | Ad-Hoc", SyncStatusDisplay.download);
                                }
                                //adjust the ITR GUID if it's not ad-hoc
                                else
                                {
                                    bool isITRDeleted = false;
                                    Guid? toITR_GUID = QueryPunchlistITRGuid(drPUNCHLIST_REMOTE.ITR_GUID, drPUNCHLIST_REMOTE.TAG_GUID, _daITRRemote, _daITRLocal, out isITRDeleted);
                                    //validate the ITR number
                                    if (toITR_GUID != null)
                                    {
                                        drNEWPUNCHLIST_LOCAL.ITR_GUID = (Guid)toITR_GUID;
                                        if(isITRDeleted)
                                            UpdateSyncItem(drPUNCHLIST_REMOTE.GUID, drTAG_LOCAL.NUMBER, RemotePunchlist_ItemName, "Created as Deleted | ITR Adjusted", SyncStatusDisplay.download);
                                        else
                                            UpdateSyncItem(drPUNCHLIST_REMOTE.GUID, drTAG_LOCAL.NUMBER, RemotePunchlist_ItemName, "Created | ITR Adjusted", SyncStatusDisplay.download);
                                    }
                                    else
                                    {
                                        if (isITRDeleted)
                                            UpdateSyncItem(drPUNCHLIST_REMOTE.GUID, drTAG_LOCAL.NUMBER, RemotePunchlist_ItemName, "Created as Deleted | ITR Not Adjusted", SyncStatusDisplay.download);
                                        else
                                            UpdateSyncItem(drPUNCHLIST_REMOTE.GUID, drTAG_LOCAL.NUMBER, RemotePunchlist_ItemName, "Created | ITR Not Adjusted", SyncStatusDisplay.download);
                                    }
                                }

                                dsPUNCHLIST.PUNCHLIST_MAIN.AddPUNCHLIST_MAINRow(drNEWPUNCHLIST_LOCAL);
                                _daPunchlistLocal.Save(drNEWPUNCHLIST_LOCAL);
                                CopyPunchlistStatusAndIssue(drPUNCHLIST_REMOTE.GUID, drNEWPUNCHLIST_LOCAL.GUID, _daPunchlistStatusRemote, _daPunchlistStatusLocal, _daPunchlistIssueRemote, _daPunchlistIssueLocal);
                            }
                        }
                        
                        splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                    }
                    
                    splashScreenManager1.CloseWaitForm();
                }
            }
        }

        ///// <summary>
        ///// Perform the Punchlist Sync
        ///// </summary>
        ////Changes 7-May-2015 : Sync punchlist by entire project
        ////private void Sync_Punchlist(Guid projectGuid, string discipline)
        //private void Sync_Punchlist(Guid projectGuid)
        //{
        //    dsPUNCHLIST_MAIN dsPunchlist = new dsPUNCHLIST_MAIN();

        //    //List<SyncPunchlist> Punchlists = EstablishSyncPunchlist(projectGuid, discipline);
        //    List<SyncPunchlist> Punchlists = EstablishSyncPunchlist(projectGuid);
        //    splashScreenManager1.ShowWaitForm();
        //    splashScreenManager1.SetWaitFormCaption("Syncing Punchlists ... ");
        //    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetLargeFormat, 500);
        //    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
        //    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, Punchlists.Count);

        //    foreach (SyncPunchlist punchlist in Punchlists)
        //    {
        //        int localPunchlistScore = RateLocalPunchlist(punchlist);
        //        int remotePunchlistScore = RateRemotePunchlist(punchlist);

        //        Guid SyncTagWBSGuid = Guid.NewGuid();
        //        _SyncItems.Add(new SyncStatus(SyncTagWBSGuid, projectGuid)
        //            {
        //                additionalInfo = punchlist.attachmentName,
        //                syncItemName = punchlist.punchlistTitle,
        //                type = Sync_Type.Punchlist.ToString(),
        //                SyncDate = DateTime.Now,
        //                SyncBy = System_Environment.GetUser().GUID,
        //            });

        //        dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlistLocal;
        //        dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlistRemote;

        //        if (punchlist.isWBS)
        //        {
        //            drPunchlistLocal = _daPunchlistLocal.GetByWBSNameProject(punchlist.attachmentName, punchlist.punchlistTitle, punchlist.projectGuid);
        //            drPunchlistRemote = _daPunchlistRemote.GetByWBSNameProject(punchlist.attachmentName, punchlist.punchlistTitle, punchlist.projectGuid);
        //        }
        //        else //attach WBS is not null
        //        {
        //            drPunchlistLocal = _daPunchlistLocal.GetByTagTitleProject(punchlist.attachmentName, punchlist.punchlistTitle, punchlist.projectGuid);
        //            drPunchlistRemote = _daPunchlistRemote.GetByTagTitleProject(punchlist.attachmentName, punchlist.punchlistTitle, punchlist.projectGuid);
        //        }

        //        if (localPunchlistScore != remotePunchlistScore)
        //        {
        //            dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlistNew = dsPunchlist.PUNCHLIST_MAIN.NewPUNCHLIST_MAINRow();
        //            Guid attachmentGuid = Guid.Empty;

        //            //when either is null
        //            if (drPunchlistLocal == null)
        //            {
        //                if (!drPunchlistRemote.IsTAG_GUIDNull())
        //                    attachmentGuid = isTagWBSExist(_daTagLocal, null, punchlist.attachmentName, projectGuid);
        //                else if (!drPunchlistRemote.IsWBS_GUIDNull())
        //                    attachmentGuid = isTagWBSExist(null, _daWBSLocal, punchlist.attachmentName, projectGuid);

        //                if(attachmentGuid != Guid.Empty)
        //                {
        //                    drPunchlistNew.ItemArray = drPunchlistRemote.ItemArray;
        //                    Guid? toITR_GUID = QueryPunchlistITRGuid(drPunchlistRemote.ITR_GUID, drPunchlistRemote.TAG_GUID, _daITRRemote, _daITRLocal);
        //                    if (toITR_GUID != null)
        //                        drPunchlistNew.ITR_GUID = (Guid)toITR_GUID;

        //                    dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlistLocalAll = _daPunchlistLocal.GetAllPunchlist(drPunchlistNew.GUID);
        //                    if(drPunchlistLocalAll == null)
        //                    {
        //                        //WBSTag guid correction
        //                        if (!drPunchlistRemote.IsTAG_GUIDNull())
        //                            drPunchlistNew.TAG_GUID = attachmentGuid;
        //                        else
        //                            drPunchlistNew.WBS_GUID = attachmentGuid;

        //                        dsPunchlist.PUNCHLIST_MAIN.AddPUNCHLIST_MAINRow(drPunchlistNew);
        //                        _daPunchlistLocal.Save(drPunchlistNew);
        //                        CopyPunchlistStatusAndIssue(drPunchlistRemote.GUID, drPunchlistNew.GUID, _daPunchlistStatusRemote, _daPunchlistStatusLocal, _daPunchlistIssueRemote, _daPunchlistIssueLocal);  //Changes 15th Dec 2014 full GUID sync with master
        //                        UpdateSyncDate(SyncTagWBSGuid, drPunchlistNew.CREATED, drPunchlistNew.CREATEDBY, drPunchlistNew.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistNew.UPDATED, drPunchlistNew.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistNew.UPDATEDBY, drPunchlistNew.IsDELETEDNull() ? DateTime.MinValue : drPunchlistNew.DELETED, drPunchlistNew.IsDELETEDBYNull() ? Guid.Empty : drPunchlistNew.DELETEDBY);
        //                        UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Created", SyncStatusDisplay.download);
        //                    }
        //                    else if (!drPunchlistLocalAll.IsDELETEDNull())
        //                    {
        //                        //Change 12-05-2015: Delete remote punchlist if it was deleted locally
        //                        drPunchlistRemote.DELETED = drPunchlistLocalAll.DELETED;
        //                        drPunchlistRemote.DELETEDBY = drPunchlistLocalAll.DELETEDBY;
        //                        _daPunchlistRemote.Save(drPunchlistLocal);
        //                        UpdateSyncDate(SyncTagWBSGuid, drPunchlistLocalAll.CREATED, drPunchlistLocalAll.CREATEDBY, drPunchlistLocalAll.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistLocalAll.UPDATED, drPunchlistLocalAll.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistLocalAll.UPDATEDBY, drPunchlistLocalAll.IsDELETEDNull() ? DateTime.MinValue : drPunchlistLocalAll.DELETED, drPunchlistLocalAll.IsDELETEDBYNull() ? Guid.Empty : drPunchlistLocalAll.DELETEDBY);
        //                        UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Deleted", SyncStatusDisplay.upload);
        //                    }
        //                    else if (drPunchlistLocalAll.TITLE != drPunchlistRemote.TITLE)
        //                    {
        //                        UpdateSyncDate(SyncTagWBSGuid, drPunchlistLocalAll.CREATED, drPunchlistLocalAll.CREATEDBY, drPunchlistLocalAll.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistLocalAll.UPDATED, drPunchlistLocalAll.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistLocalAll.UPDATEDBY, drPunchlistLocalAll.IsDELETEDNull() ? DateTime.MinValue : drPunchlistLocalAll.DELETED, drPunchlistLocalAll.IsDELETEDBYNull() ? Guid.Empty : drPunchlistLocalAll.DELETEDBY);
        //                        UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Conflicting Title", SyncStatusDisplay.warning);
        //                    }
        //                    else if (drPunchlistLocalAll.REMEDIAL != drPunchlistRemote.REMEDIAL)
        //                    {
        //                        UpdateSyncDate(SyncTagWBSGuid, drPunchlistLocalAll.CREATED, drPunchlistLocalAll.CREATEDBY, drPunchlistLocalAll.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistLocalAll.UPDATED, drPunchlistLocalAll.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistLocalAll.UPDATEDBY, drPunchlistLocalAll.IsDELETEDNull() ? DateTime.MinValue : drPunchlistLocalAll.DELETED, drPunchlistLocalAll.IsDELETEDBYNull() ? Guid.Empty : drPunchlistLocalAll.DELETEDBY);
        //                        UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Conflicting Inspection", SyncStatusDisplay.warning);
        //                    }
        //                }
        //                else
        //                {
        //                    UpdateSyncDate(SyncTagWBSGuid, drPunchlistLocal.CREATED, drPunchlistLocal.CREATEDBY, drPunchlistLocal.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistLocal.UPDATED, drPunchlistLocal.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistLocal.UPDATEDBY, drPunchlistLocal.IsDELETEDNull() ? DateTime.MinValue : drPunchlistLocal.DELETED, drPunchlistLocal.IsDELETEDBYNull() ? Guid.Empty : drPunchlistLocal.DELETEDBY);
        //                    UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Attachment Not Found", SyncStatusDisplay.same);
        //                }
        //            }
        //            else if (drPunchlistRemote == null)
        //            {
        //                if (!drPunchlistLocal.IsTAG_GUIDNull())
        //                    attachmentGuid = isTagWBSExist(_daTagRemote, null, punchlist.attachmentName, projectGuid);
        //                else if (!drPunchlistLocal.IsWBS_GUIDNull())
        //                    attachmentGuid = isTagWBSExist(null, _daWBSRemote, punchlist.attachmentName, projectGuid);

        //                if(attachmentGuid != Guid.Empty)
        //                {
        //                    drPunchlistNew.ItemArray = drPunchlistLocal.ItemArray;
        //                    Guid? toITR_GUID = QueryPunchlistITRGuid(drPunchlistLocal.ITR_GUID, drPunchlistLocal.TAG_GUID, _daITRLocal, _daITRRemote);
        //                    if (toITR_GUID != null)
        //                        drPunchlistNew.ITR_GUID = (Guid)toITR_GUID;

        //                    dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlistRemoteAll = _daPunchlistRemote.GetAllPunchlist(drPunchlistNew.GUID);
        //                    if (drPunchlistRemoteAll == null)
        //                    {
        //                        //WBSTag guid correction
        //                        if (!drPunchlistLocal.IsTAG_GUIDNull())
        //                            drPunchlistNew.TAG_GUID = attachmentGuid;
        //                        else
        //                            drPunchlistNew.WBS_GUID = attachmentGuid;

        //                        dsPunchlist.PUNCHLIST_MAIN.AddPUNCHLIST_MAINRow(drPunchlistNew);
        //                        _daPunchlistRemote.Save(drPunchlistNew);
        //                        CopyPunchlistStatusAndIssue(drPunchlistLocal.GUID, drPunchlistNew.GUID, _daPunchlistStatusLocal, _daPunchlistStatusRemote, _daPunchlistIssueLocal, _daPunchlistIssueRemote);
        //                        UpdateSyncDate(SyncTagWBSGuid, drPunchlistLocal.CREATED, drPunchlistLocal.CREATEDBY, drPunchlistLocal.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistLocal.UPDATED, drPunchlistLocal.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistLocal.UPDATEDBY, drPunchlistLocal.IsDELETEDNull() ? DateTime.MinValue : drPunchlistLocal.DELETED, drPunchlistLocal.IsDELETEDBYNull() ? Guid.Empty : drPunchlistLocal.DELETEDBY);
        //                        UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Created", SyncStatusDisplay.upload);
        //                    }
        //                    else if (!drPunchlistRemoteAll.IsDELETEDNull())
        //                    {
        //                        //Change 12-05-2015: Delete local punchlist if it was deleted remotely
        //                        drPunchlistLocal.DELETED = drPunchlistRemoteAll.DELETED;
        //                        drPunchlistLocal.DELETEDBY = drPunchlistRemoteAll.DELETEDBY;
        //                        _daPunchlistLocal.Save(drPunchlistLocal);
        //                        UpdateSyncDate(SyncTagWBSGuid, drPunchlistRemoteAll.CREATED, drPunchlistRemoteAll.CREATEDBY, drPunchlistRemoteAll.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistRemoteAll.UPDATED, drPunchlistRemoteAll.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistRemoteAll.UPDATEDBY, drPunchlistRemoteAll.IsDELETEDNull() ? DateTime.MinValue : drPunchlistRemoteAll.DELETED, drPunchlistRemoteAll.IsDELETEDBYNull() ? Guid.Empty : drPunchlistRemoteAll.DELETEDBY);
        //                        UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Deleted", SyncStatusDisplay.download);
        //                    }
        //                    else if (drPunchlistRemoteAll.TITLE != drPunchlistLocal.TITLE)
        //                    {
        //                        UpdateSyncDate(SyncTagWBSGuid, drPunchlistRemoteAll.CREATED, drPunchlistRemoteAll.CREATEDBY, drPunchlistRemoteAll.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistRemoteAll.UPDATED, drPunchlistRemoteAll.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistRemoteAll.UPDATEDBY, drPunchlistRemoteAll.IsDELETEDNull() ? DateTime.MinValue : drPunchlistRemoteAll.DELETED, drPunchlistRemoteAll.IsDELETEDBYNull() ? Guid.Empty : drPunchlistRemoteAll.DELETEDBY);
        //                        UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Conflicting Title", SyncStatusDisplay.warning);
        //                    }
        //                    else if (drPunchlistRemoteAll.REMEDIAL != drPunchlistLocal.REMEDIAL)
        //                    {
        //                        UpdateSyncDate(SyncTagWBSGuid, drPunchlistRemoteAll.CREATED, drPunchlistRemoteAll.CREATEDBY, drPunchlistRemoteAll.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistRemoteAll.UPDATED, drPunchlistRemoteAll.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistRemoteAll.UPDATEDBY, drPunchlistRemoteAll.IsDELETEDNull() ? DateTime.MinValue : drPunchlistRemoteAll.DELETED, drPunchlistRemoteAll.IsDELETEDBYNull() ? Guid.Empty : drPunchlistRemoteAll.DELETEDBY);
        //                        UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Conflicting Inspection", SyncStatusDisplay.warning);
        //                    }
        //                }
        //                else
        //                {
        //                    UpdateSyncDate(SyncTagWBSGuid, drPunchlistLocal.CREATED, drPunchlistLocal.CREATEDBY, drPunchlistLocal.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistLocal.UPDATED, drPunchlistLocal.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistLocal.UPDATEDBY, drPunchlistLocal.IsDELETEDNull() ? DateTime.MinValue : drPunchlistLocal.DELETED, drPunchlistLocal.IsDELETEDBYNull() ? Guid.Empty : drPunchlistLocal.DELETEDBY);
        //                    UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Attachment Not Found", SyncStatusDisplay.same);  
        //                }
        //            }
        //            //don't need to validate both is null from here one because they would've had the same score
        //            //when both is not null
        //            else
        //            {
        //                #region New Rev Support
        //                //if (drPunchlistLocal.REMEDIAL == drPunchlistRemote.REMEDIAL) //copy status only if same action
        //                //{ 
        //                #endregion
        //                    if (localPunchlistScore > remotePunchlistScore)
        //                    {
        //                        Guid remotePunchlistGuid = drPunchlistRemote.GUID;
        //                        drPunchlistRemote.ItemArray = drPunchlistLocal.ItemArray; //rectify the GUID upon status update for conflicting punchlist
        //                        Guid? toITR_GUID = QueryPunchlistITRGuid(drPunchlistLocal.ITR_GUID, drPunchlistLocal.TAG_GUID, _daITRLocal, _daITRRemote);
        //                        if (toITR_GUID != null)
        //                            drPunchlistRemote.ITR_GUID = (Guid)toITR_GUID;

        //                        _daPunchlistRemote.Save(drPunchlistRemote);
        //                        //CopyPunchlistStatusAndIssue(drPunchlistLocal.GUID, drPunchlistRemote.GUID, _daPunchlistStatusLocal, _daPunchlistStatusRemote, _daPunchlistIssueLocal, _daPunchlistIssueRemote);  //Changes 15th Dec 2014 full GUID sync with master
        //                        CopyPunchlistStatusAndIssue(drPunchlistLocal.GUID, remotePunchlistGuid, _daPunchlistStatusLocal, _daPunchlistStatusRemote, _daPunchlistIssueLocal, _daPunchlistIssueRemote);  //Changes 15th Dec 2014 full GUID sync with master
        //                        UpdateSyncDate(SyncTagWBSGuid, drPunchlistLocal.CREATED, drPunchlistLocal.CREATEDBY, drPunchlistLocal.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistLocal.UPDATED, drPunchlistLocal.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistLocal.UPDATEDBY, drPunchlistLocal.IsDELETEDNull() ? DateTime.MinValue : drPunchlistLocal.DELETED, drPunchlistLocal.IsDELETEDBYNull() ? Guid.Empty : drPunchlistLocal.DELETEDBY);
        //                        UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Status Update", SyncStatusDisplay.upload);
        //                    }
        //                    else if (remotePunchlistScore > localPunchlistScore)
        //                    {
        //                        Guid localPunchlistGuid = drPunchlistLocal.GUID;
        //                        drPunchlistLocal.ItemArray = drPunchlistRemote.ItemArray;  //rectify the GUID upon status update for conflicting punchlist
        //                        Guid? toITR_GUID = QueryPunchlistITRGuid(drPunchlistRemote.ITR_GUID, drPunchlistRemote.TAG_GUID, _daITRRemote, _daITRLocal);
        //                        if (toITR_GUID != null)
        //                            drPunchlistLocal.ITR_GUID = (Guid)toITR_GUID;

        //                        _daPunchlistLocal.Save(drPunchlistLocal);
        //                        //CopyPunchlistStatusAndIssue(drPunchlistRemote.GUID, drPunchlistLocal.GUID, _daPunchlistStatusRemote, _daPunchlistStatusLocal, _daPunchlistIssueRemote, _daPunchlistIssueLocal); //Changes 15th Dec 2014 full GUID sync with master
        //                        CopyPunchlistStatusAndIssue(drPunchlistRemote.GUID, localPunchlistGuid, _daPunchlistStatusRemote, _daPunchlistStatusLocal, _daPunchlistIssueRemote, _daPunchlistIssueLocal); //Changes 15th Dec 2014 full GUID sync with master
        //                        UpdateSyncDate(SyncTagWBSGuid, drPunchlistRemote.CREATED, drPunchlistRemote.CREATEDBY, drPunchlistRemote.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistRemote.UPDATED, drPunchlistRemote.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistRemote.UPDATEDBY, drPunchlistRemote.IsDELETEDNull() ? DateTime.MinValue : drPunchlistRemote.DELETED, drPunchlistRemote.IsDELETEDBYNull() ? Guid.Empty : drPunchlistRemote.DELETEDBY);
        //                        UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Status Update", SyncStatusDisplay.download);
        //                    }
        //                    //already did checking on same score earlier
        //                    //else
        //                    //{
        //                    //    //already validate for both not null
        //                    //    UpdateSyncDate(SyncTagWBSGuid, drPunchlistLocal.CREATED, drPunchlistLocal.CREATEDBY, drPunchlistLocal.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistLocal.UPDATED, drPunchlistLocal.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistLocal.UPDATEDBY, drPunchlistLocal.IsDELETEDNull() ? DateTime.MinValue : drPunchlistLocal.DELETED, drPunchlistLocal.IsDELETEDBYNull() ? Guid.Empty : drPunchlistLocal.DELETEDBY);

        //                    //    if (drPunchlistLocal.ItemArray.Equals(drPunchlistRemote.ItemArray))
        //                    //    {
        //                    //        UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Conflicting Punchlist", SyncStatusDisplay.warning);
        //                    //    }
        //                    //    else
        //                    //    {
        //                    //        UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
        //                    //    }
        //                    //}
        //                #region New Rev Support
        //                //}
        //                //else //create new sequence if different action
        //                //{
        //                //    dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drNewPunchlist = dsPunchlist.PUNCHLIST_MAIN.NewPUNCHLIST_MAINRow();
        //                //    if (localPunchlistScore > remotePunchlistScore)
        //                //    {
        //                //        UpdateSyncItem(SyncTagWBSGuid, drPunchlistLocal.TITLE, "Uploading (New Rev)", 10);
        //                //        drNewPunchlist.ItemArray = drPunchlistLocal.ItemArray;
        //                //        drNewPunchlist.GUID = Guid.NewGuid();
        //                //        drNewPunchlist.SEQUENCE_NUMBER = drPunchlistRemote.SEQUENCE_NUMBER + 1;
        //                //        dsPunchlist.PUNCHLIST_MAIN.AddPUNCHLIST_MAINRow(drNewPunchlist);
        //                //        _daPunchlistRemote.RemoveBy(drPunchlistRemote.GUID);
        //                //        _daPunchlistRemote.Save(drNewPunchlist);
        //                //        UpdateSyncItem(SyncTagWBSGuid, string.Empty, "Uploading Status", 50);
        //                //        CopyPunchlistStatusAndIssue(drPunchlistLocal.GUID, drNewPunchlist.GUID, _daPunchlistStatusLocal, _daPunchlistStatusRemote, _daPunchlistIssueLocal, _daPunchlistIssueRemote, SyncTagWBSGuid);
        //                //        UpdateSyncItem(SyncTagWBSGuid, string.Empty, "Uploaded (New Rev)", 100);
        //                //    }
        //                //    else if (remotePunchlistScore > localPunchlistScore)
        //                //    {
        //                //        UpdateSyncItem(SyncTagWBSGuid, drPunchlistLocal.TITLE, "Downloading (New Rev)", 10);
        //                //        drNewPunchlist.ItemArray = drPunchlistRemote.ItemArray;
        //                //        drNewPunchlist.GUID = Guid.NewGuid();
        //                //        drNewPunchlist.SEQUENCE_NUMBER = drPunchlistLocal.SEQUENCE_NUMBER + 1;
        //                //        dsPunchlist.PUNCHLIST_MAIN.AddPUNCHLIST_MAINRow(drNewPunchlist);
        //                //        _daPunchlistLocal.RemoveBy(drPunchlistLocal.GUID);
        //                //        _daPunchlistLocal.Save(drNewPunchlist);
        //                //        UpdateSyncItem(SyncTagWBSGuid, drPunchlistLocal.TITLE, "Downloading Status", 50);
        //                //        CopyPunchlistStatusAndIssue(drPunchlistRemote.GUID, drNewPunchlist.GUID, _daPunchlistStatusRemote, _daPunchlistStatusLocal, _daPunchlistIssueRemote, _daPunchlistIssueLocal, SyncTagWBSGuid);
        //                //        UpdateSyncItem(SyncTagWBSGuid, drPunchlistLocal.TITLE, "Downloaded (New Rev)", 100);
        //                //    }
        //                //} 
        //                #endregion
        //            }
        //        }
        //        else
        //        {
        //            if(drPunchlistLocal != null && drPunchlistRemote != null)
        //            {
        //                Guid localPunchlistGuid = drPunchlistLocal.GUID;
        //                //rectify the GUID upon status update for conflicting punchlist
        //                Guid? toITR_GUID = QueryPunchlistITRGuid(drPunchlistRemote.ITR_GUID, drPunchlistRemote.TAG_GUID, _daITRRemote, _daITRLocal);
        //                Guid? fromITR_GUID = QueryPunchlistITRGuid(drPunchlistLocal.ITR_GUID, drPunchlistLocal.TAG_GUID, _daITRLocal, _daITRRemote);

        //                if (toITR_GUID != null && fromITR_GUID == null)
        //                {
        //                    drPunchlistLocal.ITR_GUID = (Guid)toITR_GUID;
        //                    UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Updated Punchlist ITR Guid", SyncStatusDisplay.download);
        //                    _daPunchlistLocal.Save(drPunchlistLocal);
        //                }
        //                else if(fromITR_GUID != null && toITR_GUID == null)
        //                {
        //                    drPunchlistRemote.ITR_GUID = (Guid)fromITR_GUID;
        //                    UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Updated Punchlist ITR Guid", SyncStatusDisplay.upload);
        //                    _daPunchlistRemote.Save(drPunchlistRemote);
        //                }
        //                else if(fromITR_GUID != null && toITR_GUID != null)
        //                {
        //                    UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "ITR Guid Matches Locally", SyncStatusDisplay.same);
        //                }
        //                else if(fromITR_GUID == null && toITR_GUID == null)
        //                {
        //                    UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "ITR Guid Not Found", SyncStatusDisplay.warning);
        //                }

        //                UpdateSyncDate(SyncTagWBSGuid, drPunchlistLocal.CREATED, drPunchlistLocal.CREATEDBY, drPunchlistLocal.IsUPDATEDNull() ? DateTime.MinValue : drPunchlistLocal.UPDATED, drPunchlistLocal.IsUPDATEDBYNull() ? Guid.Empty : drPunchlistLocal.UPDATEDBY, drPunchlistLocal.IsDELETEDNull() ? DateTime.MinValue : drPunchlistLocal.DELETED, drPunchlistLocal.IsDELETEDBYNull() ? Guid.Empty : drPunchlistLocal.DELETEDBY);
                            
        //                //if (drPunchlistLocal.ItemArray.Equals(drPunchlistRemote.ItemArray))
        //                //    UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "Conflicting Punchlist", SyncStatusDisplay.warning);
        //                //else
        //                //    UpdateSyncItem(SyncTagWBSGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
        //            }
        //            //else won't happen because if both is null they would have the same score, and both not null is validated in the section above
        //        }

        //        splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
        //    }

        //    splashScreenManager1.CloseWaitForm();
        //}

        /// <summary>
        /// Ask whether remote database knows about the ITR_GUID
        /// </summary>
        private Guid? QueryPunchlistITRGuid(Guid From_ITRGUID, Guid Universal_TagGUID, AdapterITR_MAIN daITRFrom, AdapterITR_MAIN daITRTo, out bool isDeleted)
        {
            //ask whether "From" knows about the ITR number
            dsITR_MAIN.ITR_MAINRow drITR_MAIN = daITRFrom.GetIncludeDeletedBy(From_ITRGUID);
            isDeleted = false;

            //if from doesn't know about the ITR Guid, don't do any adjustment
            if (drITR_MAIN == null)
                return null;
            else if (!drITR_MAIN.IsDELETEDNull())
                isDeleted = true;

            string ITR_NAME = drITR_MAIN.NAME;
            //check whether "To" knows about the ITR GUID by Tag GUID (Should be the same for to and from) and "From" name
            dsITR_MAIN.ITR_MAINRow drITR_MAINTo = daITRTo.GetBy_WBSTAGGUID_TEMPLATENAME(Universal_TagGUID, ITR_NAME);
            if (drITR_MAINTo == null)
                return null;
            else if (!drITR_MAINTo.IsDELETEDNull())
                isDeleted = true;

            return drITR_MAINTo.GUID;
        }

                /// <summary>
        /// Ask whether remote database knows about the ITR_GUID
        /// </summary>
        private Guid? QueryPunchlistITRGuid_CheckFrom(Guid From_ITRGUID, Guid to_ITRGUID, Guid Universal_TagGUID, AdapterITR_MAIN daITRFrom, AdapterITR_MAIN daITRTo, out bool isDeleted)
        {
            isDeleted = false;
            dsITR_MAIN.ITR_MAINRow drITR_MAIN_TO = daITRTo.GetIncludeDeletedBy(to_ITRGUID);
            if (drITR_MAIN_TO != null)
            {
                if (!drITR_MAIN_TO.IsDELETEDNull())
                    isDeleted = true;

                return drITR_MAIN_TO.GUID;
            }
                
            //ask whether "From" knows about the ITR number
            dsITR_MAIN.ITR_MAINRow drITR_MAIN = daITRFrom.GetIncludeDeletedBy(From_ITRGUID);

            //if from doesn't know about the ITR Guid, don't do any adjustment
            if (drITR_MAIN == null)
                return null;
            else if (!drITR_MAIN.IsDELETEDNull())
                isDeleted = true;

            string ITR_NAME = drITR_MAIN.NAME;
            //check whether "To" knows about the ITR GUID by Tag GUID (Should be the same for to and from) and "From" name
            dsITR_MAIN.ITR_MAINRow drITR_MAINTo = daITRTo.GetBy_WBSTAGGUID_TEMPLATENAME(Universal_TagGUID, ITR_NAME);
            if (drITR_MAINTo == null)
                return null;
            else if (!drITR_MAINTo.IsDELETEDNull())
                isDeleted = true;

            return drITR_MAINTo.GUID;
        }

        private void Sync_User_New()
        {
            dsUSER_MAIN.USER_MAINDataTable dtUSER_LOCAL = _daUserLocal.GetAll();
            dsUSER_MAIN.USER_MAINDataTable dtUSER_REMOTE1 = _daUserRemote.GetAll();
            dsUSER_MAIN dsUSERMAIN = new dsUSER_MAIN();

            if(dtUSER_LOCAL != null)
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Uploading Users ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtUSER_LOCAL.Rows.Count);

                //copy local to remote
                foreach (dsUSER_MAIN.USER_MAINRow drUSER_LOCAL in dtUSER_LOCAL.Rows)
                {
                    _SyncItems.Add(new SyncStatus_Superseded(drUSER_LOCAL.GUID, drUSER_LOCAL.ROLE)
                    {
                        syncItemName = drUSER_LOCAL.FIRSTNAME + " " + drUSER_LOCAL.LASTNAME,
                        additionalInfo = drUSER_LOCAL.QANUMBER,
                        type = Sync_Type_Superseded.User.ToString(),
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drUSER_LOCAL.CREATED,
                        CreatedBy = drUSER_LOCAL.CREATEDBY,
                        UpdatedDate = drUSER_LOCAL.IsUPDATEDNull() ? DateTime.MinValue : drUSER_LOCAL.UPDATED,
                        UpdatedBy = drUSER_LOCAL.IsUPDATEDBYNull() ? Guid.Empty : drUSER_LOCAL.UPDATEDBY,
                        DeletedDate = drUSER_LOCAL.IsDELETEDNull() ? DateTime.MinValue : drUSER_LOCAL.DELETED,
                        DeletedBy = drUSER_LOCAL.IsDELETEDBYNull() ? Guid.Empty : drUSER_LOCAL.DELETEDBY
                    });

                    dsUSER_MAIN.USER_MAINRow drFIND_USER_REMOTE = dtUSER_REMOTE1.FirstOrDefault(obj => obj.GUID == drUSER_LOCAL.GUID);
                    if (drFIND_USER_REMOTE != null && !drFIND_USER_REMOTE.IsUPDATEDBYNull() && !drUSER_LOCAL.IsUPDATEDBYNull() && drFIND_USER_REMOTE.UPDATED < drUSER_LOCAL.UPDATED)
                    {
                        _daUserRemote.RemoveAllWithExclusionBy(drUSER_LOCAL.QANUMBER, drUSER_LOCAL.GUID);
                        drFIND_USER_REMOTE.ItemArray = drUSER_LOCAL.ItemArray;
                        _daUserRemote.Save(drFIND_USER_REMOTE);

                        UpdateSyncItem(drUSER_LOCAL.GUID, string.Empty, string.Empty, "Update", SyncStatusDisplay.upload);
                    }
                    else if (drFIND_USER_REMOTE == null)
                    {
                        dsUSER_MAIN.USER_MAINRow drNEW_USER_REMOTE = dsUSERMAIN.USER_MAIN.NewUSER_MAINRow();
                        drNEW_USER_REMOTE.ItemArray = drUSER_LOCAL.ItemArray;
                        dsUSERMAIN.USER_MAIN.AddUSER_MAINRow(drNEW_USER_REMOTE);
                        _daUserRemote.Save(drNEW_USER_REMOTE);

                        UpdateSyncItem(drUSER_LOCAL.GUID, string.Empty, string.Empty, "New", SyncStatusDisplay.upload);
                    }
                    else //this happens when either updated for local and remote is null
                        UpdateSyncItem(drUSER_LOCAL.GUID, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();
            }


            //refresh the remote copy
            dsUSER_MAIN.USER_MAINDataTable dtUSER_REMOTE_REFRESH = _daUserRemote.GetAll();

            if(dtUSER_REMOTE_REFRESH != null)
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Downloading Users ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtUSER_REMOTE_REFRESH.Rows.Count);

                //copy remote to local
                foreach (dsUSER_MAIN.USER_MAINRow drUSER_REMOTE in dtUSER_REMOTE_REFRESH.Rows)
                {
                    if(dtUSER_LOCAL == null)
                    {
                        dsUSER_MAIN.USER_MAINRow drNEW_USER_LOCAL = dsUSERMAIN.USER_MAIN.NewUSER_MAINRow();
                        drNEW_USER_LOCAL.ItemArray = drUSER_REMOTE.ItemArray;
                        dsUSERMAIN.USER_MAIN.AddUSER_MAINRow(drNEW_USER_LOCAL);
                        _daUserLocal.Save(drNEW_USER_LOCAL);

                        _SyncItems.Add(new SyncStatus_Superseded(drUSER_REMOTE.GUID, drUSER_REMOTE.ROLE)
                        {
                            syncItemName = drUSER_REMOTE.FIRSTNAME + " " + drUSER_REMOTE.LASTNAME,
                            additionalInfo = drUSER_REMOTE.QANUMBER,
                            type = Sync_Type_Superseded.User.ToString(),
                            SyncDate = DateTime.Now,
                            SyncBy = System_Environment.GetUser().GUID,
                            CreatedDate = drUSER_REMOTE.CREATED,
                            CreatedBy = drUSER_REMOTE.CREATEDBY,
                            UpdatedDate = drUSER_REMOTE.IsUPDATEDNull() ? DateTime.MinValue : drUSER_REMOTE.UPDATED,
                            UpdatedBy = drUSER_REMOTE.IsUPDATEDBYNull() ? Guid.Empty : drUSER_REMOTE.UPDATEDBY,
                            DeletedDate = drUSER_REMOTE.IsDELETEDNull() ? DateTime.MinValue : drUSER_REMOTE.DELETED,
                            DeletedBy = drUSER_REMOTE.IsDELETEDBYNull() ? Guid.Empty : drUSER_REMOTE.DELETEDBY
                        });

                        UpdateSyncItem(drUSER_REMOTE.GUID, string.Empty, string.Empty, "New", SyncStatusDisplay.download);
                    }
                    else
                    {
                        dsUSER_MAIN.USER_MAINRow drFIND_USER_LOCAL = dtUSER_LOCAL.FirstOrDefault(obj => obj.GUID == drUSER_REMOTE.GUID);
                        if (drFIND_USER_LOCAL != null && !drFIND_USER_LOCAL.IsUPDATEDBYNull() && !drUSER_REMOTE.IsUPDATEDBYNull() && drFIND_USER_LOCAL.UPDATED < drUSER_REMOTE.UPDATED)
                        {
                            _daUserLocal.RemoveAllWithExclusionBy(drUSER_REMOTE.QANUMBER, drUSER_REMOTE.GUID);
                            drFIND_USER_LOCAL.ItemArray = drUSER_REMOTE.ItemArray;
                            _daUserLocal.Save(drFIND_USER_LOCAL);

                            UpdateSyncItem(drUSER_REMOTE.GUID, string.Empty, string.Empty, "Update", SyncStatusDisplay.download);
                        }
                        else if (drFIND_USER_LOCAL != null && !drFIND_USER_LOCAL.IsUPDATEDBYNull() && !drUSER_REMOTE.IsUPDATEDBYNull() && drFIND_USER_LOCAL.UPDATED == drUSER_REMOTE.UPDATED)
                        {
                            UpdateSyncItem(drUSER_REMOTE.GUID, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        }
                        else if (drFIND_USER_LOCAL == null)
                        {
                            dsUSER_MAIN.USER_MAINRow drNEW_USER_LOCAL = dsUSERMAIN.USER_MAIN.NewUSER_MAINRow();
                            drNEW_USER_LOCAL.ItemArray = drUSER_REMOTE.ItemArray;
                            dsUSERMAIN.USER_MAIN.AddUSER_MAINRow(drNEW_USER_LOCAL);
                            _daUserLocal.Save(drNEW_USER_LOCAL);

                            _SyncItems.Add(new SyncStatus_Superseded(drUSER_REMOTE.GUID, drUSER_REMOTE.ROLE)
                            {
                                syncItemName = drUSER_REMOTE.FIRSTNAME + " " + drUSER_REMOTE.LASTNAME,
                                additionalInfo = drUSER_REMOTE.QANUMBER,
                                type = Sync_Type_Superseded.User.ToString(),
                                SyncDate = DateTime.Now,
                                SyncBy = System_Environment.GetUser().GUID,
                                CreatedDate = drUSER_REMOTE.CREATED,
                                CreatedBy = drUSER_REMOTE.CREATEDBY,
                                UpdatedDate = drUSER_REMOTE.IsUPDATEDNull() ? DateTime.MinValue : drUSER_REMOTE.UPDATED,
                                UpdatedBy = drUSER_REMOTE.IsUPDATEDBYNull() ? Guid.Empty : drUSER_REMOTE.UPDATEDBY,
                                DeletedDate = drUSER_REMOTE.IsDELETEDNull() ? DateTime.MinValue : drUSER_REMOTE.DELETED,
                                DeletedBy = drUSER_REMOTE.IsDELETEDBYNull() ? Guid.Empty : drUSER_REMOTE.DELETEDBY
                            });

                            UpdateSyncItem(drUSER_REMOTE.GUID, string.Empty, string.Empty, "New", SyncStatusDisplay.download);
                        }
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();
            }
        }

        /// <summary>
        /// Sync's user between databases
        /// </summary>
        private void Sync_User(Guid projectGuid, Sync_Mode syncMode, bool includeDeletes)
        {
            dsUSER_MAIN dsUserMain = new dsUSER_MAIN();
            SyncStatusDisplay syncStatus = SyncStatusDisplay.upload; //set upload as default

            if (syncMode == Sync_Mode.None)
                return;
            else
            {
                AdapterUSER_MAIN daUserFrom;
                AdapterUSER_MAIN daUserTo;

                if(syncMode == Sync_Mode.Pull) 
                {
                    daUserFrom = _daUserLocal; //when server is set to pull, it will be pulling from local db
                    daUserTo = _daUserRemote;
                }
                else
                {
                    daUserFrom = _daUserRemote;
                    daUserTo = _daUserLocal;
                    syncStatus = SyncStatusDisplay.download;
                }

                dsUSER_MAIN.USER_MAINDataTable dtUserFrom;
                if (includeDeletes)
                    dtUserFrom = daUserFrom.GetByProjectIncludeDeleted(projectGuid);
                else
                    dtUserFrom = daUserFrom.GetByProject(projectGuid);

                if (dtUserFrom == null)
                    return;

                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Syncing Users ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtUserFrom.Rows.Count);

                foreach (dsUSER_MAIN.USER_MAINRow drUserFrom in dtUserFrom.Rows)
                {
                    Guid SyncUserGuid = drUserFrom.GUID;
                    _SyncItems.Add(new SyncStatus_Superseded(SyncUserGuid, drUserFrom.ROLE)
                    {
                        syncItemName = drUserFrom.FIRSTNAME + " " + drUserFrom.LASTNAME,
                        additionalInfo = drUserFrom.QANUMBER,
                        type = Sync_Type_Superseded.User.ToString(),
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drUserFrom.CREATED,
                        CreatedBy = drUserFrom.CREATEDBY,
                        UpdatedDate = drUserFrom.IsUPDATEDNull() ? DateTime.MinValue : drUserFrom.UPDATED,
                        UpdatedBy = drUserFrom.IsUPDATEDBYNull() ? Guid.Empty : drUserFrom.UPDATEDBY,
                        DeletedDate = drUserFrom.IsDELETEDNull() ? DateTime.MinValue : drUserFrom.DELETED,
                        DeletedBy = drUserFrom.IsDELETEDBYNull() ? Guid.Empty : drUserFrom.DELETEDBY
                    });

                    //perfect match with GUID
                    dsUSER_MAIN.USER_MAINRow drUserTo = daUserTo.GetIncludeDeletedBy(drUserFrom.GUID);
                    if (drUserTo != null)
                    {
                        //to make sure this is the only user with this QA number, because itemarray copy will have the ability to undelete items
                        daUserTo.RemoveAllWithExclusionBy(drUserFrom.QANUMBER, drUserTo.GUID);

                        if (DataRowComparer.Default.Equals(drUserTo, drUserFrom))
                            UpdateSyncItem(SyncUserGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        else
                        {
                            //Fix 12-DEC-2014: Only Sync if Newer
                            DateTime userFromUpdated = drUserFrom.IsUPDATEDNull() ? DateTime.MinValue : drUserFrom.UPDATED;
                            DateTime userToUpdated = drUserTo.IsUPDATEDNull() ? DateTime.MinValue : drUserTo.UPDATED;
                            if (userFromUpdated > userToUpdated)
                            {
                                drUserTo.ItemArray = drUserFrom.ItemArray;
                                daUserTo.Save(drUserTo);

                                if (!drUserTo.IsDELETEDNull())
                                    UpdateSyncItem(SyncUserGuid, string.Empty, string.Empty, "Delete", syncStatus);
                                else
                                {
                                    //Changes 11-DEC-2014: User discipline and user authorised project won't be sync'ed on perfect GUID match
                                    //Sync_UserDiscipline(daUserFrom, daUserTo, drUserTo.GUID, drUserTo.QANUMBER, "None", syncStatus); //pass in "None" because trigger will not be involved here
                                    //Sync_UserProject(daUserFrom, daUserTo, drUserTo.GUID, drUserTo.QANUMBER, Guid.Empty, syncStatus); //pass in Guid.Empty because trigger will not be involved here
                                    UpdateSyncItem(SyncUserGuid, string.Empty, string.Empty, "Update", syncStatus);
                                }
                            }
                            else
                                UpdateSyncItem(SyncUserGuid, string.Empty, string.Empty, "No Change (Newer DB To)", syncStatus); //Fix 12-DEC-2014: Only Sync if Newer
                        }
                    }
                    //partial match with QA number
                    else
                    {
                        drUserTo = daUserTo.GetBy(drUserFrom.QANUMBER);
                        if (drUserTo != null)
                        {
                            //if guid doesn't match with QA number, this record can be categorised as conflict, let user resolve it
                            if(!drUserTo.IsDELETEDNull())
                                UpdateSyncItem(SyncUserGuid, string.Empty, string.Empty, "Conflict", SyncStatusDisplay.warning); //very unlikely to happen because user cannot change QA number
                            else
                                UpdateSyncItem(SyncUserGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same); //very unlikely to happen because user cannot change QA number
                        }
                        else
                        {
                            //if guid and QAnumber is not found, record can be safely added
                            dsUSER_MAIN.USER_MAINRow drNewUser = dsUserMain.USER_MAIN.NewUSER_MAINRow();
                            drNewUser.ItemArray = drUserFrom.ItemArray;
                            dsUserMain.USER_MAIN.AddUSER_MAINRow(drNewUser);
                            daUserTo.Save(drNewUser);
                            Sync_UserDiscipline(daUserFrom, daUserTo, drNewUser.GUID, drNewUser.QANUMBER, drNewUser.DDISCIPLINE, syncStatus);
                            Sync_UserProject(daUserFrom, daUserTo, drNewUser.GUID, drNewUser.QANUMBER, drNewUser.DPROJECT, syncStatus);

                            if (!drNewUser.IsDELETEDNull())
                                UpdateSyncItem(SyncUserGuid, string.Empty, string.Empty, "New Deleted", syncStatus);
                            else
                                UpdateSyncItem(SyncUserGuid, string.Empty, string.Empty, "New", syncStatus);
                        }
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();
            }
        }

        /// <summary>
        /// Syncs global role
        /// </summary>
        /// <param name="syncMode"></param>
        private void Sync_Role(Sync_Mode syncMode, bool includeDeletes)
        {
            dsROLE_MAIN dsROLEMAIN = new dsROLE_MAIN();
            SyncStatusDisplay syncStatus = SyncStatusDisplay.upload; //set upload as default

            if (syncMode == Sync_Mode.None)
                return;
            else
            {
                AdapterROLE_MAIN daRoleFrom;
                AdapterROLE_MAIN daRoleTo;

                if (syncMode == Sync_Mode.Pull)
                {
                    daRoleFrom = _daRoleLocal; //when server is set to pull, it will be pulling from local db
                    daRoleTo = _daRoleRemote;
                }
                else
                {
                    daRoleFrom = _daRoleRemote;
                    daRoleTo = _daRoleLocal;
                    syncStatus = SyncStatusDisplay.download;
                }

                dsROLE_MAIN.ROLE_MAINDataTable dtRoleFrom;
                if (includeDeletes)
                    dtRoleFrom = daRoleFrom.GetIncludeDeleted();
                else
                    dtRoleFrom = daRoleFrom.Get();

                if (dtRoleFrom == null)
                    return;

                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Syncing Roles ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtRoleFrom.Rows.Count);

                foreach(dsROLE_MAIN.ROLE_MAINRow drRoleFrom in dtRoleFrom.Rows)
                {
                    Guid SyncRoleGuid = drRoleFrom.GUID;
                    _SyncItems.Add(new SyncStatus_Superseded(SyncRoleGuid, Guid.Empty)
                        {
                            syncItemName = drRoleFrom.NAME,
                            type = Sync_Type_Superseded.Role.ToString(),
                            SyncDate = DateTime.Now,
                            SyncBy = System_Environment.GetUser().GUID,
                            CreatedDate = drRoleFrom.CREATED,
                            CreatedBy = drRoleFrom.CREATEDBY,
                            UpdatedDate = drRoleFrom.IsUPDATEDNull() ? DateTime.MinValue : drRoleFrom.UPDATED,
                            UpdatedBy = drRoleFrom.IsUPDATEDBYNull() ? Guid.Empty : drRoleFrom.UPDATEDBY,
                            DeletedDate = drRoleFrom.IsDELETEDNull() ? DateTime.MinValue : drRoleFrom.DELETED,
                            DeletedBy = drRoleFrom.IsDELETEDBYNull() ? Guid.Empty : drRoleFrom.DELETEDBY
                        });

                    //perfect match with guid
                    dsROLE_MAIN.ROLE_MAINRow drRoleTo = daRoleTo.GetIncludeDeletedBy(drRoleFrom.GUID);
                    if (drRoleTo != null)
                    {
                        //to make sure this is the only role with this name
                        daRoleTo.RemoveAllWithExclusionBy(drRoleFrom.NAME, drRoleTo.GUID);

                        if (DataRowComparer.Default.Equals(drRoleTo, drRoleFrom))
                            UpdateSyncItem(SyncRoleGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        else
                        {
                            drRoleTo.ItemArray = drRoleFrom.ItemArray;
                            daRoleTo.Save(drRoleTo);

                            if(!drRoleTo.IsDELETEDNull())
                                UpdateSyncItem(SyncRoleGuid, string.Empty, string.Empty, "Delete", syncStatus);
                            else
                            {
                                //Sync_Privilege(daRoleFrom, daRoleTo, drRoleFrom.GUID, drRoleFrom.NAME, syncStatus); //Change on 11-DEC-2014 updates on role won't be synced
                                UpdateSyncItem(SyncRoleGuid, string.Empty, string.Empty, "Update (Privilege Not Synced)", syncStatus);
                            }
                        }
                    }
                    //partial match with role name
                    else
                    {
                        drRoleTo = daRoleTo.GetBy(drRoleFrom.NAME);
                        if (drRoleTo != null)
                        {
                            //if there is existing match with role name, let user resolve the conflict because there might be existing users using the role
                            if(!drRoleTo.IsDELETEDNull())
                                UpdateSyncItem(SyncRoleGuid, string.Empty, string.Empty, "Conflict", SyncStatusDisplay.warning);
                            else
                                UpdateSyncItem(SyncRoleGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        }
                        else
                        {
                            //if role name isn't found, record can be added safely
                            dsROLE_MAIN.ROLE_MAINRow drNewRole = dsROLEMAIN.ROLE_MAIN.NewROLE_MAINRow();
                            drNewRole.ItemArray = drRoleFrom.ItemArray;
                            dsROLEMAIN.ROLE_MAIN.AddROLE_MAINRow(drNewRole);
                            daRoleTo.Save(drNewRole);

                            Sync_Privilege(daRoleFrom, daRoleTo, drNewRole.GUID, drNewRole.NAME, syncStatus);

                            if (!drNewRole.IsDELETEDNull())
                                UpdateSyncItem(SyncRoleGuid, string.Empty, string.Empty, "New Deleted", syncStatus);
                            else
                                UpdateSyncItem(SyncRoleGuid, string.Empty, string.Empty, "New", syncStatus);
                        }
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();
            }
        }

        /// <summary>
        /// Sync global template
        /// </summary>
        private void Sync_Template(Sync_Mode syncMode, bool includeDeletes)
        {
            dsTEMPLATE_MAIN dsTEMPLATEMAIN = new dsTEMPLATE_MAIN();
            SyncStatusDisplay syncStatus = SyncStatusDisplay.upload; //set upload as default

            if (syncMode == Sync_Mode.None)
                return;
            else
            {
                AdapterTEMPLATE_MAIN daTemplateFrom;
                AdapterTEMPLATE_MAIN daTemplateTo;

                if(syncMode == Sync_Mode.Pull)
                {
                    daTemplateFrom = _daTemplateLocal;
                    daTemplateTo = _daTemplateRemote;
                }
                else
                {
                    daTemplateFrom = _daTemplateRemote;
                    daTemplateTo = _daTemplateLocal;
                    syncStatus = SyncStatusDisplay.download;
                }

                dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTemplateFrom;
                if (includeDeletes)
                    dtTemplateFrom = daTemplateFrom.GetIncludeDeleted();
                else
                    dtTemplateFrom = daTemplateFrom.Get();

                if (dtTemplateFrom == null)
                    return;

                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Syncing Templates ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtTemplateFrom.Rows.Count);

                foreach(dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplateFrom in dtTemplateFrom.Rows)
                {
                    Guid SyncTemplateGuid = Guid.NewGuid();
                    _SyncItems.Add(new SyncStatus_Superseded(SyncTemplateGuid, drTemplateFrom.WORKFLOWGUID)
                        {
                            syncItemName = drTemplateFrom.NAME,
                            additionalInfo = drTemplateFrom.DISCIPLINE,
                            type = Sync_Type_Superseded.Template.ToString(),
                            SyncDate = DateTime.Now,
                            SyncBy = System_Environment.GetUser().GUID,
                            CreatedDate = drTemplateFrom.CREATED,
                            CreatedBy = drTemplateFrom.CREATEDBY,
                            UpdatedDate = drTemplateFrom.IsUPDATEDNull() ? DateTime.MinValue : drTemplateFrom.UPDATED,
                            UpdatedBy = drTemplateFrom.IsUPDATEDBYNull() ? Guid.Empty : drTemplateFrom.UPDATEDBY,
                            DeletedDate = drTemplateFrom.IsDELETEDNull() ? DateTime.MinValue : drTemplateFrom.DELETED,
                            DeletedBy = drTemplateFrom.IsDELETEDBYNull() ? Guid.Empty : drTemplateFrom.DELETEDBY
                        });

                    //perfect match with guid
                    dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplateTo = daTemplateTo.GetIncludeDeletedBy(drTemplateFrom.GUID);
                    if(drTemplateTo != null)
                    {
                        //to make sure this is the only template with this name
                        daTemplateTo.RemoveWithExclusionBy(drTemplateFrom.NAME, drTemplateTo.GUID);

                        if (DataRowComparer.Default.Equals(drTemplateTo, drTemplateFrom))
                        {
                            UpdateSyncItem(SyncTemplateGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        }
                        else
                        {
                            drTemplateTo.ItemArray = drTemplateFrom.ItemArray;
                            daTemplateTo.Save(drTemplateTo);

                            if(!drTemplateTo.IsDELETEDNull())
                                UpdateSyncItem(SyncTemplateGuid, string.Empty, string.Empty, "Delete", syncStatus);
                            else
                                UpdateSyncItem(SyncTemplateGuid, string.Empty, string.Empty, "Update", syncStatus);
                        }
                    }
                    //partial match with template name
                    else
                    {
                        drTemplateTo = daTemplateTo.GetBy(drTemplateFrom.NAME);
                        if (drTemplateTo != null)
                        {
                            //if there is existing match with template name, let user resolve the conflict because there might be existing tag relationship using the template
                            if(!drTemplateTo.IsDELETEDNull())
                                UpdateSyncItem(SyncTemplateGuid, string.Empty, string.Empty, "Conflict", SyncStatusDisplay.warning);
                            else
                                UpdateSyncItem(SyncTemplateGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        }
                        else
                        {
                            //if template name isn't found, record can be safely added
                            dsTEMPLATE_MAIN.TEMPLATE_MAINRow drNewTemplate = dsTEMPLATEMAIN.TEMPLATE_MAIN.NewTEMPLATE_MAINRow();
                            drNewTemplate.ItemArray = drTemplateFrom.ItemArray;
                            dsTEMPLATEMAIN.TEMPLATE_MAIN.AddTEMPLATE_MAINRow(drNewTemplate);
                            daTemplateTo.Save(drNewTemplate);

                            if (!drNewTemplate.IsDELETEDNull())
                                UpdateSyncItem(SyncTemplateGuid, string.Empty, string.Empty, "New Deleted", syncStatus);
                            else
                                UpdateSyncItem(SyncTemplateGuid, string.Empty, string.Empty, "New", syncStatus);
                        }
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();
            }
        }

        /// <summary>
        /// Sync global prefill
        /// </summary>
        private void Sync_Prefill(Sync_Mode syncMode, bool includeDeletes)
        {
            dsPREFILL_MAIN dsPREFILL_MAINMAIN = new dsPREFILL_MAIN();
            SyncStatusDisplay syncStatus = SyncStatusDisplay.upload; //set upload as default

            if (syncMode == Sync_Mode.None)
                return;
            else
            {
                AdapterPREFILL_MAIN daPrefillFrom;
                AdapterPREFILL_MAIN daPrefillTo;

                if (syncMode == Sync_Mode.Pull)
                {
                    daPrefillFrom = _daPrefillLocal;
                    daPrefillTo = _daPrefillRemote;
                }
                else
                {
                    daPrefillFrom = _daPrefillRemote;
                    daPrefillTo = _daPrefillLocal;
                    syncStatus = SyncStatusDisplay.download;
                }

                dsPREFILL_MAIN.PREFILL_MAINDataTable dtPrefillFrom;
                if (includeDeletes)
                    dtPrefillFrom = daPrefillFrom.GetIncludeDeleted();
                else
                    dtPrefillFrom = daPrefillFrom.Get();

                if (dtPrefillFrom == null)
                    return;

                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Syncing Prefills ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtPrefillFrom.Rows.Count);

                foreach (dsPREFILL_MAIN.PREFILL_MAINRow drPrefillFrom in dtPrefillFrom.Rows)
                {
                    Guid SyncPrefillGuid = Guid.NewGuid();
                    _SyncItems.Add(new SyncStatus_Superseded(SyncPrefillGuid, Guid.Empty)
                    {
                        syncItemName = drPrefillFrom.NAME,
                        additionalInfo = drPrefillFrom.NAME,
                        type = Sync_Type_Superseded.Prefill.ToString(),
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drPrefillFrom.CREATED,
                        CreatedBy = drPrefillFrom.CREATEDBY,
                        UpdatedDate = drPrefillFrom.IsUPDATEDNull() ? DateTime.MinValue : drPrefillFrom.UPDATED,
                        UpdatedBy = drPrefillFrom.IsUPDATEDBYNull() ? Guid.Empty : drPrefillFrom.UPDATEDBY,
                        DeletedDate = drPrefillFrom.IsDELETEDNull() ? DateTime.MinValue : drPrefillFrom.DELETED,
                        DeletedBy = drPrefillFrom.IsDELETEDBYNull() ? Guid.Empty : drPrefillFrom.DELETEDBY
                    });

                    //perfect match with guid
                    dsPREFILL_MAIN.PREFILL_MAINRow drPrefillTo = daPrefillTo.GetIncludeDeletedBy(drPrefillFrom.GUID);
                    if (drPrefillTo != null)
                    {
                        //to make sure this is the only prefill with this name
                        daPrefillTo.RemoveWithExclusionBy(drPrefillFrom.NAME, drPrefillTo.GUID);

                        if (DataRowComparer.Default.Equals(drPrefillTo, drPrefillFrom))
                            UpdateSyncItem(SyncPrefillGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        else
                        {
                            drPrefillTo.ItemArray = drPrefillFrom.ItemArray;
                            daPrefillTo.Save(drPrefillTo);

                            if(!drPrefillTo.IsDELETEDNull())
                                UpdateSyncItem(SyncPrefillGuid, string.Empty, string.Empty, "Delete", syncStatus);
                            else
                                UpdateSyncItem(SyncPrefillGuid, string.Empty, string.Empty, "Update", syncStatus);
                        }
                    }
                    //partial match with prefill name
                    else
                    {
                        drPrefillTo = daPrefillTo.GetBy(drPrefillFrom.NAME);
                        if (drPrefillTo != null)
                        {
                            //if there is existing match with prefill name, let user resolve the conflict because there might be existing tag relationship using the prefill
                            if (!drPrefillTo.IsDELETEDNull())
                                UpdateSyncItem(SyncPrefillGuid, string.Empty, string.Empty, "Conflict", SyncStatusDisplay.warning);
                            else
                                UpdateSyncItem(SyncPrefillGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        }
                        else
                        {
                            //if prefill name isn't found, record can be safely added
                            dsPREFILL_MAIN.PREFILL_MAINRow drNewPrefill = dsPREFILL_MAINMAIN.PREFILL_MAIN.NewPREFILL_MAINRow();
                            drNewPrefill.ItemArray = drPrefillFrom.ItemArray;
                            dsPREFILL_MAINMAIN.PREFILL_MAIN.AddPREFILL_MAINRow(drNewPrefill);
                            daPrefillTo.Save(drNewPrefill);

                            if (!drNewPrefill.IsDELETEDNull())
                                UpdateSyncItem(SyncPrefillGuid, string.Empty, string.Empty, "New Deleted", syncStatus);
                            else
                                UpdateSyncItem(SyncPrefillGuid, string.Empty, string.Empty, "New", syncStatus);
                        }
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();
            }
        }

        private void Sync_PrefillRegister(AdapterPREFILL_REGISTER daPrefillRegFrom, AdapterPREFILL_REGISTER daPrefillRegTo, Guid wbsTagGuid, string wbsTagName, SyncStatusDisplay syncStatus)
        {
            dsPREFILL_REGISTER dsPrefillReg = new dsPREFILL_REGISTER();
            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPrefillRegFrom = daPrefillRegFrom.GetByWBSTag(wbsTagGuid);
            List<dsPREFILL_REGISTER.PREFILL_REGISTERRow> listPrefillRegTo = new List<dsPREFILL_REGISTER.PREFILL_REGISTERRow>();
            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPrefillRegTo = daPrefillRegTo.GetByWBSTag(wbsTagGuid);
            if (dtPrefillRegTo != null)
                listPrefillRegTo = dtPrefillRegTo.ToList();

            if (dtPrefillRegFrom != null)
            {
                foreach (dsPREFILL_REGISTER.PREFILL_REGISTERRow drPrefillRegFrom in dtPrefillRegFrom.Rows)
                {
                    Guid SyncPrefillGuid = Guid.NewGuid();

                    _SyncItems.Add(new SyncStatus_Superseded(SyncPrefillGuid, wbsTagGuid)
                    {
                        syncItemName = drPrefillRegFrom.NAME,
                        additionalInfo = wbsTagName,
                        type = "Prefill Data",
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drPrefillRegFrom.CREATED,
                        CreatedBy = drPrefillRegFrom.CREATEDBY,
                        UpdatedDate = drPrefillRegFrom.IsUPDATEDNull() ? DateTime.MinValue : drPrefillRegFrom.UPDATED,
                        UpdatedBy = drPrefillRegFrom.IsUPDATEDBYNull() ? Guid.Empty : drPrefillRegFrom.UPDATEDBY,
                        DeletedDate = drPrefillRegFrom.IsDELETEDNull() ? DateTime.MinValue : drPrefillRegFrom.DELETED,
                        DeletedBy = drPrefillRegFrom.IsDELETEDBYNull() ? Guid.Empty : drPrefillRegFrom.DELETEDBY
                    });

                    dsPREFILL_REGISTER.PREFILL_REGISTERRow drPrefillRegTo;
                    if(drPrefillRegFrom.TAG_GUID != null)
                        drPrefillRegTo = listPrefillRegTo.FirstOrDefault(obj => obj.NAME == drPrefillRegFrom.NAME && obj.TAG_GUID == drPrefillRegFrom.TAG_GUID && obj.DATA == drPrefillRegFrom.DATA);
                    else
                        drPrefillRegTo = listPrefillRegTo.FirstOrDefault(obj => obj.NAME == drPrefillRegFrom.NAME && obj.WBS_GUID == drPrefillRegFrom.WBS_GUID && obj.DATA == drPrefillRegFrom.DATA);

                    if (drPrefillRegTo == null)
                    {
                        dsPREFILL_REGISTER.PREFILL_REGISTERRow drPrefillRegNew = dsPrefillReg.PREFILL_REGISTER.NewPREFILL_REGISTERRow();
                        drPrefillRegNew.ItemArray = drPrefillRegFrom.ItemArray;
                        drPrefillRegNew.GUID = Guid.NewGuid(); //prevent bumping into previously synced but deleted guid
                        dsPrefillReg.PREFILL_REGISTER.AddPREFILL_REGISTERRow(drPrefillRegNew);
                        daPrefillRegTo.Save(drPrefillRegNew);
                        UpdateSyncItem(SyncPrefillGuid, string.Empty, string.Empty, "New", syncStatus);
                    }
                    else
                        UpdateSyncItem(SyncPrefillGuid, string.Empty, string.Empty, "No Change", syncStatus);
                }

                //Cross checking to trim unused in 'from' table
                foreach(dsPREFILL_REGISTER.PREFILL_REGISTERRow drPrefillRegTo in listPrefillRegTo)
                {
                    if(!dtPrefillRegFrom.Any(obj => obj.NAME == drPrefillRegTo.NAME && ((!obj.IsTAG_GUIDNull() && obj.TAG_GUID == drPrefillRegTo.TAG_GUID) || (!obj.IsWBS_GUIDNull() && obj.WBS_GUID == drPrefillRegTo.WBS_GUID)) && obj.DATA == drPrefillRegTo.DATA))
                    {
                        _SyncItems.Add(new SyncStatus_Superseded(Guid.NewGuid(), wbsTagGuid)
                        {
                            syncItemName = drPrefillRegTo.NAME,
                            additionalInfo = wbsTagName,
                            type = Sync_Type_Superseded.Privilege.ToString(),
                            status = "Delete",
                            SyncDate = DateTime.Now,
                            SyncBy = System_Environment.GetUser().GUID,
                            CreatedDate = drPrefillRegTo.CREATED,
                            CreatedBy = drPrefillRegTo.CREATEDBY,
                            UpdatedDate = drPrefillRegTo.IsUPDATEDNull() ? DateTime.MinValue : drPrefillRegTo.UPDATED,
                            UpdatedBy = drPrefillRegTo.IsUPDATEDBYNull() ? Guid.Empty : drPrefillRegTo.UPDATEDBY,
                            DeletedDate = drPrefillRegTo.IsDELETEDNull() ? DateTime.MinValue : drPrefillRegTo.DELETED,
                            DeletedBy = drPrefillRegTo.IsDELETEDBYNull() ? Guid.Empty : drPrefillRegTo.DELETEDBY
                        });

                        daPrefillRegTo.RemoveBy(wbsTagGuid, drPrefillRegTo.NAME);
                    }
                }
            }
        }

        /// <summary>
        /// Sync global workflow
        /// </summary>
        private void Sync_Workflow(Sync_Mode syncMode, bool includeDeletes)
        {
            dsWORKFLOW_MAIN dsWORKFLOWMAIN = new dsWORKFLOW_MAIN();
            SyncStatusDisplay syncStatus = SyncStatusDisplay.upload; //set upload as default

            if (syncMode == Sync_Mode.None)
                return;
            else
            {
                AdapterWORKFLOW_MAIN daWorkflowFrom;
                AdapterWORKFLOW_MAIN daWorkflowTo;

                if (syncMode == Sync_Mode.Pull)
                {
                    daWorkflowFrom = _daWorkflowLocal;
                    daWorkflowTo = _daWorkflowRemote;
                }
                else
                {
                    daWorkflowFrom = _daWorkflowRemote;
                    daWorkflowTo = _daWorkflowLocal;
                    syncStatus = SyncStatusDisplay.download;
                }

                dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWorkflowFrom;
                if (includeDeletes)
                    dtWorkflowFrom = daWorkflowFrom.GetIncludeDeletes();
                else
                    dtWorkflowFrom = daWorkflowFrom.Get();

                if (dtWorkflowFrom == null)
                    return;

                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Syncing Workflows ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtWorkflowFrom.Rows.Count);

                foreach (dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflowFrom in dtWorkflowFrom.Rows)
                {
                    Guid SyncWorkflowGuid = drWorkflowFrom.GUID;
                    _SyncItems.Add(new SyncStatus_Superseded(SyncWorkflowGuid, drWorkflowFrom.PARENTGUID)
                    {
                        syncItemName = drWorkflowFrom.NAME,
                        type = Sync_Type_Superseded.Workflow.ToString(),
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drWorkflowFrom.CREATED,
                        CreatedBy = drWorkflowFrom.CREATEDBY,
                        UpdatedDate = drWorkflowFrom.IsUPDATEDNull() ? DateTime.MinValue : drWorkflowFrom.UPDATED,
                        UpdatedBy = drWorkflowFrom.IsUPDATEDBYNull() ? Guid.Empty : drWorkflowFrom.UPDATEDBY,
                        DeletedDate = drWorkflowFrom.IsDELETEDNull() ? DateTime.MinValue : drWorkflowFrom.DELETED,
                        DeletedBy = drWorkflowFrom.IsDELETEDBYNull() ? Guid.Empty : drWorkflowFrom.DELETEDBY
                    });

                    //perfect match with guid
                    dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflowTo = daWorkflowTo.GetIncludeDeletedBy(drWorkflowFrom.GUID);
                    if (drWorkflowTo != null)
                    {
                        //to make sure this is the only workflow with this name
                        daWorkflowTo.RemoveWithExclusionBy(drWorkflowFrom.NAME, drWorkflowTo.GUID);

                        if (DataRowComparer.Default.Equals(drWorkflowTo, drWorkflowFrom))
                            UpdateSyncItem(SyncWorkflowGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        else
                        {
                            drWorkflowTo.ItemArray = drWorkflowFrom.ItemArray;
                            daWorkflowTo.Save(drWorkflowTo);

                            if(!drWorkflowTo.IsDELETEDNull())
                                UpdateSyncItem(SyncWorkflowGuid, string.Empty, string.Empty, "Delete", syncStatus);
                            else
                                UpdateSyncItem(SyncWorkflowGuid, string.Empty, string.Empty, "Update", syncStatus);
                        }
                    }
                    //partial match with workflow name
                    else
                    {
                        drWorkflowTo = daWorkflowTo.GetBy(drWorkflowFrom.NAME);
                        if (drWorkflowTo != null)
                        {
                            //if there is existing match with workflow name, let user resolve the conflict because there might be existing tag relationship using the workflow
                            if(!drWorkflowTo.IsDELETEDNull())
                                UpdateSyncItem(SyncWorkflowGuid, string.Empty, string.Empty, "Conflict", SyncStatusDisplay.warning);
                            else
                                UpdateSyncItem(SyncWorkflowGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        }
                        else
                        {
                            //if workflow name isn't found, record can be safely added
                            dsWORKFLOW_MAIN.WORKFLOW_MAINRow drNewWorkflow = dsWORKFLOWMAIN.WORKFLOW_MAIN.NewWORKFLOW_MAINRow();
                            drNewWorkflow.ItemArray = drWorkflowFrom.ItemArray;
                            dsWORKFLOWMAIN.WORKFLOW_MAIN.AddWORKFLOW_MAINRow(drNewWorkflow);
                            daWorkflowTo.Save(drNewWorkflow);

                            if(!drNewWorkflow.IsDELETEDNull())
                                UpdateSyncItem(SyncWorkflowGuid, string.Empty, string.Empty, "New Deleted", syncStatus);
                            else
                                UpdateSyncItem(SyncWorkflowGuid, string.Empty, string.Empty, "New", syncStatus);
                        }
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();
            }
        }

        /// <summary>
        /// Sync global equipment
        /// </summary>
        private void Sync_Equipment(Sync_Mode syncMode, bool includeDeletes)
        {
            dsGENERAL_EQUIPMENT dsGENERAL_EQUIPMENTMAIN = new dsGENERAL_EQUIPMENT();
            SyncStatusDisplay syncStatus = SyncStatusDisplay.upload; //set upload as default

            if (syncMode == Sync_Mode.None)
                return;
            else
            {
                AdapterGENERAL_EQUIPMENT daEquipmentFrom;
                AdapterGENERAL_EQUIPMENT daEquipmentTo;

                if (syncMode == Sync_Mode.Pull)
                {
                    daEquipmentFrom = _daEquipmentLocal;
                    daEquipmentTo = _daEquipmentRemote;
                }
                else
                {
                    daEquipmentFrom = _daEquipmentRemote;
                    daEquipmentTo = _daEquipmentLocal;
                    syncStatus = SyncStatusDisplay.download;
                }

                dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtEquipmentFrom;
                if (includeDeletes)
                    dtEquipmentFrom = daEquipmentFrom.GetAll_IncludeDeleted();
                else
                    dtEquipmentFrom = daEquipmentFrom.Get();

                if (dtEquipmentFrom == null)
                    return;

                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Syncing Equipments ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtEquipmentFrom.Rows.Count);

                foreach (dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drEquipmentFrom in dtEquipmentFrom.Rows)
                {
                    Guid SyncEquipmentGuid = drEquipmentFrom.GUID;
                    _SyncItems.Add(new SyncStatus_Superseded(SyncEquipmentGuid, Guid.Empty)
                    {
                        syncItemName = drEquipmentFrom.ASSET_NUMBER,
                        additionalInfo = drEquipmentFrom.DISCIPLINE,
                        type = Sync_Type_Superseded.Equipment.ToString(),
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drEquipmentFrom.CREATED,
                        CreatedBy = drEquipmentFrom.CREATEDBY,
                        UpdatedDate = drEquipmentFrom.IsUPDATEDNull() ? DateTime.MinValue : drEquipmentFrom.UPDATED,
                        UpdatedBy = drEquipmentFrom.IsUPDATEDBYNull() ? Guid.Empty : drEquipmentFrom.UPDATEDBY,
                        DeletedDate = drEquipmentFrom.IsDELETEDNull() ? DateTime.MinValue : drEquipmentFrom.DELETED,
                        DeletedBy = drEquipmentFrom.IsDELETEDBYNull() ? Guid.Empty : drEquipmentFrom.DELETEDBY
                    });

                    //perfect match with guid
                    dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drEquipmentTo = daEquipmentTo.GetBy_IncludeDeleted(drEquipmentFrom.GUID);
                    if (drEquipmentTo != null)
                    {
                        //to make sure this is the only euipqment with this serial number
                        daEquipmentTo.RemoveWithExclusionBy(drEquipmentTo.SERIAL, drEquipmentTo.GUID);

                        if (DataRowComparer.Default.Equals(drEquipmentTo, drEquipmentFrom))
                            UpdateSyncItem(SyncEquipmentGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        else
                        {
                            drEquipmentTo.ItemArray = drEquipmentFrom.ItemArray;
                            daEquipmentTo.Save(drEquipmentTo);

                            if (!drEquipmentTo.IsDELETEDNull())
                                UpdateSyncItem(SyncEquipmentGuid, string.Empty, string.Empty, "Delete", syncStatus);
                            else
                                UpdateSyncItem(SyncEquipmentGuid, string.Empty, string.Empty, "Update", syncStatus);
                        }
                    }
                    //partial match with workflow name
                    else
                    {
                        drEquipmentTo = daEquipmentTo.GetBy(drEquipmentFrom.SERIAL);
                        if (drEquipmentTo != null)
                        {
                            //if there is existing match by serial number, let user resolve the conflict it might be used
                            if (!drEquipmentTo.IsDELETEDNull())
                                UpdateSyncItem(SyncEquipmentGuid, string.Empty, string.Empty, "Conflict", SyncStatusDisplay.warning);
                            else
                                UpdateSyncItem(SyncEquipmentGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        }
                        else
                        {
                            //if workflow name isn't found, record can be safely added
                            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drNewEquipment = dsGENERAL_EQUIPMENTMAIN.GENERAL_EQUIPMENT.NewGENERAL_EQUIPMENTRow();
                            drNewEquipment.ItemArray = drEquipmentFrom.ItemArray;
                            dsGENERAL_EQUIPMENTMAIN.GENERAL_EQUIPMENT.AddGENERAL_EQUIPMENTRow(drNewEquipment);
                            daEquipmentTo.Save(drNewEquipment);

                            if (!drNewEquipment.IsDELETEDNull())
                                UpdateSyncItem(SyncEquipmentGuid, string.Empty, string.Empty, "New Deleted", syncStatus);
                            else
                                UpdateSyncItem(SyncEquipmentGuid, string.Empty, string.Empty, "New", syncStatus);
                        }
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();
            }
        }

        /// <summary>
        /// Sync global project
        /// </summary>
        private void Sync_Project(Sync_Mode syncMode)
        {
            dsPROJECT dsPROJECTMAIN = new dsPROJECT();
            SyncStatusDisplay syncStatus = SyncStatusDisplay.upload; //set upload as default

            if (syncMode == Sync_Mode.None)
                return;
            else
            {
                AdapterPROJECT daProjectFrom;
                AdapterPROJECT daProjectTo;

                if (syncMode == Sync_Mode.Pull)
                {
                    daProjectFrom = _daProjectLocal;
                    daProjectTo = _daProjectRemote;
                }
                else
                {
                    daProjectFrom = _daProjectRemote;
                    daProjectTo = _daProjectLocal;
                    syncStatus = SyncStatusDisplay.download;
                }

                dsPROJECT.PROJECTDataTable dtProjectFrom = daProjectFrom.Get();
                if (dtProjectFrom == null)
                    return;

                foreach(dsPROJECT.PROJECTRow drProjectFrom in dtProjectFrom.Rows)
                {
                    Guid projectSyncGuid = drProjectFrom.GUID;
                    _SyncItems.Add(new SyncStatus_Superseded(projectSyncGuid, Guid.Empty)
                    {
                        syncItemName = drProjectFrom.NUMBER,
                        additionalInfo = drProjectFrom.NAME,
                        type = Sync_Type_Superseded.Project.ToString(),
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drProjectFrom.CREATED,
                        CreatedBy = drProjectFrom.CREATEDBY,
                        UpdatedDate = drProjectFrom.IsUPDATEDNull() ? DateTime.MinValue : drProjectFrom.UPDATED,
                        UpdatedBy = drProjectFrom.IsUPDATEDBYNull() ? Guid.Empty : drProjectFrom.UPDATEDBY,
                        DeletedDate = drProjectFrom.IsDELETEDNull() ? DateTime.MinValue : drProjectFrom.DELETED,
                        DeletedBy = drProjectFrom.IsDELETEDBYNull() ? Guid.Empty : drProjectFrom.DELETEDBY
                    });

                    //perfect match with guid
                    dsPROJECT.PROJECTRow drProjectTo = daProjectTo.GetIncludeDeletedBy(drProjectFrom.GUID);
                    if (drProjectTo != null)
                    {
                        //to make sure this is the only project with this name
                        daProjectTo.RemoveWithExclusionBy(drProjectFrom.NUMBER, drProjectTo.GUID);

                        if (DataRowComparer.Default.Equals(drProjectTo, drProjectFrom))
                            UpdateSyncItem(projectSyncGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        else
                        {
                            drProjectTo.ItemArray = drProjectFrom.ItemArray;
                            daProjectTo.Save(drProjectTo);

                            if (!drProjectTo.IsDELETEDNull())
                                UpdateSyncItem(projectSyncGuid, string.Empty, string.Empty, "Delete", syncStatus);
                            else
                                UpdateSyncItem(projectSyncGuid, string.Empty, string.Empty, "Update", syncStatus);
                        }
                    }
                    //partial match with project name
                    else
                    {
                        drProjectTo = daProjectTo.GetBy(drProjectFrom.NUMBER);
                        if (drProjectTo != null)
                        {
                            //if there is existing match with project name, let user resolve the conflict because there might be existing tag relationship using the project
                            if(!drProjectTo.IsDELETEDNull())
                                UpdateSyncItem(projectSyncGuid, string.Empty, string.Empty, "Conflict", SyncStatusDisplay.warning);
                            else
                                UpdateSyncItem(projectSyncGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        }
                        else
                        {
                            //if project name isn't found, record can be safely added
                            dsPROJECT.PROJECTRow drNewProject = dsPROJECTMAIN.PROJECT.NewPROJECTRow();
                            drNewProject.ItemArray = drProjectFrom.ItemArray;
                            dsPROJECTMAIN.PROJECT.AddPROJECTRow(drNewProject);
                            daProjectTo.Save(drNewProject);

                            if (!drNewProject.IsDELETEDNull())
                                UpdateSyncItem(projectSyncGuid, string.Empty, string.Empty, "New Deleted", syncStatus);
                            else
                                UpdateSyncItem(projectSyncGuid, string.Empty, string.Empty, "New", syncStatus);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Syncs global schedule
        /// </summary>
        /// <param name="syncMode"></param>
        private void Sync_Schedule(Sync_Mode syncMode, Guid projectGuid, bool includeDeletes)
        {
            dsSCHEDULE dsSCHEDULEMAIN = new dsSCHEDULE();
            SyncStatusDisplay syncStatus = SyncStatusDisplay.upload; //set upload as default

            if (syncMode == Sync_Mode.None)
                return;
            else
            {
                AdapterSCHEDULE daScheduleFrom;
                AdapterSCHEDULE daScheduleTo;

                if (syncMode == Sync_Mode.Pull)
                {
                    daScheduleFrom = _daScheduleLocal; //when server is set to pull, it will be pulling from local db
                    daScheduleTo = _daScheduleRemote;
                }
                else
                {
                    daScheduleFrom = _daScheduleRemote;
                    daScheduleTo = _daScheduleLocal;
                    syncStatus = SyncStatusDisplay.download;
                }

                dsSCHEDULE.SCHEDULEDataTable dtScheduleFrom;
                if (includeDeletes)
                    dtScheduleFrom = daScheduleFrom.GetByProjectIncludeDeleted(projectGuid);
                else
                    dtScheduleFrom = daScheduleFrom.GetByProject(projectGuid);

                if (dtScheduleFrom == null)
                    return;

                foreach (dsSCHEDULE.SCHEDULERow drScheduleFrom in dtScheduleFrom.Rows)
                {
                    _SyncItems.Add(new SyncStatus_Superseded(drScheduleFrom.GUID, projectGuid)
                    {
                        syncItemName = drScheduleFrom.NAME,
                        type = Sync_Type_Superseded.Schedule.ToString(),
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drScheduleFrom.CREATED,
                        CreatedBy = drScheduleFrom.CREATEDBY,
                        UpdatedDate = drScheduleFrom.IsUPDATEDNull() ? DateTime.MinValue : drScheduleFrom.UPDATED,
                        UpdatedBy = drScheduleFrom.IsUPDATEDBYNull() ? Guid.Empty : drScheduleFrom.UPDATEDBY,
                        DeletedDate = drScheduleFrom.IsDELETEDNull() ? DateTime.MinValue : drScheduleFrom.DELETED,
                        DeletedBy = drScheduleFrom.IsDELETEDBYNull() ? Guid.Empty : drScheduleFrom.DELETEDBY
                    });

                    //perfect match with guid
                    dsSCHEDULE.SCHEDULERow drScheduleTo = daScheduleTo.GetIncludeDeletedBy(drScheduleFrom.GUID);
                    if (drScheduleTo != null)
                    {
                        //to make sure this is the only schedule with this name
                        daScheduleTo.RemoveAllWithExclusionBy(drScheduleFrom.NAME, drScheduleTo.GUID);

                        if (DataRowComparer.Default.Equals(drScheduleTo, drScheduleFrom))
                            UpdateSyncItem(drScheduleFrom.GUID, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        else
                        {
                            drScheduleTo.ItemArray = drScheduleFrom.ItemArray;
                            daScheduleTo.Save(drScheduleTo);

                            if(!drScheduleTo.IsDELETEDNull())
                                UpdateSyncItem(drScheduleFrom.GUID, string.Empty, string.Empty, "Delete", syncStatus);
                            else
                                UpdateSyncItem(drScheduleFrom.GUID, string.Empty, string.Empty, "Update", syncStatus);
                        }
                    }
                    //partial match with schedule name
                    else
                    {
                        drScheduleTo = daScheduleTo.GetBy(drScheduleFrom.NAME);
                        if (drScheduleTo != null)
                        {
                            //if there is existing match with schedule name, let user resolve the conflict because there might be existing users using the schedule
                            if(!drScheduleTo.IsDELETEDNull())
                                UpdateSyncItem(drScheduleFrom.GUID, string.Empty, string.Empty, "Conflict", SyncStatusDisplay.warning);
                            else
                                UpdateSyncItem(drScheduleFrom.GUID, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        }

                        else
                        {
                            //if schedule name isn't found, record can be added safely
                            dsSCHEDULE.SCHEDULERow drNewSchedule = dsSCHEDULEMAIN.SCHEDULE.NewSCHEDULERow();
                            drNewSchedule.ItemArray = drScheduleFrom.ItemArray;
                            dsSCHEDULEMAIN.SCHEDULE.AddSCHEDULERow(drNewSchedule);
                            daScheduleTo.Save(drNewSchedule);

                            if (!drNewSchedule.IsDELETEDNull())
                                UpdateSyncItem(drScheduleFrom.GUID, string.Empty, string.Empty, "New Deleted", syncStatus);
                            else
                                UpdateSyncItem(drScheduleFrom.GUID, string.Empty, string.Empty, "New", syncStatus);
                        }
                    }

                    Sync_Tag(syncMode, projectGuid, drScheduleFrom.GUID, drScheduleFrom.NAME, includeDeletes);
                    Sync_WBS(syncMode, projectGuid, drScheduleFrom.GUID, drScheduleFrom.NAME, includeDeletes);
                }
            }
        }

        /// <summary>
        /// Syncs the user discipline
        /// </summary>
        private void Sync_UserDiscipline(AdapterUSER_MAIN daUserFrom, AdapterUSER_MAIN daUserTo, Guid userGuid, string userName, string defaultDiscipline, SyncStatusDisplay syncStatus)
        {
            dsUSER_MAIN dsUSERMAIN = new dsUSER_MAIN();
            dsUSER_MAIN.USER_DISCDataTable dtUserDiscFrom = daUserFrom.GetUserDisciplines(userGuid);
            List<dsUSER_MAIN.USER_DISCRow> listUserDiscTo = new List<dsUSER_MAIN.USER_DISCRow>();
            dsUSER_MAIN.USER_DISCDataTable dtUserDiscTo = daUserTo.GetUserDisciplines(userGuid);
            if (dtUserDiscTo != null)
                listUserDiscTo = dtUserDiscTo.ToList();

            if(dtUserDiscFrom != null)
            {
                foreach(dsUSER_MAIN.USER_DISCRow drUserDiscFrom in dtUserDiscFrom.Rows)
                {
                    Guid SyncDisciplineGuid = Guid.NewGuid();

                    _SyncItems.Add(new SyncStatus_Superseded(SyncDisciplineGuid, userGuid)
                    {
                        syncItemName = drUserDiscFrom.DISCIPLINE,
                        additionalInfo = userName,
                        type = "Discipline Auth",
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drUserDiscFrom.CREATED,
                        CreatedBy = drUserDiscFrom.CREATEDBY,
                        UpdatedDate = DateTime.MinValue,
                        UpdatedBy = Guid.Empty,
                        DeletedDate = drUserDiscFrom.IsDELETEDNull() ? DateTime.MinValue : drUserDiscFrom.DELETED,
                        DeletedBy = drUserDiscFrom.IsDELETEDBYNull() ? Guid.Empty : drUserDiscFrom.DELETEDBY
                    });

                    if (drUserDiscFrom.DISCIPLINE == defaultDiscipline)
                    {
                        UpdateSyncItem(SyncDisciplineGuid, string.Empty, string.Empty, "New", syncStatus);
                        continue; //skip default discipline because it'll be added by trigger
                    }
                        
                    dsUSER_MAIN.USER_DISCRow drUserDiscTo = listUserDiscTo.FirstOrDefault(obj => obj.DISCIPLINE == drUserDiscFrom.DISCIPLINE);
                    if(drUserDiscTo == null)
                    {
                        dsUSER_MAIN.USER_DISCRow drUserDiscNew = dsUSERMAIN.USER_DISC.NewUSER_DISCRow();
                        drUserDiscNew.ItemArray = drUserDiscFrom.ItemArray;
                        drUserDiscNew.GUID = Guid.NewGuid(); //to prevent bumping into previously synced guid
                        dsUSERMAIN.USER_DISC.AddUSER_DISCRow(drUserDiscNew);
                        daUserTo.Save(drUserDiscNew);
                        UpdateSyncItem(SyncDisciplineGuid, string.Empty, string.Empty, "New", syncStatus);
                    }
                    else //else don't do anything if the authorisation already exists
                        UpdateSyncItem(SyncDisciplineGuid, string.Empty, string.Empty, "No Change", syncStatus);
                }

                //Cross checking to trim unused in 'from' table
                foreach (dsUSER_MAIN.USER_DISCRow drUserDiscTo in listUserDiscTo)
                {
                    if (!dtUserDiscFrom.Any(obj => obj.DISCIPLINE == drUserDiscTo.DISCIPLINE))
                    {
                        _SyncItems.Add(new SyncStatus_Superseded(Guid.NewGuid(), userGuid)
                        {
                            syncItemName = drUserDiscTo.DISCIPLINE,
                            additionalInfo = userName,
                            type = "Discipline Auth",
                            status = "Delete",
                            SyncDate = DateTime.Now,
                            SyncBy = System_Environment.GetUser().GUID,
                            CreatedDate = drUserDiscTo.CREATED,
                            CreatedBy = drUserDiscTo.CREATEDBY,
                            UpdatedDate = DateTime.MinValue,
                            UpdatedBy = Guid.Empty,
                            DeletedDate = drUserDiscTo.IsDELETEDNull() ? DateTime.MinValue : drUserDiscTo.DELETED,
                            DeletedBy = drUserDiscTo.IsDELETEDBYNull() ? Guid.Empty : drUserDiscTo.DELETEDBY
                        });

                        daUserTo.RemoveUserDiscipline(userGuid, drUserDiscTo.DISCIPLINE);
                    }
                }
            }
        }

        /// <summary>
        /// Syncs the user project
        /// </summary>
        private void Sync_UserProject(AdapterUSER_MAIN daUserFrom, AdapterUSER_MAIN daUserTo, Guid userGuid, string userName, Guid defaultProject, SyncStatusDisplay syncStatus)
        {
            dsUSER_MAIN dsUSERMAIN = new dsUSER_MAIN();
            dsUSER_MAIN.USER_PROJECTDataTable dtUserProjFrom = daUserFrom.GetUserProjects(userGuid);
            List<dsUSER_MAIN.USER_PROJECTRow> listUserProjTo = new List<dsUSER_MAIN.USER_PROJECTRow>();
            dsUSER_MAIN.USER_PROJECTDataTable dtUserProjTo = daUserTo.GetUserProjects(userGuid);
            if (dtUserProjTo != null)
                listUserProjTo = dtUserProjTo.ToList();

            if(dtUserProjFrom != null)
            {
                foreach(dsUSER_MAIN.USER_PROJECTRow drUserProjFrom in dtUserProjFrom.Rows)
                {
                    Guid SyncDisciplineGuid = Guid.NewGuid();

                    _SyncItems.Add(new SyncStatus_Superseded(SyncDisciplineGuid, userGuid)
                    {
                        type = "Project Auth",
                        syncItemName = Common.ConvertProjectGuidToName(drUserProjFrom.PROJECTGUID),
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drUserProjFrom.CREATED,
                        CreatedBy = drUserProjFrom.CREATEDBY,
                        UpdatedDate = DateTime.MinValue,
                        UpdatedBy = Guid.Empty,
                        DeletedDate = drUserProjFrom.IsDELETEDNull() ? DateTime.MinValue : drUserProjFrom.DELETED,
                        DeletedBy = drUserProjFrom.IsDELETEDBYNull() ? Guid.Empty : drUserProjFrom.DELETEDBY
                    });

                    if (drUserProjFrom.PROJECTGUID == defaultProject)
                    {
                        UpdateSyncItem(SyncDisciplineGuid, Common.ConvertProjectGuidToName(drUserProjFrom.PROJECTGUID), string.Empty, "New", syncStatus);
                        continue; //skip default project because it'll be added by trigger
                    }

                    dsUSER_MAIN.USER_PROJECTRow drUserProjTo = listUserProjTo.FirstOrDefault(obj => obj.PROJECTGUID == drUserProjFrom.PROJECTGUID);
                    if(drUserProjTo == null)
                    {
                        dsUSER_MAIN.USER_PROJECTRow drUserProjNew = dsUSERMAIN.USER_PROJECT.NewUSER_PROJECTRow();
                        drUserProjNew.ItemArray = drUserProjFrom.ItemArray;
                        drUserProjNew.GUID = Guid.NewGuid(); //to prevent bumping into previously synced but deleted guid
                        dsUSERMAIN.USER_PROJECT.AddUSER_PROJECTRow(drUserProjNew);
                        daUserTo.Save(drUserProjNew);
                        UpdateSyncItem(SyncDisciplineGuid, Common.ConvertProjectGuidToName(drUserProjNew.PROJECTGUID), string.Empty, "New", syncStatus);
                    }
                    else//else don't do anything if the authorisation already exists
                        UpdateSyncItem(SyncDisciplineGuid, Common.ConvertProjectGuidToName(drUserProjFrom.PROJECTGUID), string.Empty, "No Change", syncStatus);
                }

                //Cross checking to trim unused in 'from' table
                foreach(dsUSER_MAIN.USER_PROJECTRow drUserProjTo in listUserProjTo)
                {
                    if(!dtUserProjFrom.Any(obj => obj.PROJECTGUID == drUserProjTo.PROJECTGUID))
                    {
                        _SyncItems.Add(new SyncStatus_Superseded(Guid.NewGuid(), userGuid)
                        {
                            syncItemName = Common.ConvertProjectGuidToName(drUserProjTo.PROJECTGUID),
                            type = "Project Auth",
                            status = "Delete",
                            SyncDate = DateTime.Now,
                            SyncBy = System_Environment.GetUser().GUID,
                            CreatedDate = drUserProjTo.CREATED,
                            CreatedBy = drUserProjTo.CREATEDBY,
                            UpdatedDate = DateTime.MinValue,
                            UpdatedBy = Guid.Empty,
                            DeletedDate = drUserProjTo.IsDELETEDNull() ? DateTime.MinValue : drUserProjTo.DELETED,
                            DeletedBy = drUserProjTo.IsDELETEDBYNull() ? Guid.Empty : drUserProjTo.DELETEDBY
                        });

                        daUserTo.RemoveUserProject(userGuid, drUserProjTo.PROJECTGUID);
                    }
                }
            }
        }

        private void Sync_Privilege(AdapterROLE_MAIN daRoleFrom, AdapterROLE_MAIN daRoleTo, Guid roleGuid, string roleName, SyncStatusDisplay syncStatus)
        {
            dsROLE_MAIN dsROLEMAIN = new dsROLE_MAIN();
            dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtPrivilegeFrom = daRoleFrom.GetPrivilegeBy(roleGuid);
            List<dsROLE_MAIN.ROLE_PRIVILEGERow> listPrivilegeTo = new List<dsROLE_MAIN.ROLE_PRIVILEGERow>();
            dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtPrivilegeTo = daRoleTo.GetPrivilegeBy(roleGuid);
            if (dtPrivilegeTo != null)
                listPrivilegeTo = dtPrivilegeTo.ToList();

            if(dtPrivilegeFrom != null)
            {
                foreach(dsROLE_MAIN.ROLE_PRIVILEGERow drPrivilegeFrom in dtPrivilegeFrom.Rows)
                {
                    Guid SyncPrivilegeGuid = Guid.NewGuid();

                    _SyncItems.Add(new SyncStatus_Superseded(SyncPrivilegeGuid, roleGuid)
                    {
                        syncItemName = drPrivilegeFrom.NAME,
                        additionalInfo = roleName,
                        type = Sync_Type_Superseded.Privilege.ToString(),
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drPrivilegeFrom.CREATED,
                        CreatedBy = drPrivilegeFrom.CREATEDBY,
                        UpdatedDate = DateTime.MinValue,
                        UpdatedBy = Guid.Empty,
                        DeletedDate = drPrivilegeFrom.IsDELETEDNull() ? DateTime.MinValue : drPrivilegeFrom.DELETED,
                        DeletedBy = drPrivilegeFrom.IsDELETEDBYNull() ? Guid.Empty : drPrivilegeFrom.DELETEDBY
                    });

                    dsROLE_MAIN.ROLE_PRIVILEGERow drPrivilegeTo = listPrivilegeTo.FirstOrDefault(obj => obj.TYPEID == drPrivilegeFrom.TYPEID);
                    if(drPrivilegeTo == null)
                    {
                        dsROLE_MAIN.ROLE_PRIVILEGERow drPrivilegeNew = dsROLEMAIN.ROLE_PRIVILEGE.NewROLE_PRIVILEGERow();
                        drPrivilegeNew.ItemArray = drPrivilegeFrom.ItemArray;
                        drPrivilegeNew.GUID = Guid.NewGuid(); //prevent bumping into previously synced but deleted guid
                        dsROLEMAIN.ROLE_PRIVILEGE.AddROLE_PRIVILEGERow(drPrivilegeNew);
                        daRoleTo.Save(drPrivilegeNew);
                        UpdateSyncItem(SyncPrivilegeGuid, string.Empty, string.Empty, "New", syncStatus);
                    }
                    else
                        UpdateSyncItem(SyncPrivilegeGuid, string.Empty, string.Empty, "No Change", syncStatus);
                }

                //Cross checking to trim unused in 'from' table
                foreach(dsROLE_MAIN.ROLE_PRIVILEGERow drPrivilegeTo in listPrivilegeTo)
                {
                    if(!dtPrivilegeFrom.Any(obj => obj.NAME == drPrivilegeTo.TYPEID))
                    {
                        _SyncItems.Add(new SyncStatus_Superseded(drPrivilegeTo.GUID, roleGuid)
                        {
                            syncItemName = drPrivilegeTo.NAME,
                            additionalInfo = roleName,
                            type = Sync_Type_Superseded.Privilege.ToString(),
                            status = "Delete",
                            SyncDate = DateTime.Now,
                            SyncBy = System_Environment.GetUser().GUID,
                            CreatedDate = drPrivilegeTo.CREATED,
                            CreatedBy = drPrivilegeTo.CREATEDBY,
                            UpdatedDate = DateTime.MinValue,
                            UpdatedBy = Guid.Empty,
                            DeletedDate = drPrivilegeTo.IsDELETEDNull() ? DateTime.MinValue : drPrivilegeTo.DELETED,
                            DeletedBy = drPrivilegeTo.IsDELETEDBYNull() ? Guid.Empty : drPrivilegeTo.DELETEDBY
                        });

                        daRoleTo.RemoveRolePrivilege(roleGuid, drPrivilegeTo.TYPEID);
                    }
                }
            }
        }

        /// <summary>
        /// Syncs project WBS
        /// </summary>
        /// <param name="syncMode"></param>
        private void Sync_WBS(Sync_Mode syncMode, Guid projectGuid, Guid scheduleGuid, string scheduleName, bool includeDeletes)
        {
            dsWBS dsWBSMAIN = new dsWBS();
            SyncStatusDisplay syncStatus = SyncStatusDisplay.upload; //set upload as default

            if (syncMode == Sync_Mode.None)
                return;
            else
            {
                AdapterWBS daWbsFrom;
                AdapterWBS daWbsTo;
                AdapterTEMPLATE_REGISTER daRegisterFrom;
                AdapterTEMPLATE_REGISTER daRegisterTo;
                AdapterPREFILL_REGISTER daPrefillRegFrom;
                AdapterPREFILL_REGISTER daPrefillRegTo;

                if (syncMode == Sync_Mode.Pull)
                {
                    daWbsFrom = _daWBSLocal; //when server is set to pull, it will be pulling from local db
                    daWbsTo = _daWBSRemote;

                    daRegisterFrom = _daRegisterLocal;
                    daRegisterTo = _daRegisterRemote;
                    daPrefillRegFrom = _daPrefillRegLocal;
                    daPrefillRegTo = _daPrefillRegRemote;
                }
                else
                {
                    daWbsFrom = _daWBSRemote;
                    daWbsTo = _daWBSLocal;
                    syncStatus = SyncStatusDisplay.download;

                    daRegisterFrom = _daRegisterRemote;
                    daRegisterTo = _daRegisterLocal;
                    daPrefillRegFrom = _daPrefillRegRemote;
                    daPrefillRegTo = _daPrefillRegLocal;
                }

                dsWBS.WBSDataTable dtWbsFrom;
                if (includeDeletes)
                    dtWbsFrom = daWbsFrom.GetByScheduleIncludingDeleted(scheduleGuid);
                else
                {
                    //dtWbsFrom = daWbsFrom.GetByProject(scheduleGuid);
                    dtWbsFrom = daWbsFrom.GetByProject(_projectGuid);
                }

                if (dtWbsFrom == null)
                    return;

                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Syncing WBS for Schedule " + scheduleName + " ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtWbsFrom.Rows.Count);

                foreach (dsWBS.WBSRow drWbsFrom in dtWbsFrom.Rows)
                {
                    Guid SyncWBSGuid = drWbsFrom.GUID;
                    Guid SyncWBSParentGuid = drWbsFrom.PARENTGUID;
                    if (SyncWBSParentGuid == Guid.Empty)
                        SyncWBSParentGuid = scheduleGuid;

                    _SyncItems.Add(new SyncStatus_Superseded(SyncWBSGuid, SyncWBSParentGuid)
                    {
                        syncItemName = drWbsFrom.NAME,
                        additionalInfo = scheduleName,
                        type = Sync_Type_Superseded.WBS.ToString(),
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drWbsFrom.CREATED,
                        CreatedBy = drWbsFrom.CREATEDBY,
                        UpdatedDate = drWbsFrom.IsUPDATEDNull() ? DateTime.MinValue : drWbsFrom.UPDATED,
                        UpdatedBy = drWbsFrom.IsUPDATEDBYNull() ? Guid.Empty : drWbsFrom.UPDATEDBY,
                        DeletedDate = drWbsFrom.IsDELETEDNull() ? DateTime.MinValue : drWbsFrom.DELETED,
                        DeletedBy = drWbsFrom.IsDELETEDBYNull() ? Guid.Empty : drWbsFrom.DELETEDBY
                    });

                    //perfect match with guid
                    dsWBS.WBSRow drWbsTo = daWbsTo.GetIncludeDeletedBy(drWbsFrom.GUID);
                    if (drWbsTo != null)
                    {
                        //to make sure this is the only wbs with this name
                        daWbsTo.RemoveAllWithExclusionBy(drWbsFrom.NAME, projectGuid, drWbsFrom.GUID);

                        if (DataRowComparer.Default.Equals(drWbsTo, drWbsFrom))
                            UpdateSyncItem(SyncWBSGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        else
                        {
                            drWbsTo.ItemArray = drWbsFrom.ItemArray;
                            daWbsTo.Save(drWbsTo);

                            if(!drWbsTo.IsDELETEDNull())
                                UpdateSyncItem(SyncWBSGuid, string.Empty, string.Empty, "Delete", syncStatus);
                            else
                            {
                                Sync_WBSTagRegister(daRegisterFrom, daRegisterTo, drWbsTo.GUID, drWbsTo.NAME, syncStatus);
                                Sync_PrefillRegister(daPrefillRegFrom, daPrefillRegTo, drWbsFrom.GUID, drWbsFrom.NAME, syncStatus);
                                UpdateSyncItem(SyncWBSGuid, string.Empty, string.Empty, "Update", syncStatus);
                            } 
                        }
                    }
                    //partial match with wbs name
                    else
                    {
                        drWbsTo = daWbsTo.GetBy(drWbsFrom.NAME, projectGuid); //Fix 12-DEC-2014: WBS goes by name and project GUID
                        if (drWbsTo != null)
                        {
                            //if there is existing match with wbs name, let user resolve the conflict because there might be existing users using the wbs
                            if(!drWbsTo.IsDELETEDNull())
                                UpdateSyncItem(SyncWBSGuid, string.Empty, string.Empty, "Conflict", SyncStatusDisplay.warning);
                            else
                                UpdateSyncItem(SyncWBSGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        }
                        else
                        {
                            //if wbs name isn't found, record can be added safely
                            dsWBS.WBSRow drNewWbs = dsWBSMAIN.WBS.NewWBSRow();
                            drNewWbs.ItemArray = drWbsFrom.ItemArray;
                            dsWBSMAIN.WBS.AddWBSRow(drNewWbs);
                            daWbsTo.Save(drNewWbs);
                            Sync_WBSTagRegister(daRegisterFrom, daRegisterTo, drNewWbs.GUID, drNewWbs.NAME, syncStatus);
                            Sync_PrefillRegister(daPrefillRegFrom, daPrefillRegTo, drWbsFrom.GUID, drWbsFrom.NAME, syncStatus);

                            if (!drNewWbs.IsDELETEDNull())
                                UpdateSyncItem(SyncWBSGuid, string.Empty, string.Empty, "New Deleted", syncStatus);
                            else
                                UpdateSyncItem(SyncWBSGuid, string.Empty, string.Empty, "New", syncStatus);
                        }
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();
            }
        }

        /// <summary>
        /// Syncs project Tag
        /// </summary>
        /// <param name="syncMode"></param>
        private void Sync_Tag(Sync_Mode syncMode, Guid projectGuid, Guid scheduleGuid, string scheduleName, bool includeDeletes)
        {
            dsTAG dsTAGMAIN = new dsTAG();
            SyncStatusDisplay syncStatus = SyncStatusDisplay.upload; //set upload as default

            if (syncMode == Sync_Mode.None)
                return;
            else
            {
                AdapterTAG daTagFrom;
                AdapterTAG daTagTo;

                AdapterTEMPLATE_REGISTER daRegisterFrom;
                AdapterTEMPLATE_REGISTER daRegisterTo;

                AdapterPREFILL_REGISTER daPrefillRegFrom;
                AdapterPREFILL_REGISTER daPrefillRegTo;

                if (syncMode == Sync_Mode.Pull)
                {
                    daTagFrom = _daTagLocal; //when server is set to pull, it will be pulling from local db
                    daTagTo = _daTagRemote;
                    daRegisterFrom = _daRegisterLocal;
                    daRegisterTo = _daRegisterRemote;
                    daPrefillRegFrom = _daPrefillRegLocal;
                    daPrefillRegTo = _daPrefillRegRemote;
                }
                else
                {
                    daTagFrom = _daTagRemote;
                    daTagTo = _daTagLocal;
                    syncStatus = SyncStatusDisplay.download;

                    daRegisterFrom = _daRegisterRemote;
                    daRegisterTo = _daRegisterLocal;
                    daPrefillRegFrom = _daPrefillRegRemote;
                    daPrefillRegTo = _daPrefillRegLocal;
                }

                dsTAG.TAGDataTable dtTagFrom;
                if (includeDeletes)
                    dtTagFrom = daTagFrom.GetByScheduleIncludeDeleted(scheduleGuid);
                else
                    dtTagFrom = daTagFrom.GetBySchedule(scheduleGuid);

                if (dtTagFrom == null)
                    return;

                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Syncing Tag for Schedule " + scheduleName + " ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtTagFrom.Rows.Count);

                foreach (dsTAG.TAGRow drTagFrom in dtTagFrom.Rows)
                {
                    Guid SyncTagGuid = drTagFrom.GUID;
                    Guid SyncTagParentGuid = drTagFrom.PARENTGUID;
                    if (SyncTagParentGuid == Guid.Empty)
                        SyncTagParentGuid = scheduleGuid;

                    _SyncItems.Add(new SyncStatus_Superseded(SyncTagGuid, SyncTagParentGuid)
                    {
                        syncItemName = drTagFrom.NUMBER,
                        additionalInfo = scheduleName,
                        type = Sync_Type_Superseded.Tag.ToString(),
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drTagFrom.CREATED,
                        CreatedBy = drTagFrom.CREATEDBY,
                        UpdatedDate = drTagFrom.IsUPDATEDNull() ? DateTime.MinValue : drTagFrom.UPDATED,
                        UpdatedBy = drTagFrom.IsUPDATEDBYNull() ? Guid.Empty : drTagFrom.UPDATEDBY,
                        DeletedDate = drTagFrom.IsDELETEDNull() ? DateTime.MinValue : drTagFrom.DELETED,
                        DeletedBy = drTagFrom.IsDELETEDBYNull() ? Guid.Empty : drTagFrom.DELETEDBY
                    });

                    //perfect match with guid
                    dsTAG.TAGRow drTagTo = daTagTo.GetIncludeDeletedBy(drTagFrom.GUID);
                    if (drTagTo != null)
                    {
                        //to make sure this is the only tag with this name
                        //10 Dec 2014 changes - tag number is unique only to project instead of the entire system
                        daTagTo.RemoveAllWithExclusionBy(drTagFrom.NUMBER, projectGuid, drTagFrom.GUID);

                        if(DataRowComparer.Default.Equals(drTagTo, drTagFrom))
                            UpdateSyncItem(SyncTagGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        else
                        {
                            drTagTo.ItemArray = drTagFrom.ItemArray;
                            daTagTo.Save(drTagTo);

                            if(!drTagTo.IsDELETEDNull())
                                UpdateSyncItem(SyncTagGuid, string.Empty, string.Empty, "Delete", syncStatus);
                            else
                            {
                                Sync_WBSTagRegister(daRegisterFrom, daRegisterTo, drTagTo.GUID, drTagTo.NUMBER, syncStatus);
                                Sync_PrefillRegister(daPrefillRegFrom, daPrefillRegTo, drTagFrom.GUID, drTagFrom.NUMBER, syncStatus);
                                UpdateSyncItem(SyncTagGuid, string.Empty, string.Empty, "Update", syncStatus);
                            }
                        }
                    }
                    //partial match with tag name and project
                    else
                    {
                        drTagTo = daTagTo.GetBy(drTagFrom.NUMBER, projectGuid);
                        if (drTagTo != null)
                        {
                            //if there is existing match with tag name, let user resolve the conflict because there might be existing users using the tag
                            if(!drTagTo.IsDELETEDNull())
                                UpdateSyncItem(SyncTagGuid, string.Empty, string.Empty, "Conflict", SyncStatusDisplay.warning);
                            else
                                UpdateSyncItem(SyncTagGuid, string.Empty, string.Empty, "No Change", SyncStatusDisplay.same);
                        }
                        else
                        {
                            //if tag name isn't found, record can be added safely
                            dsTAG.TAGRow drNewTag = dsTAGMAIN.TAG.NewTAGRow();
                            drNewTag.ItemArray = drTagFrom.ItemArray;
                            dsTAGMAIN.TAG.AddTAGRow(drNewTag);
                            daTagTo.Save(drNewTag);
                            Sync_WBSTagRegister(daRegisterFrom, daRegisterTo, drNewTag.GUID, drNewTag.NUMBER, syncStatus);
                            Sync_PrefillRegister(daPrefillRegFrom, daPrefillRegTo, drTagFrom.GUID, drTagFrom.NUMBER, syncStatus);

                            if (!drNewTag.IsDELETEDNull())
                                UpdateSyncItem(SyncTagGuid, string.Empty, string.Empty, "New Deleted", syncStatus);
                            else
                                UpdateSyncItem(SyncTagGuid, string.Empty, string.Empty, "New", syncStatus);
                        }
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();
            }
        }

        /// <summary>
        /// Syncs the register
        /// </summary>
        private void Sync_WBSTagRegister(AdapterTEMPLATE_REGISTER daRegisterFrom, AdapterTEMPLATE_REGISTER daRegisterTo, Guid wbsTagGuid, string wbsTagName, SyncStatusDisplay syncStatus)
        {
            dsTEMPLATE_REGISTER dsRegister = new dsTEMPLATE_REGISTER();
            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtRegisterFrom = daRegisterFrom.GetByWBSTagGuid(wbsTagGuid);
            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtRegisterTo = daRegisterTo.GetByWBSTagGuid(wbsTagGuid);
            List<dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow> listRegisterTo = new List<dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow>();
            if(dtRegisterTo != null)
                listRegisterTo = daRegisterTo.GetByWBSTagGuid(wbsTagGuid).ToList();

            if(dtRegisterFrom != null)
            {
                foreach(dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drRegisterFrom in dtRegisterFrom.Rows)
                {
                    Guid SyncRegisterGuid = Guid.NewGuid();

                    _SyncItems.Add(new SyncStatus_Superseded(SyncRegisterGuid, wbsTagGuid)
                    {
                        syncItemName = wbsTagName,
                        type = "Register",
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drRegisterFrom.CREATED,
                        CreatedBy = drRegisterFrom.CREATEDBY,
                        UpdatedDate = DateTime.MinValue,
                        UpdatedBy = Guid.Empty,
                        DeletedDate = drRegisterFrom.IsDELETEDNull() ? DateTime.MinValue : drRegisterFrom.DELETED,
                        DeletedBy = drRegisterFrom.IsDELETEDBYNull() ? Guid.Empty : drRegisterFrom.DELETEDBY
                    });

                    dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drRegisterTo = listRegisterTo.FirstOrDefault(obj => ((!obj.IsTAG_GUIDNull() && obj.TAG_GUID == drRegisterFrom.TAG_GUID) || (!obj.IsWBS_GUIDNull() && obj.WBS_GUID == drRegisterFrom.WBS_GUID)) && obj.TEMPLATE_GUID == drRegisterFrom.TEMPLATE_GUID);
                
                    if(drRegisterTo == null)
                    {
                        dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drRegisterNew = dsRegister.TEMPLATE_REGISTER.NewTEMPLATE_REGISTERRow();
                        drRegisterNew.ItemArray = drRegisterFrom.ItemArray;
                        drRegisterNew.GUID = Guid.NewGuid();
                        dsRegister.TEMPLATE_REGISTER.AddTEMPLATE_REGISTERRow(drRegisterNew);
                        daRegisterTo.Save(drRegisterNew);
                        UpdateSyncItem(SyncRegisterGuid, Common.ConvertTemplateGuidToName(drRegisterNew.TEMPLATE_GUID), string.Empty, "New", syncStatus);
                    }
                    else //else don't do anything if the wbs/tag and template combination already exists
                        UpdateSyncItem(SyncRegisterGuid, Common.ConvertTemplateGuidToName(drRegisterTo.TEMPLATE_GUID), string.Empty, "No Change", syncStatus);
                }
            }


            //trim unregister combination from register from in register to
            foreach (dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drRegisterTo in listRegisterTo)
            {
                Guid SyncRegisterGuid = Guid.NewGuid();

                if(dtRegisterFrom != null)
                {
                    if (!dtRegisterFrom.Any(obj => ((!obj.IsTAG_GUIDNull() && obj.TAG_GUID == drRegisterTo.TAG_GUID) || (!obj.IsWBS_GUIDNull() && obj.WBS_GUID == drRegisterTo.WBS_GUID)) && obj.TEMPLATE_GUID == drRegisterTo.TEMPLATE_GUID))
                    {
                        _SyncItems.Add(new SyncStatus_Superseded(SyncRegisterGuid, wbsTagGuid)
                        {
                            syncItemName = wbsTagName,
                            additionalInfo = Common.ConvertTemplateGuidToName(drRegisterTo.TEMPLATE_GUID),
                            type = "Register",
                            status = "Delete",
                            SyncDate = DateTime.Now,
                            SyncBy = System_Environment.GetUser().GUID,
                            CreatedDate = drRegisterTo.CREATED,
                            CreatedBy = drRegisterTo.CREATEDBY,
                            UpdatedDate = DateTime.MinValue,
                            UpdatedBy = Guid.Empty,
                            DeletedDate = drRegisterTo.IsDELETEDNull() ? DateTime.MinValue : drRegisterTo.DELETED,
                            DeletedBy = drRegisterTo.IsDELETEDBYNull() ? Guid.Empty : drRegisterTo.DELETEDBY
                        });

                        if (!drRegisterTo.IsTAG_GUIDNull())
                        {
                            daRegisterTo.UntagTemplateByGuid(drRegisterTo.TAG_GUID, drRegisterTo.TEMPLATE_GUID);
                            UpdateSyncItem(SyncRegisterGuid, string.Empty, string.Empty, "Delete", syncStatus);
                        }
                        else if (!drRegisterTo.IsWBS_GUIDNull())
                        {
                            daRegisterTo.UntagTemplateByGuid(drRegisterTo.WBS_GUID, drRegisterTo.TEMPLATE_GUID);
                            UpdateSyncItem(SyncRegisterGuid, string.Empty, string.Empty, "Delete", syncStatus);
                        }
                    }
                }
                else
                {
                    _SyncItems.Add(new SyncStatus_Superseded(SyncRegisterGuid, wbsTagGuid)
                    {
                        syncItemName = wbsTagName,
                        additionalInfo = Common.ConvertTemplateGuidToName(drRegisterTo.TEMPLATE_GUID),
                        type = "Register",
                        status = "Delete",
                        SyncDate = DateTime.Now,
                        SyncBy = System_Environment.GetUser().GUID,
                        CreatedDate = drRegisterTo.CREATED,
                        CreatedBy = drRegisterTo.CREATEDBY,
                        UpdatedDate = DateTime.MinValue,
                        UpdatedBy = Guid.Empty,
                        DeletedDate = drRegisterTo.IsDELETEDNull() ? DateTime.MinValue : drRegisterTo.DELETED,
                        DeletedBy = drRegisterTo.IsDELETEDBYNull() ? Guid.Empty : drRegisterTo.DELETEDBY
                    });

                    if(!drRegisterTo.IsTAG_GUIDNull())
                        daRegisterTo.UntagTemplateByGuid(drRegisterTo.TAG_GUID, drRegisterTo.TEMPLATE_GUID);
                    else if(!drRegisterTo.IsWBS_GUIDNull())
                        daRegisterTo.UntagTemplateByGuid(drRegisterTo.WBS_GUID, drRegisterTo.TEMPLATE_GUID);
                    
                    UpdateSyncItem(SyncRegisterGuid, string.Empty, string.Empty, "Delete", syncStatus);
                }
            }
        }
        #endregion

        #region Initialisation
        /// <summary>
        /// Initialize the remote adapters
        /// </summary>
        /// <param name="connStr"></param>
        private void RemoteAdapterInitialisation(string remoteConnStr)
        {
            _daTagRemote = new AdapterTAG(remoteConnStr);
            _daWBSRemote = new AdapterWBS(remoteConnStr);
            _daITRRemote = new AdapterITR_MAIN(remoteConnStr);
            _daITRStatusRemote = new AdapterITR_STATUS(remoteConnStr);
            _daITRIssueRemote = new AdapterITR_STATUS_ISSUE(remoteConnStr);
            _daPunchlistRemote = new AdapterPUNCHLIST_MAIN(remoteConnStr);
            _daPunchlistStatusRemote = new AdapterPUNCHLIST_STATUS(remoteConnStr);
            _daPunchlistIssueRemote = new AdapterPUNCHLIST_STATUS_ISSUE(remoteConnStr);
            _daSyncPairRemote = new AdapterSYNC_PAIR(remoteConnStr);
            _daSyncTableRemote = new AdapterSYNC_TABLE(remoteConnStr);
            _daUserRemote = new AdapterUSER_MAIN(remoteConnStr);
            _daRoleRemote = new AdapterROLE_MAIN(remoteConnStr);
            _daTemplateRemote = new AdapterTEMPLATE_MAIN(remoteConnStr);
            _daRegisterRemote = new AdapterTEMPLATE_REGISTER(remoteConnStr);
            _daPrefillRemote = new AdapterPREFILL_MAIN(remoteConnStr);
            _daPrefillRegRemote = new AdapterPREFILL_REGISTER(remoteConnStr);
            _daWorkflowRemote = new AdapterWORKFLOW_MAIN(remoteConnStr);
            _daProjectRemote = new AdapterPROJECT(remoteConnStr);
            _daScheduleRemote = new AdapterSCHEDULE(remoteConnStr);
            _daSyncHistoryRemote = new AdapterSYNC_HISTORY(remoteConnStr);
            _daEquipmentRemote = new AdapterGENERAL_EQUIPMENT(remoteConnStr);
        }

        /// <summary>
        /// Establish the sync environment, also allows superuser to select environment parameters
        /// </summary>
        private bool EstablishEnvironmentParameters()
        {
            // To cater for instances where database is fresh
            using(AdapterPROJECT daProject = new AdapterPROJECT())
            {
                dsPROJECT.PROJECTDataTable dtProject = daProject.Get(); //Try to get project from local datatable
                if (dtProject == null)
                {
                    _projectGuid = Guid.Empty;
                    //Changes 7-May-2015 : Sync tag and wbs by entire project
                    //_discipline = string.Empty;
                    return true;
                }
            }

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
                    return false;
                }

                //Changes 7-May-2015 : Sync tag and wbs by entire project
                //frmITR_SelectDiscipline frmSelectDiscipline = new frmITR_SelectDiscipline();

                //if (frmSelectDiscipline.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                //{
                //    _discipline = frmSelectDiscipline.GetDiscipline();
                //}
                //else
                //{
                //    this.Close();
                //    return false;
                //}

                return true;
            }

            return true;
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Checks the sync mode for particular sync type
        /// </summary>
        /// <param name="syncType"></param>
        /// <returns></returns>
        private Sync_Mode CheckSync(Sync_Type_Superseded syncType, out bool includeDeletes)
        {
            dsSYNC_TABLE.SYNC_TABLERow drSyncTable = _daSyncTableRemote.GetBy(_localSyncGuid, syncType);
            string returnSyncMode = Sync_Mode.None.ToString();
            includeDeletes = false;

            if (drSyncTable != null && drSyncTable.SYNC_MODE != Sync_Mode.None.ToString())
            {
                returnSyncMode = drSyncTable.SYNC_MODE; //remember the sync mode before changing it
                includeDeletes = drSyncTable.DELETES;

                if (drSyncTable.ONETIME)
                {
                    drSyncTable.SYNC_MODE = Sync_Mode.None.ToString();
                    _daSyncTableRemote.Save(drSyncTable);
                }

                return (Sync_Mode)Enum.Parse(typeof(Sync_Mode), returnSyncMode);
            }
            else
                return Sync_Mode.None;
        }

        /// <summary>
        /// Checks whether sync was approved for this machine
        /// </summary>
        /// <returns></returns>
        private bool CheckSyncApproval()
        {
            dsSYNC_PAIR.SYNC_PAIRRow drSyncPair = _daSyncPairRemote.GetBy(_HWID);
            if (drSyncPair != null && drSyncPair.APPROVED)
            {
                _localSyncGuid = drSyncPair.GUID;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Searches attachment by name and return Guid
        /// </summary>
        private Guid isTagWBSExist(AdapterTAG daTag, AdapterWBS daWBS, string attachmentName, Guid projectGuid)
        {
            if (daWBS != null)
            {
                dsWBS.WBSRow drWBS = daWBS.GetBy(attachmentName, projectGuid);
                if (drWBS != null)
                    return drWBS.GUID;
            }
            else if (daTag != null)
            {
                dsTAG.TAGRow drTag = daTag.GetBy(attachmentName, projectGuid); //Changes 10-DEC-2014 Allow same tag number to be added per project
                if (drTag != null)
                    return drTag.GUID;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Replaces local status/issue with remote status/issue
        /// </summary>
        private void CopyITRStatusAndIssue(Guid MasterITRGuid, Guid SlaveITRGuid, AdapterITR_STATUS daMasterStatus, AdapterITR_STATUS daSlaveStatus
            , AdapterITR_STATUS_ISSUE daMasterIssue, AdapterITR_STATUS_ISSUE daSlaveIssue)
        {
            dsITR_STATUS dsITRStatus = new dsITR_STATUS();
            dsITR_STATUS_ISSUE dsITRIssue = new dsITR_STATUS_ISSUE();

            daSlaveStatus.RemoveBy(SlaveITRGuid); //this will delete the issues as well because of foreign key constraints

            dsITR_STATUS.ITR_STATUSDataTable dtITRMasterStatus = daMasterStatus.GetBy(MasterITRGuid);
            if (dtITRMasterStatus != null)
            {
                int statusCounter = 1;

                foreach (dsITR_STATUS.ITR_STATUSRow drITRMasterStatus in dtITRMasterStatus.Rows)
                {
                    Guid newStatusGuid = Guid.NewGuid();
                    dsITR_STATUS.ITR_STATUSRow drNewITRSlaveStatus = dsITRStatus.ITR_STATUS.NewITR_STATUSRow();
                    drNewITRSlaveStatus.ItemArray = drITRMasterStatus.ItemArray;
                    drNewITRSlaveStatus.GUID = newStatusGuid;
                    drNewITRSlaveStatus.SEQUENCE_NUMBER = statusCounter;
                    drNewITRSlaveStatus.ITR_MAIN_GUID = SlaveITRGuid;
                    dsITRStatus.ITR_STATUS.AddITR_STATUSRow(drNewITRSlaveStatus);
                    daSlaveStatus.Save(drNewITRSlaveStatus);

                    dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtITRMasterIssue = daMasterIssue.GetBy(drITRMasterStatus.GUID);
                    if (dtITRMasterIssue != null)
                    {
                        int issueCounter = 1;
                        foreach (dsITR_STATUS_ISSUE.ITR_STATUS_ISSUERow drITRMasterIssue in dtITRMasterIssue.Rows)
                        {
                            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUERow drITRSlaveIssue = dsITRIssue.ITR_STATUS_ISSUE.NewITR_STATUS_ISSUERow();
                            drITRSlaveIssue.ItemArray = drITRMasterIssue.ItemArray;
                            drITRSlaveIssue.GUID = Guid.NewGuid();
                            drITRSlaveIssue.SEQUENCE_NUMBER = issueCounter;
                            drITRSlaveIssue.ITR_STATUS_GUID = newStatusGuid;
                            dsITRIssue.ITR_STATUS_ISSUE.AddITR_STATUS_ISSUERow(drITRSlaveIssue);
                            daSlaveIssue.Save(drITRSlaveIssue);
                        }
                    }

                    statusCounter++;
                }
            }
        }

        /// <summary>
        /// Replaces local status/issue with remote status/issue
        /// </summary>
        private void CopyPunchlistStatusAndIssue(Guid MasterPunchlistGuid, Guid SlavePunchlistGuid, AdapterPUNCHLIST_STATUS daMasterStatus, AdapterPUNCHLIST_STATUS daSlaveStatus
            , AdapterPUNCHLIST_STATUS_ISSUE daMasterIssue, AdapterPUNCHLIST_STATUS_ISSUE daSlaveIssue)
        {
            dsPUNCHLIST_STATUS dsPunchlistStatus = new dsPUNCHLIST_STATUS();
            dsPUNCHLIST_STATUS_ISSUE dsPunchlistIssue = new dsPUNCHLIST_STATUS_ISSUE();

            daSlaveStatus.RemoveBy(SlavePunchlistGuid); //this will delete the issues as well because of foreign key constraints

            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPunchlistMasterStatus = daMasterStatus.GetBy(MasterPunchlistGuid);
            if (dtPunchlistMasterStatus != null)
            {
                int statusCounter = 1;

                foreach (dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow drPunchlistMasterStatus in dtPunchlistMasterStatus.Rows)
                {
                    Guid newStatusGuid = Guid.NewGuid();
                    dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow drNewPunchlistSlaveStatus = dsPunchlistStatus.PUNCHLIST_STATUS.NewPUNCHLIST_STATUSRow();
                    
                    drNewPunchlistSlaveStatus.ItemArray = drPunchlistMasterStatus.ItemArray;
                    drNewPunchlistSlaveStatus.GUID = newStatusGuid;
                    drNewPunchlistSlaveStatus.SEQUENCE_NUMBER = statusCounter;
                    //drNewPunchlistSlaveStatus.PUNCHLIST_MAIN_GUID = SlavePunchlistGuid; //Changes 15th Dec 2014 full GUID sync with master
                    drNewPunchlistSlaveStatus.PUNCHLIST_MAIN_GUID = MasterPunchlistGuid; //Changes 15th Dec 2014 full GUID sync with master
                    dsPunchlistStatus.PUNCHLIST_STATUS.AddPUNCHLIST_STATUSRow(drNewPunchlistSlaveStatus);
                    daSlaveStatus.Save(drNewPunchlistSlaveStatus);

                    dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable dtPunchlistMasterIssue = daMasterIssue.GetBy(drPunchlistMasterStatus.GUID);
                    if (dtPunchlistMasterIssue != null)
                    {
                        int issueCounter = 1;
                        foreach (dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUERow drPunchlistMasterIssue in dtPunchlistMasterIssue.Rows)
                        {
                            dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUERow drPunchlistSlaveIssue = dsPunchlistIssue.PUNCHLIST_STATUS_ISSUE.NewPUNCHLIST_STATUS_ISSUERow();
                            drPunchlistSlaveIssue.ItemArray = drPunchlistMasterIssue.ItemArray;
                            drPunchlistSlaveIssue.GUID = Guid.NewGuid();
                            drPunchlistSlaveIssue.SEQUENCE_NUMBER = issueCounter;
                            drPunchlistSlaveIssue.PUNCHLIST_STATUS_GUID = newStatusGuid;
                            dsPunchlistIssue.PUNCHLIST_STATUS_ISSUE.AddPUNCHLIST_STATUS_ISSUERow(drPunchlistSlaveIssue);
                            daSlaveIssue.Save(drPunchlistSlaveIssue);
                        }
                    }

                    statusCounter++;
                }
            }
        }

        /// <summary>
        /// Read the remote connection string from XML
        /// </summary>
        private string GetConnStrFromXML(out string dbName)
        {
            string xmlFilePath = Common.DatabaseXMLFilePath(false);
            dbName = "N/A";

            if (File.Exists(xmlFilePath))
            {
                try
                {
                    XDocument doc = XDocument.Load(xmlFilePath);
                    XElement findDatabase = doc.Root.Descendants().FirstOrDefault(obj => obj.Name == "Remote");
                    if (findDatabase != null)
                    {
                        string server = findDatabase.Attribute("Url").Value;
                        string database = Common.Decrypt(findDatabase.Attribute("Database").Value, true);
                        string username = Common.Decrypt(findDatabase.Attribute("Username").Value, true);
                        string password = Common.Decrypt(findDatabase.Attribute("Password").Value, true);
                        string connStr = Common.ConstructConnString(server, database, username, password);

                        dbName = server;
                        return connStr;
                    }
                }
                catch
                {

                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Hash check to determine similarity
        /// </summary>
        private bool SameHash(byte[] localHash, byte[] remoteHash)
        {
            bool bEqual = false;
            if (localHash.Length == remoteHash.Length)
            {
                int i = 0;
                while ((i < localHash.Length) && (localHash[i] == remoteHash[i]))
                {
                    i += 1;
                }
                if (i == localHash.Length)
                {
                    bEqual = true;
                }
            }

            return bEqual;
        }

        /// <summary>
        /// Gets the local ITR rating to determine sync direction
        /// </summary>
        private int RateLocalITR(SyncITR iTR)
        {
            if(iTR.isWBS)
                return _daITRStatusLocal.GetStatusByWBS(iTR.attachmentName, iTR.templateName, iTR.projectGuid) + (_daITRIssueLocal.GetWBSRejectionCount(iTR.attachmentName, iTR.templateName, iTR.projectGuid) * 10);
            else
                return _daITRStatusLocal.GetStatusByTag(iTR.attachmentName, iTR.templateName, iTR.projectGuid) + (_daITRIssueLocal.GetTagRejectionCount(iTR.attachmentName, iTR.templateName, iTR.projectGuid) * 10);
        }

        /// <summary>
        /// Gets the remote ITR rating to determine sync direction
        /// </summary>
        private int RateRemoteITR(SyncITR iTR)
        {
            if (iTR.isWBS)
                return _daITRStatusRemote.GetStatusByWBS(iTR.attachmentName, iTR.templateName, iTR.projectGuid) + (_daITRIssueRemote.GetWBSRejectionCount(iTR.attachmentName, iTR.templateName, iTR.projectGuid) * 10);
            else
                return _daITRStatusRemote.GetStatusByTag(iTR.attachmentName, iTR.templateName, iTR.projectGuid) + (_daITRIssueRemote.GetTagRejectionCount(iTR.attachmentName, iTR.templateName, iTR.projectGuid) * 10);    
        }

        /// <summary>
        /// Gets the local punchlist rating to determine sync direction
        /// </summary>
        private int RateLocalPunchlist(SyncPunchlist punchlist)
        {
            if (punchlist.isWBS)
                return _daPunchlistStatusLocal.GetStatusByWBS(punchlist.attachmentName, punchlist.punchlistTitle, punchlist.projectGuid) + (_daPunchlistIssueLocal.GetWBSRejectionCount(punchlist.attachmentName, punchlist.punchlistTitle, punchlist.projectGuid) * 10);
            else
                return _daPunchlistStatusLocal.GetStatusByTag(punchlist.attachmentName, punchlist.punchlistTitle, punchlist.projectGuid) + (_daPunchlistIssueLocal.GetTagRejectionCount(punchlist.attachmentName, punchlist.punchlistTitle, punchlist.projectGuid) * 10);
        }

        /// <summary>
        /// Gets the remote punchlist rate to determine sync direction
        /// </summary>
        private int RateRemotePunchlist(SyncPunchlist punchlist)
        {
            if (punchlist.isWBS)
                return _daPunchlistStatusRemote.GetStatusByWBS(punchlist.attachmentName, punchlist.punchlistTitle, punchlist.projectGuid) + (_daPunchlistIssueRemote.GetWBSRejectionCount(punchlist.attachmentName, punchlist.punchlistTitle, punchlist.projectGuid) * 10);
            else
                return _daPunchlistStatusRemote.GetStatusByTag(punchlist.attachmentName, punchlist.punchlistTitle, punchlist.projectGuid) + (_daPunchlistIssueRemote.GetTagRejectionCount(punchlist.attachmentName, punchlist.punchlistTitle, punchlist.projectGuid) * 10);
        }

        /// <summary>
        /// Loads all the punchlists created for this project and discipline
        /// </summary>
        private List<Punchlist> EstablishProjectDisciplinePunchlist(Guid projectGuid, string discipline)
        {
            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable dtPunchlistMaster = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable();
            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable dtTagPunchlist = _daPunchlistLocal.GetAllTagByProjectDiscipline(projectGuid, discipline);
            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable dtWBSPunchlist = _daPunchlistRemote.GetAllWBSByProjectDiscipline(projectGuid, discipline);
            List<Punchlist> AllPunchlists = new List<Punchlist>();

            if(dtTagPunchlist != null)
            {
                dtPunchlistMaster.Merge(dtTagPunchlist);
                dtTagPunchlist.Dispose();
            }

            if(dtWBSPunchlist != null)
            {
                dtPunchlistMaster.Merge(dtWBSPunchlist);
                dtWBSPunchlist.Dispose();
            }

            List<wbsTagDisplay> WBSTags = GetWBSTag(projectGuid, discipline);
            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable dtPunchlist = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable(); //just to obtain the schema for new row
            foreach(wbsTagDisplay WBSTag in WBSTags)
            {
                var query = from punchlist in dtPunchlistMaster.AsEnumerable()
                            where ((!punchlist.IsWBS_GUIDNull() && punchlist.WBS_GUID == WBSTag.wbsTagDisplayGuid) || (!punchlist.IsTAG_GUIDNull() && punchlist.TAG_GUID == WBSTag.wbsTagDisplayGuid))
                            select punchlist;

                if(query.Any())
                {
                    DataTable dt = query.CopyToDataTable();

                    foreach(DataRow dr in dt.Rows)
                    {
                        dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagRow drPunchlist = dtPunchlist.NewPUNCHLIST_MAINWBSTagRow();
                        drPunchlist.ItemArray = dr.ItemArray;
                        AllPunchlists.Add(Common.CreatePunchlistTagWBS(WBSTag, drPunchlist, true));
                    }
                }
            }

            return AllPunchlists;
        }

        //Changes 7-May-2015 : Sync punchlist by entire project
        //private List<SyncPunchlist> EstablishSyncPunchlist(Guid projectGuid, string discipline)
        private List<SyncPunchlist> EstablishSyncPunchlist(Guid projectGuid)
        {
            List<SyncPunchlist> WBSTagPunchlists = new List<SyncPunchlist>();
            //WBSTagPunchlists = CombineSyncPunchlist(_daPunchlistLocal, WBSTagPunchlists, projectGuid, discipline);
            //WBSTagPunchlists = CombineSyncPunchlist(_daPunchlistRemote, WBSTagPunchlists, projectGuid, discipline);
            WBSTagPunchlists = CombineSyncPunchlist(_daPunchlistLocal, WBSTagPunchlists, projectGuid);
            WBSTagPunchlists = CombineSyncPunchlist(_daPunchlistRemote, WBSTagPunchlists, projectGuid);

            return WBSTagPunchlists;
        }

        /// <summary>
        /// Combine required punchlist for sync using different adapters
        /// </summary>
        //Changes 7-May-2015 : Sync punchlist by entire project
        //private List<SyncPunchlist> CombineSyncPunchlist(AdapterPUNCHLIST_MAIN daPunchlist, List<SyncPunchlist> SyncPunchlist, Guid projectGuid, string discipline)
        private List<SyncPunchlist> CombineSyncPunchlist(AdapterPUNCHLIST_MAIN daPunchlist, List<SyncPunchlist> SyncPunchlist, Guid projectGuid)
        {
            List<SyncPunchlist> tempPunchlists = daPunchlist.GenerateTagSyncByProject(projectGuid);
            foreach(SyncPunchlist tempPunchlist in tempPunchlists)
            {
                if(!SyncPunchlist.Any(obj => obj.attachmentName == tempPunchlist.attachmentName && obj.punchlistTitle == tempPunchlist.punchlistTitle))
                {
                    SyncPunchlist.Add(tempPunchlist);
                }
            }

            tempPunchlists.Clear();
            tempPunchlists = daPunchlist.GenerateWBSSyncByProject(projectGuid);
            foreach (SyncPunchlist tempPunchlist in tempPunchlists)
            {
                if (!SyncPunchlist.Any(obj => obj.attachmentName == tempPunchlist.attachmentName && obj.punchlistTitle == tempPunchlist.punchlistTitle))
                {
                    SyncPunchlist.Add(tempPunchlist);
                }
            }

            return SyncPunchlist;
        }

        /// <summary>
        /// Establish all available ITRs between remote and local for syncing
        /// </summary>
        //Changes 7-May-2015 : Sync tag and wbs by entire project
        //private List<SyncITR> EstablishSyncITRs(Guid projectGuid, string discipline)
        private List<SyncITR> EstablishSyncITRs(Guid projectGuid)
        {
            List<SyncITR> WBSTagITRs = new List<SyncITR>();
            //WBSTagITRs = CombineSyncITR(_daITRLocal, WBSTagITRs, projectGuid, discipline);
            //WBSTagITRs = CombineSyncITR(_daITRRemote, WBSTagITRs, projectGuid, discipline);

            WBSTagITRs = CombineSyncITR(_daITRLocal, WBSTagITRs, projectGuid);
            WBSTagITRs = CombineSyncITR(_daITRRemote, WBSTagITRs, projectGuid);

            return WBSTagITRs;
        }

        /// <summary>
        /// Combine required ITRs for sync using different adapters
        /// </summary>
        // Changes 7-May-2015 : Sync tag and wbs by entire project
        //private List<SyncITR> CombineSyncITR(AdapterITR_MAIN daITR, List<SyncITR> SyncITR, Guid projectGuid, string discipline)
        private List<SyncITR> CombineSyncITR(AdapterITR_MAIN daITR, List<SyncITR> SyncITR, Guid projectGuid)
        {
            //List<SyncITR> tempITRs = daITR.GenerateTagSyncByProjectDiscipline(projectGuid, discipline);
            List<SyncITR> tempITRs = daITR.GenerateTagSyncByProject(projectGuid);
            foreach(SyncITR tempITR in tempITRs)
            {
                if(!SyncITR.Any(obj => obj.attachmentName == tempITR.attachmentName && obj.templateName == tempITR.templateName))
                {
                    SyncITR.Add(tempITR);
                }
            }

            tempITRs.Clear();
            //tempITRs = daITR.GenerateWBSSyncByProjectDiscipline(projectGuid, discipline);
            tempITRs = daITR.GenerateWBSSyncByProject(projectGuid);
            foreach(SyncITR tempITR in tempITRs)
            {
                if (!SyncITR.Any(obj => obj.attachmentName == tempITR.attachmentName && obj.templateName == tempITR.templateName))
                {
                    SyncITR.Add(tempITR);
                }
            }

            return SyncITR;
        }

        /// <summary>
        /// Load all the ITR assigned to this project and discipline
        /// </summary>
        private List<WorkflowTemplateTagWBS> EstablishProjectDisciplineITR(Guid projectGuid, string discipline)
        {
            dsITR_MAIN.ITR_MAINDataTable dtITRMaster = new dsITR_MAIN.ITR_MAINDataTable();
            dsITR_MAIN.ITR_MAINDataTable dtTagITR = _daITRLocal.GetByTagProject(projectGuid, discipline);
            dsITR_MAIN.ITR_MAINDataTable dtWBSITR = _daITRLocal.GetByWBSProject(projectGuid);
            List<WorkflowTemplateTagWBS> AllITRs = new List<WorkflowTemplateTagWBS>();

            if (dtTagITR != null)
            {
                dtITRMaster.Merge(dtTagITR);
                dtTagITR.Dispose();
            }

            if (dtWBSITR != null)
            {
                dtITRMaster.Merge(dtWBSITR);
                dtWBSITR.Dispose();
            }

            List<wbsTagDisplay> WBSTags = GetWBSTag(projectGuid, discipline);
            dsITR_MAIN.ITR_MAINDataTable dtITR = new dsITR_MAIN.ITR_MAINDataTable(); //just to obtain the schema
            foreach (wbsTagDisplay WBSTag in WBSTags)
            {
                var query = from iTR in dtITRMaster.AsEnumerable()
                            where ((!iTR.IsWBS_GUIDNull() && iTR.WBS_GUID == WBSTag.wbsTagDisplayGuid) || (!iTR.IsTAG_GUIDNull() && iTR.TAG_GUID == WBSTag.wbsTagDisplayGuid))
                            select iTR;

                if (query.Any())
                {
                    DataTable dt = query.CopyToDataTable();

                    foreach (DataRow dr in dt.Rows)
                    {
                        dsITR_MAIN.ITR_MAINRow drITR = dtITR.NewITR_MAINRow();
                        drITR.ItemArray = dr.ItemArray;
                        AllITRs.Add(Common.CreateWorkflowTemplateTagWBS(WBSTag, drITR, true, null));
                    }
                }
            }

            return AllITRs;
        }

        /// <summary>
        /// Loads all the WBS/Tag assigned to this project and discipline
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="discipline"></param>
        private List<wbsTagDisplay> GetWBSTag(Guid projectGuid, string discipline)
        {
            List<wbsTagDisplay> WBSTagDisplays = new List<wbsTagDisplay>();

            dsTAG.TAGDataTable dtTag = _daTagLocal.GetByProjectDiscipline(projectGuid, discipline);
            dsWBS.WBSDataTable dtWBS = _daWBSLocal.GetByProjectDiscipline(projectGuid, discipline);

            if (dtTag != null)
            {
                foreach (dsTAG.TAGRow drTag in dtTag.Rows)
                {
                    WBSTagDisplays.Add(new wbsTagDisplay(new Tag(drTag.GUID)
                    {
                        tagNumber = drTag.NUMBER,
                        tagDescription = drTag.DESCRIPTION,
                        tagParentGuid = drTag.PARENTGUID,
                        tagScheduleGuid = drTag.SCHEDULEGUID
                    }));
                }
            }

            if (dtWBS != null)
            {
                foreach (dsWBS.WBSRow drWBS in dtWBS.Rows)
                {
                    WBSTagDisplays.Add(new wbsTagDisplay(new WBS(drWBS.GUID)
                    {
                        wbsName = drWBS.NAME,
                        wbsDescription = drWBS.IsDESCRIPTIONNull() ? "" : drWBS.DESCRIPTION,
                        wbsParentGuid = drWBS.PARENTGUID,
                        wbsScheduleGuid = drWBS.SCHEDULEGUID
                    }));
                }
            }

            return WBSTagDisplays;
        }

        private void SaveHistory()
        {
            if (_SyncItems.Count == 0)
                return;

            dsSYNC_HISTORY dsSyncHistoryLocal = new dsSYNC_HISTORY();
            dsSYNC_HISTORY dsSyncHistoryRemote = new dsSYNC_HISTORY();
            string pairDesc = GetRemoteSyncDescription(_HWID);
            _daSyncHistoryLocal.RemoveBy(_HWID);
            _daSyncHistoryRemote.RemoveBy(_HWID);

            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("Logging Sync History ... ");
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, _SyncItems.Count);

            foreach (SyncStatus_Superseded syncStatus in _SyncItems)
            {
                dsSYNC_HISTORY.SYNC_HISTORYRow drSyncHistoryLocal = dsSyncHistoryLocal.SYNC_HISTORY.NewSYNC_HISTORYRow();
                drSyncHistoryLocal.GUID = Guid.NewGuid();
                drSyncHistoryLocal.SYNC_ITEM_GUID = syncStatus.GUID;
                drSyncHistoryLocal.SYNC_PARENT_GUID = syncStatus.parentGuid;
                drSyncHistoryLocal.MACHINE_HWID = _HWID;
                drSyncHistoryLocal.MACHINE_DESC = pairDesc;
                drSyncHistoryLocal.TYPE = syncStatus.type;
                drSyncHistoryLocal.SYNC_ITEM = syncStatus.syncItemName;
                drSyncHistoryLocal.SYNC_DESC = syncStatus.additionalInfo;
                drSyncHistoryLocal.SYNC_DIRECTION = syncStatus.imageIndex;
                drSyncHistoryLocal.STATUS = syncStatus.status;
                drSyncHistoryLocal.SYNCED = syncStatus.SyncDate;
                drSyncHistoryLocal.SYNCED_BY = syncStatus.SyncBy;
                drSyncHistoryLocal.CREATED = syncStatus.CreatedDate;
                drSyncHistoryLocal.CREATED_BY = syncStatus.CreatedBy;
                if (syncStatus.UpdatedDate != DateTime.MinValue)
                {
                    drSyncHistoryLocal.UPDATED = syncStatus.UpdatedDate;
                    drSyncHistoryLocal.UPDATED_BY = syncStatus.UpdatedBy;
                }
                if (syncStatus.DeletedDate != DateTime.MinValue)
                {
                    drSyncHistoryLocal.DELETED = syncStatus.DeletedDate;
                    drSyncHistoryLocal.DELETED_BY = syncStatus.DeletedBy;
                }

                dsSyncHistoryLocal.SYNC_HISTORY.AddSYNC_HISTORYRow(drSyncHistoryLocal);
                _daSyncHistoryLocal.Save(drSyncHistoryLocal);

                dsSYNC_HISTORY.SYNC_HISTORYRow drSyncHistoryRemote = dsSyncHistoryRemote.SYNC_HISTORY.NewSYNC_HISTORYRow();
                drSyncHistoryRemote.ItemArray = drSyncHistoryLocal.ItemArray;

                //reverse the sync direction for server
                if (drSyncHistoryRemote.SYNC_DIRECTION == 0)
                    drSyncHistoryRemote.SYNC_DIRECTION = 1;
                else if (drSyncHistoryRemote.SYNC_DIRECTION == 1)
                    drSyncHistoryRemote.SYNC_DIRECTION = 0;

                dsSyncHistoryRemote.SYNC_HISTORY.AddSYNC_HISTORYRow(drSyncHistoryRemote);
                _daSyncHistoryRemote.Save(drSyncHistoryRemote);

                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            splashScreenManager1.CloseWaitForm();
        }

        private string GetRemoteSyncDescription(string hwid)
        {
            dsSYNC_PAIR.SYNC_PAIRRow drSyncPair = _daSyncPairRemote.GetBy(hwid);
            if (drSyncPair != null)
                return drSyncPair.DESCRIPTION;
            else
                return "N/A";
        }
        #endregion

        #region Events
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if(!CheckSyncApproval())
            {
                Common.Warn("Pairing request isn't approved yet");
                return;
            }

            if(EstablishEnvironmentParameters())
            {
                Sync_Mode syncMode = Sync_Mode.None;
                bool includeDeletes = false;

                Sync_Project(Sync_Mode.Push);
                syncMode = CheckSync(Sync_Type_Superseded.Project, out includeDeletes);
                if(syncMode != Sync_Mode.None)
                {
                    Sync_Schedule(syncMode, _projectGuid, includeDeletes);
                    Sync_Prefill(syncMode, true);
                    Sync_Equipment(syncMode, true);
                }

                syncMode = CheckSync(Sync_Type_Superseded.Template, out includeDeletes);
                Sync_Workflow(syncMode, includeDeletes);
                Sync_Template(syncMode, includeDeletes);

                syncMode = CheckSync(Sync_Type_Superseded.User, out includeDeletes);
                Sync_Role(syncMode, includeDeletes);
                //Sync_User(_projectGuid, syncMode, includeDeletes);
                if (syncMode != Sync_Mode.None)
                    Sync_User_New();

                syncMode = CheckSync(Sync_Type_Superseded.ITR, out includeDeletes);
                if (syncMode != Sync_Mode.None)
                    //Changes 7-May-2015 : Sync tag and wbs by entire project
                    //Sync_ITR(_projectGuid, _discipline);
                    Sync_ITR(_projectGuid);

                syncMode = CheckSync(Sync_Type_Superseded.Punchlist, out includeDeletes);
                if (syncMode != Sync_Mode.None)
                    //Changes 7-May-2015 : Sync tag and wbs by entire project
                    //Sync_Punchlist(_projectGuid, _discipline);
                    //Sync_Punchlist(_projectGuid);
                    Sync_Punchlist_New(_projectGuid);

                treeListSyncStatus.RefreshDataSource();
                treeListSyncStatus.ExpandAll();
                //treeListSyncStatus.BestFitColumns();
                //treeListSyncStatus.MoveLast();
                SaveHistory();
                Common.Prompt("Sync Completed");
            }
        }

        private void DisposeAdapters()
        {
            _daITRIssueLocal.Dispose();
            _daITRIssueRemote.Dispose();
            _daITRLocal.Dispose();
            _daITRRemote.Dispose();
            _daITRStatusLocal.Dispose();
            _daITRStatusRemote.Dispose();
            _daPrefillLocal.Dispose();
            _daPrefillRegLocal.Dispose();
            _daPrefillRegRemote.Dispose();
            _daPrefillRemote.Dispose();
            _daProjectLocal.Dispose();
            _daProjectRemote.Dispose();
            _daPunchlistIssueLocal.Dispose();
            _daPunchlistIssueRemote.Dispose();
            _daPunchlistLocal.Dispose();
            _daPunchlistRemote.Dispose();
            _daPunchlistStatusLocal.Dispose();
            _daPunchlistStatusRemote.Dispose();
            _daRegisterLocal.Dispose();
            _daRegisterRemote.Dispose();
            _daRoleLocal.Dispose();
            _daRoleRemote.Dispose();
            _daScheduleLocal.Dispose();
            _daScheduleRemote.Dispose();
            _daSyncHistoryLocal.Dispose();
            _daSyncHistoryRemote.Dispose();
            _daSyncPairRemote.Dispose();
            _daSyncTableRemote.Dispose();
            _daTagLocal.Dispose();
            _daTagRemote.Dispose();
            _daTemplateLocal.Dispose();
            _daTemplateRemote.Dispose();
            _daUserLocal.Dispose();
            _daUserRemote.Dispose();
            _daWBSLocal.Dispose();
            _daWBSRemote.Dispose();
            _daWorkflowLocal.Dispose();
            _daWorkflowRemote.Dispose();
        }
        #endregion

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintingSystem printingSystem1 = new PrintingSystem();

            PrintableComponentLink printableComponentLink = new PrintableComponentLink();
            printingSystem1.Links.AddRange(new object[] { printableComponentLink });
            printableComponentLink.Component = treeListSyncStatus;
            printableComponentLink.Landscape = true;
            printableComponentLink.PrintDlg();
        }
    }
}
