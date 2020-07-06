
namespace Dal.Core.Loader
{
  using Dal;
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Text.RegularExpressions;

  public class EntityBinderFactory
  {
    private static readonly object _object = new object();
    private static readonly Dictionary<string, object> _loaders = new Dictionary<string, object>();
    private static readonly System.Type _entityBinderLoaderType = typeof(EntityBinder);
    private static readonly System.Type _stringLoaderType = typeof(StringBinder);
    private static readonly System.Type _stringType = typeof(string);

    public static EntityBinder GetBinder(object data)
    {
      EntityBinder binder;
      lock (_object)
      {
        if (data == null)
        {
          throw new Exception("El valor no puede ser nulo. GetBinder");
        }
        if (_entityBinderLoaderType.IsInstanceOfType(data))
        {
          binder = (EntityBinder)data;
        }
        else if (_stringLoaderType.IsInstanceOfType(data))
        {
          StringBinder __binder = (StringBinder)data;
          if (!_loaders.ContainsKey(__binder.Key))
          {
            Trace.WriteLine(string.Format("Dal --> Binding --> \"{0}\" ({1})", __binder.Key, __binder.Value));
            _loaders.Add(__binder.Key, Parse(__binder.Value));
          }
          binder = (EntityBinder)_loaders[__binder.Key];
        }
        else
        {
          if (!_stringType.IsInstanceOfType(data))
          {
            throw new Exception("El valor no es valido. GetBinder");
          }
          if (_loaders.ContainsKey(data.ToString()))
          {
            binder = (EntityBinder)_loaders[data.ToString()];
          }
          else
          {
            Trace.WriteLine(string.Format("Dal --> Binding --> \"{0}\"", data.ToString()));
            _loaders.Add(data.ToString(), Parse(MetaDataManager.GetNamedBinder(data.ToString())));
            binder = (EntityBinder)_loaders[data.ToString()];
          }
        }
      }
      return binder;
    }

    public static EntityBinder Parse(string value)
    {
      Trace.WriteLine(string.Format("Dal --> Binding --> Parse {0}", new Regex(@"\s*,").Replace(value, ",")));
      EntityBinder __binder = new EntityBinder();
      foreach (string str in value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
      {
        __binder.Add(new BindItem(str));
      }
      return __binder;
    }
  }
}
