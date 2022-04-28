using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace enviosShell
{
    public class Api
    {
        Recursos r = new Recursos();
        public GetToken Token(string URL, string client_id, string client_secret)
        {
            
            r.WriteFullLine("OBTENIENDO TOKEN");
            r.WriteFullLine("");
            string url = URL;
            try
            {
                var client = new WebClient { BaseAddress = url };

                client.Headers["Accept"] = "*/*";
                client.Headers["ContentLength"] = "0";

                client.Headers.Add("client_id", client_id);
                client.Headers.Add("client_secret", client_secret);

                string response = client.UploadString(url, "POST", "");
                var datos = JsonConvert.DeserializeObject<GetToken>(response);
                
                r.WriteFullLine("_GET TOKEN SUCCESS");

                
                r.GraboTxt(Program.ruta, "token.txt", datos.access_token);

                return datos;
            }
            catch (Exception x)
            {
                string errorClientID = "\r\nNo se ingresó el client_id en el header.";
                string errorClientSecret = "\r\nNo se ingresó el client_secret en el header.";
                string errorCredentials = "\r\nInvalid credentials – Código 401: Las credenciales ingresadas son inválidas y\r\nno corresponden a ningún par client_id / client_secret almacenado";
                var datos = new GetToken();

                if (x.Message.Contains("401"))
                {
                    datos.access_token = x.Message + errorCredentials;
                }
                else if (x.Message.Contains("422"))
                {
                    datos.access_token = (client_id == "") ? x.Message + errorClientID : x.Message + errorClientSecret;
                    datos.access_token = (client_secret == "") ? x.Message + errorClientSecret : x.Message + errorClientID;
                }
               
                return datos;
            }

        }

        public Adhesion adhesion(string urlAdhesion, string idEstacion, string terminos, string token)
        {
            try
            {
                var client = new WebClient { BaseAddress = urlAdhesion };

                client.Headers["Accept"] = "*/*";
                client.Headers["ContentLength"] = "0";

                client.Headers.Add("idEstacion", idEstacion);
                client.Headers.Add("terminos", terminos);
                client.Headers.Add("token", token);

                string response = client.UploadString(urlAdhesion, "POST", "");
                var datos = JsonConvert.DeserializeObject<Adhesion>(response);

                r.GraboTxt(Program.ruta, "key.txt", datos.result);

                return datos;
            }
            catch (Exception x)
            {
                var datos = new Adhesion
                {
                    result = x.Message
                };
                return datos;
            }
        }

        public Boolean PublicoDatos(string urlExtraccion, string token, string key, string JSON)
        {
            string response="sin datos";
            try
            {
                var baseAddress = urlExtraccion;

                var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                http.Accept = "*/*";
                http.ContentType = "application/json";
                http.Method = "POST";
                http.Headers.Add("container", "blob-raizenarbi-01");
                http.Headers.Add("token", token);
                http.Headers.Add("key", key);

                string parsedContent = JSON;
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(parsedContent);

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                var resp = http.GetResponse();

                var stream = resp.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();
                var rta = JsonConvert.SerializeObject(content);
               // Console.WriteLine(rta);

                return rta.Contains("Success");
            }
            catch (Exception e)
            {
                
                //if(e.Message.Contains(nroError))
                Console.WriteLine(e.Message + " - " + response );
                return false;
            }
           
        }
    }
}
