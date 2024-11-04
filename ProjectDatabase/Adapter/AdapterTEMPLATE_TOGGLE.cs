using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsTEMPLATE_TOGGLETableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterTEMPLATE_TOGGLE : SQLBase, IDisposable
    {
        private TEMPLATE_TOGGLETableAdapter _adapter;

        public AdapterTEMPLATE_TOGGLE()
            : base()
        {
            _adapter = new TEMPLATE_TOGGLETableAdapter(Variables.ConnStr);
        }

        public AdapterTEMPLATE_TOGGLE(string connStr)
            : base(connStr)
        {
            _adapter = new TEMPLATE_TOGGLETableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        public dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable GetAll()
        {
            string sql = "SELECT * FROM TEMPLATE_TOGGLE";
            sql += " WHERE DELETED IS NULL";

            dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE = new dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable();
            ExecuteQuery(sql, dtTEMPLATE_TOGGLE);

            if (dtTEMPLATE_TOGGLE.Rows.Count > 0)
                return dtTEMPLATE_TOGGLE;
            else
                return null;
        }

        public dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable GetBy_Discipline(string discipline)
        {
            string sql = "SELECT * FROM TEMPLATE_TOGGLE WHERE DISCIPLINE = '" + discipline + "'";
            sql += " AND DELETED IS NULL ORDER BY NAME";

            dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE = new dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable();
            ExecuteQuery(sql, dtTEMPLATE_TOGGLE);

            if (dtTEMPLATE_TOGGLE.Rows.Count > 0)
                return dtTEMPLATE_TOGGLE;
            else
                return null;
        }

        public dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow GetBy_DisciplineAndName(string discipline, string templateName)
        {
            string sql = "SELECT * FROM TEMPLATE_TOGGLE WHERE DISCIPLINE = '" + discipline + "'";
            sql += " AND NAME = '" + templateName + "' AND DELETED IS NULL";

            dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE = new dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable();
            ExecuteQuery(sql, dtTEMPLATE_TOGGLE);

            if (dtTEMPLATE_TOGGLE.Rows.Count > 0)
                return dtTEMPLATE_TOGGLE[0];
            else
                return null;
        }

        public dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow GetBy(Guid guid)
        {
            string sql = "SELECT * FROM TEMPLATE_TOGGLE WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE = new dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable();
            ExecuteQuery(sql, dtTEMPLATE_TOGGLE);

            if (dtTEMPLATE_TOGGLE.Rows.Count > 0)
                return dtTEMPLATE_TOGGLE[0];
            else
                return null;
        }

        /// <summary>
        /// Get deleted template toggle for purging
        /// </summary>
        public dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable Get_Deleted(string discipline)
        {
            string sql = "SELECT * FROM TEMPLATE_TOGGLE WHERE DELETED IS NOT NULL AND DISCIPLINE = '" + discipline + "'";
            dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE = new dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable();
            ExecuteQuery(sql, dtTEMPLATE_TOGGLE);

            if (dtTEMPLATE_TOGGLE.Rows.Count > 0)
                return dtTEMPLATE_TOGGLE;
            else
                return null;
        }

        /// <summary>
        /// Remove a particular toggle by GUID
        /// </summary>
        public bool RemoveBy(Guid templateToggleGuid)
        {
            string sql = "SELECT * FROM TEMPLATE_TOGGLE WHERE GUID = '" + templateToggleGuid + "'";
            sql += " AND DELETED IS NULL";

            dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATETOGGLE = new dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable();
            ExecuteQuery(sql, dtTEMPLATETOGGLE);

            if (dtTEMPLATETOGGLE != null)
            {
                dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow drTEMPLATETOGGLE = dtTEMPLATETOGGLE[0];
                drTEMPLATETOGGLE.DELETED = DateTime.Now;
                drTEMPLATETOGGLE.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drTEMPLATETOGGLE);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update multiple syncs
        /// </summary>
        public void Save(dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE)
        {
            _adapter.Update(dtTEMPLATE_TOGGLE);
        }

        /// <summary>
        /// Update one sync
        /// </summary>
        public void Save(dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow drTEMPLATE_TOGGLE)
        {
            _adapter.Update(drTEMPLATE_TOGGLE);
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
