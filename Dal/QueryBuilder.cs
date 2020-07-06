
namespace Dal.Core
{
  using Dal.Utils;
  using System;
  using System.Collections.Generic;
  using System.Text;

  public class QueryBuilder
  {

    private readonly StringBuilder __stringBuider ;
    private readonly Dictionary<string, string> __params;

    #region constructor

    public QueryBuilder()
    {
      __stringBuider = new StringBuilder();
      __params = new Dictionary<string, string>();
    }

    public QueryBuilder (string key, string value) : this(new Dictionary<string, string> { { key, value } })
    {
    }

    public QueryBuilder(Dictionary<string, string> @params) : this()
    {     
      __params = @params;
    }

    #endregion

    #region Queryparams

    public QueryBuilder Clear()
    {
      __stringBuider.Clear();
      __params.Clear();

      return this;
    }

    public QueryBuilder UseParam(string key, string value)
    {
      __params.Add(key, value);
      return this;
    }

    public string ValueOf(string key)
    {
      return __params.ContainsKey(key) ? __params[key].Trim() : "";
    }

    #endregion

    #region Properties

    public string Locator = "";

    public int Length
    {
      get
      {
        return __stringBuider.Length;
      }
    }

    #endregion

    #region methods

    public string ToQueryString()
    {
      return __stringBuider.ToString().Trim();
    }

    public string ParseListOfIntegers(string value)
    {
      return ParseListOfIntegers(value, '-');
    }

    public string ParseListOfIntegers(string value, char separator)
    {
      string[] array = value.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);

      if (array.Length == 0) return "";
      try
      {
        return string.Join(",", Array.ConvertAll<string, string>(array, s => int.Parse(s).ToString()));
      }
      catch (Exception)
      {
        return "";
      }
    }

    public string ParseListOfStrings(string value)
    {
      return ParseListOfStrings(value, '-');
    }

    public string ParseListOfStrings(string value, char separator)
    {
      string[] array = value.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);

      if (array.Length == 0) return "";
      try
      {
        return string.Join(",", Array.ConvertAll<string, string>(array, s => "'" + s + "'"));
      }
      catch (Exception)
      {
        return "";
      }
    }

    #endregion

    #region Filter methods

    public QueryBuilder AndDate(string fieldName)
    {
      if (ValueOf(fieldName) != "")
      {
        __stringBuider.Append(__and(string.Format("CAST ({0} AS DATE )= {1}",
                                                  fieldName,
                                                  Helper.ParseString(__params[fieldName]))));
      }
      return this;
    }

    public QueryBuilder AndInteger(string key)
    {
      if (ValueOf(key) != "")
      {
        __stringBuider.Append(__and(string.Format("{0}={1}",
                                                  key,
                                                  int.Parse(__params[key]))));
      }
      return this;
    }

    public QueryBuilder AndInteger(string key, string query)
    {
      if (ValueOf(key) != "")
      {
        __stringBuider.Append(__and(string.Format(query, int.Parse(__params[key]))));
      }
      return this;
    }

    public QueryBuilder AndListOfIntegers(string key, string fieldName, char separator = '-')
    {
      if (ValueOf(key) != "")
      {
        __stringBuider.Append(__and(string.Format("{0} IN ({1})",
                                                   fieldName,
                                                   ParseListOfIntegers(__params[key], separator))));
      }
      return this;
    }

    public QueryBuilder AndListOfStrings(string key, string fieldName, char separator = '-')
    {
      if (ValueOf(key) != "")
      {
        __stringBuider.Append(__and(string.Format("{0} IN ({1})",
                                                   fieldName,
                                                   ParseListOfStrings(__params[key], separator))));
      }
      return this;
    }

    public QueryBuilder AndReplace(string key, string query)
    {
      if (ValueOf(key) != "")
      {
        __stringBuider.Append(__and(string.Format(query, Helper.ParseSqlInjection(__params[key]))));
      }
      return this;
    }

    public QueryBuilder AndSentence(string sentence)
    {
      __stringBuider.Append(__and(sentence));
      return this;
    }

    public QueryBuilder AndString(string key)
    {
      if (ValueOf(key) != "")
      {
        __stringBuider.Append(__and(string.Format("{0}={1}",
                                                  key,
                                                  Helper.ParseString(__params[key]))));
      }
      return this;
    }

    public QueryBuilder AndString(string key, string fieldName)
    {
      if (ValueOf(key) != "")
      {
        __stringBuider.Append(__and(string.Format("{0}='{1}'",
                                                  fieldName,
                                                  Helper.ParseSqlInjection(__params[key]))));
      }
      return this;
    }

    public QueryBuilder AndStringLike(string key)
    {
      if (ValueOf(key) != "")
      {
        __stringBuider.Append(__and(string.Format("{0} LIKE '%{1}%'",
                                                  key,
                                                  Helper.ParseSqlInjection(__params[key]))));
      }
      return this;
    }

    public QueryBuilder AndStringLike(string key, string fieldName)
    {
      if (ValueOf(key) != "")
      {
        __stringBuider.Append(__and(string.Format("{0} LIKE '%{1}%'",
                                                  fieldName,
                                                  Helper.ParseSqlInjection(__params[key]))));
      }
      return this;
    }

    private string __and(string value)
    {
      return __stringBuider.Length == 0 ? value : (" AND " + value);
    }

    #endregion

  }
}
