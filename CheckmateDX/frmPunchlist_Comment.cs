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
    public partial class frmPunchlist_Comment : CheckmateDX.frmParent
    {
        AdapterPUNCHLIST_STATUS_ISSUE _daPunchlistStatusIssue = new AdapterPUNCHLIST_STATUS_ISSUE();
        dsPUNCHLIST_STATUS_ISSUE _dsPunchlistStatusIssue = new dsPUNCHLIST_STATUS_ISSUE();
        dsPUNCHLIST_STATUS _dsPunchlistStatus = new dsPUNCHLIST_STATUS();
        AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN();
        List<punchlistComments> _allPunchlistComments = new List<punchlistComments>();

        Guid _punchlistStatusGuid; //not used, but can be used to tag into current status
        string _statusComment;

        public frmPunchlist_Comment(Guid punchlistGuid, string operation)
        {
            InitializeComponent();
            btnOk.Text = operation;

            Common.GetPunchlistStatusComments(punchlistGuid, _allPunchlistComments, out _punchlistStatusGuid);
            punchlistCommentsBindingSource.DataSource = _allPunchlistComments;
            treeListComments.RefreshDataSource();
            treeListComments.ExpandAll();
            treeListComments.MoveLast();
        }

        public string GetComment()
        {
            return _statusComment; //used to add comments after new status has been created
        }

        #region Events
        private void btnOk_Click(object sender, EventArgs e)
        {
            //if(_punchlistStatusGuid == Guid.Empty)
            //{
            //    Common.Warn("Error occured\n\nPunchlist not found");
            //    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            //}
            //else
            //{
                //_daPunchlistStatusIssue.AddComments(_punchlistStatusGuid, txtComments.Text.Trim(), btnOk.Text == "Reject");
                _statusComment = txtComments.Text.Trim();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            //}
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion
    }
}
