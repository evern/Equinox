using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectCommon;
using ProjectLibrary;

namespace CheckmateDX
{
    public partial class frmSchedule_Add : CheckmateDX.frmParent
    {
        private Schedule _editSchedule;

        private Schedule _Schedule;

        /// <summary>
        /// Constructor for adding
        /// </summary>
        public frmSchedule_Add()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor for editing
        /// </summary>
        /// <param name="editSchedule">Schedule to edit</param>
        public frmSchedule_Add(Schedule editSchedule)
        {
            InitializeComponent();
            this.Text = "Edit Schedule";
            btnOk.Text = "Accept";
            PopulateFormElements(editSchedule);
            _editSchedule = editSchedule;
        }


        /// <summary>
        /// Populate form element for editing
        /// </summary>
        /// <param name="selectedParent">Parent schedule guid</param>
        private void PopulateFormElements(Schedule editSchedule)
        {
            txtName.Text = editSchedule.scheduleName;
            txtDescription.Text = editSchedule.scheduleDescription;
        }

        public Schedule GetSchedule()
        {
            return _Schedule;
        }

        #region Validation
        private bool ValidateFormElements()
        {
            if (txtName.Text.Trim() == string.Empty)
            {
                Common.Warn("Name cannot be empty");
                txtName.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for duplicate Schedule
        /// </summary>
        private bool ValidateSchedule()
        {
            using (AdapterSCHEDULE daSchedule = new AdapterSCHEDULE())
            {
                if (daSchedule.GetBy(txtName.Text.Trim()) != null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check for duplicate schedule on record other than itself
        /// </summary>
        /// <param name="editScheduleGuid">Schedule guid to exclude checking</param>
        private bool ValidateEditSchedule(Guid editScheduleGuid)
        {
            using (AdapterSCHEDULE daSchedule = new AdapterSCHEDULE())
            {
                dsSCHEDULE.SCHEDULERow drSchedule = daSchedule.GetBy(txtName.Text.Trim());

                if (drSchedule != null)
                {
                    if (drSchedule.GUID != editScheduleGuid)
                        return false;
                }
            }

            return true;
        }
        #endregion

        #region Events
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ValidateFormElements())
            {
                if (_editSchedule != null || ValidateSchedule())
                {
                    if (_editSchedule != null)
                    {
                        if (ValidateEditSchedule(_editSchedule.GUID))
                        {
                            _editSchedule.scheduleName = txtName.Text.Trim();
                            _editSchedule.scheduleDescription = txtDescription.Text.Trim();
                            _Schedule = _editSchedule;
                        }
                        else
                        {
                            Common.Prompt("The name specified has already been used by other schedule\n\nPlease type in a different name");
                            return;
                        }
                    }
                    else
                    {
                        Schedule newSchedule = new Schedule(Guid.NewGuid())
                        {
                            scheduleName = txtName.Text.Trim(),
                            scheduleDescription = txtDescription.Text.Trim(),
                        };

                        _Schedule = newSchedule;
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                    Common.Warn("Name already exists"); ;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion
    }
}
