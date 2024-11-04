using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsCERTIFICATE_STATUS_ISSUETableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterCERTIFICATE_STATUS_ISSUE : SQLBase, IDisposable
    {
        private CERTIFICATE_STATUS_ISSUETableAdapter _adapter;

        public AdapterCERTIFICATE_STATUS_ISSUE()
            : base()
        {
            _adapter = new CERTIFICATE_STATUS_ISSUETableAdapter(Variables.ConnStr);
        }

        public AdapterCERTIFICATE_STATUS_ISSUE(string connStr)
            : base(connStr)
        {
            _adapter = new CERTIFICATE_STATUS_ISSUETableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable Get()
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS_ISSUE WHERE DELETED IS NULL";
            dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable dtCERTIFICATE_STATUS_ISSUE = new dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS_ISSUE);

            if (dtCERTIFICATE_STATUS_ISSUE.Rows.Count > 0)
                return dtCERTIFICATE_STATUS_ISSUE;
            else
                return null;
        }


        /// <summary>
        /// Get the latest ITR status issue by ITR status guid
        /// </summary>
        public dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUERow GetLatestBy(Guid certificateStatusGuid)
        {
            string sql = "SELECT TOP 1 * FROM CERTIFICATE_STATUS_ISSUE WHERE CERTIFICATE_STATUS_GUID = '" + certificateStatusGuid + "'";
            sql += " ORDER BY SEQUENCE_NUMBER";

            dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable dtCERTIFICATE_STATUS_ISSUE = new dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS_ISSUE);

            if (dtCERTIFICATE_STATUS_ISSUE.Rows.Count > 0)
                return dtCERTIFICATE_STATUS_ISSUE[0];
            else
                return null;
        }

        /// <summary>
        /// Get deleted prefill for purging
        /// </summary>
        public dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable Get_Deleted(string discipline)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS_ISSUE WHERE DELETED IS NOT NULL AND DISCIPLINE = '" + discipline + "'";
            dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable dtCERTIFICATE_STATUS_ISSUE = new dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS_ISSUE);

            if (dtCERTIFICATE_STATUS_ISSUE.Rows.Count > 0)
                return dtCERTIFICATE_STATUS_ISSUE;
            else
                return null;
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable GetIncludeDeleted()
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS_ISSUE";
            dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable dtCERTIFICATE_STATUS_ISSUE = new dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS_ISSUE);

            if (dtCERTIFICATE_STATUS_ISSUE.Rows.Count > 0)
                return dtCERTIFICATE_STATUS_ISSUE;
            else
                return null;
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUERow GetIncludeDeletedBy(Guid prefillGuid)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS_ISSUE WHERE GUID = '" + prefillGuid + "'";
            dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable dtCERTIFICATE_STATUS_ISSUE = new dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS_ISSUE);

            if (dtCERTIFICATE_STATUS_ISSUE.Rows.Count > 0)
                return dtCERTIFICATE_STATUS_ISSUE[0];
            else
                return null;
        }

        /// <summary>
        /// Get header by discipline
        /// </summary>
        public dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable GetByDiscipline(string discipline)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS_ISSUE WHERE DISCIPLINE = '" + discipline + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable dtCERTIFICATE_STATUS_ISSUE = new dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS_ISSUE);

            if (dtCERTIFICATE_STATUS_ISSUE.Rows.Count > 0)
                return dtCERTIFICATE_STATUS_ISSUE;
            else
                return null;
        }

        /// <summary>
        /// Get ITR status issue by ITR status guid
        /// </summary>
        public dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable GetBy(Guid certificateStatusGuid)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS_ISSUE WHERE CERTIFICATE_STATUS_GUID = '" + certificateStatusGuid + "'";
            sql += " ORDER BY SEQUENCE_NUMBER";

            dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable dtCERTIFICATE_STATUS_ISSUE = new dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS_ISSUE);

            if (dtCERTIFICATE_STATUS_ISSUE.Rows.Count > 0)
                return dtCERTIFICATE_STATUS_ISSUE;
            else
                return null;
        }

        /// <summary>
        /// Get header by name
        /// </summary>
        public dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUERow GetBy(string name)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS_ISSUE WHERE NAME = '" + name + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable dtCERTIFICATE_STATUS_ISSUE = new dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_STATUS_ISSUE);

            if (dtCERTIFICATE_STATUS_ISSUE.Rows.Count > 0)
                return dtCERTIFICATE_STATUS_ISSUE[0];
            else
                return null;
        }

        /// <summary>
        /// Add a comment and with increased sequence number
        /// </summary>
        public void AddComments(Guid certificateStatusGuid, string comments, bool rejection)
        {
            string sql = "SELECT * FROM CERTIFICATE_STATUS_ISSUE WHERE CERTIFICATE_STATUS_GUID = '" + certificateStatusGuid + "'";
            sql += " ORDER BY SEQUENCE_NUMBER";

            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCertificateIssue = new dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable();
            ExecuteQuery(sql, dtCertificateIssue);

            int Sequence = 1;
            if (dtCertificateIssue.Rows.Count > 0)
            {
                Sequence = dtCertificateIssue.Rows.Count + 1;
            }

            dsCERTIFICATE_STATUS_ISSUE dsCERTIFICATE_STATUS_ISSUE = new dsCERTIFICATE_STATUS_ISSUE();
            dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUERow drNewCERTIFICATE_STATUS_ISSUE = dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUE.NewCERTIFICATE_STATUS_ISSUERow();
            drNewCERTIFICATE_STATUS_ISSUE.GUID = Guid.NewGuid();
            drNewCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_GUID = certificateStatusGuid;
            drNewCERTIFICATE_STATUS_ISSUE.SEQUENCE_NUMBER = Sequence;
            drNewCERTIFICATE_STATUS_ISSUE.COMMENTS = comments == null ? string.Empty : comments;
            drNewCERTIFICATE_STATUS_ISSUE.COMMENTS_READ = false;
            drNewCERTIFICATE_STATUS_ISSUE.REJECTION = rejection;

            drNewCERTIFICATE_STATUS_ISSUE.CREATED = DateTime.Now;
            drNewCERTIFICATE_STATUS_ISSUE.CREATEDBY = System_Environment.GetUser().GUID;
            dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUE.AddCERTIFICATE_STATUS_ISSUERow(drNewCERTIFICATE_STATUS_ISSUE);
            Save(drNewCERTIFICATE_STATUS_ISSUE);
        }

        /// <summary>
        /// Update multiple templates
        /// </summary>
        public void Save(dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable dtCERTIFICATE_STATUS_ISSUE)
        {
            _adapter.Update(dtCERTIFICATE_STATUS_ISSUE);
        }

        /// <summary>
        /// Update one template
        /// </summary>
        public void Save(dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUERow drCERTIFICATE_STATUS_ISSUE)
        {
            _adapter.Update(drCERTIFICATE_STATUS_ISSUE);
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