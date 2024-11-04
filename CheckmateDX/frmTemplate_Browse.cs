using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using System.Linq;
using DevExpress.XtraRichEdit;
using System.IO;

namespace CheckmateDX
{
    public partial class frmTemplate_Browse : CheckmateDX.frmParent
    {
        //database
        AdapterTEMPLATE_MAIN _daTemplate = new AdapterTEMPLATE_MAIN();
        dsTEMPLATE_MAIN _dsTemplate = new dsTEMPLATE_MAIN();
        AdapterWORKFLOW_MAIN _daWorkflow = new AdapterWORKFLOW_MAIN();
        dsWORKFLOW_MAIN _dsWorkflow = new dsWORKFLOW_MAIN();

        //class lists
        List<wtDisplay> _allWtDisplay = new List<wtDisplay>();

        public frmTemplate_Browse()
        {
            InitializeComponent();
            wtDisplayBindingSource.DataSource = _allWtDisplay;
            PopulateFormElement();
            RefreshWorkflowTemplates();
            cmbDiscipline.SelectedIndexChanged += cmbDiscipline_SelectedIndexChanged;
        }

        #region Form Population
        /// <summary>
        /// Populate combobox
        /// </summary>
        private void PopulateFormElement()
        {
            Common.PopulateCmbAuthDiscipline(cmbDiscipline);
        }
        
        /// <summary>
        /// Refresh all template from database
        /// </summary>
        private void RefreshWorkflowTemplates()
        {
            _allWtDisplay.Clear();

            //retrieve template
            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTemplate = _daTemplate.GetByDiscipline(cmbDiscipline.Text);
            if (dtTemplate != null)
            {
                foreach (dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate in dtTemplate.Rows)
                {
                    _allWtDisplay.Add(new wtDisplay(new Template(drTemplate.GUID)
                    {
                        templateName = drTemplate.NAME,
                        templateWorkFlow = new ValuePair(Common.ConvertWorkflowGuidToName(drTemplate.WORKFLOWGUID), drTemplate.WORKFLOWGUID),
                        templateRevision = drTemplate.REVISION,
                        templateDescription = drTemplate.IsDESCRIPTIONNull() ? "" : drTemplate.DESCRIPTION,
                        templateDiscipline = drTemplate.DISCIPLINE,
                        templateQRSupport = drTemplate.QRSUPPORT,
                        templateSkipApproved = drTemplate.SKIPAPPROVED
                    }));
                }
            }

            //retrieve workflow
            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWorkflow = _daWorkflow.Get();
            if(dtWorkflow != null)
            {
                foreach (dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow in dtWorkflow.Rows)
                {
                    _allWtDisplay.Add(new wtDisplay(new Workflow(drWorkflow.GUID)
                    {
                        workflowName = drWorkflow.NAME,
                        workflowDescription = drWorkflow.IsDESCRIPTIONNull() ? "" : drWorkflow.DESCRIPTION,
                        workflowParentGuid = drWorkflow.PARENTGUID
                    })
                    { wtDisplayName = drWorkflow.NAME });
                }
            }

            treeListTemplate.RefreshDataSource();
            treeListTemplate.ExpandAll();
        }

        #endregion

        #region Events
        private void btnDuplicate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            wtDisplay selectedWt = (wtDisplay)treeListTemplate.GetDataRecordByNode(treeListTemplate.FocusedNode);
            if (selectedWt == null || selectedWt.wtDisplayAttachTemplate == null)
            {
                Common.Warn("Please select a template to duplicate");
                return;
            }

            using (AdapterTEMPLATE_MAIN daTEMPLATE_MAIN = new AdapterTEMPLATE_MAIN())
            {
                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = daTEMPLATE_MAIN.GetBy(selectedWt.wtDisplayAttachTemplate.GUID);
                dsTEMPLATE_MAIN dsTEMPLATE_MAIN = new dsTEMPLATE_MAIN();
                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drNewTemplate = dsTEMPLATE_MAIN.TEMPLATE_MAIN.NewTEMPLATE_MAINRow();
                drNewTemplate.GUID = Guid.NewGuid();
                drNewTemplate.WORKFLOWGUID = drTemplate.WORKFLOWGUID;
                drNewTemplate.NAME = drTemplate.NAME + "-Copy";
                drNewTemplate.REVISION = drTemplate.REVISION;
                drNewTemplate.DISCIPLINE = drTemplate.DISCIPLINE;
                drNewTemplate.DESCRIPTION = drTemplate.DESCRIPTION;
                drNewTemplate.TEMPLATE = drTemplate.TEMPLATE;
                drNewTemplate.QRSUPPORT = drTemplate.QRSUPPORT;
                drNewTemplate.SKIPAPPROVED = drTemplate.SKIPAPPROVED;
                drNewTemplate.CREATED = DateTime.Now;
                drNewTemplate.CREATEDBY = System_Environment.GetUser().GUID;
                dsTEMPLATE_MAIN.TEMPLATE_MAIN.AddTEMPLATE_MAINRow(drNewTemplate);
                daTEMPLATE_MAIN.Save(drNewTemplate);

                RefreshWorkflowTemplates();
            }
        }

        private void btnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.RootFolder = Environment.SpecialFolder.Desktop;

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormDescription("Exporting ...");

                var selectedNodes = treeListTemplate.Selection;

                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, treeListTemplate.Selection.Count);
                string qualifiedNameFormat = string.Empty;

                foreach (TreeListNode selectedNode in selectedNodes)
                {
                    wtDisplay selectedWt = (wtDisplay)treeListTemplate.GetDataRecordByNode(selectedNode);
                    
                    frmTemplate_Main f = new frmTemplate_Main(selectedWt.wtDisplayAttachTemplate);
                    try
                    {
                        qualifiedNameFormat = selectedWt.wtDisplayName.Split('|').First(); 
                        CustomRichEdit customRichEdit = f.GetRichEdit();
                        customRichEdit.Convert_to_Native();
                        customRichEdit.RemoveMergeFields();
                        customRichEdit.Remove_Toggle_Permissions();
                        customRichEdit.ExportToPdf(fd.SelectedPath + "\\" + qualifiedNameFormat + ".pdf");
                    }
                    catch
                    {

                    }
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();
            }
        }

        private void btnDesignTemplate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            wtDisplay selectedWt = (wtDisplay)treeListTemplate.GetDataRecordByNode(treeListTemplate.FocusedNode);
            if (selectedWt == null || selectedWt.wtDisplayAttachTemplate == null)
            {
                Common.Warn("Please select a template to design");
                return;
            }
            
            splashScreenManager1.ShowWaitForm();
            frmTemplate_Main f = new frmTemplate_Main(selectedWt.wtDisplayAttachTemplate);
            splashScreenManager1.CloseWaitForm();
            f.ShowDialog();
        }

        private void gridControlTemplate_DoubleClick(object sender, EventArgs e)
        {
            wtDisplay selectedWt = (wtDisplay)treeListTemplate.GetDataRecordByNode(treeListTemplate.FocusedNode);
            if (selectedWt == null)
                return;

            if (selectedWt.wtDisplayAttachTemplate == null)
                return;

            splashScreenManager1.ShowWaitForm();
            frmTemplate_Main f = new frmTemplate_Main(selectedWt.wtDisplayAttachTemplate);
            splashScreenManager1.CloseWaitForm();
            f.ShowDialog();
        }

        private void btnAddWorkflow_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmTemplate_AddWorkflow frmWorkflowAdd;
            Workflow selectedWorkflow;

            wtDisplay selectedWt = (wtDisplay)treeListTemplate.GetDataRecordByNode(treeListTemplate.FocusedNode);
            if (selectedWt == null || selectedWt.wtDisplayAttachWorkflow == null)
                //when parent is unknown
                frmWorkflowAdd = new frmTemplate_AddWorkflow(Guid.Empty);
            else
            {
                selectedWorkflow = selectedWt.wtDisplayAttachWorkflow;
                //when parent is known, so that comboBox selection will be defaulted to parent
                frmWorkflowAdd = new frmTemplate_AddWorkflow(selectedWorkflow.GUID);
            }

            if (frmWorkflowAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Workflow newWorkflow = frmWorkflowAdd.GetWorkflow();
                dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow = _dsWorkflow.WORKFLOW_MAIN.NewWORKFLOW_MAINRow();
                drWorkflow.GUID = Guid.NewGuid();
                drWorkflow.PARENTGUID = newWorkflow.workflowParentGuid;
                drWorkflow.NAME = newWorkflow.workflowName;
                drWorkflow.DESCRIPTION = newWorkflow.workflowDescription;
                drWorkflow.CREATED = DateTime.Now;
                drWorkflow.CREATEDBY = System_Environment.GetUser().GUID;
                _dsWorkflow.WORKFLOW_MAIN.AddWORKFLOW_MAINRow(drWorkflow);
                _daWorkflow.Save(drWorkflow);
                RefreshWorkflowTemplates();
                //Common.Prompt("Workflow successfully added");
            }
        }

        private void btnAddTemplate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmTemplate_Add frmTemplateAdd = new frmTemplate_Add(_allWtDisplay.Where(x => x.wtDisplayAttachTemplate != null).Select(x => x.wtDisplayAttachTemplate).ToList());

            if (frmTemplateAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Template newTemplate = frmTemplateAdd.GetTemplate();
                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = _dsTemplate.TEMPLATE_MAIN.NewTEMPLATE_MAINRow();
                drTemplate.GUID = Guid.NewGuid();
                drTemplate.DISCIPLINE = Common.ConvertDisplayDisciplineForDB(cmbDiscipline.Text.Trim());
                AssignTemplateDetails(drTemplate, newTemplate);

                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drCopyFromTemplate = getCopyFromTemplate(frmTemplateAdd);
                if (drCopyFromTemplate != null)
                    drTemplate.TEMPLATE = drCopyFromTemplate.TEMPLATE;

                drTemplate.SKIPAPPROVED = newTemplate.templateSkipApproved;
                drTemplate.QRSUPPORT = newTemplate.templateQRSupport;
                drTemplate.CREATED = DateTime.Now;
                drTemplate.CREATEDBY = System_Environment.GetUser().GUID;
                _dsTemplate.TEMPLATE_MAIN.AddTEMPLATE_MAINRow(drTemplate);
                _daTemplate.Save(drTemplate);
                RefreshWorkflowTemplates();
                //Common.Prompt("Template successfully added");
            }
        }

        private dsTEMPLATE_MAIN.TEMPLATE_MAINRow getCopyFromTemplate(frmTemplate_Add frmTemplate)
        {
            string copyFromTemplateName = frmTemplate.GetCopyFromTemplate();
            if (copyFromTemplateName != string.Empty)
            {
                wtDisplay workflowTemplateDisplay = _allWtDisplay.Where(x => x.wtDisplayAttachTemplate != null).FirstOrDefault(x => x.wtDisplayAttachTemplate.templateName == copyFromTemplateName);
                if(workflowTemplateDisplay != null)
                {
                    dsTEMPLATE_MAIN.TEMPLATE_MAINRow drCopyFromTemplate = _daTemplate.GetBy(workflowTemplateDisplay.wtDisplayGuid);
                    if (drCopyFromTemplate != null)
                        return drCopyFromTemplate;
                }
            }

            return null;
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            wtDisplay selectedWt = (wtDisplay)treeListTemplate.GetDataRecordByNode(treeListTemplate.FocusedNode);
            if (selectedWt == null)
            {
                Common.Warn("Please select something to edit");
                return;
            }
                
            if(selectedWt.wtDisplayAttachTemplate != null)
            {
                frmTemplate_Add frmTemplateAdd = new frmTemplate_Add(selectedWt.wtDisplayAttachTemplate, _allWtDisplay.Where(x => x.wtDisplayAttachTemplate != null).Select(x => x.wtDisplayAttachTemplate).ToList());
                if (frmTemplateAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Template editTemplate = frmTemplateAdd.GetTemplate();

                    dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = _daTemplate.GetBy(editTemplate.GUID);
                    AssignTemplateDetails(drTemplate, editTemplate);

                    dsTEMPLATE_MAIN.TEMPLATE_MAINRow drCopyFromTemplate = getCopyFromTemplate(frmTemplateAdd);
                    if (drCopyFromTemplate != null)
                        drTemplate.TEMPLATE = drCopyFromTemplate.TEMPLATE;

                    drTemplate.SKIPAPPROVED = editTemplate.templateSkipApproved;
                    drTemplate.UPDATED = DateTime.Now;
                    drTemplate.UPDATEDBY = System_Environment.GetUser().GUID;
                    _daTemplate.Save(drTemplate);
                    //Common.Prompt("Template successfully updated");
                    RefreshWorkflowTemplates();
                }
            }
            else
            {
                Workflow selectedWorkflow = selectedWt.wtDisplayAttachWorkflow;
                frmTemplate_AddWorkflow frmWorkflowAdd = new frmTemplate_AddWorkflow(selectedWorkflow);
                if (frmWorkflowAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Workflow editWorkflow = frmWorkflowAdd.GetWorkflow();

                    dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow = _daWorkflow.GetBy(editWorkflow.GUID, false);
                    drWorkflow.NAME = editWorkflow.workflowName;
                    drWorkflow.DESCRIPTION = editWorkflow.workflowDescription;
                    drWorkflow.PARENTGUID = editWorkflow.workflowParentGuid;
                    drWorkflow.UPDATED = DateTime.Now;
                    drWorkflow.UPDATEDBY = System_Environment.GetUser().GUID;
                    _daWorkflow.Save(drWorkflow);
                    //Common.Prompt("Workflow successfully updated");
                    RefreshWorkflowTemplates();
                }
            }
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            wtDisplay selectedWt = (wtDisplay)treeListTemplate.GetDataRecordByNode(treeListTemplate.FocusedNode);
            if (selectedWt == null)
                Common.Warn("Please select something to delete");

            if(selectedWt.wtDisplayAttachWorkflow != null)
            {
                if (!Common.Confirmation("Are you sure you want to delete the selected workflow along with its children(s)?", "Confirmation"))
                    return;

                dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWorkflow = new dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable();
                Workflow selectedWorkflow = selectedWt.wtDisplayAttachWorkflow;
                dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWorkflowChild = _daWorkflow.GetWorkflowChildrens(selectedWorkflow.GUID);

                if(dtWorkflowChild != null)
                    foreach (dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflowChild in dtWorkflowChild.Rows)
                    {
                        dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow = dtWorkflow.NewWORKFLOW_MAINRow();
                        drWorkflow.GUID = drWorkflowChild.GUID;
                        drWorkflow.PARENTGUID = drWorkflowChild.PARENTGUID;
                        drWorkflow.NAME = drWorkflowChild.NAME;
                        //need to put these values in because the schema doesn't allow null
                        drWorkflow.CREATED = drWorkflowChild.CREATED;
                        drWorkflow.CREATEDBY = drWorkflowChild.CREATEDBY;
                        dtWorkflow.AddWORKFLOW_MAINRow(drWorkflow);
                    }

                splashScreenManager1.ShowWaitForm();
                rptDeletion f = new rptDeletion(dtWorkflow);
                splashScreenManager1.CloseWaitForm();
                f.ShowReport();
            }
            else
            {
                if (!Common.Confirmation("Are you sure you want to delete the selected template?", "Confirmation"))
                    return;

                dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTemplate = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
                Template selectedTemplate = selectedWt.wtDisplayAttachTemplate;
                //temporary placeholder
                //needs to be able to validate attaching object before deletion
                using(AdapterTEMPLATE_REGISTER daRegister = new AdapterTEMPLATE_REGISTER())
                {
                    if(daRegister.GetByTemplate(selectedTemplate.GUID) != null)
                    {
                        if(Common.Confirmation("This template have WBS/Tag attached, are you sure you want to remove it?", "Warning"))
                            _daTemplate.RemoveBy(selectedTemplate.GUID);
                    }
                    else
                        _daTemplate.RemoveBy(selectedTemplate.GUID);
                }
            }

            RefreshWorkflowTemplates();
        }

        private void cmbDiscipline_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshWorkflowTemplates();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Assigns common template details to data row
        /// </summary>
        /// <param name="drUserMaster">datarow to be assigned</param>
        /// <param name="user">template details</param>
        private void AssignTemplateDetails(dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate, Template template)
        {
            drTemplate.NAME = template.templateName;
            drTemplate.REVISION = template.templateRevision;
            drTemplate.DESCRIPTION = template.templateDescription;
            drTemplate.WORKFLOWGUID = (Guid)template.templateWorkFlow.Value;
            drTemplate.QRSUPPORT = template.templateQRSupport;
        }     

        /// <summary>
        /// A delegated method to unhide this form
        /// </summary>
        private void ShowThisForm()
        {
            this.Show();
        }
        #endregion

        /// <summary>
        /// Dispose local adapters upon closing
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            _daTemplate.Dispose();
            _daWorkflow.Dispose();
            base.OnClosed(e);
        }

        private void barButtonResizeFooter_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormDescription("Fixing ...");

            var selectedNodes = treeListTemplate.Selection;

            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, treeListTemplate.Selection.Count);
            string qualifiedNameFormat = string.Empty;

            foreach (TreeListNode selectedNode in selectedNodes)
            {
                wtDisplay selectedWt = (wtDisplay)treeListTemplate.GetDataRecordByNode(selectedNode);

                frmTemplate_Main f = new frmTemplate_Main(selectedWt.wtDisplayAttachTemplate);
                f.FixFooterFields();
                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            splashScreenManager1.CloseWaitForm();
        }

        private void btnExportDoc_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.RootFolder = Environment.SpecialFolder.Desktop;

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormDescription("Exporting ...");

                var selectedNodes = treeListTemplate.Selection;

                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, treeListTemplate.Selection.Count);
                string qualifiedNameFormat = string.Empty;

                foreach (TreeListNode selectedNode in selectedNodes)
                {
                    wtDisplay selectedWt = (wtDisplay)treeListTemplate.GetDataRecordByNode(selectedNode);

                    frmTemplate_Main f = new frmTemplate_Main(selectedWt.wtDisplayAttachTemplate);
                    try
                    {
                        qualifiedNameFormat = selectedWt.wtDisplayName.Split('|').First();
                        CustomRichEdit customRichEdit = f.GetRichEdit();
                        customRichEdit.Convert_to_Native();
                        customRichEdit.RemoveMergeFields();
                        customRichEdit.Remove_Toggle_Permissions();
                        customRichEdit.SaveDocument(fd.SelectedPath + "\\" + qualifiedNameFormat + ".odt", DocumentFormat.OpenDocument);
                    }
                    catch
                    {

                    }
                    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager1.CloseWaitForm();
            }
        }

        private void btnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            wtDisplay selectedWt = (wtDisplay)treeListTemplate.GetDataRecordByNode(treeListTemplate.FocusedNode);
            if (selectedWt == null || selectedWt.wtDisplayAttachTemplate != null)
            {
                Common.Warn("Please select a workflow to import to");
                return;
            }

            OpenFileDialog thisDialog = new OpenFileDialog();

            thisDialog.Filter = "DOC (*.DOCX)|*.DOCX";
            thisDialog.FilterIndex = 1;
            thisDialog.RestoreDirectory = true;
            thisDialog.Multiselect = true;
            thisDialog.Title = "Please Select Documents";
            
            if (thisDialog.ShowDialog() == DialogResult.OK)
            {
                string discipline = Common.ConvertDisplayDisciplineForDB(cmbDiscipline.Text.Trim());
                using (AdapterTEMPLATE_MAIN daTEMPLATE_MAIN = new AdapterTEMPLATE_MAIN())
                {
                    splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
                    splashScreenManager1.ShowWaitForm();
                    splashScreenManager1.SendCommand(CheckmateDX.ProgressForm.WaitFormCommand.SetLargeFormat, 1000);
                    splashScreenManager1.SendCommand(CheckmateDX.ProgressForm.WaitFormCommand.SetProgressMax, thisDialog.FileNames.Count());
                    foreach (string filename in thisDialog.FileNames)
                    {
                        CustomRichEdit customRichEdit1 = new CustomRichEdit();
                        customRichEdit1.LoadDocument(filename);
                        List<string> splitFileName = filename.Split('\\').ToList();
                        string actualFileName = splitFileName.Last().Replace(".docx", "");
                        string templateNumber = actualFileName.Split(' ').First();
                        using (MemoryStream ms = new MemoryStream())
                        {
                            splashScreenManager1.SetWaitFormCaption("Importing document " + splitFileName.Last() + " ...");
                            customRichEdit1.SaveDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);

                            dsTEMPLATE_MAIN dsTEMPLATE_MAIN = new dsTEMPLATE_MAIN();
                            dsTEMPLATE_MAIN.TEMPLATE_MAINRow drNewTemplate = dsTEMPLATE_MAIN.TEMPLATE_MAIN.NewTEMPLATE_MAINRow();
                            drNewTemplate.GUID = Guid.NewGuid();
                            drNewTemplate.TEMPLATE = ms.ToArray();
                            drNewTemplate.WORKFLOWGUID = selectedWt.wtDisplayAttachWorkflow.GUID;
                            drNewTemplate.NAME = templateNumber;
                            drNewTemplate.REVISION = "A";
                            drNewTemplate.DISCIPLINE = discipline;
                            drNewTemplate.DESCRIPTION = actualFileName;
                            drNewTemplate.QRSUPPORT = true;
                            drNewTemplate.SKIPAPPROVED = false;
                            drNewTemplate.CREATED = DateTime.Now;
                            drNewTemplate.CREATEDBY = System_Environment.GetUser().GUID;
                            dsTEMPLATE_MAIN.TEMPLATE_MAIN.AddTEMPLATE_MAINRow(drNewTemplate);
                            daTEMPLATE_MAIN.Save(drNewTemplate);
                            splashScreenManager1.SendCommand(CheckmateDX.ProgressForm.WaitFormCommand.PerformStep, null);
                        }
                        customRichEdit1.Dispose();
                    }
                }

                splashScreenManager1.CloseWaitForm();
                RefreshWorkflowTemplates();
            }
        }
    }
}
