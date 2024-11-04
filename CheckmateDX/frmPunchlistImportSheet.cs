using DevExpress.Spreadsheet;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;
using ProjectCommon;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace CheckmateDX
{
    public partial class frmPunchlistImportSheet : Form
    {
        DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        AdapterPUNCHLIST_MAIN daPunchlist = new AdapterPUNCHLIST_MAIN();
        Guid _projectGuid = Guid.Empty;
        Guid _scheduleGuid = Guid.Empty;
        string _discipline;
        IWorkbook _workbook;
        Worksheet _punchlistSheet;
        Worksheet _tagSheet;
        Worksheet _wbsSheet;
        Worksheet _prioritySheet;
        Worksheet _actionBySheet;

        string _prefixImportError = "ERROR: ";
        string _prefixImportSuccess = "SUCCESS: ";
        //string _prefixProgressWritingHeaderData = "Writing Header Data for ";
        string _prefixProgressCheckingDuplicates = "Checking Duplicates for ";
        string _prefixProgressAddingNewPunchlistItem = "Adding New Punchlist ";
        //string _prefixProgressUpdatingPunchlistItem = "Updating Punchlist Item: ";
        //string _prefixProgressRestoringPunchlistItem = "Restoring Punchlist Item: ";

        string _columnKey = "Key (Read Only)";
        string _columnImportStatus = "Import Status (Read Only)";
        string _columnTitle = "Title";
        string _columnDescription = "Description";
        string _columnPriority = "Category";
        string _columnActionBy = "Action By";
        string _columnTag = "Tag";
        string _columnWBS = "WBS (Overrides Tag)";
        //string _fileName = "";
        //bool _isBySchedule = false;

        int _indexKey = 0;
        int _indexImportStatus = 1;
        int _indexTitle = 2;
        int _indexDescription = 3;
        int _indexPriority = 4;
        int _indexActionBy = 5;
        int _indexTag = 6;
        int _indexWBS = 7;

        AdapterPREFILL_REGISTER _daPrefillReg = new AdapterPREFILL_REGISTER();
        dsPREFILL_REGISTER _dsPrefillReg = new dsPREFILL_REGISTER();
        List<SpreadsheetCommandDelegateContainer> _replaceCommandMethod = new List<SpreadsheetCommandDelegateContainer>();
        DispatcherTimer delayedImportTimer = new DispatcherTimer();
        Action<Punchlist> _savePunchlistAction;
        public frmPunchlistImportSheet()
        {
            InitializeComponent();

            _workbook = spreadsheetControl1.Document;
            _punchlistSheet = _workbook.Worksheets[0];
            _tagSheet = _workbook.Worksheets.Add();
            _wbsSheet = _workbook.Worksheets.Add();
            SetReplaceCommandMethod();
        }

        public frmPunchlistImportSheet(Guid projectGuid, string discipline, Action<Punchlist> savePunchlistAction)
        {
            InitializeComponent();

            splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
            _projectGuid = projectGuid;
            _discipline = discipline;

            SetReplaceCommandMethod();

            _workbook = spreadsheetControl1.Document;
            _punchlistSheet = _workbook.Worksheets[0];
            _tagSheet = _workbook.Worksheets.Add();
            _wbsSheet = _workbook.Worksheets.Add();
            _prioritySheet = _workbook.Worksheets.Add();
            _actionBySheet = _workbook.Worksheets.Add();
            _savePunchlistAction = savePunchlistAction;

            populateData(_scheduleGuid);

            _workbook.Worksheets.ActiveWorksheet = _punchlistSheet;
        }

        private void DelayedImportTimer_Tick(object sender, EventArgs e)
        {
            delayedImportTimer.Stop();
            importSpreadsheet();
        }

        #region Helper
        /// <summary>
        /// Assign local method to commands on spreadsheet
        /// </summary>
        private void SetReplaceCommandMethod()
        {
            _replaceCommandMethod.Add(new SpreadsheetCommandDelegateContainer(SpreadsheetCommandId.FileSave, new SpreadsheetCommandDelegate(importSpreadsheet)));
            ISpreadsheetCommandFactoryService service = spreadsheetControl1.GetService(typeof(ISpreadsheetCommandFactoryService)) as ISpreadsheetCommandFactoryService;
            spreadsheetControl1.ReplaceService<ISpreadsheetCommandFactoryService>(new CustomSpreadsheetCommandFactoryService(service, spreadsheetControl1, _replaceCommandMethod));
        }

        enum ErrorType
        {
            Duplicate_Exists,
            Key_Not_Found
        }

        private int? findColumnByName(DevExpress.Spreadsheet.Table searchTable, string columnName)
        {
            for(int i=0;i < searchTable.Columns.Count;i++)
            {
                if(searchTable.Columns[i].Name == columnName)
                {
                    return i;
                }
            }

            return null;
        }

        private bool isPunchlistDuplicate(string punchlistTitle, string wbsTagName)
        {
            DevExpress.Spreadsheet.Table masterSpreadsheetTable = _punchlistSheet.Tables[0];

            for (int row = 1; row < masterSpreadsheetTable.DataRange.BottomRowIndex; row++)
            {
                var scanRow = _punchlistSheet.Rows[row];
                string currentRowKey = scanRow[_indexKey].DisplayText;
                if (currentRowKey == string.Empty)
                    continue;

                string currentRowTagName = scanRow[_indexTag].DisplayText;
                string currentRowWBSName = scanRow[_indexWBS].DisplayText;

                if (wbsTagName == currentRowTagName || wbsTagName == currentRowWBSName)
                {
                    string currentRowTitle = scanRow[_indexTitle].DisplayText;
                    if (currentRowTitle == punchlistTitle)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Writes the wbs/tag data into db
        /// </summary>
        /// <param name="prefillGuid">wbs/tag Guid</param>
        /// <param name="prefillType">wbs or tag</param>
        /// <param name="usedRange">spreadsheet used range</param>
        /// <param name="rowIndex">current tag row index</param>
        private void AddPrefill(dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPrefill, Guid prefillGuid, PrefillType prefillType, CellRange usedRange, int rowIndex, int prefillStartIndex)
        {
            for (int columnIndex = prefillStartIndex; columnIndex < usedRange.ColumnCount; columnIndex++)
            {
                string headerString = _punchlistSheet.Cells[0, columnIndex].DisplayText.Trim();
                dsPREFILL_REGISTER.PREFILL_REGISTERRow drPrefillReg = dtPrefill.FirstOrDefault(x => x.IsDELETEDNull() && x.NAME == headerString);

                if (_punchlistSheet.Cells[rowIndex, columnIndex].DisplayText != string.Empty)
                {
                    bool addRow = false;

                    if (drPrefillReg == null)
                    {
                        drPrefillReg = dtPrefill.NewPREFILL_REGISTERRow();
                        drPrefillReg.GUID = Guid.NewGuid();

                        addRow = true;
                    }

                    if (prefillType == PrefillType.Tag)
                        drPrefillReg.TAG_GUID = prefillGuid;
                    else
                        drPrefillReg.WBS_GUID = prefillGuid;

                    drPrefillReg.NAME = headerString;
                    drPrefillReg.DATA = _punchlistSheet.Cells[rowIndex, columnIndex].DisplayText.Trim();

                    if (addRow)
                    {
                        drPrefillReg.CREATED = DateTime.Now;
                        drPrefillReg.CREATEDBY = System_Environment.GetUser().GUID;
                        dtPrefill.AddPREFILL_REGISTERRow(drPrefillReg);
                    }
                    else
                    {
                        drPrefillReg.UPDATED = DateTime.Now;
                        drPrefillReg.UPDATEDBY = System_Environment.GetUser().GUID;
                    }

                   // _daPrefillReg.Save(drPrefillReg);
                }
                //if string is empty we would want to delete the entry if it exists in db
                else
                {
                    if (drPrefillReg != null)
                    {
                        drPrefillReg.DELETED = DateTime.Now;
                        drPrefillReg.DELETEDBY = System_Environment.GetUser().GUID;
                        //_daPrefillReg.RemoveBy(drPrefillReg.GUID);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the wbs/tag data
        /// </summary>
        private void importSpreadsheet()
        {
            spreadsheetControl1.BeginUpdate();
            AdapterTAG daTag = new AdapterTAG();
            AdapterWBS daWBS = new AdapterWBS();
            dsPUNCHLIST_MAIN dsPUNCHLIST_MAIN = new dsPUNCHLIST_MAIN();
            
            _workbook = spreadsheetControl1.Document;
            _punchlistSheet = _workbook.Worksheets[0];

            if (_workbook.Worksheets.Count > 1)
                _tagSheet = _workbook.Worksheets[1];

            if (_workbook.Worksheets.Count > 2)
                _wbsSheet = _workbook.Worksheets[2];

            if (_punchlistSheet == null || _punchlistSheet.Tables == null || _punchlistSheet.Tables.Count == 0)
            {
                MessageBox.Show("Table not detected in master sheet");
                return;
            }

            if(_tagSheet == null || _tagSheet.Tables == null || _tagSheet.Tables.Count == 0)
            {
                MessageBox.Show("Table not detected in tag sheet");
                return;
            }

            if (_wbsSheet == null || _wbsSheet.Tables == null || _wbsSheet.Tables.Count == 0)
            {
                MessageBox.Show("Table not detected in WBS sheet");
                return;
            }

            if (_prioritySheet == null || _prioritySheet.Tables == null || _prioritySheet.Tables.Count == 0)
            {
                MessageBox.Show("Table not detected in priority sheet");
                return;
            }

            DevExpress.Spreadsheet.Table masterSpreadsheetTable = _punchlistSheet.Tables[0];

            try
            {
                int lastRowIndex = masterSpreadsheetTable.DataRange.BottomRowIndex;
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Saving ...");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, lastRowIndex);
                for (int row=1;row <= lastRowIndex; row++)
                {
                    string punchlistTitle = _punchlistSheet.Cells[row, _indexTitle].DisplayText;
                    if (punchlistTitle == string.Empty)
                        continue;

                    string punchlistTag = _punchlistSheet.Cells[row, _indexTag].DisplayText;
                    string punchlistWBS = _punchlistSheet.Cells[row, _indexWBS].DisplayText;
                    string punchlistKey = _punchlistSheet.Cells[row, _indexKey].DisplayText;
                    string punchlistDescription = _punchlistSheet.Cells[row, _indexDescription].DisplayText;
                    string punchlistPriority = _punchlistSheet.Cells[row, _indexPriority].DisplayText;
                    string punchlistActionBy = _punchlistSheet.Cells[row, _indexActionBy].DisplayText;
                    string punchlistWBSOrTagName = punchlistTag == string.Empty ? punchlistWBS : punchlistTag;

                    if (punchlistKey != string.Empty && punchlistKey.Length != Guid.Empty.ToString().Length)
                        return;

                    Guid? tagGuid = null;
                    Guid? wbsGuid = null;
                    if (punchlistWBS != string.Empty)
                    {
                        dsWBS.WBSRow drWBS = daWBS.GetBy(punchlistWBS, _projectGuid);
                        wbsGuid = drWBS == null ? (Guid?)null : drWBS.GUID;
                    }
                    else
                    {
                        dsTAG.TAGRow drTAG = daTag.GetBy(punchlistTag, _projectGuid);
                        tagGuid = drTAG == null ? (Guid?)null : drTAG.GUID;
                    }

                    if (tagGuid == null && punchlistTag != string.Empty)
                    {
                        _punchlistSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "Tag number not found";
                    }
                    else if (punchlistKey == string.Empty)
                    {
                        if (punchlistTitle != string.Empty)
                        {
                            splashScreenManager1.SetWaitFormDescription(_prefixProgressCheckingDuplicates + punchlistTitle);

                            //Checks for duplicate in current spreadsheet
                            dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow existingPunchlist = null;
                            if (tagGuid != null)
                                daPunchlist.GetBy((Guid)tagGuid, punchlistTitle);
                            else if (wbsGuid != null)
                                daPunchlist.GetBy((Guid)wbsGuid, punchlistTitle);
                            
                            if (existingPunchlist != null)
                            {
                                if(tagGuid != null)
                                    _punchlistSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "Duplicate Tag with same title exists in Database";
                                else
                                    _punchlistSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "Duplicate WBS with same title exists in Database";

                                continue;
                            }
                            else if (isPunchlistDuplicate(punchlistTitle, punchlistWBSOrTagName))
                            {
                                _punchlistSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "Duplicate Punchlist Exists in Spreadsheet";
                                continue;
                            }
                            else if (existingPunchlist == null)
                            {
                                splashScreenManager1.SetWaitFormDescription(_prefixProgressAddingNewPunchlistItem + punchlistTitle);
                                dsPUNCHLIST_MAIN dsPunchlist = new dsPUNCHLIST_MAIN();
                                dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlist = dsPunchlist.PUNCHLIST_MAIN.NewPUNCHLIST_MAINRow();
                                Guid newGuid = Guid.NewGuid();

                                Punchlist newPunchlist = new Punchlist(newGuid);
                                if (tagGuid != null)
                                    newPunchlist.punchlistTagGuid = ((Guid)tagGuid);
                                else
                                    newPunchlist.punchlistWBSGuid = ((Guid)wbsGuid);

                                newPunchlist.punchlistTitle = punchlistTitle;
                                newPunchlist.punchlistPriority = punchlistPriority;
                                newPunchlist.punchlistActionBy = punchlistActionBy;
                                newPunchlist.punchlistDescription = punchlistDescription;
                                newPunchlist.punchlistItem = string.Empty;
                                newPunchlist.punchlistRemedial = string.Empty;
                                newPunchlist.punchlistCategory = string.Empty;
                                newPunchlist.punchlistDiscipline = _discipline;
                                _savePunchlistAction?.Invoke(newPunchlist);

                                punchlistKey = newGuid.ToString();
                                _punchlistSheet.Cells[row, _indexKey].Value = punchlistKey;
                                _punchlistSheet.Cells[row, _indexImportStatus].Value = _prefixImportSuccess + "Punchlist Added";
                            }
                        }
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }
            }
            catch(Exception e)
            {
                string s = e.ToString();
            }
            finally
            {
                splashScreenManager1.CloseWaitForm();
                daTag.Dispose();
                daPunchlist.Dispose();
                MessageBox.Show("Import is completed, please close and re-open punchlist to see the changes");

                spreadsheetControl1.EndUpdate();
            }
        }

        private Guid getExistingOrAddWBS(AdapterWBS daWBS, Guid parentGuid, string wbsName, string wbsDescription)
        {
            dsWBS.WBSRow drWBS = daWBS.GetByProjectParentName(_projectGuid, parentGuid, wbsName);
            if (drWBS != null)
                return drWBS.GUID;
            else
            {
                dsWBS dsWBS = new dsWBS();
                dsWBS.WBSRow drNewWBS = dsWBS.WBS.NewWBSRow();

                Guid newWBSGuid = Guid.NewGuid();
                drNewWBS.GUID = newWBSGuid;
                drNewWBS.NAME = wbsName;
                drNewWBS.DESCRIPTION = wbsDescription;
                drNewWBS.SCHEDULEGUID = _scheduleGuid;
                drNewWBS.PARENTGUID = parentGuid;
                drNewWBS.CREATED = DateTime.Now;
                drNewWBS.CREATEDBY = System_Environment.GetUser().GUID;

                dsWBS.WBS.AddWBSRow(drNewWBS);
                daWBS.Save(drNewWBS);

                return newWBSGuid;
            }
        }


        private void assignTagMatrix(AdapterTEMPLATE_REGISTER daTEMPLATE_REGISTER, dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPEDataTable dtMATRIX, Guid tagGuid, string matrixType)
        {
            if (matrixType == string.Empty)
                return;

            DataTable dtGENERIC_MATRIX_TYPE;
            try
            {
                dtGENERIC_MATRIX_TYPE = dtMATRIX.AsEnumerable().Where(obj => obj.TYPE.ToUpper() == matrixType).CopyToDataTable();
            }
            catch
            {
                dtGENERIC_MATRIX_TYPE = null;
                //if dtGENERIC_MATRIX_TYPE is null for whatever reasons
            }

            if (dtGENERIC_MATRIX_TYPE != null)
            {
                List<Guid> AssignedTemplateGuids = new List<Guid>();
                dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPEDataTable dtMATRIX_TYPE = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPEDataTable();
                dtMATRIX_TYPE.Merge(dtGENERIC_MATRIX_TYPE);

                foreach (dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPERow drMATRIX_TYPE in dtMATRIX_TYPE.Rows)
                {
                    daTEMPLATE_REGISTER.AssignTagTemplate(drMATRIX_TYPE.GUID_TEMPLATE, tagGuid);
                    AssignedTemplateGuids.Add(drMATRIX_TYPE.GUID_TEMPLATE);
                }

                //dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER = daTEMPLATE_REGISTER.GetByWBSTagGuid(tagGuid);
                //if (dtTEMPLATE_REGISTER != null)
                //{
                //    foreach (dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTEMPLATE_REGISTER in dtTEMPLATE_REGISTER.Rows)
                //    {
                //        if (!AssignedTemplateGuids.Any(obj => obj == drTEMPLATE_REGISTER.TEMPLATE_GUID))
                //        {
                //            daTEMPLATE_REGISTER.RemoveBy(drTEMPLATE_REGISTER.GUID);
                //        }
                //    }
                //}
            }
        }
        #endregion

        /// <summary>
        /// Writes unique header to spreadsheet, also highlights required field for each wbs/tag
        /// </summary>
        private void populateData(Guid scheduleGuid)
        {
            spreadsheetControl1.BeginUpdate();
            AdapterPUNCHLIST_MAIN daPUNCHLIST = new AdapterPUNCHLIST_MAIN();
            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST = daPUNCHLIST.GetAllTagPunchlist_ByProject(_projectGuid);
            splashScreenManager1.ShowWaitForm();

            DataTable tagTable = new DataTable("TagTable");
            tagTable.Columns.Add("Tag", typeof(string));

            AdapterTAG daTAG = new AdapterTAG();
            dsTAG.TAGDataTable dtTAG = daTAG.GetByProjectDiscipline(_projectGuid, _discipline);
            if(dtTAG != null)
            {
                foreach (dsTAG.TAGRow drTAG in dtTAG.Rows)
                {
                    tagTable.Rows.Add(drTAG.NUMBER);
                }
            }

            DataTable wbsTable = new DataTable("WBSTable");
            wbsTable.Columns.Add("WBS", typeof(string));

            AdapterWBS daWBS = new AdapterWBS();
            dsWBS.WBSDataTable dtWBS = daWBS.GetByProject(_projectGuid);
            if(dtWBS != null)
            {
                foreach (dsWBS.WBSRow drWBS in dtWBS.Rows)
                {
                    wbsTable.Rows.Add(drWBS.NAME);
                }
            }

            ExternalDataSourceOptions options = new ExternalDataSourceOptions();
            options.ImportHeaders = true;
            DevExpress.Spreadsheet.Table tagSheetTable = _tagSheet.Tables.Add(tagTable, 0, 0, options);
            DevExpress.Spreadsheet.Table wbsSheetTable = _wbsSheet.Tables.Add(wbsTable, 0, 0, options);

            DataTable priorityTable = new DataTable("PriorityTable");
            priorityTable.Columns.Add("Category", typeof(string));
            priorityTable.Rows.Add(Variables.punchlistCategoryA);
            priorityTable.Rows.Add(Variables.punchlistCategoryB);
            priorityTable.Rows.Add(Variables.punchlistCategoryC);
            priorityTable.Rows.Add(Variables.punchlistCategoryD);

            DevExpress.Spreadsheet.Table prioritySheetTable = _prioritySheet.Tables.Add(priorityTable, 0, 0, options);

            DataTable actionByTable = new DataTable("ActionByTable");
            actionByTable.Columns.Add("ActionBy", typeof(string));
            List<string> ActionBys = Enum.GetNames(typeof(Punchlist_ActionBy)).ToList();
            foreach(string actionBy in ActionBys)
            {
                actionByTable.Rows.Add(actionBy);
            }

            DevExpress.Spreadsheet.Table actionBySheetTable = _actionBySheet.Tables.Add(actionByTable, 0, 0, options);

            //custom header
            DataTable punchlistTable = new DataTable("PunchlistTable");
            List<string> defaultHeaders = new List<string>();
            defaultHeaders.Add(_columnKey);
            defaultHeaders.Add(_columnImportStatus);
            defaultHeaders.Add(_columnTitle);
            defaultHeaders.Add(_columnDescription);
            defaultHeaders.Add(_columnPriority);
            defaultHeaders.Add(_columnActionBy);
            defaultHeaders.Add(_columnTag);
            defaultHeaders.Add(_columnWBS);

            foreach (string defaultHeader in defaultHeaders)
            {
                punchlistTable.Columns.Add(defaultHeader, typeof(string));
            }

            CellRange usedRange = _punchlistSheet.GetUsedRange();

            for (int i = 0; i < 1000; i++)
            {
                DataRow newTagRow = punchlistTable.Rows.Add();
            }

            // Insert a table in the worksheet.
            options.ImportHeaders = true;
            DevExpress.Spreadsheet.Table punchlistSpreadsheetTable = _punchlistSheet.Tables.Add(punchlistTable, 0, 0, options);
            _punchlistSheet.DataValidations.Add(punchlistSpreadsheetTable.Columns[_indexTag].DataRange, DataValidationType.List, ValueObject.FromRange(tagSheetTable.Columns[0].DataRange.GetRangeWithAbsoluteReference()));
            _punchlistSheet.DataValidations.Add(punchlistSpreadsheetTable.Columns[_indexWBS].DataRange, DataValidationType.List, ValueObject.FromRange(wbsSheetTable.Columns[0].DataRange.GetRangeWithAbsoluteReference()));
            _punchlistSheet.DataValidations.Add(punchlistSpreadsheetTable.Columns[_indexPriority].DataRange, DataValidationType.List, ValueObject.FromRange(prioritySheetTable.Columns[0].DataRange.GetRangeWithAbsoluteReference()));
            _punchlistSheet.DataValidations.Add(punchlistSpreadsheetTable.Columns[_indexActionBy].DataRange, DataValidationType.List, ValueObject.FromRange(actionBySheetTable.Columns[0].DataRange.GetRangeWithAbsoluteReference()));
            _punchlistSheet.Columns[0].Visible = false;

            string importStatusColumnAddress = punchlistSpreadsheetTable.Columns[_indexImportStatus].DataRange.First().GetRangeWithRelativeReference().GetReferenceA1();

            FormulaExpressionConditionalFormatting statusFieldSuccessConditionalFormatting = _punchlistSheet.ConditionalFormattings.AddFormulaExpressionConditionalFormatting(punchlistSpreadsheetTable.Columns[_indexImportStatus].DataRange, "=ISNUMBER(SEARCH(\"" + _prefixImportSuccess + "\", $" + importStatusColumnAddress + "))");
            statusFieldSuccessConditionalFormatting.Formatting.Fill.BackgroundColor = Color.LightGreen;

            FormulaExpressionConditionalFormatting statusFieldErrorConditionalFormatting = _punchlistSheet.ConditionalFormattings.AddFormulaExpressionConditionalFormatting(punchlistSpreadsheetTable.Columns[_indexImportStatus].DataRange, "=ISNUMBER(SEARCH(\"" + _prefixImportError + "\", $" + importStatusColumnAddress + "))");
            statusFieldErrorConditionalFormatting.Formatting.Fill.BackgroundColor = Color.Salmon;

            spreadsheetControl1.EndUpdate();
            splashScreenManager1.CloseWaitForm();
        }
    }
}
