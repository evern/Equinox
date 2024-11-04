using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsSCHEDULETableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterSCHEDULE : SQLBase, IDisposable
    {
        private SCHEDULETableAdapter _adapter;

        public AdapterSCHEDULE()
            : base()
        {
            _adapter = new SCHEDULETableAdapter(Variables.ConnStr);
        }

        public AdapterSCHEDULE(string connStr)
            : base(connStr)
        {
            _adapter = new SCHEDULETableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        /// <summary>
        /// Get a particular schedule by GUID
        /// </summary>
        public dsSCHEDULE.SCHEDULERow GetBy(Guid scheduleGuid)
        {
            string sql = "SELECT * FROM SCHEDULE WHERE GUID = '" + scheduleGuid + "'";
            sql += " AND DELETED IS NULL";

            dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE = new dsSCHEDULE.SCHEDULEDataTable();
            ExecuteQuery(sql, dtSCHEDULE);

            if (dtSCHEDULE.Rows.Count > 0)
                return dtSCHEDULE[0];
            else
                return null;
        }

        /// <summary>
        /// Get a particular schedule by GUID include deletes
        /// </summary>
        public dsSCHEDULE.SCHEDULERow GetIncludeDeletedBy(Guid scheduleGuid)
        {
            string sql = "SELECT * FROM SCHEDULE WHERE GUID = '" + scheduleGuid + "'";

            dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE = new dsSCHEDULE.SCHEDULEDataTable();
            ExecuteQuery(sql, dtSCHEDULE);

            if (dtSCHEDULE.Rows.Count > 0)
                return dtSCHEDULE[0];
            else
                return null;
        }

        /// <summary>
        /// Get a particular schedule by name
        /// </summary>
        public dsSCHEDULE.SCHEDULERow GetBy(string scheduleName)
        {
            string sql = "SELECT * FROM SCHEDULE WHERE NAME = '" + scheduleName + "'";
            sql += " AND DELETED IS NULL";

            dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE = new dsSCHEDULE.SCHEDULEDataTable();
            ExecuteQuery(sql, dtSCHEDULE);

            if (dtSCHEDULE.Rows.Count > 0)
                return dtSCHEDULE[0];
            else
                return null;
        }

        /// <summary>
        /// Get a particular schedule by discipline
        /// </summary>
        public dsSCHEDULE.SCHEDULEDataTable GetByProjectDiscipline(string discipline, Guid projectGuid)
        {
            string sql = "SELECT * FROM SCHEDULE WHERE DISCIPLINE = '" + discipline + "'";
            sql += " AND PROJECTGUID = '" + projectGuid + "'";
            sql += " AND DELETED IS NULL";

            dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE = new dsSCHEDULE.SCHEDULEDataTable();
            ExecuteQuery(sql, dtSCHEDULE);

            if (dtSCHEDULE.Rows.Count > 0)
                return dtSCHEDULE;
            else
                return null;
        }

        /// <summary>
        /// Get by project
        /// </summary>
        public dsSCHEDULE.SCHEDULEDataTable GetByProject(Guid projectGuid)
        {
            string sql = "SELECT * FROM SCHEDULE WHERE PROJECTGUID = '" + projectGuid + "'";
            sql += " AND DELETED IS NULL";

            dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE = new dsSCHEDULE.SCHEDULEDataTable();
            ExecuteQuery(sql, dtSCHEDULE);

            if (dtSCHEDULE.Rows.Count > 0)
                return dtSCHEDULE;
            else
                return null;
        }

        /// <summary>
        /// Get by project including deleted
        /// </summary>
        public dsSCHEDULE.SCHEDULEDataTable GetByProjectIncludeDeleted(Guid projectGuid)
        {
            string sql = "SELECT * FROM SCHEDULE WHERE PROJECTGUID = '" + projectGuid + "'";

            dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE = new dsSCHEDULE.SCHEDULEDataTable();
            ExecuteQuery(sql, dtSCHEDULE);

            if (dtSCHEDULE.Rows.Count > 0)
                return dtSCHEDULE;
            else
                return null;
        }

        /// <summary>
        /// Get a particular schedule by discipline
        /// </summary>
        public dsSCHEDULE.SCHEDULEDataTable GetByProjectDiscipline(Guid projectGuid, string discipline)
        {
            string sql = "SELECT * FROM SCHEDULE WHERE PROJECTGUID = '" + projectGuid + "'";
            sql += " AND DISCIPLINE = '" + discipline + "'";
            sql += " AND DELETED IS NULL";
            sql += " ORDER BY NAME";

            dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE = new dsSCHEDULE.SCHEDULEDataTable();
            ExecuteQuery(sql, dtSCHEDULE);

            if (dtSCHEDULE.Rows.Count > 0)
                return dtSCHEDULE;
            else
                return null;
        }

        /// <summary>
        /// Remove a particular schedule by GUID
        /// </summary>
        public bool RemoveBy(Guid scheduleGuid)
        {
            string sql = "SELECT * FROM SCHEDULE WHERE GUID = '" + scheduleGuid + "'";
            sql += " AND DELETED IS NULL";

            dsSCHEDULE.SCHEDULEDataTable dtSchedule = new dsSCHEDULE.SCHEDULEDataTable();
            ExecuteQuery(sql, dtSchedule);

            if (dtSchedule != null)
            {
                dsSCHEDULE.SCHEDULERow drSchedule = dtSchedule[0];
                drSchedule.DELETED = DateTime.Now;
                drSchedule.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drSchedule);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove a particular schedule by GUID
        /// </summary>
        public int RemoveAllWithExclusionBy(string scheduleName, Guid scheduleGuid)
        {
            string sql = "SELECT * FROM SCHEDULE WHERE NAME = '" + scheduleName + "'";
            sql += " AND GUID IS NOT LIKE '" + scheduleGuid + "'";
            sql += " AND DELETED IS NULL";

            dsSCHEDULE.SCHEDULEDataTable dtSchedule = new dsSCHEDULE.SCHEDULEDataTable();
            ExecuteQuery(sql, dtSchedule);

            int removeCount = 0;
            if (dtSchedule != null)
            {
                foreach(dsSCHEDULE.SCHEDULERow drSchedule in dtSchedule.Rows)
                {
                    drSchedule.DELETED = DateTime.Now;
                    drSchedule.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drSchedule);
                    removeCount++;
                }

            }

            return removeCount;
        }

        /// <summary>
        /// Get all schedules
        /// </summary
        public dsSCHEDULE.SCHEDULEDataTable Get()
        {
            string sql = "SELECT * FROM SCHEDULE WHERE DELETED IS NULL";

            dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE = new dsSCHEDULE.SCHEDULEDataTable();
            ExecuteQuery(sql, dtSCHEDULE);

            if (dtSCHEDULE.Rows.Count > 0)
                return dtSCHEDULE;
            else
                return null;
        }

        /// <summary>
        /// Get deleted schedules for purging
        /// </summary
        public dsSCHEDULE.SCHEDULEDataTable Get_Deleted(Guid projectGuid)
        {
            string sql = "SELECT * FROM SCHEDULE WHERE DELETED IS NOT NULL AND PROJECTGUID = '" + projectGuid.ToString() + "'";

            dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE = new dsSCHEDULE.SCHEDULEDataTable();
            ExecuteQuery(sql, dtSCHEDULE);

            if (dtSCHEDULE.Rows.Count > 0)
                return dtSCHEDULE;
            else
                return null;
        }

        /// <summary>
        /// Saves multiple schedules
        /// </summary>
        public void Save(dsSCHEDULE.SCHEDULEDataTable dtSCHEDULE)
        {
            _adapter.Update(dtSCHEDULE);
        }

        /// <summary>
        /// Saves one schedule
        /// </summary>
        public void Save(dsSCHEDULE.SCHEDULERow drSCHEDULE)
        {
            _adapter.Update(drSCHEDULE);
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
