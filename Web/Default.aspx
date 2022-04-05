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
  <script type="text/javascript" src="js/Utils.js"></script>
  <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/1.11.0/jquery.min.js"></script>
  <script type="text/javascript">

  </script>

</head>
<body>

  <form id="form1" runat="server" style="padding-bottom:6em !important">

    <div class="w3-container w3-teal w3-xxlarge w3-center">
    </div>

    <div class="w3-container w3-teal w3-margin w3-center">
      <h2>Página de inicio</h2>
    </div>

    <div class="w3-container w3-padding w3-card-2 w3-margin">
      <ul class="w3-ul w3-margin">
        <li><a href="Pages/Formulario.aspx">Formulario</a><span class="w3-tiny"> (WebForms)</span></li>
        <li><a href="Pages/Usuarios.aspx">Listado de usuarios</a><span class="w3-tiny"> (Ajax)</span></li>
        <li><a href="api/test.ashx?action=users.all">Usuarios</a><span class="w3-tiny"> (JSON)</span></li>
        <li><a href="api/test.ashx?action=users.item&id=1">Usuario</a><span class="w3-tiny"> (JSON)</span></li>
        <li><a href="api/tiposDeUsuario.ashx?action=all">Tipos de usuario</a><span class="w3-tiny"> (JSON)</span></li>
        <li><a href="ws/usuarioswebservice.asmx">UsuariosWebservice</a><span class="w3-tiny"> (.asmx)</span></li>
        <li><a href="ws/Service1.svc">Service1</a><span class="w3-tiny"> (.svc)</span></li>
      </ul>
    </div>

    <div class="w3-container w3-padding w3-card-2 w3-margin w3-hide">
      <div class="w3-container">
        <h1>Token</h1>
      </div>
      <button class="w3-button w3-gray" type="button" style="width:100%" onclick="getToken()">Enviar</button>
      <code id="token-result-container" class="w3-input w3-border w3-light-grey w3-code" style="width:100%;font-size: 12px;white-space: break-spaces;">
      </code>
    </div>

    <div class="w3-container w3-padding w3-card-2 w3-margin w3-hide">
      <div class="w3-container">
        <h1>Captación</h1>
      </div>
      <div class="w3-container w3-padding">
        <textarea id="txtRequest" class="w3-input w3-border w3-light-grey w3-code" rows="15" style="width:100%;font-size: 12px">

        </textarea>
        <button class="w3-button w3-gray" id="btnEnviar" type="button" style="width:100%" onclick="sendCaptacion()">Enviar</button>
         <code id="result-container" class="w3-input w3-border w3-light-grey w3-code" style="width:100%;font-size: 12px;white-space: break-spaces;">

         </code>
      </div>


    </div>

    <div class="footer-container">
    </div>

  </form>

  <div style="display: none">
  </div>

</body>

</html>
