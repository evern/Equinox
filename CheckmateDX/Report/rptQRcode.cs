using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace CheckmateDX.Report
{
    public partial class rptQRcode : DevExpress.XtraReports.UI.XtraReport
    {
        public rptQRcode()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            xrBarCode1.Text = text;
        }
    }
}
