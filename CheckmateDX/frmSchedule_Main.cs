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

namespace CheckmateDX
{
    public partial class frmSchedule_Main : CheckmateDX.frmParent
    {
        //database
        AdapterTAG _daTag = new AdapterTAG();
        dsTAG _dsTag = new dsTAG();
        AdapterWBS _daWBS = new AdapterWBS();
        dsWBS _dsWBS = new dsWBS();
        AdapterSCHEDULE _daSchedule = new AdapterSCHEDULE();
        dsSCHEDULE _dsSchedule = new dsSCHEDULE();

        //class lists
        List<Schedule> _allSchedule = new List<Schedule>();
        List<Tag> _allTag = new List<Tag>();
        List<wbsTagDisplay> _allWBSTagDisplay = new List<wbsTagDisplay>();

        //variables
        Guid _projectGuid = Guid.Empty;
        public frmSchedule_Main()
        {
            InitializeComponent();
            //get user parameter
            _projectGuid = (Guid)System_Environment.GetUser().userProject.Value;
            PopulateFormElement(); //mainly populating the combobox
            scheduleBindingSource.DataSource = _allSchedule;
            wbsTagDisplayBindingSource.DataSource = _allWBSTagDisplay;
            timer1.Enabled = true;
            cmbDiscipline.SelectedIndexChanged += cmbDiscipline_SelectedIndexChanged;
        }

        #region Form Population

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            selectScheduleParameters();
        }

        /// <summary>
        /// Used when user is superadmin, which doesn't have default project and discipline
        /// </summary>
        private void selectScheduleParameters()
        {
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
            }

            RefreshSchedule(false);
        }

        /// <summary>
        /// Populate combobox
        /// </summary>
        private void PopulateFormElement()
        {
            Common.PopulateCmbAuthDiscipline(cmbDiscipline, true);
        }
        
        private void RefreshSchedule(bool clearWBSTag)
        {
            _allSchedule.Clear();
            if (_projectGuid == Guid.Empty)
                return;

            //retrive schedule
            dsSCHEDULE.SCHEDULEDataTable dtSchedule;
            if (cmbDiscipline.Text == Variables.allDiscipline)
                dtSchedule = _daSchedule.GetByProject(_projectGuid);
            else
                dtSchedule = _daSchedule.GetByProjectDiscipline(_projectGuid ,cmbDiscipline.Text);

            if(dtSchedule != null)
            {
                foreach(dsSCHEDULE.SCHEDULERow drSchedule in dtSchedule.Rows)
                {
                    _allSchedule.Add(new Schedule(drSchedule.GUID)
                    {
                        scheduleName = drSchedule.NAME,
                        scheduleDescription = drSchedule.DESCRIPTION,
                        scheduleDiscipline = drSchedule.DISCIPLINE,
                        scheduleProjectGuid = drSchedule.PROJECTGUID,
                        CreatedBy = drSchedule.CREATEDBY,
                        CreatedDate = drSchedule.CREATED
                    });
                }
            }

            gridControlSchedule.RefreshDataSource();

            if(clearWBSTag)
            {
                _allWBSTagDisplay.Clear();
                treeListWBSTag.RefreshDataSource();
            }
        }

        /// <summary>
        /// Refresh all tag from database
        /// </summary>
        private void RefreshWBSTags()
        {
            _allWBSTagDisplay.Clear();

            if (gridViewSchedule.SelectedRowsCount == 0)
                return;

            if (_allSchedule.Count == 0)
                return;

            //retrieve tag
            dsTAG.TAGDataTable dtTag = _daTag.GetBySchedule(((Schedule)gridViewSchedule.GetFocusedRow()).GUID);
            if (dtTag != null)
            {
                foreach (dsTAG.TAGRow drTag in dtTag.Rows)
                {
                    _allWBSTagDisplay.Add(new wbsTagDisplay(new Tag(drTag.GUID)
                    {
                        tagNumber = drTag.NUMBER,
                        tagDescription = drTag.DESCRIPTION,
                        tagParentGuid = drTag.PARENTGUID,
                        tagScheduleGuid = drTag.SCHEDULEGUID,
                        tagType1 = drTag.IsTYPE1Null() ? string.Empty: drTag.TYPE1,
                        tagType2 = drTag.IsTYPE2Null() ? string.Empty : drTag.TYPE2,
                        tagType3 = drTag.IsTYPE3Null() ? string.Empty : drTag.TYPE3
                    }));
                }
            }

            //retrieve wbs
            //dsWBS.WBSDataTable dtWBS = _daWBS.GetByProject(((Schedule)gridViewSchedule.GetFocusedRow()).GUID);
            dsWBS.WBSDataTable dtWBS = _daWBS.GetByProject(_projectGuid);
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
                    }));
                }
            }

            treeListWBSTag.RefreshDataSource();
            treeListWBSTag.ExpandAll();
        }

        #endregion

        #region Events

        private void bBtnMatrixAssign_Click(object sender, EventArgs e)
        {
            if (!Common.Confirmation("Doing this will reset all tag's template to matrix assignments\n\nDo you wish to continue?", "Matrix Assignment"))
                return;

            AdapterMATRIX_ASSIGNMENT daMATRIX_ASSIGNMENT = new AdapterMATRIX_ASSIGNMENT();
            AdapterTEMPLATE_REGISTER daTEMPLATE_REGISTER = new AdapterTEMPLATE_REGISTER();
            try
            {
                dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPEDataTable dtMATRIX = daMATRIX_ASSIGNMENT.Get_Assignments_Table(_projectGuid);
                splashScreenManager2.ShowWaitForm();
                splashScreenManager2.SetWaitFormCaption("Assigning Templates ...");
                splashScreenManager2.SendCommand(ProgressForm.WaitFormCommand.SetProgressMax, _allWBSTagDisplay.Count);
                splashScreenManager2.SendCommand(ProgressForm.WaitFormCommand.SetStep, 1);
                if (dtMATRIX != null)
                {
                    foreach (wbsTagDisplay WBSTag in _allWBSTagDisplay)
                    {
                        ProjectLibrary.Tag AssignmentTag = WBSTag.wbsTagDisplayAttachTag;
                        //Only do assignments by Tag
                        if (AssignmentTag != null)
                        {
                            //Resets the Tag to Template register
                            daTEMPLATE_REGISTER.SetNull_OnWBSTag(WBSTag);
                            string tagType1 = AssignmentTag.tagType1 == null ? string.Empty : AssignmentTag.tagType1.ToUpper();
                            assignTagMatrix(daTEMPLATE_REGISTER, dtMATRIX, WBSTag, AssignmentTag, tagType1, true);

                            string tagType2 = AssignmentTag.tagType2 == null ? string.Empty : AssignmentTag.tagType2.ToUpper();
                            assignTagMatrix(daTEMPLATE_REGISTER, dtMATRIX, WBSTag, AssignmentTag, tagType2, true);

                            string tagType3 = AssignmentTag.tagType3 == null ? string.Empty : AssignmentTag.tagType3.ToUpper();
                            assignTagMatrix(daTEMPLATE_REGISTER, dtMATRIX, WBSTag, AssignmentTag, tagType3, true);
                        }

                        splashScreenManager2.SendCommand(ProgressForm.WaitFormCommand.PerformStep, null);
                    }
                }
            }
            finally
            {
                daMATRIX_ASSIGNMENT.Dispose();
                daTEMPLATE_REGISTER.Dispose();
            }

            splashScreenManager2.CloseWaitForm();
        }

        private void assignTagMatrix(AdapterTEMPLATE_REGISTER daTEMPLATE_REGISTER, dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPEDataTable dtMATRIX_ASSIGNMENT, wbsTagDisplay WBSTag, Tag AssignmentTag, string matrixType, bool doNotTrim = false)
        {
            if (matrixType == string.Empty)
                return;

            DataTable dtGENERIC_MATRIX_TYPE;
            try
            {
                dtGENERIC_MATRIX_TYPE = dtMATRIX_ASSIGNMENT.AsEnumerable().Where(obj => obj.TYPE.ToUpper() == matrixType).CopyToDataTable();
            }
            catch
            {
                dtGENERIC_MATRIX_TYPE = null;
                //if dtGENERIC_MATRIX_TYPE is null for whatever reasons
            }

            if (dtGENERIC_MATRIX_TYPE != null)
            {
                List<Guid> AssignedTemplateGuids = new List<Guid>();
                dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPEDataTable dtMATRIX_TYPE = new dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPEDataTable();
                dtMATRIX_TYPE.Merge(dtGENERIC_MATRIX_TYPE);

                foreach (dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENT_TYPERow drMATRIX_TYPE in dtMATRIX_TYPE.Rows)
                {
                    daTEMPLATE_REGISTER.AssignTagWBSTemplate(drMATRIX_TYPE.GUID_TEMPLATE, WBSTag);
                    AssignedTemplateGuids.Add(drMATRIX_TYPE.GUID_TEMPLATE);
                }

                if(!doNotTrim)
                {
                    dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTEMPLATE_REGISTER = daTEMPLATE_REGISTER.GetByWBSTagGuid(AssignmentTag.GUID);
                    if (dtTEMPLATE_REGISTER != null)
                    {
                        foreach (dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTEMPLATE_REGISTER in dtTEMPLATE_REGISTER.Rows)
                        {
                            if (!AssignedTemplateGuids.Any(obj => obj == drTEMPLATE_REGISTER.TEMPLATE_GUID))
                            {
                                daTEMPLATE_REGISTER.RemoveBy(drTEMPLATE_REGISTER.GUID);
                            }
                        }
                    }
                }
            }
        }

        private void btnImportMasterSheet_Click(object sender, EventArgs e)
        {
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.MaintainMasterFeedSheet))
            {
                MessageBox.Show("You do not have authority to maintain master feed sheet");
                return;
            }

            Schedule selectedSchedule = (Schedule)gridViewSchedule.GetFocusedRow();
            if (selectedSchedule == null)
            {
                MessageBox.Show("Please create a schedule before clicking master feed sheet, selected schedule will be used as default schedule for import");
                return;
            }

            if (!showWarningMessage())
                return;

            OpenFileDialog thisDialog = new OpenFileDialog();
            thisDialog.Filter = "Excel (*.xlsx)|*.xlsx";
            thisDialog.FilterIndex = 1;
            thisDialog.RestoreDirectory = true;
            thisDialog.Multiselect = false;
            thisDialog.Title = "Please Select Excel File";

            if (thisDialog.ShowDialog() == DialogResult.OK)
            {
                frmMasterSheet masterSheet = new frmMasterSheet(_projectGuid, selectedSchedule.GUID, cmbDiscipline.Text, false, thisDialog.FileName);
                masterSheet.Show();
            }
        }

        private void btnMasterReport_Click(object sender, EventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            frmMaster_ITR_Report masterSheet = new frmMaster_ITR_Report(_projectGuid);
            splashScreenManager1.CloseWaitForm();
            masterSheet.Show();
        }

        private void btnITRStatusBreakdownReport_Click(object sender, EventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            frmMaster_ITR_Status_Breakdown_Report masterITRStatusBreakdownReport = new frmMaster_ITR_Status_Breakdown_Report(_projectGuid);
            splashScreenManager1.CloseWaitForm();
            masterITRStatusBreakdownReport.Show();
        }

        private void cmbDiscipline_EditValueChanged(object sender, EventArgs e)
        {
            RefreshSchedule(true);
        }

        private void bBtnBulkEdit_Click(object sender, EventArgs e)
        {
            if (gridViewSchedule.SelectedRowsCount == 0)
            {
                Common.Warn("Please select schedule to edit");
                return;
            }

            Schedule selectedSchedule = (Schedule)gridViewSchedule.GetFocusedRow();

            splashScreenManager1.ShowWaitForm();
            frmSchedule_BulkTag f = new frmSchedule_BulkTag(selectedSchedule.GUID, _projectGuid);
            splashScreenManager1.CloseWaitForm();
            f.ShowDialog();

            if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                RefreshWBSTags();
        }

        private void btnPrefill_Click(object sender, EventArgs e)
        {
            if (gridViewSchedule.SelectedRowsCount == 0)
            {
                Common.Warn("Please select schedule to prefill");
                return;
            }

            Schedule selectedSchedule = (Schedule)gridViewSchedule.GetFocusedRow();
            frmSchedule_Prefill f = new frmSchedule_Prefill(selectedSchedule, _projectGuid);
            f.ShowDialog();
        }

        private void bBtnAssignTemplate_Click(object sender, EventArgs e)
        {
            var selectedNodes = treeListWBSTag.Selection;
            List<wbsTagDisplay> selectedWBSTagDisplay = new List<wbsTagDisplay>();
            foreach (TreeListNode selectedNode in selectedNodes)
            {
                selectedWBSTagDisplay.Add((wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(selectedNode));
            }

            frmSchedule_Assign frmScheduleAssign = new frmSchedule_Assign(cmbDiscipline.Text, selectedWBSTagDisplay);
            frmScheduleAssign.ShowDialog();
        }

        private void gridControlSchedule_Click(object sender, EventArgs e)
        {
            RefreshWBSTags();
        }

        private void btnStatusOnly_Click(object sender, EventArgs e)
        {
            frmMasterSheet masterSheet = new frmMasterSheet(_projectGuid, Guid.Empty, cmbDiscipline.Text, false, string.Empty, RefreshWBSTags, true, true);
            masterSheet.Show();
        }

        private void btnMasterSheet_Click(object sender, EventArgs e)
        {
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.MaintainMasterFeedSheet))
            {
                MessageBox.Show("You do not have authority to maintain master feed sheet");
                return;
            }

            Schedule selectedSchedule = (Schedule)gridViewSchedule.GetFocusedRow();
            if(selectedSchedule == null)
            {
                MessageBox.Show("Please create a schedule before clicking master feed sheet, selected schedule will be used as default schedule for import");
                return;
            }

            if (!showWarningMessage())
                return;

            frmMasterSheet masterSheet = new frmMasterSheet(_projectGuid, selectedSchedule.GUID, cmbDiscipline.Text, false, string.Empty, RefreshWBSTags);
            masterSheet.Show();
        }

        private bool showWarningMessage()
        {
            return MessageBox.Show("If you are using excel upload, please only make changes in the excel document and not in checkmate for the particular schedule\n\nIf you plan to use checkmate feedsheet, please edit the feedsheet on a nominated device only\n\nIf you are unsure which is the nominated device\nPlease consult your supervisor or email\n\nservice.desk@primero.com.au\n\nDo you wish to continue?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK;
        }

        private void btnScheduleSheet_Click(object sender, EventArgs e)
        {
            if (!System_Environment.HasPrivilege(PrivilegeTypeID.MaintainMasterFeedSheet))
            {
                MessageBox.Show("You do not have authority to maintain schedule feed sheet");
                return;
            }

            Schedule selectedSchedule = (Schedule)gridViewSchedule.GetFocusedRow();
            if (selectedSchedule == null)
            {
                MessageBox.Show("Please select a schedule");
                return;
            }

            if (!showWarningMessage())
                return;

            frmMasterSheet masterSheet = new frmMasterSheet(_projectGuid, selectedSchedule.GUID, cmbDiscipline.Text, true, string.Empty, RefreshWBSTags);
            masterSheet.Show();
        }

        private void btnAddSchedule_Click(object sender, EventArgs e)
        {
            frmSchedule_Add frmScheduleAdd = new frmSchedule_Add();

            if(frmScheduleAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Schedule newSchedule = frmScheduleAdd.GetSchedule();
                dsSCHEDULE.SCHEDULERow drSchedule = _dsSchedule.SCHEDULE.NewSCHEDULERow();
                drSchedule.GUID = Guid.NewGuid();
                drSchedule.NAME = newSchedule.scheduleName;

                Guid projectGuid = (Guid)System_Environment.GetUser().userProject.Value;
                if (projectGuid == Guid.Empty)
                {
                    frmTool_Project frmSelectProject = new frmTool_Project();
                    frmSelectProject.ShowAsDialog();
                    if (frmSelectProject.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        projectGuid = frmSelectProject.GetSelectedProject().GUID;
                    }
                    else
                        return;
                }

                drSchedule.PROJECTGUID = projectGuid;
                drSchedule.DESCRIPTION = newSchedule.scheduleDescription;
                drSchedule.DISCIPLINE = cmbDiscipline.Text;
                drSchedule.CREATED = DateTime.Now;
                drSchedule.CREATEDBY = System_Environment.GetUser().GUID;
                _dsSchedule.SCHEDULE.AddSCHEDULERow(drSchedule);
                _daSchedule.Save(drSchedule);
                RefreshSchedule(true);
                //Common.Prompt("Schedule successfully added");
            }
        }

        private void btnAddWBS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridViewSchedule.SelectedRowsCount == 0)
            {
                Common.Warn("Please select schedule to add WBS");
                return;
            }

            frmSchedule_AddWBS frmWBSAdd;

            wbsTagDisplay selectedWBSTag = (wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(treeListWBSTag.FocusedNode);
            List<wbsTagDisplay> tempWBSTagDisplay = new List<wbsTagDisplay>(_allWBSTagDisplay); //temporary tag display for tree list selection to prevent modification to original

            if (selectedWBSTag == null || selectedWBSTag.wbsTagDisplayAttachWBS == null)
                //frmWBSAdd = new frmSchedule_AddWBS(((Schedule)gridViewSchedule.GetFocusedRow()).GUID, tempWBSTagDisplay, Guid.Empty); //Fix 12-DEC-2014: WBS is unique to project
                frmWBSAdd = new frmSchedule_AddWBS(_projectGuid, tempWBSTagDisplay, Guid.Empty);
            else
            {
                //frmWBSAdd = new frmSchedule_AddWBS(((Schedule)gridViewSchedule.GetFocusedRow()).GUID, tempWBSTagDisplay, selectedWBSTag.wbsTagDisplayGuid); //Fix 12-DEC-2014: WBS is unique to project
                frmWBSAdd = new frmSchedule_AddWBS(_projectGuid, tempWBSTagDisplay, selectedWBSTag.wbsTagDisplayGuid);
            }

            if (frmWBSAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Schedule selectedSchedule = (Schedule)gridViewSchedule.GetFocusedRow();

                WBS newWBS = frmWBSAdd.GetWBS();
                dsWBS.WBSRow drWBS = _dsWBS.WBS.NewWBSRow();
                drWBS.GUID = Guid.NewGuid();
                drWBS.PARENTGUID = newWBS.wbsParentGuid;
                drWBS.NAME = newWBS.wbsName;
                drWBS.DESCRIPTION = newWBS.wbsDescription;
                drWBS.SCHEDULEGUID = selectedSchedule.GUID;
                drWBS.CREATED = DateTime.Now;
                drWBS.CREATEDBY = System_Environment.GetUser().GUID;
                _dsWBS.WBS.AddWBSRow(drWBS);
                _daWBS.Save(drWBS);
                RefreshWBSTags();
                //Common.Prompt("WBS successfully added");
            }
        }

        private void btnAddTag_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridViewSchedule.SelectedRowsCount == 0)
            {
                Common.Warn("Please select schedule to add tag");
                return;
            }

            frmSchedule_AddTag frmTagAdd;

            wbsTagDisplay selectedWBSTag = (wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(treeListWBSTag.FocusedNode);
            List<wbsTagDisplay> tempWBSTagDisplay = new List<wbsTagDisplay>(_allWBSTagDisplay); //temporary tag display for tree list selection to prevent modification to original

            if (selectedWBSTag == null)
                frmTagAdd = new frmSchedule_AddTag(tempWBSTagDisplay, Guid.Empty, _projectGuid);
            else
            {
                frmTagAdd = new frmSchedule_AddTag(tempWBSTagDisplay, selectedWBSTag.wbsTagDisplayGuid, _projectGuid);
            }

            if (frmTagAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Schedule selectedSchedule = (Schedule)gridViewSchedule.GetFocusedRow();

                if (selectedSchedule != null)
                {
                    Tag newTag = frmTagAdd.GetTag();
                    dsTAG.TAGRow drTag = _dsTag.TAG.NewTAGRow();
                    drTag.GUID = Guid.NewGuid();
                    AssignTagDetails(drTag, newTag);
                    drTag.SCHEDULEGUID = selectedSchedule.GUID;
                    drTag.TYPE1 = newTag.tagType1;
                    drTag.TYPE2 = newTag.tagType2;
                    drTag.TYPE3 = newTag.tagType3;
                    drTag.CREATED = DateTime.Now;
                    drTag.CREATEDBY = System_Environment.GetUser().GUID;
                    _dsTag.TAG.AddTAGRow(drTag);
                    _daTag.Save(drTag);
                    RefreshWBSTags();
                    //Common.Prompt("Tag successfully added");
                }
                else
                    Common.Prompt("Please select a schedule to add the tag");
            }
        }

        private void btnEditSchedule_Click(object sender, EventArgs e)
        {
            if (gridViewSchedule.SelectedRowsCount == 0)
            {
                Common.Warn("Please select schedule to edit");
                return;
            }

            frmSchedule_Add frmScheduleAdd = new frmSchedule_Add((Schedule)gridViewSchedule.GetFocusedRow());

            if (frmScheduleAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Schedule editSchedule = frmScheduleAdd.GetSchedule();

                dsSCHEDULE.SCHEDULERow drSchedule = _daSchedule.GetBy(editSchedule.GUID);
                drSchedule.NAME = editSchedule.scheduleName;
                drSchedule.DESCRIPTION = editSchedule.scheduleDescription;
                drSchedule.DISCIPLINE = cmbDiscipline.Text;
                drSchedule.UPDATED = DateTime.Now;
                drSchedule.UPDATEDBY = System_Environment.GetUser().GUID;
                _daSchedule.Save(drSchedule);
                //Common.Prompt("Schedule successfully updated");
                RefreshSchedule(false);
            }
        }

        private void btnEditTagWBS_Click(object sender, EventArgs e)
        {
            //multiple selection handling
            List<wbsTagDisplay> tempWBSTagDisplay = new List<wbsTagDisplay>(_allWBSTagDisplay); //temporary tag display for tree list selection to prevent modification to original
            if(treeListWBSTag.Selection.Count > 1)
            {
                List<wbsTagDisplay> assignWBSTags = new List<wbsTagDisplay>();

                //only perform deletion on the lowest level
                foreach (TreeListNode node in treeListWBSTag.Selection)
                {
                    assignWBSTags.Add((wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(node));
                }

                //take not whether we have done any tag assignment
                bool tagReassignment = false;

                frmSchedule_AddTag frmAddTag = new frmSchedule_AddTag(tempWBSTagDisplay, assignWBSTags);
                if(frmAddTag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Guid selectedParent = frmAddTag.GetParentSelection();
                    string selectedType1 = frmAddTag.GetType1Selection();
                    string selectedType2 = frmAddTag.GetType2Selection();
                    string selectedType3 = frmAddTag.GetType3Selection();

                    foreach (wbsTagDisplay wbsTag in assignWBSTags)
                    {
                        if(wbsTag.wbsTagDisplayAttachTag != null)
                        {
                            dsTAG.TAGRow drTag = _daTag.GetBy(wbsTag.wbsTagDisplayAttachTag.GUID);
                            if(drTag != null)
                            {
                                if(selectedParent != Guid.Empty || selectedType1 != null)
                                {
                                    drTag.UPDATED = DateTime.Now;
                                    drTag.UPDATEDBY = System_Environment.GetUser().GUID;
                                    drTag.PARENTGUID = selectedParent;
                                    if (selectedType1 != null && selectedType1 != string.Empty)
                                        drTag.TYPE1 = selectedType1;

                                    if (selectedType2 != null && selectedType2 != string.Empty)
                                        drTag.TYPE2 = selectedType2;

                                    if (selectedType3 != null && selectedType3 != string.Empty)
                                        drTag.TYPE3 = selectedType3;

                                    _daTag.Save(drTag);
                                    tagReassignment = true;
                                }
                            }
                        }
                        else if(wbsTag.wbsTagDisplayAttachWBS != null)
                        {
                            dsWBS.WBSRow drWBS = _daWBS.GetBy(wbsTag.wbsTagDisplayAttachWBS.GUID);
                            if(drWBS != null)
                            {
                                if (selectedParent != Guid.Empty)
                                {
                                    drWBS.UPDATED = DateTime.Now;
                                    drWBS.UPDATEDBY = System_Environment.GetUser().GUID;
                                    drWBS.PARENTGUID = selectedParent;
                                    _daWBS.Save(drWBS);
                                }
                            }
                        }
                    }

                    if(tagReassignment)
                    {
                        //need to rectify WBS parent for tag assignments
                        foreach(wbsTagDisplay wbsTag in assignWBSTags)
                        {
                            if(wbsTag.wbsTagDisplayAttachTag != null)
                            {
                                //fix the tag first
                                //dsTAG.TAGRow drTag = _daTag.GetBy(wbsTag.wbsTagDisplayAttachTag.GUID);
                                //if(drTag != null)
                                //{
                                //    Guid? firstWBSParent = _daWBS.GetFirstWBSGuid(drTag.PARENTGUID);
                                //    if (firstWBSParent != null)
                                //    {
                                //        drTag.WBSGUID = (Guid)firstWBSParent;
                                //        _daTag.Save(drTag);
                                //    }
                                //}

                                //fix the tag children next
                                dsTAG.TAGDataTable dtTagAndChildren = _daTag.GetTagChildrens(wbsTag.wbsTagDisplayGuid, true);
                                if(dtTagAndChildren != null)
                                {
                                    foreach (dsTAG.TAGRow drTagChildren in dtTagAndChildren.Rows)
                                    {
                                        Guid? firstWBSParent = _daWBS.GetFirstWBSGuid(drTagChildren.PARENTGUID);
                                        if (firstWBSParent != null)
                                        {
                                            drTagChildren.WBSGUID = (Guid)firstWBSParent;
                                            _daTag.Save(drTagChildren);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    RefreshWBSTags();
                }
                return;
            }

            //single selection handling
            wbsTagDisplay selectedWt = (wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(treeListWBSTag.FocusedNode);
            if (selectedWt == null)
            {
                Common.Warn("Please select something to edit");
                return;
            }


            if (selectedWt.wbsTagDisplayAttachTag != null)
            {
                frmSchedule_AddTag frmAddTag = new frmSchedule_AddTag(tempWBSTagDisplay, selectedWt.wbsTagDisplayAttachTag, _projectGuid);
                if (frmAddTag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Tag editTag = frmAddTag.GetTag();

                    dsTAG.TAGRow drTag = _daTag.GetBy(editTag.GUID);
                    drTag.TYPE1 = editTag.tagType1;
                    drTag.TYPE2 = editTag.tagType2;
                    drTag.TYPE3 = editTag.tagType3;
                    AssignTagDetails(drTag, editTag);
                    drTag.UPDATED = DateTime.Now;
                    drTag.UPDATEDBY = System_Environment.GetUser().GUID;
                    _daTag.Save(drTag);
                    //Common.Prompt("Tag successfully updated");
                    RefreshWBSTags();
                }
            }
            else
            {
                //frmSchedule_AddWBS frmAddWBS = new frmSchedule_AddWBS(((Schedule)gridViewSchedule.GetFocusedRow()).GUID, tempWBSTagDisplay, selectedWt.wbsTagDisplayAttachWBS); ////Fix 12-DEC-2014: WBS is unique to project
                frmSchedule_AddWBS frmAddWBS = new frmSchedule_AddWBS(_projectGuid, tempWBSTagDisplay, selectedWt.wbsTagDisplayAttachWBS);
                if (frmAddWBS.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    WBS editWBS = frmAddWBS.GetWBS();

                    dsWBS.WBSRow drWBS = _daWBS.GetBy(editWBS.GUID);
                    drWBS.NAME = editWBS.wbsName;
                    drWBS.DESCRIPTION = editWBS.wbsDescription;
                    drWBS.PARENTGUID = editWBS.wbsParentGuid;
                    drWBS.UPDATED = DateTime.Now;
                    drWBS.UPDATEDBY = System_Environment.GetUser().GUID;
                    _daWBS.Save(drWBS);
                    //Common.Prompt("WBS successfully updated");
                    RefreshWBSTags();
                }
            }
        }

        private void btnDeleteSchedule_Click(object sender, EventArgs e)
        {
            if (gridViewSchedule.SelectedRowsCount == 0)
            {
                Common.Warn("Please select schedule to delete");
                return;
            }

            if (!Common.Confirmation("Are you sure you want to delete the selected schedule?", "Confirmation"))
                return;

            dsSCHEDULE.SCHEDULEDataTable dtSchedule = new dsSCHEDULE.SCHEDULEDataTable();
            Schedule selectedSchedule = (Schedule)gridViewSchedule.GetFocusedRow();

            dsSCHEDULE.SCHEDULERow drSchedule = dtSchedule.NewSCHEDULERow();
            drSchedule.GUID = selectedSchedule.GUID;
            drSchedule.PROJECTGUID = selectedSchedule.scheduleProjectGuid;
            drSchedule.NAME = selectedSchedule.scheduleName;
            drSchedule.DESCRIPTION = selectedSchedule.scheduleDescription;
            drSchedule.DISCIPLINE = selectedSchedule.scheduleDiscipline;
            drSchedule.CREATED = selectedSchedule.CreatedDate;
            drSchedule.CREATEDBY = selectedSchedule.CreatedBy;
            dtSchedule.AddSCHEDULERow(drSchedule);

            splashScreenManager1.ShowWaitForm();
            rptDeletion f = new rptDeletion(dtSchedule);
            splashScreenManager1.CloseWaitForm();
            f.ShowReport();

            RefreshSchedule(true);
            
        }

        private void btnDeleteTag_Click(object sender, EventArgs e)
        {
            if (treeListWBSTag.FocusedNode == null)
            {
                Common.Warn("Please select something to delete");
                return;
            }

            if (!Common.Confirmation("Are you sure you want to delete the selection along with its childrens?", "Confirmation"))
                return;

            Schedule selectedSchedule = (Schedule)gridViewSchedule.GetFocusedRow();
            List<wbsTagDisplay> selectedNodes = new List<wbsTagDisplay>();

            //Same as construct node selection but nodes couldn't be used because treeListWBSTag acts like an interface
            foreach (TreeListNode selectedNode in treeListWBSTag.Selection)
            {
                wbsTagDisplay nodeWBSTag = (wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(selectedNode);
                if (selectedNodes != null && !selectedNodes.Any(obj => obj.wbsTagDisplayGuid == nodeWBSTag.wbsTagDisplayGuid))
                {
                    if (nodeWBSTag.wbsTagDisplayAttachWBS != null)
                        continue;

                    selectedNodes.Add(nodeWBSTag);
                    if (selectedNode.HasChildren)
                    {
                        ConstructNodeSelection(selectedNodes, selectedNode.Nodes);
                    }
                }
            }

            splashScreenManager1.ShowWaitForm();
            rptDeletion f = new rptDeletion(selectedSchedule, selectedNodes);
            splashScreenManager1.CloseWaitForm();
            f.ShowReport();

            RefreshWBSTags();
        }

        private void btnTrimWBS_Click(object sender, EventArgs e)
        {
            AdapterTAG daTAG = new AdapterTAG();
            AdapterITR_MAIN daITR_MAIN = new AdapterITR_MAIN();
            AdapterPUNCHLIST_MAIN daPUNCHLIST_MAIN = new AdapterPUNCHLIST_MAIN();
            AdapterWBS daWBS = new AdapterWBS();
            HashSet<dsWBS.WBSRow> WBSExcludedForDeletions = new HashSet<dsWBS.WBSRow>();
            int wbsRemovedCount = 0;

            try
            {
                dsITR_MAIN.ITR_MAINDataTable dtMasterITRs = new dsITR_MAIN.ITR_MAINDataTable();
                dsITR_MAIN.ITR_MAINDataTable dtTagITRs = daITR_MAIN.GetByTagProject(_projectGuid);
                dsITR_MAIN.ITR_MAINDataTable dtWBSITRs = daITR_MAIN.GetByWBSProject(_projectGuid);
                if(dtTagITRs != null)
                {
                    dtMasterITRs.Merge(dtTagITRs);
                    dtTagITRs.Dispose();
                }

                if(dtWBSITRs != null)
                {
                    dtMasterITRs.Merge(dtWBSITRs);
                    dtWBSITRs.Dispose();
                }

                dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable dtMasterPunchlists = new dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable();
                dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable dtPunchlistsByTag = daPUNCHLIST_MAIN.GetAllTagSystemByProject(_projectGuid);
                dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemDataTable dtPunchlistsByWBS = daPUNCHLIST_MAIN.GetAllWBSSystemByProject(_projectGuid);
                if (dtPunchlistsByTag != null)
                {
                    dtMasterPunchlists.Merge(dtPunchlistsByTag);
                    dtPunchlistsByTag.Dispose();
                }

                if(dtPunchlistsByWBS != null)
                {
                    dtMasterPunchlists.Merge(dtPunchlistsByWBS);
                    dtPunchlistsByWBS.Dispose();
                }

                dsTAG.TAGDataTable dtTAGs = daTAG.GetByProject(_projectGuid);
                dsWBS.WBSDataTable dtWBS = daWBS.GetByProject(_projectGuid);
                foreach(dsWBS.WBSRow drWBS in dtWBS.Rows)
                {
                    if(dtTAGs.Any(x => x.PARENTGUID == drWBS.GUID) || dtMasterITRs.Any(x => !x.IsWBS_GUIDNull() && x.WBS_GUID == drWBS.GUID) || dtMasterPunchlists.Any(x => !x.IsWBS_GUIDNull() && x.WBS_GUID == drWBS.GUID))
                    {
                        IEnumerable<dsWBS.WBSRow> allParentWBSs = AllParent(drWBS.GUID, dtWBS);
                        foreach(dsWBS.WBSRow allParentWBS in allParentWBSs)
                        {
                            if(!WBSExcludedForDeletions.Any(x => x.GUID == allParentWBS.GUID))
                            {
                                WBSExcludedForDeletions.Add(allParentWBS);
                            }
                        }
                    }
                }

                foreach (dsWBS.WBSRow drWBS in dtWBS.Rows)
                {
                    if (!WBSExcludedForDeletions.Any(x => x.GUID == drWBS.GUID))
                    {
                        daWBS.RemoveBy(drWBS.GUID);
                        wbsRemovedCount += 1;
                    }
                }

                MessageBox.Show(wbsRemovedCount.ToString() + " has been removed", "WBS Removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                daTAG.Dispose();
                daITR_MAIN.Dispose();
                daWBS.Dispose();
                daPUNCHLIST_MAIN.Dispose();
            }
        }

        /// <summary>
        /// Get's all the parent WBSDisciplineRow
        /// </summary>
        public static IEnumerable<dsWBS.WBSRow> AllParent(Guid parentGuid, dsWBS.WBSDataTable dtWBS)
        {
            foreach (dsWBS.WBSRow wbsTagParent in dtWBS.Rows)
            {
                if (wbsTagParent.GUID == parentGuid)
                {
                    yield return wbsTagParent;

                    foreach (dsWBS.WBSRow wbsTagGrandparent in AllParent(wbsTagParent.PARENTGUID, dtWBS))
                    {
                        yield return wbsTagGrandparent;
                    }
                }
            }
        }

        private void bBtnDeleteTagWBS_Click(object sender, EventArgs e)
        {
            if(treeListWBSTag.FocusedNode == null)
            {
                Common.Warn("Please select something to delete");
                return;
            }

            if (!Common.Confirmation("Are you sure you want to delete the selection along with its childrens?", "Confirmation"))
                return;

            Schedule selectedSchedule = (Schedule)gridViewSchedule.GetFocusedRow();
            List<wbsTagDisplay> selectedNodes = new List<wbsTagDisplay>();

            //Same as construct node selection but nodes couldn't be used because treeListWBSTag acts like an interface
            foreach (TreeListNode selectedNode in treeListWBSTag.Selection)
            {
                wbsTagDisplay nodeWBSTag = (wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(selectedNode);
                if (selectedNodes != null && !selectedNodes.Any(obj => obj.wbsTagDisplayGuid == nodeWBSTag.wbsTagDisplayGuid))
                {
                    selectedNodes.Add(nodeWBSTag);
                    if (selectedNode.HasChildren)
                    {
                        ConstructNodeSelection(selectedNodes, selectedNode.Nodes);
                    }
                }
            }

            splashScreenManager1.ShowWaitForm();
            rptDeletion f = new rptDeletion(selectedSchedule, selectedNodes);
            splashScreenManager1.CloseWaitForm();
            f.ShowReport();

            RefreshWBSTags();
        }

        /// <summary>
        /// Construct the full tree of user selection
        /// </summary>
        private void ConstructNodeSelection(List<wbsTagDisplay> selectedNodes, TreeListNodes nodes)
        {
            foreach(TreeListNode node in nodes)
            {
                wbsTagDisplay nodeWBSTag = (wbsTagDisplay)treeListWBSTag.GetDataRecordByNode(node);
                if (!selectedNodes.Any(obj => obj.wbsTagDisplayGuid == nodeWBSTag.wbsTagDisplayGuid))
                {
                    selectedNodes.Add(nodeWBSTag);
                    if (node.HasChildren)
                    {
                        ConstructNodeSelection(selectedNodes, node.Nodes);
                    }
                }
            }
        }

        private void cmbDiscipline_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshWBSTags();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Assigns common tag details to data row
        /// </summary>
        /// <param name="drUserMaster">datarow to be assigned</param>
        /// <param name="user">tag details</param>
        private void AssignTagDetails(dsTAG.TAGRow drTag, Tag tag)
        {
            drTag.NUMBER = tag.tagNumber;
            drTag.DESCRIPTION = tag.tagDescription;
            drTag.PARENTGUID = tag.tagParentGuid;
            drTag.SCHEDULEGUID = tag.tagScheduleGuid;
            drTag.WBSGUID = tag.tagWBSGuid;
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
            _daTag.Dispose();
            _daWBS.Dispose();
            base.OnClosed(e);
        }

        private void btnDisciplineMasterReport_Click(object sender, EventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("Loading...");
            frmReportSheet frmCertificateSheet = new frmReportSheet(_projectGuid);
            frmCertificateSheet.populateCertificates(Variables.CertificateReportType.DisciplineMasterReport);
            splashScreenManager1.CloseWaitForm();
            frmCertificateSheet.Show();
        }
    }
}
