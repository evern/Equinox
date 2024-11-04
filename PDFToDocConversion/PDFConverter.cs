using CheckmateDX;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Pdf;
using DevExpress.XtraRichEdit.API.Native;
using ProjectCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PDFToDocConverter
{
    public partial class PDFConverter : DevExpress.XtraEditors.XtraForm
    {
        DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        Timer delayedDisposalTimer = new Timer();
        public PDFConverter()
        {
            InitializeComponent();
            splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
            delayedDisposalTimer.Interval = 1000;
            delayedDisposalTimer.Tick += DelayedDisposalTimer_Tick;
        }

        private void DelayedDisposalTimer_Tick(object sender, EventArgs e)
        {
            delayedDisposalTimer.Stop();
            for(int i = 0; i < delayedImageRecycleBin.Count;i++)
            {
                Image imageForDisposal = delayedImageRecycleBin[i];
                imageForDisposal.Dispose();
                delayedImageRecycleBin.Remove(imageForDisposal);
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
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


                splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SendCommand(CheckmateDX.ProgressForm.WaitFormCommand.SetLargeFormat, 1000);

                List<Image> images = new List<Image>();
                splashScreenManager1.SendCommand(CheckmateDX.ProgressForm.WaitFormCommand.SetProgressMax, thisDialog.FileNames.Count());
                foreach (string filename in thisDialog.FileNames)
                {
                    using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
                    {
                        documentProcessor.LoadDocument(filename);
                        List<string> splitFileName = filename.Split('\\').ToList();
                        splashScreenManager1.SetWaitFormCaption("Converting PDF to image from " + splitFileName.Last() + " ...");
                        for (int i = 1; i <= documentProcessor.Document.Pages.Count; i++)
                        {
                            Bitmap pageBitmap = documentProcessor.CreateBitmap(i, 1000);
                            images.Add(pageBitmap);
                        }
                        CustomRichEdit customRichEdit = ReplaceDocumentWithImages(images);
                        string savePath = directoryInfo.FullName + "\\";
                        savePath += splitFileName.Last().Replace(".pdf", ".doc");
                        customRichEdit.SaveDocument(savePath, DevExpress.XtraRichEdit.DocumentFormat.OpenDocument);
                        customRichEdit.Dispose();
                        delayedDisposalTimer.Start();
                        documentProcessor.CloseDocument();
                        images.Clear();
                        splashScreenManager1.SendCommand(CheckmateDX.ProgressForm.WaitFormCommand.PerformStep, null);
                    }
                }

                splashScreenManager1.CloseWaitForm();
                Process.Start(directoryInfo.FullName);
            }
        }

        List<Image> delayedImageRecycleBin = new List<Image>();
        public CustomRichEdit ReplaceDocumentWithImages(List<Image> images)
        {
            CustomRichEdit customRichEdit1 = new CustomRichEdit();
            if (images.Count > 0)
            {
                customRichEdit1.Document.Unit = DocumentUnit.Document;
                //Section appendSection = customRichEdit1.Document.AppendSection();
                Section mainSection = customRichEdit1.Document.Sections[0];

                //Table table = customRichEdit1.Document.Tables.Create(customRichEdit1.Document.CreatePosition(0), images.Count, 1);
                //table.TableLayout = TableLayoutType.Fixed;
                //table.PreferredWidthType = WidthType.Fixed;

                SubDocument headerDoc = mainSection.BeginUpdateHeader();
                headerDoc.Delete(headerDoc.Range);
                int lastParagraphIndex = headerDoc.Paragraphs.Count;
                headerDoc.Paragraphs[0].LineSpacingType = ParagraphLineSpacing.Exactly;
                headerDoc.Paragraphs[0].LineSpacingMultiplier = 1;
                headerDoc.Paragraphs[0].Alignment = ParagraphAlignment.Right;
                mainSection.EndUpdateHeader(headerDoc);

                SubDocument footerDoc = mainSection.BeginUpdateFooter();
                footerDoc.Delete(footerDoc.Range);
                lastParagraphIndex = footerDoc.Paragraphs.Count;
                footerDoc.Paragraphs[0].LineSpacingType = ParagraphLineSpacing.Exactly;
                footerDoc.Paragraphs[0].LineSpacingMultiplier = 1;
                footerDoc.Paragraphs[0].Alignment = ParagraphAlignment.Right;
                mainSection.EndUpdateHeader(footerDoc);

                for (int i = 0; i < images.Count; i++)
                {
                    //TableCell tableCell = table.Cell(i, 0);

                    //DocumentRange range = tableCell.ContentRange;
                    //ParagraphProperties pp = customRichEdit1.Document.BeginUpdateParagraphs(range);
                    //pp.Alignment = ParagraphAlignment.Right;
                    //customRichEdit1.Document.EndUpdateParagraphs(pp);

                    Image image = images[i];

                    //tableCell.PreferredWidthType = WidthType.Fixed;
                    //tableCell.PreferredWidth = pageWidth;
                    //tableCell.HeightType = HeightType.Auto;
                    //tableCell.Borders.Top.LineStyle = TableBorderLineStyle.Single;
                    //tableCell.Borders.Bottom.LineStyle = TableBorderLineStyle.Single;
                    //tableCell.Borders.Left.LineStyle = TableBorderLineStyle.Single;
                    //tableCell.Borders.Right.LineStyle = TableBorderLineStyle.Single;
                    int imageNativeWidth = image.Width;
                    int imageNativeHeight = image.Height;
                    Section newSection;
                    if(i == 0)
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
                    newSection.Page.PaperKind = System.Drawing.Printing.PaperKind.A4;

                    float pageWidth = isLandscape ? newSection.Page.Height : newSection.Page.Width;
                    float pageHeight = isLandscape ? newSection.Page.Width : newSection.Page.Height;
                    float imageWidth = Units.DocumentsToPixelsF(pageWidth, customRichEdit1.DpiX);
                    float imageHeight = Units.DocumentsToPixelsF(pageHeight, customRichEdit1.DpiY);
                    int intWidth = Convert.ToInt32(imageWidth);
                    int intHeight = Convert.ToInt32(imageHeight);
                    image = Common.ResizeImage(image, intWidth, intHeight);
                    
                    customRichEdit1.Document.Images.Insert(newSection.Range.Start, image);
                    delayedImageRecycleBin.Add(image);
                    //image.Dispose();
                }
            }

            return customRichEdit1;
        }
    }
}
