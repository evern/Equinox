using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectCommon;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectLibrary;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace CheckmateDX
{
    public partial class frmSchedule_Matrix : CheckmateDX.frmParent
    {
        AdapterMATRIX_TYPE _daMATRIX_TYPE = new AdapterMATRIX_TYPE();
        AdapterMATRIX_ASSIGNMENT _daMATRIX_ASSIGNMENT = new AdapterMATRIX_ASSIGNMENT();
        AdapterTEMPLATE_MAIN _daTEMPLATE_MAIN = new AdapterTEMPLATE_MAIN();
        List<MatrixType> _MatrixType = new List<MatrixType>();
        List<Template_Check> _TemplateChecks = new List<Template_Check>();

        public frmSchedule_Matrix()
        {
            InitializeComponent();
            Common.PopulateCmbAuthProject(cmbProject, false);
            gridControlType.DataSource = _MatrixType;
            gridControlTemplate.DataSource = _TemplateChecks;
            RefreshMatrixType();
            RefreshTemplate();
        }

        #region Add/Edit/Delete Matrix Type
        private void btnAddType_Click(object sender, EventArgs e)
        {
            dsMATRIX_TYPE dsMatrixType = new dsMATRIX_TYPE();
            frmSchedule_Matrix_Add frmMatrixAdd = new frmSchedule_Matrix_Add();
            if (frmMatrixAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MatrixType newMatrixType = frmMatrixAdd.GetMatrixType();
                dsMATRIX_TYPE.MATRIX_TYPERow drMatrixType = dsMatrixType.MATRIX_TYPE.NewMATRIX_TYPERow();
                drMatrixType.GUID = Guid.NewGuid();
                AssignMatrixDetail(drMatrixType, newMatrixType);
                drMatrixType.CREATED = DateTime.Now;
                drMatrixType.CREATEDBY = System_Environment.GetUser().GUID;
                dsMatrixType.MATRIX_TYPE.AddMATRIX_TYPERow(drMatrixType);
                _daMATRIX_TYPE.Save(drMatrixType);
                RefreshMatrixType();
            }
        }

        private void btnEditType_Click(object sender, EventArgs e)
        {
            if (gridViewType.SelectedRowsCount == 0)
            {
                Common.Warn("Please select equipment type to edit");
                return;
            }

            frmSchedule_Matrix_Add frmMatrixAdd = new frmSchedule_Matrix_Add((MatrixType)gridViewType.GetFocusedRow());
            if(frmMatrixAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MatrixType editType = frmMatrixAdd.GetMatrixType();
                dsMATRIX_TYPE.MATRIX_TYPERow drMatrixType = _daMATRIX_TYPE.GetBy(editType.GUID);
                if(drMatrixType != null)
                {
                    AssignMatrixDetail(drMatrixType, editType);
                    drMatrixType.UPDATED = DateTime.Now;
                    drMatrixType.UPDATEDBY = System_Environment.GetUser().GUID;
                    _daMATRIX_TYPE.Save(drMatrixType);
                    RefreshMatrixType();
                }
            }
        }

        private void btnDeleteType_Click(object sender, EventArgs e)
        {
            if (gridViewType.SelectedRowsCount == 0)
            {
                Common.Warn("Please select equipment type(s) to delete");
                return;
            }

            int[] selectedRowIndexes = gridViewType.GetSelectedRows();
            foreach (int selectedRowIndex in selectedRowIndexes)
            {
                MatrixType selectedType = (MatrixType)gridViewType.GetRow(selectedRowIndex);
                _daMATRIX_TYPE.RemoveBy(selectedType.GUID);
            }

            RefreshMatrixType();
        }

        private void AssignMatrixDetail(dsMATRIX_TYPE.MATRIX_TYPERow drMatrixType, MatrixType matrixType)
        {
            drMatrixType.NAME = matrixType.typeName;
            drMatrixType.DESCRIPTION = matrixType.typeDescription;
            drMatrixType.CATEGORY = matrixType.typeCategory;
            drMatrixType.DISCIPLINE = matrixType.typeDiscipline;
        }


        private void btnTrimDuplicates_Click(object sender, EventArgs e)
        {
            if (cmbProject.SelectedItem == null)
                return;

            Guid toProjectId = (Guid)((ValuePair)cmbProject.SelectedItem).Value;
            dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtToMATRIX_ASSIGNMENT = _daMATRIX_ASSIGNMENT.GetByProject(toProjectId);

            if (dtToMATRIX_ASSIGNMENT == null)
            {
                MessageBox.Show("Selected project doesn't have any assignments to trim");
                return;
            }

            List<KeyValuePair<Guid, Guid>> existingTemplates = new List<KeyValuePair<Guid, Guid>>();
            List<dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow> assignmentToDelete = new List<dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow>();
            int deleted_assignments = 0;
            foreach (dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow drToMATRIX_ASSIGNMENT in dtToMATRIX_ASSIGNMENT.Rows)
            {
                if(!existingTemplates.Any(x => x.Key == drToMATRIX_ASSIGNMENT.GUID_MATRIX_TYPE && x.Value == drToMATRIX_ASSIGNMENT.GUID_TEMPLATE))
                {
                    existingTemplates.Add(new KeyValuePair<Guid, Guid>(drToMATRIX_ASSIGNMENT.GUID_MATRIX_TYPE, drToMATRIX_ASSIGNMENT.GUID_TEMPLATE));
                }
                else
                {
                    drToMATRIX_ASSIGNMENT.DELETED = DateTime.Now;
                    drToMATRIX_ASSIGNMENT.DELETEDBY = System_Environment.GetUser().GUID;
                    _daMATRIX_ASSIGNMENT.Save(drToMATRIX_ASSIGNMENT);
                    deleted_assignments += 1;
                }
            }

            MessageBox.Show(deleted_assignments.ToString() + " duplicates removed");
        }

        private void btnCopyFrom_Click(object sender, EventArgs e)
        {
            if (cmbProject.SelectedItem == null)
                return;

            if (MessageBox.Show("This will clear current project matrix and replace it with selected project matrix, do you wish to continue?", "Confirm Copy", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                return;

            frmTool_Project frmSelectProject = new frmTool_Project();
            frmSelectProject.ShowAsDialog();
            if (frmSelectProject.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Guid toProjectId = (Guid)((ValuePair)cmbProject.SelectedItem).Value;
                Guid fromProjectId = frmSelectProject.GetSelectedProject().GUID;

                _daMATRIX_ASSIGNMENT.RemoveByProject((Guid)((ValuePair)cmbProject.SelectedItem).Value);
                dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtFromMATRIX_ASSIGNMENT = _daMATRIX_ASSIGNMENT.GetByProject(fromProjectId);
                dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTDataTable dtToMATRIX_ASSIGNMENT = _daMATRIX_ASSIGNMENT.GetByProject(toProjectId);
                
                if(dtFromMATRIX_ASSIGNMENT == null)
                {
                    MessageBox.Show("Selected project doesn't have any assignment to copy from");
                    return;
                }

                EnumerableRowCollection<dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow> enumToMATRIX_ASSIGNMENT = dtToMATRIX_ASSIGNMENT == null ? null : dtToMATRIX_ASSIGNMENT.AsEnumerable();
                dsMATRIX_ASSIGNMENT dsMATRIX_ASSIGN = new dsMATRIX_ASSIGNMENT();
                int saved_assignments = 0;
                int deleted_assignments = 0;
                foreach (dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow drFromMATRIX_ASSIGNMENT in dtFromMATRIX_ASSIGNMENT.Rows)
                {
                    EnumerableRowCollection<dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow> existingFromRows = enumToMATRIX_ASSIGNMENT == null ? null : enumToMATRIX_ASSIGNMENT.Where(x => x.GUID_TEMPLATE == drFromMATRIX_ASSIGNMENT.GUID_TEMPLATE && x.GUID_MATRIX_TYPE == drFromMATRIX_ASSIGNMENT.GUID_MATRIX_TYPE);
                    if(existingFromRows == null || existingFromRows.Count() == 0)
                    {
                        dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow drNEW_MATRIX_ASSIGNMENT = dsMATRIX_ASSIGN.MATRIX_ASSIGNMENT.NewMATRIX_ASSIGNMENTRow();
                        Guid NewRowGuid = Guid.NewGuid();
                        drNEW_MATRIX_ASSIGNMENT.GUID = NewRowGuid;
                        drNEW_MATRIX_ASSIGNMENT.GUID_MATRIX_TYPE = drFromMATRIX_ASSIGNMENT.GUID_MATRIX_TYPE;
                        drNEW_MATRIX_ASSIGNMENT.GUID_PROJECT = (Guid)((ValuePair)cmbProject.SelectedItem).Value;
                        drNEW_MATRIX_ASSIGNMENT.GUID_TEMPLATE = drFromMATRIX_ASSIGNMENT.GUID_TEMPLATE;
                        drNEW_MATRIX_ASSIGNMENT.CREATED = DateTime.Now;
                        drNEW_MATRIX_ASSIGNMENT.CREATEDBY = System_Environment.GetUser().GUID;
                        dsMATRIX_ASSIGN.MATRIX_ASSIGNMENT.AddMATRIX_ASSIGNMENTRow(drNEW_MATRIX_ASSIGNMENT);
                        _daMATRIX_ASSIGNMENT.Save(drNEW_MATRIX_ASSIGNMENT);
                        saved_assignments += 1;
                    }
                }

                //dtToMATRIX_ASSIGNMENT = _daMATRIX_ASSIGNMENT.GetByProject(toProjectId);
                //EnumerableRowCollection<dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow> enumFromMATRIX_ASSIGNMENT = dtFromMATRIX_ASSIGNMENT == null ? null : dtFromMATRIX_ASSIGNMENT.AsEnumerable();
                //List<dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow> drToRowsToDelete = new List<dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow>();

                //if (enumFromMATRIX_ASSIGNMENT != null)
                //{
                //    foreach (dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow drToMATRIX_ASSIGNMENT in dtToMATRIX_ASSIGNMENT.Rows)
                //    {
                //        EnumerableRowCollection<dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow> existingToRows = enumFromMATRIX_ASSIGNMENT.Where(x => x.GUID_TEMPLATE == drToMATRIX_ASSIGNMENT.GUID_TEMPLATE && x.GUID_MATRIX_TYPE == drToMATRIX_ASSIGNMENT.GUID_MATRIX_TYPE);
                //        if (existingToRows.Count() == 0)
                //        {
                //            drToMATRIX_ASSIGNMENT.DELETED = DateTime.Now;
                //            drToMATRIX_ASSIGNMENT.DELETEDBY = System_Environment.GetUser().GUID;
                //            _daMATRIX_ASSIGNMENT.Save(drToMATRIX_ASSIGNMENT);
                //            deleted_assignments += 1;
                //        }
                //    }
                //}


                MessageBox.Show("Copied " + saved_assignments.ToString() + " assignments and deleted " + deleted_assignments + " assignments");
            }
            else
            {
                this.Close();
                return;
            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("Loading...");
            ValuePair vpPROJECT = (ValuePair)cmbProject.SelectedItem;
            frmReportSheet frmCertificateSheet = new frmReportSheet((Guid)vpPROJECT.Value);
            frmCertificateSheet.populateMatrixReport();
            splashScreenManager1.CloseWaitForm();
            frmCertificateSheet.Show();
        }
        #endregion

        #region Events
        private void cmbProject_EditValueChanged(object sender, EventArgs e)
        {
            RefreshTemplate();
        }

        private void gridViewType_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            GridView view = (GridView)sender;
            GridHitInfo hitInfo = view.CalcHitInfo(e.Location);
            if (!hitInfo.InRow)
            {
                _TemplateChecks.Clear();
                gridViewTemplate.RefreshData();
            }
            else
            {
                RefreshTemplate();
            }
        }
        #endregion

        #region Initialisation
        private void RefreshMatrixType()
        {
            _MatrixType.Clear();

            dsMATRIX_TYPE.MATRIX_TYPEDataTable dtMatrixType = _daMATRIX_TYPE.Get();
            if(dtMatrixType != null)
            {
                foreach(dsMATRIX_TYPE.MATRIX_TYPERow drMatrixType in dtMatrixType.OrderBy(x => x.NAME))
                {
                    _MatrixType.Add(new MatrixType(drMatrixType.GUID)
                        {
                            typeName = drMatrixType.NAME,
                            typeDescription = drMatrixType.IsDESCRIPTIONNull() ? string.Empty : drMatrixType.DESCRIPTION,
                            typeDiscipline = drMatrixType.DISCIPLINE,
                            typeCategory = drMatrixType.CATEGORY
                        });
                }

                gridControlType.RefreshDataSource();
            }
        }

        private void RefreshTemplate()
        {
            _TemplateChecks.Clear();
            if(gridViewType.FocusedRowHandle > -1)
            {
                MatrixType SelectedType = (MatrixType)gridViewType.GetFocusedRow();
                if(SelectedType != null)
                {
                    dsTEMPLATE_MAIN.TEMPLATE_MAINDataTable dtTEMPLATE_MAIN = _daTEMPLATE_MAIN.Get();
                    foreach(dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTEMPLATE_MAIN in dtTEMPLATE_MAIN.Rows)
                    {
                        _TemplateChecks.Add(new Template_Check() { 
                                                                    GUID = drTEMPLATE_MAIN.GUID,
                                                                    templateAssignmentGuid = Guid.Empty,
                                                                    templateName = drTEMPLATE_MAIN.NAME,
                                                                    templateDescription = drTEMPLATE_MAIN.IsDESCRIPTIONNull() ? string.Empty : drTEMPLATE_MAIN.DESCRIPTION,
                                                                    templateDiscipline = drTEMPLATE_MAIN.DISCIPLINE,
                                                                    templateRevision = drTEMPLATE_MAIN.REVISION,
                                                                    templateWorkFlow = new ValuePair(string.Empty, Guid.Empty),
                                                                    templateSelected = false,
                                                                    templateQRSupport = drTEMPLATE_MAIN.QRSUPPORT
                                            });
                    }

                    Update_Template_CheckState(SelectedType.GUID);
                }
            }

            gridViewTemplate.RefreshData();
            //gridViewTemplate.MoveFirst();
        }

        private void Update_Template_CheckState(Guid TypeGuid)
        {
            foreach(Template_Check Template_Check in _TemplateChecks)
            {
                ValuePair vpPROJECT = (ValuePair)cmbProject.SelectedItem;
                dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow drMATRIX_ASSIGNMENT = _daMATRIX_ASSIGNMENT.GetBy_Type_And_Template((Guid)vpPROJECT.Value, TypeGuid, Template_Check.GUID);
                if(drMATRIX_ASSIGNMENT != null)
                {
                    Template_Check.templateAssignmentGuid = drMATRIX_ASSIGNMENT.GUID;
                    Template_Check.templateSelected = true;
                }
            }
        }
        #endregion

        private void repositoryItemCheckEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            Template_Check SelectedTemplate = (Template_Check)gridViewTemplate.GetFocusedRow();
            if(SelectedTemplate != null)
            {
                //Existing Row in MATRIX ASSIGNMENT table
                if(SelectedTemplate.templateAssignmentGuid != Guid.Empty)
                {
                    dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow drMATRIX_ASSIGNMENT = _daMATRIX_ASSIGNMENT.GetBy(SelectedTemplate.templateAssignmentGuid);
                    if(drMATRIX_ASSIGNMENT != null) //This shouldn't happen because templateAssignmentGUID indicates that it's already in the database
                    {
                        //if((bool)e.NewValue == false) //When entry already exists in database, click should register a false value
                        _daMATRIX_ASSIGNMENT.RemoveBy(SelectedTemplate.templateAssignmentGuid);
                        SelectedTemplate.templateAssignmentGuid = Guid.Empty;
                    }
                }
                //Row doesn't already exists in MATRIX ASSIGNMENT table
                else
                {
                    dsMATRIX_ASSIGNMENT dsMATRIX_ASSIGN = new dsMATRIX_ASSIGNMENT();
                    dsMATRIX_ASSIGNMENT.MATRIX_ASSIGNMENTRow drNEW_MATRIX_ASSIGNMENT = dsMATRIX_ASSIGN.MATRIX_ASSIGNMENT.NewMATRIX_ASSIGNMENTRow();
                    MatrixType SelectedType = (MatrixType)gridViewType.GetFocusedRow();
                    if(SelectedType != null)
                    {
                        Guid NewRowGuid = Guid.NewGuid();
                        SelectedTemplate.templateAssignmentGuid = NewRowGuid;
                        drNEW_MATRIX_ASSIGNMENT.GUID = NewRowGuid;
                        drNEW_MATRIX_ASSIGNMENT.GUID_MATRIX_TYPE = SelectedType.GUID;
                        drNEW_MATRIX_ASSIGNMENT.GUID_PROJECT = (Guid)((ValuePair)cmbProject.SelectedItem).Value;
                        drNEW_MATRIX_ASSIGNMENT.GUID_TEMPLATE = SelectedTemplate.GUID;
                        drNEW_MATRIX_ASSIGNMENT.CREATED = DateTime.Now;
                        drNEW_MATRIX_ASSIGNMENT.CREATEDBY = System_Environment.GetUser().GUID;
                        dsMATRIX_ASSIGN.MATRIX_ASSIGNMENT.AddMATRIX_ASSIGNMENTRow(drNEW_MATRIX_ASSIGNMENT);
                        _daMATRIX_ASSIGNMENT.Save(drNEW_MATRIX_ASSIGNMENT);
                    }
                }
            }
        }
    }
}
