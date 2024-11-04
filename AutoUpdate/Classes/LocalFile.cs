using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutoUpdate
{
    public class LocalFile
    {
        #region The private fields
        private string path = "";
        private string md5 = "";
        private int size = 0;
        #endregion

        #region The public property
        [XmlAttribute("path")]
        public string Path { get { return path; } set { path = value; } }
        [XmlAttribute("lasthash")]
        public string MD5 { get { return md5; } set { md5 = value; } }
        [XmlAttribute("size")]
        public int Size { get { return size; } set { size = value; } }
        #endregion

        #region The constructor of LocalFile
        public LocalFile(string path, string ver, int size)
        {
            this.path = path;
            this.md5 = ver;
            this.size = size;
        }

        public LocalFile()
        {
        }
        #endregion

    }
}
