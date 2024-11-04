using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.Spreadsheet;

namespace CheckmateDX
{
    public partial class frmSchedule_BulkTag : CheckmateDX.frmParent
    {
        //max rows to apply conditional formatting
        static int _maxRows = 1000;
        //header text
        static string _headerTag = "Tag";
        static string _headerDescription = "Description";
        static string _headerType1 = "Type 1";
        static string _headerType2 = "Type 2";
        static string _headerType3 = "Type 3";
        IWorkbook _workbook;
        Worksheet _worksheet;
        Guid _scheduleGuid;
        Guid _projectGuid;

        AdapterTAG _daTag = new AdapterTAG();
        List<SpreadsheetCommandDelegateContainer> _replaceCommandMethod = new List<SpreadsheetCommandDelegateContainer>();
        List<string> ValidTypeString = new List<string>();

        public frmSchedule_BulkTag(Guid scheduleGuid, Guid projectGuid)
        {
            InitializeComponent();
            CacheAssignmentType();
            _projectGuid = projectGuid;
            _scheduleGuid = scheduleGuid;
            //remove the ability to add or remove worksheets
            spreadsheetControl1.Options.Behavior.Worksheet.Insert = DevExpress.XtraSpreadsheet.DocumentCapability.Hidden;
            spreadsheetControl1.Options.Behavior.Worksheet.Delete = DevExpress.XtraSpreadsheet.DocumentCapability.Hidden;
            _workbook = spreadsheetControl1.Document;
            _worksheet = _workbook.Worksheets[0];
            SetReplaceCommandMethod();
            ConstructHeader();
            FormatHeader();
            PopulateSpreadsheet();
            FormatTagRows();
        }

        #region Helper
        /// <summary>
        /// Cache all available matrix type to determine type validity later
        /// </summary>
        private void CacheAssignmentType()
        {
            using(AdapterMATRIX_TYPE daMATRIX_TYPE = new AdapterMATRIX_TYPE())
            {
                dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMATRIX_TYPE = daMATRIX_TYPE.Get();
                if(dtMATRIX_TYPE != null)
                {
                    foreach(dsMATRIX_TYPE.MATRIX_TYPERow drMATRIX_TYPE in dtMATRIX_TYPE.Rows)
                    {
                        ValidTypeString.Add(drMATRIX_TYPE.NAME);
                    }
                }
            }
        }

        /// <summary>
        /// Build the static header
        /// </summary>
        private void ConstructHeader()
        {
            _worksheet.Cells[0, 0].Value = _headerTag;
            _worksheet.Cells[0, 0].ColumnWidth = 500;
            _worksheet.Cells[0, 1].Value = _headerDescription;
            _worksheet.Cells[0, 1].ColumnWidth = 500;
            _worksheet.Cells[0, 2].Value = _headerType1;
            _worksheet.Cells[0, 2].ColumnWidth = 500;
            _worksheet.Cells[0, 3].Value = _headerType2;
            _worksheet.Cells[0, 3].ColumnWidth = 500;
            _worksheet.Cells[0, 4].Value = _headerType3;
            _worksheet.Cells[0, 4].ColumnWidth = 500;
        }

        /// <summary>
        /// Adds conditional formatting to tag rows to indicate duplicate values
        /// </summary>
        private void FormatTagRows()
        {
            string rangeString = _worksheet.Cells[1, 0].GetReferenceA1() + ":" + _worksheet.Cells[_maxRows, 0].GetReferenceA1();
            SpecialConditionalFormatting cfRule = _worksheet.ConditionalFormattings.AddSpecialConditionalFormatting(_worksheet.Range[rangeString], ConditionalFormattingSpecialCondition.ContainDuplicateValue);
            cfRule.Formatting.Fill.BackgroundColor = Color.LightPink;

            CellRange usedRange = _worksheet.GetUsedRange();
        }

        /// <summary>
        /// Formats the header
        /// </summary>
        private void FormatHeader()
        {
            CellRange usedRange = _worksheet.GetUsedRange();
            for (int i = 0; i < usedRange.ColumnCount; i++)
            {
                Cell editCell = _worksheet.Cells[0, i];
                editCell.FillColor = Color.DarkGray;
                editCell.Font.Color = Color.White;
            }
        }

        /// <summary>
        /// Populate spreadsheet with existing data
        /// </summary>
        private void PopulateSpreadsheet()
        {
            dsTAG.TAGDataTable dtTag = _daTag.GetBySchedule(_scheduleGuid);
            if(dtTag != null)
            {
                int rowIndex = 1;
                foreach(dsTAG.TAGRow drTag in dtTag.Rows)
                {
                    _worksheet.Cells[rowIndex, 0].Value = drTag.NUMBER;
                    _worksheet.Cells[rowIndex, 1].Value = drTag.DESCRIPTION;
                    _worksheet.Cells[rowIndex, 2].Value = drTag.IsTYPE1Null() ? string.Empty : drTag.TYPE1;
                    _worksheet.Cells[rowIndex, 3].Value = drTag.IsTYPE2Null() ? string.Empty : drTag.TYPE2;
                    _worksheet.Cells[rowIndex, 4].Value = drTag.IsTYPE3Null() ? string.Empty : drTag.TYPE3;
                    rowIndex += 1;
                }
            }

            CellRange usedRange = _worksheet.GetUsedRange();
            //_worksheet.Columns.AutoFit(0, usedRange.ColumnCount);
        }

        /// <summary>
        /// Saves the edited spreadsheet into db
        /// </summary>
        private void ImportSpreadsheet()
        {
            dsTAG dsTag = new dsTAG();
            _worksheet.Cells.EndUpdate();
            CellRange usedRange = _worksheet.GetUsedRange();
            List<ImportStatus> importStatuses = new List<ImportStatus>();

            for(int rowIndex = 1;rowIndex < usedRange.RowCount;rowIndex++)
            {
                string tagText = _worksheet.Cells[rowIndex, 0].DisplayText.Trim();
                string tagDesc = _worksheet.Cells[rowIndex, 1].DisplayText.Trim();
                string tagType1 = _worksheet.Cells[rowIndex, 2].DisplayText.Trim();
                string tagType2 = _worksheet.Cells[rowIndex, 3].DisplayText.Trim();
                string tagType3 = _worksheet.Cells[rowIndex, 4].DisplayText.Trim();
                string PrimaryStatus = string.Empty;
                string SecondaryStatus = string.Empty;

                if(tagText != string.Empty)
                {
                    dsTAG.TAGRow drTag = _daTag.GetBy(tagText, _projectGuid);
                    //only start adding if it doesn't exists
                    if(drTag == null)
                    {
                        PrimaryStatus = "Imported";
                        
                        dsTAG.TAGRow drNewTag = dsTag.TAG.NewTAGRow();
                        drNewTag.GUID = Guid.NewGuid();
                        drNewTag.SCHEDULEGUID = _scheduleGuid;
                        drNewTag.PARENTGUID = Guid.Empty;
                        drNewTag.WBSGUID = Guid.Empty;
                        drNewTag.NUMBER = tagText;
                        drNewTag.DESCRIPTION = tagDesc;
                        if(ValidTypeString.Any(obj => obj.ToUpper() == tagType1.ToUpper()))
                            drNewTag.TYPE1 = tagType1;
                        else
                            SecondaryStatus = ": Invalid Type 1";

                        if (ValidTypeString.Any(obj => obj.ToUpper() == tagType2.ToUpper()))
                            drNewTag.TYPE1 = tagType2;
                        else
                            SecondaryStatus = ": Invalid Type 2";

                        if (ValidTypeString.Any(obj => obj.ToUpper() == tagType3.ToUpper()))
                            drNewTag.TYPE1 = tagType3;
                        else
                            SecondaryStatus = ": Invalid Type 3";

                        drNewTag.CREATED = DateTime.Now;
                        drNewTag.CREATEDBY = System_Environment.GetUser().GUID;
                        dsTag.TAG.AddTAGRow(drNewTag);
                        _daTag.Save(drNewTag);

                        importStatuses.Add(new ImportStatus(drNewTag.NUMBER)
                        {
                            tagStatus = PrimaryStatus + " " + SecondaryStatus
                        });
                    }
                    else
                    {
                        if (drTag.SCHEDULEGUID == _scheduleGuid)
                        {
                            PrimaryStatus = "Edited";
                            drTag.NUMBER = tagText;
                            drTag.DESCRIPTION = tagDesc;
                            if (ValidTypeString.Any(obj => obj.ToUpper() == tagType1.ToUpper()))
                                drTag.TYPE1 = tagType1;
                            else
                                SecondaryStatus = ": Invalid Type 1";

                            if (ValidTypeString.Any(obj => obj.ToUpper() == tagType2.ToUpper()))
                                drTag.TYPE1 = tagType2;
                            else
                                SecondaryStatus = ": Invalid Type 2";

                            if (ValidTypeString.Any(obj => obj.ToUpper() == tagType3.ToUpper()))
                                drTag.TYPE1 = tagType3;
                            else
                                SecondaryStatus = ": Invalid Type 3";

                            drTag.UPDATED = DateTime.Now;
                            drTag.UPDATEDBY = System_Environment.GetUser().GUID;
                            _daTag.Save(drTag);
                        }
                        else
                            PrimaryStatus = "Skipped: Exists in other schedule";

                        importStatuses.Add(new ImportStatus(drTag.NUMBER)
                        {
                            tagStatus = PrimaryStatus + " " + SecondaryStatus
                        });
                    }
                }
            }

            splashScreenManager1.ShowWaitForm();
            rptDeletion f = new rptDeletion(importStatuses);
            splashScreenManager1.CloseWaitForm();
            f.ShowReport();
        }

        /// <summary>
        /// Assign local method to commands on spreadsheet
        /// </summary>
        private void SetReplaceCommandMethod()
        {
            _replaceCommandMethod.Add(new SpreadsheetCommandDelegateContainer(SpreadsheetCommandId.FileSave, new SpreadsheetCommandDelegate(ImportSpreadsheet)));
            ISpreadsheetCommandFactoryService service = spreadsheetControl1.GetService(typeof(ISpreadsheetCommandFactoryService)) as ISpreadsheetCommandFactoryService;
            spreadsheetControl1.ReplaceService<ISpreadsheetCommandFactoryService>(new CustomSpreadsheetCommandFactoryService(service, spreadsheetControl1, _replaceCommandMethod));
        }
        #endregion

        #region Event
        private void btnOk_Click(object sender, EventArgs e)
        {
            ImportSpreadsheet();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion

        /// <summary>
        /// Dispose local adapters upon closing
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            _daTag.Dispose();
            base.OnClosed(e);
        }
    }
}
