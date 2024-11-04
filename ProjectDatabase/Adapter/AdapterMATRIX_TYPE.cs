using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsMATRIX_TYPETableAdapters;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterMATRIX_TYPE : SQLBase, IDisposable
    {
        private MATRIX_TYPETableAdapter _adapter;

        public AdapterMATRIX_TYPE()
            : base()
        {
            _adapter = new MATRIX_TYPETableAdapter(Variables.ConnStr);
        }

        public AdapterMATRIX_TYPE(string connStr)
            : base(connStr)
        {
            _adapter = new MATRIX_TYPETableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all matrix type
        /// </summary>
        public dsMATRIX_TYPE.MATRIX_TYPEDataTable Get()
        {
            string sql = "SELECT * FROM MATRIX_TYPE WHERE DELETED IS NULL ORDER BY NAME";

            dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMatrixType = new dsMATRIX_TYPE.MATRIX_TYPEDataTable();
            ExecuteQuery(sql, dtMatrixType);

            if (dtMatrixType.Rows.Count > 0)
                return dtMatrixType;
            else
                return null;
        }

        public dsMATRIX_TYPE.MatrixReportDataTable GetReport(Guid projectGuid)
        {
            string sql = "SELECT MATRIX_TYPE_NAME = MATRIX_TYPE.NAME, MATRIX_TYPE.DESCRIPTION, MATRIX_TYPE.DISCIPLINE, TEMPLATE_NAME = TEMPLATE_MAIN.NAME ";
            sql += "FROM MATRIX_ASSIGNMENT ";
            sql += "JOIN MATRIX_TYPE ON MATRIX_TYPE.GUID = MATRIX_ASSIGNMENT.GUID_MATRIX_TYPE ";
            sql += "JOIN TEMPLATE_MAIN ON TEMPLATE_MAIN.GUID = MATRIX_ASSIGNMENT.GUID_TEMPLATE ";
            sql += "WHERE MATRIX_ASSIGNMENT.GUID_PROJECT = '" + projectGuid + "' ";

            dsMATRIX_TYPE.MatrixReportDataTable dtMatrixReport = new dsMATRIX_TYPE.MatrixReportDataTable();
            ExecuteQuery(sql, dtMatrixReport);

            return dtMatrixReport;
        }

        /// <summary>
        /// Get deleted entries for purging
        /// </summary>
        /// <returns></returns>
        public dsMATRIX_TYPE.MATRIX_TYPEDataTable Get_Deleted(string discipline)
        {
            string sql = "SELECT * FROM MATRIX_TYPE WHERE DELETED IS NOT NULL AND DISCIPLINE = '" + discipline + "'";

            dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMatrixType = new dsMATRIX_TYPE.MATRIX_TYPEDataTable();
            ExecuteQuery(sql, dtMatrixType);

            if (dtMatrixType.Rows.Count > 0)
                return dtMatrixType;
            else
                return null;
        }

        /// <summary>
        /// Get specific matrix type by guid
        /// </summary>
        public dsMATRIX_TYPE.MATRIX_TYPERow GetBy(Guid TypeGuid)
        {
            string sql = "SELECT * FROM MATRIX_TYPE ";
            sql += "WHERE GUID = '" + TypeGuid + "'";

            dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMatrixType = new dsMATRIX_TYPE.MATRIX_TYPEDataTable();
            ExecuteQuery(sql, dtMatrixType);

            if (dtMatrixType.Rows.Count > 0)
                return dtMatrixType[0];
            else
                return null;
        }

        /// <summary>
        /// Get specific matrix type by name
        /// </summary>
        public dsMATRIX_TYPE.MATRIX_TYPERow GetBy(string typeName)
        {
            string sql = "SELECT * FROM MATRIX_TYPE ";
            sql += "WHERE NAME = '" + typeName + "' AND DELETED IS NULL";

            dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMatrixType = new dsMATRIX_TYPE.MATRIX_TYPEDataTable();
            ExecuteQuery(sql, dtMatrixType);

            if (dtMatrixType.Rows.Count > 0)
                return dtMatrixType[0];
            else
                return null;
        }

        public dsMATRIX_TYPE.MATRIX_TYPEDataTable GetBy_Discipline(string discipline_name)
        {
            string sql = "SELECT * FROM MATRIX_TYPE ";
            sql += "WHERE DISCIPLINE = '" + discipline_name + "' AND DELETED IS NULL";

            dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMatrixType = new dsMATRIX_TYPE.MATRIX_TYPEDataTable();
            ExecuteQuery(sql, dtMatrixType);

            if (dtMatrixType.Rows.Count > 0)
                return dtMatrixType;
            else
                return null;
        }

        public bool RemoveBy(Guid typeGuid)
        {
            string sql = "SELECT * FROM MATRIX_TYPE ";
            sql += "WHERE GUID = '" + typeGuid + "'";

            dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMATRIXTYPE = new dsMATRIX_TYPE.MATRIX_TYPEDataTable();
            ExecuteQuery(sql, dtMATRIXTYPE);

            if (dtMATRIXTYPE != null)
            {
                dtMATRIXTYPE[0].DELETED = DateTime.Now;
                dtMATRIXTYPE[0].DELETEDBY = System_Environment.GetUser().GUID;
                Save(dtMATRIXTYPE[0]);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Update multiple syncs
        /// </summary>
        public void Save(dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMATRIX_TYPE)
        {
            _adapter.Update(dtMATRIX_TYPE);
        }

        /// <summary>
        /// Update one sync
        /// </summary>
        public void Save(dsMATRIX_TYPE.MATRIX_TYPERow drMATRIX_TYPE)
        {
            _adapter.Update(drMATRIX_TYPE);
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
