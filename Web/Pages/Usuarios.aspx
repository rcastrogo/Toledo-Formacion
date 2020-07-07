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

  </style>

  <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
  <script type="text/javascript">


    String.prototype.format = function() {
        var args = arguments;     
        return this.replace(/\{(\d+)\}/g, function(m, i) { return args[i]; });
    }


    var __users = (function(){

      var END_POINT  = '../Api/Test.ashx';
      var __btn;
      var __container;

      function __init() {
        __btn = $('#btnLoadUsers')[0];          // document.getElementById('btnLoadUsers');
        __container = $('#users-container')[0]; // document.getElementById('users-container');
        __btn.onclick = __loadUsers;
      }

      function __loadUsers() {
        
        // ========================================================
        // Inicialización previa a la carga de usuario
        // ========================================================
        __container.innerHTML = 'Cargando datos...';
        // ========================================================
        // Invocar la carga asincrona de los usuarios
        // ========================================================
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
            var __item_template = '<tr>' + 
                                  '  <td>{0}</td><td>{2}</td><td>{1}</td><td>{3}</td><td>{4}</td>' +
                                  '  <td>' + 
                                  '    <a class="w3-button w3-hover-black" href="Formulario.aspx?id={0}" target="_blank">' +
                                  '      Editar' +
                                  '    </a>' + 
                                  '  </td>' +
                                  '</tr>';
            var __container_template = '<table class="w3-table w3-striped w3-bordered w3-hoverable">' + 
                                       '  <tr>' + 
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

            var __innerHTML = response.usuarios
                                      .reduce( function(html, u){
                                        html += __item_template.format(u._id, 
                                                                       u._nombre,
                                                                       u._nif,
                                                                       u._descripcion,
                                                                       u._fechaDeAlta);
                                        return html;
                                      }, '');
            __container.innerHTML = __container_template.format(__innerHTML);

            
         });
      }

      return { init : __init};

    })();

    $(document).ready(function() {
      __users.init();
    });

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
        <div class="w3-bar-item w3-hover-green" id="btnEditUser"><i class="fa fa-edit"></i></div>
        <div class="w3-bar-item w3-hover-green" id="btnAddUser"><i class="fa fa-plus"></i></div>
        <div class="w3-bar-item w3-hover-green" id="btnDeleteUser"><i class="fa fa-trash"></i></div>
      </div>
    </div>

    <div id="users-container" class="w3-container w3-padding w3-card-2 w3-margin" style="min-height:5em;">

    </div>

    <div class="footer-container">
      <TO4500:PageFooterControl ID="PageFooterControl" runat="server" />
    </div>

    <div style="display: none">

    </div>


  </form>

  </body>

</html>
