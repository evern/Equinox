using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectLibrary
{
    public static class System_Privilege
    {
        static List<Privilege> _list;

        static System_Privilege()
        {
            _list = new List<Privilege>();
            //Add, Edit, Delete User (child filter applies)
            //Authorise user disciplines to self authorised disciplines
            //Authorise user projects to self authorised projects
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.ManageUser.ToString(), privName = "Manage Users", privCategory = "System" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.ResetPassword.ToString(), privName = "Reset User Password", privCategory = "System" });
            //Add, Edit, Delete Projects (Recommended for admin only)
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.ManageProject.ToString(), privName = "Manage Projects", privCategory = "System" });
            //Add, Edit, Delete Roles, Assign Privilege (child filter applies)
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.ManageRole.ToString(), privName = "Manage Roles", privCategory = "System" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.ManageTemplate.ToString(), privName = "Manage Template", privCategory = "System" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.ManagePrefill.ToString(), privName = "Manage Prefill", privCategory = "System" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.ManageSchedule.ToString(), privName = "Manage Schedule", privCategory = "System" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.ManageEquipment.ToString(), privName = "Manage Equipment", privCategory = "System" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.ManageDatabase.ToString(), privName = "Manage Database", privCategory = "System" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.RestrictRoleToAuthorisedProjectsOnly.ToString(), privName = "Restrict Role to Authorised Projects Only", privCategory = "System" });

            //ITR
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.MaintainMasterFeedSheet.ToString(), privName = "Maintain Master Feed Sheet", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.CreateITR.ToString(), privName = "Create ITR", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.DeleteITR.ToString(), privName = "Delete ITR", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.MarkITRInspected.ToString(), privName = "Inspect ITR", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.MarkITRApproved.ToString(), privName = "Approve ITR", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.MarkITRCompleted.ToString(), privName = "Complete ITR", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.MarkITRClosed.ToString(), privName = "Close ITR", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.RejectITRInspected.ToString(), privName = "Reject Inspected ITR", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.RejectITRApproved.ToString(), privName = "Reject Approved ITR", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.RejectITRCompleted.ToString(), privName = "Reject Completed ITR", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.UserIsTester.ToString(), privName = "User Is Tester", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.ReplaceITRContent.ToString(), privName = "Replace ITR Content", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.SkipStages.ToString(), privName = "Skip Stages", privCategory = "ITR" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.ShowCompletedAndClosedITRsOnly.ToString(), privName = "Show Completed and Closed ITRs Only", privCategory = "ITR" });

            //Punchlist
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.ImportPunchlist.ToString(), privName = "Import Punchlist", privCategory = "Punchlist" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.CreatePunchlist.ToString(), privName = "Create Punchlist", privCategory = "Punchlist" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.DeletePunchlist.ToString(), privName = "Delete Punchlist", privCategory = "Punchlist" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.MarkPunchlistCategorised.ToString(), privName = "Categorise Punchlist", privCategory = "Punchlist" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.MarkPunchlistInspected.ToString(), privName = "Inspect Punchlist", privCategory = "Punchlist" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.MarkPunchlistApproved.ToString(), privName = "Approve Punchlist", privCategory = "Punchlist" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.MarkPunchlistCompleted.ToString(), privName = "Complete Punchlist", privCategory = "Punchlist" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.MarkPunchlistClosed.ToString(), privName = "Close Punchlist", privCategory = "Punchlist" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.RejectPunchlistCategorised.ToString(), privName = "Reject Categorised Punchlist", privCategory = "Punchlist" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.RejectPunchlistInspected.ToString(), privName = "Reject Inspected Punchlist", privCategory = "Punchlist" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.RejectPunchlistApproved.ToString(), privName = "Reject Approved Punchlist", privCategory = "Punchlist" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.RejectPunchlistCompleted.ToString(), privName = "Reject Completed Punchlist", privCategory = "Punchlist" });

            //Certificate
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.CreatePunchlistWalkdown.ToString(), privName = "Create Punchlist Walkdown", privCategory = "Punchlist Walkdown" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.DeletePunchlistWalkDown.ToString(), privName = "Delete Punchlist Walkdown", privCategory = "Punchlist Walkdown" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.PunchlistWalkdown_Status_Exported.ToString(), privName = "Export Punchlist Walkdown", privCategory = "Punchlist Walkdown" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Reject_PunchlistWalkdown_Status_Exported.ToString(), privName = "Reject Punchlist Walkdown Exported Status", privCategory = "Punchlist Walkdown" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.PunchlistWalkdown_Status_Closed.ToString(), privName = "Close Punchlist Walkdown", privCategory = "Punchlist Walkdown" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Reject_PunchlistWalkdown_Status_Closed.ToString(), privName = "Reject Punchlist Walkdown Closed Status", privCategory = "Punchlist Walkdown" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.CreateCVC.ToString(), privName = "Create CVC", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.DeleteCVC.ToString(), privName = "Delete CVC", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.CVC_Status_Approved.ToString(), privName = "Approve CVC", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Toggle_CVC_Status_Approved.ToString(), privName = "Approved CVC Interactable Toggle", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Reject_CVC_Status_Approved.ToString(), privName = "Reject Approved CVC", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.CVC_Status_Supervisor.ToString(), privName = "CVC Supervisor Approval", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Toggle_CVC_Status_Supervisor.ToString(), privName = "Supervisor CVC Interactable Toggle", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Reject_CVC_Status_Supervisor.ToString(), privName = "Reject CVC Supervisor Approval", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.CVC_Status_ConstructionManager.ToString(), privName = "CVC Construction Manager Approval", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Toggle_CVC_Status_ConstructionManager.ToString(), privName = "Construction Manager CVC Interactable Toggle", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Reject_CVC_Status_ConstructionManager.ToString(), privName = "Reject CVC Construction Manager Approval", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.CVC_Status_Client.ToString(), privName = "CVC Client Approval", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Toggle_CVC_Status_Client.ToString(), privName = "Client CVC Interactable Toggle", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Reject_CVC_Status_Client.ToString(), privName = "Reject CVC Client Approval", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.CVC_Status_CommissioningManager.ToString(), privName = "CVC Commissioning Manager Approval", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Toggle_CVC_Status_CommissioningManager.ToString(), privName = "Commissioning Manager CVC Interactable Toggle", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Reject_CVC_Status_CommissioningManager.ToString(), privName = "Reject CVC Commissioning Manager Approval", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.CreateNOE.ToString(), privName = "Create NOE", privCategory = "NOE" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.DeleteNOE.ToString(), privName = "Delete NOE", privCategory = "NOE" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.NOE_Status_Approved.ToString(), privName = "Approve NOE", privCategory = "NOE" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Reject_NOE_Status_Approved.ToString(), privName = "Reject Approved NOE", privCategory = "NOE" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.NOE_Status_CommissioningEngineer.ToString(), privName = "NOE Commissioning Engineer Approval", privCategory = "NOE" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Reject_NOE_Status_CommissioningEngineer.ToString(), privName = "Reject NOE Commissioning Engineer Approval", privCategory = "NOE" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.NOE_Status_ConstructionManager.ToString(), privName = "NOE Construction Manager Approval", privCategory = "NOE" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Reject_NOE_Status_ConstructionManager.ToString(), privName = "Reject NOE Construction Manager Approval", privCategory = "NOE" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.NOE_Status_AuthorisationIsolationOfficer.ToString(), privName = "NOE Authorisation Isolation Officer Approval", privCategory = "NOE" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Reject_NOE_Status_AuthorisationIsolationOfficer.ToString(), privName = "Reject NOE Authorisation Isolation Officer Approval", privCategory = "NOE" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.NOE_Status_CommissioningManager.ToString(), privName = "NOE Commissioning Manager Approval", privCategory = "NOE" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.Reject_NOE_Status_CommissioningManager.ToString(), privName = "Reject NOE Commissioning Manager Approval", privCategory = "NOE" });

            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.DesignPunchlistWalkdown.ToString(), privName = "Design Punchlist Walkdown Template", privCategory = "Punchlist Walkdown" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.DesignCVC.ToString(), privName = "Design CVC Template", privCategory = "CVC" });
            _list.Add(new Privilege(Guid.Empty) { privTypeID = PrivilegeTypeID.DesignNOE.ToString(), privName = "Design NOE Template", privCategory = "NOE" });
        }

        public static List<Privilege> GetList()
        {
            return _list;
        }
    }
}
