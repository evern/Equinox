namespace ProjectDatabase.Dataset.dsCERTIFICATE_STATUSTableAdapters
{
    public partial class CERTIFICATE_STATUSTableAdapter
    {
        public CERTIFICATE_STATUSTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = false;
        }
    }
}

namespace ProjectDatabase.Dataset
{


    partial class dsCERTIFICATE_STATUS
    {
    }
}
