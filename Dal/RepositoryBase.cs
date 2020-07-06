
namespace Dal.Core
{
  using Dal;
  using Dal.Core.Loader;
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Collections.ObjectModel;

  public abstract class RepositoryBase : IDisposable
  {
    protected DbContext Context;
    private readonly bool _auto;

    public RepositoryBase()
    {
      _auto = true;
      Context = new DbContext();
    }

    public RepositoryBase(DbContext context)
    {
      Context = context;
      if (Context == null)
      {
        _auto = true;
        Context = new DbContext();
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_auto)
      {
        Context.Dispose();
      }
    }

    // ===================================================================================
    // DELETE
    // ===================================================================================
    public virtual int Delete(int id)
    {
      return Context.ExecuteNonQuery(string.Format(DefaultDeleteQuery(RepoPrefix()), id));
    }
    // ===================================================================================
    // GET_ITEMS
    // ===================================================================================
    public virtual IDataReader GetItem(int id)
    {
      return GetItems(string.Format("Id={0}", id));
    }

    public virtual IDataReader GetItems()
    {
      return Context.ExecuteReader(string.Format("{0} ORDER BY {1};",
                                                 DefaultSelectQuery(RepoPrefix()),
                                                 DefaultOrderBy(RepoPrefix())));
    }

    internal virtual IDataReader GetItems(string filter)
    {
      if(filter == ""){
        return GetItems();
      }
      else
      {
        return GetItems(filter, DefaultOrderBy(RepoPrefix()));
      }
    }

    internal virtual IDataReader GetItems(string filter, string orderBy)
    {
      if(filter == "" && orderBy == "") return GetItems();
      if(orderBy == "") return GetItems(filter);
      return Context.ExecuteReader(string.Format("{0} WHERE {1} ORDER BY {2};", 
                                                 DefaultSelectQuery(RepoPrefix()), 
                                                 filter, 
                                                 orderBy));

    }

    // ===================================================================================
    // INSERT
    // ===================================================================================
    internal virtual int Insert(string[] values)
    {
      return Convert.ToInt32(Context.ExecuteScalar<decimal>(string.Format(DefaultInsertQuery(RepoPrefix()), (object[])values)));
    }

    internal virtual int Insert(string namedQuery, string[] values)
    {
      return Convert.ToInt32(Context.ExecuteScalar<decimal>(string.Format(MetaDataManager.GetNamedQuery(namedQuery), (object[])values)));
    }

    // ===================================================================================
    // LOAD
    // ===================================================================================
    public Collection<T> Load<T>(Collection<T> target, IDataReader dr) where T : class, new()
    {
      return Load<T>(target, dr, null);
    }

    public Collection<T> Load<T>(Collection<T> target, IDataReader dr, object loaderInfo) where T : class, new()
    {
      target = (loaderInfo != null) ? Dal.Core.Loader.Loader.LoadObjects<T>(target, dr, EntityBinderFactory.GetBinder(loaderInfo), Context) :
                                      Dal.Core.Loader.Loader.LoadObjects<T>(target, dr, EntityBinderFactory.GetBinder(typeof(T).ToString()), Context);
      return target;
    }

    public T LoadOne<T>(T target, IDataReader dr) where T : class, new()
    {
      return LoadOne<T>(target, dr, null);
    }

    public static T LoadOne<T>(T target, IDataReader dr, object loaderInfo) where T : class, new()
    {
      if (loaderInfo == null)
      {
        Dal.Core.Loader.Loader.LoadObject<T>(target, dr, EntityBinderFactory.GetBinder(target.GetType().ToString()));
      }
      else
      {
        Dal.Core.Loader.Loader.LoadObject<T>(target, dr, EntityBinderFactory.GetBinder(loaderInfo));
      }
      return target;
    }

    // ===================================================================================
    // UPDATE
    // ===================================================================================
    internal virtual int Update(string[] values)
    {
      return Context.ExecuteNonQuery(string.Format(DefaultUpdateQuery(RepoPrefix()), (object[])values));
    }

    internal virtual int Update(string namedQuery, string[] values)
    {
      return Context.ExecuteNonQuery(string.Format(MetaDataManager.GetNamedQuery(namedQuery), (object[])values));
    }

    protected string NamedQuery(string name)
    {
      return MetaDataManager.GetNamedQuery(string.Format("{0}.{1}", RepoPrefix(), name));
    }

    private string DefaultDeleteQuery(string typeName)
    {
      return MetaDataManager.GetNamedQuery(string.Format("{0}.Delete", typeName));
    }

    private string DefaultInsertQuery(string typeName)
    {
      return MetaDataManager.GetNamedQuery(string.Format("{0}.Insert", typeName));
    }

    private string DefaultOrderBy(string typeName)
    {
      return MetaDataManager.GetNamedQuery(string.Format("{0}.OrderBy", typeName));
    }

    private string DefaultSelectQuery(string typeName)
    {
      return MetaDataManager.GetNamedQuery(string.Format("{0}.Select", typeName));
    }

    private string DefaultUpdateQuery(string typeName)
    {
      return MetaDataManager.GetNamedQuery(string.Format("{0}.Update", typeName));
    }

    private string RepoPrefix()
    {
      return (GetType().GetCustomAttributes(true)[0] as RepoName).Name;
    }

    internal virtual T ExecuteScalar<T>(string namedQuery, string[] values)
    {
      return Context.ExecuteScalar<T>(string.Format(MetaDataManager.GetNamedQuery(namedQuery), values));
    }

  }
}
