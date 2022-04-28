
# Envio de Datos a Azure Microsoft

Programa desarrollado en C# (08/2020)

* Mediante el uso de Apis provistas por el proveedor, se podrá:

    *   Pedir un Token de autorización
    *   Realizar la Adhesión al servicio
    *   Extracción de datos necesarios 
    *   Envío de datos requeridos

El programa recupera datos solicitados de una DB local, los procesa en formato JSON y envia.

La Adhesión se realiza mediante la ejecucion del programa, pasandole 2 parametros de configuración especificos.


## Captura

![App Screenshot](https://github.com/diegobiasatti/EnviosToAzure/blob/main/vista_.JPG?raw=true)


Si el proceso falla por alguna razón, el envio se guarda, y al volverse a iniciar la app, primero chequea si hubo envios fallidos.

Si los hubo primero envia estos y luego continua con la DB.

![App Screenshot](https://github.com/diegobiasatti/EnviosToAzure/blob/main/vista_1.JPG?raw=true)

## Observaciones

Corre en Windows Xp o superior.


##  Desarrollado en
C# Console

.NET Framework 3.5

Library Newtonsoft.Json
