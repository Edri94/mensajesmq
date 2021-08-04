using System.Configuration;

namespace MensajesMqNET.Configuracion.MqSeries
{
    public class MqSeriesInstanceCollection : ConfigurationElementCollection
    {
        public MqSeriesInstanceElement this[int index]
        {
            get
            {
                return (MqSeriesInstanceElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public new MqSeriesInstanceElement this[string key]
        {
            get
            {
                return (MqSeriesInstanceElement)BaseGet(key);
            }
            set
            {
                if (BaseGet(key) != null)
                    BaseRemoveAt(BaseIndexOf(BaseGet(key)));

                BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new MqSeriesInstanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MqSeriesInstanceElement)element).Name;
        }
    }
}
