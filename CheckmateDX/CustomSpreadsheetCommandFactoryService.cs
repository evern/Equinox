using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Commands;

namespace CheckmateDX
{
    public class CustomSpreadsheetCommandFactoryService : SpreadsheetCommandFactoryServiceWrapper
    {
        SpreadsheetControl _control { get; set; }
        List<SpreadsheetCommandDelegateContainer> _delegateContainer { get; set; }
        public CustomSpreadsheetCommandFactoryService(ISpreadsheetCommandFactoryService Service, SpreadsheetControl Control, List<SpreadsheetCommandDelegateContainer> DelegateContainer)
            : base(Service)
        {
            _control = Control;
            _delegateContainer = DelegateContainer;
        }

        public override SpreadsheetCommand CreateCommand(SpreadsheetCommandId id)
        {
            SpreadsheetCommand command = base.CreateCommand(id);
            SpreadsheetCommandDelegate delegateMethod;
            if (id == SpreadsheetCommandId.FileSave)
            {
                delegateMethod = SearchDelegate(id);
                if (delegateMethod != null)
                    return new SpreadsheetSaveCommand(_control, delegateMethod);
            }
            return command;
        }

        private SpreadsheetCommandDelegate SearchDelegate(SpreadsheetCommandId searchId)
        {
            SpreadsheetCommandDelegateContainer searchDelegateContainer = _delegateContainer.FirstOrDefault(obj => obj.commandDelegateSearchId == searchId);
            if (searchDelegateContainer != null)
                return searchDelegateContainer.commandDelegateMethod;
            else
                return null;
        }
    }

    public class SpreadsheetSaveCommand : SaveDocumentCommand
    {
        SpreadsheetCommandDelegate _delegateMethod;
        public SpreadsheetSaveCommand(ISpreadsheetControl control, SpreadsheetCommandDelegate DelegateMethod)
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
    public delegate void SpreadsheetCommandDelegate();
    public class SpreadsheetCommandDelegateContainer
    {
        public SpreadsheetCommandDelegateContainer(SpreadsheetCommandId SearchId, SpreadsheetCommandDelegate DelegateMethod)
        {
            commandDelegateSearchId = SearchId;
            commandDelegateMethod = DelegateMethod;
        }

        public SpreadsheetCommandId commandDelegateSearchId { get; set; }
        public SpreadsheetCommandDelegate commandDelegateMethod { get; set; }
    }
    #endregion
}
