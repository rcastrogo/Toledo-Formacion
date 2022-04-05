using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.UI;

namespace Toledo.Core.Async
{

  public class AsyncPage : Page, ICallbackEventHandler
  {

    private CallbackResult _callbackResult = new CallbackResult();

    private Dictionary<string, object> _controls = new Dictionary<string,object>();

    #region ICallbackEventHandler Members

    public string GetCallbackResult()
    {
      // ===============================================================================================
      // Actualizar el VIEWSTATE durante peticiones
      // ===============================================================================================
      if(EnableViewState == true)
      {
        var mi = typeof(Page).GetMethod("SaveAllState", BindingFlags.Instance | BindingFlags.NonPublic);
        mi.Invoke(Page, null);
        _callbackResult.ViewState = typeof(Page).GetProperty("ClientState", BindingFlags.Instance | 
                                                                            BindingFlags.NonPublic)
                                                .GetValue(Page, null).ToString();
      }
      // ===========================================================================
      // Información de controles que necesitan actualizarse en el cliente
      // ===========================================================================
      _callbackResult.Controls = _controls.Select(k => {
                                            var __value = k.Value as UpdateInfo;
                                            if (__value != null)
                                            {
                                              return new UpdateInfo() {
                                                Id = k.Key,
                                                Disabled = __value.Disabled,
                                                ReadOnly = __value.ReadOnly,
                                                Value = __value.Value,
                                                Checked = __value.Checked
                                              };
                                            } else
                                            {
                                              return new UpdateInfo() {
                                                Id = k.Key,
                                                Value = k.Value
                                              };
                                            }})
                                          .ToList();
      // ===========================================================================
      // Devolver la información al cliente
      // ===========================================================================
      return _callbackResult.ToJsonString();
    }

    public void RaiseCallbackEvent(string eventArgument)
    {
      try
      {
          __invokeAction(eventArgument);
      }
      catch (TargetInvocationException excepcion)
      {
        _callbackResult.Data = "";
        SetError(520, excepcion.InnerException.Message);
      }
      catch (Exception excepcion)
      {
        _callbackResult.Data = "";
        SetError(500, excepcion.Message);
      }
    }

    private void __invokeAction(string argument)
    {
      var __req = Extensions.FromJsonTo<CallbackRequest>(argument);
      if (__req.Method == "") throw new Exception("Falta el nombre del método.");
      var __mi = GetType().GetMethod(__req.Method, BindingFlags.Instance | BindingFlags.NonPublic);
      // ==========================================================================================
      // Método inexistente
      // ==========================================================================================
      if (__mi == null) throw new Exception(
        string.Format("No se ha encontrado el método: {0}.", __req.Method)); 
      // ==========================================================================================
      // Método sin argumentos
      // ==========================================================================================
      var __paramsInfo = __mi.GetParameters();
      if(__paramsInfo.Length == 0)
      {
        _callbackResult.Data = __mi.Invoke(this, null).ToJsonString();
        return;
      }
      // ==========================================================================================
      // Método con un argumento
      // ==========================================================================================
      if(__paramsInfo.Length == 1)
      {
        if(__req.Params[0] == "") 
          throw new Exception(
            string.Format("Falta el valor del parámetro {0} para invocar el método: {1}.",
                          __paramsInfo[0].Name,
                          __req.Method));
          var __parameters = new object[] { 
            __createParamObject(__paramsInfo[0].Name, __paramsInfo[0].ParameterType, __req.Params[0]) 
          };
          _callbackResult.Data = __mi.Invoke(this, __parameters).ToJsonString();
          return;
      }
      // ==========================================================================================
      // Métodos con mas de un argumento no son soportados
      // ==========================================================================================
      throw new Exception(
        string.Format("Error en el número de parámetros para el método: {0}.", __req.Method));
    }

    private object __createParamObject(string name, Type type, string jsonString)
    {
      try
      {
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
        {
          DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
          return serializer.ReadObject(stream);
        }
      }
      catch (Exception)
      {
        throw new Exception(
          String.Format("El valor del parámetro {0} no es del tipo {1} requerido.", name, type.Name)
        );
      }

    }

    #endregion

    protected override void CreateChildControls()
    {
      base.CreateChildControls();

      if (!Page.IsCallback)
      {
        ClientScript.GetCallbackEventReference(this, "", "", "");

        if (!ClientScript.IsClientScriptIncludeRegistered("Default"))
          ClientScript.RegisterClientScriptInclude("Default", Page.ResolveUrl("~/js/Async.js"));

        if (!ClientScript.IsStartupScriptRegistered("_Default_Script"))
        {
          string script = string.Format("var __remote = async.create('{0}', '{1}');", ClientID, UniqueID);
          ClientScript.RegisterStartupScript(this.GetType(), "_Default_Script", script, true);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    protected AsyncPage AddClientCommand(string @method, params object[] @params)
    {
      _callbackResult.ClientCommands.Add(new CallbackClientCommand() {  Method = method, Params = @params});
      return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    protected AsyncPage AddClientScript(string name, string body)
    {
      _callbackResult.ClientCommands.Add(new CallbackClientCommand() { Method = name, Body = body});
      return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected AsyncPage AddMessage(string message)
    {
      _callbackResult.Messages.Add(message);
      return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected AsyncPage SetError(int code, string message)
    {
      _callbackResult.Error = new CallbackError() { Code= code, Message = message };
      return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    protected AsyncPage AddControl(string id, object value)
    {
      if (_controls.ContainsKey(id)) return this;
      _controls.Add(id, value);
      return this;
    }

  }

}