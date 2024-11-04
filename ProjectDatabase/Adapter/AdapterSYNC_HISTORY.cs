using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsSYNC_HISTORYTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterSYNC_HISTORY : SQLBase, IDisposable
    {
        private SYNC_HISTORYTableAdapter _adapter;

        public AdapterSYNC_HISTORY()
            : base()
        {
            _adapter = new SYNC_HISTORYTableAdapter(Variables.ConnStr);
        }

        public AdapterSYNC_HISTORY(string connStr)
            : base(connStr)
        {
            _adapter = new SYNC_HISTORYTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Gets sync history by hwid
        /// </summary>
        public dsSYNC_HISTORY.SYNC_HISTORYDataTable GetBy(string hwid)
        {
            string sql = "SELECT * FROM SYNC_HISTORY WHERE MACHINE_HWID = '" + hwid + "'";
            sql += " ORDER BY SYNCED";

            dsSYNC_HISTORY.SYNC_HISTORYDataTable dtSYNC_HISTORY = new dsSYNC_HISTORY.SYNC_HISTORYDataTable();
            ExecuteQuery(sql, dtSYNC_HISTORY);

            if (dtSYNC_HISTORY.Rows.Count > 0)
                return dtSYNC_HISTORY;
            else
                return null;
        }

        public void RemoveBy(string hwid)
        {
            string sql = "DELETE FROM SYNC_HISTORY WHERE MACHINE_HWID = '" + hwid + "'";
            ExecuteNonQuery(sql);
        }

        /// <summary>
        /// Update multiple syncs
        /// </summary>
        public void Save(dsSYNC_HISTORY.SYNC_HISTORYDataTable dtSYNC_HISTORY)
        {
            _adapter.Update(dtSYNC_HISTORY);
        }

        /// <summary>
        /// Update one sync
        /// </summary>
        public void Save(dsSYNC_HISTORY.SYNC_HISTORYRow drSYNC_HISTORY)
        {
            _adapter.Update(drSYNC_HISTORY);
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
