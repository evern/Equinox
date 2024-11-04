namespace ProjectDatabase.Dataset.dsELECTRICAL_EQUIPMENTTableAdapters
{
    public partial class ELECTRICAL_EQUIPMENTTableAdapter
    {
        public ELECTRICAL_EQUIPMENTTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}