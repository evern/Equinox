namespace ProjectDatabase.Dataset
{
}
namespace ProjectDatabase.Dataset.dsSCHEDULETableAdapters
{
    public partial class SCHEDULETableAdapter
    {
        public SCHEDULETableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}
