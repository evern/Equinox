using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AutoUpdate
{
    public partial class XMLWriter : DevExpress.XtraEditors.XtraForm
    {
        public XMLWriter()
        {
            InitializeComponent();

            txtFtpAddress.Text = "checkmate.primerogroup.com.au";
            txtFtpPath.Text = "/CheckmateUpdate";
            txtPort.Text = "21";
            txtUsername.Text = "ftpuser";
            txtPassword.Text = "&Ch9^k%usZ!5UtF";
            txtAppFolder.Text = AppDomain.CurrentDomain.BaseDirectory;
            txtXMLFolder.Text = AppDomain.CurrentDomain.BaseDirectory;

            ////for testing purpose
            //AutoUpdate.AutoUpdater au = new AutoUpdate.AutoUpdater();
            //au.Update();
        }

        #region Button Events
        private void btnKeyBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            fDialog.DefaultExt = "ppk";
            fDialog.Filter = "Putty Key File (*.ppk)|*.ppk|All Files (*.*)|*.*";
            if(fDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtKeyPath.Text = fDialog.FileName;
            }
        }

        private void btnUpdateBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fDialog = new FolderBrowserDialog();
            fDialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
            if(fDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtAppFolder.Text = fDialog.SelectedPath;
                txtXMLFolder.Text = fDialog.SelectedPath;
            }
        }

        private void btnXMLBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fDialog = new FolderBrowserDialog();
            fDialog.RootFolder = Environment.SpecialFolder.Desktop;
            if(fDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtXMLFolder.Text = fDialog.SelectedPath;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if(ValidateForm())
            {
                bool isClient = (bool)radioGroupType.EditValue;
                string[] fileEntries = Directory.GetFiles(txtAppFolder.Text.Trim());
                List<LocalFile> localFiles = new List<LocalFile>();
                List<RemoteFile> remoteFiles = new List<RemoteFile>();

                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetMax, fileEntries.Count());

                foreach (string fileName in fileEntries)
                {
                    List<string> fileDescription = fileName.Split('\\').ToList();
                    splashScreenManager1.SetWaitFormDescription("Processing " + fileDescription.Last());
                    FileInfo fi = new FileInfo(fileName);
                    byte[] calculateMD5;
                    using(var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(fileName))
                        {
                            calculateMD5 = md5.ComputeHash(stream);
                        }
                    }

                    string md5Text = Convert.ToBase64String(calculateMD5);

                    if(isClient)
                    {
                        LocalFile lf = new LocalFile(fi.Name, md5Text, Convert.ToInt32(fi.Length));
                        localFiles.Add(lf);
                    }
                    else
                    {
                        string ftpPath = txtFtpPath.Text.Trim();
                        if (ftpPath.Last() != '/')
                            ftpPath += "/";

                        RemoteFile rf = new RemoteFile(fi.Name, ftpPath + fi.Name, md5Text, fi.Length, true);
                        remoteFiles.Add(rf);
                    }
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, 0);
                }

                splashScreenManager1.CloseWaitForm();
                if ((bool)radioGroupType.EditValue)
                {
                    WriteXML_Client(localFiles);

                    if ((bool)checkEncrypt.EditValue)
                    {
                        Encryption.Encrypt_File(txtKeyPath.Text.Trim());
                        MessageBox.Show("Client XML Written and Key File Encrypted");
                    }
                    else
                        MessageBox.Show("Client XML Written");
                }
                else
                {
                    WriteXML_Server(remoteFiles);
                    MessageBox.Show("Server XML Written");
                }
            }
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Excluded string to scan for writing client config and server xml
        /// </summary>
        private List<string> Populate_Excluded_String()
        {
            List<string> ExcludedString = new List<string>();
            ExcludedString.Add(".log");
            ExcludedString.Add(".ppk");
            ExcludedString.Add(".old");
            ExcludedString.Add(".pdb");
            ExcludedString.Add(".sdf");
            ExcludedString.Add("vshost");
            ExcludedString.Add("manifest");
            ExcludedString.Add(".txt");
            ExcludedString.Add(".application");
            ExcludedString.Add(".config");
            ExcludedString.Add(ConstFile.CLIENT_CONFIG_FILENAME);
            ExcludedString.Add(ConstFile.SERVER_XML_FILENAME);
            return ExcludedString;
        }

        /// <summary>
        /// Excluded deletion strings
        /// </summary>
        /// <returns></returns>
        private List<string> Populate_Delete_Exclusions()
        {
            List<string> Delete_Exclusions = new List<string>();
            Delete_Exclusions.Add(ConstFile.CLIENT_CONFIG_FILENAME);
            Delete_Exclusions.Add(ConstFile.SERVER_XML_FILENAME);

            List<string> keyPath = txtKeyPath.Text.Split('\\').ToList();
            Delete_Exclusions.Add(keyPath.Last());

            return Delete_Exclusions;
        }

        /// <summary>
        /// Validate and format the textbox fields
        /// </summary>
        /// <returns>Validate Result</returns>
        private bool ValidateForm()
        {
            if (txtFtpAddress.Text.Trim() == "")
            {
                MessageBox.Show("Please enter the root path where the updated files will be located", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtFtpAddress.Focus();
                return false;
            }

            string URLText = txtFtpAddress.Text.Trim();
            if (URLText.EndsWith("/"))
            {
                txtFtpAddress.Text = URLText.Substring(1, URLText.Length - 1);
            }

            if (txtAppFolder.Text.Trim() == "")
            {
                MessageBox.Show("Please browse to the path where all the update files are located", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnUpdateBrowse.Focus();
                return false;
            }

            if (txtXMLFolder.Text.Trim() == "")
            {
                MessageBox.Show("Please browse to the path where the xml file will be created", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnXMLBrowse.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates the client XML file
        /// </summary>
        /// <param name="localFiles">Local File Properties</param>
        private void WriteXML_Client(List<LocalFile> localFiles)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            string xmlFilePath = txtXMLFolder.Text.Trim() + "\\AutoUpdateClient.config";
            using (XmlWriter writer = XmlWriter.Create(xmlFilePath, xmlSettings))
            {
                writer.WriteStartElement("Config");
                writer.WriteStartElement("Enabled");
                writer.WriteString("true");
                writer.WriteEndElement();
                writer.WriteStartElement("ServerUrl");
                writer.WriteString(txtFtpAddress.Text.Trim());
                writer.WriteEndElement();
                writer.WriteStartElement("Fingerprint");
                writer.WriteString(txtFingerprint.Text.Trim());
                writer.WriteEndElement();
                writer.WriteStartElement("FtpPath");
                writer.WriteString(txtFtpPath.Text);
                writer.WriteEndElement();
                writer.WriteStartElement("Port");
                writer.WriteString(txtPort.Text);
                writer.WriteEndElement();

                List<string> keyPath = txtKeyPath.Text.Split('\\').ToList();

                writer.WriteStartElement("KeyFile");
                writer.WriteString(keyPath.Last());
                writer.WriteEndElement();
                writer.WriteStartElement("Username");
                writer.WriteString(txtUsername.Text.Trim());
                writer.WriteEndElement();
                writer.WriteStartElement("Password");
                writer.WriteString(txtPassword.Text.Trim());
                writer.WriteEndElement();
                writer.WriteStartElement("SSHKeyPass");
                writer.WriteString(txtSSHKeyPass.Text.Trim());
                writer.WriteEndElement();
                writer.WriteStartElement("UpdateFileList");

                List<string> ExcludedFileString = Populate_Excluded_String();
                List<string> DeleteExclusions = Populate_Delete_Exclusions();
                List<string> RemoveFilePath = new List<string>();
                foreach (LocalFile localfile in localFiles)
                {
                    if (!ExcludedFileString.Any(obj => localfile.Path.Contains(obj)))
                    {
                        writer.WriteStartElement("LocalFile");
                        writer.WriteAttributeString("path", localfile.Path);
                        writer.WriteAttributeString("lasthash", localfile.MD5);
                        writer.WriteAttributeString("size", localfile.Size.ToString());
                        writer.WriteEndElement();
                    }
                    else
                    {
                        if(!DeleteExclusions.Any(obj => localfile.Path.Contains(obj)))
                        {
                            #if DEBUG
                                RemoveFilePath.Add(localfile.Path);
                            #else
                                File.Delete(localfile.Path);
                            #endif
                        }
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            //Encryption.Encrypt_File(xmlFilePath);
        }

        /// <summary>
        /// Creates the server XML file
        /// </summary>
        /// <param name="localFiles">Remote File Properties</param>
        private void WriteXML_Server(List<RemoteFile> remoteFiles)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            string xmlFilePath = txtXMLFolder.Text.Trim() + "\\AutoUpdateServer.xml";
            using (XmlWriter writer = XmlWriter.Create(xmlFilePath, xmlSettings))
            {
                writer.WriteStartElement("UpdateFileList");
                List<string> ExcludedFileString = Populate_Excluded_String();
                List<string> DeleteExclusions = Populate_Delete_Exclusions();
                foreach (RemoteFile remotefile in remoteFiles)
                {
                    if (!ExcludedFileString.Any(obj => remotefile.Path.Contains(obj)))
                    {
                        writer.WriteStartElement("LocalFile");
                        writer.WriteAttributeString("path", remotefile.Path);
                        writer.WriteAttributeString("url", remotefile.Url);
                        writer.WriteAttributeString("lasthash", remotefile.MD5);
                        writer.WriteAttributeString("size", remotefile.Size.ToString());
                        writer.WriteAttributeString("needRestart", remotefile.NeedRestart.ToString());
                        writer.WriteEndElement();
                    }
                    else
                    {
                        if (!DeleteExclusions.Any(obj => remotefile.Path.Contains(obj)))
                        {
                            #if DEBUG
                            #else
                                File.Delete(remotefile.Path);
                            #endif
                        }
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            //Encryption.Encrypt_File(xmlFilePath);
        }
        #endregion

        private void radioGroupType_EditValueChanged(object sender, EventArgs e)
        {
            if((bool)radioGroupType.EditValue)
            {
                pnlUsername.Enabled = true;
                pnlPassword.Enabled = true;
                pnlPort.Enabled = true;
                pnlKey.Enabled = true;
                pnlFingerPrint.Enabled = true;
            }
            else
            {
                pnlUsername.Enabled = false;
                pnlPassword.Enabled = false;
                pnlPort.Enabled = false;
                pnlKey.Enabled = false;
                pnlFingerPrint.Enabled = false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D))
            {
                Authentication auth = new Authentication();
                if (auth.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (auth.GetPassword() == "lhpwrma6")
                    {
                        txtFtpAddress.Text = "checkmate.primero.com.au";
                        txtFtpPath.Text = "/CheckmateUpdate";
                        //txtFingerprint.Text = "ssh-rsa 2048 5a:04:5e:e9:a8:5b:c7:04:93:f3:8a:d2:72:f7:28:98";
                        txtPort.Text = "21";
                        txtUsername.Text = "ftpuser";
                        txtPassword.Text = "&Ch9^k%usZ!5UtF";
                        //txtSSHKeyPass.Text = "P4y57zcvP";
                    }
                    else
                    {
                        MessageBox.Show("Incorrect Password");
                    }

                    return true;
                }
                else
                    return false;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
