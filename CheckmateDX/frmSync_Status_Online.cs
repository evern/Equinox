using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Linq;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectCommon;
using ProjectLibrary;
using System.IO;
using System.Xml.Linq;
using DevExpress.XtraPrinting;

namespace CheckmateDX
{
    public partial class frmSync_Status_Online : CheckmateDX.frmParent
    {
        //global storage
        Guid _projectGuid;
        Guid _syncTableGuid; //guid for the sync table to look up sync settings
        string _discipline;
        string _HWID = Common.GetHWID();

        List<SyncStatus_Superseded> _SyncItems = new List<SyncStatus_Superseded>(); //stores the sync status for preview
        SyncWebService.CheckmateSyncSoapClient SyncWebService = new SyncWebService.CheckmateSyncSoapClient();

        public frmSync_Status_Online()
        {
            InitializeComponent();
            lblSyncServer.Text = "Perth Server";
            syncStatusBindingSource.DataSource = _SyncItems;

            //get user parameter
            _projectGuid = (Guid)System_Environment.GetUser().userProject.Value;
            _discipline = System_Environment.GetUser().userDiscipline;
            timer1.Enabled = true;
        }

        #region ITR Sync
        private void Sync_ITR()
        {
            AdapterITR_MAIN daITR = new AdapterITR_MAIN();
            List<WebServer_ITRSummary> iTRs =  WebServiceCommon.GenerateSyncITR(_projectGuid, _discipline); //generates the list of ITR to check for sync direction

            foreach(WebServer_ITRSummary iTR in iTRs)
            {

            }
        }
        #endregion

        #region Pairing Functions
        /// <summary>
        /// Read the remote connection string from XML
        /// </summary>
        private string GetConnStrFromXML(out string dbName)
        {
            string xmlFilePath = Common.DatabaseXMLFilePath(false);
            dbName = "N/A";

            if (File.Exists(xmlFilePath))
            {
                try
                {
                    XDocument doc = XDocument.Load(xmlFilePath);
                    XElement findDatabase = doc.Root.Descendants().FirstOrDefault(obj => obj.Name == "Remote");
                    if (findDatabase != null)
                    {
                        string server = findDatabase.Attribute("Url").Value;
                        string database = Common.Decrypt(findDatabase.Attribute("Database").Value, true);
                        string username = Common.Decrypt(findDatabase.Attribute("Username").Value, true);
                        string password = Common.Decrypt(findDatabase.Attribute("Password").Value, true);
                        string connStr = Common.ConstructConnString(server, database, username, password);

                        dbName = server;
                        return connStr;
                    }
                }
                catch
                {

                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Checks whether sync was approved for this machine
        /// </summary>
        /// <returns></returns>
        private bool CheckSyncApproval()
        {
            _syncTableGuid = SyncWebService.Sync_CheckApproval(_HWID);

            if(_syncTableGuid != Guid.Empty)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Establish the sync environment, also allows superuser to select environment parameters
        /// </summary>
        private bool EstablishEnvironmentParameters()
        {
            if ((Guid)System_Environment.GetUser().userRole.Value == Guid.Empty)
            {
                frmTool_Project frmSelectProject = new frmTool_Project();
                frmSelectProject.ShowAsDialog();
                if (frmSelectProject.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _projectGuid = frmSelectProject.GetSelectedProject().GUID;
                }
                else
                {
                    this.Close();
                    return false;
                }

                frmITR_Select_Discipline frmSelectDiscipline = new frmITR_Select_Discipline();

                if (frmSelectDiscipline.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _discipline = frmSelectDiscipline.GetDiscipline();
                }
                else
                {
                    this.Close();
                    return false;
                }

                return true;
            }

            return true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (EstablishEnvironmentParameters())
                Sync_ITR();
            //if (!CheckSyncApproval())
            //{
            //    Common.Warn("Pairing request isn't approved yet");
            //    return;
            //}
            //else
            //{
            //    Common.Prompt("Sync Approved");
            //    return;
            //}
        }
        #endregion

    }
}
