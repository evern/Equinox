
namespace ProjectDatabase.Dataset.dsROLE_MAINTableAdapters
{
    public partial class ROLE_MAINTableAdapter
    {
        public ROLE_MAINTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }

    public partial class ROLE_PRIVILEGETableAdapter
    {
        public ROLE_PRIVILEGETableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}