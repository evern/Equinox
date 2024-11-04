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
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;

namespace CheckmateDX
{
    public partial class frmITR_Select_Toggle : DevExpress.XtraEditors.XtraForm
    {
        List<Template_Toggle> _allTemplateToggle = new List<Template_Toggle>();
        AdapterTEMPLATE_TOGGLE _daTemplateToggle = new AdapterTEMPLATE_TOGGLE(Variables.ConnStr);
        List<string> _richEditPermissions = new List<string>();
        string _discipline = string.Empty;

        public frmITR_Select_Toggle(List<string> RichEditPermissions, string discipline)
        {
            InitializeComponent();
            _richEditPermissions = RichEditPermissions;
            _discipline = discipline;
            gridControl1.DataSource = _allTemplateToggle;
            Refresh_Template_Toggle();
        }

        private void Refresh_Template_Toggle()
        {
            _allTemplateToggle.Clear();
            dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLEDataTable dtTEMPLATE_TOGGLE = _daTemplateToggle.GetAll();
            if(dtTEMPLATE_TOGGLE != null)
            {
                foreach(dsTEMPLATE_TOGGLE.TEMPLATE_TOGGLERow drTEMPLATE_TOGGLE in dtTEMPLATE_TOGGLE.Rows)
                {
                    if (_richEditPermissions.Any(obj => obj == drTEMPLATE_TOGGLE.NAME && drTEMPLATE_TOGGLE.DISCIPLINE == _discipline))
                    {
                        _allTemplateToggle.Add(new Template_Toggle(drTEMPLATE_TOGGLE.GUID)
                        {
                            toggleName = drTEMPLATE_TOGGLE.NAME,
                            toggleDescription = drTEMPLATE_TOGGLE.DESCRIPTION,
                            toggleDiscipline = _discipline,
                            CreatedDate = drTEMPLATE_TOGGLE.CREATED,
                            CreatedBy = drTEMPLATE_TOGGLE.CREATEDBY
                        });
                    }
                }
            }

            gridView1.RefreshData();
        }

        public List<Template_Toggle> Get_Template_Toggles()
        {
            return _allTemplateToggle;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}