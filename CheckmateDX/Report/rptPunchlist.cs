using System;
using DevExpress.XtraReports.UI;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;
using System.Collections.Generic;
using ProjectDatabase.Dataset;

namespace CheckmateDX
{
    public partial class rptPunchlist : DevExpress.XtraReports.UI.XtraReport, IReport
    {
        List<ViewModel_PunchlistReport> reportData;
        List<ViewModel_ProjectPunchlistStatusByDate> reportChronoData;
        Guid project_guid;
        public rptPunchlist(bool designMode)
        {
            InitializeComponent();

            //just get the schema by default
            project_guid = Guid.Empty;
            if (!designMode)
                project_guid = selectScheduleParameters();
        }

        public void Initialize_Data()
        {
            using (AdapterPUNCHLIST_MAIN daPUNCHLIST_MAIN = new AdapterPUNCHLIST_MAIN())
            {
                reportData = daPUNCHLIST_MAIN.GeneratePunchlistReport(project_guid);
                reportChronoData = daPUNCHLIST_MAIN.GeneratePunchlistChronologyReport(project_guid);
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

        Punchlist_Score DisciplineSummary = new Punchlist_Score();
        Punchlist_Score WBSSummary = new Punchlist_Score();
        Punchlist_Score GrandTotalSummary = new Punchlist_Score();
        private void xrLabelGroupPercent_SummaryGetResult(object sender, SummaryGetResultEventArgs e)
        {
            Punchlist_Score currentAssignment = SummaryVarSelection((XRLabel)sender);
            e.Result = currentAssignment.Percentage;
            e.Handled = true;
        }

        private void xrLabelGroupPercent_SummaryReset(object sender, EventArgs e)
        {
            Punchlist_Score currentAssignment = SummaryVarSelection((XRLabel)sender);
            currentAssignment.TotalSavedCount = 0;
            currentAssignment.TotalCategorisedCount = 0;
            currentAssignment.TotalInspectedCount = 0;
            currentAssignment.TotalApprovedCount = 0;
            currentAssignment.TotalCompletedCount = 0;
            currentAssignment.TotalClosedCount = 0;
        }

        private void xrLabelGroupPercent_SummaryRowChanged(object sender, EventArgs e)
        {
            ViewModel_PunchlistReport currentRow = (ViewModel_PunchlistReport)GetCurrentRow();
            Punchlist_Score currentAssignment = SummaryVarSelection((XRLabel)sender);

            currentAssignment.TotalSavedCount += currentRow.Saved_Count;
            currentAssignment.TotalCategorisedCount += currentRow.Categorised_Count;
            currentAssignment.TotalInspectedCount += currentRow.Inspected_Count;
            currentAssignment.TotalApprovedCount += currentRow.Approved_Count;
            currentAssignment.TotalCompletedCount += currentRow.Completed_Count;
            currentAssignment.TotalClosedCount += currentRow.Closed_Count;
        }

        private Punchlist_Score SummaryVarSelection(XRLabel label)
        {
            if (label.Name.Contains("WBSSummary"))
                return WBSSummary;
            else if (label.Name.Contains("DisciplineSummary"))
                return DisciplineSummary;
            else
                return GrandTotalSummary;
        }

        Punchlist_Score ElectricalSummary = new Punchlist_Score();
        Punchlist_Score InstrumentSummary = new Punchlist_Score();
        Punchlist_Score MechanicalSummary = new Punchlist_Score();
        Punchlist_Score PipingSummary = new Punchlist_Score();
        Punchlist_Score CivilSummary = new Punchlist_Score();
        Punchlist_Score StructuralSummary = new Punchlist_Score();
        Punchlist_Score HeaderGrandTotalSummary = new Punchlist_Score();

        private Punchlist_Score SummaryDisciplineVarSelection(XRLabel label)
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

        private void xrLabelDisciplineTotal_SummaryGetResult(object sender, SummaryGetResultEventArgs e)
        {
            XRLabel currentLabel = (XRLabel)sender;
            Punchlist_Score currentDisciplineScore = SummaryDisciplineVarSelection(currentLabel);
            if (currentLabel.Name.Contains("Saved"))
                e.Result = currentDisciplineScore.TotalSavedCount;
            else if (currentLabel.Name.Contains("Categorised"))
                e.Result = currentDisciplineScore.TotalCategorisedCount;
            else if (currentLabel.Name.Contains("Inspected"))
                e.Result = currentDisciplineScore.TotalInspectedCount;
            else if (currentLabel.Name.Contains("Approved"))
                e.Result = currentDisciplineScore.TotalApprovedCount;
            else if (currentLabel.Name.Contains("Completed"))
                e.Result = currentDisciplineScore.TotalCompletedCount;
            else if (currentLabel.Name.Contains("Closed"))
                e.Result = currentDisciplineScore.TotalClosedCount;
            else if (currentLabel.Name.Contains("AllStatus"))
                e.Result = currentDisciplineScore.TotalCount;
            else
                e.Result = currentDisciplineScore.Percentage;

            e.Handled = true;
        }

        private void xrLabelDisciplineTotal_SummaryRowChanged(object sender, EventArgs e)
        {
            Punchlist_Score currentDisciplineScore = SummaryDisciplineVarSelection((XRLabel)sender);
            ViewModel_PunchlistReport currentRow = (ViewModel_PunchlistReport)GetCurrentRow();

            if (((XRLabel)sender).Name.Contains("GrandTotal") || ((XRLabel)sender).Name.Contains(currentRow.Discipline))
            {
                currentDisciplineScore.TotalSavedCount += currentRow.Saved_Count;
                currentDisciplineScore.TotalCategorisedCount += currentRow.Categorised_Count;
                currentDisciplineScore.TotalInspectedCount += currentRow.Inspected_Count;
                currentDisciplineScore.TotalApprovedCount += currentRow.Approved_Count;
                currentDisciplineScore.TotalCompletedCount += currentRow.Completed_Count;
                currentDisciplineScore.TotalClosedCount += currentRow.Closed_Count;
            }
        }
    }
}
