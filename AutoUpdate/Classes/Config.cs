using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace AutoUpdate
{
    public class Config
    {
        #region The private fields
        private bool enabled = true;
        private string serverUrl = string.Empty;
        private string fingerprint = string.Empty;
        private string ftpPath = string.Empty;
        private string username = string.Empty;
        private string password = string.Empty;
        private string sshkeypass = string.Empty;
        public int port = 0;
        public string keyfile = string.Empty;
        private UpdateFileList updateFileList = new UpdateFileList();
        #endregion

        #region The public property
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public string ServerUrl
        {
            get { return serverUrl; }
            set { serverUrl = value; }
        }

        public string Fingerprint
        {
            get { return fingerprint; }
            set { fingerprint = value; }
        }

        public string FtpPath
        {
            get { return ftpPath; }
            set { ftpPath = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string SSHKeyPass
        {
            get { return sshkeypass; }
            set { sshkeypass = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public string KeyFile
        {
            get { return keyfile; }
            set { keyfile = value; }
        }

        public UpdateFileList UpdateFileList
        {
            get { return updateFileList; }
            set { updateFileList = value; }
        }
        #endregion

        #region The public method
        public static Config LoadConfig(string file)
        {
            //Encryption.Decrypt_File(file);
            string text = File.ReadAllText(file);
            //string decryptText = Encryption.Decrypt(text, true);

            using(var reader = new StringReader(text))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Config));
                StreamReader sr = new StreamReader(file);
                //Config config = xs.Deserialize(sr) as Config;
                Config config = xs.Deserialize(reader) as Config;

                sr.Close();
                return config;
            }

            //Encryption.Encrypt_File(file);
        }

        public void SaveConfig(string file)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Config));
            using(StringWriter textWriter = new StringWriter())
            {
                xs.Serialize(textWriter, this);
                //string encryptedText = Encryption.Encrypt(textWriter.ToString(), true);
                //File.WriteAllText(file, encryptedText);

                File.WriteAllText(file, textWriter.ToString());
            }

            //StreamWriter sw = new StreamWriter(file);
            //xs.Serialize(sw, this);
            //sw.Close();
            //Encryption.Encrypt_File(file);
        }
        #endregion
    }

}
