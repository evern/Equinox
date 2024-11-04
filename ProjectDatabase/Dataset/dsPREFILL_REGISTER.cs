namespace ProjectDatabase.Dataset
{
}
namespace ProjectDatabase.Dataset
{
}

namespace ProjectDatabase.Dataset.dsPREFILL_REGISTERTableAdapters
{
    public partial class PREFILL_REGISTERTableAdapter
    {
        public PREFILL_REGISTERTableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}