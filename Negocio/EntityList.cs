
namespace Negocio.Core
{
  using Dal.Core;
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Xml.Serialization;

  [Serializable]
  public abstract class EntityList<T> : Collection<T> where T : Entity
  {
    [NonSerialized]
    private object _tag;
    [NonSerialized]
    private object _dataContext;

    public EntityList()
    {
      _tag = null;
      _dataContext = null;
    }

    public EntityList(DbContext context)
    {
      _tag = null;
      _dataContext = context;
    }

    public EntityList<T> SetDataProvider(Entity.GetExternalData dataProvider)
    {
      foreach (Entity entity in this)
      {
        entity.DataProvider = dataProvider;
      }
      return this;
    }

    public void Sort()
    {
      ((List<T>)Items).Sort();
    }

    [XmlIgnore]
    public object Tag
    {
      get
      {
        return _tag;
      }
      set
      {
        _tag = value;
      }
    }

    [XmlIgnore]
    public DbContext DataContext
    {
      get
      {
        return (DbContext)_dataContext;
      }
      set
      {
        _dataContext = value;
      }
    }

  }
}
