using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsCERTIFICATE_STATUSTableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterCERTIFICATE_STATUS : SQLBase, IDisposable
    {
        private CERTIFICATE_STATUSTableAdapter _adapter;

        public AdapterCERTIFICATE_STATUS()
            : base()
        {
            _adapter = new CERTIFICATE_STATUSTableAdapter(Variables.ConnStr);
        }

        public AdapterCERTIFICATE_STATUS(string connStr)
            : base(connStr)
        {
            _adapter = new CERTIFICATE_STATUSTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable Get()
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS WHERE DELETED IS NULL";
            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
                return dtCERTIFICATE_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get Certificate status by guid including deleted
        /// </summary>
        public dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable GetAll(Guid certificateGuid)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS WHERE CERTIFICATE_MAIN_GUID = '" + certificateGuid + "'";
            sql += " AND DELETED IS NULL ORDER BY SEQUENCE_NUMBER";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
                return dtCERTIFICATE_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get the last 2 certificate sequence for comparing status progression
        /// </summary>
        public dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable GetStatusByCertificate(Guid certificateGuid, int count)
        {
            string sql = "SELECT TOP " + count + " * FROM CERTIFICATE_STATUS ";
            sql += "JOIN CERTIFICATE_MAIN ON CERTIFICATE_MAIN.GUID = CERTIFICATE_STATUS.CERTIFICATE_MAIN_GUID ";
            sql += "WHERE CERTIFICATE_MAIN.DELETED IS NULL AND CERTIFICATE_STATUS.CERTIFICATE_MAIN_GUID = '" + certificateGuid + "' AND CERTIFICATE_STATUS.DELETED IS NULL ORDER BY CERTIFICATE_STATUS.SEQUENCE_NUMBER DESC";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
                return dtCERTIFICATE_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get latest certificate status by guid
        /// </summary>
        public dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow GetLatestStatusBy(Guid certificateGuid)
        {
            string sql = "SELECT TOP 1 * FROM CERTIFICATE_STATUS WHERE CERTIFICATE_MAIN_GUID = '" + certificateGuid + "' AND DELETED IS NULL ORDER BY SEQUENCE_NUMBER DESC";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
                return dtCERTIFICATE_STATUS[0];
            else
                return null;
        }

        /// <summary>
        /// Get certificate status by guid including deleted
        /// </summary>
        public dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable GetAllExcludeRejected(Guid certificateGuid)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS cStatus WHERE cStatus.CERTIFICATE_MAIN_GUID = '" + certificateGuid + "'";
            sql += " AND cStatus.DELETED IS NULL ORDER BY cStatus.SEQUENCE_NUMBER";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
                return dtCERTIFICATE_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get deleted prefill for purging
        /// </summary>
        public dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable Get_Deleted(string discipline)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS WHERE DELETED IS NOT NULL AND DISCIPLINE = '" + discipline + "'";
            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
                return dtCERTIFICATE_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable GetIncludeDeleted()
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS";
            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
                return dtCERTIFICATE_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow GetIncludeDeletedBy(Guid prefillGuid)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS WHERE GUID = '" + prefillGuid + "'";
            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
                return dtCERTIFICATE_STATUS[0];
            else
                return null;
        }

        /// <summary>
        /// Get header by discipline
        /// </summary>
        public dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable GetByDiscipline(string discipline)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS WHERE DISCIPLINE = '" + discipline + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
                return dtCERTIFICATE_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get ITR status by guid
        /// </summary>
        public dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow GetRowBy(Guid certificateGuid)
        {
            string sql = "SELECT TOP 1 * FROM CERTIFICATE_STATUS WHERE CERTIFICATE_MAIN_GUID = '" + certificateGuid + "' AND DELETED IS NULL ORDER BY SEQUENCE_NUMBER DESC";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
                return dtCERTIFICATE_STATUS[0];
            else
                return null;
        }

        /// <summary>
        /// Get header by guid
        /// </summary>
        public dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow GetBy(Guid headerGuid)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS WHERE GUID = '" + headerGuid + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
                return dtCERTIFICATE_STATUS[0];
            else
                return null;
        }

        /// <summary>
        /// Get header by name
        /// </summary>
        public dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow GetBy(string name)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS WHERE NAME = '" + name + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
                return dtCERTIFICATE_STATUS[0];
            else
                return null;
        }

        /// <summary>
        /// Remove a particular header by GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtHeader = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtHeader);

            if (dtHeader != null)
            {
                dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drHeader = dtHeader[0];
                drHeader.DELETED = DateTime.Now;
                drHeader.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drHeader);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Change the ITR status and delete previous statuses
        /// </summary>
        public void ChangeStatus(Guid certificateGuid, int statusNumber)
        {
            dsCERTIFICATE_STATUS dsCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS();
            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drNewCERTIFICATE_STATUS;

            string sql = "SELECT TOP 1 * FROM CERTIFICATE_STATUS WHERE CERTIFICATE_MAIN_GUID = '" + certificateGuid + "' AND DELETED IS NULL ORDER BY SEQUENCE_NUMBER DESC";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            int Sequence = 1;
            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
            {
                dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drCERTIFICATE_STATUS = ((dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow)dtCERTIFICATE_STATUS.Rows[0]);
                //don't update if the status number is the same
                if ((int)drCERTIFICATE_STATUS.STATUS_NUMBER == statusNumber)
                    return;

                sql = "SELECT * FROM CERTIFICATE_STATUS WHERE CERTIFICATE_MAIN_GUID = '" + certificateGuid + "' AND DELETED IS NULL"; //get the sequence count
                dsITR_STATUS.ITR_STATUSDataTable dt = new dsITR_STATUS.ITR_STATUSDataTable();
                ExecuteQuery(sql, dt);
                Sequence = dt.Rows.Count + 1;
            }

            drNewCERTIFICATE_STATUS = dsCERTIFICATE_STATUS.CERTIFICATE_STATUS.NewCERTIFICATE_STATUSRow();
            drNewCERTIFICATE_STATUS.GUID = Guid.NewGuid();
            drNewCERTIFICATE_STATUS.CERTIFICATE_MAIN_GUID = certificateGuid;
            drNewCERTIFICATE_STATUS.SEQUENCE_NUMBER = Sequence;
            drNewCERTIFICATE_STATUS.STATUS_NUMBER = statusNumber;
            drNewCERTIFICATE_STATUS.CREATED = DateTime.Now;
            drNewCERTIFICATE_STATUS.CREATEDBY = System_Environment.GetUser().GUID;
            dsCERTIFICATE_STATUS.CERTIFICATE_STATUS.AddCERTIFICATE_STATUSRow(drNewCERTIFICATE_STATUS);
            Save(drNewCERTIFICATE_STATUS);
        }

        /// <summary>
        /// Change the ITR status and delete previous statuses
        /// </summary>
        public void ChangeStatus(Guid certificateGuid, bool increment, Guid certificateStatusGuid)
        {
            dsCERTIFICATE_STATUS dsCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS();
            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drNewCERTIFICATE_STATUS;

            string sql = "SELECT TOP 1 * FROM CERTIFICATE_STATUS WHERE CERTIFICATE_MAIN_GUID = '" + certificateGuid + "' AND DELETED IS NULL ORDER BY SEQUENCE_NUMBER DESC";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS);

            int CurrentStatus = 0;
            int Sequence = 1;

            if (dtCERTIFICATE_STATUS.Rows.Count > 0)
            {
                //stores the current status number
                CurrentStatus = (int)dtCERTIFICATE_STATUS[0].STATUS_NUMBER;
                if (increment)
                    CurrentStatus += 1;
                else
                {
                    if(CurrentStatus > 0)
                        CurrentStatus -= 1;
                }

                sql = "SELECT * FROM CERTIFICATE_STATUS WHERE CERTIFICATE_MAIN_GUID = '" + certificateGuid + "' AND DELETED IS NULL"; //get the sequence count
                dsITR_STATUS.ITR_STATUSDataTable dt = new dsITR_STATUS.ITR_STATUSDataTable();
                ExecuteQuery(sql, dt);
                Sequence = dt.Rows.Count + 1;
            }
            else
            {
                if (increment)
                    CurrentStatus = 1;
                else
                    CurrentStatus = 0;
            }

            drNewCERTIFICATE_STATUS = dsCERTIFICATE_STATUS.CERTIFICATE_STATUS.NewCERTIFICATE_STATUSRow();
            drNewCERTIFICATE_STATUS.GUID = certificateStatusGuid;
            drNewCERTIFICATE_STATUS.CERTIFICATE_MAIN_GUID = certificateGuid;
            drNewCERTIFICATE_STATUS.SEQUENCE_NUMBER = Sequence;
            drNewCERTIFICATE_STATUS.STATUS_NUMBER = CurrentStatus;
            drNewCERTIFICATE_STATUS.CREATED = DateTime.Now;
            drNewCERTIFICATE_STATUS.CREATEDBY = System_Environment.GetUser().GUID;
            dsCERTIFICATE_STATUS.CERTIFICATE_STATUS.AddCERTIFICATE_STATUSRow(drNewCERTIFICATE_STATUS);
            Save(drNewCERTIFICATE_STATUS);
        }

        /// <summary>
        /// Remove a particular header by GUID
        /// </summary>
        public int RemoveWithExclusionBy(string prefillName, Guid excludedGuid)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS WHERE NAME = '" + prefillName + "'";
            sql += " AND GUID IS NOT '" + excludedGuid + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtPrefill = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtPrefill);

            int removeCount = 0;
            if (dtPrefill != null)
            {
                foreach (dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drPrefill in dtPrefill.Rows)
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
        public void Save(dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS)
        {
            _adapter.Update(dtCERTIFICATE_STATUS);
        }

        /// <summary>
        /// Update one template
        /// </summary>
        public void Save(dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drCERTIFICATE_STATUS)
        {
            _adapter.Update(drCERTIFICATE_STATUS);
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