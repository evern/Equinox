using DevExpress.Spreadsheet;
using DevExpress.XtraSplashScreen;
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
using System.Windows.Threading;
using static ProjectLibrary.Variables;

namespace CheckmateDX
{
    public partial class frmReportSheet : Form
    {
        Guid _projectGuid = Guid.Empty;
        IWorkbook _workbook;
        Worksheet _masterSheet;
        public frmReportSheet()
        {
            InitializeComponent();
        }

        public void populateCertificates(CertificateReportType certificateReportType)
        {
            ExternalDataSourceOptions options = new ExternalDataSourceOptions();
            options.ImportHeaders = true;
            using (AdapterCERTIFICATE_MAIN daCERTIFICATE_MAIN = new AdapterCERTIFICATE_MAIN())
            {
                if(certificateReportType == CertificateReportType.CVCMasterReport || certificateReportType == CertificateReportType.PunchlistWalkdown)
                {
                    string certificateTemplateName = string.Empty;
                    if (certificateReportType == CertificateReportType.CVCMasterReport)
                    {
                        this.Text = "CVC Master Report";
                        certificateTemplateName = Variables.constructionVerificationCertificateTemplateName;
                    }
                    else if (certificateReportType == CertificateReportType.PunchlistWalkdown)
                    {
                        this.Text = "Punchlist Walkdown Master Report";
                        certificateTemplateName = Variables.punchlistWalkdownTemplateName;
                    }

                    dsCERTIFICATE_MAIN.CertificateReportDataTable dtCertificateReport = daCERTIFICATE_MAIN.GetCertificateReport(_projectGuid, certificateTemplateName);
                    _masterSheet.Tables.Add(dtCertificateReport, 0, 0, options);
                }
                else if(certificateReportType == CertificateReportType.NOEReport)
                {
                    this.Text = "NOE Master Report";
                    dsCERTIFICATE_MAIN.NOE_ReportDataTable dtNOEReport = daCERTIFICATE_MAIN.GetNOEReport(_projectGuid);
                    _masterSheet.Tables.Add(dtNOEReport, 0, 0, options);
                }
                else if (certificateReportType == CertificateReportType.DisciplineMasterReport)
                {
                    this.Text = "Discipline Master Report";
                    dsCERTIFICATE_MAIN.DisciplineReportDataTable dtDisciplineReport = daCERTIFICATE_MAIN.GetDisciplineReport(_projectGuid);
                    _masterSheet.Tables.Add(dtDisciplineReport, 0, 0, options);
                }
            }
        }

        public void populateMatrixReport()
        {
            ExternalDataSourceOptions options = new ExternalDataSourceOptions();
            options.ImportHeaders = true;
            using (AdapterMATRIX_TYPE daMATRIX_TYPE = new AdapterMATRIX_TYPE())
            {
                this.Text = "Matrix Assignment Report";
                dsMATRIX_TYPE.MatrixReportDataTable dtMatrixReport = daMATRIX_TYPE.GetReport(_projectGuid);
                _masterSheet.Tables.Add(dtMatrixReport, 0, 0, options);
            }
        }

        public frmReportSheet(Guid projectGuid)
        {
            InitializeComponent();
            _projectGuid = projectGuid;
            _workbook = spreadsheetControl1.Document;
            _masterSheet = _workbook.Worksheets[0];
        }
    }
}
