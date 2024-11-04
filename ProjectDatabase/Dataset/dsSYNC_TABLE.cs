namespace ProjectDatabase.Dataset.dsSYNC_TABLETableAdapters
{
    public partial class SYNC_TABLETableAdapter
    {
        public SYNC_TABLETableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}

namespace ProjectDatabase.Dataset {
    
    
    public partial class dsSYNC_TABLE {
    }
}
