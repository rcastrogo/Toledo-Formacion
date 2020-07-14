<%@ WebService Language="C#" Class="UsuariosWebService" %>

using System;
using System.Web;
using System.Web.Services;
using Negocio;

[WebService(Namespace = "http://rcastro.org/services/usuarios")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class UsuariosWebService  : System.Web.Services.WebService {

  [WebMethod]
  public String GetVersion() {
    return "1.0.0";
  }

  [WebMethod]
  public string GetUsuarios(string format) {
    if(format.ToLower() == "xml") return new Negocio.Entities.Usuarios().Load().ToXml();
    return new Negocio.Entities.Usuarios().Load().ToJsonString();
  }

  [WebMethod]
  public Negocio.Entities.Usuario GetUsuario(int idUsuario) {
    return new Negocio.Entities.Usuario().Load(idUsuario);
  }

}