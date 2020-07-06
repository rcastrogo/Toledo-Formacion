using System;
using System.Collections.Generic;
using Negocio;
using Negocio.Core;
using Negocio.Entities;

public partial class Pages_Formulario : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    txtId.ReadOnly = true;
    txtFechaDeAlta.ReadOnly = true;
    if (this.IsPostBack) {
      System.Diagnostics.Debug.WriteLine("this.IsPostBack");
    }
    else {
      System.Diagnostics.Debug.WriteLine("Not this.IsPostBack");
      _initForm();
    }
  }

  protected void btnSave_Click(object sender, EventArgs e)
  {
    try {
      // =================================================================================================
      // Validar campos
      // =================================================================================================
      if(txtNif.Text.Trim() == String.Empty) throw new Exception("El Nif es obligatorio");
      if(txtNombre.Text.Trim() == String.Empty) throw new Exception("El Nombre es obligatorio");
      if(txtDescripcion.Text.Trim() == String.Empty) throw new Exception("La descripción es obligatoria");
      if(txtNif.Text.Trim().Length < 3) throw new Exception("El formato del Nif es incorrecto");
      // =================================================================================================
      // Determinar si es una inserción o una actualización
      // =================================================================================================
      int __id = 0;
      int.TryParse(txtId.Text, out __id);
      Usuario __usuario = new Usuario();
      if(__id > 0) {
        __usuario.Load(__id);
        if (__usuario.Id == 0){
          throw new Exception(String.Format("El usuario {0} no existe", __id));
        }
      }
      // =================================================================================================
      // Pasar los valores de los controles al objeto de negocio
      // =================================================================================================
      __usuario.Nombre = txtNombre.Text.Trim();
      __usuario.Nif = txtNif.Text.Trim();
      __usuario.Descripcion = txtDescripcion.Text.Trim();
      // =================================================================================================
      // Grabar/persistir datos en la base de datos
      // =================================================================================================
      __usuario.Save();
      // =================================================================================================
      // Actualizar/formatear los datos en el lado cliente
      // =================================================================================================
      txtId.Text = __usuario.Id.ToString();
      txtNif.Text = txtNif.Text.Trim().ToUpper();
      txtFechaDeAlta.Text = __usuario.FechaDeAlta;
      txtMessage.Text = "Usuario grabado correctamente";
    }
    catch (Exception ex) {
      txtMessage.Text = ex.Message;
    }
  }

  protected void btnDelete_Click(object sender, EventArgs e)
  {
    txtMessage.Text = "Botón borrar pulsado";
    try {
      // ======================================================================
      // Validar id
      // ======================================================================
      int __id = 0;
      int.TryParse(txtId.Text, out __id);
      if(__id < 1) throw new Exception("Nada que borrar");
      // ======================================================================
      // Determinar si el usuario existe
      // ======================================================================
      Usuario __usuario = new Usuario().Load(__id);
      if (__usuario.Id == 0){
        throw new Exception(String.Format("El usuario {0} no existe", __id));
      }
      // ======================================================================
      // Intentar borrarlo
      // ======================================================================
      //__usuario.Delete();
      txtMessage.Text = "El usuario se ha borrado correctamente";
      _clearFormsControls();
    } catch (Exception ex){
      txtMessage.Text = ex.Message;
    }
  }

  protected void btnNew_Click(object sender, EventArgs e)
  {
    txtMessage.Text = "Botón nuevo pulsado";
    // =============================================================
    // Al limpiar el id estamos "posibilitando" la inserción
    // =============================================================
    _clearFormsControls();
  }

  protected void btnEdit_Click(object sender, EventArgs e)
  {
    txtMessage.Text = "Botón editar pulsado";
    try {
      // ====================================================================
      // Determinar qué usuario editar
      // ====================================================================
      int __id = 2;
      // ====================================================================
      // Cargar los datos del usuario
      // ====================================================================
      Usuario __usuario = new Usuario().Load(__id);
      if (__usuario.Id == 0){
        throw new Exception(String.Format("El usuario {0} no existe", __id));
      }
      // ====================================================================
      // Pasar los datos del usuario a los controles
      // ====================================================================
      _populateFormsControls(__usuario);
    } catch (Exception ex){
      txtMessage.Text = ex.Message;
    }
  }

  private void _populateFormsControls(Usuario usuario)
  {
    txtId.Text = usuario.Id.ToString();
    txtNif.Text = usuario.Nif;
    txtNombre.Text = usuario.Nombre;
    txtDescripcion.Text = usuario.Descripcion;
    txtFechaDeAlta.Text = usuario.FechaDeAlta;
  }

  private void _clearFormsControls()
  {
    txtId.Text = "";
    txtNif.Text = "";
    txtNombre.Text = "";
    txtDescripcion.Text = "";
    txtFechaDeAlta.Text = "";
  }

  private void _initForm()
  {
    // ==================================================
    // Determinar y validar el id que se quiere mostrar
    // ==================================================
    int __id = 0;
    if (Request.QueryString["id"] != string.Empty) { 
      int.TryParse(Request.QueryString["id"], out __id);
      if (__id < 1) return;
    }
    // ==================================================
    // Cargar los datos del usuario
    // ==================================================
    Usuario __usuario = new Usuario().Load(__id);
    if (__usuario.Id > 0) {
      _populateFormsControls(__usuario);
    }
  }

}