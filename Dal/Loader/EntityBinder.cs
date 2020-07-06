
namespace Dal.Core.Loader
{
  using System;
  using System.Collections.Generic;

  public class EntityBinder
  {
    public Delegate FillObjectDelegate;
    public Delegate FillObjectsDelegate;

    private readonly List<BindItem> _items = new List<BindItem>();

    public void Add(BindItem value)
    {
      _items.Add(value);
    }

    public List<BindItem> bindItems()
    {
      return _items;
    }

    public BindItem this[int index]
    {
      get
      {
        return _items[index];
      }
    }
  }
}
