using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using Negocio.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;

using Toledo.Core;

namespace Toledo.Controllers {

  public class TestController : BaseController {


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
      using (Dal.Core.DbContext __dbContext = new Dal.Core.DbContext())
      {
        Usuario __usuario = new Negocio.Entities.Usuario(__dbContext).Load(__id);
        if (__usuario.Id == 0)
        {
          String __message = String.Format("No se ha encontrado el usuario con el Id: {0}", __id);
          throw new Exception(__message);
        }
        // =======================================================================================
        // Devolución de los datos en formato JSON
        // =======================================================================================
        return JsonActionResult.Success("usuario", __usuario);
      }
    }

    public ActionResult DeleteUsuarios()
    {
      String __ids = Context.GetItem("ids");

      Dictionary<String, String> __params = new Dictionary<string, string>();
      __params.Add("Ids", __ids);

      List<int> __deleted = new List<int>();
      List<int> __notDeleted = new List<int>();
      foreach (Usuario __target in new Usuarios().Load(__params))
      {
        if (__target.Id > 3)
        {
          __target.Delete();
          __deleted.Add(__target.Id);
        }
        else
        {
          __notDeleted.Add(__target.Id);
        }
      }
      return JsonActionResult.Success(new string[] { "deleted",
                                                     "notDeleted" },
                                      new Object[] { __deleted.ToArray(),
                                                     __notDeleted.ToArray()});
    }

    public ActionResult EditUsuario()
    {
      // =========================================================================================
      // Validación de parámetros de entrada
      // =========================================================================================
      int __id = Context.ParseInteger("id", 0);
      if (__id < 1) throw new Exception("El identificador no es válido.");
      using (Dal.Core.DbContext __dbContext = new Dal.Core.DbContext())
      {
        // =======================================================================================
        // Cargar el usuario
        // =======================================================================================
        Usuario __target = new Negocio.Entities.Usuario(__dbContext).Load(__id);
        if (__target.Id == 0)
        {
          String __message = String.Format("No se ha encontrado el usuario con el Id: {0}", __id);
          throw new Exception(__message);
        }
        // =======================================================================================
        // Obtener los nuevos valores que hay que almacenar
        // =======================================================================================
        Usuario __payload = Negocio.Extensions.FromJsonTo<Usuario>(Context.GetItem("data"));
        __target.Nif = __payload.Nif;
        __target.Nombre = __payload.Nombre;
        __target.Descripcion = __payload.Descripcion;
        __target.Save();
        // =======================================================================================
        // Devolución de los datos en formato JSON
        // =======================================================================================
        return JsonActionResult.Success("usuario", __target);
      }
    }

    public ActionResult NewUsuario()
    {
      // =======================================================================================
      // Obtener los valores que hay que almacenar
      // =======================================================================================
      Usuario __target = Negocio.Extensions.FromJsonTo<Usuario>(Context.GetItem("data"));
      __target.Id = 0;
      __target.Save();
      return JsonActionResult.Success("usuario", __target);
    }

    private T __default<T>(object data, T @default)
    {
    public ActionResult GetPdfFile(){
            return new PdfActionResult(ms.ToArray(), "document.pdf");
    }
      if (data is DBNull) return (T)@default;
      return (T)data;
    }

    [Serializable]
    public class Estado {
    public String codigo = "";
    public String descripcion = "";

    public Estado(String codigo, String descripcion)  {
      this.codigo = codigo;
      this.descripcion = descripcion;
    }
  }

  }
}