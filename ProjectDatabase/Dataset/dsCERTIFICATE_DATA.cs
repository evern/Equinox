namespace ProjectDatabase.Dataset
{
}

namespace ProjectDatabase.Dataset.dsCERTIFICATE_DATATableAdapters
{
    public partial class CERTIFICATE_DATATableAdapter
    {
        public CERTIFICATE_DATATableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = false;
        }
    }
}