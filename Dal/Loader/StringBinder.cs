
namespace Dal.Core.Loader
{
  using System;

  public class StringBinder
  {
    public string Value;
    public string Key;

    public StringBinder(string key, string value)
    {
      Value = value;
      Key = key;
    }
  }

}
