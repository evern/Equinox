using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectCommon;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

namespace CheckmateDX
{
    public partial class frmSchedule_Assign : CheckmateDX.frmParent
    {
        //drag and drop
        GridHitInfo _downHitInfo = null;

        //database
        AdapterTEMPLATE_MAIN _daTemplate = new AdapterTEMPLATE_MAIN();
        dsTEMPLATE_MAIN _dsTemplate = new dsTEMPLATE_MAIN();
        AdapterWORKFLOW_MAIN _daWorkflow = new AdapterWORKFLOW_MAIN();
        dsWORKFLOW_MAIN _dsWorkflow = new dsWORKFLOW_MAIN();
        AdapterTEMPLATE_REGISTER _daTemplateRegister = new AdapterTEMPLATE_REGISTER();
        AdapterTAG _daTag = new AdapterTAG();
        AdapterWBS _daWBS = new AdapterWBS();

        //class lists
        List<Template> _allTemplate = new List<Template>();
        List<wtDisplay> _allWtDisplay = new List<wtDisplay>();
        List<wbsTagDisplay> _allWBSTagDisplay = new List<wbsTagDisplay>();

        List<wbsTagDisplay> _enabledWBSTagDisplay = new List<wbsTagDisplay>();
        List<wbsTagDisplay> _disabledWBSTagDisplay = new List<wbsTagDisplay>();

        //global variables
        string _discipline = string.Empty;

        public frmSchedule_Assign(string discipline, List<wbsTagDisplay>wbsTagDisplay)
        {
            InitializeComponent();
            InitializeWBSTagImage();
            _allWBSTagDisplay = wbsTagDisplay;
            _discipline = discipline;
            wtDisplayBindingSource.DataSource = _allWtDisplay;
            wbsTagDisplayBindingSource.DataSource = _enabledWBSTagDisplay;
            wbsTagDisplayBindingSource1.DataSource = _disabledWBSTagDisplay;
            RefreshWorkflowTemplates();
        }

        #region Form Population
        /// <summary>
        /// Refresh all template from database
        /// </summary>
        private void RefreshWorkflowTemplates()
        {
            _allWtDisplay.Clear();
            //retrieve template
            dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTemplate = new dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable();
            if(_discipline == Discipline.Electrical.ToString() || _discipline == Discipline.Instrumentation.ToString()) //if discipline is electrical or instrumentation retrieve both discipline
            {
                dtTemplate = _daTemplate.GetByDiscipline(Discipline.Electrical.ToString());
                dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTemplateAlt = _daTemplate.GetByDiscipline(Discipline.Instrumentation.ToString());
                if(dtTemplateAlt != null)
                    dtTemplate.Merge(dtTemplateAlt);
            }
            else if(_discipline == Discipline.Mechanical.ToString() || _discipline == Discipline.Piping.ToString())
            {
                dtTemplate = _daTemplate.GetByDiscipline(Discipline.Mechanical.ToString());
                dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTemplatePiping = _daTemplate.GetByDiscipline(Discipline.Piping.ToString());
                dtTemplate.Merge(dtTemplatePiping);
            }
            else
                dtTemplate = _daTemplate.GetByDiscipline(_discipline);

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
                        templateDiscipline = drTemplate.DISCIPLINE
                    }));
                }
            }

            //retrieve workflow
            dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWorkflow = _daWorkflow.Get();
            if (dtWorkflow != null)
            {
                foreach (dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow in dtWorkflow.Rows)
                {
                    _allWtDisplay.Add(new wtDisplay(new Workflow(drWorkflow.GUID)
                    {
                        workflowName = drWorkflow.NAME,
                        workflowDescription = drWorkflow.IsDESCRIPTIONNull() ? "" : drWorkflow.DESCRIPTION,
                        workflowParentGuid = drWorkflow.PARENTGUID
                    })
                    {
                        wtDisplayName = drWorkflow.NAME
                    });
                }
            }

            treeListTemplate.RefreshDataSource();
            treeListTemplate.ExpandAll();
        }

        /// <summary>
        /// Refresh all wbs/tag from database
        /// </summary>
        private void RefreshWBSTag()
        {
            _enabledWBSTagDisplay.Clear();
            _disabledWBSTagDisplay.Clear();

            wtDisplay selectedWt = (wtDisplay)treeListTemplate.GetDataRecordByNode(treeListTemplate.FocusedNode);

            //only allows tagging to template
            if (selectedWt == null || selectedWt.wtDisplayAttachTemplate == null)
            {   //clears the list
                gridControlIncluded.RefreshDataSource();
                gridControlExcluded.RefreshDataSource(); 
                return;
            }

            dsTEMPLATE_REGISTER.TEMPLATE_REGISTERDataTable dtTemplateReg = _daTemplateRegister.GetByTemplate(selectedWt.wtDisplayAttachTemplate.GUID);
            if(dtTemplateReg != null)
            {
                foreach(dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTemplateReg in dtTemplateReg.Rows)
                {
                    AddWBSTagToList(_enabledWBSTagDisplay, drTemplateReg);
                }
            }

            foreach(wbsTagDisplay wbsTag in _allWBSTagDisplay)
            {
                if(!_enabledWBSTagDisplay.Any(obj => obj.wbsTagDisplayGuid == wbsTag.wbsTagDisplayGuid))
                {
                    _disabledWBSTagDisplay.Add(wbsTag);
                }
            }

            gridControlIncluded.RefreshDataSource();
            gridControlExcluded.RefreshDataSource();
        }
        #endregion

        #region Event
        private void treeListTemplate_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            RefreshWBSTag();
        }
        #endregion

        #region Drag'n'Drop
        /// <summary>
        /// Action handler to validate area which user starts dragging
        /// </summary>
        private void grid_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            _downHitInfo = null;

            GridHitInfo hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
            if (Control.ModifierKeys != Keys.None) return;
            if (e.Button == MouseButtons.Left && hitInfo.InRow && hitInfo.HitTest != GridHitTest.RowIndicator)
                _downHitInfo = hitInfo;
        }

        /// <summary>
        /// Action handler for when user draging starts on a valid area
        /// </summary>
        private void grid_MouseMove(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            if (e.Button == MouseButtons.Left && _downHitInfo != null)
            {
                Size dragSize = SystemInformation.DragSize;
                Rectangle dragRect = new Rectangle(new Point(_downHitInfo.HitPoint.X - dragSize.Width / 2,
                    _downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);

                if (!dragRect.Contains(new Point(e.X, e.Y)))
                {
                    view.GridControl.DoDragDrop(GetDragData(view), DragDropEffects.All);
                    _downHitInfo = null;
                }
            }
        }

        /// <summary>
        /// Action handler for when user starts dropping
        /// </summary>
        private void grid_DragDrop(object sender, DragEventArgs e)
        {
            GridControl grid = (GridControl)sender;
            BindingSource binding = (BindingSource)grid.DataSource;
            DragData data = (DragData)e.Data.GetData(typeof(DragData));

            List<wbsTagDisplay> wbsTagFrom;
            List<wbsTagDisplay> wbsTagTo;

            if (grid.Name == gridControlExcluded.Name)
            {
                wbsTagFrom = _enabledWBSTagDisplay;
                wbsTagTo = _disabledWBSTagDisplay;
            }
            else
            {
                wbsTagFrom = _disabledWBSTagDisplay;
                wbsTagTo = _enabledWBSTagDisplay;
            }

            if (data != null && binding != null)
            {
                for (int i = 0; i < data.dataRowIndexes.GetLength(0); i++)
                {
                    wbsTagDisplay movingWBSTag = wbsTagFrom[data.dataRowIndexes[i]];
                    wbsTagTo.Add(movingWBSTag);
                }

                foreach (wbsTagDisplay wbsTag in wbsTagTo)
                {
                    wbsTagDisplay findEnabled = wbsTagFrom.FirstOrDefault(obj => obj.wbsTagDisplayGuid == wbsTag.wbsTagDisplayGuid);
                    if (findEnabled != null)
                    {
                        wbsTagFrom.Remove(findEnabled);

                        //database operation
                        wtDisplay selectedTemplate = (wtDisplay)treeListTemplate.GetDataRecordByNode(treeListTemplate.FocusedNode);
                        if(selectedTemplate != null)
                        {
                            if(grid.Name == gridControlIncluded.Name)
                            {
                                _daTemplateRegister.AssignTagWBSTemplate(selectedTemplate.wtDisplayGuid, wbsTag);
                            }
                            else
                            {
                                _daTemplateRegister.UntagTemplate(selectedTemplate.wtDisplayGuid, wbsTag);
                            }

                            if(wbsTag.wbsTagDisplayAttachTag != null)
                            {
                                dsTAG.TAGRow drTagUpdate = _daTag.GetBy(wbsTag.wbsTagDisplayAttachTag.GUID);
                                if(drTagUpdate != null)
                                {
                                    drTagUpdate.UPDATED = DateTime.Now;
                                    drTagUpdate.UPDATEDBY = System_Environment.GetUser().GUID;
                                    _daTag.Save(drTagUpdate);
                                }
                            }

                            if(wbsTag.wbsTagDisplayAttachWBS != null)
                            {
                                dsWBS.WBSRow drWBSUpdate = _daWBS.GetBy(wbsTag.wbsTagDisplayAttachWBS.GUID);
                                if(drWBSUpdate != null)
                                {
                                    drWBSUpdate.UPDATED = DateTime.Now;
                                    drWBSUpdate.UPDATEDBY = System_Environment.GetUser().GUID;
                                    _daWBS.Save(drWBSUpdate);
                                }
                            }
                        }
                    }
                }
            }

            gridControlIncluded.RefreshDataSource();
            gridControlExcluded.RefreshDataSource();
        }

        /// <summary>
        /// Dragging UI representation
        /// </summary>
        private void grid_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DragData)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Retrive selected indexes when user start draging the data
        /// </summary>
        private DragData GetDragData(GridView view)
        {
            int[] selection = view.GetSelectedRows();
            if (selection == null)
                return null;

            int count = selection.Length;
            DragData result = new DragData() { sourceData = (BindingSource)view.GridControl.DataSource, dataRowIndexes = new int[count] };

            for (int i = 0; i < count; i++)
            {
                result.dataRowIndexes[i] = view.GetDataSourceRowIndex(selection[i]);
            }

            return result;
        }
        #endregion

        #region Helper
        /// <summary>
        /// Adds datarow template into list
        /// </summary>
        private void AddWBSTagToList(List<wbsTagDisplay> wbsTagList, dsTEMPLATE_REGISTER.TEMPLATE_REGISTERRow drTemplateRegister)
        {
            //use a general guid to store both tag and wbs guid
            Guid generalGuid = Guid.Empty; 

            if(!drTemplateRegister.IsTAG_GUIDNull())
               generalGuid = drTemplateRegister.TAG_GUID;
            else if(!drTemplateRegister.IsWBS_GUIDNull())
                generalGuid = drTemplateRegister.WBS_GUID;

            wbsTagDisplay findWBSTag = _allWBSTagDisplay.FirstOrDefault(obj => obj.wbsTagDisplayGuid == generalGuid);

            if (findWBSTag != null)
                wbsTagList.Add(findWBSTag);
        }

        /// <summary>
        /// Customize the datagrid to show image for WBS/Tag type
        /// </summary>
        private void InitializeWBSTagImage()
        {
            RepositoryItemImageComboBox imageCombo = gridControlExcluded.RepositoryItems.Add("ImageComboBoxEdit") as RepositoryItemImageComboBox;
            DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
            images.AddImage(imageList2.Images[0]);
            images.AddImage(imageList2.Images[1]);
            imageCombo.SmallImages = images;
            imageCombo.Items.Add(new ImageComboBoxItem("WBS", 0, 0));
            imageCombo.Items.Add(new ImageComboBoxItem("Tag", 1, 1));
            imageCombo.GlyphAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridViewIncluded.Columns["wbsTagDisplayImageIndex"].ColumnEdit = imageCombo;
            gridViewExcluded.Columns["wbsTagDisplayImageIndex"].ColumnEdit = imageCombo;
        }
        #endregion

        /// <summary>
        /// Dispose local adapters upon closing
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            _daTemplate.Dispose();
            _daTemplateRegister.Dispose();
            _daWorkflow.Dispose();
            _daTag.Dispose();
            _daWBS.Dispose();
            base.OnClosed(e);
        }
    }
}
