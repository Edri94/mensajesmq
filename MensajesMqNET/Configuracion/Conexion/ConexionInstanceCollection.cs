using System.Configuration;

namespace MensajesMqNET.Configuracion.Conexion
{
    public class ConexionInstanceCollection : ConfigurationElementCollection
    {
        public ConexionInstanceElement this[int index]
        {
            get
            {
                return (ConexionInstanceElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        public new ConexionInstanceElement this[string key]
        {
            get
            {
                return (ConexionInstanceElement)BaseGet(key);
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
            return new ConexionInstanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConexionInstanceElement)element).Name;
        }
    }
}
