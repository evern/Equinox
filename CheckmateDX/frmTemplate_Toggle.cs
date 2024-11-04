using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ProjectCommon;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectLibrary;

namespace CheckmateDX
{
    public partial class frmTemplate_Toggle : CheckmateDX.frmTool_Main
    {
        AdapterTEMPLATE_TOGGLE _daTEMPLATE_TOGGLE = new AdapterTEMPLATE_TOGGLE(Variables.ConnStr);
        dsTEMPLATE_TOGGLE dsTEMPLATETOGGLE = new dsTEMPLATE_TOGGLE();
        List<Template_Toggle> _allTemplateToggle = new List<Template_Toggle>();
        string _discipline = string.Empty;
        Guid _templateGuid = Guid.Empty;

        public frmTemplate_Toggle()
        {
            InitializeComponent();
            _templateGuid = Guid.Empty;
            templateToggleBindingSource.DataSource = _allTemplateToggle;
            timer1.Enabled = true;
        }

        #region Form Population
        private void Refresh_TemplateToggle()
        {
            _allTemplateToggle.Clear();
            dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE = _daTEMPLATE_TOGGLE.GetBy_Discipline(_discipline);

            if(dtTEMPLATE_TOGGLE != null)
            {
                foreach (dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow drTEMPLATE_TOGGLE in dtTEMPLATE_TOGGLE.Rows)
                {
                    _allTemplateToggle.Add(new Template_Toggle(drTEMPLATE_TOGGLE.GUID)
                    {
                        toggleName = drTEMPLATE_TOGGLE.NAME,
                        toggleDescription = drTEMPLATE_TOGGLE.DESCRIPTION,
                        CreatedDate = drTEMPLATE_TOGGLE.CREATED,
                        CreatedBy = drTEMPLATE_TOGGLE.CREATEDBY
                    });
                }
            }

            gridViewToggle.RefreshData();
        }
        #endregion

        #region Events
        protected override void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmTemplate_Toggle_Add fToggle_Add = new frmTemplate_Toggle_Add(_discipline);
            if(fToggle_Add.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Template_Toggle newTemplateToggle = fToggle_Add.GetTemplate_Toggle();
                dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow drNewTEMPLATE_TOGGLE = dsTEMPLATETOGGLE.TEMPLATE_TOGGLE.NewTEMPLATE_TOGGLERow();
                drNewTEMPLATE_TOGGLE.GUID = Guid.NewGuid();
                drNewTEMPLATE_TOGGLE.DISCIPLINE = _discipline;
                drNewTEMPLATE_TOGGLE.NAME = newTemplateToggle.toggleName;
                drNewTEMPLATE_TOGGLE.DESCRIPTION = newTemplateToggle.toggleDescription;
                drNewTEMPLATE_TOGGLE.CREATED = DateTime.Now;
                drNewTEMPLATE_TOGGLE.CREATEDBY = System_Environment.GetUser().GUID;
                dsTEMPLATETOGGLE.TEMPLATE_TOGGLE.AddTEMPLATE_TOGGLERow(drNewTEMPLATE_TOGGLE);
                _daTEMPLATE_TOGGLE.Save(drNewTEMPLATE_TOGGLE);
                Refresh_TemplateToggle();
            }
        }

        protected override void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(gridViewToggle.SelectedRowsCount == 0)
            {
                Common.Warn("Please select a row to edit");
                return;
            }

            Template_Toggle editTemplateToggle = (Template_Toggle)gridViewToggle.GetFocusedRow();
            frmTemplate_Toggle_Add fToggle_Edit = new frmTemplate_Toggle_Add(editTemplateToggle);
            if(fToggle_Edit.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Template_Toggle editedTemplateToggle = fToggle_Edit.GetTemplate_Toggle();
                dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow drTEMPLATE_TOGGLE = _daTEMPLATE_TOGGLE.GetBy(editedTemplateToggle.GUID);
                if(drTEMPLATE_TOGGLE != null)
                {
                    drTEMPLATE_TOGGLE.NAME = editedTemplateToggle.toggleName;
                    drTEMPLATE_TOGGLE.DESCRIPTION = editedTemplateToggle.toggleDescription;
                    drTEMPLATE_TOGGLE.UPDATED = DateTime.Now;
                    drTEMPLATE_TOGGLE.UPDATEDBY = System_Environment.GetUser().GUID;
                    _daTEMPLATE_TOGGLE.Save(drTEMPLATE_TOGGLE);
                    Refresh_TemplateToggle();
                }
            }
        }

        protected override void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridViewToggle.SelectedRowsCount == 0)
            {
                Common.Warn("Please select a row to delete");
                return;
            }

            Template_Toggle deleteTemplateToggle = (Template_Toggle)gridViewToggle.GetFocusedRow();
            _daTEMPLATE_TOGGLE.RemoveBy(deleteTemplateToggle.GUID);
            Refresh_TemplateToggle();
        }
        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            SetupToggleEnvironment();
        }

        private void SetupToggleEnvironment()
        {
            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                frmITR_Select_Discipline frmSelectDiscipline = new frmITR_Select_Discipline();

                if (frmSelectDiscipline.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _discipline = frmSelectDiscipline.GetDiscipline();
                }
                else
                {
                    this.Close();
                    return;
                }
            }
            else
                _discipline = System_Environment.GetUser().userDiscipline;

            Refresh_TemplateToggle();
        }
    }
}