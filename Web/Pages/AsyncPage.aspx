<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AsyncPage.aspx.cs" Inherits="Usuarios_Formulario" EnableViewState="true" %>
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

  <script type="text/javascript" src="../js/Utils.js"></script>
  <script type="text/javascript">

      function _save() {
          var __method  = $('txtMethodName').value;
          var __payload = $('txtPayload').value;
          __remote.call(__method, __payload, function (res) {
              console.log(res);
              $('txtNombre').value = JSON.stringify(res);
          });
      }

      function alertDos(a, b, c) {
          alert('{0} - {1} - {2}'.format(a, b, c));
      }

  </script>

</head>
  <body>

      <form id="form1" runat="server" style="padding-bottom: 6em !important">

          <header class="w3-container w3-teal">
              <h2>Datos de usuario</h2>
          </header>

          <div class="w3-container w3-padding">
              <label class="w3-text-teal" for="txtMethodName"><b>Operación</b></label>
              <input type="text" id="txtMethodName" name="txtMethodName" class="w3-input w3-border w3-light-grey" />

              <label class="w3-text-teal" for="txtPayload"><b>Parámetros</b></label>
              <input type="text" id="txtPayload" name="txtPayload" class="w3-input w3-border w3-light-grey" />

              <label class="w3-text-teal" for="txtNombre"><b>Nombre</b></label>
              <input type="text" id="txtNombre" class="w3-input w3-border w3-light-grey" />

              <label class="w3-text-teal" for="txtDescripcion"><b>Descripción</b></label>
              <input type="text" id="txtDescripcion" class="w3-input w3-border w3-light-grey" />

              <label class="w3-text-teal" for="txtFechaDeAlta"><b>Fecha de alta</b></label>
              <input type="text" id="txtFechaDeAlta" class="w3-input w3-border w3-light-grey" />

              <label class="w3-text-teal" for="txtComentarios"><b>Comentarios</b></label>
              <textarea type="text" id="txtComentarios" name="txtComentarios" class="w3-input w3-border w3-light-grey"></textarea>

              <div class="w3-third w3-padding">
                  <h2>Checkboxes</h2>
                  <p>
                      <input id="checkMilk" name="checkMilk" class="w3-check" type="checkbox" />
                      <label>Milk</label>
                  </p>
                  <p>
                      <input id="checkSugar" name="checkSugar" class="w3-check" type="checkbox" />
                      <label>Sugar</label>
                  </p>
                  <p>
                      <input id="checkLemon" name="checkLemon" class="w3-check" type="checkbox" disabled="disabled" />
                      <label>Lemon (Disabled)</label>
                  </p>
              </div>

              <div class="w3-third w3-padding">
                  <h2>Radio Buttons</h2>
                  <input class="w3-radio" type="radio" name="gender" value="male" checked="checked" />
                  <label>Male</label>

                  <input class="w3-radio" type="radio" name="gender" value="female" />
                  <label>Female</label>

                  <input class="w3-radio" type="radio" name="gender" value="" disabled="disabled" />
                  <label>Don't know (Disabled)</label>
              </div>

              <div class="w3-third w3-padding">
                  <h2>Select Your Free Option</h2>
                  <select id="cmbOptions" name="cmbOptions" class="w3-select">
                      <option value="" disabled="disabled" selected="selected">Choose your option</option>
                      <option value="1">Option 1</option>
                      <option value="2">Option 2</option>
                      <option value="3">Option 3</option>
                  </select>
              </div>

          </div>

          <footer class="w3-container w3-teal w3-center w3-padding">
              <button class="w3-button w3-black" id="btnGrabar" type="button" onclick="_save()">Grabar</button>
          </footer>

      </form>

  </body>

</html>
