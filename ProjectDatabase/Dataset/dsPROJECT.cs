
namespace ProjectDatabase.Dataset.dsPROJECTTableAdapters
{
    public partial class PROJECTTableAdapter
    {
        public PROJECTTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}
namespace ProjectDatabase.Dataset {
    
    
    public partial class dsPROJECT {
    }
}
