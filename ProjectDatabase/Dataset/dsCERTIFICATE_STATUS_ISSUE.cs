namespace ProjectDatabase.Dataset.dsCERTIFICATE_STATUS_ISSUETableAdapters
{
    public partial class CERTIFICATE_STATUS_ISSUETableAdapter
    {
        public CERTIFICATE_STATUS_ISSUETableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = false;
        }
    }
}

namespace ProjectDatabase.Dataset
{


    partial class dsCERTIFICATE_STATUS_ISSUE
    {
    }
}
