using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ProjectLibrary;
using DevExpress.XtraRichEdit.API.Native;

namespace CheckmateDX
{
    public partial class frmITR_Select_Acceptance : DevExpress.XtraEditors.XtraForm
    {
        public delegate void SetAcceptanceHandler(DocumentRange DocRange, string Selection);
        public event SetAcceptanceHandler Set_Acceptance_Event = delegate { };

        private string _selectedOption = string.Empty;
        private DocumentRange _docRange;
        public frmITR_Select_Acceptance(Point systemWindowsFormsCursorPosition, DocumentRange DocRange)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = systemWindowsFormsCursorPosition;
            _docRange = DocRange;
        }

        public string Get_Selection()
        {
            return _selectedOption;
        }

        private void btnAcceptable_Click(object sender, EventArgs e)
        {
            _selectedOption = Toggle_Acceptance.Acceptable.ToString();
            Set_Acceptance_Event(_docRange, _selectedOption);
            this.Close();
        }

        private void btnNotApplicable_Click(object sender, EventArgs e)
        {
            _selectedOption = Toggle_Acceptance.Not_Applicable.ToString();
            Set_Acceptance_Event(_docRange, _selectedOption);
            this.Close();
        }

        private void btnPunchlist_Click(object sender, EventArgs e)
        {
            _selectedOption = Toggle_Acceptance.Punchlisted.ToString();
            Set_Acceptance_Event(_docRange, _selectedOption);
            this.Close();
        }

        private void frmITR_Select_Acceptance_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}