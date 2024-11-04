using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsTEMPLATE_MAINTableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterTEMPLATE_MAIN : SQLBase, IDisposable
    {
        private TEMPLATE_MAINTableAdapter _adapter;

        public AdapterTEMPLATE_MAIN()
            : base()
        {
            _adapter = new TEMPLATE_MAINTableAdapter(Variables.ConnStr);
        }

        public AdapterTEMPLATE_MAIN(string connStr)
            : base(connStr)
        {
            _adapter = new TEMPLATE_MAINTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all templates
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable Get()
        {
            string sql = "SELECT * FROM TEMPLATE_MAIN WHERE DELETED IS NULL ORDER BY NAME";
            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            ExecuteQuery(sql, dtTEMPLATE_MAIN);

            if (dtTEMPLATE_MAIN.Rows.Count > 0)
                return dtTEMPLATE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get all templates metadata only without the binary data
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable GetMetaOnly()
        {
            string sql = "SELECT GUID, WORKFLOWGUID, NAME, REVISION, DISCIPLINE, DESCRIPTION, TEMPLATE = CAST(0x as varbinary(MAX)), QRSUPPORT, SKIPAPPROVED, CREATED, CREATEDBY, UPDATED, UPDATEDBY, DELETED, DELETEDBY FROM TEMPLATE_MAIN WHERE DELETED IS NULL ORDER BY NAME";
            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            ExecuteQuery(sql, dtTEMPLATE_MAIN);

            if (dtTEMPLATE_MAIN.Rows.Count > 0)
                return dtTEMPLATE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get deleted template in the system for purging
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable Get_Deleted()
        {
            string sql = "SELECT * FROM TEMPLATE_MAIN WHERE DELETED IS NOT NULL";
            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            ExecuteQuery(sql, dtTEMPLATE_MAIN);

            if (dtTEMPLATE_MAIN.Rows.Count > 0)
                return dtTEMPLATE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get all template in the system include deleted
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable GetIncludeDeleted()
        {
            string sql = "SELECT * FROM TEMPLATE_MAIN";
            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            ExecuteQuery(sql, dtTEMPLATE_MAIN);

            if (dtTEMPLATE_MAIN.Rows.Count > 0)
                return dtTEMPLATE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get template by discipline
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable GetByDiscipline(string discipline)
        {
            string sql = "SELECT * FROM TEMPLATE_MAIN WHERE DISCIPLINE = '" + discipline + "'";
            sql += " AND DELETED IS NULL";
            sql += " ORDER BY NAME";

            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            ExecuteQuery(sql, dtTEMPLATE_MAIN);

            if (dtTEMPLATE_MAIN.Rows.Count > 0)
                return dtTEMPLATE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get template by guid
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAINRow GetBy(Guid templateGuid)
        {
            string sql = "SELECT * FROM TEMPLATE_MAIN WHERE GUID = '" + templateGuid + "'";
            sql += " AND DELETED IS NULL";

            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            ExecuteQuery(sql, dtTEMPLATE_MAIN);

            if (dtTEMPLATE_MAIN.Rows.Count > 0)
                return dtTEMPLATE_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get template by guid include deletes
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAINRow GetIncludeDeletedBy(Guid templateGuid)
        {
            string sql = "SELECT * FROM TEMPLATE_MAIN WHERE GUID = '" + templateGuid + "'";

            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            ExecuteQuery(sql, dtTEMPLATE_MAIN);

            if (dtTEMPLATE_MAIN.Rows.Count > 0)
                return dtTEMPLATE_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get template by name
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAINRow GetBy(string name)
        {
            string sql = "SELECT * FROM TEMPLATE_MAIN WHERE NAME = '" + name + "'";
            sql += " AND DELETED IS NULL";

            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            ExecuteQuery(sql, dtTEMPLATE_MAIN);

            if (dtTEMPLATE_MAIN.Rows.Count > 0)
                return dtTEMPLATE_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get template by workflow
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable GetByWorkflow(Guid workflowGuid)
        {
            string sql = "SELECT * FROM TEMPLATE_MAIN WHERE WORKFLOWGUID = '" + workflowGuid + "'";
            sql += " AND DELETED IS NULL";

            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            ExecuteQuery(sql, dtTEMPLATE_MAIN);

            if (dtTEMPLATE_MAIN.Rows.Count > 0)
                return dtTEMPLATE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get by Tag Attachment to Project and Discipline
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable GetByTagProject(Guid projGuid)
        {
            string sql = "SELECT " +
            "tag.GUID AS WBSTAGGUID, TEMPLATE.GUID, TEMPLATE.WORKFLOWGUID, TEMPLATE.NAME, TEMPLATE.REVISION, TEMPLATE.DISCIPLINE, TEMPLATE.DESCRIPTION, TEMPLATE = CAST(0x as varbinary(MAX)), " +
            "TEMPLATE.QRSUPPORT, TEMPLATE.SKIPAPPROVED, TEMPLATE.CREATED, TEMPLATE.CREATEDBY, TEMPLATE.UPDATED, TEMPLATE.UPDATEDBY, TEMPLATE.DELETED, TEMPLATE.DELETEDBY " +
            "FROM TEMPLATE_MAIN template JOIN TEMPLATE_REGISTER reg ON (reg.TEMPLATE_GUID = template.GUID)";
            sql += " JOIN TAG tag ON (tag.GUID = reg.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL and tag.DELETED IS NULL";
            sql += " AND reg.DELETED IS NULL AND template.DELETED IS NULL";

            dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable dtTEMPLATE_WBSTAG = new dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable();
            ExecuteQuery(sql, dtTEMPLATE_WBSTAG);

            if (dtTEMPLATE_WBSTAG.Rows.Count > 0)
                return dtTEMPLATE_WBSTAG;
            else
                return null;
        }

        /// <summary>
        /// Get by Tag Attachment to Project and Discipline and Number
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable GetByTagProjectDisciplineNumber(Guid projGuid, string discipline = "", string number = "")
        {
            string sql = "SELECT " +
            "tag.GUID AS WBSTAGGUID, TEMPLATE.GUID, TEMPLATE.WORKFLOWGUID, TEMPLATE.NAME, TEMPLATE.REVISION, TEMPLATE.DISCIPLINE, TEMPLATE.DESCRIPTION, TEMPLATE = CAST(0x as varbinary(MAX)), " +
            "TEMPLATE.QRSUPPORT, TEMPLATE.SKIPAPPROVED, TEMPLATE.CREATED, TEMPLATE.CREATEDBY, TEMPLATE.UPDATED, TEMPLATE.UPDATEDBY, TEMPLATE.DELETED, TEMPLATE.DELETEDBY " +
            "FROM TEMPLATE_MAIN template JOIN TEMPLATE_REGISTER reg ON (reg.TEMPLATE_GUID = template.GUID)";
            sql += " JOIN TAG tag ON (tag.GUID = reg.TAG_GUID) JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";
            if (discipline != string.Empty)
                sql += " AND sch.DISCIPLINE = '" + discipline + "'";
            sql += " AND tag.NUMBER LIKE '%" + number + "%'";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL and tag.DELETED IS NULL";
            sql += " AND reg.DELETED IS NULL AND template.DELETED IS NULL";

            dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable dtTEMPLATE_WBSTAG = new dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable();
            ExecuteQuery(sql, dtTEMPLATE_WBSTAG);

            if (dtTEMPLATE_WBSTAG.Rows.Count > 0)
                return dtTEMPLATE_WBSTAG;
            else
                return null;
        }

        /// <summary>
        /// Get by WBS Attachment to Project and Discipline
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable GetByWBSProject(Guid projGuid, string discipline = "")
        {
            string sql = "SELECT " +
            "wbs.GUID AS WBSTAGGUID, TEMPLATE.GUID, TEMPLATE.WORKFLOWGUID, TEMPLATE.NAME, TEMPLATE.REVISION, TEMPLATE.DISCIPLINE, TEMPLATE.DESCRIPTION, TEMPLATE = CAST(0x as varbinary(MAX)), " +
            "TEMPLATE.QRSUPPORT, TEMPLATE.SKIPAPPROVED, TEMPLATE.CREATED, TEMPLATE.CREATEDBY, TEMPLATE.UPDATED, TEMPLATE.UPDATEDBY, TEMPLATE.DELETED, TEMPLATE.DELETEDBY " +
            "FROM TEMPLATE_MAIN template JOIN TEMPLATE_REGISTER reg ON (reg.TEMPLATE_GUID = template.GUID)";
            sql += " JOIN WBS wbs ON (wbs.GUID = reg.WBS_GUID) JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";
            if(discipline != string.Empty)
                sql += " AND sch.DISCIPLINE = '" + discipline + "'";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL and wbs.DELETED IS NULL";
            sql += " AND reg.DELETED IS NULL AND template.DELETED IS NULL";

            dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable dtTEMPLATE_WBSTAG = new dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable();
            ExecuteQuery(sql, dtTEMPLATE_WBSTAG);

            if (dtTEMPLATE_WBSTAG.Rows.Count > 0)
                return dtTEMPLATE_WBSTAG;
            else
                return null;
        }

        /// <summary>
        /// Get by WBS Attachment to Project and Discipline
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable GetByWBSProjectDisciplineName(Guid projGuid, string discipline = "", string searchName = "")
        {
            string sql = "SELECT " +
            "wbs.GUID AS WBSTAGGUID, TEMPLATE.GUID, TEMPLATE.WORKFLOWGUID, TEMPLATE.NAME, TEMPLATE.REVISION, TEMPLATE.DISCIPLINE, TEMPLATE.DESCRIPTION, TEMPLATE = CAST(0x as varbinary(MAX)), " +
            "TEMPLATE.QRSUPPORT, TEMPLATE.SKIPAPPROVED, TEMPLATE.CREATED, TEMPLATE.CREATEDBY, TEMPLATE.UPDATED, TEMPLATE.UPDATEDBY, TEMPLATE.DELETED, TEMPLATE.DELETEDBY " +
            "FROM TEMPLATE_MAIN template JOIN TEMPLATE_REGISTER reg ON (reg.TEMPLATE_GUID = template.GUID)";
            sql += " JOIN WBS wbs ON (wbs.GUID = reg.WBS_GUID) JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID) JOIN";
            sql += " PROJECT proj ON (proj.GUID = sch.PROJECTGUID) WHERE proj.GUID = '" + projGuid + "'";
            if (discipline != string.Empty)
                sql += " AND sch.DISCIPLINE = '" + discipline + "'";
            sql += " AND wbs.NAME LIKE '%" + searchName + "'%";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL and wbs.DELETED IS NULL";
            sql += " AND reg.DELETED IS NULL AND template.DELETED IS NULL";

            dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable dtTEMPLATE_WBSTAG = new dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable();
            ExecuteQuery(sql, dtTEMPLATE_WBSTAG);

            if (dtTEMPLATE_WBSTAG.Rows.Count > 0)
                return dtTEMPLATE_WBSTAG;
            else
                return null;
        }

        /// <summary>
        /// Get template by WBS or Tag
        /// </summary>
        public dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable GetByWBSTag(Guid wbsTagGuid)
        {
            string sql = "SELECT Main.* FROM TEMPLATE_MAIN Main JOIN TEMPLATE_REGISTER Register";
            sql += " ON (Main.GUID = Register.TEMPLATE_GUID) WHERE ";
            sql += " (Register.TAG_GUID = '" + wbsTagGuid + "'";
            sql += " OR Register.WBS_Guid = '" + wbsTagGuid + "')";
            sql += " AND Main.DELETED IS NULL";
            sql += " AND Register.DELETED IS NULL";
            sql += " ORDER BY Main.NAME";

            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            ExecuteQuery(sql, dtTEMPLATE_MAIN);

            if (dtTEMPLATE_MAIN.Rows.Count > 0)
                return dtTEMPLATE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get all wbs's incompleted template count
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="discipline"></param>
        /// <returns></returns>
        public List<wbsCount> GetWBSIncompleteTemplateCount(Guid projectGuid)
        {
            string sql = "SELECT SCORE.GUID, SCORE.NAME, TOTAL_COUNT = SCORE.PENDING_COUNT + SCORE.NOTINCL_COUNT, SCORE.INSPECTED_COUNT, SCORE.APPROVED_COUNT, SCORE.COMPLETED_COUNT, SCORE.CLOSED_COUNT FROM ";
            sql += "(";
            sql += "SELECT Wbs1.GUID, Wbs1.NAME, MAX(ITR_TOTAL.PENDING_COUNT) AS PENDING_COUNT, SUM(ITR_NOTINCL.TOTAL_NOTINCL_COUNT) AS NOTINCL_COUNT, SUM(ITR_SAVED.SAVEDCOUNT) AS SAVED_COUNT, SUM(ITR_INSPECTED.INSPECTEDCOUNT) AS INSPECTED_COUNT, SUM(ITR_APPROVED.APPROVEDCOUNT) AS APPROVED_COUNT, SUM(ITR_COMPLETED.COMPLETEDCOUNT) AS COMPLETED_COUNT, SUM(ITR_CLOSED.CLOSEDCOUNT) AS CLOSED_COUNT ";
            sql += "FROM WBS Wbs1 ";
            sql += "JOIN SCHEDULE Sch ON (Sch.GUID = Wbs1.SCHEDULEGUID) ";
            sql += "JOIN PROJECT Proj ON (Proj.GUID = Sch.PROJECTGUID) ";
            sql += "OUTER APPLY ";
            sql += "(SELECT Itr2.GUID, iStatus2.STATUS_NUMBER, iStatus2.SEQUENCE_NUMBER FROM ITR_MAIN Itr2 LEFT JOIN ITR_STATUS iStatus2 ON (iStatus2.ITR_MAIN_GUID = Itr2.GUID) WHERE itr2.TAG_GUID = Wbs1.GUID AND Itr2.SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) FROM ITR_MAIN Itr3 WHERE Itr3.GUID = Itr2.GUID) AND (iStatus2.SEQUENCE_NUMBER = (SELECT MAX(iStatus3.SEQUENCE_NUMBER) FROM ITR_STATUS iStatus3 WHERE iStatus3.ITR_MAIN_GUID = itr2.GUID) OR iStatus2.SEQUENCE_NUMBER IS NULL) AND itr2.TAG_GUID = Wbs1.GUID AND itr2.DELETED IS NULL ) LatestITR ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS PENDING_COUNT FROM TEMPLATE_REGISTER WHERE TAG_GUID = Wbs1.GUID AND DELETED IS NULL) ITR_TOTAL ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS TOTAL_NOTINCL_COUNT ";
            sql += "FROM ITR_MAIN LEFT JOIN ";
            sql += "(SELECT TEMPLATE_REGISTER.* FROM TEMPLATE_REGISTER JOIN TEMPLATE_MAIN ON TEMPLATE_MAIN.GUID = TEMPLATE_REGISTER.TEMPLATE_GUID WHERE TEMPLATE_REGISTER.DELETED IS NULL AND TEMPLATE_MAIN.DELETED IS NULL) Register1 ON Register1.WBS_GUID = ITR_MAIN.WBS_GUID AND Register1.TEMPLATE_GUID = ITR_MAIN.TEMPLATE_GUID ";
            sql += "WHERE Register1.GUID IS NULL AND ITR_MAIN.GUID = LatestITR.GUID ";
            sql += ") ITR_NOTINCL ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS SAVEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NULL OR STATUS_NUMBER >= '-1') ITR_SAVED ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS INSPECTEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '0') ITR_INSPECTED ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS APPROVEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '1') ITR_APPROVED ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS COMPLETEDCOUNT FROM ITR_MAIN WHERE GUID =  LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '2') ITR_COMPLETED ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS CLOSEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '3') ITR_CLOSED ";
            sql += "WHERE Proj.GUID = '" + projectGuid + "' AND Wbs1.DELETED IS NULL AND Sch.DELETED IS NULL AND Proj.DELETED IS NULL GROUP BY Wbs1.GUID, Wbs1.NAME ) SCORE";

            //string sql = "SELECT Wbs1.GUID, Wbs1.NAME, MAX(ITR_TOTAL.TOTAL_COUNT) AS TOTAL_COUNT ";
            //sql += ", SUM(ITR_SAVED.SAVEDCOUNT) AS SAVED_COUNT ";
            //sql += ", SUM(ITR_INSPECTED.INSPECTEDCOUNT) AS INSPECTED_COUNT ";
            //sql += ", SUM(ITR_APPROVED.APPROVEDCOUNT) AS APPROVED_COUNT ";
            //sql += ", SUM(ITR_COMPLETED.COMPLETEDCOUNT) AS COMPLETED_COUNT ";
            //sql += ", SUM(ITR_CLOSED.CLOSEDCOUNT) AS CLOSED_COUNT ";
            //sql += "FROM WBS Wbs1 JOIN SCHEDULE Sch ON (Sch.GUID = Wbs1.SCHEDULEGUID) ";
            //sql += "JOIN PROJECT Proj ON (Proj.GUID = Sch.PROJECTGUID) ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT Itr2.GUID, iStatus2.STATUS_NUMBER, iStatus2.SEQUENCE_NUMBER FROM ITR_MAIN Itr2 LEFT JOIN ITR_STATUS iStatus2 ON (iStatus2.ITR_MAIN_GUID = Itr2.GUID) ";
            //sql += "WHERE ";
            //sql += "itr2.TAG_GUID = Wbs1.GUID ";
            //sql += "AND ";
            //sql += "Itr2.SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) FROM ITR_MAIN Itr3 WHERE Itr3.GUID = Itr2.GUID) ";
            //sql += "AND ";
            //sql += "(iStatus2.SEQUENCE_NUMBER = (SELECT MAX(iStatus3.SEQUENCE_NUMBER) FROM ITR_STATUS iStatus3 WHERE iStatus3.ITR_MAIN_GUID = itr2.GUID) ";
            //sql += "OR iStatus2.SEQUENCE_NUMBER IS NULL) ";
            //sql += "AND itr2.TAG_GUID = Wbs1.GUID AND itr2.DELETED IS NULL ";
            //sql += ") LatestITR ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT COUNT(*) AS TOTAL_COUNT FROM TEMPLATE_REGISTER WHERE TAG_GUID = Wbs1.GUID AND DELETED IS NULL) ITR_TOTAL ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT COUNT(*) AS SAVEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NULL OR STATUS_NUMBER >= '-1') ITR_SAVED ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT COUNT(*) AS INSPECTEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '0') ITR_INSPECTED ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT COUNT(*) AS APPROVEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '1') ITR_APPROVED ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT COUNT(*) AS COMPLETEDCOUNT FROM ITR_MAIN WHERE GUID =  LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '2') ITR_COMPLETED ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT COUNT(*) AS CLOSEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '3') ITR_CLOSED ";
            //sql += "WHERE Proj.GUID = '" + projectGuid + "' AND Sch.DISCIPLINE = '" + discipline + "' AND Wbs1.DELETED IS NULL AND Sch.DELETED IS NULL ";
            //sql += "AND Proj.DELETED IS NULL GROUP BY Wbs1.GUID, Wbs1.NAME ";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<wbsCount> wiIncompletes = new List<wbsCount>();
            foreach (DataRow dr in dt.Rows)
            {
                wiIncompletes.Add(new wbsCount()
                {
                    wcWBS = new ValuePair(dr["NAME"].ToString(), (Guid)dr["GUID"]),
                    wcTotalCount = dr.IsNull("TOTAL_COUNT") ? 0 : Convert.ToInt32(dr["TOTAL_COUNT"].ToString()),
                    wcInspectedCount = dr.IsNull("INSPECTED_COUNT") ? 0 : Convert.ToInt32(dr["INSPECTED_COUNT"].ToString()),
                    wcApprovedCount = dr.IsNull("APPROVED_COUNT") ? 0 : Convert.ToInt32(dr["APPROVED_COUNT"].ToString()),
                    wcCompletedCount = dr.IsNull("COMPLETED_COUNT") ? 0 : Convert.ToInt32(dr["COMPLETED_COUNT"].ToString()),
                    wcClosedCount = dr.IsNull("CLOSED_COUNT") ? 0 : Convert.ToInt32(dr["CLOSED_COUNT"].ToString()),
                });
            }

            return wiIncompletes;
        }

        /// <summary>
        /// Get all tag's incompleted template count
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="discipline"></param>
        /// <returns></returns>
        public List<tagCount> GetTagIncompleteTemplateCount(Guid projectGuid)
        {
            string sql = "SELECT SCORE.GUID, SCORE.NUMBER, TOTAL_COUNT = SCORE.PENDING_COUNT + SCORE.NOTINCL_COUNT, SCORE.INSPECTED_COUNT, SCORE.APPROVED_COUNT, SCORE.COMPLETED_COUNT, SCORE.CLOSED_COUNT FROM ";
            sql += "(";
            sql += "SELECT Tag1.GUID, Tag1.NUMBER, MAX(ITR_TOTAL.TOTAL_COUNT) AS PENDING_COUNT,";
            sql += "SUM(ITR_NOTINCL.TOTAL_NOTINCL_COUNT) AS NOTINCL_COUNT, ";
            sql += "SUM(ITR_INSPECTED.INSPECTEDCOUNT) AS INSPECTED_COUNT , ";
            sql += "SUM(ITR_APPROVED.APPROVEDCOUNT) AS APPROVED_COUNT, ";
            sql += "SUM(ITR_COMPLETED.COMPLETEDCOUNT) AS COMPLETED_COUNT, ";
            sql += "SUM(ITR_CLOSED.CLOSEDCOUNT) AS CLOSED_COUNT ";
            sql += "FROM TAG Tag1 JOIN SCHEDULE Sch ON (Sch.GUID = tag1.SCHEDULEGUID) ";
            sql += "JOIN PROJECT Proj ON (Proj.GUID = Sch.PROJECTGUID) ";
            sql += "OUTER APPLY ";
            sql += "(SELECT Itr2.GUID, iStatus2.STATUS_NUMBER, iStatus2.SEQUENCE_NUMBER FROM ITR_MAIN Itr2 ";
            sql += "LEFT JOIN ITR_STATUS iStatus2 ON (iStatus2.ITR_MAIN_GUID = Itr2.GUID) ";
            sql += "WHERE itr2.TAG_GUID = tag1.GUID AND Itr2.SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) FROM ITR_MAIN Itr3 WHERE Itr3.GUID = Itr2.GUID) ";
            sql += "AND (iStatus2.SEQUENCE_NUMBER = (SELECT MAX(iStatus3.SEQUENCE_NUMBER) FROM ITR_STATUS iStatus3 WHERE iStatus3.ITR_MAIN_GUID = itr2.GUID) ";
            sql += "OR iStatus2.SEQUENCE_NUMBER IS NULL) AND itr2.TAG_GUID = Tag1.GUID AND itr2.DELETED IS NULL ) LatestITR ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS TOTAL_COUNT ";
            sql += "FROM TEMPLATE_REGISTER ";
            sql += "JOIN TEMPLATE_MAIN ON TEMPLATE_REGISTER.TEMPLATE_GUID = TEMPLATE_MAIN.GUID ";
            sql += "WHERE TAG_GUID = Tag1.GUID AND TEMPLATE_REGISTER.DELETED IS NULL AND TEMPLATE_MAIN.DELETED IS NULL) ITR_TOTAL ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS TOTAL_NOTINCL_COUNT ";
            sql += "FROM ITR_MAIN LEFT JOIN ";
            sql += "(SELECT TEMPLATE_REGISTER.* FROM TEMPLATE_REGISTER JOIN TEMPLATE_MAIN ON TEMPLATE_MAIN.GUID = TEMPLATE_REGISTER.TEMPLATE_GUID WHERE TEMPLATE_REGISTER.DELETED IS NULL AND TEMPLATE_MAIN.DELETED IS NULL) Register1 ON Register1.TAG_GUID = ITR_MAIN.TAG_GUID AND Register1.TEMPLATE_GUID = ITR_MAIN.TEMPLATE_GUID ";
            sql += "WHERE Register1.GUID IS NULL AND ITR_MAIN.GUID = LatestITR.GUID ";
            sql += ") ITR_NOTINCL ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS INSPECTEDCOUNT ";
            sql += "FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '0') ITR_INSPECTED ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS APPROVEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '1') ITR_APPROVED ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS COMPLETEDCOUNT FROM ITR_MAIN WHERE GUID =  LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '2') ITR_COMPLETED ";
            sql += "OUTER APPLY ";
            sql += "(SELECT COUNT(*) AS CLOSEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '3') ITR_CLOSED WHERE Proj.GUID = '" + projectGuid + "' AND Tag1.DELETED IS NULL AND Sch.DELETED IS NULL AND Proj.DELETED IS NULL GROUP BY Tag1.GUID, Tag1.NUMBER";
            sql += ") SCORE";


            //string sql = "SELECT Tag1.GUID, Tag1.NUMBER, MAX(ITR_TOTAL.TOTAL_COUNT) AS TOTAL_COUNT ";
            //sql += ", SUM(ITR_INSPECTED.INSPECTEDCOUNT) AS INSPECTED_COUNT ";
            //sql += ", SUM(ITR_APPROVED.APPROVEDCOUNT) AS APPROVED_COUNT ";
            //sql += ", SUM(ITR_COMPLETED.COMPLETEDCOUNT) AS COMPLETED_COUNT ";
            //sql += ", SUM(ITR_CLOSED.CLOSEDCOUNT) AS CLOSED_COUNT ";
            //sql += "FROM TAG Tag1 JOIN SCHEDULE Sch ON (Sch.GUID = tag1.SCHEDULEGUID) ";
            //sql += "JOIN PROJECT Proj ON (Proj.GUID = Sch.PROJECTGUID) ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT Itr2.GUID, iStatus2.STATUS_NUMBER, iStatus2.SEQUENCE_NUMBER FROM ITR_MAIN Itr2 LEFT JOIN ITR_STATUS iStatus2 ON (iStatus2.ITR_MAIN_GUID = Itr2.GUID) ";
            //sql += "WHERE ";
            //sql += "itr2.TAG_GUID = tag1.GUID ";
            //sql += "AND ";
            //sql += "Itr2.SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) FROM ITR_MAIN Itr3 WHERE Itr3.GUID = Itr2.GUID) ";
            //sql += "AND ";
            //sql += "(iStatus2.SEQUENCE_NUMBER = (SELECT MAX(iStatus3.SEQUENCE_NUMBER) FROM ITR_STATUS iStatus3 WHERE iStatus3.ITR_MAIN_GUID = itr2.GUID) ";
            //sql += "OR iStatus2.SEQUENCE_NUMBER IS NULL) ";
            //sql += "AND itr2.TAG_GUID = Tag1.GUID AND itr2.DELETED IS NULL ";
            //sql += ") LatestITR ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT COUNT(*) AS TOTAL_COUNT FROM TEMPLATE_REGISTER WHERE TAG_GUID = Tag1.GUID AND DELETED IS NULL) ITR_TOTAL ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT COUNT(*) AS INSPECTEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '0') ITR_INSPECTED ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT COUNT(*) AS APPROVEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '1') ITR_APPROVED ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT COUNT(*) AS COMPLETEDCOUNT FROM ITR_MAIN WHERE GUID =  LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '2') ITR_COMPLETED ";
            //sql += "OUTER APPLY ";
            //sql += "(SELECT COUNT(*) AS CLOSEDCOUNT FROM ITR_MAIN WHERE GUID = LatestITR.GUID AND STATUS_NUMBER IS NOT NULL AND STATUS_NUMBER >= '3') ITR_CLOSED ";
            //sql += "WHERE Proj.GUID = '" + projectGuid + "' AND Sch.DISCIPLINE = '" + discipline + "' AND Tag1.DELETED IS NULL AND Sch.DELETED IS NULL ";
            //sql += "AND Proj.DELETED IS NULL GROUP BY Tag1.GUID, Tag1.NUMBER";


            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<tagCount> tiIncompletes = new List<tagCount>();
            foreach (DataRow dr in dt.Rows)
            {
                tiIncompletes.Add(new tagCount()
                {
                    tcTag = new ValuePair(dr["NUMBER"].ToString(), (Guid)dr["GUID"]),
                    tcTotalCount = dr.IsNull("TOTAL_COUNT") ? 0 : Convert.ToInt32(dr["TOTAL_COUNT"].ToString()),
                    tcInspectedCount = dr.IsNull("INSPECTED_COUNT") ? 0 : Convert.ToInt32(dr["INSPECTED_COUNT"].ToString()),
                    tcApprovedCount = dr.IsNull("APPROVED_COUNT") ? 0 : Convert.ToInt32(dr["APPROVED_COUNT"].ToString()),
                    tcCompletedCount = dr.IsNull("COMPLETED_COUNT") ? 0 : Convert.ToInt32(dr["COMPLETED_COUNT"].ToString()),
                    tcClosedCount = dr.IsNull("CLOSED_COUNT") ? 0 : Convert.ToInt32(dr["CLOSED_COUNT"].ToString()),
                });
            }

            return tiIncompletes;
        }

        /// <summary>
        /// Get all tag's incompleted template count - Deprecated Release_1.0.0.157
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="discipline"></param>
        /// <returns></returns>
        public List<tagCount> GetTagIncompleteTemplateCount_Old(Guid projectGuid, string discipline)
        {
            string sql = "SELECT tag.GUID AS TAG_GUID, tag.NUMBER, regitr.INCOMPLETED_COUNT FROM SCHEDULE sch";
            sql += " JOIN TAG tag ON (tag.SCHEDULEGUID = sch.GUID)";
            sql += " LEFT JOIN ";
            sql += " (SELECT reg.TAG_GUID, COUNT(*) AS INCOMPLETED_COUNT";
            sql += " FROM TEMPLATE_REGISTER reg";
            sql += " JOIN TEMPLATE_MAIN template ON (template.GUID = reg.TEMPLATE_GUID)";
            sql += " LEFT JOIN(SELECT * FROM ITR_MAIN a WHERE SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) FROM ITR_MAIN b WHERE a.TAG_GUID = b.TAG_GUID AND a.TEMPLATE_GUID = b.TEMPLATE_GUID))";
            sql += " itr ON (itr.TEMPLATE_GUID = reg.TEMPLATE_GUID AND itr.TAG_GUID = reg.TAG_GUID)";
            sql += " LEFT JOIN(SELECT * FROM ITR_STATUS a WHERE SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) FROM ITR_STATUS b WHERE a.ITR_MAIN_GUID = b.ITR_MAIN_GUID))";
            sql += " istatus ON (istatus.ITR_MAIN_GUID = itr.GUID)";
            sql += " WHERE reg.DELETED IS NULL AND template.DELETED IS NULL AND (itr.DELETED IS NOT NULL OR istatus.STATUS_NUMBER IS NULL OR istatus.STATUS_NUMBER < 2)";
            sql += " GROUP BY reg.TAG_GUID)";
            sql += " regitr ON (regitr.TAG_GUID = tag.GUID)";
            sql += " WHERE sch.PROJECTGUID = '" + projectGuid + "' AND sch.DISCIPLINE = '" + discipline + "'";
            sql += " AND tag.DELETED IS NULL";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<tagCount> tiIncompletes = new List<tagCount>();
            foreach (DataRow dr in dt.Rows)
            {
                tiIncompletes.Add(new tagCount()
                {
                    tcTag = new ValuePair(dr["NUMBER"].ToString(), (Guid)dr["TAG_GUID"]),
                    tcCompletedCount = dr.IsNull("INCOMPLETED_COUNT") ? 0 : Convert.ToInt32(dr["INCOMPLETED_COUNT"].ToString())
                });
            }

            return tiIncompletes;
        }

        /// <summary>
        /// Get tags that hasn't been progressed to status 0
        /// </summary>
        public List<tagWorkflow> GetUnprogressedTagTemplate(Guid projectGuid)
        {
            string sql = "SELECT tag.GUID AS TAG_GUID, tag.Number AS TAG_NUM, workflow.GUID AS WF_GUID, workflow.NAME AS WF_NAME, itr.GUID, itr.DELETED, istatus.STATUS_NUMBER";
            sql += " FROM SCHEDULE sch";
            sql += " JOIN TAG tag ON (tag.SCHEDULEGUID = sch.GUID)";
            sql += " JOIN TEMPLATE_REGISTER reg ON (reg.TAG_GUID = tag.GUID)";
            sql += " JOIN TEMPLATE_MAIN template ON (template.GUID = reg.TEMPLATE_GUID)";
            sql += " JOIN WORKFLOW_MAIN workflow ON (workflow.GUID = template.WORKFLOWGUID)";
            sql += " LEFT JOIN";
            sql += " (SELECT * FROM ITR_MAIN a WHERE a.DELETED IS NULL AND SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) FROM ITR_MAIN b WHERE a.TAG_GUID = b.TAG_GUID AND a.TEMPLATE_GUID = b.TEMPLATE_GUID))";
            sql += " itr ON (itr.TAG_GUID = tag.GUID AND itr.TEMPLATE_GUID = template.GUID)";
            sql += " LEFT JOIN";
            sql += " (SELECT * FROM ITR_STATUS a WHERE SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) FROM ITR_STATUS b WHERE a.ITR_MAIN_GUID = b.ITR_MAIN_GUID))";
            sql += " istatus ON (istatus.ITR_MAIN_GUID = itr.GUID)";
            sql += " WHERE sch.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND sch.DELETED IS NULL AND tag.DELETED IS NULL AND reg.DELETED IS NULL AND template.DELETED IS NULL AND workflow.DELETED IS NULL";
            sql += " AND itr.DELETED IS NULL AND (istatus.STATUS_NUMBER IS NULL OR istatus.STATUS_NUMBER < 0)";
            sql += " ORDER BY tag.NUMBER, workflow.NAME";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<tagWorkflow> twList = new List<tagWorkflow>();
            foreach (DataRow dr in dt.Rows)
            {
                twList.Add(new tagWorkflow()
                {

                    twTag = new ValuePair(dr["TAG_NUM"].ToString(), (Guid)dr["TAG_GUID"]),
                    twWorkflow = new ValuePair(dr["WF_NAME"].ToString(), (Guid)dr["WF_GUID"]),

                });
            }

            return twList;
        }

        /// <summary>
        /// Get wbs that hasn't been progressed to status 0
        /// </summary>
        public List<wbsWorkflow> GetUnprogressedWBSTemplate(Guid projectGuid)
        {
            string sql = " SELECT wbs.GUID AS WBS_GUID, wbs.NAME AS WBS_NAME, workflow.GUID AS WF_GUID, workflow.NAME AS WF_NAME, itr.GUID, itr.DELETED, istatus.STATUS_NUMBER";
            sql += " FROM SCHEDULE sch";
            sql += " JOIN WBS wbs ON (wbs.SCHEDULEGUID = sch.GUID)";
            sql += " JOIN TEMPLATE_REGISTER reg ON (reg.WBS_GUID = wbs.GUID)";
            sql += " JOIN TEMPLATE_MAIN template ON (template.GUID = reg.TEMPLATE_GUID)";
            sql += " JOIN WORKFLOW_MAIN workflow ON (workflow.GUID = template.WORKFLOWGUID)";
            sql += " LEFT JOIN";
            sql += " (SELECT * FROM ITR_MAIN a WHERE a.DELETED IS NULL AND SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) FROM ITR_MAIN b WHERE a.WBS_GUID = b.WBS_GUID AND a.TEMPLATE_GUID = b.TEMPLATE_GUID))";
            sql += " itr ON (itr.WBS_GUID = wbs.GUID AND itr.TEMPLATE_GUID = template.GUID)";
            sql += " LEFT JOIN";
            sql += " (SELECT * FROM ITR_STATUS a WHERE SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) FROM ITR_STATUS b WHERE a.ITR_MAIN_GUID = b.ITR_MAIN_GUID))";
            sql += " istatus ON (istatus.ITR_MAIN_GUID = itr.GUID)";
            sql += " WHERE sch.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND sch.DELETED IS NULL AND wbs.DELETED IS NULL AND reg.DELETED IS NULL AND template.DELETED IS NULL AND workflow.DELETED IS NULL";
            sql += " AND (itr.DELETED IS NOT NULL OR istatus.STATUS_NUMBER IS NULL OR istatus.STATUS_NUMBER < 0)";
            sql += " ORDER BY wbs.NAME, workflow.NAME";


            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            List<wbsWorkflow> wbsWorkflowList = new List<wbsWorkflow>();
            foreach (DataRow dr in dt.Rows)
            {
                wbsWorkflowList.Add(new wbsWorkflow()
                {
                    wwWBS = new ValuePair(dr["WBS_NAME"].ToString(), (Guid)dr["WBS_GUID"]),
                    wwWorkflow = new ValuePair(dr["WF_NAME"].ToString(), (Guid)dr["WF_GUID"]),
                });
            }

            return wbsWorkflowList;
        }

        /// <summary>
        /// Remove a particular template by GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM TEMPLATE_MAIN WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTemplate = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            ExecuteQuery(sql, dtTemplate);

            if (dtTemplate != null)
            {
                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = dtTemplate[0];
                drTemplate.DELETED = DateTime.Now;
                drTemplate.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drTemplate);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove all template by name except exclusion
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="excludedGuid"></param>
        /// <returns></returns>
        public int RemoveWithExclusionBy(string templateName, Guid excludedGuid)
        {
            string sql = "SELECT * FROM TEMPLATE_MAIN WHERE NAME = '" + templateName + "'";
            sql += " AND GUID IS NOT '" + excludedGuid + "'";
            sql += " ANd DELETED IS NULL";

            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTemplate = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            ExecuteQuery(sql, dtTemplate);

            int removeCount = 0;
            if(dtTemplate != null)
            {
                foreach(dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate in dtTemplate.Rows)
                {
                    drTemplate.DELETED = DateTime.Now;
                    drTemplate.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drTemplate);
                    removeCount++;
                }
            }

            return removeCount;
        }


        /// <summary>
        /// Update multiple templates
        /// </summary>
        public void Save(dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN)
        {
            _adapter.Update(dtTEMPLATE_MAIN);
        }

        /// <summary>
        /// Update one template
        /// </summary>
        public void Save(dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTEMPLATE_MAIN)
        {
            _adapter.Update(drTEMPLATE_MAIN);
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