using BitacorasNET.Configuracion.MqSeries;
using MensajesMqNET.Configuracion.EscribeArchivoLOG;
using MensajesMqNET.Configuracion.MqSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensajesMqNET
{
    public class Mensajes
    {
        private string Archivo;
        private string ArchivoIni;
        private string Ls_Archivo;
        private string lsCommandLine;

        // Variables para el control del log
        private string strlogFileName;
        private string strlogFilePath;
        private bool Mb_GrabaLog;

        // Variables para el registro de los valores del header IH
        private string strFuncionHost; // Valor que indica el programa que invocara el CICSBRIDGE
        private string strHeaderTagIni; // Bandera que indica el comienzo del Header
        private string strIDProtocol; // Identificador  del protocolo (PS9)
        private string strLogical; // Terminal Lógico Asigna Arquitectura ASTA
        private string strAccount; // Terminal Contable (CR Contable)
        private string strUser; // Usuario. Debe ser diferente de espacios
        private string strSeqNumber; // Número de Secuencia (indicador de paginación)
        private string strTXCode; // Función específica Asigna Arquitectura Central
        private string strUserOption; // Tecla de función (no aplica)
        private string strCommit; // Indicador de commit: Permite realizar commit
        private string strMsgType; // Tipo de mensaje: Nuevo requerimiento
        private string strProcessType; // Tipo de proceso: on line
        private string strChannel; // Canal Asigna Arquitectura Central
        private string strPreFormat; // Indicador de preformateo: Arquitectura no deberá de preformatear los datos
        private string strLenguage; // Idioma: Español
        private string strHeaderTagEnd; // Bandera que indica el final del header

        // Variables para el registro de los valores del header ME
        private string strMETAGINI; // Bandera que indica el comienzo del mensaje
                                    // Private strMsgColecMax          As String 'Longitud del layout  del colector
        private string strMsgTypeCole; // Tipo de mensaje: Copy
                                       // Private strMaxMsgCole           As String 'Máximo X(30641)
        private string strMETAGEND; // Bandera que indica el fin del mensaje

        // Variables para el registro de los valores Default
        private string strFechaBaja; // fecha_baja
        private string strColectorMaxLeng; // Maxima longitud del COLECTOR
        private string strMsgMaxLeng; // Maxima longitud del del bloque ME
        private string strPS9MaxLeng; // Maxima longitud del formato PS9
        private string strReplyToMQ; // MQueue de respuesta para HOST
        private string strFuncionSQL; // Funcion a ejecutar al recibir la respuesta
        private string strRndLogTerm; // Indica que el atributo Logical Terminal es random

        // Variables para el manejo de los parametros de la base de datos
        // Public gsSeccRegWdw             As String

        // VARIABLES NUVAS PARA EL ENVIO DE MENSAJE
        private string sPersistencia;
        private string sExpirar;

        private string Gs_MQManager;       // MQManager de Escritura
        private string Gs_MQQueueEscritura;       // MQQueue de Escritura
        private string gsEjecutable;       // Ejecutable a realizar

        public string Bandera;

        MqSeriesConfig mqSeriesConfig;
        EscribeArchivoLOGConfig escribeArchivoLOGConfig;

        public Mensajes()
        {
            mqSeriesConfig = new MqSeriesConfig();
            escribeArchivoLOGConfig = new EscribeArchivoLOGConfig();
        }

        public string ProcesarMensajes(string strRutaIni, string strParametros = "")
        {
            string[] Parametros;       // Arreglo para almacenar los parametros via línea de comando
            string Ls_MsgVal;       // Mensaje con el resultado de la validación
            float LnDiferencia;       // Minutos transcurridos desde el último intento de acceso

            //ArchivoIni = strRutaIni + @"\MensajesMQ.ini";
            //gstrRutaIni = ArchivoIni;

            lsCommandLine = strParametros.Trim();

            if(lsCommandLine.Equals("") == false)
            {
                //Array.Clear(Parametros, 0, Parametros.Length);
                Parametros = lsCommandLine.Split('-');
                Gs_MQManager = Parametros[0].Trim();
                Gs_MQQueueEscritura = Parametros[1].Trim();
                gsEjecutable = Parametros[2].Trim();


            }
            else
            {
                ObtenerInfoMQ();
            }

            ConfiguraFileLog();

            return "";
        }

        private void ObtenerInfoMQ()
        {
            Gs_MQManager = mqSeriesConfig.ObtenerParametro("MQManager");
            Gs_MQQueueEscritura=  mqSeriesConfig.ObtenerParametro("MQEscritura");
            gsEjecutable = mqSeriesConfig.ObtenerParametro("FGEjecutable");
        }

        private void ConfiguraFileLog()
        {
            strlogFileName = escribeArchivoLOGConfig.ObtenerParametro("logFileName");
            strlogFilePath = escribeArchivoLOGConfig.ObtenerParametro("logFilePath");
            Mb_GrabaLog = Boolean.Parse(escribeArchivoLOGConfig.ObtenerParametro("Estatus"));
        }

    }
}
