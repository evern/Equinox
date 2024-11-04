using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Commands;

namespace CheckmateDX
{
    public class CustomRichEditCommandFactoryService : RichEditCommandFactoryServiceWrapper
    {
        RichEditControl _control { get; set; }
        List<RichEditCommandDelegateContainer> _delegateContainer { get; set; }
        public CustomRichEditCommandFactoryService(IRichEditCommandFactoryService Service, RichEditControl Control, List<RichEditCommandDelegateContainer> DelegateContainer)
            : base(Service)
        {
            _control = Control;
            _delegateContainer = DelegateContainer;
        }

        public override RichEditCommand CreateCommand(RichEditCommandId id)
        {
            RichEditCommand command = base.CreateCommand(id);
            RichEditCommandDelegate delegateMethod;
            if (id == RichEditCommandId.FileSave)
            {
                delegateMethod = SearchDelegate(id);
                if (delegateMethod != null)
                    return new RichEditSaveCommand(_control, delegateMethod);
            }
            return command;
        }

        private RichEditCommandDelegate SearchDelegate(RichEditCommandId searchId)
        {
            RichEditCommandDelegateContainer searchDelegateContainer = _delegateContainer.FirstOrDefault(obj => obj.commandDelegateSearchId == searchId);
            if (searchDelegateContainer != null)
                return searchDelegateContainer.commandDelegateMethod;
            else
                return null;
        }
    }

    public class RichEditSaveCommand : SaveDocumentCommand
    {
        RichEditCommandDelegate _delegateMethod;
        public RichEditSaveCommand(IRichEditControl control, RichEditCommandDelegate DelegateMethod)
            : base(control)
        {
            _delegateMethod = DelegateMethod;
        }
        public override void Execute()
        {
            _delegateMethod();
        }
    }

    #region Delegate
    public delegate void RichEditCommandDelegate();
    public class RichEditCommandDelegateContainer
    {
        public RichEditCommandDelegateContainer(RichEditCommandId SearchId, RichEditCommandDelegate DelegateMethod)
        {
            commandDelegateSearchId = SearchId;
            commandDelegateMethod = DelegateMethod;
        }

        public RichEditCommandId commandDelegateSearchId { get; set; }
        public RichEditCommandDelegate commandDelegateMethod { get; set; }
    }
    #endregion
}
