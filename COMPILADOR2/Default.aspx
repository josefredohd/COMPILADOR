﻿<%@ Page Language="C#" Inherits="COMPILADOR2.Default" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Compilador</title>
    <link rel="shortcut icon" href="favicon-32x32.png">
    <link href="StyleSheet.css" rel="stylesheet" type="text/css" />
    <script src="https://unpkg.com/sweetalert/dist/sweetalert.min.js"></script>
    <script src="codemirror/lib/codemirror.js"></script>
    <link href="codemirror/lib/codemirror.css" rel="stylesheet"/>
    <script src="codemirror/mode/xml.js"></script>
    <link href="codemirror/theme/elegant.css" rel="stylesheet"/>
        
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.1/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-4bw+/aepP/YC94hEpVNVgiZdgIC5+VKNBQNGCHeKRQN+PtmoHDEXuppvnDJzQIu9" crossorigin="anonymous">
    <style type="text/css">
        .CodeMirror {border: 1px solid black; margin: 0;}
        .contenedor { text-align: center; margin-top: 50px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-sm">
            <h1 class="text-center font-italic">COMPILADOR</h1> 
            <h3 class="text-center font-italic">Hecho Por: José Alfredo Hernández Félix</h3> 
      
            <asp:TextBox ID="editArch" class="NumLine form-control" TextMode="MultiLine" runat="server" Height="210px" Width="500px"></asp:TextBox>
            <script>
            var editor = CodeMirror.fromTextArea
            (document.getElementById('editArch'), {
               mode: "xml",
               theme: "elegant",
               lineNumbers: true
             });

            </script>

         <div class="flex-shrink-0 text-center">
            <div class="mb-3"> <!-- Agregamos margen inferior para espaciado -->
                <asp:FileUpload ID="FileUpload1" class="form-control mt-2" accept=".txt" runat="server" />
                <asp:Button ID="btnSubir" class="btn btn-primary mt-2" runat="server" OnClick="btnSubir_Click" Text="Subir archivo" />
                <asp:Button ID="btnTokenArch" class="btn btn-primary mt-2" runat="server" OnClick="btnTokenArch_Click" Text="Tokens Archivo" />
                <asp:Button ID="btnArch" class="btn btn-primary mt-2" runat="server" OnClick="btnArch_Click" Text="Leer Archivo" />
                <asp:Button ID="btnGuardarArch" class="btn btn-primary mt-2" runat="server" Text="Modificar archivo" OnClick="btnGuardarArch_Click" />
                <asp:Button ID="btnLimpiarArch" class="btn btn-secondary mt-2" runat="server" OnClick="btnLimpiarArch_Click" Text="Limpiar Archivo" />
                <asp:Button ID="btnLimpiarToken" class="btn btn-secondary mt-2" runat="server" Text="Limpiar Tokens" OnClick="btnLimpiarToken_Click" />
            </div>

            <div class="text-center" style="height: 350px; overflow: auto"> <!-- Centro la tabla aquí -->
                <asp:GridView id="tabla" runat="server" CssClass="table table-bordered table-striped" style="width: 100%;" AutoGenerateColumns="false">
                    <HeaderStyle CssClass="table-primary" />
                    <Columns>
                        <asp:BoundField DataField="Linea" HeaderText="Línea" />
                        <asp:BoundField DataField="Token" HeaderText="Token" />
                        <asp:BoundField DataField="Error" HeaderText="Error" />
                        <asp:BoundField DataField="Valor" HeaderText="Valor" />
                        <asp:BoundField DataField="TipDato" HeaderText="Tipo de Dato" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        </div>
    </form>
</body>    
</html>