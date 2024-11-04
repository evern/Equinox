using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Linq;
using ProjectLibrary;

namespace CheckmateDX.Report
{
    public partial class rptSignature : DevExpress.XtraReports.UI.XtraReport
    {
        public rptSignature()
        {
            InitializeComponent();
        }

        public void PopulateSignatures(List<SignatureUser> signature_users, bool isElectrical)
        {
            if (!isElectrical)
                EWIDRow.Visible = false;

            SignatureUser inspector = signature_users.FirstOrDefault(x => x.SignStatus == ITR_Status.Inspected);
            SignatureUser supervisor = signature_users.FirstOrDefault(x => x.SignStatus == ITR_Status.Approved);
            SignatureUser projectmanager = signature_users.FirstOrDefault(x => x.SignStatus == ITR_Status.Completed);
            SignatureUser client = signature_users.FirstOrDefault(x => x.SignStatus == ITR_Status.Closed);
            try
            {
                if (inspector != null)
                {
                    inspectorSignatureBox.Image = inspector.Signature;
                    inspectorName.Text = inspector.Name;
                    inspectorCompany.Text = inspector.Company;
                    inspectorDate.Text = inspector.SignDate.Year == 1 ? string.Empty : inspector.SignDate.ToString("d");
                }
                if (supervisor != null)
                {
                    supervisorSignatureBox.Image = supervisor.Signature;
                    supervisorName.Text = supervisor.Name;
                    supervisorCompany.Text = supervisor.Company;
                    supervisorDate.Text = supervisor.SignDate.Year == 1 ? string.Empty : supervisor.SignDate.ToString("d");
                }
                if (projectmanager != null)
                {
                    managerSignatureBox.Image = projectmanager.Signature;
                    managerName.Text = projectmanager.Name;
                    managerCompany.Text = projectmanager.Company;
                    managerDate.Text = projectmanager.SignDate.Year == 1 ? string.Empty : projectmanager.SignDate.ToString("d");
                }
                if (client != null)
                {
                    clientSignatureBox.Image = client.Signature;
                    clientName.Text = client.Name;
                    clientCompany.Text = client.Company;
                    clientDate.Text = client.SignDate.Year == 1 ? string.Empty : client.SignDate.ToString("d");
                }

                inspectorEWID.Text = inspector == null ? string.Empty : inspector.AdditionalInfo;
                supervisorEWID.Text = supervisor == null ? string.Empty : supervisor.AdditionalInfo;
                managerEWID.Text = projectmanager == null ? string.Empty : projectmanager.AdditionalInfo;
                clientEWID.Text = client == null ? string.Empty : client.AdditionalInfo;
            }
            catch
            {
                //Populate what the system can and ignore any errors
                //tblSignature.ForEachCell(new TableCellProcessorDelegate(customRichEdit1.ClearCell));
            }
        }
    }
}
