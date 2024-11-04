using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsPROJECTTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterPROJECT : SQLBase, IDisposable
    {
        private PROJECTTableAdapter _adapter;

        public AdapterPROJECT()
            : base()
        {
            _adapter = new PROJECTTableAdapter(Variables.ConnStr);
        }

        public AdapterPROJECT(string connStr)
            : base(connStr)
        {
            _adapter = new PROJECTTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        /// <summary>
        /// Get a particular project by GUID
        /// </summary>
        public dsPROJECT.PROJECTRow GetBy(Guid projectGuid)
        {
            string sql = "SELECT * FROM PROJECT WHERE GUID = '" + projectGuid + "'";
            sql += " AND DELETED IS NULL";

            dsPROJECT.PROJECTDataTable dtPROJECT = new dsPROJECT.PROJECTDataTable();
            ExecuteQuery(sql, dtPROJECT);

            if (dtPROJECT.Rows.Count > 0)
                return dtPROJECT[0];
            else
                return null;
        }

        /// <summary>
        /// Get a particular project by project number
        /// </summary>
        public dsPROJECT.PROJECTRow GetBy(string number)
        {
            string sql = "SELECT * FROM PROJECT WHERE NUMBER = '" + number + "'";
            sql += " AND DELETED IS NULL";

            dsPROJECT.PROJECTDataTable dtPROJECT = new dsPROJECT.PROJECTDataTable();
            ExecuteQuery(sql, dtPROJECT);

            if (dtPROJECT.Rows.Count > 0)
                return dtPROJECT[0];
            else
                return null;
        }

        /// <summary>
        /// Get deleted project to store it in tombstone
        /// </summary>
        public dsPROJECT.PROJECTDataTable Get_Deleted()
        {
            string sql = "SELECT * FROM PROJECT WHERE DELETED IS NOT NULL";

            dsPROJECT.PROJECTDataTable dtPROJECT = new dsPROJECT.PROJECTDataTable();
            ExecuteQuery(sql, dtPROJECT);

            if (dtPROJECT.Rows.Count > 0)
                return dtPROJECT;
            else
                return null;
        }

        /// <summary>
        /// Get a particular project include deletes
        /// </summary>
        public dsPROJECT.PROJECTRow GetIncludeDeletedBy(Guid projectGuid)
        {
            string sql = "SELECT * FROM PROJECT WHERE GUID = '" + projectGuid + "'";

            dsPROJECT.PROJECTDataTable dtPROJECT = new dsPROJECT.PROJECTDataTable();
            ExecuteQuery(sql, dtPROJECT);

            if (dtPROJECT.Rows.Count > 0)
                return dtPROJECT[0];
            else
                return null;
        }

        public dsPROJECT.PROJECTRow GetByTag(Guid tagGuid, bool isPunchlist)
        {
            string sql = "SELECT proj.* FROM PROJECT proj JOIN SCHEDULE";
            sql += " sch ON (sch.PROJECTGUID = proj.GUID) JOIN TAG";
            sql += " tag ON (tag.SCHEDULEGUID = sch.GUID)";
            if (isPunchlist)
            {
                sql += " JOIN PUNCHLIST_MAIN punchlist ON (punchlist.TAG_GUID = tag.GUID)";
                sql += " WHERE punchlist.TAG_GUID = '" + tagGuid + "'";
                sql += " AND punchlist.DELETED IS NULL";
            }
            else
            {
                sql += " JOIN ITR_MAIN itr ON (itr.TAG_GUID = tag.GUID)";
                sql += " WHERE itr.TAG_GUID = '" + tagGuid + "'";
                sql += " AND itr.DELETED IS NULL";
            }

            sql += " AND tag.DELETED IS NULL AND sch.DELETED IS NULL AND proj.DELETED IS NULL";

            dsPROJECT.PROJECTDataTable dtPROJECT = new dsPROJECT.PROJECTDataTable();
            ExecuteQuery(sql, dtPROJECT);

            if (dtPROJECT.Rows.Count > 0)
                return dtPROJECT[0];
            else
                return null;
        }

        public dsPROJECT.PROJECTRow GetByWBS(Guid wbsGuid, bool isPunchlist)
        {
            string sql = "SELECT proj.* FROM PROJECT proj JOIN SCHEDULE";
            sql += " sch ON (sch.PROJECTGUID = proj.GUID) JOIN WBS";
            sql += " wbs ON (wbs.SCHEDULEGUID = sch.GUID)";
            if (isPunchlist)
            {
                sql += " JOIN PUNCHLIST_MAIN punchlist ON (punchlist.WBS_GUID = wbs.GUID)";
                sql += " WHERE punchlist.TAG_GUID = '" + wbsGuid + "'";
                sql += " AND punchlist.DELETED IS NULL";
            }
            else
            {
                sql += " JOIN ITR_MAIN itr ON (itr.WBS_GUID = wbs.GUID)";
                sql += " WHERE itr.WBS_GUID = '" + wbsGuid + "'";
                sql += " AND itr.DELETED IS NULL";
            }
                
            sql += " AND wbs.DELETED IS NULL AND sch.DELETED IS NULL AND proj.DELETED IS NULL";

            dsPROJECT.PROJECTDataTable dtPROJECT = new dsPROJECT.PROJECTDataTable();
            ExecuteQuery(sql, dtPROJECT);

            if (dtPROJECT.Rows.Count > 0)
                return dtPROJECT[0];
            else
                return null;
        }


        public dsPROJECT.PROJECTRow GetByCertificate(Guid certificateGuid)
        {
            string sql = "SELECT proj.* FROM PROJECT proj JOIN CERTIFICATE_MAIN certificate ON certificate.PROJECTGUID = PROJECT.GUID WHERE certificate.GUID = '" + certificateGuid + "' AND certificate.DELETED IS NULL";

            dsPROJECT.PROJECTDataTable dtPROJECT = new dsPROJECT.PROJECTDataTable();
            ExecuteQuery(sql, dtPROJECT);

            if (dtPROJECT.Rows.Count > 0)
                return dtPROJECT[0];
            else
                return null;
        }

        /// <summary>
        /// Remove a particular project by GUID
        /// </summary>
        public bool RemoveBy(Guid projectGuid)
        {
            string sql = "SELECT * FROM PROJECT WHERE GUID = '" + projectGuid + "'";
            sql += " AND DELETED IS NULL";

            dsPROJECT.PROJECTDataTable dtProject = new dsPROJECT.PROJECTDataTable();
            ExecuteQuery(sql, dtProject);

            if(dtProject != null)
            {
                dsPROJECT.PROJECTRow drProject = dtProject[0];
                drProject.DELETED = DateTime.Now;
                drProject.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drProject);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove a particular project by GUID
        /// </summary>
        public int RemoveWithExclusionBy(string projectNumber, Guid projectGuid)
        {
            string sql = "SELECT * FROM PROJECT WHERE NUMBER = '" + projectNumber + "'";
            sql += " AND GUID IS NOT '" + projectGuid + "'";
            sql += " AND DELETED IS NULL";

            dsPROJECT.PROJECTDataTable dtProject = new dsPROJECT.PROJECTDataTable();
            ExecuteQuery(sql, dtProject);

            int removeCount = 0;
            if (dtProject != null)
            {
                foreach(dsPROJECT.PROJECTRow drProject in dtProject.Rows)
                {
                    drProject.DELETED = DateTime.Now;
                    drProject.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drProject);
                    removeCount++;
                }
            }

            return removeCount;
        }

        /// <summary>
        /// Get all projects
        /// </summary
        public dsPROJECT.PROJECTDataTable Get()
        {
            string sql = "SELECT * FROM PROJECT WHERE DELETED IS NULL ORDER BY NUMBER";

            dsPROJECT.PROJECTDataTable dtPROJECT = new dsPROJECT.PROJECTDataTable();
            ExecuteQuery(sql, dtPROJECT);

            if (dtPROJECT.Rows.Count > 0)
                return dtPROJECT;
            else
                return null;
        }

        /// <summary>
        /// Get all project include deleted
        /// </summary>
        public dsPROJECT.PROJECTDataTable GetAll_IncludeDeleted()
        {
            string sql = "SELECT * FROM PROJECT ORDER BY NUMBER";
            dsPROJECT.PROJECTDataTable dtPROJECT = new dsPROJECT.PROJECTDataTable();
            ExecuteQuery(sql, dtPROJECT);

            if (dtPROJECT.Rows.Count > 0)
                return dtPROJECT;
            else
                return null;
        }


        /// <summary>
        /// Saves multiple projects
        /// </summary>
        public void Save(dsPROJECT.PROJECTDataTable dtPROJECT)
        {
            _adapter.Update(dtPROJECT);
        }

        /// <summary>
        /// Saves one project
        /// </summary>
        public void Save(dsPROJECT.PROJECTRow drPROJECT)
        {
            _adapter.Update(drPROJECT);
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
