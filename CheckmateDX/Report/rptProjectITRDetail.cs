using System;
using DevExpress.XtraReports.UI;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;
using System.Collections.Generic;
using ProjectDatabase.Dataset;

namespace CheckmateDX
{
    public partial class rptProjectITRDetail : DevExpress.XtraReports.UI.XtraReport, IReport
    {
        List<ViewModel_ProjectITRLatestStatus> reportData;
        Guid project_guid;
        public rptProjectITRDetail(bool designMode)
        {
            InitializeComponent();
            //just get the schema by default
            project_guid = Guid.Empty;
            if (!designMode)
                project_guid = selectScheduleParameters();
        }

        public void Initialize_Data()
        {
            using (AdapterITR_MAIN daITR_MAIN = new AdapterITR_MAIN())
            {
                reportData = daITR_MAIN.GenerateProjectITRItemReport(project_guid);
            }

            dsPROJECT.PROJECTRow currentProject;
            using (AdapterPROJECT daPROJECT = new AdapterPROJECT())
            {
                currentProject = daPROJECT.GetBy(project_guid);
            }

            if (currentProject != null)
            {
                projectName.Value = currentProject.NUMBER + " " + currentProject.NAME;
                this.ExportOptions.PrintPreview.DefaultFileName = currentProject.NUMBER + "-RPT-ITR-" + DateTime.Now.ToShortDateString();
            }

            this.objectDataSource1.DataSource = reportData;
        }

        /// <summary>
        /// Used when user is superadmin, which doesn't have default project and discipline
        /// </summary>
        private Guid selectScheduleParameters()
        {
            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                frmTool_Project frmSelectProject = new frmTool_Project();
                frmSelectProject.ShowAsDialog();
                if (frmSelectProject.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return frmSelectProject.GetSelectedProject().GUID;
                }
            }

            return (Guid)System_Environment.GetUser().userProject.Value;
        }

        public void ShowReport()
        {
            this.ShowPreview();
        }

    }
}
