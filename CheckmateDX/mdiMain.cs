using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectLibrary;
using DevExpress.XtraBars;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using DevExpress.XtraReports.UI;
using System.IO;
using DevExpress.Mvvm;
using DevExpress.Xpf.Printing;
using System.Linq;
using CheckmateDX.Report;

namespace CheckmateDX
{
    public partial class mdiMain : CheckmateDX.frmParent
    {
        public delegate void UnhideLoginForm();
        public UnhideLoginForm ShowLoginForm;

        public mdiMain()
        {
            InitializeComponent();
            ProcessPrivileges();
            barUsername.Caption = System_Environment.GetUser().userFirstName + " " + System_Environment.GetUser().userLastName;
            barProject.Caption = System_Environment.GetUser().userProject.Label;
            barRole.Caption = System_Environment.GetUser().userRole.Label;
            barDiscipline.Caption = System_Environment.GetUser().userDiscipline;
            populateCustomReport();
        }

        private void populateCustomReport()
        {
            barITRCustomReport.ClearLinks();

            using (AdapterREPORT adapter_report = new AdapterREPORT())
            {
                dsREPORT.REPORTDataTable reports = adapter_report.GetByProject((Guid)System_Environment.GetUser().userProject.Value);
                if (reports == null)
                    return;

                foreach (dsREPORT.REPORTRow report in reports.Rows)
                {
                    BarSubItem item = new BarSubItem();
                    item.Caption = report.REPORT_NAME;
                    item.Tag = report.GUID;

                    BarButtonItem itemPreview = new BarButtonItem(this.barManager1, "Preview");
                    itemPreview.Tag = report.GUID;
                    itemPreview.ItemClick += Item_ItemClick;

                    BarButtonItem itemEdit = new BarButtonItem(this.barManager1, "Edit");
                    itemEdit.Tag = report.GUID;
                    itemEdit.ItemClick += Item_ItemEditClick;

                    BarButtonItem itemRename = new BarButtonItem(this.barManager1, "Rename");
                    itemRename.Tag = report.GUID;
                    itemRename.ItemClick += Item_ItemRenameClick;

                    BarButtonItem itemDelete = new BarButtonItem(this.barManager1, "Delete");
                    itemDelete.Tag = report.GUID;
                    itemDelete.ItemClick += Item_ItemDeleteClick;

                    item.ItemLinks.Add(itemPreview);
                    item.ItemLinks.Add(itemEdit);
                    item.ItemLinks.Add(itemRename);
                    item.ItemLinks.Add(itemDelete);

                    barITRCustomReport.ItemLinks.Add(item);
                }
            }
        }

        private void Item_ItemClick(object sender, ItemClickEventArgs e)
        {
            Guid report_guid = (Guid)e.Item.Tag;
            dsREPORT.REPORTRow report_row;
            XtraReport report = getReport(report_guid, false, out report_row);
            if (report != null)
                report.ShowPreview();
        }

        private void Item_ItemEditClick(object sender, ItemClickEventArgs e)
        {
            Guid report_guid = (Guid)e.Item.Tag;
            dsREPORT.REPORTRow report_row;
            XtraReport report = getReport(report_guid, true, out report_row);
            if (report != null)
            {
                frmUserReport_Designer f = new frmUserReport_Designer(report, report_row);
                f.ShowDialog();
            }
        }

        private XtraReport getReport(Guid report_guid, bool designMode, out dsREPORT.REPORTRow report_row)
        {
            using (AdapterREPORT adapter_report = new AdapterREPORT())
            {
                report_row = adapter_report.GetBy(report_guid);
                if (report_row == null)
                    return null;

                XtraReport custom_report;
                if (report_row.REPORT_TYPE == "rptProjectITR")
                    custom_report = new rptProjectITR();
                else if (report_row.REPORT_TYPE == "rptProjectITRDetail")
                    custom_report = new rptProjectITRDetail(designMode);
                else
                    custom_report = new rptPunchlist(designMode);

                var report_string = report_row.REPORT;
                using (var sw = new StreamWriter(new MemoryStream()))
                {
                    sw.Write(report_string);
                    sw.Flush();
                    custom_report.LoadLayout(sw.BaseStream);

                    IReport ireport = custom_report as IReport;
                    ireport.Initialize_Data();
                    return custom_report;
                }
            }
        }

        private void Item_ItemDeleteClick(object sender, ItemClickEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete report named " + e.Item.Caption + "?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            Guid report_guid = (Guid)e.Item.Tag;
            using (AdapterREPORT adapter_report = new AdapterREPORT())
            {
                adapter_report.RemoveBy(report_guid);
            }

            populateCustomReport();
        }

        private void Item_ItemRenameClick(object sender, ItemClickEventArgs e)
        {
            Guid report_guid = (Guid)e.Item.Tag;
            using (AdapterREPORT adapter_report = new AdapterREPORT())
            {
                dsREPORT.REPORTRow report = adapter_report.GetBy(report_guid);
                if (report == null)
                    return;

                using (frmUserReportName report_name_form = new frmUserReportName())
                {
                    if (report_name_form.ShowDialog() == DialogResult.OK)
                    {
                        string report_name = report_name_form.Report_Name;
                        report.REPORT_NAME = report_name;
                        adapter_report.Save(report);
                    }
                }
            }

            populateCustomReport();
        }

        /// <summary>
        /// Change button visibility depending on user privileges
        /// </summary>
        private void ProcessPrivileges()
        {   
            //Buttons Visibility
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.ManageUser))
            {
                bbtnUser.Visibility = BarItemVisibility.Never;
                bBtnProjectAuth.Visibility = BarItemVisibility.Never;
                bBtnDisciplineAuth.Visibility = BarItemVisibility.Never;
            } 

            if (!System_Environment.HasPrivilege(PrivilegeTypeID.ManageProject))
                bbtnProject.Visibility = BarItemVisibility.Never;

            if (!System_Environment.HasPrivilege(PrivilegeTypeID.ManageTemplate))
            {
                bBtnManageTemplate.Visibility = BarItemVisibility.Never;
                bBtnPunchlistDesign.Visibility = BarItemVisibility.Never;
            }
                
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.ManagePrefill))
                bBtnPrefill.Visibility = BarItemVisibility.Never;

            if (!System_Environment.HasPrivilege(PrivilegeTypeID.ManageEquipment))
                bBtnEquipment.Visibility = BarItemVisibility.Never;

            if (!System_Environment.HasPrivilege(PrivilegeTypeID.ManageSchedule))
                bBtnSchedule.Visibility = BarItemVisibility.Never;

            if (!System_Environment.HasPrivilege(PrivilegeTypeID.ManageRole))
                bBtnRoles.Visibility = BarItemVisibility.Never;

            if (!System_Environment.HasPrivilege(PrivilegeTypeID.ManageDatabase))
            {
                bBtnDatabase.Visibility = BarItemVisibility.Never;
                bBtnPair.Visibility = BarItemVisibility.Never;
                bBtnResolveConflict.Visibility = BarItemVisibility.Never;
                bBtnSyncManage.Visibility = BarItemVisibility.Never;
                bBtnDatabaseCleanup.Visibility = BarItemVisibility.Never;
            }

            //SubItem Visibility
            if (bbtnUser.Visibility == BarItemVisibility.Never &&
                bbtnProject.Visibility == BarItemVisibility.Never)
                barSubItemEnvironment.Visibility = BarItemVisibility.Never;

            if (bBtnProjectAuth.Visibility == BarItemVisibility.Never &&
                bBtnDisciplineAuth.Visibility == BarItemVisibility.Never &&
                bBtnRoles.Visibility == BarItemVisibility.Never)
                barSubItemPermission.Visibility = BarItemVisibility.Never;

            if (bBtnManageTemplate.Visibility == BarItemVisibility.Never &&
                bBtnPrefill.Visibility == BarItemVisibility.Never)
                barSubItemTemplate.Visibility = BarItemVisibility.Never;
        }

        public void OpenChild(Type FormType, bool skipChildCheckForAdmin = false)
        {
            bool hasChild = false;
            foreach (Form f in this.MdiChildren)
            {
                if (f.GetType() == FormType)
                {
                    f.Activate();
                    f.StartPosition = FormStartPosition.CenterParent;

                    if(!skipChildCheckForAdmin || (Guid)System_Environment.GetUser().userRole.Value != Guid.Empty)
                        hasChild = true;
                }
            }

            if (!hasChild)
            {
                splashScreenManager1.ShowWaitForm();
                frmParent Child = ((frmParent)(Activator.CreateInstance(FormType, new object[] { })));
                Child.MdiParent = this;
                splashScreenManager1.CloseWaitForm();
                Child.Show();
            }
        }

        private void bbtnProject_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenChild(typeof(frmTool_Project));
        }

        private void bbtnUser_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenChild(typeof(frmTool_User));
        }

        private void bBtnProjectAuth_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenChild(typeof(frmTool_Project_Auth));
        }

        private void bBtnDisciplineAuth_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenChild(typeof(frmTool_Discipline_Auth));
        }

        private void bBtnRoles_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenChild(typeof(frmTool_Role));
        }

        private void bBtnChangePassword_ItemClick(object sender, ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            using (frmTool_User_ChangePassword f = new frmTool_User_ChangePassword())
            {
                splashScreenManager1.CloseWaitForm();
                f.ShowDialog();
            }
        }

        private void bBtnChangeSignature_ItemClick(object sender, ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            using (frmTool_User_ChangeSignature f = new frmTool_User_ChangeSignature())
            {
                splashScreenManager1.CloseWaitForm();
                f.ShowDialog();   
            }
        }

        private void bBtnChangeDefault_ItemClick(object sender, ItemClickEventArgs e)
        {
            AdapterUSER_MAIN daUser = new AdapterUSER_MAIN();
            dsUSER_MAIN.USER_MAINRow drUser = daUser.GetBy(System_Environment.GetUser().GUID);

            if(drUser != null)
            {
                splashScreenManager1.ShowWaitForm();

                frmLogin_Select frmLoginSelect = new frmLogin_Select(drUser.GUID);

                splashScreenManager1.CloseWaitForm();
                if (frmLoginSelect.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (frmLoginSelect.getDefaultProject() != Guid.Empty)
                    {
                        drUser.DPROJECT = frmLoginSelect.getDefaultProject();
                        drUser.DDISCIPLINE = frmLoginSelect.getDefaultDiscipline();
                        daUser.Save(drUser);

                        System_Environment.ChangeDefaultDiscipline(drUser.DDISCIPLINE);
                        System_Environment.ChangeDefaultProject(new ValuePair(Common.ConvertProjectGuidToName(drUser.DPROJECT), drUser.DPROJECT));

                        barProject.Caption = System_Environment.GetUser().userProject.Label;
                        barDiscipline.Caption = System_Environment.GetUser().userDiscipline;
                    }
                }
                else
                    return;
            }
        }

        private void bBtnReport_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void bBtnTemplate_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmTemplate_Browse));
        }

        private void bBtnHeader_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmPrefill_Main));
        }

        private void bBtnSchedule_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmSchedule_Main));
        }

        private void bBtnEquipment_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmEquipment_Main));
        }

        private void bBtnITR_ItemClick(object sender, ItemClickEventArgs e)
        {
            //using(frmITR_Browse f= new frmITR_Browse(true))
            //{
            //    f.ShowDialog();
            //}
            OpenChild(typeof(frmITR_Browse), true);
        }

        private void bBtnPunchlist_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmPunchlist_Browse), true);
        }

        private void bBtnDatabase_ItemClick(object sender, ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            using (frmDatabase_Config f = new frmDatabase_Config(false))
            {
                splashScreenManager1.CloseWaitForm();
                f.ShowDialog();
            }
        }

        private void barBtnPair_ItemClick(object sender, ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            using (frmDatabase_Config f = new frmDatabase_Config(true))
            {
                splashScreenManager1.CloseWaitForm();
                f.ShowDialog();
            }
        }

        private void bBtnSyncManage_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmSync_Manage));
        }

        private void bBtnToggles_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmTemplate_Toggle));
        }

        private void bBtnPunchlistDesign_ItemClick(object sender, ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            using (frmTemplate_Main f = new frmTemplate_Main(Variables.punchlistTemplateName, false))
            {
                splashScreenManager1.CloseWaitForm();
                f.ShowDialog();
            }
        }

        private void bBtnSync_ItemClick(object sender, ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            using(frmSync_Status f = new frmSync_Status())
            {
                splashScreenManager1.CloseWaitForm();
                f.ShowDialog();
            }
        }

        private void btnForceSync_ItemClick(object sender, ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            using (frmSync_Status f = new frmSync_Status(false, true))
            {
                splashScreenManager1.CloseWaitForm();
                f.ShowDialog();
            }
        }

        private void BtnStressTest_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (frmSyncTestCount f = new frmSyncTestCount())
            {
                if(f.ShowDialog() == DialogResult.OK)
                {
                    decimal testCount = f.GetTestCount();
                    for(int i = 0;i < testCount;i++)
                    {
                        frmSync_Status fSync = new frmSync_Status();
                        fSync.Show();
                    }
                }
            }
        }

        private void bBtnMyHistory_ItemClick(object sender, ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            using (frmSync_Status f = new frmSync_Status(Common.GetHWID(), Environment.MachineName))
            {
                splashScreenManager1.CloseWaitForm();
                f.ShowDialog();
            }
        }

        private void bBtnResolveConflict_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmSync_Manage_Conflict));
        }

        private void bBtnMatrix_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmSchedule_Matrix));
        }

        private void btnDatabaseCleanup_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmSync_Cleanup));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ShowLoginForm != null)
                ShowLoginForm();

            System_Environment.Clear();
            base.OnClosing(e);
        }

        private void tileNavPane1_TileClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            if (e.Element.Caption == "Project")
                bbtnProject_ItemClick(null, null);


            tileNavPane1.HideDropDownWindow();
        }

        private void bBtnITRReport_ItemClick(object sender, ItemClickEventArgs e)
        {
            rptProjectITR f = new rptProjectITR();
            f.ShowReport();
        }

        private void bBtnProjectPunchlistReport_ItemClick(object sender, ItemClickEventArgs e)
        {
            rptProjectPunchlist f = new rptProjectPunchlist();
            f.ShowReport();
        }

        private void bBtnSubsystemPunchlistReport_ItemClick(object sender, ItemClickEventArgs e)
        {
            rptSubsystemPunchlist f = new rptSubsystemPunchlist();
            f.ShowReport();
        }

        private void barButtonAddCustomReport_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmUserReport_Designer f = new frmUserReport_Designer(() => populateCustomReport());
            f.ShowDialog();
        }

        private void btnDesignPunchlistWalkdown_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(!System_Environment.HasPrivilege(PrivilegeTypeID.DesignPunchlistWalkdown))
            {
                Common.Warn("You are not authorised to design punchlist walkdown template");
                return;
            }

            splashScreenManager1.ShowWaitForm();
            using (frmTemplate_Main f = new frmTemplate_Main(Variables.punchlistWalkdownTemplateName, true))
            {
                splashScreenManager1.CloseWaitForm();
                f.ShowDialog();
            }
        }

        private void bBtnCVCDesign_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.DesignCVC))
            {
                Common.Warn("You are not authorised to design CVC template");
                return;
            }

            splashScreenManager1.ShowWaitForm();
            using (frmTemplate_Main f = new frmTemplate_Main(Variables.constructionVerificationCertificateTemplateName, true))
            {
                splashScreenManager1.CloseWaitForm();
                f.ShowDialog();
            }
        }

        private void bBtnNOEDesign_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.DesignNOE))
            {
                Common.Warn("You are not authorised to design NOE template");
                return;
            }

            splashScreenManager1.ShowWaitForm();
            using (frmTemplate_Main f = new frmTemplate_Main(Variables.noticeOfEnergisationCertificateTemplateName, true))
            {
                splashScreenManager1.CloseWaitForm();
                f.ShowDialog();
            }
        }

        private void barButtonItemPLWalkdown_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmCertificate_PunchlistWalkdown_Browse));
        }

        private void barButtonItemCVC_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmCertificate_CVC_Browse));
        }

        private void barButtonItemNOE_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenChild(typeof(frmCertificate_NOE_Browse));
        }

        private void bBtnSubsystemReport_ItemClick(object sender, ItemClickEventArgs e)
        {
            rptSubsystem f = new rptSubsystem();
            f.ShowReport();

            //frmWBS_Selection frmWBS_Selection = new frmWBS_Selection();
            //if (frmWBS_Selection.ShowDialog() == DialogResult.OK)
            //{
            //    List<wbsTagDisplay> selectedSubsystems = frmWBS_Selection.GetSelectedWBSTags();
            //    List<Guid> selectedSubsystemGuids = selectedSubsystems.Select(x => x.wbsTagDisplayGuid).ToList();
            //}
        }

        private void btnITRStatusBreakdownReport_ItemClick(object sender, ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            frmMaster_ITR_Status_Breakdown_Report masterITRStatusBreakdownReport = new frmMaster_ITR_Status_Breakdown_Report((Guid)System_Environment.GetUser().userProject.Value);
            splashScreenManager1.CloseWaitForm();
            masterITRStatusBreakdownReport.Show();
        }
    }
}
