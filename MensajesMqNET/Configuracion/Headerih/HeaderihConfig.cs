using System.Configuration;

namespace MensajesMqNET.Configuracion.Headerih
{
    public class HeaderihConfig : ConfigurationSection
    {
        [ConfigurationProperty("instances")]
        [ConfigurationCollection(typeof(HeaderihInstanceCollection))]
        public HeaderihInstanceCollection HeaderihInstances
        {
            get
            {
                return (HeaderihInstanceCollection)this["instances"];
            }
        }

        public string ObtenerParametro(string key)
        {
            string value = "";
            var config = (HeaderihConfig)ConfigurationManager.GetSection("escribeArchivoLOG");

            foreach (HeaderihInstanceElement instance in config.HeaderihInstances)
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
