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
    public partial class frmInteractable_PictureEdit : CheckmateDX.frmParent
    {
        DrawingBoard_ConstantPen _dbPictureEdit = new DrawingBoard_ConstantPen();
        Bitmap _editedPicture;
        public frmInteractable_PictureEdit(byte[] imageByte)
        {
            InitializeComponent();

            _dbPictureEdit.Parent = pnlContent;
            _dbPictureEdit.Dock = DockStyle.Fill;
            _dbPictureEdit.Option = DrawingOption.Line;
            _dbPictureEdit.PenWidth = 2;
            _dbPictureEdit.BackgroundImage = Common.ConvertByteArrayToImage(imageByte);
            _editedPicture = new Bitmap(_dbPictureEdit.BackgroundImage);
            this.Height = _editedPicture.Height + 111; //111 is the size of all the top and bottom controls
            this.Width = _editedPicture.Width + 20;
        }

        #region Events
        private void bBtnClear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _dbPictureEdit.Clear();
        }

        private void bBtnAccept_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Bitmap tempBmp = _dbPictureEdit.ExportToBitmap();
            if (tempBmp != null)
                _editedPicture = tempBmp;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void bBtnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        public Bitmap GetEditedBitmap()
        {
            return _editedPicture;
        }
        #endregion
    }
}
