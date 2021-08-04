using System.Configuration;

namespace MensajesMqNET.Configuracion.Headerme
{
    public class HeadermeConfig : ConfigurationSection
    {
        [ConfigurationProperty("instances")]
        [ConfigurationCollection(typeof(HeadermeInstanceCollection))]
        public HeadermeInstanceCollection HeadermeInstances
        {
            get
            {
                return (HeadermeInstanceCollection)this["instances"];
            }
        }

        public string ObtenerParametro(string key)
        {
            string value = "";
            var config = (HeadermeConfig)ConfigurationManager.GetSection("escribeArchivoLOG");

            foreach (HeadermeInstanceElement instance in config.HeadermeInstances)
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
