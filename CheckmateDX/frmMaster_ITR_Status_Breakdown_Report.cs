using DevExpress.Spreadsheet;
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
    public partial class frmMaster_ITR_Status_Breakdown_Report : Form
    {
        IWorkbook _workbook;
        Worksheet _masterSheet;
        Guid _projectGuid;
        AdapterTAG daTag = new AdapterTAG();
        List<ViewModel_MasterITRStatusReport> reportData;

        //column definitions
        string _columnDiscipline;
        string _columnAreaDescription;
        string _columnSystemDescription;
        string _columnSubsystemNumber;
        string _columnSubsystemDescription;
        string _columnStage;
        string _columnTagNumber;
        string _columnTemplateNumber;
        string _columnTemplateDescription;
        string _columnPendingDate;
        string _columnPendingPerson;
        string _columnSavedDate;
        string _columnSavedPerson;
        string _columnInspectedDate;
        string _columnInspectedPerson;
        string _columnApprovedDate;
        string _columnApprovedPerson;
        string _columnCompletedDate;
        string _columnCompletedPerson;
        string _columnClosedDate;
        string _columnClosedPerson;

        public frmMaster_ITR_Status_Breakdown_Report(Guid projectGuid)
        {
            InitializeComponent();
            _workbook = spreadsheetControl1.Document;
            _masterSheet = _workbook.Worksheets[0];
            _projectGuid = projectGuid;
            defineColumns();
            populateReport(_projectGuid);
        }

        private void defineColumns()
        {
            _columnDiscipline = "Discipline";
            _columnAreaDescription = "Area Description";
            _columnSystemDescription = "System Description";
            _columnSubsystemNumber = "Subsystem Number";
            _columnSubsystemDescription = "Subsystem Description";
            _columnStage = "Stage";
            _columnTagNumber = "Task Number";
            _columnTemplateNumber = "Template Number";
            _columnTemplateDescription = "Template Description";
            _columnPendingDate = "Pending";
            _columnPendingPerson = "Pending Person";
            _columnSavedDate = "Saved";
            _columnSavedPerson = "Saved Person";
            _columnInspectedDate = "Inspected";
            _columnInspectedPerson = "Inspected Person";
            _columnApprovedDate = "Approved";
            _columnApprovedPerson = "Approved Person";
            _columnCompletedDate = "Completed";
            _columnCompletedPerson = "Completed Person";
            _columnClosedDate = "Closed";
            _columnClosedPerson = "Closed Person";
        }


        private void populateReport(Guid projectGuid)
        {
            _workbook.Worksheets.ActiveWorksheet = _workbook.Worksheets[0];
            spreadsheetControl1.BeginUpdate();
            using (AdapterITR_MAIN daITR_MAIN = new AdapterITR_MAIN())
            {
                reportData = daITR_MAIN.GetProjectITRStatusBreakdownReport(projectGuid);
            }

            DataTable masterTable = new DataTable("MasterTable");

            masterTable.Columns.Add(_columnDiscipline, typeof(string));
            masterTable.Columns.Add(_columnAreaDescription, typeof(string));
            masterTable.Columns.Add(_columnSystemDescription, typeof(string));
            masterTable.Columns.Add(_columnSubsystemNumber, typeof(string));
            masterTable.Columns.Add(_columnSubsystemDescription, typeof(string));
            masterTable.Columns.Add(_columnStage, typeof(string));
            masterTable.Columns.Add(_columnTagNumber, typeof(string));
            masterTable.Columns.Add(_columnTemplateNumber, typeof(string));
            masterTable.Columns.Add(_columnTemplateDescription, typeof(string));
            masterTable.Columns.Add(_columnPendingDate, typeof(DateTime));
            masterTable.Columns.Add(_columnPendingPerson, typeof(string));
            masterTable.Columns.Add(_columnSavedDate, typeof(DateTime));
            masterTable.Columns.Add(_columnSavedPerson, typeof(string));
            masterTable.Columns.Add(_columnInspectedDate, typeof(DateTime));
            masterTable.Columns.Add(_columnInspectedPerson, typeof(string));
            masterTable.Columns.Add(_columnApprovedDate, typeof(DateTime));
            masterTable.Columns.Add(_columnApprovedPerson, typeof(string));
            masterTable.Columns.Add(_columnCompletedDate, typeof(DateTime));
            masterTable.Columns.Add(_columnCompletedPerson, typeof(string));
            masterTable.Columns.Add(_columnClosedDate, typeof(DateTime));
            masterTable.Columns.Add(_columnClosedPerson, typeof(string));

            foreach (ViewModel_MasterITRStatusReport reportDataRow in reportData)
            {
                DataRow newRow = masterTable.Rows.Add();
                newRow[_columnDiscipline] = reportDataRow.Discipline;
                newRow[_columnAreaDescription] = reportDataRow.Area;
                newRow[_columnSystemDescription] = reportDataRow.SubsystemDescription;
                newRow[_columnSubsystemNumber] = reportDataRow.SubsystemName;
                newRow[_columnSubsystemDescription] = reportDataRow.SubsystemDescription;
                newRow[_columnStage] = reportDataRow.Stage;
                newRow[_columnTagNumber] = reportDataRow.TagNumber;
                string templateName = Common.RemoveCommentedTemplateSection(reportDataRow.TemplateName);
                newRow[_columnTemplateNumber] = templateName;
                newRow[_columnTemplateDescription] = reportDataRow.TemplateDescription;
                if (reportDataRow.PendingDate != null)
                    newRow[_columnPendingDate] = reportDataRow.PendingDate;
                if (reportDataRow.PendingPerson != null)
                    newRow[_columnPendingPerson] = reportDataRow.PendingPerson;
                if (reportDataRow.SavedDate != null)
                    newRow[_columnSavedDate] = reportDataRow.SavedDate;
                if (reportDataRow.SavedPerson != null)
                    newRow[_columnSavedPerson] = reportDataRow.SavedPerson;
                if (reportDataRow.InspectedDate != null)
                    newRow[_columnInspectedDate] = reportDataRow.InspectedDate;
                if (reportDataRow.InspectedPerson != null)
                    newRow[_columnInspectedPerson] = reportDataRow.InspectedPerson;
                if (reportDataRow.ApprovedDate != null)
                    newRow[_columnApprovedDate] = reportDataRow.ApprovedDate;
                if (reportDataRow.ApprovedPerson != null)
                    newRow[_columnApprovedPerson] = reportDataRow.ApprovedPerson;
                if (reportDataRow.CompletedDate != null)
                    newRow[_columnCompletedDate] = reportDataRow.CompletedDate;
                if (reportDataRow.CompletedPerson != null)
                    newRow[_columnCompletedPerson] = reportDataRow.CompletedPerson;
                if (reportDataRow.ClosedDate != null)
                    newRow[_columnClosedDate] = reportDataRow.ClosedDate;
                if (reportDataRow.ClosedPerson != null)
                    newRow[_columnClosedPerson] = reportDataRow.ClosedPerson;
            }

            ExternalDataSourceOptions options = new ExternalDataSourceOptions();
            options.ImportHeaders = true;
            Table masterSpreadsheetTable = _masterSheet.Tables.Add(masterTable, 0, 0, options);
            _masterSheet.Columns.AutoFit(0, masterTable.Columns.IndexOf(_columnClosedDate));
            spreadsheetControl1.EndUpdate();
        }
    }
}
