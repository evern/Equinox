using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsMATRIX_ASSIGNMENTTableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterMATRIX_ASSIGNMENT : SQLBase, IDisposable
    {
        private MATRIX_ASSIGNMENTTableAdapter _adapter;

        public AdapterMATRIX_ASSIGNMENT()
            : base()
        {
            _adapter = new MATRIX_ASSIGNMENTTableAdapter(Variables.ConnStr);
        }

        public AdapterMATRIX_ASSIGNMENT(string connStr)
            : base(connStr)
        {
            _adapter = new MATRIX_ASSIGNMENTTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all matrix assignment
        /// </summary>
        public dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable Get()
        {
            string sql = "SELECT * FROM MATRIX_ASSIGNMENT";

            dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMatrixType = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable();
            ExecuteQuery(sql, dtMatrixType);

            if (dtMatrixType.Rows.Count > 0)
                return dtMatrixType;
            else
                return null;
        }

        /// <summary>
        /// Get deleted matrix type for purging
        /// </summary>
        public dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable Get_Deleted(Guid projectGuid)
        {
            string sql = "SELECT * FROM MATRIX_ASSIGNMENT WHERE DELETED IS NOT NULL AND GUID_PROJECT = '" + projectGuid.ToString() + "'";

            dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMatrixType = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable();
            ExecuteQuery(sql, dtMatrixType);

            if (dtMatrixType.Rows.Count > 0)
                return dtMatrixType;
            else
                return null;
        }

        /// <summary>
        /// Get all matrix type
        /// </summary>
        public dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable GetBy_Type(Guid ProjectGUID, Guid TypeGUID)
        {
            string sql = "SELECT * FROM MATRIX_ASSIGNMENT WHERE GUID_MATRIX_TYPE = '" + TypeGUID + "' AND GUID_PROJECT = '" + ProjectGUID + "'";
            sql += " AND DELETED IS NULL";

            dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMatrixType = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable();
            ExecuteQuery(sql, dtMatrixType);

            if (dtMatrixType.Rows.Count > 0)
                return dtMatrixType;
            else
                return null;
        }

        /// <summary>
        /// Get all matrix type
        /// </summary>
        public dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow GetBy_Type_And_Template(Guid ProjectGUID, Guid TypeGUID, Guid TemplateGuid)
        {
            string sql = "SELECT * FROM MATRIX_ASSIGNMENT WHERE GUID_MATRIX_TYPE = '" + TypeGUID + "' AND GUID_PROJECT = '" + ProjectGUID + "'";
            sql += " AND GUID_TEMPLATE = '" + TemplateGuid + "' AND DELETED IS NULL";

            dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMatrixType = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable();
            ExecuteQuery(sql, dtMatrixType);

            if (dtMatrixType.Rows.Count > 0)
                return dtMatrixType[0];
            else
                return null;
        }

        public dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPEDataTable Get_Assignments_Table(Guid ProjectGuid)
        {
            string sql = "SELECT Type.Name AS TYPE, Mass.* FROM MATRIX_ASSIGNMENT Mass JOIN MATRIX_TYPE Type ON (Type.GUID = Mass.GUID_MATRIX_TYPE)";
            sql += " WHERE Type.DELETED IS NULL AND Mass.GUID_PROJECT = '" + ProjectGuid + "' AND Mass.DELETED IS NULL";

            dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPEDataTable dtMATRIX = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPEDataTable();
            ExecuteQuery(sql, dtMATRIX);

            if (dtMATRIX.Rows.Count > 0)
                return dtMATRIX;
            else
                return null;
        }


        /// <summary>
        /// Get specific matrix type by guid
        /// </summary>
        public dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow GetBy(Guid guid)
        {
            string sql = "SELECT * FROM MATRIX_ASSIGNMENT ";
            sql += "WHERE GUID = '" + guid + "'";

            dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMATRIX_TYPE = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable();
            ExecuteQuery(sql, dtMATRIX_TYPE);

            if (dtMATRIX_TYPE.Rows.Count > 0)
                return dtMATRIX_TYPE[0];
            else
                return null;
        }

        /// <summary>
        /// Get specific matrix type by guid
        /// </summary>
        public dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable GetByProject(Guid project_guid)
        {
            string sql = "SELECT * FROM MATRIX_ASSIGNMENT ";
            sql += "WHERE GUID_PROJECT = '" + project_guid + "' AND DELETED IS NULL";

            dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMATRIX_TYPE = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable();
            ExecuteQuery(sql, dtMATRIX_TYPE);

            if (dtMATRIX_TYPE.Rows.Count > 0)
                return dtMATRIX_TYPE;
            else
                return null;
        }

        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM MATRIX_ASSIGNMENT ";
            sql += "WHERE GUID = '" + guid + "'";

            dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMATRIX_TYPE = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable();
            ExecuteQuery(sql, dtMATRIX_TYPE);

            if (dtMATRIX_TYPE.Rows.Count > 0)
            {
                dtMATRIX_TYPE[0].DELETED = DateTime.Now;
                dtMATRIX_TYPE[0].DELETEDBY = System_Environment.GetUser().GUID;
                Save(dtMATRIX_TYPE[0]);
                return true;
            }
            else
                return false;
        }

        public bool RemoveByProject(Guid guid_project)
        {
            string sql = "SELECT * FROM MATRIX_ASSIGNMENT ";
            sql += "WHERE GUID_PROJECT = '" + guid_project + "'";

            dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMATRIX_TYPE = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable();
            ExecuteQuery(sql, dtMATRIX_TYPE);

            if (dtMATRIX_TYPE.Rows.Count > 0)
            {
                foreach(dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow matrixRow in dtMATRIX_TYPE.Rows)
                {
                    matrixRow.DELETED = DateTime.Now;
                    matrixRow.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(matrixRow);
                }

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Update multiple syncs
        /// </summary>
        public void Save(dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMATRIX_ASSIGNMENT)
        {
            _adapter.Update(dtMATRIX_ASSIGNMENT);
        }

        /// <summary>
        /// Update one sync
        /// </summary>
        public void Save(dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow drMATRIX_ASSIGNMENT)
        {
            _adapter.Update(drMATRIX_ASSIGNMENT);
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
