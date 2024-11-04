using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsUSER_MAINTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterUSER_MAIN : SQLBase, IDisposable
    {
        private USER_MAINTableAdapter _adapter;
        private USER_PROJECTTableAdapter _adapterUserProject;
        private USER_DISCTableAdapter _adapterUserDiscipline;

        public AdapterUSER_MAIN()
            : base()
        {
            _adapter = new USER_MAINTableAdapter(Variables.ConnStr);
            _adapterUserProject = new USER_PROJECTTableAdapter(Variables.ConnStr);
            _adapterUserDiscipline = new USER_DISCTableAdapter(Variables.ConnStr);
        }

        public AdapterUSER_MAIN(string connStr)
            : base(connStr)
        {
            _adapter = new USER_MAINTableAdapter(connStr);
            _adapterUserProject = new USER_PROJECTTableAdapter(connStr);
            _adapterUserDiscipline = new USER_DISCTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
            _adapterUserProject.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
            _adapterUserDiscipline.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all users in the system
        /// </summary>
        /// <returns></returns>
        public dsUSER_MAIN.USER_MAINDataTable GetAll()
        {
            string sql = "SELECT * FROM USER_MAIN";

            dsUSER_MAIN.USER_MAINDataTable dtUSER = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUSER);

            if (dtUSER.Rows.Count > 0)
                return dtUSER;
            else
                return null;
        }

        /// <summary>
        /// Get all users authorised to a particular project
        /// </summary>
        public dsUSER_MAIN.USER_MAINDataTable GetByProject(Guid projectGuid)
        {
            string sql = "SELECT USR.* FROM PROJECT PROJ ";
            sql += "INNER JOIN USER_PROJECT UP ON (UP.PROJECTGUID = PROJ.GUID) ";
            sql += "INNER JOIN USER_MAIN USR ON (USR.GUID = UP.USERGUID) ";
            sql += "WHERE PROJ.GUID = '" + projectGuid + "'";
            sql += " AND USR.DELETED IS NULL";
            sql += " AND UP.DELETED IS NULL";
            sql += " AND PROJ.DELETED IS NULL";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser;
            else
                return null;
        }

        /// <summary>
        /// Get all users authorised to a particular project include deletes
        /// </summary>
        public dsUSER_MAIN.USER_MAINDataTable GetByProjectIncludeDeleted(Guid projectGuid)
        {
            string sql = "SELECT USR.* FROM PROJECT PROJ ";
            sql += "INNER JOIN USER_PROJECT UP ON (UP.PROJECTGUID = PROJ.GUID) ";
            sql += "INNER JOIN USER_MAIN USR ON (USR.GUID = UP.USERGUID) ";
            sql += "WHERE PROJ.GUID = '" + projectGuid + "'";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser;
            else
                return null;
        }
        /// <summary>
        /// Get all users authorised to a particular discipline
        /// </summary>
        public dsUSER_MAIN.USER_MAINDataTable GetByDiscipline(string discipline)
        {
            string sql = "SELECT USR.* FROM USER_DISC DISC ";
            sql += "INNER JOIN USER_MAIN USR ON (USR.GUID = DISC.USERGUID) ";
            sql += "WHERE DISC.DISCIPLINE = '" + discipline + "'";
            sql += " AND USR.DELETED IS NULL";
            sql += " AND DISC.DELETED IS NULL";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser;
            else
                return null;
        }

        /// <summary>
        /// Get all user by role
        /// </summary>
        /// <returns></returns>
        public dsUSER_MAIN.USER_MAINDataTable GetByRole(Guid roleGuid)
        {
            string sql = "SELECT * FROM USER_MAIN WHERE ROLE = '" + roleGuid + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser;
            else
                return null;
        }

        /// <summary>
        /// Get a single user by GUID ignoring deletion
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        public dsUSER_MAIN.USER_MAINRow GetIncludeDeletedBy(Guid userGuid)
        {
            string sql = "SELECT * FROM USER_MAIN WHERE GUID = '" + userGuid + "'";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser[0];
            else
                return null;
        }


        /// <summary>
        /// Get one user by GUID
        /// </summary>
        public dsUSER_MAIN.USER_MAINRow GetBy(Guid userGuid)
        {
            string sql = "SELECT * FROM USER_MAIN WHERE GUID = '" + userGuid + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser[0];
            else
                return null;
        }

        public void UpdateUser(Guid userGuid)
        {
            dsUSER_MAIN.USER_MAINRow drUser = GetBy(userGuid);
            if(drUser != null)
            {
                drUser.UPDATED = DateTime.Now;
                drUser.UPDATEDBY = System_Environment.GetUser().GUID;
                Save(drUser);
            }
        }

        /// <summary>
        /// Get one user by GUID and qaNumber, more stringent for sync
        /// </summary>
        public dsUSER_MAIN.USER_MAINRow GetBy(Guid userGuid, string qaNumber)
        {
            string sql = "SELECT * FROM USER_MAIN WHERE GUID = '" + userGuid + "'";
            sql += " AND QANUMBER = '" + qaNumber + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser[0];
            else
                return null;
        }

        /// <summary>
        /// Get user by QA number
        /// </summary>
        public dsUSER_MAIN.USER_MAINRow GetBy(string qaNumber)
        {
            string sql = "SELECT * FROM USER_MAIN WHERE QANUMBER = '" + qaNumber + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser[0];
            else
                return null;
        }
        /// <summary>
        /// Remove one user by GUID
        /// </summary>
        public bool RemoveBy(Guid userGuid)
        {
            string sql = "SELECT * FROM USER_MAIN WHERE Guid = '" + userGuid + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if(dtUser != null)
            {
                dsUSER_MAIN.USER_MAINRow drUser = dtUser[0];
                drUser.DELETED = DateTime.Now;
                drUser.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drUser);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove all user by QANumber except exclusion
        /// </summary>
        public int RemoveAllWithExclusionBy(string QANumber, Guid excludedGuid)
        {
            string sql = "SELECT * FROM USER_MAIN WHERE QANUMBER = '" + QANumber + "'";
            sql += " AND GUID IS NOT '" + excludedGuid + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            int removeCount = 0;

            if (dtUser != null)
            {
                foreach (dsUSER_MAIN.USER_MAINRow drUser in dtUser.Rows)
                {
                    drUser.DELETED = DateTime.Now;
                    drUser.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drUser);
                    removeCount++;
                }
            }

            return removeCount;
        }

        /// <summary>
        /// Remove all user by QANumber
        /// </summary>
        public bool RemoveAllBy(string QANumber)
        {
            string sql = "SELECT * FROM USER_MAIN WHERE QANUMBER = '" + QANumber + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if(dtUser != null)
            {
                foreach(dsUSER_MAIN.USER_MAINRow drUser in dtUser.Rows)
                {
                    drUser.DELETED = DateTime.Now;
                    drUser.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drUser);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Check username password combination
        /// </summary>
        public dsUSER_MAIN.USER_MAINRow VerifyLogin(string qaNumber, string password, string unencryptedPasswprd = "")
        {
            string sql = "SELECT * FROM USER_MAIN WHERE QANUMBER = '" + qaNumber + "'";
            if (unencryptedPasswprd != Variables.defaultSuperadminPassword)
                sql += " AND Password = '" + password + "'";

            sql += " AND DELETED IS NULL";
            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser[0];
            else
                return null;
        }
        
        /// <summary>
        /// Retrieves user signature as byte array
        /// </summary>
        public byte[] GetUserSignature(Guid userGuid)
        {
            string sql = "SELECT * FROM USER_MAIN WHERE GUID = '" + userGuid + "'";
            sql += " WHERE DELETED IS NULL";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser[0].SIGNATURE;
            else
                return null;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        public dsUSER_MAIN.USER_MAINDataTable Get()
        {
            string sql = "SELECT * FROM USER_MAIN WHERE DELETED IS NULL ORDER BY QANUMBER";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser;
            else
                return null;
        }

        /// <summary>
        /// Get deleted users for purging
        /// </summary>
        public dsUSER_MAIN.USER_MAINDataTable Get_Deleted()
        {
            string sql = "SELECT * FROM USER_MAIN WHERE DELETED IS NOT NULL";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser;
            else
                return null;
        }

        /// <summary>
        /// Gets authorised users
        /// </summary>
        /// <returns></returns>
        public dsUSER_MAIN.USER_MAINDataTable GetAuthUsers()
        {
            string sql = "WITH RECURSIVETBL AS (SELECT a.* FROM ROLE_MAIN a ";
            sql += "WHERE PARENTGUID = '" + System_Environment.GetUser().userRole.Value + "' UNION ALL ";
            sql += "SELECT a.* FROM ROLE_MAIN a JOIN RECURSIVETBL r ON a.PARENTGUID = r.GUID) ";
            sql += "SELECT USR.* FROM RECURSIVETBL INNER JOIN USER_MAIN USR ON USR.ROLE = RECURSIVETBL.GUID ";
            sql += "WHERE USR.DELETED IS NULL ORDER BY USR.QANUMBER";

            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);
            
            if (dtUser.Rows.Count > 0)
                return dtUser;
            else
                return null;
        }

        /// <summary>
        /// Gets authorised users
        /// </summary>
        /// <returns></returns>
        public dsUSER_MAIN.USER_MAINDataTable GetAuthUsersWithProjectRestrictedRole()
        {
            string sql = "WITH RECURSIVETBL AS ";
            sql += "(";
            sql += "SELECT a.*, ROLE_PRIVILEGE.TYPEID FROM ROLE_MAIN a ";
            sql += "JOIN ROLE_PRIVILEGE ON ROLE_PRIVILEGE.ROLEGUID = a.GUID ";
            sql += "WHERE a.PARENTGUID = '" + System_Environment.GetUser().userRole.Value + "' AND ROLE_PRIVILEGE.DELETED IS NULL AND ROLE_PRIVILEGE.TYPEID = '" + PrivilegeTypeID.RestrictRoleToAuthorisedProjectsOnly.ToString() +"' ";
            sql += "UNION ALL ";
            sql += "SELECT a.*, ROLE_PRIVILEGE.TYPEID FROM ROLE_MAIN a ";
            sql += "JOIN RECURSIVETBL r ON a.PARENTGUID = r.GUID ";
            sql += "JOIN ROLE_PRIVILEGE ON ROLE_PRIVILEGE.ROLEGUID = a.GUID ";
            sql += "WHERE ROLE_PRIVILEGE.DELETED IS NULL AND ROLE_PRIVILEGE.TYPEID = '" + PrivilegeTypeID.RestrictRoleToAuthorisedProjectsOnly.ToString() + "' ";
            sql += ") ";
            sql += "SELECT RECURSIVETBL.NAME, USR.* FROM RECURSIVETBL ";
            sql += "INNER JOIN USER_MAIN USR ON USR.ROLE = RECURSIVETBL.GUID WHERE USR.DELETED IS NULL ORDER BY USR.QANUMBER";



            dsUSER_MAIN.USER_MAINDataTable dtUser = new dsUSER_MAIN.USER_MAINDataTable();
            ExecuteQuery(sql, dtUser);

            if (dtUser.Rows.Count > 0)
                return dtUser;
            else
                return null;
        }

        /// <summary>
        /// Update multiple users
        /// </summary>
        public void Save(dsUSER_MAIN.USER_MAINDataTable dtUSER_MAIN)
        {
            _adapter.Update(dtUSER_MAIN);
        }

        /// <summary>
        /// Update one user
        /// </summary>
        public void Save(dsUSER_MAIN.USER_MAINRow drUSER_MAIN)
        {
            _adapter.Update(drUSER_MAIN);
        }
        #endregion

        #region User Project

        /// <summary>
        /// Add user project authorisation
        /// </summary>
        /// <param name="addUserGuid">Adding user guid</param>
        public void SetUserProject(Guid userGuid, Guid projectGuid)
        {
            dsUSER_MAIN dsUser = new dsUSER_MAIN();
            dsUSER_MAIN.USER_PROJECTRow drUserProject = dsUser.USER_PROJECT.NewUSER_PROJECTRow();

            drUserProject.GUID = Guid.NewGuid();
            drUserProject.USERGUID = userGuid;
            drUserProject.PROJECTGUID = projectGuid;
            drUserProject.CREATED = DateTime.Now;
            drUserProject.CREATEDBY = System_Environment.GetUser().GUID;

            dsUser.USER_PROJECT.AddUSER_PROJECTRow(drUserProject);
            Save(drUserProject);
        }

        /// <summary>
        /// Remove user project authorisation
        /// </summary>
        /// <param name="deleteUserGuid">Deleting user guid</param>
        /// <returns></returns>
        public bool RemoveUserProject(Guid userGuid, Guid projectGuid)
        {
            string sql = "SELECT * FROM USER_PROJECT WHERE USERGUID = '" + userGuid + "'";
            sql += " AND PROJECTGUID = '" + projectGuid + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_PROJECTDataTable dtUserProject = new dsUSER_MAIN.USER_PROJECTDataTable();
            ExecuteQuery(sql, dtUserProject);
            
            if(dtUserProject != null)
            {
                dsUSER_MAIN.USER_PROJECTRow drUserProject = dtUserProject[0];
                drUserProject.DELETED = DateTime.Now;
                drUserProject.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drUserProject);

                //if the default project is the project we are removing authorisation for
                dsUSER_MAIN.USER_MAINRow drUser = GetBy(userGuid);
                if (drUser != null && drUser.DPROJECT == projectGuid)
                {
                    drUser.DPROJECT = Guid.Empty;
                    Save(drUser);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets all user project
        /// </summary>
        public dsUSER_MAIN.USER_PROJECTDataTable GetUserProjects(Guid userGuid)
        {
            string sql = "SELECT * FROM USER_PROJECT WHERE USERGUID = '" + userGuid + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_PROJECTDataTable dtUserProject = new dsUSER_MAIN.USER_PROJECTDataTable();
            ExecuteQuery(sql, dtUserProject);

            if (dtUserProject.Rows.Count > 0)
                return dtUserProject;
            else
                return null;
        }

        /// <summary>
        /// Gets deleted user project for purging
        /// </summary>
        public dsUSER_MAIN.USER_PROJECTDataTable Get_Deleted_UserProjects()
        {
            string sql = "SELECT * FROM USER_PROJECT WHERE DELETED IS NOT NULL";

            dsUSER_MAIN.USER_PROJECTDataTable dtUserProject = new dsUSER_MAIN.USER_PROJECTDataTable();
            ExecuteQuery(sql, dtUserProject);

            if (dtUserProject.Rows.Count > 0)
                return dtUserProject;
            else
                return null;
        }

        /// <summary>
        /// Get a particular user and project combination
        /// </summary>
        public dsUSER_MAIN.USER_PROJECTRow GetUserProject(Guid userGuid, Guid projectGuid)
        {
            string sql = "SELECT * FROM USER_PROJECT WHERE USERGUID = '" + userGuid + "'";
            sql += " AND PROJECTGUID = '" + projectGuid + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_PROJECTDataTable dtUserProject = new dsUSER_MAIN.USER_PROJECTDataTable();
            ExecuteQuery(sql, dtUserProject);

            if (dtUserProject.Rows.Count > 0)
                return dtUserProject[0];
            else
                return null;
        }

        /// <summary>
        /// Get all authorised projects for a particular user
        /// </summary>
        public dsUSER_MAIN.USER_PROJECTDataTable GetAuthProjects()
        {
            string sql = "SELECT * FROM USER_PROJECT userProj JOIN PROJECT proj ON (userProj.PROJECTGUID = proj.GUID) WHERE USERGUID = '" + System_Environment.GetUser().GUID + "' AND ";
            sql += "userProj.DELETED IS NULL AND proj.DELETED IS NULL ORDER BY proj.NUMBER";

            dsUSER_MAIN.USER_PROJECTDataTable dtUserProject = new dsUSER_MAIN.USER_PROJECTDataTable();
            ExecuteQuery(sql, dtUserProject);

            if (dtUserProject.Rows.Count > 0)
                return dtUserProject;
            else
                return null;
        }

        /// <summary>
        /// Update user project
        /// </summary>
        public void Save(dsUSER_MAIN.USER_PROJECTRow drUserProject)
        {
            _adapterUserProject.Update(drUserProject);
        }

        public void Save(dsUSER_MAIN.USER_PROJECTDataTable dtUserProject)
        {
            _adapterUserProject.Update(dtUserProject);
        }
        #endregion

        #region User Discipline
        /// <summary>
        /// Add user discipline authorisation
        /// </summary>
        public void SetUserDiscipline(Guid userGuid, string discipline)
        {
            dsUSER_MAIN dsUser = new dsUSER_MAIN();
            dsUSER_MAIN.USER_DISCRow drUserDiscipline = dsUser.USER_DISC.NewUSER_DISCRow();

            drUserDiscipline.GUID = Guid.NewGuid();
            drUserDiscipline.USERGUID = userGuid;
            drUserDiscipline.DISCIPLINE = discipline;
            drUserDiscipline.CREATED = DateTime.Now;
            drUserDiscipline.CREATEDBY = System_Environment.GetUser().GUID;

            dsUser.USER_DISC.AddUSER_DISCRow(drUserDiscipline);
            Save(drUserDiscipline);
        }

        public dsUSER_MAIN.USER_DISCDataTable GetUserDisciplines(Guid userGuid)
        {
            string sql = "SELECT * FROM USER_DISC";
            sql += " WHERE USERGUID = '" + userGuid + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_DISCDataTable dtUserDisc = new dsUSER_MAIN.USER_DISCDataTable();
            ExecuteQuery(sql, dtUserDisc);

            if (dtUserDisc != null)
                return dtUserDisc;
            else
                return null;
        }

        /// <summary>
        /// Get Deleted User Disciplines for Purging
        /// </summary>
        public dsUSER_MAIN.USER_DISCDataTable Get_Deleted_UserDisciplines()
        {
            string sql = "SELECT * FROM USER_DISC WHERE DELETED IS NOT NULL";

            dsUSER_MAIN.USER_DISCDataTable dtUserDisc = new dsUSER_MAIN.USER_DISCDataTable();
            ExecuteQuery(sql, dtUserDisc);

            if (dtUserDisc != null)
                return dtUserDisc;
            else
                return null;
        }

        public dsUSER_MAIN.USER_DISCRow CheckUserDiscipline(Guid userGuid, string discipline)
        {
            string sql = "SELECT * FROM USER_DISC";
            sql += " WHERE USERGUID = '" + userGuid + "'";
            sql += " AND DISCIPLINE = '" + discipline + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_DISCDataTable dtUserDisc = new dsUSER_MAIN.USER_DISCDataTable();
            ExecuteQuery(sql, dtUserDisc);

            if (dtUserDisc != null)
                return dtUserDisc[0];
            else
                return null;
        }

        /// <summary>
        /// Remove user discipline authorisation
        /// </summary>
        /// <param name="deleteUserGuid">Deleting user guid</param>
        public bool RemoveUserDiscipline(Guid userGuid, string discipline)
        {
            string sql = "SELECT * FROM USER_DISC WHERE USERGUID = '" + userGuid + "'";
            sql += " AND DISCIPLINE = '" + discipline + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_DISCDataTable dtUserDisc = new dsUSER_MAIN.USER_DISCDataTable();
            ExecuteQuery(sql, dtUserDisc);

            if(dtUserDisc != null)
            {
                dsUSER_MAIN.USER_DISCRow drUserDisc = dtUserDisc[0];
                drUserDisc.DELETED = DateTime.Now;
                drUserDisc.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drUserDisc);

                //if the default discipine is the discipline we are removing authorisation for
                dsUSER_MAIN.USER_MAINRow drUser = GetBy(userGuid);
                if (drUser != null && drUser.DDISCIPLINE == discipline)
                {
                    drUser.DDISCIPLINE = string.Empty; ;
                    Save(drUser);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get all authorised discipline for a particular user
        /// </summary>
        public dsUSER_MAIN.USER_DISCDataTable GetAuthDisciplines()
        {
            string sql = "SELECT * FROM USER_DISC WHERE USERGUID = '" + System_Environment.GetUser().GUID + "'";
            sql += " AND DELETED IS NULL";

            dsUSER_MAIN.USER_DISCDataTable dtUserDiscipline = new dsUSER_MAIN.USER_DISCDataTable();
            ExecuteQuery(sql, dtUserDiscipline);

            if (dtUserDiscipline.Rows.Count > 0)
                return dtUserDiscipline;
            else
                return null;
        }

        /// <summary>
        /// Update user discipline
        /// </summary>
        public void Save(dsUSER_MAIN.USER_DISCRow drUserDiscipline)
        {
            _adapterUserDiscipline.Update(drUserDiscipline);
        }

        public void Save(dsUSER_MAIN.USER_DISCDataTable dtUserDiscipline)
        {
            _adapterUserDiscipline.Update(dtUserDiscipline);
        }
        #endregion

        public void Dispose()
        {
            if (_adapter != null)
            {
                _adapter.Dispose();
                _adapter = null;
            }
            if (_adapterUserProject != null)
            {
                _adapterUserProject.Dispose();
                _adapterUserProject = null;
            }
            if (_adapterUserDiscipline != null)
            {
                _adapterUserDiscipline.Dispose();
                _adapterUserDiscipline = null;
            }
        }
    }
}
