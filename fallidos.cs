using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace enviosShell
{
    
    class fallidos
    {
        Api api = new Api();
        public void chequearFallidos()
        {

            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\fallidos\";
            string[] listFilesCurrDir = Directory.GetFiles(path);

            Console.WriteLine(""); 
            Console.WriteLine("**************************************************");
            Console.WriteLine("**************************************************");
            Console.WriteLine("");
            Console.WriteLine("Chequeando Fallidos");
            Console.WriteLine(""); 
            Console.WriteLine("Cantidad de Fallidos: " + listFilesCurrDir.Length);
            
            foreach(string file in listFilesCurrDir)
            {
                string txt = File.ReadAllText(file);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(txt);
                // Envio el Json a la Api de Shell
                bool envio = api.PublicoDatos(Program.urlExtraccion, Program.tok.access_token, Program._key, txt);
                if (envio)
                {
                    Console.WriteLine("Eliminando Fallido");
                    File.Delete(file);

                }
            }
            Console.WriteLine("");
            Console.WriteLine("Fin de Proceso FALLIDOS");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("**************************************************");
            Console.WriteLine("**************************************************");
            Console.WriteLine("");

        }
        
    }
}
