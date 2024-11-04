using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.Office;
using DevExpress.XtraRichEdit.API.Layout;

namespace CheckmateDX
{
    public partial class frmTemplate_Main : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public delegate void UnhideManageForm();
        public UnhideManageForm ShowManageForm;
        Template _template;
        AdapterTEMPLATE_MAIN _daTemplate = new AdapterTEMPLATE_MAIN();
        AdapterPREFILL_MAIN _daPrefill = new AdapterPREFILL_MAIN();
        List<RichEditCommandDelegateContainer> _replaceCommandMethod = new List<RichEditCommandDelegateContainer>();
        List<Feature> _allFeature = new List<Feature>();
        List<string> _allEditRange = new List<string>();

        List<Interactables> _allInteractables = new List<Interactables>();
        List<Template_Toggle> _allTemplateToggle = new List<Template_Toggle>();

        public frmTemplate_Main()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize Template for Design Purpose
        /// </summary>
        public frmTemplate_Main(Template template)
        {
            InitializeComponent();
            _template = template;
            PopulatePrefill(_template.templateDiscipline);
            SetReplaceCommandMethod();
            customRichEdit1.LoadTemplateFromDB(template.GUID);
            Refresh_Interactables_ComboBox();
            BuildInteractablesComboBox();
            ReserveShortcutKeys();

            //Replaces the default permission listing
            customRichEdit1.Options.MailMerge.ViewMergedData = false;
            PunchlistToolsRibbonPageCategory1.Visible = false;
        }

        /// <summary>
        /// For initialising punchlist
        /// </summary>
        public frmTemplate_Main(string templateName, bool isCertificate)
        {
            InitializeComponent();
            dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = _daTemplate.GetBy(templateName);

            Template punchlistTemplate = new Template(Guid.NewGuid())
            {
                templateName = templateName,
                templateDescription = "Standard Punchlist",
                templateDiscipline = Variables.prefillGeneral.ToString(),
                templateRevision = "0",
                templateWorkFlow = new ValuePair("No Workflow", Guid.Empty)
            };

            //Replaces the default permission listing
            customRichEdit1.Options.MailMerge.ViewMergedData = false;

            if (drTemplate == null)
            {
                dsTEMPLATE_MAIN dsTemplate = new dsTEMPLATE_MAIN();
                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drNewTemplate = dsTemplate.TEMPLATE_MAIN.NewTEMPLATE_MAINRow();
                drNewTemplate.GUID = punchlistTemplate.GUID;
                drNewTemplate.WORKFLOWGUID = (Guid)punchlistTemplate.templateWorkFlow.Value;
                drNewTemplate.NAME = punchlistTemplate.templateName;
                drNewTemplate.REVISION = punchlistTemplate.templateRevision;
                drNewTemplate.DISCIPLINE = punchlistTemplate.templateDiscipline;
                drNewTemplate.CREATED = DateTime.Now;
                drNewTemplate.CREATEDBY = System_Environment.GetUser().GUID;
                drNewTemplate.QRSUPPORT = false;
                drNewTemplate.SKIPAPPROVED = false;
                dsTemplate.TEMPLATE_MAIN.AddTEMPLATE_MAINRow(drNewTemplate);
                _daTemplate.Save(drNewTemplate);
            }
            else
                punchlistTemplate.GUID = drTemplate.GUID;

            _template = punchlistTemplate;
            SetReplaceCommandMethod();
            Refresh_Interactables_ComboBox();
            BuildInteractablesComboBox();
            ReserveShortcutKeys();
            customRichEdit1.LoadTemplateFromDB(punchlistTemplate.GUID);
            PopulatePrefill(string.Empty); //pass in empty string because we only require general

            if(isCertificate)
            {
                ribbonPageCertificateCategory.Visible = true;
                PunchlistToolsRibbonPageCategory1.Visible = false;
            }
            else
            {
                ribbonPageCertificateCategory.Visible = false;
                ITRToolsRibbonPageCategory1.Visible = false;
            }
        }

        #region Event
        private void Refresh_MergeField()
        {
            PopulatePrefill(_template.templateDiscipline);
        }

        private void barToggleTouchUI_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            customRichEdit1.Set_TouchUI(barToggleTouchUI.Checked);
            splashScreenManager1.CloseWaitForm();
        }

        public CustomRichEdit GetRichEdit()
        {
            return customRichEdit1;
        }

        private void bBtnConditionAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertPermissionForTable_WithInteractableRefresh(CustomUserGroupListService.ALL, CustomUserGroupListService.ALL);
        }

        private void bBtnAddToggle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(barEditSelectToggles.EditValue == null)
            {
                Common.Warn("Please select a toggle to add");
                return;
            }

            
            string toggleName = (string)barEditSelectToggles.EditValue;
            InsertInteractables(toggleName, toggleName);
        }

        private void barButtonRemovePunchlistTable_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            RangePermission findRangePermission = rangePermissions.FirstOrDefault(obj => obj.Group == CustomUserGroupListService.PUNCHLIST_BLOCK);

            if (findRangePermission != null)
                rangePermissions.Remove(findRangePermission);

            customRichEdit1.Document.EndUpdateRangePermissions(rangePermissions);
        }

        private void barButtonAssignPunchlistTable_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!isPunchlistBlockExist())
            {
                TableCell tCell = customRichEdit1.Document.Tables.GetTableCell(customRichEdit1.Document.Selection.Start);
                if (tCell == null)
                    Common.Prompt("Please select a table");
                else
                {
                    Table tbl = tCell.Table;
                    if (tbl.FirstRow.Cells.Count < 9)
                        Common.Prompt("Your table must have at least 9 columns");
                    else
                        InsertPermissionForTable_WithInteractableRefresh(CustomUserGroupListService.PUNCHLIST_BLOCK, string.Empty);
                }
            }
            else
                Common.Warn("You can have only one punchlist table");
        }

        private void barButtonInsertPunchlistTable_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (isPunchlistBlockExist())
            {
                Common.Warn("You can have only one punchlist table");
                return;
            }

            customRichEdit1.Document.Paragraphs.Insert(customRichEdit1.Document.Selection.Start);

            Table tblPunchlist = customRichEdit1.Document.Tables.Create(customRichEdit1.Document.Selection.Start, 1, 9, AutoFitBehaviorType.AutoFitToWindow);
            tblPunchlist.FirstRow.HeightType = HeightType.Exact;
            tblPunchlist.FirstRow.Height = 50;
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(0, 0).Range.Start, "ITEM");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(0, 1).Range.Start, "DESCRIPTION OF DEFECTS");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(0, 2).Range.Start, "ACTION BY");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(0, 3).Range.Start, "CATEGORY");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(0, 4).Range.Start, "PRIORITY");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(0, 5).Range.Start, "DATE");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(0, 6).Range.Start, "STATUS");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(0, 7).Range.Start, "SIGN OFF");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(0, 8).Range.Start, "COMMENTS");

            tblPunchlist.FirstRow.Cells[0].PreferredWidthType = WidthType.Fixed;
            tblPunchlist.FirstRow.Cells[1].PreferredWidthType = WidthType.Fixed;
            tblPunchlist.FirstRow.Cells[2].PreferredWidthType = WidthType.Fixed;
            tblPunchlist.FirstRow.Cells[3].PreferredWidthType = WidthType.Fixed;
            tblPunchlist.FirstRow.Cells[4].PreferredWidthType = WidthType.Fixed;
            tblPunchlist.FirstRow.Cells[5].PreferredWidthType = WidthType.Fixed;
            tblPunchlist.FirstRow.Cells[6].PreferredWidthType = WidthType.Fixed;
            tblPunchlist.FirstRow.Cells[7].PreferredWidthType = WidthType.Fixed;
            tblPunchlist.FirstRow.Cells[8].PreferredWidthType = WidthType.Auto;
            tblPunchlist.FirstRow.Cells[0].PreferredWidth = 300;
            tblPunchlist.FirstRow.Cells[1].PreferredWidth = 800;
            tblPunchlist.FirstRow.Cells[2].PreferredWidth = 300;
            tblPunchlist.FirstRow.Cells[3].PreferredWidth = 200;
            tblPunchlist.FirstRow.Cells[4].PreferredWidth = 200;
            tblPunchlist.FirstRow.Cells[5].PreferredWidth = 300;
            tblPunchlist.FirstRow.Cells[6].PreferredWidth = 300;
            tblPunchlist.FirstRow.Cells[7].PreferredWidth = 400;

            //format header columns
            for (int i = 0; i < 9; i++)
            {
                CharacterProperties cp = customRichEdit1.Document.BeginUpdateCharacters(tblPunchlist.Cell(0, i).ContentRange);
                ParagraphProperties pp = customRichEdit1.Document.BeginUpdateParagraphs(tblPunchlist.Cell(0, i).Range);
                cp.Bold = true;
                cp.FontSize = 8;
                pp.Alignment = ParagraphAlignment.Center;
                customRichEdit1.Document.EndUpdateCharacters(cp);
                customRichEdit1.Document.EndUpdateParagraphs(pp);
            }

            customRichEdit1.Document.Selection = tblPunchlist.Cell(0, 0).Range;
            InsertInteractables("", CustomUserGroupListService.PUNCHLIST_BLOCK);
        }

        private void barButtonInsertPic_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertInteractables("Edit Picture", CustomUserGroupListService.USER_PICTURE, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonPicAttach_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertInteractables("Attach Picture", CustomUserGroupListService.ATTACH_PICTURE, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonRowAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertInteractables("Click Here to Insert Row Above", CustomUserGroupListService.INSERT_LINE, false, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonRowRemove_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertInteractables("Click Here to Remove Row Above", CustomUserGroupListService.REMOVE_LINE, false, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonInitials_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertInteractables(string.Empty, CustomUserGroupListService.INITIALS, true, barEditFontName.EditValue, barEditFontSize.EditValue, true);
        }

        private void bBtnTagSelect_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertPermissionForTable_WithInteractableRefresh(CustomUserGroupListService.SELECT_TAG, string.Empty, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonRemoveColor_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            customRichEdit1.Color_Interactables(false, false, HighlightType.Both);
            splashScreenManager1.CloseWaitForm();
        }

        private void barButtonColor_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            customRichEdit1.Color_Interactables(true, false, HighlightType.Both);
            splashScreenManager1.CloseWaitForm();
        }

        private void barButtonRemovePermission_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RemoveSelectedCellPermissions();
        }

        private void barBtnRefreshInteractables_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RefreshInteractablesRange();
        }

        private void barBtnRefreshEditables_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Refresh_Interactables_ComboBox();
        }

        private void bBtnApplyBorders_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TableCell selectedCell = customRichEdit1.Document.Tables.GetTableCell(customRichEdit1.Document.Selection.Start);
            if (selectedCell != null)
            {
                Table selectedTable = selectedCell.Table;
                selectedTable.ForEachCell(new TableCellProcessorDelegate(customRichEdit1.ApplyBorders));
            }
        }

        private void bBtnStripTable_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TableCell selectedCell = customRichEdit1.Document.Tables.GetTableCell(customRichEdit1.Document.Selection.Start);
            if(selectedCell != null)
            {
                Table selectedTable = selectedCell.Table;
                selectedTable.ForEachCell(new TableCellProcessorDelegate(customRichEdit1.ClearBorders));
            }
        }

        private void barBtnTestTime_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            customRichEdit1.Document.InsertText(customRichEdit1.Document.Selection.Start, DateTime.Now.ToString(Variables.timeStringFormat));
        }

        private void barBtnTestDate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            customRichEdit1.Document.InsertText(customRichEdit1.Document.Selection.Start, DateTime.Now.ToString(Variables.dateStringFormat));
        }

        private void barBtnTestDateTime_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            customRichEdit1.Document.InsertText(customRichEdit1.Document.Selection.Start, DateTime.Now.ToString(Variables.dateTimeStringFormat));
        }

        private void bBtnAutoFormat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            List<Table> customTables = new List<Table>();
            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

            foreach (RangePermission rangePermission in rangePermissions)
            {
                //Signature block has a parent cell so it'll take care of itself
                //if (rangePermission.Group == CustomUserGroupListService.SIGNATURE_BLOCK || rangePermission.Group == CustomUserGroupListService.SELECTION_TESTEQ)
                if (rangePermission.Group == CustomUserGroupListService.SIGNATURE_BLOCK)
                {
                    TableCell tCell = customRichEdit1.Document.Tables.GetTableCell(rangePermission.Range.Start);
                    if(tCell != null)
                        customTables.Add(tCell.Table);
                }
            }

            //get the next page threshold
            TableCell selectedCell = customRichEdit1.Document.Tables.GetTableCell(customRichEdit1.Document.Selection.Start);
            Document document = customRichEdit1.Document;

            if (selectedCell != null)
            {
                if (customTables.IndexOf(selectedCell.Table) != -1) //if this is a signature table
                {
                    Table signatureTable = selectedCell.Table;
                    if (signatureTable.Range.Start.ToInt() < customRichEdit1.Document.Selection.Start.ToInt() && signatureTable.Range.End.ToInt() > customRichEdit1.Document.Selection.Start.ToInt()) //indicates the table is spanning across pages
                    {
                        Paragraph paragraph = document.Paragraphs.Get(document.CreatePosition(signatureTable.FirstRow.FirstCell.ContentRange.Start.ToInt() - 1));
                        document.InsertText(paragraph.Range.Start, Characters.PageBreak.ToString());
                    }
                }
                else
                {
                    if (selectedCell.Range.Start.ToInt() < customRichEdit1.Document.Selection.Start.ToInt() && selectedCell.Range.End.ToInt() > customRichEdit1.Document.Selection.Start.ToInt()) //indicates that cell is spanning across pages
                    {
                        TableRow tRow = selectedCell.Row;
                        TableCell mergedCell = customRichEdit1.GetMergedRowCell(tRow);
                        if (mergedCell != null)
                        {
                            Table currentTable = mergedCell.Table;
                            int cellColumn = mergedCell.Index;

                            for (int cellRow = mergedCell.Row.Index; cellRow >= 0; cellRow--)
                            {

                                TableCell cellAbove = currentTable.Cell(cellRow, cellColumn);

                                if (cellAbove.Borders.Top.LineStyle != TableBorderLineStyle.None)
                                {
                                    document.CaretPosition = cellAbove.ContentRange.Start;
                                    new SplitTableCommand(customRichEdit1).Execute();
                                    document.InsertText(document.Selection.Start, Characters.PageBreak.ToString());
                                    break;
                                }
                            }
                        }
                        else
                        {
                            document.CaretPosition = selectedCell.ContentRange.Start;
                            new SplitTableCommand(customRichEdit1).Execute();
                            document.InsertText(document.Selection.Start, Characters.PageBreak.ToString());
                        }
                    }
                }
            }
        }

        private void customRichEdit1_DocumentLoaded(object sender, EventArgs e)
        {
            customRichEdit1.Document.DefaultCharacterProperties.FontName = "Calibri";
            customRichEdit1.Document.DefaultCharacterProperties.FontSize = 9;
        }

        /// <summary>
        /// Sets the selected text to be editable outside design mode
        /// </summary>
        private void bBtnEnableEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertPermissionForTable_WithInteractableRefresh(CustomUserGroupListService.USER, string.Empty, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void bBtnEnableSupervisorEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertPermissionForTable_WithInteractableRefresh(CustomUserGroupListService.SUPERVISOR, string.Empty, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonDatePicker_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertPermissionForTable_WithInteractableRefresh(CustomUserGroupListService.DATEPICKER, string.Empty, true);
        }

        private void barButtonDateTimePicker_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertPermissionForTable_WithInteractableRefresh(CustomUserGroupListService.DATETIMEPICKER, string.Empty, true);
        }

        private void barButtonTimePicker_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertPermissionForTable_WithInteractableRefresh(CustomUserGroupListService.TIMEPICKER, string.Empty, true);
        }

        private void barButtonPhoto_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertInteractables("Take Photo", CustomUserGroupListService.PHOTO, true);
        }

        /// <summary>
        /// Insert an interactive acceptance text block
        /// </summary>
        private void barButtonInsertAcceptanceToggle_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_Acceptance.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.TOGGLE_ACCEPTANCE, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonToggleYesNo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.TOGGLE_YESNO, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonInsertSignature_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertPermissionForTable_WithInteractableRefresh(CustomUserGroupListService.SIGNATURE_BLOCK, string.Empty);
        }

        private void barButtonInsertSignatureBlock_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            customRichEdit1.Document.BeginUpdate();
            customRichEdit1.Document.Paragraphs.Insert(customRichEdit1.Document.Selection.Start);
            int numOfRows = 5;
            if (_template.templateDiscipline == Discipline.Electrical.ToString() || _template.templateDiscipline == Discipline.Instrumentation.ToString())
                numOfRows = 6;

            Table tblSignature = customRichEdit1.Document.Tables.Create(customRichEdit1.Document.Selection.Start, numOfRows, 5, AutoFitBehaviorType.AutoFitToWindow);

            customRichEdit1.Document.InsertText(tblSignature.Cell(0, 1).Range.Start, "TEST / INSPECTION CARRIED OUT BY");
            customRichEdit1.Document.InsertText(tblSignature.Cell(0, 2).Range.Start, "TEST / INSPECTION APPROVED BY");
            customRichEdit1.Document.InsertText(tblSignature.Cell(0, 3).Range.Start, "TEST / INSPECTION AUTHORISED BY");
            customRichEdit1.Document.InsertText(tblSignature.Cell(0, 4).Range.Start, "CLIENT ACCEPTANCE");
            customRichEdit1.Document.InsertText(tblSignature.Cell(1, 0).Range.Start, "Signature:");
            customRichEdit1.Document.InsertText(tblSignature.Cell(2, 0).Range.Start, "Name:");
            customRichEdit1.Document.InsertText(tblSignature.Cell(3, 0).Range.Start, "Company:");
            customRichEdit1.Document.InsertText(tblSignature.Cell(4, 0).Range.Start, "Date:");

            if (_template.templateDiscipline == Discipline.Electrical.ToString() || _template.templateDiscipline == Discipline.Instrumentation.ToString())
                customRichEdit1.Document.InsertText(tblSignature.Cell(5, 0).Range.Start, "EWID:");

            //format signature cell margins
            for (int i = 1; i < 5; i++)
            {
                tblSignature.Cell(1, i).BottomPadding = 0;
                tblSignature.Cell(1, i).TopPadding = 0;
                tblSignature.Cell(1, i).LeftPadding = 0;
                tblSignature.Cell(1, i).RightPadding = 0;
                ParagraphProperties pp = customRichEdit1.Document.BeginUpdateParagraphs(tblSignature.Cell(1, i).Range);
                pp.Alignment = ParagraphAlignment.Left;
                customRichEdit1.Document.EndUpdateParagraphs(pp);
            }

            //format header columns
            for (int i = 1; i < 5; i++)
            {
                CharacterProperties cp = customRichEdit1.Document.BeginUpdateCharacters(tblSignature.Cell(0, i).Range);
                ParagraphProperties pp = customRichEdit1.Document.BeginUpdateParagraphs(tblSignature.Cell(0, i).Range);
                cp.Bold = true;
                pp.Alignment = ParagraphAlignment.Center;
                customRichEdit1.Document.EndUpdateCharacters(cp);
                customRichEdit1.Document.EndUpdateParagraphs(pp);
            }

            //format title rows
            for (int i = 1; i < numOfRows; i++)
            {
                CharacterProperties cp = customRichEdit1.Document.BeginUpdateCharacters(tblSignature.Cell(i, 0).Range);
                ParagraphProperties pp = customRichEdit1.Document.BeginUpdateParagraphs(tblSignature.Cell(i, 0).Range);
                cp.Bold = true;
                pp.Alignment = ParagraphAlignment.Left;
                customRichEdit1.Document.EndUpdateCharacters(cp);
                customRichEdit1.Document.EndUpdateParagraphs(pp);
            }

            customRichEdit1.Document.CaretPosition = tblSignature.Cell(0, 0).Range.Start;
            InsertInteractables("", CustomUserGroupListService.SIGNATURE_BLOCK);
            customRichEdit1.Document.EndUpdate();
        }

        private void barButtonInsertTestEQ_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertPermissionForTable_WithInteractableRefresh(CustomUserGroupListService.SELECTION_TESTEQ, "TEST EQUIPMENT", true);
        }

        /// <summary>
        /// Insert an interactive test equipment text block
        /// </summary>
        private void barButtonInsertTestEQTable_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            customRichEdit1.Document.BeginUpdate();
            customRichEdit1.Document.Paragraphs.Insert(customRichEdit1.Document.Selection.Start);
            Table tblTestEq = customRichEdit1.Document.Tables.Create(customRichEdit1.Document.Selection.Start, 5, 5, AutoFitBehaviorType.AutoFitToWindow);
            //tblTestEq.MergeCells(tblTestEq.Cell(0, 0), tblTestEq.Cell(1, 0));
            tblTestEq.TableAlignment = TableRowAlignment.Center;
            customRichEdit1.Document.InsertText(tblTestEq.Cell(0, 1).Range.Start, "MAKE");
            customRichEdit1.Document.InsertText(tblTestEq.Cell(0, 2).Range.Start, "MODEL");
            customRichEdit1.Document.InsertText(tblTestEq.Cell(0, 3).Range.Start, "SERIAL NO");
            customRichEdit1.Document.InsertText(tblTestEq.Cell(0, 4).Range.Start, "EXPIRY");

            for (int i = 1; i <= 4; i++)
            {
                CharacterProperties cp1 = customRichEdit1.Document.BeginUpdateCharacters(tblTestEq.Cell(0, i).Range);
                ParagraphProperties pp1 = customRichEdit1.Document.BeginUpdateParagraphs(tblTestEq.Cell(0, i).Range);
                cp1.Bold = true;
                pp1.Alignment = ParagraphAlignment.Center;
                customRichEdit1.Document.EndUpdateCharacters(cp1);
                customRichEdit1.Document.EndUpdateParagraphs(pp1);
            }

            for (int j = 2; j < 5; j++)
            {
                tblTestEq.MergeCells(tblTestEq.Cell(j, 0), tblTestEq.Cell(j, 4));
            }


            customRichEdit1.Document.InsertText(tblTestEq.Cell(3, 0).Range.Start, "Additional Inspector Remarks or Remedial Action Required: ");
            customRichEdit1.Document.InsertText(tblTestEq.Cell(4, 0).Range.Start, "Additional Supervisor Remarks or Remedial Action Required: ");
            customRichEdit1.Document.CaretPosition = tblTestEq.Cell(1, 0).Range.Start;
            InsertInteractables("TEST EQUIPMENT", CustomUserGroupListService.SELECTION_TESTEQ, true);
            customRichEdit1.Document.CaretPosition = tblTestEq.Cell(2, 0).Range.Start;
            string RowInsertText = "Click Here to Insert Row Above"; //need to insert space at the beginning so that character property won't propagate into previous cell
            InsertInteractables(RowInsertText, CustomUserGroupListService.INSERT_LINE, false);
            string RowRemoveText = "Click Here to Remove Row Above"; //need to insert space at the beginning so that character property won't propagate into previous cell
            InsertInteractables(RowRemoveText, CustomUserGroupListService.REMOVE_LINE, false);
            DocumentPosition RowInsertDocPos = customRichEdit1.Document.CreatePosition(tblTestEq.Cell(2, 0).ContentRange.Start.ToInt() + 3);
            DocumentRange RowInsertDocRange = customRichEdit1.Document.CreateRange(RowInsertDocPos, RowInsertText.Length - 3);

            CharacterProperties cp2 = customRichEdit1.Document.BeginUpdateCharacters(RowInsertDocRange);
            cp2.Bold = true;
            customRichEdit1.Document.EndUpdateCharacters(cp2);

            ParagraphProperties pp2 = customRichEdit1.Document.BeginUpdateParagraphs(tblTestEq.Cell(2, 0).Range);
            pp2.Alignment = ParagraphAlignment.Center;
            customRichEdit1.Document.EndUpdateParagraphs(pp2);

            customRichEdit1.Document.Selection = customRichEdit1.Document.CreateRange(tblTestEq.Cell(3, 0).Range.End.ToInt() - 2, 1);
            bBtnEnableEdit_ItemClick(null, null);
            customRichEdit1.Document.Selection = customRichEdit1.Document.CreateRange(tblTestEq.Cell(4, 0).Range.End.ToInt() - 1, 1);
            InsertPermissionForTable_WithInteractableRefresh(CustomUserGroupListService.SUPERVISOR, string.Empty, true);
            customRichEdit1.Document.EndUpdate();
        }

        /// <summary>
        /// Focus on the editables range depending on user selection
        /// </summary>
        private void barEditSelectEditables_EditValueChanged(object sender, EventArgs e)
        {
            if (barEditSelectEditables.EditValue.ToString() == string.Empty)
                return;

            customRichEdit1.Document.Selection = ((RangePermission)((ValuePair)barEditSelectEditables.EditValue).Value).Range;
        }

        /// <summary>
        /// Focuses on the interactables range depending on user selection
        /// </summary>
        private void barEditSelectInteractables_EditValueChanged(object sender, EventArgs e)
        {
            if (barEditSelectInteractables.EditValue.ToString() == string.Empty)
                return;

            customRichEdit1.Document.Selection = ((RangePermission)((ValuePair)barEditSelectInteractables.EditValue).Value).Range;
            #region Multicolumn ComboBox Implementation - customRichEdit1 gets truncated atm (bug)
            //if (barEditSelectInteractables.EditValue.ToString() == string.Empty)
            //    return;

            //customRichEdit1.Document.Selection = ((RangePermission)((Interactables)barEditSelectInteractables.EditValue).interactablesPermission).Range;

            #endregion        
        }

        /// <summary>
        /// Disable edit on editable range depending on user selection
        /// </summary>
        private void barButtonDeleteEditables_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barEditSelectEditables.EditValue == null || barEditSelectEditables.EditValue.ToString() == string.Empty)
                return;

            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            RangePermission selectedPermission = (RangePermission)((ValuePair)barEditSelectEditables.EditValue).Value;

            RangePermission rangePermission = rangePermissions.FirstOrDefault(obj => obj.Range.Start == selectedPermission.Range.Start && obj.Range.End == selectedPermission.Range.End);

            if (rangePermissions != null)
            {
                rangePermissions.Remove(rangePermission);
            }

            customRichEdit1.Document.EndUpdateRangePermissions(rangePermissions);
            barEditSelectEditables.EditValue = string.Empty;
            Refresh_Interactables_ComboBox();
        }

        /// <summary>
        /// Remove interactables on range depending on user selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonDeleteInteractables_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barEditSelectInteractables.EditValue == null || barEditSelectInteractables.EditValue.ToString() == string.Empty)
                return;

            try
            {
                RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
                RangePermission selectedPermission = (RangePermission)((ValuePair)barEditSelectInteractables.EditValue).Value;
                RangePermission rangePermission = rangePermissions.FirstOrDefault(obj => obj.Range.Start == selectedPermission.Range.Start && obj.Range.End == selectedPermission.Range.End);

                if (rangePermissions != null)
                {
                    #region Support for removing the whole block. Removed since user is allowed to insert interactables by itself only
                    //if (selectedPermission.Group != CustomUserGroupListService.TOGGLE_ACCEPTANCE)
                    //{
                    //    TableCell tCell = customRichEdit1.Document.GetTableCell(selectedPermission.Range.Start);
                    //    Table table = tCell.Table;
                    //    customRichEdit1.Document.Tables.Remove(table);
                    //} 
                    #endregion

                    //customRichEdit1.Document.Replace(rangePermission.Range, string.Empty);
                    rangePermissions.Remove(rangePermission);
                }

                customRichEdit1.Document.EndUpdateRangePermissions(rangePermissions);
                barEditSelectInteractables.EditValue = string.Empty;
                RefreshInteractablesRange();
            }
            catch
            {
                barEditSelectInteractables.EditValue = string.Empty;
                RefreshInteractablesRange();
            }
        }

        /// <summary>
        /// Handles automated interaction with ranges
        /// </summary>
        private void customRichEdit1_Click(object sender, EventArgs e)
        {
            #region Development
            //customRichEdit1.RichEditClick_Interactions(false);
            #endregion
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Reserve shortcut keys for processcmdkeys
        /// </summary>
        private void ReserveShortcutKeys()
        {
            customRichEdit1.RemoveShortcutKey(Keys.Alt, Keys.E);
            customRichEdit1.RemoveShortcutKey(Keys.Control, Keys.E);
            customRichEdit1.RemoveShortcutKey(Keys.Control, Keys.R);
            customRichEdit1.RemoveShortcutKey(Keys.Control, Keys.W);
        }

        /// <summary>
        /// Populate pre-fill fields
        /// </summary>
        private void PopulatePrefill(string discipline)
        {
            DataTable prefillSourceTable = new DataTable();
            using (AdapterPREFILL_MAIN daPrefill = new AdapterPREFILL_MAIN())
            {
                //add system prefill field
                prefillSourceTable.Columns.Add(Variables.prefillTagNumber);
                prefillSourceTable.Columns.Add(Variables.prefillTagDescription);
                prefillSourceTable.Columns.Add(Variables.prefillProjNumber);
                prefillSourceTable.Columns.Add(Variables.prefillProjName);
                prefillSourceTable.Columns.Add(Variables.prefillProjClient);
                prefillSourceTable.Columns.Add(Variables.prefillDocumentName);
                prefillSourceTable.Columns.Add(Variables.prefillTaskNumber);
                prefillSourceTable.Columns.Add(Variables.prefillDate);
                prefillSourceTable.Columns.Add(Variables.prefillDateTime);
                prefillSourceTable.Columns.Add(Variables.prefillChild);
                prefillSourceTable.Columns.Add(Variables.prefillCertificateNumber);
                prefillSourceTable.Columns.Add(Variables.prefillCertificateDescription);

                if (discipline.ToUpper() == "ELECTRICAL")
                {
                    List<string> electricalHeaderList = Common.GetElectricalHeaderList();
                    foreach(string electricalHeader in electricalHeaderList)
                    {
                        prefillSourceTable.Columns.Add(electricalHeader);
                    }
                }

                dsPREFILL_MAIN.PREFILL_MAINDataTable dtPrefillGeneral = daPrefill.GetByDiscipline(Variables.prefillGeneral);
                if(dtPrefillGeneral != null)
                {
                    foreach(dsPREFILL_MAIN.PREFILL_MAINRow drPrefill in dtPrefillGeneral)
                    {
                        if(!prefillSourceTable.Columns.Contains(drPrefill.NAME))
                            prefillSourceTable.Columns.Add(drPrefill.NAME);
                    }
                }

                dsPREFILL_MAIN.PREFILL_MAINDataTable dtPrefill = daPrefill.GetByDiscipline(discipline);
                if (dtPrefill != null)
                {
                    foreach (dsPREFILL_MAIN.PREFILL_MAINRow drPrefill in dtPrefill.Rows)
                    {
                        if (!prefillSourceTable.Columns.Contains(drPrefill.NAME))
                            prefillSourceTable.Columns.Add(drPrefill.NAME);
                    }
                }

                using(AdapterTEMPLATE_TOGGLE daTEMPLATE_TOGGLE = new AdapterTEMPLATE_TOGGLE())
                {
                    dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE = daTEMPLATE_TOGGLE.GetBy_Discipline(_template.templateDiscipline);
                    if(dtTEMPLATE_TOGGLE != null)
                    {
                        foreach(dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow drTEMPLATE_TOGGLE in dtTEMPLATE_TOGGLE.Rows)
                        {
                            if (!prefillSourceTable.Columns.Contains(drTEMPLATE_TOGGLE.NAME))
                                prefillSourceTable.Columns.Add(drTEMPLATE_TOGGLE.NAME);
                        }
                    }
                }

                customRichEdit1.Options.MailMerge.DataSource = prefillSourceTable;
            }
        }

        /// <summary>
        /// Build up the interactive combobox
        /// </summary>
        private void BuildInteractablesComboBox()
        {
            RefreshInteractablesRange();
            Refresh_Template_Toggles();
            interactablesBindingSource.DataSource = _allInteractables;
            templateToggleBindingSource.DataSource = _allTemplateToggle;

            ////Specify visible columns
            repositoryItemSearchLookUpEdit1.View.Columns.AddField("interactablesCategory").Visible = true;
            repositoryItemSearchLookUpEdit1.View.Columns.AddField("interactablesName").Visible = true;
            ////Specify columns parameters
            repositoryItemSearchLookUpEdit1.View.Columns.ColumnByFieldName("interactablesCategory").Caption = "Category";
            repositoryItemSearchLookUpEdit1.View.Columns.ColumnByFieldName("interactablesCategory").MaxWidth = 100;
            repositoryItemSearchLookUpEdit1.View.Columns.ColumnByFieldName("interactablesName").Caption = "Name";

            repositoryItemSearchLookUpToggles.View.Columns.AddField("toggleName").Visible = true;
            repositoryItemSearchLookUpToggles.View.Columns.AddField("toggleDescription").Visible = true;
            repositoryItemSearchLookUpToggles.View.Columns.ColumnByFieldName("toggleName").Caption = "Name";
            repositoryItemSearchLookUpToggles.View.Columns.ColumnByFieldName("toggleDescription").Caption = "Description";
            repositoryItemSearchLookUpToggles.DisplayMember = "toggleName";
            repositoryItemSearchLookUpToggles.ValueMember = "toggleName";
        }

        /// <summary>
        /// Assign local method to commands on richedit
        /// </summary>
        private void SetReplaceCommandMethod()
        {
            _replaceCommandMethod.Add(new RichEditCommandDelegateContainer(RichEditCommandId.FileSave, new RichEditCommandDelegate(SaveTemplateToDB)));
            IRichEditCommandFactoryService service = customRichEdit1.GetService(typeof(IRichEditCommandFactoryService)) as IRichEditCommandFactoryService;
            customRichEdit1.ReplaceService<IRichEditCommandFactoryService>(new CustomRichEditCommandFactoryService(service, customRichEdit1, _replaceCommandMethod));
            customRichEdit1.ReplaceService<IUserGroupListService>(new CustomUserGroupListService());
        }

        /// <summary>
        /// Refreshes interactable combobox
        /// </summary>
        private void Refresh_Interactables_ComboBox()
        {
            repositoryItemComboBox1.Items.Clear();

            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group == CustomUserGroupListService.USER)
                    repositoryItemComboBox1.Items.Add(new ValuePair(rangePermission.Range.Start.ToString() + "-" + rangePermission.Range.End.ToString(), rangePermission));
            }

            barEditSelectEditables.Refresh();
        }

        /// <summary>
        /// Refreshes interactables combobox
        /// </summary>
        private void RefreshInteractablesRange()
        {
            repositoryItemComboBox2.Items.Clear();

            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group != CustomUserGroupListService.USER)
                    repositoryItemComboBox2.Items.Add(new ValuePair(rangePermission.Group.ToString() + ": " + rangePermission.Range.Start.ToString() + "-" + rangePermission.Range.End.ToString(), rangePermission));
            }

            barEditSelectInteractables.Refresh();

            #region Multicolumn ComboBox Implementation - customRichEdit1 gets truncated atm (bug)
            //_allInteractables.Clear();

            //RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            //customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

            //foreach (RangePermission rangePermission in rangePermissions)
            //{
            //    if (rangePermission.Group != CustomUserGroupListService.USER)
            //    {
            //        _allInteractables.Add(new Interactables(rangePermission.Group, rangePermission.Range.Start.ToString() + " - " + rangePermission.Range.End.ToString(), rangePermission));
            //    }
            //}

            //barEditSelectInteract.Refresh(); 
            #endregion
        }

        private void Refresh_Template_Toggles()
        {
            _allTemplateToggle.Clear();
            using(AdapterTEMPLATE_TOGGLE daTemplateToggles = new AdapterTEMPLATE_TOGGLE(Variables.ConnStr))
            {
                dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE = daTemplateToggles.GetBy_Discipline(_template.templateDiscipline);
                if(dtTEMPLATE_TOGGLE != null)
                {
                    foreach (dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow drTEMPLATE_TOGGLE in dtTEMPLATE_TOGGLE.Rows)
                    {
                        _allTemplateToggle.Add(new Template_Toggle(drTEMPLATE_TOGGLE.GUID)
                        {
                            toggleName = drTEMPLATE_TOGGLE.NAME,
                            toggleDescription = drTEMPLATE_TOGGLE.DESCRIPTION,
                            CreatedDate = drTEMPLATE_TOGGLE.CREATED,
                            CreatedBy = drTEMPLATE_TOGGLE.CREATEDBY
                        });
                    }
                }
            }

            barEditSelectToggles.Refresh();
        }
        #endregion

        #region Helper
        private void InsertPermissionForTable_WithInteractableRefresh(string PermissionName, string Text, bool Stick = false, object fontNameObj = null, object fontSizeObj = null)
        {
            string fontName = string.Empty;
            if (fontNameObj != null)
                fontName = fontNameObj.ToString();

            int? fontSize = null;
            if (fontSizeObj != null)
            {
                int parseFontSize;
                if (Int32.TryParse(fontSizeObj.ToString(), out parseFontSize))
                    fontSize = parseFontSize;
            }

            customRichEdit1.InsertPermissionForTable(PermissionName, Text, Stick, fontName, fontSize);
            Refresh_Interactables_ComboBox();
        }

        private bool isPunchlistBlockExist()
        {
            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            RangePermission rangePermission = rangePermissions.FirstOrDefault(obj => obj.Group == CustomUserGroupListService.PUNCHLIST_BLOCK);

            if (rangePermission != null)
                return true;

            return false;
        }

        /// <summary>
        /// Removes the selected cell range permission
        /// </summary>
        private void RemoveSelectedCellPermissions()
        {
            SelectionCollection Selections = customRichEdit1.Document.Selections;
            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();

            foreach (DocumentRange Selection in Selections)
            {
                TableCell tCell = customRichEdit1.Document.Tables.GetTableCell(Selection.Start);
                if (tCell != null)
                {
                    int cellStartPosition = tCell.Range.Start.ToInt();
                    int selectionEndPosition = Selection.End.ToInt();
                    do
                    {
                        DocumentPosition rowPosition = customRichEdit1.Document.CreatePosition(cellStartPosition);
                        TableCell rowCell = customRichEdit1.Document.Tables.GetTableCell(rowPosition);
                        if (rowCell != null)
                        {
                            RangePermission findRangePermission = rangePermissions.FirstOrDefault(obj => rowCell.Range.Contains(obj.Range.Start));
                            if (findRangePermission != null)
                            {
                                rowCell.BackgroundColor = Color.Transparent;
                                SubDocument SubDoc = findRangePermission.Range.BeginUpdateDocument();
                                selectionEndPosition = selectionEndPosition - (findRangePermission.Range.End.ToInt() - findRangePermission.Range.Start.ToInt());
                                //SubDoc.Replace(findRangePermission.Range, string.Empty);
                                SubDoc.EndUpdate();
                                rangePermissions.Remove(findRangePermission);
                            }
                            cellStartPosition = rowCell.Range.End.ToInt() + 1; //Add 1 to move to next cell
                        }
                        else
                            cellStartPosition += 1;
                    } while (cellStartPosition < selectionEndPosition);
                }
            }

            customRichEdit1.Document.EndUpdateRangePermissions(rangePermissions);
        }

        /// <summary>
        /// Clear table cell used by clearCellDelegate
        /// </summary>
        void ClearCell(TableCell cell, int i, int j)
        {
            if (i > 0 && j > 0)
                customRichEdit1.Document.Replace((cell).Range, string.Empty);
        }

        /// <summary>
        /// Save the richedit template to database
        /// </summary>
        private void SaveTemplateToDB()
        {
            string errorRowSpanningAcrossPagesContent = customRichEdit1.IsAnyRowSpansAcrossPages();
            if(errorRowSpanningAcrossPagesContent != string.Empty)
            {
                Common.Warn("Please keep rows within the same page\n\nCurrently is a row that spans across pages which contain the following text\n\n" + errorRowSpanningAcrossPagesContent);
                return;
            }

            dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = _daTemplate.GetBy(_template.GUID);
            if (drTemplate != null)
            {
                MemoryStream ms = new MemoryStream();
                customRichEdit1.SaveDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                drTemplate.TEMPLATE = ms.ToArray();
                drTemplate.UPDATED = DateTime.Now;
                drTemplate.UPDATEDBY = System_Environment.GetUser().GUID;
                _daTemplate.Save(drTemplate);
            }
        }


        /// <summary>
        /// Checks the document on standard formatting requirement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonCheckDocument_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int fixedTableCount = 0; ;
            bool isDocumentNameExist = false;
            bool isTaskIdExist = false;
            int signatureCount = 0;
            int testEquipmentCount = 0;

            //Check if table width type is fixed, and also get auto fixed count
            fixedTableCount = VerifyTableWidthType();

            //Check if document contains document name prefill
            isDocumentNameExist = SearchDocumentText("<<" + Variables.prefillDocumentName + ">>");

            //Check if document contains task number prefill
            isTaskIdExist = SearchDocumentText("<<" + Variables.prefillTaskNumber + ">>");

            //Get the number of signature block and test equipment count
            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group == CustomUserGroupListService.SIGNATURE_BLOCK)
                    signatureCount += 1;
                else if (rangePermission.Group == CustomUserGroupListService.SELECTION_TESTEQ)
                    testEquipmentCount += 1;
            }

            string messageString = "Fixed Table(s): " + fixedTableCount.ToString() + "\n"
                            + "Document Name Exists: " + isDocumentNameExist.ToString() + "\n"
                            + "Task Number Exists: " + isTaskIdExist.ToString() + "\n"
                            + "Signature Block Count: " + signatureCount.ToString() + "\n"
                            + "Test Equipment Count: " + testEquipmentCount.ToString();

            if (!isDocumentNameExist || !isTaskIdExist || signatureCount == 0)
                Common.Warn(messageString);
            else
                Common.Prompt(messageString);
        }

        /// <summary>
        /// Inserts the current user signature to test the signature block
        /// </summary>
        private void barButtonTestSignature_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

            AdapterUSER_MAIN daUser = new AdapterUSER_MAIN();
            dsUSER_MAIN.USER_MAINRow drUser = daUser.GetIncludeDeletedBy(System_Environment.GetUser().GUID);
            if (drUser != null)
            {
                if (!drUser.IsSIGNATURENull())
                {
                    Bitmap userSignature = new Bitmap(Common.ConvertByteArrayToImage(drUser.SIGNATURE));
                    userSignature = Common.ResizeBitmap(userSignature, 300, 150);
                    string userName = drUser.LASTNAME + ", " + drUser.FIRSTNAME;
                    string userCompany = drUser.COMPANY;
                    string UserInfo = drUser.IsINFONull() ? string.Empty : drUser.INFO;

                    foreach (RangePermission rangePermission in rangePermissions)
                    {
                        if (rangePermission.Group == CustomUserGroupListService.SIGNATURE_BLOCK)
                        {
                            TableCell tCell = customRichEdit1.Document.Tables.GetTableCell(rangePermission.Range.Start);
                            Table tblSignature = tCell.Table;
                            tblSignature.ForEachCell(new TableCellProcessorDelegate(customRichEdit1.ClearCell));

                            try
                            {
                                customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 1).Range.Start, userSignature);
                                customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 2).Range.Start, userSignature);
                                customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 3).Range.Start, userSignature);
                                customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 4).Range.Start, userSignature);

                                customRichEdit1.Document.InsertText(tblSignature.Cell(2, 1).Range.Start, userName);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(2, 2).Range.Start, userName);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(2, 3).Range.Start, userName);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(2, 4).Range.Start, userName);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(3, 1).Range.Start, userCompany);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(3, 2).Range.Start, userCompany);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(3, 3).Range.Start, userCompany);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(3, 4).Range.Start, userCompany);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(4, 1).Range.Start, DateTime.Now.ToString("d"));
                                customRichEdit1.Document.InsertText(tblSignature.Cell(4, 2).Range.Start, DateTime.Now.ToString("d"));
                                customRichEdit1.Document.InsertText(tblSignature.Cell(4, 3).Range.Start, DateTime.Now.ToString("d"));
                                customRichEdit1.Document.InsertText(tblSignature.Cell(4, 4).Range.Start, DateTime.Now.ToString("d"));

                                if (_template.templateDiscipline == Common.Replace_WithSpaces(Discipline.Electrical.ToString()) || _template.templateDiscipline == Common.Replace_WithSpaces(Discipline.Instrumentation.ToString()))
                                {
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(5, 1).Range.Start, UserInfo);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(5, 2).Range.Start, UserInfo);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(5, 3).Range.Start, UserInfo);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(5, 4).Range.Start, UserInfo);
                                }
                            }
                            catch
                            {
                                //Populate what the system can and ignore any errors
                                //tblSignature.ForEachCell(new TableCellProcessorDelegate(customRichEdit1.ClearCell));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unpopulate the signature from signature block testing
        /// </summary>
        private void barButtonRemoveSignature_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group == CustomUserGroupListService.SIGNATURE_BLOCK)
                {
                    TableCell tCell = customRichEdit1.Document.Tables.GetTableCell(rangePermission.Range.Start);
                    Table tblSignature = tCell.Table;
                    tblSignature.ForEachCell(new TableCellProcessorDelegate(customRichEdit1.ClearCell));
                }
            }
        }

        /// <summary>
        /// Try to fix table width and verify if all tables width type is fixed, so that when user input text the tablecell with expand vertically
        /// </summary>
        private int VerifyTableWidthType()
        {
            int pFixedTblCount = 0;
            //customRichEdit1.Document.CaretPosition = customRichEdit1.Document.CreatePosition(0);

            //Get document section
            Section firstSection = customRichEdit1.Document.Sections[0];
            SubDocument headerDoc = firstSection.BeginUpdateHeader();

            customRichEdit1.Document.ChangeActiveDocument(headerDoc);
            //loop through tables in header
            foreach(Table headerTable in headerDoc.Tables)
            {
                if(headerTable.PreferredWidthType != WidthType.Fixed)
                {
                    customRichEdit1.Document.CaretPosition = headerTable.Range.Start;
                    new ToggleTableFixedColumnWidthCommand(customRichEdit1).Execute();
                    headerTable.TableLayout = TableLayoutType.Fixed;
                    headerTable.PreferredWidthType = WidthType.Fixed;
                    pFixedTblCount += 1;
                }
            }
            customRichEdit1.Document.Sections[0].EndUpdateHeader(headerDoc);

            SubDocument footerDoc = firstSection.BeginUpdateFooter();
            customRichEdit1.Document.ChangeActiveDocument(footerDoc);
            foreach (Table footerTable in footerDoc.Tables)
            {
                //auto fixing for other table width type will skew the table
                if (footerTable.PreferredWidthType != WidthType.Fixed)
                {
                    customRichEdit1.Document.CaretPosition = footerTable.Range.Start;
                    new ToggleTableFixedColumnWidthCommand(customRichEdit1).Execute();
                    footerTable.TableLayout = TableLayoutType.Fixed;
                    footerTable.PreferredWidthType = WidthType.Fixed;
                    pFixedTblCount += 1;
                }
            }
            customRichEdit1.Document.Sections[0].EndUpdateFooter(footerDoc);

            customRichEdit1.Document.ChangeActiveDocument(customRichEdit1.Document);
            //loop through tables in main document
            foreach (Table table in customRichEdit1.Document.Tables)
            {
                //auto fixing for other table width type will skew the table
                if (table.PreferredWidthType != WidthType.Fixed)
                {
                    customRichEdit1.Document.CaretPosition = table.Range.Start;
                    new ToggleTableFixedColumnWidthCommand(customRichEdit1).Execute();
                    table.TableLayout = TableLayoutType.Fixed;
                    table.PreferredWidthType = WidthType.Fixed;
                    pFixedTblCount += 1;
                }
            }


            return pFixedTblCount;
        }

        /// <summary>
        /// Searches the document including header and footer for specific string
        /// </summary>
        private bool SearchDocumentText(string findText)
        {
            bool searchBit = false ;
            //Get text from header
            Section firstSection = customRichEdit1.Document.Sections[0];

            SubDocument headerDoc = firstSection.BeginUpdateHeader();
            for (int i = 0; i < headerDoc.Fields.Count; i++)
            {
                string getText = headerDoc.GetText(headerDoc.Fields[i].Range);
                if (getText == findText)
                    searchBit = true;
            }
            customRichEdit1.Document.Sections[0].EndUpdateHeader(headerDoc);

            if (customRichEdit1.Document.FindAll(findText, SearchOptions.None).Count() > 0)
                searchBit = true;

            //Get text from footer
            SubDocument footerDoc = firstSection.BeginUpdateFooter();
            for (int i = 0; i < footerDoc.Fields.Count; i++)
            {
                string getText = footerDoc.GetText(footerDoc.Fields[i].Range);
                if (getText == findText)
                    searchBit = true;
            }
            customRichEdit1.Document.Sections[0].EndUpdateFooter(footerDoc);

            return searchBit;
        }

        /// <summary>
        /// Insert an interactable
        /// </summary>
        /// <param name="TextBlock">TextBlock to show on Rich Edit</param>
        /// <param name="PermissionName">Group name to add in permission</param>
        /// <param name="Stick">Allow permission to not stick to text to aid touch interaction</param>
        private void InsertInteractables(string TextBlock, string PermissionName, bool Stick = false, object fontNameObj = null, object fontSizeObj = null, bool isZeroLeftPadding = false)
        {
            string fontName = string.Empty;
            if (fontNameObj != null)
                fontName = fontNameObj.ToString();

            int? fontSize = null;
            if (fontSizeObj != null)
            {
                int parseFontSize;
                if (Int32.TryParse(fontSizeObj.ToString(), out parseFontSize))
                    fontSize = parseFontSize;
            }

            SelectionCollection Selections = customRichEdit1.Document.Selections;
            customRichEdit1.Document.BeginUpdate();
            foreach (DocumentRange Selection in Selections)
            {
                //int startPos = Selection.Start.ToInt() + 1;
                //DocumentPosition originalPosition = Selection.Start;
                //SubDocument subDoc = Selection.BeginUpdateDocument();

                //subDoc.InsertText(originalPosition, " " + TextBlock + " "); //add spaces to prevent previous and subsequent content from sticking to the permission group
                //subDoc.EndUpdate();

                //DocumentRange docRange = customRichEdit1.Document.CreateRange(startPos, TextBlock.Length);
                //List<Paragraph> para = customRichEdit1.Document.Paragraphs.Get(docRange).ToList();
                //foreach (Paragraph p1 in para)
                //{
                //    p1.KeepLinesTogether = true;
                //}
                //CharacterProperties cp = customRichEdit1.Document.BeginUpdateCharacters(docRange);
                //cp.Bold = true;
                //customRichEdit1.Document.EndUpdateCharacters(cp);
                TableCell tCell = customRichEdit1.Document.Tables.GetTableCell(Selection.Start);

                if (tCell != null)
                {
                    if(isZeroLeftPadding)
                        tCell.LeftPadding = 0;

                    if (customRichEdit1.Document.GetText(tCell.ContentRange).Trim() == string.Empty)
                        customRichEdit1.Document.Replace(tCell.ContentRange, string.Empty);

                    // Obtain character properties
                    CharacterProperties cp = customRichEdit1.Document.BeginUpdateCharacters(tCell.Range);

                    if (fontName != string.Empty)
                        cp.FontName = fontName;
                    
                    if (fontSize != null)
                        cp.FontSize = (int)fontSize;

                    // Finalize modifications
                    customRichEdit1.Document.EndUpdateCharacters(cp);

                    customRichEdit1.InsertRowPermission(PermissionName, tCell.Range.Start.ToInt(), Selection.End.ToInt(), Stick, TextBlock);
                }
                //customRichEdit1.InsertRangePermission(PermissionName, docRange);
            }
            customRichEdit1.Document.EndUpdate();
            RefreshInteractablesRange();
        }
        #endregion

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.E))
                bBtnEnableEdit_ItemClick(null, null);
            if (keyData == (Keys.Control | Keys.Alt | Keys.E))
                bBtnEnableSupervisorEdit_ItemClick(null, null);
            if (keyData == (Keys.Control | Keys.R))
                RemoveSelectedCellPermissions();
            if (keyData == (Keys.Control | Keys.W))
                barButtonInsertAcceptanceToggle_ItemClick(null, null);
            if (keyData == (Keys.Control | Keys.Alt | Keys.W))
                barButtonInitials_ItemClick(null, null);
            if (keyData == Keys.F3)
                barButtonInsertSignature_ItemClick(null, null);
            if (keyData == Keys.F4)
                barButtonCheckDocument_ItemClick(null, null);
            if (keyData == Keys.F1)
                bBtnPopulateHeader_ItemClick(null, null);
            if (keyData == Keys.F2)
                bBtnPopulateFooter_ItemClick(null, null);

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void bBtnPopulateHeader_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            customRichEdit1.Populate_Header_Field(barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        public void FixHeaderFields()
        {
            customRichEdit1.Populate_Header_Field(barEditFontName.EditValue, barEditFontSize.EditValue);
            SaveTemplateToDB();
        }

        private void barButtonItemCertificateNumber_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            insertMergeField(Variables.prefillCertificateNumber);
        }

        private void barButtonItemCertificateDescription_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            insertMergeField(Variables.prefillCertificateDescription);
        }

        private void insertMergeField(string mergeField)
        {
            SelectionCollection Selections = customRichEdit1.Document.Selections;
            customRichEdit1.Document.BeginUpdate();
            foreach (DocumentRange Selection in Selections)
            {
                TableCell tCell = customRichEdit1.Document.Tables.GetTableCell(Selection.Start);
                if (tCell != null)
                {
                    customRichEdit1.Document.Fields.Create(tCell.Range.Start, " MERGEFIELD \"" + mergeField + "\"");
                }
            }

            customRichEdit1.Document.EndUpdate();
            customRichEdit1.Options.MailMerge.ViewMergedData = true;
            customRichEdit1.Options.MailMerge.ViewMergedData = false;
        }

        public void FixFooterFields()
        {
            customRichEdit1.ResizeFooterForQRCode();
            SaveTemplateToDB();
        }

        private void bBtnPrefill_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmPrefill_Main f = new frmPrefill_Main();
            f.ClosePrefill = new frmPrefill_Main.PrefillClose(Refresh_MergeField);
            f.ShowDialog();
        }

        private void bBtnPopulateFooter_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MessageBox.Show("If footer doesn't turn out to be fine please go to Review Tab and click Accept All Revision button", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            customRichEdit1.Populate_Footer_Field(barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void btnAcceptAllRevision_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            customRichEdit1.AcceptAllRevision();
        }

        private void barButtonDisciplineArchitectural_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.DISCIPLINE_TOGGLE_ARCHITECTURAL, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonDisciplineCivils_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.DISCIPLINE_TOGGLE_CIVILS, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonDisciplineElectrical_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.DISCIPLINE_TOGGLE_ELECTRICAL, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonDisciplineInstrumentation_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.DISCIPLINE_TOGGLE_INSTRUMENTATION, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonDisciplineMechanical_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.DISCIPLINE_TOGGLE_MECHANICAL, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonDisciplinePiping_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.DISCIPLINE_TOGGLE_PIPING, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonDisciplineStructural_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.DISCIPLINE_TOGGLE_STRUCTURAL, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonDisciplineOthers_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.DISCIPLINE_TOGGLE_OTHERS, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonItemSubsystem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertInteractables(Variables.Select_Subsystem_String, CustomUserGroupListService.SUBSYSTEM_SELECTION, false, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonItemCVCType_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertInteractables(Variables.Select_CVC_Type, CustomUserGroupListService.CVC_TYPE, false, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonItemTag_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InsertInteractables(Variables.Select_Tag, CustomUserGroupListService.CERTIFICATE_TAG_SELECTION, false, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonItemArchitecturalITR_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.ITR_COMPLETION_ARCHITECTURAL, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonItemCivilITR_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.ITR_COMPLETION_CIVIL, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonItemMechanical_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.ITR_COMPLETION_MECHANICAL, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonItemInstrumentation_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.ITR_COMPLETION_INSTRUMENTATION, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonItemStructuralITR_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.ITR_COMPLETION_STRUCTURAL, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonItemPiping_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.ITR_COMPLETION_PIPING, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonItemElectricalITR_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.ITR_COMPLETION_ELECTRICAL, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonItemPunchlistA_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.PUNCHLIST_CAT_A, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonCVCCompletion_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.CVC_SUBSYSTEM_COMPLETION, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonPLWDCompletion_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString());
            InsertInteractables(s, CustomUserGroupListService.PUNCHLIST_WALKDOWN_SUBSYSTEM_COMPLETION, true, barEditFontName.EditValue, barEditFontSize.EditValue);
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmCertificateRoleSelection frmCertificateRoleSelection = new frmCertificateRoleSelection();
            if(frmCertificateRoleSelection.ShowDialog() == DialogResult.OK)
            {
                string selectedRole = frmCertificateRoleSelection.GetSelectedCVCRole();
                selectedRole = Common.ReplaceSpacesWith_(selectedRole);

                string s = Common.Replace_WithSpaces(selectedRole + " " + Toggle_YesNo.Click_Here.ToString());
                InsertInteractables(s, CustomUserGroupListService.CVC_STATUS_TOGGLE_ACCEPTANCE_PREFIX + selectedRole, true, barEditFontName.EditValue, barEditFontSize.EditValue);
            }
        }
    }

    /// <summary>
    /// A model with category for holding interactables
    /// </summary>
    public class Interactables
    {
        public Interactables(string Category, string Name, RangePermission Permission)
        {
            interactablesCategory = Category;
            interactablesName = Name;
            interactablesPermission = Permission;
        }

        public string interactablesName { get; set; }
        public string interactablesCategory { get; set; }
        public RangePermission interactablesPermission { get; set; }

        public override string ToString()
        {
            return interactablesCategory + " " + interactablesName;
        }
    }
}
