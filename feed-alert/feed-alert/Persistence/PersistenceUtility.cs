using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace feed_alert.Persistence
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    class PersistenceUtility
    {
        private static Dictionary<Type, DataContractSerializer> serializers = new Dictionary<Type, DataContractSerializer>();

        public static void SerializaToXML<T>(object obj, string file)
        {
            using (FileStream stream = new FileStream(file, FileMode.Create))
            {
                GetSerializer(typeof(T)).WriteObject(stream, obj);
            }
        }

        public static T DeserializeFromXML<T>(string file)
        {
            using (FileStream stream = new FileStream(file, FileMode.Create))
            {
                return (T)GetSerializer(typeof(T)).ReadObject(stream);
            }
        }

        private static DataContractSerializer GetSerializer(Type type)
        {
            DataContractSerializer serializer = serializers[type];
            if (serializer == null)
            {
                serializer = new DataContractSerializer(type);
                serializers[type] = serializer;
            }

            return serializer;
        }
    }
}
