<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Usuarios.aspx.cs" Inherits="Usuarios_Formulario" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Usuarios</title>
  <meta name="viewport" content="width=device-width"/>
  <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
  
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css"/>
  <link rel="stylesheet" href="../css/w3.css"/>
  <link rel="stylesheet" href="../css/Estilo.css"/>
  <style type="text/css" media="screen">

    .w3-vertical-align-middle td{ vertical-align:middle !important; }

  </style>

  <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
  <script type="text/javascript">

    var _tipoDeUsuario   = <%= GetTiposDeUsuario() %>;
    var _version = <%= VersionDelProducto %>;

    // =============================================================================================
    // Utils
    // =============================================================================================
    String.prototype.format = function() {
      var args = arguments;     
      return this.replace(/\{(\d+)\}/g, function(m, i) { return args[i]; });
    }

    Array.prototype.remove = function (value) {
      if (typeof(value) === 'function') {
        var __target = this.filter(value)[0];
        if (__target) {
          this.splice(this.indexOf(__target), 1);
        }
        return;
      } 
      this.splice(this.indexOf(value), 1);
    }

    Array.prototype.item = function (prop, value) {
      return this.filter( function(u){ return u[prop] == value; })[0];
    }

    var toArray  = function(v) { return Array.prototype.slice.call(v); }


    // =============================================================================================
    // Gestión de usuario
    // =============================================================================================
    var __users = (function(){

      var END_POINT  = '../Api/Test.ashx';
      var __dataset  = [];
      var __btn;
      var __container;
      var __messageContainer;

      function __init() {
        __btn              = $('#btnLoadUsers')[0];     // document.getElementById('btnLoadUsers');
        __container        = $('#users-container')[0];  // document.getElementById('users-container');
        __messageContainer = $('#message-container')[0];// document.getElementById('message-container');
        __btn.onclick = __loadUsers;
      }

      function __onCheckChange(sender) {
        __messageContainer.innerHTML = 'CheckClick: {0}'.format(this.id);
        setTimeout( function(){ 
          __messageContainer.innerHTML = '';
        }, 3000);
      }

      // ===================================================
      // Acciones de los botones
      // ===================================================
      function __onButtonClick(sender) {

        function __onEditUser() {
          var __selected = toArray(__container.querySelectorAll(':checked'));
          if (__selected.length == 1) {

            var __targetId = __selected[0].id.split('-')[2];
            var __target   = __dataset.filter( function(usuario){ 
                                                 return usuario._id == __targetId;
                                               })[0];
            document.querySelector('#txtId').value = __target._id;
            document.querySelector('#txtNif').value = __target._nif;
            document.querySelector('#txtNombre').value = __target._nombre;
            document.querySelector('#txtDescripcion').value = __target._descripcion;
            document.querySelector('#txtFechaDeAlta').value = __target._fechaDeAlta;
            _dialogManager.show();
            return;
          }
          __messageContainer.innerHTML = 'Solo un elemento';
        }

        function __onAddUser() {
          document.querySelector('#txtId').value = '';
          document.querySelector('#txtNif').value = '';
          document.querySelector('#txtNombre').value = '';
          document.querySelector('#txtDescripcion').value = '';
          document.querySelector('#txtFechaDeAlta').value = '';
          _dialogManager.show();
        }

        function __onDeleteUser() {
          var __selected = toArray(__container.querySelectorAll(':checked'));
          if (__selected.length > 0) {
            var __ids = __selected.map( function(check){ return check.id; })
                                  .map(function(id){ return id.split('-')[2]; })
                                  .join('-');
            // ============================================
            // Invocar la carga asincrona de los usuarios
            // ============================================
            $.ajax({ url   : END_POINT, 
                     cache : false,
                     data  : { action : 'users.delete',
                               ids    : __ids } })
             .done( function(response){
               // =======================================================================================
               // Borrados
               // =======================================================================================
               response.deleted
                       .forEach( function(id){
                          // ============================================
                          // Sincronizar Interfaz de usuario
                          // ============================================
                          var __row = __container.querySelector('#user-tr-{0}'.format(id));
                          __row.parentNode.removeChild(__row);
                          // ============================================
                          // Sincronizar dataset
                          // ============================================
                          //var __target = __dataset.item('_id', id);
                          //__dataset.remove(__target);
                          __dataset.remove(function(u) { return u._id == id; });                       
                       });
               // =======================================================================================
               // No Borrados
               // =======================================================================================
               response.notDeleted
                       .forEach( function(id){
                          __container.querySelector('#user-tr-{0}'.format(id))
                                     .style
                                     .backgroundColor = 'mediumspringgreen';
                       });
             });
          }
        }

        if(this.id == 'btnEditUser')   __onEditUser();
        if(this.id == 'btnAddUser')    __onAddUser();
        if(this.id == 'btnDeleteUser') __onDeleteUser();
     
      }

      function __setEventHandlers() {
        // ===========================================================================
        // ChecksBoxes
        // ===========================================================================
        var __checks = toArray(__container.querySelectorAll('input[type=checkbox]'));
        __checks.forEach( function(check){ check.onclick = __onCheckChange; });
        // ===========================================================================
        // Buttons
        // ===========================================================================
        var __buttons = document.querySelectorAll('div.js-btn');
        __buttons.forEach( function(btn){ btn.onclick = __onButtonClick; });
      }

      function __loadUsers() {
        
        // ============================================
        // Inicialización previa a la carga de usuario
        // ============================================
        __container.innerHTML = 'Cargando datos...';
        // ============================================
        // Invocar la carga asincrona de los usuarios
        // ============================================
        $.ajax({ url   : END_POINT, 
                 cache : false,
                 data  : { action : 'users.all' } })
         .done( function(response){
            // ==================================================================================================
            // Controlar/Mostrar posibles errores
            // ==================================================================================================
            if(response.result != 'ok'){
              __container.innerHTML = response.message;
              return;
            }
            // ==================================================================================================
            // Crear la UI una vez que los datos están disponibles
            // ==================================================================================================
            var __item_template = '<tr id="user-tr-{0}" class="w3-vertical-align-middle">' + 
                                  '  <td>' + 
                                  '    <input type="checkbox" id="user-check-{0}" />' + 
                                  '  </td>' +
                                  '  <td>{0}</td><td>{2}</td><td>{1}</td><td>{3}</td><td>{4}</td>' +
                                  '  <td>' + 
                                  '    <a class="w3-button w3-hover-black" href="Formulario.aspx?id={0}" target="_blank">' +
                                  '      Editar' +
                                  '    </a>' + 
                                  '  </td>' +
                                  '</tr>';
            var __container_template = '<table class="w3-table w3-striped w3-bordered w3-hoverable">' + 
                                       '  <tr>' + 
                                       '    <th></th>' +
                                       '    <th>Id</th>' +
                                       '    <th>Nif</th>' +
                                       '    <th>Nombre</th>' +
                                       '    <th>Descripción</th>' +
                                       '    <th>Fecha de alta</th>' +
                                       '    <th></th>' +
                                       '  </tr>' + 
                                       '  {0}' +
                                       '</table>';

            //var __innerHTML = '';
            //$.each(response.usuarios, function (index, usuario ) {
            //  __innerHTML += __item_template.format(usuario._id, 
            //                                        usuario._nombre,
            //                                        usuario._nif,
            //                                        usuario._descripcion,
            //                                        usuario._fechaDeAlta);
            //});
           __dataset = response.usuarios;
            var __innerHTML = __dataset.reduce( function(html, u){
                                  html += __item_template.format(u._id, 
                                                                  u._nombre,
                                                                  u._nif,
                                                                  u._descripcion,
                                                                  u._fechaDeAlta);
                                  return html;
                                }, '');
            __container.innerHTML = __container_template.format(__innerHTML);
            // =============================================
            // Establecer manejadores de eventos
            // =============================================
            __setEventHandlers();
            
         });

      }

      function __save(payload) {
        // ========================================================
        // Invocar la actualizacion/inserción del usuario
        // ========================================================
        var __editMode = payload._id != 0;
        $.ajax({ url   : END_POINT,
                 type  : 'POST',
                 data  : { action : __editMode ? 'users.edit'
                                               : 'users.new',
                           data   : JSON.stringify(payload), //  Negocio.Extensions.FromJsonTo<Usuario>(Context.GetItem("data"));
                           id          : payload._id,        //  Context.GetItem("id");
                           nif         : payload._nif,
                           nombre      : payload._nombre,
                           descripcion : payload._descripcion
                         } })
          .done( function(response){
            console.log(response);
            // =========================================
            // Forma rápida (volver a cargar todo)
            // =========================================
            __loadUsers();
            _dialogManager.close();
            return;
            // =========================================
            // Forma optimizada (Sincronizar datos y UI)
            // =========================================
            if (__editMode) {
              // TODO: Actualizar los datos de la fila
              // TODO: Actualizar los datos de __dataset
            } else {
              // TODO: Insertar una fila
              // TODO: Anadir manejadores de eventos
              // TODO: Añadir el usuario al __dataset
            }
            _dialogManager.close();
          });
      }

      return { init      : __init,
               loadUsers : __loadUsers, 
               saveUser  : __save };

    })();

    $(document).ready(function() {
      __users.init();
      __users.loadUsers()
    });

    var _dialogManager = (function(){

      function __openDialog() {
        document.querySelector('#dlg-modal').style.display = 'block';
      }

      function __closeDialog() {
        document.querySelector('#dlg-modal').style.display = 'none';
      }

      function __acceptChanges() {
        // ==============================================================
        // Valido cosas
        // ==============================================================
        if (!document.querySelector('#txtDescripcion').value) {
          return;
        }
        // ==============================================================
        // Ver que hago
        // ==============================================================

        // ==============================================================
        // Hacerlo
        // ==============================================================
        var __paidload = { 
          _id          : ~~(document.querySelector('#txtId').value || 0),
          _nombre      : document.querySelector('#txtNombre').value,
          _nif         : document.querySelector('#txtNif').value,
          _descripcion : document.querySelector('#txtDescripcion').value,
          _fechaDeAlta : document.querySelector('#txtFechaDeAlta').value
        };
        __users.saveUser(__paidload)
      }
      
      return {
        show  : __openDialog,
        close : __closeDialog,
        acceptChanges : __acceptChanges
      };

    }());    


  </script>

</head>
<body>

  <form id="form1" runat="server" style="padding-bottom:6em !important">

    <div class="w3-container w3-teal w3-xxlarge w3-center">
      <TO4500:PageHeaderControl ID="PageHeaderControl1" runat="server" />
    </div>

    <div class="w3-container w3-teal w3-margin">
      <h2>Listado de usuarios  <a href="../Default.aspx" class="w3-right"><i class="fa fa-2x fa-home"></i></a></h2>
    </div>

    <div class="w3-container">
      <div class="w3-bar w3-black">
        <div class="w3-bar-item w3-hover-green w3-right" id="btnLoadUsers"><i class="fa fa-refresh"></i></div>
        <div class="w3-bar-item w3-hover-green js-btn" id="btnEditUser"><i class="fa fa-edit"></i></div>
        <div class="w3-bar-item w3-hover-green js-btn" id="btnAddUser"><i class="fa fa-plus"></i></div>
        <div class="w3-bar-item w3-hover-green js-btn" id="btnDeleteUser"><i class="fa fa-trash"></i></div>
        <div class="w3-bar-item" id="message-container"></div>
      </div>
    </div>

    <div id="users-container" class="w3-container w3-padding w3-card-2 w3-margin" style="min-height:5em;">

    </div>

    <div class="footer-container">
      <TO4500:PageFooterControl ID="PageFooterControl" runat="server" />
    </div>

  </form>


  <div id="dlg-modal" class="w3-modal">

    <div class="w3-modal-content  w3-animate-zoom">

      <header class="w3-container w3-teal">
        <span onclick="_dialogManager.close()" class="w3-button w3-display-topright">&times;</span>
        <h2>Datos de usuario</h2>
      </header>

      <div class="w3-container w3-padding">
        
        <label class="w3-text-teal" for="txtId"><b>Identificador</b></label>
        <input type="text" id="txtId" class="w3-input w3-border w3-light-grey" />

        <label class="w3-text-teal" for="txtNif"><b>Nif</b></label>
        <input type="text" id="txtNif" class="w3-input w3-border w3-light-grey"/>

        <label class="w3-text-teal" for="txtNombre"><b>Nombre</b></label>
        <input type="text" id="txtNombre" class="w3-input w3-border w3-light-grey"/>

        <label class="w3-text-teal" for="txtDescripcion"><b>Descripción</b></label>
        <input type="text" id="txtDescripcion" class="w3-input w3-border w3-light-grey"/>

        <label class="w3-text-teal" for="txtFechaDeAlta"><b>Fecha de alta</b></label>
        <input type="text" id="txtFechaDeAlta" class="w3-input w3-border w3-light-grey"/>

      </div>

      <footer class="w3-container w3-teal w3-center w3-padding">
        <button class="w3-button w3-black" id="btnGrabar" type="button" onclick="_dialogManager.acceptChanges()">Grabar</button>
      </footer>

    </div>

  </div>

  </body>

</html>
