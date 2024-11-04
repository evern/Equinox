using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectLibrary;
using ProjectDatabase.Dataset;
using ProjectDatabase.Dataset.dsCERTIFICATE_MAINTableAdapters;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDatabase.DataAdapters
{
    public class AdapterCERTIFICATE_MAIN : SQLBase, IDisposable
    {
        private CERTIFICATE_MAINTableAdapter _adapter;

        public AdapterCERTIFICATE_MAIN()
            : base()
        {
            _adapter = new CERTIFICATE_MAINTableAdapter(Variables.ConnStr);
        }

        public AdapterCERTIFICATE_MAIN(string connStr)
            : base(connStr)
        {
            _adapter = new CERTIFICATE_MAINTableAdapter(connStr);
        }

        public void Set_Update_Event(SqlRowUpdatedEventHandler RowUpdateEvent)
        {
            _adapter.Adapter.RowUpdated += new SqlRowUpdatedEventHandler(RowUpdateEvent);
        }

        #region Main
        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable Get()
        {
            string sql = "SELECT * FROM CERTIFICATE_MAIN WHERE DELETED IS NULL";
            dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtCERTIFICATE_MAIN = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_MAIN);

            if (dtCERTIFICATE_MAIN.Rows.Count > 0)
                return dtCERTIFICATE_MAIN;
            else
                return null;
        }

        public List<T> GetCertificate<T>(Guid projectGuid, string templateName)
            where T : ICertificate, new()
        {
            string sql = "SELECT cMain.*, cStatus.STATUS_NUMBER FROM CERTIFICATE_MAIN cMain ";
            sql += "LEFT JOIN CERTIFICATE_STATUS cStatus ON cStatus.CERTIFICATE_MAIN_GUID = cMain.GUID ";
            sql += "OUTER APPLY ";
            sql += "(SELECT MAX(cStatusLatest.SEQUENCE_NUMBER) AS LatestSeq FROM CERTIFICATE_STATUS cStatusLatest WHERE cStatusLatest.CERTIFICATE_MAIN_GUID = cMain.GUID) LatestCertificateSequence ";
            sql += "WHERE cMain.TEMPLATE_NAME = '" + templateName + "' AND cMain.PROJECTGUID = '" + projectGuid + "' AND cMain.DELETED IS NULL AND cSTATUS.DELETED IS NULL AND (cStatus.SEQUENCE_NUMBER IS NULL OR cStatus.SEQUENCE_NUMBER = LatestCertificateSequence.LatestSeq)";

            dsCERTIFICATE_MAIN.CERTIFICATE_MAIN_STATUSDataTable dtCERTIFICATE_MAIN = new dsCERTIFICATE_MAIN.CERTIFICATE_MAIN_STATUSDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_MAIN);

            string sqlCertificateData = "SELECT CERTIFICATE_DATA.* FROM CERTIFICATE_DATA ";
            sqlCertificateData += "JOIN CERTIFICATE_MAIN ON CERTIFICATE_MAIN.GUID = CERTIFICATE_DATA.CERTIFICATEGUID ";
            sqlCertificateData += "WHERE CERTIFICATE_MAIN.PROJECTGUID = '" + projectGuid + "' AND CERTIFICATE_MAIN.DELETED IS NULL AND CERTIFICATE_DATA.DELETED IS NULL";
            dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable dtCERTIFICATE_DATA = new dsCERTIFICATE_DATA.CERTIFICATE_DATADataTable();
            ExecuteQuery(sqlCertificateData, dtCERTIFICATE_DATA);

            List<T> certificates = new List<T>();
            if (dtCERTIFICATE_MAIN.Rows.Count > 0)
            {
                foreach(dsCERTIFICATE_MAIN.CERTIFICATE_MAIN_STATUSRow drCERTIFICATE_MAIN in dtCERTIFICATE_MAIN.Rows)
                {
                    var CERTIFICATE_DATARows = dtCERTIFICATE_DATA.Where(x => x.CERTIFICATEGUID == drCERTIFICATE_MAIN.GUID);
                    List<string> disciplines = CERTIFICATE_DATARows.Where(x => x.DATA_TYPE == Variables.Certificate_DataType_Discipline).Select(x => x.DATA1).ToList();
                    List<string> subsystems = CERTIFICATE_DATARows.Where(x => x.DATA_TYPE == Variables.Certificate_DataType_Subsystem).Select(x => x.DATA1).ToList();

                    T newCertificate = new T()
                    {
                        GUID = drCERTIFICATE_MAIN.GUID,
                        Number = drCERTIFICATE_MAIN.NUMBER,
                        Description = drCERTIFICATE_MAIN.DESCRIPTION,
                        Disciplines = disciplines,
                        Subsystems = subsystems
                    };

                    ViewModel_CVC cvcViewModel = newCertificate as ViewModel_CVC;
                    if(cvcViewModel != null)
                    {
                        dsCERTIFICATE_DATA.CERTIFICATE_DATARow drCERTIFICATE_MAIN_CVC_TYPE = CERTIFICATE_DATARows.FirstOrDefault(x => x.DATA_TYPE == Variables.Certificate_DataType_CVC_Type);
                        if (drCERTIFICATE_MAIN_CVC_TYPE != null)
                            cvcViewModel.CVC_Type = drCERTIFICATE_MAIN_CVC_TYPE.DATA1;

                        cvcViewModel.Status = drCERTIFICATE_MAIN.IsSTATUS_NUMBERNull() ? CVC_Status.Pending.ToString() : ((CVC_Status)drCERTIFICATE_MAIN.STATUS_NUMBER).ToString();
                        cvcViewModel.StatusNumber = drCERTIFICATE_MAIN.IsSTATUS_NUMBERNull() ? 0 : (int)drCERTIFICATE_MAIN.STATUS_NUMBER;
                    }

                    ViewModel_NOE noeViewModel = newCertificate as ViewModel_NOE;
                    if(noeViewModel != null)
                    {
                        noeViewModel.Status = drCERTIFICATE_MAIN.IsSTATUS_NUMBERNull() ? NOE_Status.Pending.ToString() : ((NOE_Status)drCERTIFICATE_MAIN.STATUS_NUMBER).ToString();
                        noeViewModel.StatusNumber = drCERTIFICATE_MAIN.IsSTATUS_NUMBERNull() ? 0 : (int)drCERTIFICATE_MAIN.STATUS_NUMBER;
                    }

                    ViewModel_PunchlistWalkdown punchlistWalkdownViewModel = newCertificate as ViewModel_PunchlistWalkdown;
                    if(punchlistWalkdownViewModel != null)
                    {
                        punchlistWalkdownViewModel.Status = drCERTIFICATE_MAIN.IsSTATUS_NUMBERNull() ? PunchlistWalkdown_Status.Pending.ToString() : ((PunchlistWalkdown_Status)drCERTIFICATE_MAIN.STATUS_NUMBER).ToString();
                        punchlistWalkdownViewModel.StatusNumber = drCERTIFICATE_MAIN.IsSTATUS_NUMBERNull() ? 0 : (int)drCERTIFICATE_MAIN.STATUS_NUMBER;
                    }

                    certificates.Add(newCertificate);
                }

                return certificates;
            }
            else
                return new List<T>();
        }

        /// <summary>
        /// Get deleted prefill for purging
        /// </summary>
        public dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable Get_Deleted(string discipline)
        {
            string sql = "SELECT * FROM CERTIFICATE_MAIN WHERE DELETED IS NOT NULL AND DISCIPLINE = '" + discipline + "'";
            dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtCERTIFICATE_MAIN = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_MAIN);

            if (dtCERTIFICATE_MAIN.Rows.Count > 0)
                return dtCERTIFICATE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable GetIncludeDeleted()
        {
            string sql = "SELECT * FROM CERTIFICATE_MAIN";
            dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtCERTIFICATE_MAIN = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_MAIN);

            if (dtCERTIFICATE_MAIN.Rows.Count > 0)
                return dtCERTIFICATE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get all header in the system
        /// </summary>
        public dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow GetIncludeDeletedBy(Guid prefillGuid)
        {
            string sql = "SELECT * FROM CERTIFICATE_MAIN WHERE GUID = '" + prefillGuid + "'";
            dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtCERTIFICATE_MAIN = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_MAIN);

            if (dtCERTIFICATE_MAIN.Rows.Count > 0)
                return dtCERTIFICATE_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Get header by discipline
        /// </summary>
        public dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable GetByDiscipline(string discipline)
        {
            string sql = "SELECT * FROM CERTIFICATE_MAIN WHERE DISCIPLINE = '" + discipline + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtCERTIFICATE_MAIN = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_MAIN);

            if (dtCERTIFICATE_MAIN.Rows.Count > 0)
                return dtCERTIFICATE_MAIN;
            else
                return null;
        }

        /// <summary>
        /// Get header by guid
        /// </summary>
        public dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow GetBy(Guid headerGuid)
        {
            string sql = "SELECT * FROM CERTIFICATE_MAIN WHERE GUID = '" + headerGuid + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtCERTIFICATE_MAIN = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_MAIN);

            if (dtCERTIFICATE_MAIN.Rows.Count > 0)
                return dtCERTIFICATE_MAIN[0];
            else
                return null;
        }


        /// <summary>
        /// Get header by guid
        /// </summary>
        public string GetNextSequence(Guid _projectGuid, string partialShortCode)
        {
            string sql = "SELECT TOP 1 * FROM CERTIFICATE_MAIN WHERE PROJECTGUID = '" + _projectGuid + "' AND NUMBER LIKE '%" + partialShortCode + "%'" + " AND DELETED IS NULL";
            sql += " ORDER BY NUMBER DESC";

            dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtCERTIFICATE_MAIN = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_MAIN);

            if (dtCERTIFICATE_MAIN.Rows.Count > 0)
                return Helpers.GetNextSequenceNumber(dtCERTIFICATE_MAIN[0].NUMBER);
            else
                return null;
        }

        /// <summary>
        /// Get header by name
        /// </summary>
        public dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow GetBy(string name)
        {
            string sql = "SELECT * FROM CERTIFICATE_MAIN WHERE NAME = '" + name + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtCERTIFICATE_MAIN = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable();
            ExecuteQuery(sql, dtCERTIFICATE_MAIN);

            if (dtCERTIFICATE_MAIN.Rows.Count > 0)
                return dtCERTIFICATE_MAIN[0];
            else
                return null;
        }

        /// <summary>
        /// Remove a particular header by GUID
        /// </summary>
        public bool RemoveBy(Guid guid)
        {
            string sql = "SELECT * FROM CERTIFICATE_MAIN WHERE GUID = '" + guid + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtHeader = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable();
            ExecuteQuery(sql, dtHeader);

            if (dtHeader != null)
            {
                dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow drHeader = dtHeader[0];
                drHeader.DELETED = DateTime.Now;
                drHeader.DELETEDBY = System_Environment.GetUser().GUID;
                Save(drHeader);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove a particular header by GUID
        /// </summary>
        public int RemoveWithExclusionBy(string prefillName, Guid excludedGuid)
        {
            string sql = "SELECT * FROM CERTIFICATE_MAIN WHERE NAME = '" + prefillName + "'";
            sql += " AND GUID IS NOT '" + excludedGuid + "'";
            sql += " AND DELETED IS NULL";

            dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtPrefill = new dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable();
            ExecuteQuery(sql, dtPrefill);

            int removeCount = 0;
            if (dtPrefill != null)
            {
                foreach (dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow drPrefill in dtPrefill.Rows)
                {
                    drPrefill.DELETED = DateTime.Now;
                    drPrefill.DELETEDBY = System_Environment.GetUser().GUID;
                    Save(drPrefill);
                    removeCount++;
                }
            }

            return removeCount;
        }

        public dsCERTIFICATE_MAIN.CertificateReportDataTable GetCertificateReport(Guid projectGuid, string templateName)
        {
            string sql = "SELECT * ";
            sql += "FROM ";
            sql += "( ";
            sql += "SELECT Subsystem, SubsystemDesc, CertificateNumber, CertificateDiscipline, StatusNumber ";
            sql += "FROM ";
            sql += "( ";
            sql += "SELECT SubsystemGuid, SUBSYSTEM, SUBSYSTEMDESC FROM ";
            sql += "( ";
            sql += "SELECT  ";
            sql += "Area = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN ThirdWBS.NAME ELSE SecondWBS.NAME END ELSE FirstWBS.NAME END, ";
            sql += "AreaDesc =CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN ThirdWBS.DESCRIPTION ELSE SecondWBS.DESCRIPTION END ELSE FirstWBS.DESCRIPTION END, ";
            sql += "System = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END ELSE SecondWBS.NAME END, ";
            sql += "SystemDesc = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END ELSE SecondWBS.DESCRIPTION END, ";
            sql += "Subsystem = CASE WHEN FirstWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END, ";
            sql += "SubsystemDesc = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END, ";
            sql += "SubsystemGuid = CASE WHEN FirstWBS.GUID IS NULL THEN NULL ELSE ThirdWBS.GUID END ";
            sql += "FROM PROJECT Project ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.PROJECTGUID = Project.GUID) ";
            sql += "JOIN WBS ThirdWBS ON (Schedule.GUID = ThirdWBS.SCHEDULEGUID) ";
            sql += "LEFT JOIN WBS SecondWBS ON (SecondWBS.GUID = ThirdWBS.PARENTGUID)   ";
            sql += "LEFT JOIN WBS FirstWBS ON (FirstWBS.GUID = SecondWBS.PARENTGUID)   ";
            sql += "WHERE Project.GUID = '" + projectGuid + "' AND Schedule.DELETED IS NULL AND Project.DELETED IS NULL AND ThirdWBS.DELETED IS NULL AND SecondWBS.DELETED IS NULL AND FirstWBS.DELETED IS NULL ";
            sql += ") WBSQuery GROUP BY SUBSYSTEM, SUBSYSTEMDESC, SubsystemGuid ";
            sql += ") SubsystemQuery  ";
            sql += "OUTER APPLY ";
            sql += "( ";
            sql += "SELECT CertificateDiscipline = DisciplineData.DATA1, CertificateNumber = CERTIFICATE_MAIN.NUMBER, StatusNumber = MAX(LatestCertificateStatus.STATUS_NUMBER) FROM CERTIFICATE_MAIN  ";
            sql += "JOIN (SELECT * FROM CERTIFICATE_DATA WHERE DATA_TYPE = 'Subsystem' AND DELETED IS NULL) SubsystemData ON CERTIFICATE_MAIN.GUID = SubsystemData.CERTIFICATEGUID ";
            sql += "JOIN (SELECT * FROM CERTIFICATE_DATA WHERE DATA_TYPE = 'Discipline' AND DELETED IS NULL) DisciplineData ON CERTIFICATE_MAIN.GUID = DisciplineData.CERTIFICATEGUID ";
            sql += "OUTER APPLY   ";
            sql += "( ";
            sql += "SELECT TOP 1 * FROM CERTIFICATE_STATUS WHERE CERTIFICATE_STATUS.CERTIFICATE_MAIN_GUID = CERTIFICATE_MAIN.GUID AND (CERTIFICATE_STATUS.SEQUENCE_NUMBER = (SELECT MAX(cStatus2.SEQUENCE_NUMBER) FROM CERTIFICATE_STATUS cStatus2 WHERE cStatus2.CERTIFICATE_MAIN_GUID = CERTIFICATE_MAIN.GUID) OR CERTIFICATE_STATUS.SEQUENCE_NUMBER IS NULL)  ";
            sql += "AND CERTIFICATE_STATUS.DELETED IS NULL ";
            sql += ") LatestCertificateStatus   ";
            sql += "WHERE SubsystemData.DATA1 = SubsystemQuery.Subsystem AND TEMPLATE_NAME = '" + templateName + "' AND CERTIFICATE_MAIN.DELETED IS NULL  ";
            sql += "GROUP BY DisciplineData.DATA1, CERTIFICATE_MAIN.NUMBER ";
            sql += ") CertificateQuery ";
            sql += "WHERE SubsystemQuery.Subsystem IS NOT NULL ";
            sql += ") CascadeQuery ORDER BY Subsystem, StatusNumber DESC, CertificateNumber DESC ";

            string sqlDiscipline = "SELECT * FROM ";
            sqlDiscipline += "( ";
            sqlDiscipline += "SELECT SubsystemName = WBS.NAME, TemplateDiscipline = TEMPLATE_MAIN.DISCIPLINE, TagNumber = TAG.NUMBER,  ";
            sqlDiscipline += "ROW_NUMBER() OVER ( ";
            sqlDiscipline += "PARTITION BY WBS.NAME, TEMPLATE_MAIN.DISCIPLINE ";
            sqlDiscipline += "ORDER BY  TAG.NUMBER ";
            sqlDiscipline += ") row_num ";
            sqlDiscipline += "FROM TAG  JOIN SCHEDULE ON TAG.SCHEDULEGUID = SCHEDULE.GUID  JOIN TEMPLATE_REGISTER ON TEMPLATE_REGISTER.TAG_GUID = TAG.GUID JOIN TEMPLATE_MAIN ON TEMPLATE_MAIN.GUID = TEMPLATE_REGISTER.TEMPLATE_GUID JOIN WBS ON WBS.GUID = TAG.PARENTGUID ";
            sqlDiscipline += "WHERE SCHEDULE.PROJECTGUID = '" + projectGuid + "' ";
            sqlDiscipline += ") TblQuery WHERE row_num = 1 ";

            dsCERTIFICATE_MAIN.CertificateReportDataTable dtCertificateReport = new dsCERTIFICATE_MAIN.CertificateReportDataTable();
            dsCERTIFICATE_MAIN.SubsystemCertificateDataTable dtSubsystemCertificate = new dsCERTIFICATE_MAIN.SubsystemCertificateDataTable();
            ExecuteQuery(sql, dtSubsystemCertificate);

            dsCERTIFICATE_MAIN.AssetsDataTable dtAsset = new dsCERTIFICATE_MAIN.AssetsDataTable();
            ExecuteQuery(sqlDiscipline, dtAsset);

            //determine status type from template name
            Type statusType;
            if (templateName == Variables.punchlistWalkdownTemplateName)
                statusType = typeof(PunchlistWalkdown_Status);
            else
                statusType = typeof(CVC_Status);

            if (dtSubsystemCertificate.Rows.Count > 0)
            {
                IEnumerable<dsCERTIFICATE_MAIN.SubsystemCertificateRow> subsystemCertificateRows = dtSubsystemCertificate.AsEnumerable();
                HashSet<string> subsystems = new HashSet<string>(subsystemCertificateRows.Select(x => x.Subsystem));
                string s;
                IEnumerable<dsCERTIFICATE_MAIN.AssetsRow> disciplineSequence = dtAsset.AsEnumerable();
                foreach(string subsystem in subsystems)
                {
                    if (subsystem == "151-20-31")
                        s = string.Empty;

                    List<dsCERTIFICATE_MAIN.SubsystemCertificateRow> currentSubsystems = subsystemCertificateRows.Where(row => row.Subsystem == subsystem).ToList();
                    dsCERTIFICATE_MAIN.CertificateReportRow drCertificateReport = dtCertificateReport.NewCertificateReportRow();
                    drCertificateReport.Subsystem = subsystem;
                    drCertificateReport.SubsystemDesc = currentSubsystems.First().SubsystemDesc;
                    populateCertificateDisciplineStatus(drCertificateReport, currentSubsystems, disciplineSequence, subsystem, Discipline.Mechanical.ToString(), statusType);
                    populateCertificateDisciplineStatus(drCertificateReport, currentSubsystems, disciplineSequence, subsystem, Discipline.Electrical.ToString(), statusType);
                    populateCertificateDisciplineStatus(drCertificateReport, currentSubsystems, disciplineSequence, subsystem, Discipline.Instrumentation.ToString(), statusType);
                    populateCertificateDisciplineStatus(drCertificateReport, currentSubsystems, disciplineSequence, subsystem, Discipline.Piping.ToString(), statusType);
                    populateCertificateDisciplineStatus(drCertificateReport, currentSubsystems, disciplineSequence, subsystem, Discipline.Structural.ToString(), statusType);
                    populateCertificateDisciplineStatus(drCertificateReport, currentSubsystems, disciplineSequence, subsystem, Discipline.Architectural.ToString(), statusType);
                    populateCertificateDisciplineStatus(drCertificateReport, currentSubsystems, disciplineSequence, subsystem, Discipline.Civil.ToString(), statusType);
                    populateCertificateDisciplineStatus(drCertificateReport, currentSubsystems, disciplineSequence, subsystem, Discipline.Others.ToString(), statusType);
                    dtCertificateReport.AddCertificateReportRow(drCertificateReport);
                }
            }

            return dtCertificateReport;
        }

        public dsCERTIFICATE_MAIN.NOE_ReportDataTable GetNOEReport(Guid projectGuid)
        {
            string sql = "SELECT * ";
            sql += "FROM ";
            sql += "( ";
            sql += "SELECT Subsystem, SubsystemDesc, CertificateNumber, CertificateDescription, StatusNumber, ROW_NUMBER() OVER(PARTITION BY Subsystem ORDER BY StatusNumber DESC) SEQ ";
            sql += "FROM ";
            sql += "( ";
            sql += "SELECT SubsystemGuid, SUBSYSTEM, SUBSYSTEMDESC FROM ";
            sql += "( ";
            sql += "SELECT  ";
            sql += "Area = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN ThirdWBS.NAME ELSE SecondWBS.NAME END ELSE FirstWBS.NAME END, ";
            sql += "AreaDesc =CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN ThirdWBS.DESCRIPTION ELSE SecondWBS.DESCRIPTION END ELSE FirstWBS.DESCRIPTION END, ";
            sql += "System = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END ELSE SecondWBS.NAME END, ";
            sql += "SystemDesc = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END ELSE SecondWBS.DESCRIPTION END, ";
            sql += "Subsystem = CASE WHEN FirstWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END, ";
            sql += "SubsystemDesc = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END, ";
            sql += "SubsystemGuid = CASE WHEN FirstWBS.GUID IS NULL THEN NULL ELSE ThirdWBS.GUID END ";
            sql += "FROM PROJECT Project ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.PROJECTGUID = Project.GUID) ";
            sql += "JOIN WBS ThirdWBS ON (Schedule.GUID = ThirdWBS.SCHEDULEGUID) ";
            sql += "LEFT JOIN WBS SecondWBS ON (SecondWBS.GUID = ThirdWBS.PARENTGUID)   ";
            sql += "LEFT JOIN WBS FirstWBS ON (FirstWBS.GUID = SecondWBS.PARENTGUID)   ";
            sql += "WHERE Project.GUID = '" + projectGuid + "' AND Schedule.DELETED IS NULL AND Project.DELETED IS NULL AND ThirdWBS.DELETED IS NULL AND SecondWBS.DELETED IS NULL AND FirstWBS.DELETED IS NULL ";
            sql += ") WBSQuery GROUP BY SUBSYSTEM, SUBSYSTEMDESC, SubsystemGuid ";
            sql += ") SubsystemQuery  ";
            sql += "OUTER APPLY ";
            sql += "( ";
            sql += "SELECT CertificateNumber = CERTIFICATE_MAIN.NUMBER, CertificateDescription = CERTIFICATE_MAIN.DESCRIPTION, StatusNumber = MAX(LatestCertificateStatus.STATUS_NUMBER) ";
            sql += "FROM CERTIFICATE_MAIN  ";
            sql += "JOIN (SELECT * FROM CERTIFICATE_DATA WHERE DATA_TYPE = 'Subsystem' AND DELETED IS NULL) SubsystemData ON CERTIFICATE_MAIN.GUID = SubsystemData.CERTIFICATEGUID ";
            sql += "OUTER APPLY   ";
            sql += "( ";
            sql += "SELECT TOP 1 * FROM CERTIFICATE_STATUS WHERE CERTIFICATE_STATUS.CERTIFICATE_MAIN_GUID = CERTIFICATE_MAIN.GUID AND (CERTIFICATE_STATUS.SEQUENCE_NUMBER = (SELECT MAX(cStatus2.SEQUENCE_NUMBER) FROM CERTIFICATE_STATUS cStatus2 WHERE cStatus2.CERTIFICATE_MAIN_GUID = CERTIFICATE_MAIN.GUID) OR CERTIFICATE_STATUS.SEQUENCE_NUMBER IS NULL)  ";
            sql += "AND CERTIFICATE_STATUS.DELETED IS NULL ";
            sql += ") LatestCertificateStatus   ";
            sql += "WHERE SubsystemData.DATA1 = SubsystemQuery.Subsystem AND TEMPLATE_NAME = 'NoticeOfEnergisation' AND CERTIFICATE_MAIN.DELETED IS NULL  ";
            sql += "GROUP BY CERTIFICATE_MAIN.NUMBER, CERTIFICATE_MAIN.DESCRIPTION ";
            sql += ") CertificateQuery ";
            sql += "WHERE SubsystemQuery.Subsystem IS NOT NULL ";
            sql += ") CascadeQuery ORDER BY Subsystem, StatusNumber DESC, CertificateNumber DESC ";


            dsCERTIFICATE_MAIN.NOE_ReportDataTable dtNOEReport = new dsCERTIFICATE_MAIN.NOE_ReportDataTable();
            dsCERTIFICATE_MAIN.NOESubsystemCertificateDataTable dtSubsystemCertificate = new dsCERTIFICATE_MAIN.NOESubsystemCertificateDataTable();
            ExecuteQuery(sql, dtSubsystemCertificate);
            Type statusEnumType = typeof(NOE_Status);
            if (dtSubsystemCertificate.Rows.Count > 0)
            {
                IEnumerable<dsCERTIFICATE_MAIN.NOESubsystemCertificateRow> subsystemCertificateRows = dtSubsystemCertificate.AsEnumerable();
                HashSet<string> subsystems = new HashSet<string>(subsystemCertificateRows.Where(x => !x.IsSubsystemNull()).Select(x => x.Subsystem));

                foreach (string subsystem in subsystems)
                {
                    List<dsCERTIFICATE_MAIN.NOESubsystemCertificateRow> currentSubsystemCertificateRows = subsystemCertificateRows.Where(row => row.Subsystem == subsystem).ToList();
                    dsCERTIFICATE_MAIN.NOE_ReportRow drNOEReport = dtNOEReport.NewNOE_ReportRow();
                    drNOEReport.Subsystem = subsystem;
                    drNOEReport.SubsystemDesc = currentSubsystemCertificateRows.First().SubsystemDesc;
                    populateNOESequenceStatus(drNOEReport, currentSubsystemCertificateRows, subsystem, 1, statusEnumType);
                    populateNOESequenceStatus(drNOEReport, currentSubsystemCertificateRows, subsystem, 2, statusEnumType);
                    populateNOESequenceStatus(drNOEReport, currentSubsystemCertificateRows, subsystem, 3, statusEnumType);
                    dtNOEReport.AddNOE_ReportRow(drNOEReport);
                }
            }

            return dtNOEReport;
        }


        public dsCERTIFICATE_MAIN.DisciplineReportDataTable GetDisciplineReport(Guid projectGuid)
        {
            string sql = "SELECT * FROM ";
            sql += "( ";
            sql += "SELECT SUBSYSTEM, SUBSYSTEMDESC, SUBSYSTEMGUID FROM ";
            sql += "( ";
            sql += "SELECT  ";
            sql += "Area = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN ThirdWBS.NAME ELSE SecondWBS.NAME END ELSE FirstWBS.NAME END, ";
            sql += "AreaDesc =CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN ThirdWBS.DESCRIPTION ELSE SecondWBS.DESCRIPTION END ELSE FirstWBS.DESCRIPTION END, ";
            sql += "System = CASE WHEN FirstWBS.NAME IS NULL THEN CASE WHEN SecondWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END ELSE SecondWBS.NAME END, ";
            sql += "SystemDesc = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN CASE WHEN SecondWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END ELSE SecondWBS.DESCRIPTION END, ";
            sql += "Subsystem = CASE WHEN FirstWBS.NAME IS NULL THEN NULL ELSE ThirdWBS.NAME END, ";
            sql += "SubsystemDesc = CASE WHEN FirstWBS.DESCRIPTION IS NULL THEN NULL ELSE ThirdWBS.DESCRIPTION END, ";
            sql += "SubsystemGuid = CASE WHEN FirstWBS.GUID IS NULL THEN NULL ELSE ThirdWBS.GUID END ";
            sql += "FROM PROJECT Project ";
            sql += "JOIN SCHEDULE Schedule ON (Schedule.PROJECTGUID = Project.GUID) ";
            sql += "JOIN WBS ThirdWBS ON (Schedule.GUID = ThirdWBS.SCHEDULEGUID) ";
            sql += "LEFT JOIN WBS SecondWBS ON (SecondWBS.GUID = ThirdWBS.PARENTGUID)   ";
            sql += "LEFT JOIN WBS FirstWBS ON (FirstWBS.GUID = SecondWBS.PARENTGUID)   ";
            sql += "WHERE Project.GUID = '" + projectGuid + "' AND Schedule.DELETED IS NULL AND Project.DELETED IS NULL AND ThirdWBS.DELETED IS NULL AND SecondWBS.DELETED IS NULL AND FirstWBS.DELETED IS NULL ";
            sql += ") WBSGroupQuery GROUP BY SUBSYSTEM, SUBSYSTEMDESC, SUBSYSTEMGUID ";
            sql += ") RawSubsystemQuery WHERE SUBSYSTEM IS NOT NULL ";

            dsCERTIFICATE_MAIN.DisciplineReportDataTable dtDisciplineReport = new dsCERTIFICATE_MAIN.DisciplineReportDataTable();
            dsCERTIFICATE_MAIN.SubsystemsDataTable dtSubsystems = new dsCERTIFICATE_MAIN.SubsystemsDataTable();
            ExecuteQuery(sql, dtSubsystems);

            string sqlDiscipline = "SELECT * FROM  ";
            sqlDiscipline += "(  ";
            sqlDiscipline += "SELECT SubsystemName = WBS.NAME, TemplateDiscipline = TEMPLATE_MAIN.DISCIPLINE, TagNumber = TAG.NUMBER,   ";
            sqlDiscipline += "ROW_NUMBER() OVER ( PARTITION BY WBS.NAME, TEMPLATE_MAIN.DISCIPLINE ORDER BY  TAG.NUMBER ) row_num  ";
            sqlDiscipline += "FROM TAG   ";
            sqlDiscipline += "JOIN SCHEDULE ON TAG.SCHEDULEGUID = SCHEDULE.GUID   ";
            sqlDiscipline += "JOIN TEMPLATE_REGISTER ON TEMPLATE_REGISTER.TAG_GUID = TAG.GUID  ";
            sqlDiscipline += "JOIN TEMPLATE_MAIN ON TEMPLATE_MAIN.GUID = TEMPLATE_REGISTER.TEMPLATE_GUID  ";
            sqlDiscipline += "JOIN WBS ON WBS.GUID = TAG.PARENTGUID WHERE SCHEDULE.PROJECTGUID = '" + projectGuid + "' ";
            sqlDiscipline += ") TblQuery WHERE row_num = 1";

            dsCERTIFICATE_MAIN.AssetsDataTable dtAsset = new dsCERTIFICATE_MAIN.AssetsDataTable();
            ExecuteQuery(sqlDiscipline, dtAsset);

            if (dtSubsystems.Rows.Count > 0)
            {
                List<dsCERTIFICATE_MAIN.AssetsRow> assetRows = dtAsset.AsEnumerable().ToList();
                foreach (dsCERTIFICATE_MAIN.SubsystemsRow drSubsystem in dtSubsystems.Rows)
                {
                    dsCERTIFICATE_MAIN.DisciplineReportRow drDisciplineReport = dtDisciplineReport.NewDisciplineReportRow();
                    drDisciplineReport.Subsystem = drSubsystem.SUBSYSTEM;
                    drDisciplineReport.SubsystemDesc = drSubsystem.SUBSYSTEMDESC;
                    populateDisciplineReportStatus(drDisciplineReport, assetRows, drSubsystem.SUBSYSTEM, Discipline.Mechanical.ToString());
                    populateDisciplineReportStatus(drDisciplineReport, assetRows, drSubsystem.SUBSYSTEM, Discipline.Electrical.ToString());
                    populateDisciplineReportStatus(drDisciplineReport, assetRows, drSubsystem.SUBSYSTEM, Discipline.Instrumentation.ToString());
                    populateDisciplineReportStatus(drDisciplineReport, assetRows, drSubsystem.SUBSYSTEM, Discipline.Piping.ToString());
                    populateDisciplineReportStatus(drDisciplineReport, assetRows, drSubsystem.SUBSYSTEM, Discipline.Structural.ToString());
                    populateDisciplineReportStatus(drDisciplineReport, assetRows, drSubsystem.SUBSYSTEM, Discipline.Architectural.ToString());
                    populateDisciplineReportStatus(drDisciplineReport, assetRows, drSubsystem.SUBSYSTEM, Discipline.Civil.ToString());
                    populateDisciplineReportStatus(drDisciplineReport, assetRows, drSubsystem.SUBSYSTEM, Discipline.Others.ToString());
                    dtDisciplineReport.AddDisciplineReportRow(drDisciplineReport);
                }
            }

            return dtDisciplineReport;
        }

        /// <summary>
        /// Populate the discipline columns of certificate reports
        /// </summary>
        public void populateCertificateDisciplineStatus(dsCERTIFICATE_MAIN.CertificateReportRow drCertificate, List<dsCERTIFICATE_MAIN.SubsystemCertificateRow> subsystemRows, IEnumerable<dsCERTIFICATE_MAIN.AssetsRow> disciplineSequence, string subsystem, string discipline, Type statusEnumType)
        {
            dsCERTIFICATE_MAIN.SubsystemCertificateRow subsystemDisciplineRow = subsystemRows.Where(x => !x.IsCertificateDisciplineNull()).FirstOrDefault(x => x.CertificateDiscipline.ToUpper() == discipline.ToUpper());
            string disciplineStatusColumnName = string.Concat(discipline, "Status");
            if (subsystemDisciplineRow != null && drCertificate.Table.Columns.Contains(discipline))
            {
                drCertificate[discipline] = subsystemDisciplineRow.CertificateNumber;
                Enum result;
                if(drCertificate.Table.Columns.Contains(disciplineStatusColumnName) && EnumExtensions.TryParse(statusEnumType, subsystemDisciplineRow.StatusNumber.ToString(), true, out result))
                    drCertificate[disciplineStatusColumnName] = result.ToString().Replace('_', ' ');
                else
                    drCertificate[disciplineStatusColumnName] = "Pending";
            }
            else
            {
                bool isDisciplineAssetPresent = disciplineSequence.Any(x => x.SubsystemName.ToUpper() == subsystem.ToUpper() && x.TemplateDiscipline.ToUpper() == discipline.ToUpper());
                drCertificate[discipline] = isDisciplineAssetPresent ? string.Empty : "N/A";
                drCertificate[disciplineStatusColumnName] = isDisciplineAssetPresent ? string.Empty : "N/A";
            }
        }

        /// <summary>
        /// Populate the sequence column of the NOE reports
        /// </summary>
        public void populateNOESequenceStatus(dsCERTIFICATE_MAIN.NOE_ReportRow drNOEReport, List<dsCERTIFICATE_MAIN.NOESubsystemCertificateRow> subsystemCertificateRows, string subsystem, long sequenceNumber, Type statusEnumType)
        {
            dsCERTIFICATE_MAIN.NOESubsystemCertificateRow drNOESequence = subsystemCertificateRows.Where(x => !x.IsSEQNull()).FirstOrDefault(x => x.SEQ == sequenceNumber);
            if (drNOESequence != null)
            {
                if(!drNOESequence.IsCertificateNumberNull())
                {
                    string NOEPrefixColumnString = string.Concat("NOE", sequenceNumber.ToString());
                    string NOENumberColumnSequenceString = string.Concat(NOEPrefixColumnString, "_NUMBER");
                    string NOEDescriptionColumnSequenceString = string.Concat(NOEPrefixColumnString, "_DESC");
                    string NOEStatusColumnSequenceString = string.Concat(NOEPrefixColumnString, "_STATUS");
                    if (drNOEReport.Table.Columns.Contains(NOENumberColumnSequenceString) && drNOEReport.Table.Columns.Contains(NOENumberColumnSequenceString) && drNOEReport.Table.Columns.Contains(NOEStatusColumnSequenceString))
                    {
                        drNOEReport[NOENumberColumnSequenceString] = drNOESequence.CertificateNumber;
                        drNOEReport[NOEDescriptionColumnSequenceString] = drNOESequence.CertificateDescription;
                        Enum result;
                        if (!drNOESequence.IsStatusNumberNull() && EnumExtensions.TryParse(statusEnumType, drNOESequence.StatusNumber.ToString(), true, out result))
                            drNOEReport[NOEStatusColumnSequenceString] = result.ToString().Replace('_', ' ');
                        else
                            drNOEReport[NOEStatusColumnSequenceString] = "Pending";
                    }
                }
            }
        }

        /// <summary>
        /// Populate the discipline columns of discpline asset report
        /// </summary>
        public void populateDisciplineReportStatus(dsCERTIFICATE_MAIN.DisciplineReportRow drDisciplineReport, List<dsCERTIFICATE_MAIN.AssetsRow> assetRows, string subsystem, string discipline)
        {
            if (!drDisciplineReport.Table.Columns.Contains(discipline))
                return;

            DataRow subsystemDisciplineRow = assetRows.FirstOrDefault(x => x.SubsystemName.ToUpper() == subsystem.ToUpper() && x.TemplateDiscipline.ToUpper() == discipline.ToUpper());
            string disciplineStatusColumnName = discipline;
            if (subsystemDisciplineRow != null)
                drDisciplineReport[discipline] = 1.ToString();
            else
                drDisciplineReport[discipline] = "N/A";
        }

        /// <summary>
        /// Update multiple templates
        /// </summary>
        public void Save(dsCERTIFICATE_MAIN.CERTIFICATE_MAINDataTable dtCERTIFICATE_MAIN)
        {
            _adapter.Update(dtCERTIFICATE_MAIN);
        }

        /// <summary>
        /// Update one template
        /// </summary>
        public void Save(dsCERTIFICATE_MAIN.CERTIFICATE_MAINRow drCERTIFICATE_MAIN)
        {
            _adapter.Update(drCERTIFICATE_MAIN);
        }
        #endregion
        public void Dispose()
        {
            if (_adapter != null)
            {
                _adapter.Dispose();
                _adapter = null;
            }
        }
    }
}