
namespace Dal.Core
{
  using System.Collections.Generic;
  using System.Linq;

  public class ParameterContainer
  {
    private readonly Dictionary<string, string> __params;

    #region constructor

    public ParameterContainer()
    {
      __params = new Dictionary<string, string>();
    }

    #endregion

    public ParameterContainer Add(string key, string value)
    {
      __params.Add(key, value);
      return this;
    }

    public ParameterContainer AddRange((string, string)[] values)
    {
      foreach (var value in values)
      {
        __params.Add(value.Item1, value.Item2);
      }
      return this;
    }

    public Dictionary<string, string> ToDictionary()
    {
      return __params;
    }

    public QueryBuilder ToQueryBuilder()
    {
      return new QueryBuilder(__params);
    }

  }

}

