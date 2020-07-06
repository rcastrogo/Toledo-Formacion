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

    $(document).ready(function() {
      console.log( "ready!" );
    });

  </script>

</head>
<body>

  <form id="form1" runat="server">

    <div class="w3-container w3-teal w3-xxlarge w3-center">
      <TO4500:PageHeaderControl ID="PageHeaderControl1" runat="server" />
    </div>

    <ul class="w3-ul w3-margin">
      <li><a href="api/test.ashx?action=users.all">Lista de todos los usuarios</a></li>
      <li><a href="api/test.ashx?action=users.item&id=1">Usuario con id = 1</a></li>
    </ul>

    <div class="footer-container">
      <TO4500:PageFooterControl ID="PageFooterControl" runat="server" />
    </div>

  </form>

  <div style="display: none">
  </div>

</body>

</html>
