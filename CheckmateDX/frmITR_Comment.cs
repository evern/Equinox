using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;

namespace CheckmateDX
{
    public partial class frmITR_Comment : CheckmateDX.frmParent
    {
        AdapterITR_STATUS_ISSUE _daITRStatusIssue = new AdapterITR_STATUS_ISSUE();
        dsITR_STATUS_ISSUE _dsITRIssue = new dsITR_STATUS_ISSUE();
        AdapterITR_STATUS _daITRStatus = new AdapterITR_STATUS();
        dsITR_STATUS _dsITRStatus = new dsITR_STATUS();
        AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN();
        List<iTRComments> _allITRComments = new List<iTRComments>();

        Guid _iTRStatusGuid;
        string _statusComment; //not used, but can be used to tag into current status

        public frmITR_Comment(Guid iTRGuid, string operation)
        {
            InitializeComponent();
            btnOk.Text = operation;

            Common.GetITRStatusComments(iTRGuid, _allITRComments, out _iTRStatusGuid);
            iTRCommentsBindingSource.DataSource = _allITRComments;
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
            _daITRStatus.Dispose();
            _daITRStatusIssue.Dispose();
            _daUser.Dispose();
            base.OnClosed(e);
        }
    }
}
