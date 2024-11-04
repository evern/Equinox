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
    public partial class frmTool_Project_Add : CheckmateDX.frmParent
    {
        private Project _editProject;

        private Project _project;
        public frmTool_Project_Add()
        {
            InitializeComponent();
        }

        public frmTool_Project_Add(Project editProject)
        {
            InitializeComponent();
            this.Text = "Edit Project";
            pnlNumber.Visible = false;
            btnOk.Text = "Accept";
            PopulateFormElements(editProject);
            _editProject = editProject;
        }

        /// <summary>
        /// Populate form element for editing
        /// </summary>
        private void PopulateFormElements(Project editProject)
        {
            txtNumber.Text = editProject.projectNumber;
            txtName.Text = editProject.projectName;
            txtClient.Text = editProject.projectClient;
        }

        public Project GetProject()
        {
            return _project;
        }

        #region Validation
        /// <summary>
        /// Checks whether form is properly filled
        /// </summary>
        private bool ValidateFormElements()
        {
            if (txtNumber.Text.Trim() == string.Empty)
            {
                Common.Warn("Number cannot be empty");
                txtNumber.Focus();
                return false;
            }

            if (txtName.Text.Trim() == string.Empty)
            {
                Common.Warn("Name cannot be empty");
                txtName.Focus();
                return false;
            }

            if (txtClient.Text.Trim() == string.Empty)
            {
                Common.Warn("Client cannot be empty");
                txtClient.Focus();
                return false;
            }

            return true;
        } 

        /// <summary>
        /// Checks for duplicate project
        /// </summary>
        private bool ValidateProject()
        {
            using (AdapterPROJECT daProject = new AdapterPROJECT())
            {
                if (daProject.GetBy(txtNumber.Text.Trim()) != null)
                {
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
                if (_editProject != null || ValidateProject())
                {
                    if (_editProject != null)
                    {
                        _editProject.projectNumber = txtNumber.Text.Trim();
                        _editProject.projectName = txtName.Text.Trim();
                        _editProject.projectClient = txtClient.Text.Trim();
                        _project = _editProject;
                    }
                    else
                    {
                        Project newProject = new Project(Guid.NewGuid())
                        {
                            projectNumber = txtNumber.Text.Trim(),
                            projectName = txtName.Text.Trim(),
                            projectClient = txtClient.Text.Trim(),
                        };

                        _project = newProject;
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                    Common.Warn("Project already exists");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion
    }
}
