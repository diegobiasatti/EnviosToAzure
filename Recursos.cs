using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace enviosShell
{
    public class Recursos
    {
       
        public void CargoTitulo()
        {
            string titulo = "ENVIO DE DATOS SHELL API";
            string linea = "************************";
            Console.SetCursorPosition((Console.WindowWidth - titulo.Length) / 2, 2);
            Console.WriteLine(titulo);
            Console.SetCursorPosition((Console.WindowWidth - linea.Length) / 2, 3);
            Console.WriteLine(linea);
        }
        public  void WriteFullLine(string value)
        {
            // Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(value.PadRight(Console.WindowWidth - 1));
            Console.ResetColor();
        }
        public void RenglonRelleno(string value)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(value.PadRight(Console.WindowWidth - 1));
            Console.ResetColor();
        }

        public void WriteFullLineNg(string value)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(value.PadRight(Console.WindowWidth - 1));
            Console.ResetColor();
        }
        public void WriteDetalle(string value)
        {
            // Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(value.PadRight(Console.WindowWidth - 1));
            Console.ResetColor();
        }

        public void GraboTxt(string path, string archivo, string txt)
        {
            using (var tw = new StreamWriter((path + @"\"+archivo)))
            {
                tw.WriteLine(txt);
                tw.Close();
            }
        }
        public void GraboTxtAppend(string path, string archivo, string txt)
        {
            using (var tw = new StreamWriter((path + @"\" + archivo), true))
            {
                tw.WriteLine(txt);
                tw.Close();
            }
        }
        public  void Config(string ruta)
        {
            string[] lineas = File.ReadAllLines(ruta + @"\config.cfg");

            Program.urlGetToken = lineas[0].Substring(12);
            Program.urlAdhesion = lineas[1].Substring(12);
            Program.urlExtraccion = lineas[2].Substring(14);
            Program.client_id = lineas[3].Substring(10);
            Program.client_secret = lineas[4].Substring(14);
            Program.database = lineas[5].Substring(5);
            Program.IdEstacion = lineas[6].Substring(11);
            Program.monsterMdb = lineas[7].Substring(12);
            
            Program._key = key(ruta);
         
        }
        public string key(string ruta)
        {
            if (File.Exists(ruta + @"\key.txt"))
            {
                string[] key = File.ReadAllLines(ruta + @"\key.txt");
                return key[0];
            }
            else
            {
                return "Cierre el Programa y ejecutelo en Modo Adhesion, luego ejecute normalmente.";
            }
        }

        public string LastIdLeido()
        {
            sql consulta = new sql();
            
            try
            {   
                if(File.Exists(Program.ruta + @"\last.id"))
                {
                    StreamReader sr = new StreamReader(Program.ruta + @"\last.id");
                    String line = sr.ReadLine();
                    sr.Close();
                    if(line == "")
                    {
                        string PrimerRegistroDelDia = consulta.SelectTop1DelDia();
                        Thread.Sleep(5000);
                        Console.WriteLine("ID Encontrado: " + PrimerRegistroDelDia);
                        using (var tw = new StreamWriter(Program.ruta + @"\last.id"))
                        {
                            tw.WriteLine(PrimerRegistroDelDia);
                            tw.Close();
                        }

                        return PrimerRegistroDelDia;
                    }
                    else return line;
                }
                else
                {
                    string PrimerRegistroDelDia = consulta.SelectTop1DelDia();
                    Thread.Sleep(5000);
                    Console.WriteLine("ID Encontrado: " + PrimerRegistroDelDia);
                    using (var tw = new StreamWriter(Program.ruta + @"\last.id"))
                    {
                        tw.WriteLine(PrimerRegistroDelDia);
                        tw.Close();
                    }
                    
                    return PrimerRegistroDelDia;
                }

               
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public void ejecutoPrograma(string archivo)
        {
            Process.Start(Program.ruta + @"\" + archivo);
        }
        public void AvisoAlMonster(  int fecha_hora_or_descripcion_error)
        {
            string upd = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
            string app = "enviosShell";
            string _connectionString = Program.monsterMdb;
            try
            {
                using (OleDbConnection connection = new OleDbConnection(string.Format("Provider = Microsoft.ACE.OLEDB.12.0; Data Source ={0}", _connectionString)))
                {
                    if (fecha_hora_or_descripcion_error == 1) //upd FechaHora
                    {
                        using (OleDbCommand updateCommand = new OleDbCommand("UPDATE EstadosApps SET [FechaHora] = ? WHERE [Aplicacion] = ?", connection))
                        {
                            connection.Open();

                            updateCommand.Parameters.AddWithValue("@FechaHora", upd);
                            updateCommand.Parameters.AddWithValue("@Aplicacion", app);

                            updateCommand.ExecuteNonQuery();
                            Console.WriteLine("Aviso al Monster Correcto.");
                        }
                    }
                    else if (fecha_hora_or_descripcion_error == 0) //upd Descripcion Error
                    {
                        using (OleDbCommand updateCommand = new OleDbCommand("UPDATE EstadosApps SET [DescripcionError] = ? WHERE [Aplicacion] = ?", connection))
                        {
                            connection.Open();

                            updateCommand.Parameters.AddWithValue("@DescripcionError", upd);
                            updateCommand.Parameters.AddWithValue("@Aplicacion", app);

                            updateCommand.ExecuteNonQuery();
                            connection.Close();
                            Console.WriteLine("Aviso al Monster Error.");
                        }
                    }
                }
            }
            catch (Exception o)
            {
                Log(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Main exception: " + o);
            }
        }
        private static void Log(string error)
        {
            string path = (Program.ruta + @"\log.txt");

            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine(error);
            sw.Close();


        }
    }
}
