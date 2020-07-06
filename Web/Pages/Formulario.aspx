<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Formulario.aspx.cs" Inherits="Pages_Formulario" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Formulario</title>
  <meta name="viewport" content="width=device-width"/>
  <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
  
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css"/>
  <link rel="stylesheet" href="../css/w3.css"/>
  <link rel="stylesheet" href="../css/Estilo.css"/>
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


    <div class="footer-container">
      <TO4500:PageFooterControl ID="PageFooterControl" runat="server" />
    </div>

  <div style="display: none">
  </div>

    <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />

    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>

  </form>

  </body>

</html>
