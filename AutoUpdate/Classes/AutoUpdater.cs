using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using WinSCP;

namespace AutoUpdate
{
    #region The delegate
    public delegate void ShowHandler();
    #endregion

    public class AutoUpdater : IAutoUpdater
    {
        #region The private fields
        private Config config = null;
        private bool bNeedRestart = false;

        //#if DEBUG
        //    private bool bDownload = true;
        //#else
            private bool bDownload = false;
        //#endif

        //AutoUpdate Progress Members
        string destinationUrl = string.Empty;
        List<DownloadFileInfo> downloadFileListTemp = null;
        #endregion

        #region The public event
        public event ShowHandler OnShow;
        #endregion

        #region The constructor of AutoUpdater
        public AutoUpdater()
        {
            config = Config.LoadConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.CLIENT_CONFIG_FILENAME));
        }
        #endregion

        #region The public method
        //use winscp to test connectivity faster
        public bool IsFtpValid()
        {
            try
            {
                Config config = Config.LoadConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.CLIENT_CONFIG_FILENAME));
                SessionOptions sessionOptions = new SessionOptions();
                sessionOptions.Protocol = Protocol.Ftp;
                sessionOptions.HostName = config.ServerUrl; //hostname e.g. IP: 192.54.23.32, or mysftpsite.com
                sessionOptions.UserName = config.Username;
                sessionOptions.Password = config.Password;
                sessionOptions.PortNumber = config.port;
                //sessionOptions.FtpMode = FtpMode.Passive;
                Session session = new Session();
                session.Open(sessionOptions);
                session.Close();

                return true;
            }
            catch(Exception ex)
            {
                string error = ex.ToString();
                return false;
            }
        }


        public void Update()
        {
            if (!config.Enabled)
                return;

            if (!IsFtpValid())
            {
                if(MessageBox.Show("Cannot connect to update server\n Do you wish to email IT support about this?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    System.Diagnostics.Process.Start("mailto:service.desk@primero.com.au");
                    return;
                }
                else
                    return;
            }

            Dictionary<string, RemoteFile> listRemoteFile = ParseRemoteXml(config.ServerUrl);

            if (listRemoteFile.Count == 0)
                return;

            List<DownloadFileInfo> downloadList = new List<DownloadFileInfo>();

            //Check current version to determine whether to replace
            foreach (LocalFile file in config.UpdateFileList)
            {
                if (listRemoteFile.ContainsKey(file.Path))
                {
                    RemoteFile rf = listRemoteFile[file.Path];
                    string v1 = rf.MD5;
                    string v2 = file.MD5;

                    if (v1 != v2)
                    {
                        downloadList.Add(new DownloadFileInfo(rf.Url, file.Path, rf.MD5, rf.Size));
                        file.MD5 = rf.MD5;
                        file.Size = rf.Size;

                        if (rf.NeedRestart)
                            bNeedRestart = true;

                        bDownload = true;
                    }

                    //Remote the file from the remote file list because it'll be used for new remote file addition
                    listRemoteFile.Remove(file.Path);
                }
            }

            //New remote file addition
            foreach (RemoteFile file in listRemoteFile.Values)
            {
                downloadList.Add(new DownloadFileInfo(file.Url, file.Path, file.MD5, file.Size));

                if (file.NeedRestart)
                    bNeedRestart = true;
            }

            downloadFileListTemp = downloadList;

            if (bDownload)
            {
                AutoUpdate_Confirm dc = new AutoUpdate_Confirm(downloadList);

                if (this.OnShow != null)
                    this.OnShow();

                if (DialogResult.OK == dc.ShowDialog())
                {
                    StartDownload(downloadList);
                }
            }
        }

        public void RollBack()
        {
            foreach (DownloadFileInfo file in downloadFileListTemp)
            {
                string tempUrlPath = CommonEntity.GetFolderUrl(file);
                string oldPath = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(tempUrlPath))
                    {
                        oldPath = Path.Combine(CommonEntity.SystemBinUrl + tempUrlPath.Substring(1), file.FileName);
                    }
                    else
                    {
                        oldPath = Path.Combine(CommonEntity.SystemBinUrl, file.FileName);
                    }

                    if (oldPath.EndsWith("_"))
                        oldPath = oldPath.Substring(0, oldPath.Length - 1);

                    MoveFolderToOld(oldPath + ".old", oldPath);

                }
                catch (Exception)
                {
                }
            }
        }


        #endregion

        #region The private method
        string newfilepath = string.Empty;
        private void MoveFolderToOld(string oldPath, string newPath)
        {
            if (File.Exists(oldPath) && File.Exists(newPath))
            {
                System.IO.File.Copy(oldPath, newPath, true);
            }
        }

        private void StartDownload(List<DownloadFileInfo> downloadList)
        {
            //if (winSCPSession == null)
            //    return;

            AutoUpdate_Progress dp = new AutoUpdate_Progress(downloadList);

            if (dp.ShowDialog() == DialogResult.OK)
            {
                //Update successfully
                config.SaveConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.CLIENT_CONFIG_FILENAME));

                if (bNeedRestart)
                {
                    //Delete the temp folder
                    Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.TEMPFOLDERNAME), true);

                    MessageBox.Show(ConstFile.APPLYTHEUPDATE, ConstFile.MESSAGETITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CommonEntity.RestartApplication();
                }
            }
        }

        private Dictionary<string, RemoteFile> ParseRemoteXml(string xml)
        {
            Dictionary<string, RemoteFile> list = new Dictionary<string, RemoteFile>();
            XmlDocument document = new XmlDocument();

            try
            {
                //winSCPSession = FtpTransfer.CreateSession();
                string tempFolderPath = Path.Combine(CommonEntity.SystemBinUrl, ConstFile.TEMPFOLDERNAME);
                if (!Directory.Exists(tempFolderPath))
                {
                    Directory.CreateDirectory(tempFolderPath);
                }

                string xamlText = FtpTransfer.GetXaml(config.FtpPath + "/" + ConstFile.SERVER_XML_FILENAME, true);
                #region Normal FTP Request
                //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(xml));
                //request.Method = WebRequestMethods.Ftp.DownloadFile;
                //request.Credentials = new NetworkCredential(decryptedUsername, decryptedPassword);
                //request.Timeout = 5000;
                //request.UseBinary = true;
                //FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                //HttpWebRequest rq = WebRequest.Create(xml) as HttpWebRequest;
                ////1 Second Timeout
                //rq.Timeout = 1000;
                ////Also note you can set the Proxy property here if required; sometimes it is, especially if you are behind a firewall - rq.Proxy = new WebProxy("proxy_address");
                //HttpWebResponse response = rq.GetResponse() as HttpWebResponse;

                //XmlTextReader reader = new XmlTextReader(response.GetResponseStream());

                //XmlTextReader reader = new XmlTextReader(response.GetResponseStream());
                //document.Load(reader); 
                #endregion

                if(xamlText != string.Empty)
                {
                    using (var reader = new StringReader(xamlText))
                    {
                        document.Load(reader);
                        reader.Close();
                    }
                }
                else
                    return list;
            }
            catch(Exception ex)
            {
                string s = ex.ToString();
                return list;
            }

            foreach (XmlNode node in document.DocumentElement.ChildNodes)
            {
                list.Add(node.Attributes["path"].Value, new RemoteFile(node));
            }

            return list;
        }
        #endregion

    }

}
