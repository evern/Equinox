using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DrawingBoard;
using ProjectLibrary;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using DevExpress.Utils;

namespace CheckmateDX
{
     public partial class frmTool_User_ChangeSignature : CheckmateDX.frmParent
    {
        DrawingBoard_ConstantPen _dbSignature = new DrawingBoard_ConstantPen();
        SuperToolTip _sToolTip = new SuperToolTip();
        public frmTool_User_ChangeSignature()
        {
            InitializeComponent();

            _dbSignature.Parent = pnlContent;
            _dbSignature.Dock = DockStyle.Fill;
            _dbSignature.Option = DrawingOption.Pen;
            _dbSignature.PenWidth = 2;

            LoadInitialToolTip();
            _dbSignature.MouseUp += _dbSignature_MouseUp;
        }

        private void LoadInitialToolTip()
        {
            using (AdapterUSER_MAIN daUser = new AdapterUSER_MAIN())
            {
                dsUSER_MAIN.USER_MAINRow drUser = daUser.GetBy(System_Environment.GetUser().GUID);
                if (drUser != null)
                {
                    if(!drUser.IsSIGNATURENull())
                    {
                        Bitmap signature = new Bitmap(Common.ConvertByteArrayToImage(drUser.SIGNATURE));
                        signature = Common.ResizeBitmap(signature, 400, 200);

                        ToolTipItem toolTipItem = new ToolTipItem { Image = signature, Text = "Actual Size in Form" };
                        _sToolTip.Items.Add(toolTipItem);
                    }
                }
            }

            bBtnAccept.SuperTip = _sToolTip;
        }

        #region Events
        private void _dbSignature_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                Bitmap signature = new Bitmap(_dbSignature.ExportToBitmap());
                signature = Common.ResizeBitmap(signature, 400, 200);

                ToolTipItem toolTipItem;
                if (_sToolTip.Items.Count > 0)
                {
                    toolTipItem = (ToolTipItem)_sToolTip.Items[0];
                    toolTipItem.Image = signature;
                }
                else
                {
                    toolTipItem = new ToolTipItem() { Image = signature, Text = "Actual Size in Form" };
                    _sToolTip.Items.Add(toolTipItem);
                }

                bBtnAccept.SuperTip = _sToolTip;
            }
            catch
            {

            }
        }

        private void bBtnClear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _dbSignature.Clear();
            _sToolTip.Items.Clear();
        }

        private void bBtnAccept_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (AdapterUSER_MAIN daUser = new AdapterUSER_MAIN())
            {
                dsUSER_MAIN.USER_MAINRow drUser = daUser.GetBy(System_Environment.GetUser().GUID);
                if (drUser != null)
                {
                    ToolTipItem toolTipItem = (ToolTipItem)_sToolTip.Items[0];

                    drUser.SIGNATURE = Common.ConvertImageToByteArray(toolTipItem.Image);
                    drUser.UPDATED = DateTime.Now;
                    drUser.UPDATEDBY = drUser.GUID;
                    daUser.Save(drUser);
                    //Common.Prompt("Signature successfully changed");
                }
                else
                    Common.Warn("Something went wrong, please contact an administrator");

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void bBtnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }


        private void bBtnBrowse_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                byte[] bytes = System.IO.File.ReadAllBytes(openFileDialog1.FileName);
                
                Bitmap OriginalBitmap = new Bitmap(Common.ConvertByteArrayToImage(bytes));
                Bitmap ResizedBitmap = Common.ResizeBitmap(OriginalBitmap, 200, 100);
                _dbSignature.BackgroundImage = ResizedBitmap;
                ToolTipItem toolTipItem;

                if (_sToolTip.Items.Count > 0)
                {
                    toolTipItem = (ToolTipItem)_sToolTip.Items[0];
                    toolTipItem.Image = ResizedBitmap;
                }
                else
                {
                    toolTipItem = new ToolTipItem() { Image = ResizedBitmap, Text = "Actual Size in Form" };
                    _sToolTip.Items.Add(toolTipItem);
                }

                bBtnAccept.SuperTip = _sToolTip;
            }
        }

        private void btnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ToolTipItem toolTipItem = (ToolTipItem)_sToolTip.Items[0];
            toolTipItem.Image.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Signature.bmp");
        }
        #endregion

    }
}
