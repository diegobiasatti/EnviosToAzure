using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace enviosShell
{  
    public class Cliente
    {
        public string id_cliente { get; set; }
        public string TarjetaLatamPass { get; set; }
        public string RazonSocial { get; set; }
        public string CategoriaIVA { get; set; }
        public string CUIT { get; set; }
        public string Domicilio { get; set; }
        public string Localidad { get; set; }
        public string CodigoPostal { get; set; }
        public string Provincia { get; set; }
        public string Pais { get; set; }    


    }
  

    public class Detalle
    {
        [JsonProperty(Order = 20)]
        public string Identificador { get; set; }

        [JsonProperty(Order = 21)]
        public string IdentificadorMovimiento { get; set; }

        [JsonProperty(Order = 22)]
        //public Articulo Articulo;
        public string Articulo_id { get; set; }
        [JsonProperty(Order = 23)]
        public decimal Cantidad { get; set; }
        //[JsonProperty(Order = 24)]
       // public string promocionOpcion { get; set; }
        [JsonProperty(Order = 25)]
        public decimal PrecioUnitario { get; set; }
        [JsonProperty(Order = 26)]
        public decimal CostoUnitario { get; set; }
        
       // [JsonProperty(Order = 27)]
       //  public int NumeroManguera { get; set; }

        //en mail del 09/03/2021  se definio:::
        //  "Surtidor":1, -> En base a la consulta se agregó este campo, siendo el mismo opcional. 

        // [JsonProperty(Order = 28)]
        //  public string IdDespacho { get; set; }
    }


    public class Extraccion
    {
        public string idEstacion { get; set; }

       // [JsonProperty(ItemIsReference = true)] // agrega los indices visibles 
        public IList<Rows> rows { get; set; }
      
    }

    public class Rows
    {
        [JsonProperty(Order = 1)] 
        public string Identificador { get; set; }
        
        //[JsonProperty(Order = 2)]
        //public string IdCaja { get; set; }

        //[JsonProperty(Order = 3)]
        //public string IdUsuario { get; set; }

        [JsonProperty(Order = 4)]
        public string Turno { get; set; }

       // [JsonProperty(Order = 5)]
       // public string Vendedor { get; set; }

       // [JsonProperty(Order = 6)]
       // public string PlantillaDeVenta { get; set; }

        [JsonProperty(Order = 7)]
        public string TipoDeMovimiento { get; set; }

        [JsonProperty(Order = 8)]
        public int PuntoDeVentaAFIP { get; set; }

        [JsonProperty(Order = 9)]
        public int NumeroComprobante { get; set; }

        [JsonProperty(Order = 10)]
        public string FechaEmision { get; set; }

      //  [JsonProperty(Order = 11)]
      //  public Cliente Cliente;

        [JsonProperty(Order = 12)]
        public string CondicionVenta { get; set; }

        [JsonProperty(Order = 13)]
        public string Impresora { get; set; }

       // [JsonProperty(Order = 14)]
       // public string Impuestos_Internos { get; set; }

       // [JsonProperty(Order = 15)]
       // public string Percepciones_IIBB { get; set; }

      //  [JsonProperty(Order = 16)]
      //  public string Observaciones { get; set; }

        [JsonProperty(Order = 17)]
        public Decimal Total { get; set; }

        [JsonProperty(Order = 18)]
        public int EstacionOrigen { get; set; }

        [JsonProperty(Order = 19)]
        public IList<Detalle> Detalle = new List<Detalle>();// { get; set; }
        

    }
}
