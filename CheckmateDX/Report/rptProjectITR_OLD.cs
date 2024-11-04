using System;
using DevExpress.XtraReports.UI;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;
using System.Collections.Generic;
using ProjectDatabase.Dataset;

namespace CheckmateDX
{
    public partial class rptProjectITR_OLD : DevExpress.XtraReports.UI.XtraReport, IReport
    {
        List<ViewModel_ProjectReport> reportData;
        List<ViewModel_ProjectITRStatusByDate> reportChronoData;
        Guid project_guid;
        public rptProjectITR_OLD(bool designMode)
        {
            InitializeComponent();

            //just get the schema by default
            project_guid = Guid.Empty;
            if(!designMode)
                project_guid = selectScheduleParameters();
        }

        public void Initialize_Data()
        {
            using (AdapterITR_MAIN daITR_MAIN = new AdapterITR_MAIN())
            {
                reportData = daITR_MAIN.GenerateProjectITRReport(project_guid);
                reportChronoData = daITR_MAIN.GenerateProjectChronologyReport(project_guid);
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
            this.objectDataSource2.DataSource = reportChronoData;
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

        ITR_Score DisciplineSummary = new ITR_Score();
        ITR_Score WBSSummary = new ITR_Score();
        ITR_Score GrandTotalSummary = new ITR_Score();
        private void xrLabelGroupPercent_SummaryGetResult(object sender, SummaryGetResultEventArgs e)
        {
            ITR_Score currentAssignment = SummaryVarSelection((XRLabel)sender);
            e.Result = currentAssignment.Percentage;
            e.Handled = true;
        }

        private void xrLabelGroupPercent_SummaryReset(object sender, EventArgs e)
        {
            ITR_Score currentAssignment = SummaryVarSelection((XRLabel)sender);
            currentAssignment.TotalAssignedCount = 0;
            currentAssignment.TotalInspectedCount = 0;
            currentAssignment.TotalApprovedCount = 0;
            currentAssignment.TotalCompletedCount = 0;
            currentAssignment.TotalClosedCount = 0;
        }

        private void xrLabelGroupPercent_SummaryRowChanged(object sender, EventArgs e)
        {
            ViewModel_ProjectReport currentRow = (ViewModel_ProjectReport)GetCurrentRow();
            ITR_Score currentAssignment = SummaryVarSelection((XRLabel)sender);

            currentAssignment.TotalAssignedCount += currentRow.Saved_Count;
            currentAssignment.TotalInspectedCount += currentRow.Inspected_Count;
            currentAssignment.TotalApprovedCount += currentRow.Approved_Count;
            currentAssignment.TotalCompletedCount += currentRow.Completed_Count;
            currentAssignment.TotalClosedCount += currentRow.Closed_Count;
        }

        private ITR_Score SummaryVarSelection(XRLabel label)
        {
            if (label.Name.Contains("WBSSummary"))
                return WBSSummary;
            else if (label.Name.Contains("DisciplineSummary"))
                return DisciplineSummary;
            else
                return GrandTotalSummary;
        }

        private ITR_Score SummaryDisciplineVarSelection(XRLabel label)
        {
            if (label.Name.Contains("Electrical"))
                return ElectricalSummary;
            else if (label.Name.Contains("Instrument"))
                return InstrumentSummary;
            else if (label.Name.Contains("Mechanical"))
                return MechanicalSummary;
            else if (label.Name.Contains("Piping"))
                return PipingSummary;
            else if (label.Name.Contains("Civil"))
                return CivilSummary;
            else if (label.Name.Contains("Structural"))
                return StructuralSummary;
            else
                return HeaderGrandTotalSummary;
        }

        ITR_Score ElectricalSummary = new ITR_Score();
        ITR_Score InstrumentSummary = new ITR_Score();
        ITR_Score MechanicalSummary = new ITR_Score();
        ITR_Score PipingSummary = new ITR_Score();
        ITR_Score CivilSummary = new ITR_Score();
        ITR_Score StructuralSummary = new ITR_Score();
        ITR_Score HeaderGrandTotalSummary = new ITR_Score();
        private void xrLabelDisciplineTotal_SummaryGetResult(object sender, SummaryGetResultEventArgs e)
        {
            XRLabel currentLabel = (XRLabel)sender;
            ITR_Score currentDisciplineScore = SummaryDisciplineVarSelection(currentLabel);
            if (currentLabel.Name.Contains("Assigned"))
                e.Result = currentDisciplineScore.TotalAssignedCount;
            else if (currentLabel.Name.Contains("Inspected"))
                e.Result = currentDisciplineScore.TotalInspectedCount;
            else if (currentLabel.Name.Contains("Approved"))
                e.Result = currentDisciplineScore.TotalApprovedCount;
            else if (currentLabel.Name.Contains("Completed"))
                e.Result = currentDisciplineScore.TotalCompletedCount;
            else if (currentLabel.Name.Contains("Closed"))
                e.Result = currentDisciplineScore.TotalClosedCount;
            else
                e.Result = currentDisciplineScore.Percentage;

            e.Handled = true;
        }

        private void xrLabelDisciplineTotal_SummaryRowChanged(object sender, EventArgs e)
        {
            ITR_Score currentDisciplineScore = SummaryDisciplineVarSelection((XRLabel)sender);
            ViewModel_ProjectReport currentRow = (ViewModel_ProjectReport)GetCurrentRow();

            if (((XRLabel)sender).Name.Contains("GrandTotal") || ((XRLabel)sender).Name.Contains(currentRow.Discipline))
            {
                currentDisciplineScore.TotalAssignedCount += currentRow.Saved_Count;
                currentDisciplineScore.TotalInspectedCount += currentRow.Inspected_Count;
                currentDisciplineScore.TotalApprovedCount += currentRow.Approved_Count;
                currentDisciplineScore.TotalCompletedCount += currentRow.Completed_Count;
                currentDisciplineScore.TotalClosedCount += currentRow.Closed_Count;
            }
        }
    }
}
