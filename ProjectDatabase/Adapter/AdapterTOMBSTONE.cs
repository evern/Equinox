using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsTOMBSTONETableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterTOMBSTONE : SQLBase, IDisposable
    {
        private TOMBSTONETableAdapter _adapter;

        public AdapterTOMBSTONE()
            : base()
        {
            _adapter = new TOMBSTONETableAdapter(Variables.ConnStr);
        }

        public AdapterTOMBSTONE(string connStr)
            : base(connStr)
        {
            _adapter = new TOMBSTONETableAdapter(connStr);
        }


        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        /// <summary>
        /// Get all tombstone in the system
        /// </summary>
        public dsTOMBSTONE.TOMBSTONEDataTable Get()
        {
            string sql = "SELECT * FROM TOMBSTONE";
            dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE = new dsTOMBSTONE.TOMBSTONEDataTable();
            ExecuteQuery(sql, dtTOMBSTONE);

            return dtTOMBSTONE;
        }

        /// <summary>
        /// Get all tombstone in the system
        /// </summary>
        public dsTOMBSTONE.TOMBSTONEDataTable GetUpdatedRecords(string tableLastSyncDateStr)
        {
            string sql = "SELECT * FROM TOMBSTONE WHERE CREATED >= '" + tableLastSyncDateStr + "'";
            dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE = new dsTOMBSTONE.TOMBSTONEDataTable();
            ExecuteQuery(sql, dtTOMBSTONE);

            return dtTOMBSTONE;
        }

        /// <summary>
        /// Clear all tombstone records
        /// </summary>
        public int Clear()
        {
            string sql = "DELETE FROM TOMBSTONE";
            int result = ExecuteNonQuery(sql);

            return result;
        }

        /// <summary>
        /// Update multiple tombstones
        /// </summary>
        public void Save(dsTOMBSTONE.TOMBSTONEDataTable dtTOMBSTONE)
        {
            _adapter.Update(dtTOMBSTONE);
        }

        /// <summary>
        /// Update one tombstone
        /// </summary>
        public void Save(dsTOMBSTONE.TOMBSTONERow drTOMBSTONE)
        {
            _adapter.Update(drTOMBSTONE);
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
