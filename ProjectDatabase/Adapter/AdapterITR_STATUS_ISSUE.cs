using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsITR_STATUS_ISSUETableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterITR_STATUS_ISSUE : SQLBase, IDisposable
    {
        private ITR_STATUS_ISSUETableAdapter _adapter;

        public AdapterITR_STATUS_ISSUE()
            : base()
        {
            _adapter = new ITR_STATUS_ISSUETableAdapter(Variables.ConnStr);
        }

        public AdapterITR_STATUS_ISSUE(string connStr)
            : base(connStr)
        {
            _adapter = new ITR_STATUS_ISSUETableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all ITR status in the system
        /// </summary>
        public dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable Get()
        {
            string sql = "SELECT * FROM ITR_STATUS_ISSUE WHERE DELETED IS NULL";
            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtITR_STATUS_ISSUE = new dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtITR_STATUS_ISSUE);

            if (dtITR_STATUS_ISSUE.Rows.Count > 0)
                return dtITR_STATUS_ISSUE;
            else
                return null;
        }

        /// <summary>
        /// Get the latest ITR status issue by ITR status guid
        /// </summary>
        public dsITR_STATUS_ISSUE.ITR_STATUS_ISSUERow GetLatestBy(Guid itrStatusGuid)
        {
            string sql = "SELECT TOP 1 * FROM ITR_STATUS_ISSUE WHERE ITR_STATUS_GUID = '" + itrStatusGuid + "'";
            sql += " ORDER BY SEQUENCE_NUMBER";

            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtITR_STATUS_ISSUE = new dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtITR_STATUS_ISSUE);

            if (dtITR_STATUS_ISSUE.Rows.Count > 0)
                return dtITR_STATUS_ISSUE[0];
            else
                return null;
        }

        /// <summary>
        /// Get the latest ITR status issue by ITR status guid
        /// </summary>
        public dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable GetITR_StatusIssueByITRs(List<Guid> iTRGuids)
        {
            string sql = "SELECT ITR_STATUS_ISSUE.* FROM ITR_STATUS_ISSUE ";
            sql += "JOIN ITR_STATUS ON ITR_STATUS_ISSUE.ITR_STATUS_GUID = ITR_STATUS.GUID ";

            if (iTRGuids.Count == 0)
            {
                return null;
            }
            else if (iTRGuids.Count == 1)
            {
                sql += "AND (ITR_MAIN_GUID = '" + iTRGuids[0] + "')";
            }
            else
            {
                for (int i = 0; i < iTRGuids.Count; i++)
                {
                    if (i == 0)
                        sql += "AND (ITR_MAIN_GUID = '" + iTRGuids[i] + "' ";
                    else if (i == iTRGuids.Count - 1)
                        sql += " OR ITR_MAIN_GUID = '" + iTRGuids[i] + "')";
                    else
                        sql += "OR ITR_MAIN_GUID = '" + iTRGuids[i] + "' ";
                }
            }

            sql += " ORDER BY ITR_STATUS_ISSUE.SEQUENCE_NUMBER DESC";

            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtITR_STATUS_ISSUE = new dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtITR_STATUS_ISSUE);

            if (dtITR_STATUS_ISSUE.Rows.Count > 0)
                return dtITR_STATUS_ISSUE;
            else
                return null;
        }

        /// <summary>
        /// Get the latest ITR status issue by ITR status guid
        /// </summary>
        public dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable GetTagStatusIssueByWBSs(List<Guid> wbsGuids)
        {
            string sql = "SELECT ITR_STATUS_ISSUE.* FROM ITR_STATUS_ISSUE ";
            sql += "JOIN ITR_STATUS ON ITR_STATUS_ISSUE.ITR_STATUS_GUID = ITR_STATUS.GUID ";
            sql += "JOIN ITR_MAIN ON ITR_MAIN.GUID = ITR_STATUS.ITR_MAIN_GUID ";
            sql += "JOIN TAG ON ITR_MAIN.TAG_GUID = TAG.GUID ";
            sql += "JOIN WBS ON WBS.GUID = TAG.PARENTGUID ";
            sql += "WHERE ITR_MAIN.DELETED IS NULL AND ITR_STATUS.DELETED IS NULL AND TAG.DELETED IS NULL AND WBS.DELETED IS NULL";

            if (wbsGuids.Count == 0)
                return null;
            if (wbsGuids.Count == 1)
            {
                sql += " AND (WBS.GUID = '" + wbsGuids[0] + "')";

            }
            else
            {
                for (int i = 0; i < wbsGuids.Count; i++)
                {
                    if (i == 0)
                        sql += " AND (WBS.GUID = '" + wbsGuids[i] + "' ";
                    else if (i == wbsGuids.Count - 1)
                        sql += " OR WBS.GUID = '" + wbsGuids[i] + "')";
                    else
                        sql += "OR WBS.GUID = '" + wbsGuids[i] + "' ";
                }
            }

            sql += "ORDER BY SEQUENCE_NUMBER DESC";

            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtITR_STATUS_ISSUE = new dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtITR_STATUS_ISSUE);

            if (dtITR_STATUS_ISSUE.Rows.Count > 0)
                return dtITR_STATUS_ISSUE;
            else
                return null;
        }

        /// <summary>
        /// Get the last 2 ITR sequence for comparing status progression
        /// </summary>
        public dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable GetWBSStatusIssueByWBSs(List<Guid> wbsGuids)
        {
            string sql = "SELECT ITR_STATUS_ISSUE.* FROM ITR_STATUS_ISSUE ";
            sql += "JOIN ITR_STATUS ON ITR_STATUS_ISSUE.ITR_STATUS_GUID = ITR_STATUS.GUID ";
            sql += "JOIN ITR_MAIN ON ITR_MAIN.GUID = ITR_STATUS.ITR_MAIN_GUID ";
            sql += "JOIN WBS ON WBS.GUID = ITR_MAIN.WBS_GUID ";
            sql += "WHERE ITR_MAIN.DELETED IS NULL AND ITR_STATUS.DELETED IS NULL AND WBS.DELETED IS NULL";

            if (wbsGuids.Count == 0)
                return null;
            else if (wbsGuids.Count == 1)
            {
                sql += " AND (WBS.GUID = '" + wbsGuids[0] + "')";

            }
            else
            {
                for (int i = 0; i < wbsGuids.Count; i++)
                {
                    if (i == 0)
                        sql += " AND (WBS.GUID = '" + wbsGuids[i] + "' ";
                    else if (i == wbsGuids.Count - 1)
                        sql += " OR WBS.GUID = '" + wbsGuids[i] + "')";
                    else
                        sql += "OR WBS.GUID = '" + wbsGuids[i] + "' ";
                }
            }

            sql += "ORDER BY SEQUENCE_NUMBER DESC";

            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtITR_STATUS_ISSUE = new dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtITR_STATUS_ISSUE);

            if (dtITR_STATUS_ISSUE.Rows.Count > 0)
                return dtITR_STATUS_ISSUE;
            else
                return null;
        }

        /// <summary>
        /// Get ITR status issue by ITR status guid
        /// </summary>
        public dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable GetBy(Guid itrStatusGuid)
        {
            string sql = "SELECT * FROM ITR_STATUS_ISSUE WHERE ITR_STATUS_GUID = '" + itrStatusGuid + "'";
            sql += " ORDER BY SEQUENCE_NUMBER";

            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtITR_STATUS_ISSUE = new dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtITR_STATUS_ISSUE);

            if (dtITR_STATUS_ISSUE.Rows.Count > 0)
                return dtITR_STATUS_ISSUE;
            else
                return null;
        }

        /// <summary>
        /// Add a comment and with increased sequence number
        /// </summary>
        public void AddComments(Guid iTRStatusGuid, string comments, bool rejection)
        {
            string sql = "SELECT * FROM ITR_STATUS_ISSUE WHERE ITR_STATUS_GUID = '" + iTRStatusGuid + "'";
            sql += " ORDER BY SEQUENCE_NUMBER";

            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtITRIssue = new dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtITRIssue);

            int Sequence = 1;

            if (dtITRIssue.Rows.Count > 0)
            {
                Sequence = dtITRIssue.Rows.Count + 1;
            }

            dsITR_STATUS_ISSUE dsITRIssue = new dsITR_STATUS_ISSUE();
            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUERow drNewITRIssue = dsITRIssue.ITR_STATUS_ISSUE.NewITR_STATUS_ISSUERow();
            drNewITRIssue.GUID = Guid.NewGuid();
            drNewITRIssue.ITR_STATUS_GUID = iTRStatusGuid;
            drNewITRIssue.SEQUENCE_NUMBER = Sequence;
            drNewITRIssue.COMMENTS = comments;
            drNewITRIssue.COMMENTS_READ = false;
            drNewITRIssue.REJECTION = rejection;

            drNewITRIssue.CREATED = DateTime.Now;
            drNewITRIssue.CREATEDBY = System_Environment.GetUser().GUID;
            dsITRIssue.ITR_STATUS_ISSUE.AddITR_STATUS_ISSUERow(drNewITRIssue);
            Save(drNewITRIssue);
        }

        //Changes 10-DEC-2014 Allow same tag number to be added per project
        /// <summary>
        /// Gets the Tag rejection count for an ITR, including deleted Tag
        /// </summary>
        public int GetTagRejectionCount(string tagNumber, string iTRName, Guid projectGuid)
        {
            string sql = "SELECT tag.NUMBER, itr.NAME FROM TAG tag LEFT JOIN ITR_MAIN itr ON (itr.TAG_GUID = tag.GUID) LEFT JOIN ITR_STATUS istatus";
            sql += " ON (istatus.ITR_MAIN_GUID = itr.GUID) JOIN ITR_STATUS_ISSUE issue ON (issue.ITR_STATUS_GUID = istatus.GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE tag.NUMBER = '" + tagNumber + "' AND itr.NAME = '" + iTRName + "'";
            sql += " AND proj.GUID = '" + projectGuid + "'";
            sql += " AND issue.REJECTION = 1 AND istatus.DELETED IS NULL AND itr.DELETED IS NULL";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows.Count;
            }
            else
                return 0;
        }

        /// 12-DEC-2014 Fix: WBS goes by name and project guid
        /// <summary>
        /// Gets the WBS rejection count for an ITR, including deleted WBS
        /// </summary>
        public int GetWBSRejectionCount(string wbsName, string iTRName, Guid projectGuid)
        {
            string sql = "SELECT wbs.NAME, itr.NAME FROM WBS wbs LEFT JOIN ITR_MAIN itr ON (itr.WBS_GUID = wbs.GUID) LEFT JOIN ITR_STATUS istatus";
            sql += " ON (istatus.ITR_MAIN_GUID = itr.GUID) JOIN ITR_STATUS_ISSUE issue ON (issue.ITR_STATUS_GUID = istatus.GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE wbs.NAME = '" + wbsName + "' AND itr.NAME = '" + iTRName + "'";
            sql += " AND proj.GUID = '" + projectGuid + "'";
            sql += " AND issue.REJECTION = 1 AND istatus.DELETED IS NULL AND itr.DELETED IS NULL";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            if (dt.Rows.Count > 0)
            {
                return dt.Rows.Count;
            }
            else
                return 0;
        }

        /// <summary>
        /// Update multiple itr status
        /// </summary>
        public void Save(dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtITR_STATUS_ISSUE)
        {
            _adapter.Update(dtITR_STATUS_ISSUE);
        }

        /// <summary>
        /// Update one itr status
        /// </summary>
        public void Save(dsITR_STATUS_ISSUE.ITR_STATUS_ISSUERow drITR_STATUS_ISSUE)
        {
            _adapter.Update(drITR_STATUS_ISSUE);
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