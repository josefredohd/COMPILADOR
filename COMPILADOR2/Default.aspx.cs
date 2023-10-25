using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Collections;
using System.Globalization;

namespace COMPILADOR2
{

    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            dt.Columns.AddRange(new DataColumn[5] { new DataColumn("Linea", typeof(String)),
            new DataColumn("Token", typeof(String)), new DataColumn("Error", typeof(String)), new DataColumn("Valor", typeof(String)), new DataColumn("TipDato", typeof(String))});
        }


        string contenidoArchivo;
        string Acum = null;
        static List<int> LToken = new List<int>();
        int cont = 0;
        int nextoken;
        int ENTERO = 140;
        int REAL = 141;
        int BOLEANO = 142;
        int CADENA = 143;
        int SI = 144;
        int ENTONCES = 145;
        int SINO = 146;
        int HASTA = 147;
        int HACER = 148;
        int MIENTRAS = 149;
        int FUNCION = 150;
        int INICIO = 151;
        int FIN = 152;
        int LEER = 153;
        int ESCRIBIR = 154;
        int DESDE = 155;
        int REGRESA = 156;
        int PROGRAMA = 157;
        int FALSO = 158;
        int VERDADERO = 159;
        int LineNum = 0;
        DataTable dt = new DataTable();


        List<string> PalRes = new List<string>
        {"ENTERO","REAL","BOLEANO","CADENA","SI","ENTONCES","SINO","HASTA","HACER","MIENTRAS","FUNCION","INICIO","FIN","LEER","ESCRIBIR","DESDE","REGRESA","PROGRAMA","FALSO","VERDADERO"};
        int[,] Matriz = new int[25, 26]
        { // 0…9   e.E    .     _     +     -     =     /    EOF-EOL   *     ^     %     :     >     <     Y     O     N    ""   (      )    ;   Espacio  a…z,A…Z     ,        O.C
            {  1,    7,   19,    7,    8,    9,   18,   10,    - 12,   14,  108,  111,   15,   16,   17,    7,    7,    7,   24,  129,  130,  131,       0,      7,   132,  - 10},
            {  1,    2,    4,  101,  101,  101,  101,  101,     101,  101,  101,  101,  101,  101,  101,  101,  101,  101,  101,  101,  101,  101,     101,    101,   101,   101},
            {  3,  - 1,  - 1,  - 1,  - 1,   6 ,  - 1,  - 1,     - 1,  - 1,  - 1,  - 1,  - 1,  - 1,  - 1,  - 1,  - 1,  - 1,   -1,   -1,   -1,   -1,      -1,     -1,    -1,   - 1},
            {  3,  101,  101,  101,  101,  101,  101,  101,     101,  101,  101,  101,  101,  101,  101,  101,  101,  101,  101,  101,  101,  101,     101,    101,   101,   101},
            {  5,  - 2,  - 2,  - 2,  - 2,  - 2,  - 2,  - 2,     - 2,  - 2,  - 2,  - 2,  - 2,  - 2,  - 2,  - 2,  - 2,  - 2,   -2,   -2,   -2,   -2,      -2,     -2,    -2,   - 2},
            {  5,  102,  102,  102,  102,  102,  102,  102,     102,  102,  102,  102,  102,  102,  102,  102,  102,  102,  102,  102,  102,  102,     102,    102,   102,   102},
            {  5,  - 2,  - 2,  - 2,  - 2,  - 2,  - 2,  - 2,     - 2,  - 2,  - 2,  - 2,  - 2,  - 2,  - 2,  - 2,  - 2,  - 2,   -2,   -2,   -2,   -2,      -2,     -2,    -2,   - 2},
            {  7,    7,  103,    7,  103,  103,  103,  103,     103,  103,  103,  103,  103,  103,  103,    7,    7,    7,  103,  103,  103,  103,     103,      7,   103,   103},
            {104,  104,  104,  104,  109,  104,  113,  104,     104,  104,  104,  104,  104,  104,  104,  104,  104,  104,  104,  104,  104,  104,     104,    104,   104,   104},
            {105,  105,  105,  105,  105,  110,  114,  105,     105,  105,  105,  105,  105,  105,  105,  105,  105,   10,  105,  105,  105,  105,     105,    105,   105,   105},
            {106,  106,  106,  106,  106,  106,  116,   11,     106,   12,  106,  106,  106,  106,  106,  106,  106,  106,  106,  106,  106,  106,     106,    106,   106,   106},
            { 11,   11,   11,   11,   11,   11,   11,   11,     117,   11,   11,   11,   11,   11,   11,   11,   11,   11,   11,   11,   11,   11,      11,     11,    11,    11},
            { 12,   12,   12,   12,   12,   12,   12,   12,     - 3,   13,   12,   12,   12,   12,   12,   12,   12,   12,   12,   12,   12,   12,      12,     12,    12,    12},
            { 12,   12,   12,   12,   12,   12,   12,  128,     - 3,   12,   12,   12,   12,   12,   12,   12,   12,   12,   12,   12,   12,   12,      12,     12,    12,    12},
            {107,  107,  107,  107,  107,  107,  115,  107,     107,  107,  107,  107,  107,  107,  107,  107,  107,  107,  107,  107,  107,  107,     107,    107,   107,   107},
            {- 9,  - 9,  - 9,  - 9,  - 9,  - 9,  112,  - 9,     - 9,  - 9,  - 9,  - 9,  - 9,  - 9,  - 9,  - 9,  - 9,  - 9,  - 9,  - 9,  - 9,  - 9,     - 9,    - 9,   - 9,   - 9},
            {118,  118,  118,  118,  118,  118,  120,  118,     118,  118,  118,  118,  118,  118,  118,  118,  118,  118,  118,  118,  118,  118,     118,    118,   118,   118},
            {119,  119,  119,  119,  119,  119,  121,  119,     119,  119,  119,  119,  119,  122,  119,  119,  119,  119,  119,  119,  119,  119,     119,    119,   119,   119},
            {- 4,  - 4,  - 4,  - 4,  - 4,  - 4,  123,  - 4,     - 4,  - 4,  - 4,  - 4,  - 4,  - 4,  - 4,  - 4,  - 4,  - 4,  - 4,  - 4,  - 4,  - 4,     - 4,    - 4,   - 4,   - 4},
            {- 6,  - 6,  - 6,  - 6,  - 6,  - 6,  - 6,  - 6,     - 6,  - 6,  - 6,  - 6,  - 6,  - 6,  - 6,   20,   21,   22,   -6,   -6,   -6,   -6,      -6,     -6,    -6,   - 6},
            {- 7,  - 7,  124,  - 7,  - 7,  - 7,  - 7,  - 7,     - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,   -7,   -7,   -7,   -7,      -7,     -7,    -7,   - 7},
            {- 7,  - 7,  125,  - 7,  - 7,  - 7,  - 7,  - 6,     - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,     - 7,    - 7,   - 7,   - 7},
            {- 5,  - 5,  - 5,  - 5,  - 5,  - 5,  - 5,  - 5,     - 5,  - 5,  - 5,  - 5,  - 5,  - 5,  - 5,  - 5,   23,  - 5,  - 5,  - 5,  - 5,  - 5,     - 5,    - 5,   - 5,   - 5},
            {- 7,  - 7,  126,  - 7,  - 7,  - 7,  - 7,  - 7,     - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,  - 7,     - 7,    - 7,   - 7,   - 7},
            { 24,   24,   24,   24,    24,  24,   24,   24,     - 8,   24,   24,   24,   24,   24,   24,   24,   24,   24,   127,   24,   24,   24,     24,     24,    24,   - 8}
        };



        protected void btnLimpiarToken_Click(object sender, EventArgs e)
        {
            tabla.DataSource = null;
            tabla.DataBind();
            LToken.Clear();
            PalRes.Clear();
        }

        protected void btnLimpiarArch_Click(object sender, EventArgs e)
        {
            editArch.Text = "";
        }

        protected void btnSubir_Click(object sender, EventArgs e)
        {
            string path = Server.MapPath("/arch/");
            if (FileUpload1.HasFile)
            {

                string extension = System.IO.Path.GetExtension(FileUpload1.PostedFile.FileName);
                if (extension == ".txt")
                {
                    string sourceFile = @"/home/fredo/Desktop/COMPILADOR2/COMPILADOR2/arch/FREDO.txt";
                    string destinationFile = @"/home/fredo/archivos/FREDO.txt";
                    if (File.Exists(destinationFile))
                    {
                        File.Delete(destinationFile);
                    }
                    FileUpload1.SaveAs(path + "FREDO" + extension);
                    string script = string.Format("swal('Archivo cargado exitosamente', '', 'success');");
                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                    System.IO.File.Copy(sourceFile, destinationFile);
                }
                else
                {
                    string script = string.Format("swal('Solo se admiten archivos .txt', '', 'error');");
                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                }
            }
            else
            {
                string script = string.Format("swal('Debes seleccionar un archivo!', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
            }
        }

        protected void btnArch_Click(object sender, EventArgs e)
        {
            /*TextReader Leer = new StreamReader("FREDO.txt");
            listArch.Items.Clear();
            while ((contenidoArchivo = Leer.ReadLine()) != null)
            {
                listArch.Items.Add(contenidoArchivo);
            }*/

            string fileText;
            string path = "/home/fredo/archivos/FREDO.txt";
            bool result = File.Exists(path);
            //StreamReader sr = new StreamReader("/home/ubuntu/archivos/FREDO.txt");
            {
                if (result == false)
                {
                    string script = string.Format("swal('No existe el archivo', '', 'error');");
                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                }
                else
                {
                    StreamReader sr = new StreamReader("/home/fredo/archivos/FREDO.txt");
                    fileText = sr.ReadToEnd();
                     editArch.Text = fileText;
                       sr.Dispose();
                }
            }
            
        }

        public void LeerArchivo()
        {
            string fileText;
            string path = "/home/fredo/archivos/FREDO.txt";
            bool result = File.Exists(path);
            //StreamReader sr = new StreamReader("/home/ubuntu/archivos/FREDO.txt");
            {
                if (result == false)
                {
                    string script = string.Format("swal('No existe el archivo', '', 'error');");
                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                }
                else
                {
                    StreamReader sr = new StreamReader("/home/fredo/archivos/FREDO.txt");
                    fileText = sr.ReadToEnd();
                    editArch.Text = fileText;
                    sr.Dispose();
                }
            }
        }

        protected void btnGuardarArch_Click(object sender, EventArgs e)
        {
            if (editArch.Text.Length == 0)
            {
                string script2 = string.Format("swal('No se puede guardar un archivo vacío', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script2, true);
            }
            else
            {
                string destinationFile = @"/home/fredo/archivos/FREDO.txt";
                if (File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }
                    string writerfile = @"/home/fredo/archivos/FREDO.txt";
                    System.IO.File.WriteAllText(writerfile, this.editArch.Text);
                    string script = string.Format("swal('Archivo modificado exitosamente', '', 'success');");
                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);

            }
        }

        protected void btnTokenArch_Click(object sender, EventArgs e)
        {
            //dt.Columns.AddRange(new DataColumn[5] { new DataColumn("Linea", typeof(String)),
            //new DataColumn("Token", typeof(String)), new DataColumn("Error", typeof(String)), new DataColumn("Valor", typeof(String)), new DataColumn("TipDato", typeof(String))});

            TextReader LeerLine = new StreamReader(@"/home/fredo/archivos/FREDO.txt");

            while ((contenidoArchivo = LeerLine.ReadLine()) != null)
            {

                string Caracter = contenidoArchivo;
                LineNum++;
                int Estado = 0;
                int Columna = 0;
                int i = 0;
                char resultado;
                do
                {
                    if (i < Caracter.Length)
                    {
                        resultado = Caracter[i];
                    }
                    else
                    {
                        if (i >= Caracter.Length)
                        {
                            resultado = Convert.ToChar(03);
                        }
                        else
                        {
                            resultado = '@';
                        }
                    }
                    switch (resultado)
                    {
                        case char resulfin when (resulfin >= 48 && resulfin <= 57):
                            {
                                Acum = Acum + resultado;
                                Columna = 0;
                                break;
                            }
                        case char resulfin when (resulfin == 101 || resulfin == 69):
                            {
                                Acum = Acum + resultado;
                                Columna = 1;
                                break;
                            }
                        case char resulfin when (resulfin == 46):
                            {
                                Acum = Acum + resultado;
                                Columna = 2;
                                break;
                            }
                        case char resulfin when (resulfin == 95):
                            {
                                Acum = Acum + resultado;
                                Columna = 3;
                                break;
                            }
                        case char resulfin when (resulfin == 43):
                            {
                                Acum = Acum + resultado;
                                Columna = 4;
                                break;
                            }
                        case char resulfin when (resulfin == 45):
                            {
                                Acum = Acum + resultado;
                                Columna = 5;
                                break;
                            }
                        case char resulfin when (resulfin == 61):
                            {
                                Acum = Acum + resultado;
                                Columna = 6;
                                break;
                            }
                        case char resulfin when (resulfin == 47):
                            {
                                Acum = Acum + resultado;
                                Columna = 7;
                                break;
                            }
                        case char resulfin when (resulfin == 04):
                            {
                                Acum = Acum + resultado;
                                Columna = 8;
                                break;
                            }
                        case char resulfin when (resulfin == 42):
                            {
                                Acum = Acum + resultado;
                                Columna = 9;
                                break;
                            }
                        case char resulfin when (resulfin == 94):
                            {
                                Acum = Acum + resultado;
                                Columna = 10;
                                break;
                            }
                        case char resulfin when (resulfin == 37):
                            {
                                Acum = Acum + resultado;
                                Columna = 11;
                                break;
                            }
                        case char resulfin when (resulfin == 58):
                            {
                                Acum = Acum + resultado;
                                Columna = 12;
                                break;
                            }
                        case char resulfin when (resulfin == 62):
                            {
                                Acum = Acum + resultado;
                                Columna = 13;
                                break;
                            }
                        case char resulfin when (resulfin == 60):
                            {
                                Acum = Acum + resultado;
                                Columna = 14;
                                break;
                            }
                        case char resulfin when (resulfin == 89):
                            {
                                Acum = Acum + resultado;
                                Columna = 15;
                                break;
                            }
                        case char resulfin when (resulfin == 79):
                            {
                                Acum = Acum + resultado;
                                Columna = 16;
                                break;
                            }
                        case char resulfin when (resulfin == 78):
                            {
                                Acum = Acum + resultado;
                                Columna = 17;
                                break;
                            }
                        case char resulfin when (resulfin == 34):
                            {
                                Acum = Acum + resultado;
                                Columna = 18;
                                break;
                            }
                        case char resulfin when (resulfin == 40):
                            {
                                Acum = Acum + resultado;
                                Columna = 19;
                                break;
                            }
                        case char resulfin when (resulfin == 41):
                            {
                                Acum = Acum + resultado;
                                Columna = 20;
                                break;
                            }
                        case char resulfin when (resulfin == 59):
                            {
                                Acum = Acum + resultado;
                                Columna = 21;
                                break;
                            }
                        case char resulfin when (resulfin == 32):
                            {
                                Acum = Acum + resultado;
                                Columna = 22;
                                break;
                            }
                        case char resulfin when ((resulfin >= 97 && resulfin <= 122) || (resulfin >= 65 && resulfin <= 90) || (resulfin == 164 && resulfin == 165)):
                            {
                                Acum = Acum + resultado;
                                Columna = 23;
                                break;
                            }
                        case char resulfin when (resulfin == 44):
                            {
                                Acum = Acum + resultado;
                                Columna = 24;
                                break;
                            }
                        default:
                            {
                                Columna = 25;
                                break;
                            }
                    }
                    i++;
                    Estado = Matriz[Estado, Columna];
                    if (Estado > 100)
                    {
                        if (Estado == 101)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Numero");
                            if (Estado == 101 && resultado == 32 || Estado == 101 && Columna == 25 || Estado == 101 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Entero");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                                Estado = 0;

                            }
                            if (Estado == 101)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Entero");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 102)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Decimal");
                            if (Estado == 102 && resultado == 32 || Estado == 102 && Columna == 25 || Estado == 102 && Columna == 8)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Decimal");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                                Estado = 0;

                            }
                            if (Estado == 102)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Decimal");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 103)
                        {
                            if (Estado == 103 && resultado == 32 || Estado == 103 && Columna == 25 || Estado == 103 && Columna == 8)
                            {
                                for (int b = 0; b < PalRes.Count; b++)
                                {
                                    if (PalRes[b].Equals(Acum, StringComparison.OrdinalIgnoreCase))
                                    {
                                        Estado = 140 + b;
                                        dt.Rows.Add(LineNum, Estado, "", Acum, "Palabra Reservada");
                                        tabla.DataSource = dt;
                                        tabla.DataBind();
                                        LToken.Add(Estado);
                                        break;
                                    }
                                    if (b == 19)
                                    {
                                        dt.Rows.Add(LineNum, Estado, "", Acum, "Identificador");
                                        tabla.DataSource = dt;
                                        tabla.DataBind();
                                        LToken.Add(Estado);
                                        Estado = 0;
                                    }
                                }
                            }
                            if (Estado == 103)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                for (int b = 0; b < PalRes.Count; b++)
                                {
                                    if (PalRes[b].Equals(Acum, StringComparison.OrdinalIgnoreCase))
                                    {
                                        Estado = 140 + b;
                                        dt.Rows.Add(LineNum, Estado, "", Acum, "Palabra Reservada");
                                        tabla.DataSource = dt;
                                        tabla.DataBind();
                                        LToken.Add(Estado);
                                        break;
                                    }
                                    if (b == 19)
                                    {
                                        dt.Rows.Add(LineNum, Estado, "", Acum, "Identificador");
                                        tabla.DataSource = dt;
                                        tabla.DataBind();
                                        LToken.Add(Estado);
                                    }

                                }

                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 104)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Suma");
                            if (Estado == 104 && resultado == 32 || Estado == 104 && Columna == 25 || Estado == 104 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Suma");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                                Estado = 0;
                            }
                            if (Estado == 104)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Suma");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 105)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Resta");
                            if (Estado == 105 && resultado == 32 || Estado == 105 && Columna == 25 || Estado == 105 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "R");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                                Estado = 0;

                            }
                            if (Estado == 105)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Resta");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 106)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " División");
                            if (Estado == 106 && resultado == 32 || Estado == 106 && Columna == 25 || Estado == 106 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "División");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                                Estado = 0;

                            }
                            if (Estado == 106)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Divisón");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 107)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Multiplicador");
                            if (Estado == 107 && resultado == 32 || Estado == 107 && Columna == 25 || Estado == 107 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Multiplicador");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                                Estado = 0;

                            }
                            if (Estado == 107)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Multiplicador");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 108)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Potencia");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Potencia");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 109)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Incrementador");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Incrementador");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            Acum = "";
                        }
                        if (Estado == 110)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Decrementador");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Decrementador");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 111)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Modulador");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Modulador");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 112)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Operador de Asignacion");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Operador de Asignación");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 113)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Operador de Asignacion");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Operador de Asignación");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 114)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Operador de Asignacion");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Operador de Asignación");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 115)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Operador de Asignacion");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Operador de Asignación");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 116)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Operador de Asignacion");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Operador de Asignación");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 117)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Comentario de una sola linea");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Comentario");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 118)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Mayor que");
                            if (Estado == 118 && resultado == 32 || Estado == 118 && Columna == 25 || Estado == 118 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Mayor que");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                                Estado = 0;

                            }
                            if (Estado == 118)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Mayor que");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 119)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Menor que");
                            if (Estado == 119 && resultado == 32 || Estado == 119 && Columna == 25 || Estado == 119 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Menor que");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                                Estado = 0;

                            }
                            if (Estado == 119)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Menor que");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(Estado);
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 120)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Mayor igual que");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Mayor igual que");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 121)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Menor igual que");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Menor igual que");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 122)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Diferente que");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Diferente que");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 123)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Igual que");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Igual que");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 124)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Y");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Y");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 125)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " O");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "O");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 126)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " NO");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "NO");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 127)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Cadena");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Cadena");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 128)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Comentario en bloque");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Comentario en bloque");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 129)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Paréntesis abierto");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Paréntesis abierto");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 130)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Paréntesis cerrado");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Paréntesis cerrado");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 131)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Punto y Coma");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Punto y coma");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        if (Estado == 132)
                        {
                            //listToken.Items.Add(Convert.ToString(Estado) + " Coma");
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Coma");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(Estado);
                            Acum = "";
                        }
                        Estado = 0;
                    }
                    if (Estado < 0)
                    {
                        if (Estado <= -1 && Estado >= -8)
                        {
                            ListErrores(Estado);
                            Estado = 0;
                        }
                        Acum = "";
                    }
                }
                while (Estado >= 0 && Estado <= 100);
            }
            string msg = string.Format("swal('Fin de Cadena', '', 'warning');");
            ClientScript.RegisterStartupScript(this.GetType(), "swal", msg, true);
            LineNum = 0;
        }


        public void ListErrores(int Error)
        {

            switch (Error)
            {
                case -1:
                    {
                        string script = string.Format("swal('ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        //listToken.Items.Add(Convert.ToString(Error + " Error: Se esperaba un número o un -"));
                        dt.Rows.Add(LineNum, "", Error, "Error: Se esperaba un número o un -");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
                case -2:
                    {
                        Acum = "";
                        string script = string.Format("swal('ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        //listToken.Items.Add(Convert.ToString(Error + " Error: Se esperaba un número"));
                        dt.Rows.Add(LineNum, "", Error, "Error: Se esperaba un número");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
                case -3:
                    {
                        Acum = "";
                        string script = string.Format("swal('ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        //listToken.Items.Add(Convert.ToString(Error + " Error"));
                        dt.Rows.Add(LineNum, "", Error, "Error: Se esperaba /");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
                case -4:
                    {
                        Acum = "";
                        string script = string.Format("swal('ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        //listToken.Items.Add(Convert.ToString(Error + " Error: Se esperaba un ="));
                        dt.Rows.Add(LineNum, "", Error, "Error: Se esperaba un ==");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
                case -5:
                    {
                        Acum = "";
                        string script = string.Format("swal('ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        //listToken.Items.Add(Convert.ToString(Error + " Error: Se esperaba una O"));
                        dt.Rows.Add(LineNum, "", Error, "Error: Se esperaba una 0");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
                case -6:
                    {
                        Acum = "";
                        string script = string.Format("swal('ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        //listToken.Items.Add(Convert.ToString(Error + " Error: Se esperaba un operador lógico"));
                        dt.Rows.Add(LineNum, "", Error, "Error: Se esperaba un operador lógico");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
                case -7:
                    {
                        Acum = "";
                        string script = string.Format("swal('ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        //listToken.Items.Add(Convert.ToString(Error + " Error: Se esperaba un punto"));
                        dt.Rows.Add(LineNum, "", Error, "Error: Se esperaba un punto");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
                case -8:
                    {
                        Acum = "";
                        string script = string.Format("ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        //listToken.Items.Add(Convert.ToString(Error + " Error: Se esperaban comillas"));
                        dt.Rows.Add(LineNum, "", Error, "Error: Se esperaban comillas");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
                case -9:
                    {
                        Acum = "";
                        string script = string.Format("swal('ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        //listToken.Items.Add(Convert.ToString(Error + " Error: Caracter inválido"));
                        dt.Rows.Add(LineNum, "", Error, "Error: Caracter inválido");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
            }
        }

    }
}