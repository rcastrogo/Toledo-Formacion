
using Dal.Core;
using Dal.Core.Loader;
using Dal.Utils;
using System.Collections.Generic;
using System.Data;

namespace Dal.Repositories
{

  [RepoName("Dal.Repositories.TiposDeUsuarioRepository")]
  public class TiposDeUsuarioRepository : RepositoryBase {
  
    public TiposDeUsuarioRepository(DbContext context) : base(context) { }
        
    public IDataReader GetItems(Dictionary<string, string> @params){
        return GetItems(__toQuery(@params));
    }

    private static string __toQuery(Dictionary<string, string> @params)
    {
      QueryBuilder builder = new QueryBuilder(@params);
      builder.AndListOfIntegers("Id", "Ids"); 
      builder.AndStringLike("Descripcion");
      return builder.ToQueryString();
    }

    public int Insert(string descripcion){
   
      return Insert( new string[] { Helper.ParseString(descripcion) });      
                
    }
    
    public int Update(int id, 
                      string descripcion){			         
      return Update( new string[] { id.ToString(),
                                    Helper.ParseString(descripcion)});           
    }

  }
}