namespace ProjectDatabase.Dataset.dsITR_MAINTableAdapters
{
    public partial class ITR_MAINTableAdapter
    {
        public ITR_MAINTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = false;
        }
    }
}
namespace ProjectDatabase.Dataset
{


    public partial class dsITR_MAIN
    {
    }
}
