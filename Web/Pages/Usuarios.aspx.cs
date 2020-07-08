using System;
using System.Collections.Generic;
using Negocio;
using Negocio.Core;
using Negocio.Entities;

public partial class Usuarios_Formulario : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  public String GetTiposDeUsuario()
  {
    return new TiposDeUsuario().Load()
                               .ToJsonString();
  }

}