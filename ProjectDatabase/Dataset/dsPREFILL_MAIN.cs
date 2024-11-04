namespace ProjectDatabase.Dataset.dsPREFILL_MAINTableAdapters
{
    public partial class PREFILL_MAINTableAdapter
    {
        public PREFILL_MAINTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}
namespace ProjectDatabase.Dataset {
    
    
    public partial class dsPREFILL_MAIN {
    }
}
