using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsPREFILL_MAINTableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterPREFILL_MAIN : SQLBase, IDisposable
    {
        private PREFILL_MAINTableAdapter _adapter;

        public AdapterPREFILL_MAIN()
            : base()
        {
            _adapter = new PREFILL_MAINTableAdapter(Variables.ConnStr);
        }

        public AdapterPREFILL_MAIN(string connStr)
            : base(connStr)
        {
            _adapter = new PREFILL_MAINTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsPREFILL_MAIN.PREFILL_MAINDataTable Get()
        {
            string sql = "SELECT * FROM PREFILL_MAIN WHERE DELETED IS NULL";
            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL_MAIN = new dsPREFILL_MAIN.PREFILL_MAINDataTable();
            ExecuteQuery(sql, dtPREFILL_MAIN);

            if (dtPREFILL_MAIN.Rows.Count > 0)
                return dtPREFILL_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get deleted prefill for purging
        /// </summary>
        public dsPREFILL_MAIN.PREFILL_MAINDataTable Get_Deleted(string discipline)
        {
            string sql = "SELECT * FROM PREFILL_MAIN WHERE DELETED IS NOT NULL AND DISCIPLINE = '" + discipline + "'";
            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL_MAIN = new dsPREFILL_MAIN.PREFILL_MAINDataTable();
            ExecuteQuery(sql, dtPREFILL_MAIN);

            if (dtPREFILL_MAIN.Rows.Count > 0)
                return dtPREFILL_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsPREFILL_MAIN.PREFILL_MAINDataTable GetIncludeDeleted()
        {
            string sql = "SELECT * FROM PREFILL_MAIN";
            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL_MAIN = new dsPREFILL_MAIN.PREFILL_MAINDataTable();
            ExecuteQuery(sql, dtPREFILL_MAIN);

            if (dtPREFILL_MAIN.Rows.Count > 0)
                return dtPREFILL_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsPREFILL_MAIN.PREFILL_MAINRow GetIncludeDeletedBy(Guid prefillGuid)
        {
            string sql = "SELECT * FROM PREFILL_MAIN WHERE GUID = '" + prefillGuid + "'";
            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL_MAIN = new dsPREFILL_MAIN.PREFILL_MAINDataTable();
            ExecuteQuery(sql, dtPREFILL_MAIN);

            if (dtPREFILL_MAIN.Rows.Count > 0)
                return dtPREFILL_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get header by discipline
        /// </summary>
        public dsPREFILL_MAIN.PREFILL_MAINDataTable GetByDiscipline(string discipline)
        {
            string sql = "SELECT * FROM PREFILL_MAIN WHERE DISCIPLINE = '" + discipline + "'";
            sql += " AND DELETED IS NULL";

            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL_MAIN = new dsPREFILL_MAIN.PREFILL_MAINDataTable();
            ExecuteQuery(sql, dtPREFILL_MAIN);

            if (dtPREFILL_MAIN.Rows.Count > 0)
                return dtPREFILL_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get header by guid
        /// </summary>
        public dsPREFILL_MAIN.PREFILL_MAINRow GetBy(Guid headerGuid)
        {
            string sql = "SELECT * FROM PREFILL_MAIN WHERE GUID = '" + headerGuid + "'";
            sql += " AND DELETED IS NULL";

            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL_MAIN = new dsPREFILL_MAIN.PREFILL_MAINDataTable();
            ExecuteQuery(sql, dtPREFILL_MAIN);

            if (dtPREFILL_MAIN.Rows.Count > 0)
                return dtPREFILL_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get header by name
        /// </summary>
        public dsPREFILL_MAIN.PREFILL_MAINRow GetBy(string name)
        {
            string sql = "SELECT * FROM PREFILL_MAIN WHERE NAME = '" + name + "'";
            sql += " AND DELETED IS NULL";

            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL_MAIN = new dsPREFILL_MAIN.PREFILL_MAINDataTable();
            ExecuteQuery(sql, dtPREFILL_MAIN);

            if (dtPREFILL_MAIN.Rows.Count > 0)
                return dtPREFILL_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Remove a particular header by GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM PREFILL_MAIN WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsPREFILL_MAIN.PREFILL_MAINDataTable dtHeader = new dsPREFILL_MAIN.PREFILL_MAINDataTable();
            ExecuteQuery(sql, dtHeader);

            if (dtHeader != null)
            {
                dsPREFILL_MAIN.PREFILL_MAINRow drHeader = dtHeader[0];
                drHeader.DELETED = DateTime.Now;
                drHeader.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drHeader);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove a particular header by GUID
        /// </summary>
        public int RemoveWithExclusionBy(string prefillName, Guid excludedGuid)
        {
            string sql = "SELECT * FROM PREFILL_MAIN WHERE NAME = '" + prefillName + "'";
            sql += " AND GUID IS NOT '" + excludedGuid + "'";
            sql += " AND DELETED IS NULL";

            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPrefill = new dsPREFILL_MAIN.PREFILL_MAINDataTable();
            ExecuteQuery(sql, dtPrefill);

            int removeCount = 0;
            if (dtPrefill != null)
            {
                foreach (dsPREFILL_MAIN.PREFILL_MAINRow drPrefill in dtPrefill.Rows)
                {
                    drPrefill.DELETED = DateTime.Now;
                    drPrefill.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drPrefill);
                    removeCount++;
                }
            }

            return removeCount;
        }

        /// <summary>
        /// Update multiple templates
        /// </summary>
        public void Save(dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL_MAIN)
        {
            _adapter.Update(dtPREFILL_MAIN);
        }

        /// <summary>
        /// Update one template
        /// </summary>
        public void Save(dsPREFILL_MAIN.PREFILL_MAINRow drPREFILL_MAIN)
        {
            _adapter.Update(drPREFILL_MAIN);
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