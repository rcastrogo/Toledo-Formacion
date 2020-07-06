<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Pages_Default" %>

<!DOCTYPE html>
<html>
<head runat="server">
  <title>Página de inicio</title>
  <meta name="viewport" content="width=device-width">
  <meta http-equiv="X-UA-Compatible" content="IE=edge">
  
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">
  <link rel="stylesheet" href="./css/w3.css">
  <link rel="stylesheet" href="./css/Estilo.css">
  <style type="text/css" media="screen">

  </style>

  <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
  <script type="text/javascript">

    var __users = (function(){

      var __btn;
      var __container;

      function __init() {
        __btn = $('#btnLoadUsers')[0];          // document.getElementById('btnLoadUsers');
        __container = $('#users-container')[0]; // document.getElementById('users-container');
        __btn.onclick = __loadUsers;
      }

      function __loadUsers() {
        console.log("Carga de usuarios");
        __container.innerHTML = 'Rafa';
      }

      return { init : __init };

    })();

    $(document).ready(function() {
      __users.init();
    });

  </script>

</head>
<body>

  <form id="form1" runat="server">

    <div class="w3-container w3-teal w3-xxlarge w3-center">
      <TO4500:PageHeaderControl ID="PageHeaderControl1" runat="server" />
    </div>

    <div class="w3-container w3-teal w3-margin w3-center">
      <h2>Página de inicio</h2>
    </div>

    <div class="w3-container w3-padding w3-card-2 w3-margin">
      <ul class="w3-ul w3-margin">
        <li><a href="Pages/Formulario.aspx">Formulario</a><span class="w3-tiny"> (WebForms)</span></li>
        <li><a href="api/test.ashx?action=users.all">Usuarios</a><span class="w3-tiny"> (JSON)</span></li>
        <li><a href="api/test.ashx?action=users.item&id=1">Usuario</a><span class="w3-tiny"> (JSON)</span></li>
      </ul>
    </div>

    <div class="w3-center">
      <input type="button" id="btnLoadUsers" value="Cargar usuarios" class="w3-btn w3-blue-grey" />
    </div>

    <div id="users-container" class="w3-container w3-padding w3-card-2 w3-margin" style="min-height:5em;">

    </div>

    <div class="footer-container">
      <TO4500:PageFooterControl ID="PageFooterControl" runat="server" />
    </div>

  </form>

  <div style="display: none">
  </div>

</body>

</html>
