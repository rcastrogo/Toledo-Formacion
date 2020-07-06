
namespace Dal.Core.Connections
{
  using System.Data;

  public interface IConnectionBuilder
  {
    IDbConnection CreateConnection();
    IDbConnection CreateConnection(string connectionString);
  }

}
