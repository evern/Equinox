namespace ProjectDatabase.Dataset
{
}
namespace ProjectDatabase.Dataset.dsTEMPLATE_TOGGLETableAdapters
{
    public partial class TEMPLATE_TOGGLETableAdapter
    {
        public TEMPLATE_TOGGLETableAdapter(string sqlConnection)
            : this()
        {
            Connection.ConnectionString = sqlConnection;
            Adapter.ContinueUpdateOnError = true;
        }
    }
}
