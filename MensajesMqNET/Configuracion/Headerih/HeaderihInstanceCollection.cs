using System.Configuration;

namespace MensajesMqNET.Configuracion.Headerih
{
    public class HeaderihInstanceCollection : ConfigurationElementCollection
    {
        public HeaderihInstanceElement this[int index]
        {
            get
            {
                return (HeaderihInstanceElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public new HeaderihInstanceElement this[string key]
        {
            get
            {
                return (HeaderihInstanceElement)BaseGet(key);
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
            return new HeaderihInstanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((HeaderihInstanceElement)element).Name;
        }
    }
}
