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
    public partial class frmImageViewer : Form
    {
        public frmImageViewer()
        {
            InitializeComponent();
        }

        public frmImageViewer(List<Image> images, int currentImageIndex = -1)
        {
            InitializeComponent();
            foreach (Image image in images)
            {
                imageSlider1.Images.Add(image);
            }

            imageSlider1.CurrentImageIndex = currentImageIndex;
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
