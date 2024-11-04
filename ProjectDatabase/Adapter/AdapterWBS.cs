using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsWBSTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterWBS : SQLBase, IDisposable
    {
        private WBSTableAdapter _adapter;

        public AdapterWBS()
            : base()
        {
            _adapter = new WBSTableAdapter(Variables.ConnStr);
        }

        public AdapterWBS(string connStr)
            : base(connStr)
        {
            _adapter = new WBSTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        /// <summary>
        /// Searches for first instance of WBS Guid
        /// </summary>
        /// <param name="parentGuid">Parent guid - can be tag guid or wbs guid</param>
        /// <returns>first instance of WBS guid</returns>
        public Guid? GetFirstWBSGuid(Guid parentGuid)
        {
            string sql = "SELECT * FROM WBS WHERE GUID = '" + parentGuid + "'";
            sql += " AND DELETED IS NULL";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS[0].GUID;
            else
            {
                string sqlTag = "SELECT * FROM TAG WHERE GUID = '" + parentGuid + "'";
                sqlTag += " AND DELETED IS NULL";

                dsTAG.TAGDataTable dtTag = new dsTAG.TAGDataTable();
                ExecuteQuery(sqlTag, dtTag);

                //should have something here, if not return null, if yes continue to find WBS parent
                if (dtTag.Rows.Count > 0)
                    return GetFirstWBSGuid(dtTag[0].PARENTGUID);
                else
                    return null;
            }
        }

        /// <summary>
        /// Get a particular wbs by GUID
        /// </summary>
        public dsWBS.WBSRow GetBy(Guid wbsGuid)
        {
            string sql = "SELECT * FROM WBS WHERE GUID = '" + wbsGuid + "'";
            sql += " AND DELETED IS NULL";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS[0];
            else
                return null;
        }

        /// <summary>
        /// Get a particular WBS by name
        /// </summary>
        public dsWBS.WBSRow GetBy(string wbsName)
        {
            string sql = "SELECT * FROM WBS WHERE NAME = '" + wbsName + "'";
            sql += " AND DELETED IS NULL";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS[0];
            else
                return null;
        }

        //Fix 12-DEC-2014: WBS is unique to project
        /// <summary>
        /// Get a particular wbs by wbs name and schedule Guid
        /// </summary>
        public dsWBS.WBSRow GetBy(string wbsName, Guid projectGuid)
        {
            string sql = "SELECT wbs.* FROM WBS wbs";
            sql += " LEFT JOIN SCHEDULE sch ON (wbs.SCHEDULEGUID = sch.GUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE wbs.NAME = '" + wbsName + "'";
            sql += " AND proj.GUID = '" + projectGuid + "'";
            sql += " AND wbs.DELETED IS NULL AND sch.DELETED IS NULL";
            sql += " AND proj.DELETED IS NULL";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS[0];
            else
                return null;
        }

        /// <summary>
        /// Get a particular wbs by wbs name and schedule Guid including deletes
        /// </summary>
        public dsWBS.WBSRow GetIncludeDeletedBy(Guid wbsGuid)
        {
            string sql = "SELECT * FROM WBS WHERE GUID = '" + wbsGuid + "'";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS[0];
            else
                return null;
        }

        /// <summary>
        /// Get wbss by project and discipline
        /// </summary>
        public dsWBS.WBSRow GetByProjectDiscipline(Guid projectGuid, string discipline, string wbsName)
        {
            string sql = "SELECT wbs.* FROM WBS wbs JOIN SCHEDULE sch ON (wbs.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND sch.DISCIPLINE = '" + discipline + "'";
            sql += " AND wbs.NAME = '" + wbsName + "')";
            sql += " AND wbs.DELETED IS NULL ";
            sql += " AND sch.DELETED IS NULL ";
            sql += " ORDER BY wbs.NAME";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS[0];
            else
                return null;
        }

        /// <summary>
        /// Get wbss by project and discipline
        /// </summary>
        public dsWBS.WBSRow GetByProjectDisciplineIncludingDeleted(Guid projectGuid, string discipline, Guid wbsGuid)
        {
            string sql = "SELECT wbs.* FROM WBS wbs JOIN SCHEDULE sch ON (wbs.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND sch.DISCIPLINE = '" + discipline + "'";
            sql += " AND wbs.GUID = '" + wbsGuid + "')";
            sql += " AND sch.DELETED IS NULL ";
            sql += " AND wbs.DELETED IS NULL ";
            sql += " ORDER BY wbs.NAME";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS[0];
            else
                return null;
        }

        /// <summary>
        /// Get wbss by project and discipline
        /// </summary>
        public dsWBS.WBSRow GetByProjectDiscipline(Guid projectGuid, string discipline, Guid wbsGuid)
        {
            string sql = "SELECT wbs.* FROM WBS wbs JOIN SCHEDULE sch ON (wbs.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND sch.DISCIPLINE = '" + discipline + "')";
            sql += " AND wbs.GUID = '" + wbsGuid + "')";
            sql += " AND sch.DELETED IS NULL ";
            sql += " AND wbs.DELETED IS NULL ";
            sql += " ORDER BY wbs.NAME";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS[0];
            else
                return null;
        }

        public dsWBS.WBSDataTable GetByProjectDisciplineIncludeDeleted(Guid projectGuid, string discipline)
        {
            string sql = "SELECT * FROM WBS JOIN SCHEDULE ON WBS.SCHEDULEGUID = SCHEDULE.GUID WHERE SCHEDULE.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND SCHEDULE.DISCIPLINE = '" + discipline + "'";
            sql += " ORDER BY WBS.NAME, WBS.DELETED";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS;
            else
                return null;
        }

        /// <summary>
        /// Get a WBS by schedule GUID
        /// </summary>
        public dsWBS.WBSDisciplineDataTable GetByProjectIncludeDiscipline(Guid projectGuid)
        {
            string sql = "SELECT WBS.*, SCHEDULE.DISCIPLINE FROM WBS JOIN SCHEDULE ON (WBS.SCHEDULEGUID = SCHEDULE.GUID) WHERE SCHEDULE.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND WBS.DELETED IS NULL AND SCHEDULE.DELETED IS NULL";
            sql += " ORDER BY WBS.NAME";

            //string sql = "SELECT * FROM WBS WHERE SCHEDULEGUID = '" + scheduleGuid + "'";
            //sql += " AND DELETED IS NULL";
            //sql += " ORDER BY NAME";

            dsWBS.WBSDisciplineDataTable dtWBS = new dsWBS.WBSDisciplineDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS;
            else
                return null;
        }

        /// <summary>
        /// Get a WBS by schedule GUID
        /// </summary>
        public dsWBS.WBSDataTable GetByProject(Guid projectGuid)
        {
            string sql = "SELECT * FROM WBS JOIN SCHEDULE ON (WBS.SCHEDULEGUID = SCHEDULE.GUID) WHERE SCHEDULE.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND WBS.DELETED IS NULL AND SCHEDULE.DELETED IS NULL";
            sql += " ORDER BY WBS.NAME";

            //string sql = "SELECT * FROM WBS WHERE SCHEDULEGUID = '" + scheduleGuid + "'";
            //sql += " AND DELETED IS NULL";
            //sql += " ORDER BY NAME";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS;
            else
                return null;
        }

        /// <summary>
        /// Get a WBS by schedule GUID
        /// </summary>
        public dsWBS.WBSRow GetByProjectParentName(Guid projectGuid, Guid parentGuid, string wbsName)
        {
            string sql = "SELECT * FROM WBS JOIN SCHEDULE ON (WBS.SCHEDULEGUID = SCHEDULE.GUID) WHERE SCHEDULE.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND WBS.NAME = '" + wbsName + "' AND PARENTGUID = '" + parentGuid + "' AND WBS.DELETED IS NULL AND SCHEDULE.DELETED IS NULL";
            sql += " ORDER BY WBS.NAME";

            //string sql = "SELECT * FROM WBS WHERE SCHEDULEGUID = '" + scheduleGuid + "'";
            //sql += " AND DELETED IS NULL";
            //sql += " ORDER BY NAME";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS[0];
            else
                return null;
        }

        /// <summary>
        /// Get a WBS by schedule GUID
        /// </summary>
        public dsWBS.WBSDataTable GetByScheduleIncludingDeleted(Guid scheduleGuid)
        {
            string sql = "SELECT * FROM WBS WHERE SCHEDULEGUID = '" + scheduleGuid + "'";
            sql += " ORDER BY NAME";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS;
            else
                return null;
        }

        /// <summary>
        /// Get a WBS by project and discipline
        /// </summary>
        public dsWBS.WBSDisciplineDataTable GetByProjectDisciplineIncludeDiscipline(Guid projectGuid, string discipline)
        {
            string sql = "SELECT wbs.*, sch.DISCIPLINE FROM WBS wbs JOIN SCHEDULE sch ON (wbs.SCHEDULEGUID = sch.GUID)";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid + "' AND sch.DISCIPLINE = '" + discipline + "')";
            sql += " AND wbs.DELETED IS NULL";
            sql += " AND sch.DELETED IS NULL";
            sql += " ORDER BY wbs.NAME";

            dsWBS.WBSDisciplineDataTable dtWBS = new dsWBS.WBSDisciplineDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS;
            else
                return null;
        }

        /// <summary>
        /// Get a WBS by project and discipline
        /// </summary>
        public dsWBS.WBSDataTable GetByProjectDiscipline(Guid projectGuid, string discipline)
        {
            string sql = "SELECT * FROM WBS WHERE GUID IN ";
            sql += "(SELECT DISTINCT(wbs.GUID) FROM WBS wbs ";
            sql += "JOIN TAG Tag ON Tag.PARENTGUID = wbs.GUID ";
            sql += "JOIN SCHEDULE sch ON (Tag.SCHEDULEGUID = sch.GUID) ";
            sql += "WHERE (sch.PROJECTGUID = '" + projectGuid + "' AND Sch.DISCIPLINE = '" + discipline + "') AND wbs.DELETED IS NULL AND sch.DELETED IS NULL) ORDER BY NAME";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS;
            else
                return null;
        }

        /// <summary>
        /// Get a WBS by project and discipline
        /// </summary>
        public dsWBS.WBSDisciplineDataTable GetByScheduleIncludeDiscipline(Guid scheduleGuid, string discipline)
        {
            string sql = "SELECT wbs.*, sch.DISCIPLINE FROM WBS wbs JOIN SCHEDULE sch ON (wbs.SCHEDULEGUID = sch.GUID)";
            sql += " WHERE (sch.GUID = '" + scheduleGuid + "' AND sch.DISCIPLINE = '" + discipline + "')";
            sql += " AND wbs.DELETED IS NULL";
            sql += " AND sch.DELETED IS NULL";
            sql += " ORDER BY wbs.NAME";

            dsWBS.WBSDisciplineDataTable dtWBS = new dsWBS.WBSDisciplineDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS;
            else
                return null;

        }

        /// <summary>
        /// Get a WBS by project and discipline
        /// </summary>
        public dsWBS.WBSDataTable GetBySchedule(Guid scheduleGuid, string discipline)
        {
            string sql = "SELECT wbs.* FROM WBS wbs JOIN SCHEDULE sch ON (wbs.SCHEDULEGUID = sch.GUID)";
            sql += " WHERE (sch.GUID = '" + scheduleGuid + "' AND sch.DISCIPLINE = '" + discipline + "')";
            sql += " AND wbs.DELETED IS NULL";
            sql += " AND sch.DELETED IS NULL";
            sql += " ORDER BY wbs.NAME";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS;
            else
                return null;

        }

        public dsWBS.WBSDataTable GetByProjectDisciplineTemplate(Guid projectGuid, string discipline, Guid templateGuid, Guid excludeWBSTagGuid, List<Guid> excludeGuids)
        {
            string sql = "SELECT Wbs.* FROM WBS Wbs JOIN SCHEDULE sch ON (Wbs.SCHEDULEGUID = sch.GUID)";
            sql += " JOIN TEMPLATE_REGISTER reg ON (reg.WBS_GUID = Wbs.GUID)";
            sql += " OUTER APPLY";
            sql += " (SELECT * FROM ITR_MAIN WHERE WBS_GUID = Wbs.GUID AND TEMPLATE_GUID = reg.TEMPLATE_GUID AND DELETED IS NULL) iTR";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid + "' AND sch.DISCIPLINE = '" + discipline + "')";
            sql += " AND reg.TEMPLATE_GUID = '" + templateGuid + "'";
            sql += " AND Wbs.GUID <> '" + excludeWBSTagGuid + "'";

            if (excludeGuids != null && excludeGuids.Count > 0)
            {
                foreach (Guid excludeGuid in excludeGuids)
                {
                    sql += " AND Wbs.GUID <> '" + excludeGuid + "'";
                }
            }
            sql += " AND Wbs.DELETED IS NULL";
            sql += " AND sch.DELETED IS NULL";
            sql += " AND reg.DELETED IS NULL";
            sql += " AND iTR.NAME IS NULL";
            sql += " ORDER BY Wbs.NAME";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS;
            else
                return null;
        }

        /// <summary>
        /// Get recursive wbs childrens
        /// </summary>
        public dsWBS.WBSDataTable GetWBSChildrens(Guid parentGuid, bool includeSelf)
        {
            string sql = "WITH RECURSIVETBL AS (SELECT a.* FROM WBS a ";
            if (includeSelf)
                sql += "WHERE GUID = '";
            else
                sql += "WHERE PARENTGUID = '";

            sql += parentGuid + "' UNION ALL ";
            sql += "SELECT a.* FROM WBS a JOIN RECURSIVETBL r ON a.PARENTGUID = r.GUID) ";
            sql += "SELECT * FROM RECURSIVETBL WHERE RECURSIVETBL.DELETED IS NULL";

            dsWBS.WBSDataTable dtWBS_MAIN = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS_MAIN);

            if (dtWBS_MAIN.Rows.Count > 0)
                return dtWBS_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Remove a particular wbs by GUID
        /// </summary>
        public bool RemoveBy(Guid wbsGuid)
        {
            string sql = "SELECT * FROM WBS WHERE GUID = '" + wbsGuid + "'";

            dsWBS.WBSDataTable dtWbs = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWbs);

            if (dtWbs != null)
            {
                dsWBS.WBSRow drWbs = dtWbs[0];
                drWbs.DELETED = DateTime.Now;
                drWbs.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drWbs);
                return true;
            }

            return false;
        }

        /// 12-DEC-2014 Fix: WBS goes by name and project guid
        /// <summary>
        /// Remove a particular wbs by GUID
        /// </summary>
        public int RemoveAllWithExclusionBy(string wbsName, Guid projectGuid, Guid excludeGuid)
        {
            string sql = "SELECT wbs.* FROM WBS wbs";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE wbs.NAME = '" + wbsName + "'";
            sql += " AND proj.GUID = '" + projectGuid + "'";
            sql += " AND wbs.GUID != '" + excludeGuid + "'";
            sql += " AND wbs.DELETED IS NULL";
            sql += " AND sch.DELETED IS NULL AND proj.DELETED IS NULL";

            dsWBS.WBSDataTable dtWbs = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWbs);

            int removeCount = 0;
            if (dtWbs != null)
            {
                foreach(dsWBS.WBSRow drWbs in dtWbs.Rows)
                {
                    drWbs.DELETED = DateTime.Now;
                    drWbs.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drWbs);
                    removeCount++;
                }
            }

            return removeCount;
        }

        /// <summary>
        /// Get all wbs
        /// </summary
        public dsWBS.WBSDataTable Get()
        {
            string sql = "SELECT * FROM WBS WHERE DELETED IS NULL";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS;
            else
                return null;
        }

        /// <summary>
        /// Get deleted wbs for purging
        /// </summary
        public dsWBS.WBSDataTable Get_Deleted(Guid projectGuid)
        {
            string sql = "SELECT wbs.* FROM WBS wbs";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = wbs.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE proj.GUID = '" + projectGuid.ToString() + "'";
            sql += " AND wbs.DELETED IS NOT NULL";

            dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
            ExecuteQuery(sql, dtWBS);

            if (dtWBS.Rows.Count > 0)
                return dtWBS;
            else
                return null;
        }

        /// <summary>
        /// Gets the project WBS structure by Area, System and Subsystem
        /// </summary>
        public List<ProjectWBS> GetProjectWBS(Guid projectGuid)
        {
            string sql = "SELECT AREA_WBS.NAME AREAWBS, AREA_WBS.DESCRIPTION AREADESCRIPTION, SYSTEM_WBS.NAME SYSTEMWBS, SYSTEM_WBS.DESCRIPTION SYSTEMDESCRIPTION, SUBSYSTEM_WBS.NAME SUBSYSTEMWBS, SUBSYSTEM_WBS.DESCRIPTION SUBSYSTEMDESCRIPTION FROM WBS AREA_WBS ";
            sql += "JOIN SCHEDULE ON SCHEDULE.GUID = AREA_WBS.SCHEDULEGUID ";
            sql += "JOIN PROJECT ON PROJECT.GUID = SCHEDULE.PROJECTGUID ";
            sql += "OUTER APPLY ";
            sql += "( ";
            sql += "SELECT SYSTEMWBS.* FROM WBS SYSTEMWBS WHERE SYSTEMWBS.PARENTGUID = AREA_WBS.GUID AND SYSTEMWBS.DELETED IS NULL";
            sql += ") SYSTEM_WBS ";
            sql += "OUTER APPLY ";
            sql += "( ";
            sql += "SELECT SUBSYSTEMWBS.* FROM WBS SUBSYSTEMWBS WHERE PARENTGUID = SYSTEM_WBS.GUID AND SUBSYSTEMWBS.DELETED IS NULL";
            sql += ") SUBSYSTEM_WBS ";
            sql += "WHERE PROJECT.GUID = '" + projectGuid + "' AND AREA_WBS.PARENTGUID = '00000000-0000-0000-0000-000000000000' ";
            sql += "AND AREA_WBS.DELETED IS NULL ";
            sql += "ORDER BY AREA_WBS.DESCRIPTION, SYSTEM_WBS.DESCRIPTION, SUBSYSTEM_WBS.DESCRIPTION";


            DataTable dtWBS = new DataTable();
            dtWBS.Columns.Add("AREAWBS", typeof(string));
            dtWBS.Columns.Add("SYSTEMWBS", typeof(string));
            dtWBS.Columns.Add("SUBSYSTEMWBS", typeof(string));

            ExecuteQuery(sql, dtWBS);

            List<ProjectWBS> projectWBSs = new List<ProjectWBS>();
            if (dtWBS.Rows.Count > 0)
            {
                foreach(DataRow drWBS in dtWBS.Rows)
                {
                    ProjectWBS projectWBS = new ProjectWBS();
                    projectWBS.Area = drWBS["AREAWBS"].ToString();
                    projectWBS.AreaDescription = drWBS["AREADESCRIPTION"].ToString();
                    projectWBS.System = drWBS["SYSTEMWBS"].ToString();
                    projectWBS.SystemDescription = drWBS["SYSTEMDESCRIPTION"].ToString();
                    projectWBS.Subsystem = drWBS["SUBSYSTEMWBS"].ToString();
                    projectWBS.SubsystemDescription = drWBS["SUBSYSTEMDESCRIPTION"].ToString();
                    projectWBSs.Add(projectWBS);
                }
            }

            return projectWBSs;
        }

        /// <summary>
        /// Saves multiple wbss
        /// </summary>
        public void Save(dsWBS.WBSDataTable dtWBS)
        {
            _adapter.Update(dtWBS);
        }

        /// <summary>
        /// Saves one wbs
        /// </summary>
        public void Save(dsWBS.WBSRow drWBS)
        {
            _adapter.Update(drWBS);
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
