
namespace Dal.Core.Connections.Builders
{
  using Dal.Core.Connections;
  using System;
  using System.Collections;
  using System.Data;
  using System.Data.SqlClient;
  using System.Diagnostics;
  using System.Web.Configuration;

  public class SqlServerConnectionBuilder : IConnectionBuilder
  {
    private static Hashtable _hash = new Hashtable();
    
    public IDbConnection CreateConnection()
    {
      SqlConnection connection = new SqlConnection(WebConfigurationManager.AppSettings["ConnectionString"]);
      connection.Open();
      _hash.Add(connection.GetHashCode(), DateTime.Now);
      connection.Disposed += new EventHandler(SqlServerConnectionBuilder.OnDisposeConnection);
      Trace.WriteLineIf(SqlEngine.TraceSQLStatements, string.Format("DAL --> CreateConnection : {0}", connection.GetHashCode()));
      return connection;
    }

    public System.Data.IDbConnection CreateConnection(string connectionString)
    {
      SqlConnection _connection = new SqlConnection(connectionString);
      _connection.Open();
      _hash.Add(_connection.GetHashCode(), DateTime.Now);
      _connection.Disposed += OnDisposeConnection;
      Trace.WriteLineIf(SqlEngine.TraceSQLStatements, string.Format("DAL --> CreateConnection (connectionString) : {0}", _connection.GetHashCode()));
      return _connection;
    }


    private static void OnDisposeConnection(object sender, EventArgs e)
    {
      Trace.WriteLineIf(SqlEngine.TraceSQLStatements, string.Format("DAL --> DisposeConnection : {0} {1:0.000} milliseconds",
                                                        sender.GetHashCode(),
                                                        (DateTime.Now - ((DateTime)_hash[sender.GetHashCode()]))
                                                        .TotalMilliseconds));
      _hash.Remove(sender.GetHashCode());
    }
  }
}
