using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsSYNC_PAIRTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterSYNC_PAIR : SQLBase, IDisposable
    {
        private SYNC_PAIRTableAdapter _adapter;

        public AdapterSYNC_PAIR()
            : base()
        {
            _adapter = new SYNC_PAIRTableAdapter(Variables.ConnStr);
        }

        public AdapterSYNC_PAIR(string connStr)
            : base(connStr)
        {
            _adapter = new SYNC_PAIRTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        public dsSYNC_PAIR.SYNC_PAIRRow GetBy(Guid guid)
        {
            string sql = "SELECT * FROM SYNC_PAIR WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsSYNC_PAIR.SYNC_PAIRDataTable dtSYNC_PAIR = new dsSYNC_PAIR.SYNC_PAIRDataTable();
            ExecuteQuery(sql, dtSYNC_PAIR);

            if (dtSYNC_PAIR.Rows.Count > 0)
                return dtSYNC_PAIR[0];
            else
                return null;
        }

        public dsSYNC_PAIR.SYNC_PAIRRow GetBy(string hwid)
        {
            string sql = "SELECT * FROM SYNC_PAIR WHERE HWID = '" + hwid + "'";
            sql += " AND DELETED IS NULL";

            dsSYNC_PAIR.SYNC_PAIRDataTable dtSYNC_PAIR = new dsSYNC_PAIR.SYNC_PAIRDataTable();
            ExecuteQuery(sql, dtSYNC_PAIR);

            if (dtSYNC_PAIR.Rows.Count > 0)
                return dtSYNC_PAIR[0];
            else
                return null;
        }

        public dsSYNC_PAIR.SYNC_PAIRDataTable GetAll()
        {
            string sql = "SELECT * FROM SYNC_PAIR";

            dsSYNC_PAIR.SYNC_PAIRDataTable dtSYNC_PAIR = new dsSYNC_PAIR.SYNC_PAIRDataTable();
            ExecuteQuery(sql, dtSYNC_PAIR);

            if (dtSYNC_PAIR.Rows.Count > 0)
                return dtSYNC_PAIR;
            else
                return null;
        }

        public dsSYNC_PAIR.SYNC_PAIRDataTable GetAll_ExcludeDeleted()
        {
            string sql = "SELECT * FROM SYNC_PAIR WHERE DELETED IS NULL";

            dsSYNC_PAIR.SYNC_PAIRDataTable dtSYNC_PAIR = new dsSYNC_PAIR.SYNC_PAIRDataTable();
            ExecuteQuery(sql, dtSYNC_PAIR);

            if (dtSYNC_PAIR.Rows.Count > 0)
                return dtSYNC_PAIR;
            else
                return null;
        }

        /// <summary>
        /// Remove a particular sync client by GUID
        /// </summary>
        public bool RemoveBy(Guid clientGuid)
        {
            string sql = "SELECT * FROM SYNC_PAIR WHERE GUID = '" + clientGuid + "'";
            sql += " AND DELETED IS NULL";

            dsSYNC_PAIR.SYNC_PAIRDataTable dtSYNCPAIR = new dsSYNC_PAIR.SYNC_PAIRDataTable();
            ExecuteQuery(sql, dtSYNCPAIR);

            if (dtSYNCPAIR != null)
            {
                dsSYNC_PAIR.SYNC_PAIRRow drSYNCPAIR = dtSYNCPAIR[0];
                drSYNCPAIR.DELETED = DateTime.Now;
                drSYNCPAIR.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drSYNCPAIR);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update multiple syncs
        /// </summary>
        public void Save(dsSYNC_PAIR.SYNC_PAIRDataTable dtSYNC_PAIR)
        {
            _adapter.Update(dtSYNC_PAIR);
        }

        /// <summary>
        /// Update one sync
        /// </summary>
        public void Save(dsSYNC_PAIR.SYNC_PAIRRow drSYNC_PAIR)
        {
            _adapter.Update(drSYNC_PAIR);
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
