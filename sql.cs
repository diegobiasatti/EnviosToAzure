using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace enviosShell
{
    class sql
    {
        Extraccion extraccion = new Extraccion();
        Extraccion ReportoVenta;
        Api api = new Api();
        Recursos r = new Recursos();
        string _connectionString = Program.database;
        string _clave = "claveBaseDeDatos";

        public string SelectTop1DelDia()
        {
            string fechaHoy = DateTime.Now.ToString("MM/dd/yyyy"); // puse primero el mes.. asi en el access me traia bien los datos.
            string query = "SELECT top 1 ID FROM Tickets WHERE FechaHora >=#" + fechaHoy + "# order by ID ASC";
            Console.WriteLine(query);
            try
            {
                r.WriteDetalle("selectTop1 " + _connectionString);
                using (OleDbConnection connection = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + _connectionString + "; Jet OLEDB:Database Password=" + _clave)))
                {

                    using (OleDbCommand selectCommand = new OleDbCommand(query, connection))
                    {
                        string valor="";
                        connection.Open();
                        OleDbDataReader reader = selectCommand.ExecuteReader();
                        while (reader.Read())
                        {
                           valor = (reader.GetValue(0)).ToString();
                        }
                        reader.Close();
                        Console.WriteLine("ID Nro. " + valor);
                        return valor;
                    }
                }
            }
            catch(Exception x)
            {
                return x.Message;
            }
        }
        public Extraccion SelectTicket(string ultimoId)
        {
            string tipoMovimiento = "";
            string consulta = "SELECT " +
                "T.ID AS IdT, TP.ID AS IdTp,T.IdCliente, T.Numero, TP.IdProducto,Productos.Producto, TP.Precio, Productos.Costo, TP.Cantidad, T.IdCaja, " +
                "T.Turno, T.FechaHora, T.TipoFactura, T.Impresora, T.Importe,TMDP.Tipo " +
                "FROM((Tickets T " +
                "INNER JOIN TicketsProductos TP ON T.ID = TP.IdTicket) " +
                "LEFT JOIN TiposMediosDePago TMDP ON T.IdMedioDePago = TMDP.ID) " +
                "INNER JOIN Productos ON Productos.ID = TP.IdProducto " +
                "WHERE T.ID > "+ultimoId;
           
            try
            {
                using (OleDbConnection connection = new OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" +_connectionString + "; Jet OLEDB:Database Password="+_clave)))
                {

                    using (OleDbCommand selectCommand = new OleDbCommand(consulta, connection))
                    {
                        connection.Open();
                      
                        selectCommand.ExecuteNonQuery();
                        DataTable table = new DataTable();
                        OleDbDataAdapter adapter = new OleDbDataAdapter();
                        adapter.SelectCommand = selectCommand;
                        adapter.Fill(table);

                        var i = 0;
                        Rows datosVenta = new Rows();
                        
                        int nroTicketUltimo = 0;
                        int cantidadRegistros = table.Rows.Count;
                        if (cantidadRegistros == 0)
                        {
                            Console.WriteLine("No se encontraron Registros Nuevos.");
                        }
                        foreach (DataRow row in table.Rows)
                        {
                            string dateString = row["FechaHora"].ToString();
                            string FecheHora = String.Format("{0:s}", Convert.ToDateTime(dateString)) + "Z";
                            //Console.WriteLine(String.Format("{0:s}", Convert.ToDateTime(dateString)) + "Z");

                            switch (row["TipoFactura"])
                            {
                                case "A":
                                    tipoMovimiento = "FAA";
                                    break;
                                case "B":
                                    tipoMovimiento = (Convert.ToInt32(row["IdCliente"]) == 0) ? "TIK" : "FAB";
                                    break;
                                case "X":
                                    tipoMovimiento = "REM";
                                    break;
                            
                            }
                            if (nroTicketUltimo == 0)
                            {
                                nroTicketUltimo = Convert.ToInt32(row["IdT"]);
                                ReportoVenta = new Extraccion
                                {
                                    idEstacion = Program.IdEstacion,  //viene del config.cfg
                                    rows = new List<Rows>
                                    {
                                        datosVenta
                                    }
                                };
                                datosVenta.Identificador = row["IdT"] + "-" + Program.IdEstacion + "-" + row["IdTp"] + "-" + row["Numero"];
                               // datosVenta.IdCaja = "";// row["IdCaja"].ToString();
                               // datosVenta.IdUsuario = "1";
                                datosVenta.Turno = row["Turno"].ToString();
                               // datosVenta.Vendedor = "1";
                               // datosVenta.PlantillaVenta = "1";
                                datosVenta.TipoDeMovimiento = tipoMovimiento;
                                datosVenta.PuntoDeVentaAFIP = Convert.ToInt32(row["Numero"].ToString().Substring(0,4)); 
                                datosVenta.NumeroComprobante = Convert.ToInt32(row["Numero"].ToString().Substring(5));
                                datosVenta.FechaEmision =FecheHora; 
                              /*  datosVenta.Cliente = new Cliente
                                {
                                    id_cliente = "1",
                                    TarjetaLatamPass = "1",
                                    RazonSocial = "1",
                                    CategoriaIVA = "1",
                                    CUIT = "1",
                                    Domicilio = "1",
                                    Localidad = "1",
                                    CodigoPostal = "1",
                                    Provincia = "1",
                                    Pais = "1"
                                };*/
                                datosVenta.CondicionVenta = (row["Tipo"].ToString() != "") ?row["Tipo"].ToString() :"Efectivo";
                                datosVenta.Impresora = row["Impresora"].ToString();
                               // datosVenta.Impuestos_Internos = "Obligatorio";
                               // datosVenta.Percepciones_IIBB = "Obligatorio";
                               // datosVenta.Observaciones = "Obs";
                                datosVenta.Total = Convert.ToDecimal(row["Importe"]);
                                datosVenta.EstacionOrigen = Convert.ToInt32(Program.IdEstacion);
                                datosVenta.Detalle.Add(new Detalle
                                {
                                    Identificador = row["IdProducto"].ToString() + "-"+ row["IdT"] + "-" + Program.IdEstacion + "-" + row["IdTp"] + "-" + row["Numero"],
                                    IdentificadorMovimiento = row["IdT"] + "-" + Program.IdEstacion + "-" + row["IdTp"] + "-" + row["Numero"],
                                    Articulo_id = row["IdProducto"].ToString(),
                                    Cantidad = Convert.ToDecimal(row["Cantidad"]),
                                    //promocionOpcion = "",
                                    PrecioUnitario = Convert.ToDecimal(row["Precio"]),
                                    CostoUnitario = Convert.ToDecimal(row["Costo"]),
                                    //NumeroManguera = 0,
                                    //IdDespacho = "",
                                    
                                });
                            }
                            else
                            {
                                if(nroTicketUltimo == Convert.ToInt32(row["IdT"]))
                                {
                                    datosVenta.Detalle.Add(new Detalle
                                    {
                                        Identificador = row["IdProducto"].ToString() + "-" + row["IdT"] + "-" + Program.IdEstacion + "-" + row["IdTp"] + "-" + row["Numero"],
                                        IdentificadorMovimiento = row["IdT"] + "-" + Program.IdEstacion + "-" + row["IdTp"] + "-" + row["Numero"],
                                        Articulo_id = row["IdProducto"].ToString(),
                                        Cantidad = Convert.ToInt16(row["Cantidad"]),
                                        //promocionOpcion = "",
                                        PrecioUnitario = Convert.ToDecimal(row["Precio"]),
                                        CostoUnitario = Convert.ToDecimal(row["Costo"]),
                                       // NumeroManguera = 0,
                                        //IdDespacho = "",

                                    });
                                    
                                }
                                else
                                {
                                    string json = JsonConvert.SerializeObject(ReportoVenta, Formatting.Indented);
                                   // r.GraboTxt(Program.ruta, "test.txt", DateTime.Now.ToString());
                                    Console.WriteLine(json);

                                    // Envio el Json a la Api de Shell
                                    bool envio = api.PublicoDatos(Program.urlExtraccion, Program.tok.access_token, Program._key, json);
                                    if (envio)
                                    {
                                        // r.GraboTxt(Program.ruta, "test.txt", DateTime.Now.ToString());
                                        r.GraboTxt(Program.ruta, "last.id", row["IdT"].ToString()); // voy grabando el ultimo ID exitoso por la dudas si se corta la luz.. o algun otro imprevisto
                                        r.WriteFullLine("Envio Exitoso.");
                                    }
                                    else
                                    {
                                        r.WriteFullLineNg("No puedo realizarse el Envio de Datos.");
                                        for (int x = 1; x <= 3; x++)
                                        {
                                            r.WriteFullLineNg("Reintento Nro " + x + ":");
                                            bool reintento = api.PublicoDatos(Program.urlExtraccion, Program.tok.access_token, Program._key, json);
                                            if (reintento)
                                            {
                                                r.GraboTxt(Program.ruta, "last.id", row["IdT"].ToString()); // voy grabando el ultimo ID exitoso por la dudas si se corta la luz.. o algun otro imprevisto
                                                r.WriteFullLine("Envio Exitoso.");
                                                break;
                                            }
                                            if (x == 3)
                                            {
                                                r.WriteFullLineNg("3 Intentos Fallidos. Se guardan datos para analizar.");
                                                r.GraboTxt(Program.ruta +@"\fallidos", row["IdT"].ToString(), json);
                                            }

                                        }

                                    

                                    }
                                    datosVenta.Detalle.Clear(); // Limpio todos los detalles.
                                 
                                    nroTicketUltimo = Convert.ToInt32(row["IdT"]);
                                    
                                    ReportoVenta = new Extraccion
                                    {
                                        
                                        idEstacion = Program.IdEstacion,  //viene del config.cfg
                                        rows = new List<Rows>
                                        {
                                            datosVenta
                                        }
                                    };
                                    datosVenta.Identificador = row["IdT"] + "-" + Program.IdEstacion + "-" + row["IdTp"] + "-" + row["Numero"];
                                    // datosVenta.IdCaja = "";// row["IdCaja"].ToString();
                                    //  datosVenta.IdUsuario = "1";
                                    datosVenta.Turno = row["Turno"].ToString();
                                  //  datosVenta.Vendedor = "1";
                                  //  datosVenta.PlantillaVenta = "1";
                                    datosVenta.TipoDeMovimiento = tipoMovimiento;
                                    datosVenta.PuntoDeVentaAFIP = Convert.ToInt32(row["Numero"].ToString().Substring(0, 4));
                                    datosVenta.NumeroComprobante = Convert.ToInt32(row["Numero"].ToString().Substring(5));
                                    datosVenta.FechaEmision = FecheHora;
                                    /*  datosVenta.Cliente = new Cliente
                                      {
                                          id_cliente = "1",
                                          TarjetaLatamPass = "1",
                                          RazonSocial = "1",
                                          CategoriaIVA = "1",
                                          CUIT = "1",
                                          Domicilio = "1",
                                          Localidad = "1",
                                          CodigoPostal = "1",
                                          Provincia = "1",
                                          Pais = "1"
                                      };*/
                                    datosVenta.CondicionVenta = (row["Tipo"].ToString() != "") ? row["Tipo"].ToString() : "Efectivo"; 
                                    datosVenta.Impresora = row["Impresora"].ToString();
                                    //datosVenta.Impuestos_Internos = "Obligatorio";
                                    //datosVenta.Percepciones_IIBB = "Obligatorio";
                                    //datosVenta.Observaciones = "Obs";
                                    datosVenta.Total = Convert.ToDecimal(row["Importe"]);
                                    datosVenta.EstacionOrigen = Convert.ToInt32(Program.IdEstacion);
                                    datosVenta.Detalle.Add(new Detalle
                                    {
                                        Identificador = row["IdProducto"].ToString() + "-" + row["IdT"] + "-" + Program.IdEstacion + "-" + row["IdTp"] + "-" + row["Numero"],
                                        IdentificadorMovimiento = row["IdT"] + "-" + Program.IdEstacion + "-" + row["IdTp"] + "-" + row["Numero"],
                                        Articulo_id = row["IdProducto"].ToString(),
                                        Cantidad = Convert.ToInt16(row["Cantidad"]),
                                        //promocionOpcion = "",
                                        PrecioUnitario = Convert.ToDecimal(row["Precio"]),
                                        CostoUnitario = Convert.ToDecimal(row["Costo"]),
                                       // NumeroManguera = 0,
                                        //IdDespacho = "",

                                    });
                                   
                                }
                            }
                            if (++i == cantidadRegistros) //imprimo el ultimo registro
                            {
                                string json = JsonConvert.SerializeObject(ReportoVenta, Formatting.Indented);

                               // r.GraboTxt(Program.ruta, "test.txt", DateTime.Now.ToString());
                                Console.WriteLine(json);

                                // Envio el Json a la Api de Shell
                                bool envio = api.PublicoDatos(Program.urlExtraccion, Program.tok.access_token, Program._key, json);
                                if (envio)
                                {
                                    r.GraboTxt(Program.ruta, "last.id", row["IdT"].ToString()); // voy grabando el ultimo ID exitoso por la dudas si se corta la luz.. o algun otro imprevisto
                                    r.WriteFullLine("Envio Exitoso.");
                                   // r.GraboTxt(Program.ruta, "test.txt", DateTime.Now.ToString());
                                }
                                else
                                {
                                    r.WriteFullLineNg("No puedo realizarse el Envio de Datos.");
                                    for(int x=1; x <= 3; x++)
                                    {
                                        r.WriteFullLineNg("Reintento Nro " + x +":");
                                        bool reintento = api.PublicoDatos(Program.urlExtraccion, Program.tok.access_token, Program._key, json);
                                        if (reintento)
                                        {
                                            r.GraboTxt(Program.ruta, "last.id", row["IdT"].ToString()); // voy grabando el ultimo ID exitoso por la dudas si se corta la luz.. o algun otro imprevisto
                                            r.WriteFullLine("Envio Exitoso.");
                                            break;
                                        }
                                        if(x == 3)
                                        {
                                            r.WriteFullLineNg("3 Intentos Fallidos. Se guardan datos para analizar.");
                                            r.GraboTxt(Program.ruta + @"\fallidos", row["IdT"].ToString(), json);
                                        }

                                    }



                                }
                                //actualizo last.id con el ultimo registro
                                Program.ultimoIdDeLaLecturaActual = row["IdT"].ToString();
                                //r.GraboTxt(Program.ruta, "last.id", row["IdT"].ToString());
                            }
                          
                        }
                        connection.Close();
                    }
                    return ReportoVenta;
                    
                }
                
            }
            catch (Exception o)
            {
                r.WriteDetalle(Program.database);
                Console.WriteLine("error en SelectTicket "+ o.Message);
                return RegistroVacio();
               // Log(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " Main exception: " + o);
            }
           
        }

        public static Extraccion RegistroVacio()
        {
            Rows datosVenta = new Rows
            {

                Identificador = "",
               // IdCaja = "",
                //IdUsuario = "",
                Turno = "",
                //Vendedor = "",
                //PlantillaVenta = "",
                TipoDeMovimiento = "",
                PuntoDeVentaAFIP = 0,
                NumeroComprobante = 0,
                FechaEmision = "",
              /*  Cliente = new Cliente
                {
                    id_cliente = "",
                    TarjetaLatamPass = "",
                    RazonSocial = "",
                    CategoriaIVA = "",
                    CUIT = "",
                    Domicilio = "",
                    Localidad = "",
                    CodigoPostal = "",
                    Provincia = "",
                    Pais = ""
                },*/
                CondicionVenta = "",
                Impresora = "",
                //Impuestos_Internos = "",
                //Percepciones_IIBB = "",
                //Observaciones = "",
                Total = 0,
                EstacionOrigen = 0,
               /* Detalle = new Detalle
                {
                    Identificador = "",
                    IdentificadorMovimiento = "",
                    Articulo_id = "",
                    Cantidad = 0,
                    promocionOpcion = "",
                    PrecioUnitario = 0,
                    CostoUnitario = 0,
                    NumeroManguera = 0,
                    IdDespacho = ""
                }*/

            };

            Extraccion ReportoVenta = new Extraccion
            {
                idEstacion = "error.",
                rows = new List<Rows>
                             {
                                 datosVenta

                             }
            };
            return ReportoVenta;
        }
    }
}
