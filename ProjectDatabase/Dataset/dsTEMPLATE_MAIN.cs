
namespace ProjectDatabase.Dataset.dsTEMPLATE_MAINTableAdapters
{
    public partial class TEMPLATE_MAINTableAdapter
    {
        public TEMPLATE_MAINTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}
namespace ProjectDatabase.Dataset
{


    public partial class dsTEMPLATE_MAIN
    {
    }
}
