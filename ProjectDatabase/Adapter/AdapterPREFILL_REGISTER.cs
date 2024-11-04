using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsPREFILL_REGISTERTableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterPREFILL_REGISTER : SQLBase, IDisposable
    {
        private PREFILL_REGISTERTableAdapter _adapter;

        public AdapterPREFILL_REGISTER()
            : base()
        {
            _adapter = new PREFILL_REGISTERTableAdapter(Variables.ConnStr);
        }

        public AdapterPREFILL_REGISTER(string connStr)
            : base(connStr)
        {
            _adapter = new PREFILL_REGISTERTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all assigned prefill in the system
        /// </summary>
        public dsPREFILL_REGISTER.PREFILL_REGISTERDataTable Get()
        {
            string sql = "SELECT * FROM PREFILL_REGISTER WHERE DELETED IS NULL";
            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPREFILL_REGISTER = new dsPREFILL_REGISTER.PREFILL_REGISTERDataTable();
            ExecuteQuery(sql, dtPREFILL_REGISTER);

            if (dtPREFILL_REGISTER.Rows.Count > 0)
                return dtPREFILL_REGISTER;
            else
                return null;
        }

        /// <summary>
        /// Get deleted prefill assignments in the system for purging
        /// </summary>
        public dsPREFILL_REGISTER.PREFILL_REGISTERDataTable Get_Deleted(Guid ProjectGuid)
        {
            string sql = "SELECT Register.* FROM PREFILL_REGISTER Register ";
            sql += "JOIN TAG Tag ON (TAG.GUID = Register.TAG_GUID) ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID) ";
            sql += "JOIN PROJECT Project ON (Project.GUID = Schedule.PROJECTGUID) ";
            sql += "WHERE Project.GUID = '" + ProjectGuid.ToString() + "' AND Tag.DELETED IS NOT NULL";

            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPREFILL_REGISTER = new dsPREFILL_REGISTER.PREFILL_REGISTERDataTable();
            ExecuteQuery(sql, dtPREFILL_REGISTER);

            if (dtPREFILL_REGISTER.Rows.Count > 0)
                return dtPREFILL_REGISTER;
            else
                return null;
        }

        /// <summary>
        /// Get register by guid
        /// </summary>
        public dsPREFILL_REGISTER.PREFILL_REGISTERRow GetBy(Guid registerGuid)
        {
            string sql = "SELECT * FROM PREFILL_REGISTER WHERE GUID = '" + registerGuid + "'";
            sql += " AND DELETED IS NULL";

            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPREFILL_REGISTER = new dsPREFILL_REGISTER.PREFILL_REGISTERDataTable();
            ExecuteQuery(sql, dtPREFILL_REGISTER);

            if (dtPREFILL_REGISTER.Rows.Count > 0)
                return dtPREFILL_REGISTER[0];
            else
                return null;
        }

        /// <summary>
        /// Get register by tag/wbs guid and header name
        /// </summary>
        public dsPREFILL_REGISTER.PREFILL_REGISTERRow GetBy(Guid wbsTagGuid, string headerName, PrefillType prefillType)
        {
            string sql = "SELECT * FROM PREFILL_REGISTER WHERE ";
            if (prefillType == PrefillType.WBS)
                sql += "WBS_GUID = '";
            else
                sql += "TAG_GUID = '";

            sql += wbsTagGuid + "' AND NAME = '" + headerName + "' AND DELETED IS NULL";

            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPREFILL_REGISTER = new dsPREFILL_REGISTER.PREFILL_REGISTERDataTable();
            ExecuteQuery(sql, dtPREFILL_REGISTER);

            if (dtPREFILL_REGISTER.Rows.Count > 0)
                return dtPREFILL_REGISTER[0];
            else
                return null;
        }

        /// <summary>
        /// Get register by tag/wbs guid and header name
        /// </summary>
        public dsPREFILL_REGISTER.PREFILL_REGISTERDataTable GetByWBSTag(Guid wbsTagGuid, PrefillType prefillType)
        {
            string sql = "SELECT * FROM PREFILL_REGISTER WHERE ";
            if (prefillType == PrefillType.WBS)
                sql += "WBS_GUID = '";
            else
                sql += "TAG_GUID = '";

            sql += wbsTagGuid + "' ";

            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPREFILL_REGISTER = new dsPREFILL_REGISTER.PREFILL_REGISTERDataTable();
            ExecuteQuery(sql, dtPREFILL_REGISTER);

            return dtPREFILL_REGISTER;
        }

        /// <summary>
        /// Get register by tag/wbs guid and header name
        /// </summary>
        public dsPREFILL_REGISTER.PREFILL_REGISTERDataTable GetBy(Guid wbsTagGuid, PrefillType prefillType)
        {
            string sql = "SELECT * FROM PREFILL_REGISTER WHERE ";
            if (prefillType == PrefillType.WBS)
                sql += "WBS_GUID = '";
            else
                sql += "TAG_GUID = '";

            sql += wbsTagGuid + "' AND DELETED IS NULL";

            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPREFILL_REGISTER = new dsPREFILL_REGISTER.PREFILL_REGISTERDataTable();
            ExecuteQuery(sql, dtPREFILL_REGISTER);

            if (dtPREFILL_REGISTER.Rows.Count > 0)
                return dtPREFILL_REGISTER;
            else
                return null;
        }

        /// <summary>
        /// Get register by wbs
        /// </summary>
        public dsPREFILL_REGISTER.PREFILL_REGISTERDataTable GetByWBS(Guid wbsGuid)
        {
            string sql = "SELECT * FROM PREFILL_REGISTER WHERE WBS_GUID = '" + wbsGuid + "'";
            sql += " AND DELETED IS NULL";

            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPREFILL_REGISTER = new dsPREFILL_REGISTER.PREFILL_REGISTERDataTable();
            ExecuteQuery(sql, dtPREFILL_REGISTER);

            if (dtPREFILL_REGISTER.Rows.Count > 0)
                return dtPREFILL_REGISTER;
            else
                return null;
        }

        /// <summary>
        /// Get register by tag
        /// </summary>
        public dsPREFILL_REGISTER.PREFILL_REGISTERDataTable GetByWBSTag(Guid wbsTagGuid)
        {
            string sql = "SELECT * FROM PREFILL_REGISTER WHERE (TAG_GUID = '" + wbsTagGuid + "' OR WBS_GUID = '" + wbsTagGuid + "')";
            sql += " AND DELETED IS NULL";

            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPREFILL_REGISTER = new dsPREFILL_REGISTER.PREFILL_REGISTERDataTable();
            ExecuteQuery(sql, dtPREFILL_REGISTER);

            if (dtPREFILL_REGISTER.Rows.Count > 0)
                return dtPREFILL_REGISTER;
            else
                return null;
        }

        /// <summary>
        /// Remove a particular header by GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM PREFILL_REGISTER WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtHeader = new dsPREFILL_REGISTER.PREFILL_REGISTERDataTable();
            ExecuteQuery(sql, dtHeader);

            if (dtHeader != null)
            {
                dsPREFILL_REGISTER.PREFILL_REGISTERRow drHeader = dtHeader[0];
                drHeader.DELETED = DateTime.Now;
                drHeader.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drHeader);
                return true;
            }

            return false;
        }

        public bool RemoveBy(Guid wbsTagGuid, string name)
        {
            string sql = "SELECT * FROM PREFILL_REGISTER WHERE NAME = '" + name + "'";
            sql += " AND (TAG_GUID = '" + wbsTagGuid + "' OR WBS_GUID = '" + wbsTagGuid + "')";
            sql += " AND DELETED IS NULL";

            dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtHeader = new dsPREFILL_REGISTER.PREFILL_REGISTERDataTable();
            ExecuteQuery(sql, dtHeader);

            if (dtHeader != null)
            {
                dsPREFILL_REGISTER.PREFILL_REGISTERRow drHeader = dtHeader[0];
                drHeader.DELETED = DateTime.Now;
                drHeader.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drHeader);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update multiple templates
        /// </summary>
        public void Save(dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPREFILL_REGISTER)
        {
            _adapter.Update(dtPREFILL_REGISTER);
        }

        /// <summary>
        /// Update one template
        /// </summary>
        public void Save(dsPREFILL_REGISTER.PREFILL_REGISTERRow drPREFILL_REGISTER)
        {
            _adapter.Update(drPREFILL_REGISTER);
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