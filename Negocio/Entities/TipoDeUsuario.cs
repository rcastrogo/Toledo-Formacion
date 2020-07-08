
using Dal.Core;
using Dal.Repositories;
using Negocio.Core;
using System;

namespace Negocio.Entities
{
  [Serializable()]
  public class TipoDeUsuario : Entity
  {

    public TipoDeUsuario(){ }
    public TipoDeUsuario(DbContext context) : base(context) { }
        
    public TipoDeUsuario Load(int id){    
      using (TiposDeUsuarioRepository repo = new TiposDeUsuarioRepository(DataContext)){
        return repo.LoadOne<TipoDeUsuario>(this, repo.GetItem(id));
      }   
    }

    public TipoDeUsuario Save(){
      using (TiposDeUsuarioRepository repo = new TiposDeUsuarioRepository(DataContext)){
        if(_id == 0){
          _id = repo.Insert(Descripcion);
        } else{
          repo.Update(Id, Descripcion);
        }
        return this;
      }
    }
             
    public void Delete(){
      using (TiposDeUsuarioRepository repo = new TiposDeUsuarioRepository(DataContext)){
        repo.Delete(_id);
      }
    }
  
    int _id;
    public override int Id  
    {
      get { return _id; }         
      set { _id = value; }
    }

    String _descripcion;
    public String Descripcion  
    {
      get { return _descripcion; }         
      set { _descripcion = value; }
    }

  }
}
