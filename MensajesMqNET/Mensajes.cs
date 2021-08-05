using MensajesMqNET.Configuracion.Conexion;
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
        ConexionConfig conexionConfig;

        MQ mQ;

        public Mensajes()
        {
            mqSeriesConfig = new MqSeriesConfig();
            escribeArchivoLOGConfig = new EscribeArchivoLOGConfig();
            conexionConfig = new ConexionConfig();

            mQ = new MQ();

        }

        public string ProcesarMensajes(string strRutaIni, string strParametros = "")
        {
            string[] Parametros;       // Arreglo para almacenar los parametros via línea de comando
            string Ls_MsgVal = "";       // Mensaje con el resultado de la validación
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
            ConfiguraHeader_IH_ME();

            if(ConectDB())
            {
                return "error conexion";
            }
            //EscribeArchivoLOGConfig("Comienza la función MAIN de la aplicación MensajesMQ: " + DateTime.Now.ToString() + " Tipo Función: '" + gsEjecutable + "'");
            mQ.gsAccesoActual = DateTime.Now.ToString();

            if( ValidaInfoMQ(Ls_MsgVal))
            {
                mQ.psInsertarSQL(mQ.gsAccesoActual, 1, Ls_MsgVal, "MSG", "Valida InfoMQ");
                //Escribe("Termina el acceso a la aplicación MensajesMQ. Cheque la bitácora de errores en SQL. Tipo Función: '" + gsEjecutable + "'");
                //Escribe("");
                Desconectar();
                return "desconectar";
            }

            switch (gsEjecutable)
            {
                case "F":
                    ProcesoBDtoMQQUEUEFunc();
                    break;
                case "A":
                    ProcesoBDtoMQQUEUEAuto();
                    break;
                default:
                    break;
            }
            return "";
        }

        private void ProcesoBDtoMQQUEUEAuto()
        {
            string Ls_MensajeMQ;       // Cadena con el mensaje armado con los registros de la base de datos
            string Ls_MsgColector;       // Cadena para almecenar el COLECTOR
            string Ls_HeaderMsg;       // Cadena para almacenar el HEADER del mensaje
            string strQuery;       // Cadena para almacenar el Query a ejecutarse en la base de datos
            int NumeroMsgEnviados;      // Contador para almacenar el número de mensajes procesados
            string[] las_Autorizaciones;    // Arreglo para ingresar todos los registros que han sido enviados correctamente
                                            // Para el armado de la solicitud
            string ls_Operacion;    // 1  operacion
            string ls_Oficina;    // 2  oficina
            string ls_NumeroFunc;    // 3  codusu
            string ls_Transaccion;    // 4  transaccion
            string ls_CodigoOperacion;    // 5  tipo-oper
            string ls_Cuenta;    // 6  cuenta-ced
            string ls_Divisa;    // 7  divisa
            string ls_Importe;    // 8  importe
            string ls_Fecha_Ope;    // 9  Fecha (operacion)
            string ls_Folio_Ope;    // 10 Folio
            string ls_Status_Envio;    // 11 Status
            string ls_Fecha;
            string ls_Hora;

            strQuery = "";

            try
            {
                //Escribe("Inicia el envío de mensajes a Host: " + mQ.gsAccesoActual + " Función: " + strFuncionSQL);
                NumeroMsgEnviados = 0;

                strQuery = "SELECT" + (char)13;
                strQuery = strQuery + "operacion," + (char)13;                         
                strQuery = strQuery + "oficina," + (char)13;                          
                strQuery = strQuery + "numero_funcionario," + (char)13;                
                strQuery = strQuery + "id_transaccion," + (char)13;                    
                strQuery = strQuery + "codigo_operacion," + (char)13;                 
                strQuery = strQuery + "cuenta," + (char)13;                            
                strQuery = strQuery + "divisa," + (char)13;                           
                strQuery = strQuery + "importe," + (char)13;                          
                strQuery = strQuery + "fecha_operacion," + (char)13;                   
                strQuery = strQuery + "folio_autorizacion," + (char)13;                
                strQuery = strQuery + "status_envio," + (char)13;                      
                strQuery = strQuery + "CONVERT(char(8),getdate(),112)," + (char)13;    
                strQuery = strQuery + "CONVERT(char(5),getdate(),108)" + (char)13;     
                strQuery = strQuery + "FROM " + (char)13;
                strQuery = strQuery + mQ.gsNameDB + "..TMP_AUTORIZACIONES_PU" + (char)13;
                strQuery = strQuery + "WHERE status_envio = 0";

                if(mQ.rssRegistro != null)
                {
                    if(mQ.MQConectar(Gs_MQManager, mQ.mqManager))
                    {
                        mQ.blnConectado = true;
                    }
                    else
                    {
                        mQ.psInsertarSQL(mQ.gsAccesoActual, 3, "ProcesoBDtoMQQUEUEAuto. Fallo conexión MQ-Manager " + Gs_MQManager + ":  mqSession.ReasonCode  -   mqSession.ReasonName", "MSG", "MQConectar");
                        return;
                    }

                    int i = 0;
                    do
                    {
                        i++;



                    } while ( i < mQ.rssRegistro.Count() );
                }
                
                //rssRegistro.Open strQuery



            }
            catch (Exception)
            {

                throw;
            }

        }

        private void ProcesoBDtoMQQUEUEFunc()
        {
            string Ls_MensajeMQ;       // Cadena con el mensaje armado con los registros de la base de datos
            string Ls_MsgColector;       // Cadena para almecenar el COLECTOR
            string Ls_HeaderMsg;       // Cadena para almacenar el HEADER del mensaje
            int NumeroMsgEnviados;      // Contador para almacenar el número de mensajes procesados
            string[] las_Funcionarios;       // Arreglo para ingresar todos los registros que han sido enviados correctamente
                                             // Para el armado de la solicitud
            string ls_IDFuncionario;
            string ls_CentroRegional;       // 1  centro_regional
            string ls_NumRegistro;       // 2  numero_registro
            string ls_Producto;       // 3  producto
            string ls_SubProducto;       // 4  subproducto
            string ls_FechaAlta = "0000/00/00";       // 5  fecha_alta
            string ls_TipoPeticion;       // 8  tipo_peticion
            string ls_IdTransaccion;       // 12 id_transaccion
            string ls_Tipo;       // 13 tipo
            string ls_Fecha;
            string ls_Hora = "00:00";

            string strQuery;

            try
            {
                //Escribe("Inicia el envío de mensajes a Host: " + mQ.gsAccesoActual + " Función: " + strFuncionSQL);
                NumeroMsgEnviados = 0;
                
                // Logica para recuperar los n mensajes de la tabla temporal en db.funcionario
                // Logica para procesar cada registro y convertirlo en un mensaje
                strQuery = "SELECT" + (char)13;
                strQuery = strQuery + "id_funcionario," + (char)13;                       // 0  id_funcionario
                strQuery = strQuery + "centro_regional," + (char)13;                      // 1  centro_regional
                strQuery = strQuery + "numero_funcionario," + (char)13;                   // 2  numero_
                strQuery = strQuery + "producto," + (char)13;                             // 3  producto
                strQuery = strQuery + "subproducto," + (char)13;                          // 4  subproducto
                strQuery = strQuery + "CONVERT(char(11), fecha_alta, 105) + CONVERT(char(5), fecha_alta, 108)," + (char)13;                           // 5  fecha_alta
                strQuery = strQuery + "CONVERT(char(11), fecha_baja, 105) + CONVERT(char(5), fecha_baja, 108)," + (char)13;                           // 6  fecha_baja
                strQuery = strQuery + "CONVERT(char(11), fecha_ultimo_mant, 105) + CONVERT(char(6), fecha_ultimo_mant, 108)," + (char)13;                    // 7  fecha_ultimo_mant
                strQuery = strQuery + "tipo_peticion," + (char)13;                        // 8  tipo_peticion
                strQuery = strQuery + "status_envio," + (char)13;                          // 9  status_envio
                strQuery = strQuery + "CONVERT(char(8),getdate(),112)," + (char)13;        // 10
                strQuery = strQuery + "CONVERT(char(5),getdate(),108)," + (char)13;        // 11
                strQuery = strQuery + "id_transaccion," + (char)13;                        // 12  id transaccion en TKT
                strQuery = strQuery + "tipo " + (char)13;                                  // 13  Tipo  A-Alta, B-Baja, M-Mantenimiento
                strQuery = strQuery + "FROM" + (char)13;
                strQuery = strQuery + mQ.gsNameDB + "..TMP_FUNCIONARIOS_PU" + (char)13;
                strQuery = strQuery + "WHERE status_envio = 0";

                if(true) //Not rssRegistro.EOF
                {
                    if (mQ.MQConectar(Gs_MQManager, mQ.mqManager))
                    {
                        mQ.blnConectado = true;
                    }
                    else
                    {
                        mQ.psInsertarSQL(mQ.gsAccesoActual, 3 , "ProcesoBDtoMQQUEUEFunc. Fallo conexión MQ-Manager " + Gs_MQManager + ": mqSession.ReasonCode -  mqSession.ReasonName", "MSG", "MQConectar");
                        return;
                    }

                    do
                    {
                        //Almacenando variables
                        ls_IDFuncionario = Left(mQ.rssRegistro[0].ToString(), 7);
                        ls_CentroRegional = Left(mQ.rssRegistro[1], 4);
                        ls_NumRegistro = Left(mQ.rssRegistro[3].ToString(), 8);
                        ls_Producto = Left(mQ.rssRegistro[4].ToString(), 2);
                        ls_SubProducto = Left(mQ.rssRegistro[5].ToString(), 10);

                        if(mQ.rssRegistro[5] != "")
                        {
                            ls_FechaAlta = mQ.rssRegistro[5];
                            //ls_FechaAlta = Mid(ls_FechaAlta, 1, 10)
                        }

                        ls_TipoPeticion = Left(mQ.rssRegistro[8], 1);
                        ls_Fecha = Left(mQ.rssRegistro[10] + Space(8),8);
                        ls_IdTransaccion = Left(mQ.rssRegistro[12], 10);
                        ls_Tipo = Left(mQ.rssRegistro[13], 1);

                        Ls_MsgColector = Left(strFuncionSQL.Trim() + "        ", 8);
                        Ls_MsgColector = Ls_MsgColector + ls_Fecha + ls_Hora;
                        Ls_MsgColector = Ls_MsgColector + ls_TipoPeticion + ls_CentroRegional;
                        Ls_MsgColector = Ls_MsgColector + ls_NumRegistro + ls_Producto;
                        Ls_MsgColector = Ls_MsgColector + ls_SubProducto + ls_FechaAlta;
                        Ls_MsgColector = Ls_MsgColector + strFechaBaja + ls_IDFuncionario;
                        Ls_MsgColector = Ls_MsgColector + ls_IdTransaccion + ls_Tipo;
                        Ls_MsgColector = Ls_MsgColector + Space(43);


                    } while (true);

                    if(Ls_MsgColector.Length > 0)
                    {
                        Ls_MensajeMQ = ASTA_ENTRADA(Ls_MsgColector, " Funcionario: " + ls_IDFuncionario);
                        if(Ls_MensajeMQ != "")
                        {
                            //Escribe ("Mensaje Enviado: " + Ls_MensajeMQ);
                            if(mQ.MQEnviarMsg(mQ.mqManager, Gs_MQQueueEscritura, mQ.mqsEscribir,mQ.mqsMsgEscribir, Ls_MensajeMQ,strReplyToMQ, sPersistencia, sExpirar))
                            {
                                //ReDim Preserve las_Funcionarios(NumeroMsgEnviados)
                                las_Funcionarios[NumeroMsgEnviados] = ls_IDFuncionario;
                                NumeroMsgEnviados = NumeroMsgEnviados + 1;
                            }
                            else
                            {
                                mQ.psInsertarSQL(mQ.gsAccesoActual, 4 , "ProcesoBDtoMQQUEUEFunc. Error durante el armado del formato PS9 funcion ASTA_ENTRADA. Error con el Funcionario: " + ls_IDFuncionario, "MSG", "ASTA_ENTRADA");
                            }
                        }
                        else
                        {
                            mQ.psInsertarSQL(mQ.gsAccesoActual, 4, "ProcesoBDtoMQQUEUEFunc. Error durante el armado del formato PS9 funcion ASTA_ENTRADA. Error con el Funcionario: " + ls_IDFuncionario, "MSG", "ASTA_ENTRADA");
                        }
                    }
                    else
                    {
                        //Escribe("No existen registros en la consulta de los datos de tabla TMP_FUNCIONARIOS_PU. ProcesoBDtoMQQUEUEFunc");
                    }


                    mQ.MQDesconectar(mQ.mqManager, mQ.mqsEscribir);

                    if(NumeroMsgEnviados > 0)
                    {
                        if(!ActualizaRegistrosFunc(las_Funcionarios))
                        {
                            //Escribe("Existieron errores al actualizar la tabla TMP_FUNCIONARIOS_PU");
                        }
                    }

                    //Escribe("Envio de solicitures TKT -> Host Terminado. ProcesoBDtoMQQUEUEFunc");
                    //Escribe("Solicitudes enviadas a MQ: " + NumeroMsgEnviados);

                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        private bool ActualizaRegistrosFunc(string[] IDFuncionario)
        {
            bool ActualizaRegistrosFunc = false;

            string strQueryUpDate;
            int ln_indice;

            ActualizaRegistrosFunc = true;

            for (ln_indice = 0; ln_indice < (IDFuncionario.Count()); ln_indice++)
            {
                strQueryUpDate = "UPDATE " + mQ.gsNameDB + "..TMP_FUNCIONARIOS_PU" + (char)13;
                strQueryUpDate = strQueryUpDate + "SET  status_envio = 1" + (char)13;
                strQueryUpDate = strQueryUpDate + "--  ,fecha_ultimo_mant = GETDATE()," + (char)13;
                strQueryUpDate = strQueryUpDate + "WHERE status_envio = 0" + (char)13;
                strQueryUpDate = strQueryUpDate + "AND id_funcionario = " + IDFuncionario[ln_indice];
                //rssRegistro.Open strQueryUpDate
            }


            return ActualizaRegistrosFunc;
        }

        private string ASTA_ENTRADA(string strMsgColector, string psTipo)
        {
            string ls_TempColectorMsg;
            string ls_BloqueME;
            int ln_longCOLECTOR;
            int ln_AccTerminal;

            string ASTA_ENTRADA = "";

            try
            {
                ls_TempColectorMsg = strMsgColector;

                if(ls_TempColectorMsg.Length >  Int32.Parse(strColectorMaxLeng))
                {
                    //Escribe("La longitud del colector supera el maximo permitido");
                    return "ErrorASTA";
                }

                ls_BloqueME = Left(strMETAGINI +"    ", 4);
                ls_BloqueME = ls_BloqueME + Right("0000" + ls_TempColectorMsg.Length.ToString(),4);
                ls_BloqueME = ls_BloqueME + Left(strMsgTypeCole.Trim() + " ", 1);
                ls_BloqueME = ls_BloqueME + ls_TempColectorMsg;
                ls_BloqueME = ls_BloqueME + Left(strMETAGEND.Trim() + "     ", 5);


                if(ls_BloqueME.Length > Int32.Parse(strMsgMaxLeng.Trim()))
                {
                    //Escribe("La longitud del Bloque ME supera el maximo permitido");
                    return "ErrorASTA";
                }

                //'Para el uso de MQ-SERIES y CICSBRIDGE se requiere anteponer
                //'al HEADER DE ENTRADA(IH) un valor que indique el programa
                //'que invocara el CICSBRIDGE
                //'X(08)  Indica el programa que invocara el CICSBRIDGE
                ASTA_ENTRADA = Left(strFuncionHost.Trim() + "        ", 8);
                ASTA_ENTRADA = ASTA_ENTRADA + Left(strHeaderTagIni.Trim() + "    ", 4);
                ASTA_ENTRADA = ASTA_ENTRADA + Left(strIDProtocol.Trim() + "  ", 2);

                if(strRndLogTerm.Trim().Equals("1"))
                {
                    ln_AccTerminal = 0;
                    do
                    {
                        var Rnd = new Random(DateTime.Now.Second * 1000);
                        ln_AccTerminal = Rnd.Next();
                    } while (ln_AccTerminal > 0 && ln_AccTerminal < 2000);
                    ASTA_ENTRADA = ASTA_ENTRADA + Left(ln_AccTerminal.ToString("D4") + "        ", 8); ;
                }
                else
                {
                    ASTA_ENTRADA = ASTA_ENTRADA + Left(strLogical.Trim() + "        ", 8);
                }


                ASTA_ENTRADA = ASTA_ENTRADA + Left(strAccount.Trim() + "        ", 8);
                ASTA_ENTRADA = ASTA_ENTRADA + Left(strUser.Trim() + "        ", 8);
                ASTA_ENTRADA = ASTA_ENTRADA + Left(strSeqNumber.Trim() + "        ", 8);
                ASTA_ENTRADA = ASTA_ENTRADA + Left(strTXCode.Trim() + "        ", 8);
                ASTA_ENTRADA = ASTA_ENTRADA + Left(strUserOption.Trim() + "  ", 2);

                ln_longCOLECTOR = 65 + ls_BloqueME.Length;

                if(ln_longCOLECTOR > Int32.Parse(strPS9MaxLeng))
                {
                    //Escribe("La longitud del Layout PS9 supera el maximo permitido");
                    return "ErrorASTA";
                }

                ASTA_ENTRADA = ASTA_ENTRADA + Right("00000" + ln_longCOLECTOR.ToString(), 5);
                ASTA_ENTRADA = ASTA_ENTRADA + Left((strCommit).Trim() + " ", 1);
                ASTA_ENTRADA = ASTA_ENTRADA + Left((strMsgType).Trim() + " ", 1);
                ASTA_ENTRADA = ASTA_ENTRADA + Left((strProcessType).Trim() + " ", 1);
                ASTA_ENTRADA = ASTA_ENTRADA + Left((strChannel).Trim() + "  ", 2);
                ASTA_ENTRADA = ASTA_ENTRADA + Left((strPreFormat).Trim() + " ", 1);
                ASTA_ENTRADA = ASTA_ENTRADA + Left((strLenguage).Trim() + " ", 1);
                ASTA_ENTRADA = ASTA_ENTRADA + Left((strHeaderTagEnd).Trim() + "     ", 5);
                ASTA_ENTRADA = ASTA_ENTRADA + ls_BloqueME;


            }
            catch (Exception)
            {

                throw;
            }

            return "";
        }

        private bool ValidaInfoMQ(string ps_MsgVal)
        {
            string ls_msg = "";

            if(Gs_MQManager.Trim() == "")
            {
                ls_msg = ls_msg + "";
            }
            if(Gs_MQQueueEscritura.Trim() == "")
            {
                ls_msg = ls_msg + "";
            }
            if(ls_msg == "")
            {
                return true;
            }
            ps_MsgVal = ls_msg;
            return false;
        }

        private void ConfiguraHeader_IH_ME()
        {
            throw new NotImplementedException();
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

        private bool ConectDB()
        {
            try
            {
                mQ.gsCataDB = conexionConfig.ObtenerParametro("DBCata");
                mQ.gsDSNDB = conexionConfig.ObtenerParametro("DBDSN");
                mQ.gsUserDB = conexionConfig.ObtenerParametro("DBUser");
                mQ.gsPswdDB = conexionConfig.ObtenerParametro("DBPswd");
                mQ.gsNameDB = conexionConfig.ObtenerParametro("DBName");

                mQ.cnnConexion = new ConexionBDSQL.ConexionBD("");

                //mQ.cnnConexion.OpenTransaction();

            }
            catch (Exception)
            {

                throw;
            }
            return true;
        }

        private void Desconectar()
        {
            //mQ.cnnConexion.Close();
            //Set rssRegistro = Nothing
            mQ.cnnConexion = null;
        }

        public string Space(int veces)
        {
            return new String(' ', veces);
        }

        public string Left(string cadena, int posiciones)
        {
            return cadena.Substring(0, posiciones);
        }

        public string Right(string cadena, int posiciones)
        {
            return cadena.Substring((cadena.Length - posiciones), posiciones);
        }

    }
}
