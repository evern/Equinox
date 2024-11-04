namespace ProjectDatabase.Dataset
{
}
namespace ProjectDatabase.Dataset
{
}
namespace ProjectDatabase.Dataset.dsSYNC_RECORDTableAdapters
{
    public partial class SYNC_RECORDTableAdapter
    {
        public SYNC_RECORDTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}