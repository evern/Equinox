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
using System.IO;

namespace CheckmateDX
{
    public partial class frmPunchlist_Print_Single : CheckmateDX.frmParent
    {
        Punchlist _punchlist;
        AdapterPROJECT _daProject = new AdapterPROJECT();
        AdapterPUNCHLIST_STATUS _daPunchlistStatus = new AdapterPUNCHLIST_STATUS();
        AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN();

        public frmPunchlist_Print_Single(Punchlist punchlist)
        {
            InitializeComponent();
            customRichEdit1.Document.DefaultCharacterProperties.FontName = "Open Sans";
            customRichEdit1.Document.DefaultCharacterProperties.FontSize = 10;
            _punchlist = punchlist;
            this.Text = punchlist.punchlistDisplayAttachment + "_" + punchlist.punchlistItem;

            writePunclist(punchlist);
        }

        private void writePunclist(Punchlist punchlist)
        {
            customRichEdit1.Document.Sections[0].Margins.HeaderOffset = 0;
            customRichEdit1.Document.Sections[0].Margins.FooterOffset = 0;

            customRichEdit1.Document.Sections[0].Margins.Top = 50;
            customRichEdit1.Document.Sections[0].Margins.Bottom = 50;
            customRichEdit1.Document.Sections[0].Margins.Left = 50;
            customRichEdit1.Document.Sections[0].Margins.Right = 50;

            int totalRows = 13;
            Table tblPunchlist = customRichEdit1.Document.Tables.Create(customRichEdit1.Document.Sections[0].Range.Start, totalRows, 2, AutoFitBehaviorType.AutoFitToWindow);
            for (int i = 0; i < tblPunchlist.Rows.Count; i++)
            {
                tblPunchlist.Rows[i].HeightType = HeightType.Exact;
                tblPunchlist.Rows[i].Height = 75;
            }

            customRichEdit1.Document.InsertText(tblPunchlist.Cell(0, 0).Range.Start, "Punchlist Item");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(1, 0).Range.Start, "Tag/WBS");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(2, 0).Range.Start, "Subsystem");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(3, 0).Range.Start, "Title");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(4, 0).Range.Start, "Description");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(5, 0).Range.Start, "Inspection Note");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(6, 0).Range.Start, "Discipline");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(7, 0).Range.Start, "Action By");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(8, 0).Range.Start, "Category");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(9, 0).Range.Start, "Inspection Pictures");
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(11, 0).Range.Start, "Remedial Pictures");
            tblPunchlist.PreferredWidthType = WidthType.Fixed;
            tblPunchlist.TableLayout = TableLayoutType.Fixed;
            tblPunchlist.PreferredWidth = 2500;
            tblPunchlist.TableAlignment = TableRowAlignment.Left;
            int labelPreferredWidth = 600;
            int detailPreferredWidth = 1900;

            for(int rowNumber = 0;rowNumber < totalRows; rowNumber++)
            {
                TableCell firstColumn = tblPunchlist.Rows[rowNumber].Cells[0];
                firstColumn.VerticalAlignment = TableCellVerticalAlignment.Center;
                firstColumn.PreferredWidthType = WidthType.Fixed;
                firstColumn.PreferredWidth = labelPreferredWidth;

                TableCell secondColumn = tblPunchlist.Rows[rowNumber].Cells[1];
                secondColumn.VerticalAlignment = TableCellVerticalAlignment.Center;
                secondColumn.PreferredWidthType = WidthType.Fixed;
                secondColumn.PreferredWidth = detailPreferredWidth;

                if(rowNumber > 8)
                {
                    tblPunchlist.MergeCells(tblPunchlist[rowNumber, 0], tblPunchlist[rowNumber, 1]);
                }
            }


            customRichEdit1.Document.InsertText(tblPunchlist.Cell(0, 1).Range.Start, punchlist.punchlistItem);
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(1, 1).Range.Start, punchlist.punchlistDisplayAttachment);
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(2, 1).Range.Start, string.Concat(punchlist.punchlistParentWBSName, " - ", punchlist.punchlistParentWBSDesc));
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(3, 1).Range.Start, punchlist.punchlistTitle);
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(4, 1).Range.Start, punchlist.punchlistDescription);
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(5, 1).Range.Start, punchlist.punchlistRemedial);
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(6, 1).Range.Start, punchlist.punchlistDiscipline);
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(7, 1).Range.Start, punchlist.punchlistActionBy);
            customRichEdit1.Document.InsertText(tblPunchlist.Cell(8, 1).Range.Start, punchlist.punchlistCategory);

            insertTableImages(tblPunchlist, 10, punchlist, PunchlistImageType.Inspection);
            insertTableImages(tblPunchlist, 12, punchlist, PunchlistImageType.Remedial);

            customRichEdit1.ApplyPageBreaks();
            customRichEdit1.Document.EndUpdate();
        }

        private void insertTableImages(Table tblPunchlist, int imageRowIndex, Punchlist punchlist, PunchlistImageType punchlistImageType)
        {
            List<DocumentImageSource> images = getImages(punchlist, punchlistImageType);
            if (images.Count > 0)
            {
                tblPunchlist.Rows[imageRowIndex].HeightType = HeightType.Auto;
                Table tblImages = customRichEdit1.Document.Tables.Create(tblPunchlist.Cell(imageRowIndex, 0).Range.Start, images.Count, 2, AutoFitBehaviorType.AutoFitToWindow);
                tblImages.ForEachCell(new TableCellProcessorDelegate(customRichEdit1.ClearBorders));

                int insertRowIndex = 0;
                int insertColumnIndex = 0;
                for (int i = 0; i < images.Count; i++)
                {
                    tblImages.Cell(insertRowIndex, insertColumnIndex).TopPadding = 10;
                    tblImages.Cell(insertRowIndex, insertColumnIndex).RightPadding = 10;
                    customRichEdit1.Document.Images.Insert(tblImages.Cell(insertRowIndex, insertColumnIndex).Range.Start, images[i]);
                    if (insertColumnIndex == 0)
                        insertColumnIndex = 1;
                    else
                    {
                        insertColumnIndex = 0;
                        insertRowIndex++;
                    }
                }
            }
        }

        private List<DocumentImageSource> getImages(Punchlist editPunchlist, PunchlistImageType punchlistImageType)
        {
            List<DocumentImageSource> images = new List<DocumentImageSource>();
            using (AdapterPUNCHLIST_MAIN_PICTURE _adapterPUNCHLIST_MAIN_PICTURE = new AdapterPUNCHLIST_MAIN_PICTURE())
            {
                dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable dtPUNCHLIST_MAIN_PICTURE = _adapterPUNCHLIST_MAIN_PICTURE.GetBy(editPunchlist.GUID, punchlistImageType);
                if (dtPUNCHLIST_MAIN_PICTURE != null)
                {
                    foreach (dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURERow drPUNCHLIST_MAIN_PICTURE in dtPUNCHLIST_MAIN_PICTURE.Rows)
                    {
                        Bitmap bitmap = new Bitmap(new MemoryStream(drPUNCHLIST_MAIN_PICTURE.PICTURE));
                        bitmap = Common.ResizeBitmap(bitmap, 600, 600);
                        MemoryStream ms = new MemoryStream();
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        DocumentImageSource image = DocumentImageSource.FromStream(ms);
                        images.Add(image);
                    }
                }
            }

            return images;
        }

        public void ExportToPDF(string selectedDirectory)
        {
            try
            {
                customRichEdit1.ExportToPdf(string.Concat(selectedDirectory, "\\", this.Text, ".pdf"));
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
    }
}
