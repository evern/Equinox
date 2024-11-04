using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsTAGTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterTAG : SQLBase, IDisposable
    {
        private TAGTableAdapter _adapter;

        public AdapterTAG()
            : base()
        {
            _adapter = new TAGTableAdapter(Variables.ConnStr);
        }

        public AdapterTAG(string connStr)
            : base(connStr)
        {
            _adapter = new TAGTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        /// <summary>
        /// Get a particular tag by GUID
        /// </summary>
        public dsTAG.TAGRow GetBy(Guid tagGuid)
        {
            string sql = "SELECT * FROM TAG WHERE GUID = '" + tagGuid + "'";
            sql += " AND DELETED IS NULL";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG[0];
            else
                return null;
        }

        /// <summary>
        /// Get a particular tag by tag number
        /// </summary>
        public dsTAG.TAGRow GetBy(string tagNumber)
        {
            string sql = "SELECT * FROM TAG WHERE NUMBER = '" + tagNumber + "'";
            sql += " AND DELETED IS NULL";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG[0];
            else
                return null;
        }

        //Changes 10-DEC-2014 Allow same tag number to be added per project
        /// <summary>
        /// Get tag by tag number and project
        /// </summary>
        public dsTAG.TAGRow GetBy(string tagNumber, Guid projectGuid)
        {
            string sql = "SELECT * FROM TAG as tag";
            sql += " LEFT JOIN SCHEDULE sch ON (sch.GUID = tag.SCHEDULEGUID)";
            sql += " LEFT JOIN PROJECT proj ON (proj.GUID = sch.PROJECTGUID)";
            sql += " WHERE tag.NUMBER = '" + tagNumber + "' AND proj.GUID = '" + projectGuid + "' AND tag.DELETED IS NULL AND sch.DELETED IS NULL AND proj.DELETED IS NULL";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG[0];
            else
                return null;
        }

        /// <summary>
        /// Get a particular tag by tag number
        /// </summary>
        public dsTAG.TAGRow GetIncludeDeletedBy(Guid tagGuid)
        {
            string sql = "SELECT * FROM TAG WHERE GUID = '" + tagGuid + "'";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG[0];
            else
                return null;
        }

        /// <summary>
        /// Get tags by WBS
        /// </summary>>
        public dsTAG.TAGDataTable GetByWBS(Guid wbsGuid)
        {
            string sql = "SELECT * FROM TAG WHERE WBSGUID = '" + wbsGuid + "'";
            sql += " AND DELETED IS NULL";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        public dsTAG.TAGDataTable GetByProjectDisciplineIncludeDeleted(Guid projectGuid, string discipline)
        {
            string sql = "SELECT * FROM TAG JOIN SCHEDULE ON TAG.SCHEDULEGUID = SCHEDULE.GUID WHERE SCHEDULE.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND SCHEDULE.DISCIPLINE = '" + discipline + "'";
            sql += " ORDER BY TAG.DELETED, TAG.NUMBER";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Get tags by schedule
        /// </summary>
        public dsTAG.TAGDataTable GetBySchedule(Guid scheduleGuid)
        {
            string sql = "SELECT * FROM TAG WHERE SCHEDULEGUID = '" + scheduleGuid + "'";
            sql += " AND DELETED IS NULL";
            sql += " ORDER BY NUMBER";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Get tags by schedule
        /// </summary>
        public dsTAG.TAGDataTable GetByScheduleIncludeDeleted(Guid scheduleGuid)
        {
            string sql = "SELECT * FROM TAG WHERE SCHEDULEGUID = '" + scheduleGuid + "'";
            sql += " ORDER BY NUMBER";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Get tags by project and discipline
        /// </summary>
        public dsTAG.TAGDisciplineDataTable GetByProjectDisciplineIncludeDiscipline(Guid projectGuid, string discipline)
        {
            string sql = "SELECT tag.*, sch.DISCIPLINE FROM TAG tag JOIN SCHEDULE sch ON (tag.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND sch.DISCIPLINE = '" + discipline + "')";
            sql += " AND sch.DELETED IS NULL ";
            sql += " AND tag.DELETED IS NULL ";
            sql += " ORDER BY tag.NUMBER";

            dsTAG.TAGDisciplineDataTable dtTAG = new dsTAG.TAGDisciplineDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Get tags by project and discipline
        /// </summary>
        public dsTAG.TAGDataTable GetByProjectDiscipline(Guid projectGuid, string discipline)
        {
            string sql = "SELECT tag.* FROM TAG tag JOIN SCHEDULE sch ON (tag.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND sch.DISCIPLINE = '" + discipline + "')";
            sql += " AND sch.DELETED IS NULL ";
            sql += " AND tag.DELETED IS NULL ";
            sql += " ORDER BY tag.NUMBER";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Get tags by project
        /// </summary>
        public dsTAG.TAGDataTable GetByProject(Guid projectGuid)
        {
            string sql = "SELECT tag.* FROM TAG tag JOIN SCHEDULE sch ON (tag.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE sch.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND sch.DELETED IS NULL ";
            sql += " AND tag.DELETED IS NULL ";
            sql += " ORDER BY tag.NUMBER";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Get tags by project and discipline
        /// </summary>
        public dsTAG.TAGDisciplineDataTable GetByProjectIncludeDiscipline(Guid projectGuid)
        {
            string sql = "SELECT tag.*, sch.DISCIPLINE FROM TAG tag JOIN SCHEDULE sch ON (tag.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE sch.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND sch.DELETED IS NULL ";
            sql += " AND tag.DELETED IS NULL ";
            sql += " ORDER BY tag.NUMBER";

            dsTAG.TAGDisciplineDataTable dtTAG = new dsTAG.TAGDisciplineDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Get tags by project and discipline
        /// </summary>
        public dsTAG.TAGDisciplineDataTable GetBySchedule(Guid scheduleGuid, string discipline)
        {
            string sql = "SELECT tag.*, sch.DISCIPLINE FROM TAG tag JOIN SCHEDULE sch ON (tag.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE (sch.GUID = '" + scheduleGuid + "'";
            sql += " AND sch.DISCIPLINE = '" + discipline + "')";
            sql += " AND sch.DELETED IS NULL ";
            sql += " AND tag.DELETED IS NULL ";
            sql += " ORDER BY tag.NUMBER";

            dsTAG.TAGDisciplineDataTable dtTAG = new dsTAG.TAGDisciplineDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Get tags by project and discipline
        /// </summary>
        public dsTAG.TAGRow GetByProjectDiscipline(Guid projectGuid, string discipline, string tagNumber)
        {
            string sql = "SELECT tag.* FROM TAG tag JOIN SCHEDULE sch ON (tag.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid;

            if(discipline != Variables.allDiscipline)
                sql += "' AND sch.DISCIPLINE = '" + discipline;

            sql += "')";
            sql += " AND tag.NUMBER = '" + tagNumber + "'";
            sql += " AND sch.DELETED IS NULL ";
            sql += " AND tag.DELETED IS NULL ";
            sql += " ORDER BY tag.NUMBER";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG[0];
            else
                return null;
        }

        /// <summary>
        /// Get tags by project and discipline
        /// </summary>
        public dsTAG.TAGRow GetByProjectDiscipline(Guid projectGuid, string discipline, Guid tagGuid)
        {
            string sql = "SELECT tag.* FROM TAG tag JOIN SCHEDULE sch ON (tag.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid + "'";
            sql += " AND sch.DISCIPLINE = '" + discipline + "')";
            sql += " AND tag.GUID = '" + tagGuid + "')";
            sql += " AND sch.DELETED IS NULL ";
            sql += " AND tag.DELETED IS NULL ";
            sql += " ORDER BY tag.NUMBER";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG[0];
            else
                return null;
        }

        /// <summary>
        /// Get tags by project and discipline
        /// </summary>
        public dsTAG.TAGRow GetByProjectDisciplineIncludeDeleted(Guid projectGuid, string discipline, Guid tagGuid)
        {
            string sql = "SELECT tag.* FROM TAG tag JOIN SCHEDULE sch ON (tag.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid;

            if(discipline != Variables.allDiscipline)
                sql += "' AND sch.DISCIPLINE = '" + discipline;

            sql += "')";
            sql += " AND tag.GUID = '" + tagGuid + "'";
            sql += " AND sch.DELETED IS NULL ";
            sql += " ORDER BY tag.NUMBER";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG[0];
            else
                return null;
        }

        public dsTAG.TAGDisciplineDataTable GetByProjectTemplate(Guid projectGuid, Guid templateGuid, Guid excludeWBSTagGuid, List<Guid> excludeGuids)
        {
            string sql = "SELECT tag.*, sch.DISCIPLINE FROM TAG tag JOIN SCHEDULE sch ON (tag.SCHEDULEGUID = sch.GUID)";
            sql += " JOIN TEMPLATE_REGISTER reg ON (reg.TAG_GUID = tag.GUID) ";
            sql += " OUTER APPLY";
            sql += " (SELECT * FROM ITR_MAIN WHERE TAG_GUID = tag.GUID AND TEMPLATE_GUID = reg.TEMPLATE_GUID AND DELETED IS NULL) iTR";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid + "') ";
            sql += " AND reg.TEMPLATE_GUID = '" + templateGuid + "' ";
            sql += " AND tag.GUID <> '" + excludeWBSTagGuid + "'";
            if(excludeGuids != null && excludeGuids.Count > 0)
            {
                foreach(Guid excludeGuid in excludeGuids)
                {
                    sql += " AND tag.GUID <> '" + excludeGuid + "'";
                }
            }
            sql += " AND tag.DELETED IS NULL";
            sql += " AND sch.DELETED IS NULL";
            sql += " AND reg.DELETED IS NULL";
            sql += " AND iTR.NAME IS NULL";
            sql += " ORDER BY tag.NUMBER";

            dsTAG.TAGDisciplineDataTable dtTAG = new dsTAG.TAGDisciplineDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Get tags by project and discipline and number
        /// </summary>
        public dsTAG.TAGDataTable GetByProjectTagNumber(Guid projectGuid, string searchNumber, SearchMode searchMode)
        {
            string sql = "SELECT tag.* FROM TAG tag JOIN SCHEDULE sch ON (tag.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid + "'";

            if(searchMode == SearchMode.Starts_With)
                sql += " AND tag.NUMBER LIKE '" + searchNumber + "%'";
            else if (searchMode == SearchMode.Ends_With)
                sql += " AND tag.NUMBER LIKE '%" + searchNumber + "'";
            else
                sql += " AND tag.NUMBER LIKE '%" + searchNumber + "%'";

            sql += " AND sch.DELETED IS NULL ";
            sql += " AND tag.DELETED IS NULL ";
            sql += " ORDER BY tag.NUMBER";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }


        /// <summary>
        /// Get tags by project and discipline and WBS name
        /// </summary>
        public dsTAG.TAGDisciplineDataTable GetByProjectWBSName(Guid projectGuid, List<string> subsystemNames = null, List<string> systemNames = null, List<string> areaNames = null, string tagNumber = "", List<string> stages = null, ProjectLibrary.SearchMode searchMode = ProjectLibrary.SearchMode.Contains)
        {
            string sql = "SELECT tag.*, sch.DISCIPLINE FROM TAG tag JOIN SCHEDULE sch ON (tag.SCHEDULEGUID = sch.GUID) ";
            sql += " JOIN WBS SubsystemWBS ON SubsystemWBS.GUID = tag.PARENTGUID";
            sql += " JOIN WBS SystemWBS ON SystemWBS.GUID = SubsystemWBS.PARENTGUID";
            sql += " JOIN WBS AreaWBS ON AreaWBS.GUID = SystemWBS.PARENTGUID";
            sql += " WHERE (sch.PROJECTGUID = '" + projectGuid + "') ";

            if (subsystemNames != null)
            {
                string subSystemQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(subsystemNames);
                sql += "AND SubsystemWBS.NAME IN (" + subSystemQueryConcatenation + ") ";
            }
            else if (systemNames != null)
            {
                string systemQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(systemNames);
                sql += "AND SystemWBS.NAME IN (" + systemQueryConcatenation + ") ";
            }
            else if (areaNames != null)
            {
                string areaQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(areaNames);
                sql += "AND AreaWBS.NAME IN (" + areaQueryConcatenation + ") ";
            }

            if (tagNumber != null && tagNumber != string.Empty)
            {
                if (searchMode == SearchMode.Starts_With)
                    sql += "AND tag.NUMBER LIKE '" + tagNumber + "%' ";
                else if (searchMode == SearchMode.Ends_With)
                    sql += "AND tag.NUMBER LIKE '%" + tagNumber + "' ";
                else
                    sql += "AND tag.NUMBER LIKE '%" + tagNumber + "%' ";
            }

            //always load all discipline for WBS
            //if (disciplines != null)
            //{
            //    string disciplineQueryConcatenation = SQLHelper.GetSQLQueryConcatenation(disciplines);
            //    sql += "AND sch.DISCIPLINE IN (" + disciplineQueryConcatenation + ") ";
            //}

            sql += " AND AreaWBS.DELETED IS NULL AND SystemWBS.DELETED IS NULL AND SubsystemWBS.DELETED IS NULL";
            sql += " AND sch.DELETED IS NULL ";
            sql += " AND tag.DELETED IS NULL ";
            sql += " ORDER BY tag.NUMBER";
            dsTAG.TAGDisciplineDataTable dtTAG = new dsTAG.TAGDisciplineDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Get tags by project and discipline
        /// </summary>
        public dsTAG.TAGDataTable GetByProjectDisciplineLike(Guid projectGuid, string discipline, string searchText)
        {
            string sql = "SELECT tag.* FROM TAG tag JOIN SCHEDULE sch ON (tag.SCHEDULEGUID = sch.GUID) ";
            sql += " WHERE sch.PROJECTGUID = '" + projectGuid + "' ";
            sql += " AND tag.NUMBER LIKE '%" + searchText + "%'";
            sql += " AND sch.DELETED IS NULL ";
            sql += " AND tag.DELETED IS NULL ";
            sql += " ORDER BY tag.NUMBER";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Get tags by schedule and search text
        /// </summary>
        public dsTAG.TAGDataTable GetByScheduleLike(Guid scheduleGuid, string searchText)
        {
            string sql = "SELECT * FROM TAG WHERE SCHEDULEGUID = '" + scheduleGuid + "'";
            sql += " AND NUMBER LIKE '%" + searchText + "%'";
            sql += " AND DELETED IS NULL";
            sql += " ORDER BY NUMBER";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Get recursive workflow childrens
        /// </summary>
        public dsTAG.TAGDataTable GetTagChildrens(Guid parentGuid, bool includeSelf)
        {
            string sql = "WITH RECURSIVETBL AS (SELECT a.* FROM TAG a ";

            if (includeSelf)
                sql += "WHERE GUID = '";
            else
                sql += "WHERE PARENTGUID = '";

            sql += parentGuid + "' UNION ALL ";
            sql += "SELECT a.* FROM TAG a JOIN RECURSIVETBL r ON a.PARENTGUID = r.GUID) ";
            sql += "SELECT * FROM RECURSIVETBL WHERE RECURSIVETBL.DELETED IS NULL";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Remove a particular tag by GUID
        /// </summary>
        public bool RemoveBy(Guid tagGuid)
        {
            string sql = "SELECT * FROM TAG WHERE GUID = '" + tagGuid + "'";

            dsTAG.TAGDataTable dtTag = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTag);

            if (dtTag != null)
            {
                dsTAG.TAGRow drTag = dtTag[0];
                drTag.DELETED = DateTime.Now;
                drTag.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drTag);
                return true;
            }

            return false;
        }

        //Changes 10-DEC-2014 Allow same tag number to be added per project
        /// <summary>
        /// Remove a particular tag by GUID
        /// </summary>
        public int RemoveAllWithExclusionBy(string tagNumber, Guid projGuid, Guid tagGuid)
        {
            string sql = "SELECT * FROM TAG AS tag";
            sql += " LEFT JOIN SCHEDULE AS sch ON (tag.SCHEDULEGUID = sch.GUID)";
            sql += " LEFT JOIN PROJECT AS proj ON (sch.PROJECTGUID = proj.GUID)";
            sql += " WHERE tag.NUMBER = '" + tagNumber + "'";
            sql += " AND proj.GUID = '" + projGuid + "'";
            sql += " AND tag.GUID IS NOT '" + tagGuid + "'";
            sql += " AND tag.DELETED IS NULL AND proj.DELETED IS NULL AND sch.DELETED IS NULL";

            dsTAG.TAGDataTable dtTag = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTag);

            int removeCount = 0;
            if (dtTag != null)
            {
                foreach(dsTAG.TAGRow drTag in dtTag.Rows)
                {
                    drTag.DELETED = DateTime.Now;
                    drTag.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drTag);
                    removeCount++;
                }
            }

            return removeCount;
        }

        /// <summary>
        /// Get all tags
        /// </summary
        public dsTAG.TAGDataTable Get()
        {
            string sql = "SELECT * FROM TAG WHERE DELETED IS NULL";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }


        /// <summary>
        /// Get deleted tags for purging
        /// </summary
        public dsTAG.TAGDataTable Get_Deleted(Guid projectGuid)
        {
            string sql = "SELECT Tag.* FROM TAG AS tag";
            sql += " LEFT JOIN SCHEDULE AS sch ON (tag.SCHEDULEGUID = sch.GUID)";
            sql += " LEFT JOIN PROJECT AS proj ON (sch.PROJECTGUID = proj.GUID)";
            sql += " WHERE proj.GUID = '" + projectGuid.ToString() + "'";
            sql += " AND tag.DELETED IS NOT NULL";

            dsTAG.TAGDataTable dtTAG = new dsTAG.TAGDataTable();
            ExecuteQuery(sql, dtTAG);

            if (dtTAG.Rows.Count > 0)
                return dtTAG;
            else
                return null;
        }

        /// <summary>
        /// Saves multiple tags
        /// </summary>
        public void Save(dsTAG.TAGDataTable dtTAG)
        {
            _adapter.Update(dtTAG);
        }

        /// <summary>
        /// Saves one tag
        /// </summary>
        public void Save(dsTAG.TAGRow drTAG)
        {
            _adapter.Update(drTAG);
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
