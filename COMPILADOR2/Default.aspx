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
      
            
            <asp:Button ID="btnLimpiarToken" class="btn btn-secondary" runat="server" Text="Limpiar Tokens" OnClick="btnLimpiarToken_Click" />
            <br>
            <asp:TextBox ID="editArch" class="NumLine form-control" TextMode="MultiLine" runat="server" Height="210px" Width="500px"></asp:TextBox>
            <script>
            var editor = CodeMirror.fromTextArea
            (document.getElementById('editArch'), {
               mode: "xml",
               theme: "elegant",
               lineNumbers: true
             });

            </script>

            <asp:FileUpload ID="FileUpload1" class="form-control" accept=".txt" runat="server" />
            <asp:Button ID="btnSubir" class="btn btn-primary" runat="server" OnClick="btnSubir_Click" Text="Subir archivo" />
            <asp:Button ID="btnTokenArch" class="btn btn-primary" runat="server" OnClick="btnTokenArch_Click" Text="Tokens Archivo" />
            <asp:Button ID="btnArch" class="btn btn-primary" runat="server" OnClick="btnArch_Click" Text="Leer Archivo" />
            <asp:Button ID="btnGuardarArch" class="btn btn-primary" runat="server" Text="Modificar archivo" OnClick="btnGuardarArch_Click" />
            <asp:Button ID="btnLimpiarArch" class="btn btn-secondary" runat="server" Text="Limpiar Archivo" OnClick="btnLimpiarArch_Click" />
            
            <br>
            <asp:GridView id="tabla" runat="server" CssClass="table table-bordered table-striped" style="margin: 0 auto; width: 80%;" AutoGenerateColumns="false">
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
    </form>
</body>    
</html>
