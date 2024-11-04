using DevExpress.XtraEditors;
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
    public partial class rptSubsystem : DevExpress.XtraReports.UI.XtraReport
    {
        List<ViewModel_ProjectReport> reportData;
        Guid project_guid;
        public rptSubsystem()
        {
            InitializeComponent();
            //just get the schema by default
            project_guid = selectScheduleParameters();
            using (AdapterITR_MAIN daITR_MAIN = new AdapterITR_MAIN())
            {
                reportData = daITR_MAIN.GenerateProjectITRReport(project_guid);
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

        private void rptSubsystem_ParametersRequestBeforeShow(object sender, DevExpress.XtraReports.Parameters.ParametersRequestEventArgs e)
        {
            this.FillDataSources(true);
            object editor = e.ParametersInformation[0].Editor;
            if (editor is PopupBaseEdit)
            {
                ((PopupBaseEdit)editor).Properties.PopupFormSize = new Size(800, 500);
            }
        }
    }
}
