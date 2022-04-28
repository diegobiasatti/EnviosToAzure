using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Text;
using System.Threading;

namespace enviosShell
{

    class Program
    {
        public static string urlGetToken;
        public static string urlAdhesion;
        public static string urlExtraccion;
        public static string client_id;
        public static string client_secret;
        public static string database="";
        public static string IdEstacion;
        public static string _key;
        public static string ultimoIdLeido;
        public static string ultimoIdDeLaLecturaActual;
        public static string monsterMdb="";

        public static string ruta = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static Api api = new Api();
        static Recursos r = new Recursos();
        public static GetToken tok = new GetToken();
        static TimeSpan span1;
        public static class SslProtocolsExtensions
        {
            public const SslProtocols Tls12 = (SslProtocols)0x00000C00;
            public const SslProtocols Tls11 = (SslProtocols)0x00000300;
        }
        public static class SecurityProtocolTypeExtensions
        {

            public const SecurityProtocolType Tls12 = (SecurityProtocolType)SslProtocolsExtensions.Tls12;
            public const SecurityProtocolType Tls11 = (SecurityProtocolType)SslProtocolsExtensions.Tls11;
            public const SecurityProtocolType SystemDefault = (SecurityProtocolType)0;
        }
        
        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]//para que arranque minimizado
        [return: MarshalAs(UnmanagedType.Bool)]//para que arranque minimizado
        private static extern bool ShowWindow([In] IntPtr hWnd, [In] int nCmdShow);//para que arranque minimizado
        static void Main(string[] args)
        {

           
            IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;//para que arranque minimizado
            ShowWindow(handle, 6); //para que arranque minimizado
            
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolTypeExtensions.Tls12;
                //System.Net.ServicePointManager.SecurityProtocol |=    (SecurityProtocolType)3072 | (SecurityProtocolType)768;
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)768; //TLS 1.1
                //System.Net.ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3; no lo probe pero deci q era para disable ssl3
            }
            catch(Exception g)
            {
                Console.WriteLine("Error SecurityProtocol: " +g.Message);
                Console.WriteLine("");
                Console.WriteLine("Cierre el Programa y solucione el error de seguridad en Windows.");
                Console.ReadKey();
            }
            
            r.Config(ruta);
            //Console.WriteLine(_key);
            r.CargoTitulo();
            r.AvisoAlMonster(1);

           
            ultimoIdLeido = r.LastIdLeido();
            // Genera Token
            tok = api.Token(urlGetToken,client_id, client_secret);

            // Aca entra si se ejecuto el archivo .bat (Adhesion.bat)
            EsAdhesion(args, urlAdhesion, tok.access_token);


            if (File.Exists(ruta + @"\tyc_accept"))
            {
                // Traigo los datos del Access
                Console.WriteLine("Ultimo Id Leido: " + ultimoIdLeido);

                // 13/01/2021 se agrega chequeo de fallidos, cada vez que falla un envio queda guardado en esa carpeta.
                // con esto se lo envia a shell y se elimina de esa carpeta. 
                fallidos F = new fallidos();
                F.chequearFallidos();
                
                sql consulta = new sql();
                Extraccion envioApi = consulta.SelectTicket(ultimoIdLeido);

            }
            else
            {
                Console.WriteLine("Debe Realizar la Adhesion al Servicio.");
            }

            //r.GraboTxt(Program.ruta, "extraccion.txt", json);

            

            r.WriteFullLine("Proceso Finalizado");
            string reporte = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo)+" - "+ ultimoIdLeido + " al " + Program.ultimoIdDeLaLecturaActual;// + Environment.NewLine;
            r.GraboTxtAppend(Program.ruta + @"\Reportes", "report.txt", reporte);
            Thread.Sleep(1000);
            //Console.ReadKey();
            
        }
    

        static void EsAdhesion(string[] args, string url, string _token)
        {
            if ((args.Length > 0) && (args[0] == "Adhesion"))
            {
                if ((args[1] != "") )
                {
                    string nroBoca = args[1];
                    Console.WriteLine("");
                    Console.WriteLine("Presione el Nro deseado, segun lo que necesite.");
                    Console.WriteLine("");
                    Console.WriteLine("1- Para Iniciar Adhesion");
                    Console.WriteLine("2- Para Declinar Adhesion");
                    Console.WriteLine("3- Conocer Ultima Fecha de Adhesion");
                    Console.WriteLine("");
                    Console.Write("Ejecutar Opcion : ");
                    
                    var op = Console.Read();
                    int nro = Convert.ToInt32(op);
                    Console.WriteLine("");

                    Adhesion adhesion;
                        switch (nro)
                        {
                            //representa case 49 = 1 // 50 = 2 // 51 = 3
                            case 49:
                            if (!File.Exists(ruta + @"\tyc_accept"))
                            {
                                Console.Read();
                                Console.Read();
                                Console.WriteLine("Para poder realizar la Adhesion debe Aceptar los Terminos y Condiciones.");
                                Console.WriteLine("");
                                Console.WriteLine("Presione A para poder Aceptar. O bien presione R para rechazar la Adhesion.");
                                Console.WriteLine("");
                                Console.WriteLine("Presione T para ver los Terminos y Condiciones.");
                                Console.WriteLine("");
                                Console.Write("Presione ( A ) o ( R ) o ( T ): ");
                                var opcion = Console.Read();
                                int nroOp = Convert.ToInt32(opcion);
                                if ((nroOp == 116) || (nroOp == 84))
                                {
                                    r.ejecutoPrograma("tyc.pdf");
                                    Console.Clear();
                                    r.WriteFullLine("Abriendo Terminos y Condiciones");
                                    Console.WriteLine();
                                    Console.WriteLine();
                                    Console.Read();
                                    Console.Read();
                                    Console.WriteLine("Presione A para poder Aceptar. O bien presione R para rechazar la Adhesion.");
                                    Console.WriteLine("");

                                    Console.Write("Presione ( A ) o ( R ) : ");
                                    var opcion_ = Console.Read();
                                    int nroOp_ = Convert.ToInt32(opcion_);

                                    if ((nroOp_ == 65) || (nroOp_ == 97))
                                    {
                                        r.WriteDetalle(" Generando Adhesion...");
                                        r.WriteDetalle("");
                                        adhesion = api.adhesion(url, nroBoca, "accept", _token);

                                        r.GraboTxt(ruta, "key.txt", adhesion.result);
                                        Console.WriteLine(adhesion.result);
                                        if (File.Exists(ruta + @"\tyc_decline"))
                                        {
                                            File.Move(ruta + @"\tyc_decline", ruta + @"\tyc_decline_se_volvio_a_adherir_el_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss"));
                                        }
                                        r.GraboTxt(ruta, "tyc_accept", "");
                                        r.WriteDetalle(" Adhesion Realizada Correctamente.");
                                        r.WriteDetalle("");
                                        r.WriteDetalle(" Fin.");
                                    }
                                    else
                                    {
                                        Environment.Exit(0);
                                    }
                                }
                                else
                                {
                                    if ((nroOp == 65) || (nroOp == 97))
                                    {
                                        r.WriteDetalle(" Generando Adhesion...");
                                        r.WriteDetalle("");
                                        adhesion = api.adhesion(url, nroBoca, "accept", _token);
                                        r.GraboTxt(ruta, "key.txt", adhesion.result);
                                        if (File.Exists(ruta + @"\tyc_decline"))
                                        {
                                            File.Move(ruta + @"\tyc_decline", ruta + @"\tyc_decline_se_volvio_a_adherir_el_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss"));
                                        }
                                        r.GraboTxt(ruta, "tyc_accept", "");
                                        r.WriteDetalle(" Adhesion Realizada Correctamente.");
                                        r.WriteDetalle("");
                                        r.WriteDetalle(" Fin.");
                                    }
                                    else
                                    {
                                        Environment.Exit(0);
                                    }

                                }

                                //Console.ReadKey();

                            }
                            else
                            {
                                Console.WriteLine("**************************************");
                                Console.WriteLine("");
                                Console.WriteLine("La Adhesion ya se encuentra Realizada.");
                                Console.WriteLine("");
                                Console.WriteLine("**************************************");
                                
                            }

                            break;
                            case 50:
                            try
                            {
                                if (File.Exists(ruta + @"\tyc_accept"))
                                {
                                    File.Move(ruta + @"\tyc_accept", ruta + @"\tyc_accept_se_declino_el_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss"));
                                }
                                r.GraboTxt(ruta, "tyc_decline", "");

                            }
                            catch (Exception o)
                            {
                                Console.WriteLine("Error tratando de Renombrar archivo: " + o.Message);
                            }
                            finally
                            {
                                r.WriteDetalle(" Generando Baja...");
                                r.WriteDetalle("");
                                adhesion = api.adhesion(url, nroBoca, "decline", _token);

                                if (adhesion.result == "Successfully declined")
                                {
                                    r.WriteDetalle(" Adhesion Declinada Correctamente.");
                                    r.WriteDetalle("");
                                    r.WriteDetalle(" Fin.");

                                }
                                else
                                {
                                    Console.WriteLine("Fallo la operacion. " + adhesion.result);
                                }
                            }
                           
                           
                            break;
                            case 51:
                                r.WriteDetalle(" Buscando Datos...");
                                r.WriteDetalle("");
                                adhesion = api.adhesion(url, nroBoca, "lastDate", _token);
                                r.WriteDetalle(" Ultima Fecha de Adhesion " + adhesion.result);
                                r.WriteDetalle("");
                                r.WriteDetalle(" Fin.");
                                break;
                            default:
                                r.WriteDetalle("Valor Incorrecto");
                            
                                break;
                        }
                    
                }
                else
                {
                    r.WriteFullLineNg("No debe estar vacio, Nro de Boca Incorrecto");
                }
                
               
            }
        }
        
       
    }
}
