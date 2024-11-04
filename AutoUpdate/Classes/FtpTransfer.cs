using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AutoUpdate
{
    public static class FtpTransfer
    {
        public static string ftpPrefix = "ftp://";
        public static string GetXaml(string pathOfFileToGet, bool asciiMode = false)
        {
            Config config = Config.LoadConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.CLIENT_CONFIG_FILENAME));
            FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri(ftpPrefix + config.ServerUrl + pathOfFileToGet));
            ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            ftpWebRequest.Credentials = new NetworkCredential(config.Username, config.Password);
            ftpWebRequest.Timeout = 5000;
            ftpWebRequest.UseBinary = true;

            FtpWebResponse ftpResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
            Stream responseStream = ftpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string resultString = reader.ReadToEnd();
            return resultString;
        }
    }
}
