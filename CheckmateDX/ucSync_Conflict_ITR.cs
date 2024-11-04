using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;

namespace CheckmateDX
{
    public partial class ucSync_Conflict_ITR : EditFormUserControl
    {
        public ucSync_Conflict_ITR()
        {
            InitializeComponent();
            this.SetBoundFieldName(richEditControl1, "ITR_DOC");
            this.SetBoundPropertyName(richEditControl1, "EditValue");
            //MemoryStream ms = new MemoryStream((byte[])richEditControl1.Tag);

            //richEditControl1.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
        }

        private void richEditControl1_BindingContextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
