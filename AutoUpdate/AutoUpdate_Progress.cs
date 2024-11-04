using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Threading;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using ProjectDatabase;
using ProjectCommon;
using System.Data.Odbc;

namespace AutoUpdate
{
    public partial class AutoUpdate_Progress : DevExpress.XtraEditors.XtraForm
    {
#region The private fields
        private bool isFinished = false;
        private List<DownloadFileInfo> downloadFileList = null;
        private List<DownloadFileInfo> allFileList = null;
        private ManualResetEvent evtDownload = null;
        private FtpWebRequest ftpWebRequest = null;
        //private Session winSCPSession = null;
        double totalFileSize = 0;
        double nDownloadedTotal = 0;
        #endregion

        #region The constructor of DownloadProgress
        public AutoUpdate_Progress(List<DownloadFileInfo> downloadFileListTemp)
        {
            InitializeComponent();
            //this.winSCPSession = winSCPSession;
            //FtpTransfer.WinSCPProgress += session_FileTransferProgress;
            Config config = Config.LoadConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.CLIENT_CONFIG_FILENAME));
            lblFrom.Text = "From: " + config.ServerUrl;
            this.downloadFileList = downloadFileListTemp;
            allFileList = new List<DownloadFileInfo>();
            foreach (DownloadFileInfo file in downloadFileListTemp)
            {
                allFileList.Add(file);
            }

            totalFileSize = downloadFileListTemp.Sum(x => x.Size);
        }
        #endregion

        #region The method and event
        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isFinished && DialogResult.No == MessageBox.Show(ConstFile.CANCELORNOT, ConstFile.MESSAGETITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                e.Cancel = true;
                return;
            }
            else
            {
                if (ftpWebRequest != null)
                    ftpWebRequest.Abort();

                evtDownload.Set();
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            evtDownload = new ManualResetEvent(true);
            evtDownload.Reset();

            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcDownload));
        }

        private void ProcDownload(object o)
        {
            try
            {
                while (!evtDownload.WaitOne(0, false))
                {
                    if (this.downloadFileList.Count == 0)
                        break;
                    
                    DownloadFileInfo file = this.downloadFileList[0];
                    this.ShowCurrentDownloadFileName(file.FileName);

                    #region Normal FTP Download
                    //Download the folder file
                    string tempFolderPath = string.Empty;
                    string tempFolderPath1 = CommonEntity.GetFolderUrl(file);
                    if (!string.IsNullOrEmpty(tempFolderPath1))
                    {
                        tempFolderPath = Path.Combine(CommonEntity.SystemBinUrl, ConstFile.TEMPFOLDERNAME);
                        tempFolderPath += tempFolderPath1;
                    }
                    else
                    {
                        tempFolderPath = Path.Combine(CommonEntity.SystemBinUrl, ConstFile.TEMPFOLDERNAME);
                    }

                    Config config = Config.LoadConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.CLIENT_CONFIG_FILENAME));
                    ftpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri(FtpTransfer.ftpPrefix + config.ServerUrl + file.DownloadUrl));
                    ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    ftpWebRequest.Credentials = new NetworkCredential(config.Username, config.Password);

                    ftpWebRequest.UseBinary = true;

                    FtpWebResponse ftpResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                    using (Stream inputStream = ftpResponse.GetResponseStream())
                    {
                        FileStream output = new FileStream(tempFolderPath + @"\" + file.FileName, FileMode.Create);

                        byte[] buffer = new byte[1024 * 1024];
                        int totalReadBytesCount = 0;
                        int readBytesCount;
                        while ((readBytesCount = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, readBytesCount);
                            nDownloadedTotal += readBytesCount;
                            totalReadBytesCount += readBytesCount;
                            var progress = totalReadBytesCount / file.Size;
                            this.SetProcessBar((int)(totalReadBytesCount * 100 / file.Size), (int)(nDownloadedTotal * 100 / totalFileSize), 0);
                        }

                        output.Close();
                        ftpResponse.Close();

                        this.SetProcessBar(0, (int)(nDownloadedTotal * 100 / totalFileSize), 0);
                    }

                    ftpWebRequest = null;
                    #endregion

                    //Remove the downloaded files
                    nDownloadedTotal += 1;
                    this.downloadFileList.Remove(file);
                }
            }
            catch (Exception)
            {
                ShowErrorAndRestartApplication();
            }

            //When the files have not downloaded,return.
            if (downloadFileList.Count > 0)
            {
                return;
            }

            //Test network and deal with errors if there have 
            DealWithDownloadErrors();

            //Debug.WriteLine("All Downloaded");
            foreach (DownloadFileInfo file in this.allFileList)
            {
                string tempUrlPath = CommonEntity.GetFolderUrl(file);
                string oldPath = string.Empty;
                string newPath = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(tempUrlPath))
                    {
                        oldPath = Path.Combine(CommonEntity.SystemBinUrl + tempUrlPath.Substring(1), file.FileName);
                        newPath = Path.Combine(CommonEntity.SystemBinUrl + ConstFile.TEMPFOLDERNAME + tempUrlPath, file.FileName);
                    }
                    else
                    {
                        oldPath = Path.Combine(CommonEntity.SystemBinUrl, file.FileName);
                        newPath = Path.Combine(CommonEntity.SystemBinUrl + ConstFile.TEMPFOLDERNAME, file.FileName);
                    }

                    //Added for dealing with the config file download errors
                    string newfilepath = string.Empty;
                    if (newPath.Substring(newPath.LastIndexOf(".") + 1).Equals(ConstFile.CONFIGFILEKEY))
                    {
                        if (System.IO.File.Exists(newPath))
                        {
                            if (newPath.EndsWith("_"))
                            {
                                newfilepath = newPath;
                                newPath = newPath.Substring(0, newPath.Length - 1);
                                oldPath = oldPath.Substring(0, oldPath.Length - 1);
                            }
                            File.Move(newfilepath, newPath);
                        }
                    }
                    //End added

                    if (File.Exists(oldPath))
                    {
                        MoveFolderToOld(oldPath, newPath);
                    }
                    else
                    {
                        //Edit for config_ file
                        if (!string.IsNullOrEmpty(tempUrlPath))
                        {
                            if (!Directory.Exists(CommonEntity.SystemBinUrl + tempUrlPath.Substring(1)))
                            {
                                Directory.CreateDirectory(CommonEntity.SystemBinUrl + tempUrlPath.Substring(1));


                                MoveFolderToOld(oldPath, newPath);
                            }
                            else
                            {
                                MoveFolderToOld(oldPath, newPath);
                            }
                        }
                        else
                        {
                            MoveFolderToOld(oldPath, newPath);
                        }

                    }
                }
                catch (Exception)
                {
                }

            }

            //After dealed with all files, clear the data
            this.allFileList.Clear();

            if (this.downloadFileList.Count == 0)
                Exit(true);
            else
                Exit(false);

            evtDownload.Set();
        }

        //To delete or move to old files
        void MoveFolderToOld(string oldPath, string newPath)
        {
            if (File.Exists(oldPath + ".old"))
                File.Delete(oldPath + ".old");

            if (File.Exists(oldPath))
                File.Move(oldPath, oldPath + ".old");

            File.Move(newPath, oldPath);

            try
            {
                //this will fail during debugging
                //#if DEBUG
                //#else
                    File.Delete(oldPath + ".old");
                //#endif
            }
            catch
            {

            }
        }

        delegate void ShowCurrentDownloadFileNameCallBack(string name);
        private void ShowCurrentDownloadFileName(string name)
        {
            if (this.lblCurrentItem.InvokeRequired)
            {
                ShowCurrentDownloadFileNameCallBack cb = new ShowCurrentDownloadFileNameCallBack(ShowCurrentDownloadFileName);
                this.Invoke(cb, new object[] { name });
            }
            else
            {
                this.lblCurrentItem.Text = name;
            }
        }

        delegate void SetProcessBarCallBack(int current, int total, int speed);
        private void SetProcessBar(int current, int total, int speed)
        {
            if (this.progressBarCurrent.InvokeRequired)
            {
                SetProcessBarCallBack cb = new SetProcessBarCallBack(SetProcessBar);
                this.Invoke(cb, new object[] { current, total, speed });
            }
            else
            {
                progressBarCurrent.EditValue = current;
                progressBarTotal.EditValue = total;
                lblSpeed.Text = Math.Round((decimal)(speed/1000), 0).ToString() + " KB/s";
            }
        }

        delegate void ExitCallBack(bool success);
        private void Exit(bool success)
        {
            if (this.InvokeRequired)
            {
                ExitCallBack cb = new ExitCallBack(Exit);
                this.Invoke(cb, new object[] { success });
            }
            else
            {
                this.isFinished = success;
                this.DialogResult = success ? DialogResult.OK : DialogResult.Cancel;
                this.Close();
            }
        }

        private void OnCancel(object sender, EventArgs e)
        {
            ShowErrorAndRestartApplication();
        }

        //if connection cannot be established means that download ended prematurely
        private void DealWithDownloadErrors()
        {
            try
            {
                #region Normal FTP Download
                Config config = Config.LoadConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.CLIENT_CONFIG_FILENAME));
                //Test Network is OK or not.
                ftpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri(FtpTransfer.ftpPrefix + config.ServerUrl + "/" + config.FtpPath + "/" + ConstFile.SERVER_XML_FILENAME));
                ftpWebRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                ftpWebRequest.Credentials = new NetworkCredential(config.Username, config.Password);
                ftpWebRequest.UseBinary = true;
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                ftpResponse.Close();
                #endregion
            }
            catch
            {
                //log the error message,you can use the application's log code
                ShowErrorAndRestartApplication();
            }
        }

        private void ShowErrorAndRestartApplication()
        {
            MessageBox.Show(ConstFile.NOTNETWORK, ConstFile.MESSAGETITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            CommonEntity.RestartApplication();
        }

        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Exit(false);
        }
    }
}