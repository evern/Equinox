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
using System.IO;

namespace AutoUpdate
{
    public partial class AutoUpdate_Confirm : DevExpress.XtraEditors.XtraForm
    {
        #region The private fields
        List<DownloadFileInfo> downloadFileList = null;
        #endregion

        public AutoUpdate_Confirm(List<DownloadFileInfo> downloadfileList)
        {
            InitializeComponent();
            Config config = Config.LoadConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstFile.CLIENT_CONFIG_FILENAME));
            lblFrom.Text = "From: " + config.ServerUrl.Replace(ConstFile.SERVERXML, string.Empty);
            this.downloadFileList = downloadfileList;
        }

        #region The private method
        private void OnLoad(object sender, EventArgs e)
        {
            foreach (DownloadFileInfo file in this.downloadFileList)
            {
                ListViewItem item = new ListViewItem(new string[] { file.FileName, file.LastVer, file.Size.ToString() });
            }

            this.Activate();
            this.Focus();
        }
        #endregion

        #region Button Events
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion
    }
}