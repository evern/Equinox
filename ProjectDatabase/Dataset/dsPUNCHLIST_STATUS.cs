namespace ProjectDatabase.Dataset
{
}
namespace ProjectDatabase.Dataset.dsPUNCHLIST_STATUSTableAdapters
{
    public partial class PUNCHLIST_STATUSTableAdapter
    {
        public PUNCHLIST_STATUSTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}
