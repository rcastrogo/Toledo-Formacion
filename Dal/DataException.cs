
namespace Dal.Core
{
  using System;

  [Serializable]
  public class DataException : Exception
  {
    public DataException(string message, System.Data.DataException innerException) : base(message, innerException)
    {

    }
  }
}

