using DevExpress.XtraReports.UI;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace CheckmateDX.Report
{
    public partial class rptProjectPunchlist : DevExpress.XtraReports.UI.XtraReport
    {
        List<ViewModel_PunchlistPriorityReport> reportData;
        Guid project_guid;
        public rptProjectPunchlist()
        {
            InitializeComponent();            //just get the schema by default
            project_guid = selectScheduleParameters();
            using (AdapterPUNCHLIST_MAIN daPUNCHLIST_MAIN = new AdapterPUNCHLIST_MAIN())
            {
                reportData = daPUNCHLIST_MAIN.GetPunchlistPriorityReport(project_guid);
            }

            this.objectDataSource1.DataSource = reportData;
        }

        public void ShowReport()
        {
            this.ShowPreview();
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
    }
}
