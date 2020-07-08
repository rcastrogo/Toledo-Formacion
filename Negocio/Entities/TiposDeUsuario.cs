
using Dal.Core;
using Dal.Repositories;
using Negocio.Core;
using System.Collections.Generic;

namespace Negocio.Entities
{
  [System.Xml.Serialization.XmlRoot("TiposDeUsuario")]
  public class TiposDeUsuario : EntityList<TipoDeUsuario>
  {
    public TiposDeUsuario() { }

    public TiposDeUsuario(DbContext context) : base(context) { }

    public TiposDeUsuario Load()
    {
      using (TiposDeUsuarioRepository repo = new TiposDeUsuarioRepository(base.DataContext))
      {
        return (TiposDeUsuario)repo.Load<TipoDeUsuario>(this, repo.GetItems());
      }
    }

    public TiposDeUsuario Load(Dictionary<string, string> @params)
    {
      using (TiposDeUsuarioRepository repo = new TiposDeUsuarioRepository(base.DataContext))
      {
        return (TiposDeUsuario)repo.Load<TipoDeUsuario>(this, repo.GetItems(@params));
      }
    }
  }
}
