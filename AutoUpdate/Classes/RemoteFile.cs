using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AutoUpdate
{
    public class RemoteFile
    {
        #region The private fields
        private string path = "";
        private string url = "";
        private string md5 = "";
        private int size = 0;
        private bool needRestart = false;
        #endregion

        #region The public property
        public string Path { get { return path; } }
        public string Url { get { return url; } }
        public string MD5 { get { return md5; } }
        public int Size { get { return size; } }
        public bool NeedRestart { get { return needRestart; } }
        #endregion

        #region The constructor of AutoUpdater
        public RemoteFile(string Path, string URL, string MD5, long Size, bool NeedRestart)
        {
            this.path = Path;
            this.url = URL;
            this.md5 = MD5;
            this.size = Convert.ToInt32(Size);
            this.needRestart = NeedRestart;
        }

        public RemoteFile(XmlNode node)
        {
            this.path = node.Attributes["path"].Value;
            this.url = node.Attributes["url"].Value;
            this.md5 = node.Attributes["lasthash"].Value;
            this.size = Convert.ToInt32(node.Attributes["size"].Value);
            this.needRestart = Convert.ToBoolean(node.Attributes["needRestart"].Value);
        }
        #endregion
    }
}
