using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsPUNCHLIST_MAINTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterPUNCHLIST_MAIN : SQLBase, IDisposable
    {
        private PUNCHLIST_MAINTableAdapter _adapter;

        public AdapterPUNCHLIST_MAIN()
            : base()
        {
            _adapter = new PUNCHLIST_MAINTableAdapter(Variables.ConnStr);
        }

        public AdapterPUNCHLIST_MAIN(string connStr)
            : base(connStr)
        {
            _adapter = new PUNCHLIST_MAINTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        /// <summary>
        /// Get a particular punchlist by GUID
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow GetBy(Guid punchlistGuid)
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN WHERE GUID = '" + punchlistGuid + "'";
            sql += " AND DELETED IS NULL";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_MAIN = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_MAIN);

            if (dtPUNCHLIST_MAIN.Rows.Count > 0)
                return dtPUNCHLIST_MAIN[0];
            else
                return null;
        }

        public List<ViewModel_PunchlistReport> GeneratePunchlistReport(Guid projectGuid, bool priorityAOnly = false)
        {
            List<ViewModel_PunchlistReport> punchlistReport = new List<ViewModel_PunchlistReport>();
            string sql = "SELECT Sch.DISCIPLINE ";
            sql += ", IsNull(WBS.NAME, 'N/A') AS WBSNAME ";
            sql += ", IsNull(Workflow.NAME, 'N/A') AS WFNAME ";
            sql += ", IsNull(iTR.NAME, 'N/A') AS ITRNAME ";
            sql += ", IsNull(Tag.NUMBER, 'N/A') AS NUMBER ";
            sql += ", Punchlist.DESCRIPTION ";
            sql += ", SUM(CASE WHEN (pStatus.STATUS_NUMBER IS NULL OR pStatus.STATUS_NUMBER = -1 ) THEN 1 ELSE 0 END) AS SAVED ";
            sql += ", SUM(CASE WHEN pStatus.STATUS_NUMBER = 0 THEN 1 ELSE 0 END) AS CATEGORISED ";
            sql += ", SUM(CASE WHEN pStatus.STATUS_NUMBER = 1 THEN 1 ELSE 0 END) AS INSPECTED ";
            sql += ", SUM(CASE WHEN pStatus.STATUS_NUMBER = 2 THEN 1 ELSE 0 END) AS APPROVED ";
            sql += ", SUM(CASE WHEN pStatus.STATUS_NUMBER = 3 THEN 1 ELSE 0 END) AS COMPLETED ";
            sql += ", SUM(CASE WHEN pStatus.STATUS_NUMBER = 4 THEN 1 ELSE 0 END) AS CLOSED  ";
            sql += "FROM PUNCHLIST_MAIN Punchlist  ";
            sql += "JOIN TAG Tag ON (Tag.GUID = Punchlist.TAG_GUID)  ";
            sql += "JOIN SCHEDULE Sch ON (Sch.GUID = Tag.SCHEDULEGUID)  ";
            sql += "LEFT JOIN WBS Wbs ON (Tag.WBSGUID = Wbs.GUID)  ";
            sql += "LEFT JOIN ITR_MAIN iTR ON (iTR.GUID = Punchlist.ITR_GUID)  ";
            sql += "LEFT JOIN TEMPLATE_MAIN Template ON (iTR.TEMPLATE_GUID = Template.GUID)  ";
            sql += "LEFT JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID)  ";
            sql += "LEFT JOIN PUNCHLIST_STATUS pStatus ON (pStatus.PUNCHLIST_MAIN_GUID = Punchlist.GUID)  ";
            sql += "OUTER APPLY ";
            sql += "(SELECT MAX(pStatusLookup.SEQUENCE_NUMBER) AS pStatusLatestSeq FROM PUNCHLIST_MAIN PunchlistLookup  ";
            sql += "LEFT JOIN PUNCHLIST_STATUS pStatusLookup ON (pStatusLookup.PUNCHLIST_MAIN_GUID = PunchlistLookup.GUID)  ";
            sql += "WHERE PunchlistLookup.GUID = Punchlist.GUID AND PunchlistLookup.DELETED IS NULL ";
            sql += "AND pStatusLookup.DELETED IS NULL GROUP BY PunchlistLookup.GUID)  ";
            sql += "LatestPunchlist ";
            sql += "WHERE Sch.PROJECTGUID = '" + projectGuid.ToString() + "' ";
            sql += "AND (pStatus.SEQUENCE_NUMBER = LatestPunchlist.pStatusLatestSeq OR pStatus.SEQUENCE_NUMBER IS NULL) ";
            sql += "AND Punchlist.DELETED IS NULL AND Tag.DELETED IS NULL  ";
            sql += "AND Sch.DELETED IS NULL AND Wbs.DELETED IS NULL  ";
            sql += "AND iTR.DELETED IS NULL AND Template.DELETED IS NULL  ";
            sql += "AND Workflow.DELETED IS NULL  ";

            if (priorityAOnly)
                sql += "AND Punchlist.PRIORITY = '" + Variables.punchlistCategoryA + "' ";

            sql += "GROUP BY Sch.DISCIPLINE, WBS.NAME, Workflow.NAME, iTR.NAME, NUMBER, Punchlist.DESCRIPTION ORDER BY Sch.DISCIPLINE, WBS.NAME, Workflow.NAME, iTR.NAME, NUMBER, Punchlist.DESCRIPTION ";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    punchlistReport.Add(new ViewModel_PunchlistReport()
                    {
                        Discipline = (string)dr["DISCIPLINE"],
                        WBS_Name = (string)dr["WBSNAME"],
                        Workflow_Name = (string)dr["WFNAME"],
                        Tag_Number = (string)dr["NUMBER"],
                        ITR_Name = (string)dr["ITRNAME"],
                        Description = (string)dr["DESCRIPTION"],
                        Saved_Count = (int)dr["SAVED"],
                        Categorised_Count = (int)dr["CATEGORISED"],
                        Inspected_Count = (int)dr["INSPECTED"],
                        Approved_Count = (int)dr["APPROVED"],
                        Completed_Count = (int)dr["COMPLETED"],
                        Closed_Count = (int)dr["CLOSED"]
                    });
                }
            }

            return punchlistReport;
        }

        public List<ViewModel_PunchlistPriorityReport> GetPunchlistPriorityReport(Guid projectGuid)
        {
            List<ViewModel_PunchlistPriorityReport> punchlistReport = new List<ViewModel_PunchlistPriorityReport>();
            string sql = "SELECT * FROM ";
            sql += "( ";
            sql += "SELECT Punchlist.DISCIPLINE, IsNull(WBS.NAME, 'N/A') AS WBSNAME, IsNull(WBS.DESCRIPTION, 'N/A') AS WBSDESC, IsNull(Workflow.NAME, 'N/A') AS WFNAME, IsNull(iTR.NAME, 'N/A') AS ITRNAME, IsNull(Tag.NUMBER, 'N/A') AS NUMBER,  ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'A' AND (pStatus.STATUS_NUMBER IS NULL OR pStatus.STATUS_NUMBER IS NOT NULL) THEN 1 ELSE 0 END) AS CAT_A_TOTAL,  ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'A' AND pStatus.STATUS_NUMBER = 4 THEN 1 ELSE 0 END) AS CAT_A_CLOSED, ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'B' AND (pStatus.STATUS_NUMBER IS NULL OR pStatus.STATUS_NUMBER IS NOT NULL) THEN 1 ELSE 0 END) AS CAT_B_TOTAL, ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'B' AND pStatus.STATUS_NUMBER = 4 THEN 1 ELSE 0 END) AS CAT_B_CLOSED,  ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'C' AND (pStatus.STATUS_NUMBER IS NULL OR pStatus.STATUS_NUMBER IS NOT NULL) THEN 1 ELSE 0 END) AS CAT_C_TOTAL,  ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'C' AND pStatus.STATUS_NUMBER = 4 THEN 1 ELSE 0 END) AS CAT_C_CLOSED,  ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'D' AND (pStatus.STATUS_NUMBER IS NULL OR pStatus.STATUS_NUMBER IS NOT NULL) THEN 1 ELSE 0 END) AS CAT_D_TOTAL,  ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'D' AND pStatus.STATUS_NUMBER = 4 THEN 1 ELSE 0 END) AS CAT_D_CLOSED FROM PUNCHLIST_MAIN Punchlist  ";
            sql += "LEFT JOIN TAG Tag ON (Tag.GUID = Punchlist.TAG_GUID)  ";
            sql += "LEFT JOIN WBS Wbs ON (Tag.PARENTGUID = Wbs.GUID)  ";
            sql += "JOIN SCHEDULE Sch ON (Sch.GUID = Tag.SCHEDULEGUID)  ";
            sql += "LEFT JOIN ITR_MAIN iTR ON (iTR.GUID = Punchlist.ITR_GUID)  ";
            sql += "LEFT JOIN TEMPLATE_MAIN Template ON (iTR.TEMPLATE_GUID = Template.GUID)  ";
            sql += "LEFT JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID)  ";
            sql += "LEFT JOIN PUNCHLIST_STATUS pStatus ON (pStatus.PUNCHLIST_MAIN_GUID = Punchlist.GUID)  ";
            sql += "OUTER APPLY  ";
            sql += "( ";
            sql += "SELECT MAX(pStatusLookup.SEQUENCE_NUMBER) AS pStatusLatestSeq FROM PUNCHLIST_MAIN PunchlistLookup  ";
            sql += "LEFT JOIN PUNCHLIST_STATUS pStatusLookup ON (pStatusLookup.PUNCHLIST_MAIN_GUID = PunchlistLookup.GUID ";
            sql += ")  ";
            sql += "WHERE PunchlistLookup.GUID = Punchlist.GUID AND PunchlistLookup.DELETED IS NULL AND pStatusLookup.DELETED IS NULL GROUP BY PunchlistLookup.GUID)  LatestPunchlist  ";
            sql += "WHERE Sch.PROJECTGUID = '" + projectGuid.ToString() + "' ";
            sql += "AND (pStatus.SEQUENCE_NUMBER = LatestPunchlist.pStatusLatestSeq OR pStatus.SEQUENCE_NUMBER IS NULL)  ";
            sql += "AND Punchlist.DELETED IS NULL AND Tag.DELETED IS NULL  AND Sch.DELETED IS NULL AND Wbs.DELETED IS NULL  AND iTR.DELETED IS NULL AND Template.DELETED IS NULL  AND Workflow.DELETED IS NULL   ";
            sql += "GROUP BY Punchlist.DISCIPLINE, WBS.NAME, WBS.DESCRIPTION, Workflow.NAME, iTR.NAME, NUMBER ";
            sql += ") AS TagPunchlist ";
            sql += "UNION ALL ";
            sql += "( ";
            sql += "SELECT Punchlist.DISCIPLINE, IsNull(WBS.NAME, 'N/A') AS WBSNAME, IsNull(WBS.DESCRIPTION, 'N/A') AS WBSDESC, IsNull(Workflow.NAME, 'N/A') AS WFNAME, IsNull(iTR.NAME, 'N/A') AS ITRNAME, 'N/A' AS NUMBER,  ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'A' AND (pStatus.STATUS_NUMBER IS NULL OR pStatus.STATUS_NUMBER IS NOT NULL) THEN 1 ELSE 0 END) AS CAT_A_TOTAL,  ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'A' AND pStatus.STATUS_NUMBER = 4 THEN 1 ELSE 0 END) AS CAT_A_CLOSED, ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'B' AND (pStatus.STATUS_NUMBER IS NULL OR pStatus.STATUS_NUMBER IS NOT NULL) THEN 1 ELSE 0 END) AS CAT_B_TOTAL, ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'B' AND pStatus.STATUS_NUMBER = 4 THEN 1 ELSE 0 END) AS CAT_B_CLOSED,  ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'C' AND (pStatus.STATUS_NUMBER IS NULL OR pStatus.STATUS_NUMBER IS NOT NULL) THEN 1 ELSE 0 END) AS CAT_C_TOTAL,  ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'C' AND pStatus.STATUS_NUMBER = 4 THEN 1 ELSE 0 END) AS CAT_C_CLOSED,  ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'D' AND (pStatus.STATUS_NUMBER IS NULL OR pStatus.STATUS_NUMBER IS NOT NULL) THEN 1 ELSE 0 END) AS CAT_D_TOTAL,  ";
            sql += "SUM(CASE WHEN SUBSTRING(Punchlist.PRIORITY, 1, 1) = 'D' AND pStatus.STATUS_NUMBER = 4 THEN 1 ELSE 0 END) AS CAT_D_CLOSED FROM PUNCHLIST_MAIN Punchlist  ";
            sql += "LEFT JOIN WBS Wbs ON (Punchlist.WBS_GUID = Wbs.GUID)  ";
            sql += "JOIN SCHEDULE Sch ON (Sch.GUID = Wbs.SCHEDULEGUID)  ";
            sql += "LEFT JOIN ITR_MAIN iTR ON (iTR.GUID = Punchlist.ITR_GUID)  ";
            sql += "LEFT JOIN TEMPLATE_MAIN Template ON (iTR.TEMPLATE_GUID = Template.GUID)  ";
            sql += "LEFT JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID)  ";
            sql += "LEFT JOIN PUNCHLIST_STATUS pStatus ON (pStatus.PUNCHLIST_MAIN_GUID = Punchlist.GUID)  ";
            sql += "OUTER APPLY  ";
            sql += "( ";
            sql += "SELECT MAX(pStatusLookup.SEQUENCE_NUMBER) AS pStatusLatestSeq FROM PUNCHLIST_MAIN PunchlistLookup  ";
            sql += "LEFT JOIN PUNCHLIST_STATUS pStatusLookup ON (pStatusLookup.PUNCHLIST_MAIN_GUID = PunchlistLookup.GUID ";
            sql += ")  ";
            sql += "WHERE PunchlistLookup.GUID = Punchlist.GUID AND PunchlistLookup.DELETED IS NULL AND pStatusLookup.DELETED IS NULL GROUP BY PunchlistLookup.GUID)  LatestPunchlist  ";
            sql += "WHERE Sch.PROJECTGUID = '" + projectGuid.ToString() + "' AND (pStatus.SEQUENCE_NUMBER = LatestPunchlist.pStatusLatestSeq OR pStatus.SEQUENCE_NUMBER IS NULL)  ";
            sql += "AND Punchlist.DELETED IS NULL AND Sch.DELETED IS NULL AND Wbs.DELETED IS NULL  AND iTR.DELETED IS NULL AND Template.DELETED IS NULL  AND Workflow.DELETED IS NULL   ";
            sql += "GROUP BY Punchlist.DISCIPLINE, WBS.NAME, WBS.DESCRIPTION, Workflow.NAME, iTR.NAME ";
            sql += ")  ";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    punchlistReport.Add(new ViewModel_PunchlistPriorityReport()
                    {
                        Discipline = (string)dr["DISCIPLINE"],
                        WBS_Name = (string)dr["WBSNAME"],
                        WBS_Description = (string)dr["WBSDESC"],
                        Workflow_Name = (string)dr["WFNAME"],
                        Tag_Number = (string)dr["NUMBER"],
                        ITR_Name = (string)dr["ITRNAME"],
                        A_TOTAL = (int)dr["CAT_A_TOTAL"],
                        A_CLOSED = (int)dr["CAT_A_CLOSED"],
                        B_TOTAL = (int)dr["CAT_B_TOTAL"],
                        B_CLOSED = (int)dr["CAT_B_CLOSED"],
                        C_TOTAL = (int)dr["CAT_C_TOTAL"],
                        C_CLOSED = (int)dr["CAT_C_CLOSED"],
                        D_TOTAL = (int)dr["CAT_D_TOTAL"],
                        D_CLOSED = (int)dr["CAT_D_CLOSED"]
                    });
                }
            }

            return punchlistReport;
        }

        public List<ViewModel_PunchlistReport> GenerateCertificatePunchlistReport(Guid projectGuid)
        {
            List<ViewModel_PunchlistReport> punchlistReport = new List<ViewModel_PunchlistReport>();
            string sql = "SELECT Sch.DISCIPLINE ";
            sql += ", IsNull(WBS.NAME, 'N/A') AS WBSNAME ";
            sql += ", IsNull(Workflow.NAME, 'N/A') AS WFNAME ";
            sql += ", IsNull(iTR.NAME, 'N/A') AS ITRNAME ";
            sql += ", IsNull(Tag.NUMBER, 'N/A') AS NUMBER ";
            sql += ", Punchlist.DESCRIPTION ";
            sql += ", SUM(CASE WHEN (pStatus.STATUS_NUMBER IS NULL OR pStatus.STATUS_NUMBER = -1 ) THEN 1 ELSE 0 END) AS SAVED ";
            sql += ", SUM(CASE WHEN pStatus.STATUS_NUMBER = 0 THEN 1 ELSE 0 END) AS CATEGORISED ";
            sql += ", SUM(CASE WHEN pStatus.STATUS_NUMBER = 1 THEN 1 ELSE 0 END) AS INSPECTED ";
            sql += ", SUM(CASE WHEN pStatus.STATUS_NUMBER = 2 THEN 1 ELSE 0 END) AS APPROVED ";
            sql += ", SUM(CASE WHEN pStatus.STATUS_NUMBER = 3 THEN 1 ELSE 0 END) AS COMPLETED ";
            sql += ", SUM(CASE WHEN pStatus.STATUS_NUMBER = 4 THEN 1 ELSE 0 END) AS CLOSED  ";
            sql += "FROM PUNCHLIST_MAIN Punchlist  ";
            sql += "JOIN TAG Tag ON (Tag.GUID = Punchlist.TAG_GUID)  ";
            sql += "JOIN SCHEDULE Sch ON (Sch.GUID = Tag.SCHEDULEGUID)  ";
            sql += "LEFT JOIN WBS Wbs ON (Tag.PARENTGUID = Wbs.GUID)  ";
            sql += "LEFT JOIN ITR_MAIN iTR ON (iTR.GUID = Punchlist.ITR_GUID)  ";
            sql += "LEFT JOIN TEMPLATE_MAIN Template ON (iTR.TEMPLATE_GUID = Template.GUID)  ";
            sql += "LEFT JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID)  ";
            sql += "LEFT JOIN PUNCHLIST_STATUS pStatus ON (pStatus.PUNCHLIST_MAIN_GUID = Punchlist.GUID)  ";
            sql += "OUTER APPLY ";
            sql += "(SELECT MAX(pStatusLookup.SEQUENCE_NUMBER) AS pStatusLatestSeq FROM PUNCHLIST_MAIN PunchlistLookup  ";
            sql += "LEFT JOIN PUNCHLIST_STATUS pStatusLookup ON (pStatusLookup.PUNCHLIST_MAIN_GUID = PunchlistLookup.GUID)  ";
            sql += "WHERE PunchlistLookup.GUID = Punchlist.GUID AND PunchlistLookup.DELETED IS NULL ";
            sql += "AND pStatusLookup.DELETED IS NULL GROUP BY PunchlistLookup.GUID)  ";
            sql += "LatestPunchlist ";
            sql += "WHERE Sch.PROJECTGUID = '" + projectGuid.ToString() + "' ";
            sql += "AND (pStatus.SEQUENCE_NUMBER = LatestPunchlist.pStatusLatestSeq OR pStatus.SEQUENCE_NUMBER IS NULL) ";
            sql += "AND Punchlist.DELETED IS NULL AND Tag.DELETED IS NULL  ";
            sql += "AND Sch.DELETED IS NULL AND Wbs.DELETED IS NULL  ";
            sql += "AND iTR.DELETED IS NULL AND Template.DELETED IS NULL  ";
            sql += "AND Workflow.DELETED IS NULL ";
            sql += "AND Punchlist.PRIORITY = '" + Variables.punchlistCategoryA + "' ";
            sql += "GROUP BY Sch.DISCIPLINE, WBS.NAME, Workflow.NAME, iTR.NAME, NUMBER, Punchlist.DESCRIPTION ORDER BY Sch.DISCIPLINE, WBS.NAME, Workflow.NAME, iTR.NAME, NUMBER, Punchlist.DESCRIPTION ";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    punchlistReport.Add(new ViewModel_PunchlistReport()
                    {
                        Discipline = (string)dr["DISCIPLINE"],
                        WBS_Name = (string)dr["WBSNAME"],
                        Workflow_Name = (string)dr["WFNAME"],
                        Tag_Number = (string)dr["NUMBER"],
                        ITR_Name = (string)dr["ITRNAME"],
                        Description = (string)dr["DESCRIPTION"],
                        Saved_Count = (int)dr["SAVED"],
                        Categorised_Count = (int)dr["CATEGORISED"],
                        Inspected_Count = (int)dr["INSPECTED"],
                        Approved_Count = (int)dr["APPROVED"],
                        Completed_Count = (int)dr["COMPLETED"],
                        Closed_Count = (int)dr["CLOSED"]
                    });
                }
            }

            return punchlistReport;
        }

        public List<ViewModel_ProjectPunchlistStatusByDate> GeneratePunchlistChronologyReport(Guid projectGuid)
        {
            List<ViewModel_ProjectPunchlistStatusByDate> punchlistChronoReport = new List<ViewModel_ProjectPunchlistStatusByDate>();
            string sql = "SELECT INDEXED_TABLE.* FROM ";
            sql += "( ";
            sql += "SELECT DISTINCT CONV_TABLE.DISCIPLINE, CONV_TABLE.iCREATED, SUM(CONV_TABLE.iSTATUS) OVER (PARTITION BY CONV_TABLE.DISCIPLINE ORDER BY CONV_TABLE.iCREATED) AS COUNT_SCORE ";
            sql += ", ROW_NUMBER() OVER (PARTITION BY CONV_TABLE.DISCIPLINE, CONV_TABLE.CONVDATE ORDER BY CONV_TABLE.iCREATED DESC) AS ROWID, (TOTAL_TABLE.ASSIGNED * 4) AS TOTAL_SCORE FROM ";
            sql += "( ";
            sql += "SELECT *, CONVERT(VARCHAR(10),SCORE_TABLE.iCREATED,10) AS CONVDATE FROM ";
            sql += "( ";
            sql += "SELECT Sch.DISCIPLINE AS DISCIPLINE, WBS.NAME AS WBSNAME, Workflow.NAME AS WFNAME, iTR.NAME AS ITRNAME, Tag.NUMBER, Punchlist.DESCRIPTION,  ";
            sql += "(CASE WHEN  ";
            sql += "(pIssue.REJECTION = 1 AND pStatus.STATUS_NUMBER > 0) THEN -1  ";
            sql += "WHEN ";
            sql += "(pStatus.STATUS_NUMBER > 0) THEN 1 ";
            sql += "ELSE 0  ";
            sql += "END) AS iSTATUS,  ";
            sql += "(CASE WHEN (pIssue.CREATED IS NULL AND pStatus.CREATED IS NULL) THEN Punchlist.CREATED ";
            sql += "WHEN (pIssue.CREATED IS NULL) THEN pStatus.CREATED ";
            sql += "ELSE pissue.CREATED END) AS iCREATED ";
            sql += "FROM PUNCHLIST_MAIN Punchlist  ";
            sql += "JOIN TAG Tag ON (Tag.GUID = Punchlist.TAG_GUID) ";
            sql += "JOIN SCHEDULE Sch ON (Sch.GUID = Tag.SCHEDULEGUID) ";
            sql += "LEFT JOIN WBS Wbs ON (Tag.WBSGUID = Wbs.GUID) ";
            sql += "LEFT JOIN ITR_MAIN iTR ON (iTR.GUID = Punchlist.ITR_GUID) ";
            sql += "LEFT JOIN TEMPLATE_MAIN Template ON (iTR.TEMPLATE_GUID = Template.GUID) ";
            sql += "LEFT JOIN WORKFLOW_MAIN Workflow ON (Workflow.GUID = Template.WORKFLOWGUID) ";
            sql += "LEFT JOIN PUNCHLIST_STATUS pStatus ON (pStatus.PUNCHLIST_MAIN_GUID = Punchlist.GUID) ";
            sql += "LEFT JOIN PUNCHLIST_STATUS_ISSUE pIssue ON (pIssue.PUNCHLIST_STATUS_GUID = pStatus.GUID) ";
            sql += "WHERE Sch.PROJECTGUID = '" + projectGuid.ToString() + "' AND ";
            sql += "Punchlist.DELETED IS NULL AND Tag.DELETED IS NULL AND ";
            sql += "Sch.DELETED IS NULL AND Wbs.DELETED IS NULL AND ";
            sql += "iTR.DELETED IS NULL AND Template.DELETED IS NULL AND Workflow.DELETED IS NULL ";
            sql += ") SCORE_TABLE ";
            sql += ") CONV_TABLE ";
            sql += "LEFT JOIN ";
            sql += "( ";
            sql += "SELECT PLAN_TABLE.DISCIPLINE, COUNT(PLAN_TABLE.GUID) AS ASSIGNED FROM ";
            sql += "( ";
            sql += "SELECT DISTINCT Sch.DISCIPLINE AS DISCIPLINE, Punchlist.GUID ";
            sql += "FROM PUNCHLIST_MAIN Punchlist  ";
            sql += "LEFT JOIN TAG Tag ON (Tag.GUID = Punchlist.TAG_GUID) ";
            sql += "LEFT JOIN SCHEDULE Sch ON (Sch.GUID = Tag.SCHEDULEGUID) ";
            sql += "WHERE Sch.PROJECTGUID = '" + projectGuid.ToString() + "' AND ";
            sql += "Punchlist.DELETED IS NULL AND Tag.DELETED IS NULL ";
            sql += ") PLAN_TABLE ";
            sql += "GROUP BY PLAN_TABLE.DISCIPLINE ";
            sql += ") TOTAL_TABLE ON (TOTAL_TABLE.DISCIPLINE = CONV_TABLE.DISCIPLINE) ";
            sql += ") INDEXED_TABLE ";
            sql += "WHERE INDEXED_TABLE.ROWID = '1' ";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    punchlistChronoReport.Add(new ViewModel_ProjectPunchlistStatusByDate()
                    {
                        Discipline = dr.Field<string>("DISCIPLINE"),
                        Created = dr.Field<DateTime>("iCREATED"),
                        DisciplineRunning = dr.Field<int>("COUNT_SCORE"),
                        DisciplineTotal = dr.Field<int>("TOTAL_SCORE")
                    });
                }
            }

            return punchlistChronoReport;
        }

        /// <summary>
        /// Get all the punchlist in relation to iTR
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable GetByITR(Guid iTRGuid)
        {
            string sql = "SELECT * FROM PUNCHLIST WHERE ITR_GUID = '" + iTRGuid + "'";
            sql += " AND DELETED IS NULL";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtITR = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtITR);

            if (dtITR.Rows.Count > 0)
                return dtITR;
            else
                return null;
        }


        /// <summary>
        /// Get the sequence of punchlist by tag/wbs
        /// </summary>
        public int GetSequence(Guid tagWBSGuid)
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN WHERE (TAG_GUID = '" + tagWBSGuid + "'";
            sql += " OR WBS_GUID = '" + tagWBSGuid + "')";

            dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = new dsITR_MAIN.ITR_MAINDataTable();
            ExecuteQuery(sql, dtITR_MAIN);

            if (dtITR_MAIN.Rows.Count > 0)
                return (int)dtITR_MAIN.Rows.Count;
            else
                return -1;
        }

        /// <summary>
        /// Get punchlist by WBS or Tag
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable GetByWBSTag(Guid wbsTagGuid)
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN ";
            sql += "WHERE (TAG_GUID = '" + wbsTagGuid + "' ";
            sql += "OR WBS_GUID = '" + wbsTagGuid + "') ";
            sql += "AND DELETED IS NULL ";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_MAIN = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_MAIN);

            if (dtPUNCHLIST_MAIN.Rows.Count > 0)
                return dtPUNCHLIST_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get punchlist by WBS or Tag and ITR
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable GetByWBSTagITR(Guid wbsTagGuid, Guid iTRGuid)
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN ";
            sql += "WHERE (TAG_GUID = '" + wbsTagGuid + "' ";
            sql += "OR WBS_GUID = '" + wbsTagGuid + "') ";
            sql += "AND ITR_GUID = '" + iTRGuid + "' ";
            sql += "AND DELETED IS NULL ";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_MAIN = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_MAIN);

            if (dtPUNCHLIST_MAIN.Rows.Count > 0)
                return dtPUNCHLIST_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get punchlist by WBS or Tag
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable GetByWBSTagDiscipline(Guid wbsTagGuid, string discipline)
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN ";
            sql += "WHERE (TAG_GUID = '" + wbsTagGuid + "' ";
            sql += "OR WBS_GUID = '" + wbsTagGuid + "') ";
            sql += "AND DISCIPLINE = '" + discipline + "' ";
            sql += "AND DELETED IS NULL ";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_MAIN = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_MAIN);

            if (dtPUNCHLIST_MAIN.Rows.Count > 0)
                return dtPUNCHLIST_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get a particular punchlist by punchlist title
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable GetByProject(Guid guidProject)
        {
            string sql = "SELECT * FROM ";
            sql += "(SELECT PUNCHLIST_MAIN.* FROM PUNCHLIST_MAIN ";
            sql += "JOIN TAG ON PUNCHLIST_MAIN.TAG_GUID = TAG.GUID ";
            sql += "JOIN SCHEDULE ON SCHEDULE.GUID = TAG.SCHEDULEGUID ";
            sql += "WHERE SCHEDULE.PROJECTGUID = '" + guidProject + "' AND PUNCHLIST_MAIN.DELETED IS NULL AND TAG.DELETED IS NULL AND SCHEDULE.DELETED IS NULL ";
            sql += "AND ITR_GUID <> '00000000-0000-0000-0000-000000000000') Table1 ";
            sql += "UNION ";
            sql += "(SELECT PUNCHLIST_MAIN.* FROM PUNCHLIST_MAIN ";
            sql += "JOIN ITR_MAIN ON ITR_MAIN.GUID = PUNCHLIST_MAIN.ITR_GUID ";
            sql += "JOIN TAG ON PUNCHLIST_MAIN.TAG_GUID = TAG.GUID ";
            sql += "JOIN SCHEDULE ON SCHEDULE.GUID = TAG.SCHEDULEGUID ";
            sql += "WHERE SCHEDULE.PROJECTGUID = '" + guidProject + "' AND PUNCHLIST_MAIN.DELETED IS NULL AND TAG.DELETED IS NULL AND SCHEDULE.DELETED IS NULL AND ITR_MAIN.DELETED IS NULL)";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_MAIN = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_MAIN);

            return dtPUNCHLIST_MAIN;
        }

        /// <summary>
        /// Get a particular punchlist by punchlist title
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow GetBy(Guid wbsTagGuid, string title)
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN ";
            sql += "WHERE (TAG_GUID = '" + wbsTagGuid + "' ";
            sql += "OR WBS_GUID = '" + wbsTagGuid + "') ";
            sql += "AND TITLE = '" + title + "' AND DELETED IS NULL ";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_MAIN = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_MAIN);

            if (dtPUNCHLIST_MAIN.Rows.Count > 0)
                return dtPUNCHLIST_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Remove a particular punchlist by GUID
        /// </summary>
        public bool RemoveBy(Guid punchlistGuid)
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN WHERE GUID = '" + punchlistGuid + "'";
            sql += " AND DELETED IS NULL";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtProject = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtProject);

            if (dtProject != null)
            {
                dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drProject = dtProject[0];
                drProject.DELETED = DateTime.Now;
                drProject.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drProject);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get deleted punchlist
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow GetAllPunchlist(Guid guid)
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN WHERE GUID = '" + guid + "'";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPunchlist = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPunchlist);

            if (dtPunchlist.Rows.Count > 0)
                return dtPunchlist[0];
            else
                return null;
        }

        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable GetAllTagPunchlist_ByProject(Guid projectGuid)
        {
            string sql = "SELECT punchlist.* FROM PUNCHLIST_MAIN punchlist";
            sql += " JOIN TAG tag ON (tag.GUID = punchlist.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projectGuid + "'";
            sql += " ORDER BY punchlist.TITLE";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_WBSTAG = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_WBSTAG);

            if (dtPUNCHLIST_WBSTAG.Rows.Count > 0)
                return dtPUNCHLIST_WBSTAG;
            else
                return null;
        }

        /// Changes 12-DEC-2014: WBS is unique by project
        /// <summary>
        /// Get by tag number and title
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow GetByTagTitleProject(string tagNumber, string title, Guid projectGuid)
        {
            string sql = "SELECT punchlist.* FROM PUNCHLIST_MAIN punchlist JOIN TAG tag ON (punchlist.TAG_GUID = tag.GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE tag.NUMBER = '" + tagNumber + "' AND punchlist.TITLE = '" + title + "'";
            sql += " AND proj.GUID = '" + projectGuid + "'";
            sql += " AND punchlist.DELETED IS NULL AND tag.DELETED IS NULL";
            sql += " AND sch.DELETED IS NULL AND proj.DELETED IS NULL";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPunchlist = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPunchlist);

            if (dtPunchlist.Rows.Count > 0)
                return dtPunchlist[0];
            else
                return null;
        }

        /// <summary>
        /// Get punchlist by WBS name, title and project
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow GetByWBSNameProject(string wbsName, string title, Guid projectGuid)
        {
            string sql = "SELECT punchlist.* FROM PUNCHLIST_MAIN punchlist JOIN WBS wbs ON (punchlist.WBS_GUID = wbs.GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE wbs.NAME = '" + wbsName + "' AND punchlist.TITLE = '" + title + "'";
            sql += " AND proj.GUID = '" + projectGuid + "'";
            sql += " AND punchlist.DELETED IS NULL AND wbs.DELETED IS NULL";
            sql += " AND sch.DELETED IS NULL AND proj.DELETED IS NULL";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPunchlist = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPunchlist);

            if (dtPunchlist.Rows.Count > 0)
                return dtPunchlist[0];
            else
                return null;
        }

        //Changes 7-May-2015 : Sync punchlist by entire project
        /// <summary>
        /// Get punchlists linked to tag attachment by project and discipline
        /// </summary>
        public List<SyncPunchlist> GenerateTagSyncByProject(Guid projectGuid)
        {
            //Update 15th May 2015 - Sync Deleted Punchlist
            string sql = "SELECT tag.NUMBER, punchlist.TITLE, proj.GUID FROM PUNCHLIST_MAIN punchlist";
            sql += " JOIN TAG tag ON (tag.GUID = punchlist.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projectGuid + "'";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL";
            sql += " ORDER BY punchlist.TITLE";

            //string sql = "SELECT tag.NUMBER, punchlist.TITLE, proj.GUID FROM PUNCHLIST_MAIN punchlist";
            //sql += " JOIN TAG tag ON (tag.GUID = punchlist.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            //sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projectGuid + "'";
            //sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL and tag.DELETED IS NULL";
            //sql += " AND punchlist.DELETED IS NULL ORDER BY punchlist.TITLE";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<SyncPunchlist> TagPunchlists = new List<SyncPunchlist>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TagPunchlists.Add(new SyncPunchlist(false)
                    {
                        attachmentName = dr[0].ToString(),
                        punchlistTitle = dr[1].ToString(),
                        projectGuid = (Guid)dr[2]
                    });
                }
            }

            return TagPunchlists;
        }

        /// <summary>
        /// Get punchlists linked to tag attachment by project and discipline
        /// </summary>
        public List<SyncPunchlist> GenerateTagSyncByProjectDiscipline(Guid projectGuid, string discipline)
        {
            string sql = "SELECT tag.NUMBER, punchlist.TITLE, proj.GUID FROM PUNCHLIST_MAIN punchlist";
            sql += " JOIN TAG tag ON (tag.GUID = punchlist.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projectGuid + "'";
            sql += " AND sch.DISCIPLINE = '" + discipline + "' AND proj.DELETED IS NULL AND sch.DELETED IS NULL and tag.DELETED IS NULL";
            sql += " AND punchlist.DELETED IS NULL ORDER BY punchlist.TITLE";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<SyncPunchlist> TagPunchlists = new List<SyncPunchlist>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TagPunchlists.Add(new SyncPunchlist(false)
                    {
                        attachmentName = dr[0].ToString(),
                        punchlistTitle = dr[1].ToString(),
                        projectGuid = (Guid)dr[2]
                    });
                }
            }

            return TagPunchlists;
        }

        //Changes 7-May-2015 : Sync punchlist by entire project
        /// <summary>
        /// Get punchlists linked to tag attachment by project and discipline
        /// </summary>
        public List<SyncPunchlist> GenerateWBSSyncByProject(Guid projectGuid)
        {
            string sql = "SELECT wbs.NAME, punchlist.TITLE, proj.GUID FROM PUNCHLIST_MAIN punchlist";
            sql += " JOIN WBS wbs ON (wbs.GUID = punchlist.WBS_GUID) JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projectGuid + "'";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL and wbs.DELETED IS NULL";
            sql += " AND punchlist.DELETED IS NULL ORDER BY punchlist.TITLE";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<SyncPunchlist> TagPunchlists = new List<SyncPunchlist>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TagPunchlists.Add(new SyncPunchlist(true)
                    {
                        attachmentName = dr[0].ToString(),
                        punchlistTitle = dr[1].ToString(),
                        projectGuid = (Guid)dr[2]
                    });
                }
            }

            return TagPunchlists;
        }

        /// <summary>
        /// Get punchlists linked to tag attachment by project and discipline
        /// </summary>
        public List<SyncPunchlist> GenerateWBSSyncByProjectDiscipline(Guid projectGuid, string discipline)
        {
            string sql = "SELECT wbs.NAME, punchlist.TITLE, proj.GUID FROM PUNCHLIST_MAIN punchlist";
            sql += " JOIN WBS wbs ON (wbs.GUID = punchlist.WBS_GUID) JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projectGuid + "'";
            sql += " AND sch.DISCIPLINE = '" + discipline + "' AND proj.DELETED IS NULL AND sch.DELETED IS NULL and wbs.DELETED IS NULL";
            sql += " AND punchlist.DELETED IS NULL ORDER BY punchlist.TITLE";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<SyncPunchlist> TagPunchlists = new List<SyncPunchlist>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TagPunchlists.Add(new SyncPunchlist(true)
                    {
                        attachmentName = dr[0].ToString(),
                        punchlistTitle = dr[1].ToString(),
                        projectGuid = (Guid)dr[2]
                    });
                }
            }

            return TagPunchlists;
        }

        /// <summary>
        /// Get by Tag Attachment to Project and Discipline
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable GetAllTagByProjectDiscipline(Guid projGuid, string discipline)
        {
            //string sql = "SELECT tag.GUID AS WBSTAGGUID, punchlist.* FROM PUNCHLIST_MAIN punchlist";
            //sql += " JOIN TAG tag ON (tag.GUID = punchlist.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            //sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";
            //sql += " AND sch.DISCIPLINE = '" + discipline + "' ORDER BY punchlist.TITLE";

            string sql = "SELECT tag.GUID AS WBSTAGGUID, punchlist.* FROM PUNCHLIST_MAIN punchlist";
            sql += " JOIN TAG tag ON (tag.GUID = punchlist.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " LEFT JOIN ITR_MAIN iTR ON (iTR.GUID = punchlist.ITR_GUID)";
            sql += " WHERE proj.GUID = '" + projGuid + "' AND sch.DISCIPLINE = '" + discipline + "'";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL and tag.DELETED IS NULL";
            sql += " AND punchlist.DELETED IS NULL";
            sql += " AND (iTR.GUID IS NULL OR iTR.DELETED IS NULL)";
            sql += " ORDER BY punchlist.TITLE";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable dtPUNCHLIST_WBSTAG = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_WBSTAG);

            if (dtPUNCHLIST_WBSTAG.Rows.Count > 0)
                return dtPUNCHLIST_WBSTAG;
            else
                return null;
        }

        /// <summary>
        /// Get by Tag Attachment to Project and Discipline
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable GetAllTagAndSystemByProjectDiscipline(Guid projGuid, List<string> disciplines = null, List<string> subsystemNames = null, List<string> systemNames = null, List<string> areaNames = null, string strDescription = "", List<string> categories = null, ProjectLibrary.SearchMode searchMode = ProjectLibrary.SearchMode.Contains)
        {
            string sql = "SELECT WBSTAGGUID, AttachedWBSName, AttachedWBSGuid, GUID, TAG_GUID, WBS_GUID, ITR_GUID, ITR_PUNCHLIST_ITEM, SEQUENCE_NUMBER, TITLE, DESCRIPTION, REMEDIAL, DISCIPLINE, CATEGORY, ACTIONBY, PRIORITY, CREATED, CREATEDBY, UPDATED, UPDATEDBY, DELETED, DELETEDBY ";
            sql += "FROM ";
            sql += "( ";
            sql += "SELECT tag.GUID AS WBSTAGGUID, AttachedWBSName = Parent3WBS.NAME, AttachedWBSGuid = Parent3WBS.GUID, ";
            sql += "Area = CASE WHEN Parent1WBS.NAME IS NULL AND Parent2WBS.NAME IS NULL THEN Parent3WBS.NAME ELSE CASE WHEN Parent1WBS.NAME IS NULL THEN Parent2WBS.NAME ELSE Parent1WBS.NAME END END, ";
            sql += "[System] = CASE WHEN Parent1WBS.NAME IS NULL AND Parent2WBS.NAME IS NULL THEN NULL ELSE CASE WHEN Parent1WBS.NAME IS NULL THEN Parent3WBS.NAME ELSE Parent2WBS.NAME END END, ";
            sql += "Subsystem = CASE WHEN Parent1WBS.NAME IS NULL AND Parent2WBS.NAME IS NULL THEN NULL ELSE CASE WHEN Parent1WBS.NAME IS NULL THEN NULL ELSE Parent3WBS.NAME END END, ";
            sql += "punchlist.* FROM PUNCHLIST_MAIN punchlist ";
            sql += "JOIN TAG tag ON (tag.GUID = punchlist.TAG_GUID) ";
            sql += "JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) ";
            sql += "JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) ";
            sql += "LEFT JOIN ITR_MAIN iTR ON (iTR.GUID = punchlist.ITR_GUID) ";
            sql += "JOIN WBS Parent3WBS ON Parent3WBS.GUID = tag.PARENTGUID ";
            sql += "LEFT JOIN WBS Parent2WBS ON Parent2WBS.GUID = Parent3WBS.PARENTGUID ";
            sql += "LEFT JOIN WBS Parent1WBS ON Parent1WBS.GUID = Parent2WBS.PARENTGUID ";
            sql += "WHERE proj.GUID = '" + projGuid + "' ";

            if (disciplines != null)
            {
                string disciplineQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(disciplines);
                sql += "AND punchlist.DISCIPLINE IN (" + disciplineQueryConcatenation + ") ";
            }

            if (categories != null)
            {
                string categoriesQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(categories);
                sql += "AND punchlist.PRIORITY IN (" + categoriesQueryConcatenation + ") ";
            }

            sql += "AND proj.DELETED IS NULL AND sch.DELETED IS NULL and tag.DELETED IS NULL AND punchlist.DELETED IS NULL AND (iTR.GUID IS NULL OR iTR.DELETED IS NULL) ";
            sql += ") TblQuery WHERE DELETED IS NULL ";

            if (subsystemNames != null)
            {
                string subSystemQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(subsystemNames);
                sql += "AND Subsystem IN (" + subSystemQueryConcatenation + ") ";
            }
            else if (systemNames != null)
            {
                string systemQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(systemNames);
                sql += "AND [System] IN (" + systemQueryConcatenation + ") ";
            }
            else if (areaNames != null)
            {
                string areaQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(areaNames);
                sql += "AND Area IN (" + areaQueryConcatenation + ") ";
            }

            if (strDescription != null && strDescription != string.Empty)
            {
                if (searchMode == SearchMode.Starts_With)
                    sql += "AND DESCRIPTION LIKE '" + strDescription + "%' ";
                else if (searchMode == SearchMode.Ends_With)
                    sql += "AND DESCRIPTION LIKE '%" + strDescription + "' ";
                else
                    sql += "AND DESCRIPTION LIKE '%" + strDescription + "%' ";
            }

            sql += "ORDER BY TITLE";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable dtPUNCHLIST_WBSTAG = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_WBSTAG);

            if (dtPUNCHLIST_WBSTAG.Rows.Count > 0)
                return dtPUNCHLIST_WBSTAG;
            else
                return null;
        }

        /// <summary>
        /// Get by WBS Attachment to Project and Discipline
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable GetAllWBSSystemByProjectDiscipline(Guid projGuid, List<string> disciplines = null, List<string> subsystemNames = null, List<string> systemNames = null, List<string> areaNames = null, string strDescription = "", List<string> categories = null, ProjectLibrary.SearchMode searchMode = ProjectLibrary.SearchMode.Contains)
        {
            string sql = "SELECT WBSTAGGUID, AttachedWBSName, AttachedWBSGuid, GUID, TAG_GUID, WBS_GUID, ITR_GUID, ITR_PUNCHLIST_ITEM, SEQUENCE_NUMBER, TITLE, DESCRIPTION, REMEDIAL, DISCIPLINE, CATEGORY, ACTIONBY, PRIORITY, CREATED, CREATEDBY, UPDATED, UPDATEDBY, DELETED, DELETEDBY ";
            sql += "FROM ";
            sql += "( ";
            sql += "SELECT Parent3WBS.GUID AS WBSTAGGUID, AttachedWBSName = Parent3WBS.NAME, AttachedWBSGuid = Parent3WBS.GUID, ";
            sql += "Area = CASE WHEN Parent1WBS.NAME IS NULL AND Parent2WBS.NAME IS NULL THEN Parent3WBS.NAME ELSE CASE WHEN Parent1WBS.NAME IS NULL THEN Parent2WBS.NAME ELSE Parent1WBS.NAME END END, ";
            sql += "[System] = CASE WHEN Parent1WBS.NAME IS NULL AND Parent2WBS.NAME IS NULL THEN NULL ELSE CASE WHEN Parent1WBS.NAME IS NULL THEN Parent3WBS.NAME ELSE Parent2WBS.NAME END END, ";
            sql += "Subsystem = CASE WHEN Parent1WBS.NAME IS NULL AND Parent2WBS.NAME IS NULL THEN NULL ELSE CASE WHEN Parent1WBS.NAME IS NULL THEN NULL ELSE Parent3WBS.NAME END END, ";
            sql += "punchlist.* FROM PUNCHLIST_MAIN punchlist ";
            sql += "JOIN WBS Parent3WBS ON Parent3WBS.GUID = punchlist.WBS_GUID ";
            sql += "JOIN SCHEDULE sch ON (sch.GUID = Parent3WBS.SCHEDULEGUID) ";
            sql += "LEFT JOIN WBS Parent2WBS ON Parent2WBS.GUID = Parent3WBS.PARENTGUID ";
            sql += "LEFT JOIN WBS Parent1WBS ON Parent1WBS.GUID = Parent2WBS.PARENTGUID ";
            sql += "JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID) ";
            sql += "WHERE proj.GUID = '" + projGuid + "' ";

            if (disciplines != null)
            {
                string disciplineQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(disciplines);
                sql += "AND punchlist.DISCIPLINE IN (" + disciplineQueryConcatenation + ") ";
            }

            if (categories != null)
            {
                string categoriesQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(categories);
                sql += "AND punchlist.PRIORITY IN (" + categoriesQueryConcatenation + ") ";
            }

            sql += "AND proj.DELETED IS NULL AND sch.DELETED IS NULL AND Parent3WBS.DELETED IS NULL AND Parent2WBS.DELETED IS NULL AND Parent1WBS.DELETED IS NULL AND punchlist.DELETED IS NULL ";
            sql += ") TblQuery WHERE DELETED IS NULL ";

            if (subsystemNames != null)
            {
                string subSystemQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(subsystemNames);
                sql += "AND Subsystem IN (" + subSystemQueryConcatenation + ") ";
            }
            else if (systemNames != null)
            {
                string systemQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(systemNames);
                sql += "AND [System] IN (" + systemQueryConcatenation + ") ";
            }
            else if (areaNames != null)
            {
                string areaQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(areaNames);
                sql += "AND Area IN (" + areaQueryConcatenation + ") ";
            }

            if (strDescription != null && strDescription != string.Empty)
            {
                if (searchMode == SearchMode.Starts_With)
                    sql += "AND DESCRIPTION LIKE '" + strDescription + "%' ";
                else if (searchMode == SearchMode.Ends_With)
                    sql += "AND DESCRIPTION LIKE '%" + strDescription + "' ";
                else
                    sql += "AND DESCRIPTION LIKE '%" + strDescription + "%' ";
            }

            sql += "ORDER BY TITLE";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable dtPUNCHLIST_WBSTAG = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_WBSTAG);

            if (dtPUNCHLIST_WBSTAG.Rows.Count > 0)
                return dtPUNCHLIST_WBSTAG;
            else
                return null;
        }

        /// <summary>
        /// Get by Tag Attachment to Project and Discipline
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable GetAllTagSystemByProject(Guid projGuid)
        {
            //string sql = "SELECT tag.GUID AS WBSTAGGUID, punchlist.* FROM PUNCHLIST_MAIN punchlist";
            //sql += " JOIN TAG tag ON (tag.GUID = punchlist.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            //sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";
            //sql += " AND sch.DISCIPLINE = '" + discipline + "' ORDER BY punchlist.TITLE";

            string sql = "SELECT tag.GUID AS WBSTAGGUID, AttachedWBSName = wbs.NAME, AttachedWBSGuid = wbs.GUID, punchlist.* FROM PUNCHLIST_MAIN punchlist";
            sql += " JOIN TAG tag ON (tag.GUID = punchlist.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " LEFT JOIN ITR_MAIN iTR ON (iTR.GUID = punchlist.ITR_GUID)";
            sql += " LEFT JOIN WBS wbs ON wbs.GUID = tag.PARENTGUID";
            sql += " WHERE proj.GUID = '" + projGuid + "'";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL and tag.DELETED IS NULL";
            sql += " AND punchlist.DELETED IS NULL";
            sql += " AND (iTR.GUID IS NULL OR iTR.DELETED IS NULL)";
            sql += " ORDER BY punchlist.TITLE";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable dtPUNCHLIST_WBSTAG = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_WBSTAG);

            if (dtPUNCHLIST_WBSTAG.Rows.Count > 0)
                return dtPUNCHLIST_WBSTAG;
            else
                return null;
        }

        /// <summary>
        /// Get by Tag Attachment to Project and Discipline
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable GetAllTagByProject(Guid projGuid)
        {
            //string sql = "SELECT tag.GUID AS WBSTAGGUID, punchlist.* FROM PUNCHLIST_MAIN punchlist";
            //sql += " JOIN TAG tag ON (tag.GUID = punchlist.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            //sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";
            //sql += " AND sch.DISCIPLINE = '" + discipline + "' ORDER BY punchlist.TITLE";

            string sql = "SELECT tag.GUID AS WBSTAGGUID, punchlist.* FROM PUNCHLIST_MAIN punchlist";
            sql += " JOIN TAG tag ON (tag.GUID = punchlist.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " LEFT JOIN ITR_MAIN iTR ON (iTR.GUID = punchlist.ITR_GUID)";
            sql += " WHERE proj.GUID = '" + projGuid + "'";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL and tag.DELETED IS NULL";
            sql += " AND punchlist.DELETED IS NULL";
            sql += " AND (iTR.GUID IS NULL OR iTR.DELETED IS NULL)";
            sql += " ORDER BY punchlist.TITLE";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable dtPUNCHLIST_WBSTAG = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_WBSTAG);

            if (dtPUNCHLIST_WBSTAG.Rows.Count > 0)
                return dtPUNCHLIST_WBSTAG;
            else
                return null;
        }

        /// <summary>
        /// Get by WBS Attachment to Project and Discipline
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable GetAllWBSByProjectDiscipline(Guid projGuid, string discipline)
        {
            string sql = "SELECT wbs.GUID AS WBSTAGGUID, punchlist.* FROM PUNCHLIST_MAIN punchlist";
            sql += " JOIN WBS wbs ON (wbs.GUID = punchlist.WBS_GUID) JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";
            sql += " AND sch.DISCIPLINE = '" + discipline + "' AND proj.DELETED IS NULL AND sch.DELETED IS NULL and wbs.DELETED IS NULL";
            sql += " AND punchlist.DELETED IS NULL ORDER BY punchlist.TITLE";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable dtPUNCHLIST_WBSTAG = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_WBSTAG);

            if (dtPUNCHLIST_WBSTAG.Rows.Count > 0)
                return dtPUNCHLIST_WBSTAG;
            else
                return null;
        }

        /// <summary>
        /// Get by WBS Attachment to Project and Discipline
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable GetAllWBSSystemByProject(Guid projGuid)
        {
            string sql = "SELECT wbs.GUID AS WBSTAGGUID, AttachedWBSName = wbs.NAME, AttachedWBSGuid = wbs.GUID, punchlist.* FROM PUNCHLIST_MAIN punchlist";
            sql += " JOIN WBS wbs ON (wbs.GUID = punchlist.WBS_GUID) JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL and wbs.DELETED IS NULL";
            sql += " AND punchlist.DELETED IS NULL ORDER BY punchlist.TITLE";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable dtPUNCHLIST_WBSTAG = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_WBSTAG);

            if (dtPUNCHLIST_WBSTAG.Rows.Count > 0)
                return dtPUNCHLIST_WBSTAG;
            else
                return null;
        }

        /// <summary>
        /// Get by WBS Attachment to Project and Discipline
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable GetAllWBSByProject(Guid projGuid)
        {
            string sql = "SELECT wbs.GUID AS WBSTAGGUID, punchlist.* FROM PUNCHLIST_MAIN punchlist";
            sql += " JOIN WBS wbs ON (wbs.GUID = punchlist.WBS_GUID) JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL and wbs.DELETED IS NULL";
            sql += " AND punchlist.DELETED IS NULL ORDER BY punchlist.TITLE";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable dtPUNCHLIST_WBSTAG = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_WBSTAG);

            if (dtPUNCHLIST_WBSTAG.Rows.Count > 0)
                return dtPUNCHLIST_WBSTAG;
            else
                return null;
        }

        /// <summary>
        /// Gets the number of priority punchlist within status criteria
        /// </summary>
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable CheckUnapprovedPriorityPunchlist(Guid iTRGUID, string priority, int status)
        {
            if (iTRGUID == Guid.Empty)
                return null;

            string sql = "SELECT punchlist.* FROM PUNCHLIST_MAIN punchlist OUTER APPLY ";
            sql += "(SELECT TOP 1 * FROM PUNCHLIST_STATUS ";
            sql += "WHERE DELETED IS NULL AND PUNCHLIST_MAIN_GUID = punchlist.GUID ORDER BY SEQUENCE_NUMBER DESC) ";
            sql += "pStatusFilter ";
            sql += "WHERE (pStatusFilter.STATUS_NUMBER < " + status + " OR pStatusFilter.STATUS_NUMBER IS NULL) ";
            sql += "AND punchlist.ITR_GUID = '" + iTRGUID + "' ";
            sql += "AND punchlist.PRIORITY = '" + priority + "'";
            sql += "AND punchlist.DELETED IS NULL";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPUNCHLIST);

            if (dtPUNCHLIST.Rows.Count > 0)
                return dtPUNCHLIST;
            else
                return null;
        }

        /// <summary>
        /// Get all punchlists
        /// </summary
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable Get()
        {
            string sql = "SELECT * FROM PUNCHLIST_MAIN WHERE DELETED IS NULL";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_MAIN = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_MAIN);

            if (dtPUNCHLIST_MAIN.Rows.Count > 0)
                return dtPUNCHLIST_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get deleted punchlists for purging
        /// </summary
        public dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable Get_Deleted(Guid projectGuid)
        {
            string sql = "SELECT punchlist.* FROM PUNCHLIST_MAIN punchlist";
            sql += " JOIN TAG tag ON (tag.GUID = punchlist.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE proj.GUID = '" + projectGuid.ToString() + "'";
            sql += " AND punchlist.DELETED IS NOT NULL";

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_MAIN = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_MAIN);

            if (dtPUNCHLIST_MAIN.Rows.Count > 0)
                return dtPUNCHLIST_MAIN;
            else
                return null;
        }
        /// <summary>
        /// Saves multiple punchlists
        /// </summary>
        public void Save(dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST_MAIN)
        {
            _adapter.Update(dtPUNCHLIST_MAIN);
        }

        /// <summary>
        /// Saves one punchlist
        /// </summary>
        public void Save(dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPUNCHLIST_MAIN)
        {
            string s = string.Empty;
            try
            {
                int result = _adapter.Update(drPUNCHLIST_MAIN);
            }
            catch(Exception e)
            {
                s = e.ToString();
            }
            
        }

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
