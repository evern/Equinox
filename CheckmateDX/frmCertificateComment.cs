using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectLibrary;
using System;
using System.Collections.Generic;
using ProjectCommon;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckmateDX
{
    public partial class frmCertificateComment : CheckmateDX.frmParent
    {
        AdapterCERTIFICATE_STATUS_ISSUE _daCertificateStatusIssue = new AdapterCERTIFICATE_STATUS_ISSUE();
        dsCERTIFICATE_STATUS_ISSUE _dsCertificateStatusIssue = new dsCERTIFICATE_STATUS_ISSUE();

        AdapterCERTIFICATE_STATUS _daCertificateStatus = new AdapterCERTIFICATE_STATUS();
        dsCERTIFICATE_STATUS _dsCertificateStatus = new dsCERTIFICATE_STATUS();
        List<CertificateComments> _allCertificateComments = new List<CertificateComments>();
        AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN();

        Guid _iTRStatusGuid;
        string _statusComment; //not used, but can be used to tag into current status
        public frmCertificateComment(ICertificate certificate, string operation)
        {
            InitializeComponent(); 
            btnOk.Text = operation;

            Common.GetCertificateStatusComments(certificate, _allCertificateComments, _daCertificateStatus, _daCertificateStatusIssue, _daUser, out _iTRStatusGuid);
            CertificateCommentsBindingSource.DataSource = _allCertificateComments;
            treeListComments.RefreshDataSource();
            treeListComments.ExpandAll();
            treeListComments.MoveLast();

            txtComments.Enter += new EventHandler(Common.textBox_GotFocus);
        }

        public string GetComment()
        {
            return _statusComment; //used to add comments after new status has been created
        }

        #region Events
        private void btnOk_Click(object sender, EventArgs e)
        {
            _statusComment = txtComments.Text.Trim();
            //_daITRStatusIssue.AddComments(_iTRStatusGuid, txtComments.Text.Trim(), btnOk.Text == "Reject");
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion

        protected override void OnClosed(EventArgs e)
        {
            _dsCertificateStatusIssue.Dispose();
            _daCertificateStatusIssue.Dispose();
            _daUser.Dispose();
            base.OnClosed(e);
        }
    }
}
