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
using static ProjectCommon.Common;

namespace CheckmateDX
{
    public partial class frmMasterSheet : Form
    {
        DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        Guid _projectGuid;
        Guid _scheduleGuid;
        string _discipline;
        IWorkbook _workbook;
        Worksheet _masterSheet;
        Worksheet _metaSheet;

        string _prefixImportError = "ERROR: ";
        string _prefixImportSuccess = "SUCCESS: ";
        string _prefixProgressWritingHeaderData = "Writing Header Data for ";
        string _prefixProgressCheckingDuplicates = "Checking Duplicates for ";
        string _prefixProgressAddingNewTag = "Adding New Tag ";
        string _prefixProgressUpdatingTag = "Updating Tag: ";
        string _prefixProgressRestoringTag = "Restoring Tag: ";
        string _prefixProgressAddingNewWBS = "Adding New WBS ";
        string _prefixProgressUpdatingWBS = "Updating WBS: ";
        string _prefixProgressRestoringWBS = "Restoring WBS: ";

        string _columnKey;
        string _columnImportStatus;
        string _columnDelete;
        string _columnType;
        string _columnTagWBSName;
        string _columnMatrixType1;
        string _columnMatrixType2;
        string _columnMatrixType3;
        string _columnDescription;
        string _columnDiscipline;
        string _columnStage1Assigned;
        string _columnStage1Inspected;
        string _columnStage1Approved;
        string _columnStage1Completed;
        string _columnStage1Closed;

        string _columnStage2Assigned;
        string _columnStage2Inspected;
        string _columnStage2Approved;
        string _columnStage2Completed;
        string _columnStage2Closed;

        string _columnStage3Assigned;
        string _columnStage3Inspected;
        string _columnStage3Approved;
        string _columnStage3Completed;
        string _columnStage3Closed;

        string _columnAreaNumber;
        string _columnAreaDescription;
        string _columnSystemNumber;
        string _columnSystemDescription;
        string _columnSubSystemNumber;
        string _columnSubSystemDescription;

        string _fileName = "";
        bool _skipPopulatingPrefillData = false;
        bool _isBySchedule = false;
        bool _isDetailedReport = false;

        int _indexKey;
        int _indexImportStatus;
        int _indexDelete;
        int _indexType;
        int _indexTagWBSName;
        int _indexMatrixType1;
        int _indexMatrixType2;
        int _indexMatrixType3;
        int _indexDescription;
        int _indexStage1Assigned;
        int _indexStage1Inspected;
        int _indexStage1Approved;
        int _indexStage1Completed;
        int _indexStage1Closed;
        int _indexStage2Assigned;
        int _indexStage2Inspected;
        int _indexStage2Approved;
        int _indexStage2Completed;
        int _indexStage2Closed;
        int _indexStage3Assigned;
        int _indexStage3Inspected;
        int _indexStage3Approved;
        int _indexStage3Completed;
        int _indexStage3Closed;
        int _indexAreaNumber;
        int _indexAreaDescription;
        int _indexSystemNumber;
        int _indexSystemDescription;
        int _indexSubSystemNumber;
        int _indexSubSystemDescription;
        int _prefillStartIndex;

        AdapterPREFILL_REGISTER _daPrefillReg = new AdapterPREFILL_REGISTER();
        dsPREFILL_REGISTER _dsPrefillReg = new dsPREFILL_REGISTER();
        List<SpreadsheetCommandDelegateContainer> _replaceCommandMethod = new List<SpreadsheetCommandDelegateContainer>();
        DispatcherTimer delayedImportTimer = new DispatcherTimer();
        Action _refreshSchedule;
        public frmMasterSheet()
        {
            InitializeComponent();

            _workbook = spreadsheetControl1.Document;
            _masterSheet = _workbook.Worksheets[0];
            _metaSheet = _workbook.Worksheets.Add();
            SetReplaceCommandMethod();
        }

        public frmMasterSheet(Guid projectGuid, Guid scheduleGuid, string discipline, bool isBySchedule, string fileName = "", Action refreshSchedule = null, bool skipPopulatingPrefillData = false, bool isDetailedReport = false)
        {
            InitializeComponent();

            _skipPopulatingPrefillData = skipPopulatingPrefillData;
            _fileName = fileName;
            splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
            _projectGuid = projectGuid;
            _scheduleGuid = scheduleGuid;
            _discipline = discipline;
            _isBySchedule = isBySchedule;
            _refreshSchedule = refreshSchedule;
            _isDetailedReport = isDetailedReport;

            defineColumns(_isDetailedReport);
            SetReplaceCommandMethod();
            if (fileName == string.Empty)
            {
                _workbook = spreadsheetControl1.Document;
                _masterSheet = _workbook.Worksheets[0];
                _metaSheet = _workbook.Worksheets.Add();
                populateData(_scheduleGuid);
            }
            else
            {
                spreadsheetControl1.LoadDocument(fileName);
                _workbook = spreadsheetControl1.Document;
                _masterSheet = _workbook.Worksheets[0];
                if(_workbook.Worksheets.Count > 1)
                    _metaSheet = _workbook.Worksheets[1];

                delayedImportTimer.Interval = new TimeSpan(0, 0, 2);
                delayedImportTimer.Tick += DelayedImportTimer_Tick;
                delayedImportTimer.Start();
            }
        }

        private void defineColumns(bool isDetailedReport)
        {
            _columnKey = "Key (Read Only)";
            _columnImportStatus = "Import Status (Read Only)";
            _columnDelete = "DELETE";
            _columnType = "TAG/WBS";
            _columnTagWBSName = Variables.Header_TagWBS;
            _columnMatrixType1 = "Matrix Type";
            _columnMatrixType2 = "Matrix Type 2";
            _columnMatrixType3 = "Matrix Type 3";
            _columnDescription = "Description";
            _columnDiscipline = "Discipline";

            if(isDetailedReport)
            {
                _columnStage1Assigned = "Stage 1 Assigned (Read Only)";
                _columnStage1Inspected = "Stage 1 Inspected (Read Only)";
                _columnStage1Approved = "Stage 1 Approved (Read Only)";
                _columnStage1Completed = "Stage 1 Completed (Read Only)";
                _columnStage1Closed = "Stage 1 Closed (Read Only)";
            }
            else
            {
                _columnStage1Assigned = "Assigned (Read Only)";
                _columnStage1Inspected = "Inspected (Read Only)";
                _columnStage1Approved = "Approved (Read Only)";
                _columnStage1Completed = "Completed (Read Only)";
                _columnStage1Closed = "Closed (Read Only)";
            }

            _columnStage2Assigned = "Stage 2 Assigned (Read Only)";
            _columnStage2Inspected = "Stage 2 Inspected (Read Only)";
            _columnStage2Approved = "Stage 2 Approved (Read Only)";
            _columnStage2Completed = "Stage 2 Completed (Read Only)";
            _columnStage2Closed = "Stage 2 Closed (Read Only)";

            _columnStage3Assigned = "Stage 3 Assigned (Read Only)";
            _columnStage3Inspected = "Stage 3 Inspected (Read Only)";
            _columnStage3Approved = "Stage 3 Approved (Read Only)";
            _columnStage3Completed = "Stage 3 Completed (Read Only)";
            _columnStage3Closed = "Stage 3 Closed (Read Only)";

            _columnAreaNumber = Variables.prefillAreaNumber;
            _columnAreaDescription = Variables.prefillAreaDescription;
            _columnSystemNumber = Variables.prefillSystemNumber;
            _columnSystemDescription = Variables.prefillSystemDescription;
            _columnSubSystemNumber = Variables.prefillSubSystemNumber;
            _columnSubSystemDescription = Variables.prefillSubSystemDescription;

            _indexKey = 0;
            _indexImportStatus = 1;
            _indexDelete = 2;
            _indexType = 3;
            _indexTagWBSName = 4;
            _indexMatrixType1 = 5;
            _indexMatrixType2 = 6;
            _indexMatrixType3 = 7;
            _indexDescription = 8;
            _indexStage1Assigned = 9;
            _indexStage1Inspected = 10;
            _indexStage1Approved = 11;
            _indexStage1Completed = 12;
            _indexStage1Closed = 13;

            if(_isDetailedReport)
            {
                _indexStage2Assigned = 14;
                _indexStage2Inspected = 15;
                _indexStage2Approved = 16;
                _indexStage2Completed = 17;
                _indexStage2Closed = 18;
                _indexStage3Assigned = 19;
                _indexStage3Inspected = 20;
                _indexStage3Approved = 21;
                _indexStage3Completed = 22;
                _indexStage3Closed = 23;
                _indexAreaNumber = 24;
                _indexAreaDescription = 25;
                _indexSystemNumber = 26;
                _indexSystemDescription = 27;
                _indexSubSystemNumber = 28;
                _indexSubSystemDescription = 29;
                _prefillStartIndex = 24;
            }
            else
            {
                _indexAreaNumber = 14;
                _indexAreaDescription = 15;
                _indexSystemNumber = 16;
                _indexSystemDescription = 17;
                _indexSubSystemNumber = 18;
                _indexSubSystemDescription = 19;
                _prefillStartIndex = 14;
            }
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
            if(!_skipPopulatingPrefillData)
            {
                _replaceCommandMethod.Add(new SpreadsheetCommandDelegateContainer(SpreadsheetCommandId.FileSave, new SpreadsheetCommandDelegate(importSpreadsheet)));
                ISpreadsheetCommandFactoryService service = spreadsheetControl1.GetService(typeof(ISpreadsheetCommandFactoryService)) as ISpreadsheetCommandFactoryService;
                spreadsheetControl1.ReplaceService<ISpreadsheetCommandFactoryService>(new CustomSpreadsheetCommandFactoryService(service, spreadsheetControl1, _replaceCommandMethod));
            }
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

        private bool isWBSTagDuplicate(string WBSTagName, string Type)
        {
            DevExpress.Spreadsheet.Table masterSpreadsheetTable = _masterSheet.Tables[0];

            for (int row = 1; row < masterSpreadsheetTable.DataRange.BottomRowIndex; row++)
            {
                var scanRow = _masterSheet.Rows[row];
                string currentRowKey = scanRow[_indexKey].DisplayText;
                if (currentRowKey == string.Empty)
                    continue;

                string currentRowDeleteStatus = scanRow[_indexDelete].DisplayText;
                if (currentRowDeleteStatus == "Y")
                    continue;

                string currentRowType = scanRow[_indexType].DisplayText;
                if (currentRowType != Type)
                    continue;

                string currentRowWBSTagName = scanRow[_indexTagWBSName].DisplayText;
                if (currentRowWBSTagName == WBSTagName)
                    return true;
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
                string headerString = _masterSheet.Cells[0, columnIndex].DisplayText.Trim();
                dsPREFILL_REGISTER.PREFILL_REGISTERRow drPrefillReg = dtPrefill.FirstOrDefault(x => x.NAME == headerString);

                if (_masterSheet.Cells[rowIndex, columnIndex].DisplayText != string.Empty)
                {
                    bool addRow = false;
                    bool shouldSync = false;
                    if (drPrefillReg == null)
                    {
                        drPrefillReg = dtPrefill.NewPREFILL_REGISTERRow();
                        drPrefillReg.GUID = Guid.NewGuid();

                        addRow = true;
                    }
                    //undelete it and set updated to trigger sync
                    if(!drPrefillReg.IsDELETEDNull())
                    {
                        drPrefillReg.SetDELETEDNull();
                        drPrefillReg.SetDELETEDBYNull();
                        shouldSync = true;
                    }

                    if (prefillType == PrefillType.Tag)
                        drPrefillReg.TAG_GUID = prefillGuid;
                    else
                        drPrefillReg.WBS_GUID = prefillGuid;

                    //when it's the same do not tag it as updated to speed up syncing
                    if (!drPrefillReg.IsDATANull())
                    {
                        shouldSync = drPrefillReg.DATA == headerString;
                    }

                    drPrefillReg.NAME = headerString;
                    drPrefillReg.DATA = _masterSheet.Cells[rowIndex, columnIndex].DisplayText.Trim();

                    if (addRow)
                    {
                        drPrefillReg.CREATED = DateTime.Now;
                        drPrefillReg.CREATEDBY = System_Environment.GetUser().GUID;
                        dtPrefill.AddPREFILL_REGISTERRow(drPrefillReg);
                    }
                    else if(shouldSync)
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
            XMLDatabase XmlDatabase = Common.LoadDatabaseXML(false);
            if(XmlDatabase != null && XmlDatabase.Server == ".")
            {
                Warn("Cannot save spreadsheet because database is local, please connect to server to edit and save");
                return;
            }

            spreadsheetControl1.BeginUpdate();
            AdapterTAG daTag = new AdapterTAG();
            AdapterWBS daWBS = new AdapterWBS();
            AdapterMATRIX_ASSIGNMENT daMATRIX_ASSIGNMENT = new AdapterMATRIX_ASSIGNMENT();
            AdapterTEMPLATE_REGISTER daTEMPLATE_REGISTER = new AdapterTEMPLATE_REGISTER();

            dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPEDataTable dtMATRIX = daMATRIX_ASSIGNMENT.Get_Assignments_Table(_projectGuid);
            dsPREFILL_REGISTER dsPrefillReg = new dsPREFILL_REGISTER();
            _workbook = spreadsheetControl1.Document;
            _masterSheet = _workbook.Worksheets[0];
            if (_workbook.Worksheets.Count > 1)
                _metaSheet = _workbook.Worksheets[1];

            if (_masterSheet == null || _masterSheet.Tables == null || _masterSheet.Tables.Count == 0)
            {
                MessageBox.Show("Table not detected in master sheet");
                return;
            }

            if(_metaSheet == null || _metaSheet.Tables == null || _metaSheet.Tables.Count == 0)
            {
                MessageBox.Show("Table not detected in meta sheet");
                return;
            }

            DevExpress.Spreadsheet.Table masterSpreadsheetTable = _masterSheet.Tables[0];
            dsTAG dsTAG = new dsTAG();
            dsWBS dsWBS = new dsWBS();

            try
            {
                List<ViewModel_ITRSummary> reportStats = new List<ViewModel_ITRSummary>();
                using (AdapterITR_MAIN daITR_Main = new AdapterITR_MAIN())
                {
                    if (_isDetailedReport)
                        reportStats = daITR_Main.GenerateITRStagedSummary(_projectGuid);
                    else
                        reportStats = daITR_Main.GenerateITRSummary(_projectGuid);
                }

                int lastRowIndex = masterSpreadsheetTable.DataRange.BottomRowIndex;
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Saving ...");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, lastRowIndex);
                for (int row=1;row <= lastRowIndex; row++)
                {
                    string wbsTagType = _masterSheet.Cells[row, _indexType].DisplayText;
                    string wbsTagKey = _masterSheet.Cells[row, _indexKey].DisplayText;
                    string wbsTagDelete = _masterSheet.Cells[row, _indexDelete].DisplayText;
                    if (wbsTagKey != string.Empty && wbsTagKey.Length != Guid.Empty.ToString().Length)
                        return;

                    if (wbsTagType == "TAG")
                    {
                        if (wbsTagDelete == "Y")
                        {
                            string deleteTagKey = _masterSheet.Cells[row, _indexDelete].DisplayText;
                            daTag.RemoveBy(new Guid(wbsTagKey));

                            dsTAG.TAGRow deletedTag = daTag.GetIncludeDeletedBy(new Guid(wbsTagKey));
                            if (deletedTag != null)
                            {
                                splashScreenManager1.SetWaitFormDescription(_prefixProgressWritingHeaderData + deletedTag.NUMBER);
                                dsPREFILL_REGISTER.PREFILL_REGISTERDataTable prefillTable = _daPrefillReg.GetByWBSTag(deletedTag.GUID, PrefillType.Tag);
                                AddPrefill(prefillTable, deletedTag.GUID, PrefillType.Tag, masterSpreadsheetTable.HeaderRowRange, row, _prefillStartIndex);
                                _daPrefillReg.Save(prefillTable);

                                _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportSuccess + "Tag Deleted";
                            }
                            else
                                _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "Tag Not Found";
                        }
                        else if (wbsTagKey == string.Empty)
                        {
                            if(_discipline == Variables.allDiscipline)
                                _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "New Tag cannot be added in All Discipline mode because schedule cannot be determined";
                            else
                            {
                                string newTagNumber = _masterSheet.Cells[row, _indexTagWBSName].DisplayText;
                                if (newTagNumber != string.Empty)
                                {
                                    splashScreenManager1.SetWaitFormDescription(_prefixProgressCheckingDuplicates + newTagNumber);
                                    if (isWBSTagDuplicate(newTagNumber, wbsTagType))
                                    {
                                        _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "Duplicate Tag Exists";
                                        continue;
                                    }
                                    else
                                    {
                                        dsTAG.TAGRow drTAG = daTag.GetBy(newTagNumber, _projectGuid);
                                        if (drTAG != null)
                                        {
                                            _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "Duplicate Tag Exists in Database";
                                            continue;
                                        }
                                    }

                                    dsTAG.TAGRow existingTag = daTag.GetByProjectDiscipline(_projectGuid, _discipline, newTagNumber);
                                    if (existingTag == null)
                                    {
                                        string wbsErrorString;
                                        Guid? parentGuid = null;
                                        Guid? wbsGuid = assignToWBS(row, out parentGuid, out wbsErrorString);
                                        splashScreenManager1.SetWaitFormDescription(_prefixProgressAddingNewTag + newTagNumber);
                                        string newTagDescription = _masterSheet.Cells[row, _indexDescription].DisplayText;
                                        string matrixType1 = _masterSheet.Cells[row, _indexMatrixType1].DisplayText;
                                        string matrixType2 = _masterSheet.Cells[row, _indexMatrixType2].DisplayText;
                                        string matrixType3 = _masterSheet.Cells[row, _indexMatrixType3].DisplayText;

                                        dsTAG.TAGRow drTag = dsTAG.TAG.NewTAGRow();
                                        Guid newGuid = Guid.NewGuid();
                                        drTag.GUID = newGuid;
                                        drTag.NUMBER = newTagNumber;
                                        drTag.DESCRIPTION = newTagDescription;
                                        drTag.SCHEDULEGUID = _scheduleGuid;
                                        drTag.PARENTGUID = parentGuid == null ? Guid.Empty : (Guid)parentGuid;
                                        drTag.WBSGUID = wbsGuid == null ? Guid.Empty : (Guid)wbsGuid;
                                        drTag.TYPE1 = matrixType1;
                                        drTag.TYPE2 = matrixType2;
                                        drTag.TYPE3 = matrixType3;
                                        drTag.CREATED = DateTime.Now;
                                        drTag.CREATEDBY = System_Environment.GetUser().GUID;
                                        dsTAG.TAG.AddTAGRow(drTag);
                                        daTag.Save(drTag);

                                        wbsTagKey = newGuid.ToString();
                                        _masterSheet.Cells[row, _indexKey].Value = wbsTagKey;
                                        splashScreenManager1.SetWaitFormDescription(_prefixProgressWritingHeaderData + drTag.NUMBER);
                                        dsPREFILL_REGISTER.PREFILL_REGISTERDataTable prefillTable = _daPrefillReg.GetByWBSTag(drTag.GUID, PrefillType.Tag);
                                        AddPrefill(prefillTable, drTag.GUID, PrefillType.Tag, masterSpreadsheetTable.HeaderRowRange, row, _prefillStartIndex);
                                        _daPrefillReg.Save(prefillTable);
                                        daTEMPLATE_REGISTER.SetNull_OnTag(newGuid);
                                        assignTagMatrix(daTEMPLATE_REGISTER, dtMATRIX, newGuid, matrixType1);
                                        assignTagMatrix(daTEMPLATE_REGISTER, dtMATRIX, newGuid, matrixType2);
                                        assignTagMatrix(daTEMPLATE_REGISTER, dtMATRIX, newGuid, matrixType3);
                                        _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportSuccess + "Tag Added" + wbsErrorString;
                                    }
                                    else
                                    {
                                        _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "Tag Not Found";
                                    }
                                }
                            }
                        }
                        else
                        {
                            dsTAG.TAGRow existingTag = daTag.GetByProjectDisciplineIncludeDeleted(_projectGuid, _discipline, new Guid(wbsTagKey));
                            if(existingTag != null)
                            {
                                splashScreenManager1.SetWaitFormDescription(_prefixProgressUpdatingTag + existingTag.NUMBER);
                                string matrixType1 = _masterSheet.Cells[row, _indexMatrixType1].DisplayText;
                                string matrixType2 = _masterSheet.Cells[row, _indexMatrixType2].DisplayText;
                                string matrixType3 = _masterSheet.Cells[row, _indexMatrixType3].DisplayText;

                                bool isRestored = false;
                                if (!existingTag.IsDELETEDNull())
                                {
                                    splashScreenManager1.SetWaitFormDescription(_prefixProgressRestoringTag + existingTag.NUMBER);
                                    existingTag.SetDELETEDBYNull();
                                    existingTag.SetDELETEDNull();

                                    //set updated to now so that sync will retrieve this
                                    existingTag.UPDATED = DateTime.Now;
                                    existingTag.UPDATEDBY = System_Environment.GetUser().GUID;
                                    isRestored = true;
                                }

                                ViewModel_ITRSummary reportStat = reportStats.FirstOrDefault(x => x.GuidKey == existingTag.GUID.ToString());
                                if (reportStat != null)
                                {
                                    if(_isDetailedReport)
                                    {
                                        _masterSheet.Cells[row, _indexStage1Assigned].Value = reportStat.Stage1Assigned;
                                        _masterSheet.Cells[row, _indexStage1Inspected].Value = reportStat.Stage1Inspected;
                                        _masterSheet.Cells[row, _indexStage1Approved].Value = reportStat.Stage1Approved;
                                        _masterSheet.Cells[row, _indexStage1Completed].Value = reportStat.Stage1Completed;
                                        _masterSheet.Cells[row, _indexStage1Closed].Value = reportStat.Stage1Closed;

                                        _masterSheet.Cells[row, _indexStage2Assigned].Value = reportStat.Stage2Assigned;
                                        _masterSheet.Cells[row, _indexStage2Inspected].Value = reportStat.Stage2Inspected;
                                        _masterSheet.Cells[row, _indexStage2Approved].Value = reportStat.Stage2Approved;
                                        _masterSheet.Cells[row, _indexStage2Completed].Value = reportStat.Stage2Completed;
                                        _masterSheet.Cells[row, _indexStage2Closed].Value = reportStat.Stage2Closed;

                                        _masterSheet.Cells[row, _indexStage3Assigned].Value = reportStat.Stage3Assigned;
                                        _masterSheet.Cells[row, _indexStage3Inspected].Value = reportStat.Stage3Inspected;
                                        _masterSheet.Cells[row, _indexStage3Approved].Value = reportStat.Stage3Approved;
                                        _masterSheet.Cells[row, _indexStage3Completed].Value = reportStat.Stage3Completed;
                                        _masterSheet.Cells[row, _indexStage3Closed].Value = reportStat.Stage3Closed;
                                    }
                                    else
                                    {
                                        _masterSheet.Cells[row, _indexStage1Assigned].Value = reportStat.Assigned;
                                        _masterSheet.Cells[row, _indexStage1Inspected].Value = reportStat.Inspected;
                                        _masterSheet.Cells[row, _indexStage1Approved].Value = reportStat.Approved;
                                        _masterSheet.Cells[row, _indexStage1Completed].Value = reportStat.Completed;
                                        _masterSheet.Cells[row, _indexStage1Closed].Value = reportStat.Closed;
                                    }
                                }

                                string existingTagDescription = _masterSheet.Cells[row, _indexDescription].DisplayText;
                                string newTagNumber = _masterSheet.Cells[row, _indexTagWBSName].DisplayText;
                                string wbsErrorString;
                                Guid? parentGuid = null;
                                Guid? wbsGuid = assignToWBS(row, out parentGuid, out wbsErrorString);

                                existingTag.NUMBER = newTagNumber;
                                existingTag.DESCRIPTION = existingTagDescription;
                                existingTag.PARENTGUID = parentGuid == null ? Guid.Empty : (Guid)parentGuid;
                                existingTag.WBSGUID = wbsGuid == null ? Guid.Empty : (Guid)wbsGuid;
                                existingTag.TYPE1 = matrixType1;
                                existingTag.TYPE2 = matrixType2;
                                existingTag.TYPE3 = matrixType3;
                                existingTag.UPDATED = DateTime.Now;
                                existingTag.UPDATEDBY = System_Environment.GetUser().GUID;
                                daTag.Save(existingTag);
                                splashScreenManager1.SetWaitFormDescription(_prefixProgressWritingHeaderData + existingTag.NUMBER);
                                dsPREFILL_REGISTER.PREFILL_REGISTERDataTable prefillTable = _daPrefillReg.GetByWBSTag(existingTag.GUID, PrefillType.Tag);
                                AddPrefill(prefillTable, existingTag.GUID, PrefillType.Tag, masterSpreadsheetTable.HeaderRowRange, row, _prefillStartIndex);
                                _daPrefillReg.Save(prefillTable);
                                daTEMPLATE_REGISTER.SetNull_OnTag(existingTag.GUID);
                                assignTagMatrix(daTEMPLATE_REGISTER, dtMATRIX, existingTag.GUID, matrixType1);
                                assignTagMatrix(daTEMPLATE_REGISTER, dtMATRIX, existingTag.GUID, matrixType2);
                                assignTagMatrix(daTEMPLATE_REGISTER, dtMATRIX, existingTag.GUID, matrixType3);

                                if (isRestored)
                                    _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportSuccess + "Tag Restored " + wbsErrorString;
                                else 
                                    _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportSuccess + "Tag Updated " + wbsErrorString;
                            }
                            else
                            {
                                _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "Tag Not Found";
                            }
                        }
                    }
                    else if (wbsTagType == "WBS")
                    {
                        if (wbsTagDelete == "Y")
                        {
                            string deleteWBSKey = _masterSheet.Cells[row, _indexDelete].DisplayText;
                            daWBS.RemoveBy(new Guid(wbsTagKey));

                            dsWBS.WBSRow deletedWBS = daWBS.GetIncludeDeletedBy(new Guid(wbsTagKey));
                            if (deletedWBS != null)
                            {
                                splashScreenManager1.SetWaitFormDescription(_prefixProgressWritingHeaderData + deletedWBS.NAME);
                                dsPREFILL_REGISTER.PREFILL_REGISTERDataTable prefillTable = _daPrefillReg.GetByWBSTag(deletedWBS.GUID, PrefillType.WBS);
                                AddPrefill(prefillTable, deletedWBS.GUID, PrefillType.WBS, masterSpreadsheetTable.HeaderRowRange, row, _prefillStartIndex);
                                _daPrefillReg.Save(prefillTable);
                                _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportSuccess + "WBS Deleted";
                            }
                            else
                                _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "WBS Not Found";
                        }
                        else if (wbsTagKey == string.Empty)
                        {
                            string newWBSName = _masterSheet.Cells[row, _indexTagWBSName].DisplayText;
                            if (newWBSName != string.Empty)
                            {
                                splashScreenManager1.SetWaitFormDescription(_prefixProgressCheckingDuplicates + newWBSName);
                                if (isWBSTagDuplicate(newWBSName, wbsTagType))
                                {
                                    _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "Duplicate WBS Exists";
                                    continue;
                                }
                                else
                                {
                                    dsWBS.WBSRow drWBS = daWBS.GetBy(newWBSName, _projectGuid);
                                    if (drWBS != null)
                                    {
                                        _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "Duplicate WBS Exists in Database";
                                        continue;
                                    }
                                }

                                dsWBS.WBSRow existingWBS = daWBS.GetByProjectDiscipline(_projectGuid, _discipline, newWBSName);
                                if (existingWBS == null)
                                {
                                    splashScreenManager1.SetWaitFormDescription(_prefixProgressAddingNewWBS + newWBSName);
                                    string newWBSDescription = _masterSheet.Cells[row, _indexDescription].DisplayText;
                                    string newWBSMatrixType = _masterSheet.Cells[row, _indexMatrixType1].DisplayText;

                                    dsWBS.WBSRow drWBS = dsWBS.WBS.NewWBSRow();
                                    Guid newGuid = Guid.NewGuid();
                                    drWBS.GUID = newGuid;
                                    drWBS.NAME = newWBSName;
                                    drWBS.DESCRIPTION = newWBSDescription;
                                    drWBS.PARENTGUID = Guid.Empty;
                                    drWBS.SCHEDULEGUID = _scheduleGuid;
                                    drWBS.CREATED = DateTime.Now;
                                    drWBS.CREATEDBY = System_Environment.GetUser().GUID;
                                    dsWBS.WBS.AddWBSRow(drWBS);
                                    daWBS.Save(drWBS);

                                    _masterSheet.Cells[row, _indexKey].Value = newGuid.ToString();

                                    wbsTagKey = newGuid.ToString();
                                    splashScreenManager1.SetWaitFormDescription(_prefixProgressWritingHeaderData + newWBSName);
                                    dsPREFILL_REGISTER.PREFILL_REGISTERDataTable prefillTable = _daPrefillReg.GetByWBSTag(drWBS.GUID, PrefillType.WBS);
                                    AddPrefill(prefillTable, drWBS.GUID, PrefillType.WBS, masterSpreadsheetTable.HeaderRowRange, row, _prefillStartIndex);
                                    _daPrefillReg.Save(prefillTable);
                                    _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportSuccess + "WBS Added";
                                }
                                else
                                    _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "WBS Not Found";
                            }
                        }
                        else
                        {
                            string existingWBSKey = _masterSheet.Cells[row, _indexKey].DisplayText;
                            if (existingWBSKey != string.Empty)
                            {
                                dsWBS.WBSRow existingWBS = daWBS.GetByProjectDisciplineIncludingDeleted(_projectGuid, _discipline, new Guid(existingWBSKey));
                                if (existingWBS != null)
                                {
                                    splashScreenManager1.SetWaitFormDescription(_prefixProgressUpdatingWBS + existingWBS.NAME);
                                    bool isRestored = false;
                                    string existingWBSDescription = _masterSheet.Cells[row, _indexDescription].DisplayText;
                                    if (!existingWBS.IsDELETEDNull())
                                    {
                                        splashScreenManager1.SetWaitFormDescription(_prefixProgressRestoringWBS + existingWBS.NAME);
                                        isRestored = true;
                                        existingWBS.SetDELETEDBYNull();
                                        existingWBS.SetDELETEDNull();
                                        existingWBS.UPDATED = DateTime.Now;
                                        existingWBS.UPDATEDBY = System_Environment.GetUser().GUID;
                                        daWBS.Save(existingWBS);
                                    }
                                    else if(existingWBS.DESCRIPTION != existingWBSDescription)
                                    {
                                        existingWBS.DESCRIPTION = existingWBSDescription;
                                        daWBS.Save(existingWBS);
                                    }

                                    splashScreenManager1.SetWaitFormDescription(_prefixProgressWritingHeaderData + existingWBS.NAME);
                                    dsPREFILL_REGISTER.PREFILL_REGISTERDataTable prefillTable = _daPrefillReg.GetByWBSTag(existingWBS.GUID, PrefillType.WBS);
                                    AddPrefill(prefillTable, existingWBS.GUID, PrefillType.WBS, masterSpreadsheetTable.HeaderRowRange, row, _prefillStartIndex);
                                    _daPrefillReg.Save(prefillTable);

                                    if (isRestored)
                                        _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportSuccess + "WBS Restored";
                                    else
                                        _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportSuccess + "WBS Updated";
                                }
                                else
                                    _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "WBS Not Found";
                            }
                        }
                    }
                    else
                    {
                        string newTagNumber = _masterSheet.Cells[row, _indexTagWBSName].DisplayText;
                        if(newTagNumber != string.Empty)
                            _masterSheet.Cells[row, _indexImportStatus].Value = _prefixImportError + "Tag/WBS Type Not Entered";
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
                daWBS.Dispose();
                daTEMPLATE_REGISTER.Dispose();
                daMATRIX_ASSIGNMENT.Dispose();
                try
                {
                    if (_fileName == string.Empty)
                        MessageBox.Show("Please save this file as new master feed sheet");
                    else
                        spreadsheetControl1.SaveDocument(_fileName);
                }
                catch
                {
                    MessageBox.Show("File is in use, please save this spreadsheet manually as new master feed sheet");
                }

                spreadsheetControl1.EndUpdate();
            }

            _refreshSchedule?.Invoke();
        }

        //to avoid querying the database repetitively
        List<ImportWBS> importWBS = new List<ImportWBS>();
        private Guid? assignToWBS(int rowNumber, out Guid? parentWBSGuid, out string wbsErrorString)
        {
            string areaNumber = _masterSheet.Cells[rowNumber, _indexAreaNumber].DisplayText;
            string areaDescription = _masterSheet.Cells[rowNumber, _indexAreaDescription].DisplayText;
            string systemNumber = _masterSheet.Cells[rowNumber, _indexSystemNumber].DisplayText;
            string systemDescription = _masterSheet.Cells[rowNumber, _indexSystemDescription].DisplayText;
            string subSystemNumber = _masterSheet.Cells[rowNumber, _indexSubSystemNumber].DisplayText;
            string subSystemDescription = _masterSheet.Cells[rowNumber, _indexSubSystemDescription].DisplayText;

            using (AdapterWBS daWBS = new AdapterWBS())
            {
                if (areaNumber != string.Empty && systemNumber != string.Empty && subSystemNumber != string.Empty)
                {
                    ImportWBS existingImport = importWBS.FirstOrDefault(x => x.AreaNumber == areaNumber && x.SystemNumber == systemNumber && x.SubSystemNumber == subSystemNumber);
                    if (existingImport != null)
                    {
                        wbsErrorString = string.Empty;
                        parentWBSGuid = existingImport.SubSystemGuid;
                        return existingImport.AreaGuid;
                    }
                    else
                    {
                        wbsErrorString = string.Empty;
                        Guid areaGuid = getExistingOrAddWBS(daWBS, Guid.Empty, areaNumber, areaDescription);
                        Guid systemGuid = getExistingOrAddWBS(daWBS, areaGuid, systemNumber, systemDescription);
                        Guid subSystemGuid = getExistingOrAddWBS(daWBS, systemGuid, subSystemNumber, subSystemDescription);
                        importWBS.Add(new ImportWBS() { AreaNumber = areaNumber, AreaGuid = areaGuid, SystemNumber = systemNumber, SystemGuid = systemGuid, SubSystemNumber = subSystemNumber, SubSystemGuid = subSystemGuid });
                        parentWBSGuid = subSystemGuid;
                        return areaGuid;
                    }
                }
                else if (areaNumber != string.Empty && systemNumber != string.Empty)
                {
                    ImportWBS existingImport = importWBS.FirstOrDefault(x => x.AreaNumber == areaNumber && x.SystemNumber == systemNumber);
                    if (existingImport != null)
                    {
                        wbsErrorString = string.Empty;
                        parentWBSGuid = existingImport.SystemGuid;
                        return existingImport.AreaGuid;
                    }
                    else
                    {
                        wbsErrorString = string.Empty;
                        Guid areaGuid = getExistingOrAddWBS(daWBS, Guid.Empty, areaNumber, areaDescription);
                        Guid systemGuid = getExistingOrAddWBS(daWBS, areaGuid, systemNumber, systemDescription);
                        importWBS.Add(new ImportWBS() { AreaNumber = areaNumber, AreaGuid = areaGuid, SystemNumber = systemNumber, SystemGuid = systemGuid });
                        parentWBSGuid = systemGuid;
                        return areaGuid;
                    }
                }
                else if (areaNumber != string.Empty)
                {
                    ImportWBS existingImport = importWBS.FirstOrDefault(x => x.AreaNumber == areaNumber);
                    if (existingImport != null)
                    {
                        wbsErrorString = string.Empty;
                        parentWBSGuid = existingImport.AreaGuid;
                        return existingImport.AreaGuid;
                    }
                    else
                    {
                        wbsErrorString = string.Empty;
                        Guid areaGuid = getExistingOrAddWBS(daWBS, Guid.Empty, areaNumber, areaDescription);
                        importWBS.Add(new ImportWBS() { AreaNumber = areaNumber, AreaGuid = areaGuid });
                        parentWBSGuid = areaGuid;
                        return areaGuid;
                    }
                }
                else
                {
                    wbsErrorString = string.Empty;
                    parentWBSGuid = null;
                    return null;
                }
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
                dtGENERIC_MATRIX_TYPE = dtMATRIX.AsEnumerable().Where(obj => obj.TYPE.ToUpper() == matrixType.ToUpper()).CopyToDataTable();
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
            AdapterSCHEDULE daSchedule = new AdapterSCHEDULE();
            AdapterTAG daTag = new AdapterTAG();
            AdapterWBS daWBS = new AdapterWBS();
            AdapterTEMPLATE_MAIN daTemplate = new AdapterTEMPLATE_MAIN();
            AdapterPREFILL_MAIN daPrefill = new AdapterPREFILL_MAIN();
            AdapterMATRIX_TYPE daMatrix_Type = new AdapterMATRIX_TYPE();
            AdapterMATRIX_ASSIGNMENT daMatrix_Assignment = new AdapterMATRIX_ASSIGNMENT();
            List<wbsTagHeader> wbsTags = new List<ProjectLibrary.wbsTagHeader>();
            List<string> allUniqueHeaders = new List<string>();
            List<string> punchlistPrefills = new List<string>();
            dsSCHEDULE.SCHEDULERow drSchedule = daSchedule.GetBy(scheduleGuid);

            splashScreenManager1.ShowWaitForm();
            //establish exclusion list - prefill that is used by the system
            List<string> ExcludedPrefills = new List<string>();
            ExcludedPrefills.Add("<<" + Variables.prefillTagNumber + ">>");
            ExcludedPrefills.Add("<<" + Variables.prefillTagDescription + ">>");
            ExcludedPrefills.Add("<<" + Variables.prefillProjNumber + ">>");
            ExcludedPrefills.Add("<<" + Variables.prefillProjName + ">>");
            ExcludedPrefills.Add("<<" + Variables.prefillProjClient + ">>");
            ExcludedPrefills.Add("<<" + Variables.prefillDocumentName + ">>");
            ExcludedPrefills.Add("<<" + Variables.prefillDate + ">>");
            ExcludedPrefills.Add("<<" + Variables.prefillDateTime + ">>");
            ExcludedPrefills.Add("<<" + Variables.prefillChild + ">>");
            ExcludedPrefills.Add("#"); //page number

            dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMatrix_Type;
            if (_discipline == Variables.allDiscipline)
                dtMatrix_Type = daMatrix_Type.Get();
            else
                dtMatrix_Type = daMatrix_Type.GetBy_Discipline(_discipline);

            if (dtMatrix_Type == null)
            {
                dtMatrix_Type = new dsMATRIX_TYPE.MATRIX_TYPEDataTable();
                //splashScreenManager1.CloseWaitForm();
                //spreadsheetControl1.EndUpdate();
                //return;
            }


            int matrixTypeRowCount = dtMatrix_Type.Rows.Count;
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, matrixTypeRowCount);
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
            RichEditControl richEditControl1 = new RichEditControl();
            Dictionary<Guid, List<string>> cachedTemplatesHeaders = new Dictionary<Guid, List<string>>();
            Dictionary<string, List<string>> typeAssignments = new Dictionary<string, List<string>>();

            if(!_skipPopulatingPrefillData)
            {
                for (int i = 0; i < matrixTypeRowCount; i++)
                {
                    dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtMatrix_Assignment = daMatrix_Assignment.GetBy_Type(_projectGuid, dtMatrix_Type[i].GUID);
                    if (dtMatrix_Assignment != null)
                    {
                        List<string> currentTypeHeaderData = new List<string>();
                        foreach (dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow drMatrix_Assignment in dtMatrix_Assignment.Rows)
                        {
                            List<string> headerData = null;
                            //try to look for cached headers to reduce SQL load
                            KeyValuePair<Guid, List<string>> cachedTemplateHeaders = cachedTemplatesHeaders.FirstOrDefault(x => x.Key == drMatrix_Assignment.GUID_TEMPLATE);

                            if (cachedTemplateHeaders.Value != null)
                                headerData = cachedTemplateHeaders.Value;
                            else
                            {
                                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplateMain = daTemplate.GetBy(drMatrix_Assignment.GUID_TEMPLATE);
                                headerData = new List<string>();
                                if (drTemplateMain != null && !drTemplateMain.IsTEMPLATENull())
                                {
                                    byte[] receivedBytes = drTemplateMain.TEMPLATE;
                                    MemoryStream ms = new MemoryStream(receivedBytes);
                                    richEditControl1.Document.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);

                                    Common.RetrievePrefillFields(richEditControl1, null, ExcludedPrefills, allUniqueHeaders, headerData);
                                    cachedTemplatesHeaders.Add(drTemplateMain.GUID, headerData);
                                }
                            }

                            currentTypeHeaderData.AddRange(headerData);
                        }

                        //load document
                        KeyValuePair<string, List<string>> valuePair = typeAssignments.FirstOrDefault(x => x.Key == dtMatrix_Type[i].NAME);
                        if (valuePair.Key == null)
                            typeAssignments.Add(dtMatrix_Type[i].NAME, currentTypeHeaderData);
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }
            }

            splashScreenManager1.SetWaitFormCaption("Writing Assignments ... ");
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, typeAssignments.Count);
            int rowOffset = 0;

            DataTable metaTable = new DataTable("MetaTable");
            metaTable.Columns.Add(_columnMatrixType1, typeof(string));

            List<string> allHeaders = typeAssignments.SelectMany(x => x.Value).ToList();
            allHeaders = allHeaders.OrderBy(x => x).ToList();

            foreach (string header in allHeaders)
            {
                string formattedHeaderString = Common.FormatFieldString(header);
                if (!metaTable.Columns.Contains(formattedHeaderString))
                    metaTable.Columns.Add(formattedHeaderString, typeof(int));
            }

            foreach (var typeAssignment in typeAssignments.OrderBy(x => x.Key))
            {
                DataRow newMatrixTypeRow = metaTable.Rows.Add(typeAssignment.Key);

                _metaSheet.Cells[rowOffset, 0].Value = typeAssignment.Key.ToString();
                List<string> headerStrings = typeAssignment.Value;
                headerStrings = headerStrings.OrderBy(x => x).ToList();

                foreach (string headerString in headerStrings)
                {
                    if (headerString != string.Empty)
                    {
                        string formattedHeaderString = Common.FormatFieldString(headerString);
                        newMatrixTypeRow[formattedHeaderString] = 1;
                    }
                }

                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            ExternalDataSourceOptions options = new ExternalDataSourceOptions();
            options.ImportHeaders = true;
            DevExpress.Spreadsheet.Table metaSpreadsheetTable = _metaSheet.Tables.Add(metaTable, 0, 0, options);

            if (!_skipPopulatingPrefillData && drSchedule == null)
                return;
            try
            {
                _workbook.Worksheets.ActiveWorksheet = _workbook.Worksheets[0];
                splashScreenManager1.SetWaitFormCaption("Loading Tag Numbers ... ");
                //retrieve tag
                dsTAG.TAGDisciplineDataTable dtTag;
                if (_isBySchedule)
                    dtTag = daTag.GetBySchedule(_scheduleGuid, _discipline);
                else
                {
                    if (_discipline == Variables.allDiscipline)
                        dtTag = daTag.GetByProjectIncludeDiscipline(_projectGuid);
                    else
                        dtTag = daTag.GetByProjectDisciplineIncludeDiscipline(_projectGuid, _discipline);
                }

                if (dtTag != null)
                {
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtTag.Rows.Count);
                    foreach (dsTAG.TAGDisciplineRow drTag in dtTag.Rows)
                    {
                        wbsTags.Add(new wbsTagHeader(new Tag(drTag.GUID)
                        {
                            tagNumber = drTag.NUMBER,
                            tagDescription = drTag.DESCRIPTION,
                            tagParentGuid = drTag.PARENTGUID,
                            tagScheduleGuid = drTag.SCHEDULEGUID,
                            tagDiscipline = drTag.DISCIPLINE,
                            tagType1 = drTag.IsTYPE1Null() ? string.Empty : drTag.TYPE1,
                            tagType2 = drTag.IsTYPE2Null() ? string.Empty : drTag.TYPE2,
                            tagType3 = drTag.IsTYPE3Null() ? string.Empty : drTag.TYPE3,
                            tagIsDeleted = !drTag.IsDELETEDNull()
                        }));

                        splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                    }
                }

                //retrieve wbs
                dsWBS.WBSDisciplineDataTable dtWBS;
                if (_isBySchedule)
                    dtWBS = daWBS.GetByScheduleIncludeDiscipline(_scheduleGuid, _discipline);
                else
                {
                    if (_discipline == Variables.allDiscipline)
                        dtWBS = daWBS.GetByProjectIncludeDiscipline(_projectGuid);
                    else
                        dtWBS = daWBS.GetByProjectDisciplineIncludeDiscipline(_projectGuid, _discipline);
                }

                splashScreenManager1.SetWaitFormCaption("Loading WBSes ... ");
                if (dtWBS != null)
                {
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, dtWBS.Rows.Count);
                    foreach (dsWBS.WBSDisciplineRow drWBS in dtWBS.Rows)
                    {
                        wbsTags.Add(new wbsTagHeader(new WBS(drWBS.GUID)
                        {
                            wbsName = drWBS.NAME,
                            wbsDescription = drWBS.IsDESCRIPTIONNull() ? "" : drWBS.DESCRIPTION,
                            wbsParentGuid = drWBS.PARENTGUID,
                            wbsScheduleGuid = drWBS.SCHEDULEGUID,
                            wbsDiscipline = drWBS.DISCIPLINE,
                            wbsIsDeleted = !drWBS.IsDELETEDNull()
                        }));

                        splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                    }
                }
            }
            finally
            {
                daSchedule.Dispose();
                daTag.Dispose();
                daWBS.Dispose();
                daTemplate.Dispose();
                daPrefill.Dispose();
            }

            //custom header
            splashScreenManager1.CloseWaitForm();
            DataTable masterTable = new DataTable("MasterTable");
            List<HeaderDefinition> defaultHeaders = new List<HeaderDefinition>();
            defaultHeaders.Add(new HeaderDefinition(_columnKey, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnImportStatus, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnDelete, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnType, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnTagWBSName, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnMatrixType1, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnMatrixType2, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnMatrixType3, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnDescription, typeof(string)));

            if (_skipPopulatingPrefillData)
                defaultHeaders.Add(new HeaderDefinition(_columnDiscipline, typeof(string)));

            defaultHeaders.Add(new HeaderDefinition(_columnStage1Assigned, typeof(int)));
            defaultHeaders.Add(new HeaderDefinition(_columnStage1Inspected, typeof(int)));
            defaultHeaders.Add(new HeaderDefinition(_columnStage1Approved, typeof(int)));
            defaultHeaders.Add(new HeaderDefinition(_columnStage1Completed, typeof(int)));
            defaultHeaders.Add(new HeaderDefinition(_columnStage1Closed, typeof(int)));

            if(_isDetailedReport)
            {
                defaultHeaders.Add(new HeaderDefinition(_columnStage2Assigned, typeof(int)));
                defaultHeaders.Add(new HeaderDefinition(_columnStage2Inspected, typeof(int)));
                defaultHeaders.Add(new HeaderDefinition(_columnStage2Approved, typeof(int)));
                defaultHeaders.Add(new HeaderDefinition(_columnStage2Completed, typeof(int)));
                defaultHeaders.Add(new HeaderDefinition(_columnStage2Closed, typeof(int)));

                defaultHeaders.Add(new HeaderDefinition(_columnStage3Assigned, typeof(int)));
                defaultHeaders.Add(new HeaderDefinition(_columnStage3Inspected, typeof(int)));
                defaultHeaders.Add(new HeaderDefinition(_columnStage3Approved, typeof(int)));
                defaultHeaders.Add(new HeaderDefinition(_columnStage3Completed, typeof(int)));
                defaultHeaders.Add(new HeaderDefinition(_columnStage3Closed, typeof(int)));
            }

            defaultHeaders.Add(new HeaderDefinition(_columnAreaNumber, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnAreaDescription, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnSystemNumber, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnSystemDescription, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnSubSystemNumber, typeof(string)));
            defaultHeaders.Add(new HeaderDefinition(_columnSubSystemDescription, typeof(string)));

            foreach (HeaderDefinition defaultHeader in defaultHeaders)
            {
                masterTable.Columns.Add(defaultHeader.ColumnName, defaultHeader.ColumnType);
            }

            if (_discipline.ToUpper() == "ELECTRICAL" && _isBySchedule)
            {
                List<string> electricalHeaderList = Common.GetElectricalHeaderList();
                foreach (string electricalHeader in electricalHeaderList)
                {
                    if (!masterTable.Columns.Contains(electricalHeader))
                        masterTable.Columns.Add(electricalHeader, typeof(string));
                }
            }

            allUniqueHeaders = allUniqueHeaders.OrderBy(x => x).ToList();
            //write unique header to excel
            for (int i = 1; i < allUniqueHeaders.Count + 1; i++)
            {
                string formattedHeaderString = Common.FormatFieldString(allUniqueHeaders[i - 1]);
                if (!defaultHeaders.Any(x => x.ColumnName == formattedHeaderString))
                {
                    if(!masterTable.Columns.Contains(formattedHeaderString))
                        masterTable.Columns.Add(formattedHeaderString, typeof(string));
                }
            }

            CellRange usedRange = _masterSheet.GetUsedRange();

            List<ViewModel_ITRSummary> reportStats = new List<ViewModel_ITRSummary>();

            //only perform pre-population when user clicks master feed sheet instead of importing
            //stats for importing will be populated during saving
            if (_fileName == string.Empty)
                using (AdapterITR_MAIN daITR_Main = new AdapterITR_MAIN())
                {
                    if (_isDetailedReport)
                        reportStats = daITR_Main.GenerateITRStagedSummary(_projectGuid);
                    else
                        reportStats = daITR_Main.GenerateITRSummary(_projectGuid);
                }

            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, wbsTags.Count);
            foreach (wbsTagHeader wbsTag in wbsTags)
            {
                string wbsTagName = string.Empty;
                DataRow newTagRow = masterTable.Rows.Add();
                wbsTagName = wbsTag.wbsTagDisplayName;
                if (wbsTag.wbsTagDisplayAttachTag != null)
                {
                    newTagRow[_columnKey] = wbsTag.wbsTagDisplayAttachTag.GUID;
                    newTagRow[_columnType] = "TAG";
                    newTagRow[_columnMatrixType1] = wbsTag.wbsTagDisplayAttachTag.tagType1;
                    newTagRow[_columnDelete] = wbsTag.wbsTagDisplayAttachTag.tagIsDeleted ? "Y" : "N";
                    newTagRow[_columnDescription] = wbsTag.wbsTagDisplayAttachTag.tagDescription;

                    if (_skipPopulatingPrefillData)
                        newTagRow[_columnDiscipline] = wbsTag.wbsTagDisplayAttachTag.tagDiscipline;

                    if (_fileName == string.Empty)
                    {
                        ViewModel_ITRSummary reportStat = reportStats.FirstOrDefault(x => x.GuidKey == wbsTag.wbsTagDisplayAttachTag.GUID.ToString());
                        if (reportStat != null)
                        {
                            if(_isDetailedReport)
                            {
                                newTagRow[_columnStage1Assigned] = reportStat.Stage1Assigned;
                                newTagRow[_columnStage1Inspected] = reportStat.Stage1Inspected;
                                newTagRow[_columnStage1Approved] = reportStat.Stage1Approved;
                                newTagRow[_columnStage1Completed] = reportStat.Stage1Completed;
                                newTagRow[_columnStage1Closed] = reportStat.Stage1Closed;

                                newTagRow[_columnStage2Assigned] = reportStat.Stage2Assigned;
                                newTagRow[_columnStage2Inspected] = reportStat.Stage2Inspected;
                                newTagRow[_columnStage2Approved] = reportStat.Stage2Approved;
                                newTagRow[_columnStage2Completed] = reportStat.Stage2Completed;
                                newTagRow[_columnStage2Closed] = reportStat.Stage2Closed;

                                newTagRow[_columnStage3Assigned] = reportStat.Stage3Assigned;
                                newTagRow[_columnStage3Inspected] = reportStat.Stage3Inspected;
                                newTagRow[_columnStage3Approved] = reportStat.Stage3Approved;
                                newTagRow[_columnStage3Completed] = reportStat.Stage3Completed;
                                newTagRow[_columnStage3Closed] = reportStat.Stage3Closed;
                            }
                            else
                            {
                                newTagRow[_columnStage1Assigned] = reportStat.Assigned;
                                newTagRow[_columnStage1Inspected] = reportStat.Inspected;
                                newTagRow[_columnStage1Approved] = reportStat.Approved;
                                newTagRow[_columnStage1Completed] = reportStat.Completed;
                                newTagRow[_columnStage1Closed] = reportStat.Closed;

                            }
                        }
                    }
                }
                else
                {
                    newTagRow[_columnKey] = wbsTag.wbsTagDisplayAttachWBS.GUID;
                    newTagRow[_columnType] = "WBS";
                    newTagRow[_columnDelete] = wbsTag.wbsTagDisplayAttachWBS.wbsIsDeleted ? "Y" : "N";
                    newTagRow[_columnDescription] = wbsTag.wbsTagDisplayAttachWBS.wbsDescription;


                    if (_skipPopulatingPrefillData)
                        newTagRow[_columnDiscipline] = wbsTag.wbsTagDisplayAttachWBS.wbsDiscipline;
                    //newTagRow[_columnAssigned] = reportData.Where(x => x.WBS_Name == 
                }

                newTagRow[_columnTagWBSName] = wbsTagName;

                dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtPrefillReg;
                if (wbsTag.wbsTagDisplayAttachTag != null)
                    dtPrefillReg = _daPrefillReg.GetBy(wbsTag.wbsTagDisplayAttachTag.GUID, PrefillType.Tag);
                else
                    dtPrefillReg = _daPrefillReg.GetBy(wbsTag.wbsTagDisplayAttachWBS.GUID, PrefillType.WBS);

                splashScreenManager1.SetWaitFormCaption("Populating " + wbsTag.wbsTagDisplayName);
                if(dtPrefillReg != null)
                {
                    foreach (DataColumn masterColumn in masterTable.Columns)
                    {
                        dsPREFILL_REGISTER.PREFILL_REGISTERRow drPrefillReg = dtPrefillReg.FirstOrDefault(x => x.NAME == masterColumn.ColumnName);
                        if (drPrefillReg != null)
                        {
                            newTagRow[masterColumn.ColumnName] = drPrefillReg.DATA;
                        }
                    }
                }

                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            for (int i = 0; i < 1000; i++)
            {
                DataRow newTagRow = masterTable.Rows.Add();
            }

            // Insert a table in the worksheet.
            options.ImportHeaders = true;
            DevExpress.Spreadsheet.Table masterSpreadsheetTable = _masterSheet.Tables.Add(masterTable, 0, 0, options);
            _masterSheet.DataValidations.Add(masterSpreadsheetTable.Columns[_indexMatrixType1].DataRange, DataValidationType.List, ValueObject.FromRange(metaSpreadsheetTable.Columns[0].DataRange.GetRangeWithAbsoluteReference()));
            _masterSheet.DataValidations.Add(masterSpreadsheetTable.Columns[_indexMatrixType2].DataRange, DataValidationType.List, ValueObject.FromRange(metaSpreadsheetTable.Columns[0].DataRange.GetRangeWithAbsoluteReference()));
            _masterSheet.DataValidations.Add(masterSpreadsheetTable.Columns[_indexMatrixType3].DataRange, DataValidationType.List, ValueObject.FromRange(metaSpreadsheetTable.Columns[0].DataRange.GetRangeWithAbsoluteReference()));

            _masterSheet.DataValidations.Add(masterSpreadsheetTable.Columns[_indexDelete].DataRange, DataValidationType.List, "Y, N");
            _masterSheet.DataValidations.Add(masterSpreadsheetTable.Columns[_indexType].DataRange, DataValidationType.List, "TAG, WBS");
            _masterSheet.Columns[0].Visible = false;
            string metaSheetTableRange = metaSpreadsheetTable.DataRange.GetRangeWithAbsoluteReference().GetReferenceA1();
            string metaSheetHeaderRange = metaSpreadsheetTable.HeaderRowRange.GetRangeWithAbsoluteReference().GetReferenceA1();

            List<string> matrixColumnAddresses = new List<string>();
            string matrix1ColumnAddress = masterSpreadsheetTable.Columns[_indexMatrixType1].DataRange.First().GetRangeWithRelativeReference().GetReferenceA1();
            string matrix2ColumnAddress = masterSpreadsheetTable.Columns[_indexMatrixType2].DataRange.First().GetRangeWithRelativeReference().GetReferenceA1();
            string matrix3ColumnAddress = masterSpreadsheetTable.Columns[_indexMatrixType3].DataRange.First().GetRangeWithRelativeReference().GetReferenceA1();
            matrixColumnAddresses.Add(matrix1ColumnAddress);
            matrixColumnAddresses.Add(matrix2ColumnAddress);
            matrixColumnAddresses.Add(matrix3ColumnAddress);

            string importStatusColumnAddress = masterSpreadsheetTable.Columns[_indexImportStatus].DataRange.First().GetRangeWithRelativeReference().GetReferenceA1();

            FormulaExpressionConditionalFormatting statusFieldSuccessConditionalFormatting = _masterSheet.ConditionalFormattings.AddFormulaExpressionConditionalFormatting(masterSpreadsheetTable.Columns[_indexImportStatus].DataRange, "=ISNUMBER(SEARCH(\"" + _prefixImportSuccess + "\", $" + importStatusColumnAddress + "))");
            statusFieldSuccessConditionalFormatting.Formatting.Fill.BackgroundColor = Color.LightGreen;

            FormulaExpressionConditionalFormatting statusFieldErrorConditionalFormatting = _masterSheet.ConditionalFormattings.AddFormulaExpressionConditionalFormatting(masterSpreadsheetTable.Columns[_indexImportStatus].DataRange, "=ISNUMBER(SEARCH(\"" + _prefixImportError + "\", $" + importStatusColumnAddress + "))");
            statusFieldErrorConditionalFormatting.Formatting.Fill.BackgroundColor = Color.Salmon;

            foreach (string matrixColumnAddress in matrixColumnAddresses)
            {
                FormulaExpressionConditionalFormatting requiredFieldConditionalFormatting1 = _masterSheet.ConditionalFormattings.AddFormulaExpressionConditionalFormatting(masterSpreadsheetTable.DataRange, "=ISNUMBER(VLOOKUP($" + matrixColumnAddress + ", Sheet2!" + metaSheetTableRange + ", MATCH(A$1, Sheet2!" + metaSheetHeaderRange + ", 0), FALSE))");
                requiredFieldConditionalFormatting1.Formatting.Fill.BackgroundColor = Color.Yellow;
            }

            string deleteColumnAddress = masterSpreadsheetTable.Columns[_indexDelete].DataRange.First().GetRangeWithRelativeReference().GetReferenceA1();
            string nameColumnAddress = masterSpreadsheetTable.Columns[_indexTagWBSName].DataRange.First().GetRangeWithRelativeReference().GetReferenceA1();
            FormulaExpressionConditionalFormatting duplicateValueConditionalFormatting = _masterSheet.ConditionalFormattings.AddFormulaExpressionConditionalFormatting(masterSpreadsheetTable.Columns[_indexTagWBSName].DataRange, "=AND($" + deleteColumnAddress + " = \"N\", COUNTIFS($" + nameColumnAddress + ":$" + nameColumnAddress + ", $" + nameColumnAddress + "2, $" + deleteColumnAddress + ":$" + deleteColumnAddress + ", $" + deleteColumnAddress + ") > 1)");
            duplicateValueConditionalFormatting.Formatting.Fill.BackgroundColor = Color.Salmon;

            spreadsheetControl1.EndUpdate();
            splashScreenManager1.CloseWaitForm();
        }
    }

    public class ImportWBS
    {
        public String AreaNumber { get; set; }
        public Guid? AreaGuid { get; set; }
        public string SystemNumber { get; set; }
        public Guid? SystemGuid { get; set; }
        public string SubSystemNumber { get; set; }
        public Guid? SubSystemGuid { get; set; }
    }

    public class HeaderDefinition
    {
        public HeaderDefinition(string columnName, Type columnType)
        {
            ColumnName = columnName;
            ColumnType = columnType;
        }

        public string ColumnName { get; set; }
        public Type ColumnType { get; set; }
    }
}
