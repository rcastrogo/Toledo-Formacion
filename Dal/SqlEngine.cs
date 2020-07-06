
namespace Dal.Core
{
  using System;
  using System.Data;
  using System.Diagnostics;

  public abstract class SqlEngine
  {
    internal delegate T CommandHandler<T>(IDbCommand cmd);
    internal static Boolean TraceSQLStatements = true;

    private SqlEngine() { }

    private static int DoExecuteNonQuery(IDbCommand cmd)
    {
      int result = cmd.ExecuteNonQuery();
      if (TraceSQLStatements)
      {
        Trace.WriteLine(String.Format("{0} {1} filas",
                                      cmd.Transaction != null ? "Dal --> Transaction"
                                                              : "DAL",
                                      result));
      }
      return result;
    }

    private static IDataReader DoExecuteReader(IDbCommand cmd) { 
      return cmd.ExecuteReader();
    }

    private static T DoExecuteScalar<T>(IDbCommand cmd) { 
      return (T)cmd.ExecuteScalar(); 
    }

    private static T ExecuteCommand<T>(IDbCommand cmd, CommandHandler<T> handler)
    {
      T local;
      try
      {
        if (TraceSQLStatements)
        {
          Trace.WriteLine(String.Format("{0}ExecuteCommand : {1}", 
                                        cmd.Transaction != null ? "Dal --> Transaction."
                                                                : "DAL.",
                                        cmd.CommandText));          
        }
        local = handler(cmd);
      }
      catch (System.Data.DataException e)
      {
        Trace.WriteLine("--- Exception : " + e.Message);
        throw new DataException("Error inesperado al acceder a los datos.", e);

      }
      return local;
    }

    internal static int ExecuteNonQuery(IDbCommand cmd) { 
      return ExecuteCommand<int>(cmd, new CommandHandler<int>(DoExecuteNonQuery)); 
    }
    internal static IDataReader ExecuteReader(IDbCommand cmd) { 
      return ExecuteCommand<IDataReader>(cmd, new CommandHandler<IDataReader>(DoExecuteReader));
    }
    internal static T ExecuteScalar<T>(IDbCommand cmd) { 
      return ExecuteCommand<T>(cmd, new CommandHandler<T>(DoExecuteScalar<T>));
    }

  }
}
