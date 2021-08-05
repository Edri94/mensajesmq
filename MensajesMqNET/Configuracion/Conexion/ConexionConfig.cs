using System.Configuration;

namespace MensajesMqNET.Configuracion.Conexion
{
    public class ConexionConfig : ConfigurationSection
    {
        [ConfigurationProperty("instances")]
        [ConfigurationCollection(typeof(ConexionInstanceCollection))]
        public ConexionInstanceCollection ConexionInstances
        {
            get
            {
                return (ConexionInstanceCollection)this["instances"];
            }
        }

        public string ObtenerParametro(string key)
        {
            string value = "";
            var config = (ConexionConfig)ConfigurationManager.GetSection("conexion");

            foreach (ConexionInstanceElement instance in config.ConexionInstances)
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
