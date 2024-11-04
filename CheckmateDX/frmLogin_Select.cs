using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectCommon;
using ProjectLibrary;

namespace CheckmateDX
{
    public partial class frmLogin_Select : CheckmateDX.frmParent
    {
        ValuePair _selectedProject;
        string _selectedDiscipline = Variables.noDiscipline;
        public frmLogin_Select(Guid userGuid)
        {
            InitializeComponent();
            //Common.PopulateCmbProject(cmbProject, false);
            Common.PopulateCmbAuthProject(cmbProject, false);
            Common.PopulateCmbAuthDiscipline(cmbDiscipline);
        }

        #region Public methods
        /// <summary>
        /// Get user selected project
        /// </summary>
        /// <returns>GUID of project or GUID.Empty</returns>
        public Guid getDefaultProject()
        {
            if (_selectedProject != null)
                return (Guid)_selectedProject.Value;
            else
                return Guid.Empty;
        }

        /// <summary>
        /// Get user selected discipline
        /// </summary>
        /// <returns>Discipline Enum</returns>
        public string getDefaultDiscipline()
        {
            return Common.ConvertDisplayDisciplineForDB(_selectedDiscipline);
        } 
        #endregion


        #region Events
        private void btnOk_Click(object sender, EventArgs e)
        {
            _selectedProject = (ValuePair)cmbProject.SelectedItem;
            _selectedDiscipline = cmbDiscipline.Text;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        } 
        #endregion
    }
}
