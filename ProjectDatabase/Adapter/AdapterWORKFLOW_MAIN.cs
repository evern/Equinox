using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsWORKFLOW_MAINTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterWORKFLOW_MAIN : SQLBase, IDisposable
    {
        private WORKFLOW_MAINTableAdapter _adapter;

        public AdapterWORKFLOW_MAIN()
            : base()
        {
            _adapter = new WORKFLOW_MAINTableAdapter(Variables.ConnStr);
        }

        public AdapterWORKFLOW_MAIN(string connStr)
            : base(connStr)
        {
            _adapter = new WORKFLOW_MAINTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all workflow in the system
        /// </summary>
        public dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable Get()
        {
            string sql = "SELECT * FROM WORKFLOW_MAIN";
            sql += "  WHERE DELETED IS NULL";
            sql += " ORDER BY NAME";

            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWORKFLOW_MAIN = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable();
            ExecuteQuery(sql, dtWORKFLOW_MAIN);

            if (dtWORKFLOW_MAIN.Rows.Count > 0)
                return dtWORKFLOW_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get deleted workflow in the system for purging
        /// </summary>
        public dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable Get_Deleted()
        {
            string sql = "SELECT * FROM WORKFLOW_MAIN WHERE DELETED IS NOT NULL";

            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWORKFLOW_MAIN = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable();
            ExecuteQuery(sql, dtWORKFLOW_MAIN);

            if (dtWORKFLOW_MAIN.Rows.Count > 0)
                return dtWORKFLOW_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get all workflow in the system
        /// </summary>
        public dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable GetIncludeDeletes()
        {
            string sql = "SELECT * FROM WORKFLOW_MAIN";
            sql += " ORDER BY NAME";

            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWORKFLOW_MAIN = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable();
            ExecuteQuery(sql, dtWORKFLOW_MAIN);

            if (dtWORKFLOW_MAIN.Rows.Count > 0)
                return dtWORKFLOW_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get all workflow including deletes
        /// </summary>
        public dsWORKFLOW_MAIN.WORKFLOW_MAINRow GetIncludeDeletedBy(Guid workflowGuid)
        {
            string sql = "SELECT * FROM WORKFLOW_MAIN";
            sql += " WHERE GUID = '" + workflowGuid + "'";
            sql += " ORDER BY NAME";

            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWORKFLOW_MAIN = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable();
            ExecuteQuery(sql, dtWORKFLOW_MAIN);

            if (dtWORKFLOW_MAIN.Rows.Count > 0)
                return dtWORKFLOW_MAIN[0];
            else
                return null;
        }


        /// <summary>
        /// Get workflow by guid
        /// </summary>
        public dsWORKFLOW_MAIN.WORKFLOW_MAINRow GetBy(Guid workflowGuid, bool includeDeleted)
        {
            string sql = "SELECT * FROM WORKFLOW_MAIN WHERE GUID = '" + workflowGuid + "'";
            if(!includeDeleted)
                sql += " AND DELETED IS NULL";

            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWORKFLOW_MAIN = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable();
            ExecuteQuery(sql, dtWORKFLOW_MAIN);

            if (dtWORKFLOW_MAIN.Rows.Count > 0)
                return dtWORKFLOW_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get workflow by name
        /// </summary>
        public dsWORKFLOW_MAIN.WORKFLOW_MAINRow GetBy(string name)
        {
            string sql = "SELECT * FROM WORKFLOW_MAIN WHERE NAME = '" + name + "'";
            sql += " AND DELETED IS NULL";

            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWORKFLOW_MAIN = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable();
            ExecuteQuery(sql, dtWORKFLOW_MAIN);

            if (dtWORKFLOW_MAIN.Rows.Count > 0)
                return dtWORKFLOW_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get recursive workflow childrens
        /// </summary>
        public dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable GetWorkflowChildrens(Guid parentGuid)
        {
            string sql = "WITH RECURSIVETBL AS (SELECT a.* FROM WORKFLOW_MAIN a ";
            sql += "WHERE PARENTGUID = '" + parentGuid + "' UNION ALL ";
            sql += "SELECT a.* FROM WORKFLOW_MAIN a JOIN RECURSIVETBL r ON a.PARENTGUID = r.GUID) ";
            sql += "SELECT * FROM RECURSIVETBL WHERE RECURSIVETBL.DELETED IS NULL ";
            sql += "UNION ALL ";
            sql += "SELECT * FROM WORKFLOW_MAIN WHERE GUID = '" + parentGuid + "'";

            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWORKFLOW_MAIN = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable();
            ExecuteQuery(sql, dtWORKFLOW_MAIN);

            if (dtWORKFLOW_MAIN.Rows.Count > 0)
                return dtWORKFLOW_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Workflow can have only a single finishing point, this provides a mean to check if it has been assigned
        /// </summary>
        public bool IsRootExists()
        {
            string sql = "SELECT * FROM WORKFLOW_MAIN WHERE PARENTGUID = '" + Guid.Empty + "'";
            sql += " AND DELETED IS NULL";

            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWORKFLOW_MAIN = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable();
            ExecuteQuery(sql, dtWORKFLOW_MAIN);

            if (dtWORKFLOW_MAIN.Rows.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Remove a particular Role by GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM WORKFLOW_MAIN WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWorkflow = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable();
            ExecuteQuery(sql, dtWorkflow);

            if (dtWorkflow != null)
            {
                dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow = dtWorkflow[0];
                drWorkflow.DELETED = DateTime.Now;
                drWorkflow.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drWorkflow);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove workflow by name with exclusion
        /// </summary>
        public int RemoveWithExclusionBy(string workflowName, Guid excludedGuid)
        {
            string sql = "SELECT * FROM WORKFLOW_MAIN WHERE NAME = '" + workflowName + "'";
            sql += " AND GUID IS NOT '" + excludedGuid + "'";
            sql += " AND DELETED IS NULL";

            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWorkflow = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable();
            ExecuteQuery(sql, dtWorkflow);

            int removeCount = 0;
            if (dtWorkflow != null)
            {
                foreach(dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow in dtWorkflow.Rows)
                {
                    drWorkflow.DELETED = DateTime.Now;
                    drWorkflow.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drWorkflow);
                    removeCount++;
                }
            }

            return removeCount;
        }


        /// <summary>
        /// Update multiple workflows
        /// </summary>
        public void Save(dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWORKFLOW_MAIN)
        {
            _adapter.Update(dtWORKFLOW_MAIN);
        }

        /// <summary>
        /// Update one workflow
        /// </summary>
        public void Save(dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWORKFLOW_MAIN)
        {
            _adapter.Update(drWORKFLOW_MAIN);
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
