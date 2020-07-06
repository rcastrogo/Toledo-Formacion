
namespace Dal.Core.Loader
{
  using System;

  public class BindItem
  {
    public string DomainFieldName;
    public int DbIndex;
    public Type DbType;

    public BindItem(string value)
    {
      string[] __tokens = value.Split(new char[] { ',' });
      DomainFieldName = __tokens[1].Trim();
      DbIndex = int.Parse(__tokens[0].Trim());
      if (__tokens.Length == 2)
      {
        DbType = Type.TypeString;
      }
      else
      {
        string str = __tokens[2].Trim().ToLower();
        if (str == "integer")
        {
          DbType = Type.TypeInteger;
        }
        else if (str == "double")
        {
          DbType = Type.TypeDouble;
        }
        else if ((str == "date") || (str == "datetime"))
        {
          DbType = Type.TypeDate;
        }
        else if (str == "decimal")
        {
          DbType = Type.TypeDecimal;
        }
        else if (str == "boolean")
        {
          DbType = Type.TypeBoolean;
        }
        else if (str == "string")
        {
          DbType = Type.TypeString;
        }
        else if (str == "byte()")
        {
          DbType = Type.TypeByteArray;
        }
      }
    }

    public BindItem(string fieldName, int dbIndex, Type dDbType)
    {
      DomainFieldName = fieldName;
      DbIndex = dbIndex;
      DbType = dDbType;
    }

    public enum Type
    {
      TypeInteger,
      TypeString,
      TypeDouble,
      TypeDecimal,
      TypeBoolean,
      TypeDate,
      TypeByteArray
    }
  }
}
