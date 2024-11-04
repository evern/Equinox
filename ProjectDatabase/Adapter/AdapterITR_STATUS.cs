using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsITR_STATUSTableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterITR_STATUS : SQLBase, IDisposable
    {
        private ITR_STATUSTableAdapter _adapter;

        public AdapterITR_STATUS()
            : base()
        {
            _adapter = new ITR_STATUSTableAdapter(Variables.ConnStr);
        }

        public AdapterITR_STATUS(string connStr)
            : base(connStr)
        {
            _adapter = new ITR_STATUSTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all ITR status in the system
        /// </summary>
        public dsITR_STATUS.ITR_STATUSDataTable Get()
        {
            string sql = "SELECT * FROM ITR_STATUS WHERE DELETED IS NULL";
            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITR_STATUS);

            if (dtITR_STATUS.Rows.Count > 0)
                return dtITR_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get deleted ITR status in the system for purging
        /// </summary>
        public dsITR_STATUS.ITR_STATUSDataTable Get_Deleted(Guid projectGuid)
        {
            string sql = "SELECT istatus.* FROM TAG tag JOIN ITR_MAIN itr ON";
            sql += " (itr.TAG_GUID = tag.GUID) LEFT JOIN ITR_STATUS istatus ON (istatus.ITR_MAIN_GUID = itr.GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE proj.GUID = '" + projectGuid + "'";
            sql += " AND istatus.DELETED IS NOT NULL";

            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITR_STATUS);

            if (dtITR_STATUS.Rows.Count > 0)
                return dtITR_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get ITR status by guid and sequence including deleted records
        /// </summary>
        public dsITR_STATUS.ITR_STATUSRow GetSequence(Guid iTRGuid, int Sequence)
        {
            string sql = "SELECT * FROM ITR_STATUS WHERE ITR_MAIN_GUID = '" + iTRGuid + "'";
            sql += " AND SEQUENCE_NUMBER = '" + Sequence + "' AND DELETED IS NULL";

            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITR_STATUS);

            if (dtITR_STATUS.Rows.Count > 0)
                return dtITR_STATUS[0];
            else
                return null;
        }

        /// <summary>
        /// Get ITR status by guid
        /// </summary>
        public dsITR_STATUS.ITR_STATUSRow GetRowBy(Guid iTRGuid)
        {
            string sql = "SELECT TOP 1 * FROM ITR_STATUS WHERE ITR_MAIN_GUID = '" + iTRGuid + "' AND DELETED IS NULL ORDER BY SEQUENCE_NUMBER DESC";

            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITR_STATUS);

            if (dtITR_STATUS.Rows.Count > 0)
                return dtITR_STATUS[0];
            else
                return null;
        }

        /// <summary>
        /// Get the statuses by ITR Guid
        /// </summary>
        public dsITR_STATUS.ITR_STATUSDataTable GetBy(Guid certificateGuid)
        {
            string sql = "SELECT * FROM ITR_STATUS WHERE CERTIFICATE_MAIN_GUID = '" + certificateGuid + "' AND DELETED IS NULL ORDER BY SEQUENCE_NUMBER ASC";

            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITR_STATUS);

            if (dtITR_STATUS.Rows.Count > 0)
                return dtITR_STATUS;
            else
                return null;
        }

        //Changes 10-DEC-2014 Allow same tag number to be added per project
        /// <summary>
        /// Grade the ITR status by Tag number
        /// </summary>
        public int GetStatusByTag(string tagNumber, string iTRName, Guid projectGUID)
        {
            string sql = "SELECT TOP 1 istatus.STATUS_NUMBER FROM TAG tag JOIN ITR_MAIN itr ON";
            sql += " (itr.TAG_GUID = tag.GUID) LEFT JOIN ITR_STATUS istatus ON (istatus.ITR_MAIN_GUID = itr.GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE tag.NUMBER = '" + tagNumber + "' AND itr.NAME = '" + iTRName + "'";
            sql += " AND proj.GUID = '" + projectGUID + "'";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL";
            sql += " AND tag.DELETED IS NULL AND itr.DELETED IS NULL AND istatus.DELETED IS NULL ORDER BY istatus.SEQUENCE_NUMBER DESC";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0].IsNull("STATUS_NUMBER"))
                    return -1; //Saved
                else
                    return Convert.ToInt32(dt.Rows[0]["STATUS_NUMBER"]);
            }
            else
                return -2; //Doesn't Exist
        }

        //Changes 12-DEC-2014 - Validate WBS by name and projectGUID
        /// <summary>
        /// Grade the ITR status by WBS name
        /// </summary>
        public int GetStatusByWBS(string wbsName, string iTRName, Guid projectGUID)
        {
            string sql = "SELECT TOP 1 istatus.STATUS_NUMBER FROM WBS wbs JOIN ITR_MAIN itr ON";
            sql += " (itr.WBS_GUID = wbs.GUID) LEFT JOIN ITR_STATUS istatus ON (istatus.ITR_MAIN_GUID = itr.GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE wbs.NAME = '" + wbsName + "' AND itr.NAME = '" + iTRName + "'";
            sql += " AND proj.GUID = '" + projectGUID + "'";
            sql += " AND proj.DELETED IS NULL AND sch.DELETED IS NULL";
            sql += " AND wbs.DELETED IS NULL AND itr.DELETED IS NULL AND istatus.DELETED IS NULL ORDER BY istatus.SEQUENCE_NUMBER DESC";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0].IsNull("STATUS_NUMBER"))
                    return -1; //Saved
                else
                    return Convert.ToInt32(dt.Rows[0]["STATUS_NUMBER"]);
            }
            else
                return -2; //Doesn't Exist
        }

        /// <summary>
        /// Gets the ITR status by WBS/Tag
        /// </summary>
        /// <returns>status, -1 for Saved and -2 if it doesn't exists</returns>
        public int GetStatusByWBSTagName(Guid wBSTagGuid, string name)
        {
            string sql = "SELECT TOP 1 istatus.SEQUENCE_NUMBER FROM ITR_MAIN itr LEFT JOIN ITR_STATUS istatus ON";
            sql += " (itr.TAG_GUID = '" + wBSTagGuid + "' OR itr.WBS_GUID = '" + wBSTagGuid + "')";
            sql += " WHERE itr.NAME = '" + name + "' AND itr.DELETED IS NULL AND istatus.DELETED IS NULL ORDER BY SEQUENCE_NUMBER DESC";

            DataTable dt = new DataTable();
            ExecuteQuery(sql, dt);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["SEQUENCE_NUMBER"] == null)
                    return -1;
                else
                    return (int)dt.Rows[0]["SEQUENCE_NUMBER"];
            }
            else
                return -2;
        }

        /// <summary>
        /// Get the last 2 ITR sequence for comparing status progression
        /// </summary>
        public dsITR_STATUS.ITR_STATUSDataTable GetStatusByITR(Guid itrGuid, int count)
        {
            string sql = "SELECT TOP " + count + " * FROM ITR_STATUS ";
            sql += "JOIN ITR_MAIN ON ITR_MAIN.GUID = ITR_STATUS.ITR_MAIN_GUID ";
            sql += "WHERE ITR_MAIN.DELETED IS NULL AND ITR_STATUS.ITR_MAIN_GUID = '" + itrGuid + "' AND ITR_STATUS.DELETED IS NULL ORDER BY ITR_STATUS.SEQUENCE_NUMBER DESC";

            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITR_STATUS);

            if (dtITR_STATUS.Rows.Count > 0)
                return dtITR_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get the last 2 ITR sequence for comparing status progression
        /// </summary>
        public dsITR_STATUS.ITR_STATUSDataTable GetStatusByITRs(List<Guid> iTRGuids)
        {
            string sql = "SELECT ITR_STATUS.* FROM ITR_STATUS ";
            sql += "JOIN ITR_MAIN ON ITR_MAIN.GUID = ITR_STATUS.ITR_MAIN_GUID WHERE ITR_MAIN.DELETED IS NULL AND ITR_STATUS.DELETED IS NULL";

            if (iTRGuids.Count == 0)
                return null;
            else if (iTRGuids.Count == 1)
            {
                sql += " AND (ITR_MAIN_GUID = '" + iTRGuids[0] + "')";
            }
            else
            {
                for (int i = 0; i < iTRGuids.Count; i++)
                {
                    if (i == 0)
                        sql += " AND (ITR_MAIN_GUID = '" + iTRGuids[i] + "' ";
                    else if (i == iTRGuids.Count - 1)
                        sql += " OR ITR_MAIN_GUID = '" + iTRGuids[i] + "')";
                    else
                        sql += "OR ITR_MAIN_GUID = '" + iTRGuids[i] + "' ";
                }
            }

            sql += "ORDER BY SEQUENCE_NUMBER DESC";

            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITR_STATUS);

            if (dtITR_STATUS.Rows.Count > 0)
                return dtITR_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get the last 2 ITR sequence for comparing status progression
        /// </summary>
        public dsITR_STATUS.ITR_STATUSDataTable GetTagStatusByWBSs(List<Guid> wbsGuids)
        {
            string sql = "SELECT ITR_STATUS.* FROM ITR_STATUS ";
            sql += "JOIN ITR_MAIN ON ITR_MAIN.GUID = ITR_STATUS.ITR_MAIN_GUID ";
            sql += "JOIN TAG ON ITR_MAIN.TAG_GUID = TAG.GUID ";
            sql += "JOIN WBS ON WBS.GUID = TAG.PARENTGUID ";
            sql += "WHERE ITR_MAIN.DELETED IS NULL AND ITR_STATUS.DELETED IS NULL AND TAG.DELETED IS NULL AND WBS.DELETED IS NULL";

            if (wbsGuids.Count == 0)
                return null;
            else if(wbsGuids.Count == 1)
            {
                sql += " AND (WBS.GUID = '" + wbsGuids[0] + "')";

            }
            else
            {
                for (int i = 0; i < wbsGuids.Count; i++)
                {
                    if (i == 0)
                        sql += " AND (WBS.GUID = '" + wbsGuids[i] + "' ";
                    else if (i == wbsGuids.Count - 1)
                        sql += " OR WBS.GUID = '" + wbsGuids[i] + "')";
                    else
                        sql += "OR WBS.GUID = '" + wbsGuids[i] + "' ";
                }
            }

            sql += "ORDER BY SEQUENCE_NUMBER DESC";

            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITR_STATUS);

            if (dtITR_STATUS.Rows.Count > 0)
                return dtITR_STATUS;
            else
                return null;
        }


        /// <summary>
        /// Get the last 2 ITR sequence for comparing status progression
        /// </summary>
        public dsITR_STATUS.ITR_STATUSDataTable GetWBSStatusByWBSs(List<Guid> wbsGuids)
        {
            string sql = "SELECT ITR_STATUS.* FROM ITR_STATUS ";
            sql += "JOIN ITR_MAIN ON ITR_MAIN.GUID = ITR_STATUS.ITR_MAIN_GUID ";
            sql += "JOIN WBS ON WBS.GUID = ITR_MAIN.WBS_GUID ";
            sql += "WHERE ITR_MAIN.DELETED IS NULL AND ITR_STATUS.DELETED IS NULL AND WBS.DELETED IS NULL";

            if (wbsGuids.Count == 0)
                return null;
            else if (wbsGuids.Count == 1)
            {
                sql += " AND (WBS.GUID = '" + wbsGuids[0] + "')";

            }
            else
            {
                for (int i = 0; i < wbsGuids.Count; i++)
                {
                    if (i == 0)
                        sql += " AND (WBS.GUID = '" + wbsGuids[i] + "' ";
                    else if (i == wbsGuids.Count - 1)
                        sql += " OR WBS.GUID = '" + wbsGuids[i] + "')";
                    else
                        sql += "OR WBS.GUID = '" + wbsGuids[i] + "' ";
                }
            }

            sql += "ORDER BY SEQUENCE_NUMBER DESC";

            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITR_STATUS);

            if (dtITR_STATUS.Rows.Count > 0)
                return dtITR_STATUS;
            else
                return null;
        }


        /// <summary>
        /// Get ITR status by guid including deleted
        /// </summary>
        public dsITR_STATUS.ITR_STATUSDataTable GetAll(Guid itrGuid)
        {
            string sql = "SELECT * FROM ITR_STATUS WHERE ITR_MAIN_GUID = '" + itrGuid + "'";
            sql += " AND DELETED IS NULL ORDER BY SEQUENCE_NUMBER";

            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITR_STATUS);

            if (dtITR_STATUS.Rows.Count > 0)
                return dtITR_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get ITR status by guid including deleted
        /// </summary>
        public dsITR_STATUS.ITR_STATUSDataTable GetAllExcludeRejected(Guid itrGuid)
        {
            string sql = "SELECT * FROM ITR_STATUS istatus LEFT JOIN ITR_STATUS_ISSUE issue ON (issue.ITR_STATUS_GUID = istatus.GUID) WHERE istatus.ITR_MAIN_GUID = '" + itrGuid + "'";
            sql += " AND istatus.DELETED IS NULL AND (issue.REJECTION IS NULL OR issue.REJECTION < 1) ORDER BY istatus.SEQUENCE_NUMBER";

            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITR_STATUS);

            if (dtITR_STATUS.Rows.Count > 0)
                return dtITR_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Change the ITR status and delete previous statuses
        /// </summary>
        public void ChangeStatus(Guid iTRGuid, ref ITR_Status? applicationITRStatus, bool increment, Guid iTRStatusGuid)
        {
            dsITR_STATUS dsITRStatus = new dsITR_STATUS();
            dsITR_STATUS.ITR_STATUSRow drNewITRStatus;

            string sql = "SELECT TOP 1 * FROM ITR_STATUS WHERE ITR_MAIN_GUID = '" + iTRGuid + "' AND DELETED IS NULL ORDER BY SEQUENCE_NUMBER DESC";

            dsITR_STATUS.ITR_STATUSDataTable dtITRStatus = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITRStatus);

            int DbITRStatus = 0;
            int Sequence = 1;

            if (dtITRStatus.Rows.Count > 0)
            {
                //stores the current status number
                DbITRStatus = (int)dtITRStatus[0].STATUS_NUMBER;

                //when database has a status but application does not
                if (applicationITRStatus == null || (int)applicationITRStatus == -1)
                    applicationITRStatus = (ITR_Status)DbITRStatus;

                //fix an issue where status can go above closed due to concurrency
                if (DbITRStatus == (int)ITR_Status.Closed && increment)
                    return;

                if (increment)
                    applicationITRStatus += 1;
                else
                    applicationITRStatus -= 1;

                //when dbStatus is iTRStatus post changing
                if ((int)applicationITRStatus == DbITRStatus)
                    return;

                sql = "SELECT * FROM ITR_STATUS WHERE ITR_MAIN_GUID = '" + iTRGuid + "' AND DELETED IS NULL"; //get the sequence count
                dsITR_STATUS.ITR_STATUSDataTable dt = new dsITR_STATUS.ITR_STATUSDataTable();
                ExecuteQuery(sql, dt);
                Sequence = dt.Rows.Count + 1;
            }
            else if (applicationITRStatus == null || (int)applicationITRStatus == -1)
                applicationITRStatus = 0;

            drNewITRStatus = dsITRStatus.ITR_STATUS.NewITR_STATUSRow();
            drNewITRStatus.GUID = iTRStatusGuid;
            drNewITRStatus.ITR_MAIN_GUID = iTRGuid;
            drNewITRStatus.SEQUENCE_NUMBER = Sequence;
            drNewITRStatus.STATUS_NUMBER = (int)applicationITRStatus;
            drNewITRStatus.CREATED = DateTime.Now;
            drNewITRStatus.CREATEDBY = System_Environment.GetUser().GUID;
            dsITRStatus.ITR_STATUS.AddITR_STATUSRow(drNewITRStatus);
            Save(drNewITRStatus);
        }

        /// <summary>
        /// Update multiple itr status
        /// </summary>
        public void Save(dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS)
        {
            _adapter.Update(dtITR_STATUS);
        }

        /// <summary>
        /// Update one itr status
        /// </summary>
        public void Save(dsITR_STATUS.ITR_STATUSRow drITR_STATUS)
        {
            _adapter.Update(drITR_STATUS);
        }

        /// <summary>
        /// Remove a particular punchlist status by punchlist main GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM ITR_STATUS WHERE ITR_MAIN_GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsITR_STATUS.ITR_STATUSDataTable dtITRStatus = new dsITR_STATUS.ITR_STATUSDataTable();
            ExecuteQuery(sql, dtITRStatus);

            if (dtITRStatus.Rows.Count > 0)
            {
                foreach(dsITR_STATUS.ITR_STATUSRow drITRStatus in dtITRStatus.Rows)
                {
                    drITRStatus.DELETED = DateTime.Now;
                    drITRStatus.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drITRStatus);
                }

                return true;
            }

            return false;
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