namespace ProjectDatabase.Dataset.dsPUNCHLIST_MAIN_PICTURETableAdapters
{
    public partial class PUNCHLIST_MAIN_PICTURETableAdapter
    {
        public PUNCHLIST_MAIN_PICTURETableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}