using DevExpress.XtraReports.UI;
using ProjectLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace CheckmateDX.Report
{
    public partial class rptErrorMessage : DevExpress.XtraReports.UI.XtraReport
    {
        public rptErrorMessage()
        {
            InitializeComponent();
        }

        public rptErrorMessage(string title, List<ViewModel_ErrorMessage> errorMessages)
        {
            InitializeComponent();
            this.xrTitle.Text = title;
            this.bindingSource1.DataSource = errorMessages;
        }

        public void ShowReport()
        {
            this.ShowPreview();
        }
    }
}
