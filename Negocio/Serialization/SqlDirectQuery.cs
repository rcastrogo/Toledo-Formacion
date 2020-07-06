
namespace Negocio.Core
{
  using Dal.Core;
  using System.Text.RegularExpressions;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Microsoft.VisualBasic.CompilerServices;

  public class SqlDirectQuery
  {
    private SqlDirectQuery() { }

    public static SmallXmlSerializer CreateAndFillSerializerFromQuery(string name, DbContext context, string query)
    {
      using (var __repo = new Dal.Repositories.DynamicRepository(context))
      {
        using (var __reader = __repo.ExecuteReader(query))
        {
          string __name = string.Format("{0}_{1}", name, "SqlDQ");
          Type __otype = SmallXmlSerializer.GetTypeByName(__name);
          if (__otype == null)
            __otype = CreateType(name, __reader, null);
          var __list = new List<object>();
          while (__reader.Read())
            __list.Add(Activator.CreateInstance(__otype, new object[] { __reader, null }));
          return new SmallXmlSerializer(__otype).SetValues(__list);
        }
      }
    }

    public static SmallXmlSerializer CreateAndFillSerializerFromQuery(string name, string query)
    {
      using (var __context = new Dal.Core.DbContext())
      {
        return SqlDirectQuery.CreateAndFillSerializerFromQuery(name, __context, query);
      }
    }

    public static SmallXmlSerializer CreateAndFillSerializer(string name, DbContext context, string queryName, ExtensionPoint extensionPoint, string extraColumns, QueryBuilder queryBuilder)
    {
      using (var __repo = new Dal.Repositories.DynamicRepository(context))
      {
        using (var __reader = __repo.ExecuteNamedReader(queryName, queryBuilder))
        {
          string __name = string.Format("{0}_{1}", name, "SqlDQ");
          Type __otype = SmallXmlSerializer.GetTypeByName(__name);
          if (__otype == null)
            __otype = CreateType(name, __reader, extraColumns);
          var __list = new List<object>();
          while (__reader.Read())
            __list.Add(Activator.CreateInstance(__otype, new object[] { __reader, extensionPoint }));
          return new SmallXmlSerializer(__otype).SetValues(__list);
        }
      }
    }

    public static SmallXmlSerializer CreateAndFillSerializer(string name, DbContext context, string queryName)
    {
      return CreateAndFillSerializer(name, context, queryName, null, null, null);
    }

    public static SmallXmlSerializer CreateAndFillSerializer(string name, DbContext context, string queryName, QueryBuilder queryBuilder)
    {
      return CreateAndFillSerializer(name, context, queryName, null, null, queryBuilder);
    }

    private static Type CreateType(string name, System.Data.IDataReader reader, string extraColumns)
    {
      string columns = string.Join("#", Enumerable.Range(0, reader.FieldCount)
                                                  .Select(i => string.Format("{0},{1},{2}", 
                                                                             reader.GetFieldType(i).Name,
                                                                             i,
                                                                             reader.GetName(Conversions.ToInteger(i))))
                                                  .ToArray());
      if (extraColumns != null && extraColumns.Length > 0)
        columns += extraColumns;// #Integer,~key1,Prueba#String,~key2,PruebaS
      return SmallXmlSerializer.CreateType(name, FieldInfo.FromString(columns));
    }

  }

}




