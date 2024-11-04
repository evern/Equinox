using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectDatabase.DataAdapters;
using ProjectCommon;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraEditors.Controls; //for punchlist combobox
using System.Linq;
using DevExpress.Office;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Camera;

namespace CheckmateDX
{
    public partial class frmPunchlist_Main : CheckmateDX.frmParent
    {
        private Punchlist _editPunchlist;
        private Punchlist _Punchlist;

        private bool _askForProgress = true; //used for browse form to know whether to ask for progress when criteria has been met
        private Guid _selectedTemplateGuid = Guid.Empty;
        private string _discipline = string.Empty;
        private Action<frmPunchlist_Main> punchlist_update;
        private List<wbsTagDisplay> allWBStags;
        private List<Tuple<Guid, Image>> _ImageSource = new List<Tuple<Guid, Image>>();
        private List<Tuple<Guid, Image>> _RemedialImageSource = new List<Tuple<Guid, Image>>();
        private AdapterPUNCHLIST_MAIN_PICTURE _adapterPUNCHLIST_MAIN_PICTURE = new AdapterPUNCHLIST_MAIN_PICTURE();
        public frmPunchlist_Main(List<wbsTagDisplay> wbsTagDisplay, wbsTagDisplay selectedWBSTag, Guid templateGuid, string discipline, Action<frmPunchlist_Main> update_punchlist = null) //invoked from ITR and punchlist browser for creating
        {
            InitializeComponent();
            _discipline = discipline;
            txtTitle.Enter += new EventHandler(Common.textBox_GotFocus);
            txtDescription.Enter += new EventHandler(Common.textBox_GotFocus);
            txtInspectionNote.Enter += new EventHandler(Common.textBox_GotFocus);
            ProcessImageSource(PunchlistImageType.Inspection);
            ProcessImageSource(PunchlistImageType.Remedial);
            BuildWBSTagSearchComboBox(wbsTagDisplay);
            if (selectedWBSTag != null)
                searchLookUpTag.EditValue = selectedWBSTag;

            _selectedTemplateGuid = templateGuid;
            searchLookUpTag.EditValueChanged += searchLookUpTag_EditValueChanged;

            PopulateITR(selectedWBSTag, templateGuid);
            this.Text = "Add Punchlist";
            PopulateFormElements();

            //if ((Guid)System_Environment.GetUser().userRole.Value != Guid.Empty) //Only superuser will be able to edit discipline
            //{
            //    cmbDiscipline.ReadOnly = true;
            //    //layoutControlItemDiscipline.HideToCustomization();
            //}

            if(!System_Environment.HasPrivilege(PrivilegeTypeID.MarkPunchlistCategorised))
            {
                layoutControlItemCategory.HideToCustomization();
                layoutControlItemActionBy.HideToCustomization();
                layoutControlItemPriority.HideToCustomization();
            }

            layoutControlItemPunchlistItemEdit.HideToCustomization();
            layoutControlItemTagWBSEdit.HideToCustomization();
            layoutControlItemSubsystemEdit.HideToCustomization();

            layoutControlItemProgress.ContentVisible = false;
            layoutControlItemReject.ContentVisible = false;
            layoutControlItemDelete.ContentVisible = false;
            //layoutControlItemProgress.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //layoutControlItemReject.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //layoutControlItemDelete.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            //btnProgress.Visible = false;
            //btnReject.Visible = false;
            //btnDelete.Visible = false;
            layoutControlItemInpsectionNote.HideToCustomization();
            punchlist_update = update_punchlist;
        }

        #region Gallery
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            populateGallery(_editPunchlist, PunchlistImageType.Remedial);
        }

        private void ProcessImageSource(PunchlistImageType punchlistImageType)
        {
            GalleryControl galleryControl = punchlistImageType == PunchlistImageType.Inspection ? galleryControlInspection : galleryControlRemedial;
            galleryControl.Gallery.BeginUpdate();
            ClearGalleryAndImages(punchlistImageType);
            GalleryItemGroup group = CreateFolderGroup();
            galleryControl.Gallery.Groups.Add(group);

            List<Tuple<Guid, Image>> imageSource = punchlistImageType == PunchlistImageType.Inspection ? _ImageSource : _RemedialImageSource;
            foreach (Tuple<Guid, Image> image in imageSource)
            {
                group.Items.Add(CreatePhotoGalleryItem(image));
            }

            galleryControl.Gallery.EndUpdate();
            galleryControl.Refresh();
        }

        private void ClearGalleryAndImages(PunchlistImageType punchlistImageType)
        {
            GalleryControl galleryControl = punchlistImageType == PunchlistImageType.Inspection ? galleryControlInspection : galleryControlRemedial;
            galleryControl.Gallery.Groups.Clear();
            foreach (GalleryItemGroup group in galleryControl.Gallery.Groups)
            {
                if (group.CaptionControl != null)
                {
                    group.CaptionControl.Dispose();
                    group.CaptionControl = null;
                    foreach (GalleryItem item in group.Items)
                    {
                        if (item.Image != null)
                        {
                            item.Image.Dispose();
                            item.Image = null;
                        }
                    }
                }
            }
        }

        private GalleryItemGroup CreateFolderGroup()
        {
            GalleryItemGroup group = new GalleryItemGroup();
            group.Tag = "";
            group.Caption = "";
            group.CaptionAlignment = GalleryItemGroupCaptionAlignment.Stretch;
            return group;
        }

        private GalleryItem CreatePhotoGalleryItem(Tuple<Guid, Image> image)
        {
            GalleryItem item = new GalleryItem();
            item.Tag = image;
            return item;
        }

        private void galleryControlGallery1_GetThumbnailImage(object sender, DevExpress.XtraBars.Ribbon.Gallery.GalleryThumbnailImageEventArgs e)
        {
            if (e.Item.Tag != null)
            {
                try
                {
                    e.ThumbnailImage = e.CreateThumbnailImage(((Tuple<Guid, Image>)e.Item.Tag).Item2);
                }
                catch { }
            }
        }
        private void galleryControl1_Gallery_ItemDoubleClick(object sender, GalleryItemClickEventArgs e)
        {
            if (e.Item.Tag == null)
                return;

            Tuple<Guid, Image> galleryItemImage = (Tuple<Guid, Image>)e.Item.Tag;
            viewImage(galleryItemImage, PunchlistImageType.Inspection);
        }

        private void galleryControlRemedial_Gallery_ItemDoubleClick(object sender, GalleryItemClickEventArgs e)
        {
            if (e.Item.Tag == null)
                return;

            Tuple<Guid, Image> galleryItemImage = (Tuple<Guid, Image>)e.Item.Tag;
            viewImage(galleryItemImage, PunchlistImageType.Remedial);
        }

        private void galleryControl1_Gallery_ContextButtonClick(object sender, DevExpress.Utils.ContextItemClickEventArgs e)
        {
            galleryContextButtonClick(e, PunchlistImageType.Inspection);
        }

        private void galleryControlRemedial_Gallery_ContextButtonClick(object sender, DevExpress.Utils.ContextItemClickEventArgs e)
        {
            galleryContextButtonClick(e, PunchlistImageType.Remedial);
        }

        private void galleryContextButtonClick(DevExpress.Utils.ContextItemClickEventArgs e, PunchlistImageType punchlistImageType)
        {
            GalleryItem galleryItem = e.DataItem as GalleryItem;
            switch (e.Item.Name)
            {
                case "contextButtonRemove":
                    if (Common.Confirmation("Are you sure you want to delete this image?", "Delete Image"))
                    {
                        RemoveImageFromAlbum(galleryItem, punchlistImageType);
                        if (_editPunchlist != null && _editPunchlist.GUID != Guid.Empty)
                        {
                            splashScreenManager1.ShowWaitForm();
                            _adapterPUNCHLIST_MAIN_PICTURE.RemoveBy(((Tuple<Guid, Image>)galleryItem.Tag).Item1);
                            splashScreenManager1.CloseWaitForm();
                        }
                    }
                    break;
                case "contextButtonView":
                    if (galleryItem.Tag == null)
                        return;

                    Tuple<Guid, Image> galleryItemImage = (Tuple<Guid, Image>)galleryItem.Tag;
                    viewImage(galleryItemImage, punchlistImageType);
                    break;
            }
        }

        private void viewImage(Tuple<Guid, Image> image, PunchlistImageType punchlistImageType)
        {
            List<Tuple<Guid, Image>> imageSource = punchlistImageType == PunchlistImageType.Inspection ? _ImageSource : _RemedialImageSource;
            int imageIndex = imageSource.IndexOf(image);
            frmImageViewer frmImageViewer = new frmImageViewer(imageSource.Select(x => x.Item2).ToList(), imageIndex);
            frmImageViewer.ShowDialog();
        }

        private void RemoveImageFromAlbum(GalleryItem item, PunchlistImageType punchlistImageType)
        {
            List<Tuple<Guid, Image>> imageSource = punchlistImageType == PunchlistImageType.Inspection ? _ImageSource : _RemedialImageSource;
            imageSource.Remove((Tuple<Guid, Image>)item.Tag);

            //UpdateData();
            ProcessImageSource(punchlistImageType);
        }
        #endregion

        public frmPunchlist_Main(Punchlist editPunchlist, List<wbsTagDisplay> wbsTagDisplay, Action<frmPunchlist_Main> update_punchlist = null) //invoked from punchlist browser for editing
        {
            InitializeComponent();
            punchlist_update = update_punchlist;
            txtTitle.Enter += new EventHandler(Common.textBox_GotFocus);
            txtDescription.Enter += new EventHandler(Common.textBox_GotFocus);
            txtInspectionNote.Enter += new EventHandler(Common.textBox_GotFocus);
            this.Text = "Edit Punchlist";
            btnSave.Text = "Save";
            PopulateFormElements(editPunchlist);
            _editPunchlist = editPunchlist;

            layoutControlItemTag.HideToCustomization();
            layoutControlItemITR.HideToCustomization();
            layoutControlItemPunchlistItem.HideToCustomization();

            textEditTagWBSEdit.Text = editPunchlist.punchlistDisplayAttachment;
            textEditPunchlistItemEdit.Text = editPunchlist.punchlistItem;
            textEditSubsystemEdit.Text = string.Concat(editPunchlist.punchlistParentWBSName, " - ", editPunchlist.punchlistParentWBSDesc);
            //saves the wbstag information in searchLookUpTag
            //BuildWBSTagSearchComboBox(wbsTagDisplay);
            //wbsTagDisplay findWBSTag;
            //if (editPunchlist.punchlistAttachTag != null)
            //    findWBSTag = wbsTagDisplay.FirstOrDefault(obj => obj.wbsTagDisplayAttachTag.GUID == editPunchlist.punchlistAttachTag.GUID);
            //else
            //    findWBSTag = wbsTagDisplay.FirstOrDefault(obj => obj.wbsTagDisplayAttachWBS.GUID == editPunchlist.punchlistAttachWBS.GUID);
            //searchLookUpTag.EditValue = findWBSTag;

            //if ((Guid)System_Environment.GetUser().userRole.Value != Guid.Empty) //Only superuser will be able to edit discipline
            //{
            //    cmbDiscipline.ReadOnly = true;
            //    //layoutControlItemDiscipline.HideToCustomization();
            //}

            if (editPunchlist.punchlistStatus == -1) //saved
            {
                if (!System_Environment.HasPrivilege(PrivilegeTypeID.MarkPunchlistCategorised))
                {
                    cmbCategory.Properties.ReadOnly = true;
                    cmbActionBy.Properties.ReadOnly = true;
                    cmbPriority.Properties.ReadOnly = true;
                }

                if (!System_Environment.HasPrivilege(PrivilegeTypeID.CreatePunchlist))
                {
                    layoutControlItemTitle.HideToCustomization();
                    layoutControlItemDescription.HideToCustomization();
                }

                if(!System_Environment.HasPrivilege(PrivilegeTypeID.MarkPunchlistCategorised) && !System_Environment.HasPrivilege(PrivilegeTypeID.CreatePunchlist))
                {
                    btnSave.Tag = "create and categorise";
                }

                layoutControlItemInpsectionNote.HideToCustomization();
                btnReject.Visible = false;
                btnProgress.Text = "Categorised";
            }
            else if(editPunchlist.punchlistStatus == (int)Punchlist_Status.Categorised)
            {
                if (!System_Environment.HasPrivilege(PrivilegeTypeID.MarkPunchlistInspected))
                {
                    txtInspectionNote.Enabled = false;
                    btnSave.Tag = "inspect"; //used to disable user from performing save operation
                }

                txtTitle.Properties.ReadOnly = true;
                txtDescription.Properties.ReadOnly = true;
                cmbCategory.Properties.ReadOnly = true;
                cmbActionBy.Properties.ReadOnly = true;
                cmbPriority.Properties.ReadOnly = true;

                //skip inspected and approved
                btnProgress.Text = "Inspected";
                //btnProgress.Text = "Inspected";
            }
            else if(editPunchlist.punchlistStatus == (int)Punchlist_Status.Inspected || editPunchlist.punchlistStatus == (int)Punchlist_Status.Approved || editPunchlist.punchlistStatus == (int)Punchlist_Status.Completed)
            {
                txtTitle.Properties.ReadOnly = true;
                txtDescription.Properties.ReadOnly = true;
                txtInspectionNote.Properties.ReadOnly = true;
                cmbCategory.Properties.ReadOnly = true;
                cmbActionBy.Properties.ReadOnly = true;
                cmbPriority.Properties.ReadOnly = true;
                btnSave.Visible = false;
                if (editPunchlist.punchlistStatus == (int)Punchlist_Status.Inspected)
                    btnProgress.Text = "Complete"; //Approve is skipped
                else if (editPunchlist.punchlistStatus == (int)Punchlist_Status.Approved)
                    btnProgress.Text = "Complete";
                else
                    btnProgress.Text = "Close";
            }
            else //closed
            {
                txtTitle.Properties.ReadOnly = true;
                txtDescription.Properties.ReadOnly = true;
                txtInspectionNote.Properties.ReadOnly = true;
                cmbCategory.Properties.ReadOnly = true;
                cmbActionBy.Properties.ReadOnly = true;
                cmbPriority.Properties.ReadOnly = true;
                btnSave.Enabled = false;
                btnProgress.Enabled = false;
                btnReject.Enabled = false;
            }
        }

        #region Initialisation Methods
        /// <summary>
        /// Populate form element for adding
        /// </summary>
        private void PopulateFormElements()
        {
            Common.PopulateCmbAuthDiscipline(cmbDiscipline, false, _discipline);
            Common.PopulateCmbCategory(cmbCategory, string.Empty);
            Common.PopulateCmbActionBy(cmbActionBy, string.Empty);
            Common.PopulateCmbPriority(cmbPriority, string.Empty);
            cmbPriority.SelectedIndex = 1;
        }

        /// <summary>
        /// Populate form element for editing
        /// </summary>
        /// <param name="selectedParent">Parent punchlist guid</param>
        private void PopulateFormElements(Punchlist editPunchlist)
        {
            _editPunchlist = editPunchlist;
            txtTitle.Text = editPunchlist.punchlistTitle;
            txtDescription.Text = editPunchlist.punchlistDescription;
            txtInspectionNote.Text = editPunchlist.punchlistRemedial;
            populateGallery(editPunchlist, PunchlistImageType.Inspection);

            //use timer to populate gallery because async operation hasn't completed yet for inspection, causing unexpected behaviour
            timer1.Start();
            Common.PopulateCmbCategory(cmbCategory, Common.Replace_WithSpaces(editPunchlist.punchlistCategory));
            Common.PopulateCmbActionBy(cmbActionBy, Common.Replace_WithSpaces(editPunchlist.punchlistActionBy));
            Common.PopulateCmbPriority(cmbPriority, Common.Replace_WithSpaces(editPunchlist.punchlistPriority));
            Common.PopulateCmbDiscipline(cmbDiscipline, Common.Replace_WithSpaces(editPunchlist.punchlistDiscipline));
        }

        private void populateGallery(Punchlist editPunchlist, PunchlistImageType punchlistImageType)
        {
            dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTUREDataTable dtPUNCHLIST_MAIN_PICTURE = _adapterPUNCHLIST_MAIN_PICTURE.GetBy(editPunchlist.GUID, punchlistImageType);
            if(dtPUNCHLIST_MAIN_PICTURE != null)
            {
                foreach(dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURERow drPUNCHLIST_MAIN_PICTURE in dtPUNCHLIST_MAIN_PICTURE.Rows)
                {
                    Image image = Image.FromStream(new System.IO.MemoryStream(drPUNCHLIST_MAIN_PICTURE.PICTURE));
                    addToImageCollection(drPUNCHLIST_MAIN_PICTURE.GUID, image, punchlistImageType);
                }

                ProcessImageSource(punchlistImageType);
            }
        }

        /// <summary>
        /// Build up the search edit combobox
        /// </summary>
        private void BuildWBSTagSearchComboBox(List<wbsTagDisplay> wbsTagDisplay)
        {
            searchLookUpTag.Properties.DataSource = wbsTagBindingSource;
            allWBStags = wbsTagDisplay;
            wbsTagBindingSource.DataSource = allWBStags;

            searchLookUpEdit1View.Columns.AddField("wbsTagDisplayName").Visible = true;
            searchLookUpEdit1View.Columns.AddField("wbsTagDisplayDescription").Visible = true;

            searchLookUpEdit1View.Columns.ColumnByFieldName("wbsTagDisplayName").Caption = "Tag";
            searchLookUpEdit1View.Columns.ColumnByFieldName("wbsTagDisplayName").MaxWidth = 200;
            searchLookUpEdit1View.Columns.ColumnByFieldName("wbsTagDisplayDescription").Caption = "Description";
        }

        private void PopulateITR(wbsTagDisplay selectedWBSTag, Guid templateGuid)
        {
            PopulateCmbPunchlistITR(cmbITR, selectedWBSTag, templateGuid);
        }

        /// <summary>
        /// Populates combobox with WBS/Tag ITR
        /// </summary>
        public void PopulateCmbPunchlistITR(ComboBoxEdit cmbITR, wbsTagDisplay selectedWBSTag, Guid selectedTemplateGuid)
        {
            ComboBoxItemCollection coll = cmbITR.Properties.Items;
            coll.Clear();
            if (selectedWBSTag == null)
            {
                coll.BeginUpdate();
                coll.Add(Variables.punchlistAdhoc); //prompt the user to select the WBS/Tag
                cmbITR.SelectedIndex = 0;
                coll.EndUpdate();
                return;
            }

            PunchlistITRTemplate vpPLAdHoc = new PunchlistITRTemplate(Variables.punchlistAdhoc, Guid.Empty, Guid.Empty); //create it in a valuepair to ensure that guid empty is used for adhoc punchlist
            coll.Add(vpPLAdHoc);
            using (AdapterITR_MAIN daITR = new AdapterITR_MAIN())
            {
                coll.BeginUpdate();
                List<PunchlistITRTemplate> vpWBSTagTemplates = daITR.GetPunchlistVP_WBSTagITR(selectedWBSTag.wbsTagDisplayGuid);
                bool hasOpenPunchlist = false;
                foreach (PunchlistITRTemplate vpWBSTagTemplate in vpWBSTagTemplates)
                {
                    ComboBoxEdit mockComboBox = new ComboBoxEdit();
                    PopulatePunchlistItem(vpWBSTagTemplate.ITRGuid, mockComboBox);
                    if (((ValuePair)mockComboBox.SelectedItem).Value.ToString() != string.Empty)
                        hasOpenPunchlist = true;

                    coll.Add(vpWBSTagTemplate);
                }

                int i = FindTemplateInPopulateCmbPunchlistITR(cmbITR, selectedTemplateGuid);
                if (i != -1 && hasOpenPunchlist)
                    cmbITR.SelectedIndex = i;
                else
                    cmbITR.SelectedIndex = 0;

                coll.EndUpdate();
                return;
            }
        }

        private void PopulatePunchlistItem(Guid iTRGuid, ComboBoxEdit comboBox)
        {
            using (AdapterITR_MAIN daITR = new AdapterITR_MAIN())
            {
                dsITR_MAIN.ITR_MAINRow drITR = daITR.GetBy(iTRGuid);
                if (drITR != null)
                {
                    MemoryStream ms = new MemoryStream(drITR.ITR);
                    CustomRichEdit richEditITR = new CustomRichEdit();
                    richEditITR.Document.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                    Guid wbsTagGuid = Guid.Empty;
                    if (drITR.IsWBS_GUIDNull())
                        wbsTagGuid = drITR.TAG_GUID;
                    else
                        wbsTagGuid = drITR.WBS_GUID;

                    richEditITR.PopulateCmbPunchlistItem(comboBox, string.Empty, wbsTagGuid, iTRGuid);
                }
                else
                {
                    ComboBoxItemCollection coll = cmbPunchlistItem.Properties.Items;
                    coll.Clear();
                    comboBox.Text = string.Empty;
                }
            }
        }

        private int FindTemplateInPopulateCmbPunchlistITR(ComboBoxEdit cmbEdit, Guid templateGuid)
        {
            for (int i = 0; i < cmbEdit.Properties.Items.Count; i++)
            {
                PunchlistITRTemplate searchPIT = (PunchlistITRTemplate)cmbEdit.Properties.Items[i];
                if (searchPIT.TemplateGuid == templateGuid)
                    return i;
            }

            return -1;
        }

        private void UpdateITRItem(Guid iTRGuid, string updatePunchlistItem, string updateStatus)
        {
            using(AdapterITR_MAIN daITR = new AdapterITR_MAIN())
            {
                dsITR_MAIN.ITR_MAINRow drITR = daITR.GetBy(iTRGuid);
                if(drITR != null)
                {
                    MemoryStream ms = new MemoryStream(drITR.ITR);
                    CustomRichEdit richEditITR = new CustomRichEdit();
                    richEditITR.Document.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);

                    RangePermissionCollection RangePermissions = richEditITR.Document.BeginUpdateRangePermissions();

                    bool permissionUpdated = false;
                    foreach (RangePermission rp in RangePermissions)
                    {
                        if (rp.Group.Contains(CustomUserGroupListService.TOGGLE_ACCEPTANCE))
                        {
                            string rangePermissionText = richEditITR.Document.GetText(rp.Range);
                            if(rangePermissionText.Contains(updatePunchlistItem))
                            {
                                richEditITR.Document.BeginUpdate();


                                TableCell tCell = richEditITR.Document.Tables.GetTableCell(rp.Range.End);
                                TableRow tCellRow = tCell.Row;
                                TableCell tCellLast = tCellRow.LastCell;

                                int replaceLength = (tCellLast.Range.End.ToInt() - 1) - (tCellLast.Range.Start.ToInt());

                                DocumentRange replaceRange = richEditITR.Document.CreateRange(tCellLast.Range.Start.ToInt(), replaceLength);
                                if (replaceLength > 0)
                                    richEditITR.Document.Replace(replaceRange, " "); //leave a space to retain the editable permission

                                DocumentPosition insertDocPosition = richEditITR.Document.CreatePosition(replaceRange.Start.ToInt() + 1);
                                if (_editPunchlist.punchlistRemedial == string.Empty)
                                    richEditITR.Document.InsertText(insertDocPosition, "Title: " + _editPunchlist.punchlistTitle + Environment.NewLine + "Date: " + DateTime.Now.ToString("d"));
                                else
                                    richEditITR.Document.InsertText(insertDocPosition, "Title: " + _editPunchlist.punchlistTitle + Environment.NewLine + "Note: " + _editPunchlist.punchlistRemedial + Environment.NewLine + "Date: " + DateTime.Now.ToString("d"));

                                string currentPunchlistText = richEditITR.Document.GetText(rp.Range).Split(new string[] { Variables.punchlistStatusDelimiter }, StringSplitOptions.None).First();
                                richEditITR.Document.Replace(rp.Range, currentPunchlistText + Variables.punchlistStatusDelimiter + updateStatus);


                                permissionUpdated = true;
                                break;
                            }
                        }
                    }

                    if(permissionUpdated)
                    {
                        richEditITR.Document.EndUpdateRangePermissions(RangePermissions);
                        richEditITR.Document.EndUpdate();
                        MemoryStream updatedMS = new MemoryStream();
                        richEditITR.SaveDocument(updatedMS, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                        drITR.ITR = updatedMS.ToArray();
                        drITR.UPDATED = DateTime.Now;
                        drITR.UPDATEDBY = System_Environment.GetUser().GUID;
                        daITR.Save(drITR);
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public Punchlist GetPunchlist()
        {
            return _Punchlist;
        }

        public List<Image> GetPunchlistImages(PunchlistImageType punchlistImageType)
        {
            List<Tuple<Guid, Image>> imageSource = punchlistImageType == PunchlistImageType.Inspection ? _ImageSource : _RemedialImageSource;
            return imageSource.Select(x => x.Item2).ToList();
        }

        public bool AskForProgression()
        {
            return _askForProgress;
        } 
        #endregion

        #region Validation
        private bool ValidateFormElements()
        {
            if (txtTitle.Text.Trim() == string.Empty)
            {
                Common.Warn("Title cannot be empty");
                txtTitle.Focus();
                return false;
            }

            if (txtDescription.Text.Trim() == string.Empty)
            {
                Common.Warn("Description cannot be empty");
                txtDescription.Focus();
                return false;
            }

            if (cmbDiscipline.Text.Trim() == string.Empty)
            {
                Common.Warn("Discipline cannot be empty");
                cmbDiscipline.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks for duplicate Punchlist
        /// </summary>
        private bool ValidatePunchlist(Guid validateGuid)
        {
            using (AdapterPUNCHLIST_MAIN daPunchlist = new AdapterPUNCHLIST_MAIN())
            {
                if (daPunchlist.GetBy(validateGuid, txtTitle.Text.Trim()) != null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks for duplicate Punchlist but exclude current punchlist
        /// </summary>
        private bool ValidatePunchlistEdit(Guid editPunchlistGuid)
        {
            using (AdapterPUNCHLIST_MAIN daPunchlist = new AdapterPUNCHLIST_MAIN())
            {
                dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlist = daPunchlist.GetBy(editPunchlistGuid, txtTitle.Text.Trim());
                if (drPunchlist != null && drPunchlist.GUID != editPunchlistGuid)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if form is filled before progressing
        /// </summary>
        private bool ValidateProgress(int currentPunchlistStatus)
        {
            if (currentPunchlistStatus == -1)
            {
                //if (cmbCategory.Text.Trim() == Variables.SelectOne)
                //{
                //    Common.Warn("Category must be selected to progress");
                //    cmbCategory.Focus();
                //    return false;
                //}

                if (cmbActionBy.Text.Trim() == Variables.SelectOne)
                {
                    Common.Warn("Action By must be selected to progress");
                    cmbActionBy.Focus();
                    return false;
                }

                if (cmbPriority.Text.Trim() == Variables.SelectOne)
                {
                    Common.Warn("Priority must be selected to progress");
                    cmbPriority.Focus();
                    return false;
                }
            }

            if (currentPunchlistStatus == (int)Punchlist_Status.Categorised)
            {
                if (txtInspectionNote.Text.Trim() == string.Empty)
                {
                    Common.Warn("Inspection note must be entered to progress");
                    txtInspectionNote.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool ValidateProgressAuthorisation()
        {
            if (btnProgress.Text == "Progress" && !System_Environment.HasPrivilege(PrivilegeTypeID.MarkPunchlistCategorised))
            {
                Common.Warn("You are not authorised to categorise a punchlist");
                return false;
            }
            else if (btnProgress.Text == "Inspected" && !System_Environment.HasPrivilege(PrivilegeTypeID.MarkPunchlistInspected))
            {
                Common.Warn("You are not authorised to inspect a punchlist");
                return false;
            }
            else if (btnProgress.Text == "Approve" && !System_Environment.HasPrivilege(PrivilegeTypeID.MarkPunchlistApproved))
            {
                Common.Warn("You are not authorised to approve a punchlist");
                return false;
            }
            else if (btnProgress.Text == "Complete" && !System_Environment.HasPrivilege(PrivilegeTypeID.MarkPunchlistCompleted))
            {
                Common.Warn("You are not authorised to complete a punchlist");
                return false;
            }
            else if (btnProgress.Text == "Close" && !System_Environment.HasPrivilege(PrivilegeTypeID.MarkPunchlistClosed))
            {
                Common.Warn("You are not authorised to close a punchlist");
                return false;
            }

            return true;
        }
        #endregion

        #region Events
        private void btnShowITR_Click(object sender, EventArgs e)
        {
            if(cmbITR.SelectedIndex == 0)
            {
                Common.Warn("There is no ITR to preview because this is an Ad-Hoc punchlist");
                return;
            }

            if (_editPunchlist != null)
            {
                frmITR_Main frmITR = new frmITR_Main(_editPunchlist.punchlistITR.ITRGuid, _editPunchlist.punchlistAttachTag);
                frmITR.ShowDialog();
            }
        }

        private void cmbPunchlistItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPunchlistItem.EditValue.GetType() == typeof(string))
                return;

            string[] titleDescription = ((ValuePair)cmbPunchlistItem.EditValue).Value.ToString().Split(Variables.delimiter);
            if(titleDescription.Count() > 1)
            {
                txtTitle.Text = titleDescription[0];
                txtDescription.Text = titleDescription[1];
            }
        }

        private void cmbITR_EditValueChanged(object sender, EventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            if (cmbITR.EditValue.ToString() == Variables.punchlistAdhoc)
            {
                layoutControlItemPunchlistItem.HideToCustomization();
                txtTitle.Text = string.Empty;
            }
            else
            {
                //pnlPunchlistItem.Visible = true;
                PopulatePunchlistItem((Guid)((PunchlistITRTemplate)cmbITR.SelectedItem).ITRGuid, cmbPunchlistItem);
            }

            splashScreenManager1.CloseWaitForm();
        }

        private void searchLookUpTag_EditValueChanged(object sender, EventArgs e)
        {
            PopulateITR((wbsTagDisplay)searchLookUpTag.EditValue, _selectedTemplateGuid);
        }

        public void btnSave_Click(object sender, EventArgs e)
        {
            if(btnSave.Tag != null)
            {
                Common.Warn("You are not authorised to " + btnSave.Tag.ToString() + " a punchlist");
                return;
            }

            if (cmbITR.Text == Variables.punchlistAdhoc)
                cmbPunchlistItem.Text = string.Empty;

            if (cmbPunchlistItem.SelectedItem.ToString() == Variables.punchlistNotAvailable) //checks whether punchlist can be added to the ITR
            {
                Common.Warn("Please add ad-hoc punchlist\nBecause punchlist is not available in this ITR");
                return;
            }

            if (ValidateFormElements())
            {
                if (_editPunchlist != null && ValidatePunchlistEdit(_editPunchlist.GUID))
                {
                    _editPunchlist.punchlistTitle = txtTitle.Text.Trim();
                    _editPunchlist.punchlistDescription = txtDescription.Text.Trim();
                    _editPunchlist.punchlistRemedial = txtInspectionNote.Text.Trim();
                    _editPunchlist.punchlistDiscipline = cmbDiscipline.Text.Trim();
                    _editPunchlist.punchlistCategory = cmbCategory.Text.Trim().Equals(Variables.SelectOne) ? string.Empty : cmbCategory.Text.Trim();
                    _editPunchlist.punchlistActionBy = cmbActionBy.Text.Trim().Equals(Variables.SelectOne) ? string.Empty : cmbActionBy.Text.Trim();
                    _editPunchlist.punchlistPriority = cmbPriority.Text.Trim().Equals(Variables.SelectOne) ? string.Empty : cmbPriority.Text.Trim();
                    _Punchlist = _editPunchlist;
                }
                else
                {
                    if (searchLookUpTag.EditValue == null || searchLookUpTag.EditValue.ToString() == string.Empty)
                    {
                        Common.Warn("Please select a tag number");
                        return;
                    }

                    wbsTagDisplay selectedWBSTag = ((wbsTagDisplay)searchLookUpTag.EditValue);

                    if (_editPunchlist != null || ValidatePunchlist(selectedWBSTag.wbsTagDisplayGuid))
                    {
                        string punchlistUniqueNumber = cmbPunchlistItem.SelectedItem.ToString();
                        if (cmbITR.EditValue != null && cmbITR.EditValue.ToString() == Variables.punchlistAdhoc)
                            punchlistUniqueNumber = Toggle_Acceptance.Punchlisted.ToString() + Variables.punchlistAffix + Common.GetProjectPunchlistCount(txtTitle.Text.Trim()).ToString();

                        Punchlist newPunchlist = new Punchlist(Guid.NewGuid());

                        newPunchlist.punchlistAttachTag = selectedWBSTag.wbsTagDisplayAttachTag ?? null;
                        newPunchlist.punchlistAttachWBS = selectedWBSTag.wbsTagDisplayAttachWBS ?? null;
                        newPunchlist.punchlistITR = (PunchlistITRTemplate)cmbITR.SelectedItem;
                        newPunchlist.punchlistItem = punchlistUniqueNumber;
                        newPunchlist.punchlistDisplayAttachment = selectedWBSTag.wbsTagDisplayName;
                        newPunchlist.punchlistTitle = txtTitle.Text.Trim();
                        newPunchlist.punchlistDescription = txtDescription.Text.Trim();
                        newPunchlist.punchlistRemedial = string.Empty;
                        newPunchlist.punchlistDiscipline = cmbDiscipline.Text.Trim();
                        newPunchlist.punchlistParentWBSGuid = selectedWBSTag.wbsTagDisplayAttachTag == null ? selectedWBSTag.wbsTagDisplayAttachWBS == null ? (Guid?)null : (Guid)selectedWBSTag.wbsTagDisplayAttachWBS.GUID : (Guid)selectedWBSTag.wbsTagDisplayAttachTag.tagParentGuid;
                        newPunchlist.punchlistParentWBSName = selectedWBSTag.wbsTagDisplayAttachTag == null ? selectedWBSTag.wbsTagDisplayAttachWBS == null ? string.Empty : selectedWBSTag.wbsTagDisplayAttachWBS.wbsName : findWBSName(selectedWBSTag.wbsTagDisplayAttachTag.tagParentGuid);
                        newPunchlist.punchlistParentWBSDesc = selectedWBSTag.wbsTagDisplayAttachTag == null ? selectedWBSTag.wbsTagDisplayAttachWBS == null ? string.Empty : selectedWBSTag.wbsTagDisplayAttachWBS.wbsDescription : Common.FindWBSDesc(allWBStags, selectedWBSTag.wbsTagDisplayAttachTag.tagParentGuid);
                        newPunchlist.punchlistCategory = cmbCategory.Text.Trim().Equals(Variables.SelectOne) ? string.Empty : cmbCategory.Text.Trim();
                        newPunchlist.punchlistActionBy = cmbActionBy.Text.Trim().Equals(Variables.SelectOne) ? string.Empty : cmbActionBy.Text.Trim();
                        newPunchlist.punchlistPriority = cmbPriority.Text.Trim().Equals(Variables.SelectOne) ? string.Empty : cmbPriority.Text.Trim();

                        _Punchlist = newPunchlist;
                        //NewselectedWBSTag.wbsTagDisplayEnabled = true;
                    }
                    else
                    {
                        Common.Warn("Title already exists");
                        return;
                    }
                }

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
                punchlist_update?.Invoke(this);
            }
        }

        private string findWBSName(Guid wbsGuid)
        {
            wbsTagDisplay findWBSTagDisplay = allWBStags.FirstOrDefault(x => x.wbsTagDisplayGuid == wbsGuid);
            if (findWBSTagDisplay != null)
                return findWBSTagDisplay.wbsTagDisplayName;
            else
                return string.Empty;
        }

        public void SetCategory(string category)
        {
            cmbCategory.Text = category;
        }

        public void SetActionBy(string actionBy)
        {
            cmbActionBy.Text = actionBy;
        }

        public void SetPriority(string priority)
        {
            cmbPriority.Text = priority;
        }

        public void btnProgress_Click(object sender, EventArgs e)
        {
            if (!ValidateProgressAuthorisation())
                return;

            AdapterPUNCHLIST_STATUS daPunchlistStatus = new AdapterPUNCHLIST_STATUS();
            AdapterPUNCHLIST_STATUS_ISSUE daPunchlistIssue = new AdapterPUNCHLIST_STATUS_ISSUE();

            try
            {
                //Skip approved
                int punchlistStatusDisplayOverride = _editPunchlist.punchlistStatus;
                if (punchlistStatusDisplayOverride == 1)
                    punchlistStatusDisplayOverride += 1;

                string strProgress = ((Punchlist_Status)(Enum.Parse(typeof(Punchlist_Status), punchlistStatusDisplayOverride.ToString())) + 1).ToString();
                    
                //ask for comment if punchlist was previously rejected
                if(_editPunchlist.punchlistImageIndex == 1 || _editPunchlist.punchlistImageIndex == 4)
                {
                    frmPunchlist_Comment f = new frmPunchlist_Comment(_editPunchlist.GUID, strProgress);
                    if (ValidateProgress(_editPunchlist.punchlistStatus) && f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Guid punchlistStatusGuid = Guid.NewGuid();
                        daPunchlistStatus.ChangeStatus(_editPunchlist.GUID, (Punchlist_Status)_editPunchlist.punchlistStatus, true, punchlistStatusGuid);
                        daPunchlistIssue.AddComments(punchlistStatusGuid, f.GetComment(), false);
                        _askForProgress = false;
                        btnSave_Click(null, null);
                        UpdateITRItem(_editPunchlist.punchlistITR.ITRGuid, _editPunchlist.punchlistItem, strProgress);
                    }
                }
                //progress without comments
                else
                {
                    if (ValidateProgress(_editPunchlist.punchlistStatus) && Common.Confirmation("Are you sure you want to mark this punchlist as " + btnProgress.Text.ToLower().Replace("&", ""), "Submit Punchlist"))
                    {
                        daPunchlistStatus.ChangeStatus(_editPunchlist.GUID, (Punchlist_Status)_editPunchlist.punchlistStatus, true, Guid.NewGuid());
                        //Common.Prompt("Punchlist Successfully " + strProgress);
                        _askForProgress = false;
                        btnSave_Click(null, null);
                        UpdateITRItem(_editPunchlist.punchlistITR.ITRGuid, _editPunchlist.punchlistItem, strProgress);
                    }
                }
            }
            catch
            {
                daPunchlistStatus.Dispose();
                daPunchlistIssue.Dispose();
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            string rejectedStatusString = " "; //leave a space to retain color

            if(_editPunchlist.punchlistStatus == (int)Punchlist_Status.Categorised)
            {
                if (!System_Environment.HasPrivilege(PrivilegeTypeID.RejectPunchlistCategorised))
                {
                    Common.Warn("You are not authorised to reject a categorised punchlist");
                    return;
                }
            }
            else if(_editPunchlist.punchlistStatus == (int)Punchlist_Status.Inspected)
            {
                rejectedStatusString = Punchlist_Status.Categorised.ToString(); 

                if (!System_Environment.HasPrivilege(PrivilegeTypeID.RejectPunchlistInspected))
                {
                    Common.Warn("You are not authorised to reject an inspected punchlist");
                    return;
                }
            }
            else if(_editPunchlist.punchlistStatus == (int)Punchlist_Status.Approved)
            {
                rejectedStatusString = Punchlist_Status.Inspected.ToString(); 

                if (!System_Environment.HasPrivilege(PrivilegeTypeID.RejectPunchlistApproved))
                {
                    Common.Warn("You are not authorised to reject an approved punchlist");
                    return;
                }
            }
            else if (_editPunchlist.punchlistStatus == (int)Punchlist_Status.Completed)
            {
                rejectedStatusString = Punchlist_Status.Approved.ToString();

                if (!System_Environment.HasPrivilege(PrivilegeTypeID.RejectPunchlistCompleted))
                {
                    Common.Warn("You are not authorised to reject a completed punchlist");
                    return;
                }
            }

            AdapterPUNCHLIST_STATUS daPunchlistStatus = new AdapterPUNCHLIST_STATUS();
            AdapterPUNCHLIST_STATUS_ISSUE daPunchlistIssue = new AdapterPUNCHLIST_STATUS_ISSUE();

            try
            {
                frmPunchlist_Comment f = new frmPunchlist_Comment(_editPunchlist.GUID, "Reject");
                if(f.ShowDialog() == DialogResult.OK)
                {
                    Guid punchlistStatusGuid = Guid.NewGuid();
                    daPunchlistStatus.ChangeStatus(_editPunchlist.GUID, (Punchlist_Status)_editPunchlist.punchlistStatus, false, punchlistStatusGuid);
                    daPunchlistIssue.AddComments(punchlistStatusGuid, f.GetComment(), true);
                    _askForProgress = false;
                    _Punchlist = _editPunchlist;
                    UpdateITRItem(_editPunchlist.punchlistITR.ITRGuid, _editPunchlist.punchlistItem, rejectedStatusString);
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                    punchlist_update?.Invoke(this);
                }
            }
            catch
            {
                daPunchlistStatus.Dispose();
                daPunchlistIssue.Dispose();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(!System_Environment.HasPrivilege(PrivilegeTypeID.DeletePunchlist))
            {
                Common.Warn("You are not authorised to delete a punchlist");
                return;
            }

            if (Common.Confirmation("Are you sure you want to delete this punchlist?", "Delete Punchlist"))
            {
                using(AdapterPUNCHLIST_MAIN daPunchlist = new ProjectDatabase.DataAdapters.AdapterPUNCHLIST_MAIN())
                {
                    dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlist = daPunchlist.GetBy(_editPunchlist.GUID);
                    if(drPunchlist != null)
                    {
                        daPunchlist.RemoveBy(drPunchlist.GUID);
                        _Punchlist = _editPunchlist;

                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                        punchlist_update?.Invoke(this);
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
            punchlist_update?.Invoke(this);
        }

        #endregion

        private void btnTakePhoto_Click(object sender, EventArgs e)
        {
            takePhoto(PunchlistImageType.Inspection);
        }

        private void btnTakeRemedialPictures_Click(object sender, EventArgs e)
        {
            takePhoto(PunchlistImageType.Remedial);
        }

        private void takePhoto(PunchlistImageType punchlistImageType)
        {
            TakePictureDialog d = new TakePictureDialog();
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                splashScreenManager1.ShowWaitForm();
                addToImageCollection(Guid.Empty, d.Image, punchlistImageType, true);
                splashScreenManager1.CloseWaitForm();
            }

            ProcessImageSource(punchlistImageType);
        }

        private void btnBrowsePicture_Click(object sender, EventArgs e)
        {
            browsePictures(PunchlistImageType.Inspection);
        }

        private void btnBrowseRemedialPicture_Click(object sender, EventArgs e)
        {
            browsePictures(PunchlistImageType.Remedial);
        }

        private void browsePictures(PunchlistImageType punchlistImageType)
        {
            using (frmITR_AttachImage frmAttachImage = new frmITR_AttachImage())
            {
                if (frmAttachImage.ShowDialog() == DialogResult.OK)
                {
                    SliderImageCollection imageCollection = frmAttachImage.GetImages();
                    if (imageCollection.Count > 0)
                    {
                        splashScreenManager1.ShowWaitForm();
                        foreach (Image image in imageCollection)
                        {
                            addToImageCollection(Guid.Empty, image, punchlistImageType, true);
                        }
                        splashScreenManager1.CloseWaitForm();
                    }

                    ProcessImageSource(punchlistImageType);
                }
            }
        }

        private void addToImageCollection(Guid photoGuid, Image photo, PunchlistImageType punchlistImageType, bool trySaveImage = false)
        {
            if(trySaveImage)
            {
                //only save when punchlist is in edit mode
                if(_editPunchlist != null && _editPunchlist.GUID != Guid.Empty && photoGuid == Guid.Empty)
                {
                    dsPUNCHLIST_MAIN_PICTURE.PUNCHLIST_MAIN_PICTURERow drPUNCHLIST_MAIN_PICTURE = _adapterPUNCHLIST_MAIN_PICTURE.SavePunchlistPicture(_editPunchlist.GUID, photo, punchlistImageType);
                    photoGuid = drPUNCHLIST_MAIN_PICTURE.GUID;
                }
            }

            //Bitmap bitmap = new Bitmap(photo);
            //Bitmap ResizedBitmap = Common.ResizeBitmap(bitmap, 300, 300);
            if(punchlistImageType == PunchlistImageType.Remedial)
                _RemedialImageSource.Add(new Tuple<Guid, Image>(photoGuid, photo));
            else
                _ImageSource.Add(new Tuple<Guid, Image>(photoGuid, photo));
        }
    }
}
