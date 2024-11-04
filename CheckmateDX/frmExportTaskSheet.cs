using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
using DevExpress.XtraReports.Templates;
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
using System.Web.UI;
using System.Windows.Forms;

namespace CheckmateDX
{
    public partial class frmExportTaskSheet : Form
    {
        IWorkbook _workbook;
        Worksheet _masterSheet;
        string _columnTaskNumber = "TaskNumber";
        string _columnTagNumber = "WBS/Tag Number";
        string _columnTagDescription = "WBS/Tag Description";
        string _columnDiscipline = "Discipline";
        string _columnStage = "Stage";
        string _columnTemplateNumber = "TemplateNumber";
        string _columnTemplateDescription = "TemplateDescription";
        string _columnArea = "Area";
        string _columnSystem = "System Number";
        string _columnSubsystem = "Sub System Number";
        string _columnStatusPending = "Pending";
        string _columnStatusSaved = "Saved";
        string _columnStatusInspected = "Inspected";
        string _columnStatusApproved = "Approved";
        string _columnStatusCompleted = "Completed";
        string _columnStatusClosed = "Closed";
        AdapterPREFILL_REGISTER daPREFILL_REGISTER = new AdapterPREFILL_REGISTER();
        List<Tuple<Guid, string, string>> cachedHeaderDatas = new List<Tuple<Guid, string, string>>();
        public frmExportTaskSheet(List<WorkflowTemplateTagWBS> masterTemplate, List<WorkflowTemplateTagWBS> allSaved, List<WorkflowTemplateTagWBS>allInspected, List<WorkflowTemplateTagWBS> allApproved, List<WorkflowTemplateTagWBS> allCompleted, List<WorkflowTemplateTagWBS> allClosed)
        {
            InitializeComponent();
            initializeColumns();

            List<WorkflowTemplateTagWBS> filteredMasterTemplate = masterTemplate.ToList();
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("Generating Report...");
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, filteredMasterTemplate.Count);

            List<WorkflowTemplateTagWBS> tagWBSTemplates = new List<WorkflowTemplateTagWBS>();
            tagWBSTemplates.AddRange(filteredMasterTemplate);
            tagWBSTemplates.AddRange(allSaved);
            tagWBSTemplates.AddRange(allInspected);
            tagWBSTemplates.AddRange(allApproved);
            tagWBSTemplates.AddRange(allCompleted);
            tagWBSTemplates.AddRange(allClosed);

            foreach (WorkflowTemplateTagWBS tagWBSTemplate in tagWBSTemplates)
            {
                string pending = "false";
                string saved = "false";
                string inspected = "false";
                string approved = "false";
                string completed = "false";
                string closed = "false";

                if(tagWBSTemplate.wtDisplayAttachTag != null)
                {
                    if (allSaved.Where(x => x.wtDisplayAttachTag != null).Any(x => x.wtDisplayAttachTag.GUID == tagWBSTemplate.wtDisplayAttachTag.GUID && x.wtTrueTemplateGuid == tagWBSTemplate.wtTrueTemplateGuid))
                        saved = "true";
                    else if (allInspected.Where(x => x.wtDisplayAttachTag != null).Any(x => x.wtDisplayAttachTag.GUID == tagWBSTemplate.wtDisplayAttachTag.GUID && x.wtTrueTemplateGuid == tagWBSTemplate.wtTrueTemplateGuid))
                        inspected = "true";
                    else if (allApproved.Where(x => x.wtDisplayAttachTag != null).Any(x => x.wtDisplayAttachTag.GUID == tagWBSTemplate.wtDisplayAttachTag.GUID && x.wtTrueTemplateGuid == tagWBSTemplate.wtTrueTemplateGuid))
                        approved = "true";
                    else if (allCompleted.Where(x => x.wtDisplayAttachTag != null).Any(x => x.wtDisplayAttachTag.GUID == tagWBSTemplate.wtDisplayAttachTag.GUID && x.wtTrueTemplateGuid == tagWBSTemplate.wtTrueTemplateGuid))
                        completed = "true";
                    else if (allClosed.Where(x => x.wtDisplayAttachTag != null).Any(x => x.wtDisplayAttachTag.GUID == tagWBSTemplate.wtDisplayAttachTag.GUID && x.wtTrueTemplateGuid == tagWBSTemplate.wtTrueTemplateGuid))
                        closed = "true";

                    string area = Common.GetHeaderDataFromTag(tagWBSTemplate.wtDisplayAttachTag, cachedHeaderDatas, daPREFILL_REGISTER, _columnArea);
                    string system = Common.GetHeaderDataFromTag(tagWBSTemplate.wtDisplayAttachTag, cachedHeaderDatas, daPREFILL_REGISTER, _columnSystem);
                    string subsystem = Common.GetHeaderDataFromTag(tagWBSTemplate.wtDisplayAttachTag, cachedHeaderDatas, daPREFILL_REGISTER, _columnSubsystem);
                    addRow(tagWBSTemplate.wtTaskNumber, tagWBSTemplate.wtDisplayAttachTag.tagNumber, tagWBSTemplate.wtDisplayAttachTag.tagDescription, tagWBSTemplate.wtDisplayDiscipline, tagWBSTemplate.wtStageName, tagWBSTemplate.wtDisplayAttachTemplate.templateName, tagWBSTemplate.wtDisplayAttachTemplate.templateDescription, area, system, subsystem, pending, saved, inspected, approved, completed, closed);
                }
                else if(tagWBSTemplate.wtDisplayAttachWBS != null)
                {
                    if (allSaved.Where(x => x.wtDisplayAttachWBS != null).Any(x => x.wtDisplayAttachWBS.GUID == tagWBSTemplate.wtDisplayAttachWBS.GUID && x.wtTrueTemplateGuid == tagWBSTemplate.wtTrueTemplateGuid))
                        saved = "true";
                    else if (allInspected.Where(x => x.wtDisplayAttachWBS != null).Any(x => x.wtDisplayAttachWBS.GUID == tagWBSTemplate.wtDisplayAttachWBS.GUID && x.wtTrueTemplateGuid == tagWBSTemplate.wtTrueTemplateGuid))
                        inspected = "true";
                    else if (allApproved.Where(x => x.wtDisplayAttachWBS != null).Any(x => x.wtDisplayAttachWBS.GUID == tagWBSTemplate.wtDisplayAttachWBS.GUID && x.wtTrueTemplateGuid == tagWBSTemplate.wtTrueTemplateGuid))
                        approved = "true";
                    else if (allCompleted.Where(x => x.wtDisplayAttachWBS != null).Any(x => x.wtDisplayAttachWBS.GUID == tagWBSTemplate.wtDisplayAttachWBS.GUID && x.wtTrueTemplateGuid == tagWBSTemplate.wtTrueTemplateGuid))
                        completed = "true";
                    else if (allClosed.Where(x => x.wtDisplayAttachWBS != null).Any(x => x.wtDisplayAttachWBS.GUID == tagWBSTemplate.wtDisplayAttachWBS.GUID && x.wtTrueTemplateGuid == tagWBSTemplate.wtTrueTemplateGuid))
                        closed = "true";

                    string area = Common.GetHeaderDataFromWBS(tagWBSTemplate.wtDisplayAttachWBS, cachedHeaderDatas, daPREFILL_REGISTER, _columnArea);
                    string system = Common.GetHeaderDataFromWBS(tagWBSTemplate.wtDisplayAttachWBS, cachedHeaderDatas, daPREFILL_REGISTER, _columnSystem);
                    string subsystem = Common.GetHeaderDataFromWBS(tagWBSTemplate.wtDisplayAttachWBS, cachedHeaderDatas, daPREFILL_REGISTER, _columnSubsystem);
                    addRow(tagWBSTemplate.wtTaskNumber, tagWBSTemplate.wtDisplayAttachWBS.wbsName, tagWBSTemplate.wtDisplayAttachWBS.wbsDescription, tagWBSTemplate.wtDisplayDiscipline, tagWBSTemplate.wtStageName, tagWBSTemplate.wtDisplayAttachTemplate.templateName, tagWBSTemplate.wtDisplayAttachTemplate.templateDescription, area, system, subsystem, pending, saved, inspected, approved, completed, closed);
                }

                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            splashScreenManager1.CloseWaitForm();
            _workbook = spreadsheetControl1.Document;
            ExternalDataSourceOptions options = new ExternalDataSourceOptions();
            options.ImportHeaders = true;
            _masterSheet = _workbook.Worksheets[0];
            Table masterSpreadsheetTable = _masterSheet.Tables.Add(metaTable, 0, 0, options);
        }

        private void populateWBSTemplates(List<WorkflowTemplateTagWBS> scanTemplates, Dictionary<WBS, Guid> collection, bool isWBS)
        {
            foreach (WorkflowTemplateTagWBS scanTemplate in scanTemplates)
            {
                if (scanTemplate.wtDisplayAttachWBS != null)
                {
                    if (!collection.Any(x => x.Key == scanTemplate.wtDisplayAttachWBS && x.Value == scanTemplate.wtTrueTemplateGuid))
                    {
                        collection.Add(scanTemplate.wtDisplayAttachWBS, scanTemplate.wtTrueTemplateGuid);
                    }
                }
            }
        }

        DataTable metaTable;
        private void initializeColumns()
        {
            metaTable = new DataTable("MetaTable");
            metaTable.Columns.Add(_columnTaskNumber, typeof(string));
            metaTable.Columns.Add(_columnTagNumber, typeof(string));
            metaTable.Columns.Add(_columnTagDescription, typeof(string));
            metaTable.Columns.Add(_columnDiscipline, typeof(string));
            metaTable.Columns.Add(_columnStage, typeof(string));
            metaTable.Columns.Add(_columnTemplateNumber, typeof(string));
            metaTable.Columns.Add(_columnTemplateDescription, typeof(string));
            metaTable.Columns.Add(_columnArea, typeof(string));
            metaTable.Columns.Add(_columnSystem, typeof(string));
            metaTable.Columns.Add(_columnSubsystem, typeof(string));
            metaTable.Columns.Add(_columnStatusPending, typeof(string));
            metaTable.Columns.Add(_columnStatusSaved, typeof(string));
            metaTable.Columns.Add(_columnStatusInspected, typeof(string));
            metaTable.Columns.Add(_columnStatusApproved, typeof(string));
            metaTable.Columns.Add(_columnStatusCompleted, typeof(string));
            metaTable.Columns.Add(_columnStatusClosed, typeof(string));
        }

        private void addRow(string taskNumber, string tagNumber, string tagDescription, string discipline, string stage, string templateNumber, string templateDescription, string area, string system, string subsystem, string pending, string saved, string inspected, string approved, string completed, string closed)
        {
            DataRow newRow = metaTable.NewRow();
            newRow[_columnTaskNumber] = taskNumber;
            newRow[_columnTagNumber] = tagNumber;
            newRow[_columnTagDescription] = tagDescription;
            newRow[_columnDiscipline] = discipline;
            newRow[_columnStage] = stage;
            newRow[_columnTemplateNumber] = templateNumber;
            newRow[_columnTemplateDescription] = templateDescription;
            newRow[_columnArea] = area;
            newRow[_columnSystem] = system;
            newRow[_columnSubsystem] = subsystem;
            newRow[_columnStatusPending] = pending;
            newRow[_columnStatusSaved] = saved;
            newRow[_columnStatusInspected] = inspected;
            newRow[_columnStatusApproved] = approved;
            newRow[_columnStatusCompleted] = completed;
            newRow[_columnStatusClosed] = closed;
            metaTable.Rows.Add(newRow);
        }
    }
}
