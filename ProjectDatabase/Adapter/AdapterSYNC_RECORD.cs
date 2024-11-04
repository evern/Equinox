using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsSYNC_RECORDTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterSYNC_RECORD : SQLBase, IDisposable
    {
        private SYNC_RECORDTableAdapter _adapter;

        public AdapterSYNC_RECORD()
            : base()
        {
            _adapter = new SYNC_RECORDTableAdapter(Variables.ConnStr);
        }

        public AdapterSYNC_RECORD(string connStr)
            : base(connStr)
        {
            _adapter = new SYNC_RECORDTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        public dsSYNC_RECORD.SYNC_RECORDDataTable Get_By(string HWID)
        {
            string sql = "SELECT * FROM SYNC_RECORD WHERE HWID = '" + HWID + "' ";

            dsSYNC_RECORD.SYNC_RECORDDataTable dtSYNC_RECORD = new dsSYNC_RECORD.SYNC_RECORDDataTable();
            ExecuteQuery(sql, dtSYNC_RECORD);

            if (dtSYNC_RECORD.Rows.Count > 0)
                return dtSYNC_RECORD;
            else
                return null;
        }

        public dsSYNC_RECORD.SYNC_RECORDRow Get_Any_By(string HWID)
        {
            string sql = "SELECT * FROM SYNC_RECORD WHERE HWID = '" + HWID + "' ";

            dsSYNC_RECORD.SYNC_RECORDDataTable dtSYNC_RECORD = new dsSYNC_RECORD.SYNC_RECORDDataTable();
            ExecuteQuery(sql, dtSYNC_RECORD);

            if (dtSYNC_RECORD.Rows.Count > 0)
                return dtSYNC_RECORD[0];
            else
                return null;
        }

        public dsSYNC_RECORD.SYNC_RECORDRow Get_By(string HWID, string TableName)
        {
            string sql = "SELECT * FROM SYNC_RECORD WHERE HWID = '" + HWID + "' ";
            sql += "AND TABLE_NAME = '" + TableName + "'";

            dsSYNC_RECORD.SYNC_RECORDDataTable dtSYNC_RECORD = new dsSYNC_RECORD.SYNC_RECORDDataTable();
            ExecuteQuery(sql, dtSYNC_RECORD);

            if (dtSYNC_RECORD.Rows.Count > 0)
                return dtSYNC_RECORD[0];
            else
                return null;
        }

        public void RemoveBy(string HWID)
        {
            string sql = "DELETE FROM SYNC_RECORD WHERE HWID = '" + HWID + "'";

            ExecuteNonQuery(sql);
        }

        /// <summary>
        /// Update multiple sync tables
        /// </summary>
        public void Save(dsSYNC_RECORD.SYNC_RECORDDataTable dtSYNC_RECORD)
        {
            _adapter.Update(dtSYNC_RECORD);
        }

        /// <summary>
        /// Update one sync table
        /// </summary>
        public void Save(dsSYNC_RECORD.SYNC_RECORDRow drSYNC_RECORD)
        {
            _adapter.Update(drSYNC_RECORD);
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
