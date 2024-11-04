using CheckmateDX;
using CheckmateDX.Report;
using DevExpress.Office.Internal;
using DevExpress.Office.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CheckmateDX
{
    public static class BarCodeUtility
    {
        public static Image GetBarCode(string text)
        {
            using (rptBarcode rpt = new rptBarcode())
            {
                rpt.SetText(text);
                rpt.CreateDocument();
                MemoryStream stream = new MemoryStream();
                ImageExportOptions opt = new ImageExportOptions()
                {
                    Format = ImageFormat.Bmp,
                    Resolution = 600
                };
                rpt.ExportToImage(stream, opt);
                Image img = Image.FromStream(stream);
                return img;
            }
        }

        public static Image GetQRCode(string text)
        {
            using (rptQRcode rpt = new rptQRcode())
            {
                rpt.SetText(text);
                rpt.CreateDocument();
                MemoryStream stream = new MemoryStream();
                ImageExportOptions opt = new ImageExportOptions()
                {
                    Format = ImageFormat.Bmp,
                    Resolution = 600
                };
                rpt.ExportToImage(stream, opt);
                Image img = Image.FromStream(stream);
                return img;
            }
        }
    }
}

public static class SignatureBlockUtility
    {
        public static Image GetSignatureBlock(List<SignatureUser> signature_users, bool isElectrical)
        {
            using (rptSignature rpt = new rptSignature())
            {
                rpt.PopulateSignatures(signature_users, isElectrical);
                rpt.CreateDocument();
                MemoryStream stream = new MemoryStream();
                ImageExportOptions opt = new ImageExportOptions()
                {
                    Format = ImageFormat.Bmp,
                    Resolution = 600
                };
                rpt.ExportToImage(stream, opt);
                Image img = Image.FromStream(stream);
                return img;
            }
        }

        public static Image GetSignatureBlock()
        {
            using (rptSignature rpt = new rptSignature())
            {
                rpt.CreateDocument();
                MemoryStream stream = new MemoryStream();
                ImageExportOptions opt = new ImageExportOptions()
                {
                    Format = ImageFormat.Bmp,
                    Resolution = 600
                };
                rpt.ExportToImage(stream, opt);
                Image img = Image.FromStream(stream);
                return img;
            }
        }
    }
