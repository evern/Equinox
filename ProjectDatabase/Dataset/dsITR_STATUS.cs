namespace ProjectDatabase.Dataset.dsITR_STATUSTableAdapters
{
    public partial class ITR_STATUSTableAdapter
    {
        public ITR_STATUSTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}
namespace ProjectDatabase.Dataset {
    
    
    public partial class dsITR_STATUS {
    }
}
