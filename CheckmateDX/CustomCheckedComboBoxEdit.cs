using DevExpress.Accessibility;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CheckmateDX
{
    [UserRepositoryItem("RegisterCustomCheckedComboBoxEdit")]
    public class RepositoryItemCustomCheckedComboBoxEdit : RepositoryItemCheckedComboBoxEdit
    {
        static RepositoryItemCustomCheckedComboBoxEdit()
        {
            RegisterCustomCheckedComboBoxEdit();
        }

        public const string CustomEditName = "CustomCheckedComboBoxEdit";

        public RepositoryItemCustomCheckedComboBoxEdit()
        {
        }

        public override string EditorTypeName => CustomEditName;

        public static void RegisterCustomCheckedComboBoxEdit()
        {
            Image img = null;
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(CustomEditName, typeof(CustomCheckedComboBoxEdit), typeof(RepositoryItemCustomCheckedComboBoxEdit), typeof(PopupContainerEditViewInfo), new ButtonEditPainter(), true, img, typeof(CheckedComboBoxEditAccessible)));
        }

        public override void Assign(RepositoryItem item)
        {
            BeginUpdate();
            try
            {
                base.Assign(item);
                RepositoryItemCustomCheckedComboBoxEdit source = item as RepositoryItemCustomCheckedComboBoxEdit;
                if (source == null) return;
            }
            finally
            {
                EndUpdate();
            }
        }
    }

    [ToolboxItem(true)]
    public class CustomCheckedComboBoxEdit : DevExpress.XtraEditors.CheckedComboBoxEdit
    {
        public bool allowFireEditValueChanged = false;
        static CustomCheckedComboBoxEdit()
        {
            RepositoryItemCustomCheckedComboBoxEdit.RegisterCustomCheckedComboBoxEdit();
        }

        protected override void DoShowPopup()
        {
            allowFireEditValueChanged = true;
            base.DoShowPopup();
        }

        protected override bool AllowFireEditValueChanged => allowFireEditValueChanged;

        protected override void OnPopupClosed(PopupCloseMode closeMode)
        {
            allowFireEditValueChanged = false;
            base.OnPopupClosed(closeMode);
        }
        public CustomCheckedComboBoxEdit() { }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new RepositoryItemCustomCheckedComboBoxEdit Properties => base.Properties as RepositoryItemCustomCheckedComboBoxEdit;

        public override string EditorTypeName => RepositoryItemCustomCheckedComboBoxEdit.CustomEditName;
    }
}
