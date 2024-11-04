using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsSYNC_CONFLICTTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterSYNC_CONFLICT : SQLBase, IDisposable
    {
        private SYNC_CONFLICTTableAdapter _adapter;

        public AdapterSYNC_CONFLICT()
            : base()
        {
            _adapter = new SYNC_CONFLICTTableAdapter(Variables.ConnStr);
        }

        public AdapterSYNC_CONFLICT(string connStr)
            : base(connStr)
        {
            _adapter = new SYNC_CONFLICTTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        public dsSYNC_CONFLICT.SYNC_CONFLICTDataTable GetAll()
        {
            string sql = "SELECT * FROM SYNC_CONFLICT";

            dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtSYNC_CONFLICT = new dsSYNC_CONFLICT.SYNC_CONFLICTDataTable();
            ExecuteQuery(sql, dtSYNC_CONFLICT);

            if (dtSYNC_CONFLICT.Rows.Count > 0)
                return dtSYNC_CONFLICT;
            else
                return null;
        }

        public dsSYNC_CONFLICT.SYNC_CONFLICTDataTable Get_Conflict_Context(Guid ConflictOnGuid)
        {
            string sql = "SELECT * FROM SYNC_CONFLICT WHERE CONFLICT_ON_GUID = '" + ConflictOnGuid + "'";

            dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtSYNC_CONFLICT = new dsSYNC_CONFLICT.SYNC_CONFLICTDataTable();
            ExecuteQuery(sql, dtSYNC_CONFLICT);

            if (dtSYNC_CONFLICT.Rows.Count > 0)
                return dtSYNC_CONFLICT;
            else
                return null;
        }

        public List<Conflict_Table> Get_Unresolved_AsList()
        {
            string sql = "SELECT CONFLICT_TYPE, COUNT(*) AS CONFLICT_TYPE_COUNT ";
            sql += "FROM SYNC_CONFLICT WHERE RESOLVE_GUID IS NULL GROUP BY CONFLICT_TYPE";

            DataTable dtSYNC_CONFLICT = new DataTable();
            ExecuteQuery(sql, dtSYNC_CONFLICT);

            List<Conflict_Table> New_Conflict_Table = new List<Conflict_Table>();

            if (dtSYNC_CONFLICT.Rows.Count > 0)
            {
                foreach(DataRow drSYNC_CONFLICT in dtSYNC_CONFLICT.Rows)
                {
                    New_Conflict_Table.Add(new Conflict_Table() { ConflictCount = (int)drSYNC_CONFLICT["CONFLICT_TYPE_COUNT"], TableName = (string)drSYNC_CONFLICT["CONFLICT_TYPE"] });
                }
            }

            return New_Conflict_Table;
        }

        /// <summary>
        /// Gets sync history by hwid
        /// </summary>
        public dsSYNC_CONFLICT.SYNC_CONFLICTDataTable GetBy(string HWID)
        {
            string sql = "SELECT * FROM SYNC_CONFLICT WHERE CONFLICT_HWID = '" + HWID + "'";

            dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtSYNC_CONFLICT = new dsSYNC_CONFLICT.SYNC_CONFLICTDataTable();
            ExecuteQuery(sql, dtSYNC_CONFLICT);

            if (dtSYNC_CONFLICT.Rows.Count > 0)
                return dtSYNC_CONFLICT;
            else
                return null;
        }

        /// <summary>
        /// Gets sync history by hwid
        /// </summary>
        public dsSYNC_CONFLICT.SYNC_CONFLICTDataTable GetUnresolvedBy_Type(string SyncType)
        {
            string sql = "SELECT * FROM SYNC_CONFLICT WHERE CONFLICT_TYPE = '" + SyncType + "'";
            sql += " AND RESOLVE_GUID IS NULL";

            dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtSYNC_CONFLICT = new dsSYNC_CONFLICT.SYNC_CONFLICTDataTable();
            ExecuteQuery(sql, dtSYNC_CONFLICT);

            if (dtSYNC_CONFLICT.Rows.Count > 0)
                return dtSYNC_CONFLICT;
            else
                return null;
        }

        /// <summary>
        /// Update multiple syncs
        /// </summary>
        public void Save(dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtSYNC_CONFLICT)
        {
            _adapter.Update(dtSYNC_CONFLICT);
        }

        /// <summary>
        /// Update one sync
        /// </summary>
        public void Save(dsSYNC_CONFLICT.SYNC_CONFLICTRow drSYNC_CONFLICT)
        {
            _adapter.Update(drSYNC_CONFLICT);
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
