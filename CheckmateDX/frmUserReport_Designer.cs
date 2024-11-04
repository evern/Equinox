using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CheckmateDX
{
    public partial class frmUserReport_Designer : Form
    {
        private Action refresh_report;
        private dsREPORT.REPORTRow edit_report;
        public frmUserReport_Designer(Action update_custom_report)
        {
            InitializeComponent();
            refresh_report = update_custom_report;
        }

        public frmUserReport_Designer(XtraReport report, dsREPORT.REPORTRow report_row)
        {
            InitializeComponent();
            edit_report = report_row;
            currentREPORT = report;
            reportDesigner1.OpenReport(report);
        }

        private XtraReport currentREPORT;
        private AdapterREPORT daREPORT = new AdapterREPORT();
        private void barButtonGeneralReport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            currentREPORT = new rptProjectITR_OLD(true);
            reportDesigner1.OpenReport(currentREPORT);
        }

        private void barButtonDetailReport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            currentREPORT = new rptProjectITRDetail(true);
            reportDesigner1.OpenReport(currentREPORT);
        }

        private void barButtonPunchlistReport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            currentREPORT = new rptPunchlist(true);
            reportDesigner1.OpenReport(currentREPORT);
        }

        private void reportDesigner1_DesignPanelLoaded(object sender, DesignerLoadedEventArgs e)
        {
            var panel = (XRDesignPanel)sender;
            panel.AddCommandHandler(new SaveCommandHandler(panel, SaveReport));
        }

        private void SaveReport()
        {
            // Save the report to a stream.
            var ms = new MemoryStream();
            //Prevent user from editing parameters
            //currentREPORT.Parameters.Clear();
            currentREPORT.SaveLayout(ms);
            // Prepare the stream for reading.
            ms.Position = 0;
            string reportString;
            // Insert the report to a database.
            using (var sr = new StreamReader(ms))
            {
                // Read the report from the stream to a string variable.
                reportString = sr.ReadToEnd();
            }

            if (reportString == string.Empty)
            {
                MessageBox.Show("Error in saving report");
                return;
            }

            if (edit_report != null)
            {
                edit_report.REPORT = reportString;
                edit_report.UPDATED = DateTime.Now;
                edit_report.UPDATEDBY = System_Environment.GetUser().GUID;
                daREPORT.Save(edit_report);
            }
            else
            {
                using (frmUserReportName report_name_form = new frmUserReportName())
                {
                    if (report_name_form.ShowDialog() == DialogResult.OK)
                    {
                        string report_name = report_name_form.Report_Name;
                        using (AdapterREPORT daREPORT = new AdapterREPORT())
                        {
                            dsREPORT report_dataset = new dsREPORT();
                            dsREPORT.REPORTRow new_report = report_dataset.REPORT.NewREPORTRow();
                            new_report.GUID = Guid.NewGuid();
                            new_report.PROJECT_GUID = (Guid)System_Environment.GetUser().userProject.Value;
                            new_report.REPORT = reportString;
                            new_report.REPORT_NAME = report_name;
                            new_report.REPORT_TYPE = currentREPORT.GetType().ToString().Split('.')[1];
                            new_report.CREATED = DateTime.Now;
                            new_report.CREATEDBY = System_Environment.GetUser().GUID;
                            report_dataset.REPORT.AddREPORTRow(new_report);
                            daREPORT.Save(new_report);
                            refresh_report?.Invoke();
                        }
                    }
                }
            }
        }
    }

    public class SaveCommandHandler : ICommandHandler
    {
        private XRDesignPanel panel;

        public delegate void SaveCommandDelegate();

        public SaveCommandDelegate SaveDelegate;

        public SaveCommandHandler(XRDesignPanel panel, SaveCommandDelegate ReportSaveDelegate)
        {
            this.panel = panel;
            SaveDelegate = ReportSaveDelegate;
        }

        public void HandleCommand(ReportCommand command,
            object[] args)
        {
            // Save the report.
            Save();
        }

        public bool CanHandleCommand(ReportCommand command,
            ref bool useNextHandler)
        {
            useNextHandler = !(command == ReportCommand.SaveFile);
            return !useNextHandler;
        }

        private void Save()
        {
            // For instance:
            SaveDelegate?.Invoke();

            // Prevent the "Report has been changed" dialog from being shown.
            panel.ReportState = ReportState.Saved;
        }
    }
}

