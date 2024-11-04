using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Data.SqlClient;
using ProjectCommon;
using ProjectDatabase.Dataset;
using ProjectDatabase.DataAdapters;
using ProjectLibrary;
using static ProjectCommon.Common;

namespace CheckmateDX
{
    public partial class frmDatabase_Config : frmParent
    {
        bool _pairingMode = false;
        string hwid = Common.GetHWID();
        public frmDatabase_Config(bool pairing)
        {
            InitializeComponent();
            LoadDatabaseXML(pairing);
            lblCurrentHWID.Text = hwid;

            if(pairing)
            {
                btnOk.Text = "Pair";
                _pairingMode = true;
            }
            else
            {
                panelHWID.Visible = false;
            }
        }

        #region Initialisation
        private void LoadDatabaseXML(bool pairingMode)
        {
            XMLDatabase XmlDatabase = Common.LoadDatabaseXML(pairingMode);
            
            if (XmlDatabase != null)
            {
                txtServer.Text = XmlDatabase.Server;
                txtDatabase.Text = XmlDatabase.Database;
                txtUsername.Text = XmlDatabase.UserName;
                txtPassword.Text = XmlDatabase.Password;
            }
            else
                return;
        }
        #endregion

        #region Output
        /// <summary>
        /// Updates the database XML
        /// </summary>
        /// <returns>Connection string</returns>
        private string UpdateXML()
        {
            string xmlFilePath = Common.DatabaseXMLFilePath(true);
            string databaseType = "Local";
            if (_pairingMode)
                databaseType = "Remote";

            XDocument doc;
            if(File.Exists(xmlFilePath))
            {
                try
                {
                    doc = XDocument.Load(xmlFilePath);
                }
                catch  //if xml file fails to load recreate it
                {
                    File.Delete(xmlFilePath);
                    doc = new XDocument(new XDeclaration("1.0", "utf-8", null),
                            new XElement("Databases")
                        );
                }
            }
            else
                doc = new XDocument(new XDeclaration("1.0", "utf-8", null),
                        new XElement("Databases")
                    );

            if(!doc.Root.Descendants().Any(obj => obj.Name == databaseType))
            {
                doc.Root.Add(new XElement(databaseType,
                                new XAttribute("Url", txtServer.Text.Trim()),
                                new XAttribute("Database", Common.Encrypt(txtDatabase.Text.Trim(), true)),
                                new XAttribute("Username", Common.Encrypt(txtUsername.Text.Trim(), true)),
                                new XAttribute("Password", Common.Encrypt(txtPassword.Text.Trim(), true))
                            ));
            }
            else
            {
                XElement findDatabase = doc.Root.Descendants().FirstOrDefault(obj => obj.Name == databaseType);
                //dont need to check for null because we've already checked for any
                findDatabase.Attribute("Url").Value = txtServer.Text.Trim();
                findDatabase.Attribute("Database").Value = Common.Encrypt(txtDatabase.Text.Trim(), true);
                findDatabase.Attribute("Username").Value = Common.Encrypt(txtUsername.Text.Trim(), true);
                findDatabase.Attribute("Password").Value = Common.Encrypt(txtPassword.Text.Trim(), true);
            }

            doc.Save(xmlFilePath);
            return Common.ConstructConnString(txtServer.Text.Trim(), txtDatabase.Text.Trim(), txtUsername.Text.Trim(), txtPassword.Text.Trim());
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Test database connection string
        /// </summary>
        /// <returns>true if successful</returns>
        public bool TestConnection(string connStr)
        {
            SqlConnection conn = new SqlConnection(connStr);

            try
            {
                conn.Open();
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Events
        private void btnOk_Click(object sender, EventArgs e)
        {
            string connStr = UpdateXML();
            if (!TestConnection(connStr))
            {
                Common.Prompt("Connection failed!");
                return;
            }

            if(_pairingMode)
            {
                AdapterSYNC_PAIR daSync = new AdapterSYNC_PAIR(connStr);
                dsSYNC_PAIR.SYNC_PAIRRow drSync = daSync.GetBy(hwid);
                if(drSync != null)
                {
                    Common.Prompt("Pairing request was previously sent");
                }
                else
                {
                    dsSYNC_PAIR dsSYNC = new dsSYNC_PAIR();
                    dsSYNC_PAIR.SYNC_PAIRRow drNewSyncPair = dsSYNC.SYNC_PAIR.NewSYNC_PAIRRow();

                    //splashScreenManager1.ShowWaitForm();
                    //splashScreenManager1.SetWaitFormDescription("Retrieving Your IP ...");
                    //string externalIP = Common.GetExternalIP();
                    //splashScreenManager1.CloseWaitForm();

                    //if (externalIP != "0.0.0.0")
                    //{
                        drNewSyncPair.GUID = Guid.NewGuid();
                        drNewSyncPair.HWID = hwid;
                        drNewSyncPair.IP_ADDRESS = "0.0.0.0";
                        drNewSyncPair.DESCRIPTION = Environment.MachineName;
                        drNewSyncPair.APPROVED = false;
                        drNewSyncPair.CREATED = DateTime.Now;
                        drNewSyncPair.CREATEDBY = System_Environment.GetUser().GUID;
                        dsSYNC.SYNC_PAIR.AddSYNC_PAIRRow(drNewSyncPair);
                        daSync.Save(drNewSyncPair);
                        Common.Prompt("Pair request sent");
                    //}
                    //else
                    //    Common.Prompt("Cannot send pair request now, please try again later");
                }
            }
            else
                Common.Prompt("Please restart the application for the new setting to take effect");

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string connStr = Common.ConstructConnString(txtServer.Text.Trim(), txtDatabase.Text.Trim(), txtUsername.Text.Trim(), txtPassword.Text.Trim());
            if (TestConnection(connStr))
                Common.Prompt("Connection successful");
            else
                Common.Prompt("Connection failed");
        }
        #endregion
    }
}
