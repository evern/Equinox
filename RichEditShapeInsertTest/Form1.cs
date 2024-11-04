using DevExpress.Pdf;
using ProjectCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RichEditShapeInsertTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //this.richEditControl1.InsertFooterQRCode("test");

            //this.richEditControl1.ExportToPdf("test.pdf");
            using (PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor())
            {
                documentProcessor.LoadDocument("test.pdf");

                for (int i = 1; i <= documentProcessor.Document.Pages.Count; i++)
                {
                    //Bitmap pageBitmap = documentProcessor.CreateBitmap(i, 10000);
                    //double ratio = 0;
                    //Bitmap resizeImage = (Bitmap)Common.ScaleImage(pageBitmap, 5000, 5000, out ratio);
                    //Bitmap cellCropBitmap = Common.CropBitmap(resizeImage, 0, resizeImage.Height - 700, resizeImage.Width, 700);
                    Bitmap cellCropBitmap = documentProcessor.CreateBitmap(i, 10000);
                    Graphics g = Graphics.FromImage(cellCropBitmap);
                    SolidBrush brush = new SolidBrush(Color.White);
                    g.FillRectangle(brush, new Rectangle(((int)(cellCropBitmap.Width / 2) - 400), cellCropBitmap.Height - 800, 800, 800));

                    cellCropBitmap.Save("MetaQRCode.bmp");
                }
            }
        }
    }
}
