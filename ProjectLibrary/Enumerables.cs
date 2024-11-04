using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ProjectLibrary
{
    public enum PrivilegeTypeID
    {
        //System
        ManageUser,
        ResetPassword,
        ManageProject,
        ManageRole,
        ManageTemplate,
        ManagePrefill,
        ManageSchedule,
        ManageEquipment,
        ManageDatabase,
        RestrictRoleToAuthorisedProjectsOnly,

        //ITR
        MaintainMasterFeedSheet, 
        CreateITR,
        DeleteITR,
        MarkITRInspected,
        MarkITRApproved,
        MarkITRCompleted,
        MarkITRClosed,
        RejectITRInspected,
        RejectITRApproved,
        RejectITRCompleted,
        UserIsTester, 
        ReplaceITRContent,
        SkipStages,
        ShowCompletedAndClosedITRsOnly,

        //Punchlist
        CreatePunchlist,
        DeletePunchlist,
        ImportPunchlist,
        MarkPunchlistCategorised,
        MarkPunchlistInspected,
        MarkPunchlistApproved,
        MarkPunchlistCompleted,
        MarkPunchlistClosed,
        RejectPunchlistCategorised,
        RejectPunchlistInspected,
        RejectPunchlistApproved,
        RejectPunchlistCompleted,

        //Certificate
        CreatePunchlistWalkdown,
        CreateCVC,
        CreateNOE,
        PunchlistWalkdown_Status_Exported,
        Reject_PunchlistWalkdown_Status_Exported,
        PunchlistWalkdown_Status_Closed,
        Reject_PunchlistWalkdown_Status_Closed,
        DeletePunchlistWalkDown,
        DeleteCVC,
        DeleteNOE,
        CVC_Status_Approved,
        Toggle_CVC_Status_Approved,
        Reject_CVC_Status_Approved,
        CVC_Status_Supervisor,
        Toggle_CVC_Status_Supervisor,
        Reject_CVC_Status_Supervisor,
        CVC_Status_ConstructionManager,
        Toggle_CVC_Status_ConstructionManager,
        Reject_CVC_Status_ConstructionManager,
        CVC_Status_Client,
        Toggle_CVC_Status_Client,
        Reject_CVC_Status_Client,
        CVC_Status_CommissioningManager,
        Toggle_CVC_Status_CommissioningManager,
        Reject_CVC_Status_CommissioningManager,
        NOE_Status_Approved,
        Reject_NOE_Status_Approved,
        NOE_Status_CommissioningEngineer,
        Reject_NOE_Status_CommissioningEngineer, 
        NOE_Status_ConstructionManager,
        Reject_NOE_Status_ConstructionManager,
        NOE_Status_AuthorisationIsolationOfficer,
        Reject_NOE_Status_AuthorisationIsolationOfficer,
        NOE_Status_CommissioningManager,
        Reject_NOE_Status_CommissioningManager,
        DesignPunchlistWalkdown,
        DesignCVC,
        DesignNOE
    }

    public enum FeatureTypeID
    {
        SelectTestEq
    }

    public enum Discipline
    {
        Electrical, Instrumentation, Mechanical, Structural, Civil, Piping, Architectural, Others
    }

    public enum SearchMode
    {
        [Display(Name = "Start With")]
        Starts_With,
        [Display(Name = "Contains")]
        Contains,
        [Display(Name = "End With")]
        Ends_With
    }

    public enum Toggle_Acceptance
    {
        Click_Here, Acceptable, Punchlisted, Not_Applicable
    }

    public enum Toggle_YesNo
    {
        Click_Here, Yes, No
    }

    public enum Toggle_AcceptableNoNA
    {
        Click_Here, Acceptable, No, Not_Applicable
    }

    public enum Toggle_PartialComplete
    {
        Click_Here, Partial, Complete
    }

    public enum PrefillType
    { 
        Tag, WBS, Multiple, Certificate
    }

    public enum iTRBrowser_Update
    {
        None, Deleted, Saved, Progressed, SavedProgress, SavedClose
    }

    public enum iTRStatusChange
    {
        New, Increase, Decrease
    }

    public enum CertificateStatusChange
    {
        New, Increase, Decrease
    }

    public enum ITR_Status
    {
        Inspected, Approved, Completed, Closed
    }

    public enum PunchlistWalkdown_Status
    {
        Pending, Exported, Closed
    }

    public enum CVC_Status
    {
        Pending, Approved, Supervisor, Commissioning_Manager, Construction_Manager, Client
    }

    public enum NOE_Status
    {
        Pending, Approved, Commissioning_Engineer, Construction_Manager, Authorised_Isolation_Officer, Commissioning_Manager
    }

    public enum Punchlist_Status
    {
        Categorised, Inspected, Approved, Completed, Closed
    }

    public enum Punchlist_Category
    {
        Job_Completeness, Safety, Design_Deficiencies, Addition_to_Project, FAT_or_Offsite_Carry_Over
    }

    public enum Punchlist_ActionBy
    {
        Construction, Commissioning, Design, Principal
    }

    public enum Punchlist_Priority
    {
        A, B, C
    }

    public enum PunchlistImageType
    {
        Inspection, Remedial
    }

    public enum HighlightType
    {
        Editables, Interactables, Both
    }

    public enum Sync_Type_Superseded
    {
        ITR, Punchlist, User, Role, Template, Prefill, Workflow, Project, Schedule, WBS, Tag, Privilege, Equipment
    }

    public enum Sync_Mode
    {
        Push, Pull, Both, None
    }

    public enum Progress_Restriction
    {
        None, Acceptance, Punchlist
    }

    public enum Punchlist_Progress_Restriction
    {
        Allowed, EqualOrMore, ToggleNotExist
    }

    /// <summary>
    /// Unified Sync Category Name
    /// </summary>
    public enum Sync_Category
    {
        General,
        Design,
        Plan,
        Authorisation,
        Content
    }

    /// <summary>
    /// Unified Sync Item Name
    /// </summary>
    public enum Sync_Item
    {
        Project,
        Equipment,
        Template,
        Template_Components,
        Schedule,
        Header_Data,
        Role,
        User,
        ITR,
        Punchlist,
        Certificate
    }

    public enum Sync_Type { PROJECT, GENERAL_EQUIPMENT, WORKFLOW_MAIN, TEMPLATE_MAIN, TEMPLATE_TOGGLE, PREFILL_MAIN, SCHEDULE, TAG, WBS, TEMPLATE_REGISTER, PREFILL_REGISTER, ROLE_MAIN, ROLE_PRIVILEGE, USER_MAIN, USER_PROJECT, USER_DISC, ITR_MAIN, ITR_STATUS, ITR_STATUS_ISSUE, PUNCHLIST_MAIN, PUNCHLIST_STATUS, PUNCHLIST_STATUS_ISSUE, MATRIX_TYPE, MATRIX_ASSIGNMENT, CERTIFICATE_DATA, CERTIFICATE_MAIN, CERTIFICATE_STATUS, CERTIFICATE_STATUS_ISSUE }
    public enum Database_Type { LOCAL, REMOTE }
    public enum SyncStatus_Display { Pending, Comparing, Skipped, Ok, Downloading, Downloading_Template, Uploading_Template, Uploading, Saving_Locally, Saving_Remotely }
    public enum SyncStatus_Count { Delete, Download, Upload, Same, Conflict, Resolve, None }
}