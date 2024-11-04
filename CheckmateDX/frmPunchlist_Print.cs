using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectLibrary;
using ProjectCommon;
using DevExpress.XtraRichEdit.API.Native;

namespace CheckmateDX
{
    public partial class frmPunchlist_Print : CheckmateDX.frmParent
    {
        List<Punchlist> _punchlists;
        AdapterPROJECT _daProject = new AdapterPROJECT();
        AdapterPUNCHLIST_STATUS _daPunchlistStatus = new AdapterPUNCHLIST_STATUS();
        AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN();

        private bool PrintPossible = true;
        public frmPunchlist_Print(List<Punchlist> Punchlists)
        {
            InitializeComponent();
            customRichEdit1.Document.DefaultCharacterProperties.FontName = "Open Sans";
            customRichEdit1.Document.DefaultCharacterProperties.FontSize = 9;

            using(AdapterTEMPLATE_MAIN daTemplate = new AdapterTEMPLATE_MAIN())
            {
                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = daTemplate.GetBy(Variables.punchlistTemplateName);
                if (drTemplate != null)
                {
                    customRichEdit1.LoadTemplateFromDB(drTemplate.GUID);
                }
                else
                {
                    Common.Prompt("Please design the punchlist template before printing");
                    PrintPossible = false;
                    return;
                } 
            }

            _punchlists = Punchlists;
            bool isSingleTag = isHeaderEligible(Punchlists);
            string statusString = ((Punchlist_Status)Punchlists[0].punchlistStatus).ToString();
            if(isSingleTag)
            {
                string attachmentName = string.Empty;
                string attachmentRev = string.Empty;
                if (Punchlists[0].punchlistAttachTag != null)
                    attachmentName = Punchlists[0].punchlistAttachTag.tagNumber;
                else if (Punchlists[0].punchlistAttachWBS != null)
                    attachmentName = Punchlists[0].punchlistAttachWBS.wbsName;
                    
                customRichEdit1.PopulatePrefill(Punchlists[0].punchlistAttachTag, Punchlists[0].punchlistAttachWBS, null, attachmentName, statusString, true);
                this.Text = attachmentName + "_" + statusString;
            }
            else
            {
                customRichEdit1.PopulatePrefill(null, null, null, "MultipleTag", statusString, true);
                this.Text = "MultipleTag_" + statusString;
            }

            WritePunchlist(Punchlists, isSingleTag);
        }

        private bool isHeaderEligible(List<Punchlist> Punchlists)
        {
            Guid allowedGuid;
            if (Punchlists[0].punchlistAttachTag != null)
                allowedGuid = Punchlists[0].punchlistAttachTag.GUID;
            else
                allowedGuid = Punchlists[0].punchlistAttachWBS.GUID;

            bool onlyOneGuidDetected = true;
            foreach(Punchlist punchlist in Punchlists)
            {
                if (punchlist.punchlistAttachTag != null && allowedGuid != punchlist.punchlistAttachTag.GUID)
                    onlyOneGuidDetected = false;
                else if (punchlist.punchlistAttachWBS != null && allowedGuid != punchlist.punchlistAttachWBS.GUID)
                    onlyOneGuidDetected = false;
            }

            return onlyOneGuidDetected;
        }

        private void WritePunchlist(List<Punchlist> Punchlists, bool isSingleTag)
        {
            //look for the punchlist table
            RangePermissionCollection rangePermissions = customRichEdit1.Document.BeginUpdateRangePermissions();
            RangePermission findRangePermission = rangePermissions.FirstOrDefault(obj => obj.Group == CustomUserGroupListService.PUNCHLIST_BLOCK);
            customRichEdit1.Document.EndUpdateRangePermissions(rangePermissions);

            customRichEdit1.Document.BeginUpdate();
            if (findRangePermission == null)
                return;

            TableCell tCell = customRichEdit1.Document.Tables.GetTableCell(findRangePermission.Range.Start);
            Table punchlistTable = tCell.Table;
            int runningNumber = 1;

            foreach(Punchlist punchlist in Punchlists)
            {
                TableRow newRow = punchlistTable.Rows.Append();

                if(isSingleTag)
                    customRichEdit1.Document.InsertText(newRow.Cells[0].Range.Start, runningNumber.ToString());
                else
                {
                    string WBSTagName = string.Empty;
                    if(punchlist.punchlistAttachTag != null)
                        WBSTagName = punchlist.punchlistAttachTag.tagNumber;

                    if(WBSTagName == string.Empty)
                        customRichEdit1.Document.InsertText(newRow.Cells[0].Range.Start, "N/A");
                    else
                        customRichEdit1.Document.InsertText(newRow.Cells[0].Range.Start, WBSTagName);

                    customRichEdit1.Document.InsertText(newRow.Cells[1].Range.Start, punchlist.punchlistItem);
                }

                customRichEdit1.Document.InsertText(newRow.Cells[2].Range.Start, punchlist.punchlistDescription);
                customRichEdit1.Document.InsertText(newRow.Cells[3].Range.Start, FormatActionBy(punchlist.punchlistActionBy));
                customRichEdit1.Document.InsertText(newRow.Cells[4].Range.Start, FormatCategory(punchlist.punchlistCategory));
                customRichEdit1.Document.InsertText(newRow.Cells[5].Range.Start, punchlist.punchlistPriority);
                dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable punchlistStatuses = _daPunchlistStatus.GetAll(punchlist.GUID);
                if(punchlistStatuses.Rows.Count > 0)
                {
                    dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow completedPunchlistStatus = punchlistStatuses.FirstOrDefault(x => x.STATUS_NUMBER == (int)Punchlist_Status.Completed);
                    if(completedPunchlistStatus != null)
                    {
                        SignatureUser completedSignatureuser = SignatureUserHelper.GetSignatureUser(completedPunchlistStatus.CREATEDBY);
                        if (completedSignatureuser != null)
                        {
                            customRichEdit1.Document.InsertText(newRow.Cells[7].Range.Start, string.Concat(completedSignatureuser.Name, " ", completedPunchlistStatus.CREATED.ToShortDateString()).ToString());
                        }
                    }

                    dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow closedPunchlistStatus = punchlistStatuses.FirstOrDefault(x => x.STATUS_NUMBER == (int)Punchlist_Status.Closed);
                    if (closedPunchlistStatus != null)
                    {
                        SignatureUser closedSignatureUser = SignatureUserHelper.GetSignatureUser(closedPunchlistStatus.CREATEDBY);
                        if (closedSignatureUser != null)
                        {
                            customRichEdit1.Document.InsertText(newRow.Cells[7].Range.Start, string.Concat(closedSignatureUser.Name, " ", closedPunchlistStatus.CREATED.ToShortDateString()).ToString());
                        }
                    }
                }

                foreach (TableCell tNewRowCell in newRow.Cells)
                {
                    CharacterProperties cp = customRichEdit1.Document.BeginUpdateCharacters(tNewRowCell.Range);
                    cp.Bold = false;
                    customRichEdit1.Document.EndUpdateCharacters(cp);
                }

                runningNumber++;
            }

            customRichEdit1.ApplyPageBreaks();
            customRichEdit1.Document.EndUpdate();
        }


        /// <summary>
        /// Abbreviate the actionby detail
        /// </summary>
        private string FormatActionBy(string actionBy)
        {
            if (actionBy == Common.Replace_WithSpaces(Punchlist_ActionBy.Construction.ToString()))
                return "Con";
            if (actionBy == Common.Replace_WithSpaces(Punchlist_ActionBy.Commissioning.ToString()))
                return "Comm";
            if (actionBy == Common.Replace_WithSpaces(Punchlist_ActionBy.Design.ToString()))
                return "Des";
            if (actionBy == Common.Replace_WithSpaces(Punchlist_ActionBy.Principal.ToString()))
                return "P";

            return "N/A";
        }

        /// <summary>
        /// Abbreviate the category detail
        /// </summary>
        private string FormatCategory(string category)
        {
            if (category == Common.Replace_WithSpaces(Punchlist_Category.Addition_to_Project.ToString()))
                return "D";
            if (category == Common.Replace_WithSpaces(Punchlist_Category.Design_Deficiencies.ToString()))
                return "C";
            if (category == Common.Replace_WithSpaces(Punchlist_Category.Job_Completeness.ToString()))
                return "A";
            if (category == Common.Replace_WithSpaces(Punchlist_Category.Safety.ToString()))
                return "B";

            return "N/A";
        }

        public bool ExportToPDF()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PDF|*.pdf";
            saveFileDialog1.Title = "Save a PDF File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                try
                {
                    customRichEdit1.ExportToPdf(saveFileDialog1.FileName);
                }
                catch
                {
                    Common.Prompt("File is in use");
                    return false;
                }
            }
            else
                return false;

            return true;
        }

        public void ExportToXLS(string exportPath)
        {
            try
            {
                customRichEdit1.ExportToPdf(exportPath + "\\" + this.Text + ".pdf");
            }
            catch
            {
                Common.Prompt("File is in use");
            }
        }

        public void Print()
        {
            customRichEdit1.Print();
        }

        public bool CanPrint()
        {
            return PrintPossible;
        }
    }
}
