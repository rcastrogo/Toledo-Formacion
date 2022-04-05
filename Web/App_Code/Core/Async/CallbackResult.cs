using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Toledo.Core.Async
{
  [DataContract]
  [KnownType(typeof(OptionInfo[]))]
  public class CallbackResult
  {
    [DataMember(Name = "data")]
    public string Data = "";

    [DataMember(Name = "viewState")]
    public string ViewState = "";

    [DataMember(Name = "error")]
    public CallbackError Error;

    [DataMember(Name = "messages")]
    public List<string> Messages = new List<string>();

    [DataMember(Name = "clientCommands")]
    public List<CallbackClientCommand> ClientCommands = new List<CallbackClientCommand>();

    [DataMember(Name = "controls")]
    public List<UpdateInfo> Controls;
  }

}