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
      //debugger;
      console.log( "ready!" );
    });

  </script>

</head>
<body>

  <form id="form1" runat="server" style="padding-bottom:6em !important">

    <div class="w3-container w3-teal w3-xxlarge w3-center">
      <TO4500:PageHeaderControl ID="PageHeaderControl1" runat="server" />
    </div>

    <div class="w3-container w3-teal w3-margin">
      <h2>Datos del usuario <a href="../Default.aspx" class="w3-right"><i class="fa fa-2x fa-home"></i></a> </h2>
    </div>

    <div class="w3-container w3-padding w3-card-2 w3-margin">

      <div class="w3-right-align">
        <asp:Button ID="btnSave" runat="server" Text="Grabar" OnClick="btnSave_Click" class="w3-btn w3-blue-grey" />
        <asp:Button ID="btnDelete" runat="server" Text="Borrar" OnClick="btnDelete_Click" class="w3-btn w3-blue-grey" />
        <asp:Button ID="btnNew" runat="server" Text="Nuevo" OnClick="btnNew_Click" class="w3-btn w3-blue-grey" />
        <asp:Button ID="btnEdit" runat="server" Text="Editar" OnClick="btnEdit_Click" class="w3-btn w3-blue-grey" />
      </div>

      <div class="w3-container w3-center w3-border-bottom w3-border-top w3-margin-top" style="min-height:2em;">
        <asp:label ID="txtMessage" CssClass="w3-text-teal w3-bar-item" runat="server"></asp:label>
      </div>

      <label class="w3-text-teal" for="txtId"><b>Identificador</b></label>
      <asp:TextBox ID="txtId" runat="server" class="w3-input w3-border w3-light-grey"></asp:TextBox>

      <label class="w3-text-teal" for="txtNif"><b>Nif</b></label>
      <asp:TextBox ID="txtNif" runat="server" class="w3-input w3-border w3-light-grey"></asp:TextBox>

      <label class="w3-text-teal" for="txtNombre"><b>Nombre</b></label>
      <asp:TextBox ID="txtNombre" runat="server" class="w3-input w3-border w3-light-grey"></asp:TextBox>

      <label class="w3-text-teal" for="txtDescripcion"><b>Descripción</b></label>
      <asp:TextBox ID="txtDescripcion" runat="server" class="w3-input w3-border w3-light-grey"></asp:TextBox>

      <label class="w3-text-teal" for="txtFechaDeAlta"><b>Fecha de alta</b></label>
      <asp:TextBox ID="txtFechaDeAlta" runat="server" class="w3-input w3-border w3-light-grey"></asp:TextBox>



    </div>

    <div class="footer-container">
      <TO4500:PageFooterControl ID="PageFooterControl" runat="server" />
    </div>

    <div style="display: none">

    </div>


  </form>

  </body>

</html>
