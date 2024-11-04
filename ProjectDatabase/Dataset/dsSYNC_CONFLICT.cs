namespace ProjectDatabase.Dataset.dsSYNC_CONFLICTTableAdapters
{
    public partial class SYNC_CONFLICTTableAdapter
    {
        public SYNC_CONFLICTTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}