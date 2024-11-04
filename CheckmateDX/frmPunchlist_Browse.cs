using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPrinting;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Columns;
using static ProjectCommon.Common;
using DevExpress.XtraEditors;
using ProjectDatabase;

namespace CheckmateDX
{
    public partial class frmPunchlist_Browse : frmParent
    {
        //database
        AdapterTAG _daTag = new AdapterTAG();
        AdapterWBS _daWBS = new AdapterWBS();
        AdapterPUNCHLIST_MAIN _daPunchlist = new AdapterPUNCHLIST_MAIN();
        AdapterPUNCHLIST_MAIN_PICTURE _daPunchlistPicture = new AdapterPUNCHLIST_MAIN_PICTURE();
        AdapterPUNCHLIST_STATUS _daPunchlistStatus = new AdapterPUNCHLIST_STATUS();
        AdapterPUNCHLIST_STATUS_ISSUE _daPunchlistStatusIssue = new AdapterPUNCHLIST_STATUS_ISSUE();

        //class lists
        List<ProjectWBS> _fullProjectWBS = new List<ProjectWBS>();
        List<Schedule> _allSchedule = new List<Schedule>();
        List<wbsTagDisplay> _allWBSTagDisplay = new List<wbsTagDisplay>();
        List<wbsTagDisplay> _filterWBSTagDisplay = new List<wbsTagDisplay>();
        List<Punchlist> _masterPunchlist = new List<Punchlist>();
        List<Punchlist> _allStatus = new List<Punchlist>(); //stores all punchlists from new to closed
        List<Punchlist> _allNew = new List<Punchlist>();
        List<Punchlist> _allCategorised = new List<Punchlist>();
        List<Punchlist> _allInspected = new List<Punchlist>();
        List<Punchlist> _allApproved = new List<Punchlist>();
        List<Punchlist> _allCompleted = new List<Punchlist>();
        List<Punchlist> _allClosed = new List<Punchlist>();
        List<punchlistComments> _allComments = new List<punchlistComments>();

        //variables
        Guid _projectGuid = Guid.Empty;
        string _defaultDiscipline = string.Empty;
        Guid _commentingPunchlistStatus = Guid.Empty; //stores the status on which user will be commenting on
        List<Guid> _filterWBSTag = new List<Guid>();
        //wbsTagDisplay _selectedWBSTag;

        WBSSearch WBSSearch;
        public frmPunchlist_Browse()
        {
            InitializeComponent();
            EstablishGridEvents();
            timer1.Enabled = true; //this is where the form gets filled out, timer is used because superadmin will have to select parameters
        }

        public frmPunchlist_Browse(List<Guid> FilterWBSTag)
            : this()
        {
            _filterWBSTag = FilterWBSTag;
            this.WindowState = FormWindowState.Maximized;
            timer1.Enabled = true; //this is where the form gets filled out, timer is used because superadmin will have to select parameters
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            selectScheduleParameters();

            FilterAndShowWBSTag();

            //txtSearchTag.Enter += new EventHandler(Common.textBox_GotFocus);
            //txtSearchTag.Leave += new EventHandler(Common.textBox_Leave);

            //timer1.Enabled = false;
            //txtSearchTag.Enter += new EventHandler(Common.textBox_GotFocus);
            //txtSearchTag.Leave += new EventHandler(Common.textBox_Leave);

            //selectScheduleParameters();
            //FilterAndShowWBSTag();

            //if (_selectedWBSTag != null)
            //{
            //    txtSearchTag.Text = _selectedWBSTag.wbsTagDisplayName;
            //    treeListWBSTag.SetFocusedNode(treeListWBSTag.Nodes.FirstNode);
            //}
        }

        #region Initialization
        /// <summary>
        /// Binds all grids/treelists to their corresponding class list
        /// </summary>
        private void EstablishGridBinding()
        {
            wbsTagDisplayBindingSource.DataSource = _filterWBSTagDisplay;
            punchlistBindingNew.DataSource = _allNew;
            punchlistBindingCategorised.DataSource = _allCategorised;
            punchlistBindingInspected.DataSource = _allInspected;
            punchlistBindingApproved.DataSource = _allApproved;
            punchlistBindingCompleted.DataSource = _allCompleted;
            punchlistBindingClosed.DataSource = _allClosed;
            punchlistCommentsBindingSource.DataSource = _allComments;
            punchlistBindingAll.DataSource = _allStatus;
        }

        /// <summary>
        /// Establish the grid Events
        /// </summary>
        private void EstablishGridEvents()
        {
            RepositoryItemImageComboBox imageCombo = gridControlNew.RepositoryItems.Add("ImageComboBoxEdit") as RepositoryItemImageComboBox;
            DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
            images.AddImage(imageList3.Images[0]);
            images.AddImage(imageList3.Images[1]);
            images.AddImage(imageList3.Images[2]);
            images.AddImage(imageList3.Images[3]);
            images.AddImage(imageList3.Images[4]);
            imageCombo.SmallImages = images;
            imageCombo.Items.Add(new ImageComboBoxItem(0, 0));
            imageCombo.Items.Add(new ImageComboBoxItem(1, 1));
            imageCombo.Items.Add(new ImageComboBoxItem(2, 2));
            imageCombo.Items.Add(new ImageComboBoxItem(3, 3));
            imageCombo.Items.Add(new ImageComboBoxItem(4, 4));
            imageCombo.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridViewNew.Columns["punchlistImageIndex"].ColumnEdit = imageCombo;
            gridViewCategorised.Columns["punchlistImageIndex"].ColumnEdit = imageCombo;
            gridViewInspected.Columns["punchlistImageIndex"].ColumnEdit = imageCombo;
            gridViewCompleted.Columns["punchlistImageIndex"].ColumnEdit = imageCombo;
            
            gridViewNew.DoubleClick += GridView_DoubleClick;
            gridViewNew.MouseUp += gridView_MouseUp;
            gridViewCategorised.DoubleClick += GridView_DoubleClick;
            gridViewCategorised.MouseUp += gridView_MouseUp;
            gridViewInspected.DoubleClick += GridView_DoubleClick;
            gridViewInspected.MouseUp += gridView_MouseUp;
            gridViewCompleted.DoubleClick += GridView_DoubleClick;
            gridViewCompleted.MouseUp += gridView_MouseUp;
            gridViewClosed.DoubleClick += GridView_DoubleClick;
            gridViewClosed.MouseUp += gridView_MouseUp;
        }
        #endregion

        #region Form Population
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
                splashScreenManager1.ShowWaitForm();
                frmTool_Project frmSelectProject = new frmTool_Project();
                splashScreenManager1.CloseWaitForm();
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

                splashScreenManager1.ShowWaitForm();
                frmITR_Select_Discipline frmSelectDiscipline = new frmITR_Select_Discipline();
                if (frmSelectDiscipline.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _defaultDiscipline = frmSelectDiscipline.GetDiscipline();
                }
                else
                {
                    this.Close();
                    return;
                }
            }
            else
                splashScreenManager1.ShowWaitForm();

            _fullProjectWBS = _daWBS.GetProjectWBS(_projectGuid);
            WBSSearch = new WBSSearch(_defaultDiscipline, _fullProjectWBS, cmbArea, cmbSystem, cmbSubsystem, cmbDiscipline, cmbCategory, null, cmbSearchMode);
            EstablishGridBinding();
            EstablishWBSTags();
            splashScreenManager1.CloseWaitForm();
        }

        /// <summary>
        /// Refresh all tag from database
        /// </summary>
        private void EstablishWBSTags()
        {
            if (_projectGuid == Guid.Empty)
                return;

            _allWBSTagDisplay.Clear();

            dsTAG.TAGDataTable dtTag = _daTag.GetByProject(_projectGuid);
            dsWBS.WBSDataTable dtWBS = _daWBS.GetByProject(_projectGuid);

            if (dtTag != null)
            {
                foreach (dsTAG.TAGRow drTag in dtTag.Rows)
                {
                    _allWBSTagDisplay.Add(new wbsTagDisplay(new Tag(drTag.GUID)
                    {
                        tagNumber = drTag.NUMBER,
                        tagDescription = drTag.DESCRIPTION,
                        tagParentGuid = drTag.PARENTGUID,
                        tagScheduleGuid = drTag.SCHEDULEGUID
                    })
                    {
                        //NewwbsTagDisplayEnabled = false
                    });
                }
            }

            if (dtWBS != null)
            {
                foreach (dsWBS.WBSRow drWBS in dtWBS.Rows)
                {
                    _allWBSTagDisplay.Add(new wbsTagDisplay(new WBS(drWBS.GUID)
                    {
                        wbsName = drWBS.NAME,
                        wbsDescription = drWBS.IsDESCRIPTIONNull() ? "" : drWBS.DESCRIPTION,
                        wbsParentGuid = drWBS.PARENTGUID,
                        wbsScheduleGuid = drWBS.SCHEDULEGUID
                    })
                    {
                        //NewwbsTagDisplayEnabled = false
                    });
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string strDescriptionStr = txtPunchlistDescription.EditValue == null ? string.Empty : txtPunchlistDescription.EditValue.ToString();
            List<string> selectedSubsystems = WBSSearch.SelectedSubsystems.Count == 0 || WBSSearch.SelectedSubsystems.All(x => x == "") ? null : WBSSearch.SelectedSubsystems;
            List<string> selectedSystems = WBSSearch.SelectedSystems.Count == 0 || WBSSearch.SelectedSystems.All(x => x == "") ? null : WBSSearch.SelectedSystems;
            List<string> selectedAreas = WBSSearch.SelectedAreas.Count == 0 || WBSSearch.SelectedAreas.All(x => x == "") ? null : WBSSearch.SelectedAreas;

            EstablishMasterPunchlist(_allWBSTagDisplay, _projectGuid, WBSSearch.SelectedDisciplines, selectedSubsystems, selectedSystems, selectedAreas, strDescriptionStr, WBSSearch.SelectedCategories, WBSSearch.SearchMode);
            FilterAndShowWBSTag();
            RefreshAllPunchlist(_masterPunchlist);
            applyBestWidth();
        }

        private void applyBestWidth()
        {
            gridViewNew.BestFitColumns();
            gridViewCategorised.BestFitColumns();
            gridViewInspected.BestFitColumns();
            gridViewCompleted.BestFitColumns();
            gridViewClosed.BestFitColumns();
        }

        /// <summary>
        /// Determines which WBS/Tag to be added into the master list
        /// </summary>
        private void FilterAndShowWBSTag()
        {
            _filterWBSTagDisplay.Clear();
            populateWBSTagDiscipline();
            List<wbsTagDisplay> tempChildrens = new List<wbsTagDisplay>();
            if (_filterWBSTag.Count > 0)
            {
                foreach (wbsTagDisplay wbsTagDisplay in _allWBSTagDisplay)
                {
                    if (_filterWBSTag.Any(obj => obj == wbsTagDisplay.wbsTagDisplayGuid))
                    {
                        tempChildrens.Add(wbsTagDisplay);
                    }
                }
            }
            else
            {
                foreach (wbsTagDisplay wbsTagDisplay in _allWBSTagDisplay)
                {
                    //process tag numbers first
                    if (wbsTagDisplay.showOnPunchlistBrowser)
                        tempChildrens.Add(wbsTagDisplay);
                }
            }

            splashScreenManager2.ShowWaitForm();
            splashScreenManager2.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, tempChildrens.Count);
            splashScreenManager2.SetWaitFormCaption("Loading WBS...");

            //get wbs parents for each tag numbers
            foreach (wbsTagDisplay wbsTagDisplay in tempChildrens)
            {
                //add wbs/tag number regardless
                addUniqueEntries(_filterWBSTagDisplay, wbsTagDisplay);
                //gets all the tag/wbs parent and add it
                IEnumerable<wbsTagDisplay> wbsParents = Common.AllParent(wbsTagDisplay.wbsTagDisplayParentGuid, _allWBSTagDisplay);
                foreach (wbsTagDisplay wbsParent in wbsParents)
                {
                    if (!_filterWBSTagDisplay.Any(x => x.wbsTagDisplayGuid == wbsParent.wbsTagDisplayGuid))
                    {
                        addUniqueEntries(_filterWBSTagDisplay, wbsParent);
                    }
                }

                splashScreenManager2.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
            }

            splashScreenManager2.CloseWaitForm();

            TreeListNode saveNode = null;
            //TreeListMultiSelection multiSelection = null;
            if (treeListWBSTag.Selection.Count > 0)
                saveNode = treeListWBSTag.FocusedNode;

            treeListWBSTag.RefreshDataSource();
            treeListWBSTag.ExpandAll();

            if (saveNode != null)
                treeListWBSTag.FocusedNode = saveNode;
            else
                deselectAllWBSTag();
        }

        private void addUniqueEntries(List<wbsTagDisplay> wbsTagDisplays, wbsTagDisplay wbsTagDisplay)
        {
            if (!wbsTagDisplays.Any(x => x.wbsTagDisplayGuid == wbsTagDisplay.wbsTagDisplayGuid))
                wbsTagDisplays.Add(wbsTagDisplay);
        }

        private void populateWBSTagDiscipline()
        {
            using (AdapterSCHEDULE daSchedule = new AdapterSCHEDULE())
            {
                dsSCHEDULE.SCHEDULEDataTable dtSchedule = daSchedule.GetByProject(_projectGuid);

                foreach (wbsTagDisplay wbsTagDisplay in _allWBSTagDisplay)
                {
                    string wbsTagDiscipline = string.Empty;
                    dsSCHEDULE.SCHEDULERow drSchedule = dtSchedule.FirstOrDefault(x => x.GUID == wbsTagDisplay.wbsTagDisplayScheduleGuid);
                    if (drSchedule != null)
                        wbsTagDiscipline = drSchedule.DISCIPLINE;

                    wbsTagDisplay.wbsTagDisplayDiscipline = wbsTagDiscipline;
                }
            }
        }

        /// <summary>
        /// Refresh punchlist from selected Tag
        /// </summary>
        private void RefreshAllPunchlist(List<Punchlist> PunchlistMaster)
        {
            _allStatus.Clear();
            _allNew.Clear();
            _allCategorised.Clear();
            _allInspected.Clear();
            _allApproved.Clear();
            _allCompleted.Clear();
            _allClosed.Clear();

            var selectedNodes = treeListWBSTag.Selection;
            List<wbsTagDisplay> selectedWBSTag = new List<wbsTagDisplay>();

            #region Establishing schedule for query
            //if there are no specific selection, select all
            if (treeListWBSTag.Selection.Count == 0)
                selectedWBSTag = _filterWBSTagDisplay;
            else
            {
                foreach (TreeListNode selectedNode in selectedNodes)
                {
                    selectedWBSTag.Add((wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(selectedNode));
                }
            }
            #endregion

            #region Retrieving saved and pending
            List<wbsTagDisplay> uniqueWBSTags = new List<wbsTagDisplay>();
            foreach (wbsTagDisplay wbsTag in selectedWBSTag)
            {
                foreach (wbsTagDisplay relatedWbsTagDisplay in Common.GetChildWBSTagDisplays(_allWBSTagDisplay, wbsTag))
                    Common.AddUniqueWBSTag(uniqueWBSTags, relatedWbsTagDisplay);
            }

            foreach(wbsTagDisplay uniqueWBSTag in uniqueWBSTags)
            {
                Guid wbsTagGuid = Guid.Empty; //if all else fail, empty GUID will be queried and subsequently null will be returned to skip the ITR retrieving process
                List<Punchlist> wbsTagPunchlists = new List<Punchlist>();

                if (uniqueWBSTag.wbsTagDisplayAttachTag != null)
                    wbsTagPunchlists = PunchlistMaster.Where(obj => obj.punchlistAttachTag != null && obj.punchlistAttachTag.GUID == uniqueWBSTag.wbsTagDisplayAttachTag.GUID).ToList();
                else if (uniqueWBSTag.wbsTagDisplayAttachWBS != null)
                    wbsTagPunchlists = PunchlistMaster.Where(obj => obj.punchlistAttachWBS != null && obj.punchlistAttachWBS.GUID == uniqueWBSTag.wbsTagDisplayAttachWBS.GUID).ToList();

                //retrieve saved punchlist
                if (wbsTagPunchlists.Count > 0)
                {
                    foreach (Punchlist wbsTagPunchlist in wbsTagPunchlists)
                    {
                        _allStatus.Add(wbsTagPunchlist);
                    }
                }
            }
            #endregion

            PopulateStatuses(_allStatus);
            RefreshAllStatusDataGrid();
        }

        /// <summary>
        /// Add one new punchlist
        /// </summary>
        private void AddOnePunchlist(Punchlist punchlist)
        {
            punchlist.punchlistStatus = -1;
            _masterPunchlist.Add(punchlist);
            _allStatus.Add(punchlist);
            _allNew.Add(punchlist);

            CountAllStatusTab();
            RefreshAllStatusDataGrid();
        }

        /// <summary>
        /// Remove one punchlist from the status lists
        /// </summary>
        private void RemoveOnePunchlist(Punchlist punchlist)
        {
            _masterPunchlist.Remove(punchlist);
            _allStatus.Remove(punchlist);
            RemovePunchlistFromStatusList(punchlist);

            CountAllStatusTab();
            RefreshAllStatusDataGrid();
        }

        /// <summary>
        /// Refresh the status of one punchlist
        /// </summary>
        private void RefreshOnePunchlist(Punchlist punchlist)
        {
            RemovePunchlistFromStatusList(punchlist);
            SetPunchlistDisplayIndexAndAddToList(punchlist);

            CountAllStatusTab();
            RefreshAllStatusDataGrid();
        }

        /// <summary>
        /// Split all saved ITR to their relevant statuses
        /// </summary>
        private void PopulateStatuses(List<Punchlist> allStatuses)
        {
            foreach (Punchlist punchlist in allStatuses)
            {
                SetPunchlistDisplayIndexAndAddToList(punchlist);
            }

            CountAllStatusTab();
        }

        /// <summary>
        /// Set the display icon for the iTR and add it to the corresponding list
        /// </summary>
        private void SetPunchlistDisplayIndexAndAddToList(Punchlist punchlist)
        {
            dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPunchlistStatus = _daPunchlistStatus.GetLastTwoSequenceByPunchlist(punchlist.GUID);
            if (dtPunchlistStatus != null)
            {
                dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow drCurrentStatus = dtPunchlistStatus[0];
                if (dtPunchlistStatus.Rows.Count == 2)
                {
                    dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow drPreviousStatus = dtPunchlistStatus[1];

                    //dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUERow drPunchlistIssue = _daPunchlistStatusIssue.GetLatestBy(drPreviousStatus.GUID);
                    dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUERow drPunchlistIssue = _daPunchlistStatusIssue.GetLatestBy(drCurrentStatus.GUID);
                    if ((int)drCurrentStatus.STATUS_NUMBER < (int)drPreviousStatus.STATUS_NUMBER)
                    {
                        if (drPunchlistIssue == null || drPunchlistIssue.COMMENTS == string.Empty)
                            punchlist.punchlistImageIndex = 1; //rejected without comments
                        else
                            punchlist.punchlistImageIndex = 4; //rejected with comments
                    }
                    else
                    {
                        if (drPunchlistIssue == null || drPunchlistIssue.COMMENTS == string.Empty)
                            punchlist.punchlistImageIndex = 2; //progressed without comments
                        else
                            punchlist.punchlistImageIndex = 3; //progressed with comments
                    }
                }
                else
                {
                    punchlist.punchlistImageIndex = 0; //new status
                }

                AddPunchlistToStatusList(punchlist, (int)drCurrentStatus.STATUS_NUMBER);
            }
            else
            {
                punchlist.punchlistImageIndex = 0; //new status
                punchlist.punchlistStatusName = "New";
                punchlist.punchlistStatus = -1;
                _allNew.Add(punchlist);
            }
        }

        /// <summary>
        /// Remove the punchlist from all status list
        /// </summary>
        private void RemovePunchlistFromStatusList(Punchlist punchlist)
        {
            _allNew.Remove(punchlist);
            _allCategorised.Remove(punchlist);
            _allInspected.Remove(punchlist);
            _allApproved.Remove(punchlist);
            _allCompleted.Remove(punchlist);
            _allClosed.Remove(punchlist);
        }

        /// <summary>
        /// Add the Punchlist to corresponding status list
        /// </summary>
        private void AddPunchlistToStatusList(Punchlist punchlist, int statusNumber)
        {
            //shove ITR to corresponding status lists
            if (statusNumber <= -1)
            {
                punchlist.punchlistStatusName = "New";
                _allNew.Add(punchlist);
            }
            else if (statusNumber == (int)Punchlist_Status.Categorised)
            {
                punchlist.punchlistStatusName = Punchlist_Status.Categorised.ToString();
                _allCategorised.Add(punchlist);
            }
            else if (statusNumber == (int)Punchlist_Status.Inspected)
            {
                punchlist.punchlistStatusName = Punchlist_Status.Inspected.ToString();
                _allInspected.Add(punchlist);
            }
            else if (statusNumber == (int)Punchlist_Status.Approved)
            {
                punchlist.punchlistStatusName = Punchlist_Status.Approved.ToString();
                _allApproved.Add(punchlist);
            }
            else if (statusNumber == (int)Punchlist_Status.Completed)
            {
                punchlist.punchlistStatusName = Punchlist_Status.Completed.ToString();
                _allCompleted.Add(punchlist);
            }
            else if (statusNumber >= (int)Punchlist_Status.Closed)
            {
                punchlist.punchlistStatusName = Punchlist_Status.Closed.ToString();
                _allClosed.Add(punchlist);
            }

            punchlist.punchlistStatus = statusNumber;
        }

        /// <summary>
        /// Populate the tab with status count
        /// </summary>
        private void CountAllStatusTab()
        {
            //write status count
            int countStatus = _allNew.Count();
            xtraTabPageNew.Text = "New [" + countStatus + "]";

            countStatus = _allCategorised.Count();
            xtraTabPageCategorised.Text = "Categorised [" + countStatus + "]";

            countStatus = _allInspected.Count();
            xtraTabPageInspected.Text = "Inspected [" + countStatus + "]";

            //countStatus = _allApproved.Count();
            //xtraTabPageApproved.Text = "Approved [" + countStatus + "]";

            countStatus = _allCompleted.Count();
            xtraTabPageCompleted.Text = "Completed [" + countStatus + "]";

            countStatus = _allClosed.Count();
            xtraTabPageClosed.Text = "Closed [" + countStatus + "]";
        }

        /// <summary>
        /// Refreshes all the status tree list to reflected updated bind data
        /// </summary>
        private void RefreshAllStatusDataGrid()
        {
            gridControlNew.RefreshDataSource();
            gridControlCategorised.RefreshDataSource();
            gridControlInspected.RefreshDataSource();
            gridControlCompleted.RefreshDataSource();
            gridControlClosed.RefreshDataSource();
            gridControlExcelExport.RefreshDataSource();
        }
        #endregion

        #region Helpers
        private void ExportToExcel()
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.RootFolder = Environment.SpecialFolder.Desktop;
            string outputFileName = System_Environment.GetUser().userProject.Label + "_Punchlist";

            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    //customRichEdit1.RemoveAllRangePermissions(); //in DevExpress 15.2 range permission's color are exported to PDF even though it's not displayed, removing it will solve the issue but the signature permission will be removed as well
                    gridControlExcelExport.ExportToXls(fd.SelectedPath + "\\" + outputFileName + ".xls");
                }
                catch(Exception e)
                {
                    Common.Warn(e.Message);
                }
            }
        }

        private void exportIndividual(bool print)
        {
            GridView focusedGridView = GetFocusedGridView();
            if (focusedGridView.SelectedRowsCount == 0)
            {
                Common.Warn("Please select a punchlist to export");
                return;
            }

            int[] selectedRowIndexes = focusedGridView.GetSelectedRows();
            List<Punchlist> selectedPunchlists = new List<Punchlist>();
            foreach (int selectedRowIndex in selectedRowIndexes)
                selectedPunchlists.Add((Punchlist)focusedGridView.GetRow(selectedRowIndex));

            string selectedFolder = string.Empty;
            string processingMessage = print ? "Printing" : "Exporting";
            if (!print)
            {
                FolderBrowserDialog fDialog = new FolderBrowserDialog();
                fDialog.RootFolder = Environment.SpecialFolder.Desktop;
                if (fDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectedFolder = fDialog.SelectedPath;
                }
                else
                    return;
            }

            int i = 1;
            int totalCount = selectedPunchlists.Count;
            splashScreenManager1.ShowWaitForm();
            foreach (Punchlist seletedPunchlist in selectedPunchlists)
            {
                using (frmPunchlist_Print_Single f = new frmPunchlist_Print_Single(seletedPunchlist))
                {
                    splashScreenManager1.SetWaitFormDescription(string.Concat(processingMessage, " ", i.ToString(), " of ", totalCount.ToString(), " record(s)"));
                    if (print)
                        f.Print();
                    else
                        f.ExportToPDF(selectedFolder);
                }

                i++;
            }
            splashScreenManager1.CloseWaitForm();

            if (i > 0)
                Common.Prompt(string.Concat((i-1).ToString(), " punchlist(s) exported"));
        }

        private void Export(bool print)
        {
            GridView focusedGridView = GetFocusedGridView();

            if (focusedGridView.SelectedRowsCount == 0)
            {
                Common.Warn("Please select a punchlist to export");
                return;
            }

            int[] selectedRowIndexes = focusedGridView.GetSelectedRows();
            List<Punchlist> selectedPunchlist = new List<Punchlist>();

            foreach (int selectedRowIndex in selectedRowIndexes)
            {
                selectedPunchlist.Add((Punchlist)focusedGridView.GetRow(selectedRowIndex));
            }

            frmPunchlist_Print f = new frmPunchlist_Print(selectedPunchlist);
            if (f.CanPrint())
            {
                splashScreenManager1.ShowWaitForm();
                if (print)
                {
                    splashScreenManager1.SetWaitFormDescription("Printing ...");
                    f.Print();
                    splashScreenManager1.CloseWaitForm();
                }
                else
                {
                    splashScreenManager1.SetWaitFormDescription("Exporting ...");
                    bool exported = f.ExportToPDF();
                    splashScreenManager1.CloseWaitForm();

                    if(exported)
                        Common.Prompt("Punchlist Exported");
                }
            }
        }

        private void EstablishMasterPunchlist(List<wbsTagDisplay> fullWBSTagDisplay, Guid projectGuid, List<string> disciplines = null, List<string> subSystemNames = null, List<string> systemNames = null, List<string> areaNames = null, string strDescription = "", List<string> categories = null, ProjectLibrary.SearchMode searchMode = ProjectLibrary.SearchMode.Contains)
        {
            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable dtPunchlistMaster = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable();
            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable dtPunchlistsByTag = _daPunchlist.GetAllTagAndSystemByProjectDiscipline(projectGuid, disciplines, subSystemNames, systemNames, areaNames, strDescription, categories, searchMode);
            if (dtPunchlistsByTag != null)
            {
                dtPunchlistMaster.Merge(dtPunchlistsByTag);
                dtPunchlistsByTag.Dispose();
            }

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable dtPunchlistsByWBS = _daPunchlist.GetAllWBSSystemByProjectDiscipline(projectGuid, disciplines, subSystemNames, systemNames, areaNames, strDescription, categories, searchMode);
            if (dtPunchlistsByWBS != null)
            {
                dtPunchlistMaster.Merge(dtPunchlistsByWBS);
                dtPunchlistsByWBS.Dispose();
            }

            _masterPunchlist.Clear();

            fullWBSTagDisplay.ForEach(x => x.showOnPunchlistBrowser = false);

            foreach (dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemRow drPunchlistMaster in dtPunchlistMaster.Rows)
            {
                wbsTagDisplay findWBSTag = fullWBSTagDisplay.FirstOrDefault(obj => obj.wbsTagDisplayGuid == drPunchlistMaster.WBSTAGGUID);
                if(findWBSTag != null)
                {
                    findWBSTag.showOnPunchlistBrowser = true;
                    //NewfindWBSTag.wbsTagDisplayEnabled = true;
                    _masterPunchlist.Add(Common.CreatePunchlistTagWBS(findWBSTag, _allWBSTagDisplay, drPunchlistMaster, true));
                }
            }
        }

        /// <summary>
        /// Retrieve all comments for focused node on focused gridview
        /// </summary>
        private bool? RefreshComments()
        {
            GridView focusedGridView = GetFocusedGridView();

            if (focusedGridView.SelectedRowsCount == 0)
            {
                Common.Warn("Please select a punchlist to edit");
                return false;
            }

            Common.GetPunchlistStatusComments(((Punchlist)focusedGridView.GetFocusedRow()).GUID, _allComments, out _commentingPunchlistStatus);
            treeListComments.RefreshDataSource();
            treeListComments.BestFitColumns();
            treeListComments.ExpandAll();
            treeListComments.MoveLast();

            if (_commentingPunchlistStatus == Guid.Empty)
                return null;
            else
                return true;
        }

        /// <summary>
        /// Open up focused punchlist
        /// </summary>
        private void OpenPunchlist()
        {
            flyoutPanel.HidePopup();
            GridView selectedGridView = GetFocusedGridView();
            if (selectedGridView.SelectedRowsCount == 0)
            {
                Common.Warn("Please select a punchlist to edit");
                return;
            }

            int[] i = selectedGridView.GetSelectedRows(); //need to remember the selected row index to restore it later
            splashScreenManager1.ShowWaitForm();
            frmPunchlist_Main frmPunchlistAdd = new frmPunchlist_Main((Punchlist)selectedGridView.GetFocusedRow(), _allWBSTagDisplay, Update_Edit_Punchlist);
            frmPunchlistAdd.Show();
            splashScreenManager1.CloseWaitForm();

            selectedGridView.FocusedRowHandle = i[0];
        }

        private void Update_Edit_Punchlist(frmPunchlist_Main frmPunchlistEdit)
        {
            Punchlist editPunchlist = frmPunchlistEdit.GetPunchlist();
            if (editPunchlist == null)
                return;

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlist = _daPunchlist.GetBy(editPunchlist.GUID);
            if (drPunchlist != null)
            {
                if (editPunchlist.punchlistTitle.Length > 100)
                    drPunchlist.TITLE = editPunchlist.punchlistTitle.Substring(0, 100);
                else
                    drPunchlist.TITLE = editPunchlist.punchlistTitle;

                drPunchlist.DESCRIPTION = editPunchlist.punchlistDescription;
                drPunchlist.DISCIPLINE = editPunchlist.punchlistDiscipline;
                drPunchlist.REMEDIAL = editPunchlist.punchlistRemedial;
                drPunchlist.CATEGORY = editPunchlist.punchlistCategory;
                drPunchlist.ACTIONBY = editPunchlist.punchlistActionBy;
                drPunchlist.PRIORITY = editPunchlist.punchlistPriority;
                drPunchlist.UPDATED = DateTime.Now;
                drPunchlist.UPDATEDBY = System_Environment.GetUser().GUID;
                _daPunchlist.Save(drPunchlist);

                if (frmPunchlistEdit.AskForProgression())
                    CheckForAutoProgress(drPunchlist, editPunchlist.punchlistStatus);

                //pictures are managed within Punchlist Form
                //splashScreenManager1.ShowWaitForm();
                //List<Image> punchlistPictures = frmPunchlistEdit.GetPunchlistImages();
                //_daPunchlistPicture.SavePunchlistPictures(drPunchlist.GUID, punchlistPictures, System_Environment.GetUser().GUID);
                //splashScreenManager1.CloseWaitForm();
                RefreshOnePunchlist(editPunchlist);
            }
            else
            {
                //if we are deleting the last punchlist for this wbstag
                wbsTagDisplay findWBSTag;
                if (editPunchlist.punchlistAttachTag != null)
                {
                    List<Punchlist> findPunchlists = _masterPunchlist.Where(obj => (obj.punchlistAttachTag != null && obj.punchlistAttachTag.GUID == editPunchlist.punchlistAttachTag.GUID)).ToList();
                    if (findPunchlists.Count == 1)
                    {
                        findWBSTag = _allWBSTagDisplay.FirstOrDefault(obj => obj.wbsTagDisplayAttachTag != null && obj.wbsTagDisplayAttachTag.GUID == editPunchlist.punchlistAttachTag.GUID);
                        //NewfindWBSTag.wbsTagDisplayEnabled = false;
                        FilterAndShowWBSTag();
                    }
                }
                else if (editPunchlist.punchlistAttachWBS != null)
                {
                    List<Punchlist> findPunchlists = _masterPunchlist.Where(obj => (obj.punchlistAttachWBS != null && obj.punchlistAttachWBS.GUID == editPunchlist.punchlistAttachWBS.GUID)).ToList();
                    if (findPunchlists.Count == 1)
                    {
                        findWBSTag = _allWBSTagDisplay.FirstOrDefault(obj => obj.wbsTagDisplayAttachWBS != null && obj.wbsTagDisplayAttachWBS.GUID == editPunchlist.punchlistAttachWBS.GUID);
                        //NewfindWBSTag.wbsTagDisplayEnabled = false;
                        FilterAndShowWBSTag();
                    }
                }

                RemoveOnePunchlist(editPunchlist);
            }
        }

        /// <summary>
        /// Retrieve the focused grid
        /// </summary>
        private GridView GetFocusedGridView()
        {
            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageNew)
                return gridViewNew;
            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageCategorised)
                return gridViewCategorised;
            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageInspected)
                return gridViewInspected;
            //if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageApproved)
            //    return gridViewApproved;
            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageCompleted)
                return gridViewCompleted;
            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageClosed)
                return gridViewClosed;

            return null;
        }

        /// <summary>
        /// Retrieve the focused grid control
        /// </summary>
        private GridControl GetFocusedGridControl()
        {
            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageNew)
                return gridControlNew;
            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageCategorised)
                return gridControlCategorised;
            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageInspected)
                return gridControlInspected;
            //if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageApproved)
            //    return gridControlApproved;
            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageCompleted)
                return gridControlCompleted;
            if (xtraTabControlStatuses.SelectedTabPage == xtraTabPageClosed)
                return gridControlClosed;

            return null;

        }
        /// <summary>
        /// Deselect all tag from treeListWBSTag
        /// </summary>
        private void deselectAllWBSTag()
        {
            //int selectionCount = treeListWBSTag.Selection.Count;
            treeListWBSTag.Selection.UnselectAll();
            //for (int i = 0; i < selectionCount; i++)
            //{
            //    treeListWBSTag.Selection.RemoveAt(0);
            //}
        }

        /// <summary>
        /// Looks at punchlist main to determine whether its eligible and perform progression on confirmation
        /// </summary>
        private bool CheckForAutoProgress(dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlist, int punchlistStatus)
        {
            if(punchlistStatus == -1)
            {
                if (System_Environment.HasPrivilege(PrivilegeTypeID.MarkPunchlistCategorised) && drPunchlist.ACTIONBY != string.Empty && drPunchlist.PRIORITY != string.Empty)
                {
                    //auto mark punchlists as categorised
                    //if (Common.Confirmation("Punchlist is categorised, do you want to mark it as categorised?", "Progress Punchlist"))
                    //{
                        _daPunchlistStatus.ChangeStatus(drPunchlist.GUID, (Punchlist_Status)punchlistStatus, true, Guid.NewGuid());
                        return true;
                    //}
                }
            }
            else if(punchlistStatus == 0 && System_Environment.HasPrivilege(PrivilegeTypeID.MarkPunchlistInspected))
            {
                if (drPunchlist.REMEDIAL != string.Empty)
                {
                    //if (Common.Confirmation("Punchlist is inspected, do you want to mark it as inspected?", "Progress Punchlist"))
                    //{
                        _daPunchlistStatus.ChangeStatus(drPunchlist.GUID, (Punchlist_Status)punchlistStatus, true, Guid.NewGuid());
                        return true;
                    //}
                }
            }

            return false;
        }
        #endregion

        #region Events
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            //txtSearchTag.Text = string.Empty;
            FilterAndShowWBSTag();

            treeListWBSTag.Selection.Clear();
            RefreshAllPunchlist(_masterPunchlist);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Export(true);
        }

        private void btnPrintIndividual_Click(object sender, EventArgs e)
        {
            exportIndividual(true);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            Export(false);
        }

        private void btnExportIndividual_Click(object sender, EventArgs e)
        {
            exportIndividual(false);
        }

        private void btnComment_Click(object sender, EventArgs e)
        {
            AdapterPUNCHLIST_STATUS_ISSUE daPunchlistIssue = new AdapterPUNCHLIST_STATUS_ISSUE();
            dsPUNCHLIST_STATUS_ISSUE dsPunchlistIssue = new dsPUNCHLIST_STATUS_ISSUE();

            try
            {
                dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUERow drPunchlistIssue = dsPunchlistIssue.PUNCHLIST_STATUS_ISSUE.NewPUNCHLIST_STATUS_ISSUERow();
                daPunchlistIssue.AddComments(_commentingPunchlistStatus, txtComments.Text.Trim(), false);
                txtComments.Text = string.Empty;

                RefreshComments();
            }
            finally
            {
                daPunchlistIssue.Dispose();
                dsPunchlistIssue.Dispose();
            }
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            GridControl focusedGridControl = GetFocusedGridControl();

            flyoutPanel.OwnerControl = focusedGridControl;
            bool? refreshResult = RefreshComments();
            if (refreshResult == true)
            {
                flowLayoutPanel1.Visible = true;
                flyoutPanel.ShowPopup();
            }
            //if it's a "New Only" status, currently not used because commenting on status button is intentionally disabled
            else if (refreshResult == null)
            {
                flowLayoutPanel1.Visible = false;
                flyoutPanel.ShowPopup();
            }
        }

        private void treeListWBSTag_MouseUp(object sender, MouseEventArgs e)
        {
            var hitInfo = treeListWBSTag.CalcHitInfo(new Point(e.X, e.Y));
            if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Empty)
                deselectAllWBSTag();
        }

        private void treeListWBSTag_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                deselectAllWBSTag();
                RefreshAllPunchlist(_masterPunchlist);
            }
        }

        private void treeListWBSTag_Click(object sender, EventArgs e)
        {
            flyoutPanel.HidePopup();
            RefreshAllPunchlist(_masterPunchlist);
        }

        private void GridView_DoubleClick(object sender, EventArgs e)
        {
            OpenPunchlist();
        }

        private void txtSearchTag_EditValueChanged(object sender, EventArgs e)
        {
            FilterAndShowWBSTag();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            OpenPunchlist();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if(!System_Environment.HasPrivilege(PrivilegeTypeID.CreatePunchlist))
            {
                Common.Warn("You are not authorised to create a punchlist");
                return;
            }

            wbsTagDisplay selectedWBSTag = (wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(treeListWBSTag.FocusedNode);

            //this line follows WBS discipline
            //frmPunchlist_Main frmPunchlistAdd = new frmPunchlist_Main(_allWBSTagDisplay, selectedWBSTag, Guid.Empty, selectedWBSTag == null ? cmbDiscipline.Text : selectedWBSTag.wbsTagDisplayDiscipline, Add_Punchlist);
            
            if(WBSSearch.SelectedDisciplines.Count == 0 || WBSSearch.SelectedDisciplines.All(x => x == ""))
            {
                Common.Warn("Please select a single discipline for punchlist creation");
                return;
            }

            if(WBSSearch.SelectedDisciplines.Count > 1)
            {
                Common.Warn("Multiple disciplines selected, please select only a single discipline for punchlist creation");
                return;
            }

            //this line follows default discipline
            frmPunchlist_Main frmPunchlistAdd = new frmPunchlist_Main(_allWBSTagDisplay, selectedWBSTag, Guid.Empty, WBSSearch.SelectedDisciplines.First(), Add_Punchlist);

            frmPunchlistAdd.Show();
        }

        private void Add_Punchlist(frmPunchlist_Main frmPunchlist)
        {
            Punchlist newPunchlist = frmPunchlist.GetPunchlist();
            if (newPunchlist == null)
                return;

            List<Image> punchlistInspectionPictures = frmPunchlist.GetPunchlistImages(PunchlistImageType.Inspection);
            List<Image> punchlistRemedialPictures = frmPunchlist.GetPunchlistImages(PunchlistImageType.Remedial);
            SaveNewPunchlist(newPunchlist, punchlistInspectionPictures, punchlistRemedialPictures);
        }

        private void SaveNewPunchlist(Punchlist newPunchlist)
        {
            SaveNewPunchlist(newPunchlist, null, null);
        }

        private void SaveNewPunchlist(Punchlist newPunchlist, List<Image> punchlistInspectionPictures = null, List<Image> punchlistRemedialPictures = null)
        {
            dsPUNCHLIST_MAIN dsPunchlist = new dsPUNCHLIST_MAIN();
            dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlist = dsPunchlist.PUNCHLIST_MAIN.NewPUNCHLIST_MAINRow();

            if(newPunchlist.punchlistTagGuid != null)
            {
                drPunchlist.TAG_GUID = (Guid)newPunchlist.punchlistTagGuid;
                drPunchlist.SEQUENCE_NUMBER = 0;
            }
            else if(newPunchlist.punchlistWBSGuid != null)
            {
                drPunchlist.WBS_GUID = (Guid)newPunchlist.punchlistWBSGuid;
                drPunchlist.SEQUENCE_NUMBER = 0;
            }
            else if (newPunchlist.punchlistAttachTag != null)
            {
                drPunchlist.TAG_GUID = newPunchlist.punchlistAttachTag.GUID;
                drPunchlist.SEQUENCE_NUMBER = _daPunchlist.GetSequence(drPunchlist.TAG_GUID) + 1;
            }
            else if (newPunchlist.punchlistAttachWBS != null)
            {
                drPunchlist.WBS_GUID = newPunchlist.punchlistAttachWBS.GUID;
                drPunchlist.SEQUENCE_NUMBER = _daPunchlist.GetSequence(drPunchlist.WBS_GUID) + 1;
            }

            int Max_DB_Len = 90;
            if (newPunchlist.punchlistTitle.Length < Max_DB_Len)
                Max_DB_Len = newPunchlist.punchlistTitle.Length;

            string punchlistTitle = newPunchlist.punchlistTitle.Substring(0, Max_DB_Len);

            drPunchlist.GUID = newPunchlist.GUID;
            if (newPunchlist.punchlistITR != null)
                drPunchlist.ITR_GUID = (Guid)newPunchlist.punchlistITR.ITRGuid;
            else
                drPunchlist.ITR_GUID = Guid.Empty;

            string punchlistUniqueNumber = newPunchlist.punchlistItem;
            if (punchlistUniqueNumber == null || punchlistUniqueNumber == string.Empty)
                punchlistUniqueNumber = Toggle_Acceptance.Punchlisted.ToString() + Variables.punchlistAffix + Common.GetProjectPunchlistCount(string.Concat(punchlistTitle, newPunchlist.punchlistDescription, Guid.NewGuid().ToString())).ToString();

            //drPunchlist.ITR_PUNCHLIST_ITEM = newPunchlist.punchlistItem;
            drPunchlist.TITLE = newPunchlist.punchlistTitle.Substring(0, Max_DB_Len);
            drPunchlist.DESCRIPTION = newPunchlist.punchlistDescription;
            drPunchlist.ITR_PUNCHLIST_ITEM = punchlistUniqueNumber;
            drPunchlist.REMEDIAL = newPunchlist.punchlistRemedial;
            drPunchlist.DISCIPLINE = newPunchlist.punchlistDiscipline;
            drPunchlist.CATEGORY = newPunchlist.punchlistCategory;
            drPunchlist.ACTIONBY = newPunchlist.punchlistActionBy;
            drPunchlist.PRIORITY = newPunchlist.punchlistPriority;
            drPunchlist.CREATED = DateTime.Now;
            drPunchlist.CREATEDBY = System_Environment.GetUser().GUID;
            dsPunchlist.PUNCHLIST_MAIN.AddPUNCHLIST_MAINRow(drPunchlist);
            _daPunchlist.Save(drPunchlist);

            splashScreenManager1.ShowWaitForm();
            if(punchlistInspectionPictures != null)
                _daPunchlistPicture.SavePunchlistPictures(drPunchlist.GUID, punchlistInspectionPictures, PunchlistImageType.Inspection, true);

            if(punchlistRemedialPictures != null)
                _daPunchlistPicture.SavePunchlistPictures(drPunchlist.GUID, punchlistRemedialPictures, PunchlistImageType.Remedial, true);
            splashScreenManager1.CloseWaitForm();

            AddOnePunchlist(newPunchlist);
            if (CheckForAutoProgress(drPunchlist, -1))
                RefreshOnePunchlist(newPunchlist);

            FilterAndShowWBSTag();
        }
        #endregion

        protected override void OnClosed(EventArgs e)
        {
            _daPunchlist.Dispose();
            _daPunchlistStatus.Dispose();
            _daPunchlistStatusIssue.Dispose();
            _daTag.Dispose();
            _daWBS.Dispose();
            base.OnClosed(e);
        }

        private void gridView_MouseUp(object sender, MouseEventArgs e)
        {
            GridView focusedGridView = GetFocusedGridView();
            var hitInfo = focusedGridView.CalcHitInfo(new Point(e.X, e.Y));

            if (hitInfo.InRow)
            {
                Punchlist hitPunchlist = (Punchlist)focusedGridView.GetFocusedRow();

                if (hitPunchlist.punchlistImageIndex == 3 || hitPunchlist.punchlistImageIndex == 4)
                {
                    GridControl focusedGridControl = GetFocusedGridControl();
                    flyoutPanel.OwnerControl = focusedGridControl;
                    RefreshComments();
                    flyoutPanel.ShowPopup();
                }
                else
                    flyoutPanel.HidePopup();
            }
            else
                flyoutPanel.HidePopup();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.ImportPunchlist))
            {
                MessageBox.Show("You do not have authority to import punchlist");
                return;
            }

            if(_defaultDiscipline == Variables.allDiscipline)
            {
                MessageBox.Show("Punchlist must be imported by discipline, please choose a discipline");
                return;
            }

            frmPunchlistImportSheet importSheet = new frmPunchlistImportSheet(_projectGuid, _defaultDiscipline, SaveNewPunchlist);
            importSheet.Show();
        }

        private void btnExpandAll_Click(object sender, EventArgs e)
        {
            treeListWBSTag.ExpandAll();
        }

        private void btnCollapseAll_Click(object sender, EventArgs e)
        {
            treeListWBSTag.CollapseAll();
        }

        private void barManager1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            GridView view = gridControlNew.FocusedView as GridView;
            Punchlist_Category punchlist_Category;

            using (frmPunchlist_Main frmPunchlistAdd = new frmPunchlist_Main((Punchlist)view.GetFocusedRow(), _allWBSTagDisplay, Update_Edit_Punchlist))
            {
                if (column.ToString().ToUpper() == "CATEGORY")
                {
                    string selectedOption = Common.ReplaceSpacesWith_(e.Item.Caption);
                    if (Enum.TryParse<Punchlist_Category>(selectedOption, out punchlist_Category))
                    {
                        frmPunchlistAdd.SetCategory(e.Item.Caption);
                        frmPunchlistAdd.btnProgress_Click(null, null);
                    }
                }
                else if (column.ToString().ToUpper() == "PRIORITY")
                {
                    frmPunchlistAdd.SetPriority(e.Item.Caption);
                    frmPunchlistAdd.btnSave_Click(null, null);
                }
                else if (column.ToString().ToUpper() == "ACTIONBY")
                {
                    frmPunchlistAdd.SetActionBy(e.Item.Caption);
                    frmPunchlistAdd.btnSave_Click(null, null);
                }
            }
        }

        int rowHandle;
        GridColumn column;
        private void gridViewNew_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Point);
            if (hitInfo.InRowCell)
            {
                view.FocusedRowHandle = rowHandle = hitInfo.RowHandle;
                column = hitInfo.Column;

                popupMenu1.ItemLinks.Clear();
                if(column.ToString().ToUpper() == "CATEGORY")
                {
                    List<string> Categories = Enum.GetNames(typeof(Punchlist_Category)).ToList();
                    foreach (string category in Categories)
                    {
                        popupMenu1.ItemLinks.Add(new BarButtonItem(barManager1, Common.Replace_WithSpaces(category)));
                    }
                }
                else if(column.ToString().ToUpper() == "PRIORITY")
                {
                    List<string> Priorities = new List<string>();
                    Priorities.Add(Variables.punchlistCategoryA);
                    Priorities.Add(Variables.punchlistCategoryB);
                    Priorities.Add(Variables.punchlistCategoryC);
                    Priorities.Add(Variables.punchlistCategoryD);

                    foreach (string priority in Priorities)
                    {
                        popupMenu1.ItemLinks.Add(new BarButtonItem(barManager1, Common.ConvertDBDisciplineForDisplay(priority)));
                    }
                }
                else if(column.ToString().ToUpper() == "ACTIONBY")
                {
                    List<string> ActionBys = Enum.GetNames(typeof(Punchlist_ActionBy)).ToList();
                    foreach (string actionBy in ActionBys)
                    {
                        popupMenu1.ItemLinks.Add(new BarButtonItem(barManager1, Common.Replace_WithSpaces(actionBy)));
                    }
                }

                popupMenu1.ShowPopup(barManager1, view.GridControl.PointToScreen(e.Point));
            }
        }

        private void gridView_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView gridView = (GridView)sender;
            if (e.Column.FieldName == "punchlistTitle")
            {
                Punchlist punchlist = (Punchlist)gridView.GetRow(e.RowHandle);
                if (punchlist.punchlistImageIndex > 0)
                {
                    e.DefaultDraw();
                    Image img = imageList3.Images[punchlist.punchlistImageIndex];
                    //TODO: specify required offsets
                    e.Cache.DrawImage(img, e.Bounds.Width - 2, e.Bounds.Y + 2);
                }
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            WBSSearch.ResetWBSFilters();

            txtPunchlistDescription.Text = "";
            cmbSearchMode.SelectedIndex = 1;

            xtraTabPageNew.Text = "New [" + 0 + "]";
            xtraTabPageCategorised.Text = "Categorised [" + 0 + "]";
            xtraTabPageInspected.Text = "Inspected [" + 0 + "]";
            xtraTabPageCompleted.Text = "Completed [" + 0 + "]";
            xtraTabPageClosed.Text = "Closed [" + 0 + "]";

            _allNew.Clear();
            _allCategorised.Clear();
            _allInspected.Clear();
            _allApproved.Clear();
            _allCompleted.Clear();
            _allClosed.Clear();
            _filterWBSTagDisplay.Clear();

            gridControlNew.RefreshDataSource();
            gridControlCategorised.RefreshDataSource();
            gridControlInspected.RefreshDataSource();
            gridControlCompleted.RefreshDataSource();
            gridControlClosed.RefreshDataSource();
            treeListWBSTag.RefreshDataSource();
        }

        private void BtnLoadAll_Click(object sender, EventArgs e)
        {
            BtnReset_Click(null, null);
            WBSSearch.SelectAllDisciplines();
            btnSearch_Click(null, null);
        }

        private void xtraTabControl_MouseClick(object sender, MouseEventArgs e)
        {
            GridView focusedGridView = GetFocusedGridView();
            focusedGridView.BestFitColumns();
        }
    }
}