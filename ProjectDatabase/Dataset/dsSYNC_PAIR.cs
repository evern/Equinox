namespace ProjectDatabase.Dataset.dsSYNC_PAIRTableAdapters
{
    public partial class SYNC_PAIRTableAdapter
    {
        public SYNC_PAIRTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}

namespace ProjectDatabase.Dataset {
    
    
    public partial class dsSYNC_PAIR {
    }
}
