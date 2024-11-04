using CheckmateDX;
using DevExpress.Pdf;
using ProjectCommon;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RenamePDFToQRCode
{
    public partial class frmSplitPDFByQRCode : Form
    {
        DevExpress.XtraSplashScreen.SplashScreenManager splashScreenManager1;
        protected BackgroundWorker pdfDeleteBackgroundWorker;
        public frmSplitPDFByQRCode()
        {
            InitializeComponent();
            pdfDeleteBackgroundWorker = new BackgroundWorker();
            pdfDeleteBackgroundWorker.DoWork += PdfDeleteBackgroundWorker_DoWork;
            pdfDeleteBackgroundWorker.RunWorkerCompleted += PdfDeleteBackgroundWorker_RunWorkerCompleted; ;
            pdfDeleteBackgroundWorker.WorkerSupportsCancellation = true;
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF (*.PDF)|*.PDF";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;
                openFileDialog.Title = "Please Select PDF";

                folderBrowserDialog.Description = "Please Select Path to Export";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
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

                    DialogResult result = folderBrowserDialog.ShowDialog();
                    string selectedPath = folderBrowserDialog.SelectedPath;

                    //used for saving extracted page
                    PdfDocumentProcessor targetDocumentProcessor = new PdfDocumentProcessor();
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                    {
                        foreach (string filename in openFileDialog.FileNames)
                        {
                            List<Image> images = new List<Image>();
                            PdfDocumentProcessor sourceDocumentProcessor = new PdfDocumentProcessor();
                            splashScreenManager1.ShowWaitForm();
                            //string originalFileName = filename.Split('\\').Last().Replace(".pdf", "");
                            string originalFileName = string.Empty;
                            sourceDocumentProcessor.LoadDocument(filename);
                            List<string> splitFileName = filename.Split('\\').ToList();

                            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetLargeFormat, 500);
                            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, sourceDocumentProcessor.Document.Pages.Count);
                            for (int i = 1; i <= sourceDocumentProcessor.Document.Pages.Count; i++)
                            {
                                Bitmap pageBitmap = sourceDocumentProcessor.CreateBitmap(i, 10000);
                                string metaString = Common.GetDocumentMetaQRCodeTryHarder(pageBitmap);
                                string error_message = string.Empty;

                                string[] metaArray = metaString.Split(';');
                                string project_number = string.Empty;
                                string tag_number = string.Empty;
                                string document_name = string.Empty;
                                string document_status = string.Empty;
                                string page_number = string.Empty;
                                string total_pages = string.Empty;

                                if (metaString == string.Empty || metaArray.Count() < 2)
                                {
                                    for (int j = 1; j < 10000; j++)
                                    {
                                        string savePath = selectedPath + "\\" + originalFileName + "_NotRecognized_" + i + "_" + j.ToString() + ".pdf";
                                        if (!File.Exists(savePath))
                                        {
                                            extractPages(sourceDocumentProcessor, targetDocumentProcessor, i, savePath);
                                            //sourceDocumentProcessor.SaveDocument(savePath);
                                            break;
                                        }
                                    }

                                    continue;
                                }

                                project_number = metaArray[0];
                                tag_number = metaArray[1];
                                document_name = metaArray[2];
                                document_status = metaArray[3];
                                page_number = metaArray[4];
                                total_pages = metaArray[5];

                                for (int j = 1; j < 10000; j++)
                                {
                                    decimal pageNumber;
                                    if (decimal.TryParse(page_number, out pageNumber))
                                    {
                                        string savePath = selectedPath + "\\" + tag_number + " - " + document_name + "_Page;" + (pageNumber + 1).ToString() + "of" + total_pages + "_Version#" + j.ToString() + ".pdf";
                                        splashScreenManager1.SetWaitFormCaption("Saving " + tag_number + " " + document_name);
                                        if (!File.Exists(savePath))
                                        {
                                            extractPages(sourceDocumentProcessor, targetDocumentProcessor, i, savePath);
                                            //sourceDocumentProcessor.Document.Pages[i].SaveDocument(savePath);
                                            break;
                                        }
                                    }
                                }

                                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                            }

                            sourceDocumentProcessor.CloseDocument();
                            sourceDocumentProcessor.Dispose();
                        }

                        splashScreenManager1.CloseWaitForm();
                        //MessageBox.Show("Document renamed in " + selectedPath);
                        combinePDFPages(selectedPath);
                    }

                    targetDocumentProcessor.Dispose();
                }
            }
        }

        /// <summary>
        /// Extract page from source document processor with page number and save as a single page pdf document
        /// </summary>
        private void extractPages(PdfDocumentProcessor sourceDocumentProcessor, PdfDocumentProcessor targetDocumentProcessor, int pageNumber, string saveFilePath)
        {
            targetDocumentProcessor.CreateEmptyDocument(saveFilePath);
            targetDocumentProcessor.Document.Pages.Add(sourceDocumentProcessor.Document.Pages[pageNumber - 1]);
            targetDocumentProcessor.CloseDocument();
        }

        private void btnCombine_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                string selectedPath = fbd.SelectedPath;
                combinePDFPages(selectedPath);
            }
        }

        private void combinePDFPages(string selectedPath)
        {
            List<FileInfo> deleteFileInfos = new List<FileInfo>();
            DirectoryInfo directoryInfo = new DirectoryInfo(selectedPath);
            if (Directory.Exists(directoryInfo.FullName))
            {
                splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, directoryInfo.GetFiles().Count());
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetLargeFormat, 1000);
                foreach (FileInfo file in directoryInfo.GetFiles())
                {
                    using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
                    {
                        PdfDocumentProcessor otherSameFileProcessor = new PdfDocumentProcessor();
                        List<string> partialFileNames = file.FullName.Split(';').ToList();
                        if (partialFileNames.Count > 0)
                        {
                            if (partialFileNames.Any(x => x.Contains("NotRecognized")))
                                continue;

                            IEnumerable<FileInfo> otherSameFiles = directoryInfo.GetFiles().Where(x => x.FullName.Contains(partialFileNames[0]));
                            string finalFileName = partialFileNames[0].Replace("_Page", "");
                            documentProcessor.CreateEmptyDocument(finalFileName + ".pdf");

                            foreach (FileInfo otherSameFile in otherSameFiles)
                            {
                                documentProcessor.AppendDocument(otherSameFile.FullName);
                                deleteFileInfos.Add(otherSameFile);
                            }
                        }

                        documentProcessor.CloseDocument();
                    }

                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }
            }

            if (deleteFileInfos.Count > 0)
            {
                pdfDeleteBackgroundWorker.RunWorkerAsync(deleteFileInfos);
            }
            else
                splashScreenManager1.CloseWaitForm();
        }

        private void PdfDeleteBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetLargeFormat, 1000);
            splashScreenManager1.SetWaitFormCaption("Deleting files...");
            //splashScreenManager1.SetWaitFormCaption("Please wait a minute to ensure documents are fully combined before deleting...");
            List<FileInfo> deleteFileInfos = (List<FileInfo>)e.Argument;
            Thread.Sleep(5000);

            //splashScreenManager1.SetWaitFormCaption("Deleting files...");
            foreach (FileInfo deleteFileInfo in deleteFileInfos)
            {
                deleteFileInfo.Delete();
            }
        }

        private void PdfDeleteBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            splashScreenManager1.CloseWaitForm();
            MessageBox.Show("All documents combined");
        }
    }
}