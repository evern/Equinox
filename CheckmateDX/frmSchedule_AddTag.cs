using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectCommon;
using ProjectLibrary;
using DevExpress.XtraEditors;

namespace CheckmateDX
{
    public partial class frmSchedule_AddTag : CheckmateDX.frmParent
    {
        private Tag _editTag;
        private Tag _Tag;
        private Guid _ParentGuid;
        private Guid _projectGuid;

        private static string assignWBS = "Assign WBS";
        List<wbsTagDisplay> _allWBSTagDisplay = new List<wbsTagDisplay>();
        List<MatrixType> _allMatrixType = new List<MatrixType>();

        /// <summary>
        /// Constructor for adding
        /// </summary>
        public frmSchedule_AddTag(List<wbsTagDisplay> wbsTagDisplay, Guid selectedWBSParentGuid, Guid projectGuid)
        {
            InitializeComponent();
            _projectGuid = projectGuid; //Changes 10-DEC-2014 Allow same tag number to be added per project
            PopulateFormElements(wbsTagDisplay, selectedWBSParentGuid);
        }

        /// <summary>
        /// Constructor for editing
        /// </summary>
        /// <param name="editTag">Tag to edit</param>
        public frmSchedule_AddTag(List<wbsTagDisplay> wbsTagDisplay, Tag editTag, Guid projectGuid)
        {
            InitializeComponent();
            _projectGuid = projectGuid;  //Changes 10-DEC-2014 Allow same tag number to be added per project
            this.Text = "Edit Tag";
            btnOk.Text = "Accept";
            PopulateFormElements(wbsTagDisplay, editTag);
            _editTag = editTag;
        }

        /// <summary>
        /// Constructor for assigning WBS parent
        /// </summary>
        /// <param name="wbsTagDisplay">the full wbs/tag list</param>
        /// <param name="wbsTagSelection">user selected wbs/tag entries</param>
        public frmSchedule_AddTag(List<wbsTagDisplay> wbsTagDisplay, List<wbsTagDisplay> wbsTagSelection)
        {
            InitializeComponent();
            pnlNumber.Visible = false;
            pnlDescription.Visible = false;
            pnlType.Visible = true;
            this.Text = assignWBS;
            btnOk.Text = "Accept";

            PopulateFormElements(wbsTagDisplay, wbsTagSelection);
        }

        /// <summary>
        /// Populate form element for editing
        /// </summary>
        /// <param name="selectedParent">Parent wbs guid</param>
        private void PopulateFormElements(List<wbsTagDisplay> wbsTagDisplay, Tag editTag)
        {
            PopulateFormElements(wbsTagDisplay, editTag.tagParentGuid, editTag.GUID);

            txtNumber.Text = editTag.tagNumber;
            txtDescription.Text = editTag.tagDescription;

            MatrixType find_matrix_type = _allMatrixType.FirstOrDefault(x => x.typeName.ToUpper() == editTag.tagType1.ToUpper());
            if(find_matrix_type != null)
                searchLookUpType1.EditValue = find_matrix_type.typeName.ToString();

            find_matrix_type = _allMatrixType.FirstOrDefault(x => x.typeName.ToUpper() == editTag.tagType2.ToUpper());
            if (find_matrix_type != null)
                searchLookUpType2.EditValue = find_matrix_type.typeName.ToString();

            find_matrix_type = _allMatrixType.FirstOrDefault(x => x.typeName.ToUpper() == editTag.tagType3.ToUpper());
            if (find_matrix_type != null)
                searchLookUpType3.EditValue = find_matrix_type.typeName.ToString();
        }

        /// <summary>
        /// Populate form element for WBS assinment
        /// </summary>
        /// <param name="wbsTagDisplay">full wbs members</param>
        /// <param name="wbsTagSelection">selected wbs members</param>
        private void PopulateFormElements(List<wbsTagDisplay> wbsTagDisplay, List<wbsTagDisplay> wbsTagSelection)
        {
            List<List<wbsTagDisplay>> wbsTagDisplayLists = new List<List<wbsTagDisplay>>();
            bool wbsOnly = wbsTagSelection.Any(obj => obj.wbsTagDisplayAttachWBS != null);

            //add the validated list entries into a list of list
            foreach (wbsTagDisplay wbsTag in wbsTagSelection)
            {
                List<wbsTagDisplay> tempWBSTagDisplay = new List<ProjectLibrary.wbsTagDisplay>(wbsTagDisplay);
                wbsTagDisplayLists.Add(Common.ProcessWBSTagTreeList(tempWBSTagDisplay, wbsTag.wbsTagDisplayGuid, wbsOnly));
            }

            List<wbsTagDisplay> finalWBSTagList = new List<wbsTagDisplay>();

            //filter values available in all the lists
            foreach (List<wbsTagDisplay> wbsTagDisplayList in wbsTagDisplayLists)
            {
                foreach (wbsTagDisplay wbsTag in wbsTagDisplayList)
                {
                    //if the entry belongs in all lists
                    if (wbsTagDisplayLists.All(obj => obj.Any(obj1 => obj1.wbsTagDisplayGuid == wbsTag.wbsTagDisplayGuid)))
                    {
                        //if the entry hasn't been added
                        if (!finalWBSTagList.Any(obj => obj.wbsTagDisplayGuid == wbsTag.wbsTagDisplayGuid))
                            finalWBSTagList.Add(wbsTag);
                    }
                }
            }

            _allWBSTagDisplay = finalWBSTagList;
            wbsTagDisplayBindingSource.DataSource = _allWBSTagDisplay;
            treeListLookUpWBS.EditValue = _allWBSTagDisplay[0].wbsTagDisplayGuid;
            Bind_Matrix_Type();
            //searchLookUpType.EditValue = _allWBSTagDisplay[0].wbsTagDisplayType;
        }

        /// <summary>
        /// Populate form element for editing
        /// </summary>
        /// <param name="wbsTagDisplay">selectable wbs members</param>
        private void PopulateFormElements(List<wbsTagDisplay> wbsTagDisplay, Guid selectedWBSParentGuid, Guid? excludedGuid = null)
        {
            _allWBSTagDisplay = Common.ProcessWBSTagTreeList(wbsTagDisplay, excludedGuid);
            wbsTagDisplayBindingSource.DataSource = _allWBSTagDisplay;

            wbsTagDisplay selectedWBSTag = wbsTagDisplay.FirstOrDefault(obj => obj.wbsTagDisplayGuid == selectedWBSParentGuid);

            if (selectedWBSTag != null)
                treeListLookUpWBS.EditValue = _allWBSTagDisplay[_allWBSTagDisplay.IndexOf(selectedWBSTag)].wbsTagDisplayGuid;
            else
                treeListLookUpWBS.EditValue = _allWBSTagDisplay[0].wbsTagDisplayGuid;

            Bind_Matrix_Type();
        }

        private void Bind_Matrix_Type()
        {
            Refresh_Matrix_Type();
            matrixTypeBindingSource.DataSource = _allMatrixType;

            populateMatrixType(searchLookUpType1);
            populateMatrixType(searchLookUpType2);
            populateMatrixType(searchLookUpType3);
        }

        private void populateMatrixType(SearchLookUpEdit searchLookUp)
        {
            searchLookUp.Properties.View.Columns.AddField("typeName").Visible = true;
            searchLookUp.Properties.View.Columns.AddField("typeDescription").Visible = true;
            searchLookUp.Properties.View.Columns.AddField("typeCategory").Visible = true;
            searchLookUp.Properties.View.Columns.AddField("typeDiscipline").Visible = true;
            searchLookUp.Properties.View.Columns.ColumnByFieldName("typeName").Caption = "Name";
            searchLookUp.Properties.View.Columns.ColumnByFieldName("typeDescription").Caption = "Description";
            searchLookUp.Properties.View.Columns.ColumnByFieldName("typeCategory").Caption = "Category";
            searchLookUp.Properties.View.Columns.ColumnByFieldName("typeDiscipline").Caption = "Discipline";
            searchLookUp.Properties.DisplayMember = "typeName";
            searchLookUp.Properties.ValueMember = "typeName";
        }

        /// <summary>
        /// Populate all available matrix type in the system
        /// </summary>
        private void Refresh_Matrix_Type()
        {
            _allMatrixType.Clear();

            using (AdapterMATRIX_TYPE daMATRIX_TYPE = new AdapterMATRIX_TYPE())
            {
                dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMATRIX_TYPE = daMATRIX_TYPE.Get();
                if (dtMATRIX_TYPE != null)
                {
                    foreach(dsMATRIX_TYPE.MATRIX_TYPERow drMATRIX_TYPE in dtMATRIX_TYPE.Rows)
                    {
                        _allMatrixType.Add(new MatrixType(drMATRIX_TYPE.GUID)
                        {
                            typeName = drMATRIX_TYPE.NAME,
                            typeDescription = drMATRIX_TYPE.IsDESCRIPTIONNull() ? string.Empty : drMATRIX_TYPE.DESCRIPTION,
                            typeDiscipline = drMATRIX_TYPE.DISCIPLINE,
                            typeCategory = drMATRIX_TYPE.CATEGORY
                        });
                    }
                }
            }
        }

        public Tag GetTag()
        {
            return _Tag;
        }

        public Guid GetParentSelection()
        {
            return _ParentGuid;
        }

        public string GetType1Selection()
        {
            if (searchLookUpType1.EditValue == null || searchLookUpType1.EditValue.ToString() == "N/A")
                return null;

            return searchLookUpType1.EditValue.ToString();
        }

        public string GetType2Selection()
        {
            if (searchLookUpType2.EditValue == null || searchLookUpType1.EditValue.ToString() == "N/A")
                return null;

            return searchLookUpType2.EditValue.ToString();
        }

        public string GetType3Selection()
        {
            if (searchLookUpType3.EditValue == null || searchLookUpType1.EditValue.ToString() == "N/A")
                return null;

            return searchLookUpType3.EditValue.ToString();
        }
        #region Validation
        private bool ValidateFormElements()
        {
            if (txtNumber.Text.Trim() == string.Empty)
            {
                Common.Warn("Name cannot be empty");
                txtNumber.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for duplicate Tag
        /// </summary>
        private bool ValidateTag()
        {
            using (AdapterTAG daTag = new AdapterTAG())
            {
                if (daTag.GetBy(txtNumber.Text.Trim(), _projectGuid) != null)  //Changes 10-DEC-2014 Allow same tag number to be added per project
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateEditTag(Guid editTagGuid)
        {
            using (AdapterTAG daTag = new AdapterTAG())
            {
                dsTAG.TAGRow drTag = daTag.GetBy(txtNumber.Text.Trim(), _projectGuid);  //Changes 10-DEC-2014 Allow same tag number to be added per project

                if (drTag != null)
                {
                    if (drTag.GUID != editTagGuid)
                        return false;
                }
            }

            return true;
        }
        #endregion

        #region Events
        private void btnClearWBS_Click(object sender, EventArgs e)
        {
            treeListLookUpWBS.EditValue = null;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if(this.Text == assignWBS)
            {
                if (treeListLookUpWBS.EditValue == null)
                    _ParentGuid = Guid.Empty;
                else
                    _ParentGuid = (Guid)treeListLookUpWBS.EditValue;

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else if (ValidateFormElements())
            {
                if (_editTag != null || ValidateTag())
                {
                    if (_editTag != null)
                    {
                        if (ValidateEditTag(_editTag.GUID))
                        {
                            using (AdapterTAG daTag = new AdapterTAG())
                            {
                                //get the tag childrens because their respective parent WBS guid will be change in tandem to the tag parent
                                dsTAG.TAGDataTable dtTagChildrens = daTag.GetTagChildrens(_editTag.GUID, false);

                                _editTag.tagNumber = txtNumber.Text.Trim();
                                _editTag.tagDescription = txtDescription.Text.Trim();
                                _editTag.tagParentGuid = (Guid)treeListLookUpWBS.EditValue;
                                Guid firstWBSParentGuid = FirstParentWBSGuid(_allWBSTagDisplay, _editTag.tagParentGuid) ?? Guid.Empty;
                                _editTag.tagWBSGuid = firstWBSParentGuid;

                                if(searchLookUpType1.EditValue != null)
                                    _editTag.tagType1 = searchLookUpType1.EditValue.ToString();

                                _Tag = _editTag;
                                
                                if(dtTagChildrens != null)
                                {
                                    //update the tag children's WBS parent to match tag parent
                                    foreach (dsTAG.TAGRow drTagChildren in dtTagChildrens.Rows)
                                    {
                                        drTagChildren.WBSGUID = firstWBSParentGuid;
                                        daTag.Save(drTagChildren);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Common.Prompt("The number specified has already been used by other tag\n\nPlease type in a different name");
                            return;
                        }
                    }
                    else
                    {
                        Tag newTag = new Tag(Guid.NewGuid())
                        {
                            tagNumber = txtNumber.Text.Trim(),
                            tagDescription = txtDescription.Text.Trim(),
                            tagParentGuid = (Guid)treeListLookUpWBS.EditValue,
                            tagWBSGuid = FirstParentWBSGuid(_allWBSTagDisplay, (Guid)treeListLookUpWBS.EditValue) ?? Guid.Empty,
                            tagType1 = searchLookUpType1.EditValue.ToString()
                        };

                        _Tag = newTag;
                    }

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                    Common.Warn("Tag number already exists for this project"); ;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Retrieve first instance of a tag's WBS guid
        /// </summary>
        /// <param name="allWBSTagDisplay">Full list of WBS Tag</param>
        /// <param name="parentGuid">Parent of the current selection</param>
        /// <returns>First Instance of tag's WBS guid</returns>
        public Guid? FirstParentWBSGuid(List<wbsTagDisplay> allWBSTagDisplay, Guid parentGuid)
        {
            foreach (wbsTagDisplay wbsTagDisplay in allWBSTagDisplay)
            {
                if (wbsTagDisplay.wbsTagDisplayGuid == parentGuid)
                {
                    if (wbsTagDisplay.wbsTagDisplayAttachWBS != null)
                        return wbsTagDisplay.wbsTagDisplayGuid;
                    else
                        //if its a tag we continue to find its WBS
                        return FirstParentWBSGuid(allWBSTagDisplay, wbsTagDisplay.wbsTagDisplayParentGuid);
                }
            }

            return null;
        }
        #endregion
    }
}
