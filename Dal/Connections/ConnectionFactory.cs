
namespace Dal.Core.Connections
{

  using System.Data;

  public sealed class ConnectionManager
  {
    private static readonly object _object = new object();
    private static ConnectionManager _instance = null;
    private static IConnectionBuilder _connectionBuilder = null;

    private ConnectionManager()
    {
    }

    public static IDbConnection CreateConnection()
    {
      if (_instance == null)
      {
        lock (_object)
        {
          if (_instance == null)
          {
            _instance = new ConnectionManager();
            System.Diagnostics.Trace.WriteLine("DAL --> ConnectionBuilder : SqlServerConnectionBuilder");
            _connectionBuilder = new Connections.Builders.SqlServerConnectionBuilder();
          }
          return _connectionBuilder.CreateConnection();
        }
      }
      else
      {
        lock (_object)
        {
          return _connectionBuilder.CreateConnection();
        }
      }
    }

    public static IDbConnection CreateConnection(string connectionString)
    {
      if (_instance == null)
      {
        lock (_object)
        {
          if (_instance == null)
          {
            _instance = new ConnectionManager();
            _connectionBuilder = new Connections.Builders.SqlServerConnectionBuilder();
          }
          return _connectionBuilder.CreateConnection(connectionString);
        }
      }
      else
      {
        lock (_object)
        {
          return _connectionBuilder.CreateConnection(connectionString);
        }
      }
    }
  }
}