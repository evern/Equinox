using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace AutoUpdater
{
    #region The delegate
    public delegate void ShowHandler();
    #endregion

    public class AutoUpdater : IAutoUpdater
    {
        #region The private fields
        private Config config = null;
        private bool bNeedRestart = false;
        private bool bDownload = false;
        List<DownloadFileInfo> downloadFileListTemp = null;
        #endregion

        #region The public event
        public event ShowHandler OnShow;
        #endregion

        #region The constructor of AutoUpdater
        public AutoUpdater()
        {
            config = Config.LoadConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.FILENAME));
        }
        #endregion

        #region The public method
        public void Update()
        {
            if (!config.Enabled)
                return;

            Dictionary<string, RemoteFile> listRemoteFile = ParseRemoteXml(config.ServerUrl);

            if (listRemoteFile.Count == 0)
                return;

            List<DownloadFileInfo> downloadList = new List<DownloadFileInfo>();

            foreach (LocalFile file in config.UpdateFileList)
            {
                if (listRemoteFile.ContainsKey(file.Path))
                {
                    RemoteFile rf = listRemoteFile[file.Path];
                    Version v1 = new Version(rf.LastVer);
                    Version v2 = new Version(file.LastVer);
                    if (v1 > v2)
                    {
                        downloadList.Add(new DownloadFileInfo(rf.Url, file.Path, rf.LastVer, rf.Size));
                        file.LastVer = rf.LastVer;
                        file.Size = rf.Size;

                        if (rf.NeedRestart)
                            bNeedRestart = true;

                        bDownload = true;
                    }

                    listRemoteFile.Remove(file.Path);
                }
            }

            foreach (RemoteFile file in listRemoteFile.Values)
            {
                downloadList.Add(new DownloadFileInfo(file.Url, file.Path, file.LastVer, file.Size));

                if (file.NeedRestart)
                    bNeedRestart = true;
            }

            downloadFileListTemp = downloadList;

            if (bDownload)
            {
                DownloadConfirm dc = new DownloadConfirm(downloadList);

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
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                    //log the error message,you can use the application's log code
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
            DownloadProgress dp = new DownloadProgress(downloadList);
            if (dp.ShowDialog() == DialogResult.OK)
            {
                //
                if (DialogResult.Cancel == dp.ShowDialog())
                {
                    return;
                }
                //Update successfully
                config.SaveConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.FILENAME));

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
                HttpWebRequest rq = WebRequest.Create(xml) as HttpWebRequest;
                //1 Second Timeout
                rq.Timeout = 1000;
                //Also note you can set the Proxy property here if required; sometimes it is, especially if you are behind a firewall - rq.Proxy = new WebProxy("proxy_address");
                HttpWebResponse response = rq.GetResponse() as HttpWebResponse;

                XmlTextReader reader = new XmlTextReader(response.GetResponseStream());
                document.Load(reader);
            }
            catch
            {
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
