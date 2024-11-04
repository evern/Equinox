using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraEditors; //for punchlist combobox
using DevExpress.XtraEditors.Controls; //for punchlist combobox
using DevExpress.Office;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using System.Drawing;
using System.Threading;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.Xpf.Editors;
using DevExpress.XtraEditors.Camera;
using System.Text.RegularExpressions;
using CheckmateDX.Report;

namespace CheckmateDX
{
    public class CustomRichEdit : DevExpress.XtraRichEdit.RichEditControl
    {
        float _touchUIFontSize = 9.0f;
        float _touchUIRowHeight = 70.0f;
        float _nonTouchUIFontSize = 8.0f;
        float _nonTouchUIRowHeight = 50.0f;
        public Guid? _templateGuid;
        public Guid? _wbsTagGuid;

        public Guid _certificateGuid;
        public Guid _projectGuid;
        public bool _AllowPermissionColoring = true;

        public dsITR_MAIN.ITR_MAINDataTable _dtDeletedITRs = new dsITR_MAIN.ITR_MAINDataTable();
        SignatureUser currentSignatureUser;
        public CustomRichEdit()
            : base()
        {
        }

        public void SetSignatureUser(SignatureUser signatureUser)
        {
            currentSignatureUser = signatureUser;
        }

        #region Insertion
        /// <summary>
        /// Perform Permission Insertion on Selected Table
        /// </summary>
        /// <param name="stick">Allow permission to not leave a space behind to aid touch interaction</param>
        public void InsertPermissionForTable(string permissionName, string text, bool stick = false, string fontName = "", int? fontSize = null)
        {
            SelectionCollection Selections = this.Document.Selections;
            this.Document.BeginUpdate();
            foreach (DocumentRange Selection in Selections)
            {
                TableCell tCell = this.Document.Tables.GetTableCell(Selection.Start);

                if (tCell != null)
                {
                    if (this.Document.GetText(tCell.ContentRange).Trim() == string.Empty)
                        this.Document.Replace(tCell.ContentRange, string.Empty);

                    // Obtain character properties
                    CharacterProperties cp = this.Document.BeginUpdateCharacters(tCell.Range);

                    if(fontName != string.Empty)
                        cp.FontName = fontName;

                    if(fontSize != null)
                        cp.FontSize = (int)fontSize;

                    // Finalize modifications
                    this.Document.EndUpdateCharacters(cp);

                    InsertRowPermission(permissionName, tCell.Range.Start.ToInt(), Selection.End.ToInt(), stick, string.Empty);
                }
            }

            this.Document.EndUpdate();
        }

        /// <summary>
        /// Perform Permission Insertion on Selected Row
        /// </summary>
        /// <param name="Stick">Whether to not leave a space to aid touch interaction</param>
        public void InsertRowPermission(string PermissionName, int StartPos, int EndPos, bool Stick, string TextBlock = "")
        {
            this.Document.BeginUpdate();
            int cellStartPosition = StartPos;
            int selectionEndPosition = EndPos;
            do
            {
                DocumentPosition rowPosition = this.Document.CreatePosition(cellStartPosition);
                TableCell rowCell = this.Document.Tables.GetTableCell(rowPosition);
                int originalCellEndPos = rowCell.ContentRange.End.ToInt();
                if (rowCell != null)
                {
                    InsertCellPermission(PermissionName, rowCell, Stick, TextBlock);
                    int charactersAdded = (rowCell.ContentRange.End.ToInt() - originalCellEndPos);
                    selectionEndPosition += charactersAdded;

                    cellStartPosition = rowCell.ContentRange.End.ToInt() + 1; //Add 1 to move to next cell
                }
                else
                    cellStartPosition += 1;
            } while (cellStartPosition < selectionEndPosition);
            this.Document.EndUpdate();
        }

        /// <summary>
        /// Perform Permission Insertion on Selected Cell
        /// </summary>
        /// <param name="Stick">Whether permission should not leave a space behind to aid touch interaction</param>
        /// <param name="RecolorPermission">Whether cell background and permission character property should be recolored</param>
        public void InsertCellPermission(string PermissionName, TableCell tCell, bool Stick, string TextBlock = "")
        {
            this.Document.BeginUpdate();
            //Determine the cell horizontal alignment
            ParagraphProperties pp = this.Document.BeginUpdateParagraphs(tCell.ContentRange);
            ParagraphAlignment pAlign = pp.Alignment == null ? ParagraphAlignment.Left : (ParagraphAlignment)pp.Alignment;
            this.Document.EndUpdateParagraphs(pp);

            bool isEmpty = false;
            if (this.Document.GetText(tCell.ContentRange).Trim() == string.Empty)
            {
                isEmpty = true;
                this.Document.Replace(tCell.ContentRange, string.Empty);
            }

            DocumentRange permissionInsertRange;
            if (pAlign != ParagraphAlignment.Right)
            {
                DocumentPosition permissionInsertPos;
                int UnstickToPermission = 0;
                if (!isEmpty)
                    this.Document.InsertText(tCell.ContentRange.End, " ");
                else if (pAlign != ParagraphAlignment.Left && TextBlock == string.Empty)
                {
                    ParagraphProperties cellPP = this.Document.BeginUpdateParagraphs(tCell.Range);
                    cellPP.Alignment = ParagraphAlignment.Left;
                    this.Document.EndUpdateParagraphs(cellPP);
                }

                //if (TextBlock != string.Empty && !Stick)
                //Empty textblock needs a space to be assigned to
                if (TextBlock == string.Empty)
                    TextBlock = TextBlock + " ";

                if (!Stick)
                {
                    UnstickToPermission = 1;
                    TextBlock = TextBlock + " ";
                }

                this.Document.InsertText(tCell.ContentRange.End, TextBlock);

                //Mark characters as bold
                int RangePosition = tCell.ContentRange.End.ToInt() - TextBlock.Length;
                if (RangePosition < 0)
                    RangePosition = 0;

                DocumentRange CharacterFormatRange = this.Document.CreateRange(RangePosition, TextBlock.Length - UnstickToPermission);
                CharacterProperties cellCP = this.Document.BeginUpdateCharacters(CharacterFormatRange);
                cellCP.Bold = true;
                this.Document.EndUpdateCharacters(cellCP);

                permissionInsertPos = this.Document.CreatePosition(tCell.ContentRange.End.ToInt() - TextBlock.Length);

                //UnstickToPermission dictates whether the last space in the permission is included
                permissionInsertRange = this.Document.CreateRange(permissionInsertPos, TextBlock.Length - UnstickToPermission);
            }
            else
            {
                DocumentPosition permissionInsertPos;
                int UnstickToPermission = 0;

                if (!isEmpty)
                    this.Document.InsertText(tCell.ContentRange.Start, " ");

                //if (TextBlock != string.Empty)
                //Empty textblock needs a space to be assigned to
                if (TextBlock == string.Empty)
                    TextBlock = TextBlock + " ";

                if (!Stick)
                {
                    UnstickToPermission = 1;
                    TextBlock = TextBlock + " ";
                }

                this.Document.InsertText(tCell.ContentRange.Start, TextBlock);

                //Mark characters as bold
                DocumentRange CharacterFormatRange = this.Document.CreateRange(tCell.ContentRange.Start.ToInt() + 1, TextBlock.Length - UnstickToPermission);
                CharacterProperties cellCP = this.Document.BeginUpdateCharacters(CharacterFormatRange);
                cellCP.Bold = true;
                this.Document.EndUpdateCharacters(cellCP);

                permissionInsertPos = this.Document.CreatePosition(tCell.ContentRange.Start.ToInt() + UnstickToPermission);
                permissionInsertRange = this.Document.CreateRange(permissionInsertPos, TextBlock.Length - UnstickToPermission);
            }

            RangePermission InsertedRP = InsertRangePermission(PermissionName, permissionInsertRange);
            //During row addition character property format is copied, but the unstick character's backcolor should be transparent
            if (_AllowPermissionColoring)
            {
                CharacterProperties cpCellContent = this.Document.BeginUpdateCharacters(tCell.ContentRange);
                cpCellContent.BackColor = Color.Transparent;
                cpCellContent.ForeColor = Color.Black;
                this.Document.EndUpdateCharacters(cpCellContent);
                Color_One_Interactable(InsertedRP, true, HighlightType.Both);
            }

            this.Document.EndUpdate();
        }

        /// <summary>
        /// Perform Permission Insertion on Document Range
        /// </summary>
        public RangePermission InsertRangePermission(string GroupName, DocumentRange DocRange)
        {
            this.Document.BeginUpdate();
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();

            RangePermission rp = rangePermissions.CreateRangePermission(DocRange);
            rp.Group = GroupName;
            rangePermissions.Add(rp);
            this.Document.EndUpdateRangePermissions(rangePermissions);
            this.Document.EndUpdate();

            return rp;
        }

        /// <summary>
        /// Populate pre-fill fields
        /// </summary>
        public void PopulatePrefill(Tag tag, WBS wbs, ViewModel_Certificate certificate, string documentName, string documentRevision, bool isPunchlist)
        {
            this.Document.BeginUpdate();
            this.Options.MailMerge.ViewMergedData = false;
            DataTable prefillSourceTable = new DataTable();
            List<string> uniqueHeaders = new List<string>();
            //add system default header
            uniqueHeaders.Add(Variables.prefillTagNumber);
            uniqueHeaders.Add(Variables.prefillTagDescription);
            uniqueHeaders.Add(Variables.prefillProjNumber);
            uniqueHeaders.Add(Variables.prefillProjName);
            uniqueHeaders.Add(Variables.prefillProjClient);
            uniqueHeaders.Add(Variables.prefillDocumentName);
            uniqueHeaders.Add(Variables.prefillTaskNumber);
            uniqueHeaders.Add(Variables.prefillDate);
            uniqueHeaders.Add(Variables.prefillDateTime);
            uniqueHeaders.Add(Variables.prefillChild);
            uniqueHeaders.Add(Variables.prefillCertificateNumber);
            uniqueHeaders.Add(Variables.prefillCertificateDescription);

            Guid searchGuid;
            PrefillType prefilltype;

            dsPROJECT.PROJECTRow drProject;
            AdapterPROJECT daProject = new AdapterPROJECT();
            AdapterPREFILL_REGISTER daPrefillRegister = new AdapterPREFILL_REGISTER();

            try
            {
                if (tag != null)
                {
                    searchGuid = tag.GUID;
                    prefilltype = PrefillType.Tag;
                    drProject = daProject.GetByTag(tag.GUID, isPunchlist);
                }
                else if (wbs != null)
                {
                    searchGuid = wbs.GUID;
                    prefilltype = PrefillType.WBS;
                    drProject = daProject.GetByWBS(wbs.GUID, isPunchlist);
                }
                else if(certificate != null)
                {
                    drProject = daProject.GetByCertificate(certificate.GUID);
                    prefilltype = PrefillType.Certificate;
                    searchGuid = Guid.Empty;
                }
                else
                {
                    drProject = null;
                    searchGuid = Guid.Empty;
                    prefilltype = PrefillType.Multiple;
                }

                //final attempt to retrieve project by environment
                if (drProject == null)
                    drProject = daProject.GetBy((Guid)System_Environment.GetUser().userProject.Value);

                //Get fields from document
                FieldCollection fields = this.Document.Fields;
                foreach (Field field in fields)
                {
                    string sPrefill = this.Document.GetText(field.Range);
                    sPrefill = sPrefill.Replace("<<", string.Empty);
                    sPrefill = sPrefill.Replace(">>", string.Empty);
                    if (uniqueHeaders.IndexOf(sPrefill) == -1)
                    {
                        //store unique header for spreadsheet
                        uniqueHeaders.Add(sPrefill);
                    }
                }

                //Get fields from header
                Section firstSection = this.Document.Sections[0];

                SubDocument headerDoc = firstSection.BeginUpdateHeader();
                for (int i = 0; i < headerDoc.Fields.Count; i++)
                {
                    string sPrefill = headerDoc.GetText(headerDoc.Fields[i].Range);
                    sPrefill = sPrefill.Replace("<<", string.Empty);
                    sPrefill = sPrefill.Replace(">>", string.Empty);
                    if (uniqueHeaders.IndexOf(sPrefill) == -1)
                    {
                        uniqueHeaders.Add(sPrefill);
                    }
                }
                this.Document.Sections[0].EndUpdateHeader(headerDoc);

                //Get fields from footer
                SubDocument footerDoc = firstSection.BeginUpdateFooter();
                for (int i = 0; i < footerDoc.Fields.Count; i++)
                {
                    string sPrefill = footerDoc.GetText(footerDoc.Fields[i].Range);
                    sPrefill = sPrefill.Replace("<<", string.Empty);
                    sPrefill = sPrefill.Replace(">>", string.Empty);
                    if (uniqueHeaders.IndexOf(sPrefill) == -1)
                    {
                        uniqueHeaders.Add(sPrefill);
                    }
                }
                this.Document.Sections[0].EndUpdateFooter(footerDoc);

                //Construct the schema
                foreach (string uniqueHeader in uniqueHeaders)
                {
                    prefillSourceTable.Columns.Add(uniqueHeader);
                }

                //Population is still possible in case prefill has been deleted
                dsPREFILL_REGISTER.PREFILL_REGISTERDataTable dtWBSTagPrefill = daPrefillRegister.GetByWBSTag(searchGuid);
                if (dtWBSTagPrefill != null)
                {
                    foreach (dsPREFILL_REGISTER.PREFILL_REGISTERRow drWBSTagPrefill in dtWBSTagPrefill.Rows)
                    {
                        if (!prefillSourceTable.Columns.Contains(drWBSTagPrefill.NAME))
                            prefillSourceTable.Columns.Add(drWBSTagPrefill.NAME);
                    }
                }

                DataRow drPrefillSource = prefillSourceTable.NewRow();
                for (int i = 0; i < prefillSourceTable.Columns.Count; i++)
                {
                    if (prefilltype == PrefillType.Multiple)
                        drPrefillSource[i] = Variables.Multiple_Prefill;
                    else
                        drPrefillSource[i] = Variables.Empty_Prefill;
                }

                if (prefilltype == PrefillType.Tag)
                {
                    //add system default data
                    drPrefillSource[Variables.prefillTagNumber] = tag.tagNumber;
                    drPrefillSource[Variables.prefillTagDescription] = tag.tagDescription;
                }
                else if (prefilltype == PrefillType.WBS)
                {
                    //add system default data
                    drPrefillSource[Variables.prefillTagNumber] = wbs.wbsName;
                    drPrefillSource[Variables.prefillTagDescription] = wbs.wbsDescription;
                }
                else if (prefilltype == PrefillType.Certificate)
                {                    //add system default data
                    drPrefillSource[Variables.prefillCertificateNumber] = certificate.Number;
                    drPrefillSource[Variables.prefillCertificateDescription] = certificate.Description;
                }

                if (drProject != null)
                {
                    //Revision 1.0.0.172 - Remove project specific number from document template
                    string GeneralProjectNumber = drProject.NUMBER;
                    //Revision 1.0.0.176 - Use project specific number
                    //string GeneralProjectNumber = "00000";
                    drPrefillSource[Variables.prefillProjNumber] = drProject.NUMBER;
                    drPrefillSource[Variables.prefillProjName] = drProject.IsNAMENull() ? Variables.Empty_Prefill : drProject.NAME;
                    drPrefillSource[Variables.prefillProjClient] = drProject.IsCLIENTNull() ? Variables.Empty_Prefill : drProject.CLIENT;
                    drPrefillSource[Variables.prefillDocumentName] = GeneralProjectNumber + "_" + documentName + "_" + documentRevision;

                    if(tag != null)
                        drPrefillSource[Variables.prefillTaskNumber] = Common.GetStringHash(string.Concat(tag.tagNumber, documentName));
                }

                if (prefilltype != PrefillType.Multiple && dtWBSTagPrefill != null)
                {
                    foreach (dsPREFILL_REGISTER.PREFILL_REGISTERRow drWBSTagPrefill in dtWBSTagPrefill.Rows)
                    {
                        if (drWBSTagPrefill.DATA != string.Empty)
                            drPrefillSource[drWBSTagPrefill.NAME] = drWBSTagPrefill.DATA;
                    }
                }

                drPrefillSource[Variables.prefillDate] = DateTime.Now.GetDateTimeFormats('D')[0];
                drPrefillSource[Variables.prefillDateTime] = DateTime.Now.GetDateTimeFormats('f')[0];

                if (_wbsTagGuid != null)
                {
                    string concatenatedChildrenTagNumbers = string.Empty;
                    using (AdapterTAG daTag = new AdapterTAG())
                    {
                        //get the tag childrens because their respective parent WBS guid will be change in tandem to the tag parent
                        dsTAG.TAGDataTable dtTagChildrens = daTag.GetTagChildrens((Guid)_wbsTagGuid, false);
                        if (dtTagChildrens != null)
                        {

                            List<string> tagNumbers = new List<string>();
                            //update the tag children's WBS parent to match tag parent
                            foreach (dsTAG.TAGRow drTagChildren in dtTagChildrens.Rows)
                            {
                                tagNumbers.Add(drTagChildren.NUMBER);
                            }

                            tagNumbers = tagNumbers.OrderBy(x => x).ToList();
                            foreach (string tagNumber in tagNumbers)
                            {
                                concatenatedChildrenTagNumbers += tagNumber + ", ";
                            }
                        }
                    }

                    //using (AdapterWBS daWBS = new AdapterWBS())
                    //{
                    //    //get the tag childrens because their respective parent WBS guid will be change in tandem to the tag parent
                    //    dsWBS.WBSDataTable dtWBSChildrens = daWBS.GetWBSChildrens((Guid)_wbsTagGuid, false);
                    //    if (dtWBSChildrens != null)
                    //    {
                    //        List<string> wbsNames = new List<string>();
                    //        //update the tag children's WBS parent to match tag parent
                    //        foreach (dsWBS.WBSRow drTagChildren in dtWBSChildrens.Rows)
                    //        {
                    //            wbsNames.Add(drTagChildren.NAME);
                    //        }

                    //        wbsNames = wbsNames.OrderBy(x => x).ToList();
                    //        foreach (string wbsName in wbsNames)
                    //        {
                    //            concatenatedChildrenTagNumbers += wbsName + ", ";
                    //        }
                    //    }
                    //}

                    if (concatenatedChildrenTagNumbers != string.Empty)
                    {
                        concatenatedChildrenTagNumbers = concatenatedChildrenTagNumbers.Substring(0, concatenatedChildrenTagNumbers.Length - 2);
                        drPrefillSource[Variables.prefillChild] = concatenatedChildrenTagNumbers;
                    }
                }

                prefillSourceTable.Rows.Add(drPrefillSource);
                this.Options.MailMerge.DataSource = prefillSourceTable;
                this.Options.MailMerge.ViewMergedData = true;
            }
            finally
            {
                daProject.Dispose();
                daPrefillRegister.Dispose();
            }

            this.Document.EndUpdate();
        }
        #endregion

        #region Formatting
        /// <summary>
        /// Enable/disable TouchUI for tables and text in document
        /// </summary>
        public void Set_TouchUI(bool Enabled)
        {
            this.Document.BeginUpdate();
            foreach (Table tbl in this.Document.Tables)
            {
                if (Enabled)
                    tbl.ForEachRow(TableRowProcessor_TouchUI);
                else
                    tbl.ForEachRow(TableRowProcessor_NonTouchUI);
            }

            //Get fields from header
            Section firstSection = this.Document.Sections[0];

            SubDocument headerDoc = firstSection.BeginUpdateHeader();
            foreach (Table tbl in headerDoc.Tables)
            {
                if (Enabled)
                    tbl.ForEachRow(TableRowProcessor_HeaderTouchUI);
                else
                    tbl.ForEachRow(TableRowProcessor_HeaderNonTouchUI);
            }
            firstSection.EndUpdateHeader(headerDoc);

            SubDocument footerDoc = firstSection.BeginUpdateFooter();
            foreach (Table tbl in footerDoc.Tables)
            {
                if (Enabled)
                    tbl.ForEachRow(TableRowProcessor_FooterTouchUI);
                else
                    tbl.ForEachRow(TableRowProcessor_FooterNonTouchUI);
            }
            firstSection.EndUpdateFooter(footerDoc);

            this.Document.EndUpdate();
        }

        public void CleanUpQRCodes()
        {
            List<DocumentRange> remove_range = new List<DocumentRange>();
            foreach (var shape in this.Document.Shapes)
            {
                remove_range.Add(shape.Range);
            }

            for (int i = 0; i < remove_range.Count; i++)
            {
                this.Document.Delete(remove_range[i]);
            }
        }

        public void RemoveHeaderQRCode()
        {
            List<DocumentRange> remove_range = new List<DocumentRange>();
            Image imageForSizing = BarCodeUtility.GetQRCode(string.Empty);

            //when it's inserted the height and width got scaled by half
            int qrCodeHeight = imageForSizing.Size.Height / 2;
            int qrCodeWidth = imageForSizing.Size.Height / 2;
            foreach (var shape in this.Document.Shapes)
            {
                if (shape.Size.Height == qrCodeHeight && shape.Size.Width == qrCodeWidth)
                    remove_range.Add(shape.Range);
            }

            for (int i = 0; i < remove_range.Count; i++)
            {
                this.Document.Delete(remove_range[i]);
            }
        }

        public void InsertHeaderQRCode(string meta_tag)
        {
            this.Document.Unit = DocumentUnit.Centimeter;

            DevExpress.XtraRichEdit.Services.Implementation.IDocumentLayoutService layoutService = this.GetService<DevExpress.XtraRichEdit.Services.Implementation.IDocumentLayoutService>();
            if (layoutService != null)
            {
                DevExpress.XtraRichEdit.Layout.DocumentLayout layout = layoutService.CalculateDocumentLayout();
                DevExpress.XtraRichEdit.Model.DocumentModel model = this.Model;
                LayoutPage current;
                List<DocumentRange> remove_range = new List<DocumentRange>();
                Image imageForSizing = BarCodeUtility.GetQRCode(string.Empty);
                //when it's inserted the height and width got scaled by half
                int qrCodeHeight = imageForSizing.Size.Height / 2;
                int qrCodeWidth = imageForSizing.Size.Height / 2;
                foreach (var shape in this.Document.Shapes)
                {
                    if(shape.Size.Height == qrCodeHeight && shape.Size.Width == qrCodeWidth)
                        remove_range.Add(shape.Range);
                }

                for (int i = 0; i < remove_range.Count; i++)
                {
                    this.Document.Delete(remove_range[i]);
                }

                if (layout.Pages.Count > 0)
                {
                    for (int i = 0; i < layout.Pages.Count; i++)
                    {
                        current = this.DocumentLayout.GetPage(i);
                        if (current == null)
                            break;

                        string codeMeta = meta_tag + ";" + (i + 1).ToString() + ";" + layout.Pages.Count;
                        Image barCodeImage = BarCodeUtility.GetQRCode(codeMeta);
                        FixedRange frange = current.PageAreas[0].Range;
                        //Image resizedQRCode = Common.ScaleImage(QRCode, 78, 80);
                        Shape myPicture = this.Document.Shapes.InsertPicture(this.Document.CreatePosition(frange.Start), barCodeImage);
                        myPicture.HorizontalAlignment = ShapeHorizontalAlignment.Right;
                        myPicture.VerticalAlignment = ShapeVerticalAlignment.Top;
                        // Specify the reference item for positioning.
                        //myPicture.RelativeHorizontalPosition = ShapeRelativeHorizontalPosition.RightMargin;
                        //myPicture.RelativeVerticalPosition = ShapeRelativeVerticalPosition.TopMargin;
                        //myPicture.Offset = new PointF(0f, 0f);
                    }
                }
            }

            this.Document.Unit = DocumentUnit.Document;
        }

        public void InsertSignatureQRCode()
        {
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);

            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group == CustomUserGroupListService.SIGNATURE_BLOCK)
                {
                    TableCell tCell = this.Document.Tables.GetTableCell(rangePermission.Range.Start);
                    Table tblSignature = tCell.Table;

                    //this.Document.BeginUpdate();
                    Image QRCode = BarCodeUtility.GetQRCode("SIGNATURE");
                    //Image resized_QRCode = Common.ScaleImage(QRCode, 100, 70);

                    this.Document.Unprotect();
                    ReadOnlyDocumentImageCollection images = this.Document.Images.Get(tblSignature.Cell(0, 0).Range);
                    //if (images.Count > 0)
                    //{
                    //    foreach (var image in images)
                    //    {
                    //        this.Document.Delete(image.Range);
                    //    }
                    //}

                    TableCell tableCell = tblSignature.Cell(0, 0);
                    tableCell.LeftPadding = 0;
                    tableCell.TopPadding = 0;
                    tableCell.VerticalAlignment = TableCellVerticalAlignment.Top;

                    //if(!isRemove)
                    //{
                    if (images.Count == 0)
                        this.Document.Images.Insert(tableCell.Range.Start, QRCode);
                    //}

                    //this.Document.Protect(string.Empty);
                    //this.Document.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Detect whether there are rows that span across pages
        /// </summary>
        /// <returns>Returns first few characters of the erronouse row or empty string if there are no rows than span across pages</returns>
        public string IsAnyRowSpansAcrossPages()
        {
            DevExpress.XtraRichEdit.Services.Implementation.IDocumentLayoutService layoutService = this.GetService<DevExpress.XtraRichEdit.Services.Implementation.IDocumentLayoutService>();
            if (layoutService != null)
            {
                DevExpress.XtraRichEdit.Layout.DocumentLayout layout = layoutService.CalculateDocumentLayout();
                DevExpress.XtraRichEdit.Model.DocumentModel model = this.Model;
                LayoutPage currentPage;
                List<DocumentRange> rowRanges = new List<DocumentRange>();
                foreach (var table in this.Document.Tables)
                {
                    foreach(var row in table.Rows)
                    {
                        rowRanges.Add(row.Range);
                    }
                }

                if (layout.Pages.Count > 0)
                {
                    for (int i = 0; i < layout.Pages.Count; i++)
                    {
                        int threshold = ((DevExpress.Utils.IConvertToInt<DevExpress.XtraRichEdit.Model.DocumentLogPosition>)layout.Pages[i].GetFirstPosition(model.MainPieceTable).LogPosition).ToInt();

                        foreach(DocumentRange rowRange in rowRanges)
                        {
                            if (rowRange.Start.ToInt() < threshold && rowRange.End.ToInt() > threshold) //indicates the table is spanning across pages
                            {
                                TableCell tCell = this.Document.Tables.GetTableCell(rowRange.Start);
                                TableRow tRow = tCell.Row;

                                foreach(TableCell tRowCell in tRow.Cells)
                                {
                                    string cellContent = this.Document.GetText(tRowCell.Range);
                                    string clearCellContent = cellContent.Replace("\r\n", string.Empty);

                                    if (clearCellContent != string.Empty)
                                    {
                                        if (clearCellContent.Length >= 20)
                                            return clearCellContent.Substring(0, 20);
                                        else
                                            return clearCellContent.Substring(0, clearCellContent.Length);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        public bool IsDocumentPictures()
        {
            Document.Unprotect();

            bool isAllSectionsHaveFullSizePictures = true;
            foreach(var section in Document.Sections)
            {
                float pageWidth = section.Page.Width;
                float pageHeight = section.Page.Height;
                float sectionWidth = Units.DocumentsToPixelsF(pageWidth, DpiX);
                float sectionHeight = Units.DocumentsToPixelsF(pageHeight, DpiY);
                int intSectionWidth = Convert.ToInt32(sectionWidth);
                int intSectionHeight = Convert.ToInt32(sectionHeight);
                int sectionLongestEdge = intSectionWidth > intSectionHeight ? intSectionWidth : intSectionHeight;
                int sectionShortestEdge = intSectionWidth < intSectionHeight ? intSectionWidth : intSectionHeight;

                ReadOnlyDocumentImageCollection images = this.Document.Images.Get(section.Range);
                bool isFullSizeImageDetected = false;
                foreach (var documentImage in images)
                {
                    Image nativeImage = documentImage.Image.NativeImage;
                    int intImageLongestEdge = nativeImage.Height > nativeImage.Width ? nativeImage.Height : nativeImage.Width;
                    int intImageShortestEdge = nativeImage.Height < nativeImage.Width ? nativeImage.Height : nativeImage.Width;
                    //image orientation cannot be determined
                    if (intImageLongestEdge == sectionLongestEdge && sectionShortestEdge == intImageShortestEdge)
                    {
                        isFullSizeImageDetected = true;
                        break;
                    }
                }

                if (!isFullSizeImageDetected)
                    isAllSectionsHaveFullSizePictures = false;
            }

            Document.Protect(string.Empty);

            return isAllSectionsHaveFullSizePictures;
        }

        private string get_document_name_from_prefill()
        {
            if (this.Options.MailMerge.DataSource != null && this.Options.MailMerge.DataSource.GetType() == typeof(DataTable))
            {
                DataTable table = (DataTable)this.Options.MailMerge.DataSource;
                foreach (DataColumn tableColumn in table.Columns)
                {
                    if (tableColumn.ColumnName == Variables.prefillDocumentName)
                    {
                        return tableColumn.ToString();
                    }
                }
            }

            return string.Empty;
        }

        public void AcceptAllRevision()
        {
            //SubDocument header = this.Document.Sections[0].BeginUpdateHeader();
            //var headerRevisions = this.Document.Revisions.Get(header);
            //this.Document.Sections[0].EndUpdateHeader(header);
            //foreach (Revision headerRevision in headerRevisions)
            //    headerRevision.Accept();


            //SubDocument footer = this.Document.Sections[0].BeginUpdateFooter();
            //var footerRevisions = this.Document.Revisions.Get(header);
            //this.Document.Sections[0].EndUpdateFooter(footer);
            //foreach (Revision footerRevision in footerRevisions)
            //    footerRevision.Accept();

            var allRevisions = this.Document.Revisions;
            foreach (Revision allRevision in allRevisions)
                allRevision.Accept();


            DocumentTrackChangesOptions documentTrackChangesOptions = this.Document.TrackChanges;
            documentTrackChangesOptions.Enabled = false;
            documentTrackChangesOptions.TrackFormatting = false;
            documentTrackChangesOptions.TrackMoves = false;
        }

        /// <summary>
        /// Clear footer and insert document name and page number fields
        /// </summary>
        public void Populate_Footer_Field(object fontNameObj = null, object fontSizeObj = null)
        {
            try
            {
                string fontName = string.Empty;
                if (fontNameObj != null)
                    fontName = fontNameObj.ToString();

                int? fontSize = null;
                if (fontSizeObj != null)
                {
                    int parseFontSize;
                    if (Int32.TryParse(fontSizeObj.ToString(), out parseFontSize))
                        //somehow footer is always set to 1 size lower
                        fontSize = parseFontSize + 1;
                }

                this.Document.BeginUpdate();
                SubDocument footerDoc = this.Document.Sections[0].BeginUpdateFooter();
                footerDoc.Delete(footerDoc.Range);
                Table tFooter = footerDoc.Tables.Create(footerDoc.Range.Start, 1, 3, AutoFitBehaviorType.AutoFitToWindow);
                ParagraphProperties pp1 = footerDoc.BeginUpdateParagraphs(tFooter.Cell(0, 0).Range);
                pp1.Alignment = ParagraphAlignment.Left;
                ParagraphProperties pp2 = footerDoc.BeginUpdateParagraphs(tFooter.Cell(0, 2).Range);
                pp2.Alignment = ParagraphAlignment.Right;
                ParagraphProperties pp3 = footerDoc.BeginUpdateParagraphs(tFooter.Cell(0, 1).Range);
                pp3.Alignment = ParagraphAlignment.Center;

                CharacterProperties cp1 = footerDoc.BeginUpdateCharacters(tFooter.Cell(0, 0).ContentRange);
                CharacterProperties cp2 = footerDoc.BeginUpdateCharacters(tFooter.Cell(0, 2).ContentRange);
                CharacterProperties cp3 = footerDoc.BeginUpdateCharacters(tFooter.Cell(0, 1).ContentRange);

                if (fontName != string.Empty)
                {
                    cp1.FontName = fontName;
                    cp2.FontName = fontName;
                    cp3.FontName = fontName;
                }

                if (fontSize != null)
                {
                    cp1.FontSize = fontSize;
                    cp2.FontSize = fontSize;
                    cp3.FontSize = fontSize;
                }

                footerDoc.EndUpdateCharacters(cp1);
                footerDoc.EndUpdateCharacters(cp2);
                footerDoc.EndUpdateCharacters(cp3);

                footerDoc.EndUpdateParagraphs(pp1);
                footerDoc.EndUpdateParagraphs(pp2);
                footerDoc.EndUpdateParagraphs(pp3);

                footerDoc.Fields.Create(tFooter.Cell(0, 0).Range.Start, " MERGEFIELD \"" + Variables.prefillDocumentName + "\"");
                footerDoc.Fields.Create(tFooter.Cell(0, 1).Range.Start, " MERGEFIELD \"" + Variables.prefillTaskNumber + "\"");
                //footerDoc.InsertText(tFooter.Cell(0, 1).ContentRange.Start, "Task Id: ");
                footerDoc.InsertText(tFooter.Cell(0, 2).ContentRange.End, "Pages ");
                footerDoc.Fields.Create(tFooter.Cell(0, 2).ContentRange.End, "PAGE");
                footerDoc.InsertText(tFooter.Cell(0, 2).ContentRange.End, " of ");
                footerDoc.Fields.Create(tFooter.Cell(0, 2).ContentRange.End, "NUMPAGES");

                tFooter.ForEachCell(new TableCellProcessorDelegate(ClearBorders));

                this.Document.Sections[0].EndUpdateFooter(footerDoc);

                SubDocument footerDoc1 = this.Document.Sections[0].BeginUpdateFooter();
                this.Document.ChangeActiveDocument(footerDoc1);
                this.Document.CaretPosition = footerDoc1.Range.Start;
                new ToggleTableFixedColumnWidthCommand(this).Execute();
                tFooter.TableLayout = TableLayoutType.Autofit;
                tFooter.ForEachCell((cell, rowIndex, columnIndex) => {
                    cell.PreferredWidthType = WidthType.FiftiethsOfPercent;
                });

                tFooter.PreferredWidth = 5000;
                tFooter.PreferredWidthType = WidthType.FiftiethsOfPercent;

                this.Document.Sections[0].EndUpdateFooter(footerDoc1);

                this.Options.MailMerge.ViewMergedData = true;
                this.Options.MailMerge.ViewMergedData = false;
                this.Document.EndUpdate();
            }
            catch
            {
                MessageBox.Show("Populate footer failed, please close and reopen this document", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Try to parse header table and insert fields according to previous cell value
        /// </summary>
        public void Populate_Header_Field(object fontNameObj = null, object fontSizeObj = null)
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

            this.Document.BeginUpdate();
            SubDocument headerDoc = this.Document.Sections[0].BeginUpdateHeader();
            Table tblHeader = headerDoc.Tables[0];
            List<string> Prefill_Names = new List<string>();
            using (AdapterPREFILL_MAIN daPREFILL_MAIN = new AdapterPREFILL_MAIN())
            {
                dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL_MAIN = daPREFILL_MAIN.Get();
                foreach (dsPREFILL_MAIN.PREFILL_MAINRow drPREFILL_MAIN in dtPREFILL_MAIN.Rows)
                {
                    Prefill_Names.Add(drPREFILL_MAIN.NAME);
                }
            }

            using (AdapterTEMPLATE_TOGGLE daTEMPLATE_TOGGLE = new AdapterTEMPLATE_TOGGLE())
            {
                dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE = daTEMPLATE_TOGGLE.GetAll();
                foreach (dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow drTEMPLATE_TOGGLE in dtTEMPLATE_TOGGLE.Rows)
                {
                    Prefill_Names.Add(drTEMPLATE_TOGGLE.NAME);
                }
            }

            foreach (TableRow tRow in tblHeader.Rows)
            {
                foreach (TableCell tCell in tRow.Cells)
                {
                    string s = headerDoc.GetText(tCell.Range);
                    s = s.Replace("\r", string.Empty);
                    s = s.Replace("\n", string.Empty);
                    s = s.Trim();

                    List<string> DefaultPrefills = new List<string>();
                    DefaultPrefills.Add(Variables.prefillTagNumber);
                    DefaultPrefills.Add(Variables.prefillTagDescription);
                    DefaultPrefills.Add(Variables.prefillProjNumber);
                    DefaultPrefills.Add(Variables.prefillProjName);
                    DefaultPrefills.Add(Variables.prefillProjClient);
                    DefaultPrefills.Add(Variables.prefillDocumentName);
                    DefaultPrefills.Add(Variables.prefillDate);
                    DefaultPrefills.Add(Variables.prefillDateTime);
                    DefaultPrefills.Add(Variables.prefillChild);
                    DefaultPrefills.Add("#"); //page number

                    if (s == string.Empty || (s.Contains("<<") && s.Contains(">>")))
                    {
                        if (tCell.Previous != null)
                        {
                            string headerTitleText = headerDoc.GetText(tCell.Previous.Range);
                            headerTitleText = headerTitleText.Replace("No:", "Number");
                            headerTitleText = headerTitleText.Replace(":", string.Empty);
                            headerTitleText = headerTitleText.Replace("\r", string.Empty);
                            headerTitleText = headerTitleText.Replace("\n", string.Empty);
                            headerTitleText = headerTitleText.Trim();

                            if (headerTitleText != string.Empty)
                            {
                                //Replace commonly known values
                                if (headerTitleText.ToUpper().Contains("JOB NUMBER"))
                                    headerTitleText = Variables.prefillProjNumber;
                                else if (headerTitleText.ToUpper().Contains("PROJECT NAME"))
                                    headerTitleText = Variables.prefillProjName;
                                else if (headerTitleText.ToUpper().Contains("CLIENT"))
                                    headerTitleText = Variables.prefillProjClient;
                                else if (headerTitleText.ToUpper().Contains("DATA SHEET"))
                                    headerTitleText = "Datasheet";
                                else if (headerTitleText.ToUpper().Contains("TAG NO"))
                                    headerTitleText = Variables.prefillTagNumber;
                                else if (headerTitleText.ToUpper().Contains("SUB-SYSTEM NO"))
                                    headerTitleText = Variables.prefillSubSystemNumber;
                                else if (headerTitleText.ToUpper().Contains("SUB-SYSTEM DESC"))
                                    headerTitleText = Variables.prefillSubSystemDescription;

                                List<string> allPrefills = new List<string>();
                                allPrefills.AddRange(DefaultPrefills);
                                allPrefills.AddRange(Prefill_Names);

                                string findString = searchString(headerTitleText, allPrefills);
                                if (findString != string.Empty)
                                {
                                    headerDoc.Delete(tCell.Range);
                                    headerDoc.Fields.Create(tCell.Range.Start, " MERGEFIELD \"" + findString + "\"");
                                    CharacterProperties cp = headerDoc.BeginUpdateCharacters(tCell.ContentRange);
                                    ParagraphProperties pp = headerDoc.BeginUpdateParagraphs(tCell.ContentRange);

                                    if (fontName != string.Empty)
                                        cp.FontName = fontName;

                                    cp.Bold = false;

                                    if(fontSize != null)
                                        cp.FontSize = fontSize;

                                    pp.Alignment = ParagraphAlignment.Left;
                                    headerDoc.EndUpdateCharacters(cp);
                                    headerDoc.EndUpdateParagraphs(pp);
                                }

                                //Only add qualified names to ensure commonality
                            }
                        }
                    }
                }
            }



            this.Document.Sections[0].EndUpdateHeader(headerDoc);
            this.Options.MailMerge.ViewMergedData = true;
            this.Options.MailMerge.ViewMergedData = false;
            this.Document.EndUpdate();
        }

        /// <summary>
        /// Try to set footer height to accomodate for QR code spacing
        /// </summary>
        public void ResizeFooterForQRCode()
        {
            this.Document.BeginUpdate();
            SubDocument footerDoc = this.Document.Sections[0].BeginUpdateFooter();
            Table tblFooter = footerDoc.Tables[0];

            tblFooter.FirstRow.Height = 100;
            tblFooter.FirstRow.HeightType = HeightType.AtLeast;

            this.Document.Sections[0].EndUpdateFooter(footerDoc);
            this.Options.MailMerge.ViewMergedData = true;
            this.Options.MailMerge.ViewMergedData = false;
            this.Document.EndUpdate();
        }

        private string searchString(string searchTxt, List<string> listOfTexts)
        {
            Regex rgx = new Regex("[^0-9a-z\\.]");
            foreach (string listOfText in listOfTexts)
            {
                string actualCompareText = string.Empty;
                if (listOfText.Contains('|'))
                {
                    string[] texts = listOfText.Split('|');
                    actualCompareText = rgx.Replace(texts[1], string.Empty);
                }
                else
                    actualCompareText = listOfText;

                string cleanSearchTxt = rgx.Replace(searchTxt, string.Empty);
                string[] searchTexts = searchTxt.Split('|');
                string[] compareTexts = actualCompareText.Split('|');

                if (searchTexts.Count() != compareTexts.Count())
                    continue;

                int score = 0;
                foreach (string searchWord in searchTexts)
                {
                    foreach (string compareText in compareTexts)
                    {
                        if (compareText.ToUpper() == searchWord.ToUpper())
                            score += 1;
                    }
                }

                if (score == searchTexts.Count())
                    return listOfText;
            }

            return string.Empty;
        }

        /// <summary>
        /// Remove rows last cell permission because line insertion will cause permission to propagate onto new lines first cell
        /// </summary>
        private RangePermission RemoveAndRetrieveRowLastCellRangePermission(TableRow tRow, bool dontRemove = false)
        {
            this.Document.BeginUpdate();
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();

            TableCell rLastCell = tRow.LastCell;
            RangePermission findRangePermission = rangePermissions.FirstOrDefault(obj => tRow.LastCell.Range.Contains(obj.Range.Start));

            if (findRangePermission != null)
            {
                if(!dontRemove)
                    rangePermissions.Remove(findRangePermission);
            }

            this.Document.EndUpdateRangePermissions(rangePermissions);
            this.Document.EndUpdate();
            return findRangePermission;
        }

        /// <summary>
        /// Remove the insert instruction rows in the document
        /// </summary>
        public void Trim_Redundant_Rows()
        {
            this.Document.BeginUpdate();
            RangePermissionCollection DocumentRangePermissions = this.Document.BeginUpdateRangePermissions();
            //this.Document.CancelUpdateRangePermissions(rangePermissions);

            List<RangePermission> RemoveRangePermissions = DocumentRangePermissions.Where(obj => obj.Group == CustomUserGroupListService.INSERT_LINE || obj.Group == CustomUserGroupListService.REMOVE_LINE || obj.Group == CustomUserGroupListService.SELECT_TAG).ToList();
            foreach (RangePermission RemoveRangePermission in RemoveRangePermissions)
            {
                DocumentRangePermissions.Remove(RemoveRangePermission);
                TableCell tCell = this.Document.Tables.GetTableCell(RemoveRangePermission.Range.Start);
                if (tCell != null)
                {
                    TableRow tRow = tCell.Row;
                    tRow.Delete();
                }
            }

            this.Document.EndUpdateRangePermissions(DocumentRangePermissions);
            this.Document.EndUpdate();
        }

        /// <summary>
        /// Removes redundant interactables
        /// </summary>
        public void Trim_Redundant_Interactables()
        {
            this.Document.BeginUpdate();
            RangePermissionCollection DocumentRangePermissions = this.Document.BeginUpdateRangePermissions();
            //this.Document.CancelUpdateRangePermissions(rangePermissions);

            List<RangePermission> RemoveRangePermissions = DocumentRangePermissions.Where(obj => obj.Group == CustomUserGroupListService.CERTIFICATE_TAG_SELECTION || obj.Group == CustomUserGroupListService.SUBSYSTEM_SELECTION).ToList();

            foreach (RangePermission RemoveRangePermission in RemoveRangePermissions)
            {
                this.Document.Replace(RemoveRangePermission.Range, string.Empty);
                DocumentRangePermissions.Remove(RemoveRangePermission);
            }

            this.Document.EndUpdateRangePermissions(DocumentRangePermissions);
            this.Document.EndUpdate();
        }

        public void RemoveMergeFields()
        {
            //show merge data instead of MERGEFIELDS
            this.Options.MailMerge.ViewMergedData = true;
            //Section firstSection = Document.Sections[0];
            //SubDocument headerDoc = firstSection.BeginUpdateHeader();

            //List<DocumentRange> list = new List<DocumentRange>();
            //for (int i = 0; i < headerDoc.Fields.Count; i++)
            //{
            //    list.Add(headerDoc.Fields[i].CodeRange);
            //}

            //for (int i = list.Count - 1; i >= 0; i--)
            //{
            //    string s = headerDoc.GetText(list[i]);
            //    headerDoc.Delete(list[i]);
            //    s = headerDoc.GetText(list[i]);
            //    headerDoc.Replace(list[i], string.Empty);
            //    s = headerDoc.GetText(list[i]);
            //}

            //Document.Sections[0].EndUpdateHeader(headerDoc);
        }

        /// <summary>
        /// Convert ITR to native by removing all permission components
        /// </summary>
        public void Convert_to_Native()
        {
            this.Document.BeginUpdate();
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);

            List<RangePermission> SafeRemovalList = new List<RangePermission>();
            RangePermission signatureRangePermission = rangePermissions.FirstOrDefault(obj => obj.Group == CustomUserGroupListService.SIGNATURE_BLOCK);
            if (signatureRangePermission != null)
            {
                TableCell SignatureCell = this.Document.Tables.GetTableCell(signatureRangePermission.Range.Start);
                Table SignatureTable = SignatureCell.Table;

                SignatureTable.Rows[1].HeightType = HeightType.AtLeast;
                SignatureTable.Rows[1].Height = 300.0f;
            }

            foreach (RangePermission rangePermission in rangePermissions)
            {
                TableCell FormatCell = this.Document.Tables.GetTableCell(rangePermission.Range.Start);
                if (FormatCell != null)
                    FormatCell.BackgroundColor = Color.Transparent;

                if (rangePermission.Group != CustomUserGroupListService.USER && rangePermission.Group != CustomUserGroupListService.SIGNATURE_BLOCK)
                {
                    this.Document.Delete(rangePermission.Range);
                    SafeRemovalList.Add(rangePermission);
                }
            }

            foreach (RangePermission removePermission in SafeRemovalList)
            {
                rangePermissions.Remove(removePermission);
            }

            //if signature block spans across 2 pages, place whole signature block in next page
            //ApplyPageBreaks();
            this.Document.EndUpdate();
        }

        /// <summary>
        /// Highlight Interactables in RichEditControl
        /// </summary>
        /// <param name="SelectedCaretOnly">Perform coloring on selected permission start range only</param>
        public void Color_Interactables(bool Highlight, bool SelectedCaretOnly, HighlightType highlightType)
        {
            this.Document.BeginUpdate();
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            //this.Document.CancelUpdateRangePermissions(rangePermissions);

            if (SelectedCaretOnly)
            {
                RangePermission rangePermission = rangePermissions.FirstOrDefault(obj => obj.Range.Contains(this.Document.Selection.Start));
                if (rangePermission != null)
                    Color_One_Interactable(rangePermission, Highlight, highlightType);
                else
                {
                    //scan one position back in case user click the end of the range permission
                    DocumentPosition newScanDocPosition = this.Document.CreatePosition(this.Document.Selection.Start.ToInt() - 1);
                    rangePermission = rangePermissions.FirstOrDefault(obj => obj.Range.Contains(newScanDocPosition));
                    if (rangePermission != null)
                        Color_One_Interactable(rangePermission, Highlight, highlightType);
                }
            }
            else
            {
                foreach (RangePermission rangePermission in rangePermissions)
                {
                    FormatPermissionText(rangePermission, !Highlight);
                    Color_One_Interactable(rangePermission, Highlight, highlightType);
                }
            }

            this.Document.EndUpdateRangePermissions(rangePermissions);
            this.Document.EndUpdate();
        }

        /// <summary>
        /// Highlight one Interactable in RichEditControl
        /// </summary>
        private void Color_One_Interactable(RangePermission rangePermission, bool Highlight, HighlightType highlightType)
        {
            this.Document.BeginUpdate();
            if (rangePermission.Group == CustomUserGroupListService.USER
                || rangePermission.Group == CustomUserGroupListService.SUPERVISOR
                || rangePermission.Group == CustomUserGroupListService.DATETIMEPICKER
                || rangePermission.Group == CustomUserGroupListService.DATEPICKER
                || rangePermission.Group == CustomUserGroupListService.TIMEPICKER
                || rangePermission.Group.StartsWith(CustomUserGroupListService.SELECT_TAG))
            {
                TableCell userEditableCell = this.Document.Tables.GetTableCell(rangePermission.Range.Start);
                if (userEditableCell != null)
                {
                    if ((highlightType == HighlightType.Editables || highlightType == HighlightType.Both) && Highlight)
                    {
                        if (rangePermission.Group == CustomUserGroupListService.USER)
                            userEditableCell.BackgroundColor = Color.LemonChiffon;
                        else if (rangePermission.Group == CustomUserGroupListService.SUPERVISOR)
                            userEditableCell.BackgroundColor = Color.LightCyan;
                        else if (rangePermission.Group == CustomUserGroupListService.DATETIMEPICKER)
                            userEditableCell.BackgroundColor = Color.PowderBlue;
                        else if (rangePermission.Group == CustomUserGroupListService.DATEPICKER)
                            userEditableCell.BackgroundColor = Color.LightSalmon;
                        else if (rangePermission.Group == CustomUserGroupListService.TIMEPICKER)
                            userEditableCell.BackgroundColor = Color.Honeydew;
                        else if (rangePermission.Group.StartsWith(CustomUserGroupListService.SELECT_TAG))
                            userEditableCell.BackgroundColor = Color.PaleGreen;
                    }
                    else
                    {
                        userEditableCell.BackgroundColor = Color.Transparent;
                    }
                }
            }
            else if ((highlightType == HighlightType.Interactables || highlightType == HighlightType.Both))
            {
                CharacterProperties cp = this.Document.BeginUpdateCharacters(rangePermission.Range);

                if (Highlight)
                {
                    if (rangePermission.Group == CustomUserGroupListService.TOGGLE_ACCEPTANCE || rangePermission.Group.Contains(CustomUserGroupListService.CVC_STATUS_TOGGLE_ACCEPTANCE_PREFIX))
                    {
                        if (this.Document.GetText(rangePermission.Range).Contains(Common.Replace_WithSpaces(Toggle_Acceptance.Click_Here.ToString())))
                        {
                            cp.ForeColor = Color.White;
                            cp.BackColor = Color.Crimson;
                        }
                        else if (this.Document.GetText(rangePermission.Range).Contains(Common.Replace_WithSpaces(Toggle_Acceptance.Punchlisted.ToString())))
                        {
                            cp.BackColor = Color.Yellow;
                            cp.ForeColor = Color.Black;
                        }
                        else if (this.Document.GetText(rangePermission.Range) == Common.Replace_WithSpaces(Toggle_Acceptance.Not_Applicable.ToString()))
                        {
                            cp.BackColor = Color.LightSkyBlue;
                            cp.ForeColor = Color.Black;
                        }
                        else
                        {
                            cp.ForeColor = Color.White;
                            cp.BackColor = Color.LimeGreen;
                        }
                    }
                    if (rangePermission.Group == CustomUserGroupListService.TOGGLE_YESNO || rangePermission.Group == CustomUserGroupListService.CVC_SUBSYSTEM_COMPLETION || rangePermission.Group == CustomUserGroupListService.PUNCHLIST_WALKDOWN_SUBSYSTEM_COMPLETION || 
                        rangePermission.Group == CustomUserGroupListService.PUNCHLIST_CAT_A || rangePermission.Group.Contains(Variables.ITR_Completion_Prefix) || rangePermission.Group.Contains(Variables.Discipline_Toggle_Prefix) || rangePermission.Group == CustomUserGroupListService.CVC_TYPE)
                    {
                        if (this.Document.GetText(rangePermission.Range).Contains(Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString())))
                        {
                            cp.ForeColor = Color.White;
                            cp.BackColor = Color.Crimson;
                        }
                        else if (this.Document.GetText(rangePermission.Range).Contains(Common.Replace_WithSpaces(Toggle_YesNo.Yes.ToString())))
                        {
                            cp.ForeColor = Color.White;
                            cp.BackColor = Color.LimeGreen;
                        }
                        else if (this.Document.GetText(rangePermission.Range) == Common.Replace_WithSpaces(Toggle_YesNo.No.ToString()))
                        {
                            cp.ForeColor = Color.White;
                            cp.BackColor = Color.Crimson;
                        }
                        else if (this.Document.GetText(rangePermission.Range) == Common.Replace_WithSpaces(Toggle_Acceptance.Not_Applicable.ToString()))
                        {
                            cp.BackColor = Color.LightSkyBlue;
                            cp.ForeColor = Color.Black;
                        }
                        else
                        {
                            cp.ForeColor = Color.White;
                            cp.BackColor = Color.LimeGreen;
                        }
                    }
                    else if(rangePermission.Group == CustomUserGroupListService.SUBSYSTEM_SELECTION || rangePermission.Group == CustomUserGroupListService.CERTIFICATE_TAG_SELECTION)
                    {
                        cp.BackColor = Color.Yellow;
                        cp.ForeColor = Color.Black;
                    }
                    else if (rangePermission.Group == CustomUserGroupListService.SELECTION_TESTEQ || rangePermission.Group == CustomUserGroupListService.USER_PICTURE || rangePermission.Group == CustomUserGroupListService.ATTACH_PICTURE || rangePermission.Group == CustomUserGroupListService.INSERT_LINE || rangePermission.Group == CustomUserGroupListService.REMOVE_LINE || rangePermission.Group == CustomUserGroupListService.PHOTO || rangePermission.Group == CustomUserGroupListService.SUBSYSTEM_SELECTION || rangePermission.Group == CustomUserGroupListService.CERTIFICATE_TAG_SELECTION)
                    {
                        cp.BackColor = Color.Yellow;
                        cp.ForeColor = Color.Black;
                    }
                }
                else
                {
                    cp.ForeColor = Color.Black;
                    cp.BackColor = Color.Transparent;
                }

                this.Document.EndUpdateCharacters(cp);
            }
            this.Document.EndUpdate();
        }

        public void FormatPermissionText(RangePermission rangePermission, bool remove)
        {
            this.Document.BeginUpdate();
            if (remove)
            {
                if (rangePermission.Group == CustomUserGroupListService.SUBSYSTEM_SELECTION || rangePermission.Group == CustomUserGroupListService.CERTIFICATE_TAG_SELECTION)
                {
                    //have to put in a single character or else range permission text cannot be replaced. Tried with inserting text with empty range permission and it still wouldn't expand
                    Document.Replace(rangePermission.Range, " ");
                }
            }
            else
            {
                if (rangePermission.Group == CustomUserGroupListService.SUBSYSTEM_SELECTION)
                {
                    if (rangePermission.Range.Length == 1)
                        Document.Replace(rangePermission.Range, Variables.Select_Subsystem_String);
                }

                if (rangePermission.Group == CustomUserGroupListService.CERTIFICATE_TAG_SELECTION)
                {
                    if (rangePermission.Range.Length == 1)
                        Document.Replace(rangePermission.Range, Variables.Select_Tag);
                }
            }
            this.Document.EndUpdate();
        }

        /// <summary>
        /// Delete rows with disabled toggles
        /// </summary>
        public void Remove_Disabled_Tables(List<Template_Toggle> DisabledToggles)
        {
            this.Document.BeginUpdate();
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);
            if (DisabledToggles.Count == 0)
                goto EndUpdate;
            //goto RemoveAllCustomPermissions;

            foreach (Table table in this.Document.Tables)
            {
                Remove_Disabled_Rows(rangePermissions, table.FirstRow, DisabledToggles);
            }

            EndUpdate:
            this.Document.EndUpdate();
            formatToggleRangePermissionText();
        }

        private void formatToggleRangePermissionText()
        {
            this.Document.BeginUpdate();
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);

            List<RangePermission> removePermissions = new List<RangePermission>();
            CustomUserGroupListService CustomUserGroup = new CustomUserGroupListService();
            List<string> ReservedWord = CustomUserGroup.GetUserGroupList();
            foreach (RangePermission permission in rangePermissions)
            {
                if (!ReservedWord.Contains(permission.Group.ToString()))
                    this.Document.Replace(permission.Range, string.Empty);
            }
            this.Document.EndUpdate();
        }

        /// <summary>
        /// Check if all toggles are required for row to be shown
        /// </summary>
        public void Remove_Disabled_Rows(RangePermissionCollection RangePermissions, TableRow tRow, List<Template_Toggle> DisabledToggles)
        {
            this.Document.BeginUpdate();
            CustomUserGroupListService CustomUserGroup = new CustomUserGroupListService();
            List<string> ReservedWord = CustomUserGroup.GetUserGroupList();
            List<RangePermission> RowPermissions = RangePermissions.Where(obj => tRow.Range.Contains(obj.Range.Start) && !ReservedWord.Contains(obj.Group)).ToList();
            if (RowPermissions.Count == 0)
            {
                if (tRow.Next != null)
                    Remove_Disabled_Rows(RangePermissions, tRow.Next, DisabledToggles);

                this.Document.EndUpdate();
                return;
            }

            RangePermission ConditionalPermission = RangePermissions.FirstOrDefault(obj => tRow.Range.Contains(obj.Range.Start) && obj.Group == CustomUserGroupListService.ALL);
            bool AnyRowPermissionInDisabledToggles = DisabledToggles.Any(obj => RowPermissions.Any(obj1 => obj1.Group == obj.toggleName));
            //bool AnyRowPermissionInDisabledToggles = DisabledToggles.Any(obj => RowPermissions.Any(obj1 => obj1.Group == obj.toggleName));
            bool AllRowPermissionInDisabledToggles = RowPermissions.All(obj => DisabledToggles.Any(obj1 => obj1.toggleName == obj.Group));
            //all toggles must be enabled
            if (tRow.Next != null)
                Remove_Disabled_Rows(RangePermissions, tRow.Next, DisabledToggles);

            if (ConditionalPermission != null && AnyRowPermissionInDisabledToggles)
            {
                tRow.Delete();
                //Hide rows method
                //tRow.HeightType = HeightType.Exact;
                //tRow.Height = 0.1f;
            }
            else if (AllRowPermissionInDisabledToggles)
            {
                tRow.Delete();
                //Hide rows method
                //tRow.HeightType = HeightType.Exact;
                //tRow.Height = 0.1f;
            }

            this.Document.EndUpdate();
        }

        /// <summary>
        /// Removes all modular toggles after the template has been formatted
        /// </summary>
        public void Remove_Toggle_Permissions()
        {
            this.Document.BeginUpdate();
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            //this.Document.CancelUpdateRangePermissions(rangePermissions);
            CustomUserGroupListService CustomUserGroup = new CustomUserGroupListService();
            List<string> ReservedWord = CustomUserGroup.GetUserGroupList();
            List<RangePermission> RemoveRangePermissions = rangePermissions.Where(obj => !ReservedWord.Any(obj1 => obj1 == obj.Group) || obj.Group == CustomUserGroupListService.ALL).ToList();

            foreach (RangePermission RemovePermission in RemoveRangePermissions)
            {
                this.Document.Delete(RemovePermission.Range);
                rangePermissions.Remove(RemovePermission);
            }

            this.Document.EndUpdateRangePermissions(rangePermissions);
            this.Document.EndUpdate();
        }

        /// <summary>
        /// Apply page break to keep rows together once document has been finalized
        /// </summary>
        public void ApplyPageBreaks()
        {
            this.Document.BeginUpdate();
            this.Document.Unprotect();

            this.BeginUpdate();
            this.Document.CaretPosition = this.Document.Range.Start;
            PageBasedRichEditView currentView = this.ActiveView as PageBasedRichEditView;
            int pageNum = currentView.PageCount;
            for (int i = 0; i < pageNum - 1; i++)
            {
                NextPageCommand npc = new NextPageCommand(this);
                npc.Execute();
            }
            this.EndUpdate();
            this.Focus();

            DevExpress.XtraRichEdit.Services.Implementation.IDocumentLayoutService layoutService = this.GetService<DevExpress.XtraRichEdit.Services.Implementation.IDocumentLayoutService>();
            if (layoutService != null)
            {
                List<Table> signatureTables = new List<Table>();
                RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
                this.Document.CancelUpdateRangePermissions(rangePermissions);

                foreach (RangePermission rangePermission in rangePermissions)
                {
                    if (rangePermission.Group == CustomUserGroupListService.SIGNATURE_BLOCK)
                    {
                        TableCell tCell = this.Document.Tables.GetTableCell(rangePermission.Range.Start);
                        signatureTables.Add(tCell.Table);
                    }
                }

                DevExpress.XtraRichEdit.Layout.DocumentLayout layout = layoutService.CalculateDocumentLayout();
                DevExpress.XtraRichEdit.Model.DocumentModel model = this.Model;

                if (layout.Pages.Count > 1)
                {
                    for (int i = 1; i < layout.Pages.Count; i++)
                    {
                        ////get the next page threshold
                        //layout = layoutService.CalculateDocumentLayout();
                        int threshold = ((DevExpress.Utils.IConvertToInt<DevExpress.XtraRichEdit.Model.DocumentLogPosition>)layout.Pages[i].GetFirstPosition(model.MainPieceTable).LogPosition).ToInt();
                        TableCell tCell = this.Document.Tables.GetTableCell(this.Document.CreatePosition(threshold));
                        Document document = this.Document;

                        if (tCell != null)
                        {
                            if (signatureTables.IndexOf(tCell.Table) != -1) //if this is a signature table
                            {
                                Table signatureTable = tCell.Table;
                                if (signatureTable.Range.Start.ToInt() < threshold && signatureTable.Range.End.ToInt() > threshold) //indicates the table is spanning across pages
                                {
                                    if (signatureTable.FirstRow.FirstCell.ContentRange.Start.ToInt() > 0)
                                    {
                                        Paragraph paragraph = document.Paragraphs.Get(document.CreatePosition(signatureTable.FirstRow.FirstCell.ContentRange.Start.ToInt() - 1));
                                        document.InsertText(paragraph.Range.Start, Characters.PageBreak.ToString());
                                    }
                                }
                            }
                            //else
                            //{
                            //    if (tCell.Range.Start.ToInt() < threshold && tCell.Range.End.ToInt() > threshold) //indicates that cell is spanning across pages
                            //    {
                            //        //do not process invisible cells
                            //        if(tCell.Borders.Top.LineStyle != TableBorderLineStyle.None || tCell.Borders.Right.LineStyle != TableBorderLineStyle.None || tCell.Borders.Bottom.LineStyle != TableBorderLineStyle.None || tCell.Borders.Left.LineStyle != TableBorderLineStyle.None)
                            //        {
                            //            TableRow tRow = tCell.Row;
                            //            TableCell mergedCell = GetMergedRowCell(tRow);
                            //            if (mergedCell != null)
                            //            {
                            //                Table currentTable = mergedCell.Table;
                            //                int cellColumn = mergedCell.Index;

                            //                for (int cellRow = mergedCell.Row.Index; cellRow >= 0; cellRow--)
                            //                {

                            //                    TableCell cellAbove = currentTable.Cell(cellRow, cellColumn);

                            //                    if (cellAbove.Borders.Top.LineStyle != TableBorderLineStyle.None)
                            //                    {
                            //                        document.CaretPosition = cellAbove.ContentRange.Start;
                            //                        new SplitTableCommand(this).Execute();
                            //                        document.InsertText(document.Selection.Start, Characters.PageBreak.ToString());
                            //                        break;
                            //                    }
                            //                }
                            //            }
                            //            else
                            //            {
                            //                document.CaretPosition = tCell.ContentRange.Start;
                            //                new SplitTableCommand(this).Execute();
                            //                document.InsertText(document.Selection.Start, Characters.PageBreak.ToString());
                            //                break;
                            //            }
                            //        }
                            //    }
                            //}
                        }
                        #region Support for keeping paragraph together
                        //else
                        //{
                        //    Paragraph paragraph = document.GetParagraph(document.CreatePosition(threshold - 1));

                        //        document.InsertText(document.Selection.Start, Characters.PageBreak.ToString());
                        //}
                        #endregion
                    }
                }
            }
            this.Document.Protect(string.Empty);
            this.Document.EndUpdate();
        }
        #endregion

        #region Interactions
        /// <summary>
        /// Set user selected acceptance value in document
        /// </summary>
        public void Set_YesNo(DocumentRange docRange, string Selection, bool setYesOnly = false, bool forceSetNo = false)
        {
            //Color foreColor;
            //Color backColor;
            string result = string.Empty;
            this.Document.BeginUpdate();
            //Unprotect is necessary because permission will be removed with document.replace without unprotect
            if (System_Environment.GetUser().GUID != Guid.Empty)
                this.Document.Unprotect();

            if(forceSetNo)
            {
                result = Toggle_YesNo.No.ToString();
            }
            else if (Selection == Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString()))
            {
                result = Toggle_YesNo.Yes.ToString();
            }
            else if (!setYesOnly && Selection == Toggle_YesNo.Yes.ToString())
            {
                result = Toggle_YesNo.No.ToString();
            }
            else if (Selection == Toggle_YesNo.No.ToString())
            {
                result = Toggle_YesNo.Yes.ToString();
            }
            else
            {
                result = Common.Replace_WithSpaces(Toggle_Acceptance.Click_Here.ToString());
            }

            this.Document.Replace(docRange, result);
            Color_Interactables(true, true, HighlightType.Interactables);
            //Highlight Block to Improve Performance
            //DocumentRange ColorRange = this.Document.CreateRange(docRange.Start, Selection.Length);
            //CharacterProperties cp = this.Document.BeginUpdateCharacters(ColorRange);
            //cp.ForeColor = foreColor;
            //cp.BackColor = backColor;
            //this.Document.EndUpdateCharacters(cp);

            if (System_Environment.GetUser().GUID != Guid.Empty)
                this.Document.Protect(string.Empty);
            //this.Options.Authentication.Group = CurrentGroup;
            this.Document.EndUpdate();
        }


        /// <summary>
        /// Set user selected acceptance value in document by Permission
        /// </summary>
        public void Set_YesNoOnPermission(RangePermission rangePermission, bool setYesOnly = false, bool forceSetNo = false)
        {
            //Color foreColor;
            //Color backColor;
            string result = string.Empty;
            DocumentRange permissionRange = rangePermission.Range;
            string currentValue = this.Document.GetText(permissionRange);

            this.Document.BeginUpdate();
            //Unprotect is necessary because permission will be removed with document.replace without unprotect
            if (System_Environment.GetUser().GUID != Guid.Empty)
                this.Document.Unprotect();

            if (forceSetNo)
            {
                result = Toggle_YesNo.No.ToString();
            }
            else if (currentValue == Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString()))
            {
                result = Toggle_YesNo.Yes.ToString();
            }
            else if (!setYesOnly && currentValue == Toggle_YesNo.Yes.ToString())
            {
                result = Toggle_YesNo.No.ToString();
            }
            else if (currentValue == Toggle_YesNo.No.ToString())
            {
                result = Toggle_YesNo.Yes.ToString();
            }
            else
            {
                result = Common.Replace_WithSpaces(Toggle_Acceptance.Click_Here.ToString());
            }

            this.Document.Replace(rangePermission.Range, result);
            rangePermission.Group.Replace(currentValue, result);
            Color_One_Interactable(rangePermission, true, HighlightType.Interactables);
            
            //Highlight Block to Improve Performance
            //DocumentRange ColorRange = this.Document.CreateRange(docRange.Start, Selection.Length);
            //CharacterProperties cp = this.Document.BeginUpdateCharacters(ColorRange);
            //cp.ForeColor = foreColor;
            //cp.BackColor = backColor;
            //this.Document.EndUpdateCharacters(cp);

            if (System_Environment.GetUser().GUID != Guid.Empty)
                this.Document.Protect(string.Empty);
            //this.Options.Authentication.Group = CurrentGroup;
            this.Document.EndUpdate();
        }

        /// <summary>
        /// Set user selected acceptance value in document
        /// </summary>
        public void AcceptableNoNotApplicable(DocumentRange docRange, string Selection, bool disableNo = false, bool forceSetNo = false)
        {
            //Color foreColor;
            //Color backColor;

            this.Document.BeginUpdate();
            //Unprotect is necessary because permission will be removed with document.replace without unprotect
            if (System_Environment.GetUser().GUID != Guid.Empty)
                this.Document.Unprotect();

            if (forceSetNo)
            {
                this.Document.Replace(docRange, Toggle_AcceptableNoNA.No.ToString());
            }
            else
            {
                if (Selection == Common.Replace_WithSpaces(Toggle_AcceptableNoNA.Click_Here.ToString()))
                {
                    this.Document.Replace(docRange, Toggle_AcceptableNoNA.Acceptable.ToString());
                }
                else if (Selection == Toggle_AcceptableNoNA.Acceptable.ToString())
                {
                    if(disableNo)
                        this.Document.Replace(docRange, Common.Replace_WithSpaces(Toggle_AcceptableNoNA.Not_Applicable.ToString()));
                    else
                        this.Document.Replace(docRange, Toggle_AcceptableNoNA.No.ToString());
                }
                else if (Selection == Toggle_AcceptableNoNA.No.ToString())
                {
                    this.Document.Replace(docRange, Common.Replace_WithSpaces(Toggle_AcceptableNoNA.Not_Applicable.ToString()));
                }
                else if (Selection == Common.Replace_WithSpaces(Toggle_AcceptableNoNA.Not_Applicable.ToString()))
                {
                    this.Document.Replace(docRange, Common.Replace_WithSpaces(Toggle_Acceptance.Click_Here.ToString()));
                }
                else
                {
                    this.Document.Replace(docRange, Common.Replace_WithSpaces(Toggle_Acceptance.Click_Here.ToString()));
                }
            }

            Color_Interactables(true, true, HighlightType.Interactables);
            SetInitials(docRange, Selection);
            //Highlight Block to Improve Performance
            //DocumentRange ColorRange = this.Document.CreateRange(docRange.Start, Selection.Length);
            //CharacterProperties cp = this.Document.BeginUpdateCharacters(ColorRange);
            //cp.ForeColor = foreColor;
            //cp.BackColor = backColor;
            //this.Document.EndUpdateCharacters(cp);

            if (System_Environment.GetUser().GUID != Guid.Empty)
                this.Document.Protect(string.Empty);
            //this.Options.Authentication.Group = CurrentGroup;
            this.Document.EndUpdate();
        }

        /// <summary>
        /// Set user selected acceptance value in document
        /// </summary>
        public void Set_PartialComplete(DocumentRange docRange, string Selection)
        {
            this.Document.BeginUpdate();
            //Unprotect is necessary because permission will be removed with document.replace without unprotect
            if (System_Environment.GetUser().GUID != Guid.Empty)
                this.Document.Unprotect();

            if (Selection == Common.Replace_WithSpaces(Variables.Select_CVC_Type))
            {
                this.Document.Replace(docRange, Toggle_PartialComplete.Partial.ToString());
            }
            else if (Selection == Toggle_PartialComplete.Partial.ToString())
            {
                this.Document.Replace(docRange, Toggle_PartialComplete.Complete.ToString());
            }
            else if (Selection == Toggle_PartialComplete.Complete.ToString())
            {
                this.Document.Replace(docRange, Toggle_PartialComplete.Partial.ToString());
            }
            else
            {
                this.Document.Replace(docRange, Common.Replace_WithSpaces(Variables.Select_CVC_Type));
            }

            Color_Interactables(true, true, HighlightType.Interactables);
            if (System_Environment.GetUser().GUID != Guid.Empty)
                this.Document.Protect(string.Empty);

            this.Document.EndUpdate();
        }

        /// <summary>
        /// Set user selected acceptance value in document
        /// </summary>
        public void Set_Acceptance(DocumentRange docRange, string Selection)
        {
            //Color foreColor;
            //Color backColor;

            this.Document.BeginUpdate();
            //Unprotect is necessary because permission will be removed with document.replace without unprotect
            if (System_Environment.GetUser().GUID != Guid.Empty)
                this.Document.Unprotect();

            if (Selection == Toggle_Acceptance.Acceptable.ToString())
            {
                this.Document.Replace(docRange, Toggle_Acceptance.Acceptable.ToString());


                //foreColor = Color.White;
                //backColor = Color.LimeGreen;
            }
            else if (Selection == Toggle_Acceptance.Not_Applicable.ToString())
            {
                this.Document.Replace(docRange, Common.Replace_WithSpaces(Toggle_Acceptance.Not_Applicable.ToString()));
                //backColor = Color.LightSkyBlue;
                //foreColor = Color.Black;
            }
            else if (Selection == Toggle_Acceptance.Punchlisted.ToString())
            {
                Selection = Toggle_Acceptance.Punchlisted.ToString() + Variables.punchlistAffix + GetProjectPunchlistCount().ToString();
                this.Document.Replace(docRange, Selection);
                //backColor = Color.Yellow;
                //foreColor = Color.Black;
            }
            else
            {
                this.Document.Replace(docRange, Common.Replace_WithSpaces(Toggle_Acceptance.Click_Here.ToString()));
                //foreColor = Color.White;
                //backColor = Color.Crimson;
            }

            SetInitials(docRange, Selection);
            Color_Interactables(true, true, HighlightType.Interactables);
            //Highlight Block to Improve Performance
            //DocumentRange ColorRange = this.Document.CreateRange(docRange.Start, Selection.Length);
            //CharacterProperties cp = this.Document.BeginUpdateCharacters(ColorRange);
            //cp.ForeColor = foreColor;
            //cp.BackColor = backColor;
            //this.Document.EndUpdateCharacters(cp);

            if (System_Environment.GetUser().GUID != Guid.Empty)
                this.Document.Protect(string.Empty);
            //this.Options.Authentication.Group = CurrentGroup;
            this.Document.EndUpdate();
        }

        /// <summary>
        /// Insert initials after user has updated the toggle, requires currentSignatureUser to be set
        /// </summary>
        private void SetInitials(DocumentRange docRange, string toggle)
        {
            if (currentSignatureUser == null)
                return;

            TableCell tCell = Document.Tables.GetTableCell(docRange.Start);
            TableRow tRow = tCell.Row;

            RangePermissionCollection rangePermissions = Document.BeginUpdateRangePermissions();
            Document.CancelUpdateRangePermissions(rangePermissions);

            List<TableCell> initialCells = new List<TableCell>();
            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group == CustomUserGroupListService.INITIALS)
                {
                    TableCell initialCell = Document.Tables.GetTableCell(rangePermission.Range.Start);
                    initialCells.Add(initialCell);
                }
            }

            foreach (TableCell toggleCell in tRow.Cells)
            {
                TableCell initialCell = initialCells.FirstOrDefault(x => x.Range.Start == toggleCell.Range.Start);
                if (initialCell != null && initialCell.Range.Start == tCell.Range.End)
                {
                    Document.Delete(toggleCell.ContentRange);
                    if (toggle != Toggle_Acceptance.Click_Here.ToString())
                    {
                        if (currentSignatureUser.Signature != null)
                        {
                            Bitmap resizedSignature = Common.ResizeBitmap(currentSignatureUser.Signature, 50, 50);
                            Document.Delete(toggleCell.ContentRange);
                            //remove signature insert
                            //Document.Images.Insert(toggleCell.ContentRange.Start, resizedSignature);
                            Regex initialsRegex = new Regex(@"(\b[a-zA-Z])[a-zA-Z]* ?");
                            string initials = initialsRegex.Replace(currentSignatureUser.Name, "$1");
                            Document.InsertText(toggleCell.ContentRange.End, string.Concat(initials, " "));
                            Document.InsertText(toggleCell.ContentRange.End, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                            //Document.InsertText(toggleCell.Range.Start, "\r\n" + currentSignatureUser.Name + "\r\n");
                            InsertRowPermission(CustomUserGroupListService.INITIALS, toggleCell.ContentRange.Start.ToInt(), toggleCell.ContentRange.Start.ToInt(), false, string.Empty);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Rectify WBS/Tag number
        /// </summary>
        public List<ITR_Refresh_Item> Get_Parallel_Items(Guid SelfWBSTagGuid, bool SelfIsWBS, bool InsertNewRowAfter = false)
        {
            //Make sure self WBSTagGuid is included in the document if parallel processing is used
            SelectTagScan(SelfWBSTagGuid, InsertNewRowAfter);

            AdapterTAG daTAG = new AdapterTAG();
            AdapterWBS daWBS = new AdapterWBS();

            dsTAG.TAGRow drTAG;
            dsWBS.WBSRow drWBS;

            List<ITR_Refresh_Item> ParallelWBSTagItems = new List<ITR_Refresh_Item>();
            try
            {
                RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();

                foreach (RangePermission rangePermission in rangePermissions)
                {
                    if (rangePermission.Group.StartsWith(CustomUserGroupListService.SELECT_TAG))
                    {
                        string rangestring = rangePermission.Group;
                        string[] strWBSTagGuid = rangestring.Split('?');
                        if (strWBSTagGuid.Count() > 1)
                        {
                            Guid wbsTagGuid;
                            try
                            {
                                wbsTagGuid = new Guid(strWBSTagGuid[1]);


                                bool isGuidWBS = false;
                                drTAG = daTAG.GetBy(wbsTagGuid);
                                if (drTAG != null)
                                    this.Document.Replace(rangePermission.Range, drTAG.NUMBER);
                                else
                                {
                                    drWBS = daWBS.GetBy(wbsTagGuid);
                                    if (drWBS != null)
                                    {
                                        isGuidWBS = true;
                                        this.Document.Replace(rangePermission.Range, drWBS.NAME);
                                    }
                                }

                                //do not add primary wbs/tag guid because it'll be added later at index 0
                                if (wbsTagGuid != _wbsTagGuid)
                                {
                                    ParallelWBSTagItems.Add(new ITR_Refresh_Item() { Template_GUID = (Guid)_templateGuid, isWBS = isGuidWBS, WBSTagGuid = wbsTagGuid });
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }

                this.Document.EndUpdateRangePermissions(rangePermissions);
            }
            finally
            {
                daTAG.Dispose();
                daWBS.Dispose();
            }

            ParallelWBSTagItems.Insert(0, new ITR_Refresh_Item() { Template_GUID = (Guid)_templateGuid, isWBS = SelfIsWBS, WBSTagGuid = SelfWBSTagGuid });

            return ParallelWBSTagItems;
        }


        /// <summary>
        /// Rectify WBS/Tag number
        /// </summary>
        public List<ITR_Refresh_Item> Get_Parallel_ItemsRevised(Guid iTRGuid, Guid SelfWBSTagGuid, bool SelfIsWBS, bool InsertNewRowAfter = false)
        {
            List<ITR_Refresh_Item> ParallelWBSTagItems = new List<ITR_Refresh_Item>();
            ParallelWBSTagItems.Insert(0, new ITR_Refresh_Item() { ITR_GUID = iTRGuid, Template_GUID = (Guid)_templateGuid, isWBS = SelfIsWBS, WBSTagGuid = SelfWBSTagGuid });
            return ParallelWBSTagItems;
        }

        public bool IsPunchlistCAT_A_Completed()
        {
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);
            foreach (RangePermission rangePermission in rangePermissions)
            {
                if(rangePermission.Group == CustomUserGroupListService.PUNCHLIST_CAT_A)
                {
                    string permissionText = this.Document.GetText(rangePermission.Range);
                    if (permissionText == Common.Replace_WithSpaces(Toggle_AcceptableNoNA.No.ToString()) || permissionText == Common.Replace_WithSpaces(Toggle_AcceptableNoNA.Click_Here.ToString()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsAll_ITR_Completed()
        {
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);
            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group.Contains(Variables.ITR_Completion_Prefix))
                {
                    string permissionText = this.Document.GetText(rangePermission.Range);
                    if (permissionText == Common.Replace_WithSpaces(Toggle_YesNo.No.ToString()) || permissionText == Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool IsAll_CVC_Completed()
        {
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);
            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group == CustomUserGroupListService.CVC_SUBSYSTEM_COMPLETION)
                {
                    string permissionText = this.Document.GetText(rangePermission.Range);
                    if (permissionText == Common.Replace_WithSpaces(Toggle_YesNo.No.ToString()) || permissionText == Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        public bool IsAll_PunchlistWalkdown_Completed()
        {
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);
            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group == CustomUserGroupListService.PUNCHLIST_WALKDOWN_SUBSYSTEM_COMPLETION)
                {
                    string permissionText = this.Document.GetText(rangePermission.Range);
                    if (permissionText == Common.Replace_WithSpaces(Toggle_YesNo.No.ToString()) || permissionText == Common.Replace_WithSpaces(Toggle_YesNo.Click_Here.ToString()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Handles interactables on RichEditControl
        /// </summary>
        /// <param name="postProtection">Apply protection logic</param>
        /// <returns>Any value updated in document</returns>
        public bool RichEditClick_Interactions(bool postProtection, bool inspected = false, bool approved = false, string allowedRoleAcceptanceName = "")
        {
            this.Document.BeginUpdate();
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);
            DocumentRange selectionRange = this.Document.Selection;
            DocumentRange textSelectionRange;
            try
            {
                textSelectionRange = this.Document.CreateRange(this.Document.Selection.End.ToInt() - 1, 1);
            }
            catch //if we get -1 for document range
            {
                textSelectionRange = this.Document.CreateRange(0, 1);
            }

            RangePermission findRangePermission = rangePermissions.FirstOrDefault(obj => obj.Range.Contains(this.Document.Selection.Start) || obj.Range.Contains(textSelectionRange.Start));
            bool updated = false;

            if (findRangePermission != null)
            {
                if ((!inspected && findRangePermission.Group == CustomUserGroupListService.USER) || (inspected && findRangePermission.Group == CustomUserGroupListService.SUPERVISOR))
                {
                    Common.textBox_GotFocus(null, null);
                    this.Document.EndUpdate();
                    return false;
                }
                else
                    Common.textBox_Leave(null, null);

                //bypass other interaction when form is inspected
                if(findRangePermission.Group.Contains(CustomUserGroupListService.CVC_STATUS_TOGGLE_ACCEPTANCE_PREFIX))
                {
                    string togglerRole = findRangePermission.Group.Replace(CustomUserGroupListService.CVC_STATUS_TOGGLE_ACCEPTANCE_PREFIX.ToString(), "");
                    if(allowedRoleAcceptanceName.ToUpper() != togglerRole.ToUpper())
                    {
                        this.Document.EndUpdate();
                        Common.Warn("This toggle only works when next document status is " + togglerRole);
                        return false;
                    }
                }
                else if (inspected && findRangePermission.Group != CustomUserGroupListService.USER)
                {
                    this.Document.EndUpdate();
                    Common.Warn("Document is protected, please reject it so that it can be edited again");
                    return false;
                }

                if (findRangePermission.Group == CustomUserGroupListService.USER_PICTURE)
                {
                    TableCell tCell = this.Document.Tables.GetTableCell(selectionRange.Start);
                    if (tCell != null)
                    {
                        ReadOnlyDocumentImageCollection docImages = this.Document.Images.Get(tCell.Range);
                        if (docImages.Count == 1)
                        {
                            frmInteractable_PictureEdit f = new frmInteractable_PictureEdit(docImages[0].Image.GetImageBytes(DevExpress.Office.Utils.OfficeImageFormat.Bmp));

                            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                DocumentPosition originalDocPos = docImages[0].Range.Start;
                                Bitmap editedBmp = f.GetEditedBitmap();
                                float originalScaleX = docImages[0].ScaleX;
                                float originalScaleY = docImages[0].ScaleY;
                                docImages[0].ScaleY = 0.01f;
                                docImages[0].ScaleX = 0.01f;
                                this.Document.Images.Insert(originalDocPos, editedBmp);

                                ReadOnlyDocumentImageCollection docImagesAfter = this.Document.Images.Get(tCell.Range);
                                docImagesAfter[0].ScaleX = originalScaleX;
                                docImagesAfter[0].ScaleY = originalScaleY;
                            }
                        }
                        else if (docImages.Count == 2)
                        {
                            frmInteractable_PictureEdit f = new frmInteractable_PictureEdit(docImages[1].Image.GetImageBytes(DevExpress.Office.Utils.OfficeImageFormat.Bmp));
                            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                DocumentPosition originalDocPos = docImages[0].Range.Start;
                                Bitmap editedBmp = f.GetEditedBitmap();

                                float originalScaleX = docImages[0].ScaleX;
                                float originalScaleY = docImages[0].ScaleY;
                                docImages[1].ScaleY = 0.01f;
                                docImages[1].ScaleX = 0.01f;
                                this.Document.Delete(docImages[0].Range);
                                this.Document.Images.Insert(originalDocPos, editedBmp);

                                ReadOnlyDocumentImageCollection docImagesAfter = this.Document.Images.Get(tCell.Range);
                                docImagesAfter[0].ScaleX = originalScaleX;
                                docImagesAfter[0].ScaleY = originalScaleY;
                            }
                        }

                        updated = true;
                    }

                    this.Document.EndUpdate();
                    return updated;
                }
                else if (findRangePermission.Group == CustomUserGroupListService.ATTACH_PICTURE)
                {
                    TableCell tCell = this.Document.Tables.GetTableCell(selectionRange.Start);
                    ReadOnlyDocumentImageCollection docImages = this.Document.Images.Get(tCell.Range);
                    if (tCell != null)
                    {
                        OpenFileDialog openFileDialog1 = new OpenFileDialog();
                        openFileDialog1.Filter = "JPEG Files|*.jpg|Bitmap Files|*.bmp";
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            byte[] bytes = System.IO.File.ReadAllBytes(openFileDialog1.FileName);
                            Bitmap OriginalBitmap = new Bitmap(Common.ConvertByteArrayToImage(bytes));
                            tCell.PreferredWidthType = WidthType.Fixed;
                            Bitmap ResizedBitmap = Common.ResizeBitmap(OriginalBitmap, 300, 300);

                            //if (docImages.Count > 0)
                            //    this.Document.Delete(docImages[0].Range);

                            this.Document.Images.Insert(this.Document.CreatePosition(findRangePermission.Range.End.ToInt()), ResizedBitmap); //2 spaces so the picture won't stick to the permission
                        }
                        else
                        {
                            if ((docImages.Count > 0) && Common.Confirmation("Do you want to remove the picture?", "Remove Picture"))
                            {
                                this.Document.Delete(docImages[0].Range);
                            }
                        }

                        updated = true;
                    }

                    this.Document.EndUpdate();
                    return updated;
                }
                else if (findRangePermission.Group.StartsWith(CustomUserGroupListService.PHOTO))
                {
                    TableCell tCell = this.Document.Tables.GetTableCell(selectionRange.Start);
                    ReadOnlyDocumentImageCollection docImages = this.Document.Images.Get(tCell.Range);
                    if (tCell != null)
                    {
                        TakePictureDialog d = new TakePictureDialog();
                        if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            Image photo = d.Image;
                            Bitmap bitmap = new Bitmap(photo);
                            Bitmap ResizedBitmap = Common.ResizeBitmap(bitmap, 300, 300);
                            this.Document.Images.Insert(this.Document.CreatePosition(findRangePermission.Range.End.ToInt()), ResizedBitmap);
                        }
                        else
                        {
                            if ((docImages.Count > 0) && Common.Confirmation("Do you want to remove the picture?", "Remove Picture"))
                            {
                                this.Document.Delete(docImages[0].Range);
                            }
                        }

                        updated = true;
                    }

                    this.Document.EndUpdate();
                    return updated;
                }
                else if (findRangePermission.Group == CustomUserGroupListService.TOGGLE_ACCEPTANCE)
                {
                    DocumentRange docRange = findRangePermission.Range;
                    //string s = this.Document.GetText(docRange);
                    Point systemWindowsFormsCursorPosition = System.Windows.Forms.Cursor.Position;
                    frmITR_Select_Acceptance f = new frmITR_Select_Acceptance(systemWindowsFormsCursorPosition, docRange);
                    f.Set_Acceptance_Event += new frmITR_Select_Acceptance.SetAcceptanceHandler(Set_Acceptance);
                    f.Show();

                    updated = true;
                    this.Document.EndUpdate();
                    return updated;
                }
                else if(findRangePermission.Group.Contains(CustomUserGroupListService.CVC_STATUS_TOGGLE_ACCEPTANCE_PREFIX))
                {
                    string togglerRole = findRangePermission.Group.Replace(CustomUserGroupListService.CVC_STATUS_TOGGLE_ACCEPTANCE_PREFIX.ToString(), "");
                    if(AuthenticateCVCToggle(togglerRole.ToUpper()))
                    {
                        DocumentRange docRange = findRangePermission.Range;
                        //string s = this.Document.GetText(docRange);
                        Point systemWindowsFormsCursorPosition = System.Windows.Forms.Cursor.Position;
                        frmITR_Select_Acceptance f = new frmITR_Select_Acceptance(systemWindowsFormsCursorPosition, docRange);
                        f.Set_Acceptance_Event += new frmITR_Select_Acceptance.SetAcceptanceHandler(Set_Acceptance);
                        f.Show();

                        updated = true;
                        this.Document.EndUpdate();
                        return updated;
                    }
                    else
                    {
                        Common.Warn("You are not authorised to edit this value");
                    }
                }
                else if (findRangePermission.Group == CustomUserGroupListService.TOGGLE_YESNO || findRangePermission.Group.Contains(Variables.Discipline_Toggle_Prefix))
                {
                    DocumentRange docRange = findRangePermission.Range;

                    string currentValue = this.Document.GetText(docRange);
                    Set_YesNo(docRange, currentValue);
                    updated = true;

                    this.Document.EndUpdate();
                    return updated;
                }
                //Electrical test equipment selection
                else if (findRangePermission.Group == CustomUserGroupListService.SELECTION_TESTEQ)
                {
                    //this.Document.Unprotect(); //dont have to unprotect because permission isn't tampered
                    TableCell tCell = this.Document.Tables.GetTableCell(this.Document.Selection.Start);
                    //Table tblTestEq = tCell.Table;

                    frmEquipment_Main frmEquipment = new frmEquipment_Main();
                    frmEquipment.ShowAsDialog();

                    if (frmEquipment.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Equipment electricalEq = frmEquipment.GetSelectedEq();
                        TableRow tCellRowDefault = tCell.Row;

                        if (electricalEq != null)
                        {
                            foreach (TableCell tRowCell in tCellRowDefault.Cells)
                            {
                                if (tRowCell.Index != tCell.Index)
                                    this.Document.Replace(tRowCell.ContentRange, string.Empty);

                                if (tRowCell.Index == 1)
                                    this.Document.InsertText(tRowCell.Range.Start, electricalEq.EquipmentMake);
                                else if (tRowCell.Index == 2)
                                    this.Document.InsertText(tRowCell.Range.Start, electricalEq.EquipmentModel);
                                else if (tRowCell.Index == 3)
                                    this.Document.InsertText(tRowCell.Range.Start, electricalEq.EquipmentSerial);
                                else if (tRowCell.Index == 4)
                                    this.Document.InsertText(tRowCell.Range.Start, electricalEq.EquipmentExpiry.Date.ToShortDateString());

                                CharacterProperties cp = this.Document.BeginUpdateCharacters(tRowCell.Range);
                                cp.FontSize = _touchUIFontSize;
                                this.Document.EndUpdateCharacters(cp);
                            }
                        }
                    }
                    else
                    {
                        TableRow tCellRowDefault = tCell.Row;
                        foreach (TableCell tRowCell in tCellRowDefault.Cells)
                        {
                            if (tRowCell.Index != tCell.Index)
                                this.Document.Replace(tRowCell.ContentRange, string.Empty);
                        }
                    }

                    //if(postProtection)
                    //    this.Document.Protect(string.Empty);
                    this.Document.EndUpdate();
                    return true;
                }
                //Electrical test equipment selection
                else if (findRangePermission.Group == CustomUserGroupListService.SUBSYSTEM_SELECTION)
                {
                    if(approved)
                    {
                        Common.Warn("Cannot edit subsystem when certificate is approved");
                    }
                    else
                    {
                        //this.Document.Unprotect(); //dont have to unprotect because permission isn't tampered
                        string permissionText = getPermissionText(findRangePermission);
                        if (permissionText.Contains(Variables.Select_Subsystem_String))
                        {
                            TableCell tCell = this.Document.Tables.GetTableCell(this.Document.Selection.Start);
                            frmWBS_Selection f = new frmWBS_Selection();
                            f.SetTitle("Select Subsystem(s)");
                            if (f.ShowDialog() == DialogResult.OK)
                            {
                                List<wbsTagDisplay> selectedWBSTags = f.GetSelectedWBSTags();
                                List<string> addedSubSystemNumbers = GetSubsystemNumbers();
                                foreach (wbsTagDisplay selectedWBSTag in selectedWBSTags)
                                {
                                    if (selectedWBSTag != null && selectedWBSTag.wbsTagDisplayAttachWBS != null)
                                    {
                                        if (selectedWBSTag.wbsTagDisplayAttachWBS.wbsIsSubsystem)
                                        {
                                            if (addedSubSystemNumbers.Any(x => x.ToUpper() == selectedWBSTag.wbsTagDisplayName.ToUpper()))
                                                continue;

                                            int wbsStringLength = selectedWBSTag.wbsTagDisplayName.Length;
                                            int wbsTagPermissionStart = tCell.ContentRange.End.ToInt();
                                            int wbsTagPermissionEnd = wbsTagPermissionStart + wbsStringLength;

                                            DocumentPosition wbsTagStartPos = this.Document.CreatePosition(wbsTagPermissionStart);
                                            string rangePermissionText = selectedWBSTag.wbsTagDisplayName + "-" + selectedWBSTag.wbsTagDisplayDescription;
                                            DocumentRange permissionRange = this.Document.InsertText(wbsTagStartPos, rangePermissionText);
                                            //minus 1 to unstick permission
                                            DocumentRange unstickPermissionRange = this.Document.CreateRange(permissionRange.Start, permissionRange.Length - 1);
                                            InsertRangePermission(string.Concat(CustomUserGroupListService.SUBSYSTEM_PREFIX, selectedWBSTag.wbsTagDisplayName), unstickPermissionRange);
                                        }
                                        else
                                        {
                                            Common.Warn("Selected WBS " + selectedWBSTag.wbsTagDisplayName + " is not a Subsystem");
                                        }
                                    }
                                }

                                formatCellSubsystemText(tCell);
                            }
                        }

                    }

                    this.Document.EndUpdate();
                    return true;
                }
                else if ((findRangePermission.Group == CustomUserGroupListService.CERTIFICATE_TAG_SELECTION || findRangePermission.Group.Contains(CustomUserGroupListService.TAG_PREFIX)))
                {
                    if (approved)
                    {
                        Common.Warn("Cannot edit tag numbers when certificate is approved");
                    }
                    else
                    {
                        TableCell tCell = this.Document.Tables.GetTableCell(this.Document.Selection.Start);
                        TableRow tCellRowDefault = tCell.Row;
                        List<string> subsystemNumbers = GetSubsystemNumbers();
                        List<string> tagNumbers = GetTagNumbers();

                        if (subsystemNumbers.Count > 0)
                        {
                            string permissionName = this.Document.GetText(findRangePermission.Range);
                            if (permissionName != Variables.Select_Tag)
                            {
                                this.Document.EndUpdate();
                                if (Common.Confirmation("Do you wish to remove the tag number " + permissionName + "?", "Confirmation"))
                                {
                                    //unprotect so that permission can scale according to text
                                    this.Document.Unprotect();
                                    this.Document.Replace(findRangePermission.Range, string.Empty);
                                    rangePermissions.Remove(findRangePermission);
                                    Document.EndUpdateRangePermissions(rangePermissions);

                                    //remove row if any permission is found
                                    if (CheckTableTagSelectionPermission(tCell.Table))
                                        tCell.Table.Rows.RemoveAt(tCell.Row.Index);
                                    //replace tag prefix with tag selection when no other tag prefix or tag selection is found
                                    else
                                    {
                                        RangePermission newRP = InsertRangePermission(CustomUserGroupListService.CERTIFICATE_TAG_SELECTION, this.Document.InsertText(findRangePermission.Range.Start, Variables.Select_Tag));
                                        Color_One_Interactable(newRP, true, HighlightType.Both);
                                        CharacterProperties cellCP = this.Document.BeginUpdateCharacters(newRP.Range);
                                        cellCP.Bold = true;
                                        this.Document.EndUpdateCharacters(cellCP);

                                        //remove the description
                                        foreach (TableCell tRowCell in tCellRowDefault.Cells)
                                            if (tRowCell.Index == 1)
                                                this.Document.Replace(tRowCell.Range, string.Empty);
                                    }


                                    this.Document.Protect(string.Empty);
                                }
                            }
                            else
                            {
                                frmWBS_Selection f = new frmWBS_Selection(subsystemNumbers);
                                f.SetTitle("Select a Tag number or press cancel to remove a tag number");
                                if (f.ShowDialog() == DialogResult.OK)
                                {
                                    List<wbsTagDisplay> selectedWBSTags = f.GetSelectedWBSTags();
                                    RangePermissionCollection loopRangePermissions;
                                    foreach (wbsTagDisplay selectedWBSTag in selectedWBSTags)
                                    {
                                        if (selectedWBSTag != null)
                                        {
                                            if (selectedWBSTag.wbsTagDisplayAttachTag != null)
                                            {
                                                if (!tagNumbers.Any(x => x.ToUpper() == selectedWBSTag.wbsTagDisplayAttachTag.tagNumber.ToUpper()))
                                                {
                                                    //remember document range on where to insert the tag
                                                    DocumentRange documentRange = findRangePermission.Range;

                                                    rangePermissions = this.Document.BeginUpdateRangePermissions();
                                                    findRangePermission = rangePermissions.FirstOrDefault(obj => obj.Range.Contains(findRangePermission.Range.Start));
                                                    rangePermissions.Remove(findRangePermission);
                                                    Document.EndUpdateRangePermissions(rangePermissions);
                                                    
                                                    this.Document.Replace(documentRange, selectedWBSTag.wbsTagDisplayAttachTag.tagNumber);
                                                    InsertRangePermission(string.Concat(CustomUserGroupListService.TAG_PREFIX, selectedWBSTag.wbsTagDisplayAttachTag.tagNumber), documentRange);

                                                    //populate the description
                                                    foreach (TableCell tRowCell in tCellRowDefault.Cells)
                                                        if (tRowCell.Index == 1)
                                                        {
                                                            Document.Replace(tRowCell.Range, string.Empty);
                                                            string selectedTagDescription = selectedWBSTag.wbsTagDisplayAttachTag.tagDescription;
                                                            Document.InsertText(tRowCell.Range.Start, selectedTagDescription);
                                                        }

                                                    this.Document.CaretPosition = findRangePermission.Range.End;

                                                    //only insert new row when it's not the last item
                                                    if (selectedWBSTag != selectedWBSTags.Last())
                                                    {
                                                        tCellRowDefault = InsertNewRow(0);

                                                        //insert new row ended the update, reinitialise here
                                                        this.Document.BeginUpdate();
                                                        loopRangePermissions = this.Document.BeginUpdateRangePermissions();
                                                        findRangePermission = loopRangePermissions.FirstOrDefault(obj => obj.Range.Contains(tCellRowDefault.FirstCell.Range.Start));
                                                    }
        
                                                }
                                                else
                                                {
                                                    Common.Warn("Tag number already selected");
                                                }
                                            }
                                            else
                                            {
                                                Common.Warn("Selected item is not a tag number");
                                            }
                                        }
                                    }

                                    this.Document.EndUpdate();
                                }
                            }
                        }
                        else
                            Common.Warn("No sub-system selected");
                    }

                    this.Document.EndUpdate();
                    return true;
                }
                else if (findRangePermission.Group.Contains(Variables.ITR_Completion_Prefix))
                {
                    if (!approved)
                    {
                        Common.Warn("Please approve certificate before checking for ITR completion");
                    }
                    else
                    {
                        string disciplineString = findRangePermission.Group.Replace(Variables.ITR_Completion_Prefix, "");
                        List<string> tagNumbers = GetTagNumbers();
                        List<string> subsystemNumbers = GetSubsystemNumbers();
                        List<ViewModel_MasterITRReport> disciplineITRStatuses = iTRStatuses.Where(x => x.Subsystem != null).Where(x => x.Discipline.ToUpper() == disciplineString).Where(x => subsystemNumbers.Any(y => y != null && y.ToUpper() == x.Subsystem.ToUpper())).ToList();
                        bool isDisciplineITRsStatusCompleted = true;
                        List<ViewModel_ErrorMessage> errorMessages = new List<ViewModel_ErrorMessage>();
                        string iTRErrorDescription = string.Empty;
                        foreach (ViewModel_MasterITRReport disciplineITRStatus in disciplineITRStatuses)
                        {
                            if (disciplineITRStatus.Stage.ToUpper() != "STAGE 1")
                                continue;

                            if (!tagNumbers.Any(x => x.ToUpper() == disciplineITRStatus.Tag.ToUpper()))
                            {
                                if (disciplineITRStatus.Status.ToUpper() == ITR_Status.Completed.ToString().ToUpper() || disciplineITRStatus.Status.ToUpper() == ITR_Status.Closed.ToString().ToUpper())
                                    continue;

                                iTRErrorDescription = "Discipline: " + disciplineITRStatus.Discipline + " Tag Number: " + disciplineITRStatus.Tag + " ITR: " + disciplineITRStatus.Template + " Status: " + disciplineITRStatus.Status;
                                errorMessages.Add(new ViewModel_ErrorMessage() { Error = "ITR not completed", Description = iTRErrorDescription });
                                isDisciplineITRsStatusCompleted = false;
                            }
                        }

                        this.Document.EndUpdate();
                        bool forceSetNo = !isDisciplineITRsStatusCompleted;
                        DocumentRange docRange = findRangePermission.Range;

                        string currentValue = this.Document.GetText(docRange);
                        AcceptableNoNotApplicable(docRange, currentValue, true, forceSetNo);
                        updated = true;

                        if (forceSetNo)
                        {
                            rptErrorMessage f = new rptErrorMessage("Not all ITR's are completed, please complete it then close and reopen this certificate and try again", errorMessages);
                            f.ShowReport();
                        }
                    }

                    return updated;
                }
                else if (findRangePermission.Group == CustomUserGroupListService.PUNCHLIST_CAT_A)
                {
                    if (!approved)
                    {
                        Common.Warn("Please approve certificate before checking for punchlist completion");
                    }
                    else
                    {
                        List<string> subsystemNumbers = GetSubsystemNumbers();
                        List<string> disciplines = GetDisciplines();
                        bool isPunchlistCompleted = true;
                        List<ViewModel_ErrorMessage> errorMessages = new List<ViewModel_ErrorMessage>();
                        foreach (ViewModel_PunchlistReport punchlistStatus in punchlistStatuses)
                        {
                            if (disciplines.Any(x => x.ToUpper() == punchlistStatus.Discipline.ToUpper()))
                            {
                                if (subsystemNumbers.Any(x => x.ToUpper() == punchlistStatus.WBS_Name.ToUpper()))
                                {
                                    if (punchlistStatus.Saved_Count > 0 || punchlistStatus.Categorised_Count > 0 || punchlistStatus.Inspected_Count > 0 || punchlistStatus.Approved_Count > 0)
                                    {
                                        string errorDescription = "Subsystem: " + punchlistStatus.WBS_Name + ", ";
                                        errorDescription += "Discipline: " + punchlistStatus.Discipline + ", ";
                                        if (punchlistStatus.Saved_Count > 0)
                                            errorDescription += "Saved: " + punchlistStatus.Saved_Count.ToString() + " ";
                                        if (punchlistStatus.Categorised_Count > 0)
                                            errorDescription += "Categorised: " + punchlistStatus.Categorised_Count.ToString() + " ";
                                        if (punchlistStatus.Inspected_Count > 0)
                                            errorDescription += "Inspected: " + punchlistStatus.Inspected_Count.ToString() + " ";
                                        if (punchlistStatus.Approved_Count > 0)
                                            errorDescription += "Approved: " + punchlistStatus.Approved_Count.ToString() + " ";

                                        errorMessages.Add(new ViewModel_ErrorMessage() { Error = "Punchlist not completed", Description = errorDescription });
                                        isPunchlistCompleted = false;
                                    }
                                }
                            }
                        }

                        bool forceSetNo = !isPunchlistCompleted;
                        DocumentRange docRange = findRangePermission.Range;

                        string currentValue = this.Document.GetText(docRange);
                        AcceptableNoNotApplicable(docRange, currentValue, true, forceSetNo);
                        updated = true;

                        if (forceSetNo)
                        {
                            rptErrorMessage f = new rptErrorMessage("Not all selected subsystem's category A punchlist's are completed, please complete it then close and reopen this CVC and verify again", errorMessages);
                            f.ShowReport();
                        }
                    }

                    this.Document.EndUpdate();
                    return updated;
                }
                else if(findRangePermission.Group == CustomUserGroupListService.CVC_SUBSYSTEM_COMPLETION)
                {
                    if (!approved)
                    {
                        Common.Warn("Please approve certificate before checking for CVC completion");
                    }
                    else
                    {
                        List<string> subsystemNumbers = GetSubsystemNumbers();

                        List<ViewModel_ErrorMessage> errorMessages = new List<ViewModel_ErrorMessage>();
                        int approveCVCStatus = (int)CVC_Status.Client;

                        foreach (string subsystemNumber in subsystemNumbers)
                        {
                            if (!cvcStatuses.Any(x => x.Subsystems.Any(y => y.ToUpper() == subsystemNumber.ToUpper())))
                            {
                                string errorDescription = subsystemNumber + " (CVC Missing)";

                                //prevent duplication of error description
                                if(!errorMessages.Any(x => x.Description == errorDescription))
                                    errorMessages.Add(new ViewModel_ErrorMessage() { Error = "CVC Not Completed", Description = errorDescription });
                            }
                            else
                            {
                                foreach (ViewModel_CVC cvcStatus in cvcStatuses)
                                {
                                    bool isError = false;
                                    bool isStatusError = false;
                                    string subsystemErrorDescription = string.Empty;
                                    subsystemErrorDescription += "Subsystems: ";

                                    foreach (string subsystem in cvcStatus.Subsystems)
                                    {
                                        if (subsystemNumbers.Any(x => x.ToUpper() == subsystem.ToUpper()))
                                        {
                                            CVC_Status currentSubsystemCVCStatus = CVC_Status.Pending;
                                            if (Enum.TryParse<CVC_Status>(cvcStatus.Status, out currentSubsystemCVCStatus))
                                            {
                                                int currentSubsystemCVCStatusInt = (int)currentSubsystemCVCStatus;
                                                if (currentSubsystemCVCStatusInt < approveCVCStatus)
                                                {
                                                    subsystemErrorDescription += subsystem + ", ";
                                                    isError = true;
                                                }
                                            }
                                            else
                                            {
                                                isError = true;
                                                isStatusError = true;
                                            }
                                        }
                                    }

                                    if (isError)
                                    {
                                        if (subsystemErrorDescription.Length > 2)
                                        {
                                            subsystemErrorDescription = subsystemErrorDescription.Substring(0, subsystemErrorDescription.Length - 2);
                                            subsystemErrorDescription += " ";
                                        }

                                        if (isStatusError)
                                            subsystemErrorDescription = "Invalid CVC Status: " + cvcStatus.Status + " " + subsystemErrorDescription;
                                        else
                                            subsystemErrorDescription = "CVC Status: " + cvcStatus.Status + " " + subsystemErrorDescription;

                                        string errorDescription = "CVC Number: " + cvcStatus.Number + " " + subsystemErrorDescription;

                                        //prevent duplication of error description
                                        if (!errorMessages.Any(x => x.Description == errorDescription))
                                            errorMessages.Add(new ViewModel_ErrorMessage() { Error = "CVC Not Completed", Description = errorDescription });
                                    }
                                }
                            }
                        }

                        bool forceSetNo = errorMessages.Count > 0;
                        Set_YesNoOnPermission(findRangePermission, true, forceSetNo);

                        updated = true;

                        if (forceSetNo)
                        {
                            rptErrorMessage f = new rptErrorMessage("Not all selected CVC's are completed for selected subsystem, please sign off CVC by " + Common.Replace_WithSpaces(CVC_Status.Client.ToString()) + " then close and reopen this certificate and verify again", errorMessages);
                            f.ShowReport();
                        }
                    }

                    this.Document.EndUpdate();
                    return updated;
                }
                else if (findRangePermission.Group == CustomUserGroupListService.PUNCHLIST_WALKDOWN_SUBSYSTEM_COMPLETION)
                {
                    if (!approved)
                    {
                        Common.Warn("Please approve certificate before checking for punchlist walkdown completion");
                    }
                    else
                    {
                        List<string> subsystemNumbers = GetSubsystemNumbers();

                        List<ViewModel_ErrorMessage> errorMessages = new List<ViewModel_ErrorMessage>();
                        int approvePunchlistWalkdownStatus = (int)PunchlistWalkdown_Status.Closed;
                        foreach (ViewModel_PunchlistWalkdown punchlistWalkdownStatus in punchlistWalkdownStatuses)
                        {
                            bool isPunchlistWalkdownComplete = true;
                            bool isStatusParseError = false;
                            string subsystemErrorDescription = string.Empty;
                            subsystemErrorDescription += "Subsystems: ";
                            foreach (string subsystem in punchlistWalkdownStatus.Subsystems)
                            {
                                if (subsystemNumbers.Any(x => x.ToUpper() == subsystem.ToUpper()))
                                {
                                    PunchlistWalkdown_Status currentPunchlistWalkdownStatus = PunchlistWalkdown_Status.Pending;
                                    if (Enum.TryParse<PunchlistWalkdown_Status>(punchlistWalkdownStatus.Status, out currentPunchlistWalkdownStatus))
                                    {
                                        if ((int)currentPunchlistWalkdownStatus < approvePunchlistWalkdownStatus)
                                        {
                                            subsystemErrorDescription += subsystem + ", ";
                                            isPunchlistWalkdownComplete = false;
                                        }
                                    }
                                    else
                                    {
                                        isStatusParseError = true;
                                    }
                                }

                                if (!isPunchlistWalkdownComplete)
                                {
                                    if (subsystemErrorDescription.Length > 2)
                                    {
                                        subsystemErrorDescription = subsystemErrorDescription.Substring(0, subsystemErrorDescription.Length - 2);
                                        subsystemErrorDescription += " ";
                                    }

                                    if(isStatusParseError)
                                        subsystemErrorDescription = "Invalid PLWD Status: " + punchlistWalkdownStatus.Status + " " + subsystemErrorDescription;
                                    else
                                        subsystemErrorDescription = "PLWD Status: " + punchlistWalkdownStatus.Status + " " + subsystemErrorDescription;

                                    string errorDescription = "PLWD Number: " + punchlistWalkdownStatus.Number + " " + subsystemErrorDescription;
                                    errorMessages.Add(new ViewModel_ErrorMessage() { Error = "PLWD Not Completed", Description = errorDescription });
                                }
                            }
                        }

                        bool forceSetNo = errorMessages.Count > 0;
                        DocumentRange docRange = findRangePermission.Range;
                        string currentValue = this.Document.GetText(docRange);
                        Set_YesNoOnPermission(findRangePermission, true, forceSetNo);
                        updated = true;

                        if (forceSetNo)
                        {
                            rptErrorMessage f = new rptErrorMessage("Not all selected PLWD's are completed for selected subsystem, please approve PLWD then close and reopen this certificate and verify again", errorMessages);
                            f.ShowReport();
                        }
                    }

                    this.Document.EndUpdate();
                    return updated;
                }
                else if (findRangePermission.Group == CustomUserGroupListService.CVC_TYPE)
                {
                    DocumentRange docRange = findRangePermission.Range;
                    string currentValue = this.Document.GetText(docRange);
                    Set_PartialComplete(docRange, currentValue);
                    updated = true;

                    this.Document.EndUpdate();
                    return updated;
                }
                else if (findRangePermission.Group.Contains(CustomUserGroupListService.SUBSYSTEM_PREFIX))
                {
                    if (approved)
                    {
                        Common.Warn("Cannot remove subsystem when certificate is approved");
                    }
                    else
                    {
                        string subSystemText = this.Document.GetText(findRangePermission.Range);
                        this.Document.EndUpdate();
                        if (Common.Confirmation("Do you wish to remove subsystem: " + subSystemText, "Confirmation"))
                        {
                            rangePermissions.Remove(findRangePermission);
                            //add 1 to remove space
                            this.Document.Replace(this.Document.CreateRange(findRangePermission.Range.Start, findRangePermission.Range.Length + 1), string.Empty);
                            Document.EndUpdateRangePermissions(rangePermissions);

                            TableCell tCell = this.Document.Tables.GetTableCell(this.Document.Selection.Start);
                            formatCellSubsystemText(tCell);
                        }
                    }
                }
                else if (findRangePermission.Group == CustomUserGroupListService.DATETIMEPICKER
                    || findRangePermission.Group == CustomUserGroupListService.DATEPICKER
                    || findRangePermission.Group == CustomUserGroupListService.TIMEPICKER)
                {
                    //this.Document.Unprotect(); //must unprotect to allow permission range to scale according to replace text
                    TableCell tCell = this.Document.Tables.GetTableCell(this.Document.Selection.Start);
                    int offSet = 1; //used to prevent permission from getting replaced
                    DateTimePickerFormat dateTimePickerFormat;

                    if (findRangePermission.Group == CustomUserGroupListService.TIMEPICKER)
                        dateTimePickerFormat = DateTimePickerFormat.Time;
                    else if (findRangePermission.Group == CustomUserGroupListService.DATEPICKER)
                        dateTimePickerFormat = DateTimePickerFormat.Short;
                    else
                        dateTimePickerFormat = DateTimePickerFormat.Long;

                    if (tCell != null)
                    {
                        frmInteractable_DatetimePicker frmDateTimePicker = new frmInteractable_DatetimePicker(dateTimePickerFormat);
                        DocumentPosition insertPosition = this.Document.CreatePosition(findRangePermission.Range.Start.ToInt() + offSet);
                        DocumentRange replaceRange = this.Document.CreateRange(insertPosition, findRangePermission.Range.Length - 1);
                        if (frmDateTimePicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            DateTime selectedDateTime = frmDateTimePicker.SelectedDateTime();
                            if (dateTimePickerFormat == DateTimePickerFormat.Time)
                                this.Document.Replace(replaceRange, selectedDateTime.ToString(Variables.timeStringFormat));
                            else if (dateTimePickerFormat == DateTimePickerFormat.Short)
                                this.Document.Replace(replaceRange, selectedDateTime.ToString(Variables.dateStringFormat));
                            else if (dateTimePickerFormat == DateTimePickerFormat.Long)
                                this.Document.Replace(replaceRange, selectedDateTime.ToString(Variables.dateTimeStringFormat));

                            updated = true;
                        }
                        else
                        {
                            this.Document.Replace(replaceRange, string.Empty);
                            updated = true;
                        }
                    }

                    //if (postProtection)
                    //    this.Document.Protect(string.Empty);
                    this.Document.EndUpdate();
                    updated = true;
                    return updated;
                }
                else if (findRangePermission.Group.StartsWith(CustomUserGroupListService.SELECT_TAG))
                {
                    Guid CurrentWBSTagGuid = Guid.Empty;
                    if (findRangePermission.Group != CustomUserGroupListService.SELECT_TAG)
                    {
                        string[] arrSELECT_TAG = findRangePermission.Group.Split('?');
                        if (arrSELECT_TAG.Count() > 1 && arrSELECT_TAG[1] != string.Empty)
                        {
                            Guid WBSTagGuid = new Guid(arrSELECT_TAG[1]);
                            if (WBSTagGuid == _wbsTagGuid)
                            {
                                Common.Prompt("Cannot edit native WBS/Tag, please do your selection on an empty cell");
                                return false;
                            }
                            else
                                CurrentWBSTagGuid = WBSTagGuid;
                        }
                    }

                    this.Document.Unprotect(); //must unprotect to allow permission range to scale according to replace text

                    List<Guid> WBSTagGuids = Get_SelectTag_Guids();
                    frmITR_Browse f = new frmITR_Browse(WBSTagGuids, _templateGuid, _wbsTagGuid);
                    //int offSet = 1; //used to prevent permission from getting replaced
                    DocumentPosition insertPosition = this.Document.CreatePosition(findRangePermission.Range.Start.ToInt());
                    DocumentRange replaceRange = this.Document.CreateRange(insertPosition, findRangePermission.Range.Length);
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        List<wbsTagDisplay> SelectedWBSTags = f.SelectedWBSTags;
                        if (SelectedWBSTags.Count == 0)
                        {
                            bool ProceedWithDeletion = true;
                            if (CurrentWBSTagGuid != Guid.Empty)
                            {
                                if (!Common.Confirmation("Removing existing entry will remove it's ITR and punchlist\nDo you wish to continue?", "WBS/Tag Number Removal"))
                                    ProceedWithDeletion = false;
                                else
                                    Delete_UntaggedITRs(CurrentWBSTagGuid);
                            }

                            if (ProceedWithDeletion)
                            {
                                //this.Document.Replace(replaceRange, " ");
                                //findRangePermission.Group = CustomUserGroupListService.SELECT_TAG;

                                if (findRangePermission.Group != CustomUserGroupListService.SELECT_TAG)
                                {
                                    //if (Common.Confirmation("Do you also wish to delete the current row?", "Delete Row"))
                                    //{
                                    TableCell tCell = this.Document.Tables.GetTableCell(findRangePermission.Range.Start);
                                    TableRow tRow = tCell.Row;
                                    List<RangePermission> RemoveRangePermission = rangePermissions.Where(obj => tRow.Range.Contains(obj.Range.Start)).ToList();
                                    //need to remove the row range permission, or else the permission will be leaked to the next row upon deletion
                                    //it is also neccessary to ensure isEmptySelectTagExists() doesn't account for phantom permission
                                    foreach (RangePermission RowRangePermission in RemoveRangePermission)
                                    {
                                        rangePermissions.Remove(findRangePermission);
                                    }

                                    this.Document.EndUpdateRangePermissions(rangePermissions);
                                    tCell.Row.Delete();
                                    //}
                                }
                                else
                                    this.Document.EndUpdateRangePermissions(rangePermissions);
                            }
                            else
                            {
                                this.Document.EndUpdateRangePermissions(rangePermissions);
                            }
                        }
                        else
                        {
                            RangePermission loopRangePermission;
                            DocumentPosition loopInsertPosition = insertPosition;
                            DocumentRange loopReplaceRange = replaceRange;

                            int WBSTagLoopCount = 0;
                            RangePermissionCollection loopRangePermissions;
                            this.Document.EndUpdateRangePermissions(rangePermissions);

                            TableCell currentTCell = this.Document.Tables.GetTableCell(findRangePermission.Range.Start);

                            foreach (wbsTagDisplay WBSTag in SelectedWBSTags)
                            {
                                if (WBSTagLoopCount > 0)
                                {
                                    InsertNewRow(0);
                                    currentTCell = currentTCell.Row.Next[currentTCell.Index];
                                    this.Document.CaretPosition = currentTCell.Range.Start;
                                }

                                loopRangePermissions = this.Document.BeginUpdateRangePermissions();
                                loopRangePermission = loopRangePermissions.FirstOrDefault(obj => currentTCell.Range.Contains(obj.Range.Start));

                                //Put GUID into the group name, leave a space behind for the permission to remain when user clears the content
                                loopRangePermission.Group = CustomUserGroupListService.SELECT_TAG + WBSTag.wbsTagDisplayGuid;

                                loopReplaceRange = this.Document.CreateRange(loopRangePermission.Range.Start, loopRangePermission.Range.Length);
                                this.Document.Replace(loopReplaceRange, WBSTag.wbsTagDisplayName);
                                this.Document.EndUpdateRangePermissions(loopRangePermissions);

                                WBSTagLoopCount += 1;
                            }

                            //Have to insert a new row after multiple selection to allow potentially new WBSTag to be added
                            if (!isEmptySelectTagExists())
                                InsertNewRow(0);
                        }
                    }

                    updated = true;
                    if (postProtection)
                        this.Document.Protect(string.Empty);

                    this.Document.EndUpdate();
                    return updated;
                }
                else if (findRangePermission.Group == CustomUserGroupListService.INSERT_LINE)
                {
                    //if (Control.ModifierKeys == Keys.Control)
                    //    RemoveRow(-1);
                    //else
                    InsertNewRow(-1);

                    return updated;
                }
                else if (findRangePermission.Group == CustomUserGroupListService.REMOVE_LINE)
                {
                    TableCell tCell = this.Document.Tables.GetTableCell(this.Document.Selection.Start);
                    if (CheckTableTestEqPermission(tCell.Table))
                        RemoveRow(-1);
                    else
                    {
                        Common.Warn("Cannot remove this row because it contains the last remaining interactable");
                        this.Document.EndUpdate();
                    }

                    return updated;
                }
            }
            else
                Common.textBox_Leave(null, null);

            this.Document.EndUpdate();
            return updated;
        }

        public bool CheckTableTagSelectionPermission(Table table)
        {
            RangePermissionCollection RangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(RangePermissions);

            //only allow rows to be removed if these permission exists within the table
            List<string> removeRowPermissions = new List<string>();
            removeRowPermissions.Add(CustomUserGroupListService.CERTIFICATE_TAG_SELECTION);
            removeRowPermissions.Add(CustomUserGroupListService.TAG_PREFIX);

            foreach (RangePermission rangePermission in RangePermissions)
            {
                foreach(string removeRowPermission in removeRowPermissions)
                {
                    if(rangePermission.Group.Contains(removeRowPermission))
                        if (table.Range.Contains(rangePermission.Range.Start))
                        {
                            return true;
                        }
                }
            }

            return false;
        }


        public bool CheckTableTestEqPermission(Table table)
        {
            RangePermissionCollection RangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(RangePermissions);

            //only allow rows to be removed if these permission exists within the table
            List<string> removeRowPermissions = new List<string>();
            removeRowPermissions.Add(CustomUserGroupListService.SELECTION_TESTEQ);

            int permissionCount = 0;
            foreach (RangePermission rangePermission in RangePermissions)
            {
                foreach (string removeRowPermission in removeRowPermissions)
                {
                    if (rangePermission.Group.Contains(removeRowPermission))
                        if (table.Range.Contains(rangePermission.Range.Start))
                        {
                            permissionCount++;
                        }
                }
            }

            //only prevent removal of the row if it has a single permission to begin with
            //if the table doesn't have any permission allow removal nevertheless
            if (permissionCount == 1)
                return false;

            return false;
        }

        private string getPermissionText(RangePermission rangePermission)
        {
            return this.Document.GetText(rangePermission.Range);
        }

        /// <summary>
        /// Removed ITR for Tag/WBS number removal on SELECT TAG interaction
        /// </summary>
        private void Delete_UntaggedITRs(Guid WBSTagGuid)
        {
            Guid TemplateGuid = (Guid)_templateGuid;

            using (AdapterITR_MAIN daITR = new AdapterITR_MAIN())
            {
                dsITR_MAIN.ITR_MAINRow drITR = daITR.GetByTagWBSTemplate(WBSTagGuid, TemplateGuid);
                if (drITR != null)
                {
                    dsITR_MAIN.ITR_MAINRow drDeletedITR = _dtDeletedITRs.NewITR_MAINRow();
                    drDeletedITR.ItemArray = drITR.ItemArray;
                    _dtDeletedITRs.AddITR_MAINRow(drDeletedITR);
                    daITR.RemoveBy(drITR.GUID);

                    using (AdapterPUNCHLIST_MAIN daPUNCHLIST = new AdapterPUNCHLIST_MAIN())
                    {
                        dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPUNCHLIST = daPUNCHLIST.GetByITR(drITR.GUID);
                        if (dtPUNCHLIST != null)
                        {
                            foreach (dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPUNCHLIST in dtPUNCHLIST.Rows)
                            {
                                drPUNCHLIST.DELETED = DateTime.Now;
                                drPUNCHLIST.DELETEDBY = System_Environment.GetUser().GUID;
                                daPUNCHLIST.Save(drPUNCHLIST);
                            }
                        }
                    }
                }
            }
        }

        private void formatCellSubsystemText(TableCell tableCell)
        {
            RangePermissionCollection scanRangePermissions = this.Document.BeginUpdateRangePermissions();

            List<RangePermission> orderedRangePermissions = new List<RangePermission>();
            foreach(RangePermission rangePermission in scanRangePermissions.OrderBy(x => x.Range.Start))
            {
                if(rangePermission.Range.Start >= tableCell.Range.Start && rangePermission.Range.End <= tableCell.Range.End)
                {
                    if (rangePermission.Group.Contains(CustomUserGroupListService.SUBSYSTEM_PREFIX))
                    {
                        orderedRangePermissions.Add(rangePermission);
                    }
                }
            }

            bool shouldPlaceComma = false;
            foreach(RangePermission orderedRangePermission in orderedRangePermissions)
            {
                string rangePermissionText = (this.Document.GetText(orderedRangePermission.Range));
                if (!shouldPlaceComma)
                {
                    if (rangePermissionText.Contains(", "))
                        this.Document.Replace(orderedRangePermission.Range, rangePermissionText.Replace(", ", ""));
                }
                else
                {
                    if (!rangePermissionText.Contains(", "))
                        this.Document.Replace(orderedRangePermission.Range, ", " + rangePermissionText);
                }

                shouldPlaceComma = true;
            }

            this.Document.EndUpdateRangePermissions(scanRangePermissions);
        }

        /// <summary>
        /// Count the number of select tag permission so user can't delete the last remaining select tag row
        /// </summary>
        private List<Guid> Get_SelectTag_Guids()
        {
            RangePermissionCollection RangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(RangePermissions);

            List<Guid> WBSTagGuids = new List<Guid>();
            foreach (RangePermission rp in RangePermissions)
            {
                if (rp.Group.Contains(CustomUserGroupListService.SELECT_TAG))
                {
                    string[] arrWBSTag = rp.Group.Split('?');
                    if (arrWBSTag.Count() > 1 && arrWBSTag[1] != string.Empty)
                    {
                        Guid WBSTagGuid = new Guid(arrWBSTag[1]);
                        WBSTagGuids.Add(WBSTagGuid);
                    }
                }
            }

            return WBSTagGuids;
        }

        public List<string> GetSubsystemNumbers()
        {
            RangePermissionCollection RangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(RangePermissions);
            List<string> subsystems = new List<string>();
            foreach (RangePermission rangePermission in RangePermissions)
            {
                if (rangePermission.Group.Contains(CustomUserGroupListService.SUBSYSTEM_PREFIX))
                {
                    string subSystemNumber = rangePermission.Group.Replace(CustomUserGroupListService.SUBSYSTEM_PREFIX.ToString(), "");
                    subsystems.Add(subSystemNumber);
                }
            }

            return subsystems;
        }

        public List<string> GetDisciplines()
        {
            RangePermissionCollection RangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(RangePermissions);
            List<string> disciplines = new List<string>();
            foreach (RangePermission rangePermission in RangePermissions)
            {
                if (rangePermission.Group.Contains(Variables.Discipline_Toggle_Prefix))
                {
                    string currentValue = this.Document.GetText(rangePermission.Range);
                    string discipline = rangePermission.Group.Replace(Variables.Discipline_Toggle_Prefix.ToString(), "");
                    if (currentValue.ToUpper() == Toggle_YesNo.Yes.ToString().ToUpper())
                    {
                        discipline = discipline.Replace("_", "");
                        disciplines.Add(discipline);
                    }
                }
            }

            return disciplines;
        }

        public List<string> GetTagNumbers()
        {
            RangePermissionCollection RangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(RangePermissions);
            List<string> tagNumbers = new List<string>();
            foreach (RangePermission rangePermission in RangePermissions)
            {
                if (rangePermission.Group.Contains(CustomUserGroupListService.TAG_PREFIX))
                {
                    string tagNumber = rangePermission.Group.Replace(CustomUserGroupListService.TAG_PREFIX.ToString(), "");
                    tagNumbers.Add(tagNumber);
                }
            }

            return tagNumbers;
        }

        private bool AuthenticateCVCToggle(string CVCToggleInteractableRole)
        {
            if (CVCToggleInteractableRole == CVC_Status.Approved.ToString().ToUpper() && !System_Environment.HasPrivilege(PrivilegeTypeID.Toggle_CVC_Status_Approved) ||
                CVCToggleInteractableRole == CVC_Status.Client.ToString().ToUpper() && !System_Environment.HasPrivilege(PrivilegeTypeID.Toggle_CVC_Status_Client) ||
                CVCToggleInteractableRole == CVC_Status.Commissioning_Manager.ToString().ToUpper() && !System_Environment.HasPrivilege(PrivilegeTypeID.Toggle_CVC_Status_CommissioningManager) ||
                CVCToggleInteractableRole == CVC_Status.Construction_Manager.ToString().ToUpper() && !System_Environment.HasPrivilege(PrivilegeTypeID.Toggle_CVC_Status_ConstructionManager) ||
                CVCToggleInteractableRole == CVC_Status.Supervisor.ToString().ToUpper() && !System_Environment.HasPrivilege(PrivilegeTypeID.Toggle_CVC_Status_Supervisor))
                return false;

            return true;
        }

        /// <summary>
        /// Check if empty tag selection exists
        /// </summary>
        private bool isEmptySelectTagExists()
        {
            RangePermissionCollection DocumentRangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(DocumentRangePermissions);

            foreach (RangePermission DocumentRangePermission in DocumentRangePermissions)
            {
                if (DocumentRangePermission.Group == CustomUserGroupListService.SELECT_TAG)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check whether self is included in range permission before saving
        /// </summary>
        public void SelectTagScan(Guid WBSTagGuid, bool InsertNewRowAfter = false)
        {
            RangePermissionCollection DocumentRangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(DocumentRangePermissions);

            bool isSelfGuidExists = false;
            bool isEmptySelectTagExists = false;
            bool isContainSelectTagPermission = false;

            foreach (RangePermission DocumentRangePermission in DocumentRangePermissions)
            {
                if (DocumentRangePermission.Group.Contains(CustomUserGroupListService.SELECT_TAG))
                {
                    isContainSelectTagPermission = true;
                    string[] DocumentPermissionGuid = DocumentRangePermission.Group.Split('?');
                    if (DocumentPermissionGuid.Count() > 1 && DocumentPermissionGuid[1] != string.Empty)
                    {
                        Guid PermissionGuidCheck = new Guid(DocumentPermissionGuid[1]);
                        if (PermissionGuidCheck == WBSTagGuid)
                        {
                            isSelfGuidExists = true;
                        }
                    }
                    else
                    {
                        isEmptySelectTagExists = true;
                        this.Document.CaretPosition = DocumentRangePermission.Range.Start;
                    }

                    if (!isEmptySelectTagExists)
                        this.Document.CaretPosition = DocumentRangePermission.Range.Start;
                }
            }

            if (!isSelfGuidExists && isContainSelectTagPermission)
            {
                bool reprotect = false;
                if (this.Document.IsDocumentProtected)
                {
                    this.Document.Unprotect(); //must unprotect to allow permission range to scale according to replace text
                    reprotect = true;
                }

                TableCell tCell = this.Document.Tables.GetTableCell(this.Document.CaretPosition);

                if (!isEmptySelectTagExists)
                {
                    InsertNewRow(0);
                    tCell = tCell.Row.Next[tCell.Index];
                    this.Document.CaretPosition = tCell.Range.Start;
                }

                RangePermissionCollection LoopRangePermissions = this.Document.BeginUpdateRangePermissions();
                RangePermission findRangePermission = LoopRangePermissions.FirstOrDefault(obj => tCell.Range.Contains(obj.Range.Start));
                DocumentPosition insertPosition = this.Document.CreatePosition(findRangePermission.Range.Start.ToInt());
                DocumentRange replaceRange = this.Document.CreateRange(insertPosition, findRangePermission.Range.Length);

                //Put GUID into the group name, leave a space behind for the permission to remain when user clears the content
                findRangePermission.Group = CustomUserGroupListService.SELECT_TAG + WBSTagGuid;

                //this.Document.Replace(replaceRange, WBSTagName);
                this.Document.EndUpdateRangePermissions(LoopRangePermissions);

                if (InsertNewRowAfter)
                    InsertNewRow(0);

                if (reprotect)
                    this.Document.Protect(string.Empty);

                this.Document.EndUpdate();
            }
        }

        /// <summary>
        /// Insert New Row After Caret Position with Offset
        /// </summary>
        /// <param name="RowOffSet">Offset from caret position for row to copy</param>
        public TableRow InsertNewRow(int RowOffSet)
        {
            //this.Document.Unprotect();
            TableCell tCell = this.Document.Tables.GetTableCell(this.Document.Selection.Start);
            TableRow cRow = tCell.Row;
            Table cTable = tCell.Table;
            TableRow cPreviousRow = cTable.Rows[cRow.Index + RowOffSet];

            List<string> previousRowPermissions = new List<string>();
            foreach (TableCell previousRowCell in cPreviousRow.Cells)
            {
                RangePermission rangePermission = RetrieveCellRangePermission(previousRowCell);
                if (rangePermission != null)
                {
                    previousRowPermissions.Add(rangePermission.Group);
                }
            }

            //Remove Previous Row Range Permission and Store Text Length to be added later
            RangePermission rp = RemoveAndRetrieveRowLastCellRangePermission(cPreviousRow, true);
            int rpTextLength = 1;
            if (rp != null)
                rpTextLength = this.Document.GetText(rp.Range).Length;

            TableRow newRow = cTable.Rows.InsertAfter(cPreviousRow.Index);

            //Reinsert the previous range permission
            if (rp != null)
            {
                InsertRangePermission(rp.Group, this.Document.CreateRange(rp.Range.Start, rpTextLength));
            }

            foreach (TableCell newRowCell in newRow.Cells)
            {
                RangePermission copyRangePermission = RetrieveCellRangePermission(cPreviousRow.Cells[newRowCell.Index]);
                if (copyRangePermission != null)
                {
                    string getRangePermissionText = this.Document.GetText(copyRangePermission.Range);
                    if (copyRangePermission.Group == CustomUserGroupListService.TOGGLE_ACCEPTANCE)
                    {
                        string s = Common.Replace_WithSpaces(Toggle_Acceptance.Click_Here.ToString());

                        InsertCellPermission(CustomUserGroupListService.TOGGLE_ACCEPTANCE, newRowCell, true, s);
                    }
                    else if (copyRangePermission.Group == CustomUserGroupListService.SELECTION_TESTEQ)
                    {
                        InsertCellPermission(CustomUserGroupListService.SELECTION_TESTEQ, newRowCell, true, "TEST EQUIPMENT");
                    }
                    else if (copyRangePermission.Group == CustomUserGroupListService.CERTIFICATE_TAG_SELECTION || copyRangePermission.Group.Contains(CustomUserGroupListService.TAG_PREFIX))
                    {
                        InsertCellPermission(CustomUserGroupListService.CERTIFICATE_TAG_SELECTION, newRowCell, true, Variables.Select_Tag);
                    }
                    else if (copyRangePermission.Group.StartsWith(CustomUserGroupListService.SELECT_TAG))
                    {
                        InsertCellPermission(CustomUserGroupListService.SELECT_TAG, newRowCell, true);
                    }
                    else if (copyRangePermission.Group == CustomUserGroupListService.USER)
                    {
                        string text = this.Document.GetText(cPreviousRow.Cells[newRowCell.Index].Range);
                        if (text != string.Empty)
                            this.Document.InsertText(newRowCell.Range.Start, text.Trim());
                        InsertCellPermission(CustomUserGroupListService.USER, newRowCell, true);
                    }
                    else
                    {
                        InsertCellPermission(copyRangePermission.Group, newRowCell, true, string.Empty);
                    }
                }
            }

            this.Document.EndUpdate();
            return newRow;
        }

        /// <summary>
        /// Remove Row After Caret Position with Offset
        /// </summary>
        /// <param name="RowOffSet">Offset from caret position for row to remove</param>
        public void RemoveRow(int RowOffSet)
        {
            TableCell tCell = this.Document.Tables.GetTableCell(this.Document.Selection.Start); 
            TableRow cRow = tCell.Row;
            Table cTable = tCell.Table;
            TableRow cPreviousRow = cTable.Rows[cRow.Index + RowOffSet];

            //remove all permissions within the row
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            foreach (TableCell rowCell in cPreviousRow.Cells)
            {
                RangePermission findRangePermission = rangePermissions.FirstOrDefault(obj => rowCell.Range.Contains(obj.Range.Start));
                if (findRangePermission != null)
                    rangePermissions.Remove(findRangePermission);
            }

            this.Document.EndUpdateRangePermissions(rangePermissions);
            cPreviousRow.Delete();
            this.Document.EndUpdate();
        }
        #endregion

        #region Loading
        /// <summary>
        /// Load the richedit template from database
        /// </summary>
        public bool LoadTemplateFromDB(Guid templateGuid, Guid? wbsTagGuid = null)
        {
            _templateGuid = templateGuid;
            _wbsTagGuid = wbsTagGuid;
            using (AdapterTEMPLATE_MAIN daTemplate = new AdapterTEMPLATE_MAIN())
            {
                byte[] receivedBytes;
                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = daTemplate.GetBy(templateGuid);
                if (drTemplate != null && !drTemplate.IsTEMPLATENull())
                {
                    receivedBytes = drTemplate.TEMPLATE;
                    MemoryStream ms = new MemoryStream(receivedBytes);
                    //Test Comment Out
                   this.Document.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Load the richedit template from database
        /// </summary>
        public bool LoadCertificateFromDB(Guid certificateGuid, Guid projectGuid)
        {
            _certificateGuid = certificateGuid;
            _projectGuid = projectGuid;

            using (AdapterCERTIFICATE_MAIN daCertificate = new AdapterCERTIFICATE_MAIN())
            {
                byte[] receivedBytes;
                dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow drCertificate = daCertificate.GetBy(_certificateGuid);
                if (drCertificate != null)
                {
                    receivedBytes = drCertificate.CERTIFICATE;
                    MemoryStream ms = new MemoryStream(receivedBytes);
                    //Test Comment Out
                    this.Document.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    return true;
                }
                else
                    return false;
            }
        }

        List<ViewModel_MasterITRReport> iTRStatuses = new List<ViewModel_MasterITRReport>();
        //load all iTR status so that discipline ITR check interactable can validate
        public void LoadMasterITRReport(Guid projectGuid)
        {
            using (AdapterITR_MAIN daITR_MAIN = new AdapterITR_MAIN())
            {
                iTRStatuses = daITR_MAIN.GetProjectITRMasterReport(projectGuid);
            }
        }

        List<ViewModel_PunchlistReport> punchlistStatuses = new List<ViewModel_PunchlistReport>();
        //load all punchlist status so that subsystem's CAT A punchlist can be validated
        public void LoadPunchlistReport(Guid projectGuid)
        {
            using (AdapterPUNCHLIST_MAIN daPUNCHLIST_MAIN = new AdapterPUNCHLIST_MAIN())
            {
                punchlistStatuses = daPUNCHLIST_MAIN.GenerateCertificatePunchlistReport(projectGuid);
            }
        }

        List<ViewModel_CVC> cvcStatuses = new List<ViewModel_CVC>();
        public void LoadCVCReport(Guid projectGuid)
        {
            using(AdapterCERTIFICATE_MAIN daCERTIFICATE_MAIN = new AdapterCERTIFICATE_MAIN())
            {
                cvcStatuses = daCERTIFICATE_MAIN.GetCertificate<ViewModel_CVC>(projectGuid, Variables.constructionVerificationCertificateTemplateName);
            }
        }

        List<ViewModel_PunchlistWalkdown> punchlistWalkdownStatuses = new List<ViewModel_PunchlistWalkdown>();
        public void LoadPunchlistWalkdownReport(Guid projectGuid)
        {
            using (AdapterCERTIFICATE_MAIN daCERTIFICATE_MAIN = new AdapterCERTIFICATE_MAIN())
            {
                punchlistWalkdownStatuses = daCERTIFICATE_MAIN.GetCertificate<ViewModel_PunchlistWalkdown>(projectGuid, Variables.punchlistWalkdownTemplateName);
            }
        }

        /// <summary>
        /// Load the richedit template from database
        /// </summary>
        public bool LoadITRFromDB(Guid tagWBSGuid, Guid templateGuid)
        {
            using (AdapterITR_MAIN daITR = new AdapterITR_MAIN())
            {
                byte[] receivedBytes;
                dsITR_MAIN.ITR_MAINRow drITR = daITR.GetByTagWBSTemplate(tagWBSGuid, templateGuid);
                if (drITR != null)
                {
                    receivedBytes = drITR.ITR;
                    MemoryStream ms = new MemoryStream(receivedBytes);
                    this.Document.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    return true;
                }
                else
                    return false;
            }
        }
        #endregion

        #region Punchlist_Validation
        /// <summary>
        /// Check if interactables are validated by the user
        /// </summary>
        public Progress_Restriction CheckInteractables()
        {
            Progress_Restriction AllCheck = Progress_Restriction.None;
            RangePermissionCollection rangePermissions = Document.BeginUpdateRangePermissions();
            Document.CancelUpdateRangePermissions(rangePermissions);

            AdapterPUNCHLIST_MAIN daPunchlist = new AdapterPUNCHLIST_MAIN();
            //wbsTagDisplay wbsTag = CreateWBSTagDisplay(_wbs, _tag);

            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group == CustomUserGroupListService.TOGGLE_ACCEPTANCE || rangePermission.Group == CustomUserGroupListService.TOGGLE_YESNO)
                {
                    string permissionText = Document.GetText(rangePermission.Range);
                    if (permissionText == Common.Replace_WithSpaces(Toggle_Acceptance.Click_Here.ToString()))
                        AllCheck = Progress_Restriction.Acceptance;
                }
            }

            return AllCheck;
        }

        /// <summary>
        /// Get the signature of the interactable to name the punchlist
        /// </summary>
        public int GetITRCurrentPunchlistSignature()
        {
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);

            int documentInteractableCount = 0;
            int documentPunchlistSignature = 0;
            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group == CustomUserGroupListService.TOGGLE_ACCEPTANCE)
                {
                    documentInteractableCount += 1;
                    if (rangePermission.Range.Contains(this.Document.Selection.Start))
                        documentPunchlistSignature = documentInteractableCount;
                }
            }

            return documentPunchlistSignature + 1;
        }

        public int GetProjectPunchlistCount()
        {
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);

            int punchlistCount = 0;
            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group == CustomUserGroupListService.TOGGLE_ACCEPTANCE && this.Document.GetText(rangePermission.Range).Contains(Common.Replace_WithSpaces(Toggle_Acceptance.Punchlisted.ToString())))
                {
                    punchlistCount += 1;
                }
            }

            Random rand = new Random(Guid.NewGuid().GetHashCode());
            using (AdapterPUNCHLIST_MAIN daPUNCHLIST_MAIN = new AdapterPUNCHLIST_MAIN())
            {
                Guid projectGuid = (Guid)System_Environment.GetUser().userProject.Value;
                var dtPunchlist = daPUNCHLIST_MAIN.GetByProject(projectGuid);
                int minValue = dtPunchlist.Count() + punchlistCount;

                return rand.Next(minValue, minValue + 5000);
                //return dtPunchlist.Count() + punchlistCount + randomInt;
            }
        }

        /// <summary>
        /// Populate combobox with existing punchlist item
        /// </summary>
        public void PopulateCmbPunchlistItem(DevExpress.XtraEditors.ComboBoxEdit cmbPunchlistItem, string selectedPunchlistItem, Guid wbsTagGuid, Guid iTRGuid)
        {
            ComboBoxItemCollection coll = cmbPunchlistItem.Properties.Items;
            coll.Clear();

            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);

            List<string> existingPLItemName = new List<string>(); //to check existing punchlist item created against the iTR
            using (AdapterPUNCHLIST_MAIN daPunchlist = new AdapterPUNCHLIST_MAIN())
            {
                dsPUNCHLIST_MAIN.PUNCHLIST_MAINDataTable dtPunchlistITR = daPunchlist.GetByWBSTagITR(wbsTagGuid, iTRGuid);
                if (dtPunchlistITR != null)
                {
                    foreach (dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlistITR in dtPunchlistITR.Rows)
                    {
                        existingPLItemName.Add(drPunchlistITR.ITR_PUNCHLIST_ITEM);
                    }
                }
            }

            foreach (RangePermission rangePermission in rangePermissions)
            {
                if (rangePermission.Group == CustomUserGroupListService.TOGGLE_ACCEPTANCE && this.Document.GetText(rangePermission.Range).Contains(Common.Replace_WithSpaces(Toggle_Acceptance.Punchlisted.ToString())))
                {
                    string punchlistItemName = this.Document.GetText(rangePermission.Range);
                    string findExistingPLItem = existingPLItemName.FirstOrDefault(obj => obj == punchlistItemName);
                    if (findExistingPLItem == null) //only add punchlist item to entry if punchlist wasn't already created
                    {
                        TableCell tCell = this.Document.Tables.GetTableCell(rangePermission.Range.Start);
                        TableRow tCellRow = tCell.Row;

                        string titleText = string.Empty;
                        string descriptionText = string.Empty;

                        //try to get the title cell text
                        try
                        {
                            for (int i = 0; i < tCellRow.Cells.Count - 1; i++)
                            {
                                TableCell tCellTitle = tCellRow[i];
                                titleText = this.Document.GetText(tCellTitle.Range);
                                titleText = titleText.Replace("\r", string.Empty);
                                titleText = titleText.Replace("\n", string.Empty);
                                if (titleText != string.Empty)
                                    break;
                            }
                        }
                        catch { }

                        //try to get the description cell text
                        try
                        {
                            TableCell tCellDescription = tCellRow[tCell.Index + 1];
                            descriptionText = this.Document.GetText(tCellDescription.Range);
                        }
                        catch { }

                        ValuePair vpPunchlistText = new ValuePair(this.Document.GetText(rangePermission.Range), titleText + Variables.delimiter + descriptionText);
                        coll.Add(vpPunchlistText);
                    }
                }
            }

            if (coll.Count == 0)
            {
                ValuePair vpPunchlistText = new ValuePair(Variables.punchlistNotAvailable, string.Empty);
                coll.Add(vpPunchlistText);
            }
            else if (selectedPunchlistItem != string.Empty)
                cmbPunchlistItem.SelectedItem = selectedPunchlistItem;

            cmbPunchlistItem.SelectedIndex = 0;

            coll.EndUpdate();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Gets the range permission of a given cell
        /// </summary>
        private RangePermission RetrieveCellRangePermission(TableCell tCell)
        {
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);
            //DocumentPosition scanDocPos = this.Document.CreatePosition(tCell.Range.Start.ToInt() + 1);
            //RangePermission findRangePermission = rangePermissions.FirstOrDefault(obj => obj.Range.Contains(scanDocPos));
            RangePermission findRangePermission = rangePermissions.FirstOrDefault(obj => tCell.Range.Contains(obj.Range.Start));

            return findRangePermission;
        }

        /// <summary>
        /// Gets all the custom permission that aren't defined in CustomUserGroupListService
        /// </summary>
        public List<string> Get_Custom_Permissions()
        {
            List<string> CustomPermissions = new List<string>();
            CustomUserGroupListService CustomServices = new CustomUserGroupListService();
            RangePermissionCollection rangePermissions = this.Document.BeginUpdateRangePermissions();
            this.Document.CancelUpdateRangePermissions(rangePermissions);
            List<string> ReservedWord = CustomServices.GetUserGroupList();

            foreach (RangePermission permission in rangePermissions)
            {
                if (!ReservedWord.Any(obj => obj == permission.Group.ToString()))
                {
                    CustomPermissions.Add(permission.Group.ToString());
                }
            }

            return CustomPermissions;
        }

        #region TouchUI Delegates
        public void TableRowProcessor_FooterTouchUI(TableRow row, int rowIndex)
        {
            SubDocument footerDoc = this.Document.Sections[0].BeginUpdateFooter();
            foreach (TableCell tCell in row.Cells)
            {
                CharacterProperties cp = footerDoc.BeginUpdateCharacters(tCell.ContentRange);
                if (cp.FontSize < _touchUIFontSize)
                    cp.FontSize = _touchUIFontSize;
                footerDoc.EndUpdateCharacters(cp);
            }

            row.HeightType = HeightType.AtLeast;
            row.Height = _touchUIRowHeight;
            this.Document.Sections[0].EndUpdateFooter(footerDoc);
        }

        public void TableRowProcessor_FooterNonTouchUI(TableRow row, int rowIndex)
        {
            SubDocument footerDoc = this.Document.Sections[0].BeginUpdateFooter();
            foreach (TableCell tCell in row.Cells)
            {
                CharacterProperties cp = footerDoc.BeginUpdateCharacters(tCell.ContentRange);
                if (cp.FontSize > _nonTouchUIFontSize)
                    cp.FontSize = _nonTouchUIFontSize;
                this.Document.EndUpdateCharacters(cp);
            }

            row.HeightType = HeightType.AtLeast;
            row.Height = _nonTouchUIRowHeight;
            this.Document.Sections[0].EndUpdateFooter(footerDoc);
        }

        public void TableRowProcessor_HeaderTouchUI(TableRow row, int rowIndex)
        {
            SubDocument headerDoc = this.Document.Sections[0].BeginUpdateHeader();
            foreach (TableCell tCell in row.Cells)
            {
                CharacterProperties cp = headerDoc.BeginUpdateCharacters(tCell.ContentRange);
                if (cp.FontSize < _touchUIFontSize)
                    cp.FontSize = _touchUIFontSize;
                headerDoc.EndUpdateCharacters(cp);
            }

            row.HeightType = HeightType.AtLeast;
            row.Height = _touchUIRowHeight;
            this.Document.Sections[0].EndUpdateHeader(headerDoc);
        }

        public void TableRowProcessor_HeaderNonTouchUI(TableRow row, int rowIndex)
        {
            SubDocument headerDoc = this.Document.Sections[0].BeginUpdateHeader();
            foreach (TableCell tCell in row.Cells)
            {
                CharacterProperties cp = headerDoc.BeginUpdateCharacters(tCell.ContentRange);
                if (cp.FontSize > _nonTouchUIFontSize)
                    cp.FontSize = _nonTouchUIFontSize;
                this.Document.EndUpdateCharacters(cp);
            }

            row.HeightType = HeightType.AtLeast;
            row.Height = _nonTouchUIRowHeight;
            this.Document.Sections[0].EndUpdateHeader(headerDoc);
        }

        public void TableRowProcessor_TouchUI(TableRow row, int rowIndex)
        {
            foreach (TableCell tCell in row.Cells)
            {
                CharacterProperties cp = this.Document.BeginUpdateCharacters(tCell.ContentRange);
                if (cp.FontSize < _touchUIFontSize)
                    cp.FontSize = _touchUIFontSize;
                this.Document.EndUpdateCharacters(cp);
            }

            row.HeightType = HeightType.AtLeast;
            row.Height = _touchUIRowHeight;
        }

        public void TableRowProcessor_NonTouchUI(TableRow row, int rowIndex)
        {
            foreach (TableCell tCell in row.Cells)
            {
                CharacterProperties cp = this.Document.BeginUpdateCharacters(tCell.ContentRange);
                if (cp.FontSize > _nonTouchUIFontSize)
                    cp.FontSize = _nonTouchUIFontSize;
                this.Document.EndUpdateCharacters(cp);
            }

            row.HeightType = HeightType.AtLeast;
            row.Height = _nonTouchUIRowHeight;
        }
        #endregion

        /// <summary>
        /// Clear table cell used by clearCellDelegate
        /// </summary>
        public void ClearCell(TableCell cell, int i, int j)
        {
            if (i > 0 && j > 0) //replace only the content not the column header and row header
            {
                if (!this.Document.GetText(cell.Range).Contains(Variables.General_NotApplicable))
                    this.Document.Replace((cell).Range, string.Empty);
            }
        }

        /// <summary>
        /// Clear table cell borders used by clearCellDelegate
        /// </summary>
        public void ClearBorders(TableCell cell, int i, int j)
        {
            cell.Borders.Left.LineStyle = TableBorderLineStyle.None;
            cell.Borders.Right.LineStyle = TableBorderLineStyle.None;
            cell.Borders.Top.LineStyle = TableBorderLineStyle.None;
            cell.Borders.Bottom.LineStyle = TableBorderLineStyle.None;
        }

        /// <summary>
        /// Apply table cell borders used by clearCellDelegate
        /// </summary>
        public void ApplyBorders(TableCell cell, int i, int j)
        {
            cell.Borders.Left.LineStyle = TableBorderLineStyle.Single;
            cell.Borders.Right.LineStyle = TableBorderLineStyle.Single;
            cell.Borders.Top.LineStyle = TableBorderLineStyle.Single;
            cell.Borders.Bottom.LineStyle = TableBorderLineStyle.Single;
        }

        /// <summary>
        /// Check if a row has a merged table
        /// </summary>
        public TableCell GetMergedRowCell(TableRow tRow)
        {
            foreach (TableCell tCell in tRow.Cells)
            {
                if (tCell.Borders.Top.LineStyle == TableBorderLineStyle.None)
                {
                    return tCell;
                }
            }

            return null;
        }
        #endregion
    }
}
