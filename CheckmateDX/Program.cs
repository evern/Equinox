using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Deployment.Application;
using ProjectCommon;
using System.Xml.Linq;
using System.IO;

namespace CheckmateDX
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Assembly asm = typeof(DevExpress.UserSkins.PrimeroSkin).Assembly;
            DevExpress.Skins.SkinManager.Default.RegisterAssembly(asm);
            DevExpress.Utils.AppearanceObject.DefaultFont = new System.Drawing.Font("Candara", 10f);
            //DevExpress.XtraEditors.WindowsFormsSettings.TouchUIMode = DevExpress.LookAndFeel.TouchUIMode.True;
            //DevExpress.XtraEditors.WindowsFormsSettings.TouchScaleFactor = 1.0f;\
            if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AutoUpdate.AutoUpdater au = new AutoUpdate.AutoUpdater();
            au.Update();
            CreateProjectSpecificXML();
            Application.Run(new frmLogin());
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        /// <summary>
        /// Custom project specific deployment
        /// </summary>
        private static void CreateProjectSpecificXML()
        {
            string xmlFilePath = Common.DatabaseXMLFilePath(true);
            string databaseType = "Local";
            string siteSpecificServerURL = "APP-SQL-P08";
            string siteSpecificDatabaseName = "CHECKMATE";
            string siteSpecificUsername = "GuestUser01";
            string siteSpecificPassword = "v1$$X&5Wv*9:";

            XDocument doc;
            // Load the XML document or create a new one if it doesn't exist
            if (File.Exists(xmlFilePath))
            {
                doc = XDocument.Load(xmlFilePath);
            }
            else
            {
                doc = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("Databases"));
            }

            // Find the database element by type
            XElement databaseElement = doc.Root.Descendants()
                .FirstOrDefault(obj => obj.Name == databaseType);

            if (databaseElement != null)
            {
                var server = databaseElement.Attribute("Url").Value;

                if (server.Contains("DC2"))
                {
                    // Update the server URL and other attributes
                    UpdateDatabaseAttributes(databaseElement, siteSpecificServerURL, siteSpecificDatabaseName, siteSpecificUsername, siteSpecificPassword);
                    doc.Save(xmlFilePath);
                }
            }
            else
            {
                // Database element does not exist, so create a new one
                XElement newDatabaseElement = CreateDatabaseElement(databaseType, siteSpecificServerURL, siteSpecificDatabaseName, siteSpecificUsername, siteSpecificPassword);
                doc.Root.Add(newDatabaseElement);
                doc.Save(xmlFilePath);
            }
        }

        private static XElement CreateDatabaseElement(string type, string serverUrl, string dbName, string username, string password)
        {
            return new XElement(type,
                new XAttribute("Url", serverUrl),
                new XAttribute("Database", Common.Encrypt(dbName, true)),
                new XAttribute("Username", Common.Encrypt(username, true)),
                new XAttribute("Password", Common.Encrypt(password, true))
            );
        }

        private static void UpdateDatabaseAttributes(XElement databaseElement, string serverUrl, string dbName, string username, string password)
        {
            databaseElement.Attribute("Url").Value = serverUrl;
            databaseElement.Attribute("Database").Value = Common.Encrypt(dbName, true);
            databaseElement.Attribute("Username").Value = Common.Encrypt(username, true);
            databaseElement.Attribute("Password").Value = Common.Encrypt(password, true);
        }
    }


        public class SkinRegistration : Component
    {
        public SkinRegistration()
        {
            DevExpress.Skins.SkinManager.Default.RegisterAssembly(typeof(DevExpress.UserSkins.PrimeroSkin).Assembly);
        }
    }
}
