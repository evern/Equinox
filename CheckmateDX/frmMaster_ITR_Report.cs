using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;
using ProjectCommon;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckmateDX
{
    public partial class frmMaster_ITR_Report : Form
    {
        IWorkbook _workbook;
        Worksheet _masterSheet;
        Guid _projectGuid;
        AdapterTAG daTag = new AdapterTAG();
        List<ViewModel_MasterITRReport> reportData;

        //column definitions
        string _columnStage;
        string _columnTaskNumber;
        string _columnTemplateNumber;
        string _columnTemplateDescription;
        string _columnDiscipline;
        string _columnTagNumber;
        string _columnTagDescription;
        string _columnAreaNumber;
        string _columnAreaDescription;
        string _columnSubsystemNumber;
        string _columnSubsystemDescription;
        string _columnImportStatus;

        public frmMaster_ITR_Report(Guid projectGuid)
        {
            InitializeComponent(); 
            _workbook = spreadsheetControl1.Document;
            _masterSheet = _workbook.Worksheets[0];
            _projectGuid = projectGuid;
            defineColumns();
            populateReport(_projectGuid);
        }

        #region Helper
        private void populateReport(Guid projectGuid)
        {
            _workbook.Worksheets.ActiveWorksheet = _workbook.Worksheets[0];
            spreadsheetControl1.BeginUpdate();
            using (AdapterITR_MAIN daITR_MAIN = new AdapterITR_MAIN())
            {
                reportData = daITR_MAIN.GetProjectITRMasterReport(projectGuid);
            }

            DataTable masterTable = new DataTable("MasterTable");
            masterTable.Columns.Add(_columnStage, typeof(string));
            masterTable.Columns.Add(_columnTaskNumber, typeof(string));
            masterTable.Columns.Add(_columnTemplateNumber, typeof(string));
            masterTable.Columns.Add(_columnTemplateDescription, typeof(string));
            masterTable.Columns.Add(_columnDiscipline, typeof(string));
            masterTable.Columns.Add(_columnTagNumber, typeof(string));
            masterTable.Columns.Add(_columnTagDescription, typeof(string));
            masterTable.Columns.Add(_columnAreaNumber, typeof(string));
            masterTable.Columns.Add(_columnAreaDescription, typeof(string));
            masterTable.Columns.Add(_columnSubsystemNumber, typeof(string));
            masterTable.Columns.Add(_columnSubsystemDescription, typeof(string));
            masterTable.Columns.Add(_columnImportStatus, typeof(string));

            foreach(ViewModel_MasterITRReport reportDataRow in reportData)
            {
                DataRow newRow = masterTable.Rows.Add();
                newRow[_columnStage] = reportDataRow.Stage;

                string templateName = Common.RemoveCommentedTemplateSection(reportDataRow.Template);
                newRow[_columnTaskNumber] = Common.GetStringHash(string.Concat(reportDataRow.Tag, templateName));
                newRow[_columnTemplateNumber] = reportDataRow.Template;
                newRow[_columnTemplateDescription] = reportDataRow.TemplateDescription;
                newRow[_columnDiscipline] = reportDataRow.Discipline;
                newRow[_columnTagNumber] = reportDataRow.Tag;
                newRow[_columnTagDescription] = reportDataRow.TagDescription;
                newRow[_columnAreaNumber] = reportDataRow.Area;
                newRow[_columnAreaDescription] = reportDataRow.AreaDescription;
                newRow[_columnSubsystemNumber] = reportDataRow.Subsystem;
                newRow[_columnSubsystemDescription] = reportDataRow.SubsystemDescription;
                newRow[_columnImportStatus] = reportDataRow.Status;
            }

            ExternalDataSourceOptions options = new ExternalDataSourceOptions();
            options.ImportHeaders = true;
            DevExpress.Spreadsheet.Table masterSpreadsheetTable = _masterSheet.Tables.Add(masterTable, 0, 0, options);
            spreadsheetControl1.EndUpdate();
        }

        private void defineColumns()
        {
            _columnStage = "Stage";
            _columnTaskNumber = "Task Number";
            _columnTemplateNumber = "Template Number";
            _columnTemplateDescription = "Template Description";
            _columnDiscipline = "Discipline";
            _columnTagNumber = "Tag Number";
            _columnTagDescription = "Tag Description";
            _columnAreaNumber = "Area Number";
            _columnAreaDescription = "Area Description";
            _columnSubsystemNumber = "Subsystem Number";
            _columnSubsystemDescription = "Subsystem Description";
            _columnImportStatus = "Status";
        }
        #endregion
    }
}
