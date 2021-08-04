using System.Configuration;

namespace MensajesMqNET.Configuracion.EscribeArchivoLOG
{
    public class EscribeArchivoLOGInstanceCollection : ConfigurationElementCollection
    {
        public EscribeArchivoLOGInstanceElement this[int index]
        {
            get
            {
                return (EscribeArchivoLOGInstanceElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public new EscribeArchivoLOGInstanceElement this[string key]
        {
            get
            {
                return (EscribeArchivoLOGInstanceElement)BaseGet(key);
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
            return new EscribeArchivoLOGInstanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EscribeArchivoLOGInstanceElement)element).Name;
        }

    }
}
