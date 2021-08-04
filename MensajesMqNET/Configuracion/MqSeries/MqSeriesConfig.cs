using System.Configuration;

namespace MensajesMqNET.Configuracion.MqSeries
{
    public class MqSeriesConfig : ConfigurationSection
    {
        [ConfigurationProperty("instances")]
        [ConfigurationCollection(typeof(MqSeriesInstanceCollection))]
        public MqSeriesInstanceCollection MqSeriesInstances
        {
            get
            {
                return (MqSeriesInstanceCollection)this["instances"];
            }
        }

        public string ObtenerParametro(string key)
        {
            string value = "";
            var config = (MqSeriesConfig)ConfigurationManager.GetSection("mqSeries");

            foreach (MqSeriesInstanceElement instance in config.MqSeriesInstances)
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
