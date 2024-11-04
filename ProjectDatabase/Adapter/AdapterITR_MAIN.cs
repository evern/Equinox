using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsITR_MAINTableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterITR_MAIN : SQLBase, IDisposable
    {
        private ITR_MAINTableAdapter _adapter;

        public AdapterITR_MAIN()
            : base()
        {
            _adapter = new ITR_MAINTableAdapter(Variables.ConnStr);
        }

        public AdapterITR_MAIN(string connStr)
            : base(connStr)
        {
            _adapter = new ITR_MAINTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all ITR in the system
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable Get()
        {
            string sql = "SELECT * FROM ITR_MAIN WHERE DELETED IS NULL";
            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get deleted ITR in the system for purging
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable Get_Deleted(Guid projectGuid)
        {
            string sql = "SELECT ITR_MAIN.* FROM ITR_MAIN ";
            sql += " JOIN TAG Tag ON (Tag.GUID = ITR_MAIN.TAG_GUID)";
            sql += " JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID)";
            sql += " JOIN PROJECT Project ON (Project.GUID = Schedule.PROJECTGUID)";
            sql += " WHERE Project.GUID = '" + projectGuid.ToString() + "' AND ITR_MAIN.DELETED IS NOT NULL";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get ITR by guid
        /// </summary>
        public dsITR_MAIN.ITR_MAINRow GetBy(Guid itrGuid)
        {
            string sql = "SELECT * FROM ITR_MAIN WHERE GUID = '" + itrGuid + "'";
            sql += " AND DELETED IS NULL";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get ITR by guid
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable GetTableBy(Guid itrGuid)
        {
            string sql = "SELECT * FROM ITR_MAIN WHERE GUID = '" + itrGuid + "'";
            sql += " AND DELETED IS NULL";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get ITR by guid
        /// </summary>
        public dsITR_MAIN.ITR_MAINRow GetIncludeDeletedBy(Guid itrGuid)
        {
            string sql = "SELECT * FROM ITR_MAIN WHERE GUID = '" + itrGuid + "'";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN[0];
            else
                return null;
        }

        public List<ViewModel_ITRSummary> GenerateITRSummary(Guid projectGuid)
        {
            List<ViewModel_ITRSummary> summary = new List<ViewModel_ITRSummary>();
            string sql = "SELECT TAG.*, ITR_ASSIGNED.ASSIGNED,  ";
            sql += "ISNULL(ITR_STATUSES.SAVED, 0) SAVED,  ";
            sql += "ISNULL(ITR_STATUSES.INSPECTED, 0) INSPECTED,  ";
            sql += "ISNULL(ITR_STATUSES.APPROVED, 0) APPROVED, ";
            sql += "ISNULL(ITR_STATUSES.COMPLETED, 0) COMPLETED,  ";
            sql += "ISNULL(ITR_STATUSES.CLOSED, 0) CLOSED FROM TAG  ";
            sql += "JOIN SCHEDULE ON SCHEDULE.GUID = TAG.SCHEDULEGUID  ";
            sql += "JOIN PROJECT ON PROJECT.GUID = SCHEDULE.PROJECTGUID ";
            sql += "LEFT JOIN  ";
            sql += "(SELECT Register.TAG_GUID, COUNT(Register.TAG_GUID) AS ASSIGNED FROM TEMPLATE_REGISTER Register JOIN TEMPLATE_MAIN Template ON (Template.GUID = Register.TEMPLATE_GUID) WHERE Register.DELETED IS NULL AND Template.DELETED IS NULL GROUP BY Register.TAG_GUID) ITR_ASSIGNED ";
            sql += "ON ITR_ASSIGNED.TAG_GUID = TAG.GUID ";
            sql += "LEFT JOIN  ";
            sql += "( ";
            sql += "SELECT ITR_Table.TAG_GUID,  ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'SAVED' THEN 1 ELSE 0 END) AS SAVED,  ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'INSPECTED' THEN 1 ELSE 0 END) AS INSPECTED,  ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'APPROVED' THEN 1 ELSE 0 END) AS APPROVED, ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'COMPLETED' THEN 1 ELSE 0 END) AS COMPLETED, ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'CLOSED' THEN 1 ELSE 0 END) AS CLOSED ";
            sql += "FROM ";
            sql += "(SELECT Tag.GUID AS TAG_GUID, Tag.NUMBER, iTR.NAME AS ITRNAME, iTR.DESCRIPTION,  ";
            sql += "CASE WHEN iTR_Status.STATUS_NUMBER = -1 THEN 'SAVED'  ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 0 THEN 'INSPECTED'  ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 1 THEN 'APPROVED'  ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 2 THEN 'COMPLETED'  ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 3 THEN 'CLOSED'  ";
            sql += "ELSE 'N/A' END AS STATUS,  ";
            sql += "iTR_Status_Issue.COMMENTS, iTR.CREATED FROM ITR_MAIN iTR JOIN TAG Tag ON (Tag.GUID = iTR.TAG_GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) JOIN PROJECT Project ON (Project.GUID = Schedule.PROJECTGUID) LEFT JOIN ITR_STATUS iTR_Status ON (iTR_Status.ITR_MAIN_GUID = iTR.GUID) LEFT JOIN ITR_STATUS_ISSUE iTR_Status_Issue ON (iTR_Status_Issue.ITR_STATUS_GUID = iTR_Status.GUID)  ";
            sql += "OUTER APPLY  ";
            sql += "(SELECT MAX(ITR_StatusLatest.SEQUENCE_NUMBER) AS LatestSeq FROM ITR_STATUS iTR_StatusLatest WHERE iTR_StatusLatest.ITR_MAIN_GUID = iTR.GUID) LATESTStatusSequence  ";
            sql += "WHERE Project.GUID = '" + projectGuid + "' AND Tag.DELETED IS NULL AND Schedule.DELETED IS NULL AND Project.DELETED IS NULL AND iTR_Status.DELETED IS NULL AND (iTR_Status.SEQUENCE_NUMBER IS NULL OR iTR_Status.SEQUENCE_NUMBER = LATESTStatusSequence.LatestSeq)) ITR_Table ";
            sql += "GROUP BY ITR_Table.TAG_GUID ";
            sql += ") ITR_Statuses ON ITR_Statuses.TAG_GUID = TAG.GUID ";
            sql += "WHERE PROJECT.GUID = '" + projectGuid + "' AND TAG.DELETED IS NULL AND SCHEDULE.DELETED IS NULL ";


            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    summary.Add(new ViewModel_ITRSummary()
                    {
                        GuidKey = dr.Field<Guid>("GUID").ToString(),
                        Number = dr.IsNull("NUMBER") ? "" : dr.Field<string>("NUMBER"),
                        Assigned = dr.IsNull("ASSIGNED") ? 0 : dr.Field<int>("ASSIGNED"), 
                        Saved = dr.IsNull("SAVED") ? 0 : dr.Field<int>("SAVED"),
                        Inspected = dr.IsNull("INSPECTED") ? 0 : dr.Field<int>("INSPECTED"),
                        Approved = dr.IsNull("APPROVED") ? 0 : dr.Field<int>("APPROVED"),
                        Completed = dr.IsNull("COMPLETED") ? 0 : dr.Field<int>("COMPLETED"),
                        Closed = dr.IsNull("CLOSED") ? 0 : dr.Field<int>("CLOSED")
                    });
                }
            }

            return summary;
        }

        public List<ViewModel_ITRSummary> GenerateITRStagedSummary(Guid projectGuid)
        {
            List<ViewModel_ITRSummary> summary = new List<ViewModel_ITRSummary>();
            string sql = "SELECT TAG.*, ITR_ASSIGNED.STAGE1ASSIGNED,  ";
            sql += "ITR_ASSIGNED.STAGE2ASSIGNED, ";
            sql += "ITR_ASSIGNED.STAGE3ASSIGNED, ";
            sql += "ISNULL(ITR_STATUSES.STAGE1SAVED, 0) STAGE1SAVED,  ";
            sql += "ISNULL(ITR_STATUSES.STAGE2SAVED, 0) STAGE2SAVED,  ";
            sql += "ISNULL(ITR_STATUSES.STAGE3SAVED, 0) STAGE3SAVED,  ";
            sql += "ISNULL(ITR_STATUSES.STAGE1INSPECTED, 0) STAGE1INSPECTED,  ";
            sql += "ISNULL(ITR_STATUSES.STAGE2INSPECTED, 0) STAGE2INSPECTED, ";
            sql += "ISNULL(ITR_STATUSES.STAGE3INSPECTED, 0) STAGE3INSPECTED, ";
            sql += "ISNULL(ITR_STATUSES.STAGE1APPROVED, 0) STAGE1APPROVED,";
            sql += "ISNULL(ITR_STATUSES.STAGE2APPROVED, 0) STAGE2APPROVED,";
            sql += "ISNULL(ITR_STATUSES.STAGE3APPROVED, 0) STAGE3APPROVED,";
            sql += "ISNULL(ITR_STATUSES.STAGE1COMPLETED, 0) STAGE1COMPLETED,  ";
            sql += "ISNULL(ITR_STATUSES.STAGE2COMPLETED, 0) STAGE2COMPLETED,  ";
            sql += "ISNULL(ITR_STATUSES.STAGE3COMPLETED, 0) STAGE3COMPLETED,  ";
            sql += "ISNULL(ITR_STATUSES.STAGE1CLOSED, 0) STAGE1CLOSED, ";
            sql += "ISNULL(ITR_STATUSES.STAGE2CLOSED, 0) STAGE2CLOSED, ";
            sql += "ISNULL(ITR_STATUSES.STAGE3CLOSED, 0) STAGE3CLOSED ";
            sql += "FROM TAG  JOIN SCHEDULE ON SCHEDULE.GUID = TAG.SCHEDULEGUID  JOIN PROJECT ON PROJECT.GUID = SCHEDULE.PROJECTGUID ";
            sql += "LEFT JOIN ";
            sql += "(SELECT Register.TAG_GUID, ";
            sql += "SUM(CASE WHEN Workflow.NAME = 'STAGE 1' THEN 1 ELSE 0 END) AS STAGE1ASSIGNED, ";
            sql += "SUM(CASE WHEN Workflow.NAME = 'STAGE 2' THEN 1 ELSE 0 END) AS STAGE2ASSIGNED, ";
            sql += "SUM(CASE WHEN Workflow.NAME = 'STAGE 3' THEN 1 ELSE 0 END) AS STAGE3ASSIGNED ";
            sql += "FROM TEMPLATE_REGISTER Register ";
            sql += "JOIN TEMPLATE_MAIN Template ON (Template.GUID = Register.TEMPLATE_GUID) ";
            sql += "JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID)";
            sql += "WHERE Register.DELETED IS NULL AND Template.DELETED IS NULL ";
            sql += "GROUP BY Register.TAG_GUID) ITR_ASSIGNED ON ITR_ASSIGNED.TAG_GUID = TAG.GUID ";
            sql += "LEFT JOIN ";
            sql += "(SELECT ITR_Table.TAG_GUID, ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 1 SAVED' THEN 1 ELSE 0 END) AS STAGE1SAVED, ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 2 SAVED' THEN 1 ELSE 0 END) AS STAGE2SAVED, ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 3 SAVED' THEN 1 ELSE 0 END) AS STAGE3SAVED,  ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 1 INSPECTED' THEN 1 ELSE 0 END) AS STAGE1INSPECTED,  ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 2 INSPECTED' THEN 1 ELSE 0 END) AS STAGE2INSPECTED,  ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 3 INSPECTED' THEN 1 ELSE 0 END) AS STAGE3INSPECTED, ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 1 APPROVED' THEN 1 ELSE 0 END) AS STAGE1APPROVED, ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 2 APPROVED' THEN 1 ELSE 0 END) AS STAGE2APPROVED, ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 3 APPROVED' THEN 1 ELSE 0 END) AS STAGE3APPROVED, ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 1 COMPLETED' THEN 1 ELSE 0 END) AS STAGE1COMPLETED, ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 2 COMPLETED' THEN 1 ELSE 0 END) AS STAGE2COMPLETED, ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 3 COMPLETED' THEN 1 ELSE 0 END) AS STAGE3COMPLETED, ";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 1 CLOSED' THEN 1 ELSE 0 END) AS STAGE1CLOSED,";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 2 CLOSED' THEN 1 ELSE 0 END) AS STAGE2CLOSED,";
            sql += "SUM(CASE WHEN ITR_Table.STATUS = 'STAGE 3 CLOSED' THEN 1 ELSE 0 END) AS STAGE3CLOSED ";
            sql += "FROM ";
            sql += "(SELECT Tag.GUID AS TAG_GUID, Tag.NUMBER, iTR.NAME AS ITRNAME, iTR.DESCRIPTION, ";
            sql += "CASE ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = -1 AND Workflow.NAME = 'STAGE 1' THEN 'STAGE 1 SAVED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = -1 AND Workflow.NAME = 'STAGE 2' THEN 'STAGE 2 SAVED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = -1 AND Workflow.NAME = 'STAGE 3' THEN 'STAGE 3 SAVED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 0 AND Workflow.NAME = 'STAGE 1' THEN 'STAGE 1 INSPECTED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 0 AND Workflow.NAME = 'STAGE 2' THEN 'STAGE 2 INSPECTED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 0 AND Workflow.NAME = 'STAGE 3' THEN 'STAGE 3 INSPECTED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 1 AND Workflow.NAME = 'STAGE 1' THEN 'STAGE 1 APPROVED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 1 AND Workflow.NAME = 'STAGE 2' THEN 'STAGE 2 APPROVED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 1 AND Workflow.NAME = 'STAGE 3' THEN 'STAGE 3 APPROVED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 2 AND Workflow.NAME = 'STAGE 1' THEN 'STAGE 1 COMPLETED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 2 AND Workflow.NAME = 'STAGE 2' THEN 'STAGE 2 COMPLETED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 2 AND Workflow.NAME = 'STAGE 3' THEN 'STAGE 3 COMPLETED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 3 AND Workflow.NAME = 'STAGE 1' THEN 'STAGE 1 CLOSED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 3 AND Workflow.NAME = 'STAGE 2' THEN 'STAGE 2 CLOSED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 3 AND Workflow.NAME = 'STAGE 3' THEN 'STAGE 3 CLOSED' ";
            sql += "ELSE 'N/A' END AS STATUS,  iTR_Status_Issue.COMMENTS, iTR.CREATED FROM ITR_MAIN iTR JOIN TAG Tag ON (Tag.GUID = iTR.TAG_GUID) ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) JOIN PROJECT Project ON (Project.GUID = Schedule.PROJECTGUID) ";
            sql += "JOIN TEMPLATE_MAIN Template ON Template.GUID = iTR.TEMPLATE_GUID ";
            sql += "JOIN WORKFLOW_MAIN Workflow ON Workflow.GUID = Template.WORKFLOWGUID ";
            sql += "LEFT JOIN ITR_STATUS iTR_Status ON (iTR_Status.ITR_MAIN_GUID = iTR.GUID) ";
            sql += "LEFT JOIN ITR_STATUS_ISSUE iTR_Status_Issue ON (iTR_Status_Issue.ITR_STATUS_GUID = iTR_Status.GUID) ";
            sql += "OUTER APPLY ";
            sql += "(SELECT MAX(ITR_StatusLatest.SEQUENCE_NUMBER) AS LatestSeq FROM ITR_STATUS iTR_StatusLatest WHERE iTR_StatusLatest.ITR_MAIN_GUID = iTR.GUID) LATESTStatusSequence ";
            sql += "WHERE Project.GUID = '" + projectGuid + "' AND Tag.DELETED IS NULL AND Schedule.DELETED IS NULL AND Project.DELETED IS NULL AND iTR_Status.DELETED IS NULL AND (iTR_Status.SEQUENCE_NUMBER IS NULL OR iTR_Status.SEQUENCE_NUMBER = LATESTStatusSequence.LatestSeq)) ITR_Table GROUP BY ITR_Table.TAG_GUID ) ITR_Statuses ON ITR_Statuses.TAG_GUID = TAG.GUID WHERE PROJECT.GUID = '" + projectGuid + "' AND TAG.DELETED IS NULL AND SCHEDULE.DELETED IS NULL";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    summary.Add(new ViewModel_ITRSummary()
                    {
                        GuidKey = dr.Field<Guid>("GUID").ToString(),
                        Number = dr.IsNull("NUMBER") ? "" : dr.Field<string>("NUMBER"),
                        Stage1Assigned = dr.IsNull("STAGE1ASSIGNED") ? 0 : dr.Field<int>("STAGE1ASSIGNED"),
                        Stage2Assigned = dr.IsNull("STAGE2ASSIGNED") ? 0 : dr.Field<int>("STAGE2ASSIGNED"),
                        Stage3Assigned = dr.IsNull("STAGE3ASSIGNED") ? 0 : dr.Field<int>("STAGE3ASSIGNED"),
                        Stage1Saved = dr.IsNull("STAGE1SAVED") ? 0 : dr.Field<int>("STAGE1SAVED"),
                        Stage2Saved = dr.IsNull("STAGE2SAVED") ? 0 : dr.Field<int>("STAGE2SAVED"),
                        Stage3Saved = dr.IsNull("STAGE3SAVED") ? 0 : dr.Field<int>("STAGE3SAVED"),
                        Stage1Inspected = dr.IsNull("STAGE1INSPECTED") ? 0 : dr.Field<int>("STAGE1INSPECTED"),
                        Stage2Inspected = dr.IsNull("STAGE2INSPECTED") ? 0 : dr.Field<int>("STAGE2INSPECTED"),
                        Stage3Inspected = dr.IsNull("STAGE3INSPECTED") ? 0 : dr.Field<int>("STAGE3INSPECTED"),
                        Stage1Approved = dr.IsNull("STAGE1APPROVED") ? 0 : dr.Field<int>("STAGE1APPROVED"),
                        Stage2Approved = dr.IsNull("STAGE2APPROVED") ? 0 : dr.Field<int>("STAGE2APPROVED"),
                        Stage3Approved = dr.IsNull("STAGE3APPROVED") ? 0 : dr.Field<int>("STAGE3APPROVED"),
                        Stage1Completed = dr.IsNull("STAGE1COMPLETED") ? 0 : dr.Field<int>("STAGE1COMPLETED"),
                        Stage2Completed = dr.IsNull("STAGE2COMPLETED") ? 0 : dr.Field<int>("STAGE2COMPLETED"),
                        Stage3Completed = dr.IsNull("STAGE3COMPLETED") ? 0 : dr.Field<int>("STAGE3COMPLETED"),
                        Stage1Closed = dr.IsNull("STAGE1CLOSED") ? 0 : dr.Field<int>("STAGE1CLOSED"),
                        Stage2Closed = dr.IsNull("STAGE2CLOSED") ? 0 : dr.Field<int>("STAGE2CLOSED"),
                        Stage3Closed = dr.IsNull("STAGE3CLOSED") ? 0 : dr.Field<int>("STAGE3CLOSED")
                    });
                }
            }

            return summary;
        }

        public List<ViewModel_MasterITRReport> GetProjectITRMasterReport(Guid projectGuid)
        {
            List<ViewModel_MasterITRReport> projectITRReport = new List<ViewModel_MasterITRReport>();
            string sql = "SELECT Stage, TaskNumber, Template, TemplateDescription = DESCRIPTION, Discipline = DISCIPLINE, Tag, TagDescription, Area, AreaDescription, Subsystem, SubsystemDescription, ";
            sql += "Status =  ";
            sql += "CASE  ";
            sql += "WHEN StatusNumber = -2 THEN 'Pending' ";
            sql += "WHEN StatusNumber = -1 THEN 'Saved' ";
            sql += "WHEN StatusNumber = 0 THEN 'Inspected' ";
            sql += "WHEN StatusNumber = 1 THEN 'Approved' ";
            sql += "WHEN StatusNumber = 2 THEN 'Completed' ";
            sql += "WHEN StatusNumber >= 3 THEN 'Closed' ";
            sql += "ELSE 'N/A' END ";
            sql += "FROM ";
            sql += "( ";
            sql += "SELECT Stage, TaskNumber, Template, DESCRIPTION, DISCIPLINE, Tag, TagDescription, Area, AreaDescription, Subsystem, SubsystemDescription, StatusNumber = MAX(StatusNumber) FROM ";
            sql += "( ";
            sql += "SELECT Stage = Workflow.NAME, TaskNumber = CONCAT(Tag.NUMBER, Template.NAME), Template = Template.NAME, Template.DESCRIPTION, Schedule.DISCIPLINE, Tag = Tag.NUMBER, TagDescription = Tag.DESCRIPTION, Area = AreaWBS.NAME, AreaDescription = AreaWBS.DESCRIPTION, Subsystem = SubsystemWBS.NAME, SubsystemDescription = SubsystemWBS.DESCRIPTION, StatusNumber = -2 ";
            sql += "FROM TAG Tag ";
            sql += "LEFT JOIN TEMPLATE_REGISTER Register ON (Register.TAG_GUID = Tag.GUID) ";
            sql += "JOIN TEMPLATE_MAIN Template ON (Template.GUID = Register.TEMPLATE_GUID) ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID)  ";
            sql += "JOIN PROJECT Project ON (Project.GUID = Schedule.PROJECTGUID)  ";
            sql += "JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID) ";
            sql += "LEFT JOIN WBS SubsystemWBS ON (SubsystemWBS.GUID = Tag.PARENTGUID)  ";
            sql += "LEFT JOIN WBS SystemWBS ON (SystemWBS.GUID = SubsystemWBS.PARENTGUID)  ";
            sql += "LEFT JOIN WBS AreaWBS ON (AreaWBS.GUID = SystemWBS.PARENTGUID)  ";
            sql += "WHERE Project.GUID = '" + projectGuid.ToString() + "' AND Tag.DELETED IS NULL AND Schedule.DELETED IS NULL AND Project.DELETED IS NULL AND Register.DELETED IS NULL AND TEMPLATE.DELETED IS NULL ";
            sql += "UNION ";
            sql += "SELECT Stage = Workflow.NAME, TaskNumber = CONCAT(Tag.NUMBER, Template.NAME), Template.NAME, Template.DESCRIPTION, Schedule.DISCIPLINE, Tag = Tag.NUMBER, TagDescription = Tag.DESCRIPTION, Area = AreaWBS.NAME, AreaDescription = AreaWBS.DESCRIPTION, Subsystem = SubsystemWBS.NAME, SubsystemDescription = SubsystemWBS.DESCRIPTION, StatusNumber =  ISNULL(iTR_Status.STATUS_NUMBER, -1) ";
            sql += "FROM TAG Tag ";
            sql += "JOIN ITR_MAIN iTR ";
            sql += "ON iTR.TAG_GUID = Tag.GUID ";
            sql += "LEFT JOIN ITR_STATUS iTR_Status ON (iTR_Status.ITR_MAIN_GUID = iTR.GUID)  ";
            sql += "LEFT JOIN ITR_STATUS_ISSUE iTR_Status_Issue ON (iTR_Status_Issue.ITR_STATUS_GUID = iTR_Status.GUID)  ";
            sql += "OUTER APPLY  ";
            sql += "( ";
            sql += "SELECT MAX(ITR_StatusLatest.SEQUENCE_NUMBER) AS LatestSeq  ";
            sql += "FROM ITR_STATUS iTR_StatusLatest  ";
            sql += "WHERE iTR_StatusLatest.ITR_MAIN_GUID = iTR.GUID ";
            sql += ") LATESTStatusSequence  ";
            sql += "JOIN TEMPLATE_MAIN Template ON (Template.GUID = iTR.TEMPLATE_GUID) ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID)  ";
            sql += "JOIN PROJECT Project ON (Project.GUID = Schedule.PROJECTGUID)  ";
            sql += "JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID) ";
            sql += "LEFT JOIN WBS SubsystemWBS ON (SubsystemWBS.GUID = Tag.PARENTGUID)  ";
            sql += "LEFT JOIN WBS SystemWBS ON (SystemWBS.GUID = SubsystemWBS.PARENTGUID)  ";
            sql += "LEFT JOIN WBS AreaWBS ON (AreaWBS.GUID = SystemWBS.PARENTGUID)  ";
            sql += "WHERE Project.GUID = '" + projectGuid.ToString() + "' AND Tag.DELETED IS NULL AND Schedule.DELETED IS NULL AND Project.DELETED IS NULL ";
            sql += "AND iTR.DELETED IS NULL AND Template.DELETED IS NULL AND (iTR_Status.SEQUENCE_NUMBER IS NULL OR iTR_Status.SEQUENCE_NUMBER = LATESTStatusSequence.LatestSeq)  ";
            sql += ") AS TblUnion ";
            sql += "GROUP BY Stage, TaskNumber, Template, DESCRIPTION, DISCIPLINE, Tag, TagDescription, Area, AreaDescription, Subsystem,SubsystemDescription ";
            sql += ") TblStatusSelection ";


            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    projectITRReport.Add(new ViewModel_MasterITRReport()
                    {
                        Stage = dr.Field<string>("Stage"),
                        TaskNumber = dr.Field<string>("TaskNumber"),
                        Template = dr.Field<string>("Template"),
                        TemplateDescription = dr.Field<string>("TemplateDescription"),
                        Discipline = dr.Field<string>("Discipline"),
                        Tag = dr.Field<string>("Tag"),
                        TagDescription = dr.Field<string>("TagDescription"),
                        Area = dr.Field<string>("Area"),
                        AreaDescription = dr.Field<string>("AreaDescription"),
                        Subsystem = dr.Field<string>("Subsystem"),
                        SubsystemDescription = dr.Field<string>("SubsystemDescription"),
                        Status = dr.Field<string>("Status")
                    });
                }
            }

            return projectITRReport;
        }


        public List<ViewModel_MasterITRStatusReport> GetProjectITRStatusBreakdownReport(Guid projectGuid)
        {
            List<ViewModel_MasterITRStatusReport> projectITRStatusBreakdownReport = new List<ViewModel_MasterITRStatusReport>();
            string sql = "SELECT DISCIPLINE = Discipline, ISNULL(AreaDescription, 'N/A') AS AREAWBS, ISNULL(SystemDescription, 'N/A') AS SYSTEMWBS, ISNULL(Subsystem, 'N/A') AS SUBSYSTEMWBSNAME, ISNULL(SubsystemDescription, 'N/A') AS SUBSYSTEMWBSDESC, WFNAME = Stage, Tag, Template, TemplateDescription, Pending = MAX(RegisterCreatedDate), PendingPerson = MAX(RegisterCreatedPerson), Saved = MAX(ITRCreatedDate), SavedPerson = MAX(ITRCreatePerson),  MAX(CASE WHEN Status = 'Inspected' THEN StatusDate END) Inspected, MAX(CASE WHEN Status = 'Inspected' THEN StatusPerson END) InspectedPerson,  MAX(CASE WHEN Status = 'Approved' THEN StatusDate END) Approved, MAX(CASE WHEN Status = 'Approved' THEN StatusPerson END) ApprovedPerson,  MAX(CASE WHEN Status = 'Completed' THEN StatusDate END) Completed,  MAX(CASE WHEN Status = 'Completed' THEN StatusPerson END) CompletedPerson,  MAX(CASE WHEN Status = 'Closed' THEN StatusDate END) Closed, MAX(CASE WHEN Status = 'Closed' THEN StatusPerson END) ClosedPerson ";
            sql += "FROM   ";
            sql += "(   ";
            sql += "SELECT ProjectNumber, Stage, TaskNumber, Template, TemplateDescription = DESCRIPTION, Discipline = DISCIPLINE, Tag, TagDescription, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, Status =  CASE WHEN StatusNumber = -99 THEN 'Pending' WHEN StatusNumber <= -1 AND StatusNumber > -98 THEN 'Saved' WHEN StatusNumber = 0 THEN 'Inspected' WHEN StatusNumber = 1 THEN 'Approved' WHEN StatusNumber = 2 THEN 'Completed' WHEN StatusNumber >= 3 THEN 'Closed' ELSE 'N/A' END, StatusDate, StatusPerson, RegisterCreatedDate,  RegisterCreatedPerson, ITRCreatedDate, ITRCreatePerson FROM  ";
            sql += "(  ";
            sql += "SELECT ProjectNumber, Stage, TaskNumber, Template, Description, DISCIPLINE, Tag, TagDescription, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, StatusNumber, StatusDate, StatusPerson, RegisterCreatedDate = NULL, RegisterCreatedPerson = NULL, ITRCreatedDate, ITRCreatePerson ";
            sql += "FROM  ";
            sql += "(  ";
            sql += "SELECT ProjectNumber = Project.NUMBER, Stage = Workflow.NAME, TaskNumber = CONCAT(Tag.NUMBER, Template.NAME), Template = Template.NAME, Description = Template.DESCRIPTION,   ";
            sql += "Tag.GUID AS TagGUID, Discipline = Template.DISCIPLINE, ITRGuid = Itr.GUID, tblAllITRStatus.STATUS_NUMBER, tblAllITRStatus.SEQUENCE_NUMBER, tblAllITRStatus.CREATED,  ";
            sql += "StatusNumber =  ISNULL(tblAllITRStatus.STATUS_NUMBER, -1), StatusDate = ISNULL(tblAllITRStatus.CREATED, Itr.CREATED), StatusPerson, ITRCreatedDate = itr.CREATED, ITRCreatePerson = CONCAT(USER_MAIN.FIRSTNAME, ' ', USER_MAIN.LASTNAME) ";
            sql += "FROM ITR_MAIN Itr    ";
            sql += "JOIN TAG ON Tag.guid = Itr.TAG_GUID  ";
            sql += "JOIN WBS ON TAG.PARENTGUID = WBS.GUID  ";
            sql += "JOIN SCHEDULE ON SCHEDULE.GUID = TAG.SCHEDULEGUID  ";
            sql += "JOIN PROJECT ON PROJECT.GUID = SCHEDULE.PROJECTGUID  ";
            sql += "JOIN TEMPLATE_MAIN Template ON Template.GUID = iTR.TEMPLATE_GUID ";
            sql += "JOIN WORKFLOW_MAIN Workflow ON Workflow.GUID = Template.WORKFLOWGUID ";
            sql += "JOIN USER_MAIN ON USER_MAIN.GUID = Itr.CREATEDBY ";
            sql += "OUTER APPLY    ";
            sql += "(  ";
            sql += "SELECT ITR_STATUS.*, StatusPerson = CONCAT(USER_MAIN.FIRSTNAME, ' ', USER_MAIN.LASTNAME) ";
            sql += "FROM ITR_STATUS  ";
            sql += "LEFT JOIN USER_MAIN ON USER_MAIN.GUID = ITR_STATUS.CREATEDBY ";
            sql += "WHERE ITR_STATUS.ITR_MAIN_GUID = Itr.GUID AND itr.TAG_GUID = Tag.GUID AND itr.DELETED IS NULL  ";
            sql += ") tblAllITRStatus   ";
            sql += "WHERE itr.DELETED IS NULL AND Tag.DELETED IS NULL AND Schedule.DELETED IS NULL AND Template.DELETED IS NULL AND Workflow.DELETED IS NULL AND Project.GUID = '" + projectGuid + "'   ";
            sql += ") TblTagITRLatestStatus  ";
            sql += "OUTER APPLY  ";
            sql += "(  ";
            sql += "SELECT TblTagITRLatestStatus.ITRGuid,  ";
            sql += "Tag = Tag1.NUMBER,   ";
            sql += "TagDescription = Tag1.DESCRIPTION,   ";
            sql += "Area = AreaWBS.NAME,   ";
            sql += "AreaDescription = AreaWBS.DESCRIPTION,   ";
            sql += "System = SystemWBS.NAME,   ";
            sql += "SystemDescription = SystemWBS.DESCRIPTION,   ";
            sql += "Subsystem = SubsystemWBS.NAME, SubsystemDescription = SubsystemWBS.DESCRIPTION  ";
            sql += "FROM TAG Tag1   ";
            sql += "JOIN SCHEDULE Sch ON (Sch.GUID = tag1.SCHEDULEGUID)    ";
            sql += "JOIN PROJECT Proj ON (Proj.GUID = Sch.PROJECTGUID)   ";
            sql += "JOIN WBS SubsystemWBS ON (SubsystemWBS.GUID = Tag1.PARENTGUID)   ";
            sql += "JOIN WBS SystemWBS ON (SystemWBS.GUID = SubsystemWBS.PARENTGUID)   ";
            sql += "JOIN WBS AreaWBS ON (AreaWBS.GUID = SystemWBS.PARENTGUID)   ";
            sql += "WHERE Tag1.DELETED IS NULL AND Sch.DELETED IS NULL AND Proj.DELETED IS NULL  ";
            sql += "AND Tag1.GUID = TblTagITRLatestStatus.TagGUID  ";
            sql += ") TblTagITRLookup  ";
            sql += "GROUP BY ProjectNumber, ITRCreatedDate, ITRCreatePerson, Stage, TaskNumber, Template, Description, DISCIPLINE, Tag, TagDescription, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, StatusNumber, StatusDate, StatusPerson  ";
            sql += "UNION ALL  ";
            sql += "SELECT ProjectNumber, Stage, TaskNumber, Template, Description, DISCIPLINE, Tag, TagDescription, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, StatusNumber = -99, StatusDate = NULL, StatusPerson = NULL, RegisterCreatedDate, RegisterCreatedPerson, ITRCreatedDate = NULL, ITRCreatePerson = NULL ";
            sql += "FROM  ";
            sql += "(  ";
            sql += "SELECT Register.GUID AS RegisterGUID, ProjectNumber = PROJECT.NUMBER, Stage = Workflow.NAME, TaskNumber = CONCAT(Tag.NUMBER, Template.NAME), Template = Template.NAME, Description = Template.DESCRIPTION, Register.TAG_GUID AS TagGuid, Workflow.NAME AS WFNAME, Template.DISCIPLINE, Template.NAME, RegisterCreatedDate = Register.CREATED, RegisterCreatedPerson = CONCAT(USER_MAIN.FIRSTNAME, ' ', USER_MAIN.LASTNAME) ";
            sql += "FROM TEMPLATE_REGISTER Register   ";
            sql += "JOIN TEMPLATE_MAIN Template ON (Template.GUID = Register.TEMPLATE_GUID)    ";
            sql += "JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID)   ";
            sql += "JOIN TAG ON TAG.GUID = Register.TAG_GUID  ";
            sql += "JOIN WBS ON TAG.PARENTGUID = WBS.GUID  ";
            sql += "JOIN SCHEDULE ON SCHEDULE.GUID = TAG.SCHEDULEGUID  ";
            sql += "JOIN PROJECT ON PROJECT.GUID = SCHEDULE.PROJECTGUID  ";
            sql += "JOIN USER_MAIN ON USER_MAIN.GUID = Register.CREATEDBY ";
            sql += "WHERE Register.DELETED IS NULL AND Template.DELETED IS NULL AND Workflow.DELETED IS NULL AND SCHEDULE.DELETED IS NULL AND TAG.DELETED IS NULL AND Register.TAG_GUID IS NOT NULL AND PROJECT.GUID = '" + projectGuid + "'   ";
            sql += ") TblTagRegister  ";
            sql += "OUTER APPLY  ";
            sql += "(  ";
            sql += "SELECT   ";
            sql += "Tag = Tag1.NUMBER,   ";
            sql += "TagDescription = Tag1.DESCRIPTION,   ";
            sql += "Area = AreaWBS.NAME,   ";
            sql += "AreaDescription = AreaWBS.DESCRIPTION,   ";
            sql += "System = SystemWBS.NAME,   ";
            sql += "SystemDescription = SystemWBS.DESCRIPTION,   ";
            sql += "Subsystem = SubsystemWBS.NAME, SubsystemDescription = SubsystemWBS.DESCRIPTION ";
            sql += "FROM TAG Tag1   ";
            sql += "JOIN SCHEDULE Sch ON (Sch.GUID = tag1.SCHEDULEGUID)    ";
            sql += "JOIN PROJECT Proj ON (Proj.GUID = Sch.PROJECTGUID)   ";
            sql += "JOIN WBS SubsystemWBS ON (SubsystemWBS.GUID = Tag1.PARENTGUID)   ";
            sql += "JOIN WBS SystemWBS ON (SystemWBS.GUID = SubsystemWBS.PARENTGUID)   ";
            sql += "JOIN WBS AreaWBS ON (AreaWBS.GUID = SystemWBS.PARENTGUID)   ";
            sql += "WHERE Tag1.DELETED IS NULL AND Sch.DELETED IS NULL AND Proj.DELETED IS NULL  ";
            sql += "AND Tag1.GUID = TblTagRegister.TagGuid  ";
            sql += ") TblTagRegisterLookup  ";
            sql += "GROUP BY ProjectNumber, RegisterCreatedDate, RegisterCreatedPerson, RegisterCreatedPerson, Stage, TaskNumber, Template, Description, DISCIPLINE, Tag, TagDescription, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription ";
            sql += "UNION ALL  ";
            sql += "SELECT ProjectNumber, Stage, TaskNumber, Template, Description, Discipline, Tag = NULL, TagDescription = NULL, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, StatusNumber, StatusDate, StatusPerson, RegisterCreatedDate = NULL, RegisterCreatedPerson = NULL, ITRCreatedDate, ITRCreatePerson ";
            sql += "FROM  ";
            sql += "(  ";
            sql += "SELECT ProjectNumber = PROJECT.NUMBER, Stage = Workflow.NAME, TaskNumber = CONCAT(Wbs.NAME, Template.NAME), Template = Template.NAME, Description = Template.DESCRIPTION,   ";
            sql += "Discipline = Template.DISCIPLINE, WBSGuid = Wbs.GUID, ITRGuid = Itr.GUID, tblAllITRStatus.STATUS_NUMBER, tblAllITRStatus.SEQUENCE_NUMBER, tblAllITRStatus.CREATED, ITR_Created = Itr.CREATED,   ";
            sql += "StatusNumber =  ISNULL(tblAllITRStatus.STATUS_NUMBER, -1), StatusDate = ISNULL(tblAllITRStatus.CREATED, Itr.CREATED), StatusPerson, ITRCreatedDate = itr.CREATED, ITRCreatePerson = CONCAT(USER_MAIN.FIRSTNAME, ' ', USER_MAIN.LASTNAME) ";
            sql += "FROM ITR_MAIN Itr    ";
            sql += "JOIN WBS Wbs ON Wbs.guid = Itr.WBS_GUID  ";
            sql += "JOIN SCHEDULE ON SCHEDULE.GUID = Wbs.SCHEDULEGUID  ";
            sql += "JOIN PROJECT ON PROJECT.GUID = SCHEDULE.PROJECTGUID  ";
            sql += "JOIN TEMPLATE_MAIN Template ON (Template.Guid = iTR.TEMPLATE_GUID)   ";
            sql += "JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID)   ";
            sql += "JOIN USER_MAIN ON USER_MAIN.GUID = Itr.CREATEDBY ";
            sql += "OUTER APPLY    ";
            sql += "(  ";
            sql += "SELECT ITR_STATUS.*, StatusPerson = CONCAT(USER_MAIN.FIRSTNAME, ' ', USER_MAIN.LASTNAME) ";
            sql += "FROM ITR_STATUS  ";
            sql += "LEFT JOIN USER_MAIN ON USER_MAIN.GUID = ITR_STATUS.CREATEDBY ";
            sql += "WHERE ITR_STATUS.ITR_MAIN_GUID = Itr.GUID AND Itr.WBS_GUID = Wbs.GUID AND Wbs.DELETED IS NULL  ";
            sql += ") tblAllITRStatus   ";
            sql += "WHERE Wbs.DELETED IS NULL AND Schedule.DELETED IS NULL AND Template.DELETED IS NULL AND Workflow.DELETED IS NULL AND Itr.DELETED IS NULL AND PROJECT.GUID = '" + projectGuid + "'   ";
            sql += ") TblWBSITRLatestStatus  ";
            sql += "OUTER APPLY  ";
            sql += "(  ";
            sql += "SELECT TOP 1 * FROM  ";
            sql += "(  ";
            sql += "SELECT* FROM  ";
            sql += "(  ";
            sql += "SELECT   ";
            sql += "Area = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN ThirdWBS.NAME ELSE SecondWBS.NAME END ELSE FirstWBS.NAME END,  ";
            sql += "AreaDescription =CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN ThirdWBS.DESCRIPTION ELSE SecondWBS.DESCRIPTION END ELSE FirstWBS.DESCRIPTION END,  ";
            sql += "AreaGuid =CASE WHEN FirstWBS.GUID IS NULL THEN CASE WHEN SecondWBS.GUID IS NULL THEN ThirdWBS.GUID ELSE SecondWBS.GUID END ELSE FirstWBS.GUID END,  ";
            sql += "System = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END ELSE SecondWBS.NAME END,  ";
            sql += "SystemDescription = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END ELSE SecondWBS.DESCRIPTION END,  ";
            sql += "SystemGuid = CASE WHEN FirstWBS.GUID IS NULL THEN CASE WHEN SecondWBS.GUID IS NULL THEN NULL ELSE ThirdWBS.GUID END ELSE SecondWBS.GUID END,  ";
            sql += "Subsystem = CASE WHEN FirstWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END,  ";
            sql += "SubsystemDescription = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END,  ";
            sql += "SubsystemGuid = CASE WHEN FirstWBS.GUID IS NULL THEN NULL ELSE ThirdWBS.GUID END  ";
            sql += "FROM PROJECT Project  ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.PROJECTGUID = Project.GUID)  ";
            sql += "JOIN WBS ThirdWBS ON (Schedule.GUID = ThirdWBS.SCHEDULEGUID)  ";
            sql += "LEFT JOIN WBS SecondWBS ON (SecondWBS.GUID = ThirdWBS.PARENTGUID)    ";
            sql += "LEFT JOIN WBS FirstWBS ON (FirstWBS.GUID = SecondWBS.PARENTGUID)    ";
            sql += "WHERE Project.GUID = '" + projectGuid + "' AND Schedule.DELETED IS NULL AND Project.DELETED IS NULL AND ThirdWBS.DELETED IS NULL AND SecondWBS.DELETED IS NULL AND FirstWBS.DELETED IS NULL  ";
            sql += ") TblWBSLookup   ";
            sql += ") TblOrderedWBSLookup WHERE SubsystemGuid = WBSGuid OR SystemGuid = WBSGuid OR AreaGuid = WBSGuid ORDER BY SubsystemGuid, SystemGuid, AreaGuid  ";
            sql += ") TblWBSITRLookup  ";
            sql += "GROUP BY ProjectNumber, ITRCreatedDate, ITRCreatePerson, Stage, TaskNumber, Template, Description, DISCIPLINE, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, StatusNumber, StatusDate , StatusPerson ";
            sql += "UNION ALL  ";
            sql += "SELECT ProjectNumber, Stage, TaskNumber, Template, Description, Discipline, Tag = NULL, TagDescription = NULL, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, StatusNumber = -99, StatusDate = NULL, StatusPerson = NULL, RegisterCreatedDate, RegisterCreatedPerson, ITRCreatedDate = NULL, ITRCreatePerson = NULL ";
            sql += "FROM  ";
            sql += "(  ";
            sql += "SELECT ProjectNumber = PROJECT.NUMBER, Stage = Workflow.NAME, TaskNumber = CONCAT(Wbs.NAME, Template.NAME), Template = Template.NAME, Description = Template.DESCRIPTION,  ";
            sql += "RegisterGUID = Register.GUID, Register.CREATED, Template.GUID, Register.WBS_GUID AS WBSGuid, Workflow.NAME AS WFNAME, Template.DISCIPLINE, Template.NAME, RegisterCreatedDate = Register.CREATED, RegisterCreatedPerson = CONCAT(USER_MAIN.FIRSTNAME, ' ', USER_MAIN.LASTNAME) ";
            sql += "FROM TEMPLATE_REGISTER Register   ";
            sql += "JOIN TEMPLATE_MAIN Template ON (Template.GUID = Register.TEMPLATE_GUID)    ";
            sql += "JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID)   ";
            sql += "JOIN WBS ON WBS.GUID = Register.WBS_GUID  ";
            sql += "JOIN SCHEDULE ON SCHEDULE.GUID = WBS.SCHEDULEGUID  ";
            sql += "JOIN PROJECT ON PROJECT.GUID = SCHEDULE.PROJECTGUID  ";
            sql += "JOIN USER_MAIN ON USER_MAIN.GUID = Register.CREATEDBY ";
            sql += "WHERE Register.WBS_GUID IS NOT NULL AND Register.DELETED IS NULL AND Template.DELETED IS NULL AND Workflow.DELETED IS NULL AND WBS.DELETED IS NULL AND SCHEDULE.DELETED IS NULL AND PROJECT.GUID = '" + projectGuid + "'  ";
            sql += ") TblWBSRegister  ";
            sql += "OUTER APPLY  ";
            sql += "(  ";
            sql += "SELECT TOP 1 * FROM  ";
            sql += "(  ";
            sql += "SELECT* FROM  ";
            sql += "(  ";
            sql += "SELECT   ";
            sql += "Area = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN ThirdWBS.NAME ELSE SecondWBS.NAME END ELSE FirstWBS.NAME END,  ";
            sql += "AreaDescription =CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN ThirdWBS.DESCRIPTION ELSE SecondWBS.DESCRIPTION END ELSE FirstWBS.DESCRIPTION END,  ";
            sql += "AreaGuid =CASE WHEN FirstWBS.GUID IS NULL THEN CASE WHEN SecondWBS.GUID IS NULL THEN ThirdWBS.GUID ELSE SecondWBS.GUID END ELSE FirstWBS.GUID END,  ";
            sql += "System = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END ELSE SecondWBS.NAME END,  ";
            sql += "SystemDescription = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END ELSE SecondWBS.DESCRIPTION END,  ";
            sql += "SystemGuid = CASE WHEN FirstWBS.GUID IS NULL THEN CASE WHEN SecondWBS.GUID IS NULL THEN NULL ELSE ThirdWBS.GUID END ELSE SecondWBS.GUID END,  ";
            sql += "Subsystem = CASE WHEN FirstWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END,  ";
            sql += "SubsystemDescription = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END,  ";
            sql += "SubsystemGuid = CASE WHEN FirstWBS.GUID IS NULL THEN NULL ELSE ThirdWBS.GUID END  ";
            sql += "FROM PROJECT Project  ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.PROJECTGUID = Project.GUID)  ";
            sql += "JOIN WBS ThirdWBS ON (Schedule.GUID = ThirdWBS.SCHEDULEGUID)  ";
            sql += "LEFT JOIN WBS SecondWBS ON (SecondWBS.GUID = ThirdWBS.PARENTGUID)    ";
            sql += "LEFT JOIN WBS FirstWBS ON (FirstWBS.GUID = SecondWBS.PARENTGUID)    ";
            sql += "WHERE Project.GUID = '" + projectGuid + "' AND Schedule.DELETED IS NULL AND Project.DELETED IS NULL AND ThirdWBS.DELETED IS NULL AND SecondWBS.DELETED IS NULL AND FirstWBS.DELETED IS NULL  ";
            sql += ") TblWBSLookup   ";
            sql += ") TblOrderedWBSLookup WHERE SubsystemGuid = WBSGuid OR SystemGuid = WBSGuid OR AreaGuid = WBSGuid ORDER BY SubsystemGuid, SystemGuid, AreaGuid  ";
            sql += ") TblWBSRegisterLookup  ";
            sql += "GROUP BY ProjectNumber, RegisterCreatedDate, RegisterCreatedPerson, Stage, TaskNumber, Template, Description, DISCIPLINE, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription  ";
            sql += ") TblFormatITRStatusName  ";
            sql += ") TblStatusDates  ";
            sql += "GROUP BY DISCIPLINE, AreaDescription, SystemDescription, Subsystem, SubsystemDescription, Stage, Tag, Template, TemplateDescription ";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    projectITRStatusBreakdownReport.Add(new ViewModel_MasterITRStatusReport()
                    {
                        Discipline = dr.Field<string>("DISCIPLINE"),
                        Area = dr.Field<string>("AREAWBS"),
                        System = dr.Field<string>("SYSTEMWBS"),
                        SubsystemName = dr.Field<string>("SUBSYSTEMWBSNAME"),
                        SubsystemDescription = dr.Field<string>("SUBSYSTEMWBSDESC"),
                        Stage = dr.Field<string>("WFNAME"),
                        TagNumber = dr.Field<string>("Tag"),
                        TemplateName = dr.Field<string>("Template"),
                        TemplateDescription = dr.Field<string>("TemplateDescription"),
                        PendingDate = !dr.IsNull("Pending") ? dr.Field<DateTime?>("Pending") : null,
                        PendingPerson = !dr.IsNull("PendingPerson") ? dr.Field<string>("PendingPerson") : null,
                        SavedDate = !dr.IsNull("Saved") ? dr.Field<DateTime?>("Saved") : null,
                        SavedPerson = !dr.IsNull("SavedPerson") ? dr.Field<string>("SavedPerson") : null,
                        InspectedDate = !dr.IsNull("Inspected") ? dr.Field<DateTime?>("Inspected") : null,
                        InspectedPerson = !dr.IsNull("InspectedPerson") ? dr.Field<string>("InspectedPerson") : null,
                        ApprovedDate = !dr.IsNull("Approved") ? dr.Field<DateTime?>("Approved") : null,
                        ApprovedPerson = !dr.IsNull("ApprovedPerson") ? dr.Field<string>("ApprovedPerson") : null,
                        CompletedDate = !dr.IsNull("Completed") ? dr.Field<DateTime?>("Completed") : null,
                        CompletedPerson = !dr.IsNull("CompletedPerson") ? dr.Field<string>("CompletedPerson") : null,
                        ClosedDate = !dr.IsNull("Closed") ? dr.Field<DateTime?>("Closed") : null,
                        ClosedPerson = !dr.IsNull("ClosedPerson") ? dr.Field<string>("ClosedPerson") : null
                    });
                }
            }

            return projectITRStatusBreakdownReport;
        }

        public List<ViewModel_ProjectITRLatestStatus> GenerateProjectITRItemReport(Guid projectGuid)
        {
            List<ViewModel_ProjectITRLatestStatus> projectITRReport = new List<ViewModel_ProjectITRLatestStatus>();
            string sql = "SELECT SCHEDULE.DISCIPLINE, AreaWBS.NAME AS AREAWBSNAME, SystemWBS.NAME AS SYSTEMWBSNAME, SubsystemWBS.NAME AS SUBSYSTEMWBSNAME, Tag.NUMBER, iTR.NAME AS ITRNAME, iTR.DESCRIPTION, ";
            sql += "CASE ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = -1 THEN 'SAVED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 0 THEN 'INSPECTED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 1 THEN 'APPROVED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 2 THEN 'COMPLETED' ";
            sql += "WHEN iTR_Status.STATUS_NUMBER = 3 THEN 'CLOSED' ";
            sql += "ELSE 'N/A' END AS STATUS, ";
            sql += "iTR_Status_Issue.COMMENTS, iTR.CREATED ";
            sql += "FROM ITR_MAIN iTR JOIN TAG Tag ON (Tag.GUID = iTR.TAG_GUID) JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) JOIN PROJECT Project ON (Project.GUID = Schedule.PROJECTGUID) ";
            sql += "LEFT JOIN ITR_STATUS iTR_Status ON (iTR_Status.ITR_MAIN_GUID = iTR.GUID) LEFT JOIN ITR_STATUS_ISSUE iTR_Status_Issue ON (iTR_Status_Issue.ITR_STATUS_GUID = iTR_Status.GUID) ";
            sql += "LEFT JOIN USER_MAIN Users ON (Users.GUID = iTR_Status.CREATEDBY) ";
            sql += "LEFT JOIN WBS SubsystemWBS ON (SubsystemWBS.GUID = Tag.PARENTGUID) ";
            sql += "LEFT JOIN WBS SystemWBS ON (SystemWBS.GUID = SubsystemWBS.PARENTGUID) ";
            sql += "LEFT JOIN WBS AreaWBS ON (AreaWBS.GUID = SystemWBS.PARENTGUID) ";
            sql += "OUTER APPLY (SELECT MAX(ITR_StatusLatest.SEQUENCE_NUMBER) AS LatestSeq FROM ITR_STATUS iTR_StatusLatest WHERE iTR_StatusLatest.ITR_MAIN_GUID = iTR.GUID) LATESTStatusSequence ";
            sql += "WHERE Project.GUID = '" + projectGuid.ToString() + "' AND Tag.DELETED IS NULL AND Schedule.DELETED IS NULL AND Project.DELETED IS NULL AND iTR_Status.DELETED IS NULL ";
            sql += "AND (iTR_Status.SEQUENCE_NUMBER IS NULL OR iTR_Status.SEQUENCE_NUMBER = LATESTStatusSequence.LatestSeq) ";
            sql += "ORDER BY AreaWBS.NAME, SystemWBS.NAME, SubsystemWBS.NAME, Tag.NUMBER, iTR_Status.CREATED";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    projectITRReport.Add(new ViewModel_ProjectITRLatestStatus()
                    {
                        Discipline = dr.Field<string>("DISCIPLINE"),
                        Area_WBS_Name = dr.Field<string>("AREAWBSNAME"),
                        System_WBS_Name = dr.Field<string>("SYSTEMWBSNAME"),
                        Subsystem_WBS_Name = dr.Field<string>("SUBSYSTEMWBSNAME"),
                        Tag_Number = dr.Field<string>("NUMBER"),
                        ITR_Name = dr.Field<string>("ITRNAME"),
                        ITR_Description = dr.Field<string>("DESCRIPTION"),
                        ITR_Status = dr.Field<string>("STATUS"),
                        ITR_Status_Comments = dr.Field<string>("COMMENTS"),
                        Created = dr.Field<DateTime>("CREATED")
                    });
                }
            }

            return projectITRReport;
        }


        public List<ViewModel_ProjectITRStatusByDate> GenerateProjectChronologyReport(Guid projectGuid)
        {
            List<ViewModel_ProjectITRStatusByDate> projectChronoReport = new List<ViewModel_ProjectITRStatusByDate>();
            string sql = "SELECT CONV_TABLE.DISCIPLINE, CONV_TABLE.iCREATED, CONV_TABLE.COUNT_SCORE, CONV_TABLE.TOTAL_SCORE FROM ";
            sql += "( ";
            sql += "SELECT DISTINCT_TABLE.DISCIPLINE, DISTINCT_TABLE.iCREATED, SUM(DISTINCT_TABLE.STATUSNUM) OVER (PARTITION BY DISTINCT_TABLE.DISCIPLINE ORDER BY DISTINCT_TABLE.iCREATED ASC) AS COUNT_SCORE ";
            sql += ",ROW_NUMBER() OVER (PARTITION BY DISTINCT_TABLE.DISCIPLINE, DISTINCT_TABLE.CONVDATE ORDER BY DISTINCT_TABLE.iCREATED DESC) AS ROWID, DISTINCT_TABLE.TOTAL_SCORE FROM ";
            sql += "( ";
            sql += "SELECT DISTINCT SCORE_TABLE.DISCIPLINE, SCORE_TABLE.WBSNAME, SCORE_TABLE.NUMBER, SCORE_TABLE.ITRNAME, SCORE_TABLE.STATUSNUM, CONVERT(VARCHAR(10),SCORE_TABLE.iCREATED,10) AS CONVDATE, SCORE_TABLE.iCREATED, (PLAN_TABLE.ASSIGNED * 4) AS TOTAL_SCORE FROM ";
            sql += "( ";
            sql += "SELECT Sch.DISCIPLINE, Tag.NUMBER, Wbs.NAME AS WBSNAME, iTR.NAME AS ITRNAME,  ";
            sql += "(CASE  ";
            sql += "WHEN iSSUE.REJECTION = 1 THEN -1  ";
            sql += "ELSE 1 END) AS STATUSNUM ";
            sql += ", (CASE WHEN (iSSUE.CREATED IS NULL AND iSTATUS.CREATED IS NULL) THEN iTR.CREATED ";
            sql += "WHEN iSSUE.CREATED IS NULL THEN iSTATUS.CREATED ";
            sql += "ELSE iSSUE.CREATED  ";
            sql += "END) AS iCREATED FROM ITR_MAIN iTR  ";
            sql += "LEFT JOIN ITR_STATUS iSTATUS ON (iTR.GUID = iSTATUS.ITR_MAIN_GUID)  ";
            sql += "LEFT JOIN ITR_STATUS_ISSUE iSSUE ON (iSSUE.ITR_STATUS_GUID = iSTATUS.GUID) ";
            sql += "LEFT JOIN TAG Tag ON (Tag.GUID = iTR.TAG_GUID) JOIN SCHEDULE Sch ON (Sch.GUID = Tag.SCHEDULEGUID) LEFT JOIN WBS Wbs ON (Wbs.GUID = Tag.PARENTGUID)  ";
            sql += "WHERE Sch.PROJECTGUID = '" + projectGuid.ToString() + "' AND iTR.DELETED IS NULL AND iSTATUS.DELETED IS NULL AND Sch.DELETED IS NULL AND Tag.DELETED IS NULL AND Wbs.DELETED IS NULL ";
            sql += ") SCORE_TABLE ";
            sql += "LEFT JOIN ";
            sql += "( ";
            sql += "SELECT Sch.DISCIPLINE, COUNT(DISTINCT Reg.GUID) AS ASSIGNED FROM TAG Tag  ";
            sql += "JOIN SCHEDULE Sch ON (Sch.GUID = tag.SCHEDULEGUID)  ";
            sql += "JOIN TEMPLATE_REGISTER Reg ON (Reg.TAG_GUID = Tag.GUID)  ";
            sql += "LEFT JOIN WBS Wbs ON (Wbs.GUID = Tag.WBSGUID) ";
            sql += "WHERE Sch.PROJECTGUID = '" + projectGuid.ToString() + "' ";
            sql += "AND Sch.DELETED IS NULL AND Tag.DELETED IS NULL AND Reg.DELETED IS NULL ";
            sql += "GROUP BY Sch.DISCIPLINE ";
            sql += ") PLAN_TABLE ON (SCORE_TABLE.DISCIPLINE = PLAN_TABLE.DISCIPLINE) ";
            sql += ") DISTINCT_TABLE ";
            sql += ") CONV_TABLE ";
            sql += "WHERE CONV_TABLE.ROWID = '1' ";


            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    projectChronoReport.Add(new ViewModel_ProjectITRStatusByDate()
                    {
                        Discipline = dr.Field<string>("DISCIPLINE"),
                        Created = dr.Field<DateTime>("iCREATED"),
                        DisciplineRunning = dr.Field<int>("COUNT_SCORE"),
                        DisciplineTotal = dr.Field<int>("TOTAL_SCORE")
                    });
                }
            }

            return projectChronoReport;
        }

        public List<ViewModel_ProjectReport> GenerateProjectITRReport(Guid projectGuid)
        {
            List<ViewModel_ProjectReport> projectReport = new List<ViewModel_ProjectReport>();
            string sql = "SELECT DISCIPLINE, ISNULL(AreaDescription, 'N/A') AS AREAWBS, ISNULL(SystemDescription, 'N/A') AS SYSTEMWBS, ISNULL(Subsystem, 'N/A') AS SUBSYSTEMWBSNAME, ISNULL(SubsystemDescription, 'N/A') AS SUBSYSTEMWBSDESC, ISNULL(Tag, 'N/A') AS Tag, ISNULL(TagDescription, 'N/A') AS TAGDESC, WFNAME = Stage, Template, TemplateDescription, SAVED = SUM(CASE WHEN Status = 'Saved' THEN 1 ELSE 0 END),  PENDING = SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END),  INSPECTED = SUM(CASE WHEN Status = 'Inspected' THEN 1 ELSE 0 END), APPROVED = SUM(CASE WHEN Status = 'Approved' THEN 1 ELSE 0 END), COMPLETED = SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END), CLOSED = SUM(CASE WHEN Status = 'Closed' THEN 1 ELSE 0 END)   ";
            sql += "FROM ";
            sql += "( ";
            sql += "SELECT ProjectNumber, Stage, TaskNumber, Template, TemplateDescription = DESCRIPTION, Discipline = DISCIPLINE, Tag, TagDescription, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, Status =  CASE WHEN StatusNumber = -99 THEN 'Pending' WHEN StatusNumber <= -1 AND StatusNumber > -98 THEN 'Saved' WHEN StatusNumber = 0 THEN 'Inspected' WHEN StatusNumber = 1 THEN 'Approved' WHEN StatusNumber = 2 THEN 'Completed' WHEN StatusNumber >= 3 THEN 'Closed' ELSE 'N/A' END, StatusDate FROM ";
            sql += "( ";
            sql += "SELECT *, ROW_NUMBER() OVER (PARTITION BY Template, Discipline, Tag, Area, System, Subsystem ORDER BY StatusNumber DESC) AS rn FROM ";
            sql += "( ";
            sql += "SELECT ProjectNumber, Stage, TaskNumber, Template, Description, DISCIPLINE, Tag, TagDescription, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, StatusNumber, StatusDate ";
            sql += "FROM ";
            sql += "( ";
            sql += "SELECT ProjectNumber = Project.NUMBER, Stage = Workflow.NAME, TaskNumber = CONCAT(Tag.NUMBER, Template.NAME), Template = Template.NAME, Description = Template.DESCRIPTION,  ";
            sql += "TagGUID = Tag.GUID, Discipline = Template.DISCIPLINE, ITRGuid = Itr.GUID, tblLatestITRStatus.STATUS_NUMBER, tblLatestITRStatus.SEQUENCE_NUMBER, tblLatestITRStatus.CREATED, ";
            sql += "StatusNumber =  ISNULL(tblLatestITRStatus.STATUS_NUMBER, -1), StatusDate = ISNULL(tblLatestITRStatus.CREATED, Itr.CREATED) ";
            sql += "FROM ITR_MAIN Itr   ";
            sql += "JOIN TAG Tag ON Tag.guid = Itr.TAG_GUID ";
            sql += "JOIN WBS ON TAG.PARENTGUID = WBS.GUID ";
            sql += "JOIN Schedule ON Schedule.GUID = Tag.SCHEDULEGUID ";
            sql += "JOIN Project ON Project.GUID = Schedule.PROJECTGUID ";
            sql += "JOIN TEMPLATE_MAIN Template ON (Template.Guid = iTR.TEMPLATE_GUID)  ";
            sql += "JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID)  ";
            sql += "OUTER APPLY   ";
            sql += "( ";
            sql += "SELECT *  ";
            sql += "FROM ITR_STATUS WHERE ITR_STATUS.ITR_MAIN_GUID = Itr.GUID AND (ITR_STATUS.SEQUENCE_NUMBER = (SELECT MAX(iTRStatus.SEQUENCE_NUMBER) FROM ITR_STATUS iTRStatus  ";
            sql += "WHERE iTRStatus.ITR_MAIN_GUID = Itr.GUID) OR ITR_STATUS.SEQUENCE_NUMBER IS NULL) ";
            sql += ") tblLatestITRStatus  ";
            sql += "WHERE itr.DELETED IS NULL AND Tag.DELETED IS NULL AND Schedule.DELETED IS NULL AND Template.DELETED IS NULL AND Workflow.DELETED IS NULL AND Project.GUID = '" + projectGuid + "'  ";
            sql += ") TblTagITRLatestStatus ";
            sql += "OUTER APPLY ";
            sql += "( ";
            sql += "SELECT TblTagITRLatestStatus.ITRGuid, ";
            sql += "Tag = Tag1.NUMBER,  ";
            sql += "TagDescription = Tag1.DESCRIPTION,  ";
            sql += "Area = AreaWBS.NAME,  ";
            sql += "AreaDescription = AreaWBS.DESCRIPTION,  ";
            sql += "System = SystemWBS.NAME,  ";
            sql += "SystemDescription = SystemWBS.DESCRIPTION,  ";
            sql += "Subsystem = SubsystemWBS.NAME, SubsystemDescription = SubsystemWBS.DESCRIPTION ";
            sql += "FROM TAG Tag1  ";
            sql += "JOIN SCHEDULE Sch ON (Sch.GUID = tag1.SCHEDULEGUID)   ";
            sql += "JOIN PROJECT Proj ON (Proj.GUID = Sch.PROJECTGUID)  ";
            sql += "JOIN WBS SubsystemWBS ON (SubsystemWBS.GUID = Tag1.PARENTGUID)  ";
            sql += "JOIN WBS SystemWBS ON (SystemWBS.GUID = SubsystemWBS.PARENTGUID)  ";
            sql += "JOIN WBS AreaWBS ON (AreaWBS.GUID = SystemWBS.PARENTGUID)  ";
            sql += "WHERE Tag1.DELETED IS NULL AND Sch.DELETED IS NULL AND Proj.DELETED IS NULL ";
            sql += "AND Tag1.GUID = TagGUID ";
            sql += ") TblTagITRLookup ";
            sql += "GROUP BY ProjectNumber, Stage, TaskNumber, Template, Description, DISCIPLINE, Tag, TagDescription, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, StatusNumber, StatusDate ";
            sql += "UNION ALL ";
            sql += "SELECT ProjectNumber, Stage, TaskNumber, Template, Description, DISCIPLINE, Tag, TagDescription, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, StatusNumber = -99, StatusDate = NULL ";
            sql += "FROM ";
            sql += "( ";
            sql += "SELECT ProjectNumber = PROJECT.NUMBER, RegisterGUID = Register.GUID, Stage = Workflow.NAME, TaskNumber = CONCAT(Tag.NUMBER, Template.NAME), Template = Template.NAME, Description = Template.DESCRIPTION,  ";
            sql += "Template.GUID, Register.TAG_GUID AS TagGuid, Workflow.NAME AS WFNAME, Template.DISCIPLINE, Template.NAME ";
            sql += "FROM TEMPLATE_REGISTER Register  ";
            sql += "JOIN TEMPLATE_MAIN Template ON (Template.GUID = Register.TEMPLATE_GUID)   ";
            sql += "JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID)  ";
            sql += "JOIN TAG ON TAG.GUID = Register.TAG_GUID ";
            sql += "JOIN WBS ON TAG.PARENTGUID = WBS.GUID ";
            sql += "JOIN SCHEDULE ON SCHEDULE.GUID = TAG.SCHEDULEGUID ";
            sql += "JOIN PROJECT ON PROJECT.GUID = SCHEDULE.PROJECTGUID ";
            sql += "WHERE Register.DELETED IS NULL AND Template.DELETED IS NULL AND Workflow.DELETED IS NULL AND SCHEDULE.DELETED IS NULL AND TAG.DELETED IS NULL AND Register.TAG_GUID IS NOT NULL AND PROJECT.GUID = '" + projectGuid + "'  ";
            sql += ") TblTagRegister ";
            sql += "OUTER APPLY ";
            sql += "( ";
            sql += "SELECT  ";
            sql += "Tag = Tag1.NUMBER,  ";
            sql += "TagDescription = Tag1.DESCRIPTION,  ";
            sql += "Area = AreaWBS.NAME,  ";
            sql += "AreaDescription = AreaWBS.DESCRIPTION,  ";
            sql += "System = SystemWBS.NAME,  ";
            sql += "SystemDescription = SystemWBS.DESCRIPTION,  ";
            sql += "Subsystem = SubsystemWBS.NAME, SubsystemDescription = SubsystemWBS.DESCRIPTION, StatusNumber = -99, StatusDate = NULL ";
            sql += "FROM TAG Tag1  ";
            sql += "JOIN SCHEDULE Sch ON (Sch.GUID = tag1.SCHEDULEGUID)   ";
            sql += "JOIN PROJECT Proj ON (Proj.GUID = Sch.PROJECTGUID)  ";
            sql += "JOIN WBS SubsystemWBS ON (SubsystemWBS.GUID = Tag1.PARENTGUID)  ";
            sql += "JOIN WBS SystemWBS ON (SystemWBS.GUID = SubsystemWBS.PARENTGUID)  ";
            sql += "JOIN WBS AreaWBS ON (AreaWBS.GUID = SystemWBS.PARENTGUID)  ";
            sql += "WHERE Tag1.DELETED IS NULL AND Sch.DELETED IS NULL AND Proj.DELETED IS NULL ";
            sql += "AND Tag1.GUID = TblTagRegister.TagGuid ";
            sql += ") TblTagRegisterLookup ";
            sql += "GROUP BY ProjectNumber, Stage, TaskNumber, Template, Description, DISCIPLINE, Tag, TagDescription, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, StatusNumber, StatusDate ";
            sql += "UNION ALL ";
            sql += "SELECT ProjectNumber, Stage, TaskNumber, Template, Description, Discipline, Tag = NULL, TagDescription = NULL, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription, StatusNumber = MAX(StatusNumber), StatusDate = MAX(StatusDate) ";
            sql += "FROM ";
            sql += "( ";
            sql += "SELECT ProjectNumber = PROJECT.NUMBER, Stage = Workflow.NAME, TaskNumber = CONCAT(Wbs.NAME, Template.NAME), Template = Template.NAME, Description = Template.DESCRIPTION,  ";
            sql += "Discipline = Template.DISCIPLINE, WBSGuid = Wbs.GUID, Itr.GUID, tblLatestITRStatus.STATUS_NUMBER, tblLatestITRStatus.SEQUENCE_NUMBER, tblLatestITRStatus.CREATED, ITR_Created = Itr.CREATED,  ";
            sql += "StatusNumber =  ISNULL(tblLatestITRStatus.STATUS_NUMBER, -1), StatusDate = ISNULL(tblLatestITRStatus.CREATED, Itr.CREATED) ";
            sql += "FROM ITR_MAIN Itr   ";
            sql += "JOIN WBS Wbs ON Wbs.guid = Itr.WBS_GUID ";
            sql += "JOIN SCHEDULE ON SCHEDULE.GUID = Wbs.SCHEDULEGUID ";
            sql += "JOIN PROJECT ON PROJECT.GUID = SCHEDULE.PROJECTGUID ";
            sql += "JOIN TEMPLATE_MAIN Template ON (Template.Guid = iTR.TEMPLATE_GUID)  ";
            sql += "JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID)  ";
            sql += "OUTER APPLY   ";
            sql += "( ";
            sql += "SELECT *  ";
            sql += "FROM ITR_STATUS WHERE ITR_STATUS.ITR_MAIN_GUID = Itr.GUID AND (ITR_STATUS.SEQUENCE_NUMBER = (SELECT MAX(iTRStatus.SEQUENCE_NUMBER) FROM ITR_STATUS iTRStatus WHERE iTRStatus.ITR_MAIN_GUID = Itr.GUID) OR ITR_STATUS.SEQUENCE_NUMBER IS NULL) ";
            sql += ") tblLatestITRStatus  ";
            sql += "WHERE Wbs.DELETED IS NULL AND Schedule.DELETED IS NULL AND Template.DELETED IS NULL AND Workflow.DELETED IS NULL AND Itr.WBS_GUID IS NOT NULL AND Itr.SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) FROM ITR_MAIN Itr2 WHERE Itr2.GUID = Itr.GUID) AND itr.DELETED IS NULL AND PROJECT.GUID = '" + projectGuid + "'  ";
            sql += ") TblWBSITRLatestStatus ";
            sql += "OUTER APPLY ";
            sql += "( ";
            sql += "SELECT TOP 1 * FROM ";
            sql += "( ";
            sql += "SELECT* FROM ";
            sql += "( ";
            sql += "SELECT  ";
            sql += "Area = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN ThirdWBS.NAME ELSE SecondWBS.NAME END ELSE FirstWBS.NAME END, ";
            sql += "AreaDescription =CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN ThirdWBS.DESCRIPTION ELSE SecondWBS.DESCRIPTION END ELSE FirstWBS.DESCRIPTION END, ";
            sql += "AreaGuid =CASE WHEN FirstWBS.GUID IS NULL THEN CASE WHEN SecondWBS.GUID IS NULL THEN ThirdWBS.GUID ELSE SecondWBS.GUID END ELSE FirstWBS.GUID END, ";
            sql += "System = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END ELSE SecondWBS.NAME END, ";
            sql += "SystemDescription = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END ELSE SecondWBS.DESCRIPTION END, ";
            sql += "SystemGuid = CASE WHEN FirstWBS.GUID IS NULL THEN CASE WHEN SecondWBS.GUID IS NULL THEN NULL ELSE ThirdWBS.GUID END ELSE SecondWBS.GUID END, ";
            sql += "Subsystem = CASE WHEN FirstWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END, ";
            sql += "SubsystemDescription = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END, ";
            sql += "SubsystemGuid = CASE WHEN FirstWBS.GUID IS NULL THEN NULL ELSE ThirdWBS.GUID END ";
            sql += "FROM PROJECT Project ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.PROJECTGUID = Project.GUID) ";
            sql += "JOIN WBS ThirdWBS ON (Schedule.GUID = ThirdWBS.SCHEDULEGUID) ";
            sql += "LEFT JOIN WBS SecondWBS ON (SecondWBS.GUID = ThirdWBS.PARENTGUID)   ";
            sql += "LEFT JOIN WBS FirstWBS ON (FirstWBS.GUID = SecondWBS.PARENTGUID)   ";
            sql += "WHERE Project.GUID = '" + projectGuid + "' AND Schedule.DELETED IS NULL AND Project.DELETED IS NULL AND ThirdWBS.DELETED IS NULL AND SecondWBS.DELETED IS NULL AND FirstWBS.DELETED IS NULL ";
            sql += ") TblWBSLookup  ";
            sql += ") TblOrderedWBSLookup WHERE SubsystemGuid = WBSGuid OR SystemGuid = WBSGuid OR AreaGuid = WBSGuid ORDER BY SubsystemGuid, SystemGuid, AreaGuid ";
            sql += ") TblWBSITRLookup ";
            sql += "GROUP BY ProjectNumber, Stage, TaskNumber, Template, Description, DISCIPLINE, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription ";
            sql += "UNION ALL ";
            sql += "SELECT ProjectNumber, Stage, TaskNumber, Template, Description, Discipline, Tag = NULL, TagDescription = NULL, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription,  ";
            sql += "StatusNumber = -99, StatusDate = NULL FROM ";
            sql += "( ";
            sql += "SELECT ProjectNumber = PROJECT.NUMBER, Stage = Workflow.NAME, TaskNumber = CONCAT(Wbs.NAME, Template.NAME), Template = Template.NAME, Description = Template.DESCRIPTION, ";
            sql += "RegisterGUID = Register.GUID, Template.GUID, Register.WBS_GUID AS WBSGuid, Workflow.NAME AS WFNAME, Template.DISCIPLINE, Template.NAME ";
            sql += "FROM TEMPLATE_REGISTER Register  ";
            sql += "JOIN TEMPLATE_MAIN Template ON (Template.GUID = Register.TEMPLATE_GUID)   ";
            sql += "JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID)  ";
            sql += "JOIN WBS ON WBS.GUID = Register.WBS_GUID ";
            sql += "JOIN SCHEDULE ON SCHEDULE.GUID = WBS.SCHEDULEGUID ";
            sql += "JOIN PROJECT ON PROJECT.GUID = SCHEDULE.PROJECTGUID ";
            sql += "WHERE Register.WBS_GUID IS NOT NULL AND Register.DELETED IS NULL AND Template.DELETED IS NULL AND Workflow.DELETED IS NULL AND WBS.DELETED IS NULL AND SCHEDULE.DELETED IS NULL AND PROJECT.GUID = '" + projectGuid + "' ";
            sql += ") TblWBSRegister ";
            sql += "OUTER APPLY ";
            sql += "( ";
            sql += "SELECT TOP 1 * FROM ";
            sql += "( ";
            sql += "SELECT* FROM ";
            sql += "( ";
            sql += "SELECT  ";
            sql += "Area = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN ThirdWBS.NAME ELSE SecondWBS.NAME END ELSE FirstWBS.NAME END, ";
            sql += "AreaDescription =CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN ThirdWBS.DESCRIPTION ELSE SecondWBS.DESCRIPTION END ELSE FirstWBS.DESCRIPTION END, ";
            sql += "AreaGuid =CASE WHEN FirstWBS.GUID IS NULL THEN CASE WHEN SecondWBS.GUID IS NULL THEN ThirdWBS.GUID ELSE SecondWBS.GUID END ELSE FirstWBS.GUID END, ";
            sql += "System = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END ELSE SecondWBS.NAME END, ";
            sql += "SystemDescription = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END ELSE SecondWBS.DESCRIPTION END, ";
            sql += "SystemGuid = CASE WHEN FirstWBS.GUID IS NULL THEN CASE WHEN SecondWBS.GUID IS NULL THEN NULL ELSE ThirdWBS.GUID END ELSE SecondWBS.GUID END, ";
            sql += "Subsystem = CASE WHEN FirstWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END, ";
            sql += "SubsystemDescription = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END, ";
            sql += "SubsystemGuid = CASE WHEN FirstWBS.GUID IS NULL THEN NULL ELSE ThirdWBS.GUID END ";
            sql += "FROM PROJECT Project ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.PROJECTGUID = Project.GUID) ";
            sql += "JOIN WBS ThirdWBS ON (Schedule.GUID = ThirdWBS.SCHEDULEGUID) ";
            sql += "LEFT JOIN WBS SecondWBS ON (SecondWBS.GUID = ThirdWBS.PARENTGUID)   ";
            sql += "LEFT JOIN WBS FirstWBS ON (FirstWBS.GUID = SecondWBS.PARENTGUID)   ";
            sql += "WHERE Project.GUID = '" + projectGuid + "' AND Schedule.DELETED IS NULL AND Project.DELETED IS NULL AND ThirdWBS.DELETED IS NULL AND SecondWBS.DELETED IS NULL AND FirstWBS.DELETED IS NULL ";
            sql += ") TblWBSLookup  ";
            sql += ") TblOrderedWBSLookup WHERE SubsystemGuid = WBSGuid OR SystemGuid = WBSGuid OR AreaGuid = WBSGuid ORDER BY SubsystemGuid, SystemGuid, AreaGuid ";
            sql += ") TblWBSRegisterLookup ";
            sql += "GROUP BY ProjectNumber, Stage, TaskNumber, Template, Description, DISCIPLINE, Area, AreaDescription, System, SystemDescription, Subsystem, SubsystemDescription ";
            sql += ") TblPartition ";
            sql += ") TblFormatITRStatusName WHERE (rn = 1 AND StatusNumber = -99) OR StatusNumber > -99 ";
            sql += ") TblCountStatus ";
            sql += "GROUP BY Discipline, Tag, TagDescription, AreaDescription, SystemDescription, Subsystem, SubsystemDescription, Stage, Template, TemplateDescription ";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);
            if(dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    projectReport.Add(new ViewModel_ProjectReport()
                    {
                        Discipline = (string)dr["DISCIPLINE"],
                        AreaWBS_Name = (string)dr["AREAWBS"],
                        SystemWBS_Name = (string)dr["SYSTEMWBS"],
                        SubsystemWBS_Name = (string)dr["SUBSYSTEMWBSNAME"],
                        SubsystemWBS_Description = (string)dr["SUBSYSTEMWBSDESC"],
                        Workflow_Name = (string)dr["WFNAME"],
                        Pending_Count = (int)dr["PENDING"],
                        Saved_Count = (int)dr["SAVED"],
                        Inspected_Count = (int)dr["INSPECTED"],
                        Approved_Count = (int)dr["APPROVED"],
                        Completed_Count = (int)dr["COMPLETED"],
                        Closed_Count = (int)dr["CLOSED"]
                    });
                }
            }

            return projectReport;
        }

        public List<WebServer_ITRSummary> GenerateTagStatusSummary(Guid projectGuid, string discipline)
        {
            string sql = "SELECT itr.GUID, proj.GUID, tag.Number, itr.Name, isNull(tblTopStatus.STATUS_NUMBER, -1), tblRejection.Rejection ";
            sql += "FROM ITR_MAIN itr JOIN TAG tag ON (itr.TAG_GUID = tag.GUID) ";
            sql += "JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) ";
            sql += "OUTER APPLY ";
            sql += "(SELECT TOP 1 istatus.STATUS_NUMBER FROM ITR_STATUS istatus WHERE istatus.ITR_MAIN_GUID = itr.GUID ORDER BY istatus.SEQUENCE_NUMBER DESC) tblTopStatus ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS Rejection FROM ITR_STATUS_ISSUE issue LEFT JOIN ITR_STATUS istatus ON (istatus.GUID = issue.ITR_STATUS_GUID) ";
            sql += "WHERE istatus.ITR_MAIN_GUID = itr.GUID AND issue.REJECTION = 1) tblRejection ";
            sql += "WHERE proj.GUID = '" + projectGuid + "' AND sch.DISCIPLINE = '" + discipline + "' ";
            sql += "AND itr.DELETED IS NULL AND sch.DELETED IS NULL and proj.DELETED IS NULL ORDER BY itr.NAME";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<WebServer_ITRSummary> SyncITRSummary = new List<WebServer_ITRSummary>();
            if(dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    SyncITRSummary.Add(new WebServer_ITRSummary(
                            new SyncITR(false)
                            {
                                projectGuid = (Guid)dr[1],
                                attachmentName = dr[2].ToString(),
                                templateName = dr[3].ToString(),
                            }) 
                        { 
                            itrStatus = (int)dr[4],
                            itrRejectCount = (int)dr[5]
                        }
                    );
                }
            }

            return SyncITRSummary;
        }

        // Changes 7-May-2015 : Sync tag by entire project
        /// <summary>
        /// Get iTRs linked to tag attachment by project only
        /// </summary>
        public List<SyncITR> GenerateTagSyncByProject(Guid projectGuid)
        {
            string sql = "SELECT tag.Number, itr.Name, proj.GUID FROM ITR_MAIN itr JOIN TAG tag ON (itr.TAG_GUID = tag.GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID)";
            sql += " JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projectGuid + "'";
            sql += " AND itr.DELETED IS NULL AND sch.DELETED IS NULL and proj.DELETED IS NULL";
            sql += " ORDER BY itr.NAME";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<SyncITR> TagITRs = new List<SyncITR>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TagITRs.Add(new SyncITR(false)
                    {
                        attachmentName = dr[0].ToString(),
                        templateName = dr[1].ToString(),
                        projectGuid = (Guid)dr[2]
                    });
                }
            }

            return TagITRs;
        }

        // Changes 7-May-2015 : Sync WBS by entire project
        /// <summary>
        /// Get iTRs linked to wbs attachment by project only
        /// </summary>
        public List<SyncITR> GenerateWBSSyncByProject(Guid projectGuid)
        {
            string sql = "SELECT wbs.NAME, itr.NAME, proj.GUID FROM ITR_MAIN itr JOIN WBS wbs ON (itr.WBS_GUID = wbs.GUID) JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID)";
            sql += " JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projectGuid + "'";
            sql += " AND itr.DELETED IS NULL AND sch.DELETED IS NULL and proj.DELETED IS NULL";
            sql += " ORDER BY itr.NAME";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<SyncITR> WBSITRs = new List<SyncITR>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WBSITRs.Add(new SyncITR(true)
                    {
                        attachmentName = dr[0].ToString(),
                        templateName = dr[1].ToString(),
                        projectGuid = (Guid)dr[2]
                    });
                }
            }

            return WBSITRs;
        }

        //Changes 10-DEC-2014 : Tag sync add projectGUID into SYNCITR
        /// <summary>
        /// Get iTRs linked to tag attachment by project and discipline
        /// </summary>
        public List<SyncITR> GenerateTagSyncByProjectDiscipline(Guid projectGuid, string discipline)
        {
            string sql = "SELECT tag.Number, itr.Name, proj.GUID FROM ITR_MAIN itr JOIN TAG tag ON (itr.TAG_GUID = tag.GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID)";
            sql += " JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projectGuid + "'";
            sql += " AND sch.DISCIPLINE = '" + discipline + "'";
            sql += " AND itr.DELETED IS NULL AND sch.DELETED IS NULL and proj.DELETED IS NULL";
            sql += " ORDER BY itr.NAME";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<SyncITR> TagITRs = new List<SyncITR>();
            if(dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    TagITRs.Add(new SyncITR(false)
                        {
                            attachmentName = dr[0].ToString(),
                            templateName = dr[1].ToString(),
                            projectGuid = (Guid)dr[2]
                        });
                }
            }

            return TagITRs;
        }

        //Changes 12-DEC-2014 : WBS sync add projectGUID into SYNCITR
        /// <summary>
        /// Get iTRs linked to wbs attachment by project and discipline
        /// </summary>
        public List<SyncITR> GenerateWBSSyncByProjectDiscipline(Guid projectGuid, string discipline)
        {
            string sql = "SELECT wbs.NAME, itr.NAME, proj.GUID FROM ITR_MAIN itr JOIN WBS wbs ON (itr.WBS_GUID = wbs.GUID) JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID)";
            sql += " JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projectGuid + "'";
            sql += " AND sch.DISCIPLINE = '" + discipline + "'";
            sql += " AND itr.DELETED IS NULL AND sch.DELETED IS NULL and proj.DELETED IS NULL";
            sql += " ORDER BY itr.NAME";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<SyncITR> WBSITRs = new List<SyncITR>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WBSITRs.Add(new SyncITR(true)
                    {
                        attachmentName = dr[0].ToString(),
                        templateName = dr[1].ToString(),
                        projectGuid = (Guid)dr[2]
                    });
                }
            }

            return WBSITRs;
        }

        /// <summary>
        /// Get by Tag attachment to Project and Discipline and tag number
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable GetByTagProject(Guid projGuid, string discipline = "", bool isCompletedOrClosedOnly = false)
        {
            string sql = "SELECT ITR.GUID, ITR.TAG_GUID, ITR.WBS_GUID, ITR.TEMPLATE_GUID, ITR.SEQUENCE_NUMBER, ITR.NAME, ITR.DESCRIPTION, ITR.REVISION, ITR.DISCIPLINE, ITR.TYPE, ITR = CAST(0x as varbinary(MAX)), ITR.CREATED, ITR.CREATEDBY, ITR.UPDATED, ITR.UPDATEDBY, ITR.DELETED, ITR.DELETEDBY ";
            sql += "FROM ITR_MAIN itr ";
            sql += "JOIN TAG tag ON (itr.TAG_GUID = tag.GUID) ";
            sql += "JOIN WBS SubsystemWBS ON SubsystemWBS.GUID = tag.PARENTGUID ";
            sql += "JOIN WBS SystemWBS ON SystemWBS.GUID = SubsystemWBS.PARENTGUID ";
            sql += "JOIN WBS AreaWBS ON AreaWBS.GUID = SystemWBS.PARENTGUID ";
            sql += "JOIN SCHEDULE sch ON tag.SCHEDULEGUID = sch.GUID ";
            sql += "JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) ";
            sql += "LEFT JOIN ITR_STATUS iTR_Status ON (iTR_Status.ITR_MAIN_GUID = itr.GUID) ";
            sql += "LEFT JOIN ITR_STATUS_ISSUE iTR_Status_Issue ON (iTR_Status_Issue.ITR_STATUS_GUID = iTR_Status.GUID) ";
            sql += "OUTER APPLY ";
            sql += "( ";
            sql += "SELECT MAX(ITR_StatusLatest.SEQUENCE_NUMBER) AS LatestSeq FROM ITR_STATUS iTR_StatusLatest ";
            sql += "WHERE iTR_StatusLatest.ITR_MAIN_GUID = itr.GUID ";
            sql += ") LATESTStatusSequence ";
            sql += "WHERE proj.GUID = '" + projGuid + "' ";
            if (discipline != null && discipline != string.Empty)
                sql += "AND sch.DISCIPLINE = '" + discipline + "'";

            sql += "AND (iTR_Status.SEQUENCE_NUMBER IS NULL OR iTR_Status.SEQUENCE_NUMBER = LATESTStatusSequence.LatestSeq) ";
            if (isCompletedOrClosedOnly)
                sql += " AND (iTR_Status.STATUS_NUMBER = 2 OR iTR_Status.STATUS_NUMBER = 3) ";
            //sql += " ORDER BY itr.NAME";

            sql += "AND itr.DELETED IS NULL AND sch.DELETED IS NULL and proj.DELETED IS NULL ";
            //eliminate issues with multiple ITR status synced
            sql += "GROUP BY ITR.GUID, ITR.TAG_GUID, ITR.WBS_GUID, ITR.TEMPLATE_GUID, ITR.SEQUENCE_NUMBER, ITR.NAME, ITR.DESCRIPTION, ITR.REVISION, ITR.DISCIPLINE, ITR.TYPE, ITR.CREATED, ITR.CREATEDBY, ITR.UPDATED, ITR.UPDATEDBY, ITR.DELETED, ITR.DELETEDBY ";
            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get by Tag attachment to Project and Discipline and tag number
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable GetTagITRByProject(Guid projGuid, List<string> disciplines = null, List<string> subsystemNames = null, List<string> systemNames = null, List<string> areaNames = null, string strTagNumber = "", List<string> stages = null, ProjectLibrary.SearchMode searchMode = ProjectLibrary.SearchMode.Contains, bool isCompletedOrClosedOnly = false)
        {
            string sql = "SELECT ITR.GUID, ITR.TAG_GUID, ITR.WBS_GUID, ITR.TEMPLATE_GUID, ITR.SEQUENCE_NUMBER, ITR.NAME, ITR.DESCRIPTION, ITR.REVISION, ITR.DISCIPLINE, ITR.TYPE, ITR = CAST(0x as varbinary(MAX)), ITR.CREATED, ITR.CREATEDBY, ITR.UPDATED, ITR.UPDATEDBY, ITR.DELETED, ITR.DELETEDBY ";
            sql += "FROM ITR_MAIN itr ";
            sql += "JOIN TAG tag ON (itr.TAG_GUID = tag.GUID) ";
            sql += "JOIN WBS SubsystemWBS ON SubsystemWBS.GUID = tag.PARENTGUID ";
            sql += "JOIN WBS SystemWBS ON SystemWBS.GUID = SubsystemWBS.PARENTGUID ";
            sql += "JOIN WBS AreaWBS ON AreaWBS.GUID = SystemWBS.PARENTGUID ";
            sql += "JOIN SCHEDULE sch ON tag.SCHEDULEGUID = sch.GUID ";
            sql += "JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) ";
            sql += "LEFT JOIN ITR_STATUS iTR_Status ON (iTR_Status.ITR_MAIN_GUID = itr.GUID) ";
            sql += "LEFT JOIN ITR_STATUS_ISSUE iTR_Status_Issue ON (iTR_Status_Issue.ITR_STATUS_GUID = iTR_Status.GUID) ";
            sql += "OUTER APPLY ";
            sql += "( ";
            sql += "SELECT MAX(ITR_StatusLatest.SEQUENCE_NUMBER) AS LatestSeq FROM ITR_STATUS iTR_StatusLatest ";
            sql += "WHERE iTR_StatusLatest.ITR_MAIN_GUID = itr.GUID ";
            sql += ") LATESTStatusSequence ";
            sql += "WHERE proj.GUID = '" + projGuid + "' ";
            sql += "AND (iTR_Status.SEQUENCE_NUMBER IS NULL OR iTR_Status.SEQUENCE_NUMBER = LATESTStatusSequence.LatestSeq) ";

            if(subsystemNames != null)
            {
                string subSystemQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(subsystemNames);
                sql += "AND SubsystemWBS.NAME IN (" + subSystemQueryConcatenation + ") ";
            }
            else if(systemNames != null)
            {
                string systemQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(systemNames);
                sql += "AND SystemWBS.NAME IN (" + systemQueryConcatenation + ") ";
            }
            else if (areaNames != null)
            {
                string areaQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(areaNames);
                sql += "AND AreaWBS.NAME IN (" + areaQueryConcatenation + ") ";
            }

            if (strTagNumber != null && strTagNumber != string.Empty)
            {
                if (searchMode == SearchMode.Starts_With)
                    sql += "AND (tag.NUMBER LIKE '" + strTagNumber + "%') ";
                else if (searchMode == SearchMode.Ends_With)
                    sql += "AND (tag.NUMBER LIKE '%" + strTagNumber + "') ";
                else
                    sql += "AND (tag.NUMBER LIKE '%" + strTagNumber + "%') ";
            }

            if (disciplines != null)
            {
                string disciplineQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(disciplines);
                sql += "AND itr.DISCIPLINE IN (" + disciplineQueryConcatenation + ") ";
            }

            sql += " AND itr.DELETED IS NULL AND sch.DELETED IS NULL AND proj.DELETED IS NULL AND AreaWBS.DELETED IS NULL AND SystemWBS.DELETED IS NULL AND SubsystemWBS.DELETED IS NULL ";

            if (isCompletedOrClosedOnly)
                sql += " AND (iTR_Status.STATUS_NUMBER = 2 OR iTR_Status.STATUS_NUMBER >= 3) ";

            sql += " AND itr.DELETED IS NULL AND sch.DELETED IS NULL AND proj.DELETED IS NULL AND AreaWBS.DELETED IS NULL AND SystemWBS.DELETED IS NULL AND SubsystemWBS.DELETED IS NULL";
            //eliminate issues with multiple ITR status synced
            sql += " GROUP BY ITR.GUID, ITR.TAG_GUID, ITR.WBS_GUID, ITR.TEMPLATE_GUID, ITR.SEQUENCE_NUMBER, ITR.NAME, ITR.DESCRIPTION, ITR.REVISION, ITR.DISCIPLINE, ITR.TYPE, ITR.CREATED, ITR.CREATEDBY, ITR.UPDATED, ITR.UPDATEDBY, ITR.DELETED, ITR.DELETEDBY";
            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN;
            else
                return null;
        }


        /// <summary>
        /// Get by WBS attachment to Project and Discipline
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable GetByWBSProject(Guid projGuid, bool isCompletedOrClosedOnly = false, List<string> disciplines = null, List<string> subsystemNames = null, List<string> systemNames = null, List<string> areaNames = null, List<string> stages = null, ProjectLibrary.SearchMode searchMode = ProjectLibrary.SearchMode.Contains)
        {
            string sql = "SELECT GUID, TAG_GUID, WBS_GUID, TEMPLATE_GUID, SEQUENCE_NUMBER, NAME, DESCRIPTION, REVISION, DISCIPLINE, TYPE, ITR = CAST(0x as varbinary(MAX)), CREATED, CREATEDBY, UPDATED, UPDATEDBY, DELETED, DELETEDBY ";
            sql += "FROM ";
            sql += "( ";
            sql += "SELECT ITR.GUID, ITR.TAG_GUID, ITR.WBS_GUID, ITR.TEMPLATE_GUID, ITR.SEQUENCE_NUMBER, ITR.NAME, ITR.DESCRIPTION, ITR.REVISION, ITR.DISCIPLINE, ITR.TYPE, ITR = CAST(0x as varbinary(MAX)), ITR.CREATED, ITR.CREATEDBY, ITR.UPDATED, ITR.UPDATEDBY, ITR.DELETED, ITR.DELETEDBY, ";
            sql += "Area = CASE WHEN Parent1WBS.NAME IS NULL AND Parent2WBS.NAME IS NULL THEN Parent3WBS.NAME ELSE CASE WHEN Parent1WBS.NAME IS NULL THEN Parent2WBS.NAME ELSE Parent1WBS.NAME END END, ";
            sql += "[System] = CASE WHEN Parent1WBS.NAME IS NULL AND Parent2WBS.NAME IS NULL THEN NULL ELSE CASE WHEN Parent1WBS.NAME IS NULL THEN Parent3WBS.NAME ELSE Parent2WBS.NAME END END, ";
            sql += "Subsystem = CASE WHEN Parent1WBS.NAME IS NULL AND Parent2WBS.NAME IS NULL THEN NULL ELSE CASE WHEN Parent1WBS.NAME IS NULL THEN NULL ELSE Parent3WBS.NAME END END ";
            sql += "FROM ITR_MAIN itr ";
            sql += "JOIN WBS Parent3WBS ON Parent3WBS.GUID = itr.WBS_GUID ";
            sql += "LEFT JOIN WBS Parent2WBS ON Parent2WBS.GUID = Parent3WBS.PARENTGUID ";
            sql += "LEFT JOIN WBS Parent1WBS ON Parent1WBS.GUID = Parent2WBS.PARENTGUID ";
            sql += "JOIN SCHEDULE sch ON (sch.GUID = Parent3WBS.SCHEDULEGUID) ";
            sql += "JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) ";
            sql += "LEFT JOIN ITR_STATUS iTR_Status ON (iTR_Status.ITR_MAIN_GUID = itr.GUID) ";
            sql += "LEFT JOIN ITR_STATUS_ISSUE iTR_Status_Issue ON (iTR_Status_Issue.ITR_STATUS_GUID = iTR_Status.GUID) ";
            sql += "OUTER APPLY (SELECT MAX(ITR_StatusLatest.SEQUENCE_NUMBER) AS LatestSeq FROM ITR_STATUS iTR_StatusLatest WHERE iTR_StatusLatest.ITR_MAIN_GUID = itr.GUID ) LATESTStatusSequence ";
            sql += "WHERE proj.GUID = '" + projGuid + "' ";
            sql += "AND itr.DELETED IS NULL AND sch.DELETED IS NULL and proj.DELETED IS NULL AND (iTR_Status.SEQUENCE_NUMBER IS NULL OR iTR_Status.SEQUENCE_NUMBER = LATESTStatusSequence.LatestSeq) ";

            if (isCompletedOrClosedOnly)
                sql += "AND (iTR_Status.STATUS_NUMBER = 2 OR iTR_Status.STATUS_NUMBER = 3) ";

            //eliminate issues with multiple ITR status synced
            sql += "GROUP BY Parent1WBS.NAME, Parent2WBS.NAME, Parent3WBS.NAME, ITR.GUID, ITR.TAG_GUID, ITR.WBS_GUID, ITR.TEMPLATE_GUID, ITR.SEQUENCE_NUMBER, ITR.NAME, ITR.DESCRIPTION, ITR.REVISION, ITR.DISCIPLINE, ITR.TYPE, ITR.CREATED, ITR.CREATEDBY, ITR.UPDATED, ITR.UPDATEDBY, ITR.DELETED, ITR.DELETEDBY";
            sql += ") TblQuery ";
            sql += "WHERE 1 = 1 ";

            if (disciplines != null)
            {
                string disciplineQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(disciplines);
                sql += "AND DISCIPLINE IN (" + disciplineQueryConcatenation + ") ";
            }

            if (subsystemNames != null)
            {
                string subSystemQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(subsystemNames);
                sql += "AND Subsystem IN (" + subSystemQueryConcatenation + ") ";
            }
            else if(systemNames != null)
            {
                string systemQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(systemNames);
                sql += "AND [System] IN (" + systemQueryConcatenation + ") ";
            }
            else if(areaNames != null)
            {
                string areaQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(areaNames);
                sql += "AND Area IN (" + areaQueryConcatenation + ") ";
            }

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get by Tag attachment to Project and Discipline and WBS name
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable GetTagITRByProjectWBSName(Guid projGuid, string strSubsystemName = "", string strSystemName = "", string strAreaName = "", SearchMode searchMode = SearchMode.Contains)
        {
            string sql = "SELECT " +
            "ITR.GUID, ITR.TAG_GUID, ITR.WBS_GUID, ITR.TEMPLATE_GUID, ITR.SEQUENCE_NUMBER, ITR.NAME, ITR.DESCRIPTION, ITR.REVISION, ITR.DISCIPLINE, " +
            "ITR.TYPE, ITR = CAST(0x as varbinary(MAX)), ITR.CREATED, ITR.CREATEDBY, ITR.UPDATED, ITR.UPDATEDBY, ITR.DELETED, ITR.DELETEDBY " +
            "FROM ITR_MAIN itr JOIN TAG tag ON (itr.TAG_GUID = tag.GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID)";
            sql += " JOIN WBS SubsystemWBS ON SubsystemWBS.GUID = tag.PARENTGUID";
            sql += " JOIN WBS SystemWBS ON SystemWBS.GUID = SubsystemWBS.PARENTGUID";
            sql += " JOIN WBS AreaWBS ON AreaWBS.GUID = SystemWBS.PARENTGUID";
            sql += " JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";

            if (strSubsystemName != string.Empty)
            {
                if(searchMode == SearchMode.Starts_With)
                    sql += " AND (AreaWBS.NAME LIKE '" + strAreaName + "%')";
                else if (searchMode == SearchMode.Ends_With)
                    sql += " AND (AreaWBS.NAME LIKE '%" + strAreaName + "')";
                else
                    sql += " AND (AreaWBS.NAME LIKE '%" + strAreaName + "%')";
            }
            if (strSystemName != string.Empty)
            {
                if (searchMode == SearchMode.Starts_With)
                    sql += " AND (SystemWBS.NAME LIKE '" + strSystemName + "%')";
                else if (searchMode == SearchMode.Ends_With)
                    sql += " AND (SystemWBS.NAME LIKE '%" + strSystemName + "')";
                else
                    sql += " AND (SystemWBS.NAME LIKE '%" + strSystemName + "%')";
            }
            if (strAreaName != string.Empty)
            {
                if (searchMode == SearchMode.Starts_With)
                    sql += " AND (SubsystemWBS.NAME LIKE '" + strSubsystemName + "%')";
                else if (searchMode == SearchMode.Ends_With)
                    sql += " AND (SubsystemWBS.NAME LIKE '%" + strSubsystemName + "')";
                else
                    sql += " AND (SubsystemWBS.NAME LIKE '%" + strSubsystemName + "%')";
            }

            sql += " AND AreaWBS.DELETED IS NULL AND SystemWBS.DELETED IS NULL AND SubsystemWBS.DELETED IS NULL";
            sql += " AND itr.DELETED IS NULL AND sch.DELETED IS NULL and proj.DELETED IS NULL";
            //sql += " ORDER BY itr.NAME";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get by WBS attachment to Project and Discipline and WBS Name
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable GetByWBSProjectDisciplineWBS(Guid projGuid, string discipline = "")
        {
            string sql = "SELECT " +
            "ITR.GUID, ITR.TAG_GUID, ITR.WBS_GUID, ITR.TEMPLATE_GUID, ITR.SEQUENCE_NUMBER, ITR.NAME, ITR.DESCRIPTION, ITR.REVISION, ITR.DISCIPLINE, " +
            "ITR.TYPE, ITR = CAST(0x as varbinary(MAX)), ITR.CREATED, ITR.CREATEDBY, ITR.UPDATED, ITR.UPDATEDBY, ITR.DELETED, ITR.DELETEDBY " +
            "FROM ITR_MAIN itr JOIN WBS wbs ON (itr.WBS_GUID = wbs.GUID) JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID)";
            sql += " JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";
            if (discipline != string.Empty)
                sql += " AND sch.DISCIPLINE = '" + discipline + "'";
            sql += " AND itr.DELETED IS NULL AND sch.DELETED IS NULL and proj.DELETED IS NULL";
            sql += " ORDER BY itr.NAME";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get by WBS attachment to Project and Discipline and number
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable GetWBSITRByProjectDisciplineWBSName(Guid projGuid, string searchName, string discipline = "")
        {
            string sql = "SELECT " +
            "ITR.GUID, ITR.TAG_GUID, ITR.WBS_GUID, ITR.TEMPLATE_GUID, ITR.SEQUENCE_NUMBER, ITR.NAME, ITR.DESCRIPTION, ITR.REVISION, ITR.DISCIPLINE, " +
            "ITR.TYPE, ITR = CAST(0x as varbinary(MAX)), ITR.CREATED, ITR.CREATEDBY, ITR.UPDATED, ITR.UPDATEDBY, ITR.DELETED, ITR.DELETEDBY " +
            "FROM ITR_MAIN itr JOIN WBS wbs ON (itr.WBS_GUID = wbs.GUID) JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID)";
            sql += " JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";
            if (discipline != string.Empty)
                sql += " AND sch.DISCIPLINE = '" + discipline + "'";
            sql += " AND wbs.NAME LIKE '%" + searchName + "%'";
            sql += " AND itr.DELETED IS NULL AND sch.DELETED IS NULL and proj.DELETED IS NULL";
            sql += " ORDER BY itr.NAME";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Gets the collection of ITR saved for the tag or wbs
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable GetByTagWBS(Guid wbsTagGuid)
        {
            string sql = "SELECT * FROM ITR_MAIN WHERE DELETED IS NULL";
            sql += " AND (TAG_GUID = '" + wbsTagGuid + "' OR WBS_GUID = '" + wbsTagGuid + "')";
            sql += " ORDER BY NAME";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Gets the ITR saved for the tag or wbs
        /// </summary>
        public dsITR_MAIN.ITR_MAINRow GetByTagWBSTemplate(Guid wbsTagGuid, Guid templateGuid)
        {
            string sql = "SELECT * FROM ITR_MAIN WHERE TEMPLATE_GUID = '" + templateGuid + "'";
            sql += " AND DELETED IS NULL";
            sql += " AND (TAG_GUID = '" + wbsTagGuid + "' OR WBS_GUID = '" + wbsTagGuid + "')";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get WBS/Tag template by list of WBS/Tag Guids and Template Guid
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable GetByTagWBSTemplate(List<ITR_Refresh_Item> ITRRefreshItems)
        {
            if (ITRRefreshItems.Count == 0)
                return null;

            //all parallel items should have the same template guid
            Guid templateGuid = ITRRefreshItems[0].Template_GUID;
            string sql = "SELECT * FROM ITR_MAIN WHERE TEMPLATE_GUID = '" + templateGuid + "'";
            sql += " AND DELETED IS NULL AND (";
            foreach (ITR_Refresh_Item ITRRefreshItem in ITRRefreshItems)
            {
                if (ITRRefreshItem.isWBS)
                    sql += " WBS_GUID = '" + ITRRefreshItem.WBSTagGuid + "' OR";
                else
                    sql += " TAG_GUID = '" + ITRRefreshItem.WBSTagGuid + "' OR";
            }

            sql = sql.Substring(0, sql.Length - 3);
            sql += ")";
            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
            {
                foreach(ITR_Refresh_Item ITRRefreshItem in ITRRefreshItems)
                {
                    dsITR_MAIN.ITR_MAINRow findITR = dtITR_MAIN.FirstOrDefault(obj => 
                        ((!obj.IsTAG_GUIDNull() && obj.TAG_GUID == ITRRefreshItem.WBSTagGuid)
                        || (!obj.IsWBS_GUIDNull() && obj.WBS_GUID == ITRRefreshItem.WBSTagGuid)));

                    if(findITR != null)
                        ITRRefreshItem.ITR_GUID = findITR.GUID;
                }
                return dtITR_MAIN;
            }
            else
                return null;
        }


        /// <summary>
        /// Get WBS/Tag template by list of WBS/Tag Guids and Template Guid
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable GetByTagWBSTemplateRevision(List<ITR_Refresh_Item> ITRRefreshItems)
        {
            if (ITRRefreshItems.Count == 0)
                return null;

            //all parallel items should have the same template guid
            Guid templateGuid = ITRRefreshItems[0].Template_GUID;
            string sql = "SELECT * FROM ITR_MAIN WHERE TEMPLATE_GUID = '" + templateGuid + "'";
            sql += " AND DELETED IS NULL AND (";
            foreach (ITR_Refresh_Item ITRRefreshItem in ITRRefreshItems)
            {
                if (ITRRefreshItem.isWBS)
                    sql += " WBS_GUID = '" + ITRRefreshItem.WBSTagGuid + "' OR";
                else
                    sql += " TAG_GUID = '" + ITRRefreshItem.WBSTagGuid + "' OR";
            }

            sql = sql.Substring(0, sql.Length - 3);
            sql += ")";
            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
            {
                foreach (ITR_Refresh_Item ITRRefreshItem in ITRRefreshItems)
                {
                    dsITR_MAIN.ITR_MAINRow findITR = dtITR_MAIN.FirstOrDefault(obj =>
                        ((!obj.IsTAG_GUIDNull() && obj.TAG_GUID == ITRRefreshItem.WBSTagGuid)
                        || (!obj.IsWBS_GUIDNull() && obj.WBS_GUID == ITRRefreshItem.WBSTagGuid)));

                    if (findITR != null)
                        ITRRefreshItem.ITR_GUID = findITR.GUID;
                }
                return dtITR_MAIN;
            }
            else
                return null;
        }

        //Changes 10-DEC-2014 Allow same tag number to be added per project
        /// <summary>
        /// Gets the ITR by tag number and ITR Name
        /// </summary>
        public dsITR_MAIN.ITR_MAINRow GetByTagName(string tagNumber, string iTRName, Guid projectGuid)
        {
            string sql = "SELECT itr.* FROM ITR_MAIN itr JOIN TAG tag ON (tag.GUID = itr.TAG_GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE tag.NUMBER = '" + tagNumber + "' AND itr.NAME = '" + iTRName + "'";
            sql += " AND proj.GUID = '" + projectGuid + "'";
            sql += " AND itr.DELETED IS NULL AND tag.DELETED IS NULL";
            sql += " AND sch.DELETED IS NULL AND proj.DELETED IS NULL";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get deleted ITR
        /// </summary>
        public dsITR_MAIN.ITR_MAINRow GetDeletedITR(Guid guid)
        {
            string sql = "SELECT * FROM ITR_MAIN";
            sql += " WHERE GUID = '" + guid + "' AND DELETED IS NOT NULL";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN[0];
            else
                return null;
        }

        /// 12-DEC-2014 Fix: add WBS goes by projectGuid because WBS_ADD is restricting only to projectGuid
        /// <summary>
        /// Gets the ITR by tag number and ITR Name
        /// </summary>
        public dsITR_MAIN.ITR_MAINRow GetByWBSName(string wbsName, string iTRName, Guid projectGuid)
        {
            string sql = "SELECT itr.* FROM ITR_MAIN itr JOIN WBS wbs ON (wbs.GUID = itr.WBS_GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE wbs.NAME = '" + wbsName + "' AND itr.NAME = '" + iTRName + "'";
            sql += " AND proj.GUID = '" + projectGuid + "'";
            sql += " AND itr.DELETED IS NULL AND wbs.DELETED IS NULL";
            sql += " AND sch.DELETED IS NULL AND proj.DELETED IS NULL";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get the sequence of ITR by tag/wbs and Template
        /// </summary>
        public int GetSequence(Guid tagWBSGuid, Guid templateGuid)
        {
            string sql = "SELECT * FROM ITR_MAIN WHERE (TAG_GUID = '" + tagWBSGuid + "'";
            sql += " OR WBS_GUID = '" + tagWBSGuid + "') AND TEMPLATE_GUID = '" + templateGuid + "'";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return (int)dtITR_MAIN.Rows.Count;
            else
                return -1;
        }

        public dsITR_MAIN.ITR_MAINRow GetBy_WBSTAGGUID_TEMPLATENAME(Guid WBSTagGuid, string templateName)
        {
            string sql = "SELECT TOP 1 * FROM ITR_MAIN WHERE (TAG_GUID = '" + WBSTagGuid + "' OR WBS_GUID = '" + WBSTagGuid + "') ";
            sql += " AND NAME = '" + templateName + "' ORDER BY SEQUENCE_NUMBER DESC";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return dtITR_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get the WBS/Tag ITR as ValuePair with template name and GUID
        /// </summary>
        /// <param name="tagWBSGuid"></param>
        /// <returns></returns>
        public List<PunchlistITRTemplate> GetPunchlistVP_WBSTagITR(Guid tagWBSGuid)
        {
            string sql = "SELECT iTR.GUID AS ITRGUID, iTR.TEMPLATE_GUID AS TEMPLATEGUID, template.NAME AS TEMPLATENAME ";
            sql += " FROM ITR_MAIN iTR LEFT JOIN TEMPLATE_MAIN template ";
            sql += " ON (iTR.TEMPLATE_GUID = template.GUID) ";
            sql += " WHERE (iTR.TAG_GUID = '" + tagWBSGuid + "' OR iTR.WBS_GUID = '" + tagWBSGuid + "') ";
            sql += " AND iTR.DELETED IS NULL"; //template.DELETED is ignored so that name can be retrieved even if template was deleted

            DataTable dtWBSTagTemplate = new DataTable();
            ExecuteQuery(sql, dtWBSTagTemplate);

            List<PunchlistITRTemplate> vpWBSTagTemplate = new List<PunchlistITRTemplate>();
            if (dtWBSTagTemplate.Rows.Count > 0)
            {
                foreach(DataRow drWBSTagTemplate in dtWBSTagTemplate.Rows)
                {
                    Guid ITRGUID = new Guid(drWBSTagTemplate["ITRGUID"].ToString());
                    Guid TEMPLATEGUID = new Guid(drWBSTagTemplate["TEMPLATEGUID"].ToString());
                    vpWBSTagTemplate.Add(new PunchlistITRTemplate(drWBSTagTemplate["TEMPLATENAME"].ToString(), ITRGUID, TEMPLATEGUID));
                }
            }

            return vpWBSTagTemplate;
        }

        /// <summary>
        /// Remove a particular ITR by GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM ITR_MAIN WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsITR_MAIN.ITR_MAINDataTable dtITR = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR);

            if (dtITR != null)
            {
                dsITR_MAIN.ITR_MAINRow drITR = dtITR[0];
                drITR.DELETED = DateTime.Now;
                drITR.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drITR);

                return true;
            }

            return false;
        }

        public List<tagCount> GetTagCompletedCount(Guid scheduleGuid)
        {
            string sql = "SELECT tag.GUID AS TAG_GUID, tag.WBSGUID, tag.NUMBER AS NUMBER, tagitr.ITR_COUNT FROM TAG tag LEFT JOIN";
            sql += " (SELECT itr.TAG_GUID, COUNT(*) AS ITR_COUNT FROM ITR_MAIN itr JOIN TAG tag2 ON (itr.TAG_GUID = tag2.GUID)";
            sql += " WHERE itr.DELETED IS NULL AND tag2.DELETED IS NULL GROUP BY itr.TAG_GUID)";
            sql += " tagitr ON (tagitr.TAG_GUID = tag.GUID)";
            sql += " WHERE tag.SCHEDULEGUID = '" + scheduleGuid + "' AND tag.DELETED IS NULL";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<tagCount> tiSaved = new List<tagCount>();
            foreach (DataRow dr in dt.Rows)
            {
                tiSaved.Add(new tagCount()
                {
                    tcTag = new ValuePair(dr["NUMBER"].ToString(), (Guid)dr["TAG_GUID"]),
                    tcCompletedCount = dr.IsNull("ITR_COUNT") ? 0 : Convert.ToInt32(dr["ITR_COUNT"].ToString())
                });
            }

            return tiSaved;
        }

        public List<wbsCount> GetWBSCompletedCount(Guid scheduleGuid)
        {
            string sql = "SELECT wbs.GUID AS WBS_GUID, wbs.NAME AS WBS_NAME, ITR_COUNT FROM WBS wbs LEFT JOIN";
            sql += " (SELECT itr.WBS_GUID, COUNT(*) AS ITR_COUNT FROM ITR_MAIN itr JOIN WBS wbs2 ON (itr.WBS_GUID = wbs2.GUID)";
            sql += " WHERE itr.DELETED IS NULL AND wbs2.DELETED IS NULL GROUP BY itr.WBS_GUID)";
            sql += " wbsitr ON (wbsitr.WBS_GUID = wbs.GUID)";
            sql += " WHERE wbs.SCHEDULEGUID = '" + scheduleGuid + "' AND wbs.DELETED IS NULL";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<wbsCount> wiIncompletes = new List<wbsCount>();
            foreach (DataRow dr in dt.Rows)
            {
                wiIncompletes.Add(new wbsCount()
                {
                    wcWBS = new ValuePair(dr["WBS_NAME"].ToString(), (Guid)dr["WBS_GUID"]),
                    wcTotalCount = dr.IsNull("ITR_COUNT") ? 0 : Convert.ToInt32(dr["ITR_COUNT"].ToString())
                });
            }

            return wiIncompletes;
        }

        /// <summary>
        /// Update multiple itr
        /// </summary>
        public void Save(dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN)
        {
            _adapter.Update(dtITR_MAIN);
        }

        /// <summary>
        /// Update one itr
        /// </summary>
        public void Save(dsITR_MAIN.ITR_MAINRow drITR_MAIN)
        {
            _adapter.Update(drITR_MAIN);
        }
        #endregion
        public void Dispose()
        {
            if (_adapter != null)
            {
                _adapter.Dispose();
                _adapter = null;
            }
        }
    }
}