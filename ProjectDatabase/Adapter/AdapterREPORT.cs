using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsREPORTTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterREPORT : SQLBase, IDisposable
    {
        private REPORTTableAdapter _adapter;

        public AdapterREPORT()
            : base()
        {
            _adapter = new REPORTTableAdapter(Variables.ConnStr);
        }

        public AdapterREPORT(string connStr)
            : base(connStr)
        {
            _adapter = new REPORTTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        /// <summary>
        /// Get all reports
        /// </summary>
        public dsREPORT.REPORTDataTable Get()
        {
            string sql = "SELECT * FROM REPORT";
            sql += " WHERE DELETED IS NULL";

            dsREPORT.REPORTDataTable dtREPORT = new dsREPORT.REPORTDataTable();
            ExecuteQuery(sql, dtREPORT);

            if (dtREPORT.Rows.Count > 0)
                return dtREPORT;
            else
                return null;
        }

        /// <summary>
        /// Get deleted reports for purging
        /// </summary>
        public dsREPORT.REPORTDataTable Get_Deleted()
        {
            string sql = "SELECT * FROM REPORT WHERE DELETED IS NOT NULL";
            dsREPORT.REPORTDataTable dtREPORT = new dsREPORT.REPORTDataTable();
            ExecuteQuery(sql, dtREPORT);

            if (dtREPORT.Rows.Count > 0)
                return dtREPORT;
            else
                return null;
        }

        /// <summary>
        /// Get all reports include deleted
        /// </summary>
        public dsREPORT.REPORTDataTable GetAll_IncludeDeleted()
        {
            string sql = "SELECT * FROM REPORT";

            dsREPORT.REPORTDataTable dtREPORT = new dsREPORT.REPORTDataTable();
            ExecuteQuery(sql, dtREPORT);

            if (dtREPORT.Rows.Count > 0)
                return dtREPORT;
            else
                return null;
        }

        public dsREPORT.REPORTRow GetBy_IncludeDeleted(Guid reportGUID)
        {
            string sql = "SELECT * FROM REPORT WHERE GUID = '" + reportGUID + "'";

            dsREPORT.REPORTDataTable dtREPORT = new dsREPORT.REPORTDataTable();
            ExecuteQuery(sql, dtREPORT);

            if (dtREPORT.Rows.Count > 0)
                return dtREPORT[0];
            else
                return null;
        }

        /// <summary>
        /// Get a particular report by GUID
        /// </summary>
        public dsREPORT.REPORTRow GetBy(Guid reportGuid)
        {
            string sql = "SELECT * FROM REPORT WHERE GUID = '" + reportGuid + "'";
            sql += " AND DELETED IS NULL";

            dsREPORT.REPORTDataTable dtREPORT = new dsREPORT.REPORTDataTable();
            ExecuteQuery(sql, dtREPORT);

            if (dtREPORT.Rows.Count > 0)
                return dtREPORT[0];
            else
                return null;
        }

        /// <summary>
        /// Get a particular report by report number
        /// </summary>
        public dsREPORT.REPORTDataTable GetByProject(Guid project_guid)
        {
            string sql = "SELECT * FROM REPORT WHERE PROJECT_GUID = '" + project_guid + "'";
            sql += " AND DELETED IS NULL";

            dsREPORT.REPORTDataTable dtREPORT = new dsREPORT.REPORTDataTable();
            ExecuteQuery(sql, dtREPORT);

            if (dtREPORT.Rows.Count > 0)
                return dtREPORT;
            else
                return null;
        }

        /// <summary>
        /// Get a particular report by report number
        /// </summary>
        public dsREPORT.REPORTRow GetByProject(Guid project_guid, string report_type)
        {
            string sql = "SELECT * FROM REPORT WHERE PROJECT_GUID = '" + project_guid + "'";
            sql += " AND REPORT_TYPE = '" + report_type + "'";
            sql += " AND DELETED IS NULL";

            dsREPORT.REPORTDataTable dtREPORT = new dsREPORT.REPORTDataTable();
            ExecuteQuery(sql, dtREPORT);

            if (dtREPORT.Rows.Count > 0)
                return dtREPORT[0];
            else
                return null;
        }

        /// <summary>
        /// Remove a particular report by GUID
        /// </summary>
        public bool RemoveBy(Guid reportGuid)
        {
            string sql = "SELECT * FROM REPORT WHERE GUID = '" + reportGuid + "'";
            sql += " AND DELETED IS NULL";

            dsREPORT.REPORTDataTable dtREPORT = new dsREPORT.REPORTDataTable();
            ExecuteQuery(sql, dtREPORT);

            if (dtREPORT != null)
            {
                dsREPORT.REPORTRow drREPORT = dtREPORT[0];
                drREPORT.DELETED = DateTime.Now;
                drREPORT.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drREPORT);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Saves multiple reports
        /// </summary>
        public void Save(dsREPORT.REPORTDataTable dtREPORT)
        {
            _adapter.Update(dtREPORT);
        }

        /// <summary>
        /// Saves one report
        /// </summary>
        public void Save(dsREPORT.REPORTRow drREPORT)
        {
            _adapter.Update(drREPORT);
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
