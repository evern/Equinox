namespace ProjectDatabase.Dataset.dsUSER_MAINTableAdapters
{
    public partial class USER_MAINTableAdapter
    {
        public USER_MAINTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }

    public partial class USER_PROJECTTableAdapter
    {
        public USER_PROJECTTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }

    public partial class USER_DISCTableAdapter
    {
        public USER_DISCTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}

namespace ProjectDatabase.Dataset {
    
    
    public partial class dsUSER_MAIN {
    }
}
