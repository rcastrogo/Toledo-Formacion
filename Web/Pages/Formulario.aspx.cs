using System;
using System.Collections.Generic;
using Negocio;
using Negocio.Core;
using Negocio.Entities;

public partial class Pages_Formulario : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (this.IsPostBack) {
      System.Diagnostics.Debug.WriteLine("this.IsPostBack");
    }
    else {
      System.Diagnostics.Debug.WriteLine("Not this.IsPostBack");
    }
  }

  protected void Button1_Click(object sender, EventArgs e)
  {
    TextBox1.Text = "Hola";
  }

}