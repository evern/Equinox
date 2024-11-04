using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;

namespace CheckmateDX
{
    public partial class frmPrefill_Main : CheckmateDX.frmParent
    {
        public delegate void PrefillClose();
        public PrefillClose ClosePrefill;

        AdapterPREFILL_MAIN _daPrefill = new AdapterPREFILL_MAIN();
        dsPREFILL_MAIN _dsPrefill = new dsPREFILL_MAIN();
        List<Prefill> _allPrefill = new List<Prefill>();
        public frmPrefill_Main()
        {
            InitializeComponent();
            PopulateFormElements();
            prefillBindingSource.DataSource = _allPrefill;
            RefreshPrefills();
        }

        /// <summary>
        /// Populate form element
        /// </summary>
        private void PopulateFormElements()
        {
            repositoryItemComboBox1.BeginUpdate();
            repositoryItemComboBox1.Items.Add(Variables.prefillGeneral.ToString());
            repositoryItemComboBox1.Items.Add(Discipline.Electrical.ToString());
            repositoryItemComboBox1.Items.Add(Discipline.Instrumentation.ToString());
            repositoryItemComboBox1.Items.Add(Discipline.Mechanical.ToString());
            repositoryItemComboBox1.Items.Add(Discipline.Structural.ToString());
            repositoryItemComboBox1.Items.Add(Discipline.Civil.ToString());
            repositoryItemComboBox1.Items.Add(Discipline.Piping.ToString());
            repositoryItemComboBox1.EndUpdate();
            barEditDiscipline.EditValue = Variables.prefillGeneral.ToString();
        }

        #region Form Population
        /// <summary>
        /// Refresh all prefill from database
        /// </summary>
        private void RefreshPrefills()
        {
            _allPrefill.Clear();
            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPrefill = _daPrefill.GetByDiscipline(barEditDiscipline.EditValue.ToString());

            if(dtPrefill != null)
            {
                foreach (dsPREFILL_MAIN.PREFILL_MAINRow drPrefill in dtPrefill.Rows)
                {
                    _allPrefill.Add(new Prefill(drPrefill.GUID)
                    {
                        prefillName = drPrefill.NAME,
                        prefillCategory = drPrefill.CATEGORY,
                        CreatedDate = drPrefill.CREATED,
                        CreatedBy = drPrefill.CREATEDBY
                    });
                }
            }

            gridControl.RefreshDataSource();
        }
        #endregion

        #region Events
        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmPrefill_Add frmPrefillAdd = new frmPrefill_Add();
            if (frmPrefillAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Prefill newPrefill = frmPrefillAdd.GetPrefill();
                dsPREFILL_MAIN.PREFILL_MAINRow drPrefill = _dsPrefill.PREFILL_MAIN.NewPREFILL_MAINRow();
                drPrefill.GUID = Guid.NewGuid();
                AssignPrefillDetails(drPrefill, newPrefill);
                drPrefill.DISCIPLINE = barEditDiscipline.EditValue.ToString();
                drPrefill.CREATED = DateTime.Now;
                drPrefill.CREATEDBY = System_Environment.GetUser().GUID;
                _dsPrefill.PREFILL_MAIN.AddPREFILL_MAINRow(drPrefill);
                _daPrefill.Save(drPrefill);
                RefreshPrefills();
                //Common.Prompt("Prefill successfully added");
            }
        }

        private void btnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if(gridView1.SelectedRowsCount == 0)
            {
                Common.Warn("Please select Prefill to edit");
                return;
            }

            frmPrefill_Add frmPrefillAdd = new frmPrefill_Add((Prefill)gridView1.GetFocusedRow());
            if (frmPrefillAdd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Prefill editPrefill = frmPrefillAdd.GetPrefill();

                dsPREFILL_MAIN.PREFILL_MAINRow drPrefillMaster = _daPrefill.GetBy(editPrefill.GUID);
                AssignPrefillDetails(drPrefillMaster, editPrefill);
                drPrefillMaster.UPDATED = DateTime.Now;
                drPrefillMaster.UPDATEDBY = System_Environment.GetUser().GUID;
                _daPrefill.Save(drPrefillMaster);
                //Common.Prompt("Prefill successfully updated");
                RefreshPrefills();
            }
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gridView1.SelectedRowsCount == 0)
            {
                Common.Warn("Please select Prefill(s) to delete");
                return;
            }

            if (!Common.Confirmation("Are you sure you want to delete selected Prefill(s)?", "Confirmation"))
                return;

            dsPREFILL_MAIN.PREFILL_MAINDataTable dtPrefill = new dsPREFILL_MAIN.PREFILL_MAINDataTable(); //what prefill to delete
            int[] selectedRowIndexes = gridView1.GetSelectedRows();
            
            foreach (int selectedRowIndex in selectedRowIndexes)
            {
                Prefill selectedPrefill = (Prefill)gridView1.GetRow(selectedRowIndex);
                _daPrefill.RemoveBy(selectedPrefill.GUID);
            }

            RefreshPrefills();
        }

        private void barEditDiscipline_EditValueChanged(object sender, EventArgs e)
        {
            RefreshPrefills();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Assigns common prefill details to data row
        /// </summary>
        /// <param name="drUserMaster">datarow to be assigned</param>
        /// <param name="user">prefill details</param>
        private void AssignPrefillDetails(dsPREFILL_MAIN.PREFILL_MAINRow drPrefill, Prefill prefill)
        {
            drPrefill.NAME = prefill.prefillName;
            drPrefill.CATEGORY = prefill.prefillCategory;
        }

        /// <summary>
        /// Dispose local adapters upon closing
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            if (ClosePrefill != null)
                ClosePrefill();

            _daPrefill.Dispose();
            base.OnClosed(e);
        }
        #endregion
    }
}
