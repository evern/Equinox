using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsPUNCHLIST_STATUS_ISSUETableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterPUNCHLIST_STATUS_ISSUE : SQLBase, IDisposable
    {
        private PUNCHLIST_STATUS_ISSUETableAdapter _adapter;

        public AdapterPUNCHLIST_STATUS_ISSUE()
            : base()
        {
            _adapter = new PUNCHLIST_STATUS_ISSUETableAdapter(Variables.ConnStr);
        }

        public AdapterPUNCHLIST_STATUS_ISSUE(string connStr)
            : base(connStr)
        {
            _adapter = new PUNCHLIST_STATUS_ISSUETableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        /// <summary>
        /// Get punchlist status issue by punchlist status guid
        /// </summary>
        public dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable GetBy(Guid statusGuid)
        {
            string sql = "SELECT * FROM PUNCHLIST_STATUS_ISSUE WHERE PUNCHLIST_STATUS_GUID = '" + statusGuid + "'";
            sql += " ORDER BY SEQUENCE_NUMBER";

            dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable dtPUNCHLIST_STATUS_ISSUE = new dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_STATUS_ISSUE);

            if (dtPUNCHLIST_STATUS_ISSUE.Rows.Count > 0)
                return dtPUNCHLIST_STATUS_ISSUE;
            else
                return null;
        }

        /// <summary>
        /// Get latest punchlist issue by punchlist status guid
        /// </summary>
        public dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUERow GetLatestBy(Guid statusGuid)
        {
            string sql = "SELECT TOP 1 * FROM PUNCHLIST_STATUS_ISSUE WHERE PUNCHLIST_STATUS_GUID = '" + statusGuid + "'";
            sql += " ORDER BY SEQUENCE_NUMBER DESC";

            dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable dtPUNCHLIST_STATUS_ISSUE = new dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_STATUS_ISSUE);

            if (dtPUNCHLIST_STATUS_ISSUE.Rows.Count > 0)
                return dtPUNCHLIST_STATUS_ISSUE[0];
            else
                return null;

        }

        /// <summary>
        /// Add a comment and with increased sequence number
        /// </summary>
        public void AddComments(Guid punchlistStatusGuid, string comments, bool rejection)
        {
            string sql = "SELECT * FROM PUNCHLIST_STATUS_ISSUE WHERE PUNCHLIST_STATUS_GUID = '" + punchlistStatusGuid + "'";
            sql += " ORDER BY SEQUENCE_NUMBER";

            dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable dtPunchlistIssue = new dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtPunchlistIssue);

            int Sequence = 1;

            if (dtPunchlistIssue.Rows.Count > 0)
            {
                Sequence = dtPunchlistIssue.Rows.Count + 1;
            }

            dsPUNCHLIST_STATUS_ISSUE dsPunchlistIssue = new dsPUNCHLIST_STATUS_ISSUE();
            dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUERow drNewPunchlistIssue = dsPunchlistIssue.PUNCHLIST_STATUS_ISSUE.NewPUNCHLIST_STATUS_ISSUERow();
            drNewPunchlistIssue.GUID = Guid.NewGuid();
            drNewPunchlistIssue.PUNCHLIST_STATUS_GUID = punchlistStatusGuid;
            drNewPunchlistIssue.SEQUENCE_NUMBER = Sequence;
            drNewPunchlistIssue.COMMENTS = comments;
            drNewPunchlistIssue.COMMENTS_READ = false;
            drNewPunchlistIssue.REJECTION = rejection;
            drNewPunchlistIssue.CREATED = DateTime.Now;
            drNewPunchlistIssue.CREATEDBY = System_Environment.GetUser().GUID;
            dsPunchlistIssue.PUNCHLIST_STATUS_ISSUE.AddPUNCHLIST_STATUS_ISSUERow(drNewPunchlistIssue);
            Save(drNewPunchlistIssue);
        }

        /// <summary>
        /// Get a particular project by project number
        /// </summary>
        public dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUERow GetBy(string number)
        {
            string sql = "SELECT * FROM PUNCHLIST_STATUS_ISSUE WHERE NUMBER = '" + number + "'";
            sql += " AND DELETED IS NULL";

            dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable dtPUNCHLIST_STATUS_ISSUE = new dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_STATUS_ISSUE);

            if (dtPUNCHLIST_STATUS_ISSUE.Rows.Count > 0)
                return dtPUNCHLIST_STATUS_ISSUE[0];
            else
                return null;
        }

        /// <summary>
        /// Get all projects
        /// </summary
        public dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable Get()
        {
            string sql = "SELECT * FROM PUNCHLIST_STATUS_ISSUE WHERE DELETED IS NULL";

            dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable dtPUNCHLIST_STATUS_ISSUE = new dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_STATUS_ISSUE);

            if (dtPUNCHLIST_STATUS_ISSUE.Rows.Count > 0)
                return dtPUNCHLIST_STATUS_ISSUE;
            else
                return null;
        }

        //Changes 10-DEC-2014 Allow same tag number to be added per project
        /// <summary>
        /// Gets the Tag rejection count for an ITR
        /// </summary>
        public int GetTagRejectionCount(string tagNumber, string punchlistTitle, Guid projectGuid)
        {
            string sql = "SELECT tag.NUMBER, punchlist.TITLE FROM TAG tag JOIN PUNCHLIST_MAIN punchlist ON (punchlist.TAG_GUID = tag.GUID) JOIN PUNCHLIST_STATUS pstatus";
            sql += " ON (pstatus.PUNCHLIST_MAIN_GUID = punchlist.GUID) JOIN PUNCHLIST_STATUS_ISSUE issue ON (issue.PUNCHLIST_STATUS_GUID = pstatus.GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE tag.NUMBER = '" + tagNumber + "' AND punchlist.TITLE = '" + punchlistTitle + "'";
            sql += " AND proj.GUID = '" + projectGuid + "'";
            sql += " AND issue.REJECTION = 1 AND punchlist.DELETED IS NULL";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            return dt.Rows.Count;
        }

        //Changes 12-DEC-2012 punchlist must be validated by project
        /// <summary>
        /// Gets the WBS rejection count for a WBS
        /// </summary>
        /// <param name="wbsName"></param>
        /// <param name="punchlistTitle"></param>
        /// <returns></returns>
        public int GetWBSRejectionCount(string wbsName, string punchlistTitle, Guid projectGuid)
        {
            string sql = "SELECT wbs.NAME, punchlist.TITLE FROM WBS wbs JOIN PUNCHLIST_MAIN punchlist ON (punchlist.WBS_GUID = wbs.GUID) JOIN PUNCHLIST_STATUS pstatus";
            sql += " ON (pstatus.PUNCHLIST_MAIN_GUID = punchlist.GUID) JOIN PUNCHLIST_STATUS_ISSUE issue ON (issue.PUNCHLIST_STATUS_GUID = pstatus.GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE wbs.NAME = '" + wbsName + "' AND punchlist.TITLE = '" + punchlistTitle + "'";
            sql += " AND proj.GUID = '" + projectGuid + "'";
            sql += " AND issue.REJECTION = 1 AND punchlist.DELETED IS NULL";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            return dt.Rows.Count;
        }

        /// <summary>
        /// Saves multiple projects
        /// </summary>
        public void Save(dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable dtPUNCHLIST_STATUS_ISSUE)
        {
            _adapter.Update(dtPUNCHLIST_STATUS_ISSUE);
        }

        /// <summary>
        /// Saves one project
        /// </summary>
        public void Save(dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUERow drPUNCHLIST_STATUS_ISSUE)
        {
            _adapter.Update(drPUNCHLIST_STATUS_ISSUE);
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
