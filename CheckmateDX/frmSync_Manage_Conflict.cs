using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ProjectCommon;
using ProjectDatabase;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectLibrary;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Controls;

namespace CheckmateDX
{
    public partial class frmSync_Manage_Conflict : frmParent
    {
        List<Conflict_Table> _Conflict_Tables = new List<Conflict_Table>();
        AdapterSYNC_CONFLICT _daSYNC_CONFLICT = new AdapterSYNC_CONFLICT();
        dsSYNC_PAIR.SYNC_PAIRDataTable _dtSYNC_PAIR = new dsSYNC_PAIR.SYNC_PAIRDataTable();
        dsUSER_MAIN.USER_MAINDataTable _dtUSER = new dsUSER_MAIN.USER_MAINDataTable();

        enum Custom_General_Column_Name { RESOLVE_GUID, CONFLICT_ON_GUID, MACHINE_NAME, CREATED_BY, UPDATED_BY, SYNCED, SYNCED_BY };
        enum General_Hidden_Column_Register { RESOLVE_GUID, CONFLICT_ON_GUID, CONFLICT_GUID, GUID, CREATED, CREATEDBY, CREATED_BY, UPDATED, UPDATEDBY, UPDATED_BY, DELETED, DELETEDBY };
        enum Custom_Column_Name { ITR_DOC, TAG_NUMBER, WBS_NAME, ITR_NAME, TEMPLATE_DOC, USER_SIGNATURE };
        
        string DoubleClickViewContent = "Double Click to View";

        SQLBase _DataAdapter = new SQLBase();

        public frmSync_Manage_Conflict()
        {
            InitializeComponent();
            Cache_Sync_Descriptions();
            Cache_UserNames();
            Refresh_Conflict_Table();
            Populate_Repository();
        }

        #region Populate Data
        private void Refresh_Conflict_Table()
        {
            _Conflict_Tables.Clear();
            _Conflict_Tables = _daSYNC_CONFLICT.Get_Unresolved_AsList();

            gridControlTable.DataSource = _Conflict_Tables; //have to reassign the list because it's a different list coming out of the adapter
        }

        /// <summary>
        /// Refreshes the typed data table
        /// </summary>
        private void Refresh_Data_Table()
        {
            Conflict_Table SelectedConflictTable = (Conflict_Table)gridViewTable.GetFocusedRow();
            if (SelectedConflictTable != null)
            {
                Sync_Type SelectedSyncType = (Sync_Type)Enum.Parse(typeof(Sync_Type), SelectedConflictTable.TableName);
                Bind_And_Populate_Specific_DataTable(SelectedSyncType);
            }
            else
            {
                gridViewData.Columns.Clear();
                gridControlData.DataSource = null;
                gridViewData.RefreshData();
            }
        }

        /// <summary>
        /// Cache the sync description
        /// </summary>
        private void Cache_Sync_Descriptions()
        {
            using(AdapterSYNC_PAIR daSYNCPAIR = new AdapterSYNC_PAIR())
            {
                _dtSYNC_PAIR = daSYNCPAIR.GetAll();
            }
        }

        /// <summary>
        /// Cache the user names
        /// </summary>
        private void Cache_UserNames()
        {
            using(AdapterUSER_MAIN daUSERMAIN = new AdapterUSER_MAIN())
            {
                _dtUSER = daUSERMAIN.GetAll();
            }
        }

        /// <summary>
        /// Get the sync description
        /// </summary>
        private string Get_Sync_Description(string HWID)
        {
            dsSYNC_PAIR.SYNC_PAIRRow drFIND_SYNC_PAIR = _dtSYNC_PAIR.FirstOrDefault(obj => obj.HWID == HWID);
            if (drFIND_SYNC_PAIR != null)
                return drFIND_SYNC_PAIR.DESCRIPTION;
            else
                return string.Empty;
        }

        private string Get_UserName(Guid UserGUID)
        {
            if(UserGUID == Guid.Empty)
                return Variables.AdminSuperuser;

            dsUSER_MAIN.USER_MAINRow drFINDUSER = _dtUSER.FirstOrDefault(obj => obj.GUID == UserGUID);
            if (drFINDUSER != null)
                return drFINDUSER.FIRSTNAME + " " + drFINDUSER.LASTNAME;
            else
                return string.Empty;
        }

        /// <summary>
        /// Populate repository used within gridview
        /// </summary>
        private void Populate_Repository()
        {
            repositoryItemImageComboBox1.Items.Add(new ImageComboBoxItem(DoubleClickViewContent, 0));
        }

        private void Bind_And_Populate_Specific_DataTable(Sync_Type SyncType)
        {
            gridViewData.Columns.Clear();
            gridControlData.DataSource = null;
            gridViewData.OptionsEditForm.EditFormColumnCount = 2; //default edit form column count

            List<ValuePair> Hidden_Columns = new List<ValuePair>();
            List<string> Highlight_Columns = new List<string>();

            foreach (General_Hidden_Column_Register General_Hidden_Column in Enum.GetValues(typeof(General_Hidden_Column_Register)))
            {
                if (General_Hidden_Column == General_Hidden_Column_Register.CREATEDBY || General_Hidden_Column == General_Hidden_Column_Register.UPDATEDBY
                    || General_Hidden_Column == General_Hidden_Column_Register.DELETEDBY || General_Hidden_Column == General_Hidden_Column_Register.GUID)
                    Hidden_Columns.Add(new ValuePair(General_Hidden_Column.ToString(), false));
                else
                    Hidden_Columns.Add(new ValuePair(General_Hidden_Column.ToString(), true));
            }

            DataTable dtCONFLICT = new DataTable();
            dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtSYNC_CONFLICT = _daSYNC_CONFLICT.GetUnresolvedBy_Type(SyncType.ToString());

            if (dtSYNC_CONFLICT == null)
                return;

            foreach (dsSYNC_CONFLICT.SYNC_CONFLICTRow drSYNC_CONFLICT in dtSYNC_CONFLICT.Rows)
            {
                if (!dtCONFLICT.AsEnumerable().Any(obj => obj.Field<Guid>("GUID") == drSYNC_CONFLICT.CONFLICT_ON_GUID))
                {
                    Populate_Datatable(SyncType, ref dtCONFLICT, drSYNC_CONFLICT.CONFLICT_ON_GUID, drSYNC_CONFLICT.CONFLICT_ON_GUID, "SERVER", drSYNC_CONFLICT.CREATED, drSYNC_CONFLICT.CREATEDBY);
                }

                Populate_Datatable(SyncType, ref dtCONFLICT, drSYNC_CONFLICT.CONFLICT_GUID, drSYNC_CONFLICT.CONFLICT_ON_GUID, Get_Sync_Description(drSYNC_CONFLICT.CONFLICT_HWID), drSYNC_CONFLICT.CREATED, drSYNC_CONFLICT.CREATEDBY);
            }

            gridControlData.DataSource = dtCONFLICT;

            if (SyncType == Sync_Type.GENERAL_EQUIPMENT)
            {
                Highlight_Columns.Add("SERIAL");
                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);
            }
            else if (SyncType == Sync_Type.ITR_MAIN)
            {
                Hidden_Columns.Add(new ValuePair("ITR", false));
                Hidden_Columns.Add(new ValuePair("TEMPLATE_GUID", false));
                Hidden_Columns.Add(new ValuePair("SEQUENCE_NUMBER", false));
                Hidden_Columns.Add(new ValuePair("TYPE", false));

                dtCONFLICT.Columns.Add(Custom_Column_Name.ITR_DOC.ToString());

                Highlight_Columns.Add("NAME");

                Standard_WBS_Tag_Details_Population(SyncType, dtCONFLICT, Hidden_Columns, Highlight_Columns);

                foreach(DataRow drCONFLICT in dtCONFLICT.Rows)
                {
                    drCONFLICT[Custom_Column_Name.ITR_DOC.ToString()] = DoubleClickViewContent;
                }

                //Edit Form Customization
                gridViewData.OptionsEditForm.EditFormColumnCount = 1;
                foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridViewData.Columns)
                {
                    col.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
                }
                gridViewData.Columns[Custom_Column_Name.ITR_DOC.ToString()].ColumnEdit = repositoryItemImageComboBox1;
                gridViewData.Columns["ITR"].Visible = false;
                gridViewData.Columns["ITR"].OptionsEditForm.UseEditorColRowSpan = false;
                gridViewData.Columns["ITR"].OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.True;
                gridViewData.Columns["ITR"].OptionsEditForm.RowSpan = 20;
            }
            else if (SyncType == Sync_Type.PREFILL_MAIN)
            {
                Highlight_Columns.Add("NAME");
                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);
            }
            else if (SyncType == Sync_Type.PREFILL_REGISTER)
            {
                Standard_WBS_Tag_Details_Population(SyncType, dtCONFLICT, Hidden_Columns, Highlight_Columns);
            }
            else if (SyncType == Sync_Type.PROJECT)
            {
                Highlight_Columns.Add("NUMBER");
                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);
            }
            else if (SyncType == Sync_Type.PUNCHLIST_MAIN)
            {
                Hidden_Columns.Add(new ValuePair("ITR_GUID", false));
                dtCONFLICT.Columns.Add(Custom_Column_Name.ITR_NAME.ToString());
                Highlight_Columns.Add(Custom_Column_Name.ITR_NAME.ToString());

                Standard_WBS_Tag_Details_Population(SyncType, dtCONFLICT, Hidden_Columns, Highlight_Columns);

                foreach (DataRow drCONFLICT in dtCONFLICT.Rows)
                {
                    drCONFLICT[Custom_Column_Name.ITR_NAME.ToString()] = Common.ConvertITRGuidToTemplateName(drCONFLICT.Field<Guid>("ITR_GUID"));
                }

                gridViewData.Columns[Custom_Column_Name.ITR_NAME.ToString()].VisibleIndex = 2;
            }
            else if (SyncType == Sync_Type.ROLE_MAIN)
            {
                Hidden_Columns.Add(new ValuePair("PARENTGUID", false));
                dtCONFLICT.Columns.Add("PARENT_ROLE");
                Highlight_Columns.Add("NAME");

                foreach (DataRow drCONFLICT in dtCONFLICT.Rows)
                {
                    drCONFLICT["PARENT_ROLE"] = Common.ConvertRoleGuidToName(drCONFLICT.Field<Guid>("PARENTGUID"));
                }

                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);
            }
            else if (SyncType == Sync_Type.ROLE_PRIVILEGE)
            {
                Hidden_Columns.Add(new ValuePair("ROLEGUID", false));
                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);
            }
            else if (SyncType == Sync_Type.SCHEDULE)
            {
                Hidden_Columns.Add(new ValuePair("PROJECTGUID", false));
                dtCONFLICT.Columns.Add("PROJECT_NAME");
                Highlight_Columns.Add("NAME");

                foreach (DataRow drCONFLICT in dtCONFLICT.Rows)
                {
                    drCONFLICT["PROJECT_NAME"] = Common.ConvertProjectGuidToName(drCONFLICT.Field<Guid>("PROJECTGUID"));
                }

                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);
                gridViewData.Columns["PROJECT_NAME"].VisibleIndex = 0;
            }
            else if (SyncType == Sync_Type.TAG)
            {
                Hidden_Columns.Add(new ValuePair("SCHEDULEGUID", false));
                Hidden_Columns.Add(new ValuePair("PARENTGUID", false));
                Hidden_Columns.Add(new ValuePair("WBSGUID", false));

                dtCONFLICT.Columns.Add("SCHEDULE_NAME");
                dtCONFLICT.Columns.Add("PARENT_NAME");
                dtCONFLICT.Columns.Add("WBS_NAME");

                Highlight_Columns.Add("SCHEDULE_NAME");
                Highlight_Columns.Add("NUMBER");

                foreach (DataRow drCONFLICT in dtCONFLICT.Rows)
                {
                    drCONFLICT["SCHEDULE_NAME"] = Common.ConvertScheduleGuidToName(drCONFLICT.Field<Guid>("SCHEDULEGUID"));
                    drCONFLICT["PARENT_NAME"] = Common.ConvertTagGuidToName(drCONFLICT.Field<Guid>("PARENTGUID"));
                    drCONFLICT["WBS_NAME"] = Common.ConvertWbsGuidToName(drCONFLICT.Field<Guid>("WBSGUID"));
                }

                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);
                gridViewData.Columns["SCHEDULE_NAME"].VisibleIndex = 0;
                gridViewData.Columns["PARENT_NAME"].VisibleIndex = 1;
                gridViewData.Columns["WBS_NAME"].VisibleIndex = 2;
            }
            else if(SyncType == Sync_Type.TEMPLATE_MAIN)
            {
                Hidden_Columns.Add(new ValuePair("WORKFLOWGUID", false));

                dtCONFLICT.Columns.Add(Custom_Column_Name.TEMPLATE_DOC.ToString());
                dtCONFLICT.Columns.Add("WORKFLOW_NAME");

                Highlight_Columns.Add("NAME");

                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);

                foreach (DataRow drCONFLICT in dtCONFLICT.Rows)
                {
                    drCONFLICT["WORKFLOW_NAME"] = Common.ConvertWorkflowGuidToName(drCONFLICT.Field<Guid>("WORKFLOWGUID"));
                    drCONFLICT[Custom_Column_Name.TEMPLATE_DOC.ToString()] = DoubleClickViewContent;
                }

                //Edit Form Customization
                gridViewData.OptionsEditForm.EditFormColumnCount = 1;
                foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridViewData.Columns)
                {
                    col.OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
                }
                gridViewData.Columns[Custom_Column_Name.TEMPLATE_DOC.ToString()].ColumnEdit = repositoryItemImageComboBox1;
                gridViewData.Columns["TEMPLATE"].Visible = false;
                gridViewData.Columns["TEMPLATE"].OptionsEditForm.UseEditorColRowSpan = false;
                gridViewData.Columns["TEMPLATE"].OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.True;
                gridViewData.Columns["TEMPLATE"].OptionsEditForm.RowSpan = 20;
                gridViewData.Columns["WORKFLOW_NAME"].VisibleIndex = 0;
            }
            else if(SyncType == Sync_Type.TEMPLATE_TOGGLE)
            {
                Hidden_Columns.Add(new ValuePair("NAME", false));
                Hidden_Columns.Add(new ValuePair("DISCIPLINE", false));
                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);
            }
            else if(SyncType == Sync_Type.USER_MAIN)
            {
                Hidden_Columns.Add(new ValuePair("ROLE", false));
                Hidden_Columns.Add(new ValuePair("DPROJECT", false));

                dtCONFLICT.Columns.Add("USER_ROLE");
                dtCONFLICT.Columns.Add("DEFAULT_PROJECT");
                dtCONFLICT.Columns.Add(Custom_Column_Name.USER_SIGNATURE.ToString());

                Highlight_Columns.Add("QANUMBER");

                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);

                foreach (DataRow drCONFLICT in dtCONFLICT.Rows)
                {
                    drCONFLICT["USER_ROLE"] = Common.ConvertRoleGuidToName(drCONFLICT.Field<Guid>("ROLE"));
                    drCONFLICT["DEFAULT_PROJECT"] = Common.ConvertProjectGuidToName(drCONFLICT.Field<Guid>("DPROJECT"));
                    drCONFLICT[Custom_Column_Name.USER_SIGNATURE.ToString()] = DoubleClickViewContent;
                }

                gridViewData.OptionsEditForm.EditFormColumnCount = 3;
                gridViewData.Columns[Custom_Column_Name.USER_SIGNATURE.ToString()].ColumnEdit = repositoryItemImageComboBox1;
                gridViewData.Columns["SIGNATURE"].Visible = false;
                gridViewData.Columns["SIGNATURE"].OptionsEditForm.UseEditorColRowSpan = false;
                gridViewData.Columns["SIGNATURE"].OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.True;
                gridViewData.Columns[Custom_Column_Name.USER_SIGNATURE.ToString()].OptionsEditForm.Visible = DevExpress.Utils.DefaultBoolean.False;
                gridViewData.Columns["SIGNATURE"].OptionsEditForm.RowSpan = 5;
                gridViewData.Columns["SIGNATURE"].ColumnEdit = repositoryItemPictureEdit1;
                gridViewData.Columns["USER_ROLE"].VisibleIndex = 5;
                gridViewData.Columns["DEFAULT_PROJECT"].VisibleIndex = 9;
            }
            else if(SyncType == Sync_Type.WBS)
            {
                Hidden_Columns.Add(new ValuePair("SCHEDULEGUID", false));
                Hidden_Columns.Add(new ValuePair("PARENTGUID", false));

                dtCONFLICT.Columns.Add("SCHEDULE_NAME");
                dtCONFLICT.Columns.Add("PARENT_NAME");

                Highlight_Columns.Add("NAME");

                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);

                foreach (DataRow drCONFLICT in dtCONFLICT.Rows)
                {
                    drCONFLICT["SCHEDULE_NAME"] = Common.ConvertScheduleGuidToName(drCONFLICT.Field<Guid>("SCHEDULEGUID"));
                    drCONFLICT["PARENT_NAME"] = Common.ConvertWbsGuidToName(drCONFLICT.Field<Guid>("PARENTGUID"));
                }

                gridViewData.Columns["SCHEDULE_NAME"].VisibleIndex = 0;
                gridViewData.Columns["PARENT_NAME"].VisibleIndex = 1;
            }
            else if(SyncType == Sync_Type.WORKFLOW_MAIN)
            {
                Hidden_Columns.Add(new ValuePair("PARENTGUID", false));
                dtCONFLICT.Columns.Add("PARENT_NAME");
                Highlight_Columns.Add("NAME");

                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);

                foreach(DataRow drCONFLICT in dtCONFLICT.Rows)
                {
                    drCONFLICT["PARENT_NAME"] = Common.ConvertWorkflowGuidToName(drCONFLICT.Field<Guid>("PARENTGUID"));
                }

                gridViewData.Columns["PARENT_NAME"].VisibleIndex = 0;
            }
            else if(SyncType == Sync_Type.MATRIX_TYPE)
            {
                Highlight_Columns.Add("NAME");
                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);

                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);

                gridViewData.Columns["NAME"].VisibleIndex = 0;
                gridViewData.Columns["DESCRIPTION"].VisibleIndex = 1;
                gridViewData.Columns["CATEGORY"].VisibleIndex = 2;
                gridViewData.Columns["DISCIPLINE"].VisibleIndex = 3;
            }
            else if(SyncType == Sync_Type.MATRIX_ASSIGNMENT)
            {
                Hidden_Columns.Add(new ValuePair("GUID_PROJECT", false));
                Hidden_Columns.Add(new ValuePair("GUID_MATRIX_TYPE", false));
                Hidden_Columns.Add(new ValuePair("GUID_TEMPLATE", false));
                dtCONFLICT.Columns.Add("PROJECT");
                dtCONFLICT.Columns.Add("TYPE");
                dtCONFLICT.Columns.Add("TEMPLATE");
                Highlight_Columns.Add("TYPE");
                Highlight_Columns.Add("TEMPLATE");

                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);
                foreach(DataRow drCONFLICT in dtCONFLICT.Rows)
                {
                    drCONFLICT["PROJECT"] = Common.ConvertProjectGuidToName(drCONFLICT.Field<Guid>("GUID_PROJECT"));
                    drCONFLICT["TYPE"] = Common.ConvertMatrixTypeGuidtoName(drCONFLICT.Field<Guid>("GUID_MATRIX_TYPE"));
                    drCONFLICT["TEMPLATE"] = Common.ConvertTemplateGuidToName(drCONFLICT.Field<Guid>("GUID_TEMPLATE"));
                }

                gridViewData.Columns["PROJECT"].VisibleIndex = 0;
                gridViewData.Columns["TYPE"].VisibleIndex = 1;
                gridViewData.Columns["TEMPLATE"].VisibleIndex = 2;
            }
            else
            {
                Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);
            }
            

            Format_GridView_Header();
        }

        private void Standard_WBS_Tag_Details_Population(Sync_Type SyncType, DataTable dtCONFLICT, List<ValuePair> Hidden_Columns, List<string> Highlight_Columns, string ITR_GUID_ColumnName = "")
        {
            Hidden_Columns.Add(new ValuePair("TAG_GUID", false));
            Hidden_Columns.Add(new ValuePair("WBS_GUID", false));

            dtCONFLICT.Columns.Add(Custom_Column_Name.TAG_NUMBER.ToString());
            dtCONFLICT.Columns.Add(Custom_Column_Name.WBS_NAME.ToString());

            Highlight_Columns.Add("NAME");
            Highlight_Columns.Add(Custom_Column_Name.TAG_NUMBER.ToString());
            Highlight_Columns.Add(Custom_Column_Name.WBS_NAME.ToString());

            foreach (DataRow drCONFLICT in dtCONFLICT.Rows)
            {
                string sql = "SELECT Tag.NUMBER, Wbs.NAME";
                sql += " FROM " + SyncType.ToString() + " tblMain LEFT JOIN TAG Tag ON (tblMain.TAG_GUID = Tag.GUID) ";
                sql += "LEFT JOIN WBS Wbs ON (tblMain.WBS_GUID = Wbs.GUID) ";
                sql += "WHERE tblMain.GUID = '" + drCONFLICT.Field<Guid>("GUID").ToString() + "'";

                DataTable dtDETAILS = new DataTable();
                _DataAdapter.ExecuteQuery(sql, dtDETAILS);
                if (dtDETAILS.Rows.Count > 0)
                {
                    DataRow drDETAILS = dtDETAILS.Rows[0];
                    drCONFLICT[Custom_Column_Name.TAG_NUMBER.ToString()] = drDETAILS.IsNull("NUMBER") ? string.Empty : drDETAILS["NUMBER"];
                    drCONFLICT[Custom_Column_Name.WBS_NAME.ToString()] = drDETAILS.IsNull("NAME") ? string.Empty : drDETAILS["NAME"];
                }
            }

            Populate_and_Format_GridViewData_Columns(Hidden_Columns, Highlight_Columns);
            gridViewData.Columns[Custom_Column_Name.TAG_NUMBER.ToString()].VisibleIndex = 0;
            gridViewData.Columns[Custom_Column_Name.WBS_NAME.ToString()].VisibleIndex = 1;
        }

        /// <summary>
        /// Replace _ with " " for Header Caption
        /// </summary>
        private void Format_GridView_Header()
        {
            foreach(DevExpress.XtraGrid.Columns.GridColumn col in gridViewData.Columns)
            {
                col.Caption = Common.Replace_WithSpaces(col.FieldName);
            }
        }

        /// <summary>
        /// Populate and format the GridViewData
        /// </summary>
        private void Populate_and_Format_GridViewData_Columns(List<ValuePair> Hidden_Columns, List<string> Highlight_Columns)
        {
            gridViewData.PopulateColumns();

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridViewData.Columns)
            {
                ValuePair FindHiddenColumn = Hidden_Columns.FirstOrDefault(obj => obj.Label == col.FieldName);
                if(FindHiddenColumn != null)
                {
                    col.Visible = false;
                    col.OptionsColumn.ShowInCustomizationForm = (bool)FindHiddenColumn.Value;
                }

                if (Highlight_Columns.Any(obj => obj == col.FieldName))
                    col.AppearanceCell.BackColor = Color.Pink;

                col.OptionsColumn.AllowEdit = false;
            }
        }

        private void Populate_Datatable(Sync_Type SyncType, ref DataTable dtCONFLICT, Guid ResolveGUID, Guid ConflictOnGUID, string MachineName, DateTime SyncDateTime, Guid SyncedBy)
        {
            //get server only once
            string sql = "SELECT * FROM " + SyncType.ToString() + " WHERE GUID = '" + ResolveGUID + "'";
            DataTable dtQUERY = new DataTable();
            _DataAdapter.ExecuteQuery(sql, dtQUERY);

            if(dtQUERY.Rows.Count > 0)
            {
                //Construct the schema if it doesn't have any
                if (dtCONFLICT.Columns.Count == 0)
                {
                    dtCONFLICT = dtQUERY.Clone();
                    Add_General_Columns(dtCONFLICT);
                }

                DataRow drCONFLICT = dtCONFLICT.NewRow();
                drCONFLICT.ItemArray = dtQUERY.Rows[0].ItemArray;
                //ConflictGUID will be used as resolve guid if user selects it
                Populate_General_Columns(drCONFLICT, ResolveGUID, ConflictOnGUID, MachineName, SyncDateTime, SyncedBy);

                dtCONFLICT.Rows.Add(drCONFLICT);
            }
        }

        /// <summary>
        /// Add General Schema for Sync Details
        /// </summary>
        private void Add_General_Columns(DataTable dtCUSTOM)
        {
            foreach(Custom_General_Column_Name General_Column_Name in Enum.GetValues(typeof(Custom_General_Column_Name)))
            {
                dtCUSTOM.Columns.Add(General_Column_Name.ToString());
                //datetime typed
                if (General_Column_Name == Custom_General_Column_Name.SYNCED)
                {
                    dtCUSTOM.Columns[General_Column_Name.ToString()].DataType = typeof(DateTime);
                }
                else if(General_Column_Name == Custom_General_Column_Name.CONFLICT_ON_GUID || General_Column_Name == Custom_General_Column_Name.RESOLVE_GUID)
                {
                    dtCUSTOM.Columns[General_Column_Name.ToString()].DataType = typeof(Guid);
                }
            }
        }

        /// <summary>
        /// Populate General Schema for Sync Details
        /// </summary>
        private void Populate_General_Columns(DataRow drCUSTOM, Guid ResolveGuid, Guid ConflictOnGuid, string MachineName, DateTime SyncDateTime, Guid SyncedBy)
        {
            drCUSTOM[Custom_General_Column_Name.RESOLVE_GUID.ToString()] = ResolveGuid;
            drCUSTOM[Custom_General_Column_Name.CONFLICT_ON_GUID.ToString()] = ConflictOnGuid;
            drCUSTOM[Custom_General_Column_Name.MACHINE_NAME.ToString()] = MachineName;
            drCUSTOM[Custom_General_Column_Name.CREATED_BY.ToString()] = Get_UserName((Guid)drCUSTOM["CREATEDBY"]);
            drCUSTOM[Custom_General_Column_Name.SYNCED.ToString()] = SyncDateTime;
            drCUSTOM[Custom_General_Column_Name.SYNCED_BY.ToString()] = Get_UserName(SyncedBy);
            if (drCUSTOM.Table.Columns.Contains("UPDATEDBY"))
                drCUSTOM[Custom_General_Column_Name.UPDATED_BY.ToString()] = drCUSTOM.IsNull("UPDATEDBY") ? string.Empty : Get_UserName((Guid)drCUSTOM["UPDATEDBY"]);
        }
        #endregion

        #region Events

        private void btnCollapseAll_Click(object sender, EventArgs e)
        {
            gridViewData.CollapseAllGroups();
        }

        private void btnExpandAll_Click(object sender, EventArgs e)
        {
            gridViewData.ExpandAllGroups();
        }

        private void btnResolve_Click(object sender, EventArgs e)
        {
            if (gridViewData.SelectedRowsCount < 1)
            {
                Common.Warn("Please select a row to resolve");
                return;
            }

            if (!Common.Confirmation("Selected entry will replace other entries in the next sync\nProceed?", "Resolve Conflict"))
                return;

            DataRow drSELECTED = gridViewData.GetFocusedDataRow();
            if(drSELECTED != null)
            {
                Sync_Type SelectedSyncType;
                Conflict_Table SelectedConflictTable = (Conflict_Table)gridViewTable.GetFocusedRow();
                if (SelectedConflictTable != null)
                {
                    SelectedSyncType = (Sync_Type)Enum.Parse(typeof(Sync_Type), SelectedConflictTable.TableName);
                }
                else
                    return;

                Guid ResolveGUID = (Guid)drSELECTED[Custom_General_Column_Name.RESOLVE_GUID.ToString()];
                Guid ServerGUID = (Guid)drSELECTED[Custom_General_Column_Name.CONFLICT_ON_GUID.ToString()];
                dsSYNC_CONFLICT.SYNC_CONFLICTDataTable dtSYNC_CONFLICT = _daSYNC_CONFLICT.Get_Conflict_Context(ServerGUID);
                if(dtSYNC_CONFLICT != null)
                {
                    bool ResolveOnClient = false;
                    foreach(dsSYNC_CONFLICT.SYNC_CONFLICTRow drSYNC_CONFLICT in dtSYNC_CONFLICT)
                    {
                        drSYNC_CONFLICT.RESOLVE_GUID = ResolveGUID;
                        if(drSYNC_CONFLICT.CONFLICT_GUID == ResolveGUID)
                        {
                            Resolve_Conflict(SelectedSyncType, drSYNC_CONFLICT.CONFLICT_GUID, false); //Undelete client conflict because it's selected
                            ResolveOnClient = true;
                        }
                        else
                            Resolve_Conflict(SelectedSyncType, drSYNC_CONFLICT.CONFLICT_GUID, true); //Delete client conflict because it's not selected
                    }

                    _daSYNC_CONFLICT.Save(dtSYNC_CONFLICT);

                    if (ResolveOnClient)
                        Resolve_Conflict(SelectedSyncType, ServerGUID, true); //Delete server conflict because it's not selected
                    else
                        Resolve_Conflict(SelectedSyncType, ServerGUID, false); //Undelete server conflict because it's selected

                    Refresh_Conflict_Table();
                    Refresh_Data_Table();
                }
            }
        }

        private void Resolve_Conflict(Sync_Type SyncType, Guid SyncTypeGUID, bool Delete)
        {
            string sql = string.Empty;
            if(Delete)
            {
                sql += "UPDATE " + SyncType + " SET DELETED = CURRENT_TIMESTAMP, DELETEDBY = '" + System_Environment.GetUser().GUID.ToString() + "'";
                sql += " WHERE GUID = '" + SyncTypeGUID.ToString() + "'";
            }
            else
            {
                sql += "UPDATE " + SyncType + " SET DELETED = NULL, DELETEDBY = NULL WHERE GUID = '" + SyncTypeGUID.ToString() + "'";
            }

            _DataAdapter.ExecuteNonQuery(sql);
        }

        private void gridViewTable_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            GridView view = (GridView)sender;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
            if (!hitInfo.InRow)
            {
                gridViewData.Columns.Clear();
                gridControlData.DataSource = null;
            }
            else
            {
                Refresh_Data_Table();
            }
        }

        /// <summary>
        /// Allows repository to be bounded only during runtime
        /// </summary>
        private void gridViewData_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "ITR" || e.Column.FieldName == "TEMPLATE")
                e.RepositoryItem = repositoryItemRichTextEdit1;
            //else if (e.Column.FieldName == "SIGNATURE")
            //    e.RepositoryItem = repositoryItemPictureEdit1;
        }
        #endregion
    }
}