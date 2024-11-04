namespace ProjectDatabase.Dataset
{
}

namespace ProjectDatabase.Dataset.dsWBSTableAdapters
{
    public partial class WBSTableAdapter
    {
        public WBSTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}
namespace ProjectDatabase.Dataset
{


    partial class dsWBS
    {
        partial class WBSDisciplineDataTable
        {
        }
    }
}
