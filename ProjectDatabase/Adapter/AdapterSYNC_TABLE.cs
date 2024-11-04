using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsSYNC_TABLETableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterSYNC_TABLE : SQLBase, IDisposable
    {
        private SYNC_TABLETableAdapter _adapter;

        public AdapterSYNC_TABLE()
            : base()
        {
            _adapter = new SYNC_TABLETableAdapter(Variables.ConnStr);
        }

        public AdapterSYNC_TABLE(string connStr)
            : base(connStr)
        {
            _adapter = new SYNC_TABLETableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        public dsSYNC_TABLE.SYNC_TABLERow GetByType(Guid clientGuid, Sync_Type_Superseded type)
        {
            string sql = "SELECT * FROM SYNC_TABLE";
            sql += " WHERE SYNC_PAIR_GUID = '" + clientGuid + "'";
            sql += " AND TYPE = '" + type + "'";
            sql += " AND DELETED IS NULL";

            dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNC_TABLE = new dsSYNC_TABLE.SYNC_TABLEDataTable();
            ExecuteQuery(sql, dtSYNC_TABLE);

            if (dtSYNC_TABLE.Rows.Count > 0)
                return dtSYNC_TABLE[0];
            else
                return null;
        }

        public dsSYNC_TABLE.SYNC_TABLEDataTable GetBy(Guid clientGuid)
        {
            string sql = "SELECT * FROM SYNC_TABLE";
            sql += " WHERE SYNC_PAIR_GUID = '" + clientGuid + "' AND SYNC_MODE != '" + Sync_Mode.None.ToString() + "'";
            sql += " AND DELETED IS NULL";

            dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNC_TABLE = new dsSYNC_TABLE.SYNC_TABLEDataTable();
            ExecuteQuery(sql, dtSYNC_TABLE);

            if (dtSYNC_TABLE.Rows.Count > 0)
                return dtSYNC_TABLE;
            else
                return null;
        }

        public dsSYNC_TABLE.SYNC_TABLERow GetBy(Guid syncPairGuid, Sync_Type_Superseded syncType)
        {
            string sql = "SELECT * FROM SYNC_TABLE";
            sql += " WHERE TYPE = '" + syncType + "'";
            sql += " AND SYNC_PAIR_GUID = '" + syncPairGuid + "'";
            sql += " AND DELETED IS NULL";

            dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNC_TABLE = new dsSYNC_TABLE.SYNC_TABLEDataTable();
            ExecuteQuery(sql, dtSYNC_TABLE);

            if (dtSYNC_TABLE.Rows.Count > 0)
                return dtSYNC_TABLE[0];
            else
                return null;
        }

        /// <summary>
        /// Update multiple sync tables
        /// </summary>
        public void Save(dsSYNC_TABLE.SYNC_TABLEDataTable dtSYNC_TABLE)
        {
            _adapter.Update(dtSYNC_TABLE);
        }

        /// <summary>
        /// Update one sync table
        /// </summary>
        public void Save(dsSYNC_TABLE.SYNC_TABLERow drSYNC_TABLE)
        {
            _adapter.Update(drSYNC_TABLE);
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
