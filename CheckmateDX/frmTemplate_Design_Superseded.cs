using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.Spreadsheet;

namespace CheckmateDX
{
    public delegate void TestDelegate();
    public partial class frmTemplate_Design_Superseded : CheckmateDX.frmParent
    {
        public delegate void UnhideManageForm();
        public UnhideManageForm ShowManageForm;
        Template _template;
        AdapterTEMPLATE_MAIN _daTemplate = new AdapterTEMPLATE_MAIN();
        AdapterPREFILL_MAIN _daPrefill = new AdapterPREFILL_MAIN();
        List<RichEditCommandDelegateContainer> _replaceCommandMethod = new List<RichEditCommandDelegateContainer>();
        List<Prefill> _allPrefill = new List<Prefill>();
        List<Feature> _allFeature = new List<Feature>();

        IWorkbook _workbook;
        Worksheet _worksheet;

        public frmTemplate_Design_Superseded(Template SelectedTemplate)
        {
            InitializeComponent();
            SetReplaceCommandMethod();
            spreadsheetControl1.Options.Behavior.Worksheet.Insert = DocumentCapability.Hidden;
            spreadsheetControl1.Options.Behavior.Worksheet.Delete = DocumentCapability.Hidden;
            _template = SelectedTemplate;
            this.Text = SelectedTemplate.templateName;
            LoadTemplate();
            LoadPrefillComboBox();
            //LoadFeatureComboBox();
            GetPrefillData();
            _workbook = spreadsheetControl1.Document;
            _worksheet = _workbook.Worksheets[0];
        }

        #region Initialization
        /// <summary>
        /// Load the spreadsheet template from database
        /// </summary>
        private void LoadTemplate()
        {
            byte[] receivedBytes;
            dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = _daTemplate.GetBy(_template.GUID);
            if (drTemplate != null && !drTemplate.IsTEMPLATENull())
            {
                receivedBytes = drTemplate.TEMPLATE;
                spreadsheetControl1.Document.LoadDocument(receivedBytes, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
            }
        }

        /// <summary>
        /// Assigns datasource and formats the prefill searchLookupComboBox
        /// </summary>
        private void LoadPrefillComboBox()
        {
            RefreshPrefills();
            //prefillBindingSource.DataSource = _allPrefill;
            ////Specify visible columns
            //repositoryItemSearchLookUpEdit1.View.Columns.AddField("prefillDiscipline").Visible = true;
            //repositoryItemSearchLookUpEdit1.View.Columns.AddField("prefillCategory").Visible = true;
            //repositoryItemSearchLookUpEdit1.View.Columns.AddField("prefillName").Visible = true;
            //repositoryItemSearchLookUpEdit1.View.Columns.AddField("prefillExists").Visible = true;
            ////Specify columns parameters
            //repositoryItemSearchLookUpEdit1.View.Columns.ColumnByFieldName("prefillDiscipline").Caption = "Discipline";
            //repositoryItemSearchLookUpEdit1.View.Columns.ColumnByFieldName("prefillDiscipline").MaxWidth = 80;
            //repositoryItemSearchLookUpEdit1.View.Columns.ColumnByFieldName("prefillCategory").Caption = "Category";
            //repositoryItemSearchLookUpEdit1.View.Columns.ColumnByFieldName("prefillCategory").MaxWidth = 100;
            //repositoryItemSearchLookUpEdit1.View.Columns.ColumnByFieldName("prefillName").Caption = "Name";
            //repositoryItemSearchLookUpEdit1.View.Columns.ColumnByFieldName("prefillExists").Caption = "Exists";
            //repositoryItemSearchLookUpEdit1.View.Columns.ColumnByFieldName("prefillExists").MaxWidth = 50;
        }

        ///// <summary>
        ///// Assigns datasource and formats the feature searchLookupComboBox
        ///// </summary>
        //private void LoadFeatureComboBox()
        //{
        //    RefreshFeatures();
        //    featureBindingSource.DataSource = _allFeature;
        //    //Specify visible columns
        //    repositoryItemSearchLookUpEdit2.View.Columns.AddField("featureCategory").Visible = true;
        //    repositoryItemSearchLookUpEdit2.View.Columns.AddField("featureName").Visible = true;
        //    repositoryItemSearchLookUpEdit2.View.Columns.AddField("featureExists").Visible = true;
        //    //Specify columns parameters
        //    repositoryItemSearchLookUpEdit2.View.Columns.ColumnByFieldName("featureCategory").Caption = "Category";
        //    repositoryItemSearchLookUpEdit2.View.Columns.ColumnByFieldName("featureCategory").MaxWidth = 80;
        //    repositoryItemSearchLookUpEdit2.View.Columns.ColumnByFieldName("featureName").Caption = "Name";
        //    repositoryItemSearchLookUpEdit2.View.Columns.ColumnByFieldName("featureExists").Caption = "Exists";
        //    repositoryItemSearchLookUpEdit2.View.Columns.ColumnByFieldName("featureExists").MaxWidth = 50;
        //}


        /// <summary>
        /// Refresh all prefill from database
        /// </summary>
        private void RefreshPrefills()
        {
            _allPrefill.Clear();
            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPrefill = _daPrefill.Get();

            if (dtPrefill != null)
            {
                foreach (dsPREFILL_MAIN.PREFILL_MAINRow drPrefill in dtPrefill.Rows)
                {
                    if(drPrefill.DISCIPLINE == _template.templateDiscipline)
                    {
                        _allPrefill.Add(new Prefill(drPrefill.GUID)
                        {
                            prefillName = drPrefill.NAME,
                            prefillCategory = drPrefill.CATEGORY,
                            prefillDiscipline = drPrefill.DISCIPLINE,
                            CreatedDate = drPrefill.CREATED,
                            CreatedBy = drPrefill.CREATEDBY
                        });
                    }
                }
            }

            barEditPrefill.Refresh();
        }

        ///// <summary>
        ///// Refresh all feature from static list
        ///// </summary>
        //private void RefreshFeatures()
        //{
        //    _allFeature.Clear();
        //    List<Feature> allFeature = Template_Feature.GetList();
        //    Discipline userDiscipline = (Discipline)Enum.Parse(typeof(Discipline), _template.templateDiscipline);
        //    _allFeature = allFeature.Where(obj => obj.featureDiscipline == userDiscipline).ToList();

        //    barEditFeature.Refresh();
        //}
        #endregion

        /// <summary>
        /// Assign local method to commands on spreadsheet
        /// </summary>
        private void SetReplaceCommandMethod()
        {
            //_replaceCommandMethod.Add(new CommandDelegateContainer(SpreadsheetCommandId.FileSave, new CommandDelegate(SaveTemplateToDB)));
        }

        #region event
        /// <summary>
        /// Detects whether prefill exists in the spreadsheet
        /// </summary>
        private void barEditPrefill_ShowingEditor(object sender, DevExpress.XtraBars.ItemCancelEventArgs e)
        {
            foreach(Prefill prefill in _allPrefill)
            {
                if (IsPrefillExists(prefill, false))
                    prefill.prefillExists = true;
                else
                    prefill.prefillExists = false;
            }

            barEditPrefill.Refresh();
        }

        private void barBtnRemovePrefill_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Prefill selectedPrefill = GetSelectedPrefill();
            if (selectedPrefill == null)
            {
                Common.Warn("Please select a prefill to remove");
                return;
            }

            ShortGuid sGuid = selectedPrefill.GUID;
            if (IsPrefillExists(selectedPrefill, true))
            {
                if (Common.Confirmation("Are you sure you want to remove this prefill item?", "Confirmation"))
                {
                    DefinedNameCollection dNames = _worksheet.DefinedNames;
                    dNames.Remove(sGuid.ToString());
                    _worksheet.SelectedCell.Value = string.Empty;
                }
            }
            else
                Common.Prompt("This prefill doesn't exists in the spreadsheet");
        }

        private void barBtnRemoveFeature_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Feature selectedFeature = GetSelectedFeature();
            //if (selectedFeature == null)
            //{
            //    Common.Warn("Please select a feature to remove");
            //    return;
            //}

            //int featureCount = countFeature(selectedFeature);
            //if (featureCount > 0)
            //{
            //    if(Common.Confirmation("Are you sure you want to remove every instances of this feature on this spreadsheet?", "Confirmation"))
            //    {
            //        DefinedNameCollection dNames = _worksheet.DefinedNames;
                    
            //        for (int i = 0; i < featureCount; i++)
            //        {
            //            _worksheet.Range[selectedFeature.featureTypeID.ToString() + i.ToString()].Value = string.Empty;
            //            dNames.Remove(selectedFeature.featureTypeID.ToString() + i.ToString());
            //        }
            //    }
            //}
            //else
            //    Common.Prompt("This feature doesn't exists in the spreadsheet");
        }

        private void barBtnInsertPrefill_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Prefill selectedPrefill = GetSelectedPrefill();

            if (selectedPrefill == null)
                return;

            ShortGuid sGuid = selectedPrefill.GUID;
            try
            {
                _worksheet.SelectedCell.Name = sGuid.ToString();
                _worksheet.SelectedCell.Value = selectedPrefill.prefillName;
            }
            catch
            {
                //If we cannot set the cell name it is probably due to cell name already exists in spreadsheet
                if (IsPrefillExists(selectedPrefill, true))
                    Common.Warn("Prefill already exists!");
                else
                    Common.Warn("Unable to set prefill, please contact the system administrator");
            }
        }

        private void barBtnInsertFeature_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Feature selectedFeature = GetSelectedFeature();

            //if (selectedFeature == null)
            //    return;

            //int featureCount = countFeature(selectedFeature);
            //try
            //{
            //    _worksheet.SelectedCell.Name = selectedFeature.featureTypeID.ToString() + featureCount.ToString();
            //    _worksheet.SelectedCell.Value = selectedFeature.featureName;
            //}
            //catch
            //{
            //    //Sometimes this happens when user clicks too fast
            //    Common.Warn("Unexpected error, please try again");
            //}
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (ShowManageForm != null)
                ShowManageForm();

            base.OnFormClosing(e);
        }
        #endregion

        #region To be implemented in ITR form
        /// <summary>
        /// For Reference Only: To be implemented in ITR to replace cells marked as prefill by value
        /// </summary>
        private void GetPrefillData()
        {
            IWorkbook workbook = spreadsheetControl1.Document;
            Worksheet worksheet = workbook.Worksheets[0];
            CellRange usedRange = worksheet.GetUsedRange();
            foreach (Cell usedCell in usedRange)
            {
                if (usedCell.Formula.Contains("=HEADER"))
                {
                    usedCell.Value = "TEST";
                }
            }
        }

        /// <summary>
        /// For Reference Only: To be implemented in ITR to protect cells
        /// </summary>
        private void ProtectWorkbook()
        {
            spreadsheetControl1.ActiveWorksheet.Protect("", DevExpress.Spreadsheet.WorksheetProtectionPermissions.Default);
        } 
        #endregion

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _daTemplate.Dispose();
            base.OnFormClosed(e);
        }

        #region Helper
        /// <summary>
        /// Save the spreadsheet template to database
        /// </summary>
        private void SaveTemplateToDB()
        {
            dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = _daTemplate.GetBy(_template.GUID);
            if (drTemplate != null)
            {
                drTemplate.TEMPLATE = spreadsheetControl1.SaveDocument(DevExpress.Spreadsheet.DocumentFormat.OpenXml);
                _daTemplate.Save(drTemplate);
            }
        }

        /// <summary>
        /// Gets the selected prefill
        /// </summary>
        private Prefill GetSelectedPrefill()
        {
            if (barEditPrefill.EditValue == null || barEditPrefill.EditValue.ToString() == "")
            {
                Common.Warn("Please choose a prefill");
                return null;
            }

            Prefill selectedPrefill = (Prefill)barEditPrefill.EditValue;

            return selectedPrefill;
        }

        /// <summary>
        /// Gets the selected feature
        /// </summary>
        //private Feature GetSelectedFeature()
        //{
        //    if (barEditFeature.EditValue == null || barEditFeature.EditValue.ToString() == "")
        //    {
        //        Common.Warn("Please choose a feature");
        //        return null;
        //    }

        //    Feature selectedFeature = (Feature)barEditFeature.EditValue;

        //    return selectedFeature;
        //}

        /// <summary>
        /// Search and focuses on prefill
        /// </summary>
        private bool IsPrefillExists(Prefill selectedPrefill, bool focus)
        {
            ShortGuid sGuid = selectedPrefill.GUID;
            CellRange usedRange = _worksheet.GetUsedRange();
            foreach (Cell usedCell in usedRange)
            {
                if (usedCell.Name == sGuid.ToString())
                {
                    if(focus)
                        _worksheet.SelectedCell = usedCell;

                    return true;
                }
            }

            return false;
        }

        private int countFeature(Feature selectedFeature)
        {
            int count = 0;
            CellRange usedRange = _worksheet.GetUsedRange();
            foreach(Cell usedCell in usedRange)
            {
                if(usedCell.Name.Contains(selectedFeature.featureTypeID.ToString()))
                {
                    count += 1;
                }
            }

            return count;
        }
        #endregion
    }
}
