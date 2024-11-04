using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectLibrary
{
    public class Classes
    {

    }

    /// <summary>
    /// Common attributes for database referencing classes
    /// </summary>
    public class ParentClass
    {
        public Guid GUID { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
    }

    /// <summary>
    /// Used for storing punchlist for display purposes
    /// </summary>
    public class Punchlist : ParentClass
    {
        public Punchlist(Guid guid)
        {
            GUID = guid;
        }

        public string punchlistDisplayAttachment { get; set; }
        public string punchlistTitle { get; set; }
        public string punchlistDescription { get; set; }
        public string punchlistRemedial { get; set; }
        public string punchlistDiscipline { get; set; }
        public string punchlistCategory { get; set; }
        public string punchlistActionBy { get; set; }
        public string punchlistPriority { get; set; }
        public int punchlistImageIndex { get; set; }
        public Tag punchlistAttachTag { get; set; }
        public WBS punchlistAttachWBS { get; set; }
        public PunchlistITRTemplate punchlistITR { get; set; }
        public string punchlistItem { get; set; }
        public int punchlistStatus { get; set; }
        public bool punchlistEnabled { get; set; }

        public string punchlistStatusName { get; set; }
        public string punchlistParentWBSName { get; set; }
        public string punchlistParentWBSDesc { get; set; }
        public Guid? punchlistParentWBSGuid { get; set; }

        //use only for spreadsheet import
        public Guid? punchlistTagGuid { get; set; }
        public Guid? punchlistWBSGuid { get; set; }
    }

    public class ProjectWBS
    {
        public string Area { get; set; }
        public string AreaDescription { get; set; }
        public string System { get; set; }
        public string SystemDescription { get; set; }
        public string Subsystem { get; set; }
        public string SubsystemDescription { get; set; }

        public string AreaDisplayMember => String.Concat(Area, " - ", AreaDescription);
        public string SystemDisplayMember => String.Concat(System, " - ", SystemDescription);
        public string SubsystemDisplayMember => String.Concat(Subsystem, " - ", SubsystemDescription);
    }

    /// <summary>
    /// Used for storing punchlist comments for display purposes
    /// </summary>
    public class punchlistComments : ParentClass
    {
        public punchlistComments(Guid guid)
        {
            GUID = guid;
        }

        public Guid punchlistCommParentGuid { get; set; }
        public string punchlistCommInfo { get; set; }
        public string punchlistCommCreator { get; set; }
        public int punchlistCommImageIndex { get; set; }
    }

    /// <summary>
    /// Used for tagging whether tag is enabled within workflow
    /// </summary>
    public class tagWorkflow
    {
        public ValuePair twTag { get; set; }
        public ValuePair twWorkflow { get; set; }
        public int twPriority { get; set; }
        public bool twEnabled { get; set; }
    }

    /// <summary>
    /// Used for tagging whether wbs is enabled within workflow
    /// </summary>
    public class wbsWorkflow
    {
        public ValuePair wwWBS { get; set; }
        public ValuePair wwWorkflow { get; set; }
        public int wwPriority { get; set; }
        public bool wwEnabled { get; set; }
    }

    /// <summary>
    /// Stores saved/incompleted ITRs for Tag with WBS parent info
    /// </summary>
    public class tagCount
    {
        public tagCount()
        {
            tcCompletedCount = 0;
        }

        public ValuePair tcTag { get; set; }
        public int tcTotalCount { get; set; }
        public int tcInspectedCount { get; set; }
        public int tcApprovedCount { get; set; }
        public int tcCompletedCount { get; set; }
        public int tcClosedCount { get; set; }
    }

    /// <summary>
    /// Stores saved/incompleted ITRs for WBS
    /// </summary>
    public class wbsCount
    {
        public wbsCount()
        {
            wcTotalCount = 0;
        }

        public ValuePair wcWBS { get; set; }
        public int wcTotalCount { get; set; }
        public int wcInspectedCount { get; set; }
        public int wcApprovedCount { get; set; }
        public int wcCompletedCount { get; set; }
        public int wcClosedCount { get; set; }
    }

    /// <summary>
    /// Used for marking the priority of workflows
    /// </summary>
    public class workflowPriority
    {
        public ValuePair wpWorkflow { get; set; }
        public int wpPriority { get; set; }
    }

    /// <summary>
    /// Used for storing ITR comments for display purpose
    /// </summary>
    public class iTRComments : ParentClass
    {
        public iTRComments(Guid guid)
        {
            GUID = guid;
        }

        public Guid iTRCommParentGuid { get; set; }
        public string iTRCommInfo { get; set; }
        public string iTRCommCreator { get; set; }
        public int iTRCommImageIndex { get; set; }
    }


    /// <summary>
    /// Used for storing ITR comments for display purpose
    /// </summary>
    public class CertificateComments : ParentClass
    {
        public CertificateComments(Guid guid)
        {
            GUID = guid;
        }

        public Guid CertificateCommParentGuid { get; set; }
        public string CertificateCommInfo { get; set; }
        public string CertificateCommCreator { get; set; }
        public int CertificateCommImageIndex { get; set; }
    }

    /// <summary>
    /// WBS and Tag Display Class so that Both of them can exist on the same grid
    /// </summary>
    public class wbsTagDisplay
    {
        public wbsTagDisplay(object StoreObject)
        {
            if (StoreObject.GetType() == typeof(Tag))
            {
                wbsTagDisplayAttachTag = (Tag)StoreObject;
                wbsTagDisplayImageIndex = 1;

                wbsTagDisplayGuid = wbsTagDisplayAttachTag.GUID;
                wbsTagDisplayName = wbsTagDisplayAttachTag.tagNumber;
                wbsTagDisplayDescription = wbsTagDisplayAttachTag.tagDescription;
                wbsTagDisplayType1 = wbsTagDisplayAttachTag.tagType1;
                wbsTagDisplayType2 = wbsTagDisplayAttachTag.tagType2;
                wbsTagDisplayType3 = wbsTagDisplayAttachTag.tagType3;
                wbsTagDisplayScheduleGuid = wbsTagDisplayAttachTag.tagScheduleGuid;
                wbsTagDisplayDiscipline = wbsTagDisplayAttachTag.tagDiscipline;

                try
                {
                    wbsTagDisplayParentGuid = wbsTagDisplayAttachTag.tagParentGuid;
                }
                catch
                {
                    wbsTagDisplayParentGuid = Guid.Empty; //if problem exists attach to root by default
                }
            }
            else if (StoreObject.GetType() == typeof(WBS))
            {
                wbsTagDisplayAttachWBS = (WBS)StoreObject;
                wbsTagDisplayImageIndex = 0;
                wbsTagDisplayGuid = wbsTagDisplayAttachWBS.GUID;
                wbsTagDisplayType1 = "N/A";
                wbsTagDisplayType2 = "N/A";
                wbsTagDisplayType3 = "N/A";
                wbsTagDisplayScheduleGuid = wbsTagDisplayAttachWBS.wbsScheduleGuid;

                try
                {
                    wbsTagDisplayParentGuid = wbsTagDisplayAttachWBS.wbsParentGuid;
                }
                catch
                {
                    wbsTagDisplayParentGuid = Guid.Empty; //if problem exists attach to root
                }

                wbsTagDisplayName = wbsTagDisplayAttachWBS.wbsName;
                wbsTagDisplayDescription = wbsTagDisplayAttachWBS.wbsDescription;
            }

            //wbsTagDisplayEnabled = true;
            showOnPunchlistBrowser = false;
        }


        public bool showOnPunchlistBrowser { get; set; }

        public Guid wbsTagDisplayGuid { get; set; }
        public Guid wbsTagDisplayParentGuid { get; set; }
        public string wbsTagDisplayName { get; set; }
        public string wbsTagDisplayDescription { get; set; }
        public string wbsTagDisplayType1 { get; set; }
        public string wbsTagDisplayType2 { get; set; }
        public string wbsTagDisplayType3 { get; set; }
        public Guid wbsTagDisplayScheduleGuid { get; set; }
        public Tag wbsTagDisplayAttachTag { get; set; }
        public WBS wbsTagDisplayAttachWBS { get; set; }
        public int wbsTagDisplayImageIndex { get; set; }
        public string wbsTagDisplayDiscipline { get; set; }

        public bool wbsTagDisplayEnabled => true;

        //public bool wbsTagDisplayEnabled
        //{
        //    get
        //    {
        //        if (((wbsTagDisplayChildTotalCount - wbsTagDisplaySelfTotalCount) - wbsTagDisplayChildInspectedCount) > 0)
        //            return false;
        //        else
        //            return true;
        //    }
        //}

        public override string ToString()
        {
            return wbsTagDisplayName;
        }

    #region CountWBSPercentage
        public float wbsTagDisplaySelfTotalCount { get; set; }

        public float wbsTagDisplayChildTotalCount { get; set; }
        public float wbsTagDisplayChildInspectedCount { get; set; }
        public float wbsTagDisplayChildApprovedCount { get; set; }
        public float wbsTagDisplayChildCompletedCount { get; set; }
        public float wbsTagDisplayChildClosedCount { get; set; }
        public float wbsTagDisplayTotalProgress { get; set; }
        public float wbsTagDisplayInspectedProgress { get; set; }
        public float wbsTagDisplayApprovedProgress { get; set; }
        public float wbsTagDisplayCompletedProgress { get; set; }
        public float wbsTagDisplayClosedProgress { get; set; }

        public float wbsTagDisplayIncompleteCount
        {
            get
            {
                return wbsTagDisplayChildTotalCount - wbsTagDisplayChildInspectedCount;
            }
        }
    #endregion
    }

    /// <summary>
    /// wbs tag extension class to store the header
    /// </summary>
    public class wbsTagHeader : wbsTagDisplay
    {
        public wbsTagHeader(object StoreObject)
            : base(StoreObject)
        {
            
        }

        public List<string> wbsTagHeaderItems = new List<string>();
    }

    public class WBS : ParentClass
    {
        public WBS(Guid guid)
        {
            GUID = guid;
        }

        public Guid wbsScheduleGuid { get; set; }
        public bool wbsIsSubsystem { get; set; }
        public Guid wbsParentGuid { get; set; }
        public string wbsName { get; set; }
        public string wbsDescription { get; set; }
        public string wbsDiscipline { get; set; }
        public bool wbsIsDeleted { get; set; }
    }

    public class Tag : ParentClass
    {
        public Tag(Guid guid)
        {
            GUID = guid;
        }

        public Guid tagScheduleGuid { get; set; }
        public Guid tagParentGuid { get; set; }
        public Guid tagWBSGuid { get; set; }
        public string tagNumber { get; set; }
        public string tagDescription { get; set; }
        public string tagDiscipline { get; set; }
        public string tagType1 { get; set; }
        public string tagType2 { get; set; }
        public string tagType3 { get; set; }
        public bool tagIsDeleted { get; set; }
    }

    public class Schedule : ParentClass
    {
        public Schedule(Guid guid)
        {
            GUID = guid;
        }

        public Guid scheduleProjectGuid { get; set; }
        public string scheduleName { get; set; }
        public string scheduleDescription { get; set; }
        public string scheduleDiscipline { get; set; }
    }

    public class Equipment : ParentClass
    {
        public Equipment(Guid guid)
        {
            GUID = guid;
        }

        public string EquipmentDiscipline { get; set; }
        public string EquipmentAssetNumber { get; set; }
        public string EquipmentMake { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentModel { get; set; }
        public string EquipmentSerial { get; set; }
        public DateTime EquipmentExpiry { get; set; }
    }

    /// <summary>
    /// Workflow for datagrid binding
    /// </summary>
    public class Workflow : ParentClass
    {
        public Workflow(Guid guid)
        {
            GUID = guid;
        }

        public Guid workflowParentGuid { get; set; }
        public string workflowParentName { get; set; }
        public string workflowName { get; set; }
        public string workflowDescription { get; set; }
    }

    /// <summary>
    /// Prefill for datagrid binding
    /// </summary>
    public class Prefill : ParentClass
    {
        public Prefill(Guid guid)
        {
            GUID = guid;
            prefillExists = false;
        }

        public string prefillName { get; set; }
        public string prefillDiscipline { get; set; }
        public string prefillCategory { get; set; }
        public bool prefillExists { get; set; }
        public override string ToString()
        {
            return prefillName;
        }
    }

    /// <summary>
    /// Workflow and Template Display Class so that Both of them can exist on the same grid
    /// </summary>
    public class wtDisplay
    {
        public wtDisplay(object StoreObject)
        {
            if (StoreObject.GetType() == typeof(Template))
            {
                wtDisplayAttachTemplate = (Template)StoreObject;
                wtDisplayImageIndex = 1;
                wtDisplayGuid = wtDisplayAttachTemplate.GUID;
                wtDisplayName = wtDisplayAttachTemplate.templateName;
                wtDisplayRevision = wtDisplayAttachTemplate.templateRevision;
                wtDisplayDiscipline = wtDisplayAttachTemplate.templateDiscipline;
                wtDisplayInternalRevision = wtDisplayAttachTemplate.templateInternalRevision;
                wtDisplayDescription = wtDisplayAttachTemplate.templateDescription;
                try
                {
                    wtDisplayParentGuid = (Guid)wtDisplayAttachTemplate.templateWorkFlow.Value;
                    wtStageName = wtDisplayAttachTemplate.templateWorkFlow.Label;
                }
                catch
                {
                    wtDisplayParentGuid = Guid.Empty; //if problem exists attach to root by default
                }

                wtDisplayChecked = false;
                wtDisplayQRSupport = wtDisplayAttachTemplate.templateQRSupport;
                wtDisplaySkipApproved = wtDisplayAttachTemplate.templateSkipApproved;
            }
            else if (StoreObject.GetType() == typeof(Workflow))
            {
                wtDisplayAttachWorkflow = (Workflow)StoreObject;
                wtDisplayImageIndex = 0;
                wtDisplayGuid = wtDisplayAttachWorkflow.GUID;
                wtDisplayAttachmentName = wtDisplayAttachWorkflow.workflowName;
                //user prefer not to show workflow as hierarchical
                //wtDisplayParentGuid = wtDisplayAttachWorkflow.workflowParentGuid;
                wtDisplayParentGuid = Guid.Empty;
                //wtDisplayName = wtDisplayAttachWorkflow.workflowName;
                //wtDisplayAttachmentName = wtDisplayAttachWorkflow.workflowName;
                //wtDisplayDescription = wtDisplayAttachWorkflow.workflowDescription;
                wtDisplayChecked = false;
            }
        }

        public string wtDisplayAttachmentName { get; set; }
        public string wtDisplayAttachmentDescription { get; set; }
        public string wtDisplayDiscipline { get; set; }
        public string wtStageName { get; set; }
        public string wtTaskNumber { get; set; }
        public Guid wtDisplayGuid { get; set; }
        public Guid wtDisplayParentGuid { get; set; }
        public string wtDisplayName { get; set; }
        public string wtDisplayRevision { get; set; }
        public string wtDisplayDescription { get; set; }
        public Template wtDisplayAttachTemplate { get; set; }
        public Workflow wtDisplayAttachWorkflow { get; set; }
        public bool wtDisplayChecked { get; set; }
        public int wtDisplayImageIndex { get; set; }
        public bool wtDisplayQRSupport { get; set; }
        public bool wtDisplaySkipApproved { get; set; }
        public string wtDisplayInternalRevision { get; set; }
    }

    public class WorkflowTemplateTagWBS : wtDisplay
    {
        public WorkflowTemplateTagWBS(object storeObject)
            : base(storeObject)
        {
            wtEnabled = true;
        }
        public Guid wtTrueTemplateGuid { get; set; }
        public Guid wtITRGuid { get; set; }
        public Tag wtDisplayAttachTag { get; set; }
        public WBS wtDisplayAttachWBS { get; set; }
        public bool wtEnabled { get; set; }
        public string wtDisplayITRMeta { get; set; }
        public DateTime? wtCreatedDate { get; set; }
    }

    public class MatrixType : ParentClass
    {
        public MatrixType(Guid guid)
        {
            GUID = guid;
        }

        public string typeName { get; set; }
        public string typeDescription { get; set; }
        public string typeCategory { get; set; }
        public string typeDiscipline { get; set; }
    }

    /// <summary>
    /// Template for datagrid binding
    /// </summary>
    public class Template : ParentClass
    {
        public Template()
        { 
        }
        
        public Template(Guid guid)
        {
            GUID = guid;
        }

        public ValuePair templateWorkFlow { get; set; }
        public string templateName { get; set; }
        public string templateRevision { get; set; }
        public string templateDiscipline { get; set; }
        public string templateDescription { get; set; }
        public bool templateQRSupport { get; set; }
        public bool templateSkipApproved { get; set; }
        public string templateInternalRevision { get; set; }
    }

    public class Template_Check : Template
    {
        public Template_Check()
        {
            templateAssignmentGuid = Guid.Empty;
            templateSelected = false;
        }

        public Guid templateAssignmentGuid { get; set; }
        public bool templateSelected { get; set; }
    }

    /// <summary>
    /// ITR for datagrid binding
    /// </summary>
    public class ITR : ParentClass
    {
        public ITR(Guid guid)
        {
            GUID = guid;
        }

        public Guid ITRTagGuid { get; set; }
        public Guid ITRWBSGuid { get; set; }
        public Guid ITRTemplateGuid { get; set; }
        public string ITRName { get; set; }
        public string ITRDescription { get; set; }
        public string ITRRevision { get; set; }
    }

    /// <summary>
    /// Role for datagrid binding
    /// </summary>
    public class Role : ParentClass
    {
        public Role(Guid guid)
        {
            GUID = guid;
        }

        public Guid roleParentGuid { get; set; }
        public string roleParentName { get; set; }
        public string roleName { get; set; }
    }

    /// <summary>
    /// Privilege for datagrid binding
    /// </summary>
    public class Privilege : ParentClass
    {
        public Privilege(Guid guid)
        {
            GUID = guid;
        }

        public string privTypeID { get; set; }
        public string privName { get; set; }
        public string privCategory { get; set; }
        public bool privLocked { get; set; }
    }

    /// <summary>
    /// A simple model for datagrid binding
    /// </summary>
    public class Simple
    {
        public Simple(string Name)
        {
            simpleName = Name;
        }

        public string simpleName { get; set; }
    }

    /// <summary>
    /// Model for storing Tag number import status
    /// </summary>
    public class ImportStatus
    {
        public ImportStatus(string TagName)
        {
            tagName = TagName;
        }

        public string tagName { get; set; }
        public string tagStatus { get; set; }
    }

    /// <summary>
    /// User for datagrid binding
    /// </summary>
    public class User : ParentClass
    {
        public User(Guid UserGuid)
        {
            GUID = UserGuid;
        }

        public string userFirstName { get; set; }
        public string userLastName { get; set; }
        public string userQANumber { get; set; }
        public string userPassword { get; set; }
        public ValuePair userRole { get; set; }
        public string userEmail { get; set; }
        public string userCompany { get; set; }
        public ValuePair userProject { get; set; }
        public string userDiscipline { get; set; }
        public string userInfo { get; set; }
        public bool allDisciplinePermission { get; set; }
    }

    /// <summary>
    /// Project for datagrid binding
    /// </summary>
    public class Project : ParentClass
    {
        public Project(Guid ProjectGuid)
        {
            GUID = ProjectGuid;
        }

        public string projectNumber { get; set; }
        public string projectName { get; set; }
        public string projectClient { get; set; }
    }

    public class Sync_Status
    {
        public Sync_Status()
        {
            SyncStatus_Type = string.Empty;
            SyncStatus_Status = string.Empty;
            SyncStatus_Created = DateTime.MinValue;
            SyncStatus_CreatedBy = new ValuePair(string.Empty, Guid.Empty);
            SyncStatus_OverallPercentage = 0;
        }

        public string SyncStatus_Type { get; set; }
        public string SyncStatus_Status { get; set; }
        public int SyncStatus_Delete { get; set; }
        public int SyncStatus_Upload { get; set; }
        public int SyncStatus_Same { get; set; }
        public int SyncStatus_Conflict { get; set; }
        public int SyncStatus_Resolve { get; set; }
        public int SyncStatus_Download { get; set; }
        public DateTime SyncStatus_Created { get; set; }
        public ValuePair SyncStatus_CreatedBy { get; set; }
        public double SyncStatus_OverallPercentage { get; set; }
        public double SyncStatus_CurrentPercentage { get; set; }
        public DateTime SyncStatus_LastSynced { get; set; }

        public string SyncStatus_CreatedStr
        {
            get
            {
                if (SyncStatus_Created != DateTime.MinValue)
                    return SyncStatus_Created.ToString("g");
                else
                    return string.Empty;
            }
        }
    }

    /// <summary>
    /// Displaying sync status
    /// </summary>
    public class SyncStatus_Superseded : ParentClass
    {
        public SyncStatus_Superseded(Guid guid, Guid parentguid)
        {
            GUID = guid;
            parentGuid = parentguid;
            CreatedDate = DateTime.MinValue;
            UpdatedDate = DateTime.MinValue;
            DeletedDate = DateTime.MinValue;
            SyncDate = DateTime.MinValue;
            CreatedBy = Guid.Empty;
            UpdatedBy = Guid.Empty;
            DeletedBy = Guid.Empty;
            SyncBy = Guid.Empty;
        }

        public Guid parentGuid { get; set; }
        public string type { get; set; }
        public string additionalInfo { get; set; }
        public string syncItemName { get; set; }
        public string status { get; set; }
        public int imageIndex { get; set; }
        public DateTime SyncDate { get; set; }
        public Guid SyncBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedDateStr
        {
            get
            {
                if (UpdatedDate != DateTime.MinValue)
                    return UpdatedDate.ToString("g");
                else
                    return string.Empty;
            }
        }
        public Guid UpdatedBy { get; set; }
        public DateTime DeletedDate { get; set; }
        public string DeletedDateStr
        {
            get
            {
                if (DeletedDate != DateTime.MinValue)
                    return DeletedDate.ToString("g");
                else
                    return string.Empty;
            }
        }
        public Guid DeletedBy { get; set; }
    }

    /// <summary>
    /// Stores ITR sync information for local and remote servers
    /// </summary>
    public class SyncITR
    {
        public SyncITR(bool IsWBS)
        {
            isWBS = IsWBS;
        }

        public string templateName { get; set; }
        public string attachmentName { get; set; }
        public bool isWBS { get; set; }
        public Guid projectGuid { get; set; }
    }

    /// <summary>
    /// Stores Punchlist Sync information for local and remote servers
    /// </summary>
    public class SyncPunchlist
    {
        public SyncPunchlist(bool IsWBS)
        {
            isWBS = IsWBS;
        }

        public string punchlistTitle { get; set; }
        public string attachmentName { get; set; }
        public bool isWBS { get; set; }
        public Guid projectGuid { get; set; }
    }

    /// <summary>
    /// Store user drag data
    /// </summary>
    public class DragData : IDisposable
    {
        public BindingSource sourceData;
        public int[] dataRowIndexes;

        public DragData()
        {
            sourceData = null;
            dataRowIndexes = null;
        }
        public void Dispose()
        {
            if (sourceData != null)
            {
                sourceData.Dispose();
                sourceData = null;
            }
        }
    }

    /// <summary>
    /// ValuePair containing label for string representation of an object value
    /// </summary>
    public class ValuePair : IComparable
    {
        private string strLabel;
        private object objValue;

        public string Label
        {
            get
            {
                return strLabel;
            }
        }
        public object Value
        {
            get
            {
                return objValue;
            }
        }

        public ValuePair(string Label, object Value)
        {
            strLabel = Label;
            objValue = Value;
        }

        public override string ToString()
        {
            return strLabel;
        }

        public int CompareTo(object obj)
        {
            ValuePair compareObj = (ValuePair)obj;
            return string.Compare(this.Label.ToString(), compareObj.Label.ToString());
        }
    }

    public class PunchlistITRTemplate
    {
        private string strLabel;
        private Guid itrGuid;
        private Guid templateGuid;

        public string Label
        {
            get
            {
                return strLabel;
            }
        }

        public Guid ITRGuid
        {
            get
            {
                return itrGuid;
            }
        }

        public Guid TemplateGuid
        {
            get
            {
                return templateGuid;
            }
        }

        public PunchlistITRTemplate(string label, Guid itr_Guid, Guid template_Guid)
        {
            strLabel = label;
            itrGuid = itr_Guid;
            templateGuid = template_Guid;
        }

        public override string ToString()
        {
            return strLabel;
        }
    }

    public class Deletion
    {
        Guid id;
        string name;
        string type;

        ChildDeletionCollection childDeletion = new ChildDeletionCollection();

        public ChildDeletionCollection ChildDeletions { get { return childDeletion; } }
        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string Message { get; set; }
        public Guid ID { get { return id; } }

        public Deletion(Guid id, string type, string name)
        {
            this.name = name;
            this.type = type;
            this.id = id;
        }

        public void Add(ChildDeletion childDeletion)
        {
            ChildDeletions.Add(childDeletion);
        }
    }

    public class DeletionCollection : ArrayList, ITypedList
    {
        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (listAccessors != null && listAccessors.Length > 0)
            {
                PropertyDescriptor listAccessor = listAccessors[listAccessors.Length - 1];
                if (listAccessor.PropertyType.Equals(typeof(ChildDeletionCollection)))
                    return TypeDescriptor.GetProperties(typeof(ChildDeletion));
            }
            return TypeDescriptor.GetProperties(typeof(Deletion));
        }
        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return "Deletions";
        }
    }

    public class Client : ParentClass
    {
        public Client(Guid guid)
        {
            GUID = guid;
        }

        public string HWID { get; set; }
        public string IPAddress { get; set; }
        public string Description { get; set; }
        public bool Approved { get; set; }
        public string CreatedName { get; set; }
    }

    public class SyncOption
    {
        public SyncOption(int ID)
        {
            OptionID = ID;
        }

        public int OptionID { get; set; }
        public int OptionParentID { get; set; }
        public string OptionName { get; set; }
        public bool OptionEnabled { get; set; }
        public bool OptionOneTime { get; set; }
        public string OptionScope { get; set; }
    }

    public enum SyncScope
    {
        [Display(Name = "None")] None = 0,
        [Display(Name = "Global")] Global = 1,
        [Display(Name = "Project Specific")] Project = 2,
        [Display(Name = "Discipline Specific")] Discipline = 3
    }

    public class ChildDeletion
    {
        Guid parentID;
        string name;
        string type;

        public Guid ParentID { get { return parentID; } }
        public string Name { get { return name; } }
        public string Type { get { return type; } }
        public string Message { get; set; }
        public ChildDeletion(Guid parentID, string type, string name)
        {
            this.parentID = parentID;
            this.type = type;
            this.name = name;
        }
    }

    public class Template_Toggle : ParentClass
    {
        public Template_Toggle(Guid guid)
        {
            GUID = guid;
        }

        public string toggleDiscipline { get; set; }
        public string toggleName { get; set; }
        public string toggleDescription { get; set; }
        public bool toggleEnabled { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime DeletedDate { get; set; }
        public Guid DeletedBy { get; set; }
    }

    public class Conflict_Table
    {
        public string TableName { get; set; }
        public int ConflictCount { get; set; }
    }

    public class ITR_Refresh_Item
    {
        public ITR_Refresh_Item()
        {
            ITR_GUID = Guid.Empty;
        }

        public Guid WBSTagGuid { get; set; }
        public bool isWBS { get; set; }
        public Guid Template_GUID { get; set; }
        public Guid ITR_GUID { get; set; }
    }

    public class ChildDeletionCollection : ArrayList, ITypedList 
    {
        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return TypeDescriptor.GetProperties(typeof(ChildDeletion));
        }

        string ITypedList.GetListName(PropertyDescriptor[] listAccessors) 
        {
            return "ChildDeletions";
        }
    }

    /// <summary>
    /// Stores details relevant to syncing through web service
    /// </summary>
    public class WebServer_ITRSummary : SyncITR
    {
        public WebServer_ITRSummary(SyncITR iTRSync)
            : base(iTRSync.isWBS)
        {
            attachmentName = iTRSync.attachmentName;
            templateName = iTRSync.templateName;
            projectGuid = iTRSync.projectGuid;
        }

        public int itrStatus { get; set; }
        public int itrRejectCount { get; set; }
    }

    public class ViewModel_ProjectReport
    {
        //for creating project header/footer
        public string DummyGroup => "Dummy";
        public string Discipline { get; set; }
        public string AreaWBS_Name { get; set; }
        public string SystemWBS_Name { get; set; }
        public string SubsystemWBS_Name { get; set; }
        public string SubsystemWBS_Description { get; set; }
        public string Workflow_Name { get; set; }
        public int Total_Count
        {
            get => Pending_Count + Saved_Count + Inspected_Count + Approved_Count + Completed_Count + Closed_Count;
        }
        public int PendingSavedInspectedApprovedCount
        {
            get => Pending_Count + Saved_Count + Inspected_Count + Approved_Count;
        }
        public int Pending_Count { get; set; }
        public int Saved_Count { get; set; }
        public int Inspected_Count { get; set; }
        public int Approved_Count { get; set; }
        public int Completed_Count { get; set; }
        public int Closed_Count { get; set; }
        public string SubsystemFullName => SubsystemWBS_Name + " - " + SubsystemWBS_Description;
    }

    public class ViewModel_PunchlistReport
    {
        public string Discipline { get; set; }
        public string WBS_Name { get; set; }
        public string Workflow_Name { get; set; }
        public string ITR_Name { get; set; }
        public string Tag_Number { get; set; }
        public string Description { get; set; }
        public int Total_Count
        {
            get { return Saved_Count + Categorised_Count + Inspected_Count + Approved_Count + Completed_Count + Closed_Count; }
        }
        public int Saved_Count { get; set; }
        public int Categorised_Count { get; set; }
        public int Inspected_Count { get; set; }
        public int Approved_Count { get; set; }
        public int Completed_Count { get; set; }
        public int Closed_Count { get; set; }
    }

    public class ViewModel_PunchlistPriorityReport
    {
        public string WBS_DisplayMember => string.Concat(WBS_Name, " - ", WBS_Description);
        public string DummyGroup => "Dummy";
        public string Discipline { get; set; }
        public string WBS_Name { get; set; }
        public string WBS_Description { get; set; }
        public string Workflow_Name { get; set; }
        public string ITR_Name { get; set; }
        public string Tag_Number { get; set; }
        public int A_TOTAL { get; set; }
        public int A_CLOSED { get; set; }
        public int B_TOTAL { get; set; }
        public int B_CLOSED { get; set; }
        public int C_TOTAL { get; set; }
        public int C_CLOSED { get; set; }
        public int D_TOTAL { get; set; }
        public int D_CLOSED { get; set; }
        public int Total => A_TOTAL + B_TOTAL + C_TOTAL + D_TOTAL;
        public int Total_Closed => A_CLOSED + B_CLOSED + C_CLOSED + D_CLOSED;
    }

    public class ViewModel_ErrorMessage
    {
        public string Error { get; set; }
        public string Description { get; set; }
    }

    public class Punchlist_Score
    {
        public double TotalCount
        {
            get { return TotalSavedCount + TotalCategorisedCount + TotalInspectedCount + TotalApprovedCount + TotalCompletedCount + TotalClosedCount; }
        }

        public double TotalSavedCount { get; set; }
        public double TotalCategorisedCount { get; set; }
        public double TotalInspectedCount { get; set; }
        public double TotalApprovedCount { get; set; }
        public double TotalCompletedCount { get; set; }
        public double TotalClosedCount { get; set; }
        public double TotalScore
        {
            get { return ((TotalInspectedCount * 1) + (TotalApprovedCount * 2) + (TotalCompletedCount * 3) + (TotalClosedCount * 4)); }
        }
        public double Percentage
        {
            get
            {
                return (TotalScore / (TotalCount * 4));
            }
        }
    }

    public class ITR_Score
    {
        public double TotalAssignedCount { get; set; }
        public double TotalInspectedCount { get; set; }
        public double TotalApprovedCount { get; set; }
        public double TotalCompletedCount { get; set; }
        public double TotalClosedCount { get; set; }
        public double TotalScore
        {
            get { return ((TotalInspectedCount * 1) + (TotalApprovedCount * 2) + (TotalCompletedCount * 3) + (TotalClosedCount * 4)); }
        }
        public double Percentage
        {
            get
            {
                return (TotalScore / (TotalAssignedCount * 4));
            }
        }
    }

    public class ViewModel_ITRSummary
    {
        public string GuidKey { get; set; }
        public string Number { get; set; }
        public int Assigned { get; set; }
        public int Saved { get; set; }
        public int Inspected { get; set; }
        public int Approved { get; set; }
        public int Completed { get; set; }
        public int Closed { get; set; }

        public int Stage1Assigned { get; set; }
        public int Stage2Assigned { get; set; }
        public int Stage3Assigned { get; set; }
        public int Stage1Saved { get; set; }
        public int Stage2Saved { get; set; }
        public int Stage3Saved { get; set; }
        public int Stage1Inspected { get; set; }
        public int Stage2Inspected { get; set; }
        public int Stage3Inspected { get; set; }
        public int Stage1Approved { get; set; }
        public int Stage2Approved { get; set; }
        public int Stage3Approved { get; set; }
        public int Stage1Completed { get; set; }
        public int Stage2Completed { get; set; }
        public int Stage3Completed { get; set; }
        public int Stage1Closed { get; set; }
        public int Stage2Closed { get; set; }
        public int Stage3Closed { get; set; }
    }

    public class ViewModel_ProjectITRLatestStatus
    {
        public string Discipline { get; set; }
        public string Area_WBS_Name { get; set; }
        public string System_WBS_Name { get; set; }
        public string Subsystem_WBS_Name { get; set; }
        public string Tag_Number {get; set;}
        public string ITR_Name { get; set; }
        public string ITR_Description { get; set; }
        public string ITR_Status { get; set; }
        public string ITR_Status_Comments { get; set; }
        public DateTime Created { get; set; }
    }

    public class ViewModel_MasterITRReport
    {
        public string Stage { get; set; }
        public string TaskNumber { get; set; }
        public string Template { get; set; }
        public string TemplateDescription { get; set; }
        public string Discipline { get; set; }
        public string Tag { get; set; }
        public string TagDescription { get; set; }
        public string Area { get; set; }
        public string AreaDescription { get; set; }
        public string Subsystem { get; set; }
        public string SubsystemDescription { get; set; }
        public string Status { get; set; }
    }

    public class ViewModel_MasterITRStatusReport
    {
        public string Discipline { get; set; }
        public string Area { get; set; }
        public string System { get; set; }
        public string SubsystemName { get; set; }
        public string SubsystemDescription { get; set; }
        public string Stage { get; set; }
        public string TagNumber { get; set; }
        public string TemplateName { get; set; }
        public string TemplateDescription { get; set; }
        public DateTime? PendingDate { get; set; }
        public string PendingPerson { get; set; }
        public DateTime? SavedDate { get; set; }
        public string SavedPerson { get; set; }
        public DateTime? InspectedDate { get; set; }
        public string InspectedPerson { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedPerson { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string CompletedPerson { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string ClosedPerson { get; set; }
    }

    public class ViewModel_ProjectITRStatusByDate
    {
        public string Discipline { get; set; }
        public string WBS_Name { get; set; }
        public string Tag_Number { get; set; }
        public DateTime Created { get; set; }
        public int DisciplineRunning { get; set; }
        public int DisciplineTotal { get; set; }

        public double DisciplinePercentage
        {
            get
            {
                return (Convert.ToDouble(DisciplineRunning) / Convert.ToDouble(DisciplineTotal)) * 100;
            }
        }
    }

    public class ViewModel_ProjectPunchlistStatusByDate
    {
        public string Discipline { get; set; }
        public string WBS_Name { get; set; }
        public string Workflow_Name { get; set; }
        public string ITR_Name { get; set; }
        public string Tag_Number { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public int DisciplineRunning { get; set; }
        public int DisciplineTotal { get; set; }

        public double DisciplinePercentage
        {
            get
            {
                return (Convert.ToDouble(DisciplineRunning) / Convert.ToDouble(DisciplineTotal)) * 100;
            }
        }
    }

    public interface ICertificate
    {
        Guid GUID { get; set; }
        string Number { get; set; }
        string Description { get; set; }
        string Status { get; set; }
        int StatusNumber { get; set; }
        List<string> Disciplines { get; set; }
        List<string> Subsystems { get; set; }
        int ImageIndex { get; set; }

        string GetStatusDescription(int statusNumber);
    }

    public class ViewModel_Certificate : ParentClass, ICertificate
    {
        public ViewModel_Certificate()
        {
        }
        public string Number { get; set; }
        public string Description { get; set; }
        public List<string> Disciplines { get; set; }
        public List<string> Subsystems { get; set; }
        public string Status { get; set; }
        public int StatusNumber { get; set; }
        public int ImageIndex { get; set; }
        public string DelimitedDisciplines
        {
            get
            {
                string delimitedDisciplines = string.Empty;
                foreach (string discipline in Disciplines)
                {
                    delimitedDisciplines += string.Concat(discipline, ", ");
                }

                return delimitedDisciplines.Length > 2 ? delimitedDisciplines.Substring(0, delimitedDisciplines.Length - 2) : delimitedDisciplines;
            }
        }

        public string DelimitedSubsystems
        {
            get
            {
                string delimitedSubsystems = string.Empty;
                foreach (string subsystem in Subsystems)
                {
                    delimitedSubsystems += string.Concat(subsystem, ", ");
                }

                return delimitedSubsystems.Length > 2 ? delimitedSubsystems.Substring(0, delimitedSubsystems.Length - 2) : delimitedSubsystems;
            }
        }

        public virtual string GetStatusDescription(int statusNumber)
        {
            return string.Empty;
        }

        public override string ToString()
        {
            return Number;
        }
    }

    public class ViewModel_PunchlistWalkdown : ViewModel_Certificate
    {
        public ViewModel_PunchlistWalkdown()
        {

        }

        public ViewModel_PunchlistWalkdown(Guid guid)
        {
            GUID = guid;
        }

        public override string GetStatusDescription(int statusNumber)
        {
            return ((PunchlistWalkdown_Status)statusNumber).ToString();
        }
    }

    public class ViewModel_CVC : ViewModel_Certificate
    {
        public ViewModel_CVC()
        {

        }

        public ViewModel_CVC(Guid guid)
        {
            GUID = guid;
        }

        public override string GetStatusDescription(int statusNumber)
        {
            return ((CVC_Status)statusNumber).ToString();
        }

        public string CVC_Type { get; set; }
    }

    public class ViewModel_NOE : ViewModel_Certificate
    {
        public ViewModel_NOE()
        {

        }

        public ViewModel_NOE(Guid guid)
        {
            GUID = guid;
        }

        public override string GetStatusDescription(int statusNumber)
        {
            return ((NOE_Status)statusNumber).ToString();
        }
    }
}
