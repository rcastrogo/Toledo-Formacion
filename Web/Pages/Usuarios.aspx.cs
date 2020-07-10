using System;
using System.Collections.Generic;
using Negocio;
using Negocio.Core;
using Negocio.Entities;

public partial class Usuarios_Formulario : System.Web.UI.Page
{

  public int VersionDelProducto = 5;

  protected void Page_Load(object sender, EventArgs e)
  {
    /// Desde la BD
    VersionDelProducto = 55;
  }

  public String GetTiposDeUsuario()
  {
    return new TiposDeUsuario().Load()
                               .ToJsonString();
  }

}