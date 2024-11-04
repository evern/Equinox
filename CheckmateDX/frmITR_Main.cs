using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Data;
using System.Windows.Forms;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.Office;
using DevExpress.XtraEditors.Controls;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.Pdf;
using ZXing;
using ZXing.Common.Detector;
using ZXing.Common;
using System.Drawing.Printing;

namespace CheckmateDX
{
    public partial class frmITR_Main : CheckmateDX.frmParent
    {
        Template _template;
        Tag _tag;
        WBS _wbs;
        Guid _iTRGuid = Guid.Empty;
        int _imageIndex; //stores the image index to determine whether user might need to comment during progression/rejection
        //bool _colorOnClick = false; //if set to true, highlighting event will commence on customRichEdit1_Click
        float _fontSizeOffset = 0;
        bool _doNotShowWaitForm = false;
        bool _onlyExecuteSupervisorInteraction = false;

        AdapterITR_MAIN _daITR = new AdapterITR_MAIN();
        AdapterITR_STATUS _daITRStatus = new AdapterITR_STATUS();
        AdapterITR_STATUS_ISSUE _daITRStatusIssue = new AdapterITR_STATUS_ISSUE();
        AdapterPROJECT _daProject = new AdapterPROJECT();
        AdapterTEMPLATE_MAIN _daTemplate = new AdapterTEMPLATE_MAIN();
        AdapterPREFILL_MAIN _daPrefill = new AdapterPREFILL_MAIN();
        AdapterPREFILL_REGISTER _daPrefillRegister = new AdapterPREFILL_REGISTER();
        AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN();
        List<RichEditCommandDelegateContainer> _replaceCommandMethod = new List<RichEditCommandDelegateContainer>();

        //Have to store the deleted ITR for ITR table refresh
        private Guid _primaryITRGuid = Guid.Empty;
        //private ITR_Local_Update _updateStatus;
        private bool _iTRInitialTemplateLoad; //indicate that this iTR was initially loaded from template
        //private bool _byPassProgressConfirmation = false;
        private bool _bypassSaveConfirmation = false;
        private iTRBrowser_Update _iTRBrowserUpdate;
        //private bool _qrSupport = false;
        private Guid? _projectGuid = null;

        List<Template_Toggle> _disabledToggle = new List<Template_Toggle>();
        DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        List<ITR_Refresh_Item> _itrRefreshItems = new List<ITR_Refresh_Item>();
        List<ITR_Refresh_Item> _ParallelWBSTagItems = new List<ITR_Refresh_Item>();
        dsITR_MAIN.ITR_MAINDataTable _dtITR_Deleted = new dsITR_MAIN.ITR_MAINDataTable();
        dsITR_MAIN.ITR_MAINDataTable _dtITR_Refresh = new dsITR_MAIN.ITR_MAINDataTable();
        Timer delayedImageDisposalTimer = new Timer();
        public frmITR_Main()
        {
            InitializeComponent();
        }

        public frmITR_Main(WorkflowTemplateTagWBS TemplateTagWBS, Action<frmITR_Main> Update_Status)
        {
            InitializeComponent();
            _iTRGuid = TemplateTagWBS.wtITRGuid;
            _doNotShowWaitForm = true;
            customRichEdit1.Document.DefaultCharacterProperties.FontName = "Candara";
            customRichEdit1.Document.DefaultCharacterProperties.FontSize = 9;

            _template = TemplateTagWBS.wtDisplayAttachTemplate;
            _template.GUID = TemplateTagWBS.wtTrueTemplateGuid; //a dummy guid was assigned in ITR browse because treeView doesn't accept duplicate Guid. The true guid is reassigned here
            _tag = TemplateTagWBS.wtDisplayAttachTag;
            _wbs = TemplateTagWBS.wtDisplayAttachWBS;
            _imageIndex = TemplateTagWBS.wtDisplayImageIndex;

            SetName(_tag, _wbs);
            //customRichEdit1 = new CustomRichEditAPI(customRichEdit1);
            //Check whether ITR exists
            dsITR_MAIN.ITR_MAINRow drITR = getITR();

            if (drITR != null) //if iTR can be found
            {
                _primaryITRGuid = drITR.GUID;
                MemoryStream ms = new MemoryStream(drITR.ITR);
                customRichEdit1.Document.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                customRichEdit1._templateGuid = _template.GUID;
                customRichEdit1._wbsTagGuid = _tag == null ? _wbs.GUID : _tag.GUID;
            }
            else  //if iTR cannot be found in iTR_MAIN
            {
                _iTRInitialTemplateLoad = true; //remember iTR was loaded from template
                customRichEdit1.LoadTemplateFromDB(_template.GUID, _tag == null ? _wbs.GUID : _tag.GUID);
            }

            if (System_Environment.GetUser().GUID == Guid.Empty)
            {
                customRichEdit1.Document.Unprotect();  //CustomEdittoFixSignature
                btnCleanUpQR.Visible = true;
                btnUnprotect.Visible = true;
                btnToggleField.Visible = true;
                btnInsertSignature.Visible = true;
                btnInsertEditable.Visible = true;
                btnInsertEquipment.Visible = true;
                btnInsertAcceptance.Visible = true;
                btnInsertEditableSup.Visible = true;
                btnReject.Visible = true;
            }
            else
                customRichEdit1.Document.Protect(string.Empty);  //CustomEdittoFixSignature

            //btnSave.Enabled = false;
            this.Update_Status = Update_Status;
            //customRichEdit1_Click(null, null);
            //need to populate current WBSTag item if TAG_SELECT exists
            _ParallelWBSTagItems = customRichEdit1.Get_Parallel_ItemsRevised(_iTRGuid, SelfWBSTagGUID, _tag == null, true);
            customRichEdit1.Document.EndUpdate();
            finishInitialLoading();
        }

        //read-only from punchlist
        public frmITR_Main(Guid itrGUID, Tag headerTag)
        {
            InitializeComponent();
            splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
            dsITR_MAIN.ITR_MAINRow drITR = _daITR.GetBy(itrGUID);
            if (drITR != null)
            {
                MemoryStream ms = new MemoryStream(drITR.ITR);
                customRichEdit1.Document.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                customRichEdit1.PopulatePrefill(headerTag, null, null, string.Empty, string.Empty, false);

                _primaryITRGuid = drITR.GUID;
                customRichEdit1._templateGuid = drITR.TEMPLATE_GUID;
                _tag = headerTag;
                customRichEdit1._wbsTagGuid = _tag == null ? _wbs.GUID : _tag.GUID;
                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = _daTemplate.GetBy(drITR.TEMPLATE_GUID);
                if (drTemplate != null)
                {
                    _template = new Template(drTemplate.GUID)
                    {
                        templateName = drTemplate.NAME,
                        templateWorkFlow = new ValuePair(Common.ConvertWorkflowGuidToName(drTemplate.WORKFLOWGUID), drTemplate.WORKFLOWGUID),
                        templateRevision = drTemplate.REVISION,
                        templateDescription = drTemplate.IsDESCRIPTIONNull() ? "" : drTemplate.DESCRIPTION,
                        templateDiscipline = drTemplate.DISCIPLINE
                    };

                    splashScreenManager1.ShowWaitForm();
                    StatusSpecificFormSetup(false); //needs to go after populate prefill so that toggle selection can be determined
                    splashScreenManager1.CloseWaitForm();
                }
                else
                    btnReject.Visible = false;
            }

            btnBrowsePunchlist.Visible = false;
            btnInsertAcceptance.Visible = false;
            btnInsertEditable.Visible = false;
            btnInsertEditableSup.Visible = false;
            btnInsertEquipment.Visible = false;
            btnInsertSignature.Visible = false;
            btnPunchlist.Visible = false;
            btnSave.Visible = false;
            btnToggleField.Visible = false;
            btnUnprotect.Visible = false;
            btnCleanUpQR.Visible = false;
            btnProgress.Visible = false;
            btnDelete.Visible = false;
            btnAttachImage.Visible = true;
            btnAppendPDF.Visible = false;
            btnReplacePDF.Visible = false;
            btnAttachImage.Visible = false;
            _bypassSaveConfirmation = true;
            finishInitialLoading();
        }

        private Action<frmITR_Main> Update_Status;
        public frmITR_Main(WorkflowTemplateTagWBS TemplateTagWBS, bool ConvertToNative, Action<frmITR_Main> Update_Status = null, Guid? projectGuid = null, decimal fontSizeOffset = 0)
        {
            splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("Initializing Editor ... ");
            InitializeComponent();

            _fontSizeOffset = float.Parse(fontSizeOffset.ToString());
            customRichEdit1.Document.DefaultCharacterProperties.FontName = "Candara";
            customRichEdit1.Document.DefaultCharacterProperties.FontSize = 9;

            _iTRGuid = TemplateTagWBS.wtITRGuid;
            _template = TemplateTagWBS.wtDisplayAttachTemplate;
            _template.GUID = TemplateTagWBS.wtTrueTemplateGuid; //a dummy guid was assigned in ITR browse because treeView doesn't accept duplicate Guid. The true guid is reassigned here
            _tag = TemplateTagWBS.wtDisplayAttachTag;
            _wbs = TemplateTagWBS.wtDisplayAttachWBS;
            _imageIndex = TemplateTagWBS.wtDisplayImageIndex;

            SetName(_tag, _wbs);
            //customRichEdit1 = new CustomRichEditAPI(customRichEdit1);
            //Check whether ITR exists
            dsITR_MAIN.ITR_MAINRow drITR = getITR();

            if (drITR != null) //if iTR can be found
            {
                splashScreenManager1.SetWaitFormCaption("Loading Document ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, 5);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);

                _primaryITRGuid = drITR.GUID;
                MemoryStream ms = new MemoryStream(drITR.ITR);
                customRichEdit1.Document.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                customRichEdit1._templateGuid = _template.GUID;
                customRichEdit1._wbsTagGuid = _tag == null ? _wbs.GUID : _tag.GUID;
            }
            else  //if iTR cannot be found in iTR_MAIN
            {
                splashScreenManager1.SetWaitFormCaption("Loading Template ... ");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, 9);
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);

                _iTRInitialTemplateLoad = true; //remember iTR was loaded from template
                customRichEdit1.LoadTemplateFromDB(_template.GUID, _tag == null ? _wbs.GUID : _tag.GUID);
            }
            
            customRichEdit1.SetSignatureUser(SignatureUserHelper.GetSignatureUser());
            customRichEdit1.Document.BeginUpdate();
            SetReplaceCommandMethod();

            splashScreenManager1.SetWaitFormCaption("Populating Header ... ");
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);

            //Custom template name so that custom properties can be inserted in the template name 
            string templateName = Common.RemoveCommentedTemplateSection(_template.templateName);
            customRichEdit1.PopulatePrefill(_tag, _wbs, null, templateName, _template.templateRevision, false);

            _projectGuid = projectGuid;
            //bool removeQRCode = false;
            //if (!_template.templateQRSupport)
            //    removeQRCode = true;
            //else
            //    customRichEdit1.InsertSignatureQRCode();


            splashScreenManager1.SetWaitFormCaption("Formatting Document ... ");
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);

            StatusSpecificFormSetup(ConvertToNative); //needs to go after populate prefill so that toggle selection can be determined

            splashScreenManager1.SetWaitFormCaption("Populating Signatures ... ");
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);

            //Test Comment Out
            PopulateSignatures();

            if (System_Environment.GetUser().GUID == Guid.Empty)
            {
                customRichEdit1.Document.Unprotect();  //CustomEdittoFixSignature
                btnUnprotect.Visible = true;
                btnCleanUpQR.Visible = true;
                btnToggleField.Visible = true;
                btnInsertSignature.Visible = true;
                btnInsertEditable.Visible = true;
                btnInsertEquipment.Visible = true;
                btnInsertAcceptance.Visible = true;
                btnInsertEditableSup.Visible = true;
                btnReject.Visible = true;
                btnAttachImage.Visible = true;
            }
            else
                customRichEdit1.Document.Protect(string.Empty);  //CustomEdittoFixSignature

            //btnSave.Enabled = false;
            this.Update_Status = Update_Status;
            //customRichEdit1_Click(null, null);
            //need to populate current WBSTag item if TAG_SELECT exists
            _ParallelWBSTagItems = customRichEdit1.Get_Parallel_ItemsRevised(_iTRGuid, SelfWBSTagGUID, _tag == null, true);
            ShrinkAllFont();
            splashScreenManager1.CloseWaitForm();
            customRichEdit1.Document.EndUpdate();
            finishInitialLoading();
        }

        private void finishInitialLoading()
        {
            customRichEdit1.Document.CaretPosition = customRichEdit1.Document.Range.Start;
            customRichEdit1.Modified = false;

            //use overload of 1 so that rich edit will attempt to place caret position at the bottom and effectively shows the header
            customRichEdit1.ScrollToCaret(1);
        }

        List<Image> delayedImageRecycleBin = new List<Image>();
        private void initializeImageDisposalTimer()
        {
            delayedImageDisposalTimer.Tick += DelayedImageDisposalTimer_Tick;
        }

        private void DelayedImageDisposalTimer_Tick(object sender, EventArgs e)
        {
            delayedImageDisposalTimer.Stop();
            for (int i = 0; i < delayedImageRecycleBin.Count; i++)
            {
                Image imageForDisposal = delayedImageRecycleBin[i];
                imageForDisposal.Dispose();
                delayedImageRecycleBin.Remove(imageForDisposal);
            }
        }

        //force is used for setting cell height to auto
        public void ShrinkAllFont(bool force = false)
        {
            if (!force && _fontSizeOffset == 0)
                return;

            customRichEdit1.Document.BeginUpdate();
            foreach (Table tbl in customRichEdit1.Document.Tables)
            {
                //use foreach cell to support merged cells
                tbl.ForEachCell(tableCellProcessor);
            }

            //Get fields from header
            Section firstSection = customRichEdit1.Document.Sections[0];

            SubDocument headerDoc = firstSection.BeginUpdateHeader();
            foreach (Table tbl in headerDoc.Tables)
            {
                tbl.ForEachRow(tableHeaderRowProcessor);
            }
            firstSection.EndUpdateHeader(headerDoc);

            SubDocument footerDoc = firstSection.BeginUpdateFooter();
            foreach (Table tbl in footerDoc.Tables)
            {
                tbl.ForEachRow(tableFooterRowProcessor);
            }
            firstSection.EndUpdateFooter(footerDoc);

            customRichEdit1.Document.ReplaceAll(DevExpress.Office.Characters.PageBreak.ToString(), "", DevExpress.XtraRichEdit.API.Native.SearchOptions.None);
            customRichEdit1.Document.EndUpdate();
        }

        private void tableHeaderRowProcessor(TableRow row, int rowIndex)
        {
            SubDocument headerDoc = customRichEdit1.Document.Sections[0].BeginUpdateHeader();
            foreach (TableCell tCell in row.Cells)
            {
                CharacterProperties cp = headerDoc.BeginUpdateCharacters(tCell.ContentRange);
                cp.FontSize += _fontSizeOffset;
                headerDoc.EndUpdateCharacters(cp);
            }

            row.HeightType = HeightType.Auto;
            customRichEdit1.Document.Sections[0].EndUpdateHeader(headerDoc);
        }

        List<float> fontSizeList = new List<float>();
        private void tableCellProcessor(TableCell cell, int rowIndex, int cellIndex)
        {
            CharacterProperties cp = customRichEdit1.Document.BeginUpdateCharacters(cell.ContentRange);
            if (cp.FontSize != null)
            {
                cp.FontSize += _fontSizeOffset;
                fontSizeList.Add((float)cp.FontSize);
            }
            else if (fontSizeList.Count > 0)
            {
                float averageFontSize = fontSizeList.Sum() / fontSizeList.Count;
                cp.FontSize = averageFontSize;
            }

            customRichEdit1.Document.EndUpdateCharacters(cp);
            cell.HeightType = HeightType.Auto;
        }

        private void tableFooterRowProcessor(TableRow row, int rowIndex)
        {
            SubDocument footerDoc = customRichEdit1.Document.Sections[0].BeginUpdateFooter();
            foreach (TableCell tCell in row.Cells)
            {
                CharacterProperties cp = footerDoc.BeginUpdateCharacters(tCell.ContentRange);
                cp.FontSize += _fontSizeOffset;
                footerDoc.EndUpdateCharacters(cp);
            }

            row.HeightType = HeightType.Auto;
            customRichEdit1.Document.Sections[0].EndUpdateFooter(footerDoc);
        }

        #region Initialisation
        /// <summary>
        /// Set this form name for display and save file name purpose
        /// </summary>
        private void SetName(Tag tag, WBS wbs)
        {
            string documentName = "Document";
            if (tag != null)
                documentName = _tag.tagNumber.Replace("\\n", "");
            else if (wbs != null)
                documentName = _wbs.wbsName;

            documentName = Common.ReplaceSpacesWith_(documentName);
            documentName += "_" + _template.templateName + "_" + _template.templateRevision;

            this.Text = documentName;
        }

        private ITR_Status? getITRStatus(out bool ITR_Exists)
        {
            dsITR_MAIN.ITR_MAINRow drITR = getITR();
            if (drITR != null)
            {
                ITR_Exists = true;
                dsITR_STATUS.ITR_STATUSRow drITRStatus = _daITRStatus.GetRowBy(drITR.GUID);
                if (drITRStatus != null)
                {
                    return (ITR_Status)drITRStatus.STATUS_NUMBER;
                }
            }
            else
                ITR_Exists = false;

            return null;
        }

        ITR_Status? _ITRStatus = null;
        /// <summary>
        /// Setup status specific form parameters
        /// </summary>
        private void StatusSpecificFormSetup(bool RemoveAllPermissions)
        {
            bool ITR_Exists;
            _ITRStatus = getITRStatus(out ITR_Exists);
            if (ITR_Exists)
            {
                splashScreenManager1.SetWaitFormCaption("Updating Tag Numbers");
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                _ParallelWBSTagItems = customRichEdit1.Get_Parallel_ItemsRevised(_iTRGuid, SelfWBSTagGUID, _tag == null);
                if (_ITRStatus != null)
                {
                    //customRichEdit1.ApplyPageBreaks();
                    if (_ITRStatus == ITR_Status.Inspected)
                    {
                        //Release 1.0.0.117 - Allow supervisor to edit ITR before approving
                        if (System_Environment.HasPrivilege(PrivilegeTypeID.MarkITRApproved))
                        {
                            customRichEdit1.Options.Authentication.Group = CustomUserGroupListService.SUPERVISOR; //permission to edit
                            _onlyExecuteSupervisorInteraction = true;
                            customRichEdit1.Click += customRichEdit1_Click;
                        }

                        btnProgress.Tag = customRichEdit1.CheckInteractables();
                        //_colorOnClick = true;
                        //Release 1.0.0.117 - Allow supervisor to edit ITR before approving

                        //Straight to complete Release 1.2.0.12
                        if (_template.templateSkipApproved)
                            btnProgress.Text = "Complete";
                        else
                            btnProgress.Text = "Approve";
                    }
                    else if (_ITRStatus == ITR_Status.Approved)
                    {
                        btnProgress.Text = "Complete";
                        _bypassSaveConfirmation = true;
                    }
                    else if (_ITRStatus == ITR_Status.Completed)
                    {
                        btnProgress.Text = "Close";
                        _bypassSaveConfirmation = true;
                        //btnProgress.Visible = false;
                        //btnReject.Visible = false;
                    }
                    else if (_ITRStatus >= ITR_Status.Closed)
                    {
                        _bypassSaveConfirmation = true;
                        btnProgress.Visible = false;
                        //btnReject.Visible = true; //06801 changes
                        btnReject.Visible = false;
                    }
                    else //-1, rejected to saved
                    {
                        if (System_Environment.HasPrivilege(PrivilegeTypeID.CreateITR))
                        {
                            customRichEdit1.Options.Authentication.Group = CustomUserGroupListService.USER; //permission to edit
                            customRichEdit1.Click += customRichEdit1_Click;
                        }
                        else
                            customRichEdit1.Click += customRichEdit1_InvalidClick;

                        btnProgress.Tag = customRichEdit1.CheckInteractables();

                        //ColorInteractables(true); //document has been saved with color
                        //_colorOnClick = true;
                        btnProgress.Text = "Submit";
                        btnReject.Visible = false;
                    }
                }
                else //saved
                {
                    if (System_Environment.HasPrivilege(PrivilegeTypeID.CreateITR))
                    {
                        customRichEdit1.Options.Authentication.Group = CustomUserGroupListService.USER; //permission to edit
                        customRichEdit1.Click += customRichEdit1_Click;
                    }
                    else
                        customRichEdit1.Click += customRichEdit1_InvalidClick;

                    //ColorInteractables(true); //document has been saved with color
                    btnProgress.Tag = customRichEdit1.CheckInteractables();

                    //_colorOnClick = true;
                    btnProgress.Text = "Submit";
                    btnReject.Visible = false;
                }
            }
            else //template
            {
                if (System_Environment.HasPrivilege(PrivilegeTypeID.CreateITR))
                {
                    customRichEdit1.Options.Authentication.Group = CustomUserGroupListService.USER; //permission to edit
                    customRichEdit1.Click += customRichEdit1_Click;
                }
                else
                    customRichEdit1.Click += customRichEdit1_InvalidClick;

                //_colorOnClick = true;
                //customRichEdit1.Set_TouchUI(true);
                btnProgress.Tag = customRichEdit1.CheckInteractables();
                List<string> OptionalToggle = new List<string>();
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                splashScreenManager1.SetWaitFormCaption("Predefined Rows ... ");
                splashScreenManager1.SetWaitFormDescription("Retrieving ... ");
                _disabledToggle = Retrieve_Toggle_State_From_Header(out OptionalToggle);

                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                splashScreenManager1.SetWaitFormCaption("Predefined Rows ... ");
                splashScreenManager1.SetWaitFormDescription("Formatting ... ");
                //customRichEdit1.Remove_Disabled_Tables(_disabledToggle);

                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                splashScreenManager1.SetWaitFormCaption("Optional Rows ... ");
                splashScreenManager1.SetWaitFormDescription("Retrieving ... ");

                if (OptionalToggle.Count > 0)
                {
                    splashScreenManager1.CloseWaitForm();
                    _disabledToggle = Prompt_Toggle_Selection(OptionalToggle);
                    splashScreenManager1.ShowWaitForm(); //initialize a new instance of wait form so that it shows after the dialog has been closed
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, 10);
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 8);
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                    splashScreenManager1.SetWaitFormCaption("Optional Rows ... ");
                    splashScreenManager1.SetWaitFormDescription("Formatting ... ");
                    customRichEdit1.Remove_Disabled_Tables(_disabledToggle);
                }

                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                splashScreenManager1.SetWaitFormCaption("Row Toggles ... ");
                splashScreenManager1.SetWaitFormDescription("Removing ... ");

                customRichEdit1.Modified = false;
                //customRichEdit1.ColorInteractables(true, false); //Color document on template
                btnProgress.Text = "Submit";
                btnReject.Visible = false;
                btnDelete.Enabled = false;

                if (RemoveAllPermissions)
                {
                    customRichEdit1.Convert_to_Native();
                    customRichEdit1.Remove_Toggle_Permissions();
                }
            }
        }

        public void PrintPreparation()
        {
            customRichEdit1.Convert_to_Native();
            customRichEdit1.Remove_Toggle_Permissions();
        }

        private List<Template_Toggle> Retrieve_Toggle_State_From_Header(out List<string> OptionalToggles)
        {
            //Retrieve non-reserved word from permissions (Currently used only by toggles)
            List<string> TogglePermissions = customRichEdit1.Get_Custom_Permissions();
            List<Template_Toggle> Disabled_Toggles = new List<Template_Toggle>();
            Section firstSection = customRichEdit1.Document.Sections[0];
            SubDocument headerDoc = firstSection.BeginUpdateHeader();

            int togglePrefillEnabledCount = 0;
            OptionalToggles = new List<string>();
            for (int i = 0; i < headerDoc.Fields.Count; i++)
            {
                string togglePrefill = headerDoc.GetText(headerDoc.Fields[i].CodeRange);
                togglePrefill = togglePrefill.Replace(" MERGEFIELD \"", string.Empty);
                togglePrefill = togglePrefill.Replace("\"", string.Empty);
                if (TogglePermissions.Contains(togglePrefill)) //if this is a toggle prefill
                {
                    string toggleResult = headerDoc.GetText(headerDoc.Fields[i].ResultRange);

                    if (toggleResult.ToUpper() == "O")
                        OptionalToggles.Add(togglePrefill);
                    else if (toggleResult.ToUpper() != "Y")
                        Disabled_Toggles.Add(new Template_Toggle(Guid.Empty) { toggleName = togglePrefill, toggleEnabled = false });
                    else
                        togglePrefillEnabledCount += 1;
                }
            }

            //if no toggle prefill is found, assume that its fully optional
            if (TogglePermissions.Count > 0 && (togglePrefillEnabledCount + Disabled_Toggles.Count == 0))
                OptionalToggles = TogglePermissions;

            customRichEdit1.Document.Sections[0].EndUpdateHeader(headerDoc);

            return Disabled_Toggles;
        }

        private List<Template_Toggle> Prompt_Toggle_Selection(List<string> OptionalToggle)
        {
            List<Template_Toggle> DisabledToggle = new List<Template_Toggle>();
            if (_template == null)
                return DisabledToggle;

            List<string> RichEditPermissions;
            if (OptionalToggle == null)
                RichEditPermissions = customRichEdit1.Get_Custom_Permissions();
            else
                RichEditPermissions = OptionalToggle;

            if (RichEditPermissions.Count > 0)
            {
                frmITR_Select_Toggle f = new frmITR_Select_Toggle(RichEditPermissions, _template.templateDiscipline);
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    List<Template_Toggle> ReturnedToggle = f.Get_Template_Toggles();
                    foreach (Template_Toggle Toggle in ReturnedToggle)
                    {
                        if (!Toggle.toggleEnabled)
                            DisabledToggle.Add(Toggle);
                    }
                }
                else
                {
                    List<Template_Toggle> ReturnedToggle = f.Get_Template_Toggles();
                    foreach (Template_Toggle Toggle in ReturnedToggle)
                    {
                        DisabledToggle.Add(Toggle);
                    }
                }
            }

            return DisabledToggle;
        }

        /// <summary>
        /// Assign local method to commands on richedit
        /// </summary>
        private void SetReplaceCommandMethod()
        {
            _replaceCommandMethod.Add(new RichEditCommandDelegateContainer(RichEditCommandId.FileSave, new RichEditCommandDelegate(Shortcut_btnSaveClick)));
            IRichEditCommandFactoryService service = customRichEdit1.GetService(typeof(IRichEditCommandFactoryService)) as IRichEditCommandFactoryService;
            customRichEdit1.ReplaceService<IRichEditCommandFactoryService>(new CustomRichEditCommandFactoryService(service, customRichEdit1, _replaceCommandMethod));
            customRichEdit1.ReplaceService<IUserGroupListService>(new CustomUserGroupListService());
        }

        private void attachmentExtraFeatures()
        {
            //Section lastSection = customRichEdit1.Document.Sections[customRichEdit1.Document.Sections.Count - 1];
            //int startPosition = lastSection.Paragraphs[0].Range.Start.ToInt();
            //int endPosition = lastSection.Paragraphs[lastSection.Paragraphs.Count - 1].Range.End.ToInt();
            //DocumentRange lastSectionDocumentRange = customRichEdit1.Document.CreateRange(startPosition, endPosition - startPosition);
            //string restDocumentText = customRichEdit1.Document.GetText(lastSectionDocumentRange);

            //if(restDocumentText.Replace("\\r", "").Replace("\\n", "").Trim() == "")
            //{
            //    customRichEdit1.Document.Delete(lastSectionDocumentRange);
            //}

            btnProgress.Text = "Submit";
        }

        /// <summary>
        /// Populate user signatures based on ITR status
        /// </summary>
        private void PopulateSignatures()
        {
            bool isAttachment = isITRAttachment();
            //remove last page for attachment
            if (isAttachment)
            {
                attachmentExtraFeatures();
                //replaceSignatureBlock();

            }
            else
            {
                RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
                customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

                dsITR_MAIN.ITR_MAINRow drITR = getITR();
                if (drITR == null)
                {
                    //Remove signature QRCode
                    //foreach (RangePermission rangePermission in rangePermissions)
                    //{
                    //    if (rangePermission.Group == CustomUserGroupListService.SIGNATURE_BLOCK)
                    //    {
                    //        Image QRCode = BarCodeUtility.GetQRCode("SIGNATURE");
                    //        TableCell tCell = customRichEdit1.Document.Tables.GetTableCell(rangePermission.Range.Start);
                    //        Table tblSignature = tCell.Table;
                    //        //if (_template.templateQRSupport)
                    //        customRichEdit1.Document.Images.Insert(tblSignature.Cell(0, 0).ContentRange.Start, QRCode);
                    //    }
                    //}
                }
                else if (drITR != null)
                {
                    dsITR_STATUS.ITR_STATUSRow drITRLatestStatus = _daITRStatus.GetRowBy(drITR.GUID);
                    dsITR_STATUS.ITR_STATUSDataTable dtITRStatus = _daITRStatus.GetAllExcludeRejected(drITR.GUID);
                    if (dtITRStatus == null)
                        return;

                    int defaultSignatureWidth = 120 + (15 * Convert.ToInt32(_fontSizeOffset));
                    int defaultSignatureHeight = 120 + (15 * Convert.ToInt32(_fontSizeOffset));
                    List<SignatureUser> signature_users = SignatureUserHelper.GetSignatureUser(dtITRStatus, (ITR_Status)drITRLatestStatus.STATUS_NUMBER);
                    foreach (RangePermission rangePermission in rangePermissions)
                    {
                        if (rangePermission.Group == CustomUserGroupListService.SIGNATURE_BLOCK)
                        {
                            TableCell tCell = customRichEdit1.Document.Tables.GetTableCell(rangePermission.Range.Start);
                            if (tCell == null)
                                return;

                            Table tblSignature = tCell.Table;
                            tblSignature.ForEachCell(new TableCellProcessorDelegate(customRichEdit1.ClearCell));

                            bool isClientSignatureColumnExists = true;

                            try
                            {
                                isClientSignatureColumnExists = tblSignature.Cell(1, 4) != null;
                            }
                            catch
                            {
                                isClientSignatureColumnExists = false;
                            }

                            //if (isClientSignatureColumnExists && customRichEdit1.Document.GetText(tblSignature.Cell(1, 4).Range).Contains(Variables.General_NotApplicable) && btnProgress.Text.Contains("Close"))
                            //    btnProgress.Enabled = false;

                            SignatureUser inspector = signature_users.FirstOrDefault(x => x.SignStatus == ITR_Status.Inspected);
                            SignatureUser supervisor = signature_users.FirstOrDefault(x => x.SignStatus == ITR_Status.Approved);
                            SignatureUser projectmanager = signature_users.FirstOrDefault(x => x.SignStatus == ITR_Status.Completed);
                            SignatureUser client = signature_users.FirstOrDefault(x => x.SignStatus == ITR_Status.Closed);

                            //Image QRCode = BarCodeUtility.GetQRCode("SIGNATURE");
                            //customRichEdit1.Document.Delete(tblSignature.Cell(0, 0).ContentRange);
                            //if (_template.templateQRSupport)
                            //customRichEdit1.Document.Images.Insert(tblSignature.Cell(0, 0).ContentRange.Start, QRCode);

                            customRichEdit1.Document.Delete(tblSignature.Cell(1, 0).ContentRange);
                            customRichEdit1.Document.Delete(tblSignature.Cell(2, 0).ContentRange);
                            customRichEdit1.Document.Delete(tblSignature.Cell(3, 0).ContentRange);
                            customRichEdit1.Document.Delete(tblSignature.Cell(4, 0).ContentRange);
                            customRichEdit1.Document.InsertText(tblSignature.Cell(1, 0).Range.Start, "Signature");
                            customRichEdit1.Document.InsertText(tblSignature.Cell(2, 0).Range.Start, "Name");
                            customRichEdit1.Document.InsertText(tblSignature.Cell(3, 0).Range.Start, "Company");
                            customRichEdit1.Document.InsertText(tblSignature.Cell(4, 0).Range.Start, "Date");

                            setContentBold(tblSignature.Cell(1, 0).ContentRange);
                            setContentBold(tblSignature.Cell(2, 0).ContentRange);
                            setContentBold(tblSignature.Cell(3, 0).ContentRange);
                            setContentBold(tblSignature.Cell(4, 0).ContentRange);


                            if (drITR.DISCIPLINE == Common.Replace_WithSpaces(Discipline.Electrical.ToString()) || drITR.DISCIPLINE == Common.Replace_WithSpaces(Discipline.Instrumentation.ToString()))
                            {
                                //Some templates didn't get designed with EWID in mind
                                try
                                {
                                    customRichEdit1.Document.Delete(tblSignature.Cell(5, 0).ContentRange);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(5, 0).Range.Start, "EWID");
                                    setContentBold(tblSignature.Cell(5, 0).ContentRange);
                                }
                                catch
                                {

                                }
                            }

                            try
                            {
                                if (inspector != null)
                                {
                                    if (inspector.Signature != null)
                                    {
                                        Bitmap resizedSignature = Common.ResizeBitmap(inspector.Signature, defaultSignatureWidth, defaultSignatureHeight);
                                        customRichEdit1.Document.Delete(tblSignature.Cell(1, 1).ContentRange);
                                        customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 1).Range.Start, resizedSignature);
                                    }

                                    customRichEdit1.Document.Delete(tblSignature.Cell(2, 1).ContentRange);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(2, 1).Range.Start, inspector.Name);
                                    customRichEdit1.Document.Delete(tblSignature.Cell(3, 1).ContentRange);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(3, 1).Range.Start, inspector.Company);
                                    customRichEdit1.Document.Delete(tblSignature.Cell(4, 1).ContentRange);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(4, 1).Range.Start, inspector.SignDate.Year == 1 ? string.Empty : inspector.SignDate.ToString("d"));
                                }
                                if (supervisor != null)
                                {
                                    if (!isClientSignatureColumnExists)
                                    {
                                        if (projectmanager != null)
                                        {
                                            if (projectmanager.Signature != null)
                                            {
                                                Bitmap resizedSignature = Common.ResizeBitmap(projectmanager.Signature, defaultSignatureWidth, defaultSignatureHeight);
                                                customRichEdit1.Document.Delete(tblSignature.Cell(1, 2).ContentRange);
                                                customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 2).Range.Start, resizedSignature);
                                            }

                                            customRichEdit1.Document.Delete(tblSignature.Cell(2, 2).ContentRange);
                                            customRichEdit1.Document.InsertText(tblSignature.Cell(2, 2).Range.Start, projectmanager.Name);
                                            customRichEdit1.Document.Delete(tblSignature.Cell(3, 2).ContentRange);
                                            customRichEdit1.Document.InsertText(tblSignature.Cell(3, 2).Range.Start, projectmanager.Company);
                                            customRichEdit1.Document.Delete(tblSignature.Cell(4, 2).ContentRange);
                                            customRichEdit1.Document.InsertText(tblSignature.Cell(4, 2).Range.Start, projectmanager.SignDate.Year == 1 ? string.Empty : projectmanager.SignDate.ToString("d"));
                                        }
                                    }
                                    else
                                    {
                                        if (supervisor != null)
                                        {
                                            if (supervisor.Signature != null)
                                            {
                                                Bitmap resizedSignature = Common.ResizeBitmap(supervisor.Signature, defaultSignatureWidth, defaultSignatureHeight);
                                                customRichEdit1.Document.Delete(tblSignature.Cell(1, 2).ContentRange);
                                                customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 2).Range.Start, resizedSignature);
                                            }

                                            customRichEdit1.Document.Delete(tblSignature.Cell(2, 2).ContentRange);
                                            customRichEdit1.Document.InsertText(tblSignature.Cell(2, 2).Range.Start, supervisor.Name);
                                            customRichEdit1.Document.Delete(tblSignature.Cell(3, 2).ContentRange);
                                            customRichEdit1.Document.InsertText(tblSignature.Cell(3, 2).Range.Start, supervisor.Company);
                                            customRichEdit1.Document.Delete(tblSignature.Cell(4, 2).ContentRange);
                                            customRichEdit1.Document.InsertText(tblSignature.Cell(4, 2).Range.Start, supervisor.SignDate.Year == 1 ? string.Empty : supervisor.SignDate.ToString("d"));
                                        }
                                    }
                                }
                                if (projectmanager != null && isClientSignatureColumnExists)
                                {
                                    if (projectmanager.Signature != null)
                                    {
                                        Bitmap resizedSignature = Common.ResizeBitmap(projectmanager.Signature, defaultSignatureWidth, defaultSignatureHeight);
                                        customRichEdit1.Document.Delete(tblSignature.Cell(1, 3).ContentRange);
                                        customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 3).Range.Start, resizedSignature);
                                    }

                                    customRichEdit1.Document.Delete(tblSignature.Cell(2, 3).ContentRange);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(2, 3).Range.Start, projectmanager.Name);
                                    customRichEdit1.Document.Delete(tblSignature.Cell(3, 3).ContentRange);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(3, 3).Range.Start, projectmanager.Company);
                                    customRichEdit1.Document.Delete(tblSignature.Cell(4, 3).ContentRange);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(4, 3).Range.Start, projectmanager.SignDate.Year == 1 ? string.Empty : projectmanager.SignDate.ToString("d"));
                                }
                                if (client != null)
                                {
                                    if (isClientSignatureColumnExists)
                                    {
                                        if (client.Signature != null)
                                        {
                                            Bitmap resizedSignature = Common.ResizeBitmap(client.Signature, defaultSignatureWidth, defaultSignatureHeight);
                                            customRichEdit1.Document.Delete(tblSignature.Cell(1, 4).ContentRange);
                                            customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 4).Range.Start, resizedSignature);
                                        }

                                        customRichEdit1.Document.Delete(tblSignature.Cell(2, 4).ContentRange);
                                        customRichEdit1.Document.InsertText(tblSignature.Cell(2, 4).Range.Start, client.Name);
                                        customRichEdit1.Document.Delete(tblSignature.Cell(3, 4).ContentRange);
                                        customRichEdit1.Document.InsertText(tblSignature.Cell(3, 4).Range.Start, client.Company);
                                        customRichEdit1.Document.Delete(tblSignature.Cell(4, 4).ContentRange);
                                        customRichEdit1.Document.InsertText(tblSignature.Cell(4, 4).Range.Start, client.SignDate.Year == 1 ? string.Empty : client.SignDate.ToString("d"));
                                    }
                                    else
                                    {
                                        if (client != null && client.Signature != null)
                                        {
                                            Bitmap resizedSignature = Common.ResizeBitmap(client.Signature, defaultSignatureWidth, defaultSignatureHeight);
                                            customRichEdit1.Document.Delete(tblSignature.Cell(1, 3).ContentRange);
                                            customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 3).Range.Start, resizedSignature);
                                        }

                                        customRichEdit1.Document.Delete(tblSignature.Cell(2, 3).ContentRange);
                                        customRichEdit1.Document.InsertText(tblSignature.Cell(2, 3).Range.Start, client.Name);
                                        customRichEdit1.Document.Delete(tblSignature.Cell(3, 3).ContentRange);
                                        customRichEdit1.Document.InsertText(tblSignature.Cell(3, 3).Range.Start, client.Company);
                                        customRichEdit1.Document.Delete(tblSignature.Cell(4, 3).ContentRange);
                                        customRichEdit1.Document.InsertText(tblSignature.Cell(4, 3).Range.Start, client.SignDate.Year == 1 ? string.Empty : client.SignDate.ToString("d"));
                                    }
                                }

                                if (drITR.DISCIPLINE == Common.Replace_WithSpaces(Discipline.Electrical.ToString()) || drITR.DISCIPLINE == Common.Replace_WithSpaces(Discipline.Instrumentation.ToString()))
                                {
                                    customRichEdit1.Document.Delete(tblSignature.Cell(5, 1).ContentRange);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(5, 1).Range.Start, inspector == null ? string.Empty : inspector.AdditionalInfo);

                                    if (isClientSignatureColumnExists)
                                    {
                                        customRichEdit1.Document.Delete(tblSignature.Cell(5, 2).ContentRange);
                                        customRichEdit1.Document.InsertText(tblSignature.Cell(5, 2).Range.Start, supervisor == null ? string.Empty : supervisor.AdditionalInfo);
                                        customRichEdit1.Document.Delete(tblSignature.Cell(5, 3).ContentRange);
                                        customRichEdit1.Document.InsertText(tblSignature.Cell(5, 3).Range.Start, projectmanager == null ? string.Empty : projectmanager.AdditionalInfo);
                                        customRichEdit1.Document.Delete(tblSignature.Cell(5, 4).ContentRange);
                                        customRichEdit1.Document.InsertText(tblSignature.Cell(5, 4).Range.Start, client == null ? string.Empty : client.AdditionalInfo);
                                    }
                                    else
                                    {
                                        customRichEdit1.Document.Delete(tblSignature.Cell(5, 2).ContentRange);
                                        customRichEdit1.Document.InsertText(tblSignature.Cell(5, 2).Range.Start, projectmanager == null ? string.Empty : projectmanager.AdditionalInfo);
                                        customRichEdit1.Document.Delete(tblSignature.Cell(5, 3).ContentRange);
                                        customRichEdit1.Document.InsertText(tblSignature.Cell(5, 3).Range.Start, client == null ? string.Empty : client.AdditionalInfo);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                string s = e.ToString();
                                //Populate what the system can and ignore any errors
                                //tblSignature.ForEachCell(new TableCellProcessorDelegate(customRichEdit1.ClearCell));
                            }
                        }
                    }
                }
            }

            if(_template.templateQRSupport)
            {
                if(_ITRStatus < ITR_Status.Closed)
                {
                    customRichEdit1.InsertHeaderQRCode(getMetaTag());
                }
                else
                {
                    customRichEdit1.RemoveHeaderQRCode();
                }
            }
        }

        private void setContentBold(DocumentRange range)
        {
            SubDocument subDocument = range.BeginUpdateDocument();
            CharacterProperties cp = subDocument.BeginUpdateCharacters(range);
            cp.Bold = true;
            cp.FontName = "Candara";
            cp.FontSize = 9;
            subDocument.EndUpdateCharacters(cp);
            range.EndUpdateDocument(subDocument);

        }
        #endregion

        #region Events

        private void btnInsertEditableSup_Click(object sender, EventArgs e)
        {
            customRichEdit1.InsertPermissionForTable(CustomUserGroupListService.SUPERVISOR, string.Empty, true);
        }

        private void btnInsertAcceptance_Click(object sender, EventArgs e)
        {
            string s = Common.Replace_WithSpaces(Toggle_Acceptance.Click_Here.ToString());
            customRichEdit1.InsertPermissionForTable(s, CustomUserGroupListService.TOGGLE_ACCEPTANCE, true);
        }

        private void btnInsertEquipment_Click(object sender, EventArgs e)
        {
            customRichEdit1.InsertPermissionForTable(CustomUserGroupListService.SELECTION_TESTEQ, string.Empty, true);
        }

        private void btnInsertEditable_Click(object sender, EventArgs e)
        {
            customRichEdit1.InsertPermissionForTable(CustomUserGroupListService.USER, string.Empty, true);
        }

        private void btnInsertSignature_Click(object sender, EventArgs e)
        {
            customRichEdit1.InsertPermissionForTable(CustomUserGroupListService.SIGNATURE_BLOCK, string.Empty, false);
        }

        private void btnToggleField_Click(object sender, EventArgs e)
        {
            ShowAllFieldCodesCommand command = new ShowAllFieldCodesCommand(customRichEdit1);
            command.Execute();
        }

        private void btnUnprotect_Click(object sender, EventArgs e)
        {
            customRichEdit1.Document.EndUpdate();
        }

        private void btnCleanUpQR_Click(object sender, EventArgs e)
        {
            customRichEdit1.CleanUpQRCodes();
        }

        private void btnBrowsePunchlist_Click(object sender, EventArgs e)
        {
            //wbsTagDisplay iTRWBSTag = CreateWBSTagDisplay(_wbs, _tag);
            List<Guid> FilteredGuids = new List<Guid>();
            _ParallelWBSTagItems = customRichEdit1.Get_Parallel_ItemsRevised(_iTRGuid, SelfWBSTagGUID, _tag == null);
            foreach (ITR_Refresh_Item ParallelWBSTag in _ParallelWBSTagItems)
            {
                FilteredGuids.Add(ParallelWBSTag.WBSTagGuid);
            }

            frmPunchlist_Browse frmPunchlistBrowse = new frmPunchlist_Browse(FilteredGuids);
            frmPunchlistBrowse.ShowDialog();
        }

        private void customRichEdit1_DocumentLoaded(object sender, EventArgs e)
        {
            customRichEdit1.Document.DefaultCharacterProperties.FontName = "Candara";
            customRichEdit1.Document.DefaultCharacterProperties.FontSize = 9;
        }

        bool isRejecting = false;
        private void btnReject_Click(object sender, EventArgs e)
        {
            if (isRejecting)
                return;

            _ParallelWBSTagItems = customRichEdit1.Get_Parallel_ItemsRevised(_iTRGuid, SelfWBSTagGUID, _tag == null);

            //hereby populate ITR_GUID in the parallel items
            dsITR_MAIN.ITR_MAINDataTable dtParallelITR = Common.RevisedGetITRBy(_ParallelWBSTagItems);
            _dtITR_Refresh = dtParallelITR;

            string Comment = null;
            //dsITR_MAIN.ITR_MAINRow drITR = Common.GetITR(_tag, _wbs, _template.GUID);
            if (dtParallelITR != null)
            {
                foreach (dsITR_MAIN.ITR_MAINRow drITR in dtParallelITR.Rows)
                {
                    dsITR_STATUS.ITR_STATUSRow drITRStatus = _daITRStatus.GetRowBy(drITR.GUID);
                    if (drITRStatus != null)
                    {
                        //only validate privilege on first run
                        if (Comment == null)
                        {
                            if (drITRStatus.STATUS_NUMBER == (int)ITR_Status.Inspected && !System_Environment.HasPrivilege(PrivilegeTypeID.RejectITRInspected))
                            {
                                Common.Warn("You are not authorised to reject inspected ITR");
                                return;
                            }
                            else if (drITRStatus.STATUS_NUMBER == (int)ITR_Status.Approved && !System_Environment.HasPrivilege(PrivilegeTypeID.RejectITRApproved))
                            {
                                Common.Warn("You are not authorised to reject approved ITR");
                                return;
                            }
                            else if (drITRStatus.STATUS_NUMBER == (int)ITR_Status.Completed && !System_Environment.HasPrivilege(PrivilegeTypeID.RejectITRCompleted))
                            {
                                Common.Warn("You are not authorised to reject completed ITR");
                                return;
                            }
                        }

                        if (Comment == null)
                        {
                            frmITR_Comment f = new frmITR_Comment(drITR.GUID, "Reject");
                            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                Comment = f.GetComment();
                            else
                                return;
                        }

                        if (Comment != null)
                        {
                            isRejecting = true;
                            Guid iTRStatusGuid = Guid.NewGuid();
                            ChangeStatusWithUpdateOverriding(drITR.GUID, iTRStatusGuid, iTRStatusChange.Decrease, Comment);
                            if (drITRStatus.STATUS_NUMBER == (int)ITR_Status.Approved)
                            {
                                customRichEdit1.Color_Interactables(true, false, HighlightType.Both); //restore the color
                                //customRichEdit1.Set_TouchUI(true); //restore touchUI state
                                SaveTemplateToITRDB(null);
                            }
                            else
                                SaveTemplateToITRDB(null);
                            isRejecting = false;
                        }
                    }
                }

                //this.Close();
                customRichEdit1.Modified = false;
                Update_Status?.Invoke(this);
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
        }

        /// <summary>
        /// Update iTR status and inform iTR browser on the changes
        /// </summary>
        private void ChangeStatusWithUpdateOverriding(Guid iTRGuid, Guid iTRStatusGuid, iTRStatusChange statusChange, string statusComments)
        {
            switch (statusChange)
            {
                case iTRStatusChange.Increase:
                    _daITRStatus.ChangeStatus(iTRGuid, ref _ITRStatus, true, iTRStatusGuid);
                    _daITRStatusIssue.AddComments(iTRStatusGuid, statusComments, false);
                    _iTRBrowserUpdate = iTRBrowser_Update.Progressed; //inform iTR browser to check for new status
                    break;
                case iTRStatusChange.Decrease:
                    _daITRStatus.ChangeStatus(iTRGuid, ref _ITRStatus, false, iTRStatusGuid);
                    _daITRStatusIssue.AddComments(iTRStatusGuid, statusComments, true);
                    _iTRBrowserUpdate = iTRBrowser_Update.Progressed; //inform iTR browser to check for new status
                    break;
                case iTRStatusChange.New:
                    _daITRStatus.ChangeStatus(iTRGuid, ref _ITRStatus, true, Guid.NewGuid());
                    _iTRBrowserUpdate = iTRBrowser_Update.Progressed;
                    //don't need to inform iTR browser to check for new status because it'll be check for saving from template to iTR
                    break;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (btnProgress.Text != "Submit" && !System_Environment.HasPrivilege(PrivilegeTypeID.DeleteITR))
            {
                Common.Warn("You are not authorised to delete an ITR");
                return;
            }

            _ParallelWBSTagItems = customRichEdit1.Get_Parallel_ItemsRevised(_iTRGuid, SelfWBSTagGUID, _tag == null);

            //hereby populate ITR_GUID in the parallel items
            dsITR_MAIN.ITR_MAINDataTable dtParallelITR = Common.RevisedGetITRBy(_ParallelWBSTagItems);

            if (_ParallelWBSTagItems.Count > 0 && dtParallelITR != null)
            {
                if (Common.Confirmation("Are you sure you want to delete this ITR?", "Delete ITR"))
                {
                    _dtITR_Deleted.Merge(dtParallelITR);
                    foreach (dsITR_MAIN.ITR_MAINRow drITR in dtParallelITR.Rows)
                    {
                        if (drITR.GUID != _primaryITRGuid)
                            continue;

                        _daITR.RemoveBy(drITR.GUID);

                        //remove punchlist tagged to this iTR
                        using (AdapterPUNCHLIST_MAIN daPunchlist = new AdapterPUNCHLIST_MAIN())
                        {
                            Guid wbsTagGuid = drITR.IsTAG_GUIDNull() ? drITR.WBS_GUID : drITR.TAG_GUID;
                            //if (_tag != null)
                            //    wbsTagGuid = _tag.GUID;
                            //else if (_wbs != null)
                            //    wbsTagGuid = _wbs.GUID;

                            dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPunchlist = daPunchlist.GetByWBSTagITR(wbsTagGuid, _primaryITRGuid);
                            if (dtPunchlist != null)
                            {
                                foreach (dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlist in dtPunchlist.Rows)
                                {
                                    if (drPunchlist.ITR_GUID != Guid.Empty) //if punchlist item is not an ad-hoc punchlist
                                    {
                                        daPunchlist.RemoveBy(drPunchlist.GUID);
                                    }
                                }
                            }
                        }

                        _iTRBrowserUpdate = iTRBrowser_Update.Deleted; //need to inform the browser that this iTR has been deleted
                        customRichEdit1.Modified = false;

                        Update_Status?.Invoke(this);
                        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    }
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            RichEditCommand command = customRichEdit1.CreateCommand(RichEditCommandId.PrintPreview);
            command.Execute();
        }

        private void btnExportToPDF_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.RootFolder = Environment.SpecialFolder.Desktop;
            string outputFileName = this.Text.Replace("/", "-");
            outputFileName = outputFileName.Replace("\\", "");
            outputFileName = outputFileName.Replace("|", "");

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //customRichEdit1.RemoveAllRangePermissions(); //in DevExpress 15.2 range permission's color are exported to PDF even though it's not displayed, removing it will solve the issue but the signature permission will be removed as well
                customRichEdit1.ExportToPdf(fd.SelectedPath + "\\" + outputFileName + ".pdf");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            onExit();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void customRichEdit1_Click(object sender, EventArgs e)
        {
            //splashScreenManager1.ShowWaitForm();
            //if (customRichEdit1.RichEditClick_Interactions(true))
            //    btnSave.Enabled = true;
            customRichEdit1.RichEditClick_Interactions(true, _onlyExecuteSupervisorInteraction);
            if (customRichEdit1._dtDeletedITRs.Rows.Count > 0)
                _dtITR_Deleted.Merge(customRichEdit1._dtDeletedITRs);
            //if (_colorOnClick)
            //    customRichEdit1.ColorInteractables(true, true, HighlightType.Interactables);

            //btnProgress.Tag = CheckInteractables();
            //splashScreenManager1.CloseWaitForm();
        }

        private void customRichEdit1_InvalidClick(object sender, EventArgs e)
        {
            Common.Warn("You do not have authorisation to inspect an ITR");
        }

        private Guid SelfWBSTagGUID
        {
            get
            {
                return _tag == null ? _wbs.GUID : _tag.GUID;
            }
        }

        private bool checkPermission()
        {
            //authorisation
            bool ITR_Exists = false;
            _ITRStatus = getITRStatus(out ITR_Exists);
            if(!ITR_Exists)
            {
                if (!System_Environment.HasPrivilege(PrivilegeTypeID.MarkITRInspected))
                {
                    Common.Warn("You are not authorised to inspect an ITR");
                    return false;
                }
            }
            else
            {
                if (_ITRStatus == ITR_Status.Inspected)
                {
                    if (_template.templateSkipApproved)
                    {
                        if (!System_Environment.HasPrivilege(PrivilegeTypeID.MarkITRCompleted))
                        {
                            Common.Warn("You are not authorised to complete an ITR");
                            return false;
                        }
                    }
                    else
                    {
                        if (!System_Environment.HasPrivilege(PrivilegeTypeID.MarkITRApproved))
                        {
                            Common.Warn("You are not authorised to approve an ITR");
                            return false;
                        }
                    }
                }
                else if (_ITRStatus == ITR_Status.Approved)
                {
                    if (!System_Environment.HasPrivilege(PrivilegeTypeID.MarkITRCompleted))
                    {
                        Common.Warn("You are not authorised to complete an ITR");
                        return false;
                    }
                }
                else if (_ITRStatus == ITR_Status.Completed)
                {
                    if (!System_Environment.HasPrivilege(PrivilegeTypeID.MarkITRClosed))
                    {
                        Common.Warn("You are not authorised to close an ITR");
                        return false;
                    }
                }
            }

            return true;
        }

        public bool AttachPDF(MemoryStream ms)
        {
            List<Image> images = new List<Image>();
            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
            {
                bool isError = false;
                documentProcessor.LoadDocument(ms);
                for (int i = 1; i <= documentProcessor.Document.Pages.Count; i++)
                {
                    Bitmap pageBitmap = documentProcessor.CreateBitmap(i, 2000);
                    string metaString = Common.GetDocumentMetaQRCodeTryHarder(pageBitmap);
                    if (metaString == string.Empty)
                    {
                        isError = true;
                        break;
                    }

                    string[] metaArray = metaString.Split(';');
                    if (metaArray.Count() < 2)
                    {
                        isError = true;
                        break;
                    }
                    else
                    {
                        string[] thisMeta = getMetaTag().Split(';');
                        string this_document_name = thisMeta[2];
                        string this_tag_number = thisMeta[1];

                        string document_name = metaArray[2];
                        string tag_number = metaArray[1];

                        if (this_document_name != document_name || this_tag_number != tag_number)
                        {
                            isError = true;
                            break;
                        }
                    }

                    images.Add(pageBitmap);
                }

                if (isError)
                    return false;

                ReplaceDocumentWithImages(images, true);
                return true;
            }
        }

        public bool AttachPDF()
        {
            OpenFileDialog thisDialog = new OpenFileDialog();

            thisDialog.Filter = "PDF (*.PDF)|*.PDF";
            thisDialog.FilterIndex = 1;
            thisDialog.RestoreDirectory = true;
            thisDialog.Multiselect = false;
            thisDialog.Title = "Please Select PDF";

            if (thisDialog.ShowDialog() == DialogResult.OK)
            {
                List<Image> images = new List<Image>();
                using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
                {
                    bool isError = false;
                    documentProcessor.LoadDocument(thisDialog.FileName);
                    for (int i = 1; i <= documentProcessor.Document.Pages.Count; i++)
                    {
                        Bitmap pageBitmap = documentProcessor.CreateBitmap(i, 2000);
                        string metaString = Common.GetDocumentMetaQRCodeTryHarder(pageBitmap);
                        if (metaString == string.Empty)
                        {
                            isError = true;
                            break;
                        }

                        string[] metaArray = metaString.Split(';');
                        if (metaArray.Count() < 2)
                        {
                            isError = true;
                            break;
                        }
                        else
                        {
                            string[] thisMeta = getMetaTag().Split(';');
                            string this_document_name = thisMeta[2];
                            string this_tag_number = thisMeta[1];

                            string document_name = metaArray[2];
                            string tag_number = metaArray[1];

                            if (this_document_name != document_name || this_tag_number != tag_number)
                            {
                                isError = true;
                                break;
                            }
                        }

                        images.Add(pageBitmap);
                    }

                    if (isError)
                        return false;

                    ReplaceDocumentWithImages(images, true);
                    return true;
                }
            }

            return false;
        }

        private string getMetaTag()
        {
            if (customRichEdit1.Options.MailMerge.DataSource == null)
                return string.Empty;

            if (((DataTable)customRichEdit1.Options.MailMerge.DataSource).Rows.Count == 0)
                return string.Empty;

            DataRow dataRow = ((DataTable)customRichEdit1.Options.MailMerge.DataSource).Rows[0];
            string project_number = dataRow[Variables.prefillProjNumber].ToString().ToUpper();
            string tag_number = dataRow[Variables.prefillTagNumber].ToString().ToUpper();
            string document_name = dataRow[Variables.prefillDocumentName].ToString().ToUpper();
            bool ITR_Exists;
            ITR_Status? ITR_Status = getITRStatus(out ITR_Exists);
            string document_status = ITR_Status == null || !ITR_Exists ? string.Empty : ITR_Status.ToString();

            return project_number + ";" + tag_number + ";" + document_name + ";" + document_status;
        }

        public void AppendPDF()
        {
            OpenFileDialog thisDialog = new OpenFileDialog();

            thisDialog.Filter = "PDF (*.PDF)|*.PDF";
            thisDialog.FilterIndex = 1;
            thisDialog.RestoreDirectory = true;
            thisDialog.Multiselect = true;
            thisDialog.Title = "Please Select PDF";
            if (thisDialog.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo("Temp");
                if (Directory.Exists(directoryInfo.FullName))
                {
                    foreach (FileInfo file in directoryInfo.GetFiles())
                    {
                        file.Delete();
                    }
                }
                else
                    directoryInfo = System.IO.Directory.CreateDirectory("Temp");


                //splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
                //splashScreenManager1.ShowWaitForm();
                //splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetLargeFormat, 1000);

                foreach (string filename in thisDialog.FileNames)
                {
                    List<Image> images = new List<Image>();
                    using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
                    {
                        documentProcessor.LoadDocument(filename);
                        List<string> splitFileName = filename.Split('\\').ToList();
                        //splashScreenManager1.SetWaitFormCaption("Converting PDF to image from " + splitFileName.Last() + " ...");
                        //splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, documentProcessor.Document.Pages.Count);

                        for (int i = 1; i <= documentProcessor.Document.Pages.Count; i++)
                        {
                            Bitmap pageBitmap = documentProcessor.CreateBitmap(i, 2000);
                            images.Add(pageBitmap);
                            //splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                        }
                        AppendImagesToDocument(images);
                    }
                }

                //splashScreenManager1.CloseWaitForm();
                MessageBox.Show("Additional PDF is appended to this document as image");
            }
        }

        public bool AttachImage()
        {
            if (!checkPermission())
                return false;

            using (frmITR_AttachImage frmAttachImage = new frmITR_AttachImage())
            {
                if (frmAttachImage.ShowDialog() == DialogResult.OK)
                {
                    SliderImageCollection imageCollection = frmAttachImage.GetImages();
                    if (imageCollection.Count > 0)
                    {
                        List<Image> images = new List<Image>();
                        foreach (Image image in imageCollection)
                        {
                            images.Add(image);
                        }

                        AppendImagesToDocument(images);
                    }
                }
            }

            return true;
        }

        private bool isITRAttachment()
        {
            Section mainSection = customRichEdit1.Document.Sections[0];
            return mainSection.Margins.Top == 0;
        }

        private bool isNativeSavedDocument()
        {
            dsITR_MAIN.ITR_MAINRow drITR = getITR();
            if (drITR != null)
            {
                dsITR_STATUS.ITR_STATUSRow drITRLatestStatus = _daITRStatus.GetRowBy(drITR.GUID);
                dsITR_STATUS.ITR_STATUSDataTable dtITRStatus = _daITRStatus.GetAllExcludeRejected(drITR.GUID);
                if (dtITRStatus == null)
                    return true;
            }

            return false;
        }

        private bool replaceSignatureBlock()
        {
            bool isFirstAttachment = false;
            dsITR_MAIN.ITR_MAINRow drITR = getITR();
            if (drITR != null)
            {
                dsITR_STATUS.ITR_STATUSRow drITRLatestStatus = _daITRStatus.GetRowBy(drITR.GUID);
                dsITR_STATUS.ITR_STATUSDataTable dtITRStatus = _daITRStatus.GetAllExcludeRejected(drITR.GUID);
                Image signature_block;
                if (drITR != null && drITRLatestStatus != null && drITRLatestStatus.STATUS_NUMBER >= 0)
                {
                    List<SignatureUser> signature_users = SignatureUserHelper.GetSignatureUser(dtITRStatus, (ITR_Status)drITRLatestStatus.STATUS_NUMBER);
                    bool isElectrical = drITR.DISCIPLINE == Common.Replace_WithSpaces(Discipline.Electrical.ToString()) || drITR.DISCIPLINE == Common.Replace_WithSpaces(Discipline.Instrumentation.ToString());
                    signature_block = SignatureBlockUtility.GetSignatureBlock(signature_users, isElectrical);
                }
                else
                {
                    isFirstAttachment = true;
                    signature_block = SignatureBlockUtility.GetSignatureBlock();
                }

                Section mainSection = customRichEdit1.Document.Sections[0];
                customRichEdit1.Document.Unit = DocumentUnit.Document;
                //minus 100 to compensate for margin
                float pageWidth = mainSection.Page.Width - (mainSection.Page.Width * 0.1f);
                float pageHeight = mainSection.Page.Height;
                float imageWidth = Units.DocumentsToPixelsF(pageWidth, customRichEdit1.DpiX);
                float imageHeight = Units.DocumentsToPixelsF(pageHeight, customRichEdit1.DpiY);
                double ratio;
                signature_block = Common.ScaleImage(signature_block, (int)imageWidth, (int)imageHeight, out ratio);

                Bitmap lastPageFullBitmap = getLastPageFullBitmap();
                //lastPageFullBitmap.Save("LastPage.bmp");
                //var luminanceSource = new ZXing.BitmapLuminanceSource(qrCodeBitmapPage);
                //var binarizer = new ZXing.Common.HybridBinarizer(luminanceSource);
                //var bitMatrix = binarizer.BlackMatrix;

                //var whiterect = WhiteRectangleDetector.Create(bitMatrix);

                //ResultPoint[] whiterectpts = whiterect.detect();
                ResultPoint[] qrCodeResultPoint = getSignatureQRCodePosition();
                if (qrCodeResultPoint != null)
                {
                    //int blockWidth = (int)(whiterectpts[2].X - whiterectpts[1].X);
                    //int blockHeight = (int)(whiterectpts[1].Y - whiterectpts[0].Y);

                    //Image resized_signature_block = ScaleImage(signature_block, blockWidth, blockHeight);

                    //Bitmap debugBitmap = new Bitmap(signature_block);
                    //debugBitmap.Save("debug.bmp");

                    ResultPoint insertPoint = qrCodeResultPoint[1];
                    ResultPoint normalized_insertPoint = new ResultPoint(insertPoint.X, insertPoint.Y + Variables.RichEditHeaderOffset);
                    //Rectangle whiterectangle = new Rectangle((int)insertPoint.X, (int)insertPoint.Y, blockWidth, blockHeight);
                    Graphics g = Graphics.FromImage(lastPageFullBitmap);

                    SolidBrush brush = new SolidBrush(Color.White);

                    PointF startPoint;
                    if (isFirstAttachment)
                        startPoint = new PointF(normalized_insertPoint.X - (31f * (1 + (float)ratio)), normalized_insertPoint.Y - (18f * (1 + (float)ratio)));
                    else
                        startPoint = new PointF(normalized_insertPoint.X - (26f * (1 + (float)ratio)), normalized_insertPoint.Y - (15f * (1 + (float)ratio)));

                    g.FillRectangle(brush, new Rectangle((int)(normalized_insertPoint.X - 1000f), (int)(normalized_insertPoint.Y - (30f * (1 + (float)ratio))), 5000, signature_block.Height + 30));
                    g.DrawImage(signature_block, startPoint);
                    Image QRCode = BarCodeUtility.GetQRCode("SIGNATURE");
                    g.DrawImage(QRCode, startPoint);

                    //lastPageFullBitmap.Save("Export.bmp");
                    ReadOnlyDocumentImageCollection lastPageImages = getLastRowCellImages();
                    if (lastPageImages.Count > 0)
                    {
                        //remove all images from last page
                        foreach (var image in lastPageImages)
                        {
                            customRichEdit1.Document.Delete(image.Range);
                        }

                        customRichEdit1.Document.Images.Insert(getLastRowTableCell().ContentRange.Start, lastPageFullBitmap);
                        return true;
                    }
                }
            }

            return false;
        }

        //private string getMetaQRCodeValue(Bitmap bitmap)
        //{
        //    double ratio;
        //    Bitmap resizeImage = (Bitmap)Common.ScaleImage(bitmap, 5000, 5000, out ratio);
        //    Bitmap cellCropBitmap = Common.CropBitmap(resizeImage, 0, resizeImage.Height - Variables.RichEditHeaderOffset, resizeImage.Width, Variables.RichEditHeaderOffset);

        //    //cellCropBitmap.Save("MetaQRCode.bmp");
        //    BarcodeReader reader = new BarcodeReader();
        //    reader.AutoRotate = true;
        //    reader.Options.TryHarder = true;
        //    Result result = reader.Decode(cellCropBitmap);
        //    if (result != null)
        //    {
        //        cellCropBitmap.Dispose();
        //        resizeImage.Dispose();
        //        return result.ToString();
        //    }

        //    cellCropBitmap.Dispose();
        //    resizeImage.Dispose();
        //    return string.Empty;
        //}

        private ResultPoint[] getSignatureQRCodePosition()
        {
            Bitmap cellBitmap = getLastPageSignatureBitmap();
            if (cellBitmap != null)
            {
                //cellBitmap.Save("SignatureQRCodePosition.bmp");
                BarcodeReader reader = new BarcodeReader();
                reader.AutoRotate = true;
                reader.Options.TryHarder = true;
                Result result = reader.Decode(cellBitmap);
                if (result != null)
                {
                    ResultPoint[] points = result.ResultPoints;
                    cellBitmap.Dispose();
                    return points;
                }

                cellBitmap.Dispose();
            }

            return null;
        }

        private Bitmap getLastPageFullBitmap()
        {
            ReadOnlyDocumentImageCollection images = getLastRowCellImages();
            foreach (var cellImage in images)
            {
                if (cellImage.Size.Width > Variables.QRCodeMaxWidth)
                {
                    Image cellNativeImage = cellImage.Image.NativeImage;
                    Bitmap cellBitmap = new Bitmap(cellNativeImage);
                    //crop bitmap to avoid retrieving meta qrcode

                    Bitmap cellCropBitmap = Common.CropBitmap(cellBitmap, 0, 0, cellBitmap.Width, cellBitmap.Height);
                    return cellCropBitmap;
                }
            }

            return null;
        }

        private Bitmap getLastPageSignatureBitmap()
        {
            ReadOnlyDocumentImageCollection images = getLastRowCellImages();
            foreach (var cellImage in images)
            {
                if (cellImage.Size.Width > Variables.QRCodeMaxWidth)
                {
                    Image cellNativeImage = cellImage.Image.NativeImage;
                    Bitmap cellBitmap = new Bitmap(cellNativeImage);
                    //crop bitmap to avoid retrieving meta qrcode

                    Bitmap cellCropBitmap = Common.CropBitmap(cellBitmap, 0, Variables.RichEditHeaderOffset, cellBitmap.Width, cellBitmap.Height - Variables.RichEditHeaderOffset);
                    //cellCropBitmap.Save("DEBUGTEST.bmp");
                    return cellCropBitmap;
                }
            }

            return null;
        }

        private Bitmap getLastScannedPageMetaBitmap()
        {
            ReadOnlyDocumentImageCollection images = getLastRowCellImages();
            foreach (var cellImage in images)
            {
                Image cellNativeImage = cellImage.Image.NativeImage;
                Bitmap cellBitmap = new Bitmap(cellNativeImage);
                return cellBitmap;
            }

            return null;
        }

        private ReadOnlyDocumentImageCollection getLastRowCellImages()
        {
            TableCell lastRowTableCell = getLastRowTableCell();
            return customRichEdit1.Document.Images.Get(lastRowTableCell.ContentRange);
        }

        private TableCell getLastRowTableCell()
        {
            Table table = customRichEdit1.Document.Tables[0];
            //table.ForEachCell(new TableCellProcessorDelegate(ocrCell));
            return table.Cell(table.Rows.Count - 1, 0);
        }

        //private void ocrCell(TableCell cell, int i, int j)
        //{
        //    ReadOnlyDocumentImageCollection docImages = customRichEdit1.Document.Images.Get(cell.Range);
        //    string output = string.Empty;

        //    Dictionary<string, string> dict = new Dictionary<string, string>();
        //    dict.Add(AspriseOCR.PROP_OUTPUT_SEPARATE_WORDS, "false");
        //    dict.Add(AspriseOCR.PROP_PAGE_TYPE, "auto");
        //    dict.Add(AspriseOCR.PROP_TABLE_SKIP_DETECTION, "true");
        //    dict.Add(AspriseOCR.PROP_IMG_PREPROCESS_TYPE, AspriseOCR.PROP_IMG_PREPROCESS_TYPE_DEFAULT);
        //    AspriseOCR.SetUp();
        //    AspriseOCR aspriseOCR = new AspriseOCR();
        //    aspriseOCR.StartEngine("eng", AspriseOCR.SPEED_FASTEST);

        //    if (docImages.Count == 1)
        //    {
        //        Image cellImage = docImages[0].Image.NativeImage;
        //        Bitmap cellBitmap = new Bitmap(cellImage);

        //        string tempFilePath = Directory.GetCurrentDirectory() + "\\temp.bmp";
        //        cellBitmap.Save(tempFilePath);

        //        output = aspriseOCR.Recognize(tempFilePath, -1, -1, -1, -1, -1, AspriseOCR.RECOGNIZE_TYPE_TEXT, AspriseOCR.OUTPUT_FORMAT_XML, AspriseOCR.dictToString(dict) + AspriseOCR.CONFIG_PROP_SEPARATOR);
        //        aspriseOCR.StopEngine();
        //        File.Delete(tempFilePath);
        //    }

        //    string s = output.ToString();
        //}

        private void silentSave()
        {
            _ParallelWBSTagItems = customRichEdit1.Get_Parallel_ItemsRevised(_iTRGuid, SelfWBSTagGUID, _tag == null);
            //need to refresh the parallel items ITR_GUID
            dsITR_MAIN.ITR_MAINDataTable dtParallelItems = Common.RevisedGetITRBy(_ParallelWBSTagItems);
            //hereby populate ITR_GUID in the parallel items

            string Comment = "manual";
            foreach (ITR_Refresh_Item ParallelWBSTagItem in _ParallelWBSTagItems)
            {
                if (ParallelWBSTagItem.ITR_GUID != Guid.Empty)
                {
                    Guid iTRStatusGuid = Guid.NewGuid();
                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, iTRStatusGuid, iTRStatusChange.Increase, Comment);
                    //Common.Prompt("ITR Successfully " + strProgress);
                    _iTRBrowserUpdate = iTRBrowser_Update.Progressed;
                    if (btnProgress.Text == "Complete")
                        ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, iTRStatusGuid, iTRStatusChange.Increase, Comment);
                    else if (btnProgress.Text == "Close")
                    {
                        customRichEdit1.Trim_Redundant_Rows();
                        customRichEdit1.ApplyPageBreaks();
                    }

                    SaveTemplateToITRDB(ParallelWBSTagItem); //need to save no matter, so save confirmation won't pop up and screw up the browser
                }
                else //progress inexistant ITR
                {
                    SaveTemplateToITRDB(null);
                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.New, Comment);
                    _iTRBrowserUpdate = iTRBrowser_Update.Saved;
                }
            }

            Update_Status?.Invoke(this);
        }

        bool _isTesting;
        public void TestProgress()
        {
            _isTesting = true;
            btnProgress_Click(null, null);
            onExit();
        }

        bool isProgressing = false;
        private void btnProgress_Click(object sender, EventArgs e)
        {
            bool userIsTester = false;

            if (isProgressing)
                return;

            if (System_Environment.HasPrivilege(PrivilegeTypeID.UserIsTester))
            {
                userIsTester = true;
            }

            if (!userIsTester && !checkPermission())
                return;

            if (!userIsTester)
            {
                Progress_Restriction restriction = customRichEdit1.CheckInteractables();
                //if (btnProgress.Tag != null && (Progress_Restriction)btnProgress.Tag != Progress_Restriction.None)
                if (restriction != Progress_Restriction.None)
                {
                    if (restriction == Progress_Restriction.Acceptance)
                        Common.Warn("Please complete all 'Click Here' toggle before submitting");
                    //else if (restriction == Progress_Restriction.Punchlist) //punchlist is validated separatly now
                    //    Common.Warn("You've marked items as punchlisted, please create a punchlist before submitting");

                    return;
                }
            }

            _ParallelWBSTagItems = customRichEdit1.Get_Parallel_ItemsRevised(_iTRGuid, SelfWBSTagGUID, _tag == null);
            //need to refresh the parallel items ITR_GUID
            dsITR_MAIN.ITR_MAINDataTable dtParallelItems = Common.RevisedGetITRBy(_ParallelWBSTagItems);
            //hereby populate ITR_GUID in the parallel items

            string Comment = null;
            bool? ConfirmationResult = null;

            foreach (ITR_Refresh_Item ParallelWBSTagItem in _ParallelWBSTagItems)
            {
                if (CheckITRPunchlistItems())
                {
                    if (btnProgress.Text != "Submit")
                    {
                        if (!CheckITRPunchlistItemPriority())
                        {
                            Common.Warn("Existing punchlist with priority\n" + Variables.punchlistCategoryA + "\n unapproved, Please approve them before proceeding");
                            return;
                        }
                    }
                }
                else
                {
                    RequirePunchlist_Warning(_ParallelWBSTagItems);
                    return;
                }


                if (_imageIndex == 5 || _imageIndex == 7) //need to ask for comments when progressing rejected ITRs
                {
                    if (Comment == null)
                    {
                        frmITR_Comment f = new frmITR_Comment(ParallelWBSTagItem.ITR_GUID, btnProgress.Text);
                        if (f.ShowDialog() == DialogResult.OK)
                        {
                            Comment = f.GetComment();
                            ConfirmationResult = true;
                        }
                        else
                            return;
                    }
                }

                if (ParallelWBSTagItem.ITR_GUID != Guid.Empty)
                {
                    if (Comment != null)
                    {
                        isProgressing = true;
                        Guid iTRStatusGuid = Guid.NewGuid();
                        ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, iTRStatusGuid, iTRStatusChange.Increase, Comment);
                        //Common.Prompt("ITR Successfully " + strProgress);
                        _iTRBrowserUpdate = iTRBrowser_Update.Progressed;
                        if (btnProgress.Text == "Approve")
                        {
                            customRichEdit1.Color_Interactables(false, false, HighlightType.Both); //take out color during approval
                            //customRichEdit1.Set_TouchUI(false); //Restore the cell size
                        }
                        //Straight to complete Release 1.2.0.12
                        else if (btnProgress.Text == "Complete")
                        {
                            customRichEdit1.Color_Interactables(false, false, HighlightType.Both); //take out color during approval
                            if (_ITRStatus == ITR_Status.Inspected && _template.templateSkipApproved)
                                ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, Comment);
                        }
                        else if (btnProgress.Text == "Close")
                        {
                            bool isAttachment = isITRAttachment();
                            customRichEdit1.Trim_Redundant_Rows();
                            customRichEdit1.ApplyPageBreaks();

                            if (isAttachment)
                            {
                                if (_ITRStatus == null)
                                {
                                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, Comment);
                                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, Comment);
                                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, Comment);
                                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, Comment);
                                }
                                else if (_ITRStatus == ITR_Status.Inspected)
                                {
                                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, Comment);
                                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, Comment);
                                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, Comment);
                                }
                                else if (_ITRStatus == ITR_Status.Approved)
                                {
                                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, Comment);
                                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, Comment);
                                }
                                else if (_ITRStatus == ITR_Status.Completed)
                                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, Comment);
                            }
                        }

                        SaveTemplateToITRDB(ParallelWBSTagItem); //need to save no matter, so save confirmation won't pop up and screw up the browser
                        isProgressing = false;
                    }
                    else //progress saved ITR
                    {
                        if (_isTesting || (ConfirmationResult != null && ConfirmationResult == true) || Common.Confirmation("Are you sure you want to " + btnProgress.Text.ToLower().Replace("&", "") + " this ITR?", "Submit ITR"))
                        {
                            isProgressing = true;
                            ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, string.Empty);

                            ConfirmationResult = true;
                            if (btnProgress.Text == "Submit")
                            {
                                SaveTemplateToITRDB(ParallelWBSTagItem);
                            }
                            else if (btnProgress.Text == "Approve")
                            {
                                //customRichEdit1.Color_Interactables(false, false, HighlightType.Both); //take out color during approval
                                //customRichEdit1.Set_TouchUI(false); //Restore the cell size
                                SaveTemplateToITRDB(ParallelWBSTagItem);
                            }
                            else if (btnProgress.Text == "Complete")
                            {
                                //Straight to complete Release 1.2.0.12
                                customRichEdit1.Color_Interactables(false, false, HighlightType.Both); //take out color during approval

                                if (_ITRStatus == ITR_Status.Approved)
                                    ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, string.Empty);

                                SaveTemplateToITRDB(ParallelWBSTagItem);
                            }
                            else if (btnProgress.Text == "Close")
                            {
                                customRichEdit1.Trim_Redundant_Rows();
                                customRichEdit1.ApplyPageBreaks();
                                SaveTemplateToITRDB(ParallelWBSTagItem);

                                bool isAttachment = isITRAttachment();
                                if (isAttachment)
                                {
                                    if (_ITRStatus == null)
                                    {
                                        ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, string.Empty);
                                        ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, string.Empty);
                                        ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, string.Empty);
                                    }
                                    else if (_ITRStatus == ITR_Status.Inspected)
                                    {
                                        ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, string.Empty);
                                        ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, string.Empty);
                                    }
                                    else if (_ITRStatus == ITR_Status.Approved)
                                    {
                                        ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, string.Empty);
                                    }
                                }
                            }

                            isProgressing = false;
                        }
                        else
                        {
                            ConfirmationResult = false;
                            return;
                        }
                    }
                }
                else //progress inexistant ITR
                {
                    if (_isTesting || (ConfirmationResult != null && ConfirmationResult == true) || Common.Confirmation("Are you sure you want to " + btnProgress.Text.ToLower() + " this ITR?", "Submit ITR"))
                    {
                        isProgressing = true;
                        SaveTemplateToITRDB(null);
                        ConfirmationResult = true;

                        if (CheckITRPunchlistItems())
                        {
                            if (Comment != null)
                                ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, Comment);
                            else
                                ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.New, string.Empty);
                        }
                        else
                            RequirePunchlist_Warning(_ParallelWBSTagItems);

                        //Commit into database to generate the _primaryITRGuid before punchlist validation
                        if (btnProgress.Text == "Submit")
                            SaveTemplateToITRDB(ParallelWBSTagItem);
                        else if (btnProgress.Text == "Close")
                        {
                            string saveComment = Comment == null ? string.Empty : Comment;
                            ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, saveComment);
                            ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, saveComment);
                            ChangeStatusWithUpdateOverriding(ParallelWBSTagItem.ITR_GUID, Guid.NewGuid(), iTRStatusChange.Increase, saveComment);
                            _iTRBrowserUpdate = iTRBrowser_Update.SavedClose;
                        }
                        isProgressing = false;
                    }
                    else
                    {
                        ConfirmationResult = false;
                        return;
                    }
                }
            }

            this.Close();
            //update status is invoked in closing
            //Update_Status?.Invoke(this);
        }

        /// <summary>
        /// Checks whether the user have punchlist creation priviledge and action appropriately
        /// </summary>
        private void RequirePunchlist_Warning(List<ITR_Refresh_Item> ParallelWBSTagItems)
        {
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.CreatePunchlist))
            {
                Common.Warn("You are not authorised to create a punchlist\nYou may only reject this ITR with the new changes that you've made");
                return;
            }
            else
            {
                Common.Warn("Punchlist required to " + btnProgress.Text.ToLower() + " this ITR");
                AddPunchlist(true, ParallelWBSTagItems);
            }
        }

        /// <summary>
        /// Checks whether priority punchlist has been approved
        /// </summary>
        private bool CheckITRPunchlistItemPriority()
        {
            //Bypass punchlist validation if parallel processing is opted
            if (_ParallelWBSTagItems.Count > 1)
                return true;

            using (AdapterPUNCHLIST_MAIN daPUNCHLIST = new AdapterPUNCHLIST_MAIN())
            {
                dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST = daPUNCHLIST.CheckUnapprovedPriorityPunchlist(_primaryITRGuid, Variables.punchlistCategoryA, (int)Punchlist_Status.Approved);
                if (dtPUNCHLIST != null)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check ITR Punchlist Items (This needs to come after ITR has been saved to make sure _iTRGuid is populated
        /// </summary>
        private bool CheckITRPunchlistItems()
        {
            //Bypass punchlist validation if parallel processing is opted
            if (_ParallelWBSTagItems.Count > 1)
                return true;

            using (AdapterPUNCHLIST_MAIN daPunchlist = new AdapterPUNCHLIST_MAIN())
            {
                RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
                customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

                Guid wbsTagGuid = Guid.Empty;
                if (_tag != null)
                    wbsTagGuid = _tag.GUID;
                else
                    wbsTagGuid = _wbs.GUID;


                dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPunchlist = daPunchlist.GetByWBSTagITR(wbsTagGuid, _primaryITRGuid);
                bool AllowProgression = true;
                //Checking toggle text against punchlist to determine whether ITR has all the punchlisted item done
                foreach (RangePermission rangePermission in rangePermissions)
                {
                    if (rangePermission.Group == CustomUserGroupListService.TOGGLE_ACCEPTANCE)
                    {
                        string toggleText = customRichEdit1.Document.GetText(rangePermission.Range);
                        if (toggleText.Contains(Toggle_Acceptance.Punchlisted.ToString() + Variables.punchlistAffix))
                        {
                            AllowProgression = false; //mark false for every entry if there is a punchlisted item tagged
                            if (dtPunchlist != null)
                            {
                                string punchlistName = toggleText.Split(new string[] { Variables.punchlistStatusDelimiter }, StringSplitOptions.None).First();
                                dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlist = dtPunchlist.FirstOrDefault(obj => obj.ITR_PUNCHLIST_ITEM == punchlistName);
                                if (drPunchlist != null) //if punchlist item for this iTR is not found disallow progression
                                    AllowProgression = true;  //there are punchlisted item tagged within the document but and punchlist is found
                            }
                        }
                    }
                }

                if (AllowProgression) //only delete rogue punchlist if iTR is approved for submission
                {
                    if (dtPunchlist != null)
                    {
                        //Checking punchlist against toggle text to determine whether punchlist should be deleted
                        foreach (dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlist in dtPunchlist.Rows)
                        {
                            if (drPunchlist.ITR_GUID != Guid.Empty) //do not remove ad-hoc punchlist items
                            {
                                RangePermission findPunchlistItem = rangePermissions.FirstOrDefault(obj => obj.Group == CustomUserGroupListService.TOGGLE_ACCEPTANCE
                                                                                                    && customRichEdit1.Document.GetText(obj.Range).Split(new string[] { Variables.punchlistStatusDelimiter }, StringSplitOptions.None).First() == drPunchlist.ITR_PUNCHLIST_ITEM);
                                if (findPunchlistItem == null) //if punchlist item is not found in the document, proceed with deletion
                                {
                                    if (Common.Confirmation("Punchlist item is not longer valid\nDo you wish to delete it?\n\nTitle: " + drPunchlist.TITLE + "\nDescription: " + drPunchlist.DESCRIPTION, "Invalid Punchlist"))
                                        daPunchlist.RemoveBy(drPunchlist.GUID);
                                }
                            }
                        }
                    }
                }
                else
                    return false;

                return AllowProgression;
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Deprecated - Create the WBSTag display in context
        /// </summary>
        private wbsTagDisplay CreateWBSTagDisplay(WBS wbs, Tag tag)
        {
            wbsTagDisplay iTRWBSTag;

            if (tag != null)
            {
                iTRWBSTag = new wbsTagDisplay(new Tag(tag.GUID)
                {
                    tagNumber = tag.tagNumber,
                    tagDescription = tag.tagDescription,
                    tagParentGuid = tag.tagParentGuid,
                    tagScheduleGuid = tag.tagScheduleGuid
                })
                {
                    wbsTagDisplayGuid = tag.GUID
                };
            }
            else
            {
                iTRWBSTag = new wbsTagDisplay(new WBS(wbs.GUID)
                {
                    wbsName = wbs.wbsName,
                    wbsDescription = wbs.wbsDescription,
                    wbsParentGuid = wbs.wbsParentGuid,
                    wbsScheduleGuid = wbs.wbsScheduleGuid
                })
                {
                    wbsTagDisplayGuid = wbs.GUID
                };
            }

            return iTRWBSTag;
        }

        /// <summary>
        /// Create the WBSTag display using database adapter
        /// </summary>
        private List<wbsTagDisplay> CreateParallel_WBSTagItem(List<ITR_Refresh_Item> ParallelWBSTagItems)
        {
            List<wbsTagDisplay> WBSTagList = Common.GetWBSTagBy(ParallelWBSTagItems);

            return WBSTagList;
        }

        public void AppendImagesToDocument(List<Image> images)
        {
            if (images.Count > 0)
            {
                DocumentPosition newSectionStartPos = customRichEdit1.Document.CreatePosition(customRichEdit1.Document.Length);
                Section mainSection = customRichEdit1.Document.Sections[0];
                customRichEdit1.Document.Unit = DocumentUnit.Document;
                mainSection.UnlinkHeaderFromNext();
                mainSection.UnlinkFooterFromNext();
                for (int i = 0; i < images.Count; i++)
                {
                    Image image = images[i];
                    int imageNativeWidth = image.Width;
                    int imageNativeHeight = image.Height;
                    bool isLandscape = imageNativeWidth > imageNativeHeight;
                    Section appendSection = customRichEdit1.Document.AppendSection();
                    appendSection.UnlinkHeaderFromPrevious();
                    appendSection.UnlinkFooterFromPrevious();
                    appendSection.Margins.Left = 0;
                    appendSection.Margins.Right = 0;
                    appendSection.Margins.Top = 0;
                    appendSection.Margins.Bottom = 0;
                    appendSection.Margins.HeaderOffset = 0;
                    appendSection.Margins.FooterOffset = 0;
                    appendSection.Page.Landscape = isLandscape;
                    SubDocument headerDoc = appendSection.BeginUpdateHeader();
                    headerDoc.Delete(headerDoc.Range);
                    //int lastParagraphIndex = headerDoc.Paragraphs.Count;
                    headerDoc.Paragraphs[0].LineSpacingType = ParagraphLineSpacing.Exactly;
                    headerDoc.Paragraphs[0].LineSpacingMultiplier = 1;
                    headerDoc.Paragraphs[0].Alignment = ParagraphAlignment.Right;
                    appendSection.EndUpdateHeader(headerDoc);

                    SubDocument footerDoc = appendSection.BeginUpdateFooter();
                    footerDoc.Delete(footerDoc.Range);
                    //lastParagraphIndex = footerDoc.Paragraphs.Count;
                    footerDoc.Paragraphs[0].LineSpacingType = ParagraphLineSpacing.Exactly;
                    footerDoc.Paragraphs[0].LineSpacingMultiplier = 1;
                    footerDoc.Paragraphs[0].Alignment = ParagraphAlignment.Right;
                    appendSection.EndUpdateHeader(footerDoc);

                    float pageWidth = isLandscape ? appendSection.Page.Height : appendSection.Page.Width;
                    float pageHeight = isLandscape ? appendSection.Page.Width : appendSection.Page.Height;
                    float imageWidth = Units.DocumentsToPixelsF(pageWidth, customRichEdit1.DpiX);
                    float imageHeight = Units.DocumentsToPixelsF(pageHeight, customRichEdit1.DpiY);
                    int intWidth = Convert.ToInt32(imageWidth);

                    int offSetHeight = isLandscape ? 20 : 16;
                    //leave some space at the bottom so blank page won't be created
                    int intHeight = Convert.ToInt32(imageHeight) - offSetHeight;
                    image = Common.ResizeImage(image, intWidth, intHeight);
                    //image = Common.ScaleImage(image, 500, 500);
                    //image = ResizeImage(image, 1020, 1320);

                    Table tblAttachment = customRichEdit1.Document.Tables.Create(appendSection.Range.Start, 1, 1, AutoFitBehaviorType.AutoFitToContents);
                    tblAttachment.TableAlignment = TableRowAlignment.Center;
                    customRichEdit1.InsertRangePermission(CustomUserGroupListService.USER, tblAttachment.Range);
                    customRichEdit1.Document.Images.Insert(appendSection.Range.Start, image);
                }

                customRichEdit1.Refresh();
                //MemoryStream ms = new MemoryStream();
                //customRichEdit1.SaveDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);

                //SaveTemplateToITRDB(null);
                ////bool isWBS = _tag == null ? true : false;
                ////AddNewITR(SelfWBSTagGUID, isWBS, ms);
                //////ChangeStatusWithUpdateOverriding(SelfWBSTagGUID, Guid.NewGuid(), iTRStatusChange.New, string.Empty);
                //_iTRBrowserUpdate = iTRBrowser_Update.Saved;
                ////Update_Status?.Invoke(this);
            }
        }

        public void ReplaceDocumentWithImages(List<Image> images, bool commitDocumentToDb)
        {
            if (images.Count > 0)
            {
                //this method clears documents and formats
                customRichEdit1.CreateNewDocument();
                //customRichEdit1.Document.Delete(customRichEdit1.Document.Range);
                customRichEdit1.Document.BeginUpdate();
                RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
                rangePermissions.Clear();
                customRichEdit1.Document.EndUpdateRangePermissions(rangePermissions);

                customRichEdit1._templateGuid = _template.GUID;
                customRichEdit1._wbsTagGuid = _tag == null ? _wbs.GUID : _tag.GUID;

                PaperKind paperKind = customRichEdit1.Document.Sections[0].Page.PaperKind;
                Section mainSection = customRichEdit1.Document.Sections[0];

                //customRichEdit1 = new CustomRichEdit();
                customRichEdit1.Document.Unit = DocumentUnit.Document;
                customRichEdit1.Document.Sections[0].Page.PaperKind = paperKind;
                mainSection = customRichEdit1.Document.Sections[0];
                SubDocument doc = mainSection.BeginUpdateHeader();
                int lastParagraphIndex = doc.Paragraphs.Count - 1;
                doc.Paragraphs[lastParagraphIndex].LineSpacingType = ParagraphLineSpacing.Exactly;
                doc.Paragraphs[lastParagraphIndex].LineSpacingMultiplier = 1;
                doc.Paragraphs[lastParagraphIndex].Alignment = ParagraphAlignment.Right;
                doc.Delete(doc.Range);
                mainSection.EndUpdateHeader(doc);

                SubDocument footerDoc = mainSection.BeginUpdateFooter();
                lastParagraphIndex = footerDoc.Paragraphs.Count - 1;
                footerDoc.Paragraphs[lastParagraphIndex].LineSpacingType = ParagraphLineSpacing.Exactly;
                footerDoc.Paragraphs[lastParagraphIndex].LineSpacingMultiplier = 1;
                footerDoc.Paragraphs[lastParagraphIndex].Alignment = ParagraphAlignment.Right;
                footerDoc.Delete(footerDoc.Range);
                mainSection.EndUpdateHeader(footerDoc);

                for (int i = 0; i < images.Count; i++)
                {
                    Image image = images[i];
                    int imageNativeWidth = image.Width;
                    int imageNativeHeight = image.Height;
                    Section newSection;
                    if (i == 0)
                        newSection = mainSection;
                    else
                        newSection = customRichEdit1.Document.AppendSection();

                    bool isLandscape = imageNativeWidth > imageNativeHeight;
                    newSection.Margins.Left = 0;
                    newSection.Margins.Right = 0;
                    newSection.Margins.Top = 0;
                    newSection.Margins.Bottom = 0;
                    newSection.Margins.HeaderOffset = 0;
                    newSection.Margins.FooterOffset = 0;
                    newSection.Page.Landscape = isLandscape;
                    newSection.Page.PaperKind = paperKind;

                    float pageWidth = isLandscape ? newSection.Page.Height : newSection.Page.Width;
                    float pageHeight = isLandscape ? newSection.Page.Width : newSection.Page.Height;
                    float imageWidth = Units.DocumentsToPixelsF(pageWidth, customRichEdit1.DpiX);
                    float imageHeight = Units.DocumentsToPixelsF(pageHeight, customRichEdit1.DpiY);
                    int intWidth = Convert.ToInt32(imageWidth);
                    int intHeight = Convert.ToInt32(imageHeight);
                    image = Common.ResizeImage(image, intWidth, intHeight);
                    customRichEdit1.Document.Images.Insert(newSection.Range.Start, image);
                }

                customRichEdit1.Document.EndUpdate();
                if (commitDocumentToDb)
                {
                    MemoryStream ms = new MemoryStream();
                    customRichEdit1.SaveDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);

                    SaveTemplateToITRDB(null);
                    //bool isWBS = _tag == null ? true : false;
                    //AddNewITR(SelfWBSTagGUID, isWBS, ms);
                    ////ChangeStatusWithUpdateOverriding(SelfWBSTagGUID, Guid.NewGuid(), iTRStatusChange.New, string.Empty);
                    _iTRBrowserUpdate = iTRBrowser_Update.Saved;
                    Update_Status?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Save the richedit template to database
        /// </summary>
        private void SaveTemplateToITRDB(ITR_Refresh_Item ParallelWBSTagItem)
        {
            if (!_doNotShowWaitForm)
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormDescription("Saving ...");
            }

            //dsITR_MAIN.ITR_MAINRow drITR = Common.GetITR(_tag, _wbs, _template.GUID);
            //disable overlays so that original document is saved
            customRichEdit1.Document.BeginUpdate();
            //ColorInteractables(false);
            customRichEdit1.Options.MailMerge.ViewMergedData = false;
            MemoryStream ms = new MemoryStream();
            customRichEdit1.SaveDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
            //enable overlays so that document feature is restored
            //ColorInteractables(true);
            customRichEdit1.Options.MailMerge.ViewMergedData = true;
            customRichEdit1.Document.EndUpdate();

            if (ParallelWBSTagItem != null)
            {
                if (ParallelWBSTagItem.ITR_GUID == Guid.Empty)
                    ParallelWBSTagItem.ITR_GUID = AddNewITR(ParallelWBSTagItem.WBSTagGuid, ParallelWBSTagItem.isWBS, ms);
                else
                {
                    dsITR_MAIN.ITR_MAINRow drParallelITR = _daITR.GetBy(ParallelWBSTagItem.ITR_GUID);
                    if (drParallelITR != null)
                    {
                        drParallelITR.ITR = ms.ToArray();
                        drParallelITR.UPDATED = DateTime.Now;
                        drParallelITR.UPDATEDBY = System_Environment.GetUser().GUID;
                        _daITR.Save(drParallelITR);
                    }
                }

                customRichEdit1.Modified = false;
            }
            else
            {
                //if (_ParallelWBSTagItems.Count == 0)
                //    _ParallelWBSTagItems = customRichEdit1.Get_Parallel_Items(SelfWBSTagGUID, _tag == null);

                //hereby populate ITR_GUID in the parallel items
                dsITR_MAIN.ITR_MAINDataTable dtParallelITR = Common.RevisedGetITRBy(_ParallelWBSTagItems);
                foreach (ITR_Refresh_Item ParallelItem in _ParallelWBSTagItems)
                {
                    dsITR_MAIN.ITR_MAINRow drParallelITR = null;
                    if (dtParallelITR != null)
                        drParallelITR = dtParallelITR.FirstOrDefault(obj => (!obj.IsTAG_GUIDNull() && obj.TAG_GUID == ParallelItem.WBSTagGuid) || (!obj.IsWBS_GUIDNull() && obj.WBS_GUID == ParallelItem.WBSTagGuid));

                    if (drParallelITR == null)
                    {
                        ParallelItem.ITR_GUID = AddNewITR(ParallelItem.WBSTagGuid, ParallelItem.isWBS, ms);
                    }
                    else
                    {
                        drParallelITR.ITR = ms.ToArray();
                        drParallelITR.UPDATED = DateTime.Now;
                        drParallelITR.UPDATEDBY = System_Environment.GetUser().GUID;
                        _daITR.Save(drParallelITR);
                    }
                }

                btnDelete.Enabled = true;
                customRichEdit1.Modified = false;
                //btnSave.Enabled = false;
            }

            //this is used for punchlist validation
            _primaryITRGuid = _ParallelWBSTagItems.First().ITR_GUID;
            _iTRGuid = _primaryITRGuid;

            if (!_doNotShowWaitForm)
                splashScreenManager1.CloseWaitForm();
        }

        /// <summary>
        /// Add a new ITR
        /// </summary>
        /// <returns>Added ITR Guid</returns>
        private Guid AddNewITR(Guid WBSTagGuid, bool isWBS, MemoryStream ms)
        {
            dsITR_MAIN dsITR = new dsITR_MAIN();
            dsITR_MAIN.ITR_MAINRow drNewITR = dsITR.ITR_MAIN.NewITR_MAINRow();
            drNewITR.GUID = Guid.NewGuid();

            if (isWBS)
                drNewITR.WBS_GUID = WBSTagGuid;
            else
                drNewITR.TAG_GUID = WBSTagGuid;

            drNewITR.NAME = _template.templateName;
            //drNewITR.SEQUENCE_NUMBER = Common.GetITRSequence(_tag, _wbs, _template.GUID) + 1;
            drNewITR.SEQUENCE_NUMBER = 0;
            drNewITR.DESCRIPTION = _template.templateDescription;
            drNewITR.TEMPLATE_GUID = _template.GUID;
            drNewITR.REVISION = _template.templateRevision;
            drNewITR.DISCIPLINE = _template.templateDiscipline;
            drNewITR.ITR = ms.ToArray();
            drNewITR.CREATED = DateTime.Now;
            drNewITR.CREATEDBY = System_Environment.GetUser().GUID;

            dsITR.ITR_MAIN.AddITR_MAINRow(drNewITR);
            _daITR.Save(drNewITR);

            return drNewITR.GUID;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Allows external module to retrieve loaded ITR/Template for printing
        /// </summary>
        public DevExpress.XtraRichEdit.RichEditControl GetRichEdit()
        {
            return customRichEdit1;
        }

        /// <summary>
        /// Allows the browser to know which ITR has been deleted
        /// </summary>
        public dsITR_MAIN.ITR_MAINDataTable GetDeletedITRs()
        {
            //deleted ITR always exists so there is no need to scan from _ParallelWBSTagItems
            dsITR_MAIN.ITR_MAINDataTable dtITR_DELETED = new dsITR_MAIN.ITR_MAINDataTable();
            dtITR_DELETED.Merge(_dtITR_Deleted);

            _dtITR_Deleted.Clear();

            return dtITR_DELETED;
        }

        /// <summary>
        /// Allows the browser to know which ITR has been added
        /// </summary>
        /// <returns></returns>
        public dsITR_MAIN.ITR_MAINDataTable GetAddedITRs()
        {
            //if progressing ITR the ITRs are already populated
            if (_dtITR_Refresh.Rows.Count > 0)
                return _dtITR_Refresh;
            //if progressing template the ITR needs to be refreshed
            else
                _dtITR_Refresh = Common.RevisedGetITRBy(_ParallelWBSTagItems);

            return _dtITR_Refresh;
        }

        public iTRBrowser_Update GetUpdateStatus()
        {
            return _iTRBrowserUpdate;
        }

        #endregion

        private void Shortcut_btnSaveClick()
        {
            btnSave_Click(null, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.CreateITR))
            {
                Common.Warn("You are not authorised to create ITR");
                return;
            }
            else
            {
                _ParallelWBSTagItems = customRichEdit1.Get_Parallel_ItemsRevised(_iTRGuid, SelfWBSTagGUID, _tag == null);

                ////get parallel items will update the WBSTagName based on the guid, so this method must always be above Get_Parallel_Items()
                //customRichEdit1.SelectTagScan(SelfWBSTagGUID);

                SaveTemplateToITRDB(null);
                _iTRBrowserUpdate = iTRBrowser_Update.Saved;

                //if (isITRAttachment())
                //    btnProgress.Text = "Close";
            }
        }

        private void btnPunchlist_Click(object sender, EventArgs e)
        {
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.CreatePunchlist))
            {
                Common.Warn("You are not authorised to create a punchlist");
                return;
            }

            _ParallelWBSTagItems = customRichEdit1.Get_Parallel_ItemsRevised(_iTRGuid, SelfWBSTagGUID, _tag == null);

            AddPunchlist(false, _ParallelWBSTagItems);
        }

        /// <summary>
        /// Validate user privilege and initialize punchlist addition parameters
        /// </summary>
        private void AddPunchlist(bool consecutivePopUp, List<ITR_Refresh_Item> ParallelWBSTagItems)
        {
            AdapterPUNCHLIST_MAIN daPunchlist = new AdapterPUNCHLIST_MAIN();
            AdapterPUNCHLIST_MAIN_PICTURE daPunchlistMainPicture = new AdapterPUNCHLIST_MAIN_PICTURE();

            if (System_Environment.HasPrivilege(PrivilegeTypeID.CreateITR))
            {
                SaveTemplateToITRDB(null); //saving the ITR will update _iTRGuid if it's from template, this is also needed on ITR to know the latest updated punchlist item
                _iTRBrowserUpdate = iTRBrowser_Update.Saved;
            }
            else
                return;

            //wbsTagDisplay iTRWBSTag = CreateWBSTagDisplay(_wbs, _tag);

            //List<wbsTagDisplay> listWBSTagDisplay = new List<wbsTagDisplay>();
            List<wbsTagDisplay> listWBSTagDisplay = CreateParallel_WBSTagItem(ParallelWBSTagItems);
            //listWBSTagDisplay.Add(iTRWBSTag);

            LoopPunchlistAdd(consecutivePopUp, listWBSTagDisplay, listWBSTagDisplay[0], _template.GUID, daPunchlist, daPunchlistMainPicture);
        }

        /// <summary>
        /// Allow punchlist to be added consecutively upon requirement checks
        /// </summary>
        private void LoopPunchlistAdd(bool consecutivePopUp, List<wbsTagDisplay> listWBSTagDisplay, wbsTagDisplay selectedWBSTag, Guid selectedTemplateGuid, AdapterPUNCHLIST_MAIN daPunchlist, AdapterPUNCHLIST_MAIN_PICTURE daPunchlistPicture)
        {
            frmPunchlist_Main frmPunchlistAdd = new frmPunchlist_Main(listWBSTagDisplay, selectedWBSTag, selectedTemplateGuid, _template.templateDiscipline);
            dsPUNCHLIST_MAIN dsPunchlist = new dsPUNCHLIST_MAIN();
            if (frmPunchlistAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Punchlist newPunchlist = frmPunchlistAdd.GetPunchlist();
                dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlist = dsPunchlist.PUNCHLIST_MAIN.NewPUNCHLIST_MAINRow();

                if (newPunchlist.punchlistAttachTag != null)
                {
                    drPunchlist.TAG_GUID = newPunchlist.punchlistAttachTag.GUID;
                    drPunchlist.SEQUENCE_NUMBER = daPunchlist.GetSequence(drPunchlist.TAG_GUID) + 1;
                }
                else if (newPunchlist.punchlistAttachWBS != null)
                {
                    drPunchlist.WBS_GUID = newPunchlist.punchlistAttachWBS.GUID;
                    drPunchlist.SEQUENCE_NUMBER = daPunchlist.GetSequence(drPunchlist.WBS_GUID) + 1;
                }

                drPunchlist.GUID = newPunchlist.GUID;
                drPunchlist.ITR_GUID = (Guid)newPunchlist.punchlistITR.ITRGuid;
                drPunchlist.ITR_PUNCHLIST_ITEM = newPunchlist.punchlistItem;
                if (newPunchlist.punchlistTitle.Length > 99)
                    drPunchlist.TITLE = newPunchlist.punchlistTitle.Substring(0, 99);
                else
                    drPunchlist.TITLE = newPunchlist.punchlistTitle;
                drPunchlist.DESCRIPTION = newPunchlist.punchlistDescription;
                drPunchlist.REMEDIAL = newPunchlist.punchlistRemedial;
                drPunchlist.DISCIPLINE = newPunchlist.punchlistDiscipline;
                drPunchlist.CATEGORY = newPunchlist.punchlistCategory;
                drPunchlist.ACTIONBY = newPunchlist.punchlistActionBy;
                drPunchlist.PRIORITY = newPunchlist.punchlistPriority;
                drPunchlist.CREATED = DateTime.Now;
                drPunchlist.CREATEDBY = System_Environment.GetUser().GUID;
                dsPunchlist.PUNCHLIST_MAIN.AddPUNCHLIST_MAINRow(drPunchlist);
                daPunchlist.Save(drPunchlist);

                splashScreenManager1.ShowWaitForm();
                List<Image> punchlistPictures = frmPunchlistAdd.GetPunchlistImages(PunchlistImageType.Inspection);
                daPunchlistPicture.SavePunchlistPictures(drPunchlist.GUID, punchlistPictures, PunchlistImageType.Inspection, true);
                List<Image> punchlistRemedialPictures = frmPunchlistAdd.GetPunchlistImages(PunchlistImageType.Remedial);
                daPunchlistPicture.SavePunchlistPictures(drPunchlist.GUID, punchlistPictures, PunchlistImageType.Remedial, true);
                splashScreenManager1.CloseWaitForm();

                if (System_Environment.HasPrivilege(PrivilegeTypeID.MarkPunchlistCategorised) && drPunchlist.CATEGORY != string.Empty && drPunchlist.ACTIONBY != string.Empty && drPunchlist.PRIORITY != string.Empty)
                {
                    AdapterPUNCHLIST_STATUS PunchlistStatus = new AdapterPUNCHLIST_STATUS();
                    if (Common.Confirmation("Punchlist is categorised, do you want to mark it as categorised?", "Progress Punchlist"))
                    {
                        PunchlistStatus.ChangeStatus(drPunchlist.GUID, Punchlist_Status.Inspected, true, Guid.NewGuid());
                    }
                }

                if (consecutivePopUp)
                {
                    if (!CheckITRPunchlistItems())
                        LoopPunchlistAdd(consecutivePopUp, listWBSTagDisplay, selectedWBSTag, selectedTemplateGuid, daPunchlist, daPunchlistPicture);
                    else
                    {
                        //_byPassProgressConfirmation = true;
                        btnProgress_Click(null, null);
                    }
                }

                //Common.Prompt("Punchlist Added");
            }
        }

        /// <summary>
        /// Retrieve ITR either from  tag/wbs information only or by ITR Guid when it exists
        /// </summary>
        /// <returns></returns>
        private dsITR_MAIN.ITR_MAINRow getITR()
        {
            dsITR_MAIN.ITR_MAINRow drITR;
            if (_iTRGuid == Guid.Empty)
                drITR = Common.GetITR(_tag, _wbs, _template.GUID);
            else
                drITR = Common.GetITRByGuid(_iTRGuid);

            if (_iTRGuid == Guid.Empty && drITR != null)
                _iTRGuid = drITR.GUID;

            return drITR;
        }

        private void punchlist_updated(frmPunchlist_Main frmPunchlistAdd)
        {
        }

        //private void customRichEdit1_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (customRichEdit1.Modified)
        //        btnSave.Enabled = true;
        //    else
        //        btnSave.Enabled = false;
        //}

        private void onExit()
        {
            Common.textBox_Leave(null, null);
            if (customRichEdit1.Modified && !_bypassSaveConfirmation)
            {
                if (System_Environment.HasPrivilege(PrivilegeTypeID.CreateITR) && Common.Confirmation("Changes has been made since last save\n\nDo you wish to save?", "Warning"))
                {
                    //e.Cancel = true;
                    SaveTemplateToITRDB(null);
                    _iTRBrowserUpdate = iTRBrowser_Update.Saved;
                }
            }

            if (_iTRBrowserUpdate == iTRBrowser_Update.Progressed)
            {
                if (_iTRInitialTemplateLoad)
                    _iTRBrowserUpdate = iTRBrowser_Update.SavedProgress;
                else
                    _iTRBrowserUpdate = iTRBrowser_Update.Progressed;
            }

            Update_Status?.Invoke(this);
        }

        #region overrides
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            onExit();
            base.OnFormClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _daITR.Dispose();
            _daITRStatus.Dispose();
            _daITRStatusIssue.Dispose();
            _daPrefill.Dispose();
            _daPrefillRegister.Dispose();
            _daTemplate.Dispose();
            _daUser.Dispose();
            base.OnClosed(e);
        }

        private void btnAttachImage_Click(object sender, EventArgs e)
        {
            AttachImage();
        }

        private void btnAppendPDF_Click(object sender, EventArgs e)
        {
            AppendPDF();
        }

        private void btnReplacePDF_Click(object sender, EventArgs e)
        {
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.ReplaceITRContent))
            {
                MessageBox.Show("You do not have permission to replace the content of this ITR with PDF");
                return;
            }

            OpenFileDialog thisDialog = new OpenFileDialog();

            thisDialog.Filter = "PDF (*.PDF)|*.PDF";
            thisDialog.FilterIndex = 1;
            thisDialog.RestoreDirectory = true;
            thisDialog.Multiselect = true;
            thisDialog.Title = "Please Select PDF";
            if (thisDialog.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo("Temp");
                if (Directory.Exists(directoryInfo.FullName))
                {
                    foreach (FileInfo file in directoryInfo.GetFiles())
                    {
                        file.Delete();
                    }
                }
                else
                    directoryInfo = System.IO.Directory.CreateDirectory("Temp");


                //splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
                //splashScreenManager1.ShowWaitForm();
                //splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetLargeFormat, 1000);

                List<Image> images = new List<Image>();
                foreach (string filename in thisDialog.FileNames)
                {
                    using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
                    {
                        documentProcessor.LoadDocument(filename);
                        List<string> splitFileName = filename.Split('\\').ToList();
                        //splashScreenManager1.SetWaitFormCaption("Converting PDF to image from " + splitFileName.Last() + " ...");
                        //splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, documentProcessor.Document.Pages.Count);

                        for (int i = 1; i <= documentProcessor.Document.Pages.Count; i++)
                        {
                            Bitmap pageBitmap = documentProcessor.CreateBitmap(i, 2000);
                            images.Add(pageBitmap);
                            //splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                        }
                        //splashScreenManager1.CloseWaitForm();
                        ReplaceDocumentWithImages(images, false);
                        delayedImageDisposalTimer.Start();
                        documentProcessor.CloseDocument();
                    }
                }

                MessageBox.Show("Template is replaced with attachment, please click save if you are satisfy with the result or exit otherwise");
                btnProgress.Text = "Close";
            }
        }

        private void btnAddSignature_Click(object sender, EventArgs e)
        {
            customRichEdit1.InsertPermissionForTable(CustomUserGroupListService.SIGNATURE_BLOCK, Text, false);
        }
        #endregion

        //#region CustomEdittoFixSignature
        //private void btnInsertSignature_Click(object sender, EventArgs e)
        //{
        //    FormatBeforeInsertPermission(CustomUserGroupListService.SIGNATURE_BLOCK, string.Empty);
        //}

        ///// <summary>
        ///// Perform additional format before inserting permission
        ///// </summary>
        ///// <param name="permissionName"></param>
        //private void FormatBeforeInsertPermission(string permissionName, string text)
        //{
        //    TableCell tCell = customRichEdit1.Document.GetTableCell(customRichEdit1.Document.Selection.Start);

        //    bool leaveSpaceInFront = true;
        //    if (tCell != null)
        //    {
        //        if (customRichEdit1.Document.GetText(tCell.ContentRange).Trim() == string.Empty)
        //            customRichEdit1.Document.Delete(tCell.ContentRange);

        //        ParagraphProperties pp = customRichEdit1.Document.BeginUpdateParagraphs(tCell.ContentRange);
        //        if (pp.Alignment == ParagraphAlignment.Right)
        //            leaveSpaceInFront = false; //cannot leave space in front if horizontal alignment is left because user selection wouldn't be attached otherwise
        //        customRichEdit1.Document.EndUpdateParagraphs(pp);
        //    }

        //    int startPos = customRichEdit1.Document.Selection.Start.ToInt();
        //    DocumentPosition originalPosition = customRichEdit1.Document.Selection.Start;
        //    SubDocument subDoc = customRichEdit1.Document.Selection.BeginUpdateDocument();

        //    string strBuffer = " ";
        //    if (leaveSpaceInFront)
        //        strBuffer += " ";

        //    subDoc.InsertText(originalPosition, strBuffer + text); //first space is so that range doesn't stick to the table, second space is so that document range have something to assign to
        //    subDoc.EndUpdate();
        //    //if the permission range is attached to the top left of a tablecell, all kinds of weird stuff will happen when the table is moved/splitted

        //    int endPos = customRichEdit1.Document.Selection.End.ToInt();

        //    DocumentRange docRange;
        //    if (leaveSpaceInFront)
        //        docRange = customRichEdit1.Document.CreateRange(startPos + 1, endPos - (startPos + 1));
        //    else
        //        docRange = customRichEdit1.Document.CreateRange(startPos, endPos - startPos);

        //    InsertRangePermission(permissionName, docRange);
        //}

        ///// <summary>
        ///// Insert a permission group name on the assigned document range
        ///// </summary>
        //private void InsertRangePermission(string GroupName, DocumentRange DocRange)
        //{
        //    RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();

        //    RangePermission rp = rangePermissions.CreateRangePermission(DocRange);
        //    rp.Group = GroupName;
        //    rangePermissions.Add(rp);
        //    customRichEdit1.Document.EndUpdateRangePermissions(rangePermissions);
        //}

        //private void btnRogueSave_Click(object sender, EventArgs e)
        //{
        //    SaveTemplateToITRDB();
        //}
        //#endregion
    }
}
