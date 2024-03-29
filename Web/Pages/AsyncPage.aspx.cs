﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Web.UI;
using Negocio;
using Negocio.Core;
using Negocio.Entities;
using Toledo.Core.Async;

public partial class Usuarios_Formulario : Toledo.Core.Async.AsyncPage
{

  protected void Page_Load(object sender, EventArgs e)
  {
    var jsConfig = new JSConfigContainer();
    jsConfig.Add("url", "http://google.com")
            .Add("dato", 5)
            .Add("objeto", new Usuario())
            .Register(this);
  }

  public class JSConfigContainer: Dictionary<string, object>
  {
    public new JSConfigContainer Add(string key, object value)
    {
      base.Add(key, value);
      return this;
    }

    public void Register(Page page)
    {
      page.ClientScript
          .RegisterStartupScript(this.GetType(), 
                                 "configScript",
                                 string.Format("var serverValues = {0};", this.ToJsonStringExt()),
                                 true);
    }
  }


  protected string ActionOne()
  {
    AddControl("checkEnviar", true);
    return "ActionOne";
  }

  protected string[] GetIds()
  {
    AddControl("checkEnviar", false);
    return new string[] { "ID00001", "ID00002" };
  }

  protected Negocio.Entities.Usuario GetUsuario(int id)
  {
    AddClientCommand("alertDos", "'Hola'", 5, "{ id: 8}");
    AddClientCommand("alertDos", "'Hola'", 5);
    AddClientCommand("alertDos");
    AddClientScript("script001", "alertDos('rafa');");
    ViewState["id"] = 67;
    AddControl("txtDescripcion", 55);
    AddControl("txtFechaDeAlta", "fecha");
    return new Usuario() { Id = id };
  }

  protected string[] GetNames(string[] values)
  {
    AddControl("txtDescripcion", new UpdateInfo() { Value = "Jajaja!!", Disabled = true });
    AddControl("txtFechaDeAlta", new UpdateInfo() { Value = "Fecha!!!", Disabled = false, ReadOnly = true });
    AddControl("txtComentarios", "Hola caracola\r\nOtra línea");
    AddControl("checkLemon", new UpdateInfo() { Checked = true, Disabled = false });
    AddControl("checkSugar", new UpdateInfo() { Checked = true, Disabled = true });
    AddControl("cmbOptions", new UpdateInfo() { 
      Value = new OptionInfo[] {
        new OptionInfo() { Id = "1", Value = "Opción uno", Disabled = true},
        new OptionInfo() { Id = "2", Value = "Opción dos"},
        new OptionInfo() { Id = "3", Value = "Opción tres", Selected = true} }, 
      Disabled = false
    });
    return new string[] { "Names00001", "Names00002" };
  }

}