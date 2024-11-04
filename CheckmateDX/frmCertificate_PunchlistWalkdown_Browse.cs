using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using ProjectCommon;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckmateDX
{
    public partial class frmCertificate_PunchlistWalkdown_Browse : CheckmateDX.frmParent
    {
        //variables
        Guid _projectGuid = Guid.Empty;
        List<ViewModel_PunchlistWalkdown> _punchlistWalkdownViewModels = new List<ViewModel_PunchlistWalkdown>();
        dsPROJECT.PROJECTRow _PROJECT;
        dsCERTIFICATE_MAIN dsCERTIFICATE_MAIN = new dsCERTIFICATE_MAIN();
        AdapterCERTIFICATE_MAIN _daCERTIFICATE_MAIN = new AdapterCERTIFICATE_MAIN();
        AdapterCERTIFICATE_STATUS _daCERTIFICATE_STATUS = new AdapterCERTIFICATE_STATUS();
        AdapterCERTIFICATE_STATUS_ISSUE _daCERTIFICATE_STATUS_ISSUE = new AdapterCERTIFICATE_STATUS_ISSUE();
        AdapterUSER_MAIN _daUser = new AdapterUSER_MAIN();
        AdapterTEMPLATE_MAIN _daTemplate = new AdapterTEMPLATE_MAIN();
        List<CertificateComments> _certificateComments = new List<CertificateComments>();
        Guid _commendingCertificateStatus = Guid.Empty; //stores the status on which user will be commenting on
        string numberNamingConvention = "PLWD";

        public frmCertificate_PunchlistWalkdown_Browse()
        {
            InitializeComponent();
            //get user parameter
            _projectGuid = (Guid)System_Environment.GetUser().userProject.Value;
            punchlistWalkdownBindingSource.DataSource = _punchlistWalkdownViewModels;
            CommentBindingSource.DataSource = _certificateComments;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            selectScheduleParameters();
        }

        /// <summary>
        /// Used when user is superadmin, which doesn't have default project and discipline
        /// </summary>
        private void selectScheduleParameters()
        {
            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                frmTool_Project frmSelectProject = new frmTool_Project();
                frmSelectProject.ShowAsDialog();
                if (frmSelectProject.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _projectGuid = frmSelectProject.GetSelectedProject().GUID;
                }
                else
                {
                    this.Close();
                    return;
                }
            }

            populateProject(_projectGuid);
            refreshPunchlistWalkdowns();
        }

        private void populateProject(Guid projectGuid)
        {
            using(AdapterPROJECT daPROJECT = new AdapterPROJECT())
            {
                _PROJECT = daPROJECT.GetBy(projectGuid);
                numberNamingConvention = string.Concat(_PROJECT.NUMBER, "-", numberNamingConvention);
            }
        }

        /// <summary>
        /// Refresh all punchlist walkdown from database
        /// </summary>
        private void refreshPunchlistWalkdowns()
        {
            _punchlistWalkdownViewModels.Clear();
            List<ViewModel_PunchlistWalkdown> tempPunchlistWalkdowns = getPunchlistWalkdown();
            foreach (ViewModel_PunchlistWalkdown tempPunchlistWalkdown in tempPunchlistWalkdowns)
                Common.SetCertificateImageIndex(tempPunchlistWalkdown, _daCERTIFICATE_STATUS, _daCERTIFICATE_STATUS_ISSUE);

            _punchlistWalkdownViewModels.AddRange(tempPunchlistWalkdowns);
            gridControl.RefreshDataSource();
        }

        private List<ViewModel_PunchlistWalkdown> getPunchlistWalkdown()
        {
            using (AdapterCERTIFICATE_MAIN daCERTIFICATE_MAIN = new AdapterCERTIFICATE_MAIN())
            {
                return daCERTIFICATE_MAIN.GetCertificate<ViewModel_PunchlistWalkdown>(_projectGuid, Variables.punchlistWalkdownTemplateName);
            }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.CreatePunchlistWalkdown))
            {
                Common.Warn("You are not authorised to create punchlist walkdown certificates");
                return;
            }

            dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = _daTemplate.GetBy(Variables.punchlistWalkdownTemplateName);
            if(drTemplate == null)
            {
                MessageBox.Show("Punchlist walk down hasn't been designed, please design the template", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            frmCertificate_Add frmCertificateAdd = new frmCertificate_Add("Punchlist Walkdown");
            if (frmCertificateAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ViewModel_Certificate newViewModel_Certificate = frmCertificateAdd.GetCertificate();
                dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow drCERTIFICATE_MAIN = dsCERTIFICATE_MAIN.CERTIFICATE_MAIN.NewCERTIFICATE_MAINRow();
                drCERTIFICATE_MAIN.GUID = Guid.NewGuid();
                string number = _daCERTIFICATE_MAIN.GetNextSequence(_projectGuid, numberNamingConvention);
                if (number == null)
                    number = numberNamingConvention + "-001";

                drCERTIFICATE_MAIN.PROJECTGUID = _projectGuid;
                drCERTIFICATE_MAIN.NUMBER = number;
                drCERTIFICATE_MAIN.CERTIFICATE = drTemplate.TEMPLATE;
                drCERTIFICATE_MAIN.TEMPLATE_NAME = Variables.punchlistWalkdownTemplateName;
                drCERTIFICATE_MAIN.DESCRIPTION = newViewModel_Certificate.Description;
                drCERTIFICATE_MAIN.CREATED = DateTime.Now;
                drCERTIFICATE_MAIN.CREATEDBY = System_Environment.GetUser().GUID;
                dsCERTIFICATE_MAIN.CERTIFICATE_MAIN.AddCERTIFICATE_MAINRow(drCERTIFICATE_MAIN);
                _daCERTIFICATE_MAIN.Save(drCERTIFICATE_MAIN);
                refreshPunchlistWalkdowns();
            }
        }

        private void btnChangeDescription_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridView1.SelectedRowsCount == 0)
            {
                Common.Warn("Please select certificate to edit");
                return;
            }

            frmCertificate_Add frmCertificateAdd = new frmCertificate_Add((ViewModel_PunchlistWalkdown)gridView1.GetFocusedRow());
            if (frmCertificateAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ViewModel_Certificate editViewModel_Certificate = frmCertificateAdd.GetCertificate();
                dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow drCERTIFICATE_MAIN = _daCERTIFICATE_MAIN.GetBy(editViewModel_Certificate.GUID);
                if (drCERTIFICATE_MAIN != null)
                {
                    drCERTIFICATE_MAIN.DESCRIPTION = editViewModel_Certificate.Description;
                    drCERTIFICATE_MAIN.UPDATED = DateTime.Now;
                    drCERTIFICATE_MAIN.UPDATEDBY = System_Environment.GetUser().GUID;
                    _daCERTIFICATE_MAIN.Save(drCERTIFICATE_MAIN);
                }
                else
                    MessageBox.Show("Certificate not found, please contact IT support", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                refreshPunchlistWalkdowns();
            }
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.DeletePunchlistWalkDown))
            {
                Common.Warn("You are not authorised to delete punchlist walkdown certificates");
                return;
            }

            if (gridView1.SelectedRowsCount == 0)
            {
                Common.Warn("Please select certificate(s) to delete");
                return;
            }

            if (!Common.Confirmation("Are you sure you want to delete selected certificate(s)?", "Confirmation"))
                return;

            dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtCertificate = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable();
            int[] selectedRowIndexes = gridView1.GetSelectedRows();

            foreach (int selectedRowIndex in selectedRowIndexes)
            {
                ViewModel_Certificate selectedCertificate = (ViewModel_Certificate)gridView1.GetRow(selectedRowIndex);
                _daCERTIFICATE_MAIN.RemoveBy(selectedCertificate.GUID);
            }

            refreshPunchlistWalkdowns();
        }

        private void barButtonEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridView1.SelectedRowsCount == 0)
            {
                Common.Warn("Please select certificate");
                return;
            }

            editSelectedRow();
        }

        private void editSelectedRow()
        {
            ViewModel_PunchlistWalkdown viewModel_PunchlistWalkdown = (ViewModel_PunchlistWalkdown)gridView1.GetFocusedRow();
            if (viewModel_PunchlistWalkdown == null)
            {
                Common.Warn("Please select a certificate");
                return;
            }

            splashScreenManager1.ShowWaitForm();
            frmCertificate_Main f = new frmCertificate_Main(_projectGuid, viewModel_PunchlistWalkdown, "Punchlist Walk Down - " + viewModel_PunchlistWalkdown.Number);
            splashScreenManager1.CloseWaitForm();
            f.ShowDialog();
            refreshPunchlistWalkdowns();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            editSelectedRow();
        }

        private void updateCertificateStatus(PunchlistWalkdown_Status punchlistWalkdown_Status)
        {
            ViewModel_PunchlistWalkdown selectedPunchlistWalkdown = (ViewModel_PunchlistWalkdown)gridView1.GetFocusedRow();
            using (AdapterCERTIFICATE_STATUS daCERTIFICATE_STATUS = new AdapterCERTIFICATE_STATUS())
            {
                daCERTIFICATE_STATUS.ChangeStatus(selectedPunchlistWalkdown.GUID, (int)punchlistWalkdown_Status);
            }

            refreshPunchlistWalkdowns();
        }

        private void gridControl_MouseClick(object sender, MouseEventArgs e)
        {
            GridHitInfo hitInfo = gridView1.CalcHitInfo(new Point(e.X, e.Y));

            if (hitInfo.InRowCell)
            {
                ICertificate certificate = (ICertificate)gridView1.GetFocusedRow();
                if (certificate.ImageIndex == 6 || certificate.ImageIndex == 7)
                {
                    RefreshComments(certificate);
                    flyoutPanel.ShowPopup();
                }
                else
                    flyoutPanel.HidePopup();
            }
            else
                flyoutPanel.HidePopup();
        }

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.FieldName == "Number")
            {
                ICertificate certificate = (ICertificate)gridView1.GetRow(e.RowHandle);
                if (certificate.ImageIndex > 0)
                {
                    e.DefaultDraw();
                    Image img = imageList2.Images[certificate.ImageIndex];
                    //TODO: specify required offsets
                    e.Cache.DrawImage(img, e.Bounds.Width - 2, e.Bounds.Y + 2);
                }
            }
        }

        private void RefreshComments(ICertificate certificate)
        {
            if (certificate == null)
                return;

            Common.GetCertificateStatusComments(certificate, _certificateComments, _daCERTIFICATE_STATUS, _daCERTIFICATE_STATUS_ISSUE, _daUser, out _commendingCertificateStatus);
            treeListComments.RefreshDataSource();
            //treeListComments.BestFitColumns();
            treeListComments.ExpandAll();
            treeListComments.MoveLast();
        }

        private void btnHistory_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICertificate certificate = (ICertificate)gridView1.GetFocusedRow();
            RefreshComments(certificate);
            flyoutPanel.ShowPopup();
        }

        private void btnReport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("Loading...");
            frmReportSheet frmCertificateSheet = new frmReportSheet(_projectGuid);
            frmCertificateSheet.populateCertificates(Variables.CertificateReportType.PunchlistWalkdown);
            splashScreenManager1.CloseWaitForm();
            frmCertificateSheet.Show();
        }
    }
}