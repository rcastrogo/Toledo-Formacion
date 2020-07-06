
namespace Dal.Repositories
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using Dal.Core;

    public class DynamicRepository : IDisposable
    {
        private readonly Repository __repository;

        #region Constructor


        public DynamicRepository()
        {
            __repository = new Repository();
        }

        public DynamicRepository(DbContext context, string name)
        {
            __repository = new Repository(context, name);
        }

        public DynamicRepository(DbContext context)
        {
            __repository = new Repository(context, "");
        }

        #endregion

        #region Destructor
        ~DynamicRepository()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            __repository.Dispose();
        }

        #endregion

        #region Queries

        public IDataReader ExecuteReader(string query)
        {
            return __repository.ExecuteQuery(query);
        }

        public IDataReader ExecuteNamedReader(string queryName)
        {
            return __repository.ExecuteNamedQuery(queryName);
        }

        public IDataReader ExecuteNamedReader(string queryName, QueryBuilder qBuilder)
        {
            return __repository.ExecuteNamedQuery(queryName, qBuilder);
        }

        public T ExecuteNamedScalar<T>(string queryName, string[] values)
        {
            return __repository.ExecuteNamedScalar<T>(queryName, values);
        }

        public int ExecuteNamedNonQuery(string name, string[] values)
        {
            return __repository.ExecuteNamedNonQuery(name, values);
        }

        #endregion

        #region Carga

        public T LoadOne<T>(T target, IDataReader dr) where T : class, new()
        {
            return Repository.LoadOne(target, dr, null);
        }

        public T LoadOne<T>(T target, IDataReader dr, object loaderInfo) where T : class, new()
        {
            return Repository.LoadOne(target, dr, loaderInfo);
        }

        public T LoadOne<T>(T target, string namedQuery) where T : class, new()
        {
            return Repository.LoadOne(target, __repository.ExecuteNamedQuery(namedQuery), null);
        }

        public Collection<T> Load<T>(Collection<T> target, IDataReader dr) where T : class, new()
        {
            return __repository.Load(target, dr, null);
        }

        public Collection<T> Load<T>(Collection<T> target, IDataReader dr, object loaderInfo) where T : class, new()
        {
            return __repository.Load(target, dr, loaderInfo);
        }

        public Collection<T> Load<T>(Collection<T> target, string namedQuery) where T : class, new()
        {
            return __repository.Load(target, __repository.ExecuteNamedQuery(namedQuery), null);
        }

        #endregion

        private class Repository : RepositoryBase
        {

            #region Constructor

            public Repository() : base(null)
            {
            }

            public Repository(DbContext context, string repoPrefix) : base(context)
            {
                _repoPrefix = repoPrefix;
            }

            #endregion

            #region Queries

            public IDataReader ExecuteQuery(string query)
            {
                return Context.ExecuteReader(query);
            }

            public IDataReader ExecuteNamedQuery(string name)
            {
                return Context.ExecuteReader(NamedQuery(name));
            }

            public IDataReader ExecuteNamedQuery(string name, QueryBuilder qBuilder)
            {
                string __query = NamedQuery(name);
                if (qBuilder == null)
                    return Context.ExecuteReader(__query);
                else
                {
                    string __where = qBuilder.ToQueryString();
                    if (string.IsNullOrEmpty(__where))
                    {
                        if (qBuilder.Locator == "") return Context.ExecuteReader(__query);
                        return Context.ExecuteReader(__query.Replace(qBuilder.Locator, ""));
                    }
                    else
                        switch (qBuilder.Locator)
                        {
                            case "":
                                {
                                    return Context.ExecuteReader(string.Format("{0} WHERE {1};", __query, __where));
                                }

                            default:
                                {
                                    return Context.ExecuteReader(__query.Replace(qBuilder.Locator, string.Format(" WHERE {0}", __where)));
                                }
                        }
                }
            }

            public T ExecuteNamedScalar<T>(string name, string[] values)
            {
                return base.ExecuteScalar<T>(__parseNamed(name), values);
            }
                    
            public int ExecuteNamedNonQuery(string name, string[] values)
            {
                return base.Context.ExecuteNonQuery(NamedQuery(name), values);
            }

            #endregion

            private string _repoPrefix = "";
            protected string RepoPrefix
            {
                get
                {
                    return _repoPrefix;
                }
            }

            private string __parseNamed(string name)
            {
                return string.IsNullOrEmpty(_repoPrefix) ? name
                                                         : string.Format("{0}.{1}", _repoPrefix, name);
            }

            protected new string NamedQuery(string name)
            {
                return Dal.Core.Loader.MetaDataManager.GetNamedQuery(__parseNamed(name));
            }
        }
    }
}


