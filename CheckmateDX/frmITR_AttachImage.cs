using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CheckmateDX
{
    public partial class frmITR_AttachImage : Form
    {
        public frmITR_AttachImage()
        {
            InitializeComponent();
        }

        private void simpleButtonBrowseImage_Click(object sender, EventArgs e)
        {
            browseImage();
        }

        private void browseImage()
        {
            OpenFileDialog thisDialog = new OpenFileDialog();

            thisDialog.Filter = "Images (*.BMP;*.JPG;*.JPEG;*.GIF)|*.BMP;*.JPG;*.JPEG;*.GIF";
            thisDialog.FilterIndex = 2;
            thisDialog.RestoreDirectory = true;
            thisDialog.Multiselect = true;
            thisDialog.Title = "Please Select Images";

            if (thisDialog.ShowDialog() == DialogResult.OK)
            {
                imageSlider1.Images.Clear();
                foreach (String file in thisDialog.FileNames.OrderBy(x => x))
                {
                    try
                    {
                        imageSlider1.Images.Add(Image.FromFile(file));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    }
                }
            }
        }

        public SliderImageCollection GetImages()
        {
            return imageSlider1.Images;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
