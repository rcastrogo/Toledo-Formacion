
using Dal.Core;
using Dal.Repositories;
using Negocio.Core;
using System.Collections.Generic;

namespace Negocio.Entities
{
  [System.Xml.Serialization.XmlRoot("Usuarios")]
  public class Usuarios : EntityList<Usuario>
  {
    public Usuarios() { }

    public Usuarios(DbContext context) : base(context) { }

    public Usuarios Load()
    {
      using (UsuariosRepository repo = new UsuariosRepository(base.DataContext))
      {
        return (Usuarios)repo.Load<Usuario>(this, repo.GetItems());
      }
    }

    public Usuarios Load(Dictionary<string, string> @params)
    {
      using (UsuariosRepository repo = new UsuariosRepository(base.DataContext))
      {
        return (Usuarios)repo.Load<Usuario>(this, repo.GetItems(@params));
      }
    }
  }
}
