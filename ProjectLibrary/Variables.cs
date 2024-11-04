using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ProjectLibrary
{
    public static class Variables
    {
        private static string _connStr;
        /// <summary>
        /// Default connection string for test purpose
        /// </summary>
        public static string ConnStr
        {
            get
            {
                return _connStr;
                //return @"Data Source=PG-TB002\SQLEXPRESS;Initial Catalog=CHECKMATE;Persist Security Info=False;User ID=GuestUser01;Password=Mynpw4PG";
                //return @"Data Source=TERMINAL\SQLEXTERNAL;Initial Catalog=CHECKMATE;Persist Security Info=False;User ID=GuestUser01;Password=Mynpw4PG";
                //return @"Data Source=PG-ZOTAC-006\SQLEXPRESS;Initial Catalog=CHECKMATE;Persist Security Info=False;User ID=GuestUser01;Password=Mynpw4PG";
                //return @"Server=tcp:uz4eohctvx.database.windows.net,1433;Database=CHECKMATE;User ID=SQLAdmin@uz4eohctvx;Password=GuestUser01;Trusted_Connection=False;Encrypt=True;Connection Timeout=30";
            }
        }

        public static void SetConnStr(string connStr)
        {
            _connStr = connStr;
        }

        public static string defaultCheckmateDirectory
        {
            get { return "CHECKMATE"; }
        }

        public static string databaseXMLFilename
        {
            get { return "Database.config"; }
        }

        public static string databaseXMLFilename2
        {
            get { return "Database2.config"; }
        }

        public static string defaultSuperadminUsername
        {
            get { return "superadmin"; }
        }

        public static string defaultSuperadminPassword
        {
            get { return "Hw&wm2i2NqQEqS"; }
        }

        public static string defaultUserCompany
        {
            get { return "Primero Group Pty Ltd"; }
        }

        /// <summary>
        /// Key for encrypt/decrypt password in dataase
        /// </summary>
        public static string securityKey
        {
            get{ return "dWwshVZ1EEs="; }
        }

        /// <summary>
        /// DataGrid representation when default project is not set
        /// </summary>
        public static string noProject
        {
            get { return "<No Projects>"; }
        }

        /// <summary>
        /// DataGrid representation when default project is not set and user is superuser
        /// </summary>
        public static string allProject
        {
            get { return "<All Projects>"; }
        }

        /// <summary>
        /// DataGrid representation when default discipline is not set
        /// </summary>
        public static string noDiscipline
        {
            get { return "<No Discipline>"; }
        }

        /// <summary>
        /// DataGrid representation when default discipline is not set and user is superuser
        /// </summary>
        public static string allDiscipline
        {
            get { return "<All Disciplines>"; }
        }

        /// <summary>
        /// DataGrid representation when default category is not set and user is superuser
        /// </summary>
        public static string allCategories
        {
            get { return "<All Priorities>"; }
        }

        /// <summary>
        /// DataGrid representation when default workflow is not set and user is superuser
        /// </summary>
        public static string allWorkflow
        {
            get { return "<All Stages>"; }
        }

        /// <summary>
        /// Combobox representation for values that are not assigned
        /// </summary>
        public static string SelectOne
        {
            get { return "-- Select One --"; }
        }

        /// <summary>
        /// Default password when creating new user
        /// </summary>
        public static string defaultPassword
        {
            get { return "abc123"; }
        }

        /// <summary>
        /// Admin Superuser Name
        /// </summary>
        public static string AdminSuperuser
        {
            get { return "<Admin Superuser>"; }
        }

        /// <summary>
        /// Template header text to indicate that we want to add a new header
        /// </summary>
        public static string TemplateHeader
        {
            get { return "<Header Template>"; }
        }

        /// <summary>
        /// Root workflow text
        /// </summary>
        public static string RootWorkflow
        {
            get { return "<Root>"; }
        }

        /// <summary>
        /// Root WBS text
        /// </summary>
        public static string RootWBS
        {
            get { return "<Root>"; }
        }

        /// <summary>
        /// Root tag text
        /// </summary>
        public static string RootTag
        {
            get { return "<Root>"; }
        }

        public static string Header_TagWBS
        {
            get { return "Tag/WBS Number"; }
        }

        public static string Empty_Prefill
        {
            get { return ""; }
            //get { return "N/A"; }
        }

        public static string General_NotApplicable
        {
            get { return "N/A"; }
        }

        public static string Multiple_Prefill
        {
            get { return "Multiple"; }
        }

        public static string Unknown_User
        {
            get { return "Unknown"; }
        }

        public static string Superadmin_User
        {
            get { return "Superadmin"; }
        }

        public static string prefillTagNumber
        {
            get { return "Tag Number"; }
        }

        public static string prefillTagDescription
        {
            get { return "Tag Description"; }
        }

        public static string prefillProjNumber
        {
            get { return "Project Number"; }
        }

        public static string prefillProjName
        {
            get { return "Project Name"; }
        }

        public static string prefillProjClient
        {
            get { return "Project Client"; }
        }

        public static string prefillDocumentName
        {
            get { return "Document Name"; }
        }

        public static string prefillAreaNumber
        {
            get { return "Area Number"; }
        }

        public static string prefillAreaDescription
        {
            get { return "Area Description"; }
        }

        public static string prefillSystemNumber
        {
            get { return "System Number"; }
        }

        public static string prefillSystemDescription
        {
            get { return "System Description"; }
        }

        public static string prefillSubSystemNumber
        {
            get { return "Sub System Number"; }
        }

        public static string prefillSubSystemDescription
        {
            get { return "Sub System Description"; }
        }

        public static string prefillTaskNumber
        {
            get { return "Task Number"; }
        }

        public static string prefillDate
        {
            get { return "Date"; }
        }

        public static string prefillDateTime
        {
            get { return "DateTime"; }
        }

        public static string prefillChild
        {
            get { return "Child Tags"; }
        }

        public static string punchlistSelectWBSTag
        {
            get { return "Please select WBS/TAG"; }
        }

        public static string punchlistAdhoc
        {
            get { return "Ad-Hoc"; }
        }

        public static string punchlistNotAvailable
        {
            get { return "ITR Punchlist(s) Not Required/Complete";  }
        }

        public static string punchlistAffix
        {
            get { return "_"; }
        }

        public static string punchlistCategoryA
        {
            get { return "A - Equipment or Safety Critical"; }
        }

        public static string punchlistCategoryB
        {
            get { return "B - Mechanical/Practical Completion Item";  }
        }

        public static string punchlistCategoryC
        {
            get { return "C - Low Priority"; }
        }

        public static string punchlistCategoryD
        {
            get { return "D - Out of scope"; }
        }
        /// <summary>
        /// Act as a general discipline for prefill
        /// </summary>
        public static string prefillGeneral
        {
            get { return "General"; }
        }

        public static string punchlistTemplateName
        {
            get { return "Punchlist"; }
        }

        public static string punchlistWalkdownTemplateName
        {
            get { return "PunchlistWalkdown"; }
        }

        public static string constructionVerificationCertificateTemplateName
        {
            get { return "ConstructionVerificationCertificate"; }
        }

        public static string noticeOfEnergisationCertificateTemplateName
        {
            get { return "NoticeOfEnergisation"; }
        }

        public static string timeStringFormat
        {
            get { return "HH:mm"; }
        }

        public static string dateTimeStringFormat
        {
            get { return "HH:mm ddd, MMM d, yy"; }
        }

        public static string dateStringFormat
        {
            get { return "dd-MMM-yy"; }
        }

        public static DateTime Conflict_Date
        {
            get { return new DateTime(9999, 12, 31); }
        }

        public static char delimiter
        {
            get { return '|'; }
        }

        public static string punchlistStatusDelimiter
        {
            get { return Environment.NewLine.ToString();  }
        }

        public static int QRCodeMaxWidth
        {
            get { return 500; }
        }

        public static int RichEditHeaderOffset
        {
            get { return 100; }
        }

        public static string prefillCertificateNumber
        {
            get { return "Certificate Number"; }
        }

        public static string prefillCertificateDescription
        {
            get { return "Certificate Description"; }
        }

        public static string Certificate_DataType_Subsystem
        {
            get { return "Subsystem"; }
        }

        public static string Certificate_DataType_Discipline
        {
            get { return "Discipline"; }
        }

        public static string Certificate_DataType_CVC_Type
        {
            get { return "CVC_Type"; }
        }

        public static string Select_Subsystem_String
        {
            get { return "Click Here to Select Subsystem. Click on Subsystem to Remove it"; }
        }

        public static string Select_CVC_Type
        {
            get { return "Click Here to Select CVC Type"; }
        }

        public static string Select_Tag
        {
            get { return "Click Here to Select Tag"; }
        }

        public static string ITR_Completion_Prefix
        {
            get { return "ITR_COMPLETION_"; }
        }

        public static string Discipline_Toggle_Prefix
        {
            get { return "DISCIPLINE_TOGGLE"; }
        }

        public enum CertificateReportType { PunchlistWalkdown, CVCMasterReport, NOEReport, DisciplineMasterReport, MatrixAssignment }
    }
}