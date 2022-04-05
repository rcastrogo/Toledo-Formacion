using System.Runtime.Serialization;

namespace Toledo.Core.Async
{

  [DataContract]
  public class CallbackRequest
  {
    [DataMember(Name = "method")]
    public string Method = "";

    [DataMember(Name = "params")]
    public string[] Params = new string[] { };
  }

}