using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsTEMPLATE_REGISTERTableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterTEMPLATE_REGISTER : SQLBase, IDisposable
    {
        private TEMPLATE_REGISTERTableAdapter _adapter;

        public AdapterTEMPLATE_REGISTER()
            : base()
        {
            _adapter = new TEMPLATE_REGISTERTableAdapter(Variables.ConnStr);
        }

        public AdapterTEMPLATE_REGISTER(string connStr)
            : base(connStr)
        {
            _adapter = new TEMPLATE_REGISTERTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all template register in the system
        /// </summary>
        public dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable Get()
        {
            string sql = "SELECT * FROM TEMPLATE_REGISTER WHERE DELETED IS NULL";
            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
            ExecuteQuery(sql, dtTEMPLATE_REGISTER);

            if (dtTEMPLATE_REGISTER.Rows.Count > 0)
                return dtTEMPLATE_REGISTER;
            else
                return null;
        }

        /// <summary>
        /// Get deleted template register in the system for purging
        /// </summary>
        public dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable Get_Deleted(Guid ProjectGuid)
        {
            string sql = "SELECT TemplateRegister.* FROM TEMPLATE_REGISTER TemplateRegister ";
            sql += "LEFT OUTER JOIN TAG Tag ON (Tag.GUID = TemplateRegister.TAG_GUID) ";
            sql += "LEFT OUTER JOIN WBS Wbs ON (Wbs.GUID = TemplateRegister.WBS_GUID) ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID OR Schedule.GUID = Wbs.SCHEDULEGUID) ";
            sql += "JOIN PROJECT Project ON (Project.GUID = Schedule.PROJECTGUID) ";
            sql += "WHERE TemplateRegister.DELETED IS NOT NULL AND Project.GUID = '" + ProjectGuid.ToString() + "'";

            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
            ExecuteQuery(sql, dtTEMPLATE_REGISTER);

            if (dtTEMPLATE_REGISTER.Rows.Count > 0)
                return dtTEMPLATE_REGISTER;
            else
                return null;
        }

        /// <summary>
        /// Get template register by guid
        /// </summary>
        public dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow GetBy(Guid guid)
        {
            string sql = "SELECT * FROM TEMPLATE_REGISTER WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
            ExecuteQuery(sql, dtTEMPLATE_REGISTER);

            if (dtTEMPLATE_REGISTER.Rows.Count > 0)
                return dtTEMPLATE_REGISTER[0];
            else
                return null;
        }

        /// <summary>
        /// Gets template register by schedule guid
        /// </summary>
        public dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable GetBySchedule(Guid ScheduleGuid)
        {
            string sql = "SELECT TemplateRegister.* FROM TEMPLATE_REGISTER TemplateRegister ";
            sql += "LEFT OUTER JOIN TAG Tag ON (Tag.GUID = TemplateRegister.TAG_GUID) ";
            sql += "LEFT OUTER JOIN WBS Wbs ON (Wbs.GUID = TemplateRegister.WBS_GUID) ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.GUID = Tag.SCHEDULEGUID OR Schedule.GUID = Wbs.SCHEDULEGUID) ";
            sql += "WHERE TemplateRegister.DELETED IS NULL AND Schedule.GUID = '" + ScheduleGuid.ToString() + "'";

            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
            ExecuteQuery(sql, dtTEMPLATE_REGISTER);

            if (dtTEMPLATE_REGISTER.Rows.Count > 0)
                return dtTEMPLATE_REGISTER;
            else
                return null;
        }

        /// <summary>
        /// Get template register by template guid
        /// </summary>
        public dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable GetByTemplate(Guid templateGuid)
        {
            string sql = "SELECT * FROM TEMPLATE_REGISTER WHERE TEMPLATE_GUID = '" + templateGuid + "'";
            sql += " AND DELETED IS NULL";

            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
            ExecuteQuery(sql, dtTEMPLATE_REGISTER);

            if (dtTEMPLATE_REGISTER.Rows.Count > 0)
                return dtTEMPLATE_REGISTER;
            else
                return null;
        }

        /// <summary>
        /// Get template register by wbs/tag
        /// </summary>
        public dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable GetByWBSTag(wbsTagDisplay wbsTag)
        {
            string sql = "SELECT * FROM TEMPLATE_REGISTER WHERE ";
            if(wbsTag.wbsTagDisplayAttachTag != null)
            {
                sql += "TAG_GUID = '" + wbsTag.wbsTagDisplayAttachTag.GUID + "'";
            }
            else
                sql += "WBS_GUID = '" + wbsTag.wbsTagDisplayAttachWBS.GUID + "'";

            sql += " AND DELETED IS NULL";

            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
            ExecuteQuery(sql, dtTEMPLATE_REGISTER);

            if (dtTEMPLATE_REGISTER.Rows.Count > 0)
                return dtTEMPLATE_REGISTER;
            else
                return null;
        }

        /// <summary>
        /// Get template register by wbs/tag Guid
        /// </summary>
        public dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable GetByWBSTagGuid(Guid wbsTagGuid)
        {
            string sql = "SELECT * FROM TEMPLATE_REGISTER WHERE ";
            sql += "(TAG_GUID = '" + wbsTagGuid + "' OR WBS_GUID = '" + wbsTagGuid + "')";
            sql += " AND DELETED IS NULL";

            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
            ExecuteQuery(sql, dtTEMPLATE_REGISTER);

            if (dtTEMPLATE_REGISTER.Rows.Count > 0)
                return dtTEMPLATE_REGISTER;
            else
                return null;
        }

        public bool SetNull_OnWBSTag(wbsTagDisplay wbsTag)
        {
            string sql = "UPDATE TEMPLATE_REGISTER SET DELETED = CURRENT_TIMESTAMP, DELETEDBY = '" + System_Environment.GetUser().GUID + "'";

            if (wbsTag.wbsTagDisplayAttachTag != null)
                sql += " WHERE TAG_GUID = '" + wbsTag.wbsTagDisplayAttachTag.GUID + "'";
            else
                sql += " WHERE WBS_GUID = '" + wbsTag.wbsTagDisplayAttachWBS.GUID + "'";

            int Result = ExecuteNonQuery(sql);

            if (Result > 0)
                return true;
            else
                return false;
        }


        public bool SetNull_OnTag(Guid tagGuid)
        {
            string sql = "UPDATE TEMPLATE_REGISTER SET DELETED = CURRENT_TIMESTAMP, DELETEDBY = '" + System_Environment.GetUser().GUID + "'";
            sql += " WHERE TAG_GUID = '" + tagGuid + "'";

            int Result = ExecuteNonQuery(sql);

            if (Result > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Tag a wbs/tag to a template
        /// </summary>
        public void AssignTagTemplate(Guid templateGuid, Guid tagGuid)
        {
            string sql = "SELECT * FROM TEMPLATE_REGISTER WHERE TEMPLATE_GUID = '" + templateGuid + "'";
            sql += " AND TAG_GUID = '" + tagGuid + "'";
            sql += " AND DELETED IS NOT NULL ORDER BY CREATED DESC";

            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTemplateRegister = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
            ExecuteQuery(sql, dtTemplateRegister);

            if (dtTemplateRegister.Rows.Count > 0)
            {
                dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTemplateRegister = dtTemplateRegister.First();
                //undelete operation here is sync safe because the delete operation is executed within the same routine
                drTemplateRegister.SetDELETEDNull();
                drTemplateRegister.SetDELETEDBYNull();
                Save(drTemplateRegister);
            }
            else
            {
                dsTEMPLATE_REGISTER dsTemplateRegister = new dsTEMPLATE_REGISTER();
                dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTemplateRegister = dsTemplateRegister.TEMPLATE_REGISTER.NewTEMPLATE_REGISTERRow();

                drTemplateRegister.GUID = Guid.NewGuid();
                drTemplateRegister.TEMPLATE_GUID = templateGuid;
                drTemplateRegister.TAG_GUID = tagGuid;
                drTemplateRegister.CREATED = DateTime.Now;
                drTemplateRegister.CREATEDBY = System_Environment.GetUser().GUID;

                dsTemplateRegister.TEMPLATE_REGISTER.AddTEMPLATE_REGISTERRow(drTemplateRegister);
                Save(drTemplateRegister);
            }
        }

        /// <summary>
        /// Tag a wbs/tag to a template
        /// </summary>
        public void AssignTagWBSTemplate(Guid templateGuid, wbsTagDisplay wbsTag)
        {
            string sql = "SELECT * FROM TEMPLATE_REGISTER WHERE TEMPLATE_GUID = '" + templateGuid + "'";

            if (wbsTag.wbsTagDisplayAttachTag != null)
                sql += " AND TAG_GUID = '" + wbsTag.wbsTagDisplayAttachTag.GUID + "'";
            else
                sql += " AND WBS_GUID = '" + wbsTag.wbsTagDisplayAttachWBS.GUID + "'";

            sql += " ORDER BY CREATED DESC";

            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTemplateRegister = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
            ExecuteQuery(sql, dtTemplateRegister);

            if (dtTemplateRegister.Rows.Count > 0)
            {
                dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTemplateRegister = dtTemplateRegister.First();

                if(!drTemplateRegister.IsDELETEDNull())
                {
                    //undelete operation here is sync safe because the delete operation is executed within the same routine
                    drTemplateRegister.SetDELETEDNull();
                    drTemplateRegister.SetDELETEDBYNull();
                    drTemplateRegister.CREATED = DateTime.Now;
                    drTemplateRegister.CREATEDBY = System_Environment.GetUser().GUID;
                    Save(drTemplateRegister);
                }
                //else when it's found don't do anything
            }
            else
            {
                dsTEMPLATE_REGISTER dsTemplateRegister = new dsTEMPLATE_REGISTER();
                dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTemplateRegister = dsTemplateRegister.TEMPLATE_REGISTER.NewTEMPLATE_REGISTERRow();

                drTemplateRegister.GUID = Guid.NewGuid();
                drTemplateRegister.TEMPLATE_GUID = templateGuid;

                if (wbsTag.wbsTagDisplayAttachTag != null)
                    drTemplateRegister.TAG_GUID = wbsTag.wbsTagDisplayAttachTag.GUID;
                else
                    drTemplateRegister.WBS_GUID = wbsTag.wbsTagDisplayAttachWBS.GUID;

                drTemplateRegister.CREATED = DateTime.Now;
                drTemplateRegister.CREATEDBY = System_Environment.GetUser().GUID;

                dsTemplateRegister.TEMPLATE_REGISTER.AddTEMPLATE_REGISTERRow(drTemplateRegister);
                Save(drTemplateRegister);
            }
        }

        /// <summary>
        /// Remove a wbs/tag template combination from register
        /// </summary>
        public bool UntagTemplate(Guid templateGuid, wbsTagDisplay wbsTag)
        {
            string sql = "SELECT * FROM TEMPLATE_REGISTER WHERE TEMPLATE_GUID = '" + templateGuid + "'";

            if (wbsTag.wbsTagDisplayAttachTag != null)
                sql += " AND TAG_GUID = '" + wbsTag.wbsTagDisplayAttachTag.GUID + "'";
            else
                sql += " AND WBS_GUID = '" + wbsTag.wbsTagDisplayAttachWBS.GUID + "'";

            sql += " AND DELETED IS NULL";

            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTemplateRegister = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
            ExecuteQuery(sql, dtTemplateRegister);

            if (dtTemplateRegister.Rows.Count > 0)
            {
                dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTemplateRegister = dtTemplateRegister[0];
                drTemplateRegister.DELETED = DateTime.Now;
                drTemplateRegister.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drTemplateRegister);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove a wbs/tag template combination from register
        /// </summary>
        public bool UntagTemplateByGuid(Guid wbsTagGuid, Guid templateGuid)
        {
            string sql = "SELECT * FROM TEMPLATE_REGISTER WHERE TEMPLATE_GUID = '" + templateGuid + "'";
            sql += " AND (WBS_GUID = '" + wbsTagGuid + "' OR TAG_GUID = '" + wbsTagGuid + "')";
            sql += " AND DELETED IS NULL";

            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTemplateRegister = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
            ExecuteQuery(sql, dtTemplateRegister);

            if (dtTemplateRegister.Rows.Count > 0)
            {
                dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTemplateRegister = dtTemplateRegister[0];
                drTemplateRegister.DELETED = DateTime.Now;
                drTemplateRegister.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drTemplateRegister);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove a particular template register by GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM TEMPLATE_REGISTER WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTemplate = new dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable();
            ExecuteQuery(sql, dtTemplate);

            if (dtTemplate.Rows.Count > 0)
            {
                dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTemplate = dtTemplate[0];
                drTemplate.DELETED = DateTime.Now;
                drTemplate.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drTemplate);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update multiple template registers
        /// </summary>
        public void Save(dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER)
        {
            _adapter.Update(dtTEMPLATE_REGISTER);
        }

        /// <summary>
        /// Update one template register
        /// </summary>
        public void Save(dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTEMPLATE_REGISTER)
        {
            _adapter.Update(drTEMPLATE_REGISTER);
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