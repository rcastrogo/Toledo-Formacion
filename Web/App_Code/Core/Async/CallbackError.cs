using System.Runtime.Serialization;

namespace Toledo.Core.Async
{

  [DataContract]
  public class CallbackError
  {
    [DataMember(Name = "code")]
    public int Code;

    [DataMember(Name = "message")]
    public string Message = "";
  }

}