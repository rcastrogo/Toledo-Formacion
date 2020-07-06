
namespace Dal.Utils
{
  using System;

  public sealed class Helper
  {
    public static string FormatByteArray(byte[] value)
    {
      return ("0x" + BitConverter.ToString(value).Replace("-", ""));
    }

    public static string ParseString(string value)
    {
      return string.IsNullOrWhiteSpace(value) ? "NULL" : string.Format("'{0}'", value.Replace("'", "''"));
    }

    public static string ParseString(string value, int length)
    {
      return ParseString(Truncate(value, length));
    }

    public static string ParseDate(string value)
    {
      if (string.IsNullOrWhiteSpace(value)) return "NULL";
      try
      {
        return DateTime.Parse(value).ToString(@"\'yyyyMMdd HH:mm:ss\'");
      }
      catch
      {
        return "NULL";
      }
    }

    public static string ParseNumber(string value)
    {
      return string.IsNullOrWhiteSpace(value) ? "NULL" : value.Replace(',', '.');
    }

    public static string ParseSqlInjection(string value)
    {
      return string.IsNullOrWhiteSpace(value) ? "" : value.Replace("'", "''");
    }

    public static string Truncate( string value, int length)
    {
      if(string.IsNullOrWhiteSpace(value)) return "";
      return value.Substring(0, Math.Min(value.Length, length));
    }
  }
}
