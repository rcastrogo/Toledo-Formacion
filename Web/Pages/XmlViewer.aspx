<%@ Page Language="C#" AutoEventWireup="true" CodeFile="XmlViewer.aspx.cs" Inherits="XmlViewerPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Visor xml</title>
  <meta name="viewport" content="width=device-width"/>
  <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css"/>
  <link rel="stylesheet" href="../css/w3.css"/>
  <link rel="stylesheet" href="../css/Estilo.css"/>

  <script src="../js/pol.bundle.js"></script>

  <script type="text/javascript">
  
    var __data = [
      {
        _descripcion: "Mercadona",
        _fechaDeAlta: "03/02/2020 08:31:04.353",
        _id: 3,
        _nif: "03470001K",
        _nombre: "Allyson López Santiago"
      },
      {
        _descripcion: "Administrador",
        _fechaDeAlta: "10/02/2020 13:33:45.697",
        _id: 55,
        _nif: "000055",
        _nombre: "El cincuenta y cinco",
        __checked : true
      }
    ];
    var $ = pol.core;

    function __initAll() {
      var _tbody  = $.element('table tbody');    
      pol.templates.fillTemplate(_tbody, { proveedores : __data, 
                                           fn          : {
                                            checked: function(t, e) {
                                                return t.__checked ? "checked" : "";
                                            }
                                         }});
      pol.declarative.addEventListeners( 
        _tbody,
        {
          doAction : function (sender, event, name, data){
            console.log(event, name, data);
          },
          doAddToFavorites: function(sender, event, name, data){
            console.log(event, name, data);
          }
        }, 
        {});
    }

  </script>


</head>
  <body>

    <form id="form1" runat="server" style="padding-bottom:6em !important">

      <div id="users-container" class="w3-container w3-padding w3-margin">
        <div class="ItemsContainer" id="ItemsContainer">
          <table class="w3-table-all">
            <thead>
              <tr on-click="doSort">
                <th style="width:1%;"></th>
                <th class="w3-hover-gray sorteable">Id</th>
                <th class="w3-hover-gray sorteable">Nif</th>
                <th class="w3-hover-gray sorteable">Nombre</th>
                <th class="w3-hover-gray sorteable">Descripción</th>
                <th class="w3-hover-gray sorteable">Fecha</th>
              </tr>
            </thead>
            <tbody>
              <tr xbind="" xfor="proveedor in this.proveedores" id="row-{proveedor._id}">
                <td xbind="" class="w3-border-right">
                  <input xbind="checked:fn.checked=>@proveedor" type="checkbox" on-click="doAction:check-item:{index}">
                </td>
                <td xbind="" on-click="doAction:edit-row:{proveedor._id}">{proveedor._id}</td>
                <td xbind="">{proveedor._nif}</td>
                <td xbind="">{proveedor._nombre}</td>
                <td xbind="">{proveedor._descripcion|toUpperCase}</td>
                <td xbind="">
                  {proveedor._fechaDeAlta|fixDate}
                  <button type="button" xbind="" title="Añadir a favoritos" class="fa fa-star" style="float:right" on-click="doAddToFavorites:{proveedor._id}"></button>
                </td>
              </tr>
            </tbody>
          </table>        
          
        </div>   
      </div>

    </form>

    <div style="display:none">

    </div>


  </body>
  <script type="text/javascript"> __initAll(); </script>
</html>
