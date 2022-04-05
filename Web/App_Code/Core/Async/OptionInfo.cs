using System.Runtime.Serialization;

namespace Toledo.Core.Async
{

  [DataContract]
  public class OptionInfo
  {
    [DataMember(Name = "id")]
    public string Id;

    [DataMember(Name = "value")]
    public string Value = "";

    [DataMember(Name = "disabled", EmitDefaultValue = false, IsRequired = false)]
    public object Disabled;

    [DataMember(Name = "selected", EmitDefaultValue = false, IsRequired = false)]
    public object Selected;
  }

}