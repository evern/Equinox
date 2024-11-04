using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsPUNCHLIST_STATUSTableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterPUNCHLIST_STATUS : SQLBase, IDisposable
    {
        private PUNCHLIST_STATUSTableAdapter _adapter;

        public AdapterPUNCHLIST_STATUS()
            : base()
        {
            _adapter = new PUNCHLIST_STATUSTableAdapter(Variables.ConnStr);
        }

        public AdapterPUNCHLIST_STATUS(string connStr)
            : base(connStr)
        {
            _adapter = new PUNCHLIST_STATUSTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        public dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable GetBy(Guid punchlistGuid)
        {
            string sql = "SELECT * FROM PUNCHLIST_STATUS WHERE PUNCHLIST_MAIN_GUID = '" + punchlistGuid + "'";
            sql += " AND DELETED IS NULL ORDER BY SEQUENCE_NUMBER ASC";

            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPunchlistStatus = new dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable();
            ExecuteQuery(sql, dtPunchlistStatus);

            if (dtPunchlistStatus.Rows.Count > 0)
                return dtPunchlistStatus;
            else
                return null;
        }

        /// <summary>
        /// Get all deleted punchlist status for purging
        /// </summary>
        public dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable Get_Deleted(Guid projectGuid)
        {
            string sql = "SELECT pstatus.* FROM TAG tag JOIN PUNCHLIST_MAIN punchlist ON (punchlist.TAG_GUID = tag.GUID)";
            sql += " LEFT JOIN PUNCHLIST_STATUS pstatus ON (pstatus.PUNCHLIST_MAIN_GUID = punchlist.GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE proj.GUID = '" + projectGuid.ToString() + "'";
            sql += " AND pstatus.DELETED IS NOT NULL";

            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPunchlistStatus = new dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable();
            ExecuteQuery(sql, dtPunchlistStatus);

            if (dtPunchlistStatus.Rows.Count > 0)
                return dtPunchlistStatus;
            else
                return null;
        }

        /// <summary>
        /// Get Punchlist Status by Guid
        /// </summary>
        public dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow GetRowBy(Guid punchlistGuid)
        {
            string sql = "SELECT TOP 1 * FROM PUNCHLIST_STATUS WHERE PUNCHLIST_MAIN_GUID = '" + punchlistGuid + "'";
            sql += " AND DELETED IS NULL ORDER by SEQUENCE_NUMBER DESC";

            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPunchlistStatus = new dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable();
            ExecuteQuery(sql, dtPunchlistStatus);

            if (dtPunchlistStatus.Rows.Count > 0)
                return dtPunchlistStatus[0];
            else
                return null;
        }

        //Changes 10-DEC-2014 Allow same tag number to be added per project
        /// <summary>
        /// Grade the Punchlist status by Tag number
        /// </summary>
        public int GetStatusByTag(string tagNumber, string title, Guid projectGuid)
        {
            string sql = "SELECT TOP 1 pstatus.STATUS_NUMBER FROM TAG tag JOIN PUNCHLIST_MAIN punchlist ON (punchlist.TAG_GUID = tag.GUID)";
            sql += " LEFT JOIN PUNCHLIST_STATUS pstatus ON (pstatus.PUNCHLIST_MAIN_GUID = punchlist.GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE tag.NUMBER = '" + tagNumber + "' AND punchlist.TITLE = '" + title + "'";
            sql += " AND proj.GUID = '" + projectGuid + "'";
            sql += " AND sch.DELETED IS NULL AND proj.DELETED IS NULL";
            sql += " AND tag.DELETED IS NULL AND punchlist.DELETED IS NULL AND pstatus.DELETED IS NULL ORDER by pstatus.SEQUENCE_NUMBER DESC";

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

        //Changes 12-DEC-2012 punchlist must be validated by project
        /// <summary>
        /// Grade the Punchlist status by WBS name
        /// </summary>
        public int GetStatusByWBS(string wbsName, string punchlistTitle, Guid projectGuid)
        {
            string sql = "SELECT TOP 1 pstatus.STATUS_NUMBER FROM WBS wbs JOIN PUNCHLIST_MAIN punchlist ON (punchlist.WBS_GUID = wbs.GUID)";
            sql += " LEFT JOIN PUNCHLIST_STATUS pstatus ON (pstatus.PUNCHLIST_MAIN_GUID = punchlist.GUID)";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE wbs.NAME = '" + wbsName + "' AND punchlist.TITLE = '" + punchlistTitle + "'";
            sql += " AND proj.GUID = '" + projectGuid + "'";
            sql += " AND sch.DELETED IS NULL AND proj.DELETED IS NULL";
            sql += " AND wbs.DELETED IS NULL AND punchlist.DELETED IS NULL AND pstatus.DELETED IS NULL ORDER BY pstatus.SEQUENCE_NUMBER DESC";

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
        /// Get single punchlist status excluding rejected
        /// </summary>
        public dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow GetExcludeRejected(Guid punchlistGuid)
        {
            string sql = "SELECT TOP 1 * FROM PUNCHLIST_STATUS pstatus LEFT JOIN PUNCHLIST_STATUS_ISSUE issue ON (issue.PUNCHLIST_STATUS_GUID = pstatus.GUID)";
            sql += " WHERE pstatus.PUNCHLIST_MAIN_GUID = '" + punchlistGuid + "'";
            sql += " AND pstatus.DELETED IS NULL AND (issue.REJECTION IS NULL OR issue.REJECTION < 1) ORDER BY pstatus.SEQUENCE_NUMBER DESC";

            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPUNCHLIST_STATUS = new dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_STATUS);

            if (dtPUNCHLIST_STATUS.Rows.Count > 0)
                return dtPUNCHLIST_STATUS[0];
            else
                return null;
        }

        /// <summary>
        /// Get punchlist status by guid including deleted
        /// </summary>
        public dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable GetAll(Guid punchlistGuid)
        {
            string sql = "SELECT * FROM PUNCHLIST_STATUS WHERE PUNCHLIST_MAIN_GUID = '" + punchlistGuid + "'";
            sql += "AND DELETED IS NULL ORDER BY SEQUENCE_NUMBER";

            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPUNCHLIST_STATUS = new dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable();
            ExecuteQuery(sql, dtPUNCHLIST_STATUS);

            if (dtPUNCHLIST_STATUS.Rows.Count > 0)
                return dtPUNCHLIST_STATUS;
            else
                return null;
        }

        /// <summary>
        /// Get status by sequence number
        /// </summary>
        public dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow GetSequence(Guid punchlistGuid, int Sequence)
        {
            string sql = "SELECT * FROM PUNCHLIST_STATUS WHERE PUNCHLIST_MAIN_GUID = '" + punchlistGuid + "' ";
            sql += " AND DELETED IS NULL AND SEQUENCE_NUMBER = '" + Sequence + "'";

            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPunchlistStatus = new dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable();
            ExecuteQuery(sql, dtPunchlistStatus);

            if (dtPunchlistStatus.Rows.Count > 0)
                return dtPunchlistStatus[0];
            else
                return null;
        }

        /// <summary>
        /// Change the Punchlist status and delete previous statuses
        /// </summary>
        public void ChangeStatus(Guid punchlistGuid, Punchlist_Status? applicationPunchlistStatus, bool increment, Guid punchlistStatusGuid)
        {
            dsPUNCHLIST_STATUS dsPunchlistStatus = new dsPUNCHLIST_STATUS();
            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow drNewPunchlistStatus;

            string sql = "SELECT TOP 1 * FROM PUNCHLIST_STATUS WHERE PUNCHLIST_MAIN_GUID = '" + punchlistGuid + "' AND DELETED IS NULL ORDER BY SEQUENCE_NUMBER DESC";

            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPunchlistStatus = new dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable();
            ExecuteQuery(sql, dtPunchlistStatus);

            int dbPunchlistStatus = 0;
            int Sequence = 1;

            if (dtPunchlistStatus.Rows.Count > 0)
            {
                //stores the current status number
                dbPunchlistStatus = (int)dtPunchlistStatus[0].STATUS_NUMBER;

                //when database has a status but application does not
                if (applicationPunchlistStatus == null || (int)applicationPunchlistStatus == -1)
                    applicationPunchlistStatus = (Punchlist_Status)dbPunchlistStatus;

                if (increment)
                {
                    //contingency for concurrency issue to prevent punchlist going above closed status
                    if ((int)applicationPunchlistStatus == (int)Punchlist_Status.Closed)
                        return;

                    //skip approved and go straight to completed
                    if ((int)applicationPunchlistStatus == 1)
                        applicationPunchlistStatus += 2;
                    else
                        applicationPunchlistStatus += 1;

                    //if (CurrentStatus == 0)
                    //    CurrentStatus += 1;
                    //else
                    //    CurrentStatus += 1;
                }
                else
                {
                    //skip approved stage during rejection and go straight to inspected
                    if ((int)applicationPunchlistStatus == 3)
                        applicationPunchlistStatus -= 2;
                    else
                        applicationPunchlistStatus -= 1;
                }

                if ((int)applicationPunchlistStatus > (int)Punchlist_Status.Closed)
                    applicationPunchlistStatus = Punchlist_Status.Closed;

                sql = "SELECT * FROM PUNCHLIST_STATUS WHERE PUNCHLIST_MAIN_GUID = '" + punchlistGuid + "' AND DELETED IS NULL"; //get the count
                dsITR_STATUS.ITR_STATUSDataTable dt = new dsITR_STATUS.ITR_STATUSDataTable();
                ExecuteQuery(sql, dt);
                Sequence = dt.Rows.Count + 1;
            }
            else if (applicationPunchlistStatus == null || (int)applicationPunchlistStatus == -1)
                applicationPunchlistStatus = 0;

            drNewPunchlistStatus = dsPunchlistStatus.PUNCHLIST_STATUS.NewPUNCHLIST_STATUSRow();
            drNewPunchlistStatus.GUID = punchlistStatusGuid;
            drNewPunchlistStatus.PUNCHLIST_MAIN_GUID = punchlistGuid;
            drNewPunchlistStatus.SEQUENCE_NUMBER = Sequence;
            drNewPunchlistStatus.STATUS_NUMBER = (int)applicationPunchlistStatus;
            drNewPunchlistStatus.CREATED = DateTime.Now;
            drNewPunchlistStatus.CREATEDBY = System_Environment.GetUser().GUID;
            dsPunchlistStatus.PUNCHLIST_STATUS.AddPUNCHLIST_STATUSRow(drNewPunchlistStatus);
            Save(drNewPunchlistStatus);
        }

        /// <summary>
        /// Get the last 2 punchlist sequence for comparing status progression
        /// </summary>
        public dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable GetLastTwoSequenceByPunchlist(Guid punchlistGuid)
        {
            string sql = "SELECT * FROM PUNCHLIST_STATUS a ";
            sql += "WHERE (SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) ";
            sql += "FROM PUNCHLIST_STATUS b WHERE b.PUNCHLIST_MAIN_GUID = a.PUNCHLIST_MAIN_GUID AND b.DELETED IS NULL) ";
            sql += "OR SEQUENCE_NUMBER = (SELECT MAX(SEQUENCE_NUMBER) - 1 FROM PUNCHLIST_STATUS c ";
            sql += "WHERE c.PUNCHLIST_MAIN_GUID = a.PUNCHLIST_MAIN_GUID AND c.DELETED IS NULL)) ";
            sql += "AND a.PUNCHLIST_MAIN_GUID = '" + punchlistGuid + "' AND a.DELETED IS NULL ";
            sql += "ORDER BY SEQUENCE_NUMBER DESC";

            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPunchlistStatus = new dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable();
            ExecuteQuery(sql, dtPunchlistStatus);

            if (dtPunchlistStatus.Rows.Count > 0)
                return dtPunchlistStatus;
            else
                return null;
        }

        /// <summary>
        /// Saves multiple projects
        /// </summary>
        public void Save(dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPUNCHLIST_STATUS)
        {
            _adapter.Update(dtPUNCHLIST_STATUS);
        }

        /// <summary>
        /// Saves one project
        /// </summary>
        public void Save(dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow drPUNCHLIST_STATUS)
        {
            _adapter.Update(drPUNCHLIST_STATUS);
        }

        /// <summary>
        /// Remove a particular punchlist status by punchlist main GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM PUNCHLIST_STATUS WHERE PUNCHLIST_MAIN_GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPunchlistStatus = new dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable();
            ExecuteQuery(sql, dtPunchlistStatus);

            if (dtPunchlistStatus.Rows.Count > 0)
            {
                foreach(dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow drPunchlistStatus in dtPunchlistStatus.Rows)
                {
                    drPunchlistStatus.DELETED = DateTime.Now;
                    drPunchlistStatus.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drPunchlistStatus);
                }

                return true;
            }

            return false;
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
