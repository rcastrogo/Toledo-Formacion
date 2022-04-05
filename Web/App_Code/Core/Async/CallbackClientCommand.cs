using System.Runtime.Serialization;

namespace Toledo.Core.Async
{

  [DataContract]
  public class CallbackClientCommand
  {
    [DataMember(Name = "method")]
    public string Method = "";

    [DataMember(Name = "params")]
    public object[] Params = new object[] { };

    [DataMember(Name = "body")]
    public string Body = "";
  }

}