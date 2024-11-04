using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsGENERAL_EQUIPMENTTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterGENERAL_EQUIPMENT : SQLBase, IDisposable
    {
        private GENERAL_EQUIPMENTTableAdapter _adapter;

        public AdapterGENERAL_EQUIPMENT()
            : base()
        {
            _adapter = new GENERAL_EQUIPMENTTableAdapter(Variables.ConnStr);
        }

        public AdapterGENERAL_EQUIPMENT(string connStr)
            : base(connStr)
        {
            _adapter = new GENERAL_EQUIPMENTTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        /// <summary>
        /// Get all equipments
        /// </summary>
        public dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable Get()
        {
            string sql = "SELECT * FROM GENERAL_EQUIPMENT";
            sql += " WHERE DELETED IS NULL ORDER BY ASSET_NUMBER";

            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtGENERAL_EQUIPMENT = new dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable();
            ExecuteQuery(sql, dtGENERAL_EQUIPMENT);

            if (dtGENERAL_EQUIPMENT.Rows.Count > 0)
                return dtGENERAL_EQUIPMENT;
            else
                return null;
        }

        /// <summary>
        /// Get deleted equipments for purging
        /// </summary>
        public dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable Get_Deleted(Guid projectGuid)
        {
            string sql = "SELECT * FROM GENERAL_EQUIPMENT WHERE DELETED IS NOT NULL AND PROJECTGUID = '" + projectGuid.ToString() + "'";
            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtGENERAL_EQUIPMENT = new dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable();
            ExecuteQuery(sql, dtGENERAL_EQUIPMENT);

            if (dtGENERAL_EQUIPMENT.Rows.Count > 0)
                return dtGENERAL_EQUIPMENT;
            else
                return null;
        }

        /// <summary>
        /// Get all equipments include deleted
        /// </summary>
        public dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable GetAll_IncludeDeleted()
        {
            string sql = "SELECT * FROM GENERAL_EQUIPMENT";

            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtGENERAL_EQUIPMENT = new dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable();
            ExecuteQuery(sql, dtGENERAL_EQUIPMENT);

            if (dtGENERAL_EQUIPMENT.Rows.Count > 0)
                return dtGENERAL_EQUIPMENT;
            else
                return null;
        }

        public dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow GetBy_IncludeDeleted(Guid EquipmentGUID)
        {
            string sql = "SELECT * FROM GENERAL_EQUIPMENT WHERE GUID = '" + EquipmentGUID + "'";

            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtGENERAL_EQUIPMENT = new dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable();
            ExecuteQuery(sql, dtGENERAL_EQUIPMENT);

            if (dtGENERAL_EQUIPMENT.Rows.Count > 0)
                return dtGENERAL_EQUIPMENT[0];
            else
                return null;
        }

        /// <summary>
        /// Get a particular Equipment by GUID
        /// </summary>
        public dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow GetBy(Guid EquipmentGuid)
        {
            string sql = "SELECT * FROM GENERAL_EQUIPMENT WHERE GUID = '" + EquipmentGuid + "'";
            sql += " AND DELETED IS NULL";

            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtGENERAL_EQUIPMENT = new dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable();
            ExecuteQuery(sql, dtGENERAL_EQUIPMENT);

            if (dtGENERAL_EQUIPMENT.Rows.Count > 0)
                return dtGENERAL_EQUIPMENT[0];
            else
                return null;
        }

        /// <summary>
        /// Get a particular Equipment by Equipment number
        /// </summary>
        public dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow GetBy(string serialNumber)
        {
            string sql = "SELECT * FROM GENERAL_EQUIPMENT WHERE SERIAL = '" + serialNumber + "'";
            sql += " AND DELETED IS NULL";

            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtGENERAL_EQUIPMENT = new dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable();
            ExecuteQuery(sql, dtGENERAL_EQUIPMENT);

            if (dtGENERAL_EQUIPMENT.Rows.Count > 0)
                return dtGENERAL_EQUIPMENT[0];
            else
                return null;
        }

        /// <summary>
        /// Remove a particular Equipment by GUID
        /// </summary>
        public bool RemoveBy(Guid equipmentGuid)
        {
            string sql = "SELECT * FROM GENERAL_EQUIPMENT WHERE GUID = '" + equipmentGuid + "'";
            sql += " AND DELETED IS NULL";

            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtGENERAL_EQUIPMENT = new dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable();
            ExecuteQuery(sql, dtGENERAL_EQUIPMENT);

            if (dtGENERAL_EQUIPMENT != null)
            {
                dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drGENERAL_EQUIPMENT = dtGENERAL_EQUIPMENT[0];
                drGENERAL_EQUIPMENT.DELETED = DateTime.Now;
                drGENERAL_EQUIPMENT.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drGENERAL_EQUIPMENT);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove a particular Equipment by GUID with exclusion
        /// </summary>
        public int RemoveWithExclusionBy(string serialNumber, Guid excludedGuid)
        {
            string sql = "SELECT * FROM GENERAL_EQUIPMENT WHERE SERIAL = '" + serialNumber + "'";
            sql += " AND GUID IS NOT '" + excludedGuid + "'";
            sql += " AND DELETED IS NULL";

            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtGENERAL_EQUIPMENT = new dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable();
            ExecuteQuery(sql, dtGENERAL_EQUIPMENT);

            int removeCount = 0;
            if (dtGENERAL_EQUIPMENT != null)
            {
                foreach (dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drGENERAL_EQUIPMENT in dtGENERAL_EQUIPMENT.Rows)
                {
                    drGENERAL_EQUIPMENT.DELETED = DateTime.Now;
                    drGENERAL_EQUIPMENT.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drGENERAL_EQUIPMENT);
                    removeCount++;
                }
            }

            return removeCount;
        }

        /// <summary>
        /// Get all Equipments
        /// </summary
        public dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable GetByDisciplineProject(string discipline, Guid? project)
        {
            string sql = string.Empty;
            if(project != null)
            {
                sql = "SELECT * FROM GENERAL_EQUIPMENT";
                sql += " WHERE DISCIPLINE = '" + discipline + "' AND PROJECTGUID = '" + project + "'";
                sql += " AND DELETED IS NULL";
            }
            else
            {
                sql = "SELECT * FROM GENERAL_EQUIPMENT";
                sql += " WHERE DISCIPLINE = '" + discipline + "'";
                sql += " AND DELETED IS NULL";
            }

            dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtGENERAL_EQUIPMENT = new dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable();
            ExecuteQuery(sql, dtGENERAL_EQUIPMENT);

            if (dtGENERAL_EQUIPMENT.Rows.Count > 0)
                return dtGENERAL_EQUIPMENT;
            else
                return null;
        }

        /// <summary>
        /// Saves multiple Equipments
        /// </summary>
        public void Save(dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTDataTable dtGENERAL_EQUIPMENT)
        {
            _adapter.Update(dtGENERAL_EQUIPMENT);
        }

        /// <summary>
        /// Saves one Equipment
        /// </summary>
        public void Save(dsGENERAL_EQUIPMENT.GENERAL_EQUIPMENTRow drGENERAL_EQUIPMENT)
        {
            _adapter.Update(drGENERAL_EQUIPMENT);
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
