using System.Configuration;

namespace MensajesMqNET.Configuracion.Headerih
{
    public class HeaderihInstanceElement : ConfigurationElement
    {

        [ConfigurationProperty("key", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)base["key"];
            }
            set
            {
                base["key"] = value;
            }
        }


        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get
            {
                return (string)base["value"];
            }
            set
            {
                base["value"] = value;
            }
        }
    }
}
