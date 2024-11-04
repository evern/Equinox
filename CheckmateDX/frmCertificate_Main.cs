using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Pdf;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering.Templates;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Services;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckmateDX
{
    public partial class frmCertificate_Main : DevExpress.XtraEditors.XtraForm
    {
        ViewModel_Certificate _viewModel_Certificate;
        Guid _projectGuid;
        dsPROJECT.PROJECTRow _drProject;
        bool isCVC = false;
        CVC_Status _CVC_Status;
        CVC_Status _nextCVC_Status;
        bool isNOE = false;
        NOE_Status _NOE_Status;
        NOE_Status _nextNOE_Status;
        PunchlistWalkdown_Status _Punchlist_Status;
        PunchlistWalkdown_Status _nextPunchlist_Status;
        bool isPunchlistWalkdown = false;
        bool _isProtectDocument;
        bool _isApprovedDocument;
        public frmCertificate_Main()
        {
            InitializeComponent();
        }

        public frmCertificate_Main(Guid projectGuid, ViewModel_Certificate viewModel_Certificate, string title)
        {
            InitializeComponent();
            customRichEdit1.SetSignatureUser(SignatureUserHelper.GetSignatureUser());
            this.Text = title;
            bool isLoadMasterITRStatuses = false;
            bool isLoadMasterPunchlistStatuses = false;
            bool isLoadCVCStatuses = false;
            bool isLoadPunchlistWalkdownStatuses = false;
            if (viewModel_Certificate.GetType() == typeof(ViewModel_CVC))
            {
                btnExportToPDF.Visible = true;
                btnImportPDF.Visible = false;
                btnReplacePDF.Visible = true;
                isLoadMasterITRStatuses = true;
                isLoadMasterPunchlistStatuses = true;
                isLoadPunchlistWalkdownStatuses = true;
                isCVC = true;
            }

            if(viewModel_Certificate.GetType() == typeof(ViewModel_NOE))
            {
                btnExportToPDF.Visible = true;
                btnImportPDF.Visible = false;
                btnReplacePDF.Visible = true;
                isLoadCVCStatuses = true;
                isLoadMasterPunchlistStatuses = true;
                isNOE = true;
            }

            if (viewModel_Certificate.GetType() == typeof(ViewModel_PunchlistWalkdown))
            {
                btnReplacePDF.Visible = false;
                isPunchlistWalkdown = true;
            }

            _projectGuid = projectGuid;
            _viewModel_Certificate = viewModel_Certificate;
            _drProject = getPROJECT();
            customRichEdit1.Click += customRichEdit1_Click;
            customRichEdit1.LoadCertificateFromDB(_viewModel_Certificate.GUID, _projectGuid);
            customRichEdit1.PopulatePrefill(null, null, _viewModel_Certificate, string.Empty, string.Empty, false);
            if (isLoadMasterITRStatuses)
                customRichEdit1.LoadMasterITRReport(_projectGuid);

            if (isLoadMasterPunchlistStatuses)
                customRichEdit1.LoadPunchlistReport(_projectGuid);

            if (isLoadCVCStatuses)
                customRichEdit1.LoadCVCReport(_projectGuid);

            if (isLoadPunchlistWalkdownStatuses)
                customRichEdit1.LoadPunchlistWalkdownReport(_projectGuid);

            PopulateSignatures();

            if(isCVC)
            {
                _CVC_Status = (CVC_Status)_viewModel_Certificate.StatusNumber;
                if (_CVC_Status == CVC_Status.Client)
                {
                    btnProgress.Visible = false;
                    customRichEdit1._AllowPermissionColoring = false;
                }
                else
                {
                    _nextCVC_Status = (CVC_Status)(_viewModel_Certificate.StatusNumber + 1);
                    //customRichEdit1.InsertHeaderQRCode(getMetaTag());
                    //btnProgress.Text = "Approve by " + _nextCVC_Status.ToString();
                }

                if (_CVC_Status > CVC_Status.Pending)
                    _isApprovedDocument = true;

                //word document permission to edit
                if (_CVC_Status > CVC_Status.Approved)
                    _isProtectDocument = true;

                if (_CVC_Status == CVC_Status.Pending)
                    btnReject.Visible = false;

                customRichEdit1.Options.Authentication.Group = CustomUserGroupListService.USER; //permission to edit
            }

            if(isNOE)
            {
                _NOE_Status = (NOE_Status)_viewModel_Certificate.StatusNumber;
                if (_NOE_Status == NOE_Status.Commissioning_Manager)
                {
                    btnProgress.Visible = false;
                    customRichEdit1._AllowPermissionColoring = false;
                }
                else
                {
                    _nextNOE_Status = (NOE_Status)(_viewModel_Certificate.StatusNumber + 1);
                    //customRichEdit1.InsertHeaderQRCode(getMetaTag());
                    //btnProgress.Text = "Approve by " + _nextCVC_Status.ToString();
                }

                if (_NOE_Status > NOE_Status.Pending)
                    _isApprovedDocument = true;

                //word document permission to edit
                if (_NOE_Status > NOE_Status.Approved)
                    _isProtectDocument = true;

                if (_NOE_Status == NOE_Status.Pending)
                    btnReject.Visible = false;

                customRichEdit1.Options.Authentication.Group = CustomUserGroupListService.USER; //permission to edit
            }

            if(isPunchlistWalkdown)
            {
                _Punchlist_Status = (PunchlistWalkdown_Status)_viewModel_Certificate.StatusNumber;
                if (_Punchlist_Status == PunchlistWalkdown_Status.Closed)
                {
                    btnProgress.Visible = false;
                    customRichEdit1._AllowPermissionColoring = false;
                }
                else
                {
                    _nextPunchlist_Status = (PunchlistWalkdown_Status)(_viewModel_Certificate.StatusNumber + 1);
                    //Remove QR code for Punchlist Walkdown
                    //customRichEdit1.InsertHeaderQRCode(getMetaTag());
                }

                if (_Punchlist_Status > PunchlistWalkdown_Status.Pending)
                {
                    _isApprovedDocument = true;
                    _isProtectDocument = true;
                }

                customRichEdit1.Options.Authentication.Group = CustomUserGroupListService.USER; //permission to edit
            }

            customRichEdit1.Document.Protect(string.Empty);
            SetReplaceCommandMethod();
            customRichEdit1.Modified = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveCertificate();
            ProjectCommon.Common.Prompt("Certificate saved");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if(customRichEdit1.Modified)
            {
                if(ProjectCommon.Common.Confirmation("You have made changes to the document, do you wish to save before exiting?", "Document has been modified"))
                {
                    saveCertificate();
                }
            }

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnImportPDF_Click(object sender, EventArgs e)
        {
            if (AttachPDF())
            {
                MessageBox.Show("Punchlist walk down is replaced with attachment, please click save if you are satisfy with the result or exit otherwise", "PDF Imported", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnReplacePDF_Click(object sender, EventArgs e)
        {
            if (AttachPDF())
            {
                MessageBox.Show("Certificate is replaced with attachment, please click save if you are satisfy with the result or exit otherwise", "PDF Imported", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnExportToPDF_Click(object sender, EventArgs e)
        {
            if (isPunchlistWalkdown)
                if (!checkPunchlistWalkdownAuthorisation(false))
                    return;

            OpenFileDialog thisDialog = new OpenFileDialog();

            thisDialog.Filter = "PDF (*.PDF)|*.PDF";
            thisDialog.FilterIndex = 1;
            thisDialog.RestoreDirectory = true;
            thisDialog.Multiselect = false;
            thisDialog.Title = "Please Select PDF";

            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.RootFolder = Environment.SpecialFolder.Desktop;

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormDescription("Exporting ...");

                string projectNumber = _drProject == null ? string.Empty : _drProject.NUMBER;
                string outputFileName = string.Concat(projectNumber, "_", _viewModel_Certificate.Number);
                string fullFilePath = fd.SelectedPath + "\\" + outputFileName + ".pdf";

                if (isPunchlistWalkdown)
                {
                    customRichEdit1.Color_Interactables(false, false, HighlightType.Both); //take out color during export
                    customRichEdit1.ExportToPdf(fullFilePath);
                    customRichEdit1.Color_Interactables(true, false, HighlightType.Both); //restore color after export
                }
                else
                {
                    customRichEdit1.Color_Interactables(false, false, HighlightType.Both); //take out color during export
                    customRichEdit1.ExportToPdf(fullFilePath);
                    if(customRichEdit1._AllowPermissionColoring)
                        customRichEdit1.Color_Interactables(true, false, HighlightType.Both); //restore color after export
                }

                saveCertificate();

                splashScreenManager1.SetWaitFormDescription("Verifying QR Code ...");
                //int QRCodeErrorPageCount = checkQRCode(fullFilePath);
                splashScreenManager1.CloseWaitForm();

                //if (QRCodeErrorPageCount != -1)
                //{
                //    MessageBox.Show("PDF exported to selected folder\n\nHowever QR code is not detected in page " + QRCodeErrorPageCount.ToString() + "\n\nTry adding a page break in the template before page " + QRCodeErrorPageCount.ToString() + "\n\nThen re-create the certificate and try exporting again", "Exported with Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //}
                //else
                //{
                    if (isPunchlistWalkdown)
                    {
                        if(_Punchlist_Status == PunchlistWalkdown_Status.Pending)
                        {
                            ChangeStatusWithUpdateOverriding(_viewModel_Certificate.GUID, Guid.NewGuid(), CertificateStatusChange.Increase, GetComments());
                            MessageBox.Show("PDF exported to selected folder, punchlist walkdown status is now exported", "Exported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                            MessageBox.Show("PDF exported to selected folder", "Exported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("PDF exported to selected folder", "Exported", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}

                this.Close();
            }
        }

        private int checkQRCode(string filePath)
        {
            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
            {
                MemoryStream ms = new MemoryStream();
                documentProcessor.LoadDocument(filePath);
                for (int i = 1; i <= documentProcessor.Document.Pages.Count; i++)
                {
                    Bitmap pageBitmap = documentProcessor.CreateBitmap(i, 2000);
                    string metaString = ProjectCommon.Common.GetDocumentMetaQRCodeTryHarder(pageBitmap);
                    if (metaString == string.Empty)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private void customRichEdit1_Click(object sender, EventArgs e)
        {
            if(isCVC)
            {
                customRichEdit1.RichEditClick_Interactions(true, _isProtectDocument, _isApprovedDocument, _nextCVC_Status.ToString());
                customRichEdit1.Modified = true;
            }
            else
            {
                customRichEdit1.RichEditClick_Interactions(true, _isProtectDocument, _isApprovedDocument);
                customRichEdit1.Modified = true;
            }
        }

        private dsPROJECT.PROJECTRow getPROJECT()
        {
            using(AdapterPROJECT daPROJECT = new AdapterPROJECT())
            {
                return daPROJECT.GetBy(_projectGuid);
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
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormDescription("Importing ...");

                string fileName = thisDialog.FileName;
                List<Image> images = new List<Image>();
                //Remove validation around QR code
                //int? pageTotalsFromQRTotal = null;
                //int pageCountFromQRTotal = 0;
                //int pageTotalsFromQR = 0;
                string errorMessage = string.Empty;
                using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
                {
                    bool isError = false;
                    bool isQRCodeError = false;
                    documentProcessor.LoadDocument(fileName);
                    for (int i = 1; i <= documentProcessor.Document.Pages.Count; i++)
                    {
                        Bitmap pageBitmap = documentProcessor.CreateBitmap(i, 2000);
                        //string metaString = ProjectCommon.Common.GetDocumentMetaQRCodeTryHarder(pageBitmap);
                        //if (metaString == string.Empty)
                        //{
                        //    isQRCodeError = true;
                        //    break;
                        //}

                        //string[] metaArray = metaString.Split(';');
                        //if (metaArray.Count() < 2)
                        //{
                        //    isError = true;
                        //    break;
                        //}
                        //else
                        //{
                        //    string[] thisMeta = getMetaTag().Split(';');
                        //    string this_certificate_number = thisMeta[1];
                        //    string this_document_name = thisMeta[2];
                        //    int pageCount = Int32.Parse(metaArray[3]);
                        //    pageCountFromQRTotal = Int32.Parse(metaArray[4]);
                        //    if (pageTotalsFromQRTotal == null)
                        //    {
                        //        pageTotalsFromQRTotal = 0;
                        //        for (int j = 1; j <= pageCountFromQRTotal; j++)
                        //        {
                        //            pageTotalsFromQRTotal += j;
                        //        }
                        //    }

                        //    pageTotalsFromQR += pageCount;

                        //    string certificate_number = metaArray[1];
                        //    string document_name = metaArray[2];

                        //    if (this_certificate_number != certificate_number)
                        //    {
                        //        errorMessage = "PDF doesn't match certificate number: " + this_certificate_number;
                        //        isError = true;
                        //        break;
                        //    }
                        //    else if(this_document_name != document_name)
                        //    {
                        //        errorMessage = "PDF doesn't match document name: " + this_document_name + "\n\nRe-creating the document may fix the issue";
                        //        isError = true;
                        //        break;
                        //    }
                        //}

                        images.Add(pageBitmap);
                    }

                    splashScreenManager1.CloseWaitForm();
                    if (isQRCodeError)
                    {
                        MessageBox.Show("QR code is missing or not recognisable", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    else if (isError)
                    {
                        MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    //if(pageTotalsFromQRTotal != pageTotalsFromQR)
                    //{
                    //    MessageBox.Show("Page count from PDF doesn't match template total pages of " + pageCountFromQRTotal, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //    return false;
                    //}

                    splashScreenManager1.ShowWaitForm();
                    splashScreenManager1.SetWaitFormDescription("Inserting Images ...");
                    ReplaceDocumentWithImages(images, true);
                    splashScreenManager1.CloseWaitForm();
                    return true;
                }
            }

            return false;
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

                customRichEdit1._certificateGuid = _viewModel_Certificate.GUID;

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
                    image = ProjectCommon.Common.ResizeImage(image, intWidth, intHeight);
                    customRichEdit1.Document.Images.Insert(newSection.Range.Start, image);
                }

                customRichEdit1.Document.EndUpdate();
            }
        }

        private string getMetaTag()
        {
            if (customRichEdit1.Options.MailMerge.DataSource == null)
                return string.Empty;

            if (((DataTable)customRichEdit1.Options.MailMerge.DataSource).Rows.Count == 0)
                return string.Empty;

            DataRow dataRow = ((DataTable)customRichEdit1.Options.MailMerge.DataSource).Rows[0];
            string project_number = dataRow[Variables.prefillProjNumber].ToString().ToUpper();
            string certificateNumber = dataRow[Variables.prefillCertificateNumber].ToString().ToUpper();
            string document_name = string.Empty;

            if (isPunchlistWalkdown)
                document_name = Variables.punchlistWalkdownTemplateName.ToString().ToUpper();
            else if (isCVC)
                document_name = Variables.constructionVerificationCertificateTemplateName.ToString().ToUpper();
            else if (isNOE)
                document_name = Variables.noticeOfEnergisationCertificateTemplateName.ToString().ToUpper();

            return project_number + ";" + certificateNumber + ";" + document_name;
        }

        private void saveCertificate()
        {
            customRichEdit1.Document.BeginUpdate();
            customRichEdit1.Options.MailMerge.ViewMergedData = false;
            MemoryStream ms = new MemoryStream();
            customRichEdit1.SaveDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
            customRichEdit1.Options.MailMerge.ViewMergedData = true;
            customRichEdit1.Document.EndUpdate();
            customRichEdit1.Modified = false;

            using (AdapterCERTIFICATE_MAIN daCERTIFICATE_MAIN = new AdapterCERTIFICATE_MAIN())
            {
                dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow drCERTIFICATE_MAIN = daCERTIFICATE_MAIN.GetBy(_viewModel_Certificate.GUID);
                if (drCERTIFICATE_MAIN != null)
                {
                    drCERTIFICATE_MAIN.CERTIFICATE = ms.ToArray();
                    drCERTIFICATE_MAIN.UPDATED = DateTime.Now;
                    drCERTIFICATE_MAIN.UPDATEDBY = System_Environment.GetUser().GUID;
                    daCERTIFICATE_MAIN.Save(drCERTIFICATE_MAIN);

                    //closed status is a picture that cannot be scrapped
                    if(isPunchlistWalkdown)
                    {
                        if(_nextPunchlist_Status != PunchlistWalkdown_Status.Closed)
                        {
                            saveCertificateDisciplines();
                            saveCertificateSubsystems();
                        }
                    }
                    else if(!customRichEdit1.IsDocumentPictures())
                    {
                        saveCertificateDisciplines();
                        saveCertificateSubsystems();
                    }

                    saveCVCType();
                }
                else
                    MessageBox.Show("Certificate not found, please contact IT support", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        List<RichEditCommandDelegateContainer> _replaceCommandMethod = new List<RichEditCommandDelegateContainer>();
        /// <summary>
        /// Assign local method to commands on richedit
        /// </summary>
        private void SetReplaceCommandMethod()
        {
            _replaceCommandMethod.Add(new RichEditCommandDelegateContainer(RichEditCommandId.FileSave, new RichEditCommandDelegate(saveCertificate)));
            IRichEditCommandFactoryService service = customRichEdit1.GetService(typeof(IRichEditCommandFactoryService)) as IRichEditCommandFactoryService;
            customRichEdit1.ReplaceService<IRichEditCommandFactoryService>(new CustomRichEditCommandFactoryService(service, customRichEdit1, _replaceCommandMethod));
            customRichEdit1.ReplaceService<IUserGroupListService>(new CustomUserGroupListService());
        }

        /// <summary>
        /// Populate the wbsTagDisplay class based on parameters
        /// </summary>
        /// <param name="drITRorTemplate">Must be either ITR or Template</param>
        private void saveCertificateDisciplines()
        {
            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

            using(AdapterCERTIFICATE_DATA daCERTIFICATE_DATA = new AdapterCERTIFICATE_DATA())
            {
                foreach (RangePermission rangePermission in rangePermissions)
                {
                    if (rangePermission.Group == CustomUserGroupListService.DISCIPLINE_TOGGLE_ARCHITECTURAL)
                        isSaveDisciplineToggle(daCERTIFICATE_DATA, _viewModel_Certificate.GUID, Discipline.Architectural, customRichEdit1, rangePermission.Range);
                    else if (rangePermission.Group == CustomUserGroupListService.DISCIPLINE_TOGGLE_CIVILS)
                        isSaveDisciplineToggle(daCERTIFICATE_DATA, _viewModel_Certificate.GUID, Discipline.Civil, customRichEdit1, rangePermission.Range);
                    else if (rangePermission.Group == CustomUserGroupListService.DISCIPLINE_TOGGLE_ELECTRICAL)
                        isSaveDisciplineToggle(daCERTIFICATE_DATA, _viewModel_Certificate.GUID, Discipline.Electrical, customRichEdit1, rangePermission.Range);
                    else if (rangePermission.Group == CustomUserGroupListService.DISCIPLINE_TOGGLE_INSTRUMENTATION)
                        isSaveDisciplineToggle(daCERTIFICATE_DATA, _viewModel_Certificate.GUID, Discipline.Instrumentation, customRichEdit1, rangePermission.Range);
                    else if (rangePermission.Group == CustomUserGroupListService.DISCIPLINE_TOGGLE_MECHANICAL)
                        isSaveDisciplineToggle(daCERTIFICATE_DATA, _viewModel_Certificate.GUID, Discipline.Mechanical, customRichEdit1, rangePermission.Range);
                    else if (rangePermission.Group == CustomUserGroupListService.DISCIPLINE_TOGGLE_OTHERS)
                        isSaveDisciplineToggle(daCERTIFICATE_DATA, _viewModel_Certificate.GUID, Discipline.Others, customRichEdit1, rangePermission.Range);
                    else if (rangePermission.Group == CustomUserGroupListService.DISCIPLINE_TOGGLE_PIPING)
                        isSaveDisciplineToggle(daCERTIFICATE_DATA, _viewModel_Certificate.GUID, Discipline.Piping, customRichEdit1, rangePermission.Range);
                    else if (rangePermission.Group == CustomUserGroupListService.DISCIPLINE_TOGGLE_STRUCTURAL)
                        isSaveDisciplineToggle(daCERTIFICATE_DATA, _viewModel_Certificate.GUID, Discipline.Structural, customRichEdit1, rangePermission.Range);
                }
            }
        }

        private void saveCVCType()
        {
            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

            using (AdapterCERTIFICATE_DATA daCERTIFICATE_DATA = new AdapterCERTIFICATE_DATA())
            {
                //mark all as deleted
                daCERTIFICATE_DATA.DeleteBy(_viewModel_Certificate.GUID, Variables.Certificate_DataType_CVC_Type);
                foreach (RangePermission rangePermission in rangePermissions)
                {
                    if (rangePermission.Group.Contains(CustomUserGroupListService.CVC_TYPE))
                    {
                        string cvcType = customRichEdit1.Document.GetText(rangePermission.Range).Trim();
                        if(cvcType != Variables.Select_CVC_Type)
                            findExistingAddOrDeleteCertificateMeta(daCERTIFICATE_DATA, Variables.Certificate_DataType_CVC_Type, _viewModel_Certificate.GUID, cvcType, false, true);
                    }
                }
            }
        }

        private void saveCertificateSubsystems()
        {
            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

            using (AdapterCERTIFICATE_DATA daCERTIFICATE_DATA = new AdapterCERTIFICATE_DATA())
            {
                //mark all as deleted
                daCERTIFICATE_DATA.DeleteBy(_viewModel_Certificate.GUID, Variables.Certificate_DataType_Subsystem);
                List<string> subsystemNumbers = customRichEdit1.GetSubsystemNumbers();

                foreach (string subsystemNumber in subsystemNumbers)
                {
                    findExistingAddOrDeleteCertificateMeta(daCERTIFICATE_DATA, Variables.Certificate_DataType_Subsystem, _viewModel_Certificate.GUID, subsystemNumber, false, true);
                }
            }
        }

        private void isSaveDisciplineToggle(AdapterCERTIFICATE_DATA daCERTIFICATE_DATA, Guid certificateGuid, Discipline discipline, CustomRichEdit customRichEdit, DocumentRange documentRange)
        {
            string documentRangeText = customRichEdit1.Document.GetText(documentRange);
            if(documentRangeText == Toggle_YesNo.Yes.ToString())
                findExistingAddOrDeleteCertificateMeta(daCERTIFICATE_DATA, Variables.Certificate_DataType_Discipline, _viewModel_Certificate.GUID, discipline.ToString(), false, true);
            else
                findExistingAddOrDeleteCertificateMeta(daCERTIFICATE_DATA, Variables.Certificate_DataType_Discipline, _viewModel_Certificate.GUID, discipline.ToString(), true, true);
        }

        private void findExistingAddOrDeleteCertificateMeta(AdapterCERTIFICATE_DATA daCERTIFICATE_DATA, string certificateDataType, Guid certificateGuid, string meta, bool isDelete, bool includeDeleted)
        {
            dsCERTIFICATE_DATA.CERTIFICATE_DATARow drCERTIFICATE_DATA = daCERTIFICATE_DATA.GetBy(certificateGuid, certificateDataType, meta, includeDeleted);
            if(drCERTIFICATE_DATA != null)
            {
                if(isDelete && drCERTIFICATE_DATA.IsDELETEDNull())
                    daCERTIFICATE_DATA.RemoveBy(drCERTIFICATE_DATA.GUID);
                else if(!drCERTIFICATE_DATA.IsDELETEDNull())
                {
                    drCERTIFICATE_DATA.SetDELETEDBYNull();
                    drCERTIFICATE_DATA.SetDELETEDNull();
                    daCERTIFICATE_DATA.Save(drCERTIFICATE_DATA);
                }
            }
            else if(!isDelete)
            {
                dsCERTIFICATE_DATA dsCERTIFICATE_DATA = new dsCERTIFICATE_DATA();
                dsCERTIFICATE_DATA.CERTIFICATE_DATARow drNewCERTIFICATE_DATA = dsCERTIFICATE_DATA.CERTIFICATE_DATA.NewCERTIFICATE_DATARow();
                drNewCERTIFICATE_DATA.GUID = Guid.NewGuid();
                drNewCERTIFICATE_DATA.CERTIFICATEGUID = certificateGuid;
                drNewCERTIFICATE_DATA.DATA_TYPE = certificateDataType;
                drNewCERTIFICATE_DATA.DATA1 = meta;
                drNewCERTIFICATE_DATA.CREATED = DateTime.Now;
                drNewCERTIFICATE_DATA.CREATEDBY = System_Environment.GetUser().GUID;

                dsCERTIFICATE_DATA.CERTIFICATE_DATA.AddCERTIFICATE_DATARow(drNewCERTIFICATE_DATA);
                daCERTIFICATE_DATA.Save(drNewCERTIFICATE_DATA);
            }
        }

        bool isRejecting = false;
        private void btnReject_Click(object sender, EventArgs e)
        {
            if (isRejecting)
                return;

            if (isCVC)
            {
                if (_viewModel_Certificate.StatusNumber == (int)CVC_Status.Approved && !System_Environment.HasPrivilege(PrivilegeTypeID.Reject_CVC_Status_Approved) ||
                    _viewModel_Certificate.StatusNumber == (int)CVC_Status.Client && !System_Environment.HasPrivilege(PrivilegeTypeID.Reject_CVC_Status_Client) ||
                    _viewModel_Certificate.StatusNumber == (int)CVC_Status.Commissioning_Manager && !System_Environment.HasPrivilege(PrivilegeTypeID.Reject_CVC_Status_CommissioningManager) ||
                    _viewModel_Certificate.StatusNumber == (int)CVC_Status.Construction_Manager && !System_Environment.HasPrivilege(PrivilegeTypeID.Reject_CVC_Status_ConstructionManager) ||
                    _viewModel_Certificate.StatusNumber == (int)CVC_Status.Supervisor && !System_Environment.HasPrivilege(PrivilegeTypeID.Reject_CVC_Status_Supervisor))
                {
                    ProjectCommon.Common.Warn("You are not authorised to reject this certificate");
                    return;
                }
            }

            if (isNOE)
            {
                if (_viewModel_Certificate.StatusNumber == (int)NOE_Status.Approved && !System_Environment.HasPrivilege(PrivilegeTypeID.Reject_NOE_Status_Approved) ||
                    _viewModel_Certificate.StatusNumber == (int)NOE_Status.Authorised_Isolation_Officer && !System_Environment.HasPrivilege(PrivilegeTypeID.Reject_NOE_Status_AuthorisationIsolationOfficer) ||
                    _viewModel_Certificate.StatusNumber == (int)NOE_Status.Commissioning_Engineer && !System_Environment.HasPrivilege(PrivilegeTypeID.Reject_NOE_Status_CommissioningEngineer) ||
                    _viewModel_Certificate.StatusNumber == (int)NOE_Status.Commissioning_Manager && !System_Environment.HasPrivilege(PrivilegeTypeID.Reject_NOE_Status_CommissioningManager) ||
                    _viewModel_Certificate.StatusNumber == (int)NOE_Status.Construction_Manager && !System_Environment.HasPrivilege(PrivilegeTypeID.Reject_NOE_Status_ConstructionManager))
                {
                    ProjectCommon.Common.Warn("You are not authorised to reject this certificate");
                    return;
                }
            }

            if (isPunchlistWalkdown)
            {
                if (_viewModel_Certificate.StatusNumber == (int)PunchlistWalkdown_Status.Exported && !System_Environment.HasPrivilege(PrivilegeTypeID.Reject_PunchlistWalkdown_Status_Exported) ||
                    _viewModel_Certificate.StatusNumber == (int)PunchlistWalkdown_Status.Closed && !System_Environment.HasPrivilege(PrivilegeTypeID.Reject_PunchlistWalkdown_Status_Closed))
                {
                    ProjectCommon.Common.Warn("You are not authorised to reject this certificate");
                    return;
                }
            }

            AdapterCERTIFICATE_STATUS daCERTIFICATE_STATUS = new AdapterCERTIFICATE_STATUS();
            AdapterCERTIFICATE_STATUS_ISSUE daCERTIFICATE_STATUS_ISSUE = new AdapterCERTIFICATE_STATUS_ISSUE();
            try
            {
                string Comment = null;
                //dsITR_MAIN.ITR_MAINRow drITR = Common.GetITR(_tag, _wbs, _template.GUID);
                dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drCertificateStatus = daCERTIFICATE_STATUS.GetRowBy(_viewModel_Certificate.GUID);
                if (drCertificateStatus != null)
                {


                    if (Comment == null)
                    {
                        frmCertificateComment f = new frmCertificateComment(_viewModel_Certificate, "Reject");
                        if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            Comment = f.GetComment();
                        else
                            return;
                    }

                    if (Comment != null)
                    {
                        isRejecting = true;
                        Guid certificatStatusGuid = Guid.NewGuid();
                        ChangeStatusWithUpdateOverriding(_viewModel_Certificate.GUID, certificatStatusGuid, CertificateStatusChange.Decrease, Comment, daCERTIFICATE_STATUS, daCERTIFICATE_STATUS_ISSUE);
                        if ((isCVC && drCertificateStatus.STATUS_NUMBER == (int)CVC_Status.Client) || (isNOE && drCertificateStatus.STATUS_NUMBER == (int)NOE_Status.Commissioning_Manager) || (isPunchlistWalkdown && drCertificateStatus.STATUS_NUMBER == (int)PunchlistWalkdown_Status.Closed))
                        {
                            customRichEdit1.Color_Interactables(true, false, HighlightType.Both); //restore the color
                                                                                                  //customRichEdit1.Set_TouchUI(true); //restore touchUI state
                            saveCertificate();
                        }
                        else
                            saveCertificate();

                        isRejecting = false;
                    }

                    //this.Close();
                    customRichEdit1.Modified = false;
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
            }
            finally
            {
                daCERTIFICATE_STATUS.Dispose();
                daCERTIFICATE_STATUS_ISSUE.Dispose();
            }
        }


        /// <summary>
        /// Update iTR status and inform iTR browser on the changes
        /// </summary>
        private void ChangeStatusWithUpdateOverriding(Guid certificateGuid, Guid certificateStatusGuid, CertificateStatusChange statusChange, string statusComments, AdapterCERTIFICATE_STATUS daCERTIFICATE_STATUS = null, AdapterCERTIFICATE_STATUS_ISSUE daCERTIFICATE_STATUS_ISSUE = null)
        {
            bool disposeCertificateStatus = false;
            bool disposeCertificateStatusIssue = false;
            if (daCERTIFICATE_STATUS == null)
            {
                daCERTIFICATE_STATUS = new AdapterCERTIFICATE_STATUS();
                disposeCertificateStatus = true;
            }

            if(daCERTIFICATE_STATUS_ISSUE == null)
            {
                daCERTIFICATE_STATUS_ISSUE = new AdapterCERTIFICATE_STATUS_ISSUE();
                disposeCertificateStatusIssue = true;
            }

            try
            {
                switch (statusChange)
                {
                    case CertificateStatusChange.Increase:
                        daCERTIFICATE_STATUS.ChangeStatus(certificateGuid, true, certificateStatusGuid);
                        daCERTIFICATE_STATUS_ISSUE.AddComments(certificateStatusGuid, statusComments, false);
                        break;
                    case CertificateStatusChange.Decrease:
                        daCERTIFICATE_STATUS.ChangeStatus(certificateGuid, false, certificateStatusGuid);
                        daCERTIFICATE_STATUS_ISSUE.AddComments(certificateStatusGuid, statusComments, true);
                        break;
                    case CertificateStatusChange.New:
                        daCERTIFICATE_STATUS.ChangeStatus(certificateGuid, true, Guid.NewGuid());
                        //don't need to inform iTR browser to check for new status because it'll be check for saving from template to iTR
                        break;
                }
            }
            finally
            {
                if (disposeCertificateStatus)
                    daCERTIFICATE_STATUS.Dispose();

                if (disposeCertificateStatusIssue)
                    daCERTIFICATE_STATUS_ISSUE.Dispose();
            }
        }


        private void btnProgress_Click(object sender, EventArgs e)
        {
            if(isPunchlistWalkdown && _nextPunchlist_Status == PunchlistWalkdown_Status.Exported)
            {
                ProjectCommon.Common.Warn("Please export punchlist walkdown to PDF in order to progress it");
                return;
            }

            Progress_Restriction restriction = customRichEdit1.CheckInteractables();
            if (_isApprovedDocument)
            {
                if (restriction != Progress_Restriction.None)
                {
                    if (restriction == Progress_Restriction.Acceptance)
                        ProjectCommon.Common.Warn("Please complete all 'Click Here' toggle before submitting");

                    return;
                }

                if (!customRichEdit1.IsPunchlistCAT_A_Completed())
                {
                    ProjectCommon.Common.Warn("Please verify that all CAT A punchlist are completed");
                    return;
                }

                if (!customRichEdit1.IsAll_ITR_Completed())
                {
                    ProjectCommon.Common.Warn("Please verify that all ITRs are completed for selected subsystems");
                    return;
                }

                if (!customRichEdit1.IsAll_CVC_Completed())
                {
                    ProjectCommon.Common.Warn("Please verify that all CVCs are completed for selected subsystems");
                    return;
                }

                if(!customRichEdit1.IsAll_PunchlistWalkdown_Completed())
                {
                    ProjectCommon.Common.Warn("Please verify that all Punchlist Walkdowns are completed for selected subsystems");
                    return;
                }
            }

            if (isCVC || isNOE)
            {
                string certificateStatusString = isCVC ? ProjectCommon.Common.Replace_WithSpaces(_nextCVC_Status.ToString()) : ProjectCommon.Common.Replace_WithSpaces(_nextNOE_Status.ToString());
                if (isCVC)
                {

                    bool isError = false;
                    if (_nextCVC_Status == CVC_Status.Approved && !System_Environment.HasPrivilege(PrivilegeTypeID.CVC_Status_Approved))
                        isError = true;
                    else if (_nextCVC_Status == CVC_Status.Supervisor && !System_Environment.HasPrivilege(PrivilegeTypeID.CVC_Status_Supervisor))
                        isError = true;
                    else if (_nextCVC_Status == CVC_Status.Construction_Manager && !System_Environment.HasPrivilege(PrivilegeTypeID.CVC_Status_ConstructionManager))
                        isError = true;
                    else if (_nextCVC_Status == CVC_Status.Client && !System_Environment.HasPrivilege(PrivilegeTypeID.CVC_Status_Client))
                        isError = true;
                    else if (_nextCVC_Status == CVC_Status.Commissioning_Manager && !System_Environment.HasPrivilege(PrivilegeTypeID.CVC_Status_CommissioningManager))
                        isError = true;

                    if (isError)
                    {
                        ProjectCommon.Common.Warn("You are not authorised to approve certificate as a " + certificateStatusString);
                        return;
                    }
                }
                else if (isNOE)
                {
                    bool isError = false;
                    if (_nextNOE_Status == NOE_Status.Approved && !System_Environment.HasPrivilege(PrivilegeTypeID.NOE_Status_Approved))
                        isError = true;
                    else if (_nextNOE_Status == NOE_Status.Commissioning_Engineer && !System_Environment.HasPrivilege(PrivilegeTypeID.NOE_Status_CommissioningEngineer))
                        isError = true;
                    else if (_nextNOE_Status == NOE_Status.Construction_Manager && !System_Environment.HasPrivilege(PrivilegeTypeID.NOE_Status_ConstructionManager))
                        isError = true;
                    else if (_nextNOE_Status == NOE_Status.Authorised_Isolation_Officer && !System_Environment.HasPrivilege(PrivilegeTypeID.NOE_Status_AuthorisationIsolationOfficer))
                        isError = true;
                    else if (_nextNOE_Status == NOE_Status.Commissioning_Manager && !System_Environment.HasPrivilege(PrivilegeTypeID.NOE_Status_CommissioningManager))
                        isError = true;

                    if (isError)
                    {
                        ProjectCommon.Common.Warn("You are not authorised to approve certificate as a " + certificateStatusString);
                        return;
                    }
                }


                if ((isCVC && _nextCVC_Status == CVC_Status.Client) || (isNOE && _nextNOE_Status == NOE_Status.Commissioning_Manager))
                {
                    if (!ProjectCommon.Common.Confirmation("Are you sure you want to mark this certificate as approved by " + certificateStatusString + "?\n\nThis will remove highlights and certain interactable in the document", "Approve Certificate"))
                        return;

                    customRichEdit1.Color_Interactables(false, false, HighlightType.Both); //take out color during approval
                    customRichEdit1.Trim_Redundant_Rows(); //remove insert line
                    customRichEdit1.Trim_Redundant_Interactables(); //remove redundant interactables
                }
                else if (!ProjectCommon.Common.Confirmation("Are you sure you want to mark this certificate as approved by " + certificateStatusString + "?", "Approve Certificate"))
                    return;

                int certificateStatus = isCVC ? (int)_nextCVC_Status : (int)_nextNOE_Status;
                ChangeStatusWithUpdateOverriding(_viewModel_Certificate.GUID, Guid.NewGuid(), CertificateStatusChange.Increase, GetComments());
            }
            else if (isPunchlistWalkdown)
            {
                if (!checkPunchlistWalkdownAuthorisation(true))
                    return;
            }

            saveCertificate();
            this.Close();
        }

        private bool checkPunchlistWalkdownAuthorisation(bool increaseCertificateStatus)
        {
            if (isPunchlistWalkdown)
            {
                if (_nextPunchlist_Status == PunchlistWalkdown_Status.Exported && !System_Environment.HasPrivilege(PrivilegeTypeID.PunchlistWalkdown_Status_Exported))
                {
                    ProjectCommon.Common.Warn("You are not authorised to export punchlist walkdowns");
                    return false;
                }
                if (_nextPunchlist_Status == PunchlistWalkdown_Status.Closed && !System_Environment.HasPrivilege(PrivilegeTypeID.PunchlistWalkdown_Status_Closed))
                {
                    ProjectCommon.Common.Warn("You are not authorised to close punchlist walkdowns");
                    return false;
                }
                else if(_nextPunchlist_Status == PunchlistWalkdown_Status.Exported)
                {
                    if (!ProjectCommon.Common.Confirmation("Are you sure you want to export this punchlist walkdown? As this will mark it as exported", "Export Punchlist Walkdown"))
                        return false;

                    if(increaseCertificateStatus)
                        ChangeStatusWithUpdateOverriding(_viewModel_Certificate.GUID, Guid.NewGuid(), CertificateStatusChange.Increase, GetComments());
                }
                else if(_nextPunchlist_Status == PunchlistWalkdown_Status.Closed)
                {
                    if (!customRichEdit1.IsDocumentPictures())
                    {
                        ProjectCommon.Common.Warn("Please attach PDF to document before closing");
                        return false;
                    }

                    if (!ProjectCommon.Common.Confirmation("Are you sure you want to mark this certificate as closed?\n\nThis action is not reversible", "Close Certificate"))
                        return false;

                    if(increaseCertificateStatus)
                        ChangeStatusWithUpdateOverriding(_viewModel_Certificate.GUID, Guid.NewGuid(), CertificateStatusChange.Increase, GetComments());

                    customRichEdit1.Color_Interactables(false, false, HighlightType.Both); //take out color during approval
                    customRichEdit1.Trim_Redundant_Rows(); //remove insert line
                    customRichEdit1.Trim_Redundant_Interactables(); //remove redundant interactables
                }
            }

            return true;
        }

        private string GetComments()
        {
            string Comment = null;
            if (_viewModel_Certificate.ImageIndex == 5 || _viewModel_Certificate.ImageIndex == 7) //need to ask for comments when progressing rejected ITRs
            {
                if (Comment == null)
                {
                    frmCertificateComment f = new frmCertificateComment(_viewModel_Certificate, btnProgress.Text);
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        Comment = f.GetComment();
                    }
                    else
                        return Comment;
                }
            }

            return Comment;
        }

        /// <summary>
        /// Populate user signatures based on ITR status
        /// </summary>
        private void PopulateSignatures()
        {
            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            customRichEdit1.Document.CancelUpdateRangePermissions(rangePermissions);

            using(AdapterCERTIFICATE_STATUS daCERTIFICATE_STATUS = new AdapterCERTIFICATE_STATUS())
            {
                dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drCERTIFICATE_STATUS = daCERTIFICATE_STATUS.GetLatestStatusBy(_viewModel_Certificate.GUID);
                dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCERTIFICATE_STATUS = daCERTIFICATE_STATUS.GetAllExcludeRejected(_viewModel_Certificate.GUID);
                if (dtCERTIFICATE_STATUS == null)
                    return;

                int defaultSignatureWidth = 135;
                int defaultSignatureHeight = 135;
                List<SignatureUser> signature_users = SignatureUserHelper.GetCertificateSignatureUser(dtCERTIFICATE_STATUS, (CVC_Status)drCERTIFICATE_STATUS.STATUS_NUMBER);
                foreach (RangePermission rangePermission in rangePermissions)
                {
                    if (rangePermission.Group == CustomUserGroupListService.SIGNATURE_BLOCK)
                    {
                        TableCell tCell = customRichEdit1.Document.Tables.GetTableCell(rangePermission.Range.Start);
                        if (tCell == null)
                            return;

                        Table tblSignature = tCell.Table;
                        tblSignature.ForEachCell(new TableCellProcessorDelegate(customRichEdit1.ClearCell));

                        bool isCommisioningManagerSignatureColumnExists = true;

                        try
                        {
                            isCommisioningManagerSignatureColumnExists = tblSignature.Cell(1, 4) != null;
                        }
                        catch
                        {
                            isCommisioningManagerSignatureColumnExists = false;
                        }

                        //if (isClientSignatureColumnExists && customRichEdit1.Document.GetText(tblSignature.Cell(1, 4).Range).Contains(Variables.General_NotApplicable) && btnProgress.Text.Contains("Close"))
                        //    btnProgress.Enabled = false;

                        SignatureUser supervisor = signature_users.FirstOrDefault(x => x.SignCertificateStatus == CVC_Status.Supervisor);
                        SignatureUser constructionManager = signature_users.FirstOrDefault(x => x.SignCertificateStatus == CVC_Status.Construction_Manager);
                        SignatureUser client = signature_users.FirstOrDefault(x => x.SignCertificateStatus == CVC_Status.Client);
                        SignatureUser commissioningManager = signature_users.FirstOrDefault(x => x.SignCertificateStatus == CVC_Status.Commissioning_Manager);

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

                        try
                        {
                            if (supervisor != null)
                            {
                                if (supervisor.Signature != null)
                                {
                                    Bitmap resizedSignature = ProjectCommon.Common.ResizeBitmap(supervisor.Signature, defaultSignatureWidth, defaultSignatureHeight);
                                    customRichEdit1.Document.Delete(tblSignature.Cell(1, 1).ContentRange);
                                    customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 1).Range.Start, resizedSignature);
                                }

                                customRichEdit1.Document.Delete(tblSignature.Cell(2, 1).ContentRange);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(2, 1).Range.Start, supervisor.Name);
                                customRichEdit1.Document.Delete(tblSignature.Cell(3, 1).ContentRange);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(3, 1).Range.Start, supervisor.Company);
                                customRichEdit1.Document.Delete(tblSignature.Cell(4, 1).ContentRange);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(4, 1).Range.Start, supervisor.SignDate.Year == 1 ? string.Empty : supervisor.SignDate.ToString("d"));
                            }

                            if (commissioningManager != null)
                            {
                                if (commissioningManager.Signature != null)
                                {
                                    Bitmap resizedSignature = ProjectCommon.Common.ResizeBitmap(commissioningManager.Signature, defaultSignatureWidth, defaultSignatureHeight);
                                    customRichEdit1.Document.Delete(tblSignature.Cell(1, 2).ContentRange);
                                    customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 2).Range.Start, resizedSignature);
                                }

                                customRichEdit1.Document.Delete(tblSignature.Cell(2, 2).ContentRange);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(2, 2).Range.Start, commissioningManager.Name);
                                customRichEdit1.Document.Delete(tblSignature.Cell(3, 2).ContentRange);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(3, 2).Range.Start, commissioningManager.Company);
                                customRichEdit1.Document.Delete(tblSignature.Cell(4, 2).ContentRange);
                                customRichEdit1.Document.InsertText(tblSignature.Cell(4, 2).Range.Start, commissioningManager.SignDate.Year == 1 ? string.Empty : commissioningManager.SignDate.ToString("d"));
                            }

                            if (constructionManager != null)
                            {
                                if (constructionManager != null)
                                {
                                    if (constructionManager.Signature != null)
                                    {
                                        Bitmap resizedSignature = ProjectCommon.Common.ResizeBitmap(constructionManager.Signature, defaultSignatureWidth, defaultSignatureHeight);
                                        customRichEdit1.Document.Delete(tblSignature.Cell(1, 3).ContentRange);
                                        customRichEdit1.Document.Images.Insert(tblSignature.Cell(1, 3).Range.Start, resizedSignature);
                                    }

                                    customRichEdit1.Document.Delete(tblSignature.Cell(2, 3).ContentRange);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(2, 3).Range.Start, constructionManager.Name);
                                    customRichEdit1.Document.Delete(tblSignature.Cell(3, 3).ContentRange);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(3, 3).Range.Start, constructionManager.Company);
                                    customRichEdit1.Document.Delete(tblSignature.Cell(4, 3).ContentRange);
                                    customRichEdit1.Document.InsertText(tblSignature.Cell(4, 3).Range.Start, constructionManager.SignDate.Year == 1 ? string.Empty : constructionManager.SignDate.ToString("d"));
                                }
                            }

                            if (client != null)
                            {
                                if (client.Signature != null)
                                {
                                    Bitmap resizedSignature = ProjectCommon.Common.ResizeBitmap(client.Signature, defaultSignatureWidth, defaultSignatureHeight);
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
    }
}