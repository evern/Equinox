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
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using System.Linq;
using System.Drawing.Drawing2D;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraPrinting;
using System.IO;
using DevExpress.Pdf;
using System.Diagnostics;
using DevExpress.XtraPrinting.Preview;
using System.Drawing.Printing;
using ZXing;
using System.Threading;
using System.Collections.Concurrent;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraEditors.Controls;
using static ProjectCommon.Common;
using DevExpress.XtraEditors;

namespace CheckmateDX
{
    public partial class frmITR_Browse : frmParent
    {
        //database
        AdapterITR_MAIN _daITR = new AdapterITR_MAIN();
        AdapterWBS _daWBS = new AdapterWBS();
        AdapterITR_STATUS _daITRStatus = new AdapterITR_STATUS();
        AdapterITR_STATUS_ISSUE _daITRStatusIssue = new AdapterITR_STATUS_ISSUE();
        AdapterPREFILL_MAIN _daPREFILL = new AdapterPREFILL_MAIN();
        AdapterPREFILL_REGISTER _daPREFILL_REGISTER = new AdapterPREFILL_REGISTER();
        AdapterTAG _daTag = new AdapterTAG();
        AdapterSCHEDULE _daSchedule = new AdapterSCHEDULE();
        AdapterTEMPLATE_MAIN _daTemplate = new AdapterTEMPLATE_MAIN();
        AdapterWORKFLOW_MAIN _daWorkflow = new AdapterWORKFLOW_MAIN();
        dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable _dtTemplates;

        dsTAG _dsTag = new dsTAG();
        dsWBS _dsWBS = new dsWBS();
        dsSCHEDULE _dsSchedule = new dsSCHEDULE();

        //master lists
        List<Workflow> _masterWorkflow = new List<Workflow>(); //stores the global workflow
        List<WorkflowTemplateTagWBS> _masterTemplate = new List<WorkflowTemplateTagWBS>(); //stores all the registered template by project/discipline
        List<WorkflowTemplateTagWBS> _masterITR = new List<WorkflowTemplateTagWBS>(); //stores all the initialited ITR by project/discipline

        //class lists
        List<Schedule> _allSchedule = new List<Schedule>();
        List<wbsTagDisplay> _allWBSTagDisplay = new List<wbsTagDisplay>();
        List<wbsTagDisplay> _filterWBSTagDisplay = new List<wbsTagDisplay>();
        List<WorkflowTemplateTagWBS> _allPending = new List<WorkflowTemplateTagWBS>();
        List<WorkflowTemplateTagWBS> _allStatus = new List<WorkflowTemplateTagWBS>(); //stores all filtered ITR from saved to completed
        List<WorkflowTemplateTagWBS> _allSaved = new List<WorkflowTemplateTagWBS>();
        List<WorkflowTemplateTagWBS> _allInspected = new List<WorkflowTemplateTagWBS>();
        List<WorkflowTemplateTagWBS> _allApproved = new List<WorkflowTemplateTagWBS>();
        List<WorkflowTemplateTagWBS> _allCompleted = new List<WorkflowTemplateTagWBS>();
        List<WorkflowTemplateTagWBS> _allClosed = new List<WorkflowTemplateTagWBS>();
        List<ProjectWBS> _fullProjectWBS = new List<ProjectWBS>();
        List<iTRComments> _allComments = new List<iTRComments>();
        List<wbsTagDisplay> _allWBSStore = new List<wbsTagDisplay>();
        //all template table assigned to project and discipline
        dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable _dtTemplateMaster = new dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable();

        //all ITR assigned to project and discipline
        dsITR_MAIN.ITR_MAINDataTable _dtITRMaster = new dsITR_MAIN.ITR_MAINDataTable();

        //variables
        bool _showArea = false;
        Guid? _templateGuid = null;
        Guid? _wbsTagGuid = null;
        List<Guid> _excludeGuids = new List<Guid>();
        Guid _commentingITRStatus = Guid.Empty; //stores the status on which user will be commenting on
        List<string> _unboundColumns = new List<string>();
        bool _isShowCompletedAndClosedITRsOnly = false;
        Guid _projectGuid = Guid.Empty;
        string _defaultDiscipline = string.Empty;
        //List<TreeListNode> _persistentSelection = new List<TreeListNode>();

        WBSSearch WBSSearch;
        public frmITR_Browse()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// ITR Browser Constructor
        /// </summary>
        /// <param name="WBSTagOnly">Show WBS Tag Only</param>
        public frmITR_Browse(List<Guid> ExcludeGuid, Guid? templateGuid = null, Guid? wbsTagGuid = null)
            : base()
        {
            InitializeComponent();
            treeListWBSTag.OptionsFind.HighlightFindResults = true;

            EstablishTreeListEvents();
            _templateGuid = templateGuid;
            _wbsTagGuid = wbsTagGuid;
            _excludeGuids = ExcludeGuid;

            timer1.Enabled = true; //this is where the form gets filled out, timer is used because superadmin will have to select parameters

            populatePrefillFilters();
            _isShowCompletedAndClosedITRsOnly = System_Environment.HasPrivilege(PrivilegeTypeID.ShowCompletedAndClosedITRsOnly);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            WBSSearch.ResetWBSFilters();
            txtTagNumber.Text = "";
            cmbSearchMode.SelectedIndex = 1;
            resetTreeLists();
        }

        private void populatePrefillFilters()
        {
            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPREFILL = _daPREFILL.Get();
            foreach(dsPREFILL_MAIN.PREFILL_MAINRow drPREFILL in dtPREFILL.Rows)
            {
                if(drPREFILL.CATEGORY.ToUpper().Contains("FILTER"))
                {
                    _unboundColumns.Add(drPREFILL.NAME);
                    createColumn(drPREFILL.NAME, treeListPending);
                    createColumn(drPREFILL.NAME, treeListSaved);
                    createColumn(drPREFILL.NAME, treeListInspected);
                    createColumn(drPREFILL.NAME, treeListApproved);
                    createColumn(drPREFILL.NAME, treeListCompleted);
                    createColumn(drPREFILL.NAME, treeListClosed);
                }
            }

            treeListSaved.Refresh();
        }

        private void createColumn(string fieldName, TreeList treeList)
        {
            TreeListColumn newColumn = new TreeListColumn();
            newColumn.FieldName = fieldName;
            newColumn.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
            newColumn.Visible = true;
            newColumn.Caption = newColumn.FieldName;
            newColumn.OptionsColumn.AllowEdit = false;
            newColumn.VisibleIndex = treeList.Columns.Count - 1;
            treeList.Columns.Add(newColumn);
        }

        List<Tuple<Guid, string, string>> cachedHeaderDatas = new List<Tuple<Guid, string, string>>();
        private void treeList_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            if(e.IsGetData)
            {
                if (_unboundColumns.Contains(e.Column.FieldName))
                {
                    WorkflowTemplateTagWBS workflowTemplate = (WorkflowTemplateTagWBS)e.Row;

                    if(workflowTemplate.wtDisplayAttachTag != null) 
                        e.Value = Common.GetHeaderDataFromTag(workflowTemplate.wtDisplayAttachTag, cachedHeaderDatas, _daPREFILL_REGISTER, e.Column.FieldName);
                    if (workflowTemplate.wtDisplayAttachWBS != null)
                        e.Value = Common.GetHeaderDataFromWBS(workflowTemplate.wtDisplayAttachWBS, cachedHeaderDatas, _daPREFILL_REGISTER, e.Column.FieldName);
                }
            }
        }

        #region public member
        private List<wbsTagDisplay> selectedwbstags;
        public List<wbsTagDisplay> SelectedWBSTags
        {
            get
            {
                if (selectedwbstags == null)
                    selectedwbstags = new List<wbsTagDisplay>();

                return selectedwbstags;
            }
            set
            {
                selectedwbstags = value;
            }
        }
        #endregion

        #region Initialisation
        /// <summary>
        /// Establish the treelist Events
        /// </summary>
        private void EstablishTreeListEvents()
        {
            treeListPending.DoubleClick += treeList_DoubleClick;
            treeListSaved.DoubleClick += treeList_DoubleClick;
            treeListSaved.MouseClick += treeList_MouseClick;
            treeListInspected.DoubleClick += treeList_DoubleClick;
            treeListInspected.MouseClick += treeList_MouseClick;
            treeListApproved.DoubleClick += treeList_DoubleClick;
            treeListApproved.MouseClick += treeList_MouseClick;
            treeListCompleted.DoubleClick += treeList_DoubleClick;
            treeListCompleted.MouseClick += treeList_MouseClick;
            treeListClosed.MouseClick += treeList_MouseClick;
            treeListClosed.DoubleClick += treeList_DoubleClick;
        }

        /// <summary>
        /// Binds all treelist to their corresponding class list
        /// </summary>
        private void EstablishTreeListBinding()
        {
            _masterWorkflow = new List<Workflow>(); //stores the global workflow
            _masterTemplate = new List<WorkflowTemplateTagWBS>(); //stores all the registered template by project/discipline
            _masterITR = new List<WorkflowTemplateTagWBS>(); //stores all the initialited ITR by project/discipline

            _allSchedule = new List<Schedule>();
            _allWBSTagDisplay = new List<wbsTagDisplay>();
            _filterWBSTagDisplay = new List<wbsTagDisplay>();
            _allPending = new List<WorkflowTemplateTagWBS>();
            _allStatus = new List<WorkflowTemplateTagWBS>();
            _allSaved = new List<WorkflowTemplateTagWBS>();
            _allInspected = new List<WorkflowTemplateTagWBS>();
            _allApproved = new List<WorkflowTemplateTagWBS>();
            _allCompleted = new List<WorkflowTemplateTagWBS>();
            _allClosed = new List<WorkflowTemplateTagWBS>();
            _allComments = new List<iTRComments>();

            //wbsTagDisplayBindingSource.DataSource = _filterWBSTagDisplay;
            wbsTagDisplayBindingSource.DataSource = _allWBSTagDisplay;
            workflowTemplateTagWBSPending.DataSource = _allPending;
            workflowTemplateTagWBSSaved.DataSource = _allSaved;
            workflowTemplateTagWBSInspected.DataSource = _allInspected;
            workflowTemplateTagWBSApproved.DataSource = _allApproved;
            workflowTemplateTagWBSCompleted.DataSource = _allCompleted;
            workflowTemplateTagWBSClosed.DataSource = _allClosed;
            iTRCommentsBindingSource.DataSource = _allComments;
        }
        #endregion

        #region Form Population
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (_projectGuid == Guid.Empty)
            {
                selectScheduleParameters();
            }

            if (_templateGuid != null)
                Initialize_WBSTagOnly();

            txtTagNumber.Enter += new EventHandler(Common.textBox_GotFocus);
            txtTagNumber.Leave += new EventHandler(Common.textBox_Leave);
        }

        /// <summary>
        /// Used when user is superadmin, which doesn't have default project and discipline
        /// </summary>
        private void selectScheduleParameters()
        {
            //set default project and discipline when user is not superadmin
            _projectGuid = (Guid)System_Environment.GetUser().userProject.Value;
            _defaultDiscipline = System_Environment.GetUser().userDiscipline;

            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                frmTool_Project frmSelectProject = new frmTool_Project();
                frmSelectProject.ShowAsDialog();
                if (frmSelectProject.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _projectGuid = frmSelectProject.GetSelectedProject().GUID;
                }
                else
                {
                    this.Close();
                    return;
                }

                columnSavedCreated.Visible = true;
                columnInspectedCreated.Visible = true;
                columnApprovedCreated.Visible = true;
                columnCompletedCreated.Visible = true;
                columnClosedCreated.Visible = true;
            }

            //get user parameter
            _fullProjectWBS = _daWBS.GetProjectWBS(_projectGuid);
            WBSSearch = new WBSSearch(_defaultDiscipline, _fullProjectWBS, cmbArea, cmbSystem, cmbSubsystem, cmbDiscipline, null, cmbStage, cmbSearchMode);
        }

        private void Center(Form form)
        {
            form.Location = new Point((Screen.PrimaryScreen.Bounds.Size.Width / 2) - (form.Size.Width / 2), (Screen.PrimaryScreen.Bounds.Size.Height / 2) - (form.Size.Height / 2));
        }

        /// <summary>
        /// Initialize browser without ITRs
        /// </summary>
        private void Initialize_WBSTagOnly()
        {
            EstablishTreeListBinding();
            EstablishWBSTag(_projectGuid, _excludeGuids, _templateGuid, _wbsTagGuid); //gets all the WBS/Tag by this project (_allWBSTagDisplay initialised)
            //tableLayoutPanel1.GetControlFromPosition(1, 0).Dispose();
            //tableLayoutPanel1.SetColumnSpan(pnlITR, 2);
            colwbsTagDisplayTotal.Visible = false;
            this.Width = 500;
            btnOk.Visible = true;
            btnCancel.Visible = true;
            btnRemoveEntry.Visible = true;
            btnLogDuplicates.Visible = false;
            btnClearSelection.Visible = false;
            btnCollapseAll.Visible = false;
            btnExpandAll.Visible = false;
            treeListWBSTag.OptionsSelection.UseIndicatorForSelection = true;
            treeListWBSTag.OptionsView.ShowCheckBoxes = true;
            treeListWBSTag.RowHeight = 50;
            Center(this);
            RefreshWBSTagTreeList();
        }

        private void btnLoadAll_Click(object sender, EventArgs e)
        {
            btnReset_Click(null, null);
            WBSSearch.SelectAllDisciplines();
            btnSearchByTag_Click(null, null);
        }

        private void btnSearchByTag_Click(object sender, EventArgs e)
        {
            string strSearchTagNumber = txtTagNumber.EditValue == null ? string.Empty : txtTagNumber.EditValue.ToString();
            List<string> selectedSubsystems = WBSSearch.SelectedSubsystems.Count == 0 || WBSSearch.SelectedSubsystems.All(x => x == "") ? null : WBSSearch.SelectedSubsystems;
            List<string> selectedSystems = WBSSearch.SelectedSystems.Count == 0 || WBSSearch.SelectedSystems.All(x => x == "") ? null : WBSSearch.SelectedSystems;
            List<string> selectedAreas = WBSSearch.SelectedAreas.Count == 0 || WBSSearch.SelectedAreas.All(x => x == "") ? null : WBSSearch.SelectedAreas;

            searchWBSTag(WBSSearch.SelectedDisciplines, selectedSubsystems, selectedSystems, selectedAreas, strSearchTagNumber, WBSSearch.SelectedStages, WBSSearch.SearchMode);

            deselectAllWBSTag();
            //prevent FilterWBSTag_RefreshTemplateITR from being invoked again after it's invoked in searchWBSTag
            //btnClearSelection_Click(null, null);
            applyBestWidth();
        }

        /// <summary>
        /// Initialize browser with ITRs
        /// </summary>
        private void Initialize_WBSTagITR()
        {
            beginUpdateTreeList();
            EstablishTreeListBinding();
            EstablishWBSTag(_projectGuid, _excludeGuids); //gets all the WBS/Tag by this project (_allWBSTagDisplay initialised)
            FilteringWBSTags(); //filter the WBS/Tag according to the textbox value (_filterWBSTagDisplay initialised)
            EstablishWorkflow(); //establish the workflow in the system (_masterWorkflow initialised)
            //PopulateWorkflow(_masterWorkflow); //populate all workflow in the system //Changes 29th Jan 2015 - Populate workflow happens in FilterWBSTag_RefreshTemplateITR below
            EstablishProjectITR(_projectGuid, _allWBSTagDisplay); //establish all the initialised ITR by project and discipline (_masterITR initialised)
            EstablishProjectTemplate(_projectGuid, _allWBSTagDisplay); //establish all the registered template by project and discipline (_masterTemplate initialised)
            FilterWBSTag_RefreshTemplateITR(_filterWBSTagDisplay, true); //depending on filter or selection, populate the ITR/Template in their corresponding list
            RefreshWBSTagTreeList();
            endUpdateTreeList();
            //btnClearSelection_Click(null, null);
        }

        private void searchWBSTag(List<string> disciplines = null, List<string> subSystemNames = null, List<string> systemNames = null, List<string> areaNames = null, string strSearchTag = "", List<string> stages = null, ProjectLibrary.SearchMode searchMode = ProjectLibrary.SearchMode.Contains)
        {
            _allWBSStore.Clear();
            if(strSearchTag != string.Empty || (subSystemNames != null && subSystemNames.Count > 0) || (systemNames != null && systemNames.Count > 0) || (areaNames != null && areaNames.Count > 0) || (disciplines != null && disciplines.Count > 0) || (stages != null && stages.Count > 0))
            {
                beginUpdateTreeList();
                EstablishTreeListBinding();
                EstablishWBSTag(_projectGuid, _excludeGuids, null, null, disciplines, subSystemNames, systemNames, areaNames, strSearchTag, stages, searchMode); //gets all the WBS/Tag by this project and discipline (_allWBSTagDisplay initialised)
                FilteringWBSTags(); //filter the WBS/Tag according to the textbox value (_filterWBSTagDisplay initialised)
                EstablishWorkflow(); //establish the workflow in the system (_masterWorkflow initialised)
                                     //PopulateWorkflow(_masterWorkflow); //populate all workflow in the system //Changes 29th Jan 2015 - Populate workflow happens in FilterWBSTag_RefreshTemplateITR below
                EstablishProjectITR(_projectGuid, _allWBSTagDisplay, disciplines, subSystemNames, systemNames, areaNames, strSearchTag, stages, searchMode); //establish all the initialised ITR by project and discipline (_masterITR initialised)
                
                if(!_isShowCompletedAndClosedITRsOnly)
                    EstablishProjectTemplate(_projectGuid, _allWBSTagDisplay, stages); //establish all the registered template by project and discipline (_masterTemplate initialised)

                deselectAllWBSTag(); //prevent filtering a single WBS
                FilterWBSTag_RefreshTemplateITR(_filterWBSTagDisplay); //depending on filter or selection, populate the ITR/Template in their corresponding list
                RemoveUnusedWBS(); //remove unused WBS, i.e. WBS without ITRs or Template
                RefreshWBSTagTreeList();
                endUpdateTreeList();
            }
            else
            {
                Initialize_WBSTagITR();
            }
        }

        private void beginUpdateTreeList()
        {
            treeListWBSTag.BeginUpdate();
            treeListInspected.BeginUpdate();
            treeListPending.BeginUpdate();
            treeListApproved.BeginUpdate();
            treeListCompleted.BeginUpdate();
            treeListSaved.BeginUpdate();
        }

        private void endUpdateTreeList()
        {
            treeListWBSTag.EndUpdate();
            treeListInspected.EndUpdate();
            treeListPending.EndUpdate();
            treeListApproved.EndUpdate();
            treeListCompleted.EndUpdate();
            treeListSaved.EndUpdate();
        }

        /// <summary>
        /// Refresh user filtered WBS/Tag from textbox
        /// </summary>
        private void FilteringWBSTags()
        {
            _filterWBSTagDisplay.Clear();
            //DisableTagWBS();

            _filterWBSTagDisplay.AddRange(_allWBSTagDisplay);
            ////retrieve tag and wbs
            //if (!txtSearchTag.Text.Equals(string.Empty))
            //{
            //    foreach(wbsTagDisplay wbsTagDisplay in _allWBSTagDisplay)
            //    {
            //        if(wbsTagDisplay.wbsTagDisplayName.ToLower().Contains(txtSearchTag.Text.ToLower().Trim()))
            //        {
            //            if(!_filterWBSTagDisplay.Any(obj => obj.wbsTagDisplayGuid == wbsTagDisplay.wbsTagDisplayGuid))
            //            {
            //                _filterWBSTagDisplay.Add(wbsTagDisplay);
            //                List<wbsTagDisplay> wbsTagChildrens = Common.AllWBSTagChildren(wbsTagDisplay.wbsTagDisplayGuid, _allWBSTagDisplay).ToList();
            //                foreach (wbsTagDisplay wbsTagChildren in wbsTagChildrens)
            //                {
            //                    if (!_filterWBSTagDisplay.Any(obj => obj.wbsTagDisplayGuid == wbsTagChildren.wbsTagDisplayGuid))
            //                    {
            //                        _filterWBSTagDisplay.Add(wbsTagChildren);
            //                    }  
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (wbsTagDisplay wbsTagDisplay in _allWBSTagDisplay)
            //    {
            //        _filterWBSTagDisplay.Add(wbsTagDisplay);
            //    }
            //}

            RefreshWBSTagTreeList();
        }


        /// <summary>
        /// Refresh Template and ITR from selected Tag
        /// </summary>
        /// <param name="filteredWBSTagDisplay"> filtered WBS/Tag from textbox value</param>
        private void FilterWBSTag_RefreshTemplateITR(List<wbsTagDisplay> filteredWBSTagDisplay, bool forceAddAllWBSTagDisplay = false)
        {
            #region Establishing schedule for query
            var selectedNodes = treeListWBSTag.Selection;
            List<wbsTagDisplay> selectedWBSTags = new List<wbsTagDisplay>();
            //if there are no specific selection, select all
            if(forceAddAllWBSTagDisplay)
            {
                foreach(wbsTagDisplay wbsTagDisplay in _allWBSTagDisplay)
                    Common.AddUniqueWBSTag(selectedWBSTags, wbsTagDisplay);
            }
            else if (treeListWBSTag.Selection.Count == 0)
                selectedWBSTags = filteredWBSTagDisplay;
            else
            {
                foreach (TreeListNode selectedNode in selectedNodes)
                {
                    wbsTagDisplay wbsTagDisplay = (wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(selectedNode);
                    foreach (wbsTagDisplay relatedWbsTagDisplay in Common.GetChildWBSTagDisplays(filteredWBSTagDisplay, wbsTagDisplay))
                        Common.AddUniqueWBSTag(selectedWBSTags, relatedWbsTagDisplay);
                }
            }
            #endregion

            #region Retrieving saved and pending
            _allPending.Clear();
            _allStatus.Clear();

            ConcurrentBag<WorkflowTemplateTagWBS> allStatus = new ConcurrentBag<WorkflowTemplateTagWBS>();
            ConcurrentBag<WorkflowTemplateTagWBS> allPending = new ConcurrentBag<WorkflowTemplateTagWBS>();

            System.Threading.Tasks.Parallel.ForEach(
            selectedWBSTags,
            wbsTag =>
            {
                List<WorkflowTemplateTagWBS> _WBSTagITR = new List<WorkflowTemplateTagWBS>();
                List<WorkflowTemplateTagWBS> _WBSTagTemplate = new List<WorkflowTemplateTagWBS>();
                if (wbsTag.wbsTagDisplayAttachTag != null)
                {
                    _WBSTagITR = _masterITR.Where(obj => obj.wtDisplayAttachTag != null && WBSSearch.SelectedDisciplines.Contains(obj.wtDisplayDiscipline) && obj.wtDisplayAttachTag.GUID == wbsTag.wbsTagDisplayAttachTag.GUID).ToList();
                    foreach (var wbsTAGItem in _WBSTagITR)
                        allStatus.Add(wbsTAGItem);

                    _WBSTagTemplate = _masterTemplate.Where(obj => obj != null).Where(obj => obj.wtDisplayAttachTag != null && WBSSearch.SelectedDisciplines.Contains(obj.wtDisplayDiscipline)).Where(obj => obj.wtDisplayAttachTag.GUID == wbsTag.wbsTagDisplayAttachTag.GUID).Where(template => !_WBSTagITR.Where(savedITR => savedITR.wtDisplayAttachTag != null).Any(savedITR => savedITR.wtTrueTemplateGuid == template.wtTrueTemplateGuid && savedITR.wtDisplayAttachTag.GUID == template.wtDisplayAttachTag.GUID)).ToList();
                    foreach (var wbsTAGItem in _WBSTagTemplate)
                        allPending.Add(wbsTAGItem);
                }
                else if (wbsTag.wbsTagDisplayAttachWBS != null)
                {
                    _WBSTagITR = _masterITR.Where(obj => obj.wtDisplayAttachWBS != null && WBSSearch.SelectedDisciplines.Contains(obj.wtDisplayDiscipline) && obj.wtDisplayAttachWBS.GUID == wbsTag.wbsTagDisplayAttachWBS.GUID).ToList();
                    foreach (var wbsTAGItem in _WBSTagITR)
                        allStatus.Add(wbsTAGItem);

                    _WBSTagTemplate = _masterTemplate.Where(obj => obj != null).Where(obj => obj.wtDisplayAttachWBS != null && WBSSearch.SelectedDisciplines.Contains(obj.wtDisplayDiscipline)).Where(obj => obj.wtDisplayAttachWBS.GUID == wbsTag.wbsTagDisplayAttachWBS.GUID).Where(template => !_WBSTagITR.Where(savedITR => savedITR.wtDisplayAttachWBS != null).Any(savedITR => savedITR.wtTrueTemplateGuid == template.wtTrueTemplateGuid && savedITR.wtDisplayAttachWBS.GUID == template.wtDisplayAttachWBS.GUID)).ToList();
                    foreach (var wbsTAGItem in _WBSTagTemplate)
                        allPending.Add(wbsTAGItem);
                }

            });

            _allStatus.AddRange(allStatus);
            _allPending.AddRange(allPending);

            //foreach (wbsTagDisplay wbsTag in selectedWBSTag)
            //{
            //    List<WorkflowTemplateTagWBS> WBSTagITR = new List<WorkflowTemplateTagWBS>();
            //    List<WorkflowTemplateTagWBS> WBSTagTemplate = new List<WorkflowTemplateTagWBS>();
            //    List<WorkflowTemplateTagWBS> savedITRs = new List<WorkflowTemplateTagWBS>();
            //    if (wbsTag.wbsTagDisplayAttachTag != null)
            //    {
            //        WBSTagITR = _masterITR.Where(obj => obj.wtDisplayAttachTag != null && obj.wtDisplayAttachTag.GUID == wbsTag.wbsTagDisplayAttachTag.GUID).ToList();
            //        _allStatus.AddRange(WBSTagITR);

            //        WBSTagTemplate = _masterTemplate.Where(obj => obj.wtDisplayAttachTag != null).Where(obj => obj.wtDisplayAttachTag.GUID == wbsTag.wbsTagDisplayAttachTag.GUID).Where(template => !WBSTagITR.Where(savedITR => savedITR.wtDisplayAttachTag != null).Any(savedITR => savedITR.wtTrueTemplateGuid == template.wtTrueTemplateGuid && savedITR.wtDisplayAttachTag.GUID == template.wtDisplayAttachTag.GUID)).ToList();

            //        if (WBSTagTemplate.Count > 0)
            //            _allPending.AddRange(WBSTagTemplate);
            //    }
            //    else if (wbsTag.wbsTagDisplayAttachWBS != null)
            //    {
            //        WBSTagITR = _masterITR.Where(obj => obj.wtDisplayAttachWBS != null && obj.wtDisplayAttachWBS.GUID == wbsTag.wbsTagDisplayAttachWBS.GUID).ToList();
            //        _allStatus.AddRange(WBSTagITR);

            //        WBSTagTemplate = _masterTemplate.Where(obj => obj.wtDisplayAttachWBS != null).Where(obj => obj.wtDisplayAttachWBS.GUID == wbsTag.wbsTagDisplayAttachWBS.GUID).Where(template => !WBSTagITR.Where(savedITR => savedITR.wtDisplayAttachWBS != null).Any(savedITR => savedITR.wtTrueTemplateGuid == template.wtTrueTemplateGuid && savedITR.wtDisplayAttachWBS.GUID == template.wtDisplayAttachWBS.GUID)).ToList();

            //        if (WBSTagTemplate.Count > 0)
            //            _allPending.AddRange(WBSTagTemplate);
            //    }
            //}
            #endregion

            #region Filtering
            ////perform filtering to remove saved ITR from assigned template
            //List<WorkflowTemplateTagWBS> RemoveTemplates = new List<WorkflowTemplateTagWBS>();
            //foreach (WorkflowTemplateTagWBS pending in _allPending)
            //{
            //    bool findSaved = false;
            //    if (pending.wtDisplayAttachTag != null) //searches by attached tag
            //        findSaved = _allStatus.Any(obj => obj.wtTrueTemplateGuid == pending.wtTrueTemplateGuid && obj.wtDisplayAttachTag != null && obj.wtDisplayAttachTag.GUID == pending.wtDisplayAttachTag.GUID);
            //    else if (pending.wtDisplayAttachWBS != null)
            //        findSaved = _allStatus.Any(obj => obj.wtTrueTemplateGuid == pending.wtTrueTemplateGuid && obj.wtDisplayAttachWBS != null && obj.wtDisplayAttachWBS.GUID == pending.wtDisplayAttachWBS.GUID);

            //    if (findSaved)
            //        RemoveTemplates.Add(pending);
            //}

            ////repopulate the pending list with filtered results
            //foreach (WorkflowTemplateTagWBS removeTemplate in RemoveTemplates)
            //{
            //    _allPending.Remove(removeTemplate);
            //}
            #endregion

            PopulateWorkflow(_masterWorkflow); //populate again because pending was cleared
            //DisableTagWBSTemplate(false); //disable the templates
            AllocateITRsToSpecificTabs(_allStatus); //allocate the ITRs to specific tabs in the kanban board
        }

        /// <summary>
        /// Move one template to ITR saved or inspected
        /// </summary>
        private void AddOneTemplateToITR(WorkflowTemplateTagWBS TemplateTagWBS, dsITR_MAIN.ITR_MAINRow drITR, bool inspected, bool closed, bool commented = false)
        {
            splashScreenManager2.ShowWaitForm();

            //assign ITR guid to template because template has become a new ITR
            if (drITR != null)
                TemplateTagWBS.wtITRGuid = drITR.GUID;

            _masterITR.Add(TemplateTagWBS);
            _allStatus.Add(TemplateTagWBS);
            _allPending.Remove(TemplateTagWBS);

            if (inspected)
            {
                if (!commented)
                    TemplateTagWBS.wtDisplayImageIndex = 3;
                else
                    TemplateTagWBS.wtDisplayImageIndex = 6;

                _allInspected.Add(TemplateTagWBS);
            }
            else if (closed)
            {
                if (!commented)
                    TemplateTagWBS.wtDisplayImageIndex = 3;
                else
                    TemplateTagWBS.wtDisplayImageIndex = 6;

                _allClosed.Add(TemplateTagWBS);
            }
            else
                _allSaved.Add(TemplateTagWBS);

            Guid? calculateStatusGuid = TemplateTagWBS.wtDisplayAttachTag != null ? TemplateTagWBS.wtDisplayAttachTag.GUID : TemplateTagWBS.wtDisplayAttachWBS != null ? TemplateTagWBS.wtDisplayAttachWBS.GUID : (Guid?)null;
            if (calculateStatusGuid != null)
                DisableTagWBS((Guid)calculateStatusGuid);

            RefreshWBSTagTreeList();
            //DisableTagWBSTemplate(false);
            CountAllStatusesTab();
            //CountWBSTagCompletion();
            RefreshAllStatusTreeList();
            //deselectAllWBSTag();

            splashScreenManager2.CloseWaitForm();
        }

        /// <summary>
        /// Remove one iTR from status list and add it into pending template list
        /// </summary>
        private void RemoveOneITR(WorkflowTemplateTagWBS iTR)
        {
            _masterITR.Remove(iTR);
            WorkflowTemplateTagWBS findTemplate = _masterTemplate.FirstOrDefault(obj => obj.wtDisplayGuid == iTR.wtDisplayGuid);
            if (findTemplate != null)
            {
                findTemplate.wtDisplayImageIndex = 1;
                findTemplate.wtITRGuid = Guid.Empty;
            }

            RemoveITRFromStatusList(iTR, true);

            Guid? calculateStatusGuid = iTR.wtDisplayAttachTag != null ? iTR.wtDisplayAttachTag.GUID : iTR.wtDisplayAttachWBS != null ? iTR.wtDisplayAttachWBS.GUID : (Guid?)null;
            if (calculateStatusGuid != null)
                DisableTagWBS((Guid)calculateStatusGuid);

            RefreshWBSTagTreeList();
            //FilterWBSTag_RefreshTemplateITR(_filterWBSTagDisplay);
            FilterWBSTag_RefreshTemplateITR(_allWBSTagDisplay);
        }

        /// <summary>
        /// Refresh the status of one ITR
        /// </summary>
        private void RefreshOneStatus(WorkflowTemplateTagWBS iTR, dsITR_MAIN.ITR_MAINRow drITR)
        {
            splashScreenManager2.ShowWaitForm();
            RemoveITRFromStatusList(iTR, false);
            SetITRDisplayIndexAndAddToList(drITR.GUID, iTR);

            //disabling if rejected - rarely used, comment out to speed up
            //Guid? calculateStatusGuid = iTR.wtDisplayAttachTag != null ? iTR.wtDisplayAttachTag.GUID : iTR.wtDisplayAttachWBS != null ? iTR.wtDisplayAttachWBS.GUID : (Guid?)null;
            //if(calculateStatusGuid != null)
            //    DisableTagWBS((Guid)calculateStatusGuid);

            RefreshWBSTagTreeList();
            //EnableAllWBSTagTemplate();
            //DisableTagWBSTemplate(true);
            CountAllStatusesTab();
            //CountWBSTagCompletion();
            RefreshAllStatusTreeList();
            splashScreenManager2.CloseWaitForm();
        }


        /// <summary>
        /// Remove the iTR from all status list
        /// </summary>
        private void RemoveITRFromStatusList(WorkflowTemplateTagWBS iTR, bool sendToPending)
        {
            if (sendToPending)
                _allStatus.Remove(iTR);

            _allSaved.Remove(iTR);
            _allInspected.Remove(iTR);
            _allApproved.Remove(iTR);
            _allCompleted.Remove(iTR);
            _allClosed.Remove(iTR);
        }

        /// <summary>
        /// Split all saved ITR to their relevant statuses
        /// </summary>
        private void AllocateITRsToSpecificTabs(List<WorkflowTemplateTagWBS> allStatuses)
        {
            _allSaved.Clear();
            _allInspected.Clear();
            _allApproved.Clear();
            _allCompleted.Clear();
            _allClosed.Clear();

            List<Tuple<Guid, Guid, WorkflowTemplateTagWBS>> wBSITRs = new List<Tuple<Guid, Guid, WorkflowTemplateTagWBS>>();
            List<Tuple<Guid, Guid, WorkflowTemplateTagWBS>> tagITRs = new List<Tuple<Guid, Guid, WorkflowTemplateTagWBS>>();
            foreach (WorkflowTemplateTagWBS TempTagWBS in allStatuses)
            {
                Guid tagWBSGuid;
                dsITR_MAIN.ITR_MAINRow drITR = null;

                //Attemp to retrieve the iTR from established repository
                if (TempTagWBS.wtDisplayAttachTag != null)
                {
                    tagWBSGuid = TempTagWBS.wtDisplayAttachTag.GUID;
                    //IEnumerable<dsITR_MAIN.ITR_MAINRow> drITRs = _dtITRMaster.Where(obj => (!obj.IsTAG_GUIDNull() && obj.TAG_GUID == tagWBSGuid) && obj.TEMPLATE_GUID == TempTagWBS.wtTrueTemplateGuid);
                    //foreach (dsITR_MAIN.ITR_MAINRow drITR in drITRs)
                    //{
                    //    tagITRs.Add(new Tuple<Guid, Guid, WorkflowTemplateTagWBS>(TempTagWBS.wtDisplayAttachTag.tagParentGuid, drITR.GUID, TempTagWBS));
                    //}
                    drITR = _dtITRMaster.FirstOrDefault(obj => (!obj.IsTAG_GUIDNull() && obj.TAG_GUID == tagWBSGuid) && obj.TEMPLATE_GUID == TempTagWBS.wtTrueTemplateGuid && obj.GUID == TempTagWBS.wtITRGuid);
                    if(drITR != null)
                        tagITRs.Add(new Tuple<Guid, Guid, WorkflowTemplateTagWBS>(TempTagWBS.wtDisplayAttachTag.tagParentGuid, drITR.GUID, TempTagWBS));
                }
                else
                {
                    tagWBSGuid = TempTagWBS.wtDisplayAttachWBS.GUID;
                    //IEnumerable<dsITR_MAIN.ITR_MAINRow> drITRs = _dtITRMaster.Where(obj => (!obj.IsWBS_GUIDNull() && obj.WBS_GUID == tagWBSGuid) && obj.TEMPLATE_GUID == TempTagWBS.wtTrueTemplateGuid);
                    //foreach (dsITR_MAIN.ITR_MAINRow drITR in drITRs)
                    //{
                    //    wBSITRs.Add(new Tuple<Guid, Guid, WorkflowTemplateTagWBS>(tagWBSGuid, drITR.GUID, TempTagWBS));
                    //}
                    drITR = _dtITRMaster.FirstOrDefault(obj => (!obj.IsWBS_GUIDNull() && obj.WBS_GUID == tagWBSGuid) && obj.TEMPLATE_GUID == TempTagWBS.wtTrueTemplateGuid);
                    if (drITR != null)
                        wBSITRs.Add(new Tuple<Guid, Guid, WorkflowTemplateTagWBS>(tagWBSGuid, drITR.GUID, TempTagWBS));
                }
            }

            //query tag status by attached WBS guids
            HashSet<Guid> tagWBSParentGuids = new HashSet<Guid>(tagITRs.Where(x => x.Item1 != Guid.Empty).Select(x => x.Item1));
            dsITR_STATUS.ITR_STATUSDataTable dtTagITRStatus = _daITRStatus.GetTagStatusByWBSs(tagWBSParentGuids.ToList());
            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtTagITRStatusIssue = _daITRStatusIssue.GetTagStatusIssueByWBSs(tagWBSParentGuids.ToList());

            //query tag status on tag number without WBS parent directly
            HashSet<Guid> tagGuids = new HashSet<Guid>(tagITRs.Where(x => x.Item1 == Guid.Empty).Select(x => x.Item2));
            dsITR_STATUS.ITR_STATUSDataTable dtOrphanedTagITRStatus = _daITRStatus.GetStatusByITRs(tagGuids.ToList());
            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtOrphanedTagITRStatusIssue = _daITRStatusIssue.GetITR_StatusIssueByITRs(tagGuids.ToList());

            //query wbs status directly by WBS guids
            HashSet<Guid> wbsGuids = new HashSet<Guid>(wBSITRs.Select(x => x.Item1));
            dsITR_STATUS.ITR_STATUSDataTable dtWBSITRStatus = _daITRStatus.GetWBSStatusByWBSs(wbsGuids.ToList());
            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtWBSITRStatusIssue = _daITRStatusIssue.GetWBSStatusIssueByWBSs(wbsGuids.ToList());

            dsITR_STATUS.ITR_STATUSDataTable dtITRStatus = new dsITR_STATUS.ITR_STATUSDataTable();
            if(dtTagITRStatus != null)
                dtITRStatus.Merge(dtTagITRStatus);
            if(dtOrphanedTagITRStatus != null)
                dtITRStatus.Merge(dtOrphanedTagITRStatus);
            if(dtWBSITRStatus != null)
                dtITRStatus.Merge(dtWBSITRStatus);

            dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtITRStatusIssue = new dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable();
            if (dtTagITRStatusIssue != null)
                dtITRStatusIssue.Merge(dtTagITRStatusIssue);
            if (dtOrphanedTagITRStatusIssue != null)
                dtITRStatusIssue.Merge(dtOrphanedTagITRStatusIssue);
            if (dtWBSITRStatusIssue != null)
                dtITRStatusIssue.Merge(dtWBSITRStatusIssue);

            splashScreenManager3.ShowWaitForm();

            List<Tuple<Guid, Guid, WorkflowTemplateTagWBS>> iTRs = new List<Tuple<Guid, Guid, WorkflowTemplateTagWBS>>();
            iTRs.AddRange(tagITRs);
            iTRs.AddRange(wBSITRs);
            foreach (Tuple<Guid, Guid, WorkflowTemplateTagWBS> tempTagWBS in iTRs)
            {
                splashScreenManager3.SetWaitFormCaption("Allocating ITRs to Tabs...");
                splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, iTRs.Count);

                SetITRDisplayIndexAndAddToList(tempTagWBS.Item2, tempTagWBS.Item3, dtITRStatus, dtITRStatusIssue);

                splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            splashScreenManager3.CloseWaitForm();
            CountAllStatusesTab();
            RefreshAllStatusTreeList();
        }

        /// <summary>
        /// Set the display icon for the iTR and add it to the corresponding list
        /// </summary>
        private void SetITRDisplayIndexAndAddToList(Guid iTRGuid, WorkflowTemplateTagWBS iTR, dsITR_STATUS.ITR_STATUSDataTable dtITRStatus = null, dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtITRStatusIssue = null)
        {
            //modify image index to show rejection
            List<dsITR_STATUS.ITR_STATUSRow> iTRStatusFilteredRows = new List<dsITR_STATUS.ITR_STATUSRow>();
            if (dtITRStatus == null)
            {
                dsITR_STATUS.ITR_STATUSDataTable dtITRStatusFiltered = _daITRStatus.GetStatusByITR(iTRGuid, 2);
                if(dtITRStatusFiltered != null)
                    iTRStatusFilteredRows = dtITRStatusFiltered.AsEnumerable().ToList();
            }
            else
            {
                iTRStatusFilteredRows = dtITRStatus.AsEnumerable().Where(x => x.ITR_MAIN_GUID == iTRGuid).Take(2).ToList();
            }

            if (iTRStatusFilteredRows.Count > 0)
            {
                dsITR_STATUS.ITR_STATUSRow drCurrentStatus = iTRStatusFilteredRows[0];
                if (iTRStatusFilteredRows.Count >= 2)
                {
                    dsITR_STATUS.ITR_STATUSRow drPreviousStatus = iTRStatusFilteredRows[1];
                    dsITR_STATUS_ISSUE.ITR_STATUS_ISSUERow drITRIssue;
                    if (dtITRStatusIssue == null)
                        drITRIssue = _daITRStatusIssue.GetLatestBy(drCurrentStatus.GUID);
                    else
                        drITRIssue = dtITRStatusIssue.AsEnumerable().FirstOrDefault(x => x.ITR_STATUS_GUID == drCurrentStatus.GUID);

                    if ((int)drCurrentStatus.STATUS_NUMBER < (int)drPreviousStatus.STATUS_NUMBER)
                    {
                        if (drITRIssue == null || drITRIssue.COMMENTS == string.Empty)
                            iTR.wtDisplayImageIndex = 5; //rejected without comments
                        else
                            iTR.wtDisplayImageIndex = 7; //rejected with comments
                    }
                    else
                    {
                        if (drITRIssue == null || drITRIssue.COMMENTS == string.Empty)
                            iTR.wtDisplayImageIndex = 4; //progressed without comments
                        else
                            iTR.wtDisplayImageIndex = 6; //progressed with comments
                    }
                }
                else if (iTRStatusFilteredRows.Count >= 1)
                {
                    //it is possible now for newly inspected ITR to have comment because it's been resurfaced by parallel mechanism
                    dsITR_STATUS_ISSUE.ITR_STATUS_ISSUERow drITRIssue;
                    if (dtITRStatusIssue == null)
                        drITRIssue = _daITRStatusIssue.GetLatestBy(drCurrentStatus.GUID);
                    else
                        drITRIssue = dtITRStatusIssue.AsEnumerable().FirstOrDefault(x => x.ITR_STATUS_GUID == drCurrentStatus.GUID);

                    if (drITRIssue == null || drITRIssue.COMMENTS == string.Empty)
                        iTR.wtDisplayImageIndex = 4; //progressed without comments
                    else
                        iTR.wtDisplayImageIndex = 6; //progressed with comments
                }
                else
                {
                    if (drCurrentStatus.STATUS_NUMBER == (int)ITR_Status.Inspected)
                        iTR.wtDisplayImageIndex = 3;
                }

                AddITRToStatusList(iTR, (int)drCurrentStatus.STATUS_NUMBER);
            }
            else
            {
                iTR.wtDisplayImageIndex = 1;
                if (!_allSaved.Contains(iTR))
                    _allSaved.Add(iTR);
                if (!_masterITR.Contains(iTR))
                    _masterITR.Add(iTR);
            }
        }

        /// <summary>
        /// Add the ITR to corresponding status list
        /// </summary>
        private void AddITRToStatusList(WorkflowTemplateTagWBS iTR, int statusNumber)
        {
            var findTemplate = _dtTemplates.FirstOrDefault(x => x.GUID == iTR.wtTrueTemplateGuid);
            if (findTemplate != null)
                iTR.wtDisplayInternalRevision =  Common.GetTemplateInternalRevision(findTemplate.NAME);

            //shove ITR to corresponding status lists
            if (statusNumber <= -1) //-1 means rejected back into saved
                _allSaved.Add(iTR);
            else if (statusNumber == (int)ITR_Status.Inspected)
                _allInspected.Add(iTR);
            else if (statusNumber == (int)ITR_Status.Approved)
                _allApproved.Add(iTR);
            else if (statusNumber == (int)ITR_Status.Completed)
                _allCompleted.Add(iTR);
            else if (statusNumber >= (int)ITR_Status.Closed)
                _allClosed.Add(iTR);
        }

        /// <summary>
        /// Populate the tab with status count
        /// </summary>
        private void CountAllStatusesTab()
        {
            //write status count
            int countStatus = _allPending.Count(obj => obj.wtDisplayAttachTemplate != null);
            xtraTabPagePending.Text = "Pending [" + countStatus + "]";

            countStatus = _allSaved.Count(obj => obj.wtDisplayAttachTemplate != null);
            xtraTabPageSaved.Text = "Saved [" + countStatus + "]";

            countStatus = _allInspected.Count(obj => obj.wtDisplayAttachTemplate != null);
            xtraTabPageInspected.Text = "Inspected [" + countStatus + "]";

            countStatus = _allApproved.Count(obj => obj.wtDisplayAttachTemplate != null);
            xtraTabPageApproved.Text = "Approved [" + countStatus + "]";

            countStatus = _allCompleted.Count(obj => obj.wtDisplayAttachTemplate != null);
            xtraTabPageCompleted.Text = "Completed [" + countStatus + "]";

            countStatus = _allClosed.Count(obj => obj.wtDisplayAttachTemplate != null);
            xtraTabPageClosed.Text = "Closed [" + countStatus + "]";
        }

        /// <summary>
        /// Populate the tab with status count
        /// </summary>
        private void resetTreeLists()
        {
            xtraTabPagePending.Text = "Pending [" + 0 + "]";
            xtraTabPageSaved.Text = "Saved [" + 0 + "]";
            xtraTabPageInspected.Text = "Inspected [" + 0 + "]";
            xtraTabPageApproved.Text = "Approved [" + 0 + "]";
            xtraTabPageCompleted.Text = "Completed [" + 0 + "]";
            xtraTabPageClosed.Text = "Closed [" + 0 + "]";

            treeListWBSTag.ClearNodes();
            treeListPending.ClearNodes();
            treeListSaved.ClearNodes();
            treeListInspected.ClearNodes();
            treeListApproved.ClearNodes();
            treeListCompleted.ClearNodes();
            treeListClosed.ClearNodes();
        }

        /// <summary>
        /// Count the completion percentage of workpack
        /// </summary>
        private void CountWBSTagCompletion()
        {
            //use double in case percentages are needed later
            double totalITRCount = 0;
            double completedITRCount = 0;

            foreach (wbsTagDisplay wtDisplay in _allWBSTagDisplay)
            {
                //only display status for WBS with empty description attached to root
                if (wtDisplay.wbsTagDisplayAttachWBS != null && wtDisplay.wbsTagDisplayParentGuid == Guid.Empty && wtDisplay.wbsTagDisplayDescription == string.Empty)
                {
                    totalITRCount = RecurseCountChildrenStatusITR(wtDisplay.wbsTagDisplayGuid, _allStatus) + RecurseCountChildrenStatusITR(wtDisplay.wbsTagDisplayGuid, _allPending);
                    completedITRCount = RecurseCountChildrenStatusITR(wtDisplay.wbsTagDisplayGuid, _allClosed);

                    wtDisplay.wbsTagDisplayDescription = completedITRCount.ToString("0") + " of " + totalITRCount.ToString("0") + " Completed";
                }
            }
        }

        /// <summary>
        /// Count number of ITR attached to GUID per status list
        /// </summary>
        private double RecurseCountChildrenStatusITR(Guid parentWBSTagGuid, List<WorkflowTemplateTagWBS> statusList)
        {
            double completedITRCount = 0;
            List<wbsTagDisplay> listChildrenWBSTag = _allWBSTagDisplay.Where(obj => obj.wbsTagDisplayParentGuid == parentWBSTagGuid).ToList();

            foreach (wbsTagDisplay childrenWBSTag in listChildrenWBSTag)
            {
                completedITRCount += statusList.Where(obj => obj.wtDisplayAttachmentName == childrenWBSTag.wbsTagDisplayName).Count(); //count own completed ITR
                completedITRCount += RecurseCountChildrenStatusITR(childrenWBSTag.wbsTagDisplayGuid, statusList); //recursively find WBS/Tag children completed ITR
            }

            return completedITRCount;
        }

        /// <summary>
        /// Populate all display lists with workflow
        /// </summary>
        private void PopulateWorkflow(List<Workflow> Workflows)
        {
            foreach (Workflow workflow in Workflows)
            {
                _allPending.Add(new WorkflowTemplateTagWBS(workflow));
            }
        }

        /// <summary>
        /// Refreshes all the status tree list to reflected updated bind data
        /// </summary>
        private void RefreshAllStatusTreeList()
        {
            TreeList focusedTreeList = GetFocusedTreeList();
            TreeListNode saveNode = focusedTreeList.FocusedNode;
            treeListPending.ForceInitialize();
            treeListPending.RefreshDataSource();
            treeListPending.ExpandAll();
            treeListSaved.ForceInitialize();
            treeListSaved.RefreshDataSource();
            treeListSaved.ExpandAll();
            treeListInspected.ForceInitialize();
            treeListInspected.RefreshDataSource();
            treeListInspected.ExpandAll();
            treeListApproved.ForceInitialize();
            treeListApproved.RefreshDataSource();
            treeListApproved.ExpandAll();
            treeListCompleted.ForceInitialize();
            treeListCompleted.RefreshDataSource();
            treeListCompleted.ExpandAll();
            treeListClosed.ForceInitialize();
            treeListClosed.RefreshDataSource();
            treeListClosed.ExpandAll();

            if (saveNode != null)
                focusedTreeList.SetFocusedNode(saveNode);
            //focusedTreeList.FocusedNode = saveNode;
        }

        #endregion

        #region Event
        private void btnBulkAttach_Click(object sender, EventArgs e)
        {
            splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetLargeFormat, 1000);
            splashScreenManager1.SetWaitFormCaption("Loading all ITRs for bulk attaching...");
            searchWBSTag();
            splashScreenManager1.CloseWaitForm();
            //_filterWBSTagDisplay = _allWBSTagDisplay;
            //FilterWBSTag_RefreshTemplateITR(_filterWBSTagDisplay);
            BulkAttachDocument();
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            flyoutPanel.HidePopup();
            TreeList focusedTreeList = GetFocusedTreeList();
            if (focusedTreeList == null)
                return;

            WorkflowTemplateTagWBS TemplateTagWBS = (WorkflowTemplateTagWBS)focusedTreeList.GetDataRecordByNode(focusedTreeList.FocusedNode);

            if (TemplateTagWBS.wtEnabled)
                AttachDocument(TemplateTagWBS, false);
            else if (System_Environment.HasPrivilege(PrivilegeTypeID.SkipStages))
            {
                if (MessageBox.Show("High priority item exists, are you sure you want to skip it?", "Lower Stage Exists", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    AttachDocument(TemplateTagWBS, false);
            }
            else
                Common.Warn("Higher priority item exists\n\nPlease complete them before attaching onto this");
        }

        private void btnAttachPDF_Click(object sender, EventArgs e)
        {
            flyoutPanel.HidePopup();
            TreeList focusedTreeList = GetFocusedTreeList();
            if (focusedTreeList == null)
                return;

            WorkflowTemplateTagWBS TemplateTagWBS = (WorkflowTemplateTagWBS)focusedTreeList.GetDataRecordByNode(focusedTreeList.FocusedNode);

            if (TemplateTagWBS.wtEnabled)
                AttachDocument(TemplateTagWBS, true);
            else if (System_Environment.HasPrivilege(PrivilegeTypeID.SkipStages))
            {
                if (MessageBox.Show("High priority item exists, are you sure you want to skip it?", "Lower Stage Exists", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    AttachDocument(TemplateTagWBS, true);
            }
            else
                Common.Warn("Higher priority item exists\n\nPlease complete them before attaching onto this");
        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            treeListWBSTag.CollapseAll();
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            treeListWBSTag.ExpandAll();
        }

        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            deselectAllWBSTag();
            FilterWBSTag_RefreshTemplateITR(_filterWBSTagDisplay);
        }

        private void btnRemoveEntry_Click(object sender, EventArgs e)
        {
            SelectedWBSTags.Clear();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            GetCheckedNode OperationNodeCheck = new GetCheckedNode(treeListWBSTag);
            treeListWBSTag.NodesIterator.DoLocalOperation(OperationNodeCheck, treeListWBSTag.Nodes);
            SelectedWBSTags = OperationNodeCheck.GetCheckedWBSTags();
            //foreach (TreeListNode node in treeListWBSTag.Nodes)
            //{
            //    if (node.Checked)
            //        SelectedWBSTags.Add((wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(node));
            //}

            if (SelectedWBSTags.Count > 0)
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            else
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void dropDownExport_Click(object sender, EventArgs e)
        {
            if (dropDownExport.DropDownArrowStyle == DevExpress.XtraEditors.DropDownArrowStyle.Show)
            {
                dropDownExport.DropDownControl = popupControlContainer1;
                return;
            }
            else
            {
                dropDownExport.DropDownControl = null;
                btnExport_Click(null, null);
            }
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            //txtSearchTag.Text = string.Empty;
            //FilteringWBSTags();

            treeListWBSTag.Selection.Clear();
            FilterWBSTag_RefreshTemplateITR(_filterWBSTagDisplay);
        }

        private void btnExport_Native_Click(object sender, EventArgs e)
        {
            ExportToPDF(true);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportToPDF(false);
        }

        private void ExportToPDF(bool ConvertToNative)
        {
            TreeList focusedTreeList = GetFocusedTreeList();
            if (focusedTreeList == null)
                return;

            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.RootFolder = Environment.SpecialFolder.Desktop;

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                splashScreenManager2.ShowWaitForm();
                splashScreenManager2.SetWaitFormDescription("Exporting ...");

                var selectedNodes = focusedTreeList.Selection;

                splashScreenManager2.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, focusedTreeList.Selection.Count);
                foreach (TreeListNode selectedNode in selectedNodes)
                {
                    WorkflowTemplateTagWBS wtTagWBS = (WorkflowTemplateTagWBS)focusedTreeList.GetDataRecordByNode(selectedNode);
                    if (wtTagWBS.wtDisplayAttachTemplate != null)
                    {
                        frmITR_Main f = new frmITR_Main(wtTagWBS, ConvertToNative, null, _projectGuid, spinEditFontSize.Value);
                        string outputFileName = Common.FormatFileName(f.Text);

                        try
                        {
                            f.GetRichEdit().ExportToPdf(fd.SelectedPath + "\\" + outputFileName + ".pdf");
                        }
                        catch
                        {

                        }
                        splashScreenManager2.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                    }
                }
                splashScreenManager2.CloseWaitForm();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            TreeList focusedTreeList = GetFocusedTreeList();
            if (focusedTreeList == null)
                return;

            var selectedNodes = focusedTreeList.Selection;
            //bool loadITR = true;
            //if(focusedTreeList == treeListPending)
            //    loadITR = false;

            splashScreenManager2.ShowWaitForm();
            splashScreenManager2.SetWaitFormDescription("Printing ...");
            splashScreenManager2.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, focusedTreeList.Selection.Count);

            for (int i = 0; i < selectedNodes.Count; i++)
            {
                WorkflowTemplateTagWBS wtTagWBS = (WorkflowTemplateTagWBS)focusedTreeList.GetDataRecordByNode(selectedNodes[i]);
                using (frmITR_Main f = new frmITR_Main(wtTagWBS, false, null, _projectGuid, spinEditFontSize.Value))
                {
                    f.PrintPreparation();
                    RichEditControl richEdit = f.GetRichEdit();

                    if (i == 0)
                        richEdit.ShowPrintDialog();
                    else
                        richEdit.Print();
                }

                splashScreenManager2.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            splashScreenManager2.CloseWaitForm();
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            TreeList focusedTreeList = GetFocusedTreeList();
            if (focusedTreeList == treeListPending)
                return;

            flyoutPanel.OwnerControl = focusedTreeList;
            if (RefreshComments())
                flyoutPanel.ShowPopup();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenDocument();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            TreeList focusedTreeList = GetFocusedTreeList();
            List<TreeListNode> selectedNodes = focusedTreeList.Selection.ToList();

            foreach(var selectedNode in selectedNodes)
            {
                WorkflowTemplateTagWBS TemplateTagWBS = (WorkflowTemplateTagWBS)focusedTreeList.GetDataRecordByNode(selectedNode);
                frmITR_Main f = new frmITR_Main(TemplateTagWBS, false, update_itr_status, _projectGuid, spinEditFontSize.Value);
                f.TestProgress();
            }
        }

        private void btnComment_Click(object sender, EventArgs e)
        {
            AdapterITR_STATUS_ISSUE daITRIssue = new AdapterITR_STATUS_ISSUE();
            dsITR_STATUS_ISSUE dsITRIssue = new dsITR_STATUS_ISSUE();

            try
            {
                dsITR_STATUS_ISSUE.ITR_STATUS_ISSUERow drITRIssue = dsITRIssue.ITR_STATUS_ISSUE.NewITR_STATUS_ISSUERow();
                daITRIssue.AddComments(_commentingITRStatus, txtComments.Text.Trim(), false);
                txtComments.Text = string.Empty;

                RefreshComments();
            }
            finally
            {
                daITRIssue.Dispose();
                dsITRIssue.Dispose();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void treeList_DoubleClick(object sender, EventArgs e)
        {
            OpenDocument();
        }

        private void treeListWBSTag_BeforeCheckNode(object sender, CheckNodeEventArgs e)
        {
            //don't allow check to be handled from the checkbox, it is handled in the treeListWBSTag_Click instead
            e.CanCheck = false;
        }

        private void treeListWBSTag_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                deselectAllWBSTag();
                FilterWBSTag_RefreshTemplateITR(_filterWBSTagDisplay);
            }
        }

        private void txtSearchTag_EditValueChanged(object sender, EventArgs e)
        {
            //FilteringWBSTags();
        }

        private void treeListWBSTag_MouseUp(object sender, MouseEventArgs e)
        {
            var hitInfo = treeListWBSTag.CalcHitInfo(new Point(e.X, e.Y));
            if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Empty)
                deselectAllWBSTag();
            else if (hitInfo.HitInfoType == HitInfoType.Cell)
            {
                //if form is started with selection dialog mode
                if (_templateGuid == null)
                {
                    flyoutPanel.HidePopup();
                    splashScreenManager2.ShowWaitForm();
                    FilterWBSTag_RefreshTemplateITR(_filterWBSTagDisplay);
                    splashScreenManager2.CloseWaitForm();
                }
                else
                {
                    TreeListNode FocusedNode = treeListWBSTag.FocusedNode;
                    if (FocusedNode != null)
                        FocusedNode.Checked = !FocusedNode.Checked;
                }
            }
        }

        private void treeList_MouseClick(object sender, MouseEventArgs e)
        {
            TreeList focusedTreeList = GetFocusedTreeList();
            var hitInfo = focusedTreeList.CalcHitInfo(new Point(e.X, e.Y));

            if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Cell)
            {
                WorkflowTemplateTagWBS hitWorkflowTempTagWBS = (WorkflowTemplateTagWBS)focusedTreeList.GetDataRecordByNode(hitInfo.Node);
                if (hitWorkflowTempTagWBS.wtDisplayImageIndex == 6 || hitWorkflowTempTagWBS.wtDisplayImageIndex == 7)
                {
                    flyoutPanel.OwnerControl = focusedTreeList;
                    RefreshComments();
                    flyoutPanel.ShowPopup();
                }
                else
                    flyoutPanel.HidePopup();
            }
            else
                flyoutPanel.HidePopup();
        }

        private void xtraTabControlStatuses_MouseClick(object sender, MouseEventArgs e)
        {
            TreeList focusedTreeList = GetFocusedTreeList();
            if (focusedTreeList == treeListPending)
            {
                dropDownExport.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Show;
                btnHistory.Enabled = false;
            }
            else
            {
                dropDownExport.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Hide;
                btnHistory.Enabled = true;
            }

            focusedTreeList.BestFitColumns();
        }
        #endregion

        #region EstablishMasterData
        /// <summary>
        /// Establish the master workflow
        /// </summary>
        private void EstablishWorkflow()
        {
            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWorkflow = _daWorkflow.Get();
            if (dtWorkflow != null)
            {
                foreach (dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow in dtWorkflow.Rows)
                {
                    Workflow newWorkflow = new Workflow(drWorkflow.GUID)
                    {
                        workflowName = drWorkflow.NAME,
                        workflowDescription = drWorkflow.IsDESCRIPTIONNull() ? "" : drWorkflow.DESCRIPTION,
                        workflowParentGuid = drWorkflow.PARENTGUID
                    };

                    _masterWorkflow.Add(newWorkflow);
                }
            }
        }

        /// <summary>
        /// Loads all the WBS/Tag assigned to this project and discipline
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="searchDiscipline"></param>
        private void EstablishWBSTag(Guid projectGuid, List<Guid> excludeGuid, Guid? templateGuid = null, Guid? excludeWBSTagGuid = null, List<string> disciplines = null, List<string> subSystemNames = null, List<string> systemNames = null, List<string> areaNames = null, string strTagNumber = "", List<string> stages = null, ProjectLibrary.SearchMode searchMode = ProjectLibrary.SearchMode.Contains)
        {
            _allWBSTagDisplay.Clear();

            dsTAG.TAGDisciplineDataTable dtTag;
            dsWBS.WBSDataTable dtWBS;
            dtWBS = _daWBS.GetByProject(_projectGuid);
            if (templateGuid == null)
            {
                if(strTagNumber != string.Empty || (subSystemNames != null && subSystemNames.Count > 0) || (systemNames != null && systemNames.Count > 0) || (areaNames != null && areaNames.Count > 0) || (disciplines != null && disciplines.Count > 0) || (stages != null && stages.Count > 0))
                    dtTag = _daTag.GetByProjectWBSName(projectGuid, subSystemNames, systemNames, areaNames, strTagNumber, stages, searchMode);
                else
                    dtTag = _daTag.GetByProjectIncludeDiscipline(projectGuid);
            }
            else
            {
                dtTag = _daTag.GetByProjectTemplate(projectGuid, (Guid)templateGuid, (Guid)excludeWBSTagGuid, excludeGuid);
            }

            List<wbsTagDisplay> tempChildrens = new List<wbsTagDisplay>();

            if (dtTag != null)
            {
                foreach (dsTAG.TAGDisciplineRow drTag in dtTag.Rows)
                {
                    tempChildrens.Add(new wbsTagDisplay(new Tag(drTag.GUID)
                    {
                        tagNumber = drTag.NUMBER,
                        tagDescription = drTag.DESCRIPTION,
                        tagParentGuid = drTag.PARENTGUID,
                        tagScheduleGuid = drTag.SCHEDULEGUID,
                        tagDiscipline = drTag.DISCIPLINE
                    }));
                }
            }

            if (dtWBS != null)
            {
                foreach (dsWBS.WBSRow drWBS in dtWBS.Rows)
                {
                    //because WBS consist of multiple levels, this method is not reliable so it's commented out
                    //if (_allWBSTagDisplay.Any(x => x.wbsTagDisplayParentGuid == drWBS.GUID))
                    //{
                    _allWBSStore.Add(new wbsTagDisplay(new WBS(drWBS.GUID)
                    {
                        wbsName = drWBS.NAME,
                        wbsDescription = drWBS.IsDESCRIPTIONNull() ? "" : drWBS.DESCRIPTION,
                        wbsParentGuid = drWBS.PARENTGUID,
                        wbsScheduleGuid = drWBS.SCHEDULEGUID
                    }));
                    //}
                }
            }

            List<WorkflowTemplateTagWBS> wbsTemplates = _allStatus.Where(x => x.wtDisplayAttachWBS != null).ToList();
            List<Guid> WBSITRTemplates = new List<Guid>();
            List<Guid> masterITRFilterGuids;
            List<Guid> masterTemplateFilterGuids;
            if (stages == null || stages.Count == 0)
            {
                masterITRFilterGuids = _masterITR.Where(x => x.wtDisplayAttachWBS != null).Select(x => x.wtDisplayAttachWBS.GUID).ToList();
                masterTemplateFilterGuids = _masterTemplate.Where(x => x.wtDisplayAttachWBS != null).Select(x => x.wtDisplayAttachWBS.GUID).ToList();
            }
            else
            {
                masterITRFilterGuids = _masterITR.Where(x => x.wtDisplayAttachWorkflow != null && stages.Any(y => y.ToUpper() == x.wtDisplayAttachWorkflow.workflowName.ToUpper())).Where(x => x.wtDisplayAttachWBS != null).Select(x => x.wtDisplayAttachWBS.GUID).ToList();
                masterTemplateFilterGuids = _masterTemplate.Where(x => x.wtDisplayAttachWorkflow != null && stages.Any(y => y.ToUpper() == x.wtDisplayAttachWorkflow.workflowName.ToUpper())).Where(x => x.wtDisplayAttachWBS != null).Select(x => x.wtDisplayAttachWBS.GUID).ToList();
            }

            WBSITRTemplates.AddRange(masterITRFilterGuids);
            WBSITRTemplates.AddRange(masterTemplateFilterGuids);

            //add WBS which have ITR assigned
            foreach(wbsTagDisplay tempWBS in _allWBSStore)
            {
                if (WBSITRTemplates.Any(x => x == tempWBS.wbsTagDisplayGuid))
                {
                    tempChildrens.Add(tempWBS);
                }
            }

            splashScreenManager3.ShowWaitForm();
            splashScreenManager3.SetWaitFormCaption("Loading WBS...");
            splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, tempChildrens.Count);
            //get wbs parents for each tag numbers
            foreach (wbsTagDisplay wbsTagDisplay in tempChildrens)
            {
                //add children wbs/tag regardless
                _allWBSTagDisplay.Add(wbsTagDisplay);
                //gets all the tag/wbs parent and add it
                IEnumerable<wbsTagDisplay> wbsParents = Common.AllParent(wbsTagDisplay.wbsTagDisplayParentGuid, _allWBSStore);
                foreach(wbsTagDisplay wbsParent in wbsParents)
                {
                    if (!_allWBSTagDisplay.Any(x => x.wbsTagDisplayGuid == wbsParent.wbsTagDisplayGuid))
                    {
                        _allWBSTagDisplay.Add(wbsParent);
                    }
                }

                splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            splashScreenManager3.CloseWaitForm();
            DisableTagWBS();
        }



        /// <summary>
        /// Load all the ITR assigned to this project and discipline
        /// </summary>
        private void EstablishProjectITR(Guid projectGuid, List<wbsTagDisplay> WBSTags, List<string> disciplines = null, List<string> subSystemNames = null, List<string> systemNames = null, List<string> areaNames = null, string strTagNumber = "", List<string> stages = null, ProjectLibrary.SearchMode searchMode = ProjectLibrary.SearchMode.Contains)
        {
            _dtITRMaster.Clear();
            dsITR_MAIN.ITR_MAINDataTable dtTagITR = _daITR.GetTagITRByProject(projectGuid, disciplines, subSystemNames, systemNames, areaNames, strTagNumber, stages, searchMode, _isShowCompletedAndClosedITRsOnly);
            dsITR_MAIN.ITR_MAINDataTable dtWBSITR = _daITR.GetByWBSProject(projectGuid, _isShowCompletedAndClosedITRsOnly, disciplines, subSystemNames, systemNames, areaNames, stages, searchMode);

            List<string> guidStr = new List<string>();
            if (dtTagITR != null)
            {
                guidStr = dtTagITR.Select(x => x.GUID.ToString()).ToList();
                foreach(string guid in guidStr)
                {
                    if (guidStr.Where(x => x == guid).Count() > 1)
                        Debug.Print(guid);
                }

                _dtITRMaster.Merge(dtTagITR);
                dtTagITR.Dispose();
            }

            if (dtWBSITR != null)
            {
                _dtITRMaster.Merge(dtWBSITR);
                dtWBSITR.Dispose();
            }

            splashScreenManager3.ShowWaitForm();
            splashScreenManager3.SetWaitFormCaption("Filtering ITRs...");
            //var WBSITRs = _dtITRMaster.Where(x => !x.IsWBS_GUIDNull()).ToList();
            //foreach (var wbsTag in _allWBSStore)
            //{
            //    if (WBSITRs.Any(x => x.WBS_GUID == wbsTag.wbsTagDisplayGuid))
            //        WBSTags.Add(wbsTag);
            //}

            dsITR_MAIN.ITR_MAINDataTable dtITR = new dsITR_MAIN.ITR_MAINDataTable();
            //for internal revision
            _dtTemplates = _daTemplate.GetMetaOnly();
            var wbsITRs = _dtITRMaster.Where(x => !x.IsWBS_GUIDNull()).ToList();
            var tagITRs = _dtITRMaster.Where(x => !x.IsTAG_GUIDNull()).ToList();

            splashScreenManager3.SetWaitFormCaption("Loading ITRs...");
            splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, WBSTags.Count);
            foreach (wbsTagDisplay WBSTag in WBSTags)
            {
                List<dsITR_MAIN.ITR_MAINRow> ITRs;
                if (WBSTag.wbsTagDisplayAttachTag != null)
                    ITRs = tagITRs.Where(x => x.TAG_GUID == WBSTag.wbsTagDisplayGuid).ToList();
                else
                    ITRs = wbsITRs.Where(x => x.WBS_GUID == WBSTag.wbsTagDisplayGuid).ToList();

                if (ITRs.Any())
                {
                    DataTable dt = ITRs.CopyToDataTable();

                    foreach (DataRow dr in dt.Rows)
                    {
                        dsITR_MAIN.ITR_MAINRow drITR = dtITR.NewITR_MAINRow();
                        drITR.ItemArray = dr.ItemArray;
                        //_masterITR.Add(Common.CreateWorkflowTemplateTagWBS(WBSTag, drITR, true));
                        WorkflowTemplateTagWBS workflowTemplateTagWBS = CreateExtractTagWBS(WBSTag, drITR, _dtTemplates, true, _showArea, _masterWorkflow);

                        if(workflowTemplateTagWBS != null)
                        {
                            if(stages == null || stages.Count == 0)
                                _masterITR.Add(workflowTemplateTagWBS);
                            else if(stages.Any(x => x.ToUpper() == workflowTemplateTagWBS.wtStageName.ToUpper()))
                                _masterITR.Add(workflowTemplateTagWBS);
                        }
                    }
                }

                splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            splashScreenManager3.CloseWaitForm();
        }

        public void RemoveUnusedWBS()
        {
            splashScreenManager3.ShowWaitForm();
            splashScreenManager3.SetWaitFormCaption("Removing Empty WBS...");
            //scrape all WBS and Tag
            List<Tag> alliTRTagNumbers = _allStatus.Where(x => x.wtDisplayAttachTag != null).Select(x => x.wtDisplayAttachTag).ToList();
            List<Tag> allTemplateTagNumbers = _allPending.Where(x => x.wtDisplayAttachTag != null).Select(x => x.wtDisplayAttachTag).ToList();
            List<WBS> alliTRWBSNames = _allStatus.Where(x => x.wtDisplayAttachWBS != null).Select(x => x.wtDisplayAttachWBS).ToList();
            List<WBS> allTemplateWBSNames = _allPending.Where(x => x.wtDisplayAttachWBS != null).Select(x => x.wtDisplayAttachWBS).ToList();

            List<Tag> allTagFromITRsAndTemplate = new List<Tag>();

            allTagFromITRsAndTemplate.AddRange(alliTRTagNumbers);
            allTagFromITRsAndTemplate.AddRange(allTemplateTagNumbers);

            List<WBS> allWBSFromITRsAndTemplate = new List<WBS>();
            allWBSFromITRsAndTemplate.AddRange(alliTRWBSNames);
            allWBSFromITRsAndTemplate.AddRange(allTemplateWBSNames);

            List<wbsTagDisplay> scanLastNodeWbsTagDisplays = new List<wbsTagDisplay>();

            List<wbsTagDisplay> removeExcludeWbsTagDisplay = new List<wbsTagDisplay>();
            //get wbs parents for each tag numbers
            foreach (wbsTagDisplay wbsTagDisplay in _allWBSTagDisplay)
            {
                //skip if this is not the lowest node level, either tag or WBS
                if (_allWBSTagDisplay.Any(x => x.wbsTagDisplayParentGuid == wbsTagDisplay.wbsTagDisplayGuid))
                    continue;

                bool isITROrTemplateExistsForWBS = allWBSFromITRsAndTemplate.Any(y => y.GUID == wbsTagDisplay.wbsTagDisplayGuid);
                bool isITROrTemplateExistsForTag = allTagFromITRsAndTemplate.Any(y => y.GUID == wbsTagDisplay.wbsTagDisplayGuid);

                if (isITROrTemplateExistsForTag || isITROrTemplateExistsForWBS)
                {
                    removeExcludeWbsTagDisplay.Add(wbsTagDisplay);
                    //gets all the tag/wbs parent
                    IEnumerable<wbsTagDisplay> wbsParents = Common.AllParent(wbsTagDisplay.wbsTagDisplayParentGuid, _allWBSStore);
                    foreach (wbsTagDisplay wbsParent in wbsParents)
                    {
                        //skip when it's already added to exclusion
                        if (removeExcludeWbsTagDisplay.Any(x => x.wbsTagDisplayGuid == wbsParent.wbsTagDisplayGuid))
                            continue;

                        removeExcludeWbsTagDisplay.Add(wbsParent);
                    }
                }
            }

            List<wbsTagDisplay> removeWBSDisplays = new List<wbsTagDisplay>();
            foreach (wbsTagDisplay wbsTagDisplay in _allWBSTagDisplay)
            {
                if (removeExcludeWbsTagDisplay.Any(x => x.wbsTagDisplayGuid == wbsTagDisplay.wbsTagDisplayGuid))
                    continue;

                removeWBSDisplays.Add(wbsTagDisplay);
            }

            foreach (wbsTagDisplay removeWBSTagDisplay in removeWBSDisplays)
            {
                _allWBSTagDisplay.Remove(removeWBSTagDisplay);
            }

            splashScreenManager3.CloseWaitForm();
        }

        /// <summary>
        /// Populate the wbsTagDisplay class based on parameters
        /// </summary>
        /// <param name="drITRorTemplate">Must be either ITR or Template</param>
        public static WorkflowTemplateTagWBS CreateExtractTagWBS(wbsTagDisplay WBSTag, DataRow drITRorTemplate, dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTemplate, bool enabled, bool showArea, List<Workflow> workflows)
        {
            Template constructTemplate;
            Guid trueTemplateGuid = Guid.Empty;

            dsITR_MAIN.ITR_MAINRow drITR = (dsITR_MAIN.ITR_MAINRow)drITRorTemplate;
            trueTemplateGuid = drITR.TEMPLATE_GUID;

            string areaText = string.Empty;
            if (showArea)
            {
                MemoryStream ms = new MemoryStream(drITR.ITR);

                CustomRichEdit customRichEdit1 = new CustomRichEdit();
                customRichEdit1.Document.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);

                if (customRichEdit1.Document.Tables.Count > 0)
                {
                    Table headerTable = customRichEdit1.Document.Tables[0];
                    foreach (DevExpress.XtraRichEdit.API.Native.TableRow tRow in headerTable.Rows)
                    {
                        foreach (TableCell tCell in tRow.Cells)
                        {
                            string headerText = customRichEdit1.Document.GetText(tCell.ContentRange);
                            if (headerText.ToUpper().Contains("AREA") || headerText.ToUpper().Contains("LOCATION"))
                            {
                                areaText = customRichEdit1.Document.GetText(tCell.Next.ContentRange);
                                break;
                            }
                        }

                        if (areaText != string.Empty)
                            break;
                    }
                }
            }

            var findTemplate = dtTemplate.FirstOrDefault(x => x.GUID == trueTemplateGuid);
            if(findTemplate == null)
            {
                return null;
            }

            string workflowName = string.Empty;
            Workflow findWorkflow = workflows.FirstOrDefault(x => x.GUID == findTemplate.WORKFLOWGUID);
            if (findWorkflow != null)
                workflowName = findWorkflow.workflowName;

            constructTemplate = new Template(Guid.NewGuid())
            {
                templateName = drITR.NAME,
                templateDescription = drITR.DESCRIPTION, 
                templateWorkFlow = new ValuePair(workflowName, Guid.Empty), //workflow Guid is needed when deletion happens and iTR goes back into pending list
                templateRevision = drITR.REVISION,
                templateDiscipline = drITR.DISCIPLINE,
                templateQRSupport = findTemplate != null ? findTemplate.QRSUPPORT : false,
                templateSkipApproved = findTemplate != null ? findTemplate.SKIPAPPROVED : false
            };

            WorkflowTemplateTagWBS newWorkTemplateTagWBS = new WorkflowTemplateTagWBS(constructTemplate)
            {
                wtDisplayAttachmentName = WBSTag.wbsTagDisplayName,
                wtDisplayAttachmentDescription = WBSTag.wbsTagDisplayDescription,
                wtDisplayAttachTag = WBSTag.wbsTagDisplayAttachTag,
                wtDisplayAttachWBS = WBSTag.wbsTagDisplayAttachWBS,
                wtTrueTemplateGuid = trueTemplateGuid, //treeList doesn't allow duplicate guid, hence a new Guid was created and the actual guid is stored here
                wtITRGuid = drITR == null ? Guid.Empty : drITR.GUID,
                wtEnabled = enabled,
                wtDisplayITRMeta = areaText,
                wtTaskNumber = GetStringHash(string.Concat(WBSTag.wbsTagDisplayName, constructTemplate.templateName)),
                wtCreatedDate = drITR == null ? (DateTime?)null : drITR.CREATED
            };

            return newWorkTemplateTagWBS;
        }

        /// <summary>
        /// Load all the template assigned to this project and discipline
        /// </summary>
        private void EstablishProjectTemplate(Guid projectGuid, List<wbsTagDisplay> WBSTags, List<string> stages = null)
        {
            _dtTemplateMaster.Clear();
            dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable dtTagTemplate;
            dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable dtWBSTemplate;

            dtTagTemplate = _daTemplate.GetByTagProject(projectGuid);
            dtWBSTemplate = _daTemplate.GetByWBSProject(projectGuid);

            //Commented out because it's handled in EstablishProjectITR
            //for internal revision
            //_dtTemplates = _daTemplate.Get();

            if (dtTagTemplate != null)
            {
                _dtTemplateMaster.Merge(dtTagTemplate);
                dtTagTemplate.Dispose();
            }

            if (dtWBSTemplate != null)
            {
                _dtTemplateMaster.Merge(dtWBSTemplate);
                dtWBSTemplate.Dispose();
            }

            splashScreenManager3.ShowWaitForm();
            splashScreenManager3.SetWaitFormCaption("Filtering Templates...");
            //var WBSTemplate = _dtTemplateMaster.Where(x => !x.IsWBSTAGGUIDNull()).ToList();
            //foreach(var wbsTag in _allWBSStore)
            //{
            //    if (WBSTemplate.Any(x => x.WBSTAGGUID == wbsTag.wbsTagDisplayGuid))
            //        WBSTags.Add(wbsTag);
            //}

            dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable dtTemplate = new dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable();
            splashScreenManager3.SetWaitFormCaption("Loading Templates...");
            splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, WBSTags.Count);

            var query = (from templates in _dtTemplateMaster.AsEnumerable()
                        group templates by templates.WBSTAGGUID into templateGroup
                        select new { WBSTAGID = templateGroup.Key, templateTable = createWorkflowTemplate(templateGroup.ToList(), dtTemplate) }).ToList();

            //used for anonymous method implementation or else system will not compile
            List<wbsTagDisplay> localWBSTags = WBSTags.ToList();
            ConcurrentBag<WorkflowTemplateTagWBS> workflowTemplateTags = new ConcurrentBag<WorkflowTemplateTagWBS>();
            System.Threading.Tasks.Parallel.ForEach(query,
                queryItem =>
                {
                    wbsTagDisplay WBSTag = localWBSTags.FirstOrDefault(x => x.wbsTagDisplayGuid == queryItem.WBSTAGID);
                    if (WBSTag != null)
                    {
                        foreach (var drTemplate in queryItem.templateTable)
                        {
                            WorkflowTemplateTagWBS workflowTemplateTagWBS = Common.CreateWorkflowTemplateTagWBS(WBSTag, drTemplate, true, _masterWorkflow);

                            if(stages == null || stages.Count == 0|| workflowTemplateTagWBS.wtStageName == null || workflowTemplateTagWBS.wtStageName == string.Empty)
                                workflowTemplateTags.Add(workflowTemplateTagWBS);
                            else if(stages.Any(x => x.ToUpper() == workflowTemplateTagWBS.wtStageName.ToUpper()))
                                workflowTemplateTags.Add(workflowTemplateTagWBS);
                        }
                    }

                    splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }
            );

            _masterTemplate.AddRange(workflowTemplateTags);
            //foreach(var queryItem in query)
            //{
            //    wbsTagDisplay WBSTag = WBSTags.FirstOrDefault(x => x.wbsTagDisplayGuid == queryItem.WBSTAGID);
            //    if (WBSTag != null)
            //    {
            //        foreach (var drTemplate in queryItem.templateTable)
            //        {
            //            WorkflowTemplateTagWBS workflowTemplateTagWBS = Common.CreateWorkflowTemplateTagWBS(WBSTag, drTemplate, true);
            //            _masterTemplate.Add(workflowTemplateTagWBS);
            //        }
            //    }

            //    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            //}

            //foreach (wbsTagDisplay WBSTag in WBSTags)
            //{
            //    var queryWbsTag = query.FirstOrDefault(x => x.WBSTAGID == WBSTag.wbsTagDisplayGuid);

            //    if (queryWbsTag != null)
            //    {
            //        DataTable dt = queryWbsTag.templateRows.CopyToDataTable();

            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGRow drTemplate = dtTemplate.NewTEMPLATE_MAIN_WBSTAGRow();
            //            drTemplate.ItemArray = dr.ItemArray;
            //            _masterTemplate.Add(Common.CreateWorkflowTemplateTagWBS(WBSTag, drTemplate, true));
            //        }
            //    }

            //    splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            //}

            //System.Threading.Tasks.Parallel.ForEach(
            //    WBSTags,
            //    WBSTag =>
            //    {
            //        var queryWbsTag = query.FirstOrDefault(x => x.WBSTAGID == WBSTag.wbsTagDisplayGuid);

            //        if (queryWbsTag != null)
            //        {
            //            foreach (DataRow dr in queryWbsTag.templateTable.Rows)
            //            {
            //                dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGRow drTemplate = dtTemplate.NewTEMPLATE_MAIN_WBSTAGRow();
            //                drTemplate.ItemArray = dr.ItemArray;
            //                WorkflowTemplateTagWBS workflowTemplateTagWBS = Common.CreateWorkflowTemplateTagWBS(WBSTag, drTemplate, true);
            //                _masterTemplate.Add(workflowTemplateTagWBS);
            //            }
            //        }

            //        splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            //    });

            splashScreenManager3.CloseWaitForm();
        }

        private IEnumerable<dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGRow> createWorkflowTemplate(IEnumerable<dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGRow> templateRows, dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGDataTable dtTemplate)
        {
            List<dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGRow> returnWorkflowTemplate = new List<dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGRow>();
            DataTable dt = templateRows.CopyToDataTable();
            foreach (DataRow dr in dt.Rows)
            {
                dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGRow drTemplate = dtTemplate.NewTEMPLATE_MAIN_WBSTAGRow();
                drTemplate.ItemArray = dr.ItemArray;
                returnWorkflowTemplate.Add(drTemplate);
            }

            return returnWorkflowTemplate;
        }
        #endregion

        #region Helper
        /// <summary>
        /// Wrapper for remembering current selection and restore selection upon refreshing
        /// </summary>
        private void RefreshWBSTagTreeList()
        {
            RememberSelectedNodes rememberSelected = new RememberSelectedNodes(treeListWBSTag);
            treeListWBSTag.RefreshDataSource();
            treeListWBSTag.ExpandAll();
            //treeListWBSTag.NodesIterator.DoLocalOperation(rememberSelected, treeListWBSTag.Nodes);
        }

        /// <summary>
        /// Add a list with all childrens WBSTag
        /// </summary>
        private void CollectAllChildrens(List<wbsTagDisplay> allChildrens, List<wbsTagDisplay> allWBSTagDisplay, Guid parentGuid)
        {
            List<wbsTagDisplay> wbsTagChildrens = allWBSTagDisplay.Where(x => x.wbsTagDisplayParentGuid == parentGuid).ToList();
            if (wbsTagChildrens.Count > 0)
            {
                allChildrens.AddRange(wbsTagChildrens);
                foreach (wbsTagDisplay wbsTagChildren in wbsTagChildrens)
                {
                    CollectAllChildrens(allChildrens, allWBSTagDisplay, wbsTagChildren.wbsTagDisplayGuid);
                }
            }
            else
                return;
        }

        /// <summary>
        /// Commented out to speed up loading - Disables WBS/Tag if child template hasn't been completed
        /// </summary>
        private void DisableTagWBS(Guid? wbsTagDisplayGuid = null)
        {
            //retrieve tag/wbs incomplete template count
            List<tagCount> tagIncompleteCounts = _daTemplate.GetTagIncompleteTemplateCount(_projectGuid);
            List<wbsCount> wbsIncompleteCounts = _daTemplate.GetWBSIncompleteTemplateCount(_projectGuid);

            //final child is the last element in the tree
            ConcurrentBag<wbsTagDisplay> finalChildTags = new ConcurrentBag<wbsTagDisplay>();

            if (wbsTagDisplayGuid != null)
            {
                wbsTagDisplay wbsTag = _allWBSTagDisplay.FirstOrDefault(x => x.wbsTagDisplayGuid == wbsTagDisplayGuid);
                if (wbsTag != null)
                {
                    wbsTag.wbsTagDisplaySelfTotalCount = 0;
                    wbsTag.wbsTagDisplayChildTotalCount = 0;
                    wbsTag.wbsTagDisplayChildInspectedCount = 0;
                    wbsTag.wbsTagDisplayChildApprovedCount = 0;
                    wbsTag.wbsTagDisplayChildCompletedCount = 0;
                    wbsTag.wbsTagDisplayChildClosedCount = 0;
                    finalChildTags.Add(wbsTag);
                }
            }
            else
            {
                System.Threading.Tasks.Parallel.ForEach(_allWBSTagDisplay, wbsTagDisplay =>
                {
                    wbsTagDisplay.wbsTagDisplaySelfTotalCount = 0;
                    wbsTagDisplay.wbsTagDisplayChildTotalCount = 0;
                    wbsTagDisplay.wbsTagDisplayChildInspectedCount = 0;
                    wbsTagDisplay.wbsTagDisplayChildApprovedCount = 0;
                    wbsTagDisplay.wbsTagDisplayChildCompletedCount = 0;
                    wbsTagDisplay.wbsTagDisplayChildClosedCount = 0;

                    if (!_allWBSTagDisplay.Any(obj => obj.wbsTagDisplayParentGuid == wbsTagDisplay.wbsTagDisplayGuid))
                        finalChildTags.Add(wbsTagDisplay);
                });
            }

            splashScreenManager3.ShowWaitForm();
            splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, finalChildTags.Count);
            splashScreenManager3.SetWaitFormCaption("Loading Tags...");

            //only select one unique child for each parent guid
            var finalChildTagGrouped = (from finalChildTag in finalChildTags
                                        group finalChildTag by finalChildTag.wbsTagDisplayParentGuid into childGroup
                                        select new { ParentGuid = childGroup.Key, Childrens = childGroup.ToList() }).ToList();

            //cannot put in multithreaded operation because parent can be summed before child is processed
            foreach (var groupedChildrens in finalChildTagGrouped)
            {
                if (groupedChildrens.Childrens.Count > 0)
                {
                    foreach (var children in groupedChildrens.Childrens)
                    {
                        //only start recursing parent if the last children has been calculated
                        updateWBSTagStatus(children, tagIncompleteCounts, wbsIncompleteCounts, groupedChildrens.Childrens.Last() == children);
                        splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                    }
                    //System.Threading.Tasks.Parallel.ForEach(groupedChildrens.Childrens, children =>
                    //{
                    //    //only start recursing parent if the last children has been calculated
                    //    updateWBSTagStatus(children, tagIncompleteCounts, wbsIncompleteCounts, groupedChildrens.Childrens.Last() == children);
                    //    splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                    //});
                }
            }

            //System.Threading.Tasks.Parallel.ForEach(finalChildTagGrouped, groupedChildrens =>
            //{
            //    if (groupedChildrens.Childrens.Count > 0)
            //    {
            //        System.Threading.Tasks.Parallel.ForEach(groupedChildrens.Childrens, children =>
            //        {
            //            //only start recursing parent if the last children has been calculated
            //            updateWBSTagStatus(children, tagIncompleteCounts, wbsIncompleteCounts, groupedChildrens.Childrens.Last() == children);
            //            splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            //        });
            //    }
            //});

            splashScreenManager3.CloseWaitForm();
            //foreach (wbsTagDisplay finalChildTag in finalChildTags)
            //{
            //    RecurseParentDisabling(finalChildTag.wbsTagDisplayGuid, tagIncompleteCounts, wbsIncompleteCounts);
            //}
            RefreshProgress();
        }

        /// <summary>
        /// Refresh the percentages
        /// </summary>
        private void RefreshProgress()
        {
            float completedCount;
            System.Threading.Tasks.Parallel.ForEach(_allWBSTagDisplay, wbsTagDisplay =>
            {
                if (wbsTagDisplay.wbsTagDisplayChildInspectedCount > 0)
                    wbsTagDisplay.wbsTagDisplayInspectedProgress = (wbsTagDisplay.wbsTagDisplayChildInspectedCount / wbsTagDisplay.wbsTagDisplayChildTotalCount) * 100.0f;
                else
                    wbsTagDisplay.wbsTagDisplayInspectedProgress = 0f;

                if (wbsTagDisplay.wbsTagDisplayChildApprovedCount > 0)
                    wbsTagDisplay.wbsTagDisplayApprovedProgress = ((wbsTagDisplay.wbsTagDisplayChildApprovedCount) / wbsTagDisplay.wbsTagDisplayChildTotalCount) * 100.0f;
                else
                    wbsTagDisplay.wbsTagDisplayApprovedProgress = 0f;

                if (wbsTagDisplay.wbsTagDisplayChildCompletedCount > 0)
                    wbsTagDisplay.wbsTagDisplayCompletedProgress = ((wbsTagDisplay.wbsTagDisplayChildCompletedCount) / wbsTagDisplay.wbsTagDisplayChildTotalCount) * 100.0f;
                else
                    wbsTagDisplay.wbsTagDisplayCompletedProgress = 0f;

                if (wbsTagDisplay.wbsTagDisplayChildClosedCount > 0)
                    wbsTagDisplay.wbsTagDisplayClosedProgress = ((wbsTagDisplay.wbsTagDisplayChildClosedCount) / wbsTagDisplay.wbsTagDisplayChildTotalCount) * 100.0f;
                else
                    wbsTagDisplay.wbsTagDisplayClosedProgress = 0f;

                completedCount = ((wbsTagDisplay.wbsTagDisplayChildInspectedCount) + (wbsTagDisplay.wbsTagDisplayChildApprovedCount) + (wbsTagDisplay.wbsTagDisplayChildCompletedCount) + (wbsTagDisplay.wbsTagDisplayChildClosedCount));
                if (completedCount > 0)
                {
                    if (wbsTagDisplay.wbsTagDisplayChildTotalCount > 0)
                        wbsTagDisplay.wbsTagDisplayTotalProgress = (completedCount / (wbsTagDisplay.wbsTagDisplayChildTotalCount * 4)) * 100.0f;
                    else
                        wbsTagDisplay.wbsTagDisplayTotalProgress = 100.0f;
                }
                else
                    wbsTagDisplay.wbsTagDisplayTotalProgress = 0f;
            });

            //foreach (wbsTagDisplay wbsTagDisplay in _allWBSTagDisplay)
            //{
            //    if (wbsTagDisplay.wbsTagDisplayChildInspectedCount > 0)
            //        wbsTagDisplay.wbsTagDisplayInspectedProgress = (wbsTagDisplay.wbsTagDisplayChildInspectedCount / wbsTagDisplay.wbsTagDisplayChildTotalCount) * 100.0f;
            //    else
            //        wbsTagDisplay.wbsTagDisplayInspectedProgress = 0f;

            //    if (wbsTagDisplay.wbsTagDisplayChildApprovedCount > 0)
            //        wbsTagDisplay.wbsTagDisplayApprovedProgress = ((wbsTagDisplay.wbsTagDisplayChildApprovedCount) / wbsTagDisplay.wbsTagDisplayChildTotalCount) * 100.0f;
            //    else
            //        wbsTagDisplay.wbsTagDisplayApprovedProgress = 0f;

            //    if (wbsTagDisplay.wbsTagDisplayChildCompletedCount > 0)
            //        wbsTagDisplay.wbsTagDisplayCompletedProgress = ((wbsTagDisplay.wbsTagDisplayChildCompletedCount) / wbsTagDisplay.wbsTagDisplayChildTotalCount) * 100.0f;
            //    else
            //        wbsTagDisplay.wbsTagDisplayCompletedProgress = 0f;

            //    if (wbsTagDisplay.wbsTagDisplayChildClosedCount > 0)
            //        wbsTagDisplay.wbsTagDisplayClosedProgress = ((wbsTagDisplay.wbsTagDisplayChildClosedCount) / wbsTagDisplay.wbsTagDisplayChildTotalCount) * 100.0f;
            //    else
            //        wbsTagDisplay.wbsTagDisplayClosedProgress = 0f;

            //    completedCount = ((wbsTagDisplay.wbsTagDisplayChildInspectedCount) + (wbsTagDisplay.wbsTagDisplayChildApprovedCount) + (wbsTagDisplay.wbsTagDisplayChildCompletedCount) + (wbsTagDisplay.wbsTagDisplayChildClosedCount));
            //    if (completedCount > 0)
            //    {
            //        if (wbsTagDisplay.wbsTagDisplayChildTotalCount > 0)
            //            wbsTagDisplay.wbsTagDisplayTotalProgress = (completedCount / (wbsTagDisplay.wbsTagDisplayChildTotalCount * 4)) * 100.0f;
            //        else
            //            wbsTagDisplay.wbsTagDisplayTotalProgress = 100.0f;
            //    }
            //    else
            //        wbsTagDisplay.wbsTagDisplayTotalProgress = 0f;
            //}
        }

        private void updateWBSTagStatus(wbsTagDisplay wbsTagDisplay, List<tagCount> tagIncompleteRegister, List<wbsCount> wbsIncompleteRegister, bool updateParent)
        {
            tagCount findTagStatus = null;
            wbsCount findWBSStatus = null;
            if (wbsTagDisplay.wbsTagDisplayAttachTag != null)
            {
                findTagStatus = tagIncompleteRegister.FirstOrDefault(obj => (Guid)obj.tcTag.Value == wbsTagDisplay.wbsTagDisplayGuid);
                if (findTagStatus != null)
                {
                    wbsTagDisplay.wbsTagDisplaySelfTotalCount = findTagStatus.tcTotalCount;
                    wbsTagDisplay.wbsTagDisplayChildTotalCount += findTagStatus.tcTotalCount;
                    wbsTagDisplay.wbsTagDisplayChildInspectedCount += findTagStatus.tcInspectedCount;
                    wbsTagDisplay.wbsTagDisplayChildApprovedCount += findTagStatus.tcApprovedCount;
                    wbsTagDisplay.wbsTagDisplayChildCompletedCount += findTagStatus.tcCompletedCount;
                    wbsTagDisplay.wbsTagDisplayChildClosedCount += findTagStatus.tcClosedCount;
                }
            }
            else
            {
                findWBSStatus = wbsIncompleteRegister.FirstOrDefault(obj => (Guid)obj.wcWBS.Value == wbsTagDisplay.wbsTagDisplayGuid);
                if (findWBSStatus != null)
                {
                    wbsTagDisplay.wbsTagDisplaySelfTotalCount = findWBSStatus.wcTotalCount;
                    wbsTagDisplay.wbsTagDisplayChildTotalCount += findWBSStatus.wcTotalCount;
                    wbsTagDisplay.wbsTagDisplayChildInspectedCount += findWBSStatus.wcInspectedCount;
                    wbsTagDisplay.wbsTagDisplayChildApprovedCount += findWBSStatus.wcApprovedCount;
                    wbsTagDisplay.wbsTagDisplayChildCompletedCount += findWBSStatus.wcCompletedCount;
                    wbsTagDisplay.wbsTagDisplayChildClosedCount += findWBSStatus.wcClosedCount;
                }
            }

            if(updateParent)
                updateWBSTagParentStatus(wbsTagDisplay, tagIncompleteRegister, wbsIncompleteRegister);
        }

        private void updateWBSTagParentStatus(wbsTagDisplay childWBSTag, List<tagCount> tagIncompleteRegister, List<wbsCount> wbsIncompleteRegister)
        {
            if (childWBSTag.wbsTagDisplayParentGuid == Guid.Empty)
                return;

            //look for parent
            wbsTagDisplay wbsTagParent = _allWBSTagDisplay.FirstOrDefault(obj => obj.wbsTagDisplayGuid == childWBSTag.wbsTagDisplayParentGuid);
            if(wbsTagParent != null)
            {
                IEnumerable<wbsTagDisplay> wbsTagChildrens = _allWBSTagDisplay.Where(x => x.wbsTagDisplayParentGuid == wbsTagParent.wbsTagDisplayGuid);
                wbsTagParent.wbsTagDisplayChildTotalCount = wbsTagChildrens.Sum(x => x.wbsTagDisplayChildTotalCount);
                wbsTagParent.wbsTagDisplayChildInspectedCount = wbsTagChildrens.Sum(x => x.wbsTagDisplayChildInspectedCount);
                wbsTagParent.wbsTagDisplayChildApprovedCount = wbsTagChildrens.Sum(x => x.wbsTagDisplayChildApprovedCount);
                wbsTagParent.wbsTagDisplayChildCompletedCount = wbsTagChildrens.Sum(x => x.wbsTagDisplayChildCompletedCount);
                wbsTagParent.wbsTagDisplayChildClosedCount = wbsTagChildrens.Sum(x => x.wbsTagDisplayChildClosedCount);

                updateWBSTagParentStatus(wbsTagParent, tagIncompleteRegister, wbsIncompleteRegister);
            }
        }
        
        /// <summary>
        /// Commented out to speed up loading - Enable all Template to be started until we disable it
        /// </summary>
        private void EnableAllWBSTagTemplate()
        {
            foreach (WorkflowTemplateTagWBS pendingTemplate in _allPending)
            {
                Guid attachmentGuid = Guid.Empty;
                if (pendingTemplate.wtDisplayAttachTag != null)
                    attachmentGuid = pendingTemplate.wtDisplayAttachTag.GUID;
                else if (pendingTemplate.wtDisplayAttachWBS != null)
                    attachmentGuid = pendingTemplate.wtDisplayAttachWBS.GUID;

                wbsTagDisplay findParent = _allWBSTagDisplay.FirstOrDefault(obj => obj.wbsTagDisplayGuid == attachmentGuid);
                if (findParent != null)
                {
                    pendingTemplate.wtEnabled = findParent.wbsTagDisplayEnabled;
                }
            }
        }


        List<tagWorkflow> tagPriorities = null;
        List<wbsWorkflow> wbsPriorities = null;
        /// <summary>
        /// Commented out to speed up loading -Mark whether Tag/WBS template is able to progress according to workflow assignment
        /// </summary>
        private void DisableTagWBSTemplate(bool refreshFromDb)
        {
            ////enable all by default and disable when any of the templates are disabled for the wbs/tag
            //foreach(wbsTagDisplay wbsTag in _allWBSTagDisplay)
            //{
            //    wbsTag.wbsTagDisplayEnabled = true;
            //}

            splashScreenManager3.ShowWaitForm();
            splashScreenManager3.SetWaitFormCaption("Retrieving Template Permissions...");

            //establishing workflow priority
            List<workflowPriority> workflowPriorityList = new List<workflowPriority>();
            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWorkflow = _daWorkflow.GetWorkflowChildrens(Guid.Empty);
            if (dtWorkflow != null)
            {
                foreach (dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow in dtWorkflow.Rows)
                {
                    workflowPriorityList.Add(new workflowPriority()
                    {
                        wpWorkflow = new ValuePair(drWorkflow.NAME, drWorkflow.GUID),
                        wpPriority = dtWorkflow.Rows.IndexOf(drWorkflow)
                    });
                }
            }

            //retrieve unprogressed tag/wbs template and corresponding workflow
            if (tagPriorities == null || refreshFromDb)
            {
                tagPriorities = _daTemplate.GetUnprogressedTagTemplate(_projectGuid);
                //mark the priority of filtered tag workflow
                System.Threading.Tasks.Parallel.ForEach(
                tagPriorities,
                tagPriority =>
                {
                    workflowPriority wpSearch = workflowPriorityList.FirstOrDefault(obj => (Guid)obj.wpWorkflow.Value == (Guid)tagPriority.twWorkflow.Value);
                    if (wpSearch != null)
                        tagPriority.twPriority = wpSearch.wpPriority;
                });

                //disable lower priority tag if workflow with higher priority exists
                System.Threading.Tasks.Parallel.ForEach(
                tagPriorities,
                tagPriority =>
                {
                    if (tagPriorities.Any(obj => ((Guid)obj.twTag.Value == (Guid)tagPriority.twTag.Value && obj.twPriority > tagPriority.twPriority)))
                        tagPriority.twEnabled = false;
                    else
                        tagPriority.twEnabled = true;
                });
            }

            if (wbsPriorities == null || refreshFromDb)
            {
                wbsPriorities = _daTemplate.GetUnprogressedWBSTemplate(_projectGuid);
                //mark the priority of filtered wbs workflow
                System.Threading.Tasks.Parallel.ForEach(
                wbsPriorities,
                wbsPriorityFilter =>
                {
                    workflowPriority wpSearch = workflowPriorityList.FirstOrDefault(obj => (Guid)obj.wpWorkflow.Value == (Guid)wbsPriorityFilter.wwWorkflow.Value);
                    if (wpSearch != null)
                        wbsPriorityFilter.wwPriority = wpSearch.wpPriority;
                });

                //disable lower priority wbs if workflow with higher priority exists
                System.Threading.Tasks.Parallel.ForEach(
                wbsPriorities,
                wbsPriority =>
                {
                    if (wbsPriorities.Any(obj => ((Guid)obj.wwWBS.Value == (Guid)wbsPriority.wwWBS.Value && obj.wwPriority > wbsPriority.wwPriority)))
                        wbsPriority.wwEnabled = false;
                    else
                        wbsPriority.wwEnabled = true;
                });
            }

            splashScreenManager3.CloseWaitForm();

            splashScreenManager3.ShowWaitForm();
            splashScreenManager3.SetWaitFormCaption("Setting Template Permissions...");
            splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, _allPending.Count);
            //enabling/disabling master pending list corresponding to tagWorkflow and wbsWorkflow
            System.Threading.Tasks.Parallel.ForEach(
            _allPending,
            wTempTagWBSPending =>
            {
                Guid? parentGuid = null;
                if (wTempTagWBSPending.wtDisplayAttachTag != null)
                {
                    tagWorkflow findTW = tagPriorities.FirstOrDefault(obj => ((Guid)obj.twTag.Value == wTempTagWBSPending.wtDisplayAttachTag.GUID) && (Guid)obj.twWorkflow.Value == wTempTagWBSPending.wtDisplayParentGuid);
                    if (findTW != null && !findTW.twEnabled) //handled by this routine
                        wTempTagWBSPending.wtEnabled = false;
                    else if (findTW != null) //if this routine doesn't handle the display status assign parent status
                    {
                        parentGuid = (Guid)findTW.twTag.Value;
                        //wbsTagDisplay findWBSTag = _allWBSTagDisplay.FirstOrDefault(obj => (obj.wbsTagDisplayAttachTag != null && obj.wbsTagDisplayAttachTag.GUID == (Guid)findTW.twTag.Value));
                        //if (findWBSTag != null)
                        //    wTempTagWBSPending.wtEnabled = findWBSTag.wbsTagDisplayEnabled;
                    }
                }
                else if (wTempTagWBSPending.wtDisplayAttachWBS != null)
                {
                    wbsWorkflow findWW = wbsPriorities.FirstOrDefault(obj => ((Guid)obj.wwWBS.Value == wTempTagWBSPending.wtDisplayAttachWBS.GUID) && (Guid)obj.wwWorkflow.Value == wTempTagWBSPending.wtDisplayParentGuid);
                    if (findWW != null && !findWW.wwEnabled) //handled by this routine
                        wTempTagWBSPending.wtEnabled = false;
                    else if (findWW != null) //if this routine doesn't handle the display status assign parent status
                    {
                        parentGuid = (Guid)findWW.wwWBS.Value;
                        //wbsTagDisplay findWBSTag = _allWBSTagDisplay.FirstOrDefault(obj => (obj.wbsTagDisplayAttachWBS != null && obj.wbsTagDisplayAttachWBS.GUID == (Guid)findWW.wwWBS.Value));
                        //if (findWBSTag != null)
                        //    wTempTagWBSPending.wtEnabled = findWBSTag.wbsTagDisplayEnabled;
                    }
                }

                //when disabling template is not handled within the same tag number, begin recursing childrens to see if we should be disabling this template
                if (parentGuid != null && (wTempTagWBSPending.wtDisplayAttachTemplate != null && wTempTagWBSPending.wtDisplayAttachTemplate.templateWorkFlow != null))
                {
                    List<wbsTagDisplay> allChildrens = new List<wbsTagDisplay>();
                    CollectAllChildrens(allChildrens, _allWBSTagDisplay, (Guid)parentGuid);
                    List<Guid> childrensGuid = allChildrens.Select(x => x.wbsTagDisplayGuid).ToList();
                    List<tagWorkflow> sameWorkflowTagPriorities = tagPriorities.Where(x => x.twWorkflow.Value.ToString() == wTempTagWBSPending.wtDisplayAttachTemplate.templateWorkFlow.Value.ToString()).ToList();
                    List<tagWorkflow> allChildrensTagWorkflow = sameWorkflowTagPriorities.Where(x => childrensGuid.Any(y => y.ToString() == x.twTag.Value.ToString())).ToList();
                    if (allChildrensTagWorkflow.Count > 0)
                        wTempTagWBSPending.wtEnabled = false;
                    else
                        wTempTagWBSPending.wtEnabled = true;
                }

                ////disable the tag/wbs when any of the template within is disabled
                //if(!wTempTagWBSPending.wtEnabled)
                //{
                //    if (wTempTagWBSPending.wtDisplayAttachTag != null)
                //    {
                //        wbsTagDisplay findWBSTag = _allWBSTagDisplay.FirstOrDefault(obj => (obj.wbsTagDisplayAttachTag != null && obj.wbsTagDisplayAttachTag.GUID == wTempTagWBSPending.wtDisplayAttachTag.GUID));
                //        if (findWBSTag != null)
                //            findWBSTag.wbsTagDisplayEnabled = false;
                //    }
                //    else if (wTempTagWBSPending.wtDisplayAttachWBS != null)
                //    {
                //        wbsTagDisplay findWBSTag = _allWBSTagDisplay.FirstOrDefault(obj => (obj.wbsTagDisplayAttachWBS != null && obj.wbsTagDisplayAttachWBS.GUID == wTempTagWBSPending.wtDisplayAttachWBS.GUID));
                //        if (findWBSTag != null)
                //            findWBSTag.wbsTagDisplayEnabled = false;
                //    }
                //}

                splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            });

            splashScreenManager3.CloseWaitForm();
        }

        /// <summary>
        /// Open up focused document
        /// </summary>
        private void OpenDocument()
        {
            flyoutPanel.HidePopup();
            TreeList focusedTreeList = GetFocusedTreeList();
            if (focusedTreeList == null)
                return;

            //TreeListNode saveNode = focusedTreeList.FocusedNode;
            WorkflowTemplateTagWBS TemplateTagWBS = (WorkflowTemplateTagWBS)focusedTreeList.GetDataRecordByNode(focusedTreeList.FocusedNode);

            //if (focusedTreeList == treeListPending)
            if (TemplateTagWBS.wtEnabled)
                ShowDocument(TemplateTagWBS);
            else if (System_Environment.HasPrivilege(PrivilegeTypeID.SkipStages))
            {
                if (MessageBox.Show("High priority item exists, are you sure you want to skip it?", "Lower Stage Exists", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    ShowDocument(TemplateTagWBS);
            }
            else
                Common.Warn("Higher priority item exists\n\nPlease complete them before starting this");
            //else
            //    ShowDocument(TemplateTagWBS, true);
        }

        /// <summary>
        /// Retrieve the focused treeList
        /// </summary>
        private TreeList GetFocusedTreeList()
        {
            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPagePending)
                return treeListPending;

            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageSaved)
                return treeListSaved;

            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageInspected)
                return treeListInspected;

            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageApproved)
                return treeListApproved;

            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageCompleted)
                return treeListCompleted;

            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageClosed)
                return treeListClosed;

            return null;
        }

        /// <summary>
        /// Retrieve all comments for focused node on focused treeList
        /// </summary>
        private bool RefreshComments()
        {
            TreeList focusedTreeList = GetFocusedTreeList();

            if (focusedTreeList == null) //we don't have comments on pending list
                return false;

            WorkflowTemplateTagWBS selectedWBSTag = (WorkflowTemplateTagWBS)focusedTreeList.GetDataRecordByNode(focusedTreeList.FocusedNode);
            if (selectedWBSTag == null)
                return false;

            if (selectedWBSTag.wtDisplayAttachWorkflow != null)
            {
                Common.Warn("Workflow does not contain history");
                return false;
            }

            dsITR_MAIN.ITR_MAINRow drITR = Common.GetITR(selectedWBSTag.wtDisplayAttachTag, selectedWBSTag.wtDisplayAttachWBS, selectedWBSTag.wtTrueTemplateGuid);
            if (drITR != null)
            {
                Common.GetITRStatusComments(drITR.GUID, _allComments, out _commentingITRStatus);
                treeListComments.RefreshDataSource();
                //treeListComments.BestFitColumns();
                treeListComments.ExpandAll();
                treeListComments.MoveLast();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Shows the ITR or saved template
        /// </summary>
        /// <param name="isITR">Is template saved into ITR?</param>
        private void ShowDocument(WorkflowTemplateTagWBS TemplateTagWBS)
        {
            if (TemplateTagWBS.wtDisplayAttachTemplate != null)
            {
                //splashScreenManager2.ShowWaitForm();
                frmITR_Main f = new frmITR_Main(TemplateTagWBS, false, update_itr_status, _projectGuid, spinEditFontSize.Value);
                //splashScreenManager2.CloseWaitForm();
                f.Show();
                //iTRBrowser_Update updateStatus = f.GetUpdateStatus();
                //dsITR_MAIN.ITR_MAINDataTable dtDELETED_ITRs = f.GetDeletedITRs();
                //if (dtDELETED_ITRs.Rows.Count > 0)
                //{
                //    foreach (dsITR_MAIN.ITR_MAINRow drDeletedITR in dtDELETED_ITRs.Rows)
                //    {
                //        dsITR_MAIN.ITR_MAINRow drMasterDeletedITR = _dtITRMaster.FirstOrDefault(obj => obj.GUID == drDeletedITR.GUID);
                //        WorkflowTemplateTagWBS SearchTemplateTagWBS = FindTemplateTagWBS(drDeletedITR.IsTAG_GUIDNull() ? drDeletedITR.WBS_GUID : drDeletedITR.TAG_GUID, drDeletedITR.TEMPLATE_GUID, true);
                //        if (drMasterDeletedITR != null)
                //        {
                //            _dtITRMaster.RemoveITR_MAINRow(drMasterDeletedITR);
                //            RemoveOneITR(SearchTemplateTagWBS);
                //        }
                //    }
                //}

                //if (updateStatus == iTRBrowser_Update.Saved || updateStatus == iTRBrowser_Update.SavedProgress)
                //{
                //    dsITR_MAIN.ITR_MAINDataTable dtAddedITR = f.GetAddedITRs();
                //    if (dtAddedITR != null)
                //    {
                //        foreach (dsITR_MAIN.ITR_MAINRow drAddedITR in dtAddedITR.Rows)
                //        {
                //            dsITR_MAIN.ITR_MAINRow drNewITR = _dtITRMaster.NewITR_MAINRow();
                //            drNewITR.ItemArray = drAddedITR.ItemArray;
                //            if (_dtITRMaster.FirstOrDefault(obj => obj.GUID == drNewITR.GUID) == null) //avoid exception when user clicked save on an already saved document
                //            {
                //                _dtITRMaster.AddITR_MAINRow(drNewITR);
                //                WorkflowTemplateTagWBS SearchTemplateTagWBS = FindTemplateTagWBS(drAddedITR.IsTAG_GUIDNull() ? drAddedITR.WBS_GUID : drAddedITR.TAG_GUID, drAddedITR.TEMPLATE_GUID, false);

                //                if(SearchTemplateTagWBS != null)
                //                {
                //                    if (updateStatus == iTRBrowser_Update.Saved)
                //                        AddOneTemplateToITR(SearchTemplateTagWBS, drNewITR, false);
                //                    else if (updateStatus == iTRBrowser_Update.SavedProgress)
                //                        AddOneTemplateToITR(SearchTemplateTagWBS, drNewITR, true);
                //                }
                //            }
                //        }
                //    }
                //}
                //else if (updateStatus == iTRBrowser_Update.Progressed)
                //{
                //    dsITR_MAIN.ITR_MAINDataTable dtITRGet = f.GetAddedITRs();

                //    if (dtITRGet != null)
                //    {
                //        foreach(dsITR_MAIN.ITR_MAINRow drITR in dtITRGet)
                //        {
                //            dsITR_MAIN.ITR_MAINRow drITRMoved = _dtITRMaster.FirstOrDefault(obj => obj.GUID == drITR.GUID);
                //            WorkflowTemplateTagWBS SearchTemplateTagWBS = FindTemplateTagWBS(drITR.IsTAG_GUIDNull() ? drITR.WBS_GUID : drITR.TAG_GUID, drITR.TEMPLATE_GUID, true);

                //            string s = string.Empty;
                //            if (drITRMoved != null && SearchTemplateTagWBS != null)
                //                RefreshOneStatus(SearchTemplateTagWBS, drITRMoved);
                //            else
                //            {
                //                dsITR_MAIN.ITR_MAINRow drNewITR = _dtITRMaster.NewITR_MAINRow();
                //                drNewITR.ItemArray = drITR.ItemArray;
                //                _dtITRMaster.AddITR_MAINRow(drNewITR);

                //                SearchTemplateTagWBS = FindTemplateTagWBS(drITR.IsTAG_GUIDNull() ? drITR.WBS_GUID : drITR.TAG_GUID, drITR.TEMPLATE_GUID, false);
                //                //AddOneTemplateToITR(SearchTemplateTagWBS, drITR, true, true);
                //                AddOneTemplateToITR(SearchTemplateTagWBS, drITR, true);
                //            }
                //        }
                //    }
                //}

                //f.Dispose();
            }
        }

        /// <summary>
        /// Shows the ITR or saved template
        /// </summary>
        /// <param name="isITR">Is template saved into ITR?</param>
        private void AttachDocument(WorkflowTemplateTagWBS TemplateTagWBS, bool isPDF)
        {
            if (TemplateTagWBS.wtDisplayAttachTemplate != null)
            {
                using (frmITR_Main f = new frmITR_Main(TemplateTagWBS, false, update_itr_status, _projectGuid, spinEditFontSize.Value))
                {
                    if (isPDF)
                    {
                        bool result = f.AttachPDF();
                        if (!result)
                            MessageBox.Show("Invalid attachment");
                    }
                    else
                        f.AttachImage();
                }
            }
        }

        public void BulkAttachDocument()
        {
            OpenFileDialog thisDialog = new OpenFileDialog();

            thisDialog.Filter = "PDF (*.PDF)|*.PDF";
            thisDialog.FilterIndex = 1;
            thisDialog.RestoreDirectory = true;
            thisDialog.Multiselect = true;
            thisDialog.Title = "Please Select PDF";
            if (thisDialog.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo("Temp");
                if (Directory.Exists(directoryInfo.FullName))
                {
                    foreach (FileInfo file in directoryInfo.GetFiles())
                    {
                        file.Delete();
                    }
                }
                else
                    directoryInfo = System.IO.Directory.CreateDirectory("Temp");

                string delimiter = "#";
                using (AdapterPROJECT daPROJECT = new AdapterPROJECT())
                {
                    dsPROJECT.PROJECTRow drPROJECT = daPROJECT.GetBy(_projectGuid);
                    if (drPROJECT != null)
                    {
                        splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
                        splashScreenManager1.ShowWaitForm();
                        splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetLargeFormat, 1000);

                        int errorCount = 1;
                        foreach (string filename in thisDialog.FileNames)
                        {
                            List<Image> images = new List<Image>();
                            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
                            {
                                documentProcessor.LoadDocument(filename);
                                List<string> splitFileName = filename.Split('\\').ToList();
                                splashScreenManager1.SetWaitFormCaption("Converting PDF to image from " + splitFileName.Last() + " ...");
                                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, documentProcessor.Document.Pages.Count);

                                for (int i = 1; i <= documentProcessor.Document.Pages.Count; i++)
                                {
                                    bool isError = false;
                                    Bitmap pageBitmap = documentProcessor.CreateBitmap(i, 10000);
                                    string metaString = Common.GetDocumentMetaQRCodeTryHarder(pageBitmap);
                                    string error_message = string.Empty;
                                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                                    string[] metaArray = metaString.Split(';');

                                    string project_number = string.Empty;
                                    string tag_number = string.Empty;
                                    string document_name = string.Empty;
                                    string document_status = string.Empty;
                                    string page_number = string.Empty;
                                    string total_pages = string.Empty;

                                    if (metaArray.Count() < 2)
                                    {
                                        isError = true;
                                    }
                                    else
                                    {
                                        project_number = metaArray[0];
                                        tag_number = metaArray[1];
                                        document_name = metaArray[2];
                                        document_status = metaArray[3];
                                        page_number = metaArray[4];
                                        total_pages = metaArray[5];

                                        if (project_number != drPROJECT.NUMBER)
                                            isError = true;
                                    }

                                    if (!isError)
                                    {
                                        //erase the current QRCode
                                        Graphics g = Graphics.FromImage(pageBitmap);
                                        SolidBrush brush = new SolidBrush(Color.White);
                                        g.FillRectangle(brush, new Rectangle(((int)(pageBitmap.Width / 2) - 400), pageBitmap.Height - 800, 800, 800));

                                        string file_name = project_number + delimiter + tag_number + delimiter + document_name + delimiter + document_status + delimiter + total_pages + delimiter + page_number;
                                        file_name = Common.FormatFileName(file_name);
                                        pageBitmap.Save(directoryInfo.FullName + "\\" + file_name + ".bmp");
                                    }
                                    else
                                    {
                                        pageBitmap.Save(directoryInfo.FullName + "\\Error_" + errorCount.ToString() + ".bmp");
                                        errorCount++;
                                    }
                                }

                                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                            }
                        }


                        //Waiting for conversion to finalise before scanning
                        Thread.Sleep(5000);
                        splashScreenManager1.CloseWaitForm();
                        if (!lookup_attachment(_allClosed.ToList(), drPROJECT.NUMBER, delimiter, ITR_Status.Closed.ToString(), directoryInfo))
                            if (!lookup_attachment(_allCompleted.ToList(), drPROJECT.NUMBER, delimiter, ITR_Status.Completed.ToString(), directoryInfo))
                                if (!lookup_attachment(_allApproved.ToList(), drPROJECT.NUMBER, delimiter, ITR_Status.Approved.ToString(), directoryInfo))
                                    if (!lookup_attachment(_allInspected.ToList(), drPROJECT.NUMBER, delimiter, ITR_Status.Inspected.ToString(), directoryInfo))
                                        if (!lookup_attachment(_allPending.ToList(), drPROJECT.NUMBER, delimiter, string.Empty, directoryInfo))
                                            lookup_attachment(_allSaved.ToList(), drPROJECT.NUMBER, delimiter, string.Empty, directoryInfo);

                        //if(!lookup_attachment(_allPending.ToList(), drPROJECT.NUMBER, delimiter, string.Empty, directoryInfo))
                        //    if(!lookup_attachment(_allSaved.ToList(), drPROJECT.NUMBER, delimiter, string.Empty, directoryInfo))
                        //        if(!lookup_attachment(_allInspected.ToList(), drPROJECT.NUMBER, delimiter, ITR_Status.Inspected.ToString(), directoryInfo))
                        //            if(!lookup_attachment(_allApproved.ToList(), drPROJECT.NUMBER, delimiter, ITR_Status.Approved.ToString(), directoryInfo))
                        //                if(!lookup_attachment(_allCompleted.ToList(), drPROJECT.NUMBER, delimiter, ITR_Status.Completed.ToString(), directoryInfo))
                        //                    lookup_attachment(_allClosed.ToList(), drPROJECT.NUMBER, delimiter, ITR_Status.Closed.ToString(), directoryInfo)
                    }
                }

                MessageBox.Show("Bulk import attachment completed\nWhen this message is closed a folder will appear to show files with error");
                Process.Start(directoryInfo.FullName);
            }
        }

        private bool lookup_attachment(IEnumerable<WorkflowTemplateTagWBS> wbs_tags, string project_number, string delimiter, string status, DirectoryInfo directoryInfo)
        {
            splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(global::CheckmateDX.ProgressForm), false, true);
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetLargeFormat, 1000);

            List<string> imageFileNames = Directory.GetFiles(directoryInfo.FullName).ToList();
            IEnumerable<WorkflowTemplateTagWBS> validTemplateWBSTags = wbs_tags.Where(x => x.wtDisplayAttachTemplate != null && x.wtEnabled);
            splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, validTemplateWBSTags.Count());

            bool isAttached = false;
            foreach (WorkflowTemplateTagWBS validWBSTag in validTemplateWBSTags)
            {
                string searchFileName = project_number + delimiter;
                if (validWBSTag.wtDisplayAttachTag != null)
                {
                    searchFileName += validWBSTag.wtDisplayAttachTag.tagNumber;
                }
                else if (validWBSTag.wtDisplayAttachWBS != null)
                {
                    searchFileName += validWBSTag.wtDisplayAttachWBS.wbsName;
                }

                searchFileName += delimiter + project_number + "_" + validWBSTag.wtDisplayName + "_" + validWBSTag.wtDisplayRevision + delimiter + status;
                searchFileName = Common.FormatFileName(searchFileName);
                searchFileName = searchFileName.ToUpper();
                splashScreenManager1.SetWaitFormCaption("Searching image file " + searchFileName + " ...");
                List<string> relatedFiles = imageFileNames.Where(x => x.ToUpper().Contains(searchFileName)).ToList();

                bool isError = false;
                if (relatedFiles.Count > 0)
                {
                    string anyFileName = relatedFiles.First();
                    string[] fileMeta = anyFileName.Split('#');
                    string numberOfFilesStr = fileMeta[4];
                    int numberOfFiles = 0;
                    if (Int32.TryParse(numberOfFilesStr, out numberOfFiles))
                    {
                        if (relatedFiles.Count == numberOfFiles)
                        {
                            List<string> orderedRelatedFiles = relatedFiles.OrderBy(x => x).ToList();
                            List<Bitmap> orderedBitmap = new List<Bitmap>();
                            foreach (string orderedRelatedFile in orderedRelatedFiles)
                            {
                                //sometimes duplicate tag number with the same template can cause file to be missing because file deletion is deferred and Directory.GetFiles retrieved it before it was deleted
                                if(File.Exists(orderedRelatedFile))
                                {
                                    Bitmap bitmap = new Bitmap(orderedRelatedFile);
                                    orderedBitmap.Add(bitmap);
                                }
                            }

                            using (frmITR_Main f = new frmITR_Main(validWBSTag, update_itr_status))
                            {
                                splashScreenManager1.SetWaitFormCaption("Found image for " + searchFileName + ", attaching ...");
                                f.ReplaceDocumentWithImages(orderedBitmap.Select(x => (Image)x).ToList(), true);
                                isAttached = true;
                            }

                            foreach (Bitmap bitmap in orderedBitmap)
                            {
                                bitmap.Dispose();
                            }
                        }
                    }
                }
                else
                    isError = true;

                if (!isError)
                {
                    foreach (string relatedFile in relatedFiles)
                    {
                        File.Delete(relatedFile);
                    }
                }

                splashScreenManager1.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            splashScreenManager1.CloseWaitForm();
            return isAttached;
        }

        private void update_itr_status(frmITR_Main f)
        {
            iTRBrowser_Update updateStatus = f.GetUpdateStatus();
            dsITR_MAIN.ITR_MAINDataTable dtDELETED_ITRs = f.GetDeletedITRs();
            if (dtDELETED_ITRs.Rows.Count > 0)
            {
                foreach (dsITR_MAIN.ITR_MAINRow drDeletedITR in dtDELETED_ITRs.Rows)
                {
                    dsITR_MAIN.ITR_MAINRow drMasterDeletedITR = _dtITRMaster.FirstOrDefault(obj => obj.GUID == drDeletedITR.GUID);
                    WorkflowTemplateTagWBS SearchTemplateTagWBS = FindITR(drDeletedITR.GUID);
                    if (drMasterDeletedITR != null)
                    {
                        _dtITRMaster.RemoveITR_MAINRow(drMasterDeletedITR);
                        RemoveOneITR(SearchTemplateTagWBS);
                    }
                }
            }

            if (updateStatus == iTRBrowser_Update.Saved || updateStatus == iTRBrowser_Update.SavedProgress || updateStatus == iTRBrowser_Update.SavedClose)
            {
                dsITR_MAIN.ITR_MAINDataTable dtAddedITR = f.GetAddedITRs();
                if (dtAddedITR != null)
                {
                    foreach (dsITR_MAIN.ITR_MAINRow drAddedITR in dtAddedITR.Rows)
                    {
                        dsITR_MAIN.ITR_MAINRow drNewITR = _dtITRMaster.NewITR_MAINRow();
                        drNewITR.ItemArray = drAddedITR.ItemArray;
                        if (_dtITRMaster.FirstOrDefault(obj => obj.GUID == drNewITR.GUID) == null) //avoid exception when user clicked save on an already saved document
                        {
                            _dtITRMaster.AddITR_MAINRow(drNewITR);
                            WorkflowTemplateTagWBS SearchTemplateTagWBS = FindTemplateTagWBS(drAddedITR.IsTAG_GUIDNull() ? drAddedITR.WBS_GUID : drAddedITR.TAG_GUID, drAddedITR.TEMPLATE_GUID, false);

                            if (SearchTemplateTagWBS != null)
                            {
                                if (updateStatus == iTRBrowser_Update.Saved)
                                    AddOneTemplateToITR(SearchTemplateTagWBS, drNewITR, false, false);
                                else if (updateStatus == iTRBrowser_Update.SavedProgress)
                                    AddOneTemplateToITR(SearchTemplateTagWBS, drNewITR, true, false);
                                else if (updateStatus == iTRBrowser_Update.SavedClose)
                                    AddOneTemplateToITR(SearchTemplateTagWBS, drNewITR, false, true);
                            }
                        }
                    }
                }
            }
            else if (updateStatus == iTRBrowser_Update.Progressed)
            {
                dsITR_MAIN.ITR_MAINDataTable dtITRGet = f.GetAddedITRs();

                if (dtITRGet != null)
                {
                    foreach (dsITR_MAIN.ITR_MAINRow drITR in dtITRGet)
                    {
                        dsITR_MAIN.ITR_MAINRow drITRMoved = _dtITRMaster.FirstOrDefault(obj => obj.GUID == drITR.GUID);
                        WorkflowTemplateTagWBS SearchTemplateTagWBS = FindITR(drITR.GUID);

                        string s = string.Empty;
                        if (drITRMoved != null && SearchTemplateTagWBS != null)
                            RefreshOneStatus(SearchTemplateTagWBS, drITRMoved);
                        else
                        {
                            dsITR_MAIN.ITR_MAINRow drNewITR = _dtITRMaster.NewITR_MAINRow();
                            drNewITR.ItemArray = drITR.ItemArray;
                            _dtITRMaster.AddITR_MAINRow(drNewITR);

                            SearchTemplateTagWBS = FindTemplateTagWBS(drITR.IsTAG_GUIDNull() ? drITR.WBS_GUID : drITR.TAG_GUID, drITR.TEMPLATE_GUID, false);
                            //AddOneTemplateToITR(SearchTemplateTagWBS, drITR, true, true);
                            AddOneTemplateToITR(SearchTemplateTagWBS, drITR, true, false);
                        }
                    }
                }
            }

            f.Dispose();
        }

        /// <summary>
        /// Find the template tag wbs in master template or all statuses
        /// </summary>
        private WorkflowTemplateTagWBS FindTemplateTagWBS(Guid WBSTagGuid, Guid TemplateGuid, bool fromStatuses)
        {
            return _masterTemplate.FirstOrDefault(obj => ((obj.wtDisplayAttachTag != null && obj.wtDisplayAttachTag.GUID == WBSTagGuid)
                || (obj.wtDisplayAttachWBS != null && obj.wtDisplayAttachWBS.GUID == WBSTagGuid))
                && obj.wtTrueTemplateGuid == TemplateGuid);
        }


        /// <summary>
        /// Find the template tag wbs in master template or all statuses
        /// </summary>
        private WorkflowTemplateTagWBS FindITR(Guid iTRGuid)
        {
            return _masterITR.FirstOrDefault(obj => obj.wtITRGuid == iTRGuid);
        }

        /// <summary>
        /// Deselect all tag from treeListWBSTag
        /// </summary>
        private void deselectAllWBSTag()
        {
            treeListWBSTag.Selection.UnselectAll();
            //int selectionCount = treeListWBSTag.Selection.Count;

            //for (int i = 0; i < selectionCount; i++)
            //{
            //    treeListWBSTag.Selection.RemoveAt(0);
            //}
        }

        private void treeListPending_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            // Create brushes for cells.
            Brush backBrush;
            Brush foreBrush;
            WorkflowTemplateTagWBS wtTagWBS = (WorkflowTemplateTagWBS)treeListPending.GetDataRecordByNode(e.Node);

            if(wtTagWBS != null)
            {
                if (wtTagWBS.wtDisplayAttachWorkflow == null && !wtTagWBS.wtEnabled)
                {
                    backBrush = new LinearGradientBrush(e.Bounds, Color.Orange, Color.PeachPuff,
                      LinearGradientMode.Horizontal);
                    foreBrush = new SolidBrush(Color.DarkRed);

                    // Fill the background.
                    //e.Graphics.FillRectangle(backBrush, e.Bounds);
                }
                else
                {
                    //backBrush = new SolidBrush(Color.DarkBlue);
                    foreBrush = new SolidBrush(Color.Black);
                }

                // Paint the node value.
                e.Graphics.DrawString(e.CellText, e.Appearance.Font, foreBrush, e.Bounds,
                e.Appearance.GetStringFormat());
                // Prohibit default painting.
                e.Handled = true;
            }
        }

        private void treeListWBSTag_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            // Create brushes for cells.
            //Brush foreBrush;
            wbsTagDisplay wbsTag = (wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(e.Node);
            if (!wbsTag.wbsTagDisplayEnabled)
            {
                //foreBrush = new SolidBrush(Color.DarkRed);
                //e.Graphics.DrawString(e.CellText, e.Appearance.Font, foreBrush, e.Bounds, e.Appearance.GetStringFormat());
                e.Appearance.ForeColor = Color.DarkRed;
            }

            //else
            //    foreBrush = new SolidBrush(Color.Black);
        }
        #endregion

        protected override void OnClosed(EventArgs e)
        {
            _daITR.Dispose();
            _daITRStatus.Dispose();
            _daITRStatusIssue.Dispose();
            _daSchedule.Dispose();
            _daTag.Dispose();
            _daTemplate.Dispose();
            _daWBS.Dispose();
            _daWorkflow.Dispose();
            base.OnClosed(e);
        }

        dsSYNC_CONFLICT dsSYNCCONFLICT = new dsSYNC_CONFLICT();
        AdapterSYNC_CONFLICT _daREMOTE_SYNC_CONFLICT;
        private void btnLogDuplicates_Click(object sender, EventArgs e)
        {
            using (_daREMOTE_SYNC_CONFLICT = new AdapterSYNC_CONFLICT())
            {
                EnumerableRowCollection<dsITR_MAIN.ITR_MAINRow> enumerableITRs = _dtITRMaster.AsEnumerable();

                splashScreenManager2.ShowWaitForm();
                splashScreenManager2.SetWaitFormDescription("Detecting Conflict ...");

                splashScreenManager2.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, enumerableITRs.Count());
                int conflictCount = 0;

                HashSet<Guid> removeITRGuid = new HashSet<Guid>();
                foreach (dsITR_MAIN.ITR_MAINRow drITR in _dtITRMaster.Rows)
                {
                    EnumerableRowCollection<dsITR_MAIN.ITR_MAINRow> drDuplicateITRs = null;
                    if (drITR.IsWBS_GUIDNull())
                        drDuplicateITRs = enumerableITRs.Where(x => x.GUID != drITR.GUID && x.TAG_GUID == drITR.TAG_GUID && x.TEMPLATE_GUID == drITR.TEMPLATE_GUID);
                    else if (drITR.IsTAG_GUIDNull())
                        drDuplicateITRs = enumerableITRs.Where(x => x.GUID != drITR.GUID && x.WBS_GUID == drITR.WBS_GUID && x.TEMPLATE_GUID == drITR.TEMPLATE_GUID);

                    if (drDuplicateITRs.Count() > 0)
                    {
                        foreach(dsITR_MAIN.ITR_MAINRow drDuplicateITR in drDuplicateITRs)
                        {
                            dsITR_STATUS.ITR_STATUSDataTable dtITR_STATUS = _daITRStatus.GetStatusByITR(drITR.GUID, 1);
                            dsITR_STATUS.ITR_STATUSDataTable dtDuplicateITR_STATUS = _daITRStatus.GetStatusByITR(drDuplicateITR.GUID, 1);

                            if (dtITR_STATUS == null && dtDuplicateITR_STATUS != null)
                                removeITRGuid.Add(drITR.GUID);
                            else if (dtITR_STATUS != null && dtDuplicateITR_STATUS == null)
                                removeITRGuid.Add(drDuplicateITR.GUID);
                            else if (dtITR_STATUS != null && dtDuplicateITR_STATUS != null)
                            {
                                dsITR_STATUS.ITR_STATUSRow drITR_STATUS = dtITR_STATUS.First();
                                dsITR_STATUS.ITR_STATUSRow drDuplicateITR_STATUS = dtDuplicateITR_STATUS.First();

                                if (drITR_STATUS.STATUS_NUMBER > drDuplicateITR_STATUS.STATUS_NUMBER)
                                    removeITRGuid.Add(drDuplicateITR.GUID);
                                else if (drITR_STATUS.STATUS_NUMBER < drDuplicateITR_STATUS.STATUS_NUMBER)
                                    removeITRGuid.Add(drITR.GUID);
                                else
                                {
                                    conflictCount += 1;
                                    Log_Conflict(Common.GetHWID(), Sync_Type.ITR_MAIN, drDuplicateITR.GUID, drITR.GUID);
                                }

                            }
                            else //when both record and duplicate record status is null
                            {
                                conflictCount += 1;
                                Log_Conflict(Common.GetHWID(), Sync_Type.ITR_MAIN, drDuplicateITR.GUID, drITR.GUID);
                            }
                        }
                    }

                    splashScreenManager2.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }

                splashScreenManager2.CloseWaitForm();

                if(removeITRGuid.Count > 0)
                {
                    if(MessageBox.Show(removeITRGuid.Count.ToString() + " conflict can be automatically resolved based on status, do you wish to delete duplicate ITRs?", "Auto Resolve Conflict", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        foreach(Guid guid in removeITRGuid)
                        {
                            _daITR.RemoveBy(guid);
                        }

                        MessageBox.Show("Please close and reopen this page to reflect the removed duplicate ITRs");
                    }
                }

                if (conflictCount > 0)
                {
                    MessageBox.Show(conflictCount.ToString() + " conflict ITRs detected, please resolve them in Sync -> Resolve Conflict");
                }
            }
        }
        /// <summary>
        /// Log Conflict to Remote Server
        /// </summary>
        private void Log_Conflict(string HWID, Sync_Type SyncType, Guid ConflictOnGuid, Guid ConflictGuid)
        {
            dsSYNC_CONFLICT.SYNC_CONFLICTRow drNEW_SYNC_CONFLICT = dsSYNCCONFLICT.SYNC_CONFLICT.NewSYNC_CONFLICTRow();
            drNEW_SYNC_CONFLICT.GUID = Guid.NewGuid();
            drNEW_SYNC_CONFLICT.CONFLICT_HWID = HWID;
            drNEW_SYNC_CONFLICT.CONFLICT_TYPE = SyncType.ToString();
            drNEW_SYNC_CONFLICT.CONFLICT_GUID = ConflictGuid;
            drNEW_SYNC_CONFLICT.CONFLICT_ON_GUID = ConflictOnGuid;
            drNEW_SYNC_CONFLICT.CREATED = DateTime.Now;
            drNEW_SYNC_CONFLICT.CREATEDBY = System_Environment.GetUser().GUID;
            dsSYNCCONFLICT.SYNC_CONFLICT.AddSYNC_CONFLICTRow(drNEW_SYNC_CONFLICT);
            _daREMOTE_SYNC_CONFLICT.Save(drNEW_SYNC_CONFLICT);
        }

        private void btnExportAll_Click(object sender, EventArgs e)
        {
            frmExportTaskSheet frmExportTaskSheet = new frmExportTaskSheet(_allPending, _allSaved, _allInspected, _allApproved, _allCompleted, _allClosed);
            frmExportTaskSheet.Show();
        }

        private void applyBestWidth()
        {
            treeListPending.BestFitColumns();
            treeListSaved.BestFitColumns();
            treeListInspected.BestFitColumns();
            treeListApproved.BestFitColumns();
            treeListCompleted.BestFitColumns();
            treeListClosed.BestFitColumns();
        }
    }

    class GetCheckedNode : TreeListOperation
    {
        List<wbsTagDisplay> CheckedWBSTags = new List<wbsTagDisplay>();
        TreeList WBSTagTreeList;
        public GetCheckedNode(TreeList treeList)
            : base()
        {
            WBSTagTreeList = treeList;
        }

        public override void Execute(TreeListNode node)
        {
            if (node.Checked)
            {
                CheckedWBSTags.Add((wbsTagDisplay)WBSTagTreeList.GetDataRecordByNode(node));
            }
        }

        public List<wbsTagDisplay> GetCheckedWBSTags()
        {
            return CheckedWBSTags;
        }
    }

    // Declaring the custom operation class.
    class RememberSelectedNodes : TreeListOperation
    {
        Dictionary<TreeListNode, bool> NodesSelectionMode = new Dictionary<TreeListNode, bool>();
        public RememberSelectedNodes(TreeList treeList)
            : base()
        {
            SelectedNodeConstructor rememberSelected = new SelectedNodeConstructor();
            treeList.NodesIterator.DoLocalOperation(rememberSelected, treeList.Nodes);
            NodesSelectionMode = rememberSelected.GetTreeListSelectionMode();
        }
        public override void Execute(TreeListNode node)
        {
            KeyValuePair<TreeListNode, bool> dSelected = NodesSelectionMode.FirstOrDefault(obj => obj.Key == node);
            if (dSelected.Equals(new KeyValuePair<string, bool>(node[0].ToString(), true)))
                node.Selected = true;
            else
                node.Selected = false;
        }
    }

    class SelectedNodeConstructor : TreeListOperation
    {
        Dictionary<TreeListNode, bool> NodesSelectionMode = new Dictionary<TreeListNode, bool>();
        public SelectedNodeConstructor()
        {

        }

        public override void Execute(TreeListNode node)
        {
            try
            {
                NodesSelectionMode.Add(node, node.Selected);
            }
            catch
            {
                //MessageBox.Show("Duplicate tag number " + node[0].ToString() + " exists, please remove duplicates from master feed sheet");
            }
        }

        public Dictionary<TreeListNode, bool> GetTreeListSelectionMode()
        {
            return NodesSelectionMode;
        }
    }
}
