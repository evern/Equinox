using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.Spreadsheet;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

namespace CheckmateDX
{
    public partial class frmSchedule_Prefill : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //local parameters
        //max rows to apply conditional formatting
        static int _maxRows = 1000;

        IWorkbook _workbook;
        Worksheet _worksheet;
        Schedule _schedule;
        Guid _projectGuid;

        AdapterPREFILL_REGISTER _daPrefillReg = new AdapterPREFILL_REGISTER();
        dsPREFILL_REGISTER _dsPrefillReg = new dsPREFILL_REGISTER();
        List<SpreadsheetCommandDelegateContainer> _replaceCommandMethod = new List<SpreadsheetCommandDelegateContainer>();
        DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
                    
        public frmSchedule_Prefill()
        {
            InitializeComponent();
        }

        public frmSchedule_Prefill(Schedule schedule, Guid projectGuid)
        {
            splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("Initializing Spreadsheet ... ");
            InitializeComponent();
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, 100);
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 30);
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);

            _schedule = schedule;
            _projectGuid = projectGuid;
            SetReplaceCommandMethod();
            //remove the ability to add or remove worksheets
            spreadsheetControl1.Options.Behavior.Worksheet.Insert = DevExpress.XtraSpreadsheet.DocumentCapability.Hidden;
            spreadsheetControl1.Options.Behavior.Worksheet.Delete = DevExpress.XtraSpreadsheet.DocumentCapability.Hidden;
            _workbook = spreadsheetControl1.Document;
            _worksheet = _workbook.Worksheets[0];

            splashScreenManager1.CloseWaitForm();
            ConstructHeader(schedule.GUID);

            FormatHeader();
            FormatTagRows();
        }

        #region events
        private void bBtnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ImportSpreadsheet();
            Common.Prompt("Spreadsheet successfully imported");
        }
        #endregion

        #region Construct Spreadsheet
        /// <summary>
        /// Writes unique header to spreadsheet, also highlights required field for each wbs/tag
        /// </summary>
        private void ConstructHeader(Guid scheduleGuid)
        {
            AdapterSCHEDULE daSchedule = new AdapterSCHEDULE();
            AdapterTAG daTag = new AdapterTAG();
            AdapterWBS daWBS = new AdapterWBS();
            AdapterTEMPLATE_MAIN daTemplate = new AdapterTEMPLATE_MAIN();
            AdapterTEMPLATE_REGISTER daTemplateRegister = new AdapterTEMPLATE_REGISTER();
            AdapterPREFILL_MAIN daPrefill = new AdapterPREFILL_MAIN();

            List<wbsTagHeader> wbsTags = new List<ProjectLibrary.wbsTagHeader>();
            List<string> uniqueHeader = new List<string>();
            List<string> punchlistPrefills = new List<string>();
            dsSCHEDULE.SCHEDULERow drSchedule = daSchedule.GetBy(scheduleGuid);

            if (drSchedule == null)
                return;
            try
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("Loading Tag Numbers ... ");


                //retrieve tag
                dsTAG.TAGDataTable dtTag = daTag.GetBySchedule(scheduleGuid);

                if (dtTag != null)
                {
                    foreach (dsTAG.TAGRow drTag in dtTag.Rows)
                    {
                        wbsTags.Add(new wbsTagHeader(new Tag(drTag.GUID)
                        {
                            tagNumber = drTag.NUMBER,
                            tagDescription = drTag.DESCRIPTION,
                            tagParentGuid = drTag.PARENTGUID,
                            tagScheduleGuid = drTag.SCHEDULEGUID
                        }));
                    }
                }

                //retrieve wbs
                //dsWBS.WBSDataTable dtWBS = daWBS.GetByProject(scheduleGuid);
                dsWBS.WBSDataTable dtWBS = daWBS.GetByProject(_projectGuid);
                if (dtWBS != null)
                {
                    foreach (dsWBS.WBSRow drWBS in dtWBS.Rows)
                    {
                        wbsTags.Add(new wbsTagHeader(new WBS(drWBS.GUID)
                        {
                            wbsName = drWBS.NAME,
                            wbsDescription = drWBS.IsDESCRIPTIONNull() ? "" : drWBS.DESCRIPTION,
                            wbsParentGuid = drWBS.PARENTGUID,
                            wbsScheduleGuid = drWBS.SCHEDULEGUID
                        }));
                    }
                }

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

                int ProgressMax = wbsTags.Count * 2;
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, Convert.ToInt32(ProgressMax * 1.3));
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, Convert.ToInt32(ProgressMax * 0.3));
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                splashScreenManager1.SetWaitFormCaption("Reading Header Fields ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);

                RichEditControl richEditControl1 = new RichEditControl();
                //retrieve global punchlist headers
                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drPunchlistTemplate = daTemplate.GetBy(Variables.punchlistTemplateName);
                if(drPunchlistTemplate != null)
                {
                    byte[] receivedBytes = drPunchlistTemplate.TEMPLATE;
                    MemoryStream ms = new MemoryStream(receivedBytes);
                    richEditControl1.Document.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    Common.RetrievePrefillFields(richEditControl1, null, ExcludedPrefills, punchlistPrefills);
                }

                //for storing scanned items
                foreach (wbsTagHeader wbsTag in wbsTags)
                {
                    dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTemplateRegister = daTemplateRegister.GetByWBSTag(wbsTag);
                    if(dtTemplateRegister != null)
                    {
                        foreach(dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTemplateRegister in dtTemplateRegister.Rows)
                        {
                            dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = daTemplate.GetBy(drTemplateRegister.TEMPLATE_GUID);
                            if (drTemplate != null && !drTemplate.IsTEMPLATENull())
                            {
                                //load document
                                byte[] receivedBytes = drTemplate.TEMPLATE;
                                MemoryStream ms = new MemoryStream(receivedBytes);
                                richEditControl1.Document.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                                Common.RetrievePrefillFields(richEditControl1, wbsTag, ExcludedPrefills, uniqueHeader);
                            }

                            //add global punchlist to wbsTag headers
                            foreach(string punchlistPrefill in punchlistPrefills)
                            {
                                if (wbsTag.wbsTagHeaderItems.IndexOf(punchlistPrefill) == -1 && ExcludedPrefills.IndexOf(punchlistPrefill) == -1)
                                {
                                    wbsTag.wbsTagHeaderItems.Add(punchlistPrefill);
                                }
                            }
                        }
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }
            }
            finally
            {
                daSchedule.Dispose();
                daTag.Dispose();
                daWBS.Dispose();
                daTemplate.Dispose();
                daTemplateRegister.Dispose();
                daPrefill.Dispose();
            }

            //custom header
            _worksheet.Cells[0, 0].Value = Variables.Header_TagWBS;

            int columnCount = 1;
            //write unique header to excel
            for (int i = 1; i < uniqueHeader.Count + 1; i++)
            {
                //Get the field name
                _worksheet.Cells[0, columnCount].Value = Common.FormatFieldString(uniqueHeader[i - 1]);
                columnCount++;
            }

            //write global punchlist header to excel
            for (int i = 1; i < punchlistPrefills.Count + 1; i++)
            {
                //only write if its not already written by unique header
                if(uniqueHeader.IndexOf(punchlistPrefills[i - 1]) == -1)
                {
                    _worksheet.Cells[0, columnCount].Value = Common.FormatFieldString(punchlistPrefills[i - 1]);
                    columnCount++;
                }
            }

            //retrive the used range to traverse columns
            CellRange usedRange = _worksheet.GetUsedRange();
            int rowIndex = 1;

            splashScreenManager1.SetWaitFormCaption("Writing Header Fields ... ");

            foreach(wbsTagHeader wbsTag in wbsTags)
            {
                string wbsTagName = string.Empty;
                //insert the wbs/tag number
                _worksheet.Cells[rowIndex, 0].Value = wbsTag.wbsTagDisplayName;

                //retrieve saved value
                for (int i = 1; i < usedRange.ColumnCount; i++)
                {
                    dsPREFILL_REGISTER.PREFILL_REGISTERRow drPrefillReg;
                    string headerString = _worksheet.Cells[0, i].DisplayText;

                    if(wbsTag.wbsTagDisplayAttachTag != null)
                        drPrefillReg = _daPrefillReg.GetBy(wbsTag.wbsTagDisplayAttachTag.GUID, headerString, PrefillType.Tag);
                    else
                        drPrefillReg = _daPrefillReg.GetBy(wbsTag.wbsTagDisplayAttachWBS.GUID, headerString, PrefillType.WBS);

                    if(drPrefillReg != null)
                    {
                        _worksheet.Cells[rowIndex, i].Value = drPrefillReg.DATA;
                    }
                }

                //highlighting
                foreach(string wbsHeaderItem in wbsTag.wbsTagHeaderItems)
                {
                    for (int i = 1; i < usedRange.ColumnCount; i++)
                    {
                        string formatString = Common.FormatFieldString(wbsHeaderItem);
                        //format cell if value is required
                        if (formatString == _worksheet.Cells[0, i].DisplayText)
                        {
                            Cell editCell = _worksheet.Cells[rowIndex, i];
                            editCell.Borders.SetOutsideBorders(Color.LightBlue, BorderLineStyle.Thin);
                            editCell.Fill.BackgroundColor = Color.LemonChiffon;
                        }
                    }
                }
                rowIndex += 1;

                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            splashScreenManager1.CloseWaitForm();
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
            _worksheet.Columns.AutoFit(0, usedRange.ColumnCount);
        }
        #endregion

        #region Import Spreadsheet
        /// <summary>
        /// Saves the wbs/tag data
        /// </summary>
        private void ImportSpreadsheet()
        {
            AdapterTAG daTag = new AdapterTAG();
            AdapterWBS daWBS = new AdapterWBS();

            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormDescription("Saving ...");

            dsPREFILL_REGISTER dsPrefillReg = new dsPREFILL_REGISTER();
            CellRange usedRange = _worksheet.GetUsedRange();

            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, usedRange.RowCount);
            try
            {
                //check whether tag or wbs
                for (int i = 1; i < usedRange.RowCount; i++)
                {
                    string wbsTagNumber = _worksheet.Cells[i, 0].DisplayText;

                    dsTAG.TAGRow drTag = daTag.GetBy(wbsTagNumber, _projectGuid);
                    dsWBS.WBSRow drWBS = daWBS.GetBy(wbsTagNumber, _schedule.GUID);

                    if (drTag != null)
                    {
                        AddPrefill(drTag.GUID, PrefillType.Tag, usedRange, i);
                        drTag.UPDATED = DateTime.Now;
                        drTag.UPDATEDBY = System_Environment.GetUser().GUID;
                        daTag.Save(drTag);
                    }
                    else if (drWBS != null)
                    {
                        AddPrefill(drWBS.GUID, PrefillType.WBS, usedRange, i);
                        drWBS.UPDATED = DateTime.Now;
                        drWBS.UPDATEDBY = System_Environment.GetUser().GUID;
                        daWBS.Save(drWBS);
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }
            }
            finally
            {
                daTag.Dispose();
                daWBS.Dispose();
            }

            splashScreenManager1.CloseWaitForm();
        }

        /// <summary>
        /// Writes the wbs/tag data into db
        /// </summary>
        /// <param name="prefillGuid">wbs/tag Guid</param>
        /// <param name="prefillType">wbs or tag</param>
        /// <param name="usedRange">spreadsheet used range</param>
        /// <param name="rowIndex">current tag row index</param>
        private void AddPrefill(Guid prefillGuid, PrefillType prefillType, CellRange usedRange, int rowIndex)
        {
            for(int columnIndex = 1; columnIndex < usedRange.ColumnCount; columnIndex++)
            {
                string headerString = _worksheet.Cells[0, columnIndex].DisplayText.Trim();
                dsPREFILL_REGISTER.PREFILL_REGISTERRow drPrefillReg = _daPrefillReg.GetBy(prefillGuid, headerString, prefillType);

                if(_worksheet.Cells[rowIndex, columnIndex].DisplayText != string.Empty)
                {
                    bool addRow = false;

                    if(drPrefillReg == null)
                    {
                        drPrefillReg = _dsPrefillReg.PREFILL_REGISTER.NewPREFILL_REGISTERRow();
                        drPrefillReg.GUID = Guid.NewGuid();

                        addRow = true;
                    }
                    
                    if(prefillType == PrefillType.Tag)
                        drPrefillReg.TAG_GUID = prefillGuid;
                    else
                        drPrefillReg.WBS_GUID = prefillGuid;

                    drPrefillReg.NAME = headerString;
                    drPrefillReg.DATA = _worksheet.Cells[rowIndex, columnIndex].DisplayText.Trim();

                    if(addRow)
                    {
                        drPrefillReg.CREATED = DateTime.Now;
                        drPrefillReg.CREATEDBY = System_Environment.GetUser().GUID;
                        _dsPrefillReg.PREFILL_REGISTER.AddPREFILL_REGISTERRow(drPrefillReg);
                    }
                    else
                    {
                        drPrefillReg.UPDATED = DateTime.Now;
                        drPrefillReg.UPDATEDBY = System_Environment.GetUser().GUID;
                    }

                    _daPrefillReg.Save(drPrefillReg);
                }
                //if string is empty we would want to delete the entry if it exists in db
                else
                {
                    if(drPrefillReg != null)
                    {
                        _daPrefillReg.RemoveBy(drPrefillReg.GUID);
                    }
                }
            }
        }
        #endregion

        #region Helper
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
        /// Assign local method to commands on spreadsheet
        /// </summary>
        private void SetReplaceCommandMethod()
        {
            _replaceCommandMethod.Add(new SpreadsheetCommandDelegateContainer(SpreadsheetCommandId.FileSave, new SpreadsheetCommandDelegate(ImportSpreadsheet)));
            ISpreadsheetCommandFactoryService service = spreadsheetControl1.GetService(typeof(ISpreadsheetCommandFactoryService)) as ISpreadsheetCommandFactoryService;
            spreadsheetControl1.ReplaceService<ISpreadsheetCommandFactoryService>(new CustomSpreadsheetCommandFactoryService(service, spreadsheetControl1, _replaceCommandMethod));
        }
        #endregion

        /// <summary>
        /// Dispose local adapters upon closing
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            _daPrefillReg.Dispose();
            base.OnClosed(e);
        }
    }
}
