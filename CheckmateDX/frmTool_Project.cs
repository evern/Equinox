using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;

namespace CheckmateDX
{
    public partial class frmTool_Project : CheckmateDX.frmTool_Main
    {
        AdapterPROJECT _daProject = new AdapterPROJECT();
        dsPROJECT _dsProject = new dsPROJECT();

        List<Project> _allProject = new List<Project>();
        public frmTool_Project()
        {
            InitializeComponent();
            projectBindingSource.DataSource = _allProject;
            RefreshProjects();
        }

        #region Public
        /// <summary>
        /// Do not allow user to add/edit/delete if this form is shown as a dialog
        /// </summary>
        public void ShowAsDialog()
        {
            barMenu.Visible = false;
            panelDialogButtons.Visible = true;
            colCreatedDate.Visible = false;
            this.Width = 800;
            this.Height = 800;
            this.Text = "Select Project";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        /// <summary>
        /// Retrieves the user selected project
        /// </summary>
        public Project GetSelectedProject()
        {
            return (Project)gridView1.GetFocusedRow();
        }
        #endregion

        #region Form Population
        /// <summary>
        /// Refresh all project from database
        /// </summary>
        private void RefreshProjects()
        {
            _allProject.Clear();
            dsPROJECT.PROJECTDataTable dtProject = _daProject.Get();

            if(dtProject != null)
            {
                foreach (dsPROJECT.PROJECTRow drProject in dtProject.Rows)
                {
                    _allProject.Add(new Project(drProject.GUID)
                    {
                        projectNumber = drProject.NUMBER,
                        projectName = drProject.NAME,
                        projectClient = drProject.CLIENT,
                        CreatedDate = drProject.CREATED,
                        CreatedBy = drProject.CREATEDBY
                    });
                }
            }

            gridControl.RefreshDataSource();
        }
        #endregion

        #region Button Overrides
        protected override void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmTool_Project_Add frmProjectAdd = new frmTool_Project_Add();
            if (frmProjectAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Project newProject = frmProjectAdd.GetProject();
                dsPROJECT.PROJECTRow drProject = _dsProject.PROJECT.NewPROJECTRow();
                drProject.GUID = Guid.NewGuid();
                drProject.NUMBER = newProject.projectNumber;
                drProject.CREATED = DateTime.Now;
                drProject.CREATEDBY = System_Environment.GetUser().GUID;
                AssignProjectDetails(drProject, newProject);
                _dsProject.PROJECT.AddPROJECTRow(drProject);
                _daProject.Save(drProject);
                RefreshProjects();
                //Common.Prompt("Project successfully added");
            }

            base.btnAdd_ItemClick(sender, e);
        }

        protected override void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(gridView1.SelectedRowsCount == 0)
            {
                Common.Warn("Please select Project to edit");
                return;
            }

            frmTool_Project_Add frmProjectAdd = new frmTool_Project_Add((Project)gridView1.GetFocusedRow());
            if (frmProjectAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Project editProject = frmProjectAdd.GetProject();

                dsPROJECT.PROJECTRow drProjectMaster = _daProject.GetBy(editProject.GUID);
                AssignProjectDetails(drProjectMaster, editProject);
                drProjectMaster.UPDATED = DateTime.Now;
                drProjectMaster.UPDATEDBY = System_Environment.GetUser().GUID;
                _daProject.Save(drProjectMaster);
                //Common.Prompt("Project successfully updated");
                RefreshProjects();
            }

            base.btnEdit_ItemClick(sender, e);
        }

        protected override void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridView1.SelectedRowsCount == 0)
            {
                Common.Warn("Please select Project(s) to delete");
                return;
            }

            if (!Common.Confirmation("Are you sure you want to delete selected Project(s)?", "Confirmation"))
                return;

            dsPROJECT.PROJECTDataTable dtProject = new dsPROJECT.PROJECTDataTable(); //what project to delete
            int[] selectedRowIndexes = gridView1.GetSelectedRows();

            foreach (int selectedRowIndex in selectedRowIndexes)
            {
                Project selectedProject = (Project)gridView1.GetRow(selectedRowIndex);
                //necessary values depends on what is displayed in the report
                dsPROJECT.PROJECTRow drProject = dtProject.NewPROJECTRow();
                drProject.GUID = selectedProject.GUID;
                drProject.NUMBER = selectedProject.projectNumber;
                drProject.NAME = selectedProject.projectName;
                drProject.CLIENT = selectedProject.projectClient;
                //need to put these values in because the schema doesn't allow null
                drProject.CREATED = selectedProject.CreatedDate;
                drProject.CREATEDBY = selectedProject.CreatedBy;
                dtProject.AddPROJECTRow(drProject);
            }

            rptDeletion f = new rptDeletion(dtProject);
            f.ShowReport();
            RefreshProjects();
            base.btnDelete_ItemClick(sender, e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _daProject.Dispose();
            base.OnClosed(e);
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Assigns common project details to data row
        /// </summary>
        /// <param name="drUserMaster">datarow to be assigned</param>
        /// <param name="user">project details</param>
        private void AssignProjectDetails(dsPROJECT.PROJECTRow drProject, Project project)
        {
            drProject.NAME = project.projectName;
            drProject.CLIENT = project.projectClient;
        }
        #endregion
    }
}
