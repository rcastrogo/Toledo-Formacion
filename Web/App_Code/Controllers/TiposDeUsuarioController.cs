using System;
using System.Collections.Generic;
using System.Linq;
using Negocio.Entities;

using Toledo.Core;


namespace Toledo.Controllers
{

  public class TiposDeUsuarioController : BaseController
  {


    #region CONSTRUCTORES

    public TiposDeUsuarioController(ContextWrapper context) : base(context) { }

    #endregion


    public ActionResult GetAll()
    {

      //TipoDeUsuario __tipo = new TipoDeUsuario();
      //__tipo.Descripcion = "Solo lectura";
      //__tipo.Save();

      var __tipos = new TiposDeUsuario().Load();

      return new StringActionResult(__tipos.ToJsonString());
    }

  }

}