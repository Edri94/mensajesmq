using System.Configuration;

namespace MensajesMqNET.Configuracion.Headerme
{
    public class HeadermeInstanceCollection : ConfigurationElementCollection
    {
        public HeadermeInstanceElement this[int index]
        {
            get
            {
                return (HeadermeInstanceElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public new HeadermeInstanceElement this[string key]
        {
            get
            {
                return (HeadermeInstanceElement)BaseGet(key);
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
            return new HeadermeInstanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HeadermeInstanceElement)element).Name;
        }
    }
}
