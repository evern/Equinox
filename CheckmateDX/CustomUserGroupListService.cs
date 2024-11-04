using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Commands;
using ProjectLibrary;

namespace CheckmateDX
{
    public class CustomUserGroupListService : IUserGroupListService
    {
        public CustomUserGroupListService()
        {

        }

        public IList<string> GetUserGroups()
        {
            List<string> myUserGroup = new List<string>();
            myUserGroup.Add(CustomUserGroupListService.USER);
            myUserGroup.Add(CustomUserGroupListService.SUPERVISOR);
            myUserGroup.Add(CustomUserGroupListService.DATETIMEPICKER);
            myUserGroup.Add(CustomUserGroupListService.DATEPICKER);
            myUserGroup.Add(CustomUserGroupListService.TIMEPICKER);
            myUserGroup.Add(CustomUserGroupListService.USER_PICTURE);
            myUserGroup.Add(CustomUserGroupListService.ATTACH_PICTURE);
            myUserGroup.Add(CustomUserGroupListService.TOGGLE_ACCEPTANCE);
            myUserGroup.Add(CustomUserGroupListService.TOGGLE_YESNO);
            myUserGroup.Add(CustomUserGroupListService.SELECTION_TESTEQ);
            myUserGroup.Add(CustomUserGroupListService.SIGNATURE_BLOCK);
            myUserGroup.Add(CustomUserGroupListService.INSERT_LINE);
            myUserGroup.Add(CustomUserGroupListService.REMOVE_LINE);
            myUserGroup.Add(CustomUserGroupListService.SELECT_TAG);
            myUserGroup.Add(CustomUserGroupListService.PHOTO);
            myUserGroup.Add(CustomUserGroupListService.ALL);
            myUserGroup.Add(CustomUserGroupListService.ANY);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_ARCHITECTURAL);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_CIVILS);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_ELECTRICAL);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_INSTRUMENTATION);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_MECHANICAL);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_OTHERS);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_PIPING);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_STRUCTURAL);
            myUserGroup.Add(CustomUserGroupListService.SUBSYSTEM_SELECTION);
            myUserGroup.Add(CustomUserGroupListService.CVC_TYPE);
            myUserGroup.Add(CustomUserGroupListService.TAG_SELECTION);
            myUserGroup.Add(CustomUserGroupListService.CERTIFICATE_TAG_SELECTION);
            myUserGroup.Add(CustomUserGroupListService.CERTIFICATE_NUMBER);
            myUserGroup.Add(CustomUserGroupListService.CERTIFICATE_DESCRIPTION);
            myUserGroup.Add(CustomUserGroupListService.CAT_A_VERIFICATION);
            myUserGroup.Add(CustomUserGroupListService.SUBSYSTEM_PREFIX);
            myUserGroup.Add(CustomUserGroupListService.TAG_PREFIX);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_ARCHITECTURAL);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_CIVIL);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_MECHANICAL);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_PIPING);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_ELECTRICAL);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_STRUCTURAL);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_INSTRUMENTATION);
            myUserGroup.Add(CustomUserGroupListService.PUNCHLIST_CAT_A);
            myUserGroup.Add(CustomUserGroupListService.CVC_SUBSYSTEM_COMPLETION);
            myUserGroup.Add(CustomUserGroupListService.PUNCHLIST_WALKDOWN_SUBSYSTEM_COMPLETION);
            myUserGroup.Add(CustomUserGroupListService.TAG_SELECTION);
            return myUserGroup;
        }

        public List<string> GetUserGroupList()
        {
            List<string> myUserGroup = new List<string>();
            myUserGroup.Add(CustomUserGroupListService.USER);
            myUserGroup.Add(CustomUserGroupListService.SUPERVISOR);
            myUserGroup.Add(CustomUserGroupListService.DATETIMEPICKER);
            myUserGroup.Add(CustomUserGroupListService.DATEPICKER);
            myUserGroup.Add(CustomUserGroupListService.TIMEPICKER);
            myUserGroup.Add(CustomUserGroupListService.USER_PICTURE);
            myUserGroup.Add(CustomUserGroupListService.ATTACH_PICTURE);
            myUserGroup.Add(CustomUserGroupListService.TOGGLE_ACCEPTANCE);
            myUserGroup.Add(CustomUserGroupListService.TOGGLE_YESNO);
            myUserGroup.Add(CustomUserGroupListService.SELECTION_TESTEQ);
            myUserGroup.Add(CustomUserGroupListService.SIGNATURE_BLOCK);
            myUserGroup.Add(CustomUserGroupListService.INSERT_LINE);
            myUserGroup.Add(CustomUserGroupListService.REMOVE_LINE);
            myUserGroup.Add(CustomUserGroupListService.SELECT_TAG);
            myUserGroup.Add(CustomUserGroupListService.PHOTO);
            myUserGroup.Add(CustomUserGroupListService.ALL);
            myUserGroup.Add(CustomUserGroupListService.INITIALS);
            myUserGroup.Add(CustomUserGroupListService.ANY);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_ARCHITECTURAL);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_CIVILS);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_ELECTRICAL);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_INSTRUMENTATION);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_MECHANICAL);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_OTHERS);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_PIPING);
            myUserGroup.Add(CustomUserGroupListService.DISCIPLINE_TOGGLE_STRUCTURAL);
            myUserGroup.Add(CustomUserGroupListService.SUBSYSTEM_SELECTION);
            myUserGroup.Add(CustomUserGroupListService.CVC_TYPE);
            myUserGroup.Add(CustomUserGroupListService.CERTIFICATE_TAG_SELECTION);
            myUserGroup.Add(CustomUserGroupListService.TAG_SELECTION);
            myUserGroup.Add(CustomUserGroupListService.CERTIFICATE_NUMBER);
            myUserGroup.Add(CustomUserGroupListService.CERTIFICATE_DESCRIPTION);
            myUserGroup.Add(CustomUserGroupListService.CAT_A_VERIFICATION);
            myUserGroup.Add(CustomUserGroupListService.SUBSYSTEM_PREFIX);
            myUserGroup.Add(CustomUserGroupListService.TAG_PREFIX);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_ARCHITECTURAL);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_CIVIL);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_MECHANICAL);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_PIPING);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_ELECTRICAL);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_STRUCTURAL);
            myUserGroup.Add(CustomUserGroupListService.ITR_COMPLETION_INSTRUMENTATION);
            myUserGroup.Add(CustomUserGroupListService.PUNCHLIST_CAT_A);
            myUserGroup.Add(CustomUserGroupListService.CVC_SUBSYSTEM_COMPLETION);
            myUserGroup.Add(CustomUserGroupListService.PUNCHLIST_WALKDOWN_SUBSYSTEM_COMPLETION);
            myUserGroup.Add(CustomUserGroupListService.TAG_SELECTION);
            return myUserGroup;
        }

        public static string USER{ get { return "USER";} }
        public static string SUPERVISOR { get { return "SUPERVISOR"; } }
        public static string DATETIMEPICKER { get { return "DATETIMEPICK"; } }
        public static string DATEPICKER { get { return "DATEPICK"; } }
        public static string TIMEPICKER { get { return "TIMEPICK"; } }
        public static string USER_PICTURE { get { return "USER PICTURE"; } }
        public static string ATTACH_PICTURE { get { return "ATTACH PICTURE"; } }
        public static string TOGGLE_ACCEPTANCE { get { return "ACCEPTANCE";} }
        public static string CVC_STATUS_TOGGLE_ACCEPTANCE_PREFIX { get { return "ACCEPTANCE_CVC_"; } }
        public static string TOGGLE_YESNO { get { return "YESNO"; } }
        public static string SELECTION_TESTEQ { get { return "TEST EQ"; } }
        public static string SIGNATURE_BLOCK { get { return "SIGNATURE"; } }
        public static string PUNCHLIST_BLOCK { get { return "PUNCHLIST"; } }
        public static string INSERT_LINE { get { return "INSERT LINE"; } }
        public static string REMOVE_LINE { get { return "REMOVE LINE"; } }
        public static string SELECT_TAG { get { return "SELECT TAG?"; } }
        public static string ALL { get { return "ALL"; } }
        public static string ANY { get { return "ANY"; } }
        public static string PHOTO { get { return "PHOTO"; } }
        public static string INITIALS { get { return "INITIALS"; } }
        public static string DISCIPLINE_TOGGLE_CIVILS { get { return "DISCIPLINE_TOGGLE_CIVILS"; } }
        public static string DISCIPLINE_TOGGLE_STRUCTURAL { get { return "DISCIPLINE_TOGGLE_STRUCTURAL"; } }
        public static string DISCIPLINE_TOGGLE_PIPING { get { return "DISCIPLINE_TOGGLE_PIPING"; } }
        public static string DISCIPLINE_TOGGLE_MECHANICAL { get { return "DISCIPLINE_TOGGLE_MECHANICAL"; } }
        public static string DISCIPLINE_TOGGLE_ELECTRICAL { get { return "DISCIPLINE_TOGGLE_ELECTRICAL"; } }
        public static string DISCIPLINE_TOGGLE_INSTRUMENTATION { get { return "DISCIPLINE_TOGGLE_INSTRUMENTATION"; } }
        public static string DISCIPLINE_TOGGLE_ARCHITECTURAL { get { return "DISCIPLINE_TOGGLE_ARCHITECTURAL"; } }
        public static string DISCIPLINE_TOGGLE_OTHERS { get { return "DISCIPLINE_TOGGLE_OTHERS"; } }
        public static string SUBSYSTEM_PREFIX { get { return "CERTIFICATE_SUBSYSTEM_PREFIX_"; } }
        public static string TAG_PREFIX { get { return "CERTIFICATE_TAG_PREFIX_"; } }
        public static string CERTIFICATE_NUMBER { get { return "CERTIFICATE_NUMBER"; } }
        public static string CERTIFICATE_DESCRIPTION { get { return "CERTIFICATE_DESCRIPTION"; } }
        public static string SUBSYSTEM_SELECTION { get { return "SUBSYSTEM_SELECTION"; } }
        public static string CVC_TYPE { get { return "CVC_TYPE"; } }
        public static string CERTIFICATE_TAG_SELECTION { get { return "CERTIFICATE_TAG_SELECTION"; } }
        public static string ITR_COMPLETION_ARCHITECTURAL { get { return Variables.ITR_Completion_Prefix + Discipline.Architectural.ToString().ToUpper(); } }
        public static string ITR_COMPLETION_CIVIL { get { return Variables.ITR_Completion_Prefix + Discipline.Civil.ToString().ToUpper(); } }
        public static string ITR_COMPLETION_MECHANICAL { get { return Variables.ITR_Completion_Prefix + Discipline.Mechanical.ToString().ToUpper(); } }
        public static string ITR_COMPLETION_PIPING { get { return Variables.ITR_Completion_Prefix + Discipline.Piping.ToString().ToUpper(); } }
        public static string ITR_COMPLETION_ELECTRICAL { get { return Variables.ITR_Completion_Prefix + Discipline.Electrical.ToString().ToUpper(); } }
        public static string ITR_COMPLETION_STRUCTURAL { get { return Variables.ITR_Completion_Prefix + Discipline.Structural.ToString().ToUpper(); } }
        public static string ITR_COMPLETION_INSTRUMENTATION { get { return Variables.ITR_Completion_Prefix + Discipline.Instrumentation.ToString().ToUpper(); } }
        public static string PUNCHLIST_CAT_A { get { return "CAT_A_PUNCHLIST_COMPLETION"; } }
        public static string CVC_SUBSYSTEM_COMPLETION { get { return "CVC_SUBSYSTEM_COMPLETION"; } }
        public static string PUNCHLIST_WALKDOWN_SUBSYSTEM_COMPLETION { get { return "PUNCHLIST_WALKDOWN_SUBSYSTEM_COMPLETION"; } }
        public static string TAG_SELECTION { get { return "TAG_SELECTION"; } }
        public static string CAT_A_VERIFICATION { get { return "CAT_A_VERIFICATION"; } }
    }
}
