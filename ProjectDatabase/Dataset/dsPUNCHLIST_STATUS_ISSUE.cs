namespace ProjectDatabase.Dataset.dsPUNCHLIST_STATUS_ISSUETableAdapters
{
    public partial class PUNCHLIST_STATUS_ISSUETableAdapter
    {
        public PUNCHLIST_STATUS_ISSUETableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}
