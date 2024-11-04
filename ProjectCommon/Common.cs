using System;
using System.Data;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using ProjectLibrary;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
//using DevExpress.XtraRichEdit;
using System.Diagnostics;
using System.Xml.Linq;
using System.Management;
using System.Windows.Forms;
using ZXing;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit;

namespace ProjectCommon
{
    public class SyncProcessedEventArg : EventArgs
    {
        public Guid SyncProjectGuid { get; set; }
    }

    public static class Common
    {
        #region Windows Prompt

        /// <summary>
        /// A method to recurse wbs tag to retrieve all relevant childrens
        /// </summary>
        public static IEnumerable<wbsTagDisplay> GetChildWBSTagDisplays(List<wbsTagDisplay> allWBSTagDisplays, wbsTagDisplay selectedWBSTagDisplay)
        {
            yield return selectedWBSTagDisplay;
            foreach (wbsTagDisplay childWbsTagDisplay in allWBSTagDisplays.Where(x => x.wbsTagDisplayParentGuid == selectedWBSTagDisplay.wbsTagDisplayGuid))
                foreach (wbsTagDisplay childChildWbsTagDisplay in GetChildWBSTagDisplays(allWBSTagDisplays, childWbsTagDisplay))
                    yield return childChildWbsTagDisplay;
        }

        /// <summary>
        /// Add unique WBS Tag to collection
        /// </summary>
        public static void AddUniqueWBSTag(List<wbsTagDisplay> wbsTagDisplays, wbsTagDisplay addWbsTagDisplay)
        {
            if (!wbsTagDisplays.Any(x => x.wbsTagDisplayGuid == addWbsTagDisplay.wbsTagDisplayGuid))
                wbsTagDisplays.Add(addWbsTagDisplay);
        }

        /// <summary>
        /// Displays a warning message
        /// </summary>
        public static void Warn(string warnMessage)
        {
            System.Windows.MessageBox.Show(warnMessage, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Displays an information message
        /// </summary>
        public static void Prompt(string promptMessage)
        {
            System.Windows.MessageBox.Show(promptMessage, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Displays a confirmation message
        /// </summary>
        public static bool Confirmation(string confirmationMessage, string title)
        {
            if (System.Windows.MessageBox.Show(confirmationMessage, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Displays a confirmation message
        /// </summary>
        public static bool WarningConfirmation(string confirmationMessage, string title)
        {
            if (System.Windows.MessageBox.Show(confirmationMessage, title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }
        #endregion

        #region QRCode
        public static string SQLDateFormatString()
        {
            return "yyyy-MM-dd HH:mm:ss.fff";
        }

        static decimal? lastScaling = null;
        public static string GetDocumentMetaQRCodeTryHarder(Bitmap bitmap)
        {
            //since result will be affected by scaling, try different scaling to get results
            string result = string.Empty;
            if (lastScaling != null)
                result = GetDocumentMetaQRCode(bitmap, (decimal)lastScaling);

            if (result != string.Empty)
                return result;
            
            decimal localScaling = 1;
            do
            {
                result = GetDocumentMetaQRCode(bitmap, localScaling);
                if(result != string.Empty)
                {
                    lastScaling = localScaling;
                    return result;
                }

                localScaling = localScaling - 0.1m;
            } while (localScaling > 0);


            return result;
        }

        public static Bitmap GetPageHeaderBitmap(Bitmap fullPageBitmap, decimal scalePercentage)
        {
            double ratio;
            int scaleSize = (int)Math.Round(5000 * scalePercentage, 0);
            int cropSize = (int)Math.Round(700 * scalePercentage, 0);
            Bitmap resizeImage = (Bitmap)Common.ScaleImage(fullPageBitmap, scaleSize, scaleSize, out ratio);
            Bitmap cellCropBitmap = CropBitmap(resizeImage, 0, 0, resizeImage.Width, cropSize);
            resizeImage.Dispose();
            return cellCropBitmap;
        }

        public static string GetDocumentMetaQRCode(Bitmap bitmap, decimal scalePercentage)
        {
            Bitmap cellCropBitmap = GetPageHeaderBitmap(bitmap, scalePercentage);
            //cellCropBitmap.Save("MetaQRCode.bmp");
            //Bitmap reloadBitMap = (Bitmap)Bitmap.FromFile("MetaQRCode.bmp");
            BarcodeReader reader = new BarcodeReader();
            reader.AutoRotate = true;
            reader.Options.TryHarder = true;

            Result result = reader.Decode(cellCropBitmap);
            if (result != null)
            {
                cellCropBitmap.Dispose();
                return result.ToString();
            }

            cellCropBitmap.Dispose();
            return string.Empty;
        }
        #endregion

        #region Image Processing
        public static Bitmap CropBitmap(Bitmap bitmap, int cropX, int cropY, int cropWidth, int cropHeight)
        {
            Rectangle rect = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap cropped = bitmap.Clone(rect, bitmap.PixelFormat);
            return cropped;
        }
        #endregion

        #region Database
        /// <summary>
        /// Read the connection string from XML and feed it into a static library
        /// </summary>
        public static bool SetConnStrFromXML()
        {
            string xmlFilePath = DatabaseXMLFilePath(false);

            if (File.Exists(xmlFilePath))
            {
                try
                {
                    XDocument doc = XDocument.Load(xmlFilePath);
                    XElement findDatabase = doc.Root.Descendants().FirstOrDefault(obj => obj.Name == "Local");
                    if (findDatabase != null)
                    {
                        string server = findDatabase.Attribute("Url").Value;
                        string database = Decrypt(findDatabase.Attribute("Database").Value, true);
                        string username = Decrypt(findDatabase.Attribute("Username").Value, true);
                        string password = Decrypt(findDatabase.Attribute("Password").Value, true);

                        string connStr = ConstructConnString(server, database, username, password);
                        Variables.SetConnStr(connStr);
                        return true;
                    }
                }
                catch
                {

                }
            }

            return false;
        }

        /// <summary>
        /// Construct the database connection string
        /// </summary>
        public static string ConstructConnString(string server, string database, string username, string password)
        {
            string connStr;
            if (server.ToUpper() == "CHECKMATEDEV.DATABASE.WINDOWS.NET")
                connStr = @"Server=checkmatedev.database.windows.net; Authentication=Active Directory Password; Encrypt=True; Database=checkmate-dev; User Id=su.bing-wen@primero.com.au; Password=sbPW2021++";
            else
            {
                connStr = @"Data Source=" + server;
                connStr += ";Initial Catalog=" + database;
                connStr += ";Persist Security Info=False;User ID=" + username;
                connStr += ";Password=" + password;
            }

            return connStr;
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight, out double ratio)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public static Image ResizeImage(Image image, int width, int height)
        {
            var newImage = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, width, height);

            return newImage;
        }

        /// <summary>
        /// Retrieve ITR from database from Tag or WBS
        /// </summary>
        /// <returns></returns>
        public static dsITR_MAIN.ITR_MAINRow GetITR(Tag tag, WBS wbs, Guid templateGuid)
        {
            using (AdapterITR_MAIN daITR = new AdapterITR_MAIN())
            {
                dsITR_MAIN.ITR_MAINRow drITR;
                if (tag != null)
                    drITR = daITR.GetByTagWBSTemplate(tag.GUID, templateGuid);
                else if (wbs != null)
                    drITR = daITR.GetByTagWBSTemplate(wbs.GUID, templateGuid);
                else
                {
                    Common.Warn("Error retrieving document for ITR or WBS");
                    return null;
                }

                return drITR;
            }
        }


        /// <summary>
        /// Retrieve ITR from database from Tag or WBS
        /// </summary>
        /// <returns></returns>
        public static dsITR_MAIN.ITR_MAINRow GetITRByGuid(Guid iTRGuid)
        {
            using (AdapterITR_MAIN daITR = new AdapterITR_MAIN())
            {
                dsITR_MAIN.ITR_MAINRow drITR = daITR.GetBy(iTRGuid);

                return drITR;
            }
        }


        /// <summary>
        /// Encapsulation for AdapterITR_MAIN.GetByTagWBSTemplate
        /// </summary>
        /// <param name="WbsTagGuids"></param>
        /// <param name="templateGuid"></param>
        /// <returns></returns>
        public static dsITR_MAIN.ITR_MAINDataTable GetITRBy(List<ITR_Refresh_Item> ParallelItems)
        {
            using (AdapterITR_MAIN daITR_MAIN = new AdapterITR_MAIN())
            {
                dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = daITR_MAIN.GetByTagWBSTemplate(ParallelItems);
                return dtITR_MAIN;
            }
        }

        /// <summary>
        /// Encapsulation for AdapterITR_MAIN.GetByTagWBSTemplate
        /// </summary>
        /// <returns></returns>
        public static dsITR_MAIN.ITR_MAINDataTable RevisedGetITRBy(List<ITR_Refresh_Item> ParallelItems)
        {
            if (ParallelItems.Count == 0)
                return null;

            using (AdapterITR_MAIN daITR_MAIN = new AdapterITR_MAIN())
            {
                dsITR_MAIN.ITR_MAINDataTable dtITR_MAIN = daITR_MAIN.GetTableBy(ParallelItems[0].ITR_GUID);
                return dtITR_MAIN;
            }
        }

        public static List<wbsTagDisplay> GetWBSTagBy(List<ITR_Refresh_Item> ParallelItems)
        {
            AdapterTAG daTAG = new AdapterTAG();
            AdapterWBS daWBS = new AdapterWBS();
            List<wbsTagDisplay> ParallelItemsWBSTag = new List<wbsTagDisplay>();

            try
            {
                foreach (ITR_Refresh_Item ParallelItem in ParallelItems)
                {
                    dsTAG.TAGRow drTAG = daTAG.GetBy(ParallelItem.WBSTagGuid);
                    if (drTAG != null)
                    {
                        ParallelItemsWBSTag.Add(new wbsTagDisplay(new Tag(drTAG.GUID)
                        {
                            tagNumber = drTAG.NUMBER,
                            tagDescription = drTAG.DESCRIPTION,
                            tagParentGuid = drTAG.PARENTGUID,
                            tagScheduleGuid = drTAG.SCHEDULEGUID
                        }));
                    }
                    else
                    {
                        dsWBS.WBSRow drWBS = daWBS.GetBy(ParallelItem.WBSTagGuid);
                        if (drWBS != null)
                        {
                            ParallelItemsWBSTag.Add(new wbsTagDisplay(new WBS(drWBS.GUID)
                            {
                                wbsName = drWBS.NAME,
                                wbsDescription = drWBS.DESCRIPTION,
                                wbsParentGuid = drWBS.PARENTGUID,
                                wbsScheduleGuid = drWBS.SCHEDULEGUID
                            }));
                        }
                    }
                }
            }
            finally
            {
                daTAG.Dispose();
                daWBS.Dispose();
            }

            return ParallelItemsWBSTag;
        }

        /// <summary>
        /// Gets the ITR sequence from Tag or WBS
        /// </summary>
        /// <returns></returns>
        public static int GetITRSequence(Tag tag, WBS wbs, Guid templateGuid)
        {
            using (AdapterITR_MAIN daITR = new AdapterITR_MAIN())
            {
                int sequence = -1;
                if (tag != null)
                    sequence = daITR.GetSequence(tag.GUID, templateGuid);
                else if (wbs != null)
                    sequence = daITR.GetSequence(wbs.GUID, templateGuid);

                return sequence;
            }
        }
        #endregion

        #region Security
        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="toEncrypt">string to encrypt</param>
        /// <param name="useHashing">use hashing</param>
        /// <returns>encrypted string</returns>
        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = Variables.securityKey;

            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// Decrypts into a string
        /// </summary>
        /// <param name="cipherString">encrypted string</param>
        /// <param name="useHashing">use hashing</param>
        /// <returns>decrypted string</returns>
        public static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            string key = Variables.securityKey;

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// Gets the Processor ID for this computer
        /// </summary>
        /// <returns>Processor ID</returns>
        public static string GetHWID()
        {
            string drive = "C";
            ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            dsk.Get();
            string volumeSerial = dsk["VolumeSerialNumber"].ToString();

            return volumeSerial;
        }

        /// <summary>
        /// Get external IP address
        /// </summary>
        /// <returns>External IP address</returns>
        public static string GetExternalIP()
        {
            HTTPGet req = new HTTPGet();
            req.Request("http://checkip.dyndns.org");

            if (req.ResponseBody.Trim() == "No Server Response")
                return "0.0.0.0";

            string[] a = req.ResponseBody.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];

            return a4;
        }
        #endregion

        #region Formatting
        public static string FormatFileName(string originalFileName)
        {
            originalFileName = originalFileName.Replace("/", "-");
            originalFileName = originalFileName.Replace("\\", "");
            originalFileName = originalFileName.Replace("|", "");

            return originalFileName;
        }

        /// <summary>
        /// Removes "<<" and ">>" from string
        /// </summary>
        /// <param name="headerString"></param>
        public static string FormatFieldString(string headerString)
        {
            headerString = headerString.Replace("<<", "");
            headerString = headerString.Replace(">>", "");

            return headerString;
        }

        /// <summary>
        /// Replaces underscore with spaces
        /// </summary>
        public static string Replace_WithSpaces(string Enum)
        {
            string Result = Enum.Replace("_", " ");

            return Result;
        }

        /// <summary>
        /// Removes spaces from string
        /// </summary>
        public static string RemoveSpaces(string str)
        {
            string Result = str.Replace(" ", "");
            return Result;
        }

        /// <summary>
        /// Replaces space with underscore
        /// </summary>
        public static string ReplaceSpacesWith_(string str)
        {
            string Result = str.Replace(" ", "_");
            return Result;
        }
        #endregion  

        #region Populate Objects
        /// <summary>
        /// Refreshes the Client Sync Options Depending on GridViewClient Selections
        /// </summary>
        public static void Populate_Database_Options(List<SyncOption> SyncOptions, bool ExcludeProject = false)
        {
            int SyncItemID = 100;
            //Add the Categories
            if (!ExcludeProject)
                SyncOptions.Add(Create_SyncOptions_Category(1, Common.Replace_WithSpaces(Sync_Category.General.ToString())));

            SyncOptions.Add(Create_SyncOptions_Category(2, Common.Replace_WithSpaces(Sync_Category.Design.ToString())));
            SyncOptions.Add(Create_SyncOptions_Category(3, Common.Replace_WithSpaces(Sync_Category.Plan.ToString())));
            SyncOptions.Add(Create_SyncOptions_Category(4, Common.Replace_WithSpaces(Sync_Category.Authorisation.ToString())));
            SyncOptions.Add(Create_SyncOptions_Category(5, Common.Replace_WithSpaces(Sync_Category.Content.ToString())));

            //Add the Items
            if (!ExcludeProject)
            {
                SyncOptions.Add(Create_SyncOptions_Item(1, Common.Replace_WithSpaces(Sync_Item.Project.ToString()), ref SyncItemID, SyncScope.Global));
                SyncOptions.Add(Create_SyncOptions_Item(1, Common.Replace_WithSpaces(Sync_Item.Equipment.ToString()), ref SyncItemID, SyncScope.Project));
            }
            else
                SyncOptions.Add(Create_SyncOptions_Category(1, Common.Replace_WithSpaces(Sync_Item.Equipment.ToString())));

            SyncOptions.Add(Create_SyncOptions_Item(2, Common.Replace_WithSpaces(Sync_Item.Template.ToString()), ref SyncItemID, SyncScope.Global));
            SyncOptions.Add(Create_SyncOptions_Item(2, Common.Replace_WithSpaces(Sync_Item.Template_Components.ToString()), ref SyncItemID, SyncScope.Discipline));
            SyncOptions.Add(Create_SyncOptions_Item(3, Common.Replace_WithSpaces(Sync_Item.Schedule.ToString()), ref SyncItemID, SyncScope.Project));
            SyncOptions.Add(Create_SyncOptions_Item(3, Common.Replace_WithSpaces(Sync_Item.Header_Data.ToString()), ref SyncItemID, SyncScope.Project));
            SyncOptions.Add(Create_SyncOptions_Item(4, Common.Replace_WithSpaces(Sync_Item.Role.ToString()), ref SyncItemID, SyncScope.Global));
            SyncOptions.Add(Create_SyncOptions_Item(4, Common.Replace_WithSpaces(Sync_Item.User.ToString()), ref SyncItemID, SyncScope.Global));
            SyncOptions.Add(Create_SyncOptions_Item(5, Common.Replace_WithSpaces(Sync_Item.ITR.ToString()), ref SyncItemID, SyncScope.Project));
            SyncOptions.Add(Create_SyncOptions_Item(5, Common.Replace_WithSpaces(Sync_Item.Punchlist.ToString()), ref SyncItemID, SyncScope.Project));
            SyncOptions.Add(Create_SyncOptions_Item(5, Common.Replace_WithSpaces(Sync_Item.Certificate.ToString()), ref SyncItemID, SyncScope.Project));
        }

        /// <summary>
        /// Create Category for Sync Options
        /// </summary>
        private static SyncOption Create_SyncOptions_Category(int ID, string CategoryName)
        {
            return new SyncOption(ID) { OptionName = CategoryName, OptionEnabled = false, OptionOneTime = false, OptionParentID = 0};
        }

        /// <summary>
        /// Create the SyncItem with Parent Category Binding
        /// </summary>
        private static SyncOption Create_SyncOptions_Item(int ParentID, string ItemName, ref int SyncOptionID, SyncScope syncOptionType)
        {
            SyncOption SyncItem = new SyncOption(SyncOptionID) { OptionName = ItemName, OptionEnabled = false, OptionOneTime = false, OptionParentID = ParentID, OptionScope = syncOptionType.ToString() };
            SyncOptionID += 1;
            return SyncItem;
        }

        /// <summary>
        /// Set the display icon for the certificate
        /// </summary>
        public static void SetCertificateImageIndex(ICertificate certificateViewModel, AdapterCERTIFICATE_STATUS daCERTIFICATE_STATUS, AdapterCERTIFICATE_STATUS_ISSUE daCERTIFICATE_STATUS_ISSUE)
        {
            //modify image index to show rejection
            List<dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow> CertificateStatusFilteredRows = new List<dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow>();
            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCertificateStatusFiltered = daCERTIFICATE_STATUS.GetStatusByCertificate(certificateViewModel.GUID, 2);
            if (dtCertificateStatusFiltered != null)
                CertificateStatusFilteredRows = dtCertificateStatusFiltered.AsEnumerable().ToList();

            if (CertificateStatusFilteredRows.Count > 0)
            {
                dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drCurrentStatus = CertificateStatusFilteredRows[0];
                if (CertificateStatusFilteredRows.Count >= 2)
                {
                    dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drPreviousStatus = CertificateStatusFilteredRows[1];
                    dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUERow drCertificateStatusIssue;
                    drCertificateStatusIssue = daCERTIFICATE_STATUS_ISSUE.GetLatestBy(drCurrentStatus.GUID);

                    if ((int)drCurrentStatus.STATUS_NUMBER < (int)drPreviousStatus.STATUS_NUMBER)
                    {
                        if (drCertificateStatusIssue == null || drCertificateStatusIssue.COMMENTS == string.Empty)
                            certificateViewModel.ImageIndex = 5; //rejected without comments
                        else
                            certificateViewModel.ImageIndex = 7; //rejected with comments
                    }
                    else
                    {
                        if (drCertificateStatusIssue == null || drCertificateStatusIssue.COMMENTS == string.Empty)
                            certificateViewModel.ImageIndex = 4; //progressed without comments
                        else
                            certificateViewModel.ImageIndex = 6; //progressed with comments
                    }
                }
                else if (CertificateStatusFilteredRows.Count >= 1)
                {
                    //it is possible now for newly inspected ITR to have comment because it's been resurfaced by parallel mechanism
                    dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUERow drCertificateIssue = daCERTIFICATE_STATUS_ISSUE.GetLatestBy(drCurrentStatus.GUID);

                    if (drCertificateIssue == null || drCertificateIssue.COMMENTS == string.Empty)
                        certificateViewModel.ImageIndex = 4; //progressed without comments
                    else
                        certificateViewModel.ImageIndex = 6; //progressed with comments
                }
                else
                {
                    if (drCurrentStatus.STATUS_NUMBER == (int)CVC_Status.Supervisor)
                        certificateViewModel.ImageIndex = 3;
                }
            }
        }

        /// <summary>
        /// Retrive all certificate status comments
        /// </summary>
        public static void GetCertificateStatusComments<T>(T certificate, List<CertificateComments> allComments, AdapterCERTIFICATE_STATUS daCERTIFICATE_STATUS, AdapterCERTIFICATE_STATUS_ISSUE daCERTIFICATE_STATUS_ISSUE, AdapterUSER_MAIN daUser, out Guid commentingStatusGuid)
             where T : ICertificate
        {
            allComments.Clear();
            dsCERTIFICATE_STATUS.CERTIFICATE_STATUSDataTable dtCertificateStatus = daCERTIFICATE_STATUS.GetAll(certificate.GUID);

            if (dtCertificateStatus != null)
                commentingStatusGuid = dtCertificateStatus[0].GUID;
            else
                commentingStatusGuid = Guid.Empty;

            if (dtCertificateStatus != null)
            {
                foreach (dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drCertificateStatus in dtCertificateStatus.Rows)
                {
                    int previousStatus = 99;
                    dsCERTIFICATE_STATUS.CERTIFICATE_STATUSRow drPreviousCertificateStatus = dtCertificateStatus.FirstOrDefault(obj => obj.SEQUENCE_NUMBER == drCertificateStatus.SEQUENCE_NUMBER - 1);

                    int imageIndex = 0;
                    if (drPreviousCertificateStatus != null)
                        previousStatus = (int)drPreviousCertificateStatus.STATUS_NUMBER;

                    if (previousStatus == 99)
                        imageIndex = 0; //new status
                    else if (drCertificateStatus.STATUS_NUMBER < previousStatus)
                        imageIndex = 1; //down status
                    else
                        imageIndex = 2; //up status

                    string creatorName = "Unknown";
                    dsUSER_MAIN.USER_MAINRow drUser = daUser.GetBy(drCertificateStatus.CREATEDBY);
                    if (drUser != null)
                    {
                        creatorName = drUser.LASTNAME + " " + drUser.FIRSTNAME;
                    }



                    string previousStatusStr = "Saved"; //if it's -1 this will be shown
                    if (drCertificateStatus.STATUS_NUMBER >= 0)
                        previousStatusStr = certificate.GetStatusDescription((int)drCertificateStatus.STATUS_NUMBER);

                    //adds the ITR status
                    allComments.Add(new CertificateComments(drCertificateStatus.GUID)
                    {
                        CertificateCommParentGuid = Guid.Empty,
                        CertificateCommInfo= previousStatusStr,
                        CertificateCommImageIndex = imageIndex,
                        CertificateCommCreator = creatorName,
                        CreatedDate = drCertificateStatus.CREATED,
                        CreatedBy = drCertificateStatus.CREATEDBY
                    });

                    //adds the ITR comments
                    dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUEDataTable dtCertificateStatusIssue = daCERTIFICATE_STATUS_ISSUE.GetBy(drCertificateStatus.GUID);
                    if (dtCertificateStatusIssue != null)
                    {
                        foreach (dsCERTIFICATE_STATUS_ISSUE.CERTIFICATE_STATUS_ISSUERow drStatusIssue in dtCertificateStatusIssue.Rows)
                        {
                            creatorName = "Unknown";
                            drUser = daUser.GetBy(drStatusIssue.CREATEDBY);
                            if (drUser != null)
                            {
                                creatorName = drUser.LASTNAME + " " + drUser.FIRSTNAME;
                            }
                            allComments.Add(new CertificateComments(drStatusIssue.GUID)
                            {
                                CertificateCommParentGuid = drStatusIssue.CERTIFICATE_STATUS_GUID,
                                CertificateCommInfo = drStatusIssue.COMMENTS,
                                CertificateCommCreator = creatorName,
                                CreatedBy = drStatusIssue.CREATEDBY,
                                CreatedDate = drStatusIssue.CREATED,
                                CertificateCommImageIndex = drStatusIssue.REJECTION ? 4 : 3
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Retrive all ITR status comments
        /// </summary>
        /// <param name="iTRGuid">Guid of ITR</param>
        /// <param name="iTRStatusGuid">Returns Status to Assign Comments to</param>
        public static void GetITRStatusComments(Guid iTRGuid, List<iTRComments> allComments, out Guid iTRStatusGuid)
        {
            AdapterITR_STATUS daITRStatus = new AdapterITR_STATUS();
            AdapterITR_STATUS_ISSUE daITRIssue = new AdapterITR_STATUS_ISSUE();
            AdapterUSER_MAIN daUser = new AdapterUSER_MAIN();

            try
            {
                allComments.Clear();
                dsITR_STATUS.ITR_STATUSDataTable dtITRStatus = daITRStatus.GetAll(iTRGuid);

                if (dtITRStatus != null)
                    iTRStatusGuid = dtITRStatus[0].GUID;
                else
                    iTRStatusGuid = Guid.Empty;

                if (dtITRStatus != null)
                {
                    foreach (dsITR_STATUS.ITR_STATUSRow drITRStatus in dtITRStatus.Rows)
                    {
                        int previousStatus = 99;
                        dsITR_STATUS.ITR_STATUSRow drITRPreviousStatus = dtITRStatus.FirstOrDefault(obj => obj.SEQUENCE_NUMBER == drITRStatus.SEQUENCE_NUMBER - 1);

                        int imageIndex = 0;
                        if (drITRPreviousStatus != null)
                            previousStatus = (int)drITRPreviousStatus.STATUS_NUMBER;

                        if (previousStatus == 99)
                            imageIndex = 0; //new status
                        else if (drITRStatus.STATUS_NUMBER < previousStatus)
                            imageIndex = 1; //down status
                        else
                            imageIndex = 2; //up status

                        string creatorName = "Unknown";
                        dsUSER_MAIN.USER_MAINRow drUser = daUser.GetBy(drITRStatus.CREATEDBY);
                        if (drUser != null)
                        {
                            creatorName = drUser.LASTNAME + " " + drUser.FIRSTNAME;
                        }

                        string previousStatusStr = "Saved"; //if it's -1 this will be shown
                        if (drITRStatus.STATUS_NUMBER >= 0)
                            previousStatusStr = ((ITR_Status)drITRStatus.STATUS_NUMBER).ToString();

                        //adds the ITR status
                        allComments.Add(new iTRComments(drITRStatus.GUID)
                        {
                            iTRCommParentGuid = Guid.Empty,
                            iTRCommInfo = previousStatusStr,
                            iTRCommImageIndex = imageIndex,
                            iTRCommCreator = creatorName,
                            CreatedDate = drITRStatus.CREATED,
                            CreatedBy = drITRStatus.CREATEDBY
                        });

                        //adds the ITR comments
                        dsITR_STATUS_ISSUE.ITR_STATUS_ISSUEDataTable dtStatusIssue = daITRIssue.GetBy(drITRStatus.GUID);
                        if (dtStatusIssue != null)
                        {
                            foreach (dsITR_STATUS_ISSUE.ITR_STATUS_ISSUERow drStatusIssue in dtStatusIssue.Rows)
                            {
                                creatorName = "Unknown";
                                drUser = daUser.GetBy(drStatusIssue.CREATEDBY);
                                if (drUser != null)
                                {
                                    creatorName = drUser.LASTNAME + " " + drUser.FIRSTNAME;
                                }
                                allComments.Add(new iTRComments(drStatusIssue.GUID)
                                {
                                    iTRCommParentGuid = drStatusIssue.ITR_STATUS_GUID,
                                    iTRCommInfo = drStatusIssue.COMMENTS,
                                    iTRCommCreator = creatorName,
                                    CreatedBy = drStatusIssue.CREATEDBY,
                                    CreatedDate = drStatusIssue.CREATED,
                                    iTRCommImageIndex = drStatusIssue.REJECTION ? 4 : 3
                                });
                            }
                        }
                    }
                }
            }
            finally
            {
                daITRIssue.Dispose();
                daITRStatus.Dispose();
                daUser.Dispose();
            }
        }

        /// <summary>
        /// Retrive all Punchlist status comments
        /// </summary>
        /// <param name="punchlistGuid">Guid of Punchlist</param>
        /// <param name="punchlistStatusGuid">Returns Status to Assign Comments to</param>
        public static void GetPunchlistStatusComments(Guid punchlistGuid, List<punchlistComments> allComments, out Guid punchlistStatusGuid)
        {
            AdapterPUNCHLIST_MAIN daPunchlist = new AdapterPUNCHLIST_MAIN();
            AdapterPUNCHLIST_STATUS daPunchlistStatus = new AdapterPUNCHLIST_STATUS();
            AdapterPUNCHLIST_STATUS_ISSUE daPunchlistStatusIssue = new AdapterPUNCHLIST_STATUS_ISSUE();
            AdapterUSER_MAIN daUser = new AdapterUSER_MAIN();

            Guid newGuid = new Guid("DD4AF7ED-820D-4744-86C7-398142BA466A"); //for creating new status if no punchlist status exists

            dsPUNCHLIST_MAIN.PUNCHLIST_MAINRow drPunchlist = daPunchlist.GetBy(punchlistGuid);
            if (drPunchlist == null) //shouldn't happen but just in case
            {
                punchlistStatusGuid = Guid.Empty;
                return;
            }

            try
            {
                int previousStatus = 99;
                string creatorName = "Unknown";
                string previousStatusStr = "New"; //if it's -1 this will be shown

                allComments.Clear();
                dsPUNCHLIST_STATUS.PUNCHLIST_STATUSDataTable dtPunchlistStatus = daPunchlistStatus.GetAll(punchlistGuid);

                if (dtPunchlistStatus != null)
                    punchlistStatusGuid = dtPunchlistStatus[0].GUID;
                else
                    punchlistStatusGuid = Guid.Empty;

                if (dtPunchlistStatus != null)
                {
                    foreach (dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow drPunchlistStatus in dtPunchlistStatus.Rows)
                    {
                        dsPUNCHLIST_STATUS.PUNCHLIST_STATUSRow drPunchlistPreviousStatus = dtPunchlistStatus.FirstOrDefault(obj => obj.SEQUENCE_NUMBER == (drPunchlistStatus.SEQUENCE_NUMBER - 1));

                        int imageIndex = 0;
                        if (drPunchlistPreviousStatus != null)
                            previousStatus = (int)drPunchlistPreviousStatus.STATUS_NUMBER;
                        else //add punchlist created status
                        {
                            allComments.Add(new punchlistComments(newGuid)
                            {
                                punchlistCommParentGuid = Guid.Empty,
                                punchlistCommInfo = previousStatusStr,
                                punchlistCommImageIndex = imageIndex,
                                punchlistCommCreator = creatorName,
                                CreatedDate = drPunchlistStatus.CREATED,
                                CreatedBy = drPunchlistStatus.CREATEDBY
                            });
                        }

                        if (previousStatus == 99)
                            imageIndex = 2; //supposedly new status but since new is always present the next logical status should be an increment to categorised
                        else if (drPunchlistStatus.STATUS_NUMBER < previousStatus)
                            imageIndex = 1; //down status
                        else
                            imageIndex = 2; //up status


                        dsUSER_MAIN.USER_MAINRow drUser = daUser.GetBy(drPunchlistStatus.CREATEDBY);
                        if (drUser != null)
                        {
                            creatorName = drUser.LASTNAME + " " + drUser.FIRSTNAME;
                        }

                        if (drPunchlistStatus.STATUS_NUMBER >= 0)
                            previousStatusStr = ((Punchlist_Status)drPunchlistStatus.STATUS_NUMBER).ToString();

                        //adds the punchlist status
                        allComments.Add(new punchlistComments(drPunchlistStatus.GUID)
                        {
                            punchlistCommParentGuid = Guid.Empty,
                            punchlistCommInfo = previousStatusStr,
                            punchlistCommImageIndex = imageIndex,
                            punchlistCommCreator = creatorName,
                            CreatedDate = drPunchlistStatus.CREATED,
                            CreatedBy = drPunchlistStatus.CREATEDBY
                        });

                        dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUEDataTable dtStatusIssue = daPunchlistStatusIssue.GetBy(drPunchlistStatus.GUID);
                        //adds the punchlist comments
                        if (dtStatusIssue != null)
                        {
                            foreach (dsPUNCHLIST_STATUS_ISSUE.PUNCHLIST_STATUS_ISSUERow drStatusIssue in dtStatusIssue.Rows)
                            {
                                creatorName = "Unknown";
                                drUser = daUser.GetBy(drStatusIssue.CREATEDBY);
                                if (drUser != null)
                                {
                                    creatorName = drUser.LASTNAME + " " + drUser.FIRSTNAME;
                                }
                                allComments.Add(new punchlistComments(drStatusIssue.GUID)
                                {
                                    punchlistCommParentGuid = drStatusIssue.PUNCHLIST_STATUS_GUID,
                                    punchlistCommInfo = drStatusIssue.COMMENTS,
                                    punchlistCommCreator = creatorName,
                                    CreatedBy = drStatusIssue.CREATEDBY,
                                    CreatedDate = drStatusIssue.CREATED,
                                    punchlistCommImageIndex = drStatusIssue.REJECTION ? 4 : 3
                                });
                            }
                        }
                    }
                }
                else
                {
                    allComments.Add(new punchlistComments(newGuid)
                    {
                        punchlistCommParentGuid = Guid.Empty,
                        punchlistCommInfo = previousStatusStr,
                        punchlistCommImageIndex = 0,
                        punchlistCommCreator = creatorName,
                        CreatedDate = drPunchlist.CREATED,
                        CreatedBy = drPunchlist.CREATEDBY
                    });
                }
            }
            finally
            {
                daPunchlistStatus.Dispose();
                daPunchlistStatusIssue.Dispose();
            }
        }

        /// <summary>
        /// Search for ValuePair in combobox using label
        /// </summary>
        public static int FindValuePairInCmbEdit(ComboBoxEdit cmbEdit, ValuePair vp, bool byVal)
        {
            for (int i = 0; i < cmbEdit.Properties.Items.Count; i++)
            {
                ValuePair searchVP = (ValuePair)cmbEdit.Properties.Items[i];
                if (!byVal && searchVP.Label == vp.Label)
                    return i;
                else if (byVal && searchVP.Value.ToString() == vp.Value.ToString())
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Populates combobox with available projects in the database
        /// </summary>
        /// <param name="cmbProject"></param>
        public static void PopulateCmbProject(ComboBoxEdit cmbProject, bool allowNoProjects)
        {
            using (AdapterPROJECT daProject = new AdapterPROJECT())
            {
                dsPROJECT.PROJECTDataTable dtProject = daProject.Get();
                ComboBoxItemCollection coll = cmbProject.Properties.Items;
                coll.Clear();
                coll.BeginUpdate();
                var vpNoProject = new ValuePair(Variables.noProject, Guid.Empty);

                if (dtProject == null)
                {
                    coll.Add(vpNoProject);
                }
                else
                {
                    if (allowNoProjects)
                        coll.Add(vpNoProject);

                    foreach (dsPROJECT.PROJECTRow drProject in dtProject.Rows)
                    {
                        var vpProject = new ValuePair(drProject.NUMBER, drProject.GUID);
                        coll.Add(vpProject);
                    }
                }

                coll.EndUpdate();
                cmbProject.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Populates combobox with user authorised project and selects the default project
        /// </summary>
        public static void PopulateCmbAuthProject(ComboBoxEdit cmbProject, bool allowNoProjects)
        {
            ComboBoxItemCollection coll = cmbProject.Properties.Items;
            coll.Clear();
            coll.BeginUpdate();

            if (allowNoProjects)
            {
                ValuePair vpNoProject = new ValuePair(Variables.noProject, Guid.Empty);
                coll.Add(vpNoProject);
                cmbProject.SelectedIndex = 0;
            }


            //bool _isRestrictUserToAuthorisedProjectsOnly = System_Environment.HasPrivilege(PrivilegeTypeID.RestrictRoleToAuthorisedProjectsOnly);
            //Admin superuser gets all project
            //if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty || !_isRestrictUserToAuthorisedProjectsOnly)
            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                PopulateCmbProject(cmbProject, allowNoProjects);
                return;
            }

            AdapterUSER_MAIN daUser = new AdapterUSER_MAIN();
            AdapterPROJECT daProject = new AdapterPROJECT();

            try
            {
                dsUSER_MAIN.USER_PROJECTDataTable dtUserProject = daUser.GetAuthProjects();
                if (dtUserProject != null)
                {
                    foreach (dsUSER_MAIN.USER_PROJECTRow drUserProject in dtUserProject.Rows)
                    {
                        ValuePair vpProject = new ValuePair(Common.ConvertProjectGuidToName(drUserProject.PROJECTGUID), drUserProject.PROJECTGUID);
                        coll.Add(vpProject);
                    }

                    ValuePair vpUserProject = System_Environment.GetUser().userProject;
                    if (vpUserProject.Label != Variables.noProject)
                    {
                        cmbProject.SelectedIndex = FindValuePairInCmbEdit(cmbProject, vpUserProject, false);
                    }
                    else
                        cmbProject.SelectedIndex = 0;
                }
            }
            finally
            {
                daUser.Dispose();
                daProject.Dispose();
            }

            coll.EndUpdate();
        }

        /// <summary>
        /// Populates combobox with available discipline in enumerable type
        /// </summary>
        public static void PopulateCmbDiscipline(ComboBoxEdit cmbDiscipline, string selectedDiscipline)
        {
            List<string> disciplines = Enum.GetNames(typeof(Discipline)).ToList();
            ComboBoxItemCollection coll = cmbDiscipline.Properties.Items;
            coll.BeginUpdate();
            foreach (string discipline in disciplines)
            {
                coll.Add(Common.ConvertDBDisciplineForDisplay(discipline));
            }
            coll.EndUpdate();

            //select index in combobox
            if (selectedDiscipline != string.Empty)
            {
                cmbDiscipline.SelectedItem = selectedDiscipline;
            }
            else

                cmbDiscipline.SelectedIndex = 0;
        }

        /// <summary>
        /// Populates combobox with available search mode in enumerable type
        /// </summary>
        public static void PopulateCmbSearchMode(ComboBoxEdit cmbSearchMode)
        {
            List<string> searchModes = Enum.GetNames(typeof(ProjectLibrary.SearchMode)).ToList();
            ComboBoxItemCollection coll = cmbSearchMode.Properties.Items;
            coll.BeginUpdate();
            foreach (string searchMode in searchModes)
            {
                coll.Add(Common.Replace_WithSpaces(searchMode));
            }
            coll.EndUpdate();

            //select index in combobox
            cmbSearchMode.SelectedIndex = 1;
        }

        /// <summary>
        /// Populates combobox with available search mode in enumerable type
        /// </summary>
        public static void PopulateCmbCVCRoleEnum(ComboBoxEdit cmbCVCStatus)
        {
            List<string> searchModes = Enum.GetNames(typeof(CVC_Status)).ToList();
            ComboBoxItemCollection coll = cmbCVCStatus.Properties.Items;
            coll.BeginUpdate();
            foreach (string searchMode in searchModes)
            {
                coll.Add(Common.Replace_WithSpaces(searchMode));
            }
            coll.EndUpdate();

            //select index in combobox
            cmbCVCStatus.SelectedIndex = 1;
        }

        public class SearchCriteria
        {
            public string Number { get; set; }
            public string Description { get; set; }

            public override string ToString()
            {
                if (Number == null)
                    return string.Empty;

                return string.Concat(Number, " - ", Description);
            }
        }

        /// <summary>
        /// Populates combobox with available search mode in enumerable type
        /// </summary>
        public static void PopulateCmbArea(ComboBoxEdit cmbArea, List<ProjectWBS> projectWBSs)
        {
            ComboBoxItemCollection coll = cmbArea.Properties.Items;
            coll.Clear();
            coll.BeginUpdate();

            Dictionary<string, string> uniqueDictionary = new Dictionary<string, string>();
            foreach(ProjectWBS projectWBS in projectWBSs.OrderBy(x => x.Area))
            {
                if (!uniqueDictionary.Any(x => x.Key == projectWBS.Area))
                    uniqueDictionary.Add(projectWBS.Area, projectWBS.AreaDescription);
            }

            coll.Add(new SearchCriteria());
            foreach (KeyValuePair<string, string> uniqueValuePair in uniqueDictionary)
            {
                SearchCriteria searchCriteria = new SearchCriteria() { Number = uniqueValuePair.Key, Description = uniqueValuePair.Value };
                coll.Add(searchCriteria);
            }
            coll.EndUpdate();

            //select index in combobox
            cmbArea.SelectedIndex = 0;
        }


        /// <summary>
        /// Populates combobox with available search mode in enumerable type
        /// </summary>
        public static void PopulateCmbSystem(ComboBoxEdit cmbSystem, List<ProjectWBS> projectWBSs, string areaName)
        {
            ComboBoxItemCollection coll = cmbSystem.Properties.Items;
            coll.Clear();
            coll.BeginUpdate();

            List<ProjectWBS> filteredProjectWBSs;
            if (areaName == null || areaName == string.Empty)
                filteredProjectWBSs = projectWBSs.OrderBy(x => x.System).ToList();
            else
                filteredProjectWBSs = projectWBSs.Where(x => x.Area == areaName).OrderBy(x => x.System).ToList();

            Dictionary<string, string> uniqueDictionary = new Dictionary<string, string>();
            foreach (ProjectWBS filteredProjectWBS in filteredProjectWBSs.OrderBy(x => x.System))
            {
                if (!uniqueDictionary.Any(x => x.Key == filteredProjectWBS.System))
                    uniqueDictionary.Add(filteredProjectWBS.System, filteredProjectWBS.SystemDescription);
            }

            coll.Add(new SearchCriteria());
            foreach (KeyValuePair<string, string> uniqueValuePair in uniqueDictionary)
            {
                SearchCriteria searchCriteria = new SearchCriteria() { Number = uniqueValuePair.Key, Description = uniqueValuePair.Value };
                coll.Add(searchCriteria);
            }
            coll.EndUpdate();

            //select index in combobox
            cmbSystem.SelectedIndex = 0;
        }

        /// <summary>
        /// Populates combobox with available search mode in enumerable type
        /// </summary>
        public static void PopulateCmbSubsystem(ComboBoxEdit cmbSubsystem, List<ProjectWBS> projectWBSs, string systemName)
        {
            ComboBoxItemCollection coll = cmbSubsystem.Properties.Items;
            coll.Clear();
            coll.BeginUpdate();

            List<ProjectWBS> filteredProjectWBSs;
            if (systemName == null || systemName == string.Empty)
                filteredProjectWBSs = projectWBSs.OrderBy(x => x.Subsystem).ToList();
            else
                filteredProjectWBSs = projectWBSs.Where(x => x.System == systemName).OrderBy(x => x.Subsystem).ToList();

            Dictionary<string, string> uniqueDictionary = new Dictionary<string, string>();
            foreach (ProjectWBS filteredProjectWBS in filteredProjectWBSs.OrderBy(x => x.Subsystem))
            {
                if (!uniqueDictionary.Any(x => x.Key == filteredProjectWBS.Subsystem))
                    uniqueDictionary.Add(filteredProjectWBS.Subsystem, filteredProjectWBS.SubsystemDescription);
            }

            coll.Add(new SearchCriteria());
            foreach (KeyValuePair<string, string> uniqueValuePair in uniqueDictionary)
            {
                SearchCriteria searchCriteria = new SearchCriteria() { Number = uniqueValuePair.Key, Description = uniqueValuePair.Value };
                coll.Add(searchCriteria);
            }
            coll.EndUpdate();

            //select index in combobox
            cmbSubsystem.SelectedIndex = 0;
        }

        /// <summary>
        /// Populates combobox with user authorised discipline and selects the default discipline
        /// </summary>
        public static void PopulateCmbAuthDiscipline(ComboBoxEdit cmbDiscipline, bool includeAllDiscipline = false, string selectedDiscipline = "")
        {
            ComboBoxItemCollection coll = cmbDiscipline.Properties.Items;
            coll.Clear();
            coll.BeginUpdate();
            //Admin superuser gets all discipline
            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                PopulateCmbDiscipline(cmbDiscipline, string.Empty);
                return;
            }

            if (includeAllDiscipline)
                coll.Add(Variables.allDiscipline);

            using (AdapterUSER_MAIN daUser = new AdapterUSER_MAIN())
            {
                dsUSER_MAIN.USER_DISCDataTable dtUserDisc = daUser.GetAuthDisciplines();

                if (dtUserDisc != null)
                {
                    HashSet<string> uniqueDisciplines = new HashSet<string>();
                    foreach (dsUSER_MAIN.USER_DISCRow drUserDisc in dtUserDisc.Rows)
                    {
                        uniqueDisciplines.Add(Common.ConvertDBDisciplineForDisplay(drUserDisc.DISCIPLINE));
                    }

                    foreach(string uniqueDiscipline in uniqueDisciplines.OrderBy(x => x))
                    {
                        coll.Add(uniqueDiscipline);
                    }

                    string defaultDisciplineName;
                    if (selectedDiscipline != string.Empty)
                    {
                        defaultDisciplineName = selectedDiscipline;
                        if(coll.IndexOf(defaultDisciplineName) == -1)
                        {
                            coll.Add(defaultDisciplineName);
                        }
                    }
                    else
                        defaultDisciplineName = Common.ConvertDBDisciplineForDisplay(System_Environment.GetUser().userDiscipline);

                    if (defaultDisciplineName != Variables.noDiscipline)
                        cmbDiscipline.SelectedIndex = coll.IndexOf(defaultDisciplineName);
                    else
                        cmbDiscipline.SelectedIndex = 0;
                }
                else
                {
                    coll.Add(Variables.noDiscipline);
                    cmbDiscipline.SelectedIndex = 0;
                }
            }

            coll.EndUpdate();
        }

        public static List<string> GetAuthDiscipline(bool includeAllDiscipline = false, string selectedDiscipline = "")
        {
            List<string> disciplines = new List<string>();
            //Admin superuser gets all discipline
            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                disciplines = Enum.GetNames(typeof(Discipline)).ToList();
                return disciplines;
            }

            using (AdapterUSER_MAIN daUser = new AdapterUSER_MAIN())
            {
                dsUSER_MAIN.USER_DISCDataTable dtUserDisc = daUser.GetAuthDisciplines();
                if (dtUserDisc != null)
                {
                    foreach (dsUSER_MAIN.USER_DISCRow drUserDisc in dtUserDisc.Rows)
                    {
                        disciplines.Add(Common.ConvertDBDisciplineForDisplay(drUserDisc.DISCIPLINE));
                    }
                }
                else
                {
                    disciplines.Add(Variables.noDiscipline);
                }
            }

            List<string> uniqueDiscipline = new List<string>();
            foreach(string discipline in disciplines.OrderBy(x => x))
            {
                if (!uniqueDiscipline.Any(x => x.ToUpper() == discipline.ToUpper()))
                    uniqueDiscipline.Add(discipline);
            }

            if (includeAllDiscipline)
                uniqueDiscipline.Insert(0, Variables.allDiscipline);

            return uniqueDiscipline;
        }

        /// <summary>
        /// Populates combobox with all available role in database
        /// </summary>
        public static void PopulateCmbRole(ComboBoxEdit cmbRole)
        {
            using (AdapterROLE_MAIN daRole = new AdapterROLE_MAIN())
            {
                ComboBoxItemCollection coll = cmbRole.Properties.Items;
                coll.BeginUpdate();

                //only admin superuser can add admin superuser
                if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
                {
                    coll.Add(new ValuePair(Variables.AdminSuperuser, Guid.Empty));
                    cmbRole.SelectedIndex = 0;
                }

                dsROLE_MAIN.ROLE_MAINDataTable dtRole = daRole.Get();
                if (dtRole != null)
                {
                    foreach (dsROLE_MAIN.ROLE_MAINRow drRole in dtRole.Rows)
                    {
                        coll.Add(new ValuePair(Common.ConvertRoleGuidToName(drRole.GUID), drRole.GUID));
                    }
                }

                coll.EndUpdate();
            }
        }

        /// <summary>
        /// Populates combobox with user authorised role
        /// </summary>
        /// <param name="cmbRole">Combobox to Populate</param>
        /// <param name="ExcludeRoleGuid">Role to exclude, used when selection of self as parent is disallowed</param>
        /// <param name="ParentRoleGuid">Role to select, used when the parent is known</param>
        /// <param name="IncludeLoginUserRole">Include user login role, used for child's parent assignment</param>
        public static void PopulateCmbAuthRole(ComboBoxEdit cmbRole, Guid? ExcludeRoleGuid = null, Guid? ParentRoleGuid = null, bool IncludeLoginUserRole = false)
        {
            //Admin superuser gets all role
            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                PopulateCmbRole(cmbRole);
            }
            else
            {
                ComboBoxItemCollection coll = cmbRole.Properties.Items;
                coll.BeginUpdate();

                if (IncludeLoginUserRole)
                {
                    coll.Add(System_Environment.GetUser().userRole);
                }

                using (AdapterROLE_MAIN daRole = new AdapterROLE_MAIN())
                {
                    dsROLE_MAIN.ROLE_MAINDataTable dtRole = daRole.GetAuthRole();
                    if (dtRole != null)
                    {
                        foreach (dsROLE_MAIN.ROLE_MAINRow drRole in dtRole.Rows)
                        {
                            //cannot assign self as parent
                            if (ExcludeRoleGuid == null || drRole.GUID != ExcludeRoleGuid)
                                coll.Add(new ValuePair(Common.ConvertRoleGuidToName(drRole.GUID), drRole.GUID));
                        }
                    }
                }
                coll.EndUpdate();
            }

            //select parent in combobox
            if (ParentRoleGuid != null && ParentRoleGuid != Guid.Empty)
            {
                ValuePair vpUserRole = new ValuePair("", ParentRoleGuid);
                int i = FindValuePairInCmbEdit(cmbRole, vpUserRole, true);
                if (i != -1)
                    cmbRole.SelectedIndex = i;
                else
                    cmbRole.SelectedIndex = 0;
            }
            else
                cmbRole.SelectedIndex = 0;
        }

        /// <summary>
        /// Populates combobox with all available workflow in database
        /// </summary>
        /// <param name="cmbRole">Combobox to Populate</param>
        /// <param name="ExcludeRoleGuid">Workflow to exclude, used when selection of self as parent is disallowed</param>
        /// <param name="ParentRoleGuid">Workflow to select, used when the parent is known</param>
        /// <param name="ForceIncludeRoot">Force include root, even if workflow exists</param>
        public static void PopulateCmbWorkflow(ComboBoxEdit cmbWorkflow, Guid? ExcludeWorkflowGuid = null, Guid? ParentWorkflowGuid = null, bool ForceIncludeRoot = false, bool includeAllWorkflowItem = false)
        {
            ComboBoxItemCollection coll = cmbWorkflow.Properties.Items;
            coll.BeginUpdate();

            using (AdapterWORKFLOW_MAIN daWorkflow = new AdapterWORKFLOW_MAIN())
            {
                if (includeAllWorkflowItem)
                    coll.Add(new ValuePair(Variables.allWorkflow, Guid.Empty));

                //Root can only be assigned once because we can have only one finishing point in the workflow
                //Root can also be assigned when parent is root because that'll be only possibility during editing
                if (ForceIncludeRoot || !daWorkflow.IsRootExists() || (ParentWorkflowGuid != null && ParentWorkflowGuid == Guid.Empty))
                    coll.Add(new ValuePair(Variables.RootWorkflow, Guid.Empty));

                dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWorkflow = daWorkflow.Get();
                if (dtWorkflow != null)
                {
                    //Establish exclusion
                    List<Guid> excludedGuid = new List<Guid>();
                    //cannot assign self and child as parent
                    if (ExcludeWorkflowGuid != null)
                    {
                        excludedGuid.Add((Guid)ExcludeWorkflowGuid);
                        dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtChildWorkflow = daWorkflow.GetWorkflowChildrens((Guid)ExcludeWorkflowGuid);
                        if (dtChildWorkflow != null)
                        {
                            foreach (dsWORKFLOW_MAIN.WORKFLOW_MAINRow drChildWorkflow in dtChildWorkflow.Rows)
                            {
                                excludedGuid.Add(drChildWorkflow.GUID);
                            }
                        }
                    }

                    //Add entries not within exclusions
                    foreach (dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow in dtWorkflow.Rows)
                    {
                        if (excludedGuid.IndexOf(drWorkflow.GUID) == -1)
                            coll.Add(new ValuePair(Common.ConvertWorkflowGuidToName(drWorkflow.GUID), drWorkflow.GUID));
                    }
                }
            }
            coll.EndUpdate();

            //select parent in combobox
            if (ParentWorkflowGuid != null)
            {
                ValuePair vpUserWorkflow = new ValuePair("", ParentWorkflowGuid);
                int i = FindValuePairInCmbEdit(cmbWorkflow, vpUserWorkflow, true);
                if (i != -1)
                    cmbWorkflow.SelectedIndex = i;
                else
                    cmbWorkflow.SelectedIndex = 0;
            }
            else
                cmbWorkflow.SelectedIndex = 0;
        }

        /// <summary>
        /// Get all available workflow in database
        /// </summary>
        public static List<string> GetWorkflows()
        {
            List<string> workflows = new List<string>();
            using (AdapterWORKFLOW_MAIN daWorkflow = new AdapterWORKFLOW_MAIN())
            {
                dsWORKFLOW_MAIN.WORKFLOW_MAINDataTable dtWorkflow = daWorkflow.Get();
                if (dtWorkflow != null)
                {
                    //Add entries not within exclusions
                    foreach (dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow in dtWorkflow.Rows)
                    {
                        workflows.Add(Common.ConvertWorkflowGuidToName(drWorkflow.GUID));
                    }
                }
            }

            return workflows;
        }

        /// <summary>
        /// Populates combobox with punchlist category from enumerable type
        /// </summary>
        public static void PopulateCmbCategory(ComboBoxEdit cmbCategory, string selectedCategory)
        {
            List<string> Categories = Enum.GetNames(typeof(Punchlist_Category)).ToList();
            ComboBoxItemCollection coll = cmbCategory.Properties.Items;
            coll.BeginUpdate();
            coll.Add(Variables.SelectOne);
            foreach (string category in Categories)
            {
                coll.Add(Common.Replace_WithSpaces(category));
            }
            coll.EndUpdate();

            //select index in combobox
            if (selectedCategory != string.Empty)
            {
                cmbCategory.SelectedItem = selectedCategory;
            }
            else
                cmbCategory.SelectedIndex = 0;
        }

        /// <summary>
        /// Populates combobox with punchlist action by from enumerable type
        /// </summary>
        public static void PopulateCmbActionBy(ComboBoxEdit cmbActionBy, string selectedActionBy)
        {
            List<string> ActionBys = Enum.GetNames(typeof(Punchlist_ActionBy)).ToList();
            ComboBoxItemCollection coll = cmbActionBy.Properties.Items;
            coll.BeginUpdate();
            coll.Add(Variables.SelectOne);
            foreach (string actionBy in ActionBys)
            {
                coll.Add(Common.Replace_WithSpaces(actionBy));
            }
            coll.EndUpdate();

            //select index in combobox
            if (selectedActionBy != string.Empty)
            {
                cmbActionBy.SelectedItem = selectedActionBy;
            }
            else
                cmbActionBy.SelectedIndex = 0;
        }

        /// <summary>
        /// Populates combobox with punchlist priority from enumerable type
        /// </summary>
        public static void PopulateCmbPriority(ComboBoxEdit cmbPriority, string selectedPriority, bool includeAllCategory = false)
        {
            //List<string> Priorities = Enum.GetNames(typeof(Punchlist_Priority)).ToList();
            List<string> Priorities = new List<string>();
            foreach(string priority in GetPunchlistPriorities())
            {
                Priorities.Add(priority);
            }

            ComboBoxItemCollection coll = cmbPriority.Properties.Items;
            coll.BeginUpdate();
            if (includeAllCategory)
                coll.Add(Variables.allCategories);
            else
                coll.Add(Variables.SelectOne);

            foreach (string priority in Priorities)
            {
                coll.Add(Common.ConvertDBDisciplineForDisplay(priority));
            }
            coll.EndUpdate();

            //select index in combobox
            if (selectedPriority != string.Empty)
            {
                cmbPriority.SelectedItem = selectedPriority;
            }
            else
                cmbPriority.SelectedIndex = 0;
        }

        public static List<string> GetPunchlistPriorities()
        {
            List<string> priorities = new List<string>();
            priorities.Add(Variables.punchlistCategoryA);
            priorities.Add(Variables.punchlistCategoryB);
            priorities.Add(Variables.punchlistCategoryC);
            priorities.Add(Variables.punchlistCategoryD);
            return priorities;
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Convert Matrix Type Guid to Name
        /// </summary>
        public static string ConvertMatrixTypeGuidtoName(Guid TypeGuid)
        {
            using (AdapterMATRIX_TYPE daMATRIX_TYPE = new AdapterMATRIX_TYPE())
            {
                dsMATRIX_TYPE.MATRIX_TYPERow drMATRIX_TYPE = daMATRIX_TYPE.GetBy(TypeGuid);
                if (drMATRIX_TYPE != null)
                    return drMATRIX_TYPE.NAME;
                else
                    return "Not Found!";
            }
        }

        /// <summary>
        /// Gets the category specified for typeID
        /// </summary>
        /// <param name="privTypeID"></param>
        /// <returns></returns>
        public static string GetPrivilegeCategory(string privTypeID)
        {
            Privilege findPriv = System_Privilege.GetList().FirstOrDefault(obj => obj.privTypeID == privTypeID);
            if (findPriv != null)
                return findPriv.privCategory;
            else
                return "";
        }

        /// <summary>
        /// Converts project guid to project name
        /// </summary>
        public static string ConvertProjectGuidToName(Guid projectGuid)
        {
            if (projectGuid == Guid.Empty)
                return Variables.noProject;

            using (AdapterPROJECT daProject = new AdapterPROJECT())
            {
                dsPROJECT.PROJECTRow drProject = daProject.GetBy(projectGuid);

                if (drProject != null)
                    return $"{drProject.NUMBER} - {drProject.NAME}";
                else
                    return "Not Found!";
            }
        }

        /// <summary>
        /// Converts Schedule guid to Schedule name
        /// </summary>
        public static string ConvertScheduleGuidToName(Guid ScheduleGuid)
        {
            if (ScheduleGuid == Guid.Empty)
                return string.Empty;

            using (AdapterSCHEDULE daSCHEDULE = new AdapterSCHEDULE())
            {
                dsSCHEDULE.SCHEDULERow drSchedule = daSCHEDULE.GetBy(ScheduleGuid);

                if (drSchedule != null)
                    return drSchedule.NAME;
                else
                    return "Not Found!";
            }
        }

        /// <summary>
        /// Converts Tag guid to Tag Number
        /// </summary>
        public static string ConvertTagGuidToName(Guid TagGuid)
        {
            if (TagGuid == Guid.Empty)
                return Variables.RootTag;

            using (AdapterTAG daTAG = new AdapterTAG())
            {
                dsTAG.TAGRow drTAG = daTAG.GetBy(TagGuid);

                if (drTAG != null)
                    return drTAG.NUMBER;
                else
                    return "Not Found!";
            }
        }

        /// <summary>
        /// Converts WBS guid to Wbs Name
        /// </summary>
        public static string ConvertWbsGuidToName(Guid WbsGuid)
        {
            if (WbsGuid == Guid.Empty)
                return Variables.RootWBS;

            using (AdapterWBS daWBS = new AdapterWBS())
            {
                dsWBS.WBSRow drWBS = daWBS.GetBy(WbsGuid);

                if (drWBS != null)
                    return drWBS.NAME;
                else
                    return "Not Found!";
            }
        }

        /// <summary>
        /// Converts the ITR Guid to template name
        /// </summary>
        public static string ConvertITRGuidToTemplateName(Guid iTRGuid)
        {
            if (iTRGuid == Guid.Empty)
                return Variables.punchlistAdhoc;

            using (AdapterITR_MAIN daITR = new AdapterITR_MAIN())
            {
                dsITR_MAIN.ITR_MAINRow drITR = daITR.GetIncludeDeletedBy(iTRGuid);
                if (drITR != null)
                {
                    if (!drITR.IsDELETEDNull())
                        return drITR.NAME + " Conflict";
                    else
                        return drITR.NAME;
                }
                else
                    return Variables.punchlistAdhoc;
            }
        }

        /// <summary>
        /// Converts project guid to project name with superuser exception
        /// </summary>
        public static string ConvertProjectGuidToName(Guid projectGuid, Guid userRoleGuid)
        {
            if (userRoleGuid == Guid.Empty && projectGuid == Guid.Empty)
                return Variables.allProject;

            return ConvertProjectGuidToName(projectGuid);
        }

        /// <summary>
        /// Converts role guid to role name with guid.empty = superuser
        /// </summary>
        public static string ConvertRoleGuidToName(Guid roleGuid)
        {
            if (roleGuid == Guid.Empty)
                return Variables.AdminSuperuser;

            using (AdapterROLE_MAIN daRole = new AdapterROLE_MAIN())
            {
                dsROLE_MAIN.ROLE_MAINRow drRole = daRole.GetBy(roleGuid);

                if (drRole != null)
                    return drRole.NAME;
                else
                    return "Not Found!";
            }
        }
        public static string ConvertUserGuidToName(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return Variables.Superadmin_User;

            using (AdapterUSER_MAIN daUser = new AdapterUSER_MAIN())
            {
                dsUSER_MAIN.USER_MAINRow drUser = daUser.GetBy(userGuid);
                if (drUser != null)
                    return drUser.LASTNAME + " " + drUser.FIRSTNAME;
                else
                    return Variables.Unknown_User;
            }
        }

        /// <summary>
        /// Converts workflow guid to workflow name
        /// </summary>
        public static string ConvertWorkflowGuidToName(Guid roleGuid)
        {
            using (AdapterWORKFLOW_MAIN daWorkflow = new AdapterWORKFLOW_MAIN())
            {
                dsWORKFLOW_MAIN.WORKFLOW_MAINRow drWorkflow = daWorkflow.GetBy(roleGuid, true);

                if (drWorkflow != null)
                    return drWorkflow.NAME;
                else
                    return "Not Found!";
            }
        }

        /// <summary>
        /// Converts template guid to template name
        /// </summary>
        public static string ConvertTemplateGuidToName(Guid templateGuid)
        {
            if (templateGuid == Guid.Empty)
                return Variables.TemplateHeader;

            using (AdapterTEMPLATE_MAIN daTemplate = new AdapterTEMPLATE_MAIN())
            {
                dsTEMPLATE_MAIN.TEMPLATE_MAINRow drTemplate = daTemplate.GetBy(templateGuid);

                if (drTemplate != null)
                    return drTemplate.NAME;
                else
                    return "Not Found!";
            }
        }

        /// <summary>
        /// Converts project guid to project name with superuser exception
        /// </summary>
        public static string ConvertDBDisciplineForDisplay(string dbDiscipline, Guid userRoleGuid)
        {
            if (userRoleGuid == Guid.Empty && dbDiscipline == string.Empty)
                return Variables.allDiscipline;

            return ConvertDBDisciplineForDisplay(dbDiscipline);
        }

        /// <summary>
        /// User discipline datagrid display format
        /// </summary>
        public static string ConvertDBDisciplineForDisplay(string dbDiscipline)
        {
            if (dbDiscipline == string.Empty)
                return Variables.noDiscipline;
            else
                return Common.Replace_WithSpaces(dbDiscipline);
        }

        /// <summary>
        /// Converted displayed discipline for DB
        /// </summary>
        public static string ConvertDisplayDisciplineForDB(string displayDiscipline)
        {
            if (displayDiscipline == Variables.noDiscipline)
                return string.Empty;
            else
                return Common.ReplaceSpacesWith_(displayDiscipline);
        }

        /// <summary>
        /// Controls what to be populated in the treelist
        /// </summary>
        /// <param name="wbsTagDisplay">the full list</param>
        /// <param name="excludedGuid">the entry and its child we want to exclude</param>
        /// <param name="wbsOnly">to populate WBS only</param>
        /// <returns></returns>
        public static List<wbsTagDisplay> ProcessWBSTagTreeList(List<wbsTagDisplay> wbsTagDisplay, Guid? excludedGuid = null, bool wbsOnly = false)
        {
            AdapterTAG daTag = new AdapterTAG();
            AdapterWBS daWBS = new AdapterWBS();

            try
            {
                // allows root to be assigned as parent in all circumstances
                wbsTagDisplay.Add(new wbsTagDisplay(new WBS(Guid.Empty)
                {
                    wbsName = Variables.RootWBS,
                    wbsDescription = Variables.RootWBS,
                }));

                List<wbsTagDisplay> excludedWBSTag = new List<wbsTagDisplay>();
                dsWBS.WBSDataTable dtWBS = new dsWBS.WBSDataTable();
                dsTAG.TAGDataTable dtTag = new dsTAG.TAGDataTable();

                //try to get the childrens
                if (excludedGuid != null)
                {
                    dtWBS = daWBS.GetWBSChildrens((Guid)excludedGuid, false);
                    if (!wbsOnly)
                    {
                        dtTag = daTag.GetTagChildrens((Guid)excludedGuid, false);
                    }
                }

                foreach (wbsTagDisplay wbsTag in wbsTagDisplay)
                {
                    //exclude if wbsOnly or if its an excluded Tag children or itself
                    if (wbsTag.wbsTagDisplayAttachTag != null && (wbsOnly || (dtTag != null && dtTag.Any(obj => obj.GUID == wbsTag.wbsTagDisplayAttachTag.GUID) || wbsTag.wbsTagDisplayAttachTag.GUID == excludedGuid)))
                        excludedWBSTag.Add(wbsTag);

                    //exclude if its an excluded WBS children or itself
                    if (wbsTag.wbsTagDisplayAttachWBS != null && (dtWBS != null && dtWBS.Any(obj => obj.GUID == wbsTag.wbsTagDisplayAttachWBS.GUID) || wbsTag.wbsTagDisplayAttachWBS.GUID == excludedGuid))
                        excludedWBSTag.Add(wbsTag);
                }

                //remove the entries
                foreach (wbsTagDisplay removeWBSTag in excludedWBSTag)
                {
                    wbsTagDisplay.Remove(removeWBSTag);
                }

                return wbsTagDisplay;
            }
            finally
            {
                daTag.Dispose();
                daWBS.Dispose();
            }
        }
        #endregion

        #region Imaging
        /// <summary>
        /// Converts image to bytes to store in database
        /// </summary>
        public static byte[] ConvertImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        /// <summary>
        /// Convertes bytes to image to show on form
        /// </summary>
        public static Image ConvertByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        /// <summary>
        /// Method to resize, convert and save the image.
        /// </summary>
        /// <param name="image">Bitmap image.</param>
        /// <param name="maxWidth">resize width.</param>
        /// <param name="maxHeight">resize height.</param>
        /// <param name="quality">quality setting value.</param>
        /// <param name="filePath">file path.</param>      
        public static Bitmap ResizeBitmap(Bitmap image, int maxWidth, int maxHeight)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }
        #endregion

        #region IEnumerables
        /// <summary>
        /// Get's all the child WBSTagDisplay
        /// </summary>
        public static IEnumerable<wbsTagDisplay> AllWBSTagChildren(Guid parentGuid, List<wbsTagDisplay> allWBSTagDisplay)
        {
            foreach (wbsTagDisplay wbsTagChild in allWBSTagDisplay)
            {
                if (wbsTagChild.wbsTagDisplayParentGuid == parentGuid)
                {
                    yield return wbsTagChild;

                    foreach (wbsTagDisplay wbsTagGrandchild in AllWBSTagChildren(wbsTagChild.wbsTagDisplayGuid, allWBSTagDisplay))
                        yield return wbsTagGrandchild;
                }
            }
        }
        #endregion

        #region Class Creation
        /// <summary>
        /// Populate the Punchlist Display class based on parameters
        /// </summary>
        /// <param name="wbsTag">Contains either attached WBS or Tag</param>
        /// <param name="drPunchlist">Attaching punchlist</param>
        /// <param name="enabled">Whether punchlist should be enabled - depending on WBS/Tag status</param>
        public static Punchlist CreatePunchlistTagWBS(wbsTagDisplay wbsTag, dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagRow drPunchlist, bool enabled)
        {
            string attachmentName = string.Empty;
            if (wbsTag.wbsTagDisplayAttachTag != null)
                attachmentName = wbsTag.wbsTagDisplayAttachTag.tagNumber;
            else if (wbsTag.wbsTagDisplayAttachWBS != null)
                attachmentName = wbsTag.wbsTagDisplayAttachWBS.wbsName;

            Punchlist newPunchlist = new Punchlist(drPunchlist.GUID)
            {
                punchlistTitle = drPunchlist.TITLE,
                punchlistDescription = drPunchlist.DESCRIPTION,
                punchlistDiscipline = drPunchlist.DISCIPLINE,
                punchlistRemedial = drPunchlist.REMEDIAL,
                punchlistCategory = drPunchlist.CATEGORY,
                punchlistPriority = drPunchlist.PRIORITY,
                punchlistActionBy = drPunchlist.ACTIONBY,
                punchlistAttachTag = wbsTag.wbsTagDisplayAttachTag,
                punchlistAttachWBS = wbsTag.wbsTagDisplayAttachWBS,
                punchlistDisplayAttachment = attachmentName,
                punchlistITR = new PunchlistITRTemplate(Common.ConvertITRGuidToTemplateName(drPunchlist.ITR_GUID), drPunchlist.ITR_GUID, Guid.Empty),
                punchlistItem = drPunchlist.ITR_PUNCHLIST_ITEM,
                punchlistImageIndex = 0,
                punchlistEnabled = enabled
            };

            return newPunchlist;
        }

        /// <summary>
        /// Populate the Punchlist Display class based on parameters
        /// </summary>
        /// <param name="wbsTag">Contains either attached WBS or Tag</param>
        /// <param name="drPunchlist">Attaching punchlist</param>
        /// <param name="enabled">Whether punchlist should be enabled - depending on WBS/Tag status</param>
        public static Punchlist CreatePunchlistTagWBS(wbsTagDisplay wbsTag, List<wbsTagDisplay> wbsTagsList, dsPUNCHLIST_MAIN.PUNCHLIST_MAINWBSTagSystemRow drPunchlist, bool enabled)
        {
            string attachmentName = string.Empty;
            if (wbsTag.wbsTagDisplayAttachTag != null)
                attachmentName = wbsTag.wbsTagDisplayAttachTag.tagNumber;
            else if (wbsTag.wbsTagDisplayAttachWBS != null)
                attachmentName = wbsTag.wbsTagDisplayAttachWBS.wbsName;

            Punchlist newPunchlist = new Punchlist(drPunchlist.GUID)
            {
                punchlistTitle = drPunchlist.TITLE,
                punchlistDescription = drPunchlist.DESCRIPTION,
                punchlistDiscipline = drPunchlist.DISCIPLINE,
                punchlistRemedial = drPunchlist.REMEDIAL,
                punchlistCategory = drPunchlist.CATEGORY,
                punchlistPriority = drPunchlist.PRIORITY,
                punchlistActionBy = drPunchlist.ACTIONBY,
                punchlistAttachTag = wbsTag.wbsTagDisplayAttachTag,
                punchlistAttachWBS = wbsTag.wbsTagDisplayAttachWBS,
                punchlistParentWBSGuid = drPunchlist.IsAttachedWBSGuidNull() ? Guid.Empty : drPunchlist.AttachedWBSGuid,
                punchlistParentWBSName = drPunchlist.IsAttachedWBSGuidNull() ? string.Empty : drPunchlist.AttachedWBSName,
                punchlistParentWBSDesc = drPunchlist.IsAttachedWBSGuidNull() ? string.Empty : FindWBSDesc(wbsTagsList, drPunchlist.AttachedWBSGuid),
                punchlistDisplayAttachment = attachmentName,
                punchlistITR = new PunchlistITRTemplate(Common.ConvertITRGuidToTemplateName(drPunchlist.ITR_GUID), drPunchlist.ITR_GUID, Guid.Empty),
                punchlistItem = drPunchlist.ITR_PUNCHLIST_ITEM,
                punchlistImageIndex = 0,
                punchlistEnabled = enabled
            };

            return newPunchlist;
        }


        public static string FindWBSDesc(List<wbsTagDisplay> wbsTagsList, Guid wbsGuid)
        {
            wbsTagDisplay findWBSTagDisplay = wbsTagsList.FirstOrDefault(x => x.wbsTagDisplayGuid == wbsGuid);
            if (findWBSTagDisplay != null)
                return findWBSTagDisplay.wbsTagDisplayDescription;
            else
                return string.Empty;
        }

        /// <summary>
        /// Get unique ID for Ad-Hoc punchlist
        /// </summary>
        public static int GetProjectPunchlistCount(string seed)
        {
            int punchlistCount = 0;

            Random rand = new Random(seed.GetHashCode());
            using (AdapterPUNCHLIST_MAIN daPUNCHLIST_MAIN = new AdapterPUNCHLIST_MAIN())
            {
                Guid projectGuid = (Guid)System_Environment.GetUser().userProject.Value;
                var dtPunchlist = daPUNCHLIST_MAIN.GetByProject(projectGuid);
                int minValue = dtPunchlist.Count() + punchlistCount;

                return rand.Next(minValue, minValue + 5000);
                //return dtPunchlist.Count() + punchlistCount + randomInt;
            }
        }

        /// <summary>
        /// Populate the wbsTagDisplay class based on parameters
        /// </summary>
        /// <param name="drITRorTemplate">Must be either ITR or Template</param>
        public static WorkflowTemplateTagWBS CreateWorkflowTemplateTagWBS(wbsTagDisplay WBSTag, DataRow drITRorTemplate, bool enabled, List<Workflow> workflows)
        {
            Template constructTemplate;
            Guid trueTemplateGuid = Guid.Empty;
            //if this is from a save ITR it won't be attached to a template anymore so template can be freely modified/deleted
            if (drITRorTemplate.GetType() == typeof(dsITR_MAIN.ITR_MAINRow))
            {
                dsITR_MAIN.ITR_MAINRow drITR = (dsITR_MAIN.ITR_MAINRow)drITRorTemplate;
                trueTemplateGuid = drITR.TEMPLATE_GUID;

                constructTemplate = new Template(Guid.NewGuid())
                {
                    templateName = drITR.NAME,
                    templateDescription = drITR.DESCRIPTION,
                    templateWorkFlow = new ValuePair(string.Empty, Guid.Empty), //workflow Guid is needed when deletion happens and iTR goes back into pending list
                    templateRevision = drITR.REVISION,
                    templateDiscipline = drITR.DISCIPLINE
                };
            }
            //if this is from a template register
            else if (drITRorTemplate.GetType() == typeof(dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGRow))
            {
                dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGRow drTemplate = (dsTEMPLATE_MAIN.TEMPLATE_MAIN_WBSTAGRow)drITRorTemplate;
                trueTemplateGuid = drTemplate.GUID;
                string workflowName = string.Empty;
                Workflow findWorkflow = workflows.FirstOrDefault(x => x.GUID == drTemplate.WORKFLOWGUID);
                if (findWorkflow != null)
                    workflowName = findWorkflow.workflowName;

                constructTemplate = new Template(Guid.NewGuid())
                {
                    templateName = RemoveCommentedTemplateSection(drTemplate.NAME),
                    //templateWorkFlow = new ValuePair(Common.ConvertWorkflowGuidToName(drTemplate.WORKFLOWGUID), drTemplate.WORKFLOWGUID),
                    templateWorkFlow = new ValuePair(workflowName, drTemplate.WORKFLOWGUID),
                    templateRevision = drTemplate.REVISION,
                    templateDescription = drTemplate.IsDESCRIPTIONNull() ? "" : drTemplate.DESCRIPTION,
                    templateDiscipline = drTemplate.DISCIPLINE,
                    templateQRSupport = drTemplate.QRSUPPORT,
                    templateSkipApproved = drTemplate.SKIPAPPROVED,
                    templateInternalRevision = GetTemplateInternalRevision(drTemplate.NAME)
                };
            }
            else
                return null;

            WorkflowTemplateTagWBS newWorkTemplateTagWBS = new WorkflowTemplateTagWBS(constructTemplate)
            {
                wtDisplayAttachmentName = WBSTag.wbsTagDisplayName,
                wtDisplayAttachmentDescription = WBSTag.wbsTagDisplayDescription,
                wtDisplayAttachTag = WBSTag.wbsTagDisplayAttachTag,
                wtDisplayAttachWBS = WBSTag.wbsTagDisplayAttachWBS,
                wtTaskNumber = GetStringHash(string.Concat(WBSTag.wbsTagDisplayName, constructTemplate.templateName)),
                wtTrueTemplateGuid = trueTemplateGuid, //treeList doesn't allow duplicate guid, hence a new Guid was created and the actual guid is stored here
                wtEnabled = enabled
            };

            return newWorkTemplateTagWBS;
        }

        public static string GetHeaderDataFromTag(Tag tag, List<Tuple<Guid, string, string>> cachedHeaderDatas, AdapterPREFILL_REGISTER daPREFILL_REGISTER, string fieldName)
        {
            string returnValue = string.Empty;
            if (tag != null)
            {
                Tuple<Guid, string, string> cachedHeaderData = cachedHeaderDatas.Where(x => x.Item1 == tag.GUID).Where(x => x.Item2.ToUpper() == fieldName.ToUpper()).FirstOrDefault();
                if (cachedHeaderData != null)
                {
                    returnValue = cachedHeaderData.Item3;
                }
                else
                {
                    dsPREFILL_REGISTER.PREFILL_REGISTERRow drPREFILL_REGISTER = daPREFILL_REGISTER.GetBy(tag.GUID, fieldName, PrefillType.Tag);
                    if (drPREFILL_REGISTER != null)
                    {
                        cachedHeaderDatas.Add(new Tuple<Guid, string, string>(tag.GUID, fieldName, drPREFILL_REGISTER.DATA));
                        returnValue = drPREFILL_REGISTER.DATA;
                    }
                    else
                    {
                        cachedHeaderDatas.Add(new Tuple<Guid, string, string>(tag.GUID, fieldName, string.Empty));
                        returnValue = string.Empty;
                    }
                }
            }

            return returnValue;
        }

        public static string GetHeaderDataFromWBS(WBS wbs, List<Tuple<Guid, string, string>> cachedHeaderDatas, AdapterPREFILL_REGISTER daPREFILL_REGISTER, string fieldName)
        {
            string returnValue = string.Empty;
            if (wbs != null)
            {
                Tuple<Guid, string, string> cachedHeaderData = cachedHeaderDatas.Where(x => x.Item1 == wbs.GUID).Where(x => x.Item2.ToUpper() == fieldName.ToUpper()).FirstOrDefault();
                if (cachedHeaderData != null)
                {
                    returnValue = cachedHeaderData.Item3;
                }
                else
                {
                    dsPREFILL_REGISTER.PREFILL_REGISTERRow drPREFILL_REGISTER = daPREFILL_REGISTER.GetBy(wbs.GUID, fieldName, PrefillType.Tag);
                    if (drPREFILL_REGISTER != null)
                    {
                        cachedHeaderDatas.Add(new Tuple<Guid, string, string>(wbs.GUID, fieldName, drPREFILL_REGISTER.DATA));
                        returnValue = drPREFILL_REGISTER.DATA;
                    }
                    else
                    {
                        cachedHeaderDatas.Add(new Tuple<Guid, string, string>(wbs.GUID, fieldName, string.Empty));
                        returnValue = string.Empty;
                    }
                }
            }

            return returnValue;
        }

        public static string GetStringHash(string inputString)
        {
            //string hashCode = String.Format("{0:X}", inputString.GetHashCode());
            string hashCode = String.Format("{0:X}", inputString.GetHashCode());
            return hashCode;
        }

        /// <summary>
        /// The template name contains section which are separated by '|' character and its used only during template design to keep track of additional information
        /// Hence it needs to be separated out when the template is actually in use
        /// </summary>
        public static string RemoveCommentedTemplateSection(string defaultTemplateName)
        {
            return defaultTemplateName.Split('|').First();
        }

        public static string GetTemplateInternalRevision(string defaultTemplateName)
        {
            List<string> templateNameComponents = defaultTemplateName.Split('|').ToList();
            if (templateNameComponents.Count > 1)
                return templateNameComponents[1];

            return string.Empty;
        }
        #endregion

        #region RichEdit
        /// <summary>
        /// Retrieve the prefill fields from richEdit header, content and footer
        /// </summary>
        /// <param name="richEditControl1">The richEdit to retrieve fields from</param>
        /// <param name="wbsTag">wbsTag to store the prefill fields assigned to it</param>
        /// <param name="ExcludedPrefills">System prefill to exclude</param>
        /// <param name="uniqueHeader">Storing the unique prefill fields</param>
        public static void RetrievePrefillFields(RichEditControl richEditControl1, wbsTagHeader wbsTag, List<string> ExcludedPrefills, List<string> uniqueHeader, List<string> currentUniqueHeaders = null)
        {
            //Get fields from document
            FieldCollection fields = richEditControl1.Document.Fields;

            foreach (Field field in fields)
            {
                string sPrefill = richEditControl1.Document.GetText(field.Range);
                ////store header wbs/tag uses
                if (wbsTag != null && wbsTag.wbsTagHeaderItems.IndexOf(sPrefill) == -1 && ExcludedPrefills.IndexOf(sPrefill) == -1)
                {
                    wbsTag.wbsTagHeaderItems.Add(sPrefill);
                }

                if (currentUniqueHeaders != null && ExcludedPrefills.IndexOf(sPrefill) == -1)
                    currentUniqueHeaders.Add(sPrefill);

                if (uniqueHeader.IndexOf(sPrefill) == -1 && ExcludedPrefills.IndexOf(sPrefill) == -1)
                {
                    //store unique header for spreadsheet
                    uniqueHeader.Add(sPrefill);
                }
            }

            //Get fields from header
            Section firstSection = richEditControl1.Document.Sections[0];
            SubDocument headerDoc = firstSection.BeginUpdateHeader();

            for (int i = 0; i < headerDoc.Fields.Count; i++)
            {
                string sPrefill = headerDoc.GetText(headerDoc.Fields[i].Range);
                if (wbsTag != null && wbsTag.wbsTagHeaderItems.IndexOf(sPrefill) == -1 && ExcludedPrefills.IndexOf(sPrefill) == -1)
                {
                    wbsTag.wbsTagHeaderItems.Add(sPrefill);
                }

                if (currentUniqueHeaders != null && ExcludedPrefills.IndexOf(sPrefill) == -1)
                    currentUniqueHeaders.Add(sPrefill);

                if (uniqueHeader.IndexOf(sPrefill) == -1 && ExcludedPrefills.IndexOf(sPrefill) == -1)
                {
                    uniqueHeader.Add(sPrefill);
                }
            }
            richEditControl1.Document.Sections[0].EndUpdateHeader(headerDoc);

            //Get fields from footer
            SubDocument footerDoc = firstSection.BeginUpdateFooter();
            for (int i = 0; i < footerDoc.Fields.Count; i++)
            {
                string sPrefill = footerDoc.GetText(footerDoc.Fields[i].Range);
                if (wbsTag != null && wbsTag.wbsTagHeaderItems.IndexOf(sPrefill) == -1 && ExcludedPrefills.IndexOf(sPrefill) == -1)
                {
                    wbsTag.wbsTagHeaderItems.Add(sPrefill);
                }

                if (currentUniqueHeaders != null && ExcludedPrefills.IndexOf(sPrefill) == -1)
                    currentUniqueHeaders.Add(sPrefill);

                if (uniqueHeader.IndexOf(sPrefill) == -1 && ExcludedPrefills.IndexOf(sPrefill) == -1)
                {
                    uniqueHeader.Add(sPrefill);
                }
            }
            richEditControl1.Document.Sections[0].EndUpdateFooter(footerDoc);
        }
        #endregion

        #region Others
        //resets WBS checked edit values
        public static void ResetCheckEdit(CheckedComboBoxEdit checkedComboBoxEdit, List<ProjectWBS> dataSource, List<string> selectionSource)
        {
            selectionSource.Clear();
            checkedComboBoxEdit.Properties.DataSource = dataSource;
            checkedComboBoxEdit.Refresh();
            checkedComboBoxEdit.EditValue = null;
            checkedComboBoxEdit.RefreshEditValue();
        }

        public static void ClearCheckedComboBoxEdit(CheckedComboBoxEdit checkedComboBoxEdit)
        {
            for (int i = 0; i < checkedComboBoxEdit.Properties.Items.Count; i++)
                checkedComboBoxEdit.Properties.Items[i].CheckState = CheckState.Unchecked;
        }


        /// <summary>
        /// Get's all the parent WBSTagDisplay
        /// </summary>
        public static IEnumerable<wbsTagDisplay> AllParent(Guid parentGuid, List<wbsTagDisplay> allWBSTagDisplay)
        {
            foreach (wbsTagDisplay wbsTagParent in allWBSTagDisplay)
            {
                if (wbsTagParent.wbsTagDisplayGuid == parentGuid)
                {
                    yield return wbsTagParent;

                    foreach (wbsTagDisplay wbsTagGrandparent in AllParent(wbsTagParent.wbsTagDisplayParentGuid, allWBSTagDisplay))
                    {
                        yield return wbsTagGrandparent;
                    }
                }
            }
        }

        public static List<string> GetElectricalHeaderList()
        {
            List<string> electricalHeaderList = new List<string>();
            electricalHeaderList.Add("From Equip");
            electricalHeaderList.Add("FROM");
            electricalHeaderList.Add("To Equip");
            electricalHeaderList.Add("TO");
            electricalHeaderList.Add("GA DRAWING");
            electricalHeaderList.Add("P&ID");
            electricalHeaderList.Add("Schematic");
            electricalHeaderList.Add("Route Length(m)");
            electricalHeaderList.Add("Drum NO");
            electricalHeaderList.Add("Live drum reg");
            electricalHeaderList.Add("Cable Type");
            electricalHeaderList.Add("Cable Type Concatenate");
            electricalHeaderList.Add("Size");
            electricalHeaderList.Add("Cores");
            electricalHeaderList.Add("Conductor");
            electricalHeaderList.Add("Sheath");
            electricalHeaderList.Add("Color");
            electricalHeaderList.Add("Voltage");
            electricalHeaderList.Add("Rev");
            electricalHeaderList.Add("Cable OD");
            electricalHeaderList.Add("Gland Size");
            electricalHeaderList.Add("Start M");
            electricalHeaderList.Add("End M");
            electricalHeaderList.Add("Actul Metres");
            electricalHeaderList.Add("Installed Cables");
            electricalHeaderList.Add("Termination Score");
            electricalHeaderList.Add("ITR Score");
            electricalHeaderList.Add("DATE INSTALLED");
            electricalHeaderList.Add("FROM Gland");
            electricalHeaderList.Add("FROM Term");
            electricalHeaderList.Add("TO Gland ");
            electricalHeaderList.Add("TO Term");
            electricalHeaderList.Add("ITR");
            electricalHeaderList.Add("Installed Gap");

            return electricalHeaderList;
        }

        public class XMLDatabase
        {
            public string Server { get; set; }
            public string Database { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public static XMLDatabase LoadDatabaseXML(bool pairingMode)
        {
            string xmlFilePath = Common.DatabaseXMLFilePath(true);
            string databaseType = "Local";
            if (pairingMode)
                databaseType = "Remote";

            XMLDatabase xMLDatabase = new XMLDatabase();
            XDocument doc = null;
            if (File.Exists(xmlFilePath))
            {
                try
                {
                    doc = XDocument.Load(xmlFilePath);
                }
                catch  //if xml file fails to load recreate it
                {
                    return null;
                }
            }

            if (doc != null)
            {
                XElement findDatabase = doc.Root.Descendants().FirstOrDefault(obj => obj.Name == databaseType);
                if (findDatabase != null)
                {
                    xMLDatabase.Server = findDatabase.Attribute("Url").Value;
                    xMLDatabase.Database = Common.Decrypt(findDatabase.Attribute("Database").Value, true);
                    xMLDatabase.UserName = Common.Decrypt(findDatabase.Attribute("Username").Value, true);
                    xMLDatabase.Password = Common.Decrypt(findDatabase.Attribute("Password").Value, true);
                    return xMLDatabase;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieve the designated file path for xml
        /// </summary>
        public static string DatabaseXMLFilePath(bool createDirectory)
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string userFilePath = Path.Combine(localAppData, Variables.defaultCheckmateDirectory);

            if (!Directory.Exists(userFilePath) && createDirectory)
                Directory.CreateDirectory(userFilePath);

#if DEBUG
            string destFilePath = Path.Combine(userFilePath, Variables.databaseXMLFilename2);
#else
            string destFilePath = Path.Combine(userFilePath, Variables.databaseXMLFilename);
#endif
            return destFilePath;
        }

        /// <summary>
        /// Check if this app is running on Windows 8 or newer
        /// </summary>
        /// <returns></returns>
        public static bool IsWindows8()
        {
            var os = Environment.OSVersion;
            return os.Platform == PlatformID.Win32NT &&
                   (os.Version.Major > 6 || (os.Version.Major == 6 && os.Version.Minor >= 2));
        }

        /// <summary>
        /// Check if device is used in portrait mode
        /// </summary>
        public static bool IsPortraitMode()
        {
            int theScreenRectWidth = Screen.PrimaryScreen.Bounds.Width;
            if (theScreenRectWidth < 1920)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Brings up windows 8 touch keyboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void textBox_GotFocus(object sender, EventArgs e)
        {
            //disable keyboard showing because sometimes user plugs in bluetooth keyboard
            //if (IsWindows8() && IsPortraitMode())
            //    //if (IsWindows8())
            //    Process.Start(@"C:\Program Files\Common Files\microsoft shared\ink\tabtip.exe");
        }

        /// <summary>
        /// Hide the windows 8 touch keyboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void textBox_Leave(object sender, EventArgs e)
        {
            //if (System.Diagnostics.Process.GetProcessesByName("TabTip").Count() > 0)
            //{
            //    System.Diagnostics.Process asd = System.Diagnostics.Process.GetProcessesByName("TabTip").First();
            //    asd.Kill();
            //}
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

#endregion
    }
}
