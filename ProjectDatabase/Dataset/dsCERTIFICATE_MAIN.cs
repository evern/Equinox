namespace ProjectDatabase.Dataset.dsCERTIFICATE_MAINTableAdapters
{
    public partial class CERTIFICATE_MAINTableAdapter
    {
        public CERTIFICATE_MAINTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = false;
        }
    }
}

namespace ProjectDatabase.Dataset
{
    partial class dsCERTIFICATE_MAIN
    {
        partial class NOESubsystemCertificateDataTable
        {
        }

        partial class CERTIFICATE_MAIN_STATUSDataTable
        {
        }
    }
}
