using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using System.Linq;
using DevExpress.XtraEditors.Controls;
using static ProjectCommon.Common;

namespace CheckmateDX
{
    public partial class frmWBS_Selection : CheckmateDX.frmParent
    {
        //database
        AdapterTAG _daTag = new AdapterTAG();
        AdapterWBS _daWBS = new AdapterWBS();
        AdapterTEMPLATE_MAIN _daTemplate = new AdapterTEMPLATE_MAIN();

        //class lists
        List<wbsTagDisplay> _allWBSTagDisplay = new List<wbsTagDisplay>();
        List<wbsTagDisplay> _filterWBSTagDisplay = new List<wbsTagDisplay>();
        List<wbsTagDisplay> _allWBSStore = new List<wbsTagDisplay>();

        //variables
        Guid _projectGuid = Guid.Empty;
        List<Guid> _excludeGuids = new List<Guid>();
        List<string> _subsystemNumbers = new List<string>();
        bool isWBSOnly = false;

        /// <summary>
        /// ITR Browser Constructor
        /// </summary>
        /// <param name="WBSTagOnly">Show WBS Tag Only</param>
        public frmWBS_Selection()
        {
            InitializeComponent();
            isWBSOnly = true;
            treeListWBSTag.OptionsFind.HighlightFindResults = true;
            timer1.Enabled = true; //this is where the form gets filled out, timer is used because superadmin will have to select parameters
            EstablishTreeListBinding();
        }

        public frmWBS_Selection(List<string>subsystemNumbers)
        {
            InitializeComponent();
            _subsystemNumbers = subsystemNumbers;
            isWBSOnly = false;
            treeListWBSTag.OptionsFind.HighlightFindResults = true;

            timer1.Enabled = true; //this is where the form gets filled out, timer is used because superadmin will have to select parameters
            EstablishTreeListBinding();
        }

        public void SetTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Binds all treelist to their corresponding class list
        /// </summary>
        private void EstablishTreeListBinding()
        {
            //wbsTagDisplayBindingSource.DataSource = _filterWBSTagDisplay;
            wbsTagDisplayBindingSource.DataSource = _allWBSTagDisplay;
        }

        public wbsTagDisplay GetSelectedWBSTag()
        {
            return (wbsTagDisplay)treeListWBSTag.GetFocusedRow();
        }

        public List<wbsTagDisplay> GetSelectedWBSTags()
        {
            List<wbsTagDisplay> wbsTagDisplays = new List<wbsTagDisplay>();
            foreach(TreeListNode selectedNode in treeListWBSTag.Selection)
            {
                wbsTagDisplay selectedWBSTagDisplay = (wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(selectedNode);
                wbsTagDisplays.Add(selectedWBSTagDisplay);
            }

            return wbsTagDisplays;
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

        #region Form Population
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (_projectGuid == Guid.Empty)
            {
                selectScheduleParameters();
            }
        }

        /// <summary>
        /// Used when user is superadmin, which doesn't have default project and discipline
        /// </summary>
        private void selectScheduleParameters()
        {
            //get user parameter
            _projectGuid = (Guid)System_Environment.GetUser().userProject.Value;
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

            btnLoadAll_Click(null, null);
        }

        private void treeListWBSTag_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                deselectAllWBSTag();
                FilterWBSTag_RefreshTemplateITR(_filterWBSTagDisplay);
            }
        }

        private void btnLoadAll_Click(object sender, EventArgs e)
        {
            searchWBSTag();
        }

        /// <summary>
        /// Initialize browser with ITRs
        /// </summary>
        private void Initialize_WBSTagITR()
        {
            EstablishWBSTag(_projectGuid, _excludeGuids); //gets all the WBS/Tag by this project (_allWBSTagDisplay initialised)
            FilteringWBSTags(); //filter the WBS/Tag according to the textbox value (_filterWBSTagDisplay initialised)
            //PopulateWorkflow(_masterWorkflow); //populate all workflow in the system //Changes 29th Jan 2015 - Populate workflow happens in FilterWBSTag_RefreshTemplateITR below
            FilterWBSTag_RefreshTemplateITR(_filterWBSTagDisplay, true); //depending on filter or selection, populate the ITR/Template in their corresponding list
            RefreshWBSTagTreeList();
            //btnClearSelection_Click(null, null);
        }

        private void searchWBSTag()
        {
            _allWBSStore.Clear();
            Initialize_WBSTagITR();
        }

        /// <summary>
        /// Refresh user filtered WBS/Tag from textbox
        /// </summary>
        private void FilteringWBSTags()
        {
            _filterWBSTagDisplay.Clear();
            _filterWBSTagDisplay.AddRange(_allWBSTagDisplay);
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
        #endregion

        #region Event
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void treeListWBSTag_BeforeCheckNode(object sender, CheckNodeEventArgs e)
        {
            //don't allow check to be handled from the checkbox, it is handled in the treeListWBSTag_Click instead
            e.CanCheck = false;
        }

        private void treeListWBSTag_MouseDown(object sender, MouseEventArgs e)
        {
            var hitInfo = treeListWBSTag.CalcHitInfo(new Point(e.X, e.Y));
            if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Empty)
                deselectAllWBSTag();
        }

        /// <summary>
        /// Deselect all tag from treeListWBSTag
        /// </summary>
        private void deselectAllWBSTag()
        {
            treeListWBSTag.Selection.UnselectAll();
        }

        /// <summary>
        /// Wrapper for remembering current selection and restore selection upon refreshing
        /// </summary>
        private void RefreshWBSTagTreeList()
        {
            RememberSelectedNodes rememberSelected = new RememberSelectedNodes(treeListWBSTag);
            treeListWBSTag.RefreshDataSource();
            treeListWBSTag.ExpandAll();
        }
        #endregion

        #region EstablishMasterData
        /// <summary>
        /// Loads all the WBS/Tag assigned to this project and discipline
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <param name="discipline"></param>
        private void EstablishWBSTag(Guid projectGuid, List<Guid> excludeGuid, Guid? templateGuid = null, Guid? excludeWBSTagGuid = null, List<string> disciplines = null, List<string> subSystemNames = null, List<string> systemNames = null, List<string> areaNames = null, string strTagNumber = "", ProjectLibrary.SearchMode searchMode = ProjectLibrary.SearchMode.Contains)
        {
            _allWBSTagDisplay.Clear();

            dsTAG.TAGDisciplineDataTable dtTag;
            dsWBS.WBSDataTable dtWBS;
            dtWBS = _daWBS.GetByProject(_projectGuid);
            if (templateGuid == null)
            {
                if (strTagNumber != string.Empty || (subSystemNames != null && subSystemNames.Count > 0) || (systemNames != null && systemNames.Count > 0) || (areaNames != null && areaNames.Count > 0) || (disciplines != null && disciplines.Count > 0))
                    dtTag = _daTag.GetByProjectWBSName(projectGuid, subSystemNames, systemNames, areaNames, strTagNumber, null, searchMode);
                else
                    dtTag = _daTag.GetByProjectIncludeDiscipline(projectGuid);
            }
            else
            {
                dtTag = _daTag.GetByProjectTemplate(projectGuid, (Guid)templateGuid, (Guid)excludeWBSTagGuid, excludeGuid);
            }

            List<wbsTagDisplay> tempChildrens = new List<wbsTagDisplay>();
            List<wbsTagDisplay> WBSList = isWBSOnly ? _allWBSTagDisplay : _allWBSStore;

            if (dtWBS != null)
            {
                List<dsWBS.WBSRow> WBSRows = new List<dsWBS.WBSRow>();
                //establish WBS table to enumerate for Subsystem
                foreach(dsWBS.WBSRow drWBS in dtWBS.Rows)
                {
                    WBSRows.Add(drWBS);
                }

                foreach (dsWBS.WBSRow drWBS in dtWBS.Rows)
                {
                    //populate sub system's only
                    if(!WBSRows.Any(x => x.PARENTGUID == drWBS.GUID))
                    {
                        bool isAddSubsystem = true;
                        if(_subsystemNumbers.Count > 0)
                        {
                            if(!_subsystemNumbers.Any(x => x.ToUpper() == drWBS.NAME.ToUpper()))
                            {
                                isAddSubsystem = false;
                            }
                        }

                        if(isAddSubsystem)
                            WBSList.Add(new wbsTagDisplay(new WBS(drWBS.GUID)
                            {
                                wbsName = drWBS.NAME,
                                wbsDescription = drWBS.IsDESCRIPTIONNull() ? "" : drWBS.DESCRIPTION,
                                wbsParentGuid = drWBS.PARENTGUID,
                                wbsScheduleGuid = drWBS.SCHEDULEGUID,
                                wbsIsSubsystem = !WBSRows.Any(x => x.PARENTGUID == drWBS.GUID)
                            }));
                    }
                }
            }

            if (!isWBSOnly)
            {
                if (dtTag != null)
                {
                    foreach (dsTAG.TAGDisciplineRow drTag in dtTag.Rows)
                    {
                        if(WBSList.Any(x => x.wbsTagDisplayGuid == drTag.PARENTGUID))
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
                }
            }

            splashScreenManager3.ShowWaitForm();
            splashScreenManager3.SetWaitFormCaption("Loading WBS...");
            splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, tempChildrens.Count);
            //get wbs parents for each tag numbers

            if(!isWBSOnly)
            {
                foreach (wbsTagDisplay wbsTagDisplay in tempChildrens)
                {
                    //add children wbs/tag regardless
                    _allWBSTagDisplay.Add(wbsTagDisplay);

                    //gets all the tag/wbs parent and add it
                    IEnumerable<wbsTagDisplay> wbsParents = Common.AllParent(wbsTagDisplay.wbsTagDisplayParentGuid, _allWBSStore);
                    foreach (wbsTagDisplay wbsParent in wbsParents)
                    {
                        if (!_allWBSTagDisplay.Any(x => x.wbsTagDisplayGuid == wbsParent.wbsTagDisplayGuid))
                        {
                            _allWBSTagDisplay.Add(wbsParent);
                        }
                    }

                    splashScreenManager3.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                }
            }

            splashScreenManager3.CloseWaitForm();
        }
        #endregion

        #region Helper
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
        #endregion

        protected override void OnClosed(EventArgs e)
        {
            _daTag.Dispose();
            _daTemplate.Dispose();
            _daWBS.Dispose();
            base.OnClosed(e);
        }

        private void treeListWBSTag_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
