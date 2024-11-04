using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsROLE_MAINTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterROLE_MAIN : SQLBase, IDisposable
    {
        private ROLE_MAINTableAdapter _adapter;
        private ROLE_PRIVILEGETableAdapter _adapterPrivilege;

        public AdapterROLE_MAIN()
            : base()
        {
            _adapter = new ROLE_MAINTableAdapter(Variables.ConnStr);
            _adapterPrivilege = new ROLE_PRIVILEGETableAdapter(Variables.ConnStr);
        }

        public AdapterROLE_MAIN(string connStr)
            : base(connStr)
        {
            _adapter = new ROLE_MAINTableAdapter(connStr);
            _adapterPrivilege = new ROLE_PRIVILEGETableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
            _adapterPrivilege.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all role in the system
        /// </summary>
        public dsROLE_MAIN.ROLE_MAINDataTable Get()
        {
            string sql = "SELECT * FROM ROLE_MAIN WHERE DELETED IS NULL";
            dsROLE_MAIN.ROLE_MAINDataTable dtROLE_MAIN = new dsROLE_MAIN.ROLE_MAINDataTable();
            ExecuteQuery(sql, dtROLE_MAIN);

            if (dtROLE_MAIN.Rows.Count > 0)
                return dtROLE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get deleted role in the system for purging
        /// </summary>
        public dsROLE_MAIN.ROLE_MAINDataTable Get_Deleted()
        {
            string sql = "SELECT * FROM ROLE_MAIN WHERE DELETED IS NOT NULL";
            dsROLE_MAIN.ROLE_MAINDataTable dtROLE_MAIN = new dsROLE_MAIN.ROLE_MAINDataTable();
            ExecuteQuery(sql, dtROLE_MAIN);

            if (dtROLE_MAIN.Rows.Count > 0)
                return dtROLE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get all role in the system
        /// </summary>
        public dsROLE_MAIN.ROLE_MAINDataTable GetIncludeDeleted()
        {
            string sql = "SELECT * FROM ROLE_MAIN";
            dsROLE_MAIN.ROLE_MAINDataTable dtROLE_MAIN = new dsROLE_MAIN.ROLE_MAINDataTable();
            ExecuteQuery(sql, dtROLE_MAIN);

            if (dtROLE_MAIN.Rows.Count > 0)
                return dtROLE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get user authorised role
        /// </summary>
        public dsROLE_MAIN.ROLE_MAINDataTable GetAuthRole()
        {
            string sql = "WITH RECURSIVETBL AS (SELECT a.* FROM ROLE_MAIN a ";
            sql += "WHERE PARENTGUID = '" + System_Environment.GetUser().userRole.Value + "' UNION ALL ";
            sql += "SELECT a.* FROM ROLE_MAIN a JOIN RECURSIVETBL r ON a.PARENTGUID = r.GUID) ";
            sql += "SELECT * FROM RECURSIVETBL WHERE RECURSIVETBL.DELETED IS NULL";

            dsROLE_MAIN.ROLE_MAINDataTable dtROLE_MAIN = new dsROLE_MAIN.ROLE_MAINDataTable();
            ExecuteQuery(sql, dtROLE_MAIN);

            if (dtROLE_MAIN.Rows.Count > 0)
                return dtROLE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get role by guid
        /// </summary>
        public dsROLE_MAIN.ROLE_MAINRow GetBy(Guid roleGuid)
        {
            string sql = "SELECT * FROM ROLE_MAIN WHERE GUID = '" + roleGuid + "'";
            sql += " AND DELETED IS NULL";

            dsROLE_MAIN.ROLE_MAINDataTable dtROLE_MAIN = new dsROLE_MAIN.ROLE_MAINDataTable();
            ExecuteQuery(sql, dtROLE_MAIN);

            if (dtROLE_MAIN.Rows.Count > 0)
                return dtROLE_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get role by guid including deletes
        /// </summary>
        public dsROLE_MAIN.ROLE_MAINRow GetIncludeDeletedBy(Guid roleGuid)
        {
            string sql = "SELECT * FROM ROLE_MAIN WHERE GUID = '" + roleGuid + "'";

            dsROLE_MAIN.ROLE_MAINDataTable dtROLE_MAIN = new dsROLE_MAIN.ROLE_MAINDataTable();
            ExecuteQuery(sql, dtROLE_MAIN);

            if (dtROLE_MAIN.Rows.Count > 0)
                return dtROLE_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get role by name
        /// </summary>
        public dsROLE_MAIN.ROLE_MAINRow GetBy(string name)
        {
            string sql = "SELECT * FROM ROLE_MAIN WHERE NAME = '" + name + "'";
            sql += " AND DELETED IS NULL";

            dsROLE_MAIN.ROLE_MAINDataTable dtROLE_MAIN = new dsROLE_MAIN.ROLE_MAINDataTable();
            ExecuteQuery(sql, dtROLE_MAIN);

            if (dtROLE_MAIN.Rows.Count > 0)
                return dtROLE_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Role main needs to be tagged with latest update date for sync to detect any changes
        /// </summary>
        public void RoleUpdate(Guid roleGuid)
        {
            dsROLE_MAIN.ROLE_MAINRow drRole = GetBy(roleGuid);
            if(drRole != null)
            {
                drRole.UPDATED = DateTime.Now;
                drRole.UPDATEDBY = System_Environment.GetUser().GUID;
                Save(drRole);
            }
        }

        /// <summary>
        /// Remove a particular Role by GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM ROLE_MAIN WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsROLE_MAIN.ROLE_MAINDataTable dtRole = new dsROLE_MAIN.ROLE_MAINDataTable();
            ExecuteQuery(sql, dtRole);

            if (dtRole != null)
            {
                dsROLE_MAIN.ROLE_MAINRow drRole = dtRole[0];
                drRole.DELETED = DateTime.Now;
                drRole.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drRole);
                return true;
            }

            return false;
        }

        public int RemoveAllWithExclusionBy(string roleName, Guid excludedGuid)
        {
            string sql = "SELECT * FROM ROLE_MAIN WHERE NAME = '" + roleName + "'";
            sql += " AND GUID IS NOT '" + excludedGuid + "'";
            sql += " AND DELETED IS NULL";

            dsROLE_MAIN.ROLE_MAINDataTable dtRole = new dsROLE_MAIN.ROLE_MAINDataTable();
            ExecuteQuery(sql, dtRole);

            int removeCount = 0;
            if (dtRole != null)
            {
                foreach(dsROLE_MAIN.ROLE_MAINRow drRole in dtRole.Rows)
                {
                    drRole.DELETED = DateTime.Now;
                    drRole.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drRole);
                    removeCount++;
                }
            }

            return removeCount;
        }

        /// <summary>
        /// Update multiple roles
        /// </summary>
        public void Save(dsROLE_MAIN.ROLE_MAINDataTable dtROLE_MAIN)
        {
            _adapter.Update(dtROLE_MAIN);
        }

        /// <summary>
        /// Update one role
        /// </summary>
        public void Save(dsROLE_MAIN.ROLE_MAINRow drROLE_MAIN)
        {
            _adapter.Update(drROLE_MAIN);
        } 
        #endregion

        #region Role Privilege
        /// <summary>
        /// Get all privilege
        /// </summary>
        public dsROLE_MAIN.ROLE_PRIVILEGEDataTable GetPrivilege()
        {
            string sql = "SELECT * FROM ROLE_PRIVILEGE WHERE DELETED IS NULL";

            dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtROLE_PRIVILEGE = new dsROLE_MAIN.ROLE_PRIVILEGEDataTable();
            ExecuteQuery(sql, dtROLE_PRIVILEGE);

            if (dtROLE_PRIVILEGE.Rows.Count > 0)
                return dtROLE_PRIVILEGE;
            else
                return null;
        }

        /// <summary>
        /// Get all privilege
        /// </summary>
        public dsROLE_MAIN.ROLE_PRIVILEGEDataTable Get_Deleted_Privilege()
        {
            string sql = "SELECT * FROM ROLE_PRIVILEGE WHERE DELETED IS NOT NULL";

            dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtROLE_PRIVILEGE = new dsROLE_MAIN.ROLE_PRIVILEGEDataTable();
            ExecuteQuery(sql, dtROLE_PRIVILEGE);

            if (dtROLE_PRIVILEGE.Rows.Count > 0)
                return dtROLE_PRIVILEGE;
            else
                return null;
        }

        /// <summary>
        /// Get privilege by guid
        /// </summary>
        public dsROLE_MAIN.ROLE_PRIVILEGEDataTable GetPrivilegeBy(Guid roleGuid)
        {
            string sql = "SELECT * FROM ROLE_PRIVILEGE WHERE ROLEGUID = '" + roleGuid + "'";
            sql += " AND DELETED IS NULL";

            dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtROLE_PRIVILEGE = new dsROLE_MAIN.ROLE_PRIVILEGEDataTable();
            ExecuteQuery(sql, dtROLE_PRIVILEGE);

            if (dtROLE_PRIVILEGE.Rows.Count > 0)
                return dtROLE_PRIVILEGE;
            else
                return null;
        }

        public dsROLE_MAIN.ROLE_PRIVILEGERow GetPrivilegeBy(Guid roleGuid, Privilege privilege)
        {
            string sql = "SELECT * FROM ROLE_PRIVILEGE WHERE ROLEGUID = '" + roleGuid + "'";
            sql += " AND TYPEID = '" + privilege.privTypeID + "' AND DELETED IS NULL";

            dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtROLE_PRIVILEGE = new dsROLE_MAIN.ROLE_PRIVILEGEDataTable();
            ExecuteQuery(sql, dtROLE_PRIVILEGE);

            if (dtROLE_PRIVILEGE.Rows.Count > 0)
                return dtROLE_PRIVILEGE[0];
            else
                return null;
        }

        /// <summary>
        /// Add role privilege authorisation
        /// </summary>
        /// <param name="addRoleGuid">Adding role guid</param>
        public void SetRolePrivilege(Guid roleGuid, Privilege privilege)
        {
            dsROLE_MAIN dsRole = new dsROLE_MAIN();
            dsROLE_MAIN.ROLE_PRIVILEGERow drRolePrivilege = dsRole.ROLE_PRIVILEGE.NewROLE_PRIVILEGERow();

            drRolePrivilege.GUID = Guid.NewGuid();
            drRolePrivilege.TYPEID = privilege.privTypeID;
            drRolePrivilege.ROLEGUID = roleGuid;
            drRolePrivilege.NAME = privilege.privName;
            drRolePrivilege.LOCKED = false;
            drRolePrivilege.CREATED = DateTime.Now;
            drRolePrivilege.CREATEDBY = System_Environment.GetUser().GUID;

            dsRole.ROLE_PRIVILEGE.AddROLE_PRIVILEGERow(drRolePrivilege);
            Save(drRolePrivilege);
        }

        /// <summary>
        /// Remove role privilege authorisation
        /// </summary>
        /// <param name="deleteroleGuid">Deleting role guid</param>
        /// <returns></returns>
        public bool RemoveRolePrivilege(Guid roleGuid, string privTypeID)
        {
            string sql = "SELECT * FROM ROLE_PRIVILEGE WHERE ROLEGUID = '" + roleGuid + "'";
            sql += " AND TYPEID = '" + privTypeID + "'";
            sql += " AND DELETED IS NULL";

            dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtRolePrivilege = new dsROLE_MAIN.ROLE_PRIVILEGEDataTable();
            ExecuteQuery(sql, dtRolePrivilege);

            if (dtRolePrivilege != null)
            {
                dsROLE_MAIN.ROLE_PRIVILEGERow drRolePrivilege = dtRolePrivilege[0];
                drRolePrivilege.DELETED = DateTime.Now;
                drRolePrivilege.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drRolePrivilege);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update multiple provileges
        /// </summary>
        public void Save(dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtROLE_PRIVILEGE)
        {
            _adapterPrivilege.Update(dtROLE_PRIVILEGE);
        }

        /// <summary>
        /// Update one privilege
        /// </summary>
        public void Save(dsROLE_MAIN.ROLE_PRIVILEGERow drROLE_PRIVILEGE)
        {
            _adapterPrivilege.Update(drROLE_PRIVILEGE);
        } 
        #endregion
        public void Dispose()
        {
            if (_adapter != null)
            {
                _adapter.Dispose();
                _adapter = null;
            }
            if (_adapterPrivilege != null)
            {
                _adapterPrivilege.Dispose();
                _adapterPrivilege = null;
            }
        }
    }
}
