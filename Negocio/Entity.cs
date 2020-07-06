
namespace Negocio.Core
{
  using Dal.Core;
  using System;
  using System.Xml.Serialization;

  [Serializable]
  public abstract class Entity
  {
    public delegate string GetExternalData(string key, Entity value);

    public Entity(){ }

    public Entity(DbContext context)
    {
      _dataContext = context;
    }

    public abstract int Id { get; set; }

    [NonSerialized, XmlIgnore]
    public GetExternalData DataProvider;

    [NonSerialized]
    private object _Tag = null;
    [XmlIgnore]
    public object Tag
    {
      get
      {
        return _Tag;
      }
      set
      {
        _Tag = value;
      }
    }

    [NonSerialized]
    private DbContext _dataContext = null;
    [XmlIgnore]
    public DbContext DataContext
    {
      get
      {
        return _dataContext;
      }
      set
      {
        _dataContext = value;
      }
    }

  }
}
