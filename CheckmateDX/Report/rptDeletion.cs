using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using ProjectLibrary;
using System.Data;
using System.Linq;
using CheckmateDX.Report.Dataset;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;

namespace CheckmateDX
{
    public partial class rptDeletion : DevExpress.XtraReports.UI.XtraReport
    {
        static string successMessage = "Delete Successful";
        enum CheckUser { byProject, byRole };
        DeletionCollection _deletions = new DeletionCollection();
        public rptDeletion(DataTable dtDelete)
        {
            InitializeComponent();
            if (dtDelete.GetType() == typeof(dsPROJECT.PROJECTDataTable))
            {
                Delete_Project(dtDelete);
                xrTitle.Text = "PROJECT DELETION REPORT";
            }
            else if (dtDelete.GetType() == typeof(dsUSER_MAIN.USER_MAINDataTable))
            {
                Delete_User(dtDelete);
                xrTitle.Text = "USER DELETION REPORT";
            }
            else if (dtDelete.GetType() == typeof(dsROLE_MAIN.ROLE_MAINDataTable))
            {
                Delete_Role(dtDelete);
                xrTitle.Text = "ROLE DELETION REPORT";
            }
            else if (dtDelete.GetType() == typeof(dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable))
            {
                Delete_Workflow(dtDelete);
                xrTitle.Text = "WORKFLOW DELETION REPORT";
            }
            else if (dtDelete.GetType() == typeof(dsSCHEDULE.SCHEDULEDataTable))
            {
                Delete_Schedule(dtDelete);
                xrTitle.Text = "SCHEDULE DELETION REPORT";
            }

            bindingSource1.DataSource = _deletions;
        }

        /// <summary>
        /// Used for Preloading
        /// </summary>
        public rptDeletion()
        {
            InitializeComponent();
        }

        public rptDeletion(Schedule schedule, List<wbsTagDisplay> deleteWBSTags)
        {
            InitializeComponent();
            Delete_WBSTag(schedule, deleteWBSTags);
            xrTitle.Text = "WBS/TAG DELETION REPORT";
            bindingSource1.DataSource = _deletions;
        }

        public rptDeletion(List<ImportStatus> importStatuses)
        {
            InitializeComponent();
            Import_Report(importStatuses);
            xrTitle.Text = "BULK TAG OPERATION REPORT";
            bindingSource1.DataSource = _deletions;
        }

        public void ShowReport()
        {
            this.ShowPreview();
        }

        #region Validations
        /// <summary>
        /// Showing bulk import status
        /// </summary>
        /// <param name="importStatuses"></param>
        private void Import_Report(List<ImportStatus> importStatuses)
        {
            dsDELETION.dtDELETIONDataTable dtDeletion = new dsDELETION.dtDELETIONDataTable();

            foreach (ImportStatus importStatus in importStatuses)
            {
                _deletions.Add(new Deletion(Guid.Empty, importStatus.tagStatus, importStatus.tagName));
            }
        }

        /// <summary>
        /// Checks for attached WBS/Tag for WBS deletion and checks for attached Tag for Tag deletion
        /// </summary>
        /// <param name="dtDelete">WBS Datatable or Tag Datatable</param>
        private void Delete_WBSTag(Schedule schedule, List<wbsTagDisplay> deleteWBSTags)
        {
            AdapterITR_MAIN daITR = new AdapterITR_MAIN();
            
            AdapterTAG daTag = new AdapterTAG();
            AdapterWBS daWBS = new AdapterWBS();

            try
            {
                List<tagCount> tagITRCount = daITR.GetTagCompletedCount(schedule.GUID); //get all ITR saved count in Tag
                List<wbsCount> wbsITRCount = daITR.GetWBSCompletedCount(schedule.GUID); //get all ITR saved count in WBS
                List<wbsTagDisplay> wbsTagBranch = new List<wbsTagDisplay>();
                List<wbsTagDisplay> approvedWBSTagDeletion = new List<wbsTagDisplay>();

                //subdivide user selection into branches
                foreach(wbsTagDisplay deleteWBSTag in deleteWBSTags)
                {
                    if(!deleteWBSTags.Any(obj => obj.wbsTagDisplayGuid == deleteWBSTag.wbsTagDisplayParentGuid))
                    {
                        wbsTagBranch.Add(deleteWBSTag); //only add the grandparent to the branch
                    }
                }

                foreach (wbsTagDisplay deleteWBSTag in wbsTagBranch) //multiple branch handling
                {
                    Deletion parentWBSTagDeletion = null;
                    RecurseRemoveWBSTag(parentWBSTagDeletion, tagITRCount, wbsITRCount, deleteWBSTags, deleteWBSTag.wbsTagDisplayGuid, approvedWBSTagDeletion);
                }

                foreach(wbsTagDisplay approvedForDeletion in approvedWBSTagDeletion)
                {
                    if(approvedForDeletion.wbsTagDisplayAttachTag != null)
                    {
                        daTag.RemoveBy(approvedForDeletion.wbsTagDisplayGuid);
                    }

                    if(approvedForDeletion.wbsTagDisplayAttachWBS != null)
                    {
                        daWBS.RemoveBy(approvedForDeletion.wbsTagDisplayGuid);
                    }
                }
            }
            finally
            {
                daITR.Dispose();
                daTag.Dispose();
                daWBS.Dispose();
            }
        }

        private bool RecurseRemoveWBSTag(Deletion parentWBSTagDeletion, List<tagCount> tagITRCount, List<wbsCount> wbsITRCount, List<wbsTagDisplay> deleteWBSTags, Guid parentGuid, List<wbsTagDisplay>approvedForDeletion)
        {
            bool addParent = false; //if this is from a new branch parent is added to master at the end, if not child will be appended throughout the iteration

            wbsTagDisplay parentWBSTag = deleteWBSTags.FirstOrDefault(obj => obj.wbsTagDisplayGuid == parentGuid);
            if(parentWBSTagDeletion == null)
            {
                parentWBSTagDeletion = new Deletion(parentWBSTag.wbsTagDisplayGuid, "WBS/TAG", parentWBSTag.wbsTagDisplayName);
                addParent = true;
            }

            List<wbsTagDisplay> childWBSTags = deleteWBSTags.Where(obj => obj.wbsTagDisplayParentGuid == parentGuid).ToList();
            bool childrensHaveNoITRs = true;
            foreach (wbsTagDisplay childWBSTag in childWBSTags)
            {
                if (!RecurseRemoveWBSTag(parentWBSTagDeletion, tagITRCount, wbsITRCount, deleteWBSTags, childWBSTag.wbsTagDisplayGuid, approvedForDeletion))
                {
                    childrensHaveNoITRs = false;
                }
            }

            ChildDeletion childWBSTagDeletion = new ChildDeletion(parentWBSTag.wbsTagDisplayGuid, "WBS/Tag", parentWBSTag.wbsTagDisplayName);
            if (childrensHaveNoITRs) //check self if children have no ITR
            {
                if (parentWBSTag != null)
                {
                    if (tagITRCount.Any(obj => (Guid)obj.tcTag.Value == parentWBSTag.wbsTagDisplayGuid && obj.tcCompletedCount > 0) || wbsITRCount.Any(obj => (Guid)obj.wcWBS.Value == parentWBSTag.wbsTagDisplayGuid && obj.wcTotalCount > 0))
                    {
                        childrensHaveNoITRs = false;
                        childWBSTagDeletion.Message = "Failed: ITR(s) Exist";
                    }
                    else
                    {
                        childWBSTagDeletion.Message = successMessage;
                        approvedForDeletion.Add(parentWBSTag);
                    }
                        

                    if (addParent)
                        parentWBSTagDeletion.Message = childWBSTagDeletion.Message;
                    else
                        parentWBSTagDeletion.Add(childWBSTagDeletion);
                }
            }
            else //if children have ITR
            {
                if (addParent)
                    parentWBSTagDeletion.Message = "Failed: ITR(s) Exists in Child";
                else
                {
                    childWBSTagDeletion.Message = "Failed: ITR(s) Exist in Child";
                    parentWBSTagDeletion.Add(childWBSTagDeletion);
                }     
            }
                

            if(addParent)
                _deletions.Add(parentWBSTagDeletion);

            return childrensHaveNoITRs; //aggregate conclusion of all childrens deletion results. If all children has been approved notify parent for deletion if not notify parent for non-deletion
        }

        /// <summary>
        /// Check tag dependencies before deleting a schedule
        /// </summary>
        /// <param name="dtDelete">Table containing schedule to delete</param>
        private void Delete_Schedule(DataTable dtDelete)
        {
            AdapterSCHEDULE daSchedule = new AdapterSCHEDULE();
            AdapterTAG daTag = new AdapterTAG();

            try
            {
                dsSCHEDULE.SCHEDULEDataTable dtSchedule = (dsSCHEDULE.SCHEDULEDataTable)dtDelete;

                foreach (dsSCHEDULE.SCHEDULERow drSchedule in dtSchedule.Rows)
                {
                    Deletion deletionSchedule = new Deletion(drSchedule.GUID, "Schedule", drSchedule.NAME);

                    if (!hasAttachedWBSTag(deletionSchedule, drSchedule.GUID))
                    {
                        daSchedule.RemoveBy(drSchedule.GUID);
                        deletionSchedule.Message = successMessage;
                    }
                    else
                        deletionSchedule.Message = "Failed: Tag(s) or WBS(s) Exist";

                    _deletions.Add(deletionSchedule);
                }
            }
            finally
            {
                daSchedule.Dispose();
                daTag.Dispose();
            }
        }

        /// <summary>
        /// Check dependencies before deleting a workflow
        /// </summary>
        /// <param name="dtDelete">Table containing workflow to delete</param>
        private void Delete_Workflow(DataTable dtDelete)
        {
            AdapterWORKFLOW_MAIN daWorkflow = new AdapterWORKFLOW_MAIN();
            AdapterTEMPLATE_MAIN daTemplate = new AdapterTEMPLATE_MAIN();

            try
            {
                bool delete = true;
                dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWorkflow = (dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable)dtDelete;
                //First Pass: Populate all associated deletion
                foreach (dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow in dtWorkflow.Rows)
                {
                    Deletion workflowDeletion = new Deletion(drWorkflow.GUID, "Workflow", drWorkflow.NAME);

                    if (hasAttachedTemplate(workflowDeletion, drWorkflow.GUID))
                    {
                        delete = false;
                    }

                    _deletions.Add(workflowDeletion);
                }

                //Second Pass: Deletion depending on total validation outcome
                foreach (Deletion workflowDeletion in _deletions)
                {
                    if (delete && workflowDeletion.Type == "Workflow")
                    {
                        workflowDeletion.Message = successMessage;
                        daWorkflow.RemoveBy(workflowDeletion.ID);
                    }
                    else if (workflowDeletion.Type == "Workflow")
                    {
                        workflowDeletion.Message = "Failed: Template(s) Exists";
                    }
                }
            }
            finally
            {
                daWorkflow.Dispose();
                daTemplate.Dispose();
            }
        }

        /// <summary>
        /// Check dependencies before deleting a role
        /// </summary>
        /// <param name="dtDelete">Table containing role to delete</param>
        private void Delete_Role(DataTable dtDelete)
        {
            AdapterROLE_MAIN daRole = new AdapterROLE_MAIN();
            AdapterUSER_MAIN daUser = new AdapterUSER_MAIN();

            try
            {
                dsROLE_MAIN.ROLE_MAINDataTable dtRole = (dsROLE_MAIN.ROLE_MAINDataTable)dtDelete;
                foreach (dsROLE_MAIN.ROLE_MAINRow drRole in dtRole.Rows)
                {
                    Deletion roleDeletion = new Deletion(drRole.GUID, "Role", drRole.NAME);

                    if (!hasAttachedUser(roleDeletion, drRole.GUID, CheckUser.byRole))
                    {
                        daRole.RemoveBy(drRole.GUID);
                        roleDeletion.Message = successMessage;
                    }
                    else
                        roleDeletion.Message = "Failed: User(s) Exists";

                    _deletions.Add(roleDeletion);
                }
            }
            finally
            {
                daRole.Dispose();
                daUser.Dispose();
            }
        }

        /// <summary>
        /// Check dependencies before deleting a user
        /// </summary>
        /// <param name="dtDelete">Table containing user to delete</param>
        private void Delete_User(DataTable dtDelete)
        {
            using (AdapterUSER_MAIN daUser = new AdapterUSER_MAIN())
            {
                dsUSER_MAIN.USER_MAINDataTable dtUser = (dsUSER_MAIN.USER_MAINDataTable)dtDelete;
                foreach (dsUSER_MAIN.USER_MAINRow drUser in dtUser.Rows)
                {
                    Deletion userDeletion = new Deletion(drUser.GUID, "User", drUser.LASTNAME + " " + drUser.FIRSTNAME);
                    daUser.RemoveBy(drUser.GUID);
                    userDeletion.Message = successMessage;
                    _deletions.Add(userDeletion);
                }
            }
        }

        private void Delete_Matrix(DataTable dtDelete)
        {
            using (AdapterMATRIX_TYPE daMatrix = new AdapterMATRIX_TYPE())
            {
                dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMatrix = (dsMATRIX_TYPE.MATRIX_TYPEDataTable)dtDelete;
                foreach(dsMATRIX_TYPE.MATRIX_TYPERow drMatrix in dtMatrix.Rows)
                {
                    Deletion matrixDeletion = new Deletion(drMatrix.GUID, "Matrix", drMatrix.NAME);
                    
                }
            }
        }

        /// <summary>
        /// Check dependencies before deleting a project
        /// </summary>
        /// <param name="dtDelete">table containing projects to delete</param>
        private void Delete_Project(DataTable dtDelete)
        {
            bool isSuperadmin = ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty);

            AdapterPROJECT daProject = new AdapterPROJECT();
            try
            {
                dsPROJECT.PROJECTDataTable dtProject = (dsPROJECT.PROJECTDataTable)dtDelete;
                foreach (dsPROJECT.PROJECTRow drProject in dtProject.Rows)
                {
                    string failedMessage = string.Empty;
                    Deletion projectDeletion = new Deletion(drProject.GUID, "Project", drProject.NUMBER);

                    if (!isSuperadmin)
                        failedMessage = "Failed: Only superadmin allowed to delete project";
                    //allow deletion of project irregardless
                    //if (hasAttachedSchedule(projectDeletion, drProject.GUID))
                    //{
                    //    failedMessage = "Failed: Schedule(s) Exists";
                    //}

                    //if (hasAttachedUser(projectDeletion, drProject.GUID, CheckUser.byProject))
                    //{
                    //    if (failedMessage != string.Empty)
                    //        failedMessage = "Failed: Schedule(s) and User(s) Exists";
                    //    else
                    //        failedMessage = "Failed: User(s) Exists";
                    //}

                    if (failedMessage == string.Empty)
                    {
                        daProject.RemoveBy(drProject.GUID);
                        projectDeletion.Message = successMessage;
                    }
                    else
                        projectDeletion.Message = failedMessage;
                    
                    _deletions.Add(projectDeletion);
                }
            }
            finally
            {
                daProject.Dispose();
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Remove attached tag against tag
        /// </summary>
        /// <param name="dtDeletion">Populate for information purpose if deletion cannot be performed</param>
        /// <param name="checkGuid">Guid to check against</param>
        /// <returns></returns>
        private static bool removeAttachedTag(Deletion parentDeletion, Guid checkGuid)
        {
            AdapterTAG daTag = new AdapterTAG();
            AdapterPUNCHLIST_MAIN daPunchlist = new AdapterPUNCHLIST_MAIN();
            AdapterITR_MAIN daITR = new AdapterITR_MAIN();
            bool attachmentExists = false;

            try
            {
                dsTAG.TAGDataTable dtTag = daTag.GetTagChildrens(checkGuid, true);
                
                if (dtTag != null)
                {
                    foreach (dsTAG.TAGRow drTag in dtTag.Rows)
                    {
                        dsITR_MAIN.ITR_MAINDataTable dtITR = daITR.GetByTagWBS(drTag.GUID);
                        dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPunchlist = daPunchlist.GetByWBSTag(drTag.GUID);

                        ChildDeletion tagDeletion = new ChildDeletion(drTag.PARENTGUID, "Tag", drTag.NUMBER);
                        string failedMessage = string.Empty;

                        if(dtITR != null)
                        {
                            attachmentExists = true;
                            failedMessage = "Failed: ITR(s) Exist";
                        }
                        
                        if(dtPunchlist != null)
                        {
                            attachmentExists = true;
                            if (failedMessage != string.Empty)
                                failedMessage = "Failed: ITR(s) and Punchlist(s) Exist";
                            else
                                failedMessage = "Failed: Punchlist(s) Exist";
                        }

                        if(failedMessage != string.Empty)
                            tagDeletion.Message = failedMessage;
                        else
                        {
                            tagDeletion.Message = successMessage;
                            daTag.RemoveBy(drTag.GUID);
                        }

                        parentDeletion.Add(tagDeletion);
                    }

                    if (attachmentExists)
                        return false;
                    else
                        return true;
                }
                else
                    return true;
            }
            finally
            {
                daTag.Dispose();
                daITR.Dispose();
            }
        }

        /// <summary>
        /// Remove attached tag and wbs
        /// </summary>
        /// <param name="dtDeletion">Populate for information purpose if deletion cannot be performed</param>
        /// <param name="scheduleGuid">schedule Guid to check against</param>
        /// <returns></returns>
        private static bool hasAttachedWBSTag(Deletion parentDeletion, Guid scheduleGuid)
        {
            AdapterTAG daTag = new AdapterTAG();
            AdapterWBS daWBS = new AdapterWBS();

            try
            {
                dsTAG.TAGDataTable dtTag = new dsTAG.TAGDataTable();
                dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();

                dtTag = daTag.GetBySchedule(scheduleGuid);
                dtWBS = daWBS.GetByProject(scheduleGuid);

                dtWBS = daWBS.GetWBSChildrens(scheduleGuid, false);
                if (dtWBS != null || dtTag != null)
                {
                    return true;
                }

                return false;
            }
            finally
            {
                daTag.Dispose();
                daWBS.Dispose();
            }
        }

        /// <summary>
        /// Check attached template against workflow
        /// </summary>
        /// <param name="dtDeletion">Populate for information purpose if deletion cannot be performed</param>
        /// <param name="checkGuid">Workflow to check against</param>
        /// <returns></returns>
        private static bool hasAttachedTemplate(Deletion parentDeletion, Guid workflowGuid)
        {
            AdapterTEMPLATE_MAIN daTemplate = new AdapterTEMPLATE_MAIN();
            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTemplate = daTemplate.GetByWorkflow(workflowGuid);

            try
            {
                if (dtTemplate != null)
                {
                    foreach (dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate in dtTemplate.Rows)
                    {
                        parentDeletion.Add(new ChildDeletion(workflowGuid, "Template", drTemplate.NAME));
                    }

                    dtTemplate.Dispose();
                    return true;
                }
                else
                    return false;
            }
            finally
            {
                daTemplate.Dispose();
            }
        }

        /// <summary>
        /// Check attached schedule against project
        /// </summary>
        /// <param name="dtDeletion">Populate for information purpose if deletion cannot be performed</param>
        /// <param name="projectGuid">Project Guid to check against</param>
        /// <returns></returns>
        private bool hasAttachedSchedule(Deletion parentDeletion, Guid projectGuid)
        {
            AdapterSCHEDULE daSchedule = new AdapterSCHEDULE();

            try
            {
                dsSCHEDULE.SCHEDULEDataTable dtSchedule = daSchedule.GetByProject(projectGuid);

                if (dtSchedule != null)
                {
                    foreach (dsSCHEDULE.SCHEDULERow drSchedule in dtSchedule.Rows)
                    {
                        parentDeletion.Add(new ChildDeletion(projectGuid, "Schedule", drSchedule.NAME));
                    }

                    dtSchedule.Dispose();
                    return true;
                }
                else
                    return false;
            }
            finally
            {
                daSchedule.Dispose();
            }
        }

        /// <summary>
        /// Check attached users against type
        /// </summary>
        /// <param name="dtDeletion">Populate for information purpose if deletion cannot be performed</param>
        /// <param name="checkGuid">Guid to check against</param>
        /// <param name="checkType">Type to check against</param>
        /// <returns></returns>
        private bool hasAttachedUser(Deletion parentDeletion, Guid checkGuid,
            CheckUser checkType)
        {
            AdapterUSER_MAIN daUser = new AdapterUSER_MAIN();

            try
            {
                dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();

                if (checkType == CheckUser.byProject)
                    dtUser = daUser.GetByProject(checkGuid);
                else if (checkType == CheckUser.byRole)
                    dtUser = daUser.GetByRole(checkGuid);

                if (dtUser != null)
                {
                    foreach (dsUSER_MAIN.USER_MAINRow drUser in dtUser.Rows)
                    {
                        parentDeletion.Add(new ChildDeletion(drUser.DPROJECT, "User", String.Format("{0} - {1}, {2}", drUser.QANUMBER, drUser.FIRSTNAME, drUser.LASTNAME)));
                    }

                    dtUser.Dispose();
                    return true;
                }
                else
                    return false;
            }
            finally
            {
                daUser.Dispose();
            }
        }
        #endregion
    }
}
