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
        bool dvar = false;
        static List<COMPILADOR2.Tokens> LToken = new List<Tokens>();
        static List<Variables> LVar = new List<Variables>();
        static List<String> ListFuncion = new List<string>();
        static List<Variables> ListParam = new List<Variables>();
        // Variables para llvear el control de las Expresiones
        Nullable<int> Exp, Exp1, RExp;
        // Variable para llevar control de Asignación
        string Asig = null;
        //Declaración de variables
        string Tipo, Valor, Pertenece;
        //Llevar control de la funcion
        int TipoFuncion;
        string GFuncion;
        //Comparar funcion 
        string FunLlama = null;
        string Id, Id2;
        //Controlador del parametro actual
        int ContParAc = 0;
        //Contador del parametro de la funcion llamada
        int ContParFunc = 0;
        //Contador del total de parametros de la funcion llamada
        int ContPar = 0;
        //Variable para  devolver expresion logica
        int LOGICA = 4;
        int cont = 0;
        int nextok;
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
        int LLAMAR = 160;
        int LineNum = 0;
        DataTable dt = new DataTable();


        static List<string> PalRes = new List<string>
        {"ENTERO","REAL","BOLEANO","CADENA","SI","ENTONCES","SINO","HASTA","HACER","MIENTRAS","FUNCION","INICIO","FIN","LEER","ESCRIBIR","DESDE","REGRESA","PROGRAMA","FALSO","VERDADERO","LLAMAR"};
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
            LVar.Clear();
        }

        //Función para reiniciar la sintaxis
        public void Reiniciar()
        {
            tabla.DataSource = null;
            tabla.DataBind();
            LToken.Clear();
            LVar.Clear();
            ListParam.Clear();
            ListFuncion.Clear();
            cont = 0;
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

            StreamReader LeerLine = new StreamReader(@"/home/fredo/archivos/FREDO.txt");

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
                        case char resulfin when (resulfin == 03):
                            {
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
                            if (Estado == 101 && resultado == 32 || Estado == 101 && Columna == 25 || Estado == 101 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Entero");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Entero",
                                    Lexema = Acum
                                });
                                Estado = 0;

                            }
                            if (Estado == 101)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Entero");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Entero",
                                    Lexema = Acum
                                });
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 102)
                        {
                            if (Estado == 102 && resultado == 32 || Estado == 102 && Columna == 25 || Estado == 102 && Columna == 8)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Real");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Real",
                                    Lexema = Acum
                                });
                                Estado = 0;
                            }
                            if (Estado == 102)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Decimal");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Real",
                                    Lexema = Acum
                                });
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
                                        LToken.Add(new Tokens()
                                        {
                                            NumTok = Estado,
                                            Descripcion = "Palabra Reservada",
                                            Lexema = Acum
                                        });
                                        if (Estado == ENTERO || Estado == CADENA || Estado == BOLEANO || Estado == REAL)
                                        {
                                            Tipo = Acum;
                                        }
                                        break;
                                    }
                                    if (b == 20)
                                    {
                                        if(Tipo != null)
                                        {
                                            Id = Acum;
                                            if (Guardar() == false)
                                            {
                                                for (int g = 0; g < LVar.Count; g++)
                                                {
                                                    if (LVar[g].id == Acum)
                                                    {
                                                        dt.Rows.Add(LineNum, Estado, "", Acum, LVar[g].tipo);
                                                        tabla.DataSource = dt;
                                                        tabla.DataBind();
                                                        LToken.Add(new Tokens()
                                                        {
                                                            NumTok = Estado,
                                                            Descripcion = LVar[g].tipo,
                                                            Lexema = Acum
                                                        });
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int g = 0; g < LVar.Count; g++)
                                            {
                                                if (LVar[g].id == Acum)
                                                {
                                                    dt.Rows.Add(LineNum, Estado, "", Acum, LVar[g].tipo);
                                                    tabla.DataSource = dt;
                                                    tabla.DataBind();
                                                    LToken.Add(new Tokens()
                                                    {
                                                        NumTok = Estado,
                                                        Descripcion = LVar[g].tipo,
                                                        Lexema = Acum
                                                    });
                                                    Acum = "";
                                                    break;
                                                }
                                            }
                                            if (Acum != "")
                                            {
                                                dt.Rows.Add(LineNum, Estado, "", Acum, "Identificador");
                                                tabla.DataSource = dt;
                                                tabla.DataBind();
                                                LToken.Add(new Tokens()
                                                {
                                                    NumTok = Estado,
                                                    Descripcion = "Identificador",
                                                    Lexema = Acum
                                                });
                                            }
                                            Estado = 0;
                                        }
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
                                        LToken.Add(new Tokens()
                                        {
                                            NumTok = Estado,
                                            Descripcion = "Palabra Reservada",
                                            Lexema = Acum
                                        });
                                        if (Estado == ENTERO || Estado == CADENA || Estado == BOLEANO || Estado == REAL)
                                        {
                                            Tipo = Acum;
                                        }
                                        break;
                                    }
                                    if (b == 20)
                                    {

                                        if (Tipo != null)
                                        {
                                            Id = Acum;
                                            if (Guardar() == false)
                                            {
                                                for (int g = 0; g < LVar.Count; g++)
                                                {
                                                    if (LVar[g].id == Acum)
                                                    {
                                                        dt.Rows.Add(LineNum, Estado, "", Acum, LVar[g].tipo);
                                                        tabla.DataSource = dt;
                                                        tabla.DataBind();
                                                        LToken.Add(new Tokens()
                                                        {
                                                            NumTok = Estado,
                                                            Descripcion = LVar[g].tipo,
                                                            Lexema = Acum
                                                        });
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int g = 0; g < LVar.Count; g++)
                                            {
                                                if (LVar[g].id == Acum)
                                                {
                                                    dt.Rows.Add(LineNum, Estado, "", Acum, LVar[g].tipo);
                                                    tabla.DataSource = dt;
                                                    tabla.DataBind();
                                                    LToken.Add(new Tokens()
                                                    {
                                                        NumTok = Estado,
                                                        Descripcion = LVar[g].tipo,
                                                        Lexema = Acum
                                                    });
                                                    Acum = "";
                                                    break;
                                                }
                                            }
                                            if (Acum != "")
                                            {
                                                dt.Rows.Add(LineNum, Estado, "", Acum, "Identificador");
                                                tabla.DataSource = dt;
                                                tabla.DataBind();
                                                LToken.Add(new Tokens()
                                                {
                                                    NumTok = Estado,
                                                    Descripcion = "Identificador",
                                                    Lexema = Acum
                                                });
                                            }
                                            Estado = 0;
                                        }
                                    }
                                }
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 104)
                        {
                            if (Estado == 104 && resultado == 32 || Estado == 104 && Columna == 25 || Estado == 104 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Suma");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Suma",
                                    Lexema = Acum
                                });
                                Estado = 0;
                            }
                            if (Estado == 104)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Suma");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Suma",
                                    Lexema = Acum
                                });
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 105)
                        {
                            if (Estado == 105 && resultado == 32 || Estado == 105 && Columna == 25 || Estado == 105 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "R");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Resta",
                                    Lexema = Acum
                                });
                                Estado = 0;

                            }
                            if (Estado == 105)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Resta");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Resta",
                                    Lexema = Acum
                                });
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 106)
                        {
                            if (Estado == 106 && resultado == 32 || Estado == 106 && Columna == 25 || Estado == 106 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "División");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "División",
                                    Lexema = Acum
                                });
                                Estado = 0;

                            }
                            if (Estado == 106)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Divisón");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "División",
                                    Lexema = Acum
                                });
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 107)
                        {
                            if (Estado == 107 && resultado == 32 || Estado == 107 && Columna == 25 || Estado == 107 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Multiplicador");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Multiplicador",
                                    Lexema = Acum
                                });
                                Estado = 0;

                            }
                            if (Estado == 107)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Multiplicador");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Multiplicador",
                                    Lexema = Acum
                                });
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 108)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Potencia");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Potencia",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 109)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Incrementador");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Incrementador",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 110)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Decrementador");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Decrementador",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 111)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Modulador");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Modulador",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 112)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Operador de Asignación(:=)");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Operador de Asignación",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 113)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Operador de Asignación(+=)");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Operador de Asignación",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 114)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Operador de Asignación(-=)");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Operador de Asignación",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 115)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Operador de Asignación(*=)");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Operador de Asignación",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 116)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Operador de Asignación(/=)");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Operador de Asignación",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 117)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Comentario");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Comentario",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 118)
                        {
                            if (Estado == 118 && resultado == 32 || Estado == 118 && Columna == 25 || Estado == 118 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Mayor que");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Mayor que",
                                    Lexema = Acum
                                });
                                Estado = 0;

                            }
                            if (Estado == 118)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Mayor que");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Mayor que",
                                    Lexema = Acum
                                });
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 119)
                        {
                            if (Estado == 119 && resultado == 32 || Estado == 119 && Columna == 25 || Estado == 119 && Columna == 8)
                            {
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Menor que");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Menor que",
                                    Lexema = Acum
                                });
                                Estado = 0;

                            }
                            if (Estado == 119)
                            {
                                Acum = Acum.Remove(Acum.Length - 1);
                                dt.Rows.Add(LineNum, Estado, "", Acum, "Menor que");
                                tabla.DataSource = dt;
                                tabla.DataBind();
                                LToken.Add(new Tokens()
                                {
                                    NumTok = Estado,
                                    Descripcion = "Menor que",
                                    Lexema = Acum
                                });
                            }
                            Acum = "";
                            i--;
                        }
                        if (Estado == 120)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Mayor igual que");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Mayor igual que",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 121)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Menor igual que");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Menor igual que",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 122)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Diferente que");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Diferente que",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 123)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Igual que");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Igual que",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 124)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Y");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Y",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 125)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "O");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "O",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 126)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "NO");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "NO",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 127)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Cadena");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Cadena",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 128)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Comentario en bloque");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Comentario en bloque",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 129)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Paréntesis abierto");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Paréntesis abierto",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 130)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Paréntesis cerrado");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Paréntesis cerrado",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 131)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Punto y coma");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Punto y coma",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        if (Estado == 132)
                        {
                            dt.Rows.Add(LineNum, Estado, "", Acum, "Coma");
                            tabla.DataSource = dt;
                            tabla.DataBind();
                            LToken.Add(new Tokens()
                            {
                                NumTok = Estado,
                                Descripcion = "Coma",
                                Lexema = Acum
                            });
                            Acum = "";
                        }
                        Estado = 0;
                    }
                    if (Estado < 0)
                    {
                        if (Estado <= -1 && Estado >= -10)
                        {
                            ListErrores(Estado);
                            Estado = 0;
                        }
                    }
                }
                while (Estado >= 0 && Estado <= 100);
            }
            //string msg = string.Format("swal('Fin de Cadena', '', 'warning');");
            //ClientScript.RegisterStartupScript(this.GetType(), "swal", msg, true);
            LineNum = 0;
        }

        //Botón para sintaxis (bloque principal)
        protected void btnCompilar_Click(object sender, EventArgs e)
        {
            nextok = LToken[cont].NumTok;
            if (nextok != PROGRAMA)//Si el siguiente token es diferente a Programa, da error
            {
                string script = string.Format("swal('Se esperaba Programa', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                Reiniciar();
            }
            NextToken();
            if (nextok != 103)//Si el siguiente token es diferente a 103, da error
            {
                string script1 = string.Format("swal('Se esperaba un identificador', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script1, true);
                Reiniciar();
            }
            NextToken();
            if (nextok != 131)
            {
                string script2 = string.Format("swal('Se esperaba ;', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script2, true);
                Reiniciar();
            }
            NextToken();
            Pertenece = "Global";//Se usará para saber qué variables son globales
            DecVariables();
            Funcion();
            Bloque();
            string script3 = string.Format("swal('Compilado correctamente', '', 'error');");
            ClientScript.RegisterStartupScript(this.GetType(), "swal", script3, true);
        }

        //Función para saber el siguiente Token
        public void NextToken()
        {
            if (cont < LToken.Count)
            {
                cont++;
                nextok = LToken[cont].NumTok;
            }
            else
            {
                nextok = 0;
            }
        }

        //Función para la declaración de variables
        public void DecVariables()
        {
            if (nextok == ENTERO || nextok == CADENA || nextok == REAL || nextok == BOLEANO)//Si el siguiente token es un tipo de dato
            {
                do
                {
                    Tipo = LToken[cont].Lexema; //Se guarda el tipo para la variable a declarar
                    NextToken();
                    if (nextok != 103)
                    {
                        string script = string.Format("swal('Se esperaba un identificador', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        Reiniciar();
                    }
                    Id = LToken[cont].Lexema;
                    for (int i = 0; i < LVar.Count; i++)
                    {
                        if (Id == LVar[i].id)//Si la variable que se esta declarando ya ha sido declarada anteriormente, da error
                        {
                            string script = string.Format("swal('La variable {0} ya se ha declarado', '', 'error');", Id);
                            ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                            Reiniciar();
                        }
                    }
                    NextToken();
                    if (nextok == 112)
                    {
                        do
                        {
                            NextToken();
                            if (nextok == 101 || nextok == 102 || nextok == 127 || nextok == FALSO || nextok == VERDADERO)
                            {
                                Valor = LToken[cont].Lexema;//Se guarda el valor asignado
                                ValorVar(Tipo, nextok, Valor);//Verificamos si el valor es compatible al tipo de dato que se le asigno a la variable
                                if (nextok == 132)
                                {
                                    do
                                    {
                                        NextToken();
                                        GuardarVar();
                                        if (nextok != 103)
                                        {
                                            string script = string.Format("swal('Se esperaba un identificador', '', 'error');");
                                            ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                            Reiniciar();
                                        }
                                        Id = LToken[cont].Lexema;
                                        for (int i = 0; i < LVar.Count; i++)//Se recorre la lista de variables para verificar si ya existe
                                        {
                                            if (Id == LVar[i].id)
                                            {
                                                string script = string.Format("swal('La variable {0} ya se ha declarado', '', 'error');", Id);
                                                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                                Reiniciar();
                                            }
                                        }
                                        NextToken();
                                    } while (nextok == 132);
                                }
                            }
                            else
                            {
                                string script = string.Format("swal('Se esperaba un valor', '', 'error');");
                                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                Reiniciar();
                            }
                        } while (nextok == 112);

                    }
                    if (nextok == 132)
                    {
                        GuardarVar();
                        do
                        {
                            NextToken();
                            Id = LToken[cont].Lexema;//Se guarda el identificador
                            if (nextok != 103)
                            {
                                string script = string.Format("swal('Se esperaba un identificador', '', 'error');");
                                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                Reiniciar();
                            }
                            for (int i = 0; i < LVar.Count; i++)//Se recorre la lista de variables para verificar si ya existe
                            {
                                if (Id == LVar[i].id)
                                {
                                    string script = string.Format("swal('La variable {0} ya se ha declarado', '', 'error');", Id);
                                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                    Reiniciar();
                                }
                            }
                            NextToken();
                            if (nextok == 112)
                            {
                                NextToken();
                                if (nextok == 101 || nextok == 102 || nextok == 127 || nextok == FALSO || nextok == VERDADERO) //Si el siguiente token es alguno de estos
                                {
                                    Valor = LToken[cont].Lexema;//Se guarda el valor
                                    ValorVar(Tipo, nextok, Valor);//Se verifica si el valor es compatible al tipo de dato que se le asignó a la variable
                                }
                                else
                                {
                                    string script = string.Format("swal('Se esperaba un valor', '', 'error');");
                                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                    Reiniciar();
                                }
                            }
                            GuardarVar();
                        } while (nextok == 132);
                    }
                    if (nextok != 131)
                    {
                        string script = string.Format("swal('Se esperaba un ;', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        Reiniciar();
                    }
                    if (Id != null)
                    {
                        GuardarVar();
                    }
                    NextToken();
                } while (nextok == ENTERO || nextok == CADENA || nextok == REAL || nextok == BOLEANO);
            }
        }

        //Función Bloque
        public void Bloque()
        {
            if (nextok != INICIO)
            {
                string script = string.Format("swal('Se esperaba inicio del Bloque', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                Reiniciar();
            }
            NextToken();
            if (nextok == 103 || nextok == SI || nextok == DESDE || nextok == HACER || nextok == MIENTRAS || nextok == LEER || nextok == ESCRIBIR)
            {
                do
                {
                    Instrucciones(nextok);
                    NextToken();
                } while (nextok == 103 || nextok == SI || nextok == DESDE || nextok == HACER || nextok == MIENTRAS || nextok == LEER || nextok == ESCRIBIR);
            }
            if (nextok != FIN)
            {
                string script = string.Format("swal('Se esperaba FIN', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                Reiniciar();
            }
        }

        //Función de las Instrucciones
        public void Instrucciones(int Instruccion)
        {
            //Aun no se agregan las instrucciones
        }

        //Función Asignación
        public void Asignacion()
        {
            if (nextok != 103)
            {
                string script = string.Format("swal('Se esperaba un identificador', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                Reiniciar();
            }
            Id = LToken[cont].Lexema;
            for (int i = 0; i <= LVar.Count; i++)
            {
                if (i == LVar.Count)
                {
                    string script = string.Format("swal('El nombre {0} no existe', '', 'error');", Id);
                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                    Reiniciar();
                }
                if (Id == LVar[i].id)
                {
                    break;
                }
            }
            Tipo = LToken[cont].Descripcion;
            Asig = LToken[cont].Descripcion;
            AsignValor();
            NextToken();
            if (nextok == 112 || nextok == 113 || nextok == 114 || nextok == 115 || nextok == 116)
            {
                NextToken();
                if (nextok == LLAMAR)
                {
                    Llamar();
                }
                else
                {
                    Tipo = null;
                    Expresion();
                    if (Asig == RExp.ToString() || Asig == REAL.ToString() && RExp == ENTERO || Asig == ENTERO.ToString() && RExp == REAL)
                    {
                        if (nextok != 131)
                        {
                            string script = string.Format("swal('Se esperaba ;', '', 'error');");
                            ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                            Reiniciar();
                        }
                    }
                    else
                    {
                        string script = string.Format("swal('Los datos no son compatibles', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        Reiniciar();
                    }
                }
            }
            else
            {
                string script = string.Format("swal('Se esperaba un operador de asignación', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                Reiniciar();
            }
        }

        //Funcion 
        public void Expresion()
        {
            ///////////////////
        }

        //Función Parametros
        public void Parametros()
        {
            if (nextok == ENTERO || nextok == REAL || nextok == CADENA || nextok == BOLEANO)
            {
                do
                {
                    Tipo = LToken[cont].Lexema;
                    NextToken();
                    if (nextok != 103)
                    {
                        string script = string.Format("swal('Se esperaba un identificador', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        Reiniciar();
                    }
                    Id = LToken[cont].Lexema;
                    for (int i = 0; i < ListParam.Count; i++)
                    {
                        if (Id == ListParam[i].id && Pertenece == ListParam[i].pertenece)
                        {
                            string script = string.Format("swal('El parametro {0} está duplicado', '', 'error');", Id);
                            ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                            Reiniciar();
                        }
                    }
                    NextToken();
                    if (nextok == 132)
                    {
                        NextToken();
                    }
                    GuardarParametro();
                } while (nextok == ENTERO || nextok == REAL || nextok == CADENA || nextok == BOLEANO);
            }
        }

        //Función Bloque Funcion
        public void BloqueFuncion()
        {
            if (nextok != INICIO)
            {
                string script = string.Format("swal('Se esperaba un ;', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                Reiniciar();
            }
            NextToken();
            if (nextok == 103 || nextok == SI || nextok == DESDE || nextok == HACER || nextok == MIENTRAS || nextok == LEER || nextok == ESCRIBIR)
            {
                do
                {
                    Instrucciones(nextok);
                    NextToken();
                } while (nextok == 103 || nextok == SI || nextok == DESDE || nextok == HACER || nextok == MIENTRAS || nextok == LEER || nextok == ESCRIBIR);
            }
            if (nextok != REGRESA)
            {
                string script = string.Format("swal('Se esperaba Regresa', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                Reiniciar();
            }
            NextToken();
            Expresion();
            if (RExp == TipoFuncion)
            {
                if (nextok != 131)
                {

                    string script = string.Format("swal('Se esperaba un ;', '', 'error');");
                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                    Reiniciar();
                }
                NextToken();
                if (nextok != FIN)
                {
                    string script = string.Format("swal('Se esperaba FIN', '', 'error');");
                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                    Reiniciar();
                }
                NextToken();
            }
            else
            {
                string script = string.Format("swal('EL dato regresado es incorrecto', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                Reiniciar();
            }
        }

       
        public void Funcion()
        {
            if (nextok == FUNCION)
            {
                do
                {
                    Pertenece = "Global";
                    NextToken();
                    if (nextok == ENTERO || nextok == CADENA || nextok == REAL || nextok == BOLEANO)
                    {
                        Tipo = LToken[cont].Lexema;
                        TipoFuncion = LToken[cont].NumTok;
                        NextToken();
                        Id = LToken[cont].Lexema;
                        for (int i = 0; i < LVar.Count; i++)
                        {
                            if (Id == LVar[i].id)
                            {
                                string script = string.Format("swal('Ya se contiene una definicion para {0}', '', 'error');", Id);
                                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                Reiniciar();
                            }
                        }
                        GuardarVar();
                        Pertenece = LToken[cont].Lexema;
                        GFuncion = Pertenece;
                        ListFuncion.Add(GFuncion);
                        if (nextok != 103)
                        {
                            string script = string.Format("swal('Se esperaba un idetificador', '', 'error');");
                            ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                            Reiniciar();
                        }
                        NextToken();
                        if (nextok != 129)
                        {
                            string script = string.Format("swal('Se esperaba un (', '', 'error');");
                            ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                            Reiniciar();
                        }
                        NextToken();
                        Parametros();
                        if (nextok != 130)
                        {
                            string script = string.Format("swal('Se esperaba un )', '', 'error');");
                            ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                            Reiniciar();
                        }
                        NextToken();
                        BloqueFuncion();
                    }
                    else
                    {
                        string script = string.Format("swal('Se esperaba un tipo de dato', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        Reiniciar();
                    }
                }
                while (nextok == FUNCION);
            }
        }

        //Función para llamar una función
        public void Llamar()
        {
            ContPar = 0;
            ContParAc = 0;
            ContParFunc = 0;
            NextToken();
            if (nextok != 103)
            {
                string script = string.Format("swal('Se esperaba un identificador', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                Reiniciar();
            }
            if (LToken[cont].Descripcion.Equals(Tipo, StringComparison.OrdinalIgnoreCase))
            {
                FunLlama = LToken[cont].Lexema;
                for (int l = 0; l < ListParam.Count; l++)
                {
                    if (ListParam[l].pertenece == FunLlama)
                    {
                        ContPar++;
                    }
                }
                for (int m = 0; m <= ListFuncion.Count; m++)
                {
                    if (m == ListFuncion.Count)
                    {
                        string script = string.Format("swal('La función {0} no existe', '', 'error');", FunLlama);
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        Reiniciar();
                    }
                    if (ListFuncion[m] == FunLlama)
                    {
                        NextToken();
                        if (nextok != 129)
                        {
                            string script = string.Format("swal('Se esperaba un (', '', 'error');");
                            ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                            Reiniciar();
                        }
                        NextToken();
                        if (nextok == 130)
                        {
                            for (int h = 0; h <= ListParam.Count; h++)
                            {
                                if (ContParFunc == 0 && ContParAc == 0 && h == ListParam.Count)
                                {
                                    NextToken();
                                    if (nextok != 131)
                                    {
                                        string script = string.Format("swal('Se esperaba un ;', '', 'error');");
                                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                        Reiniciar();
                                    }
                                    break;
                                }
                                if (ListParam[h].pertenece == FunLlama)
                                {
                                    string script = string.Format("swal('Faltan parámetros en la función {0}', '', 'error');", FunLlama);
                                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                    Reiniciar();
                                }

                            }
                            break;
                        }
                        else
                        {
                            for (int h = 0; h <= ListParam.Count; h++)
                            {
                                if (h == ListParam.Count)
                                {
                                    string script = string.Format("swal('La función {0} no debe contener parámetros', '', 'error');", FunLlama);
                                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                    Reiniciar();
                                }
                                if (ListParam[h].pertenece == FunLlama)
                                {
                                    LlamarParametros();
                                    if (nextok != 130)
                                    {
                                        string script = string.Format("swal('Se esperaba un )', '', 'error');");
                                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                        Reiniciar();
                                    }
                                    if (ContParAc == ContParFunc)
                                    {
                                        NextToken();
                                        if (nextok != 131)
                                        {
                                            string script = string.Format("swal('Se esperaba un ;', '', 'error');");
                                            ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                            Reiniciar();
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        string script = string.Format("swal('Se esperaban {0} parámetros en la función {1}', '', 'error');",ContPar, FunLlama);
                                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                        Reiniciar();
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                string script = string.Format("swal('Los tipos en la asignación son incorrectos', '', 'error');");
                ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                Reiniciar();
            }
        }

        //Funcion para llamar parametros de una funcion
        public void LlamarParametros()
        {
            Expresion();
            if (Id != null)
            {
                for (int i = 0; i <= LVar.Count; i++)
                {
                    if (i == LVar.Count)
                    {
                        string script = string.Format("swal('El nombre {0} no existe', '', 'error');", Id);
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        Reiniciar();
                    }
                    if (Id == LVar[i].id)
                    {
                        break;
                    }
                }
            }
            for (int l = 0; l <= ListParam.Count; l++)
            {
                if (l == ListParam.Count)
                {
                    if (nextok == 132)
                    {
                        string script = string.Format("swal('Demasiados parámetros en la función {0}', '', 'error');", FunLlama);
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        Reiniciar();
                    }
                    else
                    {
                        break;
                    }
                }
                if (ListParam[l].pertenece == FunLlama)
                {
                    ContParFunc++;
                    if (nextok == 130 && ContParAc > 1)
                    {
                        break;
                    }
                    if (ContParFunc > 1)
                    {
                        NextToken();
                        Expresion();
                        if (Id != null)
                        {
                            for (int i = 0; i <= LVar.Count; i++)
                            {
                                if (i == LVar.Count)
                                {
                                    string script = string.Format("swal('El nombre {0} no existe', '', 'error');", Id);
                                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                                    Reiniciar();
                                }
                                if (Id == LVar[i].id)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    Asig = ListParam[l].tipo;
                    AsignValor();
                    if (RExp.ToString() == Asig)
                    {
                        ContParAc++;
                    }
                    else if (RExp == LOGICA)
                    {
                        string script = string.Format("swal('La expresión no debe ser lógica', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        Reiniciar();
                    }
                    else
                    {
                        string script = string.Format("swal('Algún parámetro no es compatible', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        Reiniciar();
                    }
                }
            }

        }


        //Función para guardar variables de los tokens
        private bool Guardar()
        {
            LVar.Add(new Variables()
            {
                tipo = Tipo,
                id = Id
            });
            if (Id2 == Id)
            {
                ListErrores(-11);
                Id2 = Id;
                Tipo = null;
                Id = null;
                return true;
            }
            else
            {
                Id2 = Id;
                Tipo = null;
                Id = null;
                return false;
            }

        }


        //Función para guardar variables
        private void GuardarVar()
        {
            LVar.Add(new Variables()
            {
                tipo = Tipo,
                id = Id,
                valor = Valor,
                pertenece = Pertenece
            });
            Id = null;
            Valor = null;
        }

        //Función para guardar parámetros
        private void GuardarParametro()
        {
            ListParam.Add(new Variables()
            {
                tipo = Tipo,
                id = Id,
                valor = Valor,
                pertenece = Pertenece
            });
            Tipo = null;
            Id = null;
            Valor = null;
        }

        //Función para asignar valor a la variable
        public void ValorVar(string Tipo, int valor, string Va)
        {
            if (Tipo.Equals("Entero", StringComparison.OrdinalIgnoreCase))
            {
                if (valor != 101)
                {
                    string script = string.Format("swal('No se puede asignar el valor {0} porque no es compatible', '', 'error');", Va.ToString());
                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                    Reiniciar();
                }
                NextToken();
            }
            if (Tipo.Equals("Real", StringComparison.OrdinalIgnoreCase))
            {
                if (valor != 102)
                {
                    string script = string.Format("swal('No se puede asignar el valor {0} porque no es compatible', '', 'error');", Va.ToString());
                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                    Reiniciar();
                }
                NextToken();
            }
            if (Tipo.Equals("Cadena", StringComparison.OrdinalIgnoreCase))
            {
                if (valor != 127)
                {
                    string script = string.Format("swal('No se puede asignar el valor {0} porque no es compatible', '', 'error');", Va.ToString());
                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                    Reiniciar();
                }
                NextToken();
            }
            if (Tipo.Equals("Boleano", StringComparison.OrdinalIgnoreCase))
            {
                if (valor == FALSO || valor == VERDADERO)
                {
                    NextToken();
                }
                else
                {
                    string script = string.Format("swal('No se puede asignar el valor {0} porque no es compatible', '', 'error');", Va.ToString());
                    ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                    Reiniciar();
                }
            }
        }

        //Función para asignarle valor al ID de la función de asignación
        public void AsignValor()
        {
            if (Asig != null)
            {
                if (Asig.Equals("Entero", StringComparison.OrdinalIgnoreCase))
                {
                    Asig = ENTERO.ToString();
                }
                if (Asig.Equals("Real", StringComparison.OrdinalIgnoreCase))
                {
                    Asig = REAL.ToString();
                }
                if (Asig.Equals("Cadena", StringComparison.OrdinalIgnoreCase))
                {
                    Asig = CADENA.ToString();
                }
                if (Asig.Equals("Boleano", StringComparison.OrdinalIgnoreCase))
                {
                    Asig = BOLEANO.ToString();
                }
            }
        }

        //Lista de errores
        public void ListErrores(int Error)
        {

            switch (Error)
            {
                case -1:
                    {
                        string script = string.Format("swal('ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
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
                        dt.Rows.Add(LineNum, "", Error, "Error: Se esperaba una O");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
                case -6:
                    {
                        Acum = "";
                        string script = string.Format("swal('ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
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
                        dt.Rows.Add(LineNum, "", Error, "Error: Se esperaba un asignador :=");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
                case -10:
                    {
                        Acum = "";
                        string script = string.Format("swal('ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        dt.Rows.Add(LineNum, "", Error, "Error: Caracter inválido");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
                case -11:
                    {
                        Acum = "";
                        string script = string.Format("swal('ERROR', '', 'error');");
                        ClientScript.RegisterStartupScript(this.GetType(), "swal", script, true);
                        dt.Rows.Add(LineNum, "", Error, "Error: Ya se declaró la variable");
                        tabla.DataSource = dt;
                        tabla.DataBind();
                        break;
                    }
            }
        }

    }
}