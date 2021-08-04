using System.Configuration;

namespace MensajesMqNET.Configuracion.EscribeArchivoLOG
{
    public class EscribeArchivoLOGConfig : ConfigurationSection
    {
        [ConfigurationProperty("instances")]
        [ConfigurationCollection(typeof(EscribeArchivoLOGInstanceCollection))]
        public EscribeArchivoLOGInstanceCollection EscribeArchivoLOGInstances
        {
            get
            {
                // Get the collection and parse it
                return (EscribeArchivoLOGInstanceCollection)this["instances"];
            }
        }

        public string ObtenerParametro(string key)
        {
            string value = "";
            var config = (EscribeArchivoLOGConfig)ConfigurationManager.GetSection("escribeArchivoLOG");

            foreach (EscribeArchivoLOGInstanceElement instance in config.EscribeArchivoLOGInstances)
            {
                if (instance.Name == key)
                {
                    value = instance.Value;
                }
            }

            return value;
        }
    }
}
