using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;
using ProjectCommon;
using ProjectDatabase;
using ProjectDatabase.DataAdapters;
using ProjectDatabase.Dataset;
using ProjectLibrary;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Threading;
using DevExpress.XtraRichEdit;
using System.Deployment.Application;
using static ProjectCommon.Common;
using System.Data.SqlClient;

namespace CheckmateDX
{
    public partial class frmLogin : frmParent
    {
        System.Windows.Forms.Timer loginTimeout = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer autoLoginTimer = new System.Windows.Forms.Timer();
        mdiMain mdiMain1;
        
        public frmLogin()
        {
            InitializeComponent();
            loginTimeout.Interval = 5000;
            loginTimeout.Tick += loginTimeout_Tick;

            //autoLoginTimer.Interval = 1000;
            //autoLoginTimer.Tick += AutoLoginTimer_Tick;
            //autoLoginTimer.Start();
            //new Thread(PreloadITRComponents).Start();
            //new Thread(PreloadSpreadsheet).Start();
            //new Thread(PreloadTemplateComponents).Start();
            //new Thread(PreloadDeletionForm).Start();
            DevExpress.UserSkins.BonusSkins.Register();
            txtUsername.Enter += new EventHandler(Common.textBox_GotFocus);
            txtPassword.Enter += new EventHandler(Common.textBox_GotFocus);
            updateDatabaseSchema();
            //#if DEBUG

            //#endif

            lblVersion.Text = "Release 1.3.0.8";
            //if (ApplicationDeployment.IsNetworkDeployed)
            //    lblVersion.Text = "Release " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(); 
        }

        private void AutoLoginTimer_Tick(object sender, EventArgs e)
        {
            autoLoginTimer.Stop();
            txtUsername.Text = "su";
            txtPassword.Text = "qweasd";
            btnLogin_Click(null, null);
        }

        void loginTimeout_Tick(object sender, EventArgs e)
        {
            int promptTerminateTime = 10800000;
            int terminateTime = 14400000;

            if (mdiMain1 != null)
            {
                uint idleTime = IdleTimeFinder.GetIdleTime();
                if (idleTime > promptTerminateTime && idleTime <= terminateTime)
                {
                    Common.Prompt("Hello! Just wanted to give you a heads up that your session has been inactive for 3 hours now, and the program will automatically terminate in an hour at " + DateTime.Now.AddHours(1).ToShortTimeString() + ". If you need more time, please move the mouse to reset the timer");
                }
                else if (idleTime > terminateTime)
                {
                    Environment.Exit(1);
                }
            }
        }

        private void PreloadITRComponents()
        {
            frmITR_Main f = new frmITR_Main();
            f.Dispose();
        }

        private void PreloadSpreadsheet()
        {
            frmSchedule_Prefill f = new frmSchedule_Prefill();
            f.Dispose();
        }

        private void PreloadTemplateComponents()
        {
            frmTemplate_Main f = new frmTemplate_Main();
            f.Dispose();
        }

        private void PreloadDeletionForm()
        {
            rptDeletion f = new rptDeletion();
            f.Dispose();
        }

        #region Events
        /// <summary>
        /// Verify username and password combination and logs the user in
        /// </summary>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            #region SUPERADMIN DEFAULT USERNAME AND PASSWORD
            if (txtUsername.Text == Variables.defaultSuperadminUsername && txtPassword.Text == Variables.defaultSuperadminPassword)
            {
                if(!Common.SetConnStrFromXML())
                    Common.Prompt("Database.config is not found or corrupted, please reconfigure it");

                System_Environment.SetUser(new User(Guid.Empty)
                { 
                    userFirstName = "SUPER",
                    userLastName = "ADMIN", 
                    userDiscipline = Common.ConvertDBDisciplineForDisplay(string.Empty, Guid.Empty),
                    userProject = new ValuePair(Common.ConvertProjectGuidToName(Guid.Empty, Guid.Empty), Guid.Empty),
                    userRole = new ValuePair(Common.ConvertRoleGuidToName(Guid.Empty), Guid.Empty),
                    userQANumber = "SUPERADMINUSER"
                });

                foreach(Privilege privilege in System_Privilege.GetList())
                {
                    if(privilege.privTypeID != PrivilegeTypeID.ShowCompletedAndClosedITRsOnly.ToString())
                        System_Environment.AddPrivileges(privilege);
                }

                Common.textBox_Leave(null, null);
                mdiMain1 = new mdiMain() { ShowLoginForm = new mdiMain.UnhideLoginForm(ShowThisForm) };
                mdiMain1.Show();
                this.Hide();
                loginTimeout.Start();
            } 
            #endregion
            else if(VerifyForm())
            {
                if(!Common.SetConnStrFromXML())
                {
                    Common.Prompt("Database not configured, please contact the system administrator");
                    return;
                }

                AdapterUSER_MAIN daUser = new AdapterUSER_MAIN();
                AdapterROLE_MAIN daRole = new AdapterROLE_MAIN();

                try
                {
                    dsUSER_MAIN.USER_MAINRow drUser = daUser.VerifyLogin(txtUsername.Text.Trim(), Common.Encrypt(txtPassword.Text.Trim(), true), txtPassword.Text.Trim());
                    if (drUser != null)
                    {
                        #region Set Environment
                        System_Environment.SetUser(new User(drUser.GUID)
                        {
                            userFirstName = drUser.FIRSTNAME,
                            userLastName = drUser.LASTNAME,
                            userRole = new ValuePair(Common.ConvertRoleGuidToName(drUser.ROLE), (Guid)drUser.ROLE),
                            userCompany = drUser.COMPANY,
                            userDiscipline = Common.ConvertDBDisciplineForDisplay(drUser.DDISCIPLINE, drUser.ROLE),
                            userProject = new ValuePair(Common.ConvertProjectGuidToName(drUser.DPROJECT, drUser.ROLE), (Guid)drUser.DPROJECT),
                            userQANumber = drUser.QANUMBER
                        });
                        #endregion

                        #region Assign Privileges
                        dsROLE_MAIN.ROLE_PRIVILEGEDataTable dtPrivilege = daRole.GetPrivilegeBy(drUser.ROLE);
                        if (drUser.ROLE == Guid.Empty)
                        {
                            foreach (Privilege privilege in System_Privilege.GetList())
                            {
                                System_Environment.AddPrivileges(privilege);
                            }
                        }
                        else
                        {
                            if (dtPrivilege != null)
                            {
                                foreach (dsROLE_MAIN.ROLE_PRIVILEGERow drPrivilege in dtPrivilege.Rows)
                                {
                                    System_Environment.AddPrivileges(new Privilege(drPrivilege.GUID)
                                    {
                                        privCategory = Common.GetPrivilegeCategory(drPrivilege.TYPEID),
                                        privName = drPrivilege.NAME,
                                        privTypeID = drPrivilege.TYPEID
                                    }
                                    );
                                }
                            }
                        }
                        #endregion

                        //Checks if user has any authorised project
                        dsUSER_MAIN.USER_PROJECTDataTable dtUserProject = daUser.GetAuthProjects();
                        dsUSER_MAIN.USER_DISCDataTable dtUserDisc = daUser.GetAuthDisciplines();
                        if (drUser.ROLE != Guid.Empty && dtUserDisc == null)
                        {
                            Common.Prompt("You are not authorised to any discipline, please click select defaults or if that fail, please contact your supervisor");
                        }
                        else
                        {
                            bool promptLoginSelection = false;
                            //if user choose to change default project or doesn't have a default project
                            if (cbDefault.Checked || drUser.DPROJECT == Guid.Empty || drUser.DDISCIPLINE == string.Empty)
                            {
                                //only proceed if user is not a superuser
                                if (drUser.ROLE != Guid.Empty)
                                {
                                    #region Assign defaults
                                    string message = string.Empty;
                                    if (drUser.DPROJECT == Guid.Empty && drUser.DDISCIPLINE == string.Empty)
                                        message = "You don't have any assigned project and discipline, click ok to assign them";
                                    else if (drUser.DPROJECT == Guid.Empty)
                                        message = "You don't have any assigned project, click ok to assign default project";
                                    else if (drUser.DDISCIPLINE == string.Empty)
                                        message = "You don't have any assigned discipline, click ok to assign discipline";
                                    if (message == string.Empty || Common.Confirmation(message, "Defaults Not Assigned"))
                                    {
                                        promptLoginSelection = true;
                                    }
                                    else
                                        return;
                                    #endregion
                                }
                                else if (cbDefault.Checked)
                                    Common.Prompt("Default configuration aren't necessary\n\nYou already have access to all disciplines and projects as an Admin Superuser");
                            }
                            else if (drUser.ROLE != Guid.Empty)
                            {
                                dsUSER_MAIN.USER_PROJECTDataTable dtUSER_PROJECT = daUser.GetAuthProjects();
                                dsUSER_MAIN.USER_DISCDataTable dtUSER_DISCIPLINE =  daUser.GetAuthDisciplines();
                                string message = string.Empty;
                                if (dtUSER_PROJECT == null || !dtUSER_PROJECT.Any(x => x.PROJECTGUID == drUser.DPROJECT))
                                {
                                    drUser.DPROJECT = Guid.Empty;
                                    daUser.Save(drUser);
                                    System_Environment.ChangeDefaultProject(new ValuePair(Common.ConvertProjectGuidToName(drUser.DPROJECT), drUser.DPROJECT));
                                    message = "Your access to your last accessed project has been removed, click ok to choose project";
                                    if(Common.Confirmation(message, "Defaults Not Assigned"))
                                        promptLoginSelection = true;
                                }
                                else if(dtUSER_DISCIPLINE == null || !dtUSER_DISCIPLINE.Any(x => x.DISCIPLINE.ToUpper() == drUser.DDISCIPLINE.ToUpper()))
                                {
                                    drUser.DDISCIPLINE = string.Empty;
                                    daUser.Save(drUser);
                                    System_Environment.ChangeDefaultDiscipline(drUser.DDISCIPLINE);
                                    message = "Your access to your last accessed discipline has been removed, click ok to choose discipline";
                                    if (Common.Confirmation(message, "Defaults Not Assigned"))
                                        promptLoginSelection = true;
                                }
                            }

                            if(promptLoginSelection)
                            {
                                frmLogin_Select frmLoginSelect = new frmLogin_Select(drUser.GUID);
                                if (frmLoginSelect.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    if (frmLoginSelect.getDefaultProject() != Guid.Empty)
                                    {
                                        drUser.DPROJECT = frmLoginSelect.getDefaultProject();
                                        drUser.DDISCIPLINE = frmLoginSelect.getDefaultDiscipline();
                                        drUser.UPDATED = DateTime.Now;
                                        drUser.UPDATEDBY = drUser.GUID;
                                        daUser.Save(drUser);

                                        System_Environment.ChangeDefaultDiscipline(drUser.DDISCIPLINE);
                                        System_Environment.ChangeDefaultProject(new ValuePair(Common.ConvertProjectGuidToName(drUser.DPROJECT), drUser.DPROJECT));
                                    }
                                }
                                else
                                    return;
                            }

                            if (Common.Decrypt(drUser.PASSWORD, true) == Variables.defaultPassword && Common.Confirmation("You are using a default password\n\nDo you want to change your password?", "Change Password Recommendation"))
                            {
                                frmTool_User_ChangePassword f = new frmTool_User_ChangePassword();
                                if (f.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                                    return;
                            }

                            if (drUser.IsSIGNATURENull() && Common.Confirmation("Do you wish to save your signature now?", "Save Signature"))
                            {
                                frmTool_User_ChangeSignature f = new frmTool_User_ChangeSignature();
                                if (f.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                                    return;
                            }

                            Common.textBox_Leave(null, null);
                            mdiMain1 = new mdiMain() { ShowLoginForm = ShowThisForm };
                            mdiMain1.Show();
                            this.Hide();
                            loginTimeout.Start();
                        }
                    }
                    else
                        Common.Warn("Incorrect Login Details");
                }
                finally
                {
                    daUser.Dispose();
                    daRole.Dispose();
                }
            }
        }

        /// <summary>
        /// Close this form
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(null, null);
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(null, null);
        }

        private void cbDefault_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(null, null);
        }
        #endregion

        #region Auxiliary
        /// <summary>
        /// Verify whether objects are populated in form
        /// </summary>
        /// <returns></returns>
        public bool VerifyForm()
        {
            if (txtUsername.Text.Trim() == "")
            {
                Common.Warn("Username must be entered");
                txtUsername.Focus();
                return false;
            }

            if (txtPassword.Text.Trim() == "")
            {
                Common.Warn("Password must be entered");
                txtPassword.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// A delegated method to unhide this form
        /// </summary>
        private void ShowThisForm()
        {
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
            cbDefault.Checked = false;
            txtUsername.Focus();
            this.Show();
        }
        #endregion

        #region Overrides
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Common.textBox_Leave(null, null);
            base.OnFormClosing(e);
        }
        #endregion

        private void cbDefault_CheckedChanged(object sender, EventArgs e)
        {

        }

        #region Database Schema Updates
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

        private bool updateDatabaseSchema()
        {
            if (Common.SetConnStrFromXML())
            {
                XMLDatabase XmlDatabase = Common.LoadDatabaseXML(false);
                bool isDatabaseConnectionError = false;
                if (XmlDatabase == null)
                    isDatabaseConnectionError = true;

                string connStr = Common.ConstructConnString(XmlDatabase.Server, XmlDatabase.Database, XmlDatabase.UserName, XmlDatabase.Password);
                if (!TestConnection(connStr))
                    isDatabaseConnectionError = true;

                if(isDatabaseConnectionError)
                {
                    Common.Warn("Database connection failed, please login as superadmin, configure the database and restart the application");
                    return false;
                }

                SQLBase sqlBase = new SQLBase();
                string sql = "IF (NOT EXISTS (SELECT *  ";
                sql += "FROM INFORMATION_SCHEMA.TABLES  ";
                sql += "WHERE TABLE_SCHEMA = 'dbo'  ";
                sql += "AND  TABLE_NAME = 'CERTIFICATE_MAIN')) ";
                sql += "BEGIN ";
                sql += "SET ANSI_NULLS ON ";
                sql += "SET QUOTED_IDENTIFIER ON ";
                sql += "SET ANSI_NULLS ON ";
                sql += "SET QUOTED_IDENTIFIER ON ";
                sql += "CREATE TABLE [dbo].[CERTIFICATE_MAIN]( ";
                sql += "[GUID] [uniqueidentifier] NOT NULL, ";
                sql += "[PROJECTGUID] [uniqueidentifier] NULL, ";
                sql += "[TEMPLATE_NAME] [nvarchar](50) NOT NULL, ";
                sql += "[NUMBER] [nvarchar](100) NOT NULL, ";
                sql += "[DESCRIPTION] [nvarchar](500) NULL, ";
                sql += "[CERTIFICATE] [varbinary](max) NOT NULL, ";
                sql += "[CREATED] [datetime] NOT NULL, ";
                sql += "[CREATEDBY] [uniqueidentifier] NOT NULL, ";
                sql += "[UPDATED] [datetime] NULL, ";
                sql += "[UPDATEDBY] [uniqueidentifier] NULL, ";
                sql += "[DELETED] [datetime] NULL, ";
                sql += "[DELETEDBY] [uniqueidentifier] NULL, ";
                sql += "CONSTRAINT [PK_CERTIFICATE_MAIN] PRIMARY KEY CLUSTERED  ";
                sql += "( ";
                sql += "[GUID] ASC ";
                sql += ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ";
                sql += ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";
                sql += "END ";
                bool success = createDatabaseTable(sql, "CERTIFICATE_MAIN");
                if (!success)
                    return false;

                sql = "IF (NOT EXISTS (SELECT *  ";
                sql += "FROM INFORMATION_SCHEMA.TABLES  ";
                sql += "WHERE TABLE_SCHEMA = 'dbo'  ";
                sql += "AND  TABLE_NAME = 'CERTIFICATE_DATA')) ";
                sql += "BEGIN ";
                sql += "SET ANSI_NULLS ON ";
                sql += "SET QUOTED_IDENTIFIER ON ";
                sql += "CREATE TABLE [dbo].[CERTIFICATE_DATA]( ";
                sql += "[GUID] [uniqueidentifier] NOT NULL, ";
                sql += "[CERTIFICATEGUID] [uniqueidentifier] NOT NULL, ";
                sql += "[DATA_TYPE] [nvarchar](50) NOT NULL, ";
                sql += "[DATA1] [nvarchar](500) NULL, ";
                sql += "[DATA2] [nvarchar](500) NULL, ";
                sql += "[DATA3] [nvarchar](500) NULL, ";
                sql += "[CREATED] [datetime] NOT NULL, ";
                sql += "[CREATEDBY] [uniqueidentifier] NOT NULL, ";
                sql += "[UPDATED] [datetime] NULL, ";
                sql += "[UPDATEDBY] [uniqueidentifier] NULL, ";
                sql += "[DELETED] [datetime] NULL, ";
                sql += "[DELETEDBY] [uniqueidentifier] NULL, ";
                sql += "CONSTRAINT [PK_CERTIFICATE_DATA] PRIMARY KEY CLUSTERED  ";
                sql += "( ";
                sql += "[GUID] ASC ";
                sql += ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ";
                sql += ") ON [PRIMARY] ";
                sql += "END ";
                success = createDatabaseTable(sql, "CERTIFICATE_DATA");
                if (!success)
                    return false;

                sql = "IF (NOT EXISTS (SELECT *  ";
                sql += "FROM INFORMATION_SCHEMA.TABLES  ";
                sql += "WHERE TABLE_SCHEMA = 'dbo'  ";
                sql += "AND  TABLE_NAME = 'CERTIFICATE_STATUS')) ";
                sql += "BEGIN ";
                sql += "SET ANSI_NULLS ON ";
                sql += "SET QUOTED_IDENTIFIER ON ";
                sql += "CREATE TABLE [dbo].[CERTIFICATE_STATUS]( ";
                sql += "[GUID] [uniqueidentifier] NOT NULL, ";
                sql += "[CERTIFICATE_MAIN_GUID] [uniqueidentifier] NOT NULL, ";
                sql += "[SEQUENCE_NUMBER] [numeric](10, 0) NOT NULL, ";
                sql += "[STATUS_NUMBER] [int] NOT NULL, ";
                sql += "[CREATED] [datetime] NOT NULL, ";
                sql += "[CREATEDBY] [uniqueidentifier] NOT NULL, ";
                sql += "[DELETED] [datetime] NULL, ";
                sql += "[DELETEDBY] [uniqueidentifier] NULL, ";
                sql += "CONSTRAINT [PK_CERTIFICATE_STATUS] PRIMARY KEY CLUSTERED  ";
                sql += "( ";
                sql += "[GUID] ASC ";
                sql += ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ";
                sql += ") ON [PRIMARY] ";
                sql += "END ";
                success = createDatabaseTable(sql, "CERTIFICATE_STATUS");
                if (!success)
                    return false;

                sql = "IF (NOT EXISTS (SELECT *  ";
                sql += "FROM INFORMATION_SCHEMA.TABLES  ";
                sql += "WHERE TABLE_SCHEMA = 'dbo'  ";
                sql += "AND  TABLE_NAME = 'CERTIFICATE_STATUS_ISSUE')) ";
                sql += "BEGIN ";
                sql += "SET ANSI_NULLS ON ";
                sql += "SET QUOTED_IDENTIFIER ON ";
                sql += "CREATE TABLE [dbo].[CERTIFICATE_STATUS_ISSUE]( ";
                sql += "[GUID] [uniqueidentifier] NOT NULL, ";
                sql += "[CERTIFICATE_STATUS_GUID] [uniqueidentifier] NOT NULL, ";
                sql += "[SEQUENCE_NUMBER] [numeric](10, 0) NOT NULL, ";
                sql += "[COMMENTS] [nvarchar](500) NOT NULL, ";
                sql += "[COMMENTS_READ] [bit] NOT NULL, ";
                sql += "[REJECTION] [bit] NOT NULL, ";
                sql += "[CREATED] [datetime] NOT NULL, ";
                sql += "[CREATEDBY] [uniqueidentifier] NOT NULL, ";
                sql += "CONSTRAINT [PK_CERTIFICATE_STATUS_ISSUE] PRIMARY KEY CLUSTERED  ";
                sql += "( ";
                sql += "[GUID] ASC ";
                sql += ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ";
                sql += ") ON [PRIMARY] ";
                sql += "END ";
                success = createDatabaseTable(sql, "CERTIFICATE_STATUS_ISSUE");
                if (!success)
                    return false;

                sql = "SET ANSI_NULLS ON ";
                sql += "SET QUOTED_IDENTIFIER ON ";
                sql += "CREATE TABLE [dbo].[PUNCHLIST_MAIN_PICTURE]( ";
                sql += "[GUID] [uniqueidentifier] NOT NULL, ";
                sql += "[PUNCHLIST_MAIN_GUID] [uniqueidentifier] NOT NULL, ";
                sql += "[PICTURE] [varbinary](max) NOT NULL, ";
                sql += "[PICTURE_TYPE] [int] NOT NULL, ";
                sql += "[CREATED] [datetime] NOT NULL, ";
                sql += "[CREATEDBY] [uniqueidentifier] NOT NULL, ";
                sql += "[UPDATED] [datetime] NULL, ";
                sql += "[UPDATEDBY] [uniqueidentifier] NULL, ";
                sql += "[DELETED] [datetime] NULL, ";
                sql += "[DELETEDBY] [uniqueidentifier] NULL, ";
                sql += "CONSTRAINT [PK_PUNCHLIST_MAIN_PICTURE] PRIMARY KEY CLUSTERED  ";
                sql += "( ";
                sql += "[GUID] ASC ";
                sql += ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ";
                sql += ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";
                success = createDatabaseTable(sql, "PUNCHLIST_MAIN_PICTURE");
                if (!success)
                    return false;

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool createDatabaseTable(string sql, string tableName)
        {
            //Common.SetConnStrFromXML() must be run for SQLBase to work
            SQLBase sqlBase = new SQLBase();
            sqlBase.ExecuteNonQuery(sql);
            if (!checkTableExist(tableName))
            {
                Common.Warn("Database table creation failed for " + tableName + ", please contact IT support");
                return false;
            }

            return true;
        }

        private bool checkTableExist(string tableName)
        {
            //Common.SetConnStrFromXML() must be run for SQLBase to work
            SQLBase sqlBase = new SQLBase();
            string sql = "select case when exists((select * from information_schema.tables where table_name = '" + tableName + "')) then 1 else 0 end";
            return sqlBase.ExecuteBoolQuery(sql);
        }
        #endregion
    }
}