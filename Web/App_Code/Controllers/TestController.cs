using System;
using System.Collections.Generic;
using System.Linq;
using Negocio.Entities;

using Toledo.Core;


namespace Toledo.Controllers
{

  public class TestController : BaseController
  {


    #region CONSTRUCTORES

    public TestController(ContextWrapper context) : base(context) { }

    #endregion

    public ActionResult GetUsuarios()
    {
      // ===============================================================
      // Datos de los usuarios
      // ===============================================================
      var __usuarios = new Negocio.Entities.Usuarios().Load();
      // ===============================================================
      // Devolución de los datos en formato JSON
      // ===============================================================
      return JsonActionResult.Success("usuarios", __usuarios.ToArray());
    }

    public ActionResult GetUsuario()
    {
      // =========================================================================================
      // Validación de parámetros de entrada
      // =========================================================================================
      int __id = Context.ParseInteger("id", 0);
      if (__id < 1) throw new Exception("El identificador no es válido."); 
      // =========================================================================================
      // Cargar el usuario
      // =========================================================================================
      using (Dal.Core.DbContext __dbContext = new Dal.Core.DbContext()) {
        Usuario __usuario = new Negocio.Entities.Usuario(__dbContext).Load(__id);
        if(__usuario.Id == 0) {
          String __message = String.Format("No se ha encontrado el usuario con el Id: {0}", __id);
          throw new Exception(__message);
        }
        // =======================================================================================
        // Devolución de los datos en formato JSON
        // =======================================================================================
        return JsonActionResult.Success("usuario", __usuario);
      }
    }

    private T __default<T>(object data, T @default)
    {
      if (data is DBNull) return (T)@default;
      return (T)data;
    }

    [Serializable]
    public class Estado
    {
      public String codigo = "";
      public String descripcion = "";

      public Estado(String codigo, String descripcion)
      {
        this.codigo = codigo;
        this.descripcion = descripcion;
      }
    }

  }
}