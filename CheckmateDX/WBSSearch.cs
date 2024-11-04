using DevExpress.XtraEditors;
using ProjectCommon;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckmateDX
{
    public class WBSSearch
    {
        private List<ProjectWBS> _projectWBSs = new List<ProjectWBS>();
        private string _defaultDiscipline = string.Empty;
        public ProjectLibrary.SearchMode SearchMode = ProjectLibrary.SearchMode.Contains;

        //Checked edit combobox lists
        protected List<ProjectWBS> _fullAreaWBSs = new List<ProjectWBS>();
        protected List<ProjectWBS> _fullSystemWBSs = new List<ProjectWBS>();
        protected List<ProjectWBS> _fullSubsystemWBSs = new List<ProjectWBS>();
        protected List<ProjectWBS> _filterAreaWBSs = new List<ProjectWBS>();
        protected List<ProjectWBS> _filterSystemWBSs = new List<ProjectWBS>();
        protected List<ProjectWBS> _filterSubsystemWBSs = new List<ProjectWBS>();
        protected List<string> _disciplines = new List<string>();
        protected List<string> _categories = new List<string>();
        protected List<string> _stages = new List<string>();
        public List<string> SelectedAreas = new List<string>();
        public List<string> SelectedSystems = new List<string>();
        public List<string> SelectedSubsystems = new List<string>();
        public List<string> SelectedDisciplines = new List<string>();
        public List<string> SelectedCategories = new List<string>();
        public List<string> SelectedStages = new List<string>();

        CheckedComboBoxEdit refCmbArea = null;
        CheckedComboBoxEdit refCmbSystem = null;
        CheckedComboBoxEdit refCmbSubsystem = null;
        CheckedComboBoxEdit refCmbDiscipline = null;
        CheckedComboBoxEdit refCmbCategory = null;
        CheckedComboBoxEdit refCmbStage = null;
        ComboBoxEdit refCmbSearchMode = null;

        public WBSSearch(string defaultDiscipline, List<ProjectWBS> FullProjectWBS, CheckedComboBoxEdit cmbAreaParent, CheckedComboBoxEdit cmbSystemParent, CheckedComboBoxEdit cmbSubsystemParent, CheckedComboBoxEdit cmbDisciplineParent, CheckedComboBoxEdit cmbCategoryParent, CheckedComboBoxEdit cmbStageParent, ComboBoxEdit cmbSearchModeParent)
        {
            _defaultDiscipline = defaultDiscipline;
            _projectWBSs = FullProjectWBS;
            _fullAreaWBSs = _projectWBSs.GroupBy(x => x.Area).Select(g => g.First()).Where(x => x.Area != string.Empty).OrderBy(x => x.Area).ToList();
            _fullSystemWBSs = _projectWBSs.GroupBy(x => x.System).Select(g => g.First()).Where(x => x.System != string.Empty).OrderBy(x => x.System).ToList();
            _fullSubsystemWBSs = _projectWBSs.GroupBy(x => x.Subsystem).Select(g => g.First()).Where(x => x.Subsystem != string.Empty).OrderBy(x => x.Subsystem).ToList();
            _filterAreaWBSs = _fullAreaWBSs.ToList();
            _filterSystemWBSs = _fullSystemWBSs.ToList();
            _filterSubsystemWBSs = _fullSubsystemWBSs.ToList();
            _disciplines = Common.GetAuthDiscipline();
            _categories = Common.GetPunchlistPriorities();
            _stages = Common.GetWorkflows();

            AssignViewComponents(cmbAreaParent, cmbSystemParent, cmbSubsystemParent, cmbDisciplineParent, cmbCategoryParent, cmbStageParent, cmbSearchModeParent);
        }


        protected void AssignViewComponents(CheckedComboBoxEdit cmbAreaParent, CheckedComboBoxEdit cmbSystemParent, CheckedComboBoxEdit cmbSubsystemParent, CheckedComboBoxEdit cmbDisciplineParent, CheckedComboBoxEdit cmbCategoryParent, CheckedComboBoxEdit cmbStageParent, ComboBoxEdit cmbSearchModeParent)
        {
            refCmbArea = cmbAreaParent;
            refCmbSystem = cmbSystemParent;
            refCmbSubsystem = cmbSubsystemParent;
            refCmbSearchMode = cmbSearchModeParent;
            refCmbDiscipline = cmbDisciplineParent;
            refCmbCategory = cmbCategoryParent;
            refCmbStage = cmbStageParent;
            Common.PopulateCmbSearchMode(refCmbSearchMode);

            refCmbArea.Properties.ForceUpdateEditValue = DevExpress.Utils.DefaultBoolean.True;
            refCmbArea.Properties.NullText = "All Areas";
            refCmbArea.EditValue = null;
            refCmbSystem.Properties.ForceUpdateEditValue = DevExpress.Utils.DefaultBoolean.True;
            refCmbSystem.Properties.NullText = "All Systems";
            refCmbSystem.EditValue = null;
            refCmbSubsystem.Properties.ForceUpdateEditValue = DevExpress.Utils.DefaultBoolean.True;
            refCmbSubsystem.Properties.NullText = "All Subsystems";
            refCmbSubsystem.EditValue = null;

            refCmbArea.EditValueChanged += RefCmbArea_EditValueChanged;
            refCmbArea.EditValueChanging += RefCmbArea_EditValueChanging;
            refCmbArea.CustomDisplayText += comboboxEdit_CustomDisplayText;
            refCmbSystem.EditValueChanged += RefCmbSystem_EditValueChanged;
            refCmbSystem.EditValueChanging += RefCmbSystem_EditValueChanging;
            refCmbSystem.CustomDisplayText += comboboxEdit_CustomDisplayText;
            refCmbSubsystem.EditValueChanged += RefCmbSubsystem_EditValueChanged;
            refCmbSubsystem.EditValueChanging += RefCmbSubsystem_EditValueChanging;
            refCmbSubsystem.CustomDisplayText += comboboxEdit_CustomDisplayText;
            refCmbDiscipline.EditValueChanged += RefCmbDiscipline_EditValueChanged;
            refCmbSearchMode.SelectedIndexChanged += RefCmbSearchMode_SelectedIndexChanged;

            if (refCmbCategory != null)
            {
                refCmbCategory.EditValueChanged += RefCmbCategory_EditValueChanged;
                refCmbCategory.Properties.DataSource = _categories;
                refCmbCategory.CheckAll();
                SelectedCategories.AddRange(_categories);
            }

            if (refCmbStage != null)
            {
                refCmbStage.EditValueChanged += RefCmbStage_EditValueChanged;
                refCmbStage.Properties.DataSource = _stages;
                refCmbStage.CheckAll();
                SelectedStages.AddRange(_stages);
            }

            refCmbArea.Properties.DataSource = _filterAreaWBSs;
            refCmbSystem.Properties.DataSource = _filterSystemWBSs;
            refCmbSubsystem.Properties.DataSource = _filterSubsystemWBSs;
            refCmbDiscipline.Properties.DataSource = _disciplines;
            SelectedDisciplines.Add(_defaultDiscipline);
            refCmbDiscipline.EditValue = _defaultDiscipline;
        }

        public void SelectAllDisciplines()
        {
            SelectedDisciplines.Clear();
            SelectedDisciplines.AddRange(_disciplines);
            string disciplineEditValue = string.Empty;
            foreach (string discipline in _disciplines)
            {
                disciplineEditValue += string.Concat(discipline, ", ");
            }

            if (disciplineEditValue.Length >= 2)
                disciplineEditValue = disciplineEditValue.Substring(0, disciplineEditValue.Length - 2);
            refCmbDiscipline.EditValue = disciplineEditValue;
        }

        private void comboboxEdit_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (e.Value != null)
            {
                e.DisplayText = e.Value.ToString();
            }
        }

        private void RefCmbArea_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue != null)
            {
                if(e.NewValue.ToString() == string.Empty)
                    e.NewValue = null;
                else
                {
                    //show null text when the selected list is the full list
                    string fullListCompareValue = constructEditValueFromList(_projectWBSs, (x) => x.Area);
                    if (fullListCompareValue == e.NewValue.ToString())
                        e.NewValue = null;
                }
            }
        }

        private void RefCmbSystem_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue != null)
            {
                if (e.NewValue.ToString() == string.Empty)
                    e.NewValue = null;
                else
                {
                    //show null text when the selected list is the full list
                    string fullListCompareValue = constructEditValueFromList(_filterSystemWBSs, (x) => x.System);
                    if (fullListCompareValue == e.NewValue.ToString())
                        e.NewValue = null;
                }
            }
        }

        private void RefCmbSubsystem_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue != null)
            {
                if (e.NewValue.ToString() == string.Empty)
                    e.NewValue = null;
                else
                {
                    //show null text when the selected list is the full list
                    string fullListCompareValue = constructEditValueFromList(_filterSubsystemWBSs, (x) => x.Subsystem);
                    if (fullListCompareValue == e.NewValue.ToString())
                        e.NewValue = null;
                }
            }
        }

        private string constructEditValueFromList(List<ProjectWBS> projectWBSs, Func<ProjectWBS, string> projectWBSStrMember)
        {
            string returnValue = string.Empty;
            List<string> listValues = new List<string>();
            listValues = projectWBSs.GroupBy(x => projectWBSStrMember(x)).Select(g => g.First()).OrderBy(x => projectWBSStrMember(x)).Select(x => projectWBSStrMember(x)).ToList();
            foreach(string listValue in listValues)
            {
                returnValue += string.Concat(listValue, ", ");
            }

            if (returnValue.Length >= 2)
                returnValue = returnValue.Substring(0, returnValue.Length - 2);

            return returnValue;
        }

        private void RefCmbArea_EditValueChanged(object sender, EventArgs e)
        {
            _filterSystemWBSs = _fullSystemWBSs.ToList();
            Common.ResetCheckEdit(refCmbSystem, _filterSystemWBSs, SelectedSystems);
            _filterSubsystemWBSs = _fullSubsystemWBSs.ToList();
            Common.ResetCheckEdit(refCmbSubsystem, _filterSubsystemWBSs, SelectedSubsystems);
            string subSystemNullText = string.Empty;

            if (refCmbArea.EditValue == null || refCmbArea.EditValue.ToString() == string.Empty)
            {
                SelectedAreas.Clear();
                SelectedSystems.Clear();
                refCmbSystem.Properties.NullText = "All Systems";
                refCmbSystem.EditValue = null;
                SelectedSubsystems.Clear();
                if (refCmbSystem.EditValue == null)
                    refCmbSubsystem.Properties.NullText = "Subsystems from All Areas and All Systems";
                else
                    refCmbSubsystem.Properties.NullText = "Subsystems from All Areas and Selected Systems";
                refCmbSubsystem.EditValue = null;
                return;
            }

            string areaValue = refCmbArea.EditValue.ToString();
            SelectedAreas = areaValue.Split(',').ToList().Select(s => s.Trim()).ToList();

            _filterSystemWBSs = _projectWBSs.Where(x => SelectedAreas.Contains(x.Area)).GroupBy(x => x.System).Select(g => g.First()).OrderBy(x => x.System).ToList();
            refCmbSystem.Properties.DataSource = _filterSystemWBSs;
            refCmbSystem.Properties.NullText = "All Systems from Selected Areas";
            refCmbSystem.EditValue = null;
            refCmbSystem.Refresh();

            _filterSubsystemWBSs = _projectWBSs.Where(x => SelectedAreas.Contains(x.Area)).GroupBy(x => x.Subsystem).Select(g => g.First()).OrderBy(x => x.Subsystem).ToList();
            refCmbSubsystem.Properties.DataSource = _filterSubsystemWBSs;
            refCmbSubsystem.Properties.NullText = "Subsystems from All Areas and All Systems";
            refCmbSubsystem.EditValue = null;
            refCmbSubsystem.Refresh();
        }

        private void RefCmbSystem_EditValueChanged(object sender, EventArgs e)
        {
            _filterSubsystemWBSs = _fullSubsystemWBSs.ToList();
            Common.ResetCheckEdit(refCmbSubsystem, _filterSubsystemWBSs, SelectedSubsystems);
            string subSystemNullText = string.Empty; 
            if (refCmbArea.EditValue == null)
            {
                if (refCmbSystem.EditValue == null)
                    subSystemNullText = "Subsystems from All Areas and All Systems";
                else
                    subSystemNullText = "Subsystems from All Areas and Selected Systems";
            }
            else
            {
                if(refCmbSystem.EditValue == null)
                    subSystemNullText = "Subsystems from All Areas and All Systems";
                else
                    subSystemNullText = "Subsystems from Selected Areas and Selected Systems";
            }    

            if (refCmbSystem.EditValue == null || refCmbSystem.EditValue.ToString() == string.Empty)
            {
                SelectedSystems.Clear();
                SelectedSubsystems.Clear();
                refCmbSubsystem.Properties.NullText = subSystemNullText;
                refCmbSubsystem.EditValue = null;
                return;
            }

            string systemValue = refCmbSystem.EditValue.ToString();
            SelectedSystems = systemValue.Split(',').ToList().Select(s => s.Trim()).ToList();
            _filterSubsystemWBSs = _projectWBSs.Where(x => SelectedSystems.Contains(x.System)).GroupBy(x => x.Subsystem).Select(g => g.First()).OrderBy(x => x.Subsystem).ToList();
            refCmbSubsystem.Properties.DataSource = _filterSubsystemWBSs;
            refCmbSubsystem.Properties.NullText = subSystemNullText;
            refCmbSubsystem.EditValue = null;
            refCmbSubsystem.Refresh();
        }

        private void RefCmbSubsystem_EditValueChanged(object sender, EventArgs e)
        {
            if (refCmbSubsystem.EditValue == null || refCmbSubsystem.EditValue.ToString() == string.Empty)
            {
                SelectedSubsystems.Clear();
                return;
            }

            string subsystemValue = refCmbSubsystem.EditValue.ToString();
            SelectedSubsystems = subsystemValue.Split(',').ToList().Select(s => s.Trim()).ToList();
        }

        private void RefCmbDiscipline_EditValueChanged(object sender, EventArgs e)
        {
            if (refCmbDiscipline.EditValue == null)
                return;

            string disciplineValue = refCmbDiscipline.EditValue.ToString();
            SelectedDisciplines = disciplineValue.Split(',').ToList().Select(s => s.Trim()).ToList();
        }

        private void RefCmbCategory_EditValueChanged(object sender, EventArgs e)
        {
            if (refCmbCategory.EditValue == null)
                return;

            string categoryValue = refCmbCategory.EditValue.ToString();
            SelectedCategories = categoryValue.Split(',').ToList().Select(s => s.Trim()).ToList();
        }

        private void RefCmbStage_EditValueChanged(object sender, EventArgs e)
        {
            if (refCmbStage.EditValue == null)
                return;

            string stageValue = refCmbStage.EditValue.ToString();
            SelectedStages = stageValue.Split(',').ToList().Select(s => s.Trim()).ToList();
        }

        private void RefCmbSearchMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string searchModeWithUnderscore = Common.ReplaceSpacesWith_(refCmbSearchMode.Text);
            SearchMode = Common.ParseEnum<ProjectLibrary.SearchMode>(searchModeWithUnderscore);
        }

        /// <summary>
        /// Clears all selection and assign comboboxes to full list
        /// </summary>
        public void ResetWBSFilters()
        {
            _defaultDiscipline = System_Environment.GetUser().userDiscipline;
            refCmbDiscipline.EditValue = _defaultDiscipline;
            SelectedDisciplines.Clear();
            SelectedDisciplines.Add(_defaultDiscipline);

            if (refCmbCategory != null)
                refCmbCategory.CheckAll();

            if (refCmbStage != null)
                refCmbStage.CheckAll();

            SelectedCategories.Clear();
            SelectedCategories.AddRange(_categories);
            Common.ClearCheckedComboBoxEdit(refCmbArea);
            Common.ClearCheckedComboBoxEdit(refCmbSystem);
            Common.ClearCheckedComboBoxEdit(refCmbSubsystem);
            SelectedAreas.Clear();
            SelectedSystems.Clear();
            SelectedSubsystems.Clear();
            refCmbArea.Properties.DataSource = _fullAreaWBSs;
            refCmbSystem.Properties.DataSource = _fullSystemWBSs;
            refCmbSubsystem.Properties.DataSource = _fullSubsystemWBSs;
        }
    }
}
