using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CheckmateDX
{
    public partial class frmInteractable_DatetimePicker : CheckmateDX.frmParent
    {
        public frmInteractable_DatetimePicker(DateTimePickerFormat dateTimePickerFormat)
        {
            InitializeComponent();
            datePicker1.Value = DateTime.Now;
            timePicker1.Value = DateTime.Now;

            if(dateTimePickerFormat == DateTimePickerFormat.Short)
            {
                panelControlTime.Visible = false;
            }
            else if(dateTimePickerFormat == DateTimePickerFormat.Time)
            {
                panelControlDate.Visible = false;
            }
        }

        public DateTime SelectedDateTime()
        {
            DateTime date = datePicker1.Value.Date;
            TimeSpan time = timePicker1.Value.TimeOfDay;
            DateTime formatDateTime = date + time;

            return formatDateTime;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
