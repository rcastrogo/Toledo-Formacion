
namespace Dal.Core
{
  using Dal.Core.Connections;
  using System;
  using System.Data;
  using System.Data.SqlClient;
  using System.Diagnostics;

  public class DbContext : IDisposable
  {
           
    public DbContext() : base() { }

    public DbContext(string connectionString)
    {
      try
      {
        _Connection = ConnectionManager.CreateConnection(connectionString);
      }
      catch (System.Data.DataException ex)
      {
        throw new Dal.Core.DataException("No se ha podido establecer la conexión con la base de datos", ex);
      }
    }


    #region destructor

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
      {
        _Connection = null;
      }
      else if (_Connection != null)
      {
        _Connection.Close();
        _Connection.Dispose();
        _Connection = null;
      }
    }

    #endregion

    #region connections

    private IDbConnection _Connection = null; 
    private IDbConnection CurrentConnection
    {
      get
      {
        if (_Connection == null)
        {
          try
          {
            _Connection = ConnectionManager.CreateConnection();
          }
          catch (System.Data.DataException e)
          {
            throw new DataException("No se ha podido establecer la conexión con la base de datos", e);
          }
        }
        return _Connection;
      }
    }

    #endregion

    #region bulkCopy

    public void BulkCopy(string destinationTableName, IDataReader dataReader, string[] mapInfo)
    {
      BulkCopy(destinationTableName, dataReader, mapInfo, SqlBulkCopyOptions.TableLock);
    }

    public void BulkCopy(string destinationTableName, IDataReader dataReader, string[] mapInfo, SqlBulkCopyOptions options)
    {
      using (SqlBulkCopy sqlBulkCopy = CreateSqlBulkCopy(options))
      {
        foreach (string __map in mapInfo)
        {
          string[] __tokens = __map.Split(new char[] { '|' });
          sqlBulkCopy.ColumnMappings.Add(__tokens[0], __tokens[1]);
        }
        sqlBulkCopy.DestinationTableName = destinationTableName;
        sqlBulkCopy.BulkCopyTimeout = 0;
        sqlBulkCopy.BatchSize = 5000;
        Trace.WriteLineIf(SqlEngine.TraceSQLStatements, string.Format("BulkCopy.Begin -> {0}", destinationTableName));
        sqlBulkCopy.WriteToServer(dataReader);
        Trace.WriteLineIf(SqlEngine.TraceSQLStatements, string.Format("{0} rows", dataReader.RecordsAffected));
        Trace.WriteLineIf(SqlEngine.TraceSQLStatements, "BulkCopy.End");
      }
    }

    public SqlBulkCopy CreateSqlBulkCopy(SqlBulkCopyOptions options)
    {
      return new SqlBulkCopy((SqlConnection)_Connection, options, (SqlTransaction)_Transaction);
    }

    #endregion

    #region transactions

    private IDbTransaction _Transaction = null;

    public void BeginTransaction()
    {
      _Transaction = CurrentConnection.BeginTransaction();
      Trace.WriteLineIf(SqlEngine.TraceSQLStatements, "Dal --> BeginTransaction");
    }

    public void Commit()
    {
      if (_Transaction == null)
      {
        Trace.WriteLineIf(SqlEngine.TraceSQLStatements, "Dal --> Commit: No existe Transacción en curso");
      }
      else
      {
        _Transaction.Commit();
        _Transaction.Dispose();
        _Transaction = null;
        Trace.WriteLineIf(SqlEngine.TraceSQLStatements, "Dal --> Commit");
      }
    }

    public void Rollback()
    {
      if (_Transaction == null)
      {
        Trace.WriteLineIf(SqlEngine.TraceSQLStatements, "Dal --> Rollback : No existe Transacción en curso");
      }
      else
      {
        _Transaction.Rollback();
        _Transaction.Dispose();
        _Transaction = null;
        Trace.WriteLineIf(SqlEngine.TraceSQLStatements, "Dal --> Rollback");
      }
    }

    #endregion

    #region commands

    internal IDbCommand CreateCommand()
    {
      IDbCommand command = CurrentConnection.CreateCommand();
      command.Connection = _Connection;
      command.Transaction = _Transaction;
      return command;
    }

    public int ExecuteNonQuery(string query)
    {
      using (IDbCommand cmd = CreateCommand())
      {
        cmd.CommandText = query;
        return SqlEngine.ExecuteNonQuery(cmd);
      }
    }

    public int ExecuteNonQuery(string query, string[] values)
    {
      using (IDbCommand cmd = CreateCommand())
      {
        cmd.CommandText = ((values != null) && (values.Length != 0)) ? string.Format(query, (object[])values)
                                                                     : query;
        return SqlEngine.ExecuteNonQuery(cmd);
      }
    }

    public IDataReader ExecuteReader(string query)
    {
      using (IDbCommand cmd = CreateCommand())
      {
        cmd.CommandText = query;
        return SqlEngine.ExecuteReader(cmd);
      }      
    }

    public IDataReader ExecuteReader(string query, string[] values)
    {
      using (IDbCommand cmd = CreateCommand())
      {
        cmd.CommandText = ((values != null) && (values.Length != 0)) ? string.Format(query, (object[])values)
                                                                     : query;
        return SqlEngine.ExecuteReader(cmd);
      }
    }

    public T ExecuteScalar<T>(string query)
    {
      using (IDbCommand cmd = CreateCommand())
      { 
        cmd.CommandText = query;
        return SqlEngine.ExecuteScalar<T>(cmd);
      }
    }

    public T ExecuteScalar<T>(string query, string[] values)
    {
      using(IDbCommand cmd = CreateCommand())
      {
        cmd.CommandText = ((values != null) && (values.Length != 0)) ? string.Format(query, (object[])values)
                                                                     : query;
        return SqlEngine.ExecuteScalar<T>(cmd);
      }     
    }

    #endregion

  }

}
