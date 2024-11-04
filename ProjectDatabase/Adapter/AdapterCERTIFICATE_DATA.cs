using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsCERTIFICATE_DATATableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterCERTIFICATE_DATA : SQLBase, IDisposable
    {
        private CERTIFICATE_DATATableAdapter _adapter;

        public AdapterCERTIFICATE_DATA()
            : base()
        {
            _adapter = new CERTIFICATE_DATATableAdapter(Variables.ConnStr);
        }

        public AdapterCERTIFICATE_DATA(string connStr)
            : base(connStr)
        {
            _adapter = new CERTIFICATE_DATATableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable Get()
        {
            string sql = "SELECT * FROM CERTIFICATE_DATA WHERE DELETED IS NULL";
            dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtCERTIFICATE_DATA = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable();
            ExecuteQuery(sql, dtCERTIFICATE_DATA);

            if (dtCERTIFICATE_DATA.Rows.Count > 0)
                return dtCERTIFICATE_DATA;
            else
                return null;
        }

        /// <summary>
        /// Get deleted prefill for purging
        /// </summary>
        public dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable Get_Deleted(string discipline)
        {
            string sql = "SELECT * FROM CERTIFICATE_DATA WHERE DELETED IS NOT NULL AND DISCIPLINE = '" + discipline + "'";
            dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtCERTIFICATE_DATA = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable();
            ExecuteQuery(sql, dtCERTIFICATE_DATA);

            if (dtCERTIFICATE_DATA.Rows.Count > 0)
                return dtCERTIFICATE_DATA;
            else
                return null;
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable GetIncludeDeleted()
        {
            string sql = "SELECT * FROM CERTIFICATE_DATA";
            dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtCERTIFICATE_DATA = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable();
            ExecuteQuery(sql, dtCERTIFICATE_DATA);

            if (dtCERTIFICATE_DATA.Rows.Count > 0)
                return dtCERTIFICATE_DATA;
            else
                return null;
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsCERTIFICATE_DATA.CERTIFICATE_DATARow GetIncludeDeletedBy(Guid prefillGuid)
        {
            string sql = "SELECT * FROM CERTIFICATE_DATA WHERE GUID = '" + prefillGuid + "'";
            dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtCERTIFICATE_DATA = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable();
            ExecuteQuery(sql, dtCERTIFICATE_DATA);

            if (dtCERTIFICATE_DATA.Rows.Count > 0)
                return dtCERTIFICATE_DATA[0];
            else
                return null;
        }

        /// <summary>
        /// Get header by discipline
        /// </summary>
        public dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable GetByDiscipline(string discipline)
        {
            string sql = "SELECT * FROM CERTIFICATE_DATA WHERE DISCIPLINE = '" + discipline + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtCERTIFICATE_DATA = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable();
            ExecuteQuery(sql, dtCERTIFICATE_DATA);

            if (dtCERTIFICATE_DATA.Rows.Count > 0)
                return dtCERTIFICATE_DATA;
            else
                return null;
        }

        /// <summary>
        /// Get header by guid
        /// </summary>
        public dsCERTIFICATE_DATA.CERTIFICATE_DATARow GetBy(Guid headerGuid)
        {
            string sql = "SELECT * FROM CERTIFICATE_DATA WHERE GUID = '" + headerGuid + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtCERTIFICATE_DATA = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable();
            ExecuteQuery(sql, dtCERTIFICATE_DATA);

            if (dtCERTIFICATE_DATA.Rows.Count > 0)
                return dtCERTIFICATE_DATA[0];
            else
                return null;
        }

        /// <summary>
        /// Get header by guid
        /// </summary>
        public dsCERTIFICATE_DATA.CERTIFICATE_DATARow GetBy(Guid certificateGuid, string dataType, string data, bool includeDeleted)
        {
            string sql = "SELECT * FROM CERTIFICATE_DATA WHERE CERTIFICATEGUID = '" + certificateGuid + "' AND DATA_TYPE = '" + dataType + "' AND DATA1 = '" + data + "'";
            if(!includeDeleted)
                sql += " AND DELETED IS NULL";

            dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtCERTIFICATE_DATA = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable();
            ExecuteQuery(sql, dtCERTIFICATE_DATA);

            if (dtCERTIFICATE_DATA.Rows.Count > 0)
                return dtCERTIFICATE_DATA[0];
            else
                return null;
        }

        /// <summary>
        /// Get header by guid
        /// </summary>
        public void DeleteBy(Guid certificateGuid, string dataType)
        {
            string sql = "SELECT * FROM CERTIFICATE_DATA WHERE CERTIFICATEGUID = '" + certificateGuid + "' AND DATA_TYPE = '" + dataType + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtCERTIFICATE_DATA = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable();
            ExecuteQuery(sql, dtCERTIFICATE_DATA);

            if (dtCERTIFICATE_DATA != null)
            {
                foreach(dsCERTIFICATE_DATA.CERTIFICATE_DATARow drCERTIFICATE_DATA in dtCERTIFICATE_DATA.Rows)
                {
                    drCERTIFICATE_DATA.DELETED = DateTime.Now;
                    drCERTIFICATE_DATA.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drCERTIFICATE_DATA);
                }
            }
        }

        /// <summary>
        /// Get header by name
        /// </summary>
        public dsCERTIFICATE_DATA.CERTIFICATE_DATARow GetBy(string name)
        {
            string sql = "SELECT * FROM CERTIFICATE_DATA WHERE NAME = '" + name + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtCERTIFICATE_DATA = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable();
            ExecuteQuery(sql, dtCERTIFICATE_DATA);

            if (dtCERTIFICATE_DATA.Rows.Count > 0)
                return dtCERTIFICATE_DATA[0];
            else
                return null;
        }

        /// <summary>
        /// Remove a particular header by GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM CERTIFICATE_DATA WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtHeader = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable();
            ExecuteQuery(sql, dtHeader);

            if (dtHeader != null)
            {
                dsCERTIFICATE_DATA.CERTIFICATE_DATARow drHeader = dtHeader[0];
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
            string sql = "SELECT * FROM CERTIFICATE_DATA WHERE NAME = '" + prefillName + "'";
            sql += " AND GUID IS NOT '" + excludedGuid + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtPrefill = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable();
            ExecuteQuery(sql, dtPrefill);

            int removeCount = 0;
            if (dtPrefill != null)
            {
                foreach (dsCERTIFICATE_DATA.CERTIFICATE_DATARow drPrefill in dtPrefill.Rows)
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
        public void Save(dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtCERTIFICATE_DATA)
        {
            _adapter.Update(dtCERTIFICATE_DATA);
        }

        /// <summary>
        /// Update one template
        /// </summary>
        public void Save(dsCERTIFICATE_DATA.CERTIFICATE_DATARow drCERTIFICATE_DATA)
        {
            _adapter.Update(drCERTIFICATE_DATA);
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