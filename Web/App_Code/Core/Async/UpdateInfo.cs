using System.Runtime.Serialization;

namespace Toledo.Core.Async
{

  [DataContract]
  [KnownType(typeof(string[]))]
  [KnownType(typeof(OptionInfo[]))]
  public class UpdateInfo
  {
    [DataMember(Name = "id")]
    public string Id;

    [DataMember(Name = "value", EmitDefaultValue = false, IsRequired = false)]
    public object Value;

    [DataMember(Name = "disabled", EmitDefaultValue = false, IsRequired = false)]
    public object Disabled;

    [DataMember(Name = "readOnly", EmitDefaultValue = false, IsRequired = false)]
    public object ReadOnly;

    [DataMember(Name = "checked", EmitDefaultValue = false, IsRequired = false)]
    public object Checked;
  }

}